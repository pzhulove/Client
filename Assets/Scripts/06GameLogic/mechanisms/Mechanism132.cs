using System;
using System.Collections.Generic;
using UnityEngine;

//每受到一定的伤害就触发某效果
class Mechanism132 : BeMechanism
{
    int hpReduce;
    int[] effectIdArray;

    int hpTotal;

    public Mechanism132(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        hpReduce = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        if (hpReduce <= 0) hpReduce = 1000;
        effectIdArray = new int[data.ValueB.Length];
        for (int i = 0; i < data.ValueB.Length; i++)
            effectIdArray[i] = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
    }

    public override void OnStart()
    {
        hpTotal = 0;
        handleA = owner.RegisterEventNew(BeEventType.onHPChange, args =>
        {
            int value = args.m_Int;
            hpTotal += value;
            while (-hpTotal > hpReduce)
            {
                hpTotal += hpReduce;
                for (int i = 0; i < effectIdArray.Length; i++)
                {
                    owner.DealEffectFrame(owner, effectIdArray[i]);
                }
            }
        });
    }
}