using System;
using System.Collections.Generic;
using UnityEngine;

//增加或者减少触发效果表中和BUFF信息表中异常BUFF攻击力（千分比）
class Mechanism107 : BeMechanism
{
    int[] effectIdArray;
    int[] buffinfoIdArray;
    int buffAttackRate;
    int parentMechanism;

    public Mechanism107(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        effectIdArray = new int[data.ValueA.Length];
        for (int i = 0; i < data.ValueA.Length; i++)
            effectIdArray[i] = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
        buffinfoIdArray = new int[data.ValueB.Length];
        for (int i = 0; i < data.ValueB.Length; i++)
            buffinfoIdArray[i] = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
        buffAttackRate = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        parentMechanism = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
    }

    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onChangeBuffAttack, args =>
        {
            var source = (BeActor)args.m_Obj;
            if (parentMechanism != 0 && source.GetMechanism(parentMechanism) == null)
                return;

            int eId = args.m_Int;
            int bId = args.m_Int2;
            if (Array.IndexOf(effectIdArray, eId) != -1 || Array.IndexOf(buffinfoIdArray, bId) != -1)
            {
                /*var buffAttackArray = (int[])args[2];
                buffAttackArray[0] += buffAttackRate;*/
                args.m_Int3 += buffAttackRate;
            }
        });
    }

}