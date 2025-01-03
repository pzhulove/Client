using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill1610 : BeSkill
{
    int savedBuffID = 0;
    public Skill1610(int sid, int skillLevel):base(sid, skillLevel){}

    public override void OnPostInit()
    {
        DoEffect();
    }

    public override void OnWeaponChange()
    {
        OnPostInit();
    }

    void DoEffect()
    {
		if (owner == null)
			return;

        if (savedBuffID != 0)
        {
            owner.buffController.RemoveBuff(savedBuffID);
            savedBuffID = 0;
        }

        int ownerWeaponType = owner.GetWeaponType();

		for(int i=0; i<skillData.ValueA.Count; ++i)
		{
			int weaponType = TableManager.GetValueFromUnionCell(skillData.ValueA[i], level);
			int buffID = TableManager.GetValueFromUnionCell(skillData.ValueB[i], level);

			if (weaponType == ownerWeaponType)
			{
				owner.buffController.TryAddBuff(buffID, int.MaxValue, level);
                savedBuffID = buffID;
				break;
			}
				
		}
    }

}
