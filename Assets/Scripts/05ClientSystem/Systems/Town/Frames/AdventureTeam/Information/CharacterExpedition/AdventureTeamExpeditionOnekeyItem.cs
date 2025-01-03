using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    public class AdventureTeamExpeditionOnekeyItem : MonoBehaviour
    {
        #region MODEL PARAM  

        private ExpeditionMapModel mapModel;
        private int[] roleJobIds;

        #endregion

        #region VIEW PARAMS

        [SerializeField] private Image mapImg;
        [SerializeField] private Text mapNameText;
        [SerializeField] private Text mapLevelText;
        [SerializeField] private ComUIListScript roleInfoView;
        [SerializeField] private CanvasGroup roleInfoCanvasGroup;
        [SerializeField] private ComUIListScript rewardInfoView;
        [SerializeField] private CanvasGroup rewardInfoCanvasGroup;

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
            if (roleInfoView != null)
            {
                if (roleInfoView.IsInitialised() == false)
                {
                    roleInfoView.Initialize();
                }
                roleInfoView.onBindItem += _OnRoleInfoItemBind;
                roleInfoView.onItemVisiable += _OnRoleInfoItemVisiable;
            }

            if (rewardInfoView != null)
            {
                if (rewardInfoView.IsInitialised() == false)
                {
                    rewardInfoView.Initialize();
                }
                rewardInfoView.onBindItem += _OnRewardItemBind;
                rewardInfoView.onItemVisiable += _OnRewardItemVisiable;
                rewardInfoView.OnItemUpdate += _OnRewardItemUpdate;
                rewardInfoView.OnItemRecycle += _OnRewardItemRecycle;
            }

            if (roleInfoCanvasGroup)
            {
                roleInfoCanvasGroup.blocksRaycasts = false;
                roleInfoCanvasGroup.interactable = false;
            }
            if (rewardInfoCanvasGroup)
            {
                rewardInfoCanvasGroup.blocksRaycasts = false;
                rewardInfoCanvasGroup.interactable = false;
            }
        }

        private void _ClearView()
        {
            if (roleInfoView != null)
            {
                roleInfoView.onBindItem -= _OnRoleInfoItemBind;
                roleInfoView.onItemVisiable -= _OnRoleInfoItemVisiable;
                roleInfoView.UnInitialize();
            }

            if (rewardInfoView != null)
            {
                rewardInfoView.onBindItem -= _OnRewardItemBind;
                rewardInfoView.onItemVisiable -= _OnRewardItemVisiable;
                rewardInfoView.OnItemUpdate -= _OnRewardItemUpdate;
                rewardInfoView.OnItemRecycle -= _OnRewardItemRecycle;
                rewardInfoView.UnInitialize();
            }

            ClearView();
        }

        private AdventureTeamExpedtionResultRoleInfo _OnRoleInfoItemBind(GameObject go)
        {
            if (go == null)
            {
                return null;
            }
            return go.GetComponent<AdventureTeamExpedtionResultRoleInfo>();
        }

        private void _OnRoleInfoItemVisiable(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }
            if (roleJobIds == null || roleJobIds.Length <= 0)
            {
                return;
            }
            int i_Index = item.m_index;
            if (i_Index < 0 || i_Index >= roleJobIds.Length)
            {
                return;
            }
            var roleInfo = item.gameObjectBindScript as AdventureTeamExpedtionResultRoleInfo;
            if (roleInfo != null)
            {
                roleInfo.RefreshView(roleJobIds[i_Index]);
            }            
        }

        private AdventureTeamExpeditionOnekeyRewardItem _OnRewardItemBind(GameObject go)
        {
            if (go == null)
            {
                return null;
            }
            return go.GetComponent<AdventureTeamExpeditionOnekeyRewardItem>();
        }

        private void _OnRewardItemVisiable(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }
            if (mapModel == null || mapModel.rewardList == null)
            {
                return;
            }
            int i_Index = item.m_index;
            if (i_Index < 0 || i_Index >= mapModel.rewardList.Count)
            {
                return;
            }
            var rewardItem = item.gameObjectBindScript as AdventureTeamExpeditionOnekeyRewardItem;
            if (rewardItem != null)
            {
                rewardItem.InitItemView(i_Index, mapModel, mapModel.rewardList[i_Index].rewardCondition);
                rewardItem.UpdateExpeditionMapBaseData();
                rewardItem.OnExpeditionRolesChanged();
            }
        }

        private void _OnRewardItemUpdate(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }
            var rewardItem = item.gameObjectBindScript as AdventureTeamExpeditionOnekeyRewardItem;
            if (rewardItem != null)
            {
                rewardItem.OnExpeditionTimeChanged();
            }
        }

        private void _OnRewardItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }
            var rewardItem = item.gameObjectBindScript as AdventureTeamExpeditionOnekeyRewardItem;
            if (rewardItem != null)
            {
                rewardItem.OnItemRecycle();
            }
        }

        private void _SetMapImg(string imgPath)
        {
            mapImg.SafeSetImage(imgPath);
        }

        private void _SetMapName(string mapName)
        {
            mapNameText.SafeSetText(mapName);
        }

        private void _SetMapLevel(string mapLevel)
        {
            mapLevelText.SafeSetText(TR.Value("adventure_team_expedition_dispatch_level", mapLevel));
        }

        #endregion

        #region  PUBLIC METHODS

        public void RefreshView(ExpeditionMapModel model)
        {
            if (model == null)
                return;
            this.mapModel = model;
            if (model.mapNetInfo != null && model.mapNetInfo.roles != null)
            {
                roleJobIds = new int[model.mapNetInfo.roles.Count];
                for (int i = 0; i < model.mapNetInfo.roles.Count; i++)
                {
                    var roleInfo = model.mapNetInfo.roles[i];
                    if (roleInfo == null)
                        continue;
                    roleJobIds[i] = (int)roleInfo.occu;
                }
            }

            _SetMapImg(model.mapImagePath);
            _SetMapName(model.mapName);
            _SetMapLevel(model.playerLevelLimit.ToString());

            if(roleInfoView != null && roleJobIds != null && roleJobIds.Length > 0)
            {
                roleInfoView.SetElementAmount(roleJobIds.Length);
            }
            if (rewardInfoView != null && model.rewardList != null && model.rewardList.Count > 0)
            {
                rewardInfoView.SetElementAmount(model.rewardList.Count);
            }
        }

        public void RefreshView()
        {
            if (rewardInfoView != null)
            {
                rewardInfoView.UpdateElement();
            }
        }

        public void ClearView()
        {
            mapModel = null;
            roleJobIds = null;
        }
        
        #endregion
    }
}
