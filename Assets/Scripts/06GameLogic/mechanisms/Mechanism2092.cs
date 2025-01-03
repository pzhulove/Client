using System;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

/// <summary>
/// 当炫纹发射后命中敌人时产生暴击时，会自动发射一枚指定属性的炫纹攻击敌人
/// （0：光，1：火，2：冰，3：暗，4：无属性）；这个额外的炫纹不会消耗自身的炫纹，而是立即生成立即发射
/// Tips: 确认该炫纹发射不用适用炫纹缩放大小机制
/// </summary>
public class Mechanism2092 : BeMechanism
{
    private readonly int mChaserPathResID = 63748;

    private readonly VInt3 mChaserLaunchPos = new VInt3(-0.752f, -0.38f, 1.811f);
    //private List<BeEventHandle> mEventHandleList = new List<BeEventHandle>();

    private readonly string[] m_ChaserPathArr =
    {
        "Effects/Hero_Lifa/Eff_Lifa_XWRH/Prefab/Eff_Lifa_XWRH_wu",
        "Effects/Hero_Lifa/Eff_Lifa_XWRH/Prefab/Eff_Lifa_XWRH_guang",
        "Effects/Hero_Lifa/Eff_Lifa_XWRH/Prefab/Eff_Lifa_XWRH_fire",
        "Effects/Hero_Lifa/Eff_Lifa_XWRH/Prefab/Eff_Lifa_XWRH_ice",
        "Effects/Hero_Lifa/Eff_Lifa_XWRH/Prefab/Eff_Lifa_XWRH_an"
    };
    
    private CoolDown m_CD = new CoolDown();
    private HashSet<int> mTargetHurtSet = new HashSet<int>();
    private int mChaserType;
    private int mAddChaserType = -1;
    private int m_TriggerCD = 0;
    private int m_ChaserLevel = -1;
    private int m_ChaserAccSpeed = 200;
    private Mechanism2072 chaserMgr = null;
    
    public Mechanism2092(int mid, int lv) : base(mid, lv) { }
    public override void OnInit()
    {
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            mTargetHurtSet.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }   
        mChaserType = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        
        if (data.ValueC.Count > 0)
        {
            m_TriggerCD = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        }

        // -2表示不会生成炫纹
        if (data.ValueD.Count > 0)
        {
            mAddChaserType = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        }
        else
        {
            mAddChaserType = -1;
        }
        
        if (data.ValueE.Count > 0)
        {
            m_ChaserLevel = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        }
        
        if (data.ValueF.Count > 0)
        {
            m_ChaserAccSpeed = TableManager.GetValueFromUnionCell(data.ValueF[0], level);
        }
    }

    public override void OnReset()
    {
        m_CD.Clear();
        mTargetHurtSet.Clear();
        m_TriggerCD = 0;
        m_ChaserLevel = -1;
        m_ChaserAccSpeed = 200;
        chaserMgr = null;
    }

    public override void OnStart()
    {
        if(owner == null)
            return;

        //handleA = owner.RegisterEvent(BeEventType.onHitOtherAfterHurt, OnAttackResult);
        handleA = OwnerRegisterEventNew(BeEventType.onHitOtherAfterHurt, OnAttackResult);
    }

    private void OnAttackResult(BeEvent.BeEventParam param)
    {
        if(m_TriggerCD > 0 && m_CD.IsCD())
            return;
        
        AttackResult result = (AttackResult)param.m_Int2;
        if (result == AttackResult.CRITICAL)
        {
            int hurtId = param.m_Int;
            if (mTargetHurtSet.Contains(hurtId))
            {
                // 对方死亡不发射炫纹
                BeActor target = param.m_Obj as BeActor;
                if (target != null && !target.IsDead())
                {
                    LaunchChaser(target);
                }
                
                if(mAddChaserType  != -2)
                    AddChaser();
                
                if (m_TriggerCD > 0)
                {
                    m_CD.StartCD(m_TriggerCD);
                }
            }
        }
    }
    
    public override void OnUpdate(int deltaTime)
    {
        if (m_TriggerCD > 0)
        {
            m_CD.UpdateCD(deltaTime);
        }
    }

    private void AddChaser()
    {
        if (chaserMgr == null)
        {
            chaserMgr = GetChaserMgr(owner);
        }

        if (chaserMgr == null)
        {
            return;
        }
        
        int type = mAddChaserType;
        if (mAddChaserType == -1)
        {
            type = FrameRandom.Random(5);
        }

        chaserMgr.AddChaserByExternal((Mechanism2072.ChaserType) type, Mechanism2072.ChaseSizeType.Normal, true);
    }
    
    private Mechanism2072 GetChaserMgr(BeActor owner)
    {
        if(owner == null)
            return null;
        
        var baseMech = owner.GetMechanism(Mechanism2072.ChaserMgrID);
        if (baseMech == null)
            return null;
        
        var mechanism = baseMech as Mechanism2072;
        return mechanism;
    }
    
    private bool LaunchChaser(BeActor target)
    {
        var projectile = GetProjectile(target);
        if (projectile == null)
            return false;
        
        // 这里不存handle因为，handle无处remove。反而一直增加内存。handle生命周期伴随projectile
        projectile.RegisterEventNew(BeEventType.onDead, OnChaserFlyComplete);
        //mEventHandleList.Add(handle);
        return true;
    }
    
    /// <summary>
    /// 炫纹命中时
    /// </summary>
    /// <param name="args"></param>
    private void OnChaserFlyComplete(BeEvent.BeEventParam eventParam)
    {
        BeProjectile projectile = eventParam.m_Obj as BeProjectile;
        if (projectile == null)
            return;
        
        CreateBoomProjectile(projectile);
    }

    private BeProjectile GetProjectile(BeActor target)
    {
        if (owner == null)
            return null;
        
        var projectile = owner.AddEntity(mChaserPathResID, GetAnchorWorldPos(mChaserLaunchPos),  GetLevel(), 10000) as BeProjectile;
        if (projectile == null)
            return null;
        
        projectile.SetFace(owner.GetPosition().x > target.GetPosition().x);
        var trail = owner.CurrentBeBattle.LogicTrailManager.AddAccelerateFollowTargetTrail(owner, projectile, target, m_ChaserAccSpeed);
        projectile.logicTrail = trail;

        //设置实体运行轨迹
        //var trail = new AccelerateFollowTarget();
        //trail.StartPoint = projectile.GetPosition();
        //trail.m_AccSpeed = m_ChaserAccSpeed;
        //if (target.attribute != null)
        //{
        //    trail.offsetHeight = target.attribute.height / 2;
        //}
        //trail.Init();
        //trail.owner = projectile;
        //trail.target = target;
        //projectile.trail = trail;

        projectile.hurtID = 0;
        
#if !LOGIC_SERVER
        CreateLightEffect(mChaserType);
        PlayAudio(mChaserType);
        // 挂载自定义特效 
        if (projectile.m_pkGeActor != null)
        {
            GeEffectEx effect = projectile.m_pkGeActor.CreateEffect(m_ChaserPathArr[mChaserType], "[actor]origin", 999999999, Vec3.zero, 1f, 1f, false, false, EffectTimeType.BUFF);
            if(effect != null)
                effect.SetScale(1f);    
        }
#endif
        return projectile;
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

    private void CreateLightEffect(int type)
    {
        if(mChaserLightEffect.Length <= type || type < 0)
            return;
        if(owner == null || owner.m_pkGeActor == null)
            return;
        string path = mChaserLightEffect[type];
        GeEffectEx effect = owner.m_pkGeActor.CreateEffect(path, "[actor]Orign", 0, Vec3.zero);
        if (effect != null)
        {
            Battle.GeUtility.AttachTo(effect.GetRootNode(), owner.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root));
            effect.SetPosition(GetAnchorWorldPos(mChaserLaunchPos).vector3);
        }
    }

    // [炫纹种类]的飞行音效ID
    private readonly int[] mChaserLightSfx = {4731,4727,4730,4728,4729};
    private readonly int[] mChaserLaunchSfx = {4704,4690,4698,4683,4681};
    private void PlayAudio(int type)
    {

        if(mChaserLightSfx.Length <= type || type < 0)
            return;
        if (owner.CurrentBeBattle != null)
            owner.CurrentBeBattle.PlaySound(mChaserLightSfx[type]);

        owner.delayCaller.DelayCall(100, () =>
        {
            if(mChaserLaunchSfx.Length <= type)
                return;
            if (owner.CurrentBeBattle != null)
                owner.CurrentBeBattle.PlaySound(mChaserLaunchSfx[type]);

        });
    }
#endif
    
    /// <summary>
    /// 创建攻击子弹
    /// </summary>
    /// <param name="pathProjectile"></param>
    private void CreateBoomProjectile(BeProjectile proj)
    {
        if(owner == null)
            return;
        
        int[] mChaserBoomResID = {63733, 63735, 63741, 63739, 63737};
        if(mChaserType >= mChaserBoomResID.Length)
            return;
        
        int boomResID = mChaserBoomResID[mChaserType];
        var eneity = owner.AddEntity(boomResID, proj.GetPosition(), GetLevel());
        if (eneity != null)
        {
            eneity.SetScale(1);
        }
    }

    private int GetLevel()
    {
        // 有配置走配置
        if (m_ChaserLevel > 0)
            return m_ChaserLevel;
        
        // 没有就去技能等级
        if(owner == null)
            return 1;
        
        int level = owner.GetSkillLevel(2302);
        if (level <= 0)
        {
            level = 1;
        }

        return level;
    }
    
    private VInt3 GetAnchorWorldPos(VInt3 pos)
    {
        if(owner == null)
            return VInt3.zero;
        
        pos.x = owner.GetFace() ? pos.x * -1 : pos.x;
        return owner.GetPosition() + pos;
    }

    /*public override void OnFinish()
    {
        base.OnFinish();
        //RemoveProjectileHandle();
    }*/
    
    /*private void RemoveProjectileHandle()
    {
        if (mEventHandleList != null)
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
        }
    }*/
    
}

