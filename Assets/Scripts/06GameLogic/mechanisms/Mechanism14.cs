using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
 * 攻击指定异常状态的敌人，给自己或者攻击对象附加buff
*/

public class Mechanism14 : BeMechanism {

	List<int> abnormalTypes = new List<int>();
	List<int> buffInfos = new List<int>();

    protected bool buffTargetIsSelf = true;

	public Mechanism14(int mid, int lv):base(mid, lv){}

	public override void OnReset()
	{
		abnormalTypes.Clear();
		buffInfos.Clear();
		buffTargetIsSelf = true;
	}

	public override void OnInit ()
	{
		for(int i=0; i<data.ValueA.Count; ++i)
		{
			int v = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
			if (v > 0 && !abnormalTypes.Contains(v))
				abnormalTypes.Add(v);
		}

		for(int i=0; i<data.ValueB.Count; ++i)
		{
			int buffInfoID = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
			if (buffInfoID > 0 && !buffInfos.Contains(buffInfoID))
				buffInfos.Add(buffInfoID);
		}

        if (data.ValueC.Count > 0)
        {
            buffTargetIsSelf = false;
        }
	}

	public override void OnStart ()
	{
		if (owner != null)
		{
            handleA = owner.RegisterEventNew(BeEventType.onBeforeHit, args => 
            {
                //handleA = owner.RegisterEvent(BeEventType.onBeforeHit, (object[] args) => {
				var target = args.m_Obj as BeActor;
				if (target != null && HasAbnormalType(target, abnormalTypes))
				{
                    if (buffTargetIsSelf)
                    {
                        AddBuff(owner, buffInfos);
                    }
                    else
                    {
                        AddBuff(target, buffInfos);
                    }
				}
			});
		}
	}

	bool HasAbnormalType(BeActor actor, List<int> abnormalTypes)
	{
		for(int i=0; i<abnormalTypes.Count; ++i)
		{
			BeBuffStateType bs = (BeBuffStateType)(1 << abnormalTypes[i]);
			if (actor.stateController.HasBuffState(bs))
				return true;
		}

		return false;
	}

	void AddBuff(BeActor actor, List<int> buffInfos)
	{
        for (int i = 0; i < buffInfos.Count; ++i)
        {
            BuffInfoData buffInfo = new BuffInfoData(buffInfos[i], level);
            actor.buffController.TryAddBuff(buffInfo, owner);
        }
    }

}
