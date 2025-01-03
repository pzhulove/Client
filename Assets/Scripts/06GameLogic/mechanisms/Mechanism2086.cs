using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 改变装备buff信息触发概率机制，目前用在团本且写死
/// </summary>
public class Mechanism2086 : BeMechanism
{
    private int buffInfoId = 0;//buff信息id
    protected int changeRate = 0;//改变buff信息触发概率 千分比

    public Mechanism2086(int sid, int skillLevel) : base(sid, skillLevel)
    {
        
    }

    public override void OnInit()
    {
        buffInfoId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        changeRate = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnStart()
    {
        AddTriggerBuff();
    }

    public override void OnFinish()
    {
        RemoveTriggerBuff();
    }

    /// <summary>
    /// 考虑到可能会扩展别的地下城类型 此函数用作判断地下城类型
    /// </summary>
    /// <returns></returns>
    private bool IsRightDungeonType()
    {
        if(owner != null)
        {
            var mBattle = owner.CurrentBeBattle;
            if(mBattle != null && mBattle.dungeonManager != null &&
                mBattle.dungeonManager.GetDungeonDataManager()!= null)
            {
                return mBattle.dungeonManager.GetDungeonDataManager().IsHardRaid;
            }
        }
        return false;
    }
    
    //添加buff信息
    private void AddTriggerBuff()
    {
        if(owner != null)
        {
            BuffInfoData infoData = new BuffInfoData(buffInfoId);
            if (IsRightDungeonType())
            {
                infoData.prob += changeRate;
            }
            owner.buffController.AddTriggerBuff(infoData);
        }
    }

    //移除buff信息
    private void RemoveTriggerBuff()
    {
        if(owner != null)
        {
            owner.buffController.RemoveTriggerBuff(buffInfoId);
        }
    }
}