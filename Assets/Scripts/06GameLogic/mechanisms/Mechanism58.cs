using UnityEngine;
using System.Collections.Generic;

/*
 * 布法罗乱跑机制
*/
public class Mechanism58 : BeMechanism
{
    int mDealIntervel = 1000;
    int mDealCount = 0;
    VInt3 mPos;


    public Mechanism58(int mid, int lv) : base(mid, lv){}

    public override void OnReset()
    {
        mDealIntervel = 1000;
        mDealCount = 0;
        mPos = VInt3.zero;
    }

    public override void OnInit()
    {
        if (data.ValueA.Count > 0)
        {
            mDealIntervel = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        }
    }

    public override void OnStart()
    {
        owner.aiManager.Stop();
        owner.ChangeRunMode(true);
    }

    void MoveTo(int delta)
    {
        mDealCount += delta;
        if (mDealCount > mDealIntervel)
        {
            var pos = owner.CurrentBeScene.GetLogicPosInRange(owner, GlobalLogic.VALUE_20000);
            mPos = pos - owner.GetPosition();
            mDealCount = 0;
            
            owner.ResetMoveCmd();

            if (mPos.x > VInt.zeroDotOne)
            {
                owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X, true);
                ChangeAnimation();
            }
            else if (mPos.x < -VInt.zeroDotOne)
            {
                owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X_NEG, true);
                ChangeAnimation();
            }

            if (mPos.y > VInt.zeroDotOne)
            {
                owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_Y, true);
                ChangeAnimation();
            }
            else if (mPos.y < -VInt.zeroDotOne)
            {
                owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_Y_NEG, true);
                ChangeAnimation();
            }
        }
    }

    void ChangeAnimation()
    {
#if !LOGIC_SERVER
        owner.m_pkGeActor.ChangeAction("Anim_Zhuangji_02", 0.25f, true);
#endif
    }

    public override void OnUpdate(int deltaTime)
    {
        if (null != owner && !owner.IsDead() && !owner.IsInPassiveState())
        {
            MoveTo(deltaTime);
        }
    }

    public override void OnFinish()
    {
        if (!owner.IsDead())
        {
            owner.GetEntityData().SetHP(-1);
            owner.DoDead();
        }
    }
}
