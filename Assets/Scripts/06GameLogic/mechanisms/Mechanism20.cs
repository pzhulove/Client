using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
 * 反伤
*/

enum ReflectType
{
	PHYSICS = 1,
	MAGIC = 2,
}


public class Mechanism20 : BeMechanism {

	ReflectType mType;
	int mHealRate = 1000;
	int mRate = 1000;

	public Mechanism20(int mid, int lv):base(mid, lv){}


	public override void OnInit ()
	{
		mType = (ReflectType)TableManager.GetValueFromUnionCell(data.ValueA[0], level);
		mRate = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
		mHealRate = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
	}

	public override void OnStart ()
	{
        handleA = OwnerRegisterEventNew(BeEventType.onHit, args =>
        //handler = owner.RegisterEvent(BeEventType.onHit, args =>
        {
				//if (args.Length >= 4)
				//{
					var damageType = (ProtoTable.EffectTable.eDamageType)(args.m_Int3);
					var damage = args.m_Int;
					var target = args.m_Obj as BeActor;
					var hurtID = args.m_Int4;

					if (target != null && hurtID != 999801)
					{
						if ((ReflectType)damageType == mType)
						{
							var toAttackerDamage = IntMath.Float2Int(damage * (mRate / (float)(GlobalLogic.VALUE_1000)));

                            //Logger.LogErrorFormat("{0} : {1} damage to attacker {2} with damage {3}", owner.GetName(), damageType, target.GetName(), toAttackerDamage);
                            if (toAttackerDamage != 0)
                            {
                                target.DoHurt(toAttackerDamage, null, GameClient.HitTextType.NORMAL, owner);
                            }
							// TODO 血条动画会有点问题, oh no 
							var healValue = damage * mHealRate / 1000;
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
}
