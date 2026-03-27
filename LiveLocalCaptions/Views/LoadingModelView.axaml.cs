using System.Threading;
using System.Threading.Tasks;
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
        Progress.Value = 0;
        LoadingSecuence();
    }
    
    public void LoadingSecuence()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                var currentVar = 0;
                while (currentVar < 100)
                {
                    currentVar++;
                    await Dispatcher.UIThread.InvokeAsync(() => Progress.Value += 1);
                    await Task.Delay(20);
                }
                
                while (currentVar > 0)
                {
                    currentVar--;
                    await Dispatcher.UIThread.InvokeAsync(() => Progress.Value -= 1);
                    await Task.Delay(20);
                }
            }
        });
    }
}