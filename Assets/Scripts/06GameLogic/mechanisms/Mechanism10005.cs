using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 连续多段冲锋机制
/// 可以配置不同段数的连续冲锋，有预警特效
/// </summary>
public class Mechanism10005 : BeMechanism
{
    int mAssaultCount;          //冲锋次数
    int[] mThinkTargetTime;     //选目标时间
    int[] mAssaultDistance;     //冲锋距离
    int[] mAssaultTime;         //冲锋时间
    string mEffectPath;         //预警特效路径

    int mTimer;
    VInt3[] mMoveSpeed;
    VInt3 mOwnerPos;
    BeActor mTarget;
    int mAssaultIndex;
    Assault_State mState;
    List<GeEffectEx> mListEffect = new List<GeEffectEx>();

    public Mechanism10005(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        mEffectPath = data.StringValueA[0];

        mAssaultCount = TableManager.GetValueFromUnionCell(data.ValueA[0], level);

        mThinkTargetTime = new int[mAssaultCount];
        mAssaultDistance = new int[mAssaultCount];
        mAssaultTime = new int[mAssaultCount];
        for (int i = 0; i < mAssaultCount; i++)
        {
            int index = 0;//数据个数不够冲锋次数的时候，取第0个的值

            if (i < data.ValueBLength)
                index = i;
            mThinkTargetTime[i] = TableManager.GetValueFromUnionCell(data.ValueB[index], level);

            if (i < data.ValueCLength)
                index = i;
            mAssaultDistance[i] = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueC[index], level), GlobalLogic.VALUE_1000).i;

            if (i < data.ValueDLength)
                index = i;
            mAssaultTime[i] = TableManager.GetValueFromUnionCell(data.ValueD[index], level);
        }
    }

    public override void OnReset()
    {
        mTimer = 0;
        mMoveSpeed = null;
        mOwnerPos = VInt3.zero;
        mTarget = null;
        mAssaultIndex = -1;
        mState = Assault_State.Init;
        mListEffect.Clear();
    }

    public override void OnStart()
    {
        mState = Assault_State.Prepare;
        mAssaultIndex = -1;
        mTimer = 0;
        mMoveSpeed = new VInt3[mAssaultCount];
        mOwnerPos = owner.GetPosition();
        mListEffect.Clear();

        handleA = owner.RegisterEventNew(BeEventType.onHit, eventParam =>
        {
            mState = Assault_State.StopAction;
            owner.ClearMoveSpeed();
        });
    }

    public override void OnUpdate(int deltaTime)
    {
        mTimer += deltaTime;
        if (mState == Assault_State.Prepare)
        {
            if (mAssaultIndex == -1 || mTimer >= mThinkTargetTime[mAssaultIndex])
            {
                mTimer = 0;
                mAssaultIndex++;
                
                if (mAssaultIndex >= mAssaultCount)
                {
                    mState = Assault_State.Running;
                    mAssaultIndex = -1;

                    for (int i = 0; i < mListEffect.Count; i++)
                    {
                        mListEffect[i].Remove();
                    }
                    mListEffect.Clear();

                    return;
                }

                //只找一次目标
                if (mTarget == null)
                {
                    mTarget = owner.CurrentBeScene.FindTarget(owner, VInt.NewVInt(100000, GlobalLogic.VALUE_1000));
                }
                if (mTarget != null)
                {
                    var vec = mTarget.GetPosition() - mOwnerPos;
                    vec = vec.NormalizeTo(mAssaultDistance[mAssaultIndex]);
                    mMoveSpeed[mAssaultIndex] = vec * VFactor.NewVFactor(GlobalLogic.VALUE_1000, mAssaultTime[mAssaultIndex]);

                    var lastPos = mOwnerPos;

                    //计算出不超边界的最后位置
                    mOwnerPos = mOwnerPos + vec;
                    if (mOwnerPos.x < owner.CurrentBeScene.logicXSize.x)
                        mOwnerPos.x = owner.CurrentBeScene.logicXSize.x;
                    if (mOwnerPos.x > owner.CurrentBeScene.logicXSize.y)
                        mOwnerPos.x = owner.CurrentBeScene.logicXSize.y;
                    if (mOwnerPos.y < owner.CurrentBeScene.logicZSize.x)
                        mOwnerPos.y = owner.CurrentBeScene.logicZSize.x;
                    if (mOwnerPos.y > owner.CurrentBeScene.logicZSize.y)
                        mOwnerPos.y = owner.CurrentBeScene.logicZSize.y;

                    //用实际能到达的位置去显示预警特效
                    CreateEffect(lastPos, mOwnerPos, mAssaultDistance[mAssaultIndex]);
                }
            }
        }
        else if (mState == Assault_State.Running)
        {
            if (mAssaultIndex == -1 || mTimer >= mAssaultTime[mAssaultIndex])
            {
                mTimer = 0;
                mAssaultIndex++;

                if (mAssaultIndex >= mAssaultCount)
                {
                    mState = Assault_State.StopAction;
                    owner.ClearMoveSpeed();
                    return;
                }

                owner.SetFace(mMoveSpeed[mAssaultIndex].x < 0);
                owner.SetMoveSpeedX(mMoveSpeed[mAssaultIndex].x);
                owner.SetMoveSpeedY(mMoveSpeed[mAssaultIndex].y);
            }
        }
    }

    void CreateEffect(VInt3 start, VInt3 end, int distance)
    {
#if !LOGIC_SERVER
        var vec = end.vector3 - start.vector3;
        var angle = Vector3.Angle(Vector3.forward, vec);//这里根据具体特效修改 meybe Vector3.left？
        var cross = Vector3.Cross(Vector3.forward, vec);//这里根据具体特效修改 meybe Vector3.left？

        var y = cross.normalized.y;
        if (y != 0) angle *= y;

        var effect = owner.CurrentBeScene.currentGeScene.CreateEffect(mEffectPath, 999, start.vec3);
        if (effect != null)
        {
            effect.GetRootNode().transform.eulerAngles = Vector3.up * angle;
            effect.GetRootNode().transform.localScale = new Vector3(1f, 1f, distance / 10000f);
            mListEffect.Add(effect);
        }
#endif
    }

    public override void OnFinish()
    {
        mListEffect.Clear();
    }
}
