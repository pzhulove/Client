using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
 * 阵鬼 冥炎卡洛技能
*/
public class Skill1811 : BeSkill
{
    protected int m_ReplaceSkillId = 1812;                      //二刀流ID
    protected List<int> m_RemoveBuffList = new List<int>();     //技能中断时移除的Buff

    public Skill1811 (int sid, int skillLevel):base(sid, skillLevel)
    {
        
    }

    public override void OnInit()
    {
        m_RemoveBuffList.Clear();
        if (skillData.ValueA.Count > 0)
        {
            for (int i = 0; i < skillData.ValueA.Count; i++)
            {
                int buffId = TableManager.GetValueFromUnionCell(skillData.ValueA[i], level);
                m_RemoveBuffList.Add(buffId);
            }
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        var replaceSkill = owner.GetSkill(m_ReplaceSkillId);
        if (replaceSkill != null)
            replaceSkill.ResetCoolDown();
    }

    public override void OnPostInit()
    {
        pressMode = SkillPressMode.TWO_PRESS_OUT;
    }

    public override void OnClickAgain()
    {
        //冥炎卡洛状态下 再次点击技能按钮 释放二刀流攻击
        if (owner.HasSkill(m_ReplaceSkillId))
        {
            owner.UseSkill(m_ReplaceSkillId);
        }
        OnCancel();
    }

    public override void OnCancel()
    {
        //职业平衡 Pve中再次点击技能按钮以后 冥炎之卡洛机制Buff继续生效
        if (!BattleMain.IsModePvP(battleType))
            return; 
        if (m_RemoveBuffList.Count > 0)
        {
            for (int i = 0; i < m_RemoveBuffList.Count; i++)
            {
                owner.buffController.RemoveBuff(m_RemoveBuffList[i]);
            }
        }
    }
}
