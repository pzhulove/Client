using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Protocol;
using Network;

namespace GameClient
{
	public class SmallPackageDownloadFrame : ClientFrame {

		#region ExtraUIBind
		private Text mTxtDownloadSize = null;
		private Button mBtnDownload = null;
		private GameObject mRootIsDownloading = null;
		private GameObject mRootDownload = null;
		private Slider mSlider = null;
        private Button mClose = null;
        private GameObject mRootDownloaded = null;
        private Button mBtnCancelDoanload = null;

        protected override void _bindExUI()
		{
			mTxtDownloadSize = mBind.GetCom<Text>("txtDownloadSize");
			mBtnDownload = mBind.GetCom<Button>("btnDownload");
			mBtnDownload.onClick.AddListener(_onBtnDownloadButtonClick);
			mRootIsDownloading = mBind.GetGameObject("rootIsDownloading");
			mRootDownload = mBind.GetGameObject("rootDownload");
			mSlider = mBind.GetCom<Slider>("slider");
            mClose = mBind.GetCom<Button>("Close");
            mClose.onClick.AddListener(_onCloseButtonClick);
            mRootDownloaded = mBind.GetGameObject("rootDownloaded");
            mBtnCancelDoanload = mBind.GetCom<Button>("btnCancelDoanload");
            mBtnCancelDoanload.onClick.AddListener(_onBtnCancelDoanloadButtonClick);
        }

		protected override void _unbindExUI()
		{
			mTxtDownloadSize = null;
			mBtnDownload.onClick.RemoveListener(_onBtnDownloadButtonClick);
			mBtnDownload = null;
			mRootIsDownloading = null;
			mRootDownload = null;
			mSlider = null;
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
            mRootDownloaded = null;
            mBtnCancelDoanload.onClick.RemoveListener(_onBtnCancelDoanloadButtonClick);
            mBtnCancelDoanload = null;
        }
		#endregion   

		#region Callback
		private void _onBtnDownloadButtonClick()
		{
           //如果是4G下，wifi是主动开始后台下载
            if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                var remainSize = (Mathf.Abs(totalDownloadSize - currentDownloadSize)) / 1024 / 1024;
                SystemNotifyManager.SysNotifyMsgBoxOkCancel(string.Format("当前是在4G环境下，是否下载剩余的{0}M资源", remainSize), ()=> {
                    _resetTimeAcc();
                    SDKInterface.Instance.OpenDownload();
                    isDownloading = true;
                });
            }
            else {
                _resetTimeAcc();
                SDKInterface.Instance.OpenDownload();
                isDownloading = true;
            }
        }

        private void _onCloseButtonClick()
        {
            ClientSystemManager.instance.CloseFrame<SmallPackageDownloadFrame>();
        }

        private void _onBtnCancelDoanloadButtonClick()
        {
            if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                SystemNotifyManager.SysNotifyMsgBoxOkCancel("确认停止下载吗", () =>
                {
                    SDKInterface.Instance.CloseDownload();
                    _resetTimeAcc();
                    isDownloading = false;
                });
            }
        }

        #endregion

        bool isDownloading = false;
        bool isDownloaded = false;
		long currentDownloadSize;
		long totalDownloadSize;
		long preDownloadSize;
        bool isSmallPackage = false;

        int count = 0;
		float timeAcc = 0;
		float UPDATE_INTERVAL = 0.5f;
		public override string GetPrefabPath()
		{
			return "UIFlatten/Prefabs/SmallPackage/SmallPackageDownloadFrame";
		}

        protected override void _OnOpenFrame()
        {
            isSmallPackage = SDKInterface.Instance.IsSmallPackage();
            if (!isSmallPackage)
            {
                SetData();
                return;
            }

            timeAcc = 0;
            count = 0;
            isDownloaded = SDKInterface.Instance.IsResourceDownloadFinished();
            currentDownloadSize = SDKInterface.Instance.GetResourceDownloadedSize();
            preDownloadSize = currentDownloadSize;
            totalDownloadSize = SDKInterface.Instance.GetTotalResourceDownloadSize();

            SetData();
        }

        void _resetTimeAcc()
        {
            timeAcc = UPDATE_INTERVAL;
        }
        void SetData()
		{
            if (mRootDownloaded != null)
                mRootDownloaded.CustomActive(false);
            if (mRootIsDownloading != null)
                mRootIsDownloading.CustomActive(false);
            if (mRootDownload != null)
                mRootDownload.CustomActive(false);

            if (isDownloaded)
            {
                currentDownloadSize = totalDownloadSize;
            }

			if (mSlider != null)
			{
				float rate = 0;
				if (totalDownloadSize > 0)
					rate = currentDownloadSize / (float)totalDownloadSize;
				mSlider.value = rate;
			}

			if (mTxtDownloadSize != null)
			{
				mTxtDownloadSize.text = string.Format("{0}M/{1}M", currentDownloadSize/1024/1024, totalDownloadSize/ 1024 / 1024);
			}
			
            if (isSmallPackage)
            {
                if (isDownloaded)
                {
                    if (mRootDownloaded != null)
                        mRootDownloaded.CustomActive(true);
                }
                else
                {
                    if (isDownloading)
                    {
                        if (mRootIsDownloading != null)
                        {
                            mRootIsDownloading.CustomActive(true);
                            //只有4G下才显示出取消的按钮
                            mBtnCancelDoanload.CustomActive(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork);
                        }
                            
                    }
                    else
                    {
                        if (mRootDownload != null)
                            mRootDownload.CustomActive(true);
                    }
                }
            }

        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
		{
            if (!isSmallPackage)
                return;

            if (isDownloaded)
                return;

            timeAcc += timeElapsed;
            if (timeAcc > UPDATE_INTERVAL)
            {
                timeAcc -= UPDATE_INTERVAL;

                isDownloaded = SDKInterface.Instance.IsResourceDownloadFinished();
                currentDownloadSize = SDKInterface.Instance.GetResourceDownloadedSize();
                if (preDownloadSize != currentDownloadSize)
                {
                    isDownloading = true;
                    count = 0;
                }
                else
                {
                    //如果之前已经在下载了
                    if (isDownloading)
                    {
                        count++;
                    }

                    if (count >= 10)
                    {
                        isDownloading = false;
                        count = 0;
                    }
                }
                    

                preDownloadSize = currentDownloadSize;

                SetData();
            }

        }

	}
}


