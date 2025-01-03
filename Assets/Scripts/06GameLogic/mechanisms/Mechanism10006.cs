using System;
using System.Collections.Generic;

/// <summary>
/// 怪物开场动作机制
/// 怪物会在开场播一个特定的动作，当玩家靠近一定距离时会播一个触发动作，并开启AI
/// 相同组ID的怪只要有一个触发了，其他怪也会触发
/// </summary>
public class Mechanism10006 : BeMechanism
{
    string mActionName;
    string mTriggerActionName;
    int mTriggerDistance;
    int mSkillId;
    int mBuffId;

    bool mIsTriggered;
    int mTimer;

    List<BeActor> mTargetList = new List<BeActor>();
    BeActor mTarget;

    public Mechanism10006(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        mActionName = data.StringValueA[0];
        if (data.StringValueALength > 1)
        {
            mTriggerActionName = data.StringValueA[1];
        }
        mTriggerDistance = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[0], level), GlobalLogic.VALUE_1000).i;
        mSkillId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        mBuffId = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
    }

    public override void OnReset()
    {
        mActionName = "";
        mTriggerActionName = "";
        mTriggerDistance = 0;
        mSkillId = 0;
        mIsTriggered = false;
        mTimer = 0;
        mTarget = null;
        mTargetList.Clear();
    }

    public override void OnStart()
    {
        mIsTriggered = false;

        sceneHandleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.AfterOnReady, args =>
        {
            if (owner.aiManager != null)
            {
                owner.aiManager.Stop();
            }

            owner.ClearMoveSpeed();

            if (owner.HasAction(mActionName))
            {
                owner.PlayAction(mActionName);
            }

            handleA = owner.RegisterEventNew(BeEventType.onHit, eventParam =>
            {
                owner.RemoveMechanism(this);

                if (owner.GetEntityData().groupID != 0)
                {
                    owner.CurrentBeScene.TriggerEventNew(BeEventSceneType.onGroupAction, new GameClient.EventParam() { m_Int = owner.GetEntityData().groupID, m_Obj = eventParam.m_Obj });
                }
            });

            handleB = owner.RegisterEventNew(BeEventType.onAddBuff, _args =>
            {
                var buff = _args.m_Obj as BeBuff;
                if (buff.buffID == mBuffId)
                {
                    BeActor target = null;
                    if (buff.releaser != null && buff.releaser.aiManager.aiTarget != null)
                    {
                        target = buff.releaser.aiManager.aiTarget;
                    }
                    else if (mTargetList.Count > 0)
                    {
                        target = mTargetList[0];
                    }
                    CheckState(target);
                }
            });

            sceneHandleB = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onGroupAction, eventParam =>
            {
                var groupId = eventParam.m_Int;
                if (owner.GetEntityData().groupID == groupId)
                {
                    TriggerAction(eventParam.m_Obj as BeActor);
                }
            });

            owner.CurrentBeScene.FindMainActor(mTargetList);
        });

    }

    public override void OnUpdate(int deltaTime)
    {
        if (!mIsTriggered)
        {
            for (int i = 0; i < mTargetList.Count; i++)
            {
                if (Math.Abs(mTargetList[i].GetPosition().x - owner.GetPosition().x) <= mTriggerDistance)
                {
                    CheckState(mTargetList[i]);
                }
            }
        }
        else
        {
            mTimer -= deltaTime;
            if (mTimer <= 0)
            {
                if (mTarget != null)
                {
                    owner.SetFace(mTarget.GetPosition().x < owner.GetPosition().x);
                }
                if (mSkillId > 0)
                {
                    owner.UseSkill(mSkillId);
                }
                owner.RemoveMechanism(this);
            }
        }
    }

    void CheckState(BeActor target)
    {
        if (owner.GetEntityData().groupID == 0)
        {
            TriggerAction(target);
        }
        else
        {
            owner.CurrentBeScene.TriggerEventNew(BeEventSceneType.onGroupAction, new GameClient.EventParam() { m_Int = owner.GetEntityData().groupID, m_Obj = target });
        }
    }

    void TriggerAction(BeActor target)
    {
        if (mIsTriggered)
        {
            return;
        }

        mTarget = target;
        mIsTriggered = true;
        if (!owner.IsDead() &&
            !owner.IsInPassiveState() && 
            owner.HasAction(mTriggerActionName))
        {
            owner.GetStateGraph().ResetCurrentStateTime(true);
            owner.PlayAction(mTriggerActionName);
            if (mSkillId > 0)
            {
                mTimer = owner.GetCurrentActionDuration();
            }
            else
            {
                mTimer = 0;
            }
        }
    }

    public override void OnFinish()
    {
        if (owner.aiManager != null)
        {
            owner.aiManager.Start();
        }
        mTargetList.Clear();
    }
}
