using UnityEngine;
using System.Collections;

public class ParabolaBehaviour : TrailBehaviour {
    public Vector3 StartVelocity;
    public Vector3 EndVelocity;
    public float TotalTime = 0f;
    public float TimeStartTangent = 0.1f;
    public float TimeEndTangent = 1.0f;
    public float TimeAccerlate = 0.5f;
    Vector3 lastPos;

    public ParabolaBehaviour()
    {
        Type = TrailType.PARABOLA;
        totalDist = 0;
    }

    public override void Init()
    {
        base.Init();

        TimeAcc = 0;
        totalDist = Vector3.Distance(StartPoint, EndPoint);

        lastPos = StartPoint;
        currentPos = StartPoint;
        velocity = StartVelocity.normalized;
    }

    public override void OnTick(int deltaTime)
    {
        if (Status == TrailStatus.TRAIL_FLY)
        {
            TimeAcc += deltaTime;
            float deltaT = TimeAcc * 1.0f / TotalTime;

            DistPercent = deltaT;
            deltaT = 0.5f * TimeAcc * TimeAcc * TimeAccerlate / 1000f / 1000f;
            deltaT = deltaT / totalDist;
            if (deltaT > 1.0f)
                deltaT = 1.0f;

            float deltaTX2 = deltaT * deltaT;
            float deltaTX3 = deltaTX2 * deltaT;
            Vector3 newPos =
                StartPoint * (2.0f * deltaTX3 - 3.0f * deltaTX2 + 1.0f) +
                EndPoint * (-2.0f * deltaTX3 + 3.0f * deltaTX2) +
                StartVelocity * (deltaTX3 - 2.0f * deltaTX2 + deltaT) +
                EndVelocity * (deltaTX3 - deltaTX2);

            lastPos = currentPos;
            currentPos = newPos;

            velocity = currentPos - lastPos;

            if (deltaT >= 1.0f)
            {
                Stay();
            }
        }
        else if (Status == TrailStatus.TRAIL_STAY)
        {
            TimeAcc += deltaTime;
            if (TimeAcc > StayTime)
            {
                End();
            }
        }
    }


}
