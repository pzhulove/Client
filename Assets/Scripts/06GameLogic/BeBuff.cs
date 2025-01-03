using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using GameClient;

#region  BUFF_TYPE
public class BDStat
{
    public enum State
    {
        READY,
        RUNNING,
        DEAD
    }

    State stat;

    public BDStat()
    {
        stat = State.READY;
    }

    public State GetState()
    {
        return stat;
    }

    public bool IsReady() { return stat == State.READY; }
    public bool IsRunning() { return stat == State.RUNNING; }
    public bool IsDead() { return stat == State.DEAD; }

    public void SetReady() { stat = State.READY; }
    public void SetRunning() { stat = State.RUNNING; }
    public void SetDead() { stat = State.DEAD; }

	public void Reset()
	{
		stat = State.READY;
	}
}
public enum BuffTimeType
{
    ONCE,
    INTERVAL,//周期�?
}
/*
0:属性修�?
1:霸体
2:迅捷
3:祝福
4:透明
5:硬质
6:感电
7:出血
8:灼烧
9:中毒
10:失明
11:眩晕
12:石化
13:冰冻
14:睡眠
15:混乱
16:束缚
17:减速
18:诅咒

*/
public enum BuffType
{
    ALL_SKILL_BUFF = -8,//技能buff，增加所有技能的属�?
    USE_MECHANISM = -7,//使用机制
	USE_SKILL = -6,//使用技�?
	TRIGGER_BUFFINFO = -5,//触发别的触发信息表ID
	BUFF_DISPEL = -4,//BUFF驱散
	SUMMON = -3,
    SKILL_BUFF = -2,//技能buff，增加技能的属�?
	ATTRIBUTE_ADD = -1,	//回HP，MP
	ATTRIBUTE_CHANGE = 0,//属性修�?,一个或多个属性增�?

    //增溢
	BATI,			/// 霸体
	SPEED_UP,		/// 迅捷
	BLESS,			/// 祝福
	TRANSPARENT,	/// 透明
	HARD,			/// 硬质
    //异常状�?
    [UIProperty("感电", "")]
    FLASH,			/// 感电
    [UIProperty("出血", "")]
    BLEEDING,		/// 出血
    [UIProperty("灼烧", "")]
    BURN,			/// 灼烧
    [UIProperty("中毒", "")]
    POISON,			/// 中毒
    [UIProperty("失明", "")]
    BLIND,			/// 失明
    [UIProperty("眩晕", "")]
    STUN,			/// 眩晕
    [UIProperty("石化", "")]
    STONE,			/// 石化
    [UIProperty("冰冻", "")]
    FROZEN,			/// 冰冻
    [UIProperty("睡眠", "")]
    SLEEP,			/// 睡眠
    [UIProperty("混乱", "")]
    CONFUNSE,		/// 混乱
    [UIProperty("束缚", "")]
    STRAIN,			/// 束缚
    [UIProperty("减速", "")]
    SPEED_DOWN,		/// 减�?
    [UIProperty("诅咒", "")]
    CURSE,			/// 诅咒
    // 服务器 使用 19
    // 服务器 使用 20
    // 服务器 使用 21
    // 服务器 使用 22


    /// 伤害:反弹,回血,回蓝
   
    DAMAGESTATE = 23, 
    BATI_NO_PAUSE = 24,
}
//buff作用类型
public enum BuffWorkType
{
    DEBUFF = 0,     //减溢
    GAINBUFF = 1,   //增溢
    SKILLBUFF       //技能实�?
}

public enum BuffOverlayType
{
    OVERLAY_NONE = 0, //互不影响
    OVERLAY_DAMAGE,
    OVERLAY_TIME,
    OVERLAY_CANT,   //不能覆盖
	OVERLAY_DELETE, //叠加就删除原来的，并且新的也不加
    OVERLAY_ALONE,   //独立叠加（用于异常Buff叠加�?
    OVERLAY_QUEUE,  //先进先出
    OVERLAY_ADDNEW, //删除原来�? 添加新的
}
#endregion

public class BeBuff
{
    protected int _id;
    public int PID { //唯一ID
        get { return _id;}
        set {_id = value;}
    }

	//public string name;
	public CrypticInt32 level;
    public CrypticInt32 abnormalLevel;
	public int buffID;
    
    public BeActor owner { get; set; }//buff作用的单�?
    public BeActor releaser { get; set; }//释放buff的单�?

    public BuffType buffType;
    public BuffWorkType buffWorkType;
    public BuffTimeType timeType = BuffTimeType.ONCE;
    public BuffOverlayType overlayType;
    public bool isShowSpell = false;

    bool dispel = false;
    public bool passive = false;//是否是被动技能带的buff

    //public List<int> damageList = new List<int>();
    int totalDamage = -1;

	public CrypticInt32 buffAttack;//buff攻击�?
    public int totalAttackCount;
    int attackCount = 0;

	public CrypticInt32 timeInterval;
	public CrypticInt32 duration;
    bool buffEffectAni = false;

    public ProtoTable.BuffTable buffData;

    protected BuffEffect buffEffect = null;

    public int overlayCount = 1;//叠加次数

    public BDStat state = new BDStat();

    int timeAcc;
    int intervalAcc;

    bool addBuffInAir = false;
    bool addBuffFallGround = false;
    bool isReduceByQuickPress = false;//是否被快速点击加速解�?
    private bool m_NeedRestoreTargetAction = false;
    public bool NeedRestoreTargetAction
    {
        set { m_NeedRestoreTargetAction = value; }
    }

    protected int[] stateChanged; //= new int[30];
	protected int stateChangedCount = 0;
 
    //事件处理
    //protected BeEventProcessor eventProcessor = new BeEventProcessor();
    protected GameClient.BeEventManager m_EventManager = new GameClient.BeEventManager(-1);

    //技�?
    public List<int> skillIDs = null;

	protected int[] valueChanged;// = new int[15];

    public int runTime {get{return timeAcc;}}
   // protected List<BeMechanism> mechanismList;// = new List<BeMechanism>();
    protected List<int> mechanismPIDList = null;
    public AbnormalBuffData abnormalBuffData = new AbnormalBuffData();      //记录异常Buff叠加相关数据
    protected List<IBeEventHandle> _handleList = new List<IBeEventHandle>();
    public int buffInfoId = 0;   //记录下BuffInfoID  
    public int skillId = 0; //记录下Buff添加时候的 Buff释放者的当前技能ID              
    private Dictionary<int, int> m_AddSkillLevel = null;
    public void Reset()
    {
        
        //name = "";
        level = 0;
        abnormalLevel = 0;
        buffID = 0;
        // marked by ckm
        // owner = null;
        releaser = null;
        buffType = BuffType.ATTRIBUTE_CHANGE;
        buffWorkType = BuffWorkType.DEBUFF;
        timeType = BuffTimeType.ONCE;
        overlayType = BuffOverlayType.OVERLAY_CANT;
        isShowSpell = false;
        dispel = false;
        passive = false;
        //damageList.Clear();
        totalDamage = 0;
        buffAttack = 0;
        totalAttackCount = 0;
        attackCount = 0;
        timeInterval = 0;
        duration = 0;
        buffEffectAni = false;
        // marked by ckm
        // buffData = null;

        overlayCount = 1;

        timeAcc = 0;
        intervalAcc = 0;

        addBuffInAir = false;
        addBuffFallGround = false;
        isReduceByQuickPress = false;
        m_NeedRestoreTargetAction = false;


        if (buffEffect != null)
            buffEffect.Reset();
        
        if (stateChanged != null)
        {
            stateChangedCount = 0;
            for(int i=0; i<stateChanged.Length; ++i)
			    stateChanged[i] = 0;
        }

        state.Reset();
       
        if (skillIDs != null)
            skillIDs.Clear();
        if (valueChanged != null)
        {   
            for(int i=0; i<valueChanged.Length; ++i)
			    valueChanged[i] = 0;
        }

        // if (mechanismList != null)
        //     mechanismList.Clear();
        if (mechanismPIDList != null)
            mechanismPIDList.Clear();

        abnormalBuffData = new AbnormalBuffData();

        skillId = 0;

        if (m_AddSkillLevel != null)
            m_AddSkillLevel.Clear();

        m_EventManager.ClearAll();
        
        OnReset();   
    }

    public virtual void OnReset(){}

    void InitForValueChanged()
    {
        if (valueChanged == null)
        {
            valueChanged = new int[15];
        }
    }
    void InitForMechanismList()
    {
        // if (mechanismList == null)
        //     mechanismList = new List<BeMechanism>();
        if (mechanismPIDList == null)
            mechanismPIDList = new List<int>();
    }
    public BeBuff(int bi, int buffLevel, int buffDuration, int attack = 0, bool buffEffectAni = true)
    {

		Init(bi, buffLevel, buffDuration, attack, buffEffectAni);
    }



    public BattleType  battleType
    {
        get
        {
           return owner.battleType;
        }
    }

	public void Init(int bi, int buffLevel, int buffDuration, int attack = 0, bool buffEffectAni = true)
	{
        //this._id = id;
		buffID = bi;
		level = buffLevel;
		duration = buffDuration;
		buffAttack = attack;		
		this.buffEffectAni = buffEffectAni;

		LoadBuffByID(bi);
	}

	public void DeInit()
	{
		//if (stateChanged != null)
		//	GamePool.ListPool<int>.Release(stateChanged);
	}

    public void LoadBuffByID(int bid)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.LoadBuffByID"))
        {
#endif
        try
        {
            var data = TableManager.GetInstance().GetTableItem<ProtoTable.BuffTable>(bid);
            if (data != null)
            {
                //name = data.Name;
                buffData = data;

                buffType = (BuffType)buffData.Type;
                buffWorkType = (BuffWorkType)buffData.WorkType;
                overlayType = (BuffOverlayType)buffData.Overlay;
                isShowSpell = buffData.IsShowSpell && buffData.OverlayLimit == 1;

                if (buffData.TriggerInterval > 0)
                {
                    timeType = BuffTimeType.INTERVAL;
                    timeInterval = buffData.TriggerInterval;
                    totalAttackCount = duration / data.TriggerInterval;
                }

                if (buffData.DispelType == 1)
                {
                    dispel = true;
                }
            }

            if (BuffEffect.NeedCreateBuffEffect(bid))
            {
                buffEffect = new BuffEffect();
                buffEffect.Init(bid, buffEffectAni);
                buffEffect.buff = this;
            }
        }
        catch(Exception e)
        {
            Logger.LogErrorFormat("Load buff error bid: {0}, exception: {1}", bid, e.Message);
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public void PostInit()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.PostInit"))
        {
#endif
        OnInit();
#if ENABLE_PROFILER
        }
#endif
    }

    //设置Buff的Releaser后处�?
    public void SetBuffReleaser(BeActor actor)
    {
        if (actor == null)
            return;
        if (releaser != null)
            return;
        releaser = actor;
        skillId = releaser.GetCurSkillID();
        //releaser.TriggerEvent(BeEventType.OnAddBuffToOthers, new object[] { owner,this });
        releaser.TriggerEventNew(BeEventType.OnAddBuffToOthers, new EventParam(){m_Obj = owner, m_Obj2 = this});
    }

    public virtual bool CanAdd(BeActor target)
    {
        return true;
    }

	protected void DoSyncHPBar(BeActor actor)
	{
#if !LOGIC_SERVER
 
		if (actor != null && actor.m_pkGeActor != null)
            actor.m_pkGeActor.SyncHPBar();
 #endif
	}

    public int GetAbnromalDamage(int buffAttack, int duration)
    {
        int damage = 0;
        if (buffAttack > 0)
        {
            if (buffType == BuffType.FLASH)
            {
                damage = owner.GetEntityData().GetAbnormalDamage(buffAttack, 0,releaser);
            }
            else
            {
				damage = owner.GetEntityData().GetAbnormalDamage(buffAttack, IntMath.Float2Int(duration / (float)(GlobalLogic.VALUE_1000) * GlobalLogic.VALUE_2), releaser);
            }
        }

        return damage;
    }

    public void Start()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.Start"))
        {
#endif
        state.SetRunning();
        TriggerEventNew(BeEventType.onBuffStart, new EventParam{m_Int = buffID});

        if (owner != null)
        {
            owner.TriggerEventNew(BeEventType.onBuffStart, new EventParam{m_Int = buffID});

            if (isShowSpell)
            {
				owner.StartSpellBar(eDungeonCharactorBar.Buff, duration, true, buffData.HeadName, true);
                owner.AddStateBar(buffData.HeadName, duration);
            }
        }

		if (timeInterval > 0 && buffType == BuffType.TRIGGER_BUFFINFO)
			;
		else
        	DoWork();

        OnStart();
#if ENABLE_PROFILER
        }
#endif
    }

    void ChangeTargetAction()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.ChangeTargetAction"))
        {
#endif
        if (owner.GetPosition().z > 0)
            addBuffInAir = true;

        if (buffData != null && buffData.TargetState.Length > 0 && buffData.TargetState[0] != -1)
        {
            var state = buffData.TargetState[0];
            if (CanLocomoteState(state))
            {
                m_NeedRestoreTargetAction = true;
                //owner.Locomote(new BeStateData(state, 0, 0, 0, 0, 0, duration, true), true);
                owner.Locomote(new BeStateData(state) { _timeout = duration, _timeoutForce = true }, true);
            }
        }

        if (owner.HasTag((int)AState.AST_FALLGROUND))
            addBuffFallGround = true;
#if ENABLE_PROFILER
        }
#endif
    }
    private bool CanLocomoteState(int state)
    {
        if (owner.stateController != null)
        {
            if (state == (int)ActionState.AS_BUSY || state == (int)ActionState.AS_GRABBED || state == (int)ActionState.AS_HURT)
            {
                return !owner.stateController.HaveSuperBati();
            }
        }
        return true;
    }
    void RestoreTargetAction()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.RestoreTargetAction"))
        {
#endif
        if (!m_NeedRestoreTargetAction) return;
        if (buffData.TargetState.Length > 0 && buffData.TargetState[0] != -1)
        {
            if (owner.sgGetCurrentState() != (int)ActionState.AS_GRABBED)
            {
                bool roughSwitchIdle = false;
                if (buffData.TargetState.Length == 2 && buffData.TargetState[1] > 0)
                {
                    roughSwitchIdle = true;
                }
                bool needClearProtect = false;
                if (owner.GetStateGraph().HasStateInStack((int)ActionState.AS_GETUP) || 
                    owner.GetPosition().z <= 0 && addBuffInAir ||
                    addBuffFallGround)
                {
                    needClearProtect = true;
                    if (roughSwitchIdle)
                    {
                        owner.Locomote(new BeStateData((int)ActionState.AS_IDLE));
                    }
                    else
                    {
                        if (isReduceByQuickPress)
                            owner.Locomote(new BeStateData((int)ActionState.AS_IDLE));
                        else
                        {
                            if (owner.HasTag((int)AState.AST_FALLGROUND))
                            {
                                owner.Locomote(new BeStateData((int)ActionState.AS_IDLE));
                            }
                            else
                                owner.sgPushState(new BeStateData((int)ActionState.AS_IDLE));
                        }
                    }

                }
                else
                {
                    if (roughSwitchIdle)
                    {
                        owner.Locomote(new BeStateData((int)ActionState.AS_IDLE));
                    }
                    else
                    {
                            if (owner.sgGetCurrentState() != (int)ActionState.AS_GETUP)
                                owner.Locomote(new BeStateData((int)ActionState.AS_IDLE));
                            else
                                owner.sgPushState(new BeStateData((int)ActionState.AS_IDLE));
                    }
                }
                //如果是冰冻，眩晕，石化，睡眠，在空中上的，在地上还没结束的要清一下浮空保护，因为不会进入GetUp状�?
                if (needClearProtect)
                {
                    owner.protectManager.ClearGroundProtect();
                    owner.protectManager.DelayClearFallProtect();
                }
                    
            }
                //owner.Locomote(new BeStateData((int)ActionState.AS_IDLE));
        }

#if ENABLE_PROFILER
        }
#endif
    }

    void ChangeTargetState()
    {
        if (buffData != null)
        {
			for(int i=0; i<buffData.StateChange.Count; ++i)
			{
				var state = buffData.StateChange[i];
				if (state < 1)
					continue;

				BeBuffStateType bs = (BeBuffStateType)(1 << state);

				if (!owner.stateController.HasBuffState(bs))
				{
					owner.stateController.SetBuffState(bs);
					//stateChanged.Add((int)bs, 1);
					//stateChanged.Add((int)bs);
                    if (stateChanged == null)
                        stateChanged = new int[30];
					stateChanged[stateChangedCount++] = (int)bs;
				}
			}
        }
    }

    void RestoreTargetState()
    {
        if (buffData != null && stateChanged != null)
        {
            //foreach (var key in stateChanged.Keys)
			for(int i=0; i<stateChangedCount; ++i)
            {
				int key = stateChanged[i];
                owner.stateController.SetBuffState((BeBuffStateType)key, true);
            }
        }
    } 
    void ChangeTargetAbility()
    {
        if (buffData != null)
        {
            //foreach (var ability in buffData.AbilityChange)
			for(int i=0; i<buffData.AbilityChange.Count; ++i)
            {
				var ability = buffData.AbilityChange[i];
                BeAbilityType at = (BeAbilityType)(int)ability;
				owner.stateController.SetAbilityEnable(at, false);
            }
        }

        if (!owner.stateController.CanMove())
        {
            owner.ClearMoveSpeed();
        }
    }

    void RestoreTargetAbility()
    {
        if (buffData != null)
        {
            //foreach (var ability in buffData.AbilityChange)
			for(int i=0; i<buffData.AbilityChange.Count; ++i)
            {                
				var ability = buffData.AbilityChange[i];
				owner.stateController.SetAbilityEnable((BeAbilityType)(int)ability, true);
            }
        }
    }

    struct _propertyRate
    {
        public string buffName;
        public string battleDataName;
        public bool isField; 
		public AttributeType at;

		public _propertyRate(string bn, string bd, bool flag, AttributeType type)
        {
            buffName = bn;
            battleDataName = bd;
            isField = flag;
			at = type;
        }

    }

    static _propertyRate[] _propertyRateName = {
        new _propertyRate( "atkAddRate", "baseAtk", true, AttributeType.baseAtk),
        new _propertyRate( "intAddRate", "baseInt", true, AttributeType.baseInt),
        new _propertyRate( "independenceAddRate", "baseIndependence", true, AttributeType.baseIndependence),
        new _propertyRate( "staAddRate", "sta", false, AttributeType.sta),
		new _propertyRate( "sprAddRate", "spr", false, AttributeType.spr),
		new _propertyRate( "maxHpAddRate", "_maxHp", true, AttributeType.maxHp),
		new _propertyRate( "maxMpAddRate", "_maxMp", true, AttributeType.maxMp),
		new _propertyRate( "ignoreDefAttackAddRate", "ignoreDefAttackAdd", true, AttributeType.ignoreDefAttackAdd),
		new _propertyRate( "ignoreDefMagicAttackAddRate", "ignoreDefMagicAttackAdd", true, AttributeType.ignoreDefMagicAttackAdd),
    };


    void ChangeAIAttribute()
    {
        if (buffData != null && owner.aiManager != null)
        {
            int sight = TableManager.GetValueFromUnionCell(buffData.ai_sight, level);
            int warlike = TableManager.GetValueFromUnionCell(buffData.ai_warlike, level);
            int attackProb = TableManager.GetValueFromUnionCell(buffData.ai_attackProb, level);

             if (sight > 0)
                owner.aiManager.sight += sight;
            if (warlike > 0)
                owner.aiManager.warlike += warlike;
            if (attackProb > 0)
            {
                owner.aiManager.AddAttackProb(attackProb);
            }
                
        }
    }

    void RestoreAIAttribute()
    {
        if (buffData != null && owner.aiManager != null)
        {
            int sight = TableManager.GetValueFromUnionCell(buffData.ai_sight, level);
            int warlike = TableManager.GetValueFromUnionCell(buffData.ai_warlike, level);
            int attackProb = TableManager.GetValueFromUnionCell(buffData.ai_attackProb, level);

            if (sight > 0)
                owner.aiManager.sight -= sight;
            if (warlike > 0)
                owner.aiManager.warlike -= warlike;
            if (attackProb > 0)
            {
                owner.aiManager.AddAttackProb(-attackProb);
            }
        }
    }

    void ChangeAttribute()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.ChangeAttribute"))
        {
#endif
       if (buffData != null)
       {
			bool needSyncHPBar = false;
            var entityData = owner.GetEntityData();

            for(int i=0; i<=(int)AttributeType.baseSpr; ++i)
            {
                {
					var cellValue = TableManager.GetInstance().GetBuffTableProperty((AttributeType)i, buffData);
					if (cellValue == null)
						continue;
					
                    int value = TableManager.GetValueFromUnionCell(cellValue, level);

					if ((AttributeType)i == AttributeType.maxHp && value != 0)
					{
						needSyncHPBar = true;
						entityData.ChangeMaxHp(value);
					}
					else if ((AttributeType)i == AttributeType.maxMp && value != 0)
					{
						needSyncHPBar = true;
						entityData.battleData.ChangeMaxMP(value);
					}
					else 
					{
						if (value != 0)
						{
							entityData.SetAttributeValue((AttributeType)i, value, true);
							if ((AttributeType)i == AttributeType.hpRecover)
								owner.GetEntityData().battleData.hpRecover = owner.GetEntityData().battleData.fHpRecoer;
							else if ((AttributeType)i == AttributeType.mpRecover)
								owner.GetEntityData().battleData.mpRecover = owner.GetEntityData().battleData.fMpRecover;
						}
							
					}
                }
            }

            int staValue = TableManager.GetValueFromUnionCell(buffData.sta, level);
            if (staValue != 0)
            {
                entityData.battleData.sta += staValue;
				needSyncHPBar = true;
            }
            
            int sprValue = TableManager.GetValueFromUnionCell(buffData.spr, level);
            if (sprValue != 0)
            {
                entityData.battleData.spr += sprValue;
				needSyncHPBar = true;
            }

            /*******************************************************************************/
            //百分比直接转换为数值进行加成，但是要记录这个加成值供恢复的时候用
            for(int i=0; i< _propertyRateName.Length; ++i)
            {
                var p = _propertyRateName[i];
                bool ret = ConvertValueForRate(i, p.buffName, p.at,  entityData.battleData, entityData);
				if (ret)
					needSyncHPBar = true;
            }


			int moveSpeed = TableManager.GetValueFromUnionCell(buffData.moveSpeed , level);
			//移动速度处理
			if (moveSpeed != 0)
			{
				if (owner.sgGetCurrentState() == (int)ActionState.AS_RUN)
					//owner.sgSwitchStates(new BeStateData((int)ActionState.AS_RUN));
					owner.RefreshMoveSpeed();
			}


			/*******************************************************************************/
			//增加伤害
			int v = TableManager.GetValueFromUnionCell(buffData.addDamageFix, level);
			if (v > 0)
			{
				entityData.battleData.addDamageFix.Add(new AddDamageInfo(v));
			}

			v = TableManager.GetValueFromUnionCell(buffData.addDamagePercent, level);
			if (v > 0)
			{
				entityData.battleData.addDamagePercent.Add(new AddDamageInfo(v));
			}
			//附加伤害
			v = TableManager.GetValueFromUnionCell(buffData.attachAddDamageFix, level);
			if (v > 0)
			{
				entityData.battleData.attachAddDamageFix.Add(new AddDamageInfo(v));
			}

			v = TableManager.GetValueFromUnionCell(buffData.attachAddDamagePercent, level);
			if (v > 0)
			{
				entityData.battleData.attachAddDamagePercent.Add(new AddDamageInfo(v));
			}

			//减伤
			v = TableManager.GetValueFromUnionCell(buffData.reduceDamageFix, level);
			if (v > 0)
			{
				entityData.battleData.reduceDamageFix.Add(new AddDamageInfo(v, buffData.reduceDamageFixType));
			}
				
			v = TableManager.GetValueFromUnionCell(buffData.reduceDamagePercent, level);
			if (v != 0)
				entityData.battleData.reduceDamagePercent.Add(new AddDamageInfo(v, buffData.reduceDamagePercentType));

            //附加减伤
            v = TableManager.GetValueFromUnionCell(buffData.extrareduceDamgePercent, level);
            if (v != 0)
            {
                entityData.battleData.reduceExtraDamagePercent.Add(new AddDamageInfo(v, buffData.extrareduceDamagePercentType));
            }
            //职业技能物理增伤百分比
            //附加减伤
            v = TableManager.GetValueFromUnionCell(buffData.skilladdDamagePercent, level);
            if (v != 0)
            {
                entityData.battleData.skillAddDamagePercent.Add(new AddDamageInfo(v));
            }
            //职业技能魔法增伤百分比
            v = TableManager.GetValueFromUnionCell(buffData.skilladdMagicDamagePercent, level);
            if (v != 0)
            {
                entityData.battleData.skillAddMagicDamagePercent.Add(new AddDamageInfo(v));
            }
            //反伤
            v = TableManager.GetValueFromUnionCell(buffData.reflectDamageFix, level);
			if (v > 0)
				entityData.battleData.reflectDamageFix.Add(new AddDamageInfo(v, buffData.reflectDamageFixType));
			v = TableManager.GetValueFromUnionCell(buffData.reflectDamagePercent, level);
			if (v > 0)
				entityData.battleData.reflectDamagePercent.Add(new AddDamageInfo(v, buffData.reflectDamagePercentType));

            //抗魔�?
            v = TableManager.GetValueFromUnionCell(buffData.ResistMagic, level);
            if (v != 0)
                ChangeResistMaic(v);

            DealElement();
            DealAbnormal();

            if (needSyncHPBar)
                DoSyncHPBar(owner);
        }
#if ENABLE_PROFILER
        }
#endif
    }

    //属强，属�?
    void DealElement(bool restore=false)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.DealElement"))
        {
#endif
        var entityData = owner.GetEntityData();

        // 附魔元素
        entityData.SetMagicElementTypes(buffData.Elements, !restore);

        int factor = restore ? -1 : 1;

        entityData.battleData.magicElementsAttack[(int)MagicElementType.FIRE] += factor * TableManager.GetValueFromUnionCell(buffData.FireAttack, level);
        entityData.battleData.magicElementsAttack[(int)MagicElementType.ICE] += factor * TableManager.GetValueFromUnionCell(buffData.IceAttack, level);
        entityData.battleData.magicElementsAttack[(int)MagicElementType.LIGHT] += factor * TableManager.GetValueFromUnionCell(buffData.LightAttack, level);
        entityData.battleData.magicElementsAttack[(int)MagicElementType.DARK] += factor * TableManager.GetValueFromUnionCell(buffData.DarkAttack, level);

        entityData.battleData.magicElementsDefence[(int)MagicElementType.FIRE] += factor * TableManager.GetValueFromUnionCell(buffData.FireDefence, level);
        entityData.battleData.magicElementsDefence[(int)MagicElementType.ICE] += factor * TableManager.GetValueFromUnionCell(buffData.IceDefence, level);
        entityData.battleData.magicElementsDefence[(int)MagicElementType.LIGHT] += factor * TableManager.GetValueFromUnionCell(buffData.LightDefence, level);
        entityData.battleData.magicElementsDefence[(int)MagicElementType.DARK] += factor * TableManager.GetValueFromUnionCell(buffData.DarkDefence, level);

#if ENABLE_PROFILER
        }
#endif
    }

    void DealAbnormal(bool restore = false)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.DealAbnormal"))
        {
#endif
        var entityData = owner.GetEntityData();

        int factor = restore ? -1 : 1;


        ProtoTable.UnionCell[] cells = new ProtoTable.UnionCell[]
        {
           buffData.abnormalResist1,
           buffData.abnormalResist2,
           buffData.abnormalResist3,
           buffData.abnormalResist4,
           buffData.abnormalResist5,
           buffData.abnormalResist6,
           buffData.abnormalResist7,
           buffData.abnormalResist8,
           buffData.abnormalResist9,
           buffData.abnormalResist10,
           buffData.abnormalResist11,
           buffData.abnormalResist12,
           buffData.abnormalResist13,
        };

        for(int i=0; i<Global.ABNORMAL_COUNT; ++i)
        {
            int value = TableManager.GetValueFromUnionCell(cells[i], level);
            if (value != 0)
                entityData.battleData.abnormalResists[i] += factor * value;
        }


#if ENABLE_PROFILER
        }
#endif
    }

	bool ConvertValueForRate(int index, string buffProperty, AttributeType at, BattleData battleData,BeEntityData entityData, bool isField = true, bool isRestore = false)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff. ConvertValueForRate"))
        {
#endif
		bool needSyncHPBar = false;
        if (isRestore)
        {
            InitForValueChanged();
			if (valueChanged[index] != 0)
            {
				int savedValue = valueChanged[index];
                if (savedValue != 0)
                {
                    bool ret = false;

					if (buffProperty == "maxHpAddRate")
					{
						entityData.ChangeMaxHp(-savedValue);
						needSyncHPBar = true;
						ret = true;
					}
					else if (buffProperty == "maxMpAddRate")
					{
						battleData.ChangeMaxMP(-savedValue);
						needSyncHPBar = true;
						ret = true;
					}
					else {
						battleData.SetValue(at, -savedValue, true);

					}
                }
            }
        }
        else
        {
			var cellValue = TableManager.GetInstance().GetBuffTablePropertyByName(buffProperty, buffData);
			if (cellValue != null)
            {
                int value = TableManager.GetValueFromUnionCell(cellValue, level);
                if (value != 0)
                {
					int curValue = battleData.GetValue(at);
					int addValue = IntMath.Float2Int(curValue * (value / (float)(GlobalLogic.VALUE_1000)));

                    bool ret = false;

					if (buffProperty == "maxHpAddRate")
					{
						entityData.ChangeMaxHp(addValue);
						needSyncHPBar = true;
						ret = true;
					}
					else if (buffProperty == "maxMpAddRate")
					{
						battleData.ChangeMaxMP(addValue);
						needSyncHPBar = true;
						ret = true;
					}
					else {
						battleData.SetValue(at, addValue, true);
					}

                    InitForValueChanged();
					valueChanged[index] = addValue;
                }
            }
        }
		return needSyncHPBar;
#if ENABLE_PROFILER
        }
#endif
	}


    void RestoreAttribute()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.RestoreAttribute"))
        {
#endif
        if (buffData != null)
        {
			bool needSyncHPBar = false;
            var entityData = owner.GetEntityData();

            for (int i = 0; i <= (int)AttributeType.baseSpr; ++i)
            {
                {
					
					var cellValue = TableManager.GetInstance().GetBuffTableProperty((AttributeType)i, buffData);//(ProtoTable.UnionCell)proInfo.GetGetMethod().Invoke(buffData, null);
					if (cellValue == null)
						continue;
					
                    int value = TableManager.GetValueFromUnionCell(cellValue, level);

					if ((AttributeType)i == AttributeType.maxHp && value != 0)
					{
						entityData.ChangeMaxHp(-value);
						needSyncHPBar = true;
					}
					else if ((AttributeType)i == AttributeType.maxHp && value != 0)
					{
						entityData.battleData.ChangeMaxMP(-value);
						needSyncHPBar = true;
					}
					else
					{
						if (value != 0)
						{
							entityData.SetAttributeValue((AttributeType)i, -value, true);
							if ((AttributeType)i == AttributeType.hpRecover)
								owner.GetEntityData().battleData.hpRecover = owner.GetEntityData().battleData.fHpRecoer;
							else if ((AttributeType)i == AttributeType.mpRecover)
								owner.GetEntityData().battleData.mpRecover = owner.GetEntityData().battleData.fMpRecover;
						}
					}
                }
            }

            int staValue = TableManager.GetValueFromUnionCell(buffData.sta, level);
            if (staValue != 0)
            {
                entityData.battleData.sta -= staValue;
				needSyncHPBar = true;
            }
                

            int sprValue = TableManager.GetValueFromUnionCell(buffData.spr, level);
            if (sprValue != 0)
            {
                entityData.battleData.spr -= sprValue;
				needSyncHPBar = true;
            }


            for (int i = 0; i < _propertyRateName.Length; ++i)
            {
                var p = _propertyRateName[i];
                bool ret = ConvertValueForRate(i, p.buffName, p.at, entityData.battleData, entityData, false, true);
				if (ret)
					needSyncHPBar = true;
            }

			int moveSpeed = TableManager.GetValueFromUnionCell(buffData.moveSpeed , level);
			//移动速度处理
			if (moveSpeed != 0)
			{
				if (owner.sgGetCurrentState() == (int)ActionState.AS_RUN)
					//owner.sgSwitchStates(new BeStateData((int)ActionState.AS_RUN));
					owner.RefreshMoveSpeed();
			}

			//增加伤害
			int v = TableManager.GetValueFromUnionCell(buffData.addDamageFix, level);
			if (v > 0)
			{
                _removeAddDamageList(entityData.battleData.addDamageFix,v);

// 				entityData.battleData.addDamageFix.RemoveAll(item=>{
// 					return item.value == v;
// 				});
			}

			v = TableManager.GetValueFromUnionCell(buffData.addDamagePercent, level);
			if (v > 0)
			{
                   _removeAddDamageList(entityData.battleData.addDamagePercent,v);
//                 entityData.battleData.addDamagePercent.RemoveAll(item=>{
// 					return item.value == v;
// 				});
            }
			//附加伤害 
			v = TableManager.GetValueFromUnionCell(buffData.attachAddDamageFix, level);
			if (v > 0)
			{
                   _removeAddDamageList(entityData.battleData.attachAddDamageFix,v);

//                 entityData.battleData.attachAddDamageFix.RemoveAll(item=>{
// 					return item.value == v;
// 				});
            }

			v = TableManager.GetValueFromUnionCell(buffData.attachAddDamagePercent, level);
			if (v > 0)
			{
                   _removeAddDamageList(entityData.battleData.attachAddDamagePercent,v);
               

//                 entityData.battleData.attachAddDamagePercent.RemoveAll(item=>{
// 					return item.value == v;
// 				});
			}

			//减伤
			v = TableManager.GetValueFromUnionCell(buffData.reduceDamageFix, level);
			if (v > 0)
            {
                   _removeAddDamageList(entityData.battleData.reduceDamageFix, v, buffData.reduceDamageFixType);
//                 entityData.battleData.reduceDamageFix.RemoveAll(item =>
//                 {
//                     return item.value == v && (int)(item.attackType) == buffData.reduceDamageFixType;
//                 });
            }
				
			v = TableManager.GetValueFromUnionCell(buffData.reduceDamagePercent, level);
			if (v != 0)
            {
                 _removeAddDamageList(entityData.battleData.reduceDamagePercent,v, buffData.reduceDamagePercentType);
             

                //                 entityData.battleData.reduceDamagePercent.RemoveAll(item =>
                //                 {
                //                     return item.value == v && (int)(item.attackType) == buffData.reduceDamagePercentType;
                //                 });
            }

            //附加减伤
            v = TableManager.GetValueFromUnionCell(buffData.extrareduceDamgePercent, level);
            if (v != 0)
            {
                _removeAddDamageList(entityData.battleData.reduceExtraDamagePercent, v, buffData.extrareduceDamagePercentType);
            }
            //技能物理增�?
            v = TableManager.GetValueFromUnionCell(buffData.skilladdDamagePercent, level);
            if (v != 0)
            {
                _removeAddDamageList(entityData.battleData.skillAddDamagePercent, v);
            }
            //技能魔法增�?
            v = TableManager.GetValueFromUnionCell(buffData.skilladdMagicDamagePercent, level);
            if (v != 0)
            {
                _removeAddDamageList(entityData.battleData.skillAddMagicDamagePercent, v);
            }

            //反伤
            v = TableManager.GetValueFromUnionCell(buffData.reflectDamageFix, level);
			if (v > 0)
            {
                   _removeAddDamageList(entityData.battleData.reflectDamageFix,v, buffData.reflectDamageFixType);

//                 entityData.battleData.reflectDamageFix.RemoveAll(item =>
//                 {
//                     return item.value == v && (int)(item.attackType) == buffData.reflectDamageFixType;
//                 });
            }
				
			v = TableManager.GetValueFromUnionCell(buffData.reflectDamagePercent, level);
			if (v > 0)
            {
                   _removeAddDamageList(entityData.battleData.reflectDamagePercent,v, buffData.reflectDamagePercentType);

//                 entityData.battleData.reflectDamagePercent.RemoveAll(item =>
//                 {
//                     return item.value == v && (int)(item.attackType) == buffData.reflectDamagePercentType;
//                 });
            }

            //抗魔�?
            v = TableManager.GetValueFromUnionCell(buffData.ResistMagic, level);
            if (v != 0)
                ChangeResistMaic(-v);

            DealElement(true);
            DealAbnormal(true);

            if (needSyncHPBar)
                DoSyncHPBar(owner);
        }
#if ENABLE_PROFILER
        }
#endif
    }

    private void _removeAddDamageList(List<AddDamageInfo> infoList, int value, int type = -1)
    {
        for (int i = 0; i < infoList.Count; i++)
        {
            if (infoList[i].value == value)
            {
                if (type == -1 || (int)infoList[i].attackType == type)
                {
                    infoList.RemoveAt(i);
                    return;
                }
            }
        }
    }

    void DoAttributeAdd()
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.DoAttributeAdd"))
        {
#endif
		if (buffData != null)
		{
			int hpAdd = TableManager.GetValueFromUnionCell(buffData.hp, level);
			if (hpAdd != 0)
			{
				if (hpAdd > 0)
				{
					var eventData = owner.TriggerEventNew(BeEventType.OnBuffHeal, new EventParam() {m_Int = buffData.ID, m_Int2 = hpAdd, m_Obj = this});
					hpAdd = eventData.m_Int2;
                    owner.DoHeal(hpAdd, ShowHPNumber());
				}
                else
                {
                    owner.DoHPChange(hpAdd, ShowHPNumber());
					//owner.TriggerEvent(BeEventType.OnBuffDamage, new object[] {-hpAdd });
                    owner.TriggerEventNew(BeEventType.OnBuffDamage, new EventParam{m_Int = -hpAdd});
                }
			}
			int mpAdd = TableManager.GetValueFromUnionCell(buffData.mp, level);
			if (mpAdd != 0)
			{
				owner.DoMPChange(mpAdd, ShowHPNumber());
			}

			int hpRate = TableManager.GetValueFromUnionCell (buffData.hpRate, level);
			if (hpRate != 0)
			{
				int hpChange = IntMath.Float2Int(owner.GetEntityData ().battleData.maxHp * (long)hpRate / (float)(GlobalLogic.VALUE_1000));


				if (hpRate > 0)
				{
					var eventData = owner.TriggerEventNew(BeEventType.OnBuffHeal, new EventParam() {m_Int = buffData.ID, m_Int2 = hpChange, m_Obj = this});
					hpChange = eventData.m_Int2;
					owner.DoHeal(hpChange, ShowHPNumber());
				}
				else
                {
                    owner.DoHPChange(hpChange, ShowHPNumber());
                    //owner.TriggerEvent(BeEventType.OnBuffDamage, new object[] { -hpChange });
                    owner.TriggerEventNew(BeEventType.OnBuffDamage, new EventParam{m_Int = -hpChange});
                } 
					
			}

			int mpRate = TableManager.GetValueFromUnionCell (buffData.mpRate, level);
			if (mpRate != 0)
			{
				owner.DoMPChange(IntMath.Float2Int(owner.GetEntityData().battleData.maxMp * mpRate / (float)(GlobalLogic.VALUE_1000)), ShowHPNumber());
			}

			int currentHpRate = TableManager.GetValueFromUnionCell(buffData.currentHpRate, level);
			if (currentHpRate != 0)
			{
				int hpChange = IntMath.Float2Int(owner.GetEntityData().GetHP() * (long)currentHpRate/(float)(GlobalLogic.VALUE_1000));
				if (hpRate > 0)
					owner.DoHeal(hpChange, ShowHPNumber());
				else
                {
                    /*
                    扣血最多扣除生命上�?8000*怪物等级
                    */
                    BeActor actor = owner as BeActor;
                    if ((buffData.currentHpRateControl == 1 ||
                         (buffData.currentHpRateControl == 2 && actor != null && actor.attribute != null &&
                         actor.attribute.monsterData != null && actor.attribute.monsterData.MonsterMode == 8
                          && (actor.IsSkillMonster() || actor.IsMonster()))) && owner != null && hpChange < 0)

                    { 
                        int maxChange = -8000 * owner.GetEntityData().level;

                        hpChange = Mathf.Max(hpChange, maxChange);
                    }

                    owner.DoHPChange(hpChange, ShowHPNumber());
                    //owner.TriggerEvent(BeEventType.OnBuffDamage, new object[] { -hpChange });
                    owner.TriggerEventNew(BeEventType.OnBuffDamage, new EventParam{m_Int = -hpChange});
                }
					
			}
            owner.TriggerEventNew(BeEventType.OnBuffHpChange, new EventParam() { m_Int = buffID });
        }
#if ENABLE_PROFILER
        }
#endif
	}


    static string[] skillAtriibuteNames =
    {
        "skill_mpCostReduceRate",
        "skill_cdReduceRate",
        "skill_speedAddFactor",
        "skill_hitRateAdd",
        "skill_criticalHitRateAdd",
        "skill_attackAddRate",
        "skill_hardAddRate"
    };

    void ChangeSkillAttribute()
    {
        if (skillIDs != null)
        {
            for(int i=0; i<skillIDs.Count; ++i)
            {
                var skill = owner.GetSkill(skillIDs[i]);
                if (skill != null)
                {
                    changeOneSkillAttr(skill);
                }
            }
        }
    }

    void ChangeAllSkillAttribute()
    {
        var dic = owner.GetSkills();
        foreach (var skill in dic.Values)
        {
            if (skill != null)
            {
                changeOneSkillAttr(skill);
            }
        }
    }

    void changeOneSkillAttr(BeSkill skill)
    {
        /*

         static string[] skillAtriibuteNames =
    {
        "skill_mpCostReduceRate",
        "skill_cdReduceRate",
        "skill_speedAddFactor",
        "skill_hitRateAdd",
        "skill_criticalHitRateAdd",
        "skill_attackAddRate",
        "skill_hardAddRate"
    };

        */
        /*
        for (int i=0; i<skillAtriibuteNames.Length; ++i)
        {
            string pname = skillAtriibuteNames[i];
            string pname2 = pname.Replace("skill_", "");

            var proInfo = buffData.GetType().GetProperty(pname);
            if (proInfo != null)
            {
                var cellValue = (ProtoTable.UnionCell)proInfo.GetGetMethod().Invoke(buffData, null);
                int value = TableManager.GetValueFromUnionCell(cellValue, level);
                if (value != 0) {
					Utility.SetValue2(typeof(BeSkill), skill, pname2, (float)(value / (float)(GlobalLogic.VALUE_1000)), true);
                 }
            }
        }*/

        {
            int value = TableManager.GetValueFromUnionCell(buffData.skill_mpCostReduceRate, level);
            if (value != 0)
            {
                skill.mpCostReduceRate += value;
            }

            value = TableManager.GetValueFromUnionCell(buffData.skill_cdReduceRate, level);
            if (value != 0)
            {
                skill.cdReduceRate += value;
            }

            value = TableManager.GetValueFromUnionCell(buffData.skill_cdReduceValue, level);
            if (value != 0)
            {
                skill.cdReduceValue += value;
            }

            value = TableManager.GetValueFromUnionCell(buffData.skill_chargeReduceRate, level);
            if (value != 0)
            {
                skill.chargeReduceRate += value;
            }

            value = TableManager.GetValueFromUnionCell(buffData.skill_speedAddFactor, level);
            if (value != 0)
            {
                skill.speedAddRate += value;
            }

            value = TableManager.GetValueFromUnionCell(buffData.skill_hitRateAdd, level);
            if (value != 0)
            {
                skill.hitRateAdd += value;
            }

            value = TableManager.GetValueFromUnionCell(buffData.skill_criticalHitRateAdd, level);
            if (value != 0)
            {
                skill.criticalHitRateAdd += value;
            }

            value = TableManager.GetValueFromUnionCell(buffData.skill_attackAddRate, level);
            if (value != 0)
            {
                skill.attackAddRate += value;
            }

            value = TableManager.GetValueFromUnionCell(buffData.skill_hardAddRate, level);
            if (value != 0)
            {
                skill.hardAddRate += value;
            }

        }

        int levelInfo = TableManager.GetValueFromUnionCell(buffData.level, level);
        if (levelInfo != 0)
        {
            int oldLevel = skill.level;
            skill.level += levelInfo;
            if (m_AddSkillLevel == null)
            {
                m_AddSkillLevel = new Dictionary<int, int>();
            }
            if (!m_AddSkillLevel.ContainsKey(skill.skillID))
            {
                m_AddSkillLevel.Add(skill.skillID, skill.level - oldLevel);
            }
            else
            {
                m_AddSkillLevel[skill.skillID] = skill.level - oldLevel;
            }
        }
		
		int attackAdd = TableManager.GetValueFromUnionCell(buffData.skill_attackAdd, level);
		if (attackAdd != 0)
			skill.attackAdd += attackAdd;
		int attackAddFix = TableManager.GetValueFromUnionCell(buffData.skill_attackAddFix, level);
		if (attackAddFix != 0)
			skill.attackAddFix += attackAddFix;
		
		owner.TriggerEventNew(BeEventType.OnBuffAddSkillAttr, new EventParam(){m_Obj = skill, m_Obj2 = this});
    }

    void RestoreSkillAttribute()
    {
        if (skillIDs != null)
        {
            for (int i = 0; i < skillIDs.Count; ++i)
            {
                var skill = owner.GetSkill(skillIDs[i]);
                if (skill != null)
                {
                    RestoreOneSkillAttribute(skill);
                }
            }
        }
    }

    void RestoreAllSkillAttribute()
    {
        var dic = owner.GetSkills();
        foreach (var skill in dic.Values)
        {
            if (skill != null)
            {
                RestoreOneSkillAttribute(skill);
            }
        }
    }

    void RestoreOneSkillAttribute(BeSkill skill)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.RestoreOneSkillAttribute"))
        {
#endif
        /*
        for (int i = 0; i < skillAtriibuteNames.Length; ++i)
        {
            string pname = skillAtriibuteNames[i];
            string pname2 = pname.Replace("skill_", "");

            var proInfo = buffData.GetType().GetProperty(pname);
            if (proInfo != null)
            {
                var cellValue = (ProtoTable.UnionCell)proInfo.GetGetMethod().Invoke(buffData, null);
                int value = TableManager.GetValueFromUnionCell(cellValue, level);
                if (value != 0)
                {
					Utility.SetValue2(typeof(BeSkill), skill, pname2, -(float)(value / (float)(GlobalLogic.VALUE_1000)), true);
                }
            }
        }*/

        {
            int value = TableManager.GetValueFromUnionCell(buffData.skill_mpCostReduceRate, level);
            if (value != 0)
            {
                skill.mpCostReduceRate -= value;
            }

            value = TableManager.GetValueFromUnionCell(buffData.skill_cdReduceRate, level);
            if (value != 0)
            {
                skill.cdReduceRate -= value;
            }

            value = TableManager.GetValueFromUnionCell(buffData.skill_cdReduceValue, level);
            if (value != 0)
            {
                skill.cdReduceValue -= value;
            }

            value = TableManager.GetValueFromUnionCell(buffData.skill_chargeReduceRate, level);
            if (value != 0)
            {
                skill.chargeReduceRate -= value;
            }

            value = TableManager.GetValueFromUnionCell(buffData.skill_speedAddFactor, level);
            if (value != 0)
            {
                skill.speedAddRate -= value;
            }

            value = TableManager.GetValueFromUnionCell(buffData.skill_hitRateAdd, level);
            if (value != 0)
            {
                skill.hitRateAdd -= value;
            }

            value = TableManager.GetValueFromUnionCell(buffData.skill_criticalHitRateAdd, level);
            if (value != 0)
            {
                skill.criticalHitRateAdd -= value;
            }

            value = TableManager.GetValueFromUnionCell(buffData.skill_attackAddRate, level);
            if (value != 0)
            {
                skill.attackAddRate -= value;
            }

            value = TableManager.GetValueFromUnionCell(buffData.skill_hardAddRate, level);
            if (value != 0)
            {
                skill.hardAddRate -= value;
            }

        }


        int levelInfo = TableManager.GetValueFromUnionCell(buffData.level, level);
        if (levelInfo != 0)
        {
            if (m_AddSkillLevel == null)
            {
                m_AddSkillLevel = new Dictionary<int, int>();
            }
            if (m_AddSkillLevel.ContainsKey(skill.skillID))
            {
                skill.level -= m_AddSkillLevel[skill.skillID];
                m_AddSkillLevel[skill.skillID] = 0;
            }
        }

        int attackAdd = TableManager.GetValueFromUnionCell(buffData.skill_attackAdd, level);
		if (attackAdd != 0)
			skill.attackAdd -= attackAdd;
		int attackAddFix = TableManager.GetValueFromUnionCell(buffData.skill_attackAddFix, level);
		if (attackAddFix != 0)
			skill.attackAddFix -= attackAddFix;
		
		owner.TriggerEventNew(BeEventType.OnBuffRemoveSkillAttr, new EventParam(){m_Obj = skill, m_Obj2 = this});
#if ENABLE_PROFILER
        }
#endif
    } 

	public void DoUseMechanism()
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.DoUseMechanism"))
        {
#endif  
        InitForMechanismList();
		for(int i=0; i<buffData.MechanismID.Count; ++i)
		{
			int mid = buffData.MechanismID[i];
			if (mid > 0)
			{
                BeMechanism mechanism = owner.AddMechanism(mid, level,MechanismSourceType.NONE,this);
                //mechanismList.Add(mechanism);
                if (mechanism != null)
                    mechanismPIDList.Add(mechanism.PID);
            }
		}
#if ENABLE_PROFILER
        }
#endif
	}

	public void RemoveMechanism()
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.RemoveMechanism"))
        {
#endif
        //只移除自己添加的机制
        //for(int i=0; i<buffData.MechanismID.Count; ++i)
        //{
        //    int mid = buffData.MechanismID[i];
        //    if (mid > 0)
        //    {
        //        owner.RemoveMechanism(mid);
        //    }
        //}
        // if (mechanismList != null)
        // {
        //     for(int i=0;i< mechanismList.Count; i++)
        //     {
        //         BeMechanism mechanism = mechanismList[i];
        //         owner.RemoveMechanism(mechanism);
        //     }
        // }

        if (mechanismPIDList != null)
        {
            // foreach(var mpid in mechanismPIDList)
            //     owner.RemoveMechanismByPID(mpid);
            // marked by ckm
            for (int i = 0; i < mechanismPIDList.Count; i++)
            {
                int mpid = mechanismPIDList[i];
                owner.RemoveMechanismByPID(mpid);
            }
        }
        
#if ENABLE_PROFILER
        }
#endif
	}

    public void GetMechanismList(List<BeMechanism> mechanismList)
    {
        if (mechanismList == null || mechanismPIDList == null)
            return;
        if (mechanismPIDList.Count < 0)
            return;

        mechanismList.Clear();
        
        // foreach(var mpid in mechanismPIDList)
        // {
        //     var mech = owner.GetMechanismByPID(mpid);
        //     if (mech != null)
        //         mechanismList.Add(mech);
        // }
        // marked by ckm
        for (int i = 0; i < mechanismPIDList.Count; i++)
        {
            int mpid = mechanismPIDList[i];
            var mech = owner.GetMechanismByPID(mpid);
            if (mech != null)
            {
                mechanismList.Add(mech);
            }
        }
    }
	public void DoUseSkill()
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.DoUseSkill"))
        {
#endif
		for(int i=0; i<buffData.UseSkillIDs.Count; ++i)
		{
			int skillID = buffData.UseSkillIDs[i];
			if (skillID > 0)
			{
				if (owner.CanUseSkill(skillID))
				{
					//owner.delayCaller.DelayCall(100, ()=>{
						owner.UseSkill(skillID, true);
					//});
				}
			}
		}
#if ENABLE_PROFILER
        }
#endif
	}

	public void DoTriggerOtherBuffInfo()
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.DoTriggerOtherBuffInfo"))
        {
#endif
        if (buffData != null)
        {
            for (int i = 0; i < buffData.TriggerBuffInfoIDs.Count; ++i)
            {
                int buffInfoID = buffData.TriggerBuffInfoIDs[i];
                BuffInfoData info = new BuffInfoData(buffInfoID, level);

                if (info.data == null) continue;

                if (info.data.BuffLevel.valueType != ProtoTable.UnionCellType.union_fix && info.data.BuffLevel.valueType!= ProtoTable.UnionCellType.union_helper)
                {
                    info.level = TableManager.GetValueFromUnionCell(info.data.BuffLevel, level);
                    info.abnormalLevel = info.level;
                }
                else if (TableManager.GetValueFromUnionCell(info.data.BuffLevel, level) == 0)
                {
                    info.level = level;
                    info.abnormalLevel = info.level;
                }

                if (info.condition <= BuffCondition.NONE)
                {
                    owner.buffController.TriggerBuffInfo(info);
                }

                else
                    owner.buffController.AddTriggerBuff(info);
            }
        }
#if ENABLE_PROFILER
        }
#endif
    }

	public void DoBuffDispel()
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.DoBuffDispel"))
        {
#endif
		if (buffData != null)
		{
			owner.buffController.DispelBuff((BuffWorkType)buffData.DispelBuffType);
		}
#if ENABLE_PROFILER
        }
#endif
	}

	public void DoSummon()
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.DoSummon"))
        {
#endif
		if (buffData != null)
		{
            /*int[] summonNumArray = new int[1];
            summonNumArray[0] = buffData.summon_num;
            owner.TriggerEvent(BeEventType.onChangeSummonNum, new object[] { -1, buffID, summonNumArray});
            var summonNum = summonNumArray[0];*/

            /*int[] summonNumLimitArray = new int[1];
            summonNumLimitArray[0] = buffData.summon_numLimit;
            owner.TriggerEvent(BeEventType.onChangeSummonNumLimit, new object[] { -1, buffID, summonNumLimitArray });
            var summonNumLimit = summonNumLimitArray[0];*/
            
            var eventData = owner.TriggerEventNew(BeEventType.onChangeSummonNumLimit, new EventParam(){m_Int = -1, m_Int2 = buffID, m_Int3 = buffData.summon_numLimit, m_Int4 = buffData.summon_num});
            var summonNumLimit = eventData.m_Int3;
            var summonNum = eventData.m_Int4;
            
            owner.DoSummon(
                buffData.summon_monsterID,
                TableManager.GetValueFromUnionCell(buffData.summon_monsterLevel,level),
                (ProtoTable.EffectTable.eSummonPosType)buffData.summon_posType,
                buffData.summon_posType2,
                summonNum,
                summonNumLimit,
				0,0,0,buffData.summon_relation>0,0,
				buffData.summon_existTime,
				null,
				(SummonDisplayType)buffData.summon_display
			);
		}
#if ENABLE_PROFILER
        }
#endif
	}

	public void ResetDuration(int d)
    {
        timeAcc = 0;
		duration = d;
        if (buffEffect != null)
		    buffEffect.ResetDuration(d);
    }

	public void DoGenEntity()
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.DoGenEntity"))
        {
#endif
		if (buffData != null)
		{
			for(int i=0; i<buffData.summon_entity.Count; ++i)
			{
				int entityId = buffData.summon_entity[i];
				if (entityId > 0)
					owner.AddEntity(entityId, owner.GetPosition());
			}
		}
#if ENABLE_PROFILER
        }
#endif
	}

	public void DoDuplicate()
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.DoDuplicate"))
        {
#endif
		if (buffData != null)
		{
			
			if (buffData.duplicate_percent > 0)
			{
				int maxNum = buffData.duplicate_max;
				if (maxNum > 0)
					maxNum++;
				owner.CurrentBeScene.DuplicateMonster(
					owner, 
					new VFactor(buffData.duplicate_percent,GlobalLogic.VALUE_1000), 
					maxNum);
			}

		}
#if ENABLE_PROFILER
        }
#endif
	}

	public void RefreshDuration(int d)
	{
        //Logger.LogErrorFormat("RefreshDuration buffid {0} new time {1}", buffID, d);
        /*int[] dArray = new int[1];
        dArray[0] = d;
        owner.TriggerEvent(BeEventType.onBuffRefresh, new object[] { buffID, dArray });
		duration = dArray[0];
        timeAcc = 0;*/
        // marked by ckm
        if (owner != null)
        {
            var ret = owner.TriggerEventNew(BeEventType.onBuffRefresh, new EventParam{m_Int = buffID, m_Int2 = d});
            duration = ret.m_Int2;
            timeAcc = 0;
        }
	}

    public void CopyRunTime(BeBuff curBuff)
    {
        if (curBuff == null)
            return;
        timeAcc = curBuff.runTime;
    }

	public void OverlayDamage(int damage, int duration = 0)
	{
        //Logger.LogErrorFormat ("OverlayDamage buffID {0} new time {1}", buffID, damage);

        int abnormalDamage = GetAbnromalDamage(damage, duration);

        overlayCount++;
        totalDamage += abnormalDamage;


        ForceDoWork();
	}

    public void DoWork()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.DoWork"))
        {
#endif
        ShowEffect();
        ShowBuffEffectName();
		if (buffData != null && buffData.IsQuickPressSupport == 1)
			StartQuickPress();

        ChangeTargetState();
        ChangeTargetAbility();
        ChangeTargetAction();

        if (buffType >= BuffType.ATTRIBUTE_CHANGE)
        {
            ChangeAttribute();
            ChangeAIAttribute();
        }
		else if (buffType == BuffType.ATTRIBUTE_ADD)
		{
			DoAttributeAdd();
		}
        else if (buffType == BuffType.SKILL_BUFF)
        {
            ChangeSkillAttribute();
        }
        else if (buffType == BuffType.ALL_SKILL_BUFF)
        {
            ChangeAllSkillAttribute();
        }
		else if (buffType == BuffType.SUMMON)
		{
			DoSummon();
			DoGenEntity();
			DoDuplicate();
		}
		else if (buffType == BuffType.BUFF_DISPEL)
		{
			DoBuffDispel();
		}
		else if (buffType == BuffType.TRIGGER_BUFFINFO)
		{
			DoTriggerOtherBuffInfo();
		}
		else if (buffType == BuffType.USE_SKILL)
		{
			DoUseSkill();
		}
		else if (buffType == BuffType.USE_MECHANISM)
		{
			DoUseMechanism();
		}
#if ENABLE_PROFILER
        }
#endif
    }

	public virtual void DoWorkForInterval()
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.DoWorkForInterval"))
        {
#endif
		//do hurt
		if (!owner.IsDead())
		{
            attackCount++;

			if (buffType == BuffType.ATTRIBUTE_ADD)
			{
				DoAttributeAdd();
				if (buffEffect != null) buffEffect.ShowHurtEffect();
			}
			else if (buffType == BuffType.SUMMON)
			{
				DoSummon();
				DoGenEntity();
				DoDuplicate();
			}
			else if (buffType == BuffType.TRIGGER_BUFFINFO)
			{
				DoTriggerOtherBuffInfo();
			}
			else 
			{
                if (buffWorkType == BuffWorkType.DEBUFF && buffData.Overlay != (int)BuffOverlayType.OVERLAY_ALONE)
                {
                    if (!owner.IsDead())
                    {
                        if (totalDamage == -1)
                        {
                            if (buffAttack > 0)
                            {
                                totalDamage = GetAbnromalDamage(buffAttack, duration);
                            }
                        }
                        DoBuffAttack(totalDamage);

                    }
                }
            }

            if (attackCount >= totalAttackCount)
            {
                Finish();
            }
		}
#if ENABLE_PROFILER
        }
#endif
	}

    public void Update(int deltaTime)
    {
        if (state.IsRunning())
        {
            timeAcc += deltaTime;
            
			if (timeType == BuffTimeType.INTERVAL)
				UpdateForInterval(deltaTime);

            if (timeAcc > duration)
            {
                Finish();
                return;
            }

            OnUpdate(deltaTime);                   
        }
    }

	public float GetProcess()
	{
		return timeAcc / (float)duration;
	}

	public void UpdateForInterval(int deltaTime)
	{
		intervalAcc += deltaTime;
		if (timeInterval < intervalAcc)
		{
			intervalAcc -= timeInterval;
			DoWorkForInterval();
		}
	}

    public int GetLeftTime()
    {
        if (duration == -1)
        {
            return -1;
        }
        else
        {
            return Mathf.Max(0, duration - timeAcc);
        }

    }

	public void ForceDoWork()
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.ForceDoWork"))
        {
#endif
		intervalAcc = 0;

        //感电Buff不进行伤害叠�?
        if (null != buffData.StateChange && !(BeUtility.HaveBuffState(buffData.StateChange, BeBuffStateType.FLASH)))
        {
            DoWorkForInterval();
        }
#if ENABLE_PROFILER
        }
#endif
	}

    public void Finish()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.Finish"))
        {
#endif
		TriggerEventNew(BeEventType.onBuffFinish, new EventParam{m_Int = buffID});

		if (owner != null)
			owner.TriggerEventNew(BeEventType.onBuffFinish, new EventParam{m_Int = buffID});

        Cancel();
#if ENABLE_PROFILER
        }
#endif
    }

    public void Cancel()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.Cancel"))
        {
#endif

        if (state.IsDead())
            return;
        state.SetDead();
        RemoveEffect();
        RemoveBuffEffectName();
		if (buffData.IsQuickPressSupport == 1)
			StopQuickPress();

        RestoreTargetAbility();
        RestoreTargetState();
        RestoreTargetAction();
        if (buffType >= BuffType.ATTRIBUTE_CHANGE)
        {
            RestoreAttribute();
            RestoreAIAttribute();
        }

        if (owner != null && isShowSpell)
        {
            owner.StopSpellBar(eDungeonCharactorBar.Buff);
            owner.RemoveStateBar();
        }

        if (buffType == BuffType.SKILL_BUFF)
        {
            RestoreSkillAttribute();
        }

        if (buffType == BuffType.ALL_SKILL_BUFF)
        {
            RestoreAllSkillAttribute();
        }

		if (buffType == BuffType.USE_MECHANISM)
		{
			RemoveMechanism();
		}

		Logger.LogWarningFormat("buff:{0} finish or dispose time:{1}", buffData.Name, Time.realtimeSinceStartup);

		OnFinish();
        if (buffID == (int)GlobalBuff.GRAB)
        {
            owner.grabController.EndGrab();
        }

        if (owner != null)
        {
	        owner.TriggerEventNew(BeEventType.onRemoveBuff, new EventParam(){m_Int = buffID, m_Obj = this});
	        if (owner.CurrentBeScene != null)
		        owner.CurrentBeScene.TriggerEventNew(BeEventSceneType.OnRemoveBuff, new EventParam() {m_Obj = this, m_Obj2 = owner});
        }
			//owner.TriggerEvent(BeEventType.onRemoveBuff, new object[]{buffID, this});
			
        //Buff结束时移除BuffInfo的Trigger
        // if (buffData.ExitRemoveTrigger == 1 && buffData.TriggerBuffInfoIDs.Count > 0)
        // marked by ckm
        if (buffData != null && buffData.ExitRemoveTrigger == 1 && buffData.TriggerBuffInfoIDs != null && buffData.TriggerBuffInfoIDs.Count > 0)
        {
            for (int i = 0; i < buffData.TriggerBuffInfoIDs.Count; i++)
            {
                owner.buffController.RemoveTriggerBuff(buffData.TriggerBuffInfoIDs[i]);
            }
        }

		DeInit();
#if ENABLE_PROFILER
        }
#endif
    }

    public bool CanRemove()
    {
        return state.IsDead();
    }

    public bool CanDispel()
    {
        return dispel;
    }

    public bool IsPassive()
    {
        return passive;
    }

	public void ReduceDuration(int value, bool isPercent=true)
	{
		int reduceValue = value;
		if (isPercent)
		{
			reduceValue = IntMath.Float2Int(duration * (value / (float)(GlobalLogic.VALUE_1000))) ;
		}

        //Logger.LogErrorFormat("reduceDuration timeAcc:{0} reduceValue:{1} duration:{2}", timeAcc, reduceValue, duration);

        if (reduceValue > 0)
            isReduceByQuickPress = true;

        timeAcc += reduceValue;

	}

	public void StartQuickPress()
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.StartQuickPress"))
        {
#endif
		if (owner != null && owner.isMainActor)
		{
			BeActor actor = owner as BeActor;
			actor.StartPressCount(BeActor.QuickPressType.BUFF, this);
		}
#if ENABLE_PROFILER
        }
#endif
	}

	public void StopQuickPress()
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.StopQuickPress"))
        {
#endif
		if (owner != null && owner.isMainActor)
		{
			BeActor actor = owner as BeActor;
			actor.EndPressCount();
		}
#if ENABLE_PROFILER
        }
#endif
	}


    public void ShowEffect()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.ShowEffect"))
        {
#endif
        if (buffEffect != null)
        {
            buffEffect.ShowEffect(owner);
        }
#if ENABLE_PROFILER
        }
#endif
    }
    public void ShowBuffEffectName()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.ShowBuffEffectName"))
        {
#endif
#if !LOGIC_SERVER
        if (owner != null && owner.m_pkGeActor!=null&&buffData!=null)
        {
            if (!owner.m_pkGeActor.hpBarBuffEffectNameList.Contains(buffData.HpBarName))
            {
                owner.m_pkGeActor.hpBarBuffEffectNameList.Add(buffData.HpBarName);
            }
        }
#endif
#if ENABLE_PROFILER
        }
#endif
    }
    public void RemoveBuffEffectName()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.RemoveBuffEffectName"))
        {
#endif
#if !LOGIC_SERVER
        if (owner != null && owner.m_pkGeActor != null && buffData != null)
        {
            if (owner.m_pkGeActor.hpBarBuffEffectNameList.Contains(buffData.HpBarName))
            {
                owner.m_pkGeActor.hpBarBuffEffectNameList.Remove(buffData.HpBarName);
            }
        }
#endif
#if ENABLE_PROFILER
        }
#endif
    }

    public void HideEffect()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.HideEffect"))
        {
#endif
        if (buffEffect != null)
        {
            buffEffect.HideEffect();
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public void ResetEffectElapsedTime()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.ResetEffectElapsedTime"))
        {
#endif
        if (buffEffect == null)
            return;
        if (buffEffect.geEffect != null && !buffEffect.geEffect.IsDead())
        {
            buffEffect.ResetElapsedTime();
        }
        else
        {
            //Buff特效已经标识为死亡时重新播放特效
            buffEffect.ShowEffect(owner);
        }
#if ENABLE_PROFILER
        }
#endif
    }


    public void RemoveEffect()
    {
        if (buffEffect != null)
        {
            buffEffect.RemoveEffect(owner);
        }
    }

	public bool ShowHPNumber()
	{
		if (buffEffect != null)
			return buffEffect.showHPNumber;
		return true;
	}
/*
    public BeEventHandle RegisterEvent(BeEventType eventKey, BeEventHandle.Del del)
    {
        BeEventHandle handler = null;
        if (eventProcessor != null)
            handler = eventProcessor.AddEventHandler((int)eventKey, del);

        return handler;
    }

    public void RemoveEvent(BeEventHandle handler)
    {
        if (eventProcessor != null)
        {
            handler.Remove();
        }
    }

    public void TriggerEvent(BeEventType eventKey, object[] args = null)
    {
        if (eventProcessor != null)
        {
            eventProcessor.HandleEvent((int)eventKey, args);
        }
    }
*/
    #region 新的事件管理机制
    /// <summary>
    /// 注册事件
    /// </summary>
    /// <param name="type">事件类型</param>
    /// <param name="function">事件回调函数</param>
    public BeEvent.BeEventHandleNew RegisterEventNew(BeEventType eventType, GameClient.BeEvent.BeEventHandleNew.Function function)
    {
        if (m_EventManager == null)
        {
            m_EventManager = new GameClient.BeEventManager(-PID);
        }
        return m_EventManager.RegisterEvent((int)eventType, function);
    }
    
    /// <summary>
    /// 触发事件(New)
    /// 使触发事件调用那边的代码更简�? 更容易使�?
    /// </summary>
    /// 使用示例
    public EventParam TriggerEventNew(BeEventType eventType, EventParam param = new EventParam())
    {
	    if (m_EventManager == null)
		    return param;
	    return m_EventManager.TriggerEvent((int)eventType, param);
    }

    /// <summary>
    /// 清除所有事�?
    /// </summary>
    public void ClearEventAllNew()
    {
        if (m_EventManager == null)
            return;
        m_EventManager.ClearAll();
    }
    #endregion

    public GeEffectEx GetEffectEx()
    {
        if (buffEffect == null)
            return null;

        return buffEffect.geEffect;
    }

    protected void DoBuffAttack(int damage)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.DoBuffAttack"))
        {
#endif
        bool forceBuffHurt = false;
        //出血 灼烧 中毒在起身无敌状态下也能持续造成伤害
        if (buffType >= BuffType.BLEEDING && buffType <= BuffType.POISON && owner.buffController.HasBuffByID((int)GlobalBuff.INVINCIBLE_GET_UP) != null)   
            forceBuffHurt = true;
        //对异常Buff伤害添加保护
        if (owner.buffController.IsAbnormalBuff(buffType) && damage <= 0)
            return;
        owner.DoHurt(damage, null, GameClient.HitTextType.BUFF_HURT,releaser,GameClient.HitTextType.NORMAL,forceBuffHurt, HitDamageType.AbnormalBuff);
        if (buffEffect != null) buffEffect.ShowHurtEffect();
        if (owner.IsDead() && !owner.IsInPassiveState())
            owner.DoDead();
#if ENABLE_PROFILER
        }
#endif
    }

    public virtual int GetAloneAbnormalDamage()
    {
        return GetAbnromalDamage(buffAttack, duration);
    }

//改变自己和召唤兽的抗魔�?
    protected void ChangeResistMaic(int value)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.ChangeResistMaic"))
        {
#endif
        owner.GetEntityData().battleData.resistMagic += value;
        owner.GetEntityData().ChangeMaxHpByResist();
#if !LOGIC_SERVER
        if (owner.m_pkGeActor != null && owner.m_pkGeActor.mCurHpBar != null)
            owner.m_pkGeActor.mCurHpBar.InitResistMagic(owner.attribute.GetResistMagic(), owner);
        DoSyncHPBar(owner);
#endif

        //只有当自己是玩家时才去该变召唤兽
        if (owner.IsMonster())
            return;
        if (owner.CurrentBeScene == null)
            return;
        List<BeActor> summonList = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindSummonMonster(summonList, owner);
        for(int i = 0; i < summonList.Count; i++)
        {
            summonList[i].GetEntityData().battleData.resistMagic += value;
            summonList[i].GetEntityData().ChangeMaxHpByResist();
            owner.CurrentBeScene.AdjustSummonMonsterAttribute(owner, summonList[i]);
            DoSyncHPBar(summonList[i]);
        }
        GamePool.ListPool<BeActor>.Release(summonList); 
#if ENABLE_PROFILER
        }
#endif
    }

    /// <summary>
    /// Buff拥有者复活时消息函数
    /// </summary>
    public void OnOwnerReborn()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeBuff.OnOwnerReborn"))
        {
#endif
        if (duration == int.MaxValue)
        {
            ShowEffect();
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public virtual void OnStart() { }
    public virtual void OnInit() { }
    public virtual void OnUpdate(int delta){}
    public virtual void OnFinish() {}
}
