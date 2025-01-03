using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoTable;
using Network;
using Protocol;

namespace GameClient
{
    public class EquipUpgradeDataModel
    {
        /// <summary>
        /// 升级后的装备ID
        /// </summary>
        public int mUpgradeEquipID;

        /// <summary>
        /// 材料数据
        /// </summary>
        public List<MaterialConsumeData> mMaterialConsumeData;

        /// <summary>
        /// 增幅装备材料数据
        /// </summary>
        public List<MaterialConsumeData> mGrowthEquipMaterialConsumeData;
    }

    public class MaterialConsumeData
    {
        /// <summary>
        /// 最小强化等级
        /// </summary>
        public int mMinStrenthLevel;

        /// <summary>
        /// 最大强化等级
        /// </summary>
        public int mMaxStrenthLevel;

        /// <summary>
        /// 基础消耗材料集合
        /// </summary>
        public List<ItemSimpleData> mItemSimpleDatas;
    }

    /// <summary>
    /// 装备升级数据管理器
    /// </summary>
    public class EquipUpgradeDataManager : DataManager<EquipUpgradeDataManager>
    {
        /// <summary>
        /// 存储可升级道具数据
        /// </summary>
        Dictionary<int, EquipUpgradeDataModel> mEquipUpgradeDataDic = new Dictionary<int, EquipUpgradeDataModel>();

        public sealed override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.Default;
        }

        public sealed override void Clear()
        {
            mEquipUpgradeDataDic.Clear();
            _UnRegisterNetHandler();
            UnbindEvent();
        }

        public sealed override void Initialize()
        {
            //InitEquipUpgradeTable();
            _RegisterNetHandler();
            BindEvent();
        }

        private void BindEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.JobIDChanged, OnJobIDChanged);
        }

        private void UnbindEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.JobIDChanged, OnJobIDChanged);
        }

        void OnJobIDChanged(UIEvent uiEvent)
        {
            InitEquipUpgradeTable();
        }

        private void _RegisterNetHandler()
        {
            NetProcess.AddMsgHandler(SceneEquieUpdateRes.MsgID, OnSyncSceneEquieUpdateRes);
        }

        private void _UnRegisterNetHandler()
        {
            NetProcess.RemoveMsgHandler(SceneEquieUpdateRes.MsgID, OnSyncSceneEquieUpdateRes);
        }

        private void OnSyncSceneEquieUpdateRes(MsgDATA msg)
        {
            SceneEquieUpdateRes res = new SceneEquieUpdateRes();
            res.decode(msg.bytes);

            if (res.code != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.code);
            }
            else
            {
                ItemData mItemData = ItemDataManager.GetInstance().GetItem(res.equipUid);
                if (mItemData == null)
                {
                    Logger.LogErrorFormat("装备升级返回装备GUID有误");
                }

                if (ClientSystemManager.GetInstance().IsFrameOpen<EquipUpgradeResultFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<EquipUpgradeResultFrame>();
                }

                ClientSystemManager.GetInstance().OpenFrame<EquipUpgradeResultFrame>(FrameLayer.Middle, mItemData);

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnEquipUpgradeSuccess);
            }
        }

        /// <summary>
        /// 装备升级请求
        /// </summary>
        public void OnSendSceneEquieUpdateReq(ulong guid)
        {
            SceneEquieUpdateReq req = new SceneEquieUpdateReq();
            req.equipUid = guid;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 得到装备升级数据
        /// </summary>
        /// <param name="equipId"></param>
        /// <returns></returns>
        public EquipUpgradeDataModel GetEquipUpgradeData(int equipId)
        {
            EquipUpgradeDataModel mData = null;

            if (mEquipUpgradeDataDic.ContainsKey(equipId))
            {
                mData = mEquipUpgradeDataDic[equipId];
            }

            return mData;
        }

        /// <summary>
        /// 得到消耗材料
        /// </summary>
        /// <param name="equipId">装备ID</param>
        /// <param name="strenthLevel">装备强化等级</param>
        /// <returns></returns>
        public List<ItemSimpleData> GetMaterialConsume(int equipId,int strenthLevel,EEquipType equipType)
        {
            List<ItemSimpleData> mItemSimpleDatas = new List<ItemSimpleData>();

            EquipUpgradeDataModel model = GetEquipUpgradeData(equipId);
            if (model != null)
            {
                if (equipType == EEquipType.ET_COMMON)
                {
                    for (int i = 0; i < model.mMaterialConsumeData.Count; i++)
                    {
                        if (strenthLevel >= model.mMaterialConsumeData[i].mMinStrenthLevel && strenthLevel <= model.mMaterialConsumeData[i].mMaxStrenthLevel)
                        {
                            return model.mMaterialConsumeData[i].mItemSimpleDatas;
                        }
                    }
                }
               else if (equipType == EEquipType.ET_REDMARK)
                {
                    for (int i = 0; i < model.mGrowthEquipMaterialConsumeData.Count; i++)
                    {
                        if (strenthLevel >= model.mGrowthEquipMaterialConsumeData[i].mMinStrenthLevel && strenthLevel <= model.mGrowthEquipMaterialConsumeData[i].mMaxStrenthLevel)
                        {
                            return model.mGrowthEquipMaterialConsumeData[i].mItemSimpleDatas;
                        }
                    }
                }
            }

            return mItemSimpleDatas;
        }

        /// <summary>
        /// 初始化装备升级表
        /// </summary>
        public void InitEquipUpgradeTable()
        {
            mEquipUpgradeDataDic.Clear();

            var mTableDic = TableManager.GetInstance().GetTable<EquieUpdateTable>().GetEnumerator();
            while (mTableDic.MoveNext())
            {
                var mEquieUpdateTable = mTableDic.Current.Value as EquieUpdateTable;
                if (mEquieUpdateTable == null)
                {
                    continue;
                }

                if (mEquieUpdateTable.JobID != PlayerBaseData.GetInstance().JobTableID && mEquieUpdateTable.JobID != 0)
                {
                    continue;
                }

                if (!mEquipUpgradeDataDic.ContainsKey(mEquieUpdateTable.EquieID))
                {
                    EquipUpgradeDataModel mModel = new EquipUpgradeDataModel();
                    mModel.mUpgradeEquipID = mEquieUpdateTable.NextLvEquieID;
                    mModel.mMaterialConsumeData = new List<MaterialConsumeData>();
                    mModel.mGrowthEquipMaterialConsumeData = new List<MaterialConsumeData>();

                    mModel.mMaterialConsumeData.AddRange(AddMaterialConsumeData(mEquieUpdateTable.MaterialConsume));
                    mModel.mGrowthEquipMaterialConsumeData.AddRange(AddMaterialConsumeData(mEquieUpdateTable.IncreaseMaterialConsume));

                    mEquipUpgradeDataDic.Add(mEquieUpdateTable.EquieID, mModel);
                }
            }
        }
        
        private List<MaterialConsumeData> AddMaterialConsumeData(string materialConsume)
        {
            List<MaterialConsumeData> List = new List<MaterialConsumeData>();
            
            string[] mMaterialConsumes = materialConsume.Split('|');

            for (int i = 0; i < mMaterialConsumes.Length; i++)
            {
                MaterialConsumeData mMaterialConsumeData = new MaterialConsumeData();

                string[] strs = mMaterialConsumes[i].Split(',');

                mMaterialConsumeData.mItemSimpleDatas = new List<ItemSimpleData>();

                for (int j = 0; j < strs.Length; j++)
                {
                    int minStrenthLevel = 0;
                    int maxStrenthLevel = 0;
                    int id = 0;
                    int num = 0;

                    if (j == 0)
                    {
                        int.TryParse(strs[j], out minStrenthLevel);
                        mMaterialConsumeData.mMinStrenthLevel = minStrenthLevel;
                    }
                    else if (j == 1)
                    {
                        int.TryParse(strs[j], out maxStrenthLevel);
                        mMaterialConsumeData.mMaxStrenthLevel = maxStrenthLevel;
                    }
                    else
                    {
                        string[] mStrings = strs[j].Split('_');

                        if (mStrings.Length >= 2)
                        {
                            int.TryParse(mStrings[0], out id);
                            int.TryParse(mStrings[1], out num);

                            ItemSimpleData itemSimpData = new ItemSimpleData();
                            itemSimpData.ItemID = id;
                            itemSimpData.Count = num;

                            mMaterialConsumeData.mItemSimpleDatas.Add(itemSimpData);
                        }
                    }
                }

                List.Add(mMaterialConsumeData);
            }

            return List;
        }

        /// <summary>
        /// 在装备升级表中查找ID 找到了就在装备升级列表显示
        /// </summary>
        /// <param name="tableID"></param>
        /// <returns></returns>
        public bool FindEquipUpgradeTableID(int tableID)
        {
            bool isFind = false;

            if (mEquipUpgradeDataDic.ContainsKey(tableID))
            {
                isFind = true;
            }

            return isFind;
        }

         /// <summary>
        /// 得到继承装备列表
        /// </summary>
        /// <returns></returns>
        public List<ulong> GetAllJichengEquipments()
        {
            List<ulong> mAllEquipments = new List<ulong>();

            var equipItems = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Equip);
            // var wearEquipItems = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);

            OnAddElement(equipItems, ref mAllEquipments);
            // OnAddElement(wearEquipItems, ref mAllEquipments);

            int minLevel = 0;
            int maxLevel = 0;
            if (SmithShopNewFrameView.iDefaultLevelData != null)
            {
                var smithshopFilterTable = TableManager.GetInstance().GetTableItem<SmithShopFilterTable>(SmithShopNewFrameView.iDefaultLevelData.Id);
                if (smithshopFilterTable != null)
                {
                    if (smithshopFilterTable.Parameter.Count >= 2)
                    {
                        minLevel = smithshopFilterTable.Parameter[0];
                        maxLevel = smithshopFilterTable.Parameter[1];
                    }
                }
            }

            mAllEquipments.RemoveAll(x=>
            {
                var itemData = ItemDataManager.GetInstance().GetItem(x);
                if (itemData == null)
                {
                    return true;
                }

                if (itemData.EquipType == EEquipType.ET_BREATH)
                {
                    return true;
                }

                //如果装备身上有转移石，不显示在列表中
                if (itemData.HasTransfered)
                {
                    return true;
                }

                if (SmithShopNewFrameView.iDefaultQuality != 0)
                {
                    if ((int)itemData.Quality != SmithShopNewFrameView.iDefaultQuality)
                        return true;
                }

                if (itemData.LevelLimit < minLevel)
                    return true;

                if (itemData.LevelLimit > maxLevel)
                    return true;

                return false;
            });

            mAllEquipments.Sort(SortEquipments);

            return mAllEquipments;
        }

        /// <summary>
        /// 得到装备列表
        /// </summary>
        /// <returns></returns>
        public List<ulong> GetAllEquipments()
        {
            List<ulong> mAllEquipments = new List<ulong>();

            var equipItems = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Equip);
            var wearEquipItems = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);

            OnAddElement(equipItems, ref mAllEquipments);
            OnAddElement(wearEquipItems, ref mAllEquipments);

            int minLevel = 0;
            int maxLevel = 0;
            if (SmithShopNewFrameView.iDefaultLevelData != null)
            {
                var smithshopFilterTable = TableManager.GetInstance().GetTableItem<SmithShopFilterTable>(SmithShopNewFrameView.iDefaultLevelData.Id);
                if (smithshopFilterTable != null)
                {
                    if (smithshopFilterTable.Parameter.Count >= 2)
                    {
                        minLevel = smithshopFilterTable.Parameter[0];
                        maxLevel = smithshopFilterTable.Parameter[1];
                    }
                }
            }

            mAllEquipments.RemoveAll(x=>
            {
                var itemData = ItemDataManager.GetInstance().GetItem(x);
                if (itemData == null)
                {
                    return true;
                }

                if (!FindEquipUpgradeTableID(itemData.TableID))
                {
                    return true;
                }

                if (itemData.EquipType == EEquipType.ET_BREATH)
                {
                    return true;
                }

                //如果装备身上有转移石，不显示在列表中
                if (itemData.HasTransfered)
                {
                    return true;
                }

                if (SmithShopNewFrameView.iDefaultQuality != 0)
                {
                    if ((int)itemData.Quality != SmithShopNewFrameView.iDefaultQuality)
                        return true;
                }

                if (itemData.LevelLimit < minLevel)
                    return true;

                if (itemData.LevelLimit > maxLevel)
                    return true;

                return false;
            });

            mAllEquipments.Sort(SortEquipments);

            return mAllEquipments;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        int SortEquipments(ulong x, ulong y)
        {
            ItemData left = ItemDataManager.GetInstance().GetItem(x);

            ItemData right = ItemDataManager.GetInstance().GetItem(y);

            if (left.PackageType != right.PackageType)
            {
                return (int)right.PackageType - (int)left.PackageType;
            }

            if (left.IsItemInUnUsedEquipPlan != right.IsItemInUnUsedEquipPlan)
            {
                if (left.IsItemInUnUsedEquipPlan == true)
                    return -1;
                if (right.IsItemInUnUsedEquipPlan == true)
                    return 1;
            }

            if (left.Quality != right.Quality)
            {
                return (int)right.Quality - (int)left.Quality;
            }

            if (left.SubType != right.SubType)
            {
                return (int)left.SubType - (int)right.SubType;
            }

            if (left.StrengthenLevel != right.StrengthenLevel)
            {
                return right.StrengthenLevel - left.StrengthenLevel;
            }

            return right.LevelLimit - left.LevelLimit;
        }

        private void OnAddElement(List<ulong> items,ref List<ulong> allitems)
        {
            for (int i = 0; i < items.Count; i++)
            {
                allitems.Add(items[i]);
            }
        }
    }
}

