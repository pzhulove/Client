using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoTable;

namespace GameClient
{
    public class SingleUseExpendItemFrame : ClientFrame
    {
        private GrowthExpendData mExpendItemData;
        private List<ItemData> mExpengItemList = new List<ItemData>();

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

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/StrengthenGrowth/GrowthExpendItemFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            mExpendItemData = userData as GrowthExpendData;

            InitExpendItemComUIListScript();
            LoadExpendItemList();
            UpdateLinkID();
        }

        protected sealed override void _OnCloseFrame()
        {
            UnInitExpendItemComUIListScript();
            mExpendItemData = null;
            if (mExpengItemList != null)
            {
                mExpengItemList.Clear();
            }
        }

        private void LoadExpendItemList()
        {
            if (mExpendItemData != null)
            {
                var items = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Material);
                if (items == null)
                {
                    return;
                }

                if (mExpendItemData.mStrengthenGrowthType == StrengthenGrowthType.SGT_Strengthen)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        var guid = items[i];
                        var itemData = ItemDataManager.GetInstance().GetItem(guid);
                        if (itemData == null)
                        {
                            continue;
                        }

                        if (itemData.ThirdType != ItemTable.eThirdType.DisposableStrengItem)
                        {
                            continue;
                        }

                        mExpengItemList.Add(itemData);
                    }
                }
                else if (mExpendItemData.mStrengthenGrowthType == StrengthenGrowthType.SGT_Gtowth)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        var guid = items[i];
                        var itemData = ItemDataManager.GetInstance().GetItem(guid);
                        if (itemData == null)
                        {
                            continue;
                        }

                        if (itemData.ThirdType != ItemTable.eThirdType.DisposableIncreaseItem)
                        {
                            continue;
                        }

                        mExpengItemList.Add(itemData);
                    }
                }
            }

            mExpendItemScrollView.SetElementAmount(mExpengItemList.Count);
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
            if (growthExpendItem != null && mExpendItemData != null && item.m_index >= 0 && item.m_index < mExpengItemList.Count)
            {
                growthExpendItem.OnItemVisiable(mExpengItemList[item.m_index]);
            }
        }

        private void OnItemSelectDelegate(ComUIListElementScript item)
        {
            var growthExpendItem = item.gameObjectBindScript as GrowthExpendItem;
            if (growthExpendItem != null && mExpendItemData != null)
            {
                if (mExpendItemData.mOnItemClick != null)
                {
                    mExpendItemData.mOnItemClick.Invoke(growthExpendItem.ItemData);
                }

                Close();
            }
        }

        private void UpdateLinkID()
        {
            if (mExpengItemList.Count <= 0)
            {
                if (mExpendItemData != null)
                {
                    if (mExpendItemData.mStrengthenGrowthType == StrengthenGrowthType.SGT_Strengthen)
                    {
                        mLink.iItemLinkID = 330000242;
                    }
                    else if (mExpendItemData.mStrengthenGrowthType == StrengthenGrowthType.SGT_Gtowth)
                    {
                        mLink.iItemLinkID = 330000243;
                    }
                }
            }
        }
    }
}