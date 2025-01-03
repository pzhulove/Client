using System.Collections;
using System.Collections.Generic;
using GameClient;
//指定怪物id的召唤物死亡时，移除buff
public class Mechanism1040: BeMechanism
{
    public Mechanism1040(int id, int level) : base(id, level) { }
    private Dictionary<int,List<int>> aLivemonsterCounts = new Dictionary<int, List<int>>();
    private List<int> monsterIds = new List<int>();
    private List<int> buffIds = new List<int>();
    private List<BeEvent.BeEventHandleNew> monsterDeadHandles = new List<BeEvent.BeEventHandleNew>();
    public override void OnInit()
    {
        monsterIds.Clear();
        aLivemonsterCounts.Clear();
        buffIds.Clear();
        if (data.ValueA.Count > 0)
        {
            for (int i = 0; i < data.ValueA.Count; i++)
            {
                int monsterId = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
                monsterIds.Add(monsterId);
                aLivemonsterCounts.Add(monsterId,new List<int>());
            }
        }

        if(data.ValueB.Count > 0)
        {
            for (int i = 0; i < data.ValueB.Count; i++)
            {
                buffIds.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
            }
        }
    }

    public override void OnReset()
    {
        aLivemonsterCounts.Clear();
        monsterIds.Clear();
        buffIds.Clear();
        removeHandle();
    }
    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onSummon, onSummonMonster);
    }
    public override void OnFinish()
    {
        base.OnFinish();
        removeHandle();
        if (owner == null || owner.IsDead()) return;
        var iter = buffIds.GetEnumerator();
        while(iter.MoveNext())
        {
            owner.buffController.RemoveBuff(iter.Current);
        }
    }

    void removeHandle()
    {
        for(int i = 0; i < monsterDeadHandles.Count;i++)
        {
            monsterDeadHandles[i].Remove();
        }
        monsterDeadHandles.Clear();
    }

    private void onSummonMonster(BeEvent.BeEventParam args)
    {
        BeActor monster = args.m_Obj as BeActor;
        if (monster == null)
            return;

        for(int i = 0; i < monsterIds.Count;i++)
        {
            if(monster.GetEntityData().MonsterIDEqual(monsterIds[i]))
            {
                if(aLivemonsterCounts.ContainsKey(monsterIds[i]))
                {
                    if(!aLivemonsterCounts[monsterIds[i]].Contains(monster.GetPID()))
                    {
                        aLivemonsterCounts[monsterIds[i]].Add(monster.GetPID());
                    }
                }
                monsterDeadHandles.Add(monster.RegisterEventNew(BeEventType.onDead, onMonsterDead));
                break;
            }
        }
    }

    // 策划说，装备的buffid是唯一的，如果出现该机制没有上过buff 而机制消失时删除了对应的buffid 那么请找策划
    private void onMonsterDead(BeEvent.BeEventParam eventParam)
    {
        var monster = eventParam.m_Obj as BeActor;
        if(monster != null && owner != null && owner.buffController != null)
        {
            for (int i  = 0; i < monsterIds.Count;i++)
            {
                if(monster.GetEntityData().MonsterIDEqual(monsterIds[i]))
                {
                    if (aLivemonsterCounts.ContainsKey(monsterIds[i]))
                    {
                        int findIndex = aLivemonsterCounts[monsterIds[i]].FindIndex(x =>
                        {
                            return x == monster.GetPID();
                        });
                        if(findIndex >= 0)
                        {
                            aLivemonsterCounts[monsterIds[i]].RemoveAt(findIndex);
                            if (aLivemonsterCounts[monsterIds[i]].Count <= 0 && i < buffIds.Count)
                            {
                                owner.buffController.RemoveBuff(buffIds[i]);
                            }
                        }
                    }
                }
            }
        }
    }
}

