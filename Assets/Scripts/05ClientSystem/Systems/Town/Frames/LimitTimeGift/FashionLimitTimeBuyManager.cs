using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using Protocol;
using ProtoTable;
using LimitTimeGift;

namespace FashionLimitTimeBuy
{
    public class FashionLimitTimeBuyManager : DataManager<FashionLimitTimeBuyManager>
    {
        //所有部位数据集合
        private Dictionary<uint, MallItemInfo> allLimitTimeFashionByIdDic;
        private Dictionary<int, List<MallItemInfo>> allLimitTimeFashionByTypeDic;

        //各部位显示用,缓存数据
        private List<MallItemInfo> typeLimitTimeFashionInfoList;
        private Dictionary<int, List<MallItemInfo>> typeLimitTimeFashionInfoDic;

        //时装套数据集合
        private Dictionary<uint, MallItemInfo> allFashionSuitByIdDic;
        private Dictionary<int, List<MallItemInfo>> allFashionSuitByTypeDic;

        //时装套显示用 缓存数据
        private List<MallItemInfo> typeFashionSuitInfoList;
        private Dictionary<int, List<MallItemInfo>> typeFashionSuitInfoDic;

        //时装套在商城展示限时数据 根据职业id
        private Dictionary<int , LimitTimeGiftData> mallFashionSuitsByOccIdDic;

        public bool haveFashionDiscount = false;
        //单项
        //public List<MallItemInfo> GetTypeLimitTimeFashionItemInfos()
        //{
        //    return typeLimitTimeFashionInfoList;
        //}
        //public Dictionary<int, List<MallItemInfo>> GetTypeLimitTimeFashionInfoByType()
        //{
        //    return typeLimitTimeFashionInfoDic;
        //}

        //时装部件 总数据
        public Dictionary<int, List<MallItemInfo>> GetAllLimtTimeFashionInfosByType()
        {
            return allLimitTimeFashionByTypeDic;
        }
        public Dictionary<uint, MallItemInfo> GetAllLimitTimeFashionById()
        {
            return allLimitTimeFashionByIdDic;
        }

        //时装套 总数据
        public Dictionary<int, List<MallItemInfo>> GetAllFashionSuitsByType()
        {
            return allFashionSuitByTypeDic;
        }
        public Dictionary<uint, MallItemInfo> GetAllFashionSuitsById()
        {
            return allFashionSuitByIdDic;
        }

        //职业id + 展示用数据
        public Dictionary<int, LimitTimeGiftData> GetMallFashionSuitsByOccIdDic()
        {
            return mallFashionSuitsByOccIdDic;
        }

        /// <summary>
        ///     根据数据类型 获取内容items ids
        /// </summary>
        /// <param name="itemInfo"></param>
        /// <param name="getIndex"> 时装商城当前 subType index 不是thirdIndex </param>
        /// <returns></returns>
        public uint[] TryGetItemIdsInMallItemInfo(MallItemInfo itemInfo, int getIndex = 0)
        {
            uint[] itemids = new uint[1] { 0 };
            if (itemInfo == null)
                return itemids;
            if (itemInfo.itemid != 0)
            {
                itemids = new uint[1] { itemInfo.itemid };
                return itemids;
            }

            if (itemInfo.giftItems == null)
                return itemids;
            int count = itemInfo.giftItems.Length;
            itemids = new uint[count];
            if (getIndex < count)
            {
                if (getIndex == 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        var giftItem = itemInfo.giftItems[i];
                        if (giftItem != null)
                        {
                            itemids[i] = giftItem.id;
                        }
                    }
                    return itemids;
                }
                else if (getIndex > 0) //表正常下，不进入
                {
                    itemids = new uint[1] { itemInfo.itemid };
                    return itemids;
                }
            }

            return itemids;
        }

        public uint TryGetItemIdInMallItemInfo(MallItemInfo itemInfo, int getIndex = 0)
        {
            if (itemInfo == null)
                return 0;
            if (itemInfo.itemid != 0)
            {
                return itemInfo.itemid;
            }

            if (itemInfo.giftItems == null)
                return 0;
            int count = itemInfo.giftItems.Length;
            if (getIndex < count)
            {
                if (getIndex == 0)
                {
                    var giftItem = itemInfo.giftItems[getIndex];
                    if (giftItem != null)
                    {
                        return giftItem.id;
                    }
                }
                else if (getIndex > 0) //表正常下，不进入
                {
                    return itemInfo.itemid;
                }
            }
            return 0;
        }


        public override void Initialize()
        {
            //部位时装
            typeLimitTimeFashionInfoList = new List<MallItemInfo>();
            typeLimitTimeFashionInfoDic = new Dictionary<int, List<MallItemInfo>>();

            allLimitTimeFashionByIdDic = new Dictionary<uint, MallItemInfo>();
            allLimitTimeFashionByTypeDic = new Dictionary<int, List<MallItemInfo>>();

            //时装套
            allFashionSuitByIdDic = new Dictionary<uint, MallItemInfo>();
            allFashionSuitByTypeDic = new Dictionary<int, List<MallItemInfo>>();

            typeFashionSuitInfoList = new List<MallItemInfo>();
            typeFashionSuitInfoDic = new Dictionary<int, List<MallItemInfo>>();

            //时装套商城展示数据
            mallFashionSuitsByOccIdDic = new Dictionary<int, LimitTimeGiftData>();
        }

        public override void Clear()
        {
            if (typeLimitTimeFashionInfoList != null)
            {
                typeLimitTimeFashionInfoList.Clear();
                typeLimitTimeFashionInfoList = null;
            }
            if (typeLimitTimeFashionInfoDic != null)
            {
                typeLimitTimeFashionInfoDic.Clear();
                typeLimitTimeFashionInfoDic = null;
            }

            if (allLimitTimeFashionByTypeDic != null)
            {
                allLimitTimeFashionByTypeDic.Clear();
                allLimitTimeFashionByTypeDic = null;
            }
            if (allLimitTimeFashionByIdDic != null)
            {
                allLimitTimeFashionByIdDic.Clear();
                allLimitTimeFashionByIdDic = null;
            }

             //时装套
            if (typeFashionSuitInfoList != null)
            {
                typeFashionSuitInfoList.Clear();
                typeFashionSuitInfoList = null;
            }
            if (typeFashionSuitInfoDic != null)
            {
                typeFashionSuitInfoDic.Clear();
                typeFashionSuitInfoDic = null;
            }

            if (allFashionSuitByIdDic != null)
            {
                allFashionSuitByIdDic.Clear();
                allFashionSuitByIdDic = null;
            }
            if (allFashionSuitByTypeDic != null)
            {
                allFashionSuitByTypeDic.Clear();
                allFashionSuitByTypeDic = null;
            }

            if (mallFashionSuitsByOccIdDic != null)
            {
                mallFashionSuitsByOccIdDic.Clear();
                mallFashionSuitsByOccIdDic = null;
            }
        }

        //public LimitTimeGiftData TryGetUpWearItemIdInFashionSuit(MallItemInfo itemInfo, FashionMallMainIndex fashionTypeIndex)
        //{
        //    LimitTimeGiftData giftData = new LimitTimeGiftData();
        //    if (fashionTypeIndex == FashionMallMainIndex.FashionAll)
        //    {
        //        if (itemInfo != null)
        //        {
        //            if (itemInfo.giftItems != null && itemInfo.giftItems.Length == (int)MallTypeTable.eMallSubType.MST_ALL - 1)
        //            {
        //                int upWearIndex = (int)MallTypeTable.eMallSubType.MST_UPWEAR - 1;
        //                giftData = ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.SyncMallItemInfoToLimitTimeGift(itemInfo);
        //                if (giftData != null)
        //                {
        //                    giftData.GiftId = itemInfo.giftItems[upWearIndex].id;
        //                }
        //                return giftData;
        //            }
        //        }
        //    }
        //    return giftData;
        //}

        public uint TryGetUpWearItemIdInFashionSuit(MallItemInfo itemInfo, FashionMallMainIndex fashionTypeIndex)
        {
            if (itemInfo == null)
                return 0;
            if (fashionTypeIndex == FashionMallMainIndex.FashionAll)
            {
                if (itemInfo != null)
                {
                    if (itemInfo.giftItems != null && itemInfo.giftItems.Length == (int)MallTypeTable.eMallSubType.MST_ALL - 1)
                    {
                        int upWearIndex = (int)MallTypeTable.eMallSubType.MST_UPWEAR - 1;
                        return itemInfo.giftItems[upWearIndex].id;
                    }
                }
            }
            return itemInfo.itemid;
        }

        public void ResetItemNameColor(ItemTable item , UnityEngine.UI.Text text)
        {
            if (text == null)
                return;
            string colorStr = "white";
            if (item != null)
            {
                colorStr = Parser.ItemParser.GetItemColor(item);
            }
            string textStr = text.text;
            text.text = string.Format("<color={0}>", colorStr) + textStr + "</color>";
        }

        public override EEnterGameOrder GetOrder()
        {
            return base.GetOrder();
        }

        /// <summary>
        /// 时装过滤  时装部件 时装套
        /// </summary>
        /// <param name="allMallItems"></param>
        /// <returns></returns>
        public MallItemInfo[] FashionLimitTimeFilter(MallItemInfo[] allMallItems)
        {
            if (allMallItems != null)
            {
                if( allMallItems.Length <= 0)
                    return allMallItems;
                if (allMallItems[0].gift != (int)MallGoodsType.COMMON_CHOOSE_ONE)
                    return allMallItems;
                //属于礼包集合类型  多选一 
                if (allMallItems[0].itemid == 0)        
                {
                    var filterItems = FashionSuitFilter(allMallItems);
                    return filterItems;
                }

                //这时候才是时装数据进来了
                if (typeLimitTimeFashionInfoList == null || typeLimitTimeFashionInfoDic == null)
                    return null;
                typeLimitTimeFashionInfoList.Clear();
                typeLimitTimeFashionInfoDic.Clear();
                //遍历后得到普通商品（时装）多选一购买
                List<MallItemInfo> allMallItemList = new List<MallItemInfo>(allMallItems);
                for (int i = 0; i < allMallItemList.Count; i++)
                {
                    var mallItem = allMallItemList[i];
                    if (mallItem != null)
                    {
                        if (mallItem.gift == (int)MallGoodsType.COMMON_CHOOSE_ONE)
                        {
                            typeLimitTimeFashionInfoList.Add(mallItem);

                            //保存各部位时装数据 根据Id
                            if (allLimitTimeFashionByIdDic != null)
                            {
                                if (allLimitTimeFashionByIdDic.ContainsKey(mallItem.id))
                                    allLimitTimeFashionByIdDic[mallItem.id] = mallItem;
                                else
                                    allLimitTimeFashionByIdDic.Add(mallItem.id, mallItem);
                            }
                        }
                    }
                }

                //按照多选一商品（时装）的类型 保存到字典
                if (typeLimitTimeFashionInfoList != null)
                {
                    for (int i = 0; i < typeLimitTimeFashionInfoList.Count; i++)
                    {
                        MallItemInfo limitTimeFashionInfo = typeLimitTimeFashionInfoList[i];
                        int typeKey = limitTimeFashionInfo.goodsSubType;
                        if (typeLimitTimeFashionInfoDic.ContainsKey(typeKey))
                        {
                            var typeFashionList = typeLimitTimeFashionInfoDic[typeKey];
                            if (typeFashionList != null)
                            {
                                if (typeFashionList.Contains(limitTimeFashionInfo))
                                {
                                    typeFashionList.Remove(limitTimeFashionInfo);
                                }
                                typeFashionList.Add(limitTimeFashionInfo);
                            }
                        }                        
                        else
                        {
                            typeLimitTimeFashionInfoDic.Add(typeKey, new List<MallItemInfo>() { limitTimeFashionInfo });
                        }

                        //保存各部位时装数据 根据good type 
                        if (allLimitTimeFashionByTypeDic != null)
                        {
                            if (allLimitTimeFashionByTypeDic.ContainsKey(typeKey))
                                allLimitTimeFashionByTypeDic[typeKey] = typeLimitTimeFashionInfoDic[typeKey];
                            else
                                allLimitTimeFashionByTypeDic.Add(typeKey, typeLimitTimeFashionInfoDic[typeKey]);
                        }
                    }

                    //Debug.Log("FashionLimitTimeFilter sync typeLimitTimeFashionInfoDic count = " + typeLimitTimeFashionInfoDic.Count);
                }

                //移除多选一商品（时装），只保留一个返回，用于显示到商城主界面上
                List<MallItemInfo> noShowItemInfoList = new List<MallItemInfo>();
                if (typeLimitTimeFashionInfoDic != null)
                {
                    var typeDicEnum = typeLimitTimeFashionInfoDic.GetEnumerator();
                    while (typeDicEnum.MoveNext())
                    {
                        var keyValue = typeDicEnum.Current;
                        if (!keyValue.Equals(new KeyValuePair<int,List<MallItemInfo>>()))
                        {
                            var valueList = keyValue.Value;
                            if (valueList != null)
                            {
                                for (int i = 0; i < valueList.Count; i++)
                                {
                                    //最后一个用于显示，其余保存，用于点选后选择不同期限
                                    var noShowItem = FilterNotShowItemByTimeLimit(valueList[i]);
                                    if (noShowItem != null)
                                    {
                                        noShowItemInfoList.Add(noShowItem);
                                    }
                                }
                            }
                        }
                    }
                }
                if (noShowItemInfoList.Count > 0)
                {
                    for (int j = 0; j < noShowItemInfoList.Count; j++)
                    {
                        for (int i = 0; i < allMallItemList.Count; i++)
                        {
                            if (allMallItemList[i].id.Equals(noShowItemInfoList[j].id))
                            {
                                allMallItemList.RemoveAt(i);
                                break;//跳出当前一个循环
                            }
                        }
                    }
                }

                return allMallItemList.ToArray();
            }
            return allMallItems;
        }

        MallItemInfo FilterNotShowItemByTimeLimit(MallItemInfo item)
        {
            if (item != null)
            {
                var itemInTable = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)item.itemid);
                if (itemInTable != null)
                {
                    //显示价格最小的
                    if (itemInTable.TimeLeft != 7 * 24 * 3600)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 时装套
        /// </summary>
        /// <param name="mallItems"></param>
        /// <returns></returns>
        MallItemInfo[] FashionSuitFilter(MallItemInfo[] mallItems)
        {
            if (mallItems != null)
            {
                if (mallItems.Length <= 0)
                    return mallItems;
                if (mallItems[0].gift != (int)MallGoodsType.COMMON_CHOOSE_ONE)
                    return mallItems;

                //这时候才是时装数据进来了
                if (typeFashionSuitInfoList == null || typeFashionSuitInfoDic == null)
                    return null;
                typeFashionSuitInfoList.Clear();
                typeFashionSuitInfoDic.Clear();
                //遍历后得到普通商品（时装）多选一购买
                List<MallItemInfo> allMallItemList = new List<MallItemInfo>(mallItems);
                for (int i = 0; i < allMallItemList.Count; i++)
                {
                    var mallItem = allMallItemList[i];
                    if (mallItem != null)
                    {
                        if (mallItem.gift == (int)MallGoodsType.COMMON_CHOOSE_ONE)
                        {
                            typeFashionSuitInfoList.Add(mallItem);

                            //保存各部位时装数据 根据Id
                            if (allFashionSuitByIdDic != null)
                            {
                                if (allFashionSuitByIdDic.ContainsKey(mallItem.id))
                                    allFashionSuitByIdDic[mallItem.id] = mallItem;
                                else
                                    allFashionSuitByIdDic.Add(mallItem.id, mallItem);
                            }
                        }
                    }
                }

                //按照多选一商品（时装）的类型 保存到字典
                if (typeFashionSuitInfoList != null)
                {
                    for (int i = 0; i < typeFashionSuitInfoList.Count; i++)
                    {
                        MallItemInfo limitTimeFashionInfo = typeFashionSuitInfoList[i];
                        int typeKey = limitTimeFashionInfo.goodsSubType;
                        if (typeFashionSuitInfoDic.ContainsKey(typeKey))
                        {
                            var typeFashionList = typeFashionSuitInfoDic[typeKey];
                            if (typeFashionList != null)
                            {
                                if (typeFashionList.Contains(limitTimeFashionInfo))
                                {
                                    typeFashionList.Remove(limitTimeFashionInfo);
                                }
                                typeFashionList.Add(limitTimeFashionInfo);
                            }
                        }
                        else
                        {
                            typeFashionSuitInfoDic.Add(typeKey, new List<MallItemInfo>() { limitTimeFashionInfo });
                        }

                        //保存各部位时装数据 根据good type 
                        if (allFashionSuitByTypeDic != null)
                        {
                            if (allFashionSuitByTypeDic.ContainsKey(typeKey))
                                allFashionSuitByTypeDic[typeKey] = typeFashionSuitInfoDic[typeKey];
                            else
                                allFashionSuitByTypeDic.Add(typeKey, typeFashionSuitInfoDic[typeKey]);
                        }
                    }

                    //Debug.Log("FashionLimitTimeFilter sync typeLimitTimeFashionInfoDic count = " + typeLimitTimeFashionInfoDic.Count);
                }

                //移除多选一商品（时装），只保留一个返回，用于显示到商城主界面上
                List<MallItemInfo> noShowItemInfoList = new List<MallItemInfo>();
                if (typeFashionSuitInfoDic != null)
                {
                    var typeDicEnum = typeFashionSuitInfoDic.GetEnumerator();
                    while (typeDicEnum.MoveNext())
                    {
                        var keyValue = typeDicEnum.Current;
                        if (!keyValue.Equals(new KeyValuePair<int, List<MallItemInfo>>()))
                        {
                            var valueList = keyValue.Value;
                            if (valueList != null)
                            {
                                for (int i = 0; i < valueList.Count; i++)
                                {
                                    //最后一个用于显示，其余保存，用于点选后选择不同期限
                                    var noShowItem = FilterNotShowItem(valueList[i]);
                                    if (noShowItem != null)
                                    {
                                        noShowItemInfoList.Add(noShowItem);
                                    }
                                }
                            }
                        }
                    }
                }
                if (noShowItemInfoList.Count > 0)
                {
                    for (int j = 0; j < noShowItemInfoList.Count; j++)
                    {
                        for (int i = 0; i < allMallItemList.Count; i++)
                        {
                            if (allMallItemList[i].id.Equals(noShowItemInfoList[j].id))
                            {
                                allMallItemList.RemoveAt(i);
                                break;//跳出当前一个循环
                            }
                        }
                    }
                }

                return allMallItemList.ToArray();
            }
            return mallItems;
        }

        MallItemInfo FilterNotShowItem(MallItemInfo item)
        {
            if (item != null)
            {
                var itemContent = item.giftItems;
                ItemReward itemUpWear = null;
                if (itemContent != null && itemContent.Length == (int)MallTypeTable.eMallSubType.MST_ALL - 1)
                {
                    itemUpWear = itemContent[(int)MallTypeTable.eMallSubType.MST_UPWEAR - 1];
                }
                if (itemUpWear != null)
                {
                    var itemInTable = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)itemUpWear.id);
                    if (itemInTable != null)
                    {
                        //显示价格最小的
                        if (itemInTable.TimeLeft != 7 * 24 * 3600)
                        {
                            return item;
                        }
                    }
                }
            }
            return null;
        }
    }

    /// <summary>
    /// 选择时装集合 - 根据goodType区分 包括（头饰、上衣、项链、下身、腰饰）中各一类的集合
    /// </summary>
    public class SelectMallItemInfoData
    {
        private int totalPrice;
        private int FashionDiscountPrice;
        private float Discount;
        private int FashionDiscount;
        /// <summary>
        /// 当前选中时装集合的价格 - 只分为 所有选择部位  对应7天的价格  30天的价格   永久的价格 这三种
        /// </summary>
        public int TotalPrice
        {
            get {
                if (selectItemInfos != null)
                {
                    totalPrice = 0;
                    for (int i = 0; i < selectItemInfos.Count; i++)
                    {
                        totalPrice += (int)selectItemInfos[i].discountprice;
                    }
                    return totalPrice;
                }
                return 0;
            }
        }

        public float _Discount
        {
            get
            {
                if (selectItemInfos != null)
                {
                    for (int i = 0; i < selectItemInfos.Count; i++)
                    {
                        Discount = selectItemInfos[i].discountRate * 1.0f / 10;
                        if (Discount != 0)
                        {
                            return Discount;
                        }
                    }

                }
                return 0;
            }
        }
        public int _FashionDiscountPrice
        {
            get
            {
                if(selectItemInfos !=null)
                {
                    for(int i=0;i<selectItemInfos.Count;i++)
                    {
                        FashionDiscountPrice += (int)selectItemInfos[i].price;
                    }
                    return FashionDiscountPrice;
                }
                return 0;
            }
        }

        

        private ItemTable.eSubType moneyType;
        public ItemTable.eSubType MoneyType
        {
            get {
                if (selectItemInfos != null)
                {
                    if (selectItemInfos.Count > 0)
                        return (ItemTable.eSubType)selectItemInfos[0].moneytype;
                }
                return ItemTable.eSubType.ST_NONE;
            }
        }

        private FashionMallMainIndex fashionTypeIndex;
        public FashionMallMainIndex FashionTypeIndex
        {
            get { return fashionTypeIndex; }
            set { fashionTypeIndex = value; }
        }

        private List<MallItemInfo> selectItemInfos;
        public List<MallItemInfo> SelectItemInfos
        {
            get { return selectItemInfos; }
            set { selectItemInfos = value; }
        }

        private List<MallItemInfo> allTypeItemInfos;
        public List<MallItemInfo> AllTypeItemInfos
        {
            get { return allTypeItemInfos; }
            set { allTypeItemInfos = value; }
        }

        /// <summary>
        /// 积分倍数
        /// </summary>
        public int multiple
        {
            get
            {
                if (selectItemInfos != null)
                {
                    if (selectItemInfos.Count > 0)
                        return selectItemInfos[0].multiple;
                }

                return 0;
            }
        }

        /// <summary>
        /// 积分时间戳
        /// </summary>
        public uint multipleEndTime
        {
            get
            {
                if (selectItemInfos != null)
                {
                    if (selectItemInfos.Count > 0)
                        return selectItemInfos[0].multipleEndTime;
                }

                return 0;
            }
        }

        public void Reset()
        {
            totalPrice = 0;
            FashionDiscountPrice = 0;
            selectItemInfos = new List<MallItemInfo>();
            allTypeItemInfos = new List<MallItemInfo>();
        }

        public SelectMallItemInfoData()
        {
            Reset();
        }
    }

    public enum FashionLimitTime
    {
        Invalid,
        SevenDay,
        OneMonth,
        Forever
    }
}
