using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家被上了特定buff之后，怪物的目标强制设为该玩家
/// </summary>
public class Mechanism2021 : BeMechanism
{
    List<IBeEventHandle> handleList = new List<IBeEventHandle>();
    public Mechanism2021(int mid, int lv) : base(mid, lv)
    {
    }

    List<int> buffList = new List<int>();
    int buffID = 0;
    readonly int buffID01 = 521732;
    public override void OnInit()
    {
        base.OnInit();
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            buffList.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }
        buffID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnReset()
    {
        RemoveHandle();
        buffList.Clear();
    }

    public override void OnStart()
    {
        base.OnStart();
        List<BattlePlayer> list = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();

        for (int i = 0; i < list.Count; i++)
        {
            BeActor actor = list[i].playerActor;
            if (actor != null && !actor.IsDead())
            {
                var addBuffHandle = actor.RegisterEventNew(BeEventType.onBuffBeforePostInit, (args) =>
                {
                    if (owner.aiManager == null || actor.buffController == null) return;

                    BeBuff buff = args.m_Obj as BeBuff;
                    if (buffList.Contains(buff.buffID))
                    {
                        if (owner.aiManager.aiTarget != null && AITargetHaveBuff(owner.aiManager.aiTarget)) return;

                        actor.buffController.TryAddBuff(buffID, -1,1,GlobalLogic.VALUE_1000,0,false,null,0,0,owner);
                        owner.aiManager.SetTarget(actor);

                        if (owner.buffController != null)
                        {
                            owner.buffController.TryAddBuff(buffID01,-1);
                        }
                    }
                });

                IBeEventHandle removeBuffHandle = actor.RegisterEventNew(BeEventType.onRemoveBuff, (args) =>
                {
                    if (owner.aiManager == null || actor.buffController == null) return;
                    int buffID = (int)args.m_Int;
                    if (buffList.Contains(buffID) && actor == owner.aiManager.aiTarget)
                    {
                        owner.aiManager.SetTarget(null);
                        actor.buffController.RemoveBuff(this.buffID);
                        if (owner.buffController != null)
                        {
                            owner.buffController.RemoveBuff(buffID01);
                        }
                    }
                });

                handleList.Add(addBuffHandle);
                handleList.Add(removeBuffHandle);
            }
        }
    }

    private bool AITargetHaveBuff(BeActor actor)
    {
        if (actor.buffController != null)
        {
            List<BeBuff> buffList = actor.buffController.GetBuffList();
            for (int i = 0; i < buffList.Count; i++)
            {
                for (int j = 0; j < this.buffList.Count; j++)
                {
                    if (buffList[i].buffID == this.buffList[j])
                        return true;
                }
            }
        }
        return false;
    }

    private void RemoveHandle()
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
        RemoveHandle();
    }

    public override void OnFinish()
    {
        RemoveHandle();
        base.OnFinish();
    }
}
