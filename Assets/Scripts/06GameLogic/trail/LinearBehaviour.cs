using UnityEngine;
using System.Collections;
using GameClient;

public class LinearBehaviour : TrailBehaviour
{

    public LinearBehaviour()
    {
        Type = TrailType.LINER;
        totalDist = 0;
    }

    public override void Init()
    {
        base.Init();

        totalDist = (StartPoint - EndPoint).magnitude;
        currentPos = StartPoint;
        velocity = (EndPoint - StartPoint).normalized;
    }

    public override void OnTick(int deltaTime)
    {
        if (ReachDest)
            return;

        TimeAcc += deltaTime;
        Vector3 moveDist = GetMoveDir() * MoveSpeed * deltaTime / 1000f;
        currentPos += moveDist;
    }
}

//回旋镖轨迹
public class BoomerangBehaviour : TrailBehaviour
{
    public enum Phase
    {
        GO,
        STAY,
        BACK,
        END
    }

    Phase state;
    int timeAcc = 0;
    public int stayDuration = 2000;

    public int userData = 0;
    bool end = false;

    public BoomerangBehaviour()
    {
        Type = TrailType.BOOMRANGE;
        state = Phase.GO;
    }

    public override void Init()
    {
        base.Init();
        end = false;

        InitStat(Phase.GO);
    }

    public void InitStat(Phase state)
    {
        this.state = state;
        if (state == Phase.GO)
        {
            totalDist = (StartPoint - EndPoint).magnitude;
            currentPos = StartPoint;
            velocity = (EndPoint - StartPoint).normalized;
            velocity.y = 0;
        }
        else if (state == Phase.STAY)
        {
            timeAcc = 0;
        }
        else if (state == Phase.BACK)
        {
            StartPoint = EndPoint;
            EndPoint = target.GetGePosition();

            currentPos = StartPoint;
            velocity = (EndPoint - StartPoint).normalized;
            velocity.y = 0;

            SetTotalDist(100);
        }
    }

    public override void OnTick(int deltaTime)
    {
        if (state == Phase.GO)
        {
            TimeAcc += deltaTime;
            Vector3 moveDist = GetMoveDir() * MoveSpeed * deltaTime / 1000f;
            currentPos += moveDist;

            if (ReachEndPoint())
            {
                InitStat(Phase.STAY);
            }
        }
        else if (state == Phase.STAY)
        {
            timeAcc += deltaTime;
            if (timeAcc >= stayDuration)
            {
                InitStat(Phase.BACK);
            }
        }
        else if (state == Phase.BACK)
        {
            TimeAcc += deltaTime;
            Vector3 moveDist = GetMoveDir() * MoveSpeed * deltaTime / 1000f;
            currentPos += moveDist;

            if (ReachTargetActorPos() && !end)
            {
                end = true;
                //target.TriggerEvent(BeEventType.onBoomerangHit, new object[] { owner });
                target.TriggerEventNew(BeEventType.onBoomerangHit, new EventParam(){m_Obj = owner});
            }
        }
    }

    public bool ReachEndPoint()
    {
        return (currentPos - EndPoint).magnitude < 0.2f;
    }

    public bool ReachTargetActorPos()
    {
        /*
        var targetPos = target.GetGePosition();
        targetPos.y = currentPos.y;

        return (currentPos - targetPos).magnitude < 0.2f;
        */

        var targetPos = target.GetGePosition();
        return Mathf.Abs(targetPos.x - currentPos.x) < 0.2f;
    }
}