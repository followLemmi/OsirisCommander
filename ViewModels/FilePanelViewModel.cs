using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using OsirisCommander.Logic.FileSystem;
using OsirisCommander.Logic.FileSystem.Events;
using OsirisCommander.Logic.FileSystem.Sorting;
using OsirisCommander.Models;
using OsirisCommander.Utils;
using OsirisCommander.Views;
using OsirisCommander.Views.Dialogs;
using ReactiveUI;
using Panel = OsirisCommander.Logic.FileSystem.Panel;

namespace OsirisCommander.ViewModels;

public class FilePanelViewModel : ViewModelBase
{
    private readonly Panel _panelOrder;
    public IClipboard? Clipboard { get; set; }
    public FilePanelView View { get; set; } = null!;

    public ICommand HotKeyCommand { get; }

    public readonly IFileSystemManager FileSystemManager;
    private readonly FileListSortingManager _fileListSortingManager;
    private ObservableCollection<FileModel> _files;
    private readonly object _selectedFileLock = new object();
    private FileModel _selectedFile;
    private bool _focused;
    private FileSystemEventProcessor _fileSystemEventProcessor;
    private bool _isEditNow = false;
    private FileInfo _editableFile;

    public delegate void CopyDelegate(Panel panel, FileModel fileModel);
    public static event CopyDelegate CopyEvent;

    public FilePanelViewModel(Panel panelOrder, IFileSystemManager fileSystemManager)
    {
        _panelOrder = panelOrder;
        HotKeyCommand = ReactiveCommand.Create<Key>(HotKeyProcess);
        FileSystemManager = fileSystemManager;
        _fileListSortingManager = new FileListSortingManager();
        _files = FileSystemManager.GetAllCurrentFiles();
        _selectedFile = _files[0];
        PanelController.CopyProgress += Progress;
        _fileSystemEventProcessor = new FileSystemEventProcessor(Files, FileSystemManager.GetCurrentDirectoryPath());

        FileEvents.DeleteFileConfirmEvent += DeleteFileConfirmHandler;
    }

    private void Progress(double percentage)
    {
        Console.WriteLine($"Percentage = {percentage}");
    }

    private void Copy()
    {
        CopyEvent.Invoke(_panelOrder, SelectedFile);
    }

    public FileModel SelectedFile
    {
        get
        {
            lock (_selectedFileLock)
            {
                return _selectedFile;
            }
        }
        set
        {
            lock (_selectedFileLock)
            {
                _selectedFile = value;
            }
        }
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
    
    public void HotKeyProcess(Key key)
    {
        switch (key)
        {
            case Key.Enter:
                OnEnterEvent();
                break;
            case Key.Back:
                OnBackspaceEvent();
                break;
            case Key.F2:
                BeginEdit();
                break;
            case Key.F5:
                Copy();
                break;
            case Key.Delete:
            case Key.F8:
                FileEvents.DeleteFileDialogEvent(SelectedFile.FullPath, _panelOrder);
                break;
            default:
                Console.WriteLine($"Key pressed {key}");
                break;
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
        if (_isEditNow)
        {
            var dataGrid = View.FileListDataGrid;
            dataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
            if (SelectedFile.IsDirectory)
            {
                Directory.Move(_editableFile.FullName, SelectedFile.FullPath);
            }
            else
            {
                File.Move(_editableFile.FullName, SelectedFile.FullPath);
            }
            _editableFile = null;
            _isEditNow = false;
            dataGrid.IsReadOnly = true;
            return;
        }
        FileSystemManager.OpenDirectory(SelectedFile.FullPath);
        FileSystemManager.PushLastSelectedFile(_selectedFile);
        Files = FileSystemManager.GetAllCurrentFiles();
        SelectedFile = Files[0];
        _fileSystemEventProcessor.WatchPath = FileSystemManager.GetCurrentDirectoryPath();
    }

    private void BeginEdit()
    {
        _editableFile = new FileInfo(SelectedFile.FullPath);
        var dataGrid = View.FileListDataGrid;
        if (dataGrid.CurrentColumn is DataGridTemplateColumn && dataGrid.SelectedItem != null)
        {
            dataGrid.IsReadOnly = false;
            dataGrid.BeginEdit();
            _isEditNow = true;
        }
        Console.WriteLine($"Begin edit {dataGrid.SelectedItem}");
    }

    public void OnDoubleTap()
    {
        if (!_isEditNow)
        {
            OnEnterEvent();
        }
    }


    private void OnBackspaceEvent()
    {
        if (!_isEditNow)
        {
            Console.WriteLine("Backspace");
            var previousSelectedFile = FileSystemManager.GetPreviousSelectedFile();
            if (previousSelectedFile == null)
            {
                return;
            }
            var parentDir = FileSystemManager.GetParentDirectoryPath();
            FileSystemManager.OpenDirectory(parentDir);
            Files = FileSystemManager.GetAllCurrentFiles();
            SelectedFile = Files[Files.ToList().FindIndex(model => model.FileName.Equals(previousSelectedFile.FileName))];
            _fileSystemEventProcessor.WatchPath = FileSystemManager.GetCurrentDirectoryPath();
        }
    }

    private void DeleteFileConfirmHandler(object sender, Panel panel)
    {
        if (panel != _panelOrder) return;
        FileSystemManager.DeleteFile(SelectedFile);
        var dialog = sender as DeleteDialog;
        dialog.Close();
    }

    public async void PasteEvent()
    {
        var dataObject = await Clipboard?.GetDataAsync(DataFormats.Text)!;
        if (dataObject != null)
        {
            var files = FileUtils.FromClipboardDataToFilePaths(dataObject as string);
            FileSystemManager.PasteFile(files);
        }
    }
}