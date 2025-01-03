using Protocol;
using System.Collections.Generic;
using ProtoTable;
using System;
///////删除linq
using Network;
using UnityEngine.Assertions.Must;

namespace GameClient
{
    /// <summary>
    /// 拍卖行冻结提醒数据
    /// </summary>
    public class AuctionFreezeRemindData
    {
        /// <summary>
        /// 冻结金币类型
        /// </summary>
        public UInt32 FrozenMoneyType { get; set; }
        /// <summary>
        /// 冻结金币数量
        /// </summary>
        public UInt32 FrozenAmount { get; set; }
        /// <summary>
        /// 异常交易时间
        /// </summary>
        public UInt32 AbnormalDealTimeStamp { get; set; }
        /// <summary>
        /// 结束保释期时间
        /// </summary>
        public UInt32 BailFinishedTimeStamp { get; set; }
        /// <summary>
        /// 保释期剩余天数
        /// </summary>
        public UInt32 RemainDayNumber { get; set; }
        /// <summary>
        /// 返回金额
        /// </summary>
        public UInt32 BackAmount { get; set; }
        /// <summary>
        /// 提前天数
        /// </summary>
        public UInt32 BackDayNumber { get; set; }
    }

    public enum AuctionNewOnShelfState
    {
        None = 0,
        OwnerItem,
        Empty,
        BuyField,
    }

    //在售架子上的数据
    public class AuctionNewOnShelfDataModel
    {
        public AuctionNewOnShelfState onShelfState;
        public AuctionBaseInfo auctionBaseInfo;
        public AuctionBoothTable boothTableData;
    }

    //打开界面传递的数据
    public class AuctionNewMagicCardDataModel
    {
        public uint ItemId;
        public int DefaultLevel;
    }

    //强化等级的数据模型
    public class AuctionNewMagicCardStrengthenLevelDataModel
    {
        public int StrengthenLevel;
        public int Number;
        public bool IsSelected;
    }

    public class AuctionNewFilterData
    {
        public AuctionNewFrameTable.eFilterItemType FilterItemType;     //过滤器的类型
        public int Id;                           //对应的ID
        public string Name;                         //名字
        public int Sort;
        public bool IsSelected;                     //是否被选中，只是用于过滤器列表中的选中框的战士
        public AuctionNewFilterTable AuctionNewFilterTable;
    }

    public class AuctionNewItemDetailData
    {
        public int Type;
        public List<AuctionBaseInfo> ItemDetailDataList;
        public int CurPage;
        public int MaxPage;
        public int NoticeType;          //0 非关注页签，1关注页签
    }

    public class AuctionNewMenuTabDataModel
    {
        public int Id;      //对应拍卖行结构表中的ID
        public int Layer;   //页签的层级：1-3
        public int Sort;
        public AuctionNewFrameTable AuctionNewFrameTable;
        public bool IsOwnerChildren = false;
        public AuctionNewMenuTabDataModel(int id,
            int layer, 
            int sort,
            AuctionNewFrameTable auctionNewFrameTable)
        {
            Id = id;
            Layer = layer;
            Sort = sort;
            AuctionNewFrameTable = auctionNewFrameTable;
            SetOwnerChildrenFlag();
        }

        public void SetOwnerChildrenFlag()
        {
            //默认为true
            IsOwnerChildren = true;

            if (AuctionNewFrameTable.LayerRelation == null
                || AuctionNewFrameTable.LayerRelation.Count == 0
                || (AuctionNewFrameTable.LayerRelation.Count == 1
                    && AuctionNewFrameTable.LayerRelation[0] == 0))
            {
                IsOwnerChildren = false;
            }
        }
    }

    //上一次上架道具的数据模型
    public class AuctionNewPreviewOnShelfItemData
    {
        public int ItemId;
        public int StrengthLevel;
        public int OnShelfPrice;

        public AuctionNewPreviewOnShelfItemData()
        {
            ItemId = 0;
            StrengthLevel = 0;
            OnShelfPrice = 0;
        }

        public void ResetItemData()
        {
            ItemId = 0;
            StrengthLevel = 0;
            OnShelfPrice = 0;
        }
    }
   
    //拍卖行数据管理
    public class AuctionNewDataManager : DataManager<AuctionNewDataManager>
    {

        //附魔卡查询，0的时候，代表全部等级，如果只查询0级的附魔卡，(和服务器确定，等级为100）；
        public int DefaultMagicCardZeroStrengthenLevelQuery = 100;

        public int MaxAddShelfNum = 5; //最大的栏位数目
        public int BaseShelfNum = 5;   //基础的栏位数目，两者都是由表中配置
        public int OnShelfItemNumber = 0;       //在架上商品的数量
        public int TreasureItemRushBuyTimeInterval = 5;         //珍品抢购时间
        public int TreasureItemRecommendPriceRate = 1;          //珍品道具上架价格倍数
        public int PageNumber = 6;       //每页的数量,由表中数据决定。如果表中不存在，默认为6。

        private const uint SubTypeCoefficient = 1000;       //子类型的系数

        private Dictionary<int, List<AuctionNewFilterData>> _auctionNewFilterDataDictionary =
            new Dictionary<int, List<AuctionNewFilterData>>();

        //商品数量的缓存
        private Dictionary<int, List<AuctionItemBaseInfo>> _auctionNewItemNumResDictionary =
            new Dictionary<int, List<AuctionItemBaseInfo>>();

        private Dictionary<int, AuctionNewItemDetailData> _auctionNewItemDetailResDictionary =
            new Dictionary<int, AuctionNewItemDetailData>();

        //销售
        private List<EPackageType> _packageTypeList = new List<EPackageType>();
        private List<ulong> _packageSellItemUidList = new List<ulong>();
        private List<AuctionSellItemData> _packageSellItemDataList = new List<AuctionSellItemData>();

        //上架数据
        private List<AuctionNewOnShelfDataModel> _onShelfDataModelList = new List<AuctionNewOnShelfDataModel>();

        //珍品交易记录
        private List<AuctionTransaction> _treasureItemSaleRecordList = new List<AuctionTransaction>();
        private List<AuctionTransaction> _treasureItemBuyRecordList = new List<AuctionTransaction>();

        //记录上次打开的数据信息
        private AuctionNewUserData _lastTimeAuctionNewUserData;

        //上架物品的详细信息
        private Dictionary<UInt64, ItemData> _itemDataDetailDictionary = new Dictionary<UInt64, ItemData>();

        //上架的提示
        public bool IsNotShowOnShelfTipOfTreasureItem = false;      //不显示珍品的上架提示
        public bool IsNotShowOnShelfTipOfNormalItem = false;        //不显示非珍品的上架提示
        //上架提示对应的通用提示表的ID
        public const int OnShelfTipIdOnTreasureItem = 9961;         //珍品
        public const int OnShelfTipIdOnNormalItem = 9962;            //非珍品
        public const int ShelfAgainTreasureItem = 10016;             //重新上架珍品

        public const int ItemForeverFreezeDays = 4000;  //时间超过4000天，表示永久冻结，超过了10年

        //查询价格的返回数据
        private WorldAuctionQueryItemPricesRes _worldAuctionQueryItemPriceRes;
        //查询价格列表返回的数据
        private WorldAuctionQueryItemPriceListRes _worldAuctionQueryItemPriceListRes;
        //查询最近交易列表返回的数据
        private WorldAuctionQueryItemTransListRes _worldAuctionQueryItemTransListRes;
        private WorldAuctionSyncPubPageIds _worldAuctionSyncPubPageIds;

        //auctionNewFrameTable中的ID，对应的所有道具的字典。避免重复的查找
        private Dictionary<int, List<int>> _auctionNewItemIdDictionary = new Dictionary<int, List<int>>();

        private AuctionNewPreviewOnShelfItemData _previewOnShelfItemData = new AuctionNewPreviewOnShelfItemData();

        private WorldAuctionQueryMagicOnsalesRes _worldAuctionQueryMagicOnSalesRes;

        #region InitAndResetData

        public override void Initialize()
        {
            ResetData();
            InitData();
            BindEvents();
        }

        private void InitData()
        {
            var auctionBoothTabData = TableManager.GetInstance().GetTable<AuctionBoothTable>();
            if (auctionBoothTabData != null)
                MaxAddShelfNum = auctionBoothTabData.Count;

            var baseShelfNumData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_ITEMAUCTION_MAX_ONSALENUM);
            if (baseShelfNumData != null)
            {
                BaseShelfNum = baseShelfNumData.Value;
            }

            var pageMaxNumData = TableManager.GetInstance()
                .GetTableItem<SystemValueTable>((int) SystemValueTable.eType2.SVT_AUCTION_QUERY_PAGE_MAXNUM);
            if (pageMaxNumData != null && pageMaxNumData.Value > 0)
                PageNumber = pageMaxNumData.Value;

            var treasureItemBuyTimeData = TableManager.GetInstance()
                .GetTableItem<SystemValueTable>((int) SystemValueTable.eType2.SVT_AUCTION_RUSH_BUY_TIME);
            if (treasureItemBuyTimeData != null)
            {
                TreasureItemRushBuyTimeInterval = treasureItemBuyTimeData.Value;
            }

            var treasureItemRecommendPriceRateData = TableManager.GetInstance()
                .GetTableItem<SystemValueTable>((int) SystemValueTable.eType2.SVT_TUIJIANJIAGE_BEISHU);
            if (treasureItemRecommendPriceRateData != null && treasureItemRecommendPriceRateData.Value > 0)
                TreasureItemRecommendPriceRate = treasureItemRecommendPriceRateData.Value;

        }

        public override void Clear()
        {
            ResetData();
            UnBindEvents();
        }

        private void BindEvents()
        {
            //在售数量
            NetProcess.AddMsgHandler(WorldAuctionItemNumRes.MsgID, OnReceiveAuctionItemNumRes);
            //在售详细信息
            NetProcess.AddMsgHandler(WorldAuctionListQueryRet.MsgID, OnReceiveAuctionItemDetailListRes);
            NetProcess.AddMsgHandler(WorldAuctionNotifyRefresh.MsgID, OnWorldAuctionNotifyRefreshRes);
            //自己上架的信息
            NetProcess.AddMsgHandler(WorldAuctionSelfListRes.MsgID, OnReceiveWorldAuctionSelfListRes);
            NetProcess.AddMsgHandler(SceneAuctionBuyBoothRes.MsgID, OnReceiveBuyShelfRes);
            NetProcess.AddMsgHandler(WorldAuctionGetTreasTransactionRes.MsgID, OnReceiveAuctionNewTreasureTransactionRes);
            NetProcess.AddMsgHandler(WorldAuctionQueryItemRet.MsgID, OnReceiveWorldAuctionQueryItemRet);

            //查询准备上架物品价格的返回
            NetProcess.AddMsgHandler(WorldAuctionQueryItemPricesRes.MsgID, OnReceiveWorldAuctionQueryItemPriceRes);
            //查询公示或者在售物品价格的列表
            NetProcess.AddMsgHandler(WorldAuctionQueryItemPriceListRes.MsgID, OnReceiveWorldAuctionQueryItemPriceListRes);
            //查询最近交易记录的列表
            NetProcess.AddMsgHandler(WorldAuctionQueryItemTransListRes.MsgID,
                OnReceiveWorldAuctionQueryItemTransListRes);

            //关注页签的同步
            NetProcess.AddMsgHandler(WorldAuctionSyncPubPageIds.MsgID, OnReceiveWorldAuctionSyncPubPageIds);

            //关注道具的返回
            NetProcess.AddMsgHandler(WorldActionAttentRes.MsgID, OnReceiveWorldActionNoticeRes);

            //附魔卡相关
            NetProcess.AddMsgHandler(WorldAuctionQueryMagicOnsalesRes.MsgID, OnReceiveAuctionNewMagicCardOnSaleRes);

            //金币冻结
            NetProcess.AddMsgHandler(SceneNotifyAbnormalTransaction.MsgID, SceneNotifyAbnormalTransactionRet);
            NetProcess.AddMsgHandler(SceneAbnormalTransactionRecordRes.MsgID, SceneAbnormalTransactionRecordRet);
        }

        private void UnBindEvents()
        {
            NetProcess.RemoveMsgHandler(WorldAuctionItemNumRes.MsgID, OnReceiveAuctionItemNumRes);
            NetProcess.RemoveMsgHandler(WorldAuctionListQueryRet.MsgID, OnReceiveAuctionItemDetailListRes);
            NetProcess.RemoveMsgHandler(WorldAuctionNotifyRefresh.MsgID, OnWorldAuctionNotifyRefreshRes);
            NetProcess.RemoveMsgHandler(WorldAuctionSelfListRes.MsgID, OnReceiveWorldAuctionSelfListRes);
            NetProcess.RemoveMsgHandler(SceneAuctionBuyBoothRes.MsgID, OnReceiveBuyShelfRes);
            NetProcess.RemoveMsgHandler(WorldAuctionGetTreasTransactionRes.MsgID, OnReceiveAuctionNewTreasureTransactionRes);
            NetProcess.RemoveMsgHandler(WorldAuctionQueryItemRet.MsgID, OnReceiveWorldAuctionQueryItemRet);

            NetProcess.RemoveMsgHandler(WorldAuctionQueryItemPricesRes.MsgID, OnReceiveWorldAuctionQueryItemPriceRes);
            NetProcess.RemoveMsgHandler(WorldAuctionQueryItemPriceListRes.MsgID, OnReceiveWorldAuctionQueryItemPriceListRes);
            NetProcess.RemoveMsgHandler(WorldAuctionQueryItemTransListRes.MsgID,
                OnReceiveWorldAuctionQueryItemTransListRes);

            NetProcess.RemoveMsgHandler(WorldAuctionSyncPubPageIds.MsgID, OnReceiveWorldAuctionSyncPubPageIds);

            NetProcess.RemoveMsgHandler(WorldActionAttentRes.MsgID, OnReceiveWorldActionNoticeRes);

            //附魔卡相关
            NetProcess.RemoveMsgHandler(WorldAuctionQueryMagicOnsalesRes.MsgID, OnReceiveAuctionNewMagicCardOnSaleRes);

            NetProcess.RemoveMsgHandler(SceneNotifyAbnormalTransaction.MsgID, SceneNotifyAbnormalTransactionRet);
            NetProcess.RemoveMsgHandler(SceneAbnormalTransactionRecordRes.MsgID, SceneAbnormalTransactionRecordRet);
        }

        private void ResetData()
        {
            //清空过滤器
            var filterDataIter = _auctionNewFilterDataDictionary.GetEnumerator();
            while (filterDataIter.MoveNext())
            {
                var filterDataList = filterDataIter.Current.Value;
                if(filterDataList != null)
                    filterDataList.Clear();
            }
            _auctionNewFilterDataDictionary.Clear();

            var itemNumResIter = _auctionNewItemNumResDictionary.GetEnumerator();
            while (itemNumResIter.MoveNext())
            {
                var itemResList = itemNumResIter.Current.Value;
                if(itemResList != null)
                    itemResList.Clear();
            }
            _auctionNewItemNumResDictionary.Clear();

            _auctionNewItemDetailResDictionary.Clear();
            
            _onShelfDataModelList.Clear();
            _treasureItemBuyRecordList.Clear();
            _treasureItemSaleRecordList.Clear();
            _lastTimeAuctionNewUserData = null;
            _itemDataDetailDictionary.Clear();

            IsNotShowOnShelfTipOfNormalItem = false;
            IsNotShowOnShelfTipOfTreasureItem = false;

            _worldAuctionQueryItemPriceRes = null;
            _worldAuctionQueryItemPriceListRes = null;
            _worldAuctionQueryItemTransListRes = null;
            _worldAuctionSyncPubPageIds = null;

            _worldAuctionQueryMagicOnSalesRes = null;

            ResetAuctionNewItemIdDictionary();

            PageNumber = 6;

            ResetPreviewOnShelfItemData();
            _packageTypeList.Clear();
        }
        
        #endregion

        #region AuctionNewFilterData

        //过滤器的相关数据
        public List<AuctionNewFilterData> GetAuctionNewFilterDataList(AuctionNewFrameTable.eFilterItemType filterType)
        {
            if (filterType <= AuctionNewFrameTable.eFilterItemType.FIT_NONE 
                || filterType > AuctionNewFrameTable.eFilterItemType.FIT_JOB)
                return null;

            List<AuctionNewFilterData> auctionNewFilterDataList = null;

            var filterTypeIndex = (int) filterType;

            if (_auctionNewFilterDataDictionary.TryGetValue(filterTypeIndex, out auctionNewFilterDataList) == false)
            {
                auctionNewFilterDataList = InitAuctionNewFilterDataListByFilterTypeIndex(filterTypeIndex);
                if (_auctionNewFilterDataDictionary.ContainsKey(filterTypeIndex) == true)
                {
                    _auctionNewFilterDataDictionary[filterTypeIndex] = auctionNewFilterDataList;
                }
                else
                {
                    _auctionNewFilterDataDictionary.Add(filterTypeIndex, auctionNewFilterDataList);
                }
            }

            return auctionNewFilterDataList;
        }

        //过滤器默认展示的数据
        public AuctionNewFilterData GetDefaultAuctionNewFilterData(AuctionNewFrameTable.eFilterItemType filterType,
            int filterSortType)
        {
            var auctionNewFilterDataList = GetAuctionNewFilterDataList(filterType);
            if (auctionNewFilterDataList == null || auctionNewFilterDataList.Count <= 0)
                return null;

            if (filterSortType <= 1)
                return auctionNewFilterDataList[0];

            var defaultFilterData = auctionNewFilterDataList[0];

            //自身等级适应
            if (filterSortType == 2)
            {
                if (defaultFilterData.FilterItemType != AuctionNewFrameTable.eFilterItemType.FIT_LEVEL)
                    return defaultFilterData;

                var playerLevel = PlayerBaseData.GetInstance().Level;

                for (var i = auctionNewFilterDataList.Count -1; i >= 1; i--)
                {
                    var filterTableId = auctionNewFilterDataList[i].Id;
                    var auctionNewFilterTable = auctionNewFilterDataList[i].AuctionNewFilterTable;
                        

                    if (auctionNewFilterTable.Parameter != null && auctionNewFilterTable.Parameter.Count == 2)
                    {
                        var preLevel = auctionNewFilterTable.Parameter[0];
                        var nextLevel = auctionNewFilterTable.Parameter[1];
                        if ((playerLevel >= preLevel && playerLevel <= nextLevel)
                            || (playerLevel <= preLevel && playerLevel >= nextLevel))
                            return auctionNewFilterDataList[i];
                    }
                }
            }
            else if (filterSortType == 3)
            {
                //根据自身职业自适应
                if (defaultFilterData.FilterItemType != AuctionNewFrameTable.eFilterItemType.FIT_JOB)
                    return defaultFilterData;

                var playerProfessionId = PlayerBaseData.GetInstance().JobTableID;
                //基础职业
                var baseProfessionId = Utility.GetBaseJobID(playerProfessionId);

                for (var i = auctionNewFilterDataList.Count - 1; i >= 1; i--)
                {
                    var curAuctionNewFilterData = auctionNewFilterDataList[i];
                    var auctionNewFilterTable = auctionNewFilterDataList[i].AuctionNewFilterTable;

                    if (auctionNewFilterTable.Parameter != null && auctionNewFilterTable.Parameter.Count > 0)
                    {
                        for (var j = 0; j < auctionNewFilterTable.Parameter.Count; j++)
                        {
                            var filterProfessionId = auctionNewFilterTable.Parameter[j];
                            if (filterProfessionId == baseProfessionId)
                                return curAuctionNewFilterData;
                        }
                    }
                }
            }
            return defaultFilterData;
        }

        private List<AuctionNewFilterData> InitAuctionNewFilterDataListByFilterTypeIndex(int filterTypeIndex)
        {
            List<AuctionNewFilterData> auctionNewFilterDataList = new List<AuctionNewFilterData>();

            var filterTables = TableManager.GetInstance().GetTable<AuctionNewFilterTable>();
            var iter = filterTables.GetEnumerator();
            while (iter.MoveNext())
            {
                var curFilterTable = iter.Current.Value as AuctionNewFilterTable;
                if (curFilterTable != null 
                    && curFilterTable.FilterItemType == filterTypeIndex)
                {
                    var auctionNewFilterData = new AuctionNewFilterData
                    {
                        FilterItemType = (AuctionNewFrameTable.eFilterItemType) filterTypeIndex,
                        Id = curFilterTable.ID,
                        Sort = curFilterTable.Sort,
                        Name = curFilterTable.Name,
                        AuctionNewFilterTable = curFilterTable,
                    };
                    auctionNewFilterDataList.Add(auctionNewFilterData);
                }
            }

            auctionNewFilterDataList.Sort((x, y) => x.Sort.CompareTo(y.Sort));

            return auctionNewFilterDataList;
        }

        #endregion

        #region AuctionNewOnSaleItemData
        //获得在售的列表
        public void SendAuctionNewOnSaleItemListReq(AuctionNewMainTabType auctionNewMainTabType,
            AuctionNewMenuTabDataModel firstLayerMenuTabDataModel,
            AuctionNewMenuTabDataModel secondLayerMenuTabDataModel,
            AuctionNewMenuTabDataModel thirdLayerMenuTabDataModel,
            AuctionNewFilterData firstFilterData = null,
            AuctionNewFilterData secondFilterData = null,
            AuctionNewFilterData thirdFilterData = null)
        {
            if (auctionNewMainTabType != AuctionNewMainTabType.AuctionBuyType
                && auctionNewMainTabType != AuctionNewMainTabType.AuctionNoticeType)
            {
                Logger.LogErrorFormat("AuctionNewMainTabType is Error ");
                return;
            }

            if (firstLayerMenuTabDataModel == null || firstLayerMenuTabDataModel.AuctionNewFrameTable == null)
            {
                Logger.LogErrorFormat("FirstLayerMenuTabDataModel is Error");
                return;
            }

            if (secondLayerMenuTabDataModel == null || secondLayerMenuTabDataModel.AuctionNewFrameTable == null)
            {
                Logger.LogErrorFormat("SecondLayerMenuTabDataModel is Error");
                return;
            }

            WorldAuctionItemNumReq req = new WorldAuctionItemNumReq();

            #region MenuDataModel

            //主要类型和状态
            req.cond.itemMainType = (byte) firstLayerMenuTabDataModel.AuctionNewFrameTable.MainItemType;

            req.cond.type = (byte)AuctionType.Item;
            //商品状态，在售还是公示
            var auctionGoodState = AuctionNewUtility.ConvertToAuctionGoodState(auctionNewMainTabType);
            req.cond.goodState = (byte)auctionGoodState;

            //由第二层的DataModel决定类型
            if (thirdLayerMenuTabDataModel == null || thirdLayerMenuTabDataModel.AuctionNewFrameTable == null)
            {
                //使用自身的subType和ThirdType
                if (secondLayerMenuTabDataModel.AuctionNewFrameTable.DeleteLayerID.Count == 0
                    || (secondLayerMenuTabDataModel.AuctionNewFrameTable.DeleteLayerID.Count == 1
                        && secondLayerMenuTabDataModel.AuctionNewFrameTable.DeleteLayerID[0] == 0))
                {
                    var itemSubTypeList = GetItemSubTypes(secondLayerMenuTabDataModel.AuctionNewFrameTable);
                    req.cond.itemSubTypes = itemSubTypeList.ToArray();
                }
                else
                {
                    //使用排除子序列
                    var deleteLayerIdNumber = secondLayerMenuTabDataModel.AuctionNewFrameTable.DeleteLayerID.Length;
                    var deleteSubItemTypeList = new List<uint>();

                    for (var i = 0; i < deleteLayerIdNumber; i++)
                    {
                        var deleteLayerId = secondLayerMenuTabDataModel.AuctionNewFrameTable.DeleteLayerID[i];
                        var deleteAuctionNewFrameTable =
                            TableManager.GetInstance().GetTableItem<AuctionNewFrameTable>(deleteLayerId);

                        if(deleteAuctionNewFrameTable == null)
                            continue;

                        var curSubItemTypeList = GetItemSubTypes(deleteAuctionNewFrameTable);
                        deleteSubItemTypeList.AddRange(curSubItemTypeList);
                    }

                    req.cond.excludeItemSubTypes = deleteSubItemTypeList.ToArray();
                }
            }
            else
            {
                //第三层的DataModel 非空。可能单独使用第三层的DataModel， 或者第三层和第二层dataModel的复合
                var thirdType = thirdLayerMenuTabDataModel.AuctionNewFrameTable.ThirdType;
                var secondSubType = thirdLayerMenuTabDataModel.AuctionNewFrameTable.SubType;

                if (thirdType.Count == 1 && thirdType[0] == -1)
                {
                    thirdType = secondLayerMenuTabDataModel.AuctionNewFrameTable.ThirdType;
                }

                if (secondSubType.Count == 1 && secondSubType[0] == -1)
                {
                    secondSubType = secondLayerMenuTabDataModel.AuctionNewFrameTable.SubType;
                }

                var subTypeNumber = secondSubType.Length;
                req.cond.itemSubTypes = new uint[subTypeNumber];
                for (var i = 0; i < subTypeNumber; i++)
                {
                    req.cond.itemSubTypes[i] = (uint)secondSubType[i] * SubTypeCoefficient + (uint)thirdType[i];
                }

                #region 特殊类别
                //只在第三个层级中可能存在：针对宝珠和强化券类型
                if (thirdLayerMenuTabDataModel.AuctionNewFrameTable.SpecialParametersType == 1)
                {
                    //宝珠类型，参数放置配置
                    req.cond.quality = (byte) thirdLayerMenuTabDataModel.AuctionNewFrameTable.SpecialParameters;
                }
                else if (thirdLayerMenuTabDataModel.AuctionNewFrameTable.SpecialParametersType == 2)
                {
                    //强化券类型, 放在强化券等级字段
                    req.cond.couponStrengthToLev =
                        (byte) thirdLayerMenuTabDataModel.AuctionNewFrameTable.SpecialParameters;
                }
                #endregion
            }
            #endregion

            //添加过滤器相关
            if (firstFilterData != null)
            {
                SetAuctionNewNumReqFilterParam(req, firstFilterData);
            }

            if (secondFilterData != null)
            {
                SetAuctionNewNumReqFilterParam(req, secondFilterData);
            }

            if (thirdFilterData != null)
            {
                SetAuctionNewNumReqFilterParam(req, thirdFilterData);
            }

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        private List<uint> GetItemSubTypes(AuctionNewFrameTable auctionNewFrameTable)
        {
            var subTypeNumber = auctionNewFrameTable.SubType.Length;
            var thirdTypeNumber = auctionNewFrameTable.ThirdType.Length;
            var minNumber = subTypeNumber >= thirdTypeNumber ? thirdTypeNumber : subTypeNumber;

            var itemSubTypes = new List<uint>();
            for (var i = 0; i < minNumber; i++)
            {
                var curSubType = auctionNewFrameTable.SubType[i];
                var curThirdType = auctionNewFrameTable.ThirdType[i];
                itemSubTypes.Add((uint)curSubType * SubTypeCoefficient + (uint)curThirdType);
            }

            return itemSubTypes;

        }


        //在售使用过滤器
        private void SetAuctionNewNumReqFilterParam(WorldAuctionItemNumReq req, AuctionNewFilterData filterData)
        {
            if(req == null || req.cond == null)
                return;

            if (filterData.AuctionNewFilterTable.Parameter == null
                || (filterData.AuctionNewFilterTable.Parameter.Count == 1
                    && filterData.AuctionNewFilterTable.Parameter[0] == 0))
                return;

            if (filterData.FilterItemType == AuctionNewFrameTable.eFilterItemType.FIT_LEVEL)
            {
                if (filterData.AuctionNewFilterTable.Parameter != null &&
                    filterData.AuctionNewFilterTable.Parameter.Count == 2)
                {
                    var preLevel = filterData.AuctionNewFilterTable.Parameter[0];
                    var nextLevel = filterData.AuctionNewFilterTable.Parameter[1];
                    if (preLevel > nextLevel)
                    {
                        req.cond.minLevel = (byte)nextLevel;
                        req.cond.maxLevel = (byte)preLevel;
                    }
                    else
                    {
                        req.cond.minLevel = (byte) preLevel;
                        req.cond.maxLevel = (byte) nextLevel;
                    }
                }
            }
            else if (filterData.FilterItemType == AuctionNewFrameTable.eFilterItemType.FIT_QUALITY)
            {

                if (filterData.AuctionNewFilterTable.Parameter.Count > 1)
                    req.cond.quality = (byte) 0;
                else
                {
                    req.cond.quality = (byte) filterData.AuctionNewFilterTable.Parameter[0];
                }
            }
            else if (filterData.FilterItemType == AuctionNewFrameTable.eFilterItemType.FIT_SUCCEEDRAT)
            {
                if(filterData.AuctionNewFilterTable.Parameter.Count <= 1)
                    return;

                var preRate = filterData.AuctionNewFilterTable.Parameter[0];
                var nextRate = filterData.AuctionNewFilterTable.Parameter[1];

                //成功率使用的是等级类型
                if (preRate > nextRate)
                {
                    req.cond.minLevel = (byte)nextRate;
                    req.cond.maxLevel = (byte) preRate;
                }
                else
                {
                    req.cond.minLevel = (byte) preRate;
                    req.cond.maxLevel = (byte) nextRate;
                }
            }
            else if (filterData.FilterItemType == AuctionNewFrameTable.eFilterItemType.FIT_JOB)
            {
                ////职业筛选
                var jobNumber = filterData.AuctionNewFilterTable.Parameter.Count;
                req.cond.occus = new byte[jobNumber];

                for (var i = 0; i < jobNumber; i++)
                {
                    req.cond.occus[i] = (byte) filterData.AuctionNewFilterTable.Parameter[i];
                }
            }
        }

        private void OnReceiveAuctionItemNumRes(MsgDATA msgData)
        {
            WorldAuctionItemNumRes ret = new WorldAuctionItemNumRes();
            ret.decode(msgData.bytes);
            var curGoodState = (int) ret.goodState;

            if (curGoodState <= 0 || curGoodState > 2)
            {
                Logger.LogErrorFormat("AuctionItemNumRes goodState is Error");
                return;
            }

            if (_auctionNewItemNumResDictionary == null)
                _auctionNewItemNumResDictionary = new Dictionary<int, List<AuctionItemBaseInfo>>();

            var auctionNewItemResList = new List<AuctionItemBaseInfo>();
            if (_auctionNewItemNumResDictionary.ContainsKey(curGoodState) == false)
                _auctionNewItemNumResDictionary.Add(curGoodState, auctionNewItemResList);
            else
            {
                if (_auctionNewItemNumResDictionary.TryGetValue(curGoodState, out auctionNewItemResList) == false)
                {
                    auctionNewItemResList = new List<AuctionItemBaseInfo>();
                    _auctionNewItemNumResDictionary[curGoodState] = auctionNewItemResList;
                }
            }

            auctionNewItemResList.Clear();

            var itemNumResListCount = ret.items.Length;
            if (itemNumResListCount > 0)
            {
                for (var i = 0; i < itemNumResListCount; i++)
                {
                    var itemTableId = (int)ret.items[i].itemTypeId;
                    var curItemTable = TableManager.GetInstance().GetTableItem<ItemTable>(itemTableId);
                    if (curItemTable == null)
                        continue;

                    auctionNewItemResList.Add(ret.items[i]);
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAuctionNewReceiveItemNumResSucceed,
                curGoodState);
        }

        public List<AuctionItemBaseInfo> GetAuctionNewItemNumResList(int goodState)
        {
            if (_auctionNewItemNumResDictionary == null || _auctionNewItemNumResDictionary.Count <= 0)
            {
                return null;
            }

            List<AuctionItemBaseInfo> auctionItemNumResList = null;
            if (_auctionNewItemNumResDictionary.TryGetValue(goodState, out auctionItemNumResList) == true)
                return auctionItemNumResList;

            return null;
        }

        #endregion

        #region AuctionNewItemDetailData

        //获得物品的详情
        public void SendAuctionNewItemDetailListReq(int itemId,
            AuctionNewMainTabType mainTabType,
            int curPage = 1,
            int sortType = 1,
            int noticeTab = 0,
            int minStrengthenLevel = 0,
            int maxStrengthenLevel = 0)
        {
            var req = new WorldAuctionListReq();
            req.cond.type = (byte)AuctionType.Item; // 基本类型
            //ItemId 和类型
            req.cond.itemTypeID = (uint)itemId;
            var auctionGoodState = AuctionNewUtility.ConvertToAuctionGoodState(mainTabType);
            req.cond.goodState = (byte) auctionGoodState;

            req.cond.page = (ushort)curPage;
            req.cond.itemNumPerPage = (byte)PageNumber;
            req.cond.sortType = (byte)sortType;
            req.cond.attent = (byte)noticeTab;

            req.cond.minStrength = (byte) minStrengthenLevel;
            req.cond.maxStrength = (byte) maxStrengthenLevel;

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        //收到物品详情的返回；关注页签会把关注的所有东西都拉去下来，（可以关注的东西一共不超过12个）
        private void OnReceiveAuctionItemDetailListRes(MsgDATA msgData)
        {
            if(msgData == null)
                return;

            var ret = new WorldAuctionListQueryRet();
            ret.decode(msgData.bytes);

            var curGoodState = (int) ret.goodState;

            if (curGoodState <= 0 || curGoodState > 2)
            {
                Logger.LogErrorFormat("AuctionItemListRes goodState is Error and goodState is {0}",
                    curGoodState);
                return;
            }

            if (_auctionNewItemDetailResDictionary == null)
                _auctionNewItemDetailResDictionary = new Dictionary<int, AuctionNewItemDetailData>();

            var auctionNewItemDetailData = new AuctionNewItemDetailData();
            auctionNewItemDetailData.Type = ret.type;
            auctionNewItemDetailData.CurPage = (int)ret.curPage;
            auctionNewItemDetailData.MaxPage = (int)ret.maxPage;
            auctionNewItemDetailData.NoticeType = (int) ret.attent;
            auctionNewItemDetailData.ItemDetailDataList = new List<AuctionBaseInfo>();
            //非关注页签，返回规定的数量，最多为PageNumber；关注页签，返回关注的所有数量，最多为12个
            if (ret.data != null && ret.data.Length > 0)
            {
                for (var i = 0; i < ret.data.Length; i++)
                {
                    auctionNewItemDetailData.ItemDetailDataList.Add(ret.data[i]);
                }
            }

            if (_auctionNewItemDetailResDictionary.ContainsKey(curGoodState) == true)
                _auctionNewItemDetailResDictionary[curGoodState] = auctionNewItemDetailData;
            else
            {
                _auctionNewItemDetailResDictionary.Add(curGoodState, auctionNewItemDetailData);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAuctionNewReceiveItemDetailDataResSucceed,
                curGoodState);
        }

        //根据goodState获得对应的在售信息
        public AuctionNewItemDetailData GetAuctionNewItemDetailData(int goodState)
        {
            if (_auctionNewItemDetailResDictionary == null || _auctionNewItemDetailResDictionary.Count <= 0)
                return null;

            if (_auctionNewItemDetailResDictionary.ContainsKey(goodState) == false)
                return null;

            AuctionNewItemDetailData itemDetailData = null;
            if (_auctionNewItemDetailResDictionary.TryGetValue(goodState, out itemDetailData) == true)
                return itemDetailData;

            return null;
        }
        #endregion

        #region AuctionNewForSellData

        //当前道具是否可以展示在某种类型的在售架子上
        public bool IsPackageItemCanInForSaleList(ulong guid, ActionNewSellTabType sellTabType)
        {
            if (IsPackageItemCanTradeByGuid(guid) == false)
                return false;

            var itemData = ItemDataManager.GetInstance().GetItem(guid);
            if (itemData == null)
                return false;

            if (sellTabType == ActionNewSellTabType.AuctionNewSellEquipType)
            {
                if (AuctionNewUtility.IsEquipItems(itemData) == true)
                    return true;
            }
            else if (sellTabType == ActionNewSellTabType.AuctionNewSellMaterialType)
            {
                if (AuctionNewUtility.IsEquipItems(itemData) == false)
                    return true;
            }

            return false;
        }

        //判断背包中的某个道具是否可以上架
        public bool IsPackageItemCanTradeByGuid(ulong guid)
        {
            var itemData = ItemDataManager.GetInstance().GetItem(guid);
            if (itemData == null)
                return false;

            //不可以交易
            if (!ItemDataManager.GetInstance().TradeItemStateFliter(itemData))
                return false;

            //类型过滤
            UpdatePackageTypeList();
            if (!ItemDataManager.GetInstance().TradeItemTypeFliter(_packageTypeList, itemData.PackageType))
                return false;

            if (itemData.isInSidePack)
                return false;

            //镶嵌铭文
            if (itemData.CheckEquipIsMosaicInscription() == true)
                return false;

            //block 锁住状态
            if (itemData.bLocked == true)
                return false;

            return true;

        }

        //设置背包类型
        private void UpdatePackageTypeList()
        {
            if (_packageTypeList == null)
                _packageTypeList = new List<EPackageType>();

            if (_packageTypeList.Count == 0)
            {
                _packageTypeList.Add(EPackageType.Equip);
                _packageTypeList.Add(EPackageType.Material);
                _packageTypeList.Add(EPackageType.Consumable);
                _packageTypeList.Add(EPackageType.Task);
                _packageTypeList.Add(EPackageType.Fashion);
                _packageTypeList.Add(EPackageType.Title);
                _packageTypeList.Add(EPackageType.Bxy);
                _packageTypeList.Add(EPackageType.Sinan);
            }
        }

        //更新背包中所有可以交易的商品
        public void UpdatePackageItemUid()
        {
            _packageSellItemUidList.Clear();

            UpdatePackageTypeList();

            var allPackageItems = ItemDataManager.GetInstance().GetAllPackageItems();
            if (allPackageItems == null)
            {
                return;
            }

            var allItems = allPackageItems.GetEnumerator();

            while (allItems.MoveNext())
            {
                var itemGuid = allItems.Current.Key;

                var kItem = ItemDataManager.GetInstance().GetItem(itemGuid);
                if (kItem == null)
                {
                    continue;
                }

                if (!ItemDataManager.GetInstance().TradeItemTypeFliter(_packageTypeList, kItem.PackageType))
                {
                    continue;
                }

                if (!ItemDataManager.GetInstance().TradeItemStateFliter(kItem))
                {
                    continue;
                }

                _packageSellItemUidList.Add(itemGuid);
            }
        }

        //获得某种具体类型的所有可以上架的商品
        public List<AuctionSellItemData> GetAuctionSellItemDataByType(ActionNewSellTabType sellTabType)
        {
            if (_packageSellItemDataList == null)
                _packageSellItemDataList = new List<AuctionSellItemData>();

            _packageSellItemDataList.Clear();

            UpdatePackageItemUid();

            for (var i = 0; i < _packageSellItemUidList.Count; i++)
            {
                var curItemData = ItemDataManager.GetInstance().GetItem(_packageSellItemUidList[i]);
                if (curItemData == null)
                {
                    continue;
                }

                //副武器
                if (curItemData.isInSidePack)
                    continue;

                //镶嵌铭文的装备，不显示
                if(curItemData.CheckEquipIsMosaicInscription() == true)
                    continue;

                //装备存在于未启用的装备方案中
                if(curItemData.IsItemInUnUsedEquipPlan == true)
                    continue;
                
                AuctionSellItemData sellItemData = null;

                if (sellTabType == ActionNewSellTabType.AuctionNewSellEquipType)
                {
                    if (AuctionNewUtility.IsEquipItems(curItemData) == true)
                    {
                        sellItemData = new AuctionSellItemData(curItemData.GUID, 
                            (int)curItemData.Quality,
                            curItemData.LevelLimit,
                            curItemData.IsTreasure);
                    }
                }
                else
                {
                    if (AuctionNewUtility.IsEquipItems(curItemData) == false)
                    {
                        sellItemData = new AuctionSellItemData(curItemData.GUID, 
                            (int)curItemData.Quality,
                            curItemData.LevelLimit,
                            curItemData.IsTreasure);
                    }
                }

                if (sellItemData == null)
                {
                    continue;
                }

                if (!curItemData.bLocked)
                {
                    _packageSellItemDataList.Add(sellItemData);
                }
            }

            //首先按照珍品排序，其次按照品质排序, 按照两者均降序排序
            _packageSellItemDataList.Sort((x, y) =>
            {
                var a = -x.IsTreasure.CompareTo(y.IsTreasure);

                if (a == 0)
                    a = -x.Quality.CompareTo(y.Quality);

                return a;
            });

            return _packageSellItemDataList;
        }
        #endregion

        #region AuctionNewNotifyRefresh
        //收到刷新信息，对他人在售的商品进行刷新
        //商品刷新
        private void OnWorldAuctionNotifyRefreshRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            var ret = new WorldAuctionNotifyRefresh();
            ret.decode(msgData.bytes);

            if (ret.type != (byte)AuctionType.Item)
                return;

            if (ret.reason == (byte)AuctionRefreshReason.SRR_BUY)
            {
                //正常的购买刷新， 发送UI事件，用于刷新
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAuctionNewNotifyRefreshToRequestDetailItems, ret.auctGuid);
            }
            else if (ret.reason == (byte)AuctionRefreshReason.SRR_RUSY_BUY)
            {
                //抢购结束进行刷新，发送UI事件，进行刷新操作
                UIEventSystem.GetInstance()
                    .SendUIEvent(EUIEventID.OnAuctionNewNotifyRefreshToRequestDetailItems, ret.auctGuid);
            }
            else if (ret.reason == (byte)AuctionRefreshReason.SRR_SELL
                     || ret.reason == (byte)AuctionRefreshReason.SRR_CANCEL
                     || ret.reason == (byte)AuctionRefreshReason.SRR_SYS_RECOVERY)
            {
                //出售，取消，扫货产生的刷新，应该重新请求架子的数据

                //发送协议，进行上架数据的更新
                //UpdatePackageItemUid();
                SendSelfAuctionListRequest();
            }
        }
        #endregion

        #region ShelfInformation

        //是否可以购买框架
        public bool IsAuctionNewCanBuyShelfField()
        {
            if (PlayerBaseData.GetInstance().AddAuctionFieldsNum >= MaxAddShelfNum)
                return false;

            return true;
        }

        //请求购买栏位
        public void SendBuyShelfRequest()
        {
            NetManager netMgr = NetManager.Instance();

            SceneAuctionBuyBoothReq req = new SceneAuctionBuyBoothReq();

            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        //购买栏位返回
        private void OnReceiveBuyShelfRes(MsgDATA msgData)
        {
            if(msgData == null)
                return;

            SceneAuctionBuyBoothRes res = new SceneAuctionBuyBoothRes();
            res.decode(msgData.bytes);

            if (res.result != 0)
            {
                SystemNotifyManager.SystemNotify((int) res.result);
                return;
            }

            PlayerBaseData.GetInstance().AddAuctionFieldsNum = (int) res.boothNum;
            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("auction_new_onSale_buy_shelf_succeed"));

            //将最后一个购买的DataModel 职位 empty
            if (_onShelfDataModelList != null && _onShelfDataModelList.Count > 0)
            {
                var lastSelfDataModel = _onShelfDataModelList[_onShelfDataModelList.Count - 1];
                if (lastSelfDataModel != null && lastSelfDataModel.onShelfState == AuctionNewOnShelfState.BuyField)
                {
                    lastSelfDataModel.onShelfState = AuctionNewOnShelfState.Empty;
                    lastSelfDataModel.boothTableData = null;
                }
            }

            //添加一个可以购买的栏位
            if (PlayerBaseData.GetInstance().AddAuctionFieldsNum < MaxAddShelfNum)
            {
                AddOneBuyShelfDataModel(PlayerBaseData.GetInstance().AddAuctionFieldsNum + 1);
            }
            
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAuctionNewBuyShelfResSucceed);
            //发送UI时间进行更新栏目
        }

        //商品下架请求
        public void SendDownShelfItemRequest(ulong guid)
        {
            NetManager netMgr = NetManager.Instance();

            WorldAuctionCancel req = new WorldAuctionCancel();
            req.id = guid;
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }
        
        //请求自己的上架信息
        public void SendSelfAuctionListRequest()
        {
            var req = new WorldAuctionSelfListReq();
            req.type = (byte)AuctionType.Item;

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        //上架商品的返回
        private void OnReceiveWorldAuctionSelfListRes(MsgDATA msgData)
        {

            //收到自己的数据之后，进行操作
            var ret = new WorldAuctionSelfListRes();
            ret.decode(msgData.bytes);

            if (ret.type != (byte) AuctionType.Item)
            {
                return;
            }

            _onShelfDataModelList.Clear();
            //添加具体的上架物品
            if (ret.data != null && ret.data.Length > 0)
            {
                for (var i = 0; i < ret.data.Length; i++)
                {
                    var curData = ret.data[i];
                    if (curData != null)
                    {
                        var auctionNewOnShelfDataModel = new AuctionNewOnShelfDataModel();
                        auctionNewOnShelfDataModel.onShelfState = AuctionNewOnShelfState.OwnerItem;
                        auctionNewOnShelfDataModel.auctionBaseInfo = curData;
                        _onShelfDataModelList.Add(auctionNewOnShelfDataModel);
                    }
                }
            }

            OnShelfItemNumber = _onShelfDataModelList.Count;

            //添加额外的按钮
            AddExtraOnShelfDataModelList();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAuctionNewReceiveSelfListResSucceed);
        }

        //添加空栏和可以购买的栏
        private void AddExtraOnShelfDataModelList()
        {
            //首先添加空栏位
            var totalShelfNumber = PlayerBaseData.GetInstance().AddAuctionFieldsNum + BaseShelfNum;
            var onShelfItemNumber = _onShelfDataModelList.Count;
            if (onShelfItemNumber < totalShelfNumber)
            {
                var addEmptyShelfNumber = totalShelfNumber - onShelfItemNumber;
                for (var i = 0; i < addEmptyShelfNumber; i++)
                {
                    var auctionNewOnShelfDataModel = new AuctionNewOnShelfDataModel();
                    auctionNewOnShelfDataModel.onShelfState = AuctionNewOnShelfState.Empty;
                    _onShelfDataModelList.Add(auctionNewOnShelfDataModel);
                }
            }

            //没有完全解锁
            //在最后添加一个可以购买的位置栏
            if (PlayerBaseData.GetInstance().AddAuctionFieldsNum < MaxAddShelfNum)
            {
                AddOneBuyShelfDataModel(PlayerBaseData.GetInstance().AddAuctionFieldsNum + 1);
            }
        }

        //添加一个新的栏位
        private void AddOneBuyShelfDataModel(int boothTableId)
        {
            var buyShelfId = boothTableId;
            var auctionBoothTable = TableManager.GetInstance().GetTableItem<AuctionBoothTable>(buyShelfId);

            if(auctionBoothTable == null)
                return;

            var auctionNewOnShelfDataModel = new AuctionNewOnShelfDataModel();
            auctionNewOnShelfDataModel.onShelfState = AuctionNewOnShelfState.BuyField;
            auctionNewOnShelfDataModel.boothTableData = auctionBoothTable;
            _onShelfDataModelList.Add(auctionNewOnShelfDataModel);
        }

        public List<AuctionNewOnShelfDataModel> GetOnShelfDataModelList()
        {
            return _onShelfDataModelList;
        }
        #endregion

        #region SellBuyRecord

        public void SendAuctionNewTreasureTransactionReq()
        {
            //交易记录列表进行重置
            ResetTreasureTransactionRecordList();

            var netMgr = NetManager.Instance();
            var req = new WorldAuctionGetTreasTransactionReq();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        private void OnReceiveAuctionNewTreasureTransactionRes(MsgDATA msgData)
        {
            if(msgData == null)
                return;

            var res = new WorldAuctionGetTreasTransactionRes();
            res.decode(msgData.bytes);

            if(res.sales != null)
                _treasureItemSaleRecordList.AddRange(res.sales);

            if (res.buys != null)
                _treasureItemBuyRecordList.AddRange(res.buys);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAuctionNewGetTreasureTransactionRecordSucceed);
        }

        public List<AuctionTransaction> GetTreasureItemSaleRecordList()
        {
            return _treasureItemSaleRecordList;
        }

        public List<AuctionTransaction> GetTreasureItemBuyRecordList()
        {
            return _treasureItemBuyRecordList;
        }

        public void ResetTreasureTransactionRecordList()
        {
            _treasureItemSaleRecordList.Clear();
            _treasureItemBuyRecordList.Clear();
        }


        #endregion

        #region SellItem
        //解封物品
        public void OnClickOnPacking(ItemData itemData)
        {
            SmithShopNewLinkData linkData = new SmithShopNewLinkData();
            linkData.itemData = itemData;

            ClientSystemManager.GetInstance().CloseFrame<EquipSealFrame>(null, true);
            ClientSystemManager.GetInstance().OpenFrame<EquipSealFrame>(FrameLayer.Middle, linkData);
        }

        //上架道具，出售物品
        public void SendOnShelfReq(ItemData itemData,
            int totalPrice,
            int itemNumber,
            byte isAgain = 0,
            ulong auctionGuid = 0)
        {

            WorldAuctionRequest req = new WorldAuctionRequest();

            req.id = itemData.GUID;
            req.typeId = (uint)itemData.TableID;
            req.type = (byte)AuctionType.Item;
            req.price = (uint)totalPrice;
            req.duration = (byte)AuctionSellDuration.Hour_24;
            req.num = (uint)itemNumber;
            req.strength = (byte)itemData.StrengthenLevel;
            if (itemData.SubType == (int)ItemTable.eSubType.Bead)
            {
                req.beadbuffId = (uint)itemData.BeadAdditiveAttributeBuffID;
            }
            else
            {
                req.beadbuffId = 0;
            }

            //红字装备的增幅类型
            req.enhanceType = (byte) itemData.GrowthAttrType;

            //重新上架，赋值字段
            if (isAgain == 1)
            {
                req.isAgain = isAgain;
                req.auctionGuid = auctionGuid;
            }

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);

            //保存上架道具的数据
            AuctionNewDataManager.GetInstance().SetPreviewOnShelfItemData(itemData.TableID,
                itemData.StrengthenLevel,
                totalPrice);

        }

        #endregion

        #region AuctionNewTempTabData

        //拍卖行系统的临时变量，用于保存当前点击的第一二层的TabId

        public void SetLastTimeUserDataMainTabType(AuctionNewMainTabType mainTabType)
        {
            InitLastTimeAuctionNewUserData();
            _lastTimeAuctionNewUserData.MainTabType = mainTabType;
        }

        public void SetLastTimeUserDataFirstLayerTabId(int firstLayerId)
        {
            InitLastTimeAuctionNewUserData();
            _lastTimeAuctionNewUserData.FirstLayerTabId = firstLayerId;
        }

        public void SetLastTimeUserDataSecondLayerTabId(int secondLayerTabId)
        {
            InitLastTimeAuctionNewUserData();
            _lastTimeAuctionNewUserData.SecondLayerTabId = secondLayerTabId;
        }

        private void InitLastTimeAuctionNewUserData()
        {
            if (_lastTimeAuctionNewUserData == null)
                _lastTimeAuctionNewUserData = new AuctionNewUserData();
        }

        public AuctionNewUserData GetLastTimeUserData()
        {
            return _lastTimeAuctionNewUserData;
        }

        public void ResetLastTimeUserData()
        {
            _lastTimeAuctionNewUserData = null;
        }

        #endregion

        #region ItemDataTipFrame

        //拍卖行中显示ItemData的TipFrame， 有的Item需要向服务器请求数据
        public void OnShowItemDetailTipFrame(ItemData itemData, ulong guid = 0)
        {
            if (guid == 0)
            {
                ShowAuctionNewSystemItemTip(itemData);
            }
            else
            {
                ShowItemDataDetailInfoTipFrame(guid);
            }
        }

        //根据Guid获得ItemData 的详细信息，之后显示ItemTipFrame
        public void ShowItemDataDetailInfoTipFrame(UInt64 guid)
        {
            var itemData = GetItemDataDetailInfoByGuid(guid);

            if (itemData == null)
            {
                SendWorldAuctionQueryItemReq(guid);
            }
            else
            {
                ShowAuctionNewSystemItemTip(itemData);
            }
        }

        //请求道具的详情信息
        private void SendWorldAuctionQueryItemReq(UInt64 guid)
        {
            var req = new WorldAuctionQueryItemReq();
            req.id = guid;

            NetManager netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        //接收到物品的详情信息
        private void OnReceiveWorldAuctionQueryItemRet(MsgDATA msgData)
        {
            int pos = 0;

            UInt64 guid = 0;
            BaseDLL.decode_uint64(msgData.bytes, ref pos, ref guid);

            UInt32 typeId = 0;
            BaseDLL.decode_uint32(msgData.bytes, ref pos, ref typeId);

            Item item = new Item();
            item.uid = guid;
            item.dataid = typeId;
            StreamObjectDecoder<Item>.DecodePropertys(ref item, msgData.bytes, ref pos, msgData.bytes.Length);


            var itemData = ItemDataManager.GetInstance().CreateItemDataFromNet(item);
            if (itemData == null)
            {
                Logger.LogError("itemData is null in [OnAuctionItemDetailRet] in AuctionItemFrame");
                return;
            }

            AddItemDataDetailInfo(guid, itemData);

            ShowAuctionNewSystemItemTip(itemData);
        }

        private void ShowAuctionNewSystemItemTip(ItemData itemData)
        {
            if (itemData != null)
            {
                //设置上在ItemTipFrame上显示珍品图标的标示
                if (itemData.IsTreasure == true)
                    itemData.IsShowTreasureFlagInTipFrame = true;
                else
                {
                    itemData.IsShowTreasureFlagInTipFrame = false;
                }
            }
            ItemTipManager.GetInstance().ShowTip(itemData);
        }

        public void AddItemDataDetailInfo(UInt64 guid, ItemData itemData)
        {
            if(guid == 0)
                return;

            if (_itemDataDetailDictionary == null)
                _itemDataDetailDictionary = new Dictionary<UInt64, ItemData>();

            if (_itemDataDetailDictionary.ContainsKey(guid) == true)
                _itemDataDetailDictionary[guid] = itemData;
            else
            {
                _itemDataDetailDictionary.Add(guid, itemData);
            }
        }

        public ItemData GetItemDataDetailInfoByGuid(UInt64 guid)
        {
            if (guid == 0)
                return null;

            if (_itemDataDetailDictionary == null)
                return null;

            if (_itemDataDetailDictionary.ContainsKey(guid) == false)
                return null;

            ItemData itemData = null;
            if (_itemDataDetailDictionary.TryGetValue(guid, out itemData) == true)
                return itemData;

            return null;
        }
        
        #endregion

        #region OnShelfTipFrame

        //珍品上架提示
        public bool IsShowOnShelfTipOfTreasureItem(Action onCancel, Action onOk)
        {
            //不用显示
            if (IsNotShowOnShelfTipOfTreasureItem == true)
                return false;

            //没有Item 或者Item 中的Desc不存在
            var commonTipsDescItem =
                TableManager.GetInstance().GetTableItem<CommonTipsDesc>(OnShelfTipIdOnTreasureItem);
            if (commonTipsDescItem == null || string.IsNullOrEmpty(commonTipsDescItem.Descs) == true)
            {
                Logger.LogErrorFormat("commonTipsDesItem is null or desc is null and commonTipsDescId is {0}",
                    OnShelfTipIdOnTreasureItem);
                return false;
            }

            var convertDescLine = AuctionNewUtility.ConvertDescLine(commonTipsDescItem.Descs);
            var convertDescStrFinal = AuctionNewUtility.ConvertDescBlank(convertDescLine);

            //显示提示
            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = convertDescStrFinal,
                IsShowNotify = true,
                OnCommonMsgBoxToggleClick = OnUpdateOnShelfTipOfTreasureItem,
                LeftButtonText = TR.Value("common_data_cancel"),
                OnLeftButtonClickCallBack = onCancel,
                RightButtonText = TR.Value("common_data_sure"),
                OnRightButtonClickCallBack = onOk,
            };

            OpenAuctionNewMsgBoxOkCancelFrame(commonMsgBoxOkCancelParamData);
            return true;
        }

        private void OnUpdateOnShelfTipOfTreasureItem(bool value)
        {
            IsNotShowOnShelfTipOfTreasureItem = value;
        }

        //非珍品上架提示
        public bool IsShowOnShelfTipOfNormalItem(Action onCancel, Action onOk)
        {
            //不显示
            if (IsNotShowOnShelfTipOfNormalItem == true)
                return false;

            //没有Item 或者Item 中的Desc不存在
            var commonTipsDescItem =
                TableManager.GetInstance().GetTableItem<CommonTipsDesc>(OnShelfTipIdOnNormalItem);
            if (commonTipsDescItem == null || string.IsNullOrEmpty(commonTipsDescItem.Descs) == true)
            {
                Logger.LogErrorFormat("commonTipsDesItem is null or desc is null and commonTipsDescId is {0}",
                    OnShelfTipIdOnTreasureItem);
                return false;
            }

            var convertDescLine = AuctionNewUtility.ConvertDescLine(commonTipsDescItem.Descs);
            var convertDescStrFinal = AuctionNewUtility.ConvertDescBlank(convertDescLine);
            //显示提示
            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = convertDescStrFinal,
                IsShowNotify = true,
                OnCommonMsgBoxToggleClick = OnUpdateOnShelfTipOfNormalItem,
                LeftButtonText = TR.Value("common_data_cancel"),
                OnLeftButtonClickCallBack = onCancel,
                RightButtonText = TR.Value("common_data_sure"),
                OnRightButtonClickCallBack = onOk,
            };

            OpenAuctionNewMsgBoxOkCancelFrame(commonMsgBoxOkCancelParamData);

            return true;

        }

        private void OnUpdateOnShelfTipOfNormalItem(bool value)
        {
            IsNotShowOnShelfTipOfNormalItem = value;
        }

        //过期的珍品再次上架的提示
        public bool OpenTreasureItemOnShelfAgain(Action onCancel, Action onOk)
        {

            //没有Item 或者Item 中的Desc不存在
            var commonTipsDescItem =
                TableManager.GetInstance().GetTableItem<CommonTipsDesc>(ShelfAgainTreasureItem);
            if (commonTipsDescItem == null || string.IsNullOrEmpty(commonTipsDescItem.Descs) == true)
            {
                Logger.LogErrorFormat("commonTipsDesItem is null or desc is null and commonTipsDescId is {0}",
                    OnShelfTipIdOnTreasureItem);
                return false;
            }

            var convertDescLine = AuctionNewUtility.ConvertDescLine(commonTipsDescItem.Descs);
            var convertDescStrFinal = AuctionNewUtility.ConvertDescBlank(convertDescLine);
            //显示提示
            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = convertDescStrFinal,
                LeftButtonText = TR.Value("common_data_cancel"),
                OnLeftButtonClickCallBack = onCancel,
                RightButtonText = TR.Value("auction_new_on_shelf_again_ensure"),
                OnRightButtonClickCallBack = onOk,
            };

            OpenAuctionNewMsgBoxOkCancelFrame(commonMsgBoxOkCancelParamData);

            return true;

        }

        private void OpenAuctionNewMsgBoxOkCancelFrame(CommonMsgBoxOkCancelNewParamData paramData)
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<AuctionNewMsgBoxOkCancelFrame>() == true)
            {
                ClientSystemManager.GetInstance().CloseFrame<AuctionNewMsgBoxOkCancelFrame>();
            }

            ClientSystemManager.GetInstance().OpenFrame<AuctionNewMsgBoxOkCancelFrame>(FrameLayer.High, paramData);
            return;
        }

        #endregion

        #region OnShelfItemInfo

        //发送请求价格的数据
        public void SendWorldAuctionQueryOnShelfItemPriceReq(ItemData itemData)
        {

            if (itemData == null)
            {
                Logger.LogErrorFormat("SendWorldAuctionQueryOnShelfItemPriceReq and itemData is null");
                return;
            }

            WorldAuctionQueryItemPricesReq req = new WorldAuctionQueryItemPricesReq();
            req.type = 0;
            req.itemTypeId = (uint)itemData.TableID;
            req.strengthen = (uint)itemData.StrengthenLevel;
            req.enhanceType = (byte) itemData.GrowthAttrType;       //增幅类型

            if (itemData.SubType == (int)ItemTable.eSubType.Bead)
            {
                req.beadbuffid = (uint)itemData.BeadAdditiveAttributeBuffID;
            }

            _worldAuctionQueryItemPriceRes = null;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        private void OnReceiveWorldAuctionQueryItemPriceRes(MsgDATA msgData)
        {
            var ret = new WorldAuctionQueryItemPricesRes();
            ret.decode(msgData.bytes);

            if (ret.type != (byte) AuctionType.Item)
            {
                Logger.LogError("OnReceiveWorldAuctionQueryItemPriceRes type is not Item");
                return;
            }

            _worldAuctionQueryItemPriceRes = ret;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAuctionNewWorldQueryItemPriceResSucceed);
        }

        public WorldAuctionQueryItemPricesRes GetWorldAuctionQueryItemPriceRes()
        {
            return _worldAuctionQueryItemPriceRes;
        }

        //发送请求公示或者在售物品价格的列表
        public void SendWorldAuctionQueryItemPriceListReq(byte auctionState, ItemData itemData)
        {

            if (itemData == null)
            {
                Logger.LogError("SendWorldAuctionQueryItemPriceListReq and itemData is null");
                return;
            }

            WorldAuctionQueryItemPriceListReq req = new WorldAuctionQueryItemPriceListReq();
            req.type = 0;
            req.auctionState = auctionState;
            req.itemTypeId = (uint)itemData.TableID;
            req.strengthen = (uint)itemData.StrengthenLevel;
            req.enhanceType = (byte) itemData.GrowthAttrType;

            _worldAuctionQueryItemPriceListRes = null;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        private void OnReceiveWorldAuctionQueryItemPriceListRes(MsgDATA msgData)
        {
            var ret = new WorldAuctionQueryItemPriceListRes();
            ret.decode(msgData.bytes);

            _worldAuctionQueryItemPriceListRes = ret;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAuctionNewWorldQueryItemPriceListResSucceed);
        }

        public WorldAuctionQueryItemPriceListRes GetWorldAuctionQueryItemPriceListRes()
        {
            return _worldAuctionQueryItemPriceListRes;
        }

        //请求最近交易记录
        public void SendWorldAuctionQueryItemTransListReq(ItemData itemData)
        {
            if (itemData == null)
            {
                Logger.LogError("SendWorldAuctionQueryItemTransListReq and itemData is null");
                return;
            }

            WorldAuctionQueryItemTransListReq req = new WorldAuctionQueryItemTransListReq();
            req.itemTypeId = (uint)itemData.TableID;
            req.strengthen = (uint)itemData.StrengthenLevel;
            req.enhanceType = (byte)itemData.GrowthAttrType;
            req.beadBuffId = (uint) itemData.BeadAdditiveAttributeBuffID;

            _worldAuctionQueryItemTransListRes = null;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        //最近交易的返回
        private void OnReceiveWorldAuctionQueryItemTransListRes(MsgDATA msgData)
        {
            var ret = new WorldAuctionQueryItemTransListRes();
            ret.decode(msgData.bytes);

            _worldAuctionQueryItemTransListRes = ret;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAuctionNewWorldQueryItemTransListResSucceed);
        }

        //最近交易的数据
        public WorldAuctionQueryItemTransListRes GetWorldAuctionQueryItemTransListRes()
        {
            return _worldAuctionQueryItemTransListRes;
        }

        public void ResetAuctionNewOnShelfData()
        {
            _worldAuctionQueryItemTransListRes = null;
            _worldAuctionQueryItemPriceListRes = null;
            _worldAuctionQueryItemPriceRes = null;
        }


        #endregion

        #region BuyNoticeTab
        //接受同步的我要关注的ID列表
        private void OnReceiveWorldAuctionSyncPubPageIds(MsgDATA msgData)
        {
            var ret = new WorldAuctionSyncPubPageIds();
            ret.decode(msgData.bytes);
            _worldAuctionSyncPubPageIds = ret;
        }

        //拍卖行的页签是否同步
        public bool IsNoticeLayerIdValid(int layerId)
        {
            //没有同步，或者同步的数组没有元素，则使用表中默认的配置
            if (_worldAuctionSyncPubPageIds == null || _worldAuctionSyncPubPageIds.pageIds == null
                                                    || _worldAuctionSyncPubPageIds.pageIds.Length <= 0)
                return true;

            for (var i = 0; i < _worldAuctionSyncPubPageIds.pageIds.Length; i++)
            {
                if ((int) _worldAuctionSyncPubPageIds.pageIds[i] == layerId)
                    return true;
            }

            return false;
        }

        //非在售的物品是否被踢出(不显示), true剔除，false不剔除
        public bool IsNotOnSaleItemNeedBeDeleted(int itemId)
        {
            if (_worldAuctionSyncPubPageIds == null || _worldAuctionSyncPubPageIds.shieldItemList == null
                                                    || _worldAuctionSyncPubPageIds.shieldItemList.Length <= 0)
                return false;

            for (var i = 0; i < _worldAuctionSyncPubPageIds.shieldItemList.Length; i++)
            {
                var needDeletedItemId = (int) _worldAuctionSyncPubPageIds.shieldItemList[i];
                //需要被剔除
                if (itemId == needDeletedItemId)
                    return true;
            }

            return false;
        }

        #endregion

        #region AuctionNewItemIdList
        //得到AuctionNewItemFrame表中中ID对应的所有ItemId,并根据ID进行缓存
        public List<int> GetAuctionNewItemIdList(AuctionNewMenuTabDataModel firstLayerMenuTabDataModel,
            AuctionNewMenuTabDataModel secondLayerMenuTabDataModel,
            AuctionNewMenuTabDataModel thirdLayerMenuTabDataModel)
        {
            if (firstLayerMenuTabDataModel == null || firstLayerMenuTabDataModel.AuctionNewFrameTable == null)
            {
                Logger.LogErrorFormat("FirstLayerMenuTabDataModel is Error");
                return null;
            }

            if (secondLayerMenuTabDataModel == null || secondLayerMenuTabDataModel.AuctionNewFrameTable == null)
            {
                Logger.LogErrorFormat("SecondLayerMenuTabDataModel is Error");
                return null;
            }

            var auctionNewFrameTableId = 0;
            List<int> auctionNewItemIdList;

            //第三层级不存在，使用第二层级的ID
            if (thirdLayerMenuTabDataModel == null || thirdLayerMenuTabDataModel.AuctionNewFrameTable == null)
            {
                auctionNewFrameTableId = secondLayerMenuTabDataModel.AuctionNewFrameTable.ID;                
            }
            else
            {
                //使用第三层级的ID
                auctionNewFrameTableId = thirdLayerMenuTabDataModel.AuctionNewFrameTable.ID;
            }

            //存在ItemId的缓存，直接返回
            if (_auctionNewItemIdDictionary.TryGetValue(auctionNewFrameTableId, out auctionNewItemIdList) ==
                true)
            {
                return auctionNewItemIdList;
            }

            //缓存不存在，获得相应的ItemIdList列表，并进行缓存
            auctionNewItemIdList = AuctionNewUtility.GetItemIdList(firstLayerMenuTabDataModel,
                secondLayerMenuTabDataModel,
                thirdLayerMenuTabDataModel);

            if (auctionNewItemIdList == null)
                auctionNewItemIdList = new List<int>();

            if (_auctionNewItemIdDictionary.ContainsKey(auctionNewFrameTableId) == false)
                _auctionNewItemIdDictionary.Add(auctionNewFrameTableId, auctionNewItemIdList);
            else
                _auctionNewItemIdDictionary[auctionNewFrameTableId] = auctionNewItemIdList;

            return auctionNewItemIdList;
        }

        public void ResetAuctionNewItemIdDictionary()
        {
            if (_auctionNewItemIdDictionary == null || _auctionNewItemIdDictionary.Count <= 0)
                return;

            var itemIdIter = _auctionNewItemIdDictionary.GetEnumerator();
            while (itemIdIter.MoveNext())
            {
                var itemIdList = itemIdIter.Current.Value;
                if (itemIdList != null)
                    itemIdList.Clear();
            }
            _auctionNewItemIdDictionary.Clear();
        }
        #endregion

        #region ItemNoticeInfo

        public void OnSendWorldActionNoticeReq(ulong itemGuid)
        {

            WorldAuctionAttentReq req = new WorldAuctionAttentReq();
            req.autionGuid = itemGuid;
            
            NetManager netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        private void OnReceiveWorldActionNoticeRes(MsgDATA msgData)
        {

            if (msgData == null)
            {
                Logger.LogErrorFormat("OnReceiveWorldActionNoticeRes MsgData is null");
                return;
            }

            var res = new WorldActionAttentRes();
            res.decode(msgData.bytes);

            if (res.code != 0)
            {
                SystemNotifyManager.SystemNotify((int)res.code);
                return;
            }

            if (res.aution == null)
            {
                Logger.LogErrorFormat("OnReceiveWorldActionNoticeRes auction is null");
                return;
            }

            //对AuctionNewItemDetailResDictionary 进行更新，更新对应道具的关注状态和关注数量
            if (_auctionNewItemDetailResDictionary == null)
            {
                Logger.LogErrorFormat("ItemDetailDictionary is null");
                return;
            }

            var itemDetailResIter = _auctionNewItemDetailResDictionary.GetEnumerator();
            while (itemDetailResIter.MoveNext())
            {
                var auctionNewItemDetailData = (AuctionNewItemDetailData)itemDetailResIter.Current.Value ;
                if (auctionNewItemDetailData != null
                    && auctionNewItemDetailData.ItemDetailDataList != null
                    && auctionNewItemDetailData.ItemDetailDataList.Count > 0)
                {
                    //查找缓存的Item
                    for (var i = 0; i < auctionNewItemDetailData.ItemDetailDataList.Count; i++)
                    {
                        var curAuctionBaseItemInfo = auctionNewItemDetailData.ItemDetailDataList[i];
                        //根据guid找到相关的数据
                        if (curAuctionBaseItemInfo != null && curAuctionBaseItemInfo.guid == res.aution.guid)
                        {
                            //缓存关注状态和关注的数量
                            curAuctionBaseItemInfo.attent = res.aution.attent;
                            curAuctionBaseItemInfo.attentNum = res.aution.attentNum;
                            //发送UI事件，并返回
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAuctionNewReceiveNoticeReqSucceed,
                                res.aution.guid);
                        }
                    }
                }
            }

        }

        #endregion

        #region PreviewOnShelfItemInfo

        //重置上架的数据
        public void ResetPreviewOnShelfItemData()
        {
            if(_previewOnShelfItemData != null)
                _previewOnShelfItemData.ResetItemData();
        }
    
        //设置上架数据
        public void SetPreviewOnShelfItemData(int itemId, int strengthLevel, int onShelfPrice)
        {
            if (_previewOnShelfItemData == null)
                _previewOnShelfItemData = new AuctionNewPreviewOnShelfItemData();

            _previewOnShelfItemData.ItemId = itemId;
            _previewOnShelfItemData.StrengthLevel = strengthLevel;
            _previewOnShelfItemData.OnShelfPrice = onShelfPrice;
        }

        //是否上架了相同的道具
        public bool IsOnShelfSameItemLastTime(int itemId, int strengthLevel, ref int onShelfPrice)
        {
            if (_previewOnShelfItemData == null)
                return false;

            if (_previewOnShelfItemData.ItemId != itemId)
                return false;

            if (_previewOnShelfItemData.StrengthLevel != strengthLevel)
                return false;

            //相同的道具，获得上次的价格
            onShelfPrice = _previewOnShelfItemData.OnShelfPrice;
            return true;
        }

        #endregion

        #region AuctionNewMagicCardOnSale

        public void ResetAuctionNewMagicCardOnSaleRes()
        {
            _worldAuctionQueryMagicOnSalesRes = null;
        }

        public WorldAuctionQueryMagicOnsalesRes GetAuctionNewMagicCardOnSaleRes()
        {
            return _worldAuctionQueryMagicOnSalesRes;
        }

        public void SendAuctionNewMagicCardOnSaleReq(uint itemId)
        {
            ResetAuctionNewMagicCardOnSaleRes();

            var req = new WorldAuctionQueryMagicOnsalesReq();
            req.itemTypeId = itemId;

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        private void OnReceiveAuctionNewMagicCardOnSaleRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            var res = new WorldAuctionQueryMagicOnsalesRes();
            res.decode(msgData.bytes);

            if (res.code != (uint) ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int) res.code);
                return;
            }

            _worldAuctionQueryMagicOnSalesRes = res;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAuctionNewWorldQueryMagicCardOnSaleResSucceed);
        }

        #endregion

        #region AuctionFreeze

        /// <summary>
        /// 异常交易记录查询
        /// </summary>
        public void SendSceneAbnormalTransactionRecordReq()
        {
            SceneAbnormalTransactionRecordReq req = new SceneAbnormalTransactionRecordReq();

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 拍卖行通知异常交易
        /// </summary>
        public void SceneNotifyAbnormalTransactionRet(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            var ret = new SceneNotifyAbnormalTransaction();
            ret.decode(msgData.bytes);

            var mIsFlag = ret.bNotify >= 1;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.AuctionFreezeRemind, mIsFlag);
        }


        /// <summary>
        /// 异常交易记录返回
        /// </summary>
        /// <param name="msg"></param>
        void SceneAbnormalTransactionRecordRet(MsgDATA msgData)
        {
            if (msgData == null)
                return;
            
            var ret = new SceneAbnormalTransactionRecordRes();
            ret.decode(msgData.bytes);

            AuctionFreezeRemindData data = new AuctionFreezeRemindData();
            data.FrozenMoneyType = ret.frozenMoneyType;
            data.FrozenAmount = ret.frozenAmount;
            data.AbnormalDealTimeStamp = ret.abnormalTransactionTime;
            data.BailFinishedTimeStamp = ret.backDeadline;
            data.RemainDayNumber = ret.backDeadline - TimeManager.GetInstance().GetServerTime();
            data.BackAmount = ret.backAmount;
            data.BackDayNumber = ret.backDays;

            if (ClientSystemManager.GetInstance().IsFrameOpen<AuctionFreezeRemindFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<AuctionFreezeRemindFrame>();
            }
            ClientSystemManager.GetInstance().OpenFrame<AuctionFreezeRemindFrame>(FrameLayer.Middle, data);
        }

        #endregion


    }
}
