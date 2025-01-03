using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    /// <summary>
    /// 商店界面 当前状态 数据
    /// </summary>
    public class ShopNewFrameViewData
    {
        public int shopId = 0;
        public int npcId = -1;
        public List<ShopNewMainTabData> totalTabDataList = null;           //MainTab 指 商店界面 右上角的页签数据 （可以为子商店， 也可以为上架的道具类型）
        public ShopNewMainTabData currentSelectedTabData;                  //当前选中的 右上角页签
        public ShopNewMainTabData defaultSelectedTabData;                  //默认选中的 右上角页签
        public int currentSelectedTabIndex;                                //当前选中的页签序号
        public int defaultSelectedTabIndex;                                //默认选中的页签序号 在同类页签列表中的序号
        public ShopNewFilterData currFirstFilterData;                      //当前选中的第一个过滤器
        public ShopNewFilterData currSecondFilterData;                     //当前选中的第二个过滤器
        public int defalutSelectShopItemId;                                 //打开界面时默认选中的道具id
        public ShopNewFrameViewData()
        {
            totalTabDataList = new List<ShopNewMainTabData>();
            currentSelectedTabData = new ShopNewMainTabData();
            defaultSelectedTabData = new ShopNewMainTabData();
            currFirstFilterData = new ShopNewFilterData();
            currSecondFilterData = new ShopNewFilterData();
        }

        public void Clear()
        {
            shopId = 0;
            npcId = 0;
            currentSelectedTabData = null;
            defaultSelectedTabData = null;
            currentSelectedTabIndex = 0;
            defaultSelectedTabIndex = 0;
            currFirstFilterData = null;
            currSecondFilterData = null;
            if (totalTabDataList != null)
            {
                totalTabDataList.Clear();
                totalTabDataList = null;
            }
        }
    }
    
    /// <summary>
    /// 帐号商店主界面
    /// </summary>
    public class AccountShopFrame : ClientFrame
    {
        #region MODEL PARAMS
       

        
        #endregion
        
        #region VIEW PARAMS
        
        
        #endregion
        
        #region PRIVATE METHODS

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/ShopNew/AccountShopFrame";
        }
       
        protected override void _OnOpenFrame()
        {
            if (userData == null)
            {
                return;
            }

            var shopNewParamData = userData as ShopNewParamData;
            if (shopNewParamData == null)
            {
                return;
            }

            _InitFrameData(shopNewParamData);
        }

        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnCloseShopFrame);
            
        }

        private void _InitFrameData(ShopNewParamData paramData)
        {
            if (paramData == null)
            {
                Logger.LogError("[AccountShopFrame] - InitFrameData failed, ShopNewParamData is null");
                return;
            }

            ShopNewFrameViewData frameComData = new ShopNewFrameViewData();
            frameComData.shopId = paramData.ShopId;
            frameComData.defaultSelectedTabData.MainTabType = ShopNewMainTabType.ShopType;
            frameComData.defaultSelectedTabData.Index = paramData.ShopId;
            frameComData.npcId = paramData.NpcId;
            frameComData.defalutSelectShopItemId = paramData.shopItemId;

            //frameComData.shopNewTable = TableManager.GetInstance().GetTableItem<ShopTable>(paramData.ShopId);
            //if (frameComData.shopNewTable == null)
            //{
            //    Logger.LogErrorFormat("[AccountShopFrame] - InitFrameData failed, can not find shop id is {0}", paramData.ShopId);
            //    return;
            //}

            if (paramData.ShopChildId > 0)
            {
                frameComData.defaultSelectedTabData.MainTabType = ShopNewMainTabType.ShopType;
                frameComData.defaultSelectedTabData.Index = paramData.ShopChildId;
            }
            else {
                //商店的类型设置 帐号商店 商品类型可以为 None = 0
                if (paramData.ShopItemType > 0)
                {
                    frameComData.defaultSelectedTabData.MainTabType = ShopNewMainTabType.ItemType;
                    frameComData.defaultSelectedTabData.Index = paramData.ShopItemType;
                }
            }

            if (mAccountShopView != null)
                mAccountShopView.InitShopView(frameComData);
        }

        #region ExtraUIBind
        private AccountShopView mAccountShopView = null;

        protected override void _bindExUI()
        {
            mAccountShopView = mBind.GetCom<AccountShopView>("AccountShopView");
        }

        protected override void _unbindExUI()
        {
            mAccountShopView = null;
        }
        #endregion
        
        #endregion
        
        
        #region  PUBLIC METHODS
        
        #region PUBLIC STATIC METHODS
        #endregion
        
        #endregion
    }
}