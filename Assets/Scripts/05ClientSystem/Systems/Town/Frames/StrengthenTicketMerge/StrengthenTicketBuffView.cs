using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class StrengthenTicketBuffView : MonoBehaviour
    {
        public delegate void onGoToOtherTab(StrengthenTicketMergeType type);
        [SerializeField] private Text[] mDestxtArry = new Text[4];
        // [SerializeField] private Text mTimeTxt; 
        [SerializeField] private GameObject mEffectRoot;
        [SerializeField] private SimpleTimer mTimer;
        private OpActivityData mData;
        private onGoToOtherTab mOnGoToOtherTab = null;

        public void OnInit(onGoToOtherTab onGoToOtherEvent)
        {
            mData = StrengthenTicketMergeDataManager.GetInstance().GetCurBuffPrayActivityData();
            if (null == mData)
            {
                Logger.LogError("没有开放中的强化祈福活动");
                return;
            }
            mOnGoToOtherTab = onGoToOtherEvent;
            _ShowInfo();
        }

        //初始化信息界面
        private void _ShowInfo()
        {
            string[] taskDesc = mData.taskDesc.Split('|');
            for (int index = 0; index < mData.tasks.Length; index++)
            {
                var taskData = mData.tasks[index];
                if (mDestxtArry[index] != null && taskDesc.Length > index)
                {
                    mDestxtArry[index].text = taskDesc[index];
                }
            }
            if (mTimer != null)
            {
                mTimer.SetCountdown((int)mData.endTime - (int)TimeManager.GetInstance().GetServerTime());
                mTimer.StartTimer();
            }
            // if(mEffectRoot!=null)
            // {
            //     Utility.ClearChild(mEffectRoot);
            //     SetEffects(Utility.GetBuffPrayActivityEffectPath(0));
            // }
        }
        // public void SetEffects(string sEffectPath)
        // {
        //     if (sEffectPath != "")
        //     {
        //         GameObject mEffectGo = AssetLoader.GetInstance().LoadResAsGameObject(sEffectPath);
        //         Utility.AttachTo(mEffectGo, mEffectRoot);
        //         mEffectGo.CustomActive(true);
        //     }
        // }

        //点击前往合成
        public void OnClickGoToCompound()
        {
            if (null != mOnGoToOtherTab)
                mOnGoToOtherTab(StrengthenTicketMergeType.Material);
        }

        //点击前往关卡
        public void OnClickGoToDungeon()
        {
            ActivityDungeonFrame.OpenLinkFrame("2003000|2");
            ClientSystemManager.GetInstance().CloseFrame<StrengthenTicketMergeFrame>();
        }
    }
}
