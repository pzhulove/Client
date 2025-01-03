using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using behaviac;
public partial class BeAIManager
{
    public ScenarioAICommand scenrioCmd;
    public int cntDonotSetFace = 0;
    protected int deltaTime = GlobalLogic.VALUE_200;
    protected int count = 0;

    public void StopCurrentScenrioCmd()
    {
        if (scenrioCmd != null)
        {
            scenrioCmd.End();
            scenrioCmd = null;
        }
    }

    public behaviac.EBTStatus Action_Move(int offsetX, int offsetY)
    {
        if (scenrioCmd == null)
        {
            if (owner != null)
            {
                var targetPos = new VInt3(offsetX / 1000f, offsetY / 1000f, 0) + owner.GetPosition();
                targetPos = FindStandPosition(targetPos, owner.CurrentBeScene, owner.GetFace());

                MoveToScenarioAICommand cmd = new MoveToScenarioAICommand(targetPos, (BeActor)owner);
                cmd.Execute();

                scenrioCmd = cmd;
            }
        }

        if (scenrioCmd != null)
        {
            return scenrioCmd.Tick(deltaTime);
        }

        return behaviac.EBTStatus.BT_SUCCESS;
    }

    public behaviac.EBTStatus Action_PlayAction(string actionName, int duration, behaviac.EndType hitEnd, int actionDuration=0)
    {
        if (scenrioCmd == null)
        {
            PlayActionScenarioAICommand cmd = new PlayActionScenarioAICommand(actionName, (BeActor)owner, actionDuration, duration, hitEnd);
            cmd.Execute();

            scenrioCmd = cmd;
        }

        if (scenrioCmd != null)
        {
            return scenrioCmd.Tick(deltaTime);
        }

        return behaviac.EBTStatus.BT_RUNNING;
    }
    
    public behaviac.EBTStatus Action_ShowDialog(int dialogId, int duration, behaviac.EndType hitEnd)
    {
        if (scenrioCmd == null)
        {
            ShowDialogScenarioAICommand cmd = new ShowDialogScenarioAICommand(dialogId, (BeActor)owner, duration, hitEnd);
            cmd.Execute();

            scenrioCmd = cmd;
        }

        if (scenrioCmd != null)
        {
            return scenrioCmd.Tick(deltaTime);
        }

        return behaviac.EBTStatus.BT_RUNNING;
    }

    public behaviac.EBTStatus StopScenrioCommand()
    {
        count++;
        if (count <= 1)
        {
            if (scenrioCmd != null)
                scenrioCmd.NeedEnd = true;
            return behaviac.EBTStatus.BT_RUNNING;
        }
            
        else
        {
            count = 0;
            
            return behaviac.EBTStatus.BT_SUCCESS;
        }

         return behaviac.EBTStatus.BT_SUCCESS;
    }

    public behaviac.EBTStatus Action_SetFace(behaviac.BE_Face dir)
    {
        BeActor target = null;
        target = owner as BeActor;
        if (target == null)
            return behaviac.EBTStatus.BT_SUCCESS;

        if (dir == BE_Face.LEFT)
            target.SetFace(true, true);
        else if (dir == BE_Face.RIGHT)
            target.SetFace(false, true);
        else if (dir == BE_Face.OPPOSITE)
            target.SetFace(!target.GetFace(), true);
        else if (dir == BE_Face.FACETONEAREST)
        {
            var battle = target.CurrentBeBattle;
            if (battle != null)
            {
                var allPlayer = battle.dungeonPlayerManager.GetAllPlayers();

                bool face = false;
                long sqrtDistance = -1;

                foreach (var player in allPlayer)
                {
                    if (player == null)
                    {
                        continue;
                    }
                    var actor = player.playerActor;
                    if (actor == null)
                    {
                        continue;
                    }

                    long distance = (actor.GetPosition2() - target.GetPosition2()).sqrMagnitudeLong;
                    if (sqrtDistance == -1 || sqrtDistance > distance)
                    {
                        sqrtDistance = distance;
                        face = actor.GetPosition2().x < target.GetPosition2().x;
                    }
                }

                target.SetFace(face, true);
            }
        }
        return EBTStatus.BT_SUCCESS;
    }

    public behaviac.EBTStatus Action_SetPosition(int x, int y, int z, bool check = false)
    {
        var targetPos = new VInt3(x / 1000f, y / 1000f, z / 1000f);
        if (owner == null)
            return behaviac.EBTStatus.BT_SUCCESS;

        if (check)
        {
            owner.SetStandPosition(targetPos, true);
        }
        else
        {
            owner.SetPosition(targetPos, true);
        }

        var actor = owner as BeActor;
        if (actor.isLocalActor && !actor.IsDead())
        {
#if !LOGIC_SERVER
            var geActor = actor.m_pkGeActor;
            owner.CurrentBeScene.currentGeScene.GetCamera().GetController().SetCameraPos(geActor.GetPosition());
#endif
        }

        return behaviac.EBTStatus.BT_SUCCESS;
    }

    public behaviac.EBTStatus Action_UseSkill(int skillID, behaviac.EndType hitEnd)
    {
        if (scenrioCmd == null)
        {
            UseSkllScenarioAICommand cmd = new UseSkllScenarioAICommand(skillID, (BeActor)owner, 0, hitEnd);
            cmd.Execute();

            scenrioCmd = cmd;
        }

        if (scenrioCmd != null)
        {
            return scenrioCmd.Tick(deltaTime);
        }

        return EBTStatus.BT_RUNNING;
    }

    public behaviac.EBTStatus MoveToPosition(int x, int y, int z, behaviac.EndType hitEnd, string actionName, int timeout, int pid, bool isOffset = false, int speedRate = 0)
    {
        if (scenrioCmd == null)
        {
            var targetPos = new VInt3(x / 1000f, y / 1000f, z / 1000f);
            if (isOffset)
            {
                targetPos += owner.GetPosition();
            }
            if (pid > 0)
            {
                var actor = owner.CurrentBeScene.FindActorById(pid);
                //找不到就直接结束当前的结点
                if (actor == null)
                    return behaviac.EBTStatus.BT_SUCCESS;
                
                targetPos = actor.GetPosition();
            }

            MoveToScenarioAICommand cmd = new MoveToScenarioAICommand(targetPos, (BeActor)owner, timeout, hitEnd, actionName, speedRate);
            cmd.Execute();

            scenrioCmd = cmd;
        }

        if (scenrioCmd != null)
        {
            return scenrioCmd.Tick(deltaTime);
        }

        return behaviac.EBTStatus.BT_SUCCESS;
    }

    public behaviac.EBTStatus Action_Wander(int radius, int duration, bool needSetFace, behaviac.EndType hitEnd)
    {
        if(scenrioCmd == null)
        {
            WanderScenarioAICommand cmd = new WanderScenarioAICommand(radius, needSetFace, (BeActor)owner, duration, hitEnd);
            cmd.Execute();
            scenrioCmd = cmd;
        }

        if(scenrioCmd != null)
        {
            return scenrioCmd.Tick(deltaTime);
        }

        return behaviac.EBTStatus.BT_SUCCESS;
    }

    public behaviac.EBTStatus Action_DoDead(int Delay, string ActionName, bool DonotNeedDeadShaderEffect)
    {
        if(scenrioCmd == null)
        {
            SuicideScenarioAICommand cmd = new SuicideScenarioAICommand((BeActor)owner, Delay + 1, EndType.None, ActionName, !DonotNeedDeadShaderEffect);
            cmd.Execute();

            scenrioCmd = cmd;
        }

        if (scenrioCmd != null)
        {
            return scenrioCmd.Tick(deltaTime);
        }

        return EBTStatus.BT_SUCCESS;
    }

    public behaviac.EBTStatus Action_Wait(int duration, behaviac.EndType hitEndType)
    {
         if (scenrioCmd == null)
        {
            var cmd = new ScenarioAICommand((BeActor)owner, duration, hitEndType);
            cmd.Execute();

            scenrioCmd = cmd;
        }

        if (scenrioCmd != null)
        {
            return scenrioCmd.Tick(deltaTime);
        }

        return behaviac.EBTStatus.BT_SUCCESS;
    }
}

