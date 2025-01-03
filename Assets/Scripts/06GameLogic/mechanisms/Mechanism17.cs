using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
 * 攻击给定等级区间段的敌人伤害加成
*/

public class Mechanism17 : BeMechanism {

	int startLevel = 0;
	int endLevel = 0;

	CrypticInt32 percent = 0;
	List <ProtoTable.UnitTable.eType> monsterType = new List<ProtoTable.UnitTable.eType>();

	public Mechanism17(int mid, int lv):base(mid, lv){}

	public override void OnReset()
	{
		monsterType.Clear();	
	}
	public override void OnInit ()
	{

		startLevel = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
		endLevel = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
		percent = TableManager.GetValueFromUnionCell(data.ValueC[0], level);

		if (data.ValueD.Count <= 0 || data.ValueD.Count > 0 && TableManager.GetValueFromUnionCell(data.ValueD[0], level)==0)
		{
			monsterType.Add(ProtoTable.UnitTable.eType.BOSS);
			monsterType.Add(ProtoTable.UnitTable.eType.ELITE);
			monsterType.Add(ProtoTable.UnitTable.eType.MONSTER);
		}
		else {
			for(int i=0; i<data.ValueD.Count; ++i)
			{
				int type = TableManager.GetValueFromUnionCell(data.ValueD[i], level);
				if (type > 0)
					monsterType.Add((ProtoTable.UnitTable.eType)type);
			}
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

	void Deal(BeEntity entity)
	{
        handleA = entity.RegisterEventNew(BeEventType.onAfterFinalDamage, args => {
        //handleA = entity.RegisterEvent(BeEventType.onAfterFinalDamage, (object[] args)=>{

            BeActor actor = args.m_Obj as BeActor;
		    if (actor != null)
		    {
			    var attribute = actor.GetEntityData();
			    int level = attribute.level;
			    if (level < startLevel || level > endLevel || !monsterType.Contains((ProtoTable.UnitTable.eType)attribute.type))
				    return;

			    //int[] vals = (int[])args[0];
			    int damage = args.m_Int;

                args.m_Int = (damage * (VFactor.one + VFactor.NewVFactor(percent, (long)GlobalLogic.VALUE_1000)));

			    //Logger.LogErrorFormat("damage {0} -> {1}", damage, vals[0]);
		    }
		});
	}
}
