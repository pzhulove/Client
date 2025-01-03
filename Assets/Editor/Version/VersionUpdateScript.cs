using UnityEngine;
using UnityEditor;
using XUPorterJSON;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System;

[InitializeOnLoad]
public class VersionUpdateScript {

    static VersionUpdateScript()
    {
        DeleteEmptyFloder();
        EditorApplication.playmodeStateChanged += UpdateVersion;
    }

    private static string[] kPaths =
    {
        "C:\\Program Files\\Git\\cmd\\git.exe",
        "C:\\Program Files\\Git\\mingw64\\bin\\git.exe",
        "D:\\Program Files\\Git\\cmd\\git.exe",
        "D:\\Program Files\\Git\\mingw64\\bin\\git.exe",
    };

    private static string[] kFindPaths =
    {
        "C:\\Program Files\\Git\\usr\\bin\\find.exe",
        "D:\\Program Files\\Git\\usr\\bin\\find.exe",
    };

    private static string _getConfigPath()
    {
        var path = Utility.FormatString(Path.Combine(Application.persistentDataPath, "version.config"));
        if (CFileManager.IsFileExist(path))
        {
            return path;
        }

        path = Utility.FormatString(Path.Combine(Application.streamingAssetsPath, "version.config"));

        if (File.Exists(path))
        {
            return path;
        }

        return "";
    }

    private static string _getExisitPath(string[] pathList)
    {
        for (int i = 0; i < pathList.Length; ++i)
        {
            if (File.Exists(pathList[i]))
            {
                return pathList[i];
            }
        }

        return "";
    }

    private static string _runGitCmd(string format)
    {
        var gitPath = _getExisitPath(kPaths);

        if (!string.IsNullOrEmpty(gitPath))
        {
            var args = string.Format("log -n 1 --pretty={0}", format);
            var str = _runCmd(gitPath, args, Application.streamingAssetsPath.Replace("Program/Client/Assets/StreamingAssets", ""));

            UnityEngine.Debug.LogFormat("str => {0}", str);
            return str;
        }

        return "";
    }

    private static string _runCmd(string exe, string args, string workpath)
    {
        ProcessStartInfo processInfo = new ProcessStartInfo();
        processInfo.FileName = exe;
        processInfo.Arguments = args;
        processInfo.CreateNoWindow = true;
        processInfo.WindowStyle = ProcessWindowStyle.Hidden;
        processInfo.WorkingDirectory = workpath;
        processInfo.UseShellExecute = false;
        processInfo.RedirectStandardOutput = true;

        Process process = new Process();
        process.StartInfo = processInfo;
        process.Start();

        return process.StandardOutput.ReadToEnd();
    }

    private static void DeleteEmptyFloder()
    {
        //var path = _getExisitPath(kFindPaths);
        //if (!string.IsNullOrEmpty(path))
        //{
        //    var str = _runCmd(path, "Program\\Client\\Assets -depth -type d -empty -delete -print", Application.streamingAssetsPath.Replace("Program/Client/Assets/StreamingAssets", ""));
        //    if (!string.IsNullOrEmpty(str))
        //    {
        //        UnityEngine.Debug.LogFormat("delete empty floder => {0}", str);
        //    }
        //}
    }

    public static void UpdateVersion()
    {
        // delete the empty floder
        DeleteEmptyFloder();

        //if (EditorApplication.isPlayingOrWillChangePlaymode &&
        //    !EditorApplication.isPlaying)
        //{
        //    var configPath = _getConfigPath();
        //    if (!string.IsNullOrEmpty(configPath))
        //    {
        //        var ctime = _runGitCmd("%ct");
        //        var cauthor = _runGitCmd("%cn");
        //        var chash = _runGitCmd("%H");
        //        var cmsg = _runGitCmd("%s");
        //        var data = CFileManager.ReadFile(configPath);
        //        string json = System.Text.UTF8Encoding.UTF8.GetString(data);
        //        var setting = MiniJSON.jsonDecode(json) as Hashtable;
        //        try
        //        {
        //            setting["commitTime"] = ctime;
        //            setting["commitMessage"] = cmsg;
        //            setting["commitAuthor"] = cauthor;
        //            setting["commitID"] = chash;
        //            var str = setting.toJson();
        //            File.WriteAllText(configPath, str);
        //        }
        //        catch (Exception e)
        //        {
        //        }
        //    }
        //}
    }
}
