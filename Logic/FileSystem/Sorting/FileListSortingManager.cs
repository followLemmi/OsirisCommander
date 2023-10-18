using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using OsirisCommander.Models;
using OsirisCommander.ViewModels;
using ReactiveUI;

namespace OsirisCommander.Logic.FileSystem;

public class FileListSortingManager
{
    private bool _isSortedAscendingByName = true;
    
    public FileListSortingManager()
    {
        
    }
    
    public ObservableCollection<FileModel> SortByName(IEnumerable<FileModel> fileModels)
    {
        var fileList = fileModels.ToList();
        fileList.Sort((model, fileModel) => String.Compare(model.FileName, fileModel.FileName, StringComparison.Ordinal));
        if (_isSortedAscendingByName)
        {
            fileList = fileList.OrderBy(item => item.FileName).ToList();
        }
        else
        {
            fileList = fileList.OrderByDescending(item => item.FileName).ToList();
        }

        _isSortedAscendingByName = !_isSortedAscendingByName;
        return new ObservableCollection<FileModel>(fileList);
    }
    
}