using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 秘术师-黑洞
/// </summary>
public class Skill2210 : BeSkill
{
    protected int[] m_MonsterIdArray = new int[4];          //黑洞怪物ID
    protected int[] m_CanBoomTime = new int[2];             //延时引爆CD（PVE|PVP)
    protected int[] m_BoomEntityId = new int[2];            //爆炸实体ID(不蓄力|蓄力)

    protected bool m_CanBoom = false;
    protected IBeEventHandle m_MonsterBirthHandle = null;
    protected IBeEventHandle m_MonsterDeadHandle = null;     //监听黑洞怪物死亡
    protected IBeEventHandle m_PassDoorHandle = null;        //监听过门
    protected IBeEventHandle m_EnterNextLayerHandle = null;  //监听爬塔
    protected BeActor m_BlackHoleMonster = null;

    protected DelayCallUnitHandle m_DelayCall;
         

    public Skill2210(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
        if (skillData.ValueA.Count > 0)
        {
            for (int i = 0; i < skillData.ValueA.Count; i++)
            {
                int monsterId = TableManager.GetValueFromUnionCell(skillData.ValueA[i], level);
                m_MonsterIdArray[i] = monsterId;
            }
        }

        if (skillData.ValueB.Count > 0)
        {
            for (int i = 0; i < skillData.ValueB.Count; i++)
            {
                int time = TableManager.GetValueFromUnionCell(skillData.ValueB[i], level);
                m_CanBoomTime[i] = time;
            }
        }

        if (skillData.ValueC.Count > 0)
        {
            for (int i = 0; i < skillData.ValueC.Count; i++)
            {
                int entityId = TableManager.GetValueFromUnionCell(skillData.ValueC[i], level);
                m_BoomEntityId[i] = entityId;
            }
        }
    }

    public override void OnStart()
    {
        if (GetBlackHoleMonsterId() != 0)
        {
            m_CanBoom = false;
            m_BlackHoleMonster = null;
            RemoveHandle();
#if !LOGIC_SERVER
            if (button != null)
            {
                button.RemoveEffect(ETCButton.eEffectType.onContinue);
            }
#endif
            m_MonsterBirthHandle = owner.RegisterEventNew(BeEventType.onSummon, (args) =>
            {
            //    BeActor actor = (BeActor)args[0];
               BeActor actor = args.m_Obj as BeActor;
                if (actor.GetEntityData().MonsterIDEqual(GetBlackHoleMonsterId()))
                {
                    m_BlackHoleMonster = actor;
                    if (m_BlackHoleMonster == null)
                    {
                        skillButtonState = SkillState.NORMAL;
                    }
                    else
                    {
                        int time = BattleMain.IsModePvP(battleType) ? m_CanBoomTime[1] : m_CanBoomTime[0];
                        m_DelayCall.SetRemove(true);
                        m_DelayCall = m_BlackHoleMonster.delayCaller.DelayCall(time, () =>
                        {
                            m_CanBoom = true;
#if !LOGIC_SERVER
                            if (button != null)
                            {
                                button.AddEffect(ETCButton.eEffectType.onContinue);
                            }
#endif
                        });

                        m_MonsterDeadHandle = m_BlackHoleMonster.RegisterEventNew(BeEventType.onDead, (arg) =>
                        {
                            skillButtonState = SkillState.NORMAL;
                            if (m_BlackHoleMonster != null)
                            {
                                VInt3 boomPos = m_BlackHoleMonster.GetPosition();
                                CreateBoomEntity(boomPos);
                            }
#if !LOGIC_SERVER
                            if (button != null)
                            {
                                button.RemoveEffect(ETCButton.eEffectType.onContinue);
                            }
#endif
                        });

                    }
                }
            });

            m_PassDoorHandle = owner.RegisterEventNew(BeEventType.onStartPassDoor, (args) => 
            {
                ClearDeadHandle();
            });

            m_EnterNextLayerHandle = owner.RegisterEventNew(BeEventType.onDeadTowerEnterNextLayer, (args) => 
            {
                ClearDeadHandle();
            });
        }
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

        m_DelayCall.SetRemove(true);
    }

    public override void OnClickAgain()
    {
        if (GetBlackHoleMonsterId() != 0)
        {
            SetBlackHoleDead();
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

    protected void End()
    {
        if (m_BlackHoleMonster == null)
        {
            skillButtonState = SkillState.NORMAL;
        }
    }

    //引爆黑洞
    protected void SetBlackHoleDead()
    {
        if (!m_CanBoom)
            return;
        if (m_BlackHoleMonster != null)
        {
            m_BlackHoleMonster.DoDead();
        }
    }

    //创建爆炸实体
    protected void CreateBoomEntity(VInt3 pos)
    {
        owner.AddEntity(GetBoomEntityId(), pos);
    }

    //获取爆炸实体ID
    protected int GetBoomEntityId()
    {
        int monsterId = charged ? m_BoomEntityId[1] : m_BoomEntityId[0];
        return monsterId;
    }


    protected int GetBlackHoleMonsterId()
    {
        int boomEntityId = 0;
        if (BattleMain.IsModePvP(battleType))            //PVP模式
        {
            boomEntityId = charged ? m_MonsterIdArray[3] : m_MonsterIdArray[2];
        }
        else
        {
            boomEntityId = charged ? m_MonsterIdArray[1] : m_MonsterIdArray[0];
        }
        return boomEntityId;
    }

    protected void RemoveHandle()
    {
        if (m_MonsterBirthHandle != null)
        {
            m_MonsterBirthHandle.Remove();
            m_MonsterBirthHandle = null;
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

        if (m_EnterNextLayerHandle != null)
        {
            m_EnterNextLayerHandle.Remove();
            m_EnterNextLayerHandle = null;
        }
    }
}
