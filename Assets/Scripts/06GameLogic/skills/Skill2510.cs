using UnityEngine;
using System.Collections;

public class Skill1008 : Skill2510 {
	public Skill1008(int sid, int skillLevel):base(sid, skillLevel){
		nextSkillID = 1111;
	}
}


public class Skill2510 : BeSkill {

	protected int nextSkillID = 2511;
	protected int skillPhase = 0;

	public Skill2510(int sid, int skillLevel):base(sid, skillLevel){}

	public override void OnInit()
	{
		
	}

	public override void OnClickAgain ()
	{
		if (skillPhase > 0)
		{
			if (IsCanceled() || owner.GetPosition().z > VInt.Float2VIntValue(0.1f))
				return;
				
			owner.CancelSkill(skillID);
			UseNextSkill();
		}
	}

	public override void OnStart ()
	{
		skillPhase = 1;
	}

	void UseNextSkill()
	{
		if (owner.HasSkill(nextSkillID))
		{
			owner.delayCaller.DelayCall(100, ()=>{
				owner.UseSkill(nextSkillID, true);
			});
		}
	}

	public override void OnCancel ()
	{
		skillPhase = 0;
	}

	public override void OnFinish ()
	{
		OnCancel();
	}
}
