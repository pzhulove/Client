using UnityEngine;
using System.Collections;

//隐藏头顶信息和脚底特效
public class Buff69 : BeBuff
{
    public Buff69(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack)
    { }

    public override void OnStart()
    {
#if !LOGIC_SERVER
        owner.m_pkGeActor.SetActorVisible(false);
#endif
    }

    public override void OnFinish()
    {
#if !LOGIC_SERVER
        owner.m_pkGeActor.SetActorVisible(true);
#endif
    }

}
