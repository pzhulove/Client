using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 后跳爆头
/// </summary>
public class Mechanism2022 : BeMechanism
{
    protected readonly int m_ReplaceJumpAttackId = 11042;         //替换成的空中攻击ID
    private readonly int originSkillID01 = 1104;
    private readonly int originSkillID02 = 11040;
    public Mechanism2022(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }

    public override void OnStart()
    {
        base.OnStart();

        handleA = owner.RegisterEventNew(BeEventType.onReplaceSkill, args =>
        {
            int skillId = args.m_Int;
            if (skillId == originSkillID01 && owner.sgGetCurrentState() == (int)ActionState.AS_JUMPBACK)
            {
                BeSkill skillAttack = owner.GetSkill(originSkillID01);
                if (skillAttack == null) return;
                if (!skillAttack.isCooldown)
                {
                    BeSkill replaceSkill = owner.GetSkill(m_ReplaceJumpAttackId);
                    if (replaceSkill != null)
                    {
                        replaceSkill.level = skillAttack.level;
                        args.m_Int = m_ReplaceJumpAttackId;
                        skillAttack.StartCoolDown();
                    }


                }
                else if (skillAttack.isCooldown && owner.isLocalActor)
                {
                    if(owner.m_pkGeActor!=null)
                       owner.m_pkGeActor.CreateHeadText(GameClient.HitTextType.SKILL_CANNOTUSE, "UI/Font/new_font/pic_incd.png");
                }
            }


        });
    }
}
