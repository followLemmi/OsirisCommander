using System.Collections.Generic;
using System.Linq;

namespace OsirisCommander.Utils;

public class FileUtils
{

    public static List<string> FromClipboardDataToFilePaths(string? clipboardData)
    {
        var splittedData = clipboardData.Split("\n\r");
        var list = splittedData.ToList();
        var lastElement = list[list.Capacity - 1];
        list.Remove(lastElement);
        return list;
    }
    
}