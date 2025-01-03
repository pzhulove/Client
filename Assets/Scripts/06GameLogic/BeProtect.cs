using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using ProtoTable;
using System;

public class BeProtect {
	protected BeActor actor;

	protected int fallHurtCount = 0;
	protected int groundHurtCount = 0;
	protected bool fallHurtCounting = false;
	protected bool groundHurtCounting = false;
	protected int fallHurtEffect = 0;
	protected int standHurtCount = 0;
	protected bool standHurtCounting = false;

	protected VRate floatHurtPercent;
	protected VRate floatHurtLevelPercent;
	protected VRate groundHurtPercent;
	protected VRate standHurtPercent;

	protected bool checkClearProtect;
	protected int timeAcc = 0;


	protected bool enable = false;

    // 倒地起身的保护(固定1200ms)
    protected int fallClrTimeAfterGetUp = 1200;
    protected int accTimeAfterGetUp = 0;
    protected int fallZAccFold = 0;

    // 倒地保护机制修改: 伤害达到门限时并不立马触发保护, 也不无敌,要连理论是可以连的.
    // 尝试在倒地状态时, 立马起身(有时间).新增状态尝试起身的状态.
    protected bool isTringToGetUp = false;
    protected int timeAccTringToGetUpMax = 167;
    protected int timeAccTringToGetUp = 0;

    protected bool abnormalBuffProtectSwitch = false; //异常Buff保护开关
    protected bool hardProtectSwitch = false; //僵直保护开关
    protected bool missProtectSwitch = false;//Miss保护

	public BeProtect(BeActor owner)
	{
		actor = owner;
		floatHurtPercent = (VRate)Global.Settings.defaultFloatHurt;
		floatHurtLevelPercent = (VRate)Global.Settings.defaultFloatLevelHurat;
		groundHurtPercent = (VRate)Global.Settings.defaultGroundHurt;
		standHurtPercent = (VRate)Global.Settings.defaultStandHurt;

        InitSwitchData();
        InitRegistEvent();
    }

	public void SetValue(VRate floatHurtPercent, VRate floatHurtLevelPercent, VRate groundHurtPercent, VRate standHurtPercent)
	{
        if (!enable)
            return;
		this.floatHurtPercent = (VRate)floatHurtPercent;
		this.floatHurtLevelPercent = (VRate)floatHurtLevelPercent;
		this.groundHurtPercent = (VRate)groundHurtPercent;
		this.standHurtPercent = (VRate)standHurtPercent;
	}

	public void SetEnable(bool flag)
	{
        enable = flag;
        if (enable && BattleMain.IsProtectFloat(actor.battleType) && actor.GetMechanism(6240) == null)
        {
            // PVP 增加蓝条伤害减免功能 方便配置与开关
            actor.AddMechanism(6240);  
        }
	}

	public bool IsEnable()
	{
		return enable;
	}

    public bool FallHurtCounting
    {
        get { return fallHurtCounting; }
    }

    public int FallHurtEffect
    {
        get { return fallHurtEffect; }
    }

    public void Tick(int deltaTime)
	{
		if (!enable)
			return;
        UpdateSleepBuffProtectTime(deltaTime);
		UpdateCheckClearProtect(deltaTime);
        UpdateDelayClearFallProtect(deltaTime);
        UpdateTryToGetUp(deltaTime);
        UpdateHardTime(deltaTime);
        UpdateMissProtect(deltaTime);
    }


	public void Update(int hurtValue)
	{
		if (!enable)
			return;

        int tempValueFALLGROUND = 0;
        int tempValueFALL = 0;
        int tempValueSTAND = 0;

        bool stand = true;
		if (actor.HasTag((int)AState.AST_FALLGROUND))
		{
            // 起立状态不累计
            if (!isTringToGetUp)
            {
                groundHurtCount += hurtValue;
                tempValueFALLGROUND = hurtValue;
            }
			    
			stand = false;
		}

		if (actor.HasTag((int)AState.ACS_FALL))
		{
			fallHurtCount += hurtValue;
            tempValueFALL = hurtValue;
            stand = false;
		}

        if (stand)
        {
            standHurtCount += hurtValue;
            tempValueSTAND = hurtValue;
        }
		else
			ClearStandProtect();
			

		UpdateHurtProtect(tempValueFALLGROUND, tempValueFALL, tempValueSTAND);
		StartCheckClearProtect();
	}
		
	public void StartFallHurtCount()
	{
		if (!enable || fallHurtCounting)
			return;
		
		fallHurtCount = 0;
		fallHurtCounting = true;
		fallHurtEffect = 0;

        accTimeAfterGetUp = 0;
        fallZAccFold = 0;

        actor.m_pkGeActor.ShowProtectFloat(true, floatHurtPercent.scalar);
	}

	public void StartGroundHurtCount()
	{
		if (!enable || groundHurtCounting)
			return;

        // 尝试起身状态开始显示新的标志
        if (isTringToGetUp)
            return;

		groundHurtCount = 0;
		groundHurtCounting = true;
        timeAccTringToGetUp = 0;

        actor.m_pkGeActor.ShowProtectGround(true, groundHurtPercent.scalar);
	}

	public void StartStandHurtCount()
	{
		if (!enable || standHurtCounting)
			return;

		standHurtCount = 0;
		standHurtCounting = true;

		actor.m_pkGeActor.ShowProtectStand(true, standHurtPercent.scalar);
	}
		
	public void ClearProtect()
	{
		if (!enable)
			return;

        //ClearFallProtect();
        ClearGroundProtectCounting();
        ClearStandProtect();

		Logger.LogProcessFormat("{0} execute ClearProtect!!!!!",  actor.GetName());
	}

    // 状态清理
    public void ClearUp()
    {
        if (!enable)
            return;

        ClearGroundProtect();
        ClearFallProtect();
        ClearStandProtect();
    }

    #region clear fall protect with delay after getup
    public void ClearFallProtect()
	{
		if (!enable)
			return;
		
		if (fallHurtCounting)
		{
			fallHurtCounting = false;
			fallHurtEffect = 0;
			actor.ResetWeight();
		}
        accTimeAfterGetUp = 0;
        fallZAccFold = 0;
        actor.m_pkGeActor.ShowProtectFloat(false);

	}

    public void DelayClearFallProtect()
    {
        if (!enable)
            return;

        if (!fallHurtCounting)
            return;

        accTimeAfterGetUp = fallClrTimeAfterGetUp;

        // 部分状态回复
        //fallHurtEffect = 0;
        //actor.ResetWeight();
    }

    private void UpdateDelayClearFallProtect(int dt)
    {
        if (!enable)
            return;
        if (accTimeAfterGetUp < 1)
            return;

        // 玩家高度大于0时停止计时
		if (actor.GetPosition().z > 0 && !actor.isFloating)
        {
            accTimeAfterGetUp = fallClrTimeAfterGetUp;
            return;
        }

        accTimeAfterGetUp -= dt;

        if (accTimeAfterGetUp > 0)
            return;

        accTimeAfterGetUp = 0;
        ClearFallProtect();
    }
    #endregion

    public void ClearGroundProtect()
	{
		if (!enable)
			return;
		
		if (groundHurtCounting)
		{
			groundHurtCounting = false;
		}
        isTringToGetUp = false;
        timeAccTringToGetUp = 0;

        actor.m_pkGeActor.ShowProtectGround(false);
	}

    private void ClearGroundProtectCounting()
    {
        if (!enable)
            return;

        if (groundHurtCounting)
        {
            groundHurtCounting = false;
        }
    }

    private void UpdateTryToGetUp(int dt)
    {
        if (!enable)
            return;
        if (!isTringToGetUp)
            return;
        //圣骑士假复活状态下不刷新平推保护
        if (actor.buffController.HasBuffByID(101) != null)
            return;

        if (!actor.HasTag((int)AState.AST_FALLGROUND) || actor.IsGrabed())
        {
            timeAccTringToGetUp = 0;
            return;
        }

        int z = actor.GetPosition().z;
        var groundThreshhold = VInt.Float2VIntValue(0.0001f);
        if (z <= groundThreshhold)
        {
            timeAccTringToGetUp += dt;
         
            if (timeAccTringToGetUp < timeAccTringToGetUpMax)
                return;
            if (actor.CurrentBeBattle != null && actor.CurrentBeBattle.GetBattleType() == BattleType.PVP3V3Battle && actor.attribute != null && actor.attribute.GetHP() <= 0)
            {
                //if (actor.GetRecordServer() != null && actor.IsProcessRecord())
                //{
                //    actor.GetRecordServer().RecordProcess("[TryToGetUp] z:{0} groundThreshhold {1} timeAccTringToGetUp {2} timeAccTringToGetUpMax {3}", z, groundThreshhold, timeAccTringToGetUp, timeAccTringToGetUpMax);
                //}
            }
            ClearGroundProtect();

            //if (z > 0)
            //{
            //    var pos = actor.GetPosition();
            //    pos.z = 0;
            //    actor.SetMoveSpeedZ(0);
            //    actor.SetStandPosition(pos, true);
            //}

            actor.sgClearStateStack();
            actor.sgPushState(new BeStateData((int)ActionState.AS_GETUP));
            actor.sgLocomoteState();

            actor.buffController.RemoveBuff(7);//站立保护清除睡眠异常Buff
			actor.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE, GlobalLogic.VALUE_1000);
        }
    }

    public void ClearStandProtect()
	{
		if (!enable)
			return;
		
		if (standHurtCounting)
		{
			standHurtCounting = false;
		}
        if (actor.m_pkGeActor != null)
        {
            actor.m_pkGeActor.ShowProtectStand(false);
        }
        else
        {
            Logger.LogErrorFormat("ClearStandProtect jobID:{0}",actor.professionID);
        }
	}

	void FallHurtTakeEffect()
    {
        if (!enable)
            return;
		var data = actor.GetEntityData();

		// 最高固定9倍
		if (fallZAccFold > (2 + 7))
			return;
		
        fallZAccFold += 1;
		actor.moveZAcc +=  (int)data.weight * 5;
		fallHurtEffect++;
		fallHurtCount -= (data.battleData.maxHp * floatHurtLevelPercent.factor);

		//if (fallHurtEffect == 1)
		//	actor.m_pkGeActor.ShowProtectFloat(false);
		//Logger.LogErrorFormat("trigger 浮空保护:{0}", actor.GetName());
	}

	void UpdateHurtProtect(int ground=0, int fall=0, int stand=0)
    {
        if (!enable)
            return;
        if (actor == null)
        {
            return;
        }
        var data = actor.GetEntityData();
        if (data == null)
        {
            return;
        }

		if (fallHurtCounting)
		{
			if (fallHurtEffect >= 1)
			{
				if (fallHurtCount >  (data.battleData.maxHp * floatHurtLevelPercent.factor))
				{
					FallHurtTakeEffect();
				}
			}
			else if (fallHurtCount > (data.battleData.maxHp * floatHurtPercent.factor))
			{
				//触发浮空保护
				FallHurtTakeEffect();
			}

            // 处在起身的计时阶段,停止计时，因为重新浮空了.
            //if (accTimeAfterGetUp > 0)
            //{
            //    accTimeAfterGetUp = 0;
            //}

            // 清理的情况(受伤害才会进来
            // 抓取逻辑, 20*8 1.正常重力 18 2.执行抓取(回复之前重力20*8) <TODO::1和2之间如果受到伤害会触发20*8>)
            if (fallZAccFold > 0)
            {
                VFactor foldFromProtect = new VFactor((int)actor.moveZAcc,(int)data.weight);
				if (foldFromProtect < (fallZAccFold+1)) // 清理的情况
					actor.moveZAcc = data.weight.i * (fallZAccFold+1);
            }

            // 更新进度tag(因为有些buff技能有伤害而不触发保护)
            if (fall > 0 && fallHurtEffect < 1)
            {
				//!! 表现用 直接转float
                // 需要 (fallHurtCount- fall)，因为 先调用了 UpdateHurtProtect 后调用了 updateHPBar()
                float perRest = floatHurtPercent.scalar - ((float)(fallHurtCount- fall) / (float)data.battleData.maxHp);
                if (perRest >= 0.0f && actor.m_pkGeActor != null)
                {
                    actor.m_pkGeActor.ShowProtectFloat(true, perRest);
                } 
            }
        }
        
        if (groundHurtCounting)
		{
			if (groundHurtCount >  data.battleData.maxHp * groundHurtPercent.factor && !actor.IsGrabed())
			{
				//如果这个时候在空中，会立马落下并起身
				//if (actor.GetPosition().z > 0)
				//{
				//	var pos = actor.GetPosition();
				//	pos.z = 0;
				//	actor.SetMoveSpeedZ(0);
				//	actor.SetStandPosition(pos, true);
				//}

				//actor.sgClearStateStack();
				//actor.sgPushState(new BeStateData((int)ActionState.AS_GETUP));
				//actor.sgLocomoteState();
				groundHurtCounting = false;
                isTringToGetUp = true;

                // 1000ms用于下落
				//actor.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE, 1000);
				//actor.moveZAcc *= 10000;

				//actor.m_pkGeActor.ShowProtectGround(false);

				//Logger.LogErrorFormat("trigger 倒地保护:{0}", actor.GetName());

			}

            // 更新进度tag(因为有些buff技能有伤害而不触发保护)
            if (ground > 0)
            {
				//!! 表现用 直接转float
                float perRest = groundHurtPercent.scalar - ((float)(groundHurtCount - ground) / (float)data.battleData.maxHp);
                if (perRest >= 0.0f && actor.m_pkGeActor != null)
                {
                    actor.m_pkGeActor.ShowProtectGround(true, perRest);
                }
            }
        }

        //圣骑士假复活状态下不触发站立保护

        if (standHurtCounting && actor.buffController.HasBuffByID(101) == null)
		{
			if (standHurtCount > data.GetMaxHP() * standHurtPercent.factor && !actor.IsGrabed())
			{
				actor.ClearMoveSpeed();
				actor.sgClearStateStack();
				actor.sgPushState(new BeStateData((int)ActionState.AS_FALLGROUND));
                //切倒地状态之前，先移除身上的强控buff
                for (int i = 11; i <= 14; i++)
                {
                    var buff = actor.buffController.HasBuffByType((BuffType)i);
                    if (buff != null)
                        actor.buffController.RemoveBuff(buff);
                }
                actor.sgLocomoteState();

				actor.buffController.TryAddBuff((int)GlobalBuff.GETUP_BATI, GlobalLogic.VALUE_1000);

				standHurtCounting = false;

				//actor.m_pkGeActor.ShowProtectStand(false);

				//Logger.LogErrorFormat("trigger 站立保护:{0}", actor.GetName());
			}

            // 更新进度tag(因为有些buff技能有伤害而不触发保护)
            if (stand > 0)
            {
				//!! 表现用 直接转float
                float perRest = standHurtPercent.scalar - ((float)(standHurtCount - stand) / (float)data.battleData.maxHp);
                if (perRest >= 0.0f && actor.m_pkGeActor != null)
                    actor.m_pkGeActor.ShowProtectStand(true, perRest);
            }
        }
	}

    //重置浮空保护时间
    public void ResetFallTime()
    {
        accTimeAfterGetUp = 0;
    }

    public bool IsAfterGetUpFallCounting()
    {
        return accTimeAfterGetUp > 0;
    }

	private void StartCheckClearProtect()
    {
        if (!enable)
            return;
		checkClearProtect = true;
		timeAcc = 0;

		Logger.LogProcessFormat("{0} StartCheckClearProtect", actor.GetName());
	}

	private void UpdateCheckClearProtect(int delta)
    {
        if (!enable)
            return;
		if (!checkClearProtect)
			return;

		timeAcc += delta;
		if (timeAcc > Global.Settings.protectClearDuration)
		{
			ClearProtect();
			StopCheckClearProtect();
		}
	}

	private void StopCheckClearProtect()
    {
        if (!enable)
            return;
		if (!checkClearProtect)
			return;

		Logger.LogProcessFormat("{0} StopCheckClearProtect", actor.GetName());
		checkClearProtect = false;
		timeAcc = 0;
	}
    
    private void InitSwitchData()
    {
        abnormalBuffProtectSwitch = BeClientSwitch.FunctionIsOpen(ClientSwitchType.PkAbnormalBuffProtect);
        hardProtectSwitch = BeClientSwitch.FunctionIsOpen(ClientSwitchType.HardProtect);
        if (actor == null || actor.CurrentBeBattle == null || actor.CurrentBeBattle.PkRaceType != (int)Protocol.RaceType.ChiJi) return;
        var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>(655);
        if (SystemValueTableData != null)
        {
            floatHurtPercent = new VRate(SystemValueTableData.Value);
        }
        SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>(656);
        if (SystemValueTableData != null)
        {
            floatHurtLevelPercent = new VRate(SystemValueTableData.Value);
        }
        SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>(657);
        if (SystemValueTableData != null)
        {
            groundHurtPercent = new VRate(SystemValueTableData.Value);
        }
        SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>(658);
        if (SystemValueTableData != null)
        {
            standHurtPercent = new VRate(SystemValueTableData.Value);
        }
        //  missProtectSwitch = BeClientSwitch.FunctionIsOpen(ClientSwitchType.PVPMISS);
    }

    /// <summary>
    /// 是否处于倒地保护状态
    /// </summary>
    /// <returns></returns>
    public bool IsInGroundProtect()
    {
        if (isTringToGetUp)
            return true;
        return false;
    }

    /// <summary>
    /// 被击
    /// </summary>
    public void BeHit()
    {
        RegistPVPMissBeHit();
        RegisterHardProtectBeHit();
        RegisterAbnormalProtectBeHit();
    }

    #region 被异常Buff连续控制保护
    protected int[] buffIdArr = new int[] {71,72,73,74};                    //眩晕 冰冻 石化 睡眠抗性BuffId
    protected List<BeBuff> protectBuffList = new List<BeBuff>();            //用于存储添加的Buff
    protected int protectBuffTime = 2500;                                   //保护Buff时间
    private int sleepRefreshProtectTime = 1000;
    //添加Buff
    public void AddBuff(BuffType type)
    {
        if (!CheckAbnormalProtectOpen())
            return;
        if (!IsControlBuff(type))
            return;
        AddProtectBuffSingle(type);
        if (CheckProtectBuff())
            RefreshProtectBuffTime();
    }

    //受到攻击，不是异常Buff攻击
    public void RegisterAbnormalProtectBeHit()
    {
        if (!CheckAbnormalProtectOpen())
            return;
        if (CheckProtectBuff())
            RefreshProtectBuffTime();
    }
    private int acc = 0;
    private void UpdateSleepBuffProtectTime(int deltaTime)
    {
        acc += deltaTime;
        if (acc >= sleepRefreshProtectTime)
        {
            acc = 0;
            if(actor.stateController.HasBuffState(BeBuffStateType.SLEEP))
                RefreshSleepProtectBuffTime();
        }
    }
    private void RefreshSleepProtectBuffTime()
    {
        if (!CheckAbnormalProtectOpen())
            return;
        if (actor.buffController.HasBuffByID(buffIdArr[3])!=null)
            RefreshSleepProtectBuff();
    }
    private void RefreshSleepProtectBuff()
    {
        var iter = actor.buffController.GetBuffList().GetEnumerator();
        while (iter.MoveNext())
        {
            var curBuff = iter.Current;
            if (curBuff != null && !curBuff.CanRemove() && curBuff.buffID == buffIdArr[3])
            {
                curBuff.RefreshDuration(curBuff.duration);
            }
        }
    }

    //添加单个保护Buff
    protected void AddProtectBuffSingle(BuffType type)
    {
        int buffId = 0;
        switch (type)
        {
            case BuffType.STUN:
                buffId = buffIdArr[0];
                break;
            case BuffType.FROZEN:
                buffId = buffIdArr[1];
                break;
            case BuffType.STONE:
                buffId = buffIdArr[2];
                break;
            case BuffType.SLEEP:
                buffId = buffIdArr[3];
                break;
        }
        BeBuff buff = actor.buffController.TryAddBuff(buffId,protectBuffTime);
        if (buff != null)
            protectBuffList.Add(buff);
    }

    //检查身上是否有保护Buff
    protected bool CheckProtectBuff()
    {
        bool flag = false;
        for(int i=0;i< buffIdArr.Length; i++)
        {
            BeBuff buff = actor.buffController.HasBuffByID(buffIdArr[i]);
            if (buff != null)
            {
                flag = true;
                break;
            }
                
        }
        return flag;
    }

    //是否控制Buff
    protected bool IsControlBuff(BuffType type)
    {
        if (type == BuffType.FROZEN || type == BuffType.STONE || type == BuffType.STUN || type == BuffType.SLEEP)
            return true;
        return false;
    }

    //刷新保护Buff时间
    protected void RefreshProtectBuffTime()
    {
        if (protectBuffList == null)
            return;
        for(int i=0;i< protectBuffList.Count; i++)
        {
            BeBuff buff = protectBuffList[i];
            if (buff != null)
            {
                buff.RefreshDuration(buff.duration);
            }
        }
    }

    /// <summary>
    /// 判断是否需要进入异常Buff保护
    /// </summary>
    /// <returns></returns>
    private bool CheckAbnormalProtectOpen()
    {
        if (!abnormalBuffProtectSwitch)
            return false;
        if (!enable)
            return false;
        if (actor.IsMonster())
            return false;
        return true;
    }
    #endregion
    #region PVP回避保护
    private bool startRecordTime = false;
    private int judgeTime = 3000;
    private int missProtectTime = 10000;
    private int curMissRate = 0;
    private int hitNumsLoop = 3;
    private bool hasTriggerMiss = false;
    private int[] missProtectTimeArr = new int[12] { 10,11,12,13,14,15,16,17,18,19,20,21 };
    private int[] missProtectRateArr = new int[12] {10,20,30,40,50,60,70,80,100,120,200,500 };
    private void RegistPVPMissBeHit()
    {      
        startRecordTime = true;
        accJudgeTimer = 0;
    }
    private int hitNums = 0;
    private void InitRegistEvent()
    {
        actor.RegisterEventNew(BeEventType.OnAttackResult, (args) =>
        {
            hitNums %= hitNumsLoop;
            if (hitNums == 0)
            {
                hasTriggerMiss = false;
            }
            hitNums++;
            // int[] results = args[0] as int[];
            // AttackResult result = (AttackResult)results[0];
            AttackResult result = (AttackResult)args.m_Int;
            if (result != AttackResult.MISS)
            {
                if (hitNums <= hitNumsLoop && hasTriggerMiss)
                {
                    return;
                }
                int random = actor.FrameRandom.InRange(1, 1000);
                int curRate = (int)(curMissRate / 3) * hitNums;
                if (random <= curRate)
                {
                    hasTriggerMiss = true;
                    // results[0] = 0;
                    args.m_Int = 0;
                }
            }
            else
            {
                hasTriggerMiss = true;
            }
        });
    }
    private int accJudgeTimer = 0;
    private int accPtotectTime = 0;
    private void UpdateMissProtect(int deltaTime)
    {
        if (!BattleMain.IsModePvP(actor.battleType)) return;
        if (startRecordTime)
        {
            //如果蹲伏回避保护不生效
            //if (actor.isInDunfu)
            //{
            //    ResetMissProtect();
            //    return;
            //}

            //如果处于被抓取状态则不进行倒计时
            if (!actor.IsGrabed())
            {
                accJudgeTimer += deltaTime;

                if (accJudgeTimer >= judgeTime)
                {
                    ResetMissProtect();
                    return;
                }
            }
            accPtotectTime += deltaTime;
            if (accPtotectTime >= missProtectTime)
            {
                TriggerMissProtect(accPtotectTime);
            }
        }
    }
    private void ResetMissProtect()
    {
        startRecordTime = false;
        accJudgeTimer = 0;
        curMissRate = 0;
        accPtotectTime = 0;
    }
    private void TriggerMissProtect(int totalTime)
    {
        int index = Array.IndexOf(missProtectTimeArr, (int)(totalTime / 1000));
        if (index < 0)
            index = missProtectRateArr.Length - 1;
        curMissRate = missProtectRateArr[index];
    }

    #endregion

    #region 决斗场僵直保护系统

    /// <summary>
    /// 僵直保护累积次数
    /// </summary>
    private int[] hardProtectCountArr = new int[4] { 8, 15, 35, 60 };
    /// <summary>
    /// 僵直保护对应的Buff
    /// </summary>
    private int[] hardProtectBuffArr = new int[4] { 79, 80, 81, 82 };
    /// <summary>
    /// 当前的次数
    /// </summary>
    private int curHardProtectCount = 0;
    /// <summary>
    /// 倒计时时间
    /// </summary>
    private int hardProtectTime = 3000;
    /// <summary>
    /// 当前倒计时时间
    /// </summary>
    private int curHardProtectTime = 0;
    /// <summary>
    /// 更新标志
    /// </summary>
    private bool hardProtectFlag = false;
    /// <summary>
    /// 僵直保护Buff时间
    /// </summary>
    private int hardProtectBuffTime = 3000;
    /// <summary>
    /// 达到该硬直时出现霸体一样的效果 能造成伤害 但是不会进入被击状态
    /// </summary>
    private int hardProtectMax = 4500;
    /// <summary>
    /// 硬直保护Buff列表
    /// </summary>
    private List<BeBuff> hardBuffList = new List<BeBuff>();

    /// <summary>
    /// 僵直保护系统 监听到被击
    /// </summary>
    private void RegisterHardProtectBeHit()
    {
        if (!CheckHardProtectOpen())
            return;
        if (!hardProtectFlag)
            StartHardTime();
        else
            ResetHardTime();
        RefreshHardProtectBuff();
    }

    /// <summary>
    /// 开始僵直保护倒计时
    /// </summary>
    private void StartHardTime()
    {
        hardProtectFlag = true;
    }

    /// <summary>
    /// 更新倒计时
    /// </summary>
    /// <param name="deltaTime"></param>
    private void UpdateHardTime(int deltaTime)
    {
        if (!CheckHardProtectOpen())
            return;
        if (!hardProtectFlag)
            return;
        curHardProtectTime += deltaTime;
        if(curHardProtectTime >= hardProtectTime)
        {
            hardProtectFlag = false;
            curHardProtectTime = 0;
            ResetHardTimeAndCount();
        }
    }

    /// <summary>
    /// 重置倒计时时间
    /// </summary>
    private void ResetHardTime()
    {
        curHardProtectTime = 0;
    }

    /// <summary>
    /// 重置时间和次数
    /// </summary>
    private void ResetHardTimeAndCount()
    {
        curHardProtectTime = 0;
        curHardProtectCount = 0;
    }

    /// <summary>
    /// 刷新次数 同时刷新僵直保护Buff
    /// </summary>
    private void RefreshHardProtectBuff()
    {
        curHardProtectCount += 1;

        if (curHardProtectCount >= hardProtectCountArr[3])
        {
            AddOrRefreshHardProtectBuff(3);
        }
        else if (curHardProtectCount >= hardProtectCountArr[2])
        {
            AddOrRefreshHardProtectBuff(2);
        }
        else if (curHardProtectCount >= hardProtectCountArr[1])
        {
            AddOrRefreshHardProtectBuff(1);
        }
        else if (curHardProtectCount >= hardProtectCountArr[0])
        {
            AddOrRefreshHardProtectBuff(0);
        }
    }

    /// <summary>
    /// 添加或者刷新僵直保护Buff
    /// </summary>
    private void AddOrRefreshHardProtectBuff(int index)
    {
        if (!CheckHardProtectOpen())
            return;
        int buffId = hardProtectBuffArr[index];
        BeBuff buff = actor.buffController.HasBuffByID(buffId);
        if (buff == null)
        {
            BeBuff newBuff = actor.buffController.TryAddBuff(buffId, hardProtectBuffTime);
            hardBuffList.Add(newBuff);
        }
        else
        {
            RefreshAllHardProtectBuff();
        }
    }

    /// <summary>
    /// 刷新所有的硬直Buff时间
    /// </summary>
    private void RefreshAllHardProtectBuff()
    {
        if (hardBuffList == null)
            return;
        for(int i=0;i< hardBuffList.Count; i++)
        {
            BeBuff buff = hardBuffList[i];
            if (buff != null)
            {
                buff.RefreshDuration(buff.duration);
            } 
        }

        hardBuffList.RemoveAll(buff => 
        {
            if (buff == null)
                return true;
            return false;
        });
    }

    /// <summary>
    /// 是否进入僵直保护
    /// </summary>
    /// <returns>返回True表示进入保护</returns>
    public bool IsEnterHardProtect()
    {
        if (actor.HasTag((int)AState.ACS_FALL))
            return false;
        int hardValue = actor.GetEntityData().battleData.hard;
        return hardValue >= hardProtectMax;
    }

    /// <summary>
    /// 判断是否需要进入僵直保护
    /// </summary>
    /// <returns></returns>
    private bool CheckHardProtectOpen()
    {
        if (!hardProtectSwitch)
            return false;
        if (!enable)
            return false;
        if (!BattleMain.IsModePvP(actor.battleType))
            return false;
        return true;
    }
    #endregion
}
