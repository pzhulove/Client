using System;
using System.Collections.Generic;
using GameClient;
//宝藏关卡宝箱掉落机制
public class Mechanism2074 : BeMechanism
{
    public Mechanism2074(int mid, int lv) : base(mid, lv){ }
    public override void OnInit()
    {
        base.OnInit();
    }
    public override void OnStart()
    {
        base.OnStart();
        if (owner == null || owner.CurrentBeBattle == null) return;
        var curBattle = owner.CurrentBeBattle as GameClient.TreasureMapBattle;
        if (curBattle == null) return;
        handleA = owner.RegisterEventNew(BeEventType.onDead, onDead);
        for(int i = 0; i < data.ValueA.Count;i++)
        {
            curBattle.AddRegionIdLibary(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }

        for (int i = 0; i < data.ValueB.Count; i++)
        {
            curBattle.AddReduceIdLibary(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
        }

    }
    private void onDead(BeEvent.BeEventParam eventParam)
    {
        if (owner == null || owner.CurrentBeBattle == null) return;
        var curBattle = owner.CurrentBeBattle as GameClient.TreasureMapBattle;
        if (curBattle == null) return;
        curBattle.GenerateRegion(owner.GetPosition());
    }

}

