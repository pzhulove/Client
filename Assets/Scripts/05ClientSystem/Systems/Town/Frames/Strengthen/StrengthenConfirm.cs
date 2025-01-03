using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    class StrengthenConfirmData
    {
        public ItemData ItemData;
        public int TargetStrengthenLevel;
        public bool UseProtect;
        public bool UseStrengthenImplement;
        public ItemData StrengthenImplementItem;
    }

    class StrengthenConfirm : ClientFrame
    {
        StrengthenConfirmData m_data;
        GameObject m_itemPrefab;

        List<ComItemNew> m_arrComItems = new List<ComItemNew>();
        Button btnOk;
        Button btnOk2;
        Text hintUsed;
        Text hintUnUsed;
        bool bUseModeNormal = true;
        GameObject goMode0;
        GameObject goMode1;
        bool isSelected = false;

        string OkBtnOriginText = "";
        bool bIsUpdate = false;
        float fCountDownTime = 3.0f;
        float fAddUpTime = 0.0f;
        Text okBtnText;
        UIGray okBtnGray2;
        class ItemObject
        {
            public int id;
            public int count;
        }

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/StrengthenConfirm";
        }

        protected sealed override void _OnOpenFrame()
        {
            isSelected = false;
            hintUsed = Utility.FindComponent<Text>(frame, "image/hintUsed");
            hintUnUsed = Utility.FindComponent<Text>(frame, "image/hintUnUsed");
            btnOk = Utility.FindComponent<Button>(frame, "Mode0/ok");
            btnOk.enabled = true;
            btnOk2 = Utility.FindComponent<Button>(frame, "Mode1/ok");
            btnOk2.enabled = true;
            goMode0 = Utility.FindChild(frame, "Mode0");
            goMode1 = Utility.FindChild(frame, "Mode1");
            okBtnText = Utility.FindComponent<Text>(frame, "Mode1/ok/Text");
            okBtnGray2 = Utility.FindComponent<UIGray>(frame, "Mode1/ok");
            if (okBtnText != null)
            {
                OkBtnOriginText = okBtnText.text;
            }

            m_data = userData as StrengthenConfirmData;
            if (m_data == null)
            {
                Logger.LogError("StrengthenConfirm data is null!!");
                return;
            }

            bUseModeNormal = m_data.ItemData.StrengthenLevel < 10 || m_data.UseProtect;
            goMode0.CustomActive(bUseModeNormal);
            goMode1.CustomActive(!bUseModeNormal);

            hintUsed.enabled = m_data.UseProtect;
            hintUnUsed.enabled = !m_data.UseProtect;

            StrengthenCost cost = new StrengthenCost();

            if (m_data.UseStrengthenImplement == true)
            {

            }
            else
            {
                if (m_data.ItemData.EquipType == EEquipType.ET_COMMON)
                {
                    bool success = StrengthenDataManager.GetInstance().GetCost(
                        m_data.ItemData.StrengthenLevel, m_data.ItemData.LevelLimit, m_data.ItemData.Quality, ref cost
                        );

                    var data = m_data.ItemData;
                    if (data.SubType == (int)ProtoTable.ItemTable.eSubType.WEAPON)
                    {
                        float fRadio = 1.0f;
                        var SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_STRENGTH_WP_COST_MOD);
                        if (SystemValueTableData != null)
                        {
                            fRadio = SystemValueTableData.Value / 10.0f;
                        }

                        cost.ColorCost = Utility.ceil(cost.ColorCost * fRadio);
                        cost.UnColorCost = Utility.ceil(cost.UnColorCost * fRadio);
                        cost.GoldCost = Utility.ceil(cost.GoldCost * fRadio);
                    }
                    else if (data.SubType >= (int)ProtoTable.ItemTable.eSubType.HEAD && data.SubType <= (int)ProtoTable.ItemTable.eSubType.BOOT)
                    {
                        float fRadio = 1.0f;
                        var SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_STRENGTH_DF_COST_MOD);
                        if (SystemValueTableData != null)
                        {
                            fRadio = SystemValueTableData.Value / 10.0f;
                        }

                        cost.ColorCost = Utility.ceil(cost.ColorCost * fRadio);
                        cost.UnColorCost = Utility.ceil(cost.UnColorCost * fRadio);
                        cost.GoldCost = Utility.ceil(cost.GoldCost * fRadio);
                    }
                    else if (data.SubType >= (int)ProtoTable.ItemTable.eSubType.RING && data.SubType <= (int)ProtoTable.ItemTable.eSubType.BRACELET)
                    {
                        float fRadio = 1.0f;
                        var SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_STRENGTH_JW_COST_MOD);
                        if (SystemValueTableData != null)
                        {
                            fRadio = SystemValueTableData.Value / 10.0f;
                        }

                        cost.ColorCost = Utility.ceil(cost.ColorCost * fRadio);
                        cost.UnColorCost = Utility.ceil(cost.UnColorCost * fRadio);
                        cost.GoldCost = Utility.ceil(cost.GoldCost * fRadio);
                    }

                    if (success == false)
                    {
                        Logger.LogErrorFormat(
                            "StrengthenConfirm can not find strengthen cost!! ItemTableID:{0} ItemGUID:{1} StrengthenLevel:{2} EquipLevel:{3} Quality:{4}",
                            m_data.ItemData.TableID,
                            m_data.ItemData.GUID,
                            m_data.ItemData.StrengthenLevel,
                            m_data.ItemData.LevelLimit,
                            (int)m_data.ItemData.Quality
                            );
                        return;
                    }
                }
            }
            
            Text level = Utility.FindGameObject(frame, "Text").GetComponent<Text>();
            if (m_data.ItemData.EquipType == EEquipType.ET_COMMON)
            {
                bIsUpdate = m_data.ItemData.Quality > ItemTable.eColor.PURPLE && !bUseModeNormal;

                level.text = TR.Value("strengthen_level_desc", m_data.TargetStrengthenLevel + 1);
            }
            else if (m_data.ItemData.EquipType == EEquipType.ET_REDMARK)
            {
                bIsUpdate = !bUseModeNormal;

                level.text = TR.Value("growth_level_desc", m_data.TargetStrengthenLevel + 1);

                if (hintUsed != null)
                {
                    hintUsed.text = TR.Value("growth_use_protectstamp_desc");
                }

                if (hintUnUsed != null)
                {
                    hintUnUsed.text = TR.Value("growth_unuse_protectstamp_desc");
                }
            }
            
            if (bIsUpdate)
            {
                var systemValue = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType3.SVT_CONFIRT_COUNTDOWN_TIME);
                fCountDownTime = systemValue.Value;

                _SetOkButtonEnabe(false);
            }

            //ComItem equipItem = _CreateComItem(Utility.FindGameObject(frame, "middle/head/itemParent"));
            //equipItem.SetActive(true);
            //equipItem.SetEnable(true);
            //equipItem.Setup(m_data.ItemData, null);
            //Text equipDesc = Utility.FindGameObject(frame, "middle/head/Name").GetComponent<Text>();
            //equipDesc.text = m_data.ItemData.GetColorName();

            Text hint = Utility.FindGameObject(frame, "middle/nick/hint").GetComponent<Text>();
            if(m_data.UseProtect)
            {
                hint.text = TR.Value("strengthen_cost_hint_use_protect");
            }
            else
            {
                if(m_data.ItemData.StrengthenLevel + 1 <= 10)
                {
                    hint.text = TR.Value("strengthen_cost_hint_neednot_protect");
                }
                else
                {
                    hint.text = TR.Value("strengthen_cost_hint_unuse_protect");
                }
            }

            List<ItemObject> aiMaterials = new List<ItemObject>();
            aiMaterials.Clear();

            if (m_data.UseStrengthenImplement == true)
            {
                if (m_data.StrengthenImplementItem != null)
                {
                    ItemObject current = new ItemObject();
                    current.id = m_data.StrengthenImplementItem.TableID;
                    current.count = 1;
                    aiMaterials.Add(current);
                }

                if (m_data.ItemData.EquipType == EEquipType.ET_COMMON)
                {
                    if (m_data.UseProtect)
                    {
                        ItemObject current = new ItemObject();
                        current.id = (int)ItemData.IncomeType.IT_PROTECTED;
                        current.count = 1;
                        aiMaterials.Add(current);
                    }
                }
                else if (m_data.ItemData.EquipType == EEquipType.ET_REDMARK)
                {
                    if (m_data.UseProtect)
                    {
                        ItemObject current = new ItemObject();
                        current.id = (int)ItemData.IncomeType.IT_GROWTHPROTECTED;
                        current.count = 1;
                        aiMaterials.Add(current);
                    }
                }
            }
            else
            {
                if (m_data.ItemData.EquipType == EEquipType.ET_COMMON)
                {
                    if (cost.ColorCost > 0)
                    {
                        ItemObject current = new ItemObject();
                        current.id = (int)ItemData.IncomeType.IT_COLOR;
                        current.count = cost.ColorCost;
                        aiMaterials.Add(current);
                    }
                    if (cost.UnColorCost > 0)
                    {
                        ItemObject current = new ItemObject();
                        current.id = (int)ItemData.IncomeType.IT_UNCOLOR;
                        current.count = cost.UnColorCost;
                        aiMaterials.Add(current);
                    }
                    if (cost.GoldCost > 0)
                    {
                        ItemObject current = new ItemObject();
                        current.id = (int)ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.BindGOLD);
                        current.count = cost.GoldCost;
                        aiMaterials.Add(current);
                    }
                    if (m_data.UseProtect)
                    {
                        ItemObject current = new ItemObject();
                        current.id = (int)ItemData.IncomeType.IT_PROTECTED;
                        current.count = 1;
                        aiMaterials.Add(current);
                    }
                }
                else if (m_data.ItemData.EquipType == EEquipType.ET_REDMARK)
                {
                    var materialList = EquipGrowthDataManager.GetInstance().GetGrowthCosts(m_data.ItemData);
                    for (int i = 0; i < materialList.Count; i++)
                    {
                        ItemObject current = new ItemObject();
                        current.id = materialList[i].ItemID;
                        current.count = materialList[i].Count;

                        aiMaterials.Add(current);
                    }

                    if (m_data.UseProtect)
                    {
                        ItemObject current = new ItemObject();
                        current.id = (int)ItemData.IncomeType.IT_GROWTHPROTECTED;
                        current.count = 1;
                        aiMaterials.Add(current);
                    }
                }
            }
            
            GameObject goPrefabs = Utility.FindGameObject(frame, "middle/body/Viewport/content/prefabs");
            goPrefabs.CustomActive(false);
            GameObject goParent = Utility.FindGameObject(frame, "middle/body/Viewport/content");
            for (int i = 0; i < aiMaterials.Count; ++i)
            {
                ItemData itemData = GameClient.ItemDataManager.CreateItemDataFromTable(aiMaterials[i].id);
                GameObject goCurrent = GameObject.Instantiate(goPrefabs);
                Utility.AttachTo(goCurrent, goParent);
                goCurrent.CustomActive(true);

                if (goCurrent != null)
                {
                    GameObject goItemParent = Utility.FindChild(goCurrent, "itemParent");
                    ComItemNew comItem = CreateComItemNew(goItemParent);
                    m_arrComItems.Add(comItem);
                    Text desc = Utility.FindComponent<Text>(goCurrent, "name");
                    ItemData.QualityInfo qualityInfo = itemData.GetQualityInfo();
                    itemData.Count = aiMaterials[i].count;
                    comItem.Setup(itemData, null);

                    if (itemData.Type == ProtoTable.ItemTable.eType.INCOME)
                    {
                        desc.text = itemData.GetColorName();// string.Format("<color={0}>{1}{2}</color>", qualityInfo.ColStr, aiMaterials[i].count, itemData.Name);
                    }
                    else
                    {
                        desc.text = itemData.GetColorName(); //string.Format("<color={0}>{1}X{2}</color>", qualityInfo.ColStr, itemData.Name, aiMaterials[i].count);
                    }
                }
            }
        }

        protected override void _OnCloseFrame()
        {
            m_arrComItems.Clear();
            m_data = null;
            isSelected = false;
            OkBtnOriginText = "";
            bIsUpdate = false;
            fCountDownTime = 3.0f;
            fAddUpTime = 0.0f;
        }

        protected ComItemNew _CreateComItem(GameObject parent)
        {
            if (parent != null)
            {
                if (m_itemPrefab == null)
                {
                    m_itemPrefab = AssetLoader.instance.LoadResAsGameObject("UI/Prefabs/Item/ItemPrefab");
                    //m_itemPrefab.SetActive(false);
                }

                // GameObject obj = GameObject.Instantiate(m_itemPrefab);
                // obj.transform.SetParent(parent.transform, false);
                // return obj.GetComponent<ComItem>();
                
                m_itemPrefab.transform.SetParent(parent.transform, false);
                return m_itemPrefab.GetComponent<ComItemNew>();
            }
            return null;
        }

        [UIEventHandle("Mode0/close")]
        void _OnClose()
        {
            _OnClickClose();
        }

        [UIEventHandle("Mode1/close")]
        void _OnClose2()
        {
            _OnClickClose();
        }

        void _OnClickClose()
        {
            if (isSelected == true)
            {
                return;
            }

            isSelected = true;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.StrengthenCanceled);
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("Mode0/ok")]
        void _OnConfirm()
        {
            btnOk.enabled = false;
            _OnClickConfirm();
        }

        [UIEventHandle("Mode1/ok")]
        void _OnConfirm2()
        {
            btnOk2.enabled = false;
            _OnClickConfirm();
        }

        void _OnClickConfirm()
        {
            if (isSelected == true)
            {
                return;
            }

            isSelected = true;
            frameMgr.CloseFrame(this);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnStrengthChanged);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.StrengthenDelay);
        }

        public override bool IsNeedUpdate()
        {
            return bIsUpdate;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            int iInt = (int)fCountDownTime;

            if (okBtnText != null)
            {
                okBtnText.text = string.Format("{0}({1}s)", OkBtnOriginText, iInt);
            }

            fAddUpTime += timeElapsed;

            if (fAddUpTime > 1.0f)
            {
                fCountDownTime -= 1.0f;
                fAddUpTime = 0.0f;
            }

            if (iInt <= 0)
            {
                bIsUpdate = false;

                if (okBtnText != null)
                {
                    okBtnText.text = OkBtnOriginText;
                }

                _SetOkButtonEnabe(true);
            }
        }

        private void _SetOkButtonEnabe(bool bEnable)
        {
            if (btnOk2 != null)
            {
                btnOk2.enabled = bEnable;
            }

            if (okBtnGray2 != null)
            {
                okBtnGray2.enabled = !bEnable;
            }
        }
    }
}