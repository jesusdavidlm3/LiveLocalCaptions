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
    private readonly ShowHistoryDialogService _showHistoryDialogService;
    private readonly TranscriptionProvider _transcriptionProvider;
    public string CurrentText
    {
        get => _CurrentText;
        set
        {
            if (_CurrentText != value)
            {
                _CurrentText = value;                
                OnPropertyChanged();
            }
        }
    } 
    private string _CurrentText { get; set; } = "Live Captions will appear here";

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
    public RelayCommand HistoryWindowCommand { get; }
    public RelayCommand ClearHistoryCommand { get; }

    public MainWindowViewModel(IHistoryService historyService, ShowHistoryDialogService showHistoryDialogService)
    {
        HistoryService = historyService;
        _showHistoryDialogService = showHistoryDialogService;
        _transcriptionProvider = new TranscriptionProvider(HistoryService);
        
        StartCommand = new RelayCommand(
            execute: _ => Start(),
            canExecute: _ => true
        );

        HistoryWindowCommand = new RelayCommand(
            execute: _ => OpenHistoryWindow(),
            canExecute: _ => true
        );
        ClearHistoryCommand = new RelayCommand(
            execute: _ => ClearHistory(),
            canExecute: _ => true
        );
    }
    
    public void Start()
    {
        if (StatusButton == "Start")
        {
            StatusButton = "Stop";
            _transcriptionProvider.Transcript(newText => CurrentText = newText);
        }
        else
        {
            StatusButton = "Start";
            _transcriptionProvider.StopTranscription();
        }
    }

    public void OpenHistoryWindow()
    {
        _showHistoryDialogService.ShowDialog(HistoryService);
    }

    public void ClearHistory()
    {
        HistoryService.Clear();
    }
}