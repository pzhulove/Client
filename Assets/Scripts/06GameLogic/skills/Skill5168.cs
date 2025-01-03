using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

public class Skill5161 : Skill5168
{
    public Skill5161(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }
}

public class Skill5168 : BeSkill
{
    protected int mDestructID = -1;

    protected int mDamageCount = 1;

    protected int mEffectID = -1;

    private const int cRange = 20;

    public Skill5168(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }

    public override void OnInit()
    {
        if (skillData != null)
        {
            mDestructID = TableManager.GetValueFromUnionCell(skillData.ValueA[0], 1);
            mDamageCount = TableManager.GetValueFromUnionCell(skillData.ValueB[0], 1);
        }
    }

    public override void OnPostInit()
    {
    }


    public override void OnStart()
    {
        var target = _findTarget();
        if (null != target )
        {
            _triggerEffect(target);
        }
    }

    private BeActor _findTarget()
    {
        var target = owner.CurrentBeScene.FindNearestFacedTarget(owner, new VInt2(10, 10));
        return target;
    }

    protected IBeEventHandle mHandle;
    protected IBeEventHandle mHandleTouchGround;

    private enum eState
    {
        None,
        Fall,
        Traped,
        Broken,
    }

    private eState mState = eState.None;

    private BeActor _createMonster(BeActor actor)
    {
        owner.RegisterEventNew(BeEventType.onSummon, args =>
        {
            BeActor summon = args.m_Obj as BeActor;
            int id = (int)args.m_Int;

            if (null != summon && skillData.ID == id)
            {
                var pos = actor.GetPosition();
                pos.z = VInt.one.i * 5;
                summon.SetPosition(pos);
                summon.hasAI = false;

                summon.stateController.SetAbilityEnable(BeAbilityType.FLOAT, false);
                summon.stateController.SetAbilityEnable(BeAbilityType.BEGRAB, false);

                mState = eState.Fall;

                mHandle = summon.RegisterEventNew(BeEventType.onHitOther, hitOther =>
                //mHandle = summon.RegisterEvent(BeEventType.onHitOther, hitOther =>
                {
                    if (mState == eState.Fall)
                    {
                        var targetActor = hitOther.m_Obj as BeActor;
                        if (targetActor == actor)
                        {
                            mState = eState.Traped;
                            pos.z = 0;
                            actor.SetPosition(pos);
                            _setBlockLayer(actor, true);

                            if (mHandle != null)
                            {
                                mHandle.Remove();
                            }
                        }
                    }
                });

                mHandleTouchGround = summon.RegisterEventNew(BeEventType.onTouchGround, uarsg =>
                {
                    if (!summon.IsDead() && mState == eState.Fall)
                    {
                        mState = eState.Broken;
                        summon.DoDead();

                        _setBlockLayer(actor, false);

                        if (null == mHandleTouchGround)
                        {
                            mHandleTouchGround.Remove();
                        }
                    }
                });


                summon.RegisterEventNew(BeEventType.onDead, eventParam =>
                {
                    if (mState == eState.Traped)
                    {
                        _setBlockLayer(actor, false);
                        _untriggerEffect(actor);
                    }
                });
            }
        });

        return null;
    }

    private void _triggerEffect(BeActor actor)
    {
        _createMonster(actor);
    }

    private void _untriggerEffect(BeActor actor)
    {
        var data = TableManager.instance.GetTableItem<ProtoTable.EffectTable>(51681);
        if (null != data)
        {
            var buff = actor.buffController.HasBuffByID(data.BuffID);
            if (null != buff)
            {
                buff.Finish();
            }
        }
    }

    private void _setBlockLayer(BeActor summon, bool block = true)
    {
        var currentBeScene = owner.CurrentBeScene;

        if (currentBeScene == null)
            return;

        DGrid grid = currentBeScene.CalGridByPosition(summon.GetPosition());
        DGrid blockRect = new DGrid(3, 3);

        //Logger.LogErrorFormat("grid.x {0}, grid.y {1}", grid.x, grid.y);

        int logicX = grid.x;
        int logicZ = grid.y;

        int startX = logicX - blockRect.x / 2;
        int startZ = logicZ - blockRect.y / 2;

        for (int i = startX; i < startX + blockRect.x; ++i)
        {
            for (int j = startZ; j < startZ + blockRect.y; ++j)
            {
                currentBeScene.SetBlock(new DGrid(i, j), block);
            }
        }
    }
}

