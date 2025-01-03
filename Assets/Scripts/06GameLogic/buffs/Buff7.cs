using UnityEngine;
using System.Collections;
using GameClient;

//睡眠
public class Buff7 : BeBuff {


	BeEvent.BeEventHandleNew handler = null;
    protected bool m_IsBeHitFinish = false;

	public Buff7(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack)
	{}

    public override void OnReset()
    {
        m_IsBeHitFinish = false;
    }

	public override void OnStart ()
	{
        RemoveHanlder();
        if (!owner.CurrentBeBattle.HasFlag(BattleFlagType.Buff7Finish))
        {
            NeedRestoreTargetAction = false;
        }

        handler = owner.RegisterEventNew(BeEventType.onHit, args =>
        //handler = owner.RegisterEvent(BeEventType.onHit, (object[] args)=>
        {
            m_IsBeHitFinish = true;
            Finish();
        });
	}

	public override void OnFinish ()
	{
		RemoveHanlder();
        SwitState();
    }

    protected void SwitState()
    {
        if (owner.sgGetCurrentState() == (int)ActionState.AS_FALLGROUND)
        {
            if (!owner.CurrentBeBattle.HasFlag(BattleFlagType.Buff7Finish))
            {
                if (!m_IsBeHitFinish)
                {
                    owner.GetStateGraph().SwitchStates(new BeStateData((int)ActionState.AS_GETUP));
                }
                else
                {
                    //owner.Locomote(new BeStateData((int)ActionState.AS_FALLGROUND, 0, 0, 0, 0, 0, GlobalLogic.VALUE_300));
                    owner.Locomote(new BeStateData((int)ActionState.AS_FALLGROUND) { _timeout = GlobalLogic.VALUE_300 });
                }
            }
            else
            {
                //owner.Locomote(new BeStateData((int)ActionState.AS_FALLGROUND, 0, 0, 0, 0, 0, GlobalLogic.VALUE_300));
                owner.Locomote(new BeStateData((int)ActionState.AS_FALLGROUND) { _timeout = GlobalLogic.VALUE_300 });
            }

        }
    }

	void RemoveHanlder()
	{
		if (handler != null)
		{
			handler.Remove();
			handler = null;
		}
	}
}
