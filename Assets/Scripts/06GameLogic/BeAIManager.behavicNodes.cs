using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PathFinder;
using ProtoTable;
using Tenmove.Runtime.Math;
public partial class BeAIManager
{
    #region tree node
    public behaviac.BE_State GetState()
    {
        behaviac.BE_State entityState = behaviac.BE_State.NONE;

        BeEntity entity = owner;

        if (entity != null)
        {
            ActionState state = (ActionState)entity.GetStateGraph().GetCurrentState();
            switch (state)
            {
                case ActionState.AS_IDLE:
                    entityState = behaviac.BE_State.IDLEE;
                    break;
                case ActionState.AS_WALK:
                case ActionState.AS_RUN:
                    entityState = behaviac.BE_State.WALK;
                    break;
                case ActionState.AS_CASTSKILL:
                    entityState = behaviac.BE_State.SKILL;
                    break;
                case ActionState.AS_DEAD:
                    entityState = behaviac.BE_State.DEAD;
                    break;
                case ActionState.AS_JUMP:
                    entityState = behaviac.BE_State.JUMP;
                    break;
                case ActionState.AS_GRABBED:
                    entityState = behaviac.BE_State.GRAPED;
                    break;
                case ActionState.AS_HURT:
                    entityState = behaviac.BE_State.BEIJI;
                    break;
                default:

                    if (entity.HasTag((int)AState.AST_FALLGROUND))
                        entityState = behaviac.BE_State.DAODI;
                    else if (entity.HasTag((int)AState.ACS_FALL))
                        entityState = behaviac.BE_State.FUKONG;
                    break;
            }
        }

        return entityState;
    }

    public bool CheckHPMP(behaviac.HMType type, behaviac.BE_Operation operation, float value)
    {
        bool ret = false;
        if (owner != null)
        {
            VFactor realValue = VFactor.zero;
            VFactor fValue = VFactor.zero;

            switch (type)
            {
                case behaviac.HMType.HP:
                    realValue = new VFactor(owner.GetEntityData().GetHP(), 1);
                    fValue = VFactor.NewVFactorF(value, 1);
                    break;
                case behaviac.HMType.HP_PERCENT:
                    realValue = owner.GetEntityData().GetHPRate();
                    fValue = VFactor.NewVFactorF(value, 100);
                    break;
                case behaviac.HMType.MP:
                    realValue = new VFactor(owner.GetEntityData().GetMP(), 1);
                    fValue = VFactor.NewVFactorF(value, 1);
                    break;
                case behaviac.HMType.MP_PERCENT:
                    realValue = owner.GetEntityData().GetMPRate();
                    fValue = VFactor.NewVFactorF(value, 100);
                    break;
            }

            switch (operation)
            {
                case behaviac.BE_Operation.EqualTo:
                    ret = realValue == fValue;
                    break;
                case behaviac.BE_Operation.GreaterThan:
                    ret = realValue > fValue;
                    break;
                case behaviac.BE_Operation.GreaterThanOrEqualTo:
                    ret = realValue >= fValue;
                    break;
                case behaviac.BE_Operation.LessThan:
                    ret = realValue < fValue;
                    break;
                case behaviac.BE_Operation.LessThanOrEqualTo:
                    ret = realValue <= fValue;
                    break;
                case behaviac.BE_Operation.NotEqualTo:
                    ret = fValue != realValue;
                    break;
            }
        }

        return ret;
    }

    public void FireWalkCommand(VInt3 targetPos,int duration)
    {
        var command = (BeAIWalkCommand)BeAICommandPool.GetAICommand(AI_COMMAND.WALK, owner);
        command.Init(duration, targetPos, VInt.one.i / Global.Settings.tolerate, false, false, true);
        ExecuteCommand(command);
    }

    #endregion
}
