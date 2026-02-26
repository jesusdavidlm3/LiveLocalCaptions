using System.Collections.ObjectModel;
using System.Threading;
using Avalonia.Controls;
using LiveLocalCaptions.Classes;
// using EchoSharp.NAudio;
using LiveLocalCaptions.Interfaces;
using LiveLocalCaptions.Services;
using LiveLocalCaptions.Views;

namespace LiveLocalCaptions.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public IHistoryService HistoryService { get; }
    private readonly TranscriptionProvider _transcriptionProvider;

    public string StatusButton
    {
        get => _StatusButton;
        set
        {
            if (_StatusButton != value)
            {
                _StatusButton = value;
                OnPropertyChanged();
            }
        }
    }
    private string _StatusButton { get; set; } = "Start";
    
    public RelayCommand StartCommand { get; }
    public RelayCommand ClearHistoryCommand { get; }

    public MainWindowViewModel(IHistoryService historyService)
    {
        HistoryService = historyService;
        _transcriptionProvider = new TranscriptionProvider(HistoryService);
        
        StartCommand = new RelayCommand(
            execute: _ => Start(),
            canExecute: _ => true
        );
        
        ClearHistoryCommand = new RelayCommand(
            execute: _ => ClearHistory(),
            canExecute: _ => true
        );
    }

    private void Start()
    {
        if (StatusButton == "Start")
        {
            StatusButton = "Stop";
            _transcriptionProvider.Transcript();
        }
        else
        {
            StatusButton = "Start";
            _transcriptionProvider.StopTranscription();
        }
    }

    private void ClearHistory()
    {
        HistoryService.Clear();
    }
}