using UnityEngine;
using System.Collections;

public class Skill1205 : BeSkill {
    IBeEventHandle handler;
    public Skill1205(int sid, int skillLevel):base(sid, skillLevel)
    {

    }

    public override void OnStart()
    {
        base.OnStart();

        if (owner != null)
        {
         	
			RemoveHandle();

            handler = owner.RegisterEventNew(BeEventType.onSummon, (args) =>
            {
                SetInnerState(InnerState.LAUNCH);

                BeActor summonActor = args.m_Obj as BeActor;
                if (summonActor != null)
                {
                    //设置为不收阻挡
                    summonActor.stateController.SetAbilityEnable(BeAbilityType.BLOCK, false);
                    summonActor.SetPosition(effectPos);
                    summonActor.attribute = owner.attribute;       
                }

				RemoveHandle();
            });
        }
    }

	public override void OnCancel ()
	{
		RemoveHandle();
	}

	public override void OnFinish ()
	{
		RemoveHandle();
	}

	void RemoveHandle()
	{
		if (handler != null)
		{
			handler.Remove();
			handler = null;
		}
	}
}
