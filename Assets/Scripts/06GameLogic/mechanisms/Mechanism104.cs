using Protocol;
using System;
using System.Collections.Generic;
using UnityEngine;

//持续跟踪目标
class Mechanism104 : BeMechanism
{
    VInt moveSpeed;
    VInt stopDistance;
    int chaseTime;
    int skillId;

    readonly int intervel = GlobalLogic.VALUE_200;
    int timer = 0;
    BeActor target;

    public Mechanism104(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        timer = 0;
        target = null;
    }
    public override void OnInit()
    {
        moveSpeed = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[0], level), GlobalLogic.VALUE_1000);
        stopDistance = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueB[0], level), GlobalLogic.VALUE_1000);
        chaseTime = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        skillId = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
    }

    public override void OnStart()
    {
        owner.aiManager.Stop();
        timer = intervel;
        FindTarget();
    }

    void FindTarget()
    {
        int minDis = int.MaxValue;
        var players = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        for (int i = 0; i < players.Count; i++)
        {
            var actor = players[i].playerActor;
            if (actor != null && !actor.IsDead())
            {
                int dis = (owner.GetPosition() - actor.GetPosition()).magnitude;
                if (dis < minDis)
                {
                    minDis = dis;
                    target = actor;
                }
            }
        }
    }

    void ChaseTarget()
    {
        if (target == null)
            return;

        var vec = target.GetPosition() - owner.GetPosition();
        if (vec.magnitude <= stopDistance.i)
        {
            owner.SetMoveSpeedX(VInt.zero);
            owner.SetMoveSpeedY(VInt.zero);

            if (skillId > 0)
            {
                ChaseEnd();
            }
        }
        else
        {
            vec.NormalizeTo(moveSpeed.i);
            owner.SetMoveSpeedX(vec.x);
            owner.SetMoveSpeedY(vec.y);
        }

    }

    void ChaseEnd()
    {
        if (skillId > 0)
        {
            owner.UseSkill(skillId, true);
        }
        else
        {
            owner.Locomote(new BeStateData((int)ActionState.AS_IDLE));
        }

        var thisAttachBuff = GetAttachBuff();
        if (thisAttachBuff != null)
        {
            //owner.buffController.RemoveBuff(attachBuff);
            RemoveAttachBuff();
        }
        else
        {
            owner.RemoveMechanism(this);
        }
    }

    public override void OnUpdate(int deltaTime)
    {
        if (target == null)
            return;
        if (target.IsDead())
            return;

        timer += deltaTime;
        if (timer >= intervel)
        {
            timer -= intervel;
            ChaseTarget();
        }

        if (chaseTime > 0)
        {
            chaseTime -= deltaTime;
            if (chaseTime <= 0)
            {
                ChaseEnd();
            }
        }
    }

    public override void OnFinish()
    {
        owner.aiManager.Start();
        owner.SetMoveSpeedX(VInt.zero);
        owner.SetMoveSpeedY(VInt.zero);
    }
}