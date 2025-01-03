using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.UI;
using Protocol;

namespace GameClient
{
    public class OPPOGrowthHaoLiView : MonoBehaviour
    {
        [SerializeField]private ComUIListScript mOppoGrowthHaoLiUIList;
        [SerializeField]private int iOPPOGrowthHaoLiActivityId = 26000;

        private bool bInitOppoGrowthHaoLiUIList = false;
        private List<ActiveManager.ActivityData> mActivityDataList;
        private void Awake()
        {
            if (bInitOppoGrowthHaoLiUIList == false)
            {
                if (mOppoGrowthHaoLiUIList != null)
                {
                    mOppoGrowthHaoLiUIList.Initialize();
                    mOppoGrowthHaoLiUIList.onBindItem += OnBindItemDelegate;
                    mOppoGrowthHaoLiUIList.onItemVisiable += OnItemVisiableDelegate;
                }

                bInitOppoGrowthHaoLiUIList = true;
            }
        }

        private void OnDestroy()
        {
            if (mOppoGrowthHaoLiUIList != null)
            {
                mOppoGrowthHaoLiUIList.onBindItem -= OnBindItemDelegate;
                mOppoGrowthHaoLiUIList.onItemVisiable -= OnItemVisiableDelegate;
            }

            bInitOppoGrowthHaoLiUIList = false;
            mActivityDataList.Clear();
        }

        public void InitView()
        {
            mActivityDataList = new List<ActiveManager.ActivityData>();
            UpdateElementAmount();
        }

        public void UpdateElementAmount()
        {
            mActivityDataList.Clear();

            ActiveManager.ActiveData activeData = ActiveManager.GetInstance().GetActiveData(iOPPOGrowthHaoLiActivityId);
            if (activeData != null)
            {
                mActivityDataList.AddRange(activeData.akChildItems);
            }

            SortActivityDataList();

            if (mOppoGrowthHaoLiUIList != null)
            {
                mOppoGrowthHaoLiUIList.SetElementAmount(mActivityDataList.Count);
            }
        }

        private OPPOGrowthHaoLiItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<OPPOGrowthHaoLiItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            OPPOGrowthHaoLiItem mOPPOGrowthHaoLi = item.gameObjectBindScript as OPPOGrowthHaoLiItem;
            if (mOPPOGrowthHaoLi != null && item.m_index >= 0 && item.m_index < mActivityDataList.Count)
            {
                mOPPOGrowthHaoLi.OnItemVisiable(mActivityDataList[item.m_index], OnSendSubmitActivity);
            }
        }

        private void SortActivityDataList()
        {
            List<ActiveManager.ActivityData> mCanReceiveList = new List<ActiveManager.ActivityData>();//可领取
            List<ActiveManager.ActivityData> mNoReceiveList = new List<ActiveManager.ActivityData>(); //不可领取
            List<ActiveManager.ActivityData> mReceivedList = new List<ActiveManager.ActivityData>(); //已领取

            for (int i = 0; i < mActivityDataList.Count; i++)
            {
                ActiveManager.ActivityData data = mActivityDataList[i];
                switch ((TaskStatus)data.status)
                {
                    case TaskStatus.TASK_INIT:
                    case TaskStatus.TASK_UNFINISH:
                        mNoReceiveList.Add(data);
                        break;
                    case TaskStatus.TASK_FINISHED:
                        mCanReceiveList.Add(data);
                        break;
                    case TaskStatus.TASK_SUBMITTING:
                    case TaskStatus.TASK_OVER:
                        mReceivedList.Add(data);
                        break;
                    default:
                        break;
                }
            }

            mActivityDataList.Clear();
            mActivityDataList.AddRange(mCanReceiveList);
            mActivityDataList.AddRange(mNoReceiveList);
            mActivityDataList.AddRange(mReceivedList);
        }

        private void OnSendSubmitActivity(int iActivityId)
        {
            if (iActivityId == 0)
            {
                return;
            }

            ActiveManager.GetInstance().SendSubmitActivity(iActivityId);
        }
    }
}
