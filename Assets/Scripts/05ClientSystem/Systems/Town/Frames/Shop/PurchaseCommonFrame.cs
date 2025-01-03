using UnityEngine;
using System.Collections;
using UnityEngine.UI;
///////删除linq
using System.Collections.Generic;
using Protocol;
using Network;
using ProtoTable;

namespace GameClient
{
    class PurchaseCommonFrame : ClientFrame
    {
        PurchaseCommonData m_kData;

        System.Int32 m_iCurCount = 0;
        List<ItemInfo> m_itemInfoList = new List<ItemInfo>();
        bool m_bIsShowConsumItem = false;
        List<GameObject> ObjList = new List<GameObject>();
        public enum CountLimitType
        {
            CLT_MIN = 0,
            CLT_NORMAL,
            CLT_MAX,
        }

        public enum CountConfigType
        {
            CCT_MIN = 1,
        }

        CountLimitType m_eCountLimitType = CountLimitType.CLT_MIN;
        [UIControl("Middle/count", typeof(InputField))]
        InputField m_inputCount;
        ComItem m_kComItem;
        Text m_kBuyInfo;
        GameObject m_gHint;
        GameObject m_gCount;
        GameObject m_gOldChangeNew;
        GameObject m_gPrefab;
        GameObject m_gDes;
        GameObject m_gContent;
        GameObject m_gScrollView;
        ToggleGroup m_tToggleGroup;
        GameObject m_gDisCountRoot;
        Text m_kDisCount;
        Text mTitleName;
        [UIControl("Bottom/ok/text", typeof(Text))]
        Text m_btnInfo;

        public static void OpenLinkFrame(string strParam)
        {
            if(string.IsNullOrEmpty(strParam))
            {
                return;
            }

            int iGoodID = 0;
            if (!int.TryParse(strParam, out iGoodID))
            {
                return;
            }

            var goodItem = TableManager.GetInstance().GetTableItem<ProtoTable.ShopItemTable>(iGoodID);
            if(null == goodItem)
            {
                return;
            }

            ShopDataManager.GetInstance().OpenShop(goodItem.ShopID, 0, -1, () =>
            {
                var shopData = ShopDataManager.GetInstance().GetGoodsDataFromShop(goodItem.ShopID);
                if (null != shopData)
                {
                    var goodsData = shopData.Goods.Find(x => { return x.ID == iGoodID; });
                    if(null != goodsData)
                    {
                        ClientSystemManager.GetInstance().OpenFrame<PurchaseCommonFrame>(FrameLayer.Middle);
                        UIEvent uiEvent = UIEventSystem.GetInstance().GetIdleUIEvent();
                        if (uiEvent != null)
                        {
                            uiEvent.EventID = EUIEventID.PurchaseCommanUpdate;
                            uiEvent.EventParams.kPurchaseCommonData.iShopID = goodItem.ShopID;
                            uiEvent.EventParams.kPurchaseCommonData.iGoodID = iGoodID;
                            uiEvent.EventParams.kPurchaseCommonData.iItemID = (int)goodsData.ItemData.TableID;
                            uiEvent.EventParams.kPurchaseCommonData.iCount = goodsData.ItemData.Count;
                            UIEventSystem.GetInstance().SendUIEvent(uiEvent);
                        }
                    }
                }
            }, null, ShopFrame.ShopFrameMode.SFM_QUERY_NON_FRAME, 0);
        }

        protected override void _OnOpenFrame()
        {
            m_kComItem = CreateComItem(Utility.FindChild(frame, "Middle/BuyItem"));
            m_kComItem.enabled = false;

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PurchaseCommanUpdate, _OnUpdate);
            m_iCurCount = (int)CountConfigType.CCT_MIN;
            m_eCountLimitType = CountLimitType.CLT_MIN;
            m_inputCount.onValueChanged.RemoveAllListeners();
            m_inputCount.onValueChanged.AddListener(OnValueChanged);
            m_kBuyInfo = Utility.FindComponent<Text>(frame, "Bottom/buyInfo");

            m_kSlider = Utility.FindComponent<Slider>(frame, "Middle/slider");
            m_kSlider.onValueChanged.AddListener(_OnSliderValueChanged);
            m_gHint = Utility.FindGameObject(frame, "Middle/hint");
            m_gCount= Utility.FindGameObject(frame, "Middle/count");
            mTitleName = Utility.FindComponent<Text>(frame, "title/Name");

            m_gOldChangeNew = Utility.FindGameObject(frame, "CostItem/item/OldChangeNew");
            m_gPrefab = Utility.FindGameObject(frame, "CostItem/item/OldChangeNew/ScrollView/Viewport/Content/Prefab");
            m_gPrefab.CustomActive(false);
            m_gScrollView = Utility.FindGameObject(frame, "CostItem/item/OldChangeNew/ScrollView");
            m_gDes = Utility.FindGameObject(frame, "CostItem/item/OldChangeNew/des");
            m_gContent = Utility.FindGameObject(frame, "CostItem/item/OldChangeNew/ScrollView/Viewport/Content");
            m_tToggleGroup = Utility.FindComponent<ToggleGroup>(frame, "CostItem/item/OldChangeNew/ScrollView/Viewport/Content/togleGroup");
            m_gDisCountRoot = Utility.FindGameObject(frame, "Middle/DisCountRoot");
            m_kDisCount = Utility.FindComponent<Text>(frame, "Middle/DisCountRoot/discount");
            m_gDisCountRoot.CustomActive(false);
            _UpdateButtonText();
            _UpdateTitleDesc();
            _InitButtons();

            OnValueChanged("1");
        }

        void _OnSliderValueChanged(float fValue)
        {
            var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(m_kData.iItemID);
            if (item != null)
            {
                var shopData = ShopDataManager.GetInstance().GetGoodsDataFromShop(m_kData.iShopID);
                if (shopData != null)
                {
                    var goodData = shopData.Goods.Find(goodsData => { return goodsData.ID == m_kData.iGoodID; });
                    if (goodData != null)
                    {
                        int iMax = goodData.LimitBuyTimes;
                        int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount((int)goodData.CostItemData.TableID);
                        int iCostCount = goodData.CostItemCount.HasValue ? goodData.CostItemCount.Value : 0;
                        int iCanBuyCount = iHasCount / iCostCount;
                        if (iCostCount == 0)
                        {
                            Logger.LogErrorFormat("iCostCount == 0 ? do you want player get money from server for free? ");
                        }

                        int itemMaxNum = item.MaxNum;

                        if (m_kData.iShopID == 26 && goodData.ItemData.SubType == (int)ItemTable.eSubType.ST_WEAPON_LEASE_TICKET)
                        {
                            itemMaxNum = GetExecutePurchaseMaxNum(item.MaxNum, goodData);
                        }

                        iCanBuyCount = itemMaxNum < iCanBuyCount ? itemMaxNum : iCanBuyCount;
                        if (iMax >= 0)
                        {
                            iCanBuyCount = iMax < iCanBuyCount ? iMax : iCanBuyCount;
                        }

                        if (iCanBuyCount < 1)
                        {
                            iCanBuyCount = 1;
                        }

                        int iCurrentValue = 0;
                        if(int.TryParse(m_inputCount.text,out iCurrentValue))
                        {
                            float fRealValue = 1.0f;
                            int iRealValue = 1;
                            if(iCanBuyCount <= 1)
                            {
                                iRealValue = 1;
                                if(fValue != 1.0f)
                                {
                                    m_kSlider.value = 1.0f;
                                    return;
                                }
                                m_inputCount.text = 1.ToString();
                                return;
                            }

                            iRealValue = (int)(fValue / (1.0f / (iCanBuyCount - 1)) + 0.50f) + 1;
                            if(iRealValue < 1)
                            {
                                iRealValue = 1;
                            }

                            fRealValue = (iRealValue - 1) * 1.0f / (iCanBuyCount - 1);
                            if(fRealValue != fValue)
                            {
                                m_kSlider.value = fRealValue;
                                return;
                            }

                            if (iCurrentValue != iRealValue)
                            {
                                m_inputCount.text = iRealValue.ToString();
                            }
                        }
                    }
                }
            }
        }

        Button btnMin;
        Button btnMax;
        Button btnAdd;
        Button btnMinus;
        UIGray grayMin;
        UIGray grayMax;
        UIGray grayAdd;
        UIGray grayMinus;
        UIGray graySlider;
        void _InitButtons()
        {
            btnMin = Utility.FindComponent<Button>(frame, "Middle/min");
            btnMax = Utility.FindComponent<Button>(frame, "Middle/max");
            btnAdd = Utility.FindComponent<Button>(frame, "Middle/add");
            btnMinus = Utility.FindComponent<Button>(frame, "Middle/minus");
            grayMin = Utility.FindComponent<UIGray>(frame, "Middle/min");
            grayMax = Utility.FindComponent<UIGray>(frame, "Middle/max");
            grayAdd = Utility.FindComponent<UIGray>(frame, "Middle/add");
            grayMinus = Utility.FindComponent<UIGray>(frame, "Middle/minus");
            graySlider = Utility.FindComponent<UIGray>(frame, "Middle/slider/handle");
        }

        protected void OnValueChanged(string value)
        {
            int iCount = 0;
            if (value.Length > 0)
            {
                iCount = System.Int32.Parse(value);
            }
            m_iCurCount = iCount;
            m_eCountLimitType = CountLimitType.CLT_NORMAL;

            _OnValueChanged();
        }

        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PurchaseCommanUpdate, _OnUpdate);
            m_kComItem = null;
            if (m_kSlider != null)
            {
                m_kSlider.onValueChanged.RemoveAllListeners();
                m_kSlider = null;
            }
            
            if (m_itemInfoList != null)
            {
                m_itemInfoList.Clear();
            }

            if (ObjList != null)
            {
                ObjList.Clear();
            }

            m_bIsShowConsumItem = false;
        }

        void _UpdateButtonText()
        {
            if(null != m_btnInfo)
            {
                string mBuyDesc = "";
                if (m_kData.iShopID == 11)
                {
                    mBuyDesc = TR.Value("common_purchase_btn_guild");
                }
                else if (m_kData.iShopID == 26)
                {
                    ItemData mItem = ItemDataManager.GetInstance().GetCommonItemTableDataByID(m_kData.iItemID);
                    if (mItem != null)
                    {
                        if (mItem.SubType == (int)ItemTable.eSubType.ST_WEAPON_LEASE_TICKET)
                        {
                            mBuyDesc = TR.Value("common_purchase_btn_normal");
                        }
                        else
                        {
                            mBuyDesc = TR.Value("common_purchase_btn_lease");
                        }
                    }
                }
                else
                {
                    mBuyDesc = TR.Value("common_purchase_btn_normal");
                }
                m_btnInfo.text = mBuyDesc;
            }
        }

        void _UpdateTitleDesc()
        {
            if (mTitleName != null)
            {
                string mTitleDesc = "";
                ItemData mItem = ItemDataManager.GetInstance().GetCommonItemTableDataByID(m_kData.iItemID);
                if (mItem != null)
                {
                    if (m_kData.iShopID == 26 )
                    {
                        if (mItem.SubType == (int)ItemTable.eSubType.ST_WEAPON_LEASE_TICKET)
                        {
                            mTitleDesc = TR.Value("common_purchase_title_buy");
                        }
                        else
                        {
                            mTitleDesc = TR.Value("common_purchase_title_lease");
                        }
                    }
                    else
                    {
                        mTitleDesc = TR.Value("common_purchase_title_buy");
                    }
                }

                mTitleName.text = mTitleDesc;
            }
        }

        protected void _OnUpdate(UIEvent uiEvent)
        {
            m_kData = uiEvent.EventParams.kPurchaseCommonData;
            _UpdateButtonText();
            _UpdateTitleDesc();
            m_bIsShowConsumItem = true;
            _OnValueChanged();
        }

        [UIEventHandle("title/closeicon")]
        void OnClickClose()
        {
            frameMgr.CloseFrame(this);
        }

        void OnClickCancel()
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("Bottom/ok")]
        void OnClickOK()
        {
            var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(m_kData.iItemID);
            if (item != null)
            {
                var shopData = ShopDataManager.GetInstance().GetGoodsDataFromShop(m_kData.iShopID);
                if (shopData != null)
                {
                    var goodData = shopData.Goods.Find(goodsData => { return goodsData.ID == m_kData.iGoodID; });
                    if (goodData != null)
                    {
                        ItemData oldChangeNewItem = new ItemData(0);

                        if (ShopDataManager.GetInstance()._IsPackHaveExchangeEquipment(goodData.shopItem.ID, EPackageType.WearEquip, ref oldChangeNewItem) &&
                            ShopDataManager.GetInstance()._IsPackHaveExchangeEquipment(goodData.shopItem.ID, EPackageType.Equip, ref oldChangeNewItem))
                        {
                            ExecutePurchaseLogic(goodData);
                            return;
                        }

                        if (ShopDataManager.GetInstance()._IsPackHaveExchangeEquipment(goodData.shopItem.ID, EPackageType.WearEquip, ref oldChangeNewItem))
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("common_purchase_pack_wearequip_have_changeequipmaterials")));
                            return;
                        }

                        if (!ShopDataManager.GetInstance()._IsPackHaveExchangeEquipment(goodData.shopItem.ID, EPackageType.Equip, ref oldChangeNewItem))
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("common_purchase_insufficient_materials", oldChangeNewItem.Name)));
                            return;
                        }

                        bool m_bIsFlag = ShopDataManager.GetInstance()._IsShowOldChangeNew(goodData);

                        if (m_bIsFlag && m_itemInfoList.Count != 0)
                        {
                            ExecutePurchaseLogic(goodData);
                            return;
                        }
                       
                        if (m_bIsFlag && m_itemInfoList.Count == 0)
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("common_purchase_pack_Consum_Item")));
                            return;
                        }
                      

                        ExecutePurchaseLogic(goodData);
                    }
                }
            }
        }

        void ExecutePurchaseLogic(GoodsData goodData)
        {
            int iCostCount = goodData.CostItemCount.HasValue ? goodData.CostItemCount.Value : 0;

            bool bIsFind = ShopDataManager.GetInstance().FindMysteryShopID(m_kData.iShopID);

            if (bIsFind)
            {
                if (goodData.GoodsDisCount.HasValue && goodData.GoodsDisCount.Value < 100 && goodData.GoodsDisCount.Value > 0)
                {
                    iCostCount = goodData.GoodsDisCount.Value  * goodData.CostItemCount.Value / 100;
                }
            }
            
            int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount((int)goodData.CostItemData.TableID);
            int iNeedCost = m_iCurCount * iCostCount;
            var costItem = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)goodData.CostItemData.TableID);
            if (costItem == null)
            {
                Logger.LogErrorFormat("所需要购买的物品ID错误，无法在道具表内找到ID={0},请联系相关策划!", goodData.CostItemData.TableID);
                return;
            }

            if (iNeedCost > iHasCount)
            {
                if (ItemComeLink.IsLinkMoney(goodData.CostItemData.TableID, () => { frameMgr.CloseFrame(this); }))
                {

                }
                else
                {
                    //如果是神秘商店 消耗金币大于已有金币 弹出购买金币弹窗
                    if (bIsFind && (goodData.CostItemData.SubType == (int)ProtoTable.ItemTable.eSubType.GOLD ||
                        goodData.CostItemData.SubType == (int)ProtoTable.ItemTable.eSubType.BindGOLD))
                    {
                        var goodTab = TableManager.GetInstance().GetTableItem<ProtoTable.ShopItemTable>(goodData.ID.Value);
                        if (goodTab == null)
                        {
                            Logger.LogErrorFormat("ShopItemTable no is Find MallGoodID = {0}", goodData.ID.Value);
                        }

                        ShopDataManager.GetInstance().OnGoldBuy(goodTab.MallGoodID);
                    }
                    else
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("common_purchase_insufficient_materials"), costItem.Name));
                    }
                }
                return;
            }

            if (m_iCurCount <= 0)
            {
                return;
            }

            //如果商店是租赁商店并且购买的是好运符
            if (m_kData.iShopID == 26 && goodData.ItemData.SubType == (int)ItemTable.eSubType.ST_WEAPON_LEASE_TICKET)
            {
                int mLuckCharmTatleNum = GetLuckCharmMaxNumber(goodData.ItemData);
                int mSelfLuckCharmNum = (int)PlayerBaseData.GetInstance().WeaponLeaseTicket;
                int mDiff = mLuckCharmTatleNum - mSelfLuckCharmNum;
                if (m_iCurCount > mDiff)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("common_purchase_buy_lose",goodData.ItemData.GetColorName(), mLuckCharmTatleNum));
                }
                else
                {
                    SendBuyGoods(goodData, iNeedCost);
                }

                return;
            }

            SendBuyGoods(goodData, iNeedCost);
            return;
        }

        int GetExecutePurchaseMaxNum(int original, GoodsData goodData)
        {
            int max = original;
            int mLuckCharmTatleNum = GetLuckCharmMaxNumber(goodData.ItemData);
            int mSelfLuckCharmNum = (int)PlayerBaseData.GetInstance().WeaponLeaseTicket;
            int mDiff = mLuckCharmTatleNum - mSelfLuckCharmNum;
            if (mDiff <= 0)
            {
                mDiff = 1;
            }
            max = mDiff < original ? mDiff : original;
            return max;
        }
        
        void SendBuyGoods(GoodsData goodData,int iNeedCost)
        {
            CostItemManager.GetInstance().TryCostMoneyDefault(new CostItemManager.CostInfo { nMoneyID = goodData.CostItemData.TableID, nCount = iNeedCost },
               () =>
               {
                   ShopDataManager.GetInstance().BuyGoods(m_kData.iShopID, m_kData.iGoodID, m_iCurCount, m_itemInfoList);
                   MysticalShopBuyGoodsConsumptionOfMoney(goodData);
                   frameMgr.CloseFrame(this);
               });
        }

        /// <summary>
        /// 拿到幸运符最大数量
        /// </summary>
        /// <returns></returns>
        int GetLuckCharmMaxNumber(ItemData item)
        {
            ItemTable mTable = TableManager.GetInstance().GetTableItem<ItemTable>(item.TableID);
            if (mTable != null)
            {
                return mTable.MaxNum;
            }

            return 0;
        }
        

        [UIEventHandle("Middle/min")]
        void OnClickMin()
        {
            m_eCountLimitType = CountLimitType.CLT_MIN;
            _OnValueChanged();
        }

        [UIEventHandle("Middle/max")]
        void OnClickMax()
        {
            m_eCountLimitType = CountLimitType.CLT_MAX;
            _OnValueChanged();
        }

        [UIEventHandle("Middle/add")]
        void OnClickAdd()
        {
            ++m_iCurCount;
            m_eCountLimitType = CountLimitType.CLT_NORMAL;
            _OnValueChanged();
        }

        [UIEventHandle("Middle/minus")]
        void OnClickMinus()
        {
            --m_iCurCount;
            m_eCountLimitType = CountLimitType.CLT_NORMAL;
            _OnValueChanged();
        }

        [UIControl("Middle/Name", typeof(Text))]
        Text m_kName;
        [UIControl("Middle/ScrollView/Viewport/Content/Desc", typeof(Text))]
        Text m_kDesc;
        //[UIControl("CostItem/title/text", typeof(Text))]
        //Text m_kName2;
        [UIControl("CostItem/item/moneyName",typeof(Text))]
        Text moneyName;
        [UIControl("CostItem/item/costnum", typeof(Text))]
        Text m_kCount;
        [UIControl("CostItem/item/icon")]
        Image m_kIcon;
        Slider m_kSlider;

        void _OnValueChanged()
        {
            int iMax = 0;
            int iMin = 1;

            var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(m_kData.iItemID);
            if (item != null)
            {
                var shopData = ShopDataManager.GetInstance().GetGoodsDataFromShop(m_kData.iShopID);
                if(shopData != null)
                {
                    var goodData = shopData.Goods.Find(goodsData => { return goodsData.ID == m_kData.iGoodID; });
                    if(goodData != null)
                    {
                        iMax = goodData.LimitBuyTimes;
                        int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount((int)goodData.CostItemData.TableID);
                        int iCostCount = goodData.CostItemCount.HasValue ? goodData.CostItemCount.Value : 0;

                        m_kComItem.enabled = true;
                        var itemData = GameClient.ItemDataManager.CreateItemDataFromTable(m_kData.iItemID);
                        if (itemData != null)
                        {
                            m_kName.text = itemData.GetColorName();
                            itemData.Count = m_kData.iCount;
                        }

                        m_kComItem.Setup(itemData, null);

                        m_gDisCountRoot.CustomActive(false);

                        bool bIsFind = ShopDataManager.GetInstance().FindMysteryShopID(m_kData.iShopID);
                        if (bIsFind)
                        {
                            if (goodData.GoodsDisCount.HasValue && goodData.GoodsDisCount.Value < 100 && goodData.GoodsDisCount.Value > 0)
                            {
                                iCostCount = goodData.GoodsDisCount.Value  * goodData.CostItemCount.Value / 100;
                                string disCount = TR.Value("shop_item_discount_info", goodData.GoodsDisCount.Value / 10);
                                m_gDisCountRoot.CustomActive(true);
                                m_kDisCount.text = disCount;
                            }
                        }
                       
                        int iCanBuyCount = iHasCount / iCostCount;
                        if (iCostCount == 0)
                        {
                            Logger.LogErrorFormat("iCostCount == 0 ? do you want player get money from server for free? ");
                        }

                        int itemMaxNum = item.MaxNum;

                        if (m_kData.iShopID == 26 && goodData.ItemData.SubType == (int)ItemTable.eSubType.ST_WEAPON_LEASE_TICKET)
                        {
                            itemMaxNum = GetExecutePurchaseMaxNum(item.MaxNum, goodData);
                        }

                        iCanBuyCount = itemMaxNum < iCanBuyCount ? itemMaxNum : iCanBuyCount;

                        if (iMax >= 0)
                        {
                            iCanBuyCount = iMax < iCanBuyCount ? iMax : iCanBuyCount;
                        }

                        if (iCanBuyCount < 1)
                        {
                            iCanBuyCount = 1;
                        }

                        m_kBuyInfo.CustomActive(iMax >= 0);
                        m_kBuyInfo.text = string.Format(TR.Value("shop_buy_limit", iMax));

                        btnMinus.enabled = m_iCurCount > 1;
                        btnMin.enabled = m_iCurCount > 1;
                        btnAdd.enabled = m_iCurCount < iCanBuyCount;
                        btnMax.enabled = m_iCurCount < iCanBuyCount;

                        grayAdd.enabled = !btnAdd.enabled;
                        grayMinus.enabled = !btnMinus.enabled;
                        grayMin.enabled = !btnMin.enabled;
                        grayMax.enabled = !btnMax.enabled;
                        if (graySlider != null)
                        {
                            graySlider.enabled = (!btnAdd.enabled && !btnMinus.enabled);    
                        }
                        

                        if (m_eCountLimitType == CountLimitType.CLT_MIN)
                        {
                            m_iCurCount = iMin;
                        }
                        else if(m_eCountLimitType == CountLimitType.CLT_MAX)
                        {
                            m_iCurCount = iCanBuyCount;
                        }

                        if (m_iCurCount > iCanBuyCount)
                        {
                            m_iCurCount = iCanBuyCount;
                        }
                        if (m_iCurCount < 1)
                        {
                            m_iCurCount = 1;
                        }

                        m_inputCount.text = m_iCurCount.ToString();

                        if (iCanBuyCount > 1)
                        {
                            float fSliderValue = (m_iCurCount - 1) * 1.0f / (iCanBuyCount - 1);
                            if (m_kSlider.value != fSliderValue)
                            {
                                m_kSlider.value = fSliderValue;
                            }
                        }
                        else
                        {
                            if (m_kSlider.value != 1.0f)
                            {
                                m_kSlider.value = 1.0f;
                            }
                        }

                       
                        //m_kName2.text = string.Format(TR.Value("common_purchase_name"), goodData.CostItemData.Name);
                        m_kCount.text = (iCostCount * m_iCurCount).ToString();
                        m_kDesc.text = item.Description;

                        int iNeedCost = iCostCount * m_iCurCount;
                        m_kCount.color = iHasCount < iNeedCost ? Color.red : new Color(0xF4*1.0f/0xFF, 0xDC * 1.0f / 0xFF, 0x89 * 1.0f / 0xFF, 1.0f);

                        var costItem = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)goodData.CostItemData.TableID);
                        if(costItem != null)
                        {
                            // m_kIcon.sprite = AssetLoader.instance.LoadRes(costItem.Icon, typeof(Sprite)).obj as Sprite;
                            ETCImageLoader.LoadSprite(ref m_kIcon, costItem.Icon);
                            moneyName.text = costItem.Name;
                            List<int> costItemIdList = ShopFrame.ms_money_show_name.ToList();
                            moneyName.CustomActive(costItemIdList.Contains(costItem.ID));
                            m_kIcon.CustomActive(!moneyName.IsActive());
                        }

                        bool m_bIsFlag = ShopDataManager.GetInstance()._IsShowOldChangeNew(goodData);
                        m_gOldChangeNew.CustomActive(m_bIsFlag);
                        if (m_bIsFlag)
                        {
                            ComOldChangeNewItem comOldChangeNewItem = m_gOldChangeNew.GetComponent<ComOldChangeNewItem>();
                            comOldChangeNewItem.SetItemId(goodData.shopItem.ID);

                            if (m_bIsShowConsumItem)
                            {
                                {
                                    List<ulong> mAllConsumeItem = ShopDataManager.GetInstance().GetPackageOldChangeNewEquip(goodData.shopItem.ID);
                                    if (mAllConsumeItem.Count > 0)
                                    {
                                        m_gDes.CustomActive(false);
                                        m_gScrollView.CustomActive(true);

                                        for (int i = 0; i < mAllConsumeItem.Count; i++)
                                        {
                                            ulong uID = mAllConsumeItem[i];
                                            ItemData data = ItemDataManager.GetInstance().GetItem(uID);

                                            GameObject go = GameObject.Instantiate(m_gPrefab) as GameObject;
                                            ObjList.Add(go);
                                            Utility.AttachTo(go, m_gContent);
                                            go.CustomActive(true);
                                            ComCommonBind comBind = go.GetComponent<ComCommonBind>();
                                            GameObject itemParent = comBind.GetGameObject("ItemParent");
                                            ComItem comItem = CreateComItem(itemParent);
                                            comItem.Setup(data, (GameObject obj, ItemData item1) =>
                                            {
                                                ItemTipManager.GetInstance().ShowTip(item1);
                                            });
                                            GameObject selectBG = comBind.GetGameObject("SelectedBG");
                                            Toggle toggle = comBind.GetCom<Toggle>("SelectedToggle");
                                            toggle.group = m_tToggleGroup;
                                            toggle.onValueChanged.RemoveAllListeners();
                                            toggle.onValueChanged.AddListener((isOn) =>
                                            {
                                                selectBG.CustomActive(isOn);

                                                if (isOn)
                                                {
                                                    ItemInfo info = new ItemInfo();
                                                    info.uid = uID;
                                                    info.num = 1;
                                                    _setConsumItemUId(info);
                                                }

                                            });

                                        }
                                        m_bIsShowConsumItem = false;
                                    }
                                    else
                                    {
                                        m_gDes.CustomActive(true);
                                        m_gScrollView.CustomActive(false);
                                    }

                                }
                            }

                        }
                       
                        btnMinus.CustomActive(!m_bIsFlag);
                        btnMin.CustomActive(!m_bIsFlag);
                        btnAdd.CustomActive(!m_bIsFlag);
                        btnMax.CustomActive(!m_bIsFlag);
                        m_kSlider.CustomActive(!m_bIsFlag);
                        m_kBuyInfo.CustomActive(iMax > 0 && !m_bIsFlag);
                        m_gHint.CustomActive(!m_bIsFlag);
                        m_gCount.CustomActive(!m_bIsFlag);

                    }
                }
            }
        }

        void _setConsumItemUId(ItemInfo info)
        {
            m_itemInfoList.Clear();
            m_itemInfoList.Add(info);
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Shop/PurchaseCommonFrame";
        }

        protected void MysticalShopBuyGoodsConsumptionOfMoney(GoodsData goodData)
        {
            int iCostCount = 0;
            bool bIsFind = ShopDataManager.GetInstance().FindMysteryShopID(m_kData.iShopID);
            if (bIsFind)
            {
                string moneyName = goodData.CostItemData.Name;
                if (goodData.GoodsDisCount.HasValue && goodData.GoodsDisCount.Value < 100 && goodData.GoodsDisCount.Value > 0)
                {
                    iCostCount = Mathf.CeilToInt(goodData.GoodsDisCount.Value / 100.0f * goodData.CostItemCount.Value);
                    int disCount = goodData.GoodsDisCount.Value / 10;
                    GameStatisticManager.GetInstance().DoStartMysticalShopBuyDisCountGoodsNumber(m_kData.iGoodID, moneyName, disCount, iCostCount);
                }
                else
                {
                    iCostCount = goodData.CostItemCount.HasValue ? goodData.CostItemCount.Value : 0;
                    GameStatisticManager.GetInstance().DoStartMysticalShopBuyGoodsConsumptionOfMoney(m_kData.iGoodID, moneyName, iCostCount);
                }
            }
        }
    }
}