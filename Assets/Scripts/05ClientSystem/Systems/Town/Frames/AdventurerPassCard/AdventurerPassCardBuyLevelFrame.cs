using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;
using Network;

namespace GameClient
{
    // 购买冒险者通行证等级界面
    public class AdventurerPassCardBuyLevelFrame : ClientFrame
    {

        private AdventurerPassCardBuyLevelFrameView mView = null;
#region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/AdventurerPassCard/AdventurerPassCardBuyLevel";
        }

        protected override void _OnOpenFrame()
        {
            mView.OnInit();
            BindUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            mView.OnUninit();
            UnBindUIEvent();
        }

        protected override void _bindExUI()
        {
            mView = mBind.GetCom<AdventurerPassCardBuyLevelFrameView>("View");
        }

        protected override void _unbindExUI()
        {
            mView = null;
        }

#endregion

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BuyAdventureLevelSucc, _OnBuyAventureLevelSucc);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UpdateAventurePassStatus, _OnUpdateAventurePassStatus);
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BuyAdventureLevelSucc, _OnBuyAventureLevelSucc);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UpdateAventurePassStatus, _OnUpdateAventurePassStatus);
        }

        void _OnBuyAventureLevelSucc(UIEvent uiEvent)
        {
            if (null != mView)
                mView.OnClickCancel();
        }

        void _OnUpdateAventurePassStatus(UIEvent uiEvent)
        {
            // UpdateBuyItems();
            return;
        }

    }
}
