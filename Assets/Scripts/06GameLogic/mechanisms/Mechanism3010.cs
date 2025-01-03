using GameClient;

public class Mechanism3010 : BeMechanism
{
    public Mechanism3010(int mid, int lv) : base(mid, lv)
    {
    }

    enum State
    {
        Normal = 0,        // 未技能强化 未蓄力完成
        Charge = 1,        // 未技能强化 蓄力完成
        NormalStrong = 2,  // 技能强化 未蓄力
        ChargeStrong = 3,  // 技能强化 蓄力
    }

    private State m_CurState;
    private bool m_IsIdle = false;

    private IBeEventHandle m_Handle;
    private int m_AttackSkillId;
    private int[] m_IdleSkillIds;
    private int[] m_AttackSkillIds;
    private int[] m_BoomEntityIds;

    private readonly int SummonSkillId = 1419;
    public override void OnInit()
    {
        base.OnInit();
        m_IdleSkillIds = new int[data.ValueA.Count];
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            m_IdleSkillIds[i] = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
        }
        
        m_AttackSkillIds = new int[data.ValueB.Count];
        for (int i = 0; i < data.ValueB.Count; i++)
        {
            m_AttackSkillIds[i] = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
        }
        
        m_BoomEntityIds = new int[data.ValueC.Count];
        for (int i = 0; i < data.ValueC.Count; i++)
        {
            m_BoomEntityIds[i] = TableManager.GetValueFromUnionCell(data.ValueC[i], level);
        }
    }

    public override void OnReset()
    {
        m_CurState = State.Normal;
        m_IsIdle = false;
        if (m_Handle != null)
        {
            m_Handle.Remove();
            m_Handle = null;
        }
        m_AttackSkillId = 0;
    }

    public override void OnStart()
    {
        base.OnStart();
        if (IsStronger())
        {
            m_CurState = State.NormalStrong;
        }
        else
        {
            m_CurState = State.Normal;
        }
        handleA = owner.RegisterEventNew(BeEventType.onHitOtherAfterHurt, OnHitOther);
        m_Handle = owner.RegisterEventNew(BeEventType.onCastSkillFinish, OnAttackComplete);
        var actor = GetTopOwner();
        if(actor != null)
        {
            handleB = actor.RegisterEventNew(BeEventType.onCastSkillFinish, OnSkillEnd);
            handleC = actor.RegisterEventNew(BeEventType.onSkillCancel, OnSkillEnd);
            handleD = actor.RegisterEventNew(BeEventType.OnSkillChargeComplete, OnSkillChargeComplete);
        }
    }

    private void OnAttackComplete(BeEvent.BeEventParam param)
    {
        if (param.m_Int == m_AttackSkillId)
        {
            if (m_Handle != null)
            {
                m_Handle.Remove();
                m_Handle = null;
            }
            owner.SetIsDead(true);
        }
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (m_IsIdle)
            return;
        
        if (!owner.IsInPassiveState())
        {
            m_IsIdle = true;
            owner.UseSkill(m_IdleSkillIds[(int) m_CurState], true);
        }
    }

    private void OnHitOther(BeEvent.BeEventParam param)
    {
        if (handleA != null)
        {
            handleA.Remove();
            handleA = null;
        }
        //owner.AddEntity(m_BoomEntityIds[(int) m_CurState], owner.GetPosition(), level);
        if (!owner.IsDead())
        {
            owner.SetIsDead(true);
            owner.DoDead();    
        }
    }

    public void OnSkillChargeComplete(BeEvent.BeEventParam param)
    {
        if(param.m_Int != SummonSkillId)
            return;
        
        if (m_CurState == State.NormalStrong)
        {
            m_CurState = State.ChargeStrong;
        }
        else if (m_CurState == State.Normal)
        {
            m_CurState = State.Charge;
        }
        owner.Locomote(new BeStateData((int)ActionState.AS_IDLE), true);
        owner.UseSkill(m_IdleSkillIds[(int) m_CurState], true);
    }

    
    public void OnSkillEnd(BeEvent.BeEventParam param)
    {
        if (param.m_Int == SummonSkillId)
        {
            CancelControl();
            owner.Locomote(new BeStateData((int)ActionState.AS_IDLE), true);
            m_AttackSkillId = m_AttackSkillIds[(int) m_CurState];
            owner.UseSkill(m_AttackSkillId, true);
        }
    }

    private void CancelControl()
    {
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
        if (handleD != null)
        {
            handleD.Remove();
            handleD = null;
        }
    }
    
    private bool IsStronger()
    {
        var topOwner = GetTopOwner();
        if (topOwner == null)
            return false;

        var skill = topOwner.GetSkill(1429);
        if (skill == null)
            return false;
        if (skill is Skill1429 s)
        {
            return s.CanAndUseStronger();
        }

        return false;
    }
}