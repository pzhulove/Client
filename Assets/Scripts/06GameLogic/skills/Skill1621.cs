using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill1621 : BeSkill {

    public Skill1621(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override bool CanUseSkill()
    {
        if (base.CanUseSkill())
        {
            var data = owner.GetEntityData();
            if (null != data)
            {
                VFactor leftRate = data.GetHPRate();
                VFactor lowFactor = new VFactor(skillData.LowHpPercent, GlobalLogic.VALUE_1000);
                return leftRate < lowFactor;
            }
        }
        return false;
    }
}
