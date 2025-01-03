using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ProtoTable;

namespace GameClient
{
    public class GrowthExpendData
    {
        public StrengthenGrowthType mStrengthenGrowthType;
        public ItemData mEquipItemData;
        public UnityAction<ItemData> mOnItemClick;
    }

    public class GrowthExpendItemFrame : ClientFrame
    {
        private GrowthExpendData mGrowthExpendItemData;
        private List<ItemData> mGrowthExpengItemList = new List<ItemData>();

        int timeLeft;
        bool bStartCountdown;
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/StrengthenGrowth/GrowthExpendItemFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            mGrowthExpendItemData = userData as GrowthExpendData;

            InitExpendItemComUIListScript();

            LoadGrowthExpendItemList();

            UpdateLinkID();
        }

        protected sealed override void _OnCloseFrame()
        {
            UnInitExpendItemComUIListScript();

            if (mGrowthExpengItemList != null)
            {
                mGrowthExpengItemList.Clear();
            }

            mGrowthExpendItemData = null;
        }

        private void LoadGrowthExpendItemList()
        {
            if (mGrowthExpengItemList != null)
            {
                mGrowthExpengItemList = new List<ItemData>();
            }
            else
            {
                mGrowthExpengItemList.Clear();
            }

            if (mGrowthExpendItemData != null)
            {
                if (mGrowthExpendItemData.mStrengthenGrowthType == StrengthenGrowthType.SGT_Strengthen)
                {
                    var items = StrengthenDataManager.GetInstance().GetStrengthenStampList(mGrowthExpendItemData.mEquipItemData);
                    if (items != null)
                    {
                        mGrowthExpengItemList.AddRange(items);
                    }
                }
                else if (mGrowthExpendItemData.mStrengthenGrowthType == StrengthenGrowthType.SGT_Gtowth)
                {
                    var items = EquipGrowthDataManager.GetInstance().GetGrowthStampList(mGrowthExpendItemData.mEquipItemData);
                    if (items != null)
                    {
                        mGrowthExpengItemList.AddRange(items);
                    }
                }
                else
                {
                    var items = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Material);
                    for (int i = 0; i < items.Count; i++)
                    {
                        var itemData = ItemDataManager.GetInstance().GetItem(items[i]);
                        if (itemData == null)
                            continue;

                        if (mGrowthExpendItemData.mStrengthenGrowthType == StrengthenGrowthType.SGT_Activate)
                        {
                            if (itemData.SubType == (int)ItemTable.eSubType.ST_ZENGFU_ACTIVATE)
                            {
                                itemData.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

                                //失效了
                                if (timeLeft <= 0 && bStartCountdown)
                                {
                                    continue;
                                }

                                AddExpendItem(itemData);
                                continue;
                            }
                        }
                        else if (mGrowthExpendItemData.mStrengthenGrowthType == StrengthenGrowthType.SGT_Clear)
                        {
                            if (itemData.SubType == (int)ItemTable.eSubType.ST_ZENGFU_CLEANUP)
                            {
                                itemData.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

                                //失效了
                                if (timeLeft <= 0 && bStartCountdown)
                                {
                                    continue;
                                }

                                AddExpendItem(itemData);
                                continue;
                            }
                        }
                        else if (mGrowthExpendItemData.mStrengthenGrowthType == StrengthenGrowthType.SGT_Change)
                        {
                            if (mGrowthExpendItemData.mEquipItemData != null)
                            {
                                //紫色装备可以使用
                                if (itemData.ThirdType == ItemTable.eThirdType.ZENGFU_COLOR_PURPLE)
                                {
                                    if (mGrowthExpendItemData.mEquipItemData.Quality != ItemTable.eColor.PURPLE)
                                    {
                                        continue;
                                    }
                                }

                                //绿色装备可以使用
                                if (itemData.ThirdType == ItemTable.eThirdType.ZENGFU_COLOR_GREEN)
                                {
                                    if (mGrowthExpendItemData.mEquipItemData.Quality != ItemTable.eColor.GREEN)
                                    {
                                        continue;
                                    }
                                }
                                
                                //如果是红字装备
                                if (mGrowthExpendItemData.mEquipItemData.EquipType == EEquipType.ET_REDMARK)
                                {
                                    //虚空物质转换器
                                    if (itemData.SubType == (int)ItemTable.eSubType.ST_ZENGFU_CHANGE)
                                    {
                                        itemData.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

                                        //失效了
                                        if (timeLeft <= 0 && bStartCountdown)
                                        {
                                            continue;
                                        }

                                        AddExpendItem(itemData);
                                        continue;
                                    }
                                }//如果是普通装备并且强化等级小于等于0
                                else if (mGrowthExpendItemData.mEquipItemData.EquipType == EEquipType.ET_COMMON && mGrowthExpendItemData.mEquipItemData.StrengthenLevel <= 0)
                                {//虚空物质生成装置
                                    if (itemData.SubType == (int)ItemTable.eSubType.ST_ZENGFU_CREATE)
                                    {
                                        itemData.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

                                        //失效了
                                        if (timeLeft <= 0 && bStartCountdown)
                                        {
                                            continue;
                                        }

                                        AddExpendItem(itemData);
                                        continue;
                                    }//虚空物质过载装置
                                    else if (itemData.SubType == (int)ItemTable.eSubType.ST_ZENGFU_OVERLOAD)
                                    {
                                        AddExpendItem(itemData);
                                        continue;
                                    }
                                }//如果是普通装备并且强化等级大于0
                                else if (mGrowthExpendItemData.mEquipItemData.EquipType == EEquipType.ET_COMMON && mGrowthExpendItemData.mEquipItemData.StrengthenLevel > 0)
                                {
                                    //虚空物质过载装置
                                    if (itemData.SubType == (int)ItemTable.eSubType.ST_ZENGFU_OVERLOAD)
                                    {
                                        AddExpendItem(itemData);
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }

                mExpendItemScrollView.SetElementAmount(mGrowthExpengItemList.Count);
            }
        }

        private void AddExpendItem(ItemData itemData)
        {
            mGrowthExpengItemList.Add(itemData);
        }

        private void UpdateLinkID()
        {
            if (mGrowthExpengItemList.Count <= 0)
            {
                if (mGrowthExpendItemData != null)
                {
                    if (mGrowthExpendItemData.mStrengthenGrowthType == StrengthenGrowthType.SGT_Activate)
                    {
                        mLink.iItemLinkID = 300000205;
                    }
                    else if (mGrowthExpendItemData.mStrengthenGrowthType == StrengthenGrowthType.SGT_Clear)
                    {
                        mLink.iItemLinkID = 300000206;
                    }
                    else if (mGrowthExpendItemData.mStrengthenGrowthType == StrengthenGrowthType.SGT_Change)
                    {
                        if (mGrowthExpendItemData.mEquipItemData != null)
                        {
                            if (mGrowthExpendItemData.mEquipItemData.EquipType == EEquipType.ET_COMMON)
                            {
                                mLink.iItemLinkID = 300000202;
                            }
                            else if (mGrowthExpendItemData.mEquipItemData.EquipType == EEquipType.ET_REDMARK)
                            {
                                mLink.iItemLinkID = 300000204;
                            }
                        }
                    }
                }
            }
        }

        private void InitExpendItemComUIListScript()
        {
            if (mExpendItemScrollView != null)
            {
                mExpendItemScrollView.Initialize();
                mExpendItemScrollView.onBindItem += OnBindItemDelegate;
                mExpendItemScrollView.onItemVisiable += OnItemVisiableDelegate;
                mExpendItemScrollView.onItemSelected += OnItemSelectDelegate;
            }
        }

        private void UnInitExpendItemComUIListScript()
        {
            if (mExpendItemScrollView != null)
            {
                mExpendItemScrollView.onBindItem -= OnBindItemDelegate;
                mExpendItemScrollView.onItemVisiable -= OnItemVisiableDelegate;
                mExpendItemScrollView.onItemSelected -= OnItemSelectDelegate;
            }
        }

        private GrowthExpendItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<GrowthExpendItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var growthExpendItem = item.gameObjectBindScript as GrowthExpendItem;
            if (growthExpendItem != null && mGrowthExpendItemData != null && item.m_index >= 0 && item.m_index < mGrowthExpengItemList.Count)
            {
                growthExpendItem.OnItemVisiable(mGrowthExpengItemList[item.m_index]);
            }
        }

        private void OnItemSelectDelegate(ComUIListElementScript item)
        {
            if (item != null && item.gameObjectBindScript != null)
            {
                GrowthExpendItem growthExpendItem = item.gameObjectBindScript as GrowthExpendItem;

                if (growthExpendItem != null && mGrowthExpendItemData != null)
                {
                    if (mGrowthExpendItemData.mOnItemClick != null && growthExpendItem.ItemData != null)
                    {
                        mGrowthExpendItemData.mOnItemClick.Invoke(growthExpendItem.ItemData);
                    }
                }

                Close();
            }
        }
        
        #region ExtraUIBind
        private ItemComeLink mLink = null;
        private ComUIListScript mExpendItemScrollView = null;

        protected sealed override void _bindExUI()
        {
            mLink = mBind.GetCom<ItemComeLink>("Link");
            mExpendItemScrollView = mBind.GetCom<ComUIListScript>("ExpendItemScrollView");
        }

        protected sealed override void _unbindExUI()
        {
            mLink = null;
            mExpendItemScrollView = null;
        }
        #endregion
    }
}