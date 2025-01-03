using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 查理监听机甲部件死亡

 */
public class Mechanism30 : BeMechanism 
{
    protected List<int> m_MonsterIdList = new List<int>();      //怪物ID列表
    protected int m_HpPercent = 0;                              //监听血量千分比
    protected List<int> m_BuffIdList = new List<int>();         //需要移除的BuffID列表
    protected int m_SkillId = 0;                                //需要执行的技能ID
    protected bool Z_MustUseSkill = false;      //必须释放技能 否则一直轮询(默认False,1表示必须释放)

    protected bool m_EffectFlag = false;                        //执行标志
    protected VFactor m_HpRate = VFactor.zero;                  //血量比例
    protected bool m_UseSkillFalg = false;  //释放技能标志

    public Mechanism30(int mid, int lv):base(mid, lv){}

    public override void OnReset()
    {
        m_MonsterIdList.Clear();
        m_HpPercent = 0;
        m_BuffIdList.Clear();
        m_SkillId = 0; 
        Z_MustUseSkill = false;
        m_EffectFlag = false; 
        m_HpRate = VFactor.zero;
        m_UseSkillFalg = false;
    }

    public override void OnInit()
    {
        for (int i = 0; i < data.ValueA.Count; ++i)
        {
            int mid = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
            m_MonsterIdList.Add(mid);
        }

        m_HpPercent = TableManager.GetValueFromUnionCell(data.ValueB[0],level);

        if (data.ValueC.Count > 0)
        {
            for (int j = 0; j < data.ValueC.Count; ++j)
            {
                int mid = TableManager.GetValueFromUnionCell(data.ValueC[j], level);
                m_BuffIdList.Add(mid);
            }
        }

        if (data.ValueD.Count > 0)
        {
            m_SkillId = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        }

        Z_MustUseSkill = TableManager.GetValueFromUnionCell(data.ValueE[0], level) == 0 ? false : true;
    }

    public override void OnStart()
    {
        m_EffectFlag = false;
        m_UseSkillFalg = false;
        m_HpRate = new VFactor(m_HpPercent, VInt.Float2VIntValue(0.1f));
    }

    public override void OnUpdate(int deltaTime)
    {
        if (!Z_MustUseSkill)
        {
            if (!m_EffectFlag)
            {
                m_EffectFlag = CheckMonsterHpRate();

                if (m_EffectFlag)
                {
                    DoMonsterAction();
                }
            }
        }
        else
        {
            //扩展机制 保证技能一定能释放成功一次
            if (CheckMonsterHpRate() && !m_UseSkillFalg)
            {
                DoUseSkillNew();
            }
        }
    }

    //检测怪物血量百分比
    protected bool CheckMonsterHpRate()
    {
        List<BeActor> actorList = GamePool.ListPool<BeActor>.Get();
        for (int i = 0; i < m_MonsterIdList.Count; i++)
        {
            if (m_HpRate == 0)
            {
                if (owner.CurrentBeScene.CheckMonsterAlive(m_MonsterIdList[i]))
                {
                    GamePool.ListPool<BeActor>.Release(actorList);
                    return false;
                }
            }
            else
            {
                owner.CurrentBeScene.FindActorById(actorList,m_MonsterIdList[i]);
                if(actorList.Count>0)
                {
                    if(actorList[0].GetEntityData().GetHPRate()>m_HpRate)
                    {
                        GamePool.ListPool<BeActor>.Release(actorList);
                        return false;
                    }
                }
            }
        }
        GamePool.ListPool<BeActor>.Release(actorList);
        return true;
    }

    //执行操作
    protected void DoMonsterAction()
    {
        //移除相关buff
        for (int i = 0; i < m_BuffIdList.Count; i++)
        {
            owner.buffController.RemoveBuff(m_BuffIdList[i]);
        }

        //执行相关技能
        if (owner.aiManager.CanAIUseSkill(m_SkillId) && owner.CanUseSkill(m_SkillId))
        {
            owner.UseSkill(m_SkillId);
        }
    }

    /// <summary>
    /// 释放技能 适用拓展后的机制 不会移除Buff
    /// </summary>
    protected void DoUseSkillNew()
    {
        if (owner.CanUseSkill(m_SkillId))
        {
            m_UseSkillFalg = true;
            owner.UseSkill(m_SkillId);
        }
    }
}
