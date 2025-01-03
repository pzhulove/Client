using UnityEngine;
using System.Collections.Generic;
using Protocol;
using Network;
using System;
using System.ComponentModel;

namespace GameClient
{
    class BoduData
    {
        public int iTimes;
        public int jarId;
    }

    class BudoManager : DataManager<BudoManager>
    {
        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.BudoManager;
        }

        static int ticketId = 0;
        public static int TicketID
        {
            get
            {
                if(ticketId == 0)
                {
                    var SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_WUDAO_TICKET_ID);
                    if (SystemValueTableData != null)
                    {
                        ticketId = SystemValueTableData.Value;
                    }
                }
                return ticketId;
            }
        }
        static int activeId = 0;
        public static int ActiveID
        {
            get
            {
                return ActiveManager.GetInstance().BudoActive;
                if (activeId == 0)
                {
                    var SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_WUDAO_ACTIVITY_ID);
                    if (SystemValueTableData != null)
                    {
                        activeId = SystemValueTableData.Value;
                    }
                }
                return activeId;
            }
        }

        public bool NeedHintAddParty
        {
            get
            {
                if(!IsOpen)
                {
                    return false;
                }

                if(!IsLevelFit)
                {
                    return false;
                }

                if(TotalTimes > 0)
                {
                    return false;
                }

                if(!CanParty)
                {
                    return false;
                }

                return true;
            }
        }

        public bool IsOpen
        {
            get
            {
                bool bOpen = false;
                ActivityInfo activity = null;
                if (ActiveManager.GetInstance().allActivities.ContainsKey(ActiveID))
                {
                    activity = ActiveManager.GetInstance().allActivities[ActiveID];
                }
                if(activity != null)
                {
                    bOpen = activity.state == 1;
                }
                
                return bOpen;
            }
        }

        public bool IsLevelFit
        {
            get
            {
                bool bLevelFit = false;
                ActivityInfo activity = null;
                if (ActiveManager.GetInstance().allActivities.ContainsKey(ActiveID))
                {
                    activity = ActiveManager.GetInstance().allActivities[ActiveID];
                }
                if (activity != null)
                {
                    bLevelFit = activity.level <= PlayerBaseData.GetInstance().Level;
                }
                return bLevelFit;
            }
        }
        public int NeedLv
        {
            get
            {
                ActivityInfo activity = null;
                if (ActiveManager.GetInstance().allActivities.ContainsKey(ActiveID))
                {
                    activity = ActiveManager.GetInstance().allActivities[ActiveID];
                }
                if (activity != null)
                {
                    return activity.level;
                }
                return 1;
            }
        }
        public bool CanParty
        {
            get
            {
                return eWudaoStatus == WudaoStatus.WUDAO_STATUS_INIT;
            }
        }
        public bool CanMatch
        {
            get
            {
                return eWudaoStatus == WudaoStatus.WUDAO_STATUS_PLAYING;
            }
        }
        public bool CanOpenMatchFrame
        {
            get
            {
                return eWudaoStatus == WudaoStatus.WUDAO_STATUS_PLAYING ||
                    eWudaoStatus == WudaoStatus.WUDAO_STATUS_COMPLETE;
            }
        }

        public bool CanAcqured
        {
            get
            {
                return eWudaoStatus == WudaoStatus.WUDAO_STATUS_COMPLETE;
            }
        }
        int iMaxWinTimes = 0;
        public int MaxWinTimes
        {
            get
            {
                return iMaxWinTimes;
            }
        }
        int iMaxLoseTimes = 0;
        public int MaxLoseTimes
        {
            get
            {
                return iMaxLoseTimes;
            }
        }
        int iWinTimes = 0;
        public int WinTimes
        {
            get
            {
                CounterInfo info = CountDataManager.GetInstance().GetCountInfo("wudao_win");
                if (info != null)
                {
                    return (int)info.value;
                }
                return 0;
            }
        }
        int iLosTimes = 0;
        public int LoseTimes
        {
            get
            {
                CounterInfo info = CountDataManager.GetInstance().GetCountInfo("wudao_lose");
                if (info != null)
                {
                    return (int)info.value;
                }
                return 0;
            }
        }
        int iTotalTimes = 0;
        public int TotalTimes
        {
            get
            {
                CounterInfo info = CountDataManager.GetInstance().GetCountInfo("wudao_times");
                if (info != null)
                {
                    return (int)info.value;
                }
                return 0;
            }
        }
        WudaoStatus eWudaoStatus = WudaoStatus.WUDAO_STATUS_INIT;
        int iBudoStatus = 0;
        bool bNeedOpenBudoInfoFrame = false;
        public bool NeedOpenBudoInfoFrame
        {
            get
            {
                return bNeedOpenBudoInfoFrame;
            }
            set
            {
                bNeedOpenBudoInfoFrame = value;
            }
        }
        public int BudoStatus
        {
            get
            {
                return iBudoStatus;
            }
            set
            {
                iBudoStatus = value;
                eWudaoStatus = (WudaoStatus)value;
                if (onBudoInfoChanged != null)
                {
                    onBudoInfoChanged.Invoke();
                }
            }
        }
        List<ProtoTable.BudoAwardTable> m_akBudoJars = new List<ProtoTable.BudoAwardTable>();
        public List<ProtoTable.BudoAwardTable> BudoJars
        {
            get
            {
                return m_akBudoJars;
            }
        }

        public int GetPreJarIdByTimes()
        {
            int iJarID = 0;
            if (m_akBudoJars.Count > 0)
            {
                iJarID = m_akBudoJars[0].ID;
            }

            int iRealWinTimes = WinTimes;
            int iPreWinTimes = iRealWinTimes > 0 ? iRealWinTimes - 1 : iRealWinTimes;

            for (int i = 0; i < m_akBudoJars.Count; ++i)
            {
                if (iPreWinTimes >= m_akBudoJars[i].Times && iPreWinTimes <= m_akBudoJars[i].MaxTimes)
                {
                    iJarID = m_akBudoJars[i].ID;
                    break;
                }
            }

            return iJarID;
        }

        public ItemData GetPreJarDataByTimes()
        {
            int iJarID = GetPreJarIdByTimes();

            return ItemDataManager.GetInstance().GetCommonItemTableDataByID(iJarID);
        }

        public ProtoTable.BudoAwardTable GetJarItemByTimes()
        {
            int iJarId = GetJarIdByTimes();
            var jarItem = TableManager.GetInstance().GetTableItem<ProtoTable.BudoAwardTable>(iJarId);
            return jarItem;
        }

        public ProtoTable.BudoAwardTable GetPreJarItemByTimes()
        {
            int iJarId = GetPreJarIdByTimes();
            var jarItem = TableManager.GetInstance().GetTableItem<ProtoTable.BudoAwardTable>(iJarId);
            return jarItem;
        }

        public int GetJarIdByTimes()
        {
            int iJarID = 0;
            if (m_akBudoJars.Count > 0)
            {
                iJarID = m_akBudoJars[0].ID;
            }

            int iRealWinTimes = WinTimes;

            for (int i = 0; i < m_akBudoJars.Count; ++i)
            {
                if (iRealWinTimes >= m_akBudoJars[i].Times && iRealWinTimes <= m_akBudoJars[i].MaxTimes)
                {
                    iJarID = m_akBudoJars[i].ID;
                    break;
                }
            }
            return iJarID;
        }

        public ItemData GetJarDataByTimes()
        {
            int iJarID = GetJarIdByTimes();

            return ItemDataManager.GetInstance().GetCommonItemTableDataByID(iJarID);
        }
        public delegate void OnBudoInfoChanged();
        public OnBudoInfoChanged onBudoInfoChanged;
        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            RegisterNetHandler();

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
            var SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_WUDAO_MAX_LOSE_NUM);
            if(SystemValueTableData != null)
            {
                iMaxLoseTimes = SystemValueTableData.Value;
            }
            SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_WUDAO_MAX_WIN_NUM);
            if (SystemValueTableData != null)
            {
                iMaxWinTimes = SystemValueTableData.Value;
            }

            SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_WUDAO_TICKET_ID);
            if (SystemValueTableData != null)
            {
                ticketId = SystemValueTableData.Value;
            }

            SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_WUDAO_ACTIVITY_ID);
            if (SystemValueTableData != null)
            {
                activeId = SystemValueTableData.Value;
            }

            m_akBudoJars.Clear();
            var budoItemTable = TableManager.GetInstance().GetTable<ProtoTable.BudoAwardTable>();
            if (budoItemTable != null)
            {
                var enumerator = budoItemTable.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    m_akBudoJars.Add(enumerator.Current.Value as ProtoTable.BudoAwardTable);
                }
                m_akBudoJars.Sort((x, y) =>
                {
                    return x.Times - y.Times;
                });
            }
        }

        /// <summary>
        /// 清理
        /// </summary>
        public override void Clear()
        {
            UnRegisterNetHandler();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
        }

        void RegisterNetHandler()
        {
            NetProcess.AddMsgHandler(SceneWudaoRewardRes.MsgID, OnRecvSceneWudaoRewardRes);
            NetProcess.AddMsgHandler(SceneWudaoJoinRes.MsgID, OnRecvSceneWudaoJoinRes);
        }

        void UnRegisterNetHandler()
        {
            NetProcess.RemoveMsgHandler(SceneWudaoRewardRes.MsgID, OnRecvSceneWudaoRewardRes);
            NetProcess.RemoveMsgHandler(SceneWudaoJoinRes.MsgID, OnRecvSceneWudaoJoinRes);
        }

        void _OnCountValueChanged(UIEvent a_event)
        {
            CounterInfo info = CountDataManager.GetInstance().GetCountInfo(a_event.Param1 as string);
            if (info != null)
            {
                OnCountChanged(info);
            }
        }

        void OnCountChanged(CounterInfo info)
        {
            if(info.name == "wudao_times")
            {
                iTotalTimes = (int)info.value;
            }
            else if (info.name == "wudao_win")
            {
                iWinTimes = (int)info.value;
            }
            else if (info.name == "wudao_lose")
            {
                iLosTimes = (int)info.value;
            }
            else
            {
                return;
            }
            if(onBudoInfoChanged != null)
            {
                onBudoInfoChanged.Invoke();
            }
        }

        public void SendAddParty()
        {
            SceneWudaoJoinReq kSend = new SceneWudaoJoinReq();
            NetManager.Instance().SendCommand<SceneWudaoJoinReq>(ServerType.GATE_SERVER, kSend);
        }

        public void SendMatchParty()
        {
            if(!CanMatch)
            {
                return;
            }

            WorldMatchStartReq kSend = new WorldMatchStartReq();
            kSend.type = (byte)PkType.Pk_Wudao;
            NetManager.Instance().SendCommand<WorldMatchStartReq>(ServerType.GATE_SERVER, kSend);

            WaitNetMessageManager.GetInstance().Wait<WorldMatchStartRes>(msgRet =>
            {
                if (msgRet == null)
                {
                    return;
                }

                if (msgRet.result != 0)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.result);

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchFailed);
                    return;
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchStartSuccess);
            });
        }

        //[MessageHandle(SceneWudaoJoinRes.MsgID)]
        void OnRecvSceneWudaoJoinRes(MsgDATA msg)
        {
            int pos = 0;
            SceneWudaoJoinRes msgRet = new SceneWudaoJoinRes();
            msgRet.decode(msg.bytes, ref pos);

            if(msgRet.result != 0)
            {
                SystemNotifyManager.SystemNotify((int)msgRet.result);
            }
            else
            {
                //开启武道大会PIPEI界面
                GotoPvpBudo();
            }
        }

        bool bReturnFromPk = false;
        public bool ReturnFromPk
        {
            get
            {
                return bReturnFromPk;
            }
            set
            {
                bReturnFromPk = value;
            }
        }

        PKResult m_ePKResult = PKResult.INVALID;
        public PKResult pkResult
        {
            get
            {
                return m_ePKResult;
            }
            set
            {
                m_ePKResult = value;
            }
        }

        public void TryBeginActive()
        {
            if (CanAcqured)
            {
                BudoResultFrameData data = new BudoResultFrameData();
                data.bOver = true;
                data.bNeedOpenBudoInfo = true;
                BudoResultFrame.Open(data);
            }
            else if (!IsOpen || CanParty)
            {
                BoduInfoFrame.Open();
            }
            else
            {
                if (TeamDataManager.GetInstance().HasTeam())
                {
                    SystemNotifyManager.SystemNotify(1104);
                    return;
                }
                GotoPvpBudo();
            }
        }

        public void GotoPvpBudo()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            ProtoTable.CitySceneTable TownTableData = TableManager.instance.GetTableItem<ProtoTable.CitySceneTable>(systemTown.CurrentSceneID);
            if (TownTableData == null)
            {
                return;
            }

            if (systemTown.CurrentSceneID != TownTableData.BudoSceneID && TownTableData.BudoSceneID > 0)
            {
                GameFrameWork.instance.StartCoroutine(systemTown._NetSyncChangeScene(
                    new SceneParams
                    {
                        currSceneID = systemTown.CurrentSceneID,
                        currDoorID = 0,
                        targetSceneID = TownTableData.BudoSceneID,
                        targetDoorID = 0,
                    }));

                ClientSystemManager.GetInstance().CloseFrame<PkWaitingRoom>();
            }
        }

        public void SendSceneWudaoRewardReq()
        {
            SceneWudaoRewardReq kSend = new SceneWudaoRewardReq();
            NetManager.Instance().SendCommand<SceneWudaoRewardReq>(ServerType.GATE_SERVER, kSend);
        }

        public void OpenBudoPreviewFrame(int iTableID)
        {
            var datas = GetPreviewItems(iTableID);
            if(datas != null && datas.Count > 0)
            {
                BudoRewardsFrameData data = new BudoRewardsFrameData();
                data.datas.Clear();
                data.datas.AddRange(datas);
                data.bJustPreView = true;
                data.title = TR.Value("budo_award_title0");

                data.ReceiveItemDataModelList = GetReceiveItemDataModelList(iTableID);

                BudoRewardsFrame.Open(data);
            }
        }

        public List<ReceiveItemDataModel> GetReceiveItemDataModelList(int awardTableId)
        {
            var awardTable = TableManager.GetInstance().GetTableItem<ProtoTable.BudoAwardTable>(awardTableId);
            if (awardTable == null)
                return null;

            var receiveItemDataModelList = CommonUtility.GetReceiveItemDataModelBySplitString(awardTable.GetPreview);

            return receiveItemDataModelList;
        }

        public List<ItemData> GetPreviewItems(int iTableID)
        {
            var budoItem = TableManager.GetInstance().GetTableItem<ProtoTable.BudoAwardTable>(iTableID);
            if(budoItem == null)
            {
                return null;
            }

            int winNum = WinTimes;
            int index = winNum - budoItem.Times;
            if(index < 0 || index >= budoItem.JarType.Count)
            {
                return null;
            }
            int jarType = budoItem.JarType[index];

            List<ItemData> datas = new List<ItemData>();
            JarData jarData = JarDataManager.GetInstance().GetJarData(jarType);
            if (jarData != null)
            {
                for(int i = 0; i < jarData.arrBonusItems.Count; i++)
                {
                    ItemData item = ItemDataManager.CreateItemDataFromTable(jarData.arrBonusItems[i].ItemID);
                    if(item == null)
                    {
                        continue;
                    }

                    item.Count = jarData.arrBonusItems[i].Count;

                    datas.Add(item);
                }            
            }
            
            return datas;
        }

        //[MessageHandle(SceneWudaoRewardRes.MsgID)]
        void OnRecvSceneWudaoRewardRes(MsgDATA msg)
        {
            int pos = 0;
            SceneWudaoRewardRes msgRet = new SceneWudaoRewardRes();
            msgRet.decode(msg.bytes, ref pos);

            if(msgRet.result == 0)
            {
                List<ItemData> items = new List<ItemData>();
                for (int i = 0; i < msgRet.getItems.Length; ++i)
                {
                    ItemReward reward = msgRet.getItems[i];
                    ItemData item = ItemDataManager.CreateItemDataFromTable((int)reward.id);
                    item.Count = (int)reward.num;
                    items.Add(item);
                }

                BudoRewardsFrameData data = new BudoRewardsFrameData();
                data.datas.AddRange(items);
                data.bJustPreView = false;
                data.title = TR.Value("budo_award_title1");
                BudoRewardsFrame.Open(data);
            }
            else
            {
                SystemNotifyManager.SystemNotify((int)msgRet.result);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BudoRewardReturn);
        }

        public bool SendReturnToTownRelation()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (systemTown == null)
            {
                return false;
            }
            ProtoTable.CitySceneTable TownTableData = TableManager.instance.GetTableItem<ProtoTable.CitySceneTable>(systemTown.CurrentSceneID);
            if (TownTableData == null)
            {
                return false;
            }

            if(TownTableData.SceneSubType == ProtoTable.CitySceneTable.eSceneSubType.BUDO)
            {
                GameFrameWork.instance.StartCoroutine(systemTown._NetSyncChangeScene(new SceneParams
                {
                    onSceneLoadFinish = () =>
                    {
                        BoduInfoFrame.Open();
                    },
                }, true));
            }

            return true;
        }
    }
}