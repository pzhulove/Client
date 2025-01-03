using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class TrailManagerImp  {

    List<TrailBehaviour> trails = new List<TrailBehaviour>();
    bool pause = false;

    int timeAcc = 0;
    int interval = 32 *2;
    bool isDirty = false;

	public TrailManagerImp()
    {

    }

	private bool CheckCanRemove(TrailBehaviour trail)
	{
		return trail.IsDead();
	}

    public void Update(int delta)
    {
        if (isDirty)
            _RemoveTrail();

        timeAcc += delta;
        if (timeAcc >= interval)
        {
            timeAcc -= interval;
        }
        else
        {
            return;
        }

        for (int i=0; i<trails.Count; ++i)
        {
            var trail = trails[i];
            if (!CheckCanRemove(trail))
            {
                trail.Tick(interval);
            }
            else
                isDirty = true;
        }
    }

    void _RemoveTrail()
    {
        trails.RemoveAll(CheckCanRemove);
        isDirty = false;
    }

	public ParabolaBehaviour AddParabolaTrail(Vector3 startPos, BeActor target, Vector3 startVel, Vector3 endVel, string effectPath = null)
    {
        ParabolaBehaviour trail = new ParabolaBehaviour();

        trail.StartVelocity = startVel;
        trail.EndVelocity = endVel;
        trail.TotalTime = 2000;
		trail.TimeAccerlate = 100;//Global.Settings.TimeAccerlate;

        trail.StartPoint = startPos;

        trail.EndPoint = target.GetGePosition();

        trail.Start();
        trail.SetTotalDist(100);

        trail.target = target;

        //Logger.LogErrorFormat("start({0},{1},{2}) end({3},{4},{5})", startPos.x, startPos.y, startPos.z,
        //     trail.EndPoint.x, trail.EndPoint.y, trail.EndPoint.z);

        if (target != null)
            trail.SetTarget(target);
        if (effectPath != null)
        {
            trail.SetEffect(effectPath);
        }

        trails.Add(trail);

        return trail;
    }
}
	
public class DropTrail
{
	public delegate void OnTouchGround();

	public Vec3 speed;
	public Vec3 acc;
	public Vec3 position;

	public BeScene currentBeScene;

	public OnTouchGround touchGroundDelegate;

	private bool dead = false;

	public DropTrail()
	{
		dead = false;
	}

	public bool IsDead()
	{
		return dead;
	}

	public void UpdatePosition(int delta)
	{
		if (dead || currentBeScene == null)
			return;

		float fDeltime = delta;
		fDeltime /= 1000.0f;


		double xcoff = speed.x > 0.0f ? 1.0 : -1.0f;
		speed.x =(float)(speed.x + xcoff * acc.x * fDeltime);

		if (xcoff > 0)
			speed.x = Math.Max(0, speed.x);
		else if (xcoff < 0)
			speed.x = Math.Min(0, speed.x);

		double ycoff = speed.y > 0.0f ? 1.0 : -1.0f;
		speed.y = (float)(speed.y + ycoff * acc.y * fDeltime);

		if (ycoff > 0)
			speed.y = Math.Max(0, speed.y);
		else if (ycoff < 0)
			speed.y = Math.Min(0, speed.y);

		var backpos = position;
		var pos = backpos;

		pos.x += speed.x * fDeltime;
		if (currentBeScene.IsInBlockPlayer(new VInt3(pos)))
			pos.x = backpos.x;

		pos.y += speed.y * fDeltime;
		if (currentBeScene.IsInBlockPlayer(new VInt3(pos)))
			pos.y = backpos.y;

		position.x = pos.x;
		position.y = pos.y;

		if (position.z > 0f)
		{
			speed.z -= acc.z * fDeltime;
			position.z += speed.z *fDeltime;

			if (position.z <= 0.0f)
			{
				position.z = 0f;
				speed.z = 0f;

				if (touchGroundDelegate != null)
					touchGroundDelegate();

				dead = true;
				currentBeScene = null;
			}
		}

		//Logger.LogErrorFormat("pos:({0},{1},{2})", position.x, position.y, position.z);
	}
}
