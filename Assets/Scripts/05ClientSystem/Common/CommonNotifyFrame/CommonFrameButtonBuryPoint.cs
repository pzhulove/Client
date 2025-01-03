using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class CommonFrameButtonBuryPoint : MonoBehaviour
    {
        [Header("界面名")]
        [SerializeField]
        private string mFrameName;

        [Header("button名字 或者 toggle名字")]
        [SerializeField]
        private string mName;

        [Header("开关 true 上报 false 不上报")]
        [SerializeField]
        private bool mSwich = true;

        public string FrameName
        {
            get { return mFrameName; }
            set { mFrameName = value; }
        }

        public string ButtonName
        {
            get { return mName; }
            set { mName = value; }
        }

        public bool Swich
        {
            get { return mSwich; }
            set { mSwich = value; }
        }
        /// <summary>
        /// 上报埋点
        /// </summary>
        public void OnSendBuryingPoint()
        {
            if (mFrameName != "" && mName != "" && mSwich)
            {
                var toggle = this.GetComponent<Toggle>();
                if (toggle != null)
                {
                    if (toggle.isOn == true)
                    {
                        DoStartFrameOperation();
                    }
                }
                else
                {
                    DoStartFrameOperation();
                }
            }
        }

        private void DoStartFrameOperation()
        {
            string sCurrentTime = Function.GetDateTime((int)TimeManager.GetInstance().GetServerTime());
            //Logger.LogErrorFormat(string.Format("FrameName|{0} ButtonName|{1} Time|{2}", mFrameName, mName, sCurrentTime));
            GameStatisticManager.GetInstance().DoStartFrameOperation(mFrameName, mName, sCurrentTime);
        }
    }
}

