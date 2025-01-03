using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
阵鬼 瘟疫附着检测
 */

public class Mechanism41 : BeMechanism
{
    protected int m_AttachBuffId = 0;                               //瘟疫附着buffID
    protected int m_AttachMaxNum = 0;                               //瘟疫最大附着数量
    protected List<int> m_BuffInfoList = new List<int>();           //需要添加的BuffInfo
    protected int m_EntityId = 0;                                   //瘟疫实体ID
    protected int m_AddEffectId = 0;                                //添加的触发效果ID     

    //protected BeEventHandle m_CollideHandle = null;                 //监听碰撞

    public Mechanism41(int sid, int skillLevel) : base(sid, skillLevel){}

    public override void OnReset()
    {
        m_BuffInfoList.Clear();
        m_AddEffectId = 0;
    }
    public override void OnInit()
    {
        m_AttachBuffId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        m_AttachMaxNum = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        if (data.ValueC.Count > 0)
        {
            for (int i = 0; i < data.ValueC.Count; i++)
            {
                int buffInfoId = TableManager.GetValueFromUnionCell(data.ValueC[i], level);
                m_BuffInfoList.Add(buffInfoId);
            }
        }
        
        m_EntityId = TableManager.GetValueFromUnionCell(data.ValueD[0],level);
        if (data.ValueE.Count > 0)
        {
            m_AddEffectId = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        }
    }

    public override void OnStart()
    {
        //监听碰撞的怪物
        //m_CollideHandle = owner.RegisterEvent(BeEventType.onCollideOther, (object[] args) => 
        //{

        //});
        handleA = OwnerRegisterEventNew(BeEventType.onCollideOther, _OnCollideOther);
    }

    private void _OnCollideOther(GameClient.BeEvent.BeEventParam param)
    {
        BeEntity target = param.m_Obj as BeEntity;
        OnCollide(target);
    }

    protected void OnCollide(BeEntity target)
    {
        if (target.GetCamp() != owner.GetCamp() && target.m_iResID == m_EntityId)
        {
            int buffCount = owner.buffController.GetBuffCountByID(m_AttachBuffId);
            if (buffCount < 3)
            {
                if (target.GetOwner().GetOwner() != null)
                {
                    target.GetOwner().GetOwner().DoAttackTo(owner, m_AddEffectId);
                }
                target.DoDead();
                if (m_BuffInfoList.Count > 0)
                {
                    for (int i = 0; i < m_BuffInfoList.Count; i++)
                    {
                        owner.buffController.TryAddBuff(m_BuffInfoList[i]);
                    }
                }
            }
        }
    }

    //public override void OnFinish()
    //{
    //    if (m_CollideHandle != null)
    //    {
    //        m_CollideHandle.Remove();
    //    }
    //}
} 