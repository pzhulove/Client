using UnityEngine;
using System.Collections;

public class Skill2110 : BeSkill
{
    int buffID;
    IBeEventHandle handle = null;
    public Skill2110(int sid, int skillLevel):base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
        buffID = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
    }

    void RemoveHandle()
    {
        if (handle != null)
        {
            handle.Remove();
            handle = null;
        }
    }

    public override void OnPostInit()
    {
        RemoveHandle();
        handle = owner.RegisterEventNew(BeEventType.onSummon, (args) =>
        {
            var monster = args.m_Obj as BeActor;
            if (monster != null && monster.m_iCamp == owner.m_iCamp && monster.GetEntityData().isSummonMonster)
            {
                monster.buffController.TryAddBuff(buffID, -1, level);
            }
        });
    }
}
