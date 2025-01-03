using System;
using System.Collections.Generic;
using GameClient;

//精神控制
public class Mechanism2048 : BeMechanism
{
    int castSkillID = 0;
    int buffID = 0;
    VInt radius = VInt.zero;
    class TargetInfo
    {
        public BeActor hitTarget = null;   //被精神控制的玩家
        public BeActor attackTarget = null; //被精神控制玩家锁定的目标
        public IBeEventHandle rebornHandle = null;//被精神控制的玩家复活句柄
    };
    List<TargetInfo> targets = new List<TargetInfo>();
    int durTime = 0;
    public Mechanism2048(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }
    public override void OnInit()
    {
        base.OnInit();
        radius = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[0], level), GlobalLogic.VALUE_1000);
        castSkillID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        buffID = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
    }

    public override void OnReset()
    {
        targets.Clear();
        durTime = 0;
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = OwnerRegisterEventNew(BeEventType.onHitOther, onSkillHitTarget);
        //handleA = owner.RegisterEvent(BeEventType.onHitOther, onSkillHitTarget);
        handleB = owner.RegisterEventNew(BeEventType.onHitOtherAfterAddBuff, onSkillHitTargetAfterAddBuff);
        
    }
    private void ReleaseHitTarget(TargetInfo target)
    {
        if (target == null || target.hitTarget == null) return;
        target.hitTarget.Reset();
        if (target.rebornHandle != null)
        {
            target.rebornHandle.Remove();
            target.rebornHandle = null;
        }
        if (target.hitTarget.buffController != null)
        {
            target.hitTarget.buffController.RemoveBuff(buffID);
        }
#if !LOGIC_SERVER
        if (target.hitTarget.isLocalActor)
        {
            InputManager.instance.SetEnable(true);
        }
#endif

    }
    public override void OnFinish()
    {
        base.OnFinish();
        for(int i = 0; i < targets.Count;i++)
        {
            var target = targets[i];
            ReleaseHitTarget(target);
        }
        targets.Clear();
    }
    private void onPlayerReborn(BeEvent.BeEventParam param)
    {
        BeActor target = param.m_Obj as BeActor;
        if (target == null || target.buffController == null) return;
        if (target.buffController.HasBuffByID(buffID) == null) return;
        for(int i = 0; i < targets.Count; i++)
        {
            var curTarget = targets[i];
            if (curTarget == null || curTarget.hitTarget == null) continue;
            if(curTarget.hitTarget.GetPID() == target.GetPID())
            {
                ReleaseHitTarget(curTarget);
                targets.RemoveAt(i);
                return;
            }
        }

    }
    private void RestoreAllTargets()
    { 
        for (int i = 0; i < targets.Count;i++)
        {
            var target = targets[i];
            if(target != null && CanRestoreTarget(target.hitTarget))
            {
                ReleaseHitTarget(target);
                targets.RemoveAt(i);
                i--;
            }
        }
    }
    private bool CanRestoreTarget(BeActor hitTarget)
    {
        //恢复精神控制的玩家为正常状态
        if (hitTarget == null || hitTarget.buffController == null) return true;
        if (hitTarget.IsDead())
        {
            return true;
        }
        if (hitTarget.buffController.HasBuffByID(buffID) == null)
        {
            return true;
        }
        return false;
    }
    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        durTime += deltaTime;
        if(durTime < 100)
        {
            return;
        }
        durTime -= 100;
        RestoreAllTargets();
        OnExecuteControlledTarget();
    }
    //执行精神控制玩家AI
    private void OnExecuteControlledTarget()
    {
        for(int i = 0; i < targets.Count;i++)
        {
            var target = targets[i];
            OnUpdateTarget(target);
        }
    }
    private bool IsTargetExist(BeActor target)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            var curTarget = targets[i];
            if(curTarget != null && curTarget.hitTarget != null &&
                curTarget.hitTarget.GetPID() == target.GetPID())
            {
                return true;
            }
        }
        return false;
    }
    //检查不满足执行精神控制的玩家，删除控制精神buff
    private void onSkillHitTargetAfterAddBuff(BeEvent.BeEventParam args)
    {
        // TODO not have trigger
        /*var hitTarget = args[2] as BeActor;
        if (hitTarget == null) return;
        if (!hitTarget.isMainActor)
        {
            return;
        }
        int skillID = (int)args[3];
        if (skillID != castSkillID) return;
        for (int i = 0; i < targets.Count; i++)
        {
            var curTarget = targets[i];
            if (curTarget != null && curTarget.hitTarget != null &&
                curTarget.hitTarget.GetPID() == hitTarget.GetPID())
            {
                if (!IsTargetActionValid(hitTarget))
                {
                    ReleaseHitTarget(curTarget);
                    targets.RemoveAt(i);
                }
                return;
            }
        }*/
    }
    private void onSkillHitTarget(BeEvent.BeEventParam param)
    {
        var curtarget = param.m_Obj as BeActor;
        if (curtarget == null) return;
        if (!curtarget.isMainActor)
        {
            return;
        }
        if (curtarget.isSpecialMonster) return;
        int skillID  = param.m_Int2;
        
        if (skillID != castSkillID) return;
        if (curtarget.buffController == null) return;
        if (curtarget.buffController.HasBuffByID(buffID) != null) return;
        if (IsTargetExist(curtarget)) return;
        TargetInfo targetinfo = new TargetInfo
        {
            hitTarget = curtarget,
            attackTarget = null,
            rebornHandle = curtarget.RegisterEventNew(BeEventType.onReborn, onPlayerReborn)
        };
        targets.Add(targetinfo);
        //精神错乱情况下，团本默认关闭自动战斗所以不需要管理自动战斗
      //  curtarget.SetAutoFight(false);
#if !LOGIC_SERVER
        if (curtarget.isLocalActor)
        {
            InputManager.instance.SetEnable(false);
        }
#endif
    }
    private BeActor FindNearestTarget(BeActor attacker)
    {
        if (owner == null || owner.CurrentBeBattle == null || owner.CurrentBeBattle.dungeonPlayerManager == null)
            return null;
        var players = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        if (players == null) return null;
        int minDist = 999999999;
        BeActor rightTarget = null;
        for (int i = 0; i < players.Count; i++)
        {
            var curPlayer = players[i];
            if (curPlayer == null) continue;
            var curActor = curPlayer.playerActor;
            if (curActor == null || curActor.IsDead() || curActor.GetPID() == attacker.GetPID())
            {
                continue;
            }
            var dist = curActor.GetDistance(attacker);
            if (dist < minDist)
            {
                rightTarget = curActor;
                minDist = dist;
            }
        }
        return rightTarget;
    }
    private void SetIdle(BeActor hitTarget)
    {
        if (hitTarget.IsCastingSkill())
            return;
        if(hitTarget.sgGetCurrentState() != (int)ActionState.AS_IDLE)
        {
            hitTarget.sgSwitchStates(new BeStateData((int)ActionState.AS_IDLE));
        }
    }
    //检测精神控制的玩家是否满足可以攻击
    private bool IsTargetActionValid(BeActor target)
    {
        
        if (target != null && (target.GetStateGraph() != null &&
            (target.GetStateGraph().CurrentStateHasTag((int)AStateTag.AST_CONTROLED) || (
           target.GetStateGraph().CurrentStateHasTag((int)AStateTag.AST_BUSY)) && !target.HasTag((int)AState.ACS_JUMP)
           )))
        {
            return false;
        }
        return true;
    }
    private void OnUpdateTarget(TargetInfo  curTarget)
    {
        if (curTarget == null || curTarget.hitTarget == null) return;
        if (curTarget.hitTarget.IsDead()) return;
        if (!curTarget.hitTarget.stateController.CanMove())
            return;

        if (!IsTargetActionValid(curTarget.hitTarget))
            return;
        int targetDist = 0;
        if (curTarget.attackTarget == null || curTarget.attackTarget.IsDead())
        {
            //没有锁定目标，找到适合条件的目标
            curTarget.attackTarget = FindNearestTarget(curTarget.hitTarget);
        }
        if (curTarget.attackTarget != null)
        {
            targetDist = curTarget.attackTarget.GetDistance(curTarget.hitTarget);
        }
        if (curTarget.attackTarget == null)
        {
            //没有找到合适的目标则停下来
            curTarget.hitTarget.ResetMoveCmd();
            SetIdle(curTarget.hitTarget);
            return;
        }
        if (curTarget.attackTarget.IsDead()) return;
        if(targetDist < radius)
        {
            //到达最小攻击范围开始攻击
            if (curTarget.hitTarget.CanUseSkill(curTarget.hitTarget.GetEntityData().normalAttackID))
            {
                curTarget.hitTarget.ResetMoveCmd();
                curTarget.hitTarget.UseSkill(curTarget.hitTarget.GetEntityData().normalAttackID);
            }
        }
        else
        {
            //超过攻击范围则移动到目标处
            var dirDist = curTarget.attackTarget.GetPosition() - curTarget.hitTarget.GetPosition();
            curTarget.hitTarget.ResetMoveCmd();

            if (dirDist.x > VInt.half)
            {
                curTarget.hitTarget.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X, true);
            }
            else if (dirDist.x < -VInt.half)
            {
                curTarget.hitTarget.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X_NEG, true);
            }

            if (dirDist.y > VInt.half)
            {
                curTarget.hitTarget.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_Y, true);
            }
            else if (dirDist.y < -VInt.half)
            {
                curTarget.hitTarget.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_Y_NEG, true);
            }
        }
    }
}

