using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 吞噬
*/
public class Mechanism9 : BeMechanism
{

	Dictionary<int, int> swallowList = new Dictionary<int, int>();
	bool ownerDead;
	int swallowMaxNum = 1;
    int buffDelay = 0;
    bool immediatelyRemove = false;

	public Mechanism9(int mid, int lv):base(mid, lv)
	{
	}

	public override void OnReset()
	{
		swallowList.Clear();
		ownerDead = false;
		swallowMaxNum = 1;
		buffDelay = 0;
		immediatelyRemove = false;
	}

	public override void OnInit ()
	{
        int hard = owner.CurrentBeBattle.dungeonManager.GetDungeonDataManager().id.diffID;
        for (int i=0; i<data.ValueA.Count; ++i)
		{
			int monsterID = data.ValueA[i].fixInitValue;
			int buffID = data.ValueA[i].fixLevelGrow;

            //不带关卡难度的怪物ID也添加进来
			if (!swallowList.ContainsKey(monsterID))
				swallowList.Add(monsterID, buffID);
            
            monsterID += hard;

            if (!swallowList.ContainsKey(monsterID))
                swallowList.Add(monsterID, buffID);
        }

		int tmp = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
		if (tmp >= 1)
			ownerDead = true;

		tmp = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
		if (tmp > 1)
			swallowMaxNum = tmp;

        if (data.ValueD.Count > 0)
            buffDelay = TableManager.GetValueFromUnionCell(data.ValueD[0], level);

        immediatelyRemove = TableManager.GetValueFromUnionCell(data.ValueE[0], level) == 0 ? false : true;
    }

	public override void OnStart ()
	{
		if (owner != null)
		{
			var hurtEntity = owner.GetHurtEntityList();

			int count = 0;
			for(int i=0; i<hurtEntity.Count; ++i)
			{
				var entity = hurtEntity[i];
				BeActor monster = entity as BeActor;
				if (monster != null && !monster.IsDead() && CanSwallow(monster.GetEntityData().monsterID) && swallowList.ContainsKey(monster.GetEntityData().monsterID))
				{
					count++;

                    owner.delayCaller.DelayCall(buffDelay, () =>
                    {
						// marked by ckm
						if(swallowList.ContainsKey(monster.GetEntityData().monsterID))
						{
							int buffID = swallowList[monster.GetEntityData().monsterID];
                        	owner.buffController.TryAddBuff(buffID, 1);
						}
                    });

                    if (!immediatelyRemove)
                    {
                        monster.delayCaller.DelayCall(200, () => {
                            monster.DoDead();
                        });
                    }
                    else
                    {
                        monster.DoDead();
                    }
                    

					if (count >= swallowMaxNum)
						break;
				}
			}

			if (count > 0 && ownerDead)
			{
				owner.delayCaller.DelayCall(200, ()=>{
					owner.DoDead();
				});

			}
		}
	}

	public bool CanSwallow(int mid)
	{

		var enumerator = swallowList.GetEnumerator();
		while(enumerator.MoveNext())
		{
			var key = enumerator.Current.Key;
			if (BeUtility.IsMonsterIDEqual(key, mid))
				return true;
		}

		return false;
	}
}
