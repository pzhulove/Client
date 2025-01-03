using Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class OnlineActiveView : MonoBehaviour
    {
        [SerializeField] private ComUIListScript mComUIList;
        [SerializeField] private OnlineActiveAwardItem[] mAwards;
        [SerializeField] private TextEx mTextDays;

        private ActiveManager.ActiveData mActiveData;
        private ActiveManager.ActiveData mCumulativeDaysActiveData;
        private int[] mActiveTableID = new int[] { 2202, 2203, 2204, 2205, 2206, 2207, 2208 };
        private int[] mCumulativeDaysActiveTableID = new int[] { 27001, 27002, 27003};
        private int mCurIndex = -1;
        private const string mKey = "DayOnline";
        private const int mTemplateId = 5000;
        private const int mCumulativeDaysTemplateId = 5100;
        private ActiveManager.ActiveMainUpdateKey mFindKey;
        private float mTime;
        private int mIAccumulatedTime = 0;
        private Double mDRecievedTime = 0;
        private bool mInited = false;

        private void _InitFindKey()
        {
            if (mActiveData == null)
            {
                return;
            }

            mFindKey = null;
            for (int i = 0; i < mActiveData.updateMainKeys.Count; ++i)
            {
                if (mActiveData.updateMainKeys[i].key == mKey)
                {
                    mFindKey = mActiveData.updateMainKeys[i];
                    break;
                }
            }

            mDRecievedTime = mFindKey.fRecievedTime;
            mIAccumulatedTime = ActiveManager.GetInstance().GetTemplateUpdateValue(mTemplateId, mFindKey.key);
        }

        private void _Init()
        {
            mActiveData = ActiveManager.GetInstance().GetActiveData(mTemplateId);
            _InitAwards();
            _InitFindKey();
        }

        private void _UpdateFindKey()
        {
            mActiveData = ActiveManager.GetInstance().GetActiveData(mTemplateId);
            _InitFindKey();
        }

        private void _InitCumulativeDays()
        {
            mCumulativeDaysActiveData = ActiveManager.GetInstance().GetActiveData(mCumulativeDaysTemplateId);
            _InitDays();
            _InitCumulativeDaysAwards();
        }

        private void _InitDays()
        {
            if (mCumulativeDaysActiveData == null || mCumulativeDaysActiveData.akChildItems == null || mCumulativeDaysActiveData.akChildItems.Count <= 0)
            {
                return;
            }

            if (mCumulativeDaysActiveData.akChildItems[mCumulativeDaysActiveData.akChildItems.Count - 1].akActivityValues == null 
                || mCumulativeDaysActiveData.akChildItems[mCumulativeDaysActiveData.akChildItems.Count - 1].akActivityValues.Count <= 0)
            {
                return;
            }

            int totalDays = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            string nowDaysStr = mCumulativeDaysActiveData.akChildItems[mCumulativeDaysActiveData.akChildItems.Count - 1].akActivityValues[0].value;
            mTextDays.SafeSetText(string.Format(TR.Value("online_active_days_format"), nowDaysStr, totalDays));
        }

        private void _InitCumulativeDaysAwards()
        {
            if (mComUIList != null && mCumulativeDaysActiveData != null && mCumulativeDaysActiveData.akChildItems != null)
            {
                mComUIList.SetElementAmount(mCumulativeDaysActiveData.akChildItems.Count);
            }
        }

        private void _InitAwards()
        {
            if (mAwards == null || mActiveData == null || mActiveData.akChildItems == null)
            {
                return;
            }

            mCurIndex = -1;
            for (int i = 0; i < mAwards.Length; i++)
            {
                if (mAwards[i] == null)
                {
                    continue;
                }

                if (mActiveData.akChildItems.Count <= i)
                {
                    break;
                }

                ActiveManager.ActivityData activityData = mActiveData.akChildItems[i];

                if ((Protocol.TaskStatus)activityData.status == Protocol.TaskStatus.TASK_UNFINISH && mCurIndex == -1)
                {
                    mCurIndex = i;
                }

                int index = i;
                mAwards[i].Init(_GetItemData(activityData), activityData, () => { _GetAward(index); }, mCurIndex == i);
            }
        }

        private ItemData _GetItemData(ActiveManager.ActivityData data)
        {
            Dictionary<uint, int> dicAwards = data.GetAwards();
            ItemData itemData = null;
            if (dicAwards != null)
            {
                foreach (uint key in dicAwards.Keys)
                {
                    itemData = ItemDataManager.CreateItemDataFromTable((int)key);
                    itemData.Count = dicAwards[key];
                    break;
                }
            }

            return itemData;
        }

        private int _GetActiveID(int index, bool isCumulativeDays = false)
        {
            if (isCumulativeDays)
            {
                if (index >= mCumulativeDaysActiveTableID.Length)
                {
                    return -1;
                }

                return mCumulativeDaysActiveTableID[index];
            }
            else
            {
                if (index >= mActiveTableID.Length)
                {
                    return -1;
                }

                return mActiveTableID[index];
            }
        }

        private void _GetAward(int index, bool isCumulativeDays = false)
        {
            int id = _GetActiveID(index, isCumulativeDays);

            if (id >= 0)
            {
                ActiveManager.GetInstance().SendSubmitActivity(id);
            }
        }

        private void OnActivityUpdate(ActiveManager.ActivityData data, ActiveManager.ActivityUpdateType EActivityUpdateType)
        {
            if (data.activeItem.TemplateID == mTemplateId)
            {
                _Init();
            }

            if (data.activeItem.TemplateID == mCumulativeDaysTemplateId)
            {
                _InitCumulativeDays();
            }
        }

        private void OnUpdateMainActivity(ActiveManager.ActiveData data)
        {
            if (data.mainItem.ID == mTemplateId)
            {
                _UpdateFindKey();
            }
        }

        private void _OnItemUpdate(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            if (mCumulativeDaysActiveData == null)
            {
                return;
            }

            if (item.m_index >= mCumulativeDaysActiveData.akChildItems.Count)
            {
                return;
            }

            ActiveManager.ActivityData activityData = mCumulativeDaysActiveData.akChildItems[item.m_index];

            var script = item.GetComponent<OnlineActiveCumulativeDaysAwardItem>();
            if (script != null)
            {
                script.Init(_GetItemData(activityData), activityData, () => { _GetAward(item.m_index, true); });
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            mTime = 0;
            ActiveManager.GetInstance().onActivityUpdate += OnActivityUpdate;
            ActiveManager.GetInstance().onUpdateMainActivity += OnUpdateMainActivity;

            if (mComUIList != null && !mInited)
            {
                mComUIList.Initialize();
                mComUIList.OnItemUpdate = _OnItemUpdate;
                mComUIList.onItemVisiable = _OnItemUpdate;

                mInited = true;
            }

            _Init();
            _InitCumulativeDays();
        }

        // Update is called once per frame
        void Update()
        {
            if (mCurIndex < 0)
            {
                return;
            }

            if (Time.time - mTime <= 1)
            {
                return;
            }
            mTime = Time.time;

            double dPassedTime = TimeManager.GetInstance().GetServerTime() - mDRecievedTime;
            double time = 0;
            double.TryParse(mActiveData.akChildItems[mCurIndex].activeItem.Param0, out time);
            time = time * 60 - dPassedTime - mIAccumulatedTime;

            if (time < 0)
            {
                time = 0;
            }

            string leftTimeStr = Function.GetLastsTimeStr(time, true);
            mAwards[mCurIndex].UpdateTimeText(leftTimeStr);
        }

        private void OnDestroy()
        {
            ActiveManager.GetInstance().onActivityUpdate -= OnActivityUpdate;
            ActiveManager.GetInstance().onUpdateMainActivity -= OnUpdateMainActivity;
        }
    }
}
