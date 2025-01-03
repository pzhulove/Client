using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using ProtoTable;
using System;
using Protocol;
///////删除linq
using Scripts.UI;

namespace GameClient
{
    /*
    class TittleBookFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TitleBookFrame/TittleBookFrame";
        }

        [UIControl("", typeof(StateController))]
        StateController comState;

        [UIControl("merge/Horizen", typeof(ComCache))]
        ComCache comCache;

        bool bToTitleMerge = false;

        protected override void _OnOpenFrame()
        {
            bToTitleMerge = false;
            if(null != userData)
            {
                bToTitleMerge = (bool)userData;
            }

            //Logger.LogErrorFormat("bToTitleMerge = {0}", bToTitleMerge);

            m_akCachedObjects.Clear();
            m_akCachedTableObjects.Clear();
            m_akRecycleObject.Clear();

            goTitleAttrParent = Utility.FindChild(frame, "tittles/Detail/ScrollView/Viewport/content");
            tittleName = Utility.FindComponent<Text>(frame, "tittles/Detail/tittle/name");
            tittleName.text = "";
            tittleDesc = Utility.FindComponent<Text>(frame, "tittles/Detail/bottom/desc");
            tittleDesc.text = "";
            textEquipt = Utility.FindComponent<Text>(frame, "FuncControls/Equipt/Text");
            FuncControls = Utility.FindChild(frame, "FuncControls");
            FuncControls.CustomActive(false);

            BtnLookUp = Utility.FindChild(frame, "FuncControls/LookUp");
            BtnLookUp.CustomActive(false);

            m_eTittleComeType = TittleComeType.TCT_INVALID;
            m_bHasCreated = false;

            _AddButton("merge/btnMerge", _OnClickMerge);

            TittleBookManager.GetInstance().onAddTittle += OnAddTittle;
            TittleBookManager.GetInstance().onUpdateTittle += OnUpdateTittle;
            TittleBookManager.GetInstance().onRemoveTittle += OnRemoveTittle;
            TittleBookManager.GetInstance().onRemoveTableTittle += OnRemoveTableTittle;
            TittleBookManager.GetInstance().onAddTableTittle += OnAddTableTittle;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPointChanged, OnRedPointChanged);

            ItemDataManager.GetInstance().onAddNewItem += _OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem += _OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem += _OnRemoveItem;

            PlayerBaseData.GetInstance().onMoneyChanged += _OnMoneyChanged;

            if (null != comState.Key)
            {
                comState.Key = "normal";
            }

            _InitMergeItems();
            _InitMergeMaterials();
            _InitTabs();
            _InitAllTittle();
            if (bToTitleMerge)
            {
                m_akToggles[(int)TittleComeType.TCT_MERGE].isOn = true;
                bToTitleMerge = false;
            }
        }

        void OnRedPointChanged(UIEvent uiEvent)
        {
            for(int i = 0; i < (int)TittleComeType.TCT_COUNT; ++i)
            {
                if(m_akTabMark[i] != null)
                {
                    bool bCheck = TittleBookManager.GetInstance().IsTittleTabMark((TittleComeType)i);
                    m_akTabMark[i].CustomActive(bCheck);
                }
            }
        }

        void OnAddTittle(ulong uid)
        {
            m_akCachedObjects.Create(uid, new object[] {uid,m_goTittlePrefab, m_goTittleParent,TittleBookManager.GetInstance().GetTittleType(uid), this ,true});
            m_akCachedObjects.FilterObject(uid, new object[] { m_eTittleComeType });

            _SortActiveObjects();

            _UpdateMergeItems();
            _UpdateMergeMaterials();
        }

        void _SortActiveObjects()
        {
            List<ItemObject> activeObjects = m_akCachedObjects.ActiveObjects.Values.ToList();
            activeObjects.InsertRange(activeObjects.Count,m_akCachedTableObjects.ActiveObjects.Values.ToList());

            for(int i = 0; i < activeObjects.Count; ++i)
            {
                if(!activeObjects[i].IsActive())
                {
                    activeObjects.RemoveAt(i--);
                }
            }

            if (activeObjects.Count > 0)
            {
                activeObjects.Sort((x, y) =>
                {
                    return x.itemTable.IdSequence - y.itemTable.IdSequence;
                });

                for (int i = 0; i < activeObjects.Count; ++i)
                {
                    activeObjects[i].SetAsLastSibling();
                }
            }
        }

        void _OnClickMerge()
        {
            if (null == ComMergeTitle.Selected)
            {
                Logger.LogErrorFormat("has no selected mergetitle!!");
                return;
            }

            if(!ComMergeTitle.checkMaterialEnough(ComMergeTitle.Selected,true))
            {
                //SystemNotifyManager.SysNotifyTextAnimation(string.Format("合成{0}所需材料不足!", ComMergeTitle.Selected.item.Name),CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                return;
            }

            if(!ComMergeTitle.checkMoneyEnough(ComMergeTitle.Selected))
            {
                ItemComeLink.OnLink(ComMergeTitle.Selected.getMoneyId(), ComMergeTitle.Selected.getMoneyCount() - ComMergeTitle.Selected.getOwnedMoneyCount(),true);
                return;
            }

            _OnConfirmToMerge();
        }

        bool _CheckHasEquiptedItem(List<CostItemManager.CostInfo> infos)
        {
            if(null != infos)
            {
                var equipments = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
                List<object> objTableIds = GamePool.ListPool<object>.Get();
                for(int i = 0; i < equipments.Count; ++i)
                {
                    var myItems = ItemDataManager.GetInstance().GetItem(equipments[i]);
                    if(null != myItems)
                    {
                        objTableIds.Add(myItems.TableID);
                    }
                }

                bool bEquiped = false;
                for (int i = 0; i < infos.Count; ++i)
                {
                    var info = infos[i];
                    int ownedCount = ItemDataManager.GetInstance().GetOwnedItemCount(info.nMoneyID);
                    if (null != info && info.nCount > 0 && info.nCount == ownedCount && objTableIds.Contains(info.nMoneyID))
                    {
                        bEquiped = true;
                        break;
                    }
                }

                GamePool.ListPool<object>.Release(objTableIds);

                return bEquiped;
            }
            return false;
        }

        void _OnConfirmToMerge()
        {
            if (ComMergeTitle.Selected != null)
            {
                List<CostItemManager.CostInfo> arrCostInfos = new List<CostItemManager.CostInfo>();
                for (int i = 0; i < ComMergeTitle.Selected.materials.Count; ++i)
                {
                    CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();
                    costInfo.nMoneyID = ComMergeTitle.Selected.materials[i].id;
                    costInfo.nCount = ComMergeTitle.Selected.materials[i].count;
                    arrCostInfos.Add(costInfo);
                }
                for (int i = 0; i < ComMergeTitle.Selected.moneys.Count; ++i)
                {
                    CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();
                    costInfo.nMoneyID = ComMergeTitle.Selected.moneys[i].id;
                    costInfo.nCount = ComMergeTitle.Selected.moneys[i].count;
                    arrCostInfos.Add(costInfo);
                }

                if(_CheckHasEquiptedItem(arrCostInfos))
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("title_merge_failed_for_equipted"), CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                    return;
                }

                CostItemManager.GetInstance().TryCostMoneiesDefault(arrCostInfos, () =>
                {
                    frameMgr.OpenFrame<EquipForgeResultFrame>(FrameLayer.Middle, ComMergeTitle.Selected.item.ID);
                });
            }
        }

        [UIControl("merge/btnMerge/horizen/Icon", typeof(Image))]
        Image moneyIcon;

        [UIControl("merge/btnMerge/horizen/Text", typeof(Text))]
        Text moneyCount;

        void _UpdateMergeMoney()
        {
            if(null == ComMergeTitle.Selected)
            {
                moneyIcon.CustomActive(false);
                moneyCount.text = "合成";
            }
            else
            {
                bool bNeedMoney = ComMergeTitle.Selected.getMoneyCount() > 0;
                if(!bNeedMoney)
                {
                    moneyIcon.CustomActive(false);
                    moneyCount.text = "合成";
                }
                else
                {
                    moneyIcon.CustomActive(true);
                    // moneyIcon.sprite = AssetLoader.instance.LoadRes(ComMergeTitle.Selected.getMoneyIcon(), typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref moneyIcon, ComMergeTitle.Selected.getMoneyIcon());
                    

                    int iOwnedCount = ItemDataManager.GetInstance().GetOwnedItemCount(ComMergeTitle.Selected.getMoneyId());
                    if (iOwnedCount >= ComMergeTitle.Selected.getMoneyCount())
                    {
                        moneyCount.text = string.Format("{0}合成", ComMergeTitle.Selected.getMoneyCount());
                    }
                    else
                    {
                        moneyCount.text = string.Format("<color=#ff0000>{0}</color>合成", ComMergeTitle.Selected.getMoneyCount());
                    }
                }
            }
        }

        void OnUpdateTittle(ulong uid)
        {
            m_akCachedObjects.RefreshObject(uid, new object[] { uid, m_goTittlePrefab, m_goTittleParent, TittleBookManager.GetInstance().GetTittleType(uid), this ,true});
            m_akCachedObjects.FilterObject(uid, new object[] { m_eTittleComeType });

            _UpdateMergeItems();
            _UpdateMergeMaterials();
        }

        void _OnAddNewItem(List<Item> items)
        {
            _UpdateMergeItems();
            _UpdateMergeMaterials();
        }
        void _OnRemoveItem(ItemData data)
        {
            _UpdateMergeItems();
            _UpdateMergeMaterials();
        }
        void _OnUpdateItem(List<Item> items)
        {
            _UpdateMergeItems();
            _UpdateMergeMaterials();
        }

        void _OnMoneyChanged(PlayerBaseData.MoneyBinderType eMoneyBinderType)
        {
            _UpdateMergeItems();
            _UpdateMergeMaterials();
        }

        void _OnSetDefaultTittle(ulong uid)
        {
            if (ItemObject.selectedItem != null && ItemObject.selectedItem.itemData.GUID == uid)
            {
                ItemObject.Clear();

                List<ulong> outValues = null;
                if (TittleBookManager.GetInstance().GetTittle(TittleComeType.TCT_TRADE, out outValues) && outValues.Count > 0)
                {
                    var target = m_akCachedObjects.GetObject(outValues[0]);
                    if (target != null)
                    {
                        target.OnSelected();
                        return;
                    }
                }

                for (int i = 0; i < (int)TittleComeType.TCT_COUNT; ++i)
                {
                    if (TittleBookManager.GetInstance().GetTittle((TittleComeType)i, out outValues) && outValues.Count > 0)
                    {
                        var target = m_akCachedObjects.GetObject(outValues[0]);
                        if (target != null)
                        {
                            target.OnSelected();
                            if (!m_akToggles[i].isOn)
                            {
                                m_akToggles[i].isOn = true;
                            }
                            return;
                        }
                    }
                }

                for (int i = 0; i < (int)TittleComeType.TCT_COUNT; ++i)
                {
                    if (TittleBookManager.GetInstance().GetTableTittle((TittleComeType)i, out outValues) && outValues.Count > 0)
                    {
                        var target = m_akCachedTableObjects.GetObject(outValues[0]);
                        if (target != null)
                        {
                            target.OnSelected();
                            if (!m_akToggles[i].isOn)
                            {
                                m_akToggles[i].isOn = true;
                            }
                            return;
                        }
                    }
                }
            }
        }

        void OnRemoveTittle(ulong uid)
        {
            m_akCachedObjects.RecycleObject(uid);

            if(null == ItemDataManager.GetInstance().GetItem(uid))
            {
                _OnSetDefaultTittle(uid);
            }

            _UpdateMergeItems();
            _UpdateMergeMaterials();
        }

        void OnRemoveTableTittle(ulong tableid)
        {
            m_akCachedTableObjects.RecycleObject(tableid);
            m_akCachedObjects.RefreshAllObjects();
        }

        void OnAddTableTittle(ulong tableid)
        {
            var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)tableid);
            if(item != null)
            {
                var type = TittleBookManager.GetInstance().GetTittleType(item);
                if(type != TittleComeType.TCT_INVALID)
                {
                    m_akCachedTableObjects.Create(tableid, new object[] { tableid, m_goTittlePrefab, m_goTittleParent, type, this, false });
                    m_akCachedTableObjects.FilterObject(tableid, new object[] { m_eTittleComeType });
                    _SortActiveObjects();
                }
            }

            m_akCachedObjects.RefreshAllObjects();
        }

        void OnTittleMarkChanged(TittleComeType eTittleComeType)
        {
            if(eTittleComeType > TittleComeType.TCT_INVALID && eTittleComeType < TittleComeType.TCT_COUNT)
            {
                m_akTabMark[(int)eTittleComeType].CustomActive(TittleBookManager.GetInstance().IsTittleTabMark(eTittleComeType));
            }
        }

        protected override void _OnCloseFrame()
        {
            ItemObject.Clear();
            m_akCachedObjects.DestroyAllObjects();
            m_akCachedTableObjects.DestroyAllObjects();

            TittleBookManager.GetInstance().onAddTittle -= OnAddTittle;
            TittleBookManager.GetInstance().onUpdateTittle -= OnUpdateTittle;
            TittleBookManager.GetInstance().onRemoveTittle -= OnRemoveTittle;
            TittleBookManager.GetInstance().onRemoveTableTittle -= OnRemoveTableTittle;
            TittleBookManager.GetInstance().onAddTableTittle -= OnAddTableTittle;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPointChanged, OnRedPointChanged);

            ItemDataManager.GetInstance().onAddNewItem -= _OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem -= _OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem -= _OnRemoveItem;

            PlayerBaseData.GetInstance().onMoneyChanged -= _OnMoneyChanged;

            if(null != comMergeTitles)
            {
                comMergeTitles.onBindItem = null;
                comMergeTitles.onItemSelected = null;
                comMergeTitles.onItemVisiable = null;
                comMergeTitles = null;
            }

            m_bMergeTitlesInit = false;
            ComMergeTitle.Clear();

            m_akRecycleObject.Clear();
        }

        #region _InitTabs
        GameObject m_goTabPrefab;
        TittleComeType m_eTittleComeType = TittleComeType.TCT_INVALID;
        Toggle[] m_akToggles = new Toggle[(int)TittleComeType.TCT_COUNT];
        GameObject[] m_akTabMark = new GameObject[(int)TittleComeType.TCT_COUNT];

        void _InitTabs()
        {
            m_goTabPrefab = Utility.FindChild(frame, "tabs/tab");
            m_goTabPrefab.CustomActive(false);
            for(int i = 0; i < (int)TittleComeType.TCT_COUNT; ++i)
            {
                GameObject goCurrent = GameObject.Instantiate(m_goTabPrefab);
                goCurrent.name = ((TittleComeType)i).ToString();
                Utility.AttachTo(goCurrent, m_goTabPrefab.transform.parent.gameObject);
                string content = Utility.GetEnumDescription((TittleComeType)i);
                Text text = Utility.FindComponent<Text>(goCurrent, "Label");
                text.text = content;
                text = Utility.FindComponent<Text>(goCurrent, "Checkmark/Label");
                text.text = content;
                goCurrent.CustomActive(true);
                Toggle toggle = goCurrent.GetComponent<Toggle>();
                toggle.onValueChanged.RemoveAllListeners();
                int k = i;
                toggle.onValueChanged.AddListener((bool bValue)=>
                {
                    if(bValue)
                    {
                        _OnTabChanged((TittleComeType)k);
                    }
                });
                m_akToggles[i] = toggle;
                m_akTabMark[i] = Utility.FindChild(toggle.gameObject, "tabMark");
            }

            m_eTittleComeType = TittleComeType.TCT_SHOP;
            _UpdateTabMark();
        }

        void _UpdateTabMark()
        {
            for(int i = 0; i < (int)TittleComeType.TCT_COUNT;++i)
            {
                m_akTabMark[i].CustomActive(TittleBookManager.GetInstance().IsTittleTabMark((TittleComeType)i));
            }
        }
        #endregion

        #region _InitTittle
        void _InitAllTittle()
        {
            StartCoroutine(_AnsyInitAllTittle());
        }

        void _SetDefaultSelected()
        {
            //Logger.LogErrorFormat("_SetDefaultSelected start 1000");
            if(ItemObject.selectedItem == null)
            {
                for(int i = 0; i < (int)TittleComeType.TCT_COUNT;++i)
                {
                    int iCurrent = ((int)(m_eTittleComeType) + i) % ((int)(TittleComeType.TCT_COUNT));
                    //Logger.LogErrorFormat("_SetDefaultSelected iCurrent = {1},eCurrent = {0}",(TittleComeType)iCurrent, iCurrent);
                    var objectLists = m_akCachedObjects.GetObjectListByFilter(new object[] { m_eTittleComeType });
                    if(objectLists.Count > 0)
                    {
                        objectLists[0].OnSelected();
                        if(!m_akToggles[iCurrent].isOn)
                        {
                            //Logger.LogErrorFormat("_AnsyInitAllTittle m_eTittleComeType CCCCC = {0}", iCurrent);
                            m_akToggles[iCurrent].isOn = true;
                        }
                        break;
                    }

                    objectLists = m_akCachedTableObjects.GetObjectListByFilter(new object[] { m_eTittleComeType });
                    if (objectLists.Count > 0)
                    {
                        objectLists[0].OnSelected();
                        if (!m_akToggles[iCurrent].isOn)
                        {
                            //Logger.LogErrorFormat("_AnsyInitAllTittle m_eTittleComeType DDDDD = {0}", iCurrent);
                            m_akToggles[iCurrent].isOn = true;
                        }
                        break;
                    }
                }
            }
        }

        GameObject m_goTittlePrefab;
        GameObject m_goTittleParent;

        class ItemObject : CachedObject
        {
            GameObject goTittlePrefab;
            GameObject goCurrent;
            GameObject goParent;
            GameObject comParent;
            ulong guid;
            public ItemData itemData;
            public ProtoTable.ItemTable itemTable;
            ComItem comItem;
            GameObject goMark;
            Text textMark;
            public ItemTable tittleItem;
            GameObject goAnimation;
            GameObject goAnimationParent;
            TittleComeType eTittleComeType = TittleComeType.TCT_INVALID;
            public TittleComeType TittleComeType
            {
                get { return eTittleComeType; }
            }
            SpriteAniRender comAniRender;
            GameObject goSelectedMark;
            Button button;
            TittleBookFrame THIS;
            Text unAcquiredName;
            //GameObject goNewMark;
            Text timeLimit;
            bool bAcquired;
            UIGray comGray;
            bool bEquiped = false;
            public bool Equiped
            {
                get
                {
                    return bEquiped;
                }
            }
            bool bHasExtra = false;
            public bool HasExtra
            {
                get
                {
                    return bHasExtra;
                }
            }
            bool bCanTrade = false;
            public bool CanTrade
            {
                get
                {
                    return bCanTrade;
                }
            }

            public bool Acquired
            {
                get { return bAcquired; }
            }


            public static ItemObject selectedItem;

            public override void SetAsLastSibling()
            {
                if(goCurrent != null)
                {
                    goCurrent.transform.SetAsLastSibling();
                }
            }

            public bool IsActive()
            {
                return goCurrent != null && goCurrent.activeSelf;
            }

            void _Update()
            {
                if (bAcquired)
                {
                    itemData = ItemDataManager.GetInstance().GetItem(guid);
                }
                else
                {
                    itemData = GameClient.ItemDataManager.CreateItemDataFromTable((int)guid);
                }
                itemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)itemData.TableID);
                comItem.Setup(itemData, OnItemClicked);

                if (itemData != null)
                {
                    goCurrent.name = itemData.TableID.ToString();
                }

                if (goAnimation != null)
                {
                    GameObject.Destroy(goAnimation);
                    goAnimation = null;
                }

                comAniRender.SetEnable(false);

                if (itemData != null)
                {
                    if (itemTable != null && itemTable.Path2.Count == 4)
                    {
                        comAniRender.Reset(itemTable.Path2[0], itemTable.Path2[1], int.Parse(itemTable.Path2[2]), float.Parse(itemTable.Path2[3]));
                    }
                    comAniRender.SetEnable(bAcquired);
                    unAcquiredName.text = itemData.Name;
                    unAcquiredName.CustomActive(!bAcquired);
                    comGray.enabled = !bAcquired;
                    // comGray.SetEnable(!bAcquired);
                    //goNewMark.CustomActive(TittleBookManager.GetInstance().IsTittleMark(guid));

                    bCanTrade = TittleBookManager.GetInstance().CanTrade(itemData);
                    bEquiped = itemData.PackageType == EPackageType.WearEquip;
                    bHasExtra = TittleBookManager.GetInstance().HasExtraTitle(itemData);
                    textMark.CustomActive(itemData.PackageType == EPackageType.WearEquip || bHasExtra);
                    if(itemData.PackageType == EPackageType.WearEquip)
                    {
                        textMark.text = TR.Value("title_has_equiped");
                    }
                    else if(bHasExtra)
                    {
                        textMark.text = TR.Value("title_has_owned");
                    }
                    timeLimit.CustomActive(eTittleComeType == TittleComeType.TCT_TIMELIMITED);
                    if (eTittleComeType == TittleComeType.TCT_TIMELIMITED)
                    {
                        timeLimit.text = itemData.GetTimeLeftDescByDay();
                    }
                }
            }

            public override void OnDestroy()
            {
                if(comItem != null)
                {
                    comItem.Setup(null, null);
                    comItem = null;
                }
            }

            public override void OnCreate(object[] param)
            {
                try
                {
                    guid = (ulong)param[0];
                    goTittlePrefab = param[1] as GameObject;
                    goParent = param[2] as GameObject;
                    eTittleComeType = (TittleComeType)param[3];
                    THIS = param[4] as TittleBookFrame;
                    bAcquired = (bool)param[5];

                    if (goCurrent == null)
                    {
                        goCurrent = GameObject.Instantiate(goTittlePrefab);
                        comParent = Utility.FindChild(goCurrent, "EnableMark/Icon");
                        comItem = THIS.CreateComItem(comParent);

                        button = goCurrent.GetComponent<Button>();

                        textMark = Utility.FindComponent<Text>(goCurrent, "EnableMark/EquiptedMark");
                        goSelectedMark = Utility.FindChild(goCurrent, "SelectMark");
                        goAnimationParent = Utility.FindChild(goCurrent, "EnableMark/Animation");
                        unAcquiredName = Utility.FindComponent<Text>(goAnimationParent, "Name");
                        comAniRender = Utility.FindComponent<SpriteAniRender>(goCurrent, "EnableMark/Animation");
                        //goNewMark = Utility.FindChild(goCurrent, "EnableMark/NewMark");
                        timeLimit = Utility.FindComponent<Text>(goCurrent, "EnableMark/timeLimit");
                        timeLimit.supportRichText = true;
                        comGray = Utility.FindComponent<UIGray>(goCurrent, "EnableMark");
                    }

                    Utility.AttachTo(goCurrent, goParent);

                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(OnSelected);

                    
                    _Update();
                    if (this == ItemObject.selectedItem)
                    {
                        THIS.OnSetTarget(this);
                    }

                    Enable();
                }
                catch (Exception e)
                {
                    Disable();
                    Logger.LogErrorFormat("guid = {0} bAcquired = {1}", guid,bAcquired);
                }
            }

            public override void OnRecycle()
            {
                Disable();
            }

            public override void OnDecycle(object[] param)
            {
                OnCreate(param);
            }

            public override void OnRefresh(object[] param)
            {
                _Update();
            }

            public override void Enable()
            {
                if(goCurrent != null)
                {
                    goCurrent.CustomActive(true);
                }
            }
            public override void Disable()
            {
                if (goCurrent != null)
                {
                    goCurrent.CustomActive(false);
                }
            }

            public override bool NeedFilter(object[] param)
            {
                if(null == itemData)
                {
                    return true;
                }

                var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(itemData.TableID);
                if(item == null)
                {
                    return true;
                }

                if(item.OldTitle == 1)
                {
                    var realItem = ItemDataManager.GetInstance().GetItem(itemData.GUID);
                    if(null == realItem)
                    {
                        return true;
                    }
                }

                return goCurrent == null || eTittleComeType != (TittleComeType)param[0];
            }

            public void OnUpdate()
            {
                if(goCurrent != null && goCurrent.activeSelf)
                {
                    if (eTittleComeType == TittleComeType.TCT_TIMELIMITED)
                    {
                        timeLimit.text = itemData.GetTimeLeftDescByDay();
                    }
                }
            }

            void OnItemClicked(GameObject obj, ItemData item)
            {

            }

            public void OnSelected()
            {
                if(selectedItem != this)
                {
                    if(selectedItem != null)
                    {
                        selectedItem.SetSelected(false);
                        selectedItem = null;
                    }
                    selectedItem = this;
                    selectedItem.SetSelected(true);
                    if(itemData != null && true == THIS.m_bHasCreated)
                    {
                        //goNewMark.CustomActive(false);
                        ItemDataManager.GetInstance().NotifyItemBeOld(itemData);
                        _Update();
                    }
                    THIS.OnSetTarget(this);
                }
            }

            void SetSelected(bool bValue)
            {
                if(goSelectedMark != null)
                {
                    goSelectedMark.CustomActive(bValue);
                }
            }

            public static void Clear()
            {
                if(selectedItem != null)
                {
                    selectedItem.SetSelected(false);
                }
                selectedItem = null;
            }
        }

        CachedObjectDicManager<ulong, ItemObject> m_akCachedObjects = new CachedObjectDicManager<ulong, ItemObject>();
        CachedObjectDicManager<ulong, ItemObject> m_akCachedTableObjects = new CachedObjectDicManager<ulong, ItemObject>();

        Text tittleName;
        GameObject goTitleAttrParent;
        Text tittleDesc;
        GameObject FuncControls;
        GameObject BtnLookUp;
        Text textEquipt;
        [UIControl("FuncControls/Equipt", typeof(Button))]
        Button BtnEquipt;

        [UIControl("FuncControls/Trade", typeof(Button))]
        Button BtnTrade;

        void _UpdateTitleDesc(ItemData itemData)
        {
            if(null != itemData)
            {
                tittleName.text = itemData.GetColorName();

                for (int i = 0; i < goTitleAttrParent.transform.childCount; ++i)
                {
                    var transform = goTitleAttrParent.transform.GetChild(i);
                    if (transform.gameObject.activeSelf)
                    {
                        List<GameObject> outValue = null;
                        if (!m_akRecycleObject.TryGetValue(transform.name, out outValue))
                        {
                            outValue = new List<GameObject>();
                            m_akRecycleObject.Add(transform.name, outValue);
                        }
                        outValue.Add(transform.gameObject);
                        transform.gameObject.CustomActive(false);
                    }
                }

                var tiplist = Utility.GetTitleTipItemList(itemData);
                for (int i = 0; i < tiplist.Count; ++i)
                {
                    var current = tiplist[i];
                    GameObject goNow = null;
                    List<GameObject> outValue = null;
                    if (m_akRecycleObject.TryGetValue(current.Prefabpath, out outValue) && outValue.Count > 0)
                    {
                        goNow = outValue[0];
                        outValue.RemoveAt(0);
                        goNow.CustomActive(true);
                    }
                    else
                    {
                        goNow = AssetLoader.instance.LoadRes(current.Prefabpath, typeof(GameObject)).obj as GameObject;
                        goNow.name = current.Prefabpath;
                    }

                    if (goNow == null)
                    {
                        continue;
                    }

                    Utility.AttachTo(goNow, goTitleAttrParent);
                    goNow.transform.localScale = Vector3.one;
                    goNow.transform.SetAsLastSibling();

                    if (current.ETipContentType == Utility.TipContent.TipContentType.TCT_BLANK_DESC)
                    {
                        LayoutElement layout = goNow.GetComponent<LayoutElement>();
                        layout.preferredHeight = current.iParam0;
                    }
                    else
                    {
                        var smartCom = goNow.GetComponent<SmartTipContent>();
                        if (smartCom != null)
                        {
                            if (current != null && !current.IsNull)
                            {
                                smartCom.SetText(current.left, current.right);
                            }
                            else
                            {
                                smartCom.SetText("", "");
                            }
                        }
                    }
                }
            }
        }

        void OnSetTarget(ItemObject itemObject)
        {
            FuncControls.CustomActive(itemObject != null);
            BtnLookUp.CustomActive(itemObject != null);
            BtnEquipt.CustomActive(itemObject != null && itemObject.Acquired && itemObject.itemData != null &&
                (
                !TittleBookManager.GetInstance().CanTrade(itemObject.itemData) || 
                TittleBookManager.GetInstance().CanTrade(itemObject.itemData) && 
                !TittleBookManager.GetInstance().HasBindedTitle(itemObject.itemData.TableID))
                );
            BtnTrade.CustomActive(itemObject != null && itemObject.CanTrade);

            if (itemObject != null)
            {
                textEquipt.text = itemObject.itemData.PackageType == EPackageType.WearEquip ? "卸下" : "穿戴";
            }

            if (itemObject.itemTable != null)
            {
                tittleDesc.text = itemObject.itemTable.ComeDesc;
            }
            else
            {
                tittleDesc.text = "";
            }

            _UpdateTitleDesc(itemObject.itemData);
        }

        public int getLoopIndex()
        {
            if(m_eTittleComeType == TittleComeType.TCT_INVALID)
            {
                return 0;
            }
            return (int)m_eTittleComeType;
        }

        Dictionary<string,List<GameObject>> m_akRecycleObject = new Dictionary<string, List<GameObject>>();
        bool m_bHasCreated = false;
        IEnumerator _AnsyInitAllTittle()
        {
            //初始化当前称号
            m_bHasCreated = true;
            int iCount = 0;
            m_goTittlePrefab = Utility.FindChild(frame, "tittles/ScrollView/Viewport/content/prefabs");
            m_goTittlePrefab.CustomActive(false);
            m_goTittleParent = m_goTittlePrefab.transform.parent.gameObject;

            //Logger.LogErrorFormat("_AnsyInitAllTittle m_eTittleComeType={0}", m_eTittleComeType);

            int iLoopIndex = getLoopIndex();

            //Logger.LogErrorFormat("_AnsyInitAllTittle iLoopIndex={0}", m_eTittleComeType);
            int iLoopCount = (int)TittleComeType.TCT_COUNT;
            for (int i = 0; i < iLoopCount; ++i)
            {
                int eCurrent = ((int)iLoopIndex + i) % (iLoopCount);
                List<ulong> currents = null;

                //Logger.LogErrorFormat("_AnsyInitAllTittle  eCurrent = {0} enumValue={1}", eCurrent, (TittleComeType)eCurrent);

                if (TittleBookManager.GetInstance().GetTittle((TittleComeType)eCurrent,out currents))
                {
                    for(int j = 0;  j < currents.Count; ++j,++iCount)
                    {
                        var current = m_akCachedObjects.Create(currents[j], new object[] { currents[j], m_goTittlePrefab, m_goTittleParent ,(TittleComeType)i,this ,true});
                        m_akCachedObjects.FilterObject(currents[j], new object[] { m_eTittleComeType });
                    }
                }

                if(TittleBookManager.GetInstance().GetTableTittle((TittleComeType)eCurrent, out currents))
                {
                    for (int j = 0; j < currents.Count; ++j, ++iCount)
                    {
                        if(!m_akCachedTableObjects.HasObject(currents[j]))
                        {
                            var current = m_akCachedTableObjects.Create(currents[j], new object[] { currents[j], m_goTittlePrefab, m_goTittleParent, (TittleComeType)i, this, false });
                            m_akCachedTableObjects.FilterObject(currents[j], new object[] { m_eTittleComeType });
                        }
                    }
                }
                if (!bToTitleMerge)
                    _SetDefaultSelected();
                _SortActiveObjects();

                if(!bToTitleMerge)
                {
                    if (ItemObject.selectedItem != null)
                    {
                        //Logger.LogErrorFormat("_AnsyInitAllTittle m_eTittleComeType AAAAA = {0}", ItemObject.selectedItem.TittleComeType);
                        m_akToggles[(int)ItemObject.selectedItem.TittleComeType].isOn = true;
                    }
                    else
                    {
                        //Logger.LogErrorFormat("_AnsyInitAllTittle m_eTittleComeType BBBBB = {0}", m_eTittleComeType);
                        m_akToggles[(int)m_eTittleComeType].isOn = true;
                    }
                    bToTitleMerge = true;
                }

                yield return new WaitForEndOfFrame();

                m_bHasCreated = true;
            }
        }
        #endregion

        [UIObject("tittles")]
        GameObject gotittles;
        [UIObject("FuncControls")]
        GameObject goFuncControls;

        void _OnTabChanged(TittleComeType target)
        {
            //Logger.LogErrorFormat("_OnTabChanged = {0}", target);
            if (m_eTittleComeType != target)
            {
                //Logger.LogErrorFormat("_OnTabChanged applyed = {0}", target);
                m_eTittleComeType = target;
                if(null != comState.Key)
                {
                    if(m_eTittleComeType != TittleComeType.TCT_MERGE)
                    {
                        comState.Key = "normal";
                    }
                    else
                    {
                        comState.Key = "merge";
                    }
                }

                if(m_eTittleComeType != TittleComeType.TCT_MERGE)
                {
                    _OnFilterChanged();
                    _SortActiveObjects();
                    bool bHasTitle = true;
                    if (!_CheckHasTittleType(target))
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("title_un_acquired"));
                        bHasTitle = false;
                    }
                    else
                    {
                        List<ItemObject> activeObjects = m_akCachedObjects.GetObjectListByFilter(new object[] { m_eTittleComeType });
                        List<ItemObject> anotherObjects = m_akCachedTableObjects.GetObjectListByFilter(new object[] { m_eTittleComeType });
                        activeObjects.InsertRange(activeObjects.Count, anotherObjects);
                        if (activeObjects.Count > 0)
                        {
                            activeObjects.Sort((x, y) =>
                            {
                                return x.itemTable.IdSequence - y.itemTable.IdSequence;
                            });

                            activeObjects[0].OnSelected();
                        }
                    }
                    gotittles.CustomActive(bHasTitle);
                    goFuncControls.CustomActive(bHasTitle);
                }
                else
                {
                    gotittles.CustomActive(true);
                    _UpdateMergeItems();
                    _UpdateMergeMaterials();
                }
            }
        }

        [UIControl("merge", typeof(ComUIListScript))]
        ComUIListScript comMergeTitles;
        bool m_bMergeTitlesInit = false;

        void _InitMergeItems()
        {
            if (null != comMergeTitles)
            {
                comMergeTitles.Initialize();
                comMergeTitles.onBindItem = (GameObject go) =>
                {
                    if (null != go)
                    {
                        return go.GetComponent<ComMergeTitle>();
                    }
                    return null;
                };
                comMergeTitles.onItemSelected = (ComUIListElementScript item) =>
                {
                    if (null != item)
                    {
                        ComMergeTitle comMergeTitle = item.gameObjectBindScript as ComMergeTitle;
                        if (null != comMergeTitle)
                        {
                            ComMergeTitle.Selected = comMergeTitle.Value;
                            _SetMergeTitleData(comMergeTitle.Value);
                        }
                    }
                };
                comMergeTitles.onItemVisiable = (ComUIListElementScript item) =>
                {
                    if (null != item && item.m_index >= 0 && item.m_index < TittleBookManager.GetInstance().MergeTitles.Count)
                    {
                        ComMergeTitle comMergeTitle = item.gameObjectBindScript as ComMergeTitle;
                        var visibleData = TittleBookManager.GetInstance().MergeTitles[item.m_index];
                        if (null != comMergeTitle && null != visibleData)
                        {
                            comMergeTitle.OnItemVisible(visibleData);
                        }
                    }
                };
            }
            m_bMergeTitlesInit = true;
        }

        void _InitMergeMaterials()
        {
            if(null != comCache)
            {
                comCache.onItemCreate = (GameObject go) =>
                {
                    if(null != go)
                    {
                        var script = go.GetComponent<ComItemSetting>();
                        go.CustomActive(true);
                        return script;
                    }
                    return null;
                };
                comCache.onItemVisible = (object script, object data)=>
                {
                    if(null != script && null != data)
                    {
                        var comSetting = script as ComItemSetting;
                        var comSettingData = data as ComItemSettingData;
                        if (null != comSetting && null != comSettingData)
                        {
                            comSetting.SetValueByTableData(comSettingData.iId, comSettingData.count, comSettingData.cost);
                        }
                    }
                };
                comCache.onItemRecycled = (object script)=>
                {
                    var comSetting = script as ComItemSetting;
                    if(null != comSetting)
                    {
                        comSetting.gameObject.CustomActive(false);
                    }
                };
            }
        }

        void _UpdateMergeItems()
        {
            if (null != comMergeTitles && m_bMergeTitlesInit)
            {
                comMergeTitles.SetElementAmount(TittleBookManager.GetInstance().MergeTitles.Count);
            }

            _TrySelectedDefaultMergeItem();
        }

        void _TrySelectedDefaultMergeItem()
        {
            var mergeTitles = TittleBookManager.GetInstance().MergeTitles;
            if(null != mergeTitles && mergeTitles.Count > 0)
            {
                if (null == ComMergeTitle.Selected)
                {
                    ComMergeTitle.Selected = mergeTitles[0];
                    _SetMergeTitleData(ComMergeTitle.Selected);
                    //Logger.LogErrorFormat("setelement 0 AAA");
                }
                else
                {
                    if (null != comMergeTitles)
                    {
                        bool bFind = false;
                        for (int i = 0; i < mergeTitles.Count; ++i)
                        {
                            if (null != mergeTitles[i] && mergeTitles[i].forgeItem == ComMergeTitle.Selected.forgeItem)
                            {
                                ComMergeTitle.Selected = mergeTitles[i];
                                _SetMergeTitleData(ComMergeTitle.Selected);
                                //Logger.LogErrorFormat("setelement {0} BBB", i);
                                bFind = true;
                                break;
                            }
                        }
                        if (!bFind)
                        {
                            ComMergeTitle.Selected = mergeTitles[0];
                            _SetMergeTitleData(ComMergeTitle.Selected);
                            //Logger.LogErrorFormat("setelement {0} CCC", 0);
                        }
                    }
                }
            }
        }

        void _UpdateMergeMaterials()
        {
            if(null != comCache)
            {
                if(null != ComMergeTitle.Selected)
                {
                    var materials = ComMergeTitle.Selected.materials;
                    List<object> objs = new List<object>();
                    for(int i = 0; i < materials.Count; ++i)
                    {
                        objs.Add(new ComItemSettingData
                        {
                            iId = materials[i].id,
                            count = 1,
                            cost = materials[i].count,
                        });
                    }
                    comCache.SetItems(objs);
                }
                else
                {
                    comCache.SetItems(null);
                }
            }
        }

        void _SetMergeTitleData(TitleMergeData data)
        {
            ItemData itemData = ItemDataManager.CreateItemDataFromTable(data.item.ID);
            _UpdateTitleDesc(itemData);
            _UpdateMergeMoney();
            _UpdateMergeMaterials();
        }

        void _OnFilterChanged()
        {
            m_akCachedObjects.Filter(new object[] { m_eTittleComeType });
            m_akCachedTableObjects.Filter(new object[] { m_eTittleComeType });
        }

        bool _CheckHasTittleType(TittleComeType target)
        {
            return m_akCachedObjects.HasObject(new object[] { target }) || m_akCachedTableObjects.HasObject(new object[] { target });
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float fDelta)
        {
            // var activedObjects = m_akCachedObjects.ActiveObjects.Values.ToList();
            // for(int i = 0; i < activedObjects.Count; ++i)
            // {
            //     activedObjects[i].OnUpdate();
            // }

            Dictionary<ulong, ItemObject> activeObj = m_akCachedObjects.ActiveObjects;
            if(null != activeObj)
            {
                Dictionary<ulong, ItemObject>.Enumerator it = activeObj.GetEnumerator();
                while(it.MoveNext())
                {
                    if (null != it.Current.Value)
                        it.Current.Value.OnUpdate();
                }
            }
        }

        #region uiEvent
        //[UIEventHandle("bottom/back/image")]
        void OnClickBack()
        {
            OnClickClose();
        }

        [UIEventHandle("tittle/close")]
        void OnClickClose()
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("FuncControls/Equipt")]
        void OnClickEquip()
        {
            if (ItemObject.selectedItem != null)
            {
                if (ItemObject.selectedItem.itemData.Packing)
                {
                    SystemNotifyManager.SystemNotify(2006,
                        () =>
                        {
                            ItemDataManager.GetInstance().UseItem(ItemObject.selectedItem.itemData);
                            ItemTipManager.GetInstance().CloseAll();
                        },
                        null,
                        ItemObject.selectedItem.itemData.GetColorName());
                }
                else
                {
                    ItemDataManager.GetInstance().UseItem(ItemObject.selectedItem.itemData);
                    ItemTipManager.GetInstance().CloseAll();
                }
            }
        }

        [UIEventHandle("FuncControls/View")]
        void OnClickView()
        {

        }

        [UIEventHandle("FuncControls/Magic")]
        void OnClickMagic()
        {

        }

        [UIEventHandle("FuncControls/Trade")]
        void OnClickTrade()
        {
            if (ItemObject.selectedItem != null)
            {
                _OnAuction(ItemObject.selectedItem.itemData, null);
            }
        }

        [UIEventHandle("FuncControls/LookUp")]
        void OnClickLookUp()
        {
            if(ItemObject.selectedItem != null)
            {
                var realItem = ItemDataManager.GetInstance().GetItem(ItemObject.selectedItem.itemData.GUID);
                List<TipFuncButon> funcs = new List<TipFuncButon>();
                if (realItem != null)
                {
                    // 装备
                    if(realItem.PackageType == EPackageType.Title)
                    {
                        TipFuncButon tempfunc = new TipFuncButon();
                        tempfunc.text = TR.Value("tip_wear");
                        tempfunc.callback = _OnUnWear;
                        funcs.Add(tempfunc);
                    }

                    // 卸下
                    if (realItem.PackageType == EPackageType.WearEquip)
                    {
                        TipFuncButon tempfunc = new TipFuncButon();
                        tempfunc.text = TR.Value("tip_takeoff");
                        tempfunc.callback = _OnUnWear;
                        funcs.Add(tempfunc);
                    }

                    // 分享
                    {
                        TipFuncButon tempFunc = new TipFuncButon();
                        tempFunc.text = TR.Value("tip_share");
                        tempFunc.callback = _OnShareClicked;
                        funcs.Add(tempFunc);
                    }

                    // 拍卖
                    if(TittleBookManager.GetInstance().CanTrade(realItem))
                    {
                        TipFuncButon tempFunc = new TipFuncButon();
                        tempFunc.text = TR.Value("tip_auction");
                        tempFunc.callback = _OnAuction;
                        funcs.Add(tempFunc);
                    }
                }

                ItemData compareItem = _GetCompareItem(ItemObject.selectedItem.itemData);
                if (compareItem != null)
                {
                    ItemTipManager.GetInstance().ShowTipWithCompareItem(ItemObject.selectedItem.itemData, compareItem, funcs);
                }
                else
                {
                    ItemTipManager.GetInstance().ShowTip(ItemObject.selectedItem.itemData, funcs);
                }
            }
        }

        ItemData _GetCompareItem(ItemData item)
        {
            ItemData compareItem = null;
            if (item != null && item.WillCanEquip())
            {
                List<ulong> guids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
                if (guids != null)
                {
                    for (int i = 0; i < guids.Count; ++i)
                    {
                        ItemData tempItem = ItemDataManager.GetInstance().GetItem(guids[i]);
                        if (
                            tempItem != null &&
                            tempItem.GUID != item.GUID &&
                            tempItem.IsWearSoltEqual(item)
                            )
                        {
                            compareItem = tempItem;
                            break;
                        }
                    }
                }
            }
            return compareItem;
        }

        protected void _OnUnWear(ItemData item, object data)
        {
            if (item != null)
            {
                ItemDataManager.GetInstance().UseItem(item);
                ItemTipManager.GetInstance().CloseAll();
            }
        }

        void _OnShareClicked(ItemData item, object data)
        {
            SystemNotifyManager.SysNotifyMsgBoxOK(TR.Value("common_not_opened"));
        }

        void _OnAuction(ItemData item, object data)
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<AuctionFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<AuctionFrame>();
            }

            OutComeAuctionData auctionData = new OutComeAuctionData();

            auctionData.eLabelType = AuctionPage.MyAuctionPage;
            auctionData.uiLinkId = ItemObject.selectedItem.itemData.GUID;

            ClientSystemManager.GetInstance().OpenFrame<AuctionFrame>(FrameLayer.Middle, auctionData);
        }
        #endregion
    }
    */
}