using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoiceSDK
{
    public enum SDKVoiceLogLevel
    {
        None = 0,
        Error = 1,
        Warning = 2,
        Debug = 4,
    }

    public class SDKVoiceGameAccInfo
    {
        public string accId;
        public string openId;
        public string token;

        public SDKVoiceGameAccInfo(string accId, string openId, string token)
        {
            this.accId = accId == null ? "" : accId;
            this.openId = openId == null ? "" : openId;
            this.token = token == null ? "" : token;
        }
    }

    public class SDKVoiceChatRecordInfo
    {
        public string voiceKey;                     //语音key
        public string receiveId;                    //可以是聊天房间id或者私聊角色id
        public int duration;                        //语音时长
        public string saveAudioPath;                //语音文件本地存放路径
        public bool isTranslate;                    //是否翻译语音信息
        public string transVoiceText;               //翻译文本
        public bool bSendToGame;                    //是否发送语音到游戏
    }

    public class SDKVoiceChatPlayInfo
    {
        public string voiceKey;
        public string targetUserId;                 //语音播放来源角色id,聊天角色时请求私聊离线语音消息
        public bool isAutoPlay;
    }

    public class SDKVoiceChatRoomInfo
    {
        public string roomId;
        public bool beSaveRoomMsg;

        public SDKVoiceChatRoomInfo(string rId, bool saveMsg = true)
        {
            roomId = rId;
            beSaveRoomMsg = saveMsg;
        }
    }

    /// <summary>
    /// 语音接口
    /// 
    /// 尽量保证 对语音SDK和系统、文件的处理放到这里或者子类中处理
    /// </summary>
    public class SDKVoiceInterface : ISDKVoiceInterface, ISDKVoiceCallbackEvent
    {
        private static SDKVoiceInterface _instance;
        public static SDKVoiceInterface Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new YouMiVoiceInterface();
                }
                return _instance;
            }
        }

        //语音文件缓存路径
        public static readonly string saveLocalPath =

#if UNITY_EDITOR
                 Application.dataPath + "/recordVoiceCache/";
#elif UNITY_IOS && !UNITY_EDITOR
			     Application.persistentDataPath + "/recordVoiceCache/";
#elif UNITY_ANDROID && !UNITY_EDITOR
                 Application.persistentDataPath + "/recordVoiceCache/";
#else 
                 Application.persistentDataPath + "/recordVoiceCache/";
#endif

        protected Queue<string> voiceQueue = null;
        protected int maxVoiceQueueLength = 20;
        protected uint onTimeDeletePrivateChat = 10;
        protected int privateChatQueryMsgCount = 10;
        protected SDKVoiceLogLevel logLevel;

        public delegate ulong GameServerTimeHandler();
        private GameServerTimeHandler gameServerTime;
        public GameServerTimeHandler GameServerTime
        {
            get
            {
                if (gameServerTime == null)
                {
                    gameServerTime = delegate { return 0; };
                }
                return gameServerTime;
            }
            set
            {
                if (gameServerTime != null)
                {
                    gameServerTime = null;
                }
                gameServerTime = value;
            }
        }

        public delegate bool IsPrivateChatHandler(string voiceKey);
        private IsPrivateChatHandler isPrivateChat;
        public IsPrivateChatHandler IsPrivateChat
        {
            get
            {
                if (isPrivateChat == null)
                {
                    isPrivateChat = delegate { return false; };
                }
                return isPrivateChat;
            }
            set
            {
                if (isPrivateChat != null)
                {
                    isPrivateChat = null;
                }
                isPrivateChat = value;
            }
        }

        public virtual void Init() { }

        public virtual void UnInit(){ }

        protected virtual string GetFileNameExtension()
        {
            return ".wav";
        }

        /** 聊天 语音 **/
        public virtual void InitChatVoice() { }

        public virtual void UnInitChatVoice() { }

        public virtual void LoginVoice(SDKVoiceGameAccInfo gameAccInfo) { }

        public virtual void LogoutVoice() { }

        public virtual bool IsLoginVoice() { return true; }

        public virtual void JoinChatRoom(SDKVoiceChatRoomInfo roomInfo) { }

        public virtual void LeaveChatRoom(SDKVoiceChatRoomInfo roomInfo) { }

        public virtual void LeaveAllChatRooms() { }

        public virtual void StartRecordVoice(SDKVoiceChatRecordInfo recordInfo) { }
        public virtual void StopRecordVoice(string extra) { }
        public virtual void CancelRecordVoice() { }

        public virtual void PlayVoice(SDKVoiceChatPlayInfo voicePlayInfo) { }
        public virtual void StopPlayVoice() { }       

        public virtual void SetVoiceVolume(float volume) { }
        public virtual float GetVoiceVolume() { return 0f; }

        public virtual void OnChatPause() { }

        public virtual void OnChatResume() { }

        public virtual bool IsVoiceRecording() { return false; }

        public virtual bool IsVoicePlaying() { return false; }

        protected virtual void _AutoPlayVoice(string voiceKey) { }
        protected virtual void _StopAutoPlayVoice() { }
        protected virtual void _PlayVoiceCommon(string voiceKey, string targetUserId) { }

        /** 实时 语音 **/
        public virtual void InitTalkVoice() { }

        public virtual void UnInitTalkVoice() { }

        public virtual void JoinTalkChannel(string channelId) { }

        public virtual void LeaveAllTalkChannels() { }

        public virtual void LeaveTalkChannel(string channelId) { }

        public virtual bool IsInVoiceTalkChannel() { return false; }

        public virtual string CurrentTalkChannelId() { return string.Empty; }

        public virtual void UpdateTalkChannel(IList<string> channelIds) { }

        public virtual bool IsJoinedTalkChannel(string channelId) { return false; }

        public virtual bool HasJoinedTalkChannel(){return false;}

        public virtual void SetCurrentTalkChannelId(string channelId){}

        public virtual string GetOtherTalkChannelId(string voicePlayerId){ return string.Empty; }

        public virtual bool IsMicEnable() { return false; }
        public virtual bool IsGlobalSilence() { return false; }
        public virtual void SetGlobalSilenceInMainChannel(string mainChannelId, bool isNotSpeak) { }
        public virtual void SetMicEnable(string voicePlayerId, bool bEnable){}

        public virtual void OpenRealMic() { }

        public virtual void CloseRealMic() { }

        public virtual void OpenRealPlayer() { }

        public virtual void CloseReaPlayer() { }

        public virtual bool IsTalkRealMicOn(){ return false; }

        public virtual bool IsTalkRealPlayerOn() {  return false; }

        public virtual void SetPlayerVolume(float volume) { }

        public virtual float GetPlayerVolume()  { return 0f;  }

        public virtual void PauseTalkChannel() { }

        public virtual void ResumeTalkChannel() { }



        public virtual string ShowDebugLog()
        {
            return "";
        }

        public void SetLogLevel(SDKVoiceLogLevel logLevel)
        {
            this.logLevel = logLevel;
        }

        #region Events

        private List<EventHandler<SDKVoiceEventArgs>> voiceDelHandlers = new List<EventHandler<SDKVoiceEventArgs>>();
        private event EventHandler<SDKVoiceEventArgs> voiceEventHandler;
        public event EventHandler<SDKVoiceEventArgs> VoiceEventHandler
        {
            add
            {
                voiceEventHandler += value;
                voiceDelHandlers.Add(value);
            }
            remove
            {
                voiceEventHandler -= value;
                voiceDelHandlers.Remove(value);
            }
        }

        public void Register(ISDKVoiceCallback cb)
        {
            if (null == cb)
            {
                return;
            }
            VoiceEventHandler += cb.OnVoiceEventCallback;
        }

        public void Detach(ISDKVoiceCallback cb)
        {
            if (null == cb)
            {
                return;
            }
            VoiceEventHandler -= cb.OnVoiceEventCallback;
        }

        private void _RemoveAllEvents()
        {
            foreach (EventHandler<SDKVoiceEventArgs> eDel in voiceDelHandlers)
            {
                voiceEventHandler -= eDel;
            }
            voiceDelHandlers.Clear();
        }

        public void DetachAll()
        {
            _RemoveAllEvents();
            voiceEventHandler = null;
        }

        public void Invoke(SDKVoiceEventArgs e)
        {
            EventHandler<SDKVoiceEventArgs> handler = voiceEventHandler;
            if (handler != null && e != null)
            {
                handler(this, e);
            }
        }
        #endregion
    }
}