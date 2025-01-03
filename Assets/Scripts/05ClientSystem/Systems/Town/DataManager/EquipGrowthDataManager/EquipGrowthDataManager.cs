using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoTable;
using Protocol;
using Network;

namespace GameClient
{
    /// <summary>
    /// 增幅消耗数据
    /// </summary>
    public class GrowthCostData
    {
        public int quality;
        public int growthLevel;
        public int equipLevel;
        public List<ItemSimpleData> growthCosts;
        public GrowthCostData(int quality,int growthLevel,int equipLevel,List<ItemSimpleData> growthCosts)
        {
            this.quality = quality;
            this.growthLevel = growthLevel;
            this.equipLevel = equipLevel;
            this.growthCosts = growthCosts;
        }
    }

    /// <summary>
    /// 增幅属性数据
    /// </summary>
    public class GrowthAttributeData
    {
        public int quality;
        public int growthLevel;
        public int equipLevel;
        public int growthAttributeNum;
        public GrowthAttributeData(int quality,int growthLevel,int equipLevel,int growthAttributeNum)
        {
            this.quality = quality;
            this.growthLevel = growthLevel;
            this.equipLevel = equipLevel;
            this.growthAttributeNum = growthAttributeNum;
        }
    }

    /// <summary>
    /// 材料合成数据
    /// </summary>
    public class MaterialsSynthesisData
    {
        public int tableID;
        public List<ItemSimpleData> expendMaterials;
    }

    public class EquipGrowthDataManager : DataManager<EquipGrowthDataManager>
    {

        List<GrowthCostData> mGrowthCostDataList = new List<GrowthCostData>();
        List<GrowthAttributeData> mGrowthAttributeDataList = new List<GrowthAttributeData>();
        List<MaterialsSynthesisData> mMaterialsSynthesisDataList = new List<MaterialsSynthesisData>();
        StrengthenResult mGrowthResult;

        public bool IsEquipIntensify = false;           //装备被激化的标志

        /// <summary>
        /// 装备激化是否破碎
        /// </summary>
        public bool IsEquipmentIntensifyBroken = false;

        public sealed override void Clear()
        {
            ClearGrowthAttributeDataList();
            ClearGrowthCostDataList();
            ClearMaterialsSynthesisDataList();
            IsEquipIntensify = false;
            IsEquipmentIntensifyBroken = false;
        }

        public sealed override void Initialize()
        {
            InitEquipEnhanceAttributeTable();
            InitEquipEnhanceCostTable();
            InitMaterialsSynthesisTable();
        }

        private void ClearMaterialsSynthesisDataList()
        {
            if (mMaterialsSynthesisDataList != null)
            {
                mMaterialsSynthesisDataList.Clear();
            }
        }

        private void ClearGrowthCostDataList()
        {
            if (mGrowthCostDataList != null)
            {
                mGrowthCostDataList.Clear();
            }
        }

        private void ClearGrowthAttributeDataList()
        {
            if (mGrowthAttributeDataList != null)
            {
                mGrowthAttributeDataList.Clear();
            }
        }

        public StrengthenResult GetGrowthResult()
        {
            return mGrowthResult;
        }
        

        public List<ItemSimpleData> GetCostMaterialsList(int itemId)
        {
            for (int i = 0; i < mMaterialsSynthesisDataList.Count; i++)
            {
                if (mMaterialsSynthesisDataList[i].tableID != itemId)
                {
                    continue;
                }

                return mMaterialsSynthesisDataList[i].expendMaterials;
            }

            return new List<ItemSimpleData>();
        }

        public List<MaterialsSynthesisData> GetMaterialsSynthesisData()
        {
            return mMaterialsSynthesisDataList;
        }

        private void InitMaterialsSynthesisTable()
        {
            if (mMaterialsSynthesisDataList == null)
            {
                mMaterialsSynthesisDataList = new List<MaterialsSynthesisData>();
            }
            else
            {
                mMaterialsSynthesisDataList.Clear();
            }

            var tableData = TableManager.GetInstance().GetTable<MaterialsSynthesis>().GetEnumerator();
            while(tableData.MoveNext())
            {
                var table = tableData.Current.Value as MaterialsSynthesis;
                if (table == null)
                {
                    continue;
                }

                MaterialsSynthesisData data = new MaterialsSynthesisData();
                data.expendMaterials = new List<ItemSimpleData>();

                data.tableID = table.ID;
                string[] composites = table.Composites.Split('|');
                for (int i = 0; i < composites.Length; i++)
                {
                    int id = 0;
                    int count = 0;
                    string[] strs = composites[i].Split('_');
                    if (strs.Length >= 2)
                    {
                        int.TryParse(strs[0], out id);
                        int.TryParse(strs[1], out count);

                        ItemSimpleData itemSimpleData = new ItemSimpleData(id, count);
                        data.expendMaterials.Add(itemSimpleData);
                    }
                }

                mMaterialsSynthesisDataList.Add(data);
            }
        }
        

        private void InitEquipEnhanceCostTable()
        {
            ClearGrowthCostDataList();
            var tableData = TableManager.GetInstance().GetTable<EquipEnhanceCostTable>().GetEnumerator();
            while (tableData.MoveNext())
            {
                var table = tableData.Current.Value as EquipEnhanceCostTable;
                if (table == null)
                {
                    continue;
                }
                
                List<ItemSimpleData> costs = new List<ItemSimpleData>();

                ItemSimpleData simpleData1 = new ItemSimpleData();
                simpleData1.ItemID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.BindGOLD);
                simpleData1.Count = table.NeedGold;

                ItemSimpleData simpleData2 = new ItemSimpleData();
                simpleData2.ItemID = table.ItemID;
                simpleData2.Count = table.ItemNum;

                costs.Add(simpleData1);
                costs.Add(simpleData2);

                GrowthCostData growthCostData = new GrowthCostData(table.Quality, table.EnhanceLevel, table.Level, costs);

                mGrowthCostDataList.Add(growthCostData);
            }
        }

        private void InitEquipEnhanceAttributeTable()
        {
            ClearGrowthAttributeDataList();
            var equipEnhanceAttributeTable = TableManager.GetInstance().GetTable<EquipEnhanceAttributeTable>().GetEnumerator();
            while (equipEnhanceAttributeTable.MoveNext())
            {
                var table = equipEnhanceAttributeTable.Current.Value as EquipEnhanceAttributeTable;
                if (table == null)
                {
                    continue;
                }

                GrowthAttributeData mGrowthAttributeData = new GrowthAttributeData(table.Quality, table.EnhanceLevel, table.Level, table.EnhanceAttribute);
                mGrowthAttributeDataList.Add(mGrowthAttributeData);
            }
        }

        /// <summary>
        /// 得到装备增幅需要消耗的材料
        /// </summary>
        /// <param name="itemData"></param>
        /// <returns></returns>
        public List<ItemSimpleData> GetGrowthCosts(ItemData itemData)
        {
            if (itemData == null)
            {
                return new List<ItemSimpleData>();
            }

            for (int i = 0; i < mGrowthCostDataList.Count; i++)
            {
               var data = mGrowthCostDataList[i];
               if ((int)itemData.Quality != data.quality)
               {
                    continue;
               }
               
               if (itemData.StrengthenLevel != data.growthLevel)
               {
                    continue;
               }

               if (itemData.LevelLimit != data.equipLevel)
               {
                    continue;
               }

                return data.growthCosts;
            }

            return new List<ItemSimpleData>();
        }

        public int GetGrowthAttributeValue(ItemData itemData, int growthLevel)
        {
            if (itemData == null)
            {
                return 0;
            }

            for (int i = 0; i < mGrowthAttributeDataList.Count; i++)
            {
                var data = mGrowthAttributeDataList[i];
                if (data == null)
                {
                    continue;
                }

                if (data.quality != (int)itemData.Quality)
                {
                    continue;
                }

                if (data.equipLevel != itemData.LevelLimit)
                {
                    continue;
                }

                if (data.growthLevel != growthLevel)
                {
                    continue;
                }

                return data.growthAttributeNum;
            }

            return 0;
        }

        /// <summary>
        /// 得到装备可以使用的增幅券
        /// </summary>
        /// <param name="currentSelectItemData"></param>
        /// <returns></returns>
        public List<ItemData> GetGrowthStampList(ItemData currentSelectItemData)
        {
            if (currentSelectItemData == null)
            {
                return new List<ItemData>();
            }

            List<ItemData> items = new List<ItemData>();
            var datas = ItemDataManager.GetInstance().GetItemsByPackageSubType(EPackageType.Material, ItemTable.eSubType.ST_ZENGFU_AMPLIFICATION);
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

            items.Sort(GrowthStampItemSort);

            return items;
        }

        private int GrowthStampItemSort(ItemData x,ItemData y)
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

        /// <summary>
        /// 请求增幅
        /// </summary>
        public void SceneEquipEnhanceUpgrade(ItemData data, byte useUnbreak, ulong ticketid = 0)
        {
            SceneEquipEnhanceUpgrade msg = new SceneEquipEnhanceUpgrade();
            msg.euqipUid = data.GUID;
            msg.strTickt = ticketid;
            msg.useUnbreak = useUnbreak;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);
            int iOrgStrengthenLevel = data.StrengthenLevel + 1;

            int[] aiOrgAttr;

            //如果选中的装备是辅助装备
            if (data.IsAssistEquip())
            {
                aiOrgAttr = new int[5]
                {
                    Mathf.FloorToInt(data.GrowthAttrNum),
                    Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel)),
                    Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel)),
                    Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel)),
                    Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel)),
            };
            }
            else
            {
                aiOrgAttr = new int[8]
                           {
                Mathf.FloorToInt(data.GrowthAttrNum),
                Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsAttack)),
                Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicAttack)),
                Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IngoreIndependence)),
                Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsDefense)),
                Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicDefense)),
                Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsDefenseRate)),
                Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicDefenseRate)),
                           };
            }

            IsEquipIntensify = true;

            WaitNetMessageManager.GetInstance().Wait(SceneEquipEnhanceUpgradeRet.MsgID, msgData =>
            {
                CustomDecoder.StrengthenRet msgRet;
                int pos = 0;
                CustomDecoder.DecodeStrengthenResult(out msgRet, msgData.bytes, ref pos, msgData.bytes.Length);

                if (msgRet.code == (uint)ProtoErrorCode.ITEM_STRENTH_FAIL)
                {
                    Logger.LogProcessFormat("strengthen fail! Level:{0}", data.StrengthenLevel);

                    UIEvent uiEvent = UIEventSystem.GetInstance().GetIdleUIEvent();
                    uiEvent.EventID = EUIEventID.ItemGrowthFail;
                    mGrowthResult = new StrengthenResult();
                    mGrowthResult.StrengthenSuccess = false;
                    mGrowthResult.EquipData = data;
                    mGrowthResult.code = msgRet.code;
                    mGrowthResult.iStrengthenLevel = iOrgStrengthenLevel;
                    mGrowthResult.iTargetStrengthenLevel = data.StrengthenLevel;
                    mGrowthResult.iTableID = (int)data.TableID;

                    //如果选中的装备是辅助装备
                    if (data.IsAssistEquip())
                    {
                        for (int i = 0; i < mGrowthResult.assistEquipGrowthOrgAttr.Length; ++i)
                        {
                            mGrowthResult.assistEquipGrowthOrgAttr[i] = aiOrgAttr[i];
                        }
                        mGrowthResult.assistEquipGrowthCurAttr[0] = Mathf.FloorToInt(data.GrowthAttrNum);
                        mGrowthResult.assistEquipGrowthCurAttr[1] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        mGrowthResult.assistEquipGrowthCurAttr[2] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        mGrowthResult.assistEquipGrowthCurAttr[3] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        mGrowthResult.assistEquipGrowthCurAttr[4] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                    }
                    else
                    {
                        for (int i = 0; i < mGrowthResult.growthOrgAttr.Length; ++i)
                        {
                            mGrowthResult.growthOrgAttr[i] = aiOrgAttr[i];
                        }
                        mGrowthResult.growthCurAttr[0] = Mathf.FloorToInt(data.GrowthAttrNum);
                        mGrowthResult.growthCurAttr[1] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsAttack));
                        mGrowthResult.growthCurAttr[2] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicAttack));
                        mGrowthResult.growthCurAttr[3] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IngoreIndependence));
                        mGrowthResult.growthCurAttr[4] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsDefense));
                        mGrowthResult.growthCurAttr[5] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicDefense));
                        mGrowthResult.growthCurAttr[6] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsDefenseRate));
                        mGrowthResult.growthCurAttr[7] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicDefenseRate));
                    }
                    
                    UIEventSystem.GetInstance().SendUIEvent(uiEvent);
                }
                else if (msgRet.code == (uint)ProtoErrorCode.ITEM_SPECIAL_STRENTH_FAIL)
                {
                    UIEvent uiEvent = UIEventSystem.GetInstance().GetIdleUIEvent();
                    uiEvent.EventID = EUIEventID.OnSpecailGrowthFailed;
                    mGrowthResult = new StrengthenResult();
                    mGrowthResult.StrengthenSuccess = false;
                    mGrowthResult.EquipData = data;
                    mGrowthResult.code = msgRet.code;
                    mGrowthResult.iStrengthenLevel = iOrgStrengthenLevel;
                    mGrowthResult.iTargetStrengthenLevel = data.StrengthenLevel;
                    mGrowthResult.iTableID = (int)data.TableID;

                    //如果选中的装备是辅助装备
                    if (data.IsAssistEquip())
                    {
                        for (int i = 0; i < mGrowthResult.assistEquipGrowthOrgAttr.Length; ++i)
                        {
                            mGrowthResult.assistEquipGrowthOrgAttr[i] = aiOrgAttr[i];
                        }
                        mGrowthResult.assistEquipGrowthCurAttr[0] = Mathf.FloorToInt(data.GrowthAttrNum);
                        mGrowthResult.assistEquipGrowthCurAttr[1] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        mGrowthResult.assistEquipGrowthCurAttr[2] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        mGrowthResult.assistEquipGrowthCurAttr[3] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        mGrowthResult.assistEquipGrowthCurAttr[4] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                    }
                    else
                    {
                        for (int i = 0; i < mGrowthResult.growthOrgAttr.Length; ++i)
                        {
                            mGrowthResult.growthOrgAttr[i] = aiOrgAttr[i];
                        }
                        mGrowthResult.growthCurAttr[0] = Mathf.FloorToInt(data.GrowthAttrNum);
                        mGrowthResult.growthCurAttr[1] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsAttack));
                        mGrowthResult.growthCurAttr[2] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicAttack));
                        mGrowthResult.growthCurAttr[3] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IngoreIndependence));
                        mGrowthResult.growthCurAttr[4] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsDefense));
                        mGrowthResult.growthCurAttr[5] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicDefense));
                        mGrowthResult.growthCurAttr[6] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsDefenseRate));
                        mGrowthResult.growthCurAttr[7] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicDefenseRate));
                    }
                    
                    UIEventSystem.GetInstance().SendUIEvent(uiEvent);
                }
                else if (msgRet.code == (uint)ProtoErrorCode.ITEM_STRENTH_FAIL_MINUS)
                {
                    Logger.LogProcessFormat("strengthen fail! Level:{0}(-1)", data.StrengthenLevel);

                    UIEvent uiEvent = UIEventSystem.GetInstance().GetIdleUIEvent();
                    uiEvent.EventID = EUIEventID.ItemGrowthFail;
                    mGrowthResult = new StrengthenResult();
                    mGrowthResult.StrengthenSuccess = false;
                    mGrowthResult.EquipData = data;
                    mGrowthResult.code = msgRet.code;
                    mGrowthResult.iStrengthenLevel = iOrgStrengthenLevel;
                    mGrowthResult.iTargetStrengthenLevel = data.StrengthenLevel;
                    mGrowthResult.iTableID = (int)data.TableID;

                    //如果选中的装备是辅助装备
                    if (data.IsAssistEquip())
                    {
                        for (int i = 0; i < mGrowthResult.assistEquipGrowthOrgAttr.Length; ++i)
                        {
                            mGrowthResult.assistEquipGrowthOrgAttr[i] = aiOrgAttr[i];
                        }
                        mGrowthResult.assistEquipGrowthCurAttr[0] = Mathf.FloorToInt(data.GrowthAttrNum);
                        mGrowthResult.assistEquipGrowthCurAttr[1] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        mGrowthResult.assistEquipGrowthCurAttr[2] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        mGrowthResult.assistEquipGrowthCurAttr[3] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        mGrowthResult.assistEquipGrowthCurAttr[4] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                    }
                    else
                    {
                        for (int i = 0; i < mGrowthResult.growthOrgAttr.Length; ++i)
                        {
                            mGrowthResult.growthOrgAttr[i] = aiOrgAttr[i];
                        }
                        mGrowthResult.growthCurAttr[0] = Mathf.FloorToInt(data.GrowthAttrNum);
                        mGrowthResult.growthCurAttr[1] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsAttack));
                        mGrowthResult.growthCurAttr[2] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicAttack));
                        mGrowthResult.growthCurAttr[3] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IngoreIndependence));
                        mGrowthResult.growthCurAttr[4] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsDefense));
                        mGrowthResult.growthCurAttr[5] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicDefense));
                        mGrowthResult.growthCurAttr[6] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsDefenseRate));
                        mGrowthResult.growthCurAttr[7] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicDefenseRate));
                    }
                    
                    UIEventSystem.GetInstance().SendUIEvent(uiEvent);
                }
                else if (msgRet.code == (uint)ProtoErrorCode.ITEM_STRENTH_FAIL_ZERO)
                {
                    Logger.LogProcessFormat("strengthen fail! Level:{0}(to zero)", data.StrengthenLevel);

                    UIEvent uiEvent = UIEventSystem.GetInstance().GetIdleUIEvent();
                    uiEvent.EventID = EUIEventID.ItemGrowthFail;
                    mGrowthResult = new StrengthenResult();
                    mGrowthResult.StrengthenSuccess = false;
                    mGrowthResult.BrokenItems = new List<ItemData>();
                    mGrowthResult.EquipData = data;
                    mGrowthResult.code = msgRet.code;
                    mGrowthResult.iStrengthenLevel = iOrgStrengthenLevel;
                    mGrowthResult.iTargetStrengthenLevel = data.StrengthenLevel;
                    mGrowthResult.iTableID = (int)data.TableID;

                    //如果选中的装备是辅助装备
                    if (data.IsAssistEquip())
                    {
                        for (int i = 0; i < mGrowthResult.assistEquipGrowthOrgAttr.Length; ++i)
                        {
                            mGrowthResult.assistEquipGrowthOrgAttr[i] = aiOrgAttr[i];
                        }
                        mGrowthResult.assistEquipGrowthCurAttr[0] = Mathf.FloorToInt(data.GrowthAttrNum);
                        mGrowthResult.assistEquipGrowthCurAttr[1] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        mGrowthResult.assistEquipGrowthCurAttr[2] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        mGrowthResult.assistEquipGrowthCurAttr[3] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        mGrowthResult.assistEquipGrowthCurAttr[4] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                    }
                    else
                    {
                        for (int i = 0; i < mGrowthResult.growthOrgAttr.Length; ++i)
                        {
                            mGrowthResult.growthOrgAttr[i] = aiOrgAttr[i];
                        }
                        mGrowthResult.growthCurAttr[0] = Mathf.FloorToInt(data.GrowthAttrNum);
                        mGrowthResult.growthCurAttr[1] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsAttack));
                        mGrowthResult.growthCurAttr[2] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicAttack));
                        mGrowthResult.growthCurAttr[3] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IngoreIndependence));
                        mGrowthResult.growthCurAttr[4] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsDefense));
                        mGrowthResult.growthCurAttr[5] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicDefense));
                        mGrowthResult.growthCurAttr[6] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsDefenseRate));
                        mGrowthResult.growthCurAttr[7] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicDefenseRate));
                    }
                    
                    for (int i = 0; i < msgRet.BrokenItems.Count; ++i)
                    {
                        CustomDecoder.RewardItem desc = msgRet.BrokenItems[i];
                        ItemData item = GameClient.ItemDataManager.CreateItemDataFromTable((int)desc.ID);
                        if (item != null)
                        {
                            item.Count = (int)desc.Num;
                            mGrowthResult.BrokenItems.Add(item);
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
                    uiEvent.EventID = EUIEventID.ItemGrowthFail;
                    mGrowthResult = new StrengthenResult();
                    mGrowthResult.StrengthenSuccess = false;
                    mGrowthResult.BrokenItems = new List<ItemData>();
                    mGrowthResult.EquipData = data;
                    mGrowthResult.code = msgRet.code;
                    mGrowthResult.iStrengthenLevel = iOrgStrengthenLevel;
                    mGrowthResult.iTargetStrengthenLevel = 0;
                    mGrowthResult.iTableID = (int)data.TableID;
                    for (int i = 0; i < msgRet.BrokenItems.Count; ++i)
                    {
                        CustomDecoder.RewardItem desc = msgRet.BrokenItems[i];
                        ItemData item = GameClient.ItemDataManager.CreateItemDataFromTable((int)desc.ID);
                        if (item != null)
                        {
                            item.Count = (int)desc.Num;
                        }
                       
                        mGrowthResult.BrokenItems.Add(item);
                    }

                    //如果选中的装备是辅助装备
                    if (data.IsAssistEquip())
                    {
                        for (int i = 0; i < mGrowthResult.assistEquipGrowthOrgAttr.Length; ++i)
                        {
                            mGrowthResult.assistEquipGrowthOrgAttr[i] = aiOrgAttr[i];
                        }
                        mGrowthResult.assistEquipGrowthCurAttr[0] = 0;
                        mGrowthResult.assistEquipGrowthCurAttr[1] = 0;
                        mGrowthResult.assistEquipGrowthCurAttr[2] = 0;
                        mGrowthResult.assistEquipGrowthCurAttr[3] = 0;
                        mGrowthResult.assistEquipGrowthCurAttr[4] = 0;
                    }
                    else
                    {
                        for (int i = 0; i < mGrowthResult.growthOrgAttr.Length; ++i)
                        {
                            mGrowthResult.growthOrgAttr[i] = aiOrgAttr[i];
                        }
                        mGrowthResult.growthCurAttr[0] = 0;
                        mGrowthResult.growthCurAttr[1] = 0;
                        mGrowthResult.growthCurAttr[2] = 0;
                        mGrowthResult.growthCurAttr[3] = 0;
                        mGrowthResult.growthCurAttr[4] = 0;
                        mGrowthResult.growthCurAttr[5] = 0;
                        mGrowthResult.growthCurAttr[6] = 0;
                    }

                    UIEventSystem.GetInstance().SendUIEvent(uiEvent);
                }
                else if (msgRet.code == (uint)ProtoErrorCode.SUCCESS)
                {
                    Logger.LogProcessFormat("strengthen success!!");

                    UIEvent uiEvent = UIEventSystem.GetInstance().GetIdleUIEvent();
                    uiEvent.EventID = EUIEventID.ItemGrowthSuccess;
                    mGrowthResult = new StrengthenResult();
                    mGrowthResult.StrengthenSuccess = true;
                    mGrowthResult.EquipData = data;
                    mGrowthResult.code = msgRet.code;
                    mGrowthResult.iStrengthenLevel = iOrgStrengthenLevel;
                    mGrowthResult.iTargetStrengthenLevel = data.StrengthenLevel;
                    mGrowthResult.iTableID = (int)data.TableID;

                    //如果选中的装备是辅助装备
                    if (data.IsAssistEquip())
                    {
                        for (int i = 0; i < mGrowthResult.assistEquipGrowthOrgAttr.Length; ++i)
                        {
                            mGrowthResult.assistEquipGrowthOrgAttr[i] = aiOrgAttr[i];
                        }
                        mGrowthResult.assistEquipGrowthCurAttr[0] = Mathf.FloorToInt(data.GrowthAttrNum);
                        mGrowthResult.assistEquipGrowthCurAttr[1] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        mGrowthResult.assistEquipGrowthCurAttr[2] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        mGrowthResult.assistEquipGrowthCurAttr[3] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                        mGrowthResult.assistEquipGrowthCurAttr[4] = Mathf.FloorToInt(data.GetBaseFourAttrStrengthenAddUpValue(data.StrengthenLevel));
                    }
                    else
                    {
                        for (int i = 0; i < mGrowthResult.growthOrgAttr.Length; ++i)
                        {
                            mGrowthResult.growthOrgAttr[i] = aiOrgAttr[i];
                        }
                        mGrowthResult.growthCurAttr[0] = Mathf.FloorToInt(data.GrowthAttrNum);
                        mGrowthResult.growthCurAttr[1] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsAttack));
                        mGrowthResult.growthCurAttr[2] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicAttack));
                        mGrowthResult.growthCurAttr[3] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IngoreIndependence));
                        mGrowthResult.growthCurAttr[4] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsDefense));
                        mGrowthResult.growthCurAttr[5] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicDefense));
                        mGrowthResult.growthCurAttr[6] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnorePhysicsDefenseRate));
                        mGrowthResult.growthCurAttr[7] = Mathf.FloorToInt(data._GetGetStrengthenDescs(EEquipProp.IgnoreMagicDefenseRate));
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
        
        /// <summary>
        /// 气息装备激活变成红字装备
        /// </summary>
        public void OnSceneEquipEnhanceRed(ItemData itemData, UInt32 expendId)
        {
            SceneEquipEnhanceRed req = new SceneEquipEnhanceRed();
            req.euqipUid = itemData.GUID;
            req.stuffID = expendId;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);

            WaitNetMessageManager.GetInstance().Wait(SceneEquipEnhanceRedRet.MsgID, msgData =>
            {
                SceneEquipEnhanceRedRet ret = new SceneEquipEnhanceRedRet();
                ret.decode(msgData.bytes);

                if (ret.code != 0)
                {
                    SystemNotifyManager.SystemNotify((int)ret.code);
                }
                else
                {
                    EquipmentClearChangeActivationResultData data = new EquipmentClearChangeActivationResultData();
                    data.mEquipItemData = itemData;
                    data.mStrengthenGrowthType = StrengthenGrowthType.SGT_Activate;

                    OpenClearActivationChangedResultFrame(data);

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnBreathEquipActivationSuccess);
                }
            });
        }

        /// <summary>
        /// 气息装备清除气息
        /// </summary>
        public void OnSceneEquipEnhanceClear(ItemData itemData, UInt32 expendId)
        {
            SceneEquipEnhanceClear req = new SceneEquipEnhanceClear();
            req.euqipUid = itemData.GUID;
            req.stuffID = expendId;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);

            WaitNetMessageManager.GetInstance().Wait(SceneEquipEnhanceClearRet.MsgID, msgData =>
            {
                SceneEquipEnhanceClearRet ret = new SceneEquipEnhanceClearRet();
                ret.decode(msgData.bytes);
                if (ret.code != 0)
                {
                    SystemNotifyManager.SystemNotify((int)ret.code);
                }
                else
                {
                    EquipmentClearChangeActivationResultData data = new EquipmentClearChangeActivationResultData();
                    data.mEquipItemData = itemData;
                    data.mStrengthenGrowthType = StrengthenGrowthType.SGT_Clear;

                    OpenClearActivationChangedResultFrame(data);

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnBreathEquipClearSuccess);
                }
            });
        }

        /// <summary>
        /// scene->client红字装备属性转化
        /// </summary>
        public void OnSceneEquipEnhanceChg(ItemData itemData, UInt32 expendId,byte enhanceType)
        {
            SceneEquipEnhanceChg req = new SceneEquipEnhanceChg();
            req.euqipUid = itemData.GUID;
            req.stuffID = expendId;
            req.enhanceType = enhanceType;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);

            WaitNetMessageManager.GetInstance().Wait(SceneEquipEnhanceChgRet.MsgID, msgData =>
            {
                SceneEquipEnhanceChgRet ret = new SceneEquipEnhanceChgRet();
                ret.decode(msgData.bytes);

                if (ret.code != 0)
                {
                    SystemNotifyManager.SystemNotify((int)ret.code);
                }
                else
                {
                    EquipmentClearChangeActivationResultData data = new EquipmentClearChangeActivationResultData();
                    data.mEquipItemData = itemData;
                    data.mStrengthenGrowthType = StrengthenGrowthType.SGT_Change;

                    OpenClearActivationChangedResultFrame(data);

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRedMarkEquipChangedSuccess);
                }
            });
        }

        /// <summary>
        /// 材料合成
        /// </summary>
        /// <param name="goalId"></param>
        /// <param name="goalNum"></param>
        public void OnSceneEnhanceMaterialCombo(UInt32 goalId, UInt32 goalNum)
        {
            SceneEnhanceMaterialCombo req = new SceneEnhanceMaterialCombo();
            req.goalId = goalId;
            req.goalNum = goalNum;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);

            WaitNetMessageManager.GetInstance().Wait(SceneEnhanceMaterialComboRet.MsgID, msgData =>
            {
                SceneEnhanceMaterialComboRet ret = new SceneEnhanceMaterialComboRet();
                ret.decode(msgData.bytes);

                if (ret.code != 0)
                {
                    SystemNotifyManager.SystemNotify((int)ret.code);
                }
                else
                {
                    MaterialsSynthesisResultData data = new MaterialsSynthesisResultData();
                    data.itemId = (int)goalId;
                    data.itemNumber = (int)goalNum;

                    OpenClearActivationChangedResultFrame(data);
                }
            });
        }

        private void OpenClearActivationChangedResultFrame(EquipmentClearChangeActivationResultData data)
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<EnchantResultFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<EnchantResultFrame>();
            }

            ClientSystemManager.GetInstance().OpenFrame<EnchantResultFrame>(FrameLayer.Middle, data);
        }

        private void OpenClearActivationChangedResultFrame(MaterialsSynthesisResultData data)
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<EnchantResultFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<EnchantResultFrame>();
            }

            ClientSystemManager.GetInstance().OpenFrame<EnchantResultFrame>(FrameLayer.Middle, data);
        }

        /// <summary>
        /// 根据装备属性类型得到描述
        /// </summary>
        /// <returns></returns>
        public string GetGrowthAttrDesc(EGrowthAttrType type)
        {
            string content = "";
            switch (type)
            {
                case EGrowthAttrType.GAT_NONE:
                    break;
                case EGrowthAttrType.GAT_STRENGTH:
                    content = TR.Value("growth_attr_strength");
                    break;
                case EGrowthAttrType.GAT_INTELLIGENCE:
                    content = TR.Value("growth_attr_intelligence");
                    break;
                case EGrowthAttrType.GAT_STAMINA:
                    content = TR.Value("growth_attr_stamina");
                    break;
                case EGrowthAttrType.GAT_SPIRIT:
                    content = TR.Value("growth_attr_spirit");
                    break;
                default:
                    break;
            }

            return content;
        }
        
        /// <summary>
        /// 强制关闭锻冶界面和与锻冶相关的界面
        /// </summary>
        public static void MandatoryCloseSmithshopNewFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<SmithShopNewFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<SmithShopNewFrame>();
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<StrengthenResultFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<StrengthenResultFrame>();
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<StrengthenContinueResultsFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<StrengthenContinueResultsFrame>();
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<StrengthenContinueConfirm>())
            {
                ClientSystemManager.GetInstance().CloseFrame<StrengthenContinueConfirm>();
            }
        }
    }
}