using UnityEngine;
using System.Collections;
using GameClient;

/// <summary>
/// 变大
/// </summary>
public class Buff23 : BeBuff 
{
    private VInt mOriginScale = VInt.one;
	private BeEvent.BeEventHandleNew handler = null;

    private VInt mScaleRate = VInt.one;

	private int mDamageToAttackerRate = GlobalLogic.VALUE_1000;
    
	public Buff23(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack)
	{}

    public override void OnReset()
    {
        mOriginScale = VInt.one;
        mScaleRate = VInt.one;
        mDamageToAttackerRate = GlobalLogic.VALUE_1000;
    }

    private void _initData()
    {
        mDamageToAttackerRate = TableManager.GetValueFromUnionCell(buffData.ValueA[0], level);
		mScaleRate = VInt.NewVInt(TableManager.GetValueFromUnionCell(buffData.ValueB[0], level),GlobalLogic.VALUE_1000);
    }

	public override void OnStart()
	{
        _initData();

        mOriginScale = owner.GetScale();
        owner.SetScale(mScaleRate);

        handler = owner.RegisterEventNew(BeEventType.onHit, args =>
        //handler = owner.RegisterEvent(BeEventType.onHit, args =>
        {
            //if (args.Length >= 3)
            //{
                var damage = args.m_Int;
                var target = args.m_Obj as BeActor;

				var toAttackerDamage = damage * GlobalLogic.VALUE_1000 / mDamageToAttackerRate;

                if (target != null)
                {
                    Logger.LogProcessFormat("{0} damage to attacker {1} with damage {2}", owner.GetName(), target.GetName(), toAttackerDamage);
                    target.DoHurt(toAttackerDamage, null, HitTextType.NORMAL, owner);
                    // TODO 血条动画会有点问题, oh no 
					owner.m_pkGeActor.CreateEffect(6, new Vec3(0, 0, 0));
                    owner.DoHeal(damage, true);
                }
            //}
        });
	}


	public override void OnFinish()
	{
        owner.SetScale(mOriginScale);
        if (null != handler)
        {
            handler.Remove();
            //owner.RemoveEvent(handler);
            handler = null;
        }
	}
}
