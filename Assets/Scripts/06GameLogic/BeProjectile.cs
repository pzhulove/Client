using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using GameClient;
using System.ComponentModel;

public enum ProjectType
{
    BULLET = 0,
    SINGLE = 1,
    TIMING = 2
}

public enum TargetChooseType
{
    [Description("无")]
    NONE = 0,

    [Description("选择最近目标")]
    NEAREST,    //选择最近目标

    [Description("选择面朝的最近的目标")]
    SMART_NEAREST,  //选择面朝的最近的目标

    [Description("选择最远目标（弃用）")]
    FARTEST,    //该类型暂时不支持(弃用)

    [Description("全屏（弃用）")]
    FULL_SCREEN,    //该类型暂时不支持(弃用)

    [Description("定点抛物线（弃用）")]
    PARABOLA_TARGET_POS,    //定点抛物线

    [Description("定目标抛物线（弃用）")]
    PARABOLA_TARGET,    //定目标抛物线

    [Description("双鹰回旋效果")]
    BOOMERANGE, //双鹰回旋效果 只要用于实体表现

    [Description("引力（光炎剑效果）")]
    GRAVITATION,    //引力(光炎剑效果 到达目标附近时 绕目标旋转)

    [Description("曲线（弃用）")]
    CURVE,  //该类型暂时不支持(弃用)

    [Description("选择最大怨念的目标")]
    MAX_RESENTMENT, //最大的怨念值

    [Description("选择最近的目标并跟随")]
    FOLLOW_TARGET,  //选择最近目标 并且一直并且一直跟随目标 靠近目标时自己死亡

    [Description("追踪最近的目标位置")]
    CHASE_TARGET,  //选择最近目标 并且与目标保持相同的偏移位置
    
    [Description("旋转追踪目标位置")]
    ROTATE_CHASE_TARGET,  //朝向攻击目标的位置追踪而去,
                          //实体的追踪时长有限，需可以配置追踪多长时间，超过时间之后就会向着最后面朝的方向继续直线前进
						  //实体的转向速度、移动速度需要可以调整，会决定实体追踪的灵敏度
						  //实体预设不会有Z轴（高度）的位移，不论玩家高度为何，都只会朝着XY轴坐标的位置追踪而去，实体永远保持发射时设置的Z轴偏移值的高度不变
	[Description("反弹型")]
	REBOUND, 		//实体碰到墙壁（或者碰撞物）后进行反弹
}

//实体死亡类型
public enum ProjectileDeadType
{
    NORMAL = 0,
    HITGROUNDDEAD = 1,      //触地死亡
}

public class BeProjectilePoolImp : IObjectPool
{
	Dictionary<int, Queue<BeProjectile>> pooledObj = new Dictionary<int, Queue<BeProjectile>>();
	Dictionary<int, Queue<BeProjectile>> useCubePooledObj = new Dictionary<int, Queue<BeProjectile>>();


	int newCount = 0;
	int refCount = 0;

	#region poolInfo

	string poolKey = "BeProjectilePool";
	string poolName = "投射物池";

	int totalInst = 0;
	int remainInst = 0;

	public string GetPoolName ()
	{
		return poolName;
	}

	public string GetPoolInfo()
	{
		return string.Format ("{0}/{1}", remainInst, totalInst);
	}

	public string GetPoolDetailInfo ()
	{
		return "detailInfo";
	}
	#endregion

	public  void Init ()
	{
#if !SERVER_LOGIC 

		CPoolManager.GetInstance ().RegisterPool (poolKey, this);

 #endif

	}

	public BeProjectile GetProjectile(int iResID, int iCamp, int iID, bool useCube=false)
	{
		//refCount++;
		BeProjectile obj = null;

		Dictionary<int, Queue<BeProjectile>> pool = pooledObj;

		if (useCube)
			pool = useCubePooledObj;


		if (!pool.ContainsKey(iResID))
			pool.Add(iResID, new Queue<BeProjectile>());

		if (pool[iResID].Count > 0)
		{
			remainInst--;
			obj = pool[iResID].Dequeue ();
		}
			
		if (obj == null)
		{
			totalInst++;
			obj = new BeProjectile (iResID, iCamp, iID);
		}
			

		obj.InitReset (iResID, iCamp, iID);

		//Logger.LogErrorFormat ("BeProjectilePool get:{0} newcount:{1} refCount:{2}", pooledObj.Count, newCount, refCount);

		return obj;
	}

	public void PutProjectile(BeProjectile obj)
	{
        if (obj.m_pkGeActor == null) return;
        remainInst++;
		// pooledObj.Enqueue(obj);

		Dictionary<int, Queue<BeProjectile>> pool = pooledObj;

		if (obj.m_pkGeActor.GetUseCube())
			pool = useCubePooledObj;

		pool[obj.m_iResID].Enqueue(obj);

		//Logger.LogErrorFormat ("BeProjectilePool put:{0} refCount:{1}", pooledObj.Count, refCount);
	}

    public Dictionary<int, Queue<BeProjectile>>.ValueCollection GetPoolValues()
    {
        return pooledObj.Values;
    }

	protected void SubClear(Dictionary<int, Queue<BeProjectile>> pool)
	{
		if (pool == null)
			return;

		Dictionary<int, Queue<BeProjectile>>.Enumerator enumerator = pool.GetEnumerator();
		while(enumerator.MoveNext())
		{
			Queue<BeProjectile> queue = enumerator.Current.Value as Queue<BeProjectile>;
			while(queue.Count > 0)
			{
				BeProjectile projectile = queue.Dequeue();
				projectile.OnRemove(true);
			}

			queue.Clear();
		}

		pool.Clear();
	}

	public void Clear()
	{
		SubClear(pooledObj);
		SubClear(useCubePooledObj);

		remainInst = 0;
		totalInst = 0;
	}
}

public sealed class BeProjectileStateGraph : BeStatesGraph{
    public BeProjectile pkProjectile;
    public int m_iPreState;
    

    public  override void InitStatesGraph()
    {
        BeStates kBirthState = new BeStates(
            (int)ActionState.AS_BIRTH,
            (int)AStateTag.AST_NULLTAG,
            (BeStates state) =>
            {
                if (pkProjectile.HasAction(pkProjectile.GetActionNameByType(ActionType.ActionType_BIRTH)))
                {
                    pkProjectile.PlayAction(ActionType.ActionType_BIRTH);
                    SetCurrentStatesTimeout(pkProjectile.GetCurrentActionDuration());
                }
                else
                {
					SetCurrentStatesTimeout(GlobalLogic.VALUE_10);
                }
            },
            (BeStates pkState) =>
            {
                SwitchStates(new BeStateData((int)ActionState.AS_IDLE));
            }
            );

       //空闲状态
       BeStates kStandState = new BeStates(
            (int)ActionState.AS_IDLE,
            (int)AStateTag.AST_NULLTAG,
            (BeStates pkState) =>
            {
                pkProjectile.PlayAction(ActionType.ActionType_IDLE);
                if (pkProjectile.m_eType == ProjectType.BULLET ||
                    pkProjectile.m_eType == ProjectType.SINGLE)
                {
                    int time = pkProjectile.GetLifeTime();
                    bool force = time > 0;
                    SetCurrentStatesTimeout(time, force);

                    
                }
            },
            (BeStates pkState) =>
            {
                //SwitchStates(new BeStateData((int)ActionState.AS_DEAD));
				pkProjectile.DoDie();
            }
            );

        SGAddEventHandler2States(kStandState,
            new BeEventsHandler(
                (int)EventCommand.EVENT_COMMAND_TOUCHGROUND,
                (BeStates pkState) =>
                {
                    if (pkProjectile.m_eType == ProjectType.BULLET ||
                        pkProjectile.m_eType == ProjectType.SINGLE)
                    {
                        if (pkProjectile.delayDead > 0)
                        {
                            if (!pkProjectile.markDead)
                            {
                                if (pkProjectile.hitGroundClick)
                                {
                                    pkProjectile.ClearMoveSpeed((int)SpeedCear.SPEEDCEAR_Y | (int)SpeedCear.SPEEDCEAR_Z);
                                    //pkProjectile.m_fMoveXSpeed /= 3;
                                    pkProjectile.SetMoveSpeedZ(3);
                                }
                                else
                                {
                                    //pkProjectile.m_pkGeActor.Pause();
                                    pkProjectile.ClearMoveSpeed();
                                }
                                pkProjectile.markDead = true;
                            }
                            else
                            {
                                pkProjectile.ClearMoveSpeed();
                                pkProjectile.m_pkGeActor.Pause();
                                return;
                            }

                            pkProjectile.projectileDeadType = ProjectileDeadType.HITGROUNDDEAD;
                            pkProjectile.delayCaller.DelayCall(pkProjectile.delayDead, () =>
                            {
                                pkProjectile.DoDie();
                                //SwitchStates(new BeStateData((int)ActionState.AS_DEAD));
                            });
                        }
                        else
                        {
                            pkProjectile.projectileDeadType = ProjectileDeadType.HITGROUNDDEAD;
                            pkProjectile.DoDie();
                            //SwitchStates(new BeStateData((int)ActionState.AS_DEAD));
                        }
                    }
                        
                }
                )
            );

        AddStates2Graph(kStandState);

        //死亡
        BeStates kDeadState = new BeStates(
            (int)ActionState.AS_DEAD,
            (int)AStateTag.AST_BUSY,
            (BeStates pkState) =>
            {
                pkProjectile.ClearMoveSpeed();

				pkProjectile.OnDead();

                pkProjectile.logicTrail = null;

				if (pkProjectile.playExtDead)
				{
					if (pkProjectile.HasAction(Global.ACTION_EXPDEAD))
					{
						pkProjectile.PlayAction(Global.ACTION_EXPDEAD);
						SetCurrentStatesTimeout(pkProjectile.GetCurrentActionDuration());
					}
					else
                    {
						pkProjectile.m_iEntityLifeState = (int)EntityLifeState.ELS_CANREMOVE;
					}
				}
                else if(pkProjectile.projectileDeadType == ProjectileDeadType.HITGROUNDDEAD && pkProjectile.HasAction(Global.ACTION_HITGROUNDDEAD))           //实体触地死亡播放的动作
                {
                    pkProjectile.PlayAction(Global.ACTION_HITGROUNDDEAD);
                    SetCurrentStatesTimeout(pkProjectile.GetCurrentActionDuration());
                }
				else
                {
					if (pkProjectile.HasAction(ActionType.ActionType_DEAD))
					{
						pkProjectile.PlayAction(ActionType.ActionType_DEAD);
						//SetCurrentStatesTimeout(20);
						SetCurrentStatesTimeout(pkProjectile.GetCurrentActionDuration());
					}
					else
                    { 
						pkProjectile.m_iEntityLifeState = (int)EntityLifeState.ELS_CANREMOVE;
					}
				}
            },
            (BeStates pkState) =>
            {
                pkProjectile.m_iEntityLifeState = (int)EntityLifeState.ELS_CANREMOVE;
            }
            );

        AddStates2Graph(kDeadState);
    }
}


public class BeProjectile : BeEntity {

    public int m_fLifes;
    public ProjectType m_eType;
    public int hurtType;
    public float forcex;
    public float forcey;
    public float hurtTime;
    public ShockInfo targetShockInfo;
    public ShockInfo sceneShockInfo;
    public bool isHitFloat = false;//是否击中空中目标给向上的力
    public float hitFloatForceY = 0;
    public bool isHurtPause = false;
    public float hurtPauseIime = 0;
	public CrypticInt32 hurtID;
    public List<int> attachHurts = new List<int>();
	public List<int> hurtAddBuffs = new List<int>();//在攻击时增加伤害

    public VInt3 originPos;

    public VInt distance = 0;
    public bool hitThrough = false;
	public CrypticInt32 totoalHitCount = 1;
    public int lifeTime = 0;
    public bool needSetFace = true;

    public int triggerSkillLevel = 0;//所属的技能等级

    public VRate hitThroughFactor = VRate.zero;//弹道穿透率
    public int delayDead = 0;
    public bool markDead = false;

    public bool hitGroundClick = false;
	public bool attackCountExceedPlayExtDead = false;
	public bool playExtDead = false;

	public VInt3 center;
    public bool isRotation = false;
    public int rotateSpeed;
    public int moveSpeed;

	public ProtoTable.ObjectTable data;

	public TargetChooseType targetChooseType;

    //用于轨迹飞行
    public LogicTrail logicTrail = null;

    //用于绕定点旋转的特效
	int degree;
	int radius;
    public ProjectileDeadType projectileDeadType = ProjectileDeadType.NORMAL;           //死亡类型

    public bool onCollideDie = false;
    public bool onXInBlockDie = false;

    public bool changForceBehindOther = false;

    public bool isAngleWithEffect = false;  //特效角度随着速度方向变化

    int ConverK2W(int r)
	{
		long v = r;
		v = v * 10000 / IntMath.kIntDen;
		return (int)v;
	}

	int ConverW2K(int r)
	{
		long v = r;
		v = v * IntMath.kIntDen / 10000;
		return (int)v;
	}

	public VFactor DegreeToRadian(int degree)
	{
		//return PI /180f * degree;
		return VFactor.pi / 180 * degree / 100;
	}
 
    public BeProjectile(int iResID, int iCamp, int iID) : base(iResID, iCamp, iID)
    {
       
    }

	public sealed override void InitReset(int iResID, int iCamp, int iID)
	{
		base.InitReset (iResID, iCamp , iID);

		m_fLifes = 0;
		m_eType = ProjectType.BULLET;
		hurtType = 0;
		forcex = 0;
		forcey = 0;
		hurtTime = 0;
		targetShockInfo = new ShockInfo ();
		sceneShockInfo = new ShockInfo ();
		isHitFloat = false;
		hitFloatForceY = 0f;
		isHurtPause = false;
		hurtPauseIime = 0;
		hurtID = 0;
		attachHurts.Clear ();
		hurtAddBuffs.Clear();
		originPos = VInt3.zero;
		distance = 0;
		hitThrough = false;
		totoalHitCount = 1;
		lifeTime = 0;
		needSetFace = true;
		triggerSkillLevel = 0;
		hitThroughFactor = VRate.zero;
		delayDead = 0;
		markDead = false;
		hitGroundClick = false;
		center = VInt3.zero;
		isRotation = false;
		rotateSpeed = 0;
		moveSpeed = 0;
		data = null;
		targetChooseType = TargetChooseType.NONE;
        logicTrail = null;
		//rotateDummy = null;
		degree = 0;
		radius = 0;

		m_bCanBeAttacked = false;
		m_eType = ProjectType.BULLET;

		//默认重力加速度为0
		moveZAcc = 0;

		//设置默认的伤害类型
		hurtType = 0;
		forcex = 4;
		forcey = 0;

		hasHP = false;
		attackCountExceedPlayExtDead = false;
		playExtDead = false;
        projectileDeadType = ProjectileDeadType.NORMAL;

        data = TableManager.GetInstance().GetTableItem<ProtoTable.ObjectTable>(m_iResID);

        onCollideDie = false;
        onXInBlockDie = false;

        changForceBehindOther = false;
    }

	public virtual void Create(float fDelaytime = 0.0f, bool useCube=false)
    {
      //  Logger.LogErrorFormat("Create Proj {0}",m_iResID);
      //  Logger.LogErrorFormat("create projectile resid:{0}", m_iResID);

		BeStatesGraph sg = null;
		if (m_pkStateGraph == null) {
			BeProjectileStateGraph pkSG = new BeProjectileStateGraph ();
			pkSG.InitStatesGraph ();
			pkSG.pkProjectile = this;
			pkSG.m_pkEntity = this;
			sg = pkSG;
		} else
		{
			sg = m_pkStateGraph;
		}
		base.Create(sg, (int)ActionState.AS_IDLE, false, false, useCube);
		InitTableData();
    }


    public sealed override bool IsAttackAdd2Statistics()
    {
        BeActor owner = GetOwner() as BeActor;
        if (null == owner)
        {
            Logger.LogProcessFormat("[战斗] 没有owner");
            return false;
        }

        return owner.IsAttackAdd2Statistics();
    }

	public void InitTableData()
	{
		if (data != null)
		{
			int duration = TableManager.GetValueFromUnionCell(data.Duration, triggerSkillLevel);
			if (duration > 0)
				m_fLifes = duration;

			bool genRune = data.GenRune > 0;
			if (genRune && owner != null)
			{
				owner.TriggerEventNew(BeEventType.onAddRune);
			}
		}

		CreateShadow();

	}

    public bool IsGenRune()
    {
        if (data != null)
        {
            return data.GenRune > 0;
        }
        return false;
    }
		
	void CreateShadow()
	{
		#if !LOGIC_SERVER
		GeSimpleShadowManager.instance.RemoveShadowObject(m_pkGeActor.renderObject);
		
		if (data != null && (data.ShadowScaleX > 0 || data.ShadowScaleZ > 0))
		{

			Vector3 scale = Vector3.one;
			Vector4 entityPlane = GeSceneEx.EntityPlane;
			m_pkGeActor.SetEntityPlane(entityPlane);
			if (data.ShadowScaleX > 0)
				scale.x = data.ShadowScaleX / 1000f;
			if (data.ShadowScaleZ > 0)
				scale.z = data.ShadowScaleZ / 1000f;
            m_pkGeActor.AddSimpleShadow(scale);
            //GeSimpleShadowManager.instance.AddShadowObject(m_pkGeActor.renderObject, entityPlane, scale);
		}
		#endif
	}

	public VFactor GetProjectileScale()
	{
		VFactor scale = VFactor.one;

		if (data != null && triggerSkillLevel > 1)
		{
			int value = TableManager.GetValueFromUnionCell(BattleMain.IsModePvP(battleType)?data.PVPScale:data.Scale, triggerSkillLevel);
			if (value > 0)
			{
				//scale = value / 1000f;
				//scale *= VFactor(value,1000);
				 scale = new VFactor(value,1000);
				//a	Logger.LogErrorFormat("skill level:{0} scale:{1}", triggerSkillLevel, scale);
			}
		}

		return scale;
	}

	public VFactor GetProjectileZDimScale()
	{
		//float scale = 1.0f;
		VFactor scale = VFactor.one;
		if (data != null && triggerSkillLevel > 1)
		{
			int value = TableManager.GetValueFromUnionCell(BattleMain.IsModePvP(battleType)?data.PVPZscale:data.Zscale, triggerSkillLevel);
			if (value > 0)
			{
				//scale = value / 1000f;
				scale = new VFactor(value,1000);
			}
		}

		return scale;
	}

    public sealed override void _updatePosition(int iDeltime)
    {
        base._updatePosition(iDeltime);
    }

    public sealed override bool _isCmdMoveForbiden()
    {
        return true;
    }

    public sealed override void _updateGraphicActor(bool force = false)
    {
        if (m_pkGeActor != null && (m_bGraphicPositionDirty || force))
        {
 #if !SERVER_LOGIC 

           //Logger.Log(string.Format("update postion:({0},{1},{2})", m_kPosition.x, m_kPosition.y, m_kPosition.z));
			var pos = GetPosition();
			m_pkGeActor.SetPosition(pos.vector3);
            if (needSetFace)
                m_pkGeActor.SetFaceLeft(GetFace());
            else
                m_pkGeActor.SetFaceLeft(false);
            m_pkGeActor.SetScale(m_fScale.scalar);

 #endif

            m_bGraphicPositionDirty = false;
        }
    }

	public sealed override void _onHurtEntity(BeEntity pkEntity, VInt3 hitPos, int hurtid = 0, uint attackProcessId = 0)
    {
        if (pkEntity == null)
            return;

        if (hurtID != 0)
            hurtid = hurtID;

        if (totoalHitCount <= 0)
            return;

		var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hurtid);

		if (hurtData != null)
			AddSkillBuffBeforeDealHurt(hurtData.SkillID);
			
		base._onHurtEntity(pkEntity, hitPos, hurtid, AttackProcessId);

		if (hurtData != null)
			RemoveSkillBuffAfterDealHurt(hurtData.SkillID);
    }

    /// <summary>
    /// 实体附加伤害
    /// </summary>
    protected override void OnAttachHurt(BeEntity pkEntity, EffectTable hurtData)
    {
        base.OnAttachHurt(pkEntity, hurtData);
        if (hurtData == null)
            return;
        if (hurtData != null && hurtData.HitGrab && !pkEntity.IsGrabed())
            return;
        DoAttachHurt(pkEntity);
    }

    public void AddSkillBuff(int buffID)
	{
		hurtAddBuffs.Add(buffID);
	}

	protected void AddSkillBuffBeforeDealHurt(int skillID)
	{
		if (skillID <= 0)
			return;

        var list = new List<int>
        {
            skillID
        };

		BeActor owner = GetOwner() as BeActor;
		if (owner != null && !owner.IsDeadOrRemoved())
		{
			for(int i=0; i<hurtAddBuffs.Count; ++i)
			{
				owner.buffController.TryAddBuff(hurtAddBuffs[i], 1, triggerSkillLevel, 1000, 0, false, list);
			}
		}
	}

	protected void RemoveSkillBuffAfterDealHurt(int skillID)
	{
		if (skillID <= 0)
			return;

		BeActor owner = GetOwner() as BeActor;
		if (owner != null && !owner.IsDeadOrRemoved())
		{
			for(int i=0; i<hurtAddBuffs.Count; ++i)
			{
				owner.buffController.RemoveBuff(hurtAddBuffs[i]);
			}
		}
	}



    public sealed override void OnDealHit(BeEntity pkEntity)
    {
		#if !LOGIC_SERVER
        //目标shock
        if (targetShockInfo.shockTime > 0)
        {
            float rx = targetShockInfo.shockRangeX;
            float ry = targetShockInfo.shockRangeY;

            if (GetFace())
            {
                rx *= -1;
            }

            pkEntity._addShock(targetShockInfo.shockTime, targetShockInfo.shockSpeed, rx, ry);
        }
        
        BeActor actor = GetTopOwner(this) as BeActor;
        if (actor != null && actor.isLocalActor)
        {
			if (sceneShockInfo.shockTime > 0)
            {
                currentBeScene.currentGeScene.GetCamera().PlayShockEffect(sceneShockInfo.shockTime, sceneShockInfo.shockSpeed, sceneShockInfo.shockRangeX, sceneShockInfo.shockRangeY, 2, actor.IsCurSkillOpenShock());
            }
        }

		#endif
    }
    public override bool IsCurSkillOpenShock()
    {
        if (owner != null)
        {
            return owner.IsCurSkillOpenShock();
        }
        return true;
    }

	public sealed override void ShowMissEffect(Vec3 Pos)
	{
#if !SERVER_LOGIC 

		BeActor owner = GetOwner() as BeActor;

		if (owner != null && owner.isMainActor && !owner.IsDeadOrRemoved())
		{
			currentBeScene.currentGeScene.CreateEffect(9, Pos, GetFace());
		}

 #endif

	}
	public sealed override void OnBeforeGetDamage (BeEntity target, AttackResult result, bool isBackHit, int hurtID)
	{
		if (GetOwner() == target)
			return;

		BeActor owner = GetOwner() as BeActor;
		if (owner != null && !owner.IsDeadOrRemoved())
		{
			owner.DealBeforeGetDamage(target, result, hurtID, isBackHit, m_iResID);
		}
	}


	public sealed override void onHitEntity(BeEntity pkEntity, VInt3 pos, int hurtID=0, AttackResult result = AttackResult.MISS,int finalDamage = 0)
    {
        if (m_eType == ProjectType.BULLET ||
            m_eType == ProjectType.SINGLE)
        {
            totoalHitCount--;

            //处理触发效果里附加的穿透率
            VRate tmpHitThrough = hitThroughFactor;
            if (hurtID > 0)
            {
               var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hurtID);
                if (hurtData != null)
                {
                    int skillLevel = 1;
                    BeEntityData data = GetEntityData();
                    if (data != null)
                    {
                        skillLevel = data.GetSkillLevel(hurtData.SkillID);
                    }

                    VRate effectHitThrough = TableManager.GetValueFromUnionCell(hurtData.HitThroughRate, skillLevel);

                    //百分比增长触发效果表中的穿刺率
                    /*VRate[] hitThroughArray = new VRate[1];
                    hitThroughArray[0] = effectHitThrough;
                    owner.TriggerEvent(BeEventType.onChangeHitThrough, new object[] {hurtID, hitThroughArray });
                    tmpHitThrough += hitThroughArray[0];*/
                    var eventData = owner.TriggerEventNew(BeEventType.onChangeHitThrough, new EventParam(){m_Int = hurtID, m_Rate = effectHitThrough});
                    tmpHitThrough += eventData.m_Rate;
                }
            }


            if (tmpHitThrough > 1)
            {
                //穿透不成功
				if (FrameRandom.Range1000() >  tmpHitThrough)
                {
                    totoalHitCount = 0;
                }
                else
                {
                    totoalHitCount = Mathf.Max(1, totoalHitCount);
                }

            }
        }

		if (result != AttackResult.MISS  && owner != null)
		{
            if (result == AttackResult.CRITICAL)
                owner.TriggerEventNew(BeEventType.onHitCritical, new EventParam() { m_Vint3 = pos });
				//owner.TriggerEvent(BeEventType.onHitCritical, new object[] { pos });

			int skillID = 0;
			var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hurtID);
			if (hurtData != null)
			{
				skillID = hurtData.SkillID;
			}

            owner.TriggerEventNew(BeEventType.onHitOther,new EventParam() { m_Obj = pkEntity, m_Int = hurtID, m_Int2 = skillID, m_Vint3 = pos, m_Int3 = 0, m_Int4 = finalDamage,m_Obj2 = this });
			//owner.TriggerEvent(BeEventType.onHitOther, new object[] {pkEntity,  hurtID, skillID, pos,0,finalDamage});

            if (owner != this)
            {
                var actor = owner as BeActor;
                if (actor != null)
                {
                    actor.onHitEntity(pkEntity, pos, hurtID, result,finalDamage);
                }
            }

            owner.TriggerEventNew(BeEventType.onHitOtherAfterHurt,new EventParam() {m_Obj = pkEntity,m_Int = hurtID, m_Int2 = (int)result, m_Int3 = (int) AttackProcessId, m_Obj2 = hurtData});
            //owner.TriggerEvent(BeEventType.onHitOtherAfterHurt, new object[] { pkEntity, hurtID, result});

        }
            
    }

    public void DoAttachHurt(BeEntity target)
    {
        for(int i=0; i<attachHurts.Count; ++i)
        {
            if (attachHurts[i] > 0)
            {
                int hurtid = attachHurts[i];
                if (!target.stateController.CanBeHit())
                    continue;

                var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hurtid);
                if (hurtData == null)
                {
                    if (m_cpkCurEntityActionInfo != null)
                    {
                        Logger.LogWarning(string.Format("{0} 触发效果表 没有ID为 {1} 的条目", m_cpkCurEntityActionInfo.actionName, hurtid));
                    }
                    continue;
                }

                //可破坏物等的特殊处理
                if ((target as BeObject) != null)
                    continue;
                DoAttackTo(target, hurtid,false,true);
            }
        }
    }

	public sealed override void ShowHitEffect(Vec3 Pos, BeEntity target, int hurtID)
    {
		#if !LOGIC_SERVER
        string effectPath;

        if (m_cpkCurEntityActionInfo != null && m_cpkCurEntityActionInfo.hitEffectAsset.IsValid())
            effectPath = m_cpkCurEntityActionInfo.hitEffectAsset.m_AssetPath;
        else
            effectPath = Global.Settings.defaultProjectileHitEffect;

        //var param = DataStructPool.EventParamPool.Get();
        //param.m_Int = hurtID; param.m_String = effectPath;
        //TriggerEventNew(BeEventType.onChangeHitEffect, param);
        //effectPath = param.m_String;
        //DataStructPool.EventParamPool.Release(param);

        var param = TriggerEventNew(BeEventType.onChangeHitEffect, new EventParam(){ m_Int = hurtID, m_String = effectPath});
        effectPath = param.m_String;

        var effect = CreateHitEffect(effectPath, Pos, m_cpkCurEntityActionInfo.hitEffectInfoTableId, target);//currentBeScene.currentGeScene.CreateEffect(asset, effectFrames, 0, Pos,1,1,false,GetFace());
        if (target != null && effect != null)
		{
			target.currentHitEffectNum++;

			int duration = System.Math.Max(IntMath.Float2Int(effect.GetTimeLen()), 100);
            target.AddDelayCallDealEffectNum(duration);
            //target.delayCaller.DelayCall (duration, () => {
            //	target.currentHitEffectNum--;
            //});
        }
		#endif
    }

    public void SetType(ProjectType t, int value)
    {
        m_eType = t;
        m_fLifes = value;
    }

    public int GetLifeTime()
    {
        return m_fLifes;
    }

    public sealed override bool Update(int iDeltaTime)
    {
        //修改实体跳帧的bug
        int actorTimeAcc = iDeltaTime;
        int fixDelta = 16;
        while (actorTimeAcc > 0)
        {
            var tmpDelta = Mathf.Min(fixDelta, actorTimeAcc);
            if (base.Update(tmpDelta))
            {
                CheckDistance();
                CheckLifeTime(timeAcc);
                CheckAttackCount();

                if (isRotation)
                    UpdateRotation();
            }
            actorTimeAcc -= fixDelta;
        }

        if (logicTrail != null)
        {
            SetPosition(logicTrail.currentPos);
        }

        return true;
    }

    public sealed override bool UpdateGraphic(int iDeltaTime)
    {
        if (isAngleWithEffect)
        {
            InitLocalRotation();
        }
        return base.UpdateGraphic(iDeltaTime);
    }

    public void InitLocalRotation()
    {
		#if !LOGIC_SERVER
			if(m_pkGeActor == null)
			{
				return;
			}
			
			if(m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Actor) == null)
			{
				return;
			}
			

			//TODO
			
			if (targetChooseType == TargetChooseType.ROTATE_CHASE_TARGET || targetChooseType == TargetChooseType.REBOUND)
			{
				float sx = m_fMoveXSpeed.scalar;
				float sy = moveYSpeed.scalar;
				Vector2 speedv = new Vector2(sx, sy);
				float dangle = 0;
				if (speedv.y != 0)
				{
					dangle = Vector2.Angle(speedv, new Vector2(1, 0));
				}
				
				if (speedv.y < 0)
				{
					dangle = -dangle;
				}

				if (speedv.x == 0 && speedv.y == 0)
					dangle = 0;
				else
					m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Actor).transform.localRotation = Quaternion.AngleAxis(dangle, Vector3.down);
				if(dangle == 0 && ((GetFace() && speedv.x > 0) || (!GetFace() && speedv.x < 0)))
				{
					m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Actor).transform.localRotation = Quaternion.AngleAxis(180, Vector3.down);
				}
			}
			else
			{
				float sx = m_fMoveXSpeed.scalar;
				float sy = moveZSpeed.scalar;
				Vector2 speedv = new Vector2(sx, sy);
				float dangle = 0;
				if (speedv.y != 0)
				{
					dangle = Vector2.Angle(speedv, new Vector2(1, 0));
				}
				
				if (GetFace() && dangle != 0)
					dangle = 180 - dangle;

				if (speedv.y < 0)
				{
					dangle = -dangle;
				}

				if (speedv.x == 0 && speedv.y == 0)
					dangle = 0;
	
				m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Actor).transform.localRotation = Quaternion.AngleAxis(dangle, Vector3.forward);
			}
		#endif
    }
    public void SetTargetShockInfo(ShockInfo info)
    {
        targetShockInfo = info;
    }

    public void SetSceneShockInfo(ShockInfo info)
    {
        sceneShockInfo = info;
    }

    public void DoDie()
    {
        if (sgGetCurrentState() == (int)ActionState.AS_DEAD)
            return;

        //TriggerEvent(BeEventType.onDead, new object[] { this });
        TriggerEventNew(BeEventType.onDead, new EventParam() { m_Bool = true, m_Bool2 = false, m_Obj = this });
        if (!sgStarted)
        {
            if(m_pkStateGraph != null)
            {
                if (!m_pkStateGraph.Start((int)ActionState.AS_DEAD))
                {
                    sgSwitchStates(new BeStateData((int)ActionState.AS_DEAD));
                    return;
                }
            }
        }
        else
        {
            sgSwitchStates(new BeStateData((int)ActionState.AS_DEAD));
        }
        ClearPhaseDeleteAudio();
        sgStarted = true;
    }

    public void RecordOriginPosition()
    {
        originPos = GetPosition();
    }

    private void CheckDistance()
    {
        if (distance > 0 && Mathf.Abs(GetPosition().x - originPos.x) >= distance)
        {
            DoDie();
        }
    }

    private void CheckLifeTime(int time)
    {
        if (lifeTime > 0 && time >= lifeTime)
        {
            //Logger.Log("life time is up");
            DoDie();
        }
    }

    private void CheckAttackCount()
    {
        if (totoalHitCount <= 0)
        {
			if (attackCountExceedPlayExtDead)
				playExtDead = true;
            DoDie();
        }
    }

	public void InitRotation(int initDegree)
    {
		this.center = GetOwner().GetPosition();
        isRotation = true;
		VInt2 centerXY = new VInt2(this.center.x, this.center.y);
		VInt2 positionXY = new VInt2(GetPosition().x, GetPosition().y);
		radius = ConverK2W((centerXY - positionXY).magnitude); 	 
		degree = initDegree * 100;
    }

	public new void SetPosition(VInt3 rkPos, bool immediate=false, bool showLog=true)
	{
		if (data != null)
		{
			if (data.IsTouchGround > 0)
				rkPos.z = VInt.Float2VIntValue(0.01f);
		}
		base.SetPosition(rkPos, immediate, showLog);
	}

	public int FormatDegree(int degree)
	{
		while(degree < 0)
			degree += 36000;
		while(degree > 36000)
			degree -= 36000;

		return degree;
	}

    public void UpdateRotation()
    {
		if (!isRotation)
			return;

		var pos = GetPosition();
		degree = FormatDegree(degree);
		
		VInt3 initPos = new VInt3(1.0f,0,0);
		VFactor radian = DegreeToRadian(degree);
		VInt3 newPos = initPos.RotateZ(ref radian);
		newPos.NormalizeTo(ConverW2K(radius));
		newPos.x = center.x + newPos.x;
		newPos.y = center.y + newPos.y;
		newPos.z = pos.z;

		SetPosition(newPos);

        //degree += (int)Global.Settings.startVel.x;//100;//Global.Settings.startVel.x;
        //radius += (int)Global.Settings.startVel.y;//10;//Global.Settings.startVel.y;

        degree += rotateSpeed;
        radius += moveSpeed;

        SetFace(false);
    }

	public sealed override void OnDead ()
	{
        if (logicTrail != null)
        {
            logicTrail.Remove();
            logicTrail = null;
        }
	}

	public sealed override bool TryAddBuff(ProtoTable.EffectTable hurtData, BeEntity target=null, int duration=0, bool useBuffAni = true, bool finishDelete=false, bool finishDeleteAll = false)
	{
		if (owner != null/* && !owner.IsDead()*/)
		{
			return owner.TryAddBuff(hurtData, target);
		}

		return false;
	}

	public sealed override bool TryAddEntity(ProtoTable.EffectTable hurtData, VInt3 pos, int triggeredLevel = 1, uint attackProcessId = 0)
    {
		if (owner != null  && !owner.IsDead())
        {
			return owner.TryAddEntity(hurtData, pos, this.triggerSkillLevel, attackProcessId);
        }

        return false;
    }

	public sealed override bool TrySummon(ProtoTable.EffectTable hurtData)
	{
		bool ret = false;
		if (owner != null && !owner.IsDead())
		{
			ret = owner.TrySummon (hurtData);
			if (ret)
			{
				List<BeActor> summons = GamePool.ListPool<BeActor>.Get();

				owner.CurrentBeScene.GetSummonBySummoner(summons, owner as BeActor, true);
				for(int i=0; i<summons.Count; ++i)
				{
					if (summons[i].GetEntityData().MonsterIDEqual(hurtData.SummonID)) {
						summons[i].SetPosition(GetPosition());
						summons[i].SetFace(GetFace(), false, true);
					}

				}
				GamePool.ListPool<BeActor>.Release(summons);
			}
		}
		return ret;
	}

    //投射物的攻击判定做特殊处理
    public sealed override bool _canAttackedEntity(BeEntity pkEntity, int hitType = 1)
    {
        bool SuperFlag = base._canAttackedEntity(pkEntity, hitType);
        if (pkEntity.GetOwner() != null && !pkEntity.GetOwner().IsDeadOrRemoved())
        {
            //攻击者处于混乱状态下
          if(pkEntity.GetOwner().stateController.IsChaosState())
            {
                if (SuperFlag)
                {
                    //投射物不能伤害自己的释放者
                    if (owner == pkEntity.GetOwner())
                    {
                        return false;
                    }
                }
            }
        }
        return SuperFlag;
    }

    protected override void OnCollide()
    {
        if (onCollideDie)
        {
            DoDie();
        }
    }

    protected override void OnXInBlock()
    {
        if (onXInBlockDie)
        {
            DoDie();
        }
    }

    protected override EventParam ChangeXForce(EventParam xForce, BeEntity target, BeEntity bullet, bool face)
    {
        EventParam param = xForce;
        if (changForceBehindOther)
        {
            if (face)
            {
                if (bullet.GetPosition().x > target.GetPosition().x)
                {
                    param.m_Vint3.y = -xForce.m_Vint3.y;
                }
            }
            else
            {
                if (bullet.GetPosition().x < target.GetPosition().x)
                {
                    param.m_Vint3.y = -xForce.m_Vint3.y;
                }
            }
        }
        return xForce;
    }
}
