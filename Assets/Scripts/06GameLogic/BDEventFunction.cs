using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

public class BDEventBase
{
	public FrameRandomImp FrameRandom(BeEntity entity)
	{
		return entity.FrameRandom;
	}

    public virtual void OnEvent(BeEntity pkEntity)
    {
        //Logger.Log("BDEventBase OnEvent");
    }

	public virtual void PreparePreload(BeActionFrameMgr frameMgr,SkillFileListCache fileCache,bool useCube=false)
	{
	}
}

public class BDPlayEffect : BDEventBase
{
    protected EffectsFrames info;
    public EffectsFrames EffectInffo
    {
        get{ return info; }
    }
    protected Vector3 scaleBackup;

    public BDPlayEffect(object info)
    {
        this.info = info as EffectsFrames;
        scaleBackup = this.info.localScale;
    }

    public override void OnEvent(BeEntity pkEntity)
    {
        base.OnEvent(pkEntity);

        if (pkEntity == null || info == null)
            return;

        if (pkEntity.actionLooped && !info.loopLoop)
            return;

        float time = 0;
        if (info.playtype == EffectPlayType.GivenTime)
            time = info.time;

        info.localScale = scaleBackup * pkEntity.GetEnityScale().scalar;

        float x = 1;
        float y = 1;
        float z = 1;
        var owner = pkEntity.GetOwner() as BeActor;
        if (owner != null)
        {
            for (int i = 0; i < owner.MechanismList.Count; i++)
            {
                var m = owner.MechanismList[i] as Mechanism122;
                if (m != null && m.effectNameList.Contains(info.name))
                {
                    x += m.xRate.single;
                    y += m.yRate.single;
                    z += m.zRate.single;
                }
            }
        }
        if (x <= 0)
            x = 0.1f;
        if (y <= 0)
            y = 0.1f;
        if (z <= 0)
            z = 0.1f;
        info.localScale.x *= x;
        info.localScale.y *= y;
        info.localScale.z *= z;

#if !LOGIC_SERVER

        bool needPlay = true;
        //有些特效只是本地玩家才能看到
        if (owner != null)
        {
            BeActor topOwner = pkEntity.GetTopOwner(owner) as BeActor;
            if(topOwner != null && info.onlyLocalSee && !topOwner.isLocalActor)
                needPlay = false;
            if (!NeedCreateEffect(topOwner))
                needPlay = false;
        }

        float speed = 1.0f;
        var actor = pkEntity as BeActor;
        if (info.useActorSpeed && actor != null && actor.IsCastingSkill())
        {
            var skill = actor.GetCurrentSkill();
            if (skill != null)
            {
                speed = skill.GetSkillSpeedFactor().single;
            }
        }
        if (needPlay)
            pkEntity.m_pkGeActor.CreateEffect(info.effectAsset, info, time, pkEntity.GetPosition().vec3, 1f, speed, info.loop);

        info.localScale = scaleBackup;

#endif
    }
    
    protected bool NeedCreateEffect(BeActor actor)
    {
#if !LOGIC_SERVER
        if (BattleMain.instance == null)
            return true;
        if (BattleMain.IsModePvP(BattleMain.battleType))
            return true;
        if (actor.isLocalActor)
            return true;
        if (!actor.isMainActor)
            return true;
        if (SettingManager.instance.GetCommmonSet(SettingManager.STR_SKILLEFFECTDISPLAY) == SettingManager.SetCommonType.Close)
            return false;
        return true;
#else
        return true;
#endif

    }

    public override void PreparePreload (BeActionFrameMgr frameMgr, SkillFileListCache fileCache, bool useCube=false)
	{
		if (useCube)
			return;

		if (info != null) 
		{
			CResPreloader.instance.AddRes(info.effectAsset.m_AssetPath, false);		
		}
	}
}

public class BDModifySpeed : BDEventBase
{
    public int tag;
    public DSkillPropertyModifyType filter;
    public VInt value;
    public VInt valueAcc;
    public VInt movedValue;
    public VInt movedValueAcc;
    public DModifyXBackward modifyXBackward;
    public VInt movedYValue;
    public VInt movedYValueAcc;
    public bool jumpToTargetPos;
    public bool joystickControl;
    public bool eachFrameModify;
    public bool useMovedYValue;
    static readonly VInt halfMI =  VInt.Float2VIntValue(0.5f);
	public BDModifySpeed(int tag, DSkillPropertyModifyType type, float value, float movedValue = 0f, bool jumpToTargetPos = false,bool joystickControl = false, float valueAcc = 0f, 
        float movedValueAcc = 0f, DModifyXBackward modifyXBackward = DModifyXBackward.NONE, bool eachFrameModify = false, bool useMovedYValue = false, float movedYValue = 0f, float movedYValueAcc = 0f)
    {
        this.tag = tag;
        filter = type;
        this.value = new VInt(value);
        this.valueAcc = new VInt(valueAcc);
        this.movedValue = new VInt(movedValue);
        this.movedValueAcc = new VInt(movedValueAcc);
        this.modifyXBackward = modifyXBackward;
        this.movedYValue = new VInt(movedYValue);
        this.movedYValueAcc = new VInt(movedYValueAcc);
        this.jumpToTargetPos = jumpToTargetPos;
        this.joystickControl = joystickControl;
        this.eachFrameModify = eachFrameModify;
        this.useMovedYValue = useMovedYValue;
    }

    public override void OnEvent(BeEntity pkEntity)
    {
        base.OnEvent(pkEntity);

        //Logger.Log("BDModifySpeed OnEvent!!!!");

        VInt val = value;
        VInt valY = value;
        if (pkEntity.IsCastingSkill())
        {
            BeActor actor = pkEntity as BeActor;
            if (actor != null)
            {
                var skill = actor.GetCurrentSkill();
                //if (skill != null && skill.pressedForwardMove && !Utility.IsFloatZero(this.movedValue))
                if (skill != null)
                {
                    if (eachFrameModify)//是否每帧都响应方向操作
                    {
                        if (actor.IsPressForwardMoveCmd() && this.movedValue != 0)
                        {
                            val = movedValue + movedValueAcc.i * (skill.level - 1);
                        }
                        else if (actor.IsPressBackwardMoveCmd() && modifyXBackward == DModifyXBackward.STOP)
                        {
                            val = 0;
                        }
                        else if (actor.IsPressBackwardMoveCmd() && modifyXBackward == DModifyXBackward.BACKMOVE)
                        {
                            val = -movedValue - movedValueAcc.i * (skill.level - 1);
                        }
                        else
                        {
                            val = value + valueAcc.i * (skill.level - 1);
                        }
                        valY = movedYValue + movedYValueAcc.i * (skill.level - 1);
                    }
                    else
                    {
                        if (skill.pressedForwardMove && this.movedValue != 0)
                        {
                            val = movedValue + movedValueAcc.i * (skill.level - 1);
                        }
                        else
                        {
                            val = value + valueAcc.i * (skill.level - 1);
                        }
                        valY = movedYValue + movedYValueAcc.i * (skill.level - 1);
                    }
                }
            }
        }

        //int[] valueArray = new int[1];
        //valueArray[0] = GlobalLogic.VALUE_1000;
        //pkEntity.TriggerEvent(BeEventType.onChangeModifySpeed, new object[] { tag, valueArray });
        //val = val.i * VFactor.NewVFactor(valueArray[0], GlobalLogic.VALUE_1000);

        var speedValue = GlobalLogic.VALUE_1000;
        var param = pkEntity.TriggerEventNew(BeEventType.onChangeModifySpeed, new EventParam() { m_Int = tag, m_Int2 = speedValue });
        speedValue = param.m_Int2;
        val = val.i * VFactor.NewVFactor(speedValue, GlobalLogic.VALUE_1000);

        if (joystickControl)
        {
            BeAIManager.MoveDir2 dir = InputManager.GetDir8(pkEntity.GetJoystickDegree());
            switch (dir)
            {
                case BeAIManager.MoveDir2.LEFT:
                case BeAIManager.MoveDir2.RIGHT:
                    pkEntity.SetMoveSpeedXLocal(val);
                    break;
                case BeAIManager.MoveDir2.RIGHT_TOP:
                case BeAIManager.MoveDir2.TOP:
                case BeAIManager.MoveDir2.LEFT_TOP:
                    pkEntity.SetMoveSpeedXLocal(val);
                    if (useMovedYValue)
                    {
                        pkEntity.SetMoveSpeedY(valY);
                    }
                    else
                    {
                        VFactor factor = new VFactor(500, GlobalLogic.VALUE_1000);
                        pkEntity.SetMoveSpeedY(val.i * factor);
                    }
                    break;
                case BeAIManager.MoveDir2.LEFT_DOWN:
                case BeAIManager.MoveDir2.DOWN:
                case BeAIManager.MoveDir2.RIGHT_DOWN:
                    pkEntity.SetMoveSpeedXLocal(val);
                    if (useMovedYValue)
                    {
                        pkEntity.SetMoveSpeedY(-valY);
                    }
                    else
                    {
                        VFactor factor = new VFactor(500, GlobalLogic.VALUE_1000);
                        pkEntity.SetMoveSpeedY(-val.i * factor);
                    }
                    break;
                case BeAIManager.MoveDir2.COUNT:
                    break;
                default:
                    pkEntity.SetMoveSpeedXLocal(val);
                    break;
            }
        }
        else
        {

            switch (filter)
            {
                case DSkillPropertyModifyType.SPEED_X:
                    pkEntity.SetMoveSpeedXLocal(val);
                    break;
                case DSkillPropertyModifyType.SPEED_Y:
                    pkEntity.SetMoveSpeedY(val);
                    break;
                case DSkillPropertyModifyType.SPEED_Z:
                    {
                        pkEntity.SetMoveSpeedZ(val);
                        if (jumpToTargetPos)
                        {
                            BeActor actor = pkEntity as BeActor;
                            if (actor != null && actor.IsMonster() && actor.aiManager != null && actor.aiManager.aiTarget != null)
                            {
                                VInt2 MAX_SPEED = new VInt2(5f, 4f);
                                VFactor t = VFactor.NewVFactor(1, 18 * IntMath.kIntDen) * Mathf.Abs(val.i) * 2 * 30 / 60;

                                var curPos = actor.GetPosition();
                                var targetPos = actor.aiManager.aiTarget.GetPosition();

                                if (t != VFactor.zero)
                                {
                                    if (Mathf.Abs(targetPos.x - curPos.x) > halfMI.i)
                                    {
                                        int speed = Mathf.Abs(targetPos.x - curPos.x) * (VFactor.one / t);
                                        //Logger.LogErrorFormat("t:{0} speedx:{1} len:{2}", t, speed, Mathf.Abs(targetPos.x - curPos.x));
                                        speed = Mathf.Min(speed, MAX_SPEED.x);
                                        pkEntity.SetMoveSpeedX(targetPos.x - curPos.x > 0 ? speed : -speed);

                                        //Logger.LogErrorFormat("Speed x {0}",speed);
                                    }

                                    if (Mathf.Abs(targetPos.y - curPos.y) > halfMI.i)
                                    {
                                        int speed = Mathf.Abs(targetPos.y - curPos.y) * (VFactor.one / t);
                                        //Logger.LogErrorFormat("t:{0} speedy:{1} len:{2}", t, speed, Mathf.Abs(targetPos.y - curPos.y));
                                        speed = Mathf.Min(speed, MAX_SPEED.y);
                                        pkEntity.SetMoveSpeedY(targetPos.y - curPos.y > 0 ? speed : -speed);

                                        //Logger.LogErrorFormat("Speed y {0}",speed);
                                    }
                                }


                                pkEntity.SetFace(targetPos.x < curPos.x);
                            }
                        }
                    }
                    break;
                case DSkillPropertyModifyType.SPEED_XACC:
                    pkEntity.SetMoveSpeedXAcc(val);
                    break;
                case DSkillPropertyModifyType.SPEED_YACC:
                    pkEntity.SetMoveSpeedYAcc(val);
                    break;
                case DSkillPropertyModifyType.SPEED_ZACC:
                    pkEntity.SetMoveSpeedZAcc(val);
                    break;
                case DSkillPropertyModifyType.SPEED_ZACC_NEW:
                    pkEntity.SetMoveSpeedZAccExtra(val);
                    break;
            }
        }
    }
}


public class BDSceneShock : BDEventBase
{
    public float time;
    public float speed;
    public float xrange;
    public float yrange;

    public bool isNew;
    public int mode;
    public bool decelerate;
    public float xreduce;
    public float yreduce;
    public int num;
    public float radius;
    public BDSceneShock(float t, float s, float xr, float yr)
    {
        time = t;
        speed = s;
        xrange = xr;
        yrange = yr;
        isNew = false;
    }

    public BDSceneShock(float totalTime, int num, float xRange, float yRange, bool decelebrate, float xReduce, float yReduce, int mode, float radius = 1)
    {
        this.time = totalTime;
        this.num = num;
        this.xrange = xRange;
        this.yrange = yRange;
        this.decelerate = decelebrate;
        this.xreduce = xReduce;
        this.yreduce = yReduce;
        this.mode = mode;
        this.radius = radius;
        isNew = true;
    }

    public override void OnEvent(BeEntity pkEntity)
    {
        base.OnEvent(pkEntity);

        //Logger.Log("BDSceneShock OnEvent!!!!");
#if !LOGIC_SERVER
        BeActor actor = pkEntity.GetTopOwner(pkEntity) as BeActor;
        if (actor != null && actor.isLocalActor)
        {
            PlayShockEffect(pkEntity);
        }
#endif
    }
    void PlayShockEffect(BeEntity pkEntity)
    {
        if (BattleMain.instance != null)
        {
            if (isNew)
            {
                BattleMain.instance.Main.currentGeScene.GetCamera().PlayShockEffect(time, num, xrange, yrange, decelerate, xreduce, yreduce, mode, radius, pkEntity.IsCurSkillOpenShock());
            }
            else
            {
                BattleMain.instance.Main.currentGeScene.GetCamera().PlayShockEffect(time, speed, xrange, yrange, 2, pkEntity.IsCurSkillOpenShock());
            }
        }
    }
}

//抓取
public class BDSkillSuspend : BDEventBase
{
    public int suspendType;
    public bool faceGraber;
    public VInt3 suspendTargetPos = VInt3.zero;
    public int actionType;
    public int angle;

    public BDSkillSuspend(int type)
    {
        suspendType = type;
    }

    public BDSkillSuspend(int type, VInt3 targetPos,bool faceOffset, int targetAction, int targetAngle)
    {
        suspendType = type;
        faceGraber = faceOffset;
        suspendTargetPos = targetPos;
        actionType = targetAction;
        angle = targetAngle;
    }

    public override void OnEvent(BeEntity pkEntity)
    {
        base.OnEvent(pkEntity);

        BeActor pkActor = (BeActor)pkEntity;
        if (pkActor == null)
            return;

        //Logger.Log("BDSkillSuspend OnEvent");

        if (suspendType == (int)DSFGrapOp.GRAP_INTERRUPT)
        {

            if (!pkActor.grabController.HasGrabbedEntity())
            {
                BeStateData state = new BeStateData((int)ActionState.AS_IDLE);
                pkActor.sgForceSwitchState(state);
            }
        }
        //改变被抓取者的坐标
        else if (suspendType == (int)DSFGrapOp.GRAP_CHANGE_TARGETPOS && pkActor.stateController.IsGrabbing())
        {
            if (pkActor.grabController.grabbedActorList != null)
            {
                pkActor.grabController.grabPos = true;
                for (int i = 0; i < pkActor.grabController.grabbedActorList.Count; i++)
                {
                    BeActor target = pkActor.grabController.grabbedActorList[i];
                    if (target == null || target.IsDead())
                        continue;
                    if (faceGraber)
                    {
                        int xOffset = target.GetPosition().x - pkActor.GetPosition().x;
                        target.SetFace(xOffset < 0 ? false : true);
                    }
                    VInt3 newPos = pkActor.GetPosition();
                    newPos.x += suspendTargetPos.x * pkActor._getFaceCoff();
                    newPos.y += suspendTargetPos.y;
                    newPos.z += suspendTargetPos.z;

                    if (target.CurrentBeScene != null
                        && target.grabController.GetGrabData() != null
                        && target.grabController.GetGrabData().notGrabToBlock 
                        && target.CurrentBeScene.IsInBlockPlayer(newPos))
                        ;
                    else
                        target.SetPosition(newPos, true);
                    VInt3 curPos = target.GetPosition();
                    if (suspendTargetPos.z == 0 && curPos.z != 0)
                        target.SetPosition(new VInt3(curPos.x, curPos.y,0), true);
                }
            }
        }
        else if (suspendType == (int)DSFGrapOp.GRAP_STOPCHANGE_TARGETPOS && pkActor.stateController.IsGrabbing())
        {
            pkActor.grabController.grabPos = false;
        }
        else if (suspendType == (int)DSFGrapOp.GRAP_CHANGE_TARGETACTION && pkActor.stateController.IsGrabbing())
        {
            if(pkActor.grabController.grabbedActorList != null)
            {
                for (int i = 0; i < pkActor.grabController.grabbedActorList.Count; i++)
                {
                    BeActor target = pkActor.grabController.grabbedActorList[i];
                    if (target == null || target.IsDead())
                        continue;
                    target.PlayAction((ActionType)actionType);
                }
            }
        }
        else if (suspendType == (int)DSFGrapOp.GRAP_CHANGE_TARGETROTATION && pkActor.stateController.IsGrabbing())
        {
#if !LOGIC_SERVER
            if (pkActor.grabController.grabbedActorList != null)
            {
                for (int i = 0; i < pkActor.grabController.grabbedActorList.Count; i++)
                {
                    var pkGeActor = pkActor.grabController.grabbedActorList[i].m_pkGeActor;
                    if (pkGeActor != null)
                    {
                        var objActor = pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Actor);
                        if (objActor != null)
                        {
                            var offset = new Vector3(0, 0.7f, 0);
                            //从OverHead节点到脚底距离的一半作为轴心，旋转一定角度
                            var overHead = Utility.FindThatChild("OverHead", objActor);
                            if (overHead != null)
                            {
                                offset.y = (overHead.transform.position - objActor.transform.position).magnitude * 0.5f;
                            }

                            var dir = Quaternion.AngleAxis(angle, -Vector3.forward) * offset;
                            objActor.transform.localPosition = -dir + offset;
                            objActor.transform.localRotation = Quaternion.Euler(0, 0, 360 - angle);
                        }
                    }
                }
            }
#endif
        }
    }
}

public class BDGenProjectile : BDEventBase
{
    //public int _resid;
    //public VInt _x, _y, _speedx, _speedy, _speedz, _zacc, _delay;
    //public int _type;
    //public int _value;
    public object _info;

    public BDGenProjectile(object info)
    {
        _info = info;
    }

    public override void OnEvent(BeEntity pkEntity)
    {
        base.OnEvent(pkEntity);

        Logger.Log("BDGenProjectile OnEvent");

        if (pkEntity == null)
            return;

        EntityFrames frame = _info as EntityFrames;
        if (frame == null)
            return;

        if (frame.type == EntityType.LogicEntity)
        {
        }
        else
        {

			BeScene main = pkEntity.CurrentBeScene; //BattleMain.instance.Main;

			if (frame.useRandomLaunch)
			{
                //var curPos = pkEntity.GetPosition();
                var curPos = GetStartPosition(pkEntity);

                int totalNum = frame.randomLaunchInfo.num;
                if (frame.randomLaunchInfo.isNumRand)
                    totalNum = FrameRandom(pkEntity).InRange(IntMath.Float2Int(frame.randomLaunchInfo.numRandRange.x), IntMath.Float2Int(frame.randomLaunchInfo.numRandRange.y + 1));
                for (int i = 0; i < totalNum; ++i)
                {
                    VInt3 pos = new VInt3();
                    if (frame.randomLaunchInfo.isFullScene)             //全场景随机取点
                    {
                        pos = pkEntity.CurrentBeScene.GetRandomPos(20);
                        pos.z = curPos.z + VInt.Float2VIntValue(frame.emitposition.y);
                    }
                    else
                    {
                        int r = VInt.Float2VIntValue(frame.randomLaunchInfo.rangeRadius);
                        var offsetX =(!pkEntity.GetFace()) ? VInt.Float2VIntValue(frame.emitposition.x) : -VInt.Float2VIntValue(frame.emitposition.x);
                        pos.x = curPos.x + FrameRandom(pkEntity).InRange(-r, r) + offsetX;
                        pos.y = curPos.y + FrameRandom(pkEntity).InRange(-r, r) + VInt.Float2VIntValue(frame.emitPositionZ);
                        pos.z = curPos.z + VInt.Float2VIntValue(frame.emitposition.y);
                    }
                    int ms = IntMath.Float2Int(frame.randomLaunchInfo.interval * GlobalLogic.VALUE_1000) * i;

                    //Logger.LogErrorFormat("random index:{0} pos:({1},{2},{3}), delay:{4} total:{5}", i, pos.x, pos.y, pos.z, ms, totalNum);

                    pkEntity.delayCaller.DelayCall(ms, () =>
                    {
                        LaunchProjectile(pkEntity, frame, main, true, pos);
                    });
                }
			}
			else
            {
                //int[] delayTimeArray = new int[1];          //延时时间
                //int[] copyNumArray = new int[1];            //复制数量
                //pkEntity.TriggerEvent(BeEventType.onChangeLaunchProNum, new object[] { frame.resID, delayTimeArray, copyNumArray});

                int delayTime = 0;      //延时时间
                int copyNum = 0;        //复制数量
                var param = pkEntity.TriggerEventNew(BeEventType.onChangeLaunchProNum, new EventParam() { m_Int = frame.resID, m_Int2 = delayTime, m_Int3 = copyNum });
                delayTime = param.m_Int2;
                copyNum = param.m_Int3;

                for(int i = 0; i < copyNum + 1; i++)
                {
                    int time = i * delayTime;
                    if (time != 0)
                    {
                        pkEntity.delayCaller.DelayCall(time, () =>
                        {
                            LaunchProjectile(pkEntity, frame, main);
                        });
                    }
                    else
                    {
                        LaunchProjectile(pkEntity, frame, main);
                    }
                }
			}
        }
    }


	void LaunchProjectile(BeEntity pkEntity, EntityFrames frame, BeScene main, bool useGivenPos=false, VInt3 gPos= new VInt3())
	{
		int projectResID = frame.resID;
		int attachHurt = 0;

		if (frame.type == EntityType.Bullet)
		{
            //int[] resIDs = new int[3];
            //resIDs[0] = projectResID;
            //resIDs[2] = 1;        // 用于判断子弹是否发射：1表示发射子弹，否则不发射
            //pkEntity.TriggerEvent(BeEventType.onBeforeGenBullet, new object[] { resIDs });
            //if (resIDs[0] != 0)
            //{
            //    projectResID = resIDs[0];
            //    attachHurt = resIDs[1];
            //}

            //1表示发射子弹，否则不发射
            //if (resIDs[2] != 1)
            //    return;

            bool isLaunch = true;
            var param = pkEntity.TriggerEventNew(BeEventType.onBeforeGenBullet, new EventParam() { m_Int = projectResID, m_Int2 = attachHurt, m_Bool = isLaunch });
            projectResID = param.m_Int;
            attachHurt = param.m_Int2;
            isLaunch = param.m_Bool;

            if (!isLaunch)
            {
                return;
            }
        }

		int triggerSkillLevel = 1;
		//设定所属技能等级 
		if (pkEntity.IsCastingSkill())
		{
			var actor = pkEntity as BeActor;
			var skill = actor.GetCurrentSkill();
			if (skill != null)
			{
				triggerSkillLevel = skill.level;
				//pkProjectile.triggerSkillLevel = skill.GetLevel();
			}
		}


		BeEntity owner = null;

		if ((pkEntity as BeProjectile) != null )
			owner = pkEntity.GetOwner();
			//pkProjectile.SetOwner(pkEntity.GetOwner());
		else
			owner = pkEntity;
			//pkProjectile.SetOwner(pkEntity);

		//!! Owner检查
		if(owner.IsRemoved())
		{
			return;
		}
        if (frame.randomResID)
        {
            int index = pkEntity.FrameRandom.InRange(0, frame.resIDList.Length);
            projectResID = frame.resIDList[index];
        }

        BeProjectile pkProjectile = main.AddProjectile(projectResID, pkEntity.m_iCamp, (int)frame.type, IntMath.Float2Int(frame.lifeTime * GlobalLogic.VALUE_1000), triggerSkillLevel, owner);
        pkProjectile.AttackProcessId = pkEntity.AttackProcessId;
		pkProjectile.hurtID = frame.hurtID;
		pkProjectile.attribute = pkEntity.attribute;
		pkProjectile.hitThrough = frame.hitThrough;
		pkProjectile.totoalHitCount = frame.hitCount;
		pkProjectile.attackCountExceedPlayExtDead = frame.attackCountExceedPlayExtDead;
		pkProjectile.distance = VInt.Float2VIntValue(frame.distance);
		pkProjectile.delayDead = IntMath.Float2Int(frame.delayDead * GlobalLogic.VALUE_1000);
		pkProjectile.hitGroundClick = frame.hitGroundClick;
        pkProjectile.onCollideDie = frame.onCollideDie;
        pkProjectile.onXInBlockDie = frame.onXInBlockDie;
        pkProjectile.changForceBehindOther = frame.changForceBehindOther;

		if (attachHurt > 0)
		{
			pkProjectile.attachHurts.Add(attachHurt);
		}

		pkProjectile.SetFace(pkEntity.GetFace());
        if(frame.changeFace == 1)
        {
            pkProjectile.SetFace(true, true, true);
        }
        else if(frame.changeFace == 2)
        {
            pkProjectile.SetFace(false, true, true);
        }

		VInt3 pos = new VInt3();
        //使用父实体的位置
        ProtoTable.ObjectTable objectData = TableManager.GetInstance().GetTableItem<ProtoTable.ObjectTable>(projectResID);
        if (objectData != null && objectData.UseOwnerPos)
        {
            pos = pkEntity.GetPosition();
            if (objectData.UseOffset)
            {
                pos.z += VInt.Float2VIntValue(frame.emitposition.y);
                pos.y += VInt.Float2VIntValue(frame.emitPositionZ);
                pos.x += (!pkEntity.GetFace()) ? VInt.Float2VIntValue(frame.emitposition.x) : -VInt.Float2VIntValue(frame.emitposition.x);
            }
        }
        else if (useGivenPos)
		{
			pos = gPos;
		}
		else
        {
			//选择出生位置
			pos = GetStartPosition(pkProjectile.GetOwner());

            if (pkEntity.CurrentBeScene != null)
            {
                var eventParam = pkEntity.CurrentBeScene.TriggerEventNew(BeEventSceneType.onChangeStartPos, new EventParam() { m_Obj = pkProjectile, m_Vint3 = pos });
                pos = eventParam.m_Vint3;
            }
            
			pos.z += VInt.Float2VIntValue(frame.emitposition.y);
			pos.y += VInt.Float2VIntValue(frame.emitPositionZ);
			pos.x += (!pkProjectile.GetFace())? VInt.Float2VIntValue(frame.emitposition.x):-VInt.Float2VIntValue(frame.emitposition.x);
		}
			
		//速度设置
		//VFactor angle = new VFactor(IntMath.Float2Int(frame.angle*100), 100);
		int angle = IntMath.Float2Int(frame.angle * 1000);
		if (frame.offsetType == OffsetType.OFFSET_ANGLE)
		{
			angle +=  FrameRandom(pkEntity).InRange(-800, 800); //FrameRandom.InRange(-0.8f,0.8f);
		}

        //改变实体的速度
        /*VInt[] speedArray = new VInt[1];
        speedArray[0] = (VInt)frame.speed;
        owner.TriggerEvent(BeEventType.onChangeProjectileSpeed, new object[] { pkProjectile,speedArray });
        VInt frameSpeed = speedArray[0];*/
        var ret = owner.TriggerEventNew(BeEventType.onChangeProjectileSpeed, new EventParam{m_Obj = pkProjectile, m_Vint = (VInt) frame.speed});
        VInt frameSpeed = ret.m_Vint;

        VFactor degree = VFactor.NewVFactor(angle, (long)1000) / (VFactor.pi * 2);
		VInt sx = frameSpeed.i * IntMath.cos(degree.nom, degree.den) /*Mathf.Cos(degree)*/;
		VInt sy = frameSpeed.i * IntMath.sin(degree.nom, degree.den)/*Mathf.Sin(degree)*/;

		int offset = VInt.Float2VIntValue(0.2f);

		//这里做子弹位置偏移
		if (frame.offsetType == OffsetType.OFFSET_VERTICAL)
		{
			if (sx != 0)
			{
				pos.y += FrameRandom(pkEntity).InRange(-offset, offset);
			}

			if (sy != 0)
			{
				pos.x += FrameRandom(pkEntity).InRange(-offset, offset);
			}
		}

		pkProjectile.SetPosition(pos);
		pkProjectile.SetScale(pkEntity.GetScale().i * pkProjectile.GetProjectileScale());
		pkProjectile.SetZDimScaleFactor(pkProjectile.GetProjectileZDimScale());
		pkProjectile._updateGraphicActor();

		pkProjectile.moveZAcc = new VInt(frame.gravity.y);
		pkProjectile.moveXAcc = new VInt(frame.gravity.x);    

		//在这里设置伤害的类型
		if (pkEntity.m_cpkCurEntityActionInfo != null)
		{
			BDEntityActionInfo actionInfo = pkEntity.m_cpkCurEntityActionInfo;
			//pkProjectile.hurtType = actionInfo.iActionType;
			//pkProjectile.forcex = actionInfo.fActionForcex;
			//pkProjectile.forcey = actionInfo.fActionForcey;
			//pkProjectile.hurtTime = actionInfo.hurtTime;
		}
		//打倒目标触发目标震动
		if (frame.shockTime > 0)
		{
			ShockInfo targetShockInfo = new ShockInfo();
			targetShockInfo.shockTime = frame.shockTime;
			targetShockInfo.shockSpeed = frame.shockSpeed;
			targetShockInfo.shockRangeX = frame.shockRangeX;
			targetShockInfo.shockRangeY = frame.shockRangeY;
			pkProjectile.SetTargetShockInfo(targetShockInfo);
		}
		//屏幕震动
		pkProjectile.SetSceneShockInfo(frame.sceneShock);

		if (frame.axisType == AxisType.Z)
		{
			pkProjectile.SetMoveSpeedXLocal(sx);
			pkProjectile.SetMoveSpeedZ(sy);

            if (frame.isAngleWithEffect)
            {
                pkProjectile.InitLocalRotation();
                pkProjectile.isAngleWithEffect = frame.isAngleWithEffect;
            }
		}
		else if (frame.axisType == AxisType.Y)
		{
            //float radian = Mathf.PI / 180f * angle;
			VFactor radian = VFactor.pi * VInt.Float2VIntValue(frame.angle) / 180 / IntMath.kIntDen;
			//sx = frame.speed * Mathf.Cos(radian);
			//sy = frame.speed * Mathf.Sin(radian);
			sx = frameSpeed.i * IntMath.cos(radian.nom, radian.den);
			sy = frameSpeed.i * IntMath.sin(radian.nom, radian.den);
			pkProjectile.SetMoveSpeedXLocal(sx);
			pkProjectile.SetMoveSpeedY(sy);
			pkProjectile.SetMoveSpeedZ(0);
		}
		else if (frame.axisType == AxisType.X)
		{
			Logger.LogWarning("暂不处理AxisType.Z");
		}


		pkProjectile.RecordOriginPosition();

		pkProjectile.isHitFloat = (frame.hitFallUP == 1);
		pkProjectile.hitFloatForceY = frame.forceY;

		//是否是绕点旋转的轨迹
		if (frame.isRotation)
		{
			pkProjectile.rotateSpeed = VInt.Float2VIntValue(frame.rotateSpeed/100f);
			pkProjectile.moveSpeed = VInt.Float2VIntValue(frame.moveSpeed/1000f);
			pkProjectile.InitRotation(frame.rotateInitDegree);
		} 

		pkEntity.TriggerEventNew(BeEventType.onAfterGenBullet, new EventParam() { m_Obj = pkProjectile });
		owner.TriggerEventNew(BeEventType.onOwnerAfterGenBullet, new EventParam() { m_Obj = pkProjectile });
        if(owner.CurrentBeScene != null)
            owner.CurrentBeScene.TriggerEventNew(BeEventSceneType.onAfterGenBullet, new EventParam() { m_Obj = owner , m_Obj2 = pkProjectile});

        if (frame.targetChooseType == TargetChooseType.SMART_NEAREST)
        {
            var trail = pkEntity.CurrentBeBattle.LogicTrailManager.AddLinearLogicTrial(pkEntity, pkProjectile, new VInt2(frame.range.x, frame.range.y), sx);
            pkProjectile.logicTrail = trail;
        }
        else if (frame.targetChooseType == TargetChooseType.MAX_RESENTMENT)
        {
            BeScene mainLogic = pkEntity.CurrentBeScene;
            if (null != mainLogic)
            {
                BeActor target = mainLogic.GetResentmentActor(true);
                if (target != null)
                {
                    pkProjectile.SetPosition(target.GetPosition(), true);
                }
            }
        }
        else if (frame.targetChooseType == TargetChooseType.BOOMERANGE)
        {
            var trail = pkEntity.CurrentBeBattle.LogicTrailManager.AddBoomerangLogicTrail(pkEntity, pkProjectile,
                new VInt2(frame.emitposition.x, frame.emitPositionZ),
                new VInt2(frame.range.x, frame.range.y),
                VInt.Float2VIntValue(frame.speed),
                VInt.Float2VIntValue(frame.boomerangeDistance),
                VInt.Float2VIntValue(frame.stayDuration) / 10); 
            pkProjectile.logicTrail = trail;
        }
        else if (frame.targetChooseType == TargetChooseType.PARABOLA_TARGET_POS)
		{
			//!! 这里很麻烦。。 需要确认！
// 			BeActor target = (pkEntity as BeActor).aiManager.aiTarget;
// 			if (target != null)
// 			{
// 				float speed = frame.paraSpeed;
// 				Vector3 startPos = pkProjectile.GetGePosition();
// 				Vector3 endPos = target.GetGePosition();
// 				pkProjectile.moveZAcc = VInt.Conver(frame.paraGravity);
// 				startPos.y = 0;
// 
// 				float t = (endPos - startPos).magnitude / speed;
// 				float speedZ = (pkProjectile.moveZAcc * t*t - 2f*pkProjectile.GetGePosition().y) / (2f*t);
// 
// 				var dir = (endPos - startPos).normalized;
// 
// 				pkProjectile.SetMoveSpeedX(dir.x*speed);
// 				pkProjectile.SetMoveSpeedY(dir.z*speed);
// 				pkProjectile.SetMoveSpeedZ(speedZ);
// 			}
		}
		else if (frame.targetChooseType == TargetChooseType.PARABOLA_TARGET)
		{
// 			BeActor target = (pkEntity as BeActor).aiManager.aiTarget;
// 			if (target != null)
// 			{
// 				var trail = new ParabolaBehaviour();
// 
// 				trail.TotalTime = Global.Settings.TotalTime;
// 				trail.TimeAccerlate = Global.Settings.TimeAccerlate;
// 				trail.StartPoint = pkProjectile.GetGePosition();
// 				trail.EndPoint = target.GetGePosition();
// 
// 				var tmp = (trail.EndPoint - trail.StartPoint).normalized;
// 
// 				var dir =  new Vector3(tmp.x*Global.Settings.startVel.x, 0, tmp.z*Global.Settings.startVel.z) + Global.Settings.endVel;//new Vector3(0, 5, 0);
// 
// 				//Logger.LogErrorFormat("trial dir:({0},{1},{2})", dir.x, dir.y, dir.z);
// 				if (target != null)
// 					trail.SetTarget(target);
// 				trail.StartVelocity = dir;
// 
// 				trail.Start();
// 				trail.SetTotalDist((trail.EndPoint - trail.StartPoint).magnitude);
// 
// 				pkProjectile.trail = trail;
// 			}
		}
        else if (frame.targetChooseType == TargetChooseType.GRAVITATION)
        {
            var trail = pkEntity.CurrentBeBattle.LogicTrailManager.AddGravitationTrail(pkEntity, pkProjectile, VInt.Float2VIntValue(frame.speed));
            pkProjectile.logicTrail = trail;
        }
        else if (frame.targetChooseType == TargetChooseType.FOLLOW_TARGET)
        {
            var trail = pkEntity.CurrentBeBattle.LogicTrailManager.AddFollowTargetTrail(pkEntity, pkProjectile, sx.i / GlobalLogic.VALUE_10);
            pkProjectile.logicTrail = trail;
        }
        else if (frame.targetChooseType == TargetChooseType.CHASE_TARGET)
        {
            var trail = pkEntity.CurrentBeBattle.LogicTrailManager.AddChaseTargetTrail(pkEntity, pkProjectile, new VInt3(frame.offset.x, frame.offset.y, 0));
            pkProjectile.logicTrail = trail;
        }
        else if (frame.targetChooseType == TargetChooseType.ROTATE_CHASE_TARGET)
        {
            var trail = pkEntity.CurrentBeBattle.LogicTrailManager.AddRotateChaseTargetTrail(pkEntity, pkProjectile, 
                VInt.Float2VIntValue(frame.angle), 
                VInt.Float2VIntValue(frame.changeMaxAngle),
                VInt.Float2VIntValue(frame.speed),
                VInt.Float2VIntValue(frame.chaseTime) / GlobalLogic.VALUE_10);
            pkProjectile.logicTrail = trail;
            if (frame.isAngleWithEffect)
            {
                pkProjectile.InitLocalRotation();
                pkProjectile.isAngleWithEffect = frame.isAngleWithEffect;
            }
            else
            {
                pkProjectile.isAngleWithEffect = false;
            }
        }
        else if (frame.targetChooseType == TargetChooseType.REBOUND)
        {
            var trail = pkEntity.CurrentBeBattle.LogicTrailManager.AddReboundTrail(pkEntity, pkProjectile, 
                VInt.Float2VIntValue(frame.angle),
                VInt.Float2VIntValue(frame.speed),
                VInt.Float2VIntValue(frame.chaseTime) / GlobalLogic.VALUE_10000);
            pkProjectile.logicTrail = trail;
            if (frame.isAngleWithEffect)
            {
                pkProjectile.InitLocalRotation();
                pkProjectile.isAngleWithEffect = frame.isAngleWithEffect;
            }
            else
            {
                pkProjectile.isAngleWithEffect = false;
            }
        }

        pkProjectile.targetChooseType = frame.targetChooseType;
    }

    VInt3 GetStartPosition(BeEntity owner)
    {
        VInt3 pos = owner.GetPosition();
        EntityFrames frame = _info as EntityFrames;
        switch(frame.targetChooseType)
        {
            case TargetChooseType.NEAREST:

                BeScene main = owner.CurrentBeScene;

                if (null != main)
                {
                    BeActor target = main.FindTarget(owner as BeActor, (VInt)30f);
                    if (target != null)
                    {
                        pos = target.GetPosition();
                        pos.z = 0;
                    }
                }

                break;
            case TargetChooseType.FARTEST:
                break;
            case TargetChooseType.FULL_SCREEN:
                break;
        }

        return pos;

    }

	public override void PreparePreload (BeActionFrameMgr frameMgr,SkillFileListCache fileCache ,bool useCube=false)
	{
		if (useCube)
			return;

		EntityFrames frame = _info as EntityFrames;
		if (frame != null)
		{
			int resID = frame.resID;
			if (resID != 0)
				GameClient.PreloadManager.PreloadResID(resID, frameMgr, fileCache);
		}
	}

    //获取投掷物的召唤者
    protected BeActor GetProjectileOwner(BeEntity entity)
    {
        BeActor actor = entity as BeActor;
        if (actor != null)
            return actor;
        actor = entity.GetOwner() as BeActor;
        if (actor != null)
            return actor;
#if UNITY_EDITOR
        Logger.LogErrorFormat("没有找到Id:{0},实体的召唤者，策划请检查配置", entity.m_iResID);
#endif
        return null;
    }
}

public class BDStateStackOP : BDEventBase
{
    public int _op, _state, _statedata, _extag;
    public int _statedata2, _statedata3;

    public BDStateStackOP(int iOP, int iState, int iStateData, float fStateData2, float fStateData3, int iExTag)
    {
        _op = iOP;
        _state = iState;
        _statedata = iStateData;
        _statedata2 = VInt.Float2VIntValue(fStateData2);
        _statedata3 = VInt.Float2VIntValue(fStateData3);
        _extag = iExTag;
    }

    public override void OnEvent(BeEntity pkEntity)
    {
        base.OnEvent(pkEntity);

        if (pkEntity == null)
            return;

        //Logger.Log("BDStateStackOP OnEvent");

        if (pkEntity.actionLooped)
            return;


        switch ((DSFEntityStateOp)_op)
        {
            case DSFEntityStateOp.Clear:
                pkEntity.sgClearStateStack();
                break;

            case DSFEntityStateOp.Pop:
                pkEntity.sgPopState();
                break;

            case DSFEntityStateOp.Push:
                pkEntity.sgPushState(new BeStateData(_state)
                {
                    _StateData = _statedata,
                    _StateData2 = _statedata2,
                    _StateData3 = _statedata3,
                    _ExTag = _extag
                });
                break;

            case DSFEntityStateOp.Locomote:
                pkEntity.sgLocomoteState();
                break;
        }
    }
}

public class BDSkillSfx : BDEventBase
{
    public Object objSfx = null;
    public string objSfxPath = null;
	public bool loop = false;
	public int soundID = 0;
    protected bool m_UseActorSpeed = false;
    protected bool m_PhaseFinishDelete = false;
    protected bool m_FinishDelete = false;
    public ProtoTable.SoundTable soundData = null;
    public float m_Volume = 0;

    public BDSkillSfx(DAssetObject assetObj, int soundID = 0, bool loop = false, bool useActorSpeed = false, bool phaseFinishDelete = false, bool finishDelete = false,float volume = 0)
    {
		#if !LOGIC_SERVER
        objSfx = assetObj.m_AssetObj;
        objSfxPath = assetObj.m_AssetPath;
		this.loop = loop;
		this.soundID = soundID;
        this.m_UseActorSpeed = useActorSpeed;
        this.m_PhaseFinishDelete = phaseFinishDelete;
        this.m_FinishDelete = finishDelete;
        this.m_Volume = volume;

        if (this.soundID > 0)
		{
			soundData = TableManager.GetInstance().GetTableItem<ProtoTable.SoundTable>(soundID);
		}
		#endif
    }

    public BDSkillSfx(Object obj)
    {
        objSfx = obj;
    }
    public override void OnEvent(BeEntity pkEntity)
    {
        
#if !LOGIC_SERVER
		if (pkEntity.actionLooped && !loop)
			return;

		if (pkEntity.m_pkGeActor != null && pkEntity.m_pkGeActor.GetUseCube())
			return;

        if (pkEntity.CurrentBeBattle != null && !pkEntity.CurrentBeBattle.NeedPlaySound)
            return;

        float speed = 1;
        var actor = pkEntity as BeActor;
        if (m_UseActorSpeed && actor != null && actor.IsCastingSkill())
        {
            var skill = actor.GetCurrentSkill();
            if (skill != null)
            {
                speed = skill.GetSkillSpeedFactor().single;
            }
        }

        uint audio = 0;
        if (soundID > 0)
		{
            audio = AudioManager.instance.PlaySound(soundData, speed, m_Volume);
            //AudioManager.instance.PlaySound(soundID);
        }
		else
        {
			if (objSfx != null)
			{
                audio = AudioManager.instance.PlaySound(objSfx as AudioClip, AudioType.AudioEffect, m_Volume, false, null, true,false,null, speed);
			}
            else
            {
                audio = AudioManager.instance.PlaySound(objSfxPath, AudioType.AudioEffect, m_Volume, false, null, true, false, null, speed);
            }  
		}

        if (m_PhaseFinishDelete && !pkEntity.PhaseDeleteAudioList.Contains(audio))
        {
            pkEntity.PhaseDeleteAudioList.Add(audio);
        }

        if(m_FinishDelete && !pkEntity.FinishDeleteAudioList.Contains(audio))
        {
            pkEntity.FinishDeleteAudioList.Add(audio);
        }

 #endif

    }

	public override void PreparePreload (BeActionFrameMgr frameMgr,SkillFileListCache fileCache ,bool useCube=false)
	{
#if !LOGIC_SERVER
 
		if (useCube)
			return;
		
		if (soundID > 0)
		{
			//AudioManager.instance.PreloadSound(soundData.Path);
			AudioManager.instance.PreloadSound(soundID);
		}
		else if (objSfx == null)
		{
			AudioManager.instance.PreloadSound(objSfxPath);
		}
 #endif

	}
}

public class BDSkillFrameEffect : BDEventBase
{
    public int effectID;
    public int buffTime;
    bool useBuffAni = true;
    bool usePause;
    float pauseTime;
	bool finishDelete = false;
    bool finishDeleteAll = false;
    int mechanismId;

    public BDSkillFrameEffect(int eid, int d, bool useBuffAni, bool usePause, float pauseTime, bool finishDelete, bool finishDeleteAll,int mechanismId)
    {
        effectID = eid;
        buffTime = d;
        this.useBuffAni = useBuffAni;
        this.usePause = usePause;
        this.pauseTime = pauseTime;
		this.finishDelete = finishDelete;
        this.finishDeleteAll = finishDeleteAll;
        this.mechanismId = mechanismId;
    }

    public override void OnEvent(BeEntity pkEntity)
    {
        base.OnEvent(pkEntity);

        //触发帧事件
        Logger.Log("OnEvent frame effect:" + effectID);

        if (usePause)
        {
            pkEntity.Pause(IntMath.Float2Int(pauseTime * GlobalLogic.VALUE_1000));
        }
        else
        {
            pkEntity.DealEffectFrame(pkEntity, effectID, buffTime, false, useBuffAni, finishDelete, finishDeleteAll, pkEntity.AttackProcessId);
        }

        if (mechanismId > 0)
        {
            pkEntity.TryAddMechanism(mechanismId);
        }
    }

	public override void PreparePreload (BeActionFrameMgr frameMgr,SkillFileListCache fileCache, bool useCube=false)
	{
		if (useCube)
			return;
		
		GameClient.PreloadManager.PreloadEffectID(effectID, frameMgr, fileCache);

        if(mechanismId > 0)
            PreloadManager.PreloadSpecialScripts(PreloadSpecialType.Mechanism, mechanismId, frameMgr, fileCache);
    }
}

public class BDSkillCameraMove : BDEventBase
{
    public float offset;
    public float duration;

    public BDSkillCameraMove(float off, float dur)
    {
        offset = off;
        duration = dur;
    }

    public override void OnEvent(BeEntity pkEntity)
    {
        base.OnEvent(pkEntity);

#if !LOGIC_SERVER
 
        Logger.LogFormat("OnEvent camera move offset {0} duraction {1}", offset, duration);

	
		BeActor actor = pkEntity as BeActor;

		if (actor != null && actor.isLocalActor)
        {
			float off = offset * (pkEntity.GetFace() ? -1.0f : 1.0f);
			actor.CurrentBeScene.currentGeScene.GetCamera().GetController().MoveCamera(off, duration);
        }

 #endif

    }
}

public class BDSkillWalkControl : BDEventBase
{
    public SkillWalkMode walkMode;
    public VFactor walkPercent;
    public bool useSkillSpeed;
    public VFactor walkPercent2;

    public BDSkillWalkControl(SkillWalkMode m, float speed, bool useSkillSpeed=false, float speed2 = 0)
    {
        walkMode = m;
        walkPercent = VFactor.NewVFactorF(speed,1000);
        this.useSkillSpeed = useSkillSpeed;
        walkPercent2 = VFactor.NewVFactorF(speed2, 1000);
    }

    public override void OnEvent(BeEntity pkEntity)
    {
        base.OnEvent(pkEntity);

        if (pkEntity.IsCastingSkill())
        {
            BeActor actor = pkEntity as BeActor;
            if (actor != null)
            {
                var skill = actor.GetCurrentSkill();
                if (skill != null && !skill.CanWalk())
                {
                    if (useSkillSpeed)
                    {
                        walkPercent = skill.GetWalkSpeedRate();
                        walkPercent2 = skill.GetWalkSpeedRate();
                    }
                    actor.SetSkillWalkMode(walkMode, walkPercent, walkPercent2);
                }
            }
        }
    }
}

public class BDEventFunction {
}

public class BDSkillAction : BDEventBase
{
	public BeActionType actionType;
	public float duration;
	public float deltaScale;
	public VInt3 deltaPos;
    public bool ignoreBlock;

	public BDSkillAction(BeActionType type, float dur, float scale, VInt3 pos, bool ignoreBlock = true)
	{
		actionType = type;
		duration = dur;
		deltaScale = scale;
		deltaPos = pos;
        this.ignoreBlock = ignoreBlock;
	}

	public override void OnEvent (BeEntity pkEntity)
	{
		base.OnEvent (pkEntity);

		if (actionType == BeActionType.MoveBy)
		{
			VInt3 tmp = deltaPos;
			tmp.x *= pkEntity._getFaceCoff();

            pkEntity.SaveCurrentPosition();

            pkEntity.actionManager.RunAction(BeMoveBy.Create(pkEntity,IntMath.Float2Int(duration*GlobalLogic.VALUE_1000), pkEntity.GetPosition(), tmp, ignoreBlock));
		}
		else if (actionType == BeActionType.ScaleBy)
		{
			pkEntity.actionManager.RunAction(BeScaleBy.Create(pkEntity,IntMath.Float2Int(duration*GlobalLogic.VALUE_1000), pkEntity.GetScale(), (VInt)deltaScale));
		}
		else if (actionType == BeActionType.MoveToSavedPosition)
		{
			pkEntity.actionManager.RunAction(BeMoveTo.Create(pkEntity, IntMath.Float2Int(duration*GlobalLogic.VALUE_1000), pkEntity.GetPosition(), pkEntity.savedPosition, ignoreBlock));
		}
        else if (actionType == BeActionType.MoveTo)
        {
            VInt3 tmp = deltaPos;
            tmp.x *= pkEntity._getFaceCoff();

            pkEntity.actionManager.RunAction(BeMoveTo.Create(pkEntity, IntMath.Float2Int(duration * GlobalLogic.VALUE_1000), pkEntity.GetPosition(), tmp, ignoreBlock));
        }
        else if (actionType == BeActionType.ScaleTo)
        {
            pkEntity.actionManager.RunAction(BeScaleTo.Create(pkEntity, IntMath.Float2Int(duration * GlobalLogic.VALUE_1000), pkEntity.GetScale(), (VInt)deltaScale));
        }
	}
}
public class BDAddBuffInfoOrBuff : BDEventBase
{
    int buffTime;
    int buffID;
    List<int> buffInfoList;
    bool phaseDelete;
    bool finishDeleteAll;

    int level;
    bool levelBySkill;

    public BDAddBuffInfoOrBuff(int bid,List<int> bufInfoList,int d,bool phaseDelete,  bool finishDeleteAll,int lv,bool lvBySkill)
    {
        this.buffID = bid;
        this.buffInfoList = bufInfoList;
        this.buffTime = d;
        this.phaseDelete = phaseDelete;
        this.finishDeleteAll = finishDeleteAll;
        this.level = lv;
        this.levelBySkill = lvBySkill;
    }

    public override void OnEvent(BeEntity pkEntity)
    {
        base.OnEvent(pkEntity);

        BeActor target = pkEntity as BeActor;
        if(target != null)
        {
            BeSkill curSkill = target.skillController.GetCurrentSkill();
            if (curSkill != null && levelBySkill)
            {
                level = curSkill.level;
            }
            List<BeBuff> buffList = new List<BeBuff>();
            if(buffID > 0)
                buffList.Add(target.buffController.TryAddBuff(buffID, buffTime, level));

            for(int i = 0; i < buffInfoList.Count; ++i)
            {
                if(buffInfoList[i] > 0)
                    buffList.Add(target.buffController.TryAddBuff(buffInfoList[i], null, false, null, level));
            }
            
            for(int i = 0; i < buffList.Count; ++i)
            {
                if (buffList[i] != null && phaseDelete) 
                {
                    target.buffController.AddPhaseDelete(buffList[i]);
                }

                if (buffList[i] != null && finishDeleteAll) 
                {
                    target.buffController.AddFinishDeleteAll(buffList[i]);
                }
            }
        }
    }

    public override void PreparePreload(BeActionFrameMgr frameMgr, SkillFileListCache fileCache, bool useCube = false)
    {
        base.PreparePreload(frameMgr, fileCache, useCube);

        for (int i = 0; i < buffInfoList.Count; ++i)
        {
            PreloadManager.PreloadBuffInfoID(buffInfoList[i], frameMgr, fileCache);
        }
    }
}

public class BDDoSummon : BDEventBase
{
    int summonID;
    int summonLevel;
    bool levelGrowBySkill;
    int summonNum;
    ProtoTable.EffectTable.eSummonPosType posType;
    List<int> posType2;
    bool isSameFace;

    public BDDoSummon(int sumID, int sumLv,bool growBySkill, int sumNum, int posType, List<int> posType2, bool isSameFace)
    {
        this.summonID = sumID;
        this.summonLevel = sumLv;
        this.levelGrowBySkill = growBySkill;
        this.summonNum = sumNum;
        this.posType = (ProtoTable.EffectTable.eSummonPosType)posType;
        this.posType2 = posType2;
        this.isSameFace = isSameFace;
    }

    public override void OnEvent(BeEntity pkEntity)
    {
        base.OnEvent(pkEntity);

        BeActor target = pkEntity as BeActor;
        if (target != null && summonID != 0 && summonNum != 0) 
        {
            int level = summonLevel;
            BeSkill curSkill = target.skillController.GetCurrentSkill();
            if (curSkill != null && levelGrowBySkill) 
                level = curSkill.level;
            target.DoSummon(summonID, level, posType, posType2, summonNum, 0, 0, 0, 0, false, 0, 0, null, SummonDisplayType.NONE, null, isSameFace);
        }
    }

    public override void PreparePreload(BeActionFrameMgr frameMgr, SkillFileListCache fileCache, bool useCube = false)
    {
        base.PreparePreload(frameMgr, fileCache, useCube);
        if (summonID > 0)
        {
            for (int i = 0; i < summonNum; i++)
            {
                PreloadManager.PreloadResID(summonID, frameMgr, fileCache);
            }
        }
    }
}
public class BDAddMechanism : BDEventBase
{
    int mId;
    int mDuration;
    int mLevel;
    bool mLevelBySkill;
    bool mPhaseDelete;
    bool mFinishDeleteAll;

    public BDAddMechanism(int id, int d, int lv, bool levelBySkill, bool phaseDelete, bool finishDeleteAll)
    {
        mId = id;
        mDuration = d;
        mLevel = lv;
        mLevelBySkill = levelBySkill;
        mPhaseDelete = phaseDelete;
        mFinishDeleteAll = finishDeleteAll;
    }

    public override void OnEvent(BeEntity pkEntity)
    {
        base.OnEvent(pkEntity);

        BeActor target = pkEntity as BeActor;
        if (target != null)
        {
            BeSkill curSkill = target.skillController.GetCurrentSkill();
            if (curSkill != null && mLevelBySkill)
            {
                mLevel = curSkill.level;
            }
            target.AddSkillMechanism(mId, mDuration, mLevel, mPhaseDelete, mFinishDeleteAll);
        }
    }

    public override void PreparePreload(BeActionFrameMgr frameMgr, SkillFileListCache fileCache, bool useCube = false)
    {
        base.PreparePreload(frameMgr, fileCache, useCube);
        if (mId > 0)
            PreloadManager.PreloadSpecialScripts(PreloadSpecialType.Mechanism, mId, frameMgr, fileCache);
    }
}

