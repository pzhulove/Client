using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 老鼠附咬

public class Buff2000002 : BeBuff
{
    int maxHitCount = 5;
    BeActor target = null;
    IBeEventHandle ownerOnHitHandler = null;
    IBeEventHandle targetBeKillHandler = null;
    IBeEventHandle targetBeFallHandler = null;

    bool isBited = false;
    //float deltaX = 0.0f;

    List<int> buffIds = new List<int>();

    public Buff2000002(int bi, int buffLevel, int buffDuration, int attack = 0)
        : base(bi, buffLevel, buffDuration, attack)
    {

    }

    public override void OnReset()
    {
        target = null;
        isBited = false;
        buffIds.Clear();
        
    }

    public override void OnInit()
    {
        int countBuff = buffData.ValueB.Count;
        for (int i = 0; i < countBuff; ++i)
        {
            var buffId = TableManager.GetValueFromUnionCell(buffData.ValueB[i], 1);
            if (buffId > 0)
                buffIds.Add(buffId);
        }
    }

    public override void OnStart()
    {
        if (owner == null)
        {
            Finish();
            return;
        }

        target = FindTarget(owner.GetPosition());
        if (target == null)
        {
            Finish();
            return;
        }

        //
        var sc = owner.stateController;
        if (sc == null)
        {
            Finish();
            return;
        }

        if (!sc.CanBeGrab())
        {
            Finish();
            return;
        }

        int faceFlag = target.GetPosition().x - owner.GetPosition().x > 0 ? -1 : 1;
        //deltaX = faceFlag * 0.5f;
        //owner.Locomote(new BeStateData((int)ActionState.AS_GRAPED, 1, 0, 0, 0, 0, duration, true));
        owner.Locomote(new BeStateData((int)ActionState.AS_GRABBED) { _StateData = 1, _timeout = duration, _timeoutForce = true });
        sc.SetGrabState(GrabState.BEING_GRAB);

        //被人攻擊
        ownerOnHitHandler = owner.RegisterEventNew(BeEventType.onHit, (args) =>
        {
            OnFinish();
        });

        //玩家被殺死
        targetBeKillHandler = target.RegisterEventNew(BeEventType.onBeKilled, args =>
        //targetBeKillHandler = target.RegisterEvent(BeEventType.onBeKilled, (object[] args) =>
        {
            OnFinish();
        });

        //玩家倒地
        targetBeFallHandler = target.RegisterEventNew(BeEventType.onStateChange, OnActorStateChange);

        isBited = true;
        BeBuffManager buffController = target.buffController;
        if (buffController != null)
        {
            for (int i = 0; i < buffIds.Count; ++i)
                buffController.TryAddBuff(buffIds[i]);
        }
    }

    public void OnActorStateChange(GameClient.BeEvent.BeEventParam param)
    {
        var state = (ActionState)param.m_Int;
        if (state == ActionState.AS_FALL)
        {
            OnFinish();
        }
    }

    protected BeActor FindTarget(VInt3 pos)
    {
        if (owner == null)
            return null;

        var target = owner.CurrentBeScene.FindNearestRangeTarget(pos, owner, VInt.one);
        return target;
    }

    public override void OnFinish()
    {
        RemoveHandler();
        if (!isBited)
            return;

        if (owner == null)
            return;

        var sc = owner.stateController;
        if (sc == null)
            return;

        if (sc.IsBeingGrab())
        {
            owner.sgClearStateStack();
            owner.sgPushState(new BeStateData((int)ActionState.AS_FALL));
            owner.sgLocomoteState();
            owner.OnDealFrameTag(DSFFrameTags.TAG_SET_TARGET_POS_XY);
        }

        if (target != null)
        {
            BeBuffManager buffController = target.buffController;
            if (buffController != null)
            {
                for (int i = 0; i < buffIds.Count; ++i)
                    buffController.RemoveBuff(buffIds[i], 1);
            }
        }
    }

    void RemoveHandler()
    {
        if (ownerOnHitHandler != null)
        {
            ownerOnHitHandler.Remove();
            ownerOnHitHandler = null;
        }

        if (targetBeKillHandler != null)
        {
            targetBeKillHandler.Remove();
            targetBeKillHandler = null;
        }

        if (targetBeFallHandler != null)
        {
            targetBeFallHandler.Remove();
            targetBeFallHandler = null;
        }
    }

    public override void OnUpdate(int delta)
    {
        if (!isBited)
            return;

        if (owner == null || target == null)
            return;

        if (target.IsDead() || owner.IsDead())
        {
            OnFinish();
            return;
        }

        var sc = owner.stateController;
        if (sc == null)
            return;

        if (!sc.IsBeingGrab())
        {
            OnFinish();
            return;
        }


        VInt3 targetBodyPos = target.GetPosition();
        VInt3 ownerPos = targetBodyPos;
        if (owner.GetFace())
        {
            ownerPos.x = targetBodyPos.x + VInt.Float2VIntValue(0.7f);
        }
        else
        {
            ownerPos.x = targetBodyPos.x - VInt.Float2VIntValue(0.7f);
        }
        ownerPos.y -= VInt.Float2VIntValue(0.03f);
        ownerPos.z += VInt.Float2VIntValue(0.3f);
        owner.SetPosition(ownerPos);
    }

    public override void DoWorkForInterval()
    {
        if (owner.stateController.IsBeingGrab())
        {
            //每秒造成傷害
            int demageId = TableManager.GetValueFromUnionCell(buffData.ValueA[0], level);
            owner.DoAttackTo(target, demageId);
        }
    }
}
