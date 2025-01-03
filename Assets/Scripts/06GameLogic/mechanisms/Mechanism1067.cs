using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 19年9月装备机制 根据地下城类型增加最终伤害
/// ValueA 地下城类型
/// ValueB 增伤百分比
/// </summary>
public class Mechanism1067 : BeMechanism
{
    private int mDungeonType = -1;
    private VFactor percent = VFactor.zero;

    public Mechanism1067(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        mDungeonType = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        percent = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(data.ValueB[0], level), GlobalLogic.VALUE_1000); 
    }

    public override void OnStart()
    {
        if (owner != null && owner.CurrentBeBattle != null && owner.CurrentBeBattle.dungeonManager != null
            && owner.CurrentBeBattle.dungeonManager.GetDungeonDataManager() != null &&
            owner.CurrentBeBattle.dungeonManager.GetDungeonDataManager().table != null) 
        {
            if ((int)owner.CurrentBeBattle.dungeonManager.GetDungeonDataManager().table.SubType == mDungeonType)
            {
                handleA = owner.RegisterEventNew(BeEventType.onAfterFinalDamageNew, args =>
                //handleA = owner.RegisterEvent(BeEventType.onAfterFinalDamageNew, (object[] args) =>
                {
                    //int[] damage = (int[])args[0];

                    args.m_Int += args.m_Int * percent;
                });
            }
        }
    }
}
