using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;

using LitJson;

public class ExceptionUploaderManager : Singleton<ExceptionUploaderManager> 
{
	protected string mUploadUrl = string.Empty;
	protected string   mOpenID  = string.Empty;
    private readonly object mLocker = new object();
    private const uint kTime2Upload = 60 * 60;

	public void SetUploadUrlAndOpenID(string url, string openID)
    {
#if EXCEPTION_UPLOAD

        mUploadUrl = url;
		mOpenID = openID;

		Logger.LogProcessFormat("[日志上传] 更新日志上传 URL {0} openID {1}", mUploadUrl, mOpenID);

		TryCacheOneErrorLog();
#endif
	}

    protected class LogDescrible
    {
        public string deviceID     { get; set; }
        public string deviceInfo   { get; set; }
		public string deviceType   { get; set; }
		public string deviceName   { get; set; }
		public string deviceMemorySize { get; set; }
		public string deviceOperatSystem { get; set; }
		public string deviceOperatSystemFamily { get; set; }
		public string devicePlatform     { get; set; }

        public string sdkChannel    { get; set; }
        public string openID        { get; set; }
        public string serverID      { get; set; }
        public string uploadUrl     { get; set; }
		public string uploadLogsPath 	{ get; set; }

		public string dataString   { get; set; }

		public uint   lastMoveLogTime { get; set; }
    }


    private const string kFileRootDir    = "_log2upload_";
    private const string kConfigFileName = "upload.conf";

    public void TryCacheOneErrorLog()
    {
#if EXCEPTION_UPLOAD
        lock(mLocker)
        {
		    if (string.IsNullOrEmpty(mUploadUrl))
            {
			    ExceptionManager.GetInstance().Fail("[日志上传] url 为空");
                return ;
            }

            string uploadConfPath         = _getUploadConfigPath();

		    LogDescrible desc             = _getLogDescrible(uploadConfPath);

		    if (null == desc) 
		    {
			    ExceptionManager.GetInstance().Fail("[日志上传] 描述文件为空 {0}", uploadConfPath);

			    return ;
		    }
	
		    if (!_canMoveExceptionLog (desc)) 
		    {
			    ExceptionManager.GetInstance().Fail("[日志上传] 不能移动日志 {0}", uploadConfPath);

			    return;
		    }

		    string excpLog 		  = _getExcptionLogs ();
		    uint currentTimeStamp = Utility.GetCurrentTimeUnix ();

		    string excpTargetRoot = _getCachePathWithCreate ();
		    if (File.Exists (excpLog))
		    {
			    string excpTargetPath = Path.Combine(excpTargetRoot, string.Format("{0}_{1}.txt", Path.GetFileNameWithoutExtension(excpLog), currentTimeStamp));

			    try 
			    {
				    File.Move (excpLog, excpTargetPath);
			    } 
			    catch (Exception e) 
			    {
				    ExceptionManager.GetInstance().Fail("[日志上传] 移动 {0} -> {1} 失败 {2}", excpLog, excpTargetPath, e.ToString());
			    }
		    } 
		    else
		    {
			    ExceptionManager.GetInstance().Fail("[日志上传] 移动文件不存在 {0}", excpLog);
		    }

		    desc.uploadUrl 			= mUploadUrl;
		    desc.lastMoveLogTime 	= currentTimeStamp;
		    desc.uploadLogsPath 	= excpTargetRoot;

		    try
		    {
			    string jsonText = LitJson.JsonMapper.ToJson(desc);
			    File.WriteAllText(uploadConfPath, jsonText);
		    }
		    catch (Exception e)
		    {
			    ExceptionManager.GetInstance().Fail("[日志上传] 写入文件失败 {0}, {1}", uploadConfPath, e.ToString());
		    }
        }
#endif
    }
    public void TryCacheOneErrorLogWithoutLog()
    {
#if EXCEPTION_UPLOAD
        lock(mLocker)
        {
            if (string.IsNullOrEmpty(mUploadUrl))
            {
                ExceptionManager.GetInstance().Fail("[日志上传] url 为空");
                return;
            }

            string uploadConfPath = _getUploadConfigPath();

            LogDescrible desc = _getLogDescrible(uploadConfPath);

            if (null == desc)
            {
                ExceptionManager.GetInstance().Fail(string.Format("[日志上传] 描述文件为空 {0}", uploadConfPath));
                return;
            }

            if (!_canMoveExceptionLog(desc))
            {
                ExceptionManager.GetInstance().Fail(string.Format("[日志上传] 不能移动日志 {0}", uploadConfPath));
                return;
            }

            string excpLog = _getExcptionLogs();
            uint currentTimeStamp = Utility.GetCurrentTimeUnix();

            string excpTargetRoot = _getCachePathWithCreate();
            if (File.Exists(excpLog))
            {
                string excpTargetPath = Path.Combine(excpTargetRoot, string.Format("{0}_{1}.txt", Path.GetFileNameWithoutExtension(excpLog), currentTimeStamp));

                try
                {
                    File.Move(excpLog, excpTargetPath);
                }
                catch (Exception e)
                {
                    ExceptionManager.GetInstance().Fail(string.Format("[日志上传] 移动 {0} -> {1} 失败 {2}", excpLog, excpTargetPath, e.ToString()));
                }
            }
            else
            {
                ExceptionManager.GetInstance().Fail(string.Format("[日志上传] 移动文件不存在 {0}", excpLog));
            }

            desc.uploadUrl = mUploadUrl;
            desc.lastMoveLogTime = currentTimeStamp;
            desc.uploadLogsPath = excpTargetRoot;

            try
            {
                string jsonText = LitJson.JsonMapper.ToJson(desc);
                File.WriteAllText(uploadConfPath, jsonText);
            }
            catch (Exception e)
            {
                ExceptionManager.GetInstance().Fail(string.Format("[日志上传] 写入文件失败 {0}, {1}", uploadConfPath, e.ToString()));
            }
        }
#endif
    }

    public void TryUploadOneError()
	{
#if EXCEPTION_UPLOAD
		_removeAllZipFiles ();

		string findRoot = _getRootPath ();

		string[] allFiles = Directory.GetFiles (findRoot, string.Format("*{0}", kConfigFileName), SearchOption.TopDirectoryOnly);

		for (int i = 0; i < allFiles.Length; ++i) 
		{
			if (File.Exists (allFiles [i])) 
			{
				LogDescrible desc = _loadExistLogDesc (allFiles [i]);

				if (null != desc) 
				{
					if (!Directory.Exists(desc.uploadLogsPath))
					{
						File.Delete(allFiles [i]);
						continue;
					}

					_compressAndUpload (allFiles[i], desc);
					break;
				}
			}
		}
#endif
	}

	private void _removeAllZipFiles()
	{
#if !UNITY_EDITOR
		try 
		{
			string findRoot = _getRootPath ();

			string[] allFiles = Directory.GetFiles (findRoot, "*.zip", SearchOption.TopDirectoryOnly);

			for (int i = 0; i < allFiles.Length; ++i)
			{
				if (File.Exists (allFiles [i]))
				{
					File.Delete(allFiles [i]);
				}
			}
		}
		catch(Exception e)
		{
			Logger.LogErrorFormat("[上传日志] 删除缓存*.zip 失败 {0}", e.ToString());
		}
#endif
	}

	private LogDescrible _getLogDescrible(string uploadConfPath)
	{
		LogDescrible desc = null;

		if (!File.Exists (uploadConfPath))
		{
			desc = _createNewLogDesc();
		} 
		else
		{
			desc = _loadExistLogDesc(uploadConfPath);
		}

		return desc;
	}

	private LogDescrible _loadExistLogDesc(string uploadConfPath)
	{
		LogDescrible desc = null;

		if (!File.Exists (uploadConfPath))
		{
			return null;
		}

		try 
		{
			string jsonContent = File.ReadAllText (uploadConfPath);
			desc = LitJson.JsonMapper.ToObject<LogDescrible> (jsonContent);	
		} 
		catch (Exception e)
		{
			Logger.LogErrorFormat("[日志上传] 读取或转json失败 {0}", uploadConfPath);
		}

		return desc;
	}

	private LogDescrible _createNewLogDesc()
	{
		LogDescrible desc             = new LogDescrible();

		desc.deviceID                 = SystemInfo.deviceUniqueIdentifier.ToString ();
		desc.deviceInfo               = SystemInfo.deviceModel.ToString();
		desc.deviceType               = SystemInfo.deviceType.ToString();
		desc.deviceName               = SystemInfo.deviceName.ToString();
		desc.deviceMemorySize         = SystemInfo.systemMemorySize.ToString();
		desc.deviceOperatSystem       = SystemInfo.operatingSystem.ToString();
		desc.deviceOperatSystemFamily = SystemInfo.operatingSystemFamily.ToString();
		desc.devicePlatform           = Application.platform.ToString();
		desc.sdkChannel               = Global.Settings.sdkChannel.ToString();

		desc.openID = mOpenID;
		desc.dataString 			  = _getTodayString ();


		desc.uploadUrl                = mUploadUrl;
		desc.uploadLogsPath 		  = "";
		desc.lastMoveLogTime 		  = 0;

		try
		{
			desc.serverID                 = ClientApplication.adminServer.id.ToString();
		} catch (Exception e)
		{
		}

		return desc;
	}

	private bool _canMoveExceptionLog(LogDescrible desc)
	{
		if (null == desc)
		{
			return false;
		}

		int time = (int)(Utility.GetCurrentTimeUnix () - desc.lastMoveLogTime) - (int)kTime2Upload;

		Logger.LogProcessFormat("[上传日志] 相差时间  {0}s", time);

		return time >= 0;
	}

	private string _getUploadConfigPath()
	{
		return Path.Combine(_getRootPath(), string.Format("{0}-{1}", _getTodayString(), kConfigFileName));
	}

    private string _getCachePathWithCreate()
    {
		string path = Path.Combine(_getRootPath(), _getTodayString());

        _makeDirExist(path);

        return path;
    }

	private string _getTodayString()
	{
		DateTime Dt = DateTime.Now;
		return string.Format ("{0}-{1}-{2}", Dt.Year, Dt.Month, Dt.Day);
	}

	private string _getYesterDayString()
	{
		DateTime Dt = DateTime.Now.AddDays(-1);
		return string.Format ("{0}-{1}-{2}", Dt.Year, Dt.Month, Dt.Day);
	}

    private string _getRootPath()
    {
        string path = string.Empty;

#if UNITY_EDITOR
        path = Path.Combine("..", kFileRootDir);
#else
        path = Path.Combine(Application.persistentDataPath, kFileRootDir);
#endif

        _makeDirExist(path);
        
        return path;
    }
    
    private void _makeDirExist(string path)
    {
		if (!Directory.Exists(path))
        {
			Directory.CreateDirectory(path);
            Logger.LogProcessFormat("[日志上传] 创建文件夹 {0}", path);
        }
    }

	private string _getExcptionLogs()
	{
		string exceptionLogPath = ExceptionManager.GetLogFolderPath();

		DirectoryInfo dinfo = new DirectoryInfo(exceptionLogPath);
		if (null == dinfo) 
		{
			return string.Empty;
		}

		List<FileInfo> allLogFiles = new List<FileInfo>(dinfo.GetFiles("Exception*", SearchOption.AllDirectories));

		allLogFiles.Sort((fst, snd)=>{
			if (fst.LastWriteTime > snd.LastWriteTime)
			{
				return -1;
			}
			else if (fst.LastWriteTime == snd.LastWriteTime)
			{
				return 0;
			}
			else
			{
				return 1;
			}
		});
	
		if (allLogFiles.Count > 0) {
			return allLogFiles[0].FullName;
		}

		return string.Empty;
	}

    

	private void _compressAndUpload(string logDescFilePath , LogDescrible logDesc)
	{
		if (!File.Exists (logDescFilePath)) {
			Logger.LogErrorFormat ("[日志上传] 压缩上传 描述文件不存在 {0}", logDescFilePath);
			return;
		}

		if (null == logDesc) {
			Logger.LogErrorFormat ("[日志上传] 压缩上传 描述文件对象为空 {0}", logDescFilePath);
			return;
		}

		if (!Directory.Exists (logDesc.uploadLogsPath)) {
			Logger.LogErrorFormat ("[日志上传] 压缩上传 日志路径不存在 {0}", logDesc.uploadLogsPath);
			return;
		}

		List<string> files = new List<string> (Directory.GetFiles (logDesc.uploadLogsPath, "*", SearchOption.TopDirectoryOnly));

		if (files.Count <= 0) {
			Logger.LogProcessFormat("[日志上传] 压缩上传 日志路径文件为空 {0}", logDesc.uploadLogsPath);
			return;
		}

		files.Add (logDescFilePath);

		string targetFileName = string.Format ("{0}-{1}-{2}.zip", logDesc.dataString, logDesc.deviceID, logDesc.lastMoveLogTime).Replace(" ", "_");
		string targetZipPath  = Path.Combine(_getRootPath(), targetFileName);

		if (!LibZip.LibZipFileReader.CompressFiles (targetZipPath, files.ToArray()))
		{
			Logger.LogErrorFormat ("[日志上传] 压缩 失败 {0}", targetZipPath);
			return;
		}

		_uploadHttpFile (logDesc, targetZipPath);

		try
		{
			for (int i = 0; i < files.Count; ++i)
			{
				if (File.Exists (files [i])) 
				{
					if (files[i] == logDescFilePath)
					{
						if (logDesc.dataString != _getTodayString ()) 
						{
							File.Delete(files [i]);
							Directory.Delete (logDesc.uploadLogsPath, true);
						}

						break;
					}
					else
					{
						File.Delete (files [i]);
					}
				}
			}
		}
		catch (Exception e)
		{
			Logger.LogErrorFormat ("[日志上传] 删除文件失败 {0}", e.ToString());
		}
	}

	private void _uploadHttpFile(LogDescrible logDesc,  string path)
	{
		if (null == logDesc) 
		{
			return ;
		}

		string fileName = Path.GetFileName(path);
		string url = string.Format("{0}?file={1}&dataString={2}&deviceId={3}&serverId{4}&lastMoveLogTime={5}", 
			logDesc.uploadUrl, fileName, logDesc.dataString, logDesc.deviceID, logDesc.serverID, logDesc.lastMoveLogTime
		).Replace(" ", "_");

		Http.UploadFile(url, path);

		UnityEngine.Debug.LogFormat("[上传日志] url {0} path: {1}", url, path);
	}
}
