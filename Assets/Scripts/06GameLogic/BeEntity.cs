using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using GameClient;
using System.Diagnostics;
using ProtoTable;

public enum PositionType
{
    OVERHEAD = 0,
    BODY,
    ORIGIN,
    ORIGIN_BUFF
}

public enum ActionState
{
    AS_NONE = -1,
    AS_IDLE = 0,
    AS_ATTACK = 1,
    AS_WALK = 2,
    AS_RUN = 3,
    AS_HURT = 4,
    AS_JUMP = 5,
    AS_JUMPBACK = 6,
    AS_RUNATTACK = 7,
    AS_JUMPATTACK = 8,
    AS_FALL = 9,
    AS_FALLCLICK = 10,
    AS_FALLGROUND = 11,
    AS_SKILL = 12,
    AS_BUSY = 13,   //繁忙状态
    AS_CASTSKILL = 14,
    AS_GRABBED = 15,
    AS_DEAD = 16,
    AS_GETUP = 17, //蹲伏+起身
    AS_BIRTH = 18, //出生状态  
    AS_WIN = 19,
    AS_ROLL = 20,
    AS_MAX = 21,
}

public enum ActionType
{
    ActionType_JumpUp = 0,
    ActionType_JumpDown = 1,
    ActionType_Getup = 2,
    ActionType_IDLE = 3,
    ActionType_STANDFIGHT = 4,
    ActionType_WALK = 5,
    ActionType_RUN = 6,
    ActionType_JUMP = 7,
    ActionType_RISE = 8,
    ActionType_SINK = 9,
    ActionType_DOWN = 10,
    ActionType_PARRY = 11,
    ActionType_HURT = 12,
    ActionType_HURT1 = 13,
    ActionType_HURTMAX = 14,
    ActionType_RUNATTACK = 15,
    ActionType_JUMPATTACK = 16,
    ActionType_FALL_DOWN = 17,
    ActionType_FALL_CHANGE = 18,
    ActionType_FALL_UP = 19,
    ActionType_FALL_GROUND = 20,
    ActionType_SKILL_BASE = 21,
    ActionType_DEAD = 22,
    ActionType_BIRTH = 23,
    ActionType_WIN = 24,
    ActionType_SpecialIdle = 25,
    ActionType_Roll = 26,
    ActionType_StartWalk = 27,
    ActionType_EndWalk = 28,
    ActionType_SpecialIdle02 = 29,
    ActionType_FALL_GROUND01 = 30,
    ActionType_NUM,
};


public enum DeadType
{
    /// <summary>
    /// 正常
    /// </summary>
    NORMAL = 0, 

    /// <summary>
    /// 暴击
    /// </summary>
    CRITICAL,   

    /// <summary>
    /// 秒杀
    /// </summary>
    IMMEDIATE,  

    /// <summary>
    /// boss
    /// </summary>
    BOSS,       

    /// <summary>
    /// 玩家
    /// </summary>
    HERO
}

public enum EventCommand
{
    EVENT_COMMAND_CHANGE = 1,
    EVENT_COMMAND_ATTACK,
    EVENT_COMMAND_UPSTOP,
    EVENT_COMMAND_TOUCHGROUND,
    EVENT_COMMAND_RELEASE_SKILL_BUTTON,
    EVENT_COMMAND_PRESS_JOYSTICK,
    EVENT_COMMAND_RELEASE_JOYSTICK,
    EVENT_COMMAND_PRESS_BUTTON_AGAIN,
}

public enum CommandMove
{
    COMMAND_MOVE_X = 0,
    COMMAND_MOVE_X_NEG,
    COMMAND_MOVE_Y,
    COMMAND_MOVE_Y_NEG,
    COMMAND_MOVE_DIRECTION,
    COMMAND_MOVE_MAX
};

public enum AStateTag
{
    AST_NULLTAG = 0,

    /// <summary>
    /// 受控
    /// </summary>
    AST_CONTROLED = 1 << 0, 

    /// <summary>
    /// 繁忙
    /// </summary>
    AST_BUSY = 1 << 1, 

    AST_LOCKZ = 1 << 2,
    AST_NORMALSKILL = 1 << 3,
}

public enum AState
{
    /// <summary>
    /// 浮空
    /// </summary>
    ACS_FALL = 1 << 0,          

    /// <summary>
    /// 跳跃
    /// </summary>
    ACS_JUMP = 1 << 1,          

    /// <summary>
    /// 倒地
    /// </summary>
    AST_FALLGROUND = 1 << 2,    

    /// <summary>
    /// 从地面掉下场景
    /// </summary>
    AST_DROPSCENE = 1 << 3,     

    /// <summary>
    /// 后跳
    /// </summary>
    ACS_JUMPBACK = 1 << 4,      
};
public enum CAMP
{
    CAMP_A = 0,
    CAMP_B
};

public enum SpeedCear
{
    SPEEDCEAR_X = 1 << 0,
    SPEEDCEAR_Y = 1 << 1,
    SPEEDCEAR_Z = 1 << 2,
    SPEEDCEAR_XY = SPEEDCEAR_X | SPEEDCEAR_Y,
    SPEEDCEAR_XYZ = SPEEDCEAR_XY | SPEEDCEAR_Z,
};


//entity life state
public enum EntityLifeState
{
    /// <summary>
    /// 没有创建
    /// </summary>
    ELS_NULL = 0,

    ELS_ERROR,

    /// <summary>
    /// 存活
    /// </summary>
    ELS_ALIVE,          

    /// <summary>
    /// 死亡
    /// </summary>
    ELS_DEAD,           

    /// <summary>
    /// 可以移除
    /// </summary>
    ELS_CANREMOVE,		

    ELS_REMOVED
};


// public enum BDFrameDataTag
// {
//     FDT_NULL = 0,
//     FDT_GRAPTRY = 1 << 0,
//     FDT_GRAPED = 1 << 1,
//     FDT_GRAPEDDECIDE = 1 << 2,
//     FDT_GRAPRELEASE = 1 << 3,
// };


public struct ActionDesc
{
    public ActionType actionType;
    public int timeout;
}


public class BeEntity
{
#if UNITY_EDITOR
    protected List<string> m_RecordHurtIdList = new List<string>();
    public List<string> RecordHurtIdList
    {
        get
        {
            return m_RecordHurtIdList;
        }
    }
#endif

    protected readonly static string[] StaticActionConfigNames =
    {
        "Jump_up",      //0
        "Jump_down",    //1
        "Getup",        //2
        "Idle",         //3
        "Idle_special", //4
        "Walk",         //5
        "Run",          //6
        "-",            //7             //后跳
        "Jump_up_loop",      //8
        "Jump_down_loop",    //9
        "Houtiao",      //10            //--先不配
        "parry",        //11            //hou tiao
        "Beiji01",      //12            //被击01
        "Beiji02",      //13            //被击02
        "Beiji01",      //14            //先不配
        "runattack",    //15            //--先不配，按技能处理
        "jumpattack",   //16            //--先不配
        "Float_down",   //17            //浮空下落
        "fallchange",   //18                        
        "Float_up",     //19            //浮空上升
        "Daodi",        //20
        "skillbase",    //21            //先不配
        "Dead",         //22
        "Birth",        //23
        "Win",          //24
		"Idle02",       //25
        "Roll",         //26
        "StartWalk",    //27            开始行走
        "EndWalk",      //28            结束行走
        "Idle03",       //29
        "Daodi01",      //30
    };

    private string[] ActionConfigNames = new string[StaticActionConfigNames.Length];
    private void _ResetActionConfigNames()
    {
        if (null == ActionConfigNames)
        {
            return;
        }

        for (int i = 0; i < ActionConfigNames.Length; ++i)
        {
            ActionConfigNames[i] = null;
        }
    }

    public static int MAX_STATE_TAG_NUM = 64;
    //public static DBox rkLastIntersetBox;

    //protected THandle m_kHandle;
    //protected float m_fLifeTime;
    //protected float m_fTimeDelayCreate;
    //protected float m_fTimeDelayRemove;
    public int m_iEntityLifeState;
    public int m_iRemoveTime = 0;

    public GeActorEx m_pkGeActor;
    public BeAttachFramesProxy attachmentproxy;

    protected VInt3 m_kPosition;
    protected VInt3 m_kBlockPosition;

    protected VInt m_fScale;
    protected VFactor m_fZdimScaleFactor;
    protected int m_nBlockWidth = 1;
    protected int m_nBlockHeight = 1;
    protected static readonly byte[] DEFAULT_BLOCK_DATA = new byte[] { 1 };
    protected byte[] m_byteBlockData = DEFAULT_BLOCK_DATA;
    //protected bool m_birthPosFlag = false;        //出生坐标标志
    //protected VInt3 m_birthPos;                    //出生坐标

    protected bool m_bFaceLeft;
    protected byte m_eventArea = 0;
    //僵直使用 最大位移距离
    public VInt FrozenDisMax = VInt.zero;
    public VInt3 FrozenStartDis = VInt3.zero;
    protected List<DelayCallUnitHandle> delayCallUnitList = new List<DelayCallUnitHandle>();    //用于记录重复造成伤害
    protected List<int> delayCallUnitHurtIdList = new List<int>();                              //用于记录重复伤害的触发效果ID
    protected bool mIsBeGraped = false;  //为解决先抓取后蹲伏的bug 不动原来代码的基础上增加一个变量，判断能否蹲伏的额外一句
    public VInt clickZSpeed = 0;
    
    // 攻击流程ID，用于判定是否是一个攻击流程（处理脱手技能）
    // Actor：在技能开始时生成唯一ID，技能结束时置0.
    // Projectile：终身不变
    // 特殊的直接调用攻击与创建实体暂不支持，可自行调用时添加
    private uint m_AttackProcessId;
    public uint AttackProcessId
    {
        get => m_AttackProcessId;
        set => m_AttackProcessId = value;
    }
    public bool IsBeGrabbed
    {
        get { return mIsBeGraped; }
        set { mIsBeGraped = value; }
    }
    public VInt forceX
    {
        set
        {
            m_fForceX = value;
        }
        get
        {
            return m_fForceX;
        }
    }
    public VInt forceY
    {
        set
        {
            m_fForceY = value;
        }
        get
        {
            return m_fForceY;
        }
    }
    public VInt forceXAcc
    {
        set
        {
            m_fForceXAcc = value;
        }
        get
        {
            return m_fForceXAcc;
        }
    }
    public VInt forceYAcc
    {
        set
        {
            m_fForceYAcc = value;
        }
        get
        {
            return m_fForceYAcc;
        }
    }

    public int forceXAccTimer
    {
        set
        {
            m_iForceXAccTimer = value;
        }
        get
        {
            return m_iForceXAccTimer;
        }
    }

    public int tempForceXAccTimer
    {
        set
        {
            m_iTempForceXAccTimer = value;
        }
        get
        {
            return m_iTempForceXAccTimer;
        }
    }

    public int forceZAccTimer
    {
        set
        {
            m_iForceZAccTimer = value;
        }
        get
        {
            return m_iForceZAccTimer;
        }
    }

    public int tempForceZAccTimer
    {
        set
        {
            m_iTempForceZAccTimer = value;
        }
        get
        {
            return m_iTempForceZAccTimer;
        }
    }

    public int hurtAction
    {
        set
        {
            m_iHurtAction = value;
        }
        get
        {
            return m_iHurtAction;
        }
    }

    protected VInt m_fForceX;
    protected VInt m_fForceY;
    protected VInt m_fForceXAcc;
    protected VInt m_fForceYAcc;
    protected int m_iForceXAccTimer;
    protected int m_iTempForceXAccTimer;
    protected int m_iForceZAccTimer;
    protected int m_iTempForceZAccTimer;
    protected int m_iHurtAction;

    public bool fallForGrab;

    public VInt3 speedConfig
    {
        set
        {
            m_kSpeedConfig = value;
        }
        get
        {
            return m_kSpeedConfig;
        }
    }

    protected VInt3 m_kSpeedConfig;
    public VInt3 walkSpeed;
    public VInt3 runSpeed;
    public ActionType walkAction = ActionType.ActionType_WALK;
    public ActionType runAction = ActionType.ActionType_RUN;

    //!! 表现用Float 可以不用转整数
    public float walkSpeedFactor = 1.0f;
    public float runSpeedFactor = 1.0f;
    public bool hasDoublePress = false; //是否用双击摇杆设置
    public bool hasRunAttackConfig = false;//是否开启前冲按键
    public bool attackReplaceLigui = false;//是否开启普攻替代里鬼
	public bool paladinAttackCharge = true;//驱魔师普攻是否支持蓄力
	public bool canUseCrystalSkill = true;//自动战斗的时候能否使用40级无色技能
    public bool backHitConfig = false;
    public bool autoHitConfig = false;
    public byte chaserSwitch = 0;

    public bool IsChaserSwitch(int type)
    {
        return ((1 << type) & chaserSwitch) != 0;
    }
    
    private int mLastAttackerId = 0;
#if !LOGIC_SERVER
    protected List<uint> m_PhaseDeleteAudioList = null;//new List<uint>();
    public List<uint> PhaseDeleteAudioList { get { if (m_PhaseDeleteAudioList==null) m_PhaseDeleteAudioList = new List<uint>(); return m_PhaseDeleteAudioList; } set { m_PhaseDeleteAudioList = value; } }

    protected List<uint> m_FinishDeleteAudioList = null;//new List<uint>();
    public List<uint> FinishDeleteAudioList { get { if (m_FinishDeleteAudioList==null) m_FinishDeleteAudioList = new List<uint>();  return m_FinishDeleteAudioList; } set { m_FinishDeleteAudioList = value; } }
#endif

    public int lastAttackerId { get { return mLastAttackerId; } }
    // public VInt lastMoveXSpeed
    // {
    //     get
    //     {
    //         return m_lastMoveXSpeed;
    //     }
    //     set
    //     {

    //         //if (m_lastMoveXSpeed != value)
    //         //	Logger.LogErrorFormat("last move x {0} => {1}", m_lastMoveXSpeed, value);

    //         m_lastMoveXSpeed = value;
    //     }
    // }

    // public VInt m_lastMoveXSpeed;


    // public VInt lastMoveYSpeed
    // {
    //     get
    //     {
    //         return m_lastMoveYSpeed;
    //     }
    //     set
    //     {
    //         //if (m_lastMoveYSpeed != value)
    //         //	Logger.LogErrorFormat("last move y {0} => {1}", m_lastMoveYSpeed, value);

    //         m_lastMoveYSpeed = value;
    //     }
    // }

    //public VInt m_lastMoveYSpeed;


    public VInt moveXSpeed
    {
        get
        {
            return m_fMoveXSpeed;
        }
        set
        {
            m_fMoveXSpeed = value;
        }
    }

    public VInt moveYSpeed
    {
        get
        {
            return m_fMoveYSpeed;
        }
        set
        {
            m_fMoveYSpeed = value;
        }
    }

    public VInt moveZSpeed
    {
        get
        {
            return m_fMoveZSpeed;
        }
        set
        {

            /*			if (sgGetCurrentState() == (int)ActionState.AS_BUSY && !inUpdatePosition && value == 0)
                        {
                            Logger.LogErrorFormat("{0} {1} set speed {2}=>{3} t:{4}", m_iID, GetName(), m_fMoveZSpeed, value, Time.realtimeSinceStartup);
                        }*/

            if (!inUpdatePosition && m_fMoveZSpeed != 0 && value == 0 && (this as BeActor) != null)
                resetMoveDirty = true;

            m_fMoveZSpeed = value;
        }
    }

    public VInt3 lastMoveSpeed;

    public VInt moveXAcc
    {
        get
        {
            return m_fMoveXAcc;
        }
        set
        {
            m_fMoveXAcc = value;
        }
    }

    public VInt moveYAcc
    {
        get
        {
            return m_fMoveYAcc;
        }
        set
        {
            m_fMoveYAcc = value;
        }
    }

    public VInt moveZAcc
    {
        get
        {
            return m_fMoveZAcc + m_fMoveZAccExtra;
        }
        set
        {
            m_fMoveZAcc = value;
        }
    }

    //技能配置文件中额外增加的Z轴加速度
    public VInt moveZAccExtra
    {
        get
        {
            return m_fMoveZAccExtra;
        }
        set
        {
            m_fMoveZAccExtra = value;
        }
    }

    protected VInt m_fMoveXSpeed;
    protected VInt m_fMoveXAcc;
    protected VInt m_fMoveYSpeed;
    protected VInt m_fMoveYAcc;
    protected VInt m_fMoveZSpeed;
    protected VInt m_fMoveZAcc;
    protected VInt m_fMoveZAccExtra;
    public VInt3 extraSpeed;

    protected VFactor m_fMoveXSpeedRate = VFactor.one;
    protected VFactor m_fMoveYSpeedRate = VFactor.one;
    protected bool onlyMoveFacedDir = false;
    protected bool onlyMoveFaceDirOpposite = false;
    protected bool keepXSkillSpeed = false;
    protected bool skillFreeTurnFace = false;
    private bool mPositionDirty;
    protected bool m_bGraphicPositionDirty;
    protected bool m_bPositionDirty
    {
        get
        {
            return mPositionDirty;
        }

        set
        {
            mPositionDirty = value;
            if (value)
            {
                m_bGraphicPositionDirty = true;
            }
        }
    }
    protected bool m_bHaveMoveCmd;
    protected bool[] m_vkMoveCmd = new bool[(int)CommandMove.COMMAND_MOVE_MAX];
    protected bool m_bCmdDirty;
    protected bool m_bLockMoveCmd = false;
    // public void LockMoveCmd(bool bLock)
    // {
    //     if (m_bLockMoveCmd != bLock)
    //         m_bCmdDirty = true;
    //     m_bLockMoveCmd = bLock;
    // }


    public VFactor skillAttackScale;

    protected short m_Degree;
    protected BeStatesGraph m_pkStateGraph;
    protected int stateStart;
    protected bool sgStarted = false;
    protected List<BeEntity> m_vHurtEntity = new List<BeEntity>();

    protected SeFlag m_kStateTag = new SeFlag(MAX_STATE_TAG_NUM);
    //protected string m_kDamageType;

    protected bool isDead = false;
    public void SetIsDead(bool flag)
    {
        isDead = flag;
    }
    public int GetAllStatTag()
    {
        return m_kStateTag.GetAllFlag();
    }
    // TODO move to the GeActor
    GeShockEffect m_kShockEffect = new GeShockEffect();
    protected int m_iCurrentLogicFrame;
    protected float m_fCurrentLogicFrame;



    public bool dontSetFace = false;
    public BeAIManager aiManager;

    public int m_iCamp;
    public int m_iID;
    public int m_iResID;
    protected bool m_bCanBeAttacked;

    protected bool pause = false;
    protected int pausetime;

    protected BDEntityRes m_cpkEntityInfo1 = new BDEntityRes();
    protected BDEntityRes m_cpkEntityInfo2 = new BDEntityRes();//第二形态单独存一份配置文件

    public bool twoStateMode;//双形态玩法测试，先写在这里
    public bool tempTwoStateMode;

    public BDEntityRes m_cpkEntityInfo
    {
        get
        {
            return twoStateMode ? m_cpkEntityInfo2 : m_cpkEntityInfo1;
        }
    }

    public BDEntityActionInfo m_cpkCurEntityActionInfo;
    public bool actionLooped = false;
    public BDEntityActionFrameData m_cpkCurEntityActionFrameData;

    protected List<DBoxImp2> m_vkCurWorldAttackBox = new List<DBoxImp2>();
    protected List<DBoxImp2> m_vkCurWorldDefenseBox = new List<DBoxImp2>();

    // public BDDBoxData CurWorldDefenseBox
    // {
    //     get { return m_vkCurWorldDefenseBox; }
    // }

    public int hurtCount = 0;

    public BeEntityData attribute; //角色的属性信息
    public BeStateControl stateController;
    public bool hasHP = true; //是否有HP血条

    public int timeAcc = 0;

    public DelayCaller delayCaller = new DelayCaller();

    protected bool restrainPosition = false;
    public bool pkRestrainPosition = false;
    public VInt2 pkRestrainRangeX;

    //bool reachAreaLimited = false;

    public VFactor moveSpeedFactor = VFactor.one;

    protected VInt3 posBeforeGrab = VInt3.zero;

    protected List<ActionDesc> actionQueue = new List<ActionDesc>();
    int actionTimeout = 0;
    float queuedActionSpeed = 1.0f;

    //protected DelayCallUnit repeatUnit = null;

    public string beHitEffect = "-"; //角色特有的被击特效
    public string hitEffect = "-";//角色特有的击中特效

    public bool isFloating = false;
    public VInt floatingHeight = 0;

    protected BeEntity owner = null;

    public VInt boxRadius = 0;

    public bool showDamageNumber = true;

    public BeActionManager actionManager = new BeActionManager();
    public VInt3 savedPosition;

    public bool playedExpDead = false;

    //hp mp回复
    int recoverInterval = 1000;
    int recoverTimeAcc = 0;

    //死亡类型
    public DeadType deadType = DeadType.NORMAL;
    protected bool needDead = false;
    public bool dontDelete = false;

    public bool needHitShader = false;

    public bool loadCommonSkill = false;

    public bool doublePressRun = false;
    public bool doublePressRunLeft = false;

    public bool changeToNoBlock = false;

    public int defaultHitSFXID = 0;
    public ProtoTable.SoundTable defaultHitSFXData = null;

    protected bool resetMoveDirty = false;
    
    //同时播放的被击特效数量
    public int currentHitEffectNum = 0;
    public int totalHitEffectNum = 5;

    //同时播放的被击音效数量
    public int currentHitSFXNum = 0;
    public int totalHitSFXNum = 5;
    
    public bool absoluteBroken = false;     //修炼场绝对破招

    public int actionCreateEffectNum = 0;



    protected BeEventManager m_EventManager;
    public BeEventManager EventManager { get { return m_EventManager; } }

    //AI 
    /*
    public bool hasAI = false;
	public bool pauseAI = false;
	*/

    public bool hasAI
    {
        set
        {
            _hasAI = value ? 1 : 0;
        }
        get
        {
            return _hasAI > 0 ? true : false;
        }
    }

    public bool pauseAI
    {
        set
        {
            _pauseAI = value ? 1 : 0;
        }
        get
        {
            return _pauseAI > 0 ? true : false;
        }
    }

    protected CrypticInt32 _hasAI = 0;
    protected CrypticInt32 _pauseAI = 0;

    /*************************************************************************************/
    public static bool CheckIntersectEx(List<DBoxImp2> rkWorldAttackData, int iAttackSize, List<DBoxImp2> rkWorldDefenseData, int iDefenseSize, ref DBox rkBoxOut)
    {
        for (int i = 0; i < iAttackSize; ++i)
        {
            for (int j = 0; j < iDefenseSize; ++j)
            {
                var rkAttackBox = rkWorldAttackData[i].vWorldBox;
                var rkDefenseBox = rkWorldDefenseData[j].vWorldBox;

                if (rkAttackBox.intersects(rkDefenseBox))
                {
                    rkAttackBox.getIntersects(rkDefenseBox, ref rkBoxOut);
                    return true;
                }
            }
        }

        return false;
    }

    // public static bool CheckIntersect3Ex(BDDBoxData rkWorldAttackData, int iAttackSize, BDDBoxData rkWorldDefenseData, int iDefenseSize, ref DBox3 rkBoxOut)
    // {
    //     for (int i = 0; i < iAttackSize; ++i)
    //     {
    //         for (int j = 0; j < iDefenseSize; ++j)
    //         {
    //             var rkAttackBox = rkWorldAttackData.vBox[i].vWorld3Box;
    //             var rkDefenseBox = rkWorldDefenseData.vBox[j].vWorld3Box;

    //             if (rkAttackBox.intersects(rkDefenseBox))
    //             {
    //                 rkAttackBox.getIntersects(rkDefenseBox, ref rkBoxOut);
    //                 return true;
    //             }
    //         }
    //     }
    //     return false;
    // }

    /*************************************************************************************/

    public void Pause(int time, bool hitEffectPause = true)
    {
        pause = true;
        pausetime = time;

#if !LOGIC_SERVER
        if (m_pkGeActor != null)
        {
            m_pkGeActor.Pause((int)GeEntity.GeEntityRes.All, hitEffectPause);
        }
        if (m_kShockEffect != null)
        {
            m_kShockEffect.Stop();
        }
#endif
    }

    /// <summary>
    /// 停止抖动
    /// </summary>
    public void StopShock()
    {
#if !LOGIC_SERVER
        if (m_kShockEffect != null)
        {
            m_kShockEffect.Stop();
        }
#endif
    }

    public void Resume()
    {
        if (pause)
        {
            pause = false;
            if (m_pkGeActor != null)
            {
                m_pkGeActor.Resume();
            }
        }
    }

    public bool IsPause()
    {
        return pause;
    }

    public bool _updatePause(int iDeltatime)
    {
        if (pause)
        {
            pausetime -= iDeltatime;

            if (pausetime <= 0)
            {
                pause = false;

                if (m_pkGeActor != null)
                {
                    m_pkGeActor.Resume();
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    public string GetActionNameBySkillID(int sID)
    {


        return GetSkillDataNameByID(sID);
    }

    public BDEntityActionInfo GetActionInfoBySkillID(int skillID)
    {
        BDEntityActionInfo info = null;

        string actionName = GetActionNameBySkillID(skillID);

        if (m_cpkEntityInfo.HasAction(actionName))
            info = m_cpkEntityInfo._vkActionsMap[actionName];

        return info;
    }

    public string GetSkillDataNameByID(int skillID)
    {
        if (m_cpkEntityInfo.skillData.ContainsKey(skillID))
            return m_cpkEntityInfo.skillData[skillID];

        return null;
    }

    public BeEntity(int iResID, int iCamp, Int64 iID)
    {
        //m_vkCurWorldAttackBox = new BDDBoxData();
        for (int i = 0; i < 10; ++i)
        {
            m_vkCurWorldAttackBox.Add(new DBoxImp2());
        }

       // m_vkCurWorldDefenseBox = new BDDBoxData();
        for (int i = 0; i < 10; ++i)
        {
            m_vkCurWorldDefenseBox.Add(new DBoxImp2());
        }

        stateController = new BeStateControl(this);
        actionManager.Init();

        InitReset(iResID, iCamp, (int)iID);
    }

    public virtual void InitReset(int iResID, int iCamp, int iID)
    {
        m_iCamp = iCamp;
        m_iID = (int)iID;
        m_iResID = iResID;

        m_bFaceLeft = false;
        m_iCurrentLogicFrame = -1;

        moveZAcc = (VInt)Global.Settings.gravity;
        //	m_fScale = 1.0f;
        //	m_fZdimScaleFactor = 1.0f;
        m_fScale = VInt.one;
        m_fZdimScaleFactor = VFactor.one;
        moveXAcc = VInt.zero;
        moveYAcc = VInt.zero;

        m_bCanBeAttacked = true;
        m_iEntityLifeState = (int)EntityLifeState.ELS_NULL;

        runSpeed = new VInt3(Global.Settings.runSpeed);
        walkSpeed = new VInt3(Global.Settings.walkSpeed);

        speedConfig = walkSpeed;

        m_iRemoveTime = 0;
        //m_pkGeActor = null;
        //attachmentproxy = null;
        m_kPosition = VInt3.zero;
        m_kBlockPosition = VInt3.zero;

        forceX = VInt.zero;
        forceY = VInt.zero;
        walkSpeedFactor = 1f;
        runSpeedFactor = 1f;
        hasDoublePress = false;
        chaserSwitch = 0;
        hasRunAttackConfig = false;
        extraSpeed = VInt3.zero;
        m_fMoveXSpeedRate = VFactor.one;
        m_fMoveYSpeedRate = VFactor.one;
        onlyMoveFacedDir = false;
        onlyMoveFaceDirOpposite = false;
        skillFreeTurnFace = false;
        mPositionDirty = false;
        m_bGraphicPositionDirty = true;
        m_bHaveMoveCmd = false;
        m_bCmdDirty = false;
        m_bLockMoveCmd = false;
        m_Degree = 0;
        stateStart = 0;
        sgStarted = false;
        m_vHurtEntity.Clear();
        m_kStateTag.Clear();
        isDead = false;
        m_fCurrentLogicFrame = 0f;
        hasAI = false;
        pauseAI = false;
        dontSetFace = false;
        m_bCanBeAttacked = true;
        pause = false;
        pausetime = 0;

        m_cpkCurEntityActionInfo = null;
        actionLooped = false;
        m_cpkCurEntityActionFrameData = null;
        hurtCount = 0;
        attribute = null;
        stateController.Reset();
        hasHP = true;
        timeAcc = 0;
        delayCaller.Clear();
        restrainPosition = false;
        pkRestrainPosition = false;
    //    reachAreaLimited = false;
        moveSpeedFactor = VFactor.one;
        posBeforeGrab = VInt3.zero;
        actionQueue.Clear();
        actionTimeout = 0;
        queuedActionSpeed = 1.0f;
        //repeatUnit = null;
        skillAttackScale = VFactor.one;
        beHitEffect = "-";
        hitEffect = "-";
        isFloating = false;
        floatingHeight = 0;
        owner = this;
        boxRadius = VInt.zero;
        showDamageNumber = true;
        savedPosition = VInt3.zero;
        playedExpDead = false;
        recoverTimeAcc = 0;
        recoverInterval = 1000;
        deadType = DeadType.NORMAL;
        needDead = false;
        dontDelete = false;
        needHitShader = false;
        loadCommonSkill = false;
        doublePressRun = false;
        doublePressRunLeft = false;
        changeToNoBlock = false;
        defaultHitSFXID = 0;
        defaultHitSFXData = null;
        resetMoveDirty = false;
        currentHitEffectNum = 0;
        delayCallUnitList.Clear();
        delayCallUnitHurtIdList.Clear();

        ClearMoveSpeed((int)SpeedCear.SPEEDCEAR_X | (int)SpeedCear.SPEEDCEAR_Y | (int)SpeedCear.SPEEDCEAR_Z);

        if (m_pkStateGraph != null)
            m_pkStateGraph.Reset();

        _ResetActionConfigNames();
    }

    protected BeScene currentBeScene;
    public void SetBeScene(BeScene beScene)
    {
        this.currentBeScene = beScene;
    }
    public BeScene CurrentBeScene
    {
        get
        {
            return currentBeScene;
        }
    }

    public BaseBattle CurrentBeBattle
    {
        get
        {
            BaseBattle battle = null;
            if (currentBeScene != null)
                battle = currentBeScene.mBattle;
            return battle;
        }
    }

    public FrameRandomImp FrameRandom
    {
        get
        {
#if !SERVER_LOGIC

            if (CurrentBeBattle == null)
            {
                return BeSkill.randomForTown;
            }


#endif
            if (CurrentBeBattle == null)
            {
                Logger.LogError("CurrentBeBattle is null");
            }

            return CurrentBeBattle.FrameRandom;
        }
    }


    public BattleType battleType
    {
        get
        {
            BattleType battleType = BattleType.Dungeon;
            if (owner.CurrentBeBattle != null)
                battleType = owner.CurrentBeBattle.GetBattleType();

            return battleType;
        }

    }

    ~BeEntity()
    {
    }

    public void Create(BeStatesGraph pkStateGraph, int iStartState, bool isSceneObj = false, bool loadCommonSkill = false, bool useCube = false)
    {
        if (GetLifeState() != (int)EntityLifeState.ELS_NULL)
            return;

        m_iEntityLifeState = (int)EntityLifeState.ELS_ALIVE;
        isDead = false;

        _LoadBlockData();
        if ((this as BeProjectile) != null && m_pkGeActor != null)
        {
            //m_pkGeActor = CurrentBeScene.currentGeScene.GetRecycledActor();
            m_pkGeActor.RecreateForProjectile(useCube);
        }
        else
        {
            m_pkGeActor = CurrentBeScene.currentGeScene.CreateActor(m_iResID, 0, 0, isSceneObj, true, true, useCube);
            if(m_pkGeActor != null)
            {
                m_pkGeActor.SuitAvatar();
            }
        }

        if (m_pkGeActor == null)
        {
            Logger.LogErrorFormat("资源ID {0} 角色创建失败", m_iResID);
        }

        this.loadCommonSkill = loadCommonSkill;



        if (attachmentproxy == null)
        {
            attachmentproxy = new BeAttachFramesProxy()
            {
                actor = m_pkGeActor
            };
        }

        m_pkGeActor.entity = this;

#if !LOGIC_SERVER
        if (m_pkGeActor != null)
        {
            m_pkGeActor.AddAnimPackage("AnimPack02");
            m_kShockEffect.Init(m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Child), 1);
            if ((this as BeObject) != null)
            currentBeScene.currentGeScene.AddToColorDescList(m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Actor));
        }
#endif

        if (pkStateGraph != null)
        {
            SetStateGraph(pkStateGraph, iStartState);
        }

        if (!_onCreate())
        {
            if (m_cpkEntityInfo._vkActionsMap.Count <= 0 && m_pkGeActor != null)
                m_pkGeActor.LoadSkillConfig(m_cpkEntityInfo, loadCommonSkill);
        }


        TriggerEventNew(BeEventType.onBirth);
    }

    public bool IsRemoved()
    {
        return m_iEntityLifeState == (int)EntityLifeState.ELS_REMOVED;
    }

    public bool IsDeadOrRemoved()
    {
        return IsRemoved() || IsDead();
    }

    public virtual void OnRemove(bool force = false)
    {
        #if RECORD_FILE
        if (null != GetRecordServer())
        {
            GetRecordServer().RecordProcess("{0} OnRemove:{1}", m_iID, force);
        }
        #endif
        
        if (owner.CurrentBeScene != null)
            owner.CurrentBeScene.TriggerEventNew(BeEventSceneType.onEntityRemove, new EventParam() { m_Obj = this });
        TriggerEventNew(BeEventType.onRemove);
        SetLifeState(EntityLifeState.ELS_REMOVED);
        if (m_pkGeActor != null && (force || !dontDelete))
        {
            if (aiManager != null)
            {
                aiManager.Remove();
                aiManager = null;
            }
            if ((this as BeProjectile) != null && !force)
            {
#if !LOGIC_SERVER
                if (m_pkGeActor.isCreatedInBackMode)
                {
                    CurrentBeScene.currentGeScene.DestroyActor(m_pkGeActor);
                    m_pkGeActor = null;
                }
                else
#endif
                    CurrentBeScene.currentGeScene.RecycleActor(m_pkGeActor);
            }
            else
            {
                CurrentBeScene.currentGeScene.DestroyActor(m_pkGeActor);

                m_pkGeActor = null;
                //先注掉，验证服务器用
                //currentBeScene = null;
            }

            if (null != delayCaller)
            {
                delayCaller.Clear();
                //delayCaller = null;
            }

            ClearEventAllNew();

        }

        if (null != actionManager)
            actionManager.Deinit();
    }

    public virtual bool UpdateGraphic(int iDeltaTime)
    {

#if !LOGIC_SERVER

        _updateGraphic(iDeltaTime);
#endif
        return true;
    }

    public void _updateGraphic(int iDeltaTime, int accTime = 0)
    {
        m_kShockEffect.Update(iDeltaTime / (float)(GlobalLogic.VALUE_1000));
        _updateGraphicActor();
        if (m_pkGeActor != null)
        {
            m_pkGeActor.Update(iDeltaTime,
                (int)GeEntity.GeEntityRes.EffectUnique |
                (int)GeEntity.GeEntityRes.EffectMultiple |
                (int)GeEntity.GeEntityRes.EffectGlobal |
                (int)GeEntity.GeEntityRes.Material,
                GetPosition().z, accTime);
        }
    }


    public virtual bool Update(int iDeltaTime)
    {
        if (GetLifeState() != (int)EntityLifeState.ELS_ALIVE)
            return false;

        timeAcc += iDeltaTime;

        //放到beactor里
        //         if (!IsDead())
        //         {
        //             UpdateRecover(iDeltaTime);
        //         }

        if (delayCaller != null)
            delayCaller.Update(iDeltaTime);

        if (actionManager != null)
            actionManager.Update(iDeltaTime);

        //m_kShockEffect.Update(iDeltaTime / 1000.0f);

        if (_updatePause(iDeltaTime))
        {
            return false;
        }

        _updateMoveCmd();
        _updatePosition(iDeltaTime);

        if (m_pkStateGraph != null)
        {
            if (!sgStarted)
            {
                sgStarted = true;
                m_pkStateGraph.Start(stateStart);
            }
            m_pkStateGraph.UpdateStates(iDeltaTime);
        }

        if (m_cpkCurEntityActionInfo != null)
        {
            UpdateFrame();
            _updateWorldDBox(iDeltaTime);
            attachmentproxy.Update(m_fCurrentLogicFrame);
        }
        else
        {
#if DEBUG_SETTING && !LOGIC_SERVER
            if (Global.Settings.showDebugBox)
            {
                if (m_pkGeActor != null)
                {
                    m_pkGeActor.SetDebugDrawData(null);
                }
            }
#endif
        }
        _updateAction(iDeltaTime);
        return true;
    }

    public void UpdateFrame()
    {
        _updateFrame();
    }

    public void PostUpate()
    {
        if (GetLifeState() != (int)EntityLifeState.ELS_ALIVE)
            return;

        if (pause)
            return;

        _calcAttack();

    }

    public void Reset()
    {
        m_pkStateGraph.SwitchStates(new BeStateData((int)ActionState.AS_IDLE));
        ResetMoveCmd();
        ClearMoveSpeed();
        m_vHurtEntity.Clear();
        _onReset();
    }

    public virtual void Reborn()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeEntity.Reborn"))
        {
#endif
            deadType = DeadType.NORMAL;
            if (m_pkGeActor != null)
            {
                m_pkGeActor.RemoveSurface(uint.MaxValue);
            }

            m_iEntityLifeState = (int)EntityLifeState.ELS_ALIVE;
            isDead = false;

            var data = GetEntityData();
            if (data != null)
            {
                data.PostInit();
            }
            if (stateController != null)
            {
                stateController.ResetMoveAbility();
            }

            if (m_pkGeActor != null)
            {
                m_pkGeActor.ResetHPBar();
            }

            if (m_pkGeActor != null)
            {
                int effectInfoId = 1;
                Vec3 pos = new Vec3(m_pkGeActor.GetPosition());
                m_pkGeActor.CreateEffect(effectInfoId, pos, true);
            }

            Reset();

            if (CurrentBeBattle != null)
                CurrentBeBattle.PlaySound(8);
#if ENABLE_PROFILER
        }
#endif
    }

    //--------------------------------------
    public void ResetMoveCmd()
    {
        m_bCmdDirty = true;
        for (int i = 0; i < (int)CommandMove.COMMAND_MOVE_MAX; ++i)
        {
            m_vkMoveCmd[i] = false;
        }

        moveXSpeed = 0;
        moveYSpeed = 0;
        moveZSpeed = 0;
    }

    public void ModifyMoveCmd(int iMoveCmd, bool bUse)
    {
        if (iMoveCmd >= (int)CommandMove.COMMAND_MOVE_X && iMoveCmd < (int)CommandMove.COMMAND_MOVE_MAX)
        {
            if (m_vkMoveCmd[iMoveCmd] != bUse)
            {
                m_bCmdDirty = true;
                m_vkMoveCmd[iMoveCmd] = bUse;

                if (bUse)
                    m_pkStateGraph.FireEvents2CurrentStates((int)EventCommand.EVENT_COMMAND_PRESS_JOYSTICK);
                else
                {
                    bool flag = true;
                    for (int i = (int)CommandMove.COMMAND_MOVE_X; i < (int)CommandMove.COMMAND_MOVE_DIRECTION; ++i)
                    {
                        if (m_vkMoveCmd[i])
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                        m_pkStateGraph.FireEvents2CurrentStates((int)EventCommand.EVENT_COMMAND_RELEASE_JOYSTICK);

                }

            }
        }
    }

    //public bool bLockMoveDegree;
    //public short nLockDegree;

    public bool bLockMove;

    public void ModifyMoveDirection(bool bSet, short nDegree = 0)
    {
        if (bLockMove)
            return;

        // if (!bLockMoveDegree && bSet)
        // {
        //     //nDegree = nLockDegree;
        //     if (nDegree > 6 && nDegree < 18)
        //     {
        //         nDegree = 12;
        //     }
        //     else
        //     {
        //         nDegree = 0;
        //     }
        // }
        //TriggerEvent(BeEventType.OnChangeMoveDir, new object[] { nDegree,bSet });
        TriggerEventNew(BeEventType.OnChangeMoveDir, new EventParam() { m_Int = nDegree, m_Bool = bSet });
        if (bSet && !m_vkMoveCmd[(int)CommandMove.COMMAND_MOVE_DIRECTION])
        {
            m_pkStateGraph.FireEvents2CurrentStates((int)EventCommand.EVENT_COMMAND_PRESS_JOYSTICK);
            TriggerEventNew(BeEventType.onMoveJoystick);
        }
        else if (!bSet && m_vkMoveCmd[(int)CommandMove.COMMAND_MOVE_DIRECTION])
        {
            m_pkStateGraph.FireEvents2CurrentStates((int)EventCommand.EVENT_COMMAND_RELEASE_JOYSTICK);
            TriggerEventNew(BeEventType.onStopMoveJoystick);
        }

        m_vkMoveCmd[(int)CommandMove.COMMAND_MOVE_DIRECTION] = bSet;
        m_Degree = nDegree;
        m_bCmdDirty = true;

        OnJoystickMove(nDegree);
    }

    public virtual void OnJoystickMove(int degree)
    {

    }

    public bool IsInMoveDirection()
    {
        return m_vkMoveCmd[(int)CommandMove.COMMAND_MOVE_DIRECTION];
    }

    public short MoveDirectionDegree()
    {
        return m_Degree;
    }

    public void GetMoveSpeedFromDirection(out VInt x, out VInt y)
    {

        VFactor radians = VFactor.pi / 180 * (m_Degree * InputManager.DegreeDiv);

        x = speedConfig.x * IntMath.cos(radians.nom, radians.den);
        y = speedConfig.y * IntMath.sin(radians.nom, radians.den);

    }

    public void ClearMoveSpeed(int iClearMod = (int)SpeedCear.SPEEDCEAR_XYZ)
    {
        if ((iClearMod & (int)SpeedCear.SPEEDCEAR_X) != 0)
        {
            moveXSpeed = 0;
        }

        if ((iClearMod & (int)SpeedCear.SPEEDCEAR_Y) != 0)
        {
            moveYSpeed = 0;
        }

        if ((iClearMod & (int)SpeedCear.SPEEDCEAR_Z) != 0)
        {
            moveZSpeed = 0;
        }

        moveXAcc = 0;
        moveYAcc = 0;
    }

    // public bool IsHaveMoveCmd()
    // {
    //     return m_bHaveMoveCmd;
    // }

    public void ResetDamageData()
    {
        m_vHurtEntity.Clear();
    }

    public void SetMoveSpeedX(VInt fValue)
    {
        moveXSpeed = fValue;
    }

    public void SetMoveSpeedXRate(VFactor fValue)
    {
        m_fMoveXSpeedRate = fValue;
    }

    public void SetMoveSpeedYRate(VFactor fValue)
    {
        m_fMoveYSpeedRate = fValue;
    }

    public void SetMoveSpeedXLocal(VInt fValue)
    {
        moveXSpeed = (int)fValue * (GetFace() ? -1 : 1);
    }

    public void SetMoveSpeedY(VInt fValue)
    {
        moveYSpeed = fValue;
    }

    public void SetMoveSpeedZ(VInt fValue)
    {
        moveZSpeed = fValue;
    }

    public void SetMoveSpeedZAcc(VInt fValue)
    {
        moveZAcc = fValue;
    }

    public void SetMoveSpeedZAccExtra(VInt fValue)
    {
        moveZAccExtra = fValue;
    }

    public void SetMoveSpeedYAcc(VInt fValue)
    {
        moveYAcc = fValue;
    }

    public void SetMoveSpeedXAcc(VInt fValue)
    {
        moveXAcc = fValue;
    }

    public void sgPushState(BeStateData rkData)
    {
        if (m_pkStateGraph != null)
        {
            m_pkStateGraph.PushState(ref rkData);
        }
    }

    public void sgLocomoteState()
    {
        if (m_pkStateGraph != null)
        {
            m_pkStateGraph.LocomoteState();
        }
    }

    public void sgForceSwitchState(BeStateData rkData)
    {
        if (m_pkStateGraph != null)
        {
            m_pkStateGraph.ClearStateStack();
            m_pkStateGraph.Locomote(rkData, true);
        }
    }

    public void sgClearStateStack()
    {
        if (m_pkStateGraph != null)
        {
            m_pkStateGraph.ClearStateStack();
        }
    }

    public void sgPopState()
    {
        if (m_pkStateGraph != null)
        {
            m_pkStateGraph.PopState();
        }
    }

    public virtual void Locomote(BeStateData rkStateData, bool bForce = false)
    {
        if (m_pkStateGraph != null && !IsRemoved())
        {
            m_pkStateGraph.Locomote(rkStateData, bForce);
        }
    }

    public void sgSwitchStates(BeStateData s)
    {
        if (m_pkStateGraph != null)
        {
            m_pkStateGraph.SwitchStates(s);
        }
    }

    public void sgSetCurrentStatesTimeout(int fTime, bool force = false)
    {
        if (m_pkStateGraph != null)
        {
            m_pkStateGraph.SetCurrentStatesTimeout((int)(m_pkStateGraph.GetCurrentStatesTime() + fTime), force);
        }
    }

    public int sgGetCurrentStateRemainTime()
    {
        if (m_pkStateGraph != null)
        {
            int timeout = m_pkStateGraph.GetCurrentStateData().uiTimeOut;
            int curtime = (int)m_pkStateGraph.GetCurrentStatesTime();
            if (timeout > 0)
            {
                return timeout - curtime;
            }
        }

        return -1;
    }

    public int sgGetCurrentState()
    {
        if (m_pkStateGraph != null)
        {
            return m_pkStateGraph.GetCurrentState();
        }

        return -1;
    }
    //------------------------------------------------------

    public int GetCamp()
    {
        return m_iCamp;
    }

    public int GetPID()
    {
        return m_iID;
    }

    public int GetLifeState()
    {
        return m_iEntityLifeState;
    }

    public void SetLifeState(EntityLifeState state)
    {
        m_iEntityLifeState = (int)state;
    }

    public void SetPosition(VInt3 rkPos, bool immediate = false, bool showLog = true, bool needUpdateBackPos = false)
    {
        if (m_kPosition != rkPos)
        {
            m_kPosition = rkPos;
            m_bPositionDirty = true;
            //是否需要更新备份位置坐标 可以用于裂波斩抓取结束后的位置设置 避免闪现到原来坐标的BUG
            if (needUpdateBackPos)
            {
                m_kBlockPosition = rkPos;
            }

            /*			if (showLog && m_iID==1 && currentBeScene != null && currentBeScene.IsInBlockPlayer(m_kPosition))
                        {
                            Logger.LogErrorFormat("set in block player:({0},{1},{2})", m_kPosition.x, m_kPosition.y, m_kPosition.z);
                        }*/

            if (immediate)
                _updateGraphicActor();
        }
    }

    //不会在阻挡里
    public void SetStandPosition(VInt3 pos, bool immediate = false)
    {
        var tmpPos = pos;
        tmpPos.z = 0;
        var newPos = tmpPos;
        if (m_kBlockPosition == VInt3.zero)
            newPos = BeAIManager.FindStandPositionNew(tmpPos, CurrentBeScene, GetFace(), false, 30);
        else
            newPos = m_kBlockPosition;
        newPos.z = pos.z;

        /*		if (m_iID==1 && currentBeScene != null && currentBeScene.IsInBlockPlayer(newPos))
                {
                    Logger.LogErrorFormat("set in block player:({0},{1},{2})", newPos.x, newPos.y, newPos.z);
                }*/

        SetPosition(newPos, immediate);
    }

    public void SaveCurrentPosition()
    {
        savedPosition = GetPosition();
    }

    public void SetRestrainPosition(bool flag)
    {
        restrainPosition = flag;
    }

    public VInt3 GetPosition()
    {
        return m_kPosition;
    }

    //!! Vec3 只给渲染用，以及配置文件用
    public Vec3 GetGePosition(PositionType pt)
    {
        Vec3 pos = GetPosition().vec3;
        if (m_pkGeActor == null)
            return pos;
#if !LOGIC_SERVER
        if (pt == PositionType.BODY)
            pos += new Vec3(m_pkGeActor.bodyLocalPosition.x, m_pkGeActor.bodyLocalPosition.z, m_pkGeActor.bodyLocalPosition.y);
        else if (pt == PositionType.OVERHEAD)
        {
            var overheadLocalPosition = m_pkGeActor.GetOverHeadPosition();
            pos += new Vec3(overheadLocalPosition.x, overheadLocalPosition.z, overheadLocalPosition.y);
        }
        else if (pt == PositionType.ORIGIN_BUFF)
            pos += new Vec3(m_pkGeActor.buffOriginLocalPosition.x, m_pkGeActor.buffOriginLocalPosition.z, m_pkGeActor.buffOriginLocalPosition.y);

        /*Vec3[] posArr = new Vec3[1];
        posArr[0] = pos;
        TriggerEvent(BeEventType.onChangeBeHitNumberPos, new object[] { posArr });
        return posArr[0];*/
        
        var ret = TriggerEventNew(BeEventType.onChangeBeHitNumberPos, new EventParam{m_Vector = pos.vector3()});
        return new Vec3(ret.m_Vector);
#else
        return pos;
#endif
    }

    public VInt2 GetPosition2()
    {
        return new VInt2(m_kPosition.x, m_kPosition.y);
    }

    public Vector3 GetGePosition()
    {
        return new Vector3(m_kPosition.fx, m_kPosition.fz, m_kPosition.fy);
    }

    //     public float GetDistance(VInt2 point)
    //     {
    //         VInt2 center = GetPosition2();
    //         return center.distance(point);
    //     }

    public int GetDistance(BeEntity target)
    {
        if (target == null)
            return 0;

        VInt2 center = GetPosition2();
        VInt2 point = target.GetPosition2();
        //return center.distance(point);
        return (center - point).magnitude;
    }

    public void SetFace(bool bFace, bool immediate = false, bool force = false)
    {
        if (!stateController.CanTurnFace() && !force)
            return;

        if (m_bFaceLeft != bFace)
        {
            m_bFaceLeft = bFace;
            m_bPositionDirty = true;

            if (IsProcessRecord())
            {
                GetRecordServer().RecordProcess("[BATTLE]change face:{0} {1}", bFace, GetInfo());
                GetRecordServer().Mark(0x8779795, GetEntityRecordAttribute(), GetName());
                // Mark:0x8779795 [BATTLE]change face:{7} PID:{0}-{12} Pos:({1},{2},{3}),Speed:({4},{5},{6}) Hp:{8},Mp:{9},Flag:{10},curSkillId:{11}
            }

            OnChangeFace();

            TriggerEventNew(BeEventType.onChangeFace);


            if (immediate)
                _updateGraphicActor();
        }
    }

    public virtual void OnChangeFace()
    {

    }

    //是否朝向向左
    public bool GetFace()
    {
        return m_bFaceLeft;
    }

    public void SetScale(VInt fScale)
    {
        if (m_fScale != fScale)
        {
            m_fScale = fScale;
            m_bPositionDirty = true;
        }
    }

    public VInt GetScale()
    {
        return m_fScale;
    }

    public void SetZDimScaleFactor(VFactor scale)
    {
        m_fZdimScaleFactor = scale;
    }

    public VFactor GetZDimScaleFactor()
    {
        return m_fZdimScaleFactor;
    }

    public bool HasTag(int iTag)
    {
        return m_kStateTag.HasFlag(iTag);
    }

    public void SetTag(int iTag, bool bSet)
    {
        if (bSet)
            m_kStateTag.SetFlag(iTag);
        else
            m_kStateTag.ClearFlag(iTag);
    }

    public bool CanBeAttacked()
    {
        return m_bCanBeAttacked;
    }

    public void SetCanBeAttacked(bool bValue)
    {
        m_bCanBeAttacked = bValue;
    }

    public virtual int GetCurSkillID()
    {
        return 0;
    }

    public virtual bool IsCurSkillOpenShock()
    {
        return true;
    }

    public int PlayQueuedAction(List<ActionDesc> actions, float aniSpeed = 1)
    {
        actionQueue = actions;
        queuedActionSpeed = aniSpeed;

        PlayActionAuto();

        return 0;
    }

    public void ClearQueuedAction()
    {
        if (actionQueue != null && actionQueue.Count > 0)
            actionQueue.Clear();
    }

    public void PlayActionAuto()
    {
        if (actionQueue.Count > 0)
        {
            var action = actionQueue[0];
            actionQueue.RemoveAt(0);

            //Logger.LogErrorFormat("play queued action {0}", ActionConfigNames[(int)action]);

            actionTimeout = PlayAction(action.actionType, queuedActionSpeed, true);
            if (Global.Settings.useNewHurtAction && action.timeout > 0)
            {
                actionTimeout = action.timeout;
            }
        }
    }

    public int PlayAction(ActionType iAction, float aniSpeed = 1, bool queued = false)
    {
        //Logger.Log("PlayAction:" + iAction);

        if (!queued)
        {
            ClearQueuedAction();
        }

        // if ((int)iAction < ActionConfigNames.Length)
        // {
        //     string moveName = ActionConfigNames[(int)iAction];
        //     //Logger.Log("playaction in BeEntity name :" + moveName + "ID: " + m_iID);
        //     var timeout = PlayAction(moveName, aniSpeed);
        //
        //     return timeout;
        // }
        
        string moveName = GetActionNameByType(iAction);
        if (moveName != null)
        {
            var timeout = PlayAction(moveName, aniSpeed);
            return timeout;
        }

        return 0;
    }

    public string GetActionNameByType(ActionType actionType)
        {
            int type = (int)actionType;
            string ret = null;
            if (type < StaticActionConfigNames.Length)
            {
                if (ActionConfigNames[type] != null)
                    ret = ActionConfigNames[type];
                else
                    ret = StaticActionConfigNames[type];
            }

            return ret;
        }

        protected ActionState GetActionStateByActionType(ActionType type)
        {
            var ret = ActionState.AS_NONE;
            if (type == ActionType.ActionType_IDLE || type == ActionType.ActionType_SpecialIdle)
                ret = ActionState.AS_IDLE;
            else if (type == ActionType.ActionType_WALK || type == ActionType.ActionType_RUN)
                ret = ActionState.AS_RUN;

            return ret;
        }
        public bool ReplaceActionName(ActionType actionType, string newName)
        {
            if (string.IsNullOrEmpty(newName))
                return false;

            if (!HasAction(newName))
                return false;

            if ((int)actionType >= StaticActionConfigNames.Length)
                return false;

            ActionConfigNames[(int)actionType] = newName;

            //当前正在要替换的状态，就需要重切到这个状态
            var actionState = GetActionStateByActionType(actionType);
            if (actionState == ActionState.AS_IDLE || actionState == ActionState.AS_RUN)
            {
                if (sgGetCurrentState() == (int)actionState)
                {
                    sgForceSwitchState(new BeStateData((int)actionState));
                }
            }

            return true;
        }

        public bool RestoreActionName(ActionType actionType)
        {
            if ((int)actionType >= StaticActionConfigNames.Length)
                return false;

            ActionConfigNames[(int)actionType] = null;

            //当前正在要替换的状态，就需要重切到这个状态
            var actionState = GetActionStateByActionType(actionType);
            if (actionState == ActionState.AS_IDLE || actionState == ActionState.AS_RUN)
            {
                if (sgGetCurrentState() == (int)actionState)
                {
                    sgForceSwitchState(new BeStateData((int)actionState));
                }
            }
            return true;
        }
        
    public bool HasAction(string actionName)
    {
        if (m_cpkEntityInfo != null)
        {
            return m_cpkEntityInfo.HasAction(actionName);
        }

        return false;
    }

    public bool HasAction(ActionType type)
    {
        string name = GetActionNameByType(type);
        return HasAction(name);
    }

    public void ResetActionInfo()
    {
        if (IsProcessRecord())
        {
            GetRecordServer().RecordProcess("[BATTLE]ResetActionInfo:{0}", GetInfo());
            GetRecordServer().Mark(0x8779796, GetEntityRecordAttribute(), GetName());
            // Mark:0x8779796 ResetActionInfo PID:{0},name {12} Pos:({1},{2},{3}),Speed:({4},{5},{6}),Face: {7},Hp: {8},Mp: {9},Flag: {10},curSkillId: {11}
        }

        if (m_pkGeActor != null)
        {
            m_iCurrentLogicFrame = 0;
            m_cpkCurEntityActionInfo = null;
            m_cpkCurEntityActionFrameData = null;
            //重置全局特效
            m_pkGeActor.Clear((int)GeEntity.GeEntityRes.EffectUnique);
            actionLooped = false;
#if !SERVER_LOGIC
            m_pkGeActor.StopAction();
#endif
        }
    }

    public int PlayAction(string acActionName, float aniSpeed = 1)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeEntity.PlayAction"))
        {
#endif
        //Logger.Log("PlayAction:" + acActionName);

        if (acActionName == Global.ACTION_EXPDEAD)
            playedExpDead = true;

        if (m_pkGeActor != null)
        {
            m_iCurrentLogicFrame = -1;

            if (m_cpkEntityInfo != null)
            {
                if (m_cpkEntityInfo.HasAction(acActionName))
                {
                    m_cpkCurEntityActionInfo = m_cpkEntityInfo._vkActionsMap[acActionName];
                    m_cpkEntityInfo.SetActionName(acActionName);
                    //Logger.LogErrorFormat("Change Action {0} {1}",acActionName,m_cpkCurEntityActionInfo.actionName);

                    TriggerEventNew(BeEventType.onPlayAction, new EventParam() { m_String = acActionName });
                }
                else
                {
                    if (GetRecordServer().IsProcessRecord())
                    {
                        GetRecordServer().RecordProcess("[BATTLE]PID:{0}, 没有技能名为 {1} 的技能配置文件{2}", GetPID(), GetName(), acActionName);
                        GetRecordServer().Mark(0x8779797, GetEntityRecordAttribute(), GetName(), acActionName);
                        // Mark:0x8779797 没有技能配置文件 PID:{0},name: {12} actionName: {13},Pos:({1},{2},{3}),Speed:({4},{5},{6}),Face: {7},Hp: {8},Mp: {9},Flag: {10},curSkillId: {11} 
                    }
                    m_cpkCurEntityActionInfo = null;
                    m_cpkEntityInfo.SetActionName("");
                }
            }
            else
            {
                m_cpkCurEntityActionInfo = null;
            }

            attachmentproxy.Init(m_cpkCurEntityActionInfo);
            //重置全局特效
            m_pkGeActor.Clear((int)GeEntity.GeEntityRes.EffectUnique);

            if (m_cpkCurEntityActionInfo != null)
            {

                // //设置tag
                // int flags = m_cpkCurEntityActionInfo.exFlag.GetAllFlag();
                // if (flags > 0)
                // {
                //     m_pkStateGraph.GetCurrentStateData().kExTags.SetFlag(flags);
                // }

                //init frame
                actionLooped = false;
                _updateFrame(true);
                attachmentproxy.Update(m_fCurrentLogicFrame);

#if !SERVER_LOGIC

                bool loop = m_cpkCurEntityActionInfo.bLoop;

                if (m_cpkCurEntityActionInfo.actionName.Contains("idle", System.StringComparison.OrdinalIgnoreCase) ||
                    m_cpkCurEntityActionInfo.actionName.Contains("walk", System.StringComparison.OrdinalIgnoreCase))
                {
                    loop = true;
                }


                m_pkGeActor.ChangeAction(m_cpkCurEntityActionInfo.actionName, aniSpeed * m_cpkCurEntityActionInfo.animationspeed, loop);

#endif
                    //int[] timeArray = new int[2];
                    //timeArray[0] = m_cpkCurEntityActionInfo.skillTotalTime;
                    //timeArray[1] = GlobalLogic.VALUE_1000;

                    var param = TriggerEventNew(BeEventType.onChangeSkillTime, new EventParam() { m_Vint3 = new VInt3(m_cpkCurEntityActionInfo.skillID, m_cpkCurEntityActionInfo.skillTotalTime, GlobalLogic.VALUE_1000) });
                    var skillTotalTime = param.m_Vint3.y * VFactor.NewVFactor(param.m_Vint3.z, GlobalLogic.VALUE_1000);

                    return Math.Max(m_cpkCurEntityActionInfo.fRealFramesTime, skillTotalTime);
            }
            else
            {
                if (IsProcessRecord())
                {
                    GetRecordServer().RecordProcess("[BATTLE]PID:{0}, m_cpkCurEntityActionInfo is null  acActionName:{1} GetInfo:{2}", GetPID(), acActionName, GetInfo());
                    GetRecordServer().Mark(0x8779798, GetEntityRecordAttribute(), acActionName, GetName());
                    // Mark:0x8779798 m_cpkCurEntityActionInfo is null PID:{0}, actionName: {12} name {13} Pos:({1},{2},{3}),Speed:({4},{5},{6}),Face: {7},Hp: {8},Mp: {9},Flag: {10},curSkillId: {11}
                }
            }
        }

        return 0;
#if ENABLE_PROFILER
        }
#endif
    }

    public void _ChangeAnimation(string aniName, float aniSpeed = 1)
    {
        if (m_cpkCurEntityActionInfo != null && m_pkGeActor != null)
        {
            bool loop = m_cpkCurEntityActionInfo.bLoop;
            m_pkGeActor.ChangeAction(aniName, aniSpeed * m_cpkCurEntityActionInfo.animationspeed, loop);
        }
    }

    public void SetNeedDead(bool flag)
    {
        needDead = flag;
    }

    public int GetCurrentActionDuration()
    {
        int duration = 0;
        if (m_cpkCurEntityActionInfo != null)
        {
            duration = (int)(m_cpkCurEntityActionInfo.fRealFramesTime);
        }

        return duration;
    }
    
    public BeStatesGraph GetStateGraph()
    {
        return m_pkStateGraph;
    }

    public void SetStateGraph(BeStatesGraph pkStateGraph, int iStartState)
    {
        m_pkStateGraph = pkStateGraph;
        stateStart = iStartState;
    }

    public virtual bool _isCmdMoveForbiden()
    {
        if (m_pkStateGraph == null || GetEntityData() == null)
            return true;

        if (GetEntityData() != null && IsDead())
            return true;

        if (!stateController.CanMove())
            return true;

        if (m_pkStateGraph.CurrentStateHasTag((int)AStateTag.AST_CONTROLED) || (
            m_pkStateGraph.CurrentStateHasTag((int)AStateTag.AST_BUSY) && !m_kStateTag.HasFlag((int)AState.ACS_JUMP)
            ))
            return true;

        return false;
    }



    public void ClearState()
    {
        _clearStates();
    }
    public virtual bool _onCreate()
    {
        return false;
    }

    public virtual void _updateAction(int deltaTime)
    {
        if (actionQueue.Count <= 0)
            return;

        actionTimeout -= deltaTime;
        if (actionTimeout <= 0)
        {
            PlayActionAuto();
        }
    }

    public virtual void _updateFrame(bool force = false)
    {
        if (!force && (pause || m_cpkCurEntityActionInfo == null))
        {
            return;
        }

        int iPreLogicFrame = m_iCurrentLogicFrame;
        uint fTime = m_pkStateGraph.GetCurrentStatesTime();



        if (m_cpkCurEntityActionInfo.iLogicFramesNum <= 1)
        {
            m_fCurrentLogicFrame = 0.0f;
            m_iCurrentLogicFrame = 0;
        }
        else
        {
            //!! 这里是个问题
            //double tmp = (fTime / (double)(GlobalLogic.VALUE_1000) / m_cpkCurEntityActionInfo.fLogicFrameDeltaTime);
            VFactor tmp = new VFactor(fTime * m_cpkCurEntityActionInfo.fLogicFrameDeltaTime.den,
            GlobalLogic.VALUE_1000 * m_cpkCurEntityActionInfo.fLogicFrameDeltaTime.nom);
            m_iCurrentLogicFrame = tmp.integer;
            m_fCurrentLogicFrame = tmp.single;
            //Clamp Frame
            if (m_iCurrentLogicFrame >= m_cpkCurEntityActionInfo.iLogicFramesNum)
            {
                if (m_cpkCurEntityActionInfo.bLoop)
                {
                    actionLooped = true;
                    m_iCurrentLogicFrame %= m_cpkCurEntityActionInfo.iLogicFramesNum;

                    if (m_iCurrentLogicFrame == 0)
                    {
                        TriggerEventNew(BeEventType.onActionLoop);
                        //Logger.LogErrorFormat("trigger onActionLoop:{0}", Time.realtimeSinceStartup);
                    }
                }
            }
            //Clamp Float Frame 临时使用
            if (m_fCurrentLogicFrame > m_cpkCurEntityActionInfo.iLogicFramesNum - 1)
            {
                if (m_cpkCurEntityActionInfo.bLoop)
                {
                    m_fCurrentLogicFrame %= (m_cpkCurEntityActionInfo.iLogicFramesNum - 1);
                }
                else
                {
                    m_fCurrentLogicFrame = m_cpkCurEntityActionInfo.iLogicFramesNum - 1;
                }
            }
        }

        if (m_iCurrentLogicFrame >= 0 && m_iCurrentLogicFrame < m_cpkCurEntityActionInfo.vFramesData.Count)
        {
            m_cpkCurEntityActionFrameData = m_cpkCurEntityActionInfo.vFramesData[m_iCurrentLogicFrame];

#if DEBUG_SETTING && !LOGIC_SERVER
            if (m_pkGeActor != null)
            {
                if (Global.Settings.showDebugBox)
                {
                    m_pkGeActor.SetDebugDrawData(m_cpkCurEntityActionFrameData, m_fScale.scalar * skillAttackScale.single, m_fZdimScaleFactor.single);
                }
                else
                {
                    m_pkGeActor.SetDebugDrawData(null);
                }
            }
#endif
        }
        else
        {
            m_cpkCurEntityActionFrameData = m_cpkCurEntityActionInfo.vFramesData[m_cpkCurEntityActionInfo.iLogicFramesNum - 1];
        }
        /*
                if (sgGetCurrentState() == (int)ActionState.AS_CASTSKILL)
                {
                    uint fTotalTime = m_pkStateGraph.GetCurrentTimeSinceStart();
                    int totalFrame = (int)(fTotalTime / 1000.0f / m_cpkCurEntityActionInfo.fLogicFrameDeltaTime);
                    //Logger.LogError("skill frame:" + m_iCurrentLogicFrame + " total frame:" + m_cpkCurEntityActionInfo.vFramesData.Count + " skill id:" + GetCurSkillID() + " time:" + fTotalTime);
                }
        */
        if (iPreLogicFrame != m_iCurrentLogicFrame)
        {
            /*
                 if (sgGetCurrentState() == (int)ActionState.AS_CASTSKILL)
                 {
                     uint fTotalTime = m_pkStateGraph.GetCurrentTimeSinceStart();
                     int totalFrame = (int)(fTotalTime / 1000.0f / m_cpkCurEntityActionInfo.fLogicFrameDeltaTime);
                    Logger.LogError("-----skill frame:" + m_iCurrentLogicFrame + " total frame:" + m_cpkCurEntityActionInfo.vFramesData.Count + " skill id:" + GetCurSkillID() + " time:" + fTotalTime);
                 }
    */

            _onEnterFrame(m_iCurrentLogicFrame);
        }
    }
    public virtual void _onEnterFrame(int iFrame)
    {
        BDEntityActionFrameData frameData = m_cpkCurEntityActionFrameData;
        if (frameData != null && frameData.pEvents.Count > 0)
        {
            for (int i = 0; i < frameData.pEvents.Count; ++i)
                frameData.pEvents[i].OnEvent(this);
        }

        if (frameData.kFlag.HasFlag((int)DSFFrameTags.TAG_NEWDAMAGE))
        {
            ResetDamageData();
        }

        if (frameData.kFlag.HasFlag((int)DSFFrameTags.TAG_LOCKZSPEED))
        {
            m_pkStateGraph.SetCurrentStateTag((int)AStateTag.AST_LOCKZ, true);
        }

        if (frameData.kFlag.HasFlag((int)DSFFrameTags.TAG_LOCKZSPEEDFREE))
        {
            m_pkStateGraph.SetCurrentStateTag((int)AStateTag.AST_LOCKZ, false);
        }

        if (frameData.kFlag.HasFlag((int)DSFFrameTags.TAG_RESTORE_GRAVITY))
        {
            if (isFloating)
                RemoveFloating();
            else
                stateController.SetAbilityEnable(BeAbilityType.GRAVITY, true);
        }

        if (frameData.kFlag.HasFlag((int)DSFFrameTags.TAG_IGNORE_GRAVITY))
        {
            if (isFloating)
                RestoreFloating(true);
            else
                stateController.SetAbilityEnable(BeAbilityType.GRAVITY, false);
        }

        if (frameData.kFlag.HasFlag((int)DSFFrameTags.TAG_SET_TARGET_POS_XY))
        {
            OnDealFrameTag(DSFFrameTags.TAG_SET_TARGET_POS_XY);
        }

        if (frameData.kFlag.HasFlag((int)DSFFrameTags.TAG_CURFRAME))
        {
            string tagFlag = frameData.kFlag.GetFlagData();
            if (tagFlag != null)
            {
                owner.TriggerEventNew(BeEventType.onSkillCurFrame, new EventParam() {m_String = tagFlag, m_Int = owner.GetCurSkillID()});
                //owner.TriggerEvent(BeEventType.onSkillCurFrame, new object[] { tagFlag });  //标识当前帧
            }
        }

        if (frameData.kFlag.HasFlag((int)DSFFrameTags.TAG_CHANGEFACE))
        {
            owner.SetFace(!owner.GetFace());                                                    //转向
        }

        if (frameData.kFlag.HasFlag((int)DSFFrameTags.TAG_CHANGEFACEBYDIR))     //根据当前自己的摇杆方向去判断是否需要转向
        {
            int degree = m_Degree * 15;         //这个地方的15对应InputManager.DegreeDiv这个值 因为验证服务器不能取这个值 所以这个位置写死
            if (degree > 90 && degree < 270)
                owner.SetFace(true);
            else
                owner.SetFace(false);
        }

        if (frameData.kFlag.HasFlag((int)DSFFrameTags.TAG_LOOKAT_TARGET))
        {
            BeActor target = null;
            if (target == null && owner.aiManager != null)
            {
                target = owner.aiManager.aiTarget;
            }
            if (target == null)
            {
                target = owner.CurrentBeScene.GetEntityByPID(owner.lastAttackerId) as BeActor;
            }
            if (target != null)
            {
                owner.SetFace(target.GetPosition().x < owner.GetPosition().x);
            }
        }

        if (frameData.kFlag.HasFlag((int)DSFFrameTags.TAG_OPEN_2ND_STATE))
        {
            tempTwoStateMode = true;
            TriggerEventNew(BeEventType.onOpen2ndState);
        }

        if (frameData.kFlag.HasFlag((int)DSFFrameTags.TAG_CLOSE_2ND_STATE))
        {
            tempTwoStateMode = false;
            TriggerEventNew(BeEventType.onClose2ndState);
        }

        if (frameData.kFlag.HasFlag((int) DSFFrameTags.TAG_REMOVEEFFECT))
        {
#if !LOGIC_SERVER
            string path = frameData.kFlag.GetFlagData();
            if (m_pkGeActor != null && !string.IsNullOrEmpty(path))
                m_pkGeActor.DestroyEffectByName(path);
#endif
        }

        if (frameData.kFlag.HasFlag((int)DSFFrameTags.TAG_STARTCHECKHIT))
        {
            var actor = this as BeActor;
            if (actor != null)
                actor.SkipToNextPhase = new BeSkillSkipToNextPhase(actor) as IBeSkillSkipToNextPhase;
        }

        if (frameData.kFlag.HasFlag((int)DSFFrameTags.TAG_STARTDEALSKIPPHASE))
        {
            var actor = this as BeActor;
            if (actor != null && actor.SkipToNextPhase != null)
                actor.SkipToNextPhase.DealSwitch();
        }
        //隐藏影子
        if (frameData.kFlag.HasFlag((int)DSFFrameTags.TAG_SHADOW_HIDE))
        {
#if !LOGIC_SERVER
            if (m_pkGeActor != null)
            {
                var obj = m_pkGeActor.GetShadowObj();
                if (null != obj)
                    obj.CustomActive(false);
            }
#endif
        }
        //显示影子
        if (frameData.kFlag.HasFlag((int)DSFFrameTags.TAG_SHADOW_SHOW))
        {
#if !LOGIC_SERVER
            if (m_pkGeActor != null)
            {
                var obj = m_pkGeActor.GetShadowObj();
                if (null != obj)
                    obj.CustomActive(true);
            }
#endif
        }
        //隐藏名字板
        if (frameData.kFlag.HasFlag((int)DSFFrameTags.TAG_NAME_HIDE))
        {
#if !LOGIC_SERVER
            if (m_pkGeActor != null)
            {
                var obj = m_pkGeActor.goInfoBar;
                if (null != obj)
                    obj.CustomActive(false);
            }
#endif
        }
        //显示名字板
        if (frameData.kFlag.HasFlag((int)DSFFrameTags.TAG_NAME_SHOW))
        {
#if !LOGIC_SERVER
            if (m_pkGeActor != null)
            {
                var obj = m_pkGeActor.goInfoBar;
                if (null != obj)
                    obj.CustomActive(true);
            }
#endif
        }
        //隐藏血条
        if (frameData.kFlag.HasFlag((int)DSFFrameTags.TAG_HPBAR_HIDE))
        {
#if !LOGIC_SERVER
            if (m_pkGeActor != null)
            {
                var obj = m_pkGeActor.goHPBar;
                if (null != obj)
                    obj.CustomActive(false);
            }
#endif
        }
        //显示血条
        if (frameData.kFlag.HasFlag((int)DSFFrameTags.TAG_HPBAR_SHOW))
        {
#if !LOGIC_SERVER
            if (m_pkGeActor != null)
            {
                var obj = m_pkGeActor.goHPBar;
                if (null != obj)
                    obj.CustomActive(true);
            }
#endif
        }
    }

    public virtual void OnDealFrameTag(DSFFrameTags frameTag)
    {

    }

    static VFactor speedScaleFactor = new VFactor(32, 1000);
    public bool CanMoveNext(bool dirX, bool moveRight)
    {
        var pos = GetPosition();
        if (dirX)
        {
            var tmpSpeed = speedConfig.x;
            tmpSpeed = moveRight ? tmpSpeed : -tmpSpeed;

            pos.x += (speedScaleFactor * m_fMoveXSpeedRate * tmpSpeed).roundInt;
        }
        else
        {
            var tmpSpeed = speedConfig.y;
            tmpSpeed = moveRight ? tmpSpeed : -tmpSpeed;

            pos.y += (speedScaleFactor * m_fMoveYSpeedRate * tmpSpeed).roundInt;
        }

        return !currentBeScene.IsInBlockPlayer(pos);
    }


    public int GetEnhanceRadius()
    {
        if (GetEntityData() != null)
        {
            return GetEntityData().enhancedRadius;
        }

        return 0;
    }
    protected VInt3 EnhanceVec(VInt3 pos, bool faceLeft, VInt dis)
    {
        if (faceLeft)
            pos.x -= dis.i;
        else
            pos.x += dis.i;
        return pos;
    }

    //     public Vec3 EnhanceVec2(Vec3 pos, float dis = 0.0f)
    //     {
    //         pos.x -= dis;
    //         pos.x += dis;
    //         return pos;
    //     }

    public bool inUpdatePosition = false;

    public virtual void _updatePosition(int iDeltime)
    {
        inUpdatePosition = true;

        if (moveXSpeed != 0 || moveYSpeed != 0 || moveZSpeed != 0 ||
            extraSpeed.x != 0 || extraSpeed.y != 0 ||
            GetPosition().z > 0 || resetMoveDirty)
        {
            if (resetMoveDirty)
            {
                resetMoveDirty = false;
            }

            /*
            float fDeltime = iDeltime;
            fDeltime /= 1000.0f;
            */

            if (forceXAccTimer > 0)
            {
                forceXAccTimer -= iDeltime;
                if (forceXAccTimer <= 0)
                {
                    forceXAccTimer = 0;
                    SetMoveSpeedXAcc(0);
                }
            }

            if (forceZAccTimer > 0)
            {
                forceZAccTimer -= iDeltime;
                if (forceZAccTimer <= 0)
                {
                    forceZAccTimer = 0;
                    ResetWeight();
                }
            }

            VFactor fDeltime = new VFactor(iDeltime, 1000);
            VInt intDeltime = fDeltime.vint;

            if (moveXSpeed != 0 && moveXAcc != 0)
            {
                int xcoff = moveXSpeed > 0 ? 1 : -1;
                moveXSpeed = (moveXSpeed + xcoff * moveXAcc.i * fDeltime);

                if (xcoff > 0)
                    moveXSpeed = Math.Max(0, moveXSpeed.i);
                else if (xcoff < 0)
                    moveXSpeed = Math.Min(0, moveXSpeed.i);
            }

            if (moveYSpeed != 0 && moveYAcc != 0)
            {
                int ycoff = moveYSpeed > 0 ? 1 : -1;
                moveYSpeed = (moveYSpeed + ycoff * moveYAcc.i * fDeltime);

                if (ycoff > 0)
                    moveYSpeed = Math.Max(0, moveYSpeed.i);
                else if (ycoff < 0)
                    moveYSpeed = Math.Min(0, moveYSpeed.i);
            }

            var backpos = m_kPosition;
            var pos = backpos;

            var face = (moveXSpeed + extraSpeed.x) > 0 ? false : true;

            bool flag = (boxRadius > 0 ? currentBeScene.IsInBlockPlayer(EnhanceVec(pos, face, boxRadius)) : false);
            if (aiManager != null && aiManager.isAutoFight && !pauseAI ||
                sgGetCurrentState() == (int)ActionState.AS_JUMPBACK)
                flag = false;

            var backupX = pos.x;
            bool xHaveChange = false;
            //int oldx = pos.x;
            if (CanUpdateX())
            {
                pos.x += (fDeltime * m_fMoveXSpeedRate * (moveXSpeed + extraSpeed.x).i).roundInt;
            }
            //Logger.LogErrorFormat("oldx {0} newx{1} delta {2} time{3} {4}",oldx,pos.x,pos.x - oldx,iDeltime,Time.deltaTime);
            //pos.x += VInt.Conver(fDeltime.single * m_fMoveXSpeedRate.single * (moveXSpeed.scalar + extraSpeed.vec3.x));
            if ((this as BeActor) != null &&
                (currentBeScene.IsInBlockPlayer(pos) || flag) &&
                stateController.HasAbility(BeAbilityType.BLOCK))
            {
                //Logger.LogError("in block x!!!!");
                TriggerEventNew(BeEventType.onXInBlock);
            }
            else
            {
                m_kPosition.x = pos.x;
                xHaveChange = true;
            }

            if (currentBeScene.IsInBlockPlayer(pos) || flag)
            {
                OnXInBlock();
            }
            if (!(currentBeScene.IsInBlockPlayer(pos) || flag))
            {
                m_kBlockPosition.x = pos.x;
            }

            pos = backpos;

            //如果X轴位置发生变化 则当前Pos的X轴坐标需要刷新 避免下面Y轴阻挡判定出问题
            if (xHaveChange)
            {
                pos.x = m_kBlockPosition.x;
            }

            if (CanUpdateY())
            {
                pos.y += (fDeltime * m_fMoveYSpeedRate * (moveYSpeed + extraSpeed.y).i).roundInt;
            }

            flag = (boxRadius > 0 ? currentBeScene.IsInBlockPlayer(pos) : false);
            if (aiManager != null && aiManager.isAutoFight && !pauseAI ||
                sgGetCurrentState() == (int)ActionState.AS_JUMPBACK)
                flag = false;

            if ((this as BeActor) != null &&
                (currentBeScene.IsInBlockPlayer(pos) || flag) &&
                stateController.HasAbility(BeAbilityType.BLOCK))
            {
                var param = TriggerEventNew(BeEventType.onYInBlock,new EventParam() { m_Int = backupX, m_Bool = false });
                if (param.m_Bool)
                    m_kPosition.x = param.m_Int;
            }
            else
            {
                m_kPosition.y = pos.y;
            }

            if (!(currentBeScene.IsInBlockPlayer(pos) || flag))
                m_kBlockPosition.y = pos.y;

            /*
			if (m_iID ==1 && (moveXSpeed != 0.0f || moveYSpeed != 0.0f) && currentBeScene.IsInBlockPlayer(m_kPosition))
			{
				Logger.LogErrorFormat("set in block player:({0},{1},{2})", m_kPosition.x, m_kPosition.y, m_kPosition.z);
			}*/

            var b = currentBeScene.InEventArea(pos);
            if (b != m_eventArea)
            {
                if (m_eventArea > 0)
                {
                    TriggerEventNew(BeEventType.onExitEventArea, new EventParam() { m_Int = m_eventArea, m_Obj = this });
                }
                m_eventArea = b;
                if (m_eventArea > 0)
                {
                    TriggerEventNew(BeEventType.onEnterEventArea, new EventParam() { m_Int = m_eventArea, m_Obj = this });
                }
            }

            bool bLockZ = false;

            if (m_pkStateGraph != null && m_pkStateGraph.CurrentStateHasTag((int)AStateTag.AST_LOCKZ))
            {
                bLockZ = true;
            }

            //忽略重力
            if (!stateController.HasAbility(BeAbilityType.GRAVITY))
                bLockZ = true;

            if (!bLockZ && CanUpdateZ())
            {
                if ((m_kPosition.z > 0 || moveZSpeed != 0))
                {
                    VInt tmpZAcc = moveZAcc;
                    //下落阶段
                    if (HasTag((int)AState.ACS_FALL) && moveZSpeed <= 0)
                    {
                        tmpZAcc = moveZAcc.i * VFactor.NewVFactorF(Global.Settings.fallGravityReduceFactor, 1000);
                    }

                    if (Global.Settings.useNewGravity)
                    {
                        tmpZAcc = GetZSpeedAcc();
                    }

                    var fOldZSpeed = moveZSpeed;
                    moveZSpeed -= tmpZAcc.i * fDeltime;
                    m_kPosition.z += moveZSpeed.i * fDeltime;
                    //m_kPosition.z =  
                    if (fOldZSpeed >= 0 && moveZSpeed <= 0 && m_pkStateGraph != null)
                    {
                        m_pkStateGraph.FireEvents2CurrentStates((int)EventCommand.EVENT_COMMAND_UPSTOP);
                    }

                    if (m_kPosition.z <= 0 && !HasTag((int)AState.AST_DROPSCENE))
                    {
                        clickZSpeed = moveZSpeed;
                        m_kPosition.z = 0;
                        moveZSpeed = 0;
                        if (m_pkStateGraph != null)
                        {
                            TriggerEventNew(BeEventType.onTouchGround);
                            m_pkStateGraph.FireEvents2CurrentStates((int)EventCommand.EVENT_COMMAND_TOUCHGROUND);
                            //ExceptionManager.GetInstance().RecordLog(string.Format("entity:{0} fire EVENT_COMMAND_TOUCHGROUND t:{1}", GetName(), Time.realtimeSinceStartup));
                        }

                    }
                }
                else
                {
                    m_kPosition.z = 0;
                    moveZSpeed = 0;

                    if (m_pkStateGraph != null)
                    {
                        TriggerEventNew(BeEventType.onTouchGround);
                        m_pkStateGraph.FireEvents2CurrentStates((int)EventCommand.EVENT_COMMAND_TOUCHGROUND);
                        //	ExceptionManager.GetInstance().RecordLog(string.Format("entity:{0} fire EVENT_COMMAND_TOUCHGROUND t:{1}", GetName(), Time.realtimeSinceStartup));
                    }

                }
            }

            m_bPositionDirty = true;
        }


        if (pkRestrainPosition)
        {
            m_kPosition.x = Mathf.Clamp(m_kPosition.x, pkRestrainRangeX.x, pkRestrainRangeX.y);
        }

        if (restrainPosition && m_bPositionDirty)
        {
            //CheckReachAreaLimit();

            var oldPos = m_kPosition;

            //Vector2 logicZSize = currentBeScene.sceneData.GetLogicZSize();
            //Vector2 logicXSize = currentBeScene.sceneData.GetLogicXSize();
            VInt2 logicZSize = currentBeScene.logicZSize;
            VInt2 logicXSize = currentBeScene.logicXSize;

            m_kPosition.y = Mathf.Clamp(m_kPosition.y, logicZSize.x, logicZSize.y);
            m_kPosition.x = Mathf.Clamp(m_kPosition.x, logicXSize.x, logicXSize.y);

            /*
			if(m_kPosition.y != oldPos.y || m_kPosition.x != oldPos.x)
			{
				//防止在阻挡里
				SetStandPosition(GetPosition(), true);
			}*/

            m_bPositionDirty = false;
        }

        /*		extraSpeed.x = 0.0f;
                extraSpeed.y = 0.0f;
                extraSpeed.z = 0.0f;*/

        extraSpeed = VInt3.zero;

        inUpdatePosition = false;
    }

    protected virtual VInt GetZSpeedAcc()
    {
        return moveZAcc;
    }

    // public void CheckReachAreaLimit()
    // {
    //     /*        if (!reachAreaLimited && 
    //                 (m_kPosition.x <= (VInt)(currentBeScene.sceneData._LogicXSize.x+0.1) || 
    //                     m_kPosition.x >= (VInt)(currentBeScene.sceneData._LogicXSize.y-0.1) ||
    //                     m_kPosition.y <= (VInt)(currentBeScene.sceneData._LogicZSize.x+0.1) || 
    //                     m_kPosition.y >= (VInt)(currentBeScene.sceneData._LogicZSize.y-0.1)))
    //             {
    //                 reachAreaLimited = true;
    //                 TriggerEvent(BeEventType.onWalkToAreaLimit);
    //             }
    //             else
    //             {
    //                 reachAreaLimited = false;
    //             }*/

    // }

    // public InputManager.PressDir GetJoystickPressDir()
    // {
    //     int tmp = (m_Degree * InputManager.DegreeDiv);

    //     return InputManager.GetDir(tmp);
    // }

    public int GetJoystickDegree()
    {
        return (int)(m_Degree * InputManager.DegreeDiv); ;
    }

    public virtual void _updateMoveCmd()
    {
        if (!m_bCmdDirty)
            return;
        if (_isCmdMoveForbiden())
            return;

        m_bHaveMoveCmd = false;
        m_bCmdDirty = false;

        moveXSpeed = 0;
        moveYSpeed = 0;


        if (m_vkMoveCmd[(int)CommandMove.COMMAND_MOVE_DIRECTION])
        {
            VInt tmpx, tmpy;
            GetMoveSpeedFromDirection(out tmpx, out tmpy);

            /*	if (tmpx>=-0.000001f && tmpx<=0.000001f)
                    tmpx = 0;
                if (tmpy>=-0.000001f && tmpy<=0.000001f)
                    tmpy = 0;*/

            moveXSpeed = tmpx;
            moveYSpeed = tmpy;



            goto LabelEnd;
        }

        if (m_vkMoveCmd[(int)CommandMove.COMMAND_MOVE_X])
        {
            m_bHaveMoveCmd = true;
            moveXSpeed = speedConfig.x;
        }

        if (m_vkMoveCmd[(int)CommandMove.COMMAND_MOVE_X_NEG])
        {
            m_bHaveMoveCmd = true;
            moveXSpeed = -speedConfig.x;
        }

        if (m_vkMoveCmd[(int)CommandMove.COMMAND_MOVE_Y])
        {
            m_bHaveMoveCmd = true;
            moveYSpeed = speedConfig.y;
        }

        if (m_vkMoveCmd[(int)CommandMove.COMMAND_MOVE_Y_NEG])
        {
            m_bHaveMoveCmd = true;
            moveYSpeed = -speedConfig.y;
        }
        LabelEnd:
        if (keepXSkillSpeed)
        {
            moveXSpeed = GetFace() ? -speedConfig.x : speedConfig.x;
        }
        if (onlyMoveFacedDir && (GetFace() && moveXSpeed.i > 0 || !GetFace() && moveXSpeed.i < 0))
        {
            moveXSpeed = 0;
        }

        if (onlyMoveFaceDirOpposite && (GetFace() && moveXSpeed.i < 0 || !GetFace() && moveXSpeed.i > 0))
        {
            moveXSpeed = 0;
        }

        if (stateController.HasBuffState(BeBuffStateType.CONFUNSE))
        {
            moveXSpeed = -1 * moveXSpeed.i;
            moveYSpeed = -1 * moveYSpeed.i;
        }

        if (moveXSpeed != 0)
        {
            if (dontSetFace || (sgGetCurrentState() == (int)ActionState.AS_CASTSKILL && !skillFreeTurnFace))
            {

            }
            else
                SetFace(moveXSpeed.i < 0 ? true : false);
        }


        if (!stateController.CanMoveX())
        {
            moveXSpeed = 0;
            moveXAcc = 0;
        }
        if (!stateController.CanMoveY())
        {
            moveYSpeed = 0;
            moveYAcc = 0;
        }
        if (!stateController.CanMoveZ())
        {
            moveZSpeed = 0;
            moveZAcc = 0;
        }

        if (m_pkStateGraph != null)
        {
            m_pkStateGraph.FireEvents2CurrentStates((int)EventCommand.EVENT_COMMAND_CHANGE);
        }
        var param = TriggerEventNew(BeEventType.OnChangeSpeed, new EventParam() { m_Vint = moveXSpeed, m_Vint2 = moveYSpeed });
        moveXSpeed = param.m_Vint;
        moveYSpeed = param.m_Vint2;
    }

    public bool IsPressForwardMoveCmd()
    {
        if (IsInMoveDirection())
        {
            VInt x, y;
            GetMoveSpeedFromDirection(out x, out y);
            //!! TODO CHECK !!
            return GetFace() && x.i < -VInt.one.i / 100 || !GetFace() && x.i > VInt.one.i / 100;
        }
        else
        {
            return !GetFace() && m_vkMoveCmd[(int)CommandMove.COMMAND_MOVE_X] ||
                GetFace() && m_vkMoveCmd[(int)CommandMove.COMMAND_MOVE_X_NEG];
        }
    }

    public bool IsPressBackwardMoveCmd()
    {
        if (IsInMoveDirection())
        {
            VInt x, y;
            GetMoveSpeedFromDirection(out x, out y);
            //!! TODO CHECK !!
            return !GetFace() && x.i < -VInt.one.i / 100 || GetFace() && x.i > VInt.one.i / 100;
        }
        else
        {
            return GetFace() && m_vkMoveCmd[(int)CommandMove.COMMAND_MOVE_X] ||
                !GetFace() && m_vkMoveCmd[(int)CommandMove.COMMAND_MOVE_X_NEG];
        }
    }

    int[] temp_array_3 = new int[3];
    //object[] temp_event = new object[2];
    public virtual void _updateWorldDBox(int iDeltaTime)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeEntity._updateWorldDBox"))
        {
#endif
        BDEntityActionFrameData frameData = m_cpkCurEntityActionFrameData;
        if (frameData != null)
        {
            var pos = GetPosition();
            int x = pos.x;
            int y = pos.y * DBoxConfig.fSinA + pos.z * DBoxConfig.fCosA;
            VFactor scale = m_fScale.factor;
            if (frameData.pAttackData != null)
            {
                //int[] array = new int[3] { 0, 0, 0 };
                int[] array = temp_array_3;
                array[0] = 0;
                array[1] = 0;
                array[2] = 0;

                if (m_cpkCurEntityActionInfo != null)
                {
                    //TriggerEvent(BeEventType.onChangeAttackDBox, new object[] { m_cpkCurEntityActionInfo.skillID, array });
                    //temp_event[0] = m_cpkCurEntityActionInfo.skillID;
                    //temp_event[1] = array;
                    //TriggerEvent(BeEventType.onChangeAttackDBox, temp_event);

                    TriggerEventNew(BeEventType.onChangeAttackDBox, new EventParam() { m_Int = m_cpkCurEntityActionInfo.skillID, m_Obj = array });

                }
                var align = array[0];
                var xRate = VFactor.NewVFactor(array[1], GlobalLogic.VALUE_1000);
                var yRate = VFactor.NewVFactor(array[2], GlobalLogic.VALUE_1000);
                
                var localbox = frameData.pAttackData.vBox;
                for (int i = 0; i < localbox.Count; ++i)
                {
                    if (DBoxConfig.b2D)
                    {
                        var vBox = localbox[i].vBox;

                        if (!xRate.IsZero || !yRate.IsZero)
                        {
                            var _x = (vBox._max.x - vBox._min.x) * xRate;
                            var _y = (vBox._max.y - vBox._min.y) * yRate;
                            if (align == 0)
                            {
                                vBox._max.x += _x / 2;
                                vBox._min.x -= _x / 2;
                                vBox._max.y += _y / 2;
                                vBox._min.y -= _y / 2;
                            }
                            else
                            {
                                vBox._max.x += _x;
                                vBox._max.y += _y / 2;
                                vBox._min.y -= _y / 2;
                            }
                        }

                        m_vkCurWorldAttackBox[i].vWorldBox.offset(vBox, x, y,scale * skillAttackScale,GetFace());
                    }
                    //  else
                    //                     {
                    //                         DBox3 box
                    //                        = new DBox3(localbox[i].vBox, frameData.pAttackData.zDim);
                    //                         m_vkCurWorldAttackBox.vBox[i].vWorld3Box.offset(
                    //                             box, pos.x, pos.z, pos.y, m_fScale, GetFace());
                    //                     }
                }
            }

            if (frameData.pDefenseData != null)
            {
                var localbox = frameData.pDefenseData.vBox;
                for (int i = 0; i < localbox.Count; ++i)
                {
                    if (DBoxConfig.b2D)
                    {
                        m_vkCurWorldDefenseBox[i].vWorldBox.offset(localbox[i].vBox, x, y, scale, GetFace());
                    }
                    //   else
                    //                     {
                    //                         DBox3 box
                    //                       = new DBox3(localbox[i].vBox, 0.0f);
                    //                         m_vkCurWorldDefenseBox.vBox[i].vWorld3Box.offset(
                    //                             box, pos.x, pos.z, pos.y, m_fScale, GetFace());
                    //                     }
                }
            }

            /*
            if (frameData.pMoveData != null)
            {
                float fSpeed = frameData.pMoveData.fMoveSpeed * (m_bFaceLeft ? -1.0f : 1.0f);
                m_kPosition.x += fSpeed * iDeltaTime / 1000.0f;
                m_bPositionDirty = true;
            }*/
        }
#if ENABLE_PROFILER
        }
#endif
    }

    //int[] temp_array_1 = new int[1];
    // VInt3[] temp_vint3_1 = new VInt3[1];
    // object[] temp_event_1 = new object[1];
    public virtual void _calcAttack()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeEntity._calcAttack"))
        {
#endif
        BDEntityActionFrameData frameData = m_cpkCurEntityActionFrameData;
        if (frameData != null && frameData.pAttackData != null && frameData.pAttackData.vBox.Count > 0)
        {
            int iEntityCount = currentBeScene.GetEntityCount();

            DBox rkTemp = new DBox();

            int iAttackSize, iDefenseSize;

            //定义哪些对象可以被攻击
            int attackHurtType = frameData.pAttackData.hurtType;

            for (int i = 0; i < iEntityCount; ++i)
            {
                BeEntity pkEntity = currentBeScene.GetEntityAt(i);
                if (pkEntity == null)
                    continue;

                //攻击对象拥有混乱能力则可以被任何人攻击
                if (pkEntity.stateController.IsChaosState())
                {
                    attackHurtType = (int)ProtoTable.EffectTable.eEffectTargetType.H_NONE;
                }
                else if (CheckCanAttackFriend(pkEntity))
                {
                    attackHurtType = (int)ProtoTable.EffectTable.eEffectTargetType.H_FRIEND;
                }
                else
                {
                    attackHurtType = frameData.pAttackData.hurtType;
                }

                if ((_betrayCanAttack(pkEntity) || _canAttackedEntity(pkEntity, attackHurtType))&& !_checkEntityHurted(pkEntity))
                {
                    if (DBoxConfig.b2D)
                    {
                        var attackdata = frameData.pAttackData;

                        bool canAttack = false;

                        //int[] zDimArray = new int[1];
                        //int[] zDimArray = temp_array_1;
                        //zDimArray[0] = GlobalLogic.VALUE_1000;

                        int zDimTemp = GlobalLogic.VALUE_1000;
                        if (m_cpkCurEntityActionInfo != null)
                        {
                            var param = TriggerEventNew(BeEventType.onChangeAttackZDim, new EventParam() { m_Int = m_cpkCurEntityActionInfo.skillID, m_Int2 = zDimTemp });
                            zDimTemp = param.m_Int2;
                        }

                        int zDim = attackdata.zDimInt.i * (Global.Settings.zDimFactor * m_fZdimScaleFactor) * VFactor.NewVFactor(zDimTemp, GlobalLogic.VALUE_1000);

                        int r = pkEntity.GetEnhanceRadius();
                        if (r != 0)
                        {
                            int y1 = pkEntity.GetPosition().y - r - zDim;
                            int y2 = pkEntity.GetPosition().y + r + zDim;
                            canAttack = GetPosition().y >= y1 && GetPosition().y <= y2;
                        }
                        else
                        {
                            int iDis = (pkEntity.GetPosition().y - GetPosition().y);
                            canAttack = iDis >= -zDim && iDis <= zDim;
                        }

                        //VInt zDim = new VInt(attackdata.zDim);

                        //if (iDis >= -zDim && iDis <= zDim)
                        if (canAttack)
                        {
                            var it = pkEntity.m_cpkCurEntityActionFrameData;
                            if (it != null && it.pDefenseData != null)
                            {
                                var defensedata = it.pDefenseData;

                                iAttackSize = attackdata.vBox.Count;
                                iDefenseSize = defensedata.vBox.Count;

                                if (CheckIntersectEx(m_vkCurWorldAttackBox,
                                   iAttackSize,
                                   pkEntity.m_vkCurWorldDefenseBox,
                                   iDefenseSize,
                                   ref rkTemp))
                                {
                                    //rkLastIntersetBox = rkTemp;

                                    var hitPos = GetHitPos(rkTemp, pkEntity);


                                    /*VInt3[] posArr = temp_vint3_1;
                                    posArr[0] = hitPos;
                                    temp_event_1[0] = posArr;
                                    pkEntity.TriggerEvent(BeEventType.onChangeBeHitEffectPos, temp_event_1);
                                    _onHurtEntity(pkEntity, posArr[0], frameData.pAttackData.hurtID);*/
                                    
                                    var ret = pkEntity.TriggerEventNew(BeEventType.onChangeBeHitEffectPos, new EventParam{m_Vint3 = hitPos});
                                    hitPos = ret.m_Vint3;
                                    
                                    _onHurtEntity(pkEntity, hitPos, frameData.pAttackData.hurtID, AttackProcessId);
                                }

                            }
                        }
                    }
                    else
                    {
                        /*  var attackdata = frameData.pAttackData;
                          var it = pkEntity.m_cpkCurEntityActionFrameData;
                          if (it != null && it.pDefenseData != null)
                          {
                              var defensedata = it.pDefenseData;

                              iAttackSize = attackdata.vBox.Count;
                              iDefenseSize = defensedata.vBox.Count;

                              if (CheckIntersect3Ex(m_vkCurWorldAttackBox,
                                                  iAttackSize,
                                                  pkEntity.m_vkCurWorldDefenseBox,
                                                  iDefenseSize,
                                      ref rkTemp3)
                                  )
                              {
                                  rkTemp._min.x = rkTemp3._min.x;
                                  rkTemp._min.y = rkTemp3._min.y;
                                  rkTemp._max.x = rkTemp3._max.x;
                                  rkTemp._max.y = rkTemp3._max.y;
                                  rkLastIntersetBox = rkTemp;
                                  //_onHurtEntity(pkEntity, rkTemp);
                              }

                          }*/
                    }
                }
            }
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public virtual void _updateGraphicActor(bool force = false)
    {
#if !LOGIC_SERVER
        if (m_pkGeActor != null && (m_bGraphicPositionDirty || force))
        {
            //Logger.Log(string.Format("update postion:({0},{1},{2})", m_kPosition.x, m_kPosition.y, m_kPosition.z));
            var pos = GetPosition();
            Vector3 des = pos.vector3; //new Vector3(pos.x,pos.z,pos.y);

            m_pkGeActor.SetPosition(des);
            if (dontSetFace)
            {

            }
            else
                m_pkGeActor.SetFaceLeft(GetFace());

            m_pkGeActor.SetFootIndicatorTouchGround(des.y);
            m_pkGeActor.SetScale(m_fScale.scalar);

            m_bGraphicPositionDirty = false;
        }
#endif
    }

    public virtual void _clearStates()
    {
        moveXAcc = 0;
        moveYAcc = 0;

        m_bCmdDirty = true;
        m_vHurtEntity.Clear();
    }

    public void ResetCmdDirty()
    {
        m_bCmdDirty = true;
    }

    public void ResetWeight()
    {
        if (GetEntityData() != null)
            SetMoveSpeedZAcc(GetEntityData().weight);
    }

    public void RestoreWeight()
    {
        var data = GetEntityData();
        if (data != null)
        {
            if (data.backupWeight > 0)
            {
                SetMoveSpeedZAcc(data.backupWeight);
                data.backupWeight = 0;
            }
        }
    }

    public void SetStandardWeight()
    {
        var data = GetEntityData();
        if (data != null)
        {
            data.backupWeight = moveZAcc;
            SetMoveSpeedZAcc((VInt)Global.Settings.gravity);
        }
    }

    /*
    public string GetActionHandler(int eType)
    {
        return m_vActionHandler[eType];
    }

    public void SetActionHandler(int eType, string acAction)
    {
        m_vActionHandler[eType] = acAction;
    }
    */

    public BDEntityActionInfo GetSkillActionInfo(int skillid)
    {
        if (m_pkGeActor == null)
            return null;

        string strSkillData = GetSkillDataNameByID(skillid);//m_pkGeActor.GetSkillDataNameByID(skillid);
        BDEntityActionInfo actionInfo = null;

        if (strSkillData != null)
        {
            actionInfo = m_cpkEntityInfo._vkActionsMap[strSkillData];
        }

        return actionInfo;
    }

    public virtual void _onReset() { }

    public virtual bool _canAttackedEntity(BeEntity pkEntity, int hitType = 1)
    {
        if (!CheckCondition(pkEntity)) return false;
        //打敌人
        if (hitType == (int)ProtoTable.EffectTable.eEffectTargetType.H_ENEMY && pkEntity.m_iCamp == m_iCamp)
            return false;
        //给已方用
        if (hitType == (int)ProtoTable.EffectTable.eEffectTargetType.H_FRIEND && pkEntity.m_iCamp != m_iCamp)
            return false;

        return true;
    }

    /// <summary>
    /// 背叛情况下攻击判定
    /// </summary>
    /// <param name="pkEntity"></param>
    /// <returns></returns>
    private bool _betrayCanAttack(BeEntity pkEntity)
    {
        if (!CheckCondition(pkEntity)) return false;

        if (this is BeProjectile)
        {
            if (owner == pkEntity) return false;
        }
        if (owner.stateController == null || pkEntity.stateController == null) return false;
        if (IsSummonMonster() || owner.IsSummonMonster())
            return false;
        if (owner.stateController.CanAttackFriendAndEnemy() || pkEntity.stateController.CanAttackFriendAndEnemy())
            return true;
        return false;
    }

    private bool CheckCondition(BeEntity pkEntity)
    {
        if (pkEntity == null)
            return false;

        if (pkEntity.m_bCanBeAttacked == false)
            return false;

        if (pkEntity == this)
            return false;

        if (pkEntity.sgGetCurrentState() == (int)ActionState.AS_DEAD)
            return false;

        if (pkEntity.m_cpkCurEntityActionFrameData == null)
            return false;

        if (pkEntity.m_cpkCurEntityActionFrameData.pDefenseData == null)
            return false;

        if (!pkEntity.stateController.CanBeHit())
            return false;
        return true;
    }

    public bool _checkEntityHurted(BeEntity pkEntity)
    {
        for (int i = 0; i < m_vHurtEntity.Count; ++i)
        {
            if (m_vHurtEntity[i] == pkEntity)
                return true;
        }

        return false;
    }

    //TODO
    public VInt3 GetHitPos(DBox rkBox, BeEntity pkEntity)
    {
        VInt3 Pos = new VInt3();
        VInt2 rkCenter = rkBox.getCenter();

        int z = 0;
        if (pkEntity != null)
        {
            z = pkEntity.GetPosition().z;
        }

        if (DBoxConfig.b2D)
        {
            //!!这里只是影响显示,X才是最重要的值
            float y = (rkCenter.fy - pkEntity.GetPosition().fy * DBoxConfig.fSinA.single) / DBoxConfig.fCosA.single;
            Pos = new VInt3(rkCenter.x, pkEntity.GetPosition().y, ((VInt)y).i);
        }
        else
        {
            //Pos = new VInt3(rkCenter.x, z, rkCenter.y);
        }

        return Pos;
    }

    public virtual bool OnDamage()
    {
        return false;
    }

    public virtual void ShowMissEffect(Vec3 Pos) {}

    static EffectsFrames DUMMY_HIT_EFF_FRAME = new EffectsFrames();
    public virtual void ShowHitEffect(Vec3 Pos, BeEntity target, int hurtID)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeEntity.ShowHitEffect"))
        {
#endif
        EffectsFrames effectInfo = DUMMY_HIT_EFF_FRAME;
    #if !LOGIC_SERVER
        effectInfo.localPosition = new Vector3(0, 0, 0);
        effectInfo.localRotation = Quaternion.identity;
        effectInfo.localScale = Vector3.one;
    #endif

        string effectPath = null;

        if (m_cpkCurEntityActionInfo.hitEffectAsset.IsValid())
        {
            effectPath = m_cpkCurEntityActionInfo.hitEffectAsset.m_AssetPath;
        }
        else
        {
            if (Utility.IsStringValid(hitEffect))
            {
                effectPath = hitEffect;
            }
            else
            {
                effectPath = Global.Settings.defaultHitEffect;
            }
        }

            //string[] pathList = new string[1];
            //pathList[0] = effectPath;
            //TriggerEvent(BeEventType.onChangeHitEffect, new object[] { hurtID, pathList });
            //effectPath = pathList[0];

        var param = TriggerEventNew(BeEventType.onChangeHitEffect, new EventParam() { m_Int = hurtID, m_String = effectPath });
        effectPath = param.m_String;

        if (effectPath != null)
        {


            var effect = CreateHitEffect(effectPath, Pos, m_cpkCurEntityActionInfo.hitEffectInfoTableId, target);//CurrentBeScene.currentGeScene.CreateEffect(effectPath, 0, Pos, 1, 1, false, GetFace());
                if (target != null && effect != null)
            {
                target.currentHitEffectNum++;

                //Logger.LogErrorFormat("hiteffect:{0}", effectPath);
                int duration = Math.Max((int)effect.GetTimeLen(), GlobalLogic.VALUE_100);
                target.AddDelayCallDealEffectNum(duration);
            }
        }

#if ENABLE_PROFILER
        }
#endif
    }

    #region CreateEffectNew
    int hitEffectInfoId = 0;
    HitEffectInfoTable hitEffectInfoItem;
    System.Random hitEffectIndexRand;
    protected GeEffectEx CreateHitEffect(string effectPath, Vec3 Pos, int hitEffectInfoTableId, BeEntity target)
    {
        GeEffectEx effect = null;

        DAssetObject asset;
        asset.m_AssetObj = null;
        asset.m_AssetPath = effectPath;

        EffectsFrames effectFrames = DUMMY_HIT_EFF_FRAME; //new EffectsFrames();
    #if !LOGIC_SERVER
        effectFrames.localPosition = Vector3.zero;
        effectFrames.localRotation = m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Actor).transform.localRotation;
        effectFrames.localScale = Vector3.one;
    #endif

        if (0 == hitEffectInfoTableId)//原有逻辑
        {
            effect = CurrentBeScene.currentGeScene.CreateEffect(asset, effectFrames, 0, Pos, 1, 1, false, GetFace());
        }
        else
        {
            if (hitEffectInfoId != hitEffectInfoTableId)//id不同再重置
            {
                hitEffectInfoId = hitEffectInfoTableId;
                hitEffectInfoItem = TableManager.GetInstance().GetTableItem<HitEffectInfoTable>(hitEffectInfoId);
                target.actionCreateEffectNum = 0;//换表格id了 置值0
            }
#if UNITY_EDITOR && !LOGIC_SERVER
            else
            {
                // 编辑器下，支持直接战斗内刷表
                hitEffectInfoItem = TableManager.GetInstance().GetTableItem<HitEffectInfoTable>(hitEffectInfoTableId);
            }
#endif 
            var offsetX = hitEffectInfoItem.OffsetX;
            var offsetY = hitEffectInfoItem.OffsetY;
            if ((target.HasTag((int)AState.ACS_FALL) && !target.HasTag((int)AState.AST_FALLGROUND)) ||
                (target.GetPosition().z > VInt.Float2VIntValue(0.1f) && !target.isFloating))
            {
                offsetX = hitEffectInfoItem.FloatOffsetX;
                offsetY = hitEffectInfoItem.FloatOffsetY;
            }
            if (null != hitEffectInfoItem && offsetX.Count > 0)
            {
                int index = 0;
                if(hitEffectInfoItem.Type == 0)
                {
                    if (hitEffectInfoItem.Loop == 1)//循环
                    {
                        index = target.actionCreateEffectNum % offsetX.Count;
                    }
                    else
                    {
                        index = Math.Min(target.actionCreateEffectNum, offsetX.Count);
                    }
                }
                else if(hitEffectInfoItem.Type == 1)
                {
                    if(null == hitEffectIndexRand)
                    {
                        hitEffectIndexRand = new System.Random();
                    }
                    index = hitEffectIndexRand.Next(offsetX.Count);
                }

                EffectsFrames effectInfo = DUMMY_HIT_EFF_FRAME;
                #if !LOGIC_SERVER
                effectInfo.localPosition = new Vector3(offsetX[index] / 100f, offsetY[index] / 100f, 0);
                effectInfo.localRotation = Quaternion.Euler(hitEffectInfoItem.RotateX[index] / 100f, hitEffectInfoItem.RotateY[index] / 100f, hitEffectInfoItem.RotateZ[index] / 100f);
                effectInfo.localScale *= hitEffectInfoItem.Scale[index] / 100f;
                #endif

                effect = CurrentBeScene.currentGeScene.CreateEffect(asset, effectInfo, 0, Pos, 1, 1, false, GetFace());
            }
        }
        return effect;
    }
    #endregion

    public void ShowHit(BeEntity pkEntity, /*DBox rkBox*/Vec3 hitPos, int hurtID = 0)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeEntity.ShowHit"))
        {
#endif
        //被击特效
        if (m_cpkCurEntityActionInfo != null)
        {


            var target = pkEntity;
            if (target != null && target.currentHitEffectNum >= target.totalHitEffectNum)
            {
                //Logger.LogErrorFormat ("target:{0} hitNum:{1}/{2}", target.GetName (), target.currentHitEffectNum, target.totalHitEffectNum);
                //return;
            }
            else
            {
                ShowMagicElementsHitEffect(hitPos, pkEntity, hurtID);
                ShowHitEffect(hitPos, pkEntity, hurtID);
            }
        }


        if (Utility.IsStringValid(pkEntity.beHitEffect))
        {
            currentBeScene.currentGeScene.CreateEffect(pkEntity.beHitEffect, 0, hitPos, 1, 1, false, GetFace());
        }

        //播放被击音效
        PlayHitSfx(pkEntity, hurtID);

        //被击震屏
#if ENABLE_PROFILER
        }
#endif
    }


    private bool ShowMagicElementsHitEffect(Vec3 hitPos, BeEntity target, int hurtID = 0)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeEntity.ShowMagicElementsHitEffect"))
        {
#endif
        if (target == null)
            return false;

        int hurtIDMagicType = (int)MagicElementType.NONE;

        var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hurtID);
        if (hurtData != null)
        {
            hurtIDMagicType = hurtData.MagicElementType;
        }
        //int[] magicElementTypeArr = new int[2];
        //magicElementTypeArr[0] = hurtIDMagicType;
        //magicElementTypeArr[1] = hurtID;
        //owner.TriggerEvent(BeEventType.onChangeMagicElement, new object[] { magicElementTypeArr });
        //hurtIDMagicType = magicElementTypeArr[0];

        var param = TriggerEventNew(BeEventType.onChangeMagicElement, new EventParam() { m_Int= hurtIDMagicType,m_Int2 = hurtID });
        hurtIDMagicType = param.m_Int;

        int resMax = Global.magicElementsEffectMap.Count;
        int delayIdx = 1;
        for (int i = 1; i < (int)MagicElementType.MAX; i++)
        {
            if (hurtData != null && !hurtData.MagicElementISuse && i != hurtIDMagicType)                    //如果触发效果表中设置装备属性攻击不生效
                continue;
            if (!attribute.HasMagicElementType(i) && i != hurtIDMagicType)
                continue;

            if (i > resMax)
                continue;

            string curEffectPath = Global.magicElementsEffectMap[i - 1];
            if (!Utility.IsStringValid(curEffectPath))
                continue;

            // delayCaller.DelayCall(delayIdx * 30, () =>
            // {

            var effect = CreateHitEffect(curEffectPath, hitPos, m_cpkCurEntityActionInfo.hitEffectInfoTableId, target);//CurrentBeScene.currentGeScene.CreateEffect(asset2, effectInfo, 0, hitPos, 1, 1, false, GetFace());
            if (effect != null)
            {
                target.currentHitEffectNum++;
                int duration = Math.Max((int)effect.GetTimeLen(), GlobalLogic.VALUE_100);
                target.AddDelayCallDealEffectNum(duration);
            }
            // }
            //);

            delayIdx++;
        }


        return true;
#if ENABLE_PROFILER
        }
#endif
    }


    public void DealHit(int hurtid, BeEntity pkEntity, VInt3 hitPos, ProtoTable.EffectTable hurtData, int skillLevel, bool isBackHit)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeEntity.DealHit"))
        {
#endif
        /*
        if (IsDead())
            return;
		*/
        ShowHit(pkEntity, hitPos.vec3, hurtid);

        //处理格挡
        if (pkEntity.stateController.HasBuffState(BeBuffStateType.BLOCK))
        {
            BeActor actor = pkEntity as BeActor;
            if (!isBackHit)
            {
                int[] vals = new int[2];
                int dis = pkEntity.GetFace() ? 1 : -1 * VInt.Float2VIntValue(Global.Settings.degangBackDistance);//0.5f;

                int duration = GlobalLogic.VALUE_100;
                //vals[0] = duration;
                //vals[1] = dis;
                //pkEntity.TriggerEvent(BeEventType.BlockSuccess, new object[] { vals });
                //duration = vals[0];
                //dis = vals[1];

                var eventParam = pkEntity.TriggerEventNew(BeEventType.BlockSuccess,new EventParam() { m_Int = duration,m_Int2 = dis });
                duration = eventParam.m_Int;
                dis = eventParam.m_Int2;

                pkEntity.actionManager.StopAll();
                pkEntity.actionManager.RunAction(BeMoveBy.Create(pkEntity, duration, pkEntity.GetPosition(), new VInt3(dis, 0, 0), false,null,true));

                currentBeScene.currentGeScene.CreateEffect(10, hitPos.vec3, GetFace());

                if (CurrentBeBattle != null)
                    CurrentBeBattle.PlaySound(2022);
            }
            else
            {

            }
        }

        //处理击中屏震
        if (pkEntity != null && currentBeScene.currentGeScene != null)
        {
#if !LOGIC_SERVER
            var actor = (this as BeActor);
            if (actor != null && actor.isLocalActor)
            {
                /*int screenShakeID = 0;
                int[] screenShakeIDArray = new int[1];
                screenShakeIDArray[0] = hurtData.ScreenShakeID;
                actor.TriggerEvent(BeEventType.onChangeScreenShakeID, new object[] { hurtid, screenShakeIDArray });
                screenShakeID = screenShakeIDArray[0];*/
                var eventData = actor.TriggerEventNew(BeEventType.onChangeScreenShakeID, new EventParam(){m_Int = hurtid, m_Int2 = hurtData.ScreenShakeID});
                int screenShakeID = eventData.m_Int2;
                if (screenShakeID > 0 && currentBeScene.currentGeScene.GetCamera().shockTime <= 0)
                {
                    ProtoTable.ScreenShakeTable tableItem = TableManager.GetInstance().GetTableItem<ProtoTable.ScreenShakeTable>(screenShakeID);
                    if (tableItem != null)
                    {
                        currentBeScene.currentGeScene.GetCamera().shockTime = tableItem.CDTime;
                        currentBeScene.currentGeScene.GetCamera().PlayShockEffect(
                            tableItem.TotalTime / 1000.0f,
                            tableItem.Num,
                            tableItem.Xrange / 1000.0f,
                            tableItem.Yrange / 1000.0f,
                            tableItem.Decelerate == 1,
                            tableItem.Xreduce / 1000.0f,
                            tableItem.Yreduce / 1000.0f,
                            tableItem.Mode,
                            tableItem.Radius / 1000.0f,
                            IsCurSkillOpenShock());
                    }
                }
                else if (hurtData.HitScreenShakeTime > 0)
                {
                    currentBeScene.currentGeScene.GetCamera().PlayShockEffect(
                        hurtData.HitScreenShakeTime / 1000f,
                        hurtData.HitScreenShakeSpeed,
                        hurtData.HitScreenShakeX / 1000f,
                        hurtData.HitScreenShakeY / 1000f,
                        2,
                        IsCurSkillOpenShock()
                    );
                }

            }
            else if (this is BeProjectile)
            {
                BeProjectile projectile = this as BeProjectile;
                BeActor owner = projectile.owner as BeActor;
                if (owner != null && owner.isLocalActor)
                {
                    /*int screenShakeID = 0;
                    int[] screenShakeIDArray = new int[1];
                    screenShakeIDArray[0] = hurtData.ScreenShakeID;
                    owner.TriggerEvent(BeEventType.onChangeScreenShakeID, new object[] { hurtid, screenShakeIDArray });
                    screenShakeID = screenShakeIDArray[0];*/
                    var eventData = owner.TriggerEventNew(BeEventType.onChangeScreenShakeID, new EventParam(){m_Int = hurtid, m_Int2 = hurtData.ScreenShakeID});
                    int screenShakeID = eventData.m_Int2;
                    
                    if (screenShakeID > 0 && currentBeScene.currentGeScene.GetCamera().shockTime <= 0)
                    {
                        ProtoTable.ScreenShakeTable tableItem = TableManager.GetInstance().GetTableItem<ProtoTable.ScreenShakeTable>(screenShakeID);
                        if (tableItem != null)
                        {
                            currentBeScene.currentGeScene.GetCamera().shockTime = tableItem.CDTime;
                            currentBeScene.currentGeScene.GetCamera().PlayShockEffect(
                                tableItem.TotalTime / 1000.0f,
                                tableItem.Num,
                                tableItem.Xrange / 1000.0f,
                                tableItem.Yrange / 1000.0f,
                                tableItem.Decelerate == 1,
                                tableItem.Xreduce / 1000.0f,
                                tableItem.Yreduce / 1000.0f,
                                tableItem.Mode,
                                tableItem.Radius / 1000.0f,
                                owner.IsCurSkillOpenShock()
                                );
                        }
                    }
                }
            }
#endif
        }

        if (pkEntity != null && pkEntity.needHitShader && pkEntity.m_pkGeActor != null)
        {
            //被击高亮效果
            if (!pkEntity.IsDead())
            {
                pkEntity.AddChangeBeHitSurfaceDelayCall(50);
            }
                //pkEntity.delayCaller.DelayCall(50, () =>
                //{
                //    if (pkEntity != null && pkEntity.m_pkGeActor != null && !pkEntity.IsDead())
                //        pkEntity.m_pkGeActor.ChangeSurface("受击", Global.Settings.hitTime);
                //});
        }

        //清状态
        if (hurtData != null && hurtData.ClearTargetState.Count > 0 && hurtData.ClearTargetState[0] > 0)
        {
            for (int i = 0; i < hurtData.ClearTargetState.Count; ++i)
            {
                pkEntity.SetTag(hurtData.ClearTargetState[i], false);
            }
        }

        //不能被破招，并且没有中石化和冰冻
        if (CheckBeHitCondition(pkEntity,hurtData))
        {
            if (!needHitShader && pkEntity != null && pkEntity.m_pkGeActor != null)
            {
                var data = TableManager.GetInstance().GetTableItem<ProtoTable.BuffTable>(hurtData.BuffID);
                if(data == null || !Utility.IsStringValid(data.EffectShaderName))
                {
                    pkEntity.AddChangeBeHitSurfaceDelayCall(50);
                }
                ////被击高亮效果
                //pkEntity.delayCaller.DelayCall(50, () =>
                //{
                //    if (pkEntity != null && pkEntity.m_pkGeActor != null && !pkEntity.IsDead())
                //    {
                //        var data = TableManager.GetInstance().GetTableItem<ProtoTable.BuffTable>(hurtData.BuffID);
                //        //如果buff会附加其他shader，则不使用受击shader
                //        if (data != null && Utility.IsStringValid(data.EffectShaderName))
                //            return;
                //        pkEntity.m_pkGeActor.ChangeSurface("受击", Global.Settings.hitTime);
                //    }
                //});
            }

            if (hurtData.HitEffect == ProtoTable.EffectTable.eHitEffect.NO_EFFECT && !(hurtData.HitDeadFall > 0 && pkEntity.IsDead()))
                return;

            //bool[] hitNoEffects = new bool[] { false };
            //owner.TriggerEvent(BeEventType.onHitEffect, new object[] { hitNoEffects, hurtid, hurtData, pkEntity });

            var eventParam = owner.TriggerEventNew(BeEventType.onHitEffect, new EventParam() {m_Bool = false,m_Int = hurtid,m_Obj = hurtData, m_Obj2 = pkEntity });

            if (eventParam.m_Bool)
            {
                return;
            }

            bool face = !GetFace();
            if (hurtData.ChangeFaceDir)
                face = !face;

            if (hurtData.HitSpreadOut == 1)                //攻击时将攻击目标击散
			{
                VInt3 attackPos = VInt3.zero;
                if (this is BeProjectile)
                    attackPos = owner.GetPosition();
                else
                    attackPos = GetPosition();

                if (pkEntity.GetPosition().x > attackPos.x)
                    face = true;
                else
                    face = false;
            }
			
            //倒地状态下暂时不设方向
            if (!pkEntity.HasTag((int)AState.AST_FALLGROUND))
            {
                pkEntity.SetFace(face);
            }

            int attack = 0;
            int floatingRate = 0;
            int attackForceAcc = 0;
            int forcexAccTime = 0;
            int floatForceAcc = 0;
            int forceyAccTime = 0;
            int hurtAction = 0;

            if (BattleMain.IsChijiNeedReplaceHurtId(hurtid, battleType))
            {
                var chijiEffectMapTable = TableManager.instance.GetTableItem<ChijiEffectMapTable>(hurtid);
                attack = TableManager.GetValueFromUnionCell(chijiEffectMapTable.Attack, skillLevel);
                floatingRate = TableManager.GetValueFromUnionCell(chijiEffectMapTable.FloatingRate, skillLevel);
            }
            else
            {
                attack = TableManager.GetValueFromUnionCell(hurtData.Attack, skillLevel);
                floatingRate = TableManager.GetValueFromUnionCell(hurtData.FloatingRate, skillLevel);
                attackForceAcc = TableManager.GetValueFromUnionCell(hurtData.AttackForceAcc, skillLevel);
                forcexAccTime = TableManager.GetValueFromUnionCell(hurtData.AttackForceAccTime, skillLevel);
                floatForceAcc = TableManager.GetValueFromUnionCell(hurtData.FloatForceAcc, skillLevel);
                forceyAccTime = TableManager.GetValueFromUnionCell(hurtData.FloatForceAccTime, skillLevel);
                hurtAction = hurtData.HitActionType;
            }

            VFactor forcex = new VFactor(attack, (GlobalLogic.VALUE_1000) * (face ? 1 : -1));
            VFactor forcey = new VFactor(floatingRate, GlobalLogic.VALUE_1000);
            VFactor forcexAcc = new VFactor(attackForceAcc, GlobalLogic.VALUE_1000);
            VFactor forceyAcc = new VFactor(floatForceAcc, GlobalLogic.VALUE_1000);
                
            //int[] xForceArray = new int[2];
            //xForceArray[0] = (int)forcex.nom;
            //xForceArray[1] = GlobalLogic.VALUE_1000;
            //pkEntity.TriggerEvent(BeEventType.onChangeXRate, new object[] { hurtData.ID, xForceArray });
            //if (this != null) 
            //{
            //    ChangeXForce(xForceArray, pkEntity, this, face);
            //}
            //int forceX = xForceArray[0] * VFactor.NewVFactor(xForceArray[1], GlobalLogic.VALUE_1000);

            var param = pkEntity.TriggerEventNew(BeEventType.onChangeXRate, new EventParam() { m_Vint3 = new VInt3(hurtData.ID, (int)forcex.nom, GlobalLogic.VALUE_1000)});
            int forceX = param.m_Vint3.y * VFactor.NewVFactor(param.m_Vint3.z, GlobalLogic.VALUE_1000);
            forcex = VFactor.NewVFactor(forceX, forcex.den);
                
            //int[] yForceArray = new int[2];
            //yForceArray[0] = (int)forcey.nom;
            //yForceArray[1] = GlobalLogic.VALUE_1000;
            //pkEntity.TriggerEvent(BeEventType.onChangeFloatingRate, new object[] { hurtData.ID, yForceArray });
            //int force = yForceArray[0] * VFactor.NewVFactor(yForceArray[1], GlobalLogic.VALUE_1000);
            
            var param1 = pkEntity.TriggerEventNew(BeEventType.onChangeFloatingRate, new EventParam() { m_Vint3 = new VInt3(hurtData.ID, (int)forcey.nom, GlobalLogic.VALUE_1000) });
            int force = param1.m_Vint3.y * VFactor.NewVFactor(param1.m_Vint3.z, GlobalLogic.VALUE_1000);
            forcey = VFactor.NewVFactor(force, forcey.den);

            ActionState hurtState = ActionState.AS_HURT;
            if (ProtoTable.EffectTable.eHitEffect.HITFLY == hurtData.HitEffect)
                hurtState = ActionState.AS_FALL;

            //打到空中的时候会有一个向上的力
            if ((pkEntity.HasTag((int)AState.ACS_FALL) &&
                !pkEntity.HasTag((int)AState.AST_FALLGROUND)))
            {
                int floatForceY = TableManager.GetValueFromUnionCell(hurtData.HitFloatYForce, skillLevel);
                    
                //int[] yForceFloatingArray = new int[2];
                //yForceFloatingArray[0] = floatForceY;
                //yForceFloatingArray[1] = GlobalLogic.VALUE_1000;
                //pkEntity.TriggerEvent(BeEventType.onChangeFloatYForce, new object[] { hurtData.ID, yForceFloatingArray });
                //floatForceY = yForceFloatingArray[0] * VFactor.NewVFactor(yForceFloatingArray[1], GlobalLogic.VALUE_1000);

                var param2 = pkEntity.TriggerEventNew(BeEventType.onChangeFloatYForce, new EventParam() {m_Vint3 = new VInt3(hurtData.ID, floatForceY, GlobalLogic.VALUE_1000) });
                floatForceY = param2.m_Vint3.y * VFactor.NewVFactor(param2.m_Vint3.z, GlobalLogic.VALUE_1000);

                if (floatForceY > 0)
                {
                    forcey = new VFactor(floatForceY, GlobalLogic.VALUE_1000);
                }

                int floatForceX = 0;
                int floatForceXAcc = 0;
                if (BattleMain.IsChijiNeedReplaceHurtId(hurtid, battleType))
                {
                    var chijiEffectMapTable = TableManager.instance.GetTableItem<ChijiEffectMapTable>(hurtid);
                    floatForceX = TableManager.GetValueFromUnionCell(chijiEffectMapTable.HitFloatXForce, skillLevel);
                }
                else
                {
                    floatForceX = TableManager.GetValueFromUnionCell(hurtData.HitFloatXForce, skillLevel);
                    floatForceXAcc = TableManager.GetValueFromUnionCell(hurtData.HitFloatXForceAcc, skillLevel);
                    forcexAccTime = TableManager.GetValueFromUnionCell(hurtData.HitFloatXForceAccTime, skillLevel);
                }

                //int[] xForceFloatingArray = new int[2];
                //xForceFloatingArray[0] = floatForceX;
                //xForceFloatingArray[1] = GlobalLogic.VALUE_1000;
                //pkEntity.TriggerEvent(BeEventType.onChangeFloatXForce, new object[] { hurtData.ID, xForceFloatingArray });

                EventParam paramResult = new EventParam();
                VInt3 vint3Data = new VInt3(hurtData.ID, floatForceX, GlobalLogic.VALUE_1000);
                var param3 = pkEntity.TriggerEventNew(BeEventType.onChangeFloatXForce, new EventParam() {m_Vint3 = vint3Data });
                paramResult = param3;
                if (this != null)
                {
                    paramResult = ChangeXForce(paramResult, pkEntity, this, face);
                }
                floatForceX = paramResult.m_Vint3.y * VFactor.NewVFactor(paramResult.m_Vint3.z, GlobalLogic.VALUE_1000);

                forcex = new VFactor(floatForceX, GlobalLogic.VALUE_1000) * _getFaceCoff();

                forcexAcc = new VFactor(floatForceXAcc, GlobalLogic.VALUE_1000);

                hurtState = ActionState.AS_FALL;
            }

            //正常在空中被打到的情况
            if (hurtState != ActionState.AS_FALL /*&& pkEntity.IsCastingSkill()*/ && pkEntity.GetPosition().z > VInt.Float2VIntValue(0.1f) && !pkEntity.isFloating)
            {
                hurtState = ActionState.AS_FALL;
                forcex = VFactor.one;
                forcey = VFactor.NewVFactorF(0.1f, GlobalLogic.VALUE_1000);
            }


            bool isGrab = false;
            if (hurtData.UseStandardWeight)
            {
                isGrab = true;
                pkEntity.SetStandardWeight();



                if (pkEntity.HasTag((int)AState.AST_FALLGROUND))
                {
                    forcex *= 2;
                    forcey *= 2;
                }
            }

            if (hurtData.UseNoBlock)
            {
                if (pkEntity.stateController.HasAbility(BeAbilityType.BLOCK))
                {
                    pkEntity.changeToNoBlock = true;
                    pkEntity.stateController.SetAbilityEnable(BeAbilityType.BLOCK, false);
                }
            }

            int hurtTime = (int)attribute.GetHurtFrozenTime(pkEntity.GetEntityData(), hurtid);
            //最小设置为150毫秒,最大值为600毫秒
            hurtTime = Mathf.Clamp(hurtTime, TableManager.instance.gst.hurtTime[0], TableManager.instance.gst.hurtTime[1]);
            //设置最大僵直距离
            pkEntity.FrozenDisMax = attribute.GetFrozenDisMax(hurtid);

            //不能被浮空的情况
            if (hurtState == ActionState.AS_FALL && !pkEntity.stateController.CanBeFloat())
            {
                hurtState = ActionState.AS_HURT;
            }

            //不能被平推
            if (!pkEntity.stateController.CanBeFloat())
            {
                forcex = VFactor.zero;
                forcey = VFactor.zero;
            }

            if (hurtState == ActionState.AS_HURT)
                forcey = VFactor.zero;
            if (hurtData.HitDeadFall > 0 && pkEntity.IsDead() && hurtState == ActionState.AS_HURT)
            {
                hurtState = ActionState.AS_FALL;
                forcex = (pkEntity.GetPosition().x - GetPosition().x) > 0 ? VFactor.NewVFactorF(3f, GlobalLogic.VALUE_1000) : VFactor.NewVFactorF(-3f, GlobalLogic.VALUE_1000);
                forcey = VFactor.NewVFactorF(5f, 1000);
                if (pkEntity.aiManager != null)
                    pkEntity.aiManager.Stop();
            }

            //束缚状态不能进行X轴移动
            if (JudgeChangeForceXByStrain(pkEntity))
            {
                forcex = VFactor.zero;
            }
            
            //pkEntity.Locomote(new BeStateData((int)hurtState, isGrab ? 1 : 0, forcex.vint.i, forcey.vint.i, forcexAcc.vint.i, forceAccTime, hurtTime, true));
            pkEntity.Locomote(new BeStateData((int)hurtState)
            {
                _StateData = isGrab ? 1 : 0,
                _StateData2 = forcex.vint.i,
                _StateData3 = forcey.vint.i,
                _StateData4 = forcexAcc.vint.i,
                _StateData5 = forcexAccTime,
                _StateData6 = forceyAcc.vint.i,
                _StateData7 = forceyAccTime,
                _HurtAction = hurtAction,
                _timeout = hurtTime,
                _timeoutForce = true
            });
        }
        else
        {
            if (pkEntity.IsDead())
            {
                if (pkEntity.GetPosition().z > 0)
                {
                    var pos = pkEntity.GetPosition();
                    pos.z = 0;
                    pkEntity.SetMoveSpeedZ(VInt.zero);
                    pkEntity.SetStandPosition(pos, true);

                }
                pkEntity.DoDead();
            }
        }

        //角色攻击是否顿帧
        if (hurtData.HitPause)
        {
            delayCaller.DelayCall(30, () =>
            {
                int pauseTime = hurtData.HitPauseTime;
                bool effectPause = !hurtData.HitEffectPause;

                Pause(pauseTime, effectPause);
            });
        }

        //被击的目标是否顿帧
        if (hurtData.HitTargetPause)
        {
            delayCaller.DelayCall(30, () =>
            {
                int pauseTime = hurtData.HitTargetPauseTime;
                //目标在空中的话，顿帧时间不一样
                if (pkEntity.GetPosition().z > VInt.Float2VIntValue(0.01f) && !pkEntity.isFloating)
                {
                    pauseTime = hurtData.FloatTargetPauseTime;
                }
                bool effectPause = !hurtData.HitEffectPause;

                BeActor actor = pkEntity as BeActor;
                if (actor != null)
                {
                    if (!actor.buffController.HaveBatiNoPauseBuff())
                    {
                        pkEntity.Pause(pauseTime, effectPause);
                    }
                }
                else
                {
                    pkEntity.Pause(pauseTime, effectPause);
                }
            });
        }

        if (hurtData.HitEffect != ProtoTable.EffectTable.eHitEffect.NO_EFFECT)
            OnDealHit(pkEntity);
#if ENABLE_PROFILER
        }
#endif
    }

    /// <summary>
    ///  判断是否需要进入被击状态
    /// </summary>
    private bool CheckBeHitCondition(BeEntity pkEntity,ProtoTable.EffectTable hurtData)
    {
        if (pkEntity.m_iCamp == m_iCamp)
            return false;
        if (pkEntity.stateController.HasBuffState(BeBuffStateType.FROZEN))
            return false;
        if (pkEntity.stateController.HasBuffState(BeBuffStateType.STONE))
            return false;
        if (!pkEntity.stateController.CanBeForceBreakAction())
            return false;
        if (!pkEntity.stateController.CanBeBreakAction() && !hurtData.UseStandardWeight)
            return false;
        BeActor actor = (BeActor)pkEntity;
        if (actor != null && actor.protectManager != null && actor.protectManager.IsEnterHardProtect() && hurtData.HitEffect == ProtoTable.EffectTable.eHitEffect.HIT)
            return false;
        return true;
    }

    //调整束缚状态下的X轴推力
    private bool JudgeChangeForceXByStrain(BeEntity entity)
    {
        if (!BeClientSwitch.FunctionIsOpen(ClientSwitchType.Strain))
            return false;
        if (!entity.stateController.HasBuffState(BeBuffStateType.STRAIN))
            return false;
        return true;
    }

    public virtual void OnDealHit(BeEntity pkEntity)
    {
    }

    public virtual void _onHurtEntity(BeEntity pkEntity, VInt3 hitPos/*DBox rkBox*/, int hurtid = 0, uint attackProcessId = 0)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeEntity._onHurtEntity"))
        {
#endif
        if (pkEntity == null)
            return;

        if (!pkEntity.stateController.CanBeHit())
            return;
        int finalDamage = 0;
        
        var eventData = owner.TriggerEventNew(BeEventType.OnHurtEnter, new EventParam(){m_Int = hurtid, m_Bool = true, m_Obj = pkEntity});
        if (!eventData.m_Bool)
            return;
        
        hurtid = eventData.m_Int;
        var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hurtid);
        if (hurtData == null)
        {
            if (m_cpkCurEntityActionInfo != null)
            {
                Logger.LogWarning(string.Format("{0} {1}触发效果表 没有ID为 {2} 的条目", GetName(), m_cpkCurEntityActionInfo.actionName, hurtid));
            }

            return;
        }

        //only hit grabed target
        if (hurtData.HitGrab && !pkEntity.IsGrabed())
        {
            return;
        }

        m_vHurtEntity.Add(pkEntity);

        //用于截断伤害
        if (owner.CurrentBeScene != null)
        {
            EventParam param = owner.CurrentBeScene.TriggerEventNew(BeEventSceneType.onHurtEntity, new EventParam() { m_Int = hurtid, m_Vint3 = hitPos, m_Obj = this, m_Obj2 = pkEntity, m_Obj3  = hurtData});
            if (param.m_Int <= 0)
                return;
            hurtid = param.m_Int;
            hurtData = param.m_Obj3 as ProtoTable.EffectTable;
        }
        
        int skillLevel = 1;

        //魔法伤害加成 和 物理伤害加成 要及时反映到伤害结果中
        BeActor attacker = this as BeActor;
        //如果是实体则由实体的召唤者触发
        if (attacker == null)
        {
            attacker = GetTopOwner(owner) as BeActor;
        }
        if (attacker != null && attacker.buffController != null)
        {
            if (hurtData.DamageType == ProtoTable.EffectTable.eDamageType.MAGIC)
                attacker.buffController.TriggerBuffs(BuffCondition.MAGIC_ATTACK, pkEntity as BeActor);
            else if (hurtData.DamageType == ProtoTable.EffectTable.eDamageType.PHYSIC)
                attacker.buffController.TriggerBuffs(BuffCondition.PHYSIC_ATTACK, pkEntity as BeActor);
        }
        //可破坏物等的特殊处理
        if (pkEntity.OnDamage())
        {
            onHitEntity(pkEntity, VInt3.zero);
            pkEntity.m_pkGeActor.ChangeSurface("受击", Global.Settings.hitTime);
            ShowHit(pkEntity, hitPos.vec3, hurtid);

            return;
        }

        if (hurtData.HasDamage == 0)
        {
            onHitEntity(pkEntity, VInt3.zero);
            return;
        }

        DealRepeatHurt(hurtData);

        //var hitPos = GetHitPos(rkBox, pkEntity); 
        bool isBackHit = false;

        //不能被背击
        if (pkEntity.stateController.HasAbility(BeAbilityType.CAN_HIT_BACK))
        {
            VInt offset = (VInt)0.2f;
            if (hurtData != null && hurtData.IsFriendDamage == 0 &&
                (pkEntity.GetFace() && hitPos.x > (pkEntity.GetPosition().x + offset) || !pkEntity.GetFace() && hitPos.x <= (pkEntity.GetPosition().x - offset)))
                isBackHit = true;
        }
        

        //Logger.LogErrorFormat("hitpos.x:{0} pkentity.x:{1} face:{2} isbackhit:{3}", hitPos.x, pkEntity.GetPosition().x, pkEntity.GetFace(), isBackHit);

        bool isDoReflectDamage = false;
        int reflectDamage = 0;
        AttackResult result = DoAttackTo(pkEntity, hurtid, ref isDoReflectDamage, ref reflectDamage, ref finalDamage,hitPos, true, isBackHit, true, false, attackProcessId);

        //重载附加伤害函数
        if(result != AttackResult.MISS)
        {
            OnAttachHurt(pkEntity, hurtData);
        }

        BeEntityData data = GetEntityData();
        if (data != null)
        {
            skillLevel = data.GetSkillLevel(hurtData.SkillID);
        }

        // 注意这个流程：DoAttackTo -> protect->getup状态 -> 无敌 -> behit,无敌之后不应该进来了(DealHit->flyHit->fall 状态)
        // TODO:: onHitEntity处理
        if (result != AttackResult.MISS && pkEntity.stateController.CanBeHit())
        {
            //if (result == AttackResult.CRITICAL)
            {
                TriggerEventNew(BeEventType.onHitCritical, new EventParam() { m_Vint3 = hitPos });
                //TriggerEvent(BeEventType.onHitCritical, new object[] { hitPos });
            }


            DealHit(hurtid, pkEntity, hitPos, hurtData, skillLevel, isBackHit);

            if (isDoReflectDamage)
            {
                DoReflectHurt(reflectDamage, pkEntity);
            }

        }
        else
        {
            ShowMissEffect(hitPos.vec3);
        }

        onHitEntity(pkEntity, hitPos, hurtid, result,finalDamage);
#if ENABLE_PROFILER
        }
#endif
    }

    public void DoReflectHurt(int reflectDamage, BeEntity pkEntity)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeEntity.DoReflectHurt"))
        {
#endif
        if (attribute != null)
        {
            var prevHP = attribute.GetHP();
            DoHurt(reflectDamage, null, HitTextType.NORMAL, pkEntity);
            var curHP = attribute.GetHP();
            var hurtValue = prevHP - curHP;
            if ((this as BeProjectile) != null && owner != null)
            {
                owner.TriggerEventNew(BeEventType.onHurt, new EventParam() { m_Int = hurtValue, m_Obj = owner });
                //owner.TriggerEvent(BeEventType.onHurt, new object[] { hurtValue });
            }
        }
        /*rkBox._min.x = GetPosition().x;
		rkBox._max.x = GetPosition().x;
		rkBox._max.y = 1.0f;
		rkBox._min.y = 1.0f;*/

        VInt3 hitPos = GetPosition();
        hitPos.z += VInt.Float2VIntValue(1.0f);

        ShowHit(pkEntity, hitPos.vec3);

        //判断是否进入硬直保护
        bool enterHardprotect = false;
        BeActor actor = this as BeActor;
        if (actor != null && actor.protectManager != null && actor.protectManager.IsEnterHardProtect())
        {
            enterHardprotect = true;
        }
            
        if (stateController.CanBeBreakAction() && !enterHardprotect)
        {
            //Locomote(new BeStateData((int)ActionState.AS_HURT, 0, 0, 0, 0, 0, GlobalLogic.VALUE_150, true));
            Locomote(new BeStateData((int)ActionState.AS_HURT) { _timeout = GlobalLogic.VALUE_150, _timeoutForce = true });
        }
#if ENABLE_PROFILER
        }
#endif
    }

   // private static int kCount = 1;

    public void DealRepeatHurt(ProtoTable.EffectTable hurtData)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeEntity.DealRepeatHurt"))
        {
#endif
        if (hurtData == null)
            return;
        if (hurtData.RepeatAttackInterval[0] == 0)
            return;
        if (delayCallUnitHurtIdList.Contains(hurtData.ID))
            return;

        int skillLevel = GetEntityData().GetSkillLevel(hurtData.SkillID);
        //改变重复攻击间隔以及最大攻击次数
        /*int[] AttackIntervalList = new int[2];
        AttackIntervalList[0] = GlobalLogic.VALUE_1000;
        AttackIntervalList[1] = 0;
        owner.TriggerEvent(BeEventType.onRepeatAttackInterval, new object[] { AttackIntervalList, hurtData.ID });
        AttackIntervalList[0] = hurtData.RepeatAttackInterval[0] * VFactor.NewVFactor(AttackIntervalList[0], GlobalLogic.VALUE_1000);*/

        var eventDate = owner.TriggerEventNew(BeEventType.onRepeatAttackInterval, new EventParam(){m_Int = GlobalLogic.VALUE_1000, m_Int2 = 0, m_Int3 = hurtData.ID});
        int count = eventDate.m_Int2;
        int damage = hurtData.RepeatAttackInterval[0] * VFactor.NewVFactor(eventDate.m_Int, GlobalLogic.VALUE_1000);
        

        //添加重复攻击最大数量
        int attackCount = 9999999;
        if (hurtData.RepeatAttackInterval.Count > 1)
        {
            attackCount = hurtData.RepeatAttackInterval[1] + count - 2;          //保证表里面填1只执行一次 填2只执行两次
        }

        if (attackCount < 0)
            return;

        DelayCallUnitHandle m_DelayCallUnit = delayCaller.RepeatCall(damage, () =>
        {
            ResetDamageData();
        }, attackCount);
        delayCallUnitHurtIdList.Add(hurtData.ID);
        delayCallUnitList.Add(m_DelayCallUnit);
#if ENABLE_PROFILER
        }
#endif
    }

    public virtual void onHitEntity(BeEntity pkEntity, VInt3 pos, int hurtID = 0, AttackResult result = AttackResult.MISS,int finalDamage = 0)
    {

    }

    public void PlayHitSfx(BeEntity target, int hurtID = 0)
    {
        if (CurrentBeBattle != null && !CurrentBeBattle.NeedPlaySound)
            return;
        
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeEntity.PlayHitSfx"))
        {
#endif
        if (target != null && target.currentHitSFXNum >= target.totalHitSFXNum)
        {
            //Logger.LogErrorFormat ("target:{0} hitsfx:{1}/{2}", target.currentHitSFXNum, target.totalHitSFXNum);
            return;
        }

        int hurtIDMagicType = (int)MagicElementType.NONE;
        bool magicElementISuse = false;

        var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hurtID);
        if (hurtData != null)
        {
            hurtIDMagicType = hurtData.MagicElementType;
            magicElementISuse = hurtData.MagicElementISuse;
        }

        var param = TriggerEventNew(BeEventType.onChangeMagicElement, new EventParam() { m_Int = hurtIDMagicType,m_Int2 = hurtID });
        hurtIDMagicType = param.m_Int;

        //int[] magicElementTypeArr = new int[2];
        //magicElementTypeArr[0] = hurtIDMagicType;
        //magicElementTypeArr[1] = hurtID;
        //owner.TriggerEvent(BeEventType.onChangeMagicElement, new object[] { magicElementTypeArr });
        //hurtIDMagicType = magicElementTypeArr[0];

        // 附魔音效(目前是和普通音效一起播)
        int resMax = Global.magicElementsSoundMap.Count;
        int delayIdx = 1;
        for (int i = 1; i < (int)MagicElementType.MAX; i++)
        {
            if (!magicElementISuse && i != hurtIDMagicType)                //如果触发效果表中设置装备属性攻击不生效
                continue;
            if (!attribute.HasMagicElementType(i) && i != hurtIDMagicType)
                continue;

            if (i > resMax)
                continue;

            int curSoundIdx = Global.magicElementsSoundMap[i - 1];
            //delayCaller.DelayCall(delayIdx * 30, () =>
            //{
            uint hdl = AudioManager.instance.PlaySound(curSoundIdx);
            if (hdl > 0)
            {
                target.currentHitSFXNum++;
                int duration = AudioManager.instance.GetAudioLength(hdl);
                target.AddDelayCallDealSFXNum(duration);
                //target.delayCaller.DelayCall(duration, () =>
                //{
                //    target.currentHitSFXNum--;
                //});
            }
            //}
            //);

            delayIdx++;
        }

        //UnityEngine.Object clip = null;
        //string clipPath = null;

        var hitSfxData = defaultHitSFXData;
        if (m_cpkCurEntityActionInfo != null && m_cpkCurEntityActionInfo.hitSFXID > 0)
            hitSfxData = m_cpkCurEntityActionInfo.hitSFXIDData;
        uint handle = AudioManager.instance.PlaySound(hitSfxData);
        if (handle > 0)
        {
            target.currentHitSFXNum++;
            int duration = AudioManager.instance.GetAudioLength(handle);
            target.AddDelayCallDealSFXNum(duration);
            //target.delayCaller.DelayCall(duration, () =>
            //{
            //    target.currentHitSFXNum--;
            //});
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public void SetOwner(BeEntity o)
    {
        owner = o;
    }

    public BeEntity GetOwner()
    {
        return owner;
    }

    /// 目标是否背对释放者
    public bool IsFacingAway(BeEntity target)
    {
        if (!target.stateController.HasAbility(BeAbilityType.CAN_HIT_BACK))
            return false;
        if (target == null)
        {
            return false;
        }

        VInt3 ownerPos = GetPosition();
        if (this is BeProjectile)
        {
            ownerPos = owner.GetPosition();
        }
        var pos = target.GetPosition() - ownerPos;

        // 目标在右边
        if (pos.x > 0)
        {
            return !target.GetFace();
        }
        else
        {
            return target.GetFace();
        }
    }


    public virtual void OnBeforeGetDamage(BeEntity target, AttackResult result, bool isBackHit, int hurtID)
    {

    }

    public AttackResult DoAttackTo(BeEntity target, int hurtid, bool triggerFlashHurt = true,bool isAttachHurt = false)
    {
        bool tmp1 = false;
        int tmp2 = 0;
        int finalDamage = 0;
        return DoAttackTo(target, hurtid, ref tmp1, ref tmp2, ref finalDamage, GetPosition(), true, false, triggerFlashHurt, isAttachHurt);
    }

    private void _addHurtData2Statistics(int skillId, int hurtId, int damage)
    {
        if (null == this)
        {
            return;
        }

        if (null == CurrentBeBattle)
        {
            Logger.LogError("[战斗] 战斗对象为空");
            return;
        }

        IDungeonStatistics statistics = CurrentBeBattle.dungeonStatistics;

        if (null == statistics)
        {
            Logger.LogError("[战斗] 数据统计为空");
            return;
        }

        statistics.AddHurtData(skillId, hurtId, damage);
    }
    public bool IsBoss()
    {
        return IsMonster() && attribute.type == (int)ProtoTable.UnitTable.eType.BOSS;
    }

    //是否是敌方怪物
    public bool IsMonster()
    {
        return (attribute != null && attribute.isMonster && m_iCamp != (int)ProtoTable.UnitTable.eCamp.C_HERO);
    }
    public virtual bool IsAttackAdd2Statistics()
    {
        return false;
    }


    public AttackResult DoAttackTo(BeEntity target, int hurtid, ref bool doRefectDamage, ref int reflectDamage, ref int outFinalDamage, VInt3 hitPos, bool needCalcBackHit = true, bool isBackHit = false, bool triggerFlashHurt = true,bool isAttachHurt = false, uint attackProcessId = 0)
    {
    #if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeEntity.DoAttackTo"))
        {
    #endif

        var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUICommon>();

        int finalDamage = 1;
        outFinalDamage = 0;
        AttackResult result = AttackResult.NORMAL;

        var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hurtid);
        if (hurtData == null)
            return result;
        ProtoTable.EffectTable.eDamageType atkType = hurtData.DamageType;

        result = attribute.GetAttackResult(target.GetEntityData(), atkType, hurtid);

        if (!isAttachHurt)//附加伤害不计算Miss保护
        {
            // TODO GC 40B
            //int[] results = new int[1];
            //results[0] = (int)result;
            //target.TriggerEvent(BeEventType.OnAttackResult, new object[] { results });
            //result = (AttackResult)results[0];
            var param = target.TriggerEventNew(BeEventType.OnAttackResult, new EventParam() { m_Int = (int)result });
            result = (AttackResult)param.m_Int;
        }
        //附加伤害如果是miss则会强制设为普通攻击
        if (isAttachHurt && result == AttackResult.MISS)
        {
            result = AttackResult.NORMAL;
        }
        //if (IsProcessRecord())
        //{
        //    string zDimInt = "Empty";
        //    string vBox = "Empty";
        //    if (m_cpkCurEntityActionFrameData != null && m_cpkCurEntityActionFrameData.pAttackData != null)
        //    {
        //        zDimInt = m_cpkCurEntityActionFrameData.pAttackData.zDimInt.ToString();
        //        if (m_cpkCurEntityActionFrameData.pAttackData.vBox.Count > 0)
        //            vBox = m_cpkCurEntityActionFrameData.pAttackData.vBox[0].vBox.ToString();
        //    }

        //    GetRecordServer().RecordProcess("[BATTLE]PID:{0}-{1} attack PID:{2}-{3} ret:{4} {5} {6} {7} {8} {9} {10}",
        //        m_iID,
        //        GetName(),
        //        target.m_iID,
        //        target.GetName(),
        //        result,
        //        GetInfo(),
        //        target.GetInfo(),
        //        hurtid,
        //        m_iCurrentLogicFrame,
        //        zDimInt,
        //        vBox);
            
        //    if (m_iCurrentLogicFrame == -1)
        //    {
        //        GetRecordServer().RecordProcess("[BATTLE]PID:{0},moveName:{1} actionName:{2} skillId:{3}", 
        //            GetPID(), m_cpkCurEntityActionInfo == null ? "null" : m_cpkCurEntityActionInfo.moveName,
        //            m_pkGeActor == null ? "null" : m_pkGeActor.GetCurActionName(), 
        //            GetCurSkillID());
        //    }

        //}

        //免疫远程伤害
        if (hurtData.DamageDistanceType == ProtoTable.EffectTable.eDamageDistanceType.FAR && target.stateController.IgnoreFarDamage())
        {
            if (showDamageNumber && battleUI != null && battleUI.comShowHit != null)
                battleUI.comShowHit.ShowHitNumber(0, null, m_iID, hitPos.vec3, GetFace(), HitTextType.NORMAL, this, target);

            TriggerMiss(owner, target,0);
            return AttackResult.MISS;
        }

        //拥有只能被近身伤害能力时 只能被近身攻击伤害
        if(hurtData.DamageDistanceType != ProtoTable.EffectTable.eDamageDistanceType.NEAR && target.stateController.CanBeHitOnlyNear())
        {
            if (showDamageNumber && battleUI != null && battleUI.comShowHit != null)
                battleUI.comShowHit.ShowHitNumber(0, null, m_iID, hitPos.vec3, GetFace(), HitTextType.NORMAL, this, target);
            TriggerMiss(owner, target,1);
            return AttackResult.MISS;
        }

        if (result == AttackResult.NORMAL || result == AttackResult.CRITICAL)
        {
            if (hurtData.HasDamage == 1)
            {
                if (needCalcBackHit && hurtData.IsFriendDamage == 0)
                    isBackHit = IsFacingAway(target);

                OnBeforeGetDamage(target, result, isBackHit, hurtid);

                //伤害计算
                int damage = attribute.GetDamage(target.GetEntityData(), hurtid, result);


#if !LOGIC_SERVER
                // 伤害统计
                // by duanduan 
                if (IsAttackAdd2Statistics())
                {
                    _addHurtData2Statistics(hurtData.SkillID, hurtid, damage);
                }
#endif


                //对这次伤害结果的调整(魔法护罩)
                //int[] vals = new int[1];
                int modifyDamageValue = damage;
                if (m_iCamp == target.m_iCamp && !target.stateController.IsChaosState() && !CheckCanAttackFriend(target) && !CheckCanAttackFriendAndEnemy(target))
                    modifyDamageValue = 1;
                //vals[0] = 1;
                //else
                //    vals[0] = damage;
                //target.TriggerEvent(BeEventType.onAfterCalFirstDamage, new object[] { vals, true, hurtid});
                var modifyEvemtParam = target.TriggerEventNew(BeEventType.onAfterCalFirstDamage,new EventParam() {m_Int = modifyDamageValue,m_Bool = true,m_Int2 = hurtid });
                modifyDamageValue = modifyEvemtParam.m_Int;

                BeEntity projectileOwner = null;
                if (this is BeProjectile)
                {
                    projectileOwner = GetOwner();
                }

                //TriggerEvent(BeEventType.onAfterCalFirstDamage, new object[] { vals, false, hurtid });
                var modifyEvemtParam2 = TriggerEventNew(BeEventType.onAfterCalFirstDamage,new EventParam() {m_Int = modifyDamageValue,m_Bool = false,m_Int2 = hurtid });
                modifyDamageValue = modifyEvemtParam2.m_Int;

                //damage = vals[0];
                damage = modifyDamageValue;

                //属性伤害对不同状态的加成效果

                CrypticInt32 ELEMENT_DAMAGE_ADD = GlobalLogic.VALUE_100;

                var attackElementType = attribute.GetAttackElementType(hurtid);
                int addPercentDamage = 0;

                if (attackElementType == MagicElementType.LIGHT && target.stateController.HasBuffState(BeBuffStateType.FLASH))
                    addPercentDamage = ELEMENT_DAMAGE_ADD;
                else if (attackElementType == MagicElementType.FIRE && target.stateController.HasBuffState(BeBuffStateType.BURN))
                    addPercentDamage = ELEMENT_DAMAGE_ADD;
                else if (attackElementType == MagicElementType.ICE && target.stateController.HasBuffState(BeBuffStateType.FROZEN))
                    addPercentDamage = ELEMENT_DAMAGE_ADD;
                else if (attackElementType == MagicElementType.DARK && target.stateController.HasBuffState(BeBuffStateType.CURSE))
                    addPercentDamage = ELEMENT_DAMAGE_ADD;

                if (addPercentDamage > 0)
                    attribute.battleData.addDamagePercent.Add(new AddDamageInfo(addPercentDamage));

                //增加和附加伤害,减伤
                List<int> attachValues = GamePool.ListPool<int>.Get();
                int damage2 = 0;
                finalDamage = attribute.GetAttachDamages(target, damage, hurtData.DamageType, hurtData.DamageDistanceType, ref damage2, attachValues);

                if (addPercentDamage > 0)
                {
                    attribute.battleData.addDamagePercent.RemoveAll(item =>
                    {
                        return item.value == addPercentDamage;
                    });
                }

                //反伤
                reflectDamage = target.GetEntityData().GetReflectDamages(damage2);
                if (reflectDamage > 0)
                {
                    doRefectDamage = true;
                }

                if (!stateController.CanBeHit())
                {
                    doRefectDamage = false;
                    reflectDamage = 0;
                }


                /********************************************************************************/
                if (m_iCamp == target.m_iCamp && !target.stateController.IsChaosState() && !CheckCanAttackFriend(target) && !CheckCanAttackFriendAndEnemy(target))
                    finalDamage = 1;

                bool isTargetBackHit = isBackHit;
                if (isTargetBackHit)
                {
                    //target.TriggerEvent(BeEventType.onBackHit, new object[] { this });
                    target.TriggerEventNew(BeEventType.onBackHit,new EventParam() { m_Obj = this});

                    BeActor actor = null;
                    if ((this as BeProjectile) != null)
                    {
                        actor = GetOwner() as BeActor;
                    }
                    if ((this as BeActor) != null)
                    {
                        actor = this as BeActor;
                    }

                    if (actor != null && !actor.IsDeadOrRemoved() && actor.isLocalActor)
                    {
                        actor.m_pkGeActor.CreateHeadText(HitTextType.SPECIAL_ATTACK, "UI/Font/new_font/pic_back_hit");
                    }
                }

                //伤害躲避
                bool avoidDamage = false;
                switch (hurtData.AvoidDamageType)
                {
                    case ProtoTable.EffectTable.eAvoidDamageType.AV_FACINGAWAY:
                        avoidDamage = isTargetBackHit;
                        break;
                }
                if (avoidDamage)
                {
                    doRefectDamage = false;
                    finalDamage = 0;
                }


                //对最终伤害的调整
                //int [] vals2 = new int[1];
                //vals[0] = finalDamage;

                modifyDamageValue = finalDamage;
                //TriggerEvent(BeEventType.onAfterFinalDamage, new object[] { vals, target,hurtid });
                var modifyEvemtParam3 = TriggerEventNew(BeEventType.onAfterFinalDamage, new EventParam() { m_Int = modifyDamageValue,m_Obj = this,m_Int2 = hurtid});
                modifyDamageValue = modifyEvemtParam3.m_Int;

                if (projectileOwner != null && !projectileOwner.IsDeadOrRemoved())
                {
                    var modifyEvemtParam4 = projectileOwner.TriggerEventNew(BeEventType.onAfterFinalDamageNew, new EventParam() {m_Int = modifyDamageValue,m_Obj = target,m_Int2 = hurtid,m_Int3 = (int)atkType,m_Obj2 = attachValues,m_Obj3 = this,m_Vint3 = hitPos });
                    modifyDamageValue = modifyEvemtParam4.m_Int;
                    //projectileOwner.TriggerEvent(BeEventType.onAfterFinalDamageNew, new object[] { vals, target, hurtid, atkType, attachValues, this, hitPos});
                }
                else
                {
                    var modifyEvemtParam5 = TriggerEventNew(BeEventType.onAfterFinalDamageNew, new EventParam() { m_Int = modifyDamageValue, m_Obj = target, m_Int2 = hurtid, m_Int3 = (int)atkType, m_Obj2 = attachValues, m_Obj3 = this, m_Vint3 = hitPos });
                    modifyDamageValue = modifyEvemtParam5.m_Int;
                    //TriggerEvent(BeEventType.onAfterFinalDamageNew, new object[] { vals, target, hurtid, atkType, attachValues, this, hitPos});
                }

                bool useEquipElement = hurtData.MagicElementISuse;                              //是否使用装备属性攻击
                List<int> magicElementTypeList = new List<int>();                               //角色拥有的属性攻击类型
                ChangeEquipElementList(useEquipElement, magicElementTypeList, attackElementType);

                //修改属性攻击列表
                GetProjectileEventTrigger().TriggerEventNew(BeEventType.onChangeMagicElementList, new EventParam() { m_Int = hurtid, m_Obj = magicElementTypeList });

                //第二个参数表示攻击属性（比如火属性，用于冰冻buff解除）
                //第四个参数表示攻击伤害类型，例如魔法伤害，物理伤害
                //第七个参数表示当前角色拥有的所有属性
                bool isBeActor = (this as BeActor) != null;

                EventParam eventParam1 = new EventParam(); 
                //被别人攻击后改变最终伤害数值
                bool[] absorbDamage = new bool[1];
                absorbDamage[0] = false;
                if (isBeActor)
                {
                    var modifyEvemtParam5 = target.TriggerEventNew(BeEventType.onBeHitAfterFinalDamage,new EventParam() {m_Int = modifyDamageValue,m_Int2 = hurtid,m_Bool = false,m_Obj = this,m_Bool2 = triggerFlashHurt,m_Vint3 = hitPos,m_Obj2 = magicElementTypeList,m_Obj3 = attachValues });
                    modifyDamageValue = modifyEvemtParam5.m_Int;
                    //target.TriggerEvent(BeEventType.onBeHitAfterFinalDamage, new object[] { vals, hurtid, absorbDamage, this, triggerFlashHurt, hitPos, magicElementTypeList, attachValues });
                }
                else
                {
                    var vOwner = GetOwner();
                    if (vOwner != null && !vOwner.IsDeadOrRemoved())
                    {
                        var modifyEvemtParam6 = target.TriggerEventNew(BeEventType.onBeHitAfterFinalDamage, new EventParam() { m_Int = modifyDamageValue, m_Int2 = hurtid, m_Bool = false, m_Obj = this, m_Bool2 = triggerFlashHurt, m_Vint3 = hitPos, m_Obj2 = magicElementTypeList, m_Obj3 = attachValues });
                        modifyDamageValue = modifyEvemtParam6.m_Int;
                        //target.TriggerEvent(BeEventType.onBeHitAfterFinalDamage, new object[] { vals, hurtid, absorbDamage, vOwner, triggerFlashHurt, hitPos, magicElementTypeList, attachValues });
                    }
                }
                finalDamage = modifyDamageValue;
                // test by ckm
                var mowner = (BeActor)GetOwner();
                if(mowner != null && mowner.isMainActor)
                {
                    // finalDamage = 0;
                }else{
                    // finalDamage = 0;
                }

                if (absorbDamage[0])
                {
                    GamePool.ListPool<int>.Release(attachValues);
                    return AttackResult.MISS;
                }

                if (isBeActor)
                {
                    target.TriggerEventNew(BeEventType.onHit,new EventParam() { m_Int = finalDamage, m_Int2 = (int)attackElementType, m_Obj = this, m_Int3 = (int)atkType, m_Int4 = hurtid, m_Bool = triggerFlashHurt, m_Obj2 = magicElementTypeList, m_Obj3 = hurtData, m_Int5 = (int) attackProcessId});
                    //target.TriggerEvent(BeEventType.onHit, new object[] { finalDamage, attackElementType, this, atkType, hurtid, triggerFlashHurt, magicElementTypeList });
                }
                else
                {
                    var vOwner = GetOwner();
                    if (vOwner != null && !vOwner.IsDeadOrRemoved())
                    {
                        target.TriggerEventNew(BeEventType.onHit,new EventParam() { m_Int = finalDamage ,m_Int2 = (int)attackElementType ,m_Obj = vOwner ,m_Int3 = (int)atkType , m_Int4 = hurtid ,m_Bool = triggerFlashHurt ,m_Obj2 = magicElementTypeList,  m_Obj3 = hurtData, m_Int5 = (int) attackProcessId});
                        //target.TriggerEvent(BeEventType.onHit, new object[] { finalDamage, attackElementType, vOwner, atkType, hurtid, triggerFlashHurt, magicElementTypeList });
                    }
                }

                if (target.stateController.CanBeHitNoDamage())
                {
                    TriggerMiss(owner, target, 2);
                    GamePool.ListPool<int>.Release(attachValues);
                    return AttackResult.MISS;     //如果处于能被攻击 但是不造成伤害状态 直接返回
                }

                finalDamage = FixSummonMonsterDamage(finalDamage);
                outFinalDamage = finalDamage;
                BeActor beActor = this as BeActor;
                int curSkillId = beActor != null ? beActor.GetCurSkillID() : 0;
                //if (beActor != null)
                //{
                //    TriggerEvent(BeEventType.onHitOther, new object[] { target, hurtid, hurtData.SkillID, hitPos , beActor.GetCurSkillID() ,finalDamage});
                //}
                //else
                //{
                //    TriggerEvent(BeEventType.onHitOther, new object[] { target, hurtid, hurtData.SkillID, hitPos,0,finalDamage });
                //}

                TriggerEventNew(BeEventType.onHitOther, new EventParam() { m_Obj = target,m_Int = hurtid,m_Int2 = hurtData.SkillID,m_Vint3 = hitPos,m_Int3 = curSkillId ,m_Int4 = finalDamage });

                HitTextType htType = result == AttackResult.NORMAL ? HitTextType.NORMAL : HitTextType.CRITICAL;
                HitTextType htTypeForSpecialBuff = HitTextType.NORMAL;
                if (hurtData.IsFriendDamage > 0)
                {
                    htTypeForSpecialBuff = HitTextType.FRIEND_HURT;

                    attachValues.Clear();
                }
                //异常buff保护
                BeActor targetActor = target as BeActor;
                if (targetActor != null && targetActor.protectManager != null && triggerFlashHurt)
                    targetActor.protectManager.BeHit();
                OnSkillDamageEvent(finalDamage, targetActor,hurtData.SkillID);

                if (projectileOwner != null && !projectileOwner.IsDeadOrRemoved())
                {
                    projectileOwner.RecordHurtId(hurtid, target);
                }
                else
                {
                    RecordHurtId(hurtid, target);
                }
               
                target.DoHurt(finalDamage, attachValues, htType, this, htTypeForSpecialBuff);

                //TriggerEvent(BeEventType.onHitOtherAfterHurt, new object[] { target, hurtid, result});
                TriggerEventNew(BeEventType.onHitOtherAfterHurt, new EventParam() { m_Obj = target,m_Int = hurtid,m_Int2 = (int)result, m_Int3 = (int) attackProcessId, m_Obj2 = hurtData});

                if ((hurtData.HitDeadFall > 0 && target.IsDead()))
                {
                    DealHit(hurtid, target, hitPos, hurtData, 1, false);
                }

                GamePool.ListPool<int>.Release(attachValues);

                if (IsProcessRecord())
                {
                    GetRecordServer().RecordProcess("[BATTLE]PID:{0}-{1} attack PID:{2}-{3} ret:{4} final:{5} {6} {7}", m_iID, GetName(), target.m_iID, target.GetName(), result, finalDamage, GetInfo(), target.GetInfo());
                    GetRecordServer().Mark(0x88888, new int[]
                                {
                                    m_iID,target.m_iID, (int)result, finalDamage,GetPosition().x,GetPosition().y,GetPosition().z,
                                    moveXSpeed.i,moveYSpeed.i,moveZSpeed.i,GetFace() ? 1 : 0,attribute.GetHP(),attribute.GetMP(),
                                    m_kStateTag.GetAllFlag(),attribute.battleData.attack.ToInt(),target.GetPosition().x,
                                    target.GetPosition().y,target.GetPosition().z,target.moveXSpeed.i,target.moveYSpeed.i,target.moveZSpeed.i,target.GetFace() ? 1 : 0,
                                    target.attribute.GetHP(),target.attribute.GetMP(),target.m_kStateTag.GetAllFlag(),target.attribute.battleData.attack.ToInt(),
                                }, GetName(), target.GetName());
                        // Mark:0x88888 [BATTLE]PID:{0}-{26} attack PID:{1}-{27} ret:{2} final:{3} [name:{26} pos:({4},{5},{6}) speed:({7},{8},{9}) face:{10} hp:{11} mp:{12} flag:{13} attack:{14}][name:{27} pos:({15},{16},{17}) speed:({18},{19},{20}) face:{21} hp:{22} mp:{23} flag:{24} attack:{25}]
                }

            }
            else if (hurtData.HasDamage == -1)
            {
                //TriggerEvent(BeEventType.onCollide, new object[] { target,hurtid });       //自己的攻击框碰撞到别人
                //target.TriggerEvent(BeEventType.onCollideOther, new object[] { this });  //敌人的攻击框碰撞到自己

                TriggerEventNew(BeEventType.onCollide, new EventParam() { m_Obj = target,m_Int = hurtid});       //自己的攻击框碰撞到别人
                target.TriggerEventNew(BeEventType.onCollideOther, new EventParam() { m_Obj = this});  //敌人的攻击框碰撞到自己
            }
            OnCollide();

            //添加buff
            TryAddBuff(hurtData, target);

            if (hurtData.HasDamage == 1)
            {
                bool isBeActor = (this as BeActor) != null;
                BeEntity entity = this;
                if (isBeActor)
                {
                    //target.TriggerEvent(BeEventType.onHitAfterAddBuff, new object[] { hurtid, triggerFlashHurt, this, hurtData.SkillID });
                }
                else
                {
                    var vOwner = GetOwner();
                    entity = vOwner;
                    //target.TriggerEvent(BeEventType.onHitAfterAddBuff, new object[] { hurtid, triggerFlashHurt, vOwner, hurtData.SkillID });
                }
                target.TriggerEventNew(BeEventType.onHitAfterAddBuff,new EventParam() { m_Int = hurtid ,m_Bool = triggerFlashHurt,m_Obj = entity,m_Int2 = hurtData.SkillID , m_Int3 = (int) attackProcessId});
            }

            //添加实体
            TryAddEntity(hurtData, GetPosition(), 1, attackProcessId);

            TrySummon(hurtData);


        }
        else if (AttackResult.MISS == result)
        {
            TriggerMiss(owner, target,3);
            // 添加MISS冒字
            if (battleUI != null)
            {
                if (showDamageNumber)
                    battleUI.comShowHit.ShowHitNumber(0, null, m_iID, hitPos.vec3, GetFace(), GameClient.HitTextType.MISS, owner, target);

            }
        }

        if (battleUI != null)
            battleUI.comDebugBattleStatis.BS_AddBattleInfo(this.GetName(), target.GetName(), result, finalDamage);

        return result;
    #if ENABLE_PROFILER
        }
    #endif
    }
    //递归保护最多允许5层
    private int mCurRecursiveCount = 0;
    public BeEntity GetTopOwner(BeEntity parent)
    {
        if (parent == null) return null;
        mCurRecursiveCount = 0;
        return _GetTopOwner(parent.GetOwner());
    }

    //记录造成伤害的触发效果ID
    protected void RecordHurtId(int hurtId,BeEntity target)
    {
#if UNITY_EDITOR && !LOGIC_SERVER
        var actor = this as BeActor;
        if (actor == null)
            return;
        if (target == null)
            return;
        string log = string.Format("触发效果ID:{0},目标Pid:{1},目标名称:{2},时间:{3}", hurtId, target.GetPID(),target.GetName(), Time.time);
        if (m_RecordHurtIdList.Count >= 40)
            m_RecordHurtIdList.RemoveAt(0);
        m_RecordHurtIdList.Add(log);
#endif
    }

    private BeEntity _GetTopOwner(BeEntity parent)
    {
        if (parent == null /*|| parent.IsRemoved()*/) return null;
        if (mCurRecursiveCount >= 5)
        {
            Logger.LogErrorFormat("recursive count is out of range curparent {0} {1}", parent != null ? parent.GetName() : "null", GetName());
            return parent;
        }
        mCurRecursiveCount++;

        if (parent == parent.GetOwner()) return parent;
        var parentOwner = _GetTopOwner(parent.GetOwner());
        if (parentOwner == null) return parent;
        return parentOwner;
    }

    //监听技能造成的伤害
    private void OnSkillDamageEvent(int hurtValue,BeActor target,int skillId)
    {
        if (owner.CurrentBeScene == null)
            return;
        if (target == null)
            return;
        BeActor actor = this as BeActor;
        if (actor == null)
            actor = owner as BeActor;
        if (actor == null)
            return;
        BeSkill skill = actor.GetSkill(skillId);
        if (skill == null)
            return;
        int sourceId = BeUtility.GetComboSkillId(actor, skillId);
        owner.CurrentBeScene.TriggerEventNew(BeEventSceneType.onDoHurt, new EventParam() { m_Int = hurtValue, m_Int2 = sourceId, m_Obj = actor, m_Obj2 = target });

    }

    //召唤兽伤害修正
    protected int FixSummonMonsterDamage(int damage)
    {
        if (attribute.monsterData != null &&
            attribute.monsterData.MonsterMode == (int)MonsterMode.SUMMON_PVE &&
            BattleMain.IsModePvP(CurrentBeScene.mBattle.GetBattleType()))
        {
            var data = TableManager.GetInstance().GetTableItem<ProtoTable.PkHPProfessionAdjustTable>(attribute.simpleMonsterID);
            if (data != null)
            {
                if (CurrentBeScene.mBattle.PkRaceType == (int)Protocol.RaceType.ScoreWar)
                {
                    damage *= VRate.Factor(data.DamageFactor_3v3);
                }
                else if (CurrentBeScene.mBattle.PkRaceType == (int)Protocol.RaceType.ChiJi)
                {
                    damage *= VRate.Factor(data.DamageFactor_chiji);
                }
                else
                {
                    damage *= VRate.Factor(data.DamageFactor);
                }
            }
        }
        return damage;
    }

    //miss事件监听
    protected void TriggerMiss(BeEntity attacker, BeEntity target,int missFlag)
    {
        BeActor ownerActor = (BeActor)attacker;
        BeActor targetActor = (BeActor)target;
        if (ownerActor != null)
        {
            ownerActor.buffController.TriggerBuffs(BuffCondition.ATTACKMISS, target as BeActor);   //攻击时miss
        }

        if (targetActor != null)
        {
            targetActor.buffController.TriggerBuffs(BuffCondition.BEHITMISS, target as BeActor);   //被击时miss       
        }

#if MG_TEST
        if (IsProcessRecord())
        {
            GetRecordServer().RecordProcess("[BATTLE]PID:{0}-{1} TriggerMiss PID:{2}-{3} ret:{4} final:{5} {6}", m_iID, GetName(), target.m_iID, target.GetName(), GetInfo(), target.GetInfo(), missFlag);
        }
#endif

    }

    //改变装备属性攻击列表
    protected void ChangeEquipElementList(bool useEquipElement, List<int> magicElementTypeList, MagicElementType attackElementType)
    {
        if (useEquipElement)
        {
            attribute.GetOwnerEquipElement(magicElementTypeList);
        }
        if (!magicElementTypeList.Contains((int)attackElementType))
        {
            magicElementTypeList.Add((int)attackElementType);
        }
    }

    // public void DoReflect(int reflectDamage, BeEntity target)
    // {
    //     if (reflectDamage > 0 && target != null && target != this)
    //     {
    //         target.DoHurt(reflectDamage);
    //     }
    // }

    //帧事件处理,肯定是对自身使用
    public void DealEffectFrame(BeEntity target, int hurtid, int duration = 0, bool phaseDelete = false, bool useBuffAni = true, bool finishDelete = false, bool finishDeleteAll = false, uint attackProcessId = 0)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeEntity.DealEffectFrame"))
        {
#endif
        var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hurtid);

        if (hurtData == null)
        {
            return;
        }

        TryAddBuff(hurtData, this, duration, useBuffAni, finishDelete, finishDeleteAll);
        TryAddEntity(hurtData, GetPosition(), 1, attackProcessId);
        TrySummon(hurtData);
#if ENABLE_PROFILER
        }
#endif
    }

    public virtual bool TryAddBuff(ProtoTable.EffectTable hurtData, BeEntity target = null, int duration = 0, bool useBuffAni = true, bool finishDelete = false, bool finishDeleteAll = false)
    {
        Logger.LogWarning("Children must implement TryAddBuff.");
        return false;
    }

    public virtual bool TryAddEntity(ProtoTable.EffectTable hurtData, VInt3 pos, int triggeredLevel = 1, uint attackProcessId = 0)
    {
        Logger.LogWarning("Children must implement TryAddEntity.");
        return false;
    }

    public virtual bool TrySummon(ProtoTable.EffectTable hurtData)
    {
        Logger.LogWarning("Children must implement TrySummon.");
        return false;
    }

    //用于执行一次性的机制
    public virtual void TryAddMechanism(int mechanismId)
    {
        
    }

    public BeEntityData GetEntityData()
    {
        return attribute;
    }

    /// <summary>
    /// 判断是否是召唤师召唤出来的宝宝
    /// </summary>
    /// <returns></returns>
    public bool IsSummonMonster()
    {
        return attribute != null && attribute.monsterData != null && attribute.monsterData.MonsterMode == 5;
    }


    public VInt GetEnityScale()
    {
        return m_fScale;
    }
    public int _getFaceCoff()
    {
        return GetFace() ? -1 : 1;
    }

    public void AddShock(ShockData sd)
    {
        _addShock(sd.time, sd.speed, sd.xrange, sd.yrange, sd.mode);
    }

    public void _addShock(float time, float speed, float xrange, float yrange, int mode = 1)
    {
#if !LOGIC_SERVER
        m_kShockEffect.SetMode(mode);
        m_kShockEffect.Start(time, speed, xrange * _getFaceCoff(), yrange);
#endif
    }

    public virtual void JudgeDead()
    {
        bool preValue = isDead;
        if (attribute != null && attribute.GetHP() <= 0)
        {
            isDead = true;
            if (!preValue && isDead)
            {
                TriggerEventNew(BeEventType.onBeKilled,new EventParam());
                //TriggerEvent(BeEventType.onBeKilled);
                
            }
        }
    }

    public bool IsDead()
    {
        return isDead;
        //return attribute!=null && attribute.battleData.hp <= 0;
    }

    public virtual void DoDead(bool isForce = false)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeEntity.DoDead"))
        {
#endif
            if (sgGetCurrentState() == (int)ActionState.AS_DEAD)
                return;

            //TriggerEvent(BeEventType.onDead, new object[] { vals , isForce,this });
            bool continueDead = true;
            var eventParam = TriggerEventNew(BeEventType.onDead, new EventParam() { m_Bool = continueDead, m_Bool2 = isForce, m_Obj = this });
            continueDead = eventParam.m_Bool;

            if (currentBeScene != null)
            {
                currentBeScene.TriggerEventNew(BeEventSceneType.onEntityDead, new EventParam() {m_Obj = this});
            }
            if (continueDead)
            {
                if (aiManager != null)
                {
                    aiManager.Stop();
                }
                if (!sgStarted)
                {
                    if(m_pkStateGraph != null)
                    {
                        if(!m_pkStateGraph.Start((int)ActionState.AS_DEAD))
                        {
                            Locomote(new BeStateData((int)ActionState.AS_DEAD));
                            return;
                        }
                    }
                }
                else
                {
                    Locomote(new BeStateData((int)ActionState.AS_DEAD));
                }
                sgStarted = true;
            }

#if ENABLE_PROFILER
        }
#endif
    }

    public virtual void OnDead()
    {

    }

    public void DoMPChange(int value, bool showNumber = false)
    {

#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeEntity.DoMPChange"))
        {
#endif
        if (attribute != null)
        {
            attribute.OnMPChange(value);

            TriggerEventNew(BeEventType.onMPChange,new EventParam());
            //TriggerEvent(BeEventType.onMPChange);

            //attribute.battleData.mp / (float)attribute.battleData.maxMp
            if (m_pkGeActor != null)
                m_pkGeActor.SetMpValue(attribute.GetMPRate().single);

            if (showNumber)
            {
                var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUICommon>();
                if (battleUI != null && battleUI.comShowHit != null)
                {
                    if (showDamageNumber)
                        battleUI.comShowHit.ShowHitNumber(value, null, m_iID, GetGePosition(PositionType.OVERHEAD), GetFace(), HitTextType.MP_RECOVER);
                }
            }
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public void DoHPChange(int value, bool showNumber = true)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeEntity.DoHPChange"))
        {
#endif
        if (attribute != null)
        {
            int oldHP = attribute.GetHP();
            attribute.OnHPChange(value);

            float geHpRatio = attribute.GetHPRate().single; //attribute.battleData.hp /(float) attribute.battleData.maxHp;
            geHpRatio = Mathf.Clamp(geHpRatio, 0, 1);

            int hpChanged = attribute.GetHP() - oldHP;

            HitTextType type = HitTextType.RECOVE;
            if (value <= 0)
                type = HitTextType.BUFF_HURT;


            if (m_pkGeActor != null)
            {
                //更新血条
                m_pkGeActor.SetHPValue(geHpRatio);

                if (hpChanged > 0 && !stateController.CanHurt())
                    ;
                else
                {
                    //69是隐藏UI的buff，这时候不应该显示血条
                    var actor = this as BeActor;
                    if (actor != null && actor.buffController.HasBuffByID(69) != null)
                        return;

                    m_pkGeActor.SetHPDamage(-hpChanged, type);
                }
            }

            if (showNumber)
            {
                var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUICommon>();
                if (battleUI != null)
                {
                    if (showDamageNumber)
                        battleUI.comShowHit.ShowHitNumber(value, null, m_iID, GetGePosition(PositionType.OVERHEAD), GetFace(), type, null, this);
                }
            }
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public void DoHeal(int healValue, bool showNumber = true)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeEntity.DoHeal"))
        {
#endif
        if (healValue > 0)
        {
            DoHPChange(healValue, showNumber);
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public virtual void DoHurt(int value, List<int> attachValues = null, HitTextType type = HitTextType.NORMAL, BeEntity attacker = null, HitTextType typeForSpecialBuff = HitTextType.NORMAL, bool forceBuffHurt = false, HitDamageType damageType = HitDamageType.Normal)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeEntity.DoHurt"))
        {
#endif
        if (attribute != null)
        {
            int previousHp = attribute.GetHP();

            int hurtValue = value;

            //监听此事件可以调整最终伤害
            //int[] hurtDamageArr = new int[1];
            //hurtDamageArr[0] = hurtValue;
            //TriggerEvent(BeEventType.onChangeHurtValue, new object[] { hurtDamageArr });
            //hurtValue = hurtDamageArr[0];

            var eventParam = TriggerEventNew(BeEventType.onChangeHurtValue,new EventParam() { m_Int = hurtValue, m_Int2 = (int) damageType});
            hurtValue = eventParam.m_Int;

            //Logger.LogErrorFormat("do hurt {0}", value);
            attribute.OnHPChange(-hurtValue);
            if (IsBoss())
            {
                if (attacker != null)
                {
                    int pid = attacker.GetPID();
                    BeActor actorAttacker = attacker as BeActor;
                    if (actorAttacker == null || !actorAttacker.isMainActor)
                    {
                        BeActor attackerParent = GetTopOwner(attacker) as BeActor;
                        if (attackerParent != null && attackerParent.isMainActor)
                        {
                            pid = attackerParent.GetPID();
                        }
                    }
                    IDungeonStatistics statistics = CurrentBeBattle.dungeonStatistics;
                    if (statistics != null)
                    {
                        statistics.AddBossHurtData(pid, hurtValue, GetEntityData().monsterID);
                    }
                }
                else
                {
                    Logger.LogWarningFormat("attacker is null PID {0} Value {1}", GetPID(), value);
                }
            }
            if (attribute.GetHP() <= 0 && previousHp > 0 && attacker != null)
            {
                VFactor hpFactor = new VFactor(Global.Settings.immediateDeadHPPercent, GlobalLogic.VALUE_100);
                if (hurtValue >= attribute.GetMaxHP() * hpFactor)
                {
                    needDead = true;
                    if(!PlayDeadAction())
                    deadType = DeadType.IMMEDIATE;
                }
                else if (type == HitTextType.CRITICAL)
                {
                    BeActor thisActor = this as BeActor;
                    if (thisActor == null || (thisActor != null && thisActor.IsCanCritDead()))
                    {
                        needDead = true;
                        if (!PlayDeadAction())
                        deadType = DeadType.CRITICAL;
                    }
                }


                //Logger.LogErrorFormat("死亡, posz:{0}", GetPosition().z);
                if (deadType == DeadType.NORMAL)
                {
                    //m_pkGeActor.ChangeSurface("死亡2", 0f);
                    //Logger.LogErrorFormat("空中死亡！！！！ posz:{0}", GetPosition().z);
                }

                var vOwner = attacker.GetOwner();
                if (vOwner != null && !vOwner.IsDeadOrRemoved())
                {
                    vOwner.TriggerEventNew(BeEventType.onKill, new EventParam() { m_Obj = this});
                    //vOwner.TriggerEvent(BeEventType.onKill, new object[] { this });
                }

                var selfActor = attacker;
                if (selfActor is BeProjectile)
                {
                    selfActor = attacker.GetOwner();
                }
                selfActor.TriggerEventNew(BeEventType.onSelfKill, new EventParam() { m_Obj = this});
                
                
                if (currentBeScene != null)
                {
                    currentBeScene.TriggerEventNew(BeEventSceneType.onKill, new EventParam() {m_Obj = this, m_Obj2 = attacker});
                }
            }

            TriggerEventNew(BeEventType.onHurt, new EventParam() { m_Int = hurtValue, m_Obj = this ,m_Obj2 = attacker});
            //TriggerEvent(BeEventType.onHurt, new object[] { hurtValue });
            

            if (attacker != null)
                mLastAttackerId = attacker.GetPID();
            OnHurt(hurtValue, attacker, typeForSpecialBuff);

            float geHpRatio = attribute.GetHPRate().single;//attribute.battleData.hp /(float) attribute.battleData.maxHp;
            geHpRatio = Mathf.Clamp(geHpRatio, 0, 1);

            if(m_pkGeActor != null)
                m_pkGeActor.SetHPValue(geHpRatio);
            //更新血条
            if (hurtValue > 0 && !stateController.CanHurt())
                ;
            else
            {
                if (this is BeProjectile)
                {
                    if (owner != null && owner.m_pkGeActor != null)
                        owner.m_pkGeActor.SetHPDamage(hurtValue, type);
                }
                else
                {
                    if(m_pkGeActor != null)
                        m_pkGeActor.SetHPDamage(hurtValue, type);
                }
            }
            if (attacker != null)
            {
                BeEntity topOwner = attacker.GetTopOwner(attacker);
                BeActor topActor = topOwner as BeActor;
                if (topActor != null && topActor.isMainActor)
                {
                    topOwner.GetEntityData().battleData.AddDamage(value, attacker, topOwner);
                }
            }


            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUICommon>();
            if (battleUI != null)
            {
                int normalValue = hurtValue;
                if (attachValues != null)
                {
                    for (int i = 0; i < attachValues.Count; ++i)
                    {
                        normalValue -= attachValues[i];
                    }
                }

                //!! 这里只是使用Owner的数值，不用做判断
                if (showDamageNumber)
                {
                    if (attacker != null)
                    {
                        if ((attacker as BeProjectile) != null ||
                            (attacker as BeActor) != null && attacker.GetEntityData() != null &&
                            attacker.GetEntityData().type == (int)ProtoTable.UnitTable.eType.SKILL_MONSTER ||
                            (attacker.GetOwner() != null && attacker.GetEntityData() == attacker.GetOwner().GetEntityData())
                        )
                            attacker = attacker.GetOwner();
                    }

                    if (typeForSpecialBuff != HitTextType.NORMAL)
                        type = typeForSpecialBuff;
                    battleUI.comShowHit.ShowHitNumber(normalValue, attachValues, m_iID, GetGePosition(PositionType.OVERHEAD), GetFace(), type, attacker, this);
                }

            }
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public virtual bool PlayDeadAction()
    {
        return false;
    }

    public virtual void OnHurt(int value, BeEntity attacker, HitTextType curHitTextType)
    {

    }

    public List<BeEntity> GetHurtEntityList()
    {
        return m_vHurtEntity;
    }

    #region BeEventHandleNew
    /// <summary>
    /// 注册事件
    /// </summary>
    /// <param name="type">事件类型</param>
    /// <param name="function">事件回调函数</param>
    public BeEvent.BeEventHandleNew RegisterEventNew(BeEventType eventType, BeEvent.BeEventHandleNew.Function function)
    {
        if (m_EventManager == null)
        {
            m_EventManager = new BeEventManager(GetPID());
        }
        return m_EventManager.RegisterEvent((int)eventType, function);
    }
    
    /// <summary>
    /// 触发事件(New)
    /// 使触发事件调用那边的代码更简洁 更容易使用
    /// </summary>
    public EventParam TriggerEventNew(BeEventType eventType, EventParam param = new EventParam())
    {
        if (m_EventManager == null)
            return param;
        return m_EventManager.TriggerEvent((int)eventType, param);
    }

    /// <summary>
    /// 清除所有事件
    /// </summary>
    public void ClearEventAllNew()
    {
        if (m_EventManager == null)
            return;
        m_EventManager.ClearAll();
    }
#endregion


    /**********************************************************************/
    public bool IsCastingSkill()
    {
        return sgGetCurrentState() == (int)ActionState.AS_CASTSKILL;
    }

    public bool IsGrabed()
    {
        return sgGetCurrentState() == (int)ActionState.AS_GRABBED;
    }


    public void RecordGrabPosition()
    {
        if (currentBeScene != null && !currentBeScene.IsInBlockPlayer(GetPosition()))
        {
            posBeforeGrab = GetPosition();
        }
    }

    public void JugePositionAfterGrab()
    {
        var pos = GetPosition();
        //pos.z = 0;
        //如果在阻挡里就社会原来的x,y 
        if (currentBeScene != null && currentBeScene.IsInBlockPlayer(pos))
        {
            if (m_kBlockPosition == VInt3.zero)
            {
                m_kBlockPosition = posBeforeGrab;
            }
            SetStandPosition(new VInt3(m_kBlockPosition.x, m_kBlockPosition.y, pos.z));
        }
    }

    public string GetName()
    {
        //#if !LOGIC_SERVER
        if (m_pkGeActor != null)
            return m_pkGeActor.m_ActorDesc.name;
        //#endif
        return "";
    }

    public void SetFloating(VInt height, bool footIndicator = true)
    {
        isFloating = true;
        floatingHeight = height;
        if (footIndicator)
            m_pkGeActor.CreateFootIndicator();

        RestoreFloating();
    }

    public void RestoreFloating(bool force = false)
    {
        if (force || !stateController.IgnoreGravity())
        {
            stateController.SetAbilityEnable(BeAbilityType.GRAVITY, false);
            var tmpPos = GetPosition();
            tmpPos.z = 0;
            tmpPos.z += floatingHeight.i;
            SetPosition(tmpPos);
        }
    }

    public void RemoveFloating()
    {
        if (stateController.IgnoreGravity())
            stateController.SetAbilityEnable(BeAbilityType.GRAVITY, true);
    }


    // public bool IsNormalInAir()
    // {
    //     if (!IsInPassiveState())
    //     {
    //         return GetPosition().z > VInt.zeroDotOne;
    //     }

    //     return false;
    // }

    public bool IsInPassiveState()
    {
        return GetStateGraph().CurrentStateHasTag((int)AStateTag.AST_BUSY | (int)AStateTag.AST_CONTROLED | (int)AStateTag.AST_LOCKZ);
    }

    public void UpdateRecover(int delta)
    {
        recoverTimeAcc += delta;
        if (recoverTimeAcc >= recoverInterval)
        {
            recoverTimeAcc -= recoverInterval;

            if (!IsDead())
            {
                DoRecover();
                DoReduce();
            }
        }
    }
    static readonly VFactor Recover_Tmp2 = new VFactor(1, GlobalLogic.VALUE_60);
    public void DoRecover()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeEntity.DoRecover"))
        {
#endif
        if (attribute == null)
        {
            return;
        }

        if ((attribute.battleData.hpRecover > 0 && attribute.GetHP() < attribute.battleData.maxHp) ||
            (attribute.battleData.hpRecover < 0 && attribute.GetHP() > 0))
        {
            VFactor tmp = new VFactor(recoverInterval, GlobalLogic.VALUE_1000);
            int value = attribute.battleData.hpRecover * (Recover_Tmp2 * tmp);
            //value = Mathf.Ceil(value);


            if (attribute.battleData.hpRecover > 0)
            {
                value = Mathf.Max(1, value);
                if (value != 0)
                {
                    //DoHeal(value, false);
                    DoHPChange(value, false);
                }
            }
            else if (attribute.battleData.hpRecover < 0)
            {
                if (value != 0 && Math.Abs(value) < attribute.GetHP())
                    DoHPChange(value, false);
            }

            //Logger.LogErrorFormat("recover hp:{0}", value);          
        }

        if (attribute.battleData.mpRecover != 0 && attribute.GetMP() < attribute.GetMaxMP())
        {
            VFactor tmp = new VFactor(recoverInterval, GlobalLogic.VALUE_1000);

            int value = attribute.battleData.mpRecover * (Recover_Tmp2 * tmp);
            //float value = attribute.battleData.mpRecover / (float)(GlobalLogic.VALUE_60) * tmp;
            //value = Mathf.Ceil(value);
            // value = Mathf.Max(1, value);

            //Logger.LogErrorFormat("recover mp:{0}", value);

            if (value != 0)
            {
                DoMPChange(value, false);
            }
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public void DoReduce()
    {
        if (attribute == null)
            return;

        if (attribute.battleData.hpReduce > 0 && attribute.GetHP() > attribute.battleData.hpReduce)
        {
            DoHPChange(-attribute.battleData.hpReduce, false);
        }

        if (attribute.battleData.mpReduce > 0 && attribute.GetMP() > attribute.battleData.mpReduce)
        {
            DoMPChange(-attribute.battleData.mpReduce, false);
        }
    }

    public void _LoadBlockData()
    {
        string modelDataRes = FBModelDataSerializer.GetBlockDataResPath(m_iResID);
        DModelData modelData = null;
        if (!string.IsNullOrEmpty(modelDataRes))
        {
#if USE_FB
            FBModelDataSerializer.LoadFBModelData(Path.Combine(Utility.kRawDataPrefix, Path.ChangeExtension(modelDataRes, Utility.kRawDataExtension)), out modelData);
#else
            modelData = AssetLoader.instance.LoadRes(modelDataRes, typeof(DModelData)).obj as DModelData;
#endif
        }

        if (null != modelData)
        {
            m_nBlockWidth = modelData.blockGridChunk.gridWidth;
            m_nBlockHeight = modelData.blockGridChunk.gridHeight;
            m_byteBlockData = new byte[modelData.blockGridChunk.gridBlockData.Length];
            Array.Copy(modelData.blockGridChunk.gridBlockData,m_byteBlockData, modelData.blockGridChunk.gridBlockData.Length);
        }
        else
        {
            m_nBlockWidth = 1;
            m_nBlockHeight = 1;
            m_byteBlockData = DEFAULT_BLOCK_DATA;
        }
    }
    public byte[] _GetBlockData(out int width, out int height)
    {
        width = m_nBlockWidth;
        height = m_nBlockHeight;
        return m_byteBlockData;
    }

    public void SetBlockLayer(bool block = true)
    {
        if (currentBeScene == null)
            return;

        int width;
        int height;

        DGrid blockRect = new DGrid(5, 10);

        byte[] blocks = _GetBlockData(out width, out height);

        bool hasBlock = true;
        if (width == 1 && height == 1)
        {
            hasBlock = false;
        }
        else
        {
            blockRect.x = width;
            blockRect.y = height;
        }

        if (!hasBlock)
            return;

        DGrid grid = currentBeScene.CalGridByPosition(GetPosition());
        int logicX = grid.x;
        int logicZ = grid.y;

        int startX = logicX - blockRect.x / 2;
        int startZ = logicZ - blockRect.y / 2 + 1;

        if ((blockRect.x % 2) == 1)
            startX++;

        if ((blockRect.y % 2) == 1)
            startZ++;

        for (int i = startX; i < startX + blockRect.x; ++i)
        {
            for (int j = startZ; j < startZ + blockRect.y; ++j)
            {

                if (hasBlock)
                {
                    int index = (i - startX) + (j - startZ) * width;
                    bool value = blocks[index] == 1;

                    if (value)
                    {
                        if (block)
                            currentBeScene.SetBlock(new DGrid(i, j), true);
                        else
                            currentBeScene.SetBlock(new DGrid(i, j), false);
                    }
                }
                else
                {
                    currentBeScene.SetBlock(new DGrid(i, j), block);
                }
            }
        }

    }

    public void SetDefaultHitSFX(int sfxID)
    {
        defaultHitSFXID = sfxID;
        if (defaultHitSFXID > 0)
        {
            defaultHitSFXData = TableManager.GetInstance().GetTableItem<ProtoTable.SoundTable>(defaultHitSFXID);
            AudioManager.instance.PreloadSound(defaultHitSFXID);
        }
    }

    public virtual string GetInfo()
    {
        var pos = GetPosition();
        string info = string.Format(" [PID:{10} name:{9} pos:({0},{1},{2}) speed:({3},{4},{5}) face:{6} hp:{7} mp:{8} attack:{12} tag:{11}]",
             pos.x, pos.y, pos.z, moveXSpeed, moveYSpeed, moveZSpeed, GetFace(), attribute.GetHP(), attribute.GetMP(), GetName(), m_iID, m_kStateTag.GetAllFlag(),attribute.battleData.attack);

        return info;
    }
    public virtual int[] GetEntityRecordAttribute()
    {
        var pos = GetPosition();
        return new int[] { m_iID, pos.x, pos.y, pos.z, moveXSpeed.i, moveYSpeed.i, moveZSpeed.i, GetFace() ? 1 : 0, null != attribute ? attribute.GetHP() : 0, null != attribute ? attribute.GetMP() : 0, m_kStateTag.GetAllFlag(), GetCurSkillID() };
    }

    //移除重复伤害
    public void RemoveRepeatDamage()
    {
        if (delayCallUnitList.Count > 0)
        {
            for (int i = 0; i < delayCallUnitList.Count; i++)
            {
                delayCallUnitList[i].SetRemove(true);
            }
        }

        delayCallUnitHurtIdList.Clear();
        delayCallUnitList.Clear();
    }

    public bool IsProcessRecord()
    {
        var recorder = GetRecordServer();

        if (recorder != null)
            return recorder.IsProcessRecord();

        return false;
    }

    public RecordServer GetRecordServer()
    {
        if (currentBeScene == null)
            return null;

        return currentBeScene.mBattle.recordServer;
    }

    //获取技能的当前帧
    public int GetCurrentFrame()
    {
        return m_iCurrentLogicFrame;
    }

    //检测自己能够攻击队友 投射物则检测自己的召唤者能否攻击队友
    private bool CheckCanAttackFriend(BeEntity pkEntity)
    {
        if (pkEntity == null)
            return false;
        if (this is BeProjectile)
           return owner != pkEntity && owner.stateController.CanAttackFriend();
        else
            return stateController.CanAttackFriend();
    }

    private bool CheckCanAttackFriendAndEnemy(BeEntity pkEntity)
    {
        if (pkEntity == null) return false;
        return owner.stateController.CanAttackFriendAndEnemy()||pkEntity.stateController.CanAttackFriendAndEnemy();
    }

    /// <summary>
    /// X轴位置是否刷新
    /// </summary>
    private bool CanUpdateX()
    {
        return stateController.CanUpdateX();
    }

    /// <summary>
    /// Y轴位置是否刷新
    /// </summary>
    private bool CanUpdateY()
    {
        return stateController.CanUpdateY();
    }

    /// <summary>
    /// Z轴位置是否刷新
    /// </summary>
    private bool CanUpdateZ()
    {
        return stateController.CanUpdateZ();
    }
    
    /// <summary>
    /// 获取事件的触发者 如果自己是实体则用自己的召唤者作为触发者
    /// </summary>
    /// <returns></returns>
    protected BeEntity GetProjectileEventTrigger()
    {
        var projetile = this as BeProjectile;
        if (projetile == null)
        {
            return this;
        }
        else
        {
            return GetOwner();
        }
    }

    protected virtual void OnCollide()
    {

    }

    protected virtual void OnXInBlock()
    {

    }

    protected virtual EventParam ChangeXForce(EventParam xForce, BeEntity target, BeEntity bullet, bool face)
    {
        return xForce;
    }

    protected virtual void OnAttachHurt(BeEntity pkEntity, ProtoTable.EffectTable hurtData)
    {

    }

    /// <summary>
    /// 删除阶段需要删除的音效
    /// </summary>
    public void ClearPhaseDeleteAudio()
    {
#if !LOGIC_SERVER
        if (m_PhaseDeleteAudioList == null)
            return;
        for(int i=0;i< m_PhaseDeleteAudioList.Count; i++)
        {
           AudioManager.instance.Stop(m_PhaseDeleteAudioList[i]);
        }
        m_PhaseDeleteAudioList.Clear();
#endif
    }

    /// <summary>
    /// 删除技能结束需要删除的音效列表
    /// </summary>
    public void ClearSkillFinishDeleteAudio()
    {
#if !LOGIC_SERVER
        if (m_FinishDeleteAudioList == null)
            return;
        for (int i = 0; i < m_FinishDeleteAudioList.Count; i++)
        {
            AudioManager.instance.Stop(m_FinishDeleteAudioList[i]);
        }
        m_FinishDeleteAudioList.Clear();
#endif
    }
    
    /// <summary>
    /// 是否属于同一个召唤主
    /// </summary>
    public bool IsSameTopOwner(BeEntity entity)
    {
        var ownerTopOwner = GetTopOwner(this);
        if (ownerTopOwner == null)
            return false;
        if (entity == null)
            return false;
        var entityTopOwner = entity.GetTopOwner(entity);
        if (entityTopOwner == null)
            return false;
        return entityTopOwner == ownerTopOwner;
    }

    public void ClearEvent()
    {
        ClearEventAllNew();
    }
    #region DELAY_CALL
    DelayCaller.Del mChangeBeHitSurface;
    public void AddChangeBeHitSurfaceDelayCall(int duration)
    {
        if (!SwitchFunctionUtility.IsHitShaderOpen)
        {
            return;
        }

        if(mChangeBeHitSurface == null)
        {
            mChangeBeHitSurface = new DelayCaller.Del(ChangeBeHitSurface);
        }
        if (delayCaller != null)
        {
            delayCaller.DelayCall(duration, mChangeBeHitSurface);
        }
    }

    private void ChangeBeHitSurface()
    {
        if (m_pkGeActor != null && !IsDead())
            m_pkGeActor.ChangeSurface("受击", Global.Settings.hitTime);
    }
    DelayCaller.Del mReduceEffectNumDel;
    public void AddDelayCallDealEffectNum(int duration)
    {
        if(mReduceEffectNumDel == null)
        {
            mReduceEffectNumDel = new DelayCaller.Del(ReduceEffectNum);
        }
        if(delayCaller != null)
        {
            delayCaller.DelayCall(duration, mReduceEffectNumDel);
        }
    }

    private void ReduceEffectNum()
    {
        currentHitEffectNum--;
    }

    DelayCaller.Del mReduceSFXNumDel;

    public void AddDelayCallDealSFXNum(int duration)   
    {
        if(mReduceSFXNumDel == null)
        {
            mReduceSFXNumDel = new DelayCaller.Del(ReduceSFXNum);
        }
        if (delayCaller != null)
        {
            delayCaller.DelayCall(duration, mReduceSFXNumDel);
        }
    }

    private void ReduceSFXNum()
    {
        currentHitSFXNum--;
    }
    #endregion
}
