using System;
using System.Collections.Generic;
using UnityEngine;


public class Mechanism148 : BeMechanism
{
    public Mechanism148(int mid, int lv) : base(mid, lv) { }
    float speed = 0.0f;
    float accTime = 0.0f;
   // float deccTime = 0.0f;
    public override void OnInit()
    {
        base.OnInit();
        speed = TableManager.GetValueFromUnionCell(data.ValueA[0], level) /1000.0f;
        accTime = TableManager.GetValueFromUnionCell(data.ValueB[0], level) / 1000.0f;
    }
    public override void OnStart()
    {
        base.OnStart();
#if !LOGIC_SERVER
        if (owner == null || owner.m_pkGeActor == null) return;
        var node = owner.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Charactor);
        if (node == null) return;
        var uvComponent = node.GetComponentInChildren<ScrollAnimator>();
        if(uvComponent != null)
        {
            uvComponent.SetSpeed(speed,accTime);
        }
#endif

    }
}

