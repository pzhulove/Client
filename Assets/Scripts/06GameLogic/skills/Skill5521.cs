using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill5521 : BeSkill {

	int buffInfoID = 2020030; //保护罩buff info ID
	int pillarID = 572; 	//柱子怪物ID

	BeActor pillarSelected;

	public Skill5521(int sid, int skillLevel):base(sid, skillLevel)
	{
		buffInfoID = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
		pillarID = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
	}

	public override void OnStart ()
	{
		List<BeActor> pillars = GamePool.ListPool<BeActor>.Get();
		owner.CurrentBeScene.FindActorInRange(pillars, owner.GetPosition(), int.MaxValue, owner.GetCamp(), pillarID);
		if (pillars != null && pillars.Count > 0)
		{
			int randIndex = FrameRandom.InRange(0, pillars.Count);
			pillars[randIndex].buffController.AddTriggerBuff(buffInfoID);
			pillarSelected = pillars[randIndex];
		}

		GamePool.ListPool<BeActor>.Release(pillars);
	}

	public override void OnFinish ()
	{
		if (pillarSelected != null)
		{
			pillarSelected.buffController.RemoveTriggerBuff(buffInfoID);
			pillarSelected = null;
		}
	}
}
