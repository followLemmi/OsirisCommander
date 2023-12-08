using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Threading;
using DynamicData;
using OsirisCommander.Logic.FileSystem;
using OsirisCommander.Logic.FileSystem.Sorting;
using OsirisCommander.Models;
using OsirisCommander.Utils;
using OsirisCommander.Views;
using ReactiveUI;

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
    private FileSystemWatcher _fileSystemWatcher;

    public delegate void CopyDelegate(Panel panel, FileModel fileModel);
    public static event CopyDelegate CopyEvent;

    public FilePanelViewModel(Panel panelOrder, IFileSystemManager fileSystemManager)
    {
        _panelOrder = panelOrder;
        HotKeyCommand = ReactiveCommand.Create<Key>(HotKeyProcessor);
        FileSystemManager = fileSystemManager;
        _fileListSortingManager = new FileListSortingManager();
        _files = FileSystemManager.GetAllCurrentFiles();
        _selectedFile = _files[0];
        PanelController.CopyProgress += Progress;
        
        _fileSystemWatcher = new FileSystemWatcher();
        _fileSystemWatcher.Path = FileSystemManager.GetCurrentDirectoryPath();
        _fileSystemWatcher.EnableRaisingEvents = true;
        _fileSystemWatcher.Changed += OnChange;
        _fileSystemWatcher.Deleted += OnDelete;
        _fileSystemWatcher.Created += OnCreate;
        _fileSystemWatcher.Renamed += OnRename;
    }

    private void OnChange(object obj, FileSystemEventArgs args)
    {
        
    }

    private void OnDelete(object obj, FileSystemEventArgs args)
    {
        
    }

    private void OnCreate(object obj, FileSystemEventArgs args)
    {
        if (Directory.Exists(args.FullPath))
        {
            var directoryInfo = FileModel.FromDirectoryInfo(new DirectoryInfo(args.FullPath));
            int index = Files.BinarySearch(directoryInfo);
            if (index < 0)
            {
                index = ~index;
            }
            Dispatcher.UIThread.Invoke(() => Files.Insert(index, directoryInfo));
        }
        else
        {
            var fileInfo = FileModel.FromFileInfo(new FileInfo(args.FullPath));
            int index = Files.BinarySearch(fileInfo);
            if (index < 0)
            {
                index = ~index;
            }
            Dispatcher.UIThread.Invoke(() => Files.Insert(index, fileInfo));
        }
    }

    private void OnRename(object obj, RenamedEventArgs args)
    {
        
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
            case Key.A:
                Copy();
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
        FileSystemManager.OpenDirectory(SelectedFile.FullPath);
        FileSystemManager.PushLastSelectedFile(_selectedFile);
        Files = FileSystemManager.GetAllCurrentFiles();
        SelectedFile = Files[0];
        _fileSystemWatcher.Path = FileSystemManager.GetCurrentDirectoryPath();
    }


    private void OnBackspaceEvent()
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
        _fileSystemWatcher.Path = FileSystemManager.GetCurrentDirectoryPath();
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

    private void UpdateFileView()
    {
        while (true)
        {
            UpdateFileList(FileSystemManager.GetAllCurrentFiles().ToList());
            Thread.Sleep(200);
        }
    }
    
    private void UpdateFileList(List<FileModel> updatedFiles)
    {
        var addedFiles = updatedFiles.Except(Files);
        var removedFiles = Files.Except(updatedFiles);
    
        foreach (var addedFile in addedFiles)
        {
            int index = Files.BinarySearch(addedFile);
            if (index < 0)
            {
                index = ~index;
            }
            Dispatcher.UIThread.Invoke(() => Files.Insert(index, addedFile));
        }
        foreach (var removedFile in removedFiles)
        {
            Dispatcher.UIThread.Invoke(() => Files.Remove(removedFile));
        }
    }
}