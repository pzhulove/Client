using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using Protocol;
using System.Reflection;
using EItemQuality = ProtoTable.ItemTable.eColor;
using UnityEngine;
using ProtoTable;

namespace GameClient
{
    /// <summary>
    /// 品质Tab数据
    /// </summary>
    public class CommonQualityTabData : ComControlData
    {
        public CommonQualityTabData(int index, int id, string name, bool isSelected)
            : base(index, id, name, isSelected)
        {

        }
    }

    /// <summary>
    /// 等级Tab数据
    /// </summary>
    public class CommonLevelTabData : ComControlData
    {
        public CommonLevelTabData(int index, int id, string name, bool isSelected)
            : base(index, id, name, isSelected)
        {

        }
    }

    struct StrengthenCost
    {
        public StrengthenCost(int goldCost, int unColorCost, int colorCost)
        {
            GoldCost = goldCost;
            UnColorCost = unColorCost;
            ColorCost = colorCost;
        }

        public int GoldCost;
        public int UnColorCost;
        public int ColorCost;
    }

    class AuxiliaryEquipmentStrengthenAttrData
    {
        public int strengthenLevel;
        public int equipLevel;
        public int quality;
        public int quality2;
        public float attrValue;
    }

    class StrengthenDataManager : DataManager<StrengthenDataManager>
    {
        class CostData
        {
            public int EquipLevelMin;
            public int EquipLevelMax;
            public StrengthenCost[] Costs = new StrengthenCost[(int)EItemQuality.YELLOW];
        }

        CostData[,] m_arrStrengthenCost;
        bool m_bInited = false;

        StrengthenResult m_strengthenResult;
        int m_iMaxIndex = 0;

        public bool IsEquipStrengthened = false;      //装备被强化的标志

        List<AuxiliaryEquipmentStrengthenAttrData> mAuxiliaryEquipmentStrengthenAttrDataList = new List<AuxiliaryEquipmentStrengthenAttrData>();

        /// <summary>
        /// 装备强化是否破碎
        /// </summary>
        public bool IsEquipmentStrengthBroken = false;

        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.StrengthenDataManager;
        }

        public override void Initialize()
        {
            m_bIsStrenghtenContinue = false;
            if (m_bInited == false)
            {
                m_iMaxIndex = 0;
                Dictionary<int, object> tableData = TableManager.GetInstance().GetTable<ProtoTable.EquipStrengthenTable>();
                if (tableData != null && tableData.Count >= 1)
                {
                    m_iMaxIndex = tableData.Count / 20 + (tableData.Count % 20 == 0 ? 0 : 1);
                    m_arrStrengthenCost = new CostData[20, m_iMaxIndex];
                }
                var iter = tableData.GetEnumerator();
                while (iter.MoveNext())
                {
                    ProtoTable.EquipStrengthenTable data = iter.Current.Value as ProtoTable.EquipStrengthenTable;

                    CostData costData = new CostData();
                    costData.EquipLevelMin = data.Lv[0];
                    costData.EquipLevelMax = data.Lv.Count > 1 ? data.Lv[1] : data.Lv[0];

                    try
                    {
                        costData.Costs[(int)EItemQuality.WHITE - 1] = new StrengthenCost(data.WhiteGoldCost, data.WhiteCost[0], data.WhiteCost[1]);
                        costData.Costs[(int)EItemQuality.BLUE - 1] = new StrengthenCost(data.BlueGoldCost, data.BlueCost[0], data.BlueCost[1]);
                        costData.Costs[(int)EItemQuality.PURPLE - 1] = new StrengthenCost(data.PurpleGoldCost, data.PurpleCost[0], data.PurpleCost[1]);
                        costData.Costs[(int)EItemQuality.GREEN - 1] = new StrengthenCost(data.GreenGoldCost, data.GreenCost[0], data.GreenCost[1]);
                        costData.Costs[(int)EItemQuality.PINK - 1] = new StrengthenCost(data.PinkGoldCost, data.PinkCost[0], data.PinkCost[1]);
                        costData.Costs[(int)EItemQuality.YELLOW - 1] = new StrengthenCost(data.YellowGoldCost, data.YellowCost[0], data.YellowCost[1]);
                        m_arrStrengthenCost[data.Strengthen, (data.ID - 1) / 20] = costData;
                    }
                    catch (Exception e)
                    {
                        Logger.LogErrorFormat("data.WhiteCost.Length = {0}", data.WhiteCost.Count);
                        Logger.LogErrorFormat("data.Strengthen = {0},data.ID = {1},(data.ID - 1) / 20 = {2} costData.min = {3} costData.max = {4}", data.Strengthen, data.ID, (data.ID - 1) / 20, costData.EquipLevelMin, costData.EquipLevelMax);
                    }
                }

                m_bInited = true;
            }

            InitAssistEqStrengFouerDimAddTable();
        }

        public override void Clear()
        {
            if (mAuxiliaryEquipmentStrengthenAttrDataList != null)
            {
                mAuxiliaryEquipmentStrengthenAttrDataList.Clear();
            }

            IsEquipStrengthened = false;
            IsEquipmentStrengthBroken = false;
        }

        /// <summary>
        /// 得到辅助装备属性值
        /// </summary>
        /// <param name="itemData"></param>
        /// <returns></returns>
        public float GetAssistEqStrengthAttrValue(ItemData itemData,int strengthenLevel)
        {
            if (itemData == null)
            {
                return 0;
            }

            for (int i = 0; i < mAuxiliaryEquipmentStrengthenAttrDataList.Count; i++)
            {
                var data = mAuxiliaryEquipmentStrengthenAttrDataList[i];
                if (strengthenLevel != data.strengthenLevel)
                {
                    continue;
                }

                if (itemData.LevelLimit != data.equipLevel)
                {
                    continue;
                }

                if ((int)itemData.Quality != data.quality)
                {
                    continue;
                }

                if (itemData.TableData.Color2 != data.quality2)
                {
                    continue;
                }

                return (data.attrValue) / 1000;
            }

            return 0;
        }

        private void InitAssistEqStrengFouerDimAddTable()
        {
            if (mAuxiliaryEquipmentStrengthenAttrDataList == null)
            {
                mAuxiliaryEquipmentStrengthenAttrDataList = new List<AuxiliaryEquipmentStrengthenAttrData>();
            }
            else
            {
                mAuxiliaryEquipmentStrengthenAttrDataList.Clear();
            }

            var tableData = TableManager.GetInstance().GetTable<AssistEqStrengFouerDimAddTable>().GetEnumerator();
            while (tableData.MoveNext())
            {
                var table = tableData.Current.Value as AssistEqStrengFouerDimAddTable;
                if (table == null)
                {
                    continue;
                }

                AuxiliaryEquipmentStrengthenAttrData data = new AuxiliaryEquipmentStrengthenAttrData();
                data.strengthenLevel = table.Strengthen;
                data.equipLevel = table.Lv;
                data.quality = (int)table.Color;
                data.quality2 = (int)table.Color2;
                data.attrValue = table.StrengNum;

                mAuxiliaryEquipmentStrengthenAttrDataList.Add(data);
            }
        }

        public bool GetCost(int strengthenLevel, int equipLevel, EItemQuality equipQuality, ref StrengthenCost result)
        {
            if (strengthenLevel >= 0 && strengthenLevel < 20)
            {
                for (int i = 0; i < m_iMaxIndex; ++i)
                {
                    CostData cost = m_arrStrengthenCost[strengthenLevel, i];
                    if (equipLevel >= cost.EquipLevelMin && equipLevel <= cost.EquipLevelMax)
                    {
                        result = cost.Costs[(int)equipQuality - 1];
                        return true;
                    }
                }
            }
            return false;
        }

        //发送装备强化消息
        public void StrengthenItem(ItemData data, byte useUnbreak, ulong ticketid = 0)
        {
            SceneEquipStrengthen msg = new SceneEquipStrengthen();
            msg.euqipUid = data.GUID;
            msg.strTickt = ticketid;
            msg.useUnbreak = (byte)useUnbreak;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);
            int iOrgStrengthenLevel = data.StrengthenLevel + 1;

            int[] aiOrgAttr;

            //如果选中的装备是辅助装备
            if (data.IsAssistEquip())
            {
                aiOrgAttr = new int[4]
                {
                    Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel)),
                    Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel)),
                    Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel)),
                    Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel)),
            };
            }
            else
            {
                aiOrgAttr = new int[7]
           {
                Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsAttack)),
                Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicAttack)),
                Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IngoreIndependence)),
                Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsDefense)),
                Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicDefense)),
                Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsDefenseRate)),
                Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicDefenseRate)),
           };
            }

            IsEquipStrengthened = true;

            WaitNetMessageManager.GetInstance().Wait(SceneEquipStrengthenRet.MsgID, msgData =>
            {
                CustomDecoder.StrengthenRet msgRet;
                int pos = 0;
                CustomDecoder.DecodeStrengthenResult(out msgRet, msgData.bytes, ref pos, msgData.bytes.Length);

                if (msgRet.code == (uint)ProtoErrorCode.ITEM_STRENTH_FAIL)
                {
                    Logger.LogProcessFormat("strengthen fail! Level:{0}", data.StrengthenLevel);

                    UIEvent uiEvent = UIEventSystem.GetInstance().GetIdleUIEvent();
                    uiEvent.EventID = EUIEventID.ItemStrengthenFail;
                    m_strengthenResult = new StrengthenResult();
                    m_strengthenResult.StrengthenSuccess = false;
                    m_strengthenResult.EquipData = data;
                    m_strengthenResult.code = msgRet.code;
                    m_strengthenResult.iStrengthenLevel = iOrgStrengthenLevel;
                    m_strengthenResult.iTargetStrengthenLevel = data.StrengthenLevel;
                    m_strengthenResult.iTableID = (int)data.TableID;

                    //如果选中的装备是辅助装备
                    if (data.IsAssistEquip())
                    {
                        for (int i = 0; i < m_strengthenResult.assistEquipStrengthenOrgAttr.Length; ++i)
                        {
                            m_strengthenResult.assistEquipStrengthenOrgAttr[i] = aiOrgAttr[i];
                        }
                        m_strengthenResult.assistEquipStrengthenCurAttr[0] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        m_strengthenResult.assistEquipStrengthenCurAttr[1] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        m_strengthenResult.assistEquipStrengthenCurAttr[2] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        m_strengthenResult.assistEquipStrengthenCurAttr[3] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                    }
                    else
                    {
                        for (int i = 0; i < m_strengthenResult.orgAttr.Length; ++i)
                        {
                            m_strengthenResult.orgAttr[i] = aiOrgAttr[i];
                        }
                        m_strengthenResult.curAttr[0] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsAttack));
                        m_strengthenResult.curAttr[1] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicAttack));
                        m_strengthenResult.curAttr[2] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IngoreIndependence));
                        m_strengthenResult.curAttr[3] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsDefense));
                        m_strengthenResult.curAttr[4] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicDefense));
                        m_strengthenResult.curAttr[5] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsDefenseRate));
                        m_strengthenResult.curAttr[6] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicDefenseRate));
                    }
                    
                    UIEventSystem.GetInstance().SendUIEvent(uiEvent);
                }
                else if (msgRet.code == (uint)ProtoErrorCode.ITEM_SPECIAL_STRENTH_FAIL)
                {
                    UIEvent uiEvent = UIEventSystem.GetInstance().GetIdleUIEvent();
                    uiEvent.EventID = EUIEventID.OnSpecailStrenghthenFailed;
                    m_strengthenResult = new StrengthenResult();
                    m_strengthenResult.StrengthenSuccess = false;
                    m_strengthenResult.EquipData = data;
                    m_strengthenResult.code = msgRet.code;
                    m_strengthenResult.iStrengthenLevel = iOrgStrengthenLevel;
                    m_strengthenResult.iTargetStrengthenLevel = data.StrengthenLevel;
                    m_strengthenResult.iTableID = (int)data.TableID;

                    //如果选中的装备是辅助装备
                    if (data.IsAssistEquip())
                    {
                        for (int i = 0; i < m_strengthenResult.assistEquipStrengthenOrgAttr.Length; ++i)
                        {
                            m_strengthenResult.assistEquipStrengthenOrgAttr[i] = aiOrgAttr[i];
                        }
                        m_strengthenResult.assistEquipStrengthenCurAttr[0] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        m_strengthenResult.assistEquipStrengthenCurAttr[1] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        m_strengthenResult.assistEquipStrengthenCurAttr[2] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        m_strengthenResult.assistEquipStrengthenCurAttr[3] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                    }
                    else
                    {
                        for (int i = 0; i < m_strengthenResult.orgAttr.Length; ++i)
                        {
                            m_strengthenResult.orgAttr[i] = aiOrgAttr[i];
                        }
                        m_strengthenResult.curAttr[0] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsAttack));
                        m_strengthenResult.curAttr[1] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicAttack));
                        m_strengthenResult.curAttr[2] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IngoreIndependence));
                        m_strengthenResult.curAttr[3] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsDefense));
                        m_strengthenResult.curAttr[4] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicDefense));
                        m_strengthenResult.curAttr[5] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsDefenseRate));
                        m_strengthenResult.curAttr[6] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicDefenseRate));
                    }
                    
                    UIEventSystem.GetInstance().SendUIEvent(uiEvent);
                }
                else if (msgRet.code == (uint)ProtoErrorCode.ITEM_STRENTH_FAIL_MINUS)
                {
                    Logger.LogProcessFormat("strengthen fail! Level:{0}(-1)", data.StrengthenLevel);

                    UIEvent uiEvent = UIEventSystem.GetInstance().GetIdleUIEvent();
                    uiEvent.EventID = EUIEventID.ItemStrengthenFail;
                    m_strengthenResult = new StrengthenResult();
                    m_strengthenResult.StrengthenSuccess = false;
                    m_strengthenResult.EquipData = data;
                    m_strengthenResult.code = msgRet.code;
                    m_strengthenResult.iStrengthenLevel = iOrgStrengthenLevel;
                    m_strengthenResult.iTargetStrengthenLevel = data.StrengthenLevel;
                    m_strengthenResult.iTableID = (int)data.TableID;
                    
                    //如果选中的装备是辅助装备
                    if (data.IsAssistEquip())
                    {
                        for (int i = 0; i < m_strengthenResult.assistEquipStrengthenOrgAttr.Length; ++i)
                        {
                            m_strengthenResult.assistEquipStrengthenOrgAttr[i] = aiOrgAttr[i];
                        }
                        m_strengthenResult.assistEquipStrengthenCurAttr[0] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        m_strengthenResult.assistEquipStrengthenCurAttr[1] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        m_strengthenResult.assistEquipStrengthenCurAttr[2] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        m_strengthenResult.assistEquipStrengthenCurAttr[3] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                    }
                    else
                    {
                        for (int i = 0; i < m_strengthenResult.orgAttr.Length; ++i)
                        {
                            m_strengthenResult.orgAttr[i] = aiOrgAttr[i];
                        }
                        m_strengthenResult.curAttr[0] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsAttack));
                        m_strengthenResult.curAttr[1] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicAttack));
                        m_strengthenResult.curAttr[2] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IngoreIndependence));
                        m_strengthenResult.curAttr[3] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsDefense));
                        m_strengthenResult.curAttr[4] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicDefense));
                        m_strengthenResult.curAttr[5] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsDefenseRate));
                        m_strengthenResult.curAttr[6] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicDefenseRate));
                    }
                    
                    UIEventSystem.GetInstance().SendUIEvent(uiEvent);
                }
                else if (msgRet.code == (uint)ProtoErrorCode.ITEM_STRENTH_FAIL_ZERO)
                {
                    Logger.LogProcessFormat("strengthen fail! Level:{0}(to zero)", data.StrengthenLevel);

                    UIEvent uiEvent = UIEventSystem.GetInstance().GetIdleUIEvent();
                    uiEvent.EventID = EUIEventID.ItemStrengthenFail;
                    m_strengthenResult = new StrengthenResult();
                    m_strengthenResult.StrengthenSuccess = false;
                    m_strengthenResult.BrokenItems = new List<ItemData>();
                    m_strengthenResult.EquipData = data;
                    m_strengthenResult.code = msgRet.code;
                    m_strengthenResult.iStrengthenLevel = iOrgStrengthenLevel;
                    m_strengthenResult.iTargetStrengthenLevel = data.StrengthenLevel;
                    m_strengthenResult.iTableID = (int)data.TableID;

                    //如果选中的装备是辅助装备
                    if (data.IsAssistEquip())
                    {
                        for (int i = 0; i < m_strengthenResult.assistEquipStrengthenOrgAttr.Length; ++i)
                        {
                            m_strengthenResult.assistEquipStrengthenOrgAttr[i] = aiOrgAttr[i];
                        }
                        m_strengthenResult.assistEquipStrengthenCurAttr[0] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        m_strengthenResult.assistEquipStrengthenCurAttr[1] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        m_strengthenResult.assistEquipStrengthenCurAttr[2] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        m_strengthenResult.assistEquipStrengthenCurAttr[3] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                    }
                    else
                    {
                        for (int i = 0; i < m_strengthenResult.orgAttr.Length; ++i)
                        {
                            m_strengthenResult.orgAttr[i] = aiOrgAttr[i];
                        }
                        m_strengthenResult.curAttr[0] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsAttack));
                        m_strengthenResult.curAttr[1] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicAttack));
                        m_strengthenResult.curAttr[2] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IngoreIndependence));
                        m_strengthenResult.curAttr[3] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsDefense));
                        m_strengthenResult.curAttr[4] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicDefense));
                        m_strengthenResult.curAttr[5] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsDefenseRate));
                        m_strengthenResult.curAttr[6] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicDefenseRate));
                    }
                   
                    for (int i = 0; i < msgRet.BrokenItems.Count; ++i)
                    {
                        CustomDecoder.RewardItem desc = msgRet.BrokenItems[i];
                        ItemData item = GameClient.ItemDataManager.CreateItemDataFromTable((int)desc.ID);
                        if (item != null)
                        {
                            item.Count = (int)desc.Num;
                            m_strengthenResult.BrokenItems.Add(item);
                        }
                        else
                        {
                            Logger.LogErrorFormat("Item can not find with id = {0}", desc.ID);
                        }
                    }
                    UIEventSystem.GetInstance().SendUIEvent(uiEvent);
                }
                else if (msgRet.code == (uint)ProtoErrorCode.ITEM_STRENTH_FAIL_BROKE)
                {
                    Logger.LogProcessFormat("strengthen fail! broke!!");

                    UIEvent uiEvent = UIEventSystem.GetInstance().GetIdleUIEvent();
                    uiEvent.EventID = EUIEventID.ItemStrengthenFail;
                    m_strengthenResult = new StrengthenResult();
                    m_strengthenResult.StrengthenSuccess = false;
                    m_strengthenResult.BrokenItems = new List<ItemData>();
                    m_strengthenResult.EquipData = data;
                    m_strengthenResult.code = msgRet.code;
                    m_strengthenResult.iStrengthenLevel = iOrgStrengthenLevel;
                    m_strengthenResult.iTargetStrengthenLevel = 0;
                    m_strengthenResult.iTableID = (int)data.TableID;
                    for (int i = 0; i < msgRet.BrokenItems.Count; ++i)
                    {
                        CustomDecoder.RewardItem desc = msgRet.BrokenItems[i];
                        ItemData item = GameClient.ItemDataManager.CreateItemDataFromTable((int)desc.ID);
                        item.Count = (int)desc.Num;
                        m_strengthenResult.BrokenItems.Add(item);
                    }

                    //如果选中的装备是辅助装备
                    if (data.IsAssistEquip())
                    {
                        for (int i = 0; i < m_strengthenResult.assistEquipStrengthenOrgAttr.Length; ++i)
                        {
                            m_strengthenResult.assistEquipStrengthenOrgAttr[i] = aiOrgAttr[i];
                        }
                        m_strengthenResult.assistEquipStrengthenCurAttr[0] = 0;
                        m_strengthenResult.assistEquipStrengthenCurAttr[1] = 0;
                        m_strengthenResult.assistEquipStrengthenCurAttr[2] = 0;
                        m_strengthenResult.assistEquipStrengthenCurAttr[3] = 0;
                    }
                    else
                    {
                        for (int i = 0; i < m_strengthenResult.orgAttr.Length; ++i)
                        {
                            m_strengthenResult.orgAttr[i] = aiOrgAttr[i];
                        }
                        m_strengthenResult.curAttr[0] = 0;
                        m_strengthenResult.curAttr[1] = 0;
                        m_strengthenResult.curAttr[2] = 0;
                        m_strengthenResult.curAttr[3] = 0;
                        m_strengthenResult.curAttr[4] = 0;
                        m_strengthenResult.curAttr[5] = 0;
                        m_strengthenResult.curAttr[6] = 0;
                    }
                    
                    UIEventSystem.GetInstance().SendUIEvent(uiEvent);
                }
                else if (msgRet.code == (uint)ProtoErrorCode.SUCCESS)
                {
                    Logger.LogProcessFormat("strengthen success!!");

                    UIEvent uiEvent = UIEventSystem.GetInstance().GetIdleUIEvent();
                    uiEvent.EventID = EUIEventID.ItemStrengthenSuccess;
                    m_strengthenResult = new StrengthenResult();
                    m_strengthenResult.StrengthenSuccess = true;
                    m_strengthenResult.EquipData = data;
                    m_strengthenResult.code = msgRet.code;
                    m_strengthenResult.iStrengthenLevel = iOrgStrengthenLevel;
                    m_strengthenResult.iTargetStrengthenLevel = data.StrengthenLevel;
                    m_strengthenResult.iTableID = (int)data.TableID;

                    //如果选中的装备是辅助装备
                    if (data.IsAssistEquip())
                    {
                        for (int i = 0; i < m_strengthenResult.assistEquipStrengthenOrgAttr.Length; ++i)
                        {
                            m_strengthenResult.assistEquipStrengthenOrgAttr[i] = aiOrgAttr[i];
                        }
                        m_strengthenResult.assistEquipStrengthenCurAttr[0] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        m_strengthenResult.assistEquipStrengthenCurAttr[1] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        m_strengthenResult.assistEquipStrengthenCurAttr[2] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        m_strengthenResult.assistEquipStrengthenCurAttr[3] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                    }
                    else
                    {
                        for (int i = 0; i < m_strengthenResult.orgAttr.Length; ++i)
                        {
                            m_strengthenResult.orgAttr[i] = aiOrgAttr[i];
                        }
                        m_strengthenResult.curAttr[0] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsAttack));
                        m_strengthenResult.curAttr[1] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicAttack));
                        m_strengthenResult.curAttr[2] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IngoreIndependence));
                        m_strengthenResult.curAttr[3] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsDefense));
                        m_strengthenResult.curAttr[4] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicDefense));
                        m_strengthenResult.curAttr[5] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsDefenseRate));
                        m_strengthenResult.curAttr[6] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicDefenseRate));
                    }
                   
                    UIEventSystem.GetInstance().SendUIEvent(uiEvent);
                }
                else
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.code);
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnStrengthenError);
                }
            });
        }

        bool m_bIsStrenghtenContinue = false;
        public bool IsStrengthenContinue
        {
            get
            {
                return m_bIsStrenghtenContinue;
            }
            set
            {
                m_bIsStrenghtenContinue = value;
            }
        }

        public StrengthenResult GetStrengthenResult()
        {
            return m_strengthenResult;
        }

        /// <summary>
        /// 得到装备可以使用的强化券
        /// </summary>
        /// <param name="currentSelectItemData"></param>
        /// <returns></returns>
        public List<ItemData> GetStrengthenStampList(ItemData currentSelectItemData)
        {
            if (currentSelectItemData == null)
            {
                return new List<ItemData>();
            }

            List<ItemData> items = new List<ItemData>();
            var datas = ItemDataManager.GetInstance().GetItemsByPackageSubType(EPackageType.Material, ItemTable.eSubType.Coupon);
            if (datas != null)
            {
                for (int i = 0; i < datas.Count; ++i)
                {
                    var itemData = ItemDataManager.GetInstance().GetItem(datas[i]);
                    if (itemData == null)
                    {
                        continue;
                    }

                    var mItemTable = TableManager.GetInstance().GetTableItem<ItemTable>((int)itemData.TableID);
                    if (mItemTable == null)
                    {
                        continue;
                    }

                    var ticketItem = TableManager.GetInstance().GetTableItem<ProtoTable.StrengthenTicketTable>(mItemTable.StrenTicketDataIndex);
                    if (ticketItem == null)
                    {
                        continue;
                    }

                    if (currentSelectItemData.StrengthenLevel >= ticketItem.Level)
                    {
                        continue;
                    }

                    items.Add(itemData);
                }
            }

            items.Sort(StrengthenStampItemSort);

            return items;
        }

        private int StrengthenStampItemSort(ItemData x, ItemData y)
        {
            var leftItemTable = TableManager.GetInstance().GetTableItem<ItemTable>((int)x.TableID);
            var rightItemTable = TableManager.GetInstance().GetTableItem<ItemTable>((int)y.TableID);
            var left = TableManager.GetInstance().GetTableItem<ProtoTable.StrengthenTicketTable>(leftItemTable.StrenTicketDataIndex);
            var right = TableManager.GetInstance().GetTableItem<ProtoTable.StrengthenTicketTable>(rightItemTable.StrenTicketDataIndex);

            if (left.Level != right.Level)
            {
                return right.Level - left.Level;
            }

            if (left.Probility != right.Probility)
            {
                return right.Probility - left.Probility;
            }

            return left.ID - right.ID;
        }


        public List<ComControlData> GetQualiyDataList()
        {
            List<ComControlData> tabDataList = new List<ComControlData>();

            for (int i = 0; i <= (int)ItemTable.eColor.YELLOW; i++)
            {
                int index = i;
                string name = EnchantmentsCardManager.GetQualityName((ItemTable.eColor)i);
                CommonQualityTabData tabData = new CommonQualityTabData(index, index, name, index == 0);
                tabDataList.Add(tabData);
            }

            return tabDataList;
        }

        public List<ComControlData> GetLevelDataList()
        {
            List<ComControlData> tabDataList = new List<ComControlData>();
            
            var enumerator = TableManager.GetInstance().GetTable<SmithShopFilterTable>().GetEnumerator();
            while (enumerator.MoveNext())
            {
                var table = enumerator.Current.Value as SmithShopFilterTable;
                if (table == null)
                    continue;

                CommonLevelTabData commonLevelTabData = new CommonLevelTabData(table.ID, table.ID, table.Name, table.ID == 1);
                tabDataList.Add(commonLevelTabData);
            }

            return tabDataList;
        }
    }
}
