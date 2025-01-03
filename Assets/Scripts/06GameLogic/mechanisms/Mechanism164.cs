using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//增加或者减少怪物的质量的大小（千分比）
public class Mechanism164 : BeMechanism
{
    int weightRate;
    int monsterId = 0;
    List<BeActor> summonList = new List<BeActor>();
    bool restoreFlag = false;
    VInt originalWeight;
    List<VInt> originalWeightList = new List<VInt>();
    public Mechanism164(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        summonList.Clear();
        originalWeight = VInt.one;
        originalWeightList.Clear();
    }
    public override void OnInit()
    {
        weightRate = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        monsterId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        restoreFlag = TableManager.GetValueFromUnionCell(data.ValueC[0], level) == 0;
    }

    public override void OnStart()
    {
        if (monsterId == 0)
        {
            originalWeight = owner.GetEntityData().weight;
            var weight = originalWeight.i * VFactor.NewVFactor(GlobalLogic.VALUE_1000 + weightRate, GlobalLogic.VALUE_1000);
            owner.GetEntityData().weight = weight;
        }
        else
        {
            handleA = owner.RegisterEventNew(BeEventType.onChangeSummonWeight, args =>
            {
                var actor = args.m_Obj as BeActor;
                if (actor != null && actor.GetEntityData().MonsterIDEqual(monsterId))
                {
                    args.m_Int += weightRate;
                    summonList.Add(actor);
                    originalWeightList.Add(actor.GetEntityData().weight);
                }
            });
        }
    }

    public override void OnFinish()
    {
        if (restoreFlag)
        {
            if (monsterId == 0)
            {
                owner.GetEntityData().weight = originalWeight;
            }
            else
            {
                for (int i = 0; i < summonList.Count; i++)
                {
                    if (null == summonList[i])
                        continue;
                    VInt originScale = summonList[i].GetEntityData().weight - weightRate;
                    summonList[i].GetEntityData().weight = originScale;
                }
                originalWeightList.Clear();
            }
        }
    }
}
