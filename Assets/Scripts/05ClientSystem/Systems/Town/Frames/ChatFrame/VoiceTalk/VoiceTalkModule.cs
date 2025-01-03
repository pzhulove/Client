using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoiceSDK;
using UnityEngine.UI;

namespace GameClient
{
    public struct VoiceTalkConfig
    {
        public string resPath;
        public SDKVoiceManager.VoiceSDKSwitch switchType;
        public bool isMicOnAtFirst;                                                     //初始时 是否打开麦
        public bool isPlayerOnAtFirst;                                                  //初始时 是否打开扬声
        public bool isGlobalSilenceAtFirst;                                             //初始时 是否开启全员禁言
    }

    public class VoiceTalkOtherSpeakInfo
    {
        public bool isSpeak;
        public string gameAccId;
    }

    /// <summary>
    /// 实时语音 Control
    /// </summary>
    public class VoiceTalkModule: ISDKVoiceCallback
    {
        private List<string> talkSceneIds;                          //实时语音所在场景的唯一ID  可以加入多个场景

        private bool isVoiceTalkOpen = false;
        private bool isInited = false;
        private bool bFirstJoinChannel = true;                     //是否是第一次加入房间

        private VoiceTalkConfig talkConfig;

        private int setMicCount, setPlayerCount = 0;
        private bool bFirstSelectChannel = true;                   //是否第一次选择频道
        private bool bJoinChannelMicChanged = false;               //加入房间后的麦状态改变

        void _OnJoinChannelSucc()
        {
            if(bFirstJoinChannel)
            {
                SDKVoiceManager.GetInstance().ResetRealTalkVoiceParams();
                                
                if (talkConfig.isMicOnAtFirst && IsSelfMicEnable())  //如果被别人禁言
                {
                    if(!IsMicOn())
                    {
                        bJoinChannelMicChanged = true;
                    }
                    SDKVoiceManager.GetInstance().OpenRealMic();
                }
                else
                {
                    if(IsMicOn())
                    {
                        bJoinChannelMicChanged = true;
                    }
                    SDKVoiceManager.GetInstance().CloseRealMic();
                }
                if (talkConfig.isPlayerOnAtFirst)
                {
                    SDKVoiceManager.GetInstance().OpenRealPlayer();
                }
                else
                {
                    SDKVoiceManager.GetInstance().CloseRealPlayer();
                }   
                if(talkConfig.isGlobalSilenceAtFirst)
                {
                    OpenGlobalSilence();   
                }
                bFirstJoinChannel = false;
            }
        }

        void _OnLeaveChannelSucc()
        {
            if(!SDKVoiceManager.GetInstance().HasJoinedTalkChannel())
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VoiceTalkLeaveAllChannel);
            }
        }

        void _OnOtherLeaveChannel(string channelId, string voicePlayerId)
        {
            _SendOtherSpeak(false, voicePlayerId);
        }

        void _OnVoiceSDKMicOn(bool isOn)
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VoiceTalkMicSwitch, new bool[] { isOn });

            if(bJoinChannelMicChanged)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VoiceTalkJoinChannelAndMicChanged);
                bJoinChannelMicChanged = false;
            }

            if (isOn)
            {
                SDKVoiceManager.GetInstance().CutGameVolumnInTalkVoice();
                if(setMicCount > 0)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("voice_talk_self_mic_on"));
                    setMicCount--;
                }
            }
            else
            {            
                if(setMicCount > 0)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("voice_talk_self_mic_off"));
                    setMicCount--;
                }
            }
        }

        void _OnVoiceSDKPlayerOn(bool isOn)
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VoiceTalkPlayerSwitch, new bool[] { isOn });
            if(isOn)
            {
                if(setPlayerCount > 0)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("voice_talk_self_player_on"));
                    setPlayerCount--;
                }
            }
            else
            {
                if(setPlayerCount > 0)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("voice_talk_self_player_off"));
                    setPlayerCount--;
                }
            }
        }

        void _OnVoiceLimitAllNotSpeak(bool isOn)
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VoiceTalkLimitAllNotSpeak, new bool[] { isOn });
        }

        void _OnVoiceMicClosedByOther(bool isOn)
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VoiceTalkMicClosedByOther, new bool[] { isOn });        
        }        

        void _OnVoiceGlobalSilenceChanged(bool isSilence)
        {
            if(!isSilence)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("voice_talk_global_mic_on"));
            }
            else
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("voice_talk_global_mic_off"));
            }
        }

        void _OnVoiceTalkChannelChanged(string channelId)
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VoiceTalkChannelChanged, channelId);  

            if(!string.IsNullOrEmpty(channelId))
            {
                if(talkSceneIds == null || talkSceneIds.Count <= 0)
                {
                    return;
                }
                if(bFirstSelectChannel)
                {
                    bFirstSelectChannel = false;
                    return;
                }
                string currTalkSceneId = GetCurrentTalkChanneld();
                if(currTalkSceneId == channelId)
                {
                    return;
                }
                if(talkSceneIds[0] == channelId)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("voice_talk_mic_teamcopy_mode"));
                }
                else
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("voice_talk_mic_team_mode"));
                }
            } 
        }

        void _OnOtherSpeakInChannel(bool isSpeak, string voicePlayerId)
        {
            _SendOtherSpeak(isSpeak, voicePlayerId);
        }

        void _OnOtherControlMic(bool isOn, string voicePlayerId)
        {
            if(!isOn)
            {
                _SendOtherSpeak(false, voicePlayerId);
            }
        }

        void _OnOtherChangeSpeakChannel(string channelId, string voicePlayerId)
        {
            //bool isJoinedOtherSpeakChannel = IsJoinedTalkChannel(channelId);
            //切频道时 可以认为不讲话   
            _SendOtherSpeak(false, voicePlayerId);
        }

        private void _SendOtherSpeak(bool isSpeak, string voicePlayerId)
        {
            VoiceTalkOtherSpeakInfo info = new VoiceTalkOtherSpeakInfo();
            info.isSpeak = isSpeak;
            info.gameAccId = SDKVoiceManager.GetInstance().GetGameAccIdByVoicePlayerId(voicePlayerId); 
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VoiceTalkOtherSpeakInChannel, info);
        }

        void _OnMicAuthNoPermission()
        {
            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("voice_sdk_audio_auth_request"));
        }

        //其他开关条件 otherVoiceTalkSwitch 默认为true
        public void Reset(VoiceTalkConfig vtConfig, bool otherVoiceTalkSwitch = true)
        {
            this.talkConfig = vtConfig;
            if (talkSceneIds == null)
            {
                talkSceneIds = new List<string>();
            }
            this.isVoiceTalkOpen = VoiceSDK.SDKVoiceManager.GetInstance().GetVoiceSDKSwitch(vtConfig.switchType) && otherVoiceTalkSwitch;

            _Init();
        }

        private void _Init()
        {
            if (isInited)
            {
                return;
            }
            isInited = true;
            bFirstJoinChannel = true;
            bFirstSelectChannel = true;
            bJoinChannelMicChanged = false;

            if(!isVoiceTalkOpen)
            {
                return;
            }
            SDKVoiceManager.GetInstance().AddVoiceEventListener(this);
        }

        public void UnInit()
        {
            isInited = false; 
            if (!isVoiceTalkOpen)
            {
                return;
            }

            SDKVoiceManager.GetInstance().CloseRealMic();
            SDKVoiceManager.GetInstance().CloseRealPlayer();
            SDKVoiceManager.GetInstance().RecoverGameVolumnInTalkVoice();
            SDKVoiceManager.GetInstance().LeaveAllTalkChannels();
            SDKVoiceManager.GetInstance().RemoveVoiceEventListener(this);

            if(talkSceneIds != null)
            {
                talkSceneIds.Clear();
                talkSceneIds = null;
            }
            setMicCount = setPlayerCount = 0;
        }  

        public List<string> GetMultipleTalkChannels()
        {
            return talkSceneIds;
        }

        public void UpdateMultipleTalkChannel(List<string> sIds)
        {
            if (sIds == null)
                return;
            talkSceneIds = sIds;
            _UpdateTalkChannels();
        }

        public void AddMultipleTalkChannel(string sId)
        {
            if (string.IsNullOrEmpty(sId))
                return;
            if (talkSceneIds != null && !talkSceneIds.Contains(sId))
            {
                talkSceneIds.Add(sId);
                _UpdateTalkChannels();
            }
        }

        public void RemoveMultipleTalkChannel(string sId)
        {
            if (string.IsNullOrEmpty(sId))
                return;
            if (talkSceneIds != null)
            {
                talkSceneIds.Remove(sId);
                _UpdateTalkChannels();
            }
        }

        private void _UpdateTalkChannels()
        {
            if (talkSceneIds != null)
            {
                SDKVoiceManager.GetInstance().UpdateTalkChannel(talkSceneIds);
            }
        }

        public bool IsMicOn()
        {
            return SDKVoiceManager.GetInstance().IsTalkRealMicOn();
        }

        public void OpenMic()
        {
            if (SDKVoiceManager.GetInstance().IsRecordVoiceEnabled == false)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("voice_sdk_record_not_enabled"));
                return;
            }   
            //被禁言
            if(!IsSelfMicEnable())
            {
                return;
            }         
            SDKVoiceManager.GetInstance().OpenRealMic();            
        }

        public bool CloseMic()
        {
            if (SDKVoiceManager.GetInstance().IsRecordVoiceEnabled == false)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("voice_sdk_record_not_enabled"));
                return false;
            }
            if (!SDKVoiceManager.GetInstance().IsTalkRealMicOn())
            {
                return false;
            }
            SDKVoiceManager.GetInstance().CloseRealMic();
            setMicCount++;
            return true;
        }

        public void ControlMic()
        {
            if (SDKVoiceManager.GetInstance().IsRecordVoiceEnabled == false)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("voice_sdk_record_not_enabled"));
                return;
            }
            SDKVoiceManager.GetInstance().ControlRealVoiceMic();
            setMicCount++;
        }

        public bool IsPlayerOn()
        {
            return SDKVoiceManager.GetInstance().IsTalkRealPlayerOn();
        }

        public void OpenPlayer()
        {
            if (SDKVoiceManager.GetInstance().IsPlayVoiceEnabled == false)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("voice_sdk_play_not_enabled"));
                return;
            }
            SDKVoiceManager.GetInstance().OpenRealPlayer();
        }

        public void ClosePlayer()
        {
            if (SDKVoiceManager.GetInstance().IsPlayVoiceEnabled == false)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("voice_sdk_play_not_enabled"));
                return;
            }
            SDKVoiceManager.GetInstance().CloseRealPlayer();
        }

        public void ControlPlayer()
        {
            if (SDKVoiceManager.GetInstance().IsPlayVoiceEnabled == false)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("voice_sdk_play_not_enabled"));
                return;
            }
            SDKVoiceManager.GetInstance().ControlRealVociePlayer();
            setPlayerCount++;
        }

        //切换说话频道，设置麦是否需要开启
        public bool SwitchSpeakChannel(string sceneId, bool needMicOn = true)
        {
            if (SDKVoiceManager.GetInstance().IsRecordVoiceEnabled == false)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("voice_sdk_record_not_enabled"));
                return false;
            }
            //被禁言
            if(!IsSelfMicEnable())
            {
                return false;
            }     
            string currTalkSceneId = GetCurrentTalkChanneld();
            bool bSameChannelId = currTalkSceneId == sceneId;
            if (needMicOn)
            {
                if (!SDKVoiceManager.GetInstance().IsTalkRealMicOn())
                {
                    SDKVoiceManager.GetInstance().OpenRealMic();
                    //特殊处理 - 如果是麦关闭下，并且当前说话频道id相同 则认为切换成功 刷新UI
                    if (bSameChannelId)
                    {
                        _OnVoiceTalkChannelChanged(sceneId);
                        return true;
                    }
                }
            }
            if (bSameChannelId)
            {
                return false;
            }
            SDKVoiceManager.GetInstance().SwitchTalkChannel(sceneId);
            return true;
        }

        public string GetCurrentTalkChanneld()
        {
            return  SDKVoiceManager.GetInstance().GetCurrentTalkChanneld();
        }
        
        //是否在其他玩家所在的语音频道内
        private bool _IsInOtherTalkChannel(string otherGameAccId)
        {
            string otherTalkChannelId = SDKVoiceManager.GetInstance().GetOtherTalkChannelId(otherGameAccId);
            if(string.IsNullOrEmpty(otherGameAccId))
            {
                return false;
            }
            return IsJoinedTalkChannel(otherTalkChannelId);
        }

        public bool IsJoinedTalkChannel(string talkChannelId)
        {
            if(string.IsNullOrEmpty(talkChannelId))
            {
                return false;
            }
            return SDKVoiceManager.GetInstance().IsJoinedTalkChannel(talkChannelId);
        }

        public void ControlGlobalSilence()
        {
            if(talkSceneIds == null || talkSceneIds.Count <= 0)
            {
                return;
            }
            //默认设置第一个频道为主频道
            SDKVoiceManager.GetInstance().ControlGlobalSilence(talkSceneIds[0]);
        }

        public void SetMicEnable(string gameAccId, bool bEnable)
        {
            SDKVoiceManager.GetInstance().SetMicEnable(gameAccId, bEnable);
        }

        //用是否被禁言的状态去控制其他玩家
        public void ResetGlobalSilence()
        {         
            if(talkSceneIds == null || talkSceneIds.Count <= 0)
            {
                return;
            }
            SDKVoiceManager.GetInstance().SetGlobalSilence(talkSceneIds[0], IsGlobalSilence());
        }

        //开启全局禁言
        public void OpenGlobalSilence()
        {
            if(talkSceneIds == null || talkSceneIds.Count <= 0)
            {
                return;
            }
            SDKVoiceManager.GetInstance().SetGlobalSilence(talkSceneIds[0], true);
        }

        //自己是否被禁言
        public bool IsSelfMicEnable()
        {
            return SDKVoiceManager.GetInstance().IsMicEnable();
        }

        //是否开启禁言
        public bool IsGlobalSilence()
        {
            return SDKVoiceManager.GetInstance().IsGlobalSilence();            
        }

        public void OnVoiceEventCallback(object sender, SDKVoiceEventArgs e)
        {
            if (null == e)
            {
                return;
            }

            //上报语音操作埋点
            if (e.eventType == SDKVoiceEventType.TalkJoinChannelSuccReport ||
                e.eventType == SDKVoiceEventType.TalkLeaveChannelSuccReport)
            {
                SDKVoiceManager.GetInstance().ReportUsingVoice(e.eventType, e.param1.ToString());
            }

            //TODO 修改为UIEvent 给 VoiceTalkBtn
            //TODO 修改为分开的状态
            switch (e.eventType)
            {
                case SDKVoiceEventType.TalkJoinChannelSucc:
                    _OnJoinChannelSucc();
                    break;
                case SDKVoiceEventType.TalkMicSwitch:
                    _OnVoiceSDKMicOn(e.status);
                    if (e.status)
                    {
                        SDKVoiceManager.GetInstance().CutGameVolumnInTalkVoice();
                    }
                    break;
                case SDKVoiceEventType.TalkPlayerSwitch:
                    _OnVoiceSDKPlayerOn(e.status);
                    break;
                case SDKVoiceEventType.TalkChangeSpeakChannelSucc:
                case SDKVoiceEventType.TalkChangeSpeakChannelFailed:
                    _OnVoiceTalkChannelChanged(e.param1 as string);
                    break;
                case SDKVoiceEventType.TalkCtrlGlobalSilence:
                    _OnVoiceLimitAllNotSpeak(e.status);
                    break;
                case SDKVoiceEventType.TalkMicChangeByOther:
                    _OnVoiceMicClosedByOther(e.status);
                    break;
                case SDKVoiceEventType.TalkGlobalSilenceChanged:
                    _OnVoiceGlobalSilenceChanged(e.status);
                    break;
                case SDKVoiceEventType.TalkChannelOtherSpeak:
                    _OnOtherSpeakInChannel(e.status, e.param1 as string);
                    break;
                case SDKVoiceEventType.TalkChannelOtherMicChanged:
                    _OnOtherControlMic(e.status, e.param1 as string);
                    break;
                case SDKVoiceEventType.TalkOtherChannelChanged:
                    _OnOtherChangeSpeakChannel(e.param1 as string, e.param2 as string);
                    break;
                case SDKVoiceEventType.TalkMicAuthNoPermission:
                    _OnMicAuthNoPermission();
                    break;
                case SDKVoiceEventType.TalkOtherLeaveChannel:
                    _OnOtherLeaveChannel(e.param1 as string, e.param2 as string);
                    break;
                case SDKVoiceEventType.TalkLeaveChannelSucc:
                    _OnLeaveChannelSucc();
                    break;
            }
        }
    }
}
