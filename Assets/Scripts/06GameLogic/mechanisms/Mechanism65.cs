using UnityEngine;
using System.Collections.Generic;
using GameClient;

/*
 * 攻击别人伤害计算前给自己添加一个Buff
*/
public class Mechanism65 : BeMechanism
{
    protected int m_AttackType = -1;                                //攻击类型
    protected int m_MonsterType = 0;                                //Buff目标类型
    protected List<int> m_MonsterIdList = new List<int>();          //Buff目标怪物ID列表
    protected int m_BuffInfoId = 0;                                 //添加的BuffId
    protected List<int> skillIdList = new List<int>();              //受影响的技能列表

    protected IBeEventHandle m_OnHitOtherHandle = null;              //监听攻击到别人
    protected IBeEventHandle m_OnBeHitHandle = null;                 //监听被别人攻击

    public Mechanism65(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        m_MonsterIdList.Clear();
        skillIdList.Clear();
        m_OnHitOtherHandle = null; 
        m_OnBeHitHandle = null;
    }
    public override void OnInit()
    {
        m_AttackType = TableManager.GetValueFromUnionCell(data.ValueA[0],level);
        m_MonsterType = TableManager.GetValueFromUnionCell(data.ValueB[0],level);
        if (data.ValueC.Count > 0)
        {
            for(int i = 0; i < data.ValueC.Count; i++)
            {
                int monsterId = TableManager.GetValueFromUnionCell(data.ValueC[i],level);
                m_MonsterIdList.Add(monsterId);
            }
        }
        m_BuffInfoId = TableManager.GetValueFromUnionCell(data.ValueD[0],level);
        for(int i = 0; i < data.ValueE.Count; i++)
        {
            skillIdList.Add(TableManager.GetValueFromUnionCell(data.ValueE[i], level));
        }
    }

    public override void OnStart()
    {
        RemoveHandle();
        if (m_AttackType == 0)
        {
            AttackHandle();
        }
        else if (m_AttackType == 1)
        {
            OnBeHitHandle();
        }
    }

    //监听攻击
    protected void AttackHandle()
    {
        m_OnHitOtherHandle = owner.RegisterEventNew(BeEventType.onBeforeHit, args =>
        //m_OnHitOtherHandle = owner.RegisterEvent(BeEventType.onBeforeHit, (object[] args) =>
        {
            BeActor target = args.m_Obj as BeActor;
            if (target != null && !target.IsDead())
            {
                if (target.GetEntityData().type == m_MonsterType || (target.GetEntityData().isSummonMonster && m_MonsterType == Global.SUMMONMONSTERTYPE))      //对召唤兽类型进行特殊处理
                {
                    var hurtData = args.m_Obj2 as ProtoTable.EffectTable;
                    if (HaveMonster(target) && CheckSkillUseId(hurtData))
                    {
                        target.buffController.TryAddBuffInfo(m_BuffInfoId,owner,level);
                    }
                }
            }
        });
    }
    
    //监听被击
    protected void OnBeHitHandle()
    {
        m_OnBeHitHandle = owner.RegisterEventNew(BeEventType.onBeforeOtherHit, args =>
        //m_OnBeHitHandle = owner.RegisterEvent(BeEventType.onBeforeOtherHit, (object[] args) => 
        {
            BeActor attacker = args.m_Obj as BeActor;
            if (attacker != null && !attacker.IsDead())
            {
                if (attacker.GetEntityData().type == m_MonsterType || (attacker.GetEntityData().isSummonMonster && m_MonsterType == Global.SUMMONMONSTERTYPE))      //对召唤兽类型进行特殊处理
                {
                    var hurtData = args.m_Obj2 as ProtoTable.EffectTable;
                    if (HaveMonster(attacker) && CheckSkillUseId(hurtData))
                    {
                        owner.buffController.TryAddBuffInfo(m_BuffInfoId, owner,level);
                    }
                }
            }
        });
    }

    //判断列表中是否有这个怪物
    protected bool HaveMonster(BeActor monster)
    {
        if (m_MonsterIdList.Count <= 0)
            return true;
        bool haveMonster = false;
        for (int i=0;i< m_MonsterIdList.Count; i++)
        {
            int monsterId = m_MonsterIdList[i];
            if (monster.GetEntityData().MonsterIDEqual(m_MonsterIdList[i]))
            {
                haveMonster = true;
            }
        }
        return haveMonster;
    }

    /// <summary>
    /// 检查技能ID
    /// </summary>
    /// <returns></returns>
    protected bool CheckSkillUseId(ProtoTable.EffectTable hurtData)
    {
        if (skillIdList.Count <= 0)
            return true;
        if (hurtData == null)
            return true;
        if (skillIdList.Contains(hurtData.SkillID))
            return true;
        return false;
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

        if (m_OnBeHitHandle != null)
        {
            m_OnBeHitHandle.Remove();
            m_OnBeHitHandle = null;
        }
    }
}
