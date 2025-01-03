using System;
using System.Collections.Generic;
using GameClient;

/// <summary>
/// 靠近目标怪触发特效及增加buff机制机制
/// </summary>
public class Mechanism1094 : BeMechanism
{
    private string chainEffect;
    private int monsterID = 0;
    private int buffId;
    private VInt distance;

    private BeActor target;

    private BeEvent.BeEventHandleNew ownerDeadHandle;//自身死亡EventHandle
    private IBeEventHandle ownerRebornHandle;//自身复活EventHandle
    private IBeEventHandle targetDeadHandle;//目标死亡EventHandle
    private IBeEventHandle targetRebornHandle;//自身复活EventHandle

    public Mechanism1094(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        chainEffect = data.StringValueA[0];
        monsterID = TableManager.GetValueFromUnionCell(data.ValueA[0], level); //目标怪物id
        buffId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);//添加buffid
        distance = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueC[0], level), GlobalLogic.VALUE_1000);//搜索半径范围
    }

    public override void OnReset()
    {
        target = null;
        RemoveEventHandle();
        needUpdatePos = false;
        hasCreateChain = false;
        needUpdateState = false;
        mFindTargetCD = 500;
        mFindTargetTimer = 0;
    }

    bool needUpdatePos = false;//是否需要更新位置标志位 当有怪物死亡等则不更新位置
    bool hasCreateChain = false;//是否创建特效标志位 特效只需要创建一次
    bool needUpdateState = false;//更新标志位的开关，只有当怪物死亡或者复活后才需要再更新一次needUpdatePos变量
    public override void OnStart()
    {
        if(owner != null)
        {
            RemoveEventHandle();

            ownerDeadHandle = owner.RegisterEventNew(BeEventType.onDead, OnDeadEvent);
            
            ownerRebornHandle = owner.RegisterEventNew(BeEventType.onReborn, OnRebornEvent);
            
        }
    }

    int mFindTargetCD = 500;//当没有目标时查找频率
    int mFindTargetTimer = 0;//计时器
    public override void OnUpdate(int deltaTime)
    {
        if(target == null)
        {                                                          ////////
            mFindTargetTimer += deltaTime;                         ////////
            if (mFindTargetTimer > mFindTargetCD)                  ///查///
            {                                                      ///找///
                mFindTargetTimer = 0;                              ///目///
                FindTarget();                                      ///标///
                needUpdateState = true;                            ////////
            }                                                      ////////
        }                                           
        if (needUpdateState)                                       ////////////////
        {                                                          //////更新//////
            UpdateFlag();                                          /////状态位/////
        }                                                          ////////////////
        if (needUpdatePos)
        {
            if(owner != null && target != null)
            {                                                       ////////////////
                bool needCreateChain = IsInRange(owner, target);    ////////////////
                                                                    /////随位置/////
                if (needCreateChain && !hasCreateChain)             ////更新特效////
                {                                                   //////以及//////
                    CreateChainEffect();                            ////增删buff////
                }                                                   ////////////////
                else if (!needCreateChain && hasCreateChain)        ////////////////
                {
                    ClearChainEffect();
                }
            }
        }
    }

    public override void OnFinish()
    {
        RemoveEventHandle();
    }
    //查找目标函数
    private void FindTarget()
    {
        List<BeActor> monsterList = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindMonsterByID(monsterList, monsterID);
        if (monsterList != null)
        {
            for (int i = 0; i < monsterList.Count; ++i)
            {
                if (monsterList[i] != null && monsterList[i].GetPID() != owner.GetPID())
                {
                    target = monsterList[i];
                    targetDeadHandle = target.RegisterEventNew(BeEventType.onDead, OnDeadEvent);
                    targetRebornHandle = target.RegisterEventNew(BeEventType.onReborn, OnRebornEvent);
                    break;
                }
            }
        }
        GamePool.ListPool<BeActor>.Release(monsterList);
    }

    private void OnDeadEvent(BeEvent.BeEventParam eventParam)
    {
        needUpdateState = true;
        ClearChainEffect();
    }

    private void OnRebornEvent(BeEvent.BeEventParam eventParam)
    {
        needUpdateState = true;
    }

    private void UpdateFlag()//惰性处理只在开始。死亡。复活 对标志位特殊处理
    {
        if(owner == null || target == null)
        {
            needUpdatePos = false;
            return;
        }
        if (owner.IsDead() || target.IsDead())
        {
            needUpdatePos = false;
        }
        else
        {
            needUpdatePos = true;
        }
    }

    private bool IsInRange(BeActor player1, BeActor player2)
    {
        if (player1.GetDistance(player2) > distance)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void RemoveEventHandle()
    {
        if(ownerDeadHandle != null)
        {
            ownerDeadHandle.Remove();
            ownerDeadHandle = null;
        }
        if (ownerRebornHandle != null)
        {
            ownerRebornHandle.Remove();
            ownerRebornHandle = null;
        }
        if (targetDeadHandle != null)
        {
            targetDeadHandle.Remove();
            targetDeadHandle = null;
        }
        if (targetRebornHandle != null)
        {
            targetRebornHandle.Remove();
            targetRebornHandle = null;
        }
    }

    private void CreateChainEffect()
    {
#if !LOGIC_SERVER
        if (owner == null || target == null || owner.m_pkGeActor == null)
            return;
        owner.m_pkGeActor.CreateChainEffect(target, chainEffect);
#endif
        owner.buffController.TryAddBuff(buffId, -1, level);
        target.buffController.TryAddBuff(buffId, -1, level);

        hasCreateChain = true;
    }
    
    private void ClearChainEffect()
    {
#if !LOGIC_SERVER
        if (owner == null || owner.m_pkGeActor == null)
            return;
        owner.m_pkGeActor.ClearChainEffect();
#endif
        owner.buffController.RemoveBuff(buffId);
        target.buffController.RemoveBuff(buffId);

        hasCreateChain = false;
    }
}

