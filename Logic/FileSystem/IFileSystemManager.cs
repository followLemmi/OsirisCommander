using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using OsirisCommander.Models;

namespace OsirisCommander.Logic.FileSystem;

public interface IFileSystemManager
{

    public DriveInfo[] GetDrivesInfo();

    public string GetParentDirectoryPath();

    public string GetCurrentDirectoryPath();

    public List<FileModel> GetCurrentFilesList();
    
    public List<FileModel> GetCurrentDirectoriesList();

    public ObservableCollection<FileModel> GetAllCurrentFiles();

    public void OpenDirectory(string path);

    public void DeleteFile(FileModel fileModel);

    public void PasteFile(List<string>? files);

    public FileModel GetPreviousSelectedFile();

    public void PushLastSelectedFile(FileModel fileModel);

}