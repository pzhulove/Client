/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色在身前召唤一个炮兵。炮兵会存在一定时间。
/// 当敌人进入炮兵小组的攻击范围内时，敌人的脚底下会始终出现攻击预警特效。此时玩家再次点击此技能时，炮兵小组会进行攻击。
/// 在短暂延迟后炮弹落地，对区域范围敌人造成伤害和浮空效果
/// </summary>
public class Skill1403 : SummonSkill
{
    public Skill1403(int sid, int skillLevel) : base(sid, skillLevel){ }
    
    private BeEventHandle m_SkillFrameHandle;
    private int _mechanismId = 6204;

    public override void OnClickAgain()
    {
        DoAttack();
    }

    protected override void OnSummon(BeActor monster)
    {
        
    }

    protected void DoAttack()
    {
        var mechanism = GetMonsterMechanism<Mechanism1125>(_mechanismId);
        if(mechanism==null)
            return;
        mechanism.DoAttack();
    }
    /// <summary>
    /// 点亮技能按键
    /// </summary>
    public void LightSkillButton(bool active)
    {
        if (!CanClickAgain)
            return;

        var updateState = active ? SkillState.WAIT_FOR_NEXT_PRESS : SkillState.NORMAL; 
        if(updateState == skillButtonState)
            return;
        
        if (active)
        {
            skillButtonState = SkillState.WAIT_FOR_NEXT_PRESS;
            ChangeButtonEffect();    
        }
        else
        {
            ResetButtonEffect();
        }
    }
}
*/
