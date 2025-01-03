using UnityEngine;
using System.Collections;

/*
 * 机制描述:出血buff的时间减少50%
*/

public class Mechanism1 : BeMechanism {

	int buffType;
	CrypticInt32 percent;

	public Mechanism1(int mid, int lv):base(mid, lv)
	{
		//Logger.LogErrorFormat("Mechanism1!!!!!!!");
	}

	public override void OnInit ()
	{
		buffType = TableManager.GetValueFromUnionCell(data.ValueA[0], 1);
		percent = TableManager.GetValueFromUnionCell(data.ValueB[0], 1)/* / 1000f*/;
	}

	public override void OnStart ()
	{
		if (owner != null)
		{
			owner.RegisterEventNew(BeEventType.onAddBuff, (args)=>{
				BeBuff buff = args.m_Obj as BeBuff;
				if (buff != null && buff.buffType == (BuffType)buffType)
				{
					//buff.duration = IntMath.Float2Int(buff.duration * percent/(float)(GlobalLogic.VALUE_1000));
					if (percent > 0)
                    {
                         buff.duration = buff.duration * (VFactor.one - VFactor.NewVFactor(percent, GlobalLogic.VALUE_1000));
                    }

				}
			});
		}
	}
}
