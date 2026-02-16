using System.Collections.ObjectModel;
using LiveLocalCaptions.Interfaces;

namespace LiveLocalCaptions.Services;

public class HistoryService : IHistoryService
{
    public ObservableCollection<string> Transcriptions { get; }
    public void Add(string transcription)
    {
        // if (!string.IsNullOrWhiteSpace(transcription))
        // {
            Transcriptions.Add(transcription);
        // }
    }

    public HistoryService()
    {
        Transcriptions = new ObservableCollection<string>();
    }
}