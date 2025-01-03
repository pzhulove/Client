using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill1617 : BeSkill
{
    int buffID;
    int weaponType;

    public Skill1617(int sid, int skillLevel):base(sid, skillLevel){}

    public override void OnInit()
    {
        weaponType = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        buffID = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
    }

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
        if (owner != null && buffID != 0)
            owner.buffController.RemoveBuff(buffID);

        if (owner != null && buffID > 0 && owner.GetWeaponType() == weaponType)
        {
            owner.buffController.TryAddBuff(buffID, int.MaxValue, level);
        }
    }

}
