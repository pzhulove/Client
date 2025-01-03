using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 领袖机制
*/
public class Mechanism11 : BeMechanism {
    bool setNeedDead = false;
    int monsterID = 1010011;
	int monsterSkill = 5000;
	List<BeActor> savedMonsters = null;
	Dictionary<BeActor, IBeEventHandle> savedHandles = new Dictionary<BeActor, IBeEventHandle>();

	public Mechanism11(int mid, int lv):base(mid, lv)
	{
	}

	public override void OnReset()
	{
		if (savedMonsters != null) savedMonsters.Clear();
		savedHandles.Clear();
	}

	public override void OnInit ()
	{
		monsterID = TableManager.GetValueFromUnionCell(data.ValueA[0], 1);
		monsterSkill = TableManager.GetValueFromUnionCell(data.ValueB[0], 1);
        setNeedDead = TableManager.GetValueFromUnionCell(data.ValueC[0], 1) == 1;
    }

	public override void OnStart ()
	{
	}

	public override void OnFinish ()
	{
		Dictionary<BeActor, IBeEventHandle>.Enumerator enumerator = savedHandles.GetEnumerator();
		while(enumerator.MoveNext())
		{
			var handle = enumerator.Current.Value;
			handle.Remove();
		}
        savedHandles.Clear();
    }

	public override void OnUpdate (int deltaTime)
	{
		if (owner != null)
		{
			if (savedMonsters == null)
			{
				savedMonsters = new List<BeActor>();
				owner.CurrentBeScene.FindMonsterByID(savedMonsters, monsterID);

				for(int i = 0; i < savedMonsters.Count; ++i)
				{
					var monster = savedMonsters[i];
                    var handle = savedMonsters[i].RegisterEventNew(BeEventType.onDead, eventParam =>
                    {
                        if (owner != null && !owner.IsDead())
                        {
							eventParam.m_Bool = false;
                            if (setNeedDead)
                                monster.SetNeedDead(false);
                            monster.SetIsDead(false);
                            monster.GetEntityData().SetHP(1);
                            if (!setNeedDead)
                                monster.m_pkGeActor.ResetHPBar();
                            monster.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE, GlobalLogic.VALUE_3000);
                            monster.UseSkill(monsterSkill, true);
                        }
                    });

                    savedHandles.Add(monster, handle);
				}
			}
		}
	}


}
