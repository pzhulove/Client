using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///   给被抓取的加个BUFF
/// </summary>
public class Mechanism46 : BeMechanism
{

    private int buffID = 0;
    private int duration = 0;

    public Mechanism46(int sid, int skillLevel) : base(sid, skillLevel){}

    public override void OnInit()
    {
        base.OnInit();
        buffID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        duration = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
    }

    public override void OnStart()
    {
        base.OnStart();

        handleA = owner.RegisterEventNew(BeEventType.OnGrab, (args) =>
        {
            BeActor actor = args.m_Obj as BeActor;
            if (actor != null && actor.buffController.HasBuffByID(buffID) == null)
            {
                actor.buffController.TryAddBuff(buffID, duration);
            }
        });
    }

    public override void OnFinish()
    {
        base.OnFinish();
    }
}
