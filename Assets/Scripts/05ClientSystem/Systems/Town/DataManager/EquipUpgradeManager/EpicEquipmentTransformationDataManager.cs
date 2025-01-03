using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoTable;
using Network;
using Protocol;

namespace GameClient
{
    /// <summary>
    /// 史诗装备转化类型
    /// </summary>
    public enum EpicEquipmentTransformationType
    {
        None = 0,
        /// <summary>
        /// 同套转化
        /// </summary>
        SetOfConversion,
        /// <summary>
        /// 跨套转化
        /// </summary>
        AcrossaSetOfConversion,
    }

    public class EpicEquipmentTransformationSuccessData
    {
        public ItemData itemData;
    }

    /// <summary>
    /// 史诗装备转化数据管理器
    /// </summary>
    public class EpicEquipmentTransformationDataManager : DataManager<EpicEquipmentTransformationDataManager>
    {
        /// <summary>
        /// 史诗转换表格数据
        /// </summary>
        private List<EquieChangeTable> mEquieChangeTableList = new List<EquieChangeTable>();

        private List<EquChangeConsumeTable> mEquChangeConsumeTableList = new List<EquChangeConsumeTable>();
        
        public sealed override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.Default;
        }

        public sealed override void Clear()
        {
            _UnRegisterNetHandler();
            UnbindEvent();
            mEquieChangeTableList.Clear();
            mEquChangeConsumeTableList.Clear();
        }

        public sealed override void Initialize()
        {
            OnInitEquieChangeTable();
            OnInitEquChangeConsumeTable();
            _RegisterNetHandler();
            BindEvent();
        }

        private void BindEvent()
        {
           
        }

        private void UnbindEvent()
        {
            
        }
        
        private void _RegisterNetHandler()
        {
            NetProcess.AddMsgHandler(SceneEquipConvertRes.MsgID, OnSceneEquipConvertRes);
        }

        private void _UnRegisterNetHandler()
        {
            NetProcess.RemoveMsgHandler(SceneEquipConvertRes.MsgID, OnSceneEquipConvertRes);
        }

        private void OnSceneEquipConvertRes(MsgDATA msg)
        {
            SceneEquipConvertRes res = new SceneEquipConvertRes();
            res.decode(msg.bytes);

            if (res.retCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.retCode);
            }
            else
            {
                ItemData mItemData = ItemDataManager.GetInstance().GetItem(res.eqGuid);
                if (mItemData == null)
                {
                    Logger.LogErrorFormat("装备转化返回装备GUID有误");
                    return;
                }

                mItemData.TableData = TableManager.GetInstance().GetTableItem<ItemTable>(mItemData.TableID);
                mItemData.ThirdType = mItemData.TableData.ThirdType;
                mItemData.SubType = (int)mItemData.TableData.SubType;
                mItemData.EquipWearSlotType = (EEquipWearSlotType)mItemData.TableData.SubType;
                if (mItemData.MagicProp != null)
                {
                    mItemData.MagicProp.ResetProperties();
                }

                if (mItemData.BeadProp != null)
                {
                    mItemData.BeadProp.ResetProperties();
                }

                if (mItemData.IncriptionProp != null)
                {
                    mItemData.IncriptionProp.ResetProperties();
                }
                
                bool inscriptionHoleIsOpen = mItemData.InscriptionHoles.Count > 0 ? mItemData.InscriptionHoles[0].IsOpenHole : false;
              
                mItemData.InscriptionHoles = InscriptionMosaicDataManager.GetInstance().GetEquipmentInscriptionHoleData(mItemData);
                if (mItemData.InscriptionHoles.Count > 0)
                {
                    mItemData.InscriptionHoles[0].IsOpenHole = inscriptionHoleIsOpen;
                }
                
                EpicEquipmentTransformationSuccessData data = new EpicEquipmentTransformationSuccessData();
                data.itemData = mItemData;

                if (ClientSystemManager.GetInstance().IsFrameOpen<EquipUpgradeResultFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<EquipUpgradeResultFrame>();
                }

                ClientSystemManager.GetInstance().OpenFrame<EquipUpgradeResultFrame>(FrameLayer.Middle, data);

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnEpicEquipmentConversionSuccessed);
            }
        }

        /// <summary>
        /// 史诗转化请求协议
        /// </summary>
        /// <param name="equipConvertType">转化类型</param>
        /// <param name="guid">原装备GUID</param>
        /// <param name="targetid">目标装备表格ID</param>
        public void OnSceneEquipConvertReq(byte equipConvertType, UInt64 guid, UInt32 targetid,UInt64 converterGuid)
        {
            SceneEquipConvertReq req = new SceneEquipConvertReq();
            req.type = equipConvertType;
            req.sourceEqGuid = guid;
            req.targetEqTypeId = targetid;
            req.convertorGuid = converterGuid;

            NetManager.instance.SendCommand(ServerType.GATE_SERVER,req);
        }

        private void OnInitEquieChangeTable()
        {
            if (mEquieChangeTableList != null)
            {
                mEquieChangeTableList.Clear();
            }

            var iter = TableManager.GetInstance().GetTable<EquieChangeTable>().GetEnumerator();
            while (iter.MoveNext())
            {
                var tableData = iter.Current.Value as EquieChangeTable;
                if (tableData == null)
                {
                    continue;
                }

                if (tableData.ConvertType != 1)
                {
                    continue;
                }

                mEquieChangeTableList.Add(tableData);
            }
        }

        private void OnInitEquChangeConsumeTable()
        {
            if (mEquChangeConsumeTableList != null)
            {
                mEquChangeConsumeTableList.Clear();
            }

            var iter = TableManager.GetInstance().GetTable<EquChangeConsumeTable>().GetEnumerator();
            while (iter.MoveNext())
            {
                var tableData = iter.Current.Value as EquChangeConsumeTable;
                if (tableData == null)
                {
                    continue;
                }

                mEquChangeConsumeTableList.Add(tableData);
            }
        }

        /// <summary>
        /// 同套转换检查背包的史诗装备是否可以展示在装备列表中
        /// </summary>
        /// <returns></returns>
        public bool CheckEpicEquipmentCanbeDisplayedInTheEquipmentList(ItemData itemData)
        {
            if (itemData == null)
            {
                return false;
            }

            for (int i = 0; i < mEquieChangeTableList.Count; i++)
            {
                var tableData = mEquieChangeTableList[i];
                if (tableData == null)
                {
                    continue;
                }
                
                if (!tableData.JobID.Contains(PlayerBaseData.GetInstance().JobTableID))
                {
                    continue;
                }

                if (!tableData.UseEquipSuit.Contains(itemData.TableID))
                {
                    continue;
                }

                return true;
            }
            
            return false;
        }

        /// <summary>
        /// 跨套转换检查背包的史诗装备是否可以展示在装备列表中
        /// </summary>
        /// <returns></returns>
        public bool CheckCrossEpicEquipmentCanbeDisplayedInTheEquipmentList(ItemData itemData)
        {
            if (itemData == null)
            {
                return false;
            }

            for (int i = 0; i < mEquieChangeTableList.Count; i++)
            {
                var tableData = mEquieChangeTableList[i];
                if (tableData == null)
                {
                    continue;
                }
                
                //找到属于自己职业的过滤
                if (tableData.JobID.Contains(PlayerBaseData.GetInstance().JobTableID))
                {
                    continue;
                }

                if (!tableData.UseEquipSuit.Contains(itemData.TableID))
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 得到目标装备列表
        /// </summary>
        /// <param name="itemData"></param>
        /// <returns></returns>
        public List<ItemData> GetTargetEquipmentList(ItemData itemData)
        {
            List<ItemData> mTargetEquipmentList = new List<ItemData>();

            for (int i = 0; i < mEquieChangeTableList.Count; i++)
            {
                var tableData = mEquieChangeTableList[i];
                if (tableData == null)
                {
                    continue;
                }

                //不符合自己职业的过滤
                if (!tableData.JobID.Contains(PlayerBaseData.GetInstance().JobTableID))
                {
                    continue;
                }

                //不符合装备等级的过滤
                if (!tableData.Level.Contains(itemData.LevelLimit))
                {
                    continue;
                }

                for (int j = 0; j < tableData.UseEquipSuit.Count; j++)
                {
                    var targetItemData = ItemDataManager.CreateItemDataFromTable(tableData.UseEquipSuit[j],itemData.SubQuality);
                    if (targetItemData == null)
                    {
                        continue;
                    }

                    //相同类型的过滤
                    if (targetItemData.SubType == itemData.SubType)
                    {
                        continue;
                    }

                    //不同种护甲的过滤
                    if (targetItemData.ThirdType != itemData.ThirdType)
                    {
                        continue;
                    }

                    targetItemData.StrengthenLevel = itemData.StrengthenLevel;
                    targetItemData.bLocked = itemData.bLocked;
                    targetItemData.SubQuality = itemData.SubQuality;
                    targetItemData.BeadAdditiveAttributeBuffID = itemData.BeadAdditiveAttributeBuffID;
                    targetItemData.EquipType = itemData.EquipType;
                    targetItemData.GrowthAttrType = itemData.GrowthAttrType;
                    targetItemData.GrowthAttrNum = EquipGrowthDataManager.GetInstance().GetGrowthAttributeValue(targetItemData, targetItemData.StrengthenLevel);
                    targetItemData.InscriptionHoles = InscriptionMosaicDataManager.GetInstance().GetEquipmentInscriptionHoleData(targetItemData);
                    for (int k = 0; k < itemData.InscriptionHoles.Count; k++)
                    {
                        var holeData = itemData.InscriptionHoles[k];
                        if (holeData == null)
                        {
                            continue;
                        }

                        if (targetItemData.InscriptionHoles[k] != null)
                        {
                            targetItemData.InscriptionHoles[k].IsOpenHole = holeData.IsOpenHole;
                        }
                    }

                    var itemStrengthenAttrA = ItemStrengthAttribute.Create(targetItemData.TableID);
                    itemStrengthenAttrA.SetStrength(targetItemData.StrengthenLevel);
                    var data = itemStrengthenAttrA.GetItemData();

                    targetItemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsAttack] = data.BaseProp.props[(int)EEquipProp.IgnorePhysicsAttack];
                    targetItemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsAttackRate] = data.BaseProp.props[(int)EEquipProp.IgnorePhysicsAttackRate];
                    targetItemData.BaseProp.props[(int)EEquipProp.IgnoreMagicAttack] = data.BaseProp.props[(int)EEquipProp.IgnoreMagicAttack];
                    targetItemData.BaseProp.props[(int)EEquipProp.IgnoreMagicAttackRate] = data.BaseProp.props[(int)EEquipProp.IgnoreMagicAttackRate];
                    targetItemData.BaseProp.props[(int)EEquipProp.IngoreIndependence] = data.BaseProp.props[(int)EEquipProp.IngoreIndependence];
                    targetItemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsDefense] = data.BaseProp.props[(int)EEquipProp.IgnorePhysicsDefense];
                    targetItemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsDefenseRate] = data.BaseProp.props[(int)EEquipProp.IgnorePhysicsDefenseRate];
                    targetItemData.BaseProp.props[(int)EEquipProp.IgnoreMagicDefense] = data.BaseProp.props[(int)EEquipProp.IgnoreMagicDefense];
                    targetItemData.BaseProp.props[(int)EEquipProp.IgnoreMagicDefenseRate] = data.BaseProp.props[(int)EEquipProp.IgnoreMagicDefenseRate];

                    targetItemData.RefreshRateScore();

                    mTargetEquipmentList.Add(targetItemData);
                }
            }

            return mTargetEquipmentList;
        }

        /// <summary>
        /// 得到跨套装目标装备
        /// </summary>
        /// <param name="itemData"></param>
        /// <returns></returns>
        public List<ItemData> GetCrossTargetEquipmentList(ItemData itemData)
        {
            List<ItemData> mTargetEquipmentList = new List<ItemData>();

            for (int i = 0; i < mEquieChangeTableList.Count; i++)
            {
                var tableData = mEquieChangeTableList[i];
                if (tableData == null)
                {
                    continue;
                }
                
                //不符合自己职业的过滤
                if (!tableData.JobID.Contains(PlayerBaseData.GetInstance().JobTableID))
                {
                    continue;
                }

                //不符合装备等级的过滤
                if (!tableData.Level.Contains(itemData.LevelLimit))
                {
                    continue;
                }

                for (int j = 0; j < tableData.UseEquipSuit.Count; j++)
                {
                    var targetItemData = ItemDataManager.CreateItemDataFromTable(tableData.UseEquipSuit[j], itemData.SubQuality);
                    if (targetItemData == null)
                    {
                        continue;
                    }

                    //不同类型的过滤
                    if (targetItemData.SubType != itemData.SubType)
                    {
                        continue;
                    }
                    
                    targetItemData.StrengthenLevel = itemData.StrengthenLevel;
                    targetItemData.bLocked = itemData.bLocked;
                    targetItemData.SubQuality = itemData.SubQuality;
                    targetItemData.BeadAdditiveAttributeBuffID = itemData.BeadAdditiveAttributeBuffID;
                    targetItemData.EquipType = itemData.EquipType;
                    targetItemData.GrowthAttrType = itemData.GrowthAttrType;
                    targetItemData.GrowthAttrNum = EquipGrowthDataManager.GetInstance().GetGrowthAttributeValue(targetItemData, targetItemData.StrengthenLevel);
                    targetItemData.InscriptionHoles = InscriptionMosaicDataManager.GetInstance().GetEquipmentInscriptionHoleData(targetItemData);
                    for (int k = 0; k < itemData.InscriptionHoles.Count; k++)
                    {
                        var holeData = itemData.InscriptionHoles[k];
                        if (holeData == null)
                        {
                            continue;
                        }

                        if (targetItemData.InscriptionHoles[k] != null)
                        {
                            targetItemData.InscriptionHoles[k].IsOpenHole = holeData.IsOpenHole;
                        }
                    }

                    var itemStrengthenAttrA = ItemStrengthAttribute.Create(targetItemData.TableID);
                    itemStrengthenAttrA.SetStrength(targetItemData.StrengthenLevel);
                    var data = itemStrengthenAttrA.GetItemData();

                    targetItemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsAttack] = data.BaseProp.props[(int)EEquipProp.IgnorePhysicsAttack];
                    targetItemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsAttackRate] = data.BaseProp.props[(int)EEquipProp.IgnorePhysicsAttackRate];
                    targetItemData.BaseProp.props[(int)EEquipProp.IgnoreMagicAttack] = data.BaseProp.props[(int)EEquipProp.IgnoreMagicAttack];
                    targetItemData.BaseProp.props[(int)EEquipProp.IgnoreMagicAttackRate] = data.BaseProp.props[(int)EEquipProp.IgnoreMagicAttackRate];
                    targetItemData.BaseProp.props[(int)EEquipProp.IngoreIndependence] = data.BaseProp.props[(int)EEquipProp.IngoreIndependence];
                    targetItemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsDefense] = data.BaseProp.props[(int)EEquipProp.IgnorePhysicsDefense];
                    targetItemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsDefenseRate] = data.BaseProp.props[(int)EEquipProp.IgnorePhysicsDefenseRate];
                    targetItemData.BaseProp.props[(int)EEquipProp.IgnoreMagicDefense] = data.BaseProp.props[(int)EEquipProp.IgnoreMagicDefense];
                    targetItemData.BaseProp.props[(int)EEquipProp.IgnoreMagicDefenseRate] = data.BaseProp.props[(int)EEquipProp.IgnoreMagicDefenseRate];

                    targetItemData.RefreshRateScore();

                    mTargetEquipmentList.Add(targetItemData);
                }
            }

            return mTargetEquipmentList;
        }

        /// <summary>
        /// 得到转化目标装备材料
        /// </summary>
        /// <param name="materialList">普通材料</param>
        /// <param name="converterList">转化器</param>
        public void GetConvertedTargetEquipmentMaterial(EpicEquipmentTransformationType type, ItemData itemData,ref List<ItemSimpleData> materialList,ref List<ItemSimpleData> converterList)
        {
            for (int i = 0; i < mEquChangeConsumeTableList.Count; i++)
            {
                var tableData = mEquChangeConsumeTableList[i];
                if (tableData == null)
                {
                    continue;
                }

                //类型不一样过滤
                if (tableData.ConvertType != (int)type)
                {
                    continue;
                }

                //等级不同过滤
                if (tableData.Level != itemData.LevelLimit)
                {
                    continue;
                }

                //不同部位过滤
                if ((int)tableData.SubType != itemData.SubType)
                {
                    continue;
                }

                for (int j = 0; j < tableData.ItemConsume.Count; j++)
                {
                    string itemConsumeStr = tableData.ItemConsume[j];
                    string[] itemConsumeStrs = itemConsumeStr.Split('|');

                    for (int k = 0; k < itemConsumeStrs.Length; k++)
                    {
                        string itemStr = itemConsumeStrs[k];
                        string[] itemStrs = itemStr.Split('_');

                        if (itemStrs.Length >= 2)
                        {
                            int itemId = 0;
                            int count = 0;

                            int.TryParse(itemStrs[0], out itemId);
                            int.TryParse(itemStrs[1], out count);

                            ItemSimpleData itemSimpleData = new ItemSimpleData();
                            itemSimpleData.ItemID = itemId;
                            itemSimpleData.Count = count;

                            materialList.Add(itemSimpleData);
                        }
                    }
                }

                string converterConsumeStr = tableData.ConverterConsume;
                string[] converterConsumeStrs = converterConsumeStr.Split('|');

                for (int k = 0; k < converterConsumeStrs.Length; k++)
                {
                    string itemStr = converterConsumeStrs[k];
                    string[] itemStrs = itemStr.Split('_');

                    if (itemStrs.Length >= 3)
                    {
                        int converterItemId = 0;
                        int converterCount = 0;
                        int converterItemLevel = 0;

                        int.TryParse(itemStrs[0], out converterItemId);
                        int.TryParse(itemStrs[1], out converterItemLevel);
                        int.TryParse(itemStrs[2], out converterCount);

                        ItemSimpleData cobverterItemSimpleData = new ItemSimpleData();
                        cobverterItemSimpleData.ItemID = converterItemId;
                        cobverterItemSimpleData.Count = converterCount;
                        cobverterItemSimpleData.level = converterItemLevel;

                        converterList.Add(cobverterItemSimpleData);
                    }
                }
            }
        }
    }
}

