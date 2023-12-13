using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Threading;
using DynamicData;
using OsirisCommander.Models;

namespace OsirisCommander.Logic.FileSystem;

public class FileSystemEventProcessor
{

    private FileSystemWatcher _fileSystemWatcher;

    private ObservableCollection<FileModel> _files;
    private string _watchPath;

    public FileSystemEventProcessor(ObservableCollection<FileModel> files, string basePath)
    {
        _files = files;
        _watchPath = basePath;

        _fileSystemWatcher = new FileSystemWatcher();
        _fileSystemWatcher.Path = _watchPath;
        _fileSystemWatcher.EnableRaisingEvents = true;
        _fileSystemWatcher.Changed += OnChange;
        _fileSystemWatcher.Created += OnCreate;
        _fileSystemWatcher.Deleted += OnDelete;
        _fileSystemWatcher.Renamed += OnRename;
    }

    public string WatchPath
    {
        get => _watchPath;
        set => _watchPath = SetWatchPath(value);
    }

    private string SetWatchPath(string watchPath)
    {
        _fileSystemWatcher.EnableRaisingEvents = false;
        _watchPath = watchPath;
        _fileSystemWatcher.Path = _watchPath;
        _fileSystemWatcher.EnableRaisingEvents = true;
        return _watchPath;
    }

    private void OnChange(object obj, FileSystemEventArgs args)
    {
    }

    private void OnDelete(object obj, FileSystemEventArgs args)
    {
        var filesToDelete = _files.Where(model => model.FullPath == args.FullPath).Select(model => model).ToList();
        foreach (var file in filesToDelete)
        {
            Dispatcher.UIThread.Invoke(() => _files.Remove(file));
        }
    }

    private void OnCreate(object obj, FileSystemEventArgs args)
    {
        InsertFileToCollection(Directory.Exists(args.FullPath) ? FileModel.FromDirectoryInfo(new DirectoryInfo(args.FullPath)) : FileModel.FromFileInfo(new FileInfo(args.FullPath)));
    }

    private void OnRename(object obj, RenamedEventArgs args)
    {
        var oldNamedFile = _files.Where(f => f.FileName == args.OldName).Select(f => f).ToList();
        foreach (var file in oldNamedFile)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                _files.Remove(file);
                InsertFileToCollection(Directory.Exists(args.FullPath) ? FileModel.FromDirectoryInfo(new DirectoryInfo(args.FullPath)) : FileModel.FromFileInfo(new FileInfo(args.FullPath)));
            });
        }
    }
    
    private void InsertFileToCollection(FileModel fileModel)
    {
        var index = _files.BinarySearch(fileModel);
        if (index < 0)
        {
            index = ~index;
        }
        Dispatcher.UIThread.Invoke(() => _files.Insert(index, fileModel));
    }

}