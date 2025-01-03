using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;

//钢筋铁骨
public class Skill3114 : BeSkill
{
    protected int m_PveBuffId = 0;
    protected int m_PvpBuffId = 0;

    public Skill3114(int sid, int skillLevel)
        : base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
        m_PveBuffId = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        m_PvpBuffId = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
    }

    public override void OnPostInit()
    {
        DoEffect();
    }

    //添加一个Buff
    protected void DoEffect()
    {
        if (!BattleMain.IsModePvP(battleType))
        {
            owner.buffController.RemoveBuff(m_PveBuffId);
            owner.buffController.TryAddBuff(m_PveBuffId, -1, level);
        }
        else
        {
            owner.buffController.RemoveBuff(m_PvpBuffId);
            owner.buffController.TryAddBuff(m_PvpBuffId, -1, level);
        }
    }
}
