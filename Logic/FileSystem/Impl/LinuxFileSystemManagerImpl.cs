using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using OsirisCommander.Models;
using OsirisCommander.ViewModels;

namespace OsirisCommander.Logic.FileSystem;

public class LinuxFileSystemManagerImpl : IFileSystemManager
{
    private const string RootDirectoryPath = "/";
    private string _currentDirectory;
    private string? _parentDirectory;

    public LinuxFileSystemManagerImpl()
    {
        _parentDirectory = "/";
        _currentDirectory = RootDirectoryPath;
    }

    public DriveInfo[] GetDrivesInfo()
    {
        throw new System.NotImplementedException();
    }

    public string GetParentDirectoryPath()
    {
        return _parentDirectory;
    }

    public string GetCurrentDirectoryPath()
    {
        throw new System.NotImplementedException();
    }

    public List<FileModel> GetCurrentFilesList()
    {
        return new DirectoryInfo(_currentDirectory).GetFiles().Select(FileModel.FromFileInfo).ToList();
    }

    public List<FileModel> GetCurrentDirectoriesList()
    {
        return new DirectoryInfo(_currentDirectory).GetDirectories().Select(FileModel.FromDirectoryInfo).ToList();
    }

    public ObservableCollection<FileModel> GetAllCurrentFiles()
    {
        List<FileModel> files = new List<FileModel>();
        if (!_currentDirectory.Equals("/"))
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
            Process.Start("xdg-open", path);
        }
    }

    public FileModel GetPreviousSelectedFile()
    {
        throw new System.NotImplementedException();
    }

    public void PushLastSelectedFile(FileModel fileModel)
    {
        throw new System.NotImplementedException();
    }

    public void PasteFile(List<string>? files)
    {
        if (files != null && files.Capacity != 0)
        {
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                if (Directory.Exists(file) || File.Exists(file))
                {
                    File.Copy(file, _currentDirectory + $"/{fileInfo.Name}");
                }
            }
        }
    }
}