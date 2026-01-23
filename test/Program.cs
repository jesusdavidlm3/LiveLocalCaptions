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
int bytesPerSecond = 48000 * 2 * 2;     //samples por segundo, profundidad en bytes, canales
int segmentDurationSeconds = 5;
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
        
        var resampler = new MediaFoundationResampler(bufferedWaveProvider, targetFormat);
        resampler.ResamplerQuality = 60;
        
        // var audioBytes =  bufferedWaveProvicer;
        // bufferStream.SetLength(0);
        // float[] samples = new float[bufferedWaveProvicer.BufferedBytes];
        // for (int i = 0; i < samples.Length; i++)
        // {
        //     short sample = BitConverter.ToInt16(audioBytes, i * 2);
        //     samples[i] = sample / 32768f;
        // }

        //Modelo de transcripcion
        try
        {
            // await foreach (var result in processor.ProcessAsync(samples)) 
            // {
            //     Console.WriteLine($"{result.Start}->{result.End}: {result.Text}");
            // }            
        }catch(Exception ex)
        {
            Console.WriteLine($"No hay audio para Capturar");
        }
    }
    //Guardado de audio en archivo
    // writer.Write(e.Buffer, 0, e.BytesRecorded);   no se donde va esto...
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