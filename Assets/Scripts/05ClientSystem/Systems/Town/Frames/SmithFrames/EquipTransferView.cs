using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using Network;

namespace GameClient
{
    public class EquipTransferView : MonoBehaviour, ISmithShopNewView
    {
        [SerializeField] private ComUIListScript mTransferScrollView;

        [SerializeField] private GameObject goTransferComeItemLink;
        [SerializeField] private GameObject goItemParent;
        [SerializeField] private GameObject goCostItemParent;
        [SerializeField] private GameObject goEquipments;
        [SerializeField] private GameObject mGoTransferSelectItemRoot;

        [SerializeField] private Text m_kTransferEquipName;
        [SerializeField] private Text m_kTransferName;

        [SerializeField] private Button btnItemComLink;
        [SerializeField] private Button btnTransfer;

        [SerializeField] private int m_iCostSealID = 200110001;

        private ComItemNew m_kComTransfer;
        private ComItemNew m_kCostTransferItem;
        private List<ItemData> m_kEquipTransferFitStoneData = null;
        private EquipSealData m_kEquipTransferData = null;
        private ComSealEquipmentList m_kComSealEquipmentList = new ComSealEquipmentList();

        private void Awake()
        {
            RegisterEventHander();

            if (btnItemComLink != null)
            {
                btnItemComLink.onClick.RemoveAllListeners();
                btnItemComLink.onClick.AddListener(OnTransferItemComeLink);
            }
            
            if (btnTransfer != null)
            {
                btnTransfer.onClick.RemoveAllListeners();
                btnTransfer.onClick.AddListener(OnTransferClicked);
            }

        }

        private void OnDestroy()
        {
            UnRegisterEventHander();
            UnInitmTransferScrollViewUIList();

            m_kComSealEquipmentList.UnInitialize();

            if (m_kEquipTransferFitStoneData != null)
            {
                m_kEquipTransferFitStoneData.Clear();
            }

            m_kEquipTransferData = null;

            m_kComTransfer = null;
            m_kCostTransferItem = null;
    }

        public void InitView(SmithShopNewLinkData linkData)
        {
            InitInterface(linkData);
        }

        public void OnEnableView()
        {
            RegisterEventHander();

            if (m_kComSealEquipmentList != null)
            {
                m_kComSealEquipmentList.RefreshAllEquipments(SmithShopNewTabType.SSNTT_EQUIPMENTTRANSFER, true);
            }
        }

        public void OnDisableView()
        {
            UnRegisterEventHander();
        }

        #region Event(Delegate)

        private void RegisterEventHander()
        {
            ItemDataManager.GetInstance().onAddNewItem += OnAddNewItem;
            ItemDataManager.GetInstance().onRemoveItem += OnRemoveItem;
            ItemDataManager.GetInstance().onUpdateItem += OnUpdateItem;
        }

        private void UnRegisterEventHander()
        {
            ItemDataManager.GetInstance().onAddNewItem -= OnAddNewItem;
            ItemDataManager.GetInstance().onRemoveItem -= OnRemoveItem;
            ItemDataManager.GetInstance().onUpdateItem -= OnUpdateItem;
        }

        private void OnAddNewItem(List<Item> items)
        {
            if (items == null)
            {
                return;
            }

            for (int i = 0; i < items.Count; i++)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
                if (itemData == null)
                {
                    continue;
                }

                if (itemData.Type == ProtoTable.ItemTable.eType.EQUIP &&
                       (itemData.PackageType == EPackageType.Equip || itemData.PackageType == EPackageType.WearEquip))
                {
                    if (itemData.Type == ProtoTable.ItemTable.eType.EQUIP &&
                     (itemData.PackageType == EPackageType.Equip || itemData.PackageType == EPackageType.WearEquip))
                    {
                        //common
                        m_kComSealEquipmentList.RefreshAllEquipments(SmithShopNewTabType.SSNTT_EQUIPMENTTRANSFER);
                    }
                }
                
            }
        }

        private void OnRemoveItem(ItemData itemData)
        {
            if (itemData == null)
            {
                return;
            }

            if (m_kComSealEquipmentList.Initilized)
            {
                m_kComSealEquipmentList.RefreshAllEquipments(SmithShopNewTabType.SSNTT_EQUIPMENTTRANSFER);
            }
        }

        private void OnUpdateItem(List<Item> items)
        {
            if (items == null)
            {
                return;
            }

            for (int i = 0; i < items.Count; i++)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
                if (itemData == null)
                {
                    continue;
                }

                if (m_kComSealEquipmentList.Initilized)
                {
                    m_kComSealEquipmentList.RefreshAllEquipments(SmithShopNewTabType.SSNTT_EQUIPMENTTRANSFER);
                }
            }
        }

        #endregion

        #region Interface

        private void InitInterface(SmithShopNewLinkData linkData)
        {
            if (m_kComTransfer == null)
            {
                m_kComTransfer = ComItemManager.CreateNew(goItemParent);
            }

            m_kComTransfer.Setup(null, null);

            if (m_kCostTransferItem == null)
            {
                m_kCostTransferItem = ComItemManager.CreateNew(goCostItemParent);
            }

            m_kCostTransferItem.transform.SetAsFirstSibling();

            if (mGoTransferSelectItemRoot != null)
            {
                mGoTransferSelectItemRoot.CustomActive(false);
            }
            
            InitmTransferScrollViewUIList();

            OnAdjustEquipItemChanged(null);
            OnAdjustTransferChanged(null);

            if (m_kComSealEquipmentList != null && !m_kComSealEquipmentList.Initilized)
            {
                m_kComSealEquipmentList.Initialize(goEquipments, OnAdjustTransferItemSelected, SmithShopNewTabType.SSNTT_EQUIPMENTTRANSFER, linkData);
            }
        }

        private void InitmTransferScrollViewUIList()
        {
            if (mTransferScrollView != null)
            {
                mTransferScrollView.Initialize();
                mTransferScrollView.onBindItem += OnBindTransferItemDelegate;
                mTransferScrollView.onItemVisiable += OnnTransferScrollViewOnVisible;
                mTransferScrollView.onItemSelected += OnTransferItemSelectedDelegate;
                mTransferScrollView.onItemChageDisplay += OnTransferItemChangedDisplayDelegate;
            }
        }

        private void UnInitmTransferScrollViewUIList()
        {
            if (mTransferScrollView != null)
            {
                mTransferScrollView.onBindItem -= OnBindTransferItemDelegate;
                mTransferScrollView.onItemVisiable -= OnnTransferScrollViewOnVisible;
                mTransferScrollView.onItemSelected -= OnTransferItemSelectedDelegate;
                mTransferScrollView.onItemChageDisplay -= OnTransferItemChangedDisplayDelegate;
            }
        }

        private TransferItemElement OnBindTransferItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<TransferItemElement>();
        }
        
        private void OnnTransferScrollViewOnVisible(ComUIListElementScript item)
        {
            var element = item.gameObjectBindScript as TransferItemElement;
            if (element != null && item.m_index >= 0 && item.m_index < m_kEquipTransferFitStoneData.Count)
            {
                element.OnItemVisiable(m_kEquipTransferFitStoneData[item.m_index]);
            }
        }

        private void OnTransferItemSelectedDelegate(ComUIListElementScript item)
        {
            var element = item.gameObjectBindScript as TransferItemElement;
            if (element != null)
            {
                m_kEquipTransferData.material = element.ItemData;
                OnAdjustTransferChanged(m_kEquipTransferData);
            }
        }

        private void OnTransferItemChangedDisplayDelegate(ComUIListElementScript item,bool bSelected)
        {
            var element = item.gameObjectBindScript as TransferItemElement;
            if (element != null)
                element.OnChangedDisplay(bSelected);
        }

        private void OnAdjustEquipItemChanged(EquipSealData transferData)
        {
            m_kEquipTransferData = transferData;

            if (transferData == null || transferData.item == null)
            {
                if (null != m_kComTransfer)
                {
                    m_kComTransfer.Setup(null, null);
                }

                if (null != m_kTransferEquipName)
                {
                    m_kTransferEquipName.text = TR.Value("ADJUS_TRANSFER_DEFAULT_NAME");
                }
            }
            else
            {
                if (null != m_kComTransfer)
                {
                    m_kComTransfer.Setup(transferData.item, Utility.OnItemClicked);
                }
                
                if (null != m_kTransferEquipName)
                {
                    m_kTransferEquipName.text = transferData.item.GetColorName();
                }
            }
        }

        private void OnAdjustTransferChanged(EquipSealData transferData)
        {
            mGoTransferSelectItemRoot.CustomActive(transferData != null && null != transferData.material);

            if (null != transferData && null != transferData.material)
            {
                m_kCostTransferItem.Setup(transferData.material, Utility.OnItemClicked);
                m_kTransferName.text = transferData.material.GetColorName();
            }
            else
            {
                m_kTransferName.text = string.Empty;
                m_kCostTransferItem.Setup(null, null);
                m_kTransferName.text = string.Empty;
            }
        }

        private void OnAdjustTransferItemSelected(ItemData itemData)
        {
            EquipSealData data = new EquipSealData();
            data.item = itemData;

            if (null != m_kEquipTransferData)
            {
                if (EquipTransferUtility.IsMatch(itemData, m_kEquipTransferData.material))
                {
                    data.material = m_kEquipTransferData.material;
                }
            }

            OnAdjustEquipItemChanged(data);
            OnAdjustTransferChanged(data);
            OnSetElementAmount();
        }

        #endregion

        #region OnBtnClick

        private void OnTransferItemComeLink()
        {
            ItemComeLink.OnLink(330000160, 0);
        }

        private void OnTransferClicked()
        {
            if (m_kEquipTransferData == null || m_kEquipTransferData.item == null)
            {
                SystemNotifyManager.SysNotifyTextAnimation("请先选择一件装备!");
                return;
            }

            if (m_kEquipTransferData.material != null)
            {
                int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount((int)m_kEquipTransferData.material.TableID);
                int iCostCount = m_kEquipTransferData.material.Count;
                if (iHasCount < iCostCount)
                {
                    SystemNotifyManager.SysNotifyTextAnimation("数量不足!");
                   
                    return;
                }


                TransferConfirmFrameData data = new TransferConfirmFrameData()
                {
                    data = m_kEquipTransferData.material,
                    item = m_kEquipTransferData.item,
                    onOk = OnEquipTransfer,
                };

                ClientSystemManager.GetInstance().OpenFrame<TransferConfirmFrame>(FrameLayer.Middle, data);
            }
            else
            {
                SystemNotifyManager.SysNotifyFloatingEffect("请选择转移石");
            }
        }

        private void OnEquipTransfer(ItemData data, ItemData material)
        {
            OnAdjustEquipItemChanged(null);
            OnAdjustTransferChanged(null);

            ItemData ownMaterialData = ItemDataManager.GetInstance().GetItemByTableID(material.TableID);

            SceneEquipTransferReq msg = new SceneEquipTransferReq
            {
                equid = data.GUID,
                transferId = ownMaterialData.GUID,
            };

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<SceneEquipTransferRes>(msgRet =>
            {
                if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.code);
                }
                else
                {
                    ClientSystemManager.instance.OpenFrame<EquipTransferResultFrame>(FrameLayer.Middle, data);
                }
            });
        }

        private void OnSetElementAmount()
        {
            if (m_kEquipTransferData == null || m_kEquipTransferData.item == null)
            {
                SystemNotifyManager.SysNotifyTextAnimation("请先选择一件装备!");
                return;
            }
            
            mTransferScrollView.ResetContentPosition();
            mTransferScrollView.ResetSelectedElementIndex();

            if (null == m_kEquipTransferData || null == m_kEquipTransferData.item)
            {
                mTransferScrollView.SetElementAmount(0);
            }
            else
            {
                m_kEquipTransferFitStoneData = EquipTransferUtility.GetTransferStones(m_kEquipTransferData.item);
                mTransferScrollView.SetElementAmount(m_kEquipTransferFitStoneData.Count);
            }

            TrySelectedTransferItem();
        }

        private void TrySelectedTransferItem()
        {
            if(m_kEquipTransferData.material != null)
            {
                int iBindIndex = -1;
                for (int i = 0; i < m_kEquipTransferFitStoneData.Count; ++i)
                {
                    if (m_kEquipTransferFitStoneData[i].TableID == m_kEquipTransferData.material.TableID)
                    {
                        iBindIndex = i;
                        break;
                    }
                }

                if (iBindIndex >= 0 && iBindIndex < m_kEquipTransferFitStoneData.Count)
                {
                    if (!mTransferScrollView.IsElementInScrollArea(iBindIndex))
                    {
                        mTransferScrollView.EnsureElementVisable(iBindIndex);
                    }
                    mTransferScrollView.SelectElement(iBindIndex);
                }
                else
                {
                    mTransferScrollView.SelectElement(-1);
                }
            }
        }

        #endregion
    }
}