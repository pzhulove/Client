using System;
using System.Collections.Generic;
using UnityEngine;

//当前技能攻击到敌人后，移除某些buff
class Mechanism100 : BeMechanism
{
    int[] buffArray;
    int[] buffInfoArray;

    bool hitOtherFlag;
    bool finishRemoveBuff;
    
    public Mechanism100(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        buffArray = new int[data.ValueA.Length];
        for (int i = 0; i < data.ValueA.Length; i++)
        {
            buffArray[i] = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
        }
        buffInfoArray = new int[data.ValueB.Length];
        for (int i = 0; i < data.ValueB.Length; i++)
        {
            buffInfoArray[i] = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
        }
        finishRemoveBuff = TableManager.GetValueFromUnionCell(data.ValueC[0], level) == 0 ? false : true;
    }

    public override void OnStart()
    {
        hitOtherFlag = false;

        handleA = owner.RegisterEventNew(BeEventType.onHitOther, args =>
        {
            hitOtherFlag = true;
        });

        handleB = owner.RegisterEventNew(BeEventType.onCastSkillFinish, args =>
        {
            if (hitOtherFlag)
            {
                RemoveBuff();
            }
        });
    }

    public override void OnFinish()
    {
        base.OnFinish();
        if (finishRemoveBuff)
        {
            RemoveBuff();
        }
    }

    private void RemoveBuff()
    {
        for (int i = 0; i < buffArray.Length; i++)
        {
            owner.buffController.RemoveBuff(buffArray[i]);
        }
        for (int i = 0; i < buffInfoArray.Length; i++)
        {
            owner.buffController.RemoveBuffByBuffInfoID(buffInfoArray[i]);
        }
    }

}

