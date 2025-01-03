using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill2602 : BeSkill
{
    VRate speed = VRate.zero;
    public Skill2602(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }

    public override void OnInit()
    {
        speed = new VRate(TableManager.GetValueFromUnionCell(skillData.ValueE[0], level) / 1000.0f);
    }

    public override void OnStart()
    {
        base.OnStart();
        SetSkillSpeed();
    }

    private void SetSkillSpeed()
    {
        VRate n = GlobalLogic.VALUE_1000;
        n += speed;
        skillSpeed = skillData.Speed * n.factor;
    }
    public override void OnEnterPhase(int phase)
    {
        base.OnEnterPhase(phase);
        if (phase == 2)
        {
            skillSpeed = skillData.Speed;
        }
    }
}
