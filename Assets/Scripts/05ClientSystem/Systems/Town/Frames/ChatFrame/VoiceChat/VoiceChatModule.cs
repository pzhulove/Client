using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using GameClient;
using VoiceSDK;

namespace GameClient
{
    public struct VoiceChatConfig
    {
        public SDKVoiceManager.VoiceSDKSwitch voiceSDKSwitchType;
        public VoiceChatModule.IsSatisfiedWithChatCondition isSatisfiedWithChat;
        public VoiceChatModule.IsRecordLimitCondition isRecordLimitCondition;
        public void Clear()
        {
            voiceSDKSwitchType = SDKVoiceManager.VoiceSDKSwitch.ChatVoice;
            isSatisfiedWithChat = null;
            isRecordLimitCondition = null;
        }
    }


    /// <summary>
    /// 即时语音（语音聊天） Control
    /// </summary>
    public class VoiceChatModule : ISDKVoiceCallback
    {
        bool isVoiceChatOpen = false;

        bool isInitialized = false;
        ChatType chatType = ChatType.CT_ALL;                              //缓存最新一次的聊天频道类型
        ulong targetUserRoleId = 0;

        private VoiceChatConfig voiceChatConfig;

        public delegate bool IsSatisfiedWithChatCondition();
        public delegate bool IsRecordLimitCondition();

        public void Reset(VoiceChatConfig vcConfig, bool otherVoiceChatSwitch)
        {
            this.voiceChatConfig = vcConfig;
            isVoiceChatOpen = SDKVoiceManager.instance.GetVoiceSDKSwitch(vcConfig.voiceSDKSwitchType) && otherVoiceChatSwitch;

            _Init();
        }

        private void _Init()
        {
            if (isInitialized)
                return;
            isInitialized = true;

            AddUIHandler();
        }

        public void UnInit()
        {
            isInitialized = false;

            RemoveUIHandler();

            chatType = ChatType.CT_ALL;
            targetUserRoleId = 0;

            voiceChatConfig.Clear();
        }

        void AddUIHandler()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildCreateSuccess, _OnGuildJoin);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildGlobalJoined, _OnGuildJoin);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildGlobalKickedOut, _OnGuildLeave);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildLeaveGuildSuccess, _OnGuildLeave);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamCreateSuccess, _OnTeamJoin);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamJoinSuccess, _OnTeamJoin);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamGlobalJoined, _OnTeamJoin);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamGlobalLeaved, _OnTeamLeave);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RoleChatDirtyChanged, _OnSelectRolePrivateChat);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnDelPrivate, _OnDeleteRolePrivateChat);

            SDKVoiceManager.GetInstance().AddVoiceEventListener(this);
        }
        void RemoveUIHandler()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildCreateSuccess, _OnGuildJoin);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildGlobalJoined, _OnGuildJoin);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildGlobalKickedOut, _OnGuildLeave);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildLeaveGuildSuccess, _OnGuildLeave);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamCreateSuccess, _OnTeamJoin);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamJoinSuccess, _OnTeamJoin);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamGlobalJoined, _OnTeamJoin);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamGlobalLeaved, _OnTeamLeave);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RoleChatDirtyChanged, _OnSelectRolePrivateChat);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnDelPrivate, _OnDeleteRolePrivateChat);

            SDKVoiceManager.GetInstance().RemoveVoiceEventListener(this);
        }

        private void _OnGuildJoin(UIEvent evt)
        {
            _JoinTypeVoiceChatRoom(ChatType.CT_GUILD);
        }

        private void _OnGuildLeave(UIEvent evt)
        {
            _LeaveTypeVoiceChatRoom(ChatType.CT_GUILD);
        }

        private void _OnTeamJoin(UIEvent evt)
        {
            _JoinTypeVoiceChatRoom(ChatType.CT_TEAM);
        }

        private void _OnTeamLeave(UIEvent evt)
        {
            _LeaveTypeVoiceChatRoom(ChatType.CT_TEAM);
        }

        private void _OnSelectRolePrivateChat(UIEvent evt)
        {
            if (evt == null)
                return;
            if (evt.Param1 == null)
                return;
            string rIdStr = evt.Param1.ToString();
            if (string.IsNullOrEmpty(rIdStr))
            {
                return;
            }
            ulong rId = 0;
            if (ulong.TryParse(rIdStr, out rId))
            {
                SDKVoiceManager.GetInstance().JoinPrivateChatRoom(rId);
            }
        }

        private void _OnDeleteRolePrivateChat(UIEvent evt)
        {
            if (evt == null)
                return;
            UIEventDelPrivate evtDel = evt as UIEventDelPrivate;
            if (evtDel != null)
            {
                SDKVoiceManager.GetInstance().LeavePrivateChatRoom(evtDel.m_uid);
            }
        }

        private void _JoinTypeVoiceChatRoom(ChatType roomType)
        {
            SDKVoiceManager.GetInstance().JoinChatRoom(roomType);
        }

        private void _LeaveTypeVoiceChatRoom(ChatType roomType)
        {
            SDKVoiceManager.GetInstance().LeaveChatRoom(roomType);
        }

        bool CheckRecordEnabled()
        {
            if (PluginManager.instance.GetAudioAuthorization() == false)
            {
                _SetAudioAuthFailed();
                Logger.LogProcessFormat("[当前设备未激活语音权限]");
                return false;
            }
            if (SDKVoiceManager.GetInstance().IsRecordVoiceEnabled == false)
            {
                _SetRecordNotEnabled();
                Logger.LogProcessFormat("[未在设置中开启录音]");
                return false;
            }
            if (voiceChatConfig.isRecordLimitCondition != null)
            {
                if(voiceChatConfig.isRecordLimitCondition() == false)
                {
                    Logger.LogProcessFormat("[语音输入受限]");
                    return false;
                }
            }
            if (SDKVoiceManager.GetInstance().IsInVoiceTalkChannel())
            {
                _SetNotifyInRealVoice();
                Logger.LogProcessFormat("[当前在实时语音中，暂时无法使用麦克风录音]");
                return false;
            }
            if (SDKVoiceManager.GetInstance().IsLoginVoice() == false)
            {
                _SetNotLoginChatVoice();
                Logger.LogProcessFormat("[当前语音聊天未登录！！！]");
                return false;
            }
            return true;
        }

        private void _SetAudioAuthFailed()
        {
            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("voice_sdk_audio_auth_request"));
        }

        private void _SetRecordNotEnabled()
        {
            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("voice_sdk_record_not_enabled"));
        }

        private void _SetPlayVoiceNotEnabled()
        {
            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("voice_sdk_play_not_enabled"));
        }

        private void _SetNotifyInRealVoice()
        {
            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("voice_sdk_be_in_real_voice"));
        }

        private void _SetNotLoginChatVoice()
        {
            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("voice_sdk_not_login"));
        }

        #region  NEW MODIFTY

        public void PlayVoice(string voiceKey)
        {
            if (SDKVoiceManager.GetInstance().IsPlayVoiceEnabled == false)
            {
                _SetPlayVoiceNotEnabled();
                return;
            }

            if (SDKVoiceManager.GetInstance().IsInVoiceTalkChannel())
            {
                _SetNotifyInRealVoice();
                return;
            }

            if (SDKVoiceManager.GetInstance().IsLoginVoice() == false)
            {
                _SetNotLoginChatVoice();
                return;
            }

            //手动播放时 关闭自动播放的语音 清空当前自动播放队列
            SDKVoiceManager.GetInstance().StopPlayVoice();
            //SDKVoiceManager.GetInstance().ControlGameMusicMute(true);
            //Modify

            SDKVoiceManager.GetInstance().PlayVoice(voiceKey, false);
        }

        public void TryAutoPlayVoiceMessage(ChatData chatdata)
        {
            if (chatdata == null)
                return;

            if (!IsChatVoiceAutoPlay(chatdata))
                return;

            //放在IsChatVoiceAutoPlay后面判断
            if (SDKVoiceManager.GetInstance().IsLoginVoice() == false)
                return;

            if (chatdata.objid == GameClient.PlayerBaseData.GetInstance().RoleID)
                return;

            if (!string.IsNullOrEmpty(chatdata.voiceKey))
            {
                SDKVoiceManager.GetInstance().PlayVoice(chatdata.voiceKey, true);
            }
        }

        bool IsChatVoiceAutoPlay(ChatData chatData)
        {
            if (SDKVoiceManager.GetInstance().GetVoiceSDKSwitch(SDKVoiceManager.VoiceSDKSwitch.ChatVoiceInGloabl) == false)
                return false;
            if (SDKVoiceManager.GetInstance().IsPlayVoiceEnabled == false)
                return false;
            if (SDKVoiceManager.GetInstance().IsInVoiceTalkChannel())
                return false;

            if (chatData != null)
            {
                if (chatData.objid == PlayerBaseData.GetInstance().RoleID)
                    return false;
                switch (chatData.eChatType)
                {
                    case ChatType.CT_WORLD:
                        return SDKVoiceManager.instance.IsAutoPlayInWorld;
                    case ChatType.CT_TEAM:
                        return SDKVoiceManager.instance.IsAutoPlayInTeam;
                    case ChatType.CT_GUILD:
                        return SDKVoiceManager.instance.IsAutoPlayInGuild;
                    case ChatType.CT_NORMAL:
                        return SDKVoiceManager.instance.IsAutoPlayInNearby;
                    case ChatType.CT_PRIVATE:
                        return SDKVoiceManager.instance.IsAutoPlayInPrivate;
                    default:
                        return false;
                }
            }

            return false;
        }


        public void StartRecordVoice(ChatType chatType, ulong targetUserId)
        {
            if(chatType == ChatType.CT_ALL)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VoiceChatRecordFailed);
                return;
            }

            this.chatType = chatType;
            this.targetUserRoleId = targetUserId;

            //先停止播放语音
            SDKVoiceManager.GetInstance().StopPlayVoice();
            SDKVoiceManager.GetInstance().ControlGameMusicMute(true);
            SDKVoiceManager.GetInstance().StartRecordVoice(chatType);
        }

        public void StopRecordVoice()
        {
            SDKVoiceManager.GetInstance().StopRecordVoice();
        }

        public void CancelRecordVoice()
        {
            SDKVoiceManager.GetInstance().CancelRecordVoice();
        }

        #region VoiceChat UI Event Deal
        void _OnRecordVoiceEnd()
        {
            SDKVoiceManager.GetInstance().ControlGameMusicMute(false);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VoiceChatRecordEnd);
        }

        void _OnRecordVoiceFailed()
        {
            SDKVoiceManager.GetInstance().ControlGameMusicMute(false);            
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VoiceChatRecordFailed);
        }

        void _OnSendVoiceStart(SDKVoiceChatRecordInfo recordInfo)
        {
            SDKVoiceManager.GetInstance().ControlGameMusicMute(false);

            if (null == recordInfo || recordInfo.duration <= 0)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VoiceChatSendFailed);
                return;
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VoiceChatSendStart, recordInfo);

            if (recordInfo.bSendToGame)
            {
                SendVoiceChatToGame(recordInfo);
            }
        }

        void _OnSendVoiceSucc()
        {
            Logger.LogProcessFormat("youmi send voice in voice chat module , success !!!");
        }

        void _OnSendVoiceFailed()
        {
            SDKVoiceManager.GetInstance().ControlGameMusicMute(false);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VoiceChatRecordFailed);
        }

        void _OnPlayChatVoiceEnd()
        {
            //SDKVoiceManager.GetInstance().ControlGameMusicMute(false);
        }

        void _OnMicAutoNoEnabled()
        {
            SDKVoiceManager.GetInstance().ControlGameMusicMute(false);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VoiceMicNotAuth);
        }

        void _OnRecordVolumeChanged(object volumeInfo)
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VoiceChatRecordVolumeChanged, volumeInfo);
        }

        #endregion VoiceChat UI Event Deal

        void SendVoiceChatToGame(SDKVoiceChatRecordInfo recordInfo)
        {
            if (null == recordInfo)
            {
                return;
            }
            if (recordInfo.transVoiceText.Length < ChatData.CD_MAX_WORDS)
            {
                if(voiceChatConfig.isSatisfiedWithChat != null)
                {
                    if(!voiceChatConfig.isSatisfiedWithChat())
                    {
                        return;
                    }
                }

                Logger.LogProcessFormat("[开始发送语音聊天到游戏]");

                ChatManager.GetInstance().SendVoiceChat(chatType,
                                                            recordInfo.voiceKey,
                                                            recordInfo.transVoiceText,
                                                            (byte)recordInfo.duration,
                                                            targetUserRoleId);
            }
        }

        #endregion

        public void OnVoiceEventCallback(object sender, SDKVoiceEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            //上报语音操作埋点
            if (e.eventType == SDKVoiceEventType.ChatSendEndReport ||
                e.eventType == SDKVoiceEventType.ChatDownloadRecordVoiceReport)
            {               
                SDKVoiceManager.GetInstance().ReportUsingVoice(e.eventType, e.param1.ToString());
            }

            switch (e.eventType)
            {
                case SDKVoiceEventType.ChatLogin:
                    SDKVoiceManager.GetInstance().TryJoinChatRooms();
                    ChatManager.GetInstance().AddSyncVoiceChatListener(TryAutoPlayVoiceMessage);
                    break;
                case SDKVoiceEventType.ChatLogout:
                    ChatManager.GetInstance().RemoveAllSyncVoiceChatListener();
                    break;
                case SDKVoiceEventType.ChatJoinRoom:
                    SDKVoiceManager.GetInstance().SetJoinedChatRoomId(e.param1.ToString());
                    break;
                case SDKVoiceEventType.ChatNotJoinRoom:
                    SDKVoiceManager.GetInstance().TryReJoinCurrentRoom();
                    break;
                case SDKVoiceEventType.ChatLeaveRoom:
                    SDKVoiceManager.GetInstance().SetLeavedChatRoomId(e.param1.ToString());
                    break;
                case SDKVoiceEventType.ChatRecordEnd:
                    _OnRecordVoiceEnd();
                    break;
                case SDKVoiceEventType.ChatRecordFailed:
                    _OnRecordVoiceFailed();
                    break;
                case SDKVoiceEventType.ChatRecordVolumeChanged:
                    if( e.param1 != null)
                    {
                        _OnRecordVolumeChanged(e.param1);
                    }
                    break;
                case SDKVoiceEventType.ChatSendStart:
                    SDKVoiceChatRecordInfo recordInfo = e.param1 as SDKVoiceChatRecordInfo;
                    _OnSendVoiceStart(recordInfo);
                    break;
                case SDKVoiceEventType.ChatSendEnd:
                    _OnSendVoiceSucc();
                    break;
                case SDKVoiceEventType.ChatSendFailed:
                    _OnSendVoiceFailed();
                    break;
                case SDKVoiceEventType.ChatPlayEnd:
                    _OnPlayChatVoiceEnd();
                    break;
            }
        }
    }
}
