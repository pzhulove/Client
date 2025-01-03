using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
Buff的释放者间隔一段时间给Buff的拥有者造成一次触发效果ID伤害
 */

public class Mechanism45 : BeMechanism
{
    protected int m_BuffId = 0;                 //机制对应的BuffId
    protected int m_TimeAcc = 0;                //伤害间隔时间
    protected int m_HurtId = 0;                 //触发效果ID
    protected int m_HurtNumMax = -1;             //最大伤害次数

    protected int m_CurrTimeAcc = 0;            //当前时间间隔
    protected int m_CurrHurtNum = 0;            //当前伤害次数

    public Mechanism45(int sid, int skillLevel) : base(sid, skillLevel){}

    public override void OnReset()
    {
        m_HurtNumMax = -1;
        m_CurrTimeAcc = 0; 
        m_CurrHurtNum = 0;
    }
    public override void OnInit()
    {
        m_BuffId = TableManager.GetValueFromUnionCell(data.ValueA[0],level);
        m_TimeAcc = TableManager.GetValueFromUnionCell(data.ValueB[0],level);
        m_HurtId = TableManager.GetValueFromUnionCell(data.ValueC[0],level);
        if (data.ValueD.Count > 0)
        {
            m_HurtNumMax = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        }
    }

    public override void OnUpdate(int deltaTime)
    {
        if (m_HurtNumMax == -1 || m_CurrHurtNum < m_HurtNumMax)
        {
            if (m_CurrTimeAcc > m_TimeAcc)
            {
                m_CurrTimeAcc = 0;
                DoAttack();
            }
            else
            {
                m_CurrTimeAcc += deltaTime;
            }
        }
    }

    //造成伤害
    protected void DoAttack()
    {
        BeBuff buff = owner.buffController.HasBuffByID(m_BuffId);
        BeActor m_Releaser = null;
        if (buff != null)
        {
            m_Releaser = buff.releaser;
        }
        if (m_Releaser != null)
        {
            m_Releaser.DoAttackTo(owner, m_HurtId);
            m_CurrHurtNum++;
        }
    }
}