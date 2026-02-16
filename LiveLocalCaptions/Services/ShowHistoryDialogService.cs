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

    public void ShowDialog(IHistoryService historyService)
    {
        var historyWindow = new HistoryView(historyService);
        historyWindow.ShowDialog(_owner);
    }
}