using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEngine.UI;

namespace GameClient
{
    public class PlayerPrefsManager : Singleton<PlayerPrefsManager>
    {
        /*
         *   以下是系统内用到的 Key
         *   记录截止时间 : 2019年5月8日
         *   
        //SDK Voice
        public const string VOICE_MIC_SETTING_KEY = "SDKVoiceMicON";
        public const string VOICE_PLAYER_SETTING_KEY = "SDKVoicePlayerON";
        public const string VOICE_AUTO_PLAY_WORLD = "SDKVoiceAutoPlayWorld";
        public const string VOICE_AUTO_PLAY_TEAM = "SDKVoiceAutoPlayTeam";
        public const string VOICE_AUTO_PLAY_GUILD = "SDKVoiceAutoPlayGuild";
        public const string VOICE_AUTO_PLAY_NEARBY = "SDKVoiceAutoPlayNearby";
        public const string VOICE_AUTO_PLAY_PRIVATE = "SDKVoiceAutoPlayPrivate";
        public const string VOICE_PLAYER_VOLUMN = "SDKVoicePlayerVolumn";

        //Chapter First Finish
        public const string CHAPTER_HAS_PRESTORY_AND_ONCE_FINISH_BOSS = "{0}-{1}-{2}-TheFirstTimeFinishBoss";

        //Charpter Buff Drug Toggle On
        public const string CHAPTER_BUFF_DRUG_TOGGLE_ON = "{0}-{1}-ChapterBuffDrugToggleIsOn";

        //AdventureTeam Expedition
        public const string ADVENTURE_TEAM_EXPEDITION_MAPID_TIME_SETTING = "{0}-{1}-{2}-ExpedtionMapIdTime";

        //Week Sign in
        public const string WEEK_SIGN_IN_ACTIVITY = "ActivityWeekSignIn";
        public const string WEEK_SIGN_IN_NEW_PLAYER = "NewPlayerWeekSignIn";

        //Financial Plan
        public const string FINANCIAL_PLAN_STR = "FinancialPlan";

        //Month Card Tip
        public const string MONTH_CARD_TIP_STR = "HasShowMonthCardTip_{0}";

        //Vibrate settings
        public const string VIBRATE_SETTINGS_STR = "{0}_{1}_{2}_VibrateSettings";

        //Chat Manager
        public const string PRIVATE_CHAT_LIMIT_LEVEL = "PrivateChatLimitLevel";

        */

        public enum PlayerPrefsKeyType
        {
            None = 0,

            MonthCardRewardRedPointUpdateTime,

            DailyTodoFunctionRefreshState,

            DailyTodoFunctionEndStateTime,

            DailyTodoFunctionWeekFinishTime,

            CurrencyDeadLineTipsTime,

            ATPassBlessCheckTime,
            
            ATWeeklyTaskCheckTime,

            TapRedPointCheck,

            TeamDuplicationVoiceTalkMicOn, //团本语音配置 - 麦开启状态

            TeamDuplicationVoiceTalkPlayerOn, //团本语音配置 - 扬声器开启状态
            AdventurePassCardEndeTip,//冒险通行证结束提示
            AdventurePassCardOpenSeasonId,//冒险通行证赛季第一次打开
            Count
        }

        //public enum PlayerPrefsValueType
        //{
        //    Int32,
        //    Single,
        //    String
        //}
       
        private static readonly string format_keyType_serverId_roleId = "{0}_{1}_{2}";
        private static readonly string format_keyType_accId = "{0}_{1}";

        #region METHODS using UnityEngine

        public string GetType_ServerId_RoleId_Key(PlayerPrefsKeyType keyType, params object[] extraStr)
        {
            if (keyType == PlayerPrefsKeyType.None || keyType == PlayerPrefsKeyType.Count)
            {
                return "";
            }
            string serverId = "";
            string roleId = "";
            if (null != ClientApplication.playerinfo)
            {
                serverId = ClientApplication.playerinfo.serverID.ToString();
            }
            if (null != PlayerBaseData.GetInstance())
            {
                roleId = PlayerBaseData.GetInstance().RoleID.ToString();
            }

            //Type _type = Type.GetType(String.Format("System.{0}", valueType.ToString()));
            if (extraStr == null || extraStr.Length <= 0)
            {
                return string.Format(format_keyType_serverId_roleId, keyType.ToString(), serverId, roleId);
            }
            else
            {
                string strFormat = "{0}";
                string oldKeyType = string.Format(strFormat, keyType.ToString());
                for (int i = 0; i < extraStr.Length; i++)
                {
                    int index = i;
                    oldKeyType += string.Format("_{{{0}}}", index);
                }
                
                string newKeyType = string.Format(oldKeyType, extraStr);
                return string.Format(format_keyType_serverId_roleId, newKeyType, serverId, roleId);
            }
        }

        public string GetType_AccId_Key(PlayerPrefsKeyType keyType, params object[] extraStr)
        {
            if (keyType == PlayerPrefsKeyType.None || keyType == PlayerPrefsKeyType.Count)
            {
                return "";
            }
            string AccId = "";
            if (null != ClientApplication.playerinfo)
            {
                AccId = ClientApplication.playerinfo.accid.ToString();
            }

            //Type _type = Type.GetType(String.Format("System.{0}", valueType.ToString()));
            if (extraStr == null || extraStr.Length <= 0)
            {
                return string.Format(format_keyType_accId, keyType.ToString(), AccId);
            }
            else
            {
                string strFormat = "{0}";
                string oldKeyType = string.Format(strFormat, keyType.ToString());
                for (int i = 0; i < extraStr.Length; i++)
                {
                    int index = i;
                    oldKeyType += string.Format("_{{{0}}}", index);
                }
                
                string newKeyType = string.Format(oldKeyType, extraStr);
                return string.Format(format_keyType_accId, newKeyType, AccId);
            }
        }
		
	   	public string HasType_ServerId_RoleId_Key(PlayerPrefsKeyType keyType, params object[] extraKeyTypes)
        {
            if (keyType == PlayerPrefsKeyType.None || keyType == PlayerPrefsKeyType.Count)
            {
                return string.Empty;
            }
            string typeKey = GetType_ServerId_RoleId_Key(keyType, extraKeyTypes);
            if (string.IsNullOrEmpty(typeKey))
            {
                return string.Empty;
            }
            if (!UnityEngine.PlayerPrefs.HasKey(typeKey))
            {
                return string.Empty;
            }
            return typeKey;
        }

        public string HasTypeKey(PlayerPrefsKeyType keyType)
        {
            if (keyType == PlayerPrefsKeyType.None || keyType == PlayerPrefsKeyType.Count)
            {
                return string.Empty;
            }
            string typeKey = keyType.ToString();
            if (string.IsNullOrEmpty(typeKey))
            {
                return string.Empty;
            }
            if (!UnityEngine.PlayerPrefs.HasKey(typeKey))
            {
                return string.Empty;
            }
            return typeKey;
        }

        public void SetTypeKeyIntValue(PlayerPrefsKeyType keyType, int value, params object[] extraKeyTypes)
        {
            if (keyType == PlayerPrefsKeyType.None || keyType == PlayerPrefsKeyType.Count)
            {
                return;
            }

            string typeKey = GetType_ServerId_RoleId_Key(keyType, extraKeyTypes);
            if (string.IsNullOrEmpty(typeKey))
            {
                return;
            }
            PlayerPrefs.SetInt(typeKey, value);
            PlayerPrefs.Save();
        }

        public void SetAccTypeKeyIntValue(PlayerPrefsKeyType keyType, int value, params object[] extraKeyTypes)
        {
            if (keyType == PlayerPrefsKeyType.None || keyType == PlayerPrefsKeyType.Count)
            {
                return;
            }

            string typeKey = GetType_AccId_Key(keyType, extraKeyTypes);
            if (string.IsNullOrEmpty(typeKey))
            {
                return;
            }
            PlayerPrefs.SetInt(typeKey, value);
            PlayerPrefs.Save();
        }

        public void SetTypeKeyFloatValue(PlayerPrefsKeyType keyType, float value, params object[] extraKeyTypes)
        {
            if (keyType == PlayerPrefsKeyType.None || keyType == PlayerPrefsKeyType.Count)
            {
                return;
            }

            string typeKey = GetType_ServerId_RoleId_Key(keyType, extraKeyTypes);
            if (string.IsNullOrEmpty(typeKey))
            {
                return;
            }
            PlayerPrefs.SetFloat(typeKey, value);
            PlayerPrefs.Save();
        }

        public void SetTypeKeyStringValue(PlayerPrefsKeyType keyType, string value, params object[] extraKeyTypes)
        {
            if (keyType == PlayerPrefsKeyType.None || keyType == PlayerPrefsKeyType.Count)
            {
                return;
            }

            string typeKey = GetType_ServerId_RoleId_Key(keyType, extraKeyTypes);
            if (string.IsNullOrEmpty(typeKey))
            {
                return;
            }
            PlayerPrefs.SetString(typeKey, value);
            PlayerPrefs.Save();
        }

        public void SetTypeKeyStringValueNoExtra(PlayerPrefsKeyType keyType, string value)
        {
            if (keyType == PlayerPrefsKeyType.None || keyType == PlayerPrefsKeyType.Count)
            {
                return;
            }

            string typeKey = keyType.ToString();
            if (string.IsNullOrEmpty(typeKey))
            {
                return;
            }
            UnityEngine.PlayerPrefs.SetString(typeKey, value);
            UnityEngine.PlayerPrefs.Save();
        }

        public void SetTypeKeyIntValueNoExtra(PlayerPrefsKeyType keyType, int value)
        {
            if (keyType == PlayerPrefsKeyType.None || keyType == PlayerPrefsKeyType.Count)
            {
                return;
            }

            string typeKey = keyType.ToString();
            if (string.IsNullOrEmpty(typeKey))
            {
                return;
            }
            UnityEngine.PlayerPrefs.SetInt(typeKey, value);
            UnityEngine.PlayerPrefs.Save();
        }

        public int GetTypeKeyIntValue(PlayerPrefsKeyType keyType, params object[] extraKeyTypes)
        {
            if (keyType == PlayerPrefsKeyType.None || keyType == PlayerPrefsKeyType.Count)
            {
                return 0;
            }
            string typeKey = GetType_ServerId_RoleId_Key(keyType, extraKeyTypes);
            if (string.IsNullOrEmpty(typeKey))
            {
                return 0;
            }
            if (!PlayerPrefs.HasKey(typeKey))
            {
                return 0;
            }
            return PlayerPrefs.GetInt(typeKey);
        }

        public int GetAccTypeKeyIntValue(PlayerPrefsKeyType keyType, params object[] extraKeyTypes)
        {
            if (keyType == PlayerPrefsKeyType.None || keyType == PlayerPrefsKeyType.Count)
            {
                return 0;
            }
            string typeKey = GetType_AccId_Key(keyType, extraKeyTypes);
            if (string.IsNullOrEmpty(typeKey))
            {
                return 0;
            }
            if (!PlayerPrefs.HasKey(typeKey))
            {
                return 0;
            }
            return PlayerPrefs.GetInt(typeKey);
        }

        public int GetTypeKeyIntValue(string typeKey)
        {
            if (string.IsNullOrEmpty(typeKey))
            {
                return 0;
            }
            return UnityEngine.PlayerPrefs.GetInt(typeKey);
        }

        public float GetTypeKeyFloatValue(PlayerPrefsKeyType keyType, params object[] extraKeyTypes)
        {
            if (keyType == PlayerPrefsKeyType.None || keyType == PlayerPrefsKeyType.Count)
            {
                return 0f;
            }
            string typeKey = GetType_ServerId_RoleId_Key(keyType, extraKeyTypes);
            if (string.IsNullOrEmpty(typeKey))
            {
                return 0f;
            }
            if (!PlayerPrefs.HasKey(typeKey))
            {
                return 0f;
            }
            return PlayerPrefs.GetFloat(typeKey);
        }

        public string GetTypeKeyStringValue(PlayerPrefsKeyType keyType, params object[] extraKeyTypes)
        {
            if (keyType == PlayerPrefsKeyType.None || keyType == PlayerPrefsKeyType.Count)
            {
                return "";
            }
            string typeKey = GetType_ServerId_RoleId_Key(keyType, extraKeyTypes);
            if (string.IsNullOrEmpty(typeKey))
            {
                return "";
            }
            if (!PlayerPrefs.HasKey(typeKey))
            {
                return "";
            }
            return PlayerPrefs.GetString(typeKey);
        }

        public string GetTypeKeyStringValueNoExtra(PlayerPrefsKeyType keyType)
        {
            if (keyType == PlayerPrefsKeyType.None || keyType == PlayerPrefsKeyType.Count)
            {
                return "";
            }
            string typeKey = keyType.ToString();
            if (string.IsNullOrEmpty(typeKey))
            {
                return "";
            }
            if (!UnityEngine.PlayerPrefs.HasKey(typeKey))
            {
                return "";
            }
            return UnityEngine.PlayerPrefs.GetString(typeKey);
        }
        #endregion
    }
}
