using System.Collections.ObjectModel;
using LiveLocalCaptions.Interfaces;

namespace LiveLocalCaptions.Services;

public class HistoryService : IHistoryService
{
    public ObservableCollection<string> Transcriptions { get; }
    public void Add(string transcription)
    {
        if (Transcriptions.Count == 0 || Transcriptions[Transcriptions.Count - 1] != transcription)
        {
            Transcriptions.Add(transcription);
        }
    }

    public void Clear()
    {
        Transcriptions.Clear();
    }

    public HistoryService()
    {
        Transcriptions = new ObservableCollection<string>();
    }
}