using UnityEngine;
using System.Collections;
using Protocol;
using Network;
using System.Collections.Generic;

namespace GameClient
{
    class FashionResultData
    {
        public List<ItemData> datas;
        public FashionMergeResultType eFashionMergeResultType;
        public bool bOk;
    }

    public enum FashionType
    {
        FT_NORMAL = 0,//general
        FT_SKY,//general_sky//天夜月华
        FT_GOLD_SKY,//天昼光耀
        FT_LUCENCY_SKY,//透明天空
        FT_NATIONALDAY = 10000,//国庆套
        FT_COUNT,
    }

    class FashionMergeManager : DataManager<FashionMergeManager>
    {
        #region process
        Dictionary<int,List<int>> m_dicOccuToFashion = new Dictionary<int, List<int>>();
        Dictionary<int, int> m_key2Fashions = new Dictionary<int, int>();

        int _getFashionKey(FashionType type,int Occu,int SuitId,ProtoTable.ItemTable.eSubType subType)
        {
            int key = 0;
            key |= (((int)type      ) & 0xFF) << 24;//24-31
            key |= (((int)Occu      ) & 0xFF) << 16;//16-23
            key |= (((int)SuitId    ) & 0xFF) << 08;//08-15
            key |= (((int)subType   ) & 0xFF) << 00;//08-15
            return key;
        }

        public FashionType generateEnumValue(int iType)
        {
            switch (iType)
            {
                case 0:
                    return FashionType.FT_NORMAL;
                case 1:
                    return FashionType.FT_SKY;
                case 2:
                    return FashionType.FT_GOLD_SKY;
                case 3:
                    return FashionType.FT_LUCENCY_SKY;
                case 10000:
                    return FashionType.FT_NATIONALDAY;
            }

            return FashionType.FT_NORMAL;
        }

        bool m_bChangeFashionIsAllMerged = false;
        /// <summary>
        /// 百变换装活动是否全部合出
        /// </summary>
        public bool ChangeFashionIsAllMerged
        {
            get
            {
                return m_bChangeFashionIsAllMerged;
            }
            set
            {
                m_bChangeFashionIsAllMerged = value;
            }
        }

        bool m_bLoadType2OccuToFashions = false;
        protected void _LoadType2OccuToFashions()
        {
            if(!m_bLoadType2OccuToFashions)
            {
                var skyTable = TableManager.GetInstance().GetTable<ProtoTable.FashionComposeSkyTable>();
                var enumerator = skyTable.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(enumerator.Current.Key);
                    ProtoTable.FashionComposeSkyTable skyItem = enumerator.Current.Value as ProtoTable.FashionComposeSkyTable;
                    if (null != item && null != skyItem)
                    {
                        var type = generateEnumValue(skyItem.Type);
                        int key = _getFashionKey(type, skyItem.Occu, skyItem.SuitID, item.SubType);
                        if (m_key2Fashions.ContainsKey(key))
                        {
                            Logger.LogErrorFormat("error key repeated !!! for ProtoTable.FashionComposeSkyTable !!! id = {0} <color=#ffff00>name = {1}</color>",item.ID,item.Name);
                        }
                        else
                        {
                            m_key2Fashions[key] = item.ID;
                        }
                    }
                }

                var normalTable = TableManager.GetInstance().GetTable<ProtoTable.FashionComposeTable>();
                var enumeratorNormal = normalTable.GetEnumerator();
                while (enumeratorNormal.MoveNext())
                {
                    var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(enumeratorNormal.Current.Key);
                    ProtoTable.FashionComposeTable normalItem = enumeratorNormal.Current.Value as ProtoTable.FashionComposeTable;
                    if (null != item && null != normalItem)
                    {
                        var type = FashionType.FT_NORMAL;
                        int key = _getFashionKey(type, normalItem.Occu, normalItem.SuitID, item.SubType);
                        if (m_key2Fashions.ContainsKey(key))
                        {
                            Logger.LogErrorFormat("error key repeated !!! for ProtoTable.FashionComposeSkyTable !!! id = {0} <color=#ffff00>name = {1}</color>", item.ID, item.Name);
                        }
                        else
                        {
                            m_key2Fashions[key] = item.ID;
                        }
                    }
                }
                m_bLoadType2OccuToFashions = true;
            }
        }

        public sealed override void Initialize()
        {
            if(0 == m_dicOccuToFashion.Count)
            {
                var skyTable = TableManager.GetInstance().GetTable<ProtoTable.FashionComposeSkyTable>();
                var enumerator = skyTable.GetEnumerator();
                while(enumerator.MoveNext())
                {
                    var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(enumerator.Current.Key);
                    ProtoTable.FashionComposeSkyTable skyItem = enumerator.Current.Value as ProtoTable.FashionComposeSkyTable;
                    if (null != item && null != skyItem)
                    {
                        if (!m_dicOccuToFashion.ContainsKey(skyItem.Occu))
                        {
                            m_dicOccuToFashion.Add(skyItem.Occu, new List<int>());
                        }
                        m_dicOccuToFashion[skyItem.Occu].Add(item.ID);
                    }
                }

                var normalTable = TableManager.GetInstance().GetTable<ProtoTable.FashionComposeTable>();
                var enumeratorNormal = normalTable.GetEnumerator();
                while (enumeratorNormal.MoveNext())
                {
                    var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(enumeratorNormal.Current.Key);
                    ProtoTable.FashionComposeTable normalItem = enumeratorNormal.Current.Value as ProtoTable.FashionComposeTable;
                    if (null != item && null != normalItem)
                    {
                        if (!m_dicOccuToFashion.ContainsKey(normalItem.Occu))
                        {
                            m_dicOccuToFashion.Add(normalItem.Occu, new List<int>());
                        }
                        m_dicOccuToFashion[normalItem.Occu].Add(item.ID);
                    }
                }
            }

            ServerSceneFuncSwitchManager.GetInstance().AddServerFuncSwitchListener(ServiceType.SERVICE_FASHION_MERGO, _OnServerSwitchFunc);
        }

        public List<int> GetFashionItemsByOccu(int occu)
        {
            if(m_dicOccuToFashion.ContainsKey(occu/10))
            {
                return m_dicOccuToFashion[(occu / 10)];
            }
            return null;
        }

        public int GetFashionByKey(FashionType eFashionType, int occu,int suit,ProtoTable.ItemTable.eSubType subType)
        {
            occu /= 10;
            int key = _getFashionKey(eFashionType, occu, suit, subType);

            if (!m_bLoadType2OccuToFashions)
            {
                _LoadType2OccuToFashions();
            }

            if (m_key2Fashions.ContainsKey(key))
            {
                return m_key2Fashions[key];
            }

            return 0;
        }

        ProtoTable.ItemTable.eSubType[] mSubTypes = new ProtoTable.ItemTable.eSubType[]
        {
            ProtoTable.ItemTable.eSubType.FASHION_HEAD,
            ProtoTable.ItemTable.eSubType.FASHION_CHEST,
            ProtoTable.ItemTable.eSubType.FASHION_EPAULET,
            ProtoTable.ItemTable.eSubType.FASHION_LEG,
            ProtoTable.ItemTable.eSubType.FASHION_SASH,
            ProtoTable.ItemTable.eSubType.FASHION_HAIR,
        };

        public void GetFashionItemsByTypeAndOccu(FashionType eFashionType,int occu,int suit, ref List<int> ids)
        {
            if(!m_bLoadType2OccuToFashions)
            {
                _LoadType2OccuToFashions();
            }

            if(null != ids)
            {
                ids.Clear();
                for (int i = 0; i < mSubTypes.Length; ++i)
                {
                    int key = GetFashionByKey(eFashionType, occu, suit, mSubTypes[i]);
                    if (0 != key)
                    {
                        ids.Add(key);
                    }
                }
            }
        }

        public override void Clear()
        {
            data.bOk = false;
            data.datas.Clear();
            data.eFashionMergeResultType = FashionMergeResultType.FMRT_NORMAL;
            _skySuitId = 1;
            _normalSuitId = 1;
            _fashionType = FashionType.FT_SKY;
            _recordNomalFashionType = FashionType.FT_SKY;
            _FashionPart = ProtoTable.ItemTable.eSubType.ST_NONE;
            mSkyGuids.Clear();
            ChangeFashionIsAllMerged = false;
            ServerSceneFuncSwitchManager.GetInstance().RemoveServerFuncSwitchListener(ServiceType.SERVICE_FASHION_MERGO, _OnServerSwitchFunc);
        }

        private void _OnServerSwitchFunc(ServerSceneFuncSwitch funcSwitch)
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnFashionMergeSwich);
        }
        #endregion

        List<int> mSkyGuids = new List<int>();
        public int redCount
        {
            get
            {
                return mSkyGuids.Count;
            }
        }
        public void ClearRedPoit()
        {
            mSkyGuids.Clear();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnFashionMergeRedCounChanged);
        }

        public delegate void OnFashionMergeChanged(FashionResultData data);
        public OnFashionMergeChanged onFashionMergeChanged;
        FashionResultData data = new FashionResultData
        {
            bOk = false,
            datas = new List<ItemData>(),
            eFashionMergeResultType = FashionMergeResultType.FMRT_NORMAL,
        };

        int _skySuitId = 1;
        public int SkySuitID
        {
            get
            {
                return _skySuitId;
            }
            set
            {
                _skySuitId = value;
            }
        }

        int _normalSuitId = 1;
        public int NormalSuitID
        {
            get
            {
                return _normalSuitId;
            }
            set
            {
                _normalSuitId = value;
            }
        }
        FashionType _fashionType = FashionType.FT_SKY;
        public FashionType FashionType
        {
            get
            {
                return _fashionType;
            }
            set
            {
                if(_fashionType != value)
                {
                    ClearRedPoit();
                }
                _fashionType = value;
            }
        }

        FashionType _recordNomalFashionType = FashionType.FT_SKY;
        /// <summary>
        /// 记录普通时装合成界面当前看那套时装类型
        /// </summary>
        public FashionType RecordNomalFashionType
        {
            get
            {
                return _recordNomalFashionType;
            }
            set
            {
                _recordNomalFashionType = value;
            }
        }

        ProtoTable.ItemTable.eSubType _FashionPart = ProtoTable.ItemTable.eSubType.ST_NONE;
        public ProtoTable.ItemTable.eSubType FashionPart
        {
            get
            {
                return _FashionPart;
            }
            set
            {
                _FashionPart = value;
            }
        }

        ProtoTable.ItemTable.eSubType[] pools = new ProtoTable.ItemTable.eSubType[]
        {
            ProtoTable.ItemTable.eSubType.FASHION_HEAD,
            ProtoTable.ItemTable.eSubType.FASHION_CHEST,
            ProtoTable.ItemTable.eSubType.FASHION_EPAULET,
            ProtoTable.ItemTable.eSubType.FASHION_LEG,
            ProtoTable.ItemTable.eSubType.FASHION_SASH,
        };

        public bool HasSkyFashion(ProtoTable.ItemTable.eSubType sub)
        {
            var items = ItemDataManager.GetInstance().GetItemsByPackageSubType(EPackageType.Fashion, sub);
            items.InsertRange(items.Count, ItemDataManager.GetInstance().GetItemsByPackageSubType(EPackageType.WearFashion, sub));
            for (int j = 0; j < items.Count; ++j)
            {
                var item = ItemDataManager.GetInstance().GetItem(items[j]);
                if (null == item)
                {
                    continue;
                }

                if (item.SubType != (int)sub)
                {
                    continue;
                }

                if (item.FixTimeLeft > 0)
                {
                    continue;
                }

                var skyItem = TableManager.GetInstance().GetTableItem<ProtoTable.FashionComposeSkyTable>(item.TableID);
                if (null == skyItem)
                {
                    continue;
                }

                FashionType eTarget = (FashionType)skyItem.Type;
                if (eTarget != FashionType)
                {
                    continue;
                }

                if (skyItem.Occu != (PlayerBaseData.GetInstance().JobTableID / 10))
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        public ProtoTable.ItemTable.eSubType GetDefaultFashionPart()
        {
            ProtoTable.ItemTable.eSubType ret = ProtoTable.ItemTable.eSubType.FASHION_HEAD;
            int iMask = 0;
            for (int i = 0; i < pools.Length; ++i)
            {
                if(HasSkyFashion(pools[i]))
                {
                    iMask |= (1 << i);
                }
            }

            for(int i = 0; i < pools.Length; ++i)
            {
                if ((iMask & (1 << i)) == 0)
                {
                    ret = pools[i];
                    break;
                }
            }

            return ret;
        }

        public FashionResultData FashionResultData
        {
            get
            {
                return data;
            }
        }

        public int FashionMergeMaterialID
        {
            get
            {
                int meterialid = 0;
                var SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_FASHIOH_MERGE_MATERIAL_ID);
                if (SystemValueTableData != null)
                {
                    meterialid = SystemValueTableData.Value;
                }
                return meterialid;
            }
        }

        public void SendFashionMerge(ulong left,ulong right,ulong mergeBoxID,int skySuitID, int skyFashionID)
        {
            SceneFashionMergeReq kSend = new SceneFashionMergeReq();
            kSend.leftid = left;
            kSend.rightid = right;
            kSend.composer = mergeBoxID;
            kSend.skySuitID = (uint)skySuitID;
            kSend.selFashionID = (uint)skyFashionID;

            NetManager.Instance().SendCommand<SceneFashionMergeReq>(ServerType.GATE_SERVER, kSend);
        }

        public void SendChangeFashionMerge(ulong left,ulong changeTicketID,uint mergeFashionID)
        {
            SceneFashionChangeActiveMergeReq kSend = new SceneFashionChangeActiveMergeReq();
            kSend.fashionId = left;
            kSend.ticketId = changeTicketID;
            kSend.choiceComFashionId = mergeFashionID;

            NetManager.Instance().SendCommand<SceneFashionChangeActiveMergeReq>(ServerType.GATE_SERVER, kSend);
        }

        //[MessageHandle(SceneFashionMergeRes.MsgID)]
        public void OnRecvSceneFashionMergeRes(MsgDATA msg)
        {
            SceneFashionMergeRes ret = new SceneFashionMergeRes();
            ret.decode(msg.bytes);

            if(ret.result == 0)
            {
                data.bOk = true;
                data.eFashionMergeResultType = ret.itemB == 0 ? FashionMergeResultType.FMRT_NORMAL : FashionMergeResultType.FMRT_SPECIAL;
                data.datas.Clear();

                var itemData = ItemDataManager.CreateItemDataFromTable((int)ret.itemA);
                if (itemData != null)
                {
                    itemData.Count = ret.numA;
                    data.datas.Add(itemData);
                }

                itemData = ItemDataManager.CreateItemDataFromTable((int)ret.itemB);
                if (itemData != null)
                {
                    itemData.Count = ret.numB;
                    data.datas.Add(itemData);
                }
                
                itemData = ItemDataManager.CreateItemDataFromTable((int)ret.itemC);
                if (itemData != null)
                {
                    itemData.Count = 1;
                    data.datas.Add(itemData);
                }
/*
#if UNITY_EDITOR
                if (data.datas.Count == 1)
                {
                    data.eFashionMergeResultType = FashionMergeResultType.FMRT_SPECIAL;
                    itemData = ItemDataManager.CreateItemDataFromTable(GetFashionByKey(_fashionType, PlayerBaseData.GetInstance().JobTableID, SkySuitID, (ProtoTable.ItemTable.eSubType)data.datas[0].SubType));
                    data.datas.Add(itemData);
                    itemData = ItemDataManager.CreateItemDataFromTable(531005018);
                    data.datas.Add(itemData);
                }
#endif
*/

                if (data.datas.Count > 0)
                {
                    FashionMergeResultFrame.Open(data);
                }

                if(data.datas.Count >= 2)
                {
                    for(int i = 1; i < data.datas.Count; ++i)
                    {
                        var current = data.datas[i];
                        if(null == current)
                        {
                            continue;
                        }

                        var item = TableManager.GetInstance().GetTableItem<ProtoTable.FashionComposeSkyTable>(current.TableID);
                        if(null == item)
                        {
                            continue;
                        }

                        if(item.Type == (int)_fashionType)
                        {
                            continue;
                        }

                        if(!mSkyGuids.Contains(current.TableID))
                        {
                            mSkyGuids.Add(current.TableID);
                        }

                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnFashionMergeRedCounChanged);
                    }
                }
            }
            else
            {
                data.datas.Clear();
                data.eFashionMergeResultType = FashionMergeResultType.FMRT_NORMAL;
                data.bOk = false;
                SystemNotifyManager.SystemNotify((int)ret.result);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnFashionMergeNotify,data);
        }

        public void OnRecvSceneFashionChangeActiveMergeRet(MsgDATA msg)
        {
            SceneFashionChangeActiveMergeRet ret = new SceneFashionChangeActiveMergeRet();
            ret.decode(msg.bytes);

            if (ret.ret == 0)
            {
                ChangeFashionIsAllMerged = ret.allMerged == 1;
                data.bOk = true;
                data.eFashionMergeResultType = ret.advanceId == 0 ? FashionMergeResultType.FMRT_NORMAL : FashionMergeResultType.FMRT_SPECIAL;
                data.datas.Clear();

                var itemData = ItemDataManager.CreateItemDataFromTable((int)ret.commonId);
                if (itemData != null)
                {
                    data.datas.Add(itemData);
                }

                itemData = ItemDataManager.CreateItemDataFromTable((int)ret.advanceId);
                if (itemData != null)
                {
                    data.datas.Add(itemData);
                }
                
                if (data.datas.Count > 0)
                {
                    FashionMergeResultFrame.Open(data);
                }
                
            }
            else
            {
                data.datas.Clear();
                data.eFashionMergeResultType = FashionMergeResultType.FMRT_NORMAL;
                data.bOk = false;
                SystemNotifyManager.SystemNotify((int)ret.ret);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnFashionMergeNotify, data);
        }

        TipFuncButon mMergeFunction = null;
        public TipFuncButon MergeFunction
        {
            get
            {
                if (null == mMergeFunction)
                {
                    mMergeFunction = new TipFuncButon();
                    mMergeFunction.text = TR.Value("tip_fashion_merge");
                    mMergeFunction.name = "fashion_merge";
                    mMergeFunction.callback = _OnFashionMergeSelItem;
                }

                return mMergeFunction;
            }
        }

        void _OnFashionMergeSelItem(ItemData a_item, object a_data)
        {
            if (null != a_item)
            {
                if(a_item.bFashionItemLocked)
                {
                    SystemNotifyManager.SystemNotify(1000107);
                    return;
                }

                ClientSystemManager.GetInstance().CloseFrame<FashionMergeNewFrame>();
                _FashionPart = (ProtoTable.ItemTable.eSubType)a_item.SubType;
                FashionMergeNewFrame.OpenLinkFrame(string.Format("0|0|{0}|{1}|{2}", (int)FashionMergeManager.GetInstance().FashionType, (int)FashionMergeManager.GetInstance().FashionPart, a_item.GUID));
                ItemTipManager.GetInstance().CloseAll();
            }
        }

        public void SetAutoEquipState(int state)
        {
            SceneFashionMergeRecordReq req = new SceneFashionMergeRecordReq();
            req.handleType = (uint)state;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        public bool IsChangeSectionActivity(FashionType type)
        {
            if (type != FashionType.FT_NATIONALDAY)
            {
                return false;
            }

            return true;
        }
    }
}