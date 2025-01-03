using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
 * 阵鬼 鬼影闪
*/
public class Skill1810 : BeSkill
{

    protected int m_CheckBuff = 180013;             //残影之凯贾-无敌Buff

    public Skill1810 (int sid, int skillLevel):base(sid, skillLevel)
    {
        
    }

    public override void OnStart()
    {
#if !LOGIC_SERVER
        if (button != null)
        {
            button.RemoveEffect(ETCButton.eEffectType.onContinue);
        }
#endif
    }

    public override bool CanUseSkill()
    {
        bool canUse = base.CanUseSkill();
        //在残影之凯贾的前冲无敌状态下才能释放技能
        if (owner.buffController.HasBuffByID(m_CheckBuff) == null)
        {
            canUse = false;
#if !LOGIC_SERVER
            if (button != null)
            {
                button.RemoveEffect(ETCButton.eEffectType.onContinue);
            }
#endif
        }
        else
        {
#if !LOGIC_SERVER
            if (button != null && canUse)
            {
                button.AddEffect(ETCButton.eEffectType.onContinue);
            }
#endif
        }
        return canUse;
    }

    public override BeSkillManager.SkillCannotUseType GetCannotUseType()
    {
        if (owner.buffController.HasBuffByID(m_CheckBuff) == null)
        {
            return BeSkillManager.SkillCannotUseType.NO_CYZKJ;
        }

        return base.GetCannotUseType();
    }
}
