using System;
using System.Collections.Generic;
//当击杀一个敌方单位时，给自己添加一个BUFFinfo
public class Mechanism1050 : BeMechanism
{
    int buffInfoId = 0;
    //这里的buff信息用于概率和时间控制buff的
    public Mechanism1050(int mid, int lv) : base(mid, lv)
    {

    }
    public override void OnInit()
    {
        buffInfoId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
    }
    public override void OnStart()
    {
        if (owner == null) return;
        handleA = owner.RegisterEventNew(BeEventType.onKill, OnKillMonster);
        //handleA = owner.RegisterEvent(BeEventType.onKill, OnKillMonster);
    }
    private void OnKillMonster(GameClient.BeEvent.BeEventParam param)
    {
        if (owner == null || owner.buffController == null) return;
        owner.buffController.TryAddBuff(buffInfoId);
    }
}
