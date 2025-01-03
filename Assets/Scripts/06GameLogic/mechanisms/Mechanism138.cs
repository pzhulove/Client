using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//杀意波动机制2
public class Mechanism138 : BeMechanism
{
    public Mechanism138(int mid, int lv) : base(mid, lv) { }

    protected VInt radius = 3000;               //范围
    protected int entityId = 63610;         //创建的实体ID
    protected int targetCount = 9999;          //目标数量

    //寻找目标的时间间隔
    public int timeAcc = 1000;
    protected int curTimeAcc = 0;

    public override void OnReset()
    {
        targetCount = 0;
        curTimeAcc = 0;
    }
    public override void OnInit()
    {
        base.OnInit();
        radius = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[0], level), GlobalLogic.VALUE_1000);
        entityId = TableManager.GetValueFromUnionCell(data.ValueB[0],level);
        timeAcc = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        if (data.ValueD.Count > 0)
        {
            targetCount = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        curTimeAcc = timeAcc;

        int addEntityCount = 0;
        int addRadius = GlobalLogic.VALUE_1000;
        int addTimeAcc = GlobalLogic.VALUE_1000;

        for (int i = 0; i < owner.MechanismList.Count; i++)
        {
            var m = owner.MechanismList[i] as Mechanism1005;
            if (m == null)
                continue;
            if (m.impactMechanismIdList.Contains(mechianismID))
                continue;
            addEntityCount += m.addEntityCount;
            addRadius += m.addRadius;
            addTimeAcc += m.addTimeAccRate;
        }

        if (addRadius == GlobalLogic.VALUE_1000 && addTimeAcc == GlobalLogic.VALUE_1000 && addEntityCount == 0)
        {
            var actor = owner.GetOwner() as BeActor;
            if (actor != null)
            {
                for (int i = 0; i < actor.MechanismList.Count; i++)
                {
                    var m = actor.MechanismList[i] as Mechanism1005;
                    if (m == null)
                        continue;
                    if (!m.impactMechanismIdList.Contains(mechianismID))
                        continue;
                    addEntityCount += m.addEntityCount;
                    addRadius += m.addRadius;
                    addTimeAcc += m.addTimeAccRate;
                }
            }
        }

        radius = radius.i * VFactor.NewVFactor(addRadius, GlobalLogic.VALUE_1000);
        targetCount += addEntityCount;
        timeAcc = timeAcc * VFactor.NewVFactor(addTimeAcc, GlobalLogic.VALUE_1000);
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        UpdateCheckTime(deltaTime);
    }

    protected void UpdateCheckTime(int deltaTime)
    {
        if (curTimeAcc >= timeAcc)
        {
            curTimeAcc -= timeAcc;
            DoAttack();
        }
        else
        {
            curTimeAcc += deltaTime;
        }
    }

    protected void DoAttack()
    {
        List<BeActor> targets = GamePool.ListPool<BeActor>.Get();
        var pos = owner.GetPosition();
        pos.z = 0;
        int count = 0;
        owner.CurrentBeScene.FindActorInRange(targets, pos, radius);
        for (int i = 0; i < targets.Count; i++)
        {
            if (count >= targetCount)
                continue;
            BeActor target = targets[i];
            if (!target.IsDead() && target.GetCamp() != owner.GetCamp() && target.GetLifeState() == (int)EntityLifeState.ELS_ALIVE)
            {
                BeActor attackActor = (BeActor)owner.GetOwner();
                if (attackActor != null && target.stateController.CanBeHit())
                {
                    count++;
                    VInt3 targetPos = target.GetPosition();
                    targetPos.z = 0;
                    attackActor.AddEntity(entityId, targetPos,level);
                }
            }
        }
        GamePool.ListPool<BeActor>.Release(targets);
    }
}
