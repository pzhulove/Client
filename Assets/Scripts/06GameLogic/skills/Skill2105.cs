using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

public class Skill2105 : BeSkill {

	VInt range;
	int removeNum;
	int effectIDCaster;
	int effectIDEnemy;

	public Skill2105(int sid, int skillLevel):base(sid, skillLevel)
	{
		
	}

	public override void OnInit ()
	{
		range = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueA[0], level),GlobalLogic.VALUE_1000);
		effectIDCaster = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
		effectIDEnemy = TableManager.GetValueFromUnionCell(skillData.ValueC[1], level);
	}

    public override void OnPostInit()
    {
        removeNum = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
    }

    public override void OnFinish ()
	{
		List<BeActor> targets = GamePool.ListPool<BeActor>.Get();
		owner.CurrentBeScene.FindTargets(targets, owner, range);

		for(int i=0; i<targets.Count; ++i)
		{
            var target = targets[i];
			target.buffController.DispelBuff(BuffWorkType.GAINBUFF, removeNum);
			
			AddBuff(target, effectIDEnemy);
			AddBuff(owner, effectIDCaster);

            target.m_pkGeActor.CreateEffect(1014, new Vec3(0, 0, 0));
		}

		GamePool.ListPool<BeActor>.Release(targets);
	}
}
