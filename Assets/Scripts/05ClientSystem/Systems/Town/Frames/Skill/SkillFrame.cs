using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameClient
{
    public enum SkillFrameType
    {
        Normal = 0,
        FairDuel,
    }

    public enum SkillFrameTabType
    {
        PVE = 0,
        PVP,
        Setting,
    }

    public class SkillFrameParam
    {
        public SkillFrameType frameType = SkillFrameType.Normal;
        public SkillFrameTabType tabTypeIndex = SkillFrameTabType.PVE;

        public void Clear()
        {
            frameType = SkillFrameType.Normal;
            tabTypeIndex = SkillFrameTabType.PVE;
        }
    }

    public class SkillFrame : ClientFrame
    {
        public static SkillFrameParam frameParam = new SkillFrameParam();
        private bool bIsSwitchMainTabDrived = false;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Skill/SkillFrame";
        }

        protected override void _OnOpenFrame()
        {
            if(userData != null)
            {
                frameParam = userData as SkillFrameParam;
            }
            _BindUIEvent();
            mView.OnInit();
        }

        protected override void _OnCloseFrame()
        {
            _UnBindUIEvent();
            mView.OnUninit();
        }

        private void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SpChanged, _OnSpChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSelectSkillPage, _OnSelectSkillPage);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SkillPlanPageUnlock, _OnBuySkillPage2Sucess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CloseSkillConfigFrame, _OnCloseSkillConfigFrame);
        }

        private void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SpChanged, _OnSpChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSelectSkillPage, _OnSelectSkillPage);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SkillPlanPageUnlock, _OnBuySkillPage2Sucess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CloseSkillConfigFrame, _OnCloseSkillConfigFrame);
        }

        //关闭技能页
        private void _OnCloseSkillConfigFrame(UIEvent uiEvent)
        {
            if (null != mView)
                mView.OnCloseSkillConfigFrame();
        }

        //解锁技能页2成功
        private void _OnBuySkillPage2Sucess(UIEvent uiEvent)
        {
            if (null != mView)
                mView.OnBuySkillPage2Sucess();
        }

        //技能点变化
        private void _OnSpChanged(UIEvent uiEvent)
        {
            if (null != mView)
                mView.OnUpdateSp();
        }

        //选中技能
        private void _OnSelectSkillPage(UIEvent uiEvent)
        {
            if (null != mView)
                mView.OnSelectSkillPage();
        }

        #region ExtraUIBind
        private SkillFrameView mView = null;
        protected override void _bindExUI()
        {
            mView = mBind.GetCom<SkillFrameView>("SkillFrameView");
        }
        protected override void _unbindExUI()
        {
            mView = null;
        }
        #endregion
    }
}
