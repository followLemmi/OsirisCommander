using OsirisCommander.Logic.FileSystem;

namespace OsirisCommander.ViewModels;

public class DeleteDialogViewModel : ViewModelBase
{
    
    public string FilePath { get; }
    public Panel CallerPanel { get; }

    public DeleteDialogViewModel(string filePath, Panel callerPanel)
    {
        FilePath = filePath;
        CallerPanel = callerPanel;
    }
    
}