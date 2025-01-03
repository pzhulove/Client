using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill1709 : BeSkill
{
	protected IBeEventHandle handle = null;
	protected int buffID = 0;
	protected int mpReduce = 0;
	bool takeEffect = false;

	public Skill1709(int sid, int skillLevel):base(sid, skillLevel)
    {

    }

	public override void OnInit ()
	{
		buffID = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
	}

    public override void OnPostInit()
    {
        base.OnPostInit();
        mpReduce = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
    }

    public override void OnStart()
    {
		
    }

	public override void OnFinish ()
	{
		takeEffect = true;

		owner.buffController.TryAddBuff(buffID, int.MaxValue, level);
		owner.GetEntityData().ChangeMPReduce(mpReduce);

		RemoveHandle();
		handle = owner.RegisterEventNew(BeEventType.onMPChange, (args)=>{
			if (owner.GetEntityData().GetMP() < mpReduce)
				PressAgainCancel();
		});
	}

	public override void OnCancel ()
	{
		RemoveHandle();

		if (takeEffect)
		{
			owner.GetEntityData().ChangeMPReduce(-mpReduce);
			owner.buffController.RemoveBuff(buffID);
			takeEffect = false;
		}
	}

	public void RemoveHandle()
	{
		if (handle != null)
		{
			handle.Remove();
			handle = null;
		}
	}
}
