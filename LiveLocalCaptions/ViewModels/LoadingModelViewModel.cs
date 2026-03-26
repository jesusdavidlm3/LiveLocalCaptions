using System.Threading.Tasks;
using Avalonia.Controls;

namespace LiveLocalCaptions.ViewModels;

public class LoadingModelViewModel : ViewModelBase
{
    private Window _window;
    private TranscriptionProvider _transcriptionProvider;
    
    public LoadingModelViewModel(TranscriptionProvider transcriptionProvider,  Window window)
    {
        _window = window;
        _transcriptionProvider = transcriptionProvider;
        _ = Download();
    }

    private async Task Download()
    {
        await _transcriptionProvider.DownloadModel();
        _window.Close();
    }
}