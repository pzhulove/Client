using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.UIElements;

//石化
public class Buff523301 : BeBuff {

	VInt3 [] points = new VInt3[5];
	bool [] flags = new bool[3];
	VFactor[] percents = new VFactor[3];
	int summonID = 1920011;

	private IBeEventHandle handler;
	public Buff523301(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack)
	{
		points[0] = new VInt3(-4.2f, 5.2f, 0f);
		points[1] = new VInt3(-2.8f, 5.2f, 0f);
		points[2] = new VInt3(-0f, 5.2f, 0f);
		points[3] = new VInt3(-2.8f, 5.2f, 0f);
		points[4] = new VInt3(4.2f, 5.2f, 0f);

		percents[0] = new VFactor(70,100);
		percents[1] = new VFactor(60,100);
		percents[2] = new VFactor(50,100);
	}

	public override void OnReset()
	{
		for(int i=0; i<flags.Length; ++i)
		{
			flags[i] = false;
		}
		summonID = 0;
	}

	public override void OnInit ()
	{
		summonID = TableManager.GetValueFromUnionCell(buffData.ValueA[0], level);
	}

	public override void OnStart ()
	{
		RemoveHandler();
		handler = owner.RegisterEventNew(BeEventType.onHPChange, (args)=>{
			if (!owner.IsDead())
			{
				VFactor hpRate = owner.GetEntityData().GetHPRate();
				for(int i=0; i<3; ++i)
				{
					if (hpRate < percents[i] && !flags[i])
					{
						flags[i] = true;
						DoSummon(i);
					}
				}
			}
		});
	}

	void DoSummon(int index)
	{
		List<int> indexes = new List<int>();
		if (index == 0)
			indexes.Add(2);
		else if (index == 1)
		{
			indexes.Add(1);
			indexes.Add(3);
		}
		else if (index == 2)
		{
			indexes.Add(0);
			indexes.Add(2);
			indexes.Add(4);
		}
		for(int i=0; i<indexes.Count; ++i)
		{
			int mLevel = owner.GetEntityData().level;
			int monsterID = summonID + mLevel * GlobalLogic.VALUE_100;
			BeUtility.AdjustMonsterDifficulty(ref owner.GetEntityData().monsterID, ref monsterID);
			owner.CurrentBeScene.SummonMonster(monsterID, points[indexes[i]], owner.GetCamp(), owner);
		}
	}
	
	public override void OnFinish ()
	{
		RemoveHandler();
	}

	void RemoveHandler()
	{
		if (handler != null)
		{
			handler.Remove();
			handler = null;
		}
	}
}
