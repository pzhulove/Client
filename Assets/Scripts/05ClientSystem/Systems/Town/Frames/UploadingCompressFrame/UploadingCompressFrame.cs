using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Tenmove;
using System.IO;

namespace GameClient
{
    public enum UploadingCompressState
    {
        None,
        Compressing,
        Uploading,
        Error,
        Finish,
    }


    public class UploadingCompressFrame : ClientFrame
    {
		#region ExtraUIBind
		private Text mTitle = null;
		private Text mLoadText = null;
		private Slider mLoadSlider = null;
		private Button mClose = null;
		
		protected override void _bindExUI()
		{
			mTitle = mBind.GetCom<Text>("Title");
			mLoadText = mBind.GetCom<Text>("loadText");
			mLoadSlider = mBind.GetCom<Slider>("loadSlider");
			mClose = mBind.GetCom<Button>("Close");
			if (null != mClose)
			{
				mClose.onClick.AddListener(_onCloseButtonClick);
			}
		}
		
		protected override void _unbindExUI()
		{
			mTitle = null;
			mLoadText = null;
			mLoadSlider = null;
			if (null != mClose)
			{
				mClose.onClick.RemoveListener(_onCloseButtonClick);
			}
			mClose = null;
		}
		#endregion

        private void _onCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/UploadingCompressFrame/UploadingCompressFrame";
        }

        protected override void _OnOpenFrame()
        {
            State = UploadingCompressState.None;
            _Start(ExceptionManager.instance.LoggerFilePath, "Exception*");
        }

        protected override void _OnCloseFrame()
        {
            //_Stop();
        }

        public UploadingCompressState State { get; private set; }


        public string ZipFilePath { get; private set; }

        public string FilePathRoot { get; private set; }
        public string FilePattern { get; private set; }

        public string ZipFileRoot {get; private set;}
        public string ZipFileName {get; private set;}


        private void _Start(string filePathRoot, string filePattern)
        {
            FilePathRoot = filePathRoot;
            FilePattern = filePattern;

            ZipFileRoot = _getRootPath();
            ZipFileName = _getZipFilePath();
            ZipFilePath = Path.Combine(ZipFileRoot, ZipFileName);
            TMPathUtil.MakeParentRootExist(ZipFilePath);

            Logger.LogProcessFormat("[UploadingCompressFrame] Start Process FilePathRoot {0}, Pattern {1}, ZipFilePath {2}", FilePathRoot, FilePattern, ZipFilePath);

            StartCoroutine(_Process());
            // StartCoroutine
            //
            //this.frame.gameObject.StartCoroutine(_Process());
        }

        private void _Stop()
        {

            Logger.LogProcessFormat("[UploadingCompressFrame] StopProcess FilePathRoot {0}, Pattern {1}, ZipFilePath {2}", FilePathRoot, FilePattern, ZipFilePath);

            StopCoroutine(_Process());
            // StopCoroutine
            //
            
            if (UploadingCompressState.Compressing == State)
            {
                if (null != mWaitCompressFile)
                {
                    mWaitCompressFile.Abort();
                    mWaitCompressFile = null;
                }
            }
            else if (UploadingCompressState.Uploading == State)
            {
                if (null != mWaitUploadFile)
                {
                    mWaitUploadFile.Abort();
                    mWaitUploadFile = null;
                }
            }
        }

        private WaitCompressFile mWaitCompressFile = null;
        private UnityWebRequest  mWaitUploadFile   = null;

        private string skDirName = "_bug_report_root_";

        private string _getRootPath()
        {
            string path = string.Empty;

#if UNITY_EDITOR
            path = Path.Combine("..", skDirName);
#else
            path = Path.Combine(Application.persistentDataPath, skDirName);
#endif


            return path;
        }

        private string _getZipFilePath()
        {
            string platform = "InvalidPlatform";
            string sdkChannel = "InvalidSDKChannel";
            string serverID = "InvalidServerName";
            string accountID = "InvalidAccountID";
            string roleID = "InvalidRoleID";
            string dateStr = TMPathUtil.GetCurrentDateTime();

            try 
            {
                platform = Application.platform.ToString();
            }
            catch (System.Exception e) { Logger.LogErrorFormat(e.ToString()); }

            try 
            {
                sdkChannel = Global.Settings.sdkChannel.ToString();
            }
            catch (System.Exception e) { Logger.LogErrorFormat(e.ToString()); }
            
            try 
            {
                serverID = ClientApplication.playerinfo.serverID.ToString();
            }
            catch (System.Exception e) { Logger.LogErrorFormat(e.ToString()); }

            try
            { 
                accountID = ClientApplication.playerinfo.accid.ToString(); 
            }
            catch (System.Exception e) { Logger.LogErrorFormat(e.ToString()); }


            try 
            { 
                roleID = ClientApplication.playerinfo.GetSelectRoleBaseInfoByLogin().roleId.ToString();
            }
            catch (System.Exception e) { Logger.LogErrorFormat(e.ToString()); }


            string fileName = string.Format("{0}_{1}_{2}_{3}_{4}_{5}.zip",
                    platform,
                    sdkChannel,
                    serverID,
                    accountID,
                    roleID,
                    dateStr
                    );

            return fileName;

        }
    

        private IEnumerator _Process()
        {
            mWaitCompressFile = new WaitCompressFile(ZipFilePath, FilePathRoot, FilePattern);

            State = UploadingCompressState.Compressing;

            while (mWaitCompressFile.MoveNext())
            {
                _SetRate(mWaitCompressFile.Rate);
                yield return null;
            }


            Logger.LogProcessFormat("[UploadingCompressFrame] Finish Compress FilePathRoot {0}, Pattern {1}, ZipFilePath {2}", FilePathRoot, FilePattern, ZipFilePath);

            if (!mWaitCompressFile.IsSuccessFinish())
            {
                State = UploadingCompressState.Error;
                Logger.LogErrorFormat("[CompressAndUploading] Compress Error");
                _SetTitle("压缩出错");
                yield break;
            }

            mWaitCompressFile.DeleteCompressedFiles();

            IList<string> zipFiles = TMFile.GetFiles(ZipFileRoot, "*.zip");

            for (int i = 0; i < zipFiles.Count; ++i)
            {
                string currentZipFilePath = zipFiles[i];


                Logger.LogProcessFormat("[UploadingCompressFrame] Start Uploading {0}", currentZipFilePath);


                byte[] payload = TMFile.ReadAllBytes(currentZipFilePath);

                string url = string.Format("{0}?file={1}&dataString=fk&deviceId=fk&serverId=fk&lastMoveLogTime=fk", "http://39.108.138.140:59969", Path.GetFileName(currentZipFilePath));
                //mWaitUploadFile = new UnityWebRequest("http://www.mysite.com/data-upload");
                //
                mWaitUploadFile = new UnityWebRequest(url,  UnityWebRequest.kHttpVerbPOST);
                UploadHandler uploader = new UploadHandlerRaw(payload);
                // Send header: "Content-Type: custom/content-type";
                //uploader.contentType = "custom/content-type";
                mWaitUploadFile.SetRequestHeader("Content-Type", "application/json");
                mWaitUploadFile.uploadHandler = uploader;
                mWaitUploadFile.Send();
                //mWaitUploadFile.downloadHandler = new DownloadHandlerBuffer();

                _SetTitle("开始上传");


                float time = 0.0f;
                ulong lastByte = 0;
                State = UploadingCompressState.Uploading;


                while (!mWaitUploadFile.isDone && time < 10.0f)
                {
                    yield return null;

                    float rate = mWaitUploadFile.uploadProgress / zipFiles.Count + i * 1.0f / zipFiles.Count;
                    Logger.LogProcessFormat("[UploadingCompressFrame] rate {0} {1}", rate, mWaitUploadFile.uploadProgress);
                    _SetRate(rate);

                    if (lastByte == mWaitUploadFile.uploadedBytes)
                    {
                        time += Time.deltaTime;
                    }
                    else 
                    {
                        lastByte = mWaitUploadFile.uploadedBytes;
                        time = 0.0f;
                    }
                }

                Logger.LogProcessFormat("[UploadingCompressFrame] Finish Uploading {0}", currentZipFilePath);

                if (!mWaitUploadFile.isDone && time >= 10.0f)
                {
                    State = UploadingCompressState.Error;
                    Logger.LogErrorFormat("[CompressAndUploading] WebReqError {0}", mWaitUploadFile.error);
                    _SetTitle("上传超时");
                    yield break;
                }

                if (mWaitUploadFile.isNetworkError)
                {
                    State = UploadingCompressState.Error;
                    Logger.LogErrorFormat("[CompressAndUploading] WebReqError {0}", mWaitUploadFile.error);
                    _SetTitle("上传失败");
                    yield break;
                }

                if (File.Exists(currentZipFilePath))
                {
                    File.Delete(currentZipFilePath);
                }
            }

            State = UploadingCompressState.Finish;

            _SetTitle("成功");
        }

        private void _SetRate(float rate)
        {
            rate = Mathf.Clamp01(rate);
            if (null != mLoadSlider)
            {
                mLoadSlider.value = rate;
            }

            if (null != mLoadText)
            {
                mLoadText.text = string.Format("{0:0.00}%", rate * 100);
            }
        }

        private void _SetTitle(string msg)
        {
            if (null == mTitle)
            {
                return;
            }

            mTitle.text = msg;
        }

    }
}
