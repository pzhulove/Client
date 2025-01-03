using System.Collections.Generic;
using GameClient;
using UnityEngine.EventSystems;

/// <summary>
/// 药剂师:宝宝在idleMode（无攻击目标）指定怪物周边寻路
/// 有攻击目标的AI逻辑配置在行为树里
///
/// 配置
/// A:怪物ID
/// B:怪物身上的BuffInfo
/// </summary>
public class Mechanism3003 : BeMechanism
{
    public Mechanism3003(int mid, int lv) : base(mid, lv) { }

    private int mMonsterId;
    private int mBuffInfoId;
    private int mRadius;
    private BeAIManager.IdleMode mPreMode;
    private BeActor mPreFollow;
    private bool isStartCustom = false;
    private BeActor mTargetActor;
    
    public override void OnInit()
    {
        base.OnInit();
        mMonsterId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        mBuffInfoId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnReset()
    {
        mRadius = 0;
        mPreMode = BeAIManager.IdleMode.IDLE;
        mPreFollow = null;
        isStartCustom = false;
        mTargetActor = null;
    }

    public override void OnStart()
    {
        base.OnStart();
        SaveAIConfig();
        handleA = owner.RegisterEventNew(BeEventType.onAIMoveEnd, OnAIMoveEnd);
        if (owner.CurrentBeScene != null)
        {
            sceneHandleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onSummon, OnSummon);
            sceneHandleB = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onEntityRemove, OnRemove);
            CheckStartCustomIdleMode();
        }
    }

    private void SaveAIConfig()
    {
        mPreFollow = owner.aiManager.followTarget;
        mPreMode = owner.aiManager.idleMode;
    }
    
    private void CheckStartCustomIdleMode()
    {
        var list = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene?.FindMonsterByID(list, mMonsterId, false);
        for (int i = 0; i < list.Count; i++)
        {
            var monster = list[i];
            if (monster.IsSameTopOwner(owner))
            {
                StartCustomIdleMode(monster);
                break;
            }
        }
        GamePool.ListPool<BeActor>.Release(list);
    }

    private int GetRadius(BeActor monster)
    {
        if (monster == null)
            return 0;
        
        var buffInfo = monster.buffController?.GetTriggerBuff(new BuffInfoData(mBuffInfoId));
        if (buffInfo != null)
        {
            return buffInfo.buffRangeRadius;
        }
        return 0;
    }

    private void OnAIMoveEnd(BeEvent.BeEventParam param)
    {
        if(!isStartCustom)
            return;
        
        if (owner.aiManager != null)
        {
            owner.aiManager.ResetDestinationSelect();
        }
    }

    private void OnSummon(BeEvent.BeEventParam args)
    {
        var monster = (BeActor) args.m_Obj;
        if (monster != null && monster.attribute.MonsterIDEqual(mMonsterId) && monster.IsSameTopOwner(owner))
        {
            StartCustomIdleMode(monster);

            monster.RegisterEventNew(BeEventType.onChangeBuffRangeRadius, OnAddBuffInfo);
        }
    }

    private void OnAddBuffInfo(BeEvent.BeEventParam args)
    {
        if (args.m_Int == mBuffInfoId)
        {
            if (owner.aiManager != null)
            {
                owner.aiManager.radius = GetRadius(mTargetActor);
            }
        }
    }


    private void OnRemove(BeEvent.BeEventParam param)
    {
        var monster = param.m_Obj as BeActor;
        if (monster != null)
        {
            if (monster.attribute.MonsterIDEqual(mMonsterId) && monster.IsSameTopOwner(owner))
            {
                // 还存在另一个
                if (HasMonster(monster))
                {
                    if (owner.aiManager != null)
                    {
                        owner.aiManager.ResetDestinationSelect();
                    }
                }
                else
                {
                    RestoreCustomIdleMode();
                }
            }
        }
    }

    private bool HasMonster(BeActor removeActor)
    {
        if (owner.CurrentBeScene == null)
            return false;

        bool ret = false;
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindMonsterByID(list, mMonsterId, false);
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            if (item.IsSameTopOwner(owner))
            {
                if (item.GetPID() != removeActor.GetPID())
                {
                    ret = true;
                }
            }
        }
        GamePool.ListPool<BeActor>.Release(list);
        return ret;
    }
    public override void OnFinish()
    {
        base.OnFinish();
        RestoreCustomIdleMode();
    }

    
    private void StartCustomIdleMode(BeActor monster)
    {
        if (owner.aiManager == null)
            return;
                
        owner.aiManager.followTarget = null;
        owner.aiManager.idleMode = BeAIManager.IdleMode.CUSTOM;
        owner.aiManager.radius = GetRadius(monster);
        owner.aiManager.monsterID = mMonsterId;
        owner.aiManager.customDestinationType = BeAIManager.DestinationType.WANDER_IN_OWNER_CIRCLE;
        isStartCustom = true;
        owner.aiManager.ResetDestinationSelect();

        mTargetActor = monster;
    }

    private void RestoreCustomIdleMode()
    {
        if (!isStartCustom)
            return;
        
        if (owner.aiManager == null)
            return;
        
        owner.aiManager.followTarget = mPreFollow;
        owner.aiManager.idleMode = mPreMode;
        owner.aiManager.radius = 0;
        owner.aiManager.monsterID = 0;
        owner.aiManager.customDestinationType = BeAIManager.DestinationType.IDLE;
        
        mTargetActor = null;
    }
}

