using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using OsirisCommander.Models;

namespace OsirisCommander.Logic.FileSystem.Impl;

public class WindowsFileSystemManagerImpl : IFileSystemManager
{

    private string _currentDirectory;
    private string _parentDirectory;
    private Stack<FileModel> _selectionQueue = new Stack<FileModel>();

    public WindowsFileSystemManagerImpl()
    {
        _currentDirectory = GetDrivesInfo()[0].RootDirectory.FullName;
    }
    
    public DriveInfo[] GetDrivesInfo()
    {
        return DriveInfo.GetDrives();
    }

    public string GetParentDirectoryPath()
    {
        return _parentDirectory;
    }

    public string GetCurrentDirectoryPath()
    {
        return _currentDirectory;
    }

    public List<FileModel> GetCurrentFilesList()
    {
        return new DirectoryInfo(_currentDirectory).GetFiles().Select(FileModel.FromFileInfo).ToList();
    }

    public List<FileModel> GetCurrentDirectoriesList()
    {
        var accessibleDirs = new List<FileModel>();
        accessibleDirs.AddRange(new DirectoryInfo(_currentDirectory).GetDirectories().Where(dir => !dir.Attributes.HasFlag(FileAttributes.Hidden)).Select(FileModel.FromDirectoryInfo));
        return accessibleDirs;
    }

    public ObservableCollection<FileModel> GetAllCurrentFiles()
    {
        List<FileModel> files = new List<FileModel>();
        if (!IsCurrentDirectoryRoot())
        {
            files.Add(new FileModel("..", "<DIR>", "avares://OsirisCommander/Assets/up-arrow.png", 0, true,
                _parentDirectory));
        }

        files.AddRange(GetCurrentDirectoriesList());
        files.AddRange(GetCurrentFilesList());
        return new ObservableCollection<FileModel>(files);
    }

    public void OpenDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            var dir = new DirectoryInfo(path);
            if (dir.Parent == null)
            {
                _currentDirectory = path;
            }
            else
            {
                _parentDirectory = dir.Parent?.FullName;
                _currentDirectory = path;
            }
        }
        else
        {
            Process.Start(path);
        }
    }

    public void DeleteFile(FileModel fileModel)
    {
        if (fileModel.IsDirectory)
        {
            Directory.Delete(fileModel.FullPath, true);
        }
        else
        {
            File.Delete(fileModel.FullPath);
        }
    }

    public void PasteFile(List<string>? files)
    {
        throw new System.NotImplementedException();
    }

    public FileModel GetPreviousSelectedFile()
    {
        try
        {
            return _selectionQueue.Pop();
        }
        catch (Exception e)
        {
            Console.WriteLine("Selected files stack is empty");
            return null!;
        }
    }

    public void PushLastSelectedFile(FileModel fileModel)
    {
        _selectionQueue.Push(fileModel);
    }

    private bool IsCurrentDirectoryRoot()
    {
        return GetDrivesInfo().Any(dir => _currentDirectory.Equals(dir.Name));
    }
    
}