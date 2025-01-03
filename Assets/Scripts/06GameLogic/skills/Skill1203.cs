using UnityEngine;
using System.Collections;

public class Skill1203 : BeSkill {
    IBeEventHandle handler;
    public Skill1203(int sid, int skillLevel):base(sid, skillLevel)
    {

    }

    public override void OnStart()
    {
        base.OnStart();

        if (owner != null)
        {
			RemoveHandle();

            handler = owner.RegisterEventNew(BeEventType.onAfterGenBullet, (args) =>
            {
                SetInnerState(InnerState.LAUNCH);

                BeProjectile project = args.m_Obj as BeProjectile;
                if (project != null && project.m_iResID == 60030)
                {
                    VInt3 projectPos = project.GetPosition();

                    if (owner.GetFace())
                        projectPos.x = effectPos.x + VInt.Float2VIntValue(2.26f);
                    else
                        projectPos.x = effectPos.x - VInt.Float2VIntValue(2.26f);
                    projectPos.y = effectPos.y;

                    project.SetPosition(projectPos);
                    project.SetFace(owner.GetFace(), true);
                    RemoveHandle();
                }

            });
        }
    }

	public override void OnFinish ()
	{
		RemoveHandle();
	}

	public override void OnCancel ()
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
