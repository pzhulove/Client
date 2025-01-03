using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using GameClient;
using System;
using System.Reflection;

public sealed class BeActorStateGraph : BeStatesGraph
{
    public BeActor pkActor;
    public int m_iStateData;
    public int m_iPreState;
    public VInt m_vMoveSpeed;
    public int m_iCurrentMoveState;

    public override void InitStatesGraph()
    {
        BeStates kBirthState = new BeStates(
            (int)ActionState.AS_BIRTH,
            (int)AStateTag.AST_BUSY,
            (BeStates state) =>
            {
                if (pkActor.HasAction(pkActor.GetActionNameByType(ActionType.ActionType_BIRTH)))
                {
                    pkActor.PlayAction(ActionType.ActionType_BIRTH);
                    SetCurrentStatesTimeout(pkActor.GetCurrentActionDuration());
                }
                else
                {
                    SetCurrentStatesTimeout(GlobalLogic.VALUE_10);
                }
            },
            (BeStates pkState) =>
            {
                SwitchStates(new BeStateData((int)ActionState.AS_IDLE));
            }
        );

        AddStates2Graph(kBirthState);

        //-----------------------------------------------------
        BeStates kIdleState = new BeStates(
            (int)ActionState.AS_IDLE,
            (int)AStateTag.AST_NULLTAG,
            (BeStates state) =>
            {
                pkActor.clickZSpeed = -VInt.Float2VIntValue(Global.Settings.clickForce);
                if ((pkActor.GetPosition().z > 0 && !pkActor.stateController.IgnoreGravity())
                    || pkActor.moveZSpeed != 0)
                {
                    //pkActor.delayCaller.DelayCall(10, ()=>{
                    SwitchStates(new BeStateData((int)ActionState.AS_BUSY));
                    //});

                }
                else
                {

                    //在这里判断是否要进入死亡状态
                    if (pkActor.IsDead())
                    {
                        pkActor.DoDead();
                        return;
                    }

                    BeActorStateGraph pkGraph = (BeActorStateGraph)state.pkGraph;
                    BeActor pkActor2 = pkGraph.pkActor;

                    ActionType actionType = ActionType.ActionType_IDLE;

                    if (pkActor2.isMainActor && pkActor2.CurrentBeScene.IsEnemyClear(pkActor2) && !pkActor2.GetEntityData().isSummonMonster)
                    {
                        actionType = ActionType.ActionType_SpecialIdle;
                    }
                    if (pkActor.buffController != null)
                    {
                        BeBuff buff = pkActor.buffController.HasBuffByID(BattleMain.IsModePvP(pkActor.battleType) ? 162210 : 162209);
                        if (buff != null && !buff.state.IsDead())
                            actionType = ActionType.ActionType_SpecialIdle02;
                    }
                    if ((pkGraph.m_lastState == (int)ActionState.AS_RUN) &&
                        pkActor2.HasAction(ActionType.ActionType_EndWalk) && 
                        (pkActor2.m_cpkCurEntityActionInfo != null && 
                        pkActor2.m_cpkCurEntityActionInfo.moveName != pkActor2.GetActionNameByType(ActionType.ActionType_EndWalk)))
                    {
                        var time = pkActor2.PlayAction(ActionType.ActionType_EndWalk);
                        pkActor2.delayCaller.DelayCall(time, () =>
                        {
                            if (pkActor2.sgGetCurrentState() == (int)ActionState.AS_IDLE)
                                pkActor2.PlayAction(actionType);
                        });
                    }
                    else
                        pkActor2.PlayAction(actionType);


                    pkActor.SetTag((int)AState.ACS_FALL | (int)AState.ACS_JUMP | (int)AState.AST_FALLGROUND | (int)AState.ACS_JUMPBACK, false);
                    pkActor2.ClearMoveSpeed();

                    //pkActor.protectManager.ClearProtect();
                    pkActor.ResetWeight();

                    if (pkActor.isFloating)
                    {
                        pkActor.RestoreFloating();
                    }

                    //如果是眩晕状态，则恢复
                    if (pkActor.stateController.HasBuffState(BeBuffStateType.STUN))
                    {
                        var buff = pkActor.buffController.GetBuffByType(BuffType.STUN);
                        if (buff != null)
                        {
                            if (buff.buffData.TargetState.Length > 0)
                            {
                                //pkActor.Locomote(new BeStateData((int)buff.buffData.TargetState[0], 0, 0, 0, 0, 0, buff.GetLeftTime(), true));
                                pkActor.Locomote(new BeStateData(buff.buffData.TargetState[0]) { _timeout = buff.GetLeftTime(), _timeoutForce = true });
                            }
                            else
                            {
                                Logger.LogErrorFormat("Actor pid {0} buffId {1} can not switch to Stun state", pkActor.GetPID(), buff.buffData.ID);
                            }
                            pkActor.protectManager.ClearStandProtect();
                        }
                    }
                }
            }
        );

        SGAddEventHandler2States(kIdleState, new BeEventsHandler(
            (int)EventCommand.EVENT_COMMAND_CHANGE,
            (BeStates pkState) =>
            {
                if (pkActor.moveXSpeed != 0 || pkActor.moveYSpeed != 0)
                {
                    if (pkActor.HasAction(ActionType.ActionType_WALK) || pkActor.HasAction(ActionType.ActionType_RUN))
                    {
                        if (pkActor.isMainActor && !pkActor.hasDoublePress)
                            pkActor.ChangeRunMode(true);
                        SwitchStates(new BeStateData((int)ActionState.AS_RUN));
                    }
                }
            }
        ));

        AddStates2Graph(kIdleState);

        //-----------------------------------------------------
        BeStates kBusyState = new BeStates(
            (int)ActionState.AS_BUSY,
            (int)AStateTag.AST_BUSY,
            (BeStates state) =>
            {
                if (pkActor.moveZSpeed > 0)
                {
                    if (pkActor.HasTag((int)AState.ACS_FALL))
                    {
                        if (pkActor.GetPosition().z >= 8 * VInt.one.i)
                            pkActor.m_cpkCurEntityActionInfo = null;
                        if (Global.Settings.useNewHurtAction)
                        {
                            if (pkActor.HasTag((int)AState.AST_FALLGROUND))
                                pkActor.PlayAction(ActionType.ActionType_FALL_UP);
                        }
                    }
                    else
                        pkActor.PlayAction(ActionType.ActionType_RISE);//跳跃上
                }
                else
                {
                    if (pkActor.HasTag((int)AState.ACS_FALL))
                    {
                        if (pkActor.GetPosition().z >= 8 * VInt.one.i)
                            pkActor.m_cpkCurEntityActionInfo = null;
                    }
                    else
                        pkActor.PlayAction(ActionType.ActionType_SINK);//跳跃下
                }
            },
            null,
            null,
            (BeStates state, int t) =>
            {
                if (pkActor.changeToNoBlock)
                {
                    pkActor.changeToNoBlock = false;

                    if (!pkActor.stateController.HasAbility(BeAbilityType.BLOCK))
                    {
                        pkActor.stateController.SetAbilityEnable(BeAbilityType.BLOCK, true);
                    }
                }
            }
        );

        SGAddEventHandler2States(kBusyState,
            new BeEventsHandler(
                (int)EventCommand.EVENT_COMMAND_UPSTOP,
                (BeStates pkState) =>
                {
                    if (pkActor.HasTag((int)AState.ACS_FALL))
                    {
                        pkActor.geLastTopZ = pkActor.GetPosition().fz;
                        if (Global.Settings.useNewHurtAction)
                        {
                            if (pkActor.HasTag((int)AState.AST_FALLGROUND))
                                pkActor.PlayAction(ActionType.ActionType_FALL_DOWN);
                        }
                    }
                    else
                    {
                        if (!pkActor.HasTag((int)AState.ACS_JUMP))
                            pkActor.SetTag((int)AState.ACS_JUMP, true);
                        pkActor.PlayAction(ActionType.ActionType_SINK);
                    }

                }
            )
        );

        SGAddEventHandler2States(
            kBusyState,
            new BeEventsHandler(
                (int)EventCommand.EVENT_COMMAND_TOUCHGROUND,
                (BeStates state) =>
                {
                    bool bFall = pkActor.HasTag((int)AState.ACS_FALL);
                    bool bJump = pkActor.HasTag((int)AState.ACS_JUMP);
                    bool bGround = pkActor.HasTag((int)AState.AST_FALLGROUND);

                    if (bFall)
                    {
                        if (bGround)
                            SwitchStates(new BeStateData((int)ActionState.AS_FALLGROUND));
                        else
                            SwitchStates(new BeStateData((int)ActionState.AS_FALLCLICK));
                    }
                    else if (bJump)
                    {
                        pkActor.SetTag((int)AState.ACS_JUMP, false);
                        int aniTime = pkActor.PlayAction(ActionType.ActionType_JumpDown);
                        pkActor.stateController.SetAbilityEnable(BeAbilityType.MOVE, false);
                        pkActor.moveXSpeed = 0;
                        pkActor.moveYSpeed = 0;

                        pkActor.delayCaller.DelayCall(aniTime, () =>
                            {
                                pkActor.stateController.SetAbilityEnable(BeAbilityType.MOVE, true);
                                LocomoteState();
                            });
                    }
                    else
                    {
                        LocomoteState();
                    }

                }
            )
        );

        AddStates2Graph(kBusyState);

        //-----------------------------------------------------
        BeStates kRunState = new BeStates(
            (int)ActionState.AS_RUN,
            (int)AStateTag.AST_NULLTAG,
            (BeStates pkState) =>
            {
                pkActor.buffController.TriggerBuffs(BuffCondition.START_RUN, null);

                int delayTime = 0;
                if (pkActor.HasAction(ActionType.ActionType_StartWalk))
                {
                    delayTime = pkActor.PlayAction(ActionType.ActionType_StartWalk);
                }
                
                if (delayTime > 0)
                {
                    pkActor.stateController.SetAbilityEnable(BeAbilityType.MOVE, false);
                    pkActor.delayCaller.DelayCall(delayTime, () =>
                    {
                        pkActor.stateController.SetAbilityEnable(BeAbilityType.MOVE, true);

                        if (pkActor.sgGetCurrentState() == (int)ActionState.AS_RUN)
                            pkActor.RefreshMoveSpeed();
                    });
                }
                else
                {
                    pkActor.RefreshMoveSpeed();
                }
            },
            null,
            null,
            (BeStates pkState, int iNewState) =>
            {
                if (iNewState != (int)ActionState.AS_JUMP)
                    pkActor.ChangeRunMode(false);

                if (pkActor.doublePressRun)
                {
                    pkActor.doublePressRun = false;
                    int buffID = 35;
                    pkActor.buffController.RemoveBuff(buffID);
                }
            }
        );

        SGAddEventHandler2States(kRunState, new BeEventsHandler(
            (int)EventCommand.EVENT_COMMAND_CHANGE,
            (BeStates pkState) =>
            {
                if (pkActor.moveXSpeed == 0 && pkActor.moveYSpeed == 0)
                {
                    pkActor.ChangeRunMode(false);
                    SwitchStates(new BeStateData((int)ActionState.AS_IDLE));
                }
                else
                {
                    int iCur = (int)(pkActor.IsRunning() ? pkActor.runAction : pkActor.walkAction);
                    if (m_iCurrentMoveState != iCur)
                    {
                        m_iCurrentMoveState = iCur;

                        float factor = pkActor.IsRunning() ? pkActor.runSpeedFactor : pkActor.walkSpeedFactor;

                        pkActor.PlayAction((ActionType)m_iCurrentMoveState, factor);
                    }
                }
            }
        ));

        AddStates2Graph(kRunState);


        //-----------------------------------------------------
        BeStates kJumpState = new BeStates(
            (int)ActionState.AS_JUMP,
            (int)AStateTag.AST_NULLTAG,
            (BeStates pkState) =>
            {
                pkActor.SetMoveSpeedZ(VInt.Float2VIntValue(Global.Settings.jumpForce));
                pkActor.SetTag((int)AState.ACS_JUMP, true);

                int aniTime = pkActor.PlayAction(ActionType.ActionType_JumpUp);
                pkActor.stateController.SetAbilityEnable(BeAbilityType.MOVE, false);
                pkActor.moveYSpeed = 0;

                pkActor.delayCaller.DelayCall(aniTime, () =>
                    {
                        pkActor.stateController.SetAbilityEnable(BeAbilityType.MOVE, true);
                        SwitchStates(new BeStateData((int)ActionState.AS_BUSY));
                    });

            }

        );
        AddStates2Graph(kJumpState);

        //-----------------------------------------------------
        BeStates kJumpBackState = new BeStates(
            (int)ActionState.AS_JUMPBACK,
            (int)AStateTag.AST_BUSY | (int)AStateTag.AST_CONTROLED,
            (BeStates pkState) =>
            {
                pkActor.SetMoveSpeedZ(VInt.NewVInt(TableManager.instance.gst.jumpBackSpeed[1], (long)GlobalLogic.VALUE_1000));
                pkActor.SetMoveSpeedXLocal(VInt.NewVInt(TableManager.instance.gst.jumpBackSpeed[0], (long)GlobalLogic.VALUE_1000));
                pkActor.SetMoveSpeedY(0);
                m_uiCurrentStatesTime = 0;
                pkActor.PlayAction(ActionType.ActionType_DOWN);
                pkActor.SetTag((int)AState.ACS_JUMPBACK, true);
            }
        );

        SGAddEventHandler2States(kJumpBackState,
            new BeEventsHandler(
                (int)EventCommand.EVENT_COMMAND_TOUCHGROUND,
                (BeStates pkState) =>
                {
                    LocomoteState();
                }
            )
        );

        AddStates2Graph(kJumpBackState);

        //-----------------------------------------------------
        // 浮空
        BeStates kFallState = new BeStates(
            (int)ActionState.AS_FALL,
            (int)AStateTag.AST_BUSY | (int)AStateTag.AST_CONTROLED,
            (BeStates pkState) =>
            {
                if (pkActor.isFloating)
                {
                    pkActor.RemoveFloating();
                }

                if (pkActor.HasTag((int)AState.AST_FALLGROUND))
                {
                    pkActor.forceX = pkActor.forceX.i * VFactor.NewVFactorF(Global.Settings.fallgroundHitFactor, GlobalLogic.VALUE_1000);
                    pkActor.forceY = pkActor.forceY.i * VFactor.NewVFactorF(Global.Settings.fallgroundHitFactor, GlobalLogic.VALUE_1000);
                }
                else
                {
                    if (pkActor.fallForGrab)
                    {
                        pkActor.PlayQueuedAction(new List<ActionDesc> {
                            new ActionDesc() { actionType = ActionType.ActionType_HURT, timeout = 90 },
                            new ActionDesc() { actionType = ActionType.ActionType_HURT1, timeout = 60 },
                        });
                    }
                    else
                    {
                        pkActor.PlayQueuedAction(new List<ActionDesc> {
                            new ActionDesc() { actionType = ActionType.ActionType_HURT, timeout = 90 },
                            new ActionDesc() { actionType = ActionType.ActionType_HURT1, timeout = 60 },
                            new ActionDesc() { actionType = ActionType.ActionType_FALL_UP, timeout = 0 },
                        });
                    }
                }
                if (pkActor.forceY == 0)
                {
                    pkActor.forceY = VInt.Float2VIntValue(0.01f);
                }

                pkActor.SetMoveSpeedX(pkActor.forceX);
                pkActor.SetMoveSpeedZ(pkActor.forceY);
                pkActor.SetMoveSpeedY(0);
                pkActor.SetTag((int)AState.ACS_FALL, true);
                pkActor.SetTag((int)AState.ACS_JUMP, false);
                pkActor.SetTag((int)AState.ACS_JUMPBACK, false);

                //pkActor.delayCaller.DelayCall(10, ()=>{
                SwitchStates(new BeStateData((int)ActionState.AS_BUSY));
                //});

                pkActor.protectManager.StartFallHurtCount();
                pkActor.protectManager.ClearStandProtect();
            },
            null,
            null,
            (BeStates state, int t) =>
            {
            }
        );

        AddStates2Graph(kFallState);

        //-----------------------------------------------------
        //落地弹跳

        BeStates kFallClickState = new BeStates(
            (int)ActionState.AS_FALLCLICK,
            (int)AStateTag.AST_BUSY | (int)AStateTag.AST_CONTROLED,
            (BeStates pkState) =>
            {
                pkActor.ClearMoveSpeed((int)SpeedCear.SPEEDCEAR_Y | (int)SpeedCear.SPEEDCEAR_Z);
                
                //VInt[] clickForceArr = new VInt[1];
                //clickForceArr[0] = -pkActor.clickZSpeed.i * VFactor.NewVFactor(50, 100);
                //pkActor.TriggerEvent(BeEventType.onChangeClickForce, new object[] { clickForceArr });

                var param = pkActor.TriggerEventNew(BeEventType.onChangeClickForce, new EventParam(){ m_Vint = -pkActor.clickZSpeed.i * VFactor.NewVFactor(50, 100) });
                pkActor.SetMoveSpeedZ(param.m_Vint);

                m_uiCurrentStatesTime = 0;

                if (!pkActor.HasTag((int)AState.AST_FALLGROUND))
                    pkActor.PlayAction(ActionType.ActionType_FALL_UP);

                pkActor.SetTag((int)AState.AST_FALLGROUND, true);
                pkActor.protectManager.StartGroundHurtCount();  
                pkActor.protectManager.ClearStandProtect();

#if !LOGIC_SERVER
                if (pkActor.geLastTopZ > Global.Settings.highFallHight)
                {
                    pkActor.CurrentBeScene.currentGeScene.GetCamera().PlayShockEffect(Global.Settings.playerHighFallTouchGroundShockData);
                }
#endif

                //设置五秒超时
                SetCurrentStatesTimeout(GlobalLogic.VALUE_5000);

#if !LOGIC_SERVER

                if (pkActor != null && pkActor.CurrentBeBattle != null)
                    pkActor.CurrentBeBattle.PlaySound(1000);

#endif

            }          
        );

        SGAddEventHandler2States(kFallClickState,
            new BeEventsHandler(
                (int)EventCommand.EVENT_COMMAND_UPSTOP,
                (BeStates pkState) =>
                {
                    if (!pkActor.HasTag((int)AState.AST_FALLGROUND))
                        pkActor.PlayAction(ActionType.ActionType_FALL_DOWN);
                }
            ));

        SGAddEventHandler2States(kFallClickState,
            new BeEventsHandler(
                (int)EventCommand.EVENT_COMMAND_TOUCHGROUND,
                (BeStates pkState) =>
                {
                    SwitchStates(new BeStateData((int)ActionState.AS_FALLGROUND));
                }
            ));
        AddStates2Graph(kFallClickState);

        //-----------------------------------------------------
        //落地
        BeStates kFallGroundState = new BeStates(
            (int)ActionState.AS_FALLGROUND,
            (int)AStateTag.AST_BUSY | (int)AStateTag.AST_CONTROLED,
            //进入攻击状态
            (BeStates pkState) =>
            {
                pkActor.ClearMoveSpeed();

                pkActor.PlayAction(ActionType.ActionType_FALL_GROUND);
                SetCurrentStatesTimeout(pkActor.GetCurrentActionDuration());

                pkActor.SetTag((int)AState.AST_FALLGROUND, true);
                pkActor.protectManager.StartGroundHurtCount();
                pkActor.protectManager.ClearStandProtect();


#if !LOGIC_SERVER
                if (BattleMain.IsModePvP(pkActor.battleType))
                {
                    //蹲伏CD相关
                    if (pkActor.isLocalActor)
                    {
                        if (pkActor.HasSkill(Global.DUNFU_SKILL_ID))
                        {
                            int leftTime = pkActor.GetSkill(Global.DUNFU_SKILL_ID).CDLeftTime;
                            if (leftTime > 0)
                            {
                                pkActor.StartSpellBar(eDungeonCharactorBar.DunFuCD, leftTime, false, "", true);
                            }
                            else
                            {
                                if (pkActor.CanUseSkill(Global.DUNFU_SKILL_ID))
                                    pkActor.StartSpellBar(eDungeonCharactorBar.DunFuCD, 0, false, "", true);
                            }
                        }
                    }
                }
#endif

            },
            //timeout
            (BeStates pkState) =>
            {
                if (pkActor.CanRoll())
                {
                    SwitchStates(new BeStateData((int)ActionState.AS_ROLL));
                }
                else if (!pkActor.stateController.HasBuffState(BeBuffStateType.SLEEP))
                {
                    SwitchStates(new BeStateData((int)ActionState.AS_GETUP));
                }
            },
              (BeStates pkState, int t) =>
              {
                  if (m_uiCurrentStatesTime >= 180 && pkActor.CanUseDunfu())
                  {
                      SwitchStates(new BeStateData((int)ActionState.AS_GETUP));
                  }
              },
            (BeStates pkState, int t) =>
            {

#if !LOGIC_SERVER
                if (BattleMain.IsModePvP(pkActor.battleType))
                {
                    if (pkActor.isLocalActor)
                    {
                        pkActor.StopSpellBar(eDungeonCharactorBar.DunFuCD);
                    }
                }
#endif

            }
        );

        AddStates2Graph(kFallGroundState);

        BeStates kRollState = new BeStates(
            (int)ActionState.AS_ROLL,
            (int)AStateTag.AST_BUSY | (int)AStateTag.AST_CONTROLED,
            (BeStates pkState) =>
            {
                pkActor.ClearMoveSpeed();
                pkActor.SetMoveSpeedXLocal(VInt.Float2VIntValue(Global.Settings.rollSpeed.x));
                pkActor.SetMoveSpeedY(VInt.Float2VIntValue(Global.Settings.rollSpeed.y));

                if (pkActor.HasAction(ActionType.ActionType_Roll))
                {
                    pkActor.PlayAction(ActionType.ActionType_Roll);

                    int dur = pkActor.GetCurrentActionDuration();

                    SetCurrentStatesTimeout(dur);

                    pkActor.buffController.TryAddBuff(32, dur - GlobalLogic.VALUE_100);
                }
                else if (pkActor.HasAction(ActionType.ActionType_WALK))
                {
                    pkActor.PlayAction(ActionType.ActionType_WALK);

                    int dur = GlobalLogic.VALUE_1000;


                    SetCurrentStatesTimeout(dur);
                    pkActor.buffController.TryAddBuff(32, dur - GlobalLogic.VALUE_100);
                }
                else
                {
                    SetCurrentStatesTimeout(GlobalLogic.VALUE_100);
                }

            },
            (BeStates pkState) =>
            {
                SwitchStates(new BeStateData((int)ActionState.AS_IDLE));
            }
        );

        AddStates2Graph(kRollState);


        //--------------------------------------------------------
        //起身
        BeStates kGetupState = new BeStates(
            (int)ActionState.AS_GETUP,
            (int)AStateTag.AST_BUSY | (int)AStateTag.AST_CONTROLED,
            (BeStates pkState) =>
            {
                //在这里判断是否要进入死亡状态
                if (pkActor.IsDead())
                {
                    pkActor.DoDead();
                    return;
                }

                pkActor.buffController.TriggerBuffs(BuffCondition.GETUP, null);

                pkActor.stateController.SetAbilityEnable(BeAbilityType.MOVE, false);
                pkActor.SetTag((int)AState.AST_FALLGROUND | (int)AState.ACS_FALL, false);
                pkActor.ClearMoveSpeed();
                pkActor.PlayAction(ActionType.ActionType_Getup);
                SetCurrentStatesTimeout(pkActor.GetCurrentActionDuration());

                pkActor.TriggerEventNew(BeEventType.onGetUp);

                pkActor.protectManager.ClearGroundProtect();
                pkActor.protectManager.DelayClearFallProtect();

                var battleType = pkActor.battleType;
                //这里加一个无敌buff
                if (battleType == BattleType.MutiPlayer ||
                        battleType == BattleType.PVP3V3Battle ||
                        battleType == BattleType.GuildPVP ||
                        battleType == BattleType.Training ||
                        battleType == BattleType.MoneyRewardsPVP ||
                        battleType == BattleType.ScufflePVP ||
                        battleType == BattleType.ChijiPVP||
                        battleType == BattleType.BattlegroundPVP)
                {
                    pkActor.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE_GET_UP, GlobalLogic.VALUE_700);
                }

                if (pkActor.CanUseDunfu())
                {
                    pkActor.StartDunfu();
                    //SetCurrentStatesTimeout(GlobalLogic.VALUE_100000);
                }
                if (pkActor.isInDunfu)
                {
                    SetCurrentStatesTimeout(GlobalLogic.VALUE_100000);
                }

                // BOSS怪物起身技能触发
                if (null != pkActor.GetEntityData())
                {
                    if (pkActor.GetEntityData().getupID > 0 &&
                        (pkActor.GetEntityData().type == (int)ProtoTable.UnitTable.eType.BOSS
                            || pkActor.GetEntityData().type == (int)ProtoTable.UnitTable.eType.MONSTER
                            || pkActor.GetEntityData().type == (int)ProtoTable.UnitTable.eType.ELITE))
                    {
                        // 计算概率，使用技能
                        if (pkActor.FrameRandom.Random((uint)GlobalLogic.VALUE_1000) < pkActor.GetEntityData().getupIDRand)
                        {
                            pkActor.UseSkill(pkActor.GetEntityData().getupID, true);
                        }
                    }
                }
            },
            (BeStates pkState) =>
            {
                ClearStateStack();
                var state = new BeStateData((int)ActionState.AS_IDLE);
                PushState(ref state);
                LocomoteState();
            },
            (BeStates state, int t) =>
            {
                if (pkActor.UpdateDunfu(t))
                {
                    SetCurrentStatesTimeout(pkActor.GetCurrentActionDuration());
                }

            },
            (BeStates state, int t) =>
            {
                pkActor.stateController.SetAbilityEnable(BeAbilityType.MOVE, true);
            }
        );

        AddStates2Graph(kGetupState);
        //-----------------------------------------------------
        BeStates kHurtState = new BeStates(
            (int)ActionState.AS_HURT,
            (int)AStateTag.AST_BUSY | (int)AStateTag.AST_CONTROLED,
            (BeStates pkState) =>
            {
                pkActor.ClearMoveSpeed((int)SpeedCear.SPEEDCEAR_XYZ);

                //记录僵直初始位置
                pkActor.FrozenStartDis = pkActor.GetPosition();

                if (pkActor.HasTag((int)AState.AST_FALLGROUND) && !pkState.isForceSwitch)
                {
                    if (Global.Settings.useNewHurtAction)
                    {
                        pkActor.PlayQueuedAction(new List<ActionDesc>() {
                            new ActionDesc() { actionType = ActionType.ActionType_FALL_DOWN, timeout = 90 },
                            new ActionDesc() { actionType = ActionType.ActionType_FALL_GROUND01, timeout = 0 },
                        });
                        pkActor.sgPushState(new BeStateData((int)ActionState.AS_GETUP));
                        //pkActor.PlayAction(ActionType.ActionType_FALL_GROUND);
                    }
                    SetCurrentStatesTimeout(GlobalLogic.VALUE_150);
                }
                else
                {
                    pkActor.SetMoveSpeedX(pkActor.forceX);
                    pkActor.SetMoveSpeedZ(pkActor.forceY);

                    pkActor.hurtCount++;

                    ActionType at = ActionType.ActionType_HURT + pkActor.hurtCount % 2;

                    if (pkActor.HasAction(at))
                    {
                        pkActor.PlayAction(at);
                        SetCurrentStatesTimeout((int)(pkActor.m_cpkCurEntityActionInfo.fRealFramesTime));
                    }
                    else
                    {
                        SetCurrentStatesTimeout(GlobalLogic.VALUE_10);
                    }

                    if (pkActor.GetPosition().z <= VInt.Float2VIntValue(0.0001f))
                        pkActor.protectManager.StartStandHurtCount();

                }
            },
            (BeStates pkState) =>
            {
                LocomoteState();
            },
            (BeStates pkState, int s) =>
            {
                VInt3 currentPos = pkActor.GetPosition();
                if (Mathf.Abs(currentPos.x - pkActor.FrozenStartDis.x) >= pkActor.FrozenDisMax)
                {
                    pkActor.SetMoveSpeedX(0);
                    pkActor.SetMoveSpeedZ(0);
                }
            },
            (BeStates pkStates, int s) =>
            {
                //重置僵直参数
                pkActor.FrozenStartDis = VInt3.zero;
            }
        );

        AddStates2Graph(kHurtState);

        // 技能释放
        BeStates kCastSkillState = new BeStates(
            (int)ActionState.AS_CASTSKILL,
            (int)AStateTag.AST_BUSY | (int)AStateTag.AST_CONTROLED,
            (BeStates pkState) =>
            {
                if (pkActor.IsProcessRecord())
                {
                    pkActor.GetRecordServer().RecordProcess("PID:{0}-{1} USE SKILL {2} {3}", pkActor.m_iID, pkActor.GetName(), pkActor.GetCurSkillID(), pkActor.GetInfo());
                    pkActor.GetRecordServer().Mark(0x8779788, new int[]
                   {
                        pkActor.m_iID,pkActor.GetCurSkillID(),
                        pkActor.GetPosition().x, pkActor.GetPosition().y,
                        pkActor.GetPosition().z, pkActor.moveXSpeed.i, pkActor.moveYSpeed.i, pkActor.moveZSpeed.i,
                        pkActor.GetFace() ? 1: 0, pkActor.attribute.GetHP(), pkActor.attribute.GetMP(), pkActor.GetAllStatTag(),
                        pkActor.attribute.battleData.attack
                   }, pkActor.GetName());
                    // Mark:0x8779788 PID:{0}-{13} USE SKILL {1} Pos:({2},{3},{4}) Speed:({5},{6},{7}) Face:{8} Hp:{9} Mp:{10} tag:{11} attack:{12}
                }

                pkActor.grabController.ClearGrabbedEntity();

                pkActor.buffController.TriggerBuffs(BuffCondition.RELEASE_SKILL, null);
                pkActor.buffController.TriggerBuffs(BuffCondition.RELEASE_SEPCIFY_SKILL, null, pkActor.GetCurSkillID());

                pkActor.ClearQueuedAction();

                pkActor.skillController.ResetSkillPhase(pkActor.GetCurSkillID());

                int iSkillID = pkActor.skillController.GetSkillPhaseId();

                pkActor.StartSkill(pkActor.GetCurSkillID());

                var skill = pkActor.skillController.GetCurrentSkill();
                if (skill != null && skill.useInternalID)
                {
                    iSkillID = skill.skillData.SwitchSkillID;
                }

                if (skill != null && skill.IsCanceled())
                    return;

                pkActor.skillAttackScale = VFactor.one;


                bool isPvPMode = BattleMain.IsModePvP(pkActor.battleType);
                if (isPvPMode && skill != null)
                {
                    pkActor.skillAttackScale = VFactor.NewVFactor(skill.GetPvpAttackScale(), (long)GlobalLogic.VALUE_1000);
                }

                if (skill != null && skill.specialOperate)
                {
                    if (pkActor.skillController.skillPhase == skill.operationConfig.changePhase)
                    {
                        if (skill.joystickMode == SkillJoystickMode.FREE && skill.innerState == BeSkill.InnerState.CHOOSE_TARGET)
                        {
                            skill.SetInnerState(BeSkill.InnerState.LAUNCH);
                        }

                        if (skill.joystickHasMove)
                        {
                            iSkillID = skill.operationConfig.changeSkillIDs[skill.specialChoice];       //如果摇杆移动过则取摇杆对应的技能ID
                        }
                        else if (!skill.useInternalID)
                        {
                            iSkillID = skill.operationConfig.changeSkillIDs[0];                         //如果摇杆没有移动过并且范围内没有敌人 则取默认切换ID
                        }
                    }
                }

                if (skill != null && skill.actionSelect != null && skill.actionSelect.Length > 0)
                {
                    if (pkActor.skillController.skillPhase == skill.actionSelectPhase)
                    {
                        iSkillID = skill.actionSelect[skill.actionChoice];
                    }
                }

                ExecuteSkill(iSkillID);

            },
            (BeStates pkState) =>
            {
                pkActor.grabController.TryReleaseGrabbedEntity();
                pkActor.OnCurSkillPhaseFinish();
                ExecuteNextPhaseSkill();

			},
			(BeStates pkState, int s)=>
			{
				var skill = pkActor.skillController.GetCurrentSkill();
                if (skill != null && skill.charge)
                {
                    if (pkActor.IsDead())
                    {
                        SwitchStates(new BeStateData((int)ActionState.AS_IDLE));
                    }
                }
                if (skill != null && pkActor.skillController.skillPhase == skill.chargeConfig.repeatPhase && !skill.charged && skill.pressDuration > skill.GetCurrentCharge())
                {
                    skill.isCharging = false;
                    skill.charged = true;
                    if (skill.chargeConfig.buffInfo != 0)
                    {
                        bool playBuffAni = skill.chargeConfig.playBuffAni ? true : false;        //是否播放霸气动画
                        pkActor.buffController.TryAddBuff(skill.chargeConfig.buffInfo, pkActor, playBuffAni);
                    }
#if !LOGIC_SERVER
                    var effect = pkActor.m_pkGeActor.CreateEffect(skill.chargeConfig.effect, skill.chargeConfig.locator, 0, Vec3.zero);
                    if (effect != null)
                        effect.SetPosition(effect.GetPosition() + skill.chargeConfig.effectPos);
#endif
                    pkActor.TriggerEventNew(BeEventType.OnSkillChargeComplete, new EventParam(){m_Int = skill.skillID, m_Obj = skill});

                }
            },
            (BeStates pkState, int s) =>
            {
                pkActor.grabController.TryReleaseGrabbedEntity();
                //如果目标状态不是释放技能状态 则将需要取消的技能ID置为当前释放的技能ID
                if (SwitchFunctionUtility.IsOpen(23) && s != (int)ActionState.AS_CASTSKILL)
                {
                    pkActor.SetPreSkillID(pkActor.GetCurSkillID());
                }

                //if (pkActor.GetEntityData().battleDataID < 100)
                {
                    var curSkill = pkActor.GetSkill(pkActor.GetPreSkillID());
                    //技能被打断
                    if (curSkill != null && curSkill.skillState.IsRunning())
                    {
                        pkActor.CancelSkill(pkActor.GetPreSkillID());
                    }
                }

                //在技能状态结束才切换形态
                pkActor.twoStateMode = pkActor.tempTwoStateMode;
            }
        );

        //跳到最高
        SGAddEventHandler2States(
            kCastSkillState,
            new BeEventsHandler(
                (int)EventCommand.EVENT_COMMAND_UPSTOP,
                (BeStates pkState) =>
                {
                    //Logger.Log("trigger upstop in cast skill state");
                    if (pkActor.m_cpkCurEntityActionInfo != null && pkActor.m_cpkCurEntityActionInfo.triggerType == TriggerNextPhaseType.UPSTOP)
                    {
                        ExecuteNextPhaseSkill();

                    }
                }
            )
        );

        //触地
        SGAddEventHandler2States(
            kCastSkillState,
            new BeEventsHandler(
                (int)EventCommand.EVENT_COMMAND_TOUCHGROUND,
                (BeStates pkState) =>
                {

                    // Logger.Log("trigger touchground in cast skill state");
                    if (pkActor.m_cpkCurEntityActionInfo != null && pkActor.m_cpkCurEntityActionInfo.triggerType == TriggerNextPhaseType.TOUCHGROUND)
                    {
                        ExecuteNextPhaseSkill();
                    }
                    else
                    {
                        pkActor.DealSkillEvent(EventCommand.EVENT_COMMAND_TOUCHGROUND);
                    }
                }
            )
        );

        SGAddEventHandler2States(
            kCastSkillState,
            new BeEventsHandler(
                (int)EventCommand.EVENT_COMMAND_RELEASE_SKILL_BUTTON,
                (BeStates pkState) =>
                {
                    // Logger.Log("trigger release skill button");

                    if (pkActor.m_cpkCurEntityActionInfo != null && pkActor.m_cpkCurEntityActionInfo.triggerType == TriggerNextPhaseType.RELEASE_BUTTON)
                    {
                        ExecuteNextPhaseSkill();
                    }
                }
            )
        );


        SGAddEventHandler2States(
            kCastSkillState,
            new BeEventsHandler(
                (int)EventCommand.EVENT_COMMAND_PRESS_JOYSTICK,
                (BeStates pkState) =>
                {
                    //Logger.LogError("trigger press joystick");

                    pkActor.DealSkillEvent(EventCommand.EVENT_COMMAND_PRESS_JOYSTICK);
                }
            )
        );


        SGAddEventHandler2States(
            kCastSkillState,
            new BeEventsHandler(
                (int)EventCommand.EVENT_COMMAND_RELEASE_JOYSTICK,
                (BeStates pkState) =>
                {
                    //Logger.LogError("trigger release joystick");

                    pkActor.DealSkillEvent(EventCommand.EVENT_COMMAND_RELEASE_JOYSTICK);
                }
            )
        );

        SGAddEventHandler2States(
            kCastSkillState,
            new BeEventsHandler(
                (int)EventCommand.EVENT_COMMAND_PRESS_BUTTON_AGAIN,
                (BeStates pkState) =>
                {

                    // Logger.Log("trigger release skill button");
                    if (pkActor.m_cpkCurEntityActionInfo != null && pkActor.m_cpkCurEntityActionInfo.triggerType == TriggerNextPhaseType.PRESS_AGAIN)
                    {
                        ExecuteNextPhaseSkill();
                    }
                }
            )
        );


        AddStates2Graph(kCastSkillState);


        //被控状态
        BeStates kGrabState = new BeStates(
            (int)ActionState.AS_GRABBED,
            (int)AStateTag.AST_BUSY | (int)AStateTag.AST_CONTROLED | (int)AStateTag.AST_LOCKZ,
            (BeStates pkState) =>
            {

                pkActor.ClearMoveSpeed();
                pkActor.TriggerEventNew(BeEventType.onGrabbed);
                //pkActor.SetTag((int)AState.AST_FALLGROUND, false);

                if (pkActor.grabController.isAbsorb)
                {
                    pkActor.grabController.StartAbsorb();
                }

                if (pkActor.protectManager.IsAfterGetUpFallCounting())
                {
                    pkActor.protectManager.ResetFallTime();
                }
            },
            (BeStates pkState) =>
            {
                pkActor.stateController.SetGrabState(GrabState.NONE);
                pkActor.EndPressCount();
                pkActor.JugePositionAfterGrab();

                //pkActor.Locomote(new BeStateData((int)ActionState.AS_FALL, 0, 0, VInt.Float2VIntValue(1f), 0, 0, 300), true);
                pkActor.Locomote(new BeStateData((int)ActionState.AS_FALL) { _StateData3 = VInt.Float2VIntValue(1f), _timeout = 300 }, true);
                
            },
            (BeStates pkState, int time) =>
            {
                pkActor.grabController.Update(time);
            },
            //leaves
            (BeStates pkState, int time) =>
            {
                pkActor.stateController.SetGrabState(GrabState.NONE);
                pkActor.EndPressCount();
#if !LOGIC_SERVER
                if (pkActor.m_pkGeActor != null)
                {
                    var objActor = pkActor.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Actor);
                    if (objActor != null)
                    {
                        objActor.transform.localPosition = Vector3.zero;
                        objActor.transform.localRotation = Quaternion.identity;
                    }
                }
#endif
            }
        );

        AddStates2Graph(kGrabState);

        //死亡
        BeStates kDeadState = new BeStates(
            (int)ActionState.AS_DEAD,
            (int)AStateTag.AST_LOCKZ,
            (BeStates pkState) =>
            {

                pkActor.ClearMoveSpeed();
                pkActor.grabController.EndGrab();

                //进入死亡流程，确认已经死亡
                pkActor.SetIsDead(true);

                if (pkActor.IsMonster() || pkActor.GetEntityData().isSummonMonster)
                {
                    if (!pkActor.IsSkillMonster())
                        pkActor.GetEntityData().SetHP(-1);

                    //string dead2 = "Expdead2";
                    if (pkActor.HasAction(Global.ACTION_EXPDEAD2))
                    {
#if !LOGIC_SERVER
                        
                        
                        if(pkActor.m_pkGeActor != null)
                        {
                            pkActor.m_pkGeActor.RemoveSurface(uint.MaxValue);
                        }
#endif
                        //pkActor.m_pkGeActor.ChangeSurface("死亡2", 0f);
                        pkActor.PlayAction(Global.ACTION_EXPDEAD2);
                        SetCurrentStatesTimeout(pkActor.GetCurrentActionDuration());

                        return;
                    }

                    //string dead3 = "Expdead3";
                    if (pkActor.HasAction(Global.ACTION_EXPDEAD3))
                    {
#if !LOGIC_SERVER
                        
                        if(pkActor.m_pkGeActor != null)
                        {
                            pkActor.m_pkGeActor.RemoveSurface(uint.MaxValue);
                            //pkActor.m_pkGeActor.ChangeSurface("死亡2", 0f);
                            pkActor.m_pkGeActor.SetActorVisible(false);
                        }

#endif
                        pkActor.PlayAction(Global.ACTION_EXPDEAD3);
                        SetCurrentStatesTimeout(pkActor.GetCurrentActionDuration());

                        return;
                    }

                    if (pkActor.playedExpDead)
                    {
                        SetCurrentStatesTimeout(pkActor.GetCurrentActionDuration());
                        return;
                    }

                    int whiteTime = IntMath.Float2Int(Global.Settings.deadWhiteTime * GlobalLogic.VALUE_1000);

                    int duration = 0;


                    if (pkActor.IsBoss() && !pkActor.GetEntityData().isSummonMonster)
                    {
                        if (!pkActor.HasTag((int)AState.AST_FALLGROUND))
                        {
                            if (pkActor.HasAction(ActionType.ActionType_DEAD))
                            {
                                pkActor.PlayAction(ActionType.ActionType_DEAD);
                                duration = pkActor.GetCurrentActionDuration();
                            }
                            else if (pkActor.HasAction(ActionType.ActionType_FALL_GROUND))
                            {
                                pkActor.PlayAction(ActionType.ActionType_FALL_GROUND);
                                duration = pkActor.GetCurrentActionDuration();
                            }
                        }

                        if (duration > 0)
                        {
#if !LOGIC_SERVER
                            pkActor.delayCaller.DelayCall(duration, () =>
                            {
                                //pkActor.m_pkGeActor.GetAnimation().Stop();
                                if(pkActor.m_pkGeActor != null){
                                    pkActor.m_pkGeActor.StopAction();
                                    pkActor.m_pkGeActor.ChangeSurface("死亡2", 0f);
                                }
                                //Logger.LogErrorFormat("boss死亡211111！！！！");
                            });

#endif
                            //延迟一帧切换状态，免得执行不了DelayCall
                            SetCurrentStatesTimeout(duration + GlobalLogic.VALUE_10, true);
                        }
                        else
                        {
                            //pkActor.m_pkGeActor.GetAnimation().Stop();
#if !LOGIC_SERVER
                            
                            if (pkActor.m_pkGeActor != null)
                            {
                                pkActor.m_pkGeActor.ChangeSurface("死亡2", 0f);
                            }
#endif
                            //Logger.LogErrorFormat("boss死亡2！！！！");
                            SetCurrentStatesTimeout(0, true);
                        }

                    }
                    else
                    {
                        if (pkActor.IsSkillMonster())
                        {
                            pkActor.deadType = DeadType.NORMAL;
                        }

                        if (pkActor.deadType == DeadType.NORMAL)
                        {
                            if (!pkActor.HasTag((int)AState.AST_FALLGROUND))
                            {
                                if (pkActor.HasAction(ActionType.ActionType_DEAD))
                                {
                                    pkActor.PlayAction(ActionType.ActionType_DEAD);
                                    duration = pkActor.GetCurrentActionDuration();
                                }
                                else if (pkActor.HasAction(ActionType.ActionType_FALL_GROUND))
                                {
                                    pkActor.PlayAction(ActionType.ActionType_FALL_GROUND);
                                    duration = pkActor.GetCurrentActionDuration();
                                }
                            }

                            if (duration > 0)
                            {
#if !LOGIC_SERVER

                                pkActor.delayCaller.DelayCall(duration, () =>
                                {
                                    //pkActor.m_pkGeActor.GetAnimation().Stop();
                                    pkActor.m_pkGeActor.StopAction();
                                    pkActor.m_pkGeActor.ChangeSurface("死亡", whiteTime / 1000f);
                                });
#endif

                                SetCurrentStatesTimeout(duration + whiteTime, true);


                            }
                            else
                            {


#if !LOGIC_SERVER
                                if (pkActor != null && pkActor.m_pkGeActor != null)
                                    //pkActor.m_pkGeActor.GetAnimation().Stop();
                                    pkActor.m_pkGeActor.StopAction();
                                else
                                {
                                    if (pkActor == null)
                                        Logger.LogErrorFormat("pkActor is null!!");
                                    else if (pkActor != null && pkActor.m_pkGeActor == null)
                                        Logger.LogErrorFormat("pkActor.m_pkGeActo is null!! {0}", pkActor.GetName());
                                }

                                //pkActor.m_pkGeActor.GetAnimation().Stop();
                                pkActor.m_pkGeActor.ChangeSurface("死亡", whiteTime / 1000f, true, false);

#endif
                                SetCurrentStatesTimeout(duration + whiteTime, true);


                            }
#if !LOGIC_SERVER
                            // 隐身状态下死亡不播爆炸特效
                            if (pkActor.buffController.HasBuffByID(66) == null)
                            {
                                var pos = pkActor.GetPosition();
                                var bescene = pkActor.CurrentBeScene;
                                bescene.DelayCaller.DelayCall(duration + whiteTime - GlobalLogic.VALUE_10, () =>
                                {
                                    bescene.currentGeScene.CreateEffect(Global.Settings.normalDeadEffect, 0, pos.vec3);
                                });
                            }

#endif

                            //Logger.LogErrorFormat("start die!!! {0}", Time.realtimeSinceStartup);
                        }
                        else if (pkActor.deadType == DeadType.CRITICAL || pkActor.deadType == DeadType.IMMEDIATE)
                        {

                            int time = whiteTime;
                            pkActor.ResetActionInfo();
#if !SERVER_LOGIC

                            if (pkActor.m_pkGeActor != null)
                            {
                                pkActor.m_pkGeActor.StopAction();
                                pkActor.m_pkGeActor.ChangeSurface("死亡", time / 1000f, true, false);   
                            }
                            

                            string effect = "";

                            if (pkActor.deadType == DeadType.CRITICAL)
                            {
                                effect = Global.Settings.critialDeadEffect;
                            }
                            else
                            {
                                effect = Global.Settings.immediateDeadEffect;
                            }

                            var scene = pkActor.CurrentBeScene;
                            scene.DelayCaller.DelayCall(duration + time - GlobalLogic.VALUE_10, () =>
                            {
                                scene.currentGeScene.CreateEffect(effect, 0, pkActor.GetPosition().vec3);
                            });

                            if (pkActor.deadType == DeadType.CRITICAL)
                                Logger.LogWarningFormat("暴击死亡！！！");
                            else if (pkActor.deadType == DeadType.IMMEDIATE)
                                Logger.LogWarningFormat("秒杀死亡！！！");
#endif



                            SetCurrentStatesTimeout(duration + time, true);
                        }
                    }
                }
                else
                {
                    if (pkActor.HasAction(ActionType.ActionType_DEAD))
                    {
                        pkActor.PlayAction(ActionType.ActionType_DEAD);

                        int deadTime = GlobalLogic.VALUE_1000;
                        SetCurrentStatesTimeout(deadTime, true);
                    }
                    else
                    {
                        SetCurrentStatesTimeout(GlobalLogic.VALUE_10);
                        pkActor.m_iEntityLifeState = (int)EntityLifeState.ELS_CANREMOVE;
                    }
                }
            },
            (BeStates pkState) =>
            {
                //var param1 = DataStructPool.EventParamPool.Get();
                //param1.m_Bool = true;
                //int[] vals = new int[1];
                //vals[0] = 0;
                //pkActor.TriggerEventNew(BeEventType.onMarkRemove, param1);

                var param = pkActor.TriggerEventNew(BeEventType.onMarkRemove, new EventParam(){ m_Bool = true});

                bool continueDead = param.m_Bool;
                
                var param1 = pkActor.TriggerEventNew(BeEventType.onBeforeAfterDead, new EventParam(){ m_Bool = true});
                bool continueAfterDead = param1.m_Bool;

                if (continueAfterDead)
                    pkActor.TriggerEventNew(BeEventType.onAfterDead);

                if (continueDead)
                {
                    if (pkActor.IsBoss() && !pkActor.GetEntityData().isSummonMonster)
                    {
#if !LOGIC_SERVER
                        pkActor.m_pkGeActor.SetFootIndicatorVisible(false);
                        pkActor.m_pkGeActor.SetHeadInfoVisible(false);
                        pkActor.m_pkGeActor.RemoveHPBar();

#endif
                        if(pkActor.CurrentBeBattle!=null&& pkActor.CurrentBeBattle.dungeonManager!=null&& pkActor.CurrentBeBattle.dungeonManager.GetDungeonDataManager() != null)
                        {
                            bool isBossRoom = pkActor.CurrentBeBattle.dungeonManager.GetDungeonDataManager().IsBossArea();
                            if (isBossRoom)
                            {
                                pkActor.dontDelete = true;
                            }
                        }
                    }

                    pkActor.m_iEntityLifeState = (int)EntityLifeState.ELS_CANREMOVE;
                }


#if !LOGIC_SERVER
                if(pkActor.m_pkGeActor != null)
                {
                    pkActor.m_pkGeActor.Clear(
                    (int)GeEntity.GeEntityRes.EffectUnique |
                    (int)GeEntity.GeEntityRes.EffectMultiple |
                    (int)GeEntity.GeEntityRes.EffectGlobal);
                }
#endif


                pkActor.OnDead();

            }
        );

        kDeadState.priority = 2;
        kDeadState.canCover = false;

        AddStates2Graph(kDeadState);


        BeStates kWinState = new BeStates(
            (int)ActionState.AS_WIN,
            (int)AStateTag.AST_BUSY,
            (BeStates pkState) =>
            {
                pkActor.ClearMoveSpeed();
                if (pkActor.HasAction(ActionType.ActionType_WIN))
                {
                    pkActor.PlayAction(ActionType.ActionType_WIN);
                    SetCurrentStatesTimeout(pkActor.GetCurrentActionDuration());
                }
                else
                {
                    SetCurrentStatesTimeout(GlobalLogic.VALUE_10);
                }
            },
            (BeStates pkState) =>
            {
                SwitchStates(new BeStateData((int)ActionState.AS_IDLE));
            }
        );

        AddStates2Graph(kWinState);
    }
    public override void Locomote(BeStateData rkStateData, bool bForce = false)
    {
        rkStateData.isForceSwitch = bForce;
        int iState = rkStateData._State;
        bool switchFlag = false;
        switch ((ActionState)iState)
        {
            case ActionState.AS_HURT:
            case ActionState.AS_FALL:
                pkActor.fallForGrab = rkStateData._StateData == 0 ? false : true;
                pkActor.forceX = rkStateData._StateData2;
                pkActor.forceY = rkStateData._StateData3;
                pkActor.forceXAcc = rkStateData._StateData4;
                pkActor.tempForceXAccTimer = rkStateData._StateData5;
                pkActor.forceYAcc = rkStateData._StateData6;
                pkActor.tempForceZAccTimer = rkStateData._StateData7;
                pkActor.hurtAction = rkStateData._HurtAction;

                if (pkActor.sgGetCurrentState() == (int)ActionState.AS_GRABBED && !bForce)
                {
                    break;
                }

                if (!Global.Settings.useNewHurtAction || pkActor.IsMonster())//怪物倒地被击流程还用以前的
                {
                    if (pkActor.HasTag((int)AState.AST_FALLGROUND) && iState == (int)ActionState.AS_HURT && !bForce)
                    {
                        ResetCurrentStateTime();
                        break;
                    }
                }

                if (iState == (int)ActionState.AS_HURT)
                {
                    if (pkActor.stateController.HasBuffState(BeBuffStateType.STUN) && pkActor.sgGetCurrentState() == (int)ActionState.AS_HURT)
                        return;
                }
                switchFlag = true;
                //pkActor.sgSwitchStates(rkStateData);

                break;
            case ActionState.AS_CASTSKILL:
                int skillID = rkStateData._StateData;
                if (pkActor.sgGetCurrentState() == (int)ActionState.AS_CASTSKILL)
                {
                    pkActor.SetPreSkillID(pkActor.GetCurSkillID());
                }
                else
                {
                    pkActor.SetPreSkillID(skillID);
                }
              //  Logger.LogErrorFormat("AS_CASTSKILL {0}", skillID);
                pkActor.SetCurSkillID(skillID);
                switchFlag = false;
                pkActor.sgSwitchStates(rkStateData);
                break;

            case ActionState.AS_BUSY:

                pkActor.SetMoveSpeedXLocal(rkStateData._StateData2);
                pkActor.SetMoveSpeedZ(rkStateData._StateData3);
                switchFlag = true;
                //pkActor.sgSwitchStates(rkStateData);
                break;

            case ActionState.AS_GRABBED:
                //设置播放动画
                if (rkStateData._StateData > 0)
                {
                    if ((ActionType)rkStateData._StateData == ActionType.ActionType_FALL_GROUND)
                    {
                        pkActor.sgPushState(new BeStateData((int)ActionState.AS_GETUP));
                        pkActor.SetTag((int)AState.AST_FALLGROUND, true);
                    }
                    pkActor.PlayAction((ActionType)rkStateData._StateData);
                }
                else
                {
                    pkActor.PlayAction(ActionType.ActionType_HURT);
                }
                switchFlag = true;
                //pkActor.sgSwitchStates(rkStateData);
                break;

            case ActionState.AS_JUMP:
            case ActionState.AS_JUMPBACK:
            default:
                switchFlag = true;
                //pkActor.sgSwitchStates(rkStateData);
                break;
        }
        if (switchFlag)
        {
            //当前状态是释放技能并且将要切换的状态不是释放技能释放状态时则重置设置之前使用的技能ID  用于切idle状态时的技能取消
            if ((pkActor.sgGetCurrentState() == (int)ActionState.AS_CASTSKILL) && ((ActionState)iState != ActionState.AS_CASTSKILL))
            {
                pkActor.SetPreSkillID(pkActor.GetCurSkillID());
            }
            pkActor.sgSwitchStates(rkStateData);
        }
    }

    public void TriggerEvent()
    {
        if (pkActor.IsInMoveDirection())
            FireEvents2CurrentStates((int)EventCommand.EVENT_COMMAND_PRESS_JOYSTICK);
        //else
        //    FireEvents2CurrentStates((int)EventCommand.EVENT_COMMAND_RELEASE_JOYSTICK);
    }

    public void ExecuteNextPhaseSkill()
    {
        var skill2 = pkActor.GetCurrentSkill();
        if (skill2 != null && skill2.IsCanceled())
        {
            //Logger.LogErrorFormat("ExecuteNextPhaseSkill skill {0}", skill2.skillID);
            return;
        }

        pkActor.buffController.ClearPhaseDelete();
        pkActor.ClearPhaseDelete();
        pkActor.ClearPhaseDeleteAudio();

        //保证执行到最后一帧
        pkActor.UpdateFrame();

        int iSkillID = pkActor.skillController.GetSkillPhaseId();
        //Logger.LogErrorFormat("ExecuteNextPhaseSkill GetSkillPhaseId {0}", skill2.skillID);
        if (iSkillID < 0)
        {
            pkActor.FinishSkill(pkActor.GetCurSkillID());
            LocomoteState();
        }
        else
        {
            var skill = pkActor.GetCurrentSkill();
            if (skill != null)
            {
          //      Logger.LogErrorFormat("ExecuteNextPhaseSkill GetCurrentSkill {0}", skill.skillID);
                SkillWalkMode walkMode = (SkillWalkMode)skill.GetWalkMode();

                if (walkMode == SkillWalkMode.CHANGE_DIR)
                {
                    pkActor.SetSkillWalkMode(walkMode, VFactor.zero);
                }

                if (skill.charge)
                {
                    //当不处于蓄力循环阶段时将正在蓄力标志重置
                    if (pkActor.skillController.skillPhase != skill.chargeConfig.repeatPhase)
                    {
                        skill.isCharging = false;
                    }

                    //是否使用新版蓄力开始判断条件
                    //考虑到快速点击两次按钮并且按住蓄力的情况
                    bool flag = (skill.buttonState == ButtonState.PRESS || skill.buttonState == ButtonState.PRESS_AGAIN) && pkActor.skillController.skillPhase == skill.chargeConfig.repeatPhase;
                    //flag = skill.buttonState == ButtonState.PRESS && pkActor.skillPhase == skill.chargeConfig.repeatPhase;
                    if (flag)
                    {
                        skill.pressDuration = 0;                                //从蓄力开始重新记录时间
                        skill.isCharging = true;
                        if(!skill.hideSpellBar)
                        pkActor.StartSpellBar(eDungeonCharactorBar.Power, skill.GetCurrentCharge(), false);
                    }

                    if (skill.buttonState == ButtonState.RELEASE && pkActor.skillController.skillPhase == skill.chargeConfig.repeatPhase)
                    {
                        ExecuteNextPhaseSkill();
                        return;
                    }
                    if (pkActor.skillController.skillPhase == skill.chargeConfig.changePhase
                        //因为有装备去改蓄力判定时间 这样会造成状态机那边的蓄力判定是成功的 但是实际切换技能Id会切换到不蓄力阶段
                        /*&& (skill.pressDuration > skill.GetCurrentCharge() ||skill.buttonState == ButtonState.PRESS)*/
                        && skill.charged)             //判断在蓄力成功之后才切换技能
                    {
                        iSkillID = skill.chargeConfig.switchPhaseID;
                        pkActor.StopSpellBar(eDungeonCharactorBar.Power);
                    }

                    if (pkActor.skillController.skillPhase == skill.chargeConfig.changePhase && !skill.charged)
                        pkActor.StopSpellBar(eDungeonCharactorBar.Power);

                }


                if (skill.specialOperate)
                {
                    if (pkActor.skillController.skillPhase == skill.operationConfig.changePhase)
                    {
                        if (skill.joystickMode == SkillJoystickMode.FREE && skill.innerState == BeSkill.InnerState.CHOOSE_TARGET)
                        {
                            skill.SetInnerState(BeSkill.InnerState.LAUNCH);
                        }

                        iSkillID = skill.operationConfig.changeSkillIDs[skill.specialChoice];
                        //Logger.LogErrorFormat("iSkillID = {0}", iSkillID);
                    }
                }

                if (skill != null && skill.actionSelect != null && skill.actionSelect.Length > 0)
                {
                    if (pkActor.skillController.skillPhase == skill.actionSelectPhase)
                    {
                        iSkillID = skill.actionSelect[skill.actionChoice];
                    }
                }

                if (skill.GetWalkMode() == (int)SkillWalkMode.FREE2 || skill.CurSkillWalkMode == (int)SkillWalkMode.FREE2)
                    pkActor.ResetCmdDirty();
            }
            
            //bool[] executeNextAgain = new bool[] { false };
            //pkActor.TriggerEvent(BeEventType.onNextPhaseBeforeExecute, new object[]{ executeNextAgain, iSkillID }););

            var param = pkActor.TriggerEventNew(BeEventType.onNextPhaseBeforeExecute, new EventParam(){ m_Int = iSkillID, m_Bool = false});
            bool executeNextAgain = param.m_Bool;

            if (executeNextAgain) 
            {
                ExecuteNextPhaseSkill();
                return;
            }
            ExecuteSkill(iSkillID);
        }
    }

    public void ExecuteSkill(int skillID)
    {
        if (pkActor.IsProcessRecord())
        {
            pkActor.GetRecordServer().RecordProcess("PID:{0}-{1} USE SKILL{2} - sub id {3} {4}", pkActor.m_iID, pkActor.GetName(), pkActor.GetCurSkillID(), skillID, pkActor.GetInfo());
            pkActor.GetRecordServer().Mark(0x8779A89, new int[]
          {
                 pkActor.m_iID,pkActor.GetCurSkillID(), skillID,
                 pkActor.GetPosition().x, pkActor.GetPosition().y, pkActor.GetPosition().z,
                pkActor.moveXSpeed.i, pkActor.moveYSpeed.i, pkActor.moveZSpeed.i,
                pkActor.GetFace() ? 1 : 0, pkActor.attribute.GetHP(), pkActor.attribute.GetMP(), pkActor.GetAllStatTag(),
                pkActor.attribute.battleData.attack
          }, pkActor.GetName());
            // Mark:0x8779A89 PID:{0}-{14} USE SKILL{1} - sub id {2} Pos:({3},{4},{5}),Speed:({6},{7},{8}),Face: {9},Hp: {10},Mp: {11},Flag: {12},attack: {13}
        }
        // Logger.LogErrorFormat("ExecuteSkill GetCurrentSkill {0}", skillID);
        var curSkill = pkActor.GetCurrentSkill();
        if(curSkill != null && curSkill.NeedClearSpeed())
            pkActor.ClearMoveSpeed((int)SpeedCear.SPEEDCEAR_X | (int)SpeedCear.SPEEDCEAR_Y);

        int skillIDForResetAction = skillID;

        float aniSpeed = 1.0f;
        
        if (curSkill != null)
        {
            aniSpeed = curSkill.GetSkillSpeedFactor().single;
            curSkill.EnterPhase(pkActor.skillController.skillPhase);

            // 技能1710 改1503为1715
            //int[] skillIdList = new int[1];
            //skillIdList[0] = skillID;
            //pkActor.TriggerEvent(BeEventType.onPreSetSkillAction, new object[] { skillIdList });

            //var param1 = DataStructPool.EventParamPool.Get();
            //param1.m_Int = skillID;
            //pkActor.TriggerEventNew(BeEventType.onPreSetSkillAction, param1);

            //if (param1.m_Int != skillID)
            //    skillIDForResetAction = param1.m_Int;

            //DataStructPool.EventParamPool.Release(param1);

            var param = pkActor.TriggerEventNew(BeEventType.onPreSetSkillAction, new EventParam(){ m_Int = skillID, });
            if (param.m_Int != skillID)
                skillIDForResetAction = param.m_Int;
        }

#if UNITY_EDITOR && !LOGIC_SERVER
        string log;
        if (skillID == skillIDForResetAction)
        {
            log = string.Format("技能ID:{0},当前时间:{1}", skillID, Time.time);
        }
        else
        {
            log = string.Format("技能ID:{0}->替换技能ID:{2},当前时间:{1}", skillID, Time.time, skillIDForResetAction);
        }
        if (skillUseRecord.Count >= 40)
            skillUseRecord.RemoveAt(0);
        skillUseRecord.Add(log);
#endif
        
        string tmp = pkActor.GetActionNameBySkillID(skillIDForResetAction);

        if (tmp == null)
        {
            Logger.LogErrorFormat("name:{0} skillid:{1} skillphase:{2} 找不到技能id {3}的技能配置文件",
                pkActor.GetName(),
                pkActor.GetCurSkillID(),
                pkActor.skillController.skillPhase,
                skillID);
        }

        //Logger.LogErrorFormat("execute skill id {0}", skillID);



        m_uiCurrentStatesTime = 0;
        if (curSkill != null && curSkill.skillData!=null && pkActor.m_cpkEntityInfo != null && curSkill.skillData.PhaseRelatedSpeed == 1)
        {
            BDEntityActionInfo info = pkActor.m_cpkEntityInfo._vkActionsMap[tmp];
            if (info != null && info.relatedAttackSpeed)
            {
                aniSpeed = curSkill.GetSkillSpeedFactor().single * (info.attackSpeed / 1000.0f);
                if (aniSpeed < 0.4f)
                    aniSpeed = 0.4f;
            }
            else
            {
                aniSpeed = VFactor.NewVFactor(curSkill.skillData.Speed, GlobalLogic.VALUE_1000).single;
            }
        }


        pkActor.PlayAction(tmp, aniSpeed);

        if (pkActor.m_cpkCurEntityActionInfo == null)
        {
            Logger.LogWarning("ExecuteSkill m_cpkCurEntityActionInfo is nil");
            return;
        }


        int timeout = pkActor.m_cpkCurEntityActionInfo.fRealFramesTime;

        if (pkActor.m_cpkCurEntityActionInfo.useSpellBar/* && pkActor.spellBarDuration <= 0*/)
        {
            int spellBarTime = IntMath.Float2Int(pkActor.m_cpkCurEntityActionInfo.spellBarTime * GlobalLogic.VALUE_1000);
            if (spellBarTime > 0f&& spellBarTime <= timeout)            //当蓄力读条填写时间超过实际技能释放时间时  读条采用技能释放时间
                pkActor.StartSpellBar(eDungeonCharactorBar.Sing, spellBarTime);
            else
                pkActor.StartSpellBar(eDungeonCharactorBar.Sing, timeout);
        }

        SetCurrentStatesTimeout(timeout); 

        //不蓄力,但是技能是蓄力的情况
        var skill = pkActor.GetCurrentSkill();
        if (skill != null && !skill.charge && skill.chargeConfig.repeatPhase > 0 && pkActor.skillController.skillPhase == skill.chargeConfig.repeatPhase)
        {
            SetCurrentStatesTimeout(IntMath.Float2Int(skill.chargeConfig.chargeMinDuration, GlobalLogic.VALUE_1000), true);
        }

        TriggerEvent();
    }

    public void ExecuteEndPhase()
    {
        for (int i = pkActor.skillController.skillPhase; i < pkActor.skillController.SkillPhaseArray.Length; i++)
        {
            ExecuteNextPhaseSkill();
        }
    }

    // 插入执行阶段在指定阶段前，阶段序号从0开始
    public void InsertExecuteInPhase(int phase, int index)
    {
        pkActor.skillController.skillPhase = index;
        ExecuteSkill(phase);
    }
}
