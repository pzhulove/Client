using System;
using System.Collections.Generic;
using UnityEngine;

public class Buff183206 : Buff183205
{
    public Buff183206(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack) { }
}

public class Buff183205 : BeBuff
{
    GeEffectEx effect1;
    GeEffectEx effect2;

    public Buff183205(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack) { }

    public override void OnReset()
    {
        effect1 = null;
        effect2 = null;
    }
    public override void OnStart()
    {
#if !LOGIC_SERVER
        effect1 = owner.m_pkGeActor.CreateEffect(1002, Vec3.zero, false, 0, EffectTimeType.BUFF);
        effect2 = owner.m_pkGeActor.CreateEffect(1003, Vec3.zero, false, 0, EffectTimeType.BUFF);
#endif
    }

    public override void OnFinish()
    {
        if (effect1 != null)
        {
            owner.m_pkGeActor.DestroyEffect(effect1);
            effect1 = null;
        }
        if (effect2 != null)
        {
            owner.m_pkGeActor.DestroyEffect(effect2);
            effect2 = null;
        }
    }
}
