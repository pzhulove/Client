using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    public class AdventureTeamExpeditionResultView : MonoBehaviour
    {
        #region MODEL PARAMS

        private List<ExpeditionMapModel> mFinishMapModels;

        #endregion

        #region VIEW PARAMS

        [SerializeField] private ComUIListScript resultItemView;
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
            if (mFinishMapModels != null)
            {
                mFinishMapModels.Clear();
            }

            _ClearView();
        }

        private void _InitView()
        {
            if (resultItemView != null)
            {
                if (resultItemView.IsInitialised() == false)
                {
                    resultItemView.Initialize();
                }
                resultItemView.onBindItem += _OnResultItemBind;
                resultItemView.onItemVisiable += _OnResultItemVisiable;
                resultItemView.OnItemRecycle += _OnResultItemRecycle;
            }
        }

        private void _ClearView()
        {
            if (resultItemView != null)
            {
                resultItemView.onBindItem -= _OnResultItemBind;
                resultItemView.onItemVisiable -= _OnResultItemVisiable;
                resultItemView.OnItemRecycle -= _OnResultItemRecycle;
                resultItemView.UnInitialize();
            }
        }

        private AdventureTeamExpeditionResultItem _OnResultItemBind(GameObject go)
        {
            if (go == null)
            {
                return null;
            }
            return go.GetComponent<AdventureTeamExpeditionResultItem>();
        }

        private void _OnResultItemVisiable(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }
            if (mFinishMapModels == null || mFinishMapModels.Count <= 0)
            {
                return;
            }
            int i_Index = item.m_index;
            if (i_Index < 0 || i_Index >= mFinishMapModels.Count)
            {
                return;
            }
            var resultItem = item.gameObjectBindScript as AdventureTeamExpeditionResultItem;
            if (resultItem != null)
            {
                resultItem.RefreshView(mFinishMapModels[i_Index]);
            }
        }

        private void _OnResultItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }
            var resultItem = item.gameObjectBindScript as AdventureTeamExpeditionResultItem;
            if (resultItem != null)
            {
                resultItem.ClearView();
            }
        }

        private void _SetResultDescText(string mapCount)
        {
            resultDescText.SafeSetText(TR.Value("adventure_team_expeidtion_dispatch_res", mapCount.ToString()));
        }

        private void _SetRewardItemIcon(string imgPath)
        {
            resultRewardItemIcon.SafeSetImage(imgPath);
        }

        private void _SetRewardItemCount(string itemCount)
        {
            resultRewardItemCountDescText.SafeSetText(TR.Value("adventure_team_expeidtion_dispatch_reward_count", itemCount));
        }

        #endregion

        #region  PUBLIC METHODS

        public void InitView(List<ExpeditionMapModel> finishMapModels)
        {
            if (finishMapModels == null || finishMapModels.Count <= 0)
            {
                return;
            }
            this.mFinishMapModels = finishMapModels;

            _SetResultDescText(finishMapModels.Count.ToString());

           int totalRewardCount = AdventureTeamDataManager.GetInstance().GetExpeditionRewardItemTotalCount(finishMapModels, _SetRewardItemIcon);
            _SetRewardItemCount(totalRewardCount.ToString());

            if (resultItemView != null)
            {
                resultItemView.SetElementAmount(finishMapModels.Count);
            }
        }

        public void RefreshView()
        {

        }
        
        #endregion
    }
}
