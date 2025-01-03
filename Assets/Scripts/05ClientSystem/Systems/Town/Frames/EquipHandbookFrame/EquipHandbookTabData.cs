using ProtoTable;
using System;
using System.Collections.Generic;

namespace GameClient
{
    public class EquipHandbookTabData : System.IComparable<EquipHandbookTabData>
    {
        public EquipHandbookTabData(EquipHandbookContentTable data)
        {
            if (null == data)
            {
                return;
            }

            id = data.ID;
            sortOrder = data.SortOrder;
            type = data.Type;
            name = data.Name;
            isDefaultTab = data.IsDefaultTab;
            isShowEquipScore = data.IsShowEquipScore;
            isFilterWithLevel = data.IsFilterWithLevel;
            isFilterWithEquipScore = data.IsFilterWithEquipScore;
        }

        public int id { get; private set; }
        public int sortOrder { get; private set; }
        public string name { get; private set; }
        public EquipHandbookContentTable.eType type { get; private set; }

        public bool isDefaultTab { get; private set; }
        public bool isShowEquipScore { get; private set; }
        public bool isFilterWithLevel { get; private set; }
        public bool isFilterWithEquipScore { get; private set; }

        /// <summary>
        /// 筛选之后的数据
        /// </summary>
        private List<EquipHandbookTabCollectionData> mFilterIDs = new List<EquipHandbookTabCollectionData>();
        /// <summary>
        /// 表格中的数据
        /// </summary>
        private List<EquipHandbookTabCollectionData> mCollectIDs = new List<EquipHandbookTabCollectionData>();

        public List<EquipHandbookTabCollectionData> collectIDs { get { return mFilterIDs; } }

        public List<EquipHandbookEquipItemData> GetRecommendedCollect(int weaponPartScore, int armorPartScore, int jewelryPartScore, ref bool isBest)
        {
            List<EquipHandbookEquipItemData> mAllEquipItemList = new List<EquipHandbookEquipItemData>();
            bool isBestWeapon = false;
            EquipHandbookTabCollectionData mWeaponData= _findFirstHighScore(EquipHandbookCollectionTable.eScreenType.eWeapon, weaponPartScore, ref isBestWeapon);
            if (mWeaponData != null)
            {
                mAllEquipItemList.Add(mWeaponData.pickOne(weaponPartScore));
            }

            bool isBestArmor = false;
            EquipHandbookTabCollectionData mArmorData = _findFirstHighScore(EquipHandbookCollectionTable.eScreenType.eArmor, armorPartScore, ref isBestArmor);
            if (mArmorData != null)
            {
                mAllEquipItemList.AddRange(mArmorData.itemIDs);
            }
            bool isBestJewelry = false;
            EquipHandbookTabCollectionData mJewelryData = _findFirstHighScore(EquipHandbookCollectionTable.eScreenType.eJewelry, jewelryPartScore, ref isBestJewelry);
            if (mJewelryData != null)
            {
                mAllEquipItemList.AddRange(mJewelryData.itemIDs);
            }

            isBest = isBestArmor && isBestJewelry && isBestWeapon;

            return mAllEquipItemList;
        }

        /// <summary>
        /// 找到第一个比玩家相对部位score高的集合
        /// </summary>
        /// <param name="part"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        private EquipHandbookTabCollectionData _findFirstHighScore(EquipHandbookCollectionTable.eScreenType partType, int score, ref bool isBest)
        {
            isBest = false;

            int currentLevel = EquipHandbookDataManager.GetInstance().GetSplitLevel(mFilterIDs);
            int MaxScoreIndex = 0;
            int MaxScore = 0;

            for (int i = 0; i < mFilterIDs.Count; ++i)
            {
                if (mFilterIDs[i].partScreenType != partType)
                {
                    continue;
                }

                if (mFilterIDs[i].sumEquipCollectScore > MaxScore)
                {
                    MaxScoreIndex = i;
                    MaxScore = mFilterIDs[i].sumEquipCollectScore;
                }

                if (mFilterIDs[i].sumEquipCollectScore > score && currentLevel == mFilterIDs[i].level)
                {
                    return mFilterIDs[i];
                }
                
            }

            isBest = true;
            return mFilterIDs[MaxScoreIndex];
        }
        
        /// <summary>
        /// 得到最低评分item的集合
        /// </summary>
        /// <returns></returns>
        public List<EquipHandbookEquipItemData> GetLowestScoreItemList()
        {
            List<EquipHandbookEquipItemData> mAllEquipItemList = new List<EquipHandbookEquipItemData>();

            EquipHandbookTabCollectionData mWeaponData = _findLowestScore(EquipHandbookCollectionTable.eScreenType.eWeapon);
            if (mWeaponData != null)
            {
                mAllEquipItemList.Add(mWeaponData.itemIDs[0]);
            }
            
            EquipHandbookTabCollectionData mArmorData = _findLowestScore(EquipHandbookCollectionTable.eScreenType.eArmor);
            if (mArmorData != null)
            {
                mAllEquipItemList.AddRange(mArmorData.itemIDs);
            }

            EquipHandbookTabCollectionData mJewelryData = _findLowestScore(EquipHandbookCollectionTable.eScreenType.eJewelry);
            if (mJewelryData != null)
            {
                mAllEquipItemList.AddRange(mJewelryData.itemIDs);
            }

            return mAllEquipItemList;
        }
        /// <summary>
        /// 找到最低评分的集合
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        private EquipHandbookTabCollectionData _findLowestScore(EquipHandbookCollectionTable.eScreenType part)
        {
            int currentLevel = EquipHandbookDataManager.GetInstance().GetSplitLevel(mFilterIDs);

            for (int i = 0; i < mFilterIDs.Count; ++i)
            {
                if (mFilterIDs[i].partScreenType == part && currentLevel == mFilterIDs[i].level)
                {
                    return mFilterIDs[i];
                }
            }

            return null;
        }

        public void AddTabCollectionData(EquipHandbookTabCollectionData data)
        {
            if (null == data)
            {
                return;
            }

            mCollectIDs.Add(data);
        }

        public void SortTabCollectionDatas()
        {
            mCollectIDs.Sort();
        }

        public void CalculateEquipScore()
        {
            if (!isShowEquipScore)
            {
                return;
            }

            for (int i = 0; i < mFilterIDs.Count; i++)
            {
                EquipHandbookTabCollectionData data = mFilterIDs[i];

                if (null != data)
                {
                    List<EquipHandbookEquipItemData> filterData = data.itemIDs;

                    for (int j = 0; j < filterData.Count; j++)
                    {
                        filterData[j].CalculateBaseScore();
                    }
                }
            }
        }

        public List<EquipHandbookTabCollectionData> FilterWithCondition()
        {
            mFilterIDs.Clear();
            mFilterIDs.AddRange(mCollectIDs);
            mFilterIDs.RemoveAll(_filterCondition);
            return mFilterIDs;
        }

        private bool _filterCondition(EquipHandbookTabCollectionData data)
        {
            if (!data.isFitOccupation)
            {
                return true;
            }

            if (isFilterWithLevel && !data.isFitLevel)
            {
                return true;
            }

            return false;
        }

        public List<EquipHandbookTabCollectionData> FilterItemWithCondition()
        {
            mFilterIDs.RemoveAll(_filterItemCondition);
            return mFilterIDs;
        }

        private bool _filterItemCondition(EquipHandbookTabCollectionData data)
        {
            if (data.itemIDs.Count <= 0)
            {
                return true;
            }
            
            return false;
        }

        public ComCommonBind bind { get; set; }

        public int CompareTo(EquipHandbookTabData other)
        {
            if (sortOrder == other.sortOrder)
            {
                return id - other.id;
            }

            return sortOrder - other.sortOrder;
        }

        /// <summary>
        /// 是否包含指定道具ID
        /// </summary>
        /// <param name="itemTableId"></param>
        /// <returns></returns>
        public bool IsContainItem(int itemTableId)
        {
            if (mCollectIDs == null || mCollectIDs.Count <= 0)
            {
                return false;
            }

            for (int i = 0; i < mCollectIDs.Count; i++)
            {
                var collectId = mCollectIDs[i];
                if (collectId == null)
                    continue;
                var itemIds = collectId.itemIDs;
                if (itemIds == null || itemIds.Count <= 0)
                    continue;
                for (int j = 0; j < itemIds.Count; j++)
                {
                    var itemId = itemIds[j];
                    if (itemId == null)
                        continue;
                    if (itemId.id == itemTableId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public class EquipHandbookTabCollectionData : System.IComparable<EquipHandbookTabCollectionData>
    {
        public System.Collections.IEnumerator GetWeaponSplitIterator()
        {
            if (partScreenType != EquipHandbookCollectionTable.eScreenType.eWeapon)
            {
                yield break;
            }

            for (int i = 0; i < mItemIDs.Count; ++i)
            {
                if (mItemIDs[i].id != 0)
                {
                    EquipHandbookTabCollectionData iterData = new EquipHandbookTabCollectionData(this);
                    iterData.mItemIDs.Clear();
                    iterData.mItemIDs.Add(mItemIDs[i]);
                    yield return iterData;
                }
            }
        }

        protected EquipHandbookTabCollectionData(EquipHandbookTabCollectionData data)
        {
            if (null == data)
            {
                Logger.LogErrorFormat("[EquipHandbook] 初始化参数为空");

                return;
            }

            partScreenType = data.partScreenType;
            id = data.id;
            level = data.level;
            name = data.name;
            order = data.order;
            occopationLimitType = data.occopationLimitType;
            fitOccupations.AddRange(data.fitOccupations);
        }

        public EquipHandbookTabCollectionData(EquipHandbookCollectionTable data)
        {
            if (null == data)
            {
                Logger.LogErrorFormat("[EquipHandbook] 初始化参数为空");
                return;
            }
            partScreenType = data.ScreenType;
            id = data.ID;
            level = data.Level;
            name = data.Name;
            order = data.SortOrder;
            occopationLimitType = data.OccopationLimitType;


            switch (occopationLimitType)
            {
                case EquipHandbookCollectionTable.eOccopationLimitType.eAccordingAttachedItem:
                    break;
                case EquipHandbookCollectionTable.eOccopationLimitType.eAccordingOccuptionLimit:
                    fitOccupations.AddRange(data.OccopationLimit);
                    break;
            }

            switch (data.Type)
            {
                case EquipHandbookCollectionTable.eType.eCustom:
                    for (int i = 0; i < data.CustomEquipIDs.Count; ++i)
                    {
                        EquipHandbookEquipItemData equipItemData = new EquipHandbookEquipItemData(data.CustomEquipIDs[i]);
                        equipItemData.CalculateBaseScore();
                        mItemIDs.Add(equipItemData);

                    }
                    break;
                case EquipHandbookCollectionTable.eType.eEquipSuit:

                    EquipSuitTable suitTable = TableManager.instance.GetTableItem<EquipSuitTable>(data.EquipSuitID);
                    if (null != suitTable)
                    {
                        for (int i = 0; i < suitTable.EquipIDs.Count; ++i)
                        {
                            EquipHandbookEquipItemData equipItemData = new EquipHandbookEquipItemData(suitTable.EquipIDs[i]);
                            equipItemData.CalculateBaseScore();
                            mItemIDs.Add(equipItemData);
                        }
                        name = suitTable.Name;
                    }
                    else
                    {
                        Logger.LogErrorFormat("[EquipHandbook] 装备图鉴集合表格 ID为{0}，找不到{1}的套装",data.ID,data.EquipSuitID);
                    }
                    break;
                default:
                    break;
            }
        }
        public EquipHandbookCollectionTable.eScreenType partScreenType { get; private set; }
        public int id { get; private set; }
        public int level { get; private set; }
        public string name { get; private set; }
        public int order { get; private set; }

        /// <summary>
        /// 适用职业ID
        /// </summary>
        public List<int> fitOccupations = new List<int>();

        public EquipHandbookCollectionTable.eOccopationLimitType occopationLimitType { private set; get; }

        private List<EquipHandbookEquipItemData> mItemIDs = new List<EquipHandbookEquipItemData>();
        private List<EquipHandbookEquipItemData> mFilterItemIDs = new List<EquipHandbookEquipItemData>();

        public List<EquipHandbookEquipItemData> itemIDs
        {
            get
            {
                if (occopationLimitType == EquipHandbookCollectionTable.eOccopationLimitType.eAccordingOccuptionLimit)
                {
                    return mItemIDs;
                }
                else
                {
                    return mFilterItemIDs;
                }
            }
        }

        public EquipHandbookEquipItemData pickOne(int score)
        {
            for (int i = 0; i < itemIDs.Count; i++)
            {
                if (itemIDs[i].baseScore > score )
                {
                    return itemIDs[i];
                }
            }

            return itemIDs[itemIDs.Count - 1];
        }

        public List<EquipHandbookEquipItemData> FilterWithOccupation()
        {
            if (occopationLimitType == EquipHandbookCollectionTable.eOccopationLimitType.eAccordingOccuptionLimit)
            {
                return mItemIDs;
            }

            mFilterItemIDs.Clear();
            mFilterItemIDs.AddRange(mItemIDs);
            mFilterItemIDs.RemoveAll(_filterCondition);

            return mFilterItemIDs;
        }

        private bool _filterCondition(EquipHandbookEquipItemData item)
        {
            return !item.isFitOccupation;
        }


        public bool isFitOccupation
        {
            get
            {
                return EquipHandbookUtility.IsFitOccupation(fitOccupations);
            }
        }

        public bool isFitLevel
        {
            get
            {
                return (PlayerBaseData.GetInstance().Level) >= (level);
            }
        }
        
        public int sumEquipCollectScore
        {
            get
            {
                int sum = 0;

                for (int i = 0; i < itemIDs.Count; ++i)
                {
                    sum += itemIDs[i].baseScore;
                }

                return sum;
            }
        }

        public int CompareTo(EquipHandbookTabCollectionData other)
        {
            if (partScreenType != other.partScreenType)
            {
                return partScreenType - other.partScreenType;
            }

            if (partScreenType != EquipHandbookCollectionTable.eScreenType.eNull)
            {
                if (sumEquipCollectScore != other.sumEquipCollectScore)
                {
                    return sumEquipCollectScore - other.sumEquipCollectScore;
                }
            }

            if (level != other.level)
            {
                return level - other.level;
            }
            else
            {
                return order - other.order;
            }
        }
    }

    public class EquipHandbookEquipItemData : System.IComparable<EquipHandbookEquipItemData>
    {
        public EquipHandbookEquipItemData(int id)
        {
            this.id = id;
            bind    = null;

            EquipHandbookAttachedTable attach = TableManager.instance.GetTableItem<EquipHandbookAttachedTable>(id);
            if (null != attach)
            {
                fitOccupations.AddRange(attach.OccopationLimit);
                baseOccupation.AddRange(attach.BaseOccopationLimit);
            }
        }

        /// <summary>
        /// 适用职业ID
        /// </summary>
        public List<int> fitOccupations = new List<int>();
        public List<int> baseOccupation = new List<int>();
        public bool isFitOccupation
        {
            get
            {
                return EquipHandbookUtility.IsFitOccupation(fitOccupations, baseOccupation);
            }
        }

        public int id { private set; get; }

        public int baseScore { private set; get; }

        /// <summary>
        /// 计算基础分
        /// 不需要更新, 只需要执行一次
        /// </summary>
        public void CalculateBaseScore()
        {
            if (0 != baseScore)
            {
                return;
            }

            baseScore = ItemSourceInfoTableManager.GetInstance().GetItemBaseScore(id);
        }

        public int CompareTo(EquipHandbookEquipItemData other)
        {
            return baseScore - other.baseScore;
        }

        public ComCommonBind bind { get; set; }
    }

    class EquipHandbookUtility
    {
        public static bool IsFitOccupation(List<int> fitOccupations, List<int> baseOccupations)
        {
            if (null != baseOccupations && baseOccupations.Count > 0)
            {
                for (int i = 0; i < baseOccupations.Count; ++i)
                {
                    if (baseOccupations[i] == 0)
                    {
                        return true;
                    }

                    if (Utility.GetBaseJobID(PlayerBaseData.GetInstance().JobTableID) == Utility.GetBaseJobID(baseOccupations[i]))
                    {
                        return true;
                    }

                }
            }
            else
            {
                for (int i = 0; i < fitOccupations.Count; ++i)
                {
                    if (PlayerBaseData.GetInstance().JobTableID == fitOccupations[i])
                    {
                        return true;
                    }
                }
            }
           
            return false;
        }

        public static bool IsFitOccupation(List<int> fitOccupations)
        {
            if (null == fitOccupations || fitOccupations.Count <= 0)
            {
                return true;
            }

            for (int i = 0; i < fitOccupations.Count; ++i)
            {
                if (fitOccupations[i] == 0)
                {
                    return true;
                }

                if (PlayerBaseData.GetInstance().JobTableID == fitOccupations[i])
                {
                    return true;
                }
            }

            return false;
        }

    }
}
