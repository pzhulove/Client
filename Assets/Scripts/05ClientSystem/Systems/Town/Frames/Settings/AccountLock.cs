﻿using System;
using System.Collections.Generic;
///////删除linq
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
    public class AccountLock : ClientFrame
    {
        #region val

        #endregion

        #region ui bind

        InputField edtPwd1;
        InputField edtPwd2;
        Button btnClose;
        Button btnOK;
        Image imgGou;

        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SettingPanel/AccountLock";
        }

        protected override void _OnOpenFrame()
        {
            BindUIEvent();           
        }

        protected override void _OnCloseFrame()
        {
            UnBindUIEvent();
        }

        protected override void _bindExUI()
        {
            edtPwd1 = mBind.GetCom<InputField>("Pwd1");
            edtPwd2 = mBind.GetCom<InputField>("Pwd2");
            btnClose = mBind.GetCom<Button>("BtnClose");
            if(btnClose != null)
            {
                btnClose.onClick.RemoveAllListeners();
                btnClose.onClick.AddListener(() => 
                {
                    frameMgr.CloseFrame(this);
                });
            }

            btnOK = mBind.GetCom<Button>("btOK");
            if(btnOK != null)
            {
                btnOK.onClick.RemoveAllListeners();
                btnOK.onClick.AddListener(() => 
                {
                    if(edtPwd1 != null && edtPwd2 != null)
                    {
                        if(edtPwd1.textComponent.text != edtPwd2.textComponent.text)
                        {
                            SystemNotifyManager.SysNotifyFloatingEffect("两次输入的密码不一致");
                            return;
                        }

                        if(edtPwd1.textComponent.text.Length < SecurityLockDataManager.nPwdMinLength || edtPwd1.textComponent.text.Length > SecurityLockDataManager.nPwdMaxLength)
                        {
                            SystemNotifyManager.SysNotifyFloatingEffect("密码长度错误(密码长度最小4位，最大8位)");
                            return;
                        }

                        SecurityLockDataManager.GetInstance().SendWorldSecurityLockOpReq(LockOpType.LT_LOCK, edtPwd1.textComponent.text);
                    }                    
                });
            }

            imgGou = mBind.GetCom<Image>("imgGou");
        }

        protected override void _unbindExUI()
        {
            edtPwd1 = null;
            edtPwd2 = null;
            btnClose = null;
            btnOK = null;
            imgGou = null;
        }

        public override bool IsNeedUpdate()
        {
            return false;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            if (imgGou != null && edtPwd1 != null && edtPwd2 != null)
            {
                imgGou.CustomActive(edtPwd1.textComponent.text == edtPwd2.textComponent.text && edtPwd1.textComponent.text != "");
            }
        }

        #endregion 

        #region method

        void BindUIEvent()
        {
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChangeNum2, OnChangeNum);
        }

        void UnBindUIEvent()
        {
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ChangeNum2, OnChangeNum);
        }

        #endregion

        #region ui event
        void OnChangeNum(UIEvent iEvent)
        {
            
        }

        #endregion
    }
}