using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff560001 : BeBuff
{

    public Buff560001(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack)
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
