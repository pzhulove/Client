using UnityEngine;
using System.Collections;
using GameClient;

//隐身
public class Buff16 : BeBuff {

	BeEvent.BeEventHandleNew handler = null;

	public Buff16(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack)
	{}

	public override void OnStart ()
	{
		RemoveHanlder();
        handler = owner.RegisterEventNew(BeEventType.onHit, args => 
        //handler = owner.RegisterEvent(BeEventType.onHit, (object[] args)=>{
        {
        
            Finish();
		});
	}

	public override void OnFinish ()
	{
		RemoveHanlder();
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
