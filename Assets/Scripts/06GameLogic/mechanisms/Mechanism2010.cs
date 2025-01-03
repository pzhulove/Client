using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanism2010 : BeMechanism
{
    private int time;
    public Mechanism2010(int id, int lv) : base(id, lv)
    { }

    public override void OnInit()
    {
        base.OnInit();
        time = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
    }


    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onClear, (args) =>
        {
            int time = owner.CurrentBeScene.mBattle.dungeonManager.GetDungeonDataManager().AllFightTime(true);
            if (time <= this.time * 1000 && owner.CurrentBeBattle.HasFlag(GameClient.BattleFlagType.Eff_Devilddom_hidden_room))
            {
                owner.CurrentBeScene.OpenEggDoor();
                GameClient.PVEBattle battle = owner.CurrentBeBattle as GameClient.PVEBattle;
                if (battle != null)
                {
                    battle.SetEggRoom(true);
                }
            }
        });
    }

}
