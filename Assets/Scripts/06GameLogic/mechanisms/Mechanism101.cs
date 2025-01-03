using System;
using System.Collections.Generic;
using UnityEngine;

//当buff移除，某buff信息进CD
class Mechanism101 : BeMechanism
{
    int buffId;
    int[] buffInfoIdArray;

    public Mechanism101(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        buffId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        buffInfoIdArray = new int[data.ValueB.Length];
        for (int i = 0; i < data.ValueB.Length; i++)
        {
            buffInfoIdArray[i] = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
        }
    }

    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onRemoveBuff, args =>
        {
            for (int i = 0; i < buffInfoIdArray.Length; i++)
            {
                var buffInfo = new BuffInfoData(buffInfoIdArray[i]);
                buffInfo = owner.buffController.GetTriggerBuff(buffInfo);
                if (buffInfo != null)
                    buffInfo.StartCD();
            }
        });
    }

}