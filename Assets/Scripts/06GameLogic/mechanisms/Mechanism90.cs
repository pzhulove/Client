using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 祭坛保护罩机制
public class Mechanism90 : BeMechanism
{
    int actorNumber;                                //可以进圈的角色数量
    VInt radius;                                    //圈的半径

    List<BeActor> actorList = new List<BeActor>();  //用来保存已经在圈里的角色

    readonly int skillId = 5770;                            //保护罩变色技能

    readonly int interval = 150;                             //距离检测时间间隔
    int timer;

    public Mechanism90(int id, int level) : base(id, level) { }

    public override void OnReset()
    {
        actorList.Clear();
        timer = 0;
    }
    public override void OnInit()
    {
        actorNumber = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        radius = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueB[0], level), GlobalLogic.VALUE_1000);
    }

    public override void OnStart()
    {
        timer = 0;
    }

    public override void OnFinish()
    {
        actorList.Clear();
    }

    public override void OnUpdate(int deltaTime)
    {
        timer += deltaTime;
        if (timer >= interval)
        {
            _checkActorToProtect();
            _checkActorToHurt();

            timer = 0;
        }
    }

    private void _checkActorToProtect()
    {
        if (owner == null)
            return;
        if (owner.IsDead())
            return;

        var targetList = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        for (int i = 0; i < targetList.Count; i++)
        {
            var actor = targetList[i].playerActor;
            if (actor == null)
                continue;
            if (actor.IsDead())
                continue;

            int dis = (actor.GetPosition() - owner.GetPosition()).magnitude;
            if (actorList.Contains(actor))
            {
                if (dis >= radius.i)
                {
                    owner.CancelSkill(skillId);
                    owner.sgSwitchStates(new BeStateData((int)ActionState.AS_IDLE));
                    actorList.Remove(actor);
                }
            }
            else if (actorList.Count < actorNumber && dis < radius.i)
            {
                owner.UseSkill(skillId);
                actorList.Add(actor);
            }
        }
    }

    private void _checkActorToHurt()
    {
        if (owner == null)
            return;
        if (owner.IsDead())
            return;

        var targetList = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        for (int i = 0; i < targetList.Count; i++)
        {
            var actor = targetList[i].playerActor;
            if (actor == null)
                continue;
            if (actor.IsDead())
                continue;
            if (actorList.Contains(actor))
                continue;

            var vec = actor.GetPosition() - owner.GetPosition();
            if (vec.magnitude < radius.i)
            {
                if (actor.sgGetCurrentState() != (int)ActionState.AS_HURT)
                {
                    int forceX = vec.x > 0 ? 60000 : -60000;
                    //actor.Locomote(new BeStateData((int)ActionState.AS_HURT, 0, forceX, 0, 0, 0, 200, true));
                    actor.Locomote(new BeStateData((int)ActionState.AS_HURT) { _StateData2 = forceX, _timeout = 200, _timeoutForce = true });
                }
            }
        }
    }
}
