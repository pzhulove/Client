using System;
using Protocol;
using Network;
using ProtoTable;
using System.Collections.Generic;
using UnityEngine;


namespace GameClient
{
    //消耗品的数据
    public class ShopNewCostItemDataModel

    {
        public int CostItemId;
        public int CostItemNumber;
    }


    public class ShopNewFilterData
    {
        public ShopTable.eFilter FilterType;     //过滤器的类型
        public int Index;                           //对应类型下的数值, 职业类型:职业ID；护甲或者等级：商店过滤表中的ID
        public string Name;                         //名字
        public bool IsSelected;                     //是否被选中，只是用于过滤器列表中的选中框的展示
        public ShopNewFilterTable FilterTableItem;       //选中框筛选的内容，对应ShopNewFilterTable表中的内容，针对 护甲和等级类型有效
    }

    //归属于某个商店的数据，主要是用于刷新
    public class ShopNewShopData
    {
        public int ShopId;
        //刷新消耗
        public int RefreshCost;

        //剩余刷新次数
        public int RefreshLeftTimes;
        //剩余刷新总次数
        public int RefreshAllTimes;

        //每日刷新剩余时间
        public uint ResetRefreshTime;
        //每周刷新剩余时间
        public uint WeekResetRefreshTime;
        //每月刷新剩余时间
        public uint MonthRefreshTime;
    }

    public class ShopNewShopItemInfo
    {
        //商店ID
        public int ShopId;

        //商品ID
        public int ShopItemId;

        //商品基础信息：对应商品表中的数值
        public ShopItemTable ShopItemTable;

        //格子数
        public int Grid;

        //剩余购买次数
        public int LimitBuyTimes = -1;              //-1 表示不限次数

        //VIP等级
        public int VipLimitLevel;

        //VIP折扣
        public int VipDiscount;

        //折扣
        public int GoodDiscount;
        public ShopNewShopItemInfo()
        {
            
        }
        public ShopNewShopItemInfo(ShopItemTable table, int time = -1)
        {
            ShopItemTable = table;
            ShopItemId = table.ID;
            ShopId = table.ShopID;
            LimitBuyTimes = time;
        }
    }

    //商店表中每个页签下的消耗品，只是针对商店表中，按照页签显示货币的商店
    public class ShopNewShopTabCostItem
    {
        public int ShopId;
        public int ShopTabIndex;
        public List<int> CostItems = new List<int>();
    }

    public class ShopNewDataManager : DataManager<ShopNewDataManager>
    {

        #region ShopNewData

        //使用新的商店框架
        //25 勇者商店
        //24 晶石商店
        //23 虚空商店
        //22 魔器商店
        //21 宠物商店
        //16 诺丁石商店
        //15 黑曜石商店
        //14 冰晶石商店
        //13 绿萤石商店
        //9 深渊商店
        //7 决斗商店
        //5 决斗商店

        private int[] _shopNewShopIdList = new int[]
        {
            5, 7, 9, 13,14,15,16, 21, 22, 23, 24, 25,
        };

        private int[] moneyItemShowName = new int[]
        {
            600000064,600000065,
        };

        public readonly Color specialColor =
            new Color(0xF4 * 1.0f / 0xFF, 0xDC * 1.0f / 0xFF, 0x89 * 1.0f / 0xFF, 1.0f);

        //shopId 和 ShopItemList
        //商店中商品的基础信息和服务器同步的数据信息
        private Dictionary<int, List<ShopNewShopItemInfo>> mShopNewShopItemInfoDictionary = 
            new Dictionary<int, List<ShopNewShopItemInfo>>();

        //商店的相关信息。只有在需要刷新的商店中才会存在，非刷新商店使用默认值
        private List<ShopNewShopData> _shopNewShopDataList = new List<ShopNewShopData>();

        //商店中消耗品的信息
        private Dictionary<int, List<int>> _shopCostItemIdDictionary = new Dictionary<int, List<int>>();
        //商店中页签对应的消耗品
        private List<ShopNewShopTabCostItem> _shopTabCostItemList = new List<ShopNewShopTabCostItem>();

        private Dictionary<ShopTable.eFilter, List<ShopNewFilterData>> _shopFilterDataDictionary =
            new Dictionary<ShopTable.eFilter, List<ShopNewFilterData>>();

        private List<int> _shopFirstFilterIndexList = new List<int>();
        private List<int> _shopSecondFilterIndexList = new List<int>();
        private List<int> _shopFilterTitleIndexList = new List<int>();       //是否显示对应过滤器title的标志
        private List<int> _shopHideFilterItemList = new List<int>();        //过滤器页签隐藏的ItemId

        //推荐页信息
        private List<MallRecommendPageInfo> mRecommendList = new List<MallRecommendPageInfo>();
        private List<ProtoShopItem> mRecommendShopItemList = new List<ProtoShopItem>();
        private List<AccountShopItemInfo> mRecommendAccountShopItemList = new List<AccountShopItemInfo>();
        private List<MallItemInfo> mRecommendMallItemList = new List<MallItemInfo>();
        
        private float timeInterval = 0.0f;

        #endregion

        #region InitManager

        public override void Initialize()
        {
            BindNetEvents();
        }

        private void BindNetEvents()
        {
            NetProcess.AddMsgHandler(SceneShopSync.MsgID, OnSyncShopNewShopItemTable);
            NetProcess.AddMsgHandler(SceneShopQueryBatchItemRes.MsgID, OnSyncSceneShopQueryBatchItemRes);
            NetProcess.AddMsgHandler(WorldAccountShopQueryBatchItemRes.MsgID, OnSyncWorldAccountShopQueryBatchItemRes);
            NetProcess.AddMsgHandler(WorldMallQueryBatchItemRes.MsgID, OnSyncWorldMallQueryBatchItemRes);
            NetProcess.AddMsgHandler(WorldMallRecommendPageListRes.MsgID, OnSyncWorldMallRecommendPageListRes);
            NetProcess.AddMsgHandler(WorldAccountShopItemBuyRes.MsgID, _OnWorldAccountShopItemBuyRes);
            NetProcess.AddMsgHandler(WorldMallBuyRet.MsgID, OnSyncWorldMallBuyRet);
        }

        public override void Clear()
        {
            ClearData();
            UnBindNetEvents();
        }

        private void UnBindNetEvents()
        {
            NetProcess.RemoveMsgHandler(SceneShopSync.MsgID, OnSyncShopNewShopItemTable);
            NetProcess.RemoveMsgHandler(SceneShopQueryBatchItemRes.MsgID, OnSyncSceneShopQueryBatchItemRes);
            NetProcess.RemoveMsgHandler(WorldAccountShopQueryBatchItemRes.MsgID, OnSyncWorldAccountShopQueryBatchItemRes);
            NetProcess.RemoveMsgHandler(WorldMallQueryBatchItemRes.MsgID, OnSyncWorldMallQueryBatchItemRes);
            NetProcess.RemoveMsgHandler(WorldMallRecommendPageListRes.MsgID, OnSyncWorldMallRecommendPageListRes);
            NetProcess.RemoveMsgHandler(WorldAccountShopItemBuyRes.MsgID, _OnWorldAccountShopItemBuyRes);
            NetProcess.RemoveMsgHandler(WorldMallBuyRet.MsgID, OnSyncWorldMallBuyRet);
        }

        public void ClearData()
        {
            var iter = mShopNewShopItemInfoDictionary.GetEnumerator();
            while (iter.MoveNext())
            {
                var shopItemList = iter.Current.Value;
                if(shopItemList != null)
                    shopItemList.Clear();
            }
            mShopNewShopItemInfoDictionary.Clear();

            _shopNewShopDataList.Clear();

            var costItemIter = _shopCostItemIdDictionary.GetEnumerator();
            while (costItemIter.MoveNext())
            {
                var shopCostItemList = costItemIter.Current.Value;
                if(shopCostItemList != null)
                    shopCostItemList.Clear();
            }
            _shopCostItemIdDictionary.Clear();

            for (var i = 0; i < _shopTabCostItemList.Count; i++)
            {
                if(_shopTabCostItemList[i] != null && _shopTabCostItemList[i].CostItems != null)
                    _shopTabCostItemList[i].CostItems.Clear();
            }
            _shopTabCostItemList.Clear();


            //过滤的数值
            var filterDataIter = _shopFilterDataDictionary.GetEnumerator();
            while (filterDataIter.MoveNext())
            {
                var filterDataList = filterDataIter.Current.Value;
                if(filterDataList != null)
                    filterDataList.Clear();
            }
            _shopFilterDataDictionary.Clear();

            _shopFirstFilterIndexList.Clear();
            _shopSecondFilterIndexList.Clear();
            _shopFilterTitleIndexList.Clear();
            _shopHideFilterItemList.Clear();
        }

        #endregion

        #region InitShopData
        //根据商店的ID，初始化商店的数据：主页签，过滤器，消耗品
        public void InitShopData(int shopId)
        {
            var shopNewTable = TableManager.GetInstance().GetTableItem<ShopTable>(shopId);
            if (shopNewTable == null)
            {
                Logger.LogErrorFormat("ShopNewTable is null and shopId is {0}", shopId);
                return;
            }

            var shopChildren = shopNewTable.Children;
            if (shopChildren.Count <= 0
                || (shopChildren.Count == 1 && shopChildren[0] == 0))
            {
                //单独一个商店
                InitShopItemTableByShopId(shopId);
            }
            else
            {
                //多个商店
                for (var i = 0; i < shopChildren.Count; i++)
                {
                    int childrenShopId = shopChildren[i];
                    if(childrenShopId == 0)
                        continue;

                    InitShopItemTableByShopId(childrenShopId);
                }
            }

            InitShopFilterDataList(shopNewTable);

        }

        //根据商店的ShopTable初始化对应的过滤器
        private void InitShopFilterDataList(ShopTable shopNewTable)
        {
            _shopFirstFilterIndexList.Clear();
            _shopSecondFilterIndexList.Clear();
            _shopFilterTitleIndexList.Clear();
            _shopHideFilterItemList.Clear();

            var filterCount = 0;
            filterCount = (shopNewTable.Filter.Count > shopNewTable.Filter2.Count)
                ? shopNewTable.Filter.Count
                : shopNewTable.Filter2.Count;

            for (var i = 0; i < filterCount; i++)
            {
                if (i >= shopNewTable.Filter.Count)
                {
                    _shopFirstFilterIndexList.Add(0);
                }
                else
                {
                    _shopFirstFilterIndexList.Add((int) shopNewTable.Filter[i]);
                }

                if (i >= shopNewTable.Filter2.Count)
                {
                    _shopSecondFilterIndexList.Add(0);
                }
                else
                {
                    _shopSecondFilterIndexList.Add(shopNewTable.Filter2[i]);
                }

                if (i >= shopNewTable.IsShowFilterTitle.Count)
                {
                    _shopFilterTitleIndexList.Add(0);
                }
                else
                {
                    _shopFilterTitleIndexList.Add(shopNewTable.IsShowFilterTitle[i]);
                }
            }

            if (shopNewTable.HideFilterItem != null && shopNewTable.HideFilterItem.Count >0)
            {
                for (var i = 0; i < shopNewTable.HideFilterItem.Count; i++)
                {
                    if(shopNewTable.HideFilterItem[i] == 0)
                        continue;

                    _shopHideFilterItemList.Add(shopNewTable.HideFilterItem[i]);
                }
            }
        }

        //获得每个具体商店对应的ShopItemTable以及消耗品的信息
        public void InitShopItemTableByShopId(int shopId)
        {
            //添加商店的整体信息
            var shopNewShopData = _shopNewShopDataList.Find(x => x.ShopId == shopId);
            if (shopNewShopData == null)
            {
                shopNewShopData = new ShopNewShopData()
                {
                    ShopId = shopId,
                };
                _shopNewShopDataList.Add(shopNewShopData);
            }

            //商品
            List<ShopNewShopItemInfo> shopNewShopItemTableList = null;
            if (mShopNewShopItemInfoDictionary.TryGetValue(shopId, out shopNewShopItemTableList) == false)
            {
                shopNewShopItemTableList = new List<ShopNewShopItemInfo>();
                mShopNewShopItemInfoDictionary.Add(shopId, shopNewShopItemTableList);
            }
            shopNewShopItemTableList.Clear();

            //消耗品
            List<int> shopCostItemIdList = null;
            if (_shopCostItemIdDictionary.TryGetValue(shopId, out shopCostItemIdList) == false)
            {
                shopCostItemIdList = new List<int>();
                _shopCostItemIdDictionary.Add(shopId, shopCostItemIdList);
            }
            shopCostItemIdList.Clear();

            //商店的数据，根据商店的数据，判断是否生成页签对应的消耗品
            var curShopTable = TableManager.GetInstance().GetTableItem<ShopTable>(shopId);

            var shopItemTables = TableManager.GetInstance().GetTable<ShopItemTable>();
            var iter = shopItemTables.GetEnumerator();
            while (iter.MoveNext())
            {
                var shopItemTable = iter.Current.Value as ShopItemTable;
                if(shopItemTable == null)
                    continue;

                if (shopItemTable.ShopID == shopId)
                {
                    var shopNewShopItemTable = new ShopNewShopItemInfo()
                    {
                        ShopId = shopId,
                        ShopItemId = shopItemTable.ID,
                        LimitBuyTimes = -1,
                        ShopItemTable = shopItemTable,
                    };
                    shopNewShopItemTableList.Add(shopNewShopItemTable);

                    if (curShopTable != null && curShopTable.CurrencyShowType == 0)
                    {
                        //消耗品
                        if (shopCostItemIdList.Contains(shopItemTable.CostItemID) == false)
                        {
                            shopCostItemIdList.Add(shopItemTable.CostItemID);
                        }
                    }
                    
                }
            }

            //商品排序
            shopNewShopItemTableList.Sort((x, y) => x.ShopItemTable.SortID.CompareTo(y.ShopItemTable.SortID));

            //额外的消耗品
            var shopNewTable = TableManager.GetInstance().GetTableItem<ShopTable>(shopId);
            if (shopNewTable != null)
            {
                try
                {
                    var shopExtraMoneyId = int.Parse(shopNewTable.ExtraShowMoneys);
                    if (shopExtraMoneyId > 0)
                    {
                        if (shopCostItemIdList.Contains(shopExtraMoneyId) == false)
                        {
                            shopCostItemIdList.Add(shopExtraMoneyId);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogErrorFormat("excption is {0}", e.ToString());
                }
            }
            //消耗品排序
            shopCostItemIdList.Sort((x, y) => x.CompareTo(y));

            //商店页签的额外消耗品
            if (curShopTable != null && curShopTable.CurrencyShowType == 1)
            {
                AddShopTabExtraCostItem(curShopTable);
            }
        }

        ////根据商品的信息，存储商店中页签下的消耗品
        //private void AddShopTabCostItem(int shopId, ShopItemTable shopItemTable)
        //{
        //    AddOneShopTabCostItem(shopId, (int)shopItemTable.SubType, shopItemTable.CostItemID);
        //}

        //增加商店中页签的额外的消耗品
        private void AddShopTabExtraCostItem(ShopTable shopTable)
        {
            var shopId = shopTable.ID;
            for (var i = 0; i < shopTable.SubType.Count; i++)
            {
                var shopTabIndex = (int)shopTable.SubType[i];
                if(i >= shopTable.CurrencyExtraItem.Count)
                    continue;

                var currentExtraItem = shopTable.CurrencyExtraItem[i];
                var currentExtraItemList = currentExtraItem.Split('_');
                var currentExtraItemCount = currentExtraItemList.Length;
                if(currentExtraItemCount <= 0)
                    continue;

                for (var j = 0; j < currentExtraItemCount; j++)
                {
                    var curExtraItemIdStr = currentExtraItemList[j];
                    var curExtraItemId = int.Parse(curExtraItemIdStr);
                    if(curExtraItemId <= 0)
                        continue;
                    AddOneShopTabCostItem(shopId, shopTabIndex, curExtraItemId);
                }
            }
        }

        //添加一个消耗品
        private void AddOneShopTabCostItem(int shopId, int shopTabIndex, int costItem)
        {
            //按照页签显示货币，则需要获得页签下对应的货币
            bool isFind = false;
            ShopNewShopTabCostItem curShopTabCostItem = null;
            for (var i = 0; i < _shopTabCostItemList.Count; i++)
            {
                curShopTabCostItem = _shopTabCostItemList[i];
                if (curShopTabCostItem == null)
                    continue;

                if (curShopTabCostItem.ShopId == shopId
                    && curShopTabCostItem.ShopTabIndex == shopTabIndex)
                {
                    isFind = true;
                    break;
                }
            }

            if (isFind == true)
            {
                //在页签下添加CostItem
                if (curShopTabCostItem.CostItems.Contains(costItem) == false)
                    curShopTabCostItem.CostItems.Add(costItem);
            }
            else
            {
                //创建页签的Item， 之后添加CostItem
                curShopTabCostItem = new ShopNewShopTabCostItem
                {
                    ShopId = shopId,
                    ShopTabIndex = shopTabIndex,
                };
                curShopTabCostItem.CostItems.Add(costItem);
                _shopTabCostItemList.Add(curShopTabCostItem);
            }
        }

        //对消耗品进行排序
        private void SortShopTabCostItem()
        {
            for (var i = 0; i < _shopTabCostItemList.Count; i++)
            {
                if (_shopTabCostItemList[i] != null
                    && _shopTabCostItemList[i].CostItems != null
                    && _shopTabCostItemList[i].CostItems.Count > 1)
                {
                    _shopTabCostItemList[i].CostItems.Sort((x, y) => x.CompareTo(y));
                }
            }
        }

        #endregion

        #region DealWithShopData
        //获得MainTab的数据（商店类型或者Item类型）
        public List<ShopNewMainTabData> GetShopNewMainTabDataList(int shopId)
        {
            var shopNewTable = TableManager.GetInstance().GetTableItem<ShopTable>(shopId);
            if (shopNewTable == null)
            {
                Logger.LogErrorFormat("ShopNewTable is null and shopId is {0}", shopId);
                return null;
            }

            var shopNewMainTabDataList = new List<ShopNewMainTabData>();

            var shopNewTableChildren = shopNewTable.Children;
            if (shopNewTableChildren.Count <= 0
                || (shopNewTableChildren.Count == 1 && shopNewTableChildren[0] == 0))
            {
                //Item类型的页签
                for (var i = 0; i < shopNewTable.SubType.Count; i++)
                {
                    if(shopNewTable.SubType[i] == 0)
                        continue;

                    var tabData = new ShopNewMainTabData
                    {
                        MainTabType = ShopNewMainTabType.ItemType,
                        Index = (int) shopNewTable.SubType[i],
                        Name = TR.Value(string.Format(TR.Value("shop_tab_format"), (int) shopNewTable.SubType[i])),
                    };
                    shopNewMainTabDataList.Add(tabData);
                }
            }
            else
            {
                //商店类型的页签
                for (var i = 0; i < shopNewTableChildren.Count; i++)
                {
                    var childrenShopId = shopNewTableChildren[i];
                    var childrenShopTable = TableManager.GetInstance().GetTableItem<ShopTable>(childrenShopId);
                    if(childrenShopTable == null)
                        continue;

                    var tabData = new ShopNewMainTabData
                    {
                        MainTabType = ShopNewMainTabType.ShopType,
                        Index = childrenShopId,
                    };

                    //判断是否使用商店名字的简称（晶石商店）
                    if (string.IsNullOrEmpty(childrenShopTable.ShopSimpleName) == true)
                    {
                        tabData.Name = childrenShopTable.ShopName;
                    }
                    else
                    {
                        tabData.Name = childrenShopTable.ShopSimpleName;
                    }

                    shopNewMainTabDataList.Add(tabData);
                }
            }
            return shopNewMainTabDataList;
        }

        //获得商店所有元素信息
        public List<ShopNewShopItemInfo> GetOriginalShopNewShopItemList(int shopId)
        {
            List<ShopNewShopItemInfo> shopNewShopItemTableList = null;

            mShopNewShopItemInfoDictionary.TryGetValue(shopId, out shopNewShopItemTableList);

            return shopNewShopItemTableList;
        }

        //根据过滤器获得对应商品列表的数据
        public List<ShopNewShopItemInfo> GetShopNewNeedShowingShopItemList(int shopId,
            ShopNewMainTabData shopNewMainTabData = null,
            ShopNewFilterData firstFilterData = null,
            ShopNewFilterData secondFilterData = null)
        {
            List<ShopNewShopItemInfo> originalShopItemList = null;

            //获得商店中的所有的商品数量
            if (shopNewMainTabData == null || shopNewMainTabData.MainTabType == ShopNewMainTabType.ItemType)
            {
                //单独的一个商店
                originalShopItemList = GetOriginalShopNewShopItemList(shopId);
            }
            else
            {
                //多个子商店
                if (shopNewMainTabData.MainTabType == ShopNewMainTabType.ShopType)
                {
                    originalShopItemList = GetOriginalShopNewShopItemList(shopNewMainTabData.Index);
                }
            }

            if (originalShopItemList == null)
                return null;

            List<ShopNewShopItemInfo> filterShopItemList = new List<ShopNewShopItemInfo>();
            for (var i = 0; i < originalShopItemList.Count; i++)
            {
                if(originalShopItemList[i].ShopItemTable == null)
                    continue;
                filterShopItemList.Add(originalShopItemList[i]);
            }

            filterShopItemList.Sort((x, y) => x.ShopItemTable.SortID.CompareTo(y.ShopItemTable.SortID));
            
            //过滤ItemType类型的元素
            if (shopNewMainTabData != null
                && shopNewMainTabData.MainTabType == ShopNewMainTabType.ItemType)
            {
                var itemType = (ShopTable.eSubType) shopNewMainTabData.Index;
                if (itemType != ShopTable.eSubType.ST_NONE)
                {
                    filterShopItemList = filterShopItemList.FindAll(x => (x.ShopItemTable != null
                                                                          && ((int) x.ShopItemTable.SubType ==
                                                                              (int) itemType)));
                }
            }

            //使用两种过滤器进行商品的过滤
            filterShopItemList =
                FilterShopItemTableListByFilterData(filterShopItemList, firstFilterData, secondFilterData);

            //根据商品的等级限制过滤商品
            filterShopItemList = FilterShopItemTableListByShopLevel(filterShopItemList);

            return filterShopItemList;
        }

        private List<ShopNewShopItemInfo> FilterShopItemTableListByShopLevel(
            List<ShopNewShopItemInfo> shopNewShopItemTableList)
        {
            if (shopNewShopItemTableList == null || shopNewShopItemTableList.Count <= 0)
                return shopNewShopItemTableList;

            var itemsCount = shopNewShopItemTableList.Count;
            for (var i = itemsCount - 1; i >= 0; i--)
            {
                var shopNewShopItemTable = shopNewShopItemTableList[i];
                if(shopNewShopItemTable.ShopItemTable == null)
                    continue;

                //角色的等级小于限制的角色，则该商品不显示，数据删除
                if (shopNewShopItemTable.ShopItemTable.ShowLevelLimit > PlayerBaseData.GetInstance().Level)
                {
                    shopNewShopItemTableList.RemoveAt(i);
                }
            }

            return shopNewShopItemTableList;
        }

        //根据商店的过滤器来过滤商品
        private List<ShopNewShopItemInfo> FilterShopItemTableListByFilterData(List<ShopNewShopItemInfo> shopNewShopItemTableList, 
            ShopNewFilterData firstFilterData,
            ShopNewFilterData secondFilterData)
        {
            if (shopNewShopItemTableList == null || shopNewShopItemTableList.Count <= 0)
                return shopNewShopItemTableList;

            var itemsCount = shopNewShopItemTableList.Count;

            for (var i = itemsCount - 1; i >= 0; i--)
            {
                var shopNewShopItemTable = shopNewShopItemTableList[i];
                if(shopNewShopItemTable.ShopItemTable == null)
                    continue;

                var itemId = shopNewShopItemTable.ShopItemTable.ItemID;

                var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(itemId);

                if (itemTable == null)
                {
                    shopNewShopItemTableList.RemoveAt(i);
                    continue;
                }

                //是否展示商品
                var isShow = true;
                isShow = IsItemNeedShow(itemTable, firstFilterData);
                if (isShow == true)
                {
                    isShow = IsItemNeedShow(itemTable, secondFilterData);
                }

                if (isShow == false)
                {
                    shopNewShopItemTableList.RemoveAt(i);
                }
            }

            return shopNewShopItemTableList;
        }

        private bool IsItemNeedShow(ItemTable itemTable, ShopNewFilterData filterData)
        {
            if (filterData == null || filterData.FilterType == ShopTable.eFilter.SF_NONE)
                return true;

            var flag = false;
            if (filterData.FilterType == ShopTable.eFilter.SF_ARMOR)
            {
                //flag = IsItemNeedShowByItemType(itemTable, filterData.Index);
                flag = IsItemNeedShowByItemType(itemTable, filterData);
            }
            else if (filterData.FilterType == ShopTable.eFilter.SF_OCCU)
            {
                if(itemTable.Occu.Count == 0 ||
                   (itemTable.Occu.Count == 1 && itemTable.Occu[0] == 0))
                {
                    flag = true;
                }
                else
                {
                    for (var j = 0; j < itemTable.Occu.Count; j++)
                    {
                        var curItemOccu = itemTable.Occu[j];
                        if (curItemOccu == 0 || curItemOccu == filterData.Index)
                        {
                            flag = true;
                        }
                        //判断是否为小职业的数值
                        if (flag == false)
                        {
                            var curJobTable = TableManager.GetInstance().GetTableItem<JobTable>(curItemOccu);
                            if (curJobTable != null && curJobTable.prejob == filterData.Index)
                                flag = true;
                        }

                    }
                }

                //判断Item的小职业是否属于基础职业
                if (flag == false)
                {
                    if (itemTable.Occu2.Count >= 1)
                    {
                        for (var j = 0; j < itemTable.Occu2.Count; j++)
                        {
                            var curOccu2 = itemTable.Occu2[j];
                            var curJobTable = TableManager.GetInstance().GetTableItem<JobTable>(curOccu2);
                            if (curJobTable != null && curJobTable.prejob == filterData.Index)
                                flag = true;
                        }
                    }
                }

            }
            else if (filterData.FilterType == ShopTable.eFilter.SF_OCCU2)
            {
                if (itemTable.Occu2.Count == 0 ||
                    (itemTable.Occu2.Count == 1 && itemTable.Occu2[0] == 0))
                {
                    flag = true;
                }
                else
                {
                    for (var j = 0; j < itemTable.Occu2.Count; j++)
                    {
                        if (itemTable.Occu2[j] == 0 || itemTable.Occu2[j] == filterData.Index)
                        {
                            flag = true;
                        }
                    }
                }
            }
            else if (filterData.FilterType == ShopTable.eFilter.SF_PLAY_OCCU)
            {
                if (itemTable.Occu2.Count == 0 ||
                    (itemTable.Occu2.Count == 1 && itemTable.Occu2[0] == 0))
                {
                    flag = true;
                }
                else
                {
                    for (var j = 0; j < itemTable.Occu2.Count; j++)
                    {
                        if (itemTable.Occu2[j] == 0 || itemTable.Occu2[j] == PlayerBaseData.GetInstance().JobTableID)
                        {
                            flag = true;
                        }
                    }
                }
            }
            else if (filterData.FilterType == ShopTable.eFilter.SF_LEVEL)
            {
                if (filterData.FilterTableItem == null || filterData.FilterTableItem.Parameter == null
                                                       || filterData.FilterTableItem.Parameter.Count <= 1)
                {
                    flag = true;
                }
                else
                {

                    if (itemTable.NeedLevel >= filterData.FilterTableItem.Parameter[0] &&
                        itemTable.NeedLevel <= filterData.FilterTableItem.Parameter[1])
                        flag = true;
                }
            }
            return flag;
        }

        //如果类型是护甲的话，就是除其他各种甲之外的各种东西
        //比如：布甲类型：布甲可以显示；其他护甲类型的物品不可以显示；非护甲类型的物品可以显示

        private bool IsItemNeedShowByItemType(ItemTable itemTable, ShopNewFilterData filterData)
        {
            if (filterData.FilterTableItem == null || filterData.FilterTableItem.Parameter == null
                                                   || filterData.FilterTableItem.Parameter.Count <= 0)
                return true;

            //护甲类型相同
            if ((int) itemTable.ThirdType == filterData.FilterTableItem.Parameter[0])
                return true;

            var shopNewFilterDataList = GetShopNewFilterDataList(ShopTable.eFilter.SF_ARMOR);
            for (var i = 0; i < shopNewFilterDataList.Count; i++)
            {
                var curFilterData = shopNewFilterDataList[i];
                if (curFilterData.FilterTableItem != null && curFilterData.FilterTableItem.Parameter != null
                                                          && curFilterData.FilterTableItem.Parameter.Count == 1)
                {
                    if ((int) itemTable.ThirdType == curFilterData.FilterTableItem.Parameter[0])
                        return false;
                }
            }

            return true;
        }

        //获得商店的消耗品信息
        public List<int> GetShopCostItems(int shopId)
        {
            List<int> shopCostItemList = null;
            _shopCostItemIdDictionary.TryGetValue(shopId, out shopCostItemList);
            return shopCostItemList;
        }

        //获得商店某个页签对应的消耗品
        public List<int> GetShopCostItemsByShopTab(int shopId, int shopTabIndex)
        {
            List<int> shopTabCostItemList = null;
            for (var i = 0; i < _shopTabCostItemList.Count; i++)
            {
                var shopTabCostItem = _shopTabCostItemList[i];
                if(shopTabCostItem == null)
                    continue;

                if (shopTabCostItem.ShopId == shopId && shopTabCostItem.ShopTabIndex == shopTabIndex)
                    shopTabCostItemList = shopTabCostItem.CostItems;
            }
            return shopTabCostItemList;
        }

        public void BuyGoods(int shopId, int goodId, int count, List<ItemInfo> info)
        {

            var msg = new SceneShopBuy()
            {
                shopId = (byte)shopId,
                shopItemId = (uint)goodId,
                num = (ushort)count,
                costExtraItems = info.ToArray(),
            };

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<SceneShopBuyRet>(msgRet =>
            {
                if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.code);
                }
                else
                {

                    List<ShopNewShopItemInfo> shopNewShopItemTableList = null;
                    if (mShopNewShopItemInfoDictionary == null || mShopNewShopItemInfoDictionary.Count <= 0)
                        return;

                    if (mShopNewShopItemInfoDictionary.TryGetValue(shopId, out shopNewShopItemTableList) == false)
                        return;

                    if (shopNewShopItemTableList == null || shopNewShopItemTableList.Count <= 0)
                        return;

                    var shopNewShopItemTable = shopNewShopItemTableList.Find(x => x.ShopItemId == msgRet.shopItemId);

                    if (shopNewShopItemTable == null)
                        return;


                    shopNewShopItemTable.LimitBuyTimes = shopNewShopItemTable.LimitBuyTimes >= 0 ? msgRet.newNum : -1;

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ShopNewBuyGoodsSuccess, shopNewShopItemTable.ShopItemId, msgRet.newNum);
                }
            });
        }

        #endregion

        #region FilterData
        //过滤器相关类型获得过滤器的数据

        public ShopNewFilterData GetDefaultFilterDataByFilterType(ShopTable.eFilter filterType)
        {
            var shopNewFilterData = new ShopNewFilterData();

            shopNewFilterData.FilterType = filterType;

            //职业
            if (filterType == ShopTable.eFilter.SF_OCCU)
            {
                var baseJobId = Utility.GetBaseJobID(PlayerBaseData.GetInstance().JobTableID);
                shopNewFilterData.Index = baseJobId;

                var jobTable = TableManager.GetInstance().GetTableItem<JobTable>(baseJobId);
                if (jobTable != null)
                    shopNewFilterData.Name = jobTable.Name;

            }
            else if (filterType == ShopTable.eFilter.SF_OCCU2)
            {
                //小职业
                var betterJobId = Utility.GetBetterJobId(PlayerBaseData.GetInstance().JobTableID);
                shopNewFilterData.Index = betterJobId;

                var jobTable = TableManager.GetInstance().GetTableItem<JobTable>(betterJobId);
                if (jobTable != null)
                    shopNewFilterData.Name = jobTable.Name;
            }
            else if (filterType == ShopTable.eFilter.SF_ARMOR)
            {
                //护甲
                var shopNewFilterDataList =
                    ShopNewDataManager.GetInstance().GetShopNewFilterDataList(ShopTable.eFilter.SF_ARMOR);
                if (shopNewFilterDataList != null && shopNewFilterDataList.Count > 0)
                {
                    shopNewFilterData = shopNewFilterDataList[0];
                    var jobItem = TableManager.GetInstance()
                        .GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
                    if (jobItem != null && jobItem.SuitArmorType > 0)
                    {
                        for (var i = 0; i < shopNewFilterDataList.Count; i++)
                        {
                            var curFilterData = shopNewFilterDataList[i];
                            if (curFilterData != null && curFilterData.Index == jobItem.SuitArmorType)
                            {
                                shopNewFilterData = curFilterData;
                            }
                        }
                    }
                }
            }
            else if (filterType == ShopTable.eFilter.SF_LEVEL)
            {
                //等级
                var shopNewFilterDataList =
                    ShopNewDataManager.GetInstance().GetShopNewFilterDataList(ShopTable.eFilter.SF_LEVEL);
                if (shopNewFilterDataList != null && shopNewFilterDataList.Count > 0)
                {
                    shopNewFilterData = shopNewFilterDataList[0];
                    for (var i = 1; i < shopNewFilterDataList.Count; i++)
                    {
                        var curFilterData = shopNewFilterDataList[i];
                        if(curFilterData == null 
                           || curFilterData.FilterTableItem == null
                           || curFilterData.FilterTableItem.Parameter == null
                           || curFilterData.FilterTableItem.Parameter.Length <= 0)
                            continue;

                        if (PlayerBaseData.GetInstance().Level >= curFilterData.FilterTableItem.Parameter[0]
                            && PlayerBaseData.GetInstance().Level <= curFilterData.FilterTableItem.Parameter[1])
                        {
                            shopNewFilterData = curFilterData;
                            break;
                        }
                    }
                }
            }
            //ShopTable.eFilter.SF_PLAY_OCCU 直接返回

            return shopNewFilterData;
        }

        public int GetFirstFilterIndex(int index)
        {
            if (index < 0 || index >= _shopFirstFilterIndexList.Count)
                return 0;

            return _shopFirstFilterIndexList[index];
        }

        public int GetSecondFilterIndex(int index)
        {
            if (index < 0 || index >= _shopSecondFilterIndexList.Count)
                return 0;

            return _shopSecondFilterIndexList[index];
        }

        //是否显示过滤器的页签
        public int GetFilterTitleIndex(int index)
        {
            if (index < 0 || index >= _shopFilterTitleIndexList.Count)
                return 0;

            return _shopFilterTitleIndexList[index];
        }

        public List<ShopNewFilterData> GetShopNewFilterDataList(ShopTable.eFilter filterType)
        {
            if (filterType == ShopTable.eFilter.SF_NONE)
                return null;


            List<ShopNewFilterData> shopNewFilterDataList = null;
            //在缓存中查找，如果找到，直接返回，否则的话，需要添加
            if (_shopFilterDataDictionary.TryGetValue(filterType, out shopNewFilterDataList) == false)
            {
                //没有找到，直接初始化，并进行缓存
                shopNewFilterDataList = InitShopNewFilterDataListByFilterType(filterType);
                _shopFilterDataDictionary.Add(filterType, shopNewFilterDataList);
            }
            
            //根据过滤器类型返回过滤器相关的数据
            return shopNewFilterDataList;
        }

        //根据类型初始化过滤器的数据
        private List<ShopNewFilterData> InitShopNewFilterDataListByFilterType(ShopTable.eFilter filterType)
        {
            switch (filterType)
            {
                case ShopTable.eFilter.SF_OCCU:
                    return InitBaseJobFilterDataList();
                case ShopTable.eFilter.SF_OCCU2:
                    return InitBetterJobFilterDataList();
                case ShopTable.eFilter.SF_ARMOR:
                    return InitFilterDataListByFilterType(ShopTable.eFilter.SF_ARMOR);
                case ShopTable.eFilter.SF_LEVEL:
                    return InitFilterDataListByFilterType(ShopTable.eFilter.SF_LEVEL);
            }
            return null;
        }

        //基础职业
        private List<ShopNewFilterData> InitBaseJobFilterDataList()
        {
            List<ShopNewFilterData> shopNewFilterDataList = new List<ShopNewFilterData>();

            var jobTable = TableManager.GetInstance().GetTable<JobTable>();
            if (jobTable != null)
            {
                var iter = jobTable.GetEnumerator();
                while (iter.MoveNext())
                {
                    var current = iter.Current.Value as JobTable;
                    if (current != null
                        && current.JobType == 0
                        && current.Open == 1)
                    {
                        var shopNewFilterData = new ShopNewFilterData()
                        {
                            FilterType = ShopTable.eFilter.SF_OCCU,
                            Name = current.Name,
                            Index = current.ID,
                        };
                        shopNewFilterDataList.Add(shopNewFilterData);
                    }
                }
            }
            return shopNewFilterDataList;
        }

        //小职业
        private List<ShopNewFilterData> InitBetterJobFilterDataList()
        {
            List<ShopNewFilterData> shopNewFilterDataList = new List<ShopNewFilterData>();

            var jobTable = TableManager.GetInstance().GetTable<JobTable>();
            if (jobTable != null)
            {
                var iter = jobTable.GetEnumerator();
                while (iter.MoveNext())
                {
                    var current = iter.Current.Value as JobTable;
                    if (current != null
                        && current.JobType == 1
                        && current.Open == 1)
                    {
                        var shopNewFilterData = new ShopNewFilterData()
                        {
                            FilterType = ShopTable.eFilter.SF_OCCU2,
                            Name = current.Name,
                            Index = current.ID,
                        };
                        shopNewFilterDataList.Add(shopNewFilterData);
                    }
                }
            }
            return shopNewFilterDataList;
        }

        private List<ShopNewFilterData> InitFilterDataListByFilterType(ShopTable.eFilter eFilter)
        {
            List<ShopNewFilterData> shopNewFilterDataList = new List<ShopNewFilterData>();

            var shopNewFilterTable = TableManager.GetInstance().GetTable<ShopNewFilterTable>();
            if (shopNewFilterTable != null)
            {
                var iter = shopNewFilterTable.GetEnumerator();
                while (iter.MoveNext())
                {
                    var currentTableItem = iter.Current.Value as ShopNewFilterTable;
                    if (currentTableItem != null)
                    {
                        if (currentTableItem.FilterItemType == (int) eFilter)
                        {
                            bool isHideFilterItem = false;
                            for (var i = 0; i < _shopHideFilterItemList.Count; i++)
                            {
                                if (_shopHideFilterItemList[i] == currentTableItem.ID)
                                    isHideFilterItem = true;
                            }

                            if(isHideFilterItem == true)
                                continue;

                            var shopNewFilterData = new ShopNewFilterData()
                            {
                                FilterType = eFilter,
                                Name = currentTableItem.Name,
                                Index = currentTableItem.ID,
                                FilterTableItem = currentTableItem,
                            };
                            shopNewFilterDataList.Add(shopNewFilterData);
                        }
                    }
                }
            }

            shopNewFilterDataList.Sort((x, y) => x.FilterTableItem.Sort.CompareTo(y.FilterTableItem.Sort));

            return shopNewFilterDataList;
        }
        
        #endregion

        #region OpenShopFrame

        public void OpenShopNewFrame(int shopId, int shopChildrenId = 0, int shopItemType = 0, int npcId = -1, int shopItemId = 0)
        {
            //首先关闭界面
            ShopNewUtility.OnCloseShopNewFrame();

            var shopNewParamData = new ShopNewParamData()
            {
                ShopId = shopId,
                ShopChildId = shopChildrenId,
                ShopItemType = shopItemType,
                NpcId = npcId,
                shopItemId = shopItemId,
            };

            var shopNewTable = TableManager.GetInstance().GetTableItem<ShopTable>(shopId);
            if (shopNewTable == null)
            {
                Logger.LogErrorFormat("The shopNewTable is null and shop id is {0}", shopId);
                return;
            }

            InitShopData(shopId);

            //不需要刷新，直接使用表中的数据
            if (shopNewTable.Refresh == 0)
            {
                ClientSystemManager.GetInstance().OpenFrame<ShopNewFrame>(FrameLayer.Middle, shopNewParamData);
            }
            else
            {
                OpenRefreshShopFrame(shopId, shopNewParamData);
            }
        }

        private void OpenRefreshShopFrame(int shopId, ShopNewParamData shopNewParamData)
        {
            SendSceneShopQuery(shopId);
            WaitSceneShopQueryRet(shopId, shopNewParamData);
        }

        //请求子页签为商店类型的商店数据
        public void SendRequestChildrenShopData(int shopId, int childrenShopId)
        {
            SendSceneShopQuery(childrenShopId);
            WaitRequestChildrenShopData(shopId, childrenShopId);
        }

        private void SendSceneShopQuery(int shopId)
        {
            SceneShopQuery sceneShopQuery = new SceneShopQuery();
            sceneShopQuery.shopId = (byte) shopId;
            sceneShopQuery.cache = (byte) 0;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, sceneShopQuery);

        }

        private void WaitRequestChildrenShopData(int shopId, int childrenShopId)
        {
            WaitNetMessageManager.GetInstance().Wait<SceneShopQueryRet>(msgRet =>
            {
                if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.code);
                    return;
                }
                else
                {
                    //请求子商店的数据成功，发送通知消息
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ShopNewRequestChildrenShopDataSucceed, shopId,
                        childrenShopId);
                }
            });
        }

        private void WaitSceneShopQueryRet(int shopId, ShopNewParamData shopNewParamData)
        {
            WaitNetMessageManager.GetInstance().Wait<SceneShopQueryRet>(msgRet =>
            {
                if (msgRet.code != (uint) ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int) msgRet.code);
                    return;
                }
                else
                {
                    ClientSystemManager.GetInstance().OpenFrame<ShopNewFrame>(FrameLayer.Middle, shopNewParamData);
                }
            });
        }

        public ShopNewShopData GetShopNewShopData(int shopId)
        {
            if (_shopNewShopDataList == null || _shopNewShopDataList.Count == 0)
                return null;

            var shopNewShopData = _shopNewShopDataList.Find(x => x.ShopId == shopId);
            return shopNewShopData;
        }

        private void OnSyncShopNewShopItemTable(MsgDATA msg)
        {
            //在吃鸡场景中，不处理（吃鸡场景中有单独的处理机制）
            if (CommonUtility.IsInGameBattleScene() == true)
                return;

            CustomDecoder.ProtoShop msgRet;
            var pos = 0;

            if (CustomDecoder.DecodeShop(out msgRet, msg.bytes, ref pos, msg.bytes.Length) == false)
            {
                Logger.LogErrorFormat("Open ShopNewFrame OnSyncShopItem Decode is error");
                return;
            }

            if (msgRet == null)
            {
                Logger.LogErrorFormat("Open ShopNewFrame OnSyncShopItem Decode msgRet is null");
                return;
            }

            //商店的整体数值：刷新时间
            var shopId = (int)msgRet.shopID;
            var shopNewShopData = _shopNewShopDataList.Find(x => x.ShopId == shopId);
            if (shopNewShopData == null)
            {
                shopNewShopData = new ShopNewShopData();
                _shopNewShopDataList.Add(shopNewShopData);
            }

            shopNewShopData.ShopId = shopId;
            shopNewShopData.RefreshCost = (int) msgRet.refreshCost;

            shopNewShopData.ResetRefreshTime = msgRet.restRefreshTime;
            shopNewShopData.WeekResetRefreshTime = msgRet.WeekRestRefreshTime;
            shopNewShopData.MonthRefreshTime = msgRet.MonthRefreshTime;

            shopNewShopData.RefreshLeftTimes = msgRet.refreshTimes;
            shopNewShopData.RefreshAllTimes = msgRet.refreshAllTimes;


            //商店中商品相关信息，商品同步的相关信息
            List<ShopNewShopItemInfo> shopNewShopItemTableList = null;
            if (mShopNewShopItemInfoDictionary.TryGetValue(shopId, out shopNewShopItemTableList) == false)
            {
                shopNewShopItemTableList = new List<ShopNewShopItemInfo>();
                mShopNewShopItemInfoDictionary.Add(shopId, shopNewShopItemTableList);
            }
            shopNewShopItemTableList.Clear();

            for (var i = 0; i < msgRet.shopItemList.Count; i++)
            {
                var protoShopItem = msgRet.shopItemList[i];

                var shopItemId = (int)protoShopItem.shopItemId;
                var shopItemTable = TableManager.GetInstance().GetTableItem<ShopItemTable>(shopItemId);
                if (shopItemTable == null)
                {
                    Logger.LogErrorFormat("ShopItemTable is null and shopItemId is {0}", shopItemId);
                    continue;
                }

                var shopNewShopItemTable = new ShopNewShopItemInfo();
                shopNewShopItemTable.ShopItemTable = shopItemTable;
                shopNewShopItemTable.ShopId = shopId;
                shopNewShopItemTable.ShopItemId = shopItemId;

                shopNewShopItemTable.VipLimitLevel = protoShopItem.vipLv;
                shopNewShopItemTable.VipDiscount = protoShopItem.vipDiscount;
                shopNewShopItemTable.LimitBuyTimes = protoShopItem.restNum;
                shopNewShopItemTable.GoodDiscount = (int) protoShopItem.discount;

                shopNewShopItemTableList.Add(shopNewShopItemTable);
            }
        }
        
        //请求拉取推荐页界面
        public void ReqWorldMallRecommendPageList()
        {
            var req = new WorldMallRecommendPageListReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        //拉取推荐页界面返回
        private void OnSyncWorldMallRecommendPageListRes(MsgDATA msg)
        {
            var res = new WorldMallRecommendPageListRes();
            res.decode(msg.bytes);
            mRecommendList.Clear();
            var mallList = new List<uint>();
            var shopList = new List<uint>();
            var accountShopList = new List<uint>();
            foreach (var item in res.pageInfos)
            {
                mRecommendList.Add(item);
                if (item.linkFunctionType == (byte)MallRecommendPageTable.eLinkFunctionType.LINK_FUNCTION_TYPE_BUY || 
                    item.recommendType == (byte)MallRecommendPageTable.eRecommendType.RECOMMEND_TYPE_ITEM)
                {
                    if (item.itemBelongMallType == (byte)MallRecommendPageTable.eItemBelongMallType.ITEM_BELONG_MALL_TYPE_MALL)
                        mallList.Add(item.mallItemID);
                    if (item.itemBelongMallType == (byte)MallRecommendPageTable.eItemBelongMallType.ITEM_BELONG_MALL_TYPE_NORMAL_SHOP)
                        shopList.Add(item.mallItemID);
                    if (item.itemBelongMallType == (byte)MallRecommendPageTable.eItemBelongMallType.ITEM_BELONG_MALL_TYPE_ACCOUNT_SHOP)
                        accountShopList.Add(item.mallItemID);
                }
            }
            if (shopList.Count > 0)
                ReqShopQueryBatchItem(shopList.ToArray());
            else
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GetCommendShopItemSucc);
            if (mallList.Count > 0)
                ReqWorldMallQueryBatchItem(mallList.ToArray());
            else
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GetCommendMallItemSucc);
            if (accountShopList.Count > 0)
                ReqAccountShopQueryBatchItem(accountShopList.ToArray());
            else
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GetCommendAccountShopItemSucc);
        }

        // 购买账号商店中商品的返回 进行数据同步
        void _OnWorldAccountShopItemBuyRes(MsgDATA msg)
        {
            WorldAccountShopItemBuyRes msgData = new WorldAccountShopItemBuyRes();
            msgData.decode(msg.bytes);
            if (msgData.resCode != (uint)ProtoErrorCode.SUCCESS)
            {
                return;
            }
            for (int index = 0; index < mRecommendAccountShopItemList.Count; index++)
            {
                if (mRecommendAccountShopItemList[index].shopItemId == msgData.buyShopItemId)
                {
                    mRecommendAccountShopItemList[index].accountRestBuyNum = msgData.accountRestBuyNum;
                    mRecommendAccountShopItemList[index].roleRestBuyNum = msgData.roleRestBuyNum;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GetCommendAccountShopItemSucc);
                    return;
                }
            }
        }

        //购买商城中商品的返回 进行数据同步
        private void OnSyncWorldMallBuyRet(MsgDATA msg)
        {
            var worldMallBuyRet = new WorldMallBuyRet();
            worldMallBuyRet.decode(msg.bytes);
            if (worldMallBuyRet.code != (uint) ProtoErrorCode.SUCCESS)
            {
                //不处理 GlobalNetMessage已经处理
                // SystemNotifyManager.SystemNotify((int)worldMallBuyRet.code);
                return;
            }
            for (int index = 0; index < mRecommendMallItemList.Count; index++)
            {
                if (mRecommendMallItemList[index].id == worldMallBuyRet.mallitemid)
                {
                    mRecommendMallItemList[index].accountRestBuyNum = (uint)worldMallBuyRet.accountRestBuyNum;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GetCommendShopItemSucc);
                    return;
                }
            }
        }
        
        //商店批量查询商品请求 用于商城推荐
        public void ReqShopQueryBatchItem(uint[] itemArr)
        {
            var req = new SceneShopQueryBatchItemReq();
            req.shopItemIds = itemArr;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        //商店批量查询商品请求返回
        private void OnSyncSceneShopQueryBatchItemRes(MsgDATA msg)
        {
            var res = new SceneShopQueryBatchItemRes();
            res.decode(msg.bytes);
            for (int index = mRecommendList.Count - 1; index >= 0; --index)
            {
                var item = mRecommendList[index];
                if (item.linkFunctionType == (byte)MallRecommendPageTable.eLinkFunctionType.LINK_FUNCTION_TYPE_BUY || 
                    item.recommendType == (byte)MallRecommendPageTable.eRecommendType.RECOMMEND_TYPE_ITEM)
                {
                    if (item.itemBelongMallType == (byte)MallRecommendPageTable.eItemBelongMallType.ITEM_BELONG_MALL_TYPE_NORMAL_SHOP)
                    {
                        bool isNeed = false;
                        foreach (var info in res.shopItemList)
                        {
                            if (info.shopItemId == item.mallItemID)
                            {
                                isNeed = true;
                                break;
                            }
                        }
                        //如果商品信息不下发 则删除这条推荐
                        if (!isNeed)
                            mRecommendList.Remove(item);
                    }
                }
            }
            //获取数据 发送信息
            mRecommendShopItemList = new List<ProtoShopItem>(res.shopItemList);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GetCommendShopItemSucc);
        }

        //账号商店批量查询商品请求 用于商城推荐
        public void ReqAccountShopQueryBatchItem(uint[] itemArr)
        {
            var req = new WorldAccountShopQueryBatchItemReq();
            req.shopItemIds = itemArr;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        //账号商店批量查询商品请求返回
        private void OnSyncWorldAccountShopQueryBatchItemRes(MsgDATA msg)
        {
            var res = new WorldAccountShopQueryBatchItemRes();
            res.decode(msg.bytes);
            for (int index = mRecommendList.Count - 1; index >= 0; --index)
            {
                var item = mRecommendList[index];
                if (item.linkFunctionType == (byte)MallRecommendPageTable.eLinkFunctionType.LINK_FUNCTION_TYPE_BUY || 
                    item.recommendType == (byte)MallRecommendPageTable.eRecommendType.RECOMMEND_TYPE_ITEM)
                {
                    if (item.itemBelongMallType == (byte)MallRecommendPageTable.eItemBelongMallType.ITEM_BELONG_MALL_TYPE_ACCOUNT_SHOP)
                    {
                        bool isNeed = false;
                        foreach (var info in res.shopItemList)
                        {
                            if (info.shopItemId == item.mallItemID)
                            {
                                isNeed = true;
                                break;
                            }
                        }
                        //如果商品信息不下发 则删除这条推荐
                        if (!isNeed)
                            mRecommendList.Remove(item);
                    }
                }
            }
            //获取数据 发送信息
            mRecommendAccountShopItemList = new List<AccountShopItemInfo>(res.shopItemList);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GetCommendAccountShopItemSucc);
        }

        //商城批量查询商品请求 用于商城推荐
        public void ReqWorldMallQueryBatchItem(uint[] itemArr)
        {
            var req = new WorldMallQueryBatchItemReq();
            req.mallItemIds = itemArr;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        //商城批量查询商品请求返回
        private void OnSyncWorldMallQueryBatchItemRes(MsgDATA msg)
        {
            var res = new WorldMallQueryBatchItemRes();
            res.decode(msg.bytes);
            for (int index = mRecommendList.Count - 1; index >= 0; --index)
            {
                var item = mRecommendList[index];
                if (item.linkFunctionType == (byte)MallRecommendPageTable.eLinkFunctionType.LINK_FUNCTION_TYPE_BUY || 
                    item.recommendType == (byte)MallRecommendPageTable.eRecommendType.RECOMMEND_TYPE_ITEM)
                {
                    if (item.itemBelongMallType == (byte)MallRecommendPageTable.eItemBelongMallType.ITEM_BELONG_MALL_TYPE_MALL)
                    {
                        bool isNeed = false;
                        foreach (var info in res.mallItemInfos)
                        {
                            if (info.id == item.mallItemID)
                            {
                                isNeed = true;
                                break;
                            }
                        }
                        //如果商品信息不下发 则删除这条推荐
                        if (!isNeed)
                            mRecommendList.Remove(item);
                    }
                }
            }
            //获取数据 发送信息
            mRecommendMallItemList = new List<MallItemInfo>(res.mallItemInfos);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GetCommendMallItemSucc);
        }
        #endregion

        #region Update
        public override void Update(float time)
        {
            timeInterval += time;

            //达到1s
            if (timeInterval >= 1.0f)
            {
                timeInterval = 0.0f;
                if (_shopNewShopDataList != null && _shopNewShopDataList.Count > 0)
                {
                    for (var i = 0; i < _shopNewShopDataList.Count; i++)
                    {
                        var shopNewShopData = _shopNewShopDataList[i];
                        if(shopNewShopData == null)
                            continue;

                        if (shopNewShopData.ResetRefreshTime <= 0)
                        {
                            shopNewShopData.ResetRefreshTime = 0;
                        }
                        else
                        {
                            shopNewShopData.ResetRefreshTime -= 1;
                        }

                        if (shopNewShopData.WeekResetRefreshTime <= 0)
                        {
                            shopNewShopData.WeekResetRefreshTime = 0;
                        }
                        else
                        {
                            shopNewShopData.WeekResetRefreshTime -= 1;
                        }

                        if (shopNewShopData.MonthRefreshTime <= 0)
                        {
                            shopNewShopData.MonthRefreshTime = 0;
                        }
                        else
                        {
                            shopNewShopData.MonthRefreshTime -= 1;
                        }
                    }
                }
            }
        }
        #endregion

        #region Help

        public void JumpToShopById(int shopId)
        {
            var shopTable = TableManager.GetInstance().GetTableItem<ShopTable>(shopId);
            if (shopTable != null)
            {
                //小于解锁等级
                if (PlayerBaseData.GetInstance().Level < shopTable.OpenLevel)
                {
                    var exchangeNotOpenTip = string.Format(TR.Value("exchange_mall_not_open"), shopTable.OpenLevel
                        , shopTable.ShopName);
                    SystemNotifyManager.SysNotifyFloatingEffect(exchangeNotOpenTip);
                }
                else
                {
                    //单独商店
                    OpenShopNewFrame(shopId);
                }
            }
            else
            {
                Logger.LogErrorFormat("ShopTable is null and shop id is {0}", shopId);
            }
        }

        public bool IsShopNewFrameByShopId(int shopId)
        {
            if (_shopNewShopIdList == null || _shopNewShopIdList.Length <= 0)
                return false;

            //根据商店的ID，判断是否使用新的商店框架
            for (var i = 0; i < _shopNewShopIdList.Length; i++)
            {
                if (_shopNewShopIdList[i] == shopId)
                    return true;
            }

            return false;
        }

        public bool IsShowOldChangeNew(ShopNewShopItemInfo shopNewShopItemTable)
        {
            //只有7：积分商店，9：深渊商店的时候才可能显示
            if (shopNewShopItemTable.ShopId != 7 && shopNewShopItemTable.ShopId != 9)
                return false;

            //只有装备，武器，护甲才可能存在
            var shopItemTableSubType = (int)shopNewShopItemTable.ShopItemTable.SubType;
            if (shopItemTableSubType != (int)ShopTable.eSubType.ST_EQUIP
                && shopItemTableSubType != (int)ShopTable.eSubType.ST_ARMOR
                && shopItemTableSubType != (int)ShopTable.eSubType.ST_WEAPON)
                return false;

            if (string.IsNullOrEmpty(shopNewShopItemTable.ShopItemTable.OldChangeNewItemID))
                return false;

            return true;
        }

        public List<ulong> GetPackageOldChangeNewEquip(int id)
        {
            return ShopDataManager.GetInstance().GetPackageOldChangeNewEquip(id);
        }

        public bool IsPackageHaveExchangeEquipment(int itemId, EPackageType type, ref ItemData oldChangeNewItemData)
        {
            return ShopDataManager.GetInstance()._IsPackHaveExchangeEquipment(itemId, type, ref oldChangeNewItemData);
        }

        public void GetOldChangeNewItem(ShopItemTable shopItemTable, List<AwardItemData> oldChangeNewItemList)
        {
            ShopDataManager.GetInstance()._GetOldChangeNewItem(shopItemTable, oldChangeNewItemList);
        }



        public bool IsMoneyItemShowName(int moneyItemId)
        {
            if (moneyItemShowName == null || moneyItemShowName.Length <= 0)
                return false;

            for (var i = 0; i < moneyItemShowName.Length; i++)
            {
                if (moneyItemId == moneyItemShowName[i])
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 客户端尝试刷新帐号商店道具  
        /// </summary>
        /// <param name="accShopItemInfos"></param>
        /// <param name="firstFilterData"></param>
        /// <param name="secondFilterData"></param>
        public AccountShopItemInfo[] TryFilterAccountShopItemInfos(AccountShopItemInfo[] accShopItemInfos, ShopNewFilterData firstFilterData = null,  ShopNewFilterData secondFilterData = null)
        {
            if(accShopItemInfos == null)
            {
                return null;
            }
            List<AccountShopItemInfo> newAccShopItemInfos = new List<AccountShopItemInfo>();

            for (int i = 0; i < accShopItemInfos.Length; i++)
            {
                if(accShopItemInfos[i] == null)
                    continue;
                newAccShopItemInfos.Add(accShopItemInfos[i]);
            }
            
            if (newAccShopItemInfos == null || newAccShopItemInfos.Count <= 0)
                return null;

            var itemsCount = newAccShopItemInfos.Count;

            for (var i = itemsCount - 1; i >= 0; i--)
            {
                var shopItemInfo = newAccShopItemInfos[i];
                var itemId = (int)shopItemInfo.itemDataId;
                var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(itemId);
                if (itemTable == null)
                {
                    newAccShopItemInfos.RemoveAt(i);
                    continue;
                }
                //是否展示商品
                var isShow = true;
                isShow = IsItemNeedShow(itemTable, firstFilterData);
                if (isShow == true)
                {
                    isShow = IsItemNeedShow(itemTable, secondFilterData);
                }
                if (isShow == false)
                {
                    newAccShopItemInfos.RemoveAt(i);
                }
            }
            return newAccShopItemInfos.ToArray();
        }

        //获取对应区域的推荐页内容
        public List<MallRecommendPageInfo> GetRecommendPageInfoList(MallRecommendPageTable.eRecommendType type)
        {
            var list = new List<MallRecommendPageInfo>();
            foreach (var info in mRecommendList)
            {
                if (info.recommendType == (byte)type)
                    list.Add(info);
            }
            return list;
        }

        //推荐页打开购买界面
        public void OpenBuyFrame(MallRecommendPageInfo info)
        {
            if (null != info)
            {
                //商城
                if (info.itemBelongMallType == (byte)MallRecommendPageTable.eItemBelongMallType.ITEM_BELONG_MALL_TYPE_MALL)
                    ClientSystemManager.GetInstance().OpenFrame<RecommendBuyFrame>(FrameLayer.Middle, (MallItemInfo)GetRecommendItemInfo(info));
                    //ClientSystemManager.GetInstance().OpenFrame<MallBuyFrame>(FrameLayer.Middle, (MallItemInfo)GetRecommendItemInfo(info));
                //普通商店
                if (info.itemBelongMallType == (byte)MallRecommendPageTable.eItemBelongMallType.ITEM_BELONG_MALL_TYPE_NORMAL_SHOP)
                {
                    var ProtoShopItem = (ProtoShopItem)GetRecommendItemInfo(info);
                    var shopTable = TableManager.GetInstance().GetTableItem<ShopItemTable>((int)ProtoShopItem.shopItemId);
                    if (null != shopTable)
                        ClientSystemManager.GetInstance().OpenFrame<RecommendBuyFrame>(FrameLayer.Middle, new ShopNewShopItemInfo(shopTable));
                        //ClientSystemManager.GetInstance().OpenFrame<ShopNewPurchaseItemFrame>(FrameLayer.Middle, new ShopNewShopItemInfo(shopTable));
                }
                //账号商店
                if (info.itemBelongMallType == (byte)MallRecommendPageTable.eItemBelongMallType.ITEM_BELONG_MALL_TYPE_ACCOUNT_SHOP)
                        ClientSystemManager.GetInstance().OpenFrame<RecommendBuyFrame>(FrameLayer.Middle, (AccountShopItemInfo)GetRecommendItemInfo(info));
                    // ClientSystemManager.GetInstance().OpenFrame<AccountShopPurchaseItemFrame>(FrameLayer.Middle, 
                    //                                         new AccountShopPurchaseItemInfo((AccountShopItemInfo)GetRecommendItemInfo(info)));

            }
        }

        //获取到推荐页信息对应的商品
        public System.Object GetRecommendItemInfo(MallRecommendPageInfo info)
        {
            if (null ==  info)
                return null;
            //商城
            if (info.itemBelongMallType == (byte)MallRecommendPageTable.eItemBelongMallType.ITEM_BELONG_MALL_TYPE_MALL)
            {
                foreach (var item in mRecommendMallItemList)
                {
                    if (item.id == info.mallItemID)
                        return item;
                }
            }
            //普通商店
            if (info.itemBelongMallType == (byte)MallRecommendPageTable.eItemBelongMallType.ITEM_BELONG_MALL_TYPE_NORMAL_SHOP)
            {
                foreach (var item in mRecommendShopItemList)
                {
                    if (item.shopItemId == info.mallItemID)
                        return item;
                }
            }
            //账号商店
            if (info.itemBelongMallType == (byte)MallRecommendPageTable.eItemBelongMallType.ITEM_BELONG_MALL_TYPE_ACCOUNT_SHOP)
            {
                foreach (var item in mRecommendAccountShopItemList)
                {
                    if (item.shopItemId == info.mallItemID)
                        return item;
                }
            }
            return null;
        }
        #endregion

    }
}
