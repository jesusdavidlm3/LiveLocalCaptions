using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using LiveLocalCaptions.Views;

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
    
    public LoadingModelViewModel(TranscriptionProvider transcriptionProvider,  LoadingModelView window)
    {
        _window = window;
        _transcriptionProvider = transcriptionProvider;
        _ = Download(window);
    }

    private async Task Download(LoadingModelView window)
    {
        await _transcriptionProvider.DownloadModel();
        await window.FinishSecuence();
        Task.Delay(500).Wait();
        _window.Close();
    }
}