using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
阵鬼 冥炎卡洛技能机制
 */

public class Mechanism44 : BeMechanism
{
    protected int m_ReduceMp = 0;                       //每秒法力值消耗

    readonly protected int m_SkillId = 1811;                     //冥炎卡洛技能ID
    readonly protected int m_ReplaceAttackId = 1819;             //替换的普攻ID
    readonly protected int m_ReplaceJumpAttackId = 1820;         //替换成的空中攻击ID
    readonly protected int m_NormalJumpAttackId = 1513;          //原始的空中攻击ID
    readonly protected int m_NormalTuciId = 1504;                //突刺技能ID
    protected IBeEventHandle m_JumpBackHandle = null;    //监听后跳
    protected IBeEventHandle m_ReplaceSkillHandle = null;//技能替换
    protected BeActor.NormalAttack m_AttackData = new BeActor.NormalAttack();     //备份替换普攻数据
    

    public Mechanism44(int sid, int skillLevel) : base(sid, skillLevel) {}
    public override void OnReset()
    {
        m_JumpBackHandle = null;
        m_ReplaceSkillHandle = null;
        m_AttackData = new BeActor.NormalAttack();
    }
    public override void OnInit()
    {
        m_ReduceMp = TableManager.GetValueFromUnionCell(data.ValueA[0],level);
    }

    public override void OnStart()
    {
		owner.GetEntityData().ChangeMPReduce(m_ReduceMp);
        //后跳普攻替换
        m_JumpBackHandle = owner.RegisterEventNew(BeEventType.onJumpBackAttack, (args) => 
        {
            if (owner.HasSkill(m_ReplaceJumpAttackId))
            {
                owner.UseSkill(m_ReplaceJumpAttackId,true);
            }
        });

        //替换突刺 空中攻击普攻替换
        m_ReplaceSkillHandle = owner.RegisterEventNew(BeEventType.onPreSetSkillAction, (GameClient.BeEvent.BeEventParam param) => 
        {
            //int[] skillList = (int[])args[0];
            if (param.m_Int == m_NormalTuciId)
            {
                param.m_Int = m_ReplaceAttackId;
            }

            if (param.m_Int == m_NormalJumpAttackId) 
            {
                param.m_Int = m_ReplaceJumpAttackId;
                int[] newPhaseArray = new int[2] { 1820, 18200};
                owner.skillController.SetCurrentSkillPhases(newPhaseArray);
            }
        });

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
        RemoveHandle();
        RestoreAttackId();
        if (owner.HasSkill(m_SkillId))
        {
            owner.GetSkill(m_SkillId).Cancel();
        }
    }

    public void RemoveHandle()
    {
        owner.GetEntityData().ChangeMPReduce(-m_ReduceMp);
        if (m_JumpBackHandle != null)
        {
            m_JumpBackHandle.Remove();
        }
        if (m_ReplaceSkillHandle != null)
        {
            m_ReplaceSkillHandle.Remove();
        }
    }
}