using System;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

/// <summary>
/// 瞬移到身前某个坐标
/// </summary>
public class Mechanism1122 : BeMechanism
{
    private HashSet<int> m_HurtIdList = new HashSet<int>();     //监听的触发效果Id
    private int m_BuffInfoId = 0;       //切换形态BuffInfoId
    private VInt m_TeleportDis = 0;     //面朝方向的瞬移距离

    protected bool m_CurFace = false;
    protected VInt3 m_Pos = VInt3.zero;

    public Mechanism1122(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        m_HurtIdList.Clear();
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            m_HurtIdList.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }

        if (data.ValueB.Count > 0)
        {
            m_BuffInfoId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);   
        }
        
        m_TeleportDis = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueC[0], level), GlobalLogic.VALUE_1000);
    }

    public override void OnReset()
    {
        m_HurtIdList.Clear();
        m_BuffInfoId = 0;
        m_TeleportDis = 0;
        m_CurFace = false;
        m_Pos = VInt3.zero;
    }

    public override void OnStart()
    {
        base.OnStart();
        InitData();
    }

    /// <summary>
    /// 初始化数据
    /// </summary>
    protected void InitData()
    {
        m_CurFace = owner.GetFace();
        m_Pos = owner.GetPosition();
        //handleA = owner.RegisterEvent(BeEventType.onHitOtherAfterHurt, OnHitOtherAfterHurt);
        handleA = OwnerRegisterEventNew(BeEventType.onHitOtherAfterHurt, OnHitOtherAfterHurt);
    }

    //添加一个强制切换人物形态的buff 同时瞬移
    private void OnHitOtherAfterHurt(BeEvent.BeEventParam param)
    {
        int hurtId = param.m_Int;
        if (!m_HurtIdList.Contains(hurtId))
            return;
        BeActor actor = param.m_Obj as BeActor;
        if (actor == null || actor.IsDead())
            return;
        //不能瞬移被抓取的目标
        if (actor.grabController.isAbsorb)
            return;
        //不能瞬移霸体怪物
        if (actor.buffController == null || actor.buffController.HaveBatiBuff() || actor.buffController.HasBuffByID((int)GlobalBuff.GEDANG) != null)
            return;
        if (actor.stateController == null || !actor.stateController.HasBornAbility(BeAbilityType.FALLGROUND) || !actor.stateController.HasBornAbility(BeAbilityType.FLOAT))
            return;
        if (actor.protectManager != null && actor.protectManager.IsInGroundProtect())
            return;
        
        if (m_BuffInfoId > 0)
        {
            if (!actor.stateController.HasBuffState(BeBuffStateType.FROZEN) && !actor.stateController.HasBuffState(BeBuffStateType.STONE))
                actor.buffController.TryAddBuff(m_BuffInfoId);
        }

        MoveTo(actor);
    }

    /// <summary>
    /// 移动到某个位置
    /// </summary>
    private void MoveTo(BeActor target)
    {
        VInt3 targetPos = target.GetPosition();
        int dis = Mathf.Abs(m_CurFace ? targetPos.x - m_Pos.x : m_Pos.x - targetPos.x);
        if (dis > m_TeleportDis)
            return;
        int time = 200;
        var targetNewPos = m_Pos;
        targetNewPos.x += m_CurFace ? -m_TeleportDis.i : m_TeleportDis.i;
        targetNewPos.y = targetPos.y;
        if (m_Pos.z > 0)
            targetNewPos.z = 0;
        BeActionActorFilter filter = new BeActionActorFilter();
        filter.Init(true, true, true, true, true);
        BeMoveTo moveTo = BeMoveTo.Create(target, time, targetPos, targetNewPos, false, filter);
        target.actionManager.RunAction(moveTo);
    }

}

