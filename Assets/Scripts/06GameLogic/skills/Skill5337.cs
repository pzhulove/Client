using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill5337 : BeSkill
{
    private enum eState
    {
        onStart,
        onRotate,
        onMove,
        onEnd,
    }

    private int mCamp = 0;

    private eState mState = eState.onStart;

    public Skill5337(int sid, int skillLevel) : base(sid, skillLevel)
    {
        mState = eState.onStart;
    }


    public override void OnStart()
    {
        mState = eState.onStart;

        //mOnLimit = owner.RegisterEvent(BeEventType.onWalkToAreaLimit, args => 
        //{
        //    var op = (int)args[0];
        //    _changeCB(op);
        //});

        //mOnHit = owner.RegisterEvent(BeEventType.onHitOther, args =>
        //{
        //    _changeCB(0);
        //});
    }


    private void _changeCB(int op)
    {
        if (op == 0)
        {
            if (FrameRandom.Random(3) >= 1)
            {
                _changeLeftOps(1);
            }
            else
            {
                _changeLeftOps(2);
            }
        }
        else
        {
            _changeLeftOps(op);
        }
    }

    private BeActor _findActor()
    {
		List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindActorInRange(list, owner.GetPosition(), 15, mCamp);
		BeActor target = null;
        if (list.Count > 0)
        {
			target = list[0];
        }

		GamePool.ListPool<BeActor>.Release(list);

		return target;
    }

    private int mFindTargetDelta = 10000;

    private int mFindTargetTime = 0;

    private BeActor mTarget = null;

    private Queue<MoveOp> mMoveOpQueue = new Queue<MoveOp>();

    private struct MoveOp
    {
        public short cx;
        public short cy;
    }

    public override void OnUpdate(int iDeltime)
    {
        if (mState == eState.onMove)
        {
            // maybe change the target
            mFindTargetTime += iDeltime;
            if (mFindTargetTime > mFindTargetDelta)
            {
                mFindTargetTime = 0;
                mTarget = _findActor();
            }

            _moveUpdate();
            _checkChanageUpdate();
        }
    }

    private static readonly VInt kDelta = VInt.half;

    private void _checkChanageUpdate()
    {
        var npos = owner.GetPosition();

        int dir = 0;
        npos.x += kDelta.i;
        if (owner.CurrentBeScene.IsInBlockPlayer(npos))
        {
            dir += 2;
        }

        npos.x -= 2 * kDelta.i;
        if (owner.CurrentBeScene.IsInBlockPlayer(npos))
        {
            dir += 1;
        }
        npos.x += kDelta.i;

        npos.y += kDelta.i;
        if (owner.CurrentBeScene.IsInBlockPlayer(npos))
        {
            dir += 4;
        }

        npos.y -= 2 * kDelta.i;
        if (owner.CurrentBeScene.IsInBlockPlayer(npos))
        {
            dir += 8;
        }
        npos.y += kDelta.i;

        if (dir > 0)
        {
            _changeCB(dir);
        }
    }

    private int mTotalStep = 300;

    private void _move(VInt3 pos)
    {
        VFactor rate = new VFactor(pos.x,pos.y);
        VFactor low = new VFactor(33,100);
        VFactor high = new VFactor(400,100);

        if(rate < low)
        {
            rate = low;
        }

        if(rate > high)
        {
            rate = high;
        }

        //rate = Mathf.Clamp(rate, 0.33f, 4f);

        var stepx = (VFactor.one + rate).integer;
        var stepy = ((rate + VFactor.one) / rate).integer;

        for (int i = 0; i < mTotalStep; i++)
        {
            MoveOp op;

            op.cx = -1;
            op.cy = -1;

            if (i % stepx == 0)
            {
                if (pos.x > VInt.zeroDotOne)
                {
                    op.cx = (short)CommandMove.COMMAND_MOVE_X;
                }
                else if (pos.x < VInt.zeroDotOne)
                {
                    op.cx = (short)CommandMove.COMMAND_MOVE_X_NEG;
                }
            }

            if (i % stepy == 0)
            {
                if (pos.y > VInt.zeroDotOne)
                {
                    op.cy = (short)CommandMove.COMMAND_MOVE_Y;
                }
                else if (pos.y < VInt.zeroDotOne)
                {
                    op.cy = (short)CommandMove.COMMAND_MOVE_Y_NEG;
                }
            }

            mMoveOpQueue.Enqueue(op);
        }
    }

    private void _moveUpdate()
    {
        if (mMoveOpQueue.Count > 0)
        {
            var op = mMoveOpQueue.Dequeue();

            owner.ResetMoveCmd();

            if (op.cx == (short)CommandMove.COMMAND_MOVE_X)
            {
                owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X, true);
            }
            else if (op.cx == (short)CommandMove.COMMAND_MOVE_X_NEG)
            {
                owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X_NEG, true);
            }

            if (op.cy == (short)CommandMove.COMMAND_MOVE_Y)
            {
                owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_Y, true);
            }
            else if (op.cy == (short)CommandMove.COMMAND_MOVE_Y_NEG)
            {
                owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_Y_NEG, true);
            }
        }
        else
        {
            _move(mTarget.GetPosition() - owner.GetPosition());
        }
    }

    private void _changeLeftOps(int changex)
    {
        var ops = mMoveOpQueue.ToArray();

        mMoveOpQueue.Clear();

        for (int i = 0; i < ops.Length; ++i)
        {
            var op = ops[i];

            if ((changex & 1) > 0)
            {
                if (op.cx == (short)CommandMove.COMMAND_MOVE_X_NEG)
                {
                    op.cx = (short)CommandMove.COMMAND_MOVE_X;
                }
            }

            if ((changex & 2) > 0)
            {
                if (op.cx == (short)CommandMove.COMMAND_MOVE_X)
                {
                    op.cx = (short)CommandMove.COMMAND_MOVE_X_NEG;
                }
            }

            if ((changex & 4) > 0)
            {
                if (op.cy == (short)CommandMove.COMMAND_MOVE_Y)
                {
                    op.cy = (short)CommandMove.COMMAND_MOVE_Y_NEG;
                }
            }

            if ((changex & 8) > 0)
            { 
                if (op.cy == (short)CommandMove.COMMAND_MOVE_Y_NEG)
                {
                    op.cy = (short)CommandMove.COMMAND_MOVE_Y;
                }
            }

            mMoveOpQueue.Enqueue(op);
        }
    }

    public override void OnFinish()
    {
        _end();
    }

    public override void OnCancel()
    {
        _end();
    }

    private void _end()
    {
        mState = eState.onEnd;
        mMoveOpQueue.Clear();
        owner.ResetMoveCmd();
        owner.aiManager.Start();
    }

    public override void OnEnterPhase(int phase)
    {
        if (phase == 1)
        {
            mState = eState.onRotate;
        }
        else if (phase == 2)
        {
            mState = eState.onMove;

            mTarget = _findActor();
            owner.aiManager.Stop();
        }
        else if (phase == 3)
        {
            _end();
        }
    }
}
