using LiveLocalCaptions.Classes;
using EchoSharp.NAudio;

namespace LiveLocalCaptions.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting
    {
        get => _Greeting;
        set
        {
            if (_Greeting != value)
            {
                _Greeting = value;                
                OnPropertyChanged();
            }
        }
    } 
    private string _Greeting { get; set; } = "Live Captions will appear here";

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

    public MainWindowViewModel()
    {
        StartCommand = new RelayCommand(
            execute: _ => Start(),
            canExecute: _ => true
        );
    }
    
    public void Start()
    {
        if (StatusButton == "Start")
        {
            StatusButton = "Stop";
            // var audioSource = new Outpu
        }
        else
        {
            StatusButton = "Start";
        }
    }
}