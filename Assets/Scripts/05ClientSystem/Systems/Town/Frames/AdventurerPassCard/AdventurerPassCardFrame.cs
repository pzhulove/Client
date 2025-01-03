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
    // 冒险者通行证加界面
    public class AdventurerPassCardFrame : ClientFrame
    {
        #region override

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/AdventurerPassCard/AdventurerPassCard";
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnOpenFrame()
        {
            if (null != mView)
            {
                mView.OnInit();
            }
            BindUIEvent();
        }

        protected override void _OnCloseFrame()
        {   
            if (null != mView)
                mView.OnUninit();
            UnBindUIEvent();
        }

        private AdventurerPassCardFrameView mView;
        protected override void _bindExUI()
        {
            mView = mBind.GetCom<AdventurerPassCardFrameView>("View");
        }

        protected override void _unbindExUI()
        {
            mView = null;
        }

        #endregion

        #region method

        void BindUIEvent()
        {
            
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.AdventureUnlockKing, _OnAdventureUnlockKing);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UpdateAventurePassStatus, _OnUpdateAventurePassStatus);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UpdateAventurePassExpPackStatus, _OnUpdateAventurePassExpPackStatus);
            
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.AdventureUnlockKing, _OnAdventureUnlockKing);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UpdateAventurePassStatus, _OnUpdateAventurePassStatus);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UpdateAventurePassExpPackStatus, _OnUpdateAventurePassExpPackStatus);
        }

        // void UpdateUI()
        // {
        //     _OnUpdateAventurePassStatus(null);
        // }

        protected override void _OnUpdate(float timeElapsed)
        {
            if (null != mView)
                mView.OnUpdate(timeElapsed);
        }

        //能否一键领取
        public static bool CanOneKeyGetAwards()
        {
            bool canGetNormalAwards = false;
            bool canGetHighAwards = false;

            for (int i = 1; i <= AdventurerPassCardDataManager.GetInstance().CardLv; i++)
            {
                if (!AdventurerPassCardDataManager.GetInstance().IsNormalAwardReceived(i))
                {
                    canGetNormalAwards = true;
                    break;
                }
            }
            // 购买了高级通行证
            if (AdventurerPassCardDataManager.GetInstance().GetPassCardType > AdventurerPassCardDataManager.PassCardType.Normal)
            {
                for (int i = 1; i <= AdventurerPassCardDataManager.GetInstance().CardLv; i++)
                {
                    if (!AdventurerPassCardDataManager.GetInstance().IsHighAwardReceived(i))
                    {
                        canGetHighAwards = true;
                        break;
                    }
                }
            }
            return canGetNormalAwards || canGetHighAwards;
        }

        #endregion

        #region ui event

        // 刷新经验包状态
        void _OnUpdateAventurePassExpPackStatus(UIEvent uiEvent)
        {   
            if (null != mView)
                mView.OnUpdateAventurePassExpPackStatus(uiEvent);
        }

        // 刷新通行证相关数据
        void _OnUpdateAventurePassStatus(UIEvent uiEvent)
        {
            if (null != mView)
                mView.OnUpdateAventurePassStatus(uiEvent);
        }

        //解锁王者版
        private void _OnAdventureUnlockKing(UIEvent uiEvent)
        {
            if (null != mView)
            {
                mView.OnAdventureUnlockKing();
            }
        }
        #endregion
    }
}
