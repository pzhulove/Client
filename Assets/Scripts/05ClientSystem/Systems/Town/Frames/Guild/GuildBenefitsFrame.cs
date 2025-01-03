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
    public class GuildBenefitsFrame : ClientFrame
    {
        #region val    

        #endregion

        #region ui bind      
        private Button openStorage = null;
        private Button openRedPacket = null;
        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildBenefits";
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
            openStorage = mBind.GetCom<Button>("openStorage");
            openStorage.SafeSetOnClickListener(() => 
            {
                GuildStoreFrame.ansyOpen();
            });

            openRedPacket = mBind.GetCom<Button>("openRedPacket");
            openRedPacket.SafeSetOnClickListener(() => 
            {
                frameMgr.OpenFrame<GuildRedPacketFrame>(FrameLayer.Middle);
            });
        }

        protected override void _unbindExUI()
        {
            openStorage = null;
            openRedPacket = null;
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
