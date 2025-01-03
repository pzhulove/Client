using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Network;
using Protocol;
using GoodsType = ProtoTable.ShopTable.eSubType;
using ProtoTable;
using UnityEngine.UI;

namespace GameClient
{
    class GoodsData
    {
        public enum GoodsDataShowType
        {
            GDST_NORMAL = 0,
            GDST_VIP,
            GDST_LIMIT_COUNT,
            GDST_HIDE,
        }

        public GoodsDataShowType GetShowType(System.Int32 iVipLevel)
        {
            if (VipLimitLevel > 0)
            {
                //if(iVipLevel >= VipLimitLevel)
                //{
                //    return GoodsDataShowType.GDST_VIP;
                //}
                //return GoodsDataShowType.GDST_HIDE;
                return GoodsDataShowType.GDST_VIP;
            }
            else if (LimitBuyTimes >= 0)
            {
                return GoodsDataShowType.GDST_LIMIT_COUNT;
            }
            return GoodsDataShowType.GDST_NORMAL;
        }

        public int? ID;
        public ItemData ItemData;
        public GoodsType? Type;
        public ItemData CostItemData;
        public int? CostItemCount;
        public int? VipLimitLevel;
        public int? VipDiscount;
        public int? LimitBuyCount;
        public int LimitBuyTimes;
        public GoodsLimitButyType eGoodsLimitButyType;
        public int iLimitValue;
        public ProtoTable.ShopItemTable shopItem;
        public int TotalLimitBuyTimes;//神秘商点用的字段  限购总次数
        public int? GoodsDisCount;//神秘商点用的字段  商品折扣
        public int LeaseEndTimeStamp;//武器租赁时间戳

    }

    enum GoodsLimitButyType
    {
        GLBT_NONE = 0,
        GLBT_FIGHT_SCORE,
        GLBT_TOWER_LEVEL,
        GLBT_GUILD_LEVEL,
        GLBT_HONOR_LEVEL_LIMIT,     //4 荣誉登记限制
    }

    class ShopData
    {
        public int? ID;
        public string Name;
        public string ShopNamePath;
        public List<GoodsType> GoodsTypes = new List<GoodsType>();
        public int? NeedRefresh;
        public int? RefreshCost;
        public uint RefreshTime;
        public List<GoodsData> Goods = new List<GoodsData>();
        public int RefreshLeftTimes;
        public int RefreshTotalTimes;
        public uint WeekRefreshTime;
        public uint MonthRefreshTime;
        public int iLinkNpcId = -1;
    }

    public struct BuyGoodsResult
    {
        public BuyGoodsResult(int a_nGoodsID, int a_nLimitBuyCount)
        {
            goodsID = a_nGoodsID;
            limitBuyCount = a_nLimitBuyCount;
        }

        public int goodsID;
        public int limitBuyCount;
    }

    class ShopDataManager : DataManager<ShopDataManager>
    {
        List<ShopData> m_arrShopDatas = new List<ShopData>();

        List<ProtoTable.ShopItemTable> DuelWeaponsList = new List<ProtoTable.ShopItemTable>();
        Dictionary<int, List<ProtoTable.ShopItemTable>> DuelWeaponsDic = new Dictionary<int, List<ProtoTable.ShopItemTable>> ();//决斗武器

        List<JuedouchangItemPropellingTable> juedouchangItemPropelingList = new List<JuedouchangItemPropellingTable>();

        /// <summary>
        /// 装备初始化携带属性生成表数据 ulong表示表中ItemID 
        /// </summary>
        Dictionary<int, EquipInitialAttribute> mEquipInitialAttrbuteDic = new Dictionary<int, EquipInitialAttribute>();

        int mySteryMerchantTriggerNumber = 0;//用于神秘商人触发次数添加埋点
        /// <summary>
        /// 神秘商店ID
        /// </summary>
        public int[] mysteryShopIDs = new int[] {17, 18, 19 };

        int mysticalMerchantId = -1;
        /// <summary>
        /// 神秘商人ID
        /// </summary>
        public int MysticalMerchantID
        {
            get { return mysticalMerchantId; }
            set { mysticalMerchantId = value; }
        }

        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.ShopDataManager;
        }

        public override void Initialize()
        {
            NetProcess.AddMsgHandler(SceneShopSync.MsgID, _OnResetShopGoods);
            NetProcess.AddMsgHandler(SceneShopItemSync.MsgID, _OnSceneShopItemSync);
            NetProcess.AddMsgHandler(SceneSyncMysticalMerchant.MsgID, _OnSceneSyncMysticalMerchant);
            NetProcess.AddMsgHandler(WorldMallQuerySingleItemRes.MsgID, _OnWorldMallQuerySingleItemRes);

            _InitShopItemTableData();
            _InitJuedouchangItemPropellingTableData();
            InitEquipInitialAttributeTableData();
            m_arrShopDatas.Clear();
        }

        public override void Clear()
        {
            NetProcess.RemoveMsgHandler(SceneShopSync.MsgID, _OnResetShopGoods);
            NetProcess.RemoveMsgHandler(SceneShopItemSync.MsgID, _OnSceneShopItemSync);
            NetProcess.RemoveMsgHandler(SceneSyncMysticalMerchant.MsgID, _OnSceneSyncMysticalMerchant);
            NetProcess.RemoveMsgHandler(WorldMallQuerySingleItemRes.MsgID, _OnWorldMallQuerySingleItemRes);

            m_arrShopDatas.Clear();

            DuelWeaponsList.Clear();
            DuelWeaponsDic.Clear();
            juedouchangItemPropelingList.Clear();
            oldChangeNewItem.Clear();
            mysticalMerchantId = -1;
            mEquipInitialAttrbuteDic.Clear();
        }

        public delegate void OnOpenChildShopFrame(int iShopID,ShopFrame frame,int iId);
        public OnOpenChildShopFrame onOpenChildShopFrame;
        int m_iUniqueId = 0;
        Dictionary<int, int> m_akMainFrameIds = new Dictionary<int, int>();

        public int RegisterMainFrame()
        {
            int iRet = ++m_iUniqueId;
            m_akMainFrameIds.Add(iRet, iRet);
            return iRet;
        }

        //7代表魔罐积分商店  9代表深渊商店  判断是否显示以旧换新
        public bool _IsShowOldChangeNew(GoodsData goodsData)
        {
            if (goodsData.shopItem.ShopID != 7 && goodsData.shopItem.ShopID != 9)
            {
                return false;
            }

            if (goodsData.Type != ProtoTable.ShopTable.eSubType.ST_EQUIP && goodsData.Type != ProtoTable.ShopTable.eSubType.ST_ARMOR && goodsData.Type != ProtoTable.ShopTable.eSubType.ST_WEAPON)
            {
                return false;
            }

            var shopItemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ShopItemTable>(goodsData.shopItem.ID);
            if (shopItemTable == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(shopItemTable.OldChangeNewItemID))
            {
                return false;
            }

            return true;
        }
        //判断是否是灰态
        public bool _IsCanOldChangeNew(ItemData data, EPackageType type)
        {
            List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(type);
            if (itemGuids != null)
            {
                ItemData curItem = null;
                for (int i = 0; i < itemGuids.Count; i++)
                {
                    curItem = ItemDataManager.GetInstance().GetItem(itemGuids[i]);
                    if (curItem == null || curItem.TableID != data.TableID)
                    {
                        continue;
                    }

                    return true;
                }
            }
            else
            {
                return false;
            }

            return false;
        }

        //判断背包有没有可以兑换装备的道具
        public bool _IsPackHaveExchangeEquipment(int id, EPackageType type,ref ItemData oldChangeNewItemData)
        {
            var shopItemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ShopItemTable>(id);
            if (shopItemTable != null)
            {
                if (string.IsNullOrEmpty(shopItemTable.OldChangeNewItemID))
                {
                    if (type == EPackageType.Equip)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                   
                }

                _GetOldChangeNewItem(shopItemTable, oldChangeNewItem);
                
                for (int i = 0; i < oldChangeNewItem.Count; i++)
                {
                    oldChangeNewItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(oldChangeNewItem[i].ID);

                    List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(type);

                    int num = 0;

                    if (itemGuids != null)
                    {
                        ItemData curItem = null;
                        for (int j = 0; j < itemGuids.Count; j++)
                        {
                            curItem = ItemDataManager.GetInstance().GetItem(itemGuids[j]);
                            if (curItem == null || curItem.TableID != oldChangeNewItemData.TableID)
                            {
                                continue;
                            }

                            if (type == EPackageType.Equip)
                            {
                                num++;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }

                    if (num >= oldChangeNewItem[i].Num)
                    {
                        return true;
                    }

                }
                
            }

            return false;
        }

        public void _GetOldChangeNewItem(ShopItemTable shopItemTable,List<AwardItemData> oldChangeNewItem)
        {
            oldChangeNewItem.Clear();

            if (!string.IsNullOrEmpty(shopItemTable.OldChangeNewItemID))
            {
                var awards = shopItemTable.OldChangeNewItemID.Split(new char[] { ',' });
                for (int i = 0; i < awards.Length; ++i)
                {
                    if (!string.IsNullOrEmpty(awards[i]))
                    {
                        var substrings = awards[i].Split(new char[] { '_' });
                        if (substrings.Length == 2)
                        {
                            int iID = int.Parse(substrings[0]);
                            int iCount = int.Parse(substrings[1]);
                            var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(iID);
                            if (item == null || iCount <= 0)
                            {
                                continue;
                            }

                            oldChangeNewItem.Add(new AwardItemData
                            {
                                ID = iID,
                                Num = iCount,
                            });
                        }
                    }
                }
            }
        }

        public List<ulong> GetPackageOldChangeNewEquip(int id)
        {
            List<ulong> mPackageComsumeItemList = new List<ulong>();

            var shopItemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ShopItemTable>(id);
            if (shopItemTable != null)
            {
                _GetOldChangeNewItem(shopItemTable, oldChangeNewItem);

                for (int i = 0; i < oldChangeNewItem.Count; i++)
                {
                   ItemData oldChangeNewItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(oldChangeNewItem[i].ID);

                   List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Equip);

                   if (itemGuids != null)
                   {
                        for (int j = 0; j < itemGuids.Count; j++)
                        {
                           ulong uId = itemGuids[j];
                           ItemData data = ItemDataManager.GetInstance().GetItem(uId);
                            if (data == null) continue;
                            if (data.TableID != oldChangeNewItemData.TableID)
                            {
                                continue;
                            }
                            else
                            {
                                mPackageComsumeItemList.Add(uId);
                            }
                        }
                        
                   }
                }
            }

            return mPackageComsumeItemList;
        }

        List<AwardItemData> oldChangeNewItem = new List<AwardItemData>();
        
        public void UnRegisterMainFrame(int iKey)
        {
            m_akMainFrameIds.Remove(iKey);
        }

        bool HasMainFrame(int iKey)
        {
            return m_akMainFrameIds.ContainsKey(iKey);
        }

        void _InitShopItemTableData()
        {
            var shopItemTable = TableManager.GetInstance().GetTable<ProtoTable.ShopItemTable>();
            if (shopItemTable!=null)
            {
                var enumerator = shopItemTable.GetEnumerator();
                while(enumerator.MoveNext())
                {
                    var shopItem = enumerator.Current.Value as ProtoTable.ShopItemTable;
                    if (null == shopItem)
                    {
                        continue;
                    }

                    var itemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(shopItem.ItemID);
                    if (itemTable==null)
                    {
                        continue;
                    }

                    if (shopItem.SubType != ProtoTable.ShopItemTable.eSubType.ST_EQUIP)
                    {
                        continue;
                    }


                    if (itemTable.SubType!= ProtoTable.ItemTable.eSubType.WEAPON)
                    {
                        continue;
                    }
                    

                    DuelWeaponsList.Add(shopItem);
                }
            }
        }

        void _InitJuedouchangItemPropellingTableData()
        {
            var juedouchangTable = TableManager.GetInstance().GetTable<JuedouchangItemPropellingTable>();
            if (juedouchangTable != null)
            {
                var enumerator = juedouchangTable.GetEnumerator();
                while(enumerator.MoveNext())
                {
                   var juedouTable = enumerator.Current.Value as JuedouchangItemPropellingTable;
                    if (juedouTable == null)
                    {
                        continue;
                    }
                    juedouchangItemPropelingList.Add(juedouTable);
                }
            }
        }

        /// <summary>
        /// 初始化租赁武器商店用到的装备初始化携带属性生成表
        /// </summary>
        void InitEquipInitialAttributeTableData()
        {
            var mTableDic = TableManager.GetInstance().GetTable<EquipInitialAttribute>().GetEnumerator();
            while (mTableDic.MoveNext())
            {
                EquipInitialAttribute table = mTableDic.Current.Value as EquipInitialAttribute;
                if (mEquipInitialAttrbuteDic.ContainsKey(table.ItemID) == false)
                {
                    mEquipInitialAttrbuteDic.Add(table.ItemID, table);
                }
            }
        }

        /// <summary>
        /// 得到武器租赁初始品质和初始强化等级
        /// </summary>
        /// <param name="itemID">道具ID</param>
        /// <param name="quality">品质</param>
        /// <param name="strenth">强化等级</param>
        public void GetWeaponLeaseInitQualityAndInitStrenth(int itemID,ref int quality,ref int strenth)
        {
            if (mEquipInitialAttrbuteDic.ContainsKey(itemID))
            {
                var mTable = mEquipInitialAttrbuteDic[itemID];
                quality = mTable.EquipQL;
                strenth = mTable.Strengthen;
            }
        }

        /// <summary>
        /// 武器租赁的装备判断是否是推荐
        /// </summary>
        /// <param name="itemID"></param>
        public bool WeaponLeaseIsRecommendOccu(int itemID)
        {
            bool isRecommendFlag = false;
            if (mEquipInitialAttrbuteDic.ContainsKey(itemID))
            {
                var mTable = mEquipInitialAttrbuteDic[itemID];

                for (int i = 0; i < mTable.FitOccu.Length; i++)
                {
                    int fitOccu = 0;

                    if (int.TryParse(mTable.FitOccu[i] , out fitOccu))
                    {
                    }

                    if (fitOccu != PlayerBaseData.GetInstance().JobTableID)
                    {
                        continue;
                    }

                    isRecommendFlag = true;
                }
            }

            return isRecommendFlag;
        }

        public void InitBaseWeaponData(int curJob)
        {
            DuelWeaponsDic.Clear();

            //对GetDict的数据初始化；
            for (int i = 0; i < DuelWeaponsList.Count; i++)
            {
                ProtoTable.ItemTable itemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(DuelWeaponsList[i].ItemID);

                if (itemTable==null)
                {
                    continue;
                }

                if (itemTable.Occu.Count >= 1 && itemTable.Occu[0] != 0)
                {
                    for (int j = 0; j < itemTable.Occu.Count; j++)
                    {
                        if (itemTable.Occu[j] / 10 * 10 != curJob)
                        {
                            continue;
                        }

                        List<ProtoTable.ShopItemTable> shopdata = null;

                        if(!DuelWeaponsDic.TryGetValue(itemTable.NeedLevel , out shopdata))
                        {
                            DuelWeaponsDic.Add(itemTable.NeedLevel, new List<ProtoTable.ShopItemTable>());
                        }

                        DuelWeaponsDic[itemTable.NeedLevel].Add(DuelWeaponsList[i]);
                    }
                }
            }
        }

        public List<ProtoTable.ShopItemTable> _ScreenCurrJobDuelWeapons(int curLevel)
        {
            List<ProtoTable.ShopItemTable> GetDuelList = null;

            for (int i = 0; i < juedouchangItemPropelingList.Count; i++)
            {
                if (curLevel >=juedouchangItemPropelingList[i].NeedMinLevel && curLevel<=juedouchangItemPropelingList[i].NeedMaxLevel)
                {
                    if (DuelWeaponsDic.TryGetValue(juedouchangItemPropelingList[i].ItemLevel, out GetDuelList))
                    {
                        return GetDuelList;
                    }
                }
            }
            return null;
        }

        public Dictionary<int, List<ProtoTable.ShopItemTable>> GetDuelWeaponsDict()
        {
            return DuelWeaponsDic;
        }

        public void OpenShop(int shopID,int shopLinkID = 0,int shopTabID = -1, ShopFrame.OnShopReturn onShopReturn = null,GameObject goRoot = null,ShopFrame.ShopFrameMode eMode = ShopFrame.ShopFrameMode.SFM_MAIN_FRAME,int target = 0,int linkNpcId = -1)
        {
            //判断是否使用新的商店框架
            //且eMode 非 ShopFrameMode.SFM_QUERY_NON_FRAME 类型。如果是这种类型的话：只是同步数据，并处理回调，并不会打开页面，
            //所以在这种类型下，可以不打开新的框架而去同步数据
            if (ShopNewDataManager.GetInstance().IsShopNewFrameByShopId(shopID) == true
                && eMode != ShopFrame.ShopFrameMode.SFM_QUERY_NON_FRAME)
            {
                ShopNewDataManager.GetInstance().OpenShopNewFrame(shopID, 0, shopTabID);
                return;
            }

            ShopData shopData = m_arrShopDatas.Find(data => { return data.ID == shopID; });
            if (shopData == null)
            {
                ProtoTable.ShopTable shopTable = TableManager.GetInstance().GetTableItem<ProtoTable.ShopTable>(shopID);
                if (shopTable == null)
                {
                    return;
                }

                shopData = new ShopData();
                shopData.ID = shopTable.ID;
                shopData.Name = shopTable.ShopName;
                shopData.ShopNamePath = shopTable.ShopNamePath;
                shopData.GoodsTypes = new List<GoodsType>(shopTable.SubType);
                shopData.NeedRefresh = shopTable.Refresh;
                shopData.RefreshCost = 0;// shopTable.RefreshCost;
                shopData.RefreshTime = 0;
                shopData.RefreshLeftTimes = 0;
                shopData.RefreshTotalTimes = 0;
                shopData.WeekRefreshTime = 0;
                shopData.MonthRefreshTime = 0;
                m_arrShopDatas.Add(shopData);
            }
            shopData.iLinkNpcId = linkNpcId;

            if (shopData.NeedRefresh == 0)
            {
                if (shopData.Goods.Count <= 0)
                {
                    Dictionary<int, object> shopItemTables = TableManager.GetInstance().GetTable<ProtoTable.ShopItemTable>();
                    var iter = shopItemTables.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        ProtoTable.ShopItemTable shopItemTable = iter.Current.Value as ProtoTable.ShopItemTable;
                        if (shopItemTable.ShopID == shopData.ID)
                        {
                            GoodsData goodsData = new GoodsData();
                            goodsData.ID = shopItemTable.ID;
                            goodsData.ItemData = GameClient.ItemDataManager.CreateItemDataFromTable(shopItemTable.ItemID);
                            goodsData.Type = (GoodsType)(int)shopItemTable.SubType;
                            goodsData.CostItemData = GameClient.ItemDataManager.CreateItemDataFromTable(shopItemTable.CostItemID);
                            goodsData.CostItemCount = shopItemTable.CostNum;
                            goodsData.eGoodsLimitButyType = (GoodsLimitButyType)shopItemTable.ExLimite;
                            goodsData.iLimitValue = shopItemTable.ExValue;
                            goodsData.shopItem = iter.Current.Value as ProtoTable.ShopItemTable;
                            goodsData.LimitBuyCount = 1;
                            if (goodsData.shopItem != null)
                            {
                                goodsData.LimitBuyCount = goodsData.shopItem.GroupNum;
                            }
                            goodsData.LimitBuyTimes = -1;
                            if(goodsData.shopItem != null)
                            {
                                goodsData.LimitBuyTimes = -1;
                            }
                            goodsData.ItemData.Count = goodsData.LimitBuyCount.GetValueOrDefault();

                            if (goodsData.ItemData != null && goodsData.CostItemData != null)
                            {
                                shopData.Goods.Add(goodsData);
                            }
                            else
                            {
                                if(goodsData.ItemData == null)
                                {
                                    Logger.LogErrorFormat("goodsData.ItemData is null id = {0}", shopItemTable.ItemID);
                                }
                                
                                if(goodsData.CostItemData == null)
                                {
                                    Logger.LogErrorFormat("goodsData.CostItemData is null id = {0}", shopItemTable.CostItemID);
                                }
                            }
                        }
                    }
                }

                if(eMode == ShopFrame.ShopFrameMode.SFM_QUERY_NON_FRAME)
                {
                    if(null != onShopReturn)
                    {
                        onShopReturn();
                    }
                    return;
                }

                ShopFrame.ShopFrameData data = new ShopFrame.ShopFrameData();
                data.m_kShopData = shopData;
                data.m_iShopLinkID = shopLinkID;
                data.m_iShopTabID = shopTabID;
                data.onShopReturn = onShopReturn;
                data.eShopFrameMode = ShopFrame.ShopFrameMode.SFM_MAIN_FRAME;

                if(goRoot == null)
                {
                    ClientSystemManager.GetInstance().OpenFrame<ShopFrame>(FrameLayer.Middle, data);
                }
                else
                {
                    //onOpenChildShopFrame
                    if(target != 0 && HasMainFrame(target))
                    {
                        data.eShopFrameMode = eMode;
                        IClientFrame clientFrame = ClientSystemManager.GetInstance().OpenFrame<ShopFrame>(goRoot, data, "ShopFrame" + shopID);
                        if (onOpenChildShopFrame != null)
                        {
                            onOpenChildShopFrame.Invoke(shopID, clientFrame as ShopFrame, target);
                        }
                    }
                }
            }
            else
            {
                SceneShopQuery msg = new SceneShopQuery();
                msg.shopId = (byte)shopData.ID;
                msg.cache = (byte)(shopData.Goods.Count > 0 ? 1 : 0);
                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, msg);

                WaitNetMessageManager.GetInstance().Wait<SceneShopQueryRet>(msgRet =>
                {
                    if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                    {
                        SystemNotifyManager.SystemNotify((int)msgRet.code);
                    }
                    else
                    {
                        for (int i = 0; i < mysteryShopIDs.Length; i++)
                        {
                            if (shopID != mysteryShopIDs[i])
                            {
                                continue;
                            }

                            MysteryShopData mysteryShopData = new MysteryShopData();
                            mysteryShopData.mysteryShopData = shopData;

                            ClientSystemManager.GetInstance().OpenFrame<MysteryShopFrame>(FrameLayer.Middle, mysteryShopData);
                            return;
                        }

                        var mShopTable = TableManager.GetInstance().GetTableItem<ShopTable>(shopID);
                        if (mShopTable != null)
                        {
                            if (mShopTable.ShopKind == ShopTable.eShopKind.SK_Lease)
                            {
                                ClientSystemManager.GetInstance().OpenFrame<WeaponLeaseShopFrame>(FrameLayer.Middle, shopData);
                                return;
                            }
                        }
                       
                        if (eMode == ShopFrame.ShopFrameMode.SFM_QUERY_NON_FRAME)
                        {
                            if (null != onShopReturn)
                            {
                                onShopReturn();
                            }
                            return;
                        }

                        ShopFrame.ShopFrameData data = new ShopFrame.ShopFrameData();
                        data.m_kShopData = shopData;
                        data.m_iShopLinkID = shopLinkID;
                        data.m_iShopTabID = shopTabID;
                        data.onShopReturn = onShopReturn;
                        data.eShopFrameMode = ShopFrame.ShopFrameMode.SFM_MAIN_FRAME;

                        if (goRoot == null)
                        {
                            ClientSystemManager.GetInstance().OpenFrame<ShopFrame>(FrameLayer.Middle, data);
                        }
                        else
                        {
                            if (target != 0 && HasMainFrame(target))
                            {
                                data.eShopFrameMode = eMode;
                                IClientFrame clientFrame = ClientSystemManager.GetInstance().OpenFrame<ShopFrame>(goRoot, data, "ShopFrame" + shopData.ID.Value);
                                if (onOpenChildShopFrame != null)
                                {
                                    onOpenChildShopFrame.Invoke(shopData.ID.Value, clientFrame as ShopFrame, target);
                                }
                            }
                        }
                    }
                });
            }
        }

        public void OpenMysteryShopFrame()
        {
            var tab = TableManager.GetInstance().GetTableItem<MysticalMerchantTable>(MysticalMerchantID);
            if (tab == null)
            {
                return;
            }

            OpenShop(tab.ShopId);
        }

        public ShopData GetGoodsDataFromShop(int iShopID)
        {
            ShopData shopData = m_arrShopDatas.Find(data => { return data.ID == iShopID; });
            return shopData;
        }

        protected void _OnSceneShopItemSync(MsgDATA msg)
        {
            Logger.LogError("_OnSceneShopItemSync !");
        }

        protected void _OnResetShopGoods(MsgDATA msg)
        {
            CustomDecoder.ProtoShop msgRet;
            int pos = 0;
            if (CustomDecoder.DecodeShop(out msgRet, msg.bytes, ref pos, msg.bytes.Length) && msgRet != null)
            {
                ShopData shopData = m_arrShopDatas.Find(data => { return data.ID == msgRet.shopID; });
                if (shopData != null)
                {
                    shopData.RefreshCost = (int)msgRet.refreshCost;
                    shopData.RefreshTime = msgRet.restRefreshTime;
                    shopData.RefreshLeftTimes = msgRet.refreshTimes;
                    shopData.RefreshTotalTimes = msgRet.refreshAllTimes;
                    shopData.WeekRefreshTime = msgRet.WeekRestRefreshTime;
                    shopData.MonthRefreshTime = msgRet.MonthRefreshTime;
                    shopData.Goods.Clear();
                    for (int i = 0; i < msgRet.shopItemList.Count; ++i)
                    {
                        CustomDecoder.ProtoShopItem protoShopItem = msgRet.shopItemList[i];

                        GoodsData goodsData = new GoodsData();
                        goodsData.ID = (int)protoShopItem.shopItemId;
                        goodsData.VipLimitLevel = protoShopItem.vipLv;
                        goodsData.VipDiscount = protoShopItem.vipDiscount;
                        
                        // init table data
                        ProtoTable.ShopItemTable shopItemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ShopItemTable>((int)goodsData.ID);
                        goodsData.shopItem = shopItemTable;
                        goodsData.LimitBuyCount = 1;
                        if (goodsData.shopItem != null)
                        {
                            goodsData.LimitBuyCount = goodsData.shopItem.GroupNum;
                        }
                        if(shopItemTable == null)
                        {
                            Logger.LogErrorFormat("shopItemId = {0} can not find in table shopItemTable!", (int)goodsData.ID);
                            continue;
                        }
                        goodsData.LimitBuyTimes = protoShopItem.restNum;

                        ShopTable mShopTable = TableManager.GetInstance().GetTableItem<ShopTable>((int)msgRet.shopID);
                        if (mShopTable != null)
                        {
                            //如果是武器租赁商店 创建Item设置初始品级和初始强化等级
                            if (mShopTable.ShopKind == ShopTable.eShopKind.SK_Lease)
                            {
                                int quality = 0;
                                int strenthLevel = 0;
                                GetWeaponLeaseInitQualityAndInitStrenth(shopItemTable.ItemID, ref quality, ref strenthLevel);
                                goodsData.ItemData = GameClient.ItemDataManager.CreateItemDataFromTable(shopItemTable.ItemID,quality,strenthLevel);
                                goodsData.LeaseEndTimeStamp = (int)protoShopItem.leaseEndTimeStamp;
                            }
                            else
                            {
                                goodsData.ItemData = GameClient.ItemDataManager.CreateItemDataFromTable(shopItemTable.ItemID);
                            }
                        }
                        
                        if (goodsData.ItemData == null)
                        {
                            Logger.LogErrorFormat("ItemID = {0} can not find in itemTable!", (int)shopItemTable.ItemID);
                            continue;
                        }
                        goodsData.Type = (GoodsType)(int)shopItemTable.SubType;
                        goodsData.CostItemData = GameClient.ItemDataManager.CreateItemDataFromTable(shopItemTable.CostItemID);
                        if (goodsData.CostItemData == null)
                        {
                            Logger.LogErrorFormat("CostItemID = {0} can not find in itemTable!", (int)shopItemTable.CostItemID);
                            continue;
                        }
                        goodsData.CostItemCount = shopItemTable.CostNum;
                        goodsData.eGoodsLimitButyType = (GoodsLimitButyType)shopItemTable.ExLimite;
                        goodsData.iLimitValue = shopItemTable.ExValue;
                        goodsData.ItemData.Count = goodsData.LimitBuyCount.GetValueOrDefault();
                        goodsData.TotalLimitBuyTimes = shopItemTable.NumLimite;
                        goodsData.GoodsDisCount = (int)protoShopItem.discount;

                        if(goodsData.ItemData != null && goodsData.CostItemData != null)
                        {
                            shopData.Goods.Add(goodsData);
                        }
                    }
                }
            }

        }

        public void BuyGoods(int shopID, int goodsID, int count,List<ItemInfo> info)
        {
            SceneShopBuy msg = new SceneShopBuy();
            msg.shopId = (byte)shopID;
            msg.shopItemId = (uint)goodsID;
            msg.num = (ushort)count;
            msg.costExtraItems = info.ToArray();
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
                    UIEvent uiEvent = UIEventSystem.GetInstance().GetIdleUIEvent();
                    uiEvent.EventID = EUIEventID.ShopBuyGoodsSuccess;
                    uiEvent.EventParams.buyGoodsResult = new BuyGoodsResult((int)msgRet.shopItemId, (int)msgRet.newNum);

                    var shopData = GetGoodsDataFromShop(shopID);
                    if (shopData != null)
                    {
                        var current = shopData.Goods.Find(x => { return x.ID == msgRet.shopItemId; });
                        if (current != null)
                        {
                            current.LimitBuyTimes = current.LimitBuyTimes >= 0 ? msgRet.newNum : -1;

                            var mShopTable = TableManager.GetInstance().GetTableItem<ShopTable>(shopID);
                            if (mShopTable != null)
                            {
                                //只有武器租赁商店用到LeaseEndTimeStamp字段
                                if (mShopTable.ShopKind == ShopTable.eShopKind.SK_Lease)
                                {
                                    current.LeaseEndTimeStamp = (int)msgRet.leaseEndTimeStamp;
                                }
                            }

                            UIEventSystem.GetInstance().SendUIEvent(uiEvent);
                        }
                    }
                }
            });
        }



        public void RefreshShop(int shopID)
        {
            SceneShopRefresh msg = new SceneShopRefresh();
            msg.shopId = (byte)shopID;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<SceneShopRefreshRet>(msgRet =>
            {
                if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.code);
                }
                else
                {
                    UIEvent uiEvent = UIEventSystem.GetInstance().GetIdleUIEvent();
                    uiEvent.EventID = EUIEventID.ShopRefreshSuccess;
                    UIEventSystem.GetInstance().SendUIEvent(uiEvent);
                }
            });
        }

        public void _OnSceneSyncMysticalMerchant(MsgDATA msg)
        {
            SceneSyncMysticalMerchant ret = new SceneSyncMysticalMerchant();
            ret.decode(msg.bytes);

            mysticalMerchantId = (int)ret.id;

            MysticalMerchantTiggerNumber();
            MysticalMerchantType();
        }

        public void OnGoldBuy(int id)
        {
            WorldMallQuerySingleItemReq msg = new WorldMallQuerySingleItemReq();
            msg.mallItemId = (uint)id;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);
        }

        private void _OnWorldMallQuerySingleItemRes(MsgDATA msg)
        {
            WorldMallQuerySingleItemRes ret = new WorldMallQuerySingleItemRes();
            ret.decode(msg.bytes);

            if (ret.retCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)ret.retCode);
            }
            else
            {
                var mallItemInfo = ret.mallItemInfo;
                ClientSystemManager.GetInstance().OpenFrame<MallBuyFrame>(FrameLayer.Middle, mallItemInfo);
            }
        }

        /// <summary>
        /// 寻找神秘商店ID
        /// </summary>
        /// <param name="goodData"></param>
        /// <returns></returns>
        public bool FindMysteryShopID(int shopId)
        {
            bool isFind = false;
            for (int i = 0; i < ShopDataManager.GetInstance().mysteryShopIDs.Length; i++)
            {
                if (shopId != ShopDataManager.GetInstance().mysteryShopIDs[i])
                {
                    continue;
                }

                isFind = true;
            }

            return isFind;
        }

        void MysticalMerchantTiggerNumber()
        {
            mySteryMerchantTriggerNumber++;
            GameStatisticManager.GetInstance().DoStartMysticalMerchant(mySteryMerchantTriggerNumber);
        }

        void MysticalMerchantType()
        {
            var tab = TableManager.GetInstance().GetTableItem<MysticalMerchantTable>(MysticalMerchantID);
            if (tab==null)
            {
                return;
            }

            var shopTab = TableManager.GetInstance().GetTableItem<ShopTable>(tab.ShopId);
            if (shopTab == null)
                return;

            GameStatisticManager.GetInstance().DoStartMysticalMerchantType(shopTab.ShopName);

        }

    }
}
