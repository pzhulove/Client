using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class FileArchiveAccessor
{
    static private string DataPath = string.Empty;
    static private string ReadOnlyPath = string.Empty;
    static private string ReadWritePath = string.Empty;

    static public void SetFileArchivePath(string dataPath, string readOnlyPath,string readWritePath)
    {
        DataPath = dataPath;
        ReadOnlyPath = readOnlyPath;
        ReadWritePath = readWritePath;
    }

    static public bool SaveFileInLocalFileArchive(string fileResRelative,byte[] data)
    {
#if UNITY_EDITOR
        return _SaveFileWithFileArchive(_GetLocalStreamPath(), fileResRelative, data);
#elif UNITY_IOS
        Logger.LogWarningFormat("Can not save file {0} with iOS application local file path {1}!",fileResRelative,_GetLocalUrlProtocol());
        return false;
#elif UNITY_ANDROID
        Logger.LogWarningFormat("Can not save file {0} with Android application local file path {1}!",fileResRelative,_GetLocalUrlProtocol());
        return false;
#else
        return _SaveFileWithFileArchive(_GetLocalStreamPath(), fileResRelative, data);
#endif
    }

    static public bool SaveFileInLocalFileArchive(string fileResRelative, string data)
    {
#if UNITY_EDITOR
        return _SaveFileWithFileArchive(_GetLocalStreamPath(), fileResRelative, data);
#elif UNITY_IOS
        Logger.LogWarningFormat("Can not save file {0} with iOS application local file path {1}!",fileResRelative,_GetLocalUrlProtocol());
        return false;
#elif UNITY_ANDROID
        Logger.LogWarningFormat("Can not save file {0} with Android application local file path {1}!",fileResRelative,_GetLocalUrlProtocol());
        return false;
#else
        return _SaveFileWithFileArchive(_GetLocalStreamPath(), fileResRelative, data);
#endif
    }

    static public bool LoadFileInLocalFileArchive(string fileResRelative,out byte[] data)
    {
        return _LoadFileWithFileArchive(_GetLocalUrlProtocol(), fileResRelative, out data);
    }

    static public bool LoadFileInLocalFileArchive(string fileResRelative,out string data)
    {
        return _LoadFileWithFileArchive(_GetLocalUrlProtocol(), fileResRelative,out data);
    }

    static public bool SaveFileInPersistentFileArchive(string fileResRelative,byte[] data)
    {
        return _SaveFileWithFileArchive(_GetPersistentStreamPath(), fileResRelative, data);
    }

    static public bool SaveFileInPersistentFileArchive(string fileResRelative, string data)
    {
        return _SaveFileWithFileArchive(_GetPersistentStreamPath(), fileResRelative, data);
    }

    static public bool LoadFileInPersistentFileArchive(string fileResRelative, out byte[] data)
    {
        return _LoadFileWithFileArchive(_GetPersistentUrlProtocol(), fileResRelative, out data);
    }

    static public bool LoadFileInPersistentFileArchive(string fileResRelative, out string data)
    {
        return _LoadFileWithFileArchive(_GetPersistentUrlProtocol(), fileResRelative, out data);
    }

    static protected bool _SaveFileWithFileArchive(string archivePath,string fileResRelative,byte[] data)
    {
        string filePath = Path.Combine(archivePath, fileResRelative);

        int tryCnt = 0;
        while (true)
        {
            try
            {
                File.WriteAllBytes(filePath, data);
                return true;
            }
            catch (Exception exception)
            {
                tryCnt++;
                if (tryCnt >= 3)
                {
                    Logger.LogError("Write File " + filePath + " Error! Exception = " + exception.ToString());
                    if(File.Exists(filePath))
                        File.Delete(filePath);

                    return false;
                }
            }
        }
    }

    static protected bool _SaveFileWithFileArchive(string archivePath, string fileResRelative, string data)
    {
        return _SaveFileWithFileArchive(archivePath, fileResRelative, System.Text.UTF8Encoding.Default.GetBytes(data));
    }

    static protected bool _LoadFileWithFileArchive(string archivePath,string fileResRelative, out byte[] data)
    {
        string filePath = Path.Combine(archivePath, fileResRelative);

        bool res = false;
        data = null;
        //CFileManager.ReadFile(path);

      //  Debug.LogWarningFormat("Loading WWW with path [{0}]!", filePath);
        WWW www = new WWW(filePath);
        //Debug.LogWarningFormat("Loading WWW with path [{0}],This maybe cause a blocking!", filePath);
        while (!www.isDone) { }
        if (string.IsNullOrEmpty(www.error))
        {
     //       Debug.LogWarning("Loading byte data from WWW!");
            List<byte> byteList = new List<byte>();
            //List<byte> byteList = GamePool.ListPool<byte>.Get();
            byteList.AddRange(www.bytes);
            data = byteList.ToArray();
            //GamePool.ListPool<byte>.Release(byteList);
            res = true;
        }
        //else
        //    Debug.LogWarning(www.error);

        www.Dispose();
        return res;
    }

    static protected bool _LoadFileWithFileArchive(string archivePath, string fileResRelative, out string data)
    {
        string filePath = Path.Combine(archivePath, fileResRelative);

        bool res = false;
        data = null;
        WWW www = new WWW(filePath);
        //Debug.LogWarningFormat("Loading WWW with path [{0}],This maybe cause a blocking!", filePath);
        while (!www.isDone) { }
        if (string.IsNullOrEmpty(www.error))
        {
            data = www.text;
            res = true;
        }
        //else
        //    Debug.LogWarning(www.error);

        www.Dispose();
        return res;
    }

    static protected string _GetLocalUrlProtocol()
    {
#if UNITY_EDITOR
        return "file:///" + ReadOnlyPath;
#elif UNITY_IOS
        return "file://" + ReadOnlyPath;
#elif UNITY_ANDROID
        return "jar:file://" + DataPath + "!/assets/";
#else
        return "file:///" + ReadOnlyPath;
#endif
    }

    static protected string _GetPersistentUrlProtocol()
    {
#if UNITY_EDITOR
        return "file:///" + ReadWritePath;
#elif UNITY_IOS
        return "file://" + ReadWritePath;
#elif UNITY_ANDROID
        return "file://" + ReadWritePath;
#else
        return "file:///" + ReadWritePath;
#endif
    }

    static protected string _GetLocalStreamPath()
    {
        return ReadOnlyPath;
    }

    static protected string _GetPersistentStreamPath()
    {
        return ReadWritePath;
    }
}
