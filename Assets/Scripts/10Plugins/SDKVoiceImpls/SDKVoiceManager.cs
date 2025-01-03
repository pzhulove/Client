using System.Collections.Generic;
using UnityEngine;
using VoiceRoomType = GameClient.ChatType;
using Protocol;
using GameClient;

namespace VoiceSDK
{
    public class SDKVoiceManager : Singleton<SDKVoiceManager>
    {
        //TODO 整合
        public const string VOICE_MIC_SETTING_KEY = "SDKVoiceMicON";
        public const string VOICE_PLAYER_SETTING_KEY = "SDKVoicePlayerON";
        public const string VOICE_AUTO_PLAY_WORLD = "SDKVoiceAutoPlayWorld";
        public const string VOICE_AUTO_PLAY_TEAM = "SDKVoiceAutoPlayTeam";
        public const string VOICE_AUTO_PLAY_GUILD = "SDKVoiceAutoPlayGuild";
        public const string VOICE_AUTO_PLAY_NEARBY = "SDKVoiceAutoPlayNearby";
        public const string VOICE_AUTO_PLAY_PRIVATE = "SDKVoiceAutoPlayPrivate";
        public const string VOICE_PLAYER_VOLUMN = "SDKVoicePlayerVolumn";

        #region 开关

        public enum VoiceSDKSwitch
        {
            ChatVoice = 0,                                              //语音聊天
            ChatVoiceInGloabl = 1,                                      //城镇语音聊天
            TalkVoice = 2,                                              //实时语音
            TalkVoiceIn3v3Pvp = 3,                                      //3v3战斗实时语音
            TalkVoiceIn3v3Room = 4,                                     //3v3房间实时语音
            TalkVoiceInTeam = 5,                                        //组队实时语音
            TalkVoiceInTeamDuplication = 6,                             //团本实时语音

            All
        }

        //原先的开关
        public bool OpenChatVoice { get; private set; }
        public bool OpenTalkRealVocie { get; private set; }

        //新加开关
        private bool[] mVoiceSDKSwitches = new bool[(int)VoiceSDKSwitch.All];
        public bool GetVoiceSDKSwitch(VoiceSDKSwitch switchType)
        {
            return mVoiceSDKSwitches[(int)switchType];
        }

        public void InitVoiceSDK(uint voiceSwitchFlag)
        {
            Logger.LogProcessFormat("InitVoiceChatSDK voiceFlag = {0}", System.Convert.ToString(voiceSwitchFlag, 2).PadLeft(8, '0'));

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        OpenChatVoice = false;
        OpenTalkRealVocie = false;
#else
            if (mVoiceSDKSwitches != null)
            {
                int chatVoiceValue = (int)VoiceSDKSwitch.ChatVoice;
                int talkVoiceValue = (int)VoiceSDKSwitch.TalkVoice;
                for (int i = 0; i < mVoiceSDKSwitches.Length; i++)
                {
                    bool s = (voiceSwitchFlag & (uint)(1 << i)) == 0;
                    if (i >= chatVoiceValue && i < talkVoiceValue)
                    {
                        mVoiceSDKSwitches[i] = (i == chatVoiceValue) ? OpenChatVoice = s : (OpenChatVoice && s);
                    }
                    else if (i >= talkVoiceValue)
                    {
                        mVoiceSDKSwitches[i] = (i == talkVoiceValue) ? OpenTalkRealVocie = s : OpenTalkRealVocie && s;
                    }
                }
            }
#endif
            InitVoiceEnabled(OpenChatVoice, OpenTalkRealVocie);
            if (Global.Settings.isDebug)
            {
                SetVoiceDebugLevel(VoiceSDK.SDKVoiceLogLevel.Error);
            }
            else
            {
                SetVoiceDebugLevel(VoiceSDK.SDKVoiceLogLevel.None);
            }
            InitChatVoice();
            InitTalkVoice();
        }
        #endregion

        public bool IsInited { get; private set;}
        public bool IsChatVoiceEnabled {get; private set;}
        public bool IsTalkRealVoiceEnabled {get; private set;}

        private VoiceChatModule _voiceChatModule = null;
        public VoiceChatModule VoiceChatModule
        {
            get
            {
                if (_voiceChatModule != null)
                {
                    _voiceChatModule = new VoiceChatModule();
                }
                return _voiceChatModule;
            }
        }

        #region Global Params

        public bool IsRecordVoiceEnabled
        {
            get
            {
                return GetMicPref();
            }
        }
        public bool IsPlayVoiceEnabled
        {
            get
            {
                return GetPlayerPref();
            }
        }
        public bool IsAutoPlayInWorld
        {
            get
            {
                return GetWorldAutoPlayValue();
            }
        }
        public bool IsAutoPlayInTeam
        {
            get
            {
                return GetTeamAutoPlayValue();
            }
        }
        public bool IsAutoPlayInGuild
        {
            get
            {
                return GetGuildAutoPlayValue();
            }
        }
        public bool IsAutoPlayInNearby
        {
            get
            {
                return GetNearbyAutoPlayValue();
            }
        }
        public bool IsAutoPlayInPrivate
        {
            get
            {
                return GetPrivateAutoPlayValue();
            }
        }

        public float VoicePlayerVolume
        {
            get
            {
                return GetPlayerVolumnPref();
            }
        }
        #endregion

        #region Private Params

        //Game Info
        VoiceRoomType currRoomType = VoiceRoomType.CT_ALL;
        static string gameAccountId = string.Empty;
        static string gameRoleIdStr = string.Empty;
        static ulong gameRoleId = 0;
        static string gameOpenId = string.Empty;
        static string gameToken = string.Empty;
        static int gameServerId = 0;
        static string gameServerIdStr = string.Empty;
        static string gameChannelId = string.Empty;

        #endregion

        #region PUBLIC METHOD

        #region Menber Methods

        public override void Init()
        {
            SDKVoiceInterface.Instance.GameServerTime = _GetServerTimeStamp;
            SDKVoiceInterface.Instance.IsPrivateChat = _IsPrivateChat;
            IsInited = true;
        }

        public override void UnInit()
        {
            IsInited = false;
            Logger.LogProcessFormat("!!! SDKVoiceManager UnInit !!!");

            SDKVoiceInterface.Instance.UnInit();

            //清理语音自动播放队列 停止播放语音
            StopPlayVoice();

            //清理本地语音缓存 (暂时不清理本地语音缓存)
            //ClearLocalCache();

            //恢复系统音量
            ControlGameMusicMute(false);
            ControlGameMusicVolumn(false);

            //注意调用顺序 ， 先进行反初始化前的调用！！！
            UnInitChatVoice();
            UnInitTalkVoice();

            _RemoveAllVoiceEventListener();
        }

        public void InitVoiceEnabled(bool chatVoiceEnabled, bool talkVoiceEnabled)
        {
            IsChatVoiceEnabled = chatVoiceEnabled;
            IsTalkRealVoiceEnabled = talkVoiceEnabled;

            if (!IsChatVoiceEnabled && !IsTalkRealVoiceEnabled)
                return;

            SDKVoiceInterface.Instance.Init();
        }

        public void AddVoiceEventListener(ISDKVoiceCallback cb)
        {
            SDKVoiceInterface.Instance.Register(cb);
        }

        public void RemoveVoiceEventListener(ISDKVoiceCallback cb)
        {
            SDKVoiceInterface.Instance.Detach(cb);
        }

        private void _RemoveAllVoiceEventListener()
        {
            SDKVoiceInterface.Instance.DetachAll();
        }

        public void SetVoiceDebugLevel(SDKVoiceLogLevel level)
        {
            SDKVoiceInterface.Instance.SetLogLevel(level);
        }

        public void InvokeVoiceEvent(SDKVoiceEventArgs args)
        {
            SDKVoiceInterface.Instance.Invoke(args);
        }

        //重置实时语音部分参数
        public void ResetRealTalkVoiceParams()
        {
            float realPlayerVolumn = GetPlayerVolumnPref();
            SetPlayerVolume(realPlayerVolumn);

            ControlGameMusicVolumn(true);
        }

        public void CutGameVolumnInTalkVoice()
        {
            ControlGameMusicVolumn(true);
        }

        public void RecoverGameVolumnInTalkVoice()
        {
            ControlGameMusicVolumn(false);
        }

        public void ControlGameMusicMute(bool isMute)
        {
            if (isMute)
            {
                if (AudioManager.instance != null)
                    AudioManager.instance.SetMute(AudioType.AudioStream, isMute);
                //AudioManager.instance.SetMute(AudioType.AudioEffect, isMute);
                //AudioManager.instance.SetMute(AudioType.AudioVoice, isMute);
                Logger.LogProcessFormat("[语音播放，暂停系统声音]");
            }
            else
            {
                bool sMute = GameClient.SystemConfigManager.GetInstance().SystemConfigData.SoundConfig.Mute;
                //bool eMute = GameClient.SystemConfigManager.GetInstance().SystemConfigData.MusicConfig.Mute;
                if (AudioManager.instance != null)
                {
                    AudioManager.instance.SetMute(AudioType.AudioStream, sMute);
                    //AudioManager.instance.SetMute(AudioType.AudioEffect, eMute);
                    //AudioManager.instance.SetMute(AudioType.AudioVoice, eMute);
                    float sVolumn = (float)GameClient.SystemConfigManager.GetInstance().SystemConfigData.SoundConfig.Volume;
                    //float eVolumn = (float)GameClient.SystemConfigManager.GetInstance().SystemConfigData.MusicConfig.Volume;
                    AudioManager.instance.SetVolume(AudioType.AudioStream, sVolumn);
                    //AudioManager.instance.SetVolume(AudioType.AudioEffect, eVolumn);
                    //AudioManager.instance.SetVolume(AudioType.AudioVoice, eVolumn);
                    Logger.LogProcessFormat("[语音暂停，播放系统声音]");
                }
            }
        }

        void ControlGameMusicVolumn(bool beLower)
        {
            float sVolumn = (float)GameClient.SystemConfigManager.GetInstance().SystemConfigData.SoundConfig.Volume;
            //float eVolumn = (float)GameClient.SystemConfigManager.GetInstance().SystemConfigData.MusicConfig.Volume;
            if (AudioManager.instance == null) return;
            if (beLower)
            {
                AudioManager.instance.SetVolume(AudioType.AudioStream, sVolumn * 0.5f);
                //AudioManager.instance.SetVolume(AudioType.AudioEffect, eVolumn * 0.5f);
                //AudioManager.instance.SetVolume(AudioType.AudioVoice, eVolumn * 0.5f);
            }
            else
            {
                AudioManager.instance.SetVolume(AudioType.AudioStream, sVolumn);
                //AudioManager.instance.SetVolume(AudioType.AudioEffect, eVolumn * 0.5f);
                //AudioManager.instance.SetVolume(AudioType.AudioVoice, eVolumn * 0.5f);
            }
        }

        public void ControlRealVoiceMic()
        {
            bool isMicOn = IsTalkRealMicOn();
            if (isMicOn)
            {
                CloseRealMic();
            }
            else
            {
                OpenRealMic();
            }
        }

        public void ControlRealVociePlayer()
        {
            bool isPlayerOn = IsTalkRealPlayerOn();
            if (isPlayerOn)
            {
                CloseRealPlayer();
            }
            else
            {
                OpenRealPlayer();
            }
        }

        #endregion

        #region VoiceImpl Methods
        public void InitChatVoice()
        {
            if (!IsChatVoiceEnabled)
                return;
            SDKVoiceInterface.Instance.InitChatVoice();
        }

        /// <summary>
        /// 反初始化 强烈建议只调用一次 根据SDK需求
        /// </summary>
        public void UnInitChatVoice()
        {
            if (!IsChatVoiceEnabled)
                return;
            SDKVoiceInterface.Instance.UnInitChatVoice();
        }

        /// <summary>
        /// 登录语音SDK
        /// </summary>
        /// <param name="roleId">建议使用游戏账号（而非游戏角色ID）登录，因为大部分语音SDK根据DAU计费</param>
        /// <param name="openId">OpenId作为密码</param>
        /// <param name="token">暂时不使用</param>
        public void LoginVoice()
        {
            if (!IsChatVoiceEnabled)
                return;

            if (ClientApplication.playerinfo != null)
            {
                gameAccountId = ClientApplication.playerinfo.accid.ToString();
                gameRoleId = ClientApplication.playerinfo.roleinfo[ClientApplication.playerinfo.curSelectedRoleIdx].roleId;
                gameRoleIdStr = ClientApplication.playerinfo.roleinfo[ClientApplication.playerinfo.curSelectedRoleIdx].strRoleId;
                gameOpenId = ClientApplication.playerinfo.openuid;
                if (string.IsNullOrEmpty(gameOpenId))
                {
                    gameOpenId = "123456";
                }
                gameToken = ClientApplication.playerinfo.token;
                gameServerId = (int)ClientApplication.playerinfo.serverID;
                gameServerIdStr = gameServerId.ToString();
            }
            gameChannelId = PluginManager.GetInstance().GetChannelName();

            SDKVoiceGameAccInfo gameAccInfo = new SDKVoiceGameAccInfo(localPlayerVoiceId, gameOpenId, gameToken);

            LogoutVoice();
            SDKVoiceInterface.Instance.LoginVoice(gameAccInfo);
        }

        public void LogoutVoice()
        {
            if (!IsChatVoiceEnabled)
                return;

            SDKVoiceInterface.Instance.LogoutVoice();

            OnLogoutingVoice();
        }

        public void PlayVoice(string voiceKey, bool autoPlay)
        {
            if (!IsChatVoiceEnabled)
                return;

            //TODO
            SDKVoiceChatPlayInfo playInfo = new SDKVoiceChatPlayInfo();
            playInfo.voiceKey = voiceKey;
            playInfo.isAutoPlay = autoPlay;
            playInfo.targetUserId = _GetChatRoomId(VoiceRoomType.CT_PRIVATE);
            SDKVoiceInterface.Instance.PlayVoice(playInfo);
        }

        public void StopPlayVoice()
        {
            if (!IsChatVoiceEnabled)
                return;
            SDKVoiceInterface.Instance.StopPlayVoice();
        }

        public void SetVoiceVolume(float volume)
        {
            if (!IsChatVoiceEnabled)
                return;
            SDKVoiceInterface.Instance.SetVoiceVolume(volume);
        }

        public float GetVoiceVolume()
        {
            if (!IsChatVoiceEnabled)
                return 0f;
            return SDKVoiceInterface.Instance.GetVoiceVolume();
        }

        public void OnPause()
        {
            if (!IsChatVoiceEnabled)
                return;
            SDKVoiceInterface.Instance.OnChatPause();
        }

        public void OnResume()
        {
            if (!IsChatVoiceEnabled)
                return;
            SDKVoiceInterface.Instance.OnChatResume();
        }

        public bool IsVoiceRecording()
        {
            if (!IsChatVoiceEnabled)
                return false;

            return SDKVoiceInterface.Instance.IsVoiceRecording();
        }

        public bool IsVoicePlaying()
        {
            if (!IsChatVoiceEnabled)
                return false;
            return SDKVoiceInterface.Instance.IsVoicePlaying();
        }

        public void JoinChatRoom(VoiceRoomType roomType)
        {
            SDKVoiceChatRoomInfo roomInfo = voiceRoomInfos.SafeGetValue(roomType);
            if (null == roomInfo)
            {
                return;
            }
            _JoinChatRoom(roomInfo);
        }

        public void LeaveChatRoom(VoiceRoomType roomType)
        {
            SDKVoiceChatRoomInfo roomInfo = voiceRoomInfos.SafeGetValue(roomType);
            if (null == roomInfo)
            {
                return;
            }
            _LeaveChatRoom(roomInfo);
        }

        private void _JoinChatRoom(SDKVoiceChatRoomInfo roomInfo)
        {
            if (!IsChatVoiceEnabled)
                return;
            SDKVoiceInterface.Instance.JoinChatRoom(roomInfo);
        }

        private void _LeaveChatRoom(SDKVoiceChatRoomInfo roomInfo)
        {
            if (!IsChatVoiceEnabled)
                return;

            SDKVoiceInterface.Instance.LeaveChatRoom(roomInfo);
        }

        private void _LeaveAllChatRooms()
        {
            if (!IsChatVoiceEnabled)
                return;
            SDKVoiceInterface.Instance.LeaveAllChatRooms();
        }

        public void CancelRecordVoice()
        {
            if (!IsChatVoiceEnabled)
                return;
            SDKVoiceInterface.Instance.CancelRecordVoice();
        }

        public void StopRecordVoice()
        {
            if (!IsChatVoiceEnabled)
                return;
            string extra = ((int)currRoomType).ToString();
            string token = _GetVoiceToken(extra);
            SDKVoiceInterface.Instance.StopRecordVoice(token);
        }

        public void StartRecordVoice(VoiceRoomType roomType)
        {
            this.currRoomType = roomType;

            string recId = _GetChatRoomId(roomType);

            //TODO 配到功能开关表
            bool isTranslate = TR.Value("voice_sdk_be_translate").Equals("1") ? true : false;  //翻译开关

            if (!IsChatVoiceEnabled)
                return;

            SDKVoiceChatRecordInfo recordInfo = new SDKVoiceChatRecordInfo();
            recordInfo.receiveId = recId;
            recordInfo.isTranslate = isTranslate;
            SDKVoiceInterface.Instance.StartRecordVoice(recordInfo);
        }

        public bool IsLoginVoice()
        {
            if (!IsChatVoiceEnabled)
                return false;

            return SDKVoiceInterface.Instance.IsLoginVoice();
        }

        #region 聊天频道事件处理

        public void JoinPrivateChatRoom(ulong targetRoleId)
        {
            currSelectPlayerVoiceId = _GetPrivateChatRoomId(targetRoleId);
            _JoinPrivateRoom(currSelectPlayerVoiceId);
        }

        public void LeavePrivateChatRoom(ulong targetRoleId)
        {
            currSelectPlayerVoiceId = _GetPrivateChatRoomId(targetRoleId);
            _LeavePrivateRoom(false, currSelectPlayerVoiceId);
        }

        //其实是全部聊天频道的回调 主要为了私聊使用
        public void SetJoinedChatRoomId(string roomId)
        {
            if (string.IsNullOrEmpty(roomId))
            {
                return;
            }
            if (privateRoomIdList != null && !privateRoomIdList.Contains(roomId))
            {
                privateRoomIdList.Add(roomId);
            }
        }

        //其实是全部聊天频道的回调 主要为了私聊使用
        public void SetLeavedChatRoomId(string roomId)
        {
            if (string.IsNullOrEmpty(roomId))
            {
                return;
            }

            if (privateRoomIdList != null && privateRoomIdList.Contains(roomId))
            {
                privateRoomIdList.Remove(roomId);
            }
        }

        #region Game Params

        private static Dictionary<VoiceRoomType, SDKVoiceChatRoomInfo> voiceRoomInfos = new Dictionary<VoiceRoomType, SDKVoiceChatRoomInfo>()
        {
            { VoiceRoomType.CT_WORLD, new SDKVoiceChatRoomInfo(worldChatRoomId) },
            { VoiceRoomType.CT_NORMAL, new SDKVoiceChatRoomInfo(sceneChatRoomId) },
            { VoiceRoomType.CT_GUILD, new SDKVoiceChatRoomInfo(guildChatRoomId)},
            { VoiceRoomType.CT_TEAM, new SDKVoiceChatRoomInfo(teamChatRoomId)}
        };

        private static string worldChatRoomId
        {
            get
            {
                return string.Format("{0}_world", gameServerIdStr);
            }
        }

        private static string sceneChatRoomId
        {
            get
            {
                GameClient.ClientSystemTown town = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemTown;
                if (town != null)
                {
                    return string.Format("{0}_s_{1}", gameServerIdStr, town.CurrentSceneID);
                }
                return string.Empty;
            }
        }

        private static string guildChatRoomId
        {
            get
            {
                var guild = GameClient.GuildDataManager.GetInstance().myGuild;
                if (null != guild)
                {
                    return string.Format("{0}_g_{1}", gameServerIdStr, guild.uGUID);
                }
                return string.Empty;
            }
        }

        private static string teamChatRoomId
        {
            get
            {
                GameClient.Team myTeam = GameClient.TeamDataManager.GetInstance().GetMyTeam();
                if (null != myTeam)
                {
                    return string.Format("{0}_t_{1}", gameServerIdStr, myTeam.teamID);
                }
                return string.Empty;
            }
        }

        private string currSelectPlayerVoiceId;

        //当前语音播放本地构造的玩家id （按帐号id还是角色id或其他）
        private string _localPlayerVoiceId;
        public string localPlayerVoiceId
        {
            get
            {
                if (string.IsNullOrEmpty(_localPlayerVoiceId))
                {
                    _localPlayerVoiceId = _GetPlayerVoiceId(gameAccountId);
                }
                return _localPlayerVoiceId;
            }
        }

        //构造语音消息透传
        private string _GetVoiceToken(string extra)
        {
            return string.Format("{0}_{1}_{2}", localPlayerVoiceId, _GetServerTimeStamp(), extra);
        }

        private ulong _GetServerTimeStamp()
        {
            ulong serverTimeStamp = 0;
            try
            {
                serverTimeStamp = GameClient.TimeManager.GetInstance().GetServerTime();//(ulong)System.DateTime.Now.Ticks;
            }
            catch (System.Exception e)
            {
                Logger.LogError("get server timeStamp error" + e.ToString());
            }
            return serverTimeStamp;
        }

        private bool _IsPrivateChat(string voiceKey)
        {
            if (string.IsNullOrEmpty(voiceKey))
            {
                Logger.LogProcessFormat("GetAccidInVoiceKey voiceKey is null");
                return false;
            }

            string[] voiceKeyArr = voiceKey.Split('_');
            if (voiceKeyArr != null && voiceKeyArr.Length == 5)
            {
                if (string.IsNullOrEmpty(voiceKeyArr[4]))
                {
                    return false;
                }
                int typeInt = -1;
                if (int.TryParse(voiceKeyArr[4], out typeInt))
                {
                    if (typeInt < 0)
                    {
                        return false;
                    }
                    return (VoiceRoomType)typeInt == VoiceRoomType.CT_PRIVATE;
                }
            }
            return false;
        }

        private string _GetPlayerVoiceId(string accId)
        {
            return string.Format("{0}_{1}_{2}", gameServerIdStr, gameChannelId, accId);
        }

        private string _GetPrivateChatRoomId(ulong targetId)
        {
            ulong uId = gameRoleId;
            string privateVoiceId = targetId > uId ? string.Format("{0}_{1}", uId, targetId) : string.Format("{0}_{1}", targetId, uId);
            return string.Format("{0}_p_{1}_{2}", gameServerIdStr, gameChannelId, privateVoiceId);
        }

        private ulong _GetAccIdInVoiceKey(string voiceKey)
        {
            ulong accid = 0;
            if (string.IsNullOrEmpty(voiceKey))
            {
                Logger.LogProcessFormat("GetAccidInVoiceKey voiceKey is null");
                return accid;
            }

            string[] voiceKeyArr = voiceKey.Split('_');
            if (voiceKeyArr != null && voiceKeyArr.Length == 5)
            {
                if (string.IsNullOrEmpty(voiceKeyArr[2]))
                {
                    return accid;
                }
                if (ulong.TryParse(voiceKeyArr[2], out accid))
                {
                    return accid;
                }
            }
            return accid;
        }

        private string _GetChatRoomId(VoiceRoomType roomType)
        {
            switch (roomType)
            {
                case VoiceRoomType.CT_WORLD:
                    return worldChatRoomId;
                case VoiceRoomType.CT_TEAM:
                    return teamChatRoomId;
                case VoiceRoomType.CT_GUILD:
                    return guildChatRoomId;
                case VoiceRoomType.CT_NORMAL:
                    return sceneChatRoomId;
                case VoiceRoomType.CT_PRIVATE:
                    return currSelectPlayerVoiceId;
                default:
                    return "";
            }
        }

        #endregion

        #region VOICE CHAT ONLOGIN

        public void TryJoinChatRooms()
        {
            JoinChatRoom(VoiceRoomType.CT_WORLD);
            TryJoinGuildRoom();
            TryJoinTeamRoom();
            TryJoinSceneRoom();
            _JoinPrivateRoom(currSelectPlayerVoiceId);
        }

        public void TryReJoinCurrentRoom()
        {
            switch (currRoomType)
            {
                case VoiceRoomType.CT_WORLD:
                    JoinChatRoom(VoiceRoomType.CT_WORLD);
                    break;
                case VoiceRoomType.CT_TEAM:
                    TryJoinTeamRoom();
                    break;
                case VoiceRoomType.CT_GUILD:
                    TryJoinGuildRoom();
                    break;
                case GameClient.ChatType.CT_NORMAL:
                    TryJoinSceneRoom();
                    break;
                case VoiceRoomType.CT_PRIVATE:
                    _JoinPrivateRoom(currSelectPlayerVoiceId);
                    break;
            }
        }

        void TryJoinGuildRoom()
        {
            bool hasGuild = GameClient.GuildDataManager.GetInstance().myGuild != null ? true : false;
            if (hasGuild)
            {
                JoinChatRoom(VoiceRoomType.CT_GUILD);
            }
        }

        void TryJoinTeamRoom()
        {
            bool hasTeam = GameClient.TeamDataManager.GetInstance().HasTeam();
            if (hasTeam)
            {
                JoinChatRoom(VoiceRoomType.CT_TEAM);
            }
        }

        void TryJoinSceneRoom()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown != null)
            {
                ProtoTable.CitySceneTable sceneData = TableManager.GetInstance().GetTableItem<ProtoTable.CitySceneTable>(systemTown.CurrentSceneID);
                if (sceneData == null)
                {
                    return;
                }
                // 城镇与城镇之间的切场景,从单局退出到城镇,以及从登陆进入到城镇也都会触发的
                //LeaveSceneChannel();
                //Logger.LogErrorFormat("sceneData type {0}", sceneData.SceneType.ToString());
                if (sceneData.SceneType == ProtoTable.CitySceneTable.eSceneType.NORMAL || sceneData.SceneType == ProtoTable.CitySceneTable.eSceneType.PK_PREPARE)
                {
                    JoinChatRoom(VoiceRoomType.CT_NORMAL);
                }
            }
        }

        #endregion

        //私聊频道 但是加入的频道id包括了所有的频道 只是离开时从里面判断对应的私聊频道还在不在
        private List<string> privateRoomIdList = new List<string>();
        private void _JoinPrivateRoom(string privateChannelId)
        {
            if (privateRoomIdList == null)
            {
                return;
            }
            if (privateRoomIdList.Contains(privateChannelId))
            {
                //Logger.LogError("JoinPrivateChannel - has already private room id :" + privateChannelId);
                return;
            }
            SDKVoiceChatRoomInfo roomInfo = new SDKVoiceChatRoomInfo(privateChannelId);
            _JoinChatRoom(roomInfo);
        }

        private void _LeavePrivateRoom(bool beLeaveAll = false, string pChannelId = "")
        {
            if (privateRoomIdList == null)
            {
                return;
            }
            SDKVoiceChatRoomInfo roomInfo = null;
            if (beLeaveAll == false && string.IsNullOrEmpty(pChannelId) == false)
            {
                if (privateRoomIdList.Contains(pChannelId))
                {
                    roomInfo = new SDKVoiceChatRoomInfo(pChannelId);
                    _LeaveChatRoom(roomInfo);
                }
            }
            else if(beLeaveAll)
            {
                for (int i = 0; i < privateRoomIdList.Count; i++)
                {
                   roomInfo = new SDKVoiceChatRoomInfo(privateRoomIdList[i]);
                   _LeaveChatRoom(roomInfo);
                }
            }
        }

        public void LeaveVoiceSDK(bool beLogout = false)
        {
            _LeaveAllChatRooms();
            if (beLogout)
            {
                LogoutVoice();
            }
            else
            {
                OnLogoutingVoice();
            }
            LeaveAllTalkChannels();
            ComVoiceTalk.ForceDestroy();
        }

        void OnLogoutingVoice()
        {
            gameAccountId = string.Empty;
            gameRoleIdStr = string.Empty;
            gameRoleId = 0;
            gameOpenId = string.Empty;
            gameToken = string.Empty;
            gameServerId = 0;
            gameServerIdStr = string.Empty;
            gameChannelId = string.Empty;
            
			_voiceChatModule = null;
			currSelectPlayerVoiceId = string.Empty;
			_localPlayerVoiceId = string.Empty;
			privateRoomIdList.Clear();
        }

        #endregion 聊天频道事件处理

        #endregion

        #region Talk Voice Methods
        public void InitTalkVoice()
        {
            if (!IsTalkRealVoiceEnabled)
                return;
            SDKVoiceInterface.Instance.InitTalkVoice();
        }

        /// <summary>
        /// 反初始化 强烈建议只调用一次 根据SDK需求
        /// </summary>
        public void UnInitTalkVoice()
        {
            if (!IsTalkRealVoiceEnabled)
                return;
            SDKVoiceInterface.Instance.UnInitTalkVoice();
        }

        //生成实时语音频道ID
        private string _GenerateTalkChannelId(int sceneId)
        {
            string channelId = "";
            int idLength = sceneId.ToString().Length;
            if (idLength > 0)
            {
                channelId = (gameServerId * System.Math.Pow(10, idLength) + sceneId).ToString();
            }
            return channelId;
        }

        private string _GetTalkGameSceneId(string talkChannelId)
        {
            if (string.IsNullOrEmpty(talkChannelId))
            {
                return "";
            }
            int idLength = talkChannelId.Length;
            int cId, sceneId = 0;
            if (int.TryParse(talkChannelId, out cId))
            {
                sceneId = cId % (int)System.Math.Pow(10, idLength);
            }
            else
            {
                return talkChannelId;
            }
            return sceneId.ToString();
        }
        private string _TryGetTalkChannelId(string sIdStr)
        {
            if (string.IsNullOrEmpty(sIdStr))
            {
                return "";
            }
            int sId;
            string cId;
            if (int.TryParse(sIdStr, out sId))
            {
                cId = _GenerateTalkChannelId(sId);
            }
            else
            {
                cId = sIdStr;
            }
            return cId;
        }

        //当前是否在实时语音频道
        public bool IsInVoiceTalkChannel()
        {
            if (!IsTalkRealVoiceEnabled)
                return false;
            return SDKVoiceInterface.Instance.IsInVoiceTalkChannel();
        }

        /// <summary>
        /// 加入实时语音频道
        /// </summary>
        /// <param name="sceneId">场景ID，还需要组装成频道ID</param>
        public void JoinTalkChannel(string sceneId)
        {
            if (!IsTalkRealVoiceEnabled)
                return;
            int sId;
            if (int.TryParse(sceneId, out sId))
            {
                JoinTalkChannel(sId);
            }
            else
            {
                _JoinTalkChannel(sceneId);
            }
        }

        /// <summary>
        /// 加入实时语音频道
        /// </summary>
        /// <param name="sceneId">场景ID，还需要组装成频道ID</param>
        public void JoinTalkChannel(int sceneId)
        {
            if (!IsTalkRealVoiceEnabled)
                return;
            string channelId = _GenerateTalkChannelId(sceneId);
            if (!string.IsNullOrEmpty(channelId))
            {
                SDKVoiceInterface.Instance.JoinTalkChannel(channelId);
            }
        }

        private void _JoinTalkChannel(string channelId)
        {
            if (!IsTalkRealVoiceEnabled)
                return;
            string cId = _TryGetTalkChannelId(channelId);
            if (!string.IsNullOrEmpty(cId))
            {
                SDKVoiceInterface.Instance.JoinTalkChannel(cId);
            }
        }

        public void LeaveAllTalkChannels()
        {
            if (!IsTalkRealVoiceEnabled)
                return;
            SDKVoiceInterface.Instance.LeaveAllTalkChannels();
        }

        public void LeaveTalkChannel(string sceneId)
        {
            if (!IsTalkRealVoiceEnabled)
                return;
            int sId;
            if (int.TryParse(sceneId, out sId))
            {
                LeaveTalkChannel(sId);
            }
            else
            {
                _LeaveTalkChannel(sceneId);
            }
        }

        public void LeaveTalkChannel(int sceneId)
        {
            if (!IsTalkRealVoiceEnabled)
                return;
            string channelId = _GenerateTalkChannelId(sceneId);
            if (!string.IsNullOrEmpty(channelId))
            {
                SDKVoiceInterface.Instance.LeaveTalkChannel(channelId);
            }
        }

        private void _LeaveTalkChannel(string channelId)
        {
            if (!IsTalkRealVoiceEnabled)
                return;
            string cId = _TryGetTalkChannelId(channelId);
            if (!string.IsNullOrEmpty(cId))
            {
                SDKVoiceInterface.Instance.LeaveTalkChannel(cId);
            }
        }

        //从talk channel id中获取 scene id
        public string GetCurrentTalkChanneld()
        {
            string talkChannelId = string.Empty;
            talkChannelId = SDKVoiceInterface.Instance.CurrentTalkChannelId();
            return _GetTalkGameSceneId(talkChannelId);
        }

        public void SwitchTalkChannel(string sceneId)
        {
            if (!IsTalkRealVoiceEnabled)
                return;
            int sId;
            if (int.TryParse(sceneId, out sId))
            {
                SwitchTalkChannel(sId);
            }
            else
            {
                _SwitchTalkChannel(sceneId);
            }
        }

        public void SwitchTalkChannel(int sceneId)
        {
            if (!IsTalkRealVoiceEnabled)
                return;
            string channelId = _GenerateTalkChannelId(sceneId);
            if (!string.IsNullOrEmpty(channelId))
            {
                SDKVoiceInterface.Instance.SetCurrentTalkChannelId(channelId);
            }
        }

        private void _SwitchTalkChannel(string channelId)
        {
            if (!IsTalkRealVoiceEnabled)
                return;
            if (string.IsNullOrEmpty(channelId))
                return;
            string cId = _TryGetTalkChannelId(channelId);
            if (!string.IsNullOrEmpty(cId))
            {
                SDKVoiceInterface.Instance.SetCurrentTalkChannelId(cId);
            }
        }

        public void UpdateTalkChannel(List<string> sceneIds)
        {
            if (sceneIds == null)
            {
                return;
            }
            List<string> channelIds = new List<string>();
            for (int i = 0; i < sceneIds.Count; i++)
            {
                string sIdStr = sceneIds[i];
                string cId = _TryGetTalkChannelId(sIdStr);
                if (string.IsNullOrEmpty(cId))
                    continue;
                if (!channelIds.Contains(cId))
                {
                    channelIds.Add(cId);
                }
            }
            SDKVoiceInterface.Instance.UpdateTalkChannel(channelIds);
        }

        public bool IsJoinedTalkChannel(int sceneId)
        {
            if (!IsTalkRealVoiceEnabled)
                return false;
            string channelId = _GenerateTalkChannelId(sceneId);
            return SDKVoiceInterface.Instance.IsJoinedTalkChannel(channelId);
        }

        public bool IsJoinedTalkChannel(string sceneId)
        {
            if (!IsTalkRealVoiceEnabled)
                return false;
            string cId = _TryGetTalkChannelId(sceneId);
            if (!string.IsNullOrEmpty(cId))
            {
                return SDKVoiceInterface.Instance.IsJoinedTalkChannel(cId);
            }
            return false;
        }

        public bool HasJoinedTalkChannel()
        {
            if (!IsTalkRealVoiceEnabled)
                return false;
            return SDKVoiceInterface.Instance.HasJoinedTalkChannel();
        }

        public string GetOtherTalkChannelId(string gameAccId)
        {
            if (!IsTalkRealVoiceEnabled)
                return "";
            string voicePlayerId = _GetPlayerVoiceId(gameAccId);
            if (string.IsNullOrEmpty(voicePlayerId))
                return "";
            return SDKVoiceInterface.Instance.GetOtherTalkChannelId(voicePlayerId);
        }

        public void ControlGlobalSilence(string mainChannelIdStr)
        {
            if (!IsTalkRealVoiceEnabled)
                return;
            if (!IsRecordVoiceEnabled)
                return;
            if (string.IsNullOrEmpty(mainChannelIdStr))
            {
                return;
            }
            if (IsGlobalSilence())
            {
                SetGlobalSilence(mainChannelIdStr, false);
            }
            else
            {
                SetGlobalSilence(mainChannelIdStr, true);
            }
        }

        //自己是否被禁言 可能是团长 不会被禁言
        public bool IsMicEnable()
        {
            return SDKVoiceInterface.Instance.IsMicEnable();
        }

        //是否开启禁言
        public bool IsGlobalSilence()
        {
            return SDKVoiceInterface.Instance.IsGlobalSilence();
        }

        public void SetGlobalSilence(string mainChannelId, bool isNotSpeak)
        {
            if (!IsTalkRealVoiceEnabled)
                return;

            if (!IsRecordVoiceEnabled)
                return;
            string cId = _TryGetTalkChannelId(mainChannelId);
            if (string.IsNullOrEmpty(cId))
            {
                return;
            }
            SDKVoiceInterface.Instance.SetGlobalSilenceInMainChannel(cId, isNotSpeak);
        }

        public string GetGameAccIdByVoicePlayerId(string voicePlayerId)
        {
            string gameAccId = voicePlayerId;
            if (string.IsNullOrEmpty(voicePlayerId))
            {
                return gameAccId;
            }
            string[] idSplitArray = gameAccId.Split('_');
            if (idSplitArray != null && idSplitArray.Length >= 3)
            {
                gameAccId = idSplitArray[2];
            }
            return gameAccId;
        }

        public void SetMicEnable(string gameAccId, bool bEnable)
        {
            if (!IsTalkRealVoiceEnabled)
                return;
            if (!IsRecordVoiceEnabled)
                return;
            string voicePlayerId = _GetPlayerVoiceId(gameAccId);
            SDKVoiceInterface.Instance.SetMicEnable(voicePlayerId, bEnable);
        }

        public void OpenRealMic()
        {
            if (!IsTalkRealVoiceEnabled)
                return;

            if (!IsRecordVoiceEnabled)
                return;

            SDKVoiceInterface.Instance.OpenRealMic();
        }

        public void CloseRealMic()
        {
            if (!IsTalkRealVoiceEnabled)
                return;

            if (!IsRecordVoiceEnabled)
                return;

            SDKVoiceInterface.Instance.CloseRealMic();
        }

        public void OpenRealPlayer()
        {
            if (!IsTalkRealVoiceEnabled)
                return;

            if (!IsPlayVoiceEnabled)
                return;

            SDKVoiceInterface.Instance.OpenRealPlayer();
        }

        public void CloseRealPlayer()
        {
            if (!IsTalkRealVoiceEnabled)
                return;

            if (!IsPlayVoiceEnabled)
                return;

            SDKVoiceInterface.Instance.CloseReaPlayer();
        }

        public bool IsTalkRealMicOn()
        {
            if (!IsTalkRealVoiceEnabled)
                return false;

            return SDKVoiceInterface.Instance.IsTalkRealMicOn();
        }

        public bool IsTalkRealPlayerOn()
        {
            if (!IsTalkRealVoiceEnabled)
                return false;
            return SDKVoiceInterface.Instance.IsTalkRealPlayerOn();
        }

        // 设置当前程序输出音量大小。建议该状态值在加入房间成功后按需再重置一次!!!
        public void SetPlayerVolume(float volume)
        {
            if (!IsTalkRealVoiceEnabled)
                return;
            SDKVoiceInterface.Instance.SetPlayerVolume(volume);
        }

        public float GetPlayerVolume()
        {
            if (!IsTalkRealVoiceEnabled)
                return 0f;
            return SDKVoiceInterface.Instance.GetPlayerVolume();
        }

        public void PauseChannel()
        {
            if (!IsTalkRealVoiceEnabled)
                return;
            SDKVoiceInterface.Instance.PauseTalkChannel();
        }

        public void ResumeChannel()
        {
            if (!IsTalkRealVoiceEnabled)
                return;
            SDKVoiceInterface.Instance.ResumeTalkChannel();
        }

        #region Voice Params in Local

        public void SaveWorldAutoPlayValue(bool world)
        {
            PlayerPrefs.SetInt(VOICE_AUTO_PLAY_WORLD, world ? 1 : 0);
            PlayerPrefs.Save();
        }
        public void SaveTeamAutoPlayValue(bool team)
        {
            PlayerPrefs.SetInt(VOICE_AUTO_PLAY_TEAM, team ? 1 : 0);
            PlayerPrefs.Save();
        }
        public void SaveGuildAutoPlayValue(bool guild)
        {
            PlayerPrefs.SetInt(VOICE_AUTO_PLAY_GUILD, guild ? 1 : 0);
            PlayerPrefs.Save();
        }
        public void SaveNearbyAutoPlayValue(bool nearby)
        {
            PlayerPrefs.SetInt(VOICE_AUTO_PLAY_NEARBY, nearby ? 1 : 0);
            PlayerPrefs.Save();
        }
        public void SavePrivateAutoPlayValue(bool _private)
        {
            PlayerPrefs.SetInt(VOICE_AUTO_PLAY_PRIVATE, _private ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void SaveMicPref(bool isOn)
        {
            int value = isOn ? 1 : 0;
            PlayerPrefs.SetInt(VOICE_MIC_SETTING_KEY, value);
            PlayerPrefs.Save();
        }

        public void SavePlayerPref(bool isOn)
        {
            int value = isOn ? 1 : 0;
            PlayerPrefs.SetInt(VOICE_PLAYER_SETTING_KEY, value);
            PlayerPrefs.Save();
        }

        public void SavePlayerVolumnPref(float volumn)
        {
            PlayerPrefs.SetFloat(VOICE_PLAYER_VOLUMN, volumn);
            PlayerPrefs.Save();
        }

        #endregion

        #endregion

        #region PRIVATE METHOD

        #region Voice Params in Local

        bool GetWorldAutoPlayValue()
        {
            if (PlayerPrefs.HasKey(VOICE_AUTO_PLAY_WORLD))
                return PlayerPrefs.GetInt(VOICE_AUTO_PLAY_WORLD) == 1 ? true : false;
            else
                return false;
        }

        bool GetTeamAutoPlayValue()
        {
            if (PlayerPrefs.HasKey(VOICE_AUTO_PLAY_TEAM))
                return PlayerPrefs.GetInt(VOICE_AUTO_PLAY_TEAM) == 1 ? true : false;
            else
                return false;
        }

        bool GetGuildAutoPlayValue()
        {
            if (PlayerPrefs.HasKey(VOICE_AUTO_PLAY_GUILD))
                return PlayerPrefs.GetInt(VOICE_AUTO_PLAY_GUILD) == 1 ? true : false;
            else
                return false;
        }

        bool GetNearbyAutoPlayValue()
        {
            if (PlayerPrefs.HasKey(VOICE_AUTO_PLAY_NEARBY))
                return PlayerPrefs.GetInt(VOICE_AUTO_PLAY_NEARBY) == 1 ? true : false;
            else
                return false;
        }

        bool GetPrivateAutoPlayValue()
        {
            if (PlayerPrefs.HasKey(VOICE_AUTO_PLAY_PRIVATE))
                return PlayerPrefs.GetInt(VOICE_AUTO_PLAY_PRIVATE) == 1 ? true : false;
            else
                return false;
        }
        bool GetMicPref()
        {
            if (PlayerPrefs.HasKey(VOICE_MIC_SETTING_KEY))
                return PlayerPrefs.GetInt(VOICE_MIC_SETTING_KEY) == 1 ? true : false;
            else
                return true;
        }
        bool GetPlayerPref()
        {
            if (PlayerPrefs.HasKey(VOICE_PLAYER_SETTING_KEY))
                return PlayerPrefs.GetInt(VOICE_PLAYER_SETTING_KEY) == 1 ? true : false;
            else
                return true;
        }

        float GetPlayerVolumnPref()
        {
            if (PlayerPrefs.HasKey(VOICE_PLAYER_VOLUMN))
                return PlayerPrefs.GetFloat(VOICE_PLAYER_VOLUMN);
            else
                return GetPlayerVolume();
        }

        #endregion

        #endregion

        #region Report voice sdk using 

        public void ReportUsingVoice(SDKVoiceEventType eventType, string param = "")
        {
            CustomLogReportType voiceType = CustomLogReportType.CLRT_INVALID;
            switch (eventType)
            {
                case SDKVoiceEventType.ChatSendEndReport:
                    voiceType = CustomLogReportType.CLRT_SEND_RECORD_VOICE;
                    break;
                case SDKVoiceEventType.ChatDownloadRecordVoiceReport:
                    voiceType = CustomLogReportType.CLRT_LOAD_RECORD_VOICE;
                    break;
                case SDKVoiceEventType.TalkJoinChannelSuccReport:
                    voiceType = CustomLogReportType.CLRT_JOIN_VOICE_ROOM;
                    break;
                case SDKVoiceEventType.TalkLeaveChannelSuccReport:
                    voiceType = CustomLogReportType.CLRT_QUIT_VOICE_ROOM;
                    break;
            }
            if (voiceType == CustomLogReportType.CLRT_INVALID ||
                string.IsNullOrEmpty(param))
            {
                return;
            }
            SceneCustomLogReport voicesdkDataReport = new SceneCustomLogReport();
            voicesdkDataReport.type = (byte)voiceType;
            voicesdkDataReport.param = param;
            Network.NetManager.Instance().SendCommand(Network.ServerType.GATE_SERVER, voicesdkDataReport);

            Logger.LogProcessFormat("[SDKVoiceManager] - ReportUsingVoice, voiceType = {0} , param = {1}", voiceType.ToString(), param);
        }

        #endregion

        #endregion
    }
}