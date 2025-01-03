using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 秘术师-觉醒 
/// </summary>
public class Skill2212 : BeSkill
{

    public Skill2212(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override void OnStart()
    {
        base.OnStart();
        SetEffectVisible(false);
    }

    public override void OnCancel()
    {
        base.OnCancel();
        SetEffectVisible(true);
    }

    public override void OnFinish()
    {
        base.OnFinish();
        SetEffectVisible(true);
    }

    protected void SetEffectVisible(bool isVisible)
    {
#if !LOGIC_SERVER
        owner.m_pkGeActor.GetEffectManager().SetEffectVisible(isVisible);
#endif
    }
}
