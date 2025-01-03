using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YIMEngine;        //chat voice 
using YouMe;            //talk voice 

/**
 游密 youme 语音中间件 
  
  API:
  https://www.youme.im/doc/index.php
 */

namespace VoiceSDK
{
    class YoumiVoiceGameAccInfo : SDKVoiceGameAccInfo
    {
        public string YoumiId;

        public YoumiVoiceGameAccInfo(string accId, string openId, string token)
                                     : base(accId, openId, token)
        {

        }
    }

    public class AudioInfo
    {
        public string token;                //自己构建的语音消息透传  角色id|服务器时间
        public ulong requestId;             //游密语音生成的消息Id
        public DownloadStatus status;       //语音下载状态
        public string path;                 //语音消息的存放路径

        public AudioInfo()
        {
        }
    }

    public enum DownloadStatus
    {
        DS_SUCCESS = 0,
        DS_NOTDOWNLOAD = 1,
        DS_FAILED = 2,
        DS_DOWNLOADING = 3,
    }

    public struct TalkChannelMemberInfo
    {
        public string userId;
        public string talkChannelId;
        public bool isJoined;
    }

    public class OtheTalkChannelInfo
    {
        public string userId;
        public string currentTalkChannelId;
    }


    public class YouMiVoiceInterface : SDKVoiceInterface, LoginListen,
                                                                MessageListen,
                                                                ChatRoomListen,
                                                                AudioPlayListen,
                                                                ContactListen,
                                                                LocationListen,
                                                                DownloadListen,
                                                                ISDKVoiceCallback
    {
        const string AppKey = "YOUMEF6EFE7326698A3E3E0AEEEDA02BCA6B6B39EF406";
        const string AppSecret = "1VhLqAbGn5Da4gxsIowTYGQYJ1hwLZPOi6YiSBJgzfqskjE5m9Oo0y5+P781O5U9SsK9IURbybKr/YB/4h/qQ3rKTlFmyWDRbNf+gOWPvEq2ilqHhj1K4LKC8Av0ORqSSJQAlDPATj6tzx1LprJL1HLePObGZ5lzDCyVrggJtmcBAAE=";

        //测试 参数
        //"r1+ih9rvMEDD3jUoU+nj8C7VljQr7Tuk4TtcByIdyAqjdl5lhlESU0D+SoRZ30sopoaOBg9EsiIMdc8R16WpJPNwLYx2WDT5hI/HsLl1NJjQfa9ZPuz7c/xVb8GHJlMf/wtmuog3bHCpuninqsm3DRWiZZugBTEj2ryrhK7oZncBAAE=";
        //"3hzCOsAPitudP7DiQN7ANrkbnTpEVdm0KJ1fFNmXzwL6BZTfEGwfBU4W2efhnAxkx11idqN60lIJ26KkRhBrBQcgui8SahalzxtVv+hKHvDhg/KTMhmal8tuknnAcxlWkq7102ZG3EM6loBPCMp6t96078W7XCWNoszlzPxT6w0BAAE=";

        //调试日志
        string cacheLogMsg = "";

        SDKVoiceEventArgs _args = null;
        SDKVoiceEventArgs args
        {
            get{
                if(_args == null)
                {
                    _args = new SDKVoiceEventArgs(SDKVoiceEventType.None);
                }
                return _args;
            }
        }

        //游密语音登录帐号信息
        YoumiVoiceGameAccInfo cacheChatVoiceAccInfo;

        /** 聊天语音模式 **/

        public enum LoginVoiceState
        {
            Logined,
            Logining,
            Logouted,
            Logouting,
        }
        LoginVoiceState loginVoiceState = LoginVoiceState.Logouted;

        bool isChatVoiceInited = false;
        //bool isChatVoiceLogined = false;
        //bool isChatVoiceLoiginFailed = false;

        bool isVoiceTranslate = false;
        float voiceVolume = 0f;
        bool isRecording = false;
        bool isRecordPTTFailed = false;
        ErrorCode chatMethodResult;
        bool isQueryHistoryMsgClear = false;

        List<string> voiceChatRoomIdList = new List<string>();

        //自己构建的语音消息透传参数（结构为：角色|当前服务器时间）
        string _leastLocalAudioToken = "";
        //当前请求播放的语音携带的透传参数
        string _currentReqPlayAudioToken = "";

        //语音消息本地缓存字典 消息透传 , 消息结构体
        Dictionary<string, AudioInfo> _AudioCache = new Dictionary<string, AudioInfo>();

        /** 实时语音模式 **/

        /*

        //实时语音初始化 和 加入频道 的接口调用状态 根据是否能多次调用决定！！！
        public enum JoinTalkChannelState 
        {
            LeavedAll,
            Joined,
            Joining,
            Leaving
        }
        JoinTalkChannelState joinChannelState = JoinTalkChannelState.LeavedAll;
        */

        public enum VoiceTalkPauseState
        {
            Paused,
            Pausing,
            Resumed,
            Resuming,
        }
        VoiceTalkPauseState voiceTalkPauseState = VoiceTalkPauseState.Resumed;

        bool isTalkVoiceInited = false;
        float playerVolume = 0f;
        //bool isTalkVoicePaused = false;

        //退出房间时需要重置参数
        bool isTalkMicEnable = true;            //麦是否可用，无论本地是否打开，不可用时说话不被其他人听到
        bool isTalkMicMaunalOn = false;         //麦是否主动打开
        bool isTalkMicEnableChangedDirty = false;
        bool isTalkMicAutoOn = false;           //麦是否自动打开
        bool isTalkMicForceOffDirty = false;    //麦是否强制关
        YouMeErrorCode talkMethodResult;
        ulong joinTalkRoomStartTime = 0;

        bool isFirstJoinTalkChannelDirty = true;
        List<string> talkChannelIds = null;
        string currTalkChannelId;             //当前实时语音说话频道ID

        List<OtheTalkChannelInfo> otherTalkChannelInfos = null;  //其他玩家当前说话频道

        bool isLastGlobalSilence;             //上一个全局禁言状态
        bool isGlobalSilence;                 //其他玩家是否被禁言
        bool isSetGlobalSilenceSucc = true;   //设置禁言状态是否成功
        bool isSetGlobalSilenceDirty = false;      //是否设置了禁言    
        bool hasReqVoiceAuth = false;

        public enum TalkBroadcastType
        {
            None,
            ChangeTalkChannel,
            LeaveTalkChannel,
            JoinTalkChannel,
            Count
        }
        private static readonly string[] TALK_BROADCAST_TYPES = new string[(int)TalkBroadcastType.Count]
        {
            "none", "changeTalkChannel", "leaveTalkChannel", "joinTalkChannel"
        };

        public override void Init()
        {
            //缓存的聊天模式语音的登录帐号信息
            //cacheChatVoiceAccInfo = new YoumiVoiceGameAccInfo();

            //实时语音回调接收实例化
            var voiceCB = VoiceSDK.SDKVoiceCallback.instance;

            Register(this);
        }

        public override void UnInit()
        {
            Detach(this);
            _args = null;
        }

        #region PUBLIC Method for Chat Voice
        public override void InitChatVoice()
        {
            if (isChatVoiceInited)
                return;

            isVoiceTranslate = false;

            chatMethodResult = IMAPI.Instance().Init(AppKey, AppSecret, ServerZone.China);
            if (chatMethodResult == ErrorCode.Success)
            {
                isChatVoiceInited = true;

                IMAPI.Instance().SetLoginListen(this);
                IMAPI.Instance().SetMessageListen(this);
                IMAPI.Instance().SetChatRoomListen(this);
                IMAPI.Instance().SetDownloadListen(this);
                IMAPI.Instance().SetContactListen(this);
                IMAPI.Instance().SetAudioPlayListen(this);
                IMAPI.Instance().SetLocationListen(this);

                //IMAPI.Instance().SetServerZone(ServerZone.China);
                if (!string.IsNullOrEmpty(saveLocalPath))
                {
                    IMAPI.Instance().SetAudioCachePath(saveLocalPath);
                    LogForYouMiChat("InitVoice params set:", chatMethodResult, "SaveVoiceCachePath: " + saveLocalPath);
                }
                //初始化缓存队列
                voiceQueue = new Queue<string>();

                if (System.Int32.TryParse(TR.Value("voice_sdk_voice_queue_count"), out maxVoiceQueueLength))
                {
                    LogForYouMiChat("InitVoice", ErrorCode.Success, "Set voice auto play queue captity is " + maxVoiceQueueLength);
                }

                if (System.UInt32.TryParse(TR.Value("voice_sdk_private_chat_deleteTime"), out onTimeDeletePrivateChat))
                {
                    LogForYouMiChat("InitVoice", ErrorCode.Success, "Set private chat delete on time , day : " + onTimeDeletePrivateChat);
                }

                if (System.Int32.TryParse(TR.Value("voice_sdk_private_chat_queryCount"), out privateChatQueryMsgCount))
                {
                    LogForYouMiChat("InitVoice", ErrorCode.Success, "Set private chat query chat msg count is " + privateChatQueryMsgCount);
                }
            }
            LogForYouMiChat("InitVoice", chatMethodResult);
        }

        public override void UnInitChatVoice()
        {
            base.UnInitChatVoice();

            //语音队列
            if (voiceQueue != null)
            {
                voiceQueue.Clear();
                voiceQueue = null;
            }
            cacheChatVoiceAccInfo = null;

            isChatVoiceInited = false;
            //isChatVoiceLogined = false;
            //isChatVoiceLoiginFailed = false;

            isVoiceTranslate = false;
            maxVoiceQueueLength = 20;
            cacheLogMsg = "";
            voiceVolume = 0f;
            isRecording = false;

            isRecordPTTFailed = false;

            _leastLocalAudioToken = "";
            _currentReqPlayAudioToken = "";
            _AudioCache.Clear();

            loginVoiceState = LoginVoiceState.Logouted;

            isQueryHistoryMsgClear = false;

            voiceChatRoomIdList.Clear();
        }

        public override void LoginVoice(SDKVoiceGameAccInfo gameAccInfo)
        {
            if (null == gameAccInfo)
            {
                return;
            }

            cacheChatVoiceAccInfo = new YoumiVoiceGameAccInfo(gameAccInfo.accId, gameAccInfo.openId, gameAccInfo.token);

            LogForYouMiChat("LoginVoice !!! Just Set !!! YoumiChatVoiceAccInfo : ", chatMethodResult, "roleId " + gameAccInfo.accId + "| pass " + gameAccInfo.openId + "| token " + gameAccInfo.token);
            //TryLoginYoumiChatVoice(cacheChatVoiceAccInfo);
        }

        public override void LogoutVoice()
        {
            TryLogoutYoumiChatVoice();
        }

        public override bool IsLoginVoice()
        {
            return CheckYoumiChatVoiceLoginState();
        }

        public override void PlayVoice(SDKVoiceChatPlayInfo voicePlayInfo)
        {
            if (null == voicePlayInfo)
            {
                return;
            }

            if (voicePlayInfo.isAutoPlay)
            {
                _AutoPlayVoice(voicePlayInfo.voiceKey);
            }
            else
            {
                _StopAutoPlayVoice();
                _PlayVoiceCommon(voicePlayInfo.voiceKey, voicePlayInfo.targetUserId);
            }
        }

        protected override void _PlayVoiceCommon(string voiceKey, string targetUserId = "")
        {
            if (string.IsNullOrEmpty(voiceKey))
            {
                return;
            }
            _currentReqPlayAudioToken = voiceKey;

            if (_AudioCache == null)
            {
                //Logger.LogError("[PlayVoiceCommon] - _AudioCache is null ");
                return;
            }

            if (_AudioCache.ContainsKey(_currentReqPlayAudioToken))
            {
                AudioInfo audio = _AudioCache[_currentReqPlayAudioToken];

                if (audio.status == DownloadStatus.DS_SUCCESS)
                {
                    TryPlayVoiceByPath(audio.path);
                }
                else if (audio.status == DownloadStatus.DS_NOTDOWNLOAD)
                {
                    DownloadAudioFile(audio);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(targetUserId))
                {
                    //Logger.LogError("[PlayVoiceCommon] - QueryPrivateHistoryMsgByUserId targetUserid is null !!!");
                    return;
                }
                QueryPrivateHistoryMsgByUserId(targetUserId, privateChatQueryMsgCount);
            }
        }

        protected override void _AutoPlayVoice(string voiceKey)
        {
            if (voiceQueue != null && !string.IsNullOrEmpty(voiceKey))
            {
                if (_AudioCache != null && _AudioCache.ContainsKey(voiceKey))
                {
                    return;
                }

                if (isRecording)
                {
                    return;
                }

                if (voiceQueue.Count > maxVoiceQueueLength)
                {
                    voiceQueue.Dequeue();
                }
                voiceQueue.Enqueue(voiceKey);
                PlayVoiceQueue();
            }
        }

        protected override void _StopAutoPlayVoice()
        {
            if (voiceQueue != null && voiceQueue.Count > 0)
            {
                voiceQueue.Clear();
                LogForYouMiChat("ClearVoicePathQueue", ErrorCode.Success, "Chat Voice auto play queue is clear now");
            }
        }

        //！！！ 注意 进副本时 关闭自动语音播放 ？？？
        public override void StopPlayVoice()
        {
            TryStopPlayVoice();

            //停止语音播放时，清空自动语音播放队列
            _StopAutoPlayVoice();
        }

        public override void SetVoiceVolume(float volume)
        {
            if (!CheckYoumiChatVoiceLoginState(false))
            {
                return;
            }
            try
            {
                IMAPI.Instance().SetVolume(volume);
                voiceVolume = volume;
                LogForYouMiChat("SetVoiceVolume", ErrorCode.Success, "set curr volumn is " + volume);
            }
            catch (System.Exception e)
            {
                Logger.LogErrorFormat("Chat Voice Set Volumn is Failed : {0}", e.ToString());
            }
        }

        public override float GetVoiceVolume()
        {
            return voiceVolume;
        }

        private void ClearLocalCache()
        {
            if (!isChatVoiceInited)
            {
                //InitChatVoice();
                return;
            }
            bool isClearOk = IMAPI.Instance().ClearAudioCachePath();
            if (isClearOk)
            {
                LogForYouMiChat("ClearLocalCache", ErrorCode.Success, "clear local voice cache succ");
            }
            else
            {
                LogForYouMiChat("ClearLocalCache", ErrorCode.Fail, "clear local voice cache failed");
            }
        }

        public override void OnChatPause()
        {
            if (!isChatVoiceInited)
            {
                //InitChatVoice();
                return;
            }
            IMAPI.Instance().OnPause(false);
            LogForYouMiChat("OnPause Chat Voice", ErrorCode.Success, "[YouMe - Voice Chat] OnPause !!!");
        }

        public override void OnChatResume()
        {
            if (!isChatVoiceInited)
            {
                //InitChatVoice();
                return;
            }
            IMAPI.Instance().OnResume();
            LogForYouMiChat("OnResume Chat Voice", ErrorCode.Success, "[YouMe - Voice Chat] OnResume !!!");
        }

        public override string ShowDebugLog()
        {
            return cacheLogMsg;
        }

        public override bool IsVoiceRecording()
        {
            if (!CheckYoumiChatVoiceLoginState(false))
            {
                return false;
            }
            return isRecording;
        }

        public override bool IsVoicePlaying()
        {
            if (!CheckYoumiChatVoiceLoginState(false))
            {
                return false;
            }
            return IMAPI.Instance().IsPlaying();
        }

        public override void JoinChatRoom(SDKVoiceChatRoomInfo roomInfo)
        {
            if (null == roomInfo)
            {
                return;
            }
            if (!CheckYoumiChatVoiceLoginState(false))
            {
                return;
            }
            if (string.IsNullOrEmpty(roomInfo.roomId))
            {
                LogForYouMiChat("LeaveChatRoom", ErrorCode.Fail, "LeaveChatRoom roomId is null");
                return;
            }
            TryJoinChatRoom(roomInfo.roomId, roomInfo.beSaveRoomMsg);
        }

        public override void LeaveChatRoom(SDKVoiceChatRoomInfo roomInfo)
        {
            if (null == roomInfo)
            {
                return;
            }
            if (!CheckYoumiChatVoiceLoginState(false))
            {
                return;
            }
            if (string.IsNullOrEmpty(roomInfo.roomId))
            {
                LogForYouMiChat("LeaveChatRoom", ErrorCode.Fail, "LeaveChatRoom roomId is null");
                return;
            }
            TryLeaveChatRoom(roomInfo.roomId);
        }

        public override void LeaveAllChatRooms()
        {
            if (!CheckYoumiChatVoiceLoginState(false))
            {
                return;
            }
            TryLeaveAllChatRooms();
            //清空一段时间前的聊天记录
            _ClearVoiceChatMsgCache();
        }

        public override void StartRecordVoice(SDKVoiceChatRecordInfo recordInfo)
        {
            if (null == recordInfo)
            {
                return;
            }
            if (!CheckYoumiChatVoiceLoginState())
            {
                return;
            }
            //TryGetMicroPhoneStatus();

            if (isRecording)
            {
                return;
            }

            if (isRecordPTTFailed)
            {
                StopRecordVoice("");
                LogForYouMiChat("SendVoiceMessage", ErrorCode.Success, "isRecordPTTFailed !!! stopaudioMessage");
                return;
            }

            if (string.IsNullOrEmpty(recordInfo.receiveId))
            {
                //TODO 如果录音时 目标房间或者角色ID为空 尝试加入 
                args.Reset(SDKVoiceEventType.ChatNotJoinRoom);
                Invoke(args);
                return;
            }

            //统一使用房间语音类型
            YIMEngine.ChatType yChatType = ChatType.RoomChat;

            //消息序列号，用于校验一条消息发送成功与否的标识
            ulong voiceRequestId = 0;
            if (!recordInfo.isTranslate)
            {
                chatMethodResult = IMAPI.Instance().SendOnlyAudioMessage(recordInfo.receiveId, yChatType, ref voiceRequestId);
            }
            else
            {
                chatMethodResult = IMAPI.Instance().SendAudioMessage(recordInfo.receiveId, yChatType, ref voiceRequestId);
            }

            if (chatMethodResult != ErrorCode.Success)
            {
                //不要调用 获取麦克风状态 的接口
                if (chatMethodResult == ErrorCode.PTT_Fail)
                {
                    isRecordPTTFailed = true;
                    isRecording = false;
                }
                else if (chatMethodResult == ErrorCode.NotLogin)
                {
                    loginVoiceState = LoginVoiceState.Logouted;
                    TryLoginYoumiChatVoice(cacheChatVoiceAccInfo);
                }
                args.Reset(SDKVoiceEventType.ChatRecordFailed);
                Invoke(args);
            }
            else
            {
                isRecording = true;
                isRecordPTTFailed = false;
            }
        }

        public override void StopRecordVoice(string extra)
        {
            if (!CheckYoumiChatVoiceLoginState(false))
            {
                return;
            }

            if (string.IsNullOrEmpty(extra))
            {
                args.Reset(SDKVoiceEventType.ChatSendFailed);
                Invoke(args);
                return;
            }

            _leastLocalAudioToken = extra;

            chatMethodResult = IMAPI.Instance().StopAudioMessage(_leastLocalAudioToken);

            LogForYouMiChat("StopAudioMessage", chatMethodResult, "Stop record with token is " + extra);

            isRecording = false;
            if (chatMethodResult != ErrorCode.Success)
            {
                args.Reset(SDKVoiceEventType.ChatRecordFailed);
                Invoke(args);
            }
        }

        public override void CancelRecordVoice()
        {
            if (!CheckYoumiChatVoiceLoginState(false))
            {
                return;
            }

            chatMethodResult = IMAPI.Instance().CancleAudioMessage();
            LogForYouMiChat("CancelRecordVoice", chatMethodResult);
            isRecording = false;
            if (chatMethodResult != ErrorCode.Success)
            {
                args.Reset(SDKVoiceEventType.ChatRecordFailed);
                Invoke(args);
            }
            else
            {
                args.Reset(SDKVoiceEventType.ChatRecordEnd);
                Invoke(args);
            }
        }

        private void _ClearVoiceChatMsgCache()
        {
            //手动调用登出前清空
            //DeleteHistoryChatMsgByTime();
            //DeleteRoomChatMsgByMsgId();

            DeleteRoomChatMsgOnTime(onTimeDeletePrivateChat);

            if (voiceChatRoomIdList != null && voiceChatRoomIdList.Count > 0)
            {
                string rid = voiceChatRoomIdList[0];
                QueryHistoryMsgToClear(rid, privateChatQueryMsgCount, 0);
            }
            else
            {
                TryLogoutYoumiChatVoice();
            }
        }

        #endregion

        #region PUBLIC Method for Talk Voice
        /** 实时语音 **/

        public override void InitTalkVoice()
        {
            if (isTalkVoiceInited)
            {
                return;
            }

            YouMeVoiceAPI.GetInstance().SetCallback(SDKVoiceCallback.instance.gameObject.name);

            talkChannelIds = new List<string>();
            otherTalkChannelInfos = new List<OtheTalkChannelInfo>();

            //绑定回调

            TryInitTalkVoice();
        }

        public override void UnInitTalkVoice()
        {
            base.UnInitTalkVoice();

            //解绑回调

            isTalkVoiceInited = false;
            //isTalkVoiceJoinSucc = false;

            //joinChannelState = JoinTalkChannelState.LeavedAll;

            playerVolume = 0f;
            joinTalkRoomStartTime = 0;
            //isTalkVoicePaused = false;
            voiceTalkPauseState = VoiceTalkPauseState.Resumed;

            _ResetVoiceTalk();

            talkMethodResult = YouMeVoiceAPI.GetInstance().UnInit();
        }

        private void _ResetVoiceTalk()
        {
            isTalkMicEnable = true;
            isTalkMicMaunalOn = false;
            isTalkMicEnableChangedDirty = false;
            isTalkMicAutoOn = false;
            isTalkMicForceOffDirty = false;

            //isFirstJoinTalkChannelDirty = true;
            if (talkChannelIds != null)
            {
                talkChannelIds.Clear();
            }
            currTalkChannelId = string.Empty;

            if (otherTalkChannelInfos != null)
            {
                otherTalkChannelInfos.Clear();
            }

            isLastGlobalSilence = false;
            isGlobalSilence = false;
            isSetGlobalSilenceSucc = true;
            //isSetGlobalSilenceDirty = false; 
            hasReqVoiceAuth = false;
        }

        public override void JoinTalkChannel(string channelId)
        {
            if (!isTalkVoiceInited)
            {
                //TryInitTalkVoice();
                return;
            }

            TryJoinYoumiTalkChannel(channelId);
        }

        public override void LeaveAllTalkChannels()
        {
            //if (isTalkVoiceJoinSucc)
            TryLeaveAllYoumiTalkChannel();
        }

        public override void LeaveTalkChannel(string channelId)
        {
            TryLeaveYoumiTalkChannel(channelId);
        }

        public override void OpenRealMic()
        {
            if (!CheckYouMiTalkVoiceJoinState())
            {
                return;
            }
            isTalkMicEnableChangedDirty = false;
            isTalkMicAutoOn = false;
            YouMeVoiceAPI.GetInstance().SetMicrophoneMute(false);
            LogForYouMiTalk("OpenRealMic", YouMeErrorCode.YOUME_SUCCESS);
        }

        public override void CloseRealMic()
        {
            if (!CheckYouMiTalkVoiceJoinState())
            {
                return;
            }
            isTalkMicEnableChangedDirty = false;
            YouMeVoiceAPI.GetInstance().SetMicrophoneMute(true);
            LogForYouMiTalk("CloseRealMic", YouMeErrorCode.YOUME_SUCCESS);
        }

        private void _CloseRealMicOnMicEnable()
        {
            if (!CheckYouMiTalkVoiceJoinState())
            {
                return;
            }
            YouMeVoiceAPI.GetInstance().SetMicrophoneMute(true);
            LogForYouMiTalk("CloseRealMic On Mic Enable", YouMeErrorCode.YOUME_SUCCESS);
        }

        public override void OpenRealPlayer()
        {
            if (!CheckYouMiTalkVoiceJoinState())
            {
                return;
            }
            YouMeVoiceAPI.GetInstance().SetSpeakerMute(false);
            LogForYouMiTalk("OpenRealPlayer", YouMeErrorCode.YOUME_SUCCESS);
        }

        public override void CloseReaPlayer()
        {
            if (!CheckYouMiTalkVoiceJoinState())
            {
                return;
            }
            YouMeVoiceAPI.GetInstance().SetSpeakerMute(true);
            LogForYouMiTalk("CloseReaPlayer", YouMeErrorCode.YOUME_SUCCESS);
        }

        public override bool IsTalkRealMicOn()
        {
            if (!CheckYouMiTalkVoiceJoinState())
            {
                return false;
            }
            return !YouMeVoiceAPI.GetInstance().GetMicrophoneMute();
        }

        public override bool IsTalkRealPlayerOn()
        {
            if (!CheckYouMiTalkVoiceJoinState())
            {
                return false;
            }
            return !YouMeVoiceAPI.GetInstance().GetSpeakerMute();
        }

        public override void SetPlayerVolume(float volume)
        {
            //if (!CheckYouMiTalkVoiceJoinState())
            //{
            //    return;
            //}
            try
            {
                if (volume <= 0)
                {
                    volume = 0;
                }
                else if (volume > 1f)
                {
                    volume = 1f;
                }
                YouMeVoiceAPI.GetInstance().SetVolume((uint)(volume * 100));
                playerVolume = volume;
                LogForYouMiTalk("SetPlayerVolume", YouMeErrorCode.YOUME_SUCCESS, "set volume is " + playerVolume);
            }
            catch (System.Exception e)
            {
                Logger.LogErrorFormat("Real Talk Voice Set Volumn is Failed : {0}", e.ToString());
            }
        }

        public override float GetPlayerVolume()
        {
            //if (!CheckYouMiTalkVoiceJoinState())
            //{
            //    return playerVolume;
            //}
            int v = YouMeVoiceAPI.GetInstance().GetVolume();
            return v / 100;
        }

        public override void PauseTalkChannel()
        {
            //if (!CheckYouMiTalkVoiceJoinState())
            //{
            //    return;
            //}

            //if (joinChannelState == JoinChannelState.Joined || joinChannelState == JoinChannelState.Joining)
            //if(isTalkVoicePaused == false)
            if (voiceTalkPauseState == VoiceTalkPauseState.Resumed)
            {
                talkMethodResult = YouMeVoiceAPI.GetInstance().PauseChannel();
                voiceTalkPauseState = VoiceTalkPauseState.Pausing;
                LogForYouMiTalk("PauseChannel", YouMeErrorCode.YOUME_SUCCESS);
            }
        }

        public override void ResumeTalkChannel()
        {
            //if (!CheckYouMiTalkVoiceJoinState())
            //{
            //    return;
            //}

            //if (isTalkVoicePaused)
            if (voiceTalkPauseState == VoiceTalkPauseState.Paused)
            {
                talkMethodResult = YouMeVoiceAPI.GetInstance().ResumeChannel();
                voiceTalkPauseState = VoiceTalkPauseState.Resuming;
                LogForYouMiTalk("ResumeChannel", YouMeErrorCode.YOUME_SUCCESS);
            }
        }

        //是否在实时语音频道
        public override bool IsInVoiceTalkChannel()
        {
            bool hasJoinedTalkChannel = CheckYouMiTalkVoiceJoinState();

            LogForYouMiTalk("BeInRealVoiceChannel", YouMeErrorCode.YOUME_SUCCESS, hasJoinedTalkChannel ? "has join chennal" : "has not join channel");
            return hasJoinedTalkChannel;
        }

        public override string CurrentTalkChannelId() { return currTalkChannelId; }

        public override bool IsJoinedTalkChannel(string channelId)
        {
            if (talkChannelIds != null)
            {
                return talkChannelIds.Contains(channelId);
            }
            return base.IsJoinedTalkChannel(channelId);
        }

        public override bool HasJoinedTalkChannel()
        {
            if(talkChannelIds != null)
            {
                return talkChannelIds.Count > 0;
            }
            return false;
        }
        public override void SetCurrentTalkChannelId(string channelId)
        {
            _TrySetSpeackChannelInMultiMode(channelId);
        }

        public override void UpdateTalkChannel(IList<string> channelIds)
        {
            if (channelIds == null)
            {
                return;
            }
            if (channelIds.Count == 0)
            {
                LeaveAllTalkChannels();
                return;
            }
            if (talkChannelIds == null)
            {
                return;
            }
            List<string> needJoinChannelIds = channelIds.Except(talkChannelIds);
            List<string> needLeaveChannelIds = talkChannelIds.Except(channelIds);

            //先退出
            if (needLeaveChannelIds != null)
            {
                for (int i = 0; i < needLeaveChannelIds.Count; i++)
                {
                    LeaveTalkChannel(needLeaveChannelIds[i]);
                }
            }
            if (needJoinChannelIds != null)
            {
                for (int i = 0; i < needJoinChannelIds.Count; i++)
                {
                    JoinTalkChannel(needJoinChannelIds[i]);
                }
            }
        }

        public override string GetOtherTalkChannelId(string voicePlayerId)
        {
            if (null == otherTalkChannelInfos || otherTalkChannelInfos.Count <= 0)
                return "";
            var info = otherTalkChannelInfos.Find(x => { return x.userId == voicePlayerId; });
            if (null == info)
                return "";
            return info.currentTalkChannelId;
        }

        //自己是否被禁言
        public override bool IsMicEnable()
        {
            return isTalkMicEnable;
        }

        //频道是否禁言
        public override bool IsGlobalSilence()
        {
            LogForYouMiTalk("IsGlobalSilence", talkMethodResult, "global silence is " + this.isGlobalSilence);
            return this.isGlobalSilence;
        }

        public override void SetGlobalSilenceInMainChannel(string mainChannelId, bool isNotSpeak)
        {
            //isSetGlobalSilenceDirty = true;

            _SetGlobalSilenceStatus(isNotSpeak);
            LogForYouMiTalk("SetGlobalSilenceInMainChannel", talkMethodResult, "1 set global silence " + this.isGlobalSilence);

            //TODO
            //先获取成员列表  GetChannelUserList() + 再根据回调获取 加入到频道的成员列表信息
            //如果是禁言状态 则持续监听频道进出角色 
            //如果是非禁言状态 则不再监听频道进出角色

            //修改为 无论是否是禁言状态 都监听 频道内角色变换
            if (string.IsNullOrEmpty(mainChannelId))
            {
                return;
            }
            if (talkChannelIds == null || talkChannelIds.Count <= 0)
            {
                return;
            }
            if (!talkChannelIds.Contains(mainChannelId))
            {
                return;
            }
            _GetChannelUserList(mainChannelId, true);
        }

        public override void SetMicEnable(string voicePlayerId, bool bEnable)
        {
            //_SetCurrentMicSilenceStatus(true);
            //mic 不一定能开成功 如果禁用时上次麦是关闭状态 则本次也是关闭状态
            //OpenRealMic();
            _SetOtherMicMute(voicePlayerId, !bEnable);
        }

        #endregion

        #region PRIVATE METHOD  -  Chat voice
        /// <summary>
        /// 播放语音
        /// </summary>
        /// <param name="voicePath"></param>
        bool TryPlayVoiceByPath(string voicePath)
        {
            if (!CheckYoumiChatVoiceLoginState())
            {
                return false;
            }

            //反复播放 不需要调用停止播放！
            chatMethodResult = IMAPI.Instance().StartPlayAudio(voicePath);
            LogForYouMiChat("PlayVoiceSelected", chatMethodResult, "curr play voice path is " + voicePath);
            if (chatMethodResult == ErrorCode.Success)
            {
                args.Reset(SDKVoiceEventType.ChatPlayStart);
                return true;
            }
            else if (chatMethodResult == ErrorCode.PTT_IsPlaying)
            {
                args.Reset(SDKVoiceEventType.ChatPlayStart);
                return true;
            }
            else if (chatMethodResult == ErrorCode.NotLogin)
            {
                TryLoginYoumiChatVoice(cacheChatVoiceAccInfo);
            }
            return false;
        }

        /// <summary>
        /// 暂停播放语音
        /// </summary>
        /// <returns></returns>
        bool TryStopPlayVoice()
        {
            if (!CheckYoumiChatVoiceLoginState(false))
            {
                return false;
            }
            chatMethodResult = IMAPI.Instance().StopPlayAudio();
            LogForYouMiChat("StopPlayVoice", chatMethodResult);
            if (chatMethodResult == ErrorCode.Success)
            {
                return true;
            }
            return false;
        }

        bool CheckYoumiChatVoiceLoginState(bool beTryLogin = true)
        {
            //if (isChatVoiceLogined)
            if (loginVoiceState == LoginVoiceState.Logined)
            {
                return true;
            }
            //else if (isChatVoiceLoiginFailed)
            else if (loginVoiceState == LoginVoiceState.Logouted)
            {
                if (beTryLogin)
                {
                    LogForYouMiChat("CheckYoumiChatVoiceLoginState", ErrorCode.Success, "beTryLogin current loginState is " + loginVoiceState.ToString());
                    TryLoginYoumiChatVoice(cacheChatVoiceAccInfo);
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        void TryLoginYoumiChatVoice(YoumiVoiceGameAccInfo accInfo)
        {
            if (accInfo == null)
            {
                LogForYouMiChat("TryLoginYoumiChatVoice", ErrorCode.NotLogin, "TryLoginYoumiChatVoice accinfo is null");
                return;
            }

            if (string.IsNullOrEmpty(accInfo.accId) || string.IsNullOrEmpty(accInfo.openId))
            {
                LogForYouMiChat("TryLoginYoumiChatVoice", ErrorCode.NotLogin, "TryLoginYoumiChatVoice accinfo RoleId is " + accInfo.accId + ", OpenId is " + accInfo.openId);
                return;
            }

            // if (accInfo.token == null)
            // {
            //     LogForYouMiChat("TryLoginYoumiChatVoice", ErrorCode.NotLogin, "TryLoginYoumiChatVoice accinfo token is null");
            //     return;
            // }

            LogForYouMiChat("TryLoginYoumiChatVoice isChatVoiceInited : ", ErrorCode.Success, isChatVoiceInited.ToString());

            if (!isChatVoiceInited)
            {
                //error : InitChatVoice(); 只能调用一次的
                return;
            }

            LogForYouMiChat("TryLoginYoumiChatVoice loginVoiceState : ", ErrorCode.Success, loginVoiceState.ToString());

            if (loginVoiceState != LoginVoiceState.Logouted)
            {
                return;
            }

            chatMethodResult = IMAPI.Instance().Login(accInfo.accId, accInfo.openId, accInfo.token);
            loginVoiceState = LoginVoiceState.Logining;

            LogForYouMiChat("TryLoginYoumiChatVoice", chatMethodResult, "roleId " + accInfo.accId + "| pass " + accInfo.openId + "| token " + accInfo.token);

            if (chatMethodResult == ErrorCode.Success)
            {
                LogForYouMiChat("TryLoginYoumiChatVoice", chatMethodResult, "is Logining !!!");
            }
            else if (chatMethodResult == ErrorCode.AlreadyLogin)
            {
                LogForYouMiChat("TryLoginYoumiChatVoice", chatMethodResult, "已经登录聊天语音了");
                //isChatVoiceLogined = true;
                //loginVoiceState = LoginVoiceState.Logined;
                TryLogoutYoumiChatVoice();
            }
            else if (chatMethodResult == ErrorCode.ParamInvalid)
            {
                LogForYouMiChat("TryLoginYoumiChatVoice", chatMethodResult, "ErrorCode.ParamInvalid !!!");
                loginVoiceState = LoginVoiceState.Logouted;
            }
            else
            {
                LogForYouMiChat("TryLoginYoumiChatVoice", chatMethodResult, "ErrorCode not success !!!");
                loginVoiceState = LoginVoiceState.Logouted;
            }
        }

        void TryLogoutYoumiChatVoice()
        {
            if (!isChatVoiceInited)
            {
                //error : InitChatVoice(); 只能调用一次的
                return;
            }

            LogForYouMiChat("TryLogoutYoumiChatVoice loginVoiceState : ", ErrorCode.Success, loginVoiceState.ToString());
            if (loginVoiceState == LoginVoiceState.Logouting)
            {
                return;
            }

            //每次登出语音前 尝试清空下聊天语音的录音缓存
            //if (beTryDestroyCache)
            //{
            //    ClearVoiceChatMsgCache();
            //}

            chatMethodResult = IMAPI.Instance().Logout();
            if (chatMethodResult != ErrorCode.Success)
            {
                loginVoiceState = LoginVoiceState.Logouted;
            }
            else
            {
                loginVoiceState = LoginVoiceState.Logouting;
            }
            LogForYouMiChat("TryLogoutYoumiChatVoice", chatMethodResult);
        }

        /// <summary>
        /// 语音聊天下载方法 
        /// 
        /// 不需要构建下载队列 内部已构建了的！！！
        /// </summary>
        /// <param name="audio"></param>
        void DownloadAudioFile(AudioInfo audio)
        {
            if (!CheckYoumiChatVoiceLoginState())
            {
                return;
            }

            if (null == audio)
            {
                LogForYouMiChat("DownloadAudioFile audio", ErrorCode.Fail, "AudioInfo audio  is null");
                return;
            }

            chatMethodResult = IMAPI.Instance().DownloadAudioFile(audio.requestId, audio.path);

            LogForYouMiChat("DownloadAudioFile audio", chatMethodResult, string.Format("AudioInfo audio requestId is {0}, path is {1}", audio.requestId, audio.path));

            if (chatMethodResult == ErrorCode.Success)
            {
                audio.status = DownloadStatus.DS_DOWNLOADING;
            }
            else if (chatMethodResult == ErrorCode.NotLogin)
            {
                TryLoginYoumiChatVoice(cacheChatVoiceAccInfo);
            }
        }


        /// <summary>
        /// deprecated !!!
        /// 
        /// 注意!!! 不能在录音时调用  录音接口内部会调  重复调用会导致  获取状态出错导致录音失败 ！！！
        /// </summary>
        AudioDeviceStatus TryGetMicroPhoneStatus()
        {
            int status = (int)IMAPI.Instance().GetMicrophoneStatus();
            LogForYouMiChat("TryGetMicroPhoneStatus", ErrorCode.Success, "尝试获取麦克风状态，是否有权限 是否有麦 : " + (AudioDeviceStatus)status);
            return (AudioDeviceStatus)status;
        }

        void TryJoinChatRoom(string chatRoomId, bool beSaveRoomMsg)
        {
            //LogForYouMiChat("JoinChatRoom", ErrorCode.Success, "JoinChatRoom joinChatRoomState is " + joinChatRoomState.ToString());

            //if (joinChatRoomState != JoinChatRoomState.Leaved)
            //{
            //    return;
            //}

            //新增保存房间历史消息
            if (string.IsNullOrEmpty(chatRoomId))
            {
                //Logger.LogError("TryJoinChatRoom - roomid is null");
                return;
            }

            //是否需要存储本地消息
            if (beSaveRoomMsg)
            {
                SetSaveHistoryChatRoomMsg(chatRoomId, beSaveRoomMsg);
                LogForYouMiChat("SetSaveHistoryChatRoomMsg", ErrorCode.Success, "chatRoomId is " + chatRoomId);
            }

            chatMethodResult = IMAPI.Instance().JoinChatRoom(chatRoomId);
            //joinChatRoomState = JoinChatRoomState.Joining;

            LogForYouMiChat("JoinChatRoom", chatMethodResult, "JoinChatRoom id is " + chatRoomId);
        }

        void TryLeaveChatRoom(string chatRoomId)
        {
            //LogForYouMiChat("LeaveChatRoom", ErrorCode.Success, "LeaveChatRoom joinChatRoomState is " + joinChatRoomState.ToString());
            //if (joinChatRoomState == JoinChatRoomState.Leaving)
            //{
            //    return;
            //}
            chatMethodResult = IMAPI.Instance().LeaveChatRoom(chatRoomId);
            //joinChatRoomState = JoinChatRoomState.Leaving;
            LogForYouMiChat("LeaveChatRoom", chatMethodResult, "LeaveChatRoom id is " + chatRoomId);
        }

        void TryLeaveAllChatRooms()
        {
            chatMethodResult = IMAPI.Instance().LeaveAllChatRooms();
            LogForYouMiChat("LeaveAllChatRooms", chatMethodResult);
        }

        private string _BuildRequestIdPath(ulong requestId)
        {
            return System.IO.Path.Combine(saveLocalPath, requestId.ToString() + this.GetFileNameExtension());
        }

        protected override string GetFileNameExtension()
        {
            return ".wav";
        }

        bool PlayVoiceQueue()
        {
            //if (!CheckYoumiChatVoiceLoginState())
            //{
            //    return false;
            //}
            //如果当前正在播放语音，则再次调用此方法时不会重复进行播放
            if (IsVoicePlaying())
                return false;
            if (this.voiceQueue != null)
            {
                if (this.voiceQueue.Count <= 0)
                {
                    return false;
                }
                string voiceKey = this.voiceQueue.Dequeue();
                if (!string.IsNullOrEmpty(voiceKey))
                {
                    // return TryPlayVoiceByPath(path);
                    _PlayVoiceCommon(voiceKey);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 新增保存房间历史消息
        /// </summary>
        /// <param name="chatRoomId"></param>
        /// <param name="bSave"></param>
        void SetSaveHistoryChatRoomMsg(string chatRoomId, bool bSave)
        {
            if (!isChatVoiceInited)
            {
                return;
            }
            List<string> chatRoomIds = new List<string>() { chatRoomId };
            IMAPI.Instance().SetRoomHistoryMessageSwitch(chatRoomIds, bSave);
        }

        //void DeleteHistoryChatMsgByTime()
        //{
        //    if (!CheckYoumiChatVoiceLoginState(false))
        //    {
        //        return;
        //    }
        //    ulong currTimeStamp = SDKVoiceManager.GetInstance()._GetServerTimeStamp();
        //    chatMethodResult = IMAPI.Instance().DeleteHistoryMessage(YIMEngine.ChatType.RoomChat, currTimeStamp);
        //    LogForYouMiChat("DeleteHistoryChatMsgByTime", chatMethodResult, "Delete history chat msg by curr time, type Room Chat");
        //}

        /// <summary>
        /// 删除语音 一段时间前的语音消息 （包括了语音音频文件）
        /// </summary>
        /// <param name="day"></param>
        void DeleteRoomChatMsgOnTime(uint day)
        {
            if (!CheckYoumiChatVoiceLoginState(false))
            {
                return;
            }
            uint secondCount = day * 24 * 60 * 60;
            ulong currTimeStamp = GameServerTime();
            ulong timeStamp = currTimeStamp - secondCount;
            //chatMethodResult = IMAPI.Instance().DeleteHistoryMessage(YIMEngine.ChatType.PrivateChat, timeStamp);
            chatMethodResult = IMAPI.Instance().DeleteHistoryMessage(YIMEngine.ChatType.RoomChat, timeStamp);
            LogForYouMiChat("DeletePrivateChatMsgOnTime", chatMethodResult, "Delete history chat msg by curr time, type Private Chat");
        }

        void DeleteRoomChatMsgByMsgId(ulong msgId)
        {
            //if (_AudioCache != null && _AudioCache.Values != null)
            //{
            //    foreach (var aInfo in _AudioCache.Values)
            //    {
            //        if (aInfo.bPrivate == false)
            //        {
            //            chatMethodResult = IMAPI.Instance().DeleteHistoryMessageByID(aInfo.requestId);
            //            LogForYouMiChat("DeleteRoomChatMsgByMsgId", chatMethodResult, "Delete history chat msg by msg id : " + aInfo.requestId);
            //        }
            //    }
            //}

            chatMethodResult = IMAPI.Instance().DeleteHistoryMessageByID(msgId);
            LogForYouMiChat("DeleteRoomChatMsgByMsgId", chatMethodResult, "Delete history chat msg by msg id : " + msgId);
        }

        /// <summary>
        ///  倒序查询!!!，每次查询一定数量消息，判断是否存在对应token的语音
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="onceQueryMsgCount"></param>
        /// 
        /// targetID：私聊用户的id
        /// chatType：表示查询私聊或者频道聊天的历史记录
        /// startMessageID：起始历史记录消息id（与requestid不同），为0表示首次查询，将倒序获取count条记录
        /// count：最多获取多少条
        /// direction：历史记录查询方向，startMessageID>0时，0表示查询比startMessageID小的消息，1表示查询比startMessageID大的消息
        /// 
        void QueryPrivateHistoryMsgByUserId(string targetId, int onceQueryMsgCount, ulong startId = 0)
        {
            if (!isChatVoiceInited)
            {
                return;
            }
            if (onceQueryMsgCount < 0)
            {
                //Logger.LogError("[QueryPrivateHistoryMsgByUserId] - onceQueryMsgCount is less than zero !!!");
                return;
            }
            chatMethodResult = IMAPI.Instance().QueryHistoryMessage(targetId, YIMEngine.ChatType.RoomChat, startId, onceQueryMsgCount, 0);//最后一位为0为倒序查
            if (chatMethodResult == ErrorCode.Success)
            {
                isQueryHistoryMsgClear = false;
            }
            LogForYouMiChat("QueryPrivateHistoryMsgByUserId", chatMethodResult, string.Format("query History private msg , tarId {0} , once query count {1}", targetId, onceQueryMsgCount));
        }

        void QueryHistoryMsgToClear(string chatRoomId, int onceQueryMsgCount, ulong startId = 0)
        {
            if (!isChatVoiceInited)
            {
                return;
            }
            chatMethodResult = IMAPI.Instance().QueryHistoryMessage(chatRoomId, YIMEngine.ChatType.RoomChat, startId, onceQueryMsgCount, 0);
            if (chatMethodResult == ErrorCode.Success)
            {
                isQueryHistoryMsgClear = true;
            }
            LogForYouMiChat("QueryHistoryMsgToClear !", chatMethodResult, string.Format("query History private msg , tarId {0} , once query count {1}", chatRoomId, privateChatQueryMsgCount));
        }

        #endregion

        #region PRIVATE METHOD  -  Talk Voice

        void TryInitTalkVoice()
        {
            talkMethodResult = YouMeVoiceAPI.GetInstance().Init(AppKey, AppSecret, YOUME_RTC_SERVER_REGION.RTC_CN_SERVER, "cn");
            LogForYouMiTalk("TryInitTalkVoice", talkMethodResult);

            if (talkMethodResult == YouMeErrorCode.YOUME_ERROR_WRONG_STATE)
            {
                isTalkVoiceInited = true;
            }
        }

        void TryJoinYoumiTalkChannel(string channelId)
        {
            if (string.IsNullOrEmpty(channelId))
            {
                LogForYouMiTalk("TryJoinYoumiTalkChannel", YouMeErrorCode.YOUME_ERROR_INVALID_PARAM, "TryLoginYoumiChatVoice accinfo is null");
                return;
            }
            if (cacheChatVoiceAccInfo == null || cacheChatVoiceAccInfo.accId == "")
            {
                LogForYouMiTalk("TryJoinYoumiTalkChannel", YouMeErrorCode.YOUME_ERROR_INVALID_PARAM, 
                                "TryLoginYoumiChatVoice accinfo roleid is " + cacheChatVoiceAccInfo != null ? cacheChatVoiceAccInfo.accId : "");
                return;
            }
            if (!isTalkVoiceInited)
            {
                //TryInitTalkVoice();
                return;
            }
            //如果不是多渠道模式 则需要判断当前加入状态是否时完全退出 
            /*
            if (!isMultipleChannel && joinChannelState != JoinTalkChannelState.LeavedAll)
            {
                LogForYouMiTalk("TryJoinYoumiTalkChannel", YouMeErrorCode.YOUME_ERROR_INVALID_PARAM, "TryLoginYoumiChatVoice but state is not leaved");
                //TryLeaveAllYoumiTalkChannel();
                return;
            }*/

            talkMethodResult = YouMeVoiceAPI.GetInstance().JoinChannelMultiMode(cacheChatVoiceAccInfo.accId, channelId, YouMeUserRole.YOUME_USER_TALKER_FREE, false);
            //talkMethodResult = YouMeVoiceAPI.GetInstance().JoinChannelMultiMode(accInfo.RoleId, accInfo.TalkChannelId, false);
            /*joinChannelState = JoinTalkChannelState.Joining;*/
            LogForYouMiTalk("TryJoinYoumiTalkChannel", talkMethodResult, "res :" + "RoleId : " + cacheChatVoiceAccInfo.accId + "TalkChannelId : " + channelId);
            // + "!!! join state = "+joinChannelState.ToString());
        }

        /// <summary>
        /// 离开聊天房间接口可实时调用
        /// </summary>
        void TryLeaveAllYoumiTalkChannel()
        {
            //if (joinChannelState != JoinTalkChannelState.Leaving)
            {
                talkMethodResult = YouMeVoiceAPI.GetInstance().LeaveChannelAll();
                //joinChannelState = JoinTalkChannelState.Leaving;
                LogForYouMiTalk("LeaveChannel", talkMethodResult, "离开全部频道");//: state = " + joinChannelState.ToString());
            }
            //else
            //{
            //    LogForYouMiTalk("LeaveChannel", talkMethodResult, "正在离开频道 ：state = " + joinChannelState.ToString());
            //}
        }

        void TryLeaveYoumiTalkChannel(string channelId)
        {
            //if (joinChannelState != JoinTalkChannelState.Leaving)
            {
                talkMethodResult = YouMeVoiceAPI.GetInstance().LeaveChannelMultiMode(channelId);
                //joinChannelState = JoinTalkChannelState.Leaving;
                LogForYouMiTalk("LeaveChannel", talkMethodResult, "离开指定频道: channel id = " + channelId);// +", state = " + joinChannelState.ToString());
            }
            //else
            //{
            //    LogForYouMiTalk("LeaveChannel", talkMethodResult, "正在离开频道 ：state = " + joinChannelState.ToString());
            //}
        }

        void _TrySetSpeackChannelInMultiMode(string channelId)
        {
            if (string.IsNullOrEmpty(channelId))
            {
                return;
            }
            talkMethodResult = YouMeVoiceAPI.GetInstance().SpeakToChannel(channelId);
            LogForYouMiTalk("Speak To Channel", talkMethodResult, "指定讲话频道 ：channel id = " + channelId);
        }

        // 是否处于完全退出状态 ！！！
        bool CheckYouMiTalkVoiceJoinState()
        {
            bool hasJoinedTalkChannel = false;
            if (talkChannelIds != null)
            {
                hasJoinedTalkChannel = talkChannelIds.Count > 0;
            }
            return hasJoinedTalkChannel;
        }

        /// <summary>
        /// 注意 需要在加入频道前调用 !!!!!!!!!!!!
        /// </summary>
        /// <param name="enabled"></param>
        void SetMobileNetworkEnabled(bool enabled)
        {
            //if (!CheckYouMiTalkVoiceJoinState())
            //{
            //    return;
            //}
            if (GetMobileNetworkEnabled() == enabled)
            {
                return;
            }
            YouMeVoiceAPI.GetInstance().SetUseMobileNetworkEnabled(enabled);

            LogForYouMiTalk("SetMobileNetworkEnabled", YouMeErrorCode.YOUME_SUCCESS, "是否允许在移动网络下可用 实时语音 isAllow : " + enabled);
        }

        public bool GetMobileNetworkEnabled()
        {
            return YouMeVoiceAPI.GetInstance().GetUseMobileNetworkEnabled();
        }

        //当麦克风静音时，释放麦克风设备，此时允许第三方模块使用麦克风设备录音。在Android上，语音通过媒体音轨，而不是通话音轨输出。
        //需要在初始化成功后，加入房间之前调用
        void SetYoumiReleaseMicWhenMicOff(bool isRelease)
        {
            //if (!CheckYouMiTalkVoiceJoinState())
            //{
            //    return;
            //}
            talkMethodResult = YouMeVoiceAPI.GetInstance().SetReleaseMicWhenMute(isRelease);
            LogForYouMiTalk("SetYoumiReleaseMicWhenMicOff", talkMethodResult);
        }

        //设置插耳机的情况下开启或关闭语音监听（即通过耳机听到自己说话的内容）。
        //这是一个同步调用接口。
        void SetYoumiHeadsetMontorOn(bool enabled)
        {
            if (!CheckYouMiTalkVoiceJoinState())
            {
                return;
            }
            talkMethodResult = YouMeVoiceAPI.GetInstance().SetHeadsetMonitorOn(enabled, enabled);
            LogForYouMiTalk("SetYoumiHeadsetMontorOn", talkMethodResult);
        }

        //设置是否通知其他人，自己开关麦克风扬声器的状态
        void SetYoumiAutoSendStatus(bool isAutoSend)
        {
            if (!CheckYouMiTalkVoiceJoinState())
            {
                return;
            }
            YouMeVoiceAPI.GetInstance().SetAutoSendStatus(isAutoSend);
            LogForYouMiTalk("SetYoumiAutoSendStatus", YouMeErrorCode.YOUME_SUCCESS);
        }

        //开启频道内有人讲话和结束讲话的通知
        void SetYoumiTalkVoiceListener(bool isOpen)
        {
            if (!CheckYouMiTalkVoiceJoinState())
            {
                return;
            }
            talkMethodResult = YouMeVoiceAPI.GetInstance().SetVadCallbackEnabled(isOpen);
            LogForYouMiTalk("SetYoumiTalkVoiceListener", talkMethodResult);
        }

        //是否输出到扬声器
        void SetYoumiOutputToPlayer(bool outToPlayer)
        {
            if (!CheckYouMiTalkVoiceJoinState())
            {
                return;
            }
            talkMethodResult = YouMeVoiceAPI.GetInstance().SetOutputToSpeaker(outToPlayer);
            LogForYouMiTalk("SetOutputToPlayer", talkMethodResult);
        }

        // 获取频道内玩家列表，并且设置是否监听频道内玩家变动
        void _GetChannelUserList(string channelId, bool stayListen)
        {
            if (string.IsNullOrEmpty(channelId))
            {
                return;
            }
            if (!CheckYouMiTalkVoiceJoinState())
            {
                return;
            }
            talkMethodResult = YouMeVoiceAPI.GetInstance().GetChannelUserList(channelId, -1, stayListen);
            LogForYouMiTalk("_GetChannelUserList", talkMethodResult);
        }

        bool _SetOtherMicStatus(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }
            if (cacheChatVoiceAccInfo != null && cacheChatVoiceAccInfo.accId == userId)
            {
                return false;
            }
            return _SetOtherMicMute(userId, isGlobalSilence);
        }
        //开启其他人麦，相当于恢复其他人对自己麦的控制权
        bool _SetOtherMicMute(string userId, bool isMute)
        {
            talkMethodResult = YouMeVoiceAPI.GetInstance().SetOtherMicMute(userId, isMute);
            LogForYouMiTalk("_SetOtherMicStatus", talkMethodResult, "Set Other Mic Mute : " + isMute);
            if (talkMethodResult == YouMeErrorCode.YOUME_SUCCESS)
            {
                return true;
            }
            return false;
        }

        //设置是否听到某人声音
        bool _SetListenOtherVoice(string userId, bool isOn)
        {
            talkMethodResult = YouMeVoiceAPI.GetInstance().SetListenOtherVoice(userId, isOn);
            LogForYouMiTalk("_SetListenOtherVoice", talkMethodResult, "Set Listen Other Voice : " + isOn);
            if (talkMethodResult == YouMeErrorCode.YOUME_SUCCESS)
            {
                return true;
            }
            return false;
        }

        //通知别人自己麦克风和扬声器的状态
        void _SetSendSelfMicAndPlayerStatus(bool bSend)
        {
            YouMeVoiceAPI.GetInstance().SetAutoSendStatus(bSend);
        }

        //在语音频道内，广播一个文本消息
        void _SendBroadcastMsg(string talkChannelId, string content)
        {
            int reqId = 0;
            talkMethodResult = YouMeVoiceAPI.GetInstance().SendMessage(talkChannelId, content, ref reqId);
            LogForYouMiTalk("SendBroadcastMsg", talkMethodResult, "Set Broadcast Message : " + talkChannelId + " msg : " + content);
        }

        string _GenerateBroadcastMsg(TalkBroadcastType type, string param1 = "")
        {
            if (type == TalkBroadcastType.None || type == TalkBroadcastType.Count)
            {
                return "";
            }
            string typeStr = TALK_BROADCAST_TYPES[(int)type];
            string voicePlayerId = "";
            if (cacheChatVoiceAccInfo != null)
            {
                voicePlayerId = cacheChatVoiceAccInfo.accId;
            }
            if (string.IsNullOrEmpty(param1))
            {
                return string.Format("{0}|{1}", typeStr, voicePlayerId);
            }
            else
            {
                return string.Format("{0}|{1}|{2}", typeStr, voicePlayerId, param1);
            }
        }

        string _GetVoicePlayerIdFromBCMsg(string[] broadcastMsgs)
        {
            string playerId = "";
            if (null == broadcastMsgs || broadcastMsgs.Length < 2)
            {
                return playerId;
            }
            playerId = broadcastMsgs[1];
            return playerId;
        }

        string _GetVoicePlayerIdFromBCMsg(string broadcastMsg)
        {
            string playerId = "";
            if (string.IsNullOrEmpty(broadcastMsg))
            {
                return playerId;
            }
            string[] msgArray = broadcastMsg.Split('|');
            playerId = _GetVoicePlayerIdFromBCMsg(msgArray);
            return playerId;
        }

        TalkBroadcastType _GetTalkBroadcastTypeFromBCMsg(string[] broadcastMsgs)
        {
            if (null == broadcastMsgs || broadcastMsgs.Length < 2)
            {
                return TalkBroadcastType.None;
            }
            string typeStr = broadcastMsgs[0];
            for (int i = 0; i < TALK_BROADCAST_TYPES.Length; i++)
            {
                if (TALK_BROADCAST_TYPES[i] == typeStr)
                {
                    return (TalkBroadcastType)i;
                }
            }
            return TalkBroadcastType.None;
        }
        TalkBroadcastType _GetTalkBroadcastTypeFromBCMsg(string broadcastMsg)
        {
            if (string.IsNullOrEmpty(broadcastMsg))
            {
                return TalkBroadcastType.None;
            }
            string[] msgArray = broadcastMsg.Split('|');
            return _GetTalkBroadcastTypeFromBCMsg(msgArray);
        }

        string _GetVoiceParam1FromBCMsg(string[] broadcastMsgs)
        {
            string param1 = "";
            if (null == broadcastMsgs || broadcastMsgs.Length < 3)
            {
                return param1;
            }
            param1 = broadcastMsgs[2];
            return param1;
        }

        string _GetVoiceParam1FromBCMsg(string broadcastMsg)
        {
            string param1 = "";
            if (string.IsNullOrEmpty(broadcastMsg))
            {
                return param1;
            }
            string[] msgArray = broadcastMsg.Split('|');
            param1 = _GetVoiceParam1FromBCMsg(msgArray);
            return param1;
        }

        #endregion

        public void OnVoiceEventCallback(object sender, SDKVoiceEventArgs e)
        {
            if (null == e)
            {
                return;
            }
            switch (e.eventType)
            {
                case SDKVoiceEventType.TalkInitSucc:
                    _OnTalkVoiceInitSucc();
                    break;
                case SDKVoiceEventType.TalkInitFailed:
                    _OnTalkVoiceInitFailed();
                    break;
                case SDKVoiceEventType.TalkJoinChannel:
                    _OnTalkVoiceJoinChannel(e.status, e.param1 as string);              
                    break;
                case SDKVoiceEventType.TalkLeaveChannel:
                    _OnTalkVoiceLeaveChannel(e.status, e.param1 as string);
                    break;
                case SDKVoiceEventType.TalkPauseChannel:
                    _OnTalkVoiceChannelPause();
                    break;
                case SDKVoiceEventType.TalkResumeChannel:
                    _OnTalkVoiceChannelResume();
                    break;
                case SDKVoiceEventType.TalkMicSwitch:
                    _OnTalkVoiceMicSwitch(e.status);
                    break;
                case SDKVoiceEventType.TalkPlayerSwitch:
                    _OnTalkVoicePlayerSwitch(e.status);
                    break;                
            }
        }

        #region YM Chat Voice Callback Listener
        public void OnUpdateLocation(ErrorCode errorcode, GeographyLocation location)
        {

        }

        public void OnGetNearbyObjects(ErrorCode errorcode, List<RelativeLocation> neighbourList, uint startDistance, uint endDistance)
        {

        }

        public void OnGetContact(List<ContactsSessionInfo> contactLists)
        {

        }

        public void OnGetUserInfo(ErrorCode code, string userID, IMUserInfo userInfo)
        {

        }

        public void OnQueryUserStatus(ErrorCode code, string userID, UserStatus status)
        {

        }

        public void OnPlayCompletion(ErrorCode errorcode, string path)
        {
            LogForYouMiChat("OnPlayCompletion", errorcode, "voice play end , voice path is " + path);
            if (errorcode == ErrorCode.Success ||
                errorcode == ErrorCode.PTT_IsPlaying)  //表示播放到中途被打断
            {
                args.Reset(SDKVoiceEventType.ChatPlayEnd);
                Invoke(args);

                //轮训播放语音队列！
                if (this.voiceQueue != null && this.voiceQueue.Count > 0)
                {
                    LogForYouMiChat("Start Play Voice Queue", errorcode, "curr voice queue length is " + this.voiceQueue.Count);
                    PlayVoiceQueue();
                }
                else
                {
                    LogForYouMiChat("Start Play Voice Queue is NULL, could do some things other", errorcode);

                    //语音播放结束，语音队列中已空!
                    //if (onVoicePlayCompleteHandler != null)
                    //{
                    //    onVoicePlayCompleteHandler();
                    //}
                    //else
                    //{
                    //    LogForYouMiChat("OnPlayCompletion : onVoicePlayCompleteHandler is null !!!", errorcode);
                    //}
                }
            }
        }

        public void OnGetMicrophoneStatus(AudioDeviceStatus status)
        {
            LogForYouMiChat("OnGetMicrophoneStatus", ErrorCode.Success, "curr AudioDeviceStatus is " + status.ToString());

            //if (status != AudioDeviceStatus.STATUS_AVAILABLE && isRecordPTTFailed)
            //{
            //    GameClient.UIEventSystem.GetInstance().SendUIEvent(GameClient.EUIEventID.OnVoiceChatMicNoAuth);
            //}
        }

        public void OnJoinRoom(ErrorCode errorcode, string strChatRoomID)
        {
            LogForYouMiChat("OnJoinRoom", errorcode, "strChatRoomID = " + strChatRoomID);
            if (errorcode != ErrorCode.Success)
            {
                //error : TryJoinChatRoom(strChatRoomID);
            }
            else
            {
                //if (joinChatRoomState != JoinChatRoomState.Leaving)
                //{
                //    joinChatRoomState = JoinChatRoomState.Leaved;
                //}

                args.Reset(SDKVoiceEventType.ChatJoinRoom, strChatRoomID);
                Invoke(args);

                if (voiceChatRoomIdList != null)
                {
                    if (!voiceChatRoomIdList.Contains(strChatRoomID))
                    {
                        voiceChatRoomIdList.Add(strChatRoomID);
                    }
                }
            }
        }

        public void OnLeaveRoom(ErrorCode errorcode, string strChatRoomID)
        {
            LogForYouMiChat("OnLeaveRoom", errorcode, "strChatRoomID = " + strChatRoomID);
            if (errorcode != ErrorCode.Success)
            {
                //error : TryLeaveChatRoom(strChatRoomID);
            }
            else
            {
                //joinChatRoomState = JoinChatRoomState.Leaved;

                args.Reset(SDKVoiceEventType.ChatLeaveRoom, strChatRoomID);
                Invoke(args);
            }
        }

        public void OnUserJoinChatRoom(string strRoomID, string strUserID)
        {
            LogForYouMiChat("OnUserJoinChatRoom", ErrorCode.Success, "有玩家 : " + strUserID + ",加入了房间 ：strChatRoomID = " + strRoomID);
        }

        public void OnUserLeaveChatRoom(string strRoomID, string strUserID)
        {
            LogForYouMiChat("OnUserLeaveChatRoom", ErrorCode.Success, "有玩家 : " + strUserID + ",离开了房间 ：strChatRoomID = " + strRoomID);
        }

        public void OnGetRoomMemberCount(ErrorCode errorcode, string strRoomID, uint count)
        {
            LogForYouMiChat("OnGetRoomMemberCount", errorcode, strRoomID + " 号房间内当前一共有玩家人数 : " + count);
        }

        public void OnSendMessageStatus(ulong iRequestID, ErrorCode errorcode, uint sendTime, bool isForbidRoom, int reasonType, ulong forbidEndTime)
        {
            //文本消息和自定义消息
        }

        public void OnRecvMessage(MessageInfoBase message)
        {
            if (message == null)
                return;
            if (message.MessageType == MessageBodyType.Voice)
            {
                LogForYouMiChat("OnRecvMessage", ErrorCode.Success, "message is voice");

                VoiceMessage voiceMessage = message as VoiceMessage;
                if (null == voiceMessage)
                {
                    Logger.LogProcessFormat("YouMiVoice--message 异常.");
                    return;
                }

                LogForYouMiChat("OnRecvMessage", ErrorCode.Success, "message is voice  req id is " + voiceMessage.RequestID + " | req token is " + voiceMessage.Param);

                if (string.IsNullOrEmpty(voiceMessage.Param))
                {
                    Logger.LogProcessFormat("YouMiVoice--message 透传参数为空.");
                    return;
                }

                AudioInfo audio = new AudioInfo();
                audio.token = voiceMessage.Param;
                audio.requestId = voiceMessage.RequestID;
                audio.status = DownloadStatus.DS_NOTDOWNLOAD;
                audio.path = _BuildRequestIdPath(audio.requestId);

                LogForYouMiChat("OnRecvMessage", ErrorCode.Success, string.Format("AudioInfo token : {0}, requestId : {1}, path : {2}", audio.token, audio.requestId, audio.path));

                if (_AudioCache.ContainsKey(voiceMessage.Param))
                {
                    LogForYouMiChat("OnRecvMessage", ErrorCode.Success, "TOKEN冲突");
                    _AudioCache.Remove(voiceMessage.Param);
                }

                _AudioCache.Add(voiceMessage.Param, audio);

                if (audio.token.Equals(_currentReqPlayAudioToken))
                {
                    DownloadAudioFile(audio);
                }
            }
            else
            {
                LogForYouMiChat("OnRecvMessage", ErrorCode.Success, "message is not voice");
            }
        }

        //录音结束，开始发送录音的通知，这个时候已经可以拿到语音文件进行播放
        public void OnStartSendAudioMessage(ulong iRequestID, ErrorCode errorcode, string strText, string strAudioPath, int iDuration)
        {
            isRecording = false;

            LogForYouMiChat("OnStartSendAudioMessage", errorcode, string.Format("reqID {0} , text {1} , path {2} , duration {3}", iRequestID, strText, strAudioPath, iDuration));
            if (errorcode == ErrorCode.Success || errorcode == ErrorCode.PTT_ReachMaxDuration)
            {
                //如果翻译失败
                if (string.IsNullOrEmpty(strText))
                {
                    strText = TR.Value("voice_sdk_message");//VoiceInputBtnTextCN.VOICE_MESSAGE; 
                }

                if (string.IsNullOrEmpty(_leastLocalAudioToken))
                {
                    LogForYouMiChat("OnStartSendAudioMessage", errorcode, "这条消息没有透传参数");
                    args.Reset(SDKVoiceEventType.ChatSendFailed);
                    Invoke(args);
                    return;
                }

                if (_AudioCache.ContainsKey(_leastLocalAudioToken))
                {
                    LogForYouMiChat("OnStartSendAudioMessage", errorcode, "TOKEN冲突");
                    _AudioCache.Remove(_leastLocalAudioToken);
                }

                AudioInfo audio = new AudioInfo();
                audio.token = _leastLocalAudioToken;
                audio.requestId = iRequestID;
                audio.path = strAudioPath;
                audio.status = DownloadStatus.DS_SUCCESS;

                _AudioCache.Add(_leastLocalAudioToken, audio);

                //再构造语音上传到游戏的key值 ： 角色ID | 服务器时间戳 | 语音请求识别码
                //string voiceKey = string.Format(AUDIO_TOKEN_FORMAT, _leastLocalAudioToken, iRequestID);

                SDKVoiceChatRecordInfo recordInfo = new SDKVoiceChatRecordInfo();
                recordInfo.voiceKey = _leastLocalAudioToken;
                recordInfo.transVoiceText = strText;
                recordInfo.saveAudioPath = strAudioPath;
                recordInfo.duration = iDuration;
                args.Reset(SDKVoiceEventType.ChatSendStart, recordInfo);
                Invoke(args);

                //重置
                _leastLocalAudioToken = "";
            }
            else
            {
                args.Reset(SDKVoiceEventType.ChatRecordFailed);
                Invoke(args);
            }
        }

        // 自己的语音消息发送成功或者失败的通知
        public void OnSendAudioMessageStatus(ulong iRequestID, ErrorCode errorcode, string strText, string strAudioPath, int iDuration, uint sendTime, bool isForbidRoom, int reasonType, ulong forbidEndTime)
        {
            isRecording = false;

            LogForYouMiChat("OnSendAudioMessageStatus", errorcode, string.Format("reqID {0} , text {1} , path {2} , duration {3} , sendTime {4}", iRequestID, strText, strAudioPath, iDuration, sendTime));
            if (errorcode == ErrorCode.Success || errorcode == ErrorCode.PTT_ReachMaxDuration)
            {
                string ymUserId = "";
                if (cacheChatVoiceAccInfo != null)
                {
                    ymUserId = cacheChatVoiceAccInfo.YoumiId;
                }

                args.Reset(SDKVoiceEventType.ChatSendEnd, iRequestID.ToString());
                Invoke(args);

                //GameStatisticManager.GetInstance().DoYouMiVoiceIM(YouMiVoiceSDKCostType.VOICE_SDK_SENDED, string.Format("sendVoice|reqId,{0}|", iRequestID));
                //SDKVoiceManager.GetInstance().ReportUsingVoice(Protocol.CustomLogReportType.CLRT_SEND_RECORD_VOICE, iRequestID + "");
            }
            else
            {
                args.Reset(SDKVoiceEventType.ChatSendFailed);
                Invoke(args);
            }
        }

        public void OnStopAudioSpeechStatus(ErrorCode errorcode, ulong iRequestID, string strDownloadURL, int iDuration, int iFileSize, string strLocalPath, string strText)
        {
            LogForYouMiChat("OnStopAudioSpeechStatus", errorcode, string.Format("reqID {0} , text {1} , path {2} , duration {3", iRequestID, strText, strLocalPath, iDuration));
            if (errorcode == ErrorCode.Success)
            {
                args.Reset(SDKVoiceEventType.ChatRecordEnd);
            }
            else
            {
                args.Reset(SDKVoiceEventType.ChatRecordFailed);
            }
            Invoke(args);
        }

        /// <summary>
        /// 获取录音音量回调
        /// </summary>
        /// <param name="volume"></param>
        public void OnRecordVolume(int volume)
        {
            LogForYouMiChat("OnRecordVolume", ErrorCode.Success, string.Format("当前录音音量为 ： " + volume));
        }

        public void OnQueryHistoryMessage(ErrorCode errorcode, string targetID, int remain, List<HistoryMsg> messageList)
        {
            LogForYouMiChat("OnQueryHistoryMessage", errorcode, string.Format("OnQueryHistoryMessage targetId is {0}, remain msg num is {1}", targetID, remain));
            if (errorcode == ErrorCode.Success)
            {
                List<HistoryMsg> msgList = messageList;
                if (msgList == null)
                {
                    Logger.LogError("OnQueryHistoryMessage callback messageList is null !!!");
                    return;
                }

                LogForYouMiChat("OnQueryHistoryMessage", errorcode, string.Format("OnQueryHistoryMessage msgListCount is {0}", msgList.Count));

                bool isFindMsg = false;
                int currMsgListCount = msgList.Count;
                int msgTotalCount = currMsgListCount + remain;

                //是否需要进行缓存清理
                if (isQueryHistoryMsgClear)
                {
                    for (int i = 0; i < msgList.Count; i++)
                    {
                        var msg = msgList[i];
                        if (msg.MessageType != MessageBodyType.Voice)
                        {
                            return;
                        }
                        string voiceParam = msg.Param;
                        //LogForYouMiChat("isQueryHistoryMsgClear is true", ErrorCode.Success, "voice param is " + msg.Param);
                        if (IsPrivateChat(voiceParam))
                        {
                            DeleteRoomChatMsgByMsgId(msg.MessageID);
                        }
                    }

                    if (remain <= 0)
                    {
                        if (voiceChatRoomIdList == null)
                        {
                            TryLogoutYoumiChatVoice();
                            return;
                        }
                        else
                        {
                            if (voiceChatRoomIdList.Contains(targetID))
                            {
                                voiceChatRoomIdList.Remove(targetID);
                            }
                            if (voiceChatRoomIdList.Count <= 0)
                            {
                                TryLogoutYoumiChatVoice();
                                return;
                            }
                            QueryHistoryMsgToClear(voiceChatRoomIdList[0], privateChatQueryMsgCount);
                        }
                        return;
                    }

                    int nextStartMsgId = msgTotalCount - currMsgListCount;

                    if (privateChatQueryMsgCount > remain)
                    {
                        QueryHistoryMsgToClear(targetID, remain, (ulong)nextStartMsgId);
                    }
                    else
                    {
                        QueryHistoryMsgToClear(targetID, privateChatQueryMsgCount, (ulong)nextStartMsgId);
                    }

                    return;
                }

                for (int i = 0; i < currMsgListCount; i++)
                {
                    HistoryMsg hMsg = msgList[i];
                    //if(hMsg.MessageType == MessageBodyType.Voice && hMsg.ChatType == ChatType.PrivateChat)
                    if (hMsg.MessageType == MessageBodyType.Voice && hMsg.ChatType == ChatType.RoomChat)
                    {
                        if (hMsg.Param.Equals(_currentReqPlayAudioToken))
                        {
                            TryPlayVoiceByPath(hMsg.LocalPath);
                            isFindMsg = true;
                            LogForYouMiChat("OnQueryHistoryMessage", errorcode, string.Format("hMsg MessageType sendId is {0} , recId is {1} , MessageID is {2}", hMsg.SenderID, hMsg.ReceiveID, hMsg.MessageID));
                            break;
                        }
                    }
                }
                if (remain <= 0)
                {
                    return;
                }

                if (!isFindMsg)
                {
                    int nextStartMsgId = msgTotalCount - currMsgListCount;

                    if (privateChatQueryMsgCount > remain)
                    {
                        QueryPrivateHistoryMsgByUserId(targetID, remain, (ulong)nextStartMsgId);
                    }
                    else
                    {
                        QueryPrivateHistoryMsgByUserId(targetID, privateChatQueryMsgCount, (ulong)nextStartMsgId);
                    }
                }
            }
        }

        /*
        public void OnStopAudioSpeechStatus(ErrorCode errorcode, ulong iRequestID, string strDownloadURL, int iDuration, int iFileSize, string strLocalPath, string strText)
        {
            isRecording = false;

            LogForYouMiChat("OnStopAudioSpeechStatus", errorcode, "strDownloadURL " + strDownloadURL +
                        "| iDuration " + iDuration + "| iFileSize " + iFileSize + "! strLocalPath " + strLocalPath + "| strText " + strText);
            if (errorcode == ErrorCode.Success)
            {
                if (string.IsNullOrEmpty(strDownloadURL) || string.IsNullOrEmpty(strLocalPath))
                {
                    LogForYouMiChat("OnStopAudioSpeechStatus", ErrorCode.Success, "strDownloadURL = " + strDownloadURL + " | strLocalPath = " + strLocalPath);
                    if (onRecordFailedHandler != null)
                    {
                        onRecordFailedHandler();
                    }
                    return;
                }

                if (onRecordEndHandler != null)
                {
                    onRecordEndHandler();
                }

                if (isSendVoiceReadyHandler != null)
                {
                    if (isSendVoiceReadyHandler() == false)
                    {
                        return;
                    }
                    if (sendRichMsgToServerHandler != null)
                    {
                        //如果翻译失败
                        if (string.IsNullOrEmpty(strText))
                        {
                            strText = TR.Value("voice_sdk_message");//VoiceInputBtnTextCN.VOICE_MESSAGE;
                        }
                        RichMsgData richData = new RichMsgData(strText, strLocalPath, iDuration, strDownloadURL);
                        sendRichMsgToServerHandler(richData);
                    }
                }
            }
            else
            {
                if (onRecordFailedHandler != null)
                {
                    onRecordFailedHandler();
                }
            }

        }
         * */

        public void OnRecvNewMessage(ChatType chatType, string targetID)
        {

        }

        public void OnAccusationResultNotify(AccusationDealResult result, string userID, uint accusationTime)
        {

        }

        public void OnGetForbiddenSpeakInfo(ErrorCode errorcode, List<ForbiddenSpeakInfo> forbiddenSpeakList)
        {

        }

        /// <summary>
        /// 只设置翻译的接口回调
        /// </summary>
        /// <param name="errorcode"></param>
        /// <param name="text"></param>
        public void OnGetRecognizeSpeechText(ulong iRequestID, ErrorCode errorcode, string text)
        {

        }

        public void OnBlockUser(ErrorCode errorcode, string userID, bool block)
        {

        }

        public void OnUnBlockAllUser(ErrorCode errorcode)
        {

        }

        public void OnGetBlockUsers(ErrorCode errorcode, List<string> userList)
        {

        }

        public void OnLogin(ErrorCode errorcode, string strYouMeID)
        {
            LogForYouMiChat("OnLogin", errorcode, "login strYouMeID " + strYouMeID);

            if (errorcode == ErrorCode.Success)
            {
                //isChatVoiceLogined = true;
                loginVoiceState = LoginVoiceState.Logined;
                if (cacheChatVoiceAccInfo != null)
                {
                    cacheChatVoiceAccInfo.YoumiId = strYouMeID;
                }

                //加入世界聊天房间
                //SDKVoiceManager.GetInstance().JoinWorldChannel();
                //SDKVoiceManager.GetInstance().TryJoinGuildChannel();
                //SDKVoiceManager.GetInstance().TryJoinTeamChannel();
                //SDKVoiceManager.GetInstance().TryJoinSceneChannel();

                args.Reset(SDKVoiceEventType.ChatLogin);
                Invoke(args);

                //埋点
                //GameStatisticManager.GetInstance().DoYouMiVoiceIM(YouMiVoiceSDKCostType.VOICE_SDK_LOGINED,"logined");
            }
            else
            {
                //error : TryLoginYoumiChatVoice(cacheChatVoiceAccInfo);
                //isChatVoiceLoiginFailed = false;
                //isChatVoiceLogined = false;
                loginVoiceState = LoginVoiceState.Logouted;
            }
        }

        public void OnLogout()
        {
            LogForYouMiChat("OnLogout !", ErrorCode.Success);

            //isChatVoiceLogined = false;
            loginVoiceState = LoginVoiceState.Logouted;

            args.Reset(SDKVoiceEventType.ChatLogout);
            Invoke(args);
        }

        public void OnDownload(ErrorCode errorcode, MessageInfoBase message, string strSavePath)
        {
            LogForYouMiChat("OnDownload", errorcode, "message RequestID : " + message.RequestID + " , savePath : " + strSavePath);
            if (errorcode == ErrorCode.Success)
            {
                if (string.IsNullOrEmpty(strSavePath))
                    return;

                VoiceMessage voiceMessage = message as VoiceMessage;
                if (null == voiceMessage)
                {
                    Logger.LogProcessFormat("YouMiVoice--message 异常.");
                    return;
                }

                string ymUserId = "";
                if (cacheChatVoiceAccInfo != null)
                {
                    ymUserId = cacheChatVoiceAccInfo.YoumiId;
                }

                args.Reset(SDKVoiceEventType.ChatDownloadRecordVoiceReport, voiceMessage.RequestID.ToString());
                Invoke(args);

                //GameStatisticManager.GetInstance().DoYouMiVoiceIM(YouMiVoiceSDKCostType.VOICE_SDK_DOWNLOADED, string.Format("downloadVoice|reqId,{0}|", voiceMessage.RequestID));
                //SDKVoiceManager.GetInstance().ReportUsingVoice(Protocol.CustomLogReportType.CLRT_LOAD_RECORD_VOICE, voiceMessage.RequestID + "");

                AudioInfo audio = null;
                if (_AudioCache == null)
                {
                    Logger.LogProcessFormat("YouMiVoice--message 语音字典未初始化 为空");
                    return;
                }
                if (!_AudioCache.ContainsKey(voiceMessage.Param))
                {
                    LogForYouMiChat("OnDownload", errorcode, "下载了未知的音频文件");

                    audio = new AudioInfo();
                    audio.token = voiceMessage.Param;
                    audio.requestId = voiceMessage.RequestID;
                    audio.path = strSavePath;
                    audio.status = DownloadStatus.DS_SUCCESS;
                    _AudioCache.Add(voiceMessage.Param, audio);
                    return;
                }
                else
                {
                    audio = _AudioCache[voiceMessage.Param];
                    audio.status = DownloadStatus.DS_SUCCESS;
                    audio.path = strSavePath;
                }

                if (audio.token.Equals(_currentReqPlayAudioToken))
                {
                    TryPlayVoiceByPath(audio.path);
                }
            }
        }

        public void OnDownloadByUrl(ErrorCode errorcode, string strFromUrl, string strSavePath)
        {
            LogForYouMiChat("OnDownloadByUrl", errorcode, "通过URL下载语音文件到指定路径");
            //bool isSuccess = false;
            //if (errorcode == ErrorCode.Success)
            //{
            //    isSuccess = true;
            //}
            //if (onDownloadVoiceHandler != null)
            //{
            //    onDownloadVoiceHandler(strSavePath, isSuccess);
            //}
        }

        public void OnKickOff()
        {

        }

        public void OnQueryRoomHistoryMessageFromServer(ErrorCode errorcode, string roomID, int remain, List<MessageInfoBase> messageList)
        {

        }

        /// <summary>
        /// 录音音量回调
        /// 频率 iOS 2次/s   Android 8次/s
        /// </summary>
        /// <param name="volume">音量 范围0-1</param>
        public void OnRecordVolumeChange(float volume)
        {
            args.Reset(SDKVoiceEventType.ChatRecordVolumeChanged);
            args.param1 = new float[] { volume };
            Invoke(args);
        }

        public void OnLeaveAllRooms(ErrorCode errorcode)
        {

        }

        public void OnGetDistance(ErrorCode errorcode, string userID, uint distance)
        {

        }

        #endregion

        #region YM Talk Voice Callback Listener

        private void _OnTalkVoiceInitSucc()
        {
            isTalkVoiceInited = true;

            //实时语音一些参数配置
            SetMobileNetworkEnabled(true);
            SetYoumiReleaseMicWhenMicOff(true);
        }

        private void _OnTalkVoiceInitFailed()
        {
            isTalkVoiceInited = false;
        }

        private void _OnTalkVoiceJoinChannel(bool bSucc, string channelId)
        {
            if(bSucc)
            {
                if (talkChannelIds != null && !talkChannelIds.Contains(channelId))
                {
                    talkChannelIds.Add(channelId);
                }

                args.Reset(SDKVoiceEventType.TalkJoinChannelSucc);
                Invoke(args);

                //实时语音一些参数配置
                SetYoumiAutoSendStatus(true);
                SetYoumiTalkVoiceListener(true);

                if (!string.IsNullOrEmpty(channelId))
                {
                    joinTalkRoomStartTime = GameServerTime();
                    //GameStatisticManager.GetInstance().DoYouMiVoiceIM(YouMiVoiceSDKCostType.VOICE_SDK_JOIN_TALK, string.Format("jointalk|channelId,{0}|", cacheChatVoiceAccInfo.TalkChannelId));
                    //SDKVoiceManager.GetInstance().ReportUsingVoice(Protocol.CustomLogReportType.CLRT_JOIN_VOICE_ROOM, cacheChatVoiceAccInfo.TalkChannelId + "|" + joinTalkRoomStartTime);
                }
                string recordInfo = channelId + "|" + joinTalkRoomStartTime;
                args.Reset(SDKVoiceEventType.TalkJoinChannelSuccReport, recordInfo);
                Invoke(args);
                //尝试测试是不是会提示获取语音权限
                if (!hasReqVoiceAuth)
                {
                    //ulong req = 0;
                    //IMAPI.Instance().StartAudioSpeech(ref req, false);   
                    //IMAPI.Instance().StopAudioSpeech();
                    AudioDeviceStatus status = TryGetMicroPhoneStatus();
                    if (status != AudioDeviceStatus.STATUS_AVAILABLE)
                    {
                        args.Reset(SDKVoiceEventType.TalkMicAuthNoPermission);
                        Invoke(args);
                    }
                    hasReqVoiceAuth = true;
                }
            }
        }

        private void _OnTalkVoiceLeaveChannel(bool bSucc, string channelId)
        {
            if(bSucc)
            {
                if (talkChannelIds != null)
                {
                    talkChannelIds.Remove(channelId);
                }
                args.Reset(SDKVoiceEventType.TalkLeaveChannelSucc);
                Invoke(args);

                ulong inTalkRoomTime = 0;
                if (!string.IsNullOrEmpty(channelId))
                {
                    ulong leaveTalkRoomStartTime = GameServerTime();
                    inTalkRoomTime = leaveTalkRoomStartTime - joinTalkRoomStartTime;
                    joinTalkRoomStartTime = 0;
                }

                string recordInfo = channelId + "|" + inTalkRoomTime;
                args.Reset(SDKVoiceEventType.TalkLeaveChannelSuccReport, recordInfo);
                Invoke(args);
            }
        }
        private void _OnTalkVoiceLeaveAllChannelsSucc()
        {
            if (talkChannelIds != null)
            {
                talkChannelIds.Clear();
            }
            args.Reset(SDKVoiceEventType.TalkLeaveChannelSucc);
            Invoke(args);            

            //离开所有房间时  重置状态
            _ResetVoiceTalk();

            ulong leaveTalkRoomStartTime = GameServerTime();
            ulong inTalkRoomTime = leaveTalkRoomStartTime - joinTalkRoomStartTime;
            joinTalkRoomStartTime = 0;

            string recordInfo = inTalkRoomTime.ToString();
            args.Reset(SDKVoiceEventType.TalkLeaveChannelSuccReport, recordInfo);
            Invoke(args);
        }


        private void _OnTalkVoiceChannelPause()
        {
            voiceTalkPauseState = VoiceTalkPauseState.Paused;
            ResumeTalkChannel();
        }

        private void _OnTalkVoiceChannelResume()
        {
            voiceTalkPauseState = VoiceTalkPauseState.Resumed;
        }

        private void _OnTalkVoiceMicSwitch(bool isOn)
        {
            bool isNeedForceOffMic = isOn && isTalkMicAutoOn && isTalkMicForceOffDirty;
            if (!isNeedForceOffMic)
            {                
                args.Reset(isOn ? SDKVoiceEventType.TalkMicOn : SDKVoiceEventType.TalkMicOff);
                Invoke(args);
            }
            if (!isTalkMicEnableChangedDirty)
            {
                isTalkMicMaunalOn = isOn;
            }
            //话筒开启时，听筒必定开启，若听筒关闭状态下，打开话筒，则听筒自动开启
            if (isOn && !IsTalkRealPlayerOn() && !isTalkMicAutoOn)
            {
                OpenRealPlayer();
            }
        }

        private void _OnTalkVoicePlayerSwitch(bool isOn)
        {
            args.Reset(isOn ? SDKVoiceEventType.TalkPlayerOn : SDKVoiceEventType.TalkPlayerOff);
            Invoke(args);

            //关闭听筒时，若话筒为开启状态，则自动关闭话筒
            if (!isOn && IsTalkRealMicOn())
            {
                CloseRealMic();
            }
        }

        public void OnChannelMemberChange(IList<TalkChannelMemberInfo> members)
        {
            isSetGlobalSilenceSucc = true;
			
            List<TalkChannelMemberInfo> talkChannelMembers = members as List<TalkChannelMemberInfo>;
            if (talkChannelMembers != null && talkChannelMembers.Count > 0)
            {
                talkChannelMembers.ForEach(_ForeachChannelMembers);
            }

            //if(isSetGlobalSilenceDirty)
            {
                if(!isSetGlobalSilenceSucc)
                {
                    //其中一个未设置成功，则需要重新设置
                    _ResetGlobalSilenceStatus();
                    LogForYouMiTalk("SetGlobalSilenceInMainChannel - OnChannelMemberChange", talkMethodResult, "2 set global silence failed " + this.isGlobalSilence);
                }
                args.Reset(SDKVoiceEventType.TalkCtrlGlobalSilence, isGlobalSilence);
                Invoke(args);
            }

            LogForYouMiTalk("OnChannelMemberChange", YouMeErrorCode.YOUME_SUCCESS, "talk channel members count is " + members.Count);
        }

        private void _ForeachChannelMembers(TalkChannelMemberInfo memberInfo)
        {
            string _userId = memberInfo.userId;
            if(string.IsNullOrEmpty(_userId))
            {
                return;
            }
            if(cacheChatVoiceAccInfo != null && cacheChatVoiceAccInfo.accId == _userId)
            {
                return;
            }
            if(memberInfo.isJoined)
            {                
                //if(isSetGlobalSilenceDirty)
                {
                    isSetGlobalSilenceSucc = _SetOtherMicStatus(_userId);
                }

                //获取频道内其他玩家信息 存下来
                //_AddOtherTalkChannelInfo(memberInfo.talkChannelId, _userId, false);
            }
            else
            {
                //if(isSetGlobalSilenceDirty)
                {
                    //如果玩家离开频道 则恢复
                    _SetOtherMicMute(_userId, false);

                    args.Reset(SDKVoiceEventType.TalkOtherLeaveChannel, memberInfo.talkChannelId, _userId);    
                    Invoke(args);                
                }
            }
        }

        public void OnTalkVoiceMicChangeByOther(bool isOn, string voicePlayerId, YouMeErrorCode errorCode)
        {
            if (errorCode == YouMeErrorCode.YOUME_SUCCESS)
            {                
                //设置麦是否可用
                _SetCurrentMicSilenceStatus(isOn);
                //同步禁言开启状态
                if(cacheChatVoiceAccInfo != null && voicePlayerId != cacheChatVoiceAccInfo.accId)
                {            
                    //重置自己可设置他人禁言的标志  无法再设置了
                    //isSetGlobalSilenceDirty = false;

                    _SetGlobalSilenceStatus(!isOn);
                    LogForYouMiTalk("OnTalkVoiceMicChangeByOther", errorCode, "Current Global Silence is : " + isGlobalSilence);
                }
                LogForYouMiTalk("SetGlobalSilenceInMainChannel - OnTalkVoiceMicChangeByOther", talkMethodResult, "3 set global silence " + this.isGlobalSilence);
            }
            LogForYouMiTalk("OnTalkVoiceMicChangeByOther", errorCode, "Set Mic on : " + isOn);
        }

        private void _SetGlobalSilenceStatus(bool isSilence)
        {
            isLastGlobalSilence = isGlobalSilence;
            isGlobalSilence = isSilence;

            if(isLastGlobalSilence != isGlobalSilence)
            {                
                args.Reset(SDKVoiceEventType.TalkGlobalSilenceChanged, isSilence);
                Invoke(args);
            }
        }

        private void _ResetGlobalSilenceStatus()
        {
            if(isLastGlobalSilence != isGlobalSilence)
            {
                args.Reset(SDKVoiceEventType.TalkGlobalSilenceChanged, isLastGlobalSilence);
                Invoke(args);
            }
            isGlobalSilence = isLastGlobalSilence;
        }

        private void _SetCurrentMicSilenceStatus(bool isMicEnable)
        {
            args.Reset(SDKVoiceEventType.TalkMicChangeByOther, isMicEnable);
            Invoke(args);

            isTalkMicEnable = isMicEnable;
			isTalkMicEnableChangedDirty = true;
            isTalkMicForceOffDirty = false;
            if (!isTalkMicMaunalOn)
            {
                isTalkMicForceOffDirty = true;
            }
            if (isMicEnable)
            {
                isTalkMicAutoOn = true;
                if (isTalkMicForceOffDirty)
                {
                    _CloseRealMicOnMicEnable();
                }
            }
        }

        public void OnSetSpeakChannel(bool success, string channelId, YouMeErrorCode erroCode)
        {
            if(success)
            {
                LogForYouMiTalk("OnSetSpeakChannel", erroCode, "Set Speak Channel Succ : "+ channelId);        
                //注意顺序                        
                args.Reset(SDKVoiceEventType.TalkChangeSpeakChannelSucc, channelId);
                Invoke(args);

                currTalkChannelId = channelId;

                //发广播消息 通知其他玩家 自己切换了说话频道 
                //需要在主频道广播 广播消息中带上本次切换的频道id
                //默认为第一个频道为主频道
                string bcMsg = _GenerateBroadcastMsg(TalkBroadcastType.ChangeTalkChannel, channelId);
                if(talkChannelIds != null && talkChannelIds.Count > 0)
                {
                    _SendBroadcastMsg(talkChannelIds[0], bcMsg);
                }
            }
            else
            {
                LogForYouMiTalk("OnSetSpeakChannel", erroCode, "Set Speak Channel Failed : "+ channelId);

                //这个太坑了！！！ 不知道是不是当前语音SDK版本的BUG  记录时间：2020年9月2日
                if(erroCode == YouMeErrorCode.YOUME_ERROR_UNKNOWN)
                {
                    LeaveTalkChannel(channelId);
                    JoinTalkChannel(channelId);

                    args.Reset(SDKVoiceEventType.TalkChangeSpeakChannelFailed);
                    Invoke(args);
                    return;
                }

                if(currTalkChannelId == channelId)
                {
                    args.Reset(SDKVoiceEventType.TalkChangeSpeakChannelFailed, currTalkChannelId);
                    Invoke(args);
                    return;
                }

                if(talkChannelIds != null && !talkChannelIds.Contains(channelId))
                {
                    JoinTalkChannel(channelId);                   
                }
                args.Reset(SDKVoiceEventType.TalkChangeSpeakChannelFailed);
                Invoke(args);
            }
        }

        public void OnBroadcastMsg(string channelId, string content, YouMeErrorCode errorCode)
        {
            if(errorCode == YouMeErrorCode.YOUME_SUCCESS)
            {
                int reqId = 0;
                if(int.TryParse(content, out reqId))
                {
                    LogForYouMiTalk("OnBroadcastMsg", errorCode, "Send Broadcast msg success : "+ reqId);
                    return;
                }
                string[] contents = content.Split('|');
                TalkBroadcastType type = _GetTalkBroadcastTypeFromBCMsg(contents);
                string voicePlayerId = _GetVoicePlayerIdFromBCMsg(contents);
                string newChannelId = _GetVoiceParam1FromBCMsg(contents);                
                LogForYouMiTalk("OnBroadcastMsg", errorCode, 
                    string.Format("Receive Broadcast msg, type : {0}, broadcast channelId : {1}, playerId : {2}, msg param1 : {3}",
                                     type.ToString(), channelId, voicePlayerId, newChannelId));
                switch(type)
                {
                    case TalkBroadcastType.ChangeTalkChannel:
                        if(!string.IsNullOrEmpty(newChannelId))
                        {
                            _AddOtherTalkChannelInfo(newChannelId, voicePlayerId, true);
                            args.Reset(SDKVoiceEventType.TalkOtherChannelChanged, newChannelId, voicePlayerId);
                            Invoke(args);
                        }
                        break;
                    case TalkBroadcastType.JoinTalkChannel:
                        _AddOtherTalkChannelInfo(channelId, voicePlayerId, true);
                        break;
                    case TalkBroadcastType.LeaveTalkChannel:
                        _RemoveOtherTalkChannelInfo(channelId, voicePlayerId);
                        break;
                }
            }
        }

        void _AddOtherTalkChannelInfo(string channelId, string voicePlayerId, bool bOverride)
        {
            if (otherTalkChannelInfos != null)
            {
                var info = otherTalkChannelInfos.Find(x => { return x.userId == voicePlayerId; });
                if (info == null)
                {
                    var otherTalkChannel = new OtheTalkChannelInfo()
                    {
                        userId = voicePlayerId,
                        currentTalkChannelId = channelId
                    };
                    otherTalkChannelInfos.Add(otherTalkChannel);
                }
                else
                {
                    if(bOverride)
                    {
                        info.currentTalkChannelId = channelId;
                    }
                }
            }
        }

        void _RemoveOtherTalkChannelInfo(string channelId, string voicePlayerId)
        {
            if (otherTalkChannelInfos != null)
            {
                otherTalkChannelInfos.RemoveAll(x => { return x.userId == voicePlayerId; });
            }
        }

        #endregion

        #region Log for youmi
        void LogForYouMiChat(string method, ErrorCode errorCode, string errorMsg = "", SDKVoiceLogLevel logLevel = SDKVoiceLogLevel.Error)
        {
            if (this.logLevel > 0)
            {
                cacheLogMsg = string.Format("[youmi voice] - Chat - method : {0} , errorCode : {1} , errorMsg : {2}", method, errorCode, errorMsg);
                SetLogLevel(cacheLogMsg, logLevel);
            }
        }

        void LogForYouMiTalk(string method, YouMeErrorCode errorCode, string errorMsg = "", SDKVoiceLogLevel logLevel = SDKVoiceLogLevel.Error)
        {
            if (this.logLevel > 0)
            {
                cacheLogMsg = string.Format("[youmi voice] - Talk -method : {0} , errorCode : {1} , errorMsg : {2}", method, errorCode, errorMsg);
                SetLogLevel(cacheLogMsg,logLevel);
            }
        }

        void SetLogLevel(string log, SDKVoiceLogLevel logLevel)
        {
            switch (logLevel)
            {
                case SDKVoiceLogLevel.Error:
                    Logger.LogError(log);
                    break;
                case SDKVoiceLogLevel.Warning:
                    Logger.LogProcessFormat(log);
                    break;
                case SDKVoiceLogLevel.Debug:
                    Logger.Log(log);
                    break;
            }
        }

        #endregion
    }
}