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
    private readonly IHistoryService _historyService;
    private readonly ShowHistoryDialogService _showHistoryDialogService;
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

    public MainWindowViewModel(IHistoryService historyService, ShowHistoryDialogService showHistoryDialogService)
    {
        _historyService = historyService;
        _showHistoryDialogService = showHistoryDialogService;
        
        StartCommand = new RelayCommand(
            execute: _ => Start(),
            canExecute: _ => true
        );

        HistoryWindowCommand = new RelayCommand(
            execute: _ => OpenHistoryWindow(),
            canExecute: _ => true
        );
    }
    
    public void Start()
    {
        if (StatusButton == "Start")
        {
            StatusButton = "Stop";
            var transcriptionProvider = new TranscriptionProvider(_historyService);
            transcriptionProvider.Transcript(newText => CurrentText = newText);
        }
        else
        {
            StatusButton = "Start";
        }
    }

    public void OpenHistoryWindow()
    {
        _showHistoryDialogService.ShowDialog();
    }
}