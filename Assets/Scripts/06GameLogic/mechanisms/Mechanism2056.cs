using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 幻影触碰消失机制
/// </summary>
public class Mechanism2056 : BeMechanism
{
    private int monsterID = 81120011;
    private int bossID = 87100031;
    private Mechanism2057 mechanism;
    public Mechanism2056(int mid, int lv) : base(mid, lv)
    {
        var value = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        if (value != 0) 
        {
            monsterID = value;
        }
        value = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        if(value != 0)
        {
            bossID = value;
        }
    }

    public override void OnReset()
    {
        mechanism = null;
        monsterID = 81120011;
        bossID = 87100031;
}

    public override void OnStart()
    {
        base.OnStart();
        BeActor boss = owner.CurrentBeScene.FindMonsterByID(bossID);
        if (boss != null)
        {
            mechanism = boss.GetMechanismByIndex(2057) as Mechanism2057;
        }

        //明亮幻影触碰到自己的召唤者死亡
        //handleA = owner.RegisterEvent(BeEventType.onCollide, (args) => 
        //{


        //});
        handleA = OwnerRegisterEventNew(BeEventType.onCollide, _OnCollide);

        //明亮幻影死亡再召唤
        handleB = owner.RegisterEventNew(BeEventType.onDead, eventParam => 
        {
            if (mechanism != null && mechanism.IsTimeStop()) return;
            VInt3 randomPos = owner.CurrentBeScene.GetRandomPos();
            if (owner.CurrentBeScene.IsInBlockPlayer(randomPos))
                randomPos = owner.GetPosition();
            BeActor acotr = owner.GetOwner() as BeActor;
            if (acotr != null)
            {
                owner.CurrentBeScene.SummonMonster(monsterID, randomPos, 1, acotr);
            }
        });
    }

    private void _OnCollide(GameClient.BeEvent.BeEventParam param)
    {
        BeEntity entity = param.m_Obj as BeEntity;
        BeActor actor = entity as BeActor;
        if (actor != null && actor.isMainActor && actor.GetPID() == owner.GetOwner().GetPID())
        {
            owner.DoDead();
        }
    }

    public override void OnFinish()
    {
        base.OnFinish();
    }
}
