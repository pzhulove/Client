using UnityEngine;
using System.Collections;

//波动刻印
public class Buff171001 : BeBuff {

	Mechanism22 runeManager = null;
	int mechanismID = 1001;

	public Buff171001(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack)
	{}

	public override void OnReset()
	{
		runeManager = null;
	}

	public override void OnStart ()
	{
		if (runeManager == null)
			runeManager = owner.GetMechanism(mechanismID) as Mechanism22;

		DoWorkForInterval();
	}

	public override void DoWorkForInterval ()
	{
		if (runeManager != null)
		{
			runeManager.AddRune();
		}
	}
}

public class Buff171002 : BeBuff
{
    Mechanism22 runeManager = null;
    int mechanismID = 1001;

    public Buff171002(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack)
	{ }

	public override void OnReset()
	{
		runeManager = null;
	}

    public override void OnStart()
    {
        if (runeManager == null)
            runeManager = owner.GetMechanism(mechanismID) as Mechanism22;

        if (runeManager != null)
        {
            runeManager.AddRune();
        }
    }

}
