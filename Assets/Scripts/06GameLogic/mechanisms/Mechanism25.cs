using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 机制描述:宠物复活主人
*/

public class Mechanism25 : BeMechanism {

	int skillID = 9506;
	List<int> addBuffIDList1 = new List<int>();
	List<int> addBuffIDList2 = new List<int>();

	public Mechanism25(int mid, int lv):base(mid, lv)
	{
	}

	public override void OnReset()
	{
		skillID = 0;
		addBuffIDList1.Clear();
		addBuffIDList2.Clear();
	}

	public override void OnInit ()
	{
		skillID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);

        for(int i = 0; i < data.ValueB.Count; i++)
        {
            addBuffIDList1.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
        }

        for (int i = 0; i < data.ValueC.Count; i++)
        {
            addBuffIDList2.Add(TableManager.GetValueFromUnionCell(data.ValueC[i], level));
        }
	}

	public override void OnStart ()
	{
		var master = owner.GetOwner() as BeActor;

		if (master == null || master.IsDeadOrRemoved())
			return;

		handleA = master.RegisterEventNew(BeEventType.onHPChange, (args)=>{

			if (master.GetEntityData().GetHP() <= 0)
			{
				bool succeed = owner.UseSkill(skillID);

				if (succeed)
				{
					//强制设成1
					master.GetEntityData().SetHP(1);
                    master.SetIsDead(false);
                    for(int i=0;i< addBuffIDList1.Count; i++)
                    {
                        master.buffController.TryAddBuff(addBuffIDList1[i], GlobalLogic.VALUE_2000, level);
                    }
					master.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE, GlobalLogic.VALUE_2000);
					master.delayCaller.DelayCall(GlobalLogic.VALUE_100, ()=>{
						master.m_pkGeActor.SyncHPBar();
					});

						
					master.delayCaller.DelayCall(GlobalLogic.VALUE_1000, ()=>{
                        for (int i = 0; i < addBuffIDList2.Count; i++)
                        {
                            master.buffController.TryAddBuff(addBuffIDList2[i], GlobalLogic.VALUE_2000, level);
                        }
					});
#if !LOGIC_SERVER
                    int time = 500;
                    if(master!=null && !master.IsDead())
                    {
                        master.m_pkGeActor.ChangeAction("Anim_Daodi",1.0f);
                        master.delayCaller.DelayCall(time, () =>
                        {
							if (master != null && !master.IsDead() && master.sgGetCurrentState() == (int)ActionState.AS_IDLE) //存在其他状态就不要播idle动画了
								master.m_pkGeActor.ChangeAction("Anim_Idle02", 1.0f);
                        });
                    }
#endif
                }
			}

		});
	}
}
