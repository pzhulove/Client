using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    public enum PersonalSettingMainTabType
    {
        PSMTT_NONE,
        PSMTT_TITLE, //头衔
        PSMTT_HEADPORTRAIT, // 头像框
    }

    [Serializable]
    public class PersonalSettingMainTabDataModle
    {
        public int mIndex;
        public PersonalSettingMainTabType mPersonalSettingMainTabType;
        public string mTabName;
        public GameObject mContentRoot;
    }


    public class PersonalSettingView : MonoBehaviour,IDisposable
    {
        [SerializeField]private List<PersonalSettingMainTabDataModle> mMainTabList;
        [SerializeField]private Button mCloseBtn;
        [SerializeField]private ComUIListScript mMainTabComUIList;

        
        private void Awake()
        {
            if (mCloseBtn != null)
            {
                mCloseBtn.onClick.RemoveAllListeners();
                mCloseBtn.onClick.AddListener(() =>
                {
                    ClientSystemManager.GetInstance().CloseFrame<PersonalSettingFrame>();
                });
            }
        }

        public void InitView()
        {
            InitMainTabComUIList();
        }

        private void InitMainTabComUIList()
        {
            if (mMainTabComUIList != null)
            {
                mMainTabComUIList.Initialize();
                mMainTabComUIList.onBindItem += OnBindItemDelegate;
                mMainTabComUIList.onItemVisiable += OnItemVisiableDelegate;
            }

            mMainTabComUIList.SetElementAmount(mMainTabList.Count);
        }

        private void UnInitMainTabComUIList()
        {
            if (mMainTabComUIList != null)
            {
                mMainTabComUIList.onBindItem -= OnBindItemDelegate;
                mMainTabComUIList.onItemVisiable -= OnItemVisiableDelegate;
            }
            mMainTabComUIList = null;
        }

        private PersonalSettingMainTabItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<PersonalSettingMainTabItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var tabItem = item.gameObjectBindScript as PersonalSettingMainTabItem;
            if (tabItem == null) return;

            if (item.m_index >= 0 && item.m_index < mMainTabList.Count)
            {
                if (PersonalSettingFrame.mDefalutTabIndex == mMainTabList[item.m_index].mIndex)
                {
                    tabItem.InitTabItem(mMainTabList[item.m_index], true);
                }
                else
                {
                    tabItem.InitTabItem(mMainTabList[item.m_index], false);
                }
            }
        }

        public void Dispose()
        {
            UnInitMainTabComUIList();
            if (mCloseBtn != null)
            {
                mCloseBtn.onClick.RemoveAllListeners();
            }
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}

