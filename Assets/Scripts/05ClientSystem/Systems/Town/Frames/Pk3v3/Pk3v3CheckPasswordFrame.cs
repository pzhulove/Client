using Network;
using Protocol;
using UnityEngine;
using ProtoTable;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

namespace GameClient
{
    public class Pk3v3CheckPasswordFrame : ClientFrame
    {
        RoomSimpleInfo simpleinfo = null;
        string password = "";

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk3v3/Pk3v3PasswordAuthentication";
        }

        protected override void _OnOpenFrame()
        {
            if(userData != null)
            {
                simpleinfo = (RoomSimpleInfo)userData;
            }

            InitInterface();
            BindUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            ClearData();
            UnBindUIEvent();
        }

        void ClearData()
        {
            simpleinfo = null;
            password = "";
        }

        protected void BindUIEvent()
        {
            mPassword.onValueChanged.AddListener(OnPasswordInputEnd);
        }

        protected void UnBindUIEvent()
        {
            mPassword.onValueChanged.RemoveListener(OnPasswordInputEnd);
        }

        void OnPasswordInputEnd(string passWord)
        {
            if (string.IsNullOrEmpty(passWord))
            {
                return;
            }

            password = passWord;
        }

        void InitInterface()
        {
        }

        #region ExtraUIBind
        private InputField mPassword = null;
        private Button mBtOk = null;
        private Button mBtClose = null;

        protected override void _bindExUI()
        {
            mPassword = mBind.GetCom<InputField>("Password");
            mBtOk = mBind.GetCom<Button>("btOk");
            mBtOk.onClick.AddListener(_onBtOkButtonClick);
            mBtClose = mBind.GetCom<Button>("btClose");
            mBtClose.onClick.AddListener(_onBtCloseButtonClick);
        }

        protected override void _unbindExUI()
        {
            mPassword = null;
            mBtOk.onClick.RemoveListener(_onBtOkButtonClick);
            mBtOk = null;
            mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            mBtClose = null;
        }
        #endregion

        #region Callback
        private void _onBtOkButtonClick()
        {
            if(simpleinfo == null)
            {
                return;
            }

            bool bCanSend = false;
            int iPassword = 0;

            if (int.TryParse(password, out iPassword))
            {
                if (iPassword < 1000 || iPassword > 9999)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("密码格式错误,请输入4位数字密码");
                }
                else
                {
                    bCanSend = true;
                }
            }
            else
            {
                SystemNotifyManager.SysNotifyFloatingEffect("密码格式错误,请输入4位数字密码");
            }

            if(bCanSend)
            {
                Pk3v3DataManager.SendJoinRoomReq(simpleinfo.id, (RoomType)simpleinfo.roomType, password);
                frameMgr.CloseFrame(this);
            }           
        }

        private void _onBtCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}
