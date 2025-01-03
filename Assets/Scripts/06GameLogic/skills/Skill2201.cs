using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 秘术师-移动魔法
/// </summary>
public class Skill2201 : BeSkill
{
    protected int[] m_BuffIdArray = new int[2];                     //添加的BuffId（Pve，Pvp）

    protected IBeEventHandle m_OnBeforeHitHandle = null;             //监听被击之前的事件

    protected int m_ReplaceNormalId = 20001;                        //替换后的普攻ID

    public Skill2201(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override void OnPostInit()
    {
        if (skillData.ValueA.Count > 0)
        {
            for (int i = 0; i < skillData.ValueA.Count; i++)
            {
                int skillId = TableManager.GetValueFromUnionCell(skillData.ValueA[i], level);
                if (skillId != 0)
                {
                    BeSkill skill = owner.GetSkill(skillId);
                    if (skill != null)
                    {
                        skill.walk = true;
                        skill.walkSpeed = GetWalkSpeed();
                    }
                }
            }
        }

        if (skillData.ValueB.Count > 0)
        {
            for (int i = 0; i < skillData.ValueB.Count; i++)
            {
                int buffInfoId = TableManager.GetValueFromUnionCell(skillData.ValueB[i], level);
                m_BuffIdArray[i] = buffInfoId;
            }
        }

        RemoveHandle();
        m_OnBeforeHitHandle = owner.RegisterEventNew(BeEventType.onBeforeOtherHit, (args) =>
        {
            BeSkill curSkill = owner.GetCurrentSkill();
            if (curSkill != null&& owner.sgGetCurrentState() == (int)ActionState.AS_CASTSKILL)
            {
                if (curSkill.GetCurrPhase() == curSkill.chargeConfig.repeatPhase && curSkill.walk == true)
                {
                    int buffId = 0;
                    if (BattleMain.IsModePvP(battleType))
                    {
                        buffId = m_BuffIdArray[1];
                    }
                    else
                    {
                        buffId = m_BuffIdArray[0];
                    }
                    owner.buffController.TryAddBuff(buffId,50,level);
                }
            }
        });
        ReplaceNormalAttackId();
    }

    //替换普攻ID
    protected void ReplaceNormalAttackId()
    {
        BeSkill replaceNormalSkill = owner.GetSkill(m_ReplaceNormalId);
        if (replaceNormalSkill != null)
        {
            owner.GetEntityData().normalAttackID = m_ReplaceNormalId;
        }
    }

    protected VFactor GetWalkSpeed()
    {
        int speedRate = 0;
        if (BattleMain.IsModePvP(battleType))
        {
            speedRate = TableManager.GetValueFromUnionCell(skillData.WalkSpeedPVP, level);
        }
        else
        {
            speedRate = TableManager.GetValueFromUnionCell(skillData.WalkSpeed, level);
        }
        return new VFactor(speedRate, GlobalLogic.VALUE_1000);
    }

    protected void RemoveHandle()
    {
        if (m_OnBeforeHitHandle != null)
        {
            m_OnBeforeHitHandle.Remove();
            m_OnBeforeHitHandle = null;
        }
    }
}
