using UnityEngine;
using System.Collections;

public class Skill10000 : BeSkill {

	public int fixCD;

	public Skill10000(int sid, int skillLevel):base(sid, skillLevel)
	{

	}

	public override void OnStart()
	{
		if (owner != null)
		{
			//4070071, 30, 9025
			//owner.SummonHelp(owner.accompanyData);
		}
	}

	public override void OnUpdate (int iDeltime)
	{
#if !SERVER_LOGIC 

		if (button != null)
		{
			if (!CanForceUseSkill())
				button.RemoveEffect(ETCButton.eEffectType.onSummonAccompy);
			else
				button.AddEffect(ETCButton.eEffectType.onSummonAccompy);
		}

 #endif

	}

	public override int GetCurrentCD()
	{
		return fixCD * (VRate.one-cdReduceRate).factor;
	}

	public override bool CanForceUseSkill ()
	{
		if (isCooldown)
		{
#if !SERVER_LOGIC 

			if (button != null)
				button.RemoveEffect(ETCButton.eEffectType.onSummonAccompy);

 #endif

			return false;
		}

#if !SERVER_LOGIC 

		if (button != null)
			button.AddEffect(ETCButton.eEffectType.onSummonAccompy);

 #endif
 
		return true;
	}
}
