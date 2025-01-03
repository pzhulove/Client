using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    public class AdventureTeamExpeditionResultItem : MonoBehaviour
    {
        #region MODEL PARAM  

        private ExpeditionMapModel mapModel;
        private int[] roleJobIds;

        private ComItem awardItem;

        #endregion

        #region VIEW PARAMS

        [SerializeField] private Image mapImg;
        [SerializeField] private Text mapNameText;
        [SerializeField] private Text mapLevelText;
        [SerializeField] private ComUIListScript roleInfoView;
        [SerializeField] private CanvasGroup roleInfoCanvasGroup;
        [SerializeField] private GameObject awardItemRoot;
        
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

            if (roleInfoCanvasGroup)
            {
                roleInfoCanvasGroup.blocksRaycasts = false;
                roleInfoCanvasGroup.interactable = false;
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

            ComItemManager.Destroy(awardItem);
            awardItem = null;

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

        private void _SetAwardItem(int itemTableId, int itemTotalCount)
        {
            if (awardItem == null)
            {
                awardItem = ComItemManager.Create(awardItemRoot);
            }
            if(awardItem != null)
            {
                ItemData itemData = ItemDataManager.CreateItemDataFromTable(itemTableId);
                awardItem.SetCountFormatter((var) =>
                {
                    return TR.Value("adventure_team_expeidtion_dispatch_reward_count", itemTotalCount.ToString());
                });
                awardItem.Setup(itemData, Utility.OnItemClicked);
            }
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
            if (model.rewardList != null && model.rewardList.Count > 0)
            {
                var reward = model.rewardList[0];
                if (!string.IsNullOrEmpty(reward.rewards))
                {
                    string[] tempRewards = reward.rewards.Split(':');
                    if (tempRewards != null && tempRewards.Length == 2)
                    {
                        int firstItemId;
                        if (int.TryParse(tempRewards[0], out firstItemId))
                        {
                            int totalCount = AdventureTeamDataManager.GetInstance().GetExpeditionRewardItemTotalCount(new List<ExpeditionMapModel>() { this.mapModel });
                            _SetAwardItem(firstItemId, totalCount);
                        }
                    }
                }
            }
            if(roleInfoView != null && roleJobIds != null && roleJobIds.Length > 0)
            {
                roleInfoView.SetElementAmount(roleJobIds.Length);
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
