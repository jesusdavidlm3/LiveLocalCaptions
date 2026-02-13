using Whisper.net;
using Whisper.net.Ggml;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

//Configuracion del modelo de Reconocimiento
var modelName = "ggml-base.bin";
if (!File.Exists(modelName))
{
    using var modelStream = await WhisperGgmlDownloader.Default.GetGgmlModelAsync(GgmlType.BaseEn);
    using var fileWriter = File.OpenWrite(modelName);
    await modelStream.CopyToAsync(fileWriter);
}
using var whisperFactory = WhisperFactory.FromPath("ggml-base.bin");
using var processor = whisperFactory.CreateBuilder()
    .WithLanguage("en")
    .Build();

//Configuracion de la captura de audio
using var capture = new WasapiLoopbackCapture();
var bufferStream = new MemoryStream();
int bytesPerSecond = 48000 * 2 * 2;     //samples por segundo, profundidad en bytes, canales
int segmentDurationSeconds = 2;
int segmentSize = bytesPerSecond * segmentDurationSeconds;

//Ajustes de formato
var sourceFormat = new WaveFormat(48000, 16, 2 );
var targetFormat = new WaveFormat(16000, 16, 1);
var writer = new WaveFileWriter("test.wav", targetFormat);
capture.WaveFormat = sourceFormat;
var bufferedWaveProvider = new BufferedWaveProvider(sourceFormat);
bufferedWaveProvider.BufferLength = segmentSize * 2;

//Operaciones a realizar durante la captura
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
        // var processedAudio = msOut.ToArray();

        //Modelo de transcripcion
        try
        {
            await foreach (var result in processor.ProcessAsync(samples.ToArray())) 
            {
                Console.WriteLine($"{result.Start}->{result.End}: {result.Text}");
            }            
        }catch(Exception ex)
        {
            Console.WriteLine($"No hay audio para Capturar");
        }
    }
};

//Operaciones al terminar la captura
capture.RecordingStopped += (s, e) =>
{
    writer.Dispose();
    writer = null;
};

//Arranque de la aplicacion
capture.StartRecording();
Console.WriteLine("Presiona enter para cerrar");
Console.ReadLine();
capture.StopRecording();