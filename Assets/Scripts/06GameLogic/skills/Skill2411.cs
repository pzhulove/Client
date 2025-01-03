using GameClient;

/// <summary>
/// 药剂-鹿
/// </summary>
public class Skill2411 : MixSummonSkill
{
    public Skill2411(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }
    
    private bool m_InMix = false;
    
    public override void OnClickAgain()
    {
        if (m_InMix)
        {
            // 合体技能再次点击
            owner.GetSkill(2419)?.OnClickAgain();
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
            return owner.skillController.skillPhase == 2;
        }

        return base.CanMix();
    }
}

// 鹿-合体时技能
public class Skill2419 : BeSkill
{
    public Skill2419(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }
    
    private int m_SkillDuration;
    private int m_SkillDurationAcc = 0;
    private int m_IdlePhase;
    private int m_AgainPhase;
    private int m_NormalAttackPhase = 0;
    private int m_CancelSkillPhase = 0;
    private int m_CurPhaseId = 0;
    
    public override void OnInit()
    {
        base.OnInit();
        m_SkillDuration = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        m_IdlePhase = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
        m_NormalAttackPhase = TableManager.GetValueFromUnionCell(skillData.ValueB[1], level);
        m_CancelSkillPhase = TableManager.GetValueFromUnionCell(skillData.ValueB[2], level);

        startJumpBackCnacelFlag = "241901";
        endJumpBackCnacelFlag = "241902";
    }
    
    public override bool CanBeInterrupt(int skillId)
    {
        if (skillId == owner.GetEntityData().normalAttackID || skillId == owner.GetEntityData().jumpAttackID)
            return true;
        return base.CanBeInterrupt(skillId);
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onExecSkillFrame, onExecSkillFrame);
        handleB = owner.RegisterEventNew(BeEventType.onPreSetSkillAction, OnExecutePhase);
        m_SkillDurationAcc = 0;
    }

    private void OnExecutePhase(BeEvent.BeEventParam param)
    {
        m_CurPhaseId = param.m_Int;
    }
    
    public override void OnUpdate(int iDeltime)
    {
        base.OnUpdate(iDeltime);
        m_SkillDurationAcc += iDeltime;
        if (m_SkillDurationAcc > m_SkillDuration && m_CurPhaseId == m_IdlePhase)
        {
            ((BeActorStateGraph)owner.GetStateGraph()).ExecuteEndPhase();
        }
    }

    private void onExecSkillFrame(BeEvent.BeEventParam param)
    {
        if (!param.m_Bool && m_CurPhaseId == m_IdlePhase)
        {
            if (param.m_Int == (int) SpecialSkillID.JUMP_BACK)
            {
                if (canPressJumpBackCancel)
                {
                    owner.skillController.skillPhase = owner.skillController.SkillPhaseArray.Length + 1;
                    ExecSkillPhase(m_CancelSkillPhase);
                    param.m_Bool2 = true;
                }
            }
            else if (param.m_Int == (int) SpecialSkillID.NORMAL_ATTACK)
            {
                owner.skillController.skillPhase--;
                ExecSkillPhase(m_NormalAttackPhase);
                param.m_Bool2 = true;
            }
        }
    }

    public override void OnClickAgain()
    {
        base.OnClickAgain();
        owner.SetFace(!owner.GetFace());
    }

    public void ExecSkillPhase(int phase)
    {
        ((BeActorStateGraph)owner.GetStateGraph()).ExecuteSkill(phase);
    }
    
    public override bool NeedClearSpeed()
    {
        if (owner.skillController.skillPhase == 2 || owner.skillController.skillPhase == 1)
            return false;

        return true;
    }
}