using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//随机选择一个玩家添加BuffInfo
public class Mechanism1043 : BeMechanism
{
    public Mechanism1043(int id, int level) : base(id, level) { }

    private int buffInfoId = 0;     //添加的Buff信息ID

    public override void OnInit()
    {
        base.OnInit();
        buffInfoId = TableManager.GetValueFromUnionCell(data.ValueA[0],level);
    }


    public override void OnStart()
    {
        base.OnStart();
        SelectPlayerAddBuff();
    }

    /// <summary>
    /// 随机选择玩家添加Buff
    /// </summary>
    private void SelectPlayerAddBuff()
    {
        if (owner.CurrentBeBattle == null)
            return;
        List<BattlePlayer> list = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        if (list != null)
        {
            int index = FrameRandom.InRange(0, list.Count);
            if (list[index] != null && list[index].playerActor != null)
            {
                list[index].playerActor.buffController.TryAddBuff(buffInfoId);
            } 
        }
    }
}
