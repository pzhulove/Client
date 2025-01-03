using UnityEngine;
using System.Text;
using UnityEngine.UI;
using System.Collections;

namespace GameClient
{
    public class VersionUpdateFrame : ClientFrame
    {
        [UIControl("hot_update_progress")]
        Slider m_LoadProcess;

        [UIControl("hot_update_progress/hot_update_progress_info")]
        Text m_LoadingProgressInfoText;

        [UIControl("hot_update_progress/hot_update_progress_cur")]
        Text m_LoadingProgressText;

        [UIControl("hot_update_progress")]
        Image m_LoadProcessBgImg;

        [UIControl("hot_update_progress/hot_update_progress_bar")]
        Image m_LoadProcessCoverImg;

        [UIObject("version_info_background")]
        GameObject versionInfoBg;

        [UIControl("hot_update_background")]
        Image mBackground;

        protected StringBuilder m_StringBuilder;
        protected string m_UpdateInfo;

        protected int m_UpdateSpeed = 10;
        protected int m_TargetProgress = 0;
        protected int m_CurrentProgress = -1;

        public void UpdateProgressState(string state)
        {
            if(null != m_LoadingProgressText)
                m_LoadingProgressText.text = state;
        }

        public void ResetProgress(string info)
        {
            m_TargetProgress = 0;
            m_CurrentProgress = -1;

            m_UpdateInfo = info;
        }

        public void UpdateProgress(int targetProgress,string info)
        {
            if (targetProgress > m_TargetProgress)
                m_TargetProgress = targetProgress;

            if(info != null)
            {
                m_UpdateInfo = info;
            }
        }

        public override string GetPrefabPath()
        {
            return "Base/Version/VersionFrame/VersionUpdateFrame";
        }

        protected override void _OnOpenFrame()
        {
            SetBackgroundImage();

#if UNITY_EDITOR
            m_UpdateSpeed = Global.Settings.loadingProgressStepInEditor;
#else
            m_UpdateSpeed = Global.Settings.loadingProgressStep;
#endif
            GameObject.DontDestroyOnLoad(frame);
            m_StringBuilder = StringBuilderCache.Acquire();
            m_TargetProgress = 0;
            m_CurrentProgress = -1;
            StartCoroutine(_UpdateProgress());
        }

        // 销毁
        protected override void _OnCloseFrame()
        {
            StringBuilderCache.Release(m_StringBuilder);
        }

        protected override bool _IsLoadingFrame()
        {
            return false;
        }

        protected void _SetProgress(int progress)
        {
            if (progress < 0)
                progress = 0;
            if (progress > 100)
                progress = 100;

            m_LoadProcess.value = progress / 100.0f;
            m_StringBuilder.Clear();
            m_StringBuilder.AppendFormat("{0}%", progress);
            m_LoadingProgressInfoText.text = m_UpdateInfo + m_StringBuilder.ToString();
        }

        public IEnumerator _UpdateProgress()
        {
            while (m_TargetProgress <= 100)
            {
                while (m_CurrentProgress < m_TargetProgress)
                {
                    m_CurrentProgress += m_UpdateSpeed;
                    if (m_CurrentProgress > m_TargetProgress)
                        m_CurrentProgress = m_TargetProgress;
                    
                    _SetProgress(m_CurrentProgress);
                    yield return Yielders.EndOfFrame;
                }

                if (m_TargetProgress == 100)
                {
                    //frameMgr.CloseFrame(this);
                    //yield return GameFrameWork.instance.OpenFadeFrame(() => { frameMgr.CloseFrame(this); });
                    break;
                }

                yield return Yielders.EndOfFrame;
            }
        }

        //add by mjx for version info show

        public void SetVersionInfoBgActive(bool isShow)
        {
            if (versionInfoBg)
            {
                versionInfoBg.CustomActive(isShow);
            }
        }

        private void SetBackgroundImage()
        {
            if (mBackground != null)
            {
                string imgPath = PluginManager.GetSDKLogoPath(SDKInterface.SDKLogoType.VersionUpdate);
                if (string.IsNullOrEmpty(imgPath))
                    return;
                Sprite bgSprite = Resources.Load<Sprite>(imgPath);
                if (bgSprite != null)
                {
                    mBackground.sprite = bgSprite;
                }
            }
        }

        public void SetSliderColorAlpha(float colorAlpha)
        {            
            if (m_LoadProcessBgImg)
            {
                Color bgImgColor = m_LoadProcessBgImg.color;
                m_LoadProcessBgImg.color = new Color(bgImgColor.r, bgImgColor.g, bgImgColor.b, colorAlpha);
            }
            if(m_LoadProcessCoverImg)
            {
                Color coverImgColor = m_LoadProcessCoverImg.color;
                m_LoadProcessCoverImg.color = new Color(coverImgColor.r, coverImgColor.g, coverImgColor.b, colorAlpha);
            }
            if (m_LoadingProgressInfoText)
            {
                Color infoTextColor = m_LoadingProgressInfoText.color;
                m_LoadingProgressInfoText.color = new Color(infoTextColor.r, infoTextColor.g, infoTextColor.b, colorAlpha);
            }
            if (m_LoadingProgressText)
            {
                Color textColor = m_LoadingProgressText.color;
                m_LoadingProgressText.color = new Color(textColor.r, textColor.g, textColor.b, colorAlpha);
            }
        }
    }
}

