using UnityEngine;
using System.Collections;
using GameClient;
using ProtoTable;

public class Buff24 : BeBuff 
{
	private BeEvent.BeEventHandleNew handler = null;

	public Buff24(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack)
	{
    }

    private int mType = -1;
	private int mHealRate = GlobalLogic.VALUE_1000;
	private int mRate = GlobalLogic.VALUE_1000;

    public override void OnReset()
    {
        mType = -1;
        mHealRate = GlobalLogic.VALUE_1000;
        mRate = GlobalLogic.VALUE_1000;
    }

    private void _initData()
    {
        mType = TableManager.GetValueFromUnionCell(buffData.ValueA[0], level);
        mRate = TableManager.GetValueFromUnionCell(buffData.ValueB[0], level);
        mHealRate = TableManager.GetValueFromUnionCell(buffData.ValueC[0], level);
    }

	public override void OnStart()
	{
        _initData();

        handler = owner.RegisterEventNew(BeEventType.onHit, args =>
        //handler = owner.RegisterEvent(BeEventType.onHit, args =>
        {
            //if (args.Length >= 4)
            //{
                var damageType = (ProtoTable.EffectTable.eDamageType)(args.m_Int3);
                var damage = args.m_Int;
                var target = args.m_Obj as BeActor;

                if (target != null)
                {
                    if ((int)damageType == mType)
                    {
							var toAttackerDamage = (int)(damage * VFactor.NewVFactor(mRate, (long)GlobalLogic.VALUE_1000));

                        Logger.LogProcessFormat("{0} : {1} damage to attacker {2} with damage {3}", owner.GetName(), damageType, target.GetName(), toAttackerDamage);
                        target.DoHurt(toAttackerDamage, null, HitTextType.NORMAL, owner);
                        // TODO 血条动画会有点问题, oh no 
							var healValue = damage * mHealRate / GlobalLogic.VALUE_1000;
                        if (healValue > 0)
                        {
							owner.m_pkGeActor.CreateEffect(6, new Vec3(0, 0, 0));
                            owner.DoHeal(healValue, true);
							
                            Logger.LogProcessFormat("{0} heal hp {1}", owner.GetName(), healValue);
                        }
                    }
                }
            //}
        });
	}


	public override void OnFinish()
	{
        if (null != handler)
        {
            handler.Remove();
            //owner.RemoveEvent(handler);
            handler = null;
        }
	}
}
