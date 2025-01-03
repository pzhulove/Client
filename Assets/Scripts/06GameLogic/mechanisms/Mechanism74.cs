using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanism74 : BeMechanism
{
    readonly protected int m_ReplaceJumpAttackId = 1919;         //替换成的空中攻击ID
    protected IBeEventHandle m_JumpBackHandle = null;    //监听后跳
    readonly private int jumpBackAttackID = 1907;



    public Mechanism74(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }

    public override void OnStart()
    {
        base.OnStart();
        //后跳普攻替换
        m_JumpBackHandle = owner.RegisterEventNew(BeEventType.onJumpBackAttack, (args) =>
        {
            BeSkill skillAttack = owner.GetSkill(jumpBackAttackID);
            if (skillAttack == null) return;
            if (!skillAttack.isCooldown)
            {
                owner.UseSkill(m_ReplaceJumpAttackId, true);
                BeSkill skill = owner.GetSkill(jumpBackAttackID);
                if (skill != null)
                {
                    skill.StartCoolDown();
                }
            }
            else if (skillAttack.isCooldown && owner.isLocalActor)
            {
                owner.m_pkGeActor.CreateHeadText(GameClient.HitTextType.SKILL_CANNOTUSE, "UI/Font/new_font/pic_incd.png");
            }

        });
    }

    public override void OnFinish()
    {
        if (m_JumpBackHandle != null)
        {
            m_JumpBackHandle.Remove();
            m_JumpBackHandle = null;
        }
    }

}
