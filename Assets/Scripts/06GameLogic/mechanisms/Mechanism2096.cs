using System.Collections.Generic;
using GameClient;

/// <summary>
/// 集火标记机制
/// 与LockAttackMechanism配套使用，支持同战斗下多套集火共存
/// </summary>
public class Mechanism2096 : BeMechanism
{
    private BeActor m_attachActor;
    public Mechanism2096(int mid, int lv) : base(mid, lv)
    {
    }

    public override void OnReset()
    {
        base.OnReset();
        m_attachActor = null;
    }

    public override void OnStart()
    {
        m_attachActor = GetAttachBuffReleaser();
        TriggerEvent();
    }

    public override void OnFinish()
    {
        RemoveEvent();
        m_attachActor = null;
    }

    /// <summary>
    /// 发送集火怪物创建的消息
    /// </summary>
    private void TriggerEvent()
    {
        if (owner.CurrentBeScene == null)
            return;
        owner.CurrentBeScene.TriggerEventNew(BeEventSceneType.onZhihuiguanSelectTarget, new EventParam() { m_Obj = owner, m_Obj2 = m_attachActor, m_Int = mechianismID, m_Int2 = level});
    }
    
    /// <summary>
    /// 发送集火怪物创建的消息
    /// </summary>
    private void RemoveEvent()
    {
        if (owner.CurrentBeScene == null)
            return;
        owner.CurrentBeScene.TriggerEventNew(BeEventSceneType.onZhihuiguanUnSelectTarget, new EventParam() { m_Obj = owner, m_Obj2 = m_attachActor, m_Int = mechianismID, m_Int2 = level});
    }
}


/// <summary>
/// 集火功能机制基类,支持群体集火功能
/// 用于维护集火目标
/// 集火目标变更时触发OnForceTargetChange
/// </summary>
public abstract class LockAttackMechanism : BeMechanism
{
    protected LockAttackMechanism(int mid, int lv) : base(mid, lv){}
    
    private List<IBeEventHandle> m_EventHandles = new List<IBeEventHandle>();
    protected List<BeActor> m_LockActors = new List<BeActor>();
    private BeActor m_ForceTarget;

    protected BeActor ForceTarget
    {
        get => m_ForceTarget;
        set
        {
            if (m_ForceTarget != value)
            {
                m_ForceTarget = value;
                OnForceTargetChange();     
            }
        }
    }

    // 用于支持同战斗下，有多种集火机制情况。不同集火逻辑不会互相影响
    protected int m_LockMarkMechanismId;
    
    // 当改变集火目标时
    protected virtual void OnForceTargetChange() {}
    
    // 集火目标buff的等级
    private int m_LockAttackLevel;
    protected int LockAttackLevel => m_LockAttackLevel;

    public override void OnReset()
    {
        m_LockMarkMechanismId = 0;
        m_LockAttackLevel = 0;
        m_EventHandles.Clear();
        m_LockActors.Clear();
        m_ForceTarget = null;
    }

    public override void OnInit()
    {
        base.OnInit();
        m_LockMarkMechanismId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        FindLockMonster();
        _RegisterLockAim();
    }

    private void _RegisterLockAim()
    {
        if (owner.CurrentBeScene != null)
        {
            m_EventHandles.Add(owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onZhihuiguanSelectTarget, _OnLockSelectTarget));
            m_EventHandles.Add(owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onZhihuiguanUnSelectTarget, _OnUnLockSelectTarget));
        }
    }
    
    /// <summary>
    /// 监听集火目标指定
    /// </summary>
    private void _OnLockSelectTarget(BeEvent.BeEventParam param)
    {
        if(param.m_Int != m_LockMarkMechanismId)
            return;
        
        var releaseTarget = param.m_Obj2 as BeActor;
        if (!IsOwnerAim(releaseTarget))
            return;
        
        var target = param.m_Obj as BeActor;
        if (target == null)
            return;
        
        m_LockActors.Add(target);
        m_LockAttackLevel = param.m_Int2;
        ForceTarget = GetFirstActor();
    }
    
    /// <summary>
    /// 取消集火目标指定
    /// </summary>
    private void _OnUnLockSelectTarget(BeEvent.BeEventParam param)
    {
        if(param.m_Int != m_LockMarkMechanismId)
            return;
        
        var releaseTarget = param.m_Obj2 as BeActor;
        if (!IsOwnerAim(releaseTarget))
            return;

        var target = param.m_Obj as BeActor;
        if(target != null)
            m_LockActors.Remove(target);
        
        if (ForceTarget == target)
        {
            ForceTarget = GetFirstActor();
        }
    }
    
    /// <summary>
    ///  是否是我的主角集火目标
    /// </summary>
    /// <returns></returns>
    private bool IsOwnerAim(BeEntity entity)
    {
        if (entity == null)
            return false;
        
        if (!entity.IsSameTopOwner(owner))
            return false;

        return true;
    }
    
    private void FindLockMonster()
    {
        if (owner.CurrentBeScene == null)
            return;
        m_LockActors.Clear();
        List<BeEntity> list = GamePool.ListPool<BeEntity>.Get();
        owner.CurrentBeScene.GetEntitys2(list);
        for(int i = 0; i < list.Count; i++)
        {
            var actor = list[i] as BeActor;
            if (actor != null)
            {
                var mesh = actor.GetMechanism(m_LockMarkMechanismId);
                if (mesh != null)
                {
                    var releaser = mesh.GetAttachBuffReleaser();
                    if (releaser != null && releaser.IsSameTopOwner(owner))
                    {
                        m_LockActors.Add(actor);
                    }
                }
            }
        }
        GamePool.ListPool<BeEntity>.Release(list);

        ForceTarget = GetFirstActor();
    }

    /// <summary>
    /// 按优先级与距离排序。可自行重写
    /// </summary>
    /// <returns></returns>
    protected virtual BeActor GetFirstActor()
    {
        BeActor ret = null;
        BeUtility.TargetPriority priority = BeUtility.TargetPriority.None;
        for (int i = 0; i < m_LockActors.Count; i++)
        {
            var actor = m_LockActors[i];
            var pri = BeUtility.GetActorPriority(actor);
            if (pri > priority)
            {
                priority = pri;
                ret = actor;
            }
            else if(pri == priority)
            {
                if (ret != null)
                {
                    var targetDis = ret.GetDistance(owner);
                    var listDis = actor.GetDistance(owner);
                    if (listDis < targetDis)
                    {
                        ret = actor;
                        priority = pri;
                    }    
                }
            }
        }

        return ret;
    }
    
    public override void OnFinish()
    {
        base.OnFinish();
        ForceTarget = null;
        m_LockActors.Clear();
        RemoveHandle();
    }

    private void RemoveHandle()
    {
        for (int i = 0; i < m_EventHandles.Count; i++)
        {
            var handle = m_EventHandles[i];
            handle.Remove();
        }
        m_EventHandles.Clear();
    }
}
