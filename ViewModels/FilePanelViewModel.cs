using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using OsirisCommander.Logic.FileSystem;
using OsirisCommander.Utils;
using OsirisCommander.Views;
using ReactiveUI;

namespace OsirisCommander.ViewModels;

public class FilePanelViewModel : ViewModelBase
{
    public IClipboard? Clipboard { get; set; }
    public FilePanelView View { get; set; }

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
    }

    public async void CopyEvent()
    {
        DataObject dataObject = new DataObject();
        var stringBuilder = new StringBuilder();
        foreach (var file in View.FileListDataGrid?.SelectedItems!)
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
            _linuxFileSystemManagerImpl.PasteFile(files);
        }
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
    
}