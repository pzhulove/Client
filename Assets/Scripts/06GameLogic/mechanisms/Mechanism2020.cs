using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 黑色大地监听BOSS变身之后，周围的小怪也要变身
/// </summary>
public class Mechanism2020 : BeMechanism
{
    readonly int buffID = 521716;
    readonly int summonMonsterID = 30950011;
    readonly int monsterID = 31130011;
    readonly int changeBuffID = 521724;

    List<IBeEventHandle> handleList = new List<IBeEventHandle>();
    public Mechanism2020(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        base.OnInit();
    }

    public override void OnReset()
    {
        RemoveHandleList();
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onAddBuff, (args) =>
        {
            BeBuff buff = args.m_Obj as BeBuff;
            if (buff.buffID == buffID)
            {
                List<BeActor> list = GamePool.ListPool<BeActor>.Get();
                owner.CurrentBeScene.FindMonsterByIDAndCamp(list, monsterID, owner.GetCamp());
                for (int i = 0; i < list.Count; i++)
                {
                    BeActor actor = list[i];
                    if (actor.IsDead()) continue;
                    actor.buffController.TryAddBuff(changeBuffID, -1);
                }
                GamePool.ListPool<BeActor>.Release(list);
            }
        });


        List<BeActor> actorList = new List<BeActor>();
        owner.CurrentBeScene.FindMonsterByIDAndCamp(actorList, summonMonsterID, owner.GetCamp());

        for (int i = 0; i < actorList.Count; i++)
        {
            IBeEventHandle handle = actorList[i].RegisterEventNew(BeEventType.onSummon, (args) =>
             {
                 if (owner.buffController.HasBuffByID(buffID) == null) return;
                 BeActor monster = args.m_Obj as BeActor;
                 if (monster != null)
                 {
                     monster.buffController.TryAddBuff(changeBuffID, -1);
                 }
             });

            handleList.Add(handle);
        }
    }

    private void RemoveHandleList()
    {
        for (int i = 0; i < handleList.Count; i++)
        {
            if (handleList[i] != null)
            {
                handleList[i].Remove();
                handleList[i] = null;
            }
        }
        handleList.Clear();
    }

    public override void OnDead()
    {
        base.OnDead();
        RemoveHandleList();
    }

    public override void OnFinish()
    {
        base.OnFinish();
        RemoveHandleList();
    }
}
