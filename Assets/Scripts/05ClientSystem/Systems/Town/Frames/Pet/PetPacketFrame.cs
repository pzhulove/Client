using UnityEngine.UI;
using Scripts.UI;
using UnityEngine;
using ProtoTable;
using System.Collections.Generic;
using Protocol;
using Network;
using System;

namespace GameClient
{
    enum PetPackageTab
    {
        PetItemTab = 0,
        PetEggTab,
        PetFoodTab, //口粮现在取消掉 
    }

    enum PetItemListType
    {
        PetItem = 0,
        PackageItem,

        PetItemListTypeNum
    }

    public enum PetItemSortType
    {
        QualitySort = 0,
        PetTypeSort,
    }

    class PetPacketFrame : ClientFrame
    {
        string EquipEffectPath = "Effects/Scene_effects/EffectUI/EffUI_cwkl_zhuangbei";

        const int MaxPackageTypeNum = 2; //==PetItemListTypeNum 记得检查
        const int MaxOnUsePetNum = 3;
        const int SortTypeNum = 2;

        int MaxPetItemNum = 0;
        int MaxShowPetItemNum = 20;
        int MaxSatietyNum = 0;

        PetPackageTab packageTab = PetPackageTab.PetItemTab;
        PetItemSortType CurSelSortType = PetItemSortType.QualitySort;

        List<PetInfo> PetInfoList = new List<PetInfo>();
        PetInfo CurSelPetInfo = new PetInfo();
        int CurSelPetIndex = 0;
        private List<ComControlData> sortTypeDataList;

        [UIObject("TipsRoot")]
        GameObject tipsParent;

        void _OpenDetailTipsFrame()
        {
            if (mMTipState != null)
            {
                mMTipState.Key = "showDetail";
            }
            if (ClientSystemManager.GetInstance().IsFrameOpen<PetNormalTips>())
            {
                ClientSystemManager.GetInstance().CloseFrame<PetNormalTips>();
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<PetDetailFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<PetDetailFrame>();
            }

            ClientSystemManager.GetInstance().OpenFrame<PetDetailFrame>(tipsParent);
        }



        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Package/PetPacket";
        }

        protected override void _OnOpenFrame()
        {
            InitInterface();
            BindUIEvent();

            _AddButton("DetailRoot/BtnPetInfo1", _OpenDetailTipsFrame);
            _AddButton("DetailRoot/BtnPetInfo0", UpdatePetNormalTip);
        }

        protected override void _OnCloseFrame()
        {
            UnBindUIEvent();
            ClearData();

            if (PetDataManager.GetInstance().GetIsActiveFeed())
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PlayActiveFeedPetAction);
                PetDataManager.GetInstance().SetActiveFeed(false);
            }
            if (ClientSystemManager.GetInstance().IsFrameOpen<PetNormalTips>())
            {
                ClientSystemManager.GetInstance().CloseFrame<PetNormalTips>();
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<PetDetailFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<PetDetailFrame>();
            }
        }

        void ClearData()
        {
            CurSelSortType = PetItemSortType.QualitySort;
            MaxPetItemNum = 0;
            MaxSatietyNum = 0;
            packageTab = PetPackageTab.PetItemTab;
            mAvatarRenderer.ClearAvatar();
            PetInfoList.Clear();
            CurSelPetIndex = 0;
        }

        protected void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PetInfoInited, _updateGroupUpRedPoint);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PetSlotChanged, OnPetSlotChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PetItemsInfoUpdate, OnUpdatePetList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.EatPetSuccess, OnEatPetSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemCountChanged, OnItemCountChanged);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemTakeSuccess, _OnUpdateItemList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemStoreSuccess, _OnUpdateItemList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemUseSuccess, _OnUpdateItemList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemNotifyGet, _OnUpdateItemList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemNotifyRemoved, _OnUpdateItemList);

        }

        protected void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PetInfoInited, _updateGroupUpRedPoint);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PetSlotChanged, OnPetSlotChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PetItemsInfoUpdate, OnUpdatePetList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.EatPetSuccess, OnEatPetSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemCountChanged, OnItemCountChanged);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemTakeSuccess, _OnUpdateItemList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemStoreSuccess, _OnUpdateItemList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemUseSuccess, _OnUpdateItemList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemNotifyGet, _OnUpdateItemList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemNotifyRemoved, _OnUpdateItemList);


        }

        void OnPetSlotChanged(UIEvent iEvent)
        {
            int iSlotIndex = (int)iEvent.Param1;
            bool bIsWear = (bool)iEvent.Param2;

            if (IsCurSelPetRemove())
            {
                PetDataManager.GetInstance().SetPetData(CurSelPetInfo, new PetInfo());
                _setNonePetEquiped(false);
            }

            UpdatePetListData();
            RefreshPetItemListCount();
            UpdateOnUsePet();
            SelectSuitableShowPet(false, iSlotIndex, bIsWear);

            if (bIsWear)
            {
                PlayEquipEffect(iSlotIndex);
            }

            _updatePetEggRedPoint(null);
            _updateGroupUpRedPoint(null);
            _updatePetRedPoint(null);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRefreshPackageProperty);
        }

        void OnUpdatePetList(UIEvent iEvent)
        {
            UpdatePetListData();
            RefreshPetItemListCount();
            UpdateCurSelOnUsePet();

            UpdateOnUsePet();
            UpdateSelPetInfo();
        }

        void OnEatPetSuccess(UIEvent iEvent)
        {
            UpdateCurSelOnUsePet();
            UpdateOnUsePet();
            SelectSuitableShowPet();
        }

        void OnItemCountChanged(UIEvent iEvent)
        {
            if (packageTab == PetPackageTab.PetItemTab)
            {
                return;
            }

            int itemID = (int)iEvent.Param1;

            ItemTable itemData = TableManager.GetInstance().GetTableItem<ItemTable>(itemID);
            if (itemData == null || itemData.Type != ItemTable.eType.PET)
            {
                return;
            }

            RefreshNormalItemListCount();
        }

        [UIEventHandle("PacketTab/Func{0}", typeof(Toggle), typeof(UnityEngine.Events.UnityAction<int, bool>), 1, MaxPackageTypeNum)]
        void OnSwitchPackageType(int iIndex, bool value)
        {
            if (!value || iIndex < 0)
            {
                return;
            }

            packageTab = (PetPackageTab)iIndex;

            UpdatePackageType();

            if (packageTab == PetPackageTab.PetItemTab)
            {
                RefreshPetItemListCount();
                mSortRoot.CustomActive(true);

                PetDataManager.GetInstance().IsUseClickPetTab = true;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PetTabClick);
                _updatePetRedPoint(null);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPointChanged, ERedPoint.PackageMain);
            }
            else if (packageTab == PetPackageTab.PetFoodTab)
            {
                InitNormalItemScrollListBind();
                RefreshNormalItemListCount();
            }
            else if (packageTab == PetPackageTab.PetEggTab)
            {
                InitNormalItemScrollListBind();
                RefreshNormalItemListCount();

                _updatePetEggRedPoint(null);
            }
        }

        private void _setNonePetEquiped(bool isEquiped)
        {
            mActorShowRoot.CustomActive(isEquiped);
            mNoneEquipRoot.CustomActive(!isEquiped);
            mPropertyBtn.CustomActive(isEquiped);
            mDetailRoot.CustomActive(isEquiped);
            if (!isEquiped)
            {
                if (ClientSystemManager.GetInstance().IsFrameOpen<PetNormalTips>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<PetNormalTips>();
                }

                if (ClientSystemManager.GetInstance().IsFrameOpen<PetDetailFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<PetDetailFrame>();
                }
            }
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="iIndex"></param>
        /// <param name="value"></param>
        [UIEventHandle("OnUsePet/UsePet{0}", typeof(Toggle), typeof(UnityEngine.Events.UnityAction<int, bool>), 1, MaxOnUsePetNum)]
        void OnChooseUsePet(int iIndex, bool value)
        {
            if (!value || iIndex < 0)
            {
                return;
            }

            CurSelPetIndex = iIndex;
            CurSelPetInfo = GetSelPetInfo(iIndex);

            if (CurSelPetInfo != null)
            {
                PetDataManager.GetInstance().SelectPetId = CurSelPetInfo.id;
                PetDataManager.GetInstance().SelectPetInfo = CurSelPetInfo;
                _updateGroupUpRedPoint(null);
            }

            if (CurSelPetInfo.dataId <= 0)
            {
                _setNonePetEquiped(false);
                mSelectTips.SafeSetText(string.Format(TR.Value("pet_select_tips"), PetDataManager.GetInstance().GetPetTypeDesc((PetTable.ePetType)(iIndex + 1))));
                InitPetItemScrollListBind(true);
            }
            else
            {
                _setNonePetEquiped(true);
                UpdateSelPetInfo();
                UpdateActor((int)CurSelPetInfo.dataId);
                InitPetItemScrollListBind();
            }

            RefreshPetItemListCount();
            UpdatePetNormalTip();
        }


        void SetHavePetShow()
        {
            mPropertyBtn.CustomActive(CurSelPetInfo.dataId > 0);
            mDetailRoot.CustomActive(CurSelPetInfo.dataId > 0);
        }

        void UpdatePetNormalTip()
        {
            if (mMTipState != null)
            {
                mMTipState.Key = "showNormal";
            }
            SetHavePetShow();
            if (ClientSystemManager.GetInstance().IsFrameOpen<PetNormalTips>())
            {
                ClientSystemManager.GetInstance().CloseFrame<PetNormalTips>();
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<PetDetailFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<PetDetailFrame>();
            }

            if (CurSelPetInfo != null && CurSelPetInfo.dataId > 0)
            {
                ClientSystemManager.GetInstance().OpenFrame<PetNormalTips>(tipsParent, CurSelPetInfo);
            }
        }

        void OnShowNormalItemTip(int iIndex)
        {
            List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Pet);

            if (iIndex >= itemGuids.Count)
            {
                return;
            }

            ItemData item = ItemDataManager.GetInstance().GetItem(itemGuids[iIndex]);

            ItemTipManager.GetInstance().ShowTip(item);
        }

        // [UIEventHandle("PacketNavigation/SortTypeSelect/SortTypeList/SortType{0}", typeof(Toggle), typeof(UnityEngine.Events.UnityAction<int, bool>), 1, SortTypeNum)]
        void OnChooseSortType(int iIndex, bool value)
        {
            if (!value || iIndex < 0)
            {
                return;
            }

            CurSelSortType = (PetItemSortType)iIndex;

            // mShowSortType.text = SortTypeTexts[iIndex].text;
            // mSortTypeListRoot.CustomActive(false);

            UpdatePetListData();
            RefreshPetItemListCount();
        }

        private void OnTypeDropDownItemClick(ComControlData data)
        {
            if (null == data || data.Index < 0)
            {
                return;
            }
            
            CurSelSortType = (PetItemSortType)data.Index;

            // mShowSortType.text = SortTypeTexts[data.Index].text;
            // mSortTypeListRoot.CustomActive(false);

            UpdatePetListData();
            RefreshPetItemListCount();
        }



        public void SetPetToggle()
        {
            if(mBind == null)
            {
                return;
            }

            var tog = mBind.GetCom<Toggle>("PetToggle");
            if(tog == null)
            {
                return;              
            }

            tog.isOn = true;
        }

        void InitInterface()
        {
            InitData();
            
            InitPetItemScrollListBind();
            RefreshPetItemListCount();
            mSelectTips.SafeSetText(TR.Value("pet_default_select_tips"));
            UpdateOnUsePet();

            int selectedDefaultIndex = _getDefaultShowIndex();
            if (selectedDefaultIndex != -1)
            {
                SelectSuitableShowPet(false, selectedDefaultIndex, true);
            }
            else
            {
                SelectSuitableShowPet(true);
            }

            _updateGroupUpRedPoint(null);
            _updatePetEggRedPoint(null);
            _updatePetRedPoint(null);
            if (mComDropDownControl != null)
            {
                mComDropDownControl.InitComDropDownControl(sortTypeDataList[0],sortTypeDataList,OnTypeDropDownItemClick);
            }
        }

        private int _getDefaultShowIndex()
        {
            if (PetDataManager.GetInstance().IsUserClickFeedCount)
            {
                return -1;
            }

            return PetDataManager.GetInstance().GetPetsContainGoldFeedCountTypeIndex();
        }

        private void _updatePetRedPoint(UIEvent ui)
        {
            mPetRedPoint.CustomActive(PetDataManager.GetInstance().IsNeedShowPetRedPoint());
        }

        private void _updatePetEggRedPoint(UIEvent ui)
        {
            mPetEggRedPoint.CustomActive(PetDataManager.GetInstance().IsNeedShowPetEggRedPoint());
        }

        private void _updateGroupUpRedPoint(UIEvent ui)
        {
            var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
            if (current != null)
            {
                return;
            }
            mGroupUpRedPoint.CustomActive(PetDataManager.GetInstance().SelectPetsNeedShowRedPoint());
        }

        void InitData()
        {
            SystemValueTable valuedata = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_PET_MAX_NUM);
            if (valuedata != null)
            {
                MaxPetItemNum = valuedata.Value;
            }

            SystemValueTable SatietyData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_PET_HUNGER_MAX_NUM);
            MaxSatietyNum = SatietyData.Value;

            InitDropDownData();

            UpdatePetListData();
        }

        void InitDropDownData()
        {
            sortTypeDataList = new List<ComControlData>();
            var data = new ComControlData(
                0,
                0,
                "品质排序",
                true
            );
            sortTypeDataList.Add(data);
            
            
            var data2 = new ComControlData(
                1,
                1,
                "类型排序",
                false
            );
            sortTypeDataList.Add(data2);
        }

        void InitPetItemScrollListBind(bool bCheckCover = false)
        {
            if (packageTab != PetPackageTab.PetItemTab)
            {
                return;
            }

            mItemUIScrollList[0].Initialize();

            mItemUIScrollList[0].onItemVisiable = (item) =>
            {
                if (item.m_index >= 0)
                {
                    UpdatePetItemScrollListBind(item, bCheckCover);
                }
            };

            mItemUIScrollList[0].OnItemRecycle = (item) =>
            {
                ComCommonBind combind = item.GetComponent<ComCommonBind>();
                if (combind == null)
                {
                    return;
                }

                Button iconBtn = combind.GetCom<Button>("btPetItem");
                iconBtn.onClick.RemoveAllListeners();
            };
        }

        void InitPetEggScrollList()
        {
            mItemUIScrollList[1].Initialize();

            mItemUIScrollList[1].onBindItem = (obj) =>
            {
                return CreateComItem(obj);
            };

            mItemUIScrollList[1].onItemVisiable = (item) =>
            {
                List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Pet);

                int iMaxNum = itemGuids.Count > MaxShowPetItemNum ? MaxPetItemNum : MaxShowPetItemNum;

                if (item.m_index >= 0 && item.m_index < iMaxNum)
                {
                    var comItem = item.gameObjectBindScript as ComItem;

                    if (item.m_index < itemGuids.Count)
                    {
                        ItemData itemData = ItemDataManager.GetInstance().GetItem(itemGuids[item.m_index]);
                        if (itemData != null)
                        {
                            comItem.SetupSlot(ComItem.ESlotType.Opened, string.Empty);

                            int idx = item.m_index;
                            comItem.Setup(itemData, (GameObject obj, ItemData tipitem) => { OnShowNormalItemTip(idx); });

                            comItem.SetEnable(true);
                            comItem.SetShowUnusableState(true);
                        }
                        else
                        {
                            comItem.SetupSlot(ComItem.ESlotType.Opened, string.Empty);
                            comItem.Setup(null, null);
                            comItem.SetEnable(true);
                            comItem.SetShowBetterState(false);
                            comItem.SetShowSelectState(false);
                            comItem.SetShowUnusableState(false);
                        }
                    }
                    else
                    {
                        comItem.SetupSlot(ComItem.ESlotType.Opened, string.Empty);
                        comItem.Setup(null, null);
                        comItem.SetEnable(true);
                        comItem.SetShowBetterState(false);
                        comItem.SetShowSelectState(false);
                    }
                }
            };
        }

        void OnNormalItemClick(int index)
        {
            if (packageTab == PetPackageTab.PetFoodTab)
            {
                OnShowNormalItemTip(index);
            }
            else if (packageTab == PetPackageTab.PetEggTab)
            {
                _OnPackageItemClicked(index);
            }
        }

        void _OnUpdateItemList(UIEvent a_event)
        {
            if (a_event.EventID == EUIEventID.ItemUseSuccess)
            {
                ItemData item = (ItemData)a_event.Param1;

                if (item.PackageType == EPackageType.Consumable)
                {
                    var TotalData = TableManager.GetInstance().GetTable<PetTable>();
                    var enumer = TotalData.GetEnumerator();

                    bool bIsPet = PetDataManager.GetInstance().IsPetEggItem(item.TableID);

                    if (bIsPet)
                    {
                        if (ClientSystemManager.GetInstance().IsFrameOpen<OpenPetEggFrame>())
                        {
                            ClientSystemManager.GetInstance().CloseFrame<OpenPetEggFrame>();

                        }
                        ClientSystemManager.GetInstance().OpenFrame<OpenPetEggFrame>(FrameLayer.Middle, item);
                    }
                }
            }
            RefreshNormalItemListCount();

            _updatePetEggRedPoint(null);
            _updatePetRedPoint(null);
        }

        void _OnShareClicked(ItemData item, object data)
        {
            ChatManager.GetInstance().ShareEquipment(item);
        }
        void _OnUseItem(ItemData item, object data)
        {
            if (item != null)
            {
                if (item.PackID > 0)
                {
                    GiftPackTable giftPackTable = TableManager.GetInstance().GetTableItem<GiftPackTable>(item.PackID);
                    if (giftPackTable != null)
                    {
                        if (giftPackTable.FilterType == GiftPackTable.eFilterType.Custom || giftPackTable.FilterType == GiftPackTable.eFilterType.CustomWithJob)
                        {
                            if (giftPackTable.FilterCount > 0)
                            {
                                ClientSystemManager.GetInstance().OpenFrame<SelectItemFrame>(FrameLayer.Middle, item);
                            }
                            else
                            {
                                Logger.LogErrorFormat("礼包{0}的FilterCount小于等于0", item.PackID);
                            }
                        }
                        else
                        {
                            ItemDataManager.GetInstance().UseItem(item);
                            if (item.Count <= 1 || item.CD > 0)
                            {
                                ItemTipManager.GetInstance().CloseAll();
                            }
                        }
                    }
                    else
                    {
                        Logger.LogErrorFormat("道具{0}的礼包ID{1}不存在", item.TableID, item.PackID);
                    }
                }
                else
                {
                    ItemDataManager.GetInstance().UseItem(item);

                    if (item.PackageType == EPackageType.Equip || item.PackageType == EPackageType.Fashion)
                    {
                        AudioManager.instance.PlaySound(102);
                    }

                    if (item.Count <= 1 || item.CD > 0)
                    {
                        ItemTipManager.GetInstance().CloseAll();
                    }
                }
            }
        }

        void _OnPackageItemClicked(int idx)
        {
            var list = GetPetEggList(false);

            if (idx < 0 || idx >= list.Count)
            {
                return;
            }

            ItemData item = ItemDataManager.GetInstance().GetItem(list[idx]);
            List<TipFuncButon> funcs = new List<TipFuncButon>();
            TipFuncButon tempFunc = null;
            if (item != null)
            {
                // 使用，穿戴
                if (item.UseType == ProtoTable.ItemTable.eCanUse.UseOne ||
                    item.UseType == ProtoTable.ItemTable.eCanUse.UseTotal)
                {
                    if (item.IsCooling() == false)
                    {
                        tempFunc = new TipFuncButonSpecial();
                        tempFunc.text = TR.Value("tip_use");
                        tempFunc.callback = _OnUseItem;
                        funcs.Add(tempFunc);
                    }
                }
            }

            if (item != null && item.Quality > ItemTable.eColor.PURPLE)
            {
                // 分享
                tempFunc = new TipFuncButon();
                tempFunc.text = TR.Value("tip_share");
                tempFunc.name = "Share";
                tempFunc.callback = _OnShareClicked;
                funcs.Add(tempFunc);
            }
         
            ItemTipManager.GetInstance().ShowTip(item, funcs, TextAnchor.MiddleLeft);
        }

        void InitNormalItemScrollListBind()
        {
            if (packageTab == PetPackageTab.PetItemTab)
            {
                return;
            }

            var ScrollList = mItemUIScrollList[(int)PetItemListType.PackageItem];

            if (ScrollList == null)
            {
                return;
            }

            if (ScrollList.IsInitialised())
            {
                return;
            }

            ScrollList.Initialize();

            ScrollList.onBindItem = (obj) =>
            {
                return CreateComItem(obj);
            };

            ScrollList.onItemVisiable = (item) =>
            {

                List<ulong> itemGuids;
                EPackageType type = EPackageType.Invalid;
                if (packageTab == PetPackageTab.PetFoodTab)
                {
                    type = EPackageType.Pet;
                }
                else if (packageTab == PetPackageTab.PetEggTab)
                {
                    type = EPackageType.Consumable;
                }
                else
                {
                    Logger.LogError("UnSupport PetPackageTab" + packageTab);
                    return;
                }

                itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(type);
                int iMaxNum = itemGuids.Count > MaxShowPetItemNum ? MaxPetItemNum : MaxShowPetItemNum;

                //这里临时每次都获取一个列表
                if (packageTab == PetPackageTab.PetEggTab)
                {
                    itemGuids = GetPetEggList(false);
                    iMaxNum =  mPetEggMaxCount;
                }

                if (item.m_index >= 0 && item.m_index < iMaxNum)
                {
                    var comItem = item.gameObjectBindScript as ComItem;

                    if (item.m_index < itemGuids.Count)
                    {
                        ItemData itemData = ItemDataManager.GetInstance().GetItem(itemGuids[item.m_index]);
                        if (itemData != null)
                        {
                            comItem.SetupSlot(ComItem.ESlotType.Opened, string.Empty);

                            int idx = item.m_index;
                            comItem.Setup(itemData, (GameObject obj, ItemData tipitem) => { OnNormalItemClick(idx); });

                            comItem.SetEnable(true);
                            comItem.SetShowUnusableState(true);
                        }
                        else
                        {
                            comItem.SetupSlot(ComItem.ESlotType.Opened, string.Empty);
                            comItem.Setup(null, null);
                            comItem.SetEnable(true);
                            comItem.SetShowBetterState(false);
                            comItem.SetShowSelectState(false);
                            comItem.SetShowUnusableState(false);
                        }
                    }
                    else
                    {
                        comItem.SetupSlot(ComItem.ESlotType.Opened, string.Empty);
                        comItem.Setup(null, null);
                        comItem.SetEnable(true);
                        comItem.SetShowBetterState(false);
                        comItem.SetShowSelectState(false);
                    }
                }
            };
        }

        void UpdatePetItemScrollListBind(ComUIListElementScript item, bool bCheckCover)
        {
            ComCommonBind combind = item.GetComponent<ComCommonBind>();
            if (combind == null)
            {
                return;
            }

            if (item.m_index < 0 || item.m_index >= PetInfoList.Count)
            {
                return;
            }

            Image IconRoot = combind.GetCom<Image>("IconRoot");

            if (PetInfoList[item.m_index].dataId > 0)
            {
                PetTable petData = TableManager.GetInstance().GetTableItem<PetTable>((int)PetInfoList[item.m_index].dataId);
                if (petData == null)
                {
                    IconRoot.gameObject.CustomActive(false);
                    return;
                }

                bool bCover = false;
                if(bCheckCover)
                {
                    if(petData.PetType != (PetTable.ePetType)(CurSelPetIndex + 1))
                    {
                        bCover = true;
                    }
                }

                PetDataManager.GetInstance().SetPetItemData(item.gameObject, PetInfoList[item.m_index], PlayerBaseData.GetInstance().JobTableID, PetTipsType.PetItemTip, bCover);
            }
            else
            {
                IconRoot.gameObject.CustomActive(false);
            }
        }

        void RefreshPetItemListCount()
        {
            mItemUIScrollList[0].SetElementAmount(PetInfoList.Count);

            mGridNum.text = TR.Value("grid_info", PetDataManager.GetInstance().GetPetList().Count + PetDataManager.GetInstance().GetOnUsePetList().Count, MaxPetItemNum);
        }

        private List<ulong> mPetEggList;

        private int mPetEggMaxCount = 0;

        private const int kMaxRowCount = 4;

        int GetPetEggMaxCount(int realCount)
        {
            int ct = realCount;
            
            int c1 = ct / kMaxRowCount;
            int c2 = ct % kMaxRowCount;
            if(c2 != 0)
            {
                c1++;
            }

            ct = c1 * kMaxRowCount;

            return Mathf.Max(ct, kMaxRowCount * 5);
        }

        List<ulong> GetPetEggList(bool bForceRefresh = false)
        {
            if (mPetEggList == null || bForceRefresh)
            {
                List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Consumable);
                List<ulong> temp = new List<ulong>();
                for (int i = 0; i < itemGuids.Count; ++i)
                {
                    var cur = itemGuids[i];
                    var curitem = ItemDataManager.GetInstance().GetItem(cur);
                    if (curitem == null)
                    {
                        continue;
                    }
                    if (PetDataManager.GetInstance().IsPetEggItem(curitem.TableID))
                    {
                        temp.Add(cur);
                    }
                }

                mPetEggList = temp;

                mPetEggMaxCount = GetPetEggMaxCount(mPetEggList.Count);
            }

            return mPetEggList;
        }

        void RefreshNormalItemListCount()
        {
            if (packageTab == PetPackageTab.PetFoodTab)
            {
                List<ulong> itemGuids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Pet);

                int AddedNum = GetAddedGridNum(itemGuids.Count);

                mItemUIScrollList[1].SetElementAmount(itemGuids.Count + AddedNum);

                mGridNum.text = TR.Value("grid_info", itemGuids.Count, MaxPetItemNum);

                mSortRoot.CustomActive(false);
            }
            else if (packageTab == PetPackageTab.PetEggTab)
            {
                GetPetEggList(true);
                mItemUIScrollList[1].SetElementAmount(mPetEggMaxCount);
                mGridNum.text = TR.Value("grid_info", mPetEggList.Count, mPetEggMaxCount);
                mSortRoot.CustomActive(false);
            }
            else
            {
                Logger.LogWarning("UnSupport PetPackageTab" + packageTab);
                return;
            }
        }



        void UpdateOnUsePet()
        {
            List<PetInfo> OnUsePetinfoList = PetDataManager.GetInstance().GetOnUsePetList();
            for (int i = 0; i < UsePetBind.Length; i++)
            {
                bool bFind = false;
                
                for (int j = 0; j < OnUsePetinfoList.Count; j++)
                {
                    PetTable petData = TableManager.GetInstance().GetTableItem<PetTable>((int)OnUsePetinfoList[j].dataId);
                    if (petData == null)
                    {
                        UsePetBind[i].gameObject.CustomActive(false);
                        continue;
                    }

                    if ((int)petData.PetType == (i + 1))
                    {
                        PetDataManager.GetInstance().SetPetItemData(UsePetBind[i].gameObject, OnUsePetinfoList[j], PlayerBaseData.GetInstance().JobTableID, PetTipsType.OnUsePetTip);
                        ShowUsePetOtherInfo(true, UsePet[i].gameObject,(int)OnUsePetinfoList[j].dataId);
                        bFind = true;
                        break;
                    }
                }

                if (bFind)
                {
                    UsePetBind[i].gameObject.CustomActive(true);
                }
                else
                {
                    UsePetBind[i].gameObject.CustomActive(false);
                    ShowUsePetOtherInfo(false, UsePet[i].gameObject);
                }
            }
        }


        void ShowUsePetOtherInfo(bool havePet,GameObject parent,int petId = 0)
        {
            GameObject petType = Utility.FindChild("petType", parent);
            petType.CustomActive(havePet);

            GameObject petTypeText = Utility.FindChild("petTypeText", parent);
            petTypeText.CustomActive(!havePet);

            GameObject addIcon = Utility.FindChild("addIcon", parent);
            addIcon.CustomActive(!havePet);

            GameObject petName = Utility.FindChild("PetName", parent);
            if (petName != null)
            {
                if (havePet)
                {
                    PetTable petData = TableManager.GetInstance().GetTableItem<PetTable>(petId);
                    if (null != petData)
                    {
                        petName.GetComponent<Text>().SafeSetText(PetDataManager.GetInstance().GetColorName(petData.Name, petData.Quality));
                    }
                }
            }
            petName.CustomActive(havePet);
        }


        void SelectSuitableShowPet(bool bInit = false, int iSlotIndex = -1, bool bIsWear = false)
        {
            int showIndex = -1;

            PetInfo pet = PetDataManager.GetInstance().GetFollowPet();

            if (bInit && pet.dataId != 0)
            {
                PetTable petData = TableManager.GetInstance().GetTableItem<PetTable>((int)pet.dataId);
                if (petData != null && petData.PetType > PetTable.ePetType.PT_NONE)
                {
                    showIndex = (int)petData.PetType - 1;
                }
            }
            else
            {
                List<PetInfo> OnUsePetList = PetDataManager.GetInstance().GetOnUsePetList();

                bool bHasUsePet = false;

                if (bIsWear)
                {
                    for (int i = 0; i < OnUsePetList.Count; i++)
                    {
                        PetTable petData = TableManager.GetInstance().GetTableItem<PetTable>((int)OnUsePetList[i].dataId);
                        if (petData == null)
                        {
                            continue;
                        }

                        if ((iSlotIndex + 1) == (int)petData.PetType)
                        {
                            showIndex = iSlotIndex;
                            bHasUsePet = true;

                            break;
                        }
                    }
                }
                else
                {
                    int iIndex = MaxOnUsePetNum - 1;

                    for (int i = 0; i < OnUsePetList.Count; i++)
                    {
                        PetTable petData = TableManager.GetInstance().GetTableItem<PetTable>((int)OnUsePetList[i].dataId);
                        if (petData == null)
                        {
                            continue;
                        }

                        if ((iIndex + 1) > (int)petData.PetType)
                        {
                            iIndex = (int)petData.PetType - 1;
                        }

                        bHasUsePet = true;
                    }

                    if (bHasUsePet)
                    {
                        showIndex = iIndex;
                    }
                }
            }

            if (showIndex == -1)
            {
                _setNonePetEquiped(false);
                //mActorShowRoot.gameObject.CustomActive(false);
                return;
            }

            for (int i = 0; i < UsePet.Length; i++)
            {
                UsePet[i].isOn = false;

                if (i == showIndex)
                {
                    UsePet[i].isOn = true;
                }
            }
        }

        void UpdatePackageType()
        {
            for (int i = 0; i < mItemUIScrollList.Length; i++)
            {
                if ((PetPackageTab)i == packageTab)
                {
                    mItemUIScrollList[i].gameObject.CustomActive(true);
                }
                else
                {
                    mItemUIScrollList[i].gameObject.CustomActive(false);
                }
            }
        }

        void UpdateSelPetInfo()
        {
            PetTable petData = TableManager.GetInstance().GetTableItem<PetTable>((int)CurSelPetInfo.dataId);
            if (petData == null)
            {
                return;
            }

            if (SelPetIsFollowPet())
            {
                mShowInTown.isOn = true;
            }
            else
            {
                mShowInTown.isOn = false;
            }

            mName.text = PetDataManager.GetInstance().GetColorName(petData.Name, petData.Quality);
            mLevel.text = string.Format("Lv.{0}/{1}", CurSelPetInfo.level, petData.MaxLv);
            mSatiety.text = string.Format("{0}/{1}", CurSelPetInfo.hunger, MaxSatietyNum);
            UpdateActor((int)CurSelPetInfo.dataId);
            DrawPetExpBar(CurSelPetInfo.level, CurSelPetInfo.exp, petData.Quality, true);
        }

        void UpdateActor(int iPetID)
        {
            PetTable Pet = TableManager.instance.GetTableItem<PetTable>(iPetID);
            if (Pet == null)
            {
                Logger.LogErrorFormat("can not find PetTable with id:{0}", iPetID);
            }
            else
            {
                ResTable res = TableManager.instance.GetTableItem<ResTable>(Pet.ModeID);

                if (res == null)
                {
                    Logger.LogErrorFormat("can not find ResTable with id:{0}", Pet.ModeID);
                }
                else
                {
                    PlayerUtility.LoadPetAvatarRenderEx(iPetID, mAvatarRenderer);

                    Vector3 avatarPos = mAvatarRenderer.avatarPos;
                    avatarPos.y = Pet.ChangedHeight / 1000.0f;
                    mAvatarRenderer.avatarPos = avatarPos;

                    Quaternion qua = mAvatarRenderer.avatarRoation;
                    mAvatarRenderer.avatarRoation = Quaternion.Euler(qua.x, Pet.ModelOrientation / 1000.0f, qua.z);

                    var vscale= mAvatarRenderer.avatarScale;

                    Vector3 avatarScale = mAvatarRenderer.avatarScale;
                    avatarScale.y = avatarScale.x = avatarScale.z = Pet.PetModelSize/1000.0f;
                    mAvatarRenderer.avatarScale = avatarScale;
                }
            }
        }

        void DrawPetExpBar(int iLevel, UInt64 PetExp, PetTable.eQuality ePetQuality, bool force)
        {
            mExp.SetExp(PetExp, force, exp =>
            {
                return TableManager.instance.GetCurPetExpBar(iLevel, exp, ePetQuality);
            });
        }

        void UpdatePetListData()
        {
            PetInfoList.Clear();

            List<PetInfo> InitPetInfoList = PetDataManager.GetInstance().GetPetList();

            int iCount = 0;
            PetInfoList = PetDataManager.GetInstance().GetPetSortListBySortType(InitPetInfoList, ref iCount, CurSelSortType, MaxPetItemNum);

            int AddedNum = GetAddedGridNum(iCount);

            for (int i = 0; i < AddedNum; i++)
            {
                PetInfoList.Add(new PetInfo());
            }

            mGridNum.text = TR.Value("grid_info", iCount, MaxPetItemNum);
        }

        void UpdateCurSelOnUsePet()
        {
            List<PetInfo> OnUsePetList = PetDataManager.GetInstance().GetOnUsePetList();

            for (int i = 0; i < OnUsePetList.Count; i++)
            {
                if (OnUsePetList[i].id == CurSelPetInfo.id)
                {
                    PetDataManager.GetInstance().SetPetData(CurSelPetInfo, OnUsePetList[i]);
                    break;
                }
            }
        }

        PetInfo GetSelPetInfo(int iIdex)
        {
            List<PetInfo> petlist = PetDataManager.GetInstance().GetOnUsePetList();

            PetInfo eInfo = new PetInfo();

            for (int i = 0; i < petlist.Count; i++)
            {
                PetTable petData = TableManager.GetInstance().GetTableItem<PetTable>((int)petlist[i].dataId);
                if (petData != null && (int)petData.PetType == iIdex + 1)
                {
                    PetDataManager.GetInstance().SetPetData(eInfo, petlist[i]);
                    break;
                }
            }

            return eInfo;
        }

        bool IsCurSelPetRemove()
        {
            List<PetInfo> OnUsePetList = PetDataManager.GetInstance().GetOnUsePetList();

            for (int i = 0; i < OnUsePetList.Count; i++)
            {
                if (OnUsePetList[i].id == CurSelPetInfo.id)
                {
                    return false;
                }
            }

            return true;
        }

        bool SelPetIsFollowPet()
        {
            PetInfo FollowPet = PetDataManager.GetInstance().GetFollowPet();

            if (FollowPet.id <= 0)
            {
                return false;
            }
            else
            {
                if (FollowPet.id == CurSelPetInfo.id)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        int GetAddedGridNum(int RealCount)
        {
            int AddedNum = 0;

            if (MaxShowPetItemNum >= RealCount)
            {
                AddedNum = MaxShowPetItemNum - RealCount;
            }
            else
            {
                if (RealCount % 5 > 0)
                {
                    AddedNum = 5 - RealCount % 5;
                }
            }

            return AddedNum;
        }

        void PlayEquipEffect(int iIndex)
        {
            GameObject obj = AssetLoader.instance.LoadResAsGameObject(EquipEffectPath);
            if (obj == null)
            {
                return;
            }

            Utility.AttachTo(obj, UsePet[iIndex].gameObject);
        }

        void OnClickOKRemove()
        {
            SendRemovePetReq();
        }

        void SendRemovePetReq()
        {
            PetTable petdata = TableManager.GetInstance().GetTableItem<PetTable>((int)CurSelPetInfo.dataId);
            if (petdata == null)
            {
                return;
            }

            SceneSetPetSoltReq req = new SceneSetPetSoltReq();

            req.petType = (byte)petdata.PetType;
            req.petId = 0;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        void SendFollowPetReq(bool bFollow)
        {
            SceneSetPetFollowReq req = new SceneSetPetFollowReq();

            if (bFollow)
            {
                req.id = CurSelPetInfo.id;
            }
            else
            {
                req.id = 0;
            }

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            if (null != mAvatarRenderer)
            {
                while (global::Global.Settings.avatarLightDir.x > 360)
                    global::Global.Settings.avatarLightDir.x -= 360;
                while (global::Global.Settings.avatarLightDir.x < 0)
                    global::Global.Settings.avatarLightDir.x += 360;

                while (global::Global.Settings.avatarLightDir.y > 360)
                    global::Global.Settings.avatarLightDir.y -= 360;
                while (global::Global.Settings.avatarLightDir.y < 0)
                    global::Global.Settings.avatarLightDir.y += 360;

                while (global::Global.Settings.avatarLightDir.z > 360)
                    global::Global.Settings.avatarLightDir.z -= 360;
                while (global::Global.Settings.avatarLightDir.z < 0)
                    global::Global.Settings.avatarLightDir.z += 360;

                mAvatarRenderer.m_LightRot = global::Global.Settings.avatarLightDir;
            }
        }

        [UIControl("OnUsePet/UsePet{0}", typeof(Toggle), 1)]
        Toggle[] UsePet = new Toggle[MaxOnUsePetNum];

        [UIControl("OnUsePet/UsePet{0}/pos/PetItem", typeof(ComCommonBind), 1)]
        ComCommonBind[] UsePetBind = new ComCommonBind[MaxOnUsePetNum];

        [UIControl("Packet/Scroll View{0}", typeof(ComUIListScript), 1)]
        ComUIListScript[] mItemUIScrollList = new ComUIListScript[MaxPackageTypeNum];
        
        // [UIControl("PacketNavigation/SortTypeSelect/SortTypeList/SortType{0}/Text", typeof(Text), 1)]
        // Text[] SortTypeTexts = new Text[SortTypeNum];

		#region ExtraUIBind
		private GeAvatarRendererEx mAvatarRenderer = null;
		private GameObject mActorShowRoot = null;
		private Button mGroupUp = null;
		private Button mInfo = null;
		private Button mRemove = null;
		private Toggle mShowInTown = null;
        private Button mPropertyBtn = null;
		private Text mName = null;
		private Text mLevel = null;
		private ComExpBar mExp = null;
		private Text mGridNum = null;
		// private Button mBtSelSortType = null;
		// private Text mShowSortType = null;
		// private GameObject mSortTypeListRoot = null;
		private Text mSatiety = null;
		private GameObject mSortRoot = null;
		// private Button mProperty = null;
		private Text mSelectTips = null;
		// private Button mLook = null;
		private Button mGetPath = null;
		private GameObject mNoneEquipRoot = null;
		private GameObject mPetEggRedPoint = null;
		private GameObject mPetRedPoint = null;
		private GameObject mGroupUpRedPoint = null;
		private Toggle mPetToggle = null;
        private ComDropDownControl mComDropDownControl = null;
        private GameObject mDetailRoot = null;
        private StateController mMTipState = null;

        protected override void _bindExUI()
		{
			mAvatarRenderer = mBind.GetCom<GeAvatarRendererEx>("AvatarRenderer");
			mActorShowRoot = mBind.GetGameObject("ActorShowRoot");
			mGroupUp = mBind.GetCom<Button>("GroupUp");
			if (null != mGroupUp)
			{
				mGroupUp.onClick.AddListener(_onGroupUpButtonClick);
			}
			mInfo = mBind.GetCom<Button>("Info");
			if (null != mInfo)
			{
				mInfo.onClick.AddListener(_onInfoButtonClick);
			}
			mRemove = mBind.GetCom<Button>("Remove");
			if (null != mRemove)
			{
				mRemove.onClick.AddListener(_onRemoveButtonClick);
			}
			mShowInTown = mBind.GetCom<Toggle>("ShowInTown");
			if (null != mShowInTown)
			{
				mShowInTown.onValueChanged.AddListener(_onShowInTownToggleValueChange);
			}
            
            mPropertyBtn = mBind.GetCom<Button>("PropertyBtn");
            mPropertyBtn.onClick.AddListener(_onPropertyButtonClick);
			mName = mBind.GetCom<Text>("Name");
			mLevel = mBind.GetCom<Text>("Level");
			mExp = mBind.GetCom<ComExpBar>("Exp");
			mGridNum = mBind.GetCom<Text>("GridNum");
			// mBtSelSortType = mBind.GetCom<Button>("btSelSortType");
			// if (null != mBtSelSortType)
			// {
			// 	mBtSelSortType.onClick.AddListener(_onBtSelSortTypeButtonClick);
			// }
			// mShowSortType = mBind.GetCom<Text>("ShowSortType");
			// mSortTypeListRoot = mBind.GetGameObject("SortTypeListRoot");
			mSatiety = mBind.GetCom<Text>("Satiety");
			mSortRoot = mBind.GetGameObject("SortRoot");
			// mProperty = mBind.GetCom<Button>("Property");
			// if (null != mProperty)
			// {
			// 	mProperty.onClick.AddListener(_onPropertyButtonClick);
			// }
			mSelectTips = mBind.GetCom<Text>("SelectTips");
			// mLook = mBind.GetCom<Button>("Look");
			// if (null != mLook)
			// {
			// 	mLook.onClick.AddListener(_onLookButtonClick);
			// }
			mGetPath = mBind.GetCom<Button>("GetPath");
			if (null != mGetPath)
			{
				mGetPath.onClick.AddListener(_onGetPathButtonClick);
			}
			mNoneEquipRoot = mBind.GetGameObject("NoneEquipRoot");
			mPetEggRedPoint = mBind.GetGameObject("petEggRedPoint");
			mPetRedPoint = mBind.GetGameObject("petRedPoint");
			mGroupUpRedPoint = mBind.GetGameObject("groupUpRedPoint");
			mPetToggle = mBind.GetCom<Toggle>("PetToggle");
			if (null != mPetToggle)
			{
			}
            mComDropDownControl = mBind.GetCom<ComDropDownControl>("ComDropDownControl");
            mDetailRoot = mBind.GetGameObject("DetailRoot");
            mMTipState = mBind.GetCom<StateController>("mTipState");
        }
		
		protected override void _unbindExUI()
		{
			mAvatarRenderer = null;
			mActorShowRoot = null;
			if (null != mGroupUp)
			{
				mGroupUp.onClick.RemoveListener(_onGroupUpButtonClick);
			}
			mGroupUp = null;
			if (null != mInfo)
			{
				mInfo.onClick.RemoveListener(_onInfoButtonClick);
			}
			mInfo = null;
			if (null != mRemove)
			{
				mRemove.onClick.RemoveListener(_onRemoveButtonClick);
			}
			mRemove = null;
			if (null != mShowInTown)
			{
				mShowInTown.onValueChanged.RemoveListener(_onShowInTownToggleValueChange);
			}
			mShowInTown = null;
            mPropertyBtn.onClick.RemoveListener(_onPropertyButtonClick);
            mPropertyBtn = null;
            
			mName = null;
			mLevel = null;
			mExp = null;
			mGridNum = null;
			// if (null != mBtSelSortType)
			// {
			// 	mBtSelSortType.onClick.RemoveListener(_onBtSelSortTypeButtonClick);
			// }
			// mBtSelSortType = null;
			// mShowSortType = null;
			// mSortTypeListRoot = null;
			mSatiety = null;
			mSortRoot = null;
			// if (null != mProperty)
			// {
			// 	mProperty.onClick.RemoveListener(_onPropertyButtonClick);
			// }
			// mProperty = null;
			mSelectTips = null;
			// if (null != mLook)
			// {
			// 	mLook.onClick.RemoveListener(_onLookButtonClick);
			// }
			// mLook = null;
			if (null != mGetPath)
			{
				mGetPath.onClick.RemoveListener(_onGetPathButtonClick);
			}
			mGetPath = null;
			mNoneEquipRoot = null;
			mPetEggRedPoint = null;
			mPetRedPoint = null;
			mGroupUpRedPoint = null;
			if (null != mPetToggle)
			{
			}
			mPetToggle = null;
            mComDropDownControl = null;
            mDetailRoot = null;
            mMTipState = null;
        }
		#endregion

        #region Callback
        private void _onBtCloseButtonClick()
        {
           frameMgr.CloseFrame(this);
        }

        private void _onGroupUpButtonClick()
        {
            if (ChijiDataManager.GetInstance().CheckCurrentSystemIsClientSystemGameBattle())
            {
                return;
            }
            ClientSystemManager.GetInstance().OpenFrame<PetUpgradeFrame>(FrameLayer.Middle, CurSelPetInfo);
            //PetDataManager.GetInstance().OpenPetInfoFrame(PetInfoTabType.Pet_UpLevel, CurSelPetInfo);

            if (_isCurrentSelectedPetContainFeedCounts())
            {
                PetDataManager.GetInstance().IsUserClickFeedCount = true;
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PetGoldFeedClick);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPointChanged, ERedPoint.PackageMain);

            _updateGroupUpRedPoint(null);
        }

        private bool _isCurrentSelectedPetContainFeedCounts()
        {
            return PetDataManager.GetInstance().IsSelectPetsContainGoldFeedCount();
        }

        private void _onInfoButtonClick()
        {
            if (ChijiDataManager.GetInstance().CheckCurrentSystemIsClientSystemGameBattle())
            {
                return;
            }

            //PetDataManager.GetInstance().OpenPetInfoFrame(PetInfoTabType.Pet_Feed, CurSelPetInfo);
            ClientSystemManager.GetInstance().OpenFrame<FeedPetFrame>(FrameLayer.Middle, CurSelPetInfo);
        }

        private void _onRemoveButtonClick()
        {
            SystemNotifyManager.SystemNotify(8509, OnClickOKRemove);
        }

        private void _onShowInTownToggleValueChange(bool changed)
        {
            PetInfo pet = PetDataManager.GetInstance().GetFollowPet();

            if (changed)
            {
                if (pet.id == CurSelPetInfo.id)
                {
                    return;
                }
            }
            else
            {
                if (pet.id != CurSelPetInfo.id)
                {
                    return;
                }
            }

            SendFollowPetReq(changed);
        }

        private void _onBtSelSortTypeButtonClick()
        {
            // mSortTypeListRoot.CustomActive(!mSortTypeListRoot.activeSelf);
        }

        private void _onPropertyButtonClick()
        {
            //PetDataManager.GetInstance().OpenPetInfoFrame(PetInfoTabType.Pet_Property, CurSelPetInfo);
            ClientSystemManager.GetInstance().OpenFrame<PetPropertyFrame>(FrameLayer.Middle, CurSelPetInfo);
        }

        private void _onLookButtonClick()
        {
            PetDataManager.GetInstance().OnShowPetTips(CurSelPetInfo, PlayerBaseData.GetInstance().JobTableID, PetTipsType.OnUsePetTip);
        }

        private void _onGetPathButtonClick()
        {
            if (ChijiDataManager.GetInstance().CheckCurrentSystemIsClientSystemGameBattle())
            {
                return;
            }
            ItemComeLink.OnLink(102, 0);
        }
        #endregion
    }
}

