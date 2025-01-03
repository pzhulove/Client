using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace GameClient
{
    public class RoleSceneLoadingFrame : ClientFrame
    {
        [UIControl("Loading_progress")]
        Slider m_LoadProcess;

        [UIControl("Loading_progress/loading_progress_info")]
        Text m_LoadingProgressInfoText;

        [UIControl("Loading_progress/loading_progress_state")]
        Text m_LoadingProgressStateText;

        [UIControl("Loading_background")]
        Image mBgImg;

        protected StringBuilder m_StringBuilder;

        protected string m_DisplayInfo;
        protected string m_DisplayState;

        protected int m_CurrentProgress = 0;
        protected int m_TargetProgress = 0;

        protected List<string> m_LoadingResList = new List<string>();

        #region IClientFrame

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SelectRole/SelectRoleLoadingFrame";
        }
        protected override void _OnOpenFrame()
        {
            GameObject.DontDestroyOnLoad(frame);
            m_StringBuilder = StringBuilderCache.Acquire();
            Reset();
            SetBackgrounImage();
            StartCoroutine(_Update());
        }

        protected override void _OnCloseFrame()
        {
            if(null != m_StringBuilder)
                StringBuilderCache.Release(m_StringBuilder);
        }

        #endregion

        #region 对外方法

        public void Reset()
        {
            m_TargetProgress = 0;
            m_CurrentProgress = 0;

            m_DisplayInfo = "";
            m_DisplayState = "";

            m_LoadProcess.value = m_TargetProgress * 0.01f;
            m_LoadingProgressInfoText.text = m_DisplayInfo;
            m_LoadingProgressStateText.text = m_DisplayState;
        }

        public void AddLoadingTask(string res)
        {
            if (!string.IsNullOrEmpty(res))
                m_LoadingResList.Add(res);
        }

        public void ProcessLoading()
        {
            StartCoroutine(_StepLoading());
        }

        #endregion

        #region 内部方法
        protected IEnumerator _StepLoading()
        {
            for(int i =0,icnt = m_LoadingResList.Count;i<icnt;++i)
            {
                _SetLoadingInfo((int)(i * 100.0f / icnt), null,"正在加载角色资源...");
                //IAssetAsyncRequest curRes = CGameObjectPool.instance.GetGameObjectAsync(m_LoadingResList[i], enResourceType.BattleScene, false);
                //if(null != curRes)
                //    curRes.Abort();

                // uint curRes = CGameObjectPool.instance.GetGameObjectAsync(m_LoadingResList[i], enResourceType.BattleScene, (uint)GameObjectPoolFlag.HideAfterLoad,0x9231bb2c);
                // if (CGameObjectPool.INVILID_HANDLE != curRes)
                //     CGameObjectPool.instance.AbortRequest(curRes);

                yield return Yielders.EndOfFrame;
            }

            m_LoadingResList.Clear();

            ClientSystemManager.instance.CloseFrame(this);
        }

        protected void _SetLoadingInfo(int progress, string info = null, string state = null)
        {
            if (progress > m_TargetProgress)
                m_TargetProgress = progress;

            if (!string.IsNullOrEmpty(info))
                m_DisplayInfo = info;

            if (!string.IsNullOrEmpty(state))
                m_DisplayState = state;
        }

        protected IEnumerator _Update()
        {
            while (m_TargetProgress <= 100)
            {
                while (m_CurrentProgress < m_TargetProgress)
                {
                    m_CurrentProgress += 100;
                    if (m_CurrentProgress > m_TargetProgress)
                        m_CurrentProgress = m_TargetProgress;

                    _DisplayPrgress(m_CurrentProgress);
                    yield return Yielders.EndOfFrame;
                }

                if (m_TargetProgress == 100)
                {
                    break;
                }

                yield return Yielders.EndOfFrame;
            }
        }

        protected void _DisplayPrgress(int curPrgress)
        {
            if (curPrgress < 0)
                curPrgress = 0;
            if (curPrgress > 100)
                curPrgress = 100;

            m_LoadProcess.value = curPrgress * 0.01f;
            m_StringBuilder.Clear();
            m_StringBuilder.AppendFormat("{0} %", curPrgress);
            m_LoadingProgressInfoText.text = m_DisplayInfo + m_StringBuilder.ToString();
            m_LoadingProgressStateText.text = m_DisplayState;
        }

        private void SetBackgrounImage()
        {
            if (mBgImg != null)
            {
                string imgPath = PluginManager.GetSDKLogoPath(SDKInterface.SDKLogoType.SelectRole);
                if (string.IsNullOrEmpty(imgPath))
                    return;
                ETCImageLoader.LoadSprite(ref mBgImg, imgPath);
            }
        }
        
        #endregion
    }
}

