using UnityEngine;
using System.Collections;

public class Skill1112 : Skill2523
{
	public Skill1112(int sid, int skillLevel):base(sid, skillLevel){
		sxtSkillID = 1008;
		nextSkillID = 1111;
	}
}

public class Skill2523 : BeSkill {

	protected int sxtSkillID = 2510;
	protected int nextSkillID = 2511;

	public Skill2523(int sid, int skillLevel):base(sid, skillLevel){}

	public override void OnInit()
	{
		
	}

	public override void OnPostInit ()
	{
		if (owner != null)
		{
			var nextSkill = owner.GetSkill(nextSkillID);
			if (nextSkill != null)
				nextSkill.level = level;

			var skill = owner.GetSkill(sxtSkillID);
			if (skill != null)
			{
				skill.pressMode = SkillPressMode.TWO_PRESS;
			}
		}
	}
}
