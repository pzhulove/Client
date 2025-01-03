using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

public class Skill1109 : BeSkill {

	int markProb;
	List<int> relatedSkills = null;
	int effectIDCaster = 0;
	int effectIDTarget = 0;
    private BeEvent.BeEventHandleNew handle = null;

	public Skill1109(int sid, int skillLevel):base(sid, skillLevel)
	{

	}

	public override void OnInit ()
	{
		markProb = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
		relatedSkills = GetEffectSkills(skillData.ValueB, level);
		effectIDCaster = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
		effectIDTarget = TableManager.GetValueFromUnionCell(skillData.ValueC[1], level);
	}
		

	public override void OnPostInit ()
	{
        if(handle!=null)
        {
            handle.Remove();
            handle = null;
        }

        handle = owner.RegisterEventNew(BeEventType.onHitOther, args => 
        {
			BeActor target = args.m_Obj as BeActor;	
			int hurtID = args.m_Int;

			var data = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hurtID);
			if (data != null && relatedSkills.Contains(data.SkillID) && FrameRandom.Range1000()<=markProb)
			{
				DoWork(target);
			}
		});
	}

	void DoWork(BeActor target)
	{
		if (target != null)
			AddBuff(target, effectIDTarget);
		
		if (owner != null)
			AddBuff(owner, effectIDCaster);
	}

}
