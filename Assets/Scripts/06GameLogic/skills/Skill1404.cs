/*
/// <summary>
/// 1.在盾卫在存在时间内，再次点击技能按键，可以使盾卫发动指定ID的技能
/// </summary>
public class Skill1404 : SummonSkill, ISummonSkillUpdate
{
    public Skill1404(int sid, int skillLevel) : base(sid, skillLevel) { }
    
    private int assignSkillId = 1411; //指定的技能ID

    public override void OnClickAgain()
    {
        if(OwnerCanUseSkill() && MonsterCanUseSkill(assignSkillId))
        {
            Monster.UseSkill(assignSkillId);
        }
    }

    private bool OwnerCanUseSkill()
    {
        if (owner.IsDeadOrRemoved() || owner.IsInPassiveState())
            return false;

        return true;
    }
    
    private bool MonsterCanUseSkill(int skillId)
    {
        if (Monster == null)
            return false;
        
        if (Monster.IsDeadOrRemoved() || Monster.IsPassiveState())
            return false;
        
        if (!Monster.CanUseSkill(skillId))
            return false;

        return true;
    }

    public void OnSummonUpdate(int deltaTime)
    {
        SetLightButtonVisible(OwnerCanUseSkill() && MonsterCanUseSkill(assignSkillId));
    }

    public void OnSummonUpdateFinish()
    {
        SetLightButtonVisible(false);
    }
}
*/
