using System;
using Protocol;
using UnityEngine.UI;
using ProtoTable;
using UnityEngine;
using Scripts.UI;
using System.Collections.Generic;

namespace GameClient
{

    public class AuctionNewOnShelfItemData
    {
        public ulong PackageItemGuid;       //准备上架道具的ID
        public bool IsTreasure;             //是否为珍品

        public bool IsTimeOverItem;         //是否为过期的道具，再次上架
        public AuctionBaseInfo ItemAuctionBaseInfo;     //过期商品的相关信息

        public int MaxShelfNum;     //最大的架子数量
        public int SelfOnShelfItemNum;  //现在自己上架道具的数量
    }

    //键盘输入的类型
    public enum AuctionNewOnShelfItemKeyBoardInputType
    {
        None = -1,
        TreasureItemPrice = 0,      //珍品的价格
        NormalItemNumber = 1,       //非珍品的数量
        NormalItemPrice = 2,        //非珍品的价格
    }

   
    class AuctionNewOnShelfFrame : ClientFrame
    {
        
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/AuctionNew/AuctionNewOnShelfFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            AuctionNewOnShelfItemData auctionNewOnShelfItemData = null;
            if(userData != null)
                auctionNewOnShelfItemData = userData as AuctionNewOnShelfItemData;

            if (mAuctionNewOnShelfView != null)
                mAuctionNewOnShelfView.InitView(auctionNewOnShelfItemData);

        }

        protected sealed override void _OnCloseFrame()
        {
            AuctionNewDataManager.GetInstance().ResetAuctionNewOnShelfData();
        }

        #region ExtraUIBind
        private AuctionNewOnShelfView mAuctionNewOnShelfView = null;

        protected override void _bindExUI()
        {
            mAuctionNewOnShelfView = mBind.GetCom<AuctionNewOnShelfView>("AuctionNewOnShelfView");
        }

        protected override void _unbindExUI()
        {
            mAuctionNewOnShelfView = null;
        }
        #endregion
        
    }
}

