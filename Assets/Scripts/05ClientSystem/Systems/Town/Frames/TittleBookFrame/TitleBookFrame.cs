using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using Protocol;
using ProtoTable;
using GameClient;

namespace GameClient
{
    class TitleBookFrameData
    {
        public TittleComeType eTittleComeType = TittleComeType.TCT_SHOP;
    }
    struct ComTitleItemData
    {
        public ItemData itemData;
        public ProtoTable.ItemTable itemTable;
        public TittleComeType eType;
    }

    public enum TitleMergeMaterialNeedType
    {
        BeEquip,                    //装备在身上
        BeInUnSelectedEquipPlan,    //在未启用的装备方案中
        Enough,                       //足够
    }

    class TitleBookFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TitleBookFrame/TitleBookFrame";
        }

        public static void CommandOpen(object argv)
        {
            if(null == argv)
            {
                argv = new TitleBookFrameData();
            }
            ClientSystemManager.GetInstance().OpenFrame<TitleBookFrame>(FrameLayer.Middle,argv);
        }

        #region titleBookList
        [UIControl("tittles", typeof(ComUIListScript))]
        ComUIListScript comUIListScript;
        bool m_bInitialize = false;
        void _InitTitleBookList()
        {
            if(m_bInitialize)
            {
                return;
            }
            m_bInitialize = true;

            if(null != comUIListScript)
            {
                comUIListScript.Initialize();
                comUIListScript.onBindItem = (GameObject go) =>
                {
                    if(null != go)
                    {
                        return go.GetComponent<ComTitleItem>();
                    }
                    return null;
                };
                comUIListScript.onItemVisiable = (ComUIListElementScript item) =>
                {
                    if(null != item && item.m_index >= 0 && item.m_index < mDatas.Count)
                    {
                        var script = item.gameObjectBindScript as ComTitleItem;
                        if(null != script)
                        {
                            script.OnItemVisible(mDatas[item.m_index]);
                        }
                    }
                };
                comUIListScript.onItemChageDisplay = (ComUIListElementScript item, bool bSelected) =>
                {
                    var script = item.gameObjectBindScript as ComTitleItem;
                    if (null != script)
                    {
                        script.OnItemChangeDisplay(bSelected);
                    }
                };
                comUIListScript.onItemSelected = (ComUIListElementScript item) =>
                {
                    if (null != item && item.m_index >= 0 && item.m_index < mDatas.Count)
                    {
                        _OnSetTarget(mDatas[item.m_index]);
                    }
                };
            }
        }

        ComTitleItemData mDefault = new ComTitleItemData { itemTable = null, itemData = null};
        void _OnSetTarget(ComTitleItemData data)
        {
            if(data.itemTable == null)
            {
                mDataBinder.goTitles.CustomActive(false);
                mDataBinder.goFunction.CustomActive(false);
                mDataBinder.goLookUp.CustomActive(false);
                mDataBinder.goEquip.CustomActive(false);
                mDataBinder.btnTrade.CustomActive(false);
                return;
            }
            mDataBinder.goTitles.CustomActive(true);
            mDataBinder.goFunction.CustomActive(true);

            mDataBinder.goLookUp.CustomActive(data.itemTable != null);
            mDataBinder.goEquip.CustomActive(data.itemTable != null && data.itemData != null &&
                (
                !TittleBookManager.GetInstance().CanTrade(data.itemData) ||
                TittleBookManager.GetInstance().CanTrade(data.itemData) &&
                !TittleBookManager.GetInstance().HasBindedTitle(data.itemData.TableID))
                );
            mDataBinder.btnTrade.CustomActive(null != data.itemData && TittleBookManager.GetInstance().CanTrade(data.itemData));

            if (null != data.itemData)
            {
                if(null != mDataBinder.textEquipt)
                    mDataBinder.textEquipt.text = data.itemData.PackageType == EPackageType.WearEquip ? "卸下" : "穿戴";
            }

            if (null != mDataBinder.tittleDesc)
            {
                if (data.itemTable != null)
                    mDataBinder.tittleDesc.text = data.itemTable.ComeDesc;
                else
                    mDataBinder.tittleDesc.text = string.Empty;
            }

            var itemData = data.itemData;
            if(null == itemData && null != data.itemTable)
            {
                itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(data.itemTable.ID);
            }
            _UpdateTitleDesc(itemData);
        }

        Dictionary<string, List<GameObject>> m_akRecycleObject = new Dictionary<string, List<GameObject>>();
        void _UpdateTitleDesc(ItemData itemData)
        {
            if (null != itemData)
            {
                if(null != mDataBinder.tittleName)
                    mDataBinder.tittleName.text = itemData.GetColorName();

                if(null != mDataBinder.goTitleAttrParent)
                for (int i = 0; i < mDataBinder.goTitleAttrParent.transform.childCount; ++i)
                {
                    var transform = mDataBinder.goTitleAttrParent.transform.GetChild(i);
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

                    Utility.AttachTo(goNow, mDataBinder.goTitleAttrParent);
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
                                smartCom.SetText(string.Empty, string.Empty);
                            }
                        }
                    }
                }
            }
        }

        void _UpdateTitleBookListData(TittleComeType v)
        {
            mDatas.Clear();
            List<ulong> m_owned_guids = null;
            if (TittleBookManager.GetInstance().GetTittle(v, out m_owned_guids))
            {
                for (int i = 0; i < m_owned_guids.Count; ++i)
                {
                    var itemData = ItemDataManager.GetInstance().GetItem(m_owned_guids[i]);
                    if (null != itemData)
                    {
                        var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)itemData.TableID);
                        if (null != item)
                        {
                            mDatas.Add(new ComTitleItemData { itemData = itemData, itemTable = item, eType = v });
                        }
                    }
                }
            }
            List<ulong> m_unacquired_tableIds = null;
            if (TittleBookManager.GetInstance().GetTableTittle(v, out m_unacquired_tableIds))
            {
                for (int i = 0; i < m_unacquired_tableIds.Count; ++i)
                {
                    var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)m_unacquired_tableIds[i]);
                    if (null != item && item.OldTitle != 1)
                    {
                        mDatas.Add(new ComTitleItemData { itemTable = item ,eType = v});
                    }
                }
            }
            mDatas.Sort((x, y) =>
            {
                if(x.itemTable.IdSequence != y.itemTable.IdSequence)
                {
                    return x.itemTable.IdSequence - y.itemTable.IdSequence;
                }

                if(null != x.itemData && null != y.itemData)
                {
                    return x.itemData.GUID < y.itemData.GUID ? -1 : (x.itemData.GUID == y.itemData.GUID ? 0 : 1);
                }

                return 0;
            });
        }

        void _UpdateTitleBookList()
        {
            if(m_eType != TittleComeType.TCT_MERGE)
            {
                if(m_bInitialize)
                {
                    _UpdateTitleBookListData(m_eType);
                    if (null != comUIListScript)
                    {
                        comUIListScript.SetElementAmount(mDatas.Count);
                    }

                    if(mDatas.Count > 0)
                    {
                        int iIndex = comUIListScript.GetSelectedIndex();
                        if(iIndex < 0 || iIndex >= mDatas.Count)
                        {
                            iIndex = 0;
                        }
                        if(m_eLastType != m_eType)
                        {
                            iIndex = 0;
                            m_eLastType = m_eType;
                        }
                        comUIListScript.SelectElement(-1);
                        if (!comUIListScript.IsElementInScrollArea(iIndex))
                        {
                            comUIListScript.EnsureElementVisable(iIndex);
                        }
                        comUIListScript.SelectElement(iIndex);
                        var script = comUIListScript.GetElemenet(iIndex);
                        if(null == script)
                        {
                            _OnSetTarget(mDatas[iIndex]);
                        }
                    }
                    else
                    {
                        comUIListScript.SelectElement(-1);
                        if (m_eLastType != m_eType)
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("title_un_acquired"));
                            m_eLastType = m_eType;
                        }
                        _OnSetTarget(mDefault);
                    }
                }
            }
        }

        void _UnInitTitleBookList()
        {
            if(m_bInitialize)
            {
                if(null != comUIListScript)
                {
                    comUIListScript.onBindItem = null;
                    comUIListScript.onItemChageDisplay = null;
                    comUIListScript.onItemVisiable = null;
                    comUIListScript.onItemSelected = null;
                }
                m_bInitialize = false;
            }
            comUIListScript = null;
        }
        #endregion

        #region title_merge
        [UIControl("merge", typeof(ComUIListScript))]
        ComUIListScript comMergeTitles;
        [UIControl("merge/Horizen", typeof(ComCache))]
        ComCache comCache;
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
        }

        void _InitMergeMaterials()
        {
            if (null != comCache)
            {
                comCache.onItemCreate = (GameObject go) =>
                {
                    if (null != go)
                    {
                        var script = go.GetComponent<ComItemSetting>();
                        go.CustomActive(true);
                        return script;
                    }
                    return null;
                };
                comCache.onItemVisible = (object script, object data) =>
                {
                    if (null != script && null != data)
                    {
                        var comSetting = script as ComItemSetting;
                        var comSettingData = data as ComItemSettingData;
                        if (null != comSetting && null != comSettingData)
                        {
                            comSetting.SetValueByTableData(comSettingData.iId, comSettingData.count, comSettingData.cost);
                        }
                    }
                };
                comCache.onItemRecycled = (object script) =>
                {
                    var comSetting = script as ComItemSetting;
                    if (null != comSetting)
                    {
                        comSetting.gameObject.CustomActive(false);
                    }
                };
            }
        }
        void _SetMergeTitleData(TitleMergeData data)
        {
            ItemData itemData = ItemDataManager.CreateItemDataFromTable(data.item.ID);
            _UpdateTitleDesc(itemData);
            _UpdateMergeMoney();
            _UpdateMergeMaterials();
        }

        [UIControl("merge/btnMerge/horizen/Icon", typeof(Image))]
        Image moneyIcon;

        [UIControl("merge/btnMerge/horizen/Text", typeof(Text))]
        Text moneyCount;

        void _UpdateMergeMoney()
        {
            if(m_eType != TittleComeType.TCT_MERGE)
            {
                return;
            }

            if (null == ComMergeTitle.Selected)
            {
                moneyIcon.CustomActive(false);
                moneyCount.text = "合成";
            }
            else
            {
                bool bNeedMoney = ComMergeTitle.Selected.getMoneyCount() > 0;
                if (!bNeedMoney)
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

        void _UpdateMergeItems()
        {
            if(m_eType == TittleComeType.TCT_MERGE)
            {
                if (null != comMergeTitles && m_bMergeTitlesInit)
                {
                    comMergeTitles.SetElementAmount(TittleBookManager.GetInstance().MergeTitles.Count);
                }

                _TrySelectedDefaultMergeItem();
            }
        }

        void _UpdateMergeMaterials()
        {
            if (m_eType == TittleComeType.TCT_MERGE)
            {
                if (null != comCache)
                {
                    if (null != ComMergeTitle.Selected)
                    {
                        var materials = ComMergeTitle.Selected.materials;
                        List<object> objs = new List<object>();
                        for (int i = 0; i < materials.Count; ++i)
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
        }

        void _TrySelectedDefaultMergeItem()
        {
            var mergeTitles = TittleBookManager.GetInstance().MergeTitles;
            if (null != mergeTitles && mergeTitles.Count > 0)
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

        void _OnClickMerge()
        {
            if (null == ComMergeTitle.Selected)
            {
                Logger.LogErrorFormat("has no selected mergetitle!!");
                return;
            }

            if (!ComMergeTitle.checkMaterialEnough(ComMergeTitle.Selected, true))
            {
                //SystemNotifyManager.SysNotifyTextAnimation(string.Format("合成{0}所需材料不足!", ComMergeTitle.Selected.item.Name),CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                return;
            }

            if (!ComMergeTitle.checkMoneyEnough(ComMergeTitle.Selected))
            {
                ItemComeLink.OnLink(ComMergeTitle.Selected.getMoneyId(), ComMergeTitle.Selected.getMoneyCount() - ComMergeTitle.Selected.getOwnedMoneyCount(), true);
                return;
            }

            _OnConfirmToMerge();
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

                var titleMergeMaterialNeedType = CheckOwnerMaterialItem(arrCostInfos);
                if (titleMergeMaterialNeedType == TitleMergeMaterialNeedType.BeEquip)
                {
                    //道具正在穿戴，无法合成
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("title_merge_failed_for_equipted"));
                    return;
                }
                else if (titleMergeMaterialNeedType == TitleMergeMaterialNeedType.BeInUnSelectedEquipPlan)
                {
                    //道具在未启用的装备方案中，无法合成
                    var tipContent = TR.Value("Equip_Plan_Item_CanNot_Merge_Format",
                        EquipPlanDataManager.GetInstance().UnSelectedEquipPlanId);
                    SystemNotifyManager.SysNotifyFloatingEffect(tipContent);
                    return;
                }
                
                CostItemManager.GetInstance().TryCostMoneiesDefault(arrCostInfos, () =>
                {
                    frameMgr.OpenFrame<EquipForgeResultFrame>(FrameLayer.Middle, ComMergeTitle.Selected.item.ID);
                });
            }
        }

        //返回合成材料的可能情况(穿戴在身上，或者在未启用的装备方案中)
        private TitleMergeMaterialNeedType CheckOwnerMaterialItem(List<CostItemManager.CostInfo> infos)
        {
            //消耗材料不存在
            if (infos == null || infos.Count <= 0)
                return TitleMergeMaterialNeedType.Enough;

            //穿戴在身上的道具
            var equipItemGuidList = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
            List<int> equipTableIdList = new List<int>();
            if (equipItemGuidList != null && equipItemGuidList.Count > 0)
            {
                for (var i = 0; i < equipItemGuidList.Count; ++i)
                {
                    var equipItemGuid = equipItemGuidList[i];
                    if (equipItemGuid <= 0)
                        continue;

                    var equipItem = ItemDataManager.GetInstance().GetItem(equipItemGuid);
                    if (equipItem != null && equipItem.TableID > 0)
                        equipTableIdList.Add(equipItem.TableID);
                }
            }

            //未启用装备方案的道具
            var unSelectedEquipPlanItemGuidList = EquipPlanUtility.GetUnSelectedEquipPlanItemGuidList();

            //只在未启用的装备方案中的道具Id列表
            List<int> unSelectedEquipPlanTableIdList = new List<int>();
            if (unSelectedEquipPlanItemGuidList != null
                && unSelectedEquipPlanItemGuidList.Count > 0)
            {
                for (var i = 0; i < unSelectedEquipPlanItemGuidList.Count; i++)
                {
                    var unSelectedItemGuid = unSelectedEquipPlanItemGuidList[i];
                    if (unSelectedItemGuid <= 0)
                        continue;

                    //是否穿在身上
                    if (equipItemGuidList != null
                        && equipItemGuidList.Count > 0
                        && equipItemGuidList.Contains(unSelectedItemGuid) == true)
                        continue;

                    var unSelectedItem = ItemDataManager.GetInstance().GetItem(unSelectedItemGuid);
                    if (unSelectedItem != null)
                        unSelectedEquipPlanTableIdList.Add(unSelectedItem.TableID);
                }
            }

            for (int i = 0; i < infos.Count; ++i)
            {
                var info = infos[i];
                if (info == null)
                    continue;

                if (info.nCount <= 0)
                    continue;

                var itemId = info.nMoneyID;
                var ownedCount = ItemDataManager.GetInstance().GetOwnedItemCount(itemId);

                //需要的道具和拥有的刚好相同
                if (ownedCount == info.nCount)
                {
                    //存在道具穿在身上
                    if (equipTableIdList.Contains(itemId) == true)
                    {
                        return TitleMergeMaterialNeedType.BeEquip;
                    }
                    else if (unSelectedEquipPlanTableIdList.Contains(itemId) == true)
                    {
                        //存在道具在未启用的装备方案中
                        return TitleMergeMaterialNeedType.BeInUnSelectedEquipPlan;
                    }
                }
                else if (ownedCount == info.nCount + 1)
                {
                    //拥有的道具数量比需要的多1
                    //存在一个穿在身上，并且存在一个在未启用的装备方案中
                    if (equipTableIdList.Contains(itemId) == true
                        && unSelectedEquipPlanTableIdList.Contains(itemId) == true)
                    {
                        return TitleMergeMaterialNeedType.BeEquip;
                    }
                }
            }

            return TitleMergeMaterialNeedType.Enough;
        }

        #endregion

        #region tab_interface
        [UIControl("", typeof(ComTitleBookDataBinder))]
        ComTitleBookDataBinder mDataBinder;
        class TabItem
        {
            public Toggle toggle;
            public GameObject goCheckMark;
            public GameObject goRedPoint;
            public void Clear()
            {
                if(null != toggle)
                {
                    toggle.onValueChanged.RemoveAllListeners();
                    toggle = null;
                }
                goCheckMark = null;
                goRedPoint = null;
            }
        }
        TabItem[] mTabs = new TabItem[(int)TittleComeType.TCT_COUNT];
        string[] mTabNames = new string[(int)TittleComeType.TCT_COUNT] { "商城", "任务", "活动", "时限", "可交易", "称号合成" };
        void _InitTabs()
        {
            if(null != mDataBinder)
            {
                for(int i = 0; i < (int)TittleComeType.TCT_COUNT; ++i)
                {
                    if(null != mDataBinder.goTabPrefab)
                    {
                        GameObject goLocal = GameObject.Instantiate(mDataBinder.goTabPrefab);
                        if(null == goLocal)
                        {
                            continue;
                        }

                        Utility.AttachTo(goLocal, mDataBinder.goTabPrefab.transform.parent.gameObject);

                        goLocal.name = ((TittleComeType)i).ToString();

                        Text text = Utility.FindComponent<Text>(goLocal, "Label");
                        if(null != text)
                        {
                            text.text = mTabNames[i];
                        }

                        text = Utility.FindComponent<Text>(goLocal, "Checkmark/Label");
                        if (null != text)
                        {
                            text.text = mTabNames[i];
                        }

                        if(null == mTabs[i])
                        mTabs[i] = new TabItem();
                        mTabs[i].toggle = goLocal.GetComponent<Toggle>();
                        mTabs[i].goCheckMark = Utility.FindChild(goLocal, "Checkmark");
                        mTabs[i].goRedPoint = Utility.FindChild(goLocal, "tabMark");
                        mTabs[i].goRedPoint.CustomActive(false);

                        TittleComeType v = (TittleComeType)i;
                        mTabs[i].toggle.onValueChanged.AddListener((bool bValue) =>
                        {
                            _OnTabChanged(v, bValue);
                        });
                    }
                }

                mDataBinder.goTabPrefab.CustomActive(false);
            }
        }

        void _UnInitTabs()
        {
            for(int i = 0; i < mTabs.Length; ++i)
            {
                if(null != mTabs[i])
                {
                    mTabs[i].Clear();
                }
            }
        }

        void _SetTab(TittleComeType e)
        {
            if(e >= 0 && (int)e < mTabs.Length)
            {
                var tab = mTabs[(int)e];
                if(null != tab)
                {
                    for (int i = 0; i < mTabs.Length; ++i)
                    {
                        if (null != mTabs[i] && null != mTabs[i].toggle)
                        {
                            mTabs[i].toggle.onValueChanged.RemoveAllListeners();
                        }
                    }
                    tab.toggle.isOn = true;
                    for (int i = 0; i < mTabs.Length; ++i)
                    {
                        _OnTabChanged((TittleComeType)i, e == (TittleComeType)i);
                    }
                    for (int i = 0; i < mTabs.Length; ++i)
                    {
                        if (null != mTabs[i] && null != mTabs[i].toggle)
                        {
                            TittleComeType v = (TittleComeType)i;
                            mTabs[i].toggle.onValueChanged.AddListener((bool bValue) =>
                            {
                                _OnTabChanged(v, bValue);
                            });
                        }
                    }
                }
            }
        }
        TittleComeType m_eType = TittleComeType.TCT_COUNT;
        TittleComeType m_eLastType = TittleComeType.TCT_COUNT;
        List<ComTitleItemData> mDatas = new List<ComTitleItemData>(32);

        void _OnTabChanged(TittleComeType v,bool bValue)
        {
            if((int)v < 0 || (int)v >= mTabs.Length)
            {
                return;
            }

            TabItem data = mTabs[(int)v];
            if(null != data)
            {
                data.goCheckMark.CustomActive(bValue);
            }

            if(bValue)
            {
                m_eLastType = m_eType;
                m_eType = v;
                switch(v)
                {
                    case TittleComeType.TCT_SHOP:
                    case TittleComeType.TCT_MISSION:
                    case TittleComeType.TCT_ACTIVE:
                    case TittleComeType.TCT_TIMELIMITED:
                    case TittleComeType.TCT_TRADE:
                        {
                            if(null != comState)
                            {
                                comState.Key = mStatusNormal;
                            }
                            _InitTitleBookList();
                            _UpdateTitleBookList();
                        }
                        break;
                    case TittleComeType.TCT_MERGE:
                        {
                            if (null != comState)
                            {
                                comState.Key = mStatusMerge;
                            }

                            mDataBinder.goTitles.CustomActive(true);

                            if (!m_bMergeTitlesInit)
                            {
                                _InitMergeItems();
                                _InitMergeMaterials();
                                m_bMergeTitlesInit = true;
                            }

                            _UpdateMergeMoney();
                            _UpdateMergeItems();
                            _UpdateMergeMaterials();
                        }
                        break;
                }
            }
        }
        #endregion

        TitleBookFrameData mData = null;
        [UIControl("", typeof(StateController))]
        StateController comState;
        string mStatusNormal = "normal";
        string mStatusMerge = "merge";

        protected override void _OnOpenFrame()
        {
            mData = userData as TitleBookFrameData;
            if(null == mData)
            {
                mData = new TitleBookFrameData();
            }

            _AddButton("tittle/close", () =>
            {
            frameMgr.CloseFrame(this);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TitleBookCloseFrame);
            });
            _AddButton("merge/btnMerge", _OnClickMerge);

            _InitTabs();
            _UpdateTabRedPoint();
            _SetTab(mData.eTittleComeType);
            _RegisterEvent();
        }

        #region UIEvent
        [UIEventHandle("FuncControls/Equipt")]
        void OnClickEquip()
        {
            var selectedItem = _GetSelectedItemData(true);
            if (selectedItem != null)
            {
                if(selectedItem.PackageType == EPackageType.Storage || selectedItem.PackageType == EPackageType.RoleStorage)
                {
                    SystemNotifyManager.SystemNotify(10002);
                    return;
                }
                if (selectedItem.Packing)
                {
                    SystemNotifyManager.SystemNotify(2006,
                        () =>
                        {
                            ItemDataManager.GetInstance().UseItem(selectedItem);
                            ItemTipManager.GetInstance().CloseAll();
                        },
                        null,
                        selectedItem.GetColorName());
                }
                else
                {
                    ItemDataManager.GetInstance().UseItem(selectedItem);
                    ItemTipManager.GetInstance().CloseAll();
                }
            }
        }

        [UIEventHandle("FuncControls/Trade")]
        void OnClickTrade()
        {
            var itemData = _GetSelectedItemData(true);
            if(null != itemData)
            {
                if(itemData.PackageType == EPackageType.Storage || itemData.PackageType == EPackageType.RoleStorage)
                {
                    SystemNotifyManager.SystemNotify(10002);
                    return;
                }
                _OnAuction(itemData, null);
            }
        }

        ItemData _GetSelectedItemData(bool bReal)
        {
            if (null != comUIListScript)
            {
                if (m_bInitialize)
                {
                    int iIndex = comUIListScript.GetSelectedIndex();
                    if (iIndex >= 0 && iIndex < mDatas.Count)
                    {
                        if (null != mDatas[iIndex].itemData)
                        {
                            return mDatas[iIndex].itemData;
                        }
                        if(null != mDatas[iIndex].itemTable && !bReal)
                        {
                            return ItemDataManager.GetInstance().GetCommonItemTableDataByID(mDatas[iIndex].itemTable.ID);
                        }
                    }
                }
            }
            return null;
        }

        [UIEventHandle("FuncControls/LookUp")]
        void OnClickLookUp()
        {
            var selectedItem = _GetSelectedItemData(false);
            if (selectedItem != null)
            {
                var realItem = ItemDataManager.GetInstance().GetItem(selectedItem.GUID);
                List<TipFuncButon> funcs = new List<TipFuncButon>();
                if (realItem != null)
                {
                    // 装备
                    if (realItem.PackageType == EPackageType.Title)
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

                    //镶嵌
                    if (Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Forge) && !realItem.isInSidePack && !realItem.bLocked && !realItem.IsLease)
                    {
                        if (realItem.Type == ProtoTable.ItemTable.eType.FUCKTITTLE && realItem.SubType == (int)ItemTable.eSubType.TITLE && Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Bead))
                        {
                            if (realItem.DeadTimestamp == 0)
                            {
                                TipFuncButon tempfunc = new TipFuncButon()
                                {
                                    text = TR.Value("tip_BeadMosaic"),
                                    name = "BeadMosaic",
                                    callback = _OnForgeItem
                                };
                                funcs.Add(tempfunc);
                            }
                        }
                    }

                    // 分享
                    {
                        TipFuncButon tempFunc = new TipFuncButon();
                        tempFunc.text = TR.Value("tip_share");
                        tempFunc.callback = _OnShareClicked;
                        funcs.Add(tempFunc);
                    }

                    // 拍卖
                    if (TittleBookManager.GetInstance().CanTrade(realItem))
                    {
                        TipFuncButon tempFunc = new TipFuncButon();
                        tempFunc.text = TR.Value("tip_auction");
                        tempFunc.callback = _OnAuction;
                        funcs.Add(tempFunc);
                    }
                }

                ItemData compareItem = _GetCompareItem(selectedItem);
                if (compareItem != null)
                {
                    ItemTipManager.GetInstance().ShowTipWithCompareItem(selectedItem, compareItem, funcs);
                }
                else
                {
                    ItemTipManager.GetInstance().ShowTip(selectedItem, funcs);
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
            }
        }

        void _OnForgeItem(ItemData a_item, object a_data)
        {
            if (a_item != null)
            {
                SmithShopNewLinkData data = new SmithShopNewLinkData
                {
                    itemData = a_item,
                    iDefaultFirstTabId = (int)SmithShopNewTabType.SSNTT_BEAD
                };

                ClientSystemManager.GetInstance().CloseFrame<SmithShopNewFrame>(null, true);
                ClientSystemManager.GetInstance().OpenFrame<SmithShopNewFrame>(FrameLayer.Middle, data);
                ItemTipManager.GetInstance().CloseAll();
            }
        }

        void _OnShareClicked(ItemData item, object data)
        {
            ChatManager.GetInstance().ShareEquipment(item);
        }

        void _OnAuction(ItemData item, object data)
        {
            AuctionNewUtility.OpenAuctionNewFrame(item);
        }
        #endregion

        #region event
        void _OnAddTittle(ulong uid)
        {
            _UpdateTitleBookList();

            _UpdateMergeItems();
            _UpdateMergeMaterials();
        }

        void _OnUpdateTittle(ulong uid)
        {
            _UpdateTitleBookList();

            _UpdateMergeItems();
            _UpdateMergeMaterials();
        }

        void _OnRemoveTittle(ulong uid)
        {
            _UpdateTitleBookList();

            _UpdateMergeItems();
            _UpdateMergeMaterials();
        }

        void _OnRemoveTableTittle(ulong uid)
        {
            _UpdateTitleBookList();

            _UpdateMergeItems();
            _UpdateMergeMaterials();
        }

        void _OnAddTableTittle(ulong uid)
        {
            _UpdateTitleBookList();
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
            _UpdateMergeMoney();
        }
        #endregion

        #region redpoint
        void OnRedPointChanged(UIEvent uiEvent)
        {
            _UpdateTabRedPoint();
        }

        void _UpdateTabRedPoint()
        {
            for (int i = 0; i < (int)TittleComeType.TCT_COUNT; ++i)
            {
                if (mTabs[i] != null)
                {
                    bool bCheck = TittleBookManager.GetInstance().IsTittleTabMark((TittleComeType)i);
                    mTabs[i].goRedPoint.CustomActive(bCheck);
                }
            }
        }
        #endregion

        void _RegisterEvent()
        {
            TittleBookManager.GetInstance().onAddTittle += _OnAddTittle;
            TittleBookManager.GetInstance().onUpdateTittle += _OnUpdateTittle;
            TittleBookManager.GetInstance().onRemoveTittle += _OnRemoveTittle;
            TittleBookManager.GetInstance().onRemoveTableTittle += _OnRemoveTableTittle;
            TittleBookManager.GetInstance().onAddTableTittle += _OnAddTableTittle;

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPointChanged, OnRedPointChanged);

            ItemDataManager.GetInstance().onAddNewItem += _OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem += _OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem += _OnRemoveItem;
            PlayerBaseData.GetInstance().onMoneyChanged += _OnMoneyChanged;
        }

        void _UnRegisterEvent()
        {
            TittleBookManager.GetInstance().onAddTittle -= _OnAddTittle;
            TittleBookManager.GetInstance().onUpdateTittle -= _OnUpdateTittle;
            TittleBookManager.GetInstance().onRemoveTittle -= _OnRemoveTittle;
            TittleBookManager.GetInstance().onRemoveTableTittle -= _OnRemoveTableTittle;
            TittleBookManager.GetInstance().onAddTableTittle -= _OnAddTableTittle;

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPointChanged, OnRedPointChanged);

            ItemDataManager.GetInstance().onAddNewItem -= _OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem -= _OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem -= _OnRemoveItem;
            PlayerBaseData.GetInstance().onMoneyChanged -= _OnMoneyChanged;
        }

        protected override void _OnCloseFrame()
        {
            _UnRegisterEvent();

            _UnInitTitleBookList();
            _UnInitTabs();
            mData = null;
            m_akRecycleObject.Clear();

            if(m_bMergeTitlesInit)
            {
                if (null != comMergeTitles)
                {
                    comMergeTitles.onBindItem = null;
                    comMergeTitles.onItemSelected = null;
                    comMergeTitles.onItemVisiable = null;
                    comMergeTitles = null;
                }
                if(null != comCache)
                {
                    comCache.onItemVisible = null;
                    comCache.onItemCreate = null;
                    comCache.onItemRecycled = null;
                    comCache = null;
                }
                m_bMergeTitlesInit = false;
            }

            m_eType = TittleComeType.TCT_COUNT;
            m_eLastType = TittleComeType.TCT_COUNT;
        }
    }
}