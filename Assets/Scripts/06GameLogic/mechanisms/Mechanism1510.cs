using System.Collections.Generic;
using GameClient;
using Spine;

/// <summary>
/// 在机制期间锁定攻击buff的释放者
/// </summary>
public class Mechanism1510 : BeMechanism
{
    public Mechanism1510(int mid, int lv) : base(mid, lv)
    {
    }

    public override void OnStart()
    {
        base.OnStart();
        var attacker = GetAttachBuffReleaser();
        if (attacker != null)
        {
            if (owner.aiManager != null)
            {
                owner.aiManager.SetTarget(attacker, true);
            }
        }
    }

    public override void OnFinish()
    {
        base.OnFinish();
        if (GetAttachBuffReleaser() != null)
        {
            if (owner.aiManager != null)
            {
                owner.aiManager.SetTarget(null, false);
            }    
        }
    }
}