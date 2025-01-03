using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill1616 : BeSkill {
	int rangeRadius = 4000;
	int buffID = 161601;
	int overlayMax = 5;

	int buffLevelAdd = 2;
	int bleedBuffID = 5;

	int timeAcc = 0;
	int CHECK_INTERVAL = GlobalLogic.VALUE_1000;
	List<BeActor> inRangers = new List<BeActor>();

	public Skill1616(int sid, int skillLevel):base(sid, skillLevel)
	{

	}

	public override void OnInit ()
	{
		//Logger.LogError("skill 1616");
		buffID = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
		rangeRadius = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
		var data = TableManager.GetInstance().GetTableItem<ProtoTable.BuffTable>(buffID);
		if (data != null)
			overlayMax = data.OverlayLimit;

		buffLevelAdd = TableManager.GetValueFromUnionCell(skillData.ValueD[0], level);
		bleedBuffID = TableManager.GetValueFromUnionCell(skillData.ValueE[0], level);
	}

	public override void OnPostInit ()
	{
		skillState.SetRunning();

		for(int i=0; i<skillData.ValueC.Count; ++i)
		{
			int skillID = TableManager.GetValueFromUnionCell(skillData.ValueC[i], level);
			var skill = owner.GetSkill(skillID);
			if (skill != null)
			{
				BuffInfoData info = new BuffInfoData();
				info.buffID = bleedBuffID;
				info.level = buffLevelAdd;
				skill.buffEnhanceList.Add(buffID, info);
			}
		}
			

	}


	private bool CheckCanRemove(BeActor item)
	{
		if (item.IsDead() || item.GetDistance(owner) >
			VInt.NewVInt(rangeRadius,GlobalLogic.VALUE_1000).i || item.buffController.HasBuffByType(BuffType.BLEEDING) == null)
		{


			owner.buffController.RemoveBuff(owner.buffController.HasBuffByID(buffID));

			//Logger.LogErrorFormat("out remove buff left:{0}", owner.buffController.GetBuffCountByType(buffID));
			return true;
		}

		return false;
	}

	public override void OnUpdate (int iDeltime)
	{
		timeAcc += iDeltime;
		if (timeAcc >= CHECK_INTERVAL)
		{
			timeAcc -= CHECK_INTERVAL;

			inRangers.RemoveAll(CheckCanRemove);

			//var targets = new List<BeActor>();
			List<BeActor> targets = GamePool.ListPool<BeActor>.Get();
			List<BeActor> enemies = GamePool.ListPool<BeActor>.Get();

			owner.CurrentBeScene.FindTargets(enemies, owner, VInt.NewVInt(rangeRadius,GlobalLogic.VALUE_1000));
			for(int i=0; i<enemies.Count; ++i)
			{
				if (!enemies[i].IsDead() && enemies[i].buffController.HasBuffByType(BuffType.BLEEDING) != null)
					targets.Add(enemies[i]);
			}
				
			for(int i=0; i<targets.Count; ++i)
			{
				if (!inRangers.Contains(targets[i]) && inRangers.Count < overlayMax) {
					
					inRangers.Add(targets[i]);
					owner.buffController.TryAddBuff(buffID, -1, level);

					//Logger.LogErrorFormat("in range add buff total:{0}", owner.buffController.GetBuffCountByType(buffID));
				}
			}

			GamePool.ListPool<BeActor>.Release(enemies);
			GamePool.ListPool<BeActor>.Release(targets);
		}
	}
}
