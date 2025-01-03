using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using System.IO;
using XUPorterJSON;
using ProtoTable;
using Network;
using GameClient;
using Protocol;

#if UNITY_IPHONE || UNITY_IOS
using UnityEngine.iOS;
#endif

public enum StatType
{
	DEVICE = 0, //设备
	ROLE,		//角色
	ABNORMAL,  //异常情况统计
}

public enum StatTaskType
{
	TASK_ACCEPT = 0,
	TASK_FIND_ROAD,
	TASK_FINISH
}

public enum StatSkillPanelType
{
	SKILL_LEVEL_UP = 0,
	SKILL_LEVEL_DOWN,
	SKILL_CONFIG
}

public enum StatLevelChooseType
{
	ENTER_SELECT = 0,	//点击进入关卡选择
	CHOOSE_LEVEL,		//点击选择关卡
	ENTER_LEVEL			//点击进入战斗
}

public enum StatItemUse
{
    DUNGEON_BUFF_DRUG,
}

public enum StatInBattleType
{
	ENTER = 0,
	CLEAR_ROOM,		//怪物清楚onclear
	PASS_DOOR,		//过门
	FINISH,			//战斗结束
	CLICK_RESULT,	//点击结算
	CLICK_CARD,		//点击翻牌
	CLICK_RETURN,	//点击返回城镇

    USE_SKILL,

    PLAYER_DEAD,
    PLAYER_REBORN,
    PLAYER_USESKILL,
}

public enum StatPKType
{
	ENTER = 0, 	//进入角斗场
	MATCHING,	//点击匹配
	RESULT		//记录结果
}

public enum StartOPPOType
{
    OPPOICON=0,            //OPPO特权浮标次数
    OPPOOPEN,             //OPPO页面打开次数
    OPPOJUMPGAMECENTER,  //OPPO按钮跳转游戏中心次数
    OPPOPRIVILEGE,      //OPPO游戏特权领取次数
    OPPOLUCKYTABLE,    //OPPO幸运转盘领取次数
    OPPOSIGE,         //OPPO签到次数
    OPPOAMBERGIFT,   //OPPO琥珀礼包
}

public enum StartVIVOType
{
    VIVOICON = 0,            //VIVO特权浮标次数
    VIVOOPEN,             //VIVO页面打开次数
    VIVOJUMPGAMECENTER,  //VIVO按钮跳转游戏中心次数
    VIVOPRIVILEGE,      //VIVO游戏特权领取次数
}

//滑动操作无极剑式类型
public enum InfiniteSwordType
{
    Close,
    Open,
}

public enum RunningModeType
{
    DragDropMovement=0,     //拖拽跑动
    DoubleClickMovement=1,  //双击跑动
}

public enum StatSystemType
{
	NONE = 0,
	LOGIN,
	TOWN,
	BATTLE
}

public enum YouMiVoiceSDKCostType
{
    VOICE_SDK_LOGINED  = 0,       //游密语音登录
    VOICE_SDK_SENDED,               //游密语音发送录音完成
    VOICE_SDK_DOWNLOADED,               //游密语音下载录音完成
    VOICE_SDK_OPEN_AUTOPLAY,                 //语音自动播放功能开启
    VOICE_SDK_JOIN_TALK                         //加入实时语音房间
}

public class GameStatisticManager : Singleton<GameStatisticManager> {

    bool clientInfoSended = false;
    public class StatData
    {
        public string content;
        public string time;

        public StatData(string c, string t)
        {
            content = c;
            time = t;
        }
    }
    public DataStatistics dataStatistics = new DataStatistics();
    List<StatData> contents = new List<StatData>();
    int count = 0;

    UnityEngine.Networking.NetworkIdentity  m_Identity;

    public override void Init()
    {
        base.Init();

       

    }

    public enum DialogOperateType
    {
        DOT_NEXT,
        DOT_COMPLETE,
        DOT_JUMPOVER,
        DOT_STAR,
    }

    public enum DialogFinishType
    {
        DFT_FINISH = 0,
        DFT_RESTAR,
        DFT_NEWCREATE,
    }
#if STAT_EXTRA_INFO
    #region SMALLPACKAGE_STAT
    public enum ExtraInfo
    {
        FRAME = 0,          //帧率
        CPU_TEMP,       //cpu 温度
        BATTERY_TEMP,   //电池温度
        BATTERY,        //电量
        PASSDOOR_TOWN,  //城镇过门时间
        PASSDOOR_DUNGEON,  //地下城过门时间
        DUNGEON_LOADING,   //地下城loading时间

        SP_ISDOWNLOADING, //在游戏前十五分钟是边下边玩还是未下载
        SP_WIFI_4G,     //wifi环境还是4G环境
        SP_DOWNLOADIND_FULLTIME, //下载资源消耗总时长
    }

    public enum SPDownloadingStat
    {
        NOT_DOWNLOAD = 0,
        DOWNLOADING = 1,
        DOWNLOADED = 2,
    }

    private readonly string[] _extraInfoString = {
        "FRAME",
        "CPU_TEMP",
        "BATTERY_TEMP",
        "BATTERY",
        "PASSDOOR_TOWN",
        "PASSDOOR_DUNGEON",
        "DUNGEON_LOADING",
        "SP_ISDOWNLOADING",
        "SP_WIFI_4G",
        "SP_DOWNLOADIND_FULLTIME" };

    private readonly string[] _smallpackageString = {
        "NOT_DOWNLOAD",
        "DOWNLOADING",
        "DOWNLOADED"
    };

    private readonly string KEY_START_DOWNLOAD = "SP_START_DOWNLOAD";
    private readonly string KEY_DOWNLOADED = "SP_KEY_DOWNLOADED";

    private SPDownloadingStat GetSmallPackageResDownloadStat()
    {
        SPDownloadingStat stat = SPDownloadingStat.NOT_DOWNLOAD;

        //整包当做下载完的情况
        if (!SDKInterface.instance.IsSmallPackage())
        {
            stat = SPDownloadingStat.DOWNLOADED;
        }
        else
        {
            if (SDKInterface.instance.IsResourceDownloadFinished())
            stat = SPDownloadingStat.DOWNLOADED;
            else
            {
                bool isWifi = Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
                bool is4G = Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork;
                if (isWifi || is4G && SDKInterface.instance.IsDownload())
                    stat = SPDownloadingStat.DOWNLOADING;
            }
        }
        
        return stat;
    }

    public void RecordDownloadStartTime()
    {
        if (!PlayerPrefs.HasKey(KEY_START_DOWNLOAD))
        {
            var currentSeconds = (int)Utility.GetCurrentTimeUnix();
            PlayerPrefs.SetInt(KEY_START_DOWNLOAD, currentSeconds);
        }
    }

    public void RecordDownloadEndTime()
    {
        if (!PlayerPrefs.HasKey(KEY_DOWNLOADED) && PlayerPrefs.HasKey(KEY_START_DOWNLOAD))
        {
            var endSeconds = (int)Utility.GetCurrentTimeUnix();
            var startSeconds = PlayerPrefs.GetInt(KEY_START_DOWNLOAD);

            if (endSeconds > startSeconds)  
            {
                var seconds = endSeconds - startSeconds;
                int min = seconds / 60;
                //Logger.LogErrorFormat("seconds:{0} min:{1}", seconds, min);

                DoStatSmallPackageInfo(ExtraInfo.SP_DOWNLOADIND_FULLTIME, min.ToString());
            }
        }
    }

    public void DoStatSmallPackageInfo_Net()
    {
        var netType = (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)?"4G":"WIFI";
        DoStatSmallPackageInfo(ExtraInfo.SP_WIFI_4G, netType);
    }

    public void DoStatSmallPackageInfo_ISDOWNLOADING()
    {
        DoStatSmallPackageInfo(ExtraInfo.SP_ISDOWNLOADING, "1");
    }

    public void DoStatSmallPackageInfo(ExtraInfo info, string value)
    {
        if ( (  info == ExtraInfo.SP_DOWNLOADIND_FULLTIME ||
                info == ExtraInfo.SP_ISDOWNLOADING ||
                info == ExtraInfo.SP_WIFI_4G) && !SDKInterface.instance.IsSmallPackage())
            return;

        SPDownloadingStat stat = GetSmallPackageResDownloadStat();

        var str = string.Format("{3}|{0}|{1}|{2}", 
        _smallpackageString[(int)stat], 
        _extraInfoString[(int)info], 
        value,
        SDKInterface.instance.IsSmallPackage()?"SMALLPACKAGE":"FULLPACKAGE");

        //Logger.LogErrorFormat("[stat]{0}", str);

        DoStatistic(str);
    }

    #endregion
#endif
    //对话框
    public void DoStatDialog(int dialogID, int nextDialogID, DialogOperateType eDialogOperateType, DialogFinishType eDialogFinishType)
    {
        return;

        string value = "";
        switch (eDialogOperateType)
        {
            case DialogOperateType.DOT_NEXT:
                {
                    value = string.Format("DLG|{0}|NEXT|{1}|", dialogID, nextDialogID);
                    break;
                }
            case DialogOperateType.DOT_COMPLETE:
                {
                    value = string.Format("DLG|{0}|COMPLETE|{1}|", dialogID, nextDialogID);
                    break;
                }
            case DialogOperateType.DOT_JUMPOVER:
                {
                    value = string.Format("DLG|{0}|JUMP|{1}|", dialogID, nextDialogID);
                    break;
                }
        }

        string appendValue = "";
        switch (eDialogFinishType)
        {
            case DialogFinishType.DFT_FINISH:
                {
                    appendValue = "FH";
                    break;
                }
            case DialogFinishType.DFT_NEWCREATE:
                {
                    appendValue = "NC";
                    break;
                }
            case DialogFinishType.DFT_RESTAR:
                {
                    appendValue = "RS";
                    break;
                }
        }
        value += appendValue;

        if (!string.IsNullOrEmpty(value))
        {
            DoStatistic(value);
        }
    }

    //任务
    public void DoStatTask(StatTaskType type, int taskID)
    {
        return;

        MissionTable missiondata = TableManager.GetInstance().GetTableItem<MissionTable>(taskID);
        if (missiondata == null)
        {
            return;
        }

        if (!missiondata.IsNeedBuriedPoint)
        {
            return;
        }

        DoStatistic(string.Format("MISSION|{0}|{1}|", _getString(type), taskID));
    }

    //新手引导
    public void DoStatNewBieGuide(string key, int idex = -1)
    {
        if (ServerSceneFuncSwitchManager.GetInstance().IsServiceTypeSwitchOpen(ServiceType.SERVICE_NEW_CLIENT_LOG))
        {
            // 新的埋点机制，发送到gate
            NewDoStatistic("newbieguide", string.Format("NewBieGuide:{0}:{1}", key, idex));
        }
        else
        {
            DoStatistic(string.Format("NewBieGuide|{0}|{1}|", key, idex));
        }
    }

    // 这个不是埋点上报，这个是把客户端日志上传到服务器（可能会有数据丢失，也不要随便到处用这个接口，谁要用这个接口，请先通知王博）
    public void UploadLocalLogToServer(string log)
    {
#if MG_TEST
        if (ServerSceneFuncSwitchManager.GetInstance().IsServiceTypeSwitchOpen(ServiceType.SERVICE_NEW_CLIENT_LOG))
        {
            // 新的埋点机制，发送到gate
            NewDoStatistic("UploadLocalLogToServer", string.Format("UploadLog_{0}_{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms"), log));
        }
        else
        {
            DoStatistic(string.Format("UploadLog.{0}.{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms"), log));
        }
#endif
    }

    // 进入房间
    public void DoStatEnterPVERoom(int GuanKaID, int RoomID)
    {
        DoStatistic(string.Format("EnterPVERoom|{0}|{1}|", GuanKaID, RoomID));
    }
    public class Ping
    {
        public int S;
        public int A;
        public int B;
        public int C;
    };
    public class Fps
    {
        public int S;
        public int A;
        public int B;
    };
    public class BattleStatistic
    {
        public string roleId;
        public string roleLv;
        public string account;
        public string teamJobId;
        public string JobId;
        public string sid;
        public string platId;
        public string vip;
        public string channel;
        public string deviceId;
        public string ip;
        public string ip_ext;
        public string gameVersion;
        public string netType;
        public string mfr;
        public string model;
        public string os_ver;
        public Ping ping = new Ping();
        public Fps fps = new Fps();
        public string exeCmdRecentDelay;
        public string exeCmdAvarageDelay;
        public string exeCmdMaxDelay;
        public string recvCmdRecentDelay;
        public string recvCmdAvarageDelay;
        public string recvCmdMaxDelay;

    };
    public void DoStatFinishMeleeBattle(string jobs)
    {
        //string ping = "Ping : S|{0},A|{1},B|{2},C|{3};";
        //string fps = "Fps : S|{0},A|{1},B|{2};";

        //var execCmdPerf = FrameSync.instance.execCmdPerf;
        //var recvCmdPerf = FrameSync.instance.recvCmdPerf;
        //string framePerfText1 = string.Format("Exec: Recent:{0} Average:{1} Max:{2}\n", execCmdPerf.recentDelay, execCmdPerf.averageDelay, execCmdPerf.maxDelay);
        //string framePerfText2 = string.Format("Recv: Recent:{0} Average:{1} Max:{2}\n", recvCmdPerf.recentDelay, recvCmdPerf.averageDelay, recvCmdPerf.maxDelay);

        //string contact = string.Format(ping, dataStatistics.pingNums[0], dataStatistics.pingNums[1], dataStatistics.pingNums[2], dataStatistics.pingNums[3]) +
        //                 string.Format(fps, dataStatistics.fpsNums[0], dataStatistics.fpsNums[1], dataStatistics.fpsNums[2]) + framePerfText1 + framePerfText2;

        //DoStatistic(contact);
        var execCmdPerf = FrameSync.instance.execCmdPerf;
        var recvCmdPerf = FrameSync.instance.recvCmdPerf;
        var data = ClientApplication.playerinfo.GetSelectRoleBaseInfoByLogin();
        var battleData = new BattleStatistic
        {

            

            gameVersion = VersionManager.GetInstance().Version(),
            roleId = data != null ? data.roleId.ToString() : string.Empty,
            roleLv = data != null ? data.level.ToString() : string.Empty,
            account = ClientApplication.playerinfo.accid.ToString(),
            teamJobId = jobs,
            JobId = GameClient.PlayerBaseData.GetInstance() != null ? GameClient.PlayerBaseData.GetInstance().JobTableID.ToString(): "0",
            sid = ClientApplication.playerinfo.serverID.ToString(),
            platId = Application.platform.ToString(),
            vip = GameClient.PlayerBaseData.GetInstance() != null ? GameClient.PlayerBaseData.GetInstance().VipLevel.ToString() : "0",
            channel = Global.SDKChannelName[(int)Global.Settings.sdkChannel],
            deviceId = SystemInfo.deviceUniqueIdentifier,
            ip =  "0,0,0,0",//m_Identity.ip//UnityEngine.Network.player.ipAddress,
            ip_ext = "0,0,0,0",//UnityEngine.Network.player.externalIP,
            netType = Application.internetReachability.ToString(),
            exeCmdRecentDelay = execCmdPerf.recentDelay.ToString(),
            exeCmdAvarageDelay = execCmdPerf.averageDelay.ToString(),
            exeCmdMaxDelay = execCmdPerf.maxDelay.ToString(),
            recvCmdRecentDelay = recvCmdPerf.recentDelay.ToString(),
            recvCmdAvarageDelay = recvCmdPerf.averageDelay.ToString(),
            recvCmdMaxDelay = recvCmdPerf.maxDelay.ToString(),
#if UNITY_IPHONE || UNITY_IOS
		    mfr = string.IsNullOrEmpty(Device.vendorIdentifier) ? "" : Device.vendorIdentifier,
            model = Device.generation.ToString(),
            os_ver = Device.systemVersion
#else
            mfr = SystemInfo.deviceModel,
            model = SystemInfo.deviceName,
            os_ver = SystemInfo.operatingSystem
#endif
        };
        battleData.ping.S = dataStatistics.pingNums[0];
        battleData.ping.A = dataStatistics.pingNums[1];
        battleData.ping.B = dataStatistics.pingNums[2];
        battleData.ping.C = dataStatistics.pingNums[3];
        battleData.fps.S = dataStatistics.fpsNums[0];
        battleData.fps.A = dataStatistics.fpsNums[1];
        battleData.fps.B = dataStatistics.fpsNums[2];
        var jsonText = LitJson.JsonMapper.ToJson(battleData);
        try
        {
            Http.SendPostRequest(string.Format("http://{0}//performance", Global.BATTLE_PERFORMANCE_POST_ADDRESS), jsonText);
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat("Post BattleData error reason {0}", e.Message);
        }
    }

    // 结算界面
    public void DoStatFinishBattle(int GuanKaID)
    {
        return;
        DoStatistic(string.Format("FinishBattle|{0}|", GuanKaID));
    }

    //滑动操作
    public void DoStatSlideOperation(InfiniteSwordType infiniteSwordType)
    {
        return;
        DoStatistic(string.Format("InfiniteSwordType|{0}|", infiniteSwordType));
    }

    // 摇杆设置
    public void DoStatJoyStick(InputManager.JoystickMode joystickMode)
    {
        DoStatistic(string.Format("JoystickMode|{0}|", joystickMode));
    }
    //双击跑动和拖拽跑动
    public void DoStatRunning(RunningModeType runMode)
    {
        DoStatistic(string.Format("RunningMode|{0}|", runMode));
    }
    /// <summary>
    /// 技能面板在不同模式下配置的技能
    /// </summary>
    public void DoStartSkillConfiguration(Protocol.SkillConfigType skillConfigType,int skillId,int skillSlot)
    {
        string skillConfig = "";
        if (Protocol.SkillConfigType.SKILL_CONFIG_PVE == skillConfigType )
        {
            skillConfig = "PVE";
        }
        else
        {
            skillConfig = "PVP";
        }
        DoStatistic(string.Format("SkillConfigType:{0},SkillID:{1},SkillSlot:{2}", skillConfig, skillId, skillSlot));
    }

    //技能面板操作
    public void DoStatSkillPanel(StatSkillPanelType type, int skillID, int level=0, int slot=0)
	{
		return;
		if (type == StatSkillPanelType.SKILL_CONFIG)
			DoStatistic(string.Format("click{0}", _getString(type)));
		else
			DoStatistic(string.Format("clickSkill{0}", _getString(type), skillID, level));
	}


	//关卡UI点击
	public void DoStatLevelChoose(StatLevelChooseType type, int chapterID, int levelID=0, int difficult=0, List<int> drugs=null)
	{
		return;
		if (type == StatLevelChooseType.ENTER_SELECT)
			DoStatistic(string.Format("clickChapterSelect c:{0}", chapterID));
		else
		{
			StringBuilder stackTraceBuilder = new StringBuilder ("");
			stackTraceBuilder.AppendFormat("click{0}chapter{1}level{2}difficult{3}", _getString(type), chapterID, levelID, difficult);
			if (drugs != null)
			{
				stackTraceBuilder.Append("use drug");
				for(int i=0; i<drugs.Count; ++i)
					stackTraceBuilder.Append(drugs[i]);
			}

			DoStatistic(stackTraceBuilder.ToString());
		}
	}

    //OPPO特权
    public void DoStartOPPO(StartOPPOType type ,string str="")
    {
        return;
        DoStatistic(string.Format("Oppo|{0}|{1}", type, str));
    }
    //VIVO特权
    public void DoStartVIVO(StartVIVOType type)
    {
        return;
        DoStatistic(string.Format("VIVO|{0}", type));
    }
    //UI按钮埋点
    public void DoStartUIButton(string str)
    {
        string info = string.Format("UIButton:{0}", str);

        if (ServerSceneFuncSwitchManager.GetInstance().IsServiceTypeSwitchOpen(ServiceType.SERVICE_NEW_CLIENT_LOG))
        {
            // 新的埋点机制，发送到gate
            NewDoStatistic("guide", info);
        }
        else
        {
            DoStatistic(info);
        }
    }
    /// <summary>
    /// 关卡结算评分
    /// </summary>
    /// <param name="str"></param>
    public void DoStartCheckPointsSettlement(int dungeonId,string str)
    {
        return;
        DoStatistic(string.Format("DungeonID : {0}、CheckPointsSettlement : {1}",dungeonId, str));
    }

    public void DoStartFuilDailChargeRaffle()
    {
        DoStatistic(string.Format("FuilDailChargeRaffle"));
    }

    /// <summary>
    /// 单局内再来一次按钮
    /// </summary>
    /// <param name="str"></param>
    public void DoStartSingleBoardDoAgainButton(string str)
    {
        return;
        DoStatistic(string.Format("SingleBoardDoAgainButton|{0}", str));
    }

    /// <summary>
    /// 上报异界关卡在哪个房间死亡信息
    /// </summary>
    /// <param name="roomId"></param>
    public void DoStartAnotherWorldDie(int roomId)
    {
        DoStatistic(string.Format("YiJiePlayerDeath|roomid{0}", roomId));
    }

    /// <summary>
    /// 神秘商人
    /// </summary>
    /// <param name="str"></param>
    public void DoStartMysticalMerchant(int totalNum)
    {
        DoStatistic(string.Format("MysticalMerchantTotalNumberOfTriggers|{0}", totalNum));
    }

    /// <summary>
    /// 触发神秘商人类型
    /// </summary>
    /// <param name="strType">神秘商人类型</param>
    /// <param name="num">次数</param>
    public void DoStartMysticalMerchantType(string strType)
    {
        DoStatistic(string.Format("MysticalMerchanType|{0}",strType));
    }

    /// <summary>
    /// 神秘商人在地下城触发的次数
    /// </summary>
    /// <param name="dungeonName">地下城名字</param>
    /// <param name="merchantName">神秘商人名字</param>
    /// <param name="num">次数</param>
    public void DoStartMysticalMerchantDungeon(string dungeonName,string merchantName)
    {
        DoStatistic(string.Format("DungeonName|{0},MysticalMerchantType|{1}", dungeonName, merchantName));
    }

    /// <summary>
    /// 神秘商店商品刷新次数
    /// </summary>
    /// <param name="itemId"></param>
    public void DoStartMysticalShopGoods(int itemId)
    {
        DoStatistic(string.Format("MysticalShopGoodsID|{0}", itemId));
    }
    
    /// <summary>
    /// 神秘商店购买折扣价的商品
    /// </summary>
    /// <param name="buyItemId">折扣价商品ID</param>
    /// <param name="disCount">折扣</param>
    public void DoStartMysticalShopBuyDisCountGoodsNumber(int buyItemId, string moneyName, int disCount,int num)
    {
        DoStatistic(string.Format("MysticalShopBuyDisCountGoodsID|{0},ConsumptionMoneyType|{1},DisCountPRice|{2},MoneyNumber|{3}", buyItemId, moneyName,disCount, num));
    }

    /// <summary>
    /// 神秘商店购买每个商品消耗各种货币的数量
    /// </summary>
    /// <param name="buyItemId"></param>
    /// <param name="moneyName"></param>
    /// <param name="num"></param>
    public void DoStartMysticalShopBuyGoodsConsumptionOfMoney(int buyItemId,string moneyName,int num)
    {
        DoStatistic(string.Format("MysticalShopBuyDisCountGoodsID|{0},ConsumptionMoneyType|{1},MoneyNumber|{2}", buyItemId, moneyName, num));
    }

    /// <summary>
    /// 记录打开界面操作时长
    /// </summary>
    public void DoStartOpenFrameUseTime(string sFrameName,int iTime)
    {
        DoStatistic(string.Format("OpenFrmae|{0}Time|{1}s", sFrameName, iTime));
    }

    /// <summary>
    /// 记录打开某界面的操作
    /// </summary>
    /// <param name="sFrameName">界面名</param>
    /// <param name="sButtonName">操作的button或toggle</param>
    public void DoStartFrameOperation(string sFrameName,string sButtonName,string time)
    {
        DoStatistic(string.Format("FrameName|{0}ButtonName|{1}Time|{2}", sFrameName, sButtonName, time));
    }

    /// <summary>
    /// 组队界面击破深渊柱自动返回Toggle
    /// </summary>
    public void DoStartBreakThePitPillarAndReturnAutoMatically()
    {
        DoStatistic("BreakPitPillarReturnAutomatically");
    }

    /// <summary>
    /// 触发神秘商人用到的枚举
    /// </summary>
    public enum DungeonsType
    {
        NORMAL=0,
        YUANGU=1,
        HELL=8,
    }

    public string DungeonName(DungeonsType type)
    {
        string str = "";
        switch (type)
        {
            case DungeonsType.NORMAL:
                str = "普通地下城";
                break;
            case DungeonsType.YUANGU:
                str = "远古地下城";
                break;
            case DungeonsType.HELL:
                str = "深渊地下城";
                break;
        }

        return str;
    }
    //VOICE SDK 
    //游密语音计费埋点
    public void DoYouMiVoiceIM(YouMiVoiceSDKCostType type,string param)
    {
        //DoStatistic(string.Format("YouMi_IM_Voice|{0}|{1}", type.ToString(), param));

        //Logger.LogProcessFormat(string.Format("YouMi_IM_Voice|{0}|{1}", type.ToString(), param));
    }

    //pk场，记录结果的时候param填胜利，失败，平局
	public void DoStatPk(StatPKType type, string param="")
	{
		return;
		if (type == StatPKType.RESULT)
			DoStatistic(string.Format("{0}{1}", _getString(type), param));
		else
			DoStatistic(_getString(type));
	}

/*	//系统切换
	public void DoStatSystemSwitch(StatSystemType curSystemType, StatSystemType targetSystemType)
	{
		DoStatistic(string.Format("场景切换从{0}切换到{1}", _getString(curSystemType), _getString(targetSystemType)));
	}*/


    public void DoStatInBattleEx(StatInBattleType type, int dungeonID, int areaID, string content="")
    {
		return;
        DoStatistic(string.Format("[{0}] {1}, {2} : {3}", type, dungeonID, areaID, content));
    }

    public void DoStatDrugUse(int id, string content)
    {
		return;
        DoStatistic(string.Format("[{0}] {1}, {2}", StatItemUse.DUNGEON_BUFF_DRUG, id, content));
    }
		
    private bool _isOpen()
    {
        var tb = TableManager.instance.GetTableItem<ProtoTable.TestFunctionConfigTable>(1);

        if (null != tb && tb.Open)
        {
            return true;
        }

        return false;
    }

	public void DoStatistic(string content, StatType type=StatType.ROLE)
	{
        
		count++;

		Send(content, type);
	}

    private void NewDoStatistic(string name = "guide", string content = "")
    {
        // 新的埋点机制，发送到gate
        SceneClientLog log = new SceneClientLog();

        log.name = name;
        log.param1 = "1";
        log.param2 = content;
        log.param3 = "";

        NetManager.Instance().SendCommand(ServerType.GATE_SERVER, log);
    }

	/*
	public void UploadHotloadInfo()
	{
		string name = string.Format("hotupdate failed!!!|platform:{0}|_channel:{1}", GetPlatform(), Global.SDKChannelName[(int)Global.Settings.sdkChannel]);
		DoAbnormalStat(name);
	}*/

	//用于异常信息的发送
	public void DoAbnormalStat(string content)
	{
		Send(content, StatType.ABNORMAL);
	}

	public void DoStatClientData()
	{
		Dictionary<string, string> jsondic = new Dictionary<string, string>();
		jsondic.Add("gamever", VersionManager.GetInstance().Version());

        var data = ClientApplication.playerinfo.GetSelectRoleBaseInfoByLogin();

        if ( null != data)
        {
            jsondic.Add("roleid", data.roleId.ToString());
            jsondic.Add("level", data.level.ToString());
        }
        else 
        {
            jsondic.Add("roleid", "");
            jsondic.Add("level", "");
        }

        //string accountDefault = GameClient.PlayerLocalSetting.GetValue("AccountDefault") as string;
		jsondic.Add("uid",         ClientApplication.playerinfo.accid.ToString());


		jsondic.Add("_sid",        ClientApplication.playerinfo.serverID.ToString());
        jsondic.Add("_nettype",    Application.internetReachability.ToString());
        jsondic.Add("_ip",         /*UnityEngine.Network.player.ipAddress*/"0,0.0.0");
        jsondic.Add("_ip_ext",     /*UnityEngine.Network.player.externalIP*/"0,0.0.0");

        jsondic.Add("is_register", "false");
        jsondic.Add("_platId",     Application.platform.ToString());
        jsondic.Add("_channel",    Global.SDKChannelName[(int)Global.Settings.sdkChannel]);

        jsondic.Add("device_id",   SystemInfo.deviceUniqueIdentifier);

#if UNITY_IPHONE || UNITY_IOS
		jsondic.Add("_mfr",        string.IsNullOrEmpty(Device.vendorIdentifier) ? "" : Device.vendorIdentifier);
		jsondic.Add("_model",      Device.generation.ToString());
		jsondic.Add("_osver",      Device.systemVersion);
#else
		jsondic.Add("_mfr",        SystemInfo.deviceModel);
		jsondic.Add("_model",      SystemInfo.deviceName);
		jsondic.Add("_osver",      SystemInfo.operatingSystem);
#endif

		Send2Device(jsondic);

		clientInfoSended = true;
	}


    private void Send2Device(Dictionary<string, string> jsondic)
    {
        if (!_isOpen() || null == jsondic)
        {
            return ;
        }

		string json = jsondic.toJson();
		string url = string.Format("http://{0}/device", Global.STATISTIC_SERVER_ADDRESS);

        Http.SendPostRequest(url, json);
    }


	void Send(Dictionary<string, string> jsondic)
	{
        if (!_isOpen() || null == jsondic)
        {
            return ;
        }

		string json = jsondic.toJson();

		string url = string.Format("http://{0}:59527/game_process", Global.GLOBAL_SERVER_ADDRESS);


		if (Global.Settings.isUsingSDK)
			Http.SendPostRequest(url, json);
		else
			Http.SendPostRequest(url, json);
	}

	string GetPlatform()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return "IOS";
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			return "Andorid";
		}
		else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
		{
			return "PC";
		}
		else if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
		{
			return "MAC";
		}

		return "UNDEFINED";
	}

   public  void SendBatrayCount(string content, string guideID)
    {
        Dictionary<string, string> jsondic = new Dictionary<string, string>();
        jsondic.Add("gamever", VersionManager.GetInstance().Version());
        jsondic.Add("_account", ClientApplication.playerinfo.accid.ToString());
        jsondic.Add("roleid", GameClient.PlayerBaseData.GetInstance().RoleID.ToString());
        jsondic.Add("job", GameClient.PlayerBaseData.GetInstance().JobTableID.ToString());
        jsondic.Add("_sid", ClientApplication.playerinfo.serverID.ToString());
        jsondic.Add("_platId", GetPlatform());
        jsondic.Add("guide_id", guideID);
        jsondic.Add("guide_step", content);
        jsondic.Add("level", GameClient.PlayerBaseData.GetInstance().Level.ToString());
        jsondic.Add("vip", GameClient.PlayerBaseData.GetInstance().VipLevel.ToString());
        jsondic.Add("_channel", Global.SDKChannelName[(int)Global.Settings.sdkChannel]);

        string json = jsondic.toJson();

        //string url = string.Format("http://{0}:59527/guide_battle", Global.GLOBAL_SERVER_ADDRESS);
        string url = string.Format("http://{0}/guide_battle", Global.STATISTIC_SERVER_ADDRESS);

        if (Global.Settings.isUsingSDK)
            Http.SendPostRequest(url, json);
        else
            Http.SendPostRequest(url, json);
    }

	void Send(string content, StatType type=StatType.ROLE)
	{
        if (!_isOpen())
        {
            return ;
        }

		if (type == StatType.ROLE)
		{
			Dictionary<string, string> jsondic = new Dictionary<string, string>();
			jsondic.Add("gamever", VersionManager.GetInstance().Version());
			jsondic.Add("_account", ClientApplication.playerinfo.accid.ToString());
			jsondic.Add("roleid", GameClient.PlayerBaseData.GetInstance().RoleID.ToString());
			jsondic.Add("job", GameClient.PlayerBaseData.GetInstance().JobTableID.ToString());
			jsondic.Add("_sid", ClientApplication.playerinfo.serverID.ToString());
			jsondic.Add("_platId", GetPlatform());
			jsondic.Add("guide_id", "1");
			jsondic.Add("guide_step", content);
            jsondic.Add("level", GameClient.PlayerBaseData.GetInstance().Level.ToString());
            jsondic.Add("vip", GameClient.PlayerBaseData.GetInstance().VipLevel.ToString());
            jsondic.Add("_channel", Global.SDKChannelName[(int)Global.Settings.sdkChannel]);

			string json = jsondic.toJson();

			//string url = string.Format("http://{0}:59527/guide_battle", Global.GLOBAL_SERVER_ADDRESS);
			string url = string.Format("http://{0}/guide_battle", Global.STATISTIC_SERVER_ADDRESS) ;

			if (Global.Settings.isUsingSDK)
				Http.SendPostRequest(url, json);
			else
				Http.SendPostRequest(url, json);
		}
		else if (type == StatType.DEVICE)
		{
			Dictionary<string, string> jsondic = new Dictionary<string, string>();

#if UNITY_EDITOR
            jsondic.Add("id", SystemInfo.deviceUniqueIdentifier);
#elif UNITY_IPHONE || UNITY_IOS
            jsondic.Add("id", Device.advertisingIdentifier);
#else
            jsondic.Add("id", SystemInfo.deviceUniqueIdentifier);
#endif

			jsondic.Add("key", content);

			string json = jsondic.toJson();

			string url = string.Format("http://{0}:59527/device", Global.GLOBAL_SERVER_ADDRESS);

			if (Global.Settings.isUsingSDK)
				Http.SendPostRequest(url, json);
			else
				Http.SendPostRequest(url, json);
		}
		else if (type == StatType.ABNORMAL)
		{
			string url = string.Format("http://{0}", Global.STATISTIC2_SERVER_ADDRESS);
			Http.SendPostRequest(url, content);
		}
	}


	void Save()
	{
		
	}

	void Load()
	{
		
	}

#region _
	private string _getString(StatTaskType type)
	{
		if (type == StatTaskType.TASK_ACCEPT)
			return "TaskAccept";
		else if (type == StatTaskType.TASK_FIND_ROAD)
			return "TaskFindRoad";
		else if (type == StatTaskType.TASK_FINISH)
			return "TaskFinish";

		return "";
	}

	private string _getString(StatSkillPanelType type)
	{
		if (type == StatSkillPanelType.SKILL_LEVEL_UP)
			return "LevelUp";
		else if (type == StatSkillPanelType.SKILL_LEVEL_DOWN)
			return "LevelDown";
		else if (type == StatSkillPanelType.SKILL_CONFIG)
			return "Config";

		return "";
	}

	private string _getString(StatLevelChooseType type)
	{
		if (type == StatLevelChooseType.ENTER_SELECT)
			return "EenterSelect";
		else if (type == StatLevelChooseType.CHOOSE_LEVEL)
			return "ChooseLevel";
		else if (type == StatLevelChooseType.ENTER_LEVEL)
			return "EnterBattle";

		return "";
	}

	private string _getString(StatInBattleType type)
	{
		if (type == StatInBattleType.ENTER)
			return "EnterBattle";
		else if (type == StatInBattleType.CLEAR_ROOM)
			return "ClearRoom";
		else if (type == StatInBattleType.PASS_DOOR)
			return "PassDoor";
		else if (type == StatInBattleType.FINISH)
			return "FinishBattle";
		else if (type == StatInBattleType.USE_SKILL)
			return "UseSkill";
		else if (type == StatInBattleType.CLICK_RESULT)
			return "ClickResult";
		else if (type == StatInBattleType.CLICK_CARD)
			return "ClickCard";
		else if (type == StatInBattleType.CLICK_RETURN)
			return "ClickReturn";

		return "";
	}

	private string _getString(StatPKType type)
	{
		if (type == StatPKType.ENTER)
			return "EnterPKRoom";
		else if (type == StatPKType.MATCHING)
			return "StartMatching";
		else if (type == StatPKType.RESULT)
			return "Result";

		return "";
	}

	private string _getString(StatSystemType type)
	{
		if (type == StatSystemType.NONE)
			return "";
		else if (type == StatSystemType.LOGIN)
			return "Login";
		else if (type == StatSystemType.TOWN)
			return "Town";
		else if (type == StatSystemType.BATTLE)
			return "Battle";

		return "";
	}
#endregion

}


public class DataStatistics
{
    public int[] pingNums = new int[4];
    public int[] fpsNums = new int[3];

    public void Init()
    {
        for (int i = 0; i < pingNums.Length; i++)
        {
            pingNums[i] = 0;
        }
        for (int i = 0; i < fpsNums.Length; i++)
        {
            fpsNums[i] = 0;
        }
    }

    public void CollectPingStatistic(int ping)
    {
        if (ping <= 60)
        {
            pingNums[0]++;
        }
        else if (ping <= 100)
        {
            pingNums[1]++;
        }
        else if (ping <= 200)
        {
            pingNums[2]++;
        }
        else
        {
            pingNums[3]++;
        }

    }

    public void CollectFpsStatistic(int fps)
    {
        if (fps >= 26)
        {
            fpsNums[0]++;
        }
        else if (fps >= 21)
        {
            fpsNums[1]++;
        }
        else if (fps > 0)
        {
            fpsNums[2]++;
        }
    }
}
