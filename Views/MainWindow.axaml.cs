using System;
using Avalonia.Controls;
using OsirisCommander.ViewModels;

namespace OsirisCommander.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        Opened += OnOpened;
        InitializeComponent();
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        
    }
}