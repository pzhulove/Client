using System.Collections;
using System.Collections.Generic;
// using UnityEngine;
using behaviac;
using Tenmove.Runtime.Math;
public class ScenarioAICommand
{
    protected int duration;
    protected behaviac.EndType hitEnd;

    protected bool hasInterruptted;

    int timeAcc;
    bool startCount;
    int totalTime;

    IBeEventHandle handleOnHit;

  //  protected BTAgent agent;

    protected BeActor owner;
    protected BeActorAIManager ai; 

    protected bool needEnd;
    public bool NeedEnd
    {
        set
        {
            needEnd = value;
        }
        get {
            return needEnd;
        }
    }

    public void Reset()
    {
        duration = 0;
        hitEnd = EndType.None;
        hasInterruptted = false;
        handleOnHit = null;
        timeAcc = 0;
        startCount = false;
        totalTime = 0;
      //  agent = null;
        needEnd = false;

        OnReset();
    }

    public ScenarioAICommand(BeActor owner, int duration = 0, behaviac.EndType hitEnd = behaviac.EndType.None)
    {
        this.owner = owner;
        this.duration = duration;
        this.hitEnd = hitEnd;

        if (owner != null)
        {
            this.ai = owner.aiManager as BeActorAIManager;
            //if (this.ai != null)
            //    this.ai.SetLevelAIScenario(true);
        }
            

    }


    protected void StartCount(int tTime)
    {
        startCount = true;
        timeAcc = 0;

        totalTime = tTime;
    }

    protected behaviac.EBTStatus UpdateCount(int deltaTime)
    {
        timeAcc += deltaTime;
        if (timeAcc >= totalTime)
        {
            startCount = false;
            End();
            return EBTStatus.BT_SUCCESS;
        }

        return EBTStatus.BT_RUNNING;
    }

    public behaviac.EBTStatus Tick(int deltaTime)
    {
        if (needEnd)
        {
            needEnd = false;
            End();
             //Logger.LogErrorFormat("execute needend {0}", Time.realtimeSinceStartup);
            return EBTStatus.BT_SUCCESS;
        }

        if (hasInterruptted)
        {
            End();
            if (hitEnd == EndType.Node)
            {
                return EBTStatus.BT_SUCCESS;
            }
            else if (hitEnd == EndType.Tree)
            {
                //return agent.Action_EndScenario();
                var ai =  owner.aiManager as BeActorAIManager;
                if (ai != null)
                {
                    ai.StopScenario();
                }
            }
        }

        if (startCount)
        {
            var ret = OnTick(deltaTime);

            if (ret == EBTStatus.BT_RUNNING)
                return UpdateCount(deltaTime);
            else
                return ret;

        }

        return OnTick(deltaTime);
    }

    public void Execute()
    {
        if (duration > 0)
        {
            StartCount(duration);
        }

        if (hitEnd != EndType.None)
        {
            if (owner != null)
            {
                handleOnHit = owner.RegisterEventNew(BeEventType.onHit, eventParam =>
                {
                    hasInterruptted = true;
                });
            }
        }

        OnExecute();
    }

    public void End()
    {
        OnEnd();

        if (hitEnd != EndType.None)
        {
            if (handleOnHit != null)
            {
                handleOnHit.Remove();
                handleOnHit = null;
            }
        }
        
        var ai = owner.aiManager;
        if (ai != null)
        {
            ai.scenrioCmd = null;
        }
    }

    protected virtual behaviac.EBTStatus OnTick(int deltaTime)
    {
        return EBTStatus.BT_RUNNING;
    }

    protected virtual void OnExecute() { }

    protected virtual void OnEnd() { }

    protected virtual void OnFinish() { }

    protected virtual void OnReset() { }
}
public class MoveToScenarioAICommand : ScenarioAICommand
{
    VInt3 targetPos;
    string actionName;
    string backupName;

    VInt3 oldWalkSpeed;
    bool walkSpeedChanged;
    
    bool oldPauseAI;

    public MoveToScenarioAICommand(VInt3 targetPos, BeActor owner, int duration = 0, behaviac.EndType hitEnd = behaviac.EndType.None, string actionName = "", int speedRate = 0) :
        base(owner, duration, hitEnd)
    {
        
        Logger.LogProcessFormat("[AI]MoveToScenarioAICommand actor:{0} targetPos:{1}", owner.GetName(), targetPos.ToString());
        this.targetPos = targetPos;
        this.actionName = actionName;
        walkSpeedChanged = false;

        if (global::Utility.IsStringValid(actionName))
        {
            if (owner != null)
            {
                owner.ReplaceActionName(ActionType.ActionType_WALK, actionName);
            }
        }

        ReplaceWalkSpeed(speedRate);
        ReplacePauseAI();
    }

    private void ReplaceWalkSpeed(int speedRate)
    {
        if(speedRate != 0 && owner != null)
        {
            walkSpeedChanged = true;
            oldWalkSpeed = owner.walkSpeed;
            VFactor speed = VFactor.NewVFactor(speedRate, GlobalLogic.VALUE_100);
            owner.walkSpeed = new VInt3(oldWalkSpeed.x * speed, oldWalkSpeed.y * speed, oldWalkSpeed.z * speed);
        }
    }
    private void RestoreWalkSpeed()
    {
        if(walkSpeedChanged && owner != null)
        {
            walkSpeedChanged = false;
            owner.walkSpeed = oldWalkSpeed;
        }
    }

    private void ReplacePauseAI()
    {
        oldPauseAI = owner.pauseAI;
        owner.pauseAI = false;
    }
    private void RestorePauseAI()
    {
        owner.pauseAI = oldPauseAI;
    }
    protected override EBTStatus OnTick(int deltaTime)
    {
        if (owner != null && owner.aiManager != null)
        {
            var aiManager = (BeActorAIManager)owner.aiManager;
            int distance = VInt.one.i / Global.Settings.tolerate;
            VInt3 pos = owner.GetPosition();

            if (((TMath.Abs(targetPos.x - pos.x)) <= distance) && (TMath.Abs(targetPos.y - pos.y) <= distance))
            {
                End();
                return EBTStatus.BT_SUCCESS;
            }

            if (aiManager.currentCmd == null || aiManager.currentCmd.cmdType != AI_COMMAND.WALK)
            {
                aiManager.FireWalkCommand(targetPos, GlobalLogic.VALUE_100000);

                return EBTStatus.BT_RUNNING;
            }
        }

        return EBTStatus.BT_RUNNING;
    }

    protected override void OnEnd()
    {
        if (owner == null)
            return;

        owner.RestoreActionName(ActionType.ActionType_WALK);
        
        RestoreWalkSpeed();
        RestorePauseAI();

        var aiManager = (BeActorAIManager)owner.aiManager;
        if (aiManager != null && aiManager.IsRunning())
        {
            aiManager.DoIdle();
        }
    }

    protected override void OnReset()
    {
        targetPos = VInt3.zero;
        actionName = "";
        backupName = "";
    }
}

public class SuicideScenarioAICommand : ScenarioAICommand
{
    string actionName;
    string backupName;
    bool needDeadShaderEffect = true;

    public SuicideScenarioAICommand(BeActor owner, int duration = 0, behaviac.EndType hitEnd = behaviac.EndType.None, string actionName = "",bool needDeadShaderEffect = true) : base(owner, duration, hitEnd)
    {
        Logger.LogProcessFormat("[AI]SuicideScenarioAICommand actor:{0} actionName:{1}", owner.GetName(), actionName);
        this.actionName = actionName;
        if (global::Utility.IsStringValid(actionName))
        {
            if (owner != null)
            {
               owner.ReplaceActionName(ActionType.ActionType_DEAD, actionName);
            }
        }
        this.needDeadShaderEffect = needDeadShaderEffect;
    }

    protected override void OnEnd()
    {
        var actor = owner;
        if (actor != null)
        {
            /*BeMonster monster = actor as BeMonster;
            if (monster != null)
            {
                monster.needDeadShaderEffect = needDeadShaderEffect;
            }*/
            /*actor.DoDead();
            //if (actor.IsBoss())
            {
                actor.TriggerEvent(BeEventType.onBeKilled, new EventParam() { entity_0 = actor });
            }*/

            actor.DoHurt(actor.GetEntityData().GetHP());
            actor.deadType = DeadType.NORMAL;

            Logger.LogProcessFormat("[AI]SuicideScenarioAICommand OnEnd actor:{0}", owner.GetName());
        }
    }

    protected override void OnReset()
    {
        
    }
}

public class WanderScenarioAICommand : ScenarioAICommand
{
    VInt3 orgPos = VInt3.zero;
    int cmdRadius;
    bool cmdNeedSetFace;
    int timeIdle = 0;
    VInt3 lastTargetPos = VInt3.one;
    int timeCmd;

    public WanderScenarioAICommand(int radius, bool needSetFace, BeActor owner, int duration = 0, behaviac.EndType hitEnd = behaviac.EndType.None) : base(owner, duration, hitEnd)
    {
        Logger.LogProcessFormat("[AI], WanderScenarioAICommand actor:{0}", owner.GetName());
        this.cmdRadius = radius;
        this.cmdNeedSetFace = needSetFace;
        this.timeCmd = 0;

        if (!needSetFace)
        {
         //   ++agent.cntDonotSetFace;
        }

        BeActor actor = owner;
        if (actor != null)
        {
            orgPos = actor.GetPosition();
        }
    }

    protected override EBTStatus OnTick(int deltaTime)
    {
        BeActor actor = owner;
        if (actor == null)
        {
            return EBTStatus.BT_RUNNING;
        }

        var aiManager = (BeActorAIManager)owner.aiManager;
        if (aiManager == null)
        {
            return EBTStatus.BT_RUNNING;
        }

        this.timeIdle -= deltaTime;
        this.timeCmd -= deltaTime;

        if (aiManager.currentCmd == null || aiManager.currentCmd.cmdType != AI_COMMAND.WALK || (aiManager.currentCmd.cmdType == AI_COMMAND.WALK && !aiManager.currentCmd.IsAlive()))
        {
            if (this.cmdNeedSetFace)
            {
                if (timeIdle <= 0)
                {
                    if (aiManager.FrameRandom.Range100() > 50)
                    {
                        timeIdle = aiManager.FrameRandom.InRange(GlobalLogic.VALUE_500, GlobalLogic.VALUE_1000);
                        return EBTStatus.BT_RUNNING;
                    }
                }
                else
                {
                    if (aiManager.FrameRandom.Range100() > 95)
                    {
                        actor.SetFace(!actor.GetFace());
                    }
                    return EBTStatus.BT_RUNNING;
                }
            }

            if (this.timeCmd > 0)
            {
                return EBTStatus.BT_RUNNING;
            }

            int tarPosX = aiManager.FrameRandom.InRange(orgPos.x - this.cmdRadius, orgPos.x + this.cmdRadius);
            int tarPosY = aiManager.FrameRandom.InRange(orgPos.y - this.cmdRadius, orgPos.y + this.cmdRadius);
            tarPosX = TMath.Clamp(tarPosX, actor.CurrentBeScene.logicXSize.x, actor.CurrentBeScene.logicXSize.y);
            tarPosY = TMath.Clamp(tarPosY, actor.CurrentBeScene.logicZSize.x, actor.CurrentBeScene.logicZSize.y);
            VInt3 targetPos = new VInt3(tarPosX, tarPosY, 0);

            this.timeCmd = GlobalLogic.VALUE_500;

            lastTargetPos = targetPos;
            aiManager.FireWalkCommand(targetPos, GlobalLogic.VALUE_3000);
            return EBTStatus.BT_RUNNING;
        }

        return EBTStatus.BT_RUNNING;
    }

    protected override void OnEnd()
    {
        if (!this.cmdNeedSetFace)
        {
           // --agent.cntDonotSetFace;
        }

        var aiManager = (BeActorAIManager)owner.aiManager;
        if (aiManager == null)
        {
            return;
        }

        aiManager.DoIdle();
    }
}

public class UseSkllScenarioAICommand : ScenarioAICommand
{
    GameClient.BeEvent.BeEventHandleNew handle1;
    GameClient.BeEvent.BeEventHandleNew handle2;
    int skillID;
    bool eventReceived;
    public UseSkllScenarioAICommand(int skillID, BeActor owner, int duration = 0, behaviac.EndType hitEnd = behaviac.EndType.None) : base(owner, duration, hitEnd)
    {
        Logger.LogProcessFormat("[AI]UseSkllScenarioAICommand actor:{0} skillid:{1}", owner.GetName(), skillID);
        this.skillID = skillID;
    }

    protected override void OnExecute()
    {
        var actor = owner;

        if (!actor.IsCastingSkill() || actor.IsCastingSkill() && actor.GetCurSkillID() != skillID)
        {
            bool success = actor.UseSkill(skillID, true);

            if (success)
            {
                handle1 = actor.RegisterEventNew(BeEventType.onCastSkillFinish, eventParam =>
                {
                    int skillid = eventParam.m_Int;
                    if (skillid == skillID)
                    {
                        eventReceived = true;
                    }
                });

                handle2 = actor.RegisterEventNew(BeEventType.onSkillCancel, eventParam =>
                {
                    int skillid = eventParam.m_Int;
                    if (skillid == skillID)
                    {
                        eventReceived = true;
                    }
                });
            }
            else
            {
                eventReceived = true;
            }
            
        }
    }

    protected override void OnEnd()
    {
        if (handle1 != null)
        {
            handle1.Remove();
            handle1 = null;
        }

        if (handle2 != null)
        {
            handle2.Remove();
            handle2 = null;
        }
    }

    protected override EBTStatus OnTick(int deltaTime)
    {
        if (eventReceived)
        {
            End();
            return EBTStatus.BT_SUCCESS;
        }


        return EBTStatus.BT_RUNNING;
    }
}

public class PlayActionScenarioAICommand : ScenarioAICommand
{
    public string actionName;
    protected int actionDuration;
    public PlayActionScenarioAICommand(string actionName, BeActor owner, int actionDuration = 0, int duration = 0, behaviac.EndType hitEnd = behaviac.EndType.None) : base(owner, duration, hitEnd)
    {
        Logger.LogProcessFormat("[AI]PlayActionScenarioAICommand actor:{0} actionDuration:{1} duration:{2}", owner.GetName(), actionDuration, duration);
        this.actionName = actionName;
        if (actionDuration == 0)
            actionDuration = duration;
        this.actionDuration = actionDuration;
    }

    protected override void OnExecute()
    {
        var aiManager = owner.aiManager;
        if (aiManager != null)
        {
            if (aiManager.owner != null)
            {
                aiManager.owner.GetStateGraph().ResetCurrentStateTime(true);
                aiManager.owner.sgSetCurrentStatesTimeout(actionDuration, true);

                //List<string> actions = new List<string>();
                //actions.Add(actionName);
                //aiManager.owner.PlayQueuedAction(actions);
                
                if(!string.IsNullOrEmpty(actionName))
                {
                    aiManager.owner.PlayAction(actionName);
                }
            }
            
           
        }
    }
}

public class ShowDialogScenarioAICommand : ScenarioAICommand
{
    int dialogId;
    public ShowDialogScenarioAICommand(int dialogId, BeActor owner, int duration = 0, behaviac.EndType hitEnd = behaviac.EndType.None) : base(owner, duration, hitEnd)
    {
        this.dialogId = dialogId;
    }

    protected override void OnExecute()
    {
        var battle = owner.CurrentBeBattle;
        if (battle != null && !battle.isDialogFrameOpen)
        {
            battle.OpenDialog(dialogId);
        }
    }

    
    protected override EBTStatus OnTick(int deltaTime)
    {
        var battle = owner.CurrentBeBattle;
        if (battle != null)
        {
            if (!battle.isDialogFrameOpen)
            {
                End();
                return EBTStatus.BT_SUCCESS;
            }

            return EBTStatus.BT_RUNNING;
        }
        return EBTStatus.BT_SUCCESS;
    }
}