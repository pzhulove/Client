using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 修改触发效果的时间
/// </summary>
public class Mechanism137 : BeMechanism
{
    List<int> list = new List<int>();
    int time = 0;
    public Mechanism137(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        list.Clear();
    }
    public override void OnInit()
    {
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            list.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }
        time = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }
    public override void OnStart()
    {
        base.OnStart();

        handleA = owner.RegisterEventNew(BeEventType.OnChangeEffectTime, args =>
        {
            int id = args.m_Int;
            if (list.Contains(id))
            {
                args.m_Int2 = time;
            }
        });
    }
}
