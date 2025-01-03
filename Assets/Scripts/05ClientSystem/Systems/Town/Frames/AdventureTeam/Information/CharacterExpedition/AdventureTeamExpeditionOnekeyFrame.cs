using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AdventureTeamExpeditionOnekeyFrame : ClientFrame
    {
        List<ExpeditionMapModel> mReadyMapModels = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/AdventureTeam/AdventureTeamExpeditionOnekeyFrame";
        }

        protected override void _OnOpenFrame()
        {
            _BindUIEvent();

            if (userData == null)
            {
                return;
            }
            mReadyMapModels = userData as List<ExpeditionMapModel>;

            if (mMainContentView != null)
            {
                mMainContentView.InitView(mReadyMapModels);
            }
        }

        protected override void _OnCloseFrame()
        {
            _UnBindUIEvent();

            if (mReadyMapModels != null)
            {
                mReadyMapModels.Clear();
            }
        }

        private void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamExpeditionTimeChanged, _OnExpeditionTimeChanged);
        }

        private void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamExpeditionTimeChanged, _OnExpeditionTimeChanged);
        }

        private void _OnExpeditionTimeChanged(UIEvent uIEvent)
        {
            if (mMainContentView != null)
            {
                mMainContentView.RefreshView();
            }
        }

        #region ExtraUIBind
        private AdventureTeamExpeditionOnekeyView mMainContentView = null;
        private Button mOkButton = null;
        private Button mCloseButton = null;

        protected override void _bindExUI()
        {
            mMainContentView = mBind.GetCom<AdventureTeamExpeditionOnekeyView>("MainContentView");
            mOkButton = mBind.GetCom<Button>("okButton");
            mOkButton.onClick.AddListener(_onOkButtonButtonClick);
            mCloseButton = mBind.GetCom<Button>("closeButton");
            mCloseButton.onClick.AddListener(_onCloseButtonButtonClick);
        }

        protected override void _unbindExUI()
        {
            mMainContentView = null;
            mOkButton.onClick.RemoveListener(_onOkButtonButtonClick);
            mOkButton = null;
            mCloseButton.onClick.RemoveListener(_onCloseButtonButtonClick);
            mCloseButton = null;
        }
        #endregion

        #region Callback
        private void _onOkButtonButtonClick()
        {
            AdventureTeamDataManager.GetInstance().ReqDispatchExpeditionTeam(mReadyMapModels);
            this.Close();
        }
        private void _onCloseButtonButtonClick()
        {
            AdventureTeamDataManager.GetInstance().ClearReadyExpeditionMapModels(mReadyMapModels);
            this.Close();
        }
        #endregion
    }
}
