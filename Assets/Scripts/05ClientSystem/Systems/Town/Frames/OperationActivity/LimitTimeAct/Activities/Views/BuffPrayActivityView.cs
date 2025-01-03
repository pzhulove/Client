using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Protocol;

namespace GameClient
{
    public class BuffPrayActivityView : MonoBehaviour, IActivityView
    {  
        #region Model Params
        
        #endregion
        
        #region View Params

        [SerializeField]
        private Text m_BuffPrayTitle;
        [SerializeField]
        private Text m_BuffPrayName;
        [SerializeField]
        private Text m_BuffPrayDesc;
        [SerializeField]
        private SimpleTimer m_BuffPrayTimer;
        [SerializeField]private GameObject m_BuffEffEctRoot;
        [SerializeField]
        private Text mTimer;
        
        #endregion
        
        #region PRIVATE METHODS
        
        //Unity life cycle
        void OnDestroy () 
        {
            if (m_BuffPrayTimer != null)
            {
                m_BuffPrayTimer.StopTimer();
            }
        }
        
        void UpdateInfo(ILimitTimeActivityModel model)
        {
            for (int i = 0; i < model.TaskDatas.Count; i++)
            {
                var mTaskData = model.TaskDatas[i];
                if (mTaskData.State != OpActTaskState.OATS_FINISHED)
                {
                    continue;
                }

                if (m_BuffPrayName != null)
                {
                    m_BuffPrayName.text = mTaskData.taskName;
                }

                if (m_BuffPrayDesc != null)
                {
                    m_BuffPrayDesc.text = mTaskData.Desc;
                }

                mTimer.SafeSetText(string.Format(TR.Value("activity_qi_xi_que_qiao_time"),
                Function.GetTimeWithoutYearNoZero((int)model.StartTime, (int)model.EndTime)));

                if (m_BuffPrayTimer != null)
                {
                    int time = (int)mTaskData.DoneNum - (int)TimeManager.GetInstance().GetServerTime();
                    if (time > 0)
                    {
                        m_BuffPrayTimer.SetCountdown(time);
                        m_BuffPrayTimer.StartTimer();
                    }
                }

                if (mTaskData.ParamNums.Count > 0)
                {
                    int mEffectIndex = (int)mTaskData.ParamNums[0];
                    SetEffects(Utility.GetBuffPrayActivityEffectPath(mEffectIndex));
                }
            }
        }

        #endregion
        
        #region  PUBLIC METHODS
     
        public void Close()
        {
            Destroy(gameObject);
        }

        public void Dispose()
        {
            
        }

        public void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            UpdateInfo(model);
        }

        public void UpdateData(ILimitTimeActivityModel data)
        {
            UpdateInfo(data);
        }

        public void Show()
        {
            
        }

        public void Hide()
        {
            
        }

        public void SetBuyCallBack(FashionTicketBuyActivityView.BuyCallBack buyFashion)
        {
            
        }

        public void SetEffects(string sEffectPath)
        {
            if (sEffectPath != "")
            {
                GameObject mEffectGo = AssetLoader.GetInstance().LoadResAsGameObject(sEffectPath);
                Utility.AttachTo(mEffectGo, m_BuffEffEctRoot);
                mEffectGo.CustomActive(true);
            }
        }

        #endregion
    }
}