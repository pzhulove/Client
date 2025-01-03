using ProtoTable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    class EquipmentRemoveView : StrengthGrowthBaseView
    {
        [SerializeField] private GameObject mItemParent;
        [SerializeField] private Button mClearBtn;
        [SerializeField] private StateController mEquipStateCtrl;
        [SerializeField] private ComUIListScript mExpendUIList;

        private List<ItemData> expendItemList = new List<ItemData>();

        private StrengthenGrowthType mStrengthenGrowthType;
        private StrengthenGrowthView mStrengthenGrowthView;
        private ComItemNew mEquipComItem;
        private ItemData mExpendItemData;
        private ItemData mSelectEquipItemData;

        int timeLeft;
        bool bStartCountdown;
        public sealed override void IniteData(SmithShopNewLinkData linkData, StrengthenGrowthType type, StrengthenGrowthView strengthenGrowthView)
        {
            mStrengthenGrowthType = type;
            mStrengthenGrowthView = strengthenGrowthView;
            CreatComitem();
            if (mEquipStateCtrl != null)
            {
                mEquipStateCtrl.Key = "notHasEquip";
            }

            LoadAllExpendItem();
        }

        private void Awake()
        {
            InitExpendUIList();
            RegisterUIEventHandle();
            
            if (mClearBtn != null)
            {
                mClearBtn.onClick.RemoveAllListeners();
                mClearBtn.onClick.AddListener(OnClearBtnClick);
            }
        }

        private void OnDestroy()
        {
            UnInitExpendUIList();
            UnRegisterUIEventHandle();
            mEquipComItem = null;
            mExpendItemData = null;
            mSelectEquipItemData = null;
            mStrengthenGrowthView = null;
            mStrengthenGrowthType =  StrengthenGrowthType.SGT_NONE;
            if (expendItemList != null)
                expendItemList.Clear();
        }

        private void RegisterUIEventHandle()
        {
            StrengthenGrowthView.mOnStrengthenGrowthEquipItemClick += OnStrengthenGrowthEquipItemClick;

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnBreathEquipClearSuccess, OnBreathEquipClearSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnEquipmentListNoEquipment, OnEquipmentListNoEquipment);
        }

        private void UnRegisterUIEventHandle()
        { 
            StrengthenGrowthView.mOnStrengthenGrowthEquipItemClick -= OnStrengthenGrowthEquipItemClick;

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnBreathEquipClearSuccess, OnBreathEquipClearSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnEquipmentListNoEquipment, OnEquipmentListNoEquipment);
        }
        
        private void UpdateGrowthExpengItem(ItemData equipItem)
        {
            if (equipItem == null)
            {
                mExpendItemData = null;
            }
            else
            {
                ItemData expendItemData = null;
                var items = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Material);

                FindBindExpendItemData(items, ref expendItemData);

                //找到了
                if (expendItemData != null)
                {
                    SetExpendItemData(expendItemData);
                }
                else
                {
                    FindUnBindExpendItemData(items, ref expendItemData);

                    //找到了非绑定道具
                    if (expendItemData != null)
                    {
                        SetExpendItemData(expendItemData);
                    }
                    else
                    {
                        mExpendItemData = null;
                    }
                }
            }
        }

        /// <summary>
        /// 查找绑定道具
        /// </summary>
        /// <param name="items"></param>
        /// <param name="expendItemData"></param>
        private void FindBindExpendItemData(List<ulong> items, ref ItemData expendItemData)
        {
            //未找到再找非绑定道具
            for (int i = 0; i < items.Count; i++)
            {
                var data = ItemDataManager.GetInstance().GetItem(items[i]);
                if (data == null)
                    continue;

                if (data.SubType != (int)ItemTable.eSubType.ST_ZENGFU_CLEANUP)
                    continue;

                if (data.BindAttr <= ProtoTable.ItemTable.eOwner.NOTBIND)
                {
                    continue;
                }
                
                data.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

                //道具过期了 过滤掉
                if (timeLeft <= 0 && bStartCountdown)
                {
                    continue;
                }

                expendItemData = data;
                break;
            }
        }

        /// <summary>
        /// 非查找绑定道具
        /// </summary>
        /// <param name="items"></param>
        /// <param name="expendItemData"></param>
        private void FindUnBindExpendItemData(List<ulong> items, ref ItemData expendItemData)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var data = ItemDataManager.GetInstance().GetItem(items[i]);
                if (data == null)
                    continue;

                if (data.SubType != (int)ItemTable.eSubType.ST_ZENGFU_CLEANUP)
                    continue;

                if (data.BindAttr != ProtoTable.ItemTable.eOwner.NOTBIND)
                {
                    continue;
                }
                
                data.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

                //道具过期了 过滤掉
                if (timeLeft <= 0 && bStartCountdown)
                {
                    continue;
                }

                expendItemData = data;
                break;
            }
        }

        private void SetExpendItemData(ItemData itemData)
        {
            mExpendItemData = itemData;
        }
        
        private void OnStrengthenGrowthEquipItemClick(ItemData itemData, StrengthenGrowthType eStrengthenGrowthType)
        {
            if (itemData == null)
            {
                return;
            }

            mSelectEquipItemData = itemData;
            if (eStrengthenGrowthType == mStrengthenGrowthType)
            {
                if (mEquipStateCtrl != null)
                {
                    mEquipStateCtrl.Key = "hasEquip";
                }

                UpdateEquipItem(itemData);
                UpdateGrowthExpengItem(itemData);
                OnSetElementAmount();
            }
        }

        private void OnBreathEquipClearSuccess(UIEvent uiEvent)
        {
            LoadAllExpendItem();
            if (mStrengthenGrowthView != null)
            {
                mStrengthenGrowthView.RefreshAllEquipments();
            }
        }

        private void OnEquipmentListNoEquipment(UIEvent uiEvent)
        {
            mExpendItemData = null;
            mSelectEquipItemData = null;

            UpdateEquipItem(mSelectEquipItemData);

            if (mEquipStateCtrl != null)
            {
                mEquipStateCtrl.Key = "notHasEquip";
            }
        }
        
        private void CreatComitem()
        {
            if (mEquipComItem == null)
            {
                mEquipComItem = ComItemManager.CreateNew(mItemParent);
            }

            mEquipComItem.Setup(null, null);
        }
        
        private void UpdateEquipItem(ItemData itemData)
        {
            if (mEquipComItem != null)
            {
                mEquipComItem.Setup(itemData, Utility.OnItemClicked);
            }
        }

        private void InitExpendUIList()
        {
            if(mExpendUIList != null)
            {
                mExpendUIList.Initialize();
                mExpendUIList.onBindItem += OnBindItemDelegate;
                mExpendUIList.onItemVisiable += OnItemVisiableDelegate;
                mExpendUIList.onItemSelected += OnItemSelectedDelegate;
                mExpendUIList.onItemChageDisplay += OnItemChangeDisplayDelegate;
            }
        }

        private void UnInitExpendUIList()
        {
            if (mExpendUIList != null)
            {
                mExpendUIList.onBindItem -= OnBindItemDelegate;
                mExpendUIList.onItemVisiable -= OnItemVisiableDelegate;
                mExpendUIList.onItemSelected -= OnItemSelectedDelegate;
                mExpendUIList.onItemChageDisplay -= OnItemChangeDisplayDelegate;
            }
        }

        private CommonImplementItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<CommonImplementItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            CommonImplementItem commonImplementItem = item.gameObjectBindScript as CommonImplementItem;
            if(commonImplementItem != null && item.m_index >= 0 && item.m_index < expendItemList.Count)
            {
                ulong guid = expendItemList[item.m_index].GUID;
                commonImplementItem.OnItemVisiable(guid);
            }
        }

        private void OnItemSelectedDelegate(ComUIListElementScript item)
        {
            CommonImplementItem commonImplementItem = item.gameObjectBindScript as CommonImplementItem;
            if(commonImplementItem != null)
            {
                SetExpendItemData(commonImplementItem.ItemData);
            }
        }

        private void OnItemChangeDisplayDelegate(ComUIListElementScript item,bool bIsSelected)
        {
            CommonImplementItem commonImplementItem = item.gameObjectBindScript as CommonImplementItem;
            if (commonImplementItem != null)
            {
                commonImplementItem.OnChangeDisplay(bIsSelected);
            }
        }

        private void LoadAllExpendItem()
        {
            if (expendItemList == null)
                expendItemList = new List<ItemData>();
            expendItemList.Clear();

            var items = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Material);
            for (int i = 0; i < items.Count; i++)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(items[i]);
                if (itemData == null)
                    continue;

                if (itemData.SubType != (int)ItemTable.eSubType.ST_ZENGFU_CLEANUP)
                {
                    continue;
                }

                itemData.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

                //失效了
                if (timeLeft <= 0 && bStartCountdown)
                {
                    continue;
                }

                expendItemList.Add(itemData);
            }
        }

        private void OnSetElementAmount()
        {
            if (mExpendUIList != null)
            {
                mExpendUIList.ResetSelectedElementIndex();
                mExpendUIList.SetElementAmount(expendItemList.Count);
            }

            TrySetDefaultExpendItem();
        }

        private void TrySetDefaultExpendItem()
        {
            if(mExpendItemData == null)
            {
                return;
            }

            int iSelectedIndex = -1;
            for (int i = 0; i < expendItemList.Count; i++)
            {
                if (expendItemList[i].GUID != mExpendItemData.GUID)
                    continue;

                iSelectedIndex = i;
                break;
            }

            if(iSelectedIndex >= 0 && iSelectedIndex < expendItemList.Count)
            {
                if (mExpendUIList != null)
                {
                    if (!mExpendUIList.IsElementInScrollArea(iSelectedIndex))
                    {
                        mExpendUIList.EnsureElementVisable(iSelectedIndex);
                    }

                    mExpendUIList.SelectElement(iSelectedIndex);
                }
            }
        }

        private void OnClearBtnClick()
        {
            if (mSelectEquipItemData == null)
            {
                SystemNotifyManager.SysNotifyTextAnimation("请选择装备");
                return;
            }

            if (mSelectEquipItemData != null && mSelectEquipItemData.bLocked)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("已加锁的装备无法清除");
                return;
            }

            if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
            {
                return;
            }

            if (mExpendItemData == null)
            {
                ItemComeLink.OnLink(300000206, 0);
                return;
            }

            if (mExpendItemData != null)
            {
                ItemData expendItemData = ItemDataManager.GetInstance().GetItem(mExpendItemData.GUID);
                if (expendItemData != null)
                {
                    int timeLeft;
                    bool bStartCountdown;
                    mExpendItemData.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

                    //失效了
                    if (timeLeft <= 0 && bStartCountdown)
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("failure_item"));
                        return;
                    }
                }
                else
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("failure_item"));
                    return;
                }
            }

            EquipGrowthDataManager.GetInstance().OnSceneEquipEnhanceClear(mSelectEquipItemData, (UInt32)mExpendItemData.TableID);
        }
    }
}