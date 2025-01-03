using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//鹰踏-跳
public class Skill3018 : BeSkill
{
    IBeEventHandle handle1;
    IBeEventHandle handle2;

    public Skill3018(int sid, int skillLevel) : base(sid, skillLevel) { }

    void ReleaseSkill()
    {
        var skill = owner.GetSkill(3017) as Skill3017;
        if (skill != null)
        {
            skill.StartCD();
            skill.RemoveButtonEffect();
        }

        handle1.Remove();
        handle1 = null;

        handle2.Remove();
        handle2 = null;
    }

    public override void OnFinish()
    {
        owner.SetTag((int)AState.ACS_JUMP, true);

        handle1 = owner.RegisterEventNew(BeEventType.onTouchGround, args =>
        {
            ReleaseSkill();
        });

        handle2 = owner.RegisterEventNew(BeEventType.onHurt, args =>
        //handle2 = owner.RegisterEvent(BeEventType.onHurt, args =>
        {
            ReleaseSkill();
        });
    }
}
