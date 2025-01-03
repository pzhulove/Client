using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;

namespace GameClient
{
    //拍卖行助手类
    public static class AuctionNewUtility
    {

        //拍卖行中附魔卡等级相关内容
        public static void OnCloseAuctionNewMagicCardStrengthLevelFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<AuctionNewMagicCardStrengthenLevelFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<AuctionNewMagicCardStrengthenLevelFrame>();
        }

        public static void OnOpenAuctionNewMagicCardStrengthLevelFrame(uint itemId,
            int defaultLevel)
        {
            OnCloseAuctionNewMagicCardStrengthLevelFrame();

            AuctionNewMagicCardDataModel dataModel = new AuctionNewMagicCardDataModel()
            {
                ItemId = itemId,
                DefaultLevel = defaultLevel,
            };

            ClientSystemManager.GetInstance().OpenFrame<AuctionNewMagicCardStrengthenLevelFrame>(
                FrameLayer.Middle, dataModel);
        }

        public static bool IsMagicCardItem(uint itemId)
        {
            if (itemId <= 0)
                return false;

            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>((int)itemId);
            if (itemTable == null)
                return false;

            if (itemTable.Type == ItemTable.eType.EXPENDABLE
               && itemTable.SubType == ItemTable.eSubType.EnchantmentsCard)
                return true;

            return false;
        }

        public static string GetMainTabNameByTabType(AuctionNewMainTabType mainTabType)
        {
            switch (mainTabType)
            {
                case AuctionNewMainTabType.AuctionBuyType:
                    return TR.Value("auction_new_buy_label");
                case AuctionNewMainTabType.AuctionNoticeType:
                    return TR.Value("auction_new_notice_item_label");
                case AuctionNewMainTabType.AuctionSellType:
                    return TR.Value("auction_new_sell_label");
            }

            return TR.Value("auction_new_title");
        }

        //得到道具的基本类型
        private static AuctionMainItemType GetBaseTypeByTableId(int tableId)
        {
            ItemTable itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(tableId);

            if (itemTable == null)
            {
                return AuctionMainItemType.AMIT_INVALID;
            }

            if (itemTable.SubType == ItemTable.eSubType.WEAPON)
            {
                return AuctionMainItemType.AMIT_WEAPON;
            }
            else if (itemTable.SubType == ItemTable.eSubType.HEAD ||
                     itemTable.SubType == ItemTable.eSubType.CHEST ||
                     itemTable.SubType == ItemTable.eSubType.BELT ||
                     itemTable.SubType == ItemTable.eSubType.LEG ||
                     itemTable.SubType == ItemTable.eSubType.BOOT)
            {
                return AuctionMainItemType.AMIT_ARMOR;
            }
            else if (itemTable.SubType == ItemTable.eSubType.RING ||
                     itemTable.SubType == ItemTable.eSubType.NECKLASE ||
                     itemTable.SubType == ItemTable.eSubType.BRACELET ||
                     itemTable.SubType == ItemTable.eSubType.TITLE)
            {
                return AuctionMainItemType.AMIT_JEWELRY;
            }
            else if (itemTable.Type == ItemTable.eType.EXPENDABLE)
            {
                return AuctionMainItemType.AMIT_COST;
            }
            else if (itemTable.Type == ItemTable.eType.MATERIAL)
            {
                return AuctionMainItemType.AMIT_MATERIAL;
            }

            return AuctionMainItemType.AMIT_INVALID;
        }

        //判断道具是否为装备
        public static bool IsEquipItems(ItemData data)
        {
            var auctionMainItemType = GetBaseTypeByTableId(data.TableID);

            //装备：武器，防具，首饰
            if (auctionMainItemType == AuctionMainItemType.AMIT_WEAPON
                || auctionMainItemType == AuctionMainItemType.AMIT_ARMOR
                || auctionMainItemType == AuctionMainItemType.AMIT_JEWELRY)
            {
                return true;
            }

            //辅助装备，臂章
            var itemData = TableManager.GetInstance().GetTableItem<ItemTable>(data.TableID);
            if (itemData != null)
            {
                //辅助装备
                if (itemData.SubType == ItemTable.eSubType.ST_ASSIST_EQUIP)
                {
                    return true;
                }

                //魔法石
                if (itemData.SubType == ItemTable.eSubType.ST_MAGICSTONE_EQUIP)
                {
                    return true;
                }

                //耳环
                if (itemData.SubType == ItemTable.eSubType.ST_EARRINGS_EQUIP)
                {
                    return true;
                }

                //辟邪玉[披风]
                if (itemData.SubType == ItemTable.eSubType.ST_BXY_EQUIP)
                {
                    return true;
                }
            }

            return false;
        }

        //从TipFrame中打开拍卖行界面，并且会弹出上架界面
        public static void OpenAuctionNewFrame(ItemData itemData)
        {

            if (ClientSystemManager.GetInstance().IsFrameOpen<AuctionNewFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<AuctionNewFrame>();

            //物品是否正在冷却
            var isItemInCoolTime = IsAuctionNewItemInCoolTime(itemData);
            //展示物品需要冷却，进行飘字
            if (isItemInCoolTime == true)
            {
                AuctionNewUtility.ShowItemInCoolTimeFloatingEffect(itemData.AuctionCoolTimeStamp,
                    TimeManager.GetInstance().GetServerTime());
                return;
            }

            //物品的交易次数限制
            var isItemTradeLimit = ItemDataUtility.IsItemTradeLimitBuyNumber(itemData);
            var itemTradeLeftTime = ItemDataUtility.GetItemTradeLeftTime(itemData);

            //物品存在交易限制
            if (isItemTradeLimit == true)
            {
                //剩余交易次数为0， 直接飘字
                if (itemTradeLeftTime <= 0)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("auction_new_item__trade_number_zero"));
                    return;
                }
                else
                {
                    //剩余交易次数大于0，弹窗提示。确认后，进入拍卖行
                    var contentStr = string.Format(TR.Value("auction_new_item_on_shelf_with_trade_number"),
                        itemTradeLeftTime);

                    AuctionNewUtility.OnShowItemTradeLimitFrame(contentStr,
                        () => { OpenAuctionNewFrameAction(itemData); });
                    return;
                }
            }

            OpenAuctionNewFrameAction(itemData);
            
        }

        private static void OpenAuctionNewFrameAction(ItemData itemData)
        {
            if (itemData == null)
                return;

            AuctionNewUserData auctionNewUserData = new AuctionNewUserData();
            auctionNewUserData.MainTabType = AuctionNewMainTabType.AuctionSellType;
            auctionNewUserData.ItemLinkId = itemData.GUID;
            ClientSystemManager.GetInstance().OpenFrame<AuctionNewFrame>(FrameLayer.Middle, auctionNewUserData);
            ItemTipManager.GetInstance().CloseAll();
        }


        public static string GetQualityColorString(string textStr, ItemTable.eColor qualityColor)
        {
            var qualityInfo = ItemData.GetQualityInfo(qualityColor);
            if (qualityInfo != null)
            {
                return TR.Value("common_color_text", qualityInfo.ColStr, textStr);
            }

            return "";
        }

        public static string GetCountDownTimeStr(uint endTime, uint curTime)
        {

            uint hour = 0;
            uint min = 0;
            if (endTime <= curTime)
                return string.Format("{0:D2}:{1:D2}", hour, min);

            var intervalTime = endTime - curTime;
            hour = intervalTime / 3600;
            min = (intervalTime - 3600 * hour) / 60;

            return string.Format("{0:D2}:{1:D2}", hour, min);
        }

        public static bool IsTreasureItem(int itemId)
        {
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(itemId);
            if (itemTable != null && itemTable.IsTreas == 1)
                return true;

            return false;
        }

        //判断是否处在珍品抢购时间内：公示结束时间的5s之内购买
        public static bool IsAuctionNewTreasureRushBuy(byte isTreasure, uint publicEndTime)
        {
            bool isTreasureFlag = isTreasure == 1 ? true : false;
            //非珍品
            if (isTreasureFlag == false)
            {
                return false;
            }

            //公示的时间未到
            if (TimeManager.GetInstance().GetServerTime() < publicEndTime)
                return false;

            //超过珍品上架预设的时间间隔
            if (TimeManager.GetInstance().GetServerTime() >
                publicEndTime + AuctionNewDataManager.GetInstance().TreasureItemRushBuyTimeInterval)
                return false;

            return true;
        }

        public static bool IsAuctionNewItemInCoolTime(ItemData itemData)
        {
            if (itemData == null)
                return false;

            if (itemData.AuctionCoolTimeStamp <= 0)
                return false;

            var serverTime = TimeManager.GetInstance().GetServerTime();

            if (itemData.AuctionCoolTimeStamp <= serverTime)
                return false;

            return true;
        }

        //显示冷却提示
        public static void ShowItemInCoolTimeFloatingEffect(uint endTimeStamp, uint curServerTime)
        {
            var coolDownTimeStr = CountDownTimeUtility.GetCoolDownTimeByDayHour(endTimeStamp,
                curServerTime);
            var showStr = string.Format(TR.Value("auction_new_item_trade_in_cd_time_"),
                coolDownTimeStr);
            SystemNotifyManager.SysNotifyFloatingEffect(showStr);
        }

        public static bool IsAuctionNewSameState(AuctionNewMainTabType mainTabType,
            AuctionGoodState auctionGoodState)
        {
            if (auctionGoodState == AuctionGoodState.AGS_PUBLIC
                && mainTabType == AuctionNewMainTabType.AuctionNoticeType)
                return true;

            if (auctionGoodState == AuctionGoodState.AGS_ONSALE
                && mainTabType == AuctionNewMainTabType.AuctionBuyType)
                return true;

            return false;
        }

        public static AuctionGoodState ConvertToAuctionGoodState(AuctionNewMainTabType mainTabType)
        {
            if (mainTabType == AuctionNewMainTabType.AuctionBuyType)
                return AuctionGoodState.AGS_ONSALE;

            return AuctionGoodState.AGS_PUBLIC;
        }

        public static string ConvertDescLine(string descStr)
        {
            if (string.IsNullOrEmpty(descStr) == true)
                return descStr;

            return descStr.Replace("\\n", "\n");
        }

        public static string ConvertDescBlank(string descStr)
        {
            if (string.IsNullOrEmpty(descStr) == true)
                return descStr;

            return descStr.Replace("\\u3000", "\u3000");
        }

        //打开上架物品的页面
        public static void OpenAuctionNewOnShelfFrame(ulong itemGuid,bool isTreasure,
            int maxShelfNumber,
            int selfOnShelfNumber)
        {
            var auctionNewOnShelfItemData = new AuctionNewOnShelfItemData
            {
                PackageItemGuid = itemGuid,
                MaxShelfNum = maxShelfNumber,
                SelfOnShelfItemNum = selfOnShelfNumber,
                IsTreasure = isTreasure,
            };

            ClientSystemManager.GetInstance()
                .OpenFrame<AuctionNewOnShelfFrame>(FrameLayer.Middle, auctionNewOnShelfItemData);
        }

        public static void OpenAuctionNewOnShelfFrameByTimeOverItem(AuctionBaseInfo auctionBaseInfo)
        {
            CloseAuctionNewOnShelfFrame();
            var auctionNewOnShelfItemData = new AuctionNewOnShelfItemData
            {
                PackageItemGuid = auctionBaseInfo.guid,
                IsTreasure = auctionBaseInfo.isTreas == 1 ? true : false,
                ItemAuctionBaseInfo = auctionBaseInfo,
                IsTimeOverItem = true,
            };

            ClientSystemManager.GetInstance()
                .OpenFrame<AuctionNewOnShelfFrame>(FrameLayer.Middle, auctionNewOnShelfItemData);
        }

        //关闭上架物品的页面
        public static void CloseAuctionNewOnShelfFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<AuctionNewOnShelfFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<AuctionNewOnShelfFrame>();
        }

        //上架商品的价格数组，介于最值之间；按照一定的规律进行排列
        public static int[] GetOnShelfItemPriceArray(int averagePrice, int minPrice, int maxPrice,
                            int maxPriceRate = 0)
        {
            if (averagePrice == 0)
            {
                Logger.LogErrorFormat("AveragePrice is 0");
                return new[] {0};
            }

            var itemPriceList = new List<int> {averagePrice, minPrice, maxPrice};

            var priceRate = 110;
            var tempPrice = GetTempPrice(averagePrice, priceRate);

            var canReachMaxPriceRate = maxPriceRate * 8;    //和服务器计算保持一直，避免传过来的最大值无限大，造成一直循环

            //从平均价格的1.1倍，每次倍率增加0.1,并且价格的小于最大值，找到所有的价格数值
            //最大值已经存入
            //价格不会超过最小值
            while (tempPrice < maxPrice)
            {
                //价格加入列表
                itemPriceList.Add(tempPrice);

                //避免无限循环的增加
                if (canReachMaxPriceRate >= 100)
                {
                    if (priceRate > canReachMaxPriceRate)
                        break;
                }

                //倍率增加10
                priceRate += 10;

                tempPrice = GetTempPrice(averagePrice, priceRate);
            }

            priceRate = 90;
            tempPrice = GetTempPrice(averagePrice, priceRate);
            //从平均价格的0.9倍开始计算，在倍率大于0.1的时候，每次倍率减少0.1；在倍率小于0.1的时候，倍率每次减少0.02;倍率最低是0.01,
            //并且价格大于最小值，找到所有的价格数值
            //最小值已经存入
            //价格不会小于最小值，并且倍率最低为0.01f;
            while (tempPrice > minPrice)
            {
                //价格加入列表
                itemPriceList.Add(tempPrice);


                //处理边界条件
                if (priceRate <= 1)
                {
                    break;
                }

                //倍率大于0.1的时候，减少10
                if (priceRate > 10)
                {
                    priceRate -= 10;
                }
                else if (priceRate > 2)
                {
                    //倍率小于0.1的时候，减少2
                    priceRate -= 2;
                }
                else
                {
                    //最小倍率1
                    priceRate -= 1;
                }

                if (priceRate <= 1)
                {
                    priceRate = 1;
                }

                tempPrice = GetTempPrice(averagePrice, priceRate);
            }

            //价格排序
            itemPriceList.Sort();
            //价格去重
            var finalItemPriceList = itemPriceList.Distinct();

            //返回价格数组
            return finalItemPriceList.ToArray();
        }

        //得到价格数组中的价格索引
        public static int GetPriceIndexInPriceArray(int[] priceArray, int price)
        {
            for (var i = 0; i < priceArray.Length; i++)
            {
                if (priceArray[i] == price)
                    return i;
            }

            return 0;
        }

        //判断某个价格是否在价格数组中
        public static bool IsPriceInPriceArray(int[] priceArray, int price)
        {
            if (priceArray == null || priceArray.Length <= 0)
                return false;

            for (var i = 0; i < priceArray.Length; i++)
            {
                if (priceArray[i] == price)
                {
                    return true;
                }
            }

            return false;
        }

        //如果价格不再数组中，得到数组中比当前价格大的最小的索引
        public static int GetNextIndexInPriceArray(int[] priceArray, int price)
        {
            for (var i = 0; i < priceArray.Length; i++)
            {
                if (priceArray[i] > price)
                {
                    return i;
                }
            }

            return 0;
        }

        //如果价格不在数组中，得到数组中比当前价格小的最大索引
        public static int GetPreIndexInPriceArray(int[] priceArray, int price)
        {
            for (var i = priceArray.Length - 1; i >= 0; i--)
            {
                if (priceArray[i] < price)
                    return i;
            }

            return 0;
        }

        //得到价格
        public static int GetTempPrice(int averagePrice, int priceRate)
        {
            var tempPrice = (long) priceRate * (long)averagePrice;
            tempPrice = (long)(tempPrice / 100.0f);
            return (int) tempPrice;
        }

        //得到第二层级的TabDataModel
        public static List<AuctionNewMenuTabDataModel> GetAuctionNewChildrenLayerTabDataModelList(
            AuctionNewMenuTabDataModel parentLayerMenuTabDataModel,
            AuctionNewMainTabType auctionNewMainTabType = AuctionNewMainTabType.AuctionBuyType)
        {
            if (parentLayerMenuTabDataModel == null || parentLayerMenuTabDataModel.AuctionNewFrameTable == null)
                return null;

            if (parentLayerMenuTabDataModel.AuctionNewFrameTable.LayerRelation.Count == 0
                || (parentLayerMenuTabDataModel.AuctionNewFrameTable.LayerRelation.Count == 1
                    && parentLayerMenuTabDataModel.AuctionNewFrameTable.LayerRelation[0] == 0))
                return null;

            List<AuctionNewMenuTabDataModel> childrenLayerTabDataModelList = new List<AuctionNewMenuTabDataModel>();
            for (var i = 0; i < parentLayerMenuTabDataModel.AuctionNewFrameTable.LayerRelation.Count; i++)
            {
                var childId = parentLayerMenuTabDataModel.AuctionNewFrameTable.LayerRelation[i];
                var curAuctionNewFrameTable = TableManager.GetInstance().GetTableItem<AuctionNewFrameTable>(childId);
                if (curAuctionNewFrameTable != null)
                {
                    //关注界面的第二层数据由服务器同步决定
                    if (auctionNewMainTabType == AuctionNewMainTabType.AuctionNoticeType)
                    {
                        if (AuctionNewDataManager.GetInstance().IsNoticeLayerIdValid(curAuctionNewFrameTable.ID) == false)
                            continue;
                    }

                    var curAuctionNewMenuTabDataModel = new AuctionNewMenuTabDataModel(
                        curAuctionNewFrameTable.ID,
                        curAuctionNewFrameTable.Layer,
                        curAuctionNewFrameTable.Sort,
                        curAuctionNewFrameTable);
                    childrenLayerTabDataModelList.Add(curAuctionNewMenuTabDataModel);
                }
            }

            if (childrenLayerTabDataModelList.Count > 0)
            {
                childrenLayerTabDataModelList.Sort((x, y) => x.Sort.CompareTo(y.Sort));
            }

            return childrenLayerTabDataModelList;

        }

        //得到第一层级的TabDataModel
        public static List<AuctionNewMenuTabDataModel> GetAuctionNewFirstLayerTabDataModelList(
            AuctionNewMainTabType auctionNewMainTabType = AuctionNewMainTabType.AuctionBuyType)
        {
            var firstLayerTabDataModelList = new List<AuctionNewMenuTabDataModel>();

            //表格数据异常
            var auctionNewFrameTables = TableManager.GetInstance().GetTable<AuctionNewFrameTable>();
            if (auctionNewFrameTables == null)
                return null;

            var frameTableIter = auctionNewFrameTables.GetEnumerator();
            while (frameTableIter.MoveNext())
            {
                var auctionNewFrameTable = frameTableIter.Current.Value as AuctionNewFrameTable;
                if (auctionNewFrameTable != null
                    && auctionNewFrameTable.Layer == 1
                    && auctionNewFrameTable.IsShow == 1)
                {
                    //公示界面的非关注页签第一层数据由服务器同步决定
                    if (auctionNewMainTabType == AuctionNewMainTabType.AuctionNoticeType
                        && auctionNewFrameTable.MenuType != AuctionNewFrameTable.eMenuType.Auction_Menu_Notice)
                    {
                        if(AuctionNewDataManager.GetInstance().IsNoticeLayerIdValid(auctionNewFrameTable.ID) == false)
                            continue;
                    }

                    //关注页签根据服务器的开关来决定是否添加
                    if (auctionNewFrameTable.MenuType == AuctionNewFrameTable.eMenuType.Auction_Menu_Notice)
                    {
                        //珍品机制没有打开，不添加关注页签
                        if (IsAuctionTreasureItemOpen() == false)
                            continue;
                    }

                    var auctionNewTabDataModel = new AuctionNewMenuTabDataModel(
                        auctionNewFrameTable.ID,
                        auctionNewFrameTable.Layer,
                        auctionNewFrameTable.Sort,
                        auctionNewFrameTable);
                    firstLayerTabDataModelList.Add(auctionNewTabDataModel);
                }
            }

            if (firstLayerTabDataModelList.Count > 0)
            {
                firstLayerTabDataModelList.Sort((x, y) => x.Sort.CompareTo(y.Sort));
            }

            return firstLayerTabDataModelList;
        }

        public static void ChatWithOnShelfItemOwner(UInt64 itemOwnerGuid)
        {
            var functionData = TableManager.GetInstance().GetTableItem<FunctionUnLock>((int)FunctionUnLock.eFuncType.Friend);

            //表中数据不存在
            if (functionData == null)
            {
                Logger.LogErrorFormat("ChatWithItemOwner and FunctionUnlockData is null and id is {0}",
                    FunctionUnLock.eFuncType.Friend);
                return;
            }

            //等级不足，密聊功能没有解锁
            if (PlayerBaseData.GetInstance().Level < functionData.FinishLevel)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("relation_friend_chat_need_lv", functionData.FinishLevel));
                return;
            }

            //商品为自己上架，自己与自己无法密聊
            if (itemOwnerGuid == PlayerBaseData.GetInstance().RoleID)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("auction_new_disable_chat_with_self"));
                return;
            }

            var relationData = RelationDataManager.GetInstance().GetRelationByRoleID(itemOwnerGuid);
            //相关数据存在，好友或者陌生人
            if (relationData != null)
            {
                //直接密聊
                OpenChatFrame(relationData);
                return;
            }

            //相关数据不存在的时候，向服务器请求相关数据，获得数据之后再打开密聊界面
            OtherPlayerInfoManager.GetInstance().SendWatchOnShelfItemOwnerInfo(itemOwnerGuid);

        }

        public static void OpenChatFrame(RelationData relationData)
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<RelationFrameNew>())
            {
                RelationDataManager.GetInstance().OnAddPriChatList(relationData, false);
                RelationFrameData relationFrameData = new RelationFrameData();
                relationFrameData.eCurrentRelationData = relationData;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTAPStartTalk, relationFrameData);
            }
            else
            {
                ChatManager.GetInstance().OpenPrivateChatFrame(relationData);
            }
        }

        #region AuctionNewItemIdList

        //排序在售物品的顺序
        public static void SortOnSaleItemList(List<AuctionItemBaseInfo> auctionBaseInfoList)
        {
            if (auctionBaseInfoList == null || auctionBaseInfoList.Count <= 1)
                return;

            var onSaleItemTableList = new List<ItemTable>();
            for (var i = 0; i < auctionBaseInfoList.Count; i++)
            {
                if (auctionBaseInfoList[i] != null)
                {
                    var itemId = auctionBaseInfoList[i].itemTypeId;
                    var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>((int)itemId);
                    if (itemTableData != null)
                        onSaleItemTableList.Add(itemTableData);
                }
            }
            //排序在售物品的顺序
            SortOnSaleItemTableList(onSaleItemTableList);

            List<AuctionItemBaseInfo> newAuctionBaseInfoList = new List<AuctionItemBaseInfo>();
            for (var i = 0; i < onSaleItemTableList.Count; i++)
            {
                for (var j = 0; j < auctionBaseInfoList.Count; j++)
                {
                    if (onSaleItemTableList[i].ID == auctionBaseInfoList[j].itemTypeId)
                    {
                        newAuctionBaseInfoList.Add(auctionBaseInfoList[j]);
                        break;
                    }
                }
            }

            auctionBaseInfoList.Clear();
            onSaleItemTableList.Clear();
            for (var i = 0; i < newAuctionBaseInfoList.Count; i++)
            {
                auctionBaseInfoList.Add(newAuctionBaseInfoList[i]);
            }
            newAuctionBaseInfoList.Clear();

        }

        public static void SortOnSaleItemTableList(List<ItemTable> notOnSaleItemTableList)
        {
            if (notOnSaleItemTableList != null && notOnSaleItemTableList.Count > 0)
            {
                bool isEquipment = notOnSaleItemTableList[0] != null 
                                 && notOnSaleItemTableList[0].Type == ItemTable.eType.EQUIP;

                //装备排序
                if (isEquipment == true)
                {
                    //装备在售信息界面排序：绿色->粉色->紫色->蓝色->白色，
                    //同品质排序按装备等级由低到高
                    notOnSaleItemTableList.Sort((x, y) =>
                    {
                        if (x.Color == ItemTable.eColor.GREEN 
                            && y.Color == ItemTable.eColor.GREEN)
                        {
                            //等级，大 - 小
                            var a = -x.NeedLevel.CompareTo(y.NeedLevel);

                            //ID，小 - 大
                            if (a == 0)
                                a = x.ID.CompareTo(y.ID);

                            return a;
                        }
                        else if(x.Color == ItemTable.eColor.GREEN)
                        {
                            return -1;
                        }
                        else if (y.Color == ItemTable.eColor.GREEN)
                        {
                            return 1;
                        }
                        else
                        {
                            //品质, 高 - 低
                            var a = -x.Color.CompareTo(y.Color);

                            //等级，大 - 小
                            if (a == 0)
                                a = -x.NeedLevel.CompareTo(y.NeedLevel);

                            //ID，小 - 大
                            if (a == 0)
                                a = x.ID.CompareTo(y.ID);

                            return a;
                        }
                    });
                }
                else
                {
                    //材料+消耗品  也签内的物品在售信息界面排序，
                    //优先按品质由高到底排序，同品质按ID号由小到大排序
                    notOnSaleItemTableList.Sort((x, y) =>
                    {
                        //品质, 高 - 低
                        var a = -x.Color.CompareTo(y.Color);

                        //等级，大 - 小
                        if (a == 0)
                            a = -x.NeedLevel.CompareTo(y.NeedLevel);

                        //ID，小 - 大
                        if (a == 0)
                            a = x.ID.CompareTo(y.ID);

                        return a;
                    });
                }

            }
        }

        //按照单价进行排序
        public static void SortItemListBySinglePrice(List<AuctionBaseInfo> itemList)
        {
            if (itemList != null && itemList.Count > 0)
            {
                itemList.Sort((x, y) =>
                {
                    var singlePriceX = x.price;
                    if (x.num > 0)
                        singlePriceX /= x.num;

                    var singlePriceY = y.price;
                    if (y.num > 0)
                        singlePriceY /= y.num;

                    return singlePriceX.CompareTo(singlePriceY);
                });
            }
        }

        //按照成交的价格进行排序
        public static void SortItemListBySellRecordPrice(List<AuctionBaseInfo> itemList)
        {
            if (itemList == null || itemList.Count <= 0)
                return;

            itemList.Sort((x, y) =>
            {
                var singlePriceX = x.price;
                var singlePriceY = y.price;
                return singlePriceX.CompareTo(singlePriceY);
            });
            
        }

        public static bool IsItemOnSale(ItemTable itemTable, List<AuctionItemBaseInfo> auctionItemBaseInfoList)
        {
            if (auctionItemBaseInfoList == null || auctionItemBaseInfoList.Count <= 0)
                return false;

            for (var i = 0; i < auctionItemBaseInfoList.Count; i++)
            {
                var auctionItemBaseInfo = auctionItemBaseInfoList[i];
                if (auctionItemBaseInfo != null
                    && auctionItemBaseInfo.itemTypeId == (uint)itemTable.ID)
                    return true;
            }

            return false;
        }

        public static bool IsItemCanShow(ItemTable itemTable,
            AuctionNewFilterData filterData)
        {
            //展示Item
            if (filterData == null)
                return true;

            if (filterData.AuctionNewFilterTable == null || filterData.AuctionNewFilterTable.Parameter == null)
                return true;

            //等级过滤器
            if (filterData.FilterItemType == AuctionNewFrameTable.eFilterItemType.FIT_LEVEL)
            {
                if (filterData.AuctionNewFilterTable.Parameter.Count == 2)
                {
                    if (itemTable.NeedLevel >= filterData.AuctionNewFilterTable.Parameter[0]
                        && itemTable.NeedLevel <= filterData.AuctionNewFilterTable.Parameter[1])
                        return true;
                }
            }

            //品质过滤器
            if (filterData.FilterItemType == AuctionNewFrameTable.eFilterItemType.FIT_QUALITY)
            {
                for (var i = 0; i < filterData.AuctionNewFilterTable.Parameter.Length; i++)
                {
                    if ((int)itemTable.Color == filterData.AuctionNewFilterTable.Parameter[i])
                        return true;
                }
            }

            if (filterData.FilterItemType == AuctionNewFrameTable.eFilterItemType.FIT_SUCCEEDRAT)
            {
                var strengthenTicketTable = TableManager.GetInstance()
                    .GetTableItem<StrengthenTicketTable>(itemTable.StrenTicketDataIndex);
                if (strengthenTicketTable != null)
                {
                    if (filterData.AuctionNewFilterTable.Parameter.Count == 2)
                    {
                        var succeedRate = (int)(strengthenTicketTable.Probility * 0.1f);
                        if (succeedRate >= filterData.AuctionNewFilterTable.Parameter[0]
                            && succeedRate <= filterData.AuctionNewFilterTable.Parameter[1])
                            return true;
                    }
                }
            }

            //职业
            if (filterData.FilterItemType == AuctionNewFrameTable.eFilterItemType.FIT_JOB)
            {
                for (var i = 0; i < itemTable.Occu.Count; i++)
                {
                    //道具属于全职业，直接返回true
                    var curProfessionId = itemTable.Occu[i];
                    if (curProfessionId == 0)
                        return true;
                    else
                    {
                        //道具的基础职业满足职业过滤器
                        var baseJobId = Utility.GetBaseJobID(curProfessionId);
                        for (var j = 0; j < filterData.AuctionNewFilterTable.Parameter.Count; j++)
                        {
                            var filterProfessionId = filterData.AuctionNewFilterTable.Parameter[j];
                            if (curProfessionId == filterProfessionId)
                                return true;
                        }
                    }
                }
            }

            return false;
        }


        //道具表中的Type类型
        public static ItemTable.eType GetItemTableType(AuctionNewFrameTable.eMainItemType mainItemType)
        {
            if (mainItemType == AuctionNewFrameTable.eMainItemType.MIT_COST)
            {
                return ItemTable.eType.EXPENDABLE;
            }
            else if (mainItemType == AuctionNewFrameTable.eMainItemType.MIT_MATERIAL)
            {
                return ItemTable.eType.MATERIAL;
            }
            else
            {
                return ItemTable.eType.EQUIP;
            }
        }

        //auctionNewFrameTable表中的ID，对应的ItemList
        private static List<int> GetItemIdListByDataModel(ItemTable.eType itemTableType,
            AuctionNewFrameTable auctionNewFrameTable)
        {
            List<int> itemIdList = new List<int>();

            if (auctionNewFrameTable == null)
                return itemIdList;

            var subType = auctionNewFrameTable.SubType;
            var thirdType = auctionNewFrameTable.ThirdType;

            var subTypeNumber = subType.Length;
            //遍历SubType
            for (var i = 0; i < subTypeNumber; i++)
            {
                var curSubType = subType[i];
                //根据Type 和 subType 获得ItemIdList的列表
                var curItemIdList = ItemDataManager.GetInstance()
                    .GetAuctionItemListBaseFliter(itemTableType, (ItemTable.eSubType)curSubType);

                //ItemIdList 是空
                if (curItemIdList == null)
                    continue;

                //thirdType 和 SubType 没有一一对应
                if (i >= thirdType.Length)
                    continue;

                //对应列表中的第三类型是否相同
                var curThirdType = (ItemTable.eThirdType)thirdType[i];

                for (var j = 0; j < curItemIdList.Count; j++)
                {
                    var curItemTableId = curItemIdList[j];
                    var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(curItemTableId);

                    if (itemTableData == null)
                        continue;

                    //ThirdType可能为0的情况
                    if (itemTableData.ThirdType != curThirdType)
                        continue;

                    //宝珠类型
                    if (auctionNewFrameTable.SpecialParametersType == 1)
                    {
                        //宝珠类型：宝珠品质不符合
                        if (auctionNewFrameTable.SpecialParameters != (int)itemTableData.Color)
                            continue;

                    }
                    else if (auctionNewFrameTable.SpecialParametersType == 2)
                    {
                        //强化券类型，强化等级不符合
                        var strengthenTicketTable = TableManager.GetInstance()
                            .GetTableItem<StrengthenTicketTable>(itemTableData.StrenTicketDataIndex);
                        if (strengthenTicketTable == null || strengthenTicketTable.Level != auctionNewFrameTable.SpecialParameters)
                            continue;
                    }

                    #region Occu
                    //过滤职业相关数据
                    //全职业
                    var curAuctionJob = auctionNewFrameTable.Job;
                    if (curAuctionJob ==
                        AuctionNewFrameTable.eJob.AC_JOB_ALL)
                        itemIdList.Add(curItemTableId);
                    else
                    {
                        //具体的某个职业
                        if (itemTableData.Occu.Count >= 1 && itemTableData.Occu[0] != 0)
                        {
                            var bFind = false;
                            for (var k = 0; k < itemTableData.Occu.Count; k++)
                            {
                                var jobTableData = TableManager.GetInstance()
                                    .GetTableItem<JobTable>(itemTableData.Occu[k]);
                                if (jobTableData == null)
                                    continue;

                                if (curAuctionJob == (AuctionNewFrameTable.eJob)jobTableData.AuctionJob)
                                {
                                    bFind = true;
                                    break;
                                }
                            }

                            if (bFind == true)
                            {
                                itemIdList.Add(curItemTableId);
                            }
                        }
                    }
                    #endregion
                }
            }
            return itemIdList;
        }

        //去除某些SubType之后的ItemList
        private static List<int> GetItemIdListByDeleteSubTypeList(ItemTable.eType itemTableType,
            List<int> deleteSubTypeList)
        {
            //以SubType 为索引的列表
            var subTypeDict = ItemDataManager.GetInstance().GetAuctionItemListByItemType(itemTableType);
            if (subTypeDict == null)
                return null;

            List<int> deleteItemIdList = new List<int>();

            var subTypeDictIter = subTypeDict.GetEnumerator();
            while (subTypeDictIter.MoveNext())
            {
                var curSubType = subTypeDictIter.Current.Key;
                bool isDeleteSubType = false;
                for (var i = 0; i < deleteSubTypeList.Count; i++)
                {
                    if (deleteSubTypeList[i] == (int)curSubType)
                    {
                        isDeleteSubType = true;
                        break;
                    }
                }

                //需要删除某个SubType
                if (isDeleteSubType == true)
                    continue;

                //添加对应SubType的列表
                deleteItemIdList.AddRange(subTypeDictIter.Current.Value.ToArray());

            }
            return deleteItemIdList;
        }

        //根据三级的关系，获得ItemId的列表
        public static List<int> GetItemIdList(AuctionNewMenuTabDataModel firstLayerMenuTabDataModel,
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

            List<int> itemIdList = new List<int>();

            var itemTableType = GetItemTableType(firstLayerMenuTabDataModel.AuctionNewFrameTable.MainItemType);

            //使用第三层级的数据：需要注意职业，宝珠，强化类型
            if (thirdLayerMenuTabDataModel != null && thirdLayerMenuTabDataModel.AuctionNewFrameTable != null)
            {
                itemIdList = GetItemIdListByDataModel(itemTableType, thirdLayerMenuTabDataModel.AuctionNewFrameTable);
                return itemIdList;
            }

            //使用第二层级的数据：正常的数据，其他的数据
            //第二层级的正常数据
            if (secondLayerMenuTabDataModel.AuctionNewFrameTable.DeleteLayerID.Count == 0
                || (secondLayerMenuTabDataModel.AuctionNewFrameTable.DeleteLayerID.Count == 1
                    && secondLayerMenuTabDataModel.AuctionNewFrameTable.DeleteLayerID[0] == 0))
            {
                itemIdList = GetItemIdListByDataModel(itemTableType, secondLayerMenuTabDataModel.AuctionNewFrameTable);
                return itemIdList;
            }
            else
            {
                //排除正常数据的其他数据
                List<int> tempSubTypeList = new List<int>();
                var deleteLayerIdNumber = secondLayerMenuTabDataModel.AuctionNewFrameTable.DeleteLayerID.Count;
                //删除的Layer
                for (var i = 0; i < deleteLayerIdNumber; i++)
                {
                    var deleteLayerId = secondLayerMenuTabDataModel.AuctionNewFrameTable.DeleteLayerID[i];
                    var deleteAuctionNewFrameTable =
                        TableManager.GetInstance().GetTableItem<AuctionNewFrameTable>(deleteLayerId);

                    if(deleteAuctionNewFrameTable == null)
                        continue;
                    //删除Layer中的SubType
                    for (var j = 0; j < deleteAuctionNewFrameTable.SubType.Length; j++)
                    {
                        tempSubTypeList.Add(deleteAuctionNewFrameTable.SubType[j]);
                    }
                }

                //删除的子类型
                var deleteSubTypeList = tempSubTypeList.Distinct();

                itemIdList = GetItemIdListByDeleteSubTypeList(itemTableType, deleteSubTypeList.ToList());

                return itemIdList;
            }
        }

        //判断拍卖行珍品机制是否打开
        /// <summary>
        /// true 表示珍品机制打开，false表示珍品机制关闭
        /// </summary>
        /// <returns></returns>
        public static bool IsAuctionTreasureItemOpen()
        {
            //同步过来，返回true， 开关是关闭；没有同步过来，返回false，开关是打开的。
            //true 说明功能开关处于关闭状态
            //false 说明功能开关处在打开的状态
            if (ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_AUCTION_TREAS) == true)
                return false;

            return true;
        }

        #endregion

        #region StrengthenLevel

        public static int GetMagicCardStrengthenAddition(ItemData itemData)
        {
            if (itemData == null 
                || itemData.StrengthenLevel <= 0
                || itemData.Quality == ItemTable.eColor.CL_NONE)
                return 0;

            var strengthenLevel = itemData.StrengthenLevel;
            var itemColor = (int) itemData.Quality;

            var auctionMagicStrengthenAdditionTables =
                TableManager.GetInstance().GetTable<AuctionMagiStrengAdditTable>();

            var iter = auctionMagicStrengthenAdditionTables.GetEnumerator();
            while (iter.MoveNext())
            {
                var additionTable = iter.Current.Value as AuctionMagiStrengAdditTable;
                if (additionTable != null)
                {
                    if(additionTable.Type == 1)
                    {
                        if (additionTable.StrengthenLv == strengthenLevel
                            && additionTable.Color == itemColor)
                            return additionTable.StrengthenAddition;
                    }
                }
            }

            return 0;
        }

        #endregion

        #region ItemDataEquipType

        //设置拍卖行中装备类型和增幅的类型数值
        public static void UpdateItemDataByEquipType(ItemData itemData, AuctionBaseInfo auctionBaseInfo)
        {
            if (itemData == null || auctionBaseInfo == null)
                return;

            //设置装备的类型和增幅的类型
            itemData.EquipType = (EEquipType)auctionBaseInfo.equipType;
            //如果装备类型是红字装备，则设置装备的增幅类型和增幅数值
            if (itemData.EquipType == EEquipType.ET_REDMARK)
            {
                //自由红字装备才有增幅的数值
                itemData.GrowthAttrType = (EGrowthAttrType)auctionBaseInfo.enhanceType;
                //itemData.GrowthAttrNum = (int)auctionBaseInfo.strengthed;
                itemData.GrowthAttrNum = (int) auctionBaseInfo.enhanceNum;
            }
        }

        public static void UpdateItemDataByEquipType(ItemData itemData, byte equipType,
            byte enhanceType,
            int enhanceNum)
        {
            if (itemData == null)
                return;

            //设置装备的类型和增幅的类型
            itemData.EquipType = (EEquipType)equipType;
            //如果装备类型是红字装备，则设置装备的增幅类型和增幅数值
            if (itemData.EquipType == EEquipType.ET_REDMARK)
            {
                //自由红字装备才有增幅的数值
                itemData.GrowthAttrType = (EGrowthAttrType)enhanceType;
                itemData.GrowthAttrNum = enhanceNum;
            }
        }

        #endregion

        #region EquipPriceStrengthenCof
        //增幅装备的强化系数
        public static int GetRedEquipStrengthLvAdditionalPriceRate(ItemData itemData)
        {
            if (itemData == null
                || itemData.StrengthenLevel < 0
                || itemData.Quality == ItemTable.eColor.CL_NONE)
                return 0;

            var strengthenLevel = itemData.StrengthenLevel;
            var itemColor = (int)itemData.Quality;

            var auctionMagicStrengthenAdditionTables =
                TableManager.GetInstance().GetTable<AuctionMagiStrengAdditTable>();

            var iter = auctionMagicStrengthenAdditionTables.GetEnumerator();
            while (iter.MoveNext())
            {
                var additionTable = iter.Current.Value as AuctionMagiStrengAdditTable;
                if (additionTable != null)
                {
                    //增幅装备
                    if (additionTable.Type == 3)
                    {
                        if (additionTable.StrengthenLv == strengthenLevel
                            && additionTable.Color == itemColor)
                            return additionTable.StrengthenAddition;
                    }
                }
            }

            return 0;
        }

        //普通装备的强化系数
        public static int GetNormalEquipStrengthLvAdditionalPriceRate(int iStrengthLv)
        {
            //0 - 10 价格的系数为0;
            if (iStrengthLv <= 10)
            {
                return 0;
            }
            else if (iStrengthLv == 11)
            {
                return 30;
            }
            else if (iStrengthLv == 12)
            {
                return 60;
            }
            else if (iStrengthLv == 13)
            {
                return 150;
            }
            else if (iStrengthLv == 14)
            {
                return 300;
            }
            else if (iStrengthLv == 15)
            {
                return 400;
            }
            else if (iStrengthLv == 16)
            {
                return 500;
            }
            else if (iStrengthLv == 17)
            {
                return 600;
            }
            else if (iStrengthLv == 18)
            {
                return 700;
            }
            else if (iStrengthLv == 19)
            {
                return 800;
            }
            else if (iStrengthLv == 20)
            {
                return 900;
            }
            else
            {
                return 900;
            }
        }

        //推荐价格
        public static ulong GetBasePrice(ulong recommendPrice, int strengthenLevelRate)
        {
            return (ulong)((recommendPrice * (ulong)(100 + strengthenLevelRate)) / 100.0f);
        }

        #endregion

        #region ItemTime

        //道具是否为时效道具
        public static bool IsItemOwnerTimeValid(int itemId)
        {
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(itemId);
            //时效字段
            if (itemTable != null
                && itemTable.TimeLeft > 0)
            {
                return true;
            }

            return false;
        }

        //时效道具是否可以上线
        public static bool IsTimeItemCanOnShelf(uint endTime)
        {
            // 2天 = 2 * 24 * 60 * 60 = 172800
            if (endTime - TimeManager.GetInstance().GetServerTime() < 172800)
                return false;

            return true;
        }

        //时效道具是否在有效期内
        public static bool IsItemInValidTimeInterval(uint endTime)
        {
            if (endTime >= TimeManager.GetInstance().GetServerTime())
                return true;

            return false;
        }

        //时效道具当前剩余时间
        public static string GetTimeValidItemLeftTimeStr(uint endTime)
        {
            //1 天 = 24 * 60 * 60 = 86400
            var leftTime = endTime - TimeManager.GetInstance().GetServerTime();

            string leftTimeStr = "";
            //大于1天
            if (leftTime >= 86400)
            {
                var day = leftTime / 86400;
                var hour = (leftTime % 86400) / 3600;
                var dayHourStr = string.Format(TR.Value("auction_new_time_day_hour"), day, hour);
                leftTimeStr = string.Format(TR.Value("auction_new_item_is_valid_time"), dayHourStr);
            }
            else
            {
                var hour = leftTime / 3600;
                var minute = (leftTime % 3600) / 60;
                var hourMinuteStr = string.Format(TR.Value("auction_new_time_hour_minute"), hour, minute);
                leftTimeStr = string.Format(TR.Value("auction_new_item_is_valid_time"), hourMinuteStr);
            }

            return leftTimeStr;
        }

        #endregion

        #region AuctionNewBuyItem

        public static void OnCloseAuctionNewBuyItemFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<AuctionNewBuyItemFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<AuctionNewBuyItemFrame>();
        }

        public static void OnOpenAuctionNewBuyItemFrame(AuctionBaseInfo auctionBaseInfo)
        {
            OnCloseAuctionNewBuyItemFrame();

            AuctionNewBuyItemDataModel buyItemDataModel = new AuctionNewBuyItemDataModel();
            buyItemDataModel.SetBuyItemDataModel(auctionBaseInfo);

            ClientSystemManager.GetInstance().OpenFrame<AuctionNewBuyItemFrame>(FrameLayer.Middle,
                buyItemDataModel);
        }

        public static void OnShowItemTradeLimitFrame(string contentStr, Action onOkClick)
        {
            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = contentStr,
                IsShowNotify = false,
                LeftButtonText = TR.Value("common_data_cancel"),
                RightButtonText = TR.Value("common_data_sure"),
                OnRightButtonClickCallBack = onOkClick,
            };
            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);
        }

        #endregion

        public static int GetDays(int time)
        {
            return time / (24 * 3600);
        }

        public static bool IsItemForeverFreeze(int freezeTime)
        {
            var freezeDay = GetDays(freezeTime);
            if (freezeDay > AuctionNewDataManager.ItemForeverFreezeDays)
                return true;

            return false;
        }

        //得到一个格子上的物品总数
        public static int GetItemNumberByGuid(ulong itemGuid, bool bCheckAuctionCanSell = false)
        {
            int iNum = 0;

            Dictionary<ulong, ItemData> allPackageItems = ItemDataManager.GetInstance().GetAllPackageItems();
            if (allPackageItems == null)
            {
                return iNum;
            }

            foreach (var key in allPackageItems.Keys)
            {
                ItemData kItem = ItemDataManager.GetInstance().GetItem(key);

                if (kItem == null)
                {
                    continue;
                }

                if (bCheckAuctionCanSell)
                {
                    if (!kItem.IsItemInAuctionPackage())
                    {
                        continue;
                    }
                }

                if (kItem.GUID != itemGuid)
                {
                    continue;
                }

                iNum += kItem.Count;
            }

            return iNum;
        }



    }
}