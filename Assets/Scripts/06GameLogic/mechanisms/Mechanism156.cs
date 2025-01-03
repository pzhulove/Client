using System;
using System.Collections.Generic;

public class Mechanism156 : BeMechanism
{
    public Mechanism156(int mid, int lv) : base(mid, lv) { }
    float speedRate;
    float durTime;
    public override void OnInit()
    {
        base.OnInit();

        speedRate = TableManager.GetValueFromUnionCell(data.ValueA[0], level) /1000.0f;
        durTime = TableManager.GetValueFromUnionCell(data.ValueB[0], level) / 1000.0f;
    }
    public override void OnStart()
    {
#if !LOGIC_SERVER
        if (owner != null && owner.m_pkGeActor != null)
        {
            var parent = owner.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
            if (parent != null)
            {
                var comp = parent.GetComponentInChildren<RotateAnimator>();
                if(comp != null)
                {
                    comp.SetSpeed(speedRate,durTime);
                }
            }
        }
#endif
    }
}

