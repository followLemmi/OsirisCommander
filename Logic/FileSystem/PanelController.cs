using System.IO;
using OsirisCommander.Models;
using OsirisCommander.ViewModels;

namespace OsirisCommander.Logic.FileSystem;

public class PanelController
{

    private readonly FilePanelViewModel _leftPanel;
    private readonly FilePanelViewModel _rightPanel;
    
    public delegate void CopyProgressDelegate(double percentage);

    public static event CopyProgressDelegate CopyProgress;

    public PanelController(FilePanelViewModel leftPanel, FilePanelViewModel rightPanel)
    {
        _leftPanel = leftPanel;
        _rightPanel = rightPanel;
        FilePanelViewModel.CopyEvent += CopyFilesProcessor;
    }

    private async void CopyFilesProcessor(Panel panel, FileModel sourceFileModel)
    {
        var sourcePath = sourceFileModel.FullPath;
        var targetDirectory = panel switch
        {
            Panel.Left => _rightPanel.FileSystemManager.GetCurrentDirectoryPath() + $"/{sourceFileModel.FileName}",
            Panel.Right => _leftPanel.FileSystemManager.GetCurrentDirectoryPath() + $"/{sourceFileModel.FileName}",
            _ => ""
        };
        var files = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
        Directory.CreateDirectory(targetDirectory);

        var totalBytes = 0L;
        foreach (var file in files)
        {
            var fileInfo = new FileInfo(file);
            totalBytes += fileInfo.Length;
        }

        var totalBytesCopied = 0L;
        foreach (var file in files)
        {
            var relativePath = file.Substring(sourcePath.Length + 1);
            var targetFilePath = Path.Combine(targetDirectory, relativePath);

            var targetFileDirectory = Path.GetDirectoryName(targetFilePath);
            if (targetFileDirectory != null)
            {
                Directory.CreateDirectory(targetFileDirectory);
            }

            await using var sourceStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            await using var targetStream = new FileStream(targetFilePath, FileMode.Create, FileAccess.Write);
            var buffer = new byte[1024 * 1024];
            int bytesRead = 0;
            while ((bytesRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await targetStream.WriteAsync(buffer, 0, bytesRead);
                totalBytesCopied += bytesRead;

                var progressPercentage = (double)totalBytesCopied / totalBytes * 100;
                CopyProgress.Invoke(progressPercentage);
            }
        }
    }
}