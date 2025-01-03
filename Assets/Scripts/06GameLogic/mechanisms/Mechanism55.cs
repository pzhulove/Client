using UnityEngine;
using System.Collections.Generic;
using ProtoTable;

/*
 * 秘术师—杰克爆弹机制
*/
public class Mechanism55 : BeMechanism
{
    readonly protected int m_ReplaceAttackId = 2016;                 //替换的杰克爆弹ID
    readonly protected int m_OriginalSkillId = 2006;                //原始的杰克爆弹ID
    protected IBeEventHandle m_ReplaceSkillHandle = null;    //替换技能ID

    public Mechanism55(int mid, int lv) : base(mid, lv){}

    public override void OnReset()
    {
        m_ReplaceSkillHandle = null;
    }

    public override void OnStart()
    {
        RemoveHandle();
        //替换原来的杰克爆弹技能
        m_ReplaceSkillHandle = owner.RegisterEventNew(BeEventType.onReplaceSkill, args =>
        {
            if(owner.sgGetCurrentState()== (int)ActionState.AS_JUMP|| owner.sgGetCurrentState() == (int)ActionState.AS_JUMPBACK)
            {
                BeSkill orignSkill = owner.GetSkill(m_OriginalSkillId);
                if (orignSkill != null && !orignSkill.isCooldown)
                {
                    int skillId= args.m_Int;
                    if (skillId == m_OriginalSkillId)
                    {
                        args.m_Int = m_ReplaceAttackId;
                        orignSkill.StartCoolDown();
                    }
                }
            }
        });
    }

    public override void OnFinish()
    {
        RemoveHandle();
    }

    protected void RemoveHandle()
    {
        if (m_ReplaceSkillHandle != null)
        {
            m_ReplaceSkillHandle.Remove();
        }
    }
}
