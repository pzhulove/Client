using System.Collections;
using System.Collections.Generic;
using Network;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using EItemBindAttr = ProtoTable.ItemTable.eOwner;
using EItemType = ProtoTable.ItemTable.eType;
using EItemQuality = ProtoTable.ItemTable.eColor;
using DG.Tweening;
using ProtoTable;
using Scripts.UI;
using System;


namespace GameClient
{
    // 仓库界面右侧部分
    class StoragePackageView : MonoBehaviour
    {
        class PackageInfo
        {
            public EPackageType ePackageType;
            public Toggle toggle;
            public GameObject objRedPoint;
        }

        enum EItemsShowMode
        {
            Normal,
            Decompose,
        }

        [SerializeField] private Toggle[] m_arrQualityToggles = new Toggle[3];
        [SerializeField] private ScrollRect m_scrollRect;
        [SerializeField] private ComUIListScriptEx m_comItemListEx;
        [SerializeField] private Text m_gridCount;
        [SerializeField] private GameObject m_objQuickDecomposeRoot;
        [SerializeField] private ComButtonEnbale m_comBtnQuickDecompose;
        [SerializeField] private ComButtonEnbale m_comBtnChapterPotionSet;

        EItemsShowMode m_eShowMode = EItemsShowMode.Normal;
        EPackageType m_currentItemType = EPackageType.Invalid;
        GameObject m_objQuickDecomposeMask = null;
        bool m_bToggleBlockSignal = false;
        private bool m_inited = false;

        PackageInfo[] m_arrPackageInfos =
        {
            new PackageInfo { ePackageType = EPackageType.Equip },
            new PackageInfo { ePackageType = EPackageType.Material },
            new PackageInfo { ePackageType = EPackageType.Consumable },
            new PackageInfo { ePackageType = EPackageType.Title },
            new PackageInfo { ePackageType = EPackageType.Bxy },
            new PackageInfo { ePackageType = EPackageType.Sinan },
        };

        private StorageGroupFrame mStorageGroupFrame;

        //public override string GetPrefabPath()
        //{
        //    return "UIFlatten/Prefabs/Package/StoragePackageFrame";
        //}

        public void Init(StorageGroupFrame storageGroupFrame, EPackageType eType = EPackageType.Equip)
        {
            if (!m_inited)
            {
                mStorageGroupFrame = storageGroupFrame;
                _InitToggleClick();
                m_inited = true;
            }
            EnableView(eType);
        }

        public void EnableView(EPackageType eType = EPackageType.Equip)
        {
            _Initialize(eType);
        }

        private void _InitToggleClick()
        {
            m_arrQualityToggles[0].SafeAddOnValueChangedListener(_OnToggleWhiteClick);
            m_arrQualityToggles[1].SafeAddOnValueChangedListener(_OnToggleBlueClick);
            m_arrQualityToggles[2].SafeAddOnValueChangedListener(_OnTogglePurpleClick);
        }

        //protected override void _OnOpenFrame()
        //{
        //    EPackageType eType = EPackageType.Equip;
        //    if (userData != null)
        //    {
        //        eType = (EPackageType)userData;
        //    }
        //    _Initialize(eType);
        //}

        //protected override void _OnCloseFrame()
        //{
        //    _Clear();
        //}

        void _Initialize(EPackageType a_type)
        {
            _InitQuickDecompose();
            _InitItemList();
            _SetupTabTitle(a_type);
        }

        void OnDestroy()
        {
            _Clear();
        }

        void _Clear()
        {
            _ClearQuickDecompose();

            m_currentItemType = EPackageType.Invalid;
            m_eShowMode = EItemsShowMode.Normal;
            m_bToggleBlockSignal = false;

            for (int i = 0; i < m_arrPackageInfos.Length; ++i)
            {
                m_arrPackageInfos[i].objRedPoint = null;
                m_arrPackageInfos[i].toggle = null;
            }
        }
 

        void _InitItemList()
        {
            m_comItemListEx.Initialize();

            m_comItemListEx.onBindItem = (obj) =>
            {
                if (mStorageGroupFrame != null)
                {
                    return mStorageGroupFrame.CreateComItem(obj);
                }

                return null;
            };

            m_comItemListEx.onItemVisiable = (item) =>
            {
                ComGridBindItem bind = item.GetComponent<ComGridBindItem>();

                List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(m_currentItemType);

                int MaxPackSize = PlayerBaseData.GetInstance().PackTotalSize[(int)m_currentItemType];

                if (item.m_index >= 0 && item.m_index < 100)
                {
                    if (item.m_index < itemGuids.Count)
                    {
                        var comItem = item.gameObjectBindScript as ComItem;
                        ItemData itemData = ItemDataManager.GetInstance().GetItem(itemGuids[item.m_index]);
                        if (itemData != null)
                        {
                            comItem.SetupSlot(ComItem.ESlotType.Opened, string.Empty);
                            comItem.Setup(itemData, _OnItemClicked, true, _OnDoubleClickStoreItem);
                            if (m_eShowMode == EItemsShowMode.Normal)
                            {
                                comItem.SetEnable(_CanStore(itemData));
                            }
                            else if (m_eShowMode == EItemsShowMode.Decompose)
                            {
                                comItem.SetEnable(itemData.CanDecompose && itemData.StrengthenLevel < 10);
                            }
                            else
                            {
                                comItem.SetEnable(true);
                            }

                            comItem.SetShowSelectState(m_eShowMode == EItemsShowMode.Decompose);
                            comItem.SetShowBetterState(m_currentItemType == EPackageType.Equip);
                            comItem.SetShowUnusableState(true);


                            if (bind != null)
                            {
                                bind.param1 = item.gameObject.name;
                                bind.param2 = itemData.GUID;
                            }
                        }
                        else
                        {
                            comItem.SetupSlot(ComItem.ESlotType.Opened, string.Empty);
                            comItem.Setup(null, null);
                            comItem.SetEnable(true);
                            comItem.SetShowBetterState(false);
                            comItem.SetShowSelectState(false);
                            comItem.SetShowUnusableState(false);

                            if (bind != null)
                            {
                                bind.param1 = null;
                                bind.param2 = 0;
                            }
                        }
                    }
                    else if (item.m_index < MaxPackSize)
                    {
                        var comItem = item.gameObjectBindScript as ComItem;
                        comItem.SetupSlot(ComItem.ESlotType.Opened, string.Empty);
                        comItem.Setup(null, null);
                        comItem.SetEnable(true);
                        comItem.SetShowBetterState(false);
                        comItem.SetShowSelectState(false);
                        comItem.SetShowUnusableState(false);

                        if (bind != null)
                        {
                            bind.param1 = null;
                            bind.param2 = 0;
                        }
                    }
                    else
                    {
                        var comItem = item.gameObjectBindScript as ComItem;
                        comItem.SetupSlot(ComItem.ESlotType.Locked, string.Empty, var => {
                            _UpgradePackageSize();
                        });
                        comItem.Setup(null, null);
                        comItem.SetEnable(true);
                        comItem.SetShowBetterState(false);
                        comItem.SetShowSelectState(false);
                        comItem.SetShowUnusableState(false);

                        if (bind != null)
                        {
                            bind.param1 = null;
                            bind.param2 = 0;
                        }
                    }
                }

            };
        }

        private bool _CanStore(ItemData itemData)
        {
            if (itemData == null)
            {
                return false;
            }

            return itemData.CanStore() || StorageDataManager.GetInstance().CurrentStorageType == StorageType.RoleStorage;  // 角色仓库可以存放任何道具
        }

        #region item click
        void _OnItemClicked(GameObject obj, ItemData item)
        {
            if (m_eShowMode == EItemsShowMode.Decompose)
            {
                if (item.CanDecompose)
                {
                    item.IsSelected = !item.IsSelected;
                    if (item.bLocked) // 安全锁锁住的道具无法被选中
                    {
                        item.IsSelected = false;
                    }
                    obj.GetComponent<ComItem>().MarkDirty();
                }
                return;
            }

            List<TipFuncButon> funcs = new List<TipFuncButon>();
            TipFuncButon tempFunc = null;
            if (_CanStore(item))
            {
                tempFunc = new TipFuncButonSpecial();
                tempFunc.text = TR.Value("tip_store");
                tempFunc.callback = _OnStoreItem;
                funcs.Add(tempFunc);
            }
            ItemTipManager.GetInstance().ShowTip(item, funcs, TextAnchor.MiddleLeft);
        }

        private void _OnDoubleClickStoreItem(ItemData item)
        {
            if (m_eShowMode == EItemsShowMode.Decompose)
            {
                return;
            }

            _OnStoreItemImmediate(item, null);
        }

        private void _OnStoreItemImmediate(ItemData item, object data)
        {
            if (item == null)
            {
                return;
            }

            if (item.Type == ItemTable.eType.EQUIP
                        && StorageDataManager.GetInstance().CurrentStorageType == StorageType.AccountStorage)
            {
                //检查装备是否镶嵌了铭文，镶嵌了铭文不能放入账号仓库
                if (!item.CheckEquipmentIsCanPutAccountWarehouse())
                {
                    SystemNotifyManager.SystemNotify(1000125);
                    ItemTipManager.GetInstance().CloseAll();
                    return;
                }
            }

            //角色仓库,未启用的装备方案中的道具不放入
            if (StorageDataManager.GetInstance().CurrentStorageType == StorageType.RoleStorage)
            {
                if (item.IsItemInUnUsedEquipPlan == true)
                {
                    var tipContent = TR.Value("Equip_Plan_Item_CanNot_Store_Format",
                        EquipPlanDataManager.GetInstance().UnSelectedEquipPlanId);
                    SystemNotifyManager.SysNotifyFloatingEffect(tipContent);
                    return;
                }
            }


            StorageDataManager.GetInstance().OnSendStoreItemReq(item, item.Count);
            ItemTipManager.GetInstance().CloseAll();
        }

        void _OnStoreItem(ItemData item, object data)
        {
            if (item != null)
            {
                if (item.Count > 1)
                {
                    var storeItemFrame = ClientSystemManager.GetInstance().OpenFrame<StoreItemFrame>(FrameLayer.Middle) as StoreItemFrame;
                    if (storeItemFrame != null)
                        storeItemFrame.StoreItem(item);

                    ItemTipManager.GetInstance().CloseAll();
                }
                else
                {
                    _OnStoreItemImmediate(item, data);
                }
            }
        }

        void _OnDecomposeClicked(ItemData item, object data)
        {
            if (item != null && item.CanDecompose)
            {
                _DecomposeEquips(() => {
                    ItemTipManager.GetInstance().CloseAll();
                }, item);
            }
        }
        #endregion


        void _SetupTabTitle(EPackageType a_type)
        {
            m_bToggleBlockSignal = true;

            for (int i = 0; i < m_arrPackageInfos.Length; ++i)
            {
                PackageInfo info = m_arrPackageInfos[i];
                info.toggle = Utility.GetComponetInChild<Toggle>(gameObject, string.Format("Tabs/Title{0}", i+1));
                info.toggle.onValueChanged.RemoveAllListeners();
                info.toggle.onValueChanged.AddListener((bool a_bChecked) =>
                {
                    if (m_bToggleBlockSignal == false)
                    {
                        if (a_bChecked)
                        {
                            EPackageType newPackageType = info.ePackageType;
                            if (m_currentItemType != newPackageType)
                            {
                                m_currentItemType = newPackageType;
                                ItemDataManager.GetInstance().ArrangeItemsInPackageFrame(m_currentItemType);
                                _SetupCurrentPage(true);
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PackageTypeChanged, m_currentItemType);
                            }

                            _updateFunctionButtonShowHidden();
                        }
                        else
                        {
                            info.objRedPoint.SetActive(ItemDataManager.GetInstance().IsPackageHasNew(info.ePackageType));
                        }
                    }
                });

                info.objRedPoint = Utility.FindGameObject(gameObject, string.Format("Tabs/Title{0}/RedPoint", i + 1));
                info.objRedPoint.SetActive(ItemDataManager.GetInstance().IsPackageHasNew(info.ePackageType));
            }

            for (int i = 0; i < m_arrPackageInfos.Length; ++i)
            {
                PackageInfo info = m_arrPackageInfos[i];
                if (info.ePackageType == a_type)
                {
                    info.toggle.isOn = false;
                }
                else
                {
                    info.toggle.isOn = true;
                }
            }

            m_bToggleBlockSignal = false;
            PackageInfo currentPackageInfo = _GetPackageInfo(a_type);
            if (currentPackageInfo != null)
            {
                currentPackageInfo.toggle.isOn = true;
            }
        }

        private void _updateFunctionButtonShowHidden()
        {
            m_comBtnChapterPotionSet.gameObject.SetActive(m_currentItemType == EPackageType.Consumable);
            m_comBtnQuickDecompose.gameObject.SetActive(m_currentItemType != EPackageType.Consumable);
        }

        PackageInfo _GetPackageInfo(EPackageType a_type)
        {
            for (int i = 0; i < m_arrPackageInfos.Length; ++i)
            {
                if (m_arrPackageInfos[i].ePackageType == a_type)
                {
                    return m_arrPackageInfos[i];
                }
            }
            return null;
        }

        void _UpdateTabs()
        {
            if(m_arrPackageInfos != null)
            {
                for (int i = 0; i < m_arrPackageInfos.Length; ++i)
                {
                    PackageInfo info = m_arrPackageInfos[i];

                    if (info != null && info.objRedPoint != null)
                    {
                        info.objRedPoint.SetActive(ItemDataManager.GetInstance().IsPackageHasNew(info.ePackageType));
                    }
                }
            }
        }

        void _SetupCurrentPage(bool resetScrollPos = false)
        {
            _UpdateTabs();
            UpdateItemList();

            if (resetScrollPos && m_scrollRect)
            {
                m_scrollRect.verticalNormalizedPosition = 1.0f;
            }

            // grid count
            List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(m_currentItemType);
            m_gridCount.text = string.Format("({0}/{1})", itemGuids.Count, PlayerBaseData.GetInstance().PackTotalSize[(int)m_currentItemType]);

            // quick decompose
            m_comBtnQuickDecompose.SetEnable(m_currentItemType == EPackageType.Equip);
            _CloseQuickDecompose();
        }

        public void UpdateItemList()
        {
            m_comItemListEx.SetElementAmount(100);
        }

        void _UpgradePackageSize()
        {
            int key = PlayerBaseData.GetInstance().PackBaseSize + 10;
            if (key <= 100)
            {
                ProtoTable.SystemValueTable tableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>(key);
                ProtoTable.SystemValueTable tableDataCostID = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>(key + 1);
                if (tableData != null && tableDataCostID != null)
                {
                    CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();
                    costInfo.nMoneyID = tableDataCostID.Value;
                    costInfo.nCount = tableData.Value;

                    SystemNotifyManager.SysNotifyMsgBoxOkCancel(TR.Value("package_unlock_grids", CostItemManager.GetInstance().GetCostMoneiesDesc(costInfo)), () =>
                    {
                        CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
                        {
                            SceneEnlargePackage msg = new SceneEnlargePackage();
                            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

                            WaitNetMessageManager.GetInstance().Wait<SceneEnlargePackageRet>(msgRet =>
                            {
                                if (msgRet == null)
                                {
                                    return;
                                }

                                if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                                {
                                    SystemNotifyManager.SystemNotify((int)msgRet.code);
                                }
                                else
                                {
                                    _SetupCurrentPage();
                                }
                            });
                        });
                    });
                }
            }
            else
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("package_unlock_max"));
            }
        }

        void _DecomposeEquips(Action a_okCallback, params ItemData[] a_arrItems)
        {
            if (a_arrItems == null || a_arrItems.Length == 0)
            {
                return;
            }

            ulong[] arrGuids = new ulong[a_arrItems.Length];
            for (int i = 0; i < a_arrItems.Length; ++i)
            {
                arrGuids[i] = a_arrItems[i].GUID;
            }

            string strContent = string.Empty;
            if (a_arrItems.Length == 1)
            {
                ItemData item = a_arrItems[0];
                if (item.StrengthenLevel >= 5)
                {
                    strContent = TR.Value("equip_single_decompose_ask_01", item.GetColorName("[{0}]", true), item.StrengthenLevel);
                }
                else
                {
                    if (item.Quality > EItemQuality.BLUE)
                    {
                        strContent = TR.Value("equip_single_decompose_ask_03", item.GetQualityDesc(), item.GetColorName("[{0}]", true));
                    }
                    else
                    {
                        strContent = TR.Value("equip_single_decompose_ask_02", item.GetColorName("[{0}]", true));
                    }
                }
            }
            else
            {
                List<ItemData> arrHighValueItems = new List<ItemData>();
                for (int i = 0; i < a_arrItems.Length; ++i)
                {
                    ItemData item = a_arrItems[i];
                    if (item.StrengthenLevel >= 5 || item.Quality > EItemQuality.BLUE)
                    {
                        arrHighValueItems.Add(item);
                    }
                }

                int nNormalCount = a_arrItems.Length - arrHighValueItems.Count;
                if (arrHighValueItems.Count > 0)
                {
                    arrHighValueItems.Sort((var1, var2) => {
                        var value1 = StorageUtility.GetBasePriceByItemData(var1);
                        var value2 = StorageUtility.GetBasePriceByItemData(var2);
                        if (value1 > value2)
                        {
                            return -1;
                        }
                        else if (value1 < value2)
                        {
                            return 1;
                        }
                        else
                        {
                            return var2.StrengthenLevel - var1.StrengthenLevel;
                        }
                    });
                    string strItemNames = string.Empty;
                    for (int i = 0; i < arrHighValueItems.Count; ++i)
                    {
                        if (i <= 5)
                        {
                            strItemNames += arrHighValueItems[i].GetColorName(" [{0}]", true);
                        }
                        else
                        {
                            break;
                        }
                    }
                    strItemNames += " ";

                    if (arrHighValueItems.Count <= 5)
                    {
                        if (nNormalCount > 0)
                        {
                            strContent = TR.Value("equip_multi_decompose_ask_01", strItemNames, arrHighValueItems.Count, nNormalCount);
                        }
                        else
                        {
                            strContent = TR.Value("equip_multi_decompose_ask_02", strItemNames, arrHighValueItems.Count);
                        }
                    }
                    else
                    {
                        if (nNormalCount > 0)
                        {
                            strContent = TR.Value("equip_multi_decompose_ask_03", strItemNames, arrHighValueItems.Count, nNormalCount);
                        }
                        else
                        {
                            strContent = TR.Value("equip_multi_decompose_ask_04", strItemNames, arrHighValueItems.Count);
                        }
                    }
                }
                else
                {
                    strContent = TR.Value("equip_multi_decompose_ask_05", nNormalCount);
                }
            }

            SystemNotifyManager.SysNotifyMsgBoxOkCancel(strContent, () =>
            {
                ItemDataManager.GetInstance().SendDecomposeItem(arrGuids);
                if (a_okCallback != null)
                {
                    a_okCallback.Invoke();
                }
            });
        }

        public void ItemDecomposeFinished(UIEvent a_event)
        {
            _ClearSelectState();
            _SetupCurrentPage();
        }

        public void ItemSellSuccess(UIEvent a_event)
        {
            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("package_sell_item_success"));
            _SetupCurrentPage();
        }

        public void UpdateItemList(UIEvent a_event)
        {
            _SetupCurrentPage();

            if(a_event.EventID == EUIEventID.ItemUseSuccess)
            {
                ItemData item = (ItemData)a_event.Param1;

                if (item.PackageType == EPackageType.Consumable)
                {
                    var TotalData = TableManager.GetInstance().GetTable<PetTable>();
                    var enumer = TotalData.GetEnumerator();

                    bool bIsPet = false;
                    while(enumer.MoveNext())
                    {
                        var data = enumer.Current.Value as PetTable;

                        if(data == null)
                        {
                            continue;
                        }

                        if(data.PetEggID == item.TableID)
                        {
                            bIsPet = true;
                            break;
                        }
                    }

                    if(bIsPet)
                    {
                        if (ClientSystemManager.GetInstance().IsFrameOpen<OpenPetEggFrame>())
                        {
                            ClientSystemManager.GetInstance().CloseFrame<OpenPetEggFrame>();

                        }
                        ClientSystemManager.GetInstance().OpenFrame<OpenPetEggFrame>(FrameLayer.Middle, item);
                    }         
                }
            }
        }


        //[UIEventHandle("Bottom/Arrange")]
        public void OnArrangePackage()
        {
            SceneTrimItem msg = new SceneTrimItem();
            msg.pack = (byte)m_currentItemType;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<SceneTrimItemRet>(msgRet =>
            {
                if (msgRet == null)
                {
                    return;
                }

                if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                {
                    ProtoTable.CommonTipsDesc tableData = TableManager.GetInstance().GetTableItem<ProtoTable.CommonTipsDesc>((int)msgRet.code);
                    if (tableData != null)
                    {
                        SystemNotifyManager.SystemNotify((int)msgRet.code);
                    }
                    else
                    {
                        SystemNotifyManager.SysNotifyMsgBoxOK(Utility.ProtocolErrorString(msgRet.code));
                    }
                }
                else
                {
                    ItemDataManager.GetInstance().ArrangeItemsInPackageFrame(m_currentItemType);
                    _SetupCurrentPage();
                }
            });
        }

        //[UIEventHandle("Bottom/GridCount/LevelUp")]
        public void OnLevelupGridCountClicked()
        {
            _UpgradePackageSize();
        }

        #region quick decompose
        void _InitQuickDecompose()
        {
			if (m_objQuickDecomposeMask == null && LeanTween.instance.frameBlackMask != null)
            {
                m_objQuickDecomposeMask = GameObject.Instantiate(LeanTween.instance.frameBlackMask);
                m_objQuickDecomposeMask.transform.SetParent(ClientSystemManager.GetInstance().GetLayer(FrameLayer.Middle).transform, false);
                
                m_objQuickDecomposeMask.transform.SetParent(m_objQuickDecomposeRoot.transform, true);
                m_objQuickDecomposeMask.transform.SetAsFirstSibling();
            }

            for (int i = 0; i < m_arrQualityToggles.Length; ++i)
            {
                m_arrQualityToggles[i].isOn = false;
            }
            _ClearSelectState();
            m_objQuickDecomposeRoot.SetActive(false);
        }

        void _ClearQuickDecompose()
        {
            m_objQuickDecomposeMask = null;
            _CloseQuickDecompose();
        }

        void _OpenQuickDecompose()
        {
            if (m_eShowMode == EItemsShowMode.Normal)
            {
                m_eShowMode = EItemsShowMode.Decompose;
                UpdateItemList();
                _ClearSelectState();
                m_objQuickDecomposeRoot.SetActive(true);
            }
        }

        void _CloseQuickDecompose()
        {
            if (m_eShowMode == EItemsShowMode.Decompose)
            {
                m_eShowMode = EItemsShowMode.Normal;
                UpdateItemList();

                _ClearSelectState();
                m_objQuickDecomposeRoot.SetActive(false);
                for (int i = 0; i < m_arrQualityToggles.Length; ++i)
                {
                    m_arrQualityToggles[i].isOn = false;
                }
            }
        }

        List<ItemData> _GetItemsByQuality(EItemQuality a_quality)
        {
            List<ItemData> arrItems = new List<ItemData>();
            List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(m_currentItemType);
            for (int i = 0; i < itemGuids.Count; ++i)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(itemGuids[i]);
                if (itemData != null && itemData.Quality == a_quality && itemData.CanDecompose)
                {
                    arrItems.Add(itemData);
                }
            }
            return arrItems;
        }

        void _ClearSelectState()
        {
            List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(m_currentItemType);
            for (int i = 0; i < itemGuids.Count; ++i)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(itemGuids[i]);
                if (itemData != null)
                {
                    itemData.IsSelected = false;
                }
            }
        }

        public void OnOpenChapterPotionSetClicked()
        {
            ClientSystemManager.instance.OpenFrame<ChapterBattlePotionSetFrame>();
        }

        //[UIEventHandle("Bottom/QuickDecompose")]
        public void OnOpenQuickDecomposeClicked()
        {
            _OpenQuickDecompose();
        }

        //[UIEventHandle("DecomposeGroup/Cancel")]
        public void OnReturnClicked()
        {
            _CloseQuickDecompose();
        }

        //[UIEventHandle("DecomposeGroup/Confirm")]
        public void OnQuickDecomposeClicked()
        {
            List<ItemData> selectItems = new List<ItemData>();
            List<ulong> guids = ItemDataManager.GetInstance().GetItemsByPackageType(m_currentItemType);
            for (int i = 0; i < guids.Count; ++i)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(guids[i]);
                if (itemData != null && itemData.IsSelected)
                {
                    selectItems.Add(itemData);
                }
            }

            if (selectItems.Count > 0)
            {
                _DecomposeEquips(() => {
                    _CloseQuickDecompose();
                }, selectItems.ToArray());
            }
            else
            {
                SystemNotifyManager.SysNotifyMsgBoxOK(TR.Value("package_quick_decompose_no_select"));
            }
        }

        private void _OnToggleWhiteClick(bool isChecked)
        {
            _OnSelectQualityChanged(0, isChecked);
        }

        private void _OnToggleBlueClick(bool isChecked)
        {
            _OnSelectQualityChanged(1, isChecked);
        }

        private void _OnTogglePurpleClick(bool isChecked)
        {
            _OnSelectQualityChanged(2, isChecked);
        }

        //[UIEventHandle("DecomposeGroup/SelectGroup/Toggle{0}", typeof(Toggle), typeof(UnityEngine.Events.UnityAction<int, bool>), 1, 3)]
        private void _OnSelectQualityChanged(int index, bool isChecked)
        {
            if (m_eShowMode == EItemsShowMode.Decompose)
            {
                List<ItemData> arrSelectItems;
                if (index == 0)
                {
                    arrSelectItems = _GetItemsByQuality(EItemQuality.WHITE);
                }
                else if (index == 1)
                {
                    arrSelectItems = _GetItemsByQuality(EItemQuality.BLUE);
                }
                else if (index == 2)
                {
                    arrSelectItems = _GetItemsByQuality(EItemQuality.PURPLE);
                }
                else
                {
                    arrSelectItems = new List<ItemData>();
                }

                //             if (arrSelectItems.Count > 0)
                //             {
                //                 RectTransform item = arrSelectItems[0].GetComponent<RectTransform>();
                //                 RectTransform contentRoot = arrSelectItems[0].transform.parent.parent.GetComponent<RectTransform>();
                //                 RectTransform viewPort = arrSelectItems[0].transform.parent.parent.parent.GetComponent<RectTransform>();
                // 
                //                 float fHeight = Mathf.Abs(item.anchoredPosition.y) - item.rect.height * 0.5f - 20;
                //                 float fMaxHeight = contentRoot.rect.height - viewPort.rect.height;
                //                 m_scrollRect.verticalNormalizedPosition = 1 - fHeight / fMaxHeight;
                //            }

                List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(m_currentItemType);
                for (int i = 0; i < arrSelectItems.Count; ++i)
                {
                    arrSelectItems[i].IsSelected = isChecked && arrSelectItems[i].StrengthenLevel < 10 
                        && !arrSelectItems[i].bLocked; // 被安全锁锁住的道具无法进行一键出售或者分解 add by qxy 2019-11-18
                }

                if (arrSelectItems.Count > 0)
                {
                    ulong uGUID = arrSelectItems[0].GUID;
                    for (int i = 0; i < itemGuids.Count; ++i)
                    {
                        if (uGUID == itemGuids[i])
                        {
                            //m_comItemList.SelectElement(i);
                            m_comItemListEx.EnsureElementVisable(i);
                        }
                    }
                }

                m_comItemListEx.SetElementAmount(100);
            }
        }

		public void OpenQuickDecompose()
		{
			_OpenQuickDecompose();
		}

        #endregion
    }
}
