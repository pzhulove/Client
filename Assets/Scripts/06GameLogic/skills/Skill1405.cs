using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 原创职业  部署技能
/// </summary>
/// 
public class Skill1405 : BeSkill
{
    public Skill1405(int sid, int skillLevel) : base(sid, skillLevel)
    { }

    private readonly int _buffId = 140500;

    public override void OnClickAgainCancel()
    {
        base.OnClickAgainCancel();
        owner.buffController.RemoveBuff(_buffId);
    }
}
