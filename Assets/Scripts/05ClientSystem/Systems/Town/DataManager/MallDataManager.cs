using Protocol;
using Network;
using ProtoTable;
using System.Collections.Generic;

namespace GameClient
{
    public class MallDataManager : DataManager<MallDataManager>
    {
        public bool HaveGoodsRecommendReq = false;
        bool m_bNetBind = false;
        bool HaveTips = false;
        public bool OnlineTips = false;
        //public MallItemInfo[] GoodsRecommend = new MallItemInfo[0];
        public List<MallItemInfo> GoodsRecommend = new List<MallItemInfo>();
        private List<MallItemInfo> SortGoods = new List<MallItemInfo>();
        //public List<MallItemInfo> FashionList = new List<MallItemInfo>();
        public Dictionary<int, List<MallItemInfo>> jobFashionDic = new Dictionary<int, List<MallItemInfo>>();
        public bool isInFashionTab = false;

        public int MainTabIndex = -1;
        public int SubTabIndex = -1;

        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.Default;
        }

        public override void Initialize()
        {
            Clear();
            _BindNetMsg();
        }

        public override void Clear()
        {
            _UnBindNetMsg();
            HaveGoodsRecommendReq = false;
            OnlineTips = false;
            HaveTips = false;
            GoodsRecommend.Clear();
            SortGoods.Clear();
            MainTabIndex = -1;
            SubTabIndex = -1;
        }

        void _BindNetMsg()
        {
            if (m_bNetBind == false)
            {
                NetProcess.AddMsgHandler(WorldMallQueryItemRet.MsgID, _OnWorldMallQueryItemRet);
                NetProcess.AddMsgHandler(WorldSyncMallPersonalTailorState.MsgID, _OnWorldMallPersonalTailorStateRet);

                //充值成功发货通知 监听
                NetProcess.AddMsgHandler(SceneBillingSendGoodsNotify.MsgID, OnSyncPayRes);


                m_bNetBind = true;
            }
        }

        void _UnBindNetMsg()
        {
            NetProcess.RemoveMsgHandler(WorldMallQueryItemRet.MsgID, _OnWorldMallQueryItemRet);
            NetProcess.RemoveMsgHandler(WorldSyncMallPersonalTailorState.MsgID, _OnWorldMallPersonalTailorStateRet);

            NetProcess.RemoveMsgHandler(SceneBillingSendGoodsNotify.MsgID, OnSyncPayRes);

            m_bNetBind = false;
        }
        //isPersonGoods用于私人订制
        void _OnWorldMallQueryItemRet(MsgDATA msg)
        {
            WorldMallQueryItemRet msgData = new WorldMallQueryItemRet();
            msgData.decode(msg.bytes);
            if(msgData==null)
            {
                return;
            }
            MallItemInfo[] mallItems = msgData.items;
            bool isPersonGoods = GetIsPersonGoods(mallItems);
            if(isPersonGoods)
            {
                GoodsRecommend.Clear();
                for (int i = 0; i < msgData.items.Length; i++)
                {
                    GoodsRecommend.Add(msgData.items[i]);
                }
#if APPLE_STORE
                //add by mjx for ios appstore
                if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.LIMITTIME_GIFT) == false)
                {
                    HaveGoodsRecommendReq = true;
                }
                else
                {
                    HaveGoodsRecommendReq = false;
                }
#else 			
				HaveGoodsRecommendReq = true;
#endif

                SortGoods.Clear();
                _sortMallItemListByStartTime();
                if(SortGoods != null && SortGoods.Count > 0)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GoodsRecommend, 1, SortGoods[0].id, HaveTips);
                }
                if (HaveTips==true)
                {
                    HaveTips = false;
                    
                }
                
            }
        }

        void _OnWorldMallPersonalTailorStateRet(MsgDATA msg)
        {
            WorldSyncMallPersonalTailorState msgData = new WorldSyncMallPersonalTailorState();
            msgData.decode(msg.bytes);
            if(msgData.state== (byte)MallPersonalTailorActivateState.MPTAS_OPEN|| msgData.state == (byte)MallPersonalTailorActivateState.MPTAS_ONLINE)
            {
                if(msgData.state == (byte)MallPersonalTailorActivateState.MPTAS_ONLINE)
                {
                    OnlineTips = true;
                }
                if(msgData.state == (byte)MallPersonalTailorActivateState.MPTAS_OPEN)
                {
                    OnlineTips = false;
                }
                SendGoodsRecommendReq();
                //HaveGoodsRecommendReq = true;
                HaveTips = true;
            }
            if(msgData.state==(byte)MallPersonalTailorActivateState.MPTAS_CLOSED)
            {
                HaveGoodsRecommendReq = false;
                SendGoodsRecommendReq();
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GoodsRecommend, 2);
            }
        }

        //请求私人订制的内容
        public void SendGoodsRecommendReq()
        {
            WorldMallQueryItemReq req = new WorldMallQueryItemReq();
            req.isPersonalTailor = 1;
            req.tagType = 0;
            req.type = 0;
            req.subType = 0;
            req.moneyType = 0;
            req.occu = 0;
            req.updateTime = 0;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }
        public void SendMallQueryItemReq(int MallTableID, int iMallSubTypeIndex = 0, int JobID = 0, int mainTabIndex = -1, int subTabIndex = -1) // 第2和第3个参数目前只有时装会用到
        {
            MallTypeTable MallData = TableManager.GetInstance().GetTableItem<MallTypeTable>(MallTableID);
            if (MallData == null)
            {
                Logger.LogErrorFormat("MallData is null in [SendMallQueryItemReq], MallTableID = {0}, iMallSubTypeIndex = {1}", MallTableID, iMallSubTypeIndex);
                return;
            }

            WorldMallQueryItemReq req = new WorldMallQueryItemReq();

            if (MallData.MoneyID != 0)
            {
                req.moneyType = (byte)MallData.MoneyID;
            }

            if (MallData.MallType == MallTypeTable.eMallType.SN_HOT)
            {
                req.tagType = 1;
            }
            else
            {
                req.tagType = 0;
                req.type = (byte)MallData.MallType;

                if (MallData.MallSubType.Count > 0 && iMallSubTypeIndex < MallData.MallSubType.Count && MallData.MallSubType[iMallSubTypeIndex] != 0)
                {
                    req.subType = (byte)MallData.MallSubType[iMallSubTypeIndex];
                }
                else if (MallData.MallSubType != null && MallData.MallSubType.Count == 1)
                {
                    req.subType = (byte)MallData.MallSubType[iMallSubTypeIndex];
                }

                if (MallData.ClassifyJob == 1 && JobID > 0)
                {
                    req.occu = (byte)JobID;
                }
            }

            MainTabIndex = mainTabIndex;
            SubTabIndex = subTabIndex;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        public void setFashionDiscount(WorldMallQueryItemRet res , int jobID)
        {
            List<MallItemInfo> FashionList = new List<MallItemInfo>();
            if (FashionList != null)
            {
                FashionList.Clear();
            }

            for (int i = 0; i < res.items.Length; i++)
            {
                FashionList.Add(res.items[i]);
            }

            if (jobFashionDic == null)
            {
                Logger.LogErrorFormat("jobFashionDic is null");
                
            }
            jobFashionDic[jobID] = FashionList;
        }

        public bool haveFashionDiscount(MallItemInfo mallItemInfo)
        {
            bool result = false;
            if (jobFashionDic == null)
            {
                //Logger.LogErrorFormat("jobFashionDic is null");
                return result;
            }
            
            if(!jobFashionDic.ContainsKey(mallItemInfo.jobtype))
            {
               // Logger.LogErrorFormat("jobFashionDic not have this jobtype :{0}", mallItemInfo.jobtype);
                return result;
            }
            var FashionList = jobFashionDic[mallItemInfo.jobtype];
            
            for(int i=0;i<FashionList.Count;i++)
            {
                if(mallItemInfo.goodsSubType == FashionList[i].goodsSubType && FashionList[i].discountRate > 0)
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// 判断回的消息是否是私人定制的类型
        /// </summary>
        /// <param name="mallinfos"></param>
        /// <param name="PersonalTailType"></param>
        /// <returns></returns>
        public bool GetIsPersonGoods(MallItemInfo[] mallinfos)
        {
            if(mallinfos.Length<=0)
            {
                return false;
            }
            for(int i=0;i<mallinfos.Length;i++)
            {
                if(mallinfos[i].isPersonalTailor>0)
                {
                    return true;
                }
            }
            return false;
        }
        private void _sortMallItemListByStartTime()
        {
            for (int i = 0; i < GoodsRecommend.Count; i++)
            {
                MallItemTable mallitemdata = TableManager.GetInstance().GetTableItem<MallItemTable>((int)MallDataManager.GetInstance().GoodsRecommend[i].id);
                if (mallitemdata == null)
                {
                    continue;
                }
                int PersonalTailID = mallitemdata.PersonalTailID;
                if (PersonalTailID != 0)
                {
                    var tabledata = TableManager.GetInstance().GetTableItem<PersonalTailorTriggerTable>(PersonalTailID);
                    if (tabledata == null)
                    {
                        Logger.LogErrorFormat("PersonalTailorTriggerTable is null");
                    }
                    if (tabledata.TypeID == 5)
                    {
                        string[] FashionID = mallitemdata.giftpackitems.Split('|');
                        for (int j = 0; j < FashionID.Length; j++)
                        {
                            string[] ID_true = FashionID[j].Split(':');
                            int result_ID = 0;
                            int.TryParse(ID_true[0], out result_ID);
                            var itemtabledata = TableManager.GetInstance().GetTableItem<ItemTable>(result_ID);
                            if (itemtabledata == null)
                            {
                                Logger.LogErrorFormat("itemtabledata is null");
                                return;
                            }
                            if (itemtabledata.TimeLeft == 604800)
                            {
                                break;
                            }
                            else if (itemtabledata.TimeLeft == 2592000)
                            {
                                break;
                            }
                            else if (itemtabledata.TimeLeft == 0)
                            {
                                SortGoods.Add(GoodsRecommend[i]);
                                break;
                            }
                        }
                    }
                    else
                    {
                        SortGoods.Add(GoodsRecommend[i]);
                    }
                }
            }
            SortGoods.Sort((x, y) =>
            {
                int result;
                if (x.starttime.CompareTo(y.starttime) > 0)
                {
                    result = -1;
                }
                else
                {
                    result = 1;
                }
                return result;
            });
        }

        //商城支付回调 刷新支付界面
        void OnSyncPayRes(MsgDATA data)
        {
            if (SDKInterface.Instance.IsPayResultNotify())
            {
                UIEventSystem.GetInstance().SendUIEvent(GameClient.EUIEventID.OnPayResultNotify, "0");
            }
        }
    }
}
