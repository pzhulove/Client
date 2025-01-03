using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using Network;
using Protocol;
using ProtoTable;
using System;
using ActivityLimitTime;

//using ActivityLimitTime;

namespace LimitTimeGift
{
    /// <summary>
    ///  限时礼包 弹窗 管理
    /// </summary>
    public class LimitTimeGiftFrameManager : Singleton<LimitTimeGiftFrameManager>
    {
        private List<LimitTimeGiftFrame> currShowGiftFrameList;

        //private delegate void WaitToShowGiftFrame(LimitTimeGiftData giftData);
        //public WaitToShowGiftFrame waitToShowGiftFrame;
        private LimitTimeGiftData cacheLimitTimeDataToShowFrame;

        public override void Init()
        {
            currShowGiftFrameList = new List<LimitTimeGiftFrame>();
            cacheLimitTimeDataToShowFrame = null;
        }

        public override void UnInit()
        {
            ClearAllCurrShowGiftFrameList();
            currShowGiftFrameList = null;
            cacheLimitTimeDataToShowFrame = null;
        }

        public void AddCurrShowGiftFrame(LimitTimeGiftData giftData)
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<MallNewFrame>())
                return;
            var currSys = ClientSystemManager.GetInstance().CurrentSystem;
            if (currSys != null)
            {
                if (currShowGiftFrameList != null)
                {
                    //currShowGiftFrameList.Clear();
                    cacheLimitTimeDataToShowFrame = null;

                    //这里处理不太好
                    if (currSys is ClientSystemTown )
                    {
                        if (ChijiDataManager.GetInstance().SwitchingPrepareToTown)
                        {
                            return;
                        }

                        ClientSystemManager.GetInstance().delayCaller.DelayCall(500, () =>
                        {
                            if (ClientSystemManager.GetInstance().IsFrameOpen<StrengthenResultFrame>() ||
                                 ClientSystemManager.GetInstance().IsFrameOpen<StrengthenContinueResultsFrame>())
                            {
                                cacheLimitTimeDataToShowFrame = giftData;
                                return;
                            }
                            var giftFrame = ClientSystemManager.GetInstance().OpenFrame<LimitTimeGiftFrame>(FrameLayer.Middle, giftData) as LimitTimeGiftFrame;
                            if (giftFrame != null)
                            {
                                currShowGiftFrameList.Add(giftFrame);
                            }
                        });
                    }
                    else
                    {
                        cacheLimitTimeDataToShowFrame = giftData;
                    }
                }
            }
        }

        public void RemoveCurrShowGiftFrame(LimitTimeGiftFrame giftFrame)
        {
            if (currShowGiftFrameList != null && giftFrame != null)
            {
                if (currShowGiftFrameList.Contains(giftFrame))
                {
                    currShowGiftFrameList.Remove(giftFrame);
                }
            }
        }

        public void ClearAllCurrShowGiftFrameList()
        {
            if (currShowGiftFrameList != null)
                currShowGiftFrameList.Clear();
        }

        public void WaitToShowLimitTimeGiftFrame()
        {
            if (cacheLimitTimeDataToShowFrame != null)
            {
                ClientSystemManager.GetInstance().delayCaller.DelayCall(250, () => {
                    //暂时这么改。。。。
                    if (ClientSystemManager.GetInstance().IsFrameOpen<StrengthenContinueResultsFrame>())
                        return;
                    var giftFrame = ClientSystemManager.GetInstance().OpenFrame<LimitTimeGiftFrame>(FrameLayer.Middle, cacheLimitTimeDataToShowFrame) as LimitTimeGiftFrame;
                    if (giftFrame != null)
                    {
                        currShowGiftFrameList.Add(giftFrame);
                        cacheLimitTimeDataToShowFrame = null;
                    }
                });
            }
        }

        #region Frame Show by UI Event
        void RegisterActivateUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemStrengthenFail, _OnItemStrengthenFail);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemStrengthenSuccess, _OnItemStrengthenSucc);
        }
        void UnRegisterActivateUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemStrengthenFail, _OnItemStrengthenFail);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemStrengthenSuccess, _OnItemStrengthenSucc);
        }

        void _OnItemStrengthenFail(UIEvent uiEvent)
        {
        }

        void _OnItemStrengthenSucc(UIEvent uiEvent)
        {
        }
        #endregion
    }

    /// <summary>
    ///  商城 限时礼包 item 管理
    /// </summary>
    public class LimitTimeGiftMallItemManager : MonoSingleton<LimitTimeGiftMallItemManager>
    {
        const string GiftInMallPath = "UIFlatten/Prefabs/LimitTimeGift/LimitTimeGiftMallItem";
        private ActivityLTObjectPool<LimitTimeGiftInMall> mallGiftPool;

        private List<LimitTimeGiftInMall> mallGiftItemList;
        private Dictionary<uint, LimitTimeGiftInMall> mallGiftItemDic;

        public void Initialize(GameObject parentGo)
        {
            this.gameObject.name = "LimitTimeGiftItems";
            InitPrefabPool(parentGo);
            InitMallGiftItemList();

        }

        public void UnInitialize()
        {
            //注意释放顺序  先释放引用 再释放object
            UnInitMallGiftItemList();
            ReleasePrefabPool();

        }

        #region MallGift Object Pool
        public GameObject GetMallGiftGo()
        {
            return mallGiftPool.GetGameObject();
        }

        public void ReleaseMallGiftGo(GameObject go)
        {
            mallGiftPool.ReleaseGameObject(go);
        }

        private void InitPrefabPool(GameObject parentGo)
        {
            Utility.AttachTo(this.gameObject, parentGo);
            InitMallGiftPool(0, GiftInMallPath);
        }
        private void ReleasePrefabPool()
        {
            if (mallGiftPool!=null)
                mallGiftPool.ReleasePool();
        }
        private void InitMallGiftPool(int initNum,string prefabPath)
        {
            mallGiftPool = new ActivityLTObjectPool<LimitTimeGiftInMall>(initNum, this.gameObject, prefabPath);
        }
        #endregion     

        #region MallItemList

        private void InitMallGiftItemList()
        {
            mallGiftItemList = new List<LimitTimeGiftInMall>();
            mallGiftItemDic = new Dictionary<uint, LimitTimeGiftInMall>();
        }

        private void UnInitMallGiftItemList()
        {
            ClearMallGiftItemList();
            mallGiftItemList = null;
            mallGiftItemDic = null;
        }

        public List<LimitTimeGiftInMall> GetCurrMallGiftItems()
        {
            return mallGiftItemList;
        }

        public Dictionary<uint, LimitTimeGiftInMall> GetCurrMallGiftItemDicWithId()
        {
            if (mallGiftItemList == null)
            {
                return mallGiftItemDic;
            } 
            if (mallGiftItemDic == null)
            {
                return null;
            }

            mallGiftItemDic.Clear();
            uint giftId = 0;
            LimitTimeGiftInMall mallGiftItem = null; 
           
            for (int i = 0; i < mallGiftItemList.Count; i++)
            {
                mallGiftItem = mallGiftItemList[i];
                if (mallGiftItem != null)
                {
                    if (mallGiftItem.GetCurrItemData()!=null)
                    {
                        giftId = mallGiftItem.GetCurrItemData().GiftId;
                        if (mallGiftItemDic.ContainsKey(giftId))
                        {
                            mallGiftItemDic[giftId] = mallGiftItem;
                        }
                        else
                        {
                            mallGiftItemDic.Add(giftId, mallGiftItem);
                        }
                    }
                }
            }

           return mallGiftItemDic;
        }   

        public void ClearMallGiftItemList()
        {
            if (mallGiftItemList != null)
            {
                for (int i = 0; i < mallGiftItemList.Count; i++)
                {
                    mallGiftItemList[i].Destory();
                }
                mallGiftItemList.Clear();
            }
        }

        public void AddMallGiftItem(LimitTimeGiftInMall mallGiftItem)
        {
            if (mallGiftItemList != null)
                mallGiftItemList.Add(mallGiftItem);
        }

        public void RemoveMallGiftItem(LimitTimeGiftInMall mallGiftItem)
        {
            if (mallGiftItemList != null && mallGiftItem!=null)
            {
                if(mallGiftItemList.Contains(mallGiftItem))
                    mallGiftItemList.Remove(mallGiftItem);
            }
        }

        #endregion
    }

    /// <summary>
    /// 限时礼包 数据 管理
    /// </summary>
    public class LimitTimeGiftDataManager
    {
        private List<LimitTimeGiftData> totalLimitTimeGifts;
        private List<LimitTimeGiftData> mLimitTimeGiftList; //限时礼包活动类型的礼包
        private Dictionary<int, List<LimitTimeGiftAwardData>> limitTimeGiftAwardDicById;            //显示的礼包，根据礼包id 获得 礼包内道具列表 ，从totalGifts中获取
        private const int GIFT_MALL_TYPE_TABLE_ID = 9;

        //Unused
        private Dictionary<int, List<LimitTimeGiftData>> threeToOneGiftDicBySubType;                //三选一礼包之后还是会存入totalGifts中,根据礼包 subType 类型 - key
        private Dictionary<int,List<LimitTimeGiftAwardData>> threeToOneGiftAwardDicById;            //三选一礼包，根据礼包id 获得 礼包内道具列表，从threeToOneGifts中获取
        List<UInt64> BuyFahionItemResUids = new List<UInt64>();
        private System.Action onGiftActivated;
        private bool isFirstInTown;

        private bool isGetMallGifts = false;

        //限时活动激活
        public bool isLimitTimeActShow;
        public bool m_LimitTimeGiftIsClick = false;//限时活动按钮是否点过

        private bool petPushFrameIsOpen = false;
        /// <summary>
        /// 宠物推送界面是否打开
        /// </summary>
        public bool PetPushFrameIsOpen
        {
            get { return petPushFrameIsOpen; }
            set { petPushFrameIsOpen = value; }
        }

        /// <summary>
        /// 宠物推送信息
        /// </summary>
        List<MallItemInfo> mPetPushItemInfo;

        /// <summary>
        /// 得到宠物推送信息
        /// </summary>
        /// <returns></returns>
        public List<MallItemInfo> GetPetPushItemInfo()
        {
            return mPetPushItemInfo;
        }

        public List<LimitTimeGiftData> GetAllLimitTimeGiftData()
        {
            if( totalLimitTimeGifts != null)
                totalLimitTimeGifts.Sort();
            return totalLimitTimeGifts;
        }

        public List<LimitTimeGiftData> GetGiftsDataInMall()
        {
            if (this.mLimitTimeGiftList != null)
            {
                mLimitTimeGiftList.Sort();
            }

            return mLimitTimeGiftList;
        }

        public LimitTimeGiftData GetLimitTimeGiftDataById(uint giftId)
        {
            if (totalLimitTimeGifts != null)
            {
                for(int i = 0; i < totalLimitTimeGifts.Count;i++)
                {
                    if (totalLimitTimeGifts[i].GiftId == giftId)
                        return totalLimitTimeGifts[i];
                }
            }
            return null;
        }

        public void GetMallGiftPack()
        {
            MallTypeTable MallData = TableManager.GetInstance().GetTableItem<MallTypeTable>(GIFT_MALL_TYPE_TABLE_ID);

            WorldMallQueryItemReq req = new WorldMallQueryItemReq();

            if (MallData.MoneyID != 0)
            {
                req.moneyType = (byte)MallData.MoneyID;
            }

            req.tagType = 0;
            req.type = (byte)MallData.MallType;

            if (MallData.MallSubType.Count > 0 && MallData.MallSubType[0] != 0)
            {
                req.subType = (byte)MallData.MallSubType[0];
            }
            else if (MallData.MallSubType != null && MallData.MallSubType.Count == 1)
            {
                req.subType = (byte)MallData.MallSubType[0];
            }

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        public Dictionary<int, List<LimitTimeGiftAwardData>> GetGiftAwardsByGiftId()
        {
            if (totalLimitTimeGifts != null)
            {
                limitTimeGiftAwardDicById = new Dictionary<int, List<LimitTimeGiftAwardData>>();
                for (int i = 0; i < totalLimitTimeGifts.Count; i++)
                {
                    var giftId = totalLimitTimeGifts[i].GiftId;
                    var giftAwards = totalLimitTimeGifts[i].GiftAwards;
                    if (giftAwards != null)
                    {
                        limitTimeGiftAwardDicById.Add((int)giftId, giftAwards);
                    }
                }
            }
            return null;
        }

        public bool CheckIsGift(int gift)
        {
            if (gift != (int)MallGoodsType.INVALID && gift != (int)MallGoodsType.COMMON_CHOOSE_ONE)
                return true;
            return false;
        }

        public void Initialize()
        {
            Clear();
            totalLimitTimeGifts = new List<LimitTimeGiftData>();
            mLimitTimeGiftList = new List<LimitTimeGiftData>();
            //TODO add data
            AddALLGiftDataListener();
            LimitTimeGiftFrameManager.instance.Init();
            LimitTimeBuyActivityManager.instance.Init();
            RegisterUIEvent();

            isLimitTimeActShow = false;


            GetMallGiftPack();
            _BindNetMsg();
            petPushFrameIsOpen = false;
        }

        public void Clear()
        {
            if (totalLimitTimeGifts != null)
            {
                totalLimitTimeGifts.Clear();
            }
            totalLimitTimeGifts = null;
            if (mLimitTimeGiftList != null)
            {
                mLimitTimeGiftList.Clear();
            }
            mLimitTimeGiftList = null;
            onGiftActivated = null;
            isFirstInTown = true;
            //TODO remove data
            RemoveAllGiftDataListener();
            LimitTimeGiftFrameManager.instance.UnInit();
            LimitTimeBuyActivityManager.instance.UnInit();
            UnRegisterUIEvent();
            _UnBindNetMsg();
            isLimitTimeActShow = false;


            isGetMallGifts = false;
            m_LimitTimeGiftIsClick = false;
            petPushFrameIsOpen = false;
            if (mPetPushItemInfo != null)
            {
                mPetPushItemInfo.Clear();
            }
            mPetPushItemInfo = null;
        }

        public void Update(float a_fTime)
        {
            //if (!isInited)
            //{
            //    return;
            //}
            //updateTimer += a_fTime;
            //if (updateTimer > updateDuration)
            //{
            //    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnLimitTimeGiftViewRefresh);
            //    updateTimer = 0f;
            //}
        }

        #region PUBLIC METHOD

        /// <summary>
        /// 激活特定条件的限时礼包，并推送到页面
        /// </summary>
        /// <param name="activateCond">激活礼包的条件 （enum）</param>
        /// <param name="onGiftActivated"> 在激活过礼包情况下，需要添加的其他操作 </param>
        public void TryShowMallGift(MallGiftPackActivateCond activateCond, System.Action onGiftActivated = null)
        {
            this.onGiftActivated = onGiftActivated;
            OnSendReqActivateMallGift(activateCond);
        }

        public void TryShowMallGiftByStrengthRes(System.Action onGiftActivated = null)
        {
            this.onGiftActivated = onGiftActivated;
            var playerLevel = PlayerBaseData.GetInstance().Level;
            OnSendReqActivateMallGift(GetGiftActivateCondByLevel(playerLevel));
        }

        /// <summary>
        /// 监听玩家死亡 添加激活礼包触发
        /// </summary>
        /// <param name="onFirstActivate"></param>
        public void RegisterPlayerDead(System.Action onFirstActivate = null)
        {
            var battle = BattleMain.instance;
            if(battle!=null)
            {
                var player = battle.GetLocalPlayer();
                if (player != null)
                {
                    if (player.playerActor == null)
                        return;
                    player.playerActor.RegisterEventNew(BeEventType.onAfterDead, args =>
                    {
                        //复活币 PlayerBaseData - AliveCoin
                        var localPlayer = BattleMain.instance.GetLocalPlayer();
                        if (localPlayer == null)
                            return;
                        if (localPlayer.CanUseItem(Battle.DungeonItem.eType.RebornCoin, 1) == false)
                        {
                            TryShowMallGift(MallGiftPackActivateCond.DIE, onFirstActivate);
                        }
                    });
                }
            }
        }

        public bool HasNewGiftPackOrToBuy()
        {
            if (totalLimitTimeGifts == null)
                return false;
            if (totalLimitTimeGifts.Count == 0)
                return false;
            var functionUnLockData = TableManager.GetInstance().GetTableItem<FunctionUnLock>((int)FunctionUnLock.eFuncType.Mall);
            if (functionUnLockData != null)
            {
                if (functionUnLockData.StartLevel > PlayerBaseData.GetInstance().Level)
                    return false;
            }
            for (int i = 0; i < totalLimitTimeGifts.Count; i++)
            {
                if (totalLimitTimeGifts[i].GiftState == LimitTimeGiftState.OnSale)
                    return true;
            }
            return false;
        }

        private WorldMallQueryItemReq ReadReqInfoFromTable(int tableId)
        {
            WorldMallQueryItemReq req = null;
            var mallTypeTable = TableManager.GetInstance().GetTableItem<MallTypeTable>(tableId);
            if (mallTypeTable != null)
            {
                req = new WorldMallQueryItemReq();
                req.type = (byte)mallTypeTable.MallType;
                var subTypeList = mallTypeTable.MallSubType;
                if (subTypeList != null && subTypeList.Count > 0)//|| subTypeList.Count == 0)
                {
                    req.subType = 0;
                    if (subTypeList[0] == 0)
                    {
                        req.subType = 0;
                    }
                }
                req.occu = (byte)mallTypeTable.ClassifyJob;
                req.moneyType = (byte)mallTypeTable.MoneyID;
                //Logger.LogError("ReadReqInfoFromTable  = " + req.type + " " + req.subType + " " + req.occu + " " + req.moneyType);
            }
            return req;
        }

        #endregion

        #region Net Req & Ret

        public void SendReqLimitGiftData()
        {
            WorldMallQueryItemReq req = new WorldMallQueryItemReq();

            //TODO 添加请求类型 限时礼包固定（根据表格）
            req.type = 6;
            req.subType = 0;
            req.occu = 0;
            req.moneyType = 0;

            var tableReq = ReadReqInfoFromTable((int)MallItemType.LimitTimeGift);
            if (tableReq != null)
                req = tableReq;
            //Logger.LogError("SendReqLimitGiftData = " + req.type + " " + req.subType + " " + req.occu + " " + req.moneyType);

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        public void SendReqLimitTimeActivityData()
        {
            WorldMallQueryItemReq req = new WorldMallQueryItemReq();

            //TODO 添加请求类型 限时活动固定（根据表格）
            req.type = 7;
            req.subType = 0;
            req.occu = 0;
            req.moneyType = (int)LimitTimeGiftPriceType.Point;

            var tableReq = ReadReqInfoFromTable((int)MallItemType.LimitTimeAct);
            if (tableReq != null)
                req = tableReq;
            //Logger.LogError("SendReqLimitTimeActivityData = " + req.type + " " + req.subType + " " + req.occu + " " + req.moneyType);

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        void OnSendReqLimitGiftDetailData()
        {
            WorldMallQueryItemDetailReq req = new WorldMallQueryItemDetailReq();

            //TODO 添加 请求具体 数据

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        public void SendReqBuyGift(uint giftId,int giftNum)
        {
            WorldMallBuy req = new WorldMallBuy();

            req.itemId = giftId;
            req.num = (ushort)giftNum;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        public void SendReqBuyGiftInMall(uint giftId, int price,int giftNum)
        {
            CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();

            costInfo.nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT);

            ItemTipManager.GetInstance().CloseAll();

            costInfo.nCount = price;

            CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
            {
                WorldMallBuy req = new WorldMallBuy();

                req.itemId = giftId;
                req.num = (ushort)giftNum;

                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, req);
            });
        }

        public void SendReqBuyFashionInMall(int price, uint[] giftItemIds, FashionLimitTimeBuy.SelectMallItemInfoData detailData)
        {
            if (detailData == null || giftItemIds == null)
                return;
            if (detailData.SelectItemInfos == null)
                return;
            int count = giftItemIds.Length;//detailData.SelectItemInfos.Count;
            ItemReward[] items = new ItemReward[count];

            for (int i = 0; i < count; i++)
            {
                ItemReward reward = new ItemReward();

                reward.id = giftItemIds[i];//detailData.SelectItemInfos[i].id;
                reward.num = 1;

                items[i] = reward;
            }

            CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();
            costInfo.nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT);
            ItemTipManager.GetInstance().CloseAll();
            costInfo.nCount = price;

            CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
            {
                CWMallBatchBuyReq req = new CWMallBatchBuyReq();
                req.items = items;
                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, req);
            });
        }
		
		public void SendReqBuyFashionInMall(uint giftId, int price, int giftNum, FashionLimitTimeBuy.SelectMallItemInfoData detailData)
        {
            if (detailData == null)
                return;
            if (detailData.SelectItemInfos == null)
                return;
            int count = detailData.SelectItemInfos.Count;
            ItemReward[] items = new ItemReward[count];

            for (int i = 0; i < count; i++)
            {
                ItemReward reward = new ItemReward();

                reward.id = detailData.SelectItemInfos[i].id;
                reward.num = 1;

                items[i] = reward;
            }

            CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();
            costInfo.nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT);
            ItemTipManager.GetInstance().CloseAll();
            costInfo.nCount = price;

            CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
            {
                if (detailData.multiple > 0)
                {
                    string content = string.Empty;
                    //积分商城积分等于上限值
                    if ((int)PlayerBaseData.GetInstance().IntergralMallTicket == MallNewUtility.GetIntergralMallTicketUpper()&&
                         MallNewDataManager.GetInstance().bItemMallIntergralMallScoreIsEqual == false)
                    {
                        content = TR.Value("mall_buy_intergral_mall_score_equal_upper_value_desc");

                        MallNewUtility.CommonIntergralMallPopupWindow(content, MallNewUtility.ItemMallIntergralMallScoreIsEqual, ()=> { OnSendCWMallBatchBuyReq(count, items); });
                    }
                    else
                    {
                        int ticketConvertScoreNumber = MallNewUtility.GetTicketConvertIntergalNumnber(detailData.TotalPrice) * detailData.multiple;

                        int allIntergralScore = (int)PlayerBaseData.GetInstance().IntergralMallTicket + ticketConvertScoreNumber;

                        //购买道具后商城积分超出上限值
                        if (allIntergralScore > MallNewUtility.GetIntergralMallTicketUpper() &&
                           (int)PlayerBaseData.GetInstance().IntergralMallTicket != MallNewUtility.GetIntergralMallTicketUpper() &&
                            MallNewDataManager.GetInstance().bItemMallIntergralMallScoreIsExceed == false)
                        {
                            content = TR.Value("mall_buy_intergral_mall_score_exceed_upper_value_desc",
                                               (int)PlayerBaseData.GetInstance().IntergralMallTicket,
                                               MallNewUtility.GetIntergralMallTicketUpper(),
                                               MallNewUtility.GetIntergralMallTicketUpper() - (int)PlayerBaseData.GetInstance().IntergralMallTicket);

                            MallNewUtility.CommonIntergralMallPopupWindow(content, MallNewUtility.ItemMallIntergralMallScoreIsExceed, () => { OnSendCWMallBatchBuyReq(count, items); });
                        }
                        else
                        {//未超出
                            OnSendCWMallBatchBuyReq(count, items);
                        }
                    }
                }
                else
                {
                    OnSendCWMallBatchBuyReq(count, items);
                }

                
            });
        }

        void OnSendCWMallBatchBuyReq(int count, ItemReward[] items)
        {
            CWMallBatchBuyReq req = new CWMallBatchBuyReq();
            req.items = new ItemReward[count];
            req.items = items;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        void OnSendReqActivateMallGift(MallGiftPackActivateCond activeCond)
        {
            WorldMallGiftPackActivateReq req = new WorldMallGiftPackActivateReq();
            req.giftPackActCond = (byte)activeCond;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER,req);
        }

        void OnSendReqActivateMallGift(int testCond)
        {
            WorldMallGiftPackActivateReq req = new WorldMallGiftPackActivateReq();
            req.giftPackActCond = (byte)testCond;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        void AddALLGiftDataListener()
        {
            NetProcess.AddMsgHandler(WorldMallQueryItemRet.MsgID,OnSyncLimitTimeGiftData);
            //NetProcess.AddMsgHandler(WorldMallQueryItemDetailRet.MsgID, OnSyncLimitTimeGiftDetailData);
            NetProcess.AddMsgHandler(WorldMallBuyRet.MsgID, OnGiftBuyRes);
            NetProcess.AddMsgHandler(WorldMallGiftPackActivateRet.MsgID,OnActivateGiftRes);

            //NetProcess.AddMsgHandler(SceneBillingSendGoodsNotify.MsgID, OnPayGiftData);

            NetProcess.AddMsgHandler(SyncWorldMallGiftPackActivityState.MsgID, OnSyncLimitTimeAct);
        }

        void RemoveAllGiftDataListener()
        {
            NetProcess.RemoveMsgHandler(WorldMallQueryItemRet.MsgID, OnSyncLimitTimeGiftData);
           // NetProcess.RemoveMsgHandler(WorldMallQueryItemDetailRet.MsgID, OnSyncLimitTimeGiftDetailData);
            NetProcess.RemoveMsgHandler(WorldMallBuyRet.MsgID, OnGiftBuyRes);
            NetProcess.RemoveMsgHandler(WorldMallGiftPackActivateRet.MsgID, OnActivateGiftRes);

            //NetProcess.RemoveMsgHandler(SceneBillingSendGoodsNotify.MsgID, OnPayGiftData);

            NetProcess.RemoveMsgHandler(SyncWorldMallGiftPackActivityState.MsgID, OnSyncLimitTimeAct);
        }

        void _BindNetMsg()
        {
            NetProcess.AddMsgHandler(WorldPushMallItems.MsgID, OnWorldPushMallItems);
        }

        void _UnBindNetMsg()
        {
            NetProcess.RemoveMsgHandler(WorldPushMallItems.MsgID, OnWorldPushMallItems);
        }
        void RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnPayResultNotify, OnPayGiftData);
        }

        void UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnPayResultNotify, OnPayGiftData);
        }

        void OnSyncLimitTimeGiftData(MsgDATA msg)
        {
            //if (ClientSystemManager.GetInstance().IsFrameOpen<MallFrame>())
            //    return;
            WorldMallQueryItemRet res = new WorldMallQueryItemRet();
            res.decode(msg.bytes);
            SyncNetGiftDataToLocal(res);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnLimitTimeGiftViewRefresh);
        }

        void OnSyncLimitTimeGiftDetailData(MsgDATA msg)
        {
            WorldMallQueryItemDetailRet res = new WorldMallQueryItemDetailRet();
            res.decode(msg.bytes);
        }

        void OnGiftBuyRes(MsgDATA msg)
        {
            WorldMallBuyRet res = new WorldMallBuyRet();
            res.decode(msg.bytes);

            //还有返回限购数  -1是没有限购

            if (res.code == (uint)ProtoErrorCode.SUCCESS)
            {
                //SystemNotifyManager.SysNotifyTextAnimation("购买成功");
            }
            else
            {
                //统一在ShopNewDataManager处理
                // SystemNotifyManager.SystemNotify((int)res.code);
            }

            UpdateLocalLimitTimeGift(res);
            UpdateActivityPriceShow(res);
        }
        
        void OnActivateGiftRes(MsgDATA msg)
        {
            WorldMallGiftPackActivateRet res = new WorldMallGiftPackActivateRet();
            res.decode(msg.bytes);
            ActivateLimitTimeGift(res);
        }

        /// <summary>
        /// 服务器同步显示隐藏 限时活动入口
        /// </summary>
        /// <param name="msg"></param>
        void OnSyncLimitTimeAct(MsgDATA msg)
        {
            SyncWorldMallGiftPackActivityState res = new SyncWorldMallGiftPackActivityState();
            res.decode(msg.bytes);
            SyncLimitTimeActivty(res);
        }

        void OnPayGiftData(MsgDATA msg)
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<MallNewFrame>())
                return;
            if (ClientSystemManager.GetInstance().IsFrameOpen<LimitTimeGiftFrame>())
            {
                RefreshPayGiftData();
            }
        }

        /// <summary>
        /// 推送商城宠物礼包
        /// </summary>
        /// <param name="msg"></param>
        void OnWorldPushMallItems(MsgDATA msg)
        {
            WorldPushMallItems res = new WorldPushMallItems();
            res.decode(msg.bytes);

            mPetPushItemInfo = new List<MallItemInfo>();

            if (res.mallItems.Length > 0)
            {
                for (int i = 0; i < res.mallItems.Length; i++)
                {
                    mPetPushItemInfo.Add(res.mallItems[i]);
                }
            }

            if (mPetPushItemInfo != null)
            {
                ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
                if (systemTown != null)
                {
                    OpenLimitTimePetGiftFrame(mPetPushItemInfo);
                }
                else
                {
                    petPushFrameIsOpen = true;
                }
                //推送后请求触发礼包
                MallNewDataManager.GetInstance().GetTriggerGiftMallList();
            }
        }

        /// <summary>
        /// 打开宠物礼包界面
        /// </summary>
        /// <param name="data"></param>
        public void OpenLimitTimePetGiftFrame(List<MallItemInfo> data)
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<LimitTimePetGiftFrame>() == false)
            {
                ClientSystemManager.GetInstance().OpenFrame<LimitTimePetGiftFrame>(FrameLayer.Middle, data);
            }
        }

        void OnPayGiftData(UIEvent uiEvent)
        {
            if (uiEvent != null)
            {
                if (uiEvent.Param1.Equals("0"))
                {
                    if (ClientSystemManager.GetInstance().IsFrameOpen<MallNewFrame>())
                        return;
                    if (ClientSystemManager.GetInstance().IsFrameOpen<LimitTimeGiftFrame>())
                    {
                        RefreshPayGiftData();
                    }
                }
            }
        }

        #endregion

        #region Net Data to local

        /// <summary>
        /// 商城同步商城道具数据到本地 /  限时礼包弹窗（初始不在商城）请求回调数据
        /// </summary>
        /// <param name="res">请求商城协议</param>
        public void SyncNetGiftDataToLocal(WorldMallQueryItemRet res)
        {
            if (res == null)
                return;
            MallItemInfo[] mallItems = res.items;
            if (mallItems != null)
            {
                if (res.type == (byte)MallTypeTable.eMallType.SN_ACTIVITY_GIFT)
                {
                    if (this.mLimitTimeGiftList == null)
                    {
                        mLimitTimeGiftList = new List<LimitTimeGiftData>();
                    }
                    mLimitTimeGiftList.Clear();
                }
                //屏蔽私人定制
                else if (res.type == (byte)MallTypeTable.eMallType.MallType_None)
                {
                    return;
                }

                if (mallItems.Length <= 0)
                {
                    if (res.type == (byte) MallTypeTable.eMallType.SN_GIFT)
                    {
                        if (totalLimitTimeGifts != null)
                            totalLimitTimeGifts.Clear();
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnLimitTimeGiftDataRefresh);
                    }
                    return;
                }
                bool isInvalid = CheckSyncMallItemInfoType(mallItems, MallGoodsType.INVALID);
                if (isInvalid)
                    return;
                bool isCommon = CheckSyncMallItemInfoType(mallItems, MallGoodsType.COMMON_CHOOSE_ONE);
                if (isCommon)
                    return;
                bool isActivity = CheckSyncMallItemInfoType(mallItems, MallGoodsType.GIFT_ACTIVITY);

                if (isActivity)
                {
                    //保存限时活动数据
                    if (mallItems.Length >= 2)
                    {
                        Logger.Log("限时活动数量大于1个了");
                    }
                    for (int i = 0; i < mallItems.Length; i++)
                    {
                        mLimitTimeGiftList.Add(SyncMallItemInfoToLimitTimeGift(mallItems[i]));
                    }

                    LimitTimeGiftData limitTimeGift = SyncMallItemInfoToLimitTimeGift(mallItems[0]);
                    LimitTimeBuyActivityManager.instance.SyncLimitTimeActivityData(mLimitTimeGiftList);

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnLimitTimeGiftDataRefresh);
                    return;
                }

                if (totalLimitTimeGifts != null)
                    totalLimitTimeGifts.Clear();
                totalLimitTimeGifts = new List<LimitTimeGiftData>();
                //if (threeToOneGiftDicBySubType != null)
                //    threeToOneGiftDicBySubType.Clear();
                //threeToOneGiftDicBySubType = new Dictionary<int,List<LimitTimeGiftData>>();
                for (int i = 0; i < mallItems.Length; i++)
                {
                    var mallItem = mallItems[i];
                    LimitTimeGiftData limitTimeGift = SyncMallItemInfoToLimitTimeGift(mallItem);
                    totalLimitTimeGifts.Add(limitTimeGift);
                }

                if (OnItemPayRetHandler != null)
                {
                    OnItemPayRetHandler();

                    if (ClientSystemManager.GetInstance().IsFrameOpen<LimitTimeGiftFrame>() == false)
                    {
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.HasLimitTimeGiftToBuy);
                    }
                }
                else if (isFirstInTown)
                {
#if APPLE_STORE
                    //add by mjx for ios appstore 
                    if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.ADS_PUSH))
                    {
                        isFirstInTown = false;
                        return;
                    }
#endif
                    //每日只显示一次 限时礼包购买 但有激活条件时仍触发
                    //xzl
                    if (AdsPush.LoginPushManager.GetInstance().IsFirstLogin())
                    {
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.HasLimitTimeGiftToBuy);
                    }
                    isFirstInTown = false;
                }

                if (!this.isGetMallGifts)
                {
                    ActivityLimitTimeCombineManager.GetInstance().IsCheckedLimitTimeMallGift = totalLimitTimeGifts.Count <= 0 || !AdsPush.LoginPushManager.GetInstance().IsFirstLogin();
                    isGetMallGifts = true;
                }
                //SyncThreeToOnGiftsToTotalGifts();
            }
        }

        /// <summary>
        /// 检查同步到的商城道具类型是否为指定类型
        /// </summary>
        /// <param name="goodType"></param>
        /// <returns></returns>
        bool CheckSyncMallItemInfoType(MallItemInfo[] mallinfos,MallGoodsType goodType)
        {
            if (mallinfos == null || mallinfos.Length <= 0)
                return false;
            int count = mallinfos.Length;
            for (int i = 0; i < count; i++)
            {
                if (mallinfos[i].gift != (int)goodType)
                    return false;
            }
            return true;
        }

                               #region Unused  New add Func
        void SaveThreeToOneGiftsInDic(MallItemInfo mallItem)
        {
            if (threeToOneGiftDicBySubType != null)
            {
                LimitTimeGiftData limitTimeGift = SyncMallItemInfoToLimitTimeGift(mallItem);

                int keyType = mallItem.subtype;
                if (threeToOneGiftDicBySubType.ContainsKey(keyType))
                {
                    threeToOneGiftDicBySubType[keyType].Add(limitTimeGift);
                }
                else
                {
                    threeToOneGiftDicBySubType.Add(keyType, new List<LimitTimeGiftData>() { limitTimeGift });
                }
            }
        }

        void SyncThreeToOnGiftsToTotalGifts()
        {
            if (threeToOneGiftDicBySubType != null && totalLimitTimeGifts != null)
            {
                var enumerator = threeToOneGiftDicBySubType.GetEnumerator();

                int minLimitNum = 0;
                while (enumerator.MoveNext())
                {
                    var giftList = enumerator.Current.Value;
                    if (giftList != null)
                    {
                        if(giftList.Count>0)
                        {
                            LimitTimeGiftData limitTimeGift = new LimitTimeGiftData();

                            //用第一个礼包的基本信息展示三选一礼包
                            limitTimeGift.GiftId = giftList[0].GiftId;
                            limitTimeGift.GiftName = giftList[0].GiftName;
                            limitTimeGift.RemainingTimeSec = giftList[0].RemainingTimeSec;
                            limitTimeGift.PriceType = giftList[0].PriceType;
                            limitTimeGift.GiftPrice = giftList[0].GiftPrice;
                            limitTimeGift.LimitType = giftList[0].LimitType;
                            limitTimeGift.GiftType = giftList[0].GiftType;

                            limitTimeGift.GiftAwards = new List<LimitTimeGiftAwardData>();
                            limitTimeGift.ThreeToOneGifts = new List<LimitTimeGiftData>();
                            for (int i = 0; i < giftList.Count; i++)
                            {
                                //得到最小限购次数
                                var tempNum = giftList[i].LimitPurchaseNum;
                                if (tempNum == 0)
                                {
                                    minLimitNum = 0;
                                }
                                if (i == 0)
                                {
                                    minLimitNum = tempNum;
                                }
                                if (minLimitNum > tempNum)
                                {
                                    minLimitNum = tempNum;
                                }

                                //展示奖励，每个礼包的第一个
                                if (giftList[i].GiftAwards != null && limitTimeGift.GiftAwards!=null)
                                {
                                    if (giftList[i].GiftAwards.Count > 0)
                                    {
                                        //for (int j = 0; j < giftList[i].GiftAwards.Count; j++)
                                        //{
                                        //    limitTimeGift.GiftAwards.Add(giftList[i].GiftAwards[j]);
                                        //}
                                        //只保存每个礼包中的第一个奖励道具
                                        limitTimeGift.GiftAwards.Add(giftList[i].GiftAwards[0]);
                                    }
                                }

                               //保存三选一礼包
                                limitTimeGift.ThreeToOneGifts.Add(giftList[i]);
                            }
                            limitTimeGift.LimitPurchaseNum = minLimitNum;

                            if (totalLimitTimeGifts != null)
                            {
                                totalLimitTimeGifts.Add(limitTimeGift);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        void UpdateLocalLimitTimeGift(WorldMallBuyRet buyRes)
        {
            if (totalLimitTimeGifts != null)
            {
                for (int i = 0; i < totalLimitTimeGifts.Count; i++)
                {
                    if (totalLimitTimeGifts[i].GiftId == buyRes.mallitemid)
                    {
                        totalLimitTimeGifts[i].LimitPurchaseNum = buyRes.restLimitNum;
                        if (OnItemBuyRetHandler != null)
                        {
                            OnItemBuyRetHandler(totalLimitTimeGifts[i]);

                            if (ClientSystemManager.GetInstance().IsFrameOpen<LimitTimeGiftFrame>() == false)
                            {
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.HasLimitTimeGiftToBuy);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 限时活动 购买返回 刷新状态
        /// </summary>
        /// <param name="buyRes"></param>
        void UpdateActivityPriceShow(WorldMallBuyRet buyRes)
        {
            if(buyRes.code == (uint)ProtoErrorCode.SUCCESS)
            {
                if (ClientSystemManager.GetInstance().IsFrameOpen<LimitTimeGiftFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<LimitTimeGiftFrame>();
                }
            }
        }

        /// <summary>
        /// 激活礼包 回调
        /// </summary>
        /// <param name="actRes"></param>
        void ActivateLimitTimeGift(WorldMallGiftPackActivateRet actRes)
        {
#if APPLE_STORE
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.LIMITTIME_GIFT))
            {
                return;
            }
#endif
            if ((int)actRes.code == (int)ProtoErrorCode.SUCCESS)
            {
                if (actRes.items != null)
                {
                    var mallItems = actRes.items;
                    if (mallItems == null)
                        return;
                    if (mallItems.Length == 0)
                        return;
                    if (mallItems[0] != null)
                    {
                        LimitTimeGiftData limitTimeGift = new LimitTimeGiftData();
                        limitTimeGift = SyncMallItemInfoToLimitTimeGift(mallItems[0]);
                        if (totalLimitTimeGifts != null && limitTimeGift != null)
                        {
                            if (!totalLimitTimeGifts.Contains(limitTimeGift))
                            {
                                totalLimitTimeGifts.Add(limitTimeGift);
                            }
                        }
                        #region New add Func
                        /*
                    if (mallItems[0].gift != (int)LimitTimeGiftType.ThreeToOne && mallItems[0].gift != (int)LimitTimeGiftType.None)
                    {
                        limitTimeGift = SyncMallItemInfoToLimitTimeGift(mallItems[0]);
                        //LimitTimeGiftFrameManager.instance.AddCurrShowGiftFrame(limitTimeGift);
                    }
                    else if (mallItems[0].gift == (int)LimitTimeGiftType.ThreeToOne)
                    {
                        limitTimeGift.GiftId = mallItems[0].id;
                        limitTimeGift.GiftName = mallItems[0].giftName;
                        limitTimeGift.RemainingTimeSec = mallItems[0].endtime - mallItems[0].starttime;
                        limitTimeGift.GiftType = (LimitTimeGiftType)mallItems[0].gift;
                        limitTimeGift.PriceType = (LimitTimeGiftPriceType)mallItems[0].moneytype;
                        limitTimeGift.GiftPrice = Utility.GetMallRealPrice(mallItems[0]);
                        limitTimeGift.GiftAwards = new List<LimitTimeGiftAwardData>();
                        limitTimeGift.ThreeToOneGifts = new List<LimitTimeGiftData>();
                        int minLimitNum = -1;
                        for (int i = 0; i < mallItems.Length; i++)
                        {
                            //得到最小限购次数
                            var tempNum = mallItems[i].limittotalnum;
                            if (tempNum == 0)
                            {
                                minLimitNum = 0;
                            }
                            if (i == 0)
                            {
                                minLimitNum = tempNum;
                            }
                            if (minLimitNum > tempNum)
                            {
                                minLimitNum = tempNum;
                            }

                            //展示奖励，每个礼包的第一个
                            if (mallItems[i].giftItems != null && limitTimeGift.GiftAwards != null)
                            {
                                if (mallItems[i].giftItems.Length > 0)
                                {
                                    //for (int j = 0; j < mallItems[i].giftItems.Length; j++)
                                    //{

                                    //}
                                    var giftAward = new LimitTimeGiftAwardData();
                                    giftAward.AwardId = mallItems[i].giftItems[0].id;
                                    giftAward.AwardCount = mallItems[i].giftItems[0].num;
                                    giftAward.StrengthLevel = mallItems[i].giftItems[0].strength;
                                    limitTimeGift.GiftAwards.Add(giftAward);
                                }
                            }

                            //保存三选一礼包
                            LimitTimeGiftData tempGiftData = SyncMallItemInfoToLimitTimeGift(mallItems[i]);
                            limitTimeGift.ThreeToOneGifts.Add(tempGiftData);
                        }
                        limitTimeGift.LimitPurchaseNum = minLimitNum;
                    }
                     * */
                        #endregion
                        LimitTimeGiftFrameManager.instance.AddCurrShowGiftFrame(limitTimeGift);
                    }
                }
                //有新的触发礼包 让malldatamanager去获取最新的触发礼包
                MallNewDataManager.GetInstance().GetTriggerGiftMallList();
            }
            else
            {
                if (this.onGiftActivated != null)
                {
                    this.onGiftActivated();
                    this.onGiftActivated = null;
                }
            }
        }

        /// <summary>
        /// 服务器同步显示隐藏限时活动
        /// </summary>
        /// <param name="res"></param>
        void SyncLimitTimeActivty(SyncWorldMallGiftPackActivityState res)
        {
#if APPLE_STORE
            //add by mjx for ios appstore
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.LIMITTIME_ACTIVITY))
            {
                isLimitTimeActShow = false;
                return;
            }
#endif
            if (res != null)
            {
                if (res.state == (byte)MallGiftPackActivityState.GPAS_OPEN)
                {
                    isLimitTimeActShow = true;
                    //发送获取icon Id 的请求
                    ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.SendReqLimitTimeActivityData();
                }
                else if (res.state == (byte)MallGiftPackActivityState.GPAS_CLOSED)
                {
                    isLimitTimeActShow = false;
                    //隐藏时 设置为默认icon 避免切换活动时显示了前一个活动icon
                    //LimitTimeGift.LimitTimeBuyActivityManager.GetInstance().SetDefaultActivityIcon();
                }else
                {
                    Logger.LogErrorFormat("SyncWorldMallGiftPackActivityState's state is error");
                    isLimitTimeActShow = false;
                }

                LimitTimeGift.LimitTimeBuyActivityManager.GetInstance().NeedRefreshIcon = isLimitTimeActShow;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ShowLimitTimeActivityBtn, isLimitTimeActShow);
            }
        }

        void RefreshPayGiftData()
        {
            SendReqLimitGiftData();
        }

        public LimitTimeGiftData SyncMallItemInfoToLimitTimeGift(MallItemInfo mallItem)
        {
            LimitTimeGiftData limitTimeGift = new LimitTimeGiftData();
            if(mallItem == null)
                return limitTimeGift;
            limitTimeGift.mallItemInfoData = mallItem;
            limitTimeGift.GiftId = mallItem.id;
            limitTimeGift.GiftName = mallItem.giftName;
            limitTimeGift.GiftType = (MallGoodsType)mallItem.gift;
            limitTimeGift.GiftDesc = mallItem.giftDesc;
            if (mallItem.gift == (int)MallGoodsType.GIFT_DAILY_REFRESH || mallItem.gift == (int)MallGoodsType.GIFT_COMMON_DAILY_REFRESH)
            {
                limitTimeGift.LimitPurchaseNum = mallItem.limitnum;
            }
            else
            {
                limitTimeGift.LimitPurchaseNum = mallItem.limittotalnum;
            }
            limitTimeGift.LimitNum = mallItem.limitnum;
            limitTimeGift.LimitTotalNum = mallItem.limittotalnum;
            bool bIsDailyLimit = false;
            limitTimeGift.LimitLastNum = Utility.GetLeftLimitNum(mallItem, ref bIsDailyLimit);


            limitTimeGift.RemainingTimeSec = mallItem.endtime - TimeManager.GetInstance().GetServerTime();
            limitTimeGift.PriceType = (LimitTimeGiftPriceType)mallItem.moneytype;
            limitTimeGift.GiftPrice = Utility.GetMallRealPrice(mallItem);
            limitTimeGift.LimitType = (ELimitiTimeGiftDataLimitType)mallItem.limit;
            limitTimeGift.GiftIconPath = mallItem.icon;
            limitTimeGift.GiftStartTime = mallItem.starttime;
            limitTimeGift.GiftEndTime = mallItem.endtime;

            if (mallItem.giftItems != null)
            {
                limitTimeGift.GiftAwards = new List<LimitTimeGiftAwardData>();
                for (int j = 0; j < mallItem.giftItems.Length; j++)
                {
                    var giftAward = new LimitTimeGiftAwardData();
                    giftAward.AwardId = mallItem.giftItems[j].id;
                    giftAward.AwardCount = mallItem.giftItems[j].num;
                    giftAward.StrengthLevel = mallItem.giftItems[j].strength;
                    limitTimeGift.GiftAwards.Add(giftAward);
                }
            }

            return limitTimeGift;
        }

        #endregion

        #region UI refresh Listeners

        public event System.Action<LimitTimeGiftData> OnItemBuyRetHandler;
        public void AddItemBuyRetListener(System.Action<LimitTimeGiftData> handler)
        {
            RemoveAllItemBuyRetListener();
            if (OnItemBuyRetHandler == null)
                OnItemBuyRetHandler += handler;
        }

        public void RemoveAllItemBuyRetListener()
        {
            if (OnItemBuyRetHandler != null)
            {
                var invocations = OnItemBuyRetHandler.GetInvocationList();
                if (invocations != null)
                {
                    int count = invocations.Length;
                    for (int i = 0; i < count; i++)
                    {
                        OnItemBuyRetHandler -= invocations[i] as Action<LimitTimeGiftData>;
                    }
                }
            }
        }

        public event System.Action OnItemPayRetHandler;
        public void AddItemPayRetListener(System.Action handler)
        {
            RemoveAllItemPayRetListener();
            if (OnItemPayRetHandler == null)
                OnItemPayRetHandler += handler;
        }

        public void RemoveAllItemPayRetListener()
        {
            if (OnItemPayRetHandler != null)
            {
                var invocations = OnItemPayRetHandler.GetInvocationList();
                if (invocations != null)
                {
                    int count = invocations.Length;
                    for (int i = 0; i < count; i++)
                    {
                        OnItemPayRetHandler -= invocations[i] as Action;
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 限时条件礼包激活条件 根据角色等级判断
        /// </summary>
        /// <param name="playerLevel"></param>
        /// <returns></returns>
        MallGiftPackActivateCond GetGiftActivateCondByLevel(int playerLevel)
        {
            if (playerLevel >= 10 && playerLevel < 15)
            {
                return MallGiftPackActivateCond.STRENGEN_BROKE_TEN;
            }
            else if (playerLevel >= 15 &&playerLevel < 20)
            {
                return MallGiftPackActivateCond.STRENGEN_BROKE_FIFTEEN;
            }
            else if (playerLevel >= 20 && playerLevel < 25)
            {
                return MallGiftPackActivateCond.STRENGEN_BROKE_TWENTY;
            }
            else if (playerLevel >= 25 &&playerLevel < 30)
            {
                return MallGiftPackActivateCond.STRENGEN_BROKE_TWENTY_FIVE;
            }
            else if (playerLevel >= 30 && playerLevel < 35)
            {
                return MallGiftPackActivateCond.STRENGEN_BROKE_THIRTY;
            }
            else if (playerLevel >= 35 &&playerLevel < 40)
            {
                return MallGiftPackActivateCond.STRENGEN_BROKE_THIRTY_FIVE;
            }
            else if (playerLevel >= 40 &&playerLevel < 45)
            {
                return MallGiftPackActivateCond.STRENGEN_BROKE_FORTY;
            }
            else if (playerLevel >= 45 &&playerLevel < 50)
            {
                return MallGiftPackActivateCond.STRENGEN_BROKE_FORTY_FIVE;
            }
            else if (playerLevel >= 50)
            {
                return MallGiftPackActivateCond.STRENGEN_BROKE_FIFTY;
            }
            return MallGiftPackActivateCond.INVALID;
        }

    }

    #region  LimitTimeActivity
    public class LimitTimeBuyActivityManager : Singleton<LimitTimeBuyActivityManager>
    {
        LimitTimeGiftData currShowActivityData;
        public bool mIsHaveSummerGift { get; private set; }
        public bool mIsHaveOtherGift { get; private set; }

        #region 限时活动入口icon

        public bool NeedRefreshIcon;

        #endregion

        public override void Init()
        {
            //currShowActivityData = null;
            //iconImg = null;
            //NeedRefreshIcon = false;
            mIsHaveSummerGift = false;
            mIsHaveOtherGift = false;
        }

        public override void UnInit()
        {
            //currShowActivityData = null;
            //iconImg = null;
            //NeedRefreshIcon = false;
            mIsHaveSummerGift = false;
            mIsHaveOtherGift = false;
        }

        public LimitTimeGiftData GetCurrActivityData()
        {
            return currShowActivityData;
        }

        public void SyncLimitTimeActivityData(List<LimitTimeGiftData> data)
        {
            //currShowActivityData = null;
            //if(currShowActivityData == null)
            //{
            //    currShowActivityData = data;
            //    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.InitLimitTimeActivityView,currShowActivityData);
            //    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RefreshLimitTimeActivityIcon);
            //}
            mIsHaveSummerGift = false;
            mIsHaveOtherGift = false;

            var Limitgiftlist = data;
            if (data != null)
            {
                for (int i = 0; i < data.Count; ++i)
                {
                    if (data[i].GiftId == ActivityLimitTimeCombineManager.SUMMER_GIFT_ID || data[i].GiftId == ActivityLimitTimeCombineManager.SUMMER_DRINK_ID)
                    {
                        this.mIsHaveSummerGift = true;
                    }
                    else
                    {
                        this.mIsHaveOtherGift = true;
                    }

                    if (this.mIsHaveSummerGift && this.mIsHaveOtherGift)
                    {
                        break;
                    }
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.InitLimitTimeActivityView, currShowActivityData, Limitgiftlist);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RefreshLimitTimeActivityIcon);
            }
        }

        public void UpdateLimitTimeActIcon(UnityEngine.UI.Image icon,UnityEngine.UI.Text text)
        {
            //Logger.LogError("UpdateLimitTimeActIcon  =  NeedRefreshIcon :" + NeedRefreshIcon + " , icon path : " + icon);
            if (NeedRefreshIcon == false)
                return;
            if (icon == null || text == null)
                return;
            if (currShowActivityData != null)
            {
                var clientTable = TableManager.GetInstance().GetTableItem<MallLimitTimeActivity>((int)currShowActivityData.GiftId);
                if (clientTable != null)
                {
                    string iconPath = clientTable.IconPath;
                    if (!string.IsNullOrEmpty(iconPath))
                    {
                        try
                        {
                            //var iconSpr = AssetLoader.GetInstance().LoadRes(iconPath, typeof(Sprite)).obj as Sprite;
                            //icon.sprite = iconSpr;
                            ETCImageLoader.LoadSprite(ref icon, iconPath);
                            icon.SetNativeSize();
                            var iconRect = icon.GetComponent<RectTransform>();
                            if (iconRect)
                            {
                                iconRect.anchoredPosition = new Vector2(clientTable.IconPosX,clientTable.IconPosY);
                            }

                            //设置图标名称
                            text.text = clientTable.Name;

                            NeedRefreshIcon = false;
                        }
                        catch (Exception e)
                        {
                            //Logger.LogErrorFormat("加载限时活动入口按钮图片时出错了{0}", e.ToString());
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region Local Data Structure

    public enum ELimitiTimeGiftDataLimitType
    {
        None,//不限购
        Refresh,//刷新限购 (每天限购) limitnum 总量
        NotRefresh,//不刷新限购(永久限购) limittotalnum 总量
        Week,//每周限购 (每周限购) limitnum 总量
    }


    public class LimitTimeGiftData : IComparable<LimitTimeGiftData>
    {
        public MallItemInfo mallItemInfoData;
        private uint giftId;
        public uint GiftId
        {
            get { return giftId; }
            set { giftId = value; }
        }

        private string giftName;
        public string GiftName 
        {
            get{ return giftName;}
            set { giftName = value; }
        }

        private int limitPurchaseNum;
        public int LimitPurchaseNum 
        {
            get { return limitPurchaseNum; }
            set { limitPurchaseNum = value; }
        }
        private int limitNum;
        public int LimitNum
        {
            get { return limitNum; }
            set { limitNum = value; }
        }

        private int limitTotalNum;
        public int LimitTotalNum
        {
            get { return limitTotalNum; }
            set { limitTotalNum = value; }
        }

        private int limitLastNum;
        public int LimitLastNum
        {
            get { return limitLastNum; }
            set { limitLastNum = value; }
        }

        private List<LimitTimeGiftAwardData> giftAwards;
        public List<LimitTimeGiftAwardData> GiftAwards
        {
            get { return giftAwards; }
            set { giftAwards = value; }
        }

        private uint remainingTimeSec;
        public uint RemainingTimeSec 
        {
            get { return remainingTimeSec; }
            set { remainingTimeSec = value; }
        }
        /*
        private uint giftDescId;
        public uint GiftDescId
        {
            get { return giftDescId; }
            set { giftDescId = value; }
        }
        */
        private int giftPrice;
        public int GiftPrice
        {
            get { return giftPrice; }
            set { giftPrice = value; }
        }

        private ELimitiTimeGiftDataLimitType limitType;
        public ELimitiTimeGiftDataLimitType LimitType
        {
            get { return limitType; }
            set { limitType = value; }
        }

        private MallGoodsType giftType;
        public MallGoodsType GiftType
        {
            get { return giftType; }
            set { giftType = value; }
        }

        private LimitTimeGiftPriceType priceType;
        public LimitTimeGiftPriceType PriceType
        {
            get { return priceType; }
            set { priceType = value; }
        }

        private string giftIconPath;
        public string GiftIconPath
        {
            get { return giftIconPath; }
            set { giftIconPath = value; }
        }

        private uint giftStartTime;
        public uint GiftStartTime
        {
            get{return giftStartTime;}
            set{giftStartTime = value;}
        }

        private uint giftEndTime;
        public uint GiftEndTime
        {
            get{return giftEndTime;}
            set{giftEndTime = value;}
        }

        //private LimitTimeGiftState giftState;
        public LimitTimeGiftState GiftState
        {
            get {
                return  (limitPurchaseNum > 0 || this.limitType == ELimitiTimeGiftDataLimitType.None) ? LimitTimeGiftState.OnSale : LimitTimeGiftState.SoldOut;
            }
            //set { giftState = value; }
        }

        //一天内（限时）
        //private LimitTimeGiftIntraDay giftIntraDay;
        public LimitTimeGiftIntraDay GiftIntraDay
        {
            get {
                if (GiftState == LimitTimeGiftState.SoldOut)
                    return LimitTimeGiftIntraDay.None;
                return remainingTimeSec > 3600*24 ? LimitTimeGiftIntraDay.MoreThanOneDay : LimitTimeGiftIntraDay.IntraDay;
            }
            //set { giftIntraDay = value; }
        }
        #region DisplayData
        /*
        public string RemainingTimeStr
        {
            get { return TransTimeFormat((int)remainingTimeSec); }
        }
        */
        public string GiftDesc { get; set; }

        public bool NeedTimeCountDown
        {
            get {
                return GiftIntraDay == LimitTimeGiftIntraDay.IntraDay ? true : false; 
            }
        }

        public string GiftStartTimeToCN()
        {
            if (giftStartTime != 0)
            {
                return TransTimeStampToStr(giftStartTime);
            }
            return "";
        }

        public string GiftEndTimeToCN()
        {
            if (giftEndTime != 0)
            {
                return TransTimeStampToStr(giftEndTime);
            }
            return "";
        }

        #endregion

        
#region 三选一礼包
        private List<LimitTimeGiftData> threeToOneGifts;
        public List<LimitTimeGiftData> ThreeToOneGifts 
        {
            get { return threeToOneGifts; }
            set { threeToOneGifts = value; }
        }
        /*
        private uint threeToOneTypeId;
        public uint ThreeToOneTypeId
        {
            get{return threeToOneTypeId;}
            set{threeToOneTypeId = value;}
        }
         * * */
#endregion


        #region Method

        public LimitTimeGiftData()
        {
            Reset();
        }

        public void Reset()
        {
            giftId = uint.MaxValue;
            giftName = "";
            limitPurchaseNum = 0;
            giftAwards = null;
            threeToOneGifts = null;
            //threeToOneTypeId = 0;
            remainingTimeSec = 0;
            //giftDescId = 0;
            giftPrice = 0;
            limitType = ELimitiTimeGiftDataLimitType.None;
            priceType = LimitTimeGiftPriceType.Point;
            giftType = MallGoodsType.INVALID;
            giftIconPath = "";
            giftStartTime = 0;
            giftEndTime = 0;
            //giftState = LimitTimeGiftState.SoldOut;
            //giftIntraDay = LimitTimeGiftIntraDay.IntraDay;
        }

        #region Tools
        private string ReadGiftDescFromTable(uint giftId)
        {
            string desc = "勇士，强化时金币不足？我们为您准备了特价强化礼包，限时限量促销！";

            var limitTimeGiftTable = TableManager.GetInstance().GetTableItem<MallGiftPackTable>((int)giftId);
            if (limitTimeGiftTable != null)
            {
                return limitTimeGiftTable.giftDesc;
            }

            return desc;
        }

        private string TransTimeFormat(int second)
        {
            string showTime = "";
            if (second > 3600 * 24)
            {
                showTime = (second / (3600 * 24)).ToString();
                return showTime + "天";
            }
            int hour = second / 3600;
            int min = second % 3600 / 60;
            int sec = second % 60;
            if (hour < 10)
            {
                showTime += "0" + hour.ToString();
            }
            else
            {
                showTime += hour.ToString();
            }
            showTime += ":";
            if (min < 10)
            {
                showTime += "0" + min.ToString();
            }
            else
            {
                showTime += min.ToString();
            }
            showTime += ":";
            if (sec < 10)
            {
                showTime += "0" + sec.ToString();
            }
            else
            {
                showTime += sec.ToString();
            }
            return showTime;
        }
        private string TransTimeStampToStr(UInt32 timeStamp)
        {
            System.DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime dt = time.AddSeconds(timeStamp);// unix 总秒数
            return string.Format("{0}月{1}日", dt.Month, dt.Day);
        }

        #endregion

        #endregion

        public int CompareTo(LimitTimeGiftData obj)
        {
            return remainingTimeSec.CompareTo(obj.remainingTimeSec);
        }
    }

    public class LimitTimeGiftAwardData
    {
        private uint awardId;
        public uint AwardId 
        { 
            get { return awardId; }
            set { awardId = value; }
        }

        private uint awardCount;
        public uint AwardCount
        {
            get { return awardCount; }
            set { awardCount = value; }
        }

        private int strengthLevel;
        public int StrengthLevel
        {
            get { return strengthLevel; }
            set { strengthLevel = value; }
        }

        public void Reset()
        {
            awardId = 0;
            awardCount = 0;
            strengthLevel = 0;
        }

        public LimitTimeGiftAwardData()
        {
            Reset();
        }
    }

    public enum LimitTimeGiftPriceType
    {
        RMB = 0,
        Point = ItemTable.eSubType.POINT,
        BindPint = ItemTable.eSubType.BindPOINT,
        Gold=ItemTable.eSubType.GOLD,
        BindGOLD=ItemTable.eSubType.BindGOLD,
    }
    
    public enum LimitTimeGiftState
    {
        OnSale,
        SoldOut,
    }
   
    public enum LimitTimeGiftIntraDay
    {
        None,
        IntraDay, //一天内
        MoreThanOneDay,
    }

    public enum MallItemType
    {
        LimitTimeGift = 9,
        LimitTimeAct = 10,
        FashionAll = 11,
    }


    #region LimitTimeActivity

    //public class DateTimePublicInfo
    //{
    //    MallLimitTimeActivity.eActivityType activityType;
    //    RectTransform rect;
    //}

    #endregion


    #endregion
}