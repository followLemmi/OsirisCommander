using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using OsirisCommander.ViewModels;

namespace OsirisCommander.Views;

public partial class FilePanelView : UserControl
{
    public bool Focused { get; set; }

    public FilePanelView()
    {
        InitializeComponent();
        var dataGrid = this.FindControl<DataGrid>("FileList");
        if (dataGrid != null)
        {
            Debug.WriteLine("Fucused true");
            dataGrid.AttachedToVisualTree += (sender, args) =>
            {
                if (Focused)
                {
                    dataGrid.Focus();    
                }
            };
        }
    }

    private void FileList_OnTapped(object? sender, TappedEventArgs e)
    {
        throw new System.NotImplementedException();
    }
}