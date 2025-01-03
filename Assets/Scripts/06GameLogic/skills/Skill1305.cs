using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 闪击地雷
/// </summary>
public class Skill1305 : BeSkill
{
    protected int m_CanBoomBuff = 0;                                        //引爆地雷标记Buff
    protected int[] m_BoomMonsterIdArray = new int[2];                      //地雷怪物ID（PVE|PVP）
    protected int[] m_BoomEntityIdArray = new int[2];                       //地雷爆炸召唤的实体ID（PVE|PVP）

    protected IBeEventHandle m_AddBuffHandle = null;                        //监听Buff添加
    protected IBeEventHandle m_SummonMonsterHandle = null;                  //监听地雷创建
    protected IBeEventHandle m_MonsterDeadHandle = null;                    //监听地雷爆炸
    protected BeActor m_SummonMonster = null;                               //召唤出来的地雷
    protected bool m_CanBoom = false;                                       //标识能否自动引爆
    protected VInt3 m_MonsterPos = VInt3.zero;                              //怪物位置

    protected IBeEventHandle m_PassDoorHandle = null;                       //监听过门
    protected IBeEventHandle m_PassTowerHandle = null;                      //监听爬塔

    public Skill1305(int sid, int skillLevel):base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
        m_CanBoomBuff = TableManager.GetValueFromUnionCell(skillData.ValueA[0],level);
        if (skillData.ValueB.Count > 0)
        {
            for(int i = 0; i < skillData.ValueB.Count; i++)
            {
                int monsterId = TableManager.GetValueFromUnionCell(skillData.ValueB[i],level);
                m_BoomMonsterIdArray[i] = monsterId;
            }
        }
        if (skillData.ValueC.Count > 0)
        {
            for(int i = 0; i < skillData.ValueC.Count; i++)
            {
                int entityId = TableManager.GetValueFromUnionCell(skillData.ValueC[i],level);
                m_BoomEntityIdArray[i] = entityId;
            }
        }
    }

    public override void OnStart()
    {
        InitData();
        RemoveHandle();
        m_AddBuffHandle = owner.RegisterEventNew(BeEventType.onAddBuff, (args) => 
        {
            BeBuff buff = (BeBuff)args.m_Obj;
            if (buff.buffID == m_CanBoomBuff)
            {
                CanBoom();
            }
        });

        m_PassDoorHandle = owner.RegisterEventNew(BeEventType.onStartPassDoor, (args) =>
        {
            ClearDeadHandle();
        });

        m_PassTowerHandle = owner.RegisterEventNew(BeEventType.onDeadTowerEnterNextLayer, (args) =>
        {
            ClearDeadHandle();
        });
        m_SummonMonsterHandle = owner.RegisterEventNew(BeEventType.onSummon, (args) => 
        {
            BeActor actor = (BeActor)args.m_Obj;
            if(actor!=null && !actor.IsDead())
            {
                if (actor.GetEntityData().MonsterIDEqual(GetMonsterId()))
                {
                    m_SummonMonster = actor;
                    m_MonsterPos = actor.GetPosition();
                    m_MonsterDeadHandle = actor.RegisterEventNew(BeEventType.onDead, eventParam =>
                    {
                        MonsterDead();
                    });
                }
            }
        });
    }

    public override void OnClickAgain()
    {
        if (m_CanBoom)
        {
            SetMonsterDead();
        }
    }

    public override void OnCancel()
    {
        End();
    }

    public override void OnFinish()
    {
        End();
    }

    protected void InitData()
    {
        m_MonsterPos = VInt3.zero;
        m_CanBoom = false;
        m_SummonMonster = null;
    }

    protected void End()
    {
        if (m_SummonMonster == null)
        {
            skillButtonState = SkillState.NORMAL;
        }
    }

    protected void CanBoom()
    {
        m_CanBoom = true;
#if !LOGIC_SERVER
        if (button != null)
        {
            button.AddEffect(ETCButton.eEffectType.onContinue);
        }
#endif
    }

    //清除黑洞怪物死亡监听
    protected void ClearDeadHandle()
    {
        skillButtonState = SkillState.NORMAL;
#if !LOGIC_SERVER
        if (button != null)
        {
            button.RemoveEffect(ETCButton.eEffectType.onContinue);
        }
#endif
        if (m_MonsterDeadHandle != null)
        {
            m_MonsterDeadHandle.Remove();
            m_MonsterDeadHandle = null;
        }
    }

    //将地雷引爆
    protected void SetMonsterDead()
    {
        if (m_SummonMonster != null)
        {
            m_SummonMonster.DoDead();
            m_SummonMonster = null;
        }
    }

    //怪物死亡
    protected void MonsterDead()
    {
        int entityId = BattleMain.IsModePvP(battleType) ? m_BoomEntityIdArray[1] : m_BoomEntityIdArray[0];
        owner.AddEntity(entityId, m_MonsterPos,level);
        skillButtonState = SkillState.NORMAL;
#if !LOGIC_SERVER
        if (button != null)
        {
            button.RemoveEffect(ETCButton.eEffectType.onContinue);
        }
#endif
    }

    //获取闪击地雷怪物ID
    protected int GetMonsterId()
    {
        int monsterId = BattleMain.IsModePvP(battleType)? m_BoomMonsterIdArray[1] : m_BoomMonsterIdArray[0];
        return monsterId;
    }

    protected void RemoveHandle()
    {
        if (m_AddBuffHandle != null)
        {
            m_AddBuffHandle.Remove();
            m_AddBuffHandle = null;
        }

        if (m_SummonMonsterHandle != null)
        {
            m_SummonMonsterHandle.Remove();
            m_SummonMonsterHandle = null;
        }

        if (m_MonsterDeadHandle != null)
        {
            m_MonsterDeadHandle.Remove();
            m_MonsterDeadHandle = null;
        }

        if (m_PassDoorHandle != null)
        {
            m_PassDoorHandle.Remove();
            m_PassDoorHandle = null;
        }

        if (m_PassTowerHandle != null)
        {
            m_PassTowerHandle.Remove();
            m_PassTowerHandle = null;
        }
    }
}
