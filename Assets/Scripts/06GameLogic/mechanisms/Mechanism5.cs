using UnityEngine;
using System.Collections;

/*
 * 自身进入异常后附加BUFF，退出异常后恢复
*/
public class Mechanism5 : BeMechanism {

	int abnormalType = 5;
	int buffID = 1;

    public Mechanism5(int mid, int lv):base(mid, lv)
	{
	}

	public override void OnInit ()
	{
		abnormalType = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
		buffID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
	}
		
	public override void OnStart ()
	{
        if (owner != null)
		{
            handleA = owner.RegisterEventNew(BeEventType.onAddBuff, (args)=>{
				BeBuff buff = args.m_Obj as BeBuff;
				if (buff != null && buff.buffType == (BuffType)abnormalType)
				{
					if (owner.buffController.HasBuffByID(buffID) == null)
					{
						owner.buffController.TryAddBuff(buffID, -1, level);
					}
				}
			});

            handleB = owner.RegisterEventNew(BeEventType.onRemoveBuff, (args)=>{
				BeBuff buff = args.m_Obj as BeBuff;
				if (buff != null && buff.buffType == (BuffType)abnormalType)
				{
					owner.delayCaller.DelayCall(100, ()=>{
						if (owner.buffController.GetBuffCountByType((BuffType)abnormalType) <= 0)
						{
							owner.buffController.RemoveBuff(buffID);
						}
					});
				}
			});
		}
	}
    
}
