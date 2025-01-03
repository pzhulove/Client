using UnityEngine;
using System.Collections.Generic;
using GameClient;

/*
 * 攻击别人伤害计算前给自己添加一个Buff
*/
public class Mechanism72 : BeMechanism
{
    protected List<int> m_EntityResIdList = new List<int>();                                //实体ID
    protected List<int> m_BuffInfoIdList = new List<int>();         //添加的BuffInfoId

    protected IBeEventHandle m_OnHitOtherHandle = null;              //监听攻击到别人

    public Mechanism72(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        m_EntityResIdList.Clear();
        m_BuffInfoIdList.Clear();
        m_OnHitOtherHandle = null;
    }

    public override void OnInit()
    {
        if (data.ValueA.Count > 0)
        {
            for(int i = 0; i < data.ValueA.Count; i++)
            {
                int entityId = TableManager.GetValueFromUnionCell(data.ValueA[i],level);
                m_EntityResIdList.Add(entityId);
            }
        }
        if (data.ValueB.Count > 0)
        {
            for (int i = 0; i < data.ValueB.Count; i++)
            {
                int buffInfoId = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
                m_BuffInfoIdList.Add(buffInfoId);
            }
        }
    }

    public override void OnStart()
    {
        RemoveHandle();
        AttackHandle();
    }

    //监听攻击
    protected void AttackHandle()
    {
        m_OnHitOtherHandle = owner.RegisterEventNew(BeEventType.onBeforeHit,args =>
        //m_OnHitOtherHandle = owner.RegisterEvent(BeEventType.onBeforeHit, (object[] args) =>
        {
            int projectileResId = args.m_Int;
            if (m_EntityResIdList.Contains(projectileResId))
            {
                for (int i = 0; i < m_BuffInfoIdList.Count; i++)
                {
                    owner.buffController.TryAddBuff(m_BuffInfoIdList[i]);
                }
            }
        });
    }

    public override void OnFinish()
    {
        RemoveHandle();
    }

    protected void RemoveHandle()
    {
        if (m_OnHitOtherHandle != null)
        {
            m_OnHitOtherHandle.Remove();
            m_OnHitOtherHandle = null;
        }
    }
}
