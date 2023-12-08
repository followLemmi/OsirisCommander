using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace OsirisCommander.Models;

public class FileModel : IComparable
{
    public string FileName { get; set; }
    public string FileExtension { get; set; }
    public Bitmap FileIcon { get; set; }
    public long Size { get; set; }
    public bool IsDirectory { get; set; }
    public string FullPath { get; set; }

    public FileModel(string fileName, string fileExtension, string fileIcon, long size, bool isDirectory,
        string fullPath)
    {
        FileName = fileName;
        FileExtension = fileExtension;
        FileIcon = new Bitmap(AssetLoader.Open(new Uri(fileIcon)));
        Size = size;
        IsDirectory = isDirectory;
        FullPath = fullPath;
    }

    public override bool Equals(object? obj)
    {
        var m = (FileModel)obj!;
        return FileName.Equals(m.FileName) && FullPath.Equals(m.FullPath);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FileName, FileExtension, FullPath);
    }
    
    public override string ToString()
    {
        return
            $"{nameof(FileName)}: {FileName}, {nameof(FileExtension)}: {FileExtension}, {nameof(FileIcon)}: {FileIcon}, {nameof(Size)}: {Size}, {nameof(IsDirectory)}: {IsDirectory}, {nameof(FullPath)}: {FullPath}";
    }

    public int CompareTo(object? obj)
    {
        var model = (FileModel)obj;
        return String.Compare(FileName, model.FileName, StringComparison.OrdinalIgnoreCase);
    }

    public static FileModel FromFileInfo(FileInfo fileInfo)
    {
        return new FileModel(fileInfo.Name, fileInfo.Extension, "avares://OsirisCommander/Assets/common_file_icon.png",
            fileInfo.Length, false, fileInfo.FullName);
    }

    public static FileModel FromDirectoryInfo(DirectoryInfo directoryInfo)
    {
        return new FileModel(directoryInfo.Name, "<DIR>", "avares://OsirisCommander/Assets/folder_icon.png", 0, true,
            directoryInfo.FullName);
    }
}