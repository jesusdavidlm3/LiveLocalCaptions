using Whisper.net;
using Whisper.net.Ggml;
using NAudio.Wave;

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

using var capture = new WasapiLoopbackCapture();
var bufferStream = new MemoryStream();
int bytesPerSecond = 16000 * 2;
int segmentDurationSeconds = 5;
int segmentSize = bytesPerSecond * segmentDurationSeconds;

capture.DataAvailable += async (s, e) =>
{
    bufferStream.Write(e.Buffer, 0, e.BytesRecorded);
    if (bufferStream.Length >= segmentSize)
    {
        var audioBytes =  bufferStream.ToArray();
        bufferStream.SetLength(0);
        float[] samples = new float[audioBytes.Length / 2];
        for (int i = 0; i < samples.Length; i++)
        {
            short sample = BitConverter.ToInt16(audioBytes, i * 2);
            samples[i] = sample / 32768f;
        }

        try
        {
            await foreach (var result in processor.ProcessAsync(samples))
            {
                Console.WriteLine($"{result.Start}->{result.End}: {result.Text}");
            }            
        }catch(Exception ex)
        {
            Console.WriteLine($"No hay audio para Capturar");
        }

    }
};
capture.StartRecording();

var waveFormat = new WaveFormat(48000, 16, 2 );
var writer = new WaveFileWriter("test.wav", waveFormat);

var targetFormat = new WaveFormat(16000, 16, 1);
var bufferedWaveProvicer = new BufferedWaveProvider(capture.WaveFormat);
var resampler = new MediaFoundationResampler(bufferedWaveProvicer, targetFormat)
{
    ResamplerQuality = 60
};


Console.WriteLine("Presiona enter para cerrar");
Console.ReadLine();