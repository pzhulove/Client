using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace GameClient
{
    public sealed class MonthCardTipFrame : ClientFrame
    {
        Button closeBtn;
        Button openVipBtn;

        public  override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Vip/MonthCardTipFrame";
        }

        protected  override void _bindExUI()
        {
            closeBtn = mBind.GetCom<Button>("BtnClose");
            if (closeBtn)
            {
                closeBtn.onClick.RemoveListener(CloseBtnClick);
                closeBtn.onClick.AddListener(CloseBtnClick);
            }
            openVipBtn = mBind.GetCom<Button>("BtnVip");
            if (openVipBtn)
            {
                openVipBtn.onClick.RemoveListener(OpenVipBtnClick);
                openVipBtn.onClick.AddListener(OpenVipBtnClick);
            }
        }

        protected override void _unbindExUI()
        {
            if (closeBtn)
            {
                closeBtn.onClick.RemoveListener(CloseBtnClick);
            }
            closeBtn = null;
            if (openVipBtn)
            {
                openVipBtn.onClick.RemoveListener(OpenVipBtnClick);
            }
            openVipBtn = null;
        }

        protected override void _OnOpenFrame()
        {

        }

        protected override void _OnCloseFrame()
        {
            
        }

        void CloseBtnClick()
        {
            frameMgr.CloseFrame(this);
        }

        void OpenVipBtnClick()
        {
            this.Close();
            if (frameMgr.IsFrameOpen<VipFrame>() == false)
            {
                frameMgr.OpenFrame<VipFrame>(FrameLayer.Middle,VipTabType.PAY);
            }
        }
    }
}
