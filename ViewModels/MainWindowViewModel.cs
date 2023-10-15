using OsirisCommander.Logic.FileSystem;

namespace OsirisCommander.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public FilePanelViewModel LeftFilePanelViewModel { get; }
    public FilePanelViewModel RightFilePanelViewModel { get; }

    public MainWindowViewModel()
    {
        LeftFilePanelViewModel = new FilePanelViewModel(new LinuxFileSystemManagerImpl(), true);
        RightFilePanelViewModel = new FilePanelViewModel(new LinuxFileSystemManagerImpl(), false);
    }
}
