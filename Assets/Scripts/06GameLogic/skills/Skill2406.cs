using GameClient;

/// <summary>
/// 药剂-熊
/// </summary>
public class Skill2406 : MixSummonSkill
{
    public Skill2406(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }

    private bool m_InMix = false;
    public override void OnClickAgain()
    {
        if (m_InMix)
        {
            // 合体技能再次点击
            owner.skillController.GetCurrentSkill()?.OnClickAgain();
        }
        else
        {
            base.OnClickAgain();
        }
    }

    protected override void StartMix()
    {
        base.StartMix();
        m_InMix = true;
    }

    protected override void EndMix()
    {
        base.EndMix();
        m_InMix = false;
        if (Monster != null)
        {
            Monster.Locomote(new BeStateData((int)ActionState.AS_IDLE), true);
        }
    }

    protected override bool CanMix()
    {
        if (m_InMix)
        {
            var s = owner.skillController.GetCurrentSkill();
            if (s != null)
            {
                var skill = s as Skill2416;
                if (skill != null)
                {
                    return skill.CanUse();
                }
            }
        }

        return base.CanMix();
    }
}


/// <summary>
/// 药剂-熊(主角的合体技能)
/// </summary>
public class Skill2416 : BeSkill
{
    public Skill2416(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }

    private int m_SkillDuration;
    private int m_SkillDurationAcc = 0;
    private int m_IdleSkillPhase = 0;
    private int m_ShootSkillPhase = 0;
    private int m_EndSkillPhase = 0;
    private int m_CurPhaseId = 0;
    private IBeEventHandle m_Handle;

    public override void OnInit()
    {
        base.OnInit();
        m_SkillDuration = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        m_IdleSkillPhase = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
        m_ShootSkillPhase = TableManager.GetValueFromUnionCell(skillData.ValueB[1], level);
        m_EndSkillPhase = TableManager.GetValueFromUnionCell(skillData.ValueB[2], level);
    }

    public override void OnPostInit()
    {
        base.OnPostInit();
        if (m_Handle != null)
        {
            m_Handle.Remove();
            m_Handle = null;
        }
        m_Handle = owner.RegisterEventNew(BeEventType.onPreSetSkillAction, OnExecutePhase);
    }

    private void OnExecutePhase(BeEvent.BeEventParam param)
    {
        m_CurPhaseId = param.m_Int;
    }

    public override void OnStart()
    {
        base.OnStart();
        m_SkillDurationAcc = 0;
    }

    public override void OnUpdate(int iDeltime)
    {
        base.OnUpdate(iDeltime);
        m_SkillDurationAcc += iDeltime;
        if (m_SkillDurationAcc > m_SkillDuration && m_CurPhaseId == m_IdleSkillPhase)
        {
            owner.skillController.skillPhase = 3;
            ((BeActorStateGraph)owner.GetStateGraph()).ExecuteSkill(m_EndSkillPhase);

        }
    }

    public override void OnClickAgain()
    {
        base.OnClickAgain();
        if (m_CurPhaseId == m_IdleSkillPhase)
        {
            owner.skillController.skillPhase = 1;
            ((BeActorStateGraph)owner.GetStateGraph()).ExecuteSkill(m_ShootSkillPhase);
        }
    }

    public bool CanUse()
    {
        return m_CurPhaseId == m_IdleSkillPhase;
    }

    public override bool NeedClearSpeed()
    {
        if (owner.skillController.skillPhase == 2 || owner.skillController.skillPhase == 1)
            return false;

        return true;
    }
}
