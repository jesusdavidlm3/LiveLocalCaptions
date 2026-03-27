using Avalonia.Controls;
using LiveLocalCaptions.Views;

namespace LiveLocalCaptions.Services;

public class ShowLoadingModelDialogService
{
    private static Window _owner;
    
    public ShowLoadingModelDialogService(Window owner)
    {
        _owner = owner;
    }

    public static void ShowDialog(TranscriptionProvider transcriptionProvider)
    {
        var window = new LoadingModelView(transcriptionProvider);
        window.ShowDialog(_owner);
    }
}