using UnityEngine;
using System.Collections;

public enum TrailStatus {
	TRAIL_FLY		= 0 ,
	TRAIL_STAY		= 1	,
	TRAIL_DEATH		= 2	,
}


public enum TrailType{
	NONE 		= 0,
	LINER 		= 1,
	PARABOLA 	= 2,
    BOOMRANGE   = 3,
}


public class TrailBehaviour {
    public delegate void Del();

    public Vector3 StartPoint = Vector3.zero;
    public Vector3 EndPoint = Vector3.zero;
    protected float TimeAcc = 0f;
    public float StayTime = 0f;
    public float DistPercent = 0f;
    public float MoveSpeed = 0.1f;

    protected float SpeedAcc = 0f;
    protected bool ReachDest = false;
    protected TrailStatus Status = TrailStatus.TRAIL_FLY;

    public Vector3 currentDir = Vector3.zero;
    public Vector3 currentPos = Vector3.zero;
    public Vector3 velocity = Vector3.zero;
    public float totalDist = 0f;

    protected TrailType Type = TrailType.LINER;

    public bool Directional = false;

    GeEffectEx effect = null;
    public BeActor target = null;

    public Del reachCallBack = null;

    public BeProjectile owner = null;


    public TrailBehaviour()
    {

    }

    public void SetTarget(BeActor tar)
    {
        target = tar;
    }

    public void SetEffect(string effectPath)
    {
        //if (GameClient.BattleSceneMain.GetInstance().GetSceneType() == GameClient.EBattleSceneType.TeamDungeon)
        //{
        //    effect = GameClient.BattleSceneMain.GetInstance().GetBeScene().currentGeScene.CreateEffect(effectPath, 100000, new Vec3(0, 0, 0));
        //}
        //else
#if !LOGIC_SERVER
 

        {
            effect = BattleMain.instance.Main.currentGeScene.CreateEffect(effectPath, 100000, new Vec3(0, 0, 0));
        }

 #endif

    }

    public void SetReachCallBack(TrailBehaviour.Del del)
    {
        reachCallBack = del;
    }

    public Vector3 GetCurrentDir()
    {
        return currentDir;
    }

    public Vector3 GetCurrentPos()
    {
        return currentPos;
    }

    public Vector3 GetMoveDir()
    {
        return velocity.normalized;
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    public void SetTotalDist(float dist)
    {
        totalDist = dist;
    }

    public void UpdateEndPoint(Vector3 pos, Vector3 renderEndPos)
    {
        if (EndPoint != pos)
        {
            EndPoint = pos;
            totalDist = Vector3.Distance(StartPoint, EndPoint);
            velocity = (EndPoint - StartPoint).normalized;
        }
    }

    public void Start()
    {
        ReachDest = false;
        TimeAcc = 0;
        Status = TrailStatus.TRAIL_FLY;

        Init();
    }

    public void Stay()
    {
        TimeAcc = 0;
        Status = TrailStatus.TRAIL_STAY;

        OnStay();
    }

    public void End()
    {
        ReachDest = true;
        DistPercent = 1.0f;
        Status = TrailStatus.TRAIL_DEATH;

        if (effect != null)
        {
            effect.Remove();
        }

        if (reachCallBack != null)
        {
            reachCallBack();
        }
    }

    public bool IsDead()
    {
        return Status == TrailStatus.TRAIL_DEATH;
    }

    public virtual void Init()
    {

    }

    public virtual void OnStay()
    {

    }

    public void Tick(int deltaTime)
    {
        OnTick(deltaTime);

        if (effect != null)
        {
            effect.SetPosition(new Vector3(currentPos.x, currentPos.y, currentPos.z));
        }
// 
        if (target != null && Type != TrailType.BOOMRANGE)
        {
            EndPoint = target.GetGePosition() + Global.Settings.offset;
        }

        if (!Directional && ReachTarget())
        {
            End();
        }
    }

    public bool ReachTarget()
    {
        return (currentPos - EndPoint).magnitude < 0.2f;
    }

    public virtual void OnTick(int deltaTime)
    {
    }

}
