using System.Collections.Generic;
using behaviac;
using GameClient;
using UnityEngine;

/// <summary>
/// 范围判定器
/// </summary>
public interface IRangeFilter
{
    VInt3 GetCenterPosition();
    bool Filter(VInt3 pos);
}

/// <summary>
/// 随主角动态的区域
/// </summary>
public abstract class TargetRangeFilter : IRangeFilter
{
    protected BeActor m_Target;
    protected TargetRangeFilter(BeActor target)
    {
        m_Target = target;
    }
    public VInt3 GetCenterPosition()
    {
        if (m_Target != null)
            return m_Target.GetPosition();
        return VInt3.zero;
    }

    public abstract bool Filter(VInt3 pos);
}

/// <summary>
/// 固定中心位置的区域
/// </summary>
public abstract class FixedRangeFilter : IRangeFilter
{
    protected VInt3 m_Center;
    protected FixedRangeFilter(VInt3 center)
    {
        m_Center = center;
    }
    public VInt3 GetCenterPosition()
    {
        return m_Center;
    }

    public abstract bool Filter(VInt3 pos);
}

/// <summary>
/// 行为树2D范围判定器
/// </summary>
public class BTTargetRange2DFilter : TargetRangeFilter
{
    behaviac.Range m_Range;
    public BTTargetRange2DFilter(BeActor target, behaviac.Range range):base(target)
    {
        m_Range = range;
    }
    public override bool Filter(VInt3 pos)
    {
        var selfPos = GetCenterPosition();
        if (m_Range.AreaType == AreaType.CircularArea)
        {
            var center = selfPos + new VInt3(m_Range.StrArgA, m_Range.StrArgB, 0);
            return (center - pos).magnitude2D <= m_Range.StrArgC;
        }
        else if (m_Range.AreaType == AreaType.RectangleArea)
        {
            return pos.x <= selfPos.x + m_Range.StrArgB && pos.x >= selfPos.x - m_Range.StrArgA &&
                   pos.y <= selfPos.y + m_Range.StrArgC && pos.y >= selfPos.y - m_Range.StrArgD;
        }
        return false;
    }
}

/// <summary>
/// 行为树2D范围判定器
/// </summary>
public class BTFixedRange2DFilter : FixedRangeFilter
{
    behaviac.Range m_Range;
    public BTFixedRange2DFilter(VInt3 center, behaviac.Range range):base(center)
    {
        m_Range = range;
    }
    public override bool Filter(VInt3 pos)
    {
        var selfPos = GetCenterPosition();
        if (m_Range.AreaType == AreaType.CircularArea)
        {
            var center = selfPos + new VInt3(m_Range.StrArgA, m_Range.StrArgB, 0);
            return (center - pos).magnitude2D <= m_Range.StrArgC;

        }
        else if (m_Range.AreaType == AreaType.RectangleArea)
        {
            return pos.x <= selfPos.x + m_Range.StrArgB && pos.x >= selfPos.x - m_Range.StrArgA &&
                   pos.y <= selfPos.y + m_Range.StrArgC && pos.y >= selfPos.y - m_Range.StrArgD;
        }
        return false;
    }
}
/// <summary>
/// 基于actor为坐标系的范围判定
/// 用于处理类似杀意波动等效果
/// </summary>
public class BeRange
{
    private readonly BeActor m_Target;
    private IRangeFilter m_RangeFilter;
    private IEntityFilter m_ActorFilter;
    private readonly int m_ID = 0;
    
    public delegate bool RangeTrigger(BeActor target);
    protected RangeTrigger m_RangeInTrigger;
    protected RangeTrigger m_RangeOutTrigger;
    protected RangeTrigger m_RangeInsideTrigger;

    protected List<BeActor> m_Targets= new List<BeActor>();

    protected int m_UpdateIntervalAcc = 0;
    protected int m_UpdateInterval;
    
    protected bool m_IsRunning = false;
    private bool m_IsRemove = false;
    public bool IsRunning
    {
        get => m_IsRunning;
        set => m_IsRunning = value;
    }

    public int UpdateInterval
    {
        get => m_UpdateInterval;
        set => m_UpdateInterval = value;
    }

    public IEntityFilter ActorFilter     
    {
        get => m_ActorFilter;
        set => m_ActorFilter = value;
    }
    public IRangeFilter RangeFilter 
    {
        get => m_RangeFilter;
        set => m_RangeFilter = value;
    }

    public BeRange(BeActor target, IRangeFilter rangeFilter, IEntityFilter actorFilter, int updateInterval = 0)
    {
        m_Target = target;
        m_RangeFilter = rangeFilter;
        m_ActorFilter = actorFilter;
        m_UpdateInterval = updateInterval;
        m_UpdateIntervalAcc = 0;
        if (m_Target == null || rangeFilter == null)
        {
            Logger.LogErrorFormat("创建BeRange异常:{0}, {1}", m_Target == null, rangeFilter == null);
            return;
        }

        m_ID = ++target.rangeId;
    }
    
    public int GetPID() { return m_ID; }
    public void Remove() { m_IsRemove = true; }
    public bool IsRemove() { return m_IsRemove; }

    public RangeTrigger RangeInTrigger
    {
        set => m_RangeInTrigger = value;
    }

    public RangeTrigger RangeOutTrigger
    {
        set => m_RangeOutTrigger = value;
    }
    
    public RangeTrigger RangeInsideTrigger
    {
        set => m_RangeInsideTrigger = value;
    }

    public bool IsInRange(VInt3 pos)
    {
        return m_RangeFilter != null && m_RangeFilter.Filter(pos);
    }

    /// <summary>
    /// 获取运行时的区域内列表
    /// </summary>
    /// <param name="list"></param>
    public void GetRunningTarget(List<BeActor> list)
    {
        list.Clear();
        if (!m_IsRunning)
            return;
        
        for (int i = 0; i < m_Targets.Count; i++)
        {
            var target = m_Targets[i];
            list.Add(target);
        }
    }

    /// <summary>
    /// 重新计算区域内的列表
    /// </summary>
    /// <param name="list"></param>
    public void GetRangeTarget(List<BeActor> ret)
    {
        ret.Clear();
        var targets = GamePool.ListPool<BeActor>.Get();
        m_Target.CurrentBeScene.GetFilterTarget(targets, m_ActorFilter);
        for (int i = 0; i < targets.Count; i++)
        {
            var actor = targets[i];
            var actorPos = actor.GetPosition();
            if (m_RangeFilter.Filter(actorPos))
            {
                ret.Add(actor);
            }
        }
        GamePool.ListPool<BeActor>.Release(targets);
    }

    public void GetTarget(List<BeActor> list)
    {
        if(m_IsRunning)
            GetRunningTarget(list);
        else
            GetRangeTarget(list);
    }
    
    protected void UpdateTargetList()
    {
        var targets = GamePool.ListPool<BeActor>.Get();
        targets = m_Target.CurrentBeScene.GetFilterTarget(targets, m_ActorFilter);
        var inRangeActor = GamePool.ListPool<BeActor>.Get();
        for (int i = 0; i < targets.Count; i++)
        {
            var actor = targets[i];
            var actorPos = actor.GetPosition();
            
            if(m_RangeFilter.Filter(actorPos))
            {
                inRangeActor.Add(actor);
                if (!m_Targets.Contains(actor))
                {
                    RangeIn(actor);
                    m_Targets.Add(actor);
                }
            }
        }

        for (int i = m_Targets.Count - 1; i >= 0; i--)
        {
            var actor = m_Targets[i];
            if (!inRangeActor.Contains(actor))
            {
                RangeOut(actor);
                m_Targets.RemoveAt(i);
            }
            else
            {
                RangeInside(actor);
            }
        }
        
        GamePool.ListPool<BeActor>.Release(inRangeActor);
        GamePool.ListPool<BeActor>.Release(targets);
    }

    public void Update(int delta)
    {
        if (!m_IsRunning || m_IsRemove || m_Target == null || m_RangeFilter == null)
            return;

        if (m_UpdateInterval == 0)
        {
            UpdateTargetList();
        }
        else
        {
            m_UpdateIntervalAcc += delta;
            if (m_UpdateIntervalAcc >= m_UpdateInterval)
            {
                UpdateTargetList();
                m_UpdateIntervalAcc -= m_UpdateInterval;
            }
        }
    }
    
    protected void RangeIn(BeActor actor)
    {
        if (m_RangeInTrigger != null)
            m_RangeInTrigger(actor);
        m_Target.TriggerEventNew(BeEventType.onRangeIn, new EventParam() {m_Obj = actor, m_Obj2 = this});
        
        OnRangeIn(actor);
    }
    
    protected void RangeOut(BeActor actor)
    {
        if (m_RangeOutTrigger != null)
            m_RangeOutTrigger(actor);
        m_Target.TriggerEventNew(BeEventType.onRangeOut, new EventParam() {m_Obj = actor, m_Obj2 = this});
        
        OnRangeOut(actor);
    }

    protected void RangeInside(BeActor actor)
    {
        if (m_RangeInsideTrigger != null)
            m_RangeInsideTrigger(actor);
        m_Target.TriggerEventNew(BeEventType.onRangeInside, new EventParam() {m_Obj = actor, m_Obj2 = this});
        
        OnRangeInside(actor);
    }
    protected virtual void OnRangeIn(BeActor actor){}
    protected virtual void OnRangeOut(BeActor actor){}
    protected virtual void OnRangeInside(BeActor actor){}
}