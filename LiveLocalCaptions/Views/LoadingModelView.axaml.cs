using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LiveLocalCaptions.ViewModels;
using SukiUI.Controls;

namespace LiveLocalCaptions.Views;

public partial class LoadingModelView : SukiWindow
{
    public LoadingModelView(TranscriptionProvider transcriptionProvider)
    {
        InitializeComponent();
        DataContext = new LoadingModelViewModel(transcriptionProvider, this);
    }
}