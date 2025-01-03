using System;
using System.Collections.Generic;
//随机选择场上一个怪物，使其立即死亡，并且在死亡的同时原地召唤另一个怪物
//这个机制不能用buff上因为会全部清除buff
public class Mechanism1074 : BeMechanism
{
    private List<int> deadMonsterIds = new List<int>();
    private List<int> summonMonsterIds = new List<int>();
    private List<int> buffIds = new List<int>();
    private List<BeActor> searchResults = new List<BeActor>();
    private string effectPath = string.Empty;
    public Mechanism1074(int mid, int lv) : base(mid, lv){ }
    public override void OnInit()
    {
        base.OnInit();
        deadMonsterIds.Clear();
        summonMonsterIds.Clear();
        if (data.ValueA.Count > 0)
        {
            for (int i = 0; i < data.ValueA.Count; i++)
            {
                deadMonsterIds.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
            }
        }

        if (data.ValueB.Count > 0)
        {
            for (int i = 0; i < data.ValueB.Count; i++)
            {
                summonMonsterIds.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
            }
        }
        if (data.ValueC.Count > 0)
        {
            for (int i = 0; i < data.ValueC.Count; i++)
            {
                buffIds.Add(TableManager.GetValueFromUnionCell(data.ValueC[i], level));
            }
        }
        if (data.StringValueA.Count > 0)
        {
            effectPath = data.StringValueA[0];
        }
    }

    public override void OnReset()
    {
        buffIds.Clear();
        searchResults.Clear();
        effectPath = "";
    }

    public override void OnStart()
    {
        base.OnStart();
        if (owner == null || owner.CurrentBeScene == null || owner.FrameRandom == null) return;
        searchResults.Clear();
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        for (int i = 0; i < deadMonsterIds.Count; i++)
        {
            owner.CurrentBeScene.FindActorById2(list, deadMonsterIds[i],true);
            if (list.Count > 0)
            {
                searchResults.AddRange(list);
            }
        }
        GamePool.ListPool<BeActor>.Release(list);
        if (searchResults.Count <= 0) return;
        int monsterIndex = owner.FrameRandom.InRange(0, searchResults.Count);
        var deadMonster = searchResults[monsterIndex];
        for (int i = 0; i < deadMonsterIds.Count; i++)
        {
            if(deadMonster != null && deadMonster.GetEntityData() != null && deadMonster.GetEntityData().MonsterIDEqual(deadMonsterIds[i]))
            {
                int level = deadMonster.GetEntityData().level;
                var pos = deadMonster.GetPosition();
                pos.z = 0;
                if (i < 0 && i >= summonMonsterIds.Count)
                {
                    Logger.LogErrorFormat("mechanism {0} summonMonsterIds {1} is out of range {2}", this.mechianismID, i, deadMonsterIds[i]);
                    return;
                }
                if(i < 0 && i >= buffIds.Count)
                {
                    Logger.LogErrorFormat("mechanism {0} buffIds {1} is out of range {2}", this.mechianismID, i, deadMonsterIds[i]);
                    return;
                }
                if (!deadMonster.IsDead() && deadMonster.m_iEntityLifeState != (int)EntityLifeState.ELS_CANREMOVE)
                {
                    deadMonster.SetAutoFight(false);
                    if (deadMonster.buffController != null)
                    {
                        deadMonster.buffController.RemoveAllBuff();
                        deadMonster.buffController.TryAddBuff(buffIds[i], -1);
                    }
                    if (deadMonster.m_pkGeActor != null)
                    {
                        deadMonster.m_pkGeActor.HideActor(true);
                    }
                }
                owner.CurrentBeScene.SummonMonster(summonMonsterIds[i] + level * 100,pos,owner.GetCamp(), owner);
#if !LOGIC_SERVER
                if (!string.IsNullOrEmpty(effectPath))
                {
                    if(owner.CurrentBeScene.currentGeScene!= null)
                    {
                        owner.CurrentBeScene.currentGeScene.CreateEffect(effectPath,0,pos.vec3);
                    }
                }
#endif
                return;
            }
        }
    }
}

