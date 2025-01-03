using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;

//魔笛-点名
public class Skill5569 : BeSkill {

    int buffInfoID = 2020030; //保护罩buffid
	int pillarID = 572; 	//柱子怪物ID

	BeActor pillarSelected;

    public Skill5569(int sid, int skillLevel): base(sid, skillLevel)
	{
        buffInfoID = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        pillarID = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
	}

	public override void OnStart ()
	{
		List<BeActor> pillars = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindActorById(pillars, pillarID);
		if (pillars != null && pillars.Count > 0)
		{
			int randIndex = FrameRandom.InRange(0, pillars.Count-1);
            pillarSelected = pillars[randIndex];
            pillarSelected.buffController.AddTriggerBuff(buffInfoID);
		}
		GamePool.ListPool<BeActor>.Release(pillars);
	}

    public override void OnCancel()
    {
        if (pillarSelected != null)
        {
            pillarSelected.buffController.RemoveTriggerBuff(buffInfoID);
            pillarSelected = null;
        }
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
