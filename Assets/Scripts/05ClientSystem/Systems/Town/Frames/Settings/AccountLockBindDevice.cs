using System;
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
    public class AccountLockBindDevice : ClientFrame
    {
        #region val

        #endregion

        #region ui bind
 
        Button btnClose;
        Button btnOK;
        Button btnCancel;   

        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SettingPanel/AccountLockBindDevice";
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
            btnClose = mBind.GetCom<Button>("BtnClose");         
            if (btnClose != null)
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
                    SecurityLockDataManager.GetInstance().SendWorldBindDeviceReq(true);
                });
            }

            btnCancel = mBind.GetCom<Button>("btCancel");
            if(btnCancel != null)
            {
                btnCancel.onClick.RemoveAllListeners();
                btnCancel.onClick.AddListener(() => 
                {
                    frameMgr.CloseFrame(this);
                });
            }
        }

        protected override void _unbindExUI()
        {          
            btnOK = null;
            btnCancel = null;
            btnClose = null;         
        }

        #endregion 

        #region method

        void BindUIEvent()
        {
            
        }

        void UnBindUIEvent()
        {
            
        }

        #endregion

        #region ui event  

        #endregion
    }
}
