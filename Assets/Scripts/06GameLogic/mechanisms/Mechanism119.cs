using System;
using System.Collections.Generic;
using UnityEngine;

//增加或者减少怪物的模型的大小（千分比）
class Mechanism119 : BeMechanism
{
    int scaleRate;
    int monsterId = 0;
    List<BeActor> summonList = new List<BeActor>();
    bool restoreFlag = false;
    VInt originalScale;
    List<VInt> originalScaleList = new List<VInt>();

    public Mechanism119(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        summonList.Clear();
        originalScale = VInt.one;
        originalScaleList.Clear();
    }
    public override void OnInit()
    {
        scaleRate = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        monsterId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        restoreFlag = TableManager.GetValueFromUnionCell(data.ValueC[0], level) == 0;
    }

    public override void OnStart()
    {
        if (monsterId == 0)
        {
            originalScale = owner.GetScale();
            var scale = originalScale.i * VFactor.NewVFactor(GlobalLogic.VALUE_1000 + scaleRate, GlobalLogic.VALUE_1000);
            owner.SetScale(scale);
        }
        else
        {
            handleA = owner.RegisterEventNew(BeEventType.onChangeSummonScale, args =>
            {
                var actor = args.m_Obj as BeActor;
                if (actor != null && actor.GetEntityData().MonsterIDEqual(monsterId))
                {
                    args.m_Int += scaleRate;
                    summonList.Add(actor);
                    originalScaleList.Add(actor.GetScale());
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
                owner.SetScale(originalScale);
            }
            else
            {
                for (int i = 0; i < summonList.Count; i++)
                {
                    VInt originScale = summonList[i].GetScale() - scaleRate;
                    summonList[i].SetScale(originScale);
                }
                originalScaleList.Clear();
            }
        }
    }
}