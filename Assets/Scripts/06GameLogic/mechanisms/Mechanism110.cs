using System;
using System.Collections.Generic;
using UnityEngine;

//增加或者减少某个触发效果ID的伤害（千分比）
class Mechanism110 : BeMechanism
{
    int[] effectIdArray;
    int addDamageRate;
    int addDamageFixedRate;

    public Mechanism110(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        effectIdArray = new int[data.ValueA.Length];
        for (int i = 0; i < data.ValueA.Length; i++)
            effectIdArray[i] = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
        addDamageRate = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        addDamageFixedRate = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
    }

    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onChangeDamage, args =>
        {
            int eId = args.m_Int;
            if (Array.IndexOf(effectIdArray, eId) != -1)
            {
                /*var damageRateArray = (int[])args[1];
                damageRateArray[0] += addDamageRate;
                damageRateArray[1] += addDamageFixedRate;*/
                args.m_Int2 += addDamageRate;
                args.m_Int3 += addDamageFixedRate;
            }
        });
    }

}