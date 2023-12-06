using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
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
    private readonly Panel _panelOrder;
    public IClipboard? Clipboard { get; set; }
    public FilePanelView View { get; set; } = null!;

    public ICommand HotKeyCommand { get; }

    public readonly IFileSystemManager FileSystemManager;
    private readonly FileListSortingManager _fileListSortingManager;
    private ObservableCollection<FileModel> _files;
    private FileModel _selectedFile;
    private bool _focused;

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
        var thread = new Thread(UpdateFileView)
        {
            Name = "FILE-VIEW-UPDATER"
        };
        thread.Start();
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

    public void UpdateFileView()
    {
        var tmpSelected = _selectedFile;
        Files = FileSystemManager.GetAllCurrentFiles();
        SelectedFile = tmpSelected;
        Thread.Sleep(1000);
    }
}