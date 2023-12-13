using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OsirisCommander.Logic.FileSystem.Events;
using OsirisCommander.ViewModels;

namespace OsirisCommander.Views.Dialogs;

public partial class DeleteDialog : Window
{

    private DeleteDialogViewModel? _viewModel;
    
    public DeleteDialog()
    {
        Opened += OnOpened;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        InitializeComponent();
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        this._viewModel = DataContext as DeleteDialogViewModel;
    }

    private void OnOpened()
    {
        
    }

    private void NoButtonClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void YesButtonClick(object? sender, RoutedEventArgs e)
    {
        FileEvents.DeleteFileConfirmation(this, _viewModel.CallerPanel);
    }
}