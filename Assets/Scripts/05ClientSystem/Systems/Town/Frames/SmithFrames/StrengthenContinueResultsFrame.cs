using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ActivityLimitTime;
using StrengthOperateResult = Utility.StrengthOperateResult;

namespace GameClient
{
    class StrengthenContinueResultsFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/StrengthenContinueResultsFrame";
        }

        Text m_kTitle;
        GameObject m_goParent;
        GameObject m_goPrefabs; 
        class CachedItemDescObject : CachedObject
        {
            static string ms_succeed = "tip_strengthen_continue_ok";
            static string ms_failed = "tip_strengthen_continue_failed";
            static string[] ms_tr_values_failed = new string[7]
            {
                "tip_attr_ignore_def_physic_attack_diff_failed",
                "tip_attr_ignore_def_magic_attack_diff_failed",
                "tip_attr_ignore_atk_IngoreIndependence_def_diff_failed",
                "tip_attr_ignore_atk_physics_def_diff_failed",
                "tip_attr_ignore_atk_magic_def_diff_failed",
                "tip_attr_ignore_atk_physics_def_rate_diff_failed",
                "tip_attr_ignore_atk_magic_def_rate_diff_failed",
            };
            static string[] ms_tr_values_ok = new string[7]
            {
                "tip_attr_ignore_def_physic_attack_diff_ok",
                "tip_attr_ignore_def_magic_attack_diff_ok",
                "tip_attr_ignore_atk_IngoreIndependence_def_diff_ok",
                "tip_attr_ignore_atk_physics_def_diff_ok",
                "tip_attr_ignore_atk_magic_def_diff_ok",
                "tip_attr_ignore_atk_physics_def_rate_diff_ok",
                "tip_attr_ignore_atk_magic_def_rate_diff_ok",
            };

            static string ms_growth_succeed = "tip_growth_continue_ok";
            static string ms_growth_failed = "tip_growth_continue_failed";
            static string[] ms_tr_values_growth_failed = new string[8]
            {
                "growth_tip_attr_failed",
                "tip_attr_ignore_def_physic_attack_diff_failed",
                "tip_attr_ignore_def_magic_attack_diff_failed",
                "tip_attr_ignore_atk_IngoreIndependence_def_diff_failed",
                "tip_attr_ignore_atk_physics_def_diff_failed",
                "tip_attr_ignore_atk_magic_def_diff_failed",
                "tip_attr_ignore_atk_physics_def_rate_diff_failed",
                "tip_attr_ignore_atk_magic_def_rate_diff_failed",
            };
            static string[] ms_tr_values_growth_ok = new string[8]
            {
                "growth_tip_attr_ok",
                "tip_attr_ignore_def_physic_attack_diff_ok",
                "tip_attr_ignore_def_magic_attack_diff_ok",
                "tip_attr_ignore_atk_IngoreIndependence_def_diff_ok",
                "tip_attr_ignore_atk_physics_def_diff_ok",
                "tip_attr_ignore_atk_magic_def_diff_ok",
                "tip_attr_ignore_atk_physics_def_rate_diff_ok",
                "tip_attr_ignore_atk_magic_def_rate_diff_ok",
            };

            //辅助装备强化描述
            static string[] ms_tr_values_strengthen_assist_failed = new string[4]
            {
                "growth_tip_attr_assist_strenth_failed",
                "growth_tip_attr_assist_intellect_failed",
                "growth_tip_attr_assist_spirit_failed",
                "growth_tip_attr_assist_stamina_failed",
            };

            static string[] ms_tr_values_strengthen_assist_ok = new string[4]
            {
                "growth_tip_attr_assist_strenth_ok",
                "growth_tip_attr_assist_intellect_ok",
                "growth_tip_attr_assist_spirit_ok",
                "growth_tip_attr_assist_stamina_ok",
            };

            //辅助装备激化描述
            static string[] ms_tr_values_growth_assist_failed = new string[5]
           {
                "growth_tip_attr_failed",
                "growth_tip_attr_assist_strenth_failed",
                "growth_tip_attr_assist_intellect_failed",
                "growth_tip_attr_assist_spirit_failed",
                "growth_tip_attr_assist_stamina_failed",
           };

            static string[] ms_tr_values_growth_assist_ok = new string[5]
            {
                "growth_tip_attr_ok",
                "growth_tip_attr_assist_strenth_ok",
                "growth_tip_attr_assist_intellect_ok",
                "growth_tip_attr_assist_spirit_ok",
                "growth_tip_attr_assist_stamina_ok",
            };

            GameObject goLocal;
            GameObject goParent;
            GameObject goPrefabs;
            StrengthenContinueResultsFrame THIS;
            StrengthenResult data;
            Text desc;
            GameObject goLine;
            int iIndex = 0;

            public override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefabs = param[1] as GameObject;
                data = param[2] as StrengthenResult;
                THIS = param[3] as StrengthenContinueResultsFrame;
                iIndex = (int)param[4];

                if (goLocal == null)
                {
                    goLocal = GameObject.Instantiate(goPrefabs);
                    Utility.AttachTo(goLocal, goParent);

                    goLine = Utility.FindChild(goLocal, "Line");
                    desc = Utility.FindComponent<Text>(goLocal, "Desc");
                }

                Enable();
                _Update();
            }

            void _Update()
            {
                goLine.CustomActive(iIndex > 0);
                string content = "";
                if (data.EquipData.EquipType == EEquipType.ET_COMMON)
                {
                    content = data.StrengthenSuccess ? TR.Value(ms_succeed) : TR.Value(ms_failed);
                    ItemData itemData = GameClient.ItemDataManager.CreateItemDataFromTable(data.iTableID);
                    content += string.Format(TR.Value(data.StrengthenSuccess ? "tip_strengthen_continue_name_ok" : "tip_strengthen_continue_name_failed"), itemData.GetColorName(), data.iTargetStrengthenLevel);
                    //content += "\n";
                    StrengthenCost kCost = new StrengthenCost();
                    if (StrengthenDataManager.GetInstance().GetCost(data.iStrengthenLevel - 1, itemData.LevelLimit, itemData.Quality, ref kCost))
                    {
                        if (itemData.SubType == (int)ProtoTable.ItemTable.eSubType.WEAPON)
                        {
                            float fRadio = 1.0f;
                            var SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_STRENGTH_WP_COST_MOD);
                            if (SystemValueTableData != null)
                            {
                                fRadio = SystemValueTableData.Value / 10.0f;
                            }

                            kCost.ColorCost = Utility.ceil(kCost.ColorCost * fRadio);
                            kCost.UnColorCost = Utility.ceil(kCost.UnColorCost * fRadio);
                            kCost.GoldCost = Utility.ceil(kCost.GoldCost * fRadio);
                        }
                        else if (itemData.SubType >= (int)ProtoTable.ItemTable.eSubType.HEAD && itemData.SubType <= (int)ProtoTable.ItemTable.eSubType.BOOT)
                        {
                            float fRadio = 1.0f;
                            var SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_STRENGTH_DF_COST_MOD);
                            if (SystemValueTableData != null)
                            {
                                fRadio = SystemValueTableData.Value / 10.0f;
                            }

                            kCost.ColorCost = Utility.ceil(kCost.ColorCost * fRadio);
                            kCost.UnColorCost = Utility.ceil(kCost.UnColorCost * fRadio);
                            kCost.GoldCost = Utility.ceil(kCost.GoldCost * fRadio);
                        }
                        else if (itemData.SubType >= (int)ProtoTable.ItemTable.eSubType.RING && itemData.SubType <= (int)ProtoTable.ItemTable.eSubType.BRACELET)
                        {
                            float fRadio = 1.0f;
                            var SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_STRENGTH_JW_COST_MOD);
                            if (SystemValueTableData != null)
                            {
                                fRadio = SystemValueTableData.Value / 10.0f;
                            }

                            kCost.ColorCost = Utility.ceil(kCost.ColorCost * fRadio);
                            kCost.UnColorCost = Utility.ceil(kCost.UnColorCost * fRadio);
                            kCost.GoldCost = Utility.ceil(kCost.GoldCost * fRadio);
                        }

                        int id0 = (int)ItemData.IncomeType.IT_UNCOLOR;
                        int id1 = (int)ItemData.IncomeType.IT_COLOR;
                        int id2 = ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.BindGOLD);

                        List<AwardItemData> items = new List<AwardItemData>();
                        if (kCost.UnColorCost > 0)
                        {
                            var current = new AwardItemData();
                            current.ID = id0;
                            current.Num = kCost.UnColorCost;
                            items.Add(current);
                        }
                        if (kCost.ColorCost > 0)
                        {
                            var current = new AwardItemData();
                            current.ID = id1;
                            current.Num = kCost.ColorCost;
                            items.Add(current);
                        }
                        if (kCost.GoldCost > 0)
                        {
                            var current = new AwardItemData();
                            current.ID = id2;
                            current.Num = kCost.GoldCost;
                            items.Add(current);
                        }
                        if (items.Count > 0)
                        {
                            //bool bFirst = true;
                            //for (int j = 0; j < items.Count; ++j)
                            //{
                            //    var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(items[j].ID);
                            //    if(item == null)
                            //    {
                            //        Logger.LogErrorFormat("items[j].ID = {0}", items[j].ID);
                            //        continue;
                            //    }

                            //    if(!bFirst)
                            //    {
                            //        content += ",";
                            //    }

                            //    content += string.Format("消耗:{0}{1}", items[j].Num, item.Name);
                            //    bFirst = false;
                            //}
                            //content += ".";
                        }


                        List<string> stringAttrs = new List<string>();

                        //如果选中的装备是辅助装备
                        if (data.EquipData.IsAssistEquip())
                        {
                            for (int i = 0; i < data.assistEquipStrengthenOrgAttr.Length; ++i)
                            {
                                int delta = Mathf.FloorToInt(data.assistEquipStrengthenCurAttr[i] - data.assistEquipStrengthenOrgAttr[i]);
                                if (delta != 0)
                                {
                                    stringAttrs.Add(string.Format(TR.Value(data.StrengthenSuccess ? ms_tr_values_strengthen_assist_ok[i] : ms_tr_values_strengthen_assist_failed[i],
                                        StrengthenResult.ToValue(i, data.assistEquipStrengthenOrgAttr[i]),
                                        StrengthenResult.ToValue(i, data.assistEquipStrengthenCurAttr[i]),
                                        delta > 0 ? "+" : "",
                                        StrengthenResult.ToValue(i, delta))));
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < data.orgAttr.Length; ++i)
                            {
                                int delta = Mathf.FloorToInt(data.curAttr[i] - data.orgAttr[i]);
                                if (delta != 0)
                                {
                                    stringAttrs.Add(string.Format(TR.Value(data.StrengthenSuccess ? ms_tr_values_ok[i] : ms_tr_values_failed[i],
                                        StrengthenResult.ToValue(i, data.orgAttr[i]),
                                        StrengthenResult.ToValue(i, data.curAttr[i]),
                                        delta > 0 ? "+" : "",
                                        StrengthenResult.ToValue(i, delta))));
                                }
                            }
                        }

                            
                        for (int i = 0; stringAttrs != null && i < stringAttrs.Count; ++i)
                        {
                            content += ("\n" + stringAttrs[i]);
                        }
                    }
                    
                }
                else if (data.EquipData.EquipType == EEquipType.ET_REDMARK)
                {
                    content = data.StrengthenSuccess ? TR.Value(ms_growth_succeed) : TR.Value(ms_growth_failed);
                    ItemData itemData = GameClient.ItemDataManager.CreateItemDataFromTable(data.iTableID);
                    content += string.Format(TR.Value(data.StrengthenSuccess ? "tip_strengthen_continue_name_ok" : "tip_strengthen_continue_name_failed"), itemData.GetColorName(), data.iTargetStrengthenLevel);

                    List<string> stringAttrs = new List<string>();

                    //如果选中的装备是辅助装备
                    if (data.EquipData.IsAssistEquip())
                    {
                        for (int i = 0; i < data.assistEquipGrowthOrgAttr.Length; i++)
                        {
                            int delta = Mathf.FloorToInt(data.assistEquipGrowthCurAttr[i] - data.assistEquipGrowthOrgAttr[i]);

                            if (delta != 0)
                            {
                                if (i == 0)
                                {
                                    stringAttrs.Add(string.Format(TR.Value(data.StrengthenSuccess ? ms_tr_values_growth_assist_ok[i] : ms_tr_values_growth_assist_failed[i],
                                     EquipGrowthDataManager.GetInstance().GetGrowthAttrDesc(data.EquipData.GrowthAttrType),
                                     data.assistEquipGrowthOrgAttr[i],
                                     data.assistEquipGrowthCurAttr[i],
                                     delta > 0 ? "+" : "",
                                     delta)));
                                }
                                else
                                {
                                    stringAttrs.Add(string.Format(TR.Value(data.StrengthenSuccess ? ms_tr_values_growth_assist_ok[i] : ms_tr_values_growth_assist_failed[i],
                                      data.assistEquipGrowthOrgAttr[i],
                                      data.assistEquipGrowthCurAttr[i],
                                      delta > 0 ? "+" : "",
                                      delta)));
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < data.growthOrgAttr.Length; ++i)
                        {
                            int delta = Mathf.FloorToInt(data.growthCurAttr[i] - data.growthOrgAttr[i]);

                            if (delta != 0)
                            {
                                if (i == 0)
                                {
                                    stringAttrs.Add(string.Format(TR.Value(data.StrengthenSuccess ? ms_tr_values_growth_ok[i] : ms_tr_values_growth_failed[i],
                                     EquipGrowthDataManager.GetInstance().GetGrowthAttrDesc(data.EquipData.GrowthAttrType),
                                     StrengthenResult.GrowthToValue(i, data.growthOrgAttr[i]),
                                     StrengthenResult.GrowthToValue(i, data.growthCurAttr[i]),
                                     delta > 0 ? "+" : "",
                                     StrengthenResult.GrowthToValue(i, delta))));
                                }
                                else
                                {
                                    stringAttrs.Add(string.Format(TR.Value(data.StrengthenSuccess ? ms_tr_values_growth_ok[i] : ms_tr_values_growth_failed[i],
                                      StrengthenResult.GrowthToValue(i, data.growthOrgAttr[i]),
                                      StrengthenResult.GrowthToValue(i, data.growthCurAttr[i]),
                                      delta > 0 ? "+" : "",
                                      StrengthenResult.GrowthToValue(i, delta))));
                                }
                            }
                        }
                    }

                   
                    for (int i = 0; stringAttrs != null && i < stringAttrs.Count; ++i)
                    {
                        content += ("\n" + stringAttrs[i]);
                    }
                }

                desc.text = content;
            }

            public override void OnDestroy()
            {
                desc = null;
                goLocal = null;
                goPrefabs = null;
                goParent = null;
                THIS = null;
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
                if(param.Length > 0)
                {
                    data = param[0] as StrengthenResult;
                }
                _Update();
            }
            public override void Enable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(true);
                }
            }
            public override void Disable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public override bool NeedFilter(object[] param) { return false; }

            public override void SetAsLastSibling()
            {
                if(goLocal != null)
                {
                    goLocal.transform.SetAsLastSibling();
                }
            }
        }
        CachedObjectListManager<CachedItemDescObject> m_akCachedList = new CachedObjectListManager<CachedItemDescObject>();

        public class StrengthenContinueResultsFrameData
        {
            public bool bStopByHandle;
            public StrengthOperateResult eLastOpResult;
            public int iTarget;
            public List<StrengthenResult> results;
        }
        StrengthenContinueResultsFrameData m_kData;
        ItemData mEquipItemData;

        protected override void _OnOpenFrame()
        {
            m_kTitle = Utility.FindComponent<Text>(frame, "bg/Text/Text");
            m_goParent = Utility.FindChild(frame, "bg/ScrollView/Viewport/Content");
            m_goPrefabs = Utility.FindChild(m_goParent, "ResultObject");
            m_goPrefabs.CustomActive(false);

            m_akCachedList.Clear();
            m_kData = userData as StrengthenContinueResultsFrameData;
            if (m_kData != null && m_kData.results.Count > 0)
            {
                mEquipItemData = m_kData.results[0].EquipData;
            }
            _SetData(m_kData);
        }

        void _SetData(StrengthenContinueResultsFrameData data)
        {
            //tittle
            string value = "";
            if(data.bStopByHandle)
            {
                if (mEquipItemData != null)
                {
                    if (mEquipItemData.EquipType == EEquipType.ET_COMMON)
                    {
                        value = string.Format(TR.Value("strengthen_cs_stop_by_handle"), data.results.Count);
                    }
                    else if (mEquipItemData.EquipType == EEquipType.ET_REDMARK)
                    {
                        value = string.Format(TR.Value("growth_cs_stop_by_handle"), data.results.Count);
                    }
                }
                else
                {
                    value = string.Format(TR.Value("strengthen_cs_stop_by_handle"), data.results.Count);
                }
                
            }
            else
            {
                switch(data.eLastOpResult)
                {
                    case StrengthOperateResult.SOR_COLOR:
                        {
                            value = string.Format(TR.Value("strengthen_cs_stop_color_not_enough"), data.results.Count);
                        }
                        break;
                    case StrengthOperateResult.SOR_COST_ITEM_NOT_EXIST:
                        {
                            value = string.Format(TR.Value("strengthen_cs_stop_cost_item_not_enough"), data.results.Count);
                        }
                        break;
                    case StrengthOperateResult.SOR_GOLD:
                        {
                            if (mEquipItemData != null)
                            {
                                if (mEquipItemData.EquipType == EEquipType.ET_COMMON)
                                {
                                    value = string.Format(TR.Value("strengthen_cs_stop_gold_not_enough"), data.results.Count);
                                }
                                else if (mEquipItemData.EquipType == EEquipType.ET_REDMARK)
                                {
                                    value = string.Format(TR.Value("growth_cs_stop_gold_not_enough"), data.results.Count);
                                }
                            }
                            else
                            {
                                value = string.Format(TR.Value("strengthen_cs_stop_gold_not_enough"), data.results.Count);
                            }
                            
                        }
                        break;
                    case StrengthOperateResult.SOR_HAS_NO_ITEM:
                        {
                            value = string.Format(TR.Value("strengthen_cs_stop_target_missing"), data.results.Count);
                        }
                        break;
                    case StrengthOperateResult.SOR_HAS_NO_PROTECTED:
                        {
                            value = string.Format(TR.Value("strengthen_cs_stop_has_no_protected"), data.results.Count);
                        }
                        break;
                    case StrengthOperateResult.SOR_UNCOLOR:
                        {
                            value = string.Format(TR.Value("strengthen_cs_stop_uncolor_not_enough"), data.results.Count);
                        }
                        break;
                    case StrengthOperateResult.SOR_OK:
                        {
                            if (mEquipItemData != null)
                            {
                                if (mEquipItemData.EquipType == EEquipType.ET_COMMON)
                                {
                                    value = string.Format(TR.Value("strengthen_cs_stop_finished"), m_kData.iTarget, data.results.Count);
                                }
                                else if (mEquipItemData.EquipType == EEquipType.ET_REDMARK)
                                {
                                    value = string.Format(TR.Value("growth_cs_stop_finished"), m_kData.iTarget, data.results.Count);
                                }
                            }
                            else
                            {
                                value = string.Format(TR.Value("strengthen_cs_stop_finished"), m_kData.iTarget, data.results.Count);
                            }
                        }
                        break;
                    case StrengthOperateResult.SOR_Paradoxicalcrystal:
                        {
                            value = string.Format(TR.Value("growth_cs_stop_Paradoxicalcrystal_not_enough"), data.results.Count);
                        }
                        break;
                    case StrengthOperateResult.SOR_Strengthen_Implement:
                        {
                            if (mEquipItemData != null)
                            {
                                if (mEquipItemData.EquipType == EEquipType.ET_COMMON)
                                {
                                    value = TR.Value("strengthen_cs_stop_not_strengthenimplement", data.results.Count);
                                }
                                else if (mEquipItemData.EquipType == EEquipType.ET_REDMARK)
                                {
                                    value = TR.Value("strengthen_cs_stop_not_growthimplement", data.results.Count);
                                }
                            }
                        }
                        break;
                }
            }
            m_kTitle.text = value;
            //descs
            m_akCachedList.RecycleAllObject();
            m_kData.results.Reverse();
            for (int i = 0; i < m_kData.results.Count; ++i)
            {
                m_akCachedList.Create(new object[] 
                {
                    m_goParent,m_goPrefabs,m_kData.results[i],this,i,
                });
            }
        }

        protected override void _OnCloseFrame()
        {
            //add by mjx for strength equip to 10 firstly to show gift
            LimitTimeGift.LimitTimeGiftFrameManager.GetInstance().WaitToShowLimitTimeGiftFrame();

            m_akCachedList.DestroyAllObjects();
            mEquipItemData = null;
        }

        [UIEventHandle("bg/ok")]
        void OnClickOk()
        {
            switch (m_kData.eLastOpResult)
            {
                case StrengthOperateResult.SOR_COLOR:
                case StrengthOperateResult.SOR_GOLD:
                case StrengthOperateResult.SOR_UNCOLOR:
                    ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.TryShowMallGift(Protocol.MallGiftPackActivateCond.STRENGEN_NO_RESOURCE, null);
                    break;
            }

            frameMgr.CloseFrame(this);
        }
    }
}