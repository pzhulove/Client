using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill1101 : Skill2522
{

    public Skill1101(int sid, int skillLevel) : base(sid, skillLevel)
    {
        effectId = 11010;
        attachEffectId = 10150;
        curFrameFlag = "110101";
        skillId = 1015;
    }

    /// <summary>
    /// 获取剩余银弹次数
    /// </summary>
    /// <returns></returns>
    protected override int GetBulletNum()
    {
        Skill1015 skill = (Skill1015)owner.GetSkill(skillId);
        if (skill == null)
            return 0;
        return skill.GetLeftBulletNum();
    }

    /// <summary>
    /// 设置银弹数量
    /// </summary>
    protected override void SetSilverBulletCount()
    {
        Skill1015 skill = (Skill1015)owner.GetSkill(skillId);
        if (skill == null)
            return;
        skill.ConsumBulletCount();
    }
}
