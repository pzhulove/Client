using System;
using System.Collections.Generic;
//当组队时，每有一个队友，则给自己增加一个BUFF
public class Mechanism1052:BeMechanism
{
    int buffId = 0;
    public Mechanism1052(int mid, int lv) : base(mid, lv)
    {

    }
    public override void OnInit()
    {
        buffId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
    }
    public override void OnStart()
    {
        if (owner == null || owner.CurrentBeBattle == null  || owner.CurrentBeBattle.dungeonPlayerManager == null) return;
        var players = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        if (players == null) return;
        if (players.Count < 2) return;
        for(int i = 0; i < players.Count;i++)
        {
            owner.buffController.TryAddBuff(buffId, -1);
        }
    }
}

