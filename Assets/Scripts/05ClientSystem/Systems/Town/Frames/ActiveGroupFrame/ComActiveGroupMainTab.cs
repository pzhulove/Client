using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComActiveGroupMainTab : MonoBehaviour
    {
        const string mKeyTEnable = "t_enable";
        const string mKeyTDisable = "t_disable";
        const string mKeySEnable = "s_enable";
        const string mKeySDisable = "s_disable";
        const string mKeyTEnableOpen = "t_enable_open";
        const string mKeyTDisableOpen = "t_disable_open";
        const string mKeySEnableOpen = "s_enable_open";
        const string mKeySDisableOpen = "s_disable_open";

        public StateController comState;
        public Text[] mLabels = new Text[0];
        public Slider mSlider;
        public Toggle toggle;
        public ComAchievementTabReadPointBinder redBinder;
        bool mSelected = false;

        [HideInInspector]
        public ProtoTable.AchievementGroupMainItemTable mainItem;
        [HideInInspector]
        public GameObject expandParent;

        public void SetBinderID(int mainId,int subId)
        {
            if(null != redBinder)
            {
                redBinder.SetId(mainId, subId);
            }
        }

        public void OnValueChanged(bool bOn)
        {
            if(null != toggle)
            {
                toggle.onValueChanged.RemoveListener(OnValueChanged);
                toggle.isOn = bOn;
                toggle.onValueChanged.AddListener(OnValueChanged);
            }

            bool bRepeat = false;
            if(bOn)
            {
                if(!mSelected)
                {
                    _OnSelected();
                    mSelected = true;
                }
                else
                {
                    bRepeat = true;
                    _OnRepeatSelected();
                    mSelected = false;
                }
            }
            else
            {
                mSelected = false;
            }

            if (null != comState)
            {
                if (null != mainItem && mainItem.ChildTabs.Count == 1)
                {
                    comState.Key = bOn ? mKeyTEnable : mKeyTDisable;
                }
                else
                {
                    if(!bRepeat)
                    {
                        comState.Key = bOn ? mKeySEnableOpen : mKeySDisable;
                    }
                    else
                    {
                        comState.Key = bOn ? mKeySEnable : mKeySDisable;
                    }
                }
            }
        }

        void _OnSelected()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAchievementGroupSubTabChanged, mainItem, expandParent);
        }

        void _OnRepeatSelected()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAchievementGroupSubTabChangedRepeated, mainItem, expandParent);
        }

        public void UpdateItemValue()
        {
            int iPre = AchievementGroupDataManager.GetInstance().GetSubItemsAValue(mainItem,null,true);
            int iAft = AchievementGroupDataManager.GetInstance().GetSubItemsAValue(mainItem, null, false);
            float fRadio = 0.0f;
            if (iAft > 0)
            {
                fRadio = Mathf.Clamp01(1.0f * iPre / iAft);
            }
            if(null != mSlider)
            {
                mSlider.value = fRadio;
            }
            string desc = string.Empty;
            if (null != mainItem && !string.IsNullOrEmpty(mainItem.Name))
            {
                desc = string.Format(mainItem.Name, iPre, iAft);
            }
            for (int i = 0; i < mLabels.Length; ++i)
            {
                if (null != mLabels[i])
                {
                    mLabels[i].text = desc;
                }
            }
        }

        void Awake()
        {
            MissionManager.GetInstance().onAddNewMission += _OnAddNewMission;
            MissionManager.GetInstance().onUpdateMission += _OnUpdateMission;
            MissionManager.GetInstance().onDeleteMission += _OnDeleteMission;
        }

        void _OnAddNewMission(uint taskID)
        {
            UpdateItemValue();
        }

        void _OnUpdateMission(uint taskID)
        {
            UpdateItemValue();
        }

        void _OnDeleteMission(uint taskID)
        {
            UpdateItemValue();
        }

        void OnDestroy()
        {
            if(null != toggle)
            {
                toggle.onValueChanged.RemoveListener(OnValueChanged);
                toggle = null;
            }

            MissionManager.GetInstance().onAddNewMission -= _OnAddNewMission;
            MissionManager.GetInstance().onUpdateMission -= _OnUpdateMission;
            MissionManager.GetInstance().onDeleteMission -= _OnDeleteMission;
        }
    }
}