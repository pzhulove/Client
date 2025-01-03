using UnityEngine;
using System.Collections;
using GameClient;

public class Buff40 : BeBuff {
	public Buff40(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack)
	{}

	public override void OnStart ()
	{
		base.OnStart ();
		if (owner != null && owner.isLocalActor)
		{
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIDrug>();
			if (battleUI != null)
                battleUI.UseDrug();
		}
	}
}
