using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoiceSDK
{
    public class SDKVoiceEventArgs : EventArgs
    {
        public SDKVoiceEventType eventType;
        public bool status;
        public object param1;
        public object param2;

        public SDKVoiceEventArgs(SDKVoiceEventType eType)
        {
            Reset(eType);
        }

        public SDKVoiceEventArgs(SDKVoiceEventType eType, bool status)
        {
            Reset(eType, status);
        }

        public SDKVoiceEventArgs(SDKVoiceEventType eType, bool status, object p1)
        {
            Reset(eType, status, p1);
        }

        public SDKVoiceEventArgs(SDKVoiceEventType eType, bool status, object p1, object p2)
        {
            Reset(eType, status, p1, p2);
        }

        public SDKVoiceEventArgs(SDKVoiceEventType eType, object p1)
        {
            Reset(eType, p1);
        }

        public SDKVoiceEventArgs(SDKVoiceEventType eType, object p1, object p2)
        {
            Reset(eType, p1, p2);
        }

        private void _Clear()
        {
            this.eventType = SDKVoiceEventType.None;
            this.status = false;
            this.param1 = string.Empty;
            this.param2 = string.Empty;
        }

        public void Reset(SDKVoiceEventType eType)
        {
            _Clear();
            this.eventType = eType;
        }

        public void Reset(SDKVoiceEventType eType, bool status)
        {
            _Clear();
            this.eventType = eType;
            this.status = status;
        }

        public void Reset(SDKVoiceEventType eType, bool status, object p1)
        {
            _Clear();
            this.eventType = eType;
            this.status = status;
            this.param1 = p1;
        }

        public void Reset(SDKVoiceEventType eType, bool status, object p1, object p2)
        {
            _Clear();
            this.eventType = eType;
            this.status = status;
            this.param1 = p1;
            this.param2 = p2;
        }

        public void Reset(SDKVoiceEventType eType, object p1)
        {
            _Clear();
            this.eventType = eType;
            this.param1 = p1;
        }

        public void Reset(SDKVoiceEventType eType, object p1, object p2)
        {
            _Clear();
            this.eventType = eType;
            this.param1 = p1;
            this.param2 = p2;
        }
    }

    public enum SDKVoiceEventType
    {
        None,
        ChatLogin,
        ChatLogout,
        ChatNotJoinRoom,
        ChatRecordStart,
        ChatRecordEnd,
        ChatRecordFailed,
        ChatRecordVolumeChanged,        //录音音量              
        ChatSendStart,
        ChatSendEnd,
        ChatSendFailed,
        ChatSendEndReport,             //埋点
        ChatPlayStart,
        ChatPlayEnd,
        ChatJoinRoom,
        ChatLeaveRoom,
        ChatDownloadRecordVoiceReport, //埋点
        TalkInitSucc,
        TalkInitFailed,
        TalkJoinChannel,
        TalkJoinChannelSucc,
        TalkJoinChannelSuccReport,     //埋点
        TalkLeaveChannel,
        TalkLeaveChannelSucc,
        TalkLeaveChannelSuccReport, //埋点
        TalkPauseChannel,
        TalkResumeChannel,
        TalkMicSwitch,
        TalkMicOn,
        TalkMicOff,
        TalkPlayerSwitch,
        TalkPlayerOn,
        TalkPlayerOff,
        TalkMicAuthNoPermission,           //麦权限关闭
        TalkSpeakChannelChange,           //切换说话频道
        TalkChangeSpeakChannelSucc,
        TalkChangeSpeakChannelFailed,
        TalkMicChangeByOther,             //被他人禁言
        TalkChannelMemberChanged,         //频道内玩家状态改变
        TalkChannelOtherSpeak,            //频道内有人说话
        TalkChannelOtherMicChanged,       //频道内他人麦状态改变
        TalkOtherChannelChanged,          //他人频道改变
        TalkChannelBroadcastMsg,          //频道内广播消息
        TalkCtrlGlobalSilence,            //控制禁言
        TalkGlobalSilenceChanged,         //禁言状态改变
        TalkOtherLeaveChannel,
    }

    public interface ISDKVoiceCallbackEvent
    {
        event EventHandler<SDKVoiceEventArgs> VoiceEventHandler;
        void Register(ISDKVoiceCallback cb);
        void Detach(ISDKVoiceCallback cb);
        void DetachAll();
        void Invoke(SDKVoiceEventArgs e);
    }
}