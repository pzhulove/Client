using System;
using System.Collections.Generic;
using GameClient;

public class Mechanism2083 : BeMechanism
{
    public Mechanism2083(int mid, int lv) : base(mid, lv){ }
    public override void OnInit()
    {
        base.OnInit();
    }
    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onDead, onDead);
    }
    private void onDead(BeEvent.BeEventParam eventParam)
    {
        if (data.ValueA.Count <= 0) return;
        if (owner == null || owner.CurrentBeBattle == null) return;
        var curBattle = owner.CurrentBeBattle as GameClient.TreasureMapBattle;
        if (curBattle == null) return;
        if (owner.IsDead()) return;
        int index = this.FrameRandom.InRange(0, data.ValueA.Count);
        int regionId = TableManager.GetValueFromUnionCell(data.ValueA[index], level);
        curBattle.AddRegionInfo(regionId, owner.GetPosition());
    }
}
