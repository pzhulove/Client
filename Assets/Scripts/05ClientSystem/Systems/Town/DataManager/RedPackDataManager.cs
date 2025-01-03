using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Network;
using Protocol;
using System.Collections;
using UnityEngine;
using ProtoTable;

namespace GameClient
{
    class RedPackDataManager : DataManager<RedPackDataManager>
    {
        public int NewYearRedPackActivityID = 0;
        public int NewYearRedPackRankListActivityID = 0;

        Dictionary<ulong, RedPacketBaseEntry> m_dictRedPackets = new Dictionary<ulong, RedPacketBaseEntry>();
        Dictionary<RedPacketType, List<ulong>> m_dictPacketType = new Dictionary<RedPacketType, List<ulong>>();

        Dictionary<ulong, RedPacketRecord> redPacketRecords = new Dictionary<ulong, RedPacketRecord>();
        Dictionary<SendRedPackType, GuildRedPacketSpecInfo> guildRedPacketSpecInfos = new Dictionary<SendRedPackType, GuildRedPacketSpecInfo>();
        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.RedPackDataManager;
        }

        public override void Initialize()
        {
            Clear();
            _BindNetMsg();

            var data = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_NEW_YEAR_RED_PACKET_ACTIVITY_ID);
            NewYearRedPackActivityID = data.Value;

            var data2 = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_NEW_YEAR_RED_PACKET_SORT_LIST_ACTIVITY_ID);
            NewYearRedPackRankListActivityID = data2.Value;
            redPacketRecords = new Dictionary<ulong, RedPacketRecord>();
            guildRedPacketSpecInfos = new Dictionary<SendRedPackType, GuildRedPacketSpecInfo>();
        }

        public override void Clear()
        {
            _UnBindNetMsg();

            NewYearRedPackActivityID = 0;
            NewYearRedPackRankListActivityID = 0;

            m_dictRedPackets.Clear();
            m_dictPacketType.Clear();
            redPacketRecords = null;
            guildRedPacketSpecInfos = null;
        }

        public RedPacketBaseEntry GetRedPacketBaseInfo(ulong a_uID)
        {
            RedPacketBaseEntry data = null;
            m_dictRedPackets.TryGetValue(a_uID, out data);
            return data;
        }
        public string GetGuildRedPacketTitleName(RedPacketBaseEntry redPacketBaseEntry)
        {
            if(redPacketBaseEntry == null)
            {
                return "";
            }
            RedPacketTable redPacketTable = TableManager.GetInstance().GetTableItem<RedPacketTable>(redPacketBaseEntry.reason);
            if(redPacketTable == null)
            {
                return "";
            }
            if(redPacketTable.ThirdType == RedPacketTable.eThirdType.GUILD_ALL)
            {
                return TR.Value("guild_red_packet_all_members");
            }
            if(redPacketTable.ThirdType == RedPacketTable.eThirdType.GUILD_BATTLE)
            {
                DateTime dateTime = TimeUtility.GetDateTimeByTimeStamp((int)redPacketBaseEntry.guildTimeStamp);
                return TR.Value("guild_red_packet_take_part_in_guild_battle", dateTime.Month, dateTime.Day);
            }
            if (redPacketTable.ThirdType == RedPacketTable.eThirdType.GUILD_CROSS_BATTLE)
            {
                DateTime dateTime = TimeUtility.GetDateTimeByTimeStamp((int)redPacketBaseEntry.guildTimeStamp);
                return TR.Value("guild_red_packet_take_part_in_cross_guild_battle", dateTime.Month, dateTime.Day);
            }
            if (redPacketTable.ThirdType == RedPacketTable.eThirdType.GUILD_DUNGEON)
            {
                DateTime dateTime = TimeUtility.GetDateTimeByTimeStamp((int)redPacketBaseEntry.guildTimeStamp);
                return TR.Value("guild_red_packet_take_part_in_guild_dungeon", dateTime.Month, dateTime.Day);
            }
            return "";
        }
        public string GetRedPacketStateText(int state)
        {
            RedPacketStatus redPacketStatus = (RedPacketStatus)(state);
            if(redPacketStatus == RedPacketStatus.RECEIVED)
            {
                return TR.Value("guild_red_packet_received");
            }
            else if(redPacketStatus == RedPacketStatus.EMPTY)
            {
                return TR.Value("guild_red_packet_empty");
            }
            return "";
        }

        public List<RedPacketBaseEntry> GetRedPacketsByType(RedPacketType a_eType)
        {
            List<RedPacketBaseEntry> arrData = new List<RedPacketBaseEntry>();
            List<ulong> arrIDs = null;
            m_dictPacketType.TryGetValue(a_eType, out arrIDs);
            if (arrIDs != null)
            {
                for (int i = 0; i < arrIDs.Count; ++i)
                {
                    arrData.Add(m_dictRedPackets[arrIDs[i]]);
                }
            }
            return arrData;
        }

        public RedPacketBaseEntry GetFirstRedPacketToOpen()
        {
            var iter = m_dictRedPackets.GetEnumerator();
            while (iter.MoveNext())
            {
                RedPacketBaseEntry data = iter.Current.Value;
                if(data.opened == 0)
                {
                    if (data.status == (byte)RedPacketStatus.WAIT_RECEIVE && data.opened == 0)
                    {
                        return data;
                    }
                }
            }
            return null;
        }

        public Dictionary<ulong,RedPacketRecord> GetRedPacketRecords()
        {
            return redPacketRecords;
        }
        // 获取发送公会红包的特殊信息
        public Dictionary<SendRedPackType, GuildRedPacketSpecInfo> GetGuildRedPacketSpecInfos()
        {
            return guildRedPacketSpecInfos;
        }
        public GuildRedPacketSpecInfo GetGuildRedPacketSpecInfo(SendRedPackType sendRedPackType)
        {
            return guildRedPacketSpecInfos.SafeGetValue(sendRedPackType);
        }
        public RedPacketRecord GetRedPacketRecord(ulong guid)
        {
            if(redPacketRecords == null)
            {
                return null;
            }
            if(redPacketRecords.ContainsKey(guid))
            {
                return redPacketRecords[guid];
            }
            return null;
        }
        public int GetWaitOpenCount()
        {
            int nCount = 0;

            var iter = m_dictRedPackets.GetEnumerator();
            while (iter.MoveNext())
            {
                RedPacketBaseEntry data = iter.Current.Value;
                if (data.status == (byte)RedPacketStatus.WAIT_RECEIVE && data.opened == 0)
                {
                    nCount++;
                }
            }

            return nCount;
        }

        public string GetGetMoneyIcon(int a_nTableID)
        {
            ProtoTable.RedPacketTable tableData = TableManager.GetInstance().GetTableItem<ProtoTable.RedPacketTable>(a_nTableID);
            if (tableData == null)
            {
                Logger.LogErrorFormat("红包表找不到ID为{0}的数据", a_nTableID);
                return string.Empty;
            }
            var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(tableData.GetMoneyID);
            if (itemData != null)
            {
                return itemData.Icon;
            }
            return string.Empty;        
        }

        public string GetCostMoneyIcon(int a_nTableID)
        {
            ProtoTable.RedPacketTable tableData = TableManager.GetInstance().GetTableItem<ProtoTable.RedPacketTable>(a_nTableID);
            if (tableData == null)
            {
                Logger.LogErrorFormat("红包表找不到ID为{0}的数据", a_nTableID);
                return string.Empty;
            }
            var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(tableData.CostMoneyID);
            if(itemData != null)
            {
                return itemData.Icon;
            }
            return string.Empty;
        }

        public void SendWorldGetGuildRedPacketInfoReq()
        {
            WorldGetGuildRedPacketInfoReq msg = new WorldGetGuildRedPacketInfoReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);
        }
        public void SendRedPacket(ushort a_uID, int a_nCount, string des)
        {
            WorldSendCustomRedPacketReq msg = new WorldSendCustomRedPacketReq();

            msg.reason = a_uID;
            msg.num = (byte)a_nCount;
            msg.name = des;
            uint nTimes = (uint)CountDataManager.GetInstance().GetCount("guild_pay_rp") + 1;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<WorldSendCustomRedPacketRes>(msgRet =>
            {
                if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.result);
                }
                else
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("红包发送成功");
                    RedPacketTable redPacketTable = TableManager.GetInstance().GetTableItem<RedPacketTable>(a_uID);
                    if(redPacketTable != null && redPacketTable.ThirdType != RedPacketTable.eThirdType.INVALID)
                    {
                        CountDataManager.GetInstance().SetCount("guild_pay_rp",nTimes);
                        if(nTimes >= GuildDataManager.GetInstance().GetDailySendRedPacketMaxCount())
                        {
                            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("guild_red_packet_daily_left_count_zero"));
                        }
                    }
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPacketSendSuccess, a_uID);
                }
            });
        }

        public void OpenRedPacket(ulong a_uID)
        {
            RedPacketBaseEntry redPacketBaseEntry = GetRedPacketBaseInfo(a_uID);
            if(redPacketBaseEntry == null)
            {
                return;
            }
 
            // 发新年红包必须得开相应的活动，公会红包无此限制
            if (redPacketBaseEntry.type == (byte)RedPacketType.NEW_YEAR)
            {
                ActiveManager.ActiveData activeData = ActiveManager.GetInstance().GetActiveData(NewYearRedPackRankListActivityID);                
                if (activeData == null)
                {
                    return;
                }

                if (PlayerBaseData.GetInstance().Level < activeData.mainInfo.level)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(string.Format("等级不足{0}级,无法领取红包", activeData.mainInfo.level));
                    return;
                }
            }                 

            WorldOpenRedPacketReq msg = new WorldOpenRedPacketReq();
            msg.id = a_uID;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<WorldOpenRedPacketRes>(msgRet =>
            {
                if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.result);
                }
                else
                {
                    RedPacketBaseEntry data = GetRedPacketBaseInfo(msgRet.detail.baseEntry.id);

                    if (data == null)
                    {
                        SystemNotifyManager.SysNotifyTextAnimation("红包已抢完");
                        return;
                    }
//                     else
//                     {
//                         if (data.status == (int)RedPacketStatus.EMPTY)
//                         {
//                             SystemNotifyManager.SysNotifyTextAnimation("红包已抢完");
//                             return;
//                         }
//                     }

                    if(data.opened == 1)
                    {
                        ShowOpenedRedPacketFrame.showRedPacketMode = ShowRedPacketMode.Show;
                    }
                    data.status = msgRet.detail.baseEntry.status;
                    data.opened = msgRet.detail.baseEntry.opened;

                    ClientSystemManager.GetInstance().OpenFrame<OpenRedPacketFrame>(FrameLayer.Middle, msgRet.detail);
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPacketOpenSuccess, msgRet.detail);
                    RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.GuildRedPacket);
                }
            });
        }

        public void CheckRedPacket(ulong a_uID)
        {
            WorldOpenRedPacketReq msg = new WorldOpenRedPacketReq();
            msg.id = a_uID;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<WorldOpenRedPacketRes>(msgRet =>
            {
                if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.result);
                }
                else
                {
                    RedPacketBaseEntry data = GetRedPacketBaseInfo(msgRet.detail.baseEntry.id);
                    data.status = msgRet.detail.baseEntry.status;
                    data.opened = msgRet.detail.baseEntry.opened;

                    ShowOpenedRedPacketFrame.showRedPacketMode = ShowRedPacketMode.Show;
                    ClientSystemManager.GetInstance().OpenFrame<ShowOpenedRedPacketFrame>(FrameLayer.Middle, msgRet.detail);

                    //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPacketCheckSuccess, msgRet.detail);
                }
            });
        }

        void _BindNetMsg()
        {
            NetProcess.AddMsgHandler(WorldSyncRedPacket.MsgID, _OnNetInitRedPack);
            NetProcess.AddMsgHandler(WorldNotifyGotNewRedPacket.MsgID, _OnNetGetRedPacket);
            NetProcess.AddMsgHandler(WorldNotifyNewRedPacket.MsgID, _OnNetAddRedPacket);
            NetProcess.AddMsgHandler(WorldNotifyDelRedPacket.MsgID, _OnNetRemoveRedPacket);
            NetProcess.AddMsgHandler(WorldNotifySyncRedPacketStatus.MsgID, _OnNetSyncRedPacketState);
            NetProcess.AddMsgHandler(WorldSyncRedPacketRecord.MsgID, _OnWorldSyncRedPacketRecord);
            NetProcess.AddMsgHandler(WorldGetGuildRedPacketInfoRes.MsgID, _OnWorldGetGuildRedPacketInfoRes);
        }

        void _UnBindNetMsg()
        {
            NetProcess.RemoveMsgHandler(WorldSyncRedPacket.MsgID, _OnNetInitRedPack);
            NetProcess.RemoveMsgHandler(WorldNotifyGotNewRedPacket.MsgID, _OnNetGetRedPacket);
            NetProcess.RemoveMsgHandler(WorldNotifyNewRedPacket.MsgID, _OnNetAddRedPacket);
            NetProcess.RemoveMsgHandler(WorldNotifyDelRedPacket.MsgID, _OnNetRemoveRedPacket);
            NetProcess.RemoveMsgHandler(WorldNotifySyncRedPacketStatus.MsgID, _OnNetSyncRedPacketState);
            NetProcess.RemoveMsgHandler(WorldSyncRedPacketRecord.MsgID, _OnWorldSyncRedPacketRecord);
            NetProcess.RemoveMsgHandler(WorldGetGuildRedPacketInfoRes.MsgID, _OnWorldGetGuildRedPacketInfoRes);
        }





        void _OnNetInitRedPack(MsgDATA a_data)
        {
            if(a_data == null)
            {
                m_dictRedPackets.Clear();
                m_dictPacketType.Clear();
                return;
            }

            WorldSyncRedPacket msgData = new WorldSyncRedPacket();
            msgData.decode(a_data.bytes);

            m_dictRedPackets.Clear();
            m_dictPacketType.Clear();

            for (int i = 0; i < msgData.entrys.Length; ++i)
            {
                RedPacketBaseEntry temp = msgData.entrys[i];
                m_dictRedPackets.Add(temp.id, temp);

                RedPacketType type = (RedPacketType)temp.type;
                if (m_dictPacketType.ContainsKey(type) == false)
                {
                    m_dictPacketType.Add(type, new List<ulong>());
                }
                m_dictPacketType[type].Add(temp.id);
            }
        }

        void _OnNetGetRedPacket(MsgDATA a_data)
        {
            WorldNotifyGotNewRedPacket msgData = new WorldNotifyGotNewRedPacket();
            msgData.decode(a_data.bytes);

            if (m_dictRedPackets.ContainsKey(msgData.entry.id) == false)
            {
                m_dictRedPackets.Add(msgData.entry.id, msgData.entry);

                RedPacketType type = (RedPacketType)msgData.entry.type;
                if (m_dictPacketType.ContainsKey(type) == false)
                {
                    m_dictPacketType.Add(type, new List<ulong>());
                }
                m_dictPacketType[type].Add(msgData.entry.id);
            }

            List<ulong> arrIDs = new List<ulong>();
            arrIDs.Add(msgData.entry.id);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPacketGet, arrIDs);
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.GuildRedPacket);
        }

        void _OnNetAddRedPacket(MsgDATA a_data)
        {
            WorldNotifyNewRedPacket msgData = new WorldNotifyNewRedPacket();
            msgData.decode(a_data.bytes);

            List<ulong> arrIDs = new List<ulong>();

            for (int i = 0; i < msgData.entry.Length; ++i)
            {
                RedPacketBaseEntry temp = msgData.entry[i];
                if (m_dictRedPackets.ContainsKey(temp.id) == false)
                {
                    m_dictRedPackets.Add(temp.id, temp);

                    RedPacketType type = (RedPacketType)temp.type;
                    if (m_dictPacketType.ContainsKey(type) == false)
                    {
                        m_dictPacketType.Add(type, new List<ulong>());
                    }
                    m_dictPacketType[type].Add(temp.id);

                    arrIDs.Add(temp.id);
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPacketGet, arrIDs);
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.GuildRedPacket);
        }

        void _OnNetRemoveRedPacket(MsgDATA a_data)
        {
            WorldNotifyDelRedPacket msgData = new WorldNotifyDelRedPacket();
            msgData.decode(a_data.bytes);

            for (int i = 0; i < msgData.redPacketList.Length; ++i)
            {
                ulong id = msgData.redPacketList[i];
                if (m_dictRedPackets.ContainsKey(id))
                {
                    RedPacketBaseEntry data = m_dictRedPackets[id];
                    m_dictRedPackets.Remove(id);
                    m_dictPacketType[(RedPacketType)data.type].Remove(id);
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPacketDelete, msgData.redPacketList);
        }

        void _OnNetSyncRedPacketState(MsgDATA a_data)
        {
            WorldNotifySyncRedPacketStatus msgData = new WorldNotifySyncRedPacketStatus();
            msgData.decode(a_data.bytes);

            if (m_dictRedPackets.ContainsKey(msgData.id))
            {
                m_dictRedPackets[msgData.id].status = msgData.status;
            }
        }
        void CopyRedPacketData(ref RedPacketRecord redPacketRecord1,ref RedPacketRecord redPacketRecord2)
        {
            if(redPacketRecord1 == null || redPacketRecord2 == null)
            {
                return;
            }
            redPacketRecord1.guid = redPacketRecord2.guid;
            redPacketRecord1.isBest = redPacketRecord2.isBest;
            redPacketRecord1.moneyId = redPacketRecord2.moneyId;
            redPacketRecord1.moneyNum = redPacketRecord2.moneyNum;
            redPacketRecord1.redPackOwnerName = redPacketRecord2.redPackOwnerName;
            redPacketRecord1.time = redPacketRecord2.time;
            return;
        }
        SendRedPackType GetSendRedPackType(RedPacketTable.eThirdType thirdType)
        {
            switch(thirdType)
            {
                case RedPacketTable.eThirdType.GUILD_ALL:
                    return SendRedPackType.GuildMember;
                case RedPacketTable.eThirdType.GUILD_BATTLE:
                    return SendRedPackType.GuildWar;
                case RedPacketTable.eThirdType.GUILD_CROSS_BATTLE:
                    return SendRedPackType.CrossGuildWar;
                case RedPacketTable.eThirdType.GUILD_DUNGEON:
                    return SendRedPackType.GuildDungeon;
                default:
                    return SendRedPackType.Invalid;
            }
        }
        void _OnWorldGetGuildRedPacketInfoRes(MsgDATA a_data)
        {
            if(guildRedPacketSpecInfos == null)
            {
                return;
            }
            WorldGetGuildRedPacketInfoRes msgData = new WorldGetGuildRedPacketInfoRes();
            msgData.decode(a_data.bytes);
            if(msgData.code != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.code);
                return;
            }
            guildRedPacketSpecInfos.Clear();
            if(msgData.infos != null)
            {
                for (int i = 0; i < msgData.infos.Length; i++)
                {
                    GuildRedPacketSpecInfo guildRedPacketSpecInfo = msgData.infos[i];
                    if (guildRedPacketSpecInfo == null)
                    {
                        continue;
                    }
                    SendRedPackType sendRedPackType = GetSendRedPackType((RedPacketTable.eThirdType)guildRedPacketSpecInfo.type);
                    if(sendRedPackType == SendRedPackType.Invalid)
                    {
                        continue;
                    }
                    guildRedPacketSpecInfos.SafeAdd(sendRedPackType, guildRedPacketSpecInfo);
                }
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUpdateGuildRedPacketSpecInfo);
        }
        void _OnWorldSyncRedPacketRecord(MsgDATA a_data)
        {
            if (redPacketRecords == null)
            {
                return;
            }
            WorldSyncRedPacketRecord msgData = new WorldSyncRedPacketRecord();
            msgData.decode(a_data.bytes);
            if(msgData.dels != null)
            {
                for(int i = 0;i < msgData.dels.Length;i++)
                {
                    redPacketRecords.SafeRemove(msgData.dels[i]);                            
                }
            }
            if(msgData.updates != null)
            {
                for(int i = 0;i < msgData.updates.Length;i++)
                {
                    RedPacketRecord redPacketRecordUpadte = msgData.updates[i];
                    if(redPacketRecordUpadte == null)
                    {
                        continue;
                    }
                    RedPacketRecord redPacketRecord = redPacketRecords.SafeGetValue(redPacketRecordUpadte.guid);
                    if(redPacketRecord == null)
                    {
                        continue;
                    }
                    CopyRedPacketData(ref redPacketRecord, ref redPacketRecordUpadte);
                }
            }
            if(msgData.adds != null)
            {
                for(int i = 0;i < msgData.adds.Length;i++)
                {
                    RedPacketRecord redPacketRecordAdd = msgData.adds[i];
                    if(redPacketRecordAdd == null)
                    {
                        continue;
                    }
                    RedPacketRecord redPacketRecord = new RedPacketRecord();
                    if(redPacketRecord == null)
                    {
                        continue;
                    }
                    CopyRedPacketData(ref redPacketRecord, ref redPacketRecordAdd);
                    redPacketRecords.SafeAdd(redPacketRecord.guid, redPacketRecord);
                }
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUpdateGuildRedPacketRecord);
        }

        //[EnterGameMessageHandle(WorldSyncRedPacket.MsgID)]
        void _OnInitRedPack(MsgDATA a_data)
        {
            if (a_data != null)
            {
                _OnNetInitRedPack(a_data);
            }
            else
            {
                m_dictRedPackets.Clear();
                m_dictPacketType.Clear();
            }
        }

        public bool CheckNewYearActivityOpen()
        {
            ActiveManager.ActiveData activeData = ActiveManager.GetInstance().GetActiveData(NewYearRedPackRankListActivityID);

            if (activeData == null)
            {
                return false;
            }

            if (activeData.mainInfo.state != (byte)StateType.Running)
            {
                return false;
            }

            if (PlayerBaseData.GetInstance().Level < activeData.mainInfo.level)
            {
                return false;
            }

            return true;
        }
        public bool HasRedPacketToOpen(RedPacketType type)
        {
            var iter = m_dictRedPackets.GetEnumerator();
            while (iter.MoveNext())
            {
                RedPacketBaseEntry data = iter.Current.Value;
                if (data.status == (byte)RedPacketStatus.WAIT_RECEIVE &&
                    data.opened == 0    &&
                    data.type == (byte)type    
                    )
                {
                    return true;
                }
            }
            return false;
        }
    }
}
