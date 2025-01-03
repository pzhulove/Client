using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace GameClient
{
    class EquipForgeFrame : ClientFrame
    {
        [UIControl("Dropdown")]
        Dropdown m_dropJobSelect;

        [UIObject("TabGroup/Tabs")]
        GameObject m_objMainTypeRoot;

        [UIObject("TabGroup/Tabs/Tab")]
        GameObject m_objMainTypeTemplate;

        [UIObject("TabGroup/Page/TreeList/Viewport/Content")]
        GameObject m_objSubTypeRoot;

        [UIObject("TabGroup/Page/TreeList/Viewport/Content/Group")]
        GameObject m_objSubTypeTemplate;

        [UIObject("TabGroup/Page/TreeList/Viewport/Content/Group/SubTypes/SubType")]
        GameObject m_objForgeTemplate;

        [UIObject("TabGroup/Page/ForgeGroup/Materials/EquipRoot")]
        GameObject m_objEquipRoot;

        [UIObject("TabGroup/Page/ForgeGroup/Materials/RequireGroup")]
        GameObject m_objMaterialRoot;

        [UIObject("TabGroup/Page/ForgeGroup/Materials/RequireGroup/Template")]
        GameObject m_objMaterialTemplate;

        [UIControl("TabGroup/Page/ForgeGroup/Forge/Price/Icon")]
        Image m_imgPriceIcon;

        [UIControl("TabGroup/Page/ForgeGroup/Forge/Price/Count")]
        Text m_labPriceCount;

        [UIControl("OnlySuitable")]
        Toggle m_toggleOnlySuitable;

        [UIObject("TabGroup/Page/ForgeGroup/Detail/InfoView/Viewport/Content")]
        GameObject m_objTipRoot;

        [UIObject("TabGroup/Page/ForgeGroup/Detail/InfoView/Viewport/Content/Group")]
        GameObject m_groupPrefab;

        [UIObject("TabGroup/Page/ForgeGroup/Detail/InfoView/Viewport/Content/Group/HTwoLabels")]
        GameObject m_hTwoLabelsPrefab;

        [UIObject("TabGroup/Page/ForgeGroup/Detail/InfoView/Viewport/Content/Group/LeftLabel")]
        GameObject m_leftLabelPrefab;

        [UIObject("TabGroup/Page/ForgeGroup/Detail/InfoView/Viewport/Content/Group/RightLabel")]
        GameObject m_rightLabelPrefab;

        [UIObject("TabGroup/Page/ForgeGroup/Detail/InfoView/Viewport/Content/Line")]
        GameObject m_imagePrefab;

        [UIControl("TabGroup/Page/ForgeGroup/Detail/Title")]
        Image m_imgTipTitleBG;

        [UIControl("TabGroup/Page/ForgeGroup/Detail/Title/Text")]
        Text m_labTipTitleName;

        [UIControl("TabGroup/Page/ForgeGroup/Detail/OwnedCount")]
        Text m_labOwnedCount;

        [UIObject("TabGroup/Page/ForgeGroup/Detail")]
        GameObject m_objEquipDetail;

        int m_nSelectJobIdx = 0;
        int m_nSelectMainTypeIdx = 0;
        int m_nSelectSubTypeIdx = 0;
        int m_nSelectForgeIdx = 0;
        bool m_bResetSelectIdx = true;
        EquipForgeDataManager.ForgeInfo m_currForgeInfo = null;

        public static void CommandOpen(object argv)
        {
            ClientSystemManager.GetInstance().OpenFrame<EquipForgeFrame>(FrameLayer.Middle, argv);
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/EquipForge/EquipForge";
        }

        protected override void _OnOpenFrame()
        {
            _InitUI();
            _RegisterUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            _ClearUI();
            _UnRegisterUIEvent();
        }

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemCountChanged, _OnItemCountChanged);
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemCountChanged, _OnItemCountChanged);
        }

        void _InitUI()
        {
            EquipForgeDataManager.GetInstance().UpdateSuitable();
            EquipForgeDataManager.GetInstance().UpdateCanForge();

            m_objMainTypeTemplate.transform.SetParent(frame.transform, false);
            m_objMainTypeTemplate.SetActive(false);

            m_objSubTypeTemplate.transform.SetParent(frame.transform, false);
            m_objSubTypeTemplate.SetActive(false);

            m_objForgeTemplate.transform.SetParent(frame.transform, false);
            m_objForgeTemplate.SetActive(false);

            m_groupPrefab.transform.SetParent(frame.transform, false);
            m_groupPrefab.SetActive(false);

            m_hTwoLabelsPrefab.transform.SetParent(frame.transform, false);
            m_hTwoLabelsPrefab.SetActive(false);

            m_leftLabelPrefab.transform.SetParent(frame.transform, false);
            m_leftLabelPrefab.SetActive(false);

            m_rightLabelPrefab.transform.SetParent(frame.transform, false);
            m_rightLabelPrefab.SetActive(false);

            m_imagePrefab.transform.SetParent(frame.transform, false);
            m_imagePrefab.SetActive(false);

            m_toggleOnlySuitable.onValueChanged.RemoveAllListeners();
            m_toggleOnlySuitable.onValueChanged.AddListener(var =>
            {
                m_bResetSelectIdx = true;
                _InitJobSelect();
            });

            m_toggleOnlySuitable.onValueChanged.Invoke(true);
        }

        void _ClearUI()
        {
            m_currForgeInfo = null;
            m_nSelectJobIdx = 0;
            m_nSelectMainTypeIdx = 0;
            m_nSelectSubTypeIdx = 0;
            m_nSelectForgeIdx = 0;
        }

        void _InitJobSelect()
        {
            if (m_bResetSelectIdx)
            {
                m_nSelectJobIdx = _GetBestMatchedJobForgeInfo();
            }

            _ClearJobSelect();

            m_dropJobSelect.onValueChanged.RemoveAllListeners();
            m_dropJobSelect.ClearOptions();

            
            List<string> arrJobs = new List<string>();
            List<EquipForgeDataManager.TreeForgeInfo> infos = _GetJobMatchedForgeInfos();
            for (int i = 0; i < infos.Count; ++i)
            {
                arrJobs.Add(infos[i].strKey);
            }
            m_dropJobSelect.AddOptions(arrJobs);


            m_dropJobSelect.value = m_nSelectJobIdx;
            m_dropJobSelect.onValueChanged.AddListener(var =>
            {
                if (var >= 0 && var < infos.Count)
                {
                    m_nSelectJobIdx = var;
                    _InitMainTypeList(infos[var]);
                }
            });

            m_dropJobSelect.onValueChanged.Invoke(m_nSelectJobIdx);
        }

        void _ClearJobSelect()
        {
            _ClearMainTypeList();
            _ClearSubTypeList();
            _ClearForge();
            m_currForgeInfo = null;
        }

        int _GetBestMatchedJobForgeInfo()
        {
            List<int> arrJobs = new List<int>();
            arrJobs.Add(PlayerBaseData.GetInstance().JobTableID);
            arrJobs.AddRange(PlayerBaseData.GetInstance().ActiveJobTableIDs);
            ProtoTable.JobTable table = TableManager.instance.GetTableItem<ProtoTable.JobTable>(arrJobs[0]);
            arrJobs.AddRange(table.ToJob);

            List<EquipForgeDataManager.TreeForgeInfo> infos = _GetJobMatchedForgeInfos();
            for (int i = 0; i < arrJobs.Count; ++i)
            {
                if (arrJobs[i] == 0)
                {
                    continue;
                }
                for (int j = 0; j < infos.Count; ++j)
                {
                    if ((int)(infos[j].param) == arrJobs[i])
                    {
                        return j;
                    }
                }
            }
            
            return 0;
        }

        List<EquipForgeDataManager.TreeForgeInfo> _GetJobMatchedForgeInfos()
        {
            List<EquipForgeDataManager.TreeForgeInfo> result = new List<EquipForgeDataManager.TreeForgeInfo>();

            int nMyJobType = TableManager.instance.GetTableItem<ProtoTable.JobTable>(PlayerBaseData.GetInstance().JobTableID).JobType;
            List<EquipForgeDataManager.TreeForgeInfo> infos = EquipForgeDataManager.GetInstance().GetTreeForgeInfo().arrInfos;
            for (int i = 0; i < infos.Count; ++i)
            {
                int jobType = TableManager.instance.GetTableItem<ProtoTable.JobTable>((int)infos[i].param).JobType;
                if (jobType == nMyJobType)
                {
                    result.Add(infos[i]);
                }
            }

            return result;
        }

        void _InitMainTypeList(EquipForgeDataManager.TreeForgeInfo a_treeForgeInfo)
        {
            _ClearMainTypeList();

            List<Toggle> toggles = new List<Toggle>();
            bool isToggleInited = false;

            int nIndex = 0;
            for (int i = 0; i < a_treeForgeInfo.arrInfos.Count; ++i)
            {
                EquipForgeDataManager.TreeForgeInfo treeForgetInfo = a_treeForgeInfo.arrInfos[i];
                if (m_toggleOnlySuitable.isOn && treeForgetInfo.bSuitable == false)
                {
                    continue;
                }
                GameObject obj;
                if (nIndex < m_objMainTypeRoot.transform.childCount)
                {
                    obj = m_objMainTypeRoot.transform.GetChild(nIndex).gameObject;
                }
                else
                {
                    obj = GameObject.Instantiate(m_objMainTypeTemplate);
                    obj.transform.SetParent(m_objMainTypeRoot.transform, false);
                }
                int nToggleIdx = nIndex;
                nIndex++;
                obj.SetActive(true);

                Toggle toggle = obj.GetComponent<Toggle>();
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener(var =>
                {
                    if (isToggleInited)
                    {
                        if (var)
                        {
                            m_nSelectMainTypeIdx = nToggleIdx;
                            _InitSubTypeList(treeForgetInfo);
                        }
                    }
                });
                toggles.Add(toggle);

                Utility.GetComponetInChild<Text>(obj, "Label").text = treeForgetInfo.strKey;
                Utility.FindGameObject(obj, "RedPoint").SetActive(treeForgetInfo.bCanForge);

                obj.name = i.ToString();
            }

//             if (m_bResetSelectIdx)
//             {
//                 m_nSelectMainTypeIdx = 0;
//             }
            _InitToggleSelect(toggles, m_nSelectMainTypeIdx, ref isToggleInited);
        }

        void _ClearMainTypeList()
        {
            for (int i = 0; i < m_objMainTypeRoot.transform.childCount; ++i)
            {
                m_objMainTypeRoot.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        void _InitSubTypeList(EquipForgeDataManager.TreeForgeInfo a_treeForgeInfo)
        {
            _ClearSubTypeList();

            List<Toggle> toggles = new List<Toggle>();
            bool isToggleInited = false;

            int nIndex = 0;
            for (int i = 0; i < a_treeForgeInfo.arrInfos.Count; ++i)
            {
                EquipForgeDataManager.TreeForgeInfo treeForgetInfo = a_treeForgeInfo.arrInfos[i];
                if (m_toggleOnlySuitable.isOn && treeForgetInfo.bSuitable == false)
                {
                    continue;
                }
                GameObject obj;
                if (nIndex < m_objSubTypeRoot.transform.childCount)
                {
                    obj = m_objSubTypeRoot.transform.GetChild(nIndex).gameObject;
                }
                else
                {
                    obj = GameObject.Instantiate(m_objSubTypeTemplate);
                    obj.transform.SetParent(m_objSubTypeRoot.transform, false);
                }

                int nToggleIndex = nIndex;
                nIndex++;
                obj.SetActive(true);

                
                Toggle toggle = obj.GetComponent<Toggle>();
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener(var =>
                {
                    if (isToggleInited)
                    {
                        if (var)
                        {
                            m_nSelectSubTypeIdx = nToggleIndex;
                            _InitForgeList(Utility.FindGameObject(obj, "SubTypes"), treeForgetInfo);
                        }
                        else
                        {
                            _ClearForgeList(Utility.FindGameObject(obj, "SubTypes"));
                        }
                    }
                });
                toggles.Add(toggle);

                Utility.GetComponetInChild<Text>(obj, "MainType/Label").text = treeForgetInfo.strKey;
                Utility.FindGameObject(obj, "MainType/RedPoint").SetActive(treeForgetInfo.bCanForge);
            }

            if (m_bResetSelectIdx)
            {
                m_nSelectSubTypeIdx = 0;
            }
            _InitToggleSelect(toggles, m_nSelectSubTypeIdx, ref isToggleInited);
        }

        void _ClearSubTypeList()
        {
            for (int i = 0; i < m_objSubTypeRoot.transform.childCount; ++i)
            {
                m_objSubTypeRoot.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        void _InitForgeList(GameObject a_objRoot, EquipForgeDataManager.TreeForgeInfo a_treeForgeInfo)
        {
            _ClearForgeList(a_objRoot);

            List<Toggle> toggles = new List<Toggle>();
            bool isToggleInited = false;

            int nIndex = 0;
            for (int i = 0; i < a_treeForgeInfo.arrInfos.Count; ++i)
            {
                EquipForgeDataManager.TreeForgeInfo treeForgetInfo = a_treeForgeInfo.arrInfos[i];
                if (m_toggleOnlySuitable.isOn && treeForgetInfo.bSuitable == false)
                {
                    continue;
                }
                EquipForgeDataManager.ForgeInfo forgeInfo = treeForgetInfo.param as EquipForgeDataManager.ForgeInfo;
                GameObject obj;
                if (nIndex < a_objRoot.transform.childCount)
                {
                    obj = a_objRoot.transform.GetChild(nIndex).gameObject;
                }
                else
                {
                    obj = GameObject.Instantiate(m_objForgeTemplate);
                    obj.transform.SetParent(a_objRoot.transform, false);
                }
                int nToggleIndex = nIndex;
                nIndex++;
                obj.SetActive(true);

                Toggle toggle = obj.GetComponent<Toggle>();
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener(var =>
                {
                    if (isToggleInited)
                    {
                        if (var)
                        {
                            m_nSelectForgeIdx = nToggleIndex;
                            _InitForge(forgeInfo);
                        }
                    }
                });
                toggles.Add(toggle);

                //Utility.GetComponetInChild<Image>(obj, "Background").sprite = 
                //    AssetLoader.GetInstance().LoadRes(forgeInfo.itemData.GetQualityInfo().TitleBG2, typeof(Sprite)).obj as Sprite;
                Image bkImage = Utility.GetComponetInChild<Image>(obj, "Background");
                ETCImageLoader.LoadSprite(ref bkImage, forgeInfo.itemData.GetQualityInfo().TitleBG2);
                //Utility.GetComponetInChild<Image>(obj, "Icon").sprite =
                //    AssetLoader.GetInstance().LoadRes(forgeInfo.itemData.Icon, typeof(Sprite)).obj as Sprite;
                Image iconImage = Utility.GetComponetInChild<Image>(obj, "Icon");
                ETCImageLoader.LoadSprite(ref iconImage, forgeInfo.itemData.Icon);
                Utility.GetComponetInChild<Text>(obj, "Name").text = forgeInfo.itemData.Name;
                Utility.FindGameObject(obj, "Owned").SetActive(
                    ItemDataManager.GetInstance().GetOwnedItemCount(forgeInfo.itemData.TableID) > 0
                    );
                if (forgeInfo.itemData.LevelLimit > 0)
                {
                    Utility.GetComponetInChild<Text>(obj, "Level/Text").text = string.Format("Lv.{0}", forgeInfo.itemData.LevelLimit);
                }
                else
                {
                    Utility.GetComponetInChild<Text>(obj, "Level/Text").text = string.Empty;
                }
                
                Text labState = Utility.GetComponetInChild<Text>(obj, "State");
                EquipForgeDataManager.CheckForgeResult result = EquipForgeDataManager.GetInstance().CheckEquipCanForge(forgeInfo);
                if (result.eType == EquipForgeDataManager.CheckForgeResult.EType.CanForge)
                {
                    labState.text = TR.Value("color_green", TR.Value("equipforge_can_forge"));
                }
                else if (result.eType == EquipForgeDataManager.CheckForgeResult.EType.LessMaterial)
                {
                    labState.text = TR.Value("equipforge_less_material");
                }
                else if (result.eType == EquipForgeDataManager.CheckForgeResult.EType.LessPrice)
                {
                    ItemData price = result.userData as ItemData;
                    labState.text = TR.Value("equipforge_less_price", price.Name);
                }

                Utility.FindGameObject(obj, "RedPoint").SetActive(false);

                //obj.name = forgeInfo.itemData.TableID.ToString();
                obj.name = i.ToString();
            }

            if (m_bResetSelectIdx)
            {
                m_nSelectForgeIdx = 0;
            }
            _InitToggleSelect(toggles, m_nSelectForgeIdx, ref isToggleInited);
        }

        void _ClearForgeList(GameObject a_objRoot)
        {
            for (int i = 0; i < a_objRoot.transform.childCount; ++i)
            {
                a_objRoot.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        void _InitForge(EquipForgeDataManager.ForgeInfo a_forgeInfo)
        {
            _ClearForge();

            m_currForgeInfo = a_forgeInfo;
            Assert.IsNotNull(m_currForgeInfo);

            ComItem comEquipItem = m_objEquipRoot.GetComponentInChildren<ComItem>();
            if (comEquipItem == null)
            {
                comEquipItem = CreateComItem(m_objEquipRoot);
            }
            comEquipItem.Setup(a_forgeInfo.itemData, (var1, var2) =>
            {
                ItemData compareItem = null;
                {
                    compareItem = _GetCompareEquip(var2);
                    if (compareItem != null)
                    {
                        ItemTipManager.GetInstance().ShowTipWithCompareItem(var2, compareItem);
                    }
                    else
                    {
                        ItemTipManager.GetInstance().ShowTip(var2);
                    }
                }
            });

            for (int i = 0; i < a_forgeInfo.arrMaterials.Count; ++i)
            {
                ItemData material = a_forgeInfo.arrMaterials[i];
                GameObject obj = null;
                if (i < m_objMaterialRoot.transform.childCount)
                {
                    obj = m_objMaterialRoot.transform.GetChild(i).gameObject;
                }
                else
                {
                    obj = GameObject.Instantiate(m_objMaterialTemplate);
                    obj.transform.SetParent(m_objMaterialRoot.transform, false);
                }
                obj.SetActive(true);


                GameObject tempItemRoot = Utility.FindGameObject(obj, "Item");
                ComItem comItem = tempItemRoot.GetComponentInChildren<ComItem>();
                if (comItem == null)
                {
                    comItem = CreateComItem(tempItemRoot);
                }
                comItem.Setup(material, (var1, var2) =>
                {
                    if (ItemDataManager.GetInstance().GetOwnedItemCount(var2.TableID) >= var2.Count)
                    {
                        ItemComeLink.OnLink(var2.TableID, 0, false);
                    }
                    else
                    {
                        ItemComeLink.OnLink(var2.TableID, var2.Count, true);
                    }
                });
                comItem.SetCountFormatter(var => { return string.Empty; });

                Utility.GetComponetInChild<Text>(obj, "Name").text = material.GetColorName();
                int nOwnedCount = ItemDataManager.GetInstance().GetOwnedItemCount(material.TableID);
                int nNeedCount = material.Count;
                if (nOwnedCount >= nNeedCount)
                {
                    Utility.GetComponetInChild<Text>(obj, "Count").text =
                        string.Format("{0}/{1}", TR.Value("color_green", nOwnedCount), nNeedCount);
                }
                else
                {
                    Utility.GetComponetInChild<Text>(obj, "Count").text =
                        string.Format("{0}/{1}", TR.Value("color_red", nOwnedCount), nNeedCount);
                }
            }

            if (a_forgeInfo.arrPrices.Count > 0)
            {
                m_imgPriceIcon.gameObject.SetActive(true);
                m_labPriceCount.gameObject.SetActive(true);
                // m_imgPriceIcon.sprite = AssetLoader.GetInstance().LoadRes(a_forgeInfo.arrPrices[0].Icon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref m_imgPriceIcon, a_forgeInfo.arrPrices[0].Icon);

                int nOwnedCount = ItemDataManager.GetInstance().GetOwnedItemCount(a_forgeInfo.arrPrices[0].TableID);
                int nNeedCount = a_forgeInfo.arrPrices[0].Count;
                if (nOwnedCount >= nNeedCount)
                {
                    m_labPriceCount.text = TR.Value("color_green", nNeedCount);
                }
                else
                {
                    m_labPriceCount.text = TR.Value("color_red", nNeedCount);
                }
            }

            m_objEquipDetail.SetActive(true);
            _SetupTipContent(_GetEquipTipItemList(a_forgeInfo), m_objTipRoot);
            m_labOwnedCount.text = TR.Value("equipforge_equip_owned_count", 
                ItemDataManager.GetInstance().GetOwnedItemCount(a_forgeInfo.itemData.TableID));
        }

        void _ClearForge()
        {
            ComItem comEquipItem = m_objEquipRoot.GetComponentInChildren<ComItem>();
            if (comEquipItem == null)
            {
                comEquipItem = CreateComItem(m_objEquipRoot);
            }
            comEquipItem.Setup(null, null);

            for (int i = 0; i < 3; ++i)
            {
                GameObject obj = null;
                if (i < m_objMaterialRoot.transform.childCount)
                {
                    obj = m_objMaterialRoot.transform.GetChild(i).gameObject;
                }
                else
                {
                    obj = GameObject.Instantiate(m_objMaterialTemplate);
                    obj.transform.SetParent(m_objMaterialRoot.transform, false);
                }
                obj.SetActive(true);

                GameObject tempItemRoot = Utility.FindGameObject(obj, "Item");
                ComItem comItem = tempItemRoot.GetComponentInChildren<ComItem>();
                if (comItem == null)
                {
                    comItem = CreateComItem(tempItemRoot);
                }
                comItem.Setup(null, null);

                Utility.GetComponetInChild<Text>(obj, "Name").text = string.Empty;
                Utility.GetComponetInChild<Text>(obj, "Count").text = string.Empty;
            }

            m_imgPriceIcon.gameObject.SetActive(false);
            m_labPriceCount.gameObject.SetActive(false);

            m_objEquipDetail.SetActive(false);
        }

        ItemData _GetCompareEquip(ItemData item)
        {
            ItemData compareItem = null;
            if (item != null && item.IsOccupationFit())
            {
                List<ulong> guids = null;
                if (item.Type == ProtoTable.ItemTable.eType.EQUIP || item.Type == ProtoTable.ItemTable.eType.FUCKTITTLE)
                {
                    guids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
                }
                else if (item.Type == ProtoTable.ItemTable.eType.FASHION)
                {
                    guids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearFashion);
                }
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

        void _SetupTipContent(List<TipItem> tipItems, GameObject parentObj)
        {
            if (parentObj == null || tipItems == null || tipItems.Count <= 0)
            {
                return;
            }
            _ClearTipContent(parentObj);

            #region items

            for (int i = 0; i < tipItems.Count; ++i)
            {
                TipItem item = tipItems[i];
                switch (item.Type)
                {
                    case ETipItemType.ItemTitle:
                        {
                            TipItemItemTitle tempItem = item as TipItemItemTitle;
                            m_labTipTitleName.text = tempItem.itemData.Name;
                            // m_imgTipTitleBG.sprite = AssetLoader.instance.LoadRes(tempItem.itemData.GetQualityInfo().TitleBG, typeof(Sprite)).obj as Sprite;
                            ETCImageLoader.LoadSprite(ref m_imgTipTitleBG, tempItem.itemData.GetQualityInfo().TitleBG);

                            break;
                        }
                    case ETipItemType.HTwoLabels:
                        {
                            TipItemTwoLabels tempItem = item as TipItemTwoLabels;
                            GameObject obj = GameObject.Instantiate(m_hTwoLabelsPrefab);
                            {
                                Text leftLabel = Utility.FindGameObject(obj, "LeftLabel").GetComponent<Text>();
                                leftLabel.text = tempItem.LeftContent;
                            }
                            {
                                Text rightLabel = Utility.FindGameObject(obj, "RightLabel").GetComponent<Text>();
                                rightLabel.text = tempItem.RightContent;
                            }
                            obj.transform.SetParent(parentObj.transform, false);
                            obj.SetActive(true);
                            break;
                        }
                    case ETipItemType.LeftLabel:
                        {
                            TipItemLeftLabel tempItem = item as TipItemLeftLabel;
                            GameObject obj = GameObject.Instantiate(m_leftLabelPrefab);
                            {
                                Text label = obj.GetComponent<Text>();
                                label.text = tempItem.Content;
                            }
                            obj.transform.SetParent(parentObj.transform, false);
                            obj.SetActive(true);
                            break;
                        }
                    case ETipItemType.RightLabel:
                        {
                            TipItemRightLabel tempItem = item as TipItemRightLabel;
                            GameObject obj = GameObject.Instantiate(m_rightLabelPrefab);
                            {
                                Text label = obj.GetComponent<Text>();
                                label.text = tempItem.Content;
                            }
                            obj.transform.SetParent(parentObj.transform, false);
                            obj.SetActive(true);
                            break;
                        }
                    case ETipItemType.Image:
                        {
                            GameObject obj = GameObject.Instantiate(m_imagePrefab);
                            obj.transform.SetParent(parentObj.transform, false);
                            obj.SetActive(true);
                            break;
                        }
                    case ETipItemType.Group:
                        {
                            TipItemGroup tempItem = item as TipItemGroup;
                            GameObject obj = GameObject.Instantiate(m_groupPrefab);
                            obj.transform.SetParent(parentObj.transform, false);
                            obj.SetActive(true);

                            _SetupTipContent(tempItem.itemList, obj);
                            break;
                        }
                    default: break;
                }
            }
            #endregion
        }

        void _ClearTipContent(GameObject parentObj)
        {
            if (parentObj == null)
            {
                return;
            }

            for (int i = 0; i < parentObj.transform.childCount; i++)
            {
                GameObject go = parentObj.transform.GetChild(i).gameObject;
                GameObject.Destroy(go);
            }
        }

        List<TipItem> _GetEquipTipItemList(EquipForgeDataManager.ForgeInfo a_forgeInfo)
        {
            List<TipItem> tipItems = new List<TipItem>();

            #region title
            tipItems.Add(new TipItemItemTitle(a_forgeInfo.itemData, string.Empty));
            #endregion

            #region base
            {
                List<string> leftDescs = new List<string>();

                _TryAddDesc(leftDescs, _GetLevelLimitDesc(a_forgeInfo.itemData));
                _TryAddDesc(leftDescs, TR.Value("equipforge_tip_equip_type", a_forgeInfo.itemData.GetSubTypeDesc()));
                if (a_forgeInfo.itemData.SubType == (int)ProtoTable.ItemTable.eSubType.WEAPON)
                {
                    _TryAddDesc(leftDescs, TR.Value("equipforge_tip_weapon_type", a_forgeInfo.itemData.GetThirdTypeDesc()));
                }
                else
                {
                    _TryAddDesc(leftDescs, TR.Value("equipforge_tip_armor_type", a_forgeInfo.itemData.GetThirdTypeDesc()));
                }
                _TryAddDesc(leftDescs, _GetOccupationLimitDesc(a_forgeInfo));

                _TryShowDescsOnLeftSide(tipItems, leftDescs);
            }
            #endregion

            #region base attr
            {
                List<string> leftDescs = new List<string>();
                if (a_forgeInfo.itemData.SubType == (int)ProtoTable.ItemTable.eSubType.WEAPON)
                {
                    _TryAddDesc(leftDescs, a_forgeInfo.itemData.BaseProp.GetPropFormatStr(EEquipProp.PhysicsAttack));
                    _TryAddDesc(leftDescs, a_forgeInfo.itemData.BaseProp.GetPropFormatStr(EEquipProp.MagicAttack));
                }
                else if (
                    a_forgeInfo.itemData.SubType == (int)ProtoTable.ItemTable.eSubType.HEAD ||
                    a_forgeInfo.itemData.SubType == (int)ProtoTable.ItemTable.eSubType.CHEST ||
                    a_forgeInfo.itemData.SubType == (int)ProtoTable.ItemTable.eSubType.BELT ||
                    a_forgeInfo.itemData.SubType == (int)ProtoTable.ItemTable.eSubType.LEG ||
                    a_forgeInfo.itemData.SubType == (int)ProtoTable.ItemTable.eSubType.BOOT
                    )
                {
                    _TryAddDesc(leftDescs, a_forgeInfo.itemData.BaseProp.GetPropFormatStr(EEquipProp.PhysicsDefense));
                }
                else if (
                    a_forgeInfo.itemData.SubType == (int)ProtoTable.ItemTable.eSubType.RING ||
                    a_forgeInfo.itemData.SubType == (int)ProtoTable.ItemTable.eSubType.NECKLASE ||
                    a_forgeInfo.itemData.SubType == (int)ProtoTable.ItemTable.eSubType.BRACELET
                    )
                {
                    _TryAddDesc(leftDescs, a_forgeInfo.itemData.BaseProp.GetPropFormatStr(EEquipProp.MagicDefense));
                }

                _TryShowDescsOnLeftSide(tipItems, leftDescs);
            }
            #endregion

            #region strengthen
            _TryShowDescsOnLeftSide(tipItems, a_forgeInfo.itemData.GetStrengthenDescs());
            #endregion

            #region four attr
            _TryShowDescsOnLeftSide(tipItems, a_forgeInfo.itemData.GetFourAttrDescs());
            #endregion

            #region weapon attr
            {
                List<string> descs = new List<string>();
                _TryAddDesc(descs, a_forgeInfo.itemData.GetWeaponAttackSpeedDesc());
                _TryAddDescs(descs, a_forgeInfo.itemData.GetSkillMPAndCDDescs());
                _TryShowDescsOnLeftSide(tipItems, descs);
            }
            #endregion

            #region magic attr
            _TryShowDescOnLeftSide(tipItems, a_forgeInfo.itemData.GetMagicDescs());
            #endregion

            #region bead attr
            _TryShowDescOnLeftSide(tipItems, a_forgeInfo.itemData.GetBeadDescs());
            #endregion

            #region random attr
            _TryShowDescsOnLeftSide(tipItems, a_forgeInfo.itemData.GetRandomAttrDescs());
            #endregion

            #region additional attr
            _TryShowDescsOnLeftSide(tipItems, a_forgeInfo.itemData.GetAttachAttrDescs());
            #endregion

            #region complex attr
            _TryShowDescsOnLeftSide(tipItems, a_forgeInfo.itemData.GetComplexAttrDescs());
            #endregion

            #region master attr
            _TryShowDescsOnLeftSide(tipItems, a_forgeInfo.itemData.GetMasterAttrDescs());
            #endregion

            #region suit attr
            List<TipItem> arrSuitItems = _GetTipSuitItemList(a_forgeInfo.itemData, EquipSuitDataManager.GetInstance().GetSelfEquipSuitObj(a_forgeInfo.itemData.SuitID));
            if (arrSuitItems != null)
            {
                tipItems.AddRange(arrSuitItems);
            }
            #endregion

            #region dead timestamp description
            _TryShowDescOnLeftSide(tipItems, a_forgeInfo.itemData.GetDeadTimestampDesc());
            #endregion

            #region Interesting description
            _TryShowDescOnLeftSide(tipItems, a_forgeInfo.itemData.GetDescription());
            #endregion

            #region source description
            _TryShowDescOnLeftSide(tipItems, a_forgeInfo.itemData.GetSourceDescription());
            #endregion

            return tipItems;
        }

        string _GetLevelLimitDesc(ItemData a_itemData)
        {
            if (a_itemData.LevelLimit > 0)
            {
                string color_format = a_itemData.LevelLimit <= PlayerBaseData.GetInstance().Level ? "tip_color_good" : "tip_color_bad";
                return TR.Value("equipforge_tip_level_limit", TR.Value(color_format, a_itemData.LevelLimit));
            }
            else
            {
                return TR.Value("equipforge_tip_level_limit", 0);
            }
        }
        
        string _GetOccupationLimitDesc(EquipForgeDataManager.ForgeInfo a_info)
        {
            string desc = "";
            if (a_info.arrRecommendJobs.Count > 0)
            {
                bool bOccupationFit = false;
                for (int i = 0; i < a_info.arrRecommendJobs.Count; ++i)
                {
                    int nJobID = a_info.arrRecommendJobs[i];
                    if (PlayerBaseData.GetInstance().ActiveJobTableIDs.Contains(nJobID))
                    {
                        bOccupationFit = true;
                    }
                    ProtoTable.JobTable table = TableManager.instance.GetTableItem<ProtoTable.JobTable>(nJobID);
                    if (table != null)
                    {
                        if (string.IsNullOrEmpty(desc) == false)
                        {
                            desc += "、";
                        }
                        desc += table.Name;
                    }
                }
                string color_format = bOccupationFit ? "tip_color_good" : "tip_color_bad";
                desc = TR.Value("equipforge_tip_job_limit", TR.Value(color_format, desc));
            }
            else
            {
                desc = TR.Value("equipforge_tip_job_limit", TR.Value("tip_color_good", TR.Value("equipforge_tip_all_job")));
            }
            return desc;
        }

        void _TryAddDesc(List<string> a_descs, string a_desc)
        {
            if (string.IsNullOrEmpty(a_desc) == false)
            {
                a_descs.Add(a_desc);
            }
        }

        void _TryAddDescs(List<string> a_targetDescs, List<string> a_sourceDescs)
        {
            if (a_sourceDescs != null && a_sourceDescs.Count > 0)
            {
                a_targetDescs.AddRange(a_sourceDescs);
            }
        }

        void _TryShowDescOnLeftSide(List<TipItem> a_tipItems, string a_desc, bool a_bNeedLine = true)
        {
            if (a_tipItems != null && string.IsNullOrEmpty(a_desc) == false)
            {
                if (a_tipItems.Count > 0 && a_bNeedLine)
                {
                    a_tipItems.Add(new TipItemImage());
                }

                TipItemGroup itemGroup = new TipItemGroup();
                itemGroup.itemList.Add(new TipItemLeftLabel(a_desc));
                a_tipItems.Add(itemGroup);
            }
        }

        void _TryShowDescsOnLeftSide(List<TipItem> a_tipItems, List<string> a_descs)
        {
            if (a_tipItems != null && a_descs != null && a_descs.Count > 0)
            {
                if (a_tipItems.Count > 0)
                {
                    a_tipItems.Add(new TipItemImage());
                }

                TipItemGroup itemGroup = new TipItemGroup();
                for (int i = 0; i < a_descs.Count; ++i)
                {
                    itemGroup.itemList.Add(new TipItemLeftLabel(a_descs[i]));
                }
                a_tipItems.Add(itemGroup);
            }
        }

        List<TipItem> _GetTipSuitItemList(ItemData item, EquipSuitObj suitObj)
        {
            if (item == null)
            {
                return null;
            }
            
            if (suitObj == null)
            {
                return null;
            }

            List<TipItem> tipItems = new List<TipItem>();
            tipItems.Add(new TipItemImage());

            #region name
            {
                TipItemGroup itemGroup = new TipItemGroup();
                string temp;

                temp = TR.Value("color_green", string.Format("[{0}]", suitObj.equipSuitRes.name));
                if (string.IsNullOrEmpty(temp) == false)
                {
                    itemGroup.itemList.Add(new TipItemLeftLabel(temp));
                }
                tipItems.Add(itemGroup);
            }
            #endregion

            #region suit equips
            {
                //tipItems.Add(new TipItemImage());

                TipItemGroup itemGroup = new TipItemGroup();
                string temp = "";

                for (int i = 0; i < suitObj.equipSuitRes.equips.Count; ++i)
                {
                    int equipID = suitObj.equipSuitRes.equips[i];
                    ItemData equip = ItemDataManager.GetInstance().GetCommonItemTableDataByID(equipID);
                    if (suitObj.IsEquipActive(equip))
                    {
                        temp = TR.Value("color_green", equip.Name);
                    }
                    else
                    {
                        temp = TR.Value("color_grey", equip.Name);
                    }
                    itemGroup.itemList.Add(new TipItemLeftLabel(temp));
                }
                tipItems.Add(itemGroup);
            }
            #endregion

            #region suit effect
            {
                //tipItems.Add(new TipItemImage());

                TipItemGroup itemGroup = new TipItemGroup();
                string temp = "";

                var iter = suitObj.equipSuitRes.props.GetEnumerator();
                while (iter.MoveNext())
                {
                    string strDesc = string.Empty;
                    bool bActive = suitObj.wearedEquipIDs.Count >= iter.Current.Key;
                    string title_color_format = bActive ? "color_green" : "color_half_grey";
                    //temp = TR.Value(title_color_format, TR.Value("tip_suit_effect", iter.Current.Key));
                    //itemGroup.itemList.Add(new TipItemLeftLabel(temp));
                    strDesc = TR.Value(title_color_format, TR.Value("tip_suit_effect", iter.Current.Key));

                    #region baseProp
                    {
                        List<string> strList = iter.Current.Value.GetPropsFormatStr();
                        if (strList != null)
                        {
                            string content_color_format = bActive ? "color_blue" : "color_half_grey";
                            for (int i = 0; i < strList.Count; ++i)
                            {
                                //temp = TR.Value(content_color_format, strList[i]);
                                //itemGroup.itemList.Add(new TipItemLeftLabel(temp));
                                strDesc += "\n";
                                strDesc += TR.Value(content_color_format, strList[i]);
                            }
                        }
                    }
                    #endregion

                    #region buffSkill
                    {
                        List<EquipProp.BuffSkillInfo> buffSkillInfos = iter.Current.Value.GetBuffSkillInfos();
                        if (buffSkillInfos != null)
                        {
                            for (int i = 0; i < buffSkillInfos.Count; ++i)
                            {
                                EquipProp.BuffSkillInfo buffSkillInfo = buffSkillInfos[i];

                                string job_color_format;
                                if (_IsJobMatch(buffSkillInfo.jobID))
                                {
                                    if (bActive)
                                    {
                                        job_color_format = "color_orange";
                                    }
                                    else
                                    {
                                        job_color_format = "color_half_grey";
                                    }
                                }
                                else
                                {
                                    job_color_format = "color_grey";
                                }
                                //temp = TR.Value(job_color_format, buffSkillInfo.jobName);
                                //itemGroup.itemList.Add(new TipItemLeftLabel(temp));
                                strDesc += "\n";
                                strDesc += TR.Value(job_color_format, string.Format("[{0}]", buffSkillInfo.jobName));

                                string color_format;
                                if (_IsJobMatch(buffSkillInfo.jobID))
                                {
                                    if (bActive)
                                    {
                                        color_format = "color_blue";
                                    }
                                    else
                                    {
                                        color_format = "color_half_grey";
                                    }
                                }
                                else
                                {
                                    color_format = "color_grey";
                                }
                                for (int j = 0; j < buffSkillInfo.skillDescs.Count; ++j)
                                {
                                    //temp = TR.Value(color_format, buffSkillInfo.skillDescs[j]);
                                    //itemGroup.itemList.Add(new TipItemLeftLabel(temp));
                                    strDesc += "\n";
                                    strDesc += TR.Value(color_format, buffSkillInfo.skillDescs[j]);
                                }
                            }
                        }
                    }
                    #endregion

                    #region buffOther
                    {
                        string content_color_format = bActive ? "color_blue" : "color_half_grey";
                        List<string> strList = iter.Current.Value.GetBuffCommonDescs();
                        if (strList != null)
                        {
                            for (int i = 0; i < strList.Count; ++i)
                            {
                                //temp = TR.Value(content_color_format, strList[i]);
                                //itemGroup.itemList.Add(new TipItemLeftLabel(temp));

                                strDesc += "\n";
                                strDesc += TR.Value(content_color_format, strList[i]);
                            }
                        }
                        if (string.IsNullOrEmpty(iter.Current.Value.attachBuffDesc) == false)
                        {
                            //temp = TR.Value(content_color_format, iter.Current.Value.attachBuffDesc);
                            //itemGroup.itemList.Add(new TipItemLeftLabel(temp));
                            strDesc += "\n";
                            strDesc += TR.Value(content_color_format, iter.Current.Value.attachBuffDesc);
                        }
                    }
                    #endregion

                    #region mechanism
                    {
                        string content_color_format = bActive ? "color_blue" : "color_half_grey";
                        List<string> strList = iter.Current.Value.GetMechanismDescs();
                        if (strList != null)
                        {
                            for (int i = 0; i < strList.Count; ++i)
                            {
                                //temp = TR.Value(content_color_format, strList[i]);
                                //itemGroup.itemList.Add(new TipItemLeftLabel(temp));
                                strDesc += "\n";
                                strDesc += TR.Value(content_color_format, strList[i]);
                            }
                        }
                        if (string.IsNullOrEmpty(iter.Current.Value.attachMechanismDesc) == false)
                        {
                            //temp = TR.Value(content_color_format, iter.Current.Value.attachMechanismDesc);
                            //itemGroup.itemList.Add(new TipItemLeftLabel(temp));
                            strDesc += "\n";
                            strDesc += TR.Value(content_color_format, iter.Current.Value.attachMechanismDesc);
                        }
                    }
                    #endregion

                    itemGroup.itemList.Add(new TipItemLeftLabel(strDesc));
                }

                tipItems.Add(itemGroup);
            }
            #endregion

            return tipItems;
        }

        bool _IsJobMatch(int jobID)
        {
            return PlayerBaseData.GetInstance().ActiveJobTableIDs.Contains(jobID);
        }

        void _InitToggleSelect(List<Toggle> a_arrToggles, int a_nSelectToggleIdx, ref bool a_bBlockSignal)
        {
            if (a_arrToggles == null)
            {
                return;
            }

            if (a_nSelectToggleIdx < 0 || a_nSelectToggleIdx >= a_arrToggles.Count)
            {
                a_nSelectToggleIdx = 0;
            }

            for (int i = 0; i < a_arrToggles.Count; ++i)
            {
                if (i == a_nSelectToggleIdx)
                {
                    a_arrToggles[i].isOn = true;
                }
                else
                {
                    a_arrToggles[i].isOn = false;
                }
            }

            a_bBlockSignal = true;
            for (int i = 0; i < a_arrToggles.Count; ++i)
            {
                if (i == a_nSelectToggleIdx)
                {
                    a_arrToggles[i].onValueChanged.Invoke(true);
                }
                else
                {
                    a_arrToggles[i].onValueChanged.Invoke(false);
                }
            }
        }

        void _OnItemCountChanged(UIEvent a_event)
        {
            EquipForgeDataManager.GetInstance().UpdateCanForge();
            m_bResetSelectIdx = false;
            _InitJobSelect();
            m_bResetSelectIdx = true;
        }

        [UIEventHandle("TabGroup/Page/ForgeGroup/Forge")]
        void _OnForgeClicked()
        {
            if (m_currForgeInfo != null)
            {
                //EquipForgeDataManager.CheckForgeResult result = EquipForgeDataManager.GetInstance().CheckEquipCanForge(m_currForgeInfo);
                //if (result.eType == EquipForgeDataManager.CheckForgeResult.EType.CanForge)
                //{
                //    frameMgr.OpenFrame<EquipForgeResultFrame>(FrameLayer.Middle, m_currForgeInfo.itemData.TableID);
                //}
                //else
                //{
                //    ItemData item = result.userData as ItemData;
                //    Utility.CostInfo costInfo = new Utility.CostInfo();
                //    costInfo.nMoneyID = item.TableID;
                //    costInfo.nCount = item.Count;
                //    Utility.TryCostMoneyDefault(costInfo, null);
                //}
                //else if (result.eType == EquipForgeDataManager.CheckForgeResult.EType.LessMaterial)
                //{
                //    ItemData material = result.userData as ItemData;
                //    ItemComeLink.OnLink(material.TableID, material.Count, true);
                //}
                //else if (result.eType == EquipForgeDataManager.CheckForgeResult.EType.LessPrice)
                //{
                //    ItemData price = result.userData as ItemData;
                //    ItemComeLink.OnLink(price.TableID, price.Count, true);
                //}

                List<CostItemManager.CostInfo> arrCostInfos = new List<CostItemManager.CostInfo>();
                for (int i = 0; i < m_currForgeInfo.arrMaterials.Count; ++i)
                {
                    CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();
                    costInfo.nMoneyID = m_currForgeInfo.arrMaterials[i].TableID;
                    costInfo.nCount = m_currForgeInfo.arrMaterials[i].Count;
                    arrCostInfos.Add(costInfo);
                }
                for (int i = 0; i < m_currForgeInfo.arrPrices.Count; ++i)
                {
                    CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();
                    costInfo.nMoneyID = m_currForgeInfo.arrPrices[i].TableID;
                    costInfo.nCount = m_currForgeInfo.arrPrices[i].Count;
                    arrCostInfos.Add(costInfo);
                }
                CostItemManager.GetInstance().TryCostMoneiesDefault(arrCostInfos, () =>
                {
                    frameMgr.OpenFrame<EquipForgeResultFrame>(FrameLayer.Middle, m_currForgeInfo.itemData.TableID);
                });
            }
        }

        [UIEventHandle("BG/Title/Close")]
        void _OnCloseCliecked()
        {
            frameMgr.CloseFrame(this);
        }
    }
}
