using System;

/// <summary>
/// 远古-黑白球
/// 怪物召唤黑球白球，可以打破
/// 黑球白球持续向中心位移，双球一起到，全屏爆炸
/// 黑球或者白球单独移动到中心，都会给拥有者加Buff
/// </summary>
public class Mechanism10003 : BeMechanism
{
    public Mechanism10003(int mid, int lv) : base(mid, lv) { }

    int mBlackId;
    int mWhiteId;
    int mMoveTime;
    int mTimer;
    int mBlackBuffInfoId;
    int mWhiteBuffInfoId;
    int mSkillId;
    BeActor mBlackBall;
    BeActor mWhiteBall;
    VInt3 mBlackBallPos;
    VInt3 mWhiteBallPos;
    VInt3 mBlackSpeed;
    VInt3 mWhiteSpeed;

    public override void OnInit()
    {
        if (data.ValueALength > 1)
        {
            mBlackId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
            mWhiteId = TableManager.GetValueFromUnionCell(data.ValueA[1], level);
        }
        mMoveTime = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        if (data.ValueCLength > 1)
        {
            mBlackBuffInfoId = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
            mWhiteBuffInfoId = TableManager.GetValueFromUnionCell(data.ValueC[1], level);
        }
        mSkillId = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        if (data.ValueELength > 1)
        {
            if (data.ValueE[0].eValues.everyValues.Length >= 2)
            {
                mBlackBallPos.x = VInt.NewVInt(data.ValueE[0].eValues.everyValues[0], GlobalLogic.VALUE_1000).i;
                mBlackBallPos.y = VInt.NewVInt(data.ValueE[0].eValues.everyValues[1], GlobalLogic.VALUE_1000).i;
            }
            if (data.ValueE[1].eValues.everyValues.Length >= 2)
            {
                mWhiteBallPos.x = VInt.NewVInt(data.ValueE[1].eValues.everyValues[0], GlobalLogic.VALUE_1000).i;
                mWhiteBallPos.y = VInt.NewVInt(data.ValueE[1].eValues.everyValues[1], GlobalLogic.VALUE_1000).i;
            }
        }
    }

    public override void OnReset()
    {
        mBlackId = 0;
        mWhiteId = 0;
        mTimer = 0;
        mBlackBuffInfoId = 0;
        mWhiteBuffInfoId = 0;
        mBlackBall = null;
        mWhiteBall = null;
    }

    public override void OnStart()
    {
        mBlackBall = owner.CurrentBeScene.SummonMonster(mBlackId, mBlackBallPos, owner.GetCamp(), owner);
        mWhiteBall = owner.CurrentBeScene.SummonMonster(mWhiteId, mWhiteBallPos, owner.GetCamp(), owner);

        if (mBlackBall != null && mWhiteBall != null)
        {
            if (mBlackBall.aiManager != null)
            {
                mBlackBall.aiManager.Stop();
            }
            if (mWhiteBall.aiManager != null)
            {
                mWhiteBall.aiManager.Stop();
            }

            var vec = (mWhiteBall.GetPosition() - mBlackBall.GetPosition()) / 2;
            var speed = vec.NormalizeTo(vec.magnitude * VFactor.NewVFactor(GlobalLogic.VALUE_1000, mMoveTime));

            mBlackSpeed = speed;
            mWhiteSpeed = -speed;
        }

        mTimer = 0;
    }

    void DoWork()
    {
        int flag = 0;
        if (mBlackBall != null && !mBlackBall.IsDead())
        {
            flag += 1;
            mBlackBall.DoDead();
        }
        if (mWhiteBall != null && !mWhiteBall.IsDead())
        {
            flag += 2;
            mWhiteBall.DoDead();
        }

        if (flag == 1)//仅黑球
        {
            owner.buffController.TryAddBuffInfo(mBlackBuffInfoId, owner, level);
        }
        else if (flag == 2)//仅白球
        {
            owner.buffController.TryAddBuffInfo(mWhiteBuffInfoId, owner, level);
        }
        else if (flag == 3)//俩球都还在
        {
            owner.UseSkill(mSkillId, true);
        }
    }

    public override void OnUpdate(int deltaTime)
    {
        if (mTimer < mMoveTime)
        {
            mTimer += deltaTime;
            if (mTimer >= mMoveTime)
            {
                DoWork();
            }
            else
            {
                if (mBlackBall != null)
                {
                    var pos = mBlackBall.GetPosition();
                    pos += mBlackSpeed * VFactor.NewVFactor(deltaTime, 1000);
                    mBlackBall.SetPosition(pos);
                }

                if (mWhiteBall != null)
                {
                    var pos = mWhiteBall.GetPosition();
                    pos += mWhiteSpeed * VFactor.NewVFactor(deltaTime, 1000);
                    mWhiteBall.SetPosition(pos);
                }
            }
        }
    }
}
