using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoiceSDK
{
    public interface ISDKVoiceInterface
    {
        /* chat voice */
        void InitChatVoice();//初始化聊天模式的语音
        void UnInitChatVoice();//反初始化聊天模式语音

        void LoginVoice(SDKVoiceGameAccInfo gameAccInfo);
        void LogoutVoice();
        bool IsLoginVoice();

        void JoinChatRoom(SDKVoiceChatRoomInfo roomInfo);
        void LeaveChatRoom(SDKVoiceChatRoomInfo roomInfo);
        void LeaveAllChatRooms();

        void StartRecordVoice(SDKVoiceChatRecordInfo recordInfo);
        void StopRecordVoice(string extra);
        void CancelRecordVoice();

        void PlayVoice(SDKVoiceChatPlayInfo voicePlayInfo);
        void StopPlayVoice();

        void SetVoiceVolume(float volume);
        float GetVoiceVolume();

        void OnChatPause();
        void OnChatResume();

        bool IsVoiceRecording();
        bool IsVoicePlaying();

        /* real talk */
        void InitTalkVoice();
        void UnInitTalkVoice();

        void JoinTalkChannel(string channelId);
        void LeaveAllTalkChannels();
        void LeaveTalkChannel(string channelId);

		void UpdateTalkChannel(IList<string> channelIds);
        string CurrentTalkChannelId();
        bool IsInVoiceTalkChannel();
        bool IsJoinedTalkChannel(string channelId);   
        bool HasJoinedTalkChannel();
        void SetCurrentTalkChannelId(string channelId);
        string GetOtherTalkChannelId(string voicePlayerId);

        //麦是否可用
        bool IsMicEnable();
        //设置麦可用
        void SetMicEnable(string voicePlayerId, bool bEnable);
        //是否全局禁言
        bool IsGlobalSilence();
        //在频道中开启全局禁言，关闭除自己以外其他玩家的麦
        void SetGlobalSilenceInMainChannel(string mainChannelId, bool isNotSpeak);

        void OpenRealMic();
        void CloseRealMic();
        void OpenRealPlayer();
        void CloseReaPlayer();

        bool IsTalkRealMicOn();
        bool IsTalkRealPlayerOn();

        void SetPlayerVolume(float volume);
        float GetPlayerVolume();

        void PauseTalkChannel();
        void ResumeTalkChannel();
    }
}