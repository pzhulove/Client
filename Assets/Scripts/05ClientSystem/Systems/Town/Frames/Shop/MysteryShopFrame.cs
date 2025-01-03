using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;
///////删除linq
using Protocol;

namespace GameClient
{
    class MysteryShopData
    {
        public ShopData mysteryShopData;
    }

    class MysteryShopFrame : ClientFrame
    {

        MysteryShopData data = null;
        
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Shop/MysteryShopFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            data = userData as MysteryShopData;
            InitMerchantInfo();
            InitMysteriousGoodsDate();
            BindUIEvent();
            
        }

        protected sealed override void _OnCloseFrame()
        {
            UnBindUIEvent();
            data = null;
            m_akMysteriousGoodsDataItems.DestroyAllObjects();
        }

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ShopBuyGoodsSuccess, _RereshAllGoods);
            PlayerBaseData.GetInstance().onMoneyChanged += OnMoneyChanged;
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ShopBuyGoodsSuccess, _RereshAllGoods);
            PlayerBaseData.GetInstance().onMoneyChanged -= OnMoneyChanged;
        }

        void OnMoneyChanged(PlayerBaseData.MoneyBinderType eMoneyBinderType)
        {
            _RefreshAllObjects();
        }

        void _RereshAllGoods(UIEvent uiEvent)
        {
            _RefreshAllObjects();
        }

        void _RefreshAllObjects()
        {
            m_akMysteriousGoodsDataItems.RefreshAllObjects(null);
        }

        void InitMerchantInfo()
        {
            ProtoTable.ShopTable shopTable = TableManager.GetInstance().GetTableItem<ProtoTable.ShopTable>(data.mysteryShopData.ID.Value);
            if (shopTable == null)
            {
                return;
            }

            if (shopTable.ShopNpcBody != "")
            {
                ETCImageLoader.LoadSprite(ref mMerchantImage, shopTable.ShopNpcBody);
            }

            mName.text = shopTable.ShopName + ":";

        }

        void InitMysteriousGoodsDate()
        {
            m_MysteriousGoodsParent = Utility.FindGameObject(frame, "middleback/Goods/Scroll View/Viewport/Content");
            m_MysteriousGoodsPrefab = Utility.FindGameObject(frame, "middleback/Goods/Scroll View/Viewport/Content/Prefab");
            m_MysteriousGoodsPrefab.CustomActive(false);

            if (data != null)
            {
                for (int i = 0; i < data.mysteryShopData.Goods.Count; i++)
                {
                    OnAddMysteriousGoodsData(data.mysteryShopData.Goods[i]);
                    MysticalShopGoodsRefreshNumber(data.mysteryShopData.Goods[i]);
                }
            }
           
        }

        CachedObjectDicManager<ulong, MysteriousGoodsDateItem> m_akMysteriousGoodsDataItems = new CachedObjectDicManager<ulong, MysteriousGoodsDateItem>();

        #region MysteriousGoods

        GameObject m_MysteriousGoodsParent;
        GameObject m_MysteriousGoodsPrefab;

        void OnAddMysteriousGoodsData(GoodsData data)
        {
            m_akMysteriousGoodsDataItems.Create((ulong)data.ID, new object[] { m_MysteriousGoodsParent, m_MysteriousGoodsPrefab, data, this });
        }

        class MysteriousGoodsDateItem: CachedObject
        {
            GameObject goLocal;
            GameObject goPrefab;
            GameObject goParent;
            GoodsData goodsData;
            public GoodsData GoodsData
            {
                get { return goodsData; }
            }
            MysteryShopFrame frame;

            Text name;
            GameObject itemParent;
            ComItem comItem;
            Text buyCount;
            Image ticketIcon;
            Text curPrice;
            Color colorPrice;
            GameObject goodsDisCountInfo;
            // Text disCount;
            Image mImgdisCount;
            Button btBuy;
            UIGray btBuyGray;
            GameObject sellOver;
            GameObject goNormalPrice;

            public sealed override void OnDestroy()
            {
                if (btBuy != null)
                {
                    btBuy.onClick.RemoveAllListeners();
                    btBuy = null;
                }
               
                if (comItem != null)
                {
                    comItem.Setup(null, null);
                    comItem = null;
                }
            }

            public sealed override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                goodsData = param[2] as GoodsData;
                frame = param[3] as MysteryShopFrame;
                if (goLocal == null)
                {
                    goLocal = GameObject.Instantiate(goPrefab);
                    Utility.AttachTo(goLocal, goParent);
                    ComCommonBind comBind = goLocal.GetComponent<ComCommonBind>();
                    if (comBind != null)
                    {
                        name = comBind.GetCom<Text>("name");
                        itemParent = comBind.GetGameObject("ItemParent");
                        comItem = frame.CreateComItem(itemParent);
                        buyCount = comBind.GetCom<Text>("buyCount");
                        ticketIcon = comBind.GetCom<Image>("TicketIcon");
                        curPrice = comBind.GetCom<Text>("curprice");
                        colorPrice = curPrice.color;
                        goodsDisCountInfo = comBind.GetGameObject("VipCountInfo");
                        // disCount = comBind.GetCom<Text>("discount");
                        mImgdisCount = comBind.GetCom<Image>("ImgDiscount");
                        btBuy = comBind.GetCom<Button>("btBuy");
                        btBuy.onClick.AddListener(OnClickBuy);
                        btBuyGray = comBind.GetCom<UIGray>("btBuyGray");
                        sellOver = comBind.GetGameObject("SellOver");
                        goNormalPrice = comBind.GetGameObject("Horizen");
                    }
                }
                Enable();
                _Update();
            }
            public sealed override void OnRecycle()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }

            public sealed override void OnDecycle(object[] param)
            {
                OnCreate(param);
            }

            public sealed override void OnRefresh(object[] param)
            {
                if (param != null && param.Length > 0)
                {
                    goodsData = param[0] as GoodsData;
                }

                _Update();
            }

            public sealed override void Enable()
            {
               if (null != goLocal)
               {
                    goLocal.CustomActive(true);
               }
            }

            public sealed override void Disable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }

            public sealed override void SetAsLastSibling()
            {
                if (goLocal != null)
                {
                    goLocal.transform.SetAsLastSibling();
                }
            }

            void _Update()
            {
                goodsData.ItemData.userData = goodsData;
                comItem.Setup(goodsData.ItemData, (GameObject obj, ItemData item) =>
                {
                    ItemTipManager.GetInstance().ShowTip(item);
                });
                name.text = goodsData.ItemData.GetColorName();

                ETCImageLoader.LoadSprite(ref ticketIcon, goodsData.CostItemData.Icon);
                bool bMoneyEnough = false;
                int iCurCount = ItemDataManager.GetInstance().GetOwnedItemCount((int)goodsData.CostItemData.TableID);
                goodsDisCountInfo.CustomActive(goodsData.GoodsDisCount.HasValue && goodsData.GoodsDisCount.Value < 100 && goodsData.GoodsDisCount.Value > 0);
                if (goodsData.GoodsDisCount.HasValue && goodsData.GoodsDisCount.Value < 100 && goodsData.GoodsDisCount.Value > 0)
                {
                    int iDiscount = goodsData.GoodsDisCount.Value * goodsData.CostItemCount.Value / 100;
                    //Logger.LogErrorFormat("神秘商店商品，原价{0}，打{1}折，折算出来为{2}", goodsData.GoodsDisCount.Value, goodsData.CostItemCount.Value, goodsData.GoodsDisCount.Value * goodsData.CostItemCount.Value / 100.0f);
                    curPrice.text = iDiscount.ToString();
                    curPrice.color = iCurCount >= iDiscount ? colorPrice : Color.red;
                    // disCount.text = TR.Value("shop_item_discount_info", goodsData.GoodsDisCount.Value / 10);
                    mImgdisCount.SafeSetImage(string.Format("UI/Image/NewPacked/Shenmishangdian.png:Shangdian_Img_Zhekou_0{0}", goodsData.GoodsDisCount.Value / 10));
                    bMoneyEnough = iCurCount >= iDiscount;
                }
                else
                {
                    curPrice.text = goodsData.CostItemCount.Value.ToString();
                    curPrice.color = iCurCount >= goodsData.CostItemCount ? colorPrice : Color.red;
                    bMoneyEnough = iCurCount >= goodsData.CostItemCount;
                }

                buyCount.CustomActive(goodsData.LimitBuyTimes > 0);
                buyCount.text = string.Format(TR.Value("shop_mysteryshop_buycount", goodsData.LimitBuyTimes, goodsData.TotalLimitBuyTimes));
                btBuy.enabled = !(goodsData.LimitBuyTimes == 0);
                btBuyGray.enabled = !btBuy.enabled;
                sellOver.CustomActive(goodsData.LimitBuyTimes == 0);
                goNormalPrice.CustomActive(!sellOver.activeSelf);

               
            }

            public void OnClickBuy()
            {
                if (goodsData.eGoodsLimitButyType != GoodsLimitButyType.GLBT_NONE)
                {
                    if (goodsData.eGoodsLimitButyType == GoodsLimitButyType.GLBT_TOWER_LEVEL)
                    {
                        int iCurValue = CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_MAX_DEATH_TOWER_LEVEL);
                        if (iCurValue < goodsData.iLimitValue)
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("shop_buy_need_tower_level"), goodsData.iLimitValue));
                            return;
                        }
                    }
                    else if (goodsData.eGoodsLimitButyType == GoodsLimitButyType.GLBT_FIGHT_SCORE)
                    {
                        int iCurValue = SeasonDataManager.GetInstance().seasonLevel;
                        if (iCurValue < goodsData.iLimitValue)
                        {
                            var rankName = SeasonDataManager.GetInstance().GetRankName(goodsData.iLimitValue);
                            SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("shop_buy_need_fight_score"), rankName));
                            return;
                        }
                    }
                    else if (goodsData.eGoodsLimitButyType == GoodsLimitButyType.GLBT_GUILD_LEVEL)
                    {
                        int iCurValue = GuildDataManager.GetInstance().GetBuildingLevel(GuildBuildingType.SHOP);
                        if (iCurValue < goodsData.iLimitValue)
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("shop_buy_need_guild_level"), goodsData.iLimitValue));
                            return;
                        }
                    }
                }

                if (goodsData.VipLimitLevel > 0 && goodsData.VipLimitLevel > PlayerBaseData.GetInstance().VipLevel)
                {
                    SystemNotifyManager.SystemNotify(1800011, () =>
                    {
                        var vipFrame = ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle) as VipFrame;
                        vipFrame.OpenPayTab();
                    });
                    return;
                }
                frame._OnGoodsClicked(goodsData);
            }
        }

        #endregion

        void _OnGoodsClicked(GoodsData goodsData)
        {
            if (!frameMgr.IsFrameOpen<PurchaseCommonFrame>())
            {
                frameMgr.OpenFrame<PurchaseCommonFrame>(FrameLayer.Middle);
            }

            UIEvent uiEvent = UIEventSystem.GetInstance().GetIdleUIEvent();
            if (uiEvent != null)
            {
                uiEvent.EventID = EUIEventID.PurchaseCommanUpdate;

                uiEvent.EventParams.kPurchaseCommonData.iShopID = data.mysteryShopData.ID.HasValue ? data.mysteryShopData.ID.Value : 0;
                uiEvent.EventParams.kPurchaseCommonData.iGoodID = goodsData.ID.HasValue ? goodsData.ID.Value : 0;
                uiEvent.EventParams.kPurchaseCommonData.iItemID = (int)goodsData.ItemData.TableID;
                uiEvent.EventParams.kPurchaseCommonData.iCount = goodsData.ItemData.Count;
                UIEventSystem.GetInstance().SendUIEvent(uiEvent);
            }
        }

		#region ExtraUIBind
		private Text mName = null;
		private Image mMerchantImage = null;
        private Button mAddButton = null;
        // private Button mAddBindGoldBtn = null;
		
		protected sealed override void _bindExUI()
		{
			mName = mBind.GetCom<Text>("Name");
			mMerchantImage = mBind.GetCom<Image>("MerchantImage");
            mAddButton = mBind.GetCom<Button>("Add");
            if (mAddButton != null)
                mAddButton.onClick.AddListener(OnAddClick);
            // mAddBindGoldBtn = mBind.GetCom<Button>("AddBindGold");
            // if (null != mAddBindGoldBtn)
            //     mAddBindGoldBtn.onClick.AddListener(OnAddBindGoldClick);
        }
		
		protected sealed override void _unbindExUI()
		{
			mName = null;
			mMerchantImage = null;
            if (mAddButton != null)
                mAddButton.onClick.RemoveAllListeners();
            mAddButton = null;
            // if (null != mAddBindGoldBtn)
            //     mAddBindGoldBtn.onClick.RemoveAllListeners();
            // mAddBindGoldBtn = null;

        }

        private void OnAddClick()
        {
            int goldId = 0;
            int.TryParse(TR.Value("MysteryShopGoldID"), out goldId);
            ShopDataManager.GetInstance().OnGoldBuy(goldId);
        }

        private void OnAddBindGoldClick()
        {
            int bindGoldId = 0;
            int.TryParse(TR.Value("MysteryShopBindGoldID"), out bindGoldId);
            ShopDataManager.GetInstance().OnGoldBuy(bindGoldId);
        }
		#endregion
        [UIEventHandle("BG/Close")]
        void OnCloseClick()
        {
            frameMgr.CloseFrame(this);
        }

        protected void MysticalShopGoodsRefreshNumber(GoodsData goodsData)
        {
            int goodId = goodsData.ID.HasValue ? goodsData.ID.Value : 0;
            GameStatisticManager.GetInstance().DoStartMysticalShopGoods(goodId);
        }
    }
}
