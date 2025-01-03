using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill1905 : BeSkill {

    IBeEventHandle handle = null;

    public Skill1905(int sid, int skillLevel) : base(sid, skillLevel)
    { }

    public override void OnPostInit()
    {
        RemoveHandle();
        handle = owner.RegisterEventNew(BeEventType.ConfigCommand, (args) => 
        {
            if( owner.GetEntityData() != null &&
                owner.GetEntityData().professtion == 11 && 
                owner.attackReplaceLigui && 
                !BattleMain.IsModePvP(owner.battleType))

            owner.GetEntityData().normalAttackID = 1901;
        });
    }

    void RemoveHandle()
    {
        if (handle != null)
        {
            handle.Remove();
            handle = null;
        }
    }
}
