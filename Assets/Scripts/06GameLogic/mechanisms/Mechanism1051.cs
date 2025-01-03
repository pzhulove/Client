using System;
using System.Collections.Generic;
//当指定范围内，指定异常状态的敌人，每有一个指定异常状态的敌人则给所有队员或者自己添加一个BUFF
public class Mechanism1051 : BeMechanism
{
    VInt range = VInt.zero;
    int stateId = -2;
    bool isSelf = false;
    bool isStart = false;
    int durTime = 0;
    int maxBuffCount = 0;
    int buffId = 0;
    BeBuffStatIDFilter stateFileter = null;
    public Mechanism1051(int mid, int lv) : base(mid, lv)
    {

    }
    public override void OnInit()
    {
        isStart = false;
        durTime = 0;
        range = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[0], level), GlobalLogic.VALUE_1000);
        stateId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        buffId = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        isSelf = TableManager.GetValueFromUnionCell(data.ValueD[0], level) == 0 ? false : true;
        maxBuffCount = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        stateFileter = new BeBuffStatIDFilter();
        stateFileter.statId = 1 << stateId;
    }
    public override void OnStart()
    {
        isStart = true;
    }
    public override void OnUpdate(int delta)
    {
        if (!isStart) return;
        if (owner == null || stateFileter == null || owner.IsDead()) return;
        durTime += delta;
        if (durTime < 200)
        {
            return;
        }
        durTime -= 200;
        DoCheckBuff();
    }
    private void DoBuff(BeActor actor, int targetCount)
    {
        int buffCount = actor.buffController.GetBuffCountByID(buffId);
        if (targetCount >= buffCount)
        {
            int deltaCount = targetCount - buffCount;
            for (int i = 0; i < deltaCount; i++)
            {
                actor.buffController.TryAddBuff(buffId, -1);
            }

        }
        else
        {
            int deltaCount = buffCount - targetCount;
            actor.buffController.RemoveBuff(buffId, deltaCount);
        }
    }
    private void DoCheckBuff()
    {
        if (owner.CurrentBeScene == null) return;
        var targets = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindTargets(targets, owner, range, false, stateFileter);
        int targetCount = targets.Count;
        GamePool.ListPool<BeActor>.Release(targets);
        if (targetCount > maxBuffCount)
        {
            targetCount = maxBuffCount;
        }

        if (isSelf)
        {
            DoBuff(owner, targetCount);
        }
        else
        {
            if (owner.CurrentBeBattle == null || owner.CurrentBeBattle.dungeonPlayerManager == null) return;
            var players = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
            if (players == null) return;
            var iter = players.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current == null) continue;
                var curActor = iter.Current.playerActor;
                if (curActor == null || curActor.buffController == null) continue;
                DoBuff(curActor, targetCount);
            }
        }
    }
}

