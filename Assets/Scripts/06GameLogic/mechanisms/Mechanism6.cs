using UnityEngine;
using System.Collections;

/*
 * 自身HP降低到某个值时，上一个buff，恢复再去掉buff
*/
public class Mechanism6 : BeMechanism {

	CrypticInt32 hpPercent = 50;
	int buffID = 1;
	public Mechanism6(int mid, int lv):base(mid, lv)
	{
	}

	public override void OnInit ()
	{
		hpPercent = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
		buffID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
	}
		
	public override void OnStart ()
	{
		if (owner != null)
		{
			handleA = owner.RegisterEventNew(BeEventType.onHPChange, (args)=>{
				VFactor f = new VFactor(hpPercent,GlobalLogic.VALUE_1000);
				//if (owner.GetEntityData().GetHPRate() < hpPercent/(float)(GlobalLogic.VALUE_1000))
				if(owner.GetEntityData().GetHPRate() < f)
				{
					if (owner.buffController.HasBuffByID(buffID) == null)
					{
						owner.buffController.TryAddBuff(buffID, -1, level);
					}
				}
				else {
					if (owner.buffController.HasBuffByID(buffID) != null)
					{
						owner.buffController.RemoveBuff(buffID);
					}
				}
			});
            handleB = owner.RegisterEventNew(BeEventType.onReborn, args =>
            {
                if (owner.buffController.HasBuffByID(buffID) != null)
                {
                    owner.buffController.RemoveBuff(buffID);
                }
            });
        }
	}
}
