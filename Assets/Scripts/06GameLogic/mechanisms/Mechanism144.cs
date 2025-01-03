using System;
using System.Collections.Generic;
using UnityEngine;
public class Mechanism144 : BeMechanism
{
    public enum VISIBLE_STAT
    {
        INVISABLE,
        VISABLE,
        NONE,
    }
    string effectPath = string.Empty;
    VInt invisibleDistToPlayer = 0;
    VInt visibleDistToPlayer = 0;
    List<int> buffIds = new List<int>();
    List<int> buffTimes = new List<int>();
    readonly int interVal = 100;
    int durTime = 0;
    GeEffectEx effect = null;
    VISIBLE_STAT mVisStat = VISIBLE_STAT.INVISABLE;
    public Mechanism144(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        effectPath = string.Empty;
        buffIds.Clear();
        buffTimes.Clear();
        durTime = 0;
        effect = null;
        mVisStat = VISIBLE_STAT.INVISABLE;
    }
    public override void OnInit()
    {
        if(data.StringValueA.Length > 0)
        {
            effectPath = data.StringValueA[0];
        }
        invisibleDistToPlayer = new VInt(TableManager.GetValueFromUnionCell(data.ValueA[0], level) / 1000.0f);
        for(int i = 0; i < data.ValueB.Length;i++)
        {
            buffIds.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
        }
        for(int i = 0; i < data.ValueC.Length;i++)
        {
            buffTimes.Add(TableManager.GetValueFromUnionCell(data.ValueC[i], level));
        }
        visibleDistToPlayer = new VInt(TableManager.GetValueFromUnionCell(data.ValueD[0], level) / 1000.0f);
        durTime = 0;
    }
    public override void OnStart()
    {
     
        for (int i = 0; i < buffIds.Count && i < buffTimes.Count; i++)
        {
            if (owner.buffController.HasBuffByID(buffIds[i]) == null)
            {
                owner.buffController.TryAddBuff(buffIds[i], buffTimes[i]);
            }
        }
        owner.buffController.TryAddBuff(21, GlobalLogic.VALUE_100000 * 10);
        durTime = 0;
#if !LOGIC_SERVER
        if (owner == null || owner.m_pkGeActor == null) return;
        effect = owner.m_pkGeActor.CreateEffect(effectPath, null, GlobalLogic.VALUE_100000 * 10, Vec3.zero);
        var parent = owner.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
        if (effect != null && parent != null)
        {
            Battle.GeUtility.AttachTo(effect.GetRootNode(), parent);
        }
#endif
    }
    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        
        durTime += deltaTime;
        if (durTime < interVal) return;
        durTime -= interVal;
        if (owner == null || owner.CurrentBeBattle == null || owner.CurrentBeBattle.dungeonPlayerManager == null) return;
        if (owner.IsDead()) return;
        var players = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        VISIBLE_STAT curStat = VISIBLE_STAT.NONE;
        int inVisableStatCount = 0;
        int validPlayerCount = 0;
        for (int i = 0;i < players.Count;i++)
        {
            if(players[i] == null || players[i].playerActor == null || players[i].playerActor.IsDead())
            {
                continue;
            }
            validPlayerCount++;
            var actor = players[i].playerActor;
            int dist = actor.GetDistance(owner);
            if (visibleDistToPlayer >= dist)
            {
                curStat = VISIBLE_STAT.VISABLE;
                break;
            }
            if(invisibleDistToPlayer <= dist)
            {
                inVisableStatCount++;
            }
        }
        if(validPlayerCount == inVisableStatCount)
        {
            curStat = VISIBLE_STAT.INVISABLE;
        }
        if (mVisStat == curStat) return;
        mVisStat = curStat;
        if (mVisStat == VISIBLE_STAT.VISABLE)
        {
            for(int i = 0; i < buffIds.Count;i++)
            {
                var curbuff = owner.buffController.HasBuffByID(buffIds[i]);
                if(curbuff != null)
                    owner.buffController.RemoveBuff(curbuff);
            }

            var invisbleBuff = owner.buffController.HasBuffByID(21);
            if(invisbleBuff != null)
            {
                owner.buffController.RemoveBuff(invisbleBuff);
                var visableBuff = owner.buffController.HasBuffByID(42);
                if (visableBuff == null)
                {
                    owner.buffController.TryAddBuff(42, GlobalLogic.VALUE_1500);
                }
            }
#if !LOGIC_SERVER
            if (effect != null)
            {
                effect.SetVisible(false);
            }
#endif
        }
        else if(mVisStat == VISIBLE_STAT.INVISABLE)
        {
            for (int i = 0; i < buffIds.Count && i < buffTimes.Count; i++)
            {
                if(owner.buffController.HasBuffByID(buffIds[i]) == null)
                {
                    owner.buffController.TryAddBuff(buffIds[i], buffTimes[i]);
                }
            }

            var visableBuff = owner.buffController.HasBuffByID(42);
            if (visableBuff != null)
            {
                owner.buffController.RemoveBuff(visableBuff);
            }

            var invisbleBuff = owner.buffController.HasBuffByID(21);
            if (invisbleBuff == null)
            {
                owner.buffController.TryAddBuff(21, GlobalLogic.VALUE_100000 * 10);
            }
#if !LOGIC_SERVER
            if (owner.m_pkGeActor == null) return;
            var parent = owner.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
            //避免多次重复绑定产生消耗
            if (effect != null && parent != null && 
                effect.GetRootNode() != null &&
                effect.GetRootNode().transform != null &&
                effect.GetRootNode().transform.parent == null)
            {
                Battle.GeUtility.AttachTo(effect.GetRootNode(), parent);
            }

            if (effect != null)
            {
                effect.SetVisible(true);
            }
#endif
        }
    }

    public override void OnFinish()
    {
        if(effect != null && owner != null && owner.m_pkGeActor != null)
        {
            owner.m_pkGeActor.DestroyEffect(effect);
            effect = null;
        }
    }
}

