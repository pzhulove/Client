using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill51291 : Skill9572
{
    public Skill51291(int sid, int skillLevel) : base(sid, skillLevel) { }
}

public class Skill9572 : BeSkill
{
    protected int minCD = 0;
    protected int maxCD = 0;
    protected bool _containAwakeAndBuffSkill = false;

    public Skill9572(int sid, int skillLevel):base(sid, skillLevel){}
    public override void OnInit()
    {
        minCD = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        maxCD = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
        _containAwakeAndBuffSkill = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level) == 0 ? false : true;
    }

    public override bool CanUseSkill()
    {
        if (!_containAwakeAndBuffSkill)
            return base.CanUseSkill() && Mechanism50.GetCanCooldownSkills(owner.GetOwner() as BeActor, minCD, maxCD);
        else
            return base.CanUseSkill() && Mechanism50.GetCanReduceCDSkills(owner.GetOwner() as BeActor, minCD);
    }
}
