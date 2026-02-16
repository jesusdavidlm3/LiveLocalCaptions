using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using LiveLocalCaptions.Interfaces;
using Whisper.net;
using Whisper.net.Ggml;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace LiveLocalCaptions;

public class TranscriptionProvider
{
    private string modelName = "ggml-base.bin";
    private readonly IHistoryService _history;
    
    //Configuracion de la captura de audio
    private MemoryStream bufferStream = new MemoryStream();
    private int bytesPerSecond = 48000 * 2 * 2;     //samples por segundo, profundidad en bytes, canales
    private int segmentDurationSeconds = 3;
    private int segmentSize;
    private WaveFormat sourceFormat = new WaveFormat(48000, 16, 2 );
    private WaveFormat targetFormat = new WaveFormat(16000, 16, 1);
    private BufferedWaveProvider bufferedWaveProvider;
    private WhisperProcessor processor;
    private WhisperFactory whisperFactory;

    public TranscriptionProvider(IHistoryService history)
    {
        _history = history;
        segmentSize = bytesPerSecond * segmentDurationSeconds;
        bufferedWaveProvider = new BufferedWaveProvider(sourceFormat);
        bufferedWaveProvider.BufferLength = segmentSize * 2;
        if (!File.Exists(modelName))
        {
            DownloadModel();
        }
        else
        {
            whisperFactory = WhisperFactory.FromPath("ggml-base.bin");
            processor = whisperFactory.CreateBuilder()
                .WithLanguage("en")
                .Build();
        }
    }

    private async void DownloadModel()
    {
        using var modelStream = await WhisperGgmlDownloader.Default.GetGgmlModelAsync(GgmlType.BaseEn);
        using var fileWriter = File.OpenWrite(modelName);
        await modelStream.CopyToAsync(fileWriter);
        whisperFactory = WhisperFactory.FromPath("ggml-base.bin");
        processor = whisperFactory.CreateBuilder()
            .WithLanguage("en")
            .Build();
    }

    public void Transcript(Action<string> currentDialog)
    {
        var capture = new WasapiLoopbackCapture();
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
                        Dispatcher.UIThread.Post(() => currentDialog(result.Text));
                        _history.Add(result.Text);
                    }            
                }catch(Exception ex)
                {
                    Dispatcher.UIThread.Post(() => currentDialog("Theres no audio to transcribe"));
                }
            }
        };
        capture.StartRecording();
    }
}