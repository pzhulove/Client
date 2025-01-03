using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
 * 
*/

public class Mechanism16 : BeMechanism {

	int effectID;
	int duration;
	int hitPercent;
	const string BUFF_TEXT = "爆炸";

	public Mechanism16(int mid, int lv):base(mid, lv){}
	public override void OnInit ()
	{
		effectID = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
		duration = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
		hitPercent = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
	}

	public override void OnStart ()
	{
		owner.RegisterEventNew(BeEventType.onHit, (args)=>{
			var attacker = (BeActor)args.m_Obj;
            if (attacker != null && attacker.GetEntityData() != null && !attacker.GetEntityData().isPet)
            {
                OnHit(attacker);
            }
        });
	}


	void OnHit(BeActor attacker)
	{
		SpellBar bar = null;
		var dur = attacker.GetSpellBarDuration(eDungeonCharactorBar.Fire);
		if (dur <= 0)
		{
			bar = attacker.StartSpellBar(eDungeonCharactorBar.Fire, duration, true, BUFF_TEXT);
			bar.autoAcc = false;
			bar.reverse = true;
		}

		attacker.AddSpellBarProgress(eDungeonCharactorBar.Fire, new VFactor(hitPercent, 100));
		int progress = attacker.GetSpelBarProgress(eDungeonCharactorBar.Fire);
		if (progress >= 1)
		{
			//owner.DoAttackTo(attacker, effectID);
			var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(effectID);
			if (hurtData != null)
			{
				owner.TryAddEntity(hurtData, attacker.GetPosition());
			}
		}
	}
}
