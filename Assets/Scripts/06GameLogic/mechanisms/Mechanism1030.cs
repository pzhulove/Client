using GameClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 消耗指定道具ID添加BUFFINFO
/// </summary>
public class Mechanism1030 : BeMechanism
{
    private int itemID;
    private int time;
    private int buffInfoID;
    private int itemCnt;
    public Mechanism1030(int id, int level) : base(id, level) { }

    public override void OnInit()
    {
        base.OnInit();
        time = TableManager.GetValueFromUnionCell(data.ValueA[0],level);
        itemID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        buffInfoID = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        itemCnt = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        owner.delayCaller.DelayCall(100, () => 
        {
            timer = 0;
            DoCostItem();
        });     
    }

    private int timer = 0;
    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (time < 1000) return;
        timer += deltaTime;
        if (timer >= time)
        {
            DoCostItem();
            timer = 0;
        }
    }

    private void DoCostItem()
    {

        if (owner.GetEntityData().GetCrystalNum() >= itemCnt)
        {
            owner.buffController.TryAddBuff(buffInfoID);
            owner.GetEntityData().ModifyCrystalessNum(-itemCnt);
            owner.TriggerEventNew(BeEventType.OnUseCrystal);
            //如果是本地玩家才向服务器消耗无色晶体
            if (owner.isLocalActor)
            {
                //TODO
                if (!BattleMain.IsModeTrain(BattleMain.battleType) && !ReplayServer.GetInstance().IsReplay())
                    BeUtility.UseItemInBattle(itemID, 0, itemCnt);
            }
        }
        else
        {
#if !SERVER_LOGIC
            if (owner.isLocalActor)
            {               
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("strengthen_uncolor_not_enough"));
            }
#endif
        }
    }
}
