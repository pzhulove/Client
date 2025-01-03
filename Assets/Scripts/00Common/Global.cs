﻿/*#if !BEHAVIAC_NOT_USE_MONOBEHAVIOUR
#define BEHAVIAC_NOT_USE_MONOBEHAVIOUR
#endif

#if !BEHAVIAC_RELEASE
#define BEHAVIAC_RELEASE
#endif*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.IO;

public enum GlobalBuff
{
    /// <summary>
    /// 无敌
    /// </summary>
	INVINCIBLE = 2,
    /// <summary>
    /// 无敌不播透明
    /// </summary>
	INVINCIBLE_NO_ALPHA = 29,
    /// <summary>
    /// 无敌白色描边，假无敌
    /// </summary>
    INVINCIBLE_WHITE = 36,          
    /// <summary>
    /// 生命周期
    /// </summary>
    LIFE_TIME = 12,
    /// <summary>
    /// 抓取
    /// </summary>
    GRAB = 25,
    /// <summary>
    /// 起身霸体
    /// </summary>
    GETUP_BATI = 34,
    /// <summary>
    /// 蹲伏
    /// </summary>
	DUNFU = 38,
    /// <summary>
    /// 格挡
    /// </summary>
	GEDANG = 50,
    /// <summary>
    /// 死亡保护(复活币可以复活)
    /// </summary>
    DEAD_PROTECT = 24,
    /// <summary>
    /// 起身时候添加的无敌Buff 
    /// </summary>
    INVINCIBLE_GET_UP = 43,
}

public enum BattleRunMode
{
	RUN = 0,
	WALK_DOUBLEPRESS
}

public enum PreloadMode
{
	PART = 0,			//每个房间单独预加载，深渊一开始就加载
	PART_NO_AUDIO = 1,	//跟上面一样，但不加载音效
	ALL = 2				//全部在开始的时候预加载好
}

/// <summary>
/// 新添渠道时，注意区分支付方式的不同
/// </summary>
public enum SDKChannel
{
	NONE,

    /// <summary>
    /// Old SDKs 老渠道不再新加！！！！
    /// </summary>
	XY,
	AISI,
	TB,
    MG,
    SN79,
	TestMG,
    MGYingYB,
    MGOther,
    MGOtherHC,

    /// <summary>
    /// 多渠道 Android SDKs
    /// 
    /// 以后在这里添加新的渠道
    /// </summary>
    HuaWei,
	OPPO,
	VIVO,
	Lenovo,
	KuPai,
	JinLi,
    MeiZu,

    //
    BaiDu,
    XiaoMi,
    UC,
    SanXing,
    M4399,
    M360,
    M915,
    JUNHAI,
	JoyLand,
	
	/// <summary>
	/// 地下城盟约
	/// </summary>
	ZY,
    /// <summary>
    /// 口袋勇者 OL
    /// </summary>
    ZYOL,
	/// <summary>
    /// 泰拉契约
    /// </summary>
    ZYYH,
    /// <summary>
    /// 阿拉德之怒HD
    /// </summary>
    ZYHD,
    /// <summary>
    /// 阿拉德之怒WZ
    /// </summary>
    ZYWZ,

    /// <summary>
    /// 2018年9月7日开始新增马甲包渠道
    /// </summary>
    ZYYZ,
    ZYYS,
    ZYGB,
    ZYLK,
    ZYMG,

    ZYYY,
    ZYNL,

    ZYSY,


    /// <summary>
    /// 云测
    /// </summary>
    TestInMG,
	
	DYCC,
	
	MGDY,

    DYAY,

    COUNT
}



public class Global
{
	public const int HELP_SKILL_ID = 10000;
	public const int DUNFU_SKILL_ID = 10001;
    public const int FAKEREBORN_SKILL_ID = 10002;
	public const int GOLD_ITEM_ID = 600000001;
	public const int GOLD_ITEM_ID2 = 600000007;
    public const int CRYSTAL_ITEM_ID = 300000106;
    public const int MAX_PROFESSION = 100;
	public const int REBORN_COIN_COST_NUM = 1;
	public const int TEAM_TRANSPORT_NUM = 2;
	public const int MAX_TEAM_PLAYER_NUM = 4;
	public const int SUMMON_MONSTER_OCCU_ID = MAX_PROFESSION-1;
	public const int BODONGKEYIN_SKILL_ID = 1710;
    public const int ABNORMAL_COUNT = 13;
    public const int ELEMENT_COUNT = 4;

    public static BDGrabData gStaticGrabData = new BDGrabData();
    public static ChargeConfig gStaticChargeData = new ChargeConfig();
    public static OperationConfig gStaticOpConfig = new OperationConfig();
    public static SkillJoystickConfig gStaticJoyConfig = new SkillJoystickConfig();
    

    public static ScreenOrientation screenOrientation = ScreenOrientation.Unknown;

    public static string DEFAULT_AGENT = "DEFAULT_AGENT";
    public static string GUAN_WANG = "NONE";

    public static string USERNAME = null;
    public static string PASSWORD = null;

    // public const string MAIN_ADDRESS = "43.240.156.217:18888";
    // public const string MAIN_ADDRESS = "43.240.156.197:18888";
    // public const string MAIN_ADDRESS = "103.36.166.92:18888";
    public const string MAIN_ADDRESS = "192.168.1.105:81";

    public static string REGISTER_ADDRESS = "http://" + MAIN_ADDRESS + "/login/reg";
    public static string LOGIN_ADDRESS = "http://" + MAIN_ADDRESS + "/login/login";
    public static string LOGIN_VERSION = "2.0";

    public static string MW_RANK_ADDRESS = "http://" + MAIN_ADDRESS + "/mw_rank/index";

    public static string PAY_ADDRESS = "http://" + MAIN_ADDRESS + "/pay/";
    public static string PAY_KEY = "+QQ:5311210";

    public static string BXY_INFO = null;
    public static string REXUE_DUOBAO = null;
    public static ulong bxyMergeGuid = 0;
    public static ulong sinanRongheGuid = 0;
    public static ulong jichengGuid = 0;

    public static string UPLOAD_PLAYER_DUNGEON_INFO_ADDRESS = "http://" + MAIN_ADDRESS + "/callback/index/onDungeonRecord";

    public static string AGENT_TEXT = "邀请码";

    public static string ANTI_ADDICITION_ADDRESS = "http://" + MAIN_ADDRESS + "/realname/index/auth";
    // 控制是否弹出实名窗口
    public static bool isShowSMDialog = true;
    // 控制是否弹出防沉迷窗口
    public static bool isShowFCMDialog = true;

    // 控制是否显示登录注册协议
    public static bool isShowLoginUserAgree = false;

    public const string GLOBAL_SERVER_ADDRESS = "192.168.1.105:81";
    /*	public const string GLOBAL_PUBLISH_SERVER_ADDRESS = "118.89.36.115:12345";
        public const string GLOBAL_STATISTIC_SERVER_ADDRESS = "118.89.36.115:59236";
        public const string GLOBAL_ROLE_SAVEDATA_SERVER_ADDRESS = "118.89.36.115:8765";*/

    /*	#if UNITY_ANDROID
        public static string ROLE_SAVEDATA_SERVER_ADDRESS = "120.132.26.173:58765";
        #else*/
#if UNITY_EDITOR || TEST_PACKAGE || UNITY_STANDALONE
    public static string ROLE_SAVEDATA_SERVER_ADDRESS = MAIN_ADDRESS;
#elif UNITY_ANDROID
    public static string ROLE_SAVEDATA_SERVER_ADDRESS = MAIN_ADDRESS;
	//public static string ROLE_SAVEDATA_SERVER_ADDRESS = "118.178.215.163:58003";
#elif UNITY_IOS
	public static string ROLE_SAVEDATA_SERVER_ADDRESS = MAIN_ADDRESS;
#else
	public static string ROLE_SAVEDATA_SERVER_ADDRESS = MAIN_ADDRESS;
#endif

    public static string ROLE_QUERY_OPENID_ADDRESS = "192.168.1.120:58004";

    public static string RECORDLOG_GET_ADDRESS = "192.168.1.120:11111";
    public static string RECORDLOG_POST_ADDRESS = "192.168.1.120:11111";
    public static string BATTLE_PERFORMANCE_POST_ADDRESS = "192.168.1.120:11111";
    //	#endif
    public static string ENET_LOG_SERVER_ADDRESS = "192.168.1.120:59970";
	public static string LOGIN_SERVER_ADDRESS = "192.168.1.120:6667";
	public static string PUBLISH_SERVER_ADDRESS = "192.168.1.120:12349";
	public static string NOTICE_SERVER_ADDRESS = MAIN_ADDRESS;
    public static string STATISTIC_SERVER_ADDRESS = "192.168.1.120:59236";
    public static string STATISTIC2_SERVER_ADDRESS = "192.168.1.120:55563";//用于上报一些异常信息的统计
    public static string VOICE_SERVER_ADDRESS = "192.168.1.120:56563";//"192.168.2.22:56563";
    public static string VERIFY_BIND_PHONE_ADDRESS = "192.168.1.120:19558";
    public static string ONLINE_SERVICE_ADDRESS = "kf.kingnet.com";//"120.78.28.149";
    public static string ONLINE_SERVICE_REQ_ADDRESS = "kfservice.kingnet.com";//"120.78.28.151:9400"
    public static string ANDROID_MG_CHARGE = "192.168.1.120:58002";
    public static string ROLE_SAVEDATA_SERVER_ADDRESS_HW = "192.168.1.120:58003";
	public static string IOS_ZY_CHARGE = "192.168.1.120:56352";
	public static string IOS_BANQUAN_ADDRESS = "192.168.1.120:19268";

    public static string USER_AGREEMENT_SERVER_ADDRESS = MAIN_ADDRESS;
    public static string ONLINE_SERVICE_VIP_CHECK_ADDRESS = "ald.xy.com/services/custom";
    public static string REDPACKRANK_SERVICE_ADDRESS = "192.168.1.120:58300";//全平台红包排行榜
    public static string BANGBANGEVERISK_SERVICE_ADDRESS = "192.168.1.120:58005/custom";

    public const string ATTACH_NAME_ORIGIN = "[actor]Orign";
    public const string FOOT_INDICATOR_ATTACH_NAME = "Aureole";
    public const string HALO_ATTACH_NAME = "halo";
    public const string HALO_LOCATOR_NAME = "[actor]Orign";
    public const string WEAPON_ATTACH_NAME = "weapon";
	public const string WING_ATTACH_NAME = "wing";
    public const string WING_LOCATOR_NAME =  "[actor]Back";
	public const string STRENGTH_NAME = "强化1";
	public const string WEAPON_SWORD_NAME = "sword";
	public const string WEAPON_GUN_NAME = "gun";
	public const string WEAPON_MAGE_NAME  = "magegirl";
    public const string WEAPON_FIGHTER_NAME = "fighter";
    public const string STRENGTH_SWORD_NAME = "Sword_Effect";
	public const string STRENGTH_GUN_NAME = "Gun_Effect";
	public const string STRENGTH_MAGE_NAME = "Mage_Effect";
    public const string STRENGTH_FIGHTER_NAME = "Fighter_Effect";
    public const string STRENGTH_NIANZHU_NAME = "Paladin_Nianzhu_Effect";
    public const string STRENGTH_COMMON_NAME = "Paladin_Liandao_Effect";
    public const string WEAPON_LIANDAO_NAME = "Paladin_liandao";
    public const string WEAPON_NIANZHU_NAME = "Paladin_nianzhu";
    public const string WEAPON_ZHANFU_NAME = "Paladin_zhanfu";
    public const string WEAPON_SHIZIJIA_NAME = "Paladin_shizijia";

    public const string ACTION_EXPDEAD = "ExpDead";
	public const string ACTION_EXPDEAD2 = "Expdead2";
	public const string ACTION_EXPDEAD3 = "Expdead3";
    public const string ACTION_HITGROUNDDEAD = "HitGroundDead";         //实体触地死亡动作

	public const int PK_TOTAL_TIME = 4 * 60;//4 * 60;
	public const int PK_COUNTDOWN_TIME = 4;

	public const int REVIVE_SHOCK_SKILLID = 9998;

	public const string COMMON_SOUND_OPEN_FRAME = "Sound/UI/ui_window_open";
	public const string COMMON_SOUND_CLOSE_FRAME = "Sound/UI/ui_window_close";

	public const PreloadMode PRELOAD_MODE = PreloadMode.PART_NO_AUDIO;

    public static string[] SDKChannelName = { "none", "xy", "i4", "tb", "mg", "7977", "mg", "mgyyb" ,"mg2other", "mg2hc","hw", "oppo", "vivo", "lenovo", "kupai", "jinli", "meizu", "baidu", "xiaomi","uc","samsung", "4399", "360","915","junhai","joyland","zy","zy","zy","zy","zy",
                                            "zy","zy","zy","zy","zy","zy","zy","zy","mg","cc","mgdy","cc"};


    public static SDKChannel[] VipAuthSDKChannel = new SDKChannel[0];
    public static SDKChannel[] RealNamePopWindowsChannel = new SDKChannel[0];
    public static SDKChannel[] SDKPushChannel = new SDKChannel[0];


    public const string PATH = "Base/GlobalSettings";
    private static GlobalSetting settings = null;

    public static string[] AbnormalNames = new string[]
    {
        "感电","出血","灼烧","中毒","失明","眩晕","石化","冰冻","睡眠","混乱","束缚","减速","诅咒"
    };

#region 装备强化
	public static Dictionary<string, string> equipThirdTypeNames = new Dictionary<string, string>(){
		{"HUGESWORD","巨剑"},
		{"KATANA","太刀"},
		{"SHORTSWORD","短剑"},
		{"BEAMSWORD","光剑"},
		{"BLUNT","钝器"},
		{"REVOLVER","左轮"},
		{"CROSSBOW","弩"},
		{"HANDCANNON","手炮"},
		{"RIFLE","步枪"},
		{"PISTOL","手枪"},
		{"STAFF","法杖"},
		{"WAND","魔杖"},
		{"SPEAR","矛"},
		{"STICK","棍棒"},
		{"CLOTH","布甲"},
		{"SKIN","皮甲"},
		{"LIGHT","轻甲"},
		{"HEAVY","重甲"},
		{"PLATE","板甲"}
	};

	public static List<string> equipThirdTypeNamesList = new List<string>(){
		"HUGESWORD",
		"KATANA",
		"SHORTSWORD",
		"BEAMSWORD",
		"BLUNT",
		"REVOLVER",
		"CROSSBOW",
		"HANDCANNON",
		"RIFLE",
		"PISTOL",
		"STAFF",
		"WAND",
		"SPEAR",
		"STICK",
		"CLOTH",
		"SKIN",
		"LIGHT",
		"HEAVY",
		"PLATE",
	};

	public static Dictionary<string, int> equipPropExtra = new Dictionary<string, int>(){
		{"maxHp", 1},
		{"maxMp", 1},
		{"hpRecover", 1},
		{"mpRecover", 1},
		{"attack", 1},
		{"magicAttack", 1},
		{"defence", 1},
		{"magicDefence", 1},
		{"attackSpeed", 10},
		{"spellSpeed", 10},
		{"moveSpeed", 10},
		{"ciriticalAttack", 10},
		{"ciriticalMagicAttack", 10},
		{"dex", 10},
		{"dodge", 10},
		{"frozen", 1},
		{"hard", 1},

		{"baseAtk", 1000},
		{"baseInt", 1000},
		{"baseSta", 1000},
		{"baseSpr", 1000},

		//	{"criticalPercent", 2f},

		{"attackReduceFix", 1},
		{"magicAttackReduceFix", 1},
		{"ignoreDefAttackAdd", 1},
		{"ignoreDefMagicAttackAdd", 1},
	};

	public static List<string> equipPropName = new List<string>(){
		"maxHp",
		"maxMp", 
		"hpRecover", 
		"mpRecover", 
		"attack", 
		"magicAttack", 
		"defence", 
		"magicDefence", 
		"attackSpeed", 
		"spellSpeed", 
		"moveSpeed", 
		"ciriticalAttack", 
		"ciriticalMagicAttack", 
		"dex", 
		"dodge", 
		"frozen", 
		"hard", 

		"baseAtk", 
		"baseInt", 
		"baseSta", 
		"baseSpr", 

		"attackReduceFix", 
		"magicAttackReduceFix", 
		"ignoreDefAttackAdd", 
		"ignoreDefMagicAttackAdd", 
	};


    // 附魔元素对应的特效和音效
    public static List<string> magicElementsEffectMap = new List<string>(){
        "Effects/Common/Sfx/Hit/Prefab/Eff_hit_guang",
        "Effects/Common/Sfx/Hit/Prefab/Eff_hit_huo",
        "Effects/Common/Sfx/Hit/Prefab/Eff_hit_bing",
        "Effects/Common/Sfx/Hit/Prefab/Eff_hit_an"
    };
    public static List<int> magicElementsSoundMap = new List<int>(){
        1016,
        1015,
        1017,
        1014,
    };

#endregion

    public static int EXTRA_MODEL_MAX = 10;

    private static readonly string kSDKConfigFileName = "sdk.conf";

    private static readonly string kAgentConfigFileName = "agent.conf";
    private static readonly string kVersionConfigFileName = "version.conf";

    public static int SUMMONMONSTERTYPE = 10;                           //用于标识召唤兽怪物类型

    public static  GlobalSetting Settings {
        get {
            if(null == settings)
            {
#if USE_FB
                FBGlobalSettingSerializer.LoadFBGlobalSetting(Path.Combine(Utility.kRawDataPrefix, Path.ChangeExtension(PATH, Utility.kRawDataExtension)), out settings);
#else
                settings = Resources.Load<GlobalSetting>(PathUtil.EraseExtension(PATH));

                settings.useNewHurtAction = false;
                settings.useNewGravity = false;
                settings.isDebug = false;
                settings.isGuide = true;
                // 本地调试的时候，将这个值改成false
                settings.enableHotFix = false;
                settings.loadFromPackage = false;

                Logger.LogError("setting.isDebug = " + settings.isDebug);
                Logger.LogError("setting.isGuide = " + settings.isGuide);
                Logger.LogError("setting.enableHotFix = " + settings.enableHotFix);
                Logger.LogError("setting.loadFromPackage = " + settings.loadFromPackage);

                _loadSdkFromFile();
                _loadAgentFromFile();
                _loadVersionFromFile();
#endif

                if (settings == null)
                {
                    //Logger.LogError("setting is nil");
                }
            }

            return settings;
        }
        
        set {
            settings = value;
        }
    }

    private static void _loadSdkFromFile()
    {
        if (null == settings)
        {
            return ;
        }

        string channelName = SDKChannel.NONE.ToString();

        if (FileArchiveAccessor.LoadFileInLocalFileArchive(kSDKConfigFileName, out channelName))
        {
            try 
            {
                Logger.LogProcessFormat("[SDKConfig] 读取 {0} : {1}", kSDKConfigFileName, channelName);
                settings.sdkChannel = (SDKChannel)Enum.Parse(typeof(SDKChannel), channelName, true);
            }
            catch (System.Exception e)
            {
                Logger.LogErrorFormat("[SDKConfig] 解析 {0} 失败, {1}", kSDKConfigFileName, e.ToString());

            }
        }
        else
        {
            Logger.LogErrorFormat("[SDKConfig] 加载 {0} 失败", kSDKConfigFileName);
        }
    }

    private static void _loadVersionFromFile()
    {
        if (null == settings)
        {
            return ;
        }

        string defaultVersion = "2.0";

        if (FileArchiveAccessor.LoadFileInLocalFileArchive(kVersionConfigFileName, out defaultVersion))
        {
            Global.LOGIN_VERSION = defaultVersion;
        }
        else
        {
            Logger.LogErrorFormat("[VersionConfig] 加载 {0} 失败", kVersionConfigFileName);
        }
    }

    private static void _loadAgentFromFile()
    {
        if (null == settings)
        {
            return ;
        }

        string defaultAgent = "DEFAULT_AGENT";

        if (FileArchiveAccessor.LoadFileInLocalFileArchive(kAgentConfigFileName, out defaultAgent))
        {
            Global.DEFAULT_AGENT = defaultAgent;
        }
        else
        {
            Logger.LogErrorFormat("[AgentConfig] 加载 {0} 失败", kAgentConfigFileName);
        }
    }

	private static Dictionary<int/*AttributeType*/, string> attributeStringMap = new Dictionary<int, string>();
	public static string GetAttributeString(AttributeType att)
	{
		int at = (int)att;
		if (attributeStringMap.ContainsKey(at))
			return attributeStringMap[at];

		string str = Enum.GetName(typeof(AttributeType), att);
		attributeStringMap[at] = str;

		return str;
	}

}
