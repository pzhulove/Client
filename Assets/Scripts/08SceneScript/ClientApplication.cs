using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using System.ComponentModel;
using ProtoTable;
using Network;
using GameClient;

public enum PlayerState
{
	NONE,
	LOGIN,
	LOGOUT
}

public class PlayerInfo
{
    public uint accid;                                      // 玩家登录账号id
//     public RacePlayerInfo[] raceplayers;                                // 单局内所有玩家信息(主要用于逻辑，也可以用于UI)

    public byte[] hashValue = new byte[20];
    public string param = "";
    public UInt64 session;
    public byte seat;
    public string currentGateServer;
    public string currentGateServerName;
	public string token;
	public string openuid;
    public string sdkLoginExt;

    public int age = 0; //返回年龄
    public Protocol.AuthIDType authType = Protocol.AuthIDType.AUTH_ADULT; //实名信息
    public uint serverID;
	public PlayerState state;

    public int curSelectedRoleIdx;                                  // 当前选中角色索引(这个只是客户端自己保存的数据)    
    public RoleInfo[] roleinfo;                                     // 玩家拥有的角色列表
    public byte[] apponintmentOccus;                                // 可预约职业的列表
    public uint appointmentRoleNum;                                 // 预约角色的数量
    public uint baseRoleFieldNum;                                   // 角色基础栏位数目
    public uint extendRoleFieldNum;                                 // 可扩展角色栏位数目
    public uint unLockedExtendRoleFieldNum;                         // 已解锁的可扩展角色栏位数目(包含新解锁栏位)
    public uint newUnLockExtendRoleFieldNum;                        // 新解锁的可扩展角色栏位数目
    public AdventureTeamExtraInfo adventureTeamInfo;                // 冒险队全部信息

    public RoleInfo defaultRoleInfo = new RoleInfo()
    {
        occupation = (byte)Global.Settings.iSingleCharactorID,
        name = "DNF大神"
    };                                                   // 默认角色信息
    
    public void SortRoleInfoList()
    {
        for (int i = 0; i < roleinfo.Length - 1; i++)
        {
            for (int j = i + 1; j < roleinfo.Length; j++)
            {
                if (roleinfo[i].isCollection < roleinfo[j].isCollection)
                {
                    RoleInfo info = roleinfo[i];
                    roleinfo[i] = roleinfo[j];
                    roleinfo[j] = info;
                }
            }
        }
    }

    //判断预约活动是否开启
    public bool GetHasApponintmentActiviti()
    {
       if (apponintmentOccus.Length != 0)
       {
            return true;
       }
        return false;
    }

    //判断角色是否是预约角色
    public bool GetRoleHasApponintmentOccu(int ID)
    {

        for (int i = 0; i < apponintmentOccus.Length; i++)
        {
            if (ID == apponintmentOccus[i])
            {
                return true;
            }
        }
        
        return false;
    }

    public bool GetRoleHasApponintmentOccu(RoleInfo info)
    {
        if (info.isAppointmentOccu != 1)
        {
            return false;
        }
        return true;
    }

    public bool GetSelectRoleHasPassFirstFight()
    {
        RoleInfo info = GetSelectRoleBaseInfoByLogin();

        if (null != info && info.newboot >= (uint)NewbieGuideTable.eNewbieGuideTask.FirstFight)
        {
            return true;
        }

        /*
        if (null != info && !string.IsNullOrEmpty(info.bootFlag))
        {
            BootMaskProperty mask = new BootMaskProperty();
            mask.flags = Convert.FromBase64String(info.bootFlag);
            return mask.CheckMask((uint)NewbieGuideTable.eNewbieGuideTask.FirstFight);

        }
        */
        return false;
    }
    
    public RoleInfo GetSelectRoleBaseInfoByLogin()
    {
        if (roleinfo != null)
        {
            if (curSelectedRoleIdx >= 0 && curSelectedRoleIdx < roleinfo.Length)
            {
                return roleinfo[curSelectedRoleIdx];
            }
            else
            {
                Logger.LogErrorFormat("curSelectRoleIdx = {0},roleinfo.Count = {1}", curSelectedRoleIdx, roleinfo.Length);
            }
        }
        else
        {
            return defaultRoleInfo;
        }

        return null;
    }

    /// <summary>
    /// 判断所有角色等级是否包含传进来的等级
    /// </summary>
    /// <returns></returns>
    public bool CheckAllRoleLevelIsContainsParamLevel(int level)
    {
        for (int i = 0; i < roleinfo.Length; i++)
        {
            RoleInfo info = roleinfo[i];
            
            if (info.level < level)
            {
                continue;
            }

            return true;
        }

        if (PlayerBaseData.GetInstance().Level >= level)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 检查所有角色是否可以购买礼包
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public bool CheckRoleIsBuyGift(int min, int max)
    {
        bool isFind = false;
        for (int i = 0; i < roleinfo.Length; i++)
        {
            RoleInfo info = roleinfo[i];

            if (info.roleId == PlayerBaseData.GetInstance().RoleID)
            {
                if (PlayerBaseData.GetInstance().Level < min || PlayerBaseData.GetInstance().Level > max)
                {
                    continue;
                }
                else
                {
                    return true;
                }
            }

            if (info.level < min || info.level > max)
            {
                continue;
            }

            isFind = true;
            break;
        }

        return isFind;
    }
}

public enum eAdminServerStatus
{
    None    = 0,
    Offline = 1,
    Ready   = 2,
    Buzy    = 3,
    Full    = 4,
}

public class AdminServerAddr
{
    public uint   id;
    public string name = "";
    public string ip   = "";
    public string dirSig = "";
    public ushort port;
    public eAdminServerStatus state;

}

public class ClientApplication
{
    public static PlayerInfo playerinfo = new PlayerInfo();

    /// <summary>
    /// 这个字段删掉=。=
    /// </summary>
    public static RacePlayerInfo[]  racePlayerInfo = new RacePlayerInfo[2];

    public static AdminServerAddr adminServer = new AdminServerAddr();

    public static SockAddr gateServer   = new SockAddr();
    public static SockAddr relayServer  = new SockAddr();
	public static string replayServer = "";
    public static string channelRankListServer="";
    public static string operateAdsServer = "";
    public static bool isEncryptProtocol = true;
#if APPLE_STORE
    public static bool isOpenNewReconnectAlgo = false;
    public static bool isOpenNewReportFrameAlgo = false;
#endif
	
    /// <summary>
    ///  装备图鉴评论服地址
    /// </summary>
    public static string commentServerAddr="";

    /// <summary>
    /// 红包排行榜地址
    /// </summary>
    public static string redPackRankServerPath = "";

    /// <summary>
    /// 举报页面地址
    /// </summary>
    public static string reportPlayerUrl = "";

    /// <summary>
    /// 转移包帐号信息查询地址
    /// </summary>
    public static string convertAccountInfoUrl = "";

    /// <summary>
    /// 问卷地址
    /// </summary>
    public static string questionnaireUrl = "";

    private static uint mServerStartTime = 0;
    public static uint serverStartTime
    {
        get
        {
            return mServerStartTime;
        }

        set
        {
            mServerStartTime = value;

            Logger.LogProcessFormat("[登录] 更新服务器开服时间 {0}", mServerStartTime);
        }
    }

    private static uint mVeteranReturn = 0;
    public static uint veteranReturn
    {
        get
        {
            return mVeteranReturn;
        }

        set
        {
            mVeteranReturn = value;
        }
    }

    public static UnityEngine.AsyncOperation ops;
    public static UnityEngine.AsyncOperation LoadLevelAsync(int index)
    {
        if(ops != null)
        {
            ops.allowSceneActivation = true;
        }

        ops = UnityEngine.Application.LoadLevelAsync(index);

        return ops;
    }

    public static void DisconnectGateServerAtLogin()
    {
        GameClient.ClientReconnectManager.instance.canRelogin = false;

        NetManager.Instance().Disconnect(ServerType.GATE_SERVER);

        adminServer.ip = string.Empty;
        adminServer.port = 0;
        gateServer.ip = string.Empty;
        gateServer.port = 0;
    }

    public static bool HasRoles()
    {
        return null != playerinfo && null != playerinfo.roleinfo && playerinfo.roleinfo.Length > 0;
    }
}



