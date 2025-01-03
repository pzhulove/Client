using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class PersonalSettingMainTabItem : MonoBehaviour,IDisposable
    {
        [SerializeField]private Toggle mToggle;
        [SerializeField]private Text mTabName;

        private void Awake()
        {
            if (mToggle != null)
            {
                mToggle.onValueChanged.RemoveAllListeners();
                mToggle.onValueChanged.AddListener(OnTabToggleClick);
            }
        }

        private PersonalSettingMainTabDataModle mPersonalSettingMainTabDataModle;
        private bool IsSelect = false;
        private IClientFrame frame = null;

        public void InitTabItem(PersonalSettingMainTabDataModle mainTabDataModle,bool isSelect)
        {
            mPersonalSettingMainTabDataModle = mainTabDataModle;

            mTabName.text = mPersonalSettingMainTabDataModle.mTabName;

            if (isSelect == true)
            {
                if (mToggle != null)
                {
                    mToggle.isOn = true;
                }
            }
        }

        private void OnTabToggleClick(bool value)
        {
            if (mPersonalSettingMainTabDataModle == null) return;

            if (value == IsSelect)
            {
                return;
            }

            IsSelect = value;

            if (value == true)
            {
                mPersonalSettingMainTabDataModle.mContentRoot.CustomActive(true);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TitleModeUpdate);
                if (frame == null)
                {
                    if (mPersonalSettingMainTabDataModle.mPersonalSettingMainTabType == PersonalSettingMainTabType.PSMTT_TITLE)
                    {
                        frame = ClientSystemManager.GetInstance().OpenFrame<TitleFrame>(mPersonalSettingMainTabDataModle.mContentRoot);
                        
                    }
                    else if (mPersonalSettingMainTabDataModle.mPersonalSettingMainTabType == PersonalSettingMainTabType.PSMTT_HEADPORTRAIT)
                    {
                        frame = ClientSystemManager.GetInstance().OpenFrame<HeadPortraitFrame>(mPersonalSettingMainTabDataModle.mContentRoot);
                    }
                }
            }
            else
            {
                mPersonalSettingMainTabDataModle.mContentRoot.CustomActive(false);
            }
        }
        
        public void Dispose()
        {
            mPersonalSettingMainTabDataModle = null;
            IsSelect = false;
            if (frame != null)
            {
                frame.Close();
                frame = null;
            }
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}

