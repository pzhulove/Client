using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 给召唤兽增加buff
*/
public class Mechanism4 : BeMechanism {

	int[] buffIdArray;
    int[] buffInfoIdArray;
	List<int> specifyMonsterIDs = new List<int>();

    private bool notAddToSelf = false;
    private bool isNeedRefresh = false; //是否上机制的时候要重新查找一次已经召唤的召唤兽
    public Mechanism4(int mid, int lv):base(mid, lv){}

    public override void OnReset()
    {
        buffIdArray = null;
        buffInfoIdArray = null;
        specifyMonsterIDs.Clear();
        notAddToSelf = false;
        isNeedRefresh = false;
    }
	public override void OnInit ()
	{
        buffIdArray = new int[data.ValueA.Length];
        for (int i = 0; i < data.ValueA.Length; i++)
            buffIdArray[i] = TableManager.GetValueFromUnionCell(data.ValueA[i], level);

		for(int i=0; i<data.ValueB.Count; ++i)
		{
			var mid = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
            
			mid = BeUtility.GetOnlyMonsterID(mid);
			if (mid > 0)
				specifyMonsterIDs.Add(mid);
		}

        buffInfoIdArray = new int[data.ValueC.Length];
        for (int i = 0; i < data.ValueC.Length; i++)
            buffInfoIdArray[i] = TableManager.GetValueFromUnionCell(data.ValueC[i], level);

        notAddToSelf = TableManager.GetValueFromUnionCell(data.ValueD[0], level) == 0 ? false : true;
        if (data.ValueE.Count > 0)
        {
            isNeedRefresh = TableManager.GetValueFromUnionCell(data.ValueE[0], level) == 0 ? false : true;
        }
    }

	public override void OnStart ()
	{
		if (owner != null)
		{
            if (isNeedRefresh && owner.CurrentBeScene != null)
            {
                var targets = GamePool.ListPool<BeActor>.Get();
                for (int i = 0; i < specifyMonsterIDs.Count; i++)
                {
                    owner.CurrentBeScene.FindMonsterByID(targets, specifyMonsterIDs[i], false);
                    for (int j = 0; j < targets.Count; j++)
                    {
                        var summoner = targets[j].GetTopOwner(owner);
                        if (summoner.GetPID() != owner.GetPID()) continue;
                        for (int k = 0; k < buffIdArray.Length; k++)
                            targets[j].buffController.TryAddBuff(buffIdArray[k], -1, level);

                        for (int t = 0; t < buffInfoIdArray.Length; t++)
                        {
                            if (!notAddToSelf)
                            {
                                targets[j].buffController.TryAddBuff(buffInfoIdArray[t]);
                            }
                            targets[j].buffController.AddTriggerBuff(buffInfoIdArray[t]);
                        }
                    }
                }
                GamePool.ListPool<BeActor>.Release(targets);
            }
            handleA = owner.RegisterEventNew(BeEventType.onSummon, (args)=>{
				BeActor summonMonster = args.m_Obj as BeActor;
				if (summonMonster != null && (specifyMonsterIDs.Count<=0 || specifyMonsterIDs.Contains(summonMonster.GetEntityData().simpleMonsterID)))
				{
                    for (int i = 0; i < buffIdArray.Length; i++)
                        summonMonster.buffController.TryAddBuff(buffIdArray[i], -1, level);

                    for (int i = 0; i < buffInfoIdArray.Length; i++)
                    {
                        if (!notAddToSelf)
                        {
                            summonMonster.buffController.TryAddBuff(buffInfoIdArray[i]);
                        }
                        summonMonster.buffController.AddTriggerBuff(buffInfoIdArray[i]);
                    }
				}
			});
		}
	}
}
