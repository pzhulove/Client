using UnityEngine;
using System.Collections;


public class Skill2534 : Skill1113
{
	public Skill2534(int sid, int skillLevel):base(sid, skillLevel)
	{
        skillPhases = new int[] { 0, 25341, 25342 };
        entityIDs = new int[] { 60285, 60286, 60287 };
    }
}


public class Skill1113 : BeSkill {

	protected IBeEventHandle handle = null;

    protected int phase = 0;

    protected int[] skillPhases = null;
    protected int[] entityIDs = null;

    protected BeProjectile[] savedProjectisles = new BeProjectile[2];
    protected int projectileCount = 0;

	public Skill1113(int sid, int skillLevel):base(sid, skillLevel)
	{
        skillPhases = new int[] { 0, 11131, 11133 };
        entityIDs = new int[] { 60260, 60261, 60262 };
    }

	public override void OnInit ()
	{
	}

	public override void OnStart ()
	{
        RemoveHandle();

        phase = 0;
        projectileCount = 0;

        handle = owner.RegisterEventNew(BeEventType.onBoomerangHit, (args) => {
            if (owner.IsDead())
                return;

            if (phase >= 2)
            {
                RemoveHandle();
                return;
            }

            var projectile = args.m_Obj as BeProjectile;

            savedProjectisles[projectileCount++] = projectile;
            if (projectileCount >= 2)
            {
                phase++;

                if (CanLaunchBoomerang(skillPhases[phase]))
                {
                   // Logger.LogErrorFormat("use skill{0}", skillPhases[phase]);

                    for(int i=0; i< savedProjectisles.Length; ++i)
                    {
                        if (savedProjectisles[i] != null)
                            savedProjectisles[i].DoDie();
                    }

                    projectileCount = 0;
                    owner.UseSkill(skillPhases[phase], true);
                }
                else
                {
                    projectileCount = 0;
                    RemoveHandle();
                }
            }
            else
            {
                return;
            }
        });
    }

	public override void OnFinish ()
	{

	}

	public override void OnCancel ()
	{
	
	}

    protected int GetEntityPhaseID(int id)
    {
        for(int i=0; i< entityIDs.Length; ++i)
        {
            if (id == entityIDs[i])
                return i;
        }

        return -1;
    }
    
    protected bool CanLaunchBoomerang(int skillID)
    {
        return owner.CanUseSkill(skillID);
    }

    protected void RemoveHandle()
    {
        if (handle != null)
        {
            handle.Remove();
            handle = null;
        }

    }
}
