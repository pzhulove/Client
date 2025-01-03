using System;
using UnityEngine;

public enum Assault_State
{
    Init = 0,   // 初始化状态，没有冲锋
    Prepare,    // 准备阶段，蓄力
    Running,    // 冲锋进行中
    StopAction, // 冲锋结束动作播放中，播放完毕后，冲锋结束，状态回归Init
}

/// <summary>
/// 冲锋机制
/// </summary>
class Mechanism10002 : BeMechanism
{
    Assault_State state;

    string actionName0 = "";
    string actionName1 = "";
    string actionName2 = "";
    int finishTime = -1;
    int prepareTime = 0;
    int tarRadius = 0;
    int distance = 0;
    int speed = 0;
    int stopDistance;
    bool isToTargetPos = false;
    VInt3 targetPos = VInt3.zero;
    VInt3 lastPos = VInt3.zero;

    int stopActionTime = 0;
    VFactor angle = VFactor.zero;

    BeEntity target;


    public Mechanism10002(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        state = Assault_State.Init;

        actionName0 = "";
        actionName1 = "";
        actionName2 = "";
        prepareTime = 0;
        tarRadius = 0;
        distance = 0;
        speed = 0;
        isToTargetPos = false;
        targetPos = VInt3.zero;
        lastPos = VInt3.zero;

        actionName0 = data.StringValueA[0];

        if (data.StringValueA.Count > 1)
        {
            actionName1 = data.StringValueA[1];
        }
        if (data.StringValueA.Count > 2)
        {
            actionName2 = data.StringValueA[2];
        }

        prepareTime = TableManager.GetValueFromUnionCell(data.ValueA[0], level);

        tarRadius = TableManager.GetValueFromUnionCell(data.ValueB[0], level);

        distance = TableManager.GetValueFromUnionCell(data.ValueC[0], level);

        speed = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueD[0], level), GlobalLogic.VALUE_1000).i;

        isToTargetPos = TableManager.GetValueFromUnionCell(data.ValueE[0], level) == 0 ? false : true;

        stopDistance = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueF[0], level), GlobalLogic.VALUE_1000).i;
    }

    public override void OnReset()
    {
        finishTime = -1;
        stopActionTime = 0;
        angle = VFactor.zero;
        target = null;
    }

    public override void OnStart()
    {
        finishTime = -1;
        state = Assault_State.Prepare;

        if (owner != null)
        {
            if (tarRadius > 0 && owner.aiManager != null)
            {
                BeActorAIManager ai = owner.aiManager as BeActorAIManager;
                if (ai != null)
                {
                    var sight = ai.sight;
                    ai.sight = tarRadius;
                    target = ai.FindTarget();
                    ai.sight = sight;
                }
            }

            if (owner.CurrentBeScene.IsInBlockPlayer(owner.GetPosition()))
            {
                var pos = BeAIManager.FindStandPositionNew(owner.GetPosition(), owner.CurrentBeScene, owner.GetFace());
                owner.SetPosition(pos);
            }

            handleA = owner.RegisterEventNew(BeEventType.onHit, eventParam =>
            {
                state = Assault_State.StopAction;
                stopActionTime = 0;
            });
        }
    }

    public override void OnUpdate(int deltaTime)
    {
        switch (state)
        {
            case Assault_State.Init:
                {
                    break;
                }
            case Assault_State.Prepare:
                {
                    if (finishTime > 0)
                    {
                        finishTime -= deltaTime;
                        if (finishTime <= 0)
                        {
                            finishTime = 0;
                            Stop();
                        }
                    }

                    if (finishTime == -1)
                    {
                        int time = owner.sgGetCurrentStateRemainTime();
                        if (time > 100)
                        {
                            finishTime = time - 100;
                        }
                    }

                    VInt3 orgPos = owner.GetPosition();
                    VInt3 tarPos = owner.GetPosition() + new VInt3(10000 * owner._getFaceCoff(), 0, 0);
                    if (target != null)
                    {
                        tarPos = target.GetPosition();
                    }

                    int deltaX = tarPos.x - orgPos.x;
                    angle = IntMath.atan2(tarPos.y - orgPos.y, deltaX == 0 ? 1 : deltaX);
                    owner.SetFace(deltaX < 0, true, true);

                    if (isToTargetPos)
                    {
                        targetPos = tarPos;
                        lastPos = owner.GetPosition();
                    }

                    prepareTime -= deltaTime;
                    if (prepareTime <= 0)
                    {
                        state = Assault_State.Running;
                    }

                    break;
                }
            case Assault_State.Running:
                {
                    if (finishTime > 0)
                    {
                        finishTime -= deltaTime;
                        if (finishTime <= 0)
                        {
                            finishTime = 0;
                            Stop();
                        }
                    }

                    if (owner != null)
                    {
                        VInt3 curPos = owner.GetPosition();

                        VFactor time = new VFactor(deltaTime, GlobalLogic.VALUE_1000);

                        curPos.x += speed * IntMath.cos(angle.nom, angle.den) * time;
                        curPos.y += speed * IntMath.sin(angle.nom, angle.den) * time;

                        if (owner.CurrentBeScene == null || owner.CurrentBeScene.IsInBlockPlayer(curPos))
                        {
                            Stop();
                            break;
                        }

                        if (isCloseTargePos(curPos))
                        {
                            finishTime = 0;
                            Stop();
                            break;
                        }

                        owner.SetPosition(curPos);
                    }

                    break;
                }
            case Assault_State.StopAction:
                {
                    stopActionTime -= deltaTime;
                    if (stopActionTime <= 0)
                    {
                        state = Assault_State.Init;
                        SetFinish();
                    }

                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    private bool isCloseTargePos(VInt3 curPos)
    {
        if (!isToTargetPos || lastPos == curPos)
        {
            return false;
        }

        if (stopDistance > 0)
        {
            return (curPos - targetPos).magnitude < stopDistance;
        }

        else
        {
            var lastDis = (lastPos - targetPos).magnitude;
            var curDis = (curPos - targetPos).magnitude;
            lastPos = curPos;
            if (lastDis - curDis < 0)
            {
                return true;
            }
        }

        return false;
    }

    public void Stop()
    {
        if (state != Assault_State.Prepare && state != Assault_State.Running)
        {
            return;
        }

        state = Assault_State.StopAction;

        string name = finishTime == 0 ? actionName2 : actionName1;
        if (owner != null && owner.HasAction(name))
        {
            owner.GetStateGraph().ResetCurrentStateTime();
            owner.PlayAction(name);
            stopActionTime = owner.GetCurrentActionDuration();
            owner.sgSetCurrentStatesTimeout(stopActionTime, true);
        }

        if (isToTargetPos)
        {
            targetPos = VInt3.zero;
            lastPos = VInt3.zero;
        }
    }

    void SetFinish()
    {
        if (GetAttachBuff() != null)
        {
            RemoveAttachBuff();
        }
        else
        {
            owner.RemoveMechanism(this);
        }
    }
}
