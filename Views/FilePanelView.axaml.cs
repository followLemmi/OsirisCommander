using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OsirisCommander.ViewModels;

namespace OsirisCommander.Views;

public partial class FilePanelView : UserControl
{
    private FilePanelViewModel? _viewModel;
    public DataGrid? FileListDataGrid { get; set; }
    public bool Focused { get; set; }

    public FilePanelView()
    {
        InitializeComponent(); 
        FileListDataGrid = this.FindControl<DataGrid>("FileList");
        if (FileListDataGrid != null)
        {
            Debug.WriteLine("Fucused true");
            FileListDataGrid.AttachedToVisualTree += (sender, args) =>
            {
                if (Focused)
                {
                    FileListDataGrid.Focus();
                }
            };
        }
    }

    public void TestTap(){}
    
    protected override void OnLoaded(RoutedEventArgs e)
    {
        _viewModel =  DataContext as FilePanelViewModel;
        if (_viewModel != null) _viewModel.Clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (_viewModel != null) _viewModel.View = this;
        base.OnLoaded(e);
    }
}