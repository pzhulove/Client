using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PathFinder;
using ProtoTable;
using GameClient;

[LoggerModel("AI")]
public partial class BeAIManager
{
    public static string LAST_RESULT = "LastResult";

    public static bool logerror = false;

    public enum AutoFightMode
    {
        NONE,
        HALF,
        ALL
    }

    public enum State
    {
        NONE,
        READY,
        RUNNING_Normal,
        RUNNING_Scenario,
        STOP,
    }

    public enum AIType
    {
        //总类型
        NOATTACK = -1,   //没有攻击
        MELEE = 0,       //肉搏
        RANGED,          //远程


        //特定类型(用于AI行为树中根据类型做通用处理，如行走方式)
        //近战类型
        TANK = 10, //肉坦
        ASSASSIN = 11,//刺客
        MIGRATION = 12, //游走

        //远战类型
        RANGEDNorMal = 20, //普通远程
    }

    public enum TargetType
    {
        NEAREST = 0,
        LOW_LEVEL,
        HIGH_LEVEL,
        LOW_HP,
        HIGH_HP,
        ATTACKER,
        DOWNED,
        BOSS,
        Max_Resentment,
    }
    public enum TargetEntityType
    {
        DEFAULT = 0,
        PLAYER,
        MONSTER,
        SUMMON,
    }
    public enum AIMode
    {
        NONE = 0,
        ALERT,      //警戒
        ATTACK,     //攻击
        FOLLOW,     //跟随
    }

    public enum AIState
    {
        NONE = 0,
        ATTACK,
        WALK,
        FOLLOW
    }

    public enum DestinationType
    {
        IDLE = -1,
        GO_TO_TARGET = 0,   //追踪，寻路
        ESCAPE,             //逃跑
        BYPASS_TRACK,       //迂回追踪
        Y_FIRST,            //Y轴优先
        FOLLOW,             //跟随
        WANDER,             //游荡
        KEEP_DISTANCE,      //保持距离
        FINAL_DOOR,			//走向门
        WANDER_IN_CIRCLE, //在固定范围圆内
        WANDER_PKROBOT, //决斗场机器人徘徊
		MOVETO_LEFT_SCENEEDGE,//走向场景左边界
		GO_TO_TARGET2,  //直接走向目标点的位置（与GO_TO_TARGET也有所区别）
        GO_TO_TARGET_KEEP_DISTANCE,    // 直接走到目标点，互相保持距离
        WANDER_IN_OWNER_CIRCLE,
        WANDER_BY_OWNER,    //漫游（自身为中心）
        WANDER_BY_TARGET,   //漫游（目标为中心）
        GO_TO_TARGET_DIRECTLY,  //最短路径走向目标（和追踪类似，但是靠近目标时也会选点，选的点不会在对角线）
        RUNAWAY,            //逃跑（run）
        W_MOVE,             //W字移动
        Z_MOVE,             //Z字移动
        KEEP_DISTANCE_TABLE,    //保持距离读表
        AVOID_FRONT_FACE,       //避免正面
        CHASE_ROUNDABOUT,       //围绕着追
        CHASE_BACK_DIRECTLY,    //直接追逐后背
        GO_TO_TARGET3  // 最单纯的直接走向目标点
    }

    public enum TreeType
    {
        ACTION = 0,
        DESTINATION_SELECT
    }

    public enum RUNAWAY_STATE
    {
        NORMAL,
        LEFT,
        RIGHT,
        TOP,
        BOTTOM,
    }

    public enum IdleMode
    {
        IDLE = 0,
        WANDER,
        CUSTOM
    }

    public enum AIChoise
    {
        NONE = 0,
        ATTACK,
        CANNOT_ATTACK = -1,
        CANATTACK_BUTCHOOSENOT = -2,
    }

    public AIInputData aiInputData = null;
    public int actionResult = -1;
    public int destinationSelectResult = -1;
    public int monsterID = 0;
    public int radius = 0;
    protected BeAICommand currentCommand;
    protected BeAICommand lastCommand;

    public BeAICommand currentCmd
    {
        get { return currentCommand; }
    }

    public FrameRandomImp FrameRandom
    {
        get
        {
            return owner.FrameRandom;
        }
    }

    public TrailManagerImp TrailManager
    {
        get
        {
            return owner.CurrentBeBattle.TrailManager;
        }
    }

    public BeProjectilePoolImp BeProjectilePool
    {
        get
        {
            return  owner.CurrentBeBattle.BeProjectilePool;
        }
    }

    public BeAICommandPoolImp BeAICommandPool
    {
        get
        {
            return  owner.CurrentBeBattle.BeAICommandPool;
        }
    }

    private BeActor mAiTarget;
    public BeActor aiTarget
    {
        get
        {
            return mAiTarget;
        }
        set
        {
            if (mAiTarget == null && value != null)
            {
                //EnterBattle
                _AddBuffOnAITargetChange(true);
            }
            else if (mAiTarget != null && value == null)
            {
                //OutBattle
                _AddBuffOnAITargetChange(false);
            }

            mAiTarget = value;
        }
    }
    public List<int> enterBattleBuff = new List<int>();
    public List<int> outBattleBuff = new List<int>();
    private void _AddBuffOnAITargetChange(bool isEnter)
    {
        BeActor actor = owner as BeActor;
        if (null == actor)
        {
            return;
        }
        List<int> buffs = enterBattleBuff;
        if (!isEnter)
        {
            buffs = outBattleBuff;
        }
        BeActorAIManager actorAIManager = this as BeActorAIManager;
        if(actorAIManager != null && actorAIManager.IsRunScenario())
        {
            actorAIManager.AddBattleStateChangeBuff(isEnter, buffs);
        }
        else
        {
            for(int i = 0; i < buffs.Count; i++)
            {
                actor.buffController.AddBuffInfoByID(buffs[i]);
            }
        }
    }

    public BeActor owner;
    public BeActor followTarget;

    public BTAgent actionAgent;
    public BTAgent destinationSelectAgent;
    public BTAgent eventAgent;
    public BTAgent scenarioAgent;

    public State state;

    public int sight;
    public int chaseSight;
    public TargetType targetType;
    protected IEntityFilter filter = null;
    public int warlike;
    public VInt keepDistance;
    public float followDistance;    //跟随模式用
    public int overlapOffset = 15000;   //避免重叠偏移

    public VInt skIntMaxRunAwayDisX = 30000;

    public VInt skIntMaxRunAwayDisY = 30000;


    public VInt skIntCheckDis = 3000;

    private VInt skIntBackDis = 10000;


    //一定的浮动范围
    public VInt skIntMaxRandX = 6000;
    public VInt skIntMaxRandY = 6000;

    public VInt skIntKeepDis_TableX = 20000;
    public VInt skIntKeepDis_TableY = 20000;

    public VInt skIntKeepDisRangeX = 5000;

    public VInt skIntFrontFaceXDis = 30000;
    public VInt skIntFrontFaceAndZigZagYDis = 20000;

    public VInt3 birthPosition = VInt3.zero;

    public RUNAWAY_STATE runAwayState;

    public DestinationType destinationTypeTest = DestinationType.IDLE;


    public int thinkTerm = GlobalLogic.VALUE_200;                   //行动间隔
    public int changeDestinationTerm = GlobalLogic.VALUE_400;       //寻路间隔
    public int eventTerm = GlobalLogic.VALUE_200;                   //事件间隔
    public int followTerm = GlobalLogic.VALUE_500;              //跟随间隔
    public int findTargetTerm = GlobalLogic.VALUE_1000;         //重新寻找攻击对象间隔
    public int scenarioTerm = GlobalLogic.VALUE_200;                //剧情更新间隔

    public int idleRand;
    public int escapeRand;
    public int wanderRand;
    public int idleDuration;
    public int wanderRange;
    public int wanderByTargetRange;
    public int walkBackRange;

    public int yFirstRand;

    public static int MAX_WALK_COMMAND = int.MaxValue;
    public static int MAX_IDLE_COMMAND = int.MaxValue;

    public int maxWalkCount = MAX_WALK_COMMAND;
    public int maxIdleCount = MAX_IDLE_COMMAND;

    public bool reduceSpeed = false;

    public bool targetUnchange = false;
    public bool forceFollow = false;

    public AIState aiState;
    public AIMode aiMode = AIMode.NONE;
    public IdleMode idleMode = IdleMode.IDLE;
    public DestinationType customDestinationType = DestinationType.IDLE;

    public bool afterAttack = false;

    public AIType aiType = AIType.RANGED;
    public AIType aiCombatType = AIType.RANGED;

    public bool isAPC = false;
    public bool isAutoFight = false;
    protected int doorPosCount = 0;

    private bool attackInfoLoaded = false;
    public bool pkRobotWander = false; //决斗场机器人徘徊
    protected BeActor assignAITarget = null;   //指定AI目标
    private int[] counterArr = new int[5] { 0, 0, 0, 0, 0 };    //计数器 目前容量提供五个

    //AI用计时器 默认有五个
    private int[] timerArr = new int[5];
    public int[] TimerArr { get { return timerArr; } set { timerArr = value; } }

    private bool[] timerFlagArr = new bool[5];
    public bool[] TimerFalgArr { get { return timerFlagArr; } set { timerFlagArr = value; } }

    public List<int> steps = new List<int>();
    public TargetEntityType entityFilter
    {
        set
        {
            var newfilter = new BeTargetEntityTypeFilter();
            newfilter.targetEntityType = value;
            filter = newfilter;
        }
    }
    public IEntityFilter TargetFilter
    {
        set
        {
            filter = value;
        }
    }

    public class AttackInfo
    {
        public VInt front;
        public VInt back;
        public VInt top;
        public VInt down;
        public int skillID;
        public int prob;//概率
        public bool enable = true;
        public int attackPassiveProb;

        public int priority { get; private set; }

        int Convert(int k)
        {
            return VFactor.NewVFactor(k, (long)1000).vint.i;
        }
        public AttackInfo(int f, int b, int t, int d, int sid, int p = 100, int pri = 0)
        {
            front = Convert(f);
            back = Convert(b);
            top = Convert(t);
            down = Convert(d);
            skillID = sid;
            prob = p;
            attackPassiveProb = Global.Settings.aiSkillAttackPassive;
            priority = pri;
        }

        /*
        概率,技能ID,front,back,top,down
        100,5000,500,200,100,100
        */
        public AttackInfo(string str)
        {
            string[] tokens = str.Split(',');
            prob = Int32.Parse(tokens[0]);
            skillID = Int32.Parse(tokens[1]);
            front =  Convert(Int32.Parse(tokens[2]));
            back = Convert(Int32.Parse(tokens[3]));
            top = Convert(Int32.Parse(tokens[4]));
            down = Convert(Int32.Parse(tokens[5]));

            if (tokens.Length >= 7)
            {
                priority = Convert(Int32.Parse(tokens[6]));
            }
            else
            {
                priority = 0;
            }
        }

        public bool IsPointInRange(VInt2 origin, VInt2 point, bool faceLeft = false)
        {
            DBox aabb = new DBox();
            if (faceLeft)
            {
                aabb._min.x = origin.x - front.i;
                aabb._min.y = origin.y - down.i;
                aabb._max.x = origin.x + back.i;
                aabb._max.y = origin.y + top.i;
            }
            else
            {
                aabb._min.x = origin.x - back.i;
                aabb._min.y = origin.y - down.i;
                aabb._max.x = origin.x + front.i;
                aabb._max.y = origin.y + top.i;
            }

            return aabb.containPoint(ref point);
        }

        public int CompareTo(AttackInfo other)
        {
            var a = this;
            var b = other;

            if (a.priority == b.priority)
            {
                if (a.front.i == b.front.i)
                {
                    return a.skillID < b.skillID ? -1 : 1;
                }
                else
                {
                    return a.front.i < b.front.i ? -1 : 1;
                }
            }
            else
            {
                return a.priority > b.priority ? -1 : 1;
            }
        }
    }

    public List<AttackInfo> attackInfos = new List<AttackInfo>();

    public BeAIManager()
    {
        sight = GlobalLogic.VALUE_4000;
        targetType = TargetType.NEAREST;
        warlike = 50;

        aiState = AIState.NONE;
    }

    public void Init(BeActor o)
    {
        state = State.READY;
        owner = o;
    }

    private ProtoTable.UnitTable mTableData;
    public ProtoTable.UnitTable tableData
    {
        get { return mTableData; }
    }

    public void SetSkillsEnable(List<int> skillIDs, bool flag)
    {
        for (int i = 0; i < skillIDs.Count; ++i)
        {
            for (int j = 0; j < attackInfos.Count; ++j)
            {
                var info = attackInfos[j];
                if (info.skillID == skillIDs[i])
                {
                    info.enable = flag;
                    break;
                }
            }
        }
    }

    public bool CanAIUseSkill(int skillID)
    {

        for (int i = 0; i < attackInfos.Count; ++i)
        {
            if (attackInfos[i].skillID == skillID)
            {
                return attackInfos[i].enable;
            }
        }
        return true;
    }

    public void EnableSkillDisAbleOthers(Dictionary<int, int> skillIDMap, bool flag = true)
    {
        for (int j = 0; j < attackInfos.Count; ++j)
        {
            var info = attackInfos[j];
            if (skillIDMap.ContainsKey(info.skillID))
                info.enable = flag;
            else
                info.enable = !flag;
        }
    }

    public void SetAIInfo(ProtoTable.UnitTable data, bool bForceUpdate = false)
    {
        mTableData = data;
        this.isAPC = data.AIIsAPC != 0;
        this.sight = data.AISight == 0 ? GlobalLogic.VALUE_4000 : data.AISight;
        this.chaseSight = data.AIChase == 0 ? (this.sight * GlobalLogic.VALUE_3) : data.AIChase;
        this.warlike = data.AIWarlike == 0 ? GlobalLogic.VALUE_50 : data.AIWarlike;
        this.thinkTerm = data.AIAttackDelay == 0 ? GlobalLogic.VALUE_2000 : data.AIAttackDelay;
        this.findTargetTerm = data.AIThinkTargetTerm == 0 ? GlobalLogic.VALUE_2000 : data.AIThinkTargetTerm;
        this.changeDestinationTerm = data.AIDestinationChangeTerm == 0 ? GlobalLogic.VALUE_3000 : data.AIDestinationChangeTerm;
        this.keepDistance = VInt.NewVInt(data.AIKeepDistance,(long)GlobalLogic.VALUE_1000);

        this.skIntMaxRunAwayDisX = data.skIntMaxRunAwayDisX == 0 ? VInt.NewVInt(GlobalLogic.VALUE_1000 * 3, (long)GlobalLogic.VALUE_1000) : VInt.NewVInt(data.skIntMaxRunAwayDisX, (long)GlobalLogic.VALUE_1000);
        this.skIntMaxRunAwayDisY = data.skIntMaxRunAwayDisY == 0 ? VInt.NewVInt(GlobalLogic.VALUE_1000 * 3, (long)GlobalLogic.VALUE_1000) : VInt.NewVInt(data.skIntMaxRunAwayDisY, (long)GlobalLogic.VALUE_1000);
        this.skIntKeepDis_TableX = data.skIntKeepDis_TableX == 0 ? VInt.NewVInt(GlobalLogic.VALUE_1000 * 2, (long)GlobalLogic.VALUE_1000) : VInt.NewVInt(data.skIntKeepDis_TableX, (long)GlobalLogic.VALUE_1000);
        this.skIntKeepDis_TableY = data.skIntKeepDis_TableY == 0 ? VInt.NewVInt(GlobalLogic.VALUE_1000 * 2, (long)GlobalLogic.VALUE_1000) : VInt.NewVInt(data.skIntKeepDis_TableY, (long)GlobalLogic.VALUE_1000);
        this.skIntFrontFaceAndZigZagYDis = data.skIntFrontFaceAndZigZagYDis == 0 ? VInt.NewVInt(GlobalLogic.VALUE_1000 * 2, (long)GlobalLogic.VALUE_1000) : VInt.NewVInt(data.skIntFrontFaceAndZigZagYDis, (long)GlobalLogic.VALUE_1000);

        this.followDistance = data.AIFollowDistance / (float)(GlobalLogic.VALUE_1000);
        this.targetType = (BeAIManager.TargetType)(int)data.AITargetType[0];
        this.aiType = data.AICombatType >= 10 ? (AIType)(data.AICombatType / 10 - 1) : (AIType)data.AICombatType;
        this.aiCombatType = (AIType)data.AICombatType;

        this.idleMode = (BeAIManager.IdleMode)data.AIIdleMode;
        if (Utility.IsStringValid(data.AIAttackKind[0]))
            this.LoadAttackInfo(data.AIAttackKind);

        enterBattleBuff.Clear();
        for (int i = 0; i < data.EnterBattleBuffLength; i++)
        {
            enterBattleBuff.Add(data.EnterBattleBuffArray(i));
        }
        outBattleBuff.Clear();
        for (int i = 0; i < data.OutBattleBuffLength; i++)
        {
            outBattleBuff.Add(data.OutBattleBuffArray(i));
        }

        this.idleRand = data.AIIdleRand == 0 ? 50 : data.AIIdleRand;
        this.idleDuration = data.AIIdleDuration == 0 ? GlobalLogic.VALUE_1000 : data.AIIdleDuration;
        this.escapeRand = data.AIEscapeRand == 0 ? GlobalLogic.VALUE_50 : data.AIEscapeRand;
        this.walkBackRange = data.AIWalkBackRange == 0 ? Global.Settings.aiWAlkBackRange : data.AIWalkBackRange;
        this.wanderRange = data.AIWanderRange == 0 ? Global.Settings.aiWanderRange : data.AIWanderRange;
        this.wanderRand = data.AIWanderRand == 0 ? GlobalLogic.VALUE_50 : data.AIWanderRand;
        this.wanderByTargetRange = data.AIWanderByTargetRange == 0 ? GlobalLogic.VALUE_1 : data.AIWanderByTargetRange;
        this.yFirstRand = data.AIYFirstRand == 0 ? GlobalLogic.VALUE_50 : data.AIYFirstRand;

        owner.walkSpeed = new VInt3(Global.Settings.monsterWalkSpeed) * (data.WalkSpeed / (float)(GlobalLogic.VALUE_100));
        owner.runSpeed = new VInt3(Global.Settings.monsterRunSpeed) * (data.WalkSpeed / (float)(GlobalLogic.VALUE_100));

        int defaultMaxWalkCount = data.AICombatType == 1 ? Global.Settings.aiMaxWalkCmdCount_RANGED : Global.Settings.aiMaxWalkCmdCount;

        this.maxIdleCount = data.AIMaxIdleCmdCount == 0 ? Global.Settings.aiMaxIdleCmdcount : data.AIMaxIdleCmdCount;
        this.maxWalkCount = data.AIMaxWalkCmdCount == 0 ? defaultMaxWalkCount : data.AIMaxWalkCmdCount;

        if (reduceSpeed)
        {
            owner.walkSpeed.x /= GlobalLogic.VALUE_2;
            owner.walkSpeed.y /= GlobalLogic.VALUE_2;
            owner.runSpeed.x /= GlobalLogic.VALUE_2;
            owner.runSpeed.y /= GlobalLogic.VALUE_2;
        }

        this.sight = IntMath.Float2Int(this.sight * Global.Settings.monsterSightFactor);

        if (owner.GetEntityData() != null)
            owner.GetEntityData().walkAnimationSpeedPercent = data.WalkAnimationSpeedPerent;
        
        InitAgents(data.AIActionPath, data.AIDestinationSelectPath, data.AIEventPath);
        if (actionAgent != null)
        {
            actionAgent.SetWarAlike(this.warlike);
        }

    }

    public void SetActionAgent(string action)
    {
        if (Utility.IsStringValid(action))
        {
            if (actionAgent != null)
            {
                DeinitAgent(ref actionAgent);
            }
            actionAgent = InitAgent(action);
        }
    }

    public void SetScenearioAgent(string scenario)
    {
        if (Utility.IsStringValid(scenario))
        {
            if (scenarioAgent != null)
            {
                DeinitAgent(ref scenarioAgent);
            }
            scenrioCmd = null;
            scenarioAgent = InitAgent(scenario);
        }
    }

    public void SetDestinationSelectAgent(string destination)
    {
        if (Utility.IsStringValid(destination))
        {
            if (destinationSelectAgent != null)
            {
                DeinitAgent(ref destinationSelectAgent);
            }
            destinationSelectAgent = InitAgent(destination);
        }
    }

    public void SetEventAgent(string strEvent)
    {
        if (Utility.IsStringValid(strEvent))
        {
            if (eventAgent != null)
            {
                DeinitAgent(ref eventAgent);
            }
            eventAgent = InitAgent(strEvent);
        }
    }
    
    public void InitAgents(string action, string destination, string strEvent, string scenario = null)
    {
#if DEBUG_REPORT_ROOT
		if (DebugSettings.GetInstance().DisableAI)
            return;
#endif
        SetActionAgent(action);
        SetScenearioAgent(scenario);
        SetDestinationSelectAgent(destination);
        SetEventAgent(strEvent);
    }

    public BTAgent InitAgent(string treeName)
    {
        string path = treeName;
#if AI_USEXML && LOGIC_SERVER
        path = treeName.ToLower();
#endif
        BTAgent agent = new BTAgent();
        bool ret = agent.Init(path);

        if (ret)
        {
            agent.SetEntity(owner);
            return agent;
        }

        return null;
    }

    public void DeinitAgent(ref BTAgent agent)
    {
       if(agent != null)
        {
            agent.SetEntity(null);
            
#if BEHAVIAC_NOT_USE_MONOBEHAVIOUR
            agent.UnLoad();  
#endif
            agent = null;
        }
    }

    public void UpdateAgent(BTAgent agent, int deltaTime = 0)
    {
        if (agent != null)
            agent.Tick(deltaTime);
    }

    public void LoadAttackInfo(IList<string> strInfos = null)
    {
        if (attackInfoLoaded)
            return;

        attackInfoLoaded = true;
        for (int i = 0; i < strInfos.Count; ++i)
        {
            AttackInfo info = new AttackInfo(strInfos[i]);
            if (owner.HasSkill(info.skillID))
                attackInfos.Add(info);
        }

		ReorderAttackInfo();

		for(int i=0; i<attackInfos.Count; ++i)
		{
			Logger.LogForAI("front:{0}", attackInfos[i].front);
		}
	}

    public void ReorderAttackInfo()
    {
        ////根据front的距离进行排序 TODO
        //attackInfos.Sort((a, b) =>
        //{
        //    if(a.front.i == b.front.i)
        //    {
        //        return a.skillID < b.skillID ? -1 : 1;
        //    }
        //    else
        //    {
        //        return a.front.i < b.front.i ? -1 : 1;
        //    }
        //});
        attackInfos.Sort((a, b) =>
        {
            return a.CompareTo(b);
        });
    }

    public void AddAttackProb(int prob)
    {
        for (int i = 0; i < attackInfos.Count; ++i)
        {
            attackInfos[i].prob += prob;
        }
    }

    public virtual void Start()
    {
        state = State.RUNNING_Normal;

        idleRand += FrameRandom.InRange(-GlobalLogic.VALUE_5, GlobalLogic.VALUE_5);
        escapeRand += FrameRandom.InRange(-GlobalLogic.VALUE_5, GlobalLogic.VALUE_5);
        wanderRand += FrameRandom.InRange(-GlobalLogic.VALUE_5, GlobalLogic.VALUE_5);

        //Logger.LogErrorFormat("idleRand:{0} escapeRand:{1} wander:{2}", idleRand, escapeRand, wanderRand);

        birthPosition = owner.GetPosition();

        owner.TriggerEventNew(BeEventType.onAIStart);
    }

    public bool IsRunning()
    {
        return state == State.RUNNING_Normal || state == State.RUNNING_Scenario;
    }
    
    public bool IsRunScenario()
    {
        return state == State.RUNNING_Scenario;
    }

    public void Stop()
    {
        if (state == State.STOP)
            return;

        DoNothing();
        state = State.STOP;
    }

    public void Remove()
    {
        if (state != State.STOP)
        {
            Stop();
        }

        DeinitAgent(ref actionAgent);
        DeinitAgent(ref scenarioAgent);
        DeinitAgent(ref eventAgent);
        DeinitAgent(ref destinationSelectAgent);

        owner = null;
        aiTarget = null;
        followTarget = null;
        enterBattleBuff.Clear();
        outBattleBuff.Clear();
    }

    public virtual void Update(int deltaTime)
    {
    }

    public virtual void PostUpdate(int deltaTime)
    {
    }

    /// <summary>
    /// 更新计时器
    /// </summary>
    protected void UpdateTimer(int deltaTime)
    {
        for (int i = 0; i < timerArr.Length; i++)
        {
            if (!timerFlagArr[i])
                continue;
            timerArr[i] += deltaTime;
        }
    }

    public void ExecuteCommand(BeAICommand command)
    {
        //临时调整，指令互斥
        if (currentCmd != null && currentCommand.IsAlive() && currentCmd.IsCanClose() == false)
        {
            return;
        }

        StopCurrentCommand();

        if (IsRunning())
        {
            currentCommand = command;
            currentCommand.Execute();

            if (currentCommand != null)
                owner.TriggerEventNew(BeEventType.onExecuteAICmd, new EventParam() {m_Int = (int)currentCommand.cmdType});
                //owner.TriggerEvent(BeEventType.onExecuteAICmd, new object[] { currentCommand.cmdType });

            lastCommand = currentCommand;
        }
        else
        {
            Logger.LogErrorFormat("ExecuteCommand error! manager state is {0}", state.ToString());
        }
    }

    public void StopCurrentCommand()
    {
        if (/*state == State.RUNNING &&*/ currentCommand != null && currentCommand.IsAlive())
        {
            currentCommand.End();
            currentCommand = null;
        }
    }

    public void ClrCurrentCommand()
    {
        currentCommand = null;
    }

    public virtual void SetForceFollow(bool flag)
    {

    }

    public void SetTarget(BeActor target, bool targetUnchange = false)
    {
        aiTarget = target;
        this.targetUnchange = targetUnchange;
    }

    public void FindTarget(VInt radius)
    {
        if (aiTarget == null || aiTarget != null && aiTarget.IsDead())
        {
            aiTarget = owner.CurrentBeScene.FindTarget(owner, radius);
        }
    }

    public void DoNothing()
    {
        if (currentCommand != null && currentCommand.cmdType != AI_COMMAND.NONE)
        {
            //BeAINoneCommand command = new BeAINoneCommand(owner);
            var command = BeAICommandPool.GetAICommand(AI_COMMAND.NONE, owner);

            ExecuteCommand(command);
        }
    }


    public bool CheckDistanceWithX(BeActor target, VInt distance)
    {
        return CheckDistance(target, distance, 1);
    }

    /*
	 * mode 0:x&y
	 * mode 1:only x
	 * mode 2:only y
	*/
    public bool CheckDistance(BeActor target, VInt distance, int mode = 0)
    {

        VInt3 targetPos = target.GetPosition();

        return CheckDistance(targetPos, distance, mode);
    }

    public bool CheckDistance(VInt3 targetPos, VInt distance, int mode = 0)
    {
        VInt3 pos = owner.GetPosition();

        bool retx = Mathf.Abs(targetPos.x - pos.x) <= distance;
        bool rety = Mathf.Abs(targetPos.y - pos.y) <= distance;

        if (mode == 1)
            return retx;
        else if (mode == 2)
            return rety;

        return retx && rety;
    }

    public VInt2 GetDistance()
    {
        VInt3 pos = owner.GetPosition();
        VInt3 targetPos = aiTarget.GetPosition();

        return new VInt2(Mathf.Abs(pos.x - targetPos.x), Mathf.Abs(pos.y - targetPos.y));
    }

    public VInt3 GetTargetPosition()
    {
        return aiTarget.GetPosition();
    }

    public AIMode GetAIMode()
    {
        return aiMode;
    }
    public void SetAIMode(AIMode mode)
    {
        aiMode = mode;
    }

    public virtual void ResetThinkTarget() { }
    public virtual void ResetAction() { }
    public virtual void ResetDestinationSelect() { }
    public virtual void ResetScenarioSelect() { }

#region PATH FIND

    public bool CanWalk(MoveDir dir)
    {
        if (dir == MoveDir.DOWN)
        {
            return owner.CanMoveNext(false, false);
        }
        else if (dir == MoveDir.TOP)
        {
            return owner.CanMoveNext(false, true);
        }
        else if (dir == MoveDir.LEFT)
        {
            return owner.CanMoveNext(true, false);
        }
        else if (dir == MoveDir.RIGHT)
        {
            return owner.CanMoveNext(true, true);
        }

        return false;
    }

    class _Point
    {
        public int x, y;
        public int cnt;

        public _Point()
        {
            x = y = cnt = 0;
        }
    }

    public static int[,] DIR_VALUE = {
        {0,1},{0,-1},{-1,0},{1,0},
        {-1,1},{-1,-1},{1,1},{1,1}
    };

    public static int[,] DIR_VALUE2 = {
        {1,0},{-1,0},{0,1},{0,-1},
        {1,1},{-1,1},{1,-1},{-1,-1}
    };

	public static int[,] DIR_VALUE3 = {
		{1,0},{1,1},{0,1},{-1,1},
		{-1,0},{-1,-1},{0,-1},{1,-1}
	};

    static int MAX_ROW = 200;
    static int MAX_COL = 200;

    public enum MoveDir
    {
        RIGHT 	= 0,  //0
        LEFT 	= 1,		//180
        TOP		= 2,		//90
        DOWN	= 3,		//
		RIGHT_TOP 	= 4,
		LEFT_TOP	= 5,
		RIGHT_DOWN	= 6,
		LEFT_DOWN	= 7,
		
        COUNT
    }

	//逆时针
	public enum MoveDir2
	{
		RIGHT = 0, 	//0
		RIGHT_TOP,	//45
		TOP,		//90
		LEFT_TOP,
		LEFT,
		LEFT_DOWN,
		DOWN,
		RIGHT_DOWN,

		COUNT
	}

		MoveDir [] oppositeDir = new MoveDir[] {
		MoveDir.LEFT,
		MoveDir.RIGHT,
		MoveDir.DOWN,
		MoveDir.TOP,
		MoveDir.LEFT_DOWN,
		MoveDir.RIGHT_DOWN,
		MoveDir.LEFT_TOP,
		MoveDir.RIGHT_TOP
	};

    public MoveDir GetOppositeDir(MoveDir dir)
    {
        return oppositeDir[(int)dir];
    }


    static int[,] dp = new int[MAX_ROW, MAX_COL];
    static int[,] dp2 = new int[MAX_ROW, MAX_COL];
    List<_Point> mq = new List<_Point>();
    Queue<_Point> pointPool = new Queue<_Point>();
    _Point GetNode()
    {
        _Point node = null;
        if (pointPool.Count > 0)
            node = pointPool.Dequeue();
        else
        {
            node = new _Point();
        }

        return node;
    }

    void recycleNode(_Point node)
    {
        pointPool.Enqueue(node);
    }

    public bool DoPathFinding(DGrid start, DGrid end, List<int> steps)
    {
        var data = owner.CurrentBeScene.mBlockInfo;
        int col = owner.CurrentBeScene.logicWidth;
        int row = owner.CurrentBeScene.logicHeight;

        int MAX_VALUE = GlobalLogic.VALUE_100000;

        if (start.x == end.x && start.y == end.y)
        {
            steps.Clear();
            return true;
        }


        // 新的寻路
        if (GameClient.SwitchFunctionUtility.IsOpen(21))
        {
            start.x = Mathf.Clamp(start.x, 0, col - 1);
            start.y = Mathf.Clamp(start.y, 0, row - 1);

            end.x = Mathf.Clamp(end.x, 0, col - 1);
            end.y = Mathf.Clamp(end.y, 0, row - 1);
            if (owner.CurrentBeScene.IsInBlockPlayer(start) || owner.CurrentBeScene.IsInBlockPlayer(end))                return false;

            PathHelper._start.Set(start.x, start.y);
            PathHelper._end.Set(end.x, end.y);
            return PathHelper._astar.Do(data, col, row, PathHelper._start, PathHelper._end, steps);
        }

        if (mq.Count > 0)
        {
            for (int i = 0; i < mq.Count; ++i)
            {
                recycleNode(mq[i]);
            }
        }
        mq.Clear();

        //Logger.LogForAI("start path find reach from ({0},{1}) => ({2},{3}) time:{4}", start.x, start.y, end.x, end.y, Time.realtimeSinceStartup);

        DGrid tmpStart = start;
        DGrid tmpEnd = end;

        start.x = Mathf.Clamp(tmpStart.y, 0, row - 1);
        start.y = Mathf.Clamp(tmpStart.x, 0, col - 1);

        end.x = Mathf.Clamp(tmpEnd.y, 0, row - 1);
        end.y = Mathf.Clamp(tmpEnd.x, 0, col - 1);

        for (int i = 0; i < row; ++i)
            for (int j = 0; j < col; ++j)
                dp[i, j] = MAX_VALUE;

        for (int i = 0; i < row; ++i)
            for (int j = 0; j < col; ++j)
                dp2[i, j] = MAX_VALUE;

        _Point s = GetNode();
        _Point e = GetNode();

        s.x = start.x;
        s.y = start.y;
        dp[s.x, s.y] = 0;

        e.x = end.x;
        e.y = end.y;

        s.cnt = 0;

        mq.Add(s);

        bool canReach = false;

        while (mq.Count > 0)
        {
            var t = mq[0];
            mq.RemoveAt(0);
            recycleNode(t);

            if (t.x == e.x && t.y == e.y)
            {
                e = t;
                canReach = true;
                break;
            }

            for (int i = 0; i < (int)MoveDir.COUNT; ++i)
            {
                int x = t.x + DIR_VALUE[i, 0];
                int y = t.y + DIR_VALUE[i, 1];

                if (x >= 0 && x < row && y >= 0 && y < col && data[x * col + y] == 0)
                {
                    if (dp[t.x, t.y] + 1 < dp[x, y])
                    {
                        dp[x, y] = dp[t.x, t.y] + 1;
                        dp2[x, y] = i;

                        _Point nextMove = GetNode();
                        nextMove.x = x;
                        nextMove.y = y;
                        nextMove.cnt = t.cnt + 1;
                        mq.Add(nextMove);
                    }
                }
            }
        }

        if (canReach)
        {
            //   Logger.LogForAI("can reach from ({0},{1}) => ({2},{3}) steps:{4} time:{5} ", s.x, s.y, e.x, e.y, e.cnt, (Time.realtimeSinceStartup - startTime)*1000);

            steps.Clear();

            int lastDir = dp2[e.x, e.y];
            int x = e.x;
            int y = e.y;
            int count = e.cnt - 1;
            do
            {
                if (lastDir < 0 || lastDir >= MAX_VALUE)
                    return true;

                x -= DIR_VALUE[lastDir, 0];
                y -= DIR_VALUE[lastDir, 1];

                if (lastDir == (int)MoveDir.TOP)
                    lastDir = (int)MoveDir.DOWN;
                else if (lastDir == (int)MoveDir.DOWN)
                    lastDir = (int)MoveDir.TOP;
                else if (lastDir == (int)MoveDir.RIGHT_TOP)
                    lastDir = (int)MoveDir.RIGHT_DOWN;
                else if (lastDir == (int)MoveDir.LEFT_TOP)
                    lastDir = (int)MoveDir.LEFT_DOWN;
                else if (lastDir == (int)MoveDir.RIGHT_DOWN)
                    lastDir = (int)MoveDir.RIGHT_TOP;
                else if (lastDir == (int)MoveDir.LEFT_DOWN)
                    lastDir = (int)MoveDir.LEFT_TOP;

                steps.Insert(0, lastDir);


                if (x == start.x && y == start.y)
                {
                    return true;
                    //break;
                }
                lastDir = dp2[x, y];
            } while (true);
        }
        else
        {
            //Logger.LogForAI("can not reach from ({0},{1}) => ({2},{3})", s.x, s.y, e.x, e.y);
        }

        return false;
    }

    public void DoWalk(MoveDir dir, bool reset = false)
    {
        if (reset)
        {
            Logger.Log("reset move cmd");
            owner.ResetMoveCmd();
        }

        switch (dir)
        {
            case MoveDir.RIGHT:
                owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X, true);
                break;
            case MoveDir.LEFT:
                owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X_NEG, true);
                break;
            case MoveDir.TOP:
                owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_Y, true);
                break;
            case MoveDir.DOWN:
                owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_Y_NEG, true);
                break;
            case MoveDir.LEFT_DOWN:
                owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X_NEG, true);
                owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_Y_NEG, true);
                break;
            case MoveDir.RIGHT_DOWN:
                owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X, true);
                owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_Y_NEG, true);
                break;
            case MoveDir.LEFT_TOP:
                owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X_NEG, true);
                owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_Y, true);
                break;
            case MoveDir.RIGHT_TOP:
                owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X, true);
                owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_Y, true);
                break;
        }

        Logger.LogForAI("do walk, dir:{0}", dir.ToString());
    }


    public void DoWalk(VInt3 targetPos)
    {
        Logger.LogFormat("do walk, targetPos:({0},{1},{2})", targetPos.x, targetPos.y, targetPos.z);
        VInt3 pos = owner.GetPosition();

        owner.ResetMoveCmd();
        int disx = Mathf.Abs(targetPos.x - pos.x);
        int disy = Mathf.Abs(targetPos.y - pos.y);

        MoveDir dir = MoveDir.COUNT;

        bool flag = false;

        if (targetPos.x > pos.x)
        {
            dir = MoveDir.RIGHT;
        }
        else if (targetPos.x < pos.x)
        {
            dir = MoveDir.LEFT;
        }

        if (CanWalk(dir))
        {
            flag = true;
            DoWalk(dir);
        }

        if (targetPos.y > pos.y)
        {
            dir = MoveDir.TOP;
        }
        else if (targetPos.y < pos.y)
        {
            dir = MoveDir.DOWN;
        }

        if (CanWalk(dir))
        {
            flag = true;
            DoWalk(dir);
        }

        MoveDir tmp = dir;

        if (!flag)
        {
            dir = (MoveDir)FrameRandom.InRange(0, 4);
            while (dir == tmp)
            {
                dir = (MoveDir)FrameRandom.InRange(0, 4);
            }

        }

        {
            DoWalk(dir);
        }
    }

    public VInt3 GetWalkBackPostion()
    {
        VInt3 originPos = owner.GetPosition();
        VInt3 curPos = owner.GetPosition();

        VInt randX = (int)(FrameRandom.InRange(0, walkBackRange) * IntMath.kIntDen);
        VInt randY = (int)(FrameRandom.InRange(0, walkBackRange) * IntMath.kIntDen);

        randY = FrameRandom.InRange(1, GlobalLogic.VALUE_100) > GlobalLogic.VALUE_50 ? randY : -randY;
        curPos.x += (owner.GetFace() ? 1 : -1) * randX.i;
        if (FrameRandom.Range100() > GlobalLogic.VALUE_50)
            curPos.y += randY.i;

        Logger.LogForAI("walk back ({0},{1})=>({2},{3})  randx:{4} randy:{5}", originPos.x, originPos.y, curPos.x, curPos.y, randX, randY);

        return curPos;
    }

    public VInt3 GetWanderPosition()
    {
        VInt3 originPos = owner.GetPosition();
        VInt3 curPos = owner.GetPosition();

        int offsetx = VFactor.NewVFactor(owner.CurrentBeScene.logicXSize.y - owner.CurrentBeScene.logicXSize.x, 4).roundInt;

        VInt3 centerPos = owner.CurrentBeScene.GetSceneCenterPosition();

        int factorx = 0;
        if (originPos.x < centerPos.x - offsetx)
            factorx = 1;
        else if (originPos.x > centerPos.x + offsetx)
            factorx = -1;

        int randx = (int)(FrameRandom.InRange(-wanderRange + factorx, wanderRange + 1 + factorx) * IntMath.kIntDen);
        int randy = (int)(FrameRandom.InRange(-wanderRange, wanderRange + 1) * IntMath.kIntDen);
        
        curPos.x += randx;
        curPos.y += randy;

        Logger.LogForAI("wander ({0},{1})=>({2},{3}) randx:{4} randy:{5}", originPos.x, originPos.y, curPos.x, curPos.y, curPos.x - originPos.x, curPos.y - originPos.y);

        return curPos;
    }

    //获取机器人徘徊目标位置
    public VInt3 GetPkRobotWanderPos()
    {
        VInt3 originPos = owner.GetPosition();
        VInt3 targetPos = VInt3.zero;
        if (aiTarget != null)
            targetPos = aiTarget.GetPosition();
        int offset = 3 * VInt.one.i;
        VInt3 wanderPosA = targetPos;
        VInt3 wanderPosB = targetPos;
        VInt3 wanderPosC = targetPos;
        VInt3 wanderPosD = targetPos;

        wanderPosA.x += offset;
        wanderPosA.y += offset;
        wanderPosB.x += offset;
        wanderPosB.y -= offset;
        wanderPosC.x -= offset;
        wanderPosC.y += offset;
        wanderPosD.x -= offset;
        wanderPosD.y -= offset;

        VInt minDis = (originPos - wanderPosA).magnitude;
        VInt3 minTargetPos = wanderPosA;
        if((originPos - wanderPosB).magnitude < minDis)
        {
            minDis = (originPos - wanderPosB).magnitude;
            minTargetPos = wanderPosB;
        }

        if ((originPos - wanderPosC).magnitude < minDis)
        {
            minDis = (originPos - wanderPosC).magnitude;
            minTargetPos = wanderPosC;
        }

        if ((originPos - wanderPosD).magnitude < minDis)
        {
            minDis = (originPos - wanderPosD).magnitude;
            minTargetPos = wanderPosD;
        }

        if ((originPos - minTargetPos).magnitude <= VInt.one.i)
        {
            int randomOffset = VInt.one.i;
            int randomOffsetX = FrameRandom.InRange(-randomOffset, randomOffset);
            int randomOffsetY = FrameRandom.InRange(-randomOffset, randomOffset);
            minTargetPos.x += randomOffsetX;
            minTargetPos.y += randomOffsetY;
        }
        
        return minTargetPos;
    }
	
	/// <summary>
    /// 目前来说这个方法不太靠谱
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="scene"></param>
    /// <param name="faceLeft"></param>
    /// <param name="noOtherEntity"></param>
    /// <param name="tryStep"></param>
    /// <returns></returns>
    public static VInt3 FindStandPosition(VInt3 pos, BeScene scene, bool faceLeft = false,bool noOtherEntity = false, int tryStep = 12)
    {
        if (scene == null)
            return pos;

        DGrid startGrid = scene.CalGridByPosition(pos);

        var data = scene.mBlockInfo;

        int xrange = scene.logicWidth;
        int yrange = scene.logicHeight;
        MoveDir startDir = faceLeft ? MoveDir.LEFT : MoveDir.RIGHT;
        MoveDir originDir = startDir;
        DGrid lastMove = startGrid;
        int step = 0;
        do
        {
            int x = (int)startGrid.x + BeAIManager.DIR_VALUE2[(int)startDir, 0] * step;
            int y = (int)startGrid.y + BeAIManager.DIR_VALUE2[(int)startDir, 1] * step;
            step++;
            if (x >= 0 && x < xrange && y >= 0 && y < yrange && data[y * xrange + x] == 0 &&  
                (noOtherEntity ? HasOtherEntityInPosition(scene.CalPositionByGrid(new DGrid(x, y)), scene) : true))
            {
                lastMove = new DGrid(x, y);
                return scene.CalPositionByGrid(lastMove);
            }
            else if (x < 0 || x >= xrange || y < 0 || y >= yrange || step >= tryStep)
            {
                if (step >= tryStep)
                    break;

                if (startDir == originDir)
                {
                    startDir = originDir == MoveDir.RIGHT ? MoveDir.LEFT : MoveDir.RIGHT;
                }
                else
                {
                    int tmp = (int)startDir;
                    tmp += 2;
                    tmp = tmp % (int)MoveDir.COUNT;
                    startDir = (MoveDir)tmp;
                    if (startDir == originDir)
                        break;
                }
            }

        } while (true);

        return pos;
    }

    /// <summary>
    /// 八个方向都循环一遍算一个step
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="scene"></param>
    /// <param name="faceLeft"></param>
    /// <param name="noOtherEntity"></param>
    /// <param name="tryStep"></param>
    /// <returns></returns>
    public static VInt3 FindStandPositionNew(VInt3 pos, BeScene scene, bool faceLeft = false, bool noOtherEntity = false, int tryStep = 12)
    {
        if (scene == null)
            return pos;

        DGrid startGrid = scene.CalGridByPosition(pos);

        var data = scene.mBlockInfo;

        int xrange = scene.logicWidth;
        int yrange = scene.logicHeight;
        MoveDir startDir = faceLeft ? MoveDir.LEFT : MoveDir.RIGHT;
        MoveDir originDir = startDir;
        DGrid lastMove = startGrid;
        int step = 0;
        do
        {
            int x = startGrid.x + DIR_VALUE2[(int)startDir, 0] * step;
            int y = startGrid.y + DIR_VALUE2[(int)startDir, 1] * step;

            if (x >= 0 && x < xrange && y >= 0 && y < yrange && data[y * xrange + x] == 0 &&
                (noOtherEntity ? HasOtherEntityInPosition(scene.CalPositionByGrid(new DGrid(x, y)), scene) : true))
            {
                lastMove = new DGrid(x, y);
                return scene.CalPositionByGrid(lastMove);
            }

            int tmp = (int)startDir;
            tmp += 1;
            tmp = tmp % (int)MoveDir.COUNT;
            startDir = (MoveDir)tmp;
            if (startDir == originDir)
                step++;

            if (step > tryStep)
                break;

        } while (true);

        return pos;
    }

    public VInt3 GetPosAroundDoor(BeScene scene, VInt3 pos, VInt r)
    {
        DGrid startGrid = scene.CalGridByPosition(pos);
        var data = scene.mBlockInfo;// 网格信息
        int xrange = scene.logicWidth;
        int yrange = scene.logicHeight;
        VInt2 logicGrild = scene.logicGrild;
        VInt3 targetPos = VInt3.zero;
        int step = (r.i / logicGrild.x) + 1;
        int dirCount = BeAIManager.DIR_VALUE2.Length / 2;
        for (int i = 0; i < dirCount; i++)
        {
            int ix = startGrid.x + BeAIManager.DIR_VALUE2[i, 0] * step;
            int iy = startGrid.y + BeAIManager.DIR_VALUE2[i, 1] * step;

            if (ix < 0 || ix >= xrange || iy < 0 || iy >= yrange)
                continue;

            if (data[iy * xrange + ix] != 0)
                continue;
			//因为用格子去判断是否是阻挡点不准 所以还需要转成坐标点再判断一次
            targetPos = scene.CalPositionByGrid(new DGrid(ix, iy));
            if (scene.IsInBlockPlayer(targetPos))
                continue;

            return targetPos;
        }

        // 备用方案(随机)
        int len = 3;
        int randX = FrameRandom.InRange(-len, len) * (int)IntMath.kIntDen;
        int randY = FrameRandom.InRange(-len, len) * (int)IntMath.kIntDen;
        pos.x += randX;
        pos.y += randY;

        return pos;
    }

    public VInt3 GetRandomPosInCircle(VInt3 center, VInt radius)
    {
        var r = VFactor.NewVFactor(FrameRandom.Random(2 * 3141), GlobalLogic.VALUE_1000);
        radius = FrameRandom.Random((uint) radius.i);
        var x = radius.factor * IntMath.cos(r.nom, r.den);
        var y = radius.factor * IntMath.sin(r.nom, r.den);
        center += new VInt3(x.single, y.single,0f);
        return center;
    }
    
    public VInt3 GetRandomPos(VInt3 pos, VInt r)
    {
        int randX = FrameRandom.InRange(-r.i, r.i);
        int randY = FrameRandom.InRange(-r.i, r.i);
        pos.x += randX;
        pos.y += randY;

        return pos;
    }
    public static bool HasOtherEntityInPosition(VInt3 pos, BeScene scene)
    {
        var entities = scene.GetEntities();
        for (int i = 0; i < entities.Count; ++i)
        {
            var actor = entities[i] as BeActor;
            if (actor != null)
            {
                var actorPos = actor.GetPosition();
                
                if((pos - actorPos).magnitude <= VInt.Float2VIntValue(0.9f))
                {
                    return true;
                }
            }
        }

        return false;
    }


    //Z字形移动
    protected VInt3 GetZigZagPosition(BeEntity target)
    {

        var curPos = owner.GetPosition();

        int targetPosX = target.GetPosition().x;
        int targetPosY = target.GetPosition().y;


        VInt3 pos = new VInt3(curPos.x, curPos.y, curPos.z);

        int radius = overlapOffset;

        bool isLeft = curPos.x - targetPosX < 0;

        if (Mathf.Abs(curPos.x - targetPosX) < skIntCheckDis)
        {
            var temp = FrameRandom.InRange(0, 2) == 1 ? -1 : 1;

            pos.x = targetPosX + FrameRandom.InRange(radius / 2, radius / 2 * 3);
            pos.y = curPos.y + skIntFrontFaceAndZigZagYDis.i * temp;
        }
        else
        {
            pos.x = curPos.x + (isLeft ? 1 : -1) * skIntFrontFaceXDis.i;

            var temp = FrameRandom.InRange(0, 2) == 1 ? -1 : 1;
            pos.y = curPos.y + skIntFrontFaceAndZigZagYDis.i * temp;
            if (!isMoveAble(pos.x, pos.y))
            {
                temp = -temp;

                pos.y = curPos.y + skIntFrontFaceAndZigZagYDis.i * temp;
            }
        }
        pos = FindStandPositionNew(pos, owner.CurrentBeScene);

        return pos;
    }

    //直接追后背
    protected VInt3 GetChaseBackPosition(BeEntity target)
    {
        var curPos = owner.GetPosition();

        int targetPosX = target.GetPosition().x;
        int targetPosY = target.GetPosition().y;

        int destPosX = curPos.x;
        int destPosY = curPos.y;
        int destPosZ = curPos.z;


        bool targetFace = target.GetFace();

        var targetBackX = targetPosX + (targetFace ? 1 : -1) * FrameRandom.InRange(skIntBackDis.i / 4 * 5, skIntBackDis.i / 4 * 7);

        destPosX = targetBackX + FrameRandom.InRange(-skIntMaxRandX.i, skIntMaxRandX.i);
        destPosY = targetPosY + FrameRandom.InRange(-skIntMaxRandY.i, skIntMaxRandY.i);

        VInt3 destPos = new VInt3(destPosX, destPosY, destPosZ);
        var endPos = getDestPosNotNearLastPos(curPos, destPos);
        destPosX = endPos.x;
        destPosY = endPos.y;

        if (!isMoveAble(destPosX, destPosY))
        {
            // int nearPosX = 0;
            // int nearPosY = 0;

            var pos = FindStandPositionNew(curPos, owner.CurrentBeScene, owner.GetFace());

            // findNearMoveAblePos(ref nearPosX, ref nearPosY, destPosX, destPosY, skIntFindNearRunAwayValue,
            //     FIND_POSITION_AXIS.ALL, skIntFindNearRunAwayTerm);
            destPosX = pos.x;
            destPosY = pos.y;
        }
        return new VInt3(destPosX, destPosY, destPosZ);
    }

    //最短路径走向目标（和原本的GoToTargetPostion的方法类似）
    protected VInt3 GetGoToTargetPosition(BeActor target)
    {
        VInt3 ownerPos = owner.GetPosition();
        VInt3 targetPos = target.GetPosition();

        VInt3 destPos = targetPos;

        var isLeft = ownerPos.x < targetPos.x;
        destPos.x += isLeft ? -skIntBackDis.i : skIntBackDis.i;

        destPos.x += FrameRandom.InRange(-skIntMaxRandX.i, skIntMaxRandX.i);
        destPos.y += FrameRandom.InRange(-skIntMaxRandY.i, skIntMaxRandY.i);

        return destPos;
    }

    //避免正面
    protected VInt3 GetAvoidFrontFacePosition(BeEntity target)
    {
        var curPos = owner.GetPosition();

        int targetPosX = target.GetPosition().x;
        int targetPosY = target.GetPosition().y;

        int destPosX = curPos.x;
        int destPosY = curPos.y;
        int destPosZ = curPos.z;

        bool isUp = FrameRandom.InRange(0, 2) == 0;

        /*if (Mathf.Abs(curPos.y - targetPosY) < skIntFrontFaceAndZigZagYDis)
        {
            destPosY = targetPosY + skIntFrontFaceAndZigZagYDis.i / 2 * 3 * (isUp ? 1 : -1);
            if(!isMoveAble(destPosX, destPosY))
            {
                destPosY = targetPosY + skIntFrontFaceAndZigZagYDis.i / 2 * 3 * (!isUp ? 1 : -1);
            }
            
        }*/

        destPosY = targetPosY + FrameRandom.InRange(skIntFrontFaceAndZigZagYDis.i / 4, skIntFrontFaceAndZigZagYDis.i / 2) * (!isUp ? 1 : -1);


        return new VInt3(destPosX, destPosY, destPosZ);
    }

    //追逐某物并围绕旋转
    protected VInt3 GetChaseRoundAboutPosition(BeEntity target)
    {
        var curPos = owner.GetPosition();

        int targetPosX = target.GetPosition().x;
        int targetPosY = target.GetPosition().y;

        int destPosX = curPos.x;
        int destPosY = curPos.y;
        int destPosZ = curPos.z;

        int disX = Mathf.Abs(curPos.x - targetPosX);
        int disY = Mathf.Abs(curPos.y - targetPosY);

        int roundX = skIntMaxRandX.i / 2 + FrameRandom.InRange(0, skIntMaxRandX.i / 2);
        int roundY = skIntMaxRandY.i / 2 + FrameRandom.InRange(0, skIntMaxRandY.i / 2);

        int radius = overlapOffset;

        bool left = owner.GetPosition().x < target.GetPosition().x;
        bool down = owner.GetPosition().y < target.GetPosition().y;

        var randomLeft = FrameRandom.Range100() > 50;
        var randomDown = FrameRandom.Range100() > 50;

        if (disX < roundX)
        {
            if (FrameRandom.InRange(0, 2) == 0)
            {
                destPosX = targetPosX - FrameRandom.InRange(radius / 2, radius / 2 * 3);
            }
            else
            {
                destPosX = targetPosX + FrameRandom.InRange(radius / 2, radius / 2 * 3);
            }

            if (FrameRandom.InRange(0, 2) == 0)
            {
                destPosY = targetPosY + FrameRandom.InRange(radius / 4, radius / 2);
            }
            else
            {
                destPosY = targetPosY - FrameRandom.InRange(radius / 4, radius / 2);
            }

        }
        else
        {
            destPosX = randomLeft ?
                targetPosX + radius + FrameRandom.InRange(0, skIntMaxRandX.i / 2)
                : targetPosX - radius - FrameRandom.InRange(0, skIntMaxRandX.i / 2);


            if ((down == !randomLeft && down == left) || (down == randomLeft && down == !left))
            {
                destPosY = targetPosY - roundY - FrameRandom.InRange(radius / 4, radius / 2);
            }
            else
            {
                destPosY = randomDown ?
                    targetPosY + roundY + FrameRandom.InRange(radius / 4, radius / 2)
                    : targetPosY - roundY - FrameRandom.InRange(radius / 4, radius / 2);
            }
        }


        return new VInt3(destPosX, destPosY, destPosZ);
    }

    //游荡，自身为中心
    public VInt3 GetWanderByOwnerPosition()
    {
        VInt3 originPos = owner.GetPosition();
        VInt3 curPos = birthPosition;//owner.GetPosition();

        int dis = (int)(wanderRange * IntMath.kIntDen);

        int randx = FrameRandom.InRange(-dis, dis);
        int randy = FrameRandom.InRange(-dis, dis);

        curPos.x += randx;
        curPos.y += randy;

        //Logger.LogErrorFormat("wander ({0},{1})=>({2},{3}) randx:{4} randy:{5}", originPos.x, originPos.y, curPos.x, curPos.y, curPos.x - originPos.x, curPos.y - originPos.y);
        return curPos;
    }

    //游荡目标为中心
    public VInt3 GetWanderByTargetPosition()
    {
        VInt3 originPos = owner.GetPosition();
        VInt3 curPos = birthPosition;//owner.GetPosition();
        if (aiTarget != null)
        {
            curPos = aiTarget.GetPosition();
        }

        //现在是百分之二十概率是小圈范围，百分之八十概率是大圈范围
        //小圈范围是0-wanderByTargetRange/2，大圈范围是wanderByTargetRange/2-wanderByTargetRange

        int nearRand = 20;
        int nearRange = 0;
        int farRange = (int)(wanderByTargetRange * IntMath.kIntDen / 2);

        if (FrameRandom.InRange(0, 101) > nearRand)
        {
            nearRange = farRange;
            farRange = (int)(wanderByTargetRange * IntMath.kIntDen);
        }

        int randx = FrameRandom.InRange(nearRange, farRange) * (FrameRandom.InRange(0, 2) == 1 ? 1 : -1);
        int randy = FrameRandom.InRange(nearRange, farRange) * (FrameRandom.InRange(0, 2) == 1 ? 1 : -1);

        curPos.x += randx;
        curPos.y += randy;

        return curPos;
    }

    //保持距离读表
    protected VInt3 GetKeepDistancePosition(BeEntity target)
    {
        var curPos = owner.GetPosition();

        int targetPosX = target.GetPosition().x;
        int targetPosY = target.GetPosition().y;

        int destPosX = curPos.x;
        int destPosY = curPos.y;
        int destPosZ = curPos.z;

        int distanceX = (int)skIntKeepDis_TableX;
        int distanceY = (int)skIntKeepDis_TableY;

        int rangeX = FrameRandom.InRange(-skIntKeepDisRangeX.i, skIntKeepDisRangeX.i);

        bool isPlusX = FrameRandom.InRange(0, 2) == 1;

        bool isLeft = curPos.x < targetPosX;
        int endPosX = targetPosX + (isLeft ? -skIntBackDis.i : skIntBackDis.i);
        int endPosY = targetPosY;
        if (isLeft)
        {
            if (isPlusX)
            {
                endPosX -= distanceX + rangeX;
            }
            else if (!isPlusX)
            {
                endPosX -= distanceX - rangeX;
            }
        }
        else if (isPlusX)
        {
            endPosX += distanceX + rangeX;
        }
        else if (!isPlusX)
        {
            endPosX += distanceX + rangeX;
        }

        endPosY = targetPosY + FrameRandom.InRange(-distanceY, distanceY);

        if (isMoveAble(endPosX, endPosY))
        {
            destPosX = endPosX;
            destPosY = endPosY;

        }
        else
        {
            endPosX = curPos.x + (isLeft ? -skIntBackDis.i : skIntBackDis.i);
            endPosX = ((!isLeft)
                ? (endPosX - ((!isPlusX) ? (distanceX - rangeX) : (distanceX + rangeX)))
                : (endPosX + ((!isPlusX) ? (distanceX - rangeX) : (distanceX + rangeX))));
            if (isMoveAble(endPosX, endPosY))
            {
                destPosX = endPosX;
                destPosY = endPosY;
            }
        }
        return new VInt3(destPosX, destPosY, destPosZ);
    }

    //逃跑（run）
    protected VInt3 GetRunAwayPosition(BeEntity target, ref bool run)
    {


        int targetPosX = target.GetPosition().x;
        int targetPosY = target.GetPosition().y;

        int destPosX = targetPosX;
        int destPosY = targetPosY;
        int destPosZ = target.GetPosition().z;

        bool isLeft;
        bool isDown;
        int disX;
        int disY;
        var curPos = owner.GetPosition();
        isLeft = curPos.x < targetPosX;
        disX = isLeft ? targetPosX - curPos.x : curPos.x - targetPosX;
        isDown = curPos.y < targetPosY;
        disY = isDown ? targetPosY - curPos.y : curPos.y - targetPosY;
        run = true;
        destPosY = curPos.y;
        if (disX > skIntMaxRunAwayDisX)
        {
            destPosX = curPos.x;
        }
        else
        {
            switch (runAwayState)
            {
                case RUNAWAY_STATE.NORMAL:
                    if (disX < skIntMaxRunAwayDisX.i / 2)
                    {
                        if (!isMoveAble(curPos.x - skIntCheckDis.i, curPos.y))
                        {
                            destPosX = curPos.x + skIntMaxRunAwayDisX.i;
                            runAwayState = RUNAWAY_STATE.RIGHT;
                        }
                        else if (!isMoveAble(curPos.x + skIntCheckDis.i, curPos.y))
                        {
                            destPosX = curPos.x - skIntMaxRunAwayDisX.i;
                            runAwayState = RUNAWAY_STATE.LEFT;
                        }
                    }

                    if (runAwayState == RUNAWAY_STATE.NORMAL)
                    {
                        if (!isMoveAble(curPos.x - skIntCheckDis.i, curPos.y))
                        {
                            destPosX = curPos.x;
                            run = false;
                        }
                        else if (!isMoveAble(curPos.x + skIntCheckDis.i, curPos.y))
                        {
                            destPosX = curPos.x;
                            run = false;
                        }
                        else if (isLeft)
                        {
                            destPosX = curPos.x - FrameRandom.InRange(skIntMaxRunAwayDisX.i / 2, skIntMaxRunAwayDisX.i);
                            destPosY = curPos.y + FrameRandom.InRange(0, skIntMaxRunAwayDisY.i) - skIntMaxRunAwayDisY.i / 2;
                        }
                        else
                        {
                            destPosX = curPos.x + FrameRandom.InRange(skIntMaxRunAwayDisX.i / 2, skIntMaxRunAwayDisX.i);
                            destPosY = curPos.y + FrameRandom.InRange(0, skIntMaxRunAwayDisY.i) - skIntMaxRunAwayDisY.i / 2;
                        }
                    }
                    break;
                case RUNAWAY_STATE.LEFT:
                    destPosX = curPos.x - skIntMaxRunAwayDisX.i;
                    destPosY = curPos.y;
                    if (disX <= skIntMaxRunAwayDisX &&
                        !isMoveAble(curPos.x + skIntMaxRunAwayDisX.i, curPos.y))
                    {
                        break;
                    }
                    runAwayState = RUNAWAY_STATE.NORMAL;
                    break;
                case RUNAWAY_STATE.RIGHT:
                    destPosX = curPos.x + skIntMaxRunAwayDisX.i;
                    destPosY = curPos.y;
                    if (disX <= skIntMaxRunAwayDisX &&
                        !isMoveAble(curPos.x - skIntMaxRunAwayDisX.i, curPos.y))
                    {
                        break;
                    }
                    runAwayState = RUNAWAY_STATE.NORMAL;
                    break;
                case RUNAWAY_STATE.TOP:
                    destPosX = curPos.x;
                    destPosY = curPos.y + skIntMaxRunAwayDisY.i;
                    if (disY <= skIntMaxRunAwayDisY &&
                        isMoveAble(curPos.x, curPos.y - skIntCheckDis.i) && !isMoveAble(curPos.x, curPos.y - skIntMaxRunAwayDisY.i * 2))
                    {
                        break;
                    }
                    runAwayState = RUNAWAY_STATE.NORMAL;
                    break;
                case RUNAWAY_STATE.BOTTOM:
                    destPosX = curPos.x;
                    destPosY = curPos.y - skIntMaxRunAwayDisY.i;
                    if (disY <= skIntMaxRunAwayDisY &&
                        isMoveAble(curPos.x, curPos.y + skIntCheckDis.i) && !isMoveAble(curPos.x, curPos.y + skIntMaxRunAwayDisY.i * 2))
                    {
                        break;
                    }
                    runAwayState = RUNAWAY_STATE.NORMAL;
                    break;
            }
        }
        return new VInt3(destPosX, destPosY, destPosZ);
    }

    protected VInt3 GetMoveWPosition(BeEntity target)
    {
        var ownerPos = owner.GetPosition();
        var targetPos = target.GetPosition();

        var pos = targetPos;

        pos = getDestPosNotNearLastPos(ownerPos, targetPos);
        return pos;
    }

    private VInt3 getDestPosNotNearLastPos(VInt3 curPos, VInt3 destPos)
    {
        int nearX = skIntMaxRandX.i;
        int nearY = skIntMaxRandY.i;
        var disX = Mathf.Abs(destPos.x - curPos.x);
        var disY = Mathf.Abs(destPos.y - curPos.y);
        if (disX < nearX && disY < nearY)
        {
            curPos.x += (FrameRandom.InRange(0, 2) == 1 ? 1 : -1) * FrameRandom.InRange(nearX, skIntMaxRandX.i);
            curPos.y += (FrameRandom.InRange(0, 2) == 1 ? 1 : -1) * FrameRandom.InRange(nearY, skIntMaxRandY.i);
            return curPos;
        }
        return destPos;
    }

    public bool isMoveAble(int x, int y)
    {
        var beScene = owner.CurrentBeScene;

        if (beScene == null)
        {
            return false;
        }


        bool flag = true;

        int xx = owner.CurrentBeScene.CalGridByPosition(new VInt3(x, y, owner.GetPosition().z)).x;
        int yy = owner.CurrentBeScene.CalGridByPosition(new VInt3(x, y, owner.GetPosition().z)).y;

        if (beScene.IsInBlockPlayer(new DGrid(xx, yy)))
        {
            flag = false;
        }

        return flag;
    }

    #endregion

    /*------------------------------------------------------------------*/

    #region tree node
    public int NumberOfInAttackArea(int front, int back, int top, int down)
    {
        AttackInfo attackInfo = new AttackInfo(front, back, top, down, 0);

        List<BeActor> allTargets = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindTargets(allTargets, owner, (VInt)(float)(GlobalLogic.VALUE_1000));

        int count = 0;
        for (int i = 0; i < allTargets.Count; ++i)
        {
            if (attackInfo.IsPointInRange(owner.GetPosition2(), allTargets[i].GetPosition2(), owner.GetFace()))
            {
                count++;
            }
        }

        GamePool.ListPool<BeActor>.Release(allTargets);

        return count;
    }

    public int MonsterInArea(int front, int back, int top, int down, int[] monsterIDs,bool isEnemey = true)
    {
        if (monsterIDs == null || monsterIDs.Length <= 0)
        {
            monsterIDs = new int[] { 0 };
        }

        AttackInfo attackInfo = new AttackInfo(front, back, top, down, 0);
        int count = 0;
        for (int i = 0; i < monsterIDs.Length; ++i)
        {
            List<BeActor> list = GamePool.ListPool<BeActor>.Get();
            owner.CurrentBeScene.FindMonsterByID(list, monsterIDs[i],isEnemey);

            for (int j = 0; j < list.Count; ++j)
                if (attackInfo.IsPointInRange(owner.GetPosition2(), list[j].GetPosition2(), owner.GetFace()))
                    count++;

            GamePool.ListPool<BeActor>.Release(list);
        }

        return count;
    }

    public bool IsTargetInAttackArea(int front, int back, int top, int down)
    {
        if (aiTarget == null)
            return false;

        AttackInfo attackInfo = new AttackInfo(front, back, top, down, 0);
        return attackInfo.IsPointInRange(owner.GetPosition2(), aiTarget.GetPosition2(), owner.GetFace());
    }
    public bool IsTargetInConcentricCircles(int front0, int back0, int top0, int down0, int front1, int back1, int top1, int down1)
    {
        if (aiTarget == null)
            return false;

        AttackInfo attackInfo1 = new AttackInfo(front0, back0, top0, down0, 0);
        AttackInfo attackInfo2 = new AttackInfo(front1, back1, top1, down1, 0);
        if (attackInfo1.IsPointInRange(owner.GetPosition2(), aiTarget.GetPosition2(), owner.GetFace()))
        {
            return false;
        }
        return attackInfo2.IsPointInRange(owner.GetPosition2(), aiTarget.GetPosition2(), owner.GetFace());
    }

    public bool CanUseSkill(int skillID)
    {
        return owner.CanUseSkill(skillID);
    }

    //根据Vip设置判断是否可以使用无色晶体技能
    public bool CanCost(int skillId)
    {
        BeSkill skill = owner.GetSkill(skillId);
        if (skill == null)
            return true;
        if (skill.GetCrystalCost() <= 0)
            return true;
        if (!owner.canUseCrystalSkill)
            return false;
        return true;
    }

    public bool IsSkillInCooltime(int skillID)
    {
        return !owner.CanUseSkill(skillID);
    }

    public int EnemyNumberOfInAttackArea(int front, int back, int top, int down)
    {
        return NumberOfInAttackArea(front, back, top, down);
    }

    public bool CheckUseSkill(int skillID)
    {
        return (owner.IsCastingSkill() && owner.GetCurSkillID() == skillID);
    }

    public bool CheckState(BeActor actor, ActionState state)
    {
        return actor.sgGetCurrentState() == (int)state;
    }
#endregion

    public void ReloadSkillInfos(ProtoTable.UnitTable data)
    {
        attackInfos.Clear();
        attackInfoLoaded = false;

        if (data == null)
            return;

        // 
        if (Utility.IsStringValid(data.AIAttackKind[0]))
            this.LoadAttackInfo(data.AIAttackKind);
    }

    /// <summary>
    /// 强制设定AI目标
    /// </summary>
    public void ForceAssignAiTarget(BeActor actor)
    {
        assignAITarget = actor;
    }

    /// <summary>
    /// 根据计数器编号获取当前计数器的值
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public int GetCounter(int index)
    {
        if (index >= 5)
        {
            Logger.LogErrorFormat("计数器编号不能大于4，策划请检查AI配置");
            return 0;
        }
        return counterArr[index];
    }

    /// <summary>
    /// 设置计数器的值
    /// </summary>
    public void SetCounter(int index,int value)
    {
        if (index >= 5)
        {
            Logger.LogErrorFormat("计数器编号不能大于4，策划请检查AI配置");
            return;
        }
        counterArr[index] = value;
    }

    /// <summary>
    /// 指定编号的计数器累加
    /// </summary>
    /// <param name="index"></param>
    public void CounterAddUp(int index)
    {
        if (index >= 5)
        {
            Logger.LogErrorFormat("计数器编号不能大于4，策划请检查AI配置");
            return;
        }
        counterArr[index]++;
    }
}
