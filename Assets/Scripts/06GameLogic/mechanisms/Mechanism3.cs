using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 提升召唤兽的等级{ValueA}
*/
public class Mechanism3 : BeMechanism {

	CrypticInt32 summonLevelAdd = 0;
	List<int> specifyMonsterIDs = new List<int>();

	public Mechanism3(int mid, int lv):base(mid, lv){}
	public override void OnReset()
	{
		summonLevelAdd = 0;
		specifyMonsterIDs.Clear();
	}

	public override void OnInit ()
	{
		summonLevelAdd = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
		for(int i=0; i<data.ValueB.Count; ++i)
		{
			var mid = TableManager.GetValueFromUnionCell(data.ValueB[i], level);


			mid = BeUtility.GetOnlyMonsterID(mid);
			if (mid > 0)
				specifyMonsterIDs.Add(mid);
		}
	}


	public override void OnStart ()
	{
		if (owner != null)
		{
			handleA = owner.RegisterEventNew(BeEventType.onBeforeSummon, (args) => {
				
				int summonID = args.m_Int;

				if (specifyMonsterIDs.Count <= 0 || specifyMonsterIDs.Contains(BeUtility.GetOnlyMonsterID(summonID)))
				{
					args.m_Int2 += summonLevelAdd;
				}
					
			});
		}
	}
}
