using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using ActivityLimitTime;
using Protocol;
using ProtoTable;
using FashionLimitTimeBuy;

namespace GameClient
{
    public class FashionOutComeData
    {
        public List<MallItemInfo> mallItemInfos;
        public FashionMallMainIndex fashionTypeIndex;

        public FashionOutComeData()
        {
            mallItemInfos = new List<MallItemInfo>();
            fashionTypeIndex = FashionMallMainIndex.None;
        }
    }

    public class FashionLimitTimeBuyFrame : ClientFrame
    {
        const string AwardItemPath = "UIFlatten/Prefabs/LimitTimeGift/LimitTimeAwardItem_2";
        const string SelectToBuy_Tip = "请选择一款购买";
        
        GameObject awardParent;
        GameObject chooseItemParent;
        Button buyBtn;
        SetButtonCD mSetButtonCD;
        ToggleGroup toggleGroup;

        List<FashionLimitTimeItem> chooseItems;
        ComItem[] comItems;

        FashionMallMainIndex currFashionTypeIndex = FashionMallMainIndex.None;

        List<MallItemInfo> showMallItemInfos;

        //时装数据管理器 存储数据集合
        Dictionary<uint, MallItemInfo> allFashionInfosById = null;
        Dictionary<int, List<MallItemInfo>> allFashionInfosByType = null;
        
        //选中期限时装 类型数据
        public SelectMallItemInfoData SelectMallItemInfo { get; set; }

        SelectMallItemInfoData sevenData;
        SelectMallItemInfoData monthData;
        SelectMallItemInfoData foreverData;
        
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/LimitTimeGift/FashionLimitTimeBuyFrame";
        }

        protected override void _bindExUI()
        {
            awardParent = mBind.GetGameObject("AwardParent");
            chooseItemParent = mBind.GetGameObject("ChooseItemParent");
            buyBtn = mBind.GetCom<Button>("BuyBtn");
            if (buyBtn)
            {
                buyBtn.onClick.RemoveListener(OnBuyBtnClick);
                buyBtn.onClick.AddListener(OnBuyBtnClick);
            }
            toggleGroup = mBind.GetCom<ToggleGroup>("ChooseItemGroup");
            mSetButtonCD = mBind.GetCom<SetButtonCD>("SetButtonCD");
            FashionLimitTimeBuyManager.GetInstance().haveFashionDiscount = false;
        }

        protected override void _unbindExUI()
        {
            awardParent = null;
            chooseItemParent = null;
            if (buyBtn)
            {
                buyBtn.onClick.RemoveListener(OnBuyBtnClick);
            }
            buyBtn = null;
            toggleGroup = null;
            mSetButtonCD = null;
            FashionLimitTimeBuyManager.GetInstance().haveFashionDiscount = false;
        }

        protected override void _OnOpenFrame()
        {
            //初始化缓存数据
            sevenData = new SelectMallItemInfoData();
            monthData = new SelectMallItemInfoData();
            foreverData = new SelectMallItemInfoData();
            _OnUpdate(1.0f);
            if (userData == null)
            {
                Logger.LogProcessFormat("限时时装购买页面，传入数据为空!!!!!!");
                return;
            }
            FashionOutComeData outData = userData as FashionOutComeData;
            if (outData != null)
            {
                showMallItemInfos = outData.mallItemInfos;
                currFashionTypeIndex = outData.fashionTypeIndex;

                if (currFashionTypeIndex == FashionMallMainIndex.FashionOne)
                {
                    allFashionInfosById = FashionLimitTimeBuy.FashionLimitTimeBuyManager.GetInstance().GetAllLimitTimeFashionById();
                    allFashionInfosByType = FashionLimitTimeBuy.FashionLimitTimeBuyManager.GetInstance().GetAllLimtTimeFashionInfosByType();
                }
                else if (currFashionTypeIndex == FashionMallMainIndex.FashionAll)
                {
                    allFashionInfosById = FashionLimitTimeBuy.FashionLimitTimeBuyManager.GetInstance().GetAllFashionSuitsById();
                    allFashionInfosByType = FashionLimitTimeBuy.FashionLimitTimeBuyManager.GetInstance().GetAllFashionSuitsByType();
                }
            }
            FashionLimitTimeItemManager.instance.Initialize(chooseItemParent);
            chooseItems = new List<FashionLimitTimeItem>();

            InitAwardItems();
        }

        protected override void _OnCloseFrame()
        {
            //清空缓存
            sevenData = null;
            monthData = null;
            foreverData = null;

            showMallItemInfos = null;
            SelectMallItemInfo = null;

            allFashionInfosById = null;
            allFashionInfosByType = null;

            if (comItems != null)
            {
                for (int j = 0; j < comItems.Length; j++)
                {
                    ComItemManager.Destroy(comItems[j]);
                }
                comItems = null;
            }

            if (chooseItems != null)
            {
                for (int i = 0; i < chooseItems.Count; i++)
                {
                    chooseItems[i].Destory();
                }
            }
            chooseItems = null;

            //注意顺序
            FashionLimitTimeItemManager.instance.UnInitialize();
        }
        void OnBuyBtnClick()
        {
            //if(!CanBuy)
            //{
            //    return;
            //}
            if(!mSetButtonCD.BtIsWork)
            {
                return;
            }
            mSetButtonCD.BtIsWork = false;
            if (SelectMallItemInfo != null)
            {
                var priceType = (int)SelectMallItemInfo.MoneyType;
                uint id = 0;
                int price = 0;
                uint[] itemids = null;
                if (SelectMallItemInfo.SelectItemInfos != null)
                {
                    //每次只购买一个选择商城道具
                    if (SelectMallItemInfo.SelectItemInfos.Count == 1)
                    {
                        id = SelectMallItemInfo.SelectItemInfos[0].id;
                        price = (int)SelectMallItemInfo.SelectItemInfos[0].discountprice;
                        var selectItemInfo =  SelectMallItemInfo.SelectItemInfos[0];
                        var giftItems = selectItemInfo.giftItems;
                        itemids = FashionLimitTimeBuyManager.GetInstance().TryGetItemIdsInMallItemInfo(selectItemInfo, (int)currFashionTypeIndex);
                    }
                }
                if (priceType == (int)ItemTable.eSubType.POINT)
                {
                    ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.SendReqBuyFashionInMall(id,price,1,SelectMallItemInfo);
                }
            }
            else
            {
                string selectTobugTip = SelectToBuy_Tip;
                selectTobugTip = TR.Value("fashion_buy_noone_select_tip");
                SystemNotifyManager.SysNotifyTextAnimation(selectTobugTip);
            }
        }


        void InitAwardItems()
        {
            if (showMallItemInfos == null)
                return;

            int fashionItemCount = showMallItemInfos.Count;
            comItems = new ComItem[fashionItemCount];
            for (int i = 0; i < fashionItemCount; i++)
            {
                //重置数据 对应id 
                if (allFashionInfosById != null)
                {
                    if (allFashionInfosById.ContainsKey(showMallItemInfos[i].id))
                        showMallItemInfos[i] = allFashionInfosById[showMallItemInfos[i].id];
                }

                //实例化顺序 调整 for fashionSuit and fashionOne

                var showMallItemInfo = showMallItemInfos[i];
                uint[] itemids = FashionLimitTimeBuyManager.GetInstance().TryGetItemIdsInMallItemInfo(showMallItemInfo, (int)currFashionTypeIndex);
                if (itemids != null)
                {
                    int count2 = itemids.Length;
                    if (comItems != null)
                    {
                        if (comItems.Length < count2)
                        {
                            comItems = new ComItem[count2];
                        }
                    }
                    for (int j = 0; j < count2; j++)
                    {
                        var awardItem = AssetLoader.GetInstance().LoadResAsGameObject(AwardItemPath);
                        if (awardItem != null)
                        {
                            Utility.AttachTo(awardItem, awardParent);
                            var awardImg = awardItem.GetComponentInChildren<Image>();
                            var awardText = awardItem.GetComponentInChildren<Text>();
                            if (awardText == null && awardImg == null)
                                return;
                            var comItemTip = this.CreateComItem(awardImg.gameObject);
                            ItemData itemData = ItemDataManager.CreateItemDataFromTable((int)itemids[j]);
                            if (itemData != null)
                            {
                                awardText.text = itemData.Name;
                                var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>((int)itemids[j]);
                                if (itemTable != null)
                                {
                                    FashionLimitTimeBuy.FashionLimitTimeBuyManager.GetInstance().ResetItemNameColor(itemTable, awardText);
                                }
                                comItemTip.Setup(itemData, (GameObject go, ItemData clickedItemData) =>
                                {
                                    ItemTipManager.GetInstance().ShowTipWithoutModelAvatar(clickedItemData);
                                });
                                comItemTip.SetFashionMaskShow(false);
                            }
                            comItems[j] = comItemTip;
                        }
                    }
                }
            }

            //缓存当前选中时装对应的全部期限种类的数据 
            //生成toggles
            CreateLimitTimeItems(showMallItemInfos);
        }

        void CreateLimitTimeItems(List<MallItemInfo> showInfoList)
        {
            if (showInfoList != null)
            {
                List<MallItemInfo> typeMallInfoList = null;
                for (int i = 0; i < showInfoList.Count; i++)
                {
                    var typeShowInfo = showInfoList[i];
                    int type = typeShowInfo.goodsSubType;
                    if (allFashionInfosByType != null)
                    {
                        if (allFashionInfosByType.TryGetValue(type, out typeMallInfoList))
                        {
                            SaveSameTypeItemInfo(typeMallInfoList);
                        }
                    }
                }
                //时装的各部位类型  存好数据后最后一类中 取一下时装的期限种类数 生成 toggles
                if (typeMallInfoList != null && sevenData != null && monthData != null && foreverData !=null)
                {
                    //排序 - 根据价格升序
                    ReSortTypeMallInfoList(typeMallInfoList);

                    for (int i = 0; i < typeMallInfoList.Count; i++)
                    {
                        FashionLimitTimeItem fashionItem = null;
                        Toggle fashionToggle = null;
                        //传入缓存的三种数据
                        switch(GetItemInfoType(typeMallInfoList[i]))
                        {
                            case FashionLimitTime.SevenDay:
                                fashionItem = CreateLimitTimeItems(sevenData);
                                fashionToggle = fashionItem.GetCurrToggle();
                                break;
                            case FashionLimitTime.OneMonth:
                                fashionItem = CreateLimitTimeItems(monthData);
                                fashionToggle = fashionItem.GetCurrToggle();
                                break;
                            case FashionLimitTime.Forever:
                                fashionItem = CreateLimitTimeItems(foreverData);
                                fashionToggle = fashionItem.GetCurrToggle();
                                //默认选中最大值
                                if (fashionToggle)
                                    fashionToggle.isOn = true;
                                break;
                        }
                        if (fashionToggle && toggleGroup)
                            fashionToggle.group = toggleGroup;
                    }
                }
            }
        }

        void SaveSameTypeItemInfo(List<MallItemInfo> typeMallInfoList)
        {
            if (typeMallInfoList != null)
            {
                for (int j = 0; j < typeMallInfoList.Count; j++)
                {
                    var showInfo = typeMallInfoList[j];
                    if (GetItemInfoType(showInfo) == FashionLimitTime.SevenDay)
                    {
                        sevenData.SelectItemInfos.Add(showInfo);
                        sevenData.FashionTypeIndex = currFashionTypeIndex;
                    }
                    else if (GetItemInfoType(showInfo) == FashionLimitTime.OneMonth)
                    {
                        monthData.SelectItemInfos.Add(showInfo);
                        monthData.FashionTypeIndex = currFashionTypeIndex;
                    }
                    else if (GetItemInfoType(showInfo) == FashionLimitTime.Forever)
                    {
                        foreverData.SelectItemInfos.Add(showInfo);
                        foreverData.FashionTypeIndex = currFashionTypeIndex;
                    }
                }
            }
        }

        FashionLimitTime GetItemInfoType(MallItemInfo mallItem)
        {
            if (mallItem != null)
            {
                uint itemid = FashionLimitTimeBuyManager.GetInstance().TryGetItemIdInMallItemInfo(mallItem, (int)currFashionTypeIndex);

                var itemInTable = TableManager.GetInstance().GetTableItem<ItemTable>((int)itemid);
                if (itemInTable != null)
                {
                    if (itemInTable.TimeLeft == 7 * 24 * 3600)
                    {
                        return FashionLimitTime.SevenDay;
                    }
                    else if (itemInTable.TimeLeft == 30 * 24 * 3600)
                    {
                        return FashionLimitTime.OneMonth;
                    }
                    else if (itemInTable.TimeLeft == 0)
                    {
                        return FashionLimitTime.Forever;
                    }
                }
            }
             return FashionLimitTime.Invalid;
        }

        FashionLimitTimeItem CreateLimitTimeItems(SelectMallItemInfoData selectData)
        {
            FashionLimitTimeItem fashionItem = new FashionLimitTimeItem();
            fashionItem.Init(chooseItemParent, this, selectData);
            if (chooseItems != null)    //用于释放
            {
                chooseItems.Add(fashionItem);
            }
            return fashionItem;
        }


        void ReSortTypeMallInfoList(List<MallItemInfo> typeInfoList)
        {
            if (typeInfoList != null)
            {
                typeInfoList.Sort((x, y) => x.discountprice.CompareTo(y.discountprice));
            }
        }

    }

    class FashionLimitTimeItemManager : MonoSingleton<FashionLimitTimeItemManager>
    {
        public const string FashionLimitTimeItem = "UIFlatten/Prefabs/LimitTimeGift/FashionLimitTimebuyItem";
        private ActivityLTObjectPool<FashionLimitTimeItem> fashionItemPool;
        public void Initialize(GameObject parent)
        {
            Utility.AttachTo(this.gameObject,parent);
            fashionItemPool = new ActivityLTObjectPool<FashionLimitTimeItem>(0, parent, FashionLimitTimeItem);
        }

        public void UnInitialize()
        {
            ReleaseAllFashionItems();
            fashionItemPool = null;
        }

        public GameObject GetFashionItem()
        {
            if (fashionItemPool == null)
                return null;
            return fashionItemPool.GetGameObject();
        }

        public void ReleaseFashionItem(GameObject item)
        {
            if (fashionItemPool != null && item != null)
            {
                fashionItemPool.ReleaseGameObject(item);
            }
        }

        public void ReleaseAllFashionItems()
        {
            if (fashionItemPool != null)
            {
                fashionItemPool.ReleasePool();
            }
        }
    }

    class FashionLimitTimeItem : ActivityLTObject<FashionLimitTimeItem>
    {
        const string SevenDaysImgPath = "UI/Image/NewPacked/Shangcheng.png:Shangcheng_Tubiao_Qitian";
        const string OneMouthImgPath = "UI/Image/NewPacked/Shangcheng.png:Shangcheng_Tubiao_Sstian";
        const string ForeverImgPath = "UI/Image/NewPacked/Shangcheng.png:Shangcheng_Tubiao_Yongjiu";

        SelectMallItemInfoData currItemData;

        protected GameObject goParent;
        protected ClientFrame frame;
        protected ComCommonBind mBind;

        GameObject limitTimeLogo;
        Image limitTimeImg;
        Text priceText;
        Text descText;
        Text mOldPrice;
        Text mFashionDiscount;
        GameObject checkImg;
        Toggle toggle;
        private GameObject mIntergralMallInfoRoot = null;
        private Text mIntergralInfoText = null;

        bool isToggleOn = false;

        public override void Create()
        {
            base.Create();
            goSelf = FashionLimitTimeItemManager.instance.GetFashionItem();
        }

        public override void Destory()
        {
            base.Destory();
            FashionLimitTimeItemManager.instance.ReleaseFashionItem(goSelf);
            Reset();
        }

        public void Init(GameObject parent,ClientFrame frame,SelectMallItemInfoData itemData)
        {
            Create();
            this.goParent = parent;
            this.frame = frame;
            this.currItemData = itemData;
            if (goSelf != null && goParent != null)
            {
                Utility.AttachTo(goSelf,goParent);
                mBind = goSelf.GetComponent<ComCommonBind>();
                if (mBind)
                {
                    limitTimeLogo = mBind.GetGameObject("limitTimeLogo");
                    limitTimeImg = mBind.GetCom<Image>("limitTimeImg");
                    priceText = mBind.GetCom<Text>("priceText");
                    descText = mBind.GetCom<Text>("descText");
                    mOldPrice = mBind.GetCom<Text>("OldPrice");
                    mFashionDiscount = mBind.GetCom<Text>("FashionDiscount");
                    checkImg = mBind.GetGameObject("checkImg");
                    toggle = mBind.GetCom<Toggle>("Toggle");
                    if (toggle)
                    {
                        toggle.onValueChanged.RemoveListener(OnToggleOn);
                        toggle.onValueChanged.AddListener(OnToggleOn);
                        toggle.isOn = false;
                    }
                    mIntergralMallInfoRoot = mBind.GetGameObject("IntergralMallInfoRoot");
                    mIntergralInfoText = mBind.GetCom<Text>("IntergralInfoText");
                }
            }

            SetDataToView();
            InitIntergralInfoRoot();
        }

        public void RefreshView(SelectMallItemInfoData data)
        {
            this.currItemData = data;
            SetDataToView();
        }


        void SetDataToView()
        {
            if (currItemData == null)
                return;
            if (currItemData.SelectItemInfos == null || currItemData.SelectItemInfos.Count <= 0)
                return;
            priceText.text = currItemData.TotalPrice + "";
            mOldPrice.text = currItemData._FashionDiscountPrice + "";
            if(currItemData._Discount == 0)
            {
                mFashionDiscount.CustomActive(false);
                mOldPrice.CustomActive(false);
            }
            else
            {
                FashionLimitTimeBuyManager.GetInstance().haveFashionDiscount = true;
                mFashionDiscount.CustomActive(true);
                mOldPrice.CustomActive(true);
            }
            mFashionDiscount.text = currItemData._Discount + "折";
            List<uint> infoItemIds = new List<uint>();
            if (currItemData.FashionTypeIndex == FashionMallMainIndex.FashionAll)
            {
                var itemGiftDatas = currItemData.SelectItemInfos[0].giftItems;
                if(itemGiftDatas!=null)
                {
                    for(int i = 0;i<itemGiftDatas.Length;i++)
                    {
                        infoItemIds.Add(itemGiftDatas[i].id);
                    }
                }
            }
            else if (currItemData.FashionTypeIndex == FashionMallMainIndex.FashionOne)
            {
                infoItemIds.Add( currItemData.SelectItemInfos[0].itemid);      //取选中的其中一个部位的id 获得itemid 创建toggles
            }

            for (int i = 0; i < infoItemIds.Count; i++)
            {
                CreateItemsById((int)infoItemIds[i]);
            }
        }
		
	    /// <summary>
        /// 初始化积分信息是否显示
        /// </summary>
        private void InitIntergralInfoRoot()
        {
            if (currItemData == null)
                return;

            if (mIntergralMallInfoRoot != null)
                mIntergralMallInfoRoot.CustomActive(currItemData.multiple > 0);

            UpdataIntergralInfo();
        }

        /// <summary>
        /// 更新积分信息
        /// </summary>
        private void UpdataIntergralInfo()
        {
            if (currItemData == null)
                return;

            int price = MallNewUtility.GetTicketConvertIntergalNumnber(currItemData.TotalPrice) * currItemData.multiple;
            string mContent = string.Empty;
            if (currItemData.multiple <= 1)
            {
                mContent = TR.Value("mall_buy_intergral_single_multiple_desc", price);
            }
            else
            {
                mContent = TR.Value("mall_buy_intergral_many_multiple_desc", price, currItemData.multiple);
            }

            if (mIntergralInfoText != null)
                mIntergralInfoText.text = mContent;

        }

        void CreateItemsById(int itemId)
        {
            var itemInTable = TableManager.GetInstance().GetTableItem<ItemTable>(itemId);
            if (itemInTable != null)
            {
                int timeLeft = itemInTable.TimeLeft;
                if (timeLeft <= 0)
                {
                    if (limitTimeLogo)
                    {
                        limitTimeLogo.CustomActive(false);
                    }
                        
                    if (limitTimeImg)
                        ETCImageLoader.LoadSprite(ref limitTimeImg, ForeverImgPath);
                    if (descText)
                        descText.text = TR.Value("fashion_buy_forever_use");//"购买后可永久使用";
                }
                else
                {
                    if (limitTimeLogo)
                    {
                        limitTimeLogo.CustomActive(true);
                    }
                        
                    if (timeLeft == 7 * 24 * 3600)
                    {
                        if (limitTimeImg)
                            ETCImageLoader.LoadSprite(ref limitTimeImg, SevenDaysImgPath);
                        if (descText)
                        {
                            descText.text = TR.Value("fashion_buy_sevenday_use");//"购买后可使用7天\n7天后自动消失";
                            descText.text.Replace("\\n", "\n");
                        }
                    }
                    else if (timeLeft == 30 * 24 * 3600)
                    {
                        if (limitTimeImg)
                            // limitTimeImg.sprite = AssetLoader.GetInstance().LoadRes(OneMouthImgPath, typeof(Sprite)).obj as Sprite;
                            ETCImageLoader.LoadSprite(ref limitTimeImg, OneMouthImgPath);
                        if (descText)
                        {
                            descText.text = TR.Value("fashion_buy_month_use");//"购买后可使用30天\n30天后自动消失";
                            descText.text.Replace("\\n", "\n");
                        }
                    }
                }
            }
        }

        void Reset()
        {
            this.goSelf = null;
            this.goParent = null;
            this.frame = null;
            this.mBind = null;

            limitTimeLogo = null;
            limitTimeImg = null;
            priceText = null;
            descText = null;
            mOldPrice = null;
            mFashionDiscount = null;
            checkImg = null;
            if (toggle)
                toggle.onValueChanged.RemoveListener(OnToggleOn);
            toggle = null;
			
			//去引用啊啊啊！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
            currItemData = null;
            mIntergralMallInfoRoot = null;
            mIntergralInfoText = null;
        }

        void OnToggleOn(bool isOn)
        {
            if (isToggleOn == isOn)
                return;
            isToggleOn = isOn;
            if (isOn)
            {
                var thisFrame = this.frame as FashionLimitTimeBuyFrame;
                if (thisFrame != null)
                {
                    thisFrame.SelectMallItemInfo = currItemData;
                }
            }
        }

        public Toggle GetCurrToggle()
        {
            return toggle;
        }
    }
    
}
