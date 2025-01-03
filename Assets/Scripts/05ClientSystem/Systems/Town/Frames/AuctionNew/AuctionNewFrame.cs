using System;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    //超链接的参数，用于控制页面打开的相关逻辑
    public class AuctionNewUserData
    {
        public AuctionNewMainTabType MainTabType;       //大类页签的选择（我要购买，公示商品，我要寄售
        public int FirstLayerTabId;      //第一级页签的ID
        public int SecondLayerTabId;  //二级页签的ID

        public ulong ItemLinkId = 0;                   //默认选中需要出售的物品
    }


    public class AuctionNewFrame : ClientFrame
    {
        //超链接的参数

        //param : 
        //[0] : 拍卖行中的中的商店主页签类型（1 我要购买，2 公示商品，3 我要寄售)
        //[1] : 第一级页签的ID，对应拍卖行结构表中第一级页签
        //[2] : 第二级页签的ID，对应第一级页签下的二级页签的ID	
        //<type= framename param= 1 | 1 | 103 value= GameClient.AuctionNewFrame >

        public static void OpenLinkFrame(string strParam)
        {
            try
            {
                var strParamArray = strParam.Split(new char[] { '|' });

                if (strParamArray.Length >= 3)
                {
                    ClientSystemManager.GetInstance().CloseFrame<AuctionNewFrame>();

                    var auctionNewUserData = new AuctionNewUserData();
                    auctionNewUserData.MainTabType = (AuctionNewMainTabType)int.Parse(strParamArray[0]);
                    auctionNewUserData.FirstLayerTabId = int.Parse(strParamArray[1]);
                    auctionNewUserData.SecondLayerTabId = int.Parse(strParamArray[2]);
                    ClientSystemManager.GetInstance().OpenFrame<AuctionNewFrame>(FrameLayer.Middle, auctionNewUserData);
                }
                else
                {
                    ClientSystemManager.GetInstance().CloseFrame<AuctionNewFrame>();

                    var auctionNewUserData = new AuctionNewUserData
                    {
                        MainTabType = AuctionNewMainTabType.AuctionBuyType,
                        FirstLayerTabId = 0,
                    };
                    ClientSystemManager.GetInstance().OpenFrame<AuctionNewFrame>(FrameLayer.Middle, auctionNewUserData);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/AuctionNew/AuctionNewFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            AuctionNewDataManager.GetInstance().ResetPreviewOnShelfItemData();

            AuctionNewUserData auctionNewUserData = null;
            //if(userData != null)
            //    auctionNewUserData = userData as AuctionNewUserData;
            if (userData != null)
            {
                auctionNewUserData = userData as AuctionNewUserData;
            }
            else
            {
                auctionNewUserData = AuctionNewDataManager.GetInstance().GetLastTimeUserData();
            }

            if (mAuctionNewView != null)
                mAuctionNewView.InitView(auctionNewUserData);
        }

        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAuctionNewFrameClosed);
            //重置数据
            AuctionNewDataManager.GetInstance().ResetPreviewOnShelfItemData();
        }

        #region ExtraUIBind
        private AuctionNewView mAuctionNewView = null;

        protected override void _bindExUI()
        {
            mAuctionNewView = mBind.GetCom<AuctionNewView>("AuctionNewView");
        }

        protected override void _unbindExUI()
        {
            mAuctionNewView = null;
        }
        #endregion

    }

}
