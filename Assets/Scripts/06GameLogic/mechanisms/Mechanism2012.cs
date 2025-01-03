using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 黑色大地boss关卡变身之后解救机制
/// </summary>
public class Mechanism2012 : BeMechanism
{
    private readonly int[] buffList = new int[] { 521828, 521831, 521830, 521834 };//队友变身BUFF
    private readonly int[] buffArray = new int[] { 521823, 521824, 521825 };//恶魔变身BUFf
    private readonly int[] monsterIDs = new int[] { 30710011, 30720011, 30730011 };
    public Mechanism2012(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        base.OnInit();
    }

    /// <summary>
    /// 变身怪物之后被队友攻击之后可以解救
    /// </summary>
    public override void OnStart()
    {
        base.OnStart();
        handleA = OwnerRegisterEventNew(BeEventType.onHit, (args) =>
        //handleA = owner.RegisterEvent(BeEventType.onHit, (args) =>
        {
            BeActor actor = args.m_Obj as BeActor;
            if (actor != null)
            {
                //被队友攻击之后移除变身buff
                if (actor.isMainActor && actor.isSpecialMonster)
                {
                    for (int i = 0; i < buffList.Length; i++)
                    {
                        BeEntity topOwner = owner.GetOwner();
                        if (topOwner != null)
                        {
                            BeActor beactor = topOwner as BeActor;
                            if (beactor != null)
                            {
                                beactor.buffController.RemoveBuff(buffList[i]);
                            }
                        }
                    }

                    //剩最后一个自己移除buff
                    if (IsLastOneHaveBuff())
                    {
                        BeEntity entity = actor.GetOwner();
                        if (entity != null)
                        {
                            BeActor beactor = entity as BeActor;
                            if (beactor != null)
                            {
                                for (int i = 0; i < buffArray.Length; i++)
                                {
                                    beactor.buffController.RemoveBuff(buffArray[i]);
                                }
                            }
                        }
                    }
                }

            }
        });

        handleB = owner.RegisterEventNew(BeEventType.onSummon, (args) =>
        {
           
            BeActor monster = args.m_Obj as BeActor;
            if (monster.GetEntityData().MonsterIDEqual(31140011))
            {
                monster.AddMechanism(5361, level);
            }
        });

        handleC = owner.RegisterEventNew(BeEventType.onBeHitAfterFinalDamage, args =>
        //handleC = owner.RegisterEvent(BeEventType.onBeHitAfterFinalDamage, (args) =>
        {
            //被队友攻击，伤害不计算
            //int[] damage = args[0] as int[];
            BeEntity attacker = args.m_Obj as BeEntity;
            if (attacker != null)
            {
                BeActor beactor = attacker as BeActor;
                if (beactor != null && beactor.isMainActor && beactor.isSpecialMonster)
                {
                    args.m_Int = 0;
                }
            }
        });
    }

    private bool IsLastOneHaveBuff()
    {
        int cnt = 0;
        List<BattlePlayer> list = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < buffList.Length; j++)
            {
                if (HasBuff(list[i].playerActor, buffList[j]))
                {
                    cnt++;
                    break;
                }
            }
        }
        if (cnt == 0)
            return true;
        return false;
    }

    private bool HasBuff(BeActor entity, int buffID)
    {
        BeEntity actor = entity.GetOwner();
        if (actor != null)
        {
            BeActor beactor = actor as BeActor;
            if (beactor.buffController.HasBuffByID(buffID) != null)
                return true;

        }
        return false;
    }

    public override void OnFinish()
    {
        base.OnFinish();
    }
}
