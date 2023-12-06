using System.Runtime.InteropServices;
using OsirisCommander.Logic.FileSystem;
using OsirisCommander.Logic.FileSystem.Impl;

namespace OsirisCommander.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public FilePanelViewModel LeftFilePanelViewModel { get; }
    public FilePanelViewModel RightFilePanelViewModel { get; }

    public MainWindowViewModel()
    {
        OSPlatform currentOs;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            currentOs = OSPlatform.Windows;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            currentOs = OSPlatform.Linux;
        }
        else
        {
            currentOs = OSPlatform.OSX;
        }
        LeftFilePanelViewModel = new FilePanelViewModel(Panel.Left, currentOs == OSPlatform.Windows ? new WindowsFileSystemManagerImpl() : new LinuxFileSystemManagerImpl());
        RightFilePanelViewModel = new FilePanelViewModel(Panel.Right, currentOs == OSPlatform.Windows ? new WindowsFileSystemManagerImpl() : new LinuxFileSystemManagerImpl());

        PanelController panelController = new PanelController(LeftFilePanelViewModel, RightFilePanelViewModel);
    }
}
