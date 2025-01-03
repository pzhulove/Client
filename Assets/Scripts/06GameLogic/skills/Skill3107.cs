using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;

//气功师 - 念气环绕
public class Skill3107 : BeSkill
{
    protected int m_SkillBuffId = 0;                //念气环绕机制ID
    protected int m_PveBuffId = 0;                  //Pve添加的BuffId
    protected int m_PvpBuffId = 0;                  //Pvp添加的BuffId
    protected int m_BuffId = 180050;                //念气环绕机制BuffId

    protected IBeEventHandle m_OwnerRebornHandle = null;

    public Skill3107(int sid, int skillLevel): base(sid, skillLevel)
	{
        
	}

    public override void OnInit()
    {
        m_SkillBuffId = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        m_PveBuffId = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
        m_PvpBuffId = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
    }

    public override void OnPostInit()
    {
        m_OwnerRebornHandle = owner.RegisterEventNew(BeEventType.onReborn, arg =>
        {
            OwnerReborn();
        });
        AddBuff();
        AddSkillBuff();
    }

    protected void OwnerReborn()
    {
        if (owner.buffController.HasBuffByID(m_BuffId) != null)
        {
            owner.buffController.RemoveBuff(m_BuffId);
        }
        owner.buffController.TryAddBuff(m_BuffId,-1);
    }

    protected void AddBuff()
    {
        int addBuffId = 0;
        if (!BattleMain.IsModePvP(battleType))
        {
            addBuffId = m_PveBuffId;
        }
        else
        {
            addBuffId = m_PvpBuffId;
        }

        owner.buffController.RemoveBuff(m_SkillBuffId);
        owner.buffController.RemoveBuff(addBuffId);

        owner.buffController.TryAddBuff(m_SkillBuffId, -1, level);
        owner.buffController.TryAddBuff(addBuffId, -1, level);
    }

    protected void AddSkillBuff()
    {
        IList<UnionCell> skillUpList = null;
        if (!BattleMain.IsModePvP(battleType))
        {
            skillUpList = skillData.ValueD;
        }
        else
        {
            skillUpList = skillData.ValueE;
        }
        for (int i = 0; i < skillUpList.Count; ++i)
        {
            int skillID = skillUpList[i].fixInitValue;
            int buffID = skillUpList[i].fixLevelGrow;

            int buffLevel = level;


            owner.buffController.RemoveBuff(buffID);
            owner.buffController.AddBuffForSkill(buffID, buffLevel, -1, new List<int> { skillID });
        }
    }
}
