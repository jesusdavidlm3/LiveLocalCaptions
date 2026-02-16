using Avalonia.Controls;
using Avalonia.Interactivity;
using LiveLocalCaptions.Services;
using LiveLocalCaptions.ViewModels;

namespace LiveLocalCaptions.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var history = new HistoryService();
        var showHistoryDialogService = new ShowHistoryDialogService(this);
        DataContext = new MainWindowViewModel(history, showHistoryDialogService);
    }
}