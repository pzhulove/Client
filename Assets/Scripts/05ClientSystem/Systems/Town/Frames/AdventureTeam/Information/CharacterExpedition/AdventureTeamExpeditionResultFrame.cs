using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AdventureTeamExpeditionResultFrame : ClientFrame
    {
        List<ExpeditionMapModel> mFinishedMapModels = null;
        ExpeditionMapNetInfo tempLastNetMapInfo = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/AdventureTeam/AdventureTeamExpeditionResultFrame";
        }

        protected override void _OnOpenFrame()
        {
            if (userData == null)
            {
                return;
            }
            mFinishedMapModels = userData as List<ExpeditionMapModel>;

            if (mAdventureTeamChangeNameView != null)
            {
                mAdventureTeamChangeNameView.InitView(mFinishedMapModels);
            }
        }

        protected override void _OnCloseFrame()
        {
            if (mFinishedMapModels != null)
            {
                mFinishedMapModels.Clear();
            }

            if (tempLastNetMapInfo != null)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamExpeditionResultFrameClose, new ExpeditionMapNetInfo() { mapId = tempLastNetMapInfo.mapId });
                tempLastNetMapInfo = null;
            }
            else
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamExpeditionResultFrameClose);
            }
        }

        #region ExtraUIBind
        private AdventureTeamExpeditionResultView mAdventureTeamChangeNameView = null;
        private Button mOkButton = null;

        protected override void _bindExUI()
        {
            mAdventureTeamChangeNameView = mBind.GetCom<AdventureTeamExpeditionResultView>("MainContentView");
            mOkButton = mBind.GetCom<Button>("okButton");
            mOkButton.onClick.AddListener(_onOkButtonButtonClick);
        }

        protected override void _unbindExUI()
        {
            mAdventureTeamChangeNameView = null;
            mOkButton.onClick.RemoveListener(_onOkButtonButtonClick);
            mOkButton = null;
        }
        #endregion

        #region Callback
        private void _onOkButtonButtonClick()
        {
            if (mFinishedMapModels != null && mFinishedMapModels.Count > 0)
            {
                if (mFinishedMapModels[mFinishedMapModels.Count - 1] != null)
                {
                    tempLastNetMapInfo = mFinishedMapModels[mFinishedMapModels.Count - 1].mapNetInfo;
                }
                for (int i = 0; i < mFinishedMapModels.Count; i++)
                {
                    var mapModel = mFinishedMapModels[i];
                    if (mapModel == null || mapModel.mapNetInfo == null)
                        continue;
                    AdventureTeamDataManager.GetInstance().ReqGetExpeditionRewards(mapModel.mapNetInfo.mapId);                    
                }
            }

            this.Close();
        }
        #endregion
    }
}
