using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;

//雷龙出海
public class Skill3118 : BeSkill
{
    int delayLaunch = 200;
    int mechanismID = 1110;
    Mechanism70 mechanism = null;

    public Skill3118(int sid, int skillLevel): base(sid, skillLevel){}

    public override void OnEnterPhase(int phase)
    {
        if (phase == 3)
        {
            owner.delayCaller.DelayCall(delayLaunch, () =>
            {
                DoStart();
            });
        }
    }

    public override void OnFinish ()
	{
		DoRestore ();
	}

	public override void OnCancel ()
	{
		DoRestore ();
	}
    void DoStart()
    {
        // owner.RemoveMechanism(1110);
        mechanism = owner.AddMechanism(1110, 1) as Mechanism70;
        if (mechanism != null)
        {
            mechanism.skill = this;
            mechanism.canControl = true;
            mechanism.DoStart();
        }
    }

    public void AttackCamera(BeEntity entity)
    {
        if (entity == null)
            return;
#if !LOGIC_SERVER
        if (owner.isLocalActor)
        {
            if (entity != owner && !skillState.IsRunning())
                entity = owner;
            
            BattleMain.instance.GetDungeonManager().GetGeScene().AttachCameraTo(entity.m_pkGeActor);
        }
#endif
    }

	void DoRestore()
	{
        if (mechanism != null)
        {
            mechanism.canControl = false;
            mechanism.Finish();
            mechanism.SetDead();
            mechanism = null;
        }

        AttackCamera(owner);
	}

	
}
