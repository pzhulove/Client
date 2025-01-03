using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoTable;
using Network;
using Protocol;
using System.ComponentModel;

namespace GameClient
{
    /// <summary>
    /// 铭文镶嵌状态
    /// </summary>
    public enum InscriptionMosaicState
    {
        None = 0,
        CanOpenHole,//可开孔
        CanBeSet,//可镶嵌
        HasBeenSet,//已镶嵌
        Replace,   //替换
    }

    /// <summary>
    /// 铭文孔数据
    /// </summary>
    public class InscriptionHoleData
    {
        /// <summary>
        /// 孔索引
        /// </summary>
        public int Index;
        /// <summary>
        /// 孔类型
        /// </summary>
        public int Type;
        /// <summary>
        /// 铭文ID
        /// </summary>
        public int InscriptionId;
        /// <summary>
        /// 记录是否开孔
        /// </summary>
        public bool IsOpenHole;
    }

    public class EquipInscriptionHoleData
    {
        /// <summary>
        /// 品质
        /// </summary>
        public int iColor;
        /// <summary>
        /// 装备子类型
        /// </summary>
        public int iSubType;
        /// <summary>
        /// 开孔器
        /// </summary>
        public InscriptionConsume iItemConsume;
        /// <summary>
        /// 金币
        /// </summary>
        public InscriptionConsume iGoldConsume;
        /// <summary>
        /// 装备铭文孔
        /// </summary>
        public List<HoleData> lEquipmentInscriptionHole; 
    }

    public class HoleData
    {
        public int Index;
        public int Type;

        public HoleData(int index,int type)
        {
            Index = index;
            Type = type;
        }
    }

    /// <summary>
    /// 铭文消耗道具
    /// </summary>
    public class InscriptionConsume
    {
        public int itemId;
        public int count;
    }

    /// <summary>
    /// 铭文摘取数据
    /// </summary>
    public class InscriptionExtractData
    {
        /// <summary>
        /// 品质
        /// </summary>
        public int Color;
        /// <summary>
        /// 摘取材料
        /// </summary>
        public List<InscriptionConsume> ExtractConsumes;
        /// <summary>
        /// 摘取概率
        /// </summary>
        public string ExtractProbability;
        /// <summary>
        /// 是否可摘取
        /// </summary>
        public bool IsExtract;

        public InscriptionExtractData(int color, List<InscriptionConsume> consumes,string extractProbability,bool isExtract)
        {
            Color = color;
            ExtractConsumes = consumes;
            ExtractProbability = extractProbability;
            IsExtract = isExtract;
        }
    }

    /// <summary>
    /// 铭文镶嵌成功数据
    /// </summary>
    public class InscriptionMosaicSuccessData
    {
        /// <summary>
        /// 装备guid
        /// </summary>
        public ulong guid;
        /// <summary>
        /// 镶嵌的铭文id
        /// </summary>
        public int inscriptionId;
    }

    /// <summary>
    /// 铭文概率数据
    /// </summary>
    public class InscriptionProbabilityData
    {
        public int MinProbability;
        public int MaxProbability;
        public string SuccessName;
        public InscriptionProbabilityData(int minProbability,int maxProbability,string successName)
        {
            MinProbability = minProbability;
            MaxProbability = maxProbability;
            SuccessName = successName;
        }
    }

    /// <summary>
    /// 铭文摘取结果数据
    /// </summary>
    public class InscriptionExtractResultData
    {
        /// <summary>
        /// 摘取是否成功
        /// </summary>
        public bool IsSuccessed;
        /// <summary>
        /// 铭文道具
        /// </summary>
        public ItemData InscriptionItemData;

        public InscriptionExtractResultData(bool isSuccessed,ItemData itemData)
        {
            IsSuccessed = isSuccessed;
            InscriptionItemData = itemData;
        }
    }

    /// <summary>
    /// 铭文碎裂结果
    /// </summary>
    public class InscriptionFracturnResultData
    {
        public bool IsSuccessed;
    }

    #region InscriptionComPose 铭文合成数据

    /// <summary>
    /// 铭文品质Tab数据
    /// </summary>
    public class InscriptionQualityTabData : ComControlData
    {
        public InscriptionQualityTabData(int index, int id, string name, bool isSelected)
            :base(index, id, name, isSelected)
        {

        }
    }

    /// <summary>
    /// 铭文三类型Tab数据
    /// </summary>
    public class InscriptionThirdTypeTabData : ComControlData
    {
        public InscriptionThirdTypeTabData(int index,int id,string name,bool isSelected)
            : base(index, id, name, isSelected)
        {

        }
    }

    /// <summary>
    /// 合成可获得的铭文道具展示信息
    /// </summary>
    public class CanBeObtainedInscriptionItemData:System.IComparable<CanBeObtainedInscriptionItemData>
    {
        /// <summary>
        /// 铭文道具
        /// </summary>
        public ItemData inscriptionItemData;
        /// <summary>
        /// 概率
        /// </summary>
        public int probability;

        public int CompareTo(CanBeObtainedInscriptionItemData other)
        {
            if (inscriptionItemData.Quality != other.inscriptionItemData.Quality)
            {
                return other.inscriptionItemData.Quality - inscriptionItemData.Quality;
            }

            return inscriptionItemData.TableID - other.inscriptionItemData.TableID;
        }
    }

    /// <summary>
    /// 铭文合成数据
    /// </summary>
    public class InscriptionSysnthesisData
    {
        /// <summary>
        /// 品质
        /// </summary>
        public int quality;
        /// <summary>
        /// 合成数
        /// </summary>
        public int sysnthesisNumber;
        /// <summary>
        /// 是否是最大合成数
        /// </summary>
        public bool isMaxSynthesisNum;
        /// <summary>
        /// 可获得铭文道具数据
        /// </summary>
        public List<CanBeObtainedInscriptionItemData> canBeObtainedInscriptionItemDataList;
    }

    #endregion

    /// <summary>
    /// 铭文槽类型
    /// </summary>
    public enum EInscriptionSlotType
    {
        Invalid = 0,

        [Description("Inscription_Hole_Red")]
        RetSlot = 800,
        [Description("Inscription_Hole_Yellow")]
        YellowSlot,
        [Description("Inscription_Hole_Blue")]
        BlueSlot,
        [Description("Inscription_Hole_DarkGold")]
        DarkGoldSlot,
        [Description("Inscription_Hole_YaoGolden")]
        YaoGolSlot,
        [Description("Inscription_Hole_Orange")]
        OrangeSlot,
        [Description("Inscription_Hole_Green")]
        GreenSlot,
        [Description("Inscription_Hole_Purple")]
        PurpleSlot,
    }

    public class InscriptionMosaicDataManager : DataManager<InscriptionMosaicDataManager>
    {
        /// <summary>
        /// 所有可开孔的装备
        /// </summary>
        private List<ItemData> mInscriptionAllEquipment = new List<ItemData>();
        /// <summary>
        /// 装备可开孔用的消耗道具
        /// </summary>
        private List<EquipInscriptionHoleData> mOpenInscriptionHoleConsumeList = new List<EquipInscriptionHoleData>();

        /// <summary>
        /// 铭文摘取数据
        /// </summary>
        private List<InscriptionExtractData> mInscriptionExtractDataList = new List<InscriptionExtractData>();

        /// <summary>
        /// 铭文碎裂数据
        /// </summary>
        private List<InscriptionExtractData> mInscriptionFractureDataList = new List<InscriptionExtractData>();

        /// <summary>
        /// 铭文概率
        /// </summary>
        private List<InscriptionProbabilityData> mInscriptionProbabilityDataList = new List<InscriptionProbabilityData>();
        /// <summary>
        /// 铭文合成数据
        /// </summary>
        private List<InscriptionSysnthesisData> mInscriptionSysnthesisDataList = new List<InscriptionSysnthesisData>();
        /// <summary>
        /// 铭文合成高品质弹窗标志位
        /// </summary>
        public static bool IncriptionSynthesisHightQualityBounced = false;
        /// <summary>
        /// 铭文镶嵌弹窗标志位
        /// </summary>
        public static bool InscriptionMosiacBounced = false;
        public sealed override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.Default;
        }

        public sealed override void Clear()
        {
            if (mInscriptionAllEquipment != null)
            {
                mInscriptionAllEquipment.Clear();
            }

            if (mOpenInscriptionHoleConsumeList != null)
            {
                mOpenInscriptionHoleConsumeList.Clear();
            }

            if (mInscriptionExtractDataList != null)
            {
                mInscriptionExtractDataList.Clear();
            }

            if (mInscriptionFractureDataList != null)
            {
                mInscriptionFractureDataList.Clear();
            }

            if (mInscriptionProbabilityDataList != null)
            {
                mInscriptionProbabilityDataList.Clear();
            }

            if (mInscriptionSysnthesisDataList != null)
            {
                mInscriptionSysnthesisDataList.Clear();
            }

            IncriptionSynthesisHightQualityBounced = false;

            InscriptionMosiacBounced = false;

            UnBindNetMessage();
        }

        public sealed override void Initialize()
        {
            BindNetMessage();
            InitEquipInscriptionHoleTable();
            InitInscriptionExtractTable();
            InitInscriptionProbabilityTable();
            InitInscriptionSynthesisTable();
        }

        #region BindNetMessage
        private void BindNetMessage()
        {
            NetProcess.AddMsgHandler(SceneEquipInscriptionOpenRes.MsgID, OnSceneEquipInscriptionOpenRes);
            NetProcess.AddMsgHandler(SceneEquipInscriptionMountRes.MsgID, OnSceneEquipInscriptionMountRes);
            NetProcess.AddMsgHandler(SceneEquipInscriptionExtractRes.MsgID, OnSceneEquipInscriptionExtractRes);
            NetProcess.AddMsgHandler(SceneEquipInscriptionSynthesisRes.MsgID, OnSceneEquipInscriptionSynthesisRes);
            NetProcess.AddMsgHandler(SceneEquipInscriptionDestroyRes.MsgID, OnSceneEquipInscriptionDestroyRes);
        }

        private void UnBindNetMessage()
        {
            NetProcess.RemoveMsgHandler(SceneEquipInscriptionOpenRes.MsgID, OnSceneEquipInscriptionOpenRes);
            NetProcess.RemoveMsgHandler(SceneEquipInscriptionMountRes.MsgID, OnSceneEquipInscriptionMountRes);
            NetProcess.RemoveMsgHandler(SceneEquipInscriptionExtractRes.MsgID, OnSceneEquipInscriptionExtractRes);
            NetProcess.RemoveMsgHandler(SceneEquipInscriptionSynthesisRes.MsgID, OnSceneEquipInscriptionSynthesisRes);
            NetProcess.RemoveMsgHandler(SceneEquipInscriptionDestroyRes.MsgID, OnSceneEquipInscriptionDestroyRes);
        }

        /// <summary>
        /// 开孔成功返回
        /// </summary>
        /// <param name="msg"></param>
        private void OnSceneEquipInscriptionOpenRes(MsgDATA msg)
        {
            SceneEquipInscriptionOpenRes res = new SceneEquipInscriptionOpenRes();
            res.decode(msg.bytes);

            if (res.code != 0)
            {
                SystemNotifyManager.SystemNotify((int)res.code);
            }
            else
            {
                SystemNotifyManager.SysNotifyFloatingEffect("铭文插槽开启成功");
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnInscriptionHoleOpenHoleSuccess);
            }
        }

        private void OnSceneEquipInscriptionMountRes(MsgDATA msg)
        {
            SceneEquipInscriptionMountRes res = new SceneEquipInscriptionMountRes();
            res.decode(msg.bytes);

            if (res.code != 0)
            {
                SystemNotifyManager.SystemNotify((int)res.code);
            }
            else
            {
                InscriptionMosaicSuccessData data = new InscriptionMosaicSuccessData();
                data.guid = res.guid;
                data.inscriptionId = (int)res.inscriptionId;

                ClientSystemManager.GetInstance().OpenFrame<EquipUpgradeResultFrame>(FrameLayer.Middle, data);

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnInscriptionMosaicSuccess);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnItemEquipInscriptionSucceed, res.guid);
            }
        }

        private void OnSceneEquipInscriptionExtractRes(MsgDATA msg)
        {
            SceneEquipInscriptionExtractRes res = new SceneEquipInscriptionExtractRes();
            res.decode(msg.bytes);

            if (res.code != 0)
            {
                InscriptionExtractResultData data = new InscriptionExtractResultData(false,null);

                ClientSystemManager.GetInstance().OpenFrame<EquipUpgradeResultFrame>(FrameLayer.Middle, data);
            }
            else
            {
                ItemData itemData = ItemDataManager.CreateItemDataFromTable((int)res.inscriptionId);

                InscriptionExtractResultData data = new InscriptionExtractResultData(true, itemData);

                ClientSystemManager.GetInstance().OpenFrame<EquipUpgradeResultFrame>(FrameLayer.Middle, data);

                if (ClientSystemManager.GetInstance().IsFrameOpen<InscriptionOperationFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<InscriptionOperationFrame>();
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnInscriptionMosaicSuccess);
            }
        }

        private void OnSceneEquipInscriptionSynthesisRes(MsgDATA msg)
        {
            SceneEquipInscriptionSynthesisRes res = new SceneEquipInscriptionSynthesisRes();
            res.decode(msg.bytes);

            if (res.retCode != 0)
            {
                SystemNotifyManager.SystemNotify((int)res.retCode);
            }
            else
            {
                List<ResultItemData> items = new List<ResultItemData>();

                for (int i = 0; i < res.items.Length; i++)
                {
                    ItemReward itemReward = res.items[i];
                    if (itemReward == null)
                    {
                        continue;
                    }

                    ResultItemData resultData = new ResultItemData();
                    resultData.itemData = ItemDataManager.CreateItemDataFromTable((int)itemReward.id);
                    resultData.itemData.Count = (int)itemReward.num;
                    resultData.desc = resultData.itemData.GetColorName();

                    items.Add(resultData);
                }


                CommonGetItemData data = new CommonGetItemData();
                data.itemDatas = items;
                data.title = TR.Value("Item_merge_successed_hecheng");

                ClientSystemManager.GetInstance().OpenFrame<AdjustResultFrameEx>(FrameLayer.Middle, data);

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnIncriptionSynthesisSuccess);
            }
        }

        /// <summary>
        /// 铭文碎裂返回
        /// </summary>
        /// <param name="msg"></param>
        private void OnSceneEquipInscriptionDestroyRes(MsgDATA msg)
        {
            SceneEquipInscriptionDestroyRes res = new SceneEquipInscriptionDestroyRes();
            res.decode(msg.bytes);

            if (res.code != 0)
            {
                SystemNotifyManager.SystemNotify((int)res.code);
            }
            else
            {
                InscriptionFracturnResultData data = new InscriptionFracturnResultData();
                data.IsSuccessed = true;

                ClientSystemManager.GetInstance().OpenFrame<EquipUpgradeResultFrame>(FrameLayer.Middle, data);

                if (ClientSystemManager.GetInstance().IsFrameOpen<InscriptionOperationFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<InscriptionOperationFrame>();
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnInscriptionMosaicSuccess);
            }
        }

        /// <summary>
        /// 铭文孔开孔请求
        /// </summary>
        /// <param name="guid">装备guid</param>
        /// <param name="index">孔索引</param>
        public void OnSceneEquipInscriptionOpenReq(ulong guid,uint index)
        {
            SceneEquipInscriptionOpenReq req = new SceneEquipInscriptionOpenReq();
            req.guid = guid;
            req.index = index;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 镶嵌铭文请求
        /// </summary>
        /// <param name="equipGuid">装备GUID</param>
        /// <param name="index">孔索引</param>
        /// <param name="inscriptionGuid">铭文GUID</param>
        /// <param name="inscriptionItemid">铭文道具id</param>
        public void OnSceneEquipInscriptionMountReq(ulong equipGuid,uint index,ulong inscriptionGuid,uint inscriptionItemid)
        {
            SceneEquipInscriptionMountReq req = new SceneEquipInscriptionMountReq();
            req.guid = equipGuid;
            req.index = index;
            req.inscriptionGuid = inscriptionGuid;
            req.inscriptionId = inscriptionItemid;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 铭文摘取请求
        /// </summary>
        /// <param name="equipGuid"></param>
        /// <param name="index"></param>
        public void OnSceneEquipInscriptionExtractReq(ulong equipGuid,uint inscriptionId, uint index)
        {
            SceneEquipInscriptionExtractReq req = new SceneEquipInscriptionExtractReq();
            req.guid = equipGuid;
            req.inscriptionId = inscriptionId;
            req.index = index;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 铭文碎裂请求
        /// </summary>
        public void OnSceneEquipInscriptionDestroyReq(ulong equipGuid, uint inscriptionId, uint index)
        {
            SceneEquipInscriptionDestroyReq req = new SceneEquipInscriptionDestroyReq();
            req.guid = equipGuid;
            req.inscriptionId = inscriptionId;
            req.index = index;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 铭文合成请求
        /// </summary>
        /// <param name="itemIDVec"></param>
        public void OnSceneEquipInscriptionSynthesisReq(UInt32[] itemIDVec)
        {
            SceneEquipInscriptionSynthesisReq req = new SceneEquipInscriptionSynthesisReq();
            req.itemIDVec = itemIDVec;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }
        #endregion


        private void InitEquipInscriptionHoleTable()
        {
            if (mOpenInscriptionHoleConsumeList == null)
            {
                mOpenInscriptionHoleConsumeList = new List<EquipInscriptionHoleData>();
            }

            mOpenInscriptionHoleConsumeList.Clear();
            var equipInscriptionHoleTable = TableManager.GetInstance().GetTable<EquipInscriptionHoleTable>();
            if (equipInscriptionHoleTable != null)
            {
                var enumerator = equipInscriptionHoleTable.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var table = enumerator.Current.Value as EquipInscriptionHoleTable;
                    if (table == null)
                    {
                        continue;
                    }

                    EquipInscriptionHoleData consume = new EquipInscriptionHoleData();
                    consume.iColor = (int)table.Color;
                    consume.iSubType = (int)table.SubType;
                    consume.iItemConsume = GetInscriptionConsume(table.ItemConsume);
                    consume.iGoldConsume = GetInscriptionConsume(table.GoldConsume);
                    consume.lEquipmentInscriptionHole = GetEquipmentInscriptionHoleData(table.InscriptionHoleColor);
                    mOpenInscriptionHoleConsumeList.Add(consume);
                }
            }
        }

        private void InitInscriptionExtractTable()
        {
            if (mInscriptionExtractDataList == null)
            {
                mInscriptionExtractDataList = new List<InscriptionExtractData>();
            }

            mInscriptionExtractDataList.Clear();

            if (mInscriptionFractureDataList == null)
            {
                mInscriptionFractureDataList = new List<InscriptionExtractData>();
            }

            mInscriptionFractureDataList.Clear();

            var inscriptionExtractTable = TableManager.GetInstance().GetTable<InscriptionExtractTable>();
            if (inscriptionExtractTable != null)
            {
                var enumerator = inscriptionExtractTable.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var table = enumerator.Current.Value as InscriptionExtractTable;
                    if (table == null)
                    {
                        continue;
                    }
                    
                    InscriptionExtractData data = new InscriptionExtractData
                        ((int)table.Color,
                        GetInscriptionExtractConsumes(table.ExtractItemConsume),
                        table.ExtractProbability,
                        table.IsExtract == 1
                        );

                    mInscriptionExtractDataList.Add(data);

                    InscriptionExtractData fracture = new InscriptionExtractData(((int)table.Color),
                        GetInscriptionExtractConsumes(table.DestroyConsume),
                        table.DestroyProbability,
                        true);

                    mInscriptionFractureDataList.Add(fracture);
                }
            }
        }

        private void InitInscriptionProbabilityTable()
        {
            mInscriptionProbabilityDataList.Clear();

            var inscriptionProbabilityTable = TableManager.GetInstance().GetTable<InscriptionProbabilityTable>();
            if (inscriptionProbabilityTable != null)
            {
                var enumerator = inscriptionProbabilityTable.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var tabel = enumerator.Current.Value as InscriptionProbabilityTable;
                    if (tabel == null)
                    {
                        continue;
                    }

                    InscriptionProbabilityData data = new InscriptionProbabilityData(tabel.MinProbability, tabel.MaxProbability, tabel.SuccessName);

                    mInscriptionProbabilityDataList.Add(data);
                }
            }
        }
        private InscriptionConsume GetInscriptionConsume(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            InscriptionConsume consumeData = new InscriptionConsume();
            int itemid = 0;
            int count = 0;
            string[] itemConsumes = str.Split('_');
            if (itemConsumes != null && itemConsumes.Length >= 2)
            {
                int.TryParse(itemConsumes[0], out itemid);
                int.TryParse(itemConsumes[1], out count);
            }

            consumeData.itemId = itemid;
            consumeData.count = count;

            return consumeData;
        }

        private List<HoleData> GetEquipmentInscriptionHoleData(string str)
        {
            List<HoleData> holeDatas = new List<HoleData>();
            string[] strs = str.Split('|');
            for (int i = 0; i < strs.Length; i++)
            {
                string[] inscriptionHoles = strs[i].Split('_');
                if (inscriptionHoles != null && inscriptionHoles.Length >= 2)
                {
                    int Index = 0;
                    int Type = 0;

                    int.TryParse(inscriptionHoles[0], out Index);
                    int.TryParse(inscriptionHoles[1], out Type);

                    HoleData data = new HoleData(Index,Type);

                    holeDatas.Add(data);
                }
            }

            return holeDatas;
        }

        private List<InscriptionConsume> GetInscriptionExtractConsumes(string str)
        {
            List<InscriptionConsume> inscriptionConsumes = new List<InscriptionConsume>();
            string[] strs = str.Split('|');
            for (int i = 0; i < strs.Length; i++)
            {
                string[] extractConsums = strs[i].Split('_');
                if (extractConsums != null && extractConsums.Length >= 2)
                {
                    int itemId = 0;
                    int count = 0;

                    int.TryParse(extractConsums[0], out itemId);
                    int.TryParse(extractConsums[1], out count);

                    InscriptionConsume data = new InscriptionConsume();
                    data.itemId = itemId;
                    data.count = count;

                    inscriptionConsumes.Add(data);
                }
            }

            return inscriptionConsumes;
        }

        public List<ItemData> GetInscriptionAllEquipment()
        {
            LoadAllInscriptionEquipment();
            return mInscriptionAllEquipment;
        }

        public void LoadAllInscriptionEquipment()
        {
            if (mInscriptionAllEquipment == null)
            {
                mInscriptionAllEquipment = new List<ItemData>();
            }

            mInscriptionAllEquipment.Clear();

            var equipItems = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Equip);
            AddInscriptionItemData(equipItems);
            var wearItems = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
            AddInscriptionItemData(wearItems);

            mInscriptionAllEquipment.Sort(SoreInscriptionEquipment);
        }

        private void AddInscriptionItemData(List<ulong> items)
        {
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

            for (int i = 0; i < items.Count; i++)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(items[i]);
                if (itemData == null)
                {
                    continue;
                }

                if (itemData.Type != ItemTable.eType.EQUIP)
                {
                    continue;
                }

                if (itemData.SubType == (int)ItemTable.eSubType.WEAPON)
                {
                    continue;
                }

                //品质小于绿色过滤掉
                if (itemData.Quality < ItemTable.eColor.GREEN)
                {
                    continue;
                }
                
                //整备类型是气息装备过滤掉
                if (itemData.EquipType == EEquipType.ET_BREATH)
                {
                    continue;
                }

                if (SmithShopNewFrameView.iDefaultQuality != 0)
                {
                    if ((int)itemData.Quality != SmithShopNewFrameView.iDefaultQuality)
                        continue;
                }

                if (itemData.LevelLimit < minLevel)
                    continue;

                if (itemData.LevelLimit > maxLevel)
                    continue;

                //过滤辟邪玉
                if (itemData.SubType == 199)
                {
                    continue;
                }

                mInscriptionAllEquipment.Add(itemData);
            }
        }

        private int SoreInscriptionEquipment(ItemData x,ItemData y)
        {
            if (x.PackageType != y.PackageType)
            {
                return (int)y.PackageType - (int)x.PackageType;
            }

            //在未启用的装备方案中,排在前面
            if (x.IsItemInUnUsedEquipPlan != y.IsItemInUnUsedEquipPlan)
            {
                if (x.IsItemInUnUsedEquipPlan == true)
                    return -1;
                if (y.IsItemInUnUsedEquipPlan == true)
                    return 1;
            }

            if (x.Quality != y.Quality)
            {
                return y.Quality - x.Quality;
            }

            if (x.SubType != y.SubType)
            {
                return (int)x.SubType - (int)y.SubType;
            }

            if (x.StrengthenLevel != y.StrengthenLevel)
            {
                return y.StrengthenLevel - x.StrengthenLevel;
            }

            return y.LevelLimit - x.LevelLimit;
        }

        /// <summary>
        /// 得到装备孔数组
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public List<InscriptionHoleData> GetEquipmentInscriptionHoleData(ItemData item)
        {
            List<HoleData> holeDatas = new List<HoleData>();

            for (int i = 0; i < mOpenInscriptionHoleConsumeList.Count; i++)
            {
                if ((int)item.Quality != mOpenInscriptionHoleConsumeList[i].iColor)
                {
                    continue;
                }

                if (item.SubType != mOpenInscriptionHoleConsumeList[i].iSubType)
                {
                    continue;
                }

                holeDatas = mOpenInscriptionHoleConsumeList[i].lEquipmentInscriptionHole;
                break;
            }

            List<InscriptionHoleData> datas = new List<InscriptionHoleData>();
            for (int i = 0; i < holeDatas.Count; i++)
            {
                InscriptionHoleData data = new InscriptionHoleData();
                data.Index = holeDatas[i].Index;
                data.Type = holeDatas[i].Type;
                data.InscriptionId = 0;
                data.IsOpenHole = false;

                datas.Add(data);
            }

            return datas;
        }

        /// <summary>
        /// 得到装备铭文孔数据
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public List<HoleData> GetEquipmentInscriptionHoleNumber(ItemData item)
        {
            List<HoleData> holes = new List<HoleData>();
            for (int i = 0; i < mOpenInscriptionHoleConsumeList.Count; i++)
            {
                if ((int)item.Quality != mOpenInscriptionHoleConsumeList[i].iColor)
                {
                    continue;
                }

                if (item.SubType != mOpenInscriptionHoleConsumeList[i].iSubType)
                {
                    continue;
                }

                holes = mOpenInscriptionHoleConsumeList[i].lEquipmentInscriptionHole;
                break;
            }

            return holes;
        }

        /// <summary>
        /// 得到可镶嵌的铭文
        /// </summary>
        /// <returns></returns>
        public List<ItemData> GetCanMosaicInscription(int holeType)
        {
            List<ItemData> inscriptionList = new List<ItemData>();
            var inscriptionHoleSetTable = TableManager.GetInstance().GetTableItem<InscriptionHoleSetTable>(holeType);
            if (inscriptionHoleSetTable != null)
            {
                var inscriptionItems = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Inscription);
                if (inscriptionItems != null)
                {
                    for (int i = 0; i < inscriptionItems.Count; i++)
                    {
                        ItemData itemData = ItemDataManager.GetInstance().GetItem(inscriptionItems[i]);
                        if (itemData == null)
                        {
                            continue;
                        }

                        if (!inscriptionHoleSetTable.ThirdType.Contains((int)itemData.ThirdType))
                        {
                            continue;
                        }

                        inscriptionList.Add(itemData);
                    }
                }
            }

            if (inscriptionList.Count > 0)
            {
                inscriptionList.Sort(Sort);
            }

            return inscriptionList;
        }

        private bool CheckAttributeIsApplyCurrentRole(InscriptionTable table)
        {
            bool isApplyCurrentRole = false;

            if (table == null)
            {
                return isApplyCurrentRole;
            }

            for (int i = 0; i < table.Occu.Length; i++)
            {
                if (table.Occu[i] == 0)
                {
                    return true;
                }

                JobTable jobTable = TableManager.GetInstance().GetTableItem<JobTable>(table.Occu[i]);
                if (jobTable == null)
                {
                    continue;
                }

                //如果是大职业
                if (jobTable.prejob == 0)
                {
                    if (jobTable.ID != Utility.GetBaseJobID(PlayerBaseData.GetInstance().JobTableID))
                    {
                        continue;
                    }
                }
                else
                {
                    if (jobTable.ID != PlayerBaseData.GetInstance().JobTableID)
                    {
                        continue;
                    }
                }

                isApplyCurrentRole = true;
                break; 
            }

            return isApplyCurrentRole;
        }

        public string GetInscriptionAttributesDesc(int id, bool isLine = true)
        {
            var inscriptionTable = TableManager.GetInstance().GetTableItem<InscriptionTable>(id);
            if (inscriptionTable == null)
            {
                return string.Empty;
            }

            return GetInscriptionAttributesDesc(inscriptionTable.PropType, inscriptionTable.PropValue, inscriptionTable.BuffID,isLine, CheckAttributeIsApplyCurrentRole(inscriptionTable));
        }

        public string GetInscriptionAttributesDesc(IList<int> iPropType,IList<int> iPropValue, IList<int> iBuffID,bool isLine,bool isApplyCurrentRole)
        {
            string inscriptionAttr = string.Empty;

            bool bHasContent = false;

            var kStringBuilder = StringBuilderCache.Acquire();
            kStringBuilder.Clear();
            if (iPropType.Count == iPropValue.Count)
            {
                if (iPropType.Count > 0)
                {
                    if (isApplyCurrentRole)
                    {
                        kStringBuilder.AppendFormat("<color={0}>", TR.Value("enchant_attribute_color"));
                    }
                    else
                    {
                        kStringBuilder.AppendFormat("<color={0}>", TR.Value("enchant_skills_gray_color"));
                    }
                }
                var attrs = iPropValue;
                for (int i = 0; i < iPropType.Count; ++i)
                {
                    if (attrs[i] != 0)
                    {
                        EServerProp eEServerProp = (EServerProp)iPropType[i];
                        MapEnum mapEnum = Utility.GetEnumAttribute<EServerProp, MapEnum>(eEServerProp);

                        if (mapEnum == null && iPropType[i] >= 18 && iPropType[i] <= 21)
                        {
                            mapEnum = new MapEnum((EEquipProp)(iPropType[i] - 18));
                        }

                        if (mapEnum == null && iPropType[i] >= (int)EServerProp.IRP_LIGHT && iPropType[i] <= (int)EServerProp.IRP_DARK)
                        {
                            mapEnum = new MapEnum(EEquipProp.Elements);
                        }

                        if (mapEnum != null)
                        {
                            EEquipProp eEEquipProp = mapEnum.Prop;
                            string strValue = Utility.GetEEquipProDesc(eEEquipProp, attrs[i]);

                            if (bHasContent)
                            {
                                if (isLine)
                                {
                                    kStringBuilder.Append("\n");
                                }
                                else
                                {
                                    kStringBuilder.Append("、");
                                }
                            }
                            kStringBuilder.Append(strValue);
                            bHasContent = true;
                        }
                    }
                }

                if (iPropType.Count > 0)
                {
                    kStringBuilder.Append("</color>");
                }
            }

            for (int i = 0; i < iBuffID.Count; ++i)
            {
                var bufferitem = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(iBuffID[i]);
                if (null != bufferitem)
                {
                    if (bufferitem.Description.Count > 0)
                    {
                        if (bHasContent)
                        {
                            if (isLine)
                            {
                                kStringBuilder.Append("\n");
                            }
                            else
                            {
                                kStringBuilder.Append("、");
                            }
                        }

                        if (isApplyCurrentRole)
                        {
                            kStringBuilder.AppendFormat("<color={0}>{1}</color>", TR.Value("enchant_skills_color"), bufferitem.Description[0]);
                        }
                        else
                        {
                            kStringBuilder.AppendFormat("<color={0}>{1}</color>", TR.Value("enchant_skills_gray_color"), bufferitem.Description[0]);
                        }
                        
                        bHasContent = true;
                    }
                }
            }

            inscriptionAttr = kStringBuilder.ToString();
            StringBuilderCache.Release(kStringBuilder);
            return inscriptionAttr;
        }

        /// <summary>
        /// 得到铭文槽位显示
        /// </summary>
        /// <param name="inscriptionId"></param>
        /// <returns></returns>
        public string GetInscriptionSlotDescription(int inscriptionId)
        {
            var kStringBuilder = StringBuilderCache.Acquire();
            kStringBuilder.AppendFormat("<color={0}>", TR.Value("enchant_condition_color"));
            var inscriptionTable = TableManager.GetInstance().GetTableItem<InscriptionTable>(inscriptionId);
            if (inscriptionTable != null)
            {
                for (int i = 0; i < inscriptionTable.InscriptionHole.Count; i++)
                {
                    var describe = Utility.GetEnumDescription((EInscriptionSlotType)inscriptionTable.InscriptionHole[i]);
                    kStringBuilder.Append(TR.Value(describe));
                    if (i != inscriptionTable.InscriptionHole.Count - 1)
                    {
                        kStringBuilder.Append("、");
                    }
                }
            }

            kStringBuilder.Append("</color>");
            string ret = kStringBuilder.ToString();
            StringBuilderCache.Release(kStringBuilder);

            return ret;
        }

        /// <summary>
        /// 铭文适用职业描述
        /// </summary>
        /// <param name="inscriptionId"></param>
        /// <returns></returns>
        public string GetInscriptionApplicableToProfessionalDescription(int inscriptionId)
        {
            var inscriptionTable = TableManager.GetInstance().GetTableItem<InscriptionTable>(inscriptionId);
            if (inscriptionTable == null)
            {
                return string.Empty;
            }

            var kStringBuilder = StringBuilderCache.Acquire();
            kStringBuilder.AppendFormat(TR.Value("tip_inscription_job"));

            if (inscriptionTable.Occu.Length == 1)
            {
                if (inscriptionTable.Occu[0] == 0)
                {
                    kStringBuilder.Append(TR.Value("tip_inscription_job_green", "通用"));
                }
                else
                {
                    JobTable jobTable = TableManager.GetInstance().GetTableItem<JobTable>(inscriptionTable.Occu[0]);
                    if (jobTable == null)
                    {
                        return string.Empty;
                    }

                    //如果是大职业
                    if (jobTable.prejob == 0)
                    {
                        if (jobTable.ID == Utility.GetBaseJobID(PlayerBaseData.GetInstance().JobTableID))
                        {
                            kStringBuilder.Append(TR.Value("tip_inscription_job_green", jobTable.Name));
                        }
                        else
                        {
                            kStringBuilder.Append(TR.Value("tip_inscription_job_gray", jobTable.Name));
                        }
                    }
                    else
                    {
                        if (jobTable.ID == PlayerBaseData.GetInstance().JobTableID)
                        {
                            kStringBuilder.Append(TR.Value("tip_inscription_job_green", jobTable.Name));
                        }
                        else
                        {
                            kStringBuilder.Append(TR.Value("tip_inscription_job_gray", jobTable.Name));
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < inscriptionTable.Occu.Length; i++)
                {
                    JobTable jobTable = TableManager.GetInstance().GetTableItem<JobTable>(inscriptionTable.Occu[i]);

                    if (jobTable == null)
                    {
                        continue;
                    }

                    //如果是大职业
                    if (jobTable.prejob == 0)
                    {
                        if (jobTable.ID == Utility.GetBaseJobID(PlayerBaseData.GetInstance().JobTableID))
                        {
                            kStringBuilder.Append(TR.Value("tip_inscription_job_green", jobTable.Name));
                        }
                        else
                        {
                            kStringBuilder.Append(TR.Value("tip_inscription_job_gray", jobTable.Name));
                        }
                    }
                    else
                    {
                        if (jobTable.ID == PlayerBaseData.GetInstance().JobTableID)
                        {
                            kStringBuilder.Append(TR.Value("tip_inscription_job_green", jobTable.Name));
                        }
                        else
                        {
                            kStringBuilder.Append(TR.Value("tip_inscription_job_gray", jobTable.Name));
                        }
                    }

                    if (i != inscriptionTable.Occu.Length - 1)
                    {
                        kStringBuilder.Append("、");
                    }
                }
            }
            
            string ret = kStringBuilder.ToString();
            StringBuilderCache.Release(kStringBuilder);

            return ret;
        }

        /// <summary>
        /// 得到装备开铭文孔消耗的道具
        /// </summary>
        /// <returns></returns>
        public EquipInscriptionHoleData GetOpenInscriptionHoleConsume(ItemData item)
        {
            EquipInscriptionHoleData Consume = null;
            if (item == null)
            {
                return Consume;
            }
            for (int i = 0; i < mOpenInscriptionHoleConsumeList.Count; i++)
            {
                if ((int)item.Quality != mOpenInscriptionHoleConsumeList[i].iColor)
                {
                    continue;
                }

                if (item.SubType != mOpenInscriptionHoleConsumeList[i].iSubType)
                {
                    continue;
                }

                Consume = mOpenInscriptionHoleConsumeList[i];
                break;
            }

            return Consume;
        }

        /// <summary>
        /// 判断铭文是否可摘取
        /// </summary>
        /// <returns></returns>
        public bool CheckInscriptionItemIsExtract(int inscriptionItemId)
        {
            bool isExtract = false;//是否可摘取
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(inscriptionItemId);
            if (itemTable != null)
            {
                for (int i = 0; i < mInscriptionExtractDataList.Count; i++)
                {
                    if (mInscriptionExtractDataList[i].Color != (int)itemTable.Color)
                    {
                        continue;
                    }

                    isExtract = mInscriptionExtractDataList[i].IsExtract;
                    break;
                }
            }

            return isExtract;
        }

        /// <summary>
        /// 得到摘取铭文需要消耗的材料
        /// </summary>
        /// <param name="inscriptionItemId"></param>
        /// <returns></returns>
        public List<InscriptionConsume> GetInscriptionExtractConsume(int inscriptionItemId)
        {
            List<InscriptionConsume> datas = new List<InscriptionConsume>();

            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(inscriptionItemId);
            if (itemTable != null)
            {
                for (int i = 0; i < mInscriptionExtractDataList.Count; i++)
                {
                    if (mInscriptionExtractDataList[i].Color != (int)itemTable.Color)
                    {
                        continue;
                    }

                    datas = mInscriptionExtractDataList[i].ExtractConsumes;
                    break;
                }
            }

            return datas;
        }

        /// <summary>
        /// 得到铭文碎裂材料
        /// </summary>
        /// <param name="inscriptionItemId"></param>
        /// <returns></returns>
        public List<InscriptionConsume> GetInscriptionFractureConsume(int inscriptionItemId)
        {
            List<InscriptionConsume> datas = new List<InscriptionConsume>();

            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(inscriptionItemId);
            if (itemTable != null)
            {
                for (int i = 0; i < mInscriptionFractureDataList.Count; i++)
                {
                    if (mInscriptionFractureDataList[i].Color != (int)itemTable.Color)
                    {
                        continue;
                    }

                    datas = mInscriptionFractureDataList[i].ExtractConsumes;
                    break;
                }
            }

            return datas;
        }

        /// <summary>
        /// 得到铭文摘取成功率
        /// </summary>
        /// <param name="inscriptionItemId"></param>
        /// <returns></returns>
        public int GetInscriptionExtractSuccessRate(int inscriptionItemId)
        {
            int successRate = 0;
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(inscriptionItemId);
            if (itemTable != null)
            {
                for (int i = 0; i < mInscriptionExtractDataList.Count; i++)
                {
                    if (mInscriptionExtractDataList[i].Color != (int)itemTable.Color)
                    {
                        continue;
                    }

                    int.TryParse(mInscriptionExtractDataList[i].ExtractProbability,out successRate);
                    break;
                }
            }

            return successRate;
        }

        /// <summary>
        /// 得到铭文碎裂成功率
        /// </summary>
        /// <param name="inscriptionItemId"></param>
        /// <returns></returns>
        public int GetInscriptionFractureSuccessRate(int inscriptionItemId)
        {
            int successRate = 0;
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(inscriptionItemId);
            if (itemTable != null)
            {
                for (int i = 0; i < mInscriptionFractureDataList.Count; i++)
                {
                    if (mInscriptionFractureDataList[i].Color != (int)itemTable.Color)
                    {
                        continue;
                    }

                    int.TryParse(mInscriptionFractureDataList[i].ExtractProbability, out successRate);
                    break;
                }
            }

            return successRate;
        }

        /// <summary>
        /// 得到铭文摘取成功率描述
        /// </summary>
        /// <param name="successRate"></param>
        /// <returns></returns>
        public string GetInscriptionExtractSuccessRateDesc(int successRate)
        {
            string successRateDesc = string.Empty;
            for (int i = 0; i < mInscriptionProbabilityDataList.Count; i++)
            {
                var data = mInscriptionProbabilityDataList[i];
                if (successRate > data.MinProbability && successRate <= data.MaxProbability)
                {
                    successRateDesc = data.SuccessName;
                    break;
                }
            }

            return successRateDesc;
        }

        /// <summary>
        /// 检查装备镶嵌的铭文品质是否低于品阶五
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CheckEquipmentMosaicInscriptionQualityFollowingFive(ItemData item)
        {
            if (item.InscriptionHoles == null)
            {
                return false;
            }

            bool qualityFollowingFive = false;//品质五以下
           
            for (int i = 0; i < item.InscriptionHoles.Count; i++)
            {
                InscriptionHoleData inscriptionHoleData = item.InscriptionHoles[i];
                if (inscriptionHoleData == null)
                {
                    continue;
                }

                if (inscriptionHoleData.IsOpenHole == false)
                {
                    continue;
                }

                if (inscriptionHoleData.InscriptionId <= 0)
                {
                    continue;
                }

                ItemData inscriptionItem = ItemDataManager.CreateItemDataFromTable(inscriptionHoleData.InscriptionId);
                if (inscriptionItem == null)
                {
                    continue;
                }

                if (inscriptionItem.Quality < ItemTable.eColor.YELLOW)
                {
                    qualityFollowingFive = true;
                }
            }

            return qualityFollowingFive;
        }

        /// <summary>
        /// 检查装备镶嵌的铭文品质是否都是五阶
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CheckEquipmentMosaicInscriptionQualityFive(ref List<ItemData> inscriptionItemDatas, ItemData item, bool isQualityFive = true)
        {
            if (item.InscriptionHoles == null)
            {
                return false;
            }

            inscriptionItemDatas = new List<ItemData>();
            bool isFlag = false;//都是品质五

            bool yes = false;
            bool no = false;
            for (int i = 0; i < item.InscriptionHoles.Count; i++)
            {
                InscriptionHoleData inscriptionHoleData = item.InscriptionHoles[i];
                if (inscriptionHoleData == null)
                {
                    continue;
                }

                if (inscriptionHoleData.IsOpenHole == false)
                {
                    continue;
                }

                if (inscriptionHoleData.InscriptionId <= 0)
                {
                    continue;
                }

                ItemData inscriptionItem = ItemDataManager.CreateItemDataFromTable(inscriptionHoleData.InscriptionId);
                if (inscriptionItem == null)
                {
                    continue;
                }

                if (inscriptionItem.Quality == ItemTable.eColor.YELLOW)
                {
                    yes = true;
                }
                else
                {
                    no = true;
                }

                inscriptionItemDatas.Add(inscriptionItem);
            }

            if (isQualityFive == true)
            {
                isFlag = yes && !no;//都是品质五
            }
            else
            {
                isFlag = yes && no;//既有品质五也有低于品质五
            }

            return isFlag;
        }

        /// <summary>
        /// 得到铭文镶嵌孔名字
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetInscriptionHoleName(int id)
        {
            string name = string.Empty;
            
            if (id == (int)ItemTable.eThirdType.RedInscription)
            {
                name = TR.Value("Inscription_Hole_Red");
            }
            else if (id == (int)ItemTable.eThirdType.YellowInscription)
            {
                name = TR.Value("Inscription_Hole_Yellow");
            }
            else if (id == (int)ItemTable.eThirdType.BlueInscription)
            {
                name = TR.Value("Inscription_Hole_Blue");
            }
            else if (id == (int)ItemTable.eThirdType.DarkBlondInscription)
            {
                name = TR.Value("Inscription_Hole_DarkGold");
            }
            else if (id == (int)ItemTable.eThirdType.BrilliantGoldenInscription)
            {
                name = TR.Value("Inscription_Hole_YaoGolden");
            }
            else if (id == (int)ItemTable.eThirdType.OrangeInscription)
            {
                name = TR.Value("Inscription_Hole_Orange");
            }
            else if (id == (int)ItemTable.eThirdType.GreenInscription)
            {
                name = TR.Value("Inscription_Hole_Green");
            }
            else if (id == (int)ItemTable.eThirdType.VioletInscription)
            {
                name = TR.Value("Inscription_Hole_Purple");
            }

            return name;
        }

        #region InscriptionComPose 铭文合成

        /// <summary>
        /// 初始化铭文合成表
        /// </summary>
        private void InitInscriptionSynthesisTable()
        {
            mInscriptionSysnthesisDataList.Clear();

            var inscriptionSynthesisTable = TableManager.GetInstance().GetTable<InscriptionSynthesisTable>();
            if (inscriptionSynthesisTable != null)
            {
                var enumerator = inscriptionSynthesisTable.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var table = enumerator.Current.Value as InscriptionSynthesisTable;
                    if (table == null)
                    {
                        continue;
                    }
                    
                    InscriptionSysnthesisData data = new InscriptionSysnthesisData();
                    data.quality = (int)table.Color;
                    data.sysnthesisNumber = table.SynthesisNum;
                    data.isMaxSynthesisNum = table.IsMaxSynthesisNum == 1;
                    data.canBeObtainedInscriptionItemDataList = GetCanBeObtainedInscriptionItemDataList(table.GenerateInscriptionPreView);

                    mInscriptionSysnthesisDataList.Add(data);
                }
            }
        }

        private List<CanBeObtainedInscriptionItemData> GetCanBeObtainedInscriptionItemDataList(string str)
        {
            List<CanBeObtainedInscriptionItemData> datas = new List<CanBeObtainedInscriptionItemData>();
            string[] strSplits = str.Split('|');

            for (int i = 0; i < strSplits.Length; i++)
            {
                string[] splits = strSplits[i].Split('_');
                if (splits != null && splits.Length >= 2)
                {
                    int itemid = 0;
                    int probability = 0;
                    int.TryParse(splits[0], out itemid);
                    int.TryParse(splits[1], out probability);

                    CanBeObtainedInscriptionItemData data = new CanBeObtainedInscriptionItemData();
                    data.inscriptionItemData = ItemDataManager.CreateItemDataFromTable(itemid);
                    data.probability = probability;

                    datas.Add(data);
                }
            }

            return datas;
        }

        /// <summary>
        /// 得到最大合成数
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public int GetMaxInscriptionSynthesisNum(int color)
        {
            int maxNum = 0;
            for (int i = 0; i < mInscriptionSysnthesisDataList.Count; i++)
            {
                InscriptionSysnthesisData data = mInscriptionSysnthesisDataList[i];
                if (data.quality != color)
                {
                    continue;
                }

                if (!data.isMaxSynthesisNum)
                {
                    continue;
                }

                maxNum = data.sysnthesisNumber;
                break;
            }

            return maxNum;
        }

        /// <summary>
        /// 可合成的铭文数据
        /// </summary>
        /// <returns></returns>
        public List<CanBeObtainedInscriptionItemData> GetCanBeObtainedInscriptionItemData(int color,int putInInscriptionNumber)
        {
            List<CanBeObtainedInscriptionItemData> datas = new List<CanBeObtainedInscriptionItemData>();

            for (int i = 0; i < mInscriptionSysnthesisDataList.Count; i++)
            {
                InscriptionSysnthesisData data = mInscriptionSysnthesisDataList[i];
                if (data.quality != color)
                {
                    continue;
                }

                if (putInInscriptionNumber != data.sysnthesisNumber)
                {
                    continue;
                }

                datas = data.canBeObtainedInscriptionItemDataList;
                datas.Sort();
                break;
            }

            return datas;
        }

        /// <summary>
        /// 铭文品质页签数据
        /// </summary>
        /// <returns></returns>
        public List<ComControlData> GetInscriptionQualityTabDataList()
        {
            List<ComControlData> tabDataList = new List<ComControlData>();

            for (int i = 0; i <= (int)ItemTable.eColor.YELLOW; i++)
            {
                if (i == (int)ItemTable.eColor.GREEN)
                {
                    continue;
                }

                int index = i;
                string name = TR.Value(string.Format("Inscription_Compose_Quailty_Desc_{0}", index));
                InscriptionQualityTabData tabData = new InscriptionQualityTabData(index, index, name, index == 0);
                tabDataList.Add(tabData);
            }

            return tabDataList;
        }

        /// <summary>
        /// 铭文三类型页签数据
        /// </summary>
        /// <returns></returns>
        public List<ComControlData> GetInscriptionThirdTypeTabDataList()
        {
            List<ComControlData> tabDataList = new List<ComControlData>();

            InscriptionThirdTypeTabData data = new InscriptionThirdTypeTabData(0, 0, TR.Value("Inscription_Compose_ThirdType_Desc_0"), true);
            tabDataList.Add(data);

            for (int i = (int)InscriptionTable.eThirdType.RedInscription; i <= (int)InscriptionTable.eThirdType.VioletInscription; i++)
            {
                int index = i;
                string name = TR.Value(string.Format("Inscription_Compose_ThirdType_Desc_{0}", index));
                InscriptionThirdTypeTabData tabData = new InscriptionThirdTypeTabData(index, index, name, false);
                tabDataList.Add(tabData);
            }

            return tabDataList;
        }
        #endregion

        private int Sort(ItemData x,ItemData y)
        {
            if (x.Quality != y.Quality)
            {
                return y.Quality - x.Quality;
            }

            if (x.ThirdType != y.ThirdType)
            {
                return x.ThirdType - y.ThirdType;
            }

            return x.TableID - y.TableID;
        }
    }
}