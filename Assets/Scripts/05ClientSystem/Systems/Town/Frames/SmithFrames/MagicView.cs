using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    enum MagicCardState
    {
        None,
        CanBeSet,
        HasBeenSet,
    }
    class MagicView : MonoBehaviour,ISmithShopNewView
    {
        [SerializeField] private StateController mStateControl;
        [SerializeField] private GameObject mHasBeenSetCardParent;
        [SerializeField] private Text mHasBeenSetCardName;
        [SerializeField] private Text mHasBeenSetCardAttr;

        [SerializeField] private GameObject mItemParent;
        [SerializeField] private GameObject mEnchantCardsObjects;
        [SerializeField] private GameObject mScrollView;
        [SerializeField] private GameObject mEquipScrollView;
        [SerializeField] private Button mBtnAddMagic;
        [SerializeField] private Button mEnchantmentCardMergeBtn;
        [SerializeField] private Button mEnchantmentCardUpgradeBtn;
        [SerializeField] private Button mEnchantmentBookBtn;

        private ComMagicCardEnchantItemList m_kComMagicCardEnchantItemList = new ComMagicCardEnchantItemList();
        private EquipmentList m_kComEquipmentList = new EquipmentList();
        private EnchantmentsFunctionData data = new EnchantmentsFunctionData();

        private PrecEnchantmentCard mPrecEnchantmentCard;
        private MagicCardState mMCState;
        private ComItemNew mHasBeenSetCardComItem;
        private ComItemNew mEquipmentItem;
        private ItemData mCurrentBeadCardItem = null; //装备身上已镶嵌的宝珠道具
        private ItemData mToBeInlaidBeadItemData = null;//待镶嵌的宝珠道具

        private void Awake()
        {
            RegisterUIEvent();

            if (mBtnAddMagic != null)
            {
                mBtnAddMagic.onClick.RemoveAllListeners();
                mBtnAddMagic.onClick.AddListener(OnAddMagicClick);
            }

            if(mEnchantmentCardUpgradeBtn != null)
            {
                mEnchantmentCardUpgradeBtn.onClick.RemoveAllListeners();
                mEnchantmentCardUpgradeBtn.onClick.AddListener(OnEnchantmentCardUpgradeBtnClick);
            }

            if(mEnchantmentCardMergeBtn != null)
            {
                mEnchantmentCardMergeBtn.onClick.RemoveAllListeners();
                mEnchantmentCardMergeBtn.onClick.AddListener(OnEnchantmentCardMergeBtnClick);
            }

            if(mEnchantmentBookBtn != null)
            {
                mEnchantmentBookBtn.onClick.RemoveAllListeners();
                mEnchantmentBookBtn.onClick.AddListener(OnEnchantmentCardBookClick);
            }
        }

        private void OnDestroy()
        {
            if (mBtnAddMagic != null)
            {
                mBtnAddMagic.onClick.RemoveListener(OnAddMagicClick);
            }

            if (mEnchantmentCardUpgradeBtn != null)
            {
                mEnchantmentCardUpgradeBtn.onClick.RemoveListener(OnEnchantmentCardUpgradeBtnClick);
            }

            if (mEnchantmentBookBtn != null)
            {
                mEnchantmentBookBtn.onClick.RemoveListener(OnEnchantmentCardBookClick);
            }

            mPrecEnchantmentCard = null;
            mMCState = MagicCardState.None;
            mHasBeenSetCardComItem = null;
            mEquipmentItem = null;
            mCurrentBeadCardItem = null; 
            mToBeInlaidBeadItemData = null;
            m_kComEquipmentList.UnInitialize();
            m_kComMagicCardEnchantItemList.UnInitialize();
            data = null;

            UnRegisterUIEvent();
        }

        private void OnEnchantmentCardUpgradeBtnClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<EnchantmentCardUpgradeFrame>(FrameLayer.Middle);
        }

        private void OnEnchantmentCardMergeBtnClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<EnchantmentCardMergeNewFrame>();
        }

        private void OnEnchantmentCardBookClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<EnchantmentsBookFrame>();
        }

        #region Delegate (UIEvent)

        private void RegisterDelegateHandler()
        {
            ItemDataManager.GetInstance().onAddNewItem += OnAddNewItem;
            ItemDataManager.GetInstance().onRemoveItem += OnRemoveItem;
            ItemDataManager.GetInstance().onUpdateItem += OnUpdateItem;
        }

        private void UnRegisterDelegateHandler()
        {
            ItemDataManager.GetInstance().onAddNewItem -= OnAddNewItem;
            ItemDataManager.GetInstance().onRemoveItem -= OnRemoveItem;
            ItemDataManager.GetInstance().onUpdateItem -= OnUpdateItem;
        }

        private void RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAddMagicSuccess, OnSlotItemsEnchantChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnEnchantCardSelected, OnEnchantCardSelected);

            RegisterDelegateHandler();
        }

        private void UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAddMagicSuccess, OnSlotItemsEnchantChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnEnchantCardSelected, OnEnchantCardSelected);

            UnRegisterDelegateHandler();
        }

        private void OnSlotItemsEnchantChanged(UIEvent uiEvent)
        {
            if (data.leftItem != null)
            {
                data.leftItem = null;
            }

            if (data.rightItem != null)
            {
                data.rightItem = ItemDataManager.GetInstance().GetItem(data.rightItem.GUID);
            }

            if (data.leftItem != null && data.rightItem != null && data.leftItem.GUID == data.rightItem.GUID && data.leftItem.Count < 2)
            {
                data.rightItem = null;
            }

            OnItemChanged();

            m_kComEquipmentList.RefreshAllEquipments();
        }

        private void OnEnchantCardSelected(UIEvent uiEvent)
        {
            ItemData itemData = uiEvent.Param1 as ItemData;
            OnEnchantCardChanged(itemData);
        }

        private void OnAddNewItem(List<Item> items)
        {
            if (items == null)
            {
                return;
            }

            bool bNeedRefreshEquipments = false;
            bool bNeedRefreshEnchantCards = false;

            for (int i = 0; i < items.Count; i++)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
                if (itemData == null)
                {
                    continue;
                }

                if (itemData.Type == ProtoTable.ItemTable.eType.EQUIP &&
                   (itemData.PackageType == EPackageType.Equip || 
                    itemData.PackageType == EPackageType.WearEquip))
                {
                    bNeedRefreshEquipments = true; ;
                }

                if (!bNeedRefreshEnchantCards && m_kComMagicCardEnchantItemList.HasObject(itemData.GUID))
                {
                    var magicData = Utility._TryAddMagicCard(items[i].uid);
                    if (magicData != null)
                    {
                        bNeedRefreshEnchantCards = true;
                    }
                }
            }

            if (bNeedRefreshEquipments)
            {
                m_kComEquipmentList.RefreshAllEquipments();
            }
            if (bNeedRefreshEnchantCards)
            {
                m_kComMagicCardEnchantItemList.RefreshAllEquipments();
            }
        }

        private void OnRemoveItem(ItemData itemData)
        {
            if (itemData == null)
            {
                return;
            }

            ComEquipment comEquipment = m_kComEquipmentList.GetEquipment(itemData.GUID);
            if (comEquipment != null)
            {
                m_kComEquipmentList.RefreshAllEquipments();
            }

            //addmagic
            if (m_kComMagicCardEnchantItemList.HasObject(itemData.GUID))
            {
                m_kComMagicCardEnchantItemList.RefreshAllEquipments();
            }
        }

        private void OnUpdateItem(List<Item> items)
        {
            if (items == null)
            {
                return;
            }

            bool bNeedRefreshEnchantCard = false;

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] == null)
                {
                    continue;
                }

                ItemData itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
                if (itemData != null)
                {
                    if (!bNeedRefreshEnchantCard && m_kComMagicCardEnchantItemList.HasObject(itemData.GUID))
                    {
                        bNeedRefreshEnchantCard = true;
                    }
                }
            }

            if (bNeedRefreshEnchantCard)
            {
                m_kComMagicCardEnchantItemList.RefreshAllEquipments();
            }
        }

        #endregion

        public void InitView(SmithShopNewLinkData linkData)
        {
            if (m_kComMagicCardEnchantItemList != null)
            {
                m_kComMagicCardEnchantItemList.Initialize(mScrollView, OnEnchantCardChanged, linkData, data);
            }
            
            if (m_kComEquipmentList != null && !m_kComEquipmentList.Initilized)
            {
                m_kComEquipmentList.Initialize(mEquipScrollView, OnItemSelected, linkData);
            }
        }

        public void OnEnableView()
        {
            RegisterDelegateHandler();
            m_kComEquipmentList.RefreshAllEquipments();
        }

        public void OnDisableView()
        {
            UnRegisterDelegateHandler();
        }
        
        private void OnEnchantCardChanged(ItemData itemData)
        {
            data.leftItem = itemData;
        }

        private void OnItemSelected(ItemData itemData)
        {
            if (itemData == null)
            {
                return;
            }

            data.leftItem = null;
            data.rightItem = itemData;

            m_kComMagicCardEnchantItemList.ClearSelectedItem();
            m_kComMagicCardEnchantItemList.RefreshAllEquipments();

            OnItemChanged();
        }

        private void OnItemChanged()
        {
            if (data.leftItem != null && data.rightItem != null)
            {
                var magicItem = TableManager.GetInstance().GetTableItem<ProtoTable.MagicCardTable>((int)data.leftItem.TableID);
                if (magicItem == null || !magicItem.Parts.Contains((int)data.rightItem.EquipWearSlotType))
                {
                    data.leftItem = null;
                }
            }

            if (mEquipmentItem == null)
            {
                mEquipmentItem = ComItemManager.CreateNew(mItemParent);
            }

            if (data.rightItem != null)
            {
                mEquipmentItem.Setup(data.rightItem, null);
            }
            else
            {
                mEquipmentItem.Setup(null, null);
            }

            RefreshMagicView(data.rightItem.mPrecEnchantmentCard);
        }
       
        public void RefreshMagicView(PrecEnchantmentCard precEnchantmentCard)
        {
            if (precEnchantmentCard == null)
            {
                return;
            }

            mPrecEnchantmentCard = precEnchantmentCard;

            mCurrentBeadCardItem = ItemDataManager.CreateItemDataFromTable(mPrecEnchantmentCard.iEnchantmentCardID);
            if (mCurrentBeadCardItem != null)
            {
                mCurrentBeadCardItem.mPrecEnchantmentCard.iEnchantmentCardLevel = mPrecEnchantmentCard.iEnchantmentCardLevel;
            }

            if (mCurrentBeadCardItem != null)
            {
                SetMagicState(MagicCardState.HasBeenSet, mCurrentBeadCardItem);
            }
            else
            {
                SetMagicState(MagicCardState.CanBeSet);
            }
        }

        public void UpdateMagicSteteControl()
        {
            switch (mMCState)
            {
                case MagicCardState.None:
                    break;
                case MagicCardState.CanBeSet:
                    mStateControl.Key = "CanBeSet";
                    break;
                case MagicCardState.HasBeenSet:
                    mStateControl.Key = "HasBeenSet";
                    RefreshHasBeenSetInfo();
                    break;
            }
        }
        
        /// <summary>
        /// 刷新已镶嵌状态的展示信息
        /// </summary>
        public void RefreshHasBeenSetInfo()
        {
            if (mCurrentBeadCardItem != null)
            {
                if (mHasBeenSetCardComItem == null)
                {
                    mHasBeenSetCardComItem = ComItemManager.CreateNew(mHasBeenSetCardParent);
                }

                mHasBeenSetCardComItem.Setup(mCurrentBeadCardItem, Utility.OnItemClicked);

                mHasBeenSetCardName.text = mCurrentBeadCardItem.GetColorName();

                mHasBeenSetCardAttr.text = EnchantmentsCardManager.GetInstance().GetEnchantmentCardAttributesDesc(mCurrentBeadCardItem.TableID, mCurrentBeadCardItem.mPrecEnchantmentCard.iEnchantmentCardLevel);
            }
        }
        
        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="state"></param>
        public void SetMagicState(MagicCardState state, ItemData toBeInlaidBead = null)
        {
            mMCState = state;
            mToBeInlaidBeadItemData = toBeInlaidBead;

            UpdateMagicSteteControl();
        }
        
        #region Button

        private void OnAddMagicClick()
        {
            if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
            {
                return;
            }

            if (data != null)
            {
                if (data.leftItem != null && data.rightItem != null && data.rightItem.mPrecEnchantmentCard != null)
                {
                    var magicItem = TableManager.GetInstance().GetTableItem<ProtoTable.MagicCardTable>((int)data.rightItem.mPrecEnchantmentCard.iEnchantmentCardID);
                    if (magicItem != null)
                    {
                        CommonReplaceData commonReplaceData = new CommonReplaceData();
                        commonReplaceData.commonReplaceType = CommonReplaceType.CRT_MAGICCARD;
                        commonReplaceData.oldItemData = data.rightItem;
                        commonReplaceData.newItemData = data.leftItem;
                        commonReplaceData.callBack = OnSendMosiacMagicCard;

                        ClientSystemManager.GetInstance().OpenFrame<CommonReplaceFrame>(FrameLayer.Middle, commonReplaceData);
                    }
                    else
                    {
                        OnSendMosiacMagicCard();

                        if (mBtnAddMagic != null)
                        {
                            mBtnAddMagic.enabled = false;

                            InvokeMethod.Invoke(this, 1.00f, () =>
                            {
                                if (mBtnAddMagic != null)
                                {
                                    mBtnAddMagic.enabled = true;
                                }
                            });
                        }
                    }
                }
                else
                {
                    if (data.rightItem == null)
                    {
                        SystemNotifyManager.SystemNotify(1069);
                    }
                    else
                    {
                        SystemNotifyManager.SystemNotify(1068);
                    }
                }
            }
        }

        private void OnSendMosiacMagicCard()
        {
            if (data.leftItem != null && data.rightItem != null)
            {
                EnchantmentsCardManager.GetInstance().SendAddMagic(data.leftItem.GUID, data.rightItem.GUID);
            }
            else
            {
                if (data.leftItem == null)
                {
                    Logger.LogErrorFormat("SmithShopFrame [OnSendMosiacMagicCard] data.leftItem is Null");
                }

                if (data.rightItem == null)
                {
                    Logger.LogErrorFormat("SmithShopFrame [OnSendMosiacMagicCard] data.rightItem is Null");
                }
            }
        }
        #endregion
    }
}