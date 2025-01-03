using UnityEngine;
using System.Collections.Generic;
using GameClient;

/*
 * 增加/减少某一触发效果ID的多段攻击间隔百分比及最大攻击次数
*/
public class Mechanism61 : BeMechanism
{
    protected int m_AttackIntervalAdd = 0;                                  //重复攻击间隔增减百分比
    protected List<int> m_HurtIdList = new List<int>();                     //触发效果id列表
    protected int m_AttackCountAdd;                                         //最大攻击次数增减

    public Mechanism61(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        m_HurtIdList.Clear();
        
    }
    public override void OnInit()
    {
        m_AttackIntervalAdd = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        if (data.ValueB.Count > 0)
        {
            for (int i = 0; i < data.ValueB.Count; i++)
            {
                int hurtId = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
                m_HurtIdList.Add(hurtId);
            }
        }
        m_AttackCountAdd = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
    }

    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onRepeatAttackInterval, (args) =>
        {
            if (m_HurtIdList.Count > 0 && m_HurtIdList.Contains(args.m_Int3))
            {
                args.m_Int += m_AttackIntervalAdd;
                args.m_Int2 += m_AttackCountAdd;
            }
        });
    }

}
