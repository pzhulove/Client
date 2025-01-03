using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
///////删除linq

public class ExceptionManager : Singleton<ExceptionManager> {
    List<string> logBuffer = new List<string>();
    const int MAX_LOG_NUM = 250;
	const int WRITE_LOG_NUM = 50;

    List<string> abPackageBuffer = new List<string>();//分包记录加载的ab包名字，只在编辑器下用

    private readonly object preloadLocker = new object();
    private readonly object logBuffLocker = new object();
    private readonly object logBuffTimeLocker = new object();
    System.Diagnostics.DefaultTraceListener defaultListener = new System.Diagnostics.DefaultTraceListener();
    string loggerFilePath = string.Empty;
    public string LoggerFilePath { get { return loggerFilePath; } }
    public override void Init()
    {
        loggerFilePath = GetLogFolderPath();
#if DEBUG_REPORT_ROOT
        if (DebugSettings.GetInstance().DisableBugly)
            return;
#endif
        defaultListener.LogFileName = GetLogPath("CriticalExceptionLog");
        //先注掉，体验服放出看看
        //Application.logMessageReceived += OnExceptionCatch;
        //Application.logMessageReceivedThreaded += OnExceptionCatch;
#if EXCEPTION_UPLOAD
		ExceptionUploaderManager.instance.TryUploadOneError();
#endif
    }

    public override void UnInit()
    {
#if DEBUG_REPORT_ROOT
		if (DebugSettings.GetInstance().DisableBugly)
            return;
#endif
        //先注掉，体验服放出看看
        //Application.logMessageReceived -= OnExceptionCatch;
        //Application.logMessageReceivedThreaded -= OnExceptionCatch;
    }
    public void Fail(string str)
    {
        defaultListener.Fail(string.Format("[{0}]:{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms"), str));
    }

    public void OnExceptionCatch(string message, string stacktrace, LogType type)
    {
        lock (logBuffLocker)
        {
            if (LogType.Exception == type/* || LogType.Assert == type*/ || LogType.Error == type)
            {
                DateTime Dt = DateTime.Now;
                string strCurTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", Dt.Year, Dt.Month, Dt.Day, Dt.Hour, Dt.Minute, Dt.Second);
                string butMsg = "{\r\n" +
                    "\"message\":" + "\"" + message.Replace("\r\n", "") + "\"" +
                        ",\r\n\"stacktrace\":" + "\"" + stacktrace.Replace("\r\n", "") + "\"" +
                        ",\r\n\"time\":" + "\"" + strCurTime + "\""
                        + "\r\n" +
                        "\"device\":" + "\"";
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    butMsg += "iPhone\"";
                }
                else if (Application.platform == RuntimePlatform.Android)
                {
                    butMsg += "Android\"";
                }
                else
                {
                    butMsg += "PC Unity Editor\"";
                }
                butMsg += "\r\n}";
                if (type == LogType.Exception)
                {
                    Fail(string.Format("{0}", butMsg));
                }
                //if (type == LogType.Exception)
                //    Logger.ShowDailog("{0}\n{1}", message, stacktrace);

                // RecordLog(butMsg);
                //删掉超过MAX_LOG_NUM的部分
                if (logBuffer.Count > MAX_LOG_NUM)
                {
                    for (int i = 0; i < logBuffer.Count - MAX_LOG_NUM; ++i)
                    {
                        logBuffer.RemoveAt(0);
                    }
                }

                logBuffer.Add(butMsg);
                if (logBuffer.Count <= 0)
                    return;

                if (logBuffer.Count < WRITE_LOG_NUM)
                    return;

                string path = GetLogPath("Exception");

                //Logger.LogWarningFormat("PrintLogToFile:{0}", path);
                try
                {
                    FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.Flush();
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    for (int i = 0; i < logBuffer.Count; ++i)
                        sw.WriteLine(logBuffer[i]);

                    sw.Flush();
                    fs.Flush();
                    sw.Close();
                    fs.Close();
                    fs.Dispose();
                    logBuffer.Clear();
                }
                catch (Exception e)
                {
                    Fail(string.Format("OnExceptionCatch {0}\n{1}\n", e.ToString(), butMsg));
                }
#if EXCEPTION_UPLOAD
		         ExceptionUploaderManager.instance.TryCacheOneErrorLogWithoutLog();
#endif
            }
        }
    }

    public void RecordLog(string log)
    {
#if DEBUG_REPORT_ROOT
		if (DebugSettings.GetInstance().DisableBugly)
            return;
#endif
        lock (logBuffLocker)
        {
            //删掉超过MAX_LOG_NUM的部分
            if (logBuffer.Count > MAX_LOG_NUM)
            {
                for (int i = 0; i < logBuffer.Count - MAX_LOG_NUM; ++i)
                {
                    logBuffer.RemoveAt(0);
                }
            }

            logBuffer.Add(log);
            if (logBuffer.Count <= 0)
                return;

            if (logBuffer.Count < WRITE_LOG_NUM)
                return;

            string path = GetLogPath("Exception");

            //Logger.LogWarningFormat("PrintLogToFile:{0}", path);
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Flush();
                        sw.BaseStream.Seek(0, SeekOrigin.End);
                        for (int i = 0; i < logBuffer.Count; ++i)
                            sw.WriteLine(logBuffer[i]);
                        sw.Flush();
                        fs.Flush();
                        sw.Close();
                        fs.Close();
                    }
                }
                logBuffer.Clear();
            }
            catch (Exception e)
            {
                Fail(string.Format("PrintLogToFile {0} failed {1}", path, e.Message));
            }

#if EXCEPTION_UPLOAD
		ExceptionUploaderManager.instance.TryCacheOneErrorLog();
#endif
            //PrintLogToFile();
        }
    }
    public void SaveLog()
    {
        if (logBuffer.Count <= 0) return;
        string path = GetLogPath("Exception");
        lock (logBuffLocker)
        {
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {

                fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                sw = new StreamWriter(fs);
                sw.Flush();
                sw.BaseStream.Seek(0, SeekOrigin.End);
                for (int i = 0; i < logBuffer.Count; ++i)
                    sw.WriteLine(logBuffer[i]);
                sw.Flush();
                fs.Flush();
                sw.Close();
                fs.Close();
                sw = null;
                fs = null;
                logBuffer.Clear();
            }
            catch (Exception e)
            {
                if(sw != null)
                {
                    sw.Close();
                    sw = null;
                }
                if(fs != null)
                {
                    fs.Close();
                    fs = null;
                }
                Fail(string.Format("PrintLogToFile {0} failed {1}", path, e.Message));
            }
        }
    }

	public void PrintPreloadRes(List<string> contents)
	{
        string path = this.LoggerFilePath + "preload.txt";
        lock (preloadLocker)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.Flush();
                sw.BaseStream.Seek(0, SeekOrigin.End);
                for (int i = 0; i < contents.Count; ++i)
                    sw.WriteLine(contents[i]);

                sw.Flush();
                fs.Flush();
                sw.Close();
                fs.Close();
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("PrintPreloadRes {0} failed {1}", path, e.Message);
            }
        }
	}

    public void PrintABPackage(string ab)
    {
#if USE_SMALLPACKAGE && UNITY_EDITOR
        if (!Global.Settings.isRecordAB)
            return;
        if (string.IsNullOrEmpty(ab))
            return;

        string path = this.LoggerFilePath + "abPackageRecord.txt";
        lock (preloadLocker)
        {
            try
            {
                FileStream abFs = new FileStream(path, FileMode.Append, FileAccess.Write);
                StreamWriter abSw = new StreamWriter(abFs);

                var sw = abSw;
                var fs = abFs;

                sw.BaseStream.Seek(0, SeekOrigin.End);

                sw.WriteLine(ab);

                sw.Flush();
                fs.Flush();

                sw.Close();
                fs.Close();
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("PrintABPackage {0} failed {1}", path, e.Message);
            }
        }
#endif 		
    }

	public void PrintLogToFile(bool force=false)
    {
        lock (logBuffLocker)
        {
            if (logBuffer.Count <= 0)
                return;

            if (!force && logBuffer.Count < WRITE_LOG_NUM)
                return;

            string path = GetLogPath("Exception");
            //Logger.LogWarningFormat("PrintLogToFile:{0}", path);
            try
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.Flush();
                sw.BaseStream.Seek(0, SeekOrigin.End);
                for (int i = 0; i < logBuffer.Count; ++i)
                    sw.WriteLine(logBuffer[i]);

                sw.Flush();
                fs.Flush();
                sw.Close();
                fs.Close();
                logBuffer.Clear();
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("PrintLogToFile {0} failed {1}", path, e.Message);
            }

#if EXCEPTION_UPLOAD
		ExceptionUploaderManager.instance.TryCacheOneErrorLog();
#endif
        }

        //UploadAll();
    }

	public static string GetLogFolderPath()
	{
		//创建目录
		string path = null;
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			path = GetPersistentDataPath() + "/";
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			path = GetPersistentDataPath() + "/";
		}
		else if (Application.platform == RuntimePlatform.WindowsPlayer)
		{
			path = Application.dataPath + "/";
		}
		else
		{
			path = Application.dataPath;
			path = path.Substring(0, path.LastIndexOf('/')) + "/GameLog/";
		}

        if (!Directory.Exists(path))
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch(Exception e)
            {
                Logger.LogErrorFormat("GetLogFolderPath CreateDirectory {0} failed {1}", path, e.Message);
            }
        }

		return path;
	}


    public string GetLogPath(string strLogType)
    {
        
        //------------------------------------------------------------------------------
		var path = this.LoggerFilePath; //GetLogFolderPath();

        //创建文件名
        DateTime Dt = DateTime.Now;
        string FileName = string.Format("{0}-{1}-{2}-{3}.txt", strLogType, Dt.Year, Dt.Month, Dt.Day);
        //------------------------------------------------------------------------------------
        return path + FileName;
    }

    private static string GetPersistentDataPath()
    {
        //         if (Application.platform == RuntimePlatform.Android)
        //             return "file://" + Application.persistentDataPath;
        //         else if (Application.platform == RuntimePlatform.IPhonePlayer)
        //             return "file://" + Application.persistentDataPath;
        //         else if (Application.platform == RuntimePlatform.WindowsEditor)
        //             return "file:///" + Application.persistentDataPath;
        //         else
        //             return "file://" + Application.persistentDataPath;

        return Application.persistentDataPath;
    }

    public void Upload(string fieldName, string url, string fileName)
    {
        if (HttpClient.Instance == null)
        {
            //LogWarning("UpLoadLogFile.Upload: HttpClient.Instance is not inited.");
            return;
        }

        if (System.IO.File.Exists(fieldName))
        {
            HttpClient.Instance.BeginPostRequest();
            //             HttpClient.Instance.AddField("uuid", DeviceInfomation.Instance.DeviceUniqueCode());
            //             HttpClient.Instance.AddField("model", DeviceInfomation.Instance.DeviceMode());
            //             HttpClient.Instance.AddField("gameVersion", MainScript.Instance.InternalVersion);
            //             HttpClient.Instance.AddField("systemVersion", DeviceInfomation.Instance.DeviceOSVersion());
            //             HttpClient.Instance.AddField("manufacturer", DeviceInfomation.Instance.DeviceOSName());

            byte[] content = File.ReadAllBytes(fieldName);
            HttpClient.Instance.AddBinary("dump", content, fileName);
            HttpClient.Instance.PostRequest(url, null);
           // System.IO.File.Delete(fieldName);//上传后就删除本地Log文件
        }
        else
        {
            //LogWarning( fieldName + " not found" ) ;
            //UnityEngine.Debug.LogWarning( fieldName + " not found" ) ;
        }
    }

    public void UploadAll()
    {
        string dumpServerUrl = "ftp://daemon:xampp@192.168.0.103/dnf/ios/";
       // string dumpServerUrl = "http://192.168.108.1:8080/";

        string exceptionFilePath = GetLogPath("Exception");
        Upload(exceptionFilePath, dumpServerUrl, "exception.dump");

    }

    #region OpenFrameTime

    private List<string> logTimeBuffer = new List<string>();
    private bool isShowOpenFrameTime = true;
    private string specialFrameName = "SkillTreeFrame";
    private string openFrameTime = "OpenFrameTime";

    private readonly string[] _openFrameRecordList =
    {
        "SkillTreeFrame", "PackageNewFrame",
        "AuctionFrame","ActiveFuliFrame", "SettingPanel"
    };

    //private string[] _frameOpenRecordList =
    //{
    //    "PackageNewFrame", "FashionMergeNewFrame",
    //    "SmithShopFrame", "ActiveChargeFrame9380", "MallFrame", "AuctionFrame",
    //    "LegendFrame", "SettingFrame"
    //};

    public void RecordLogTime(string logTime)
    {
#if DEBUG_REPORT_ROOT
		if (DebugSettings.GetInstance().DisableBugly)
            return;
#endif
        lock (logBuffTimeLocker)
        {
            //删掉超过MAX_LOG_NUM的部分
            if (logTimeBuffer.Count > MAX_LOG_NUM)
            {
                for (int i = 0; i < logTimeBuffer.Count - MAX_LOG_NUM; ++i)
                {
                    logTimeBuffer.RemoveAt(0);
                }
            }

            logTimeBuffer.Add(logTime);
            if (logTimeBuffer.Count <= 0)
                return;

            if (logTimeBuffer.Count < WRITE_LOG_NUM)
                return;


            string framePath = openFrameTime;
            string path = GetLogPath(framePath);

            //Logger.LogWarningFormat("PrintLogToFile:{0}", path);
            try
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.Flush();
                sw.BaseStream.Seek(0, SeekOrigin.End);
                for (var i = 0; i < logTimeBuffer.Count; ++i)
                    sw.WriteLine(logTimeBuffer[i]);

                sw.Flush();
                fs.Flush();
                sw.Close();
                fs.Close();
                fs.Dispose();
                logTimeBuffer.Clear();
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("PrintLogToFile {0} failed {1}", path, e.ToString());
            }

#if EXCEPTION_UPLOAD
		ExceptionUploaderManager.instance.TryCacheOneErrorLog();
#endif
        }
    }



    public void PrintLogToFileTime(bool force = false,string openFramePath = null)
    {
        lock (logBuffTimeLocker)
        {
            if (logTimeBuffer.Count <= 0)
                return;

            if (!force && logTimeBuffer.Count < WRITE_LOG_NUM)
                return;


            string framePath = openFrameTime;
            if (openFramePath != null)
                framePath = openFramePath;
            string path = GetLogPath(framePath);

            //Logger.LogWarningFormat("PrintLogToFile:{0}", path);
            try
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.Flush();
                sw.BaseStream.Seek(0, SeekOrigin.End);
                for (var i = 0; i < logTimeBuffer.Count; ++i)
                    sw.WriteLine(logTimeBuffer[i]);

                sw.Flush();
                fs.Flush();
                sw.Close();
                fs.Close();
                fs.Dispose();
                logTimeBuffer.Clear();
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("PrintLogToFile {0} failed {1}", path, e.Message);
            }

#if EXCEPTION_UPLOAD
		ExceptionUploaderManager.instance.TryCacheOneErrorLog();
#endif
        }

        //UploadAll();
    }


    public void PrintOpenFrameTime(string frameName, float beginTime, float endTime, float costTime)
    {

        if (isShowOpenFrameTime == false)
            return;

        if(frameName != specialFrameName)
            return;

        var frameNameStr = string.Format("Frame Opened    {0}", frameName);
        var beginTimeStr = string.Format("Begin Time      {0}", beginTime);
        var endTimeStr = string.Format("End Time        {0}", endTime);
        var costTimeStr = string.Format("Cost Time       {0}", costTime);
        ExceptionManager.GetInstance().RecordLogTime("");
        ExceptionManager.GetInstance().RecordLogTime("");
        ExceptionManager.GetInstance().RecordLogTime(frameNameStr);
        ExceptionManager.GetInstance().RecordLogTime(beginTimeStr);
        ExceptionManager.GetInstance().RecordLogTime(endTimeStr);
        ExceptionManager.GetInstance().RecordLogTime(costTimeStr);
        ExceptionManager.GetInstance().RecordLogTime("");
        ExceptionManager.GetInstance().RecordLogTime("");
        ExceptionManager.GetInstance().PrintLogToFileTime(true, frameName);

    }

    public void PrintOpenFrameTime(string frameName, float loadGameObjectCostTime, float totalCostTime)
    {

        if (isShowOpenFrameTime == false)
            return;

        if(IsFrameOpendNeedRecord(frameName) == false)
            return;
        
        var loadGameObjectPercent = loadGameObjectCostTime / totalCostTime;
        var initFrameTime = totalCostTime - loadGameObjectCostTime;

        var totalCostTimeStr          = string.Format("OpenFrame Total Cost Time                {0:0.0000}", totalCostTime);
        var loadGameObjectCostTimeStr = string.Format("LoadGameObject Cost Time                 {0:0.0000} and Percent is {1:P2}", loadGameObjectCostTime,loadGameObjectPercent);
        var initFrameTimeStr          = string.Format("InitFrame Cost Time                      {0:0.0000}", initFrameTime);
        ExceptionManager.GetInstance().RecordLogTime("");
        ExceptionManager.GetInstance().RecordLogTime("");
        ExceptionManager.GetInstance().RecordLogTime(totalCostTimeStr);
        ExceptionManager.GetInstance().RecordLogTime(loadGameObjectCostTimeStr);
        ExceptionManager.GetInstance().RecordLogTime(initFrameTimeStr);
        ExceptionManager.GetInstance().RecordLogTime("");
        ExceptionManager.GetInstance().RecordLogTime("");
        ExceptionManager.GetInstance().PrintLogToFileTime(true, frameName);

    }

    public void PrintFrameTimeOneFile(string frameName, float loadGameObjectCostTime, float totalCostTime)
    {
        if (isShowOpenFrameTime == false)
            return;

        if (IsFrameOpendNeedRecord("AllFrames") == false)
            return;

        var loadGameObjectPercent = loadGameObjectCostTime / totalCostTime;
        var initFrameTime = totalCostTime - loadGameObjectCostTime;

        var totalCostTimeStr = string.Format("{1} OpenFrame Total Cost Time                {0:0.0000}", totalCostTime, frameName);
        ExceptionManager.GetInstance().RecordLogTime("");
        ExceptionManager.GetInstance().RecordLogTime("");
        ExceptionManager.GetInstance().RecordLogTime(totalCostTimeStr);
        ExceptionManager.GetInstance().RecordLogTime("");
        ExceptionManager.GetInstance().RecordLogTime("");
        ExceptionManager.GetInstance().PrintLogToFileTime(true, "AllFrames");
    }

    private bool IsFrameOpendNeedRecord(string frameName)
    {
        return true;
        //if (_openFrameRecordList == null || _openFrameRecordList.Length == 0)
        //    return false;

        //return _openFrameRecordList.Any(t => frameName == t);
    }

    #endregion
}
