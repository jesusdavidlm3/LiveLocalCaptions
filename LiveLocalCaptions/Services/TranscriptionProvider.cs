using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using LiveLocalCaptions.Interfaces;
using NAudio.CoreAudioApi;
using Whisper.net;
using Whisper.net.Ggml;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace LiveLocalCaptions;

public class TranscriptionProvider
{
    private readonly IHistoryService _history;
    private GgmlType Model = GgmlType.Base;
    private string ModelName { get; set; }
    
    //Configuracion de la captura de audio
    private int bytesPerSecond = 48000 * 2 * 2;     //samples por segundo, profundidad en bytes, canales
    private int segmentDurationSeconds = 3;
    private int segmentSize;
    private WaveFormat sourceFormat = new WaveFormat(48000, 16, 2 );
    private BufferedWaveProvider bufferedWaveProvider;
    private WhisperProcessor processor;
    private WhisperFactory whisperFactory;
    private WasapiLoopbackCapture capture = new WasapiLoopbackCapture();

    public TranscriptionProvider(IHistoryService history)
    {
        ModelName = $"model-{Model}.bin";
        _history = history;
        segmentSize = bytesPerSecond * segmentDurationSeconds;
        bufferedWaveProvider = new BufferedWaveProvider(sourceFormat);
        bufferedWaveProvider.BufferLength = segmentSize * 2;
    }

    public bool VerifyModel()
    {
        return File.Exists(ModelName);
    }

    private async Task Prepare()
    {
        await DownloadModel();
        BuildWhisper();
    }

    public async Task DownloadModel()
    {
        using var modelStream = await WhisperGgmlDownloader.Default.GetGgmlModelAsync(Model);
        using (var fileWriter = File.OpenWrite(ModelName))
        {
            await modelStream.CopyToAsync(fileWriter);
        }
    }

    public void BuildWhisper()
    {
        whisperFactory = WhisperFactory.FromPath(ModelName);
        processor = whisperFactory.CreateBuilder()
            .WithLanguage("auto")
            .Build();
    }
    
    public async Task ChangeSettings(GgmlType newModel)
    {
        Model = newModel;
        ModelName = $"model-{Model}.bin";
    }

    public void Transcript()
    {
        capture = new WasapiLoopbackCapture();
        capture.WaveFormat = sourceFormat;
        capture.DataAvailable += async (s, e) =>
        {
            if (bufferedWaveProvider.BufferedBytes < segmentSize)
            {
                bufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
            }
            else
            {
                //Resamplear a 16KHz, 16 bits, mono
                byte[] chunck = new byte[segmentSize];
                int toRead = 0;
                while (toRead < segmentSize)
                {
                    int r = bufferedWaveProvider.Read(chunck, toRead, segmentSize - toRead);
                    if (r == 0)
                    {
                        Thread.Sleep(5000);
                        continue;
                    }
                    toRead += r;
                }
        
                var ms = new MemoryStream(chunck, false);
                var rawMemoryStream = new RawSourceWaveStream(ms, sourceFormat);
                var sampleProvider = rawMemoryStream.ToSampleProvider();

                var stereoToMono = new StereoToMonoSampleProvider(sampleProvider);
        
                var resampler = new WdlResamplingSampleProvider(stereoToMono, 16000);
                // var waveProvider16 = new SampleToWaveProvider16(resampler);
                using var msOut = new MemoryStream();
                List<float> samples = new List<float>();
                float[] buffer = new float[resampler.WaveFormat.AverageBytesPerSecond * 10];
                int read;
                while ((read = resampler.Read(buffer, 0, buffer.Length)) > 0)
                {
                    for (int i = 0; i < read; i++)
                    {
                        samples.Add(buffer[i]);
                    }
                }

                //Modelo de transcripcion
                try
                {
                    await foreach (var result in processor.ProcessAsync(samples.ToArray())) 
                    {
                        _history.Add(result.Text);
                    }            
                }catch(Exception ex)
                {
                    if (processor == null)
                    {
                        _history.Add("Downloading transcription model, please Wait");
                    }
                    else
                    {
                        _history.Add("Theres no audio to transcribe");
                    }
                }
            }
        };
        capture.StartRecording();
    }

    public void StopTranscription()
    {
        capture.StopRecording();
        capture.Dispose();
        bufferedWaveProvider.ClearBuffer();
    }
}