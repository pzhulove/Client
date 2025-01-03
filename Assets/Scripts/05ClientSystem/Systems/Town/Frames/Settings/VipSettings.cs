using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameClient;
using ProtoTable;
using System;

namespace _Settings
{
    public class VipSettings : SettingsBindUI
    {
        private GameObject vipFrameRoot = null;

        private GameObject node = null;
        protected override string GetCurrGameObjectPath()
        {
            return "UIRoot/UI2DRoot/Middle/SettingPanel/Panel/Contents/vip";
        }

        protected override void InitBind()
        {
            base.InitBind();
            node = mBind.GetGameObject("vipContentNode");
        }

        public VipSettings(GameObject root, ClientFrame frame) : base(root, frame)
        {
        }

        protected override void OnShowOut()
        {
            if (!ClientSystemManager.instance.IsFrameOpen<VipSettingFrame>())
            {
                VipSettingFrame frame = ClientSystemManager.instance.OpenFrame<VipSettingFrame>() as VipSettingFrame;
                if (frame != null)
                {
                    vipFrameRoot = frame.GetFrame();
                    Utility.AttachTo(vipFrameRoot, node);
                }
            }

        }



        protected override void OnHideIn()
        {

        }

        protected override void Close()
        {
            ClientSystemManager.instance.CloseFrame<VipSettingFrame>();
        }
    }
}