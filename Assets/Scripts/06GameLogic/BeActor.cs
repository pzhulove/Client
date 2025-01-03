
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using GameClient;
using System;
using System.Reflection;

public struct AccompanyData
{
	public int id;
	public int level;
	public int skillID;
}

/// <summary>
/// 伤害类型
/// </summary>
public enum HitDamageType
{
	Normal,			// 普通
	AbnormalBuff,	// 异常buff伤害
}
	
public class PetData
{
	public int id;
	public int level;
	public int skillID;
	public int hunger;

	public List<BuffInfoData> buffs;
}
	

public enum SummonDisplayType
{
	NONE = 0,
	FAZHEN
}

public enum MechanismSetType
{
    NO_NEED,
    SET_TRUE,
    SET_FALSE
}

public class SpellBar
{
	public eDungeonCharactorBar type;
	public int acc = 0;
	public int duration = 0;
	public bool active = false;
	public bool autodelete = true;
	public bool reverse = false;
	public bool autoAcc = true;
	public bool reverseAutoAcc = true;
    public bool alwaysRefreshUI = false;
    public ComDungeonCharactorHeadBar com;
}

public partial class BeActor : BeEntity {

    string name;
    protected bool m_bInRunMode;
    protected bool forceRunMode = false;

    public int jumpAttackUseCount = 0;

    public BeBuffManager buffController = null;
    public BeProtect protectManager = null;
    public BeActorData actorData = null;
    public SkillDamageManager skillDamageManager = null;
    public BeGrabManager grabController = null;
    public BeSkillManager skillController = null;

    // dd: tmp add the profession ID
    public int professionID;

    public int rangeId;
    //召唤分组
    private CrypticInt32[] groupSummonNum = new CrypticInt32[10];

    protected readonly List<BeMechanism> mechanismList = new List<BeMechanism>();

    readonly private List<BeMechanism> phaseDeleteMechanismList = new List<BeMechanism>();
    readonly private List<BeMechanism> finishDeleteMechanismList = new List<BeMechanism>();
    

    /// <summary>
    /// 该变量弃用 不允许再使用 后期改用从函数中获取
    /// </summary>
    public List<BeMechanism> MechanismList
    {
        get { return mechanismList; }
    }

    //抓取到的列表
    public List<BeActor> GrapedActorList
    {
        get { return grabController.grabbedActorList; }
    }   

    public bool grabPos
    {
        get { return grabController.grabPos; }
        set { grabController.grabPos = value; }
    }   
    

    protected ButtonState attackButtonState = ButtonState.NONE;//1按下 2松开
    protected bool aiAttackNeedCheck = false;   //释放技能前需要检测AI目标是否处于浮空状态

    public bool isMainActor = false;
    public bool isLocalActor = false;
    protected bool isCancelSkill = false;

    private List<SpellBar> mBars = new List<SpellBar>();

    public IList<int> attackSpeeches = null;
    public IList<int> walkSpeeches = null;
    public IList<int> birthSpeeches = null;

    private bool isDunFu = false;
    public bool isInDunfu { get { return isDunFu; } }
	private int DunFuTimeout = 0;
	private int DunFuTimeAcc = 0;
    protected int defaultWeaponType;
	protected int defaultWeaponTag;
	protected bool actorNeedCost = true;
    public bool recieveConfigCmd = false;//是否已经收到DoublePressConfigCommand
    //全部buff提升的列表，key为类型
    protected Dictionary<int, BuffInfoData> buffEnhanceList = new Dictionary<int, BuffInfoData>();

	protected bool isRecordPress = false;
	protected int pressCount = 0;
	protected GeEffectEx recordPressEffect = null;
	//protected BeBuff thisBuff = null;
    protected int thisBuffPID = 0;
	protected BeActor thisActor = null;

    public bool isSpecialMonster = false;

    public IBeEventHandle winHandler = null;

	bool canWalkShootTurn = true;

    // 怪物信息，变身使用
    UnitTable lastTimeMonsterData = null;

    private static List<int> LiGuiSkillList = new List<int>() {1901,1902,1903,1904 };
    private static List<int> NormalSkillList = new List<int>() { 1500,1501,1502};
    private static List<int> AttackComboSkillList = new List<int>() { 1506, 1507, 1508,1522,1523,1913,1914,1915,1916,1917 };
    protected List<int> recordedEquipMechanismIDs = new List<int>();
    private int pressDunfuDurTime = -1;
    private List<ItemProperty> equips = null;//身上的所有装备
    ItemProperty suitProperty = null;   //套装
    ItemProperty masterProperty = null; //护甲精通
    public int dungeonRebornCount = 0;                                     //地下城复活次数
    public bool isPkRobot = false;                                         //是否是决斗场AI

    private IBeSkillSkipToNextPhase _skipToNextPhase;
    public IBeSkillSkipToNextPhase SkipToNextPhase
    {
        get { return _skipToNextPhase; }
        set { _skipToNextPhase = value; }
    }

    public float geLastTopZ
	{
		set{
			m_geLastTopZ = value;
		}
		get {
			return m_geLastTopZ;
		}
	}
	protected float m_geLastTopZ;

	public bool inBossRange
	{
		set {
			m_inBossRange = value;
		}
		get {
			return m_inBossRange;
		}
	}
	protected bool m_inBossRange = false;
    protected bool m_isInDeadProtect = false;
    protected int m_deadProtectDurTime = 0;

	public enum QuickPressType
	{
		BUFF = 0,
        GRAB
	}

	protected QuickPressType quickPressType;

	//public bool hasAccompany;
	public AccompanyData accompanyData
	{
		set {
			m_accompanyData = value;
		}
		get {
			return m_accompanyData;
		}
	}
	protected AccompanyData m_accompanyData;

	public PetData petData
	{
		set {
			m_petData = value;
		}
		get {
			return m_petData;
		}
	}

	protected PetData m_petData;

	public BeActor pet = null;
    public GeActorEx spirit = null;

	public BeEvent.BeEventHandleNew summonNumHandle = null;

	public Dictionary<int, BeSkill> GetSkills()
	{
		return skillController.GetSkills();
	}

    // public int GetPressCount()
    // {
    //     return pressCount;
    // }
    public BeActor(int iResID, int iCamp, int iID): base(iResID, iCamp, iID)
    {
        m_bInRunMode = false;
        restrainPosition = true;

        buffController = new BeBuffManager(this);
		protectManager = new BeProtect(this);
        actorData = new BeActorData(this);
        skillDamageManager = new SkillDamageManager(this);
        grabController = new BeGrabManager(this);
        skillController = new BeSkillManager(this);

        if (actorData != null)
        {
            actorData.InitData(this);
        }

		//groupSummonNum
		for(int i=0; i<groupSummonNum.Length; ++i)
		{
			groupSummonNum[i] = 0;
		}
    }

	public /*virtual*/ void Create(int iDelaytime = 0, bool useCube=false)
    {
        BeActorStateGraph pkSG = new BeActorStateGraph();
        pkSG.InitStatesGraph();
        pkSG.pkActor = this;
        pkSG.m_pkEntity = this;

        //在这里已经创建好GeActor
        base.Create(pkSG, (int)ActionState.AS_BIRTH, false, true, useCube);

        name = GetName();

		delayCaller.DelayCall(GlobalLogic.VALUE_100, ()=>{
			SetBlockLayer(true);	
		});

        actorTimeAcc = 0;
    }

	public sealed override bool _onCreate()
	{
		return true;
	}
    
    public bool IsRunning()
    {
        return m_bInRunMode;
    }

	public void SetForceRunMode(bool flag)
	{
		forceRunMode = flag;
	}

	public void SetDefaultWeapenTag(int tag)
	{
		defaultWeaponTag = tag;
	}

    public void SetDefaultWeapenType(int type)
    {
        defaultWeaponType = type;
    }

    public int GetDefaultWeaponType()
    {
        return defaultWeaponType;
    }

    public sealed override void OnJoystickMove (int degree)
	{
		//return;
		//Logger.LogErrorFormat("OnJoystickMove degree={0}", degree);
		if (isMainActor)
		{
			//三段斩按上下中断技能
			/*
			if (sgGetCurrentState() == (int)ActionState.AS_CASTSKILL && 
				(GetCurSkillID()==1506 || GetCurSkillID()==1507 || GetCurSkillID()==1508))
			{
				if (degree>=75/15 && degree<=105/15 || degree>=255/15 && degree <=285/15)
				{
					//Logger.LogErrorFormat("degree={0} cancel skill:{1}", degree, GetCurSkillID());
					CancelSkill(GetCurSkillID());
					sgLocomoteState();

				}
			}*/
		}
	}

    void _updateController(int delta)
    {
        if (controller != null)
        {
            if (controller.IsEnd())
            {
                if (controller.AutoRemove())
                {
                    controller = null;
                }
            }
            else
            {
                controller.OnTick(delta);
            }
        }
    }
    public sealed override bool Update(int iDeltaTime)
    {
        _updateController(iDeltaTime);
        _updateDeadProtect(iDeltaTime);
        if (GetLifeState() != (int)EntityLifeState.ELS_ALIVE)
            return false;
        _internalDealInput();
        _internalUpdate(iDeltaTime);

        if (!IsBoss() && needDead)
        {
            needDead = false;
            DoDead();
        }

        if (IsDead() && !IsInPassiveState())
        {
            DoDead();
        }

        skillDamageManager.Update(iDeltaTime);

        return true;
    }

    private IBeActorController controller;

	public void SetController(IBeActorController controller)
	{
		controller.SetOwner(this);
		controller.OnEnter();
		this.controller = controller;
	}	
    public void _internalDealInput()
    {
        //if (isMainActor)
        {
            if (attackButtonState == ButtonState.PRESS)
            {
                if (sgGetCurrentState() == (int)ActionState.AS_JUMPBACK)
                {
                    //后跳时按普攻
                    TriggerEventNew(BeEventType.onJumpBackAttack);
                }

                //决斗场机器人释放连招普攻时 需要检测目标是否处于浮空状态
                if (isPkRobot && aiAttackNeedCheck && aiManager!=null && aiManager.aiTarget != null && !aiManager.aiTarget.HasTag((int)AState.ACS_FALL))
                {
                    aiManager.StopCurrentCommand();
                    aiManager.pkRobotWander = true;
                    aiManager.ResetDestinationSelect();
                    SetAttackButtonState(ButtonState.RELEASE);
                    SetAttackCheckFlag(false);
                }

                if (HasTag((int)AState.ACS_JUMP) && sgGetCurrentState() == (int)ActionState.AS_BUSY && GetPosition().z >= Global.Settings.JumpAttackLimitHeight)
                {
                    if (jumpAttackUseCount < GetEntityData().jumpAttackCount)
                    {
                        UseSkill(GetEntityData().jumpAttackID);
                    }

                }
				else if (!hasRunAttackConfig &&
                    hasDoublePress && IsRunning() && 
					sgGetCurrentState() == (int)ActionState.AS_RUN &&
					GetEntityData().runAttackID > 0 &&
					isMainActor && 
					(aiManager==null?true:(pauseAI?true:false))
					)
                {
					UseSkill(GetEntityData().runAttackID);
                }
                else
                {
					if (isMainActor && sgGetCurrentState() == (int)ActionState.AS_CASTSKILL && (GetCurSkillID()==1102 || GetCurSkillID()==2522) && canWalkShootTurn)
					{
						canWalkShootTurn = false;
						SetFace(!GetFace());
						delayCaller.DelayCall(GlobalLogic.VALUE_500, ()=>{
							canWalkShootTurn = true;	
						});
					}


                    int attackID = GetEntityData().normalAttackID;
                    if (sgGetCurrentState() == (int)ActionState.AS_CASTSKILL &&
                        ((GetCurSkillID() == GetEntityData().runAttackID) ||
                        AttackComboSkillList.Contains(GetCurSkillID())))
                    {
                        attackID = GetCurSkillID();
                    }

                    int comboID = skillController.CheckComboSkill(attackID);
                    if (comboID > 0)
                    {
                        
                        bool flag = false;
                        if (BattleMain.IsModePvP(battleType) || !attackReplaceLigui)
                        {
                            BeSkill normalAttack = GetSkill(GetEntityData().normalAttackID);
                            if (normalAttack != null)
                            {
                                if (normalAttack.CanInterrupt(GetCurSkillID()))
                                {
                                    //      TriggerEvent(BeEventType.onCastNormalAttack);

                                    UseSkill(GetEntityData().normalAttackID);
                                    flag = true;
                                }
                            }
                        }

                        if (!flag)
                        {
                            //TriggerEvent(BeEventType.onCastNormalAttack);

                            BeSkill beSkill = skillController.GetCurrentSkill();
                            if (beSkill != null && beSkill.isComboSkill)
                            {
                                beSkill.cancelByCombo = true;
                                beSkill.OnComboBreak();
                            }

                            if (GetCurSkillID() == 1909)
                            {
                                BeSkill skill = GetSkill(comboID);
                                if (skill != null && beSkill != null && GetCurSkillID() != skill.comboSkillSourceID)
                                {
                                    return;
                                }
                            }
                            CastSkill(comboID);
                        }
                    }
                    else if (sgGetCurrentState() != (int)ActionState.AS_CASTSKILL)
                    {
                        //TriggerEvent(BeEventType.onCastNormalAttack);
                        UseSkill(attackID);
                    }
                }


            }
        }
    }

	public sealed override bool _isCmdMoveForbiden()
	{
		if(controller != null)
		{
			return false;
		}

		return base._isCmdMoveForbiden();
	}

    public sealed override bool IsAttackAdd2Statistics()
    {
		if (!isMainActor)
		{
			Logger.LogProcessFormat("[战斗] 不是玩家伤害");

            BeEntityData entityData = GetEntityData();
            if (null == entityData)
            {
                Logger.LogProcessFormat("[战斗] entityData 为空");
                return false;
            }

			if (!entityData.isSummonMonster)
			{
				Logger.LogProcessFormat("[战斗] 不是召唤物伤害");
				return false;
			}

			BeActor owner = GetOwner() as BeActor;
			if (null == owner)
			{
				Logger.LogProcessFormat("[战斗] 物体伤害 没有owner");
				return false;
			}

			if (!owner.isMainActor)
			{
				Logger.LogProcessFormat("[战斗] 物体伤害 owner{0}不是玩家", owner.GetName());
				return false;
			}

			Logger.LogProcessFormat("[战斗] 召唤物伤害");
		}

        return true;
    }


	public sealed override bool UpdateGraphic(int iDeltaTime)
	{
        int accTime = iDeltaTime;
        if (effectTimeNeedChange)
        {
            accTime = iDeltaTime * speedAcc;
        }
        _updateGraphic(iDeltaTime, accTime);
        //UpdateSpellBars(iDeltaTime);
        UpdateMechanismsGraphic(iDeltaTime);

        UpdateSpirit();

        return true;
	}

	public sealed override void OnRemove(bool force = false)
    {
		base.OnRemove(force);
		actorTimeAcc = 0;
        if (actorData != null)
        {
            actorData.RemoveHandle();
        }
    }

    int actorTimeAcc;
    public VFactor speedAcc;
    public bool effectTimeNeedChange = false;
    static readonly VFactor Skill_Speed_MIN = VFactor.NewVFactor(400, 1000);
    public void _internalUpdate(int iDeltaTime)
    {
        effectTimeNeedChange = false;
        int normalDelta = iDeltaTime;
        speedAcc = VFactor.one;
        if (sgGetCurrentState() == (int)ActionState.AS_CASTSKILL && !skillFreeTurnFace)
        {
            var skill = GetCurrentSkill();
            if (skill != null)
            {
                if (skill.SkillNeedChagneSpeed() && !skillFreeTurnFace)//skillFreeTurnFace 用于法师的移动施法
                {
                    var skillSpeed = skill.GetSkillSpeedFactor();
                    if (skill.skillData!=null && skill.skillData.PhaseRelatedSpeed == 1)
                    {
                        if (m_cpkCurEntityActionInfo != null && m_cpkCurEntityActionInfo.relatedAttackSpeed)//技能阶段受攻速影响
                        {
                            skillSpeed *= VFactor.NewVFactor(m_cpkCurEntityActionInfo.attackSpeed, GlobalLogic.VALUE_1000);
                            if (skillSpeed < Skill_Speed_MIN)
                                skillSpeed = Skill_Speed_MIN;
                        }
                        else
                        {
                            skillSpeed = VFactor.NewVFactor(skill.skillData.Speed,GlobalLogic.VALUE_1000);
                        }                      
                    }
                    iDeltaTime = (iDeltaTime * skillSpeed);
                    effectTimeNeedChange = true;
                    speedAcc = skillSpeed;
                }
                SetZDimScaleFactor(skill.GetSkillZDimFactor());
            }
        }
        else
		{
			SetZDimScaleFactor(VFactor.one);
			if (sgGetCurrentState() == (int)ActionState.AS_RUN || sgGetCurrentState() == (int)ActionState.AS_WALK || (skillFreeTurnFace&& sgGetCurrentState() == (int)ActionState.AS_CASTSKILL) ||
                (GetEntityData().isPet && (HasSpeed(moveXSpeed) || HasSpeed(moveYSpeed))))
			{
				var attribute = GetEntityData();
				if (attribute != null)
				{
					iDeltaTime = (iDeltaTime * moveSpeedFactor);
                    //技能减速保护，最多减75%
                    iDeltaTime = Mathf.Max(6, iDeltaTime);
                }
            }
		}

		actorTimeAcc = iDeltaTime;
#if NO_FIX_UPDATE
		base.Update(iDeltaTime);
#else
        int fixDelta = 16;
        while (actorTimeAcc > 0)
        {
            var tmpDelta = Mathf.Min(fixDelta, actorTimeAcc);
			base.Update(tmpDelta);

            if (!IsDead())
            {
                UpdateRecover(tmpDelta);
                UpdateSpellBarsGraphic(tmpDelta);             //读条的进度都放在逻辑里面刷新
                UpdateSpellBarsWithActor(tmpDelta);
                UpdateMechanismsImpactBySpeed(tmpDelta);
            }

            actorTimeAcc -= fixDelta;
        }
#endif
        UpdateSpellBarsWithBuff(normalDelta);      //buff相关的读条要放在外面
        //UpdateSpellBarsGraphic(normalDelta);
        if (buffController != null)
            buffController.UpdateBuff(normalDelta);
        if (skillController != null)
            skillController.UpdateSkill(normalDelta);
        //UpdateSkill(normalDelta);
        UpdateMechanisms(normalDelta);
		UpdateProtect(normalDelta);
		UpdatePet (normalDelta);
        UpdateActorData(normalDelta);
        UpdateMechanismList();
    }

    public void UpdateSpirit()
    {
#if !LOGIC_SERVER
        if (this.spirit == null)
            return;

        Vector3 pos = GetPosition().vector3;
        this.spirit.SetFaceLeft(GetFace());
#endif
    }

    public void ChangeRunMode(bool bRun)
    {
        //强制为true
		if (/*isMainActor || */forceRunMode)
            bRun = true;

        m_bInRunMode = bRun;

		if(m_bInRunMode)
		{
			doublePressRunLeft = GetFace();
		}

        RefreashSpeed();
    }

	public void RefreshMoveSpeed()
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeActor.RefreshMoveSpeed"))
        {
#endif
		moveSpeedFactor = VFactor.one;

		float geMoveSpeedModify = 1.0f;
		var attribute = GetEntityData();
		if (attribute != null)
		{
			moveSpeedFactor = new VFactor(attribute.GetMoveSpeed(),GlobalLogic.VALUE_1000);//attribute.GetMoveSpeed() / (float)(GlobalLogic.VALUE_1000);
			geMoveSpeedModify = attribute.walkAnimationSpeedPercent / (float)(GlobalLogic.VALUE_100);
		}

		var mode = m_bInRunMode ?  runAction:walkAction;
		float geFactor = m_bInRunMode?runSpeedFactor:walkSpeedFactor;

		PlayAction(mode, geFactor * moveSpeedFactor.single * geMoveSpeedModify);

		if (attribute != null)
		{
			if (IsMonster())
			{
				moveSpeedFactor = moveSpeedFactor * Global.Settings.monsterWalkSpeedFactor;
				//var f = Global.Settings.monsterWalkSpeedFactor;
				//moveSpeedFactor.nom = moveSpeedFactor.nom * f.nom * moveSpeedFactor.den / f.den;
			}
				
		}
#if ENABLE_PROFILER
        }
#endif
	}


    public void RefreashSpeed()
    {
        if (m_bInRunMode)
			speedConfig = runSpeed;
        else
			speedConfig = walkSpeed;
    }
    
    public sealed override void _onReset()
    {
		//没有处理Dunfu,Dunfu状态不能重置 TODO
		attackButtonState = ButtonState.NONE; 
    }


    public sealed override void _clearStates()
    {
        base._clearStates();
    }

    public sealed override void _onEnterFrame(int iFrame)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeActor._onEnterFrame"))
        {
#endif
        base._onEnterFrame(iFrame);

        //角色抓取处理
        if (m_cpkCurEntityActionFrameData != null)
        {
            if (m_cpkCurEntityActionFrameData.kFlag.HasFlag((int)DSFGrapOp.GRAP_EXECUTE))
            {
                grabController.ExecuteGrab();
            }

            if (m_cpkCurEntityActionFrameData.kFlag.HasFlag((int)DSFGrapOp.GRAP_RELEASE))
            {
                grabController.EndGrab();
            }
            
            //删除Buff
            if (m_cpkCurEntityActionFrameData.kFlag.HasFlag((int) DSFFrameTags.TAG_REMOVE_BUFF))
            {
	            int buffId = int.Parse(m_cpkCurEntityActionFrameData.kFlag.GetFlagData());
	            if(buffId > 0)
					buffController.RemoveBuff(buffId);
            }
            
            //删除机制
            if (m_cpkCurEntityActionFrameData.kFlag.HasFlag((int) DSFFrameTags.TAG_REMOVE_MECHANISM))
            {
	            int mId = int.Parse(m_cpkCurEntityActionFrameData.kFlag.GetFlagData());
	            if(mId > 0)
					RemoveMechanism(mId);
            }
        }
        grabController.UpdateTargetPos();
#if ENABLE_PROFILER
        }
#endif
    }

    public void CastSkill(int skillID)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeActor.CastSkill"))
        {
#endif
        //combo技能切换的时候也需要清除技能配置文件中添加的阶段机制列表
        ClearPhaseDelete();
        ClearFinishDeleteAll();
        ClearPhaseDeleteAudio();
        ClearSkillFinishDeleteAudio();

        SetCurSkillID(skillID);

        var skill = GetSkill(skillID);
        if (skill != null )
        {
            
            skill.pressedForwardMove = IsPressForwardMoveCmd();
            
        }

        sgSwitchStates(new BeStateData((int)ActionState.AS_CASTSKILL));
#if ENABLE_PROFILER
        }
#endif
    }

	public sealed override void DoHurt(int value, List<int> attachValues=null, HitTextType type = HitTextType.NORMAL, BeEntity attacker = null, HitTextType typeForSpecialBuff = HitTextType.NORMAL,bool forceBuffHurt = false, HitDamageType damageType = HitDamageType.Normal) {
        //无敌buff不受伤   无敌状态下已经存在的Buff仍然可以造成伤害
        if (!stateController.CanBeHit() && !forceBuffHurt)              
			return;

		//宠物不受伤害
		if (GetEntityData()!= null && GetEntityData().isPet)
			return;
		base.DoHurt(value, attachValues, type, attacker, typeForSpecialBuff, false, damageType);
	}
		
	public sealed override void _onHurtEntity(BeEntity pkEntity, VInt3 hitPos, int hurtID, uint attackProcessId = 0)
    {
        if (pkEntity == null)
            return;
        grabController.CheckGrab(pkEntity);   
		base._onHurtEntity(pkEntity, hitPos, hurtID, attackProcessId);
    }

	public int GenNewMonsterID(int mid, int level)
	{
		MonsterIDData idData = new MonsterIDData(mid);
		return idData.GenMonsterID(level);
	}

	public int GetSummonNum(int summonID, int summonNum=1, int numLimit=0, int group=0, int groupNumLimit=0)
	{
		if (currentBeScene == null)
			return 0;
		
		int existNum = currentBeScene.GetSummonCountByID(summonID, this);
		//单独限制
		if (numLimit > 0 && existNum + summonNum > numLimit)
			summonNum = numLimit - existNum;

		//组限制
		if (summonNum > 0 && group > 0)
		{
			if (summonNum + groupSummonNum[group] > groupNumLimit)
				summonNum = groupNumLimit - groupSummonNum[group];
		}

		return summonNum;
	}

	public bool DoSummon(
		int summonID, 
		int level=1, 
		EffectTable.eSummonPosType posType = EffectTable.eSummonPosType.FACE,
        IList<int> posType2 = null,
        int summonNum=1,
		int numLimit=0, 
		int group=0, 
		int groupNumLimit=0, 
		int skillID=0, 
		bool related = false,
		int summonMonsterSkillLevel=0,
		int existTime=0,
		List<int> summonIDs = null,
		SummonDisplayType displayType = SummonDisplayType.NONE,
		object[] summoneds = null,
        bool isSameFace = true,
        BeSummonInfoData summonInfo = null
    )
    {
        if (summonID <= 0 && summonIDs == null)
            return false;
        int originSummonId = summonID;

		summonNum = GetSummonNum(summonID, summonNum, numLimit, group, groupNumLimit);
		if (summonNum <= 0)
			return false;

		summonID = GenNewMonsterID(summonID, level);

		if (summonIDs != null)
		{
			for(int i=0; i<summonIDs.Count; ++i)
				summonIDs[i] = GenNewMonsterID(summonIDs[i], level);
		}


		bool showBlood = IsMonster();

		if (IsMonster())
		{
			BeUtility.AdjustMonsterDifficulty(ref GetEntityData().monsterID, ref summonID);
			if (summonIDs != null)
			{
				for(int i=0; i<summonIDs.Count; ++i)
				{
					int tmp = summonIDs[i];
					BeUtility.AdjustMonsterDifficulty(ref GetEntityData().monsterID, ref tmp);
					summonIDs[i] = tmp;
				}
			}
		}

		if (summonNum > 0)
		{

			for (int i = 0; i < summonNum; ++i)
			{
				VInt3 pos = GetPosition();
				pos.z = 0;

                if (posType2 != null && posType2.Count > 1)
                {
                    int offset = posType2[1];
                    if (posType2[0] == 1)
                    {
                        pos.x += (GetFace() ? -1 : 1) * (i + 1) * offset / GlobalLogic.VALUE_1000 * (int)IntMath.kIntDen;
                        if(BeClientSwitch.FunctionIsOpen(ClientSwitchType.SummonNewFindPos))
                            pos = BeAIManager.FindStandPositionNew(pos, CurrentBeScene, GetFace());
                        else
                            pos = BeAIManager.FindStandPosition(pos, CurrentBeScene, GetFace());
                    }
                    else if(posType2[0] == 2)
                    {
                        pos.x += (GetFace() ? -1 : 1) * (i + 1) * offset / GlobalLogic.VALUE_1000 * (int)IntMath.kIntDen;
                    }
                }
                else
                {
                    if (null != summonInfo && summonInfo.useSummonTable)
                    {
                       //走召唤信息表
                        switch (summonInfo.summonPosType)
                        {
                            case (int)SummonInfoTable.eSummonPosType.FACE:
                                pos.x += (GetFace() ? -1 : 1) * (i + 1) * (int)IntMath.kIntDen;
                                break;
                            case (int)SummonInfoTable.eSummonPosType.FACE_OFFSET:
                                pos.x += (GetFace() ? -1 : 1) * summonInfo.offset.x;
                                pos.y += summonInfo.offset.y;
                                break;
                            case (int)SummonInfoTable.eSummonPosType.POSITION:
                                pos = summonInfo.summonPos;
                                break;
                            case (int)SummonInfoTable.eSummonPosType.RANDOM:
                                int randAngle = FrameRandom.Random((uint)summonInfo.radiusAngle.i);
                                VFactor angle = VFactor.pi * (randAngle - summonInfo.radiusAngle.i / 2) / 180;
                                int angleLen = FrameRandom.Random((uint)summonInfo.radius.i);
                                if (!GetFace())
                                {
                                    pos.x += (IntMath.cos(angle.nom, angle.den) * angleLen).integer;
                                }
                                else
                                {
                                    pos.x -= (IntMath.cos(angle.nom, angle.den) * angleLen).integer;
                                }
                                pos.y += (IntMath.sin(angle.nom, angle.den) * angleLen).integer;
                                break;
                        }
                        if (summonInfo.considerBlock)
                        {
                            //如果召唤兽的位置在阻挡里，就从召唤师的位置开始寻找
                            if (currentBeScene.IsInBlockPlayer(pos))
                            {
                                if (BeClientSwitch.FunctionIsOpen(ClientSwitchType.SummonNewFindPos))
                                    pos = BeAIManager.FindStandPositionNew(GetPosition(), CurrentBeScene, GetFace());
                                else
                                    pos = BeAIManager.FindStandPosition(GetPosition(), CurrentBeScene, GetFace());
                            }
                        }
                    }
                    else
                    {
                        //走触发效果表
                        if (posType == EffectTable.eSummonPosType.FACE)
                        {
                            pos.x += (GetFace() ? -1 : 1) * (i + 1) * (int)IntMath.kIntDen;
                            //如果召唤兽的位置在阻挡里，就从召唤师的位置开始寻找
                            if (currentBeScene.IsInBlockPlayer(pos))
                            {
                                if (BeClientSwitch.FunctionIsOpen(ClientSwitchType.SummonNewFindPos))
                                    pos = BeAIManager.FindStandPositionNew(GetPosition(), CurrentBeScene, GetFace());
                                else
                                    pos = BeAIManager.FindStandPosition(GetPosition(), CurrentBeScene, GetFace());
                            }
                        }
                        else if (posType == EffectTable.eSummonPosType.FACE_FORCE)
                        {
                            pos.x += (GetFace() ? -1 : 1) * (i + 1) * (int)IntMath.kIntDen;
                        }
                        else if (posType == EffectTable.eSummonPosType.FACE_BLACK)
                        {
                            pos.x += (GetFace() ? -1 : 1) * (i + 1) * (int)IntMath.kIntDen;
                            if (BeClientSwitch.FunctionIsOpen(ClientSwitchType.SummonNewFindPos))
                                pos = BeAIManager.FindStandPositionNew(pos, CurrentBeScene, GetFace(), true);
                            else
                                pos = BeAIManager.FindStandPosition(pos, CurrentBeScene, GetFace(), true);
                        }
                    }
                }

                int tmpSummonID = summonID;

				if (summonIDs != null)
					tmpSummonID = summonIDs[FrameRandom.Random((uint)summonIDs.Count)];

                int levelAdd = 0;
                var param = TriggerEventNew(BeEventType.onBeforeSummon, new EventParam() { m_Int = tmpSummonID, m_Int2 = levelAdd });
                levelAdd = param.m_Int2;

				if (levelAdd != 0)
				{ 
					tmpSummonID += GlobalLogic.VALUE_100 * levelAdd;
					//Logger.LogErrorFormat("summon level add {0} summonID:{1}", levelAdd[0], tmpSummonID);
				}

                BeActor summonActor = currentBeScene.SummonMonster(tmpSummonID, pos, m_iCamp, this, related, summonMonsterSkillLevel, showBlood, originSummonId, false, isSameFace);
                if (summonActor != null)
                {
                    if (summoneds != null && summoneds.Length >= 1)
                    {
                        summoneds[0] = summonActor;
                    }
                    if (displayType == SummonDisplayType.FAZHEN)
                    {
                        owner.CurrentBeScene.currentGeScene.CreateEffect(1016, summonActor.GetPosition().vec3);
					}

					summonActor.delayCaller.DelayCall(10, ()=>{
						summonActor.buffController.TryAddBuff(31, GlobalLogic.VALUE_1000);
                    });

                    //召唤信息表中的字段功能
                    if (null != summonInfo && summonInfo.useSummonTable)
                    {
                        //添加生命周期
                        if (summonInfo.lifeTime > 0)
                            summonActor.buffController.TryAddBuff((int)GlobalBuff.LIFE_TIME, summonInfo.lifeTime);
                        //替换出生动画
                        // if (summonActor.HasAction(summonInfo.birthActionName))
                        // {
                        //     string name = summonActor.GetActionNameByType(ActionType.ActionType_BIRTH);
                        //     summonActor.m_cpkEntityInfo.ReplaceActionInfo(name, summonActor.m_cpkEntityInfo._vkActionsMap[summonInfo.birthActionName]);
                        // }
                    }


                    /*var scaleArray = new int[1];
                    scaleArray[0] = GlobalLogic.VALUE_1000;
                    TriggerEvent(BeEventType.onChangeSummonScale, new object[] { summonActor, scaleArray });
                    var scale = summonActor.GetScale().i * VFactor.NewVFactor(scaleArray[0], GlobalLogic.VALUE_1000);*/
                    var eventData = TriggerEventNew(BeEventType.onChangeSummonScale, new EventParam(){m_Obj = summonActor, m_Int = GlobalLogic.VALUE_1000});
                    var scale = summonActor.GetScale().i * VFactor.NewVFactor(eventData.m_Int, GlobalLogic.VALUE_1000);
                    summonActor.SetScale(scale);
                    var eventData1 = TriggerEventNew(BeEventType.onChangeSummonScale, new EventParam(){m_Obj = summonActor, m_Int = GlobalLogic.VALUE_1000});
                    var weight = summonActor.GetEntityData().weight.i * VFactor.NewVFactor(eventData.m_Int, GlobalLogic.VALUE_1000);
                    summonActor.GetEntityData().weight = weight;

                    summonActor.attribute.isSpecialAPC = attribute.isSpecialAPC;
                    //TriggerEvent(BeEventType.onSummon, new object[] { summonActor, skillID });
                    TriggerEventNew(BeEventType.onSummon, new EventParam(){m_Obj = summonActor, m_Int = skillID});

                    /*var lifeTimeArray = new int[2];
                    lifeTimeArray[0] = existTime;
                    lifeTimeArray[1] = GlobalLogic.VALUE_1000;
                    TriggerEvent(BeEventType.onChangeSummonLifeTime, new object[] { summonActor, lifeTimeArray });
                    existTime = lifeTimeArray[0] * VFactor.NewVFactor(lifeTimeArray[1], GlobalLogic.VALUE_1000);*/
                    
                    eventData = TriggerEventNew(BeEventType.onChangeSummonLifeTime, new EventParam(){m_Obj = summonActor, m_Int = existTime, m_Int2 = GlobalLogic.VALUE_1000});
                    existTime = eventData.m_Int * VFactor.NewVFactor(eventData.m_Int2, GlobalLogic.VALUE_1000);

                    if (existTime > 0)
					{
						summonActor.buffController.TryAddBuff((int)GlobalBuff.LIFE_TIME, existTime);
					}

					if (group > 0)
					{
						groupSummonNum[group]++;
					//	Logger.LogErrorFormat("++groupSummonNum[{0}]={1}", group, groupSummonNum[group]);
						summonActor.summonNumHandle = summonActor.RegisterEventNew(BeEventType.onDead, eventParam =>
						{
							if (summonActor.summonNumHandle != null)
							{
								summonActor.summonNumHandle.Remove();
								summonActor.summonNumHandle = null;
							}
									

							groupSummonNum[group]--;
						//	Logger.LogErrorFormat("--groupSummonNum[{0}]={1}", group, groupSummonNum[group]);
						});
					}
				}

			}

			return true;
		}
		else {
			//Logger.LogWarningFormat("summon num limit {0}, existNum:{1} limitNum:{2} group:{3}, groupLimit:{4}", summonID, existNum, numLimit, group, groupNumLimit);
		}

		return false;
	}

	
	public sealed override bool TrySummon(EffectTable hurtData)
    {
        int summonID = hurtData.SummonID;
        int summonInfoID = hurtData.SummonInfoID;
        if (summonID <= 0 && hurtData.SummonRandList[0]<=0 && summonInfoID <= 0)
            return false;
        int skillLevel = GetSkillLevel(hurtData.SkillID);
        var info = new BeSummonInfoData(hurtData, skillLevel, battleType);
        return TryDoSummon(info, hurtData.ID);
    }
	
	public bool TryDoSummon(BeSummonInfoData info, int hurtDataId = 0)
	{
		if (info == null)
			return false;
		if (info.useSummonOwnerLevel)
        {
            if (null != GetEntityData())
                info.summonLevel = GetEntityData().level;
            else
                info.summonLevel = 0;
        }
        // int summonLevel = 0;
        // if (BattleMain.IsChijiNeedReplaceHurtId(hurtData.ID, battleType))
        // {
        //     var chijiEffectMapTable = TableManager.instance.GetTableItem<ChijiEffectMapTable>(hurtData.ID);
        //     summonLevel = TableManager.GetValueFromUnionCell(chijiEffectMapTable.SummonLevel, skillLevel);
        // }
        // else
        // {
        //     summonLevel = TableManager.GetValueFromUnionCell(hurtData.SummonLevel, skillLevel);
        // } 
        // int summonNum = TableManager.GetValueFromUnionCell(hurtData.SummonNum, skillLevel);
        // int singleNumLimit = hurtData.SummonNumLimit;
		// int groupNumLimit = TableManager.GetValueFromUnionCell(hurtData.SummonGroupNumLimit, skillLevel);
        // int summonGroup = hurtData.SummonGroup;
		// bool related = hurtData.SummonRelation != 0;

		// int summonMonsterSkillLevel = 0;
		// if (related)
		// 	summonMonsterSkillLevel = skillLevel;
		// if (summonLevel <= 0)
		// 	summonLevel = skillLevel;

        //召唤怪物的时候 将上一次召唤的怪物杀死
        bool killLastSummon = info.killLastSummon;
        if (killLastSummon)
        {
            List<BeActor> summonMonsterList = GamePool.ListPool<BeActor>.Get();
            owner.CurrentBeScene.FindActorById(summonMonsterList, info.summonID);
            if (summonMonsterList.Count > 0)
            {
                for (int i = 0; i < summonMonsterList.Count; i++)
                {
                    if (summonMonsterList[i].owner == owner)
                    {
                        summonMonsterList[i].DoDead();
                    }
                }
            }
            GamePool.ListPool<BeActor>.Release(summonMonsterList);
        }

		// List<int> summonRand = null;
		// if (hurtData.SummonRandList[0] > 0)
		// {
		// 	summonRand = new List<int>();
		// 	for(int i=0; i<hurtData.SummonRandList.Count; ++i)
		// 		summonRand.Add(hurtData.SummonRandList[i]);
		// }

        /*int[] summonNumArray = new int[1];
        summonNumArray[0] = summonNum;
        owner.TriggerEvent(BeEventType.onChangeSummonNum, new object[] { hurtData.ID, -1, summonNumArray });
        summonNum = summonNumArray[0];*/
        /*int[] summonNumLimitArray = new int[1];
        summonNumLimitArray[0] = singleNumLimit;
        owner.TriggerEvent(BeEventType.onChangeSummonNumLimit, new object[] { hurtData.ID, -1, summonNumLimitArray });
        singleNumLimit = summonNumLimitArray[0];*/

        var eventData = owner.TriggerEventNew(BeEventType.onChangeSummonNumLimit, new EventParam(){m_Int = hurtDataId, m_Int2 = -1, m_Int3 = info.singleNumLimit, m_Int4 = info.summonNum});
        info.singleNumLimit = eventData.m_Int3;
        info.summonNum = eventData.m_Int4;

#if USE_FB_TABLE
        return DoSummon(info.summonID, info.summonLevel, (EffectTable.eSummonPosType)info.summonPosType, info.summonPosType2, info.summonNum, info.singleNumLimit, info.summonGroup, info.groupNumLimit, info.skillID, info.related, info.summonMonsterSkillLevel, 0, info.summonRandList, (SummonDisplayType)info.summonDisplay, null, true, info);

#else
        return DoSummon(info.summonID, info.summonLevel, (EffectTable.eSummonPosType)info.summonPosType,Utility.toList(info.summonPosType2) , info.summonNum, info.singleNumLimit, info.summonGroup, info.groupNumLimit, info.skillID, info.related, info.summonMonsterSkillLevel, 0, info.summonRandList, (SummonDisplayType)info.summonDisplay, null, true, info);
       #endif
	}

	//用于执行一次性的机制
    public override void TryAddMechanism(int mechanismId)
    {
        if (mechanismId == 0)
            return;
        base.TryAddMechanism(mechanismId);
        BeMechanism mechanism = AddMechanism(mechanismId);
        if(mechanism != null && mechanismList.Contains(mechanism))
        {
            RemoveMechanism(mechanism);
        }
    }

    public int TryGetBuffEnhanceLevel(int buffID, int skillID = 0)
	{
		int levelAdd = 0;
		if (buffID > 0)
		{
			//角色层面附加的
			if (buffEnhanceList.ContainsKey(buffID))
			{
				levelAdd += buffEnhanceList[buffID].level;
			}

			//指定技能附加的
			if (skillID > 0)
			{
				var skill = GetSkill(skillID);
				if (skill != null)
				{
					//根据技能修正
					BuffInfoData info = skill.GetBuffInfo(buffID);
					if (info != null)
					{
						levelAdd += info.level;
					}
				}
			}
		}

		return levelAdd;
	}

    public sealed override bool TryAddBuff(ProtoTable.EffectTable hurtData, BeEntity target = null, int duration = 0, bool useBuffAni = true, bool finishDelete = false, bool finishDeleteAll = false)
    {
		List<BuffInfoData> buffInfos = new List<BuffInfoData>();

		//buff等级
        int skillID = hurtData.SkillID==0?GetCurSkillID():hurtData.SkillID;
		int skillLevel = GetSkillLevel(skillID);

		if (hurtData.BuffID > 0 && attribute!=null && !attribute.isSpecialAPC)
		{
			//先处理buff等级额外提升
			int buffLevel = skillLevel;

            int buffAbnormalLevel = 0;
            int buffAddRate = 0;
            ChijiEffectMapTable chijiEffectMapTable = null;
            if (BattleMain.IsChijiNeedReplaceHurtId(hurtData.ID, battleType))
            {
                chijiEffectMapTable = TableManager.instance.GetTableItem<ChijiEffectMapTable>(hurtData.ID);
                buffAbnormalLevel = TableManager.GetValueFromUnionCell(chijiEffectMapTable.BuffLevel, buffLevel);
                buffAddRate = TableManager.GetValueFromUnionCell(chijiEffectMapTable.AttachBuffRate, buffLevel);
            }
            else
            {
                buffAddRate = TableManager.GetValueFromUnionCell(hurtData.AttachBuffRate, buffLevel);
                buffAbnormalLevel = TableManager.GetValueFromUnionCell(hurtData.BuffLevel, buffLevel);
            }
            
            int buffLevelAdd = TryGetBuffEnhanceLevel(hurtData.BuffID, skillID);
            buffAbnormalLevel += buffLevelAdd;

            /*int[] buffLevelArray = new int[1];
            buffLevelArray[0] = 0;
            owner.TriggerEvent(BeEventType.onChangeBuffLevel, new object[] { hurtData.ID, 0, buffLevelArray, owner });
            buffLevel += buffLevelArray[0];
            buffAbnormalLevel += buffLevelArray[0];*/
            
            var ret = owner.TriggerEventNew(BeEventType.onChangeBuffLevel, new EventParam{m_Int = hurtData.ID, m_Int2 = 0, m_Int3 = 0, m_Obj = owner});
            buffLevel += ret.m_Int3;
            buffAbnormalLevel += ret.m_Int3;
            
            if (buffLevel < 1) buffLevel = 1;
            if (buffAbnormalLevel < 1) buffAbnormalLevel = 1;

            int buffDuraction = 0; 
			//给的强制buff时间
			if (duration > 0)
			{
				buffDuraction = duration;
			}
			else if (duration == -1)
			{
				buffDuraction = int.MaxValue;
			}
			else
			{
                if (chijiEffectMapTable != null)
                {
                    buffDuraction = TableManager.GetValueFromUnionCell(chijiEffectMapTable.AttachBuffTime, buffLevel);
                }
                else
                {
                    buffDuraction = TableManager.GetValueFromUnionCell(hurtData.AttachBuffTime, buffLevel);
                }
				
				if (buffDuraction == -1)
					buffDuraction = int.MaxValue;
			}

            /*int[] buffAddRateArr = new int[1];
            buffAddRateArr[0] = buffAddRate;
            owner.TriggerEvent(BeEventType.onChangeBuffAttackRate,new object[] { BuffAttachType.EFFECTTABLE, hurtData.ID, buffAddRateArr, owner });
            buffAddRate = buffAddRateArr[0];*/
            
            var ret2 = owner.TriggerEventNew(BeEventType.onChangeBuffAttackRate,new EventParam{m_Int = (int) BuffAttachType.EFFECTTABLE, m_Int2 = hurtData.ID, m_Int3 = buffAddRate, m_Obj = owner});
            buffAddRate = ret2.m_Int3;

            //int[] buffDuration = new int[1];
            //buffDuration[0] = buffDuraction;
            //owner.TriggerEvent(BeEventType.OnChangeEffectTime, new object[] { hurtData.ID, buffDuration });
            //buffDuraction = buffDuration[0];
            var param = owner.TriggerEventNew(BeEventType.OnChangeEffectTime, new EventParam() { m_Int = hurtData.ID, m_Int2 = buffDuraction });
            buffDuraction = param.m_Int2;

            //异常状态攻击力
            int buffAttack = 0;
            if (chijiEffectMapTable != null)
            {
                buffAttack = TableManager.GetValueFromUnionCell(chijiEffectMapTable.BuffAttack, buffLevel);
            }
            else
            {
                buffAttack = TableManager.GetValueFromUnionCell(hurtData.BuffAttack, buffLevel);
            }

            /*int[] buffAttackArray = new int[1];
            buffAttackArray[0] = GlobalLogic.VALUE_1000;
            owner.TriggerEvent(BeEventType.onChangeBuffAttack, new object[] { hurtData.ID, 0, buffAttackArray, owner });
            buffAttack *= VFactor.NewVFactor(buffAttackArray[0], GlobalLogic.VALUE_1000);*/

            var ret3 = owner.TriggerEventNew(BeEventType.onChangeBuffAttack, new EventParam{m_Int = hurtData.ID, m_Int2 = 0, m_Int3 = GlobalLogic.VALUE_1000, m_Obj = owner});
            buffAttack *= VFactor.NewVFactor(ret3.m_Int3, GlobalLogic.VALUE_1000);
            
            BuffInfoData info = new BuffInfoData
            {
                buffID = hurtData.BuffID,
                level = buffLevel,
                duration = buffDuraction,
                prob = buffAddRate,
                attack = buffAttack,
                target = (hurtData.BuffTarget == ProtoTable.EffectTable.eBuffTarget.SELF) ? BuffTarget.SELF : BuffTarget.ENEMY,
                abnormalLevel = buffAbnormalLevel
            };
            buffInfos.Add(info);
		}

		var buffInfoList = hurtData.BuffInfoID;
        
		//pvp用这列特殊配的buff，如果没配就用前面的
		if (BattleMain.IsModePvP(battleType) && hurtData.PVPBuffInfoID.Count > 0 && hurtData.PVPBuffInfoID[0]>0)
		{
			buffInfos.Clear();
	        if (BattleMain.IsChijiNeedReplaceHurtId(hurtData.ID, battleType))
	        {
	            var chijiEffectMapTable = TableManager.instance.GetTableItem<ChijiEffectMapTable>(hurtData.ID);
	            buffInfoList = chijiEffectMapTable.PVPBuffInfoID;
	        }
	        else
	        {
	            buffInfoList = hurtData.PVPBuffInfoID;
	        }
		}

		for(int i=0; i<buffInfoList.Count; ++i)
		{
			if (buffInfoList[i] > 0)
			{
				int buffLevelAdd = TryGetBuffEnhanceLevel(hurtData.BuffID, skillID);

				BuffInfoData info = new BuffInfoData(buffInfoList[i], skillLevel);
				info.abnormalLevel += buffLevelAdd;
				buffInfos.Add(info);
			}
		}

		bool result = false;

		//buff额外属性增加， 并挨个添加buff
		for(int i=0; i<buffInfos.Count; ++i)
		{
			//技能附加
			var info = buffInfos[i];
			var skill = GetSkill(skillID);
			if (skill != null)
			{
				BuffInfoData skillEnhance = skill.GetBuffInfo(hurtData.BuffID);
				if (skillEnhance != null)
				{
					info.DoEnhance(skillEnhance);
				}
			}

			//角色附加
			if (buffEnhanceList.ContainsKey(info.buffID))
			{
				info.DoEnhance(buffEnhanceList[info.buffID]);
			}

            /*int[] radiusArray = new int[1];
            radiusArray[0] = GlobalLogic.VALUE_1000;
            owner.TriggerEvent(BeEventType.onChangeBuffTargetRadius, new object[] { (int)info.buffInfoID, radiusArray });
            info.buffTargetRangeRadius *= VFactor.NewVFactor(radiusArray[0], GlobalLogic.VALUE_1000);*/
            
            var eventData = owner.TriggerEventNew(BeEventType.onChangeBuffTargetRadius, new EventParam(){m_Int = info.buffInfoID, m_Int2 = GlobalLogic.VALUE_1000});
            info.buffTargetRangeRadius *= VFactor.NewVFactor(eventData.m_Int2, GlobalLogic.VALUE_1000);
            
            if (CheckIsRangeBuffInfo(info))
			{
				List<BeActor> targets = GamePool.ListPool<BeActor>.Get();
				if (info.target == BuffTarget.RANGE_ENEMY)
				{
					CurrentBeScene.FindTargets(
					targets, this, 
					VInt.NewVInt(info.buffTargetRangeRadius,GlobalLogic.VALUE_1000)
					);
				}
				else if (info.target == BuffTarget.RANGE_FRIEND)
				{
					CurrentBeScene.FindTargets(
					targets, this, 
					VInt.NewVInt(info.buffTargetRangeRadius,GlobalLogic.VALUE_1000),
					true
					);
					targets.RemoveAll(item=>{
						if (item == this)
							return true;	
						return false;
					});
				}
                else if(info.target == BuffTarget.RANGE_FRIEND_ADNSELF)
                {
                    CurrentBeScene.FindTargets(
                    targets, this,
                    VInt.NewVInt(info.buffTargetRangeRadius, GlobalLogic.VALUE_1000),
                    true
                    );
                }
                //查找友方英雄
                else if (info.target == BuffTarget.RANGE_FRIENDHERO)
                {
                    BeUtility.GetAllFriendPlayers(this, targets);
                }
                else if (info.target == BuffTarget.RANGE_ENEMYHERO)
                {
                    BeUtility.GetAllEnemyPlayers(this, targets);
                }
                else if(info.target == BuffTarget.RANGE_FRIEND_NOTSUMMON)
                {
                    BeGetRangeFriendNotSummon filter = new BeGetRangeFriendNotSummon();
                    CurrentBeScene.FindTargets(
                    targets, this,
                    VInt.NewVInt(info.buffTargetRangeRadius, GlobalLogic.VALUE_1000),
                    true, filter
                    );
                }
                else if(info.target == BuffTarget.OUT_OF_RANGE_ENEMY)
                {
                    BeGetConcentricCircleTarget filter = new BeGetConcentricCircleTarget();
                    filter.m_Owner = this;
                    filter.m_OwnerPosXY = new VInt2(this.GetPosition().x, this.GetPosition().y);
                    filter.m_MinCircleRadius = VInt.NewVInt(info.buffTargetRangeRadius, GlobalLogic.VALUE_1000);
                    filter.m_MaxCircleRadius = BeGetConcentricCircleTarget.LargeMaxCirleRadius;
                    CurrentBeScene.GetFilterTarget(targets, filter);
                }
				else if (info.target == BuffTarget.RANGE_OWNER)
				{
					var topOwner = owner.GetTopOwner(owner) as BeActor;
					if (topOwner != null && topOwner.GetPID() != owner.GetPID())
					{
						if(topOwner.GetDistance(owner) < VInt.NewVInt(info.buffTargetRangeRadius, GlobalLogic.VALUE_1000))
							targets.Add(topOwner);
					}
				}

                if (targets != null)
				{
					for(int j=0; j<targets.Count; ++j)
					{
						if (targets[j] != null)
						{
							targets[j].buffController.TryAddBuff(info, this,false,new VRate(), this);
						}
							
					}
				}

				GamePool.ListPool<BeActor>.Release(targets);
			}
			else {
				BeActor buffTarget = target as BeActor;
				if (info.target == BuffTarget.SELF)
					buffTarget = this;

				if (buffTarget != null)
                {
                    //if (buffTarget.protectManager.fallGroundFlag)
                    //{
                    //    return false;
                    //}

                   BeBuff buff = buffTarget.buffController.TryAddBuff(info, this, useBuffAni,new VRate(),this);

                    if (buff != null)
                    {
                        buff.SetBuffReleaser(this);
                        //buffTarget.buffController.RefreshAbnormalData(buff);
                    }

                    //这种情况下的buff在技能此阶段结束的时候停止
                    if (buff != null && duration == -1)
					{
						buffTarget.buffController.AddPhaseDelete(buff);
					}

					if (buff != null && finishDelete)
					{
						buffTarget.buffController.AddFinishDelete(buff);
					}

                    if(buff != null && finishDeleteAll)
                    {
                        buffTarget.buffController.AddFinishDeleteAll(buff);
                    }

					result = true;
				}
			}
		}

		return result;
    }

    //判断是否为范围BuffInfo
    private bool CheckIsRangeBuffInfo(BuffInfoData info)
    {
        if (info == null)
            return false;
        if (info.target == BuffTarget.RANGE_ENEMY || info.target == BuffTarget.RANGE_FRIEND || info.target == BuffTarget.RANGE_FRIEND_ADNSELF || info.target == BuffTarget.OUT_OF_RANGE_ENEMY)
            return true;
        if (info.target == BuffTarget.RANGE_FRIENDHERO)
            return true;
        if (info.target == BuffTarget.RANGE_ENEMYHERO)
            return true;
        if (info.target == BuffTarget.RANGE_FRIEND_NOTSUMMON)
            return true;
        if (info.target == BuffTarget.RANGE_OWNER)
	        return true;
        return false;
    }

    public sealed override bool TryAddEntity(EffectTable hurtData, VInt3 pos, int triggeredLevel = 1, uint attackProcessId = 0)
    {
        //foreach (var entityID in hurtData.AttachEntity)
		for(int i=0; i<hurtData.AttachEntity.Count; ++i)
        {
			var entityID = hurtData.AttachEntity[i];
            if (entityID <= 0)
                continue;
            
			AddEntity(entityID, pos, triggeredLevel, 0, attackProcessId);
        }

        return false;
    }

    public BeEntity AddEntity(int entityID, VInt3 pos, int triggerSkillLevel = 1,int lifeTime = 0, uint attackProcessId = 0)
    {
        if (currentBeScene == null)
            return null;

        BeProjectile pkProjectile = currentBeScene.AddProjectile(entityID, this.m_iCamp, (int)ProjectType.SINGLE, lifeTime, triggerSkillLevel, this, -1, m_pkGeActor != null ? m_pkGeActor.GetUseCube() : false);
        pkProjectile.AttackProcessId = attackProcessId;
        pkProjectile.attribute = attribute;
		//pkProjectile.triggerSkillLevel = triggerSkillLevel;
        pkProjectile.totoalHitCount = 99999;
        pkProjectile.needSetFace = true;
        pkProjectile.SetFace(GetFace());
        pkProjectile.SetPosition(pos);

		pkProjectile.SetZDimScaleFactor(pkProjectile.GetProjectileZDimScale());
		pkProjectile.SetScale(
			GetScale().i*pkProjectile.GetProjectileScale());

        pkProjectile._updateGraphicActor();
        pkProjectile.InitLocalRotation();
        pkProjectile.RecordOriginPosition();

        TriggerEventNew(BeEventType.onAfterGenBullet, new EventParam() { m_Obj = pkProjectile });

        if(CurrentBeScene != null)
	        CurrentBeScene.TriggerEventNew(BeEventSceneType.onAfterGenBullet, new EventParam() { m_Obj = owner , m_Obj2 = pkProjectile});
        return pkProjectile;
		//pkProjectile.SetOwner(this);
    }

    public void DealBeforeGetDamage(BeEntity target, AttackResult result, int hurtID, bool isBackHit = false, int projectileResId = 0)
    {
        var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hurtID);
        if (hurtData != null && result != AttackResult.MISS)
        {
            var skill = this.GetSkill(hurtData.SkillID);
            if (skill != null)
            {
                skill.hit = true;
            }
        }

        //攻击添加buff

        bool triggerAttack = true;
        if (hurtData != null && hurtData.IsFriendDamage > 0)
            triggerAttack = false;

        if (triggerAttack)
            buffController.TriggerBuffs(BuffCondition.ATTACK, target as BeActor);

        if (hurtData.DamageDistanceType == ProtoTable.EffectTable.eDamageDistanceType.NEAR)
        {
            buffController.TriggerBuffs(BuffCondition.NEARATTACK, target as BeActor);          //近战攻击
        }
        else if (hurtData.DamageDistanceType == ProtoTable.EffectTable.eDamageDistanceType.FAR)
        {
            buffController.TriggerBuffs(BuffCondition.FARATTACK, target as BeActor);           //远程攻击
        }

        //监听属性攻击
        var attackElementType = attribute.GetAttackElementType(hurtID);
        if (attackElementType!=MagicElementType.NONE)
        {
            BuffCondition buffCondition = BuffCondition.LIGHTATTACK;
            switch (attackElementType)
            {
                case MagicElementType.LIGHT:
                    buffCondition = BuffCondition.LIGHTATTACK;
                    break;
                case MagicElementType.FIRE:
                    buffCondition = BuffCondition.FIREATTACK;
                    break;
                case MagicElementType.ICE:
                    buffCondition = BuffCondition.ICEATTACK;
                    break;
                case MagicElementType.DARK:
                    buffCondition = BuffCondition.DARKATTACK;
                    break;
            }
            buffController.TriggerBuffs(buffCondition, target as BeActor);
        }

        //指定技能命中时
        buffController.TriggerBuffs(BuffCondition.RELEASE_SEPCIFY_SKILL_HIT, target as BeActor, this.GetCurSkillID());

        if (result == AttackResult.CRITICAL)
        {
            //暴击别人
            buffController.TriggerBuffs(BuffCondition.CRITICAL_HIT, target as BeActor);
            TriggerEventNew(BeEventType.onHitCriticalBeforDamage,new EventParam() { });
            //被别人暴击
            var actor = target as BeActor;
            if (actor != null)
            {
                actor.buffController.TriggerBuffs(BuffCondition.BE_CRITICAL_HIT, this);
            }
        }


        if (isBackHit)
        {
            //背击别人
            buffController.TriggerBuffs(BuffCondition.BACK_HIT, target as BeActor);
            //被别人背击
            var actor = target as BeActor;
            if (actor != null)
                actor.buffController.TriggerBuffs(BuffCondition.BE_BACK_HIT, this);
        }

        TriggerEventNew(BeEventType.onBeforeHit, new EventParam() { m_Obj = target,m_Int = projectileResId,m_Int2 = hurtData.ID, m_Int3 = (int) AttackProcessId, m_Obj2 = hurtData});
        target.TriggerEventNew(BeEventType.onBeforeOtherHit, new EventParam() { m_Obj = this,m_Obj2 = hurtData});

        //TriggerEvent(BeEventType.onBeforeHit, new object[] { target, projectileResId, hurtData.ID });
        //target.TriggerEvent(BeEventType.onBeforeOtherHit, new object[] { this, hurtData });

        BeActor actorTarget = target as BeActor;
        if (actorTarget != null)
        {
            actorTarget.buffController.TriggerBuffs(BuffCondition.BEHIT, this);
        }

		//破招
		if ((battleType == BattleType.TrainingPVE && absoluteBroken) || target.sgGetCurrentState() == (int)ActionState.AS_CASTSKILL && 
            (hurtData == null || hurtData != null && hurtData.IsFriendDamage==0))//杀意波动不判定破招
		{
			TriggerEventNew(BeEventType.onBreakAction);
			
			Logger.LogWarningFormat("破招!!!!");
			buffController.TriggerBuffs(BuffCondition.BREAK_ACTION, target as BeActor);


            BeActor actor = this;


#if !LOGIC_SERVER
			if (actor != null && actor.isLocalActor)
				actor.m_pkGeActor.CreateHeadText(HitTextType.SPECIAL_ATTACK, "UI/Font/new_font/pic_break_action");
#endif

		}
	}

	public sealed override void OnBeforeGetDamage (BeEntity target, AttackResult result, bool isBackHit, int hurtID)
	{
		DealBeforeGetDamage(target, result, hurtID, isBackHit);
	}

	public sealed override void ShowMissEffect(Vec3 Pos)
	{
		if (isMainActor)
		{
			currentBeScene.currentGeScene.CreateEffect(9, Pos, GetFace());
		}
	}

	public sealed override void ShowHitEffect(Vec3 Pos, BeEntity target, int hurtID)
	{
		base.ShowHitEffect(Pos, target, hurtID);
	}

	public sealed override void OnDealHit(BeEntity pkEntity)
	{
		BeActor actor = pkEntity as BeActor;
		if (actor == null)
			return;

		//增加被击抖动
		if (actor.IsMonster())
		{
			actor.AddShock(Global.Settings.monsterBeHitShockData);
		}
		else
		{
			actor.AddShock(Global.Settings.playerBeHitShockData);
		}
	}

	public sealed override void onHitEntity (BeEntity pkEntity, VInt3 pos, int hurtID = 0, AttackResult result = AttackResult.MISS,int finalDamage = 0)
	{
		base.onHitEntity (pkEntity, pos, hurtID, result, finalDamage);

		if (result != AttackResult.MISS)
		{
			BeActor target = pkEntity as BeActor;
			if (target == null)
				return;

            // 攻击触发怪物反击
            if (null != target.GetEntityData())
            {
                if (target.GetEntityData().hitID > 0 && 
                (target.GetEntityData().type == (int)ProtoTable.UnitTable.eType.BOSS ||
                target.GetEntityData().type == (int)ProtoTable.UnitTable.eType.MONSTER ||
                target.GetEntityData().type == (int)ProtoTable.UnitTable.eType.ELITE))
                {
                    // 计算概率，触发触发效果ID
                    if (FrameRandom.Random((uint)GlobalLogic.VALUE_1000) < target.GetEntityData().hitIDRand)
                    {
                        //target.DoAttackTo(this, target.GetEntityData().hitID);
                        if (target.sgGetCurrentState() == (int)ActionState.AS_HURT && (target.GetPosition().z <= 0))
                        {
                            if (!target.stateController.HasBuffState(BeBuffStateType.FROZEN) && 
								!target.stateController.HasBuffState(BeBuffStateType.STONE) && 
								!target.stateController.HasBuffState(BeBuffStateType.STUN))
                            {
                                target.UseSkill(target.GetEntityData().hitID, true);
                            }
                        }
                    }
                }
            }
        }
    }

    public int GetSkillLevel(int skillID)
    {
        int skillLevel = 0;
        var skill = GetSkill(skillID);
        if (skill != null)
        {
            skillLevel = skill.GetLevel();
        }

        return skillLevel;
    }

    /******************************************************************/
    //非技能状态
    public bool CanJump()
    {
		if (moveZSpeed != 0)
            return false;

        ActionState currentStat = (ActionState)sgGetCurrentState();
        if (currentStat == ActionState.AS_IDLE ||
            currentStat == ActionState.AS_WALK ||
            currentStat == ActionState.AS_RUN)
        {
            return true;
        }
        return false;
    }

    public bool CanJumpBack()
    {
		if (moveZSpeed != 0)
            return false;

        ActionState currentStat = (ActionState)sgGetCurrentState();
        if (currentStat == ActionState.AS_IDLE ||
            currentStat == ActionState.AS_WALK ||
            currentStat == ActionState.AS_RUN)
        {
            return true;
        }

        if (currentStat == ActionState.AS_CASTSKILL)
        {
            var curSkill = skillController.GetCurrentSkill();
            if (curSkill != null)
            {
                bool canAbortCurSkill = curSkill.CanBePositiveAbort() || LiGuiSkillList.Contains(GetCurSkillID());//后跳打断里鬼
                if (canAbortCurSkill)
					m_pkGeActor.CreateSnapshot(Color.white, Global.Settings.snapDuration);
                return canAbortCurSkill;
            }
        }

        return false;
    }

    public void OnCurSkillPhaseFinish()
    {
        if (_skipToNextPhase != null)
            _skipToNextPhase.DealSwitchInPhaseFinish();
    }

    /// <summary>
    /// 切换到下个技能阶段
    /// </summary>
    public void SwitchToNextPahseSkill()
    {
        ((BeActorStateGraph)GetStateGraph()).ExecuteNextPhaseSkill();
    }

    #region 技能机制相关
    /// <summary>
    /// 添加技能机制
    /// </summary>
    public void AddSkillMechanism(int id, int time, int level, bool isPhaseDelete, bool isFinishDelete)
    {
        BeMechanism mechanism = AddMechanism(id, level, MechanismSourceType.NONE, null, time);
        if (isPhaseDelete)
        {
            phaseDeleteMechanismList.Add(mechanism);
        }

        if (isFinishDelete)
        {
            finishDeleteMechanismList.Add(mechanism);
        }
    }

    /// <summary>
    /// 移除技能阶段删除的机制
    /// </summary>
    public void ClearPhaseDelete()
    {
        if (_skipToNextPhase != null)
        {
            _skipToNextPhase.DeInIt();
            _skipToNextPhase = null;
        }
        ClearMechanisms(phaseDeleteMechanismList);
    }

    /// <summary>
    /// 移除技能结束删除的机制
    /// </summary>
    public void ClearFinishDeleteAll()
    {
        ClearMechanisms(finishDeleteMechanismList);
    }

    /// <summary>
    /// 清除技能列表
    /// </summary>
    private void ClearMechanisms(List<BeMechanism> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            BeMechanism mechanism = list[i];
            if (mechanism != null && mechanism.isRunning)
            {
                mechanism.Finish();
            }
        }
    }

    #endregion

    /*
    这里的buff只有是在buff中添加的机制才会传进来，所以可以确定buff的owner和机制的owner是一样的
    */
    public BeMechanism AddMechanism(int mid, int level=0, MechanismSourceType sourceType = MechanismSourceType.NONE,BeBuff buff = null, int time = 0)
	{
		if (mid > 0)
		{
            BeMechanism existMechanism = GetCannotRemoveExist(mid);
            if (existMechanism != null)
                return existMechanism;
            
            bool enablePool = true;
            
            BeMechanism mechanism = null;
            int pid = 0;
            bool isTown = CurrentBeBattle == null;
            if (isTown || !enablePool)
                mechanism = BeMechanism.Create(mid, level);
            else
            {
                mechanism = CurrentBeBattle.MechanismPool.GetMechanism(mid, level);
                pid = CurrentBeBattle.MechanismPool.GetID();
            }
                

			if (mechanism != null)
			{
                mechanism.PID = pid;
				mechanism.owner = this;
                //mechanism.attachBuff = buff;
                if (buff != null)
                    mechanism.attachBuffPID = buff.PID;
				mechanism.Init(time);
                mechanism.Start();

				mechanismList.Add(mechanism);

                mechanism.sourceType = sourceType;
                TriggerEventNew(BeEventType.onAddMechanism, new EventParam{m_Int = mid});
                return mechanism;
			}
		}

		return null;
	}

    public BeMechanism GetMechanismByPID(int pid)
    {
        for(int i=mechanismList.Count-1; i>=0; --i)
        {
            var mechanism = mechanismList[i];
            if (mechanism == null)
                continue;
            if (mechanism.isRunning && mechanism.PID == pid)
                return mechanism;
        }
        return null;
    }

    public void RemoveMechanismByPID(int pid)
    {
        RemoveMechanism(GetMechanismByPID(pid));
    }

    /// <summary>
    /// 判断标记为不能被移除的机制是否存在
    /// </summary>
    /// <returns></returns>
    private BeMechanism GetCannotRemoveExist(int id)
    {
        BeMechanism mechanism = GetMechanism(id);
        if (mechanism != null && !mechanism.canRemove)
        {
            return mechanism;
        }
        return null;
    }

    #region 机制相关内容
    /// <summary>
    /// 加载机制列表
    /// </summary>
    /// <param name="mIDs"></param>
    /// <param name="level"></param>
    /// <param name="sourceType"></param>
    public void LoadMechanisms(IList<int> mIDs, int level=0, MechanismSourceType sourceType = MechanismSourceType.NONE)
	{
        if (mIDs == null)
            return;

		for(int i=0; i<mIDs.Count; ++i)
		{
			int mid = mIDs[i];
			if (mid > 0)
			{
				AddMechanism(mid, level, sourceType);
			}
		}
	}

    /// <summary>
    /// 更新机制列表
    /// </summary>
    private void UpdateMechanismList()
    {
        var tmpList = GamePool.ListPool<BeMechanism>.Get();
        foreach(var mech in mechanismList)
        {
            if (CheckCanRemoveMechanism(mech))
                tmpList.Add(mech);
        }

        mechanismList.RemoveAll(CheckCanRemoveMechanism);

        if (CurrentBeBattle != null)
        {
            foreach(var mech in tmpList)
                CurrentBeBattle.MechanismPool.PutMechanism(mech);
        }

        _removeFromList(phaseDeleteMechanismList, tmpList);
        _removeFromList(finishDeleteMechanismList, tmpList);

        GamePool.ListPool<BeMechanism>.Release(tmpList);
    }

    private bool CheckCanRemoveMechanism(BeMechanism mechanism)
    {
        if (mechanism != null)
            return (!mechanism.isRunning && mechanism.canRemove);
        return false;
    }

    private void _removeFromList(List<BeMechanism> list, List<BeMechanism> alreadyDead)
    {
        if (list == null || alreadyDead == null)
            return;
         list.RemoveAll(item => { return alreadyDead.Contains(item); });
    }

    /// <summary>
    /// 根据机制ID列表移除机制
    /// </summary>
    /// <param name="mIDs"></param>
    public void RemoveMechanisms(IList<int> mIDs)
    {
        if (mIDs == null)
            return;
        for (int i = 0; i < mIDs.Count; ++i)
        {
            if (mIDs[i] > 0)
                RemoveMechanism(mIDs[i]);
        }
    }

    /// <summary>
    /// 根据机制Id移除机制
    /// </summary>
    /// <param name="mid"></param>
	public void RemoveMechanism(int mid)
    {
        if (mid <= 0)
            return;
        for(int i = 0; i < mechanismList.Count; i++)
        {
            BeMechanism mechanism = mechanismList[i];
            if (mechanism == null)
                continue;
            if (mechanism.mechianismID != mid)
                continue;
            RemoveMechanism(mechanism);
        }
	}

    /// <summary>
    /// 移除机制
    /// </summary>
    /// <param name="mechanism">需要移除的机制对象</param>
    public void RemoveMechanism(BeMechanism mechanism)
    {
        if (mechanism == null)
            return;
        if (!mechanism.canRemove)
            return;
        mechanism.Finish();
    }

    /// <summary>
    /// 更新机制逻辑
    /// </summary>
    /// <param name="deltaTime"></param>
	public void UpdateMechanisms(int deltaTime)
	{
		for(int i=0; i<mechanismList.Count; ++i)
		{
			mechanismList[i].Update(deltaTime);
		}
	}

    /// <summary>
    /// 更新机制逻辑 受攻速影响函数
    /// </summary>
    public void UpdateMechanismsImpactBySpeed(int deltaTime)
    {
	    for (int i = 0; i < mechanismList.Count; ++i)
	    {
		    mechanismList[i].UpdateImpactBySpeed(deltaTime);
	    }
    }
    
    /// <summary>
    /// 更新机制表现
    /// </summary>
    /// <param name="deltaTime"></param>
	public void UpdateMechanismsGraphic(int deltaTime)
	{
		for(int i=0; i<mechanismList.Count; ++i)
		{
			mechanismList[i].UpdateGraphic(deltaTime);
		}
	}
  
    /// <summary>
    /// 移除所有机制
    /// </summary>
	public void RemoveAllMechanism()
	{
        RecordEquipMechanism();

        for (int i=0; i<mechanismList.Count; ++i)
		{
            var mechanism = mechanismList[i];
            if (mechanism == null)
                continue;
            mechanism.DealDead();
            RemoveMechanism(mechanism);
		}

        UpdateMechanismList();
    }

    /// <summary>
    /// 根据ID获取机制
    /// </summary>
	public BeMechanism GetMechanism(int mid)
	{
		for(int i=0; i<mechanismList.Count; ++i)
		{
			if (mechanismList[i].mechianismID == mid)
				return mechanismList[i];
		}

		return null;
	}

    public T GetMechanism<T>(int mid) where T: BeMechanism 
    {
	    var m = GetMechanism(mid);
	    if (m == null)
		    return null;
	    return m as T;
    }
    
    public BeMechanism GetMechanismByIndex(int index)
    {
        for (int i = 0; i < mechanismList.Count; ++i)
        {
            if (mechanismList[i].GetMechanismIndex() == index)
                return mechanismList[i];
        }

        return null;
    }
    #endregion

    public void LoadMechanismBuffs(IList<int> buffInfos, int level=0, bool inTown=false, ItemProperty weaponProperty = null, bool needRecord = false)
	{
		if (buffInfos == null)
			return;

		for(int i=0; i<buffInfos.Count; ++i)
		{
			if (buffInfos[i] <= 0)
				continue;
            if (needRecord && IsProcessRecord())
            {
              ////  GetRecordServer().RecordProcess("PID:{0}-{1} LoadMechanismBuffs {2} {3}", m_iID, GetName(), buffInfos[i], GetInfo());
            }
            var data = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(buffInfos[i]);
            if (data != null && data.RelatedSkillID != 0)
                level = GetSkillLevel(data.RelatedSkillID);

            BuffInfoData buffInfo = new BuffInfoData(buffInfos[i], level);
            //指定技能等级段的所有技能改变属性
            if (data != null && data.RelatedSkillLV.Length > 0)
            {
                if (data.RelatedSkillLV.Length == 1)
                {
                    int lv = data.RelatedSkillLV[0];
                    if (lv != 0)
                    {
                        var skillDic = GetSkills();
                        var iter = skillDic.GetEnumerator();
                        while (iter.MoveNext())
                        {
                            var skill = iter.Current.Value;
                            if (skill != null && skill.skillData != null &&
                               skill.skillData.LevelLimit == lv &&
                               !buffInfo.skillIDs.Contains(iter.Current.Key))
                            {
                                buffInfo.skillIDs.Add(iter.Current.Key);
                            }
                        }
                    }
                }
                else if (data.RelatedSkillLV.Length == 2)
                {
                    int lowLv = data.RelatedSkillLV[0];
                    int highLv = data.RelatedSkillLV[1];
                    if (lowLv > 0 && lowLv < highLv)
                    {
                        var skillDic = GetSkills();
                        var iter = skillDic.GetEnumerator();
                        while (iter.MoveNext())
                        {
                            var skill = iter.Current.Value;
                            if (skill != null && skill.skillData != null &&
                                skill.skillData.LevelLimit >= lowLv &&
                                skill.skillData.LevelLimit <= highLv &&
                                !buffInfo.skillIDs.Contains(iter.Current.Key))
                            {
                                buffInfo.skillIDs.Add(iter.Current.Key);
                            }
                        }
                    }
                }
            }
            if (buffInfo.condition <= BuffCondition.NONE)
			{
				if (buffInfo.condition == BuffCondition.ENTERBATTLE && !inTown || 
					buffInfo.condition == BuffCondition.NONE)
					buffController.TryAddBuff(buffInfo, this);
			}
			else
            {
                buffController.AddTriggerBuff(buffInfo);

                if (weaponProperty != null)
                {
                    var buffInfoTmp = buffController.GetTriggerBuff(buffInfo);
                    RestoreBuffInfoCDRemain(buffInfoTmp, weaponProperty);
                }
            }

        }
	}


    public void RemoveMechanismBuffs(IList<int> buffInfos, ItemProperty weaponProperty = null)
    {
        if (buffInfos == null)
            return;

        for (int i = 0; i < buffInfos.Count; ++i)
        {
            if (buffInfos[i] <= 0)
                continue;

            BuffInfoData buffInfo = new BuffInfoData(buffInfos[i]);
            if (buffInfo.condition <= BuffCondition.NONE)
            {
                if (buffInfo.condition == BuffCondition.ENTERBATTLE || buffInfo.condition == BuffCondition.NONE)
                    buffController.RemoveBuff(buffInfo.buffID, 1, buffInfo.buffInfoID);
            }
            else
            {
                if (weaponProperty != null)
                {
                    RecordBuffInfoCDRemain(buffController.GetTriggerBuff(buffInfo), weaponProperty);
                }

                buffController.RemoveTriggerBuff(buffInfos[i]);
            }
        }
    }

    public void RecordBuffInfoCDRemain(BuffInfoData buffInfo, ItemProperty weaponProperty)
    {
        if (buffInfo == null || weaponProperty == null)
            return;

        if (buffInfo.IsCD() && buffInfo.GetCDAcc() > 0)
        {
            weaponProperty.SaveTriggerBuffCDRemain(buffInfo.buffInfoID, buffInfo.GetCDAcc());
        }
    }

    public void RestoreBuffInfoCDRemain(BuffInfoData buffInfo, ItemProperty weaponProperty)
    {
        if (buffInfo == null || weaponProperty == null)
            return;

        int cdRemain = weaponProperty.GetTriggerBuffCDRemain(buffInfo.buffInfoID);
        if (cdRemain > 0)
        {
            buffInfo.SetCDRemain(cdRemain);
        }
    }

    public void RecordEquipMechanism()
    {
        recordedEquipMechanismIDs.Clear();

        for (int i = 0; i < mechanismList.Count; ++i)
        {
            var mechanism = mechanismList[i];
            if (mechanism == null)
                continue;

            if (mechanism.sourceType == MechanismSourceType.EQUIP)
                recordedEquipMechanismIDs.Add(mechanism.mechianismID);
        }
    }

    #region 技能
    /**********************************************************************/
    //skill
    /*
    skillInfos：（id，level）
    */

#if UNITY_EDITOR
    public static bool useNewLoadingSkill = true;
#endif

    public void LoadSkillConfig(Dictionary<int, int> skillInfos, bool loadConfigBySkills = false, int resID=0)
    {
        List<string> skillNames = null;

        if (loadConfigBySkills)
        {
            //加载技能配置文件
            skillNames = new List<string>();
            
            Dictionary<int, int>.Enumerator it = skillInfos.GetEnumerator();

            while (it.MoveNext())
            {
                int skillID = (int)it.Current.Key; 
                var data = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(skillID);
                if (data != null)
                {
                    var tmp = data.EnglishName;
                    if (!skillNames.Contains(tmp))
                        skillNames.Add(tmp);
                }
            }
        }

        if (m_pkGeActor != null)
        {
            // if (m_pkGeActor.m_ActorDesc.resID == 59018)//玩法测试代码
            // {
            //     m_pkGeActor.LoadSkillConfig(m_cpkEntityInfo1, false, skillNames);
            //     m_pkGeActor.LoadSkillConfig(m_cpkEntityInfo2, false, skillNames, 0, true);
            // }
            // else
            // {
                m_pkGeActor.LoadSkillConfig(m_cpkEntityInfo, loadCommonSkill, skillNames, resID);
            // }
        }

		//武器对应的技能配置文件加载
		if (attachmentproxy != null)
		{
			if (attachmentproxy.needReplace)
			{
                Dictionary<int, int> tagMap = new Dictionary<int, int>(); //武器Tag字典 Key:tag,Value:1
                Dictionary<int, List<int>> tagWeaponTypeMap = new Dictionary<int, List<int>>(); //武器Type字典 Key:tag,Value:Type的列表 tag对type可能是一对多的 故用List

                tagMap.Add(attachmentproxy.tag, 1);
                tagWeaponTypeMap.Add(attachmentproxy.tag, new List<int>(new int[] { attribute.GetWeaponType() }));
                //////////////////////////////////////////////////////////////////////////////
                if (attribute != null)
                {
                    var tags = new List<int>(); 
                    var types = new List<int>();
                    attribute.GetBackupWeaponTypesAndTags(types,tags);
                    for (int i = 0; i < tags.Count; ++i)
                    {
                        if (!tagMap.ContainsKey(tags[i]))
                        {
                            tagMap.Add(tags[i], 1);
                            tagWeaponTypeMap.Add(tags[i], new List<int>(new int[] { types[i] }));
                        }
                        else
                        {
                            if (!tagWeaponTypeMap[tags[i]].Contains(types[i]))
                            {
                                tagWeaponTypeMap[tags[i]].Add(types[i]);
                            }
                        }
                    }

                    foreach(var tag in tagMap)
                    {
                        if (m_pkGeActor != null)
                        {
                            List<int> tempType = null;
                            if (tagWeaponTypeMap.ContainsKey(tag.Key))
                            {
                                tempType = tagWeaponTypeMap[tag.Key];
                            }
                            if (this.CurrentBeBattle.HasFlag(BattleFlagType.SKILL_LOADING_TYPE))
                            {
                                m_pkGeActor.LoadWeaponRelatedConfig(m_cpkEntityInfo, tag.Key, skillNames);
                            }
                            else
                            {
#if UNITY_EDITOR
                                if (useNewLoadingSkill)
                                {
                                    m_pkGeActor.LoadWeaponRelatedConfig(m_cpkEntityInfo, tag.Key, skillNames, tempType);
                                }
                                else
                                {
                                    m_pkGeActor.LoadWeaponRelatedConfig(m_cpkEntityInfo, tag.Key, skillNames);
                                }
#else
                                m_pkGeActor.LoadWeaponRelatedConfig(m_cpkEntityInfo, tag.Key, skillNames, tempType);
#endif
                            }
                        }
                    }
                }
                //////////////////////////////////////////////////////////////////////////////

            }
            else {
				if (m_pkGeActor != null)
					m_pkGeActor.LoadWeaponRelatedConfig(m_cpkEntityInfo, defaultWeaponTag, skillNames);
			}

            
        }

        /*foreach(var skillinfo in skillInfos)
        {
            int skillID = skillinfo.Key;
            int skillLevel = skillinfo.Value;

			LoadOneSkill(skillID, skillLevel);
        }*/

        if (attribute != null)
        {
            int weaponTag = 0;
            int weaponType = 0;
            attribute.GetWeaponTagAndWeaponType(ref weaponTag, ref weaponType);

            if (weaponTag == 0)
                weaponTag = defaultWeaponTag;
            if (weaponType == 0)
                weaponType = defaultWeaponType;

            m_cpkEntityInfo.SetWeaponInfo(weaponTag, weaponType);
        }
    }

    public bool HasSkill(int skillID)
    {
		return skillController.HasSkill(skillID);
    }

    public BeSkill GetSkill(int skillID)
    {
        return skillController.GetSkill(skillID);
    }

    public int GetCastSkillID()
    {
        return skillController.GetCurSkillID();
    }

    public void ResetSkillCoolDown()
    {
        skillController.ResetSkillCoolDown();
    }

    //设置当前的技能阶段数组
    public void SetCurrSkillPhase(int[] phase)
    {
       skillController.SetCurrentSkillPhases(phase);
    }

    public bool CanUseSkill(int skillID)
    {
        // marked by ckm
        var willUseSkillParam = owner.TriggerEventNew(BeEventType.onWillUseSkill, new EventParam(){ m_Int = skillID});
        if (willUseSkillParam.m_Int < 0) return false;

        return skillController.CanUseSkill(skillID);
    }

    public bool UseSkill(int skillID, bool force = false)
    {
        return skillController.UseSkill(skillID, force);
    }

    public void SetSkillWalkMode(SkillWalkMode mode, VFactor walkSpeed, VFactor walkSpeed2 = default(VFactor))
    {
        var skill = skillController.GetCurrentSkill();
        if (skill == null)
            return;

        if (mode != SkillWalkMode.FORBID && !skill.walk)
        {
            mode = SkillWalkMode.FORBID;
        }

        skillFreeTurnFace = false;
        keepXSkillSpeed = false;

        bool hasLockZ = GetStateGraph().CurrentStateHasTag((int)AStateTag.AST_LOCKZ);
        skill.CurSkillWalkMode = (int)mode;
        switch (mode)
        {
            case SkillWalkMode.FORBID:
                GetStateGraph().ResetCurrentStateTag();
                GetStateGraph().SetCurrentStateTag((int)AStateTag.AST_BUSY | (int)AStateTag.AST_CONTROLED);
                onlyMoveFacedDir = false;
				onlyMoveFaceDirOpposite = false;
                skillFreeTurnFace = false;
                SetMoveSpeedXRate(VFactor.one);
                SetMoveSpeedYRate(VFactor.one);
                break;
            case SkillWalkMode.FREE:
            case SkillWalkMode.FREE2:
                GetStateGraph().ResetCurrentStateTag();
                SetMoveSpeedXRate(walkSpeed);
                SetMoveSpeedYRate(walkSpeed);
                onlyMoveFacedDir = false;
				onlyMoveFaceDirOpposite = false;

                break;
            case SkillWalkMode.FACEDIR:
                GetStateGraph().ResetCurrentStateTag();
                SetMoveSpeedXRate(walkSpeed);
                SetMoveSpeedYRate(VFactor.zero);
				onlyMoveFaceDirOpposite = false;
                onlyMoveFacedDir = true;

                break;
			case SkillWalkMode.FACEDIR_OPPOSITE:
				GetStateGraph().ResetCurrentStateTag();
				SetMoveSpeedXRate(walkSpeed);
				onlyMoveFaceDirOpposite = true;
				onlyMoveFacedDir = false;

                break;
            case SkillWalkMode.CHANGE_DIR:
                GetStateGraph().ResetCurrentStateTag();
                GetStateGraph().SetCurrentStateTag((int)AStateTag.AST_BUSY | (int)AStateTag.AST_CONTROLED);

                if (m_vkMoveCmd[(int)CommandMove.COMMAND_MOVE_DIRECTION])
                {
                    VInt x, y;

                    GetMoveSpeedFromDirection(out x, out y);

                    if (x > 0 && GetFace() || x < 0 && !GetFace())
                    {
                        SetFace(!GetFace(), true);
                    }
                }


                break;
            case SkillWalkMode.FREE_AND_CHANGEDIR:
                GetStateGraph().ResetCurrentStateTag();
                skillFreeTurnFace = true;

                if (isMainActor && !hasDoublePress)
                    ChangeRunMode(true);

                SetMoveSpeedXRate(walkSpeed);
                SetMoveSpeedYRate(walkSpeed);
				onlyMoveFaceDirOpposite = false;
				onlyMoveFacedDir = false;

                break;
			case SkillWalkMode.X_ONLY:
				GetStateGraph().ResetCurrentStateTag();
				onlyMoveFacedDir = false;
				onlyMoveFaceDirOpposite = false;
				SetMoveSpeedXRate(walkSpeed);
				SetMoveSpeedYRate(VFactor.zero);
				break;
			case SkillWalkMode.Y_ONLY:
				GetStateGraph().ResetCurrentStateTag();
				onlyMoveFacedDir = false;
				onlyMoveFaceDirOpposite = false;
				SetMoveSpeedXRate(VFactor.zero);
				SetMoveSpeedYRate(walkSpeed);
				break;
            case SkillWalkMode.X_YControl:
                GetStateGraph().ResetCurrentStateTag();
				onlyMoveFacedDir = false;
				onlyMoveFaceDirOpposite = false;
                keepXSkillSpeed =true;
				SetMoveSpeedXRate(walkSpeed);
                break;
            case SkillWalkMode.FORBID2:
                GetStateGraph().ResetCurrentStateTag();
                onlyMoveFacedDir = false;
                onlyMoveFaceDirOpposite = false;
                SetMoveSpeedXRate(VFactor.zero);
                SetMoveSpeedYRate(VFactor.zero);
                break;
            case SkillWalkMode.XYDiffRate:
                GetStateGraph().ResetCurrentStateTag();
                skillFreeTurnFace = true;

                if (isMainActor && !hasDoublePress)
                    ChangeRunMode(true);

                SetMoveSpeedXRate(walkSpeed);
                SetMoveSpeedYRate(walkSpeed2);
                onlyMoveFaceDirOpposite = false;
                onlyMoveFacedDir = false;

                break;
        }
    }

    public void StartSkill(int skillID)
    {
        //Logger.LogError("StartSkill ID:" + skillID);

        var skill = GetSkill(skillID);
        if (skill != null)
        {
            TriggerEventNew(BeEventType.onCastSkill, new EventParam() { m_Int = skillID, m_Obj = this });

            if (skill.GetSkillSpeedEffectType() == SkillSpeedEffectType.ATTACK_SPEED)
                skill.SetSkillSpeed(GetEntityData().GetAttackSpeed());
            else if (skill.GetSkillSpeedEffectType() == SkillSpeedEffectType.SPELL_SPEED)
                skill.SetSkillSpeed(GetEntityData().GetSpellSpeed());
            else
                skill.SetSkillSpeedWithSkillData();
            if(currentBeScene != null)
				AttackProcessId = currentBeScene.GenAttackProcessID();
            skill.Start();
	
            SkillWalkMode walkMode = (SkillWalkMode)skill.GetWalkMode();

            SetSkillWalkMode(walkMode, skill.GetWalkSpeedRate());
        }
    }

    public void FinishSkill(int skillID)
    {
        //Logger.LogError("FinishSkill ID:" + skillID);
        var skill = GetCurrentSkill();
        if (skill == null)
            return;

        skill.Finish();

        bool cancel = false;
		if (skill.charge || actionLooped)
            cancel = true;

        CancelSkill(skillID, cancel);
    }

    //技能中断
    public void CancelSkill(int skillID, bool canceled=true)
    {
        if (isCancelSkill)
            return;

        AttackProcessId = 0;
        isCancelSkill = true;
        skillAttackScale = VFactor.one;
        SetMoveSpeedXRate(VFactor.one);
        SetMoveSpeedYRate(VFactor.one);
        onlyMoveFacedDir = false;
		onlyMoveFaceDirOpposite = false;
        keepXSkillSpeed = false;
		buffController.ClearPhaseDelete();
		buffController.ClearFinishDelete();
        buffController.ClearFinishDeleteAll();
        ClearPhaseDeleteAudio();

        ClearPhaseDelete();
        ClearFinishDeleteAll();
        ClearSkillFinishDeleteAudio();

        StopSpellBar(eDungeonCharactorBar.Power);
		StopSpellBar(eDungeonCharactorBar.Sing);

        var skill = GetSkill(skillID); //GetCurrentSkill();

        if (skill != null && !skill.IsCanceled() && canceled)
        {
           //Logger.LogErrorFormat("CancelSkill ID:{0} time:{1}", skillID, Time.realtimeSinceStartup * GlobalLogic.VALUE_1000);
           if(m_pkGeActor != null)
                m_pkGeActor.Clear((int)GeEntity.GeEntityRes.EffectUnique | (int)GeEntity.GeEntityRes.EffectMultiple);
            skill.Cancel();
            
        }
        grabController.EndGrab();
        isCancelSkill = false;
    }

    public void SetCurSkillID(int skillID)
    {
        skillController.SetCurSkillID(skillID);
    }

    public override int GetCurSkillID()
    {
        return skillController.GetCurSkillID();
    }

    public BeSkill GetCurrentSkill()
    {
        return skillController.GetCurrentSkill();
    }

    public void SetPreSkillID(int skillID)
    {
        skillController.SetPreSkillID(skillID);
    }

    public int GetPreSkillID()
    {
        return skillController.GetPreSkillID();
    }

    public void ChangeAnimation(string aniName, bool isWalk=false)
    {
        float aniSpeed = 1.0f;
        var curSkill = GetCurrentSkill();
        if (curSkill != null)
            aniSpeed = curSkill.GetSkillSpeedFactor().single;

        if (isWalk)
        {
            aniSpeed = IsRunning() ? runSpeedFactor : walkSpeedFactor;
        }

        _ChangeAnimation(aniName, aniSpeed);
    }

    public void DealSkillEvent(EventCommand skillEvent)
    {
        var skill = GetCurrentSkill();
        if (skill != null)
        {
            skill.DealSkillEvents(skillEvent);
        }
    }

	public void PlaySkillAction(int skillPhaseID)
	{
		string tmp = GetActionNameBySkillID(skillPhaseID);

		if (tmp == null)
		{
			return;
		}

		float aniSpeed = 1.0f;
		var curSkill = GetCurrentSkill();
		if (curSkill != null) {
			aniSpeed = curSkill.GetSkillSpeedFactor().single;
		}

		PlayAction(tmp, aniSpeed);
	}

#endregion

	//public void EquipFashion()

#region AI
    /**********************************************************************/
    //professtionid暂时为战斗包的ID

	public void _addBuff(List<BuffInfoData> buffInfoList, bool needRecord = false, string reason = "")
	{
		if (buffInfoList != null)
		{
			for(int i=0; i<buffInfoList.Count; ++i)
			{
				var buffInfo = buffInfoList[i];
                if (IsProcessRecord() && needRecord)
                {
                    GetRecordServer().RecordProcess("PID:{0}-{1} {2} _addBuff {3} {4}", m_iID, GetName(), reason, buffInfoList[i], GetInfo());
                    var pos = GetPosition();
                    GetRecordServer().Mark(0x87797771, new int[]{
                                                        buffInfo.buffInfoID,
                                                        pos.x, pos.y, pos.z,
                                                        moveXSpeed.i, moveYSpeed.i, moveZSpeed.i,
                                                        GetFace() ? 1 : 0,
                                                        attribute.GetHP(),
                                                        attribute.GetMP(),
                                                        m_iID, m_kStateTag.GetAllFlag(),
                                                        attribute.battleData.attack.ToInt()}, GetName(), reason);
                    // Mark:0x87797771 [AddBuff] BuffID:{0} Pos:{1},{2},{3} Speed:{4},{5},{6} Face:{7}, HP:{8}, MP:{9}, PID:{10} StateTag:{11}, Attack:{12}, Name:{13}, Reason:{14}
                }
                if (buffInfo != null && buffInfo.condition == BuffCondition.NONE)
				{
					buffController.TryAddBuff(buffInfo, this);
				}
			}
		}
	}

    public void InitData(
        int level,
        int fightID,
        Dictionary<int, int> skillLevelInfo,
        string ai = "",
        int proid = 0,
        List<ItemProperty> equips = null,
        List<Battle.DungeonBuff> buffList = null,
        int weaponStrengthenLevel = 0,
        List<BuffInfoData> rankBuffList = null, //段位带来的buff,
        PetData petData = null,
        List<ItemProperty> sideEquips = null,
        Dictionary<int, string> avatar = null,
        bool isShowWeapon = false,
        bool isAIRobot = false,
        bool ispvp = false,
        SkillSystemSourceType skillSourceType = SkillSystemSourceType.None
    )
    {
        //要加上这个职业的天生携带技能
        var bornSkills = new List<int>(TableManager.instance.GetBornSkills(proid));
        professionID = proid;

#region 这里对有所属技能ID的技能进行删选
        var noNeedLoadSkills = GamePool.ListPool<int>.Get();
        
        for(int i=0; i<bornSkills.Count; ++i)
        {
            var skillid = bornSkills[i];
            var skillData = TableManager.instance.GetTableItem<ProtoTable.SkillTable>(skillid);
            if (skillData == null)
                continue;

            if (skillData.MasterSkillID != 0 && !skillLevelInfo.ContainsKey(skillData.MasterSkillID))
                noNeedLoadSkills.Add(skillid);
        }

        bornSkills.RemoveAll(item => { return noNeedLoadSkills.Contains(item); });

        GamePool.ListPool<int>.Release(noNeedLoadSkills);
#endregion

        //决斗场机器人的技能直接升到所能达到的最大等级
        if (isAIRobot)
            skillLevelInfo = BeUtility.GetPKRobotSkillLevel(skillLevelInfo, level);
        for (int i=0; i<bornSkills.Count; ++i)
        {
			var skillid = bornSkills[i];
            if (!skillLevelInfo.ContainsKey(skillid))
            {
                skillLevelInfo.Add(skillid, 1);
            }
        }

        //吃鸡模式需要加载吃鸡特定的普攻ID
        if (BattleMain.IsModeChiji(battleType))
        {
            var jobTable = TableManager.instance.GetTableItem<JobTable>(proid);
            if(jobTable != null)
            {
                int chijiAttackId = jobTable.ChijiNormalAttackID;
                if (chijiAttackId > 0 && !skillLevelInfo.ContainsKey(chijiAttackId))
                {
                    skillLevelInfo.Add(chijiAttackId, 1);
                }
            }
        }

        List<int> passiveBuffs = new List<int>();

        //计算要加上的被动技能, 
        // GetPassiveSkills这个接口获取的其实是辅助技能集合，只是辅助技能都是被动技能。1.5项目没有辅助技能了，把相关功能屏蔽掉 by wangbo 2020.08.13
        //         var passiveSkills = TableManager.instance.GetPassiveSkills(proid);
        //         for(int i=0; i<passiveSkills.Count; ++i)
        //         {
        //             var skillid = passiveSkills[i];
        //             var skillData = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(skillid);
        //             if (skillData != null && skillData.LevelLimit <= level)
        //             {
        //                 for(int j=0; j<skillData.BuffInfoIDs.Count; ++j)
        //                 {
        //                     int buffInfoID = skillData.BuffInfoIDs[j];
        //                     var buffInfoData = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(buffInfoID);
        //                     if (buffInfoData == null)
        //                         continue;
        //                     if (buffInfoData.BuffID > 0)
        //                         passiveBuffs.Add(buffInfoData.BuffID);
        //                 }
        //             }
        //         }

        if (skillLevelInfo.ContainsKey(Global.REVIVE_SHOCK_SKILLID))
        {
            skillLevelInfo[Global.REVIVE_SHOCK_SKILLID] = level;
        }

		if (equips != null)
		{
            this.equips = equips;
            List<int> equipments = new List<int>();
			for(int i=0; i<equips.Count; ++i)
			{
				equipments.Add(equips[i].itemID);
			}

			//套装
			suitProperty = EquipSuitDataManager.GetInstance().GetEquipSuitPropByIDs(equipments);

			//护甲精通
			masterProperty = EquipMasterDataManager.GetInstance().GetEquipMasterPropByIDs(proid, equipments);

			if (suitProperty != null)
				equips.Add(suitProperty);
			if (masterProperty != null)
				equips.Add(masterProperty);
		}

        attribute = BeEntityData.GetActorAttribute(level, fightID, equips, skillLevelInfo, passiveBuffs,sideEquips,this);

        //处理决斗场机器人装备强化加成
        if (isAIRobot && equips != null)
        {
            for(int i=0;i< equips.Count; i++)
            {
                attribute.SetAIRobotData(equips[i]);
            }
        }

        //attribute.owner = this;
        attribute.professtion = TableManager.GetInstance().GetJobIDByFightID(fightID);
		if (fightID!=0 && attribute.professtion != 0)
		{
			var data = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(attribute.professtion);
            if (data != null)
            {
                attribute.jobAttribute = data.JobAttribute;
                if (BattleMain.IsModeChiji(battleType) && data.ChijiNormalAttackID > 0)
                {
                    attribute.normalAttackID = data.ChijiNormalAttackID;
                }
                else if(data.NormalAttackID > 0)
                {
                    attribute.normalAttackID = data.NormalAttackID;
                }
            }
        }

        //武器模型替换
        ChangeWeaponModle(weaponStrengthenLevel);
        //处理时装武器
        if (isShowWeapon)
            InitFashionWeapon(equips,professionID);

        //加载技能
        skillController.LoadSkill(skillLevelInfo, fightID>0 || ai=="apc");

		//装备技能加载
		if (equips != null)
		{
			List<int> mechanismList = new List<int>();
			List<int> mechanismBuffList = new List<int>();

            // 包括套装属性
            // 在公平竞技场的准备房间里，角色身上的时装对技能的加成是不计算在内的（时装通过buff机制实现技能等级加成的）
            // SkillSystemSourceType仅用于战斗外给技能等级加成的计算，不用于战斗内，战斗内都是用单局类型判断
            if (skillSourceType != SkillSystemSourceType.FairDuel)
            {
                for (int i = 0; i < equips.Count; ++i)
                {
                    if (BattleMain.IsModePvP(battleType) || (ispvp && ai == "town"))
                    {
                        if (equips[i].attachPVPBuffIDs != null && equips[i].attachPVPBuffIDs.Count > 0)
                        {
                            mechanismBuffList.AddRange(equips[i].attachPVPBuffIDs);
                        }

                        if (equips[i].attachPVPMechanismIDs != null && equips[i].attachPVPMechanismIDs.Count > 0)
                        {
                            mechanismList.AddRange(equips[i].attachPVPMechanismIDs);
                        }
                    }
                    else
                    {
                        if (equips[i].attachBuffIDs != null && equips[i].attachBuffIDs.Count > 0)
                        {
                            mechanismBuffList.AddRange(equips[i].attachBuffIDs);
                        }

                        if (equips[i].attachMechanismIDs != null && equips[i].attachMechanismIDs.Count > 0)
                        {
                            mechanismList.AddRange(equips[i].attachMechanismIDs);
                        }
                    }
                }
            }   

            Dictionary<int, string> fashions = new Dictionary<int, string>();
            string headWearPath = string.Empty;
            bool oldFashiong = false;

            // 这里对时装的处理，只是处理模型外观上的规则，不计算技能等级的加成
            for (int i=0; i<equips.Count; ++i)
			{
				var equipData = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(equips[i].itemID);
				if (equipData != null)
				{
					if (equipData.SubType>=ProtoTable.ItemTable.eSubType.FASHION_HAIR && 
						equipData.SubType<=ProtoTable.ItemTable.eSubType.FASHION_EPAULET)
					{
						var resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(equipData.ResID);
						if (resData != null)
						{
                            if (equipData.SubType == ProtoTable.ItemTable.eSubType.FASHION_HEAD)
                            {
                                if (!resData.newFashion)
                                {
                                    oldFashiong = true;
                                }
                            }
                            if (equipData.SubType == ProtoTable.ItemTable.eSubType.FASHION_EPAULET)
                            {
                                headWearPath = resData.ModelPath;
                                continue;
                            }
                            fashions.Add((int)equipData.SubType, resData.ModelPath);
						}
					}
				}
			}
            if (!oldFashiong && !string.IsNullOrEmpty(headWearPath))
            {
                fashions.Add((int)ProtoTable.ItemTable.eSubType.FASHION_EPAULET, headWearPath);
            }

            //加载机制
            if (mechanismList != null)
			{
				LoadMechanisms(mechanismList, 0, MechanismSourceType.EQUIP);
            }

            // 时装对技能等级的加成要视战斗组具体的实现方式来定，目前时装对技能等级的加成是走buff机制的，
            // 所以查时装对技能的数值影响，要在这里去看buff的数值加成是否正确,
            // buff表或者buff信息表会关联对应的技能id，在BeBuff类的changeOneSkillAttr()函数里处理对应的等级加成
            if (mechanismBuffList != null)
			{
                LoadMechanismBuffs(mechanismBuffList, 0, ai == "town", null, true);
            }

			//换时装
			if (m_pkGeActor != null)
            {
                if (avatar != null && avatar.Count > 0)
                {
                    m_pkGeActor.EquipFashions(avatar);
                } 
                else
                {
                    m_pkGeActor.EquipFashions(fashions);
                }   
            }       
		}

		//段位buff代入
		if (rankBuffList != null)
		{
			_addBuff(rankBuffList, true, "rankBuffList");
		}

		//宠物
		if (petData != null && petData.buffs != null && petData.buffs.Count > 0)
		{
			_addBuff (petData.buffs, true, "petData.buffs");
		}

			
        // 城镇Buff带入
        if (buffList != null)
        {
            int battleBuffTime = GlobalLogic.VALUE_3600 * GlobalLogic.VALUE_1000;
            for (int i = 0; i < buffList.Count; ++i)
            {
                BeBuff buff = null;
                if (IsProcessRecord())
                {
                    GetRecordServer().RecordProcess("PID:{0}-{1} townbuffList {2} {3}", m_iID, GetName(), buffList[i], GetInfo());
                    var pos = GetPosition();
                    GetRecordServer().Mark(0x8779779, new int[]
                                                {
                                                    m_iID,
                                                    buffList[i].id,
                                                    pos.x, pos.y, pos.z,
                                                    moveXSpeed.i,
                                                    moveYSpeed.i,
                                                    moveZSpeed.i,
                                                    GetFace() ? 1 : 0,
                                                    attribute.GetHP(),
                                                    attribute.GetMP(),
                                                    m_kStateTag.GetAllFlag(),
                                                    attribute.battleData.attack.ToInt()
                                                },
                                                GetName());
                    // Mark:0x8779779 PID:{0}-{12} townbuffList {1} pos: ({2},{3},{4}) speed: ({5},{6},{7}) face:{8} hp:{9} mp:{10} tag:{11} attack:{12}
                }
                if (buffList[i].type == Battle.DungeonBuff.eBuffDurationType.Town
                    || buffList[i].type == Battle.DungeonBuff.eBuffDurationType.SpecialTown)
                {
                    int time = IntMath.Float2Int(buffList[i].lefttime * GlobalLogic.VALUE_1000);
                    if (time > int.MaxValue || time < 0)
                        time = battleBuffTime;
                    buff = buffController.TryAddBuff((int)buffList[i].id, time);
                }
                else if (buffList[i].type == Battle.DungeonBuff.eBuffDurationType.Battle)
                {
					buff = buffController.TryAddBuff((int)buffList[i].id, battleBuffTime);
                }
                else if(buffList[i].type == Battle.DungeonBuff.eBuffDurationType.OnlyUseInBattle && owner.CurrentBeScene != null)
                {
                    buff = buffController.TryAddBuff((int)buffList[i].id, battleBuffTime);
                }
                if (buff != null)
                {
                    buff.passive = true;

                    if (isLocalActor)
                    {
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BattleBuffAdded, buff.buffID, this);
                    }
                }
            }
        }

		skillController.PostInitSkills(ai == "town");
		PostInit();
    }

    private void RemoveSuitProperty()
    {
        if (suitProperty != null)
        {
            List<int> mechanismList = new List<int>();
            List<int> mechanismBuffList = new List<int>();

            if (BattleMain.IsModePvP(battleType))
            {
                if (suitProperty.attachPVPBuffIDs != null && suitProperty.attachPVPBuffIDs.Count > 0)
                    mechanismBuffList.AddRange(suitProperty.attachPVPBuffIDs);
                if (suitProperty.attachPVPMechanismIDs != null && suitProperty.attachPVPMechanismIDs.Count > 0)
                    mechanismList.AddRange(suitProperty.attachPVPMechanismIDs);
            }
            else
            {
                if (suitProperty.attachBuffIDs != null && suitProperty.attachBuffIDs.Count > 0)
                    mechanismBuffList.AddRange(suitProperty.attachBuffIDs);
                if (suitProperty.attachMechanismIDs != null && suitProperty.attachMechanismIDs.Count > 0)
                    mechanismList.AddRange(suitProperty.attachMechanismIDs);
            }

            if (mechanismList != null)
            {
                RemoveMechanisms(mechanismList);
            }

            if (mechanismBuffList != null)
            {
                RemoveMechanismBuffs(mechanismBuffList);
            }
        }
    }

    private void ReloadSuitProperty(ItemProperty newWeapon,ItemProperty oldWeapon)
    {
        if (equips != null)
        {
            equips.RemoveAll(f =>
            {
                return f.guid == oldWeapon.guid;
            });

            if (newWeapon != null)
                equips.Add(newWeapon);

            List<int> equipments = new List<int>();
            for (int i = 0; i < equips.Count; ++i)
            {
                equipments.Add(equips[i].itemID);
            }
            List<int> mechanismList = new List<int>();
            List<int> mechanismBuffList = new List<int>();
            //套装
            suitProperty = EquipSuitDataManager.GetInstance().GetEquipSuitPropByIDs(equipments);
            if (suitProperty != null)
            {
                if (BattleMain.IsModePvP(battleType))
                {
                    if (suitProperty.attachPVPBuffIDs != null && suitProperty.attachPVPBuffIDs.Count > 0)
                        mechanismBuffList.AddRange(suitProperty.attachPVPBuffIDs);
                    if (suitProperty.attachPVPMechanismIDs != null && suitProperty.attachPVPMechanismIDs.Count > 0)
                        mechanismList.AddRange(suitProperty.attachPVPMechanismIDs);
                }
                else
                {
                    if (suitProperty.attachBuffIDs != null && suitProperty.attachBuffIDs.Count > 0)
                        mechanismBuffList.AddRange(suitProperty.attachBuffIDs);
                    if (suitProperty.attachMechanismIDs != null && suitProperty.attachMechanismIDs.Count > 0)
                        mechanismList.AddRange(suitProperty.attachMechanismIDs);
                }
            }

            //加载机制
            if (mechanismList != null)
            {
                LoadMechanisms(mechanismList, 0, MechanismSourceType.EQUIP);
            }

            if (mechanismBuffList != null)
            {
                LoadMechanismBuffs(mechanismBuffList, 0, false);
            }

        }
    }


    public void PostInit()
	{
		if (IsMonster())
		{
			
		}
	}

	public void ReplaceWeapon(int resID, int tag, int strengthenLevel=0)
	{
		var resData = TableManager.GetInstance().GetTableItem<ResTable>(resID);
		if (resData != null && attachmentproxy != null)
		{
			SetDefaultHitSFX(resData.WeaponHitSFX);
			attachmentproxy.SetWeaponReplace(
				resData.ModelPath,
				resData.ActionConfigPath[0], 
				tag,
				strengthenLevel
			);
		}
	}

	public void InitAI(BeAIManager manager)
    {
		aiManager = manager;
		aiManager.Init(this);
    }

	public void StartAI(BeActor target, bool reduceSpeed = true)
    {
		if (aiManager.IsRunning())
			return;


        hasAI = true;
        ChangeRunMode(false);

		if (aiManager != null)
		{
			aiManager.reduceSpeed = reduceSpeed;
		}

		var entityData = GetEntityData();
		if (entityData != null && reduceSpeed)
        {
			VFactor factor = VFactor.NewVFactorF(2f,1000);
			if (aiManager.isAPC)
				factor = VFactor.NewVFactorF(1.5f,1000);

			//!!
		    //walkSpeed.x /= factor;
            //walkSpeed.y /= factor;
			int zspeed = walkSpeed.z;
			walkSpeed = IntMath.Divide(walkSpeed,factor.den,factor.nom);
			walkSpeed.z = zspeed;

			//runSpeed.x /= factor;
			//runSpeed.y /= factor;
			zspeed = runSpeed.z;
			runSpeed = IntMath.Divide(runSpeed,factor.den,factor.nom);
			runSpeed.z = zspeed;
        }

        UpdateAITarget(target);

        aiManager.Start();
    }

    public bool IsComboSkill(int skillID)
    {
        BDEntityActionInfo actionInfo = GetActionInfoBySkillID(skillID);
        BeSkill skill = GetSkill(skillID);
        if (actionInfo != null && skill != null)
            return  skill.comboSkillSourceID != 0;
        return false;
    }

    public void UpdateAITarget(BeActor target)
    {
        if (target == null)
        {
			//aiManager.FindTarget(GlobalLogic.VALUE_100);
            aiManager.FindTarget(new VInt(aiManager.sight / GlobalLogic.VALUE_1000)); 
        }
        else
        {
            aiManager.SetTarget(target);
        }
    }
#endregion

	public void UseAdjustBalance()
	{
        var data = GetEntityData();

        if (data.adjustBalance)
            return;
        data.adjustBalance = true;
        //天平下提升3%的命中率
        data.SetAttributeValue(AttributeType.dex, TableManager.instance.gst.pkHitRateAdd, true);
	}

    public void AddStateBar(string text, int duration)
    {
        if (m_pkGeActor != null)
        {
            m_pkGeActor.CreateStateBar(text, duration);
        }
    }

    public void RemoveStateBar()
    {
        if (m_pkGeActor != null)
        {
            m_pkGeActor.RemoveStateBar();
        }
    }

#region 保护机制
    /****************************************************************************/

    public void UseProtect()
	{
		protectManager.SetEnable(true);
	}

    public void ClearProtect()
    {
        protectManager.ClearUp();
    }

    public void UpdateProtect(int delta)
	{
		if (protectManager.IsEnable())
		{
			protectManager.Tick(delta);
		}
	}

	public sealed override void OnHurt(int value, BeEntity attacker, HitTextType curHitTextType)
    {
		if (protectManager.IsEnable() && curHitTextType != HitTextType.FRIEND_HURT)
		{
			protectManager.Update(value);
		}
    }

    public void UseActorData()
    {
        actorData.SetEnable(true);
    }

    public void UpdateActorData(int delta)
    {
        if (actorData.IsEnable())
        {
            actorData.Update(delta);
        }

        actorData.UpdateLogic(delta);
    }

#endregion
	public SpellBar StartSpellBar(eDungeonCharactorBar type, int time, bool autodelete = true, string text="", bool reverse=false)
    {
        var bar = _FindSpellBar(type);
        if (null == bar)
        {
            var com = m_pkGeActor.CreateBar(type);

			if (com != null)
			{
				com.SetBarType(type);
				com.SetText(text);

			}

            bar = new SpellBar
            {
                type = type
            };
            if (com != null)
			{
				bar.com = com as ComDungeonCharactorHeadBar;
			}

            mBars.Add(bar);
        }
		else {
			if (type == eDungeonCharactorBar.Continue)
			{
				bar.reverse = false;
				bar.duration = time;
				bar.active = true;
				bar.autodelete = autodelete;
				return bar;
			}
				
		}

        if (null != bar)
        {
            bar.acc = 0;
            bar.duration = time;
            bar.active = true;
            bar.autodelete = autodelete;
			bar.autoAcc = true;
			bar.reverseAutoAcc = true;

			if (reverse)
			{
				bar.acc = bar.duration;
				bar.reverse = true;
			}
        }

		return bar;
    }
    
	public void UpdateSpellBarsGraphic(int delta)
	{
#if !LOGIC_SERVER
		for (int i = 0; i < mBars.Count; ++i)
		{
			var bar = mBars[i];
			if (bar.active)
			{
                //if (bar.acc >= bar.duration)
                //{
                //    bar.acc = bar.duration;
                //}

                if (bar.acc <= bar.duration && bar.acc > 0)
				{
                    bool show = true;
                    if (bar.type == eDungeonCharactorBar.Continue && !isLocalActor)
                        show = false;
                    m_pkGeActor.SetBar(bar.type, 1.0f * bar.acc / bar.duration, show);
                    m_pkGeActor.SetCdTime(bar.type, bar.acc/1000.0f, show);
                }
			}
		}
#endif
	}

    public void UpdateSpellBarsWithActor(int delta)
    {
		bool bNeedRemove = false;

        for (int i = 0; i < mBars.Count; ++i)
        {
			var bar = mBars[i];
            if (bar.type == eDungeonCharactorBar.DunFu ||
                bar.type == eDungeonCharactorBar.DunFuCD ||
                bar.type == eDungeonCharactorBar.Loop ||
                bar.type == eDungeonCharactorBar.Power ||
                bar.type == eDungeonCharactorBar.RayDrop ||
                bar.type == eDungeonCharactorBar.Sing||
                bar.type == eDungeonCharactorBar.Progress||
                bar.type == eDungeonCharactorBar.MonsterEnergyBar)
            {
                bool needRemove = _updateSpellBar(bar, delta);
                if (needRemove)
                {
                    bNeedRemove = true;
                }
            }
        }

		if(bNeedRemove)
        {
			mBars.RemoveAll(x => { return !x.active; });
		}
    }

    public void UpdateSpellBarsWithBuff(int delta)
    {
        bool bNeedRemove = false;

        for (int i = 0; i < mBars.Count; ++i)
        {
            var bar = mBars[i];
            if (bar.type == eDungeonCharactorBar.Buff ||
                bar.type == eDungeonCharactorBar.Continue ||
                bar.type == eDungeonCharactorBar.Fire||
                bar.type == eDungeonCharactorBar.Progress)
            {
                bool bRemove = _updateSpellBar(bar, delta);
                if(bRemove)
                {
                    bNeedRemove = true;
                }
#if !LOGIC_SERVER
                else if (bar.alwaysRefreshUI && m_pkGeActor != null)
                {
                    if (bar.acc <= bar.duration && bar.acc > 0)
                    {
                        bool show = true;
                        if (bar.type == eDungeonCharactorBar.Continue && !isLocalActor)
                            show = false;
                        m_pkGeActor.SetBar(bar.type, 1.0f * bar.acc / bar.duration, show);
                        m_pkGeActor.SetCdTime(bar.type, bar.acc / 1000.0f, show);
                    }
                }
#endif

            }
        }

        if (bNeedRemove)
        {
            mBars.RemoveAll(x => { return !x.active; });
        }
    }

    private bool _updateSpellBar(SpellBar bar, int delta)
    {
        var bNeedRemove = false;

        if (bar.active)
        {

            if (bar.reverse)
            {
                if (bar.reverseAutoAcc)
                    bar.acc -= delta;
            }
            else
            {
                if (bar.autoAcc)
                    bar.acc += delta;
            }

            if (bar.acc <= bar.duration && bar.acc > 0)
            {
                /*bool show = true;
                if (bar.type == eDungeonCharactorBar.Continue && !isLocalActor)
                    show = false;
                m_pkGeActor.SetBar(bar.type, 1.0f * bar.acc / bar.duration, show);*/
            }
            else
            {
                if (bar.autodelete)
                {
                    bar.active = false;
                    bar.acc = 0;
                    StopSpellBar(bar.type, false);

                    if (!bar.reverse && bar.com != null)
                    {
#if !LOGIC_SERVER
                        GeEffectEx effect = m_pkGeActor.CreateEffect(1001, new Vec3(0, 0, 0));
                        if (effect != null)
                        {
                            Battle.GeUtility.AttachTo(effect.GetRootNode(), m_pkGeActor.mBarsRoot.transform.parent.gameObject);
                            effect.SetPosition(bar.com.transform.position);
                        }
#endif
                    }
                }
            }

            if (bar.acc >= bar.duration)
            {
                bar.acc = bar.duration;
            }
        }

        if (bar.active == false)
        {
            bNeedRemove = true;
        }

        return bNeedRemove;
    }

    private SpellBar _FindSpellBar(eDungeonCharactorBar type)
    {
        SpellBar bar = null;
        for (int i = 0; i < mBars.Count; ++i)
        {
            if (mBars[i].type == type)
            {
                bar = mBars[i];
                break;
            }
        }

        return bar;
    }

    public void StopSpellBar(eDungeonCharactorBar type, bool cancel = true)
    {
        var bar = _FindSpellBar(type);

        if (null != bar)
        {
            bar.active = false;
        }

		if (m_pkGeActor != null)
        	m_pkGeActor.StopBar(type, cancel);
    }

	public void SetSpellBarReverse(eDungeonCharactorBar type, bool flag)
	{
        var bar = _FindSpellBar(type);
		if (bar != null)
			bar.reverse = flag;
	}

	public int GetSpellBarDuration(eDungeonCharactorBar type)
	{
		int dur = 0;
        var bar = _FindSpellBar(type);
		if (bar != null)
			dur = bar.acc;

		return dur;
	}

	//addProgress 0~1
	public void AddSpellBarProgress(eDungeonCharactorBar type, VFactor addProgress)
	{
        var bar = _FindSpellBar(type);
		if (bar != null)
		{
			bar.acc += (bar.duration * addProgress);
		}
	}
    public void AddSpellBarProgress(eDungeonCharactorBar type, float addProgress)
    {
        var bar = _FindSpellBar(type);
        if (bar != null)
        {
            float acc = (bar.duration * addProgress);
            bar.acc += (int)acc;
        }
    }
    public int GetSpelBarProgress(eDungeonCharactorBar type)
	{
        var bar = _FindSpellBar(type);
		if (bar != null)
		{
			return bar.acc / bar.duration;
		}

		return 0;
	}


    /************************************************************************************/
    //utiliy
    public int GetWeaponType()
    {
        int weaponType = 0;

        if (attribute != null)
        {
            return attribute.GetWeaponType();
        }

        return weaponType;
    }

	public int GetWeaponID()
	{
		int weaponID = 0;
		if (attribute != null)
		{
            weaponID = attribute.GetWeaponItemID();
        }

		return weaponID;
	}

    public void ChangeWeapon(int index, int weaponStrengthenLevel = 0)
    {
        if (attribute == null || attribute.backupWeapons.Count <= index || attribute.currentWeapon == null)
            return;


        Dictionary<int, CrypticInt32> skillLevelBeforeChange = new Dictionary<int, CrypticInt32>(attribute.skillLevelInfo);

        var oldWeapon = attribute.currentWeapon;
        if (BattleMain.IsModePvP(battleType))
        {
            RemoveMechanisms(oldWeapon.attachPVPMechanismIDs);
            RemoveMechanismBuffs(oldWeapon.attachPVPBuffIDs, oldWeapon);
        }
        else
        {
            RemoveMechanisms(oldWeapon.attachMechanismIDs);
            RemoveMechanismBuffs(oldWeapon.attachBuffIDs, oldWeapon);
        }
        RemoveSuitProperty();

        attribute.ChangeWeapon(index);      
        var newWeapon = attribute.currentWeapon;

        ReloadSuitProperty(newWeapon,oldWeapon);

        if (BattleMain.IsModePvP(battleType))
        {
            LoadMechanisms(newWeapon.attachPVPMechanismIDs, 0, MechanismSourceType.EQUIP);
            LoadMechanismBuffs(newWeapon.attachPVPBuffIDs, 0, false, newWeapon);
        }
        else
        {
            LoadMechanisms(newWeapon.attachMechanismIDs, 0, MechanismSourceType.EQUIP);
            LoadMechanismBuffs(newWeapon.attachBuffIDs, 0, false, newWeapon);
        }
        m_cpkEntityInfo.SetWeaponInfo(attribute.GetWeaponTag(), attribute.GetWeaponType());


        //处理武器模型切换
        if (m_cpkCurEntityActionInfo != null)
        {
            var acActionName = m_cpkEntityInfo.GetCurrentActionName();
            if (m_cpkEntityInfo.HasAction(acActionName))
            {
                m_cpkCurEntityActionInfo = m_cpkEntityInfo._vkActionsMap[acActionName];
                attachmentproxy.Init(m_cpkCurEntityActionInfo);
            }
        }

        ChangeWeaponModle(newWeapon.strengthen);
        attachmentproxy.Update(0);

        

        //处理技能阶段
        if (IsCastingSkill())
        {
            skillController.SetSkillPhases(GetCurSkillID());
        }

        ItemTable oldWeaponData = TableManager.instance.GetTableItem<ItemTable>(oldWeapon.itemID);
        ItemTable newWeaponData = TableManager.instance.GetTableItem<ItemTable>(newWeapon.itemID);
        //TriggerEvent(BeEventType.OnChangeWeapon, new object[] { oldWeaponData, newWeaponData });
        TriggerEventNew(BeEventType.OnChangeWeapon, new EventParam() { m_Obj = oldWeaponData, m_Obj2 = newWeaponData });

        Dictionary<int, CrypticInt32> skillLevelAfterChange = new Dictionary<int, CrypticInt32>(attribute.skillLevelInfo);

        //技能等级变化的处理
        var enumerator = skillLevelBeforeChange.GetEnumerator();
        while(enumerator.MoveNext())
        {
            int skillID = enumerator.Current.Key;
            int skillLevelBefore = enumerator.Current.Value;
            int skillLevelAfter = 0;
            if (skillLevelAfterChange.ContainsKey(skillID))
                skillLevelAfter = skillLevelAfterChange[skillID];

            if (skillLevelBefore != skillLevelAfter)
            {
#if UNITY_EDITOR
                //Logger.LogErrorFormat("weaponChange skill:{0} {1}=>{2}", skillID, skillLevelBefore, skillLevelAfter);
#endif
                var skill = GetSkill(skillID);
                if (skill != null)
                {
                    skill.OnInit();
                    if (skill.skillData != null && skill.skillData.SkillType == SkillTable.eSkillType.PASSIVE)
                        skill.OnPostInit();
                    skill.DealLevelChange();
                }
            }
        }

        //技能处理   
        //替换技能时先将所有技能的ComboSouceId重置（暂时想不到更好的重置方案，所以先写在这里）
        var enumerator3 = skillController.GetSkills().GetEnumerator();
        while (enumerator3.MoveNext())
        {
            BeSkill skill = enumerator3.Current.Value;
            if (skill != null)
            {
                skill.comboSkillSourceID = 0;
            }
        }

        var enumerator2 = skillController.GetSkills().GetEnumerator();
        while (enumerator2.MoveNext())
        {
            BeSkill skill = enumerator2.Current.Value;
            if (skill != null)
            {
                skill.DealWeaponChange();
            }
        }

#if !LOGIC_SERVER
        if (isLocalActor)
        {
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUISwitchWeaAndEquip>();
            if (battleUI != null)
            {
                battleUI.ChangeWeaponIcon(oldWeapon.itemID);
            }
            if ( !ReplayServer.GetInstance().IsReplay())
                BeUtility.ChangeWeaponInBattle(newWeapon.guid,oldWeapon.guid);
        }
        if(m_pkGeActor != null && m_pkGeActor.mCurHpBar != null)
            m_pkGeActor.mCurHpBar.InitResistMagic(attribute.GetResistMagic(), this);
#endif
        TriggerEventNew(BeEventType.OnChangeWeaponEnd);
    }

    public void ChangeWeaponModle(int weaponStrengthenLevel)
    {
        int wid = GetWeaponID();
        if (wid > 0)
        {
            var itemData = TableManager.GetInstance().GetTableItem<ItemTable>(wid);
            if (itemData != null)
            {
                ReplaceWeapon(itemData.ResID, itemData.Tag, weaponStrengthenLevel);
            }
        }
        else
        {
            var data = TableManager.GetInstance().GetTableItem<JobTable>(attribute.professtion);
            if (data != null)
            {
                SetDefaultHitSFX(data.DefaultHitSFXID);
            }
        }
    }

    //初始化时装武器挂件
    protected void InitFashionWeapon(List<ItemProperty> equips,int jobId)
    {
#if !LOGIC_SERVER
        if (equips == null)
            return;
        for (int i = 0; i < equips.Count; i++)
        {
            int itemId = equips[i].itemID;
            ItemTable itemData = TableManager.instance.GetTableItem<ItemTable>(itemId);
            if (itemData == null || itemData.SubType != ItemTable.eSubType.FASHION_WEAPON)
                continue;
            ResTable resData = TableManager.instance.GetTableItem<ResTable>(itemData.ResID);
            if (resData == null)
                continue;
            JobTable jobData = TableManager.instance.GetTableItem<JobTable>(jobId);
            if (jobData == null)
                continue;
            if (attachmentproxy != null)
                attachmentproxy.SetShowFashionWeapon(resData.ModelPath, jobData.DefaultWeaponPath);
        }
#endif
    }

    /// <summary>
    /// 怪物表中填了技能实现的怪物可以被攻击的话  这个函数会返回False
    /// </summary>
	public bool IsSkillMonster()
	{
        return (IsMonster() || m_iCamp == (int)ProtoTable.UnitTable.eCamp.C_HERO) && attribute.type == (int)ProtoTable.UnitTable.eType.SKILL_MONSTER 
            && !attribute.skillMonsterCanBeAttack;
	}
    // 能否死在空中
    public bool IsCanCritDead()
    {
        if (!IsMonster()) // hero不行
            return false;
        if (attribute.type == (int)ProtoTable.UnitTable.eType.BOSS) // boss不行
            return false;
        return true;
    }

    public void ShowSpeech(ActionState state)
    {
        if (walkSpeeches == null && attackSpeeches == null)
        {
            return;
        }

		IList<int> speeches = null;
		if (state == ActionState.AS_BIRTH)
		{
			speeches = birthSpeeches;
		}
		else {
			if (FrameRandom.InRange(1, GlobalLogic.VALUE_100) < 95)
			{
				return;
			}
		}
			
        if (state == ActionState.AS_WALK || 
            state == ActionState.AS_RUN ||
            state == ActionState.AS_IDLE)
        {
            //走路冒泡时需要有目标（需求）
            if(aiManager != null && aiManager.aiTarget != null)
            {
                speeches = walkSpeeches;
            }
            //speeches = walkSpeeches;
        }
        else if (state == ActionState.AS_CASTSKILL)
        {
            speeches = attackSpeeches;
        }

        if (speeches != null && speeches.Count > 0 && speeches[0]!=0)
        {
            int index = FrameRandom.InRange(0, speeches.Count);

            ShowSpeechWithID(speeches[index], GetEntityData().isPet);
        }
    }

	public void ShowSpeechWithID(int speechID, bool isPet)
	{
        var speechData = TableManager.GetInstance().GetTableItem<MonsterSpeech>(speechID);
        if (null == speechData)
        {
            return;
        }

        int rv = FrameRandom.InRange(0, GlobalLogic.VALUE_1000);
        if (rv >= speechData.Rate)
        {
            return;
        }

#if !SERVER_LOGIC
        float speechTime = 3.5f;

        if (speechData.Time > 0)
        {
            speechTime = speechData.Time / 1000.0f;
        }

        if (null != m_pkGeActor)
        {
            m_pkGeActor.ShowHeadDialog(speechData.Speech, false, false, false, false, speechTime, isPet, speechData.DialogType);
        }
#endif
    }

	public bool CanUseDunfu()
	{

        if (IsGrabed())
            return false;
        if (IsBeGrabbed) return false;
        if (stateController != null &&
            (stateController.WillBeGrab() || stateController.IsGrabbing() || stateController.IsBeingGrab()))
            return false;
        //圣骑士假死状态下不能蹲伏
        if (buffController.HasBuffByID(101) != null)
            return false;
        var skill = GetSkill(Global.DUNFU_SKILL_ID);
		if (skill != null && !HaveDebuff())
		{
			return !isDead && skill.CanUseSkill() && attackButtonState == ButtonState.PRESS;
		}

		return false;
	}

    public bool HaveDebuff()
    {
        return stateController.HasBuffState(BeBuffStateType.SLEEP)||
               stateController.HasBuffState(BeBuffStateType.STUN) ||
               stateController.HasBuffState(BeBuffStateType.STONE)||
               stateController.HasBuffState(BeBuffStateType.FROZEN);
    }

	//蹲伏
	public void StartDunfu()
	{
		isDunFu = true;
        pressDunfuDurTime = -1;
        DunFuTimeAcc = 0;
		if (BattleMain.IsModePvP(battleType))
			DunFuTimeout = IntMath.Float2Int(Global.Settings.pvpDunFuTime * GlobalLogic.VALUE_1000);
		else
			DunFuTimeout = IntMath.Float2Int(Global.Settings.dunFuTime * GlobalLogic.VALUE_1000);


		delayCaller.DelayCall(GlobalLogic.VALUE_50, ()=>{
			m_pkGeActor.Pause((int)GeEntity.GeEntityRes.Action);
		});

		StartSpellBar(eDungeonCharactorBar.Loop, DunFuTimeout, true, "", true);
		buffController.TryAddBuff((int)GlobalBuff.DUNFU, DunFuTimeout);

		var skill = GetSkill(Global.DUNFU_SKILL_ID);
		if (skill != null)
		{
			skill.Start();
			skill.Finish();
		}
	}

    public ButtonState GetCurrentBtnState()
    {
        return attackButtonState;
    }

	public bool UpdateDunfu(int delta)
	{
		if (isDunFu)
		{
			bool release = false;

            if (pressDunfuDurTime >= 0)
            {
                pressDunfuDurTime += delta;
                if (pressDunfuDurTime >= GlobalLogic.VALUE_500)
                {
                    release = true;
                    StopSpellBar(eDungeonCharactorBar.Loop,false);
                    pressDunfuDurTime = -1;
                }
            }
            else
            {
                if (attackButtonState == ButtonState.PRESS)
                {
                    DunFuTimeAcc += delta;
                    if (DunFuTimeAcc >= DunFuTimeout)
                    {
                        release = true;
                        SetAttackButtonState(ButtonState.RELEASE);
                        StopSpellBar(eDungeonCharactorBar.Loop,false);
                    }
                }
                else if (attackButtonState == ButtonState.RELEASE)
                {
                    pressDunfuDurTime = DunFuTimeAcc;
                }
            }

            if (release)
            {
                m_pkGeActor.Resume((int)GeEntity.GeEntityRes.Action);
                isDunFu = false;
                buffController.RemoveBuff((int)GlobalBuff.DUNFU);
                buffController.TryAddBuff((int)GlobalBuff.GETUP_BATI, GlobalLogic.VALUE_500);

                return true;
            }
           
		}

		return false;
	}

    public void StartDeadProtect()
    {
        m_deadProtectDurTime = 0;
        m_isInDeadProtect = true;
    }
    public void EndDeadProtect()
    {
        m_deadProtectDurTime = 0;
        m_isInDeadProtect = false;
    }
    private void _updateDeadProtect(int delta)
    {
        if(m_isInDeadProtect)
        {
            m_deadProtectDurTime += delta;
            if(m_deadProtectDurTime >= GlobalLogic.VALUE_10000 + GlobalLogic.VALUE_1500)
            {
                m_isInDeadProtect = false;
                m_deadProtectDurTime = 0;
                //TriggerEventNew(BeEventType.onDeadProtectEnd, null);
                TriggerEventNew(BeEventType.onDeadProtectEnd, new EventParam(){});
            }
        }
    }
    public bool IsInDeadProtect { get { return m_isInDeadProtect; } }

	public sealed override void OnDead ()
	{
		base.OnDead ();

		SetAttackButtonState(ButtonState.RELEASE);
        RemoveAllMechanism();
		ResetSkillCoolDown();
		skillController.CancelSkills();
		buffController.RemoveInPassiveBuff();
		buffController.RemoveRangerTriggerBuff();
		SetBlockLayer(false);
		DeletePressCountRes();
        if(!IsMonster())
            buffController.TriggerBuffs(BuffCondition.PLAYER_DEAD);   //角色死亡时
        if (!IsBoss())
		{
            if(m_pkGeActor != null)
                m_pkGeActor.RemoveSurface(uint.MaxValue);
		}
#if !LOGIC_SERVER
        CheckAutoReborn();
        RemoveSkillJoyStick();
#endif
        if (!isMainActor)
        {
            buffController.RemoveAllBuff(true);
        }
    }

    public void ResetActorStatus()
    {
        SetAttackButtonState(ButtonState.RELEASE);
        RemoveAllMechanism();
        skillController.CancelSkills();
        ResetSkillCoolDown();
        buffController.RemoveInPassiveBuff();
        buffController.RemoveRangerTriggerBuff();
        SetBlockLayer(false);
        DeletePressCountRes();

        m_pkGeActor.RemoveSurface(uint.MaxValue);

        m_pkStateGraph.ClearStateStack();

        //重新添加机制
        Dictionary<int, BeSkill>.Enumerator enumerator = GetSkills().GetEnumerator();
        while (enumerator.MoveNext())
        {
            var skill = enumerator.Current.Value;
            skill.AddMechanisms();
        }
        //重新加载装备机制
        LoadMechanisms(recordedEquipMechanismIDs, 0, MechanismSourceType.EQUIP);
        recordedEquipMechanismIDs.Clear();

        Reset();

        skillController.StartInitCDForSkill();

        // TODO 此处代码是整合OnDead和Reborn两个函数的相关功能
    }


    public sealed override void Reborn()
    {
        dungeonRebornCount++;
		SetAttackButtonState(ButtonState.RELEASE);
		m_pkStateGraph.ClearStateStack();
		ResetSkillCoolDown();
		buffController.RemoveInPassiveBuff();
        //复活的时候 重新添加机制
        Dictionary<int, BeSkill>.Enumerator enumerator = GetSkills().GetEnumerator();
        while (enumerator.MoveNext())
        {
            var skill = enumerator.Current.Value;
            skill.AddMechanisms();
        }

        //重新加载装备机制
        LoadMechanisms(recordedEquipMechanismIDs, 0, MechanismSourceType.EQUIP);
        recordedEquipMechanismIDs.Clear();

        base.Reborn();
             
		buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE, GlobalLogic.VALUE_2000);
        UseSkill(Global.REVIVE_SHOCK_SKILLID, true);

        TriggerEventNew(BeEventType.onReborn, new EventParam() { m_Obj = this });
        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BattlePlayerAlive);

		if (aiManager != null && (aiManager.isAutoFight || attribute.isMonster))
			aiManager.Start();

		if (IsProcessRecord())
		{
			GetRecordServer().RecordProcess("[BATTLE]PID:{0}-{1} Reborn", m_iID, GetName());
            GetRecordServer().Mark(0x8779781, new int[]
                                                  {
                                                     m_iID
                                                  }, GetName());
            // Mark:0x8779781 [BATTLE]PID:{0}-{1} Reborn
        }

        if (m_pkGeActor != null)
        {
            m_pkGeActor.RemoveSurface(uint.MaxValue);
        }
        else
        {
            int jobId = GetEntityData() != null ? GetEntityData().professtion : 0;
            int monsterId = GetEntityData() != null ? GetEntityData().monsterID : 0;
            string name = GetEntityData() != null ? GetEntityData().name : "";
            Logger.LogErrorFormat("Reborn occur error {0} {1} {2} {3}", m_iID, jobId , monsterId, name);
        }

#if DEBUG_SETTING
		if (Global.Settings.isDebug)
		{
			if (Global.Settings.playerRebornHP > 0)
			{
				GetEntityData().SetHP(Global.Settings.playerRebornHP);
				GetEntityData().SetMaxHP(Global.Settings.playerRebornHP);
                if (m_pkGeActor != null)
                {
                    m_pkGeActor.ResetHPBar();
                }
			}
		}
#endif

        if (CurrentBeScene.mBattle.dungeonPlayerManager.GetAllPlayers().Count > 1)
		{
            if (m_pkGeActor != null)
            {
                m_pkGeActor.SetActorVisible(true);
            }
		}
        buffController.RefreshBuffStateOnReborn();
    }

    protected override VInt GetZSpeedAcc()
    {
        for (int i = 0; i < Global.Settings.speedAnchorArray.Length; i++)
        {
            var speed = VInt.NewVInt(Global.Settings.speedAnchorArray[i], 1000);
            if (moveZSpeed.i < speed)
            {
                var weight = (VInt)Global.Settings.gravity;
                if (GetEntityData() != null)
                    weight = GetEntityData().weight;
                weight = weight.i * VFactor.NewVFactor(Global.Settings.gravityRateArray[i], 1000);
                return weight;
            }
        }
        return base.GetZSpeedAcc();
    }

    public void LevelUp(int levelAdd)
	{
		attribute.CalculateLevelGrow(levelAdd+1);
		attribute.PostInit();
		m_pkGeActor.ResetHPBar();
	}

	public void LevelUpTo(int newLevel)
	{
		int levelAdd = newLevel - attribute.level;
		if (levelAdd > 0)
		{
			attribute.level = newLevel;
			LevelUp(levelAdd);
			m_pkGeActor.UpdateInfoBarLevel((ushort)newLevel, false);
			m_pkGeActor.OnLevelChanged(newLevel);
		}
	}
		
	public void SetAttackButtonState(ButtonState bs)
	{
		attackButtonState = bs;
        if (bs == ButtonState.PRESS)
            TriggerEventNew(BeEventType.onCastNormalAttack);
    }

    public void SetAttackCheckFlag(bool flag)
    {
        aiAttackNeedCheck = flag;
    }

	public sealed override void OnDealFrameTag (DSFFrameTags frameTag)
	{
		base.OnDealFrameTag (frameTag);
		if (frameTag == DSFFrameTags.TAG_SET_TARGET_POS_XY)
		{
			if (IsMonster() && aiManager != null && aiManager.aiTarget != null)
			{
				var targetPos = aiManager.aiTarget.GetPosition();
				SetStandPosition(new VInt3(targetPos.x, targetPos.y, GetPosition().z));
			}
		}
	}

	void StartPressCountBase(QuickPressType type)
	{
		if (!isMainActor)
			return;

		if (isRecordPress)
			return;

		quickPressType = type;
		isRecordPress = true;
		pressCount = 0;

		if (isLocalActor)
		{
			var pos = GetPosition();
			pos.z += VInt.Float2VIntValue(3f);
			if (recordPressEffect == null)
			{
				recordPressEffect = currentBeScene.currentGeScene.CreateEffect(11, pos.vec3);
			}

			if (recordPressEffect != null)
			{
				recordPressEffect.SetVisible(true);
				recordPressEffect.SetPosition(pos.vector3);
			}
		}
	}

	public void StartPressCount(QuickPressType type, BeActor actor)
	{
		thisActor = actor;
		StartPressCountBase(type);
	}

	public void StartPressCount(QuickPressType type, BeBuff buff)
	{
		//thisBuff = buff;
        thisBuffPID = buff.PID;
		StartPressCountBase(type);
	}

    /*
	 * 点一下减5%，最多点10下
	*/
    public void RecordPressCount()
    {
        if (!isRecordPress)
            return;

        pressCount++;

        if (quickPressType == QuickPressType.BUFF)
        {
            if (pressCount >= 2)
            {
                pressCount -= 2;
                if (thisBuffPID > 0)
                {
                    var buff = buffController.GetBuffByPID(thisBuffPID);
                    if (buff != null)
                        buff.ReduceDuration(GlobalLogic.VALUE_50);
                }
                _addShock(0.1f, 30.0f, 0.05f, 0, 0);
            }
        }
        else
        {

            if (pressCount > 0 && pressCount % 2 == 0)
            {
                _addShock(0.1f, 30.0f, 0.05f, 0, 0);
            }

            /*int[] pressCountArray = new int[1];
            pressCountArray[0] = pressCount;
            TriggerEvent(BeEventType.onGrabPressCountAdd, new object[] { pressCountArray });*/
            
            var eventData = TriggerEventNew(BeEventType.onGrabPressCountAdd, new EventParam(){m_Int = pressCount});

            if (eventData.m_Int >= 20)
            {
                if (thisActor != null && thisActor.IsCastingSkill())
                {
                    thisActor.sgSwitchStates(new BeStateData((int)ActionState.AS_HURT));

                    EndPressCount();
                }
            }
        }
    }
    public override bool IsCurSkillOpenShock()
    {
#if !SERVER_LOGIC
	    InputManager.CameraShockMode mode = SettingManager.GetInstance().GetCameraShockMode();
	    return mode == InputManager.CameraShockMode.OPEN;
#else
        return true;
#endif
    }
    public void EndPressCount()
	{
		if (!isMainActor)
			return;
		if (!isRecordPress)
			return;
		
		isRecordPress = false;

		if (isLocalActor && recordPressEffect != null)
		{
			recordPressEffect.SetVisible(false);
		}
	}

	public void DeletePressCountRes()
	{
		if (recordPressEffect != null)
		{
			m_pkGeActor.DestroyEffect(recordPressEffect);
			recordPressEffect = null;
        }
    }

#region 随从
	public void SetAccompanyData(AccompanyData data)
	{
		accompanyData = data;

		var skillData = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(accompanyData.skillID);

		var skill = GetSkill(Global.HELP_SKILL_ID);
		if (skill != null && skillData != null)
		{
			Skill10000 skillHelp = skill as Skill10000;
			if (skillHelp != null)
			{
				skillHelp.fixCD = TableManager.GetValueFromUnionCell(skillData.RefreshTime, skillHelp.level);
				skillHelp.needSetIcon = true;
				skillHelp.iconPath = skillData.Icon;
			}
		}
	}
		
	public void UseHelpSkill()
	{
		var skill = GetSkill(Global.HELP_SKILL_ID);
		if (skill != null)
		{

			if (!skill.isCooldown)
			{
				skill.Start();
			}
		}
	}

	public void SummonHelp(AccompanyData data)
	{
		int aid = data.id;
		int level = data.level;
		int skillID = data.skillID;

		var tmpPos = GetPosition();
		tmpPos.z = 0;

		var summon = currentBeScene.SummonAccompany(
			level * GlobalLogic.VALUE_100 + aid,
			tmpPos,
			(int)ProtoTable.UnitTable.eCamp.C_HERO,
			this);

		if (summon == null)
			return;

		summon.SetFace(GetFace());
		summon.aiManager.Stop();

		summon.SetRestrainPosition(false);
		summon.buffController.RemoveBuff((int)GlobalBuff.INVINCIBLE);
		summon.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE_NO_ALPHA, int.MaxValue);
		summon.stateController.SetAbilityEnable(BeAbilityType.BLOCK, false);

		summon.m_pkGeActor.RemoveHPBar();


		summon.delayCaller.DelayCall(GlobalLogic.VALUE_100, ()=>{

			summon.SetPosition(
				new VInt3
				(
				GetPosition().x + (summon.GetFace()?1:-1)* VInt.Float2VIntValue(Global.Settings.keepDis), 
				GetPosition().y, 
				0
				)
			);
            //summon.m_pkGeActor.GetAnimation().PlayAction("Anim_Run", 1.0f, true);
            summon.m_pkGeActor.ChangeAction("Anim_Run", 1.0f, true,true,true);

			BeAction action = BeMoveBy.Create(
				summon, 
				IntMath.Float2Int(Global.Settings.accompanyRunTime*GlobalLogic.VALUE_1000), 
				summon.GetPosition(), 
				new VInt3(
					(Global.Settings.keepDis+1f)*(summon.GetFace()?-1:1), 0.0f, 0.0f
					)
				);
			action.SetFinishCallback(()=>{
				summon.UseSkill(skillID, true);	

				summon.RegisterEventNew(BeEventType.onStateChange, (param)=>{
					ActionState state = (ActionState)param.m_Int;
					if (state != ActionState.AS_CASTSKILL)
					{
						summon.delayCaller.DelayCall(200, ()=>{
							summon.m_pkGeActor.ChangeSurface("无敌", 0f);
							summon.currentBeScene.currentGeScene.CreateEffect(1017, new Vec3(summon.GetPosition().fx, summon.GetPosition().fy, 0.5f));
						});

						summon.delayCaller.DelayCall(500, ()=>{
							summon.SetLifeState(EntityLifeState.ELS_CANREMOVE);
						});
					}
				});

			});
			summon.actionManager.RunAction(action);
		});
	}

#endregion

#region PET

	DelayCallUnitHandle handle;
	DelayCallUnitHandle handle2;
	bool isPetMoving = false;
	ActionState petActionState = ActionState.AS_NONE;

	public void SetPetAlongside()
	{
		if (this.pet == null)
			return;

		this.pet.SetPosition (GetTargetPos());

		PlayPetIdle();
	}

	public bool HasSpeed(VInt speed)
	{
		return  speed != 0;
	}

	public void PlayPetIdle()
	{
		if (pet.IsCastingSkill())
			return;
		

		if (petActionState == ActionState.AS_IDLE)
			return;

		petActionState = ActionState.AS_IDLE;

		string action = "Idle";

		if (petData.hunger <= GlobalLogic.VALUE_30 && pet.HasAction("Eidle"))
			action = "Eidle";

		pet.PlayAction(action);
        //var param1 = DataStructPool.EventParamPool.Get();
        //param1.m_Int = (int)ActionState.AS_WALK;
        //pet.TriggerEventNew(BeEventType.onStateChange, param1);
        //DataStructPool.EventParamPool.Release(param1);

        pet.TriggerEventNew(BeEventType.onStateChange, new EventParam(){ m_Int = (int)ActionState.AS_WALK });
        //		Logger.LogErrorFormat("play pet idle!!!");
    }

	public void PlayPetWalk()
	{
		if (pet.IsCastingSkill())
			return;
		
		if (petActionState == ActionState.AS_WALK)
			return;

		petActionState = ActionState.AS_WALK;

		string action = "Walk";
		if (petData.hunger <= GlobalLogic.VALUE_30 && pet.HasAction("Ewalk"))
			action = "Ewalk";

		pet.PlayAction(action);
        //var param1 = DataStructPool.EventParamPool.Get();
        //param1.m_Int = (int)ActionState.AS_WALK;
        //pet.TriggerEventNew(BeEventType.onStateChange, param1);
        //DataStructPool.EventParamPool.Release(param1);

        pet.TriggerEventNew(BeEventType.onStateChange, new EventParam(){ m_Int = (int)ActionState.AS_WALK });
        //	Logger.LogErrorFormat("play pet walk!!!");
    }

	public void SetPetFace(bool flag)
	{
		//Logger.LogErrorFormat("set pet {0}", flag?"left":"right");

		pet.SetFace(flag);
	}


	
	public VInt3 GetTargetPos()
	{
		var pos = GetPosition();
		pos.z = 0;
		pos.x = GetFace()?(pos.x + VInt.one.i):(pos.x-VInt.one.i);
	

		return pos;
	}

	public bool IsPetForward()
	{
		if (GetFace() && pet.GetPosition().x < GetPosition().x || !GetFace() && pet.GetPosition().x > GetPosition().x)
			return true;

		return false;
	}

	public bool IsInPosition()
	{
		VInt3 targetPos = GetTargetPos();

		int sdisx = pet.GetPosition().x - targetPos.x;
		int sdisy = pet.GetPosition().y - targetPos.y;

		int disx = Mathf.Abs(sdisx);
		int disy = Mathf.Abs(sdisy);

		return disx < VInt.zeroDotOne && disy < VInt.zeroDotOne;
	}

	public void UpdatePet(int delta)
	{
		if (this.pet == null)
			return;

		this.pet.moveSpeedFactor = moveSpeedFactor;

		if (pet.IsCastingSkill())
		{
/*			pet.moveXSpeed = 0;
			pet.moveYSpeed = 0;*/

			return;
		}

		if (!IsInPosition())
		{
            handle.SetRemove (true);
            handle2.SetRemove (true);
				
			VInt3 targetPos = GetTargetPos();

			int sdisx = pet.GetPosition().x - targetPos.x;
			int sdisy = pet.GetPosition().y - targetPos.y;

			int dis = VInt.zeroDotOne.i;

			if (sdisx >= -dis && sdisx <= dis)
				sdisx = 0;
			if (sdisy >= -dis && sdisy <= dis)
				sdisy = 0;

			int disx = Mathf.Abs(sdisx);
			int disy = Mathf.Abs(sdisy);



			pet.moveXSpeed = Mathf.Abs(GetPetMoveXSpeed(disx).i) * (sdisx>0?-1:1);
			pet.moveYSpeed = Mathf.Abs(GetPetMoveYSpeed(disy).i) * (sdisy>0?-1:1);

			if (pet.moveXSpeed < 0)
				SetPetFace(true);
			else if (pet.moveXSpeed > 0)
				SetPetFace(false);
				 
			isPetMoving = true;

			PlayPetWalk();

			//Logger.LogErrorFormat ("pet:({0},{1}) dis:({2},{3}) ", pet.moveXSpeed, pet.moveYSpeed, sdisx, sdisy);

		}
		else {
			pet.moveXSpeed = 0;
			pet.moveYSpeed = 0;
			if (isPetMoving)
			{
				if (IsPetFaceDifferent())
				{
					handle = pet.delayCaller.DelayCall (GlobalLogic.VALUE_500, () => {
						SetPetFace(GetFace());
					});
				}

				isPetMoving = false;
				pet.SetPosition (GetTargetPos ());

				handle2 = pet.delayCaller.DelayCall((int)GlobalLogic.VALUE_200, ()=>{
					PlayPetIdle();
				});
			}

		}
	}

	public bool IsPetFaceDifferent()
	{
		return pet.GetFace () != GetFace ();
	}

	public VInt GetPetMoveXSpeed(VInt disx)
	{
		VInt sx = VInt.zero;
		if (disx <= VInt.Float2VIntValue(0.01f))
			return sx;

		if (moveXSpeed == 0|| IsPetFaceDifferent())
			sx = VInt.Float2VIntValue(Global.Settings.petXMovingv2);//5.0f;
		else {
			if (disx >= VInt.Float2VIntValue(Global.Settings.petXMovingDis))
				sx = moveXSpeed;
			else if (disx > VInt.Float2VIntValue(0.5f) && disx < VInt.Float2VIntValue(1.5f))
				sx = VInt.Float2VIntValue(Global.Settings.petXMovingv1);//1.0f;
		}
		return sx;
	}

	public VInt GetPetMoveYSpeed(VInt disy)
	{
		VInt sy = VInt.zero;
		if (disy <= VInt.Float2VIntValue(0.01f))	
			return sy;

		if (moveYSpeed == 0)
			sy = VInt.Float2VIntValue(Global.Settings.petYMovingv2);//3.0f;
		else {
			if (disy >= /*1.5f*/VInt.Float2VIntValue(Global.Settings.petYMovingDis))
				sy = moveYSpeed;
			else if (disy > VInt.Float2VIntValue(0.5f) && disy < VInt.Float2VIntValue(1.5f))
				sy = VInt.Float2VIntValue(Global.Settings.petYMovingv1);//0.5f;
		}

		return sy;
	}


	public void SetPetData(PetData petData)
	{
		if (petData == null)
			return;

		if (petData.id <= 0)
			return;

		//创建怪物
		this.petData = petData;

		var tmpPos = GetPosition();
		tmpPos.z = 0;

		var pet = currentBeScene.SummonAccompany (
			petData.level * GlobalLogic.VALUE_100 + petData.id,
			tmpPos,
			GetCamp(),
			this);

		if (pet == null)
		{
			Logger.LogErrorFormat("创建宠物{0}失败!!!", petData.id);
			return;
		}

		pet.aiManager.Stop ();
#if !LOGIC_SERVER
		Vector4 entityPlane = GeSceneEx.EntityPlane;
		entityPlane.w -= 0.07f;
        pet.m_pkGeActor.AddSimpleShadow(Vector3.one);
        
#endif
		pet.stateController.SetAbilityEnable(BeAbilityType.BETARGETED, false);
		pet.stateController.SetAbilityEnable(BeAbilityType.BLOCK, false);
		pet.m_pkGeActor.SetHPBarVisible (false);
		pet.GetEntityData().isPet = true;

		pet.owner = this;
		currentBeScene.AdjustSummonMonsterAttribute(this, pet);

		RegisterEventNew(BeEventType.onAfterDead, args => {
			pet.delayCaller.DelayCall(GlobalLogic.VALUE_500, () => { 
                if(IsDead())
				    pet.m_pkGeActor.SetActorVisible(false);
			});
		});

		RegisterEventNew(BeEventType.onReborn, args => {
			if (pet != null)
			{
				if (pet.m_pkGeActor != null)
				{
					pet.m_pkGeActor.SetActorVisible(true);	
					SetPetAlongside();
				}
				else
				{
					//Logger.LogErrorFormat("pet.m_pkgeactor is null!!!!");
				}
			}
			else {
				//Logger.LogErrorFormat("pet is null!!!!");
			}


		});

		this.pet = pet;

		SetPetAlongside();

		if (BattleMain.IsModePvP(battleType))
		{
			//隐身
			pet.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE, int.MaxValue);
			return;
		}

		//处理饥饿度小于0的情况
		if (petData.hunger <= 0)
		{
            BuffInfoData triggerPetSkillInfo = new BuffInfoData
            {
                buffID = -1,
                condition = BuffCondition.ATTACK,
                prob = GlobalLogic.VALUE_100
            };
            buffController.AddTriggerBuff (triggerPetSkillInfo);

			RegisterEventNew (BeEventType.onPetSkill, (args) => 
            {
				if (FrameRandom.Range100() < 90)
                {
					return;
                }

				pet.ShowSpeechWithID(11602, true);
			});

			return;
		}

		//加载主动技能
		pet.skillController.LoadOneSkill(petData.skillID, petData.level);
		//pet.PostInitOneSkill(petData.skillID);

		var petSkill = pet.GetSkill(petData.skillID);
		if (petSkill != null)
		{
			petSkill.PostInit();

			petSkill.StartInitCD(BattleMain.IsModePvP(battleType));
		}
		else {
			Logger.LogErrorFormat("宠物 {0} 找不到技能 {1}", pet.GetName(), petData.skillID);
			return;
		}
			
		//技能释放
		bool masterAttackTrigger = false;
		for(int i=0; i<petSkill.preConditions.Count; ++i)
		{
            if (petSkill.preConditions[i] == SkillTable.ePreCondition.MASTER_ATTACK)
            {
                masterAttackTrigger = true;
                break;
            }
        }

		if (masterAttackTrigger)
		{
            BuffInfoData triggerPetSkillInfo = new BuffInfoData
            {
                buffID = -1,
                condition = BuffCondition.ATTACK,
                //triggerPetSkillInfo.CD = petSkill.GetCurrentCD ();
                prob = petSkill.useRate
            };

            buffController.AddTriggerBuff (triggerPetSkillInfo);

			petSkill.useRate = GlobalLogic.VALUE_1000;

			RegisterEventNew (BeEventType.onPetSkill, (args) => {
				if (pet.CanUseSkill(petData.skillID))
				{
                    var dir = GetPosition2() - pet.GetPosition2();
                    dir.Normalize();
                    if (dir.x != 0)
                    {
                        SetPetFace(dir.x < 0 ? true : false);
                    }
                    else
                    {
                        SetPetFace(GetFace());
                    }
                    pet.UseSkill(petData.skillID, true);

					petActionState = ActionState.AS_CASTSKILL;
				}	
			});
		}
			

	}

    public GameObject attachModel;
    //创建跟随怪物
    public void CreateFollowMonster()
    {
#if !LOGIC_SERVER
        if (m_pkGeActor == null || currentBeScene == null)
            return;
        string resPath = BeUtility.GetAttachModelPath(professionID);
        if (resPath == null)
            return;
        GameObject attachNode = m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Actor);
        attachModel = Utility.AddModelToActor(resPath, m_pkGeActor, attachNode);
#endif
    }

#endregion
		
	public bool IsPassiveState()
	{
		return sgGetCurrentState() == (int)ActionState.AS_GRABBED || sgGetCurrentState() == (int)ActionState.AS_FALLGROUND;
	}

	public bool IsNearSceneEdge()
	{
		//float limit = 1.5f;
		VInt limit = VInt.onehalf;
		if (Mathf.Abs(GetPosition().x - currentBeScene.logicXSize.x)<limit.i ||
			Mathf.Abs(GetPosition().x - currentBeScene.logicXSize.y)<limit.i)
			return true;

		return false;
	}

	public bool CanRoll()
	{
		if (!IsDead() && IsMonster() && sgGetCurrentState()!=(int)ActionState.AS_ROLL && HasAction(ActionType.ActionType_Roll) && (
			IsNearSceneEdge() && FrameRandom.Range1000() <= (uint)(Global.Settings.rollRand*GlobalLogic.VALUE_1000) ||
			!IsNearSceneEdge() && FrameRandom.Range1000()<=(uint)(Global.Settings.normalRollRand*GlobalLogic.VALUE_1000)))
			return true;

		return false;
	}

    public void InitAutoFight(
        string actionTree = null,
        string destTree = null,
        string eventTree = null,
        int thinkTerm = 0,
        int findTargetTerm = 0,
        int changeDestinationTerm = 0,
        int keepDistance = 0,
        bool pauseAI=true
	)
	{
		BeAIManager aiManager = new BeActorAIManager();
		InitAI(aiManager);
		if (aiManager != null)
		{
			aiManager.isAutoFight = true;
			aiManager.isAPC = true;
			aiManager.sight = GlobalLogic.VALUE_20000;
			aiManager.chaseSight = aiManager.sight;
			aiManager.thinkTerm = thinkTerm==0?Global.Settings.afThinkTerm:thinkTerm;
			aiManager.findTargetTerm = findTargetTerm==0?Global.Settings.afFindTargetTerm:findTargetTerm;
			aiManager.changeDestinationTerm = changeDestinationTerm==0?Global.Settings.afChangeDestinationTerm:changeDestinationTerm;
			aiManager.idleMode = BeAIManager.IdleMode.IDLE;
			aiManager.idleDuration = GlobalLogic.VALUE_300;
			aiManager.targetType = BeAIManager.TargetType.NEAREST;//BeAIManager.TargetType.BOSS;
			aiManager.walkBackRange = 2;
			aiManager.wanderRange = 2;
            aiManager.keepDistance = keepDistance;

        //	if (Global.Settings.useNewAutofightAI)
            aiManager.InitAgents(actionTree, destTree, eventTree);
		//	else
		//		aiManager.InitTrees(actionTree, destTree, eventTree);

			StartAI(null, false);
			this.pauseAI = pauseAI;
		}
	}


    public void SetDefaultAIRun(bool isRun)
    {
	    if(aiManager == null)
		    return;
	    var ai = aiManager as BeActorAIManager;
	    if (ai == null)
		    return;
	    ai.isRunDefaultAI = isRun;
    }
    
	public bool TriggerComboSkills(int skillID)
	{
        var comboSkillID = skillController.CheckComboSkill();

        if (comboSkillID > 0)
		{
            BeSkill skill = GetSkill(comboSkillID);
            BeSkill curSkill = GetCurrentSkill();
            if (skill != null && curSkill != null && skillID != skill.comboSkillSourceID)
            {
                return false;
            }
            if (curSkill != null)
            {
                curSkill.cancelByCombo = true;
                curSkill.OnComboBreak();
                curSkill.buttonState = ButtonState.RELEASE;//这里要把上一个combo技能的按钮状态改为 RELEASE
            }
            CastSkill(comboSkillID);
			return true;
		}      
        return false;
	}

	public void SetAutoFight(bool auto)
    {
        if (IsProcessRecord())
        {
            GetRecordServer().RecordProcess("[SetAutoFight]PID:{0}-{1} auto :{2} ", m_iID, GetName(), auto);
            GetRecordServer().Mark(0x8779783, new int[]
          {
                m_iID,
                auto ? 1:0
          }, GetName());
            // Mark:0x8779783 [SetAutoFight]PID:{0}-{2} auto : {1}
        }
        if (!auto)
			SetForceRunMode(false);
		
		if (pauseAI == !auto)
			return;

		pauseAI = !auto;
		if (pauseAI)
		{
			SetForceRunMode(false);
            if (aiManager != null)
			    aiManager.StopCurrentCommand();
			ResetMoveCmd();
			SetAttackButtonState(ButtonState.RELEASE);
		}
		else {
		//	forceRunMode = true;
		}
	}

	public sealed override void OnChangeFace()
	{
		if (isMainActor && IsRunning() && doublePressRunLeft != GetFace())
		{
			if (Global.Settings.changeFaceStop)
				ChangeRunMode(false);
		}
	}

	public sealed override void JudgeDead ()
	{
		//过关后角色不走死亡流程
		if (isMainActor && currentBeScene.IsBossDead())
		{
			return;
		}

		base.JudgeDead ();
	}

	public sealed override void DoDead (bool isForce = false)
	{
		base.DoDead (isForce);
        ClearAttackId();
	}

#region changemodel
    public void ChangeModel(ProtoTable.UnitTable data, bool isPreChange = true)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeActor.ChangeModel"))
        {
#endif
        if (data == null)
            return;
        // 
        if (m_pkGeActor == null)
            return ;

#if !LOGIC_SERVER
			if (isPreChange)
				m_pkGeActor.PreChangeModel(data.Mode);
			else
				m_pkGeActor.TryChangeModel(data.Mode);
#endif
            
        return;
#if ENABLE_PROFILER
        }
#endif
    }

    public void ChangeSkillForPlayer()
    {
        var proid = GetEntityData().professtion;
        var skillInfo = TableManager.instance.GetSkillInfoByPid(proid);

        //技能
        m_cpkEntityInfo.Reset();
        skillController.ClearSkillList();
        skillController.LoadSkill(skillInfo, false, m_iResID);

    }
    public void ChangeSkill(ProtoTable.UnitTable data)
    {
        if (currentBeScene == null)
            return;

        Dictionary<int, int> skillInfo = currentBeScene.GetMonsterSkillInfo(data.ID);
        if (skillInfo == null)
            return;

        // 技能
        m_cpkEntityInfo.Reset();
        skillController.ClearSkillList();
        skillController.LoadSkill(skillInfo, false, data.Mode);

        // AI
        if (aiManager != null)
            aiManager.ReloadSkillInfos(data);

        // 移动能力
        currentBeScene.ChangeMonsterAbility(this, data, true);
        if (lastTimeMonsterData == null)
        {
            var entityData = GetEntityData();
            if (entityData != null)
            {
                ProtoTable.UnitTable unitData = TableManager.GetInstance().GetTableItem<ProtoTable.UnitTable>(entityData.monsterID);
                lastTimeMonsterData = unitData;
            }
        }
        currentBeScene.ChangeMonsterAbility(this, lastTimeMonsterData, false);
        lastTimeMonsterData = data;
    }

#endregion

#region /*****替换普攻技能ID相关内容******/
    //用于存储替换的普攻ID
    public struct NormalAttack
    {
        public int PriorityLevel;           //优先级
        public int SkillID;                 //替换的技能ID
    }

    protected List<NormalAttack> m_ReplaceAttackIdList = new List<NormalAttack>();
    //添加
    public NormalAttack AddReplaceAttackId(int id, int level/*替换优先级*/)
    {
        int count = m_ReplaceAttackIdList.Count;
        NormalAttack attackData = new NormalAttack
        {
            SkillID = id,
            PriorityLevel = level
        };
        if (count>0)
        {
            int endPriorityLevel = m_ReplaceAttackIdList[count-1].PriorityLevel;
            if (level >= endPriorityLevel)
            {
                m_ReplaceAttackIdList.Add(attackData);
                GetEntityData().normalAttackID = id;
            }
            else
            {
                for (int i = count-1; i >=0; i--)
                {
                    if (level >= m_ReplaceAttackIdList[i].PriorityLevel)
                    {
                        m_ReplaceAttackIdList.Insert(i + 1, attackData);
                    }
                }
            }
        }
        else
        {
            NormalAttack originalData = new NormalAttack
            {
                SkillID = GetEntityData().normalAttackID,
                PriorityLevel = 0
            };
            m_ReplaceAttackIdList.Add(originalData);
            ReplaceAttackId(attackData);
        }
        return attackData;
    }

    //移除
    public void RemoveReplaceAttackId(NormalAttack data)
    {
        if (!data.Equals(null))
        {
            int count = m_ReplaceAttackIdList.Count;
            if (count > 1)
            {
                if (data.Equals(m_ReplaceAttackIdList[count - 1]))
                {
                    GetEntityData().normalAttackID = m_ReplaceAttackIdList[count - 2].SkillID;
                }
                m_ReplaceAttackIdList.Remove(data);
            }
        }
    }

    //清除所有
    protected void ClearAttackId()
    {
        if (m_ReplaceAttackIdList.Count > 0)
        {
            ReplaceAttackId(m_ReplaceAttackIdList[0]);
            m_ReplaceAttackIdList.Clear();
        }
    }

    protected void ReplaceAttackId(NormalAttack attackData)
    {
        GetEntityData().normalAttackID = attackData.SkillID;
        m_ReplaceAttackIdList.Add(attackData);
    }
#endregion

    /// <summary>
    /// 检查是否能够自动复活
    /// </summary>
    private void CheckAutoReborn()
    {
        if (BattleMain.IsModePvP(battleType))
            return;
        if (!isLocalActor || pauseAI)
            return;
#if ROBOT_TEST

#else
        
        if (PlayerBaseData.GetInstance().VipLevel == 0)
            return;
        if (!BeUtility.CheckVipAutoReborn())
            return;
#endif
        BattlePlayer localPlayer = BattleMain.instance.GetLocalPlayer();
        if (!localPlayer.CanUseItem(Battle.DungeonItem.eType.RebornCoin, 1) && DungeonUtility.GetVipRebornLeftCount() <= 0)
            return;
        BeUtility.PlayerReborn(this);
    }

    //移除选择玩家技能摇杆
    private void RemoveSkillJoyStick()
    {
#if !LOGIC_SERVER
        if (!isLocalActor)
            return;
        if (InputManager.instance == null)
            return;
        InputManager.instance.DisableSkillJoystick();
#endif
    }

    public void ShowTransport(bool isShow)
    {
        if (isSpecialMonster)
        {
            if (GetOwner() != null && GetOwner().m_pkGeActor != null)
                GetOwner().m_pkGeActor.ShowTranport(isShow);
        }
        else if (m_pkGeActor != null)
        {
            m_pkGeActor.ShowTranport(isShow);
        }
    }
}
