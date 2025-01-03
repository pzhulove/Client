using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using Protocol;
using Network;

namespace GameClient
{
    class ComEquipSealView : MonoBehaviour,ISmithShopNewView
    {
        [SerializeField] private ComUIListScript mEquipmentUIList;

        [SerializeField] private GameObject goSealComeItemLink;
        [SerializeField] private GameObject goItemParent;
        [SerializeField] private GameObject goCostItemParent;
        [SerializeField] private GameObject goHint;

        [SerializeField] private Text m_kSealTimes;
        [SerializeField] private Text m_kCostSealCount;
        [SerializeField] private Text m_kSealName;
        [SerializeField] private Text m_kItemName;

        [SerializeField] private Button mBtnItemComLink;
        [SerializeField] private Button mBtnSeal;

        [SerializeField] private int m_iCostSealID = 200110001;

        private List<ItemData> equipments = new List<ItemData>();
        private SmithShopNewLinkData linkData = null;
        private ComItemNew m_kComSeal;
        private ComItemNew costSealItem;
        private ulong m_uiLastSelectedId = 0;
        private ItemData m_keep_item = null;
        private EquipSealData m_kEquipSealData = null;
        private void Awake()
        {
            RegisterEventHander();
            InitEquipmentUIList();

            if (mBtnItemComLink != null)
            {
                mBtnItemComLink.onClick.RemoveAllListeners();
                mBtnItemComLink.onClick.AddListener(OnSealItemComeLink);
            }

            if (mBtnSeal != null)
            {
                mBtnSeal.onClick.RemoveAllListeners();
                mBtnSeal.onClick.AddListener(OnSealClicked);
            }
        }

        private void OnDestroy()
        {
            UnRegisterEventHander();
            UnInitEquipmentUIList();
            equipments.Clear();
            linkData = null;
            m_kComSeal = null;
            costSealItem = null;
            m_uiLastSelectedId = 0;
            m_keep_item = null;
            m_kEquipSealData = null;
        }

        public void InitView(SmithShopNewLinkData linkData)
        {
            this.linkData = linkData;

            InitInterface();

            OnAdjustSealChanged(null);

            _LoadAllEquipments();
            _BindLinkData();
        }

        public void OnEnableView()
        {
            RegisterEventHander();
            RefreshAllEquipments();
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
                    //common
                    _LoadAllEquipments(); 
                }

                if (m_iCostSealID == (int)itemData.TableID)
                {
                    OnAdjustSealChanged(m_kEquipSealData);
                }

            }
        }

        private void OnRemoveItem(ItemData itemData)
        {
            if (itemData == null)
            {
                return;
            }

            _LoadAllEquipments();

            if (m_iCostSealID == (int)itemData.TableID)
            {
                OnAdjustSealChanged(m_kEquipSealData);
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

                _LoadAllEquipments();

                //qualitychanged
                if (m_kEquipSealData != null &&
                             m_kEquipSealData.item != null &&
                             m_kEquipSealData.item.GUID == itemData.GUID)
                {
                    m_kEquipSealData.item = itemData;
                    OnAdjustSealChanged(m_kEquipSealData);
                }

                if (m_iCostSealID == itemData.TableID)
                {
                    OnAdjustSealChanged(m_kEquipSealData);
                }
            }
        }

        #endregion

        private void InitInterface()
        {
            if (m_kComSeal == null)
            {
                m_kComSeal = ComItemManager.CreateNew(goItemParent);
            }

            m_kComSeal.Setup(null, null);

            if (costSealItem == null)
            {
                costSealItem = ComItemManager.CreateNew(goCostItemParent);
            }

            ItemData costSealData = GameClient.ItemDataManager.CreateItemDataFromTable(m_iCostSealID);
            costSealItem.Setup(costSealData, Utility.OnItemClicked);
            costSealItem.transform.SetAsFirstSibling();

            if (m_kSealName != null)
            {
                m_kSealName.text = costSealData.GetColorName();
            }
        }

        private void OnEquipmentItemSelected(ItemData itemData)
        {
            if (itemData != null && ComSealEquipment.CheckCanSeal(itemData))
            {
                if (m_uiLastSelectedId != itemData.GUID)
                {
                    m_uiLastSelectedId = itemData.GUID;
                    SendSealMaterialInfo(itemData);
                }
            }
            else
            {
                OnAdjustSealChanged(null);
            }
        }

        #region ComUILitst

        public void RefreshAllEquipments()
        {
            _LoadAllEquipments();
            _TrySetDefaultEquipment();
        }

        private void InitEquipmentUIList()
        {
            if (mEquipmentUIList != null)
            {
                mEquipmentUIList.Initialize();
                mEquipmentUIList.onBindItem += _OnBindItemDelegate;
                mEquipmentUIList.onItemVisiable += _OnItemVisiableDelegate;
                mEquipmentUIList.onItemSelected += _OnItemSelectedDelegate;
                mEquipmentUIList.onItemChageDisplay += _OnItemChangeDisplayDelegate;
            }
        }

        private void UnInitEquipmentUIList()
        {
            if (mEquipmentUIList != null)
            {
                mEquipmentUIList.onBindItem -= _OnBindItemDelegate;
                mEquipmentUIList.onItemVisiable -= _OnItemVisiableDelegate;
                mEquipmentUIList.onItemSelected -= _OnItemSelectedDelegate;
                mEquipmentUIList.onItemChageDisplay -= _OnItemChangeDisplayDelegate;
            }
        }

        private ComSealEquipment _OnBindItemDelegate(GameObject itemObject)
        {
            ComSealEquipment comSealEquipment = itemObject.GetComponent<ComSealEquipment>();
            return comSealEquipment;
        }

        private void _OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as ComSealEquipment;
            if (current != null && item.m_index >= 0 && item.m_index < equipments.Count)
            {
                current.OnItemVisible(equipments[item.m_index], SmithShopNewTabType.SSNTT_EQUIPMENTSEAL,true);
            }
        }

        private void _OnItemSelectedDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as ComSealEquipment;
            ComSealEquipment.ms_selected = (current == null) ? null : current.ItemData;

            OnEquipmentItemSelected(ComSealEquipment.ms_selected);
        }

        private void _OnItemChangeDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            var current = item.gameObjectBindScript as ComSealEquipment;
            if (current != null)
            {
                current.OnItemChangeDisplay(bSelected);
            }
        }

        public void _LoadAllEquipments()
        {
            equipments.Clear();

            var uids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Equip);
            for (int i = 0; i < uids.Count; ++i)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(uids[i]);

                if(itemData == null)
                    continue;

                //租赁装备
                if(itemData.IsLease == true)
                    continue;

                //在未启用的装备方案中
                if(itemData.IsItemInUnUsedEquipPlan == true)
                    continue;

                equipments.Add(itemData);
                
            }

            uids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
            for (int i = 0; i < uids.Count; ++i)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(uids[i]);
                if (itemData != null && !itemData.IsLease && itemData.Type != ProtoTable.ItemTable.eType.FUCKTITTLE)
                {
                    equipments.Add(itemData);
                }
            }

            equipments.RemoveAll(x =>
            {
                if (x.EquipType == EEquipType.ET_BREATH)
                {
                    return true;
                }

                if (x.isInSidePack)
                    return true;

                if (x.PackageType != EPackageType.Equip)
                {
                    return true;
                }

                return !ComSealEquipment.CheckCanSeal(x);
            });

            equipments.Sort(_SortEquipments);

            mEquipmentUIList.SetElementAmount(equipments.Count);
        }

        private void _BindLinkData()
        {
            if (linkData != null)
            {
                if (linkData.itemData != null)
                {
                    _TrySelectedItem(linkData.itemData.GUID);
                }

                linkData = null;
            }
            else
            {
                _TrySetDefaultEquipment();
            }
        }

        private void _TrySelectedItem(ulong guid)
        {
            int iBindIndex = -1;
            for (int i = 0; i < equipments.Count; ++i)
            {
                if (equipments[i].GUID == guid)
                {
                    iBindIndex = i;
                    break;
                }
            }

            _SetSelectedItem(iBindIndex);
        }

        private void _SetSelectedItem(int iBindIndex)
        {
            if (iBindIndex >= 0 && iBindIndex < equipments.Count)
            {
                ComSealEquipment.ms_selected = equipments[iBindIndex];
                if (!mEquipmentUIList.IsElementInScrollArea(iBindIndex))
                {
                    mEquipmentUIList.EnsureElementVisable(iBindIndex);
                }
                mEquipmentUIList.SelectElement(iBindIndex);
            }
            else
            {
                ComSealEquipment.ms_selected = null;
                mEquipmentUIList.SelectElement(-1);
            }

            OnEquipmentItemSelected(ComSealEquipment.ms_selected);
        }

        private void _TrySetDefaultEquipment()
        {
            if (ComSealEquipment.ms_selected != null)
            {
                ComSealEquipment.ms_selected = ItemDataManager.GetInstance().GetItem(ComSealEquipment.ms_selected.GUID);
                if (ComSealEquipment.ms_selected != null && !HasObject(ComSealEquipment.ms_selected.GUID))
                {
                    ComSealEquipment.ms_selected = null;
                }
            }

            int iBindIndex = -1;
            if (ComSealEquipment.ms_selected != null)
            {
                for (int i = 0; i < equipments.Count; ++i)
                {
                    if (equipments[i].GUID == ComSealEquipment.ms_selected.GUID)
                    {
                        iBindIndex = i;
                        break;
                    }
                }
            }
            else
            {
                if (equipments.Count > 0)
                {
                    iBindIndex = 0;
                }
            }

            _SetSelectedItem(iBindIndex);
        }

        public bool HasObject(ulong guid)
        {
            for (int i = 0; i < equipments.Count; ++i)
            {
                if (equipments[i] != null && equipments[i].GUID == guid)
                {
                    return true;
                }
            }
            return false;
        }

        private int _SortEquipments(ItemData left, ItemData right)
        {
            if (left.PackageType != right.PackageType)
            {
                return (int)right.PackageType - (int)left.PackageType;
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

        #endregion

        public void SendSealMaterialInfo(ItemData a_item)
        {
            SceneCheckSealEquipReq msg = new SceneCheckSealEquipReq
            {
                uid = a_item.GUID
            };
            m_keep_item = a_item;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<SceneCheckSealEquipRet>(msgRet => 
            {
                if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.code);
                }
                else
                {
                    if (msgRet.needNum <= 0)
                    {
                        SystemNotifyManager.SysNotifyMsgBoxOK(TR.Value("equip_seal_data_error"));
                    }
                    else
                    {
                        EquipSealData equipSealData = new EquipSealData
                        {
                            item = m_keep_item,
                            material = ItemDataManager.CreateItemDataFromTable((int)msgRet.matID)
                        };
                        equipSealData.material.Count = msgRet.needNum;

                        OnAdjustSealChanged(equipSealData);
                    }
                }
            });
        }

        //[MessageHandle(SceneCheckSealEquipRet.MsgID)]
        //void OnHandleSceneCheckSealEquipRet(MsgDATA msg)
        //{
        //    SceneCheckSealEquipRet msgRet = new SceneCheckSealEquipRet();
        //    msgRet.decode(msg.bytes);

        //    if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
        //    {
        //        SystemNotifyManager.SystemNotify((int)msgRet.code);
        //    }
        //    else
        //    {
        //        if (msgRet.needNum <= 0)
        //        {
        //            SystemNotifyManager.SysNotifyMsgBoxOK(TR.Value("equip_seal_data_error"));
        //        }
        //        else
        //        {
        //            EquipSealData equipSealData = new EquipSealData
        //            {
        //                item = m_keep_item,
        //                material = ItemDataManager.CreateItemDataFromTable((int)msgRet.matID)
        //            };
        //            equipSealData.material.Count = msgRet.needNum;
                    
        //            OnAdjustSealChanged(equipSealData);
        //        }
        //    }
        //}
        
        private void OnAdjustSealChanged(EquipSealData equipSealData)
        {
            m_kEquipSealData = equipSealData;

            int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount((int)m_iCostSealID);
            int iCostCount = (equipSealData == null || equipSealData.material == null) ? 1 : equipSealData.material.Count;

            if (equipSealData == null)
            {
                if (null != m_kComSeal)
                    m_kComSeal.Setup(null, null);

                if (null != m_kSealTimes)
                    m_kSealTimes.text = string.Format("{0}次", 0);

                if (m_kItemName != null)
                    m_kItemName.text = string.Empty;
            }
            else
            {
                if (null != m_kComSeal)
                    m_kComSeal.Setup(equipSealData.item, Utility.OnItemClicked);
                if (equipSealData.item != null)
                {
                    if (null != m_kSealTimes)
                        m_kSealTimes.text = string.Format("{0}次", equipSealData.item.RePackTime);

                    if (m_kItemName != null)
                        m_kItemName.text = equipSealData.item.GetColorName();
                }
                else
                {
                    if (null != m_kSealTimes)
                        m_kSealTimes.text = string.Format("{0}次", 0);

                    if (m_kItemName != null)
                        m_kItemName.text = string.Empty;
                }

            }

            if (null != m_kCostSealCount)
            {
                m_kCostSealCount.enabled = equipSealData != null;
                m_kCostSealCount.text = string.Format("{0}/{1}", iHasCount, iCostCount);
                m_kCostSealCount.color = iHasCount >= iCostCount ? Color.white : Color.red;
            }
            goSealComeItemLink.CustomActive(iHasCount < iCostCount);
           
            if (null != m_kCostSealCount && null != goHint)
                goHint.CustomActive(m_kCostSealCount.enabled);
            if (null != goSealComeItemLink && null != costSealItem)
                costSealItem.SetShowNotEnoughState(goSealComeItemLink.activeSelf);
        }
        
        private void OnSealItemComeLink()
        {
            ItemComeLink.OnLink(m_iCostSealID, 0);
        }

        private void OnSealClicked()
        {
            if (m_kEquipSealData == null || m_kEquipSealData.item == null)
            {
                SystemNotifyManager.SysNotifyTextAnimation("请先选择一件装备!");
                return;
            }

            if (m_kEquipSealData.material != null)
            {
                int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount((int)m_kEquipSealData.material.TableID);
                int iCostCount = m_kEquipSealData.material.Count;
                if (iHasCount < iCostCount)
                {
                    ItemComeLink.OnLink(200110001, iCostCount - iHasCount, true, () =>
                    {
                        OnSceneSealEquipReq();
                    });

                    return;
                }

                OnSceneSealEquipReq();
            }
        }

        private void OnSceneSealEquipReq()
        {
            string mContent = string.Empty;
            //判断装备镶嵌的铭文是否都是低品阶
            bool qualityFollowingFive = InscriptionMosaicDataManager.GetInstance().CheckEquipmentMosaicInscriptionQualityFollowingFive(m_kEquipSealData.item);
            if (qualityFollowingFive == true)
            {
                mContent = TR.Value("Inscription_EquipSeal_Desc");

                SystemNotifyManager.SysNotifyMsgBoxOkCancel(mContent, () =>
                {
                    OnSealClicked2(m_kEquipSealData.item);
                });

                return;
            }

            List<ItemData> inscriptionItemDatas = new List<ItemData>();
            //判断装备镶嵌的铭文是否都是五阶
            bool qualityFive = InscriptionMosaicDataManager.GetInstance().CheckEquipmentMosaicInscriptionQualityFive(ref inscriptionItemDatas, m_kEquipSealData.item);
            if (qualityFive == true)
            {
                string inscriptionName = string.Empty;
                if (inscriptionItemDatas != null && inscriptionItemDatas.Count > 0)
                {
                    for (int i = 0; i < inscriptionItemDatas.Count; i++)
                    {
                        inscriptionName += string.Format("[{0}] ", inscriptionItemDatas[i].GetColorName());
                    }
                }

                mContent = TR.Value("Inscription_EquipSeal_PickHightQuality_Desc", inscriptionName);

                SystemNotifyManager.SysNotifyMsgBoxOkCancel(mContent, () =>
                {
                    OnSealClicked2(m_kEquipSealData.item);
                });

                return;
            }

            //检查装备镶嵌的铭文是否既有五阶又有低于五阶的
            bool isFlag = InscriptionMosaicDataManager.GetInstance().CheckEquipmentMosaicInscriptionQualityFive(ref inscriptionItemDatas, m_kEquipSealData.item, false);
            if (isFlag == true)
            {
                string inscriptionName = string.Empty;
                if (inscriptionItemDatas != null && inscriptionItemDatas.Count > 0)
                {
                    for (int i = 0; i < inscriptionItemDatas.Count; i++)
                    {
                        inscriptionName += string.Format("[{0}] ", inscriptionItemDatas[i].GetColorName());
                    }
                }

                mContent = TR.Value("Inscription_EquipSeal_PickHightQuality_LowQuailty_Desc", inscriptionName);

                SystemNotifyManager.SysNotifyMsgBoxOkCancel(mContent, () =>
                {
                    OnSealClicked2(m_kEquipSealData.item);
                });

                return;
            }

            OnSealClicked2(m_kEquipSealData.item);
        }

        private void OnSealClicked2(ItemData itemData)
        {
            SceneSealEquipReq msg = new SceneSealEquipReq
            {
                uid = itemData.GUID
            };
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<SceneSealEquipRet>(msgRet =>
            {
                if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.code);
                }
                else
                {
                    List<ResultItemData> items = new List<ResultItemData>();
                    ResultItemData item = new ResultItemData();
                    item.itemData = itemData;
                    item.desc = itemData.GetColorName() + "\n" + TR.Value("equip_seal_time_left", itemData.RePackTime);
                    items.Add(item);

                    for (int i = 0; i < msgRet.inscriptionIds.Length; i++)
                    {
                        var inscriptionItemData = ItemDataManager.CreateItemDataFromTable((int)msgRet.inscriptionIds[i]);
                        if (inscriptionItemData == null)
                        {
                            continue;
                        }

                        ResultItemData incriptionitem = new ResultItemData();
                        incriptionitem.itemData = inscriptionItemData;
                        incriptionitem.desc = inscriptionItemData.GetColorName();

                        items.Add(incriptionitem);
                    }

                    CommonGetItemData data = new CommonGetItemData
                    {
                        title = TR.Value("ItemSealOk"),
                        itemDatas = items,
                        itemClickCallback = null,
                    };
                    ClientSystemManager.GetInstance().OpenFrame<AdjustResultFrameEx>(FrameLayer.Middle, data);

                    RefreshAllEquipments();
                }
            });
        }
    }
}