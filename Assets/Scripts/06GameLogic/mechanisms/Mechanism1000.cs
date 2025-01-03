using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//改变某个技能的移动速率
public class Mechanism1000 : BeMechanism
{
    public Mechanism1000(int mid, int lv) : base(mid, lv){ }

    protected int walkSpeedAdd = 0;         //技能移动速度增加千分比
    protected List<int> relateSkillList = new List<int>();  //影响的技能列表 

    public override void OnInit()
    {
        base.OnInit();
        walkSpeedAdd = TableManager.GetValueFromUnionCell(data.ValueA[0],level);
        if (data.ValueB.Count > 0)
        {
            for(int i = 0; i < data.ValueB.Count; i++)
            {
                relateSkillList.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
            }
        }
    }

    public override void OnReset()
    {
        relateSkillList.Clear();
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onSkillPostInit, (args) => 
        {
            if (relateSkillList != null && relateSkillList.Contains(args.m_Int))
                ChangeSkillWalkSpeed(args.m_Int,true);
        });
    }

    public override void OnFinish()
    {
        if (relateSkillList == null)
            return;
        base.OnFinish();
        for(int i=0;i< relateSkillList.Count; i++)
        {
            ChangeSkillWalkSpeed(relateSkillList[i]);
        }
    }

    //改变技能的移动速度
    protected void ChangeSkillWalkSpeed(int skillId,bool isAdd = false)
    {
        BeSkill skill = owner.GetSkill(skillId);
        if (skill == null)
            return;
        int walkOrigion = (int)skill.walkSpeed.nom;
        if (isAdd)
            walkOrigion += walkSpeedAdd;
        else
            walkOrigion -= walkSpeedAdd;
        skill.walkSpeed = new VFactor(walkOrigion,GlobalLogic.VALUE_1000);
    }
}
