using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using GameClient;

public sealed class BeActorAIManager : BeAIManager {
    public int updateFindTargetAcc = 0;
    public int updateActionTimeAcc = 0;
    public int updateDestionTimeAcc = 0;
    public int updateEventTimeAcc = 0;
    public int updateFollowTimeAcc = 0;
    public int updateScenarioTimeAcc = 0;
    int lastAction = -1;
    int lastDestination = 0;
    int lastIdle = 0;
    bool run = false;
    int dazeTime = 0;
    bool dazeFlag = false;
    public bool isRunDefaultAI = true;
    private List<IBeEventHandle> handleList = new List<IBeEventHandle>();

    public bool targetSynced = false;
    
    private List<int> addEnterBattleBuff = new List<int>();
    private List<int> addOutBattleBuff = new List<int>();
        
    private List<string> scenarioPlayList = new List<string>();

    //如果剧情AI队列中有数据，修改状态开启剧情AI
    private void UpdateScenarioPlayList()
    {
        foreach (var item in scenarioPlayList)
        {
            StopCurrentCommand();
            if (owner != null)
            {
                owner.ResetMoveCmd();
            }
            
            SetScenearioAgent(item);
            break;
        }

        if (scenarioAgent != null)
        {
            state = State.RUNNING_Scenario;
        }

        scenarioPlayList.Clear();
    }
    
    
    
    //加入剧情AI队列
    public void StartScenario(string treeName)
    {
        if (!Utility.IsStringValid(treeName))
            return;

        scenarioPlayList.Add(treeName);
        
        if (owner != null && owner.isMainActor)
        {
            owner.pauseAI = false;
        }
    }

    public void StopScenario()
    {
        if (scenarioAgent != null)
            DeinitAgent(ref scenarioAgent);
        
        if (owner != null && owner.isMainActor)
        {
            owner.pauseAI = true;
        }

        if (state == State.RUNNING_Scenario)
            state = State.RUNNING_Normal;
        _OnScenarioEnd();
    }
    
    private void _OnScenarioEnd()
    {
        if(!IsRunScenario())
        {
            _AddBuffOnBattleStateChange();
        }
    }
    
    private void _AddBuffOnBattleStateChange()
    {
        BeActor actor = owner as BeActor;
        if(null == actor)
        {
            return;
        }
        _AddBuffOnBattleStateChange(actor, addEnterBattleBuff);
        _AddBuffOnBattleStateChange(actor, addOutBattleBuff);
    }
    
    private void _AddBuffOnBattleStateChange(BeActor actor, List<int> buffs)
    {
        if(addEnterBattleBuff != null && addEnterBattleBuff.Count > 0)
        {
            for(int i = 0; i < buffs.Count; i++)
            {
                actor.buffController.AddBuffInfoByID(buffs[i]);
            }
        }
    }
    
    public void AddBattleStateChangeBuff(bool isEnter, List<int> buffs)
    {
        if(isEnter)
        {
            for(int i = 0; i < buffs.Count; i++)
            {
                addEnterBattleBuff.Add(buffs[i]);
            }
        }
        else
        {
            for(int i = 0; i < buffs.Count; i++)
            {
                addOutBattleBuff.Add(buffs[i]);
            }
        }
    }
    
    public BeActorAIManager()
    {

    }

    private void RemoveHandles()
    {
        if (handleList != null)
        {
            for (int i = 0; i < handleList.Count; i++)
            {
                handleList[i].Remove();
            }
            handleList.Clear();
        }
    }

    public override void Start()
    {
        base.Start();
        
        //updateActionTimeAcc = thinkTerm
        updateActionTimeAcc = FrameRandom.InRange(0, thinkTerm);
        //updateDestionTimeAcc = changeDestinationTerm;
        updateDestionTimeAcc = FrameRandom.InRange(0, changeDestinationTerm);
        updateEventTimeAcc = eventTerm;
        updateFindTargetAcc = findTargetTerm;
        updateScenarioTimeAcc = scenarioTerm;

        addEnterBattleBuff.Clear();
        addOutBattleBuff.Clear();
        scenarioPlayList.Clear();
        
        if (isAPC)
        {
            run = true;
        }

        RemoveHandles();
        handleList.Add(owner.RegisterEventNew(BeEventType.onGrabbed, (args) => {
            if (owner.pauseAI)
                return;
            StopCurrentCommand();
            owner.SetAttackButtonState(GameClient.ButtonState.RELEASE);
            if (owner.isPkRobot)
            {
                pkRobotWander = true;
                ResetDestinationSelect();
            }
        }));


        handleList.Add(owner.RegisterEventNew(BeEventType.onHit, args =>
        {
            //handleList.Add(owner.RegisterEvent(BeEventType.onHit, (object[] args) => {

            if (owner.pauseAI)
                return;

            if (args.m_Int4 > 0)
            {
                //类似杀意波动的技能不会中断AI的连招
                int hurtID = args.m_Int4;
                var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hurtID);
                if (hurtData != null && hurtData.IsFriendDamage > 0)
                    return;
            }

            //决斗场机器人在霸体时候 连招不会被打断
            if (owner.isPkRobot && owner.stateController != null && !owner.stateController.CanBeBreakAction())
                return;

            if (owner.battleType == BattleType.Demo)
                return;
            
            StopCurrentCommand();
            if (owner.isPkRobot)
            {
                pkRobotWander = true;
                ResetDestinationSelect();
            }

            ThinkTarget(true);
            SyncTargetWithGroup();
            owner.SetAttackButtonState(GameClient.ButtonState.RELEASE);

            //if (aiTarget == null && args[2] != null)
            //{
            BeActor attacker = args.m_Obj as BeActor;
            if (attacker != null && attacker.GetCamp() != owner.GetCamp() && !attacker.IsSkillMonster())
            {
                aiTarget = attacker;
            }
            //}
        }));

        //放完技能重置寻路
        handleList.Add(owner.RegisterEventNew(BeEventType.onCastSkillFinish, param => {
            ResetDestinationSelect();
            int skillId = param.m_Int;
            SetMonsterInDaze(skillId);
        }));
    }

    public void SyncTargetWithGroup()
    {
        if (targetSynced)
        {
            return;
        }

        targetSynced = true;
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindGroup(list, owner as BeActor);

        foreach (var actor in list)
        {
            if (null == actor)
            {
                continue;
            }

            BeActorAIManager ai = actor.aiManager as BeActorAIManager;
            if (ai == null)
            {
                continue;
            }

            if (ai.targetSynced)
            {
                continue;
            }

            //if (ai.IsRunScenario())
            //{
            //    ai.SetScenario(false);
            //}
            actor.TriggerEventNew(BeEventType.onHit);

            if (ai.aiTarget != this.aiTarget)
            {
                ai.aiTarget = this.aiTarget;
                //ai.ResetAction();
                //ai.ResetDestinationSelect();
                updateActionTimeAcc = FrameRandom.InRange(0, thinkTerm);
                updateDestionTimeAcc = FrameRandom.InRange(0, changeDestinationTerm);
            }

            ai.targetSynced = true;
        }

        GamePool.ListPool<BeActor>.Release(list);
    }

    public override void Update(int deltaTime)
    {
        if (IsRunning() && currentCommand != null && currentCommand.IsAlive())
        {
            currentCommand.Tick(deltaTime);
        }

        if (isRunDefaultAI)
        {
            if (IsRunning())
            {
                if (!dazeFlag)
                {
                    lastAction = -1;
                    UpdateThinkTarget(deltaTime);
                    
                    if (!owner.IsInPassiveState() && owner.sgGetCurrentState() != (int)ActionState.AS_CASTSKILL)
                    {
                        UpdateScenarioPlayList();
                    }

                    if (IsRunScenario())
                    {
                        if (!owner.IsInPassiveState())
                        {
                            UpdateScenario(deltaTime);
                        }
                    }
                    else
                    {
                        if (!owner.IsInPassiveState() && owner.sgGetCurrentState() != (int) ActionState.AS_SKILL)
                        {
                            if (currentCommand != null && currentCommand.cmdType == AI_COMMAND.SKILL)
                                ;
                            else
                            {
                                UpdateThinkAction(deltaTime);
                                UpdateDestinationSelect(deltaTime);

                                if (forceFollow)
                                    UpdateFollow(deltaTime);
                            }
                        }

                        UpdateEvent(deltaTime);
                    }
                }
                else
                {
                    UpdateDaze(deltaTime);
                }
                UpdateTimer(deltaTime);
            }
        }
        
        if (IsRunning() && owner.sgGetCurrentState() == (int)ActionState.AS_DEAD)
        {
            Stop();
        }
    }
    public override void PostUpdate(int deltaTime)
    {
        if (IsRunning())
        {
            if (owner.IsMonster() && !isAPC)
                UpdateFaceTarget();
        }
    }
    
    void UpdateScenario(int deltaTime)
    {
        updateScenarioTimeAcc += deltaTime;
        if (updateScenarioTimeAcc >= scenarioTerm)
        {
            updateScenarioTimeAcc -= scenarioTerm;
            TickScenario(deltaTime);
        }
    }

    void TickScenario(int deltaTime)
    {
        UpdateAgent(scenarioAgent, scenarioTerm);
    }
    
    void UpdateThinkTarget(int delta)
    {
        updateFindTargetAcc += delta;
        if (updateFindTargetAcc >= findTargetTerm)
        {
            updateFindTargetAcc -= findTargetTerm;

            ThinkTarget(true);
        }
    }

    void UpdateThinkAction(int delta)
    {
        if (IsRunScenario())
            return;
        
        updateActionTimeAcc += delta;
        if (updateActionTimeAcc >= thinkTerm)
        {
            updateActionTimeAcc -= thinkTerm;

            lastAction = ThinkAction();
            if (lastAction > -1)
            {
                DoAction(aiInputData);
            }

        }
    }

    /// <summary>
    /// 更新发呆时间
    /// </summary>
    /// <param name="delta"></param>
    void UpdateDaze(int delta)
    {
        if (!dazeFlag)
            return;
        dazeTime -= delta;
        if (dazeTime <= 0)
        {
            dazeFlag = false;
        }
    }


    static readonly VInt distanceHalf = new VInt(0.5f);
    static readonly VInt distance01 = new VInt(0.1f);

    ISceneTransportDoorData transportDoor;
    VInt doorRadius;
    void UpdateDestinationSelect(int delta)
    {
        if (IsRunScenario())
            return;
        
        updateDestionTimeAcc += delta;
        if (updateDestionTimeAcc >= changeDestinationTerm)
        {
            updateDestionTimeAcc -= changeDestinationTerm;
            if (lastAction <= -1)
            {
                int res = ThinkDestination();
                if (res != -1)
                {
                    DoDestination(res, aiTarget, run);
                }
                else if (aiTarget != null)
                {
                    DoIdle();
                }

                //没有目标的情况
                if (aiTarget == null && aiType != AIType.NOATTACK)
                {
                    res = ThinkFollow();
                    if (res != -1)
                    {
                        DoDestination(res, followTarget, run);
                    }
                    else
                    {
                        if (isAutoFight)
                        {
                            if (owner.CurrentBeScene.IsBossSceneClear())
                            {

                                var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPve>();
                                if (battleUI != null)
                                    battleUI.SetAutoFight(false);

                                owner.ResetMoveCmd();

                                return;
                            }

                            if (owner.CurrentBeScene.state < BeSceneState.onClear)
                            {
                                return;
                            }
                            if (owner.CurrentBeBattle.HasFlag(BattleFlagType.SKILL_LOADING_TYPE))
                            {
                                doorRadius = distanceHalf;
                            }
                            else
                            {
                                transportDoor = owner.CurrentBeBattle.dungeonManager.GetDungeonDataManager().door;
                                doorRadius = transportDoor == null ? new VInt(1.2f) : new VInt(transportDoor.GetRegionInfo().GetRadius());

#if ROBOT_TEST
                               
#endif
                            }
                            
                            var doorPos = owner.CurrentBeScene.GetDoorPosition();
                            VInt3 doorPosSrc = doorPos;
                            bool isCloseToDoor = false;
                            if (CheckDistance(doorPos, doorRadius))
                            {
                                if (BeClientSwitch.FunctionIsOpen(ClientSwitchType.AutoFightTriggerDoor))
                                {
                                    if (owner.CurrentBeScene.CheckInDoorRange(owner))
                                    {
                                        owner.ResetMoveCmd();       //如果正处于过门区域并且具备过门条件时不重新出来再进入
                                        return;
                                    }
                                    else
                                    {
                                        isCloseToDoor = true;
                                    }
                                }
                                else
                                {
                                    isCloseToDoor = true;
                                }
                            }

                            // 有当前(WALK)指令在跑则返回
                            BeAIWalkCommand walkCommand = null;
                            if (currentCommand != null)
                                walkCommand = currentCommand as BeAIWalkCommand;

                            if (walkCommand != null && walkCommand.IsAlive()) //  && !walkCommand.stopable
                                return;

                            if (isCloseToDoor)
                            {
                                VInt3 posAroundDoor;
                                if (owner.CurrentBeBattle.HasFlag(BattleFlagType.SKILL_LOADING_TYPE))
                                {
                                    posAroundDoor = GetPosAroundDoor(owner.CurrentBeScene, doorPosSrc, VInt.one);
                                }
                                else
                                {
                                    posAroundDoor = GetPosAroundDoor(owner.CurrentBeScene, doorPosSrc, doorRadius + VInt.one);
                                }
                                doorPos.x = posAroundDoor.x;
                                doorPos.y = posAroundDoor.y;
                            }

                            BeAIWalkCommand command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                            if (owner.CurrentBeBattle.HasFlag(BattleFlagType.SKILL_LOADING_TYPE))
                            {
                                command.Init(GlobalLogic.VALUE_1000, doorPos, 0, true, false, true);
                            }
                            else
                            {
                                command.Init(GlobalLogic.VALUE_1000, doorPos, VInt.half, true, false, true);
                            }
                            

                            ExecuteCommand(command);
                            return;
                        }

                        if (idleMode == IdleMode.IDLE)
                        {
                            DoIdle();
                        }
                        else if (idleMode == IdleMode.WANDER)
                        {
                            DoDestination((int)DestinationType.WANDER, null, run);
                        }
                        else if (idleMode == IdleMode.CUSTOM)
                        {
                            DoDestination((int) customDestinationType, null, run);
                        }
                    }
                }
            }
        }
    }

    void UpdateEvent(int delta)
    {
        if (IsRunScenario())
            return;
        
        updateEventTimeAcc += delta;
        if (updateEventTimeAcc >= eventTerm)
        {
            updateEventTimeAcc -= eventTerm;

            int ret = ThinkEvent();
            if (ret != -1)
            {
                lastAction = ret;
                DoAction(aiInputData);
            }
        }
    }

    void UpdateFollow(int delta)
    {
        if (IsRunScenario())
            return;
        
        updateFollowTimeAcc += delta;
        if (updateFollowTimeAcc >= followTerm)
        {
            updateFollowTimeAcc -= followTerm;

            int res = ThinkFollow();
            if (res != -1)
            {
                Logger.LogForAI("force follow ret {0}", (DestinationType)res);
                DoDestination(res, followTarget, true);
            }
        }
    }

    void UpdateFaceTarget()
    {
        if (IsRunScenario())
            return;
        
        if (aiTarget != null && owner.stateController.CanTurnFace())
        {
            ActionState state = (ActionState)owner.sgGetCurrentState();
            if (state == ActionState.AS_WALK ||
                state == ActionState.AS_RUN ||
                state == ActionState.AS_IDLE)
            {
                if ((owner.GetPosition().x < (aiTarget.GetPosition().x + distance01)) && owner.GetFace())
                {
                    owner.SetFace(false, true);
                }
                else if ((owner.GetPosition().x > (aiTarget.GetPosition().x + distance01)) && !owner.GetFace())
                {
                    owner.SetFace(true, true);
                }
            }
        }
    }

    void ThinkTarget(bool force = false)
    {
        if (aiTarget != null && (aiTarget.IsDead() || !owner.CurrentBeScene.HasEntity(aiTarget) || !aiTarget.stateController.CanBeTargeted()))
        {
            aiTarget = null;
        }
        //攻击对象超出视野, 重新寻找
        if (aiTarget != null && !CheckDistance(aiTarget, VInt.NewVInt(chaseSight, (long)GlobalLogic.VALUE_1000)))
        {
            aiTarget = null;
        }

        if (aiTarget == null || (force && !targetUnchange))
        {
            BeActor target = FindTarget();

            if (target != null)
            {
                aiTarget = target;
            }
        }
    }

//     private static int _SortTarget(BeActor a,BeActor b)
//     {
//         var pos = owner.GetPosition();
//         int da = (pos - a.GetPosition()).magnitude;
//         int db = (pos - b.GetPosition()).magnitude;
// 
//         if (da == db)
//         {
//             return a.m_iID < b.m_iID ? -1 : 1;
//         }
//         else
//         {
//             return da < db ? -1 : 1;
//         }
//         //return Vec3.Distance(pos, a.GetPosition()) < Vec3.Distance(pos, b.GetPosition())?-1:1;
//     }


    public BeActor FindTarget()
    {
        //如果强制指定AI目标 该AI目标可能是队友
        if (assignAITarget != null && !assignAITarget.IsDeadOrRemoved())
            return assignAITarget;
        BeActor target = null;

        List<BeActor> targets = GamePool.ListPool<BeActor>.Get();

        owner.CurrentBeScene.FindTargets(targets, owner, VInt.NewVInt(sight, GlobalLogic.VALUE_1000), false, filter);
        //BattleMain.instance.Main.FindTargets(targets, owner, sight/(float)(GlobalLogic.VALUE_1000));
        if (targets.Count >= 1)
        {
            //targets.Sort((a, b) => {
            //    var pos = owner.GetPosition();
            //    int da = (pos - a.GetPosition()).magnitude;
            //    int db = (pos - b.GetPosition()).magnitude;

            //    if (da == db)
            //    {
            //        return a.m_iID < b.m_iID ? -1 : 1;
            //    }
            //    else
            //    {
            //        return da < db ? -1 : 1;
            //    }
            //    //return Vec3.Distance(pos, a.GetPosition()) < Vec3.Distance(pos, b.GetPosition())?-1:1;
            //});
            SortTarget(targets);
            if (targetType == TargetType.NEAREST)
            {
                for (int i = 0; i < targets.Count; ++i)
                {
                    if (targets[i].IsMonster() && targets[i].attribute.autoFightNeedAttackFirst)
                    {
                        target = targets[i];
                        break;
                    }
                }

                if (target == null)
                    target = targets[0];
            }

            else if (targetType == TargetType.BOSS)
            {
                for (int i = 0; i < targets.Count; ++i)
                {
                    if (targets[i].IsMonster() && targets[i].IsBoss())
                    {
                        target = targets[i];
                        break;
                        //return targets[i];
                    }
                }

                if (target == null)
                    target = targets[0];

            }
            else if (targetType == TargetType.Max_Resentment)
            {
                owner.CurrentBeScene.SortResentmentList(targets);
                target = targets[0];
            }
        }

        GamePool.ListPool<BeActor>.Release(targets);
        return target;
    }
    private void SortTarget(List<BeActor> targets)
    {
        var pos = owner.GetPosition();
        for (int i = 0; i < targets.Count; i++)
        {
            for (int j = i + 1; j < targets.Count; j++)
            {
                var a = targets[i];
                var b = targets[j];
                if (b == null || a == null) continue;
                var aDist = a.GetDistance(owner);
                var bDist = b.GetDistance(owner);
                if (aDist > bDist)
                {
                    targets[i] = b;
                    targets[j] = a;
                }
                else if(aDist == bDist)
                {
                    if(a.GetPID() > b.GetPID())
                    {
                        targets[i] = b;
                        targets[j] = a;
                    }
                }
            }
        }
    }

    int ThinkFollow()
    {
        if (followTarget == null)
            return -1;

        if (!CheckDistance(followTarget, VInt.one.i * 2))
        {
            if (FrameRandom.Range100() <= 20)
                return (int)DestinationType.BYPASS_TRACK;
            else
                return (int)DestinationType.FOLLOW;
        }

        return -1;
    }

    /*
	 返回技能id，-1表示不行动
	*/
    int ThinkAction()
    {
        if (aiTarget != null)
        {

            aiInputData = null;
            actionResult = -1;

            if (actionAgent != null)
            {
                UpdateAgent(actionAgent, thinkTerm);
            }
            /*			else if (actionTree != null)
                        {
                            UpdateTree(actionTree);
                        }*/
            //怪物没有配行为树的情况
            else
            {
                int res = 0;
                if (isAutoFight)
                    res = ThinkAction_AutoFight();
                else
                    res = ThinkAction_Monster();

                if (res > -1)
                {
                    var info = attackInfos[res];

                    actionResult = 0;
                    aiInputData = new AIInputData(info.skillID);
                }
				else {
                    actionResult = res;
                }
            }

            return actionResult;

        }

        return -1;
    }



    /*
	 * 返回行走方案,-1表示不走
	*/
    int ThinkDestination()
    {
        if (aiTarget != null || aiType == AIType.NOATTACK)
        {
            destinationSelectResult = -1;

            if (destinationSelectAgent != null)
            {
                UpdateAgent(destinationSelectAgent, changeDestinationTerm);
            }
            /*			else if (destinationSelectTree != null)
                        {
                            UpdateTree(destinationSelectTree);
                        }*/
			else {

                if (isAutoFight)
                    destinationSelectResult = ThinkDestination_AutoFight();
                else
                    destinationSelectResult = ThinkDestination_Monster();
            }

            if (aiTarget != null && targetUnchange && owner.GetEntityData().isMonster)
            {
                destinationSelectResult = (int)DestinationType.GO_TO_TARGET;
            }

            //决斗场机器人徘徊
            if (owner.isPkRobot && pkRobotWander)
            {
                destinationSelectResult = (int)DestinationType.WANDER_PKROBOT;
                pkRobotWander = false;
            }

            return destinationSelectResult;
        }

        return -1;
    }

    bool CanAttackTarget(int passvieProb)
    {
        if (aiTarget != null && aiTarget.IsPassiveState() && FrameRandom.InRange(0, GlobalLogic.VALUE_100) >= passvieProb)
        {
            return false;
        }

        return true;
    }

    #region THINK AUTO FIGHT
    int ThinkAction_AutoFight()
    {
        int index = (int)AIChoise.CANNOT_ATTACK;

        for (int i = 0; i < attackInfos.Count; ++i)
        {
            var info = attackInfos[i];
            if (info.enable && info.IsPointInRange(owner.GetPosition2(), aiTarget.GetPosition2(), owner.GetFace()))
            {
				if (FrameRandom.InRange(0, GlobalLogic.VALUE_100) < info.prob && CanUseSkill(info.skillID)) {
                    index = i;
                    break;
                }
            }
        }

        return index;
    }


    int ThinkDestination_AutoFight()
    {
        return (int)DestinationType.GO_TO_TARGET;
    }

    #endregion

    #region THINK MONSTER
    int ThinkAction_Monster()
    {
        int index = (int)AIChoise.CANNOT_ATTACK;

        bool inAttackRange = false;
        //能攻击，但是不攻击
        if (FrameRandom.InRange(0, GlobalLogic.VALUE_100) > warlike)
        {
            index = (int)AIChoise.CANATTACK_BUTCHOOSENOT;
        }
		else {

            //从近到远判定攻击范围
            for (int i = 0; i < attackInfos.Count; ++i)
            {
                var info = attackInfos[i];
                if (info.enable && info.IsPointInRange(owner.GetPosition2(), aiTarget.GetPosition2(), owner.GetFace()))
                {
                    inAttackRange = true;
					if (FrameRandom.InRange(0, GlobalLogic.VALUE_100) < info.prob && CanUseSkill(info.skillID) && CanAttackTarget(info.attackPassiveProb)) {
                        index = i;
                        break;
                    }
                }
            }
        }

        //如果在攻击范围，但是没选择攻击
        if (inAttackRange && index == (int)AIChoise.CANNOT_ATTACK)
            index = (int)AIChoise.CANATTACK_BUTCHOOSENOT;

        return index;
    }


    int GetRandAmong(int[] candicats)
    {
        int sum = 0;
        for (int i = 0; i < candicats.Length; ++i)
            sum += candicats[i];
        int rand = FrameRandom.InRange(0, sum);

        int tmp = 0;
        for (int i = 0; i < candicats.Length; ++i)
        {
            tmp += candicats[i];
            if (rand <= tmp)
                return i;
        }

        return -1;
    }

    static readonly VInt distanceOne = new VInt(1.0f);
    static readonly VInt distanceOneHalf = new VInt(1.5f);
    int ThinkDestination_Monster()
    {
        int ret = -1;
        //能攻击，但是选择不攻击
        if (lastAction == (int)AIChoise.CANATTACK_BUTCHOOSENOT)
        {
            if (aiType == AIType.MELEE)
            {
                if (CheckDistance(aiTarget, distanceOneHalf, 1))
                {
                    int index = GetRandAmong(new int[] { idleRand, escapeRand, wanderRand, GlobalLogic.VALUE_50 });
                    if (index == 0)
                        ;
                    else if (index == 1)
                        ret = (int)DestinationType.ESCAPE;
                    else if (index == 2)
                        ret = (int)DestinationType.WANDER;
                    else
                        ret = (int)DestinationType.GO_TO_TARGET;
                }
                //近战距离1单位外不后退(idle或go to target)
				else {
                    int index = GetRandAmong(new int[] { idleRand, GlobalLogic.VALUE_50 });
                    if (index == 0)
                        ;
                    else if (index == 1)
                        ret = (int)DestinationType.GO_TO_TARGET;
                }
            }
            else if (aiType == AIType.RANGED)
            {
                if (CheckDistance(aiTarget, keepDistance, 1) || CheckDistance(aiTarget, distanceOneHalf, 1))
                {
                    int index = GetRandAmong(new int[] { idleRand, escapeRand, wanderRand, yFirstRand, GlobalLogic.VALUE_30 });
                    if (index == 0)
                        ;
                    else if (index == 1)
                        ret = (int)DestinationType.ESCAPE;
                    else if (index == 2)
                        ret = (int)DestinationType.WANDER;
                    else if (index == 3)
                        ret = (int)DestinationType.Y_FIRST;
                    else
                        ret = (int)DestinationType.GO_TO_TARGET;
                }
                else if (CheckDistance(aiTarget, distanceOne.i * 2, 1))
                {
                    int index = GetRandAmong(new int[] { idleRand, wanderRand, GlobalLogic.VALUE_30 });
                    if (index == 0)
                        ;
                    else if (index == 1)
                        ret = (int)DestinationType.WANDER;
                    else if (index == 2)
                        ret = (int)DestinationType.GO_TO_TARGET;
                }
				else {
                    int index = GetRandAmong(new int[] { idleRand, GlobalLogic.VALUE_50 });
                    if (index == 0)
                        ;
                    else if (index == 1)
                        ret = (int)DestinationType.GO_TO_TARGET;
                }
                //Logger.LogErrorFormat("CANATTACK_BUTCHOOSENOT:{0}", (DestinationType)ret);
            }
        }
        else if (lastAction == (int)AIChoise.CANNOT_ATTACK)
        {
            if (aiType == AIType.RANGED)
            {
                if (CheckDistance(aiTarget, keepDistance, 1))
                {
                    if (CheckDistance(aiTarget, distanceOneHalf, 1))
                        ret = FrameRandom.Range100() < 70 ? (int)DestinationType.KEEP_DISTANCE : (int)DestinationType.ESCAPE;
                    else
                        ret = FrameRandom.Range100() < 30 ? (int)DestinationType.KEEP_DISTANCE : (int)DestinationType.ESCAPE;
                }
                else
                {
                    ret = FrameRandom.Range100() < 50 ? (int)DestinationType.FOLLOW : (int)DestinationType.Y_FIRST;
                }

            }
            else if (aiType == AIType.MELEE)
            {
                ret = (int)DestinationType.GO_TO_TARGET;
            }

            //Logger.LogErrorFormat("CANNOT_ATTACK:{0}", (DestinationType)ret);
        }

        //最大连续移动cmd，最大连续idle cmd
        if (ret != -1)
        {
            lastDestination++;
            lastIdle = 0;

            if (lastDestination > maxWalkCount)
            {
                ret = -1;
                lastDestination = 0;
            }
        }
		else {
            lastIdle++;

            if (lastIdle > maxIdleCount)
            {
                ret = (int)DestinationType.ESCAPE;
                lastIdle = 0;
            }

            lastDestination = 0;
        }

        //Logger.LogErrorFormat("lastIdle={0}", lastIdle);

        return ret;
    }
    #endregion

    /*
	 返回技能id，-1表示不行动，判断是否在某个event
	*/
    int ThinkEvent()
    {
        actionResult = -1;
        if (eventAgent != null)
        {
            UpdateAgent(eventAgent, eventTerm);
            return actionResult;
        }
        /*		else if (eventTree != null)
                {
                    UpdateTree(eventTree);
                    return actionResult;
                }*/

        return -1;
    }
    public void DoAction(AIInputData inputData)
    {
        Logger.LogForAI("USE SKILL {0}", inputData.inputs[0].skillID);
        lastDestination = 0;
        lastIdle = 0;

        BeAISkillCommand command = null;
        //command = new BeAISkillCommand(owner, inputData);
        command = (BeAISkillCommand)BeAICommandPool.GetAICommand(AI_COMMAND.SKILL, owner);
        command.Init(inputData);

#if UNITY_EDITOR
        var table = TableManager.instance.GetTableItem<ProtoTable.SkillTable>(inputData.inputs[0].skillID);
        if (table != null)
        {
            command.SetDebugInfo("Skill: " + table.Name + "ID:" + inputData.inputs[0].skillID);
        }
#endif

        ExecuteCommand(command);
    }

    public void DoDestination(int method, BeEntity target, bool run = false)
    {
        var toPos = VInt3.zero;
        if (target != null)
            toPos = target.GetPosition();
        DoDestination(method, toPos, run);
    }
    
    public void DoDestination(int method, VInt3 toPos, bool run = false)
    {
        if (isAPC && run && aiTarget != null)
        {
            if (CheckDistanceWithX(aiTarget, distanceOne.i * 2))
                run = false;
        }
        VInt tolerance = VInt.one;

        if (owner != null && owner.IsProcessRecord())
        {
            owner.GetRecordServer().RecordProcess("[AI]PID:{0}-{1} ENTER Do Destination:{2} {3}", owner.m_iID, owner.GetName(), (DestinationType)method, owner.GetInfo());
            owner.GetRecordServer().Mark(0x8779785, owner.GetEntityRecordAttribute(), ((DestinationType)method).ToString(), owner.GetName());
            // Mark:0x8779785 [AI]PID:{0}-{13} ENTER Do Destination:{12} Pos:({1},{2},{3}),Speed:({4},{5},{6}),Face:{7},Hp:{8},Mp:{9},Flag:{10},curSkillId:{11}
        }

        int duration = GlobalLogic.VALUE_10000;
        BeAIWalkCommand command = null;

        if(destinationTypeTest != DestinationType.IDLE)
        {
            method = (int)destinationTypeTest;
        }

        if (method != -1)
        {
            DestinationType dt = (DestinationType)method;
            switch (dt)
            {
                case DestinationType.GO_TO_TARGET:
                    Logger.LogForAI("DoDestination GO_TO_TARGET");

                    if (!CheckDistance(aiTarget, VInt.Float2VIntValue(1.1f)))
                    {
                        VInt3 pos = toPos;
                        if (!owner.isMainActor)
                        {
                            pos.x += (pos.x - owner.GetPosition().x) > 0 ? -overlapOffset : overlapOffset;
                            int rx = FrameRandom.InRange(5 * (int)IntMath.kIntDen / 10, 10 * (int)IntMath.kIntDen / 10);
                            int ry = FrameRandom.InRange(5 * (int)IntMath.kIntDen / 10, 10 * (int)IntMath.kIntDen / 10);
                            if (FrameRandom.Range100() > 50)
                                pos.x += rx;
                            else
                                pos.x -= rx;
                            if (FrameRandom.Range100() > 50)
                                pos.y += ry;
                            else
                                pos.y -= ry;
                        }
                        Logger.LogForAI("GO_TO_TARGET offset:({0},{1}) targetPos:({2},{3})", pos.x - toPos.x, pos.y - toPos.y, pos.x, pos.y);

                        if (owner != null && owner.IsProcessRecord())
                        {
                            owner.GetRecordServer().RecordProcess("[AI]PID:{0}-{1} Do Destination {2} ({3},{4}) targetpos:({5},{6}) selfpos:({7},{8}) {9}", owner.m_iID, owner.GetName(), dt, pos.x, pos.y, toPos.x,toPos.y, owner.GetPosition().x, owner.GetPosition().y, owner.GetInfo());
                            owner.GetRecordServer().Mark(0x8779890, new int[]
                          {
                                owner.m_iID, pos.x, pos.y,toPos.x, toPos.y,toPos.z,
                                owner.GetPosition().x,
                                owner.GetPosition().y,owner.GetPosition().z,owner.moveXSpeed.i, owner.moveYSpeed.i, owner.moveZSpeed.i,
                                (owner.GetFace() ? 0 : 1), owner.attribute.GetHP(),owner.attribute.GetMP(), owner.GetAllStatTag(),
                                owner.attribute.battleData.attack
                          }, owner.GetName(), dt.ToString());

                            // Mark:0x8779890 [AI]PID:{0}-{17} Do Destination {18} ({1},{2}) targetpos:({3},{4},{5}) selfpos:({6},{7},{8}) speed:({9},{10},{11}) face:{12} hp:{13} mp:{14} tag:{15} attack:{16}
                        }

                        tolerance = VInt.one.i / 2;
                        command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                        command.Init(duration, pos, tolerance, run, false, true);
                        //command = new BeAIWalkCommand(owner,duration, pos, tolerance, run, false, true);

                    }
                    else
                    {
                        Logger.LogForAI("GO_TO_TARGET in distance!!!");
                        if (owner != null && owner.IsProcessRecord())
                        {
                            owner.GetRecordServer().RecordProcess("[AI]PID:{0}-{1} Do Destination {2} within(1.1f) targetpos:({3},{4}) selfpos:({5},{6}) {7}", owner.m_iID, owner.GetName(), dt, toPos.x, toPos.y, owner.GetPosition().x, owner.GetPosition().y, owner.GetInfo());
                            owner.GetRecordServer().Mark(0x877978F, new int[]
                           {
                                owner.m_iID, toPos.x, toPos.y,toPos.z, owner.GetPosition().x,
                                owner.GetPosition().y,owner.GetPosition().z,owner.moveXSpeed.i, owner.moveYSpeed.i, owner.moveZSpeed.i,
                                (owner.GetFace() ? 0 : 1), owner.attribute.GetHP(),owner.attribute.GetMP(), owner.GetAllStatTag(),
                                owner.attribute.battleData.attack
                           }, owner.GetName(), dt.ToString());
                            // Mark:0x877978F [AI]PID:{0}-{15} Do Destination {16} within(1.1f) targetpos:({1},{2},{3}) selfpos:({4},{5},{6}) speed:({7},{8},{9}) face:{10} hp:{11} mp:{12} tag:{13} attack:{14}
                        }
                    }

                    break;

                case DestinationType.BYPASS_TRACK:
                    Logger.LogForAI("DoDestination BYPASS_TRACK");
                    VInt3 tPos = toPos;
                    if (owner != null && owner.IsProcessRecord())
                    {
                        owner.GetRecordServer().RecordProcess("[AI]PID:{0}-{1} Do Destination {2} ({3},{4}) selfpos:({5},{6})", owner.m_iID, owner.GetName(), dt, tPos.x, tPos.y, owner.GetPosition().x, owner.GetPosition().y);
                        owner.GetRecordServer().Mark(0x877978E, new int[]
                       {
                            owner.m_iID,
                           tPos.x, tPos.y, owner.GetPosition().x, owner.GetPosition().y
                       }, owner.GetName(), dt.ToString());
                        // Mark:0x877978E [AI]PID:{0}-{5} Do Destination {6} ({1},{2}) selfpos:({3},{4})
                    }
                    tolerance = VInt.one;
                    //command = new BeAIWalkCommand(owner, duration, target.GetPosition(), tolerance, run, false, false, true);
                    command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                    command.Init(duration, tPos, tolerance, run, false, false, true);

                    break;

                case DestinationType.WANDER:
                    Logger.LogForAI("DoDestination WANDER");
                    VInt3 WanderPosition = GetWanderPosition();
				if (owner != null && owner.IsProcessRecord()) {
                        owner.GetRecordServer().RecordProcess("[AI]PID:{0}-{1} Do Destination {2} ({3},{4}) selfpos:({5},{6})", owner.m_iID, owner.GetName(), dt, WanderPosition.x, WanderPosition.y, owner.GetPosition().x, owner.GetPosition().y);
                        owner.GetRecordServer().Mark(0x877978D, new int[]
                        {
                            owner.m_iID,
                           WanderPosition.x, WanderPosition.y, owner.GetPosition().x, owner.GetPosition().y
                        }, owner.GetName(), dt.ToString());
                        // Mark:0x877978D [AI]PID:{0}-{5} Do Destination {6} ({1},{2}) selfpos:({3},{4})
                    }
                    //tolerance = 1.0f;
                    //command = new BeAIWalkCommand(owner, duration, GetWanderPosition(), tolerance, run, false, true);	
                    tolerance = VInt.one.i / 2;
                    command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                    command.Init(duration, WanderPosition, tolerance, run, false, true, false);

                    break;
                case DestinationType.ESCAPE:
                    Logger.LogForAI("DoDestination ESCAPE");
                    VInt3 WalkBackPostion = GetWalkBackPostion();
				if (owner != null && owner.IsProcessRecord()) {
                        owner.GetRecordServer().RecordProcess("[AI]PID:{0}-{1} Do Destination {2} ({3},{4}) selfpos:({5},{6})", owner.m_iID, owner.GetName(), dt, WalkBackPostion.x, WalkBackPostion.y, owner.GetPosition().x, owner.GetPosition().y);
                        owner.GetRecordServer().Mark(0x877978B, new int[]
                        {
                            owner.m_iID,
                           WalkBackPostion.x, WalkBackPostion.y, owner.GetPosition().x, owner.GetPosition().y
                        }, owner.GetName(), dt.ToString());
                        // Mark:0x877978B [AI]PID:{0}-{5} Do Destination {6} ({1},{2}) selfpos:({3},{4})
                    }
                    //command = new BeAIWalkCommand(owner, duration, GetWalkBackPostion(), tolerance, run, false, false);
                    command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                    command.Init(duration, WalkBackPostion, tolerance, run, false, false);

                    break;
                case DestinationType.FOLLOW:
                    #region FOLLOW
                    Logger.LogForAI("DoDestination FOLLOW");
                    {
                        var curPos = owner.GetPosition();
                        var targetPos = toPos;
                        curPos.x += (targetPos.x - curPos.x) > 0 ? -overlapOffset : overlapOffset;
                        var dis = 2 * IntMath.kIntDen;

                        bool flag = false;
                        if (Mathf.Abs(curPos.x - targetPos.x) > dis)
                        {
                            VFactor rand = new VFactor(FrameRandom.InRange(0, 11), 10);

                            flag = true;
                            if (targetPos.x > curPos.x)
                                curPos.x = targetPos.x - (int)dis * rand;
                            else
                                curPos.x = targetPos.x + (int)dis * rand;

                            curPos.y = targetPos.y;
                        }

                        if (Mathf.Abs(curPos.y - targetPos.y) > dis)
                        {
                            VFactor rand = new VFactor(FrameRandom.InRange(0, 11), 10);
                            flag = true;
                            if (targetPos.y > curPos.y)
                                curPos.y = targetPos.y - (int)dis * rand;
                            else
                                curPos.y = targetPos.y + (int)dis * rand;

                            curPos.x = targetPos.x;
                        }


                        if (flag)
                        {

                            if (owner != null && owner.IsProcessRecord())
                            {
                                VInt3 pos = curPos;
                                owner.GetRecordServer().RecordProcess("[AI]PID:{0}-{1} Do Destination {2} ({3},{4}) ", owner.m_iID, owner.GetName(), dt, pos.x, pos.y);
                                owner.GetRecordServer().Mark(0x877978C, new int[]
                              {
                                    owner.m_iID,
                                    pos.x, pos.y
                              }, owner.GetName(), dt.ToString());
                                // Mark:0x877978C [AI]PID:{0}-{3} Do Destination {4} ({1},{2})
                            }

                            //Logger.LogForAI("({0},{1}) --> ({2},{3})", owner.GetPostion().x, owner.GetPostion().y, curPos.x, curPos.y);

                            tolerance = VInt.one.i * 2;
                            //command = new BeAIWalkCommand(owner, 5000, curPos, tolerance, run, false, true);
                            command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                            command.Init(GlobalLogic.VALUE_5000, curPos, tolerance, run, false, true);
                        }
                        else
                        {
                            if (owner != null && owner.IsProcessRecord())
                            {

                                owner.GetRecordServer().RecordProcess("[AI]PID:{0}-{1} Do Destination {2} no need follow ", owner.m_iID, owner.GetName(), dt);
                                owner.GetRecordServer().Mark(0x877978A, new int[]
                                {
                                    owner.m_iID,
                                }, owner.GetName(), dt.ToString());
                                // Mark:0x877978A [AI]PID:{0}-{1} Do Destination {2} no need follow
                            }
                        }

                    }
                    #endregion
                    break;
                case DestinationType.Y_FIRST:
                    Logger.LogForAI("DoDestination Y_FIRST");
                    {
                        if (attackInfos.Count <= 0)
                            break;

                        var info = attackInfos[0];

                        var curPos = owner.GetPosition();
                        var targetPos = aiTarget.GetPosition();


                        //x在范围内，y不在范围内
                        if (Mathf.Abs(curPos.x - targetPos.x) < info.front)
                        {
                            Logger.LogForAI("x in range y not");
                            int randV = (FrameRandom.InRange(0, (int)IntMath.kIntDen)) * (FrameRandom.Range100() > 50 ? (int)IntMath.kIntDen : -(int)IntMath.kIntDen);
                            curPos.y = targetPos.y + randV;
                        }
                        else
                        {
                            Logger.LogForAI("x not in range y not");

                            if (targetPos.x > curPos.x)
                                curPos.x = curPos.x + info.front.i + FrameRandom.InRange(0, 2 * (int)IntMath.kIntDen);
                            else
                                curPos.x = curPos.x - info.front.i - FrameRandom.InRange(0, 2 * (int)IntMath.kIntDen);

                            curPos.y = targetPos.y;
                        }

                        Logger.LogForAI("from ({0},{1},{2}) to ({3},{4},{5})", owner.GetPosition().x, owner.GetPosition().y, owner.GetPosition().z,
                            curPos.x, curPos.y, curPos.z);

                        if (owner != null && owner.IsProcessRecord())
                        {
                            VInt3 pos = curPos;
                            owner.GetRecordServer().RecordProcess("[AI]PID:{0}-{1} Do Destination {2} ({3},{4}) selfpos:({5},{6})", owner.m_iID, owner.GetName(), dt, pos.x, pos.y, owner.GetPosition().x, owner.GetPosition().y);
                            owner.GetRecordServer().Mark(0x8779989, new int[]
                            {
                                owner.m_iID,
                                pos.x, pos.y,
                                owner.GetPosition().x, owner.GetPosition().y
                            }, owner.GetName(), dt.ToString());
                            // Mark:0x8779989 [AI]PID:{0}-{5} Do Destination {6} ({1},{2}) selfpos:({3},{4})
                        }

                        //command = new BeAIWalkCommand(owner, 5000, curPos, tolerance, run, false, true);
                        command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                        command.Init(GlobalLogic.VALUE_5000, curPos, tolerance, run, false, true);
                    }
                    break;
                case DestinationType.KEEP_DISTANCE:

                    Logger.LogForAI("DoDestination KEEP_DISTANCE");
                    {
                        var curPos = owner.GetPosition();
                        var targetPos = aiTarget.GetPosition();
                        if (Mathf.Abs(curPos.x - targetPos.x) < keepDistance)
                        {
                            bool moveLeft = targetPos.x > curPos.x;

                            if (moveLeft && !CanWalk(MoveDir.LEFT) && Mathf.Abs(curPos.x - targetPos.x) < VInt.one.i * 2)
                                moveLeft = false;
                            if (!moveLeft && !CanWalk(MoveDir.RIGHT) && Mathf.Abs(curPos.x - targetPos.x) < VInt.one.i * 2)
                                moveLeft = true;

                            Logger.LogForAI("keep distance move:{0}", moveLeft ? "LEFT" : "RIGHT");

                            curPos.x = moveLeft ? (targetPos.x - keepDistance.i) : (targetPos.x + keepDistance.i);


                            if (owner != null && owner.IsProcessRecord())
                            {
                                VInt3 pos = curPos;
                                owner.GetRecordServer().RecordProcess("[AI]PID:{0}-{1} Do Destination {2} ({3},{4})", owner.m_iID, owner.GetName(), dt, pos.x, pos.y);
                                owner.GetRecordServer().Mark(0x8779789, new int[]
                               {
                                    owner.m_iID,
                                    pos.x, pos.y
                               }, owner.GetName(), dt.ToString());
                                // Mark:0x8779789 [AI]PID:{0}-{3} Do Destination {4} ({1},{2})
                            }

                            //command = new BeAIWalkCommand(owner, 5000, curPos, tolerance, run, false, false);
                            command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                            command.Init(GlobalLogic.VALUE_5000, curPos, tolerance, run, false, false);
                        }
                    }

                    break;
                case DestinationType.FINAL_DOOR:
                    {
                        var pos = owner.CurrentBeScene.GetDoorPosition();
                        if (pos == VInt3.zero)
                        {
                            StopCurrentCommand();
                            //Stop();
                            break;
                        }

                        //command = new BeAIWalkCommand(owner, 10000, pos, tolerance, run, false, false);
                        command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                        command.Init(GlobalLogic.VALUE_10000, pos, tolerance, run, false, false);
                    }
                    break;
                case DestinationType.WANDER_IN_CIRCLE:
                    {
                        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
                        owner.CurrentBeScene.FindMonsterByID(list, monsterID);
                        if (list.Count > 0)
                        {
                            BeActor monster = list[0];
                            var pos = GetRandomPos(monster.GetPosition(), VInt.Float2VIntValue(radius / 1000.0f));
                            command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                            command.Init(duration, pos, VInt.one.i / 5, run, false, true);
                        }
                        GamePool.ListPool<BeActor>.Release(list);
                    }
                    break;
                case DestinationType.WANDER_PKROBOT:
                    Logger.LogForAI("DoDestination WANDER_PKROBOT");
                    VInt3 pKRobotWanderPos = GetPkRobotWanderPos();
                    if (owner != null && owner.IsProcessRecord())
                    {
                        owner.GetRecordServer().RecordProcess("[AI]PID:{0}-{1} Do Destination {2} ({3},{4}) selfpos:({5},{6})", owner.m_iID, owner.GetName(), dt, pKRobotWanderPos.x, pKRobotWanderPos.y, owner.GetPosition().x, owner.GetPosition().y);
                        owner.GetRecordServer().Mark(0x779786, new int[]
                      {
                            owner.m_iID,
                            pKRobotWanderPos.x, pKRobotWanderPos.y,
                            owner.GetPosition().x, owner.GetPosition().y
                      }, owner.GetName(), dt.ToString());
                        // Mark:0x779786 [AI]PID:{0}-{5} Do Destination {6} ({1},{2}) selfpos:({3},{4})
                    }

                    tolerance = VInt.one.i / 2;
                    command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                    command.Init(duration, pKRobotWanderPos, tolerance, true, false, true, true);

                    break;
                case DestinationType.MOVETO_LEFT_SCENEEDGE:
                    {
                        VInt3 targetPos = owner.GetPosition();
                        targetPos.x -= GlobalLogic.VALUE_10000;

                        tolerance = VInt.one.i / 2;
                        command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                        command.Init(duration, targetPos, tolerance, run, false, true);
                    }
                    break;
                case DestinationType.GO_TO_TARGET2:
                    {
                        VInt3 targetPos = toPos;

                        if(!CheckDistance(aiTarget, VInt.Float2VIntValue(0.1f)))
                        {
                            tolerance = VInt.one.i / 2;
                            command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                            command.Init(duration, targetPos, tolerance, run, false, true);
                        }
                    }
                    break;
                case DestinationType.GO_TO_TARGET3:
                {
                    VInt3 targetPos = toPos;
                        command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                        command.Init(duration, targetPos, VInt.one.i / 4, run, false, true);
                    }
                    break;
                case DestinationType.GO_TO_TARGET_KEEP_DISTANCE:
                    {
                        // TODO 暂时随机，看效果考虑用格子避免重叠
                        VInt3 pos = toPos;
  
                        int rx = FrameRandom.InRange(5 * (int)IntMath.kIntDen / 10, 10 * (int)IntMath.kIntDen / 10);
                        int ry = FrameRandom.InRange(5 * (int)IntMath.kIntDen / 10, 10 * (int)IntMath.kIntDen / 10);
                        if (FrameRandom.Range100() > 50)
                            pos.x += rx;
                        else
                            pos.x -= rx;
                        if (FrameRandom.Range100() > 50)
                            pos.y += ry;
                        else
                            pos.y -= ry;

                        tolerance = VInt.one.i / 10;
                        command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                        command.Init(duration, pos, tolerance, run, false, true);
                    }
                    break;
                case DestinationType.WANDER_IN_OWNER_CIRCLE:
                    {
                        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
                        owner.CurrentBeScene.FindMonsterByID(list, monsterID, false);
                        for (int i = 0; i < list.Count; i++)
                        {
                            BeActor monster = list[i];
                            if (monster.IsSameTopOwner(owner))
                            {
                                var pos = GetRandomPosInCircle(monster.GetPosition(), VInt.Float2VIntValue(radius / 1000.0f));
                                if (owner.CurrentBeScene.IsInBlockPlayer(pos))
                                {
                                    pos = BeAIManager.FindStandPositionNew(pos, owner.CurrentBeScene);
                                }
                                command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                                command.Init(duration, pos, VInt.one.i / 5, run, false, true);    
                                break;
                            }
                        }
                        GamePool.ListPool<BeActor>.Release(list);
                    }
                    break;
                case DestinationType.WANDER_BY_OWNER:
                    {
                        VInt3 pos = GetWanderByOwnerPosition();
                        tolerance = VInt.one.i / 4;
                        command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                        command.Init(duration, pos, tolerance, false, false, true);
                    }
                    break;
                case DestinationType.WANDER_BY_TARGET:
                    {
                        VInt3 pos = GetWanderByTargetPosition();
                        tolerance = VInt.one.i / 4;
                        command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                        command.Init(duration, pos, tolerance, false, false, true);
                    }
                    break;
                case DestinationType.GO_TO_TARGET_DIRECTLY:
                    {
                        VInt3 pos = GetGoToTargetPosition(aiTarget);
                        tolerance = VInt.one.i / 4;
                        command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                        command.Init(duration, pos, tolerance, false, false, true);
                    }
                    break;
                case DestinationType.RUNAWAY:
                    {
                        VInt3 pos = GetRunAwayPosition(aiTarget, ref run);
                        tolerance = VInt.one.i / 4;
                        command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                        command.Init(duration, pos, tolerance, false, false, true);
                    }
                    break;
                case DestinationType.W_MOVE:
                    {
                        VInt3 pos = GetMoveWPosition(aiTarget);
                        tolerance = VInt.one.i / 4;
                        command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                        command.Init(duration, pos, tolerance, false, false, false, false, true);
                    }
                    break;
                case DestinationType.Z_MOVE:
                    {
                        VInt3 pos = GetZigZagPosition(aiTarget);
                        tolerance = VInt.one.i / 4;
                        command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                        command.Init(duration, pos, tolerance, false, false, true);
                    }
                    break;
                case DestinationType.KEEP_DISTANCE_TABLE:
                    {
                        VInt3 pos = GetKeepDistancePosition(aiTarget);
                        tolerance = VInt.one.i / 4;
                        command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                        command.Init(duration, pos, tolerance, false, false, true);
                    }
                    break;
                case DestinationType.AVOID_FRONT_FACE:
                    {
                        VInt3 pos = GetAvoidFrontFacePosition(aiTarget);
                        tolerance = VInt.one.i / 4;
                        command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                        command.Init(duration, pos, tolerance, false, false, true);
                    }
                    break;
                case DestinationType.CHASE_ROUNDABOUT:
                    {
                        VInt3 pos = GetChaseRoundAboutPosition(aiTarget);
                        tolerance = VInt.one.i / 4;
                        command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                        command.Init(duration, pos, tolerance, false, false, true);
                    }
                    break;
                case DestinationType.CHASE_BACK_DIRECTLY:
                    {
                        VInt3 pos = GetChaseBackPosition(aiTarget);
                        tolerance = VInt.one.i / 4;
                        command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
                        command.Init(duration, pos, tolerance, false, false, true);
                    }
                    break;
            }

            if (command != null)
            {
                BeAIWalkCommand walkCommand = command as BeAIWalkCommand;
                if (walkCommand != null)
                    walkCommand.destinationType = dt;
#if UNITY_EDITOR
                command.SetDebugInfo("DoDestination: " + dt.GetDescription());
#endif
            }
        }

        if (command != null)
        {

            BeAIWalkCommand walkCommand = currentCommand as BeAIWalkCommand;
            //在自动战斗或者W字移动的情况下，且本次和上次移动的目标位置相同，则直接返回。
            if (walkCommand != null && walkCommand.IsAlive() && walkCommand.destinationType == (DestinationType)method &&
                ((isAutoFight || (command as BeAIWalkCommand).moveW) ? walkCommand.targetPos == (command as BeAIWalkCommand).targetPos : true))
            {
                BeAICommandPool.PutAICommand(command);
                return;
            }

            //若目标位置过近，也直接返回
            if(CheckDistance(command.targetPos , skIntCheckDis))
            {
                BeAICommandPool.PutAICommand(command);
                return;
            }

            ExecuteCommand(command);
        }
    }


    public override void ResetThinkTarget()
    {
        updateFindTargetAcc = findTargetTerm + 1;
    }
    public override void ResetAction()
    {
        updateActionTimeAcc = thinkTerm + 1;
    }
    public override void ResetDestinationSelect()
    {
        updateDestionTimeAcc = changeDestinationTerm + 1;
    }
    public override void ResetScenarioSelect()
    {
        updateScenarioTimeAcc = scenarioTerm + 1;
    }

    public void DoIdle()
    {
        Logger.LogForAI("Do Idle");

        int idleDur = FrameRandom.InRange((uint)(idleDuration / 2), (uint)(idleDuration * 2));

        if (owner != null && owner.IsProcessRecord())
        {
            owner.GetRecordServer().RecordProcess("[AI]PID:{0}-{1} DoIdle", owner.m_iID, owner.GetName());
            owner.GetRecordServer().Mark(0x8779787, new int[] { owner.m_iID }, owner.GetName());
            // Mark:0x8779787 [AI]PID:{0}-{1} DoIdle
        }

        BeAIIdleCommand command = (BeAIIdleCommand)BeAICommandPool.GetAICommand(AI_COMMAND.IDLE, owner);
        command.Init(idleDur);

        ExecuteCommand(command);
    }

    public override void SetForceFollow(bool flag)
    {
        if (forceFollow == flag)
            return;

        forceFollow = flag;

        Logger.LogForAI("Set froce follow {0}", flag);

        if (forceFollow)
        {
            updateFollowTimeAcc = followTerm;
        }
        else
        {

        }
    }

    // 怪物释放技能后发呆一段时间
    private void SetMonsterInDaze(int skillId)
    {
        if (!owner.IsMonster())
            return;
        var unitData = owner.GetEntityData().monsterData;
        if (unitData.DazeTime == null || unitData.DazeTimeLength <= 0)
            return;
        int time = GetDazeTime(unitData, skillId);
        if (time <= 0)
            return;
        SetDazeTime(time);
    }

    //怪物释放完技能后设置发呆时间
    private void SetDazeTime(int time)
    {
        dazeFlag = true;
        dazeTime = time;
        updateFindTargetAcc += time;
        updateActionTimeAcc += time;
        updateDestionTimeAcc += time;
        updateEventTimeAcc += time;
    }

    // 从怪物表获取发呆时间
    private int GetDazeTime(ProtoTable.UnitTable unitData, int skillId)
    {
        for (int i = 0; i < unitData.DazeTimeLength; i++)
        {
            ProtoTable.UnionCell uCell = unitData.DazeTimeArray(i);
            if (uCell.fixInitValue == skillId)
                return uCell.fixLevelGrow;
        }
        return -1;
    }
}
