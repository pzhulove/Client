using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill3220 : BeSkill
{
    class TargetMovement
    {
        public BeActor mTarget;
        public VInt3 mOneStep;
        public VInt3 mOriginPos;
        public int mMoveDistance;
        public bool mIsReachedPos;
    }

    class TargetManager
    {
        Dictionary<int, TargetMovement> mTargetMovementDict = new Dictionary<int, TargetMovement>();
        VInt3 mTargetPos;
        int mPushSpeed;

        public void SetTargetPos(VInt3 targetPos)
        {
            mTargetPos = targetPos;
        }

        public void SetPushSpeed(int speed)
        {
            mPushSpeed = speed;
        }

        public void Clear()
        {
            mTargetMovementDict.Clear();
        }

        public void Update(int deltaTime)
        {
            foreach (var pid in mTargetMovementDict.Keys)
            {
                var move = mTargetMovementDict.SafeGetValue(pid);
                if (move.mTarget != null && !move.mTarget.IsDead() && !move.mIsReachedPos)
                {
                    var vec = move.mOneStep;
                    var dis = mPushSpeed * VFactor.NewVFactor(deltaTime, 1000);
                    vec.NormalizeTo(dis);
                    var pos = move.mTarget.GetPosition() + vec;
                    if ((move.mOriginPos - pos).magnitude >= move.mMoveDistance)
                    {
                        pos = mTargetPos;
                        move.mIsReachedPos = true;
                    }
                    move.mTarget.SetPosition(pos);
                }
            }
        }

        public void AddTargetMovement(TargetMovement move)
        {
            if (move.mTarget != null && !mTargetMovementDict.ContainsKey(move.mTarget.GetPID()))
            {
                mTargetMovementDict.Add(move.mTarget.GetPID(), move);
            }
        }

        public bool IsHitTarget(BeActor target)
        {
            return mTargetMovementDict.ContainsKey(target.GetPID());
        }

        public bool IsEveryTargetDead()
        {
            foreach (var pid in mTargetMovementDict.Keys)
            {
                var move = mTargetMovementDict.SafeGetValue(pid);
                if (move.mTarget != null && !move.mTarget.IsDead())
                {
                    return false;
                }
            }
            return true;
        }
    }

    int mEffectInfoId = 1045;
    int[] mSkillPhaseIds = new int[] { 32201, 32202, 32203, 32204 };
    int mStartSkillPhaseId = 3220;
    int mEndSkillPhaseId = 32205;
    int mActionCount = 5;
    int mHurtId1 = 32100;
    int mHurtId2 = 32201;
    VInt3 mTargetPos;
    bool mHitOther;
    bool mIsEnd;
    TargetManager mTargetManager;
     
    int mTargetDistance;
    int mCheckDistance;
    int mPushSpeed;
    int mAttackPosOffset;
    int mFinalPosOffset;
    int mInvRatio;
    float mEffectMinScale;

    public Skill3220(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnPostInit()
    {
        mTargetDistance = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueA[0], level), GlobalLogic.VALUE_1000).i;
        mCheckDistance = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueB[0], level), GlobalLogic.VALUE_1000).i;
        mPushSpeed = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueC[0], level), GlobalLogic.VALUE_1000).i;
        mAttackPosOffset = -VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueD[0], level), GlobalLogic.VALUE_1000).i;
        mFinalPosOffset = -VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueE[0], level), GlobalLogic.VALUE_1000).i;
        mInvRatio = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueF[0], level), GlobalLogic.VALUE_1000).i;
        mEffectMinScale = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueG[0], level), GlobalLogic.VALUE_1000).scalar;
    }

    public override void OnStart()
    {
        mHitOther = false;
        mIsEnd = false;
        mTargetManager = new TargetManager();
        mTargetManager.SetPushSpeed(mPushSpeed);

        handleA = owner.RegisterEventNew(BeEventType.onHitOther, param =>
        {
            var target = param.m_Obj as BeActor;
            var hurtId = param.m_Int;
            if (target != null)
            {
                if (!mHitOther && hurtId == mHurtId1)
                {
                    mTargetPos = owner.GetPosition();
                    SetTargetPos(target);
                    mHitOther = true;
                }
                mTargetManager.AddTargetMovement(GetMovement(target));
            }
        });

        handleB = owner.RegisterEventNew(BeEventType.onSkillCurFrame, param =>
        {
            var tag = param.m_String;
            if (tag == "Next" && mHitOther)
            {
                int[] skillPhases = new int[mActionCount + 2];
                skillPhases[0] = mStartSkillPhaseId;
                int index = owner.FrameRandom.Random((uint)mSkillPhaseIds.Length);
                for (int i = 1; i <= mActionCount; i++)
                {
                    index = (index + owner.FrameRandom.InRange(1, mSkillPhaseIds.Length)) % mSkillPhaseIds.Length;
                    skillPhases[i] = mSkillPhaseIds[index];
                }
                skillPhases[mActionCount + 1] = mEndSkillPhaseId;
                owner.skillController.SetCurrentSkillPhases(skillPhases);
                (owner.GetStateGraph() as BeActorStateGraph).ExecuteNextPhaseSkill();
            }
        });
    }

    public override void OnEnterPhase(int phase)
    {
        if (!mHitOther)
        {
            return;
        }

        if (mIsEnd)
        {
            return;
        }

        if (phase > 1 && phase < mActionCount + 2)
        {
            var flag = FindNextTarget();
            if (!flag)
            {
                if (mTargetManager.IsEveryTargetDead())
                {
                    owner.Locomote(new BeStateData((int)ActionState.AS_IDLE));
                }
                else
                {
                    mIsEnd = true;

                    int[] skillPhases = new int[phase + 1];
                    skillPhases[phase] = mEndSkillPhaseId;
                    owner.skillController.SetCurrentSkillPhases(skillPhases);
                    owner.delayCaller.DelayCall(1, () =>
                    {
                        (owner.GetStateGraph() as BeActorStateGraph).ExecuteNextPhaseSkill();
                    });
                }
            }
        }
        else if (phase == owner.skillController.SkillPhaseArray.Length)
        {
            var pos = mTargetPos + new VInt3(mFinalPosOffset * owner._getFaceCoff(), 0, 0);
            pos = BeAIManager.FindStandPositionNew(pos, owner.CurrentBeScene, false, false, 30);
            var lastPos = owner.GetPosition();
            owner.SetPosition(pos);
            CreateTrailEffect(pos, lastPos);
        }
    }

    TargetMovement GetMovement(BeActor target)
    {
        var movement = new TargetMovement();
        movement.mIsReachedPos = false;
        movement.mTarget = target;
        movement.mOriginPos = target.GetPosition();
        var vec = mTargetPos - target.GetPosition();
        movement.mMoveDistance = vec.magnitude;
        movement.mOneStep = vec;

        return movement;
    }

    void SetTargetPos(BeActor target)
    {
        if (target != null)
        {
            mTargetPos.x += owner._getFaceCoff() * mTargetDistance;
            mTargetPos = BeAIManager.FindStandPositionNew(mTargetPos, owner.CurrentBeScene);
            mTargetManager.SetTargetPos(mTargetPos);
        }
    }

    bool FindNextTarget()
    {
        int tempDistance = 0;
        BeActor tempTarget = null;
        var targets = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindAllMonsters(targets, owner);
        for (int i = 0; i < targets.Count; i++)
        {
            var target = targets[i];
            if (target != null &&
                !target.IsDead() &&
                !target.stateController.HasState(BeStateType.FALLGROUND) &&
                !mTargetManager.IsHitTarget(target))
            {
                var distance = (target.GetPosition() - mTargetPos).magnitude;
                if (tempDistance < distance && distance < mCheckDistance)
                {
                    tempDistance = distance;
                    tempTarget = target;
                }
            }
        }
        GamePool.ListPool<BeActor>.Release(targets);

        if (tempTarget != null)
        {
            var face = tempTarget.GetPosition().x > mTargetPos.x;
            owner.SetFace(face);
            var pos = tempTarget.GetPosition() + new VInt3(mAttackPosOffset * owner._getFaceCoff(), 0, 0);
            var lastPos = owner.GetPosition();
            owner.SetPosition(pos);
            CreateTrailEffect(pos, lastPos);
            return true;
        }
        else
        {
            var pos = mTargetPos + new VInt3(mFinalPosOffset * owner._getFaceCoff(), 0, 0);
            pos = BeAIManager.FindStandPositionNew(pos, owner.CurrentBeScene, false, false, 30);
            var lastPos = owner.GetPosition();
            owner.SetPosition(pos);
            CreateTrailEffect(pos, lastPos);
            return false;
        }
    }

    void CreateTrailEffect(VInt3 start, VInt3 end, int zOffset = 6000)
    {
#if !LOGIC_SERVER

        start.z += zOffset;
        end.z += zOffset;
        start.y -= 2000;
        end.y -= 2000;

        var vec = end.vector3 - start.vector3;
        var angle = Vector3.Angle(Vector3.left, vec);
        var cross = Vector3.Cross(Vector3.left, vec);

        var y = cross.normalized.y;
        if (y != 0) angle *= y;

        //特效至少要带一点角度才好看
        if (80 < angle && angle <= 90)
            angle = 80;
        else if (90 < angle && angle < 100)
            angle = 100;
        else if (-90 <= angle && angle < -80)
            angle = -80;
        else if (-100 < angle && angle < -90)
            angle = -100;

        var effect = owner.CurrentBeScene.currentGeScene.CreateEffect(mEffectInfoId, ((start + end) / 2).vec3);
        effect.GetRootNode().transform.eulerAngles = Vector3.up * angle;
        float scale = (start - end).magnitude / (float)mInvRatio;
        if (scale < mEffectMinScale) scale = mEffectMinScale;
        effect.GetRootNode().transform.localScale = new Vector3(scale, 1f, 1f);
#endif
    }

    public override void OnUpdate(int iDeltime)
    {
        mTargetManager.Update(iDeltime);
    }

    public override void OnFinish()
    {
        if (mTargetManager != null)
        {
            mTargetManager.Clear();
            mTargetManager = null;
        }
    }
}
