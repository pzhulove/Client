/// <summary>
/// 老鹰
/// </summary>
public class Skill2413 : Skill2402
{
    public Skill2413(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }
    private bool mIsFlying = false;

    private bool CanClickEnd()
    {
        return owner.GetCurSkillID() == OwnerTargetSkillId && owner.skillController.skillPhase == 2 && mIsFlying;
    }
    public override void OnClickAgain()
    {
        if (CanClickEnd())
        {
            OnClickEnd();
        }
        else
        {
            base.OnClickAgain();
        }
    }

    protected override bool CanMix()
    {
        if (mIsFlying)
        {
            return true;
        }
        else
        {
            return base.CanMix();
        }
    }
    
    protected override void StartMix()
    {
        base.StartMix();
        mIsFlying = true;
    }

    private void OnClickEnd()
    {
        if (owner.GetCurSkillID() == OwnerTargetSkillId)
        {
            ((BeActorStateGraph) owner.GetStateGraph()).ExecuteEndPhase();
        }


        if (Monster.GetCurSkillID() == SummonTargetSkillId)
        {
            if (Monster != null)
            {

                ((BeActorStateGraph) Monster.GetStateGraph()).ExecuteEndPhase();
            }
        }
    }

    protected override VInt3 GetMixPosition()
    {
        var ret = base.GetMixPosition();
        if(Monster.isFloating)
            ret.z = Monster.floatingHeight.i;
        return ret;
    }
    
    protected override void EndMix()
    {
        mIsFlying = false;
        base.EndMix();
    }
}