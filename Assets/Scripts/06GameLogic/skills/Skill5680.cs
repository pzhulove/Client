using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill5680 : BeSkill {


    public BeActor partnerMonster = null;
    public BeActor protectMonster = null;
    public Skill5680(int sid, int skillLevel):base(sid, skillLevel)
    {

    }

    public override void OnPostInit()
    {
        base.OnPostInit();
    }

    public override void OnStart()
    {
        base.OnStart();      
    }

    public override void OnCancel()
    {
        base.OnCancel();
        StopSkill();
    }

    public override void OnFinish()
    {
        StopSkill();
        base.OnFinish();
    }

    private void StopSkill()
    {
        if (partnerMonster != null)
        {
            partnerMonster.Locomote(new BeStateData((int)ActionState.AS_IDLE), true);
        }
        if (protectMonster != null)
        {
            protectMonster.DoDead();
        }
        protectMonster = null;
        partnerMonster = null;
    }
}
