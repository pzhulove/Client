using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//骨盾
public class Mechanism10024 : BeMechanism
{
    int breakHpRate;
    int breakHpNum;
    int hitEffectCount;
    int hitCount;
    List<int> buffInfoIdList = new List<int>();
    int curHurtHp;
    //普通被击
    string hitEffect;
    //累计被击
    string hitCountEffect;
    //持续特效
    int lifeEffectId;
    //破坏特效
    int removeEffectId;
    
    public Mechanism10024(int mid, int lv) : base(mid, lv) { }
    
    public override void OnReset()
    {
        breakHpNum = 0;
        curHurtHp = 0;
        hitCount = 0;
    }
    public override void OnInit()
    {
        if (data.StringValueA.Count > 0)
            hitEffect = data.StringValueA[0];
        if (data.StringValueA.Count > 1)
            hitCountEffect = data.StringValueA[1];
        breakHpRate = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
		for(int i=0; i<data.ValueB.Count; ++i)
		{
			var bid = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
			if (bid > 0)
				buffInfoIdList.Add(bid);
		}
        hitEffectCount = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        if (data.ValueD.Count > 0)
            lifeEffectId = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        if (data.ValueD.Count > 1)
            removeEffectId = TableManager.GetValueFromUnionCell(data.ValueD[1], level);
    }

    public override void OnStart()
    {
        if (null != owner)
        {
#if !LOGIC_SERVER
            owner.m_pkGeActor.CreateEffect(lifeEffectId, Vec3.zero);
#endif
            foreach (var id in buffInfoIdList)
            {
                owner.buffController.TryAddBuffInfo(id, owner, level);
            }
            breakHpNum = owner.GetEntityData().GetMaxHP() * breakHpRate / 100;
            handleA = owner.RegisterEventNew(BeEventType.onHit, (param)=>{
                curHurtHp += param.m_Int;
                if (curHurtHp >= breakHpNum)
                {
                    Finish();
                }
                ++hitCount;
            });
            handleB = owner.RegisterEventNew(BeEventType.onChangeHitEffect, (param) =>
            {
                if (hitCount % hitEffectCount != 0)
                {
                    //播放普通特效
                    param.m_String = hitEffect;
                }
                else
                {
                    //播放特殊特效
                    param.m_String = hitCountEffect;
                }
            });
        }
    }

    public override void OnFinish()
    {
        if (null != owner)
        {
#if !LOGIC_SERVER
            owner.m_pkGeActor.CreateEffect(removeEffectId, Vec3.zero);
#endif
            foreach (var id in buffInfoIdList)
            {
                owner.buffController.RemoveBuffByBuffInfoID(id);
            }
        }
    }
}
