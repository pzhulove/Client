using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    public delegate void OnStrengthenDeviceItem(ItemData item);
    public class StrengthenDeviceItem : MonoBehaviour
    {
        [SerializeField] private Button mSelectedBtn;
        [SerializeField] private GameObject mItemParent;
        [SerializeField] private Text mCount;
        [SerializeField] private Text mName;
        [SerializeField] private Image mItemBgImg;
        [SerializeField] private Image mItemIconImg;
        [SerializeField] private string mStrengthenDesc = "选择强化装置";
        [SerializeField] private string mGrowthDesc = "选择激化装置";

        private StrengthenGrowthType mStrengthenGrowthType;
        private OnStrengthenDeviceItem mOnStrengthenDeviceItem;

        private void Awake()
        {
            if (mSelectedBtn != null)
            {
                mSelectedBtn.onClick.RemoveAllListeners();
                mSelectedBtn.onClick.AddListener(OnSelectedClick);
            }
        }

        private void OnDestroy()
        {
            mStrengthenGrowthType = StrengthenGrowthType.SGT_Strengthen;
            mOnStrengthenDeviceItem = null;
        }

        private void OnSelectedClick()
        {
            if (mStrengthenGrowthType == StrengthenGrowthType.SGT_Strengthen)
            {
                var items = ItemDataManager.GetInstance().GetItemsByPackageThirdType(EPackageType.Material, ItemTable.eThirdType.DisposableStrengItem);
                if (items.Count <= 0)
                {
                    ItemComeLink.OnLink(330000242, 0, false);
                    return;
                }
            }
            else if (mStrengthenGrowthType == StrengthenGrowthType.SGT_Gtowth)
            {
                var items = ItemDataManager.GetInstance().GetItemsByPackageThirdType(EPackageType.Material, ItemTable.eThirdType.DisposableIncreaseItem);
                if (items.Count <= 0)
                {
                    ItemComeLink.OnLink(330000243, 0, false);
                    return;
                }
            }

            GrowthExpendData expendData = new GrowthExpendData();
            expendData.mStrengthenGrowthType = mStrengthenGrowthType;
            expendData.mOnItemClick = RefreshExpendItemData;

            ClientSystemManager.GetInstance().OpenFrame<SingleUseExpendItemFrame>(FrameLayer.Middle, expendData);
        }

        private void RefreshExpendItemData(ItemData item)
        {
            if (item == null)
            {
                return;
            }

            OnSetBgImg(item.GetQualityInfo().Background);
            OnSetIconImg(item.Icon);
            OnSetCount(string.Format("{0}/1", item.Count));
            OnStrengthenDeviceItem(item);
            OnSetParentGameObject(true);
            SetName(item);
        }
        
        public void InitItem(StrengthenGrowthType strengthenType, OnStrengthenDeviceItem click)
        {
            mStrengthenGrowthType = strengthenType;
            mOnStrengthenDeviceItem = click;

            SetName(null);
        }

        private void SetName(ItemData item)
        {
            if(mName != null)
            {
                if (item == null)
                {
                    if (mStrengthenGrowthType == StrengthenGrowthType.SGT_Strengthen)
                    {
                        mName.text = mStrengthenDesc;
                    }
                    else if (mStrengthenGrowthType == StrengthenGrowthType.SGT_Gtowth)
                    {
                        mName.text = mGrowthDesc;
                    }
                }
                else
                {
                    mName.text = item.GetColorName();
                }
            }
        }

        public void SetItem(ItemData item)
        {
            SetName(item);

            if (item == null)
            {
                OnSetParentGameObject(false);
                OnSetCount(string.Empty);
            }
            else
            {
                OnSetBgImg(item.GetQualityInfo().Background);
                OnSetIconImg(item.Icon);
                OnSetCount(string.Format("{0}/1", item.Count));
                OnSetParentGameObject(true);
            }
        }

        private void OnStrengthenDeviceItem(ItemData item)
        {
            if (mOnStrengthenDeviceItem != null)
            {
                mOnStrengthenDeviceItem.Invoke(item);
            }
        }

        private void OnSetCount(string str)
        {
            if (mCount != null)
            {
                mCount.text = str;
            }
        }

        private void OnSetBgImg(string path)
        {
            if (mItemBgImg != null)
            {
                ETCImageLoader.LoadSprite(ref mItemBgImg, path);
            }
        }

        private void OnSetIconImg(string path)
        {
            if (mItemIconImg != null)
            {
                ETCImageLoader.LoadSprite(ref mItemIconImg, path);
            }
        }

        private void OnSetParentGameObject(bool isFlag)
        {
            if (mItemParent != null)
            {
                mItemParent.CustomActive(isFlag);
            }
        }

        /// <summary>
        /// 填充道具
        /// </summary>
        private ItemData OnFillTheProps()
        {
            ItemData fillThePropItemData = null;

            List<ItemData> mItemDatas = new List<ItemData>();
            var items = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Material);

            if (mStrengthenGrowthType == StrengthenGrowthType.SGT_Strengthen)
            {
                if (items != null)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        var itemData = ItemDataManager.GetInstance().GetItem(items[i]);
                        if (itemData == null)
                        {
                            continue;
                        }

                        if (itemData.ThirdType != ItemTable.eThirdType.DisposableStrengItem)
                        {
                            continue;
                        }

                        mItemDatas.Add(itemData);
                    }
                }
            }
            else if (mStrengthenGrowthType == StrengthenGrowthType.SGT_Gtowth)
            {
                if (items != null)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        var itemData = ItemDataManager.GetInstance().GetItem(items[i]);
                        if (itemData == null)
                        {
                            continue;
                        }

                        if (itemData.ThirdType != ItemTable.eThirdType.DisposableIncreaseItem)
                        {
                            continue;
                        }

                        mItemDatas.Add(itemData);
                    }
                }
            }

            //筛选后的道具
            List<ItemData> filtrateItems = new List<ItemData>();

            filtrateItems = FiltrateItemDatas(mItemDatas, ItemTable.eOwner.ROLEBIND);
           
            if (filtrateItems.Count > 0)
            {
                fillThePropItemData = FiltrateItem(filtrateItems);
            }
            else
            {
                filtrateItems = FiltrateItemDatas(mItemDatas, ItemTable.eOwner.ACCBIND);
               
                if (filtrateItems.Count > 0)
                {
                    fillThePropItemData = FiltrateItem(filtrateItems);
                }
                else
                {
                    filtrateItems = FiltrateItemDatas(mItemDatas, ItemTable.eOwner.NOTBIND);
                  
                    if (filtrateItems.Count > 0)
                    {
                        fillThePropItemData = FiltrateItem(filtrateItems);
                    }
                }
            }

            return fillThePropItemData;
        }

        private List<ItemData> FiltrateItemDatas(List<ItemData> mItemDatas, ItemTable.eOwner owner)
        {
            List<ItemData> filtrateItems = new List<ItemData>();

            for (int i = 0; i < mItemDatas.Count; i++)
            {
                var itemData = mItemDatas[i];
                if (itemData == null)
                {
                    continue;
                }

                //先查找角色绑定
                if (itemData.BindAttr != owner)
                {
                    continue;
                }

                filtrateItems.Add(itemData);
            }

            return filtrateItems;
        }

        private ItemData FiltrateItem(List<ItemData> filtrateItems)
        {
            ItemData fillThePropItemData = null;

            //限时道具
            List<ItemData> limitTimeItems = new List<ItemData>();

            for (int i = 0; i < filtrateItems.Count; i++)
            {
                var itemData = filtrateItems[i];

                if (itemData == null)
                {
                    continue;
                }

                if (itemData.DeadTimestamp <= 0)
                {
                    continue;
                }

                limitTimeItems.Add(itemData);
            }

            if (limitTimeItems.Count > 0)
            {
                limitTimeItems.Sort((ItemData x, ItemData y) =>
                {
                    int timeX = x.DeadTimestamp - (int)TimeManager.GetInstance().GetServerTime();
                    int timeY = y.DeadTimestamp - (int)TimeManager.GetInstance().GetServerTime();

                    return timeX - timeY;
                });

                fillThePropItemData = limitTimeItems[0];
            }
            else
            {
                fillThePropItemData = filtrateItems[0];
            }

            return fillThePropItemData;
        }
    }
}