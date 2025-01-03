using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public enum StrengthenTicketMergeType
    {
        Buff = 0,
        Material,
        StrengthenTicket,
        Count,
    }

    /// <summary>
    /// 配合动画数量
    /// </summary>
    public enum StrengthenTicketMergeStage
    {
        NotReady = 0,
        Ready,
        Waiting,
        Process,
        ReverseReady,           //准备合成 倒放状态
        Count,
    }

    public class StrengthenTicketMergeFrame : ClientFrame
    {
        //预制体路径
        public const string LEFT_VIEW_FRAME_RES_PATH = "UIFlatten/Prefabs/StrengthenTicketMerge/StrengthenTicketMergeLeftItem";
        public const string RIGHT_VIEW_FRAME_RES_PATH = "UIFlatten/Prefabs/StrengthenTicketMerge/StrengthenTicketMergeRightItem";
        public const string CUSTOM_COM_ITEM_RES_PATH = "UIFlatten/Prefabs/StrengthenTicketMerge/ComItemCustom";
        
        #region PRIVATE METHODS
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/StrengthenTicketMerge/StrengthenTicketMergeFrame";
        }
        protected override void _OnOpenFrame()
        {
            if (null != mView)
            {
                mView.OnInit();
            }
            _BindUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            if (null != mView)
            {
                mView.OnUninit();
            }
            _UnBindUIEvent();
        }

        public int GetFuseTicketCount()
        {
            if (null != mView)
            {
                return mView.GetFuseTicketCount();
            }
            return 0;
        }

        private void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnStrengthenTicketMergeSelectTicket, _OnStrengthenTicketMergeSelectTicket);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnStrengthenTicketFuseAddTicket, _OnStrengthenTicketFuseAddTicket);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnStrengthenTicketFuseRemoveTicket, _OnStrengthenTicketFuseRemoveTicket);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnStrengthenTicketMergeSuccess, _OnStrengthenTicketMergeSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnStrengthenTicketFuseSuccess, _OnStrengtheTicketFuseSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnStrengthenTicketMergeFailed, _OnStrengthenTicketMergeFailed);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnStrengthenTicketFuseFailed, _OnStrengthenTicketFuseFailed);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnStrengthenTicketFuseCalPercent, _OnStrengthenTicketFuseCalPercent);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnStrengthenTicketMergeSelectReset, _OnStrengthenTicketMergeSelectReset);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnStrengthenTicketStartMerge, _OnStrengthenTicketStartMerge);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GoldChanged, _OnGoldChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BindGoldChanged, _OnBindGoldChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnStrengthenTicketMallBuySuccess, _OnStrengthenTicketMallBuySuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityLimitTimeTaskUpdate, _OnUpdateUseTime);
        }

        private void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnStrengthenTicketMergeSelectTicket, _OnStrengthenTicketMergeSelectTicket);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnStrengthenTicketFuseRemoveTicket, _OnStrengthenTicketFuseRemoveTicket);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnStrengthenTicketFuseAddTicket, _OnStrengthenTicketFuseAddTicket);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnStrengthenTicketMergeSuccess, _OnStrengthenTicketMergeSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnStrengthenTicketFuseSuccess, _OnStrengtheTicketFuseSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnStrengthenTicketMergeFailed, _OnStrengthenTicketMergeFailed);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnStrengthenTicketFuseFailed, _OnStrengthenTicketFuseFailed);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnStrengthenTicketFuseCalPercent, _OnStrengthenTicketFuseCalPercent);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnStrengthenTicketMergeSelectReset, _OnStrengthenTicketMergeSelectReset);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnStrengthenTicketStartMerge, _OnStrengthenTicketStartMerge);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GoldChanged, _OnGoldChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BindGoldChanged, _OnBindGoldChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnStrengthenTicketMallBuySuccess, _OnStrengthenTicketMallBuySuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityLimitTimeTaskUpdate, _OnUpdateUseTime);
        }

        #region CALLBACK
        //强化券合成折扣次数
        private void _OnUpdateUseTime(UIEvent uiEvent)
        {
            if (null != mView)
                mView.OnUpdateDisCount();
        }
        //强化券合成选择的券
        private void _OnStrengthenTicketMergeSelectTicket(UIEvent param)
        {
            if (null != mView)
            {
                mView.OnStrengthenTicketMergeSelectTicket(param);
            }
        }

        //强化券融合移除选择的券
        private void _OnStrengthenTicketFuseRemoveTicket(UIEvent param)
        {
            if (null != mView)
            {
                mView.OnStrengthenTicketFuseRemoveTicket(param);
            }
        }

        //强化券融合添加选择的券
        private void _OnStrengthenTicketFuseAddTicket(UIEvent param)
        {
            if (null != mView)
            {
                mView.OnStrengthenTicketFuseAddTicket(param);
            }
        }

        //强化券合成成功
        void _OnStrengthenTicketMergeSuccess(UIEvent param)
        {
            if (null != mView)
            {
                mView.OnStrengthenTicketMergeSuccess(param);
            }
        }

        //强化券融合成功
        void _OnStrengtheTicketFuseSuccess(UIEvent param)
        {
            if (null != mView)
            {
                mView.OnStrengtheTicketFuseSuccess(param);
            }
        }

        //强化券合成失败
        void _OnStrengthenTicketMergeFailed(UIEvent param)
        {
            if (null != mView)
            {
                mView.OnStrengthenTicketMergeFailed(param);
            }
        }

        //强化券融合失败
        void _OnStrengthenTicketFuseFailed(UIEvent param)
        {
            if (null != mView)
            {
                mView.OnStrengthenTicketFuseFailed(param);
            }
        }

        //强化券融合预计概率生成
        void _OnStrengthenTicketFuseCalPercent(UIEvent param)
        {
            if (null != mView)
            {
                mView.OnStrengthenTicketFuseCalPercent(param);
            }
        }

        //强化券合成 选中第一个
        void _OnStrengthenTicketMergeSelectReset(UIEvent param)
        {
            if (null != mView)
            {
                mView.OnStrengthenTicketMergeSelectReset(param);
            }
        }

        //开始强化券融合时禁止点击
        void _OnStrengthenTicketStartMerge(UIEvent param)
        {
            if (null != mView)
            {
                mView.OnStrengthenTicketStartMerge(param);
            }
        }

        void _OnGoldChanged(UIEvent param)
        {
            if (null != mView)
            {
                mView.OnGoldChanged(param);
            }
        }

        void _OnBindGoldChanged(UIEvent param)
        {
            if (null != mView)
            {
                mView.OnBindGoldChanged(param);
            }
        }

        void _OnStrengthenTicketMallBuySuccess(UIEvent param)
        {
            if (null != mView)
            {
                mView.OnStrengthenTicketMallBuySuccess(param);
            }
        }

        #endregion

        #endregion

		#region ExtraUIBind
        private StrengthenTicketMergeFrameView mView = null;
		
		protected override void _bindExUI()
		{
			mView = mBind.GetCom<StrengthenTicketMergeFrameView>("View");
            
		}
		
		protected override void _unbindExUI()
		{
            mView = null;
		}
		#endregion
    }
}
