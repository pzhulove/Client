using UnityEngine;
using System.Collections;
using GameClient;

//感电
public class Buff6 : BeAbnormalBuff {


	IBeEventHandle handler = null;
	
	public Buff6(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack)
	{}

	public override void OnStart ()
    {
        base.OnStart();
        RemoveHanlder();
        handler = owner.RegisterEventNew(BeEventType.onHitAfterAddBuff, args =>
        //handler = owner.RegisterEvent(BeEventType.onHitAfterAddBuff, (object[] args)=>
        {
            //if (null != args && null != args[1])
            //{
                bool triggerFlashHurt = args.m_Bool;
                if (triggerFlashHurt)
                {
                    if(overlayType == BuffOverlayType.OVERLAY_DAMAGE)
                    {
                        DoWorkForInterval();
                    }
                    else if(overlayType == BuffOverlayType.OVERLAY_ALONE && abnormalBuffData.isFirst)
                    {
                        int damage = owner.buffController.GetAbnormalDamage(buffID);
                        if (damage > 0)
                        {
	                        /*interParams[0] = this;
	                        var subParams = interParams[1] as int[];
	                        subParams[0] = damage;
	                        owner.TriggerEvent(BeEventType.AbnormalBuffHurt, interParams);
	                        damage = subParams[0];*/

	                        var ret = owner.TriggerEventNew(BeEventType.AbnormalBuffHurt, new GameClient.EventParam(){m_Obj = this,m_Int = damage});
	                        damage = ret.m_Int;
	                        
	                        if (owner.CurrentBeScene != null)
		                        owner.CurrentBeScene.TriggerEventNew(BeEventSceneType.onHurtByAbnormalBuff, new EventParam(){m_Int = damage, m_Obj = releaser, m_Obj2 = owner, m_Int2 = skillId});
                               // owner.CurrentBeScene.TriggerEvent(BeEventSceneType.onHurtByAbnormalBuff, new object[] { damage, releaser, owner, skillId });
                            DoBuffAttack(damage);
                        }
                    }
                }
            //}
        });
    }

	public override void OnFinish ()
	{
        base.OnFinish();
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
