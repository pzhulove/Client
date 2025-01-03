using System;
using System.Collections.Generic;

public class Mechanism157 : BeMechanism
{
    public Mechanism157(int mid, int lv) : base(mid, lv) { }
    VInt2 mPos;
    int mDurTime;
    readonly int intervel = GlobalLogic.VALUE_200;
    int elapsTime = 0;
    bool mStarted = false;
    readonly VInt tolerance = VInt.Float2VIntValue(0.1f);
    int moveTime = 0;
    bool disableSucked = false;

    public override void OnReset()
    {
        elapsTime = 0;
        mStarted = false;
        moveTime = 0;
        disableSucked = false;
    }
    public override void OnInit()
    {
        base.OnInit();
        int x = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        int y = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        mPos = new VInt2(x,y);
        mDurTime = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
    }
    public override void OnStart()
    {
        base.OnStart();
        if (owner == null) return;
        owner.aiManager.Stop();
        moveTime = 0;
        mStarted = true;
    }
    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (owner == null) return;
        if (!mStarted)
        {
            return;
        }
        elapsTime += deltaTime;
        if(elapsTime < intervel)
        {
            return;
        }
        if (owner.IsDead())
        {
            mStarted = false;
            return;
        }
        moveTime += elapsTime;
        elapsTime -= intervel;
        if (owner.stateController.WillBeGrab() || owner.stateController.IsGrabbing() || owner.stateController.IsBeingGrab())
        {
            mStarted = false;
            return;
        }
        if (!owner.stateController.CanNotAbsorbByBlockHole())
        {
            owner.stateController.SetAbilityEnable(BeAbilityType.CANNOTBE_SUCKED,false);
            disableSucked = true;
        }
        var vec = mPos - owner.GetPosition2();
        int leftTime = mDurTime - moveTime;
        if (vec.magnitude <= tolerance.i)
        {
            owner.SetMoveSpeedX(VInt.zero);
            owner.SetMoveSpeedY(VInt.zero);
        }
        else
        {
            if (leftTime <= 0)
            {
                owner.SetMoveSpeedX(VInt.zero);
                owner.SetMoveSpeedY(VInt.zero);
                VInt3 newPos = new VInt3(mPos.x, mPos.y, owner.GetPosition().z);
                owner.SetPosition(newPos);
            }
            else
            {
                VInt speed = vec.magnitude / leftTime;
                vec.NormalizeTo(speed.i);
                owner.SetMoveSpeedX(vec.x);
                owner.SetMoveSpeedY(vec.y);
            }
        }
        if (IsNearTargetPosition())
        {
            owner.aiManager.Start();
            if (disableSucked)
            {
                owner.stateController.SetAbilityEnable(BeAbilityType.CANNOTBE_SUCKED, true);
                disableSucked = false;
            }
            mStarted = false;
        }
    }
    private bool IsNearTargetPosition()
    {
        int distance = tolerance.i;
        int dist = (owner.GetPosition2() - mPos).magnitude;
        return dist  <= distance;
    }
}

