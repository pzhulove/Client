using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
双剑形态技能机制
 */

public class Mechanism165 : BeMechanism
{
    protected int m_SkillId = 4014;                     //双剑形态技能ID
    protected int m_ReplaceAttackId = 4000;             //替换的普攻ID
    protected BeActor.NormalAttack m_AttackData = new BeActor.NormalAttack();     //备份替换普攻数据
    

    public Mechanism165(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }

    public override void OnInit()
    {
    
    }

    public override void OnStart()
    {
        ReplaceAttackId();
    }

    //替换普攻技能
    protected void ReplaceAttackId()
    {
        m_AttackData = owner.AddReplaceAttackId(m_ReplaceAttackId, 2);
    }

    //还原普攻技能
    protected void RestoreAttackId()
    {
        owner.RemoveReplaceAttackId(m_AttackData);
    }

    public override void OnFinish()
    {
        RestoreAttackId();
        if (owner.HasSkill(m_SkillId))
        {
            owner.GetSkill(m_SkillId).Cancel();
        }
    }
}