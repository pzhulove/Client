using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanism2034 : BeMechanism
{
    private int delayTime = 0;
    private int monsterID = 0;
    private int buffID = 0;
    public Mechanism2034(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
        base.OnInit();
        monsterID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        buffID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        delayTime = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
    }
    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onDead, eventParam =>
        {
            var monsterList = GamePool.ListPool<BeActor>.Get();
            owner.CurrentBeScene.GetFilterTarget(monsterList, new BeMechanismFilter(GetMechanismIndex()));
            if (monsterList.Count <= 0)
            {
                List<BeEntity> list = owner.CurrentBeScene.GetSaveTempList();
                for (int i = 0; i < list.Count; i++)
                {
                    BeActor actor = list[i] as BeActor;
                    if (actor != null)
                    {
                        Mechanism2032 mechanism = actor.GetMechanismByIndex(2032) as Mechanism2032;
                        if (mechanism != null)
                        {
                            mechanism.EndAirBattle(delayTime);
                        }
                    }
                }
                BeActor monster = owner.CurrentBeScene.FindMonsterByID(monsterID);
                if (monster != null)
                {
                    monster.buffController.TryAddBuff(buffID,-1);
                }
            }
            GamePool.ListPool<BeActor>.Release(monsterList);
        });
    }

    public override void OnDead()
    {
        base.OnDead();
    }

    public override void OnFinish()
    {
        base.OnFinish();
    }
}
