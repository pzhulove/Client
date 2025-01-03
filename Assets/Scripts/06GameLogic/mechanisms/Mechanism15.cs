using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
 * 
*/

public class Mechanism15 : BeMechanism {

	string barText;
	int buffRangeRadius = 2600;
	int duration = 7000;

	int triggerBuffInfoID = 500021;

	protected const int CHECK_INTERVAL = 200;
	protected int timeAcc;

	protected int checkInterval = CHECK_INTERVAL;

	BuffInfoData infoData;

	protected List<BeActor> inRangers = new List<BeActor>();
	protected List<InRangeData> inRangerUpdater = new List<InRangeData>();

	public Mechanism15(int mid, int lv):base(mid, lv){}

	public delegate void InRangeCallback(BeActor actor);

	private List<BeActor> targets = new List<BeActor>();

	public override void OnReset()
	{

		timeAcc = 0;
		infoData = null;
		inRangers.Clear();
		inRangerUpdater.Clear();
		targets.Clear();
	}

	//inRangeActor
	public class InRangeData
	{
		public BeActor actor;
		public int duration;
		int timeAcc;
		public InRangeCallback callback;
		bool finish = false;
		public bool reverse = false;

		public InRangeData(BeActor ba, int d, InRangeCallback act)
		{
			actor = ba;
			duration = d;
			callback = act;
		}

		public void ShowSpellBar(string barText)
		{
			if (true || actor.isLocalActor)
			{
				actor.StartSpellBar(eDungeonCharactorBar.Continue, duration, true, barText);
				timeAcc = actor.GetSpellBarDuration(eDungeonCharactorBar.Continue);
			}
		}

		public void Update(int delta)
		{
			if (finish)
				return;
			if (reverse)
				timeAcc -= delta;
			else
				timeAcc += delta;
			if (!reverse && timeAcc >= duration)
			{
				timeAcc -= duration;
				if (callback != null)
					callback(actor);
				finish = true;
			}
			if (reverse && timeAcc <= 0)
			{
				finish = true;
			}
		}

		public bool IsFinish()
		{
			return finish;
		}
	}


	public override void OnInit ()
	{
		barText = data.StringValueA[0];
		buffRangeRadius = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
		duration = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
		triggerBuffInfoID = TableManager.GetValueFromUnionCell(data.ValueC[0], level);

		infoData = new BuffInfoData(triggerBuffInfoID);
	}

	public override void OnStart ()
	{

	}

	public override void OnUpdate (int deltaTime)
	{
		UpdateCheckRange(deltaTime, owner);
		UpdateInRanger(deltaTime);
	}

	public override void OnFinish ()
	{
		for(int i=0; i<inRangers.Count; ++i)
		{
			OutRangeEffect(inRangers[i]);
		}
		inRangers.Clear();
	}

	private bool checkCanRemove(InRangeData item)
	{
		return item.IsFinish();	
	}

	public void UpdateInRanger(int delta)
	{
		for(int i=0; i<inRangerUpdater.Count; ++i)
		{
			var item = inRangerUpdater[i];
			if (item.IsFinish())
			{
				inRangers.Remove(item.actor);
			}
		}

		inRangerUpdater.RemoveAll(checkCanRemove);


		for(int i=0; i<inRangerUpdater.Count; ++i)
		{
			inRangerUpdater[i].Update(delta);
		}
	}

	private bool CheckCanRemove(BeActor item)
	{
		if (item.IsDead() || !targets.Contains(item))
		{
			//出圈
			OutRangeEffect(item);
			return true;
		}

		return false;
	}

	public void UpdateCheckRange(int delta, BeActor owner)
	{
		if (owner == null)
			return;

		timeAcc += delta;
		if (timeAcc >= checkInterval)
		{
			timeAcc -= checkInterval;


			//List<BeActor> targets = GamePool.ListPool<BeActor>.Get();

			//var targets = new List<BeActor>();
            owner.CurrentBeScene.FindTargets(targets, owner, VInt.NewVInt(buffRangeRadius, GlobalLogic.VALUE_1000));

			for(int i=0; i<targets.Count; ++i)
			{
				//进圈
				if (!inRangers.Contains(targets[i]) && CanAddTarget(targets[i])) {
					
					inRangers.Add(targets[i]);
					targets[i].inBossRange = true;

					var currentData = inRangerUpdater.Find(dataItem=>{
						return dataItem.actor == targets[i];	
					});

					if (currentData == null)
					{
						InRangeData data = new InRangeData(targets[i], duration, InRangeEffect);
						inRangerUpdater.Add(data);
						currentData = data;
					}
					else {
						currentData.reverse = true;
					}

					currentData.ShowSpellBar(barText);
				}
			}

			inRangers.RemoveAll(CheckCanRemove);

			//GamePool.ListPool<BeActor>.Release(targets);
		}
	}
		

	public void InRangeEffect(BeActor target)
	{
		if (target != null)
		{
			if (target.buffController.HasBuffByID(infoData.buffID) == null)
				target.buffController.TryAddBuff(infoData);
			target.inBossRange = false;
		}
	}

	public void OutRangeEffect(BeActor item)
	{
		item.inBossRange = false;
		var currentData = inRangerUpdater.Find(dataItem=>{
			return !dataItem.IsFinish() && dataItem.actor == item;	
		});

		if (currentData != null)
		{
			//currentData.reverse = true;
			if (true || item.isLocalActor)
				item.SetSpellBarReverse(eDungeonCharactorBar.Continue, true);
			inRangerUpdater.Remove(currentData);
		}
	}

	public bool CanAddTarget (BeActor target)
	{
		return !target.inBossRange && null == target.buffController.HasBuffByID(infoData.buffID);
	}


}
