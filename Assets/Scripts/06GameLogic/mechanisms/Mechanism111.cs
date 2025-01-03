using System;
using System.Collections.Generic;
using UnityEngine;

//增加或者减少实体配置中BOOMRANGE的停留时间
class Mechanism111 : BeMechanism
{
    int[] skillIdArray;
    int addStayDuration;

    public Mechanism111(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        skillIdArray = new int[data.ValueA.Length];
        for (int i = 0; i < data.ValueA.Length; i++)
            skillIdArray[i] = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
        addStayDuration = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onChangeBoomerangStayDuration, args =>
        {
            int sId = args.m_Int;
            if (Array.IndexOf(skillIdArray, sId) != -1)
            {
                args.m_Int2 += addStayDuration;
            }
        });
    }

}