using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Avalonia.Controls;
using LiveLocalCaptions.Classes;
using LiveLocalCaptions.Interfaces;
using LiveLocalCaptions.Services;
using LiveLocalCaptions.Views;
using Whisper.net.Ggml;

namespace LiveLocalCaptions.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public IHistoryService HistoryService { get; }
    private readonly TranscriptionProvider _transcriptionProvider;
    private Dictionary<string, GgmlType> ModelsDictionary { get; set; } = new Dictionary<string, GgmlType>();
    public ObservableCollection<string> ModelsNames { get; set; } = new ObservableCollection<string>();
    private Dictionary<string, string> LanguagesDictionary { get; set; } = new Dictionary<string, string>();
    public ObservableCollection<string> LanguagesNames { get; set; } = new ObservableCollection<string>();

    public bool Working
    {
        get => _working;
        set
        {
            if (_working != value)
            {
                _working = value;
                OnPropertyChanged();
            }
        }
    }
    private bool _working { get; set; } = false;
    private string _SelectedModel { get; set; }
    public string SelectedModel
    {
        get => _SelectedModel;
        set
        {
            if (value != _SelectedModel)
            {
                _SelectedModel = value;
                ChangeSettings();
            }
        }
    }
    private string _SelectedLanguage { get; set; }

    public string SelectedLanguage
    {
        get => _SelectedLanguage;
        set
        {
            if (value != _SelectedLanguage)
            {
                _SelectedLanguage = value;
                ChangeLanguage();
            }
        }
    }

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
        
        ModelsDictionary.Add("1 (Very low precision)", GgmlType.Tiny);
        ModelsDictionary.Add("2 (Low Precision)", GgmlType.Base);
        ModelsDictionary.Add("3 (Low-medium precision)", GgmlType.Small);
        ModelsDictionary.Add("4 (Medium-high precision)", GgmlType.Medium);
        ModelsDictionary.Add("5 (High precision)", GgmlType.LargeV3);
        ModelsDictionary.Add("6 (Very-high precision)", GgmlType.LargeV3Turbo);
        foreach (var item in ModelsDictionary)
        {
            ModelsNames.Add(item.Key);
        }
        
        LanguagesDictionary.Add("Auto", "auto");
        LanguagesDictionary.Add("English", "en");
        LanguagesDictionary.Add("Spanish", "es");
        foreach (var item in LanguagesDictionary)
        {
            LanguagesNames.Add(item.Key);
        }

        Working = true;
        SelectedModel = ModelsNames[1];
        SelectedLanguage = LanguagesNames[1];
    }

    private void Start()
    {
        if (StatusButton == "Start")
        {
            StatusButton = "Stop";
            Working = false;
            _transcriptionProvider.Transcript();
        }
        else
        {
            StatusButton = "Start";
            Working = true;
            _transcriptionProvider.StopTranscription();
        }
    }

    private void ClearHistory()
    {
        HistoryService.Clear();
    }

    private void ChangeSettings()
    {
        _transcriptionProvider.ChangeModel(ModelsDictionary[_SelectedModel]);
        var isModelLoaded = _transcriptionProvider.VerifyModel();
        if (!isModelLoaded)
        {
            ShowLoadingModelDialogService.ShowDialog(_transcriptionProvider);
        }
    }

    private void ChangeLanguage()
    {
        _transcriptionProvider.ChangeLanguage(LanguagesDictionary[_SelectedLanguage]);
    }
}