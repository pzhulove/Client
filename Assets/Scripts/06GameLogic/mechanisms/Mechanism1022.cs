using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 修改技能CD
/// </summary>
public class Mechanism1022 : BeMechanism
{
    int[] skillArray;
    int[] skillCDArray;
    public Mechanism1022(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }


    public override void OnInit()
    {
        base.OnInit();
        skillArray = new int[data.ValueA.Length];
        for (int i = 0; i < data.ValueA.Length; i++)
        {
            skillArray[i] = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
        }
        skillCDArray = new int[data.ValueB.Length];
        for (int i = 0; i < data.ValueB.Length; i++)
        {
            skillCDArray[i] =(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
        }
    }

    int[] tmpSkillCDArray;
    public override void OnStart()
    {
        base.OnStart();
        tmpSkillCDArray = new int[skillArray.Length];
        for (int i = 0; i < skillArray.Length; i++)
        {
            BeSkill skill = owner.GetSkill(skillArray[i]);
            if (skill == null) continue;
            tmpSkillCDArray[i] = skill.tableCD;
            skill.tableCD = skillCDArray[i];
        }
    }

    private void ResetSkillCD()
    {
        for (int i = 0; i < skillArray.Length; i++)
        {
            BeSkill skill = owner.GetSkill(skillArray[i]);
            if (skill == null) continue;
            skill.tableCD = tmpSkillCDArray[i];
        }
    }

    public override void OnFinish()
    {
        base.OnFinish();
        ResetSkillCD();
    }

    public override void OnDead()
    {
        base.OnDead();
        ResetSkillCD();
    }
}
