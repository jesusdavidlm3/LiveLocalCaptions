using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LiveLocalCaptions.Interfaces;
using LiveLocalCaptions.Services;
using LiveLocalCaptions.ViewModels;

namespace LiveLocalCaptions.Views;

public partial class HistoryView : Window
{
    public HistoryView()
    {
        InitializeComponent();
        IHistoryService historyService = new HistoryService(); 
        DataContext = new HistoryViewModel(historyService);
    }
}