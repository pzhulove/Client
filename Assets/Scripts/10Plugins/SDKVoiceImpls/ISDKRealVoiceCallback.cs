using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YouMe;

namespace VoiceSDK
{
    public interface ISDKRealVoiceCallback 
    {
        void OnTalkVoiceInit(bool isInited,YouMeErrorCode errorCode);
        void OnTalkVoiceJoinChannel(bool isSuccess, YouMeErrorCode errorCode);
        void OnTalkVoiceLeaveChannel(bool isSuccess, YouMeErrorCode errorCode);
        void OnTalkVoiceMicOn(bool isOn, YouMeErrorCode errorCode);
        void OnTalkVoicePlayerOn(bool isOn,YouMeErrorCode errorCode);
        void OnTalkVoiceChannelPause(bool isPaused,YouMeErrorCode errorCode);
    }
}
