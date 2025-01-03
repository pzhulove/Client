using System;
using GameClient;

/// <summary>
/// 远古-火原力
/// 怪物被击会增加原力，每秒增加有上限，超过若干秒不攻击会持续下降
/// 原力值叠满会释放技能
/// </summary>
public class Mechanism10004 : BeMechanism
{
    public Mechanism10004(int mid, int lv) : base(mid, lv) { }

    int mMaxForceValue;         //原力值上限
    int mAddValue;              //每次被击增加原力值
    int mMaxValuePerSecond;     //每秒增加原力值上限
    int mStayTime;              //持续多长时间不被击会下降
    int mReduceValue;           //每秒减少原力值
    int mSkillId;               //原力值满了释放技能

    int mCurValue;
    int mLastValue;
    int mRecordValue;
    int mTimer;
    int mTimerSecond;
    bool mIsCooling;

    public override void OnInit()
    {
        mMaxForceValue = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        mAddValue = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        mMaxValuePerSecond = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        mStayTime = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        mReduceValue = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        mSkillId = TableManager.GetValueFromUnionCell(data.ValueF[0], level);
    }

    public override void OnReset()
    {
        mCurValue = 0;
        mLastValue = 0;
        mRecordValue = 0;
        mTimer = 0;
        mTimerSecond = 0;
        mIsCooling = false;
    }

    public override void OnStart()
    {
        mCurValue = 0;
        mTimer = 0;
        mTimerSecond = 1000;
        mIsCooling = false;
        owner.RegisterEventNew(BeEventType.onHit, _onHit);

        var bar = owner.StartSpellBar(eDungeonCharactorBar.MonsterEnergyBar, int.MaxValue);
        bar.acc = int.MaxValue / 1000;
    }

    private void _onHit(BeEvent.BeEventParam args)
    {
        if (mCurValue + mAddValue - mRecordValue <= mMaxValuePerSecond)
        {
            mCurValue += mAddValue;

            if (mCurValue >= mMaxForceValue)
            {
                mCurValue = 0;

                owner.UseSkill(mSkillId, true);
            }
        }

        mIsCooling = false;
        mTimer = 0;

        SetValue();
    }

    void SetValue()
    {
        if (mLastValue != mCurValue)
        {
            var value = (float)(mCurValue - mLastValue) / mMaxForceValue;
            owner.AddSpellBarProgress(eDungeonCharactorBar.MonsterEnergyBar, value);
        }
    }

    public override void OnUpdate(int deltaTime)
    {
        mTimerSecond -= deltaTime;
        if (mTimerSecond <= 0)
        {
            mRecordValue = mCurValue;

            mTimerSecond = 1000;
        }

        mLastValue = mCurValue;

        if (!mIsCooling)
        {
            mTimer += deltaTime;
            if (mTimer >= mStayTime)
            {
                mIsCooling = true;
                mTimer = 0;
            }
        }
        else
        {
            mTimer += deltaTime;
            if (mTimer >= 1000)
            {
                mCurValue -= mReduceValue;
                mTimer -= 1000;

                if (mCurValue < 0)
                {
                    mCurValue = 0;
                }

                SetValue();
            }
        }
    }
}
