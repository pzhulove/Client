using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill1911 : BeSkill
{

    public Skill1911(int sid, int skillLevel):base(sid, skillLevel)
	{

    }

    public override void OnWeaponChange()
    {
        base.OnWeaponChange();
        var skillData = owner.GetActionInfoBySkillID(skillID);
        if (skillData != null)
        {
            charge = owner.GetActionInfoBySkillID(skillID).useCharge;
            chargeConfig = skillData.chargeConfig;
        }
    }
}

public class Skill1928 : BeSkill
{
    public Skill1928(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

}
