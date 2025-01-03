using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
    蛛网束缚
 */
public class Mechanism31 : BeMechanism 
{
    protected int m_MonsterId = 0;  //召唤的蜘蛛网ID
    protected List<int> m_BuffInfoIdList = new List<int>();    //给玩家上的buff信息
    protected List<int> m_BuffIdList = new List<int>(); // 需要移除的BuffID
    protected BeActor m_Monster = null; //蜘蛛网
    protected VInt3 m_OrigionOPos = VInt3.zero;       //蛛网位置
    protected IBeEventHandle m_MonsterStateHandle = null;
	protected IBeEventHandle m_OwnerDeadHandle = null;
    protected int m_MonserLevel = 0;                  //蛛网等级
    private bool m_needChangePos = true;
    public Mechanism31(int mid, int lv):base(mid, lv)
	{
        //m_MonserLevel = lv;
    }

    public override void OnReset()
    {
        m_MonsterId = 0;
        m_BuffInfoIdList.Clear();
        m_BuffIdList.Clear();
        m_Monster = null;
        m_OrigionOPos = VInt3.zero;
        m_MonsterStateHandle = null;
        m_OwnerDeadHandle = null;
        m_MonserLevel = 0;  
        m_needChangePos = true;
    }

    public override void OnInit()
    {
        m_MonserLevel = this.level;
        m_MonsterId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        for (int i = 0; i < data.ValueB.Count; i++)
        {
            var buffId = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
            if (buffId > 0)
                m_BuffIdList.Add(buffId);
        }

        for (int j = 0; j < data.ValueC.Count; j++)
        {
            var buffInfoId = TableManager.GetValueFromUnionCell(data.ValueC[j], level);
            if (buffInfoId > 0)
                m_BuffInfoIdList.Add(buffInfoId);
        }
        if (data.ValueD.Length > 0)
        {
            m_needChangePos = TableManager.GetValueFromUnionCell(data.ValueD[0], level) != 0 ? false : true;
        }
    }

    public override void OnStart()
    {
        DoAction();
    }

    //执行机制相关功能
    protected void DoAction()
    {
        AddBuffInfoToOwner();
        int camp = owner.m_iCamp == 0 ? 1 : 0;
        //召唤蜘蛛网
        m_Monster = owner.CurrentBeScene.SummonMonster(m_MonsterId, owner.GetPosition(), camp, null, false, m_MonserLevel);
        if (m_Monster != null)
        {
			if (m_MonsterStateHandle != null)
			{
				m_MonsterStateHandle.Remove();
			}
            m_MonsterStateHandle = m_Monster.RegisterEventNew(BeEventType.onDead, eventParam =>
            {
                RemoveFunction();   //蛛网死亡
            });
        }
		if (m_OwnerDeadHandle != null) 
		{
			m_OwnerDeadHandle.Remove ();
		}
		m_OwnerDeadHandle = owner.RegisterEventNew(BeEventType.onDead, eventParam =>
		{
			if(m_Monster!=null&&!m_Monster.IsDead())
			{
				m_Monster.DoDead();
			}
		});

        m_OrigionOPos = m_Monster.GetPosition();
        if (m_needChangePos)
            owner.SetPosition(new VInt3(m_OrigionOPos.x, m_OrigionOPos.y, -10 * VInt.one.i));
    }

    //给玩家上BuffInfo
    protected void AddBuffInfoToOwner()
    {
        for (int i = 0; i < m_BuffInfoIdList.Count; i++)
        {
            owner.buffController.TryAddBuff(m_BuffInfoIdList[i]);
        }
    }

    //移除机制相关
    protected void RemoveFunction()
    {
        for (int i = 0; i < m_BuffIdList.Count; i++)
        {
            owner.buffController.RemoveBuff(m_BuffIdList[i]);
        }
        owner.SetPosition(m_OrigionOPos);
    }
}
