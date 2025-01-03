using System;
using System.Collections.Generic;
///////删除linq
using System.Text;

using EquipMaterialType = ProtoTable.ItemTable.eThirdType;

namespace GameClient
{

    public class EquipSuitDataManager : DataManager<EquipSuitDataManager>
    {
        protected Dictionary<int, EquipSuitRes> m_suitsDict = new Dictionary<int, EquipSuitRes>();
        protected Dictionary<int, EquipSuitObj> m_selfEquipSuitObjs = new Dictionary<int, EquipSuitObj>();


        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.EquipSuitDataManager;
        }

        public override void Initialize()
        {
            Clear();
        }

        public override void Clear()
        {
            m_suitsDict.Clear();
            m_selfEquipSuitObjs.Clear();
        }

        public void InitSelfEquipSuits()
        {
            List<ItemData> items = GamePool.ListPool<ItemData>.Get();

            List<ulong> wearEquipIDs = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
            ItemData curItem = null;
            for (int i = 0; i < wearEquipIDs.Count; ++i)
            {
                curItem = ItemDataManager.GetInstance().GetItem(wearEquipIDs[i]);
                if(curItem != null)
                    items.Add(curItem);
            }
            List<ulong> wearFashionIDs = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearFashion);
            for (int i = 0; i < wearFashionIDs.Count; ++i)
            {
                curItem = ItemDataManager.GetInstance().GetItem(wearFashionIDs[i]);
                if (curItem != null)
                    items.Add(curItem);
            }

            _CalculateEquipSuitInfos(items, m_selfEquipSuitObjs);
            GamePool.ListPool<ItemData>.Release(items);
        }

        public void UpdateSelfEquipSuits(ItemData a_item, bool a_bEquiped)
        {
            if (
                a_item != null &&
                (a_item.Type == ProtoTable.ItemTable.eType.EQUIP || a_item.Type == ProtoTable.ItemTable.eType.FASHION) &&
                a_item.SuitID > 0
                )
            {
                if (a_bEquiped == false)
                {
                    EquipSuitObj suitObj = GetSelfEquipSuitObj(a_item.SuitID);
                    if (suitObj != null && suitObj.wearedEquipIDs != null)
                    {
                        suitObj.wearedEquipIDs.Remove(a_item.TableID);
                    }
                }
                else
                {
                    EquipSuitObj suitObj = null;
                    m_selfEquipSuitObjs.TryGetValue(a_item.SuitID, out suitObj);
                    if (suitObj == null)
                    {
                        suitObj = new EquipSuitObj();
                        suitObj.wearedEquipIDs = new List<int>();
                        suitObj.equipSuitRes = GetEquipSuitRes(a_item.SuitID);
                        m_selfEquipSuitObjs.Add(a_item.SuitID, suitObj);
                    }

                    if (suitObj.wearedEquipIDs.Contains(a_item.TableID) == false)
                    {
                        suitObj.wearedEquipIDs.Add(a_item.TableID);
                    }
                }
            }
        }

        public EquipSuitObj GetSelfEquipSuitObj(int a_nSuitID)
        {
            if (a_nSuitID <= 0)
            {
                return null;
            }

            EquipSuitObj obj = null;
            m_selfEquipSuitObjs.TryGetValue(a_nSuitID, out obj);
            if (obj == null)
            {
                obj = CreateEmptyEquipSuitObj(a_nSuitID);
                m_selfEquipSuitObjs.Add(a_nSuitID, obj);
            }

            return obj;
        }

        public EquipSuitObj CreateEmptyEquipSuitObj(int a_nSuitID)
        {
            EquipSuitRes suitRes = GetEquipSuitRes(a_nSuitID);
            if (suitRes == null)
            {
                return null;
            }
            else
            {
                EquipSuitObj suitObj = new EquipSuitObj();
                suitObj.wearedEquipIDs = new List<int>();
                suitObj.equipSuitRes = suitRes;
                return suitObj;
            }
        }

        public Dictionary<int, EquipSuitObj> CalculateEquipSuitInfos(List<ItemData> a_arrItems)
        {
            if (a_arrItems == null)
            {
                return null;
            }

            Dictionary<int, EquipSuitObj> dictEquipSuitObjs = new Dictionary<int, EquipSuitObj>();
            _CalculateEquipSuitInfos(a_arrItems, dictEquipSuitObjs);
            return dictEquipSuitObjs;
        }

        public EquipProp GetEquipSuitBasePropByIDs(List<int> itemIDs)
        {
            List<ItemData> items = GamePool.ListPool<ItemData>.Get();
            for (int i = 0; i < itemIDs.Count; ++i)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(itemIDs[i]);
                if (itemData != null)
                {
                    items.Add(itemData);
                }
            }

            Dictionary<int, EquipSuitObj> dictEquipSuitObjs = CalculateEquipSuitInfos(items);

            EquipProp finalProp = new EquipProp();
            var iter = dictEquipSuitObjs.GetEnumerator();
            while (iter.MoveNext())
            {
                EquipSuitObj suitObj = iter.Current.Value;
                if (suitObj != null)
                {
                    if (suitObj.equipSuitRes == null)
                    {
                        Logger.LogErrorFormat("GetEquipSuitBasePropByIDs suitObj {0} equipSuitRes is null", iter.Current.Key);
                        continue;
                    }
                    if (suitObj.wearedEquipIDs == null)
                    {
                        Logger.LogErrorFormat("GetEquipSuitBasePropByIDs suitObj {0} wearedEquipIDs  is null", iter.Current.Key);
                        continue;
                    }
                    var iterProp = suitObj.equipSuitRes.props.GetEnumerator();
                    while (iterProp.MoveNext())
                    {
                        if (iterProp.Current.Key <= suitObj.wearedEquipIDs.Count)
                        {
                            finalProp += iterProp.Current.Value;
                        }
                    }
                }
            }

            GamePool.ListPool<ItemData>.Release(items);

            return finalProp;
        }

        public ItemProperty GetEquipSuitPropByIDs(List<int> itemIDs)
        {
            EquipProp finalProp = GetEquipSuitBasePropByIDs(itemIDs);
            if (null != finalProp)
            {
                return finalProp.ToItemProp();
            }
            return null;
        }

        public EquipSuitRes GetEquipSuitRes(int suitID)
        {
            EquipSuitRes suitRes;
            m_suitsDict.TryGetValue(suitID, out suitRes);
            if (suitRes == null)
            {
                ProtoTable.EquipSuitTable tableData = TableManager.GetInstance().GetTableItem<ProtoTable.EquipSuitTable>(suitID);
                if (tableData != null)
                {
                    suitRes = new EquipSuitRes();
                    suitRes.id = suitID;
                    suitRes.name = tableData.Name;
                    suitRes.equips = tableData.EquipIDs;
                    {
                        EquipProp prop = EquipProp.CreateFromTable(tableData.TwoEquipsAttrID);
                        if (prop != null)
                        {
                            suitRes.props.Add(2, prop);
                        }
                    }
                    {
                        EquipProp prop = EquipProp.CreateFromTable(tableData.ThreeEquipsAttrID);
                        if (prop != null)
                        {
                            suitRes.props.Add(3, prop);
                        }
                    }
                    {
                        EquipProp prop = EquipProp.CreateFromTable(tableData.FourEquipsAttrID);
                        if (prop != null)
                        {
                            suitRes.props.Add(4, prop);
                        }
                    }
                    {
                        EquipProp prop = EquipProp.CreateFromTable(tableData.FiveEquipsAttrID);
                        if (prop != null)
                        {
                            suitRes.props.Add(5, prop);
                        }
                    }
                    {
                        EquipProp prop = EquipProp.CreateFromTable(tableData.SixEquipsAttrID);
                        if (prop != null)
                        {
                            suitRes.props.Add(6, prop);
                        }
                    }
                    {
                        EquipProp prop = EquipProp.CreateFromTable(tableData.EightEquipsAttrID);
                        if (prop != null)
                        {
                            suitRes.props.Add(8, prop);
                        }
                    }

                    //添加9件套套装
                    {
                        EquipProp prop = EquipProp.CreateFromTable(tableData.NineEquipsAttrID);
                        if (prop != null)
                        {
                            suitRes.props.Add(9, prop);
                        }
                    }

                    m_suitsDict.Add(suitID, suitRes);
                }
                else
                {
                  //  Logger.LogErrorFormat("can not find suit data with id:{0}", suitID);
                }
            }
            return suitRes;
        }

        protected void _CalculateEquipSuitInfos(List<ItemData> a_arrItems, Dictionary<int, EquipSuitObj> a_dictSuitObjs)
        {
            if (a_arrItems == null || a_dictSuitObjs == null)
            {
                return;
            }

            a_dictSuitObjs.Clear();
            for (int i = 0; i < a_arrItems.Count; ++i)
            {
                ItemData item = a_arrItems[i];
                if (item != null && item.SuitID > 0)
                {
                    EquipSuitObj obj = null;
                    a_dictSuitObjs.TryGetValue(item.SuitID, out obj);
                    if (obj == null)
                    {
                        obj = new EquipSuitObj();
                        obj.wearedEquipIDs = new List<int>();
                        obj.equipSuitRes = GetEquipSuitRes(item.SuitID);
                        a_dictSuitObjs.Add(item.SuitID, obj);
                    }
                    if (obj.wearedEquipIDs.Contains(item.TableID) == false)
                    {
                        obj.wearedEquipIDs.Add(item.TableID);
                    }
                }
            }
        }
    }

    public class EquipMasterDataManager : DataManager<EquipMasterDataManager>
    {
        Dictionary<int, EquipMasterData> m_materDataDict = new Dictionary<int, EquipMasterData>();

        public class JobMasterDesc
        {
            public int jobID;           // 职业ID
            public int masterType;      // 精通的护甲类型
            public int isMaster;        //是否是精通
            public string title;        // 精通标题
            public string effectDesc;   // 精通效果描述
            public string attrDesc;     // 精通属性加成描述
        }

        Dictionary<int, JobMasterDesc> m_jobMasterDescDict = new Dictionary<int, JobMasterDesc>();

        public bool HasMasterRedPointHinted
        {
            get;set;
        }

        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.EquipMasterDataManager;
        }

        public override void Initialize()
        {
            Clear();
            Dictionary<int, object> tableDict = TableManager.GetInstance().GetTable<ProtoTable.EquipMasterTable>();
            var iter = tableDict.GetEnumerator();
            while (iter.MoveNext())
            {
                ProtoTable.EquipMasterTable table = iter.Current.Value as ProtoTable.EquipMasterTable;
                if (table != null)
                {
                    EquipMasterData master = new EquipMasterData();
                    master.id = _GetMasterID(table.JobID, table.Quality, table.Part, table.MaterialType);
                    master.jobID = table.JobID;
                    master.quality = table.Quality;
                    master.part = table.Part;
                    master.materialType = table.MaterialType;
                    master.masterItem = table;
                    _InitParams(master, (int)EEquipProp.PhysicsAttack, (table.Atk));
                    _InitParams(master, (int)EEquipProp.MagicAttack, (table.MagicAtk));
                    _InitParams(master, (int)EEquipProp.PhysicsDefense, (table.Def));
                    _InitParams(master, (int)EEquipProp.MagicDefense, (table.MagicDef));
                    _InitParams(master, (int)EEquipProp.Strenth, (table.Strenth));
                    _InitParams(master, (int)EEquipProp.Intellect, (table.Intellect));
                    _InitParams(master, (int)EEquipProp.Spirit, (table.Spirit));
                    _InitParams(master, (int)EEquipProp.Stamina, (table.Stamina));
                    _InitParams(master, (int)EEquipProp.HPMax, (table.HPMax));
                    _InitParams(master, (int)EEquipProp.MPMax, (table.MPMax));
                    _InitParams(master, (int)EEquipProp.HPRecover, (table.HPRecover));
                    _InitParams(master, (int)EEquipProp.MPRecover, (table.MPRecover));
                    _InitParams(master, (int)EEquipProp.AttackSpeedRate, (table.AttackSpeedRate));
                    _InitParams(master, (int)EEquipProp.FireSpeedRate, (table.FireSpeedRate));
                    _InitParams(master, (int)EEquipProp.MoveSpeedRate, (table.MoveSpeedRate));
                    _InitParams(master, (int)EEquipProp.HitRate, (table.HitRate));
                    _InitParams(master, (int)EEquipProp.AvoidRate, (table.AvoidRate));
                    _InitParams(master, (int)EEquipProp.PhysicCritRate, (table.PhysicCrit));
                    _InitParams(master, (int)EEquipProp.MagicCritRate, (table.MagicCrit));
                    _InitParams(master, (int)EEquipProp.Spasticity,(table.Spasticity));

                    m_materDataDict.Add(master.id, master);

                    if (m_jobMasterDescDict.ContainsKey(_GetMasterID2(table.JobID,table.MaterialType)) == false)
                    {
                        JobMasterDesc jobMasterDesc = new JobMasterDesc();
                        jobMasterDesc.jobID = table.JobID;
                        jobMasterDesc.masterType = table.MaterialType;
                        jobMasterDesc.isMaster = table.IsMaster;
                        if (table.Descs.Count > 0)
                        {
                            jobMasterDesc.title = table.Descs[0];
                        }
                        if (table.Descs.Count > 1)
                        {
                            jobMasterDesc.effectDesc = table.Descs[1];
                        }
                        if (table.Descs.Count > 2)
                        {
                            jobMasterDesc.attrDesc = table.Descs[2];
                        }
                        m_jobMasterDescDict.Add(_GetMasterID2(table.JobID, table.MaterialType), jobMasterDesc);
                    }
                }
            }
        }

        public override void Clear()
        {
            m_materDataDict.Clear();
            m_jobMasterDescDict.Clear();
            HasMasterRedPointHinted = false;
        }

        public ItemProperty GetEquipMasterPropByIDs(int jobID, List<int> itemIDs)
        {
            EquipProp equipProp = new EquipProp();
            for (int i = 0; i < itemIDs.Count; ++i)
            {
				ItemData itemData = GameClient.ItemDataManager.CreateItemDataFromTable(itemIDs[i]);
                EquipProp temp = GetEquipMasterProp(jobID, itemData);
                if (temp != null)
                {
                    equipProp += temp;
                }
            }
			return equipProp.ToItemProp();
        }

        public EquipProp GetEquipMasterProp(int jobID, ItemData equip)
        {
            if (equip == null) return null;
            EquipMasterData equipMasterData;
            int id = _GetMasterID(jobID, (int)equip.Quality, equip.SubType, (int)equip.ThirdType);
            m_materDataDict.TryGetValue(id, out equipMasterData);

            if (equipMasterData != null)
            {
                return equipMasterData.GetEquipProp(equip.LevelLimit);
            }
            else
            {
                return null;
            }
        }

        public EquipProp GetEquipMasterProp(int jobID, int quality, int subType, int thirdType, int levelLimit)
        {
            EquipMasterData equipMasterData;
            int id = _GetMasterID(jobID, quality, subType, thirdType);
            m_materDataDict.TryGetValue(id, out equipMasterData);

            if (equipMasterData != null)
            {
                return equipMasterData.GetEquipProp(levelLimit);
            }
            else
            {
                return null;
            }
        }

        public JobMasterDesc GetJobMasterDesc(int a_jobID,int material)
        {
            JobMasterDesc desc;
            m_jobMasterDescDict.TryGetValue(_GetMasterID2(a_jobID, material), out desc);
            return desc;
        }

        public bool IsPunish(int a_jobID,int quality,int part,int material)
        {
            int iId = _GetMasterID(a_jobID, quality, part, material);
            if (m_materDataDict.ContainsKey(iId))
            {
                var data = m_materDataDict[iId];
                if(null != data && data.masterItem != null)
                {
                    return data.masterItem.IsMaster == 0;
                }
            }
            return false;
        }

        public int GetMasterPriority(int a_jobID, int quality, int part, int material)
        {
            if(IsPunish(a_jobID, quality, part, material))
            {
                return 2;
            }

            if(IsMaster(a_jobID,material))
            {
                return 0;
            }

            return 1;
        }

        void _InitParams(EquipMasterData masterData, int propID, IList<int> values)
        {
            if (values.Count > 1)
            {
                masterData.qualityParam[propID] = values[0];
                masterData.partParam[propID] = values[1];
                masterData.fixParam[propID] = 0;
            }
            else
            {
                masterData.qualityParam[propID] = 0;
                masterData.partParam[propID] = 0;
                masterData.fixParam[propID] = values[0];
            }
        }

        public bool IsMaster(int a_JobID,int material)
        {
            JobMasterDesc desc;
            m_jobMasterDescDict.TryGetValue(_GetMasterID2(a_JobID, material), out desc);
            if (desc!=null)
            {
                return desc.isMaster == 1;
            }
            else
            {
                return false;
            }
        }
        int _GetMasterID(int jobID, int quality, int part, int materialType)
        {
            return jobID * 1000000 + quality * 10000 + part * 100 + materialType;
        }
        int _GetMasterID2(int jobID,int materialType)
        {
            return jobID*10000+materialType;
        }
    }
}
