using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class ETCLogger {
    static string _logdir = null;

    static string logdir
    {
        get
        {
            if (_logdir == null)
            {
                _logdir = Application.dataPath.Substring(0, Application.dataPath.IndexOf("Assets"));
                _logdir += "etctools.log";
            }

            return _logdir;
        }
    }

    public static void ClearLogger()
    {
        File.Delete(logdir);
        Debug.Log("ETCTools日志清理完成");
    }

    public static void I(string format, params object[] args)
    {
        Output("I", format, args);
    }

    public static void W(string format, params object[] args)
    {
        Output("W", format, args);
    }

    public static void E(string format, params object[] args)
    {
        Output("E", format, args);
    }

    static void Output(string level, string format, params object[] args)
    {
        string output = string.Format(
            "{0} {1}\r\n",
            System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"),
            string.Format(format, args));

        if(level == "W")
        {
            Debug.LogWarning(output);
        }
        else if (level == "E")
        {
            Debug.LogError(output);
        }

        FileStream fs = File.Open(logdir, FileMode.Append);
        if(fs == null)
        {
            Debug.LogWarningFormat("日志文件打开出错: {0}", logdir);
            return;
        }

        byte[] data = System.Text.Encoding.Default.GetBytes(level + " " + output);
        //开始写入
        fs.Write(data, 0, data.Length);

        fs.Flush();
        fs.Close();
    }
}
