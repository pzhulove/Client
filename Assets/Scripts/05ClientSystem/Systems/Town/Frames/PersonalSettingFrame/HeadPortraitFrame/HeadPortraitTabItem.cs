using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public delegate void OnHeadPortraitTabItemClick(HeadPortraitTabDataModel data);
    public class HeadPortraitTabItem : MonoBehaviour
    {
        [SerializeField]private Toggle mToggle;
        [SerializeField]private Text mName;
        [SerializeField]private GameObject mRedPoint;

        private void Awake()
        {
            if (mToggle != null)
            {
                mToggle.onValueChanged.RemoveAllListeners();
                mToggle.onValueChanged.AddListener(OnTabItemClick);
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.HeadPortraitItemStateChanged, OnHeadPortraitItemStateChanged);
        }

        private HeadPortraitTabDataModel mTabData;
        private OnHeadPortraitTabItemClick mOnHeadPortraitTabItemClick;
        private bool bIsSelect = false;
        public void InitTabItem(HeadPortraitTabDataModel data, OnHeadPortraitTabItemClick onHeadPortraitTabItemClick,bool isSelect)
        {
            mTabData = data;
            mOnHeadPortraitTabItemClick = onHeadPortraitTabItemClick;

            mName.text = mTabData.tabName;

            UpdateRedPoint();

            if (isSelect == true)
            {
                if (mToggle != null)
                {
                    mToggle.isOn = true;
                }
            }
        }

        private void UpdateRedPoint()
        {
            if (mTabData != null)
            {
                if (mRedPoint != null)
                {
                    mRedPoint.CustomActive(HeadPortraitFrameDataManager.IsHeadPortraitItemHasNew(mTabData.tabType));
                }
            }
        }

        private void OnTabItemClick(bool value)
        {
            if (mTabData == null) return;

            if (bIsSelect == value)
            {
                return;
            }

            bIsSelect = value;

            if (value == true)
            {
                if (mOnHeadPortraitTabItemClick != null)
                {
                    mOnHeadPortraitTabItemClick.Invoke(mTabData);
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.HeadPortraitItemStateChanged);
            }
        }

        private void OnHeadPortraitItemStateChanged(UIEvent uiEvent)
        {
            UpdateRedPoint();
        }

        private void OnDestroy()
        {
            if (mToggle != null)
            {
                mToggle.onValueChanged.RemoveListener(OnTabItemClick);
            }

            mTabData = null;
            mOnHeadPortraitTabItemClick = null;
            bIsSelect = false;

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.HeadPortraitItemStateChanged, OnHeadPortraitItemStateChanged);
        }
    }
}

