using System;
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

    private bool _Loading { get; set; } = true; 
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
            var currentVar = 0;
            var taskDelay = 30;
            while (_Loading)
            {
                while (currentVar < 60)
                {
                    currentVar++;
                    await Dispatcher.UIThread.InvokeAsync(() => Progress.Value += 1);
                    await Task.Delay(taskDelay);
                }

                taskDelay = 50;
                while (currentVar > 40)
                {
                    currentVar--;
                    await Dispatcher.UIThread.InvokeAsync(() => Progress.Value -= 1);
                    await Task.Delay(taskDelay);
                }
            }
        });
    }

    public async Task FinishSecuence()
    {
        _Loading = false;
        var currentVar = Convert.ToInt32(Progress.Value);
        await Task.Run(async () =>
        {
            while (currentVar < 100)
            {
                currentVar++;
                await Dispatcher.UIThread.InvokeAsync(() => Progress.Value += 1);
                await Task.Delay(15);
            }
        });
    }
}