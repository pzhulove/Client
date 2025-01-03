using System;
using System.Collections.Generic;
using GameClient;

//指定怪物死亡后，让关联怪物也死亡并且移动到指定怪物的死亡位置处

public class Mechanism1079 : BeMechanism
{
    private List<int> deadMonsterIds = new List<int>();
    private List<int> relatedMonsterIds = new List<int>();
    private List<int> relatedBuffIds = new List<int>();
    public Mechanism1079(int mid, int lv) : base(mid, lv) { }
    public override void OnInit()
    {
        base.OnInit();
        deadMonsterIds.Clear();
        relatedMonsterIds.Clear();
        relatedBuffIds.Clear();
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
                relatedMonsterIds.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
            }
        }
        if (data.ValueC.Count > 0)
        {
            for (int i = 0; i < data.ValueC.Count; i++)
            {
                relatedBuffIds.Add(TableManager.GetValueFromUnionCell(data.ValueC[i], level));
            }
        }
    }
    private void onMonsterDead(BeEvent.BeEventParam args)
    {
        var deadMonster = args.m_Obj as BeActor;
        if (deadMonster == null) return;
        for (int i = 0; i < deadMonsterIds.Count; i++)
        {
            if (deadMonster.GetEntityData() != null && deadMonster.GetEntityData().MonsterIDEqual(deadMonsterIds[i]))
            {
                if (i < 0 && i >= relatedMonsterIds.Count)
                {
                    Logger.LogErrorFormat("mechanism {0} summonMonsterIds {1} is out of range {2}", this.mechianismID, i, relatedMonsterIds[i]);
                    return;
                }
                if (i < 0 && i >= relatedBuffIds.Count)
                {
                    Logger.LogErrorFormat("mechanism {0} buffIds {1} is out of range {2}", this.mechianismID, i, relatedBuffIds[i]);
                    return;
                }
                List<BeActor> list = GamePool.ListPool<BeActor>.Get();
                owner.CurrentBeScene.FindActorById2(list, relatedMonsterIds[i]);
                if (list.Count > 0)
                {
                    for(int j = 0; j < list.Count;j++)
                    {
                        var curMonster = list[j];
                        if(curMonster != null && !curMonster.IsDead() && 
                           curMonster.buffController != null && 
                           curMonster.buffController.HasBuffByID(relatedBuffIds[i]) != null)
                        {
                            var pos = deadMonster.GetPosition();
                            pos.z = 0;
                            curMonster.SetPosition(pos);
                            curMonster.DoDead(true);
                            break;
                        }
                    }
                }
                GamePool.ListPool<BeActor>.Release(list);
                return;
            }
        }
    }
    public override void OnStart()
    {
        base.OnStart();
        if (owner == null || owner.CurrentBeScene == null) return;
        sceneHandleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onMonsterDead, onMonsterDead);
    }
}

