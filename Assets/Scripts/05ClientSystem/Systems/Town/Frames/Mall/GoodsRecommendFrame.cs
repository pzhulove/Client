using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using ProtoTable;
using Protocol;
using Network;
using System;
using System.Collections;

namespace GameClient
{
    enum GoodsRecommendType
    {
        None = 0,
        Stone=1,
        Title=2,
        BuySomeThing=3,
        gold=4,
        fashion=5,
    }


    class GoodsRecommendFrame : ClientFrame
    {
        const int GoodsSum = 7;
        int NowEndTime = 0;
        
        private int NowIndex = -1;//记录现在你在的是哪一个私人订制的页面
        List<MallItemInfo> MyGoods = new List<MallItemInfo>();
        MallItemInfo[] BuyFashion = new MallItemInfo[3];
        RectTransform[] ChildElement = new RectTransform[20];
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Mall/GoodsRecommend";
        }
        protected override void _OnOpenFrame()
        {
            //if (userData != null)
            //{
            //    GoodsStatus = (Goods)userData;
            //}
            if (null == MallDataManager.GetInstance().GoodsRecommend)
            {
                Logger.LogErrorFormat("GoodsRecommend is null");
                return;
            }
            MallNewFrame mallframe = ClientSystemManager.GetInstance().GetFrame(typeof(MallNewFrame)) as MallNewFrame;
            //if(mallframe!=null)
            //{
            //    mallframe.SetActiveGoodsRecommendBt(true);
            //}
            BindUIEvent();
            mTips.CustomActive(true);
            mRemainTimeGO.CustomActive(false);
            MallDataManager.GetInstance().SendGoodsRecommendReq();

            //_initData();
            //BindUIEvent();
            //_getGoodsList();
            //_sortGoodsList();

            //UpdateGoodsData(NowIndex);
        }

        protected override void _OnCloseFrame()
        {
            _ClearData();
            UnBindUIEvent();
            MallNewFrame mallNewFrame = ClientSystemManager.GetInstance().GetFrame(typeof(MallNewFrame)) as MallNewFrame;
            //if (mallframe != null)
            //{
            //    mallframe.SetActiveGoodsRecommendBt(false);
            //}
        }


        protected void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GoodsRecommend, SetActiceGoodsRecommend);
        }

        protected void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GoodsRecommend, SetActiceGoodsRecommend);
        }
        /// <summary>
        /// 私人订制的数据传入
        /// </summary>
        /// <param name="uiEvent"></param>
        void SetActiceGoodsRecommend(UIEvent uiEvent)
        {
            
            _initData();

            _getGoodsList();
            _sortGoodsList();
            mTips.CustomActive(false);
            mRemainTimeGO.CustomActive(true);
            _InitBt();
            UpdateGoodsData(NowIndex);
        }
        private void _initData()
        {
            NowIndex = 0;
            MyGoods.Clear();
            NowEndTime = 0;
            Array.Clear(ChildElement, 0, ChildElement.Length);
            
        }
        protected void _ClearData()
        {
            StopCoroutine(UpdateTime());
            NowIndex = -1;
            MyGoods.Clear();
            Array.Clear(ChildElement, 0, ChildElement.Length);

        }

        private void _getGoodsList()
        {
            for (int i = 0; i < MallDataManager.GetInstance().GoodsRecommend.Count; i++)
            {
                MallItemTable mallitemdata = TableManager.GetInstance().GetTableItem<MallItemTable>((int)MallDataManager.GetInstance().GoodsRecommend[i].id);
                if (mallitemdata == null)
                {
                    Logger.LogErrorFormat("mallitemdata is null");
                    return;
                }
                int PersonalTailID = mallitemdata.PersonalTailID;
                if (PersonalTailID != 0)
                {
                    var tabledata = TableManager.GetInstance().GetTableItem<PersonalTailorTriggerTable>(PersonalTailID);
                    if (tabledata == null)
                    {
                        Logger.LogErrorFormat("PersonalTailorTriggerTable is null");
                        return;
                    }
                    if (tabledata.TypeID == (int)GoodsRecommendType.fashion)
                    {
                        string[] FashionID = mallitemdata.giftpackitems.Split('|');
                        for (int j = 0; j < FashionID.Length; j++)
                        {
                            string[] ID_true = FashionID[i].Split(':');
                            int result_ID = 0;
                            int.TryParse(ID_true[0], out result_ID);
                            var itemtabledata = TableManager.GetInstance().GetTableItem<ItemTable>(result_ID);
                            if (itemtabledata == null)
                            {
                                Logger.LogErrorFormat("itemtabledata is null");
                                return;
                            }
                            if (itemtabledata.TimeLeft == 604800)
                            {
                                BuyFashion[0] = MallDataManager.GetInstance().GoodsRecommend[i];
                                break;
                            }
                            else if (itemtabledata.TimeLeft == 2592000)
                            {
                                BuyFashion[1] = MallDataManager.GetInstance().GoodsRecommend[i];
                                break;
                            }
                            else if(itemtabledata.TimeLeft == 0)
                            {
                                MyGoods.Add(MallDataManager.GetInstance().GoodsRecommend[i]);
                                BuyFashion[2] = MallDataManager.GetInstance().GoodsRecommend[i];
                                break;
                            }
                        }
                    }
                    else
                    {
                        MyGoods.Add(MallDataManager.GetInstance().GoodsRecommend[i]);
                    }
                    
                }
                else
                {
                    Logger.LogErrorFormat("PersonTaler is error:{0}", (int)MallDataManager.GetInstance().GoodsRecommend[i].id);
                }
            }
        }
        //开始时间越大排序在越后面
        private void _sortGoodsList()
        {
            MyGoods.Sort((x, y) =>
            {
                int result;
                if (x.starttime.CompareTo(y.starttime) > 0)
                {
                    result = -1;
                }
                else
                {
                    result = 1;
                }
                return result;
            });
        }
        private void _InitBt()
        {
            if(MyGoods.Count>1)
            {
                mGoRight.gameObject.CustomActive(true);
            }
            mBuy.gameObject.CustomActive(true);
        }
        //下表为index的数据的显示
        private void UpdateGoodsData(int index)
        {
            mGoldTime.CustomActive(false);
            Array.Clear(ChildElement, 0, ChildElement.Length);
            NowEndTime = (int)MyGoods[index].endtime;
            _OnUpdate(MyGoods[index].endtime);
            int PersonalTailID = 0;
            var MallItemdata = TableManager.GetInstance().GetTableItem<MallItemTable>((int)MyGoods[index].id);
            if (MallItemdata != null)
            {
                PersonalTailID = MallItemdata.PersonalTailID;
            }
            else
            {
                Logger.LogErrorFormat("MallItemdata is null");
                return;
            }

            var tabledata = TableManager.GetInstance().GetTableItem<PersonalTailorTriggerTable>(PersonalTailID);
            if (tabledata == null)
            {
                Logger.LogErrorFormat("tabledata is null私人物品配置表");
            }
            else
            {
                if (tabledata.BgPath != "")
                {
                    // mBG.sprite = AssetLoader.instance.LoadRes(tabledata.BgPath, typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref mBG, tabledata.BgPath);
                }
                else
                {
                    var JobTableData = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
                    if (JobTableData == null)
                    {
                        Logger.LogErrorFormat("JobTableData is null From xzl");
                    }
                    string path = JobTableData.GoodsRecommendBG;
                    // mBG.sprite = AssetLoader.instance.LoadRes(path, typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref mBG, path);
                }
            }
            //设置不同的背景图
            
            mGoodsFashion.CustomActive(false);
            mGoodsStone.CustomActive(false);
            mGoodsTitle.CustomActive(false);
            mGoodsBuySomeThing.CustomActive(false);
            //设置不同的界面
            if (tabledata.TypeID == (int)GoodsRecommendType.Stone)
            {
                mBuy.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(373, -220);
                mGoodsStone.CustomActive(true);
                _UpdateLimitTime(index);
                ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable((int)MyGoods[index].itemid);
                if(ItemDetailData==null)
                {
                    Logger.LogErrorFormat("ItemDetailData is null From XZL");
                    return;
                }
                ComItem comitem = mStoreElement.GetComponentInChildren<ComItem>();
                if (comitem == null)
                {
                    comitem = CreateComItem(mStoreElement.gameObject);
                }
                comitem.Setup(ItemDetailData, (GameObject Obj, ItemData sitem) => { OnShowTipsFromJob((int)MyGoods[index].itemid); });
                var ItemTableData = TableManager.GetInstance().GetTableItem<ItemTable>((int)MyGoods[index].itemid);
                if(ItemTableData==null)
                {
                    Logger.LogErrorFormat("From xzl ItemTableData is null");
                    return;
                }
                mStoreName.text = ItemTableData.Name;
            }
            if (tabledata.TypeID == (int)GoodsRecommendType.Title)
            {
                mBuy.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(467, -251);
                mGoodsTitle.CustomActive(true);
                ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable((int)MyGoods[index].itemid);
                if (ItemDetailData == null)
                {
                    Logger.LogErrorFormat("ItemDetailData is null From XZL");
                    return;
                }
                ComItem comitem = mTitleElement.GetComponentInChildren<ComItem>();
                if (comitem == null)
                {
                    comitem = CreateComItem(mTitleElement.gameObject);
                }
                comitem.Setup(ItemDetailData, (GameObject Obj, ItemData sitem) => { OnShowTipsFromJob((int)MyGoods[index].itemid); });
            }
            if (tabledata.TypeID == (int)GoodsRecommendType.BuySomeThing)
            {
                mBuy.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(375, -296);
                mGoodsBuySomeThing.CustomActive(true);
                ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable((int)MyGoods[index].itemid);
                if (ItemDetailData == null)
                {
                    Logger.LogErrorFormat("ItemDetailData is null From XZL");
                    return;
                }
                ComItem comitem = mSomeThingElement.GetComponentInChildren<ComItem>();
                if (comitem == null)
                {
                    comitem = CreateComItem(mSomeThingElement.gameObject);
                }
                comitem.Setup(ItemDetailData, (GameObject Obj, ItemData sitem) => { OnShowTipsFromJob((int)MyGoods[index].itemid); });
                var ItemTableData = TableManager.GetInstance().GetTableItem<ItemTable>((int)MyGoods[index].itemid);
                if(ItemTableData==null)
                {
                    Logger.LogErrorFormat("ItemTableData is null From XZL");
                    return;
                }
                mSomeThingName.text = ItemTableData.Name;
            }
            if (tabledata.TypeID == (int)GoodsRecommendType.gold)
            {
                mGoldTime.CustomActive(true);
                mBuy.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(587, -299);
            }
            if (tabledata.TypeID == (int)GoodsRecommendType.fashion)
            {
                RectTransform[] Fashionelement = new RectTransform[5];
                //Array.Clear(Fashionelement, 0, Fashionelement.Length);
                Fashionelement = null;
                MallItemTable mallitemdata = TableManager.GetInstance().GetTableItem<MallItemTable>((int)MyGoods[index].id);
                if(mallitemdata==null)
                {
                    Logger.LogErrorFormat("mallitemdata is null from xzl");
                    return; 
                }
                var table = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);

                if(table == null)
                {
                    Logger.LogErrorFormat("Jobtabledata is null From xzl");
                    return;
                }
                if (table.prejob == mallitemdata.jobtype || PlayerBaseData.GetInstance().JobTableID == mallitemdata.jobtype)
                {
                    mGoodsFashion.CustomActive(true);
                    switch(mallitemdata.jobtype)
                    {
                        case 10:
                            //Array.Copy(GuiJianFashionElement, 0, Fashionelement, 0, GuiJianFashionElement.Length);
                            Fashionelement = GuiJianFashionElement;
                            mBuy.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(344, -303);
                            mGuiJian.CustomActive(true);
                            mNanQiang.CustomActive(false);
                            mLuoLi.CustomActive(false);
                            mGeDou.CustomActive(false);
                            mNvQiang.CustomActive(false);
                            break;
                        case 20:
                            //Array.Copy(NanQiangFashionElement, 0, Fashionelement, 0, NanQiangFashionElement.Length);
                            Fashionelement = NanQiangFashionElement;
                            mBuy.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(318, -370);
                            mGuiJian.CustomActive(false);
                            mNanQiang.CustomActive(true);
                            mLuoLi.CustomActive(false);
                            mGeDou.CustomActive(false);
                            mNvQiang.CustomActive(false);
                            break;
                        case 30:
                            //Array.Copy(LuoLiFashionElement, 0, Fashionelement, 0, LuoLiFashionElement.Length);
                            Fashionelement = LuoLiFashionElement;
                            mBuy.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(316, -338);
                            mGuiJian.CustomActive(false);
                            mNanQiang.CustomActive(false);
                            mLuoLi.CustomActive(true);
                            mGeDou.CustomActive(false);
                            mNvQiang.CustomActive(false);
                            break;
                        case 40:
                            //Array.Copy(GeDouFashionElement, 0, Fashionelement, 0, GeDouFashionElement.Length);
                            Fashionelement = GeDouFashionElement;
                            mBuy.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(345, -304);
                            mGuiJian.CustomActive(false);
                            mNanQiang.CustomActive(false);
                            mLuoLi.CustomActive(false);
                            mGeDou.CustomActive(true);
                            mNvQiang.CustomActive(false);
                            break;
                        case 50:
                            //Array.Copy(NvQiangFashionElement, 0, Fashionelement, 0, NvQiangFashionElement.Length);
                            Fashionelement = NvQiangFashionElement;
                            mBuy.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(261, -352);
                            mGuiJian.CustomActive(false);
                            mNanQiang.CustomActive(false);
                            mLuoLi.CustomActive(false);
                            mGeDou.CustomActive(false);
                            mNvQiang.CustomActive(true);
                            break;
                    }
                    string[] FashionID = mallitemdata.giftpackitems.Split('|');
                    for (int i = 0; i < FashionID.Length; i++)
                    {
                        string[] ID_true = FashionID[i].Split(':');
                        int result_ID = 0;
                        int.TryParse(ID_true[0], out result_ID);
                        ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(result_ID);
                        if(ItemDetailData ==null)
                        {
                            Logger.LogErrorFormat("ItemDetailData is null from XZL");
                            return;
                        }
                        if (Fashionelement[i] == null)
                        {
                            Logger.LogErrorFormat("Fashionelement[{0}] is null", i);
                            return;
                        }

                        var ItemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(result_ID);
                        if(ItemTableData ==null)
                        {
                            Logger.LogErrorFormat("ItemTableData is null From XZL");
                            return;
                            
                        }
                        Button ItemBt = Fashionelement[i].GetComponent<Button>();
                        if(ItemBt==null)
                        {
                            Logger.LogErrorFormat("can not find button in Item");
                            return;
                        }
                        Image ItemImage = Fashionelement[i].GetComponent<Image>();
                        if(ItemImage==null)
                        {
                            Logger.LogErrorFormat("can not find Image in Item");
                            return;
                        }
                        string ImagePath = ItemTableData.Icon;
                        if(ImagePath == null)
                        {
                            Logger.LogErrorFormat("ImagePath is null From XZL");
                            return;
                        }
                        // ItemImage.sprite = AssetLoader.instance.LoadRes(ImagePath, typeof(Sprite)).obj as Sprite;
                        ETCImageLoader.LoadSprite(ref ItemImage, ImagePath);
                        ItemBt.onClick.RemoveAllListeners();
                        ItemBt.onClick.AddListener(() => { OnShowTipsFromJob(result_ID); });
                    }
                }
            }
        }

        //限购次数
        private void _UpdateLimitTime(int index)
        {
            bool bIsDailyLimit = false;
            int LeftLimitNum = Utility.GetLeftLimitNum(MyGoods[index], ref bIsDailyLimit);
            int RightLimitNum = MyGoods[index].limitnum;
            if(bIsDailyLimit)
            {
                mLimitTime.text = string.Format("每日限购:{0}/{1}", LeftLimitNum, RightLimitNum);
            }
            else
            {
                mLimitTime.text = string.Format("限购:{0}/5", LeftLimitNum);
            }
            //mLimitTime.text = bIsDailyLimit ? string.Format("每日限购:{0}/{1}", LeftLimitNum, RightLimitNum) : string.Format("限购:{0}/5", LeftLimitNum);
        }

        void OnShowTipsFromJob(int result)
        {
            ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(result);
            ItemTipManager.GetInstance().ShowTip(ItemDetailData);
        }

        private void _RegistenUIEvent()
        {

        }
        private void _UnRegistenUIEvent()
        {

        }
        public override bool IsNeedUpdate()
        {
            return true;
        }
        protected override void _OnUpdate(float LastTime)
        {
            float lastTime = 0;
            float curTime = 0;
            int SumTime = NowEndTime - (int)TimeManager.GetInstance().GetServerTime();
            int TimeHour = SumTime / 60 / 60;
            int TimeMin = SumTime / 60 % 60;
            int TimeSecond = SumTime % 60;
            string Hour = "";
            string Min = "";
            string Second = "";
            if (TimeHour != 0)
            {
                Hour = string.Format("{0}小时", TimeHour);
            }
            if (TimeMin != 0 || TimeHour != 0)
            {
                Min = string.Format("{0}分", TimeMin);
            }
            if (TimeSecond != 0 || TimeMin != 0 || TimeHour != 0)
            {
                Second = string.Format("{0}秒", TimeSecond);
            }
            mRemainTime.text = string.Format("剩余时间：{0}{1}{2}", Hour, Min, Second);

            //every second update time
            curTime = Time.time;
            if (curTime - lastTime >= 1)
            {
                mTitleTime.text = mRemainTime.text;
                mStoneTime.text = mRemainTime.text;
                mBuySomethingTime.text = mRemainTime.text;
                mGuiJianTime.text = mRemainTime.text;
                mNanQiangTime.text = mRemainTime.text;
                mLuoLiTime.text = mRemainTime.text;
                mGeDouTime.text = mRemainTime.text;
                mNvQiangTime.text = mRemainTime.text;
                lastTime = curTime;
            }
            
        }
        [UIControl("Middle/GoodsFashion/MiddleGuiJian/element{0}", typeof(RectTransform), 1)]
        RectTransform[] GuiJianFashionElement = new RectTransform[5];

        [UIControl("Middle/GoodsFashion/MiddleNanQiang/element{0}",typeof(RectTransform),1)]
        RectTransform[] NanQiangFashionElement = new RectTransform[5];

        [UIControl("Middle/GoodsFashion/MiddleNvQiang/element{0}", typeof(RectTransform), 1)]
        RectTransform[] NvQiangFashionElement = new RectTransform[5];

        [UIControl("Middle/GoodsFashion/MiddleLuoLi/element{0}", typeof(RectTransform), 1)]
        RectTransform[] LuoLiFashionElement = new RectTransform[5];

        [UIControl("Middle/GoodsFashion/MiddleGeDou/element{0}", typeof(RectTransform), 1)]
        RectTransform[] GeDouFashionElement = new RectTransform[5];

        #region ExtraUIBind
        private Button mGoRight = null;
        private Button mGoLeft = null;
        private Text mRemainTime = null;
        private Button mBuy = null;
        private GameObject mStoreElement = null;
        private Text mStoreName = null;
        private Text mLimitTime = null;
        private GameObject mTitleElement = null;
        private GameObject mSomeThingElement = null;
        private Text mSomeThingName = null;
        private Image mBG = null;
        private GameObject mGoodsFashion = null;
        private GameObject mGoodsStone = null;
        private GameObject mGoodsTitle = null;
        private GameObject mGoodsBuySomeThing = null;
        private Button mClose = null;
        private GameObject mTips = null;
        private GameObject mRemainTimeGO = null;
        private Text mTitleTime = null;
        private Text mStoneTime = null;
        private Text mBuySomethingTime = null;
        private Text mGuiJianTime = null;
        private Text mNanQiangTime = null;
        private Text mNvQiangTime = null;
        private Text mLuoLiTime = null;
        private Text mGeDouTime = null;
        private GameObject mGuiJian = null;
        private GameObject mNanQiang = null;
        private GameObject mNvQiang = null;
        private GameObject mLuoLi = null;
        private GameObject mGeDou = null;
        private GameObject mGoldTime = null;

        protected override void _bindExUI()
        {
            mGoRight = mBind.GetCom<Button>("GoRight");
            mGoRight.onClick.AddListener(_onGoRightButtonClick);
            mGoLeft = mBind.GetCom<Button>("GoLeft");
            mGoLeft.onClick.AddListener(_onGoLeftButtonClick);
            mRemainTime = mBind.GetCom<Text>("RemainTime");
            mBuy = mBind.GetCom<Button>("Buy");
            mBuy.onClick.AddListener(_onBuyButtonClick);
            mStoreElement = mBind.GetGameObject("StoreElement");
            mStoreName = mBind.GetCom<Text>("StoreName");
            mLimitTime = mBind.GetCom<Text>("LimitTime");
            mTitleElement = mBind.GetGameObject("TitleElement");
            mSomeThingElement = mBind.GetGameObject("SomeThingElement");
            mSomeThingName = mBind.GetCom<Text>("SomeThingName");
            mBG = mBind.GetCom<Image>("BG");
            mGoodsFashion = mBind.GetGameObject("GoodsFashion");
            mGoodsStone = mBind.GetGameObject("GoodsStone");
            mGoodsTitle = mBind.GetGameObject("GoodsTitle");
            mGoodsBuySomeThing = mBind.GetGameObject("GoodsBuySomeThing");
            mClose = mBind.GetCom<Button>("Close");
            mClose.onClick.AddListener(_onCloseButtonClick);
            mTips = mBind.GetGameObject("Tips");
            mRemainTimeGO = mBind.GetGameObject("RemainTimeGO");
            mTitleTime = mBind.GetCom<Text>("TitleTime");
            mStoneTime = mBind.GetCom<Text>("StoneTime");
            mBuySomethingTime = mBind.GetCom<Text>("BuySomethingTime");
            mGuiJianTime = mBind.GetCom<Text>("GuiJianTime");
            mNanQiangTime = mBind.GetCom<Text>("NanQiangTime");
            mNvQiangTime = mBind.GetCom<Text>("NvQiangTime");
            mLuoLiTime = mBind.GetCom<Text>("LuoLiTime");
            mGeDouTime = mBind.GetCom<Text>("GeDouTime");
            mGuiJian = mBind.GetGameObject("GuiJian");
            mNanQiang = mBind.GetGameObject("NanQiang");
            mNvQiang = mBind.GetGameObject("NvQiang");
            mLuoLi = mBind.GetGameObject("LuoLi");
            mGeDou = mBind.GetGameObject("GeDou");
            mGoldTime = mBind.GetGameObject("GoldTime");
        }

        protected override void _unbindExUI()
        {
            mGoRight.onClick.RemoveListener(_onGoRightButtonClick);
            mGoRight = null;
            mGoLeft.onClick.RemoveListener(_onGoLeftButtonClick);
            mGoLeft = null;
            mRemainTime = null;
            mBuy.onClick.RemoveListener(_onBuyButtonClick);
            mBuy = null;
            mStoreElement = null;
            mStoreName = null;
            mLimitTime = null;
            mTitleElement = null;
            mSomeThingElement = null;
            mSomeThingName = null;
            mBG = null;
            mGoodsFashion = null;
            mGoodsStone = null;
            mGoodsTitle = null;
            mGoodsBuySomeThing = null;
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
            mTips = null;
            mRemainTime = null;
            mTitleTime = null;
            mStoneTime = null;
            mBuySomethingTime = null;
            mGuiJianTime = null;
            mNanQiangTime = null;
            mNvQiangTime = null;
            mLuoLiTime = null;
            mGeDouTime = null;
            mGuiJian = null;
            mNanQiang = null;
            mNvQiang = null;
            mLuoLi = null;
            mGeDou = null;
            mGoldTime = null;
        }
        #endregion

        #region Callback
        private void _onGoRightButtonClick()
        {
            /* put your code in here */
            if (NowIndex < MyGoods.Count - 1)
            {
                UpdateGoodsData(++NowIndex);
                mGoLeft.gameObject.CustomActive(true);
                if(NowIndex==MyGoods.Count-1)
                {
                    mGoRight.gameObject.CustomActive(false);
                }
            }
            else
            {
                return;
            }
        }
        private void _onGoLeftButtonClick()
        {
            /* put your code in here */
            if (NowIndex > 0)
            {
                UpdateGoodsData(--NowIndex);
                mGoRight.gameObject.CustomActive(true);
                if(NowIndex==0)
                {
                    mGoLeft.gameObject.CustomActive(false);
                }
            }
            else
            {
                return;
            }
        }
        private void _onBuyButtonClick()
        {
            /* put your code in here */
            int PersonalTailID = 0;
            var MallItemdata = TableManager.GetInstance().GetTableItem<MallItemTable>((int)MyGoods[NowIndex].id);
            if (MallItemdata != null)
            {
                PersonalTailID = MallItemdata.PersonalTailID;
            }
            else
            {
                Logger.LogErrorFormat("MallItemdata is null");
            }

            var tabledata = TableManager.GetInstance().GetTableItem<PersonalTailorTriggerTable>(PersonalTailID);
            if(tabledata ==null)
            {
                Logger.LogErrorFormat("From xzl tabledata is null");
                return;
            }
            MallItemInfo data = MyGoods[NowIndex];
            if (tabledata.TypeID != (int)GoodsRecommendType.fashion)
            {
                ClientSystemManager.GetInstance().OpenFrame<MallBuyFrame>(FrameLayer.Middle, data);
                
                if (tabledata.TypeID==(int)GoodsRecommendType.Stone)
                {
                    StopCoroutine(UpdateTime());
                    StartCoroutine(UpdateTime());
                }
                
            }
                
            else if (tabledata.TypeID == (int)GoodsRecommendType.fashion)
            {
                List<MallItemInfo> FashionBuyItem = new List<MallItemInfo>();
                
                for(int i=0;i<3;i++)
                {
                    FashionBuyItem.Add(BuyFashion[i]);
                }
                ClientSystemManager.GetInstance().OpenFrame<FashionBuyFrame>(FrameLayer.Middle,FashionBuyItem);
            }
            

            //WorldMallBuy req = new WorldMallBuy();

            //req.itemId = MyGoods[NowIndex].id;
            //req.num = 1;

            //NetManager netMgr = NetManager.Instance();
            //netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }
        IEnumerator UpdateTime()
        {
            yield return Yielders.GetWaitForSeconds(2.0f);
            _UpdateLimitTime(NowIndex);
        }
        private void _onCloseButtonClick()
        {
            /* put your code in here */
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}