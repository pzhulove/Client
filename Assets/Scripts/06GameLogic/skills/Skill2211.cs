using System;
using UnityEngine;
using System.Collections.Generic;
using GameClient;
using ProtoTable;

/// <summary>
/// 秘术师-极冰盛宴
/// </summary>
public class Skill2211 : BeSkill
{
    protected List<int> m_MonsterIdList = new List<int>();          //极冰盛宴怪物ID（PVE,PVE蓄力，PVP，PVP蓄力）
    protected int m_AutoCancelTime = 10000;                         //主动取消技能时间

    protected IBeEventHandle m_SummonHandle = null;                  //监听召唤事件
    protected IBeEventHandle m_AddBuffHandle = null;                 //监听添加Buff
    protected int[] m_BuffIdArray = new int[4] { 221100, 221101, 221102, 221103 };  //极冰盛宴机制BuffId
    protected int[] m_MechanismIdArray = new int[4] { 5007, 5008, 5009, 5010 };     //极冰盛宴机制ID数组

    protected BeActor m_Monster = null;                             //极冰盛宴怪物

    public Skill2211(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
        m_MonsterIdList.Clear();
        if (skillData.ValueA.Count > 0)
        {
            for (int i = 0; i < skillData.ValueA.Count; i++)
            {
                int monsterId = TableManager.GetValueFromUnionCell(skillData.ValueA[i], level);
                m_MonsterIdList.Add(monsterId);
            }
        }
        m_AutoCancelTime = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
    }

    public override void OnStart()
    {
        RemoveHandle();
        m_Monster = null;

        m_SummonHandle = owner.RegisterEventNew(BeEventType.onSummon, (args) =>
        {
            //int monsterId = GetMonsterId();
           // if (monsterId != 0)
            {
                BeActor monster = (BeActor)args.m_Obj;
                //if (monster.GetEntityData().monsterID.Equals(monsterId))
                if(m_MonsterIdList.Contains(monster.GetEntityData().monsterID))
                {
                    m_Monster = monster;
                    m_AddBuffHandle = m_Monster.RegisterEventNew(BeEventType.onAddBuff, (args2) =>
                    {
                        BeBuff buff = (BeBuff)args2.m_Obj;
                        //if (buff.buffID == GetBuffId() && m_Monster!=null)
                        if (Array.IndexOf(m_BuffIdArray, buff.buffID)!= -1 && m_Monster!=null)
                        {
                            //Mechanism23 mechanism = m_Monster.GetMechanism(GetMechanismId()) as Mechanism23;
                            BeMechanism baseMechanism = m_Monster.GetMechanismByIndex(23);
                            if (baseMechanism != null)
                            {
                                var mechanism = baseMechanism as Mechanism23;
                                if (mechanism != null)
                                {
                                    mechanism.SetAttackTimeAcc(GetSkillSpeedFactor());
                                }
                            }
                            
                        }
                    });
                }
            }
        });
    }

    public override void OnCancel()
    {
        SetMonsterDead();
    }

    public override void OnFinish()  
    {
        SetMonsterDead();
    }

    protected void SetMonsterDead()
    {
        if (m_Monster != null && !m_Monster.IsDeadOrRemoved() && m_Monster.GetOwner() == owner)
        {
            m_Monster.DoDead();
            m_Monster = null;
        }
        /*int monsterId = GetMonsterId();
        if (monsterId != 0)
        {
            List<BeActor> monsterList = GamePool.ListPool<BeActor>.Get();
            owner.CurrentBeScene.FindActorById2(monsterList, monsterId);
            if (monsterList.Count > 0)
            {
                for(int i = 0; i < monsterList.Count; i++)
                {
                    BeActor actor = monsterList[i];
                    if(actor!=null && !actor.IsDead() && actor.GetOwner()==owner)
                    {
                        actor.DoDead();
                    }
                }
            }
            GamePool.ListPool<BeActor>.Release(monsterList);
        }*/
    }

    /*protected int GetMonsterId()
    {
        int monsterId = 0;
        if (BattleMain.IsModePvP(battleType))
        {
            monsterId = charged ? m_MonsterIdList[3] : m_MonsterIdList[2];
        }
        else
        {
            monsterId = charged ? m_MonsterIdList[1] : m_MonsterIdList[0];
        }
        return monsterId;
    }

    protected int GetMechanismId()
    {
        int mechanismId = 0;
        if (BattleMain.IsModePvP(battleType))
        {
            mechanismId = charged ? m_MechanismIdArray[3] : m_MechanismIdArray[2];
        }
        else
        {
            mechanismId = charged ? m_MechanismIdArray[1] : m_MechanismIdArray[0];
        }
        return mechanismId;
    }

    protected int GetBuffId()
    {
        int buffId = 0;
        if (BattleMain.IsModePvP(battleType))
        {
            buffId = charged ? m_BuffIdArray[3] : m_BuffIdArray[2];
        }
        else
        {
            buffId = charged ? m_BuffIdArray[1] : m_BuffIdArray[0];
        }
        return buffId;
    }*/

    protected void RemoveHandle()
    {
        if (m_SummonHandle != null)
        {
            m_SummonHandle.Remove();
            m_SummonHandle = null;
        }

        if (m_AddBuffHandle != null)
        {
            m_AddBuffHandle.Remove();
            m_AddBuffHandle = null;
        }
    }
}

public class Skill9740:BeSkill
{
    protected IBeEventHandle m_SummonHandle = null;
    protected BeActor m_Monster = null;
    public Skill9740(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override void OnStart()
    {
        RemoveHandle();
        m_Monster = null;

        m_SummonHandle = owner.RegisterEventNew(BeEventType.onSummon, (args) =>
        {
                BeActor monster = (BeActor)args.m_Obj;
                m_Monster = monster;                                 
            
        });
    }

    public override void OnCancel()
    {
        SetMonsterDead();
        RemoveHandle();
    }

    public override void OnFinish()
    {
        SetMonsterDead();
        RemoveHandle();
    }

    private void SetMonsterDead()
    {
        if (m_Monster != null && !m_Monster.IsDead() && m_Monster.GetOwner() == owner)
        {
            m_Monster.DoDead();
            m_Monster = null;
        }
    }

    protected void RemoveHandle()
    {
        if (m_SummonHandle != null)
        {
            m_SummonHandle.Remove();
            m_SummonHandle = null;
        }

    }
}
