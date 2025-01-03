using UnityEngine;
using System.Collections;

public class PathUtil
{
    public static string EraseExtension(string fullName)
    {
        if (fullName == null)
            return null;
        
        int length = fullName.LastIndexOf('.');
		if (length > 0 && fullName.Substring(length, fullName.Length-length)!=".lua")
            return fullName.Substring(0, length);
        else
            return fullName;
    }

    public static string GetExtension(string fullName)
    {
        int startIndex = fullName.LastIndexOf('.');
        if ((startIndex > 0) && ((startIndex + 1) < fullName.Length))
            return fullName.Substring(startIndex);

        return string.Empty;
    }
}
