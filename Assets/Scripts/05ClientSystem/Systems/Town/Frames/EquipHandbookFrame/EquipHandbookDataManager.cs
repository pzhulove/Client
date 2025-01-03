using ProtoTable;
using System.Collections.Generic;

namespace GameClient
{
    public class EquipHandbookDataManager : DataManager<EquipHandbookDataManager>
    {
        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.Default;
        }

        public const int EQUIP_HANDBOOK_TAB_JINPIN_TABLE_ID = 2;                                                    //极品神器页签表格ID

        private List<EquipHandbookTabData> mEquipTabs = new List<EquipHandbookTabData>();

        private List<EquipHandbookEquipItemData> mPlayerEquipData = new List<EquipHandbookEquipItemData>();
        private List<EquipHandbookEquipItemData> mPendingPlayerEquipData = new List<EquipHandbookEquipItemData>();

        private int[] mLevel = new int[] { 20, 30, 40, 50 };

        private int mTabSelectedIndex = 0;
        public int TabSelectedIndex
        {
            get
            {
                return mTabSelectedIndex;
            }
            set
            {
                mTabSelectedIndex = value;
            }
        }

       
        private bool mOnloginFlag = true;
        /// <summary>
        /// 表示每次登陆显示装备图鉴Tips和魔罐Tips的标志位
        /// </summary>
        public bool OnLoginFlag
        {
            get
            {
                return mOnloginFlag;
            }
            set
            {
                mOnloginFlag = value;
            }
        }

        public bool bIsHintEquipmentGuide()
        {
            for (int i = 0; i < mLevel.Length; i++)
            {
                if (PlayerBaseData.GetInstance().Level != mLevel[i])
                {
                   continue;
                }
                return true;
            }

            return false;
        }
        

        public List<EquipHandbookTabData> equipTabs
        {
            get { return mEquipTabs; }
        }

        public List<EquipHandbookEquipItemData> playerEquipData
        {
            get
            {
                return mPlayerEquipData;
            }
        }

        public int sumPlayerEquipCollectScore
        {
            get
            {
                int sum = 0;

                for (int i = 0; i < mPlayerEquipData.Count; ++i)
                {
                    sum += mPlayerEquipData[i].baseScore;
                }

                return sum;
            }
        }

        public override void Clear()
        {
            mEquipTabs.Clear();
            _clearPlayerEquipData();

            _unbindEvent();

            TabSelectedIndex = 0;

            mOnloginFlag = true;
        }

        public override void Initialize()
        {
            Clear();

            _initEquipTabsFromContentTable();
            _attachEquipTabsInfoFromCollectTable();

            _sortEquipTabs();
            
            _bindEvent();
        }

        public void InitSelfEquipData()
        { 
            _initPlayerEquipData();
            _updateEquipScore();
            _updateFilterByCondition();
        }

        public void _updateFilterByCondition()
        {
            for (int i = 0; i < mEquipTabs.Count; ++i)
            {
                EquipHandbookTabData tabData = mEquipTabs[i];

                List<EquipHandbookTabCollectionData> list = tabData.FilterWithCondition();

                for (int j = 0; j < list.Count; j++)
                {
                    list[j].FilterWithOccupation();
                }

                tabData.FilterItemWithCondition();
            }
        }

        private void _updateEquipScore()
        {
            for (int i = 0; i < mEquipTabs.Count; i++)
            {
                EquipHandbookTabData tabData = mEquipTabs[i];

                if (null == tabData)
                {
                    continue;
                }

                if (tabData.isShowEquipScore)
                {
                    tabData.CalculateEquipScore();
                }
            }
        }

        private void _onFilterConditionChanged(UIEvent ui)
        {
            _updateFilterByCondition();
            _updateEquipScore();
        }

        #region MainPlayerEquipData
        /// <summary>
        /// 获得玩家已装备的数据
        /// </summary>
        private void _initPlayerEquipData()
        {
            mPlayerEquipData.Clear();

            List<ulong> equipIDs = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);

            for (int i = 0; i < equipIDs.Count; ++i)
            {
                ItemData item = ItemDataManager.GetInstance().GetItem(equipIDs[i]);
                if (item == null) continue;

                if (item.Type==ItemTable.eType.FUCKTITTLE && item.SubType==(int)ItemTable.eSubType.TITLE)
                {
                    continue;
                }

                _addPlayerEquipData(item.TableID);
            }
        }

        private void _addPlayerEquipData(int itemId)
        {
            EquipHandbookEquipItemData data = new EquipHandbookEquipItemData(itemId);
            data.CalculateBaseScore();
            mPlayerEquipData.Add(data);
        }

        private void _updatePlayerEquipData()
        {
            mPendingPlayerEquipData.Clear();
            mPendingPlayerEquipData.AddRange(mPlayerEquipData);
            mPlayerEquipData.Clear();

            List<ulong> equipIDs = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
            for (int i = 0; i < equipIDs.Count; ++i)
            {
                ItemData item = ItemDataManager.GetInstance().GetItem(equipIDs[i]);
                if (item == null) continue;
                if (item.Type == ItemTable.eType.FUCKTITTLE && item.SubType == (int)ItemTable.eSubType.TITLE)
                {
                    continue;
                }

                EquipHandbookEquipItemData data = _findEquipHandbookEquipItemData(item.TableID);
                if (null != data)
                {
                    mPlayerEquipData.Add(data);
                }
                else
                {
                    _addPlayerEquipData(item.TableID);
                }
            }

            mPendingPlayerEquipData.Clear();
        }

        private EquipHandbookEquipItemData _findEquipHandbookEquipItemData(int itemId)
        {
            for (int i = 0; i < mPendingPlayerEquipData.Count; i++)
            {
                if (itemId == mPendingPlayerEquipData[i].id)
                {
                    return mPendingPlayerEquipData[i];
                }
            }

            return null;
        }

        private void _clearPlayerEquipData()
        {
            mPlayerEquipData.Clear();
            mPendingPlayerEquipData.Clear();
        }

        private void _onSwitchEquipSuccess(UIEvent ui)
        {
            _updatePlayerEquipData();
            _updateEquipScore();
        }
        #endregion

        #region Event
        private void _bindEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemUseSuccess, _onSwitchEquipSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.LevelChanged, _onFilterConditionChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.JobIDChanged, _onFilterConditionChanged);
        }

        private void _unbindEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemUseSuccess, _onSwitchEquipSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.LevelChanged, _onFilterConditionChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.JobIDChanged, _onFilterConditionChanged);
        }
        #endregion

        private void _initEquipTabsFromContentTable()
        {
            var kv = TableManager.instance.GetTable<ProtoTable.EquipHandbookContentTable>();

            if (null == kv)
            {
                Logger.LogErrorFormat("[EquipHandbook] EquipHandbookContentTable is broken!");
                return;
            }

            var iter = kv.GetEnumerator();

            while (iter.MoveNext())
            {
                ProtoTable.EquipHandbookContentTable item = iter.Current.Value as ProtoTable.EquipHandbookContentTable;
                EquipHandbookTabData tabData = new EquipHandbookTabData(item);
                mEquipTabs.Add(tabData);
            }
        }

        private void _attachEquipTabsInfoFromCollectTable()
        {
            var kv = TableManager.instance.GetTable<ProtoTable.EquipHandbookCollectionTable>();

            if (null == kv)
            {
                Logger.LogErrorFormat("[EquipHandbook] EquipHandbookCollectionTable is broken!");
                return ;
            }

            var iter = kv.GetEnumerator();

            while (iter.MoveNext())
            {
                ProtoTable.EquipHandbookCollectionTable item = iter.Current.Value as ProtoTable.EquipHandbookCollectionTable;

                EquipHandbookTabData tabData = _findEquipTab(item.EquipHandbookContentID);

                if (null != tabData)
                {
                    EquipHandbookTabCollectionData data = new EquipHandbookTabCollectionData(item);
                    if (item.ScreenType == EquipHandbookCollectionTable.eScreenType.eWeapon)
                    {
                        System.Collections.IEnumerator collectioniter = data.GetWeaponSplitIterator();

                        while (collectioniter.MoveNext())
                        {
                            EquipHandbookTabCollectionData iterDat = collectioniter.Current as EquipHandbookTabCollectionData;
                            if (null != iterDat)
                            {
                                tabData.AddTabCollectionData(iterDat);
                            }
                        }
                    }
                    else
                    {
                        tabData.AddTabCollectionData(data);
                    }
                }
                else
                {
                    Logger.LogErrorFormat("[EquipHandbook] 页签 {0} 无法找到", item.ID);
                }
            }
        }

        /// <summary>
        /// 数据排序
        /// </summary>
        private void _sortEquipTabs()
        {
            mEquipTabs.Sort();

            for (int i = 0; i < mEquipTabs.Count; ++i)
            {
                mEquipTabs[i].SortTabCollectionDatas();
            }
        }

        private void _clearEquipTabs()
        {
            mEquipTabs.Clear();
        }

        private EquipHandbookTabData _findEquipTab(int id)
        {
            for (int i = 0; i < mEquipTabs.Count; ++i)
            {
                if (mEquipTabs[i].id == id)
                {
                    return mEquipTabs[i];
                }
            }

            return null;
        }

        public int GetPlayerWeaponScore()
        {

            List<ulong> equipIDs = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);

            for (int i = 0; i < equipIDs.Count; ++i)
            {
                ItemData item = ItemDataManager.GetInstance().GetItem(equipIDs[i]);

                if (item == null || !PlayerBaseData.IsWeapon((ItemTable.eSubType)item.SubType))
                {
                    continue;
                }

                for (int j = 0; j < mPlayerEquipData.Count; j++)
                {
                    if (item.TableID !=  mPlayerEquipData[j].id)
                    {
                        continue;
                    }

                    return mPlayerEquipData[j].baseScore;
                }

               
            }

            return 0;
        }

        public int GetSplitLevel(List<EquipHandbookTabCollectionData> collectIDs)
        {
            if (null == collectIDs)
            {
                return -1;
            }

            int lastLevel = -1;
            int curLevel = -1;

            for (int i = 0; i < collectIDs.Count; i++)
            {
                curLevel = collectIDs[i].level;

                if (curLevel == PlayerBaseData.GetInstance().Level)
                {
                    return curLevel;
                }
                else if (curLevel > PlayerBaseData.GetInstance().Level)
                {
                    return lastLevel;
                }

                lastLevel = curLevel;
            }

            return curLevel;
        }

        public int GetPlayerArmorScore()
        {
            int score = 0;
            List<ulong> equipIDs = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);

            for (int i = 0; i < equipIDs.Count; ++i)
            {
                ItemData item = ItemDataManager.GetInstance().GetItem(equipIDs[i]);

                if (item == null || !PlayerBaseData.IsArmy((ItemTable.eSubType)item.SubType))
                {
                    continue;
                }

                for (int j = 0; j < mPlayerEquipData.Count; j++)
                {
                    if (item.TableID != mPlayerEquipData[j].id)
                    {
                        continue;
                    }
                    score += mPlayerEquipData[j].baseScore;
                }

                
            }

            return score;
        }

        public int GetPlayerJewelryScore()
        {
            int score = 0;
            List<ulong> equipIDs = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);

            for (int i = 0; i < equipIDs.Count; ++i)
            {
                ItemData item = ItemDataManager.GetInstance().GetItem(equipIDs[i]);

                if (item == null || !PlayerBaseData.IsJewelry((ItemTable.eSubType)item.SubType))
                {
                    continue;
                }

                for (int j = 0; j < mPlayerEquipData.Count; j++)
                {
                    if (item.TableID != mPlayerEquipData[j].id)
                    {
                        continue;
                    }
                    score += mPlayerEquipData[j].baseScore;
                }
            }

            return score;
        }

        public bool BIsHintEquipmentGuide()
        {
            List < EquipHandbookEquipItemData > mLowestScoreItemList = new List<EquipHandbookEquipItemData>();

            for (int i = 0; i < mEquipTabs.Count; i++)
            {
                EquipHandbookTabData tabData = mEquipTabs[i];

                if (null == tabData)
                {
                    continue;
                }

                if (tabData.isShowEquipScore)
                {
                    mLowestScoreItemList = tabData.GetLowestScoreItemList();
                }
            }

            int LowestScore = 0;

            for (int i = 0; i < mLowestScoreItemList.Count; i++)
            {
                LowestScore += mLowestScoreItemList[i].baseScore;
            }

            if (LowestScore > sumPlayerEquipCollectScore)
            {
                return true;
            }

            return false;

        }

        /// <summary>
        /// 判断背包中的装备在装备图鉴附加条目表中是否找到 //用来显示装备获取途径按钮。
        /// </summary>
        /// <param name="mItemID"></param>
        /// <returns></returns>
        public bool EquipHandbookAttachedTableIsFindItemID(int mItemID)
        {
            var mEquipbookAttachedTable = TableManager.GetInstance().GetTableItem<EquipHandbookAttachedTable>(mItemID);
            if (mEquipbookAttachedTable != null)
            {
                if (mEquipbookAttachedTable.EquipSourceEntrance == 1)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否是极品道具
        /// </summary>
        /// <param name="itemTableId"></param>
        /// <returns></returns>
        public bool IsHighestGradeItem(int itemTableId)
        {
            if (mEquipTabs == null || mEquipTabs.Count <= 0)
            {
                return false;
            }
            for (int i = 0; i < mEquipTabs.Count; i++)
            {
                var equipTab = mEquipTabs[i];
                if (equipTab == null)
                    continue;
                if (equipTab.id == EQUIP_HANDBOOK_TAB_JINPIN_TABLE_ID)
                {
                    return equipTab.IsContainItem(itemTableId);
                }
            }
            return false;
        }

    }
}
