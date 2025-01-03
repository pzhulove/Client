using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    public class AdventureTeamExpeditionOnekeyView : MonoBehaviour
    {
        #region MODEL PARAMS

        private List<ExpeditionMapModel> mReadyMapModels;
        private List<byte> mReadyMapTimeList;

        private uint mLastMaxLevelMapModelDurationTime;

        #endregion

        #region VIEW PARAMS

        [SerializeField] private ComUIListScript mapInfoView;
        [SerializeField] private ComUIListScript mapTimeToggleView;
        [SerializeField] private ToggleGroup mapTimeToggleGroup;

        [SerializeField] private Text resultDescText;
        [SerializeField] private Image resultRewardItemIcon;
        [SerializeField] private Text resultRewardItemCountDescText;
        
        #endregion
        
        #region PRIVATE METHODS
        
        //Unity life cycle
        void Awake()
        {
            _InitView();
        }       
        
        //Unity life cycle
        void OnDestroy () 
        {
            _ClearView();
        }

        private void _InitView()
        {
            if (mapInfoView != null)
            {
                if (mapInfoView.IsInitialised() == false)
                {
                    mapInfoView.Initialize();
                }
                mapInfoView.onBindItem += _OnMapItemBind;
                mapInfoView.onItemVisiable += _OnMapItemVisiable;
                mapInfoView.OnItemUpdate += _OnMapItemUpdate;
                mapInfoView.OnItemRecycle += _OnMapItemRecycle;
            }

            if (mapTimeToggleView != null)
            {
                if (mapTimeToggleView.IsInitialised() == false)
                {
                    mapTimeToggleView.Initialize();
                }
                mapTimeToggleView.onBindItem += _OnMapTimeToggleBind;
                mapTimeToggleView.onItemVisiable += _OnMapTimeToggleVisiable;
                mapTimeToggleView.OnItemRecycle += _OnMapTimeToggleRecycle;
            }
        }

        private void _ClearView()
        {
            if (mapInfoView != null)
            {
                mapInfoView.onBindItem -= _OnMapItemBind;
                mapInfoView.onItemVisiable -= _OnMapItemVisiable;
                mapInfoView.OnItemUpdate -= _OnMapItemUpdate;
                mapInfoView.OnItemRecycle -= _OnMapItemRecycle;
                mapInfoView.UnInitialize();
            }

            if (mapTimeToggleView != null)
            {
                mapTimeToggleView.onBindItem -= _OnMapTimeToggleBind;
                mapTimeToggleView.onItemVisiable -= _OnMapTimeToggleVisiable;
                mapTimeToggleView.OnItemRecycle -= _OnMapTimeToggleRecycle;
                mapTimeToggleView.UnInitialize();
            }

            if (mReadyMapModels != null)
            {
                mReadyMapModels.Clear();
            }
            if (mReadyMapTimeList != null)
            {
                mReadyMapTimeList.Clear();
            }
        }

        private AdventureTeamExpeditionOnekeyItem _OnMapItemBind(GameObject go)
        {
            if (go == null)
            {
                return null;
            }
            return go.GetComponent<AdventureTeamExpeditionOnekeyItem>();
        }

        private void _OnMapItemVisiable(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }
            if (mReadyMapModels == null || mReadyMapModels.Count <= 0)
            {
                return;
            }
            int i_Index = item.m_index;
            if (i_Index < 0 || i_Index >= mReadyMapModels.Count)
            {
                return;
            }
            var mapItem = item.gameObjectBindScript as AdventureTeamExpeditionOnekeyItem;
            if (mapItem != null)
            {
                mapItem.RefreshView(mReadyMapModels[i_Index]);
            }
        }

        private void _OnMapItemUpdate(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }
            var mapItem = item.gameObjectBindScript as AdventureTeamExpeditionOnekeyItem;
            if (mapItem != null)
            {
                mapItem.RefreshView();
            }
        }

        private void _OnMapItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }
            var mapItem = item.gameObjectBindScript as AdventureTeamExpeditionOnekeyItem;
            if (mapItem != null)
            {
                mapItem.ClearView();
            }
        }

        private AdventureTeamExpeditionTimeToggle _OnMapTimeToggleBind(GameObject go)
        {
            if (go == null)
            {
                return null;
            }
            return go.GetComponent<AdventureTeamExpeditionTimeToggle>();
        }

        private void _OnMapTimeToggleVisiable(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }
            if (mReadyMapTimeList == null || mReadyMapTimeList.Count <= 0)
            {
                return;
            }
            int i_Index = item.m_index;
            if (i_Index < 0 || i_Index >= mReadyMapTimeList.Count)
            {
                return;
            }
            var timeToggle = item.gameObjectBindScript as AdventureTeamExpeditionTimeToggle;
            if (timeToggle != null)
            {
                timeToggle.InitItemView(mReadyMapTimeList[i_Index],true, true);
                if (mReadyMapTimeList[i_Index] == mLastMaxLevelMapModelDurationTime)
                {
                    timeToggle.ChangeToggleState(true);
                }
                else
                {
                    timeToggle.ChangeToggleState(false);
                }
                timeToggle.UpdateItemInfo();
            }
        }

        private void _OnMapTimeToggleRecycle(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }
            var timeToggle = item.gameObjectBindScript as AdventureTeamExpeditionTimeToggle;
            if (timeToggle != null)
            {
                timeToggle.OnItemRecycle();
            }
        }

        private void _SetResultDescText(string mapCount)
        {
            resultDescText.SafeSetText(TR.Value("adventure_team_expeidtion_dispatch_ready", mapCount.ToString()));
        }

        private void _SetRewardItemIcon(string imgPath)
        {
            resultRewardItemIcon.SafeSetImage(imgPath);
        }

        private void _SetRewardItemCount(string itemCount)
        {
            resultRewardItemCountDescText.SafeSetText(TR.Value("adventure_team_expeidtion_dispatch_reward_count",itemCount));
        }

        private void _SetRewardInfo(bool bSetItemIcon)
        {
            if (this.mReadyMapModels == null || this.mReadyMapModels.Count <= 0)
            {
                return;
            }
            int totalRewardCount = 0;
            if (bSetItemIcon)
            {
                totalRewardCount = AdventureTeamDataManager.GetInstance().GetExpeditionRewardItemTotalCount(this.mReadyMapModels, _SetRewardItemIcon);
            }
            else
            {
                totalRewardCount = AdventureTeamDataManager.GetInstance().GetExpeditionRewardItemTotalCount(this.mReadyMapModels);
            }
            _SetRewardItemCount(totalRewardCount.ToString());
        }

        #endregion

        #region  PUBLIC METHODS

        public void InitView(List<ExpeditionMapModel> readyMapModels)
        {
            if (readyMapModels == null || readyMapModels.Count <= 0)
            {
                return;
            }
            this.mReadyMapModels = readyMapModels;
            this.mReadyMapTimeList = AdventureTeamDataManager.GetInstance().GetExpeditionTimeList(readyMapModels[0]);
            this.mLastMaxLevelMapModelDurationTime = AdventureTeamDataManager.GetInstance().GetLastExpeditionMaxMapDurationTime(this.mReadyMapModels);

            _SetResultDescText(readyMapModels.Count.ToString());

            _SetRewardInfo(true);

            if (mapInfoView != null)
            {
                mapInfoView.SetElementAmount(readyMapModels.Count);
            }

            if (mapTimeToggleView != null && mReadyMapTimeList != null)
            {
                mapTimeToggleView.SetElementAmount(mReadyMapTimeList.Count);
            }
        }

        public void RefreshView()
        {
            if (mapInfoView != null)
            {
                mapInfoView.UpdateElement();
            }

            _SetRewardInfo(false);
        }
        
        #endregion
    }
}
