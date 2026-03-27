using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;

namespace LiveLocalCaptions.ViewModels;

public class LoadingModelViewModel : ViewModelBase
{
    private Window _window;
    private TranscriptionProvider _transcriptionProvider;
    private int _LoadValue { get; set; } = 0;

    public int LoadValue
    {
        get => _LoadValue;
        set
        {
            if (_LoadValue != value)
            {
                _LoadValue = value;
                OnPropertyChanged();
            }
        }
    }
    
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