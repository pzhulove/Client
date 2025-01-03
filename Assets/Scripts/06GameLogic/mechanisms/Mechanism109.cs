using System;
using System.Collections.Generic;
using UnityEngine;

//增加或者减少触发效果表中或者BUFF表中的召唤怪物数量、召唤怪物数量上限（固定值）
class Mechanism109 : BeMechanism
{
    int effectId;
    int buffId;
    int addNum;
    int addNumLimit;

    public Mechanism109(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        effectId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        buffId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        addNum = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        addNumLimit = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
    }

    public override void OnStart()
    {
        handleB = owner.RegisterEventNew(BeEventType.onChangeSummonNumLimit, args =>
        {
            int eId = args.m_Int;
            int bId = args.m_Int2;
            if (effectId == eId || buffId == bId)
            {
                args.m_Int3 += addNumLimit;
                args.m_Int4 += addNum;
            }
        });
    }

}