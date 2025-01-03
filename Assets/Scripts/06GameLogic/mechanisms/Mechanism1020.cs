using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanism1020 : BeMechanism
{
    int curSkillID = 0;
    int skillID = 0;
    public Mechanism1020(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        base.OnInit();
        curSkillID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        skillID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onHit, args =>
        //handleA = owner.RegisterEvent(BeEventType.onHit, (args) => 
        {
            if (owner.sgGetCurrentState() == (int)ActionState.AS_CASTSKILL && owner.GetCurSkillID() == curSkillID)
            {
                owner.UseSkill(skillID);
            }
        });
    }

    public override void OnDead()
    {
        base.OnDead();
    }

    public override void OnFinish()
    {
        base.OnFinish();
    }
}
