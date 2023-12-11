using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using OsirisCommander.Logic.FileSystem.Sorting;
using OsirisCommander.ViewModels;

namespace OsirisCommander.Views;

public partial class FilePanelView : UserControl
{
    public DataGrid? FileListDataGrid { get; set; }
    public bool Focused { get; set; }
    
    private FilePanelViewModel? _viewModel;

    private bool _isEditing = false;

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
    
    protected override void OnLoaded(RoutedEventArgs e)
    {
        _viewModel =  DataContext as FilePanelViewModel;
        if (_viewModel != null) _viewModel.Clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (_viewModel != null) _viewModel.View = this;
        base.OnLoaded(e);
    }

    private void FileList_OnSorting(object? sender, DataGridColumnEventArgs e)
    {
        var sortMemberPath = e.Column.SortMemberPath;
        var eValue = (FilePanelColumn) Enum.Parse(typeof(FilePanelColumn), sortMemberPath);
        _viewModel.Sort(eValue);
        e.Handled = true;
    }

    private void FileList_OnKeyDown(object? sender, KeyEventArgs e)
    {
        _viewModel.HotKeyProcess(e.Key);
    }

    private void FileList_OnTapped(object? sender, TappedEventArgs e)
    {
        // if (_isDoubleClick)
        // {
        //     var dataGrid = (DataGrid)sender;
        //     if (dataGrid.CurrentColumn is DataGridTemplateColumn && dataGrid.SelectedItem != null)
        //     {
        //         dataGrid.BeginEdit();
        //     }
        // }
        // else
        // {
        //     _isDoubleClick = true;
        //     _doubleClickTimer.Start();
        // }
    }
    
    private void FileList_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (!_isEditing)
        {
            _viewModel.OnDoubleTap();
        }
    }
}