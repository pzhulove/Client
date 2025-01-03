using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using BehaviorDesigner.Runtime;
using GameClient;

 
public class BeActorMoveController : IBeActorController
{
    public BeActor owner;
    public VInt3 targetPos;
    public VInt tolerance;

    bool isEnd = false;
    bool autoRemove = false;

    public BeActorMoveController(VInt3 targetPosition, float tolerance,bool autoRemove = true)
    {
        targetPos = targetPosition;
        this.tolerance = VInt.Float2VIntValue(tolerance);
        this.autoRemove = autoRemove;
    }
    public void SetOwner(BeActor actor)
    {
        owner = actor;
    }
    public void OnEnter()
    {

    }

    public bool AutoRemove()
    {
        return autoRemove;
    }
    
    public void OnTick(int delta)
    {
        if(isEnd)
        {
            return;
        }

        int x = owner.GetPosition().x - targetPos.x;
        if (x < 0)
        {
            DoWalk(MoveDir.RIGHT, true);
        }
        else 
        {
            DoWalk(MoveDir.LEFT, true);
        }

        if(IsNearTargetPosition())
        {
            isEnd = true;
            owner.ResetMoveCmd();        
        }
    }
    public bool IsEnd()
    {
       return isEnd;
    }

    bool IsNearTargetPosition()
    {
        int distance = tolerance.i;
        VInt3 pos = owner.GetPosition();
        return (((Mathf.Abs(targetPos.x - pos.x)) <= distance) && (Mathf.Abs(targetPos.y - pos.y) <= distance));
    }

    enum MoveDir
    {
        RIGHT = 0,
        LEFT,
        TOP,
        DOWN,
		RIGHT_TOP,
		LEFT_TOP,
		RIGHT_DOWN,
		LEFT_DOWN,
		
        COUNT
    }


    void DoWalk(MoveDir dir, bool reset=false)
    {
        if (reset)
        {
            owner.ResetMoveCmd();
        }

        switch (dir)
        {
		case MoveDir.RIGHT:
            owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X, true);
            break;
        case MoveDir.LEFT:
            owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X_NEG, true);
            break;
        case MoveDir.TOP:
            owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_Y, true);
            break;
        case MoveDir.DOWN:
            owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_Y_NEG, true);
            break;
		case MoveDir.LEFT_DOWN:
			owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X_NEG, true);
			owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_Y_NEG, true);
			break;
		case MoveDir.RIGHT_DOWN:
			owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X, true);
			owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_Y_NEG, true);
			break;
		case MoveDir.LEFT_TOP:
			owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X_NEG, true);
			owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_Y, true);
			break;
		case MoveDir.RIGHT_TOP:
			owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X, true);
			owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_Y, true);
			break;
        }
        //Logger.LogForAI("do walk:{0}", dir);
    }
}