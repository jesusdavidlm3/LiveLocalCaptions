using System.Collections.ObjectModel;

namespace LiveLocalCaptions.Interfaces;

public interface IHistoryService
{
    ObservableCollection<string> Transcriptions { get; }
    void Add(string transcription);
}