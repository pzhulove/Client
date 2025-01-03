using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 改变怪物固定血量
/// </summary>
public class Mechanism150 : BeMechanism
{
    int monsterID = 0;
    int addHp = 0;
    VFactor addHpRate = VFactor.zero;
    public Mechanism150(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        base.OnInit();
        monsterID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        addHpRate = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(data.ValueC[0], level), GlobalLogic.VALUE_1000);
        addHp = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onSummon, (args) => 
        {
            BeActor monster = args.m_Obj as BeActor;
            if (monster != null && monster.GetEntityData().MonsterIDEqual(monsterID))
            {
                int maxHp = monster.GetEntityData().GetMaxHP();
                if (addHpRate != VFactor.zero)
                {
                    monster.GetEntityData().SetHP(maxHp * (VFactor.one + addHpRate));
                    monster.GetEntityData().SetMaxHP(maxHp * (VFactor.one + addHpRate));
                }
                if (addHp != 0)
                {
                    monster.GetEntityData().SetHP(maxHp + addHp );
                    monster.GetEntityData().SetMaxHP(maxHp + addHp);
                }
            }
        });
    }
}
