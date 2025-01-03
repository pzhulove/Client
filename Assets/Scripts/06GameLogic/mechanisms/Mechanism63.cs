using UnityEngine;
using System.Collections.Generic;
using GameClient;

/*
 * 替换技能
*/
public class Mechanism63 : BeMechanism
{
    protected int m_OriginalSkillId = 0;            //原来的技能ID
    protected int m_ReplaceSkillId = 0;             //替换后的技能ID

    protected IBeEventHandle m_ReplaceSkillHandle = null;        //替换技能
    protected IBeEventHandle m_CastSkillFinishHandle = null;     //释放完技能
    protected int m_ReplaceSkillLevel = -1;						//替换后的技能的原来的等级
    protected bool changeSkillLevel = true;
    public Mechanism63(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        m_ReplaceSkillHandle = null;
        m_CastSkillFinishHandle = null;
        m_ReplaceSkillLevel = -1;
        changeSkillLevel = true;
    }    
    public override void OnInit()
    {
        m_OriginalSkillId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        m_ReplaceSkillId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        changeSkillLevel = TableManager.GetValueFromUnionCell(data.ValueC[0], level) != 1;
    }

    public override void OnStart()
    {
        RemoveHandle();
        m_ReplaceSkillHandle = owner.RegisterEventNew(BeEventType.onReplaceSkill, args =>
        {
            int skillId = args.m_Int;
            BeSkill originalSkill = owner.GetSkill(m_OriginalSkillId);
            if (skillId == m_OriginalSkillId && IsYinGunagLuoren(m_OriginalSkillId))
            {
                BeSkill replaceSkill = owner.GetSkill(m_ReplaceSkillId);
                if (replaceSkill != null && replaceSkill.CanUseSkill())             //判断替换后的技能能不能使用
                {
                    m_ReplaceSkillLevel = replaceSkill.level;
                    originalSkill.StartCoolDown();                                  //被替换的技能开始CD
                    if (changeSkillLevel)
                        replaceSkill.level = originalSkill.level;                       //替换的技能等级继承原来的技能等级
                    args.m_Int = m_ReplaceSkillId;
                }
                else
                {
                    Logger.LogWarning("技能替换机制63中替换后的技能不可用,如果替换的是combo技能可以忽略此警告");
                }
            }
        });

        m_CastSkillFinishHandle = owner.RegisterEventNew(BeEventType.onCastSkillFinish,  args =>
        {
            int skillId = args.m_Int;
            if (skillId == m_ReplaceSkillId && m_ReplaceSkillLevel != -1)
            {
                BeSkill skill = owner.GetSkill(m_ReplaceSkillId);
                if (skill != null && skill.level != m_ReplaceSkillLevel && changeSkillLevel)
                {
                    skill.level = m_ReplaceSkillLevel;
                }
            }
        });
    }

    private bool IsYinGunagLuoren(int id)
    {
        if (id == 1514)
        {
            if (owner.sgGetCurrentState() == (int)ActionState.AS_JUMPBACK)
                return true;
            return false;
        }
        return true;

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
            m_ReplaceSkillHandle = null;
        }

        if (m_CastSkillFinishHandle != null)
        {
            m_CastSkillFinishHandle.Remove();
            m_CastSkillFinishHandle = null;
        }

    }
}
