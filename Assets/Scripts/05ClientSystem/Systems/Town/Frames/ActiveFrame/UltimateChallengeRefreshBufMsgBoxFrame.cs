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
    public class UltimateChallengeRefreshBufMsgBoxFrame : ClientFrame
    {
        #region val

        public class MsgBoxData
        {
            public string content = "";
            public UnityEngine.Events.UnityAction cancelCallBack = null;
            public UnityEngine.Events.UnityAction okCallBack = null;
        }

        MsgBoxData msgBoxData = null;
        #endregion

        #region ui bind       
        Text content = null;
        Button Cancel = null;
        Button OK = null;
        Toggle CanNotify = null;
        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Activity/UltimateChallengeRefreshBufMsgBox";
        }

        protected override void _OnOpenFrame()
        {
            msgBoxData = userData as MsgBoxData;
            BindUIEvent();

            Cancel.SafeRemoveAllListener();
            OK.SafeRemoveAllListener();           

            if (msgBoxData != null)
            {
                content.SafeSetText(msgBoxData.content);

                Cancel.SafeAddOnClickListener(() =>
                {
                    if (msgBoxData.cancelCallBack != null)
                    {
                        msgBoxData.cancelCallBack();
                    }
                });

                OK.SafeAddOnClickListener(() =>
                {
                    if (msgBoxData.okCallBack != null)
                    {
                        msgBoxData.okCallBack();
                    }
                });
            }

            Cancel.SafeAddOnClickListener(() =>
            {
                frameMgr.CloseFrame(this);
            });

            OK.SafeAddOnClickListener(() =>
            {
                frameMgr.CloseFrame(this);
            });

            CanNotify.SafeSetToggleOnState(ActivityDataManager.GetInstance().NotPopUpRefreshBufMsgBox);
        }

        protected override void _OnCloseFrame()
        {
            msgBoxData = null;
            UnBindUIEvent();                      
        }

        protected override void _bindExUI()
        {
            content = mBind.GetCom<Text>("content");
            Cancel = mBind.GetCom<Button>("Cancel"); 
            OK = mBind.GetCom<Button>("OK");
            CanNotify = mBind.GetCom<Toggle>("CanNotify");
            CanNotify.SafeAddOnValueChangedListener((val) => 
            {
                ActivityDataManager.GetInstance().NotPopUpRefreshBufMsgBox = val;
            });
        }

        protected override void _unbindExUI()
        {
            content = null;
            Cancel = null;
            OK = null;
            CanNotify = null;
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
