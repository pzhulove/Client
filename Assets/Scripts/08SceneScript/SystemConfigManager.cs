using System;
using System.Xml;
using UnityEngine;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using Network;
using Protocol;

namespace GameClient
{
    public class ChatConfig
    {
        public bool Around { get; set; }
        public bool Private { get; set; }
        public bool World { get; set; }
        public bool System { get; set; }
        public bool Guild { get; set; }
        public bool Team { get; set; }
        public bool Accompany { get; set; }
    }

    public class SoundConfig
    {
        public SoundConfig(float volume,bool mute)
        {
            fVolume = volume;
            bMute = mute;
        }

        public SoundConfig()
        {
            fVolume = 0.55;
            bMute = false;
        }

        public double Volume { get { return fVolume; } set { fVolume = value; } }
        public bool Mute { get { return bMute; } set { bMute = value; } }

        double fVolume = 0.55;
        bool bMute = false;
    }

    public class SystemConfigData
    {
        public SoundConfig SoundConfig { get { return kSoundConfig; } set { kSoundConfig = value; } }
        public SoundConfig MusicConfig { get { return kMusicConfig; } set { kMusicConfig = value; } }
        public SoundConfig EnvironmentMusicConfig { get {return kEnvironmentMusicConfig;} set {kEnvironmentMusicConfig = value;}}
        public ChatConfig ChatConfig { get { return kChatConfig; } set { kChatConfig = value; } }

        SoundConfig kSoundConfig = new SoundConfig(0.55f,false);
        SoundConfig kMusicConfig = new SoundConfig(0.86f, false);
        SoundConfig kEnvironmentMusicConfig = new SoundConfig(1.0f, false);
        ChatConfig kChatConfig = new ChatConfig();
    }

    class SystemConfigManager : DataManager<SystemConfigManager>
    {
        public delegate void OnChatFilterChanged(List<bool> chatTypes);
        public OnChatFilterChanged onChatFilterChanged;
        public override void Initialize()
        {
            InviteFriendLvLimit = 0;
            MaskGuildInvite = false;
            MaskTeamInvite = false;
            MaskPkInvite = false;

            _BindNetMessage();
        }

        string m_kSavePath = "systemConfig.json";

        public override void OnApplicationStart()
        {
            string jsonText = null;
            FileArchiveAccessor.LoadFileInPersistentFileArchive(m_kSavePath,out jsonText);
            if(string.IsNullOrEmpty(jsonText))
            {
                //Logger.LogErrorFormat("read systemConfig failed !");
                return;
            }

            m_kSystemConfig = LitJson.JsonMapper.ToObject<SystemConfigData>(jsonText);
            if(null == m_kSystemConfig)
            {
             //   Logger.LogErrorFormat("read systemConfig failed ! ToObject m_kSystemConfig is null");
                return;
            }

            AudioManager.instance.SetVolume(AudioType.AudioStream, (float)m_kSystemConfig.SoundConfig.Volume);
            AudioManager.instance.SetVolume(AudioType.AudioEffect, (float)m_kSystemConfig.MusicConfig.Volume);
            AudioManager.instance.SetVolume(AudioType.AudioVoice, (float)m_kSystemConfig.MusicConfig.Volume);
            AudioManager.instance.SetVolume(AudioType.AudioEnvironment, (float)m_kSystemConfig.EnvironmentMusicConfig.Volume);

            AudioManager.instance.SetMute(AudioType.AudioStream, m_kSystemConfig.SoundConfig.Mute);
            AudioManager.instance.SetMute(AudioType.AudioEffect, m_kSystemConfig.MusicConfig.Mute);
            AudioManager.instance.SetMute(AudioType.AudioVoice, m_kSystemConfig.MusicConfig.Mute);
            AudioManager.instance.SetMute(AudioType.AudioEnvironment, m_kSystemConfig.EnvironmentMusicConfig.Mute);

            NpcVoiceCachedManager.instance.SetVolume((float)m_kSystemConfig.MusicConfig.Volume);

            _InitChatDatas();
        }

        public override void Clear()
        {
            InviteFriendLvLimit = 0;
            MaskGuildInvite = false;
            MaskTeamInvite = false;
            MaskPkInvite = false;

            _UnBindNetMessage();
        }

        #region chatDatas
        List<bool> m_akChatToggle = new List<bool>();
        public List<bool> ChatFilters
        {
            get { return m_akChatToggle; }
        }

        void _InitChatDatas()
        {
            m_akChatToggle.Clear();
            for (int i = 0; i < (int)ChatType.CT_MAX_WORDS; ++i)
            {
                m_akChatToggle.Add(false);
            }

            var chatConfig = SystemConfigData.ChatConfig;
            m_akChatToggle[(int)ChatType.CT_NORMAL] = true;// chatConfig.Around;
            m_akChatToggle[(int)ChatType.CT_WORLD] = true;//chatConfig.World;
            m_akChatToggle[(int)ChatType.CT_SYSTEM] = true;//chatConfig.System;
            m_akChatToggle[(int)ChatType.CT_PRIVATE] = true;//chatConfig.Private;
            m_akChatToggle[(int)ChatType.CT_GUILD] = true;//chatConfig.Guild;
            m_akChatToggle[(int)ChatType.CT_TEAM] = true;//chatConfig.Team;
            m_akChatToggle[(int)ChatType.CT_ACOMMPANY] = true;//chatConfig.Accompany;
        }

        public void SetChatToggle(ChatType eChatType, bool bValue)
        {
            m_akChatToggle[(int)eChatType] = bValue;
            var chatConfig = SystemConfigManager.GetInstance().SystemConfigData.ChatConfig;
            switch (eChatType)
            {
                case ChatType.CT_PRIVATE:
                    {
                        chatConfig.Private = bValue;
                        break;
                    }
                case ChatType.CT_NORMAL:
                    {
                        chatConfig.Around = bValue;
                        break;
                    }
                case ChatType.CT_WORLD:
                    {
                        chatConfig.World = bValue;
                        break;
                    }
                case ChatType.CT_SYSTEM:
                    {
                        chatConfig.System = bValue;
                        break;
                    }
                case ChatType.CT_GUILD:
                    {
                        chatConfig.Guild = bValue;
                        break;
                    }
                case ChatType.CT_TEAM:
                    {
                        chatConfig.Team = bValue;
                        break;
                    }
                case ChatType.CT_ACOMMPANY:
                    {
                        chatConfig.Accompany = bValue;
                        break;
                    }
            }

            if (onChatFilterChanged != null)
            {
                onChatFilterChanged(m_akChatToggle);
            }
        }

        public bool IsChatToggleOn(ChatType eChatType)
        {
            var chatConfig = SystemConfigData.ChatConfig;
            switch (eChatType)
            {
                case ChatType.CT_PRIVATE:
                    {
                        return chatConfig.Private;
                    }
                case ChatType.CT_NORMAL:
                    {
                        return chatConfig.Around;
                    }
                case ChatType.CT_WORLD:
                    {
                        return chatConfig.World;
                    }
                case ChatType.CT_SYSTEM:
                    {
                        return chatConfig.System;
                    }
                case ChatType.CT_GUILD:
                    {
                        return chatConfig.Guild;
                    }
                case ChatType.CT_TEAM:
                    {
                        return chatConfig.Team;
                    }
                case ChatType.CT_ACOMMPANY:
                    {
                        return chatConfig.Accompany;
                    }
            }
            return false;
        }
        #endregion

        public override void OnApplicationQuit()
        {
            SaveConfig();
        }

        void _BindNetMessage()
        {
        }

        /// <summary>
        /// 解绑网络消息
        /// </summary>
        void _UnBindNetMessage()
        {
        }
        public void SaveConfig()
        {
            try
            {
                var jsonText = LitJson.JsonMapper.ToJson(SystemConfigData);
                if(!string.IsNullOrEmpty(jsonText))
                {
                    FileArchiveAccessor.SaveFileInPersistentFileArchive(m_kSavePath, jsonText);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        SystemConfigData m_kSystemConfig = new SystemConfigData();
        public SystemConfigData SystemConfigData
        {
            get { return m_kSystemConfig; }
            set { m_kSystemConfig = value; }
        }
        public void SendSceneGameSetReq(GameSetType gameSetType,uint setValue)
        {
            SceneGameSetReq req = new SceneGameSetReq();
            req.gameSetType = (uint)gameSetType;
            req.setValue = setValue.ToString();
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }
        public void ParseGameSet(string gameSetValue)
        {
            if(string.IsNullOrEmpty(gameSetValue))
            {
                return;
            }
            string[] reward = gameSetValue.Split('|');
            for(int i = 0;i < reward.Length;i++)
            {
                string[] sets = reward[i].Split('_');
                if(sets.Length >= 2)
                {
                    int iType = 0;
                    int.TryParse(sets[0], out iType);
                    if(iType == (int)Protocol.GameSetType.GST_FRIEND_INVATE)
                    {
                        int iValue = 0;
                        int.TryParse(sets[1], out iValue);
                        InviteFriendLvLimit = iValue;
                    }
                    else if(iType == (int)Protocol.GameSetType.GST_SECRET)
                    {
                        int iValue = 0;
                        int.TryParse(sets[1], out iValue);
                        MaskGuildInvite = (iValue & (int)Protocol.SecretSetType.SST_GUILD_INVATE) == (int)SecretSetType.SST_GUILD_INVATE;
                        MaskTeamInvite = (iValue & (int)Protocol.SecretSetType.SST_TEAM_INVATE) == (int)SecretSetType.SST_TEAM_INVATE;
                        MaskPkInvite = (iValue & (int)Protocol.SecretSetType.SST_DUEL_INVATE) == (int)SecretSetType.SST_DUEL_INVATE;
                    }
                }
            }
            return;
        }

        // 游戏设置数据(通过协议交互修改)
        #region game set value
        public int InviteFriendLvLimit
        {
            private set;
            get;
        }
        public bool MaskGuildInvite
        {
            private set;
            get;
        }
        public bool MaskTeamInvite
        {
            private set;
            get;
        }
        public bool MaskPkInvite
        {
            private set;
            get;
        }
        #endregion
    }
}