using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using System;
using ProtoTable;
using System.ComponentModel;

public struct Operation
{
    public enum OP
    {
        NONE = 0,
        ADD = 1,
        REPLACE
    }

    public enum TARGET
    {
        NONE = 0,
        SKILL = 1,
        ACTOR
    }

    public OP op;
    public TARGET target;
    public string varName;
    public int value;
    public int[] skillIDs;
}

public enum SkillMaigcType
{
    NONE = 0,
    PHYSIC = 1,
    MAGIC = 2
}

public enum SkillWalkMode
{
    [Description("禁止移动")]
    FORBID = 0,

    [Description("所有方向都能走")]
    FREE,

    [Description("只能往面朝的方向走")]
    FACEDIR,

    [Description("变换方向")]
    CHANGE_DIR,

    [Description("自由变换方向")]
    FREE_AND_CHANGEDIR,

    [Description("只能X轴")]
    X_ONLY,

    [Description("只能Y轴")]
    Y_ONLY,

    [Description("只能往面朝的反方向走")]
    FACEDIR_OPPOSITE,

    [Description("Y轴方向接收方向输入（不影响原技能位移）")]
    X_YControl,

    [Description("所有方向都能走2(支持多阶段技能)")]
    FREE2,

    [Description("真正的禁止移动 将技能行走速率改为0")]
    FORBID2,

    [Description("X,Y已不同速率移动")]
    XYDiffRate
}

public enum SkillJoystickMode
{
    [Description("默认")]
    NONE = 0,

    [Description("自由移动")]
    FREE,

    [Description("方向控制")]
    DIRECTION,

    [Description("用于特殊操作")]
    SPECIAL,

    [Description("选择玩家")]
    SELECTSEAT,

    [Description("前后选择")]
    FORWARDBACK,

    [Description("模式选择")]
    MODESELECT,

    [Description("技能选择")]
    ACTIONSELECT,
}

public enum SkillCostMode
{
	ALL = 0,
	MP_ONLY,	//只消耗MP
	HP_ONLY,	//只消耗HP
}

public enum SkillSpeedEffectType
{
	NONE = 0,
	ATTACK_SPEED,
	SPELL_SPEED,
}

public enum SkillPressMode
{
	NORMAL = 0,
	TWO_PRESS,//红色框,技能内
	PRESS_AGAIN_CANCEL,//会出现“技能取消”，技能内
	TWO_PRESS_OUT,
	PRESS_AGAIN_CANCEL_OUT,
	BUFF_INDICATOR,//只是显示
	PRESS_MANY, //按多次
    PRESS_JOYSTICK,  //每次按下都显示技能摇杆
    PRESS_MANY_TWO  //按多次 处于正在释放技能状态也能响应
}

//public enum Skill

public partial class BeSkill
{
    public enum InnerState
    {
        NONE = 0,
        CHOOSE_TARGET,      //选择目标
        LAUNCH    //目标选择结束 发射 
    }

	public enum SkillState
	{
		NORMAL = 0,
		WAIT_FOR_NEXT_PRESS,
	}

    protected struct SummonEffectData
    {
        public int summonId;
        public int summonNum;
        public int singleNumLimit;
        public int groupNumLimit;
        public int summonGroup;
    }
    protected SummonEffectData summonEffectData;
    public int skillID;
    public bool mIsIgnoreCD = false;
    protected bool createdInBackMode = false;
    public void SetIgnoreCD(bool isIgnore)
    {
        if (isIgnore)
        {
            ResetCoolDown();
        }
        mIsIgnoreCD = isIgnore;
    }
#if !SERVER_LOGIC

    private ETCButton _button = null;

    public ETCButton button { 
		get
		{
			return _button;
		}
		set
		{
			_button = value;
			if (needSetIcon && iconPath != null)
				InputManager._setButtonImageByPath(_button, iconPath, 0.7f);
		}
	}
   

#endif

    public BeActor owner { get; set; }//技能拥有者

#if !SERVER_LOGIC 
    public static FrameRandomImp randomForTown = new FrameRandomImp();

 #endif


    public FrameRandomImp FrameRandom
    {
        get
        {
#if !SERVER_LOGIC 
            if(owner.FrameRandom == null)
            {
                return randomForTown;
            }

 #endif

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

    public BattleType  battleType
    {
        get
        {
            if(owner.CurrentBeBattle == null)
            {
                return BattleType.Dungeon;
            }

            return owner.CurrentBeBattle.GetBattleType();
        }
    }
    public int raceType
    {
        get
        {
            if (owner.CurrentBeBattle == null)
            {
                return -1;
            }

            return owner.CurrentBeBattle.PkRaceType;
        }
    }
    public BeProjectilePoolImp BeProjectilePool
    {
        get
        {
            return owner.CurrentBeBattle.BeProjectilePool;
        }
    }

    public BeAICommandPoolImp BeAICommandPool
    {
        get
        {
            return owner.CurrentBeBattle.BeAICommandPool;
        }
    }
    public ProtoTable.SkillTable skillData;
    public int skillCategory;

    public List<SkillTable.ePreCondition> preConditions { get; set; }

    public CrypticInt32 mpCost;

    public bool isBuffSkill = false;
    public CrypticInt32 hpCost;
    public CrypticInt32 hpCostPercent;
    public CrypticInt32 crystalCost;
    public CrypticInt32 skillSpeed = GlobalLogic.VALUE_1000;
    public bool canChangeWeapon = false;
    public bool needSetIcon;
    public string iconPath = null;
    public int comboSkillSourceID = 0;//Combo技能的第一个技能ID(技能过程中切换武器要重置的)
    public bool inTown = false;

    //受加速影响,读表
    public CrypticInt32 skillSpeedFactor = GlobalLogic.VALUE_1000;

    //触发概率
    public int useRate { get; set; }

    //表格CD
    public CrypticInt32 tableCD = 0;
#if MG_TEST_EXTENT
    protected CrypticInt32 cdTimeAcc = 0;
#else
    protected int cdTimeAcc = 0;
#endif
    int useTimeAcc = 0;
    int cdAtStartCoolDown = 0;

    //冒泡喊话
    protected string skillSpeech = null;
    protected int skillSpeechRand = 0;
    //对应怪物喊话表
    protected int skillSpeechID = 0;

    int useCount = 0;

    IBeEventHandle eventHandler;
    protected IBeEventHandle hander1;
    protected IBeEventHandle hander2;

    public BDStat skillState = new BDStat();
    protected bool canceled = false;

    public bool pressedForwardMove = false;
    public bool pressedBackwardMove = false;
    public bool useInternalID = false;
    //public float startTime = 0;

    public ButtonState buttonState = ButtonState.NONE;

    public  ChargeConfig chargeConfig = Global.gStaticChargeData; /*readonly*/
    public bool charged = false;
    public int pressDuration;

    public bool specialOperate = false;
    public  OperationConfig operationConfig = Global.gStaticOpConfig; /*readonly*/
    public int specialChoice = 0;
    public BeActor joystickSelectActor = null;      //选择的玩家

    public int[] actionSelect = null;
    public string[] actionIconPath = null;
    public int actionChoice = 0;
    public int actionSelectPhase = 0;

    protected IBeEventHandle handleA = null;
    protected IBeEventHandle handleB = null;
    protected IBeEventHandle handleC = null;

    SkillEvent[] skillEvents = null;
    public bool cancelByCombo = false;
    public bool isComboSkill = false;
    public int comboSourceID = 0;//技能释放过程中不变的sourceid，给技能冷却用
    public bool canSwitchWeapon
    {
        get
        {
            return skillData.CanSwithWeapon == 1;
        }
    }

    public virtual bool isCooldown
    {
        set
        {
            if (_coolDown > 0 && !value)
            {
                owner.TriggerEventNew(BeEventType.OnSkillCoolDown, new EventParam {m_Int = skillID});
            }
            
            _coolDown = value ? 1 : 0;
        }
        get
        {
            return _coolDown > 0 ? true : false;
        }
    }

    //public bool isCooldown = false;

    public CrypticInt32 _coolDown = 0;

    public SkillPressMode pressMode = SkillPressMode.NORMAL;
    public SkillState skillButtonState = SkillState.NORMAL;
    protected Dictionary<int, int> jobInterruptSkills = new Dictionary<int, int>();
    protected List<int> interruptSkills = new List<int>();
    protected List<int> hitInterruptSkills = new List<int>();

    protected CrypticInt32 _level = 1;
    public bool isQTE = false;
    public bool isRunAttack = false;
    public bool canSlide = false;           //支持摇杆滑动
    public bool canUse = false;
    bool canUseWithRightWeapon = true;

    public bool walk = false;               //技能是否能行走
    public VFactor walkSpeed = VFactor.zero;//技能行走速度
    public bool charge = false;             //技能是否能蓄力
    public bool hideSpellBar = false;       //是否隐藏蓄力条
    protected bool chargeCD = false;        //蓄力CD
    protected int chargeCDTimeAcc = 0;      //蓄力时间间隔
    protected bool _hit = false; 
    protected bool cdFlag = false;          //用于记录技能是否CD过
    public bool isCharging = false;         //是否正在蓄力
    public bool joystickHasMove = false;    //表示技能摇杆是否移动过
    protected string jumpBackImage = "UI/Image/NewPacked/NewBattle/Battle_Skill.png:Battle_Skill_Houtiao";
    protected string jumpBackCancelImage = "UI/Image/NewPacked/NewBattle/Battle_Skill.png:Battle_Skill_Quxiao";

    protected SkillMaigcType skillMagicType = SkillMaigcType.NONE;
    protected int skillWalkMode = 0;

    private int _CurSkillWalkMode = 0;  //当前的移动模式
    public int CurSkillWalkMode
    {
        get
        {
            return _CurSkillWalkMode;
        }
        set
        {
            _CurSkillWalkMode = value;
        }
    } 

    protected int curPhase = 0;

    public bool hit
    {
        get
        {
            return _hit;
        }
        set
        {
            _hit = value;
        }
    }

    //cost mode
    public SkillCostMode costMode = SkillCostMode.MP_ONLY;
    
    //技能等级
    public int level
    {
        get
        {
            return _level;
        }
        set
        {
            _level = value;
            if (skillData != null)
            {
                _level = Mathf.Min(skillData.TopLevel, _level);
            }

            if (_level < 1)
            {
                _level = 1;
            }

            InitValues();
            if (owner != null)
            {
                var entity = owner.GetEntityData();
                if (entity != null)
                {
                    entity.UpdateLevel(skillID, _level);
                }
            }
        }
    }

    //!!技能計算還是浮點
    //技能速度增加百分比
    public VRate speedAddRate = 0;
    //技能命中率提升
    public VRate hitRateAdd = 0;
    //技能暴击率提升
    public VRate criticalHitRateAdd = 0;
    //技能攻击力增加百分比
    public VRate attackAddRate = 0;
    //技能攻击百分比增加值
    public CrypticInt32 attackAdd = 0;
    //技能攻击固定值增加值
    public CrypticInt32 attackAddFix = 0;
    //技能mp缩减百分比
    public VRate mpCostReduceRate = 0;
    //技能cd缩短百分比
    public VRate cdReduceRate = 0;
    //技能cd缩短固定值
    public CrypticInt32 cdReduceValue = 0;
    //僵直增加百分比
    public VRate hardAddRate = 0;
    //蓄力时间缩减百分比
    public VRate chargeReduceRate = 0;

    protected VFactor skillZdimFactor = VFactor.one;

    public bool canPressJumpBackCancel = false;

    public Dictionary<int, BuffInfoData> buffEnhanceList = new Dictionary<int, BuffInfoData>();
    
   // private List<BeMechanism> skillMechanismList = new List<BeMechanism>();
    private List<int> skillMechanismPIDList = new List<int>();

    private bool _forceShowButtonImage = false;
    public bool ForceShowButtonImage { get { return _forceShowButtonImage; }set { _forceShowButtonImage = value; } }

    //技能开始阶段 技能执行直接跳转到指定阶段
    private int _skillStartPahse = 0;

    public BeSkill(int sid, int skillLevel)
    {
        skillID = sid;
        level = skillLevel;

        skillData = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(skillID);

        if (skillData == null)
        {
            Logger.LogErrorFormat("技能表 没有ID为 {0} 的技能", skillID);
        }

        ChangeSummonData();
        //Init();
    }

    public void InitValues()
    {
        if (skillData != null)
        {
            mpCost = TableManager.GetValueFromUnionCell(skillData.MPCost, level);
            hpCost = TableManager.GetValueFromUnionCell(skillData.HPCost, level);
            hpCostPercent = TableManager.GetValueFromUnionCell(skillData.HPCostPercent, level);
            if (this.raceType != (int)Protocol.RaceType.ChiJi && this.raceType != (int)Protocol.RaceType.PK_EQUAL_1V1)
            {
                crystalCost = TableManager.GetValueFromUnionCell(skillData.CrystalCost, level);
            }
            else
            {
                crystalCost = 0;
            }

            useRate = TableManager.GetValueFromUnionCell(skillData.Probability, level);

            int value = TableManager.GetValueFromUnionCell(BattleMain.IsModePvP(battleType) ? skillData.PVPZscale : skillData.Zscale, level);
            if (value > 0)
            {
                skillZdimFactor = new VFactor(value, GlobalLogic.VALUE_1000);//value / (float)(GlobalLogic.VALUE_1000);
            }

            if (BattleMain.IsChijiNeedReplaceSkillId(skillID,battleType))
            {
                var chijiSkillMapTable = TableManager.instance.GetTableItem<ChijiSkillMapTable>(skillID);
                tableCD = TableManager.GetValueFromUnionCell(chijiSkillMapTable.RefreshTimePVP, level);
            }
            else
            {
                tableCD = (BattleMain.IsModePvP(battleType) ? TableManager.GetValueFromUnionCell(skillData.RefreshTimePVP, level) : TableManager.GetValueFromUnionCell(skillData.RefreshTime, level));
            }
        }
    }

    public void Init()
    {
        if (skillData != null)
        {
            InitValues();

            skillCategory = skillData.SkillCategory;

            //useRate = Mathf.Clamp(useRate, 0, 1.0f);

            canChangeWeapon = skillData.CanSwithWeapon > 0;
            canPressJumpBackCancel = skillData.PressBackJumpCancel > 0;

            skillSpeedFactor = skillData.Speed;

            charge = skillData.IsChargeEnable;
            walk = skillData.IsWalkEnable;
            hideSpellBar = skillData.HideSpellBar==1;
            isBuffSkill = skillData.IsBuff > 0;
            costMode = (SkillCostMode)skillData.CostMode;


            pressMode = (SkillPressMode)skillData.SkillPressType;

            isQTE = skillData.IsQTE != 0;

            isRunAttack = skillData.IsRunAttack != 0;

            skillSpeech = skillData.SkillSpeech;
            skillSpeechRand = skillData.SkillSpeechRand;
            skillSpeechID = skillData.SkillSpeechID;


            skillWalkMode = skillData.WalkMode;


            SetInterruptSkills();

            for (int i = 0; i < skillData.HitInterruptSkills.Count; ++i)
            {
                int sid = skillData.HitInterruptSkills[i];
                if (sid > 0 && !hitInterruptSkills.Contains(sid))
                    hitInterruptSkills.Add(sid);
            }

            if (skillData.SkillEffect.Contains(ProtoTable.SkillTable.eSkillEffect.PHYSICAL_SKILL))
                skillMagicType = SkillMaigcType.PHYSIC;
            else if (skillData.SkillEffect.Contains(ProtoTable.SkillTable.eSkillEffect.MAGIC_SKILL))
                skillMagicType = SkillMaigcType.MAGIC;
            CurSkillWalkMode = skillData.WalkMode;

            if (preConditions == null)
            {
                preConditions = new List<SkillTable.ePreCondition>();
            }
            preConditions.Clear();
            for (int i = 0; i < skillData.PreCondition.Count; i++)
            {
                var c = skillData.PreCondition[i];
                preConditions.Add(c);
            }
        }
        OnInit();
    }

    /// <summary>
    /// 预加载资源
    /// </summary>
    public virtual void PreloadRes()
    {

    }

    public void DeInit()
    {
        OnDeInit();
    }

    private void SetInterruptSkills()
    {
        if (!string.IsNullOrEmpty(skillData.InterruptSkills))
        {
            if (!skillData.InterruptSkills.Contains(":"))
            {
                string[] skills = skillData.InterruptSkills.Split('|');
                for (int i = 0; i < skills.Length; i++)
                {
                    int id = 0;
                    int.TryParse(skills[i], out id);
                    if (id > 0 && !interruptSkills.Contains(id))
                        interruptSkills.Add(id);
                }
            }
            else
            {
                string[] skills = skillData.InterruptSkills.Split('|');
                for (int i = 0; i < skills.Length; i++)
                {
                    string[] s = skills[i].Split(':');
                    int job = 0;
                    int id = 0;
                    int.TryParse(s[0], out job);
                    int.TryParse(s[1], out id);
                    if (job != 0 && id != 0 && !jobInterruptSkills.ContainsKey(job))
                    {
                        jobInterruptSkills[id] = job;
                    }
                }

            }
        }
    }

    public void InitForPassive()
    {
        if (skillData == null)
            return;

        if (skillData.SkillType == ProtoTable.SkillTable.eSkillType.PASSIVE)
        {
            //foreach (var condition in skillData.PreCondition)
            for (int i = 0; i < preConditions.Count; ++i)
            {
                var condition = preConditions[i];
                //被击触发
                if (condition == ProtoTable.SkillTable.ePreCondition.BEHIT)
                {
                    if (owner != null)
                    {
                        eventHandler = owner.RegisterEventNew(BeEventType.onHurt, args =>
                        //eventHandler = owner.RegisterEvent(BeEventType.onHurt, (object[] args) =>
                        {
                            DealEffectFrame();
                        });
                    }
                }
                else if (condition == ProtoTable.SkillTable.ePreCondition.INIT)
                {
                    DealEffectFrame();
                    ExecuteOperation(ParseOperation());
                }
                else if (condition == ProtoTable.SkillTable.ePreCondition.LOWHP)
                {
                    if (owner != null)
                    {
                        if (skillData.ValueA.Count >= 1)
                        {
                            int value = TableManager.GetValueFromUnionCell(skillData.ValueA[0], 1);
                            bool isTrigger = false;

                            // 状态改变时，检查低血量暴怒
                            eventHandler = owner.RegisterEventNew(BeEventType.onStateChange, (param) =>
                                {
                                    var data = owner.GetEntityData();
                                    if (null != data)
                                    {
                                        //float leftRate = data.GetHPRate() * GlobalLogic.VALUE_1000;//data.battleData.hp * 1.0f / data.battleData.maxHp * 1000;
                                        VFactor leftRate = data.GetHPRate();
                                        VFactor fvalue = new VFactor(value,GlobalLogic.VALUE_1000);
                                        if (!isTrigger && leftRate < fvalue)
                                        {
                                            isTrigger = true;

                                            DealEffectFrame();
                                        }
                                    }
                                });
                        }
                    }
                }

            }
        }

        AddBuffs();
        AddMechanisms();
    }
    
    //添加技能表中的Buff
    public void AddBuffs()
    {
        if (skillData != null && owner != null)
        {
            if (skillData.BuffInfoIDs[0] != 0)
            {
                var list = BattleMain.IsModePvP(battleType)?skillData.BuffInfoIDsPVP:skillData.BuffInfoIDs;

                owner.RemoveMechanismBuffs(list);
                owner.LoadMechanismBuffs(list, level);
            }
                
        }
    }

    //添加技能表中的机制
    public void AddMechanisms()
    {
        if(skillData!=null && owner != null)
        {
            if (skillData.MechismIDs[0] != 0)
            {
                var list = BattleMain.IsModePvP(battleType) ? skillData.MechismIDsPVP : skillData.MechismIDs;
                for (int i=0;i< list.Count; i++)
                {
                    var mech = owner.AddMechanism(list[i],level);
                    if (mech != null)
                        skillMechanismPIDList.Add(mech.PID);
                    //skillMechanismList.Add();
                }
            }
        }
    }

    //移除技能表中添加的机制
    private void RemoveMechanisms()
    {
        if (skillData == null)
            return;
        if (owner == null)
            return;
            /*
        for(int i=0;i< skillMechanismList.Count; i++)
        {
            owner.RemoveMechanism(skillMechanismList[i]);
        }
        skillMechanismList.Clear();*/
        // foreach(var pid in skillMechanismPIDList)
        // {
        //     owner.RemoveMechanismByPID(pid);
        // }
		// marked by ckm
        for (int i = 0; i < skillMechanismPIDList.Count; i++)
        {
            int pid = skillMechanismPIDList[i];
            owner.RemoveMechanismByPID(pid);
        }
        skillMechanismPIDList.Clear();
    }

    public void DealEffectFrame()
    {
        //删除buff
        var hitEffectIDs = BattleMain.IsModePvP(battleType) ? skillData.HitEffectIDsPVP : skillData.HitEffectIDs;
        for (int i = 0; i < hitEffectIDs.Count; ++i)
        {
            var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hitEffectIDs[i]);
            if (hurtData != null && owner != null)
            {
                if (hurtData.BuffID > 0)
                    owner.buffController.RemoveBuff(hurtData.BuffID);
                else
                {
                    for (int j = 0; j < hurtData.BuffInfoID.Count; ++j)
                    {
                        if (hurtData.BuffInfoID[j] > 0)
                            owner.buffController.RemoveBuffByBuffInfoID(hurtData.BuffInfoID[j]);
                    }

                    if (BattleMain.IsChijiNeedReplaceHurtId(hitEffectIDs[i], battleType))
                    {
                        var chijiEffectMapTable = TableManager.instance.GetTableItem<ChijiEffectMapTable>(hitEffectIDs[i]);
                        for (int j = 0; j < chijiEffectMapTable.PVPBuffInfoID.Count; ++j)
                        {
                            if (chijiEffectMapTable.PVPBuffInfoID[j] > 0)
                                owner.buffController.RemoveBuffByBuffInfoID(chijiEffectMapTable.PVPBuffInfoID[j]);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < hurtData.PVPBuffInfoID.Count; ++j)
                        {
                            if (hurtData.PVPBuffInfoID[j] > 0)
                                owner.buffController.RemoveBuffByBuffInfoID(hurtData.PVPBuffInfoID[j]);
                        }
                    }
                }
            }
        }

        if (FrameRandom.Range1000() <= useRate)
        {
            for (int i = 0; i < hitEffectIDs.Count; ++i)
            {
                owner.DealEffectFrame(null, hitEffectIDs[i]);

                var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hitEffectIDs[i]);
                if (hurtData != null && hurtData.BuffID > 0)
                {
                    var buff = owner.buffController.HasBuffByID(hurtData.BuffID);
                    if (buff != null)
                        buff.passive = true;
                }

            }
        }
    }

    /*
    在这一步的时候owner已经设置
    */
    public void PostInit()
    {
        JoyStickPostInit();
        SetOrignWalkSpeed();
        
        InitForPassive();

        if (owner != null)
        {
            SetComboSourceSkillID();
            var skillData = owner.GetSkillActionInfo(skillID);
            if (skillData != null)
            {
                if (skillData.useCharge)
                {
                    chargeConfig = skillData.chargeConfig;
                }

                //注册事件
                if (skillData.skillEvents != null)
                {
                    skillEvents = skillData.skillEvents;
                }

                if (skillData.useSpecialOperation)
                {
                    specialOperate = true;
                    operationConfig = skillData.operationConfig;
                }

                if (skillData.skillJoystickConfig.mode == SkillJoystickMode.ACTIONSELECT)
                {
                    actionSelectPhase = skillData.actionSelectPhase;
                    actionSelect = skillData.actionSelect;
                    actionIconPath = skillData.actionIconPath;
                }
            }

            this.isComboSkill = comboSkillSourceID > 0;
        }

        // weapon
        UpdateCanUseSkillWithRightWeapon();

        OnPostInit();
		if(owner != null)
        	owner.TriggerEventNew(BeEventType.onSkillPostInit,new EventParam(){m_Int = skillID});
    }

    private void SetComboSourceSkillID()
    {
        if (skillData.SkillCategory == 3)
        {
            bool first = true;
            int cnt = 0;
            var data = owner.GetActionInfoBySkillID(skillID);
            if (data == null) return;
            do
            {
                if (cnt >= 10) break;//循环保护
                if (data != null && data.comboSkillID != 0)
                {
                    var comboSkill = owner.GetSkill(data.comboSkillID);
                    if (comboSkill == null)
                        break;

                    if (first)
                    {
                        first = false;
                        comboSkillSourceID = skillID;
                    }
                    comboSkill.comboSkillSourceID = skillID;
                    cnt++;
                    data = owner.GetActionInfoBySkillID(data.comboSkillID);
                }
                else
                    break;


            } while (true);
        }
    }


    //设置初始技能行走速度
    protected void SetOrignWalkSpeed()
    {
        try
        {
            int speedRate = GlobalLogic.VALUE_1000;
            if (BattleMain.IsModePvP(battleType))
            {
                speedRate = TableManager.GetValueFromUnionCell(skillData.WalkSpeedPVP, level);
            }
            else
            {
                speedRate = TableManager.GetValueFromUnionCell(skillData.WalkSpeed, level);
            }
            walkSpeed = new VFactor(speedRate, GlobalLogic.VALUE_1000);
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat("SetOrignWalkSpeed方法中的技能id为：{0}错误信息为：{1}", skillID.ToString(),e.ToString());
        }
    }

    public void DealSkillEvents(EventCommand skillEvent)
    {
        if (skillEvents == null)
            return;

        for (int i = 0; i < skillEvents.Length; ++i)
        {
            var se = skillEvents[i];
            if (se.eventType == skillEvent && (se.workPhase == 0 || se.workPhase == curPhase ))
            {
                if ((se.eventType == EventCommand.EVENT_COMMAND_PRESS_JOYSTICK || 
                    se.eventType == EventCommand.EVENT_COMMAND_RELEASE_JOYSTICK) && !walk)
                    return;

               // Logger.LogErrorFormat("curPhase:{0} se.workPahse:{1}", curPhase, se.workPhase);
                if (se.eventAction == SkillAction.CHANGE_ANIMATION)
                {
                    if (owner as BeActor != null)
                    {
                        //播放挂件动画
                        if (!string.IsNullOrEmpty(se.attachActionName) 
                            && owner.m_cpkEntityInfo != null 
                            && owner.m_cpkEntityInfo.HasAction(se.attachActionName))
                        {
                            var actionInfo = owner.m_cpkEntityInfo._vkActionsMap[se.attachActionName];
                            owner.attachmentproxy.Init(actionInfo);
                            owner.attachmentproxy.Update(0);
                        }

                        (owner as BeActor).ChangeAnimation(se.paramter, true);
                        owner.TriggerEventNew(BeEventType.onSkillEventChangeAnimation,
                            new EventParam() {m_Int = skillID});
                    }
                }
                else if (se.eventAction == SkillAction.DISPOSE_SKILL)
                {


                    //owner.CancelSkill(Int32.Parse(se.paramter));
                    owner.FinishSkill(Int32.Parse(se.paramter));
                    owner.GetStateGraph().LocomoteState();

                }
            }

        }
    }

    float startTime;

    public void Start()
    {
        //         if (Global.Settings.isDebug)
        //         {
        //             startTime = Time.realtimeSinceStartup * 1000;
        //             Logger.LogErrorFormat("SSSSSSSSTART skill:{0} time:{1}", skillID, startTime);
        //         }
        cancelByCombo = false;
        this.isComboSkill = comboSkillSourceID!=0;
        comboSourceID = comboSkillSourceID;
        useCount++;
        canceled = false;
        hit = false;
        curPhase = 0;
        cdFlag = false;
        skillState.SetRunning();

        JoyStickStart();
        if (skillData.SwitchSkillID > 0)
        {
            useInternalID = CheckSpellSituation();
        }

        //startTime = Time.realtimeSinceStartup;
        buttonState = ButtonState.PRESS;
        pressDuration = 0;
        isCharging = false;
        useTimeAcc = 0;
        charged = false;

        if (pressMode == SkillPressMode.BUFF_INDICATOR)
        {
            if (owner != null)
            {

#if !SERVER_LOGIC

				if (hander1 == null)
					hander1 = owner.RegisterEventNew(BeEventType.onAddBuff, (args)=>{
						var buff = args.m_Obj as BeBuff;
						if (buff != null)
						{
                            if (buff.buffID == skillData.WatchBuffID)
                                if (button != null) 
									button.StartBuffCD(buff);
						}
					});

				if (hander2 == null)
					hander2 = owner.RegisterEventNew(BeEventType.onRemoveBuff, (args)=>{
						var buff = args.m_Obj as BeBuff;
						if (buff != null)
						{
                            if (buff.buffID == skillData.WatchBuffID)
                                if (button != null)
									button.StopBuffCD();
						}
					});

#endif

            }
        }


        OnStart();
        LoadJoystickUI();

        if (NeedCoolDown() && skillData.CDPhase == 0&& !isComboSkill)
            StartCoolDown();

        ChangeButtonState();
        ChangeJumpBackImage(false);
        ShowSpeech();
        ResetMoveZAccExtra();
        SpecialSkillStart();
        owner.TriggerEventNew(BeEventType.onSkillStart, new EventParam() { m_Int = skillID, m_Obj = this });
    }

    public VFactor GetSkillZDimFactor()
    {
        return skillZdimFactor;
    }

    public void ShowSpeech()
    {
        if (owner != null && owner.GetEntityData() != null && owner.GetEntityData().isShowSkillSpeech)
        {
            if ((!string.IsNullOrEmpty(skillSpeech) || skillSpeechID != 0) && skillSpeechRand > 0 && FrameRandom.InRange(0, GlobalLogic.VALUE_1000) < skillSpeechRand)
            {
                bool useSkill = true;
                bool isPet = owner.GetEntityData().isPet;
                if (isPet)
                    useSkill = false;


                //owner.m_pkGeActor.ShowHeadDialog(skillSpeech, false, false, false, useSkill, 3.5f, isPet);
                if(skillSpeechID != 0)
                {
                    owner.ShowSpeechWithID(skillSpeechID, isPet);
                }
#if !SERVER_LOGIC
                else if(owner.m_pkGeActor != null)
                {
                    owner.m_pkGeActor.ShowHeadDialog(skillSpeech, false, false, false, useSkill, 3.5f, isPet);
                }
#endif

            }
            ShowSkillTip();                                 //技能冒泡与技能提示飘字分开显示
        }
    }

    public void ShowSkillTip()
    {
#if !LOGIC_SERVER
		if (skillData != null && Utility.IsStringValid(skillData.SkillDealText))
        {
            float duration = 5000;
            if (skillData.SkillDealTextDuration > 0)
                duration = skillData.SkillDealTextDuration;
            SystemNotifyManager.SysDungeonSkillTip(skillData.SkillDealText, duration / 1000f);
        }
#endif
    }

    public void ChangeButtonState()
    {
        if (pressMode == SkillPressMode.PRESS_AGAIN_CANCEL)
        {
#if !SERVER_LOGIC

			if (button!= null)
				button.AddEffect(ETCButton.eEffectType.onBreak, skillData.IsBuff>0);

#endif

        }
        else if (pressMode == SkillPressMode.TWO_PRESS)
        {
#if !SERVER_LOGIC

			if (button!= null)
				button.AddEffect(ETCButton.eEffectType.onContinue, skillData.IsBuff>0);

#endif

            skillButtonState = SkillState.WAIT_FOR_NEXT_PRESS;
        }
        else if (pressMode == SkillPressMode.PRESS_AGAIN_CANCEL_OUT)
        {
#if !SERVER_LOGIC

			if (button!= null)
				button.AddEffect(ETCButton.eEffectType.onBreak, skillData.IsBuff>0);

#endif

            skillButtonState = SkillState.WAIT_FOR_NEXT_PRESS;
        }
        else if (pressMode == SkillPressMode.TWO_PRESS_OUT)
        {
#if !SERVER_LOGIC

			if (button!= null)
				button.AddEffect(ETCButton.eEffectType.onContinue, skillData.IsBuff>0);

#endif

            skillButtonState = SkillState.WAIT_FOR_NEXT_PRESS;
        }
        else if (pressMode == SkillPressMode.PRESS_MANY)
        {
            skillButtonState = SkillState.WAIT_FOR_NEXT_PRESS;
        }
        else if(pressMode == SkillPressMode.PRESS_MANY_TWO)
        {
            skillButtonState = SkillState.WAIT_FOR_NEXT_PRESS;
        }
    }

    public void RestoreButtonState()
    {
        if (pressMode == SkillPressMode.PRESS_AGAIN_CANCEL)
        {
#if !SERVER_LOGIC

			if (button!= null)
				button.RemoveEffect(ETCButton.eEffectType.onBreak, skillData.IsBuff>0);

#endif

        }
        else if (pressMode == SkillPressMode.TWO_PRESS)
        {
#if !SERVER_LOGIC

			if (button!= null)
				button.RemoveEffect(ETCButton.eEffectType.onContinue, skillData.IsBuff>0);


#endif
            skillButtonState = SkillState.NORMAL;
        }
    }

    public void RestoreButtonStateOut()
    {
        if (pressMode == SkillPressMode.PRESS_AGAIN_CANCEL_OUT)
        {
#if !SERVER_LOGIC

			if (button!= null)
				button.RemoveEffect(ETCButton.eEffectType.onBreak, skillData.IsBuff>0);

#endif

            skillButtonState = SkillState.NORMAL;
        }
        else if (pressMode == SkillPressMode.TWO_PRESS_OUT)
        {
#if !SERVER_LOGIC

			if (button!= null)
				button.RemoveEffect(ETCButton.eEffectType.onContinue, skillData.IsBuff>0);

#endif

            skillButtonState = SkillState.NORMAL;
        }
    }

    public virtual bool NeedCost()
    {
        return true;
    }

    public void SetButtonRelease()
    {
        buttonState = ButtonState.RELEASE;
        //按钮状态改变后pressDuration会停止累加，即为这次的蓄力时间
        //pressDuration = Time.realtimeSinceStartup - startTime;
        EndJoystick();
    }

    public void SetButtonPressAgain()
    {
        if (buttonState == ButtonState.PRESS_AGAIN)
            return;

        if (pressMode == SkillPressMode.PRESS_AGAIN_CANCEL && !CanCancel())
            return;

        buttonState = ButtonState.PRESS_AGAIN;
        RestoreButtonState();
        if (pressMode == SkillPressMode.PRESS_AGAIN_CANCEL)
        {
            if (owner != null)
            {
                owner.Locomote(new BeStateData((int)ActionState.AS_IDLE));
            }
        }
    }

    //设置蓄力CD时间
    public void SetChargeCdTime(int time)
    {
        chargeCD = true;
        chargeCDTimeAcc = time;
    }
		
    public bool CheckSpellSituation()
    {
        bool flag = false;
        VInt2 checkDis = new VInt2(
            VInt.NewVInt(skillData.RangeX,GlobalLogic.VALUE_1000).i, 
            VInt.NewVInt(skillData.RangeY,GlobalLogic.VALUE_1000).i
            );

        List<BeEntity> targets = GamePool.ListPool<BeEntity>.Get();
        VFactor shooterFitPercent = VFactor.NewVFactorF(Global.Settings.shooterFitPercent,1000);
        owner.CurrentBeScene.FindFaceTargets(
            targets, 
            owner as BeActor, 
            checkDis,shooterFitPercent, 
            VInt.NewVInt(skillData.BackRangeX,GlobalLogic.VALUE_1000)
            );
        for (int i = 0; i < targets.Count; ++i)
        {
            if ((targets[i] as BeObject) != null || !targets[i].HasTag((int)AState.ACS_FALL) || targets[i].HasTag((int)AState.AST_FALLGROUND))
            {
                flag = true;
                break;
            }
        }

        GamePool.ListPool<BeEntity>.Release(targets);

        return flag;
    }

    public void Finish()
    {
        //   if (Global.Settings.isDebug)
        //     Logger.LogErrorFormat("FFFFFFFFFFFFFFFFinish skill:{0} time:{1} delta:{2}", skillID, Time.realtimeSinceStartup * 1000, Time.realtimeSinceStartup * 1000-startTime);
        isCharging = false;
        joystickHasMove = false;
        skillState.SetDead();
        RestoreCameraMove();
        RestoreButtonState();
        owner.RemoveRepeatDamage();
        ChangeJumpBackImage(true);
        OnFinish();
        StartComboSkillCoolDown();
        owner.buffController.TriggerBuffs(BuffCondition.RELEASE_SEPCIFY_SKILL_COMPLETE, null, skillID);
        owner.TriggerEventNew(BeEventType.onCastSkillFinish, new EventParam(){m_Int = skillID});
        RemoveAllHandles();
        ResetMoveZAccExtra();
        SpecialSkillFinish();
        owner.grabController.ResetGrabPositionInfo();
    }
    public bool isFinish()
    {
        return skillState.GetState() == BDStat.State.DEAD;
    }
    private void StartComboSkillCoolDown(bool onlyCheckComboSkill = true)
    {
        if (!cdFlag)
        {
            if (isComboSkill)
            {
                if (comboSourceID != 0)
                {
                    BeSkill skill = owner.GetSkill(comboSourceID);
                    if (skill != null)
                    {
                        skill.StartCoolDown();
                    }
                }
                else
                    StartCoolDown();
            }
            else if (!onlyCheckComboSkill)
                StartCoolDown();
        }
    }





    public bool IsCanceled()
    {
        return canceled;
    }
    private CrypticInt32 initTableCd = 0;
    public int LeftInitCd
    {
        get
        {
            int cd = GetCurrentCD();
            if (initTableCd < cd)
            {
                return cd - cdTimeAcc;
            }
            return initTableCd - cdTimeAcc;
        }
    }
    public void AccTimeCD(int accValue)
    {
        cdTimeAcc += accValue;
        if (cdTimeAcc >= GetCurrentCD())
        {
            FinishCoolDown();
        }
    }
    public void StartInitCD(bool isPvP = false)
    {
        int initCD = 0;
        if (BattleMain.IsChijiNeedReplaceSkillId(skillID, battleType))
        {
            var chijiSkillMapTable = TableManager.instance.GetTableItem<ProtoTable.ChijiSkillMapTable>(skillID);
            initTableCd = TableManager.GetValueFromUnionCell(chijiSkillMapTable.InitCDPVP, level);
        }
        else
        {
            if (skillData != null)
                initTableCd = isPvP ? TableManager.GetValueFromUnionCell(skillData.InitCDPVP, level) : TableManager.GetValueFromUnionCell(skillData.InitCD, level);
        }

        if (initTableCd > 0)
        {
            StartCoolDown();
            if (initCD < GetCurrentCD())
                cdTimeAcc = GetCurrentCD() - initTableCd;
        }
    }

    //改变后跳按钮图标
    protected void ChangeJumpBackImage(bool restore)
    {
#if !LOGIC_SERVER
        if (!canPressJumpBackCancel)
            return;
        if (!owner.isLocalActor)
            return;
        string imagePath = restore? jumpBackImage: jumpBackCancelImage;
        if (InputManager.instance != null)
        {
            ETCButton jumpBackBtn = InputManager.instance.GetSpecialETCButton(SpecialSkillID.JUMP_BACK);
            if (jumpBackBtn != null)
            {
                jumpBackBtn.SetFgImage(imagePath);
            }
        }
#endif
    }

    private bool CanCancel()
    {
        //!! 1000浮点转换
        return useTimeAcc >= VRate.Conver(Global.Settings.skillCancelLimitTime);
    }

    public void PressAgainCancel()
    {
        if (!CanCancel())
            return;

        OnClickAgainCancel();

        Cancel();
        skillButtonState = SkillState.NORMAL;
        RestoreButtonStateOut();
        StartCoolDown();
    }

    public void PressJoystick()
    {
        OnClickAgain();
    }

    public void PressAgainRelease()
    {
        OnClickAgain();

        Cancel();
        skillButtonState = SkillState.NORMAL;
        RestoreButtonStateOut();
    }

    public void PressMany()
    {
        OnClickAgain();
        SpecialSkillPressAgain();
    }

    public void PressAgain()
    {
        OnClickAgain();
        skillButtonState = SkillState.NORMAL;
        RestoreButtonStateOut();
    }

    public void Cancel()
    {
        if (canceled)
            return;

        canceled = true;
        isCharging = false;
        joystickHasMove = false;
        owner.RemoveRepeatDamage();
        RestoreCameraMove();
        RestoreButtonState();
        RestoreButtonStateOut();
        SetButtonRelease();
        OnCancel();
        ChangeJumpBackImage(true);
        if (!cdFlag)
        {
            CancelSkillStartCoolDown();
        }
        skillState.SetDead();
        RemoveAllHandles();
        ResetMoveZAccExtra();
        SpecialSkillCancel();
        owner.TriggerEventNew(BeEventType.onSkillCancel,new EventParam(){m_Int =  skillID});
    }


    private void CancelSkillStartCoolDown()
    {
        if (!cancelByCombo)//不是被自己的combo打断
        {
            StartComboSkillCoolDown(false);
        }       
      
    }

    public void RestoreCameraMove(bool isCancel = false)
    {
#if !LOGIC_SERVER
        var skillConfigData = owner.GetSkillActionInfo(skillID);
        if (skillConfigData != null)
        {
            float time = CheckResetCamera() && isCancel && !BattleMain.IsModePvP(battleType) ? 0.01f : skillConfigData.cameraRestoreTime;
            if (skillConfigData.cameraRestore && skillConfigData.cameraRestoreTime > 0f)
            {
                var cameraCtrl = owner.CurrentBeScene.currentGeScene.GetCamera().GetController();
                float off = -1 * cameraCtrl.GetOffset();
                cameraCtrl.MoveCamera(off, time);
            }
        }
#endif
    }

    public virtual bool CheckSpellCondition(ActionState state)
    {
        bool canuse = false;

        if (state == ActionState.AS_CASTSKILL)
        {
            for (int i = 0; i < preConditions.Count; ++i)
            {
                var c = preConditions[i];
                switch (c)
                {
                    case SkillTable.ePreCondition.PreCondition_None:
                    case SkillTable.ePreCondition.STAND:
                    case SkillTable.ePreCondition.RUN:
                    case SkillTable.ePreCondition.WALK:
                        canuse = true;
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            for (int i = 0; i < preConditions.Count; ++i)
            {
                var c = preConditions[i];
                switch (c)
                {
                    case ProtoTable.SkillTable.ePreCondition.STAND:
                        if (state == ActionState.AS_IDLE)
                            canuse = true;
                        break;
                    case ProtoTable.SkillTable.ePreCondition.RUN:
                        if (state == ActionState.AS_RUN)
                            canuse = true;
                        break;
                    case ProtoTable.SkillTable.ePreCondition.WALK:
                        if (state == ActionState.AS_WALK)
                            canuse = true;
                        break;
                    case ProtoTable.SkillTable.ePreCondition.JUMP:
                        if (owner.HasTag((int)AState.ACS_JUMP))
                            canuse = true;
                        break;
                    case ProtoTable.SkillTable.ePreCondition.BEHIT:
                        if (owner.sgGetCurrentState() == (int)ActionState.AS_HURT)
                            canuse = true;
                        break;
                    case ProtoTable.SkillTable.ePreCondition.DAODI:
                        if (owner.sgGetCurrentState() == (int)ActionState.AS_FALLGROUND)
                            canuse = true;
                        break;
                    case ProtoTable.SkillTable.ePreCondition.JUMP_BACK:
                        if (owner.sgGetCurrentState() == (int)ActionState.AS_JUMPBACK)
                            canuse = true;
                        break;
                }
                if (canuse)
                    break;
            }
        }

        // Buff
        bool hasBuffCondition = false;
        bool hasBuff = false;
        var ownBuffIds = skillData.OwnBuffID;
        for (int i = 0; i < preConditions.Count; ++i)
        {
            var condition = preConditions[i];
            if (condition == SkillTable.ePreCondition.OWN_BUFF)
            {
                hasBuffCondition = true;
                for (int k = 0; k < ownBuffIds.Count; k++)
                {
                    if (owner.buffController.HasBuffByID(ownBuffIds[k]) != null)
                    {
                        hasBuff = true;
                        break;
                    }
                }
                break;
            }
        }
        if (hasBuffCondition && !hasBuff)
        {
            canuse = false;
        }

        // LOWHP : 低血量的层级更高一点
        if (canuse && skillData.LowHpPercent > 0)
        {
            canuse = false;
            var data = owner.GetEntityData();
            if (null != data)
            {
                VFactor leftRate = data.GetHPRate();
                VFactor lowFactor  = new VFactor(skillData.LowHpPercent,GlobalLogic.VALUE_1000);
                if (leftRate < lowFactor)
                    canuse = true;
            }
        }

        return canuse;
    }

    public virtual bool CanUseSkill()
    {
        if (!CanUseInPvp())
            return false;
        bool ret = true;

#if DEBUG_FIGHT || UNITY_EDITOR
        if (!Global.Settings.skillHasCooldown)
            ret = true;
        else
#endif
        {
            //if (NeedCoolDown())
            ret = !isCooldown;
            if (BeClientSwitch.FunctionIsOpen(ClientSwitchType.SkillCanSummon))
            {
                ret = ret && CanSummon2();
            }
            else
            {
                ret = ret && CanSummon();
            }
        }

        if (isRunAttack && owner.hasRunAttackConfig)
        {
            ret = ret && (owner.sgGetCurrentState() == (int)ActionState.AS_RUN && owner.IsRunning());
        }
        
        var eventData = owner.TriggerEventNew(BeEventType.CanUseSkill, new EventParam(){m_Int = skillID, m_Bool = true});
        return (ret && canUseWithRightWeapon && eventData.m_Bool) || CanForceUseSkill();
    }

    public virtual BeSkillManager.SkillCannotUseType GetCannotUseType()
    {
        if (!canUseWithRightWeapon)
            return BeSkillManager.SkillCannotUseType.NO_RIGHT_WEAPON;
        if (!CanUseInPvp())
            return BeSkillManager.SkillCannotUseType.CAN_NOT_USE;

        return BeSkillManager.SkillCannotUseType.CAN_USE;
    }

    /// <summary>
    /// 该技能能否在pvp中使用
    /// </summary>
    /// <returns></returns>
    private bool CanUseInPvp()
    {
        if (skillData == null)
            return true;
        if (!BattleMain.IsModePvP(battleType))
            return true;
        if (skillData.CanUseInPVP == 3)
            return false;
        return true;
    }
	
	public bool CanSummon()
    {
        if (skillData.SummonRestrainEffectID > 0)
        {
            var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(skillData.SummonRestrainEffectID);
            if (hurtData != null && hurtData.SummonID > 0)
            {
                int summonNum = TableManager.GetValueFromUnionCell(hurtData.SummonNum, GetLevel());
                int singleNumLimit = hurtData.SummonNumLimit;
                int groupNumLimit = TableManager.GetValueFromUnionCell(hurtData.SummonGroupNumLimit, GetLevel());
                int summonGroup = hurtData.SummonGroup;

                int num = owner.GetSummonNum(hurtData.SummonID, summonNum, singleNumLimit, summonGroup, groupNumLimit);
                return num > 0;
            }
        }

        return true;
    }

    public bool CanSummon2()
    {
        if (skillData.SummonRestrainEffectID > 0 || summonEffectData.summonId > 0)
        {
            int num = owner.GetSummonNum(summonEffectData.summonId, summonEffectData.summonNum, summonEffectData.singleNumLimit, summonEffectData.summonGroup, summonEffectData.groupNumLimit);
            return num > 0;
        }
        return true;
    }

    public int GetMPCost()
    {
        VRate tmpMPCostReduceRate = mpCostReduceRate;
        if (owner != null)
            tmpMPCostReduceRate = tmpMPCostReduceRate + owner.GetEntityData().GetMPCostReduce(skillMagicType);

        return (mpCost * (VRate.one - tmpMPCostReduceRate).factor);
    }

    public int GetHPCost(int maxHP = 0)
    {
        if (hpCostPercent > 0)
        {
            return maxHP * VFactor.NewVFactor(hpCostPercent, (long)GlobalLogic.VALUE_1000);
        }
        return hpCost;
    }

    public int GetCrystalCost()
    {
        return crystalCost;
    }

    
    /// 显示用接口 与逻辑剥离
    public virtual int DisplayCD => cdTimeAcc;
    public virtual int DisplayFullCD => GetCurrentCD();
    public virtual bool IsDisplayCDing => isCooldown;
    
    

    public virtual int CDTimeAcc
    {
        get
        {
            if (isCooldown)
            {
                return cdTimeAcc;
            }
            else
            {
                return 0;
            }
        }
        set
        {
            if (isCooldown)
            {
                cdTimeAcc = IntMath.Max(0, value);
            }
        }
    }

    public virtual int CDLeftTime
    {
        get
        {
            if (isCooldown)
            {
                return GetCurrentCD() - cdTimeAcc;
            }
            else
            {
                return 0;
            }
        }
    }

    public void UpdateJoystick(int degree)
    {
        joystickHasMove = true;
        OnUpdateJoystick(degree);
    }

    public void ReleaseJoystick()
    {
        OnReleaseJoystick();
    }

    public void Update(int deltaTime)
    {
        useTimeAcc += deltaTime;

        if (owner != null && (isQTE || (isRunAttack && owner.hasRunAttackConfig)))
        {
            var tmpCanuse = owner.CanUseSkill(skillID);

            if (tmpCanuse != canUse)
            {
                canUse = tmpCanuse;
#if !SERVER_LOGIC

				if(button != null)
				{
					button.SetSkillBtnVisible(canUse);
				}

#endif

            }
        }

        UpdateCoolDown(deltaTime);

        if (chargeCDTimeAcc > 0)
        {
            chargeCDTimeAcc -= deltaTime;
            charge = false;
        }
        else
        {
            if (chargeCD)
            {
                chargeCD = false;
                charge = true;
            }
        }

        //修罗邪光斩蓄力技能BUG 临时屏蔽
        //if (buttonState == ButtonState.PRESS || buttonState == ButtonState.PRESS_AGAIN)
        //    pressDuration += deltaTime ;

        if (skillState.IsRunning())
        {

            if (buttonState == ButtonState.PRESS)
                pressDuration += deltaTime;

            OnUpdate(deltaTime);
            UpdateEffectMove(deltaTime); 
            _SwitchToAssignPhase();
        }

        CheckComboSklill();
    }

    protected virtual void UpdateCoolDown(int deltaTime)
    {
        if (isCooldown)
        {
            cdTimeAcc += deltaTime;
            if (cdTimeAcc >= GetCurrentCD())
            {
                FinishCoolDown();
            }
        }
    }

    public void EnterPhase(int phase)
    {
        if (!cdFlag&&phase==skillData.CDPhase)
        {
            StartCoolDown();
        }
        curPhase = phase;
        OnEnterPhase(phase);
        owner.TriggerEventNew(BeEventType.onEnterPhase, new EventParam() { m_Int = skillID, m_Int2 = phase });
    }

    /// <summary>
    /// 设置技能开始阶段
    /// </summary>
    public void SetSkillStartPhase(int phase)
    {
        _skillStartPahse = phase;
    }

     /// <summary>
    /// 跳转到技能指定的阶段
    /// </summary>
    private void _SwitchToAssignPhase()
    {
        if(_skillStartPahse<=0)return;
        for (int i = 0; i < _skillStartPahse - 1; i++)
        {
            (owner.GetStateGraph() as BeActorStateGraph).ExecuteNextPhaseSkill();
        }
        _skillStartPahse = 0;
    }

    //获取当前技能阶段
    public int GetCurrPhase()
    {
        return curPhase;
    }

    public void ResetCoolDown()
    {
        isCooldown = false;
        cdAtStartCoolDown = 0;
    }

    public void StartCoolDown()
    {
        if (mIsIgnoreCD) return;
        cdAtStartCoolDown = GetCurrentCD();

        cdFlag = true;
        cdTimeAcc = 0;
        isCooldown = true;

        owner.TriggerEventNew(BeEventType.onSkillCoolDownStart, new EventParam() { m_Int = skillID });
    }

    public virtual bool NeedCoolDown()
    {
        return pressMode != SkillPressMode.PRESS_AGAIN_CANCEL_OUT&& pressMode != SkillPressMode.PRESS_JOYSTICK;
    }

    public void FinishCoolDown()
    {
        isCooldown = false;
        cdAtStartCoolDown = 0;
        owner.TriggerEventNew(BeEventType.onSkillCoolDownFinish, new EventParam() { m_Int = skillID });
    }

    public bool IsTargetTypeEnemy()
    {
        return skillData.SkillTarget == ProtoTable.SkillTable.eSkillTarget.ENEMY;
    }

    public bool IsFroce()
    {
        return skillData.IsForce;
    }

    public virtual bool CanBePositiveAbort()
    {
        return skillData.IsCanCancel;
    }

    /// <summary>
    /// 是否能中断指定的技能ID
    /// </summary>
    /// <param name="otherSkillID">传递进来的是当前的技能ID</param>
    /// <param name="otherSkillHit"></param>
    /// <returns></returns>
    public bool CanInterrupt(int otherSkillID, bool otherSkillHit = false)
    {
        //旋风腿特殊处理，在空中不能被打断
        if (otherSkillID == 3113 && owner.GetPosition().z > 0)
            return false;

        return InterruptSkills(otherSkillID) ||
            (otherSkillHit && hitInterruptSkills.Contains(otherSkillID));
    }

    /// <summary>
    /// 是否能被指定的技能中断
    /// </summary>
    /// <param name="skillId">即将要释放的技能ID</param>
    /// <returns></returns>
    ///    //数组含义 第0个为当前技能实例，第2个元素的数组第一个元素为待打断的id，第二个元素为能否打断的返回值  （为了避免频繁拆装箱）
    object[] interParams = new object[] { null, new int[] { 0, 0 } };
    public virtual bool CanBeInterrupt(int skillId)
    {
        /*interParams[0] = this;
        var subParams = interParams[1] as int[];
        subParams[0] = skillId;
        subParams[1] = 0;
        if (owner != null)
            owner.TriggerEvent(BeEventType.onSkillCanBeInterrupt, interParams);
        return subParams[1] != 0;*/

        int ret = 0;
        if (owner != null)
        {
            var eventData = owner.TriggerEventNew(BeEventType.onSkillCanBeInterrupt, new EventParam(){m_Obj = this, m_Int = skillId, m_Int2 = ret});
            ret = eventData.m_Int2;
        }
        return ret != 0;
    }
    private bool InterruptSkills(int skillID)
    {
        if (jobInterruptSkills.Count > 0)
        {
            if (jobInterruptSkills.ContainsKey(skillID))
            {
                return jobInterruptSkills[skillID] == owner.professionID;
            }
        }
        else
        {
            return interruptSkills.Contains(skillID);
        }
        return false;
    }

    public virtual bool CanForceUseSkill()
    {
        return false;
    }

    public int GetWalkMode()
    {
        return skillData.WalkMode;
    }

    public bool CanWalk()
    {
        return skillData.WalkMode != (int)SkillWalkMode.FORBID;
    }

    public VFactor GetWalkSpeedRate()
    {
        return walkSpeed;
    }

    public int GetLevel()
    {
        return level;
    }

    public VFactor GetSkillSpeedFactor()
    {
        return new VFactor(skillSpeed,GlobalLogic.VALUE_1000);
    }

    public bool SkillNeedChagneSpeed()
    {
        return  !(isCharging && owner.skillController.skillPhase == chargeConfig.repeatPhase);           //当不处于正在蓄力状态并且当前技能阶段不是蓄力循环阶段时才会改变技能速度
    }

    public void SetSkillSpeed(int attackSpeed)
    {
        VRate n = (int)skillSpeedFactor;
        n += speedAddRate;
        skillSpeed = attackSpeed * n.factor;

        //技能减速保护，最多减40%
        if (skillSpeed < GlobalLogic.VALUE_400)
            skillSpeed = GlobalLogic.VALUE_400;
    }

    public void SetSkillSpeedWithSkillData()
    {
        VRate n = GlobalLogic.VALUE_1000;
        n += speedAddRate;
        skillSpeed = skillData.Speed * n.factor;
    }

    public SkillSpeedEffectType GetSkillSpeedEffectType()
    {
        return (SkillSpeedEffectType)(int)skillData.SpeedEffectType;
    }

    public virtual int GetCurrentCD()
    {
        //如果在cd过程中，就用开始时记录的
        if (isCooldown && cdAtStartCoolDown > 0)
        {
            return cdAtStartCoolDown;
        }

        VRate tmpCDReduceRate = cdReduceRate;
        if (owner != null)
        {
            tmpCDReduceRate = tmpCDReduceRate + owner.GetEntityData().GetCDReduce(skillMagicType);
        }
        int cd = tableCD * (VRate.one - tmpCDReduceRate).factor;
        cd -= cdReduceValue;

        int minCD = BattleMain.IsModePvP(battleType) ? TableManager.GetValueFromUnionCell(skillData.MinCDPVP, level) : TableManager.GetValueFromUnionCell(skillData.MinCD, level);
        if (cd < minCD)
            cd = minCD;

        return cd;
    }

    public int GetCurrentCharge()
    {
        int original = IntMath.Float2IntWithFixed(chargeConfig.chargeDuration, GlobalLogic.VALUE_1000);
        return original * (VRate.one - chargeReduceRate).factor;
    }

    //operation
    public Operation ParseOperation()
    {
        Operation operation = new Operation();
        if (skillData != null)
        {
            operation.op = (Operation.OP)skillData.SkillOperation;
            operation.target = (Operation.TARGET)skillData.SkillOpTarget;
            operation.varName = skillData.SkillOpVar;
            operation.value = TableManager.GetValueFromUnionCell(skillData.SkillOpValue, level);

            if (operation.target == Operation.TARGET.SKILL)
            {
                int index = 0;
                operation.skillIDs = new int[skillData.SkillOpSkillIDs.Count];
                for (int i = 0; i < skillData.SkillOpSkillIDs.Count; ++i)
                {
                    int id = skillData.SkillOpSkillIDs[i];
                    if (id > 0)
                        operation.skillIDs[index++] = id;
                }
            }
        }
        return operation;
    }

    //!! 这里要检查下
    public void ExecuteOperation(Operation operation)
    {
        if (operation.op == Operation.OP.NONE)
            return;

        Type type = typeof(BeSkill);
        object obj = this;
        if (operation.target == Operation.TARGET.ACTOR)
        {
            type = typeof(BeActor);
            obj = owner;
        }

        if (operation.target == Operation.TARGET.SKILL)
        {
            for (int i = 0; i < operation.skillIDs.Length; ++i)
            {
                var skill = owner.GetSkill(operation.skillIDs[i]);
                if (skill != null)
                {
                    obj = skill;
                    Logger.LogWarningFormat("[SKILL_OPERATION]set {0} {1} {2}", operation.op, operation.varName, operation.value);
                    if (operation.varName == "level")
                        Utility.SetValueForProperty(type, obj, operation.varName, operation.value, operation.op == Operation.OP.ADD);
                    else
                        Utility.SetValue(type, obj, operation.varName, operation.value, operation.op == Operation.OP.ADD);
                }
            }
        }
        else
        {
            Utility.SetValue(type, obj, operation.varName, operation.value, operation.op == Operation.OP.ADD);
        }
    }

    public int GetPvpAttackScale()
    {
        return skillData.AttackScalePVP;
    }

    //utility,valueA - valueF
    public static List<int> GetEffectSkills(IList<ProtoTable.UnionCell> value, int skillLevel)
    {
        List<int> list = new List<int>();
        for (int i = 0; i < value.Count; ++i)
        {
            int sid = TableManager.GetValueFromUnionCell(value[i], skillLevel);
            list.Add(sid);
        }

        return list;
    }



    public void AddBuff(BeActor target, int effectID)
    {
        //Logger.LogErrorFormat("try add buff with effect ID:{0} to {1}", effectID, target.GetName());

        var data = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(effectID);
        if (data != null)
        {
            int buffLevel = level;

            int buffDuration = 0;
            if (BattleMain.IsChijiNeedReplaceHurtId(effectID, battleType))
            {
                var chijiEffectMapTable = TableManager.instance.GetTableItem<ChijiEffectMapTable>(effectID);
                buffDuration = TableManager.GetValueFromUnionCell(chijiEffectMapTable.AttachBuffTime, buffLevel);
            }
            else
            {
                buffDuration = TableManager.GetValueFromUnionCell(data.AttachBuffTime, buffLevel);
            }
            target.buffController.TryAddBuff(data.BuffID, buffDuration, buffLevel);
        }
    }

    public BuffInfoData GetBuffInfo(int buffID)
    {
        if (buffEnhanceList.ContainsKey(buffID))
            return buffEnhanceList[buffID];

        return null;
    }

    protected bool CheckResetCamera()
    {
        if (skillData == null)
            return false;
        if (!skillData.IsResetCamera)
            return false;
        return true;
    }

    #region weapon and can use skill
    public void ChangeWeapon()
    {
        // TODO:: 人物更换武器调用这里，改变技能是否能释放的状态
        UpdateCanUseSkillWithRightWeapon();
    }

    public void UpdateCanUseSkillWithRightWeapon()
    {
        if (owner == null || skillData == null)
            return;

        int typ = skillData.NeedWeaponType;
        if (typ == 0)
            return;
         
       canUseWithRightWeapon = typ == owner.GetWeaponType();
    }

    public void DealLevelChange()
    {
        AddBuffs();

        //技能等级改变时重新添加技能表中的机制
        RemoveMechanisms();
        AddMechanisms();

        if (skillData.SkillType == ProtoTable.SkillTable.eSkillType.PASSIVE)
        {
            for (int i = 0; i < preConditions.Count; ++i)
            {
                var condition = preConditions[i];
                if (condition == ProtoTable.SkillTable.ePreCondition.INIT)
                {
                    DealEffectFrame();
                }
            }
        }
        ChangeSummonData();
    }

    public void DealWeaponChange()
    {
        UpdateCanUseSkillWithRightWeapon();
        //AddBuffs();

        OnWeaponChange();
        SetComboSourceSkillID();
    }

    public void DealEquipChange()
    {
        OnWeaponChange();
    }

#endregion

    /// <summary>
    /// 该变按钮特效
    /// </summary>
    protected void ChangeButtonEffect()
    {
#if !LOGIC_SERVER
        if (button != null)
        {
            button.AddEffect(ETCButton.eEffectType.onContinue);
        }
#endif
    }

    /// <summary>
    /// 重置按钮特效
    /// </summary>
    protected void ResetButtonEffect()
    {
        skillButtonState = SkillState.NORMAL;
#if !LOGIC_SERVER
        if (button != null)
        {
            button.RemoveEffect(ETCButton.eEffectType.onContinue);
        }
#endif
    }

    /// <summary>
    /// 改变技能召唤数据
    /// </summary>
    protected void ChangeSummonData()
    {
        if (skillData == null || skillData.SummonRestrainEffectID <= 0)
            return;
        var hurtData = TableManager.GetInstance().GetTableItem<EffectTable>(skillData.SummonRestrainEffectID);
        if (hurtData == null)
            return;
        summonEffectData.summonId = hurtData.SummonID;
        summonEffectData.summonNum = TableManager.GetValueFromUnionCell(hurtData.SummonNum, GetLevel());
        summonEffectData.singleNumLimit = hurtData.SummonNumLimit;
        summonEffectData.groupNumLimit = TableManager.GetValueFromUnionCell(hurtData.SummonGroupNumLimit, GetLevel());
        summonEffectData.summonGroup = hurtData.SummonGroup;
    }

    //重置Z轴额外的加速度
    protected void ResetMoveZAccExtra()
    {
        owner.SetMoveSpeedZAccExtra(0);
    }

    /// <summary>
    /// 检测自己或者自己的召唤者其中至少有一个处于异常状态
    /// </summary>
    protected bool CheckSelfAndOwnerInPass()
    {
        if (owner.IsPassiveState())
            return true;
        var topOwner = owner.GetTopOwner(owner);
        if (topOwner == null)
            return false;
        if (topOwner.IsInPassiveState())
            return true;
        return false;
    }
    
    public void RemoveAllHandles()
    {
        if (handleA != null)
        {
            handleA.Remove();
            handleA = null;
        }

        if (handleB != null)
        {
            handleB.Remove();
            handleB = null;
        }

        if (handleC != null)
        {
            handleC.Remove();
            handleC = null;
        }
    }

    private bool CheckComboSklill()
    {
        if (!this.isComboSkill)
        {
            return false;
        }

        if (buttonState != ButtonState.PRESS &&
            !(skillData.IsAttackCombo > 0 && owner.GetCurrentBtnState() == ButtonState.PRESS))
        {
            return false;
        }

        return owner.TriggerComboSkills(this.comboSkillSourceID);
    }

    public void SetLightButtonVisible(bool active)
    {
        if (active)
        {
            skillButtonState = SkillState.WAIT_FOR_NEXT_PRESS;
            ChangeButtonEffect();    
        }
        else
        {
            ResetButtonEffect();
        }
    }

    
    //这个时候owner是空
    public virtual void OnInit() { }
    public virtual void OnDeInit() { }
    //这个时候owner不为空
    public virtual void OnPostInit() { }
    public virtual void OnStart() { }
    public virtual void OnUpdate(int iDeltime) { }
    public virtual void OnUpdateJoystick(int degree) { }
    public virtual void OnReleaseJoystick() { owner.TriggerEventNew(BeEventType.onReleaseJoystick, new EventParam() {m_Obj = this}); }
    public virtual void OnFinish() { }
    public virtual void OnComboBreak() { }
    public virtual void OnCancel() { }
    public virtual void OnEnterPhase(int phase) { }

    public virtual void OnClickAgain() { owner.TriggerEventNew(BeEventType.onClickAgain, new EventParam() {m_Obj = this}); }
    public virtual void OnClickAgainCancel() { }

    public virtual void OnWeaponChange() {}
    /// <summary>
    /// 主动切换到下一阶段
    /// </summary>
    /// <param name="phase"></param>
    public virtual void OnEnterNextPhase(int phase) {}

    public virtual bool NeedClearSpeed() { return true; }
}


