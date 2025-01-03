using System;
using System.Collections.Generic;
using GameClient;
//当场上存在指定怪物时（多填），每存在一只，则给自己/所有队员/指定ID的召唤兽添加一个BUFFinfo
public class Mechanism1054:BeMechanism
{
    List<int> monsterId = new List<int>();
    int targetType = 0; //0，自己；1，所有队员；2，指定ID的召唤兽
    int buffInfo = 0;
    List<int> targetMonsterId = new List<int>();
    List<BeEvent.BeEventHandleNew> summonMonsterHandles = new List<BeEvent.BeEventHandleNew>();
    int buffId = 0;
    public Mechanism1054(int mid, int lv) : base(mid, lv)
    {

    }
    public override void OnInit()
    {
        monsterId.Clear();
        targetMonsterId.Clear();
        if (data.ValueA.Count > 0)
        {
            for(int i = 0; i < data.ValueA.Length;i++)
            {
                monsterId.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
            }
        }
        targetType = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        buffInfo = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        if (data.ValueD.Count > 0)
        {
            for (int i = 0; i < data.ValueD.Length; i++)
            {
                targetMonsterId.Add(TableManager.GetValueFromUnionCell(data.ValueD[i], level));
            }
        }
        buffId = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
    }

    public override void OnReset()
    {
        removeHandle();
    }

    public override void OnStart()
    {
        if (owner == null) return;
        handleA = owner.RegisterEventNew(BeEventType.onSummon, onSummonMonster);
    }
    public override void OnFinish()
    {
        removeHandle();
    }

    void removeHandle()
    {
        var iter = summonMonsterHandles.GetEnumerator();
        while(iter.MoveNext())
        {
            if(iter.Current != null)
                iter.Current.Remove();
        }
        summonMonsterHandles.Clear();
    }

    private void onMonsterDead(BeEvent.BeEventParam eventParam)
    {
        if (owner == null || owner.CurrentBeBattle == null)
            return;

        BeActor monster = eventParam.m_Obj as BeActor;
        if (monster == null) return;
        
        var topOwner = monster.GetTopOwner(monster);
        if(topOwner != null && topOwner.GetPID() == owner.GetPID())
        {
            if (targetType == 0)
            {
                if (owner == null || owner.buffController == null) return;
                owner.buffController.RemoveBuff(buffId);
            }
            else if (targetType == 1)
            {
                var players = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
                if (players.Count >= 2)
                {
                    var iter = players.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        var curPlayer = iter.Current;
                        if (curPlayer != null &&
                           curPlayer.playerActor != null &&
                           curPlayer.playerActor.buffController != null)
                        {
                            curPlayer.playerActor.buffController.RemoveBuff(buffId);
                        }
                    }
                }
            }
            else if (targetType == 3)
            {
                var filter = new BeSummonMonsterIdFilter();
                filter.ownerId = owner.GetPID();
                filter.summonMonsterId = targetMonsterId;
                List<BeActor> list = GamePool.ListPool<BeActor>.Get();
                owner.CurrentBeScene.FindTargets(list, owner, true, filter);
                var iter = list.GetEnumerator();
                while (iter.MoveNext())
                {
                    var curMonster = iter.Current;
                    if (curMonster != null && curMonster.buffController != null)
                    {
                        curMonster.buffController.RemoveBuff(buffId);
                    }
                }
                GamePool.ListPool<BeActor>.Release(list);

            }
        }
    }
    private void onSummonMonster(BeEvent.BeEventParam args)
    {
        if (owner == null || owner.CurrentBeBattle == null)
            return;
        var monster = args.m_Obj as BeActor;
        if (monster == null) return;
        bool bFind = false;
        for(int i = 0; i < monsterId.Count;i++)
        {
            if(monster.GetEntityData().MonsterIDEqual(monsterId[i]))
            {
                bFind = true;
                break;
            }
        }
        if(!bFind)
        {
            return;
        }
        var handle = monster.RegisterEventNew(BeEventType.onDead, onMonsterDead);
        summonMonsterHandles.Add(handle);
        if (targetType == 0)
        {
            if (owner == null || owner.buffController == null) return;
            owner.buffController.TryAddBuff(buffInfo);
        }
        else if(targetType == 1)
        {
            var players = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
            if (players.Count >= 2)
            {
                var iter = players.GetEnumerator();
                while (iter.MoveNext())
                {
                    var curPlayer = iter.Current;
                    if (curPlayer != null &&
                       curPlayer.playerActor != null &&
                       curPlayer.playerActor.buffController != null)
                    {
                        curPlayer.playerActor.buffController.TryAddBuff(buffInfo);
                    }
                }
            }
        }
        else if(targetType == 2)
        {
            var filter = new BeSummonMonsterIdFilter();
            filter.ownerId = owner.GetPID();
            filter.summonMonsterId = targetMonsterId;
            List<BeActor> list = GamePool.ListPool<BeActor>.Get();
            owner.CurrentBeScene.FindTargets(list, owner, true, filter);
            var iter = list.GetEnumerator();
            while(iter.MoveNext())
            {
                var curMonster = iter.Current;
                if(curMonster != null && curMonster.buffController != null)
                {
                    curMonster.buffController.TryAddBuff(buffInfo);
                }
            }
            GamePool.ListPool<BeActor>.Release(list);
        }
    }
}

