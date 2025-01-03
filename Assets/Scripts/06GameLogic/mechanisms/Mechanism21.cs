using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
 * 血池
*/


public class Mechanism21 : BeMechanism {

	VInt radius = VInt.one.i * 2;
	readonly VInt radiusSpeed = VInt.Float2VIntValue(2.06f);
	//VFactor speed = VInt.Conver(0.0004f);
	readonly VFactor speed = new VFactor(4,10000);
	
	GeEffectEx effect = null;
	float scalex = 1f;
	float scalez = 1f;
	int timeAcc = 0;
	readonly int checkInterval = 500;

	int hurtBuffID = 0;

	protected List<BeActor> inRangers = new List<BeActor>();

	public Mechanism21(int mid, int lv):base(mid, lv){}

	public override void OnReset()
	{
		radius = VInt.one.i * 2;
		effect = null;
		scalex = 1f;
		scalez = 1f;
		timeAcc = 0;
		hurtBuffID = 0;
		inRangers.Clear();
	}

	public override void OnInit ()
	{
		hurtBuffID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
	}

	public override void OnStart ()
	{
		effect = owner.m_pkGeActor.CreateEffect(7, Vec3.zero);
	}

	public override void OnUpdate (int deltaTime)
	{
		UpdateSize();
		UpdateCheckRange(deltaTime);
	}

	public override void OnFinish ()
	{
		if (effect != null)
		{
			owner.m_pkGeActor.DestroyEffect(effect);
			effect = null;
		}
	}

	void UpdateSize()
	{
		var orgPos = owner.GetPosition();
		var pos = owner.GetPosition();
		VInt dspeed = radiusSpeed.i * speed;

		int offset = VInt.half.i;
		bool scaled = false;

		bool top = (owner.GetPosition().y + radius) > (owner.CurrentBeScene.logicZSize.y + offset);
		bool down = (owner.GetPosition().y - radius) < (owner.CurrentBeScene.logicZSize.x - offset);
		bool left = (owner.GetPosition().x - radius) < (owner.CurrentBeScene.logicXSize.x - offset);
		bool right = (owner.GetPosition().x + radius) > (owner.CurrentBeScene.logicXSize.y + offset);

		if (!(top && down && left && right))
			radius += dspeed;

		if (!top || !down)
		{
			scaled = true;
			scalez += speed.single;
		}
			
		//向上移动
		if (top && !down)
			pos.y -= dspeed.i;
		//往下移动
		if (!top && down)
			pos.y += dspeed.i;

		if (!left || !right)
		{
			scaled = true;
			scalex += speed.single;
		}
			
		//往右移
		if (left && !right)
			pos.x += dspeed.i;
		//往左移动
		if (!left && right)
			pos.x -= dspeed.i;
		
		if (orgPos != pos)
			owner.SetPosition(pos);

		if (scaled && effect != null)
			effect.SetScale(scalex, 1.0f, scalez);
	}

	void UpdateCheckRange(int deltaTime)
	{
		timeAcc += deltaTime;
		if (timeAcc > checkInterval)
		{
			timeAcc -= checkInterval;
			List<BeActor> targets = GamePool.ListPool<BeActor>.Get();
			owner.CurrentBeScene.FindTargets(targets, owner, (radius-VInt.half));
			for(int i=0; i<inRangers.Count; ++i)
			{
				if (!targets.Contains(inRangers[i]))
				{
					OutRange(inRangers[i]);
					inRangers.RemoveAt(i);
					i--;
				}
			}

			for(int i=0; i<targets.Count; ++i)
			{
				if (!inRangers.Contains(targets[i]))
				{
					inRangers.Add(targets[i]);
					InRange(targets[i]);
				}
			}

			for(int i=0; i<inRangers.Count; ++i)
			{
				InRange(inRangers[i]);
			}

			GamePool.ListPool<BeActor>.Release(targets);
		}
	}

	void InRange(BeActor actor)
	{
		//Logger.LogErrorFormat("add buff:{0}", actor.GetName());
		if (hurtBuffID > 0 && !actor.IsDead() && actor.buffController.HasBuffByID(hurtBuffID) == null)
			actor.buffController.TryAddBuff(hurtBuffID, int.MaxValue, level);
	}

	void OutRange(BeActor actor)
	{
		//Logger.LogErrorFormat("remove buff:{0}", actor.GetName());
		if (hurtBuffID > 0)
			actor.buffController.RemoveBuff(hurtBuffID);
	}
}
