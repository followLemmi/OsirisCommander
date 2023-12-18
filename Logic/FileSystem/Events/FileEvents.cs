using System;

namespace OsirisCommander.Logic.FileSystem.Events;

public class FileEvents
{
    
    public delegate void DeleteFileDelegate(string filePath, Panel panel);

    public delegate void DeleteConfirmationDelegate(object sender, Panel panel);

    public static event DeleteFileDelegate DeleteFileEvent;
    public static event DeleteConfirmationDelegate DeleteFileConfirmEvent;
    public static void DeleteFileDialogEvent(string filePath, Panel panel)
    {
        DeleteFileEvent.Invoke(filePath, panel);
    }

    public static void DeleteFileConfirmation(object sender, Panel callerPanel)
    {
        DeleteFileConfirmEvent?.Invoke(sender, callerPanel);
    }

}