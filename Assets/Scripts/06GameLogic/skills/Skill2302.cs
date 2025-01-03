using System.Collections.Generic;
using UnityEngine;
using GameClient;


/// <summary>
/// 用于炫纹发射逻辑,释放假子弹，不会发生碰撞，再到目标点后发生爆炸
/// </summary>
public class LaunchChaser
{
    private BeActor mOwner = null;
    private int level = 1;
    private BeEvent.BeEventHandleNew mHandle;
    private BeActor mTarget = null;
    //private List<BeEventHandle> mEventHandleList = new List<BeEventHandle>();
    private Mechanism2072 mChaserMgr = null;

    private Dictionary<int, Mechanism2072.ChaserData> mProjectileChaserData =
        new Dictionary<int, Mechanism2072.ChaserData>();

    private readonly int mChaserPathResID = 63748;
    private int mTrailAccSpeed = 200;
    
    // 爆炸实体表[炫纹大小][炫纹种类]
    private readonly int[,] mChaserBoomResID =
    {
        {63733, 63735, 63741, 63739, 63737}, 
        {63734, 63736, 63742, 63740, 63738}
    };

    public void Init(BeActor owner, int level, int accSpeed)
    {
        if(owner == null)
            return;

        RemoveHandle();
        this.level = level;

        if (accSpeed > 0)
        {
            mTrailAccSpeed = accSpeed;
        }
        
        mOwner = owner;
        //mHandle = owner.RegisterEvent(BeEventType.onHitOtherAfterHurt, (object[] args) =>
        //{

        //});
        mHandle = owner.RegisterEventNew(BeEventType.onHitOtherAfterHurt, _OnHitOtherAfterHurt);

        //mEventHandleList.Clear();
        mProjectileChaserData.Clear();
    }

    private void _OnHitOtherAfterHurt(BeEvent.BeEventParam param)
    {
        BeActor actor = param.m_Obj as BeActor;
        if (actor == null)
            return;

        if (actor.IsDead())
            return;

        mTarget = actor;
    }

    private Mechanism2072 GetChaserMgr(BeActor owner)
    {
        var baseMech = owner.GetMechanism(Mechanism2072.ChaserMgrID);
        if (baseMech == null)
            return null;
        
        var mechanism = baseMech as Mechanism2072;
        return mechanism;
    }
    public bool CanLaunch()
    {
        if (mTarget == null || mTarget.IsDead())
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 炫纹发射
    /// </summary>
    /// <returns>是否发射成功</returns>
    public bool Launch()
    {
        if (!CanLaunch())
            return false;

        if (mChaserMgr == null)
        {
            mChaserMgr = GetChaserMgr(mOwner);
            if (mChaserMgr == null)
                return false;
        }

        var chaserList = GamePool.ListPool<Mechanism2072.ChaserData>.Get();
        mChaserMgr.LaunchChaser(1, chaserList);

        if (chaserList.Count <= 0)
        {
            GamePool.ListPool<Mechanism2072.ChaserData>.Release(chaserList);
            return false;
        }
#if !LOGIC_SERVER
        CreateLightEffect(chaserList[0]);
        PlayAudio((int) chaserList[0].type);
        var effect = chaserList[0].effect;
        if (effect != null && mOwner != null && mOwner.m_pkGeActor != null)
            mOwner.m_pkGeActor.DestroyEffect(chaserList[0].effect);
#endif
        var projectile = GetProjectile(chaserList[0]);
        if (projectile == null)
        {
            GamePool.ListPool<Mechanism2072.ChaserData>.Release(chaserList);
            return false;
        }
        
        // 这里不存handle因为，handle无处remove。反而一直增加内存。handle生命周期伴随projectile
        projectile.RegisterEventNew(BeEventType.onDead, OnChaserFlyComplete);
        //mEventHandleList.Add(handle);
        
        GamePool.ListPool<Mechanism2072.ChaserData>.Release(chaserList);
        return true;
    }
    
#if !LOGIC_SERVER
    // [炫纹种类]的点亮特效
    private readonly string[] mChaserLightEffect =
    {
        "Effects/Hero_Lifa/Eff_Lifa_XWRH/Prefab/Eff_Lifa_XWRH_wu_fashe",
        "Effects/Hero_Lifa/Eff_Lifa_XWRH/Prefab/Eff_Lifa_XWRH_guang_fashe",
        "Effects/Hero_Lifa/Eff_Lifa_XWRH/Prefab/Eff_Lifa_XWRH_fire_fashe",
        "Effects/Hero_Lifa/Eff_Lifa_XWRH/Prefab/Eff_Lifa_XWRH_ice_fashe",
        "Effects/Hero_Lifa/Eff_Lifa_XWRH/Prefab/Eff_Lifa_XWRH_an_fashe"
    };

    private void CreateLightEffect(Mechanism2072.ChaserData data)
    {
        int index = (int) data.type;
        if(mChaserLightEffect.Length <= index || index < 0)
            return;
        if(mOwner == null || mOwner.m_pkGeActor == null)
            return;
        string path = mChaserLightEffect[index];
        GeEffectEx effect = mOwner.m_pkGeActor.CreateEffect(path, "[actor]Orign", 0, Vec3.zero);
        if (effect != null)
        {
            Battle.GeUtility.AttachTo(effect.GetRootNode(), mOwner.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root));
            effect.SetPosition(data.position.vector3);
        }
    }

    // [炫纹种类]的飞行音效ID
    private readonly int[] mChaserLightSfx = {4731,4727,4730,4728,4729};
    private readonly int[] mChaserLaunchSfx = {4704,4690,4698,4683,4681};
    private void PlayAudio(int type)
    {
        if(mChaserLightSfx.Length <= type || type < 0)
            return;
        if (mOwner != null && mOwner.CurrentBeBattle != null)
			mOwner.CurrentBeBattle.PlaySound(mChaserLightSfx[type]);

        mOwner.delayCaller.DelayCall(100, () =>
        {
            if(mChaserLaunchSfx.Length <= type)
                return;
            if (mOwner != null && mOwner.CurrentBeBattle != null)
			    mOwner.CurrentBeBattle.PlaySound(mChaserLaunchSfx[type]);

        });
    }
#endif
    /// <summary>
    /// 保存子弹的炫纹数据，用于爆炸时取用
    /// </summary>
    /// <param name="proj"></param>
    /// <param name="data"></param>
    private void SetProjectileData(BeProjectile proj, Mechanism2072.ChaserData data)
    {
        if(proj == null)
            return;
        
        int pid = proj.GetPID();
        if (mProjectileChaserData.ContainsKey(pid))
            return;
        
        mProjectileChaserData[pid] = data;
    }

    /// <summary>
    /// 获取保存的子弹炫纹数据
    /// </summary>
    /// <param name="proj"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private bool GetProjectileData(BeProjectile proj, out Mechanism2072.ChaserData data)
    {
        if (proj == null)
        {
            data = default(Mechanism2072.ChaserData);
            return false;
        }
        
        int pid = proj.GetPID();
        if (mProjectileChaserData.ContainsKey(pid))
        {
            data = mProjectileChaserData[pid];
            mProjectileChaserData.Remove(pid);
            return true;
        }

        data = default(Mechanism2072.ChaserData);
        return false;
    }
    

    /// <summary>
    /// 炫纹命中时，创建攻击子弹/炫纹管理器计数-1
    /// </summary>
    /// <param name="args"></param>
    private void OnChaserFlyComplete(BeEvent.BeEventParam eventParam)
    {
        BeProjectile projectile = eventParam.m_Obj as BeProjectile;
        if (projectile == null)
            return;
        
        ReduceChaseCount();
        Mechanism2072.ChaserData chaserData;
        if (!GetProjectileData(projectile, out chaserData))
        {
            return;
        }
        CheckInvincible(projectile, chaserData);
    }

    /// <summary>
    /// 当爆炸时，目标是无敌状态，需要额外补一个炫纹,否则爆炸
    /// </summary>
    /// <returns></returns>
    private void CheckInvincible(BeProjectile projectile, Mechanism2072.ChaserData chaserData)
    {
        if (projectile.logicTrail != null)
        {
            var trail = projectile.logicTrail as AccelerateFollowTargetTrail;
            if (trail != null && trail.target != null && trail.target.stateController != null)
            {
                if (!trail.target.stateController.CanBeHit())
                {
                    AddChaser(chaserData);
                }
                else
                {
                    CreateBoomProjectile(projectile, chaserData);
                }
            }
        }
    }

    /// <summary>
    /// 添加炫纹
    /// </summary>
    /// <param name="chaserData"></param>
    private void AddChaser(Mechanism2072.ChaserData chaserData)
    {
        if (mChaserMgr == null)
        {
            mChaserMgr = GetChaserMgr(mOwner);
            if (mChaserMgr == null)
                return;
        }

        mChaserMgr.AddChaser(chaserData.type, chaserData.sizeType, true);
    }

    /// <summary>
    /// 创建攻击子弹
    /// </summary>
    private void CreateBoomProjectile(BeProjectile proj, Mechanism2072.ChaserData chaserData)
    {
        if(mOwner == null)
            return;

        if((int)chaserData.sizeType >= mChaserBoomResID.GetLength(0))
            return;
        if((int)chaserData.type >= mChaserBoomResID.GetLength(1))
            return;
        int boomResID = mChaserBoomResID[(int)chaserData.sizeType, (int)chaserData.type];
        var eneity = mOwner.AddEntity(boomResID, proj.GetPosition(), level);
        if (eneity != null)
        {
            var scale = GetChaserSizeTypeScale(chaserData.sizeType) + chaserData.scale;
            eneity.SetScale(scale.vint);
        }
    }
    
    
    /// <summary>
    /// 炫纹管理器计数-1
    /// </summary>
    private void ReduceChaseCount()
    {
        if(mOwner == null)
            return;
        
        var mechanism = mOwner.GetMechanism(Mechanism2072.ChaserMgrID) as Mechanism2072;
        if (mechanism == null)
            return;

        mechanism.ReduceChaserCount(1);
    }

    /// <summary>
    /// 生成炫纹弹道子弹
    /// </summary>
    /// <param name="chaserData">炫纹数据</param>
    /// <returns>子弹</returns>
    private BeProjectile GetProjectile(Mechanism2072.ChaserData chaserData)
    {
        if(mOwner == null)
            return null;
        
        var projectile = mOwner.AddEntity(mChaserPathResID, chaserData.position, level, 10000) as BeProjectile;
        if (projectile == null)
            return null;
        
        projectile.SetFace(mOwner.GetPosition().x > mTarget.GetPosition().x);
        var trail = mOwner.CurrentBeBattle.LogicTrailManager.AddAccelerateFollowTargetTrail(mOwner, projectile, mTarget, mTrailAccSpeed);
        projectile.logicTrail = trail;

        //设置实体运行轨迹
        //var trail = new AccelerateFollowTarget();
        //trail.StartPoint = projectile.GetPosition();
        //trail.m_InitSpeed = 0;
        //trail.m_AccSpeed = mTrailAccSpeed;
        //if (mTarget.attribute != null)
        //{
        //    trail.offsetHeight = mTarget.attribute.height / 2;
        //}
        //trail.Init();
        //trail.owner = projectile;
        //trail.target = mTarget;
        //projectile.trail = trail;

        projectile.hurtID = 0;
        
#if !LOGIC_SERVER
        // 挂载自定义特效
        //Battle.GeUtility.AttachTo(chaserData.effect.GetRootNode(), projectile.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Actor));
        //chaserData.effect.GetRootNode().transform.localPosition = Vector3.zero;
        if (projectile.m_pkGeActor != null && chaserData.effect != null && chaserData.effect.GetEffectName() != null)
        {
            GeEffectEx effect = projectile.m_pkGeActor.CreateEffect(chaserData.effect.GetEffectName(), "[actor]origin", 999999999, Vec3.zero, 1f, 1f, false, false, EffectTimeType.BUFF);
            SetEffectScale(chaserData, effect);    
        }
#endif
        // 传透炫纹数据
        SetProjectileData(projectile, chaserData);
        return projectile;
    }
    
#if !LOGIC_SERVER
    /// <summary>
    /// 设置炫纹大小
    /// </summary>
    private void SetEffectScale(Mechanism2072.ChaserData data, GeEffectEx effect)
    {
        if (effect == null)
        {
            return;
        }

        var scale = GetChaserSizeTypeScale(data.sizeType);
        scale += data.scale;
        effect.SetScale(scale.single);
    }
#endif

    private VFactor GetChaserSizeTypeScale(Mechanism2072.ChaseSizeType size)
    {
        VFactor scale = VFactor.one;
        switch (size)
        {
            case Mechanism2072.ChaseSizeType.Big:
                scale = VFactor.NewVFactor(1300, GlobalLogic.VALUE_1000);
                break;
        }
        return scale;
    }
    
    public void Destroy()
    {
        mOwner = null;
        mTarget = null;
        mProjectileChaserData.Clear();
        RemoveHandle();
    }

    private void RemoveHandle()
    {
        if (mHandle != null)
        {
            mHandle.Remove();
            mHandle = null;
        }

/*        if (mEventHandleList != null)
        {
            for (int i = 0; i < mEventHandleList.Count; ++i)
            {
                if (mEventHandleList[i] != null)
                {
                    mEventHandleList[i].Remove();
                    mEventHandleList[i] = null;
                }
            }
            mEventHandleList.Clear();
        }*/
    }
}

/// <summary>
/// 力法-炫纹发射
/// 
///1.在攻击成功后，发射炫纹；炫纹对人物攻击命中的最后一个目标造成相对应的打击与效果
///2.炫纹发射所发射的炫纹 为 先入先出/FIFO 栈 形式
///3.只有在人物直接攻击到目标时才可以 使用 炫纹发射 来发射炫纹（炫纹命中除外，炫纹命中不会触发，异常伤害不会触发，仅人物主动技能伤害后触发）
///4.炫纹最多存在7个
///5.技能释放判定时间为0.5s（PVE）0.4s(PVP)
///6.炫纹爆炸是一个范围攻击（可以攻击多个目标）
///7.一旦人物被攻击时则无法发射炫纹（验证霸体时被击会触发）
///8.PVP是0.4s，PVE是1.5s。冷却PVP是1s，PVE是0.3s（时间自调）
///9.炫纹只有在飞出去并且到达目标后死亡后，才会生成新的炫纹（场上不会同时存在8个炫纹，有且只有7个炫纹）
/// </summary>
public class Skill2302 : BeSkill
{
    private LaunchChaser mLaunchHelper = new LaunchChaser();
    public Skill2302(int sid, int skillLevel) : base(sid, skillLevel)
    {
    
    }
    
    public override void OnInit()
    {
        base.OnInit();
        var trailTime = 200;
        if(skillData.ValueA.Count > 0)
            trailTime = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        
        mLaunchHelper.Init(owner, level, trailTime);
        SetSkillMode();
    }

    public override bool CanUseSkill()
    {
        return CanLaunch();
    }

    public override bool CheckSpellCondition(ActionState state)
    {
        return true;
    }
    
    public override void OnClickAgain()
    {
        if (CanLaunch() && mLaunchHelper.Launch())
        {
            if (owner != null)
            {
                owner.buffController.TriggerBuffs(BuffCondition.RELEASE_SEPCIFY_SKILL, null, 2302);
            }
            StartCoolDown();
            SetDisable();
        }
        else
        {
#if !LOGIC_SERVER
            if (owner.isLocalActor)
            {
                if(owner.m_pkGeActor!=null)
                    owner.m_pkGeActor.CreateHeadText(GameClient.HitTextType.SKILL_CANNOTUSE, "UI/Font/new_font/pic_ylbmzfstj.png");    
            }
#endif
        }
    }

    /// <summary>
    /// 设置技能按钮模式
    /// </summary>
    private void SetSkillMode()
    {
        skillButtonState = BeSkill.SkillState.WAIT_FOR_NEXT_PRESS;
        pressMode = SkillPressMode.PRESS_MANY_TWO;
    }   

    public override bool CanForceUseSkill()
    {
        return true;
    }

    /// <summary>
    /// 是否能够发射炫纹
    /// </summary>
    /// <returns></returns>
    private bool CanLaunch()
    {
        //1.炫纹激活：攻击到别人后开始计时，在时间内激活
        //2.是否有炫纹
        //3.是否冷却结束
        //4.判定锁定的怪是否死亡
        return IsActive() && HasChaser() && !isCooldown && mLaunchHelper.CanLaunch() && base.CanUseSkill();
    }
    
    /// <summary>
    /// 炫纹发射是否激活
    /// </summary>
    /// <returns></returns>
    private bool IsActive()
    {
        if (owner == null)
            return false;
        
        var mechanism = owner.GetMechanism(5284);
        if (mechanism == null)
            return false;
        
        Mechanism2079 mech = mechanism as Mechanism2079;
        if (mech != null)
        {
            return mech.IsActive();
        }

        return false;
    }

    /// <summary>
    /// 炫纹发射是关闭
    /// </summary>
    /// <returns></returns>
    private void SetDisable()
    {
        var mechanism = owner.GetMechanism(5284);
        if (mechanism == null)
            return;
        
        Mechanism2079 mech = mechanism as Mechanism2079;
        if (mech != null)
        {
            mech.SetActive(false);
        }
    }
    /// <summary>
    /// 是否有炫纹
    /// </summary>
    /// <returns></returns>
    private bool HasChaser()
    {
        if (owner == null)
            return false;
        
        var mechanism = owner.GetMechanism(Mechanism2072.ChaserMgrID);
        if (mechanism == null)
            return false;
        
        Mechanism2072 mech = mechanism as Mechanism2072;
        if (mech != null)
        {
            return mech.GetChaserCount() > 0;
        }

        return false;
    }

    public override void OnFinish()
    {
        base.OnFinish();
        if (mLaunchHelper != null)
        {
            mLaunchHelper.Destroy();
            mLaunchHelper = null;
        }
    }
    
}
