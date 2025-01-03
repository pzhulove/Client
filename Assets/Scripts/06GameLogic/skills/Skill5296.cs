using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill5296 : BeSkill
{
	int thunderID = 60107;
	int pillarID = 1780011;
	VInt pillarSelectRangeRadius = VInt.one.i *3;
	int delay = 0;
	int interval = 100;
	int maxPillarNum = 6;

    public Skill5296(int sid, int skillLevel):base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
		thunderID = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
		pillarID = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
		pillarSelectRangeRadius = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueC[0], level),1000);
		maxPillarNum = TableManager.GetValueFromUnionCell(skillData.ValueD[0], level);
		interval = TableManager.GetValueFromUnionCell(skillData.ValueE[0], level);
    }

	public override void OnStart ()
	{
		var pillarGuy = owner.GetOwner();
		if (pillarGuy != null && !pillarGuy.IsDeadOrRemoved()) {
			var pos = pillarGuy.GetPosition();
			pos.z = 0;
			owner.AddEntity(thunderID, pos);
			//Logger.LogErrorFormat("hit pillarGuy!!!!");
		}

		if (FrameRandom.Range100() > 30)
		{
			List<BeActor> pillars = GamePool.ListPool<BeActor>.Get();
			
			owner.CurrentBeScene.FindActorInRange(pillars, owner.GetPosition(), pillarSelectRangeRadius, owner.GetCamp(), pillarID);

			if (pillars.Count > 0)
			{
				int randNum = FrameRandom.InRange(1, pillars.Count + 1);
				randNum = Mathf.Min(randNum, maxPillarNum);

				//Logger.LogErrorFormat("pillar:{0} randNum:{1}", pillars.Count, randNum);

				for(int i=0; i<randNum; ++i)
				{
					BeActor actor = pillars[i];
					owner.delayCaller.DelayCall(delay+(i+1)*interval, ()=>{
						owner.AddEntity(thunderID, actor.GetPosition());
					});
				}
			}

			GamePool.ListPool<BeActor>.Release(pillars);
		}
	}

}
