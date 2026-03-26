using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LiveLocalCaptions.ViewModels;
using SukiUI.Controls;

namespace LiveLocalCaptions.Views;

public partial class LoadingModelView : SukiWindow
{
    public LoadingModelView(TranscriptionProvider transcriptionProvider)
    {
        InitializeComponent();
        var viewModel = new LoadingModelViewModel(transcriptionProvider, this);
        DataContext = viewModel;
        // LoadingSecuence();
    }
    
    public void LoadingSecuence()
    {
            do
            {
                Thread.Sleep(2000);
                Dispatcher.UIThread.Invoke(() => { Progress.Value++; }, DispatcherPriority.Render);
            }while(Progress.Value < 100);
            do
            {
                Thread.Sleep(2000);
                Dispatcher.UIThread.Invoke(() => { Progress.Value--; }, DispatcherPriority.Render);
            }while(Progress.Value > 0);
    }
}