using System;
using System.Collections.Generic;
using UnityEngine;

//增加或者减少某个名字的属性操作力（千分比）
class Mechanism112 : BeMechanism
{
    int[] tagArray;
    int modifySpeedRate;

    public Mechanism112(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        tagArray = new int[data.ValueA.Length];
        for (int i = 0; i < data.ValueA.Length; i++)
            tagArray[i] = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
        modifySpeedRate = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onChangeModifySpeed, args =>
        {
            int tag = args.m_Int;
            if (Array.IndexOf(tagArray, tag) != -1)
            {
                args.m_Int2 += modifySpeedRate;
            }
        });
    }

}