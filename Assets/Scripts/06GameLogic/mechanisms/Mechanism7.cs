using UnityEngine;
using System.Collections;

/*
 * 例子:对生命值低于20%的目标，有3%几率造成额外30%的伤害（最终伤害的30%，对于35级以上的目标几率降低，反之增加）
 * 机制:对生命值低于A的目标，对自己上一个buff
*/
public class Mechanism7 : BeMechanism {

	int hpPercent = 50;
	int buffInfoID = 1;
	public Mechanism7(int mid, int lv):base(mid, lv)
	{
	}

	public override void OnInit ()
	{
		hpPercent = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
		buffInfoID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
	}
		
	public override void OnStart ()
	{
		if (owner != null)
		{
            handleA = owner.RegisterEventNew(BeEventType.onBeforeHit, args => 
            {
                //handleA = owner.RegisterEvent(BeEventType.onBeforeHit, (object [] args)=>{
				BeEntity target = args.m_Obj as BeEntity;
				if (target != null && 
					target.GetEntityData() != null && 
					target.GetEntityData().GetHPRate() <= VFactor.NewVFactor(hpPercent, (long)GlobalLogic.VALUE_1000))
				{
					if (buffInfoID > 0)
					{
                        BuffInfoData buffInfo = new BuffInfoData(buffInfoID, level);
                        owner.buffController.TryAddBuff(buffInfo);
					}
				}
			});

		}
	}
}
	