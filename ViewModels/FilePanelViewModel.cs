using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using OsirisCommander.Logic.FileSystem;
using ReactiveUI;

namespace OsirisCommander.ViewModels;

public class FilePanelViewModel : ViewModelBase
{
    private LinuxFileSystemManagerImpl _linuxFileSystemManagerImpl;
    private ObservableCollection<FileModel> _files;
    private FileModel _selectedFile;
    private FileModel _lastSelectedFile;
    private bool _focused;

    public FilePanelViewModel(LinuxFileSystemManagerImpl linuxFileSystemManagerImpl, bool focused)
    {
        _linuxFileSystemManagerImpl = linuxFileSystemManagerImpl;
        _files = _linuxFileSystemManagerImpl.GetAllCurrentFiles();
        _selectedFile = _files[0];
        _lastSelectedFile = _selectedFile;
        _focused = focused;
    }

    public void OnEnterEvent()
    {
        _linuxFileSystemManagerImpl.OpenDirectory(SelectedFile.FullPath);
        _lastSelectedFile = _selectedFile;
        Files = _linuxFileSystemManagerImpl.GetAllCurrentFiles();
        SelectedFile = Files[0];
    }

    public void OnBackspaceEvent()
    {
        _linuxFileSystemManagerImpl.OpenDirectory(_linuxFileSystemManagerImpl.GetParentDirectoryPath());
        Files = _linuxFileSystemManagerImpl.GetAllCurrentFiles();
        SelectedFile = Files[Files.ToList().FindIndex(model => model.FileName.Equals(_lastSelectedFile.FileName))];
        Debug.WriteLine("Current selected item = " + SelectedFile);
    }
    
    public FileModel SelectedFile
    {
        get => _selectedFile;
        set => this.RaiseAndSetIfChanged(ref _selectedFile, value);
    }
    
    public ObservableCollection<FileModel> Files { 
        get => _files;
        set => this.RaiseAndSetIfChanged(ref _files, value); 
    }

    public bool Focused
    {
        get => _focused;
        set => _focused = value;
    }
    
}