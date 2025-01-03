using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanism98 : BeMechanism {


    private BeActor target;
    private VRate speed;

    public Mechanism98(int mid, int lv) : base(mid, lv)
    {
    }

    public override void OnInit()
    {
        base.OnInit();
        speed =new VRate( TableManager.GetValueFromUnionCell(data.ValueA[0], level));
    }

    public override void OnStart()
    {
        base.OnStart();
        owner.aiManager.StopCurrentCommand();
        owner.aiManager.Stop();
        owner.ResetMoveCmd();
        target = owner.CurrentBeScene.FindNearestRangeTarget(owner.GetPosition(),owner, new VInt(int.MaxValue));

    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (target != null)
        {
            VInt3 tartgetPos = target.GetPosition();
            VInt3 ownerPos = owner.GetPosition();
            var dir = tartgetPos - ownerPos;
            if (dir.magnitude < 500) return;
            var newPos = ownerPos + dir.NormalizeTo((int)IntMath.kIntDen)*speed.f;
            owner.SetPosition(newPos, true);
        }
    }

    public override void OnFinish()
    {
        base.OnFinish();
        target = null;
    }

}
