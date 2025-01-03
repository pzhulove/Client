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

namespace GameClient
{
    // 带此次登录是否提示toggle的msgbox
    public class LoginToggleMsgBoxOKCancelFrame : ClientFrame
    {
        #region inner define

        public enum LoginToggleMsgType
        {
            Invalid,
            NotCostFatigue,
            EnterEliteDungeonTip,
            AdventurerPassCardBuyLevel,  // 购买冒险者通行证等级
            EnterDungeonBuyDrug,         // 进入地下城的时候提示购买药水
        }

        public class LoginToggleMsgBoxParama
        {
            public LoginToggleMsgType loginToggleMsgType = LoginToggleMsgType.Invalid;
            public string msgContent = "";
            public string okContent = "";
            public string cancelContent = "";
            public UnityEngine.Events.UnityAction okAction = null;
            public UnityEngine.Events.UnityAction cancelAction = null;
            public bool isShowCloseBtn = false;
        }

        #endregion

        #region val  

        static Dictionary<LoginToggleMsgType, bool> toggleStates = new Dictionary<LoginToggleMsgType, bool>();
        static ulong frameID = 0;
        LoginToggleMsgBoxParama loginToggleMsgBoxParama = null;

        #endregion

        #region ui bind
        private Text mMsgText = null;
        private GameObject mNotifyToggleRoot = null;
        private Button mBtCancel = null;
        private Button mBtOK = null;
        private Text mCancleButtonText = null;
        private Text mOkButtonText = null;
        private Toggle mNotifyToggle = null;
        private GameObject mClose = null;

        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/CommonSystemNotify/LoginToggleMsgBoxOKCancel";
        }

        protected override void _OnOpenFrame()
        {
            loginToggleMsgBoxParama = userData as LoginToggleMsgBoxParama;

            UpdateUI();
        }

        protected override void _OnCloseFrame()
        {
            loginToggleMsgBoxParama = null;
        }

        protected override void _bindExUI()
        {
            mMsgText = mBind.GetCom<Text>("msgText");
            mNotifyToggleRoot = mBind.GetGameObject("notifyToggleRoot");
            mBtCancel = mBind.GetCom<Button>("btCancel");
            mBtCancel.SafeSetOnClickListener(() => 
            {
                if(loginToggleMsgBoxParama != null && loginToggleMsgBoxParama.cancelAction != null)
                {
                    loginToggleMsgBoxParama.cancelAction();
                }

                frameMgr.CloseFrame(this);
            });
            mBtOK = mBind.GetCom<Button>("btOK");
            mBtOK.SafeSetOnClickListener(() => 
            {
                if(loginToggleMsgBoxParama != null && loginToggleMsgBoxParama.okAction != null)
                {
                    loginToggleMsgBoxParama.okAction();
                }

                frameMgr.CloseFrame(this);
            });
            mCancleButtonText = mBind.GetCom<Text>("cancleButtonText");
            mOkButtonText = mBind.GetCom<Text>("okButtonText");

            mNotifyToggle = mBind.GetCom<Toggle>("notifyToggle");
            mNotifyToggle.SafeSetOnValueChangedListener((value) => 
            {
                if(loginToggleMsgBoxParama != null && toggleStates != null)
                {
                    toggleStates.SafeAdd(loginToggleMsgBoxParama.loginToggleMsgType, value);
                }
            });

            mClose = mBind.GetGameObject("Close");
        }

        protected override void _unbindExUI()
        {
            mMsgText = null;
            mNotifyToggleRoot = null;
            mBtCancel = null;   
            mBtOK = null;
            mCancleButtonText = null;
            mOkButtonText = null;
            mNotifyToggle = null;
            mClose = null;
        }

        #endregion 

        #region method

        void BindUIEvent()
        {
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnGuildDungeonAuctionItemsUpdate, _OnUpdateItems);
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnGuildDungeonAuctionAddNewItem, _OnGuildDungeonAuctionAddNewItem);
        }

        void UnBindUIEvent()
        {
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnGuildDungeonAuctionItemsUpdate, _OnUpdateItems);
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnGuildDungeonAuctionAddNewItem, _OnGuildDungeonAuctionAddNewItem);
        }
        
        public static void TryShowMsgBox(LoginToggleMsgType loginToggleMsgType,string msgContent,UnityEngine.Events.UnityAction okAction = null,UnityEngine.Events.UnityAction cancelAction = null,string okContent = "",string cancelContent = "", bool isShowClose = false)
        {
            LoginToggleMsgBoxParama loginToggleMsgBoxParama = new LoginToggleMsgBoxParama()
            {
                loginToggleMsgType = loginToggleMsgType,
                msgContent = msgContent,
                okContent = okContent,
                cancelContent = cancelContent,
                okAction = okAction,
                cancelAction = cancelAction,
                isShowCloseBtn = isShowClose,
            };

            if(loginToggleMsgBoxParama == null)
            {
                return;
            }

            if(toggleStates.SafeGetValue(loginToggleMsgBoxParama.loginToggleMsgType))
            {
                if(okAction != null)
                {
                    okAction();
                }

                return;
            }

            ClientSystemManager.GetInstance().OpenFrame<LoginToggleMsgBoxOKCancelFrame>(FrameLayer.Middle, loginToggleMsgBoxParama,string.Format("LoginToggleMsgBoxOKCancelFrame", frameID++));
        }

        public static void Reset()
        {
            if(toggleStates != null)
            {
                toggleStates.Clear();
            }

            frameID = 0;
        }

        void UpdateUI()
        {
            if(loginToggleMsgBoxParama == null)
            {
                return;
            }

            mClose.CustomActive(loginToggleMsgBoxParama.isShowCloseBtn);

            if (string.IsNullOrEmpty(loginToggleMsgBoxParama.msgContent))
            {
                mMsgText.SafeSetText(TR.Value("LoginToggleMsgBoxOKCancelFrame_defatul_msg_content"));
            }
            else
            {
                mMsgText.SafeSetText(loginToggleMsgBoxParama.msgContent);
            }

            if(string.IsNullOrEmpty(loginToggleMsgBoxParama.cancelContent))
            {
                mCancleButtonText.SafeSetText(TR.Value("LoginToggleMsgBoxOKCancelFrame_defatul_cancel_content"));
            }
            else
            {
                mCancleButtonText.SafeSetText(loginToggleMsgBoxParama.cancelContent);
            }

            if(string.IsNullOrEmpty(loginToggleMsgBoxParama.okContent))
            {
                mOkButtonText.SafeSetText(TR.Value("LoginToggleMsgBoxOKCancelFrame_defatul_ok_content"));
            }
            else
            {
                mOkButtonText.SafeSetText(loginToggleMsgBoxParama.okContent);
            }

            return;
        }

        #endregion

        #region ui event
      
        #endregion
    }
}
