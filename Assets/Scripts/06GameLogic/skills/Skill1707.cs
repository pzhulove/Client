using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
 * 波动爆发
*/
public class Skill1701 : Skill1707
{
	public Skill1701(int sid, int skillLevel):base(sid, skillLevel){}
}

/*
 * 邪光斩
*/
public class Skill1707 : BeSkill
{
	protected IBeEventHandle handle = null;
	protected int[] addBuffs = null;
	protected int[] projectileIDs = null;

	public Skill1707(int sid, int skillLevel):base(sid, skillLevel)
    {

    }

	public sealed override void OnInit ()
	{
        addBuffs = null;
        addBuffs = new int[skillData.ValueA.Count];
		for(int i=0; i<skillData.ValueA.Count; ++i)
		{
			addBuffs[i] = TableManager.GetValueFromUnionCell(skillData.ValueA[i], level);
		}

        projectileIDs = null;
        projectileIDs = new int[skillData.ValueB.Count];
		for(int i=0; i<skillData.ValueB.Count; ++i)
		{
			projectileIDs[i] = TableManager.GetValueFromUnionCell(skillData.ValueB[i], level);//skillData.ValueB[i];
		}
	}

	public sealed override void OnStart ()
	{
		RemoveHandler();

		handle = owner.RegisterEventNew(BeEventType.onAfterGenBullet, (args)=>{
			// var pj = args[0] as BeProjectile;
            var pj = args.m_Obj as BeProjectile;
			if (pj != null && IsTargetProjectile(pj.m_iResID))
			{
				var skill = owner.GetSkill(Global.BODONGKEYIN_SKILL_ID) as Skill1710;
				if (skill != null)
				{
					int runeCount = skill.GetRuneCount();
					var param = owner.TriggerEventNew(BeEventType.onCalcRuneAddDamage, new GameClient.EventParam() { m_Int = skillID, m_Int2 = runeCount });
					runeCount = param.m_Int2;
					if (runeCount > 0)
						pj.AddSkillBuff(addBuffs[runeCount - 1]);
				}
			}
		});
	}

	public sealed override void OnCancel ()
	{
		RemoveHandler();
	}

	public sealed override void OnFinish ()
	{
		RemoveHandler();
	}

	protected bool IsTargetProjectile(int pid)
	{
		if (projectileIDs == null)
			return false;

		for(int i=0; i<projectileIDs.Length; ++i)
			if (projectileIDs[i] == pid)
				return true;
		
		return false;
	}

	protected void RemoveHandler()
	{
		if (handle != null)
		{
			handle.Remove();
			handle = null;
		}
	}
}
