using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OsirisCommander.Logic.FileSystem;
using OsirisCommander.Logic.FileSystem.Impl;
using OsirisCommander.Logic.FileSystem.Sorting;
using OsirisCommander.ViewModels;
using ReactiveUI;

namespace OsirisCommander.Views;

public partial class FilePanelView : UserControl
{
    public DataGrid? FileListDataGrid { get; set; }
    public bool Focused { get; set; }
    
    private FilePanelViewModel? _viewModel;

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
        // if (_viewModel != null) GotFocus += (sender, args) =>
        // {
        //     _viewModel.UpdateFileView();
        // };
        base.OnLoaded(e);
    }

    private void FileList_OnSorting(object? sender, DataGridColumnEventArgs e)
    {
        var sortMemberPath = e.Column.SortMemberPath;
        var eValue = (FilePanelColumn) Enum.Parse(typeof(FilePanelColumn), sortMemberPath);
        _viewModel.Sort(eValue);
        e.Handled = true;
    }
}