using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class FashionActivityView : MonoBehaviour, IActivityView
    {
        [SerializeField] protected Image mImageBg;
        [SerializeField] protected Text mTextTime;

        private float mLastTime;
        protected ILimitTimeActivityModel mModel;
        public void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            if (model == null)
            {
                Logger.LogError("LimitTimeActivityModel data is empty");
                return;
            }

            mModel = model;
            mLastTime = Time.time;
            UpdateTime();
        }

        public void UpdateData(ILimitTimeActivityModel model)
        {
            mModel = model;
            UpdateTime();
        }

        public void Dispose()
        {

        }

        public void Close()
        {
            Dispose();
            Destroy(gameObject);
        }
        public void Show()
        {
        }
        public void Hide()
        {
        }

        void Update()
        {
            if (Time.time - mLastTime > 1)
            {
                UpdateTime();
                mLastTime = Time.time;
            }
        }

        protected virtual void UpdateTime()
        {
            int SumTime = (int)mLastTime - (int)TimeManager.GetInstance().GetServerTime();
            int TimeHour = SumTime / 60 / 60;
            int TimeMin = SumTime / 60 % 60;
            int TimeSecond = SumTime % 60;
            mTextTime.SafeSetText(string.Format(TR.Value("activity_fashion_time"), TimeHour, TimeMin, TimeSecond));
        }
    }
}
