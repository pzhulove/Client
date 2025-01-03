using System;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public sealed class MallActivityGiftPackActivityView : MonoBehaviour, IDisposable
    {
        [SerializeField] private RectTransform mTemplateRoot;
        private readonly List<ComItem> mComtItems = new List<ComItem>();
        private GameObject mTemplate;
        private UnityAction mOnBuyClick;
        private int mLastRewardItemId = -1;
        private readonly List<int> mRequestedGiftPackIds = new List<int>();
        private bool mIsInitAvatar = false;
       private List<int> mGiftPackageIdList = new List<int>();   //存贮可以预览的礼包id

        private MallLimitTimeActivity mActivityTable;

        #region 兑换相关
        private Text mBeExchangedItemLeftNumTxt;//被兑换item
        private Text mExchangeLimitNumTxt;//被兑换剩余的次数
        private Button mExchangedBtn;
        private UIGray mExchangedBtnGray;
        MallActivityExchangeParams mMallActivityExchangeParams;
        #endregion

        public void Init(LimitTimeGiftPackDetailModel model, UnityAction onBuyClick)
        {
            if (model.Id == 0)
            {
                Logger.LogError("LimitTimeActivityModel data is empty");
                return;
            }
            var tableData = TableManager.GetInstance().GetTableItem<MallLimitTimeActivity>((int) model.Id);
            if (tableData == null)
            {
                Logger.LogError("在商城限时活动表中找不到id为"+ model.Id + "的活动");
                return;
            }

            mTemplate = AssetLoader.instance.LoadResAsGameObject(tableData.ModePrefabIcon);
            if (mTemplate == null)
            {
                Logger.LogError("加载预制体失败:" + tableData.ModePrefabIcon);
                return;
            }
            mTemplate.transform.SetParent(mTemplateRoot, false);
            mOnBuyClick = onBuyClick;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GetGiftData, _OnGetGiftData);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemNotifyRemoved, _OnUpdateItemCount);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ShopNewBuyGoodsSuccess, _OnShopNewBuyItemSucceed);
            UpdateData(model);
        }



        public void UpdateData(LimitTimeGiftPackDetailModel model)
        {
            if (mTemplate == null)
            {
                Logger.LogError("mTemplate 为空");
                return;
            }

            var tableData = TableManager.GetInstance().GetTableItem<MallLimitTimeActivity>((int)model.Id);

            if (tableData == null)
            {
                Logger.LogError("在商城限时活动表中找不到id为" + model.Id + "的活动");
                return;
            }
            mActivityTable = tableData;
            PreViewDataModel preViewDataModel = new PreViewDataModel();
            preViewDataModel.isCreatItem = false;
            preViewDataModel.preViewItemList = new List<PreViewItemData>();



            mGiftPackageIdList.Clear();

            for (int i = 0; i < model.mRewards.Length; ++i)
            {
                ItemData itemData = ItemDataManager.CreateItemDataFromTable((int)model.mRewards[i].id);

                if (itemData != null)
                {
                    if (itemData.Type == ItemTable.eType.EXPENDABLE || itemData.Type == ItemTable.eType.VirtualPack)
                    {
                        mRequestedGiftPackIds.Add(itemData.TableID);
                        GiftPackDataManager.GetInstance().GetGiftPackItem(itemData.TableID);
                    }

                    if (itemData.Type == ItemTable.eType.FASHION || 
                        (itemData.SubType == (int)ItemTable.eSubType.GiftPackage && itemData.ThirdType == ItemTable.eThirdType.FashionGift) ||
                        itemData.SubType == (int)ItemTable.eSubType.TITLE ||
                        itemData.SubType == (int)ItemTable.eSubType.PetEgg)
                    {
                        PreViewItemData preViewItem = new PreViewItemData();
                        preViewItem.activityId = (int)model.Id;
                        preViewItem.itemId = (int)model.mRewards[i].id;
                        preViewDataModel.preViewItemList.Add(preViewItem);

                        mGiftPackageIdList.Add((int)model.mRewards[i].id);
                    }
                }
            }

            var bind = mTemplate.GetComponent<ComCommonBind>();
            if (bind == null)
            {
                Logger.LogErrorFormat("在预制体上找不到ComCommonBind脚本: " + tableData.ModePrefabIcon);
                return;
            }

            var imageBg = bind.GetCom<Image>("ContentBg");
            ETCImageLoader.LoadSprite(ref imageBg, tableData.BackgroundImgPath);

            var fashionTips = bind.GetCom<Image>("FashionTips");
            ETCImageLoader.LoadSprite(ref fashionTips, tableData.FashionTips);

            var textFashionName = bind.GetCom<Text>("FashionName");
            textFashionName.SafeSetText(tableData.FashionName);

            var buttonBuy = bind.GetCom<Button>("BuyBtn");
            buttonBuy.SafeRemoveAllListener();
            buttonBuy.SafeAddOnClickListener(mOnBuyClick);

            var textTime = bind.GetCom<Text>("Time");
            textTime.SafeSetText(Function.GetDate((int)model.GiftStartTime, (int)model.GiftEndTime));

            var priceIcon = bind.GetCom<Image>("PriceIcon");
            ETCImageLoader.LoadSprite(ref priceIcon, tableData.PricePath);

            Button goMall = bind.GetCom<Button>("GoShop");
            goMall.SafeRemoveAllListener();
            goMall.SafeAddOnClickListener(() =>
            {
                ClientSystemManager.instance.OpenFrame<MallNewFrame>(FrameLayer.Middle, new OutComeData() { MainTab = MallType.FashionMall });
                ClientSystemManager.instance.CloseFrame<LimitTimeActivityFrame>();
            });

            Button tipsBt = bind.GetCom<Button>("CloseTips");
            tipsBt.SafeRemoveAllListener();
            tipsBt.SafeAddOnClickListener(() =>
            {
                tipsBt.CustomActive(false);
            });

            Button fashionBt1 = bind.GetCom<Button>("FashionBt1");
            fashionBt1.onClick.RemoveAllListeners();
            fashionBt1.onClick.AddListener(() =>
            {
                tipsBt.CustomActive(true);
            });

            Button fashionBt2 = bind.GetCom<Button>("FashionBt2");
            Button fashionBt3 = bind.GetCom<Button>("FashionBt3");
            Button fashionBt4 = bind.GetCom<Button>("FashionBt4");
            Button fashionBt5 = bind.GetCom<Button>("FashionBt5");
            Button fashionBt6 = bind.GetCom<Button>("FashionBt6");
            Button fashionBt7 = bind.GetCom<Button>("FashionBt7");
            Button[] fashionButtons = { fashionBt2, fashionBt3, fashionBt4, fashionBt5, fashionBt6, fashionBt7 };

            GameObject item1 = bind.GetGameObject("Item1");
            GameObject item2 = bind.GetGameObject("Item2");
            GameObject tem3i = bind.GetGameObject("Item3");
            GameObject item4 = bind.GetGameObject("Item4");
            GameObject item5 = bind.GetGameObject("Item5");

            GameObject[] Item = { item1, item2, tem3i, item4, item5 };

            Text itemName1 = bind.GetCom<Text>("ItemName1");
            Text itemName2 = bind.GetCom<Text>("ItemName2");
            Text ItemName3 = bind.GetCom<Text>("ItemName3");
            Text ItemName4 = bind.GetCom<Text>("ItemName4");
            Text ItemName5 = bind.GetCom<Text>("ItemName5");
            Text ItemName6 = bind.GetCom<Text>("ItemName6");
            Text ItemName7 = bind.GetCom<Text>("ItemName7");

            Text[] ItemName = new Text[] { itemName1, itemName2, ItemName3, ItemName4, ItemName5, ItemName6, ItemName7 };



            if (tableData.ActivityMode == MallLimitTimeActivity.eActivityMode.Fashion)
            {
                for (int i = 1; i < model.mRewards.Length; i++)
                {
                    if (i < fashionButtons.Length + 1)
                    {
                        int tempI = i;
                        if (fashionButtons[i-1] == null) continue;
                        fashionButtons[i - 1].onClick.RemoveAllListeners();
                        fashionButtons[i - 1].onClick.AddListener(() =>
                        {
                            ItemData RedItemDetailData = ItemDataManager.CreateItemDataFromTable((int)model.mRewards[tempI].id);
                            ItemTipManager.GetInstance().ShowTip(RedItemDetailData);
                            //添加埋点
                            Utility.DoStartFrameOperation("MallActivityGiftPackActivity", string.Format("ItemId/{0}", RedItemDetailData.TableID));

                        });
                    }
                }
            }
            else if (tableData.ActivityMode == MallLimitTimeActivity.eActivityMode.Pet)
            {
                for (int i = 0; i < model.mRewards.Length; i++)
                {
                    if (i < Item.Length)
                    {
                        int tempI = i;
                        if (Item[i] == null) continue;
                        _CreateItem((int) model.mRewards[tempI].id, (int)model.mRewards[tempI].num, ItemName[tempI], Item[i].gameObject.GetComponentInChildren<ComItem>(), Item[i]);
                    }
                }
            }

            Button mPreViewBtn = bind.GetCom<Button>("Preview");
            if (mPreViewBtn != null)
            {
                mPreViewBtn.onClick.RemoveAllListeners();
                mPreViewBtn.onClick.AddListener(() => 
                {
                    ClientSystemManager.GetInstance().OpenFrame<PreviewModelFrame>(FrameLayer.Middle, preViewDataModel);
                });
            }

            //时装礼包的分别穿戴预览
            Button tryOn1 = bind.GetCom<Button>("TryOn1");
            Button tryOn2 = bind.GetCom<Button>("TryOn2");
            Button tryOn3 = bind.GetCom<Button>("TryOn3");
            Button[] tryOnBtns = { tryOn1, tryOn2, tryOn3 };
            if(mGiftPackageIdList != null)
            {
                for (int i = 0; i < mGiftPackageIdList.Count; i++)
                {
                    int tmp = i;
                    if(tmp < tryOnBtns.Length)
                    {
                        tryOnBtns[tmp].SafeRemoveAllListener();
                        tryOnBtns[tmp].SafeAddOnClickListener(() =>
                        {
                            _OnTryOnClick(tmp);
                          
                        });
                    }
                   
                }
            }
           
            _InitButtonTryOn(bind, model.mRewards);

            #region 兑换相关
            if (mActivityTable != null && mActivityTable.IsExhanged == 1)
            {

                mMallActivityExchangeParams = bind.GetCom<MallActivityExchangeParams>("MallActivityExchangeParams");
                mExchangedBtn = bind.GetCom<Button>("ExchangeBtn");
                mExchangedBtnGray = bind.GetCom<UIGray>("ExchangeBtn");
                mBeExchangedItemLeftNumTxt = bind.GetCom<Text>("CountNumTxt");
                mExchangeLimitNumTxt = bind.GetCom<Text>("LeftExchangeNum");
                mExchangedBtn.SafeAddOnClickListener(_OnExchangeBtnClick);
                var paramsData = GetExhcangedParamsData(0);
                if (paramsData != null)
                {
                    ShopNewDataManager.GetInstance().InitShopData(paramsData.ShopId);
                }
                ShowItemLeftNum();
                ShowExchageTime();
            }

            #endregion
        }
        #region 兑换相关
        private void _OnExchangeBtnClick()
        {
            if (mActivityTable != null && mActivityTable.IsExhanged == 1)
            {
                if (mMallActivityExchangeParams != null && mMallActivityExchangeParams.ExchangeGoodParamList != null)
                {
                    var paramData = GetExhcangedParamsData(0);
                    if (paramData != null)
                    {
                        ShopNewDataManager.GetInstance().BuyGoods(paramData.ShopId, paramData.GoodId, paramData.Count, new List<ItemInfo>());
                    }
                }
            }
        }
        private void _OnUpdateItemCount(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;
            uint itemID = (uint)uiEvent.Param1;
            ShowItemLeftNum((int)itemID);
        }

        private void _OnShopNewBuyItemSucceed(UIEvent uiEvent)
        {
            if (uiEvent == null)
                return;
            var shopItemId = (int)uiEvent.Param1;
            ShowExchageTime(shopItemId);
        }
        /// <summary>
        /// 显示兑换的数量
        /// </summary>
        /// <param name="tableId"></param>
        private void ShowItemLeftNum(int tableId = -1)
        {
            if (mActivityTable != null && mActivityTable.IsExhanged == 1)
            {
                MallActivityExchangeParams.ExchangeGoodParams data0 = GetExhcangedParamsData(0);
                if (data0 == null) return;
                var shopItemData = TableManager.GetInstance().GetTableItem<ShopItemTable>(data0.GoodId);
                if (shopItemData == null) return;
                var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(shopItemData.CostItemID);
                if (itemTable == null) return;
                int leftNum = ItemDataManager.GetInstance().GetItemCountInPackage(itemTable.ID);
                if (tableId == -1 || tableId == shopItemData.CostItemID)
                {
                    mBeExchangedItemLeftNumTxt.SafeSetText(string.Format(TR.Value("ExchangeShopItemLeftNumTip"), itemTable.Name, leftNum, shopItemData.CostNum));
                }
                bool isEnable = leftNum >= shopItemData.CostNum;
                SetEnableExchangeBtn(isEnable);

            }
        }

        /// <summary>
        /// 显示剩余的兑换次数
        /// </summary>
        private void ShowExchageTime(int shopItemId = -1)
        {
            if (mActivityTable != null && mActivityTable.IsExhanged == 1)
            {
                MallActivityExchangeParams.ExchangeGoodParams data0 = GetExhcangedParamsData(0);
                if (data0 == null) return;
                if (shopItemId == -1 || shopItemId == data0.GoodId)
                {
                    int limitTime = GetLimitTime(data0.GoodId);
                    if (limitTime == -1)//不限次数
                    {
                        mExchangeLimitNumTxt.CustomActive(false);
                    }
                    else
                    {
                        mExchangeLimitNumTxt.CustomActive(true);
                        mExchangeLimitNumTxt.SafeSetText(string.Format(TR.Value("ExchangeLimitTimeTip"), limitTime));
                    }
                    if (limitTime == 0)
                    {
                        SetEnableExchangeBtn(false);
                    }
                }
            }
        }

        private void SetEnableExchangeBtn(bool enable)
        {
            if (mExchangedBtn != null && mExchangedBtnGray != null)
            {
                mExchangedBtnGray.enabled = !enable;
                mExchangedBtn.interactable = enable;
            }
        }

        /// <summary>
        /// 得到限制的次数
        /// </summary>
        /// <returns></returns>
        private int GetLimitTime(int shopItemId)
        {
            int limitTime = -1;
            MallActivityExchangeParams.ExchangeGoodParams data0 = GetExhcangedParamsData(0);
            if (data0 == null) return -1;
            List<ShopNewShopItemInfo> shopItemList = ShopNewDataManager.GetInstance().GetShopNewNeedShowingShopItemList(data0.ShopId);
            if (shopItemList != null)
            {
                for (int i = 0; i < shopItemList.Count; i++)
                {
                    ShopNewShopItemInfo shopNewShopItemTable = shopItemList[i];
                    if (shopNewShopItemTable != null && shopNewShopItemTable.ShopItemId == shopItemId)
                    {
                        limitTime = shopNewShopItemTable.LimitBuyTimes;
                    }
                }
            }
            return limitTime;
        }
        /// <summary>
        /// 得到兑换的参数
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private MallActivityExchangeParams.ExchangeGoodParams GetExhcangedParamsData(int index)
        {
            MallActivityExchangeParams.ExchangeGoodParams data = null;
            if (mActivityTable != null && mActivityTable.IsExhanged == 1)
            {
                if (mMallActivityExchangeParams != null && mMallActivityExchangeParams.ExchangeGoodParamList != null)
                {
                    if (mMallActivityExchangeParams.ExchangeGoodParamList.Count > index)
                    {
                        data = mMallActivityExchangeParams.ExchangeGoodParamList[index];
                    }
                }
            }
            return data;
        }
        #endregion
        private void _OnTryOnClick(int i)
        {
            if(mGiftPackageIdList!=null && i <mGiftPackageIdList.Count)
            {
                ClientSystemManager.GetInstance().OpenFrame<PlayerTryOnFrame>(FrameLayer.Middle, mGiftPackageIdList[i]);
            }
            
        }

        void _CreateItem(int itemId, int itemNum, Text textName, ComItem comItem, GameObject root)
        {
            ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(itemId);
            if (null == ItemDetailData)
            {
                Logger.LogErrorFormat("ItemData is null");
                return;
            }
            
            if (comItem == null)
            {
                comItem = ComItemManager.Create(root);
                mComtItems.Add(comItem);
            }
            comItem.Setup(ItemDetailData, Utility.OnItemClicked);
            if (mActivityTable != null && mActivityTable.ActivityMode == MallLimitTimeActivity.eActivityMode.Fashion)
            {
                comItem.imgBackGround.enabled = false;
                Image img= comItem.objIconFashion.GetComponent<Image>();
                if(img!=null)
                { 
                    img.enabled = false;
                }
            }

            var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(itemId);

            if (itemNum == 1)
            {
                textName.SafeSetText(ItemDetailData.Name);
            }
            else
            {
                textName.SafeSetText(string.Format("{0} * {1}", itemTableData.Name, itemNum));
            }
        }


        public void Dispose()
        {
            if (mComtItems != null)
            {
                for (int i = 0; i < mComtItems.Count; ++i)
                {
                    ComItemManager.Destroy(mComtItems[i]);
                }
            }
            mComtItems.Clear();
            mLastRewardItemId = -1;
            mRequestedGiftPackIds.Clear();
            mOnBuyClick = null;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GetGiftData, _OnGetGiftData);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemNotifyRemoved, _OnUpdateItemCount);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ShopNewBuyGoodsSuccess, _OnShopNewBuyItemSucceed);
        }

        public void Close()
        {
            Dispose();
            Destroy(gameObject);
        }

        void _InitEquipItem(GiftPackItemData data)
        {
            if (mTemplate == null)
                return;

            var bind = mTemplate.GetComponent<ComCommonBind>();
            if (bind == null)
                return;

            GameObject itemEquip = bind.GetGameObject("ItemEquip");
            if (itemEquip != null)
            {
                _CreateItem(data.ItemID, data.ItemCount, itemEquip.GetComponentInChildren<Text>(), itemEquip.GetComponentInChildren<ComItem>(), itemEquip);
                Button equipButton = bind.GetCom<Button>("ButtonEquip");
                equipButton.SafeRemoveAllListener();
                equipButton.SafeAddOnClickListener(() =>
                {
                    Utility.OnItemClicked(itemEquip, ItemDataManager.CreateItemDataFromTable(data.ItemID));
                    //添加埋点
                    Utility.DoStartFrameOperation("MallActivityGiftPackActivity", string.Format("ItemId/{0}", data.ItemID));
                });
            }

        }
        /// <summary>
        /// 显示翅膀
        /// </summary>
        /// <param name="data"></param>
        void _InitWingItem(GiftPackItemData data)
        {
            if (mTemplate == null)
                return;

            var bind = mTemplate.GetComponent<ComCommonBind>();
            if (bind == null)
                return;

            GameObject wingGo = bind.GetGameObject("Wing");
            if (wingGo != null)
            {
                _CreateItem(data.ItemID, data.ItemCount, wingGo.GetComponentInChildren<Text>(), wingGo.GetComponentInChildren<ComItem>(), wingGo);
                Button equipButton = bind.GetCom<Button>("Wing");
                equipButton.SafeRemoveAllListener();
                equipButton.SafeAddOnClickListener(() =>
                {
                    Utility.OnItemClicked(wingGo, ItemDataManager.CreateItemDataFromTable(data.ItemID));
                });
            }
        }

        void _InitButtonTryOn(ComCommonBind bind, ItemReward[] rewards)
        {
            if (bind == null)
            {
                return;
            }

            var tryOnButton = bind.GetCom<Button>("ButtonTryOn");
            tryOnButton.SafeRemoveAllListener();
            tryOnButton.SafeAddOnClickListener(_OnTryOnButtonClick);
        }

        void _OnTryOnButtonClick()
        {
            if (mLastRewardItemId > 0)
            {
                ItemData itemData = ItemDataManager.CreateItemDataFromTable(mLastRewardItemId);

                if (itemData != null)
                {
                    if (itemData.IsOccupationFit())
                    {
                        _ShowTryOnFrame(mLastRewardItemId);
                    }
                }
            }
        }

        void _ShowTryOnFrame(int itemId)
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<PlayerTryOnFrame>())
            {
                var tryOnFrame = ClientSystemManager.GetInstance().GetFrame(typeof(PlayerTryOnFrame)) as PlayerTryOnFrame;
                if (tryOnFrame != null)
                {
                    tryOnFrame.Reset(itemId);
                }
            }
            else
            {
                ClientSystemManager.GetInstance().OpenFrame<PlayerTryOnFrame>(FrameLayer.Middle, itemId);
            }
        }

        void _OnGetGiftData(UIEvent param)
        {
            if (param == null || param.Param1 == null)
            {
                Logger.LogError("礼包数据为空");
                return;
            }
            GiftPackSyncInfo data = param.Param1 as GiftPackSyncInfo;

            if (data != null)
            {
                for (int i = 0; i < mRequestedGiftPackIds.Count; ++i)
                {
                    if (mRequestedGiftPackIds[i] == data.id)
                    {
                        for (int j = 0; j < data.gifts.Length; ++j)
                        {
                            GiftPackItemData giftTable = GiftPackDataManager.GetGiftDataFromNet(data.gifts[j]);
                            if (giftTable.ItemID > 0 && giftTable.RecommendJobs.Contains(PlayerBaseData.GetInstance().JobTableID))
                            {
                                mLastRewardItemId = giftTable.ItemID;
                                if (mIsInitAvatar)
                                {
                                    _ShowTryOnFrame(mLastRewardItemId);
                                }
                               
                               
                                ItemTable itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(giftTable.ItemID);
                                if (itemTable == null) continue;
                                if (itemTable.SubType==ItemTable.eSubType.FASHION_HAIR&&itemTable.Type==ItemTable.eType.FASHION) //找到翅膀的礼包
                                {
                                    _InitWingItem(giftTable);
                                }
                                else if(itemTable.SubType == ItemTable.eSubType.FASHION_WEAPON && itemTable.Type == ItemTable.eType.FASHION)//時裝武器
                                {
                                    _InitEquipItem(giftTable);
                                }
                                break;
                            }
                        }
                        mRequestedGiftPackIds.RemoveAt(i);
                        break;
                    }
                }
            }

        }

        void OnDestroy()
        {
            Dispose();
        }
    }
}
