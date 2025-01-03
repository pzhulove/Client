public class Skill1439 : BeSkill
{
    public Skill1439(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }

    private Skill1400 m_Skill;
    public override void OnStart()
    {
        var skill = GetSkill();
        if (skill != null)
        {
            skill.SetLockEnemy(false);
        }
    }

    public override void OnCancel ()
    {
        var skill = GetSkill();
        if (skill != null)
        {
            skill.SetLockEnemy(true);
        }
    }

    private Skill1400 GetSkill()
    {
        if (m_Skill == null)
        {
            var skill = owner.GetSkill(1400);
            if (skill != null)
            {
                var mgr = skill as Skill1400;
                m_Skill = mgr;
            }
        }
        return m_Skill;
    }
}
