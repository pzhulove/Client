using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;

/// <summary>
/// 冤魂缠绕机制
/// </summary>
public class Mechanism2005 : BeMechanism
{
    BeActor target = null;
    VInt dis = VInt.one;
    int skillID = 0;
    public Mechanism2005(int mid, int lv) : base(mid, lv)
    {
    }

    public override void OnInit()
    {
        base.OnInit();
        dis = new VInt(TableManager.GetValueFromUnionCell(data.ValueA[0], level) / 1000.0f);
        skillID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnReset()
    {
        target = null;
    }

    public override void OnStart()
    {
        base.OnStart();
        if (owner == null)
        {
            Finish();
            return;
        }

        target = FindTarget(owner.GetPosition());
        if (target == null)
        {
            Finish();
            return;
        }

        handleA = target.RegisterEventNew(BeEventType.onDead, eventParam =>
        {
            owner.DoDead();
        });


        if (HaveBuff(target) || !target.isMainActor)
        {           
            Finish();
            return;
        }

        handleB = OwnerRegisterEventNew(BeEventType.onBeKilled, _OnBeKilled);

        //
        //handleB = owner.RegisterEvent(BeEventType.onBeKilled, (args) => 
        //{
        //    if(owner.IsDead())
        //    owner.DoDead();
        //});
        owner.stateController.SetAbilityEnable(BeAbilityType.BEGRAB, false);
        owner.buffController.RemoveBuff(521722);
        owner.UseSkill(skillID, true);
        //  owner.PlayAction(ActionType.ActionType_SpecialIdle02);
    }

    private void _OnBeKilled(BeEvent.BeEventParam param)
    {
        if (owner.IsDead())
            owner.DoDead();
    }

    protected BeActor FindTarget(VInt3 pos)
    {
        if (owner == null)
            return null;

        var target = owner.CurrentBeScene.FindNearestRangeTarget(pos, owner, dis);
        return target;
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (owner == null || target == null || !isRunning)
            return;

        if (target.IsDead() || owner.IsDead())
        {
            owner.DoDead();
            return;
        }

        VInt3 targetBodyPos = target.GetPosition();
        VInt3 ownerPos = targetBodyPos;
        if (!owner.GetFace())
        {
            ownerPos.x = targetBodyPos.x - VInt.Float2VIntValue(0.5f);
        }
        else
        {
            ownerPos.x = targetBodyPos.x + VInt.Float2VIntValue(0.5f);
        }
        ownerPos.y -= VInt.Float2VIntValue(0.03f);
        ownerPos.z += VInt.Float2VIntValue(0.3f);
        owner.SetPosition(ownerPos);
        owner.SetFace(target.GetFace());
    }

    private bool HaveBuff(BeActor actor)
    {
        if (actor.buffController != null)
        {
            return actor.buffController.HasBuffByID(521727) != null ||
                   actor.buffController.HasBuffByID(521728) != null ||
                   actor.buffController.HasBuffByID(521729) != null;
        }
        return false;
    }

    public override void OnFinish()
    {
        base.OnFinish();
    }
}
