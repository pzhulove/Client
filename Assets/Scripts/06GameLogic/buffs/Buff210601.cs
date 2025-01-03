using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Buff210601 : BeBuff
{
    VInt targetEffectRange;
    VFactor attachAttackPercent;
    int summonBuffID;

	int checkInterval = GlobalLogic.VALUE_1000;
    int checkTimeAcc = 0;

    BeActor staticOwner = null;

    public Buff210601(int bi, int buffLevel, int buffDuration, int attack = 0) :base(bi, buffLevel, buffDuration, attack)
    {

    }

    public override void OnReset()
    {
        targetEffectRange = VInt.zero;
        attachAttackPercent = VFactor.zero;
        summonBuffID = 0;
        checkTimeAcc = 0;
        staticOwner = null;
    }

    public override void OnInit()
    {
		targetEffectRange = VInt.NewVInt(TableManager.GetValueFromUnionCell(buffData.ValueA[0], level),GlobalLogic.VALUE_1000);
		attachAttackPercent = new VFactor(TableManager.GetValueFromUnionCell(buffData.ValueB[0], level),GlobalLogic.VALUE_100);
        summonBuffID = TableManager.GetValueFromUnionCell(buffData.ValueC[0], level);
    }

    public override void OnStart()
    {
        if (staticOwner != null && !staticOwner.IsDead() && staticOwner != owner)
        {
            staticOwner.buffController.RemoveBuff(buffID);
        }

        staticOwner = owner;
    }

    public override void OnUpdate(int delta)
    {
        checkTimeAcc += delta;
        if (checkTimeAcc > checkInterval)
        {
            checkTimeAcc -= checkInterval;

            DoBuffWork();
        }
    }

    void DoBuffWork()
    {
		List<BeActor> actors = GamePool.ListPool<BeActor>.Get();
		owner.CurrentBeScene.FindActorInRange(actors, owner.GetPosition(), targetEffectRange, owner.m_iCamp == 0 ? 1 : 0);
        for(int i=0; i<actors.Count; ++i)
        {
            var actor = actors[i];
            if (actor.GetEntityData().isSummonMonster)
            {
                if (actor.buffController.HasBuffByID(summonBuffID) == null)
                {
                    var buff = actor.buffController.TryAddBuff(summonBuffID, duration,level);
                    if (buff != null)
                    {
                        if (actor.aiManager != null)
                        {
                            //actor.aiManager.StopCurrentCommand();
                            actor.aiManager.SetTarget(owner, true);
                            //actor.aiManager.UpdateThinkAttack(0, true);
							actor.aiManager.ResetAction();
							actor.aiManager.ResetDestinationSelect();

                            //Logger.LogErrorFormat("集火成功!!!");
                        }

                        buff.RegisterEventNew(BeEventType.onBuffFinish, args =>
                        {
                            if (actor.aiManager != null)
                            {
                                actor.aiManager.targetUnchange = false;
                                //Logger.LogErrorFormat("取消集火!!!");
                            }
                        });
                    }
                }
                
            }
        }

		GamePool.ListPool<BeActor>.Release(actors);
    }

    public override void OnFinish()
    {
        staticOwner = null;

        //Logger.LogErrorFormat("OnFinish staticOnwer  = null");
    }

}
