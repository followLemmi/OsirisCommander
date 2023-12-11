using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace OsirisCommander.Models;

public class FileModel : IComparable, INotifyPropertyChanged
{
    private string _fileName;
    private string _fileExtension;
    private Bitmap _fileIcon;
    private long _size;
    private bool _isDirectory;
    private string _fullPath;

    public FileModel(string fileName, string fileExtension, string fileIcon, long size, bool isDirectory,
        string fullPath)
    {
        _fileName = fileName;
        _fileExtension = fileExtension;
        _fileIcon = new Bitmap(AssetLoader.Open(new Uri(fileIcon)));
        _size = size;
        _isDirectory = isDirectory;
        _fullPath = fullPath;
    }

    public string FileName
    {
        get => _fileName;
        set
        {
            var tmpPath = _fullPath;
            SetField(ref _fileName, value);
            var split = FullPath.Split("\\");
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < split.Length - 1; i++)
            {
                stringBuilder.Append(split[i]).Append("\\");
            }

            stringBuilder.Append(value);
            FullPath = stringBuilder.ToString();

            if (IsDirectory)
            {
                Directory.Move(tmpPath, FullPath);
            }
            else
            {
                File.Move(tmpPath, FullPath);
            }
        }
    }

    public string FileExtension
    {
        get => _fileExtension;
        set => SetField(ref _fileExtension, value);
    }

    public Bitmap FileIcon
    {
        get => _fileIcon;
        set { SetField(ref _fileIcon, value); }
    }

    public long Size
    {
        get => _size;
        set => SetField(ref _size, value);
    }

    public bool IsDirectory
    {
        get => _isDirectory;
        set => SetField(ref _isDirectory, value);
    }

    public string FullPath
    {
        get => _fullPath;
        set => SetField(ref _fullPath, value);
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
        if (IsDirectory && !model.IsDirectory)
        {
            return -1;
        }

        if (!IsDirectory && model.IsDirectory)
        {
            return 1;
        }

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

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}