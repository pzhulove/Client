using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 调整摄像机的Near（大特效剔除问题）
/// </summary>
public class Mechanism1062 : BeMechanism
{
    float nearValue = 0;
    float tempValue = 0;
    public Mechanism1062(int mid, int lv) : base(mid, lv)
    {
    }

    public override void OnInit()
    {
        base.OnInit();
        nearValue = TableManager.GetValueFromUnionCell(data.ValueA[0], level) / 1000.0f;
    }

    public override void OnReset()
    {
        #if !SERVER_LOGIC
        tempValue = 0;
        #endif
    }

    public override void OnStart()
    {
        base.OnStart();
#if !SERVER_LOGIC
        if (owner.isLocalActor)
        {
            //var camera = owner.CurrentBeScene.currentGeScene.GetCamera().GetCamera();
            //tempValue = camera.nearClipPlane;
            //camera.nearClipPlane = nearValue;
        }
#endif
    }

    public override void OnFinish()
    {
        base.OnFinish();
#if !SERVER_LOGIC
        if (owner.isLocalActor)
        {
            //var camera = owner.CurrentBeScene.currentGeScene.GetCamera().GetCamera();
            //camera.nearClipPlane = tempValue;
        }
#endif
    }
}
