using System.Collections.ObjectModel;
using LiveLocalCaptions.Interfaces;

namespace LiveLocalCaptions.ViewModels;

public class HistoryViewModel : ViewModelBase
{
    public ObservableCollection<string> Transcriptions { get; }

    public HistoryViewModel(IHistoryService historyService)
    {
        Transcriptions = historyService.Transcriptions;
    }
}