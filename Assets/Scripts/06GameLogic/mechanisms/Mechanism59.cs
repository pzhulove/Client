using UnityEngine;
using System.Collections.Generic;
using GameClient;

/*
 * 修罗觉醒技能
*/
public class Mechanism59 : BeMechanism
{
    protected int m_HurtTagBuff = 0;                            //伤害标记BuffId
    protected int m_HurtId = 0;                                 //全屏伤害触发效果ID

    readonly protected int m_OrignPhaseId = 1502;                        //初始的普攻第三段ID
    readonly protected int m_OrignPhaseId2 = 1714;                       //初始的普攻第三段ID2
    readonly protected int m_ReplaceNormalId = 1719;                     //替换的普攻技能ID
    readonly protected int m_OrignGroundSkillId = 1510;                  //初始的地裂波动剑技能
    readonly protected int m_ReplaceGroundSkillId = 1720;                //替换的地裂波动剑技能
    readonly protected int m_OrignIceSkillId = 1702;                     //初始冰刃波动剑技能
    readonly protected int m_ReplaceIceSkillId = 1721;                   //替换的冰刃波动剑技能
    readonly protected int m_OrignFireSkillId = 1703;                    //初始的烈焰波动剑技能
    readonly protected int m_ReplaceFireSkillId = 1722;                  //替换的烈焰波动剑技能

    readonly protected int m_AXiuLuoJueXingId = 1718;                    //修罗觉醒技能ID  

    readonly protected int m_EntityId = 60323;                           //创建的实体ID

    protected IBeEventHandle m_ReplaceSkillHandle = null;        //替换技能
    protected IBeEventHandle m_ReplaceSkillPhaseHandle = null;   //替换技能阶段
    protected IBeEventHandle m_PassDoor = null;                  //监听过门
    protected IBeEventHandle m_AddBuffHandle = null;             //监听添加Buff

    protected GeEffectEx m_Effect = null;
    protected GeEffectEx m_Effect1 = null;

    protected float m_Offset = 0.0f;                            //火特效位置偏移
    protected float m_Offset1 = 4.4f;                           //填特效位置偏移
    protected float m_OffsetZ = 1.0f;                           //实体高度偏移

    protected int m_CurrTimeAcc = 0;                            //当前时间
    protected bool m_CreateEntityFlag = false;
    readonly protected int m_CreateEntityTime = 24900;                   //延时多久创建实体

    public Mechanism59(int mid, int lv) : base(mid, lv){}

    public override void OnReset()
    {
        m_ReplaceSkillHandle = null;
        m_ReplaceSkillPhaseHandle = null;
        m_PassDoor = null;    
        m_AddBuffHandle = null; 
        m_Effect = null;
        m_Effect1 = null;
        m_Offset = 0.0f; 
        m_Offset1 = 4.4f; 
        m_OffsetZ = 1.0f; 
        m_CurrTimeAcc = 0; 
        m_CreateEntityFlag = false;

    }
    public override void OnInit()
    {
        m_HurtTagBuff = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        m_HurtId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnStart()
    {
        RemoveHandle();
        ChangeSkillLevel();
        m_ReplaceSkillHandle = owner.RegisterEventNew(BeEventType.onReplaceSkill, param =>
        {
            int skillId = param.m_Int;
            BeSkill orignalSkill = owner.GetSkill(skillId);
            if (skillId == m_OrignGroundSkillId)
            {
                param.m_Int = m_ReplaceGroundSkillId;
            }
            else if (skillId == m_OrignIceSkillId)
            {
                param.m_Int = m_ReplaceIceSkillId;
            }
            else if (skillId == m_OrignFireSkillId)
            {
                param.m_Int = m_ReplaceFireSkillId;
            }
            if (orignalSkill != null)
            {
                orignalSkill.StartCoolDown();
            }
        });

        m_ReplaceSkillPhaseHandle = owner.RegisterEventNew(BeEventType.onPreSetSkillAction, (GameClient.BeEvent.BeEventParam param) =>
        {
            //int[] skillPhaseArray = (int[])args[0];
            if (param.m_Int == m_OrignPhaseId || param.m_Int == m_OrignPhaseId2)
            {
                param.m_Int = m_ReplaceNormalId;
            }
        });

        m_PassDoor = owner.RegisterEventNew(BeEventType.onPassedDoor, (args) =>
        {
            RefreshEffect();
        });

        m_AddBuffHandle = owner.RegisterEventNew(BeEventType.onAddBuff, (args) =>
        {
            BeBuff buff = (BeBuff)args.m_Obj;
            if (buff.buffID == m_HurtTagBuff)
            {
                DoHurtToAllEnemy();
            }
        });

        ChangeButtonEffect(m_OrignGroundSkillId, true);
        ChangeButtonEffect(m_OrignIceSkillId, true);
        ChangeButtonEffect(m_OrignFireSkillId, true);
        AddEffect(true);
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if(m_CurrTimeAcc >= m_CreateEntityTime && !m_CreateEntityFlag)
        {
            m_CreateEntityFlag = true;
            CreateEntity();
        }
        else
        {
            m_CurrTimeAcc += deltaTime;
        }
    }

    public override void OnFinish()
    {
        ChangeButtonEffect(m_OrignGroundSkillId, false);
        ChangeButtonEffect(m_OrignIceSkillId, false);
        ChangeButtonEffect(m_OrignFireSkillId, false);
        AddEffect(false);
        RemoveHandle();
    }

    //技能等级提升至修罗觉醒的等级
    protected void ChangeSkillLevel()
    {
        BeSkill jueXingSkill = owner.GetSkill(m_AXiuLuoJueXingId);
        if (jueXingSkill != null)
        {
            int level = jueXingSkill.level;
            UpdateSkillLevel(m_ReplaceNormalId, level);
            UpdateSkillLevel(m_ReplaceGroundSkillId, level);
            UpdateSkillLevel(m_ReplaceFireSkillId, level);
            UpdateSkillLevel(m_ReplaceIceSkillId, level);
        }
    }

    protected void UpdateSkillLevel(int skillId,int level)
    {
        if (owner.GetSkill(skillId) != null)
        {
            owner.GetSkill(skillId).level = level;
        }
    }


    protected void AddEffect(bool isAdd)
    {
#if !LOGIC_SERVER
        if (isAdd)
        {
            VInt3 centerPos = owner.CurrentBeScene.GetSceneCenterPosition();
            centerPos.z = (int)m_Offset * GlobalLogic.VALUE_10000;
            m_Effect = owner.CurrentBeScene.currentGeScene.CreateEffect(1024, centerPos.vec3);
            centerPos.z = (int)(m_Offset1 * GlobalLogic.VALUE_10000);
            m_Effect1 = owner.CurrentBeScene.currentGeScene.CreateEffect(1025, centerPos.vec3);
        }
        else
        {
            if (m_Effect != null)
            {
                owner.CurrentBeScene.currentGeScene.DestroyEffect(m_Effect);
                m_Effect = null;
            }
            if (m_Effect1 != null)
            {
                owner.CurrentBeScene.currentGeScene.DestroyEffect(m_Effect1);
                m_Effect = null;
            }
        }
#endif
    }

    protected void RefreshEffect()
    {
#if !LOGIC_SERVER
        VInt3 centerPos = owner.CurrentBeScene.GetSceneCenterPosition();
        centerPos.z = (int)(m_Offset * GlobalLogic.VALUE_10000);
        if (m_Effect != null)
        {
            m_Effect.SetPosition(centerPos.vector3);
        }
        centerPos.z = (int)(m_Offset1 * GlobalLogic.VALUE_10000);
        if (m_Effect1 != null)
        {
            m_Effect1.SetPosition(centerPos.vector3);
        }
#endif
    }

    //添加技能按钮特效
    protected void ChangeButtonEffect(int skillId, bool isAdd)
    {
#if !LOGIC_SERVER
        BeSkill skill = owner.GetSkill(skillId);
        if (skill != null && skill.button != null)
        {
            if (!isAdd)
            {
                skill.button.RemoveEffect(ETCButton.eEffectType.onContinue);
            }
            else
            {
                skill.button.AddEffect(ETCButton.eEffectType.onContinue);
            }
        }
#endif
    }

    //创建实体
    protected void CreateEntity()
    {
        if(owner!=null && !owner.IsDead())
        {
            VInt3 centerPos = owner.CurrentBeScene.GetSceneCenterPosition();
            BeEntity entity = owner.AddEntity(m_EntityId, centerPos);
        }
    }

    //对场景内的所有敌军造成伤害
    protected void DoHurtToAllEnemy()
    {
        List<BeActor> actorList = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindTargets(actorList, owner, VInt.Float2VIntValue(100.0f));
        if (actorList.Count > 0)
        {
            for (int i = 0; i < actorList.Count; i++)
            {
                BeActor target = actorList[i];
                if (target != null)
                {
                    var hitPos = actorList[i].GetPosition();
                    hitPos.z += VInt.one.i;
                    owner._onHurtEntity(actorList[i], hitPos, m_HurtId);
                }
            }
        }
        GamePool.ListPool<BeActor>.Release(actorList);
    }

    protected void RemoveHandle()
    {
        m_CurrTimeAcc = 0;
        m_CreateEntityFlag = false;
        if (m_ReplaceSkillHandle != null)
        {
            m_ReplaceSkillHandle.Remove();
            m_ReplaceSkillHandle = null;
        }

        if (m_ReplaceSkillPhaseHandle != null)
        {
            m_ReplaceSkillPhaseHandle.Remove();
            m_ReplaceSkillPhaseHandle = null;
        }

        if (m_AddBuffHandle != null)
        {
            m_AddBuffHandle.Remove();
            m_AddBuffHandle = null;
        }

        if (m_PassDoor != null)
        {
            m_PassDoor.Remove();
            m_PassDoor = null;
        }
    }
}
