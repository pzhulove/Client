using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LimitTimeGift;
using System.Collections.Generic;
using ActivityLimitTime;
using Protocol;
using Network;

namespace GameClient
{
    public class LimitTimeGiftFrame : ClientFrame
    {
        public const string AwardItemTransPath = "UIFlatten/Prefabs/LimitTimeGift/LimitTimeAwardItem";
        const string PayByTicketNotice = "是否确定花费{0}点券购买该礼包？";

        private Button closeBtn;
        private Text giftName;
        private Text limitBuyCount;
        private GameObject awardParent;
        private Text awardDesc;
        private Text awardLastTime;
        private SimpleTimer timer;
        private Button buyBtn;
        private UIGray uiGray;
        private Text buyBtnText;
        private Image buyBtnImg;

        private ComItem[] comItems;
        private LimitTimeGiftData giftData;
        private int countDown;
        //private Coroutine coroutine;

        //初始时 限购次数
        private int limitPurNumTemp;
        private int currLimitPurNum;
        private string purLimitTimeStr;
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/LimitTimeGift/LimitTimeGiftFrame";
        }

        protected override void _bindExUI()
        {
            closeBtn = mBind.GetCom<Button>("closeBtn");
            if (closeBtn)
            {
                closeBtn.onClick.RemoveListener(OnCloseBtnClick);
                closeBtn.onClick.AddListener(OnCloseBtnClick);
            }
            giftName = mBind.GetCom<Text>("giftName");
            limitBuyCount = mBind.GetCom<Text>("limitBuyCount");
            awardParent = mBind.GetGameObject("awardParent");
            awardDesc = mBind.GetCom<Text>("awardDesc");
            awardLastTime = mBind.GetCom<Text>("awardLastTime");
            timer = mBind.GetCom<SimpleTimer>("timer");
            buyBtn = mBind.GetCom<Button>("buyBtn");
            if (buyBtn)
            {
                buyBtn.onClick.RemoveListener(OnBuyBtnClick);
                buyBtn.onClick.AddListener(OnBuyBtnClick);
            }
            uiGray = mBind.GetCom<UIGray>("BtnGray");
            EnabledBuyBtn(true);
            buyBtnText = mBind.GetCom<Text>("buyBtnText");
            buyBtnImg = mBind.GetCom<Image>("buyBtnImage");
        }

        protected override void _unbindExUI()
        {
            if (closeBtn)
                closeBtn.onClick.RemoveListener(OnCloseBtnClick);
            closeBtn = null;
            giftName = null;
            limitBuyCount = null;
            awardParent = null;
            awardDesc = null;
            awardLastTime = null;
            timer = null;
            if (buyBtn)
                buyBtn.onClick.RemoveListener(OnBuyBtnClick);
            buyBtn = null;
            uiGray = null;
            buyBtnText = null;
            buyBtnImg = null;
        }

        protected override void _OnOpenFrame()
        {
            giftData = this.userData as LimitTimeGiftData;
            if (giftData != null)
            {
                limitPurNumTemp = giftData.LimitPurchaseNum;
                SetDataToView();
                /*
                if (giftData.NeedTimeCountDown)
                {
                    countDown = (int)giftData.RemainingTimeSec;
                    coroutine = GameFrameWork.instance.StartCoroutine(RefreshRemainTimeShow());
                }
                 */
            }

            if (ActivityLimitTimeCombineManager.GetInstance().GiftDataManager != null)
            {
                ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.AddItemBuyRetListener(RefreshView);
                ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.AddItemPayRetListener(RefreshView);
            }
        }

        protected override void _OnCloseFrame()
        {
            if (comItems != null)
            {
                for (int j = 0; j < comItems.Length; j++)
                {
                    if(comItems[j] != null)
                    {
                        ComItemManager.Destroy(comItems[j]);
                    }
                }
                comItems = null;
            }
            giftData = null;
            if (timer != null)
                timer.StopTimer();

            if (LimitTimeGiftFrameManager.instance != null)
            {
                LimitTimeGiftFrameManager.instance.RemoveCurrShowGiftFrame(this);
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.HasLimitTimeGiftToBuy);
            /*
             *  countDown = 0;
            if (coroutine != null)
            {
                GameFrameWork.instance.StopCoroutine(coroutine);
            }
             * */

            if (ActivityLimitTimeCombineManager.GetInstance().GiftDataManager != null)
            {
                ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.RemoveAllItemBuyRetListener();
                ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.RemoveAllItemPayRetListener();
            }
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            
        }

        void SetDataToView()
        {
            if (giftName)
                giftName.text = giftData.GiftName;
            if (limitBuyCount)
            {
                limitBuyCount.text = "永久限购：" + giftData.LimitPurchaseNum + "次";
            }

            if (giftData.LimitPurchaseNum > 0 || (giftData.GiftType != MallGoodsType.GIFT_DAILY_REFRESH && giftData.LimitTotalNum > 0))
                EnabledBuyBtn(true);
            else
                EnabledBuyBtn(false);

            if (awardParent)
            {
                if (comItems == null && giftData.GiftAwards != null)
                {
                    int count = giftData.GiftAwards.Count;
                    if (count <= 0)
                        return;

                    comItems = new ComItem[count];
                    GameObject[] awardItemTrans = new GameObject[count];

                    /*
                    if (giftData.GiftType == LimitTimeGiftType.ThreeToOne)
                    {
                        if (giftData.ThreeToOneGifts == null)
                            return;
                        if (giftData.ThreeToOneGifts.Count != count)
                            return;
                    }
                    */
                    for (int i = 0; i < count; i++)
                    {
                        awardItemTrans[i] = //AssetLoader.GetInstance().LoadRes(AwardItemTransPath).obj as GameObject;
                        AssetLoader.instance.LoadResAsGameObject(AwardItemTransPath);
                        if (awardItemTrans[i] != null)
                        {
                            Utility.AttachTo(awardItemTrans[i], awardParent);
                            var awardImg = awardItemTrans[i].GetComponentInChildren<Image>();
                            var awardText = awardItemTrans[i].GetComponentInChildren<Text>();
                            if (awardText == null && awardImg == null)
                                return;
                            comItems[i] = this.CreateComItem(awardImg.gameObject);

                            ItemData itemData = ItemDataManager.CreateItemDataFromTable((int)giftData.GiftAwards[i].AwardId);
                            /*
                            if (giftData.GiftType == LimitTimeGiftType.ThreeToOne)
                            {
                                itemData.PackID = (int)giftData.ThreeToOneGifts[i].GiftId;
                            }
                            */
                            if (itemData != null)
                            {
                                itemData.Count = (int)giftData.GiftAwards[i].AwardCount;
                                itemData.StrengthenLevel = giftData.GiftAwards[i].StrengthLevel;
                                awardText.text = itemData.Name;
                                comItems[i].Setup(itemData, (GameObject go, ItemData itemPa) =>
                                {
                                    /*
                                    if (giftData.GiftType == LimitTimeGiftType.ThreeToOne)
                                    {
                                        List<TipFuncButon> funcs = new List<TipFuncButon>();
                                        TipFuncButon func = new TipFuncButonSpecial();
                                        func.text = "三选一";
                                        func.callback = OnSelectThreeToOneGift;
                                        funcs.Add(func);
                                        ItemTipManager.GetInstance().ShowTip(itemPa, funcs, TextAnchor.MiddleCenter);
                                    }
                                    else
                                    {
                                        ItemTipManager.GetInstance().ShowTip(itemPa);
                                    }*/
                                    ItemTipManager.GetInstance().ShowTip(itemPa);
                                });
                            }
                        }
                    }
                }
            }

            if (awardDesc)
                awardDesc.text = giftData.GiftDesc;

            if (timer)
            {
                if (giftData.NeedTimeCountDown)
                    timer.useSystemUpdate = true;
                else
                    timer.useSystemUpdate = false;

                timer.SetCountdown((int)giftData.RemainingTimeSec);
                if (timer.GetCurrTime4() != null)
                {
                    if (timer.GetCurrTime4().day > 0)
                    {
                        purLimitTimeStr = timer.GetCurrTime4().day + "天";
                    }
                    else
                    {
                        purLimitTimeStr = timer.GetCurrTime4().hour + "小时";
                    }
                }
                timer.StartTimer();
            }
            /*
            if (awardLastTime)
                awardLastTime.text = "剩余时间：" + giftData.RemainingTimeStr;
             * */
            if (buyBtnText)
                buyBtnText.text = "促销价："+giftData.GiftPrice;
            if (buyBtnImg)
                buyBtnImg.gameObject.CustomActive(giftData.PriceType == LimitTimeGiftPriceType.Point ? true : false);


            //判断限购次数
            if (giftData.LimitPurchaseNum > 0 || (giftData.GiftType != MallGoodsType.GIFT_DAILY_REFRESH && giftData.LimitTotalNum > 0))
            {
                EnabledBuyBtn(true);
            }
            else
            {
                EnabledBuyBtn(false);
                if (limitBuyCount)
                {
                    if (giftData.GiftType == MallGoodsType.GIFT_DAILY_REFRESH)
                    {
                        UpdateBuyCount(giftData);
                        //limitBuyCount.text = "今日已购完";
                    }
                    else
                    {
                        UpdateBuyCount(giftData);
                        //limitBuyCount.text = "已购完";
                    }
                }
                if (timer)
                {
                    //timer.SetCountdown(0);
                    //timer.StopTimer();
                }
            }

            currLimitPurNum = giftData.LimitPurchaseNum;
        }

        public uint GetCurrFrameDataId()
        {
            if (giftData == null)
                return 0;
            return giftData.GiftId;
        }
        public void RefreshView(LimitTimeGiftData giftData)
        {
            this.giftData = giftData;
            SetDataToView();
        }

        public void RefreshView()
        {
            if (giftData == null)
                return;
            var giftDatas = ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.GetAllLimitTimeGiftData();
            if (giftDatas != null)
            {
                for (int i = 0; i < giftDatas.Count; i++)
                {
                    if(giftData.GiftId == giftDatas[i].GiftId)
                    {
                        giftData = giftDatas[i];
                        SetDataToView();
                    }
                }
            }
        }

        public void EnabledBuyBtn(bool enabled)
        {
            if (uiGray)
            {
                uiGray.enabled = !enabled;
                if (buyBtn)
                    buyBtn.interactable = enabled;
            }
        }

        /*
        IEnumerator  RefreshRemainTimeShow()
        {
            if (giftData == null)
                yield break;
            while (countDown >= 0)
            {
                yield return Yielders.GetWaitForSecondsRealtime(1f);
                countDown--;
                giftData.RemainingTimeSec = (uint)countDown;
                if (awardLastTime)
                {
                    awardLastTime.text = "剩余时间：" + giftData.RemainingTimeStr;
                }
            }
        }
         * */

        void OnCloseBtnClick()
        {
            if (limitPurNumTemp <= currLimitPurNum)
            {
                CommonNotifyData comData = new CommonNotifyData();
                comData.contentStr = "  限时礼包持续" + purLimitTimeStr + "，过期将无法购买\n关闭界面后可以前往商城限时抢购页面内查看";
                comData.ownerFrame = this;
                ClientSystemManager.instance.OpenFrame<CommonNotifyFrame>(this.GetLayer(), comData);
            }else
            {
                this.Close();
            }
        }

        void OnBuyBtnClick()
        {
            if (giftData != null)
            {
                var priceType = giftData.PriceType;
                var id = giftData.GiftId;
                var price = giftData.GiftPrice;
                if (priceType == LimitTimeGiftPriceType.Point)
                {
                    string notifyCont = string.Format(PayByTicketNotice,price);
                    SystemNotifyManager.SysNotifyMsgBoxOkCancel(notifyCont, () =>
                    {
                        ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.SendReqBuyGiftInMall(id, price, 1);
                    });
                   // ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.SendReqBuyGift(id, 1);
                }
                else if (priceType == LimitTimeGiftPriceType.RMB)
                {
                    PayManager.GetInstance().DoPay((int)id, price, ChargeMallType.Packet);
                }
            }
        }

        void OnSelectThreeToOneGift(ItemData itemData, object data)
        {
            if (itemData != null)
            {
                ClientSystemManager.GetInstance().OpenFrame<SelectItemFrame>(FrameLayer.Middle, itemData);
            }
        }

        //[MessageHandle(WorldMallBuyRet.MsgID)]
        //void OnMallItemBuyRes(MsgDATA msg)
        //{
        //    WorldMallBuyRet res = new WorldMallBuyRet();
        //    res.decode(msg.bytes);

        //    if (res.code == (uint)ProtoErrorCode.SUCCESS)
        //    {
        //        SystemNotifyManager.SysNotifyTextAnimation("购买成功");
        //    }
        //    else
        //    {
        //        SystemNotifyManager.SystemNotify((int)res.code);
        //    }
        //    var currRefreshData = ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.GetLimitTimeGiftDataById(giftData.GiftId);
        //    if (currRefreshData != null)
        //    {
        //        RefreshView(currRefreshData);
        //    }
        //}
        private void UpdateBuyCount(LimitTimeGiftData limitTimeGiftData)
        {
            bool bIsDailyLimit = false;
            MallItemInfo ItemInfo = new MallItemInfo();
            ItemInfo.id = limitTimeGiftData.GiftId;
            ItemInfo.limitnum = (ushort)limitTimeGiftData.LimitNum;
            ItemInfo.limittotalnum = (ushort)limitTimeGiftData.LimitTotalNum;
            ItemInfo.gift = (byte)limitTimeGiftData.GiftType;
            int LeftLimitNum = Utility.GetLeftLimitNum(ItemInfo, ref bIsDailyLimit);

            giftData.LimitPurchaseNum = LeftLimitNum;

            if (LeftLimitNum > 0)
                EnabledBuyBtn(true);
            else
                EnabledBuyBtn(false);
                  
            if (LeftLimitNum > 0)
            {
                limitBuyCount.text = bIsDailyLimit ? string.Format(TR.Value("mall_gift_daily_buy"), LeftLimitNum) : string.Format(TR.Value("mall_gift_buy"), LeftLimitNum);
            }
            else
            {
                limitBuyCount.text = bIsDailyLimit ? TR.Value("mall_gift_daily_empty") : TR.Value("mall_gift_empty");
            }
        }
    }

    public class LimitTimeGiftInMall : ActivityLTObject<LimitTimeGiftInMall>
    {
        protected LimitTimeGiftData currGiftData;

        //protected GameObject goSelf;
        protected GameObject goParent;
        protected ClientFrame frame;
        protected ComCommonBind mBind;

        private Text giftName;
        private Text limitBuyCount;
        private GameObject awardParent;
        private GameObject[] awardItems;
        private Text giftDesc;
        private Text lastTime;
        private SimpleTimer timer;
        private Button buyBtn;
        private UIGray uiGray;
        private Text giftPrice;
        private Image priceTypeImg;

        private ComItem[] comItems;

        public delegate void BuyAction(uint giftId, int giftPrice,LimitTimeGiftPriceType priceType,MallItemInfo mallItemInfoData);
        protected BuyAction buyAction;

        //giftName  limitPurchase   awardParent   giftDesc  remainingTime  buyBtn  giftPrice   priceTypeImg

        public void Init(GameObject parent, ClientFrame mallFrame, LimitTimeGiftData giftData)
        {
            Create();

            this.goParent = parent;
            this.frame = mallFrame;
            this.currGiftData = giftData;

            if (goSelf != null && goParent !=null)
            {
                Utility.AttachTo(goSelf,goParent);
                mBind = goSelf.GetComponent<ComCommonBind>();
                if (mBind)
                {
                    giftName = mBind.GetCom<Text>("giftName");
                    limitBuyCount = mBind.GetCom<Text>("limitPurchase");
                    awardParent = mBind.GetGameObject("awardParent");
                    giftDesc = mBind.GetCom<Text>("giftDesc");
                    lastTime = mBind.GetCom<Text>("remainingTime");
                    timer = mBind.GetCom<SimpleTimer>("timer");
                    buyBtn = mBind.GetCom<Button>("buyBtn");
                    if (buyBtn)
                    {
                        buyBtn.onClick.RemoveListener(OnBuyBtnClick);
                        buyBtn.onClick.AddListener(OnBuyBtnClick);
                    }
                    uiGray = mBind.GetCom<UIGray>("BtnGray");
                    EnabledBuyBtn(true);
                    giftPrice = mBind.GetCom<Text>("giftPrice");
                    priceTypeImg = mBind.GetCom<Image>("priceTypeImg");
                }

                SetDataToView();
            }
        }

        public override void Create()
        {
            base.Create();
            goSelf = LimitTimeGiftMallItemManager.instance.GetMallGiftGo();
        }

        public override void Destory()
        {
            base.Destory();
            LimitTimeGiftMallItemManager.instance.ReleaseMallGiftGo(goSelf);
            if (comItems != null)
            {
                for (int j = 0; j < comItems.Length; j++)
                {
                    ComItemManager.Destroy(comItems[j]);
                }
                comItems = null;
            }
            Reset();
        }

        private void SetDataToView()
        {
            if (currGiftData == null)
                return;
            //if (currGiftData.GiftType == MallGoodsType.INVALID)
            //    return;
            if (giftName)
                giftName.text = currGiftData.GiftName;
            if (limitBuyCount)
            {
                if (currGiftData.GiftType == MallGoodsType.GIFT_DAILY_REFRESH)
                {
                    //limitBuyCount.text = "今日限购次数：" + currGiftData.LimitPurchaseNum + "次";
                    UpdateBuyCount(currGiftData);
                }
                else if (currGiftData.GiftType == MallGoodsType.GIFT_COMMON)
                {
                    limitBuyCount.text = "";
                }
                else
                {
                    // limitBuyCount.text = "永久限购：" + currGiftData.LimitPurchaseNum + "次";
                    UpdateBuyCount(currGiftData);
                }
            }

            if(currGiftData.LimitTotalNum>0)
            {
                int tempNum = 0;
                if (currGiftData.LimitPurchaseNum < currGiftData.LimitLastNum)
                {
                    tempNum = currGiftData.LimitPurchaseNum;
                }
                else
                {
                    tempNum = currGiftData.LimitLastNum;
                }
                if (tempNum > 0)
                {
                    if (currGiftData.GiftType == MallGoodsType.GIFT_DAILY_REFRESH)
                    {
                        //limitBuyCount.text = "今日限购次数：" + currGiftData.LimitPurchaseNum + "次";
                        UpdateBuyCount(currGiftData);
                    }
                    else
                    {
                        //limitBuyCount.text = "限购次数：" + tempNum + "次";
                        UpdateBuyCount(currGiftData);
                    }
                }
                else
                {
                    if (currGiftData.GiftType == MallGoodsType.GIFT_DAILY_REFRESH)
                    {
                        //limitBuyCount.text = "今日已购完";
                        UpdateBuyCount(currGiftData);
                    }
                    else
                    {
                        //limitBuyCount.text = "已购完";
                        UpdateBuyCount(currGiftData);
                    }
                }
            }

            if (awardParent)
            {

                if (comItems == null && currGiftData.GiftAwards != null && frame != null)
                {
                    int count = currGiftData.GiftAwards.Count;
                    comItems = new ComItem[count];
                    awardItems = new GameObject[count];
                    if(awardParent.transform==null)
                    {
                        Logger.LogError("awardParent.transform is null");
                        return;
                    }
                    int childCount = awardParent.transform.childCount;

                    //显示全部挂载点
                    for (int j = 0; j < childCount; j++)
                    {
                        awardParent.transform.GetChild(j).gameObject.CustomActive(true);
                    }

                    if (awardItems == null)
                        return;
                    if (count <= 0)
                        return;

                    if(awardParent != null)
                    {
                        for (int i = 0; i < count; i++)
                        {
							if (awardParent.transform.childCount < count)
                            	return;
                            var child = awardParent.transform.GetChild(i);
                            if (child == null)
                            {
                                Logger.LogError("child is null");
                                continue;
                            }
                            awardItems[i] = awardParent.transform.GetChild(i).gameObject;

                        if (awardItems[i] != null)
                        {
                            //Utility.AttachTo(awardItems[i], awardParent);
                            var awardImg = awardItems[i].GetComponentInChildren<Image>();
                            var awardText = awardItems[i].GetComponentInChildren<Text>();
                            if (awardImg == null)
                                return;
                            if (currGiftData.GiftAwards[i] != null)
                            {
                                comItems[i] = frame.CreateComItem(awardImg.gameObject);
                                var itemData = ItemDataManager.CreateItemDataFromTable((int)currGiftData.GiftAwards[i].AwardId);
                                if (itemData != null)
                                {
                                    itemData.Count = (int)currGiftData.GiftAwards[i].AwardCount;
                                    itemData.StrengthenLevel = currGiftData.GiftAwards[i].StrengthLevel;
                                    awardText.text = itemData.Name;
                                    comItems[i].Setup(itemData, (GameObject go, ItemData itemPa) =>
                                    {
                                        /*
                                        if (currGiftData.GiftType == LimitTimeGiftType.ThreeToOne)
                                        {
                                            List<TipFuncButon> funcs = new List<TipFuncButon>();
                                            TipFuncButon func = new TipFuncButonSpecial();
                                            func.callback = OnSelectThreeToOneGift;
                                            funcs.Add(func);
                                            ItemTipManager.GetInstance().ShowTip(itemPa, funcs, TextAnchor.MiddleCenter);
                                        }
                                        else
                                        {
                                            ItemTipManager.GetInstance().ShowTip(itemPa);
                                        }*/

                                            ItemTipManager.GetInstance().ShowTip(itemPa);
                                        });
                                    }
                                }
                            }
                        }
                    }

                    //隐藏多余挂载点
                    if (childCount > count)
                    {
                        for (int j = childCount - 1; j >= count; j--)
                        {
                            awardParent.transform.GetChild(j).gameObject.CustomActive(false);
                        }
                    }
                }
            }

            if (giftDesc)
                giftDesc.text = currGiftData.GiftDesc;
            if (timer)
            {
                if (currGiftData.NeedTimeCountDown)
                    timer.useSystemUpdate = true;
                else
                    timer.useSystemUpdate = false;
                timer.SetCountdown((int)currGiftData.RemainingTimeSec);
                timer.StartTimer();

                /*
                 if (lastTime)
                 {
                     if (currGiftData.NeedTimeCountDown == false)
                     {
                         lastTime.text = timer.componetText.text;
                     }
                     else
                     {
                         lastTime.text = "剩余时间：" + timer.componetText.text;
                     }
                 }
                 * */
            }
            if (giftPrice)
                giftPrice.text = "促销价：" + currGiftData.GiftPrice;
            if (priceTypeImg)
                priceTypeImg.gameObject.CustomActive(currGiftData.PriceType == LimitTimeGiftPriceType.Point ? true : false);

            //判断限购次数
            if (currGiftData.LimitPurchaseNum > 0)
            {
                EnabledBuyBtn(true);
            }
            else
            {
                if (currGiftData.GiftType == MallGoodsType.GIFT_COMMON)
                    return;
                EnabledBuyBtn(false);
                if (limitBuyCount)
                {
                    if (currGiftData.GiftType == MallGoodsType.GIFT_DAILY_REFRESH)
                    {
                        //limitBuyCount.text = "今日已购完";
                        UpdateBuyCount(currGiftData);
                    }
                    else
                    {
                        //limitBuyCount.text = "已购完";
                        UpdateBuyCount(currGiftData);
                    }
                }
                if (timer)
                {
                    //timer.SetCountdown(0);
                    //timer.StopTimer();
                }
            }
        }

        private void UpdateBuyCount(LimitTimeGiftData limitTimeGiftData)
        {
            bool bIsDailyLimit = false;
            MallItemInfo ItemInfo = new MallItemInfo();
            ItemInfo.id = limitTimeGiftData.GiftId;
            ItemInfo.limitnum = (ushort)limitTimeGiftData.LimitNum;
            ItemInfo.limittotalnum = (ushort)limitTimeGiftData.LimitTotalNum;
            ItemInfo.gift = (byte)limitTimeGiftData.GiftType;
            int LeftLimitNum = Utility.GetLeftLimitNum(ItemInfo, ref bIsDailyLimit);

            currGiftData.LimitPurchaseNum = LeftLimitNum;

            if (LeftLimitNum > 0)
                EnabledBuyBtn(true);
            else
                EnabledBuyBtn(false);

            if (LeftLimitNum > 0)
            {
                limitBuyCount.text = bIsDailyLimit ? string.Format(TR.Value("mall_gift_daily_buy"), LeftLimitNum) : string.Format(TR.Value("mall_gift_buy"), LeftLimitNum);
            }
            else
            {
                limitBuyCount.text = bIsDailyLimit ? TR.Value("mall_gift_daily_empty") : TR.Value("mall_gift_empty");
            }
        }

        public void AddBuyAction(BuyAction buyAct)
        {
            buyAction = buyAct;
        }

        public void EnabledBuyBtn(bool enabled)
        {
            if (uiGray)
            {
                uiGray.enabled = !enabled;
                if (buyBtn)
                    buyBtn.interactable = enabled;
            }
        }
        public uint GetCurrItemDataId()
        {
            if (currGiftData == null)
                return 0;
            return currGiftData.GiftId;
        }

        public void RefreshView(LimitTimeGiftData giftData)
        {
            this.currGiftData = giftData;
            SetDataToView();
        }

        public LimitTimeGiftData GetCurrItemData()
        {
            return currGiftData;
        }


        void OnSelectThreeToOneGift(ItemData itemData,object data)
        {
            if (itemData != null)
            {
                ClientSystemManager.GetInstance().OpenFrame<SelectItemFrame>(FrameLayer.Middle,itemData);
            }
        }

        void Reset()
        {
            giftName = null;
            limitBuyCount = null;
            awardParent = null;
            giftDesc = null;
            lastTime = null;
            if (timer)
                timer.StopTimer();
            timer = null;
            if (buyBtn)
            {
                buyBtn.onClick.RemoveListener(OnBuyBtnClick);
            }
            buyBtn = null;
            uiGray = null;
            giftPrice = null;
            priceTypeImg = null;

            this.goSelf = null;
            this.goParent = null;
            this.frame = null;
            this.currGiftData = null;

            comItems = null;

            buyAction = null;
        }

        void OnBuyBtnClick()
        {
            if (buyAction != null && currGiftData != null)
                buyAction(currGiftData.GiftId, currGiftData.GiftPrice,currGiftData.PriceType,currGiftData.mallItemInfoData);
        }
    }

    public class ActivityLTObject<T>
    {
        protected GameObject goSelf;

        public bool IsUsed { get; private set; }
        public virtual void Create()
        {
            IsUsed = true;
        }
        public virtual void Destory()
        {
            IsUsed = false;
        }

        public GameObject CreatePrefab(GameObject initParent, string prefabResPath)
        {
            if (string.IsNullOrEmpty(prefabResPath))
                return null;
            // goSelf = AssetLoader.instance.LoadResAsGameObject(prefabResPath);
            goSelf = CGameObjectPool.instance.GetGameObject(prefabResPath, enResourceType.UIPrefab, (uint)GameObjectPoolFlag.ReserveLast);
            Utility.AttachTo(goSelf, initParent);
            return goSelf;
        }

        public GameObject GetGoSelf()
        {
            return goSelf;
        }

        private T objType;
        public T ObjType
        {
            get { return objType; }
            set { objType = value; }
        }
    }
    public class ActivityLTObjectPool<T>
    {
        List<ActivityLTObject<T>> actGoList;
        List<GameObject> goList;
        GameObject parent;
        string prefabPath;
        public ActivityLTObjectPool(int initializeNums, GameObject parent, string prefabPath)
        {
            this.parent = parent;
            this.prefabPath = prefabPath;
            actGoList = new List<ActivityLTObject<T>>();
            goList = new List<GameObject>();
            PreWarmPool(initializeNums);
        }

        private void PreWarmPool(int initNum)
        {
            if (initNum <= 0)
                return;
            for (int i = 0; i < initNum; i++)
            {
                var actGo = new ActivityLTObject<T>();
                var go = actGo.CreatePrefab(parent, prefabPath);
                go.CustomActive(false);
                goList.Add(go);
                actGoList.Add(actGo);
            }
        }
        public GameObject GetGameObject()
        {
            GameObject go = null;
            if (goList.Count > 0)
            {
                go = goList[0];
                go.CustomActive(true);
                if (goList != null)
                {
                    goList.RemoveAt(0);
                }
            }
            else
            {
                var actGo = new ActivityLTObject<T>();
                go = actGo.CreatePrefab(parent, prefabPath);
                go.CustomActive(true);
                //goList.Add(go);
                if (actGoList != null)
                {
                    actGoList.Add(actGo);
                }
            }
            return go;
        }

        public void ReleaseGameObject(GameObject go)
        {
            if (go == null)
                return;
            go.CustomActive(false);
            Utility.AttachTo(go, parent);
            if (goList != null)
            {
                goList.Add(go);
            }
        }

        public void ReleasePool()
        {
            if (actGoList != null)
            {
                for (int i = 0; i < actGoList.Count; i++)
                {
                    var go = actGoList[i].GetGoSelf();
                    if (go != null)
                    {
                        CGameObjectPool.instance.RecycleGameObject(go);
                        //GameObject.Destroy(go);
                    }
                }
            }
            actGoList = null;
            goList = null;
        }
    }
}
