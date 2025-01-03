using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
 * 攻击指定ID怪物伤害加成
*/

public class Mechanism18 : BeMechanism {

	CrypticInt32 percent = 0;
	List <int> monsterIDs = new List<int>();

	public Mechanism18(int mid, int lv):base(mid, lv){}

	public override void OnReset()
	{
		monsterIDs.Clear();	
	}


	public override void OnInit ()
	{
		percent = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
		for(int i=0; i<data.ValueB.Count; ++i)
		{
			int mid = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
			monsterIDs.Add(mid);
		}
	}

	public override void OnStart ()
	{
        if (owner == null)
            return;

		Deal(owner);
        handleB = owner.RegisterEventNew(BeEventType.onAfterGenBullet, (args)=>{
			var entity = args.m_Obj as BeProjectile;
			if (entity != null)
			{
				Deal(entity);
			}
		});
	}


	protected void Deal(BeEntity entity)
	{
        //handleA = entity.RegisterEvent(BeEventType.onAfterFinalDamage, (object[] args)=>{
        handleA = entity.RegisterEventNew(BeEventType.onAfterFinalDamage, args => {

            BeActor actor = args.m_Obj as BeActor;
			if (actor != null)
			{
				var attribute = actor.GetEntityData();
				if (!BeUtility.IsMonsterIDEqualList(monsterIDs, attribute.simpleMonsterID))
					return;

				//int[] vals = (int[])args[0];
				int damage = args.m_Int;

                args.m_Int = damage * (VFactor.one + VFactor.NewVFactor(percent, (long)GlobalLogic.VALUE_1000));

				//Logger.LogErrorFormat("damage {0} -> {1}", damage, vals[0]);
			}
		});
	}
}
