using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using LiveLocalCaptions.Services;
using LiveLocalCaptions.ViewModels;
using SukiUI.Controls;

namespace LiveLocalCaptions.Views;

public partial class MainWindow : SukiWindow
{
    private MainWindowViewModel viewModel {get; set;}
    public MainWindow()
    {
        InitializeComponent();
        var history = new HistoryService();
        var showLoadingModelDialogService = new ShowLoadingModelDialogService(this);
        viewModel = new MainWindowViewModel(history);
        DataContext = viewModel;
        ScrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
    }

    private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        double pos = ScrollViewer.Offset.Y;
        double max = ScrollViewer.Extent.Height - ScrollViewer.Viewport.Height;
        if (pos >= max)
        {
            Dispatcher.UIThread.Post(() => { ScrollViewer.ScrollToEnd(); }, DispatcherPriority.Background);
        }
    }

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        viewModel.ChangeSettings();
    }
}