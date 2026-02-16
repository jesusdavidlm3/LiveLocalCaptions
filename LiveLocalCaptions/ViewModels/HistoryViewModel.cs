using System.Collections.ObjectModel;
using LiveLocalCaptions.Interfaces;

namespace LiveLocalCaptions.ViewModels;

public class HistoryViewModel : ViewModelBase
{
    
    IHistoryService _historyService;
    ObservableCollection<string> transcriptions;
    
    public HistoryViewModel(IHistoryService historyService)
    {
        _historyService = historyService;
        transcriptions = historyService.Transcriptions;
    }
}