using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
 * 波动刻印
*/
public class Skill1710 : BeSkill
{
	protected int buffID = 0;
	protected int mpReduce = 0;
	int mechanismID = 1001;
	public Mechanism22 runeManager = null;

	int replaceNormalAttackID = 0;
	int backupAttackID;

    int replaceSkillID1 = 0;
    int replacedSkillID1 = 0;

	protected IBeEventHandle handle = null;
    protected IBeEventHandle handlePreSetAction = null;

	bool started = false;

    public Skill1710(int sid, int skillLevel):base(sid, skillLevel)
    {

    }

	public override void OnInit ()
	{
		buffID = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
		replaceNormalAttackID = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);

        replaceSkillID1 = TableManager.GetValueFromUnionCell(skillData.ValueD[0], 1);
        replacedSkillID1 = TableManager.GetValueFromUnionCell(skillData.ValueE[0], 1);

        mpReduce = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
    }

	public override void OnPostInit ()
    {
        mpReduce = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
        //在这里增加波动刻印机制
        if (owner != null)
		{
			runeManager = owner.AddMechanism(mechanismID, level) as Mechanism22;
		}
	}

    public override void OnStart()
    {
		if (!started)
		{
			started = true;
			DoEffect();
		}
		else
		{
			started = false;
			Restore();
		}        
    }

	void DoEffect()
	{
		owner.buffController.TryAddBuff(buffID, int.MaxValue, level);
		owner.GetEntityData().ChangeMPReduce(mpReduce);

		backupAttackID = owner.GetEntityData().normalAttackID;
		owner.GetEntityData().normalAttackID = replaceNormalAttackID;

		RemoveHandle();
		handle = owner.RegisterEventNew(BeEventType.onMPChange, (args)=>{
			if (owner.GetEntityData().GetMP() < mpReduce)
				PressAgainCancel();
		});

		// 特定技能替换
		if (replaceSkillID1 != 0 && replacedSkillID1 != 0 
			&& owner.GetActionNameBySkillID(replaceSkillID1) != null && owner.GetActionNameBySkillID(replacedSkillID1) != null) {

			handlePreSetAction = owner.RegisterEventNew(BeEventType.onPreSetSkillAction, (GameClient.BeEvent.BeEventParam param) =>
				{
					// int[] skillIdList = (int[])args[0];
					// int curSkillId = skillIdList[0];
					// if (curSkillId == replaceSkillID1)
					// 	skillIdList[0] = replacedSkillID1;
                    if (param.m_Int == replaceSkillID1)
                    {
                        param.m_Int = replaceSkillID1;
                    }
				});
		}
	}

	public override void OnCancel ()
	{
		if (started)
		{
			started = false;
			Restore();

		}
	}


	void Restore()
	{
		if (owner != null)
		{
			RemoveHandle();
			owner.buffController.RemoveBuff(buffID);
			owner.GetEntityData().ChangeMPReduce(-mpReduce);
			owner.GetEntityData().normalAttackID = backupAttackID;
		}
	}

	public void RemoveHandle()
	{
		if (handle != null)
		{
			handle.Remove();
			handle = null;
		}
        if (handlePreSetAction != null)
        {
            handlePreSetAction.Remove();
            handlePreSetAction = null;
        }
    }

	public int GetRuneCount()
	{
		int num = 0;
		if (runeManager != null)
			num = runeManager.GetRuneCount();

		return num;
	}

}
