using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill5896 : BeSkill {

    public Skill5896(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override bool CanUseSkill()
    {
        return base.CanUseSkill()&&!owner.stateController.WillBeGrab();
    }

}
