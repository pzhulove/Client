using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 机制描述:许愿，在几个buff里选择一个
*/

public class Mechanism24 : BeMechanism {
    
    public enum BuffInfoTargetType
    {
        Owner,          //主人
        Self,           //自己
    }

	protected List<int> buffInfoIDs = new List<int>();
    protected BuffInfoTargetType m_BuffInfoTarget = BuffInfoTargetType.Owner;           //Buff附加对象
    protected int m_AddBuffTimeAcc = -1;                                                //添加Buff时间

    protected int m_CurrentTimeAcc = 0;                                                 //当前的时间间隔

	public Mechanism24(int mid, int lv):base(mid, lv)
	{
	}
    public override void OnReset()
    {
        buffInfoIDs.Clear();
        m_BuffInfoTarget = BuffInfoTargetType.Owner;
        m_AddBuffTimeAcc = -1;
        m_CurrentTimeAcc = 0;
    }

	public override void OnInit ()
	{
		for(int i=0; i<data.ValueA.Count; ++i)
		{
			var bid = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
			if (bid > 0)
				buffInfoIDs.Add(bid);
		}

        if (data.ValueB.Count>0)
        {
            m_BuffInfoTarget = (BuffInfoTargetType)TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        }
        
        if (data.ValueC.Count > 0)
        {
            m_AddBuffTimeAcc = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        }
	}

    public override void OnStart()
    {
        if (m_AddBuffTimeAcc == -1)
        {
            AddBuffInfo();
        }
    }

    public override void OnUpdate(int deltaTime)
    {
        if (m_AddBuffTimeAcc != -1)
        {
            if (m_CurrentTimeAcc >= m_AddBuffTimeAcc)
            {
                m_CurrentTimeAcc = 0;
                AddBuffInfo();
            }
            else
            {
                m_CurrentTimeAcc += deltaTime;
            }
        }
    }

    //选取对象 随机上一个BuffInfo
    protected void AddBuffInfo()
    {
        BeActor actor = null;
        if (m_BuffInfoTarget == BuffInfoTargetType.Owner)
        {
            actor = owner.GetOwner() as BeActor;
        }
        else if (m_BuffInfoTarget == BuffInfoTargetType.Self)
        {
            actor = owner;
        }

        if (owner != null && !actor.IsDead())
        {
            if (actor != null)
            {
                int buffId = buffInfoIDs[FrameRandom.InRange(0, buffInfoIDs.Count)];
                BuffInfoData buffInfo = new BuffInfoData(buffId, level);
                BeBuff buff = actor.buffController.TryAddBuff(buffInfo);
                if (buff != null)
                {
                    var thisAttachBuff = GetAttachBuff();
                    if (thisAttachBuff != null)
                        buff.releaser = thisAttachBuff.releaser;
                    //对阵鬼瘟疫之罗刹进行特殊处理
                    if (data.ID == 1056 || data.ID == 1059)
                        buff.skillId = 1809;
                }
                

            }
        }
    }
}
