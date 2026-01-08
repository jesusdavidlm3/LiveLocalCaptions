using Whisper.net;
using Whisper.net.Ggml;
using NAudio.Wave;

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
int bytesPerSecond = 16000 * 2;
int segmentDurationSeconds = 5;
int segmentSize = bytesPerSecond * segmentDurationSeconds;

//Ajustes de formato
var waveFormat = new WaveFormat(48000, 24, 2 );
var writer = new WaveFileWriter("test.wav", waveFormat);
// var targetFormat = new WaveFormat(16000, 16, 1);
// var bufferedWaveProvicer = new BufferedWaveProvider(capture.WaveFormat);
// var resampler = new MediaFoundationResampler(bufferedWaveProvicer, targetFormat)
// {
//     ResamplerQuality = 60
// };
capture.WaveFormat = waveFormat; 

//Operaciones a realizar durante la captura
capture.DataAvailable += async (s, e) =>
{
    // bufferStream.Write(e.Buffer, 0, e.BytesRecorded);
    // if (bufferStream.Length >= segmentSize)
    // {
        //Procesador de audio capturado
        // var audioBytes =  bufferStream.ToArray();
        // bufferStream.SetLength(0);
        // float[] samples = new float[audioBytes.Length / 2];
        // for (int i = 0; i < samples.Length; i++)
        // {
        //     short sample = BitConverter.ToInt16(audioBytes, i * 2);
        //     samples[i] = sample / 32768f;
        //     writer.Write(e.Buffer, 0, e.BytesRecorded);
        // }

        //Modelo de transcripcion
        // try
        // {
        //     await foreach (var result in processor.ProcessAsync(samples))
        //     {
        //         Console.WriteLine($"{result.Start}->{result.End}: {result.Text}");
        //     }            
        // }catch(Exception ex)
        // {
        //     Console.WriteLine($"No hay audio para Capturar");
        // }
        
        //Guardado de audio en archivo
        // writer.Write(e.Buffer, 0, e.BytesRecorded);
    // }
    writer.Write(e.Buffer, 0, e.BytesRecorded);
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