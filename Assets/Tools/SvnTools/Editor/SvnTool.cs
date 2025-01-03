using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Diagnostics;

public class SvnTool
{
    private static string svnProcPath = "";
    private static string svnWorkPath = "";

    private static List<string> drives = new List<string>() { "c:", "d:", "e:", "f:" };
    private const string svnPath = "/Program Files/TortoiseSVN/bin";
    private const string svnProc = "/TortoiseProc.exe";

    protected static string _GetCurrentDir()
    {
        var obj = Selection.activeObject;
        if (null == obj)
        {
            return string.Empty;
        }
        var path = AssetDatabase.GetAssetPath(obj);
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }

        if (Directory.Exists(path))
        {
            return path;
        }
        else if (File.Exists(path))
        {
            return path.Replace(Path.GetFileName(path), "");
        }

        return string.Empty;
    }
        
    public static void SvnCommit(string fileNames, string logMessage)
    {
        if (GetWorkPath() == "" || GetExePath() == "")
        {
            return;
        }

        ExecuteProcess(svnProcPath + svnProc, string.Format("/command:commit /path:{0} /logmsg:{1} /closeonend:0", fileNames, logMessage));
    }

    [MenuItem("SVN/更新")]
    protected static void Menu_SvnUpdate()
    {
        //closeonend
        //closeonend:0 不自动关闭对话框
        //closeonend:1 没有错误，则自动关闭对话框
        //closeonend:2 没有错误、冲突，则自动关闭对话框
        //closeonend:3 没有错误、冲突、合并，自动关闭对话框

        Menu_SvnUpdate(svnWorkPath);
    }


    [MenuItem("Assets/SVN/更新")]
    protected static void Asset_SvnUpdate()
    {
        Menu_SvnUpdate(_GetCurrentDir());
    }
    
    protected static void Menu_SvnUpdate(string path)
    {
        if (GetExePath() == "" || string.IsNullOrEmpty(path))
        {
            return;
        }

        //closeonend
        //closeonend:0 不自动关闭对话框
        //closeonend:1 没有错误，则自动关闭对话框
        //closeonend:2 没有错误、冲突，则自动关闭对话框
        //closeonend:3 没有错误、冲突、合并，自动关闭对话框

        ExecuteProcess(svnProcPath + svnProc, string.Format("/command:update /path:{0} /closeonend:2", path));
    }

    

    [MenuItem("SVN/提交")]
    public static void Menu_SvnCommit()
    {
        Menu_SvnCommit(svnWorkPath);
    }


    [MenuItem("Assets/SVN/提交")]

    public static void Asset_SvnCommit()
    {
        Menu_SvnCommit(_GetCurrentDir());
    }
    
    public static void Menu_SvnCommit(string path)
    {
        if (GetExePath() == "" || string.IsNullOrEmpty(path))
        {
            return;
        }

        ExecuteProcess(svnProcPath + svnProc, string.Format("/command:commit /path:{0} /closeonend:0", path));
    }
    

    [MenuItem("SVN/显示日志")]
    public static void Menu_SvnShowLog()
    {
        Menu_SvnShowLog(svnWorkPath);
    }
    
    [MenuItem("Assets/SVN/显示日志")]
    public static void Asset_SvnShowLog()
    {
        Menu_SvnShowLog(_GetCurrentDir());
    }
    
    public static void Menu_SvnShowLog(string path)
    {
        if (GetWorkPath() == "" || path == "")
        {
            return;
        }

        ExecuteProcess(svnProcPath + svnProc, string.Format("/command:log /path:{0} /closeonend:0", path));
    }


    [MenuItem("SVN/检查修改")]
    public static void Menu_SvnShowChanges()
    {
        Menu_SvnShowChanges(svnWorkPath);
    }
    
    [MenuItem("Assets/SVN/检查修改")]
    public static void Asset_SvnShowChanges()
    {
        Menu_SvnShowChanges(_GetCurrentDir());
    }
    
    public static void Menu_SvnShowChanges(string path)
    {
        if (GetWorkPath() == "" || path == "")
        {
            return;
        }

        ExecuteProcess(svnProcPath + svnProc, string.Format("/command:repostatus /path:{0} /closeonend:0", path));
    }

    [MenuItem("SVN/清理")]
    public static void Menu_SvnCleanUp()
    {
        Menu_SvnCleanUp(svnWorkPath);
    }

    [MenuItem("Assets/SVN/清理")]
    public static void Asset_SvnCleanUp()
    {
        Menu_SvnCleanUp(_GetCurrentDir());
    }

    public static void Menu_SvnCleanUp(string path)
    {
        if (GetWorkPath() == "" || path == "")
        {
            return;
        }

        ExecuteProcess(svnProcPath + svnProc, string.Format("/command:cleanup /path:{0} /closeonend:0", path));
    }

    [MenuItem("SVN/设置/设置安装目录（TortoiseProc.exe所在目录）")]
    public static void Menu_SetExePath()
    {
        SetExePath();
    }

    [MenuItem("SVN/设置/设置工作目录（.svn所在目录）")]
    public static void Menu_SetWorkPath()
    {
        SetWorkPath();
    }

    [MenuItem("SVN/打开工作目录")]
    public static void Menu_OpenWorkPath()
    {
        GetWorkPath(false);
        string path = svnWorkPath.Replace("/","\\");
        System.Diagnostics.Process.Start("explorer.exe", path);
    }

    /// <summary>
    /// 设置工作目录
    /// </summary>
    /// <param name="atFirstGet">设置时先Get一次</param>
    public static void SetWorkPath(bool atFirstGet = true)
    {
        if (atFirstGet)
        {
            GetWorkPath();
        }

        svnWorkPath = EditorUtility.OpenFolderPanel("Select Svn WorkPath", svnWorkPath, "");
        if (svnWorkPath != null && svnWorkPath != "")
        {
            EditorUserSettings.SetConfigValue("SvnWorkPath", svnWorkPath);
        }
    }

    /// <summary>
    /// 设置安装目录
    /// </summary>
    /// <param name="atFirstGet">设置时先Get一次</param>
    public static void SetExePath(bool atFirstGet = true)
    {
        if (atFirstGet)
        {
            GetExePath();
        }
        
        svnProcPath = EditorUtility.OpenFolderPanel("Select TortoiseProc.exe", svnProcPath, "");
        if (svnProcPath != null && svnProcPath != "")
        {
            EditorUserSettings.SetConfigValue("SvnProcPath", svnProcPath);
        }
    }

    /// <summary>
    /// 获取安装目录
    /// </summary>
    /// <returns></returns>
    protected static string GetExePath()
    {
        svnProcPath = EditorUserSettings.GetConfigValue("SvnProcPath");
        if (svnProcPath != null && svnProcPath != "")
        {
            return svnProcPath;
        }

        foreach (string item in drives)
        {
            string path = string.Concat(item, svnPath, svnProc);
            if (File.Exists(path))
            {
                svnProcPath = string.Concat(item, svnPath);
                break;
            }
        }

        if (svnProcPath == null || svnProcPath == "")
        {
            SetExePath(false);
        }
        else
        {
            EditorUserSettings.SetConfigValue("SvnProcPath", svnProcPath);
        }

        return svnProcPath;
    }

    /// <summary>
    /// 获取工作目录
    /// </summary>
    /// <param name="ifNullSet">如果工作目录为空，进行设置</param>
    /// <returns></returns>

    protected static string GetWorkPath(bool ifNullSet = true)
    {
        svnWorkPath = EditorUserSettings.GetConfigValue("SvnWorkPath");
        if (svnWorkPath != null && svnWorkPath != "")
        {
            return svnWorkPath;
        }

        string path = Application.dataPath;
        while (!Directory.Exists(path + "/.svn"))
        {
            if (path.Length < 5)
            {
                break;
            }

            path = Path.GetDirectoryName(path);
        }

        svnWorkPath = path;

        if (ifNullSet)
        {
            if (!Directory.Exists(svnWorkPath))
            {
                SetWorkPath(false);
            }
            else
            {
                EditorUserSettings.SetConfigValue("SvnWorkPath", svnWorkPath);
            }
        }

        return svnWorkPath;
    }

    /// <summary>
    /// 进程启动
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="command"></param>
    /// <param name="workPath"></param>
    /// <param name="seconds"></param>
    protected static void ExecuteProcess(string filePath, string command, string workPath = "", int seconds = 0)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }
        Process process = new Process();//创建进程对象
        process.StartInfo.WorkingDirectory = workPath;
        process.StartInfo.FileName = filePath;
        process.StartInfo.Arguments = command;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardOutput = false;//不重定向输出
        try
        {
            if (process.Start())
            {
                if (seconds == 0)
                {
                    process.WaitForExit(); //无限等待进程结束
                }
                else
                {
                    process.WaitForExit(seconds); //等待毫秒
                }
            }
        }
        catch (Exception e)
        {
            //Debug.LogError(e.Message);
        }
        finally
        {
            process.Close();
        }

    }
}