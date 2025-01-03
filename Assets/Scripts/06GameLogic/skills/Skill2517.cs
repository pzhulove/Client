using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill2517 : Skill2522
{

    public Skill2517(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnStart()
    {
        curFrameFlag = "251701";
        effectId = BattleMain.IsModePvP(owner.battleType) ? 25171 : 25170;
        base.OnStart();
    }
}
