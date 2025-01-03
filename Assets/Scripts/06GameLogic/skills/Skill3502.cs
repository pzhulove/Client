using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//驱魔普攻
public class Skill3502 : BeSkill
{
    public Skill3502(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnInit()
    {
        base.OnInit();
    }

    public override void OnStart()
    {
        base.OnStart();
        if(!owner.paladinAttackCharge)
            charge = false;
    }
}
