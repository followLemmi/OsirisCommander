using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Avalonia.Threading;
using DynamicData;
using OsirisCommander.Models;

namespace OsirisCommander.Logic.FileSystem;

public class FileSystemEventProcessor
{

    private FileSystemWatcher _fileSystemWatcher;

    public ObservableCollection<FileModel> Files { get; set; }
    private string _watchPath;

    public FileSystemEventProcessor(ObservableCollection<FileModel> files, string basePath)
    {
        Files = files;
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
        Console.WriteLine($"WatchPath for FileSystemWatcher is set to {watchPath}");
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
        var filesToDelete = Files.Where(model => model.FullPath == args.FullPath).Select(model => model).ToList();
        foreach (var file in filesToDelete)
        {
            Dispatcher.UIThread.Invoke(() => Files.Remove(file));
        }
    }

    private void OnCreate(object obj, FileSystemEventArgs args)
    {
        InsertFileToCollection(Directory.Exists(args.FullPath) ? FileModel.FromDirectoryInfo(new DirectoryInfo(args.FullPath)) : FileModel.FromFileInfo(new FileInfo(args.FullPath)));
    }

    private void OnRename(object obj, RenamedEventArgs args)
    {
        var oldNamedFile = Files.Where(f => f.FileName == args.OldName).Select(f => f).ToList();
        foreach (var file in oldNamedFile)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                Files.Remove(file);
                InsertFileToCollection(Directory.Exists(args.FullPath) ? FileModel.FromDirectoryInfo(new DirectoryInfo(args.FullPath)) : FileModel.FromFileInfo(new FileInfo(args.FullPath)));
            });
        }
    }
    
    private void InsertFileToCollection(FileModel fileModel)
    {
        var index = Files.BinarySearch(fileModel);
        if (index < 0)
        {
            index = ~index;
        }
        Dispatcher.UIThread.Invoke(() => Files.Insert(index, fileModel));
    }

}