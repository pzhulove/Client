using System;
using Protocol;
using Network;
using ProtoTable;
using System.Collections.Generic;


namespace GameClient
{
    public class AccountShopData
    {
        /// <summary>
		///  查询索引
		/// </summary>
		public AccountShopQueryIndex queryIndex = new AccountShopQueryIndex();
        /// <summary>
        ///  商品集
        /// </summary>
        public AccountShopItemInfo[] shopItems = new AccountShopItemInfo[0];
        /// <summary>
        ///  下一波商品上架时间
        /// </summary>
        public UInt32 nextGroupOnSaleTime;
    }
    public class AccountShopDataManager : DataManager<AccountShopDataManager>
    {
        //帐号商店
        //50 佣兵商店
        //51 佣兵商店勋章子商店
        //52 佣兵商店赏金子商店
        public const int ADVENTURE_TEAM_SHOP_ID = 50;
        public const int ADVENTURE_TEAM_BLESS_CRYSTAL_CHILD_SHOP_ID = 51;
        public const int ADVENTURE_TEAM_BOUNTY_CHILD_SHOP_ID = 52;


        bool m_bNetBind = false;
        //所有账号商店商品的总数据
        private List<AccountShopData> AllAccountShopDataList = new List<AccountShopData>();
        private Dictionary<int, int> SpecialItemNumDic = new Dictionary<int, int>();

        //账号相关的计数
        private Dictionary<int, ulong> myAccountCountDic = new Dictionary<int, ulong>();

        #region Temp
        private Dictionary<int, List<int>> _tempShopTableFilterDic = new Dictionary<int, List<int>>();
        #endregion

        public sealed override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.Default;
        }

        public sealed override void Initialize()
        {
            Clear();
            AllAccountShopDataList.Clear();
            SpecialItemNumDic.Clear();
            _BindNetMsg();
        }

        public sealed override void Clear()
        {
            AllAccountShopDataList.Clear();
            SpecialItemNumDic.Clear();
            _UnBindNetMsg();
            m_bNetBind = false;

            _ClearTempFilterDic();
        }

        void _BindNetMsg()
        {
            if (m_bNetBind == false)
            {
                NetProcess.AddMsgHandler(WorldAccountShopItemQueryRes.MsgID, _OnWorldAccountShopItemQueryRes);
                NetProcess.AddMsgHandler(WorldAccountShopItemBuyRes.MsgID, _OnWorldAccountShopItemBuyRes);
                NetProcess.AddMsgHandler(WorldPlayerMallBuyGotInfoSync.MsgID, _OnWorldMallBuyGotInfoSync);
                NetProcess.AddMsgHandler(WorldAccountCounterNotify.MsgID, _OnWorldAccountCounterNotify);
                m_bNetBind = true;
            }
        }

        void _UnBindNetMsg()
        {
            NetProcess.RemoveMsgHandler(WorldAccountShopItemQueryRes.MsgID, _OnWorldAccountShopItemQueryRes);
            NetProcess.RemoveMsgHandler(WorldAccountShopItemBuyRes.MsgID, _OnWorldAccountShopItemBuyRes);
            NetProcess.RemoveMsgHandler(WorldPlayerMallBuyGotInfoSync.MsgID, _OnWorldMallBuyGotInfoSync);
            NetProcess.RemoveMsgHandler(WorldAccountCounterNotify.MsgID, _OnWorldAccountCounterNotify);
        }

        #region PRIVATE METHODS

        private void _SetShopFilterDataListTemp(ShopTable shopNewTable)
        {
            if (shopNewTable == null)
            {
                return;
            }

            if(!CheckIsShopTableFormatRight(shopNewTable))
            {                
                return;
            }

            _ClearTempFilterDic();

            var filterCount = 0;
            filterCount = (shopNewTable.Filter.Count > shopNewTable.Filter2.Count)
                ? shopNewTable.Filter.Count
                : shopNewTable.Filter2.Count;

            List<int> firstFilterList = null;
            List<int> secondFilterList = null;
            for (var i = 0; i < filterCount; i++)
            {
                if (_tempShopTableFilterDic.TryGetValue((int)AccountShopFilterType.First, out firstFilterList))
                {
                    if (firstFilterList == null)
                    {
                        firstFilterList = new List<int>();
                    }
                    if (i >= shopNewTable.Filter.Count)
                    {
                        firstFilterList.Add(0);
                    }
                    else
                    {
                        firstFilterList.Add((int)shopNewTable.Filter[i]);
                    }
                }
                else
                {
                    if (firstFilterList == null)
                    {
                        firstFilterList = new List<int>();
                    }
                    if (i >= shopNewTable.Filter.Count)
                    {
                        firstFilterList.Add(0);
                    }
                    else
                    {
                        firstFilterList.Add((int)shopNewTable.Filter[i]);
                    }
                    _tempShopTableFilterDic.Add((int)AccountShopFilterType.First, firstFilterList);
                }

                if (_tempShopTableFilterDic.TryGetValue((int)AccountShopFilterType.Second, out secondFilterList))
                {
                    if (secondFilterList == null)
                    {
                        secondFilterList = new List<int>();
                    }
                    if (i >= shopNewTable.Filter2.Count)
                    {
                        secondFilterList.Add(0);
                    }
                    else
                    {
                        secondFilterList.Add((int)shopNewTable.Filter2[i]);
                    }
                }
                else
                {
                    if (secondFilterList == null)
                    {
                        secondFilterList = new List<int>();
                    }
                    if (i >= shopNewTable.Filter2.Count)
                    {
                        secondFilterList.Add(0);
                    }
                    else
                    {
                        secondFilterList.Add((int)shopNewTable.Filter2[i]);
                    }
                    _tempShopTableFilterDic.Add((int)AccountShopFilterType.Second, secondFilterList);
                }
            }
        }

        private void _ClearTempFilterDic()
        {
            if(_tempShopTableFilterDic == null)
                return;
            var tempFilterDicEnum = _tempShopTableFilterDic.GetEnumerator();
            while (tempFilterDicEnum.MoveNext())
            {
                var filterList = tempFilterDicEnum.Current.Value;
                if (filterList != null)
                {
                    filterList.Clear();
                }
            }
           _tempShopTableFilterDic.Clear();
        }

        #endregion

        #region 协议
        /// <summary>
        /// 请求账号商店总数据
        /// </summary>
        /// <param name="item"></param>
        public void SendWorldAccountShopItemQueryReq(AccountShopQueryIndex item)
        {
            WorldAccountShopItemQueryReq req = new WorldAccountShopItemQueryReq();
            req.queryIndex = item;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }
        public void SendWorldAccountShopItemBuyReq(AccountShopQueryIndex item, uint id, uint num)
        {
            WorldAccountShopItemBuyReq req = new WorldAccountShopItemBuyReq();
            req.queryIndex = item;
            req.buyShopItemId = id;
            req.buyShopItemNum = num;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }
        /// <summary>
        /// 请求账号商店的协议
        /// </summary>
        /// <param name="msg"></param>
        void _OnWorldAccountShopItemQueryRes(MsgDATA msg)
        {
            WorldAccountShopItemQueryRes msgData = new WorldAccountShopItemQueryRes();
            msgData.decode(msg.bytes);
            if (msgData.resCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.resCode);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.AccountShopReqFailed);
                return;
            }
            for (int i = 0; i < AllAccountShopDataList.Count; i++)
            {
                if (msgData.queryIndex.shopId == AllAccountShopDataList[i].queryIndex.shopId &&
                    msgData.queryIndex.tabType == AllAccountShopDataList[i].queryIndex.tabType &&
                    msgData.queryIndex.jobType == AllAccountShopDataList[i].queryIndex.jobType)
                {
                    AllAccountShopDataList.Remove(AllAccountShopDataList[i]);
                }
            }

            AccountShopData accountShopData = new AccountShopData();
            accountShopData.queryIndex.shopId = msgData.queryIndex.shopId;
            accountShopData.queryIndex.tabType = msgData.queryIndex.tabType;
            accountShopData.queryIndex.jobType = msgData.queryIndex.jobType;
            accountShopData.shopItems = new AccountShopItemInfo[msgData.shopItems.Length];
            for (int i = 0; i < msgData.shopItems.Length; i++)
            {
                accountShopData.shopItems[i] = msgData.shopItems[i];
            }
            accountShopData.nextGroupOnSaleTime = msgData.nextGroupOnSaleTime;
            AllAccountShopDataList.Add(accountShopData);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.AccountShopUpdate, msgData.queryIndex);
        }

        /// <summary>
        /// 购买账号商店中商品的返回
        /// </summary>
        /// <param name="msg"></param>
        void _OnWorldAccountShopItemBuyRes(MsgDATA msg)
        {
            WorldAccountShopItemBuyRes msgData = new WorldAccountShopItemBuyRes();
            msgData.decode(msg.bytes);
            if (msgData.resCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.resCode);
                return;
            }
            for (int i = 0; i < AllAccountShopDataList.Count; i++)
            {
                if (msgData.queryIndex.shopId == AllAccountShopDataList[i].queryIndex.shopId &&
                    msgData.queryIndex.tabType == AllAccountShopDataList[i].queryIndex.tabType &&
                    msgData.queryIndex.jobType == AllAccountShopDataList[i].queryIndex.jobType)
                {
                    for (int j = 0; j < AllAccountShopDataList[i].shopItems.Length; j++)
                    {
                        if (AllAccountShopDataList[i].shopItems[j].shopItemId == msgData.buyShopItemId)
                        {
                            AllAccountShopDataList[i].shopItems[j].accountRestBuyNum = msgData.accountRestBuyNum;
                            AllAccountShopDataList[i].shopItems[j].roleRestBuyNum = msgData.roleRestBuyNum;
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.AccountShopItemUpdata, msgData.queryIndex, msgData.buyShopItemId, msgData.accountRestBuyNum, msgData.roleRestBuyNum);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 上线和更新的时候服务器同步特殊道具的信息。
        /// </summary>
        /// <param name="msg"></param>
        void _OnWorldMallBuyGotInfoSync(MsgDATA msg)
        {
            WorldPlayerMallBuyGotInfoSync msgData = new WorldPlayerMallBuyGotInfoSync();
            msgData.decode(msg.bytes);
            SpecialItemNumDic[(int)msgData.mallBuyGotInfo.itemDataId] = (int)msgData.mallBuyGotInfo.buyGotNum;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SpeicialItemUpdate, (int)msgData.mallBuyGotInfo.itemDataId);
        }

        /// <summary>
        /// 上线和更新服务器同步的账号用特殊道具和数量
        /// </summary>
        /// <param name="msg"></param>
        void _OnWorldAccountCounterNotify(MsgDATA msg)
        {
            WorldAccountCounterNotify msgData = new WorldAccountCounterNotify();
            msgData.decode(msg.bytes);
            for(int i = 0;i<msgData.accCounterList.Length;i++)
            {
                myAccountCountDic[(int)msgData.accCounterList[i].type] = msgData.accCounterList[i].num;
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.AccountSpecialItemUpdate);
        }
        #endregion

        #region 共有函数
        //通过key值获得共有商店数据
        public AccountShopItemInfo[] GetAccountShopData(AccountShopQueryIndex item)
        {
            for (int i = 0; i < AllAccountShopDataList.Count; i++)
            {
                if (item.shopId == AllAccountShopDataList[i].queryIndex.shopId &&
                    item.tabType == AllAccountShopDataList[i].queryIndex.tabType &&
                    item.jobType == AllAccountShopDataList[i].queryIndex.jobType)
                {
                    return AllAccountShopDataList[i].shopItems;
                }
            }
            return null;
        }

        public int GetShopNextTime(AccountShopQueryIndex item)
        {
            for (int i = 0; i < AllAccountShopDataList.Count; i++)
            {
                if (item.shopId == AllAccountShopDataList[i].queryIndex.shopId &&
                    item.tabType == AllAccountShopDataList[i].queryIndex.tabType &&
                    item.jobType == AllAccountShopDataList[i].queryIndex.jobType)
                {
                    return (int)AllAccountShopDataList[i].nextGroupOnSaleTime;
                }
            }
            return 0;
        }

        public int GetSpecialItemNum(int id)
        {
            if (SpecialItemNumDic.ContainsKey(id))
            {
                return SpecialItemNumDic[id];
            }
            else
            {
                return 0;
            }
        }

        public ulong GetAccountSpecialItemNum(AccountCounterType type)
        {
            if(myAccountCountDic.ContainsKey((int)type))
            {
                return myAccountCountDic[(int)type];
            }
            else
            {
                return 0;
            }
        }

        #region EXTRA PUBLIC METHODS

        /// <summary>
        /// 打开帐号商店
        /// </summary>
        /// <param name="shopId">帐号商店id 从商店表中获取</param>
        /// <param name="shopChildrenId">子商店id 商店表中Children列</param>
        /// <param name="shopItemType">商店道具类型 商店表中SubType列</param>
        /// <param name="npcId">商店NPC ID</param>
        public void OpenAccountShop(int shopId, int shopChildrenId = 0, int shopItemType = 0, int npcId = -1, int shopItemId = 0)
        {
            ShopNewParamData shopNewParamData = new ShopNewParamData()
            {
                ShopId = shopId,
                ShopChildId = shopChildrenId,
                ShopItemType = shopItemType,
                NpcId = npcId,
                shopItemId = shopItemId
            };

            OpenAccountShop(shopNewParamData);
        }

        public void OpenAccountShop(ShopNewParamData shopNewParam)
        {
            if (shopNewParam == null)
            {
                return;
            }

            var shopNewTable = TableManager.GetInstance().GetTableItem<ShopTable>(shopNewParam.ShopId);
            if (shopNewTable == null)
            {
                Logger.LogErrorFormat("[AccountShopDataManager] - OpenAccountShop failed, shopNewTable is null and shop id is {0}", shopNewParam.ShopId);
                return;
            }

            if (shopNewTable.BindType != ShopTable.eBindType.ACCOUNT_BIND)
            {
                Logger.LogErrorFormat("[AccountShopDataManager] - OpenAccountShop failed, shop {0}, bind type is not account shop !", shopNewParam.ShopId);
                return;
            }

            //设置过滤器缓存
            _SetShopFilterDataListTemp(shopNewTable);

            if (ClientSystemManager.GetInstance().IsFrameOpen<AccountShopFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<AccountShopFrame>();
            }
            ClientSystemManager.GetInstance().OpenFrame<AccountShopFrame>(FrameLayer.Middle, shopNewParam);
        }

        
        /// <summary>
        /// 获取商店限购类型
        /// </summary>
        /// <param name="itemInfo"></param>
        /// <returns></returns>
        public AccountShopPurchaseType GetAccShopItemBuyLimitType(AccountShopItemInfo itemInfo)
        {
            AccountShopPurchaseType buyLimitType = AccountShopPurchaseType.NotLimit;
            if (itemInfo == null)
            {
                return buyLimitType;
            }

            if (itemInfo.accountLimitBuyNum == 0 && itemInfo.roleLimitBuyNum == 0)
            {
                //不限购
                buyLimitType = AccountShopPurchaseType.NotLimit;
            }
            else if (itemInfo.accountLimitBuyNum > 0 && itemInfo.roleLimitBuyNum == 0)
            {
                //帐号不限购
                buyLimitType = AccountShopPurchaseType.AccountBind;
            }
            else if (itemInfo.accountLimitBuyNum == 0 && itemInfo.roleLimitBuyNum > 0)
            {
                //角色不限购
                buyLimitType = AccountShopPurchaseType.RoleBind;
            }
            else if (itemInfo.accountLimitBuyNum > 0 && itemInfo.roleLimitBuyNum > 0)
            {
                //帐号和角色都需要有可购买次数
                buyLimitType = AccountShopPurchaseType.AccountBind | AccountShopPurchaseType.RoleBind;
            }

            return buyLimitType;
        }

        public int GetAccountShopItemCanBuyNum(AccountShopItemInfo _accShopItemInfo)
        {
            int limitBuyTimes = 0;
            if (_accShopItemInfo == null)
            {
                return limitBuyTimes;
            }

            if (_accShopItemInfo.accountLimitBuyNum > 0 && _accShopItemInfo.roleLimitBuyNum == 0)
            {
                limitBuyTimes = (int)_accShopItemInfo.accountRestBuyNum;
            }
            else if (_accShopItemInfo.accountLimitBuyNum == 0 && _accShopItemInfo.roleLimitBuyNum > 0)
            {
                limitBuyTimes = (int)_accShopItemInfo.roleRestBuyNum;
            }
            else if (_accShopItemInfo.accountLimitBuyNum > 0 && _accShopItemInfo.roleLimitBuyNum > 0)
            {
                if (_accShopItemInfo.accountRestBuyNum > 0 && _accShopItemInfo.roleRestBuyNum > 0)
                {
                    int _accountRestBuyNumTemp = (int)_accShopItemInfo.accountRestBuyNum;
                    int _roleResetBuyNumTemp = (int)_accShopItemInfo.roleRestBuyNum;
                    limitBuyTimes = _accountRestBuyNumTemp <= _roleResetBuyNumTemp ? _accountRestBuyNumTemp : _roleResetBuyNumTemp;
                }
            }

            return limitBuyTimes;
        }

        public int GetAccountShopRefreshTime(AccountShopItemInfo[] oneTabShopItemInfos, AccountShopPurchaseType defaultBindType = AccountShopPurchaseType.AccountBind, bool bReverse = false)
        {   
            if(oneTabShopItemInfos == null)
            {
                return 0;
            }

            List<int> tempRefreshTimes = new List<int>();
            int tempRefreshTime = 0;

            for (int i = 0; i < oneTabShopItemInfos.Length; i++)
            {
                var itemInfo = oneTabShopItemInfos[i];
                if(itemInfo == null)
                    continue;            

                if(itemInfo.accountRefreshType == 0 && itemInfo.roleRefreshType == 0)
                {
                    continue;
                }
                else if(itemInfo.roleRefreshType > 0)
                {
                    tempRefreshTime = (int)itemInfo.roleBuyRecordNextRefreshTimestamp;
                    if(defaultBindType == AccountShopPurchaseType.RoleBind && tempRefreshTime > 0)
                    {
                        tempRefreshTimes.Add(tempRefreshTime);
                    }
                }
                else if(itemInfo.accountRefreshType > 0)
                {
                    tempRefreshTime = (int)itemInfo.accountBuyRecordNextRefreshTimestamp;
                    if(defaultBindType == AccountShopPurchaseType.AccountBind && tempRefreshTime > 0)
                    {
                        tempRefreshTimes.Add(tempRefreshTime);
                    }
                }
            }

            if(bReverse)
            {
                tempRefreshTimes.Sort((x,y) => -x.CompareTo(y));
            }
            else
            {
                //升序
                tempRefreshTimes.Sort((x,y) => x.CompareTo(y));
            }

            if(tempRefreshTimes != null && tempRefreshTimes.Count > 0)
            {
                int currServerTime = (int)TimeManager.GetInstance().GetServerTime();
                int latestTime = tempRefreshTimes[0];

                if(latestTime > currServerTime)
                {
                    return latestTime - currServerTime;
                }
            }

            return 0;
        }

        public AccountShopQueryIndex GetAccountShopQueryIndex(ShopNewFrameViewData shopViewData)
        {
            AccountShopQueryIndex queryIndex = new AccountShopQueryIndex();
            if(shopViewData == null)
            {
                return queryIndex;
            }

            var shopNewTable = TableManager.GetInstance().GetTableItem<ShopTable>(shopViewData.shopId);
            if (shopNewTable == null)
            {
                return queryIndex;
            }

            if (shopViewData.currentSelectedTabData != null)
            {
                //MainTab
                var selectedShopMainTabType = shopViewData.currentSelectedTabData.MainTabType;
                if (selectedShopMainTabType == ShopNewMainTabType.ShopType)
                {
                    int childShopId = shopViewData.currentSelectedTabData.Index;
                    queryIndex.shopId = (uint)childShopId;
                    var childShopTable = TableManager.GetInstance().GetTableItem<ShopTable>(childShopId);
                    if (childShopTable == null)
                    {
                        return queryIndex;
                    }
                    if (!CheckIsChildShopTable(childShopTable))
                    {
                        return queryIndex;
                    }
                    queryIndex.tabType = (byte)childShopTable.SubType[0];
                }
                else if(selectedShopMainTabType == ShopNewMainTabType.ItemType)
                {
                    queryIndex.shopId = (uint)shopViewData.shopId;
                    queryIndex.tabType = (byte)shopViewData.currentSelectedTabData.Index;
                }
            }

            //Filter
            if (shopViewData.currFirstFilterData != null)
            {
                queryIndex.jobType = (byte)shopViewData.currFirstFilterData.Index;
            }

            return queryIndex;
        }

        public void SendWorldAccountShopItemQueryReq(ShopNewFrameViewData shopViewData)
        {
            if (shopViewData == null)
            {
                return;
            }
            var queryIndex = GetAccountShopQueryIndex(shopViewData);
            SendWorldAccountShopItemQueryReq(queryIndex);
        }

        public AccountShopItemInfo[] GetAccountShopData(ShopNewFrameViewData shopViewData)
        {
            if (shopViewData == null)
            {
                return null;
            }
            var queryIndex = GetAccountShopQueryIndex(shopViewData);
            return GetAccountShopData(queryIndex);
        }

        /// <summary>
        /// 检查商店表 部分关注结构是否配置正确
        /// </summary>
        /// <param name="shopTable"></param>
        /// <returns></returns>
        public bool CheckIsShopTableFormatRight(ShopTable shopTable)
        {
            bool bRight = true;
            
            if (shopTable == null)
            {
                bRight = false;
            }

            if (shopTable.ChildrenLength != shopTable.SubTypeLength && shopTable.ChildrenLength > 1)
            {
                Logger.LogErrorFormat("[AccountShopDataManager] - CheckIsShopTableFormatRight : shop table error, children shop length > 1 but children length != subtype length, shopId is {0}", shopTable.ID.ToString());
                bRight = false;
            }

            if (shopTable.ChildrenLength != shopTable.FilterLength && shopTable.ChildrenLength > 1)
            {
                Logger.LogErrorFormat("[AccountShopDataManager] - CheckIsShopTableFormatRight : shop table error, children shop length > 1 but children length != filter length, shopId is {0}", shopTable.ID.ToString());
                bRight = false;
            }

            if (shopTable.ChildrenLength != shopTable.Filter2Length && shopTable.ChildrenLength > 1)
            {
                Logger.LogErrorFormat("[AccountShopDataManager] - CheckIsShopTableFormatRight : shop table error, children shop length > 1 but children length != filter2 length, shopId is {0}", shopTable.ID.ToString());
                bRight = false;
            }

            int filterCount = 0;
            filterCount = (shopTable.Filter.Count > shopTable.Filter2.Count) ? shopTable.Filter.Count : shopTable.Filter2.Count;

            if (filterCount != shopTable.SubTypeLength)
            {
                Logger.LogErrorFormat("[AccountShopDataManager] - CheckIsShopTableFormatRight : shop table error, filter1 or filter2 length != subtype length, shopId is {0}", shopTable.ID.ToString());
                bRight = false;
            }

            if (shopTable.SubTypeLength != shopTable.RefreshCycleLength && shopTable.Refresh == 2)
            {
                Logger.LogErrorFormat("[AccountShopDataManager] - CheckIsShopTableFormatRight : shop table error, refresh cycle  length != subtype length and refresh == 2, shopId is {0}", shopTable.ID.ToString());
                bRight = false;
            }

            return bRight;
        }

        /// <summary>
        /// 检查是否是子商店
        /// </summary>
        /// <param name="shopTable"></param>
        /// <returns></returns>
        public bool CheckIsChildShopTable(ShopTable shopTable)
        {
            bool bChildShop = true;

            if (shopTable == null)
            {
                bChildShop = false;
            }

            if(!CheckIsShopTableFormatRight(shopTable))
            {
                bChildShop = false;
            }

            if (shopTable.ChildrenLength != 1)
            {
                Logger.LogErrorFormat("[AccountShopDataManager] - CheckIsChildShopTable : shop table error, children length > 0, shopId is {0}", shopTable.ID.ToString());
                bChildShop = false;
            }

            return bChildShop;
        }

        public bool CheckHasChildShop(int shopId)
        {
            bool hasChildShop = false;
            var shopTable = TableManager.GetInstance().GetTableItem<ShopTable>(shopId);
            if (shopTable != null && shopTable.ChildrenLength > 1)
            {
                hasChildShop = true;
            }
            return hasChildShop;
        }

        /// <summary>
        /// 获取商店基础的消耗道具组ID
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public List<int> GetShopBaseMoneyIds(int shopId, int shopTabIndex = 0)
        {
            List<int> baseMoneyIds = new List<int>();
            var shopNewTable = TableManager.GetInstance().GetTableItem<ShopTable>(shopId);
            if (shopNewTable != null)
            {
                int baseId1 = 0;
                if(!string.IsNullOrEmpty(shopNewTable.ExtraShowMoneys) && int.TryParse(shopNewTable.ExtraShowMoneys, out baseId1))
                {
                    if(baseId1 > 0 && !baseMoneyIds.Contains(baseId1))
                    {
                        baseMoneyIds.Add(baseId1);
                    }
                }

                if(shopNewTable.CurrencyShowType > 0 && shopNewTable.CurrencyExtraItem != null)
                {
                    for (int i = 0; i < shopNewTable.CurrencyExtraItem.Count; i++)
                    {
                        string baseStrs = shopNewTable.CurrencyExtraItem[i];
                        if(string.IsNullOrEmpty(baseStrs))
                        {
                            continue;
                        }
                        string[] baseId2s = baseStrs.Split('_');
                        if(baseId2s == null)
                        {
                            continue;
                        }
                        for (int j = 0; j < baseId2s.Length; j++)
                        {
                            int baseId2 = 0;
                            if(!string.IsNullOrEmpty(baseId2s[j]) && int.TryParse(baseId2s[j], out baseId2))
                            {
                                if(baseId2 > 0 && !baseMoneyIds.Contains(baseId2))
                                {
                                    baseMoneyIds.Add(baseId2);
                                }
                            }
                        }
                    }
                }
            }
            return baseMoneyIds;
        }

        /// <summary>
        /// 获取商店额外的消耗道具组ID
        /// </summary>
        /// <param name="accShopItemInfos"></param>
        /// <returns></returns>
        public List<int> GetShopExtraMoneyIds(AccountShopItemInfo[] accShopItemInfos)
        {
            List<int> extraMoneyIds = new List<int>();

            if(accShopItemInfos == null)
            {
                return extraMoneyIds;
            }

            for (int i = 0; i < accShopItemInfos.Length; i++)
            {
                var info = accShopItemInfos[i];
                if(info == null)
                {
                    continue;
                }
                if(info.costItems == null || info.costItems.Length <= 0)
                {
                    continue;
                }
                for (int j = 0; j < info.costItems.Length; j++)
                {
                    var costItem = info.costItems[j];
                    if(costItem == null)
                    {
                        continue;
                    }
                    if(extraMoneyIds == null || extraMoneyIds.Contains((int)costItem.id))
                    {
                        continue;
                    }
                    extraMoneyIds.Add((int)costItem.id);
                }
            }

            return extraMoneyIds;
        }

        /// <summary>
        /// 获取商店额外的帮助ID 只获取一个
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public int GetShopHelpId(int shopId)
        {
            int helpId = 0;
            var shopNewTable = TableManager.GetInstance().GetTableItem<ShopTable>(shopId);
            if (shopNewTable != null)
            {
                helpId = shopNewTable.HelpID;
            }
            return helpId;
        }

        /// <summary>
        /// 获取商店名称 
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public string GetShopName(int shopId)
        {
            string shopName = "帐号商店";
            var shopNewTable = TableManager.GetInstance().GetTableItem<ShopTable>(shopId);
            if (shopNewTable != null)
            {
                shopName = shopNewTable.ShopName;
            }
            return shopName;
        }

        /// <summary>
        /// 获取商店刷新时间描述
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public string GetShopRefreshTimeDesc(int shopId)
        {
            string desc = "";
            var shopNewTable = TableManager.GetInstance().GetTableItem<ShopTable>(shopId);
            if (shopNewTable != null)
            {
                desc = shopNewTable.ShopItemRefreshDesc;
            }
            return desc;
        }

        /// <summary>
        /// 获取过滤器的当前选择序号对应的类型
        /// </summary>
        /// <param name="index">过滤器选择的序号</param>
        /// <returns></returns>
        public ShopTable.eFilter GetFilterDataType(AccountShopFilterType filterType, int index)
        {
            if (_tempShopTableFilterDic == null)
            {
                return ShopTable.eFilter.SF_NONE;
            }
            List<int> _tempFilterList = null;
            if (_tempShopTableFilterDic.TryGetValue((int)filterType, out _tempFilterList))
            {
                if (index < 0 || index >= _tempFilterList.Count)
                    return 0;

                return (ShopTable.eFilter)_tempFilterList[index];
            }
            return ShopTable.eFilter.SF_NONE;
        }

        public void RestoreFilterDataByIndex(ref ShopNewFilterData tempFilterData, ShopNewFilterData currSelectedFilterData, AccountShopFilterType filterType, int index)
        {
            if (tempFilterData == null)
            {
                return;
            }
            var filterDataType = GetFilterDataType(filterType, index);
            if ((int)filterDataType < (int)ShopTable.eFilter.SF_NONE || (int)filterDataType >= (int)ShopTable.eFilter.SF_COUNT)
            {
                filterDataType = ShopTable.eFilter.SF_NONE;
                tempFilterData = null;
            }
            else
            {
                if (currSelectedFilterData != null)
                {
                    tempFilterData = currSelectedFilterData;
                }
                else
                {
                    tempFilterData = ShopNewDataManager.GetInstance().GetDefaultFilterDataByFilterType(filterDataType);
                }
            }
        }

        /// <summary>
        /// 是否是佣兵商店
        /// </summary>
        /// <param name="_shopTable"></param>
        /// <returns></returns>
        public bool CheckIsAdventureTeamAccShop(ProtoTable.ShopTable _shopTable)
        {
            if(_shopTable == null)
            {
                return false;
            }
            if(_shopTable.BindType == ShopTable.eBindType.ACCOUNT_BIND &&
                _shopTable.ShopKind == ShopTable.eShopKind.SK_BlessCrystal)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断佣兵商店是否开放
        /// </summary>
        /// <param name="_shopTable"> 商店表 </param>
        /// <returns></returns>
        public bool CheckIsAdventureTeamAccShopOpen()
        {           
            bool bFuncOn = AdventureTeamDataManager.GetInstance().BFuncOpened;
            return bFuncOn;
        }

        #endregion

        #endregion
    }
}
