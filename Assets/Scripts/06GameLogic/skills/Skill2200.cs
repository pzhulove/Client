using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 秘术师-烈焰冲击
/// </summary>
public class Skill2200 : BeSkill
{
    protected int m_EffectOffset = 2000;            //技能摇杆特效位置偏移
    protected int m_BuffId = 220001;                //烈焰冲击摇杆控制落点机制Buff

    public Skill2200(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
        m_EffectOffset = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
    }

    public override void OnStart()
    {
        var offset = VInt2.zero;
        offset.x = owner.GetFace() ? -m_EffectOffset * GlobalLogic.VALUE_10 : m_EffectOffset * GlobalLogic.VALUE_10;
        SetJoysticEffectOffset(offset);
    }

    public override void OnFinish()
    {
        BeBuff buff = owner.buffController.HasBuffByID(m_BuffId);
        if (buff != null)
        {
            owner.buffController.RemoveBuff(buff);
        }
    }
}
