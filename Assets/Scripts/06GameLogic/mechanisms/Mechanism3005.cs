public class Mechanism3005 : BeMechanism
{
    public Mechanism3005(int mid, int lv) : base(mid, lv) { }

    private int m_SkillId;
    private int m_ReplaceSkillId;
    public override void OnInit()
    {
        base.OnInit();
        m_SkillId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        if(data.ValueB.Count > 0)
            m_ReplaceSkillId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnReset()
    {
        m_ReplaceSkillId = 0;
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (!owner.IsInPassiveState())
        {
            owner.UseSkill(GetSkillId(), true);
            Finish();
        }
    }

    private int GetSkillId()
    {
        if (m_ReplaceSkillId <= 0)
            return m_SkillId;

        if (IsStronger())
        {
            return m_ReplaceSkillId;
        }

        return m_SkillId;
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

