using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Scripts.UI;
using Protocol;

namespace GameClient
{
    public class AmberPrivilegeView : MonoBehaviour
    {
        [SerializeField]private Text mCureentAmberLevelDesc;
        [SerializeField]private ComUIListScript mAmberPrivilegeUIList;
        [SerializeField]private int iAmberPreivilegeActivityId = 27000;
        private bool InitAmberPrivilegeComUIList = false;
        private List<ActiveManager.ActivityData> mActivityDataList;
        private void Awake()
        {
            if (InitAmberPrivilegeComUIList == false)
            {
                if (mAmberPrivilegeUIList != null)
                {
                    mAmberPrivilegeUIList.Initialize();
                    mAmberPrivilegeUIList.onBindItem += OnBindItemDelegate;
                    mAmberPrivilegeUIList.onItemVisiable += OnItemVisiableDelegate;
                }
              
                InitAmberPrivilegeComUIList = true;
            }
        }

        private void OnDestroy()
        {
            if (mAmberPrivilegeUIList != null)
            {
                mAmberPrivilegeUIList.onBindItem -= OnBindItemDelegate;
                mAmberPrivilegeUIList.onItemVisiable -= OnItemVisiableDelegate;
            }

            InitAmberPrivilegeComUIList = false;
            mActivityDataList.Clear();
        }
        
        public void InitView()
        {
            mActivityDataList = new List<ActiveManager.ActivityData>();

            if (mCureentAmberLevelDesc != null)
            {
                mCureentAmberLevelDesc.text = OPPOPrivilegeDataManager.GetInstance().GetAmberLevel();
            }

            UpdateElementAmount();
        }

        public void UpdateElementAmount()
        {
            mActivityDataList.Clear();

            ActiveManager.ActiveData activeData = ActiveManager.GetInstance().GetActiveData(iAmberPreivilegeActivityId);
            if (activeData != null)
            {
                mActivityDataList.AddRange(activeData.akChildItems);
            }

            SortActivityDataList();

            if (mAmberPrivilegeUIList != null)
            {
                mAmberPrivilegeUIList.SetElementAmount(mActivityDataList.Count);
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

        private AmberPrivilegeItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<AmberPrivilegeItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            AmberPrivilegeItem mAmberPrivilegeItem = item.gameObjectBindScript as AmberPrivilegeItem;
            if (mAmberPrivilegeItem != null && item.m_index >= 0 && item.m_index < mActivityDataList.Count)
            {
                mAmberPrivilegeItem.OnItemVisiable(mActivityDataList[item.m_index], OnSendSubmitActivity);
            }
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

