using UnityEngine;
using System.Collections;
using Protocol;
using Network;
using System.Collections.Generic;
using System;

namespace GameClient
{
    class ChatRecordManager : DataManager<ChatRecordManager>
    {
        public delegate void OnLoadPrivateChatDataOK(ulong roleId);
        public OnLoadPrivateChatDataOK onLoadPrivateChatDataOK;
        public const int functionId = 3;

        public class RoleChatRecords
        {
            public long RoleId { get; set; }
            public List<TargetChatRecords> RoleChats { get { return targetRecords; } set { targetRecords = value; } }

            List<TargetChatRecords> targetRecords = new List<TargetChatRecords>();
            #region method
            public void Clear()
            {
                targetRecords.Clear();
            }

            public void AddChatRecords(ulong targetId,ChatData data,RoleInfo roleInfo)
            {
                var find = RoleChats.Find(x =>
                {
                    return (ulong)x.friendId == targetId;
                });

                if(find == null)
                {
                    find = new TargetChatRecords();
                    find.friendId = (long)targetId;
                    var privateChatPlayerData = RelationDataManager.GetInstance().GetPriChatList().Find(x => { return x.relationData.uid == targetId; });
                    if(privateChatPlayerData != null)
                    {
                        find.RelationDataRecords.ConvertFrom(privateChatPlayerData.relationData);
                    }

                    if(find.RelationDataRecords.uid > 0)
                    {
                        RoleChats.Add(find);
                    }
                    else
                    {
                        //Logger.LogErrorFormat("AddChatRecords uid == 0 is Error!");
                    }
                }

                if(find != null)
                {
                    PrivateChatRecords chatRecords = null;
                    if (find.TargetChats.Count < TargetChatRecords.ms_max_records)
                    {
                        chatRecords = PrivateChatRecords.ConvertFrom(data, roleInfo.roleId);
                        //chatRecords.Order = find.TargetChats().Count;
                    }
                    else
                    {
                        chatRecords = find.TargetChats[0];
                        find.TargetChats.RemoveAt(0);
                        chatRecords.Convert(data, roleInfo.roleId);
                        //chatRecords.Order = find.TargetChats()[find.TargetChats().Count - 1].Order + 1;
                    }
                    find.TargetChats.Add(chatRecords);
                }
            }

            public void TryUpdateRelation()
            {
                for(int i = 0; i < RoleChats.Count; ++i)
                {
                    var curRecord = RoleChats[i];
                    if(curRecord == null)
                    {
                        continue;
                    }

                    var rd = RelationDataManager.GetInstance().GetRelationByRoleID((ulong)curRecord.friendId);
                    if(rd != null)
                    {
                        curRecord.RelationDataRecords.ConvertFrom(rd);
                    }
                    else
                    {
                        curRecord.RelationDataRecords.type = (byte)RelationType.RELATION_STRANGER;
                    }
                }
            }
            #endregion
        }

        public class RelationDataRecords
        {
            public byte type { get; set; }
            public long uid { get; set; }
            public string name { get; set; }
            public int seasonlv { get; set; }
            public UInt16 level { get; set; }
            public byte occu { get; set; }
            public byte dayGiftNum { get; set; }
            public byte isOnline { get; set; }
            public UInt32 createTime { get; set; }
            public byte vipLv { get; set; }

            public RelationDataRecords ConvertFrom(RelationData rd)
            {
                if(rd != null)
                {
                    this.type = rd.type;
                    this.uid = (long)rd.uid;
                    this.name = rd.name;
                    this.seasonlv = (int)rd.seasonLv;
                    this.level = rd.level;
                    this.occu = rd.occu;
                    this.dayGiftNum = rd.dayGiftNum;
                    this.isOnline = rd.isOnline;
                    this.createTime = rd.createTime;
                    this.vipLv = rd.vipLv;
                }
                return this;
            }
            public RelationData ConvertTo(RelationData target)
            {
                if(target != null)
                {
                    target.type = this.type;
                    target.uid = (ulong)this.uid;
                    target.name = this.name;
                    target.seasonLv = (UInt32)this.seasonlv;
                    target.level = this.level;
                    target.occu = this.occu;
                    target.dayGiftNum = this.dayGiftNum;
                    target.isOnline = this.isOnline;
                    target.createTime = this.createTime;
                    target.vipLv = this.vipLv;
                }
                return target;
            }
        }

        public class TargetChatRecords
        {
            public static int ms_max_records = 50;
            public bool Dirty { get; set; }
            public long friendId
            {
                get
                {
                    return RelationDataRecords.uid;
                }
                set
                {
                    RelationDataRecords.uid = value;
                }
            }
            public RelationDataRecords RelationDataRecords { get { return relationRecords; } set { relationRecords = value; } }
            public List<PrivateChatRecords> TargetChats { get { return targetChats; } set { targetChats = value; } }

            RelationDataRecords relationRecords = new RelationDataRecords();
            List<PrivateChatRecords> targetChats = new List<PrivateChatRecords>();
        }

        public class PrivateChatRecords
        {
            public enum ChatMark
            {
                CM_SELF = 0x01,
                CM_LINK = 0x02,
                CM_GM = 0x04,
                CM_VOICE = 0x08,
                CM_VOICE_FIRST = 0x10,
                CM_RED_PACKET = 0x20,
                CM_ADD_FRIEND = 0x40,
            }
            public byte Mark { get; set; }
            public string word { get; set; }
            public string shortTimeString { get; set; }
            public string voiceKey { get; set; }

            public byte voiceDuration { get; set; }
            public byte timeStamp { get; set; }
            public bool isShowTimeStamp { get; set; }

            public UInt32 headFrame { get; set; }
            #region method
            public static PrivateChatRecords ConvertFrom(ChatData chatData,ulong RoleId)
            {
                return new PrivateChatRecords().Convert(chatData,RoleId);
            }

            public PrivateChatRecords Convert(ChatData chatData,ulong RoleId)
            {
                Mark = 0;
                if (RoleId == chatData.objid)
                {
                    Mark |= (byte)ChatMark.CM_SELF;
                }
                word = chatData.word;
                shortTimeString = chatData.shortTimeString;
                timeStamp = (byte)chatData.timeStamp;
                isShowTimeStamp = chatData.isShowTimeStamp;
                headFrame = chatData.headFrame;
                if (chatData.bLink == 1)
                {
                    Mark |= (byte)ChatMark.CM_LINK;
                }
                if (chatData.bGm)
                {
                    Mark |= (byte)ChatMark.CM_GM;
                }
                if(chatData.bVoice)
                {
                    Mark |= (byte)ChatMark.CM_VOICE;
                    voiceKey = chatData.voiceKey;
                    voiceDuration = chatData.voiceDuration;
                    if(chatData.bVoicePlayFirst)
                    {
                        Mark |= (byte)ChatMark.CM_VOICE_FIRST;
                    }
                }
                else
                {
                    voiceKey = string.Empty;
                    voiceDuration = (byte)0;
                }
                if(chatData.bRedPacket)
                {
                    Mark |= (byte)ChatMark.CM_RED_PACKET;
                }

                if (chatData.bAddFriend)
                {
                    Mark |= (byte)ChatMark.CM_ADD_FRIEND;
                }
                return this;
            }

            public static ChatData ConvertFrom(PrivateChatRecords chatData,RelationData rd,RoleInfo roleInfo)
            {
                ChatData data = new ChatData();
                data.channel = (byte)ChatManager.GetInstance()._TransChatType(ChatType.CT_PRIVATE);
                var current = roleInfo;
                data.objid = (chatData.Mark & (byte)ChatMark.CM_SELF) > 0 ? current.roleId : rd.uid;
                data.sex = 0;
                data.occu = (chatData.Mark & (byte)ChatMark.CM_SELF) > 0 ? current.occupation : rd.occu;
                data.level = (chatData.Mark & (byte)ChatMark.CM_SELF) > 0 ? current.level : rd.level;
                data.viplvl = (chatData.Mark & (byte)ChatMark.CM_SELF) > 0 ? (byte)(PlayerBaseData.GetInstance().VipLevel <= 0 ? 0 : PlayerBaseData.GetInstance().VipLevel) : rd.vipLv;
                data.objname = (chatData.Mark & (byte)ChatMark.CM_SELF) > 0 ? current.name : rd.name;
                data.word = chatData.word;
                data.guid = 0;
                data.shortTimeString = chatData.shortTimeString;
                data.targetID = (chatData.Mark & (byte)ChatMark.CM_SELF) > 0 ? rd.uid : current.roleId;
                data.bLink = (chatData.Mark & (byte)ChatMark.CM_LINK) > 0 ? (byte)1 : (byte)0;
                data.bGm = (chatData.Mark & (byte)ChatMark.CM_GM) > 0;
                data.eChatType = ChatType.CT_PRIVATE;
                data.bVoice = (chatData.Mark & (byte)ChatMark.CM_VOICE) > 0 ? true : false;
                data.bVoicePlayFirst = (chatData.Mark & (byte)ChatMark.CM_VOICE_FIRST) > 0 ? true : false;
                data.voiceKey = chatData.voiceKey;
                data.voiceDuration = chatData.voiceDuration;
                data.bRedPacket = (chatData.Mark & (byte)ChatMark.CM_RED_PACKET) > 0 ? true : false;
                data.timeStamp = chatData.timeStamp;
                data.isShowTimeStamp = chatData.isShowTimeStamp;
                data.bAddFriend = (chatData.Mark & (byte)ChatMark.CM_ADD_FRIEND) > 0 ? true : false;
                data.headFrame = chatData.headFrame;
                return data;
            }
            #endregion
        }

        class ChatRecordsConfig
        {
            public List<RoleChatRecords> ChatRecords { get { return chatRecords; } set { chatRecords = value; } }

            List<RoleChatRecords> chatRecords = new List<RoleChatRecords>();
            #region method
            public RoleChatRecords GetChatRecords(ulong RoleId)
            {
                var find = ChatRecords.Find(x => { return x.RoleId == (long)RoleId; });
                return find;
            }

            public void AddPrivateChatData(ChatData chatData,RoleInfo roleInfo)
            {
                if (chatData == null)
                {
                    return;
                }

                if(roleInfo == null)
                {
                    Logger.LogErrorFormat("role info error!");
                    return;
                }

                ulong targetId = roleInfo.roleId == chatData.objid ? chatData.targetID : chatData.objid;
                if(targetId == 0)
                {
                    Logger.LogErrorFormat(
                        "targetId == 0 is Error! roleInfo.roleId is {0}, chatData.objId is {1}, and chatData.targetId is {2}",
                        roleInfo.roleId, chatData.objid, chatData.targetID);
                    return;
                }

                var find = ChatRecords.Find(x => { return x.RoleId == (long)roleInfo.roleId; });
                if (find == null)
                {
                    find = new RoleChatRecords();
                    find.RoleId = (long)roleInfo.roleId;
                    ChatRecords.Add(find);
                }

                if (find != null)
                {
                    find.AddChatRecords(targetId, chatData, roleInfo);
                }
            }

            public static string GetSavePath(ulong RoleId)
            {
                if(RoleId > 0)
                {
                    return string.Format("ChatRecords_{0}.json", RoleId);
                }
                return string.Empty;
            }

            public void SaveData2Jason(ulong RoleId)
            {
                if(RoleId > 0 && SwitchFunctionUtility.IsOpen(functionId))
                {
                    var find = ChatRecords.Find(x => { return x.RoleId == (long)RoleId; });
                    if(find != null)
                    {
                        var path = GetSavePath(RoleId);
                        if (!string.IsNullOrEmpty(path))
                        {
                            Logger.LogProcessFormat("[ChatRecord] ChatRecordManager SaveData2Jason path = {0}", path);
                            var jsonText = LitJson.JsonMapper.ToJson(find);
                            if(!string.IsNullOrEmpty(jsonText))
                            {
                                FileArchiveAccessor.SaveFileInPersistentFileArchive(path, jsonText);
                            }
                        }
                        ChatRecords.Remove(find);
                    }
                }
            }

            public void ReadDataFromJson(ulong RoleId,OnLoadPrivateChatDataOK onLoadPrivateChatDataOK)
            {
                if(SwitchFunctionUtility.IsOpen(functionId))
                {
                    var path = GetSavePath(RoleId);
                    if (string.IsNullOrEmpty(path))
                    {
                        Logger.LogErrorFormat("read data from json path is empty!");
                        return;
                    }

                    string jsonText = null;
                    FileArchiveAccessor.LoadFileInPersistentFileArchive(path, out jsonText);
                    if (string.IsNullOrEmpty(jsonText))
                    {
                        //Logger.LogErrorFormat("json file is empty path = {0}", path);
                        return;
                    }

                    RoleChatRecords chatRecords = LitJson.JsonMapper.ToObject<RoleChatRecords>(jsonText);
                    if (null == chatRecords)
                    {
                        Logger.LogErrorFormat("json2object failed !");
                        return;
                    }

                    if (chatRecords.RoleId != (long)RoleId)
                    {
                        Logger.LogErrorFormat("json data may be error,roleId can not matched!");
                        return;
                    }

                    ChatRecords.RemoveAll(x => { return x.RoleId == (long)RoleId; });
                    ChatRecords.Add(chatRecords);

                    Logger.LogProcessFormat("[ChatRecord] ChatRecordManager ReadDataFromJson path = {0}", path);
                    if (onLoadPrivateChatDataOK != null)
                    {
                        onLoadPrivateChatDataOK.Invoke(RoleId);
                    }
                }
            }
            #endregion
        }

        ChatRecordsConfig m_kChatRecords = new ChatRecordsConfig();
        ChatRecordsConfig PrivateChatRecordsConfig { get { return m_kChatRecords; } set { m_kChatRecords = value; } }

        public string GetSavePath(ulong RoleId)
        {
            if(RoleId != 0)
            {
                return string.Format("ChatRecords_{0}.json", PlayerBaseData.GetInstance().RoleID);
            }
            return string.Empty;
        }

        public void AddPrivateChatData(ChatData chatData)
        {
            RoleInfo roleInfo = null;
            for(int i = 0; i < ClientApplication.playerinfo.roleinfo.Length; ++i)
            {
                if(PlayerBaseData.GetInstance().RoleID == ClientApplication.playerinfo.roleinfo[i].roleId)
                {
                    roleInfo = ClientApplication.playerinfo.roleinfo[i];
                    break;
                }
            }
            PrivateChatRecordsConfig.AddPrivateChatData(chatData, roleInfo);
        }

        public override void OnApplicationStart()
        {

        }

        public override void OnApplicationQuit()
        {
            PrivateChatRecordsConfig.SaveData2Jason(PlayerBaseData.GetInstance().RoleID);
        }

        public override void Initialize()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RoleIdChanged,_OnRoleIdChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RoleChatDirtyChanged, _OnRoleChatDirtyChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRelationChanged, _OnRelationChanged);
        }

        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.ChatRecordManager;
        }

        public override void Clear()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RoleIdChanged, _OnRoleIdChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RoleChatDirtyChanged, _OnRoleChatDirtyChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRelationChanged, _OnRelationChanged);
        }

        void _OnRoleIdChanged(ulong ulPreRoleId, ulong ulAftRoleId)
        {
            Logger.LogProcessFormat("[ChatRecord] ChatRecordManager EUIEventID.RoleIdChanged ulPreRoleId = {0} ulAftRoleId = {1}", ulPreRoleId,ulAftRoleId);

            if (ulPreRoleId != 0)
            {
                PrivateChatRecordsConfig.SaveData2Jason(ulPreRoleId);
            }

            if(ulAftRoleId != 0)
            {
                PrivateChatRecordsConfig.ReadDataFromJson(ulAftRoleId,onLoadPrivateChatDataOK);
            }
        }

        void _OnRoleIdChanged(UIEvent uiEvent)
        {
            _OnRoleIdChanged((ulong)uiEvent.Param1,(ulong)uiEvent.Param2);
        }

        void _OnRoleChatDirtyChanged(UIEvent uiEvent)
        {
            ulong uid = (ulong)uiEvent.Param1;
            bool bDirty = (bool)uiEvent.Param2;
            var chatRecords = PrivateChatRecordsConfig.GetChatRecords(PlayerBaseData.GetInstance().RoleID);
            if(chatRecords != null)
            {
                for(int j = 0; j < chatRecords.RoleChats.Count; ++j)
                {
                    var targetChatRecords = chatRecords.RoleChats[j];
                    if(targetChatRecords != null && (ulong)targetChatRecords.friendId == uid)
                    {
                        targetChatRecords.Dirty = bDirty;
                        break;
                    }
                }
            }
        }

        void _OnRelationChanged(UIEvent uiEvent)
        {
            RelationData rd = uiEvent.Param1 as RelationData;
            bool bDelete = (bool)uiEvent.Param2;
            var find = PrivateChatRecordsConfig.GetChatRecords(PlayerBaseData.GetInstance().RoleID);
            if(find == null)
            {
                return;
            }

            var target = find.RoleChats.Find(x => { return (ulong)x.friendId == rd.uid; });
            if(target == null)
            {
                return;
            }

            if (bDelete)
            {
                rd.type = (byte)RelationType.RELATION_STRANGER;
            }
            target.RelationDataRecords.ConvertFrom(rd);
        }

        public RoleChatRecords GetChatRecords(ulong RoleId)
        {
            return PrivateChatRecordsConfig.GetChatRecords(RoleId);
        }

        public void RemoveChatRecords(ulong RoleId,ulong friendId)
        {
            var find = PrivateChatRecordsConfig.GetChatRecords(RoleId);
            if(find != null)
            {
                find.RoleChats.RemoveAll(x => { return (ulong)x.friendId == friendId; });
            }
        }
    }
}