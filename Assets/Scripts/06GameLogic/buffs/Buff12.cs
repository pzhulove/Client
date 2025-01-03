using UnityEngine;
using System.Collections;

//生命周期buff
public class Buff12 : BeBuff {

	public bool showDisappearEffect = true;

	public Buff12(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack)
	{}

	public override void OnReset() 
	{
		showDisappearEffect = true;
	}

	public override void OnFinish ()
	{
		if (owner != null && !owner.IsDead())
		{
			owner.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE_NO_ALPHA, GlobalLogic.VALUE_5000);
			if (owner.aiManager != null)
			{
				owner.aiManager.Stop();
				owner.ClearMoveSpeed();
			}

#if !SERVER_LOGIC 

			owner.m_pkGeActor.ChangeSurface("无敌", 0f);
            if (showDisappearEffect && owner.GetEntityData().type != (int)ProtoTable.UnitTable.eType.SKILL_MONSTER)
			{
				owner.CurrentBeScene.currentGeScene.CreateEffect(1017, new Vec3(owner.GetPosition().fx, owner.GetPosition().fy, 0.5f));
			}

 #endif


			if (owner.HasAction(Global.ACTION_EXPDEAD2) || owner.HasAction(Global.ACTION_EXPDEAD3))
			{
				owner.DoDead();
			}
			else 
			{
				if (owner.m_iEntityLifeState != (int)EntityLifeState.ELS_CANREMOVE)
				{
					//int[] vals = new int[1];
					//vals[0] = 0;
					//owner.TriggerEvent(BeEventType.onDead, new object[] { vals, false,owner });

					owner.TriggerEventNew(BeEventType.onDead, new GameClient.EventParam() { m_Bool = true, m_Bool2 = false, m_Obj = owner });

                    owner.SetIsDead(true);
					var actor = owner;
					owner.delayCaller.DelayCall(300, ()=>{
						actor.OnDead ();
						actor.SetLifeState(EntityLifeState.ELS_CANREMOVE);
					});
				}
					
			}


		}
	}
}
