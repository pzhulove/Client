using System;
using System.Collections.Generic;
using UnityEngine;

//增加或者减少BUFF信息表中BUFF范围半径和BUFF目标选择范围（千分比）
class Mechanism114 : BeMechanism
{
    int[] buffinfoIdArray;
    int targetRadiusRate;
    int rangeRadiusRate;
    int monsterId;

    public Mechanism114(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        buffinfoIdArray = new int[data.ValueA.Length];
        for (int i = 0; i < data.ValueA.Length; i++)
            buffinfoIdArray[i] = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
        rangeRadiusRate = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        targetRadiusRate = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        monsterId = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
    }

    public override void OnStart()
    {
        if (monsterId == 0)
        {
            handleA = owner.RegisterEventNew(BeEventType.onChangeBuffRangeRadius, args =>
            {
                var bId = (int)args.m_Int;
                if (Array.IndexOf(buffinfoIdArray, bId) != -1)
                {
                    args.m_Int2 += rangeRadiusRate;
                }
            });
            handleB = owner.RegisterEventNew(BeEventType.onChangeBuffTargetRadius, args =>
            {
                var bId = (int)args.m_Int;
                if (Array.IndexOf(buffinfoIdArray, bId) != -1)
                {
                    args.m_Int2 += targetRadiusRate;
                }
            });
        }
        else
        {
            handleA = owner.RegisterEventNew(BeEventType.onSummon, args =>
            {
                var actor = args.m_Obj as BeActor;
                if (actor != null && actor.GetEntityData().MonsterIDEqual(monsterId))
                {
                    actor.RegisterEventNew(BeEventType.onChangeBuffRangeRadius, args1 =>
                    {
                        var bId = (int)args1.m_Int;
                        if (Array.IndexOf(buffinfoIdArray, bId) != -1)
                        {
                            args1.m_Int2 += rangeRadiusRate;
                        }
                    });
                    actor.RegisterEventNew(BeEventType.onChangeBuffTargetRadius, args2 =>
                    {
                        var bId = args2.m_Int;
                        if (Array.IndexOf(buffinfoIdArray, bId) != -1)
                        {
                            args2.m_Int2 += targetRadiusRate;
                        }
                    });
                }
            });
        }
    }
}