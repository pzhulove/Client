using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using System;
using ProtoTable;

namespace GameClient
{
    public enum StrengthenGrowthType
    {
        SGT_NONE = -1,
        SGT_Strengthen,//强化
        SGT_Gtowth,    //增幅
        SGT_Clear,     //清除
        SGT_Activate,  //激活
        SGT_Change,   //转化
    }

    /// <summary>
    /// Tab数据
    /// </summary>
    [Serializable]
    public class StrengthenGrowthTabModel
    {
        public int iIndex;
        public StrengthenGrowthType sType;
        public string sName;
        public GameObject mContentRoot;
    }

    public delegate void SetStrengthenGrowthType(StrengthenGrowthType type);
    public delegate void OnStrengthenGrowthEquipItemClick(ItemData itemData, StrengthenGrowthType eStrengthenGrowthType);
    public class StrengthenGrowthView : MonoBehaviour
    {
        [SerializeField]private ComUIListScript mEquipItemsUIList;
        [SerializeField]private Text mNoEquipTips;
        [SerializeField]private GameObject mBGRoot;
        [SerializeField] private GameObject mMaskRoot;
        [SerializeField]private RectTransform mNotEquipTipRect;
        [SerializeField]private Vector2 mNormalPos = new Vector2(711,17);
        [SerializeField]private Vector2 mStrengthenPos = new Vector2(764,17);
        
        private StrengthenGrowthType CurrentSelectedStrengthGrowthType = StrengthenGrowthType.SGT_Strengthen;//跳转类型||选择页签类型
        public static ItemData mLastSelectedItemData = null;//记录最后选择的道具
        public static OnStrengthenGrowthEquipItemClick mOnStrengthenGrowthEquipItemClick;
        private List<ItemData> mAllEquipItem = new List<ItemData>();
        private SmithShopNewLinkData mLinkData;
        private int mCurrentSelectEquipemtnIndex = 0;//当前选择装备的索引
        private void Awake()
        {
            InitEquipItemUIListScript();
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRefreshEquipmentList, OnRefreshEquipmentList);
        }

        private void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRefreshEquipmentList, OnRefreshEquipmentList);
            UnInitEquipItemUIListScript();
            mAllEquipItem.Clear();
            CurrentSelectedStrengthGrowthType = StrengthenGrowthType.SGT_Strengthen;
            mLastSelectedItemData = null;
            mOnStrengthenGrowthEquipItemClick = null;
            mCurrentSelectEquipemtnIndex = 0;
        }
        
        private void OnRefreshEquipmentList(UIEvent uIEvent)
        {
            RefreshAllEquipments();
        }

        public void OnSetMaskRoot(bool value)
        {
            if(mMaskRoot != null)
            {
                mMaskRoot.CustomActive(value);
            }
        }

        /// <summary>
        /// 设置页签类型
        /// </summary>
        /// <param name="type"></param>
        public void OnSetStrengthGrowthType(StrengthenGrowthType type)
        {
            CurrentSelectedStrengthGrowthType = type;
            RefreshAllEquipments();
        }
        
        public void InitView(SmithShopNewLinkData linkData)
        {
            mLinkData = linkData;
        }

        public void RefreshAllEquipments()
        {
            LoadAllEquipments(ref mAllEquipItem, CurrentSelectedStrengthGrowthType);
            if (mEquipItemsUIList != null)
            {
                mEquipItemsUIList.SetElementAmount(mAllEquipItem.Count);
            }

            if (mAllEquipItem.Count > 0)
            {
                OnSetBGRoot(true);
                mEquipItemsUIList.ResetSelectedElementIndex();
                TrySetDefaultEquipment();
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnEquipmentListHasEquip);
            }
            else
            {
                StrengthenDataManager.GetInstance().IsEquipmentStrengthBroken = false;
                EquipGrowthDataManager.GetInstance().IsEquipmentIntensifyBroken = false;
                UpdateNoEquipTipsDesc();
                OnSetBGRoot(false);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnEquipmentListNoEquipment);
            }
        }

        private void OnSetBGRoot(bool value)
        {
            if (mBGRoot != null)
                mBGRoot.CustomActive(value);
        }

        private void OnSetNotEquipTipRect(Vector2 pos)
        {
            if (mNotEquipTipRect != null)
                mNotEquipTipRect.anchoredPosition = pos;
        }

        private void UpdateNoEquipTipsDesc()
        {
            OnSetNotEquipTipRect(mNormalPos);
            string mContent = string.Empty;
            switch (CurrentSelectedStrengthGrowthType)
            {
                case StrengthenGrowthType.SGT_NONE:
                    break;
                case StrengthenGrowthType.SGT_Strengthen:
                    mContent = TR.Value("no_equip_tips_desc_strengthen");
                    OnSetNotEquipTipRect(mStrengthenPos);
                    break;
                case StrengthenGrowthType.SGT_Gtowth:
                    mContent = TR.Value("no_equip_tips_desc_growth");
                    break;
                case StrengthenGrowthType.SGT_Clear:
                case StrengthenGrowthType.SGT_Activate:
                    mContent = TR.Value("no_equip_tips_desc_clear_or_activation");
                    break;
                case StrengthenGrowthType.SGT_Change:
                    mContent = TR.Value("no_equip_tips_desc_changed");
                    break;
            }

            if (mNoEquipTips != null)
                mNoEquipTips.text = mContent;
        }
        
        #region EquipItemUIListScript

        private void InitEquipItemUIListScript()
        {
            if (mEquipItemsUIList != null)
            {
                mEquipItemsUIList.Initialize();
                mEquipItemsUIList.onBindItem += OnBindItemDelegate;
                mEquipItemsUIList.onItemVisiable += OnItemVisiableDelegate;
                mEquipItemsUIList.onItemSelected += OnItemSelectedDelegate;
                mEquipItemsUIList.onItemChageDisplay += OnItemChangeDisplayDelegate;
            }
        }

        private void UnInitEquipItemUIListScript()
        {
            if (mEquipItemsUIList != null)
            {
                mEquipItemsUIList.onBindItem -= OnBindItemDelegate;
                mEquipItemsUIList.onItemVisiable -= OnItemVisiableDelegate;
                mEquipItemsUIList.onItemSelected -= OnItemSelectedDelegate;
                mEquipItemsUIList.onItemChageDisplay -= OnItemChangeDisplayDelegate;
            }
        }

        private StrengthenGrowthEquipItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<StrengthenGrowthEquipItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var equipItem = item.gameObjectBindScript as StrengthenGrowthEquipItem;
            if (equipItem != null && item.m_index >= 0 && item.m_index < mAllEquipItem.Count)
            {
                equipItem.OnItemVisible(mAllEquipItem[item.m_index], CurrentSelectedStrengthGrowthType);
                if (mLastSelectedItemData != null)
                {
                    if (mAllEquipItem[item.m_index].GUID != mLastSelectedItemData.GUID)
                    {
                        equipItem.OnItemChangeDisplay(false);
                    }
                }
            }
        }
        
        private void OnItemSelectedDelegate(ComUIListElementScript item)
        {
            var equipItem = item.gameObjectBindScript as StrengthenGrowthEquipItem;
            if (equipItem != null)
            {
                mCurrentSelectEquipemtnIndex = item.m_index;
                mLastSelectedItemData = equipItem.EquipItemData == null ? null : equipItem.EquipItemData;
                if (mOnStrengthenGrowthEquipItemClick != null)
                {
                    mOnStrengthenGrowthEquipItemClick.Invoke(mLastSelectedItemData, CurrentSelectedStrengthGrowthType);
                }
            }
        }

        private void OnItemChangeDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            var equipItem = item.gameObjectBindScript as StrengthenGrowthEquipItem;
            if (equipItem != null)
            {
                equipItem.OnItemChangeDisplay(bSelected);
            }
        }
        #endregion

        private  void LoadAllEquipments(ref List<ItemData> equipments, StrengthenGrowthType type)
        {
            if (equipments != null)
            {
                equipments.Clear();
            }

            int minLevel = 0;
            int maxLevel = 0;
            if (SmithShopNewFrameView.iDefaultLevelData != null)
            {
                var smithshopFilterTable = TableManager.GetInstance().GetTableItem<SmithShopFilterTable>(SmithShopNewFrameView.iDefaultLevelData.Id);
                if (smithshopFilterTable != null)
                {
                    if (smithshopFilterTable.Parameter.Count >= 2)
                    {
                        minLevel = smithshopFilterTable.Parameter[0];
                        maxLevel = smithshopFilterTable.Parameter[1];
                    }
                }
            }

            var uids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Equip);
            for (int i = 0; i < uids.Count; ++i)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(uids[i]);

                if (itemData == null)
                {
                    continue;
                }

                //辅助装备开关关闭 不可以强化、激化
                if (!ServerSceneFuncSwitchManager.GetInstance().IsServiceTypeSwitchOpen(Protocol.ServiceType.SERVICE_ASSIST_EQUIP))
                {
                    //如果是辅助装备
                    if (itemData.IsAssistEquip())
                    {
                        continue;
                    }
                }
                
                if (itemData.IsBxyEquip())
                {
                    continue;
                }
                
                if (itemData.IsLease)
                {
                    continue;
                }

                if (itemData.isInSidePack)
                {
                    continue;
                }

                if (itemData.DeadTimestamp > 0)
                {
                    continue;
                }

                switch (type)
                {
                    case StrengthenGrowthType.SGT_Strengthen:
                        {
                            if (itemData.EquipType != EEquipType.ET_COMMON)
                            {
                                continue;
                            }
                        }
                        break;
                    case StrengthenGrowthType.SGT_Gtowth:
                        {
                            if (itemData.EquipType != EEquipType.ET_REDMARK)
                            {
                                continue;
                            }
                        }
                        break;
                    case StrengthenGrowthType.SGT_Clear:
                    case StrengthenGrowthType.SGT_Activate:
                        {
                            if (itemData.EquipType != EEquipType.ET_BREATH)
                            {
                                continue;
                            }
                        }
                        break;
                    case StrengthenGrowthType.SGT_Change:
                        {
                            if (itemData.EquipType == EEquipType.ET_BREATH)
                            {
                                continue;
                            }
                            
                            if (itemData.Quality < ItemTable.eColor.PURPLE)
                            {
                                continue;
                            }
                        }
                        break;
                }

                if(SmithShopNewFrameView.iDefaultQuality != 0)
                {
                    if ((int)itemData.Quality != SmithShopNewFrameView.iDefaultQuality)
                        continue;
                }

                if (itemData.LevelLimit < minLevel)
                    continue;

                if (itemData.LevelLimit > maxLevel)
                    continue;
             
                equipments.Add(itemData);
            }

            var wearUids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
            for (int i = 0; i < wearUids.Count; ++i)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(wearUids[i]);

                if (itemData == null)
                {
                    continue;
                }

                //辅助装备开关关闭 不可以强化、激化
                if (!ServerSceneFuncSwitchManager.GetInstance().IsServiceTypeSwitchOpen(Protocol.ServiceType.SERVICE_ASSIST_EQUIP))
                {
                    //如果是辅助装备
                    if (itemData.IsAssistEquip())
                    {
                        continue;
                    }
                }

                if (itemData.IsBxyEquip())
                {
                    continue;
                }

                if (itemData.Type != ItemTable.eType.EQUIP)
                {
                    continue;
                }

                //租赁装备
                if (itemData.IsLease)
                {
                    continue;
                }

                //副武器
                if (itemData.isInSidePack)
                {
                    continue;
                }

                //限时装备
                if (itemData.DeadTimestamp > 0)
                {
                    continue;
                }

                switch (type)
                {
                    case StrengthenGrowthType.SGT_Strengthen:
                        {
                            if (itemData.EquipType != EEquipType.ET_COMMON)
                            {
                                continue;
                            }
                        }
                        break;
                    case StrengthenGrowthType.SGT_Gtowth:
                        {
                            if (itemData.EquipType != EEquipType.ET_REDMARK)
                            {
                                continue;
                            }
                        }
                        break;
                    case StrengthenGrowthType.SGT_Clear:
                    case StrengthenGrowthType.SGT_Activate:
                        {
                            if (itemData.EquipType != EEquipType.ET_BREATH)
                            {
                                continue;
                            }
                        }
                        break;
                    case StrengthenGrowthType.SGT_Change:
                        {
                            if (itemData.EquipType == EEquipType.ET_BREATH)
                            {
                                continue;
                            }
                            
                            if (itemData.Quality < ItemTable.eColor.PURPLE)
                            {
                                continue;
                            }
                        }
                        break;
                }

                if (SmithShopNewFrameView.iDefaultQuality != 0)
                {
                    if ((int)itemData.Quality != SmithShopNewFrameView.iDefaultQuality)
                        continue;
                }

                if (itemData.LevelLimit < minLevel)
                    continue;

                if (itemData.LevelLimit > maxLevel)
                    continue;

                equipments.Add(itemData);

            }
            
            equipments.Sort(SortEquipments);
            
        }

        private int SortEquipments(ItemData left, ItemData right)
        {
            if (left.PackageType != right.PackageType)
            {
                return (int)right.PackageType - (int)left.PackageType;
            }

            //在未启用的装备方案中，放在前面
            if (left.IsItemInUnUsedEquipPlan != right.IsItemInUnUsedEquipPlan)
            {
                if (left.IsItemInUnUsedEquipPlan == true)
                    return -1;
                if (right.IsItemInUnUsedEquipPlan == true)
                    return 1;
            }

            if (left.Quality != right.Quality)
            {
                return (int)right.Quality - (int)left.Quality;
            }

            if (left.SubType != right.SubType)
            {
                return (int)left.SubType - (int)right.SubType;
            }

            if (left.StrengthenLevel != right.StrengthenLevel)
            {
                return right.StrengthenLevel - left.StrengthenLevel;
            }

            return right.LevelLimit - left.LevelLimit;
        }

        private void TrySetDefaultEquipment()
        {
            int iSelectIndex = -1;
            if (mLinkData != null)
            {
                if (mLinkData != null && mLinkData.itemData != null)
                {
                    int iBindIndex = 0;
                    for (int i = 0; i < mAllEquipItem.Count; i++)
                    {
                        var item = mAllEquipItem[i];
                        if (item.GUID != mLinkData.itemData.GUID)
                        {
                            continue;
                        }

                        iBindIndex = i;
                        break;
                    }

                    SetSelectItem(iBindIndex);

                    mLinkData = null;
                    return;
                }
            }

            if (mLastSelectedItemData != null)
            {
                var find = mAllEquipItem.Find(x =>
                {
                    return x.GUID == mLastSelectedItemData.GUID;
                });
                if (find != null)
                {
                    mLastSelectedItemData = ItemDataManager.GetInstance().GetItem(mLastSelectedItemData.GUID);
                }
                else
                {
                    mLastSelectedItemData = null;
                }
            }

            if (mLastSelectedItemData != null)
            {
                for (int i = 0; i < mAllEquipItem.Count; i++)
                {
                    var item = mAllEquipItem[i];
                    if (item.GUID != mLastSelectedItemData.GUID)
                    {
                        continue;
                    }

                    iSelectIndex = i;
                    break;
                }
            }
            else
            {
                if (mAllEquipItem.Count > 0)
                {
                    iSelectIndex = 0;
                }
            }

            if (StrengthenDataManager.GetInstance().IsEquipmentStrengthBroken == true)
            {
                if (mCurrentSelectEquipemtnIndex >= 0 && mCurrentSelectEquipemtnIndex < mAllEquipItem.Count)
                    iSelectIndex = mCurrentSelectEquipemtnIndex;

                StrengthenDataManager.GetInstance().IsEquipmentStrengthBroken = false;
            }

            if (EquipGrowthDataManager.GetInstance().IsEquipmentIntensifyBroken == true)
            {
                if (mCurrentSelectEquipemtnIndex >= 0 && mCurrentSelectEquipemtnIndex < mAllEquipItem.Count)
                    iSelectIndex = mCurrentSelectEquipemtnIndex;

                EquipGrowthDataManager.GetInstance().IsEquipmentIntensifyBroken = false;
            }

            SetSelectItem(iSelectIndex);
        }

        private void SetSelectItem(int index)
        {
            if (index >= 0 && index < mAllEquipItem.Count)
            {
                if (mEquipItemsUIList != null)
                {
                    mLastSelectedItemData = mAllEquipItem[index];
                    if (!mEquipItemsUIList.IsElementInScrollArea(index))
                    {
                        mEquipItemsUIList.EnsureElementVisable(index);
                    }
                    
                    mEquipItemsUIList.SelectElement(index);
                }
            }
            else
            {
                if (mEquipItemsUIList != null)
                {
                    mEquipItemsUIList.SelectElement(-1);
                    mLastSelectedItemData = null;
                }
            }

            if (mOnStrengthenGrowthEquipItemClick != null)
            {
                mOnStrengthenGrowthEquipItemClick.Invoke(mLastSelectedItemData, CurrentSelectedStrengthGrowthType);
            }
        }
    }
}