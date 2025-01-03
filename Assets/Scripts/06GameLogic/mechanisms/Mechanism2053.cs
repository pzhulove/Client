using System;
using System.Collections.Generic;
using GameClient;

//光线吸收技能逻辑
public class Mechanism2053 : BeMechanism
{
    int srcSkillID = 0;
    int targetSkillID = 0;
    public Mechanism2053(int mid, int lv) : base(mid, lv)
    {
    }

    public override void OnInit()
    {
        base.OnInit();
        srcSkillID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        targetSkillID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onReachMaxEnergy, onReachMaxEnergy);
    }
    private void onReachMaxEnergy(BeEvent.BeEventParam args)
    {
        if(owner.sgGetCurrentState() == (int)ActionState.AS_CASTSKILL)
        {
            if(owner.GetCurSkillID() == srcSkillID)
            {
                owner.UseSkill(targetSkillID);
            }
        }
    }
}

