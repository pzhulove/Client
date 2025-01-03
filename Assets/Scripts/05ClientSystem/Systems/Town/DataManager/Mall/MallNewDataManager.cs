using Protocol;
using Network;
using ProtoTable;
using System.Collections.Generic;
using ActivityLimitTime;
using System;

namespace GameClient
{

    public class MallNewQueryItem
    {
        public int MallType;
        public int SubType;
        public int JobId;
        public List<MallItemInfo> Items;
    }

    /// <summary>
    /// 商城道具多本积分道具数据
    /// </summary>
    public class MallItemMultipleIntegralData
    {
        public int endTime;//结束时间
        public int multiple;//倍数
    }

    public class MallNewDataManager : DataManager<MallNewDataManager>
    {
        /// <summary>
        /// 商城商品对应多倍积分数据
        /// </summary>
        Dictionary<int, MallItemMultipleIntegralData> mallItemMultipleIntergralDict = new Dictionary<int, MallItemMultipleIntegralData>();

        //对应商城表中的时装类型和套装类型
        public const int FashionMallType = 4;
        public const int FashionSuitType = 6;

        private List<MallNewQueryItem> _mallNewQueryItemList = new List<MallNewQueryItem>();

        /// <summary>
        /// 积分商城积分是否超出上限值
        /// </summary>
        public bool bItemMallIntergralMallScoreIsExceed = false;

        /// <summary>
        /// 购买时装积分商城积分是否 等于上限值
        /// </summary>
        public bool bItemMallIntergralMallScoreIsEqual = false;

        private MallItemInfo queryMallItemInfo;
        /// <summary>
        /// 查询商城道具信息
        /// </summary>
        public MallItemInfo QueryMallItemInfo
        {
            get { return queryMallItemInfo; }
            set { queryMallItemInfo = value; }
        }
        public override void Initialize()
        {
            InitData();
            BindNetEvents();
        }

        private void InitData()
        {
            InitMallShopMultilTable();
        }

        /// <summary>
        /// 查询商品是否有多倍积分数据
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public MallItemMultipleIntegralData CheckMallItemMultipleIntegral(int itemId)
        {
            MallItemMultipleIntegralData data = null;
            if (mallItemMultipleIntergralDict.TryGetValue(itemId,out data))
            {

            }

            return data;
        }

        private void InitMallShopMultilTable()
        {
            if (mallItemMultipleIntergralDict == null)
            {
                mallItemMultipleIntergralDict = new Dictionary<int, MallItemMultipleIntegralData>();
            }

            var table = TableManager.GetInstance().GetTable<MallShopMultiITable>().GetEnumerator();
            while (table.MoveNext())
            {
                var mallShopMultiITable = table.Current.Value as MallShopMultiITable;
                if (mallShopMultiITable == null)
                {
                    continue;
                }

                string[] splits = mallShopMultiITable.Malls.Split('|');
                for (int i = 0; i < splits.Length; i++)
                {
                    int itemId = int.Parse(splits[i]);
                    DateTime time = DateTime.Parse(mallShopMultiITable.EndTime);
                    int timeStamp = (int)Function.ConvertDateTimeInt(time);

                    MallItemMultipleIntegralData data = new MallItemMultipleIntegralData()
                    {
                        multiple = mallShopMultiITable.Multiple,
                        endTime = timeStamp
                    };

                    mallItemMultipleIntergralDict.Add(itemId, data);
                }
            }
        }

        private void BindNetEvents()
        {
            NetProcess.AddMsgHandler(WorldMallQueryItemRet.MsgID, OnSyncWorldMallQueryItemRet);
            NetProcess.AddMsgHandler(SCMallBatchBuyRes.MsgID, OnSyncMallBatchBuyRes);
            NetProcess.AddMsgHandler(WorldMallBuyRet.MsgID, OnSyncWorldMallBuyRet);
        }

        public override void Clear()
        {
            ClearData();
            UnBindNetEvents();
            bItemMallIntergralMallScoreIsExceed = false;
            bItemMallIntergralMallScoreIsEqual = false;

            if (mallItemMultipleIntergralDict != null)
            {
                mallItemMultipleIntergralDict.Clear();
            }
        }

        public void ClearData()
        {
            for (var i = 0; i < _mallNewQueryItemList.Count; i++)
            {
                if (_mallNewQueryItemList[i].Items != null)
                {
                    _mallNewQueryItemList[i].Items.Clear();
                }
            }
            _mallNewQueryItemList.Clear();
        }

        private void UnBindNetEvents()
        {
            NetProcess.RemoveMsgHandler(WorldMallQueryItemRet.MsgID, OnSyncWorldMallQueryItemRet);
            NetProcess.RemoveMsgHandler(SCMallBatchBuyRes.MsgID, OnSyncMallBatchBuyRes);
            NetProcess.RemoveMsgHandler(WorldMallBuyRet.MsgID, OnSyncWorldMallBuyRet);
        }

        //购买单个商品，进行数据同步（限时商城和道具商城的元素）,更新剩余的次数
        private void OnSyncWorldMallBuyRet(MsgDATA msg)
        {
            if(msg == null)
                return;

            var worldMallBuyRet = new WorldMallBuyRet();
            worldMallBuyRet.decode(msg.bytes);

            //购买不成功
            if (worldMallBuyRet.code != (uint) ProtoErrorCode.SUCCESS)
            {
                //统一在ShopNewDataManager处理
                // SystemNotifyManager.SystemNotify((int) worldMallBuyRet.code);
                return;
            }

            //宠物推送界面购买道具数据处理
            if (ActivityLimitTimeCombineManager.GetInstance().GiftDataManager != null)
            {
                List<MallItemInfo> mPetPushItemInfo = ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.GetPetPushItemInfo();

                if (mPetPushItemInfo != null)
                {
                    for (int i = 0; i < mPetPushItemInfo.Count; i++)
                    {
                        var info = mPetPushItemInfo[i];
                        if (info.id != worldMallBuyRet.mallitemid)
                        {
                            continue;
                        }

                        info.limittotalnum = (ushort)worldMallBuyRet.restLimitNum;
                    }

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSyncWorldBuyPetSucceed, mPetPushItemInfo);
                }
            }

            //更新账号购买的剩余数量
            UpdateMallItemInfoAccountLeftNumber(worldMallBuyRet.mallitemid,
                worldMallBuyRet.accountRestBuyNum);

            //购买成功，发送UI时间，对剩余限购次数进行同步
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSyncWorldMallBuySucceed,
                worldMallBuyRet.mallitemid,
                worldMallBuyRet.restLimitNum,
                (int)worldMallBuyRet.accountRestBuyNum);
        }
        

        //批量购买成功，进行同步操作（主要是时装）
        private void OnSyncMallBatchBuyRes(MsgDATA msg)
        {

            if (msg == null)
                return;

            var res = new SCMallBatchBuyRes();
            res.decode(msg.bytes);

            if (res.code != (uint) ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int) res.code);
                return;
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSyncMallBatchBuySucceed, res);
        }

        //主要是处理时装商城中套装类别的第一个元素,
        //将推荐值最大的套装放在第一个
        private void AdjustMallItems(List<MallItemInfo> mallItems)
        {
            if(mallItems == null || mallItems.Count <= 0)
                return;

            var mallType = (int) mallItems[0].type;
            var mallSubType = (int) mallItems[0].subtype;
            //非时装，非套装，直接返回不处理
            if (mallType != FashionMallType
                || mallSubType != FashionSuitType)
            {
                return;
            }

            //时装类型下的套装类型：选在isRecommend最大的值作为推荐Item
            var maxRecommendValue = mallItems[0].isRecommend;
            var maxRecommendIndex = 0;
            for (var i = 0; i < mallItems.Count; i++)
            {
                if (mallItems[i].isRecommend > maxRecommendValue)
                {
                    maxRecommendValue = mallItems[i].isRecommend;
                    maxRecommendIndex = i;
                }
            }

            //将MaxReccomdIndex的元素删除之后，放置在0的位置
            if (maxRecommendIndex != 0)
            {
                var maxRecommendItem = mallItems[maxRecommendIndex];
                mallItems.RemoveAt(maxRecommendIndex);
                mallItems.Insert(0, maxRecommendItem);
            }
        }

        //同步商城数据
        private void OnSyncWorldMallQueryItemRet(MsgDATA msg)
        {

            var res = new WorldMallQueryItemRet();
            res.decode(msg.bytes);

            //同类时装之显示永久时限的时装
            res.items = FashionLimitTimeBuy.FashionLimitTimeBuyManager.GetInstance().FashionLimitTimeFilter(res.items);
            if(res.items == null)
                return;

            var sortMallItems = new List<MallItemInfo>();
            for (var i = 0; i < res.items.Length; i++)
            {
                sortMallItems.Add(res.items[i]);
            }

            if (sortMallItems.Count > 0)
            {
                var resMallType = (MallTypeTable.eMallType)res.type;
                if (resMallType == MallTypeTable.eMallType.SN_GIFT)
                {
                    //按照remainSec排序,限时礼包
                    sortMallItems.Sort((x, y) => x.endtime.CompareTo(y.endtime));
                }
                else if (resMallType == MallTypeTable.eMallType.SN_RECOMMEND
                         || resMallType == MallTypeTable.eMallType.SN_MATERIAL
                         || resMallType == MallTypeTable.eMallType.SN_COST
                         || resMallType == MallTypeTable.eMallType.SN_GOLD)
                {
                    //道具商城的四个页签：推荐，消耗品，材料，金币
                    //按照SortID进行排序
                    sortMallItems.Sort((x, y) => x.sortIdx.CompareTo(y.sortIdx));
                }
                else
                {
                    //按照SortId进行排序
                    sortMallItems.Sort((x, y) => x.sortIdx.CompareTo(y.sortIdx));
                }
            }

            AdjustMallItems(sortMallItems);

            var mallType = (int) res.type;
            var subType = 0;
            var jobId = 0;
            if ((MallTypeTable.eMallType) res.type == MallTypeTable.eMallType.SN_FASHION ||
                (MallTypeTable.eMallType) res.type == MallTypeTable.eMallType.SN_GIFT)
            {
                if (sortMallItems.Count > 0)
                {
                    subType = sortMallItems[0].subtype;
                    jobId = sortMallItems[0].jobtype;
                }
            }
            
            var find = false;      //同类型的MallItemList是否存在
            for (var i = 0; i < _mallNewQueryItemList.Count; i++)
            {
                var queryItem = _mallNewQueryItemList[i];
                if (queryItem.MallType == mallType
                    && queryItem.SubType == subType
                    && queryItem.JobId == jobId)
                {
                    //如果ItemList保存过，则更新数据
                    find = true;
                    queryItem.Items = sortMallItems;
                    break;
                }
            }

            //以前没有保存，保存相应的数据
            if (find == false)
            {
                var queryItem = new MallNewQueryItem()
                {
                    MallType = mallType,
                    SubType = subType,
                    JobId = jobId,
                    Items = sortMallItems
                };
                _mallNewQueryItemList.Add(queryItem);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSyncWorldMallQueryItems,
                mallType,subType, jobId);
        }

        //获取触发礼包列表
        public void GetTriggerGiftMallList()
        {
            MallNewDataManager.GetInstance().SendWorldMallQueryItemReq(9, 3);
        }

        //发送请求
        //子类型从1-5
        //mallSubTypeIndex, JobId 两个参数只在时装用到。SubTypeIndex用于判断时装的部位(针对时装的单品)，JobId用于判断职业
        //现在限时商城也会用到mallSubTypeIndex 1表示限时礼包 2表示节日礼包 3表示触发礼包
        public void SendWorldMallQueryItemReq(int mallTableId, int mallSubTypeIndex = 0, int jobId = 0)
        {
            var mallData = TableManager.GetInstance().GetTableItem<MallTypeTable>(mallTableId);
            if (mallData == null)
            {
                Logger.LogErrorFormat("The mallData is null and mallTableId is {0}", mallTableId);
                return;
            }

            var req = new WorldMallQueryItemReq();
            if (mallData.MoneyID != 0)
            {
                req.moneyType = (byte) mallData.MoneyID;
            }

            if (mallData.MallType == MallTypeTable.eMallType.SN_HOT)
            {
                req.tagType = 1;
            }
            else
            {
                req.tagType = 0;
                req.type = (byte) mallData.MallType;

                if (mallData.MallSubType != null)
                {
                    if (mallData.MallSubType.Count == 1)
                    {
                        req.subType = (byte) mallData.MallSubType[0];
                    }
                    else
                    {
                        if (mallData.MallSubType.Count > 0
                            && mallSubTypeIndex >= 1
                            && mallSubTypeIndex <= mallData.MallSubType.Count
                            && mallData.MallSubType[mallSubTypeIndex -1] != 0)
                        {
                            req.subType = (byte) mallData.MallSubType[mallSubTypeIndex - 1];
                        }
                    }
                }

                if (mallData.ClassifyJob == 1
                    && jobId > 0)
                {
                    req.occu = (byte) jobId;
                }
            }

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);

        }

        public List<MallItemInfo> GetMallItemInfoList(int mallType, int subType = 0, int jobId = 0)
        {
            if (_mallNewQueryItemList == null || _mallNewQueryItemList.Count <= 0)
                return null;

            for (var i = 0; i < _mallNewQueryItemList.Count; i++)
            {
                var queryItem = _mallNewQueryItemList[i];
                if (queryItem.MallType == mallType
                    && queryItem.SubType == subType
                    && queryItem.JobId == jobId)
                {
                    return queryItem.Items;
                }
            }

            return null;
        }

        //在数据层，更新账号购买剩余的数量
        private void UpdateMallItemInfoAccountLeftNumber(uint mallItemInfoId, uint accountLeftNumber)
        {
            if (_mallNewQueryItemList == null || _mallNewQueryItemList.Count <= 0)
                return;

            for (var i = 0; i < _mallNewQueryItemList.Count; i++)
            {
                var queryItem = _mallNewQueryItemList[i];
                if (queryItem != null
                    && queryItem.Items != null
                    && queryItem.Items.Count > 0)
                {
                    for (var j = 0; j < queryItem.Items.Count; j++)
                    {
                        var curMallItemInfo = queryItem.Items[j];
                        if (curMallItemInfo != null && curMallItemInfo.id == mallItemInfoId)
                        {
                            curMallItemInfo.accountRestBuyNum = accountLeftNumber;
                        }
                    }
                }
            }
        }

        //得到时装的ItemId， 分为套装和单品两种类型
        public int GetFashionItemId(MallItemInfo itemInfo, FashionMallClothTabType clothTabType )
        {
            if (itemInfo == null)
                return 0;
            if (clothTabType == FashionMallClothTabType.Suit)
            {
                if (itemInfo.giftItems != null &&
                    itemInfo.giftItems.Length == (int) MallTypeTable.eMallSubType.MST_ALL - 1)
                {
                    var suitWearIndex = (int) MallTypeTable.eMallSubType.MST_UPWEAR - 1;
                    return (int)itemInfo.giftItems[suitWearIndex].id;
                }
            }

            return (int)itemInfo.itemid;
        }

        public ItemTable GetCostItemTableByCostType(byte costType)
        {
            //根据消耗品类型，获得消耗品的ID
            var costId = ItemDataManager.GetInstance().GetMoneyIDByType((ItemTable.eSubType)costType);
            
            //获得itemTableId;
            var itemTable = TableManager.instance.GetTableItem<ItemTable>(costId);
            //if(itemTable == null)
            //{
            //    //Logger.LogErrorFormat("costType is {0}, costId is {1}", costType, costId);
            //}
            return itemTable;
        }

        //just test
        //private int testFirstIndex = 0;
        //private int testSecondIndex = 0;

        //private string[] testResult =
        //{
        //    "2|0|1|0",
        //    "2|0|1|1",
        //    "2|0|1|2",
        //    "2|0|1|3",
        //    "2|0|1|4",

        //};

        //public void TestOpenMallNewFrame()
        //{
        //    var count = testResult.Length;
        //    if (testFirstIndex < 0 || testFirstIndex >= count)
        //        testFirstIndex = 0;

        //    MallNewFrame.OpenLinkFrame(testResult[testFirstIndex]);

        //    testFirstIndex += 1;
        //}

            /// <summary>
            /// 查询道具
            /// </summary>
            /// <param name="itemId"></param>
            /// <returns></returns>
        public void ReqQueryMallItemInfo(int itemId)
        {
            WorldGetMallItemByItemIdReq msg = new WorldGetMallItemByItemIdReq();
            msg.itemId = (uint)itemId;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<WorldGetMallItemByItemIdRes>(msgRet =>
            {
                if (msgRet.retCode != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.retCode);
                    return;
                }

                queryMallItemInfo = msgRet.mallItem;

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnQueryMallItenInfoSuccess);
            }, false);
        }
    }
}
