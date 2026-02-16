using System.Collections.ObjectModel;
using Avalonia.Controls;
using LiveLocalCaptions.Interfaces;
using LiveLocalCaptions.Views;

namespace LiveLocalCaptions.Services;

public class ShowHistoryDialogService
{
    Window _owner;
    public ShowHistoryDialogService(Window owner)
    {
        _owner = owner;
    }

    public void ShowDialog()
    {
        var historyWindow = new HistoryView();
        historyWindow.ShowDialog(_owner);
    }
}