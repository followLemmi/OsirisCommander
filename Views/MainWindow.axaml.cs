using System;
using Avalonia.Controls;
using OsirisCommander.Logic.FileSystem;
using OsirisCommander.ViewModels;
using OsirisCommander.Views.Dialogs;
using Panel = OsirisCommander.Logic.FileSystem.Panel;

namespace OsirisCommander.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        Opened += OnOpened;
        InitializeComponent();
        Logic.FileSystem.Events.FileEvents.DeleteFileEvent += DeleteFileEventProcessor;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        if (DataContext is MainWindowViewModel mainWindowViewModel)
        {
            LeftFilePanel.DataContext = mainWindowViewModel.LeftFilePanelViewModel;
            RightFilePanel.DataContext = mainWindowViewModel.RightFilePanelViewModel;
        }
    }

    private void DeleteFileEventProcessor(string filePath, Panel panel)
    {
        var dialog = new DeleteDialog();
        dialog.DataContext = new DeleteDialogViewModel(filePath, panel);
        dialog.ShowDialog(this);
    }
}