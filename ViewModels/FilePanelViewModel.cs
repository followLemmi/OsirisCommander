using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using OsirisCommander.Logic.FileSystem;
using OsirisCommander.Logic.FileSystem.Sorting;
using OsirisCommander.Models;
using OsirisCommander.Utils;
using OsirisCommander.Views;
using ReactiveUI;

namespace OsirisCommander.ViewModels;

public class FilePanelViewModel : ViewModelBase
{
    public IClipboard? Clipboard { get; set; }
    public FilePanelView View { get; set; } = null!;

    public ICommand HotKeyCommand { get; }
    public ICommand CopyCommand { get; }

    private readonly IFileSystemManager _fileSystemManager;
    private readonly FileListSortingManager _fileListSortingManager;
    private ObservableCollection<FileModel> _files;
    private FileModel _selectedFile;
    private FileModel _lastSelectedFile;
    private bool _focused;

    public FilePanelViewModel(IFileSystemManager fileSystemManager)
    {
        HotKeyCommand = ReactiveCommand.Create<Key>(HotKeyProcessor);
        CopyCommand = ReactiveCommand.Create<DataGrid>(CopyEvent);

        _fileSystemManager = fileSystemManager;
        _fileListSortingManager = new FileListSortingManager();
        _files = _fileSystemManager.GetAllCurrentFiles();
        _selectedFile = _files[0];
        _lastSelectedFile = _selectedFile;
    }

    public FileModel SelectedFile
    {
        get => _selectedFile;
        set => this.RaiseAndSetIfChanged(ref _selectedFile, value);
    }

    public ObservableCollection<FileModel> Files
    {
        get => _files;
        set => this.RaiseAndSetIfChanged(ref _files, value);
    }

    public bool Focused
    {
        get => _focused;
        set => _focused = value;
    }

    private void HotKeyProcessor(Key key)
    {
        switch (key)
        {
            case Key.Enter:
                OnEnterEvent();
                break;
            case Key.Back:
                OnBackspaceEvent();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(key), key, null);
        }
    }

    public void Sort(FilePanelColumn filePanelColumn)
    {
        switch (filePanelColumn)
        {
            case FilePanelColumn.Name:
                Files = _fileListSortingManager.SortByName(Files);
                break;
            case FilePanelColumn.Extension:
                Debug.WriteLine("Extention sort");
                break;
        }
    }

    private void OnEnterEvent()
    {
        _fileSystemManager.OpenDirectory(SelectedFile.FullPath);
        _fileSystemManager.PushLastSelectedFile(_selectedFile);
        Files = _fileSystemManager.GetAllCurrentFiles();
        SelectedFile = Files[0];
    }


    private void OnBackspaceEvent()
    {
        Console.WriteLine("Backspace");
        var previousSelectedFile = _fileSystemManager.GetPreviousSelectedFile();
        if (previousSelectedFile == null)
        {
            return;
        }
        var parentDir = _fileSystemManager.GetParentDirectoryPath();
        _fileSystemManager.OpenDirectory(parentDir);
        Files = _fileSystemManager.GetAllCurrentFiles();
        SelectedFile = Files[Files.ToList().FindIndex(model => model.FileName.Equals(previousSelectedFile.FileName))];
    }

    private async void CopyEvent(DataGrid dataGrid)
    {
        var selectedItems = dataGrid.SelectedItems;
        DataObject dataObject = new DataObject();
        var stringBuilder = new StringBuilder();
        foreach (var file in selectedItems)
        {
            var fileModel = file as FileModel;
            stringBuilder.Append(fileModel!.FullPath).Append("\n\r");
        }

        dataObject.Set(DataFormats.Text, stringBuilder.ToString());
        await Clipboard?.SetDataObjectAsync(dataObject)!;
    }

    public async void PasteEvent()
    {
        var dataObject = await Clipboard?.GetDataAsync(DataFormats.Text)!;
        if (dataObject != null)
        {
            var files = FileUtils.FromClipboardDataToFilePaths(dataObject as string);
            _fileSystemManager.PasteFile(files);
        }
    }
}