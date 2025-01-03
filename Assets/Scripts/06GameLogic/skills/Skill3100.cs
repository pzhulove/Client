using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;

/// <summary>
/// 猫拳
/// </summary>
public class Skill3100 : BeSkill
{
    public Skill3100(int sid, int skillLevel): base(sid, skillLevel)
	{
        
	}

    public override void OnInit()
    {
        base.OnInit();
        if (!BattleMain.IsModePvP(battleType))
        {
            pressMode = SkillPressMode.PRESS_AGAIN_CANCEL_OUT;
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        if (!BattleMain.IsModePvP(battleType))
        {
            SetButtonMode();
        }
    }

    public override void OnClickAgainCancel()
    {
        base.OnClickAgainCancel();
        SetButtonMode(true);
        RemoveBuffs();
    }

    protected void SetButtonMode(bool isRestore = false)
    {
        if (!isRestore)
        {
            skillButtonState = BeSkill.SkillState.WAIT_FOR_NEXT_PRESS;
#if !SERVER_LOGIC
            if (button != null)
                button.AddEffect(ETCButton.eEffectType.onContinue, true);
#endif
        }
        else
        {
            skillButtonState = BeSkill.SkillState.NORMAL;
#if !SERVER_LOGIC
            if (button != null)
                button.RemoveEffect(ETCButton.eEffectType.onContinue, true);
#endif
        }
    }

    protected void RemoveBuffs()
    {
        for(int i = 0; i < skillData.ValueA.Count; i++)
        {
            owner.buffController.RemoveBuff(TableManager.GetValueFromUnionCell(skillData.ValueA[i],level));
        }
    }
}
