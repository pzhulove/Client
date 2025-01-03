using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;

public class Mechanism96 : BeMechanism
{

    int SkillID = 0;
    int buffID = 0;
    int weaponType = 0;
    IBeEventHandle castSkillHandle;
    IBeEventHandle switchWeaponHandle;
    public Mechanism96(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
         SkillID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
         buffID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
         weaponType = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
    }
   

    public override void OnStart()
    {
        RemoveBuff();
        castSkillHandle = owner.RegisterEventNew(BeEventType.onCastSkill, args =>
        {
            int id = args.m_Int;
            if (id == SkillID)
            {
                RemoveBuff();
            }
        });

        switchWeaponHandle = owner.RegisterEventNew(BeEventType.OnChangeWeapon, OnChangeWeapon);

        handleA = owner.RegisterEventNew(BeEventType.onChangeEquipEnd, OnChangeEquip);
    }

    protected void OnChangeWeapon(BeEvent.BeEventParam param)
    {
        RemoveBuff();
    }

    protected void OnChangeEquip(BeEvent.BeEventParam param)
    {
        RemoveBuff();
    }

    private void RemoveBuff()
    {
        if (weaponType != owner.GetWeaponType())
        {
            owner.buffController.RemoveBuff(buffID);
        }
    }

  

    public override void OnFinish()
    {
        if (castSkillHandle != null)
        {
            castSkillHandle.Remove();
            castSkillHandle = null;
        }
        if (switchWeaponHandle != null)
        {
            switchWeaponHandle.Remove();
            switchWeaponHandle = null;
        }
    }

}
