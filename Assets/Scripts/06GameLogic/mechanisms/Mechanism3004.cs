using System;
using System.Collections.Generic;
using GameClient;
using Protocol;

/// <summary>
/// 警卫员位置绑定机制,同步技能机制。
/// </summary>
public class Mechanism3004 : BeMechanism
{
    public Mechanism3004(int mid, int lv) : base(mid, lv) { }

    private BeEntity m_TopOwner = null;
    private Skill1400 m_Mgr;
    private VInt3 m_Offset;
    private int m_AnchorId;
    private int[] m_AttackModeSkillId = new int[(int) Skill1400.AttackMode.Count];
    private int m_CurModeSkillId;
    private bool m_curAttackState = false;

    private bool m_IsLock = true;
   // private HashSet<int> m_SyncSkillId = new HashSet<int>();
    //private readonly int ShootSkillId = 1409;
    //private bool m_IsLoopShoot = false;

    public override void OnInit()
    {
        base.OnInit();
        for (int i = 0; i < m_AttackModeSkillId.Length; i++)
        {
            m_AttackModeSkillId[i] = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
        }

        /*for (int i = 0; i < data.ValueB.Count; i++)
        {
            m_SyncSkillId.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
        }*/
    }

    public override void OnReset()
    {
        m_TopOwner = null;
        m_Mgr = null;
        m_Offset = VInt3.zero;
        m_AnchorId = 0;
        m_AttackModeSkillId = new int[(int)Skill1400.AttackMode.Count];
        m_CurModeSkillId = 0;
        m_curAttackState = false;
        m_IsLock = true;
    }

    public override void OnStart()
    {
        base.OnStart();
        m_TopOwner = owner.GetTopOwner(owner);
        BeActor actor = m_TopOwner as BeActor;
        if (actor != null) 
            m_Mgr = actor.GetSkill(1400) as Skill1400;
        if (m_Mgr != null)
        {
            m_AnchorId = m_Mgr.GetAnchorId( this);
            UpdateAttackMode();
        }

        RegisterEvent();
    }

    public void SetLockEnemy(bool isLock)
    {
        m_IsLock = isLock;
    }
    private void RegisterEvent()
    {
        if(m_TopOwner == null)
            return;
        
        //handleA = m_TopOwner.RegisterEventNew(BeEventType.onCastSkill, OnSkillStart);
        //handleB = m_TopOwner.RegisterEventNew(BeEventType.onCastSkillFinish, OnSkillFinish);
        //handleC = m_TopOwner.RegisterEventNew(BeEventType.onSkillCancel, OnSkillFinish);
    }

    /*private void OnSkillStart(BeEvent.BeEventParam param)
    {
        if (m_SyncSkillId.Contains(param.m_Int))
        {
            owner.UseSkill(m_CurModeSkillId, true);
        }
        else if(param.m_Int == ShootSkillId)
        {
            m_IsLoopShoot = true;
            if(owner.IsCastingSkill())
                owner.CancelSkill(owner.GetCurSkillID());
            owner.ResetSkillCoolDown(m_CurModeSkillId);
            owner.UseSkill(m_CurModeSkillId, true);
            
            // 确保不会在技能期间死亡
            var buff = owner.buffController.HasBuffByID((int) GlobalBuff.LIFE_TIME);
            if (buff != null)
            {
                buff.duration += GlobalLogic.VALUE_100000;
            }
        }
    }

    private void OnSkillFinish(BeEvent.BeEventParam param)
    {
        if (param.m_Int == ShootSkillId)
        {
            m_IsLoopShoot = false;
            owner.delayCaller.DelayCall(0, () => owner.DoDead(true));
        }
    }*/
    
    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        UpdatePosition();
        UpdateLoopShoot();
    }

    private void UpdateLoopShoot()
    {
        if(owner.CanUseSkill(m_CurModeSkillId))
            owner.UseSkill(m_CurModeSkillId);
    }

    private void UpdatePosition()
    {
        if(!m_IsLock)
            return;
        
        if (m_TopOwner == null)
            return;
        
        var toPos = m_TopOwner.GetPosition() + GetOffsetPosition();
        owner.SetPosition(toPos);
        owner.SetFace(m_TopOwner.GetFace());
    }

    public void UpdateAttackMode()
    {
        if(m_Mgr == null)
            return;
        
        m_Offset = m_Mgr.GetAnchorPosition(m_AnchorId);
        UpdatePosition();
        UpdateAttackSkill();
    }

    private void UpdateAttackSkill()
    {
        var mode = m_Mgr.CurAttackMode;
        m_CurModeSkillId = m_AttackModeSkillId[(int) mode];
    }
    
    private VInt3 GetOffsetPosition()
    {
        if (m_TopOwner == null)
            return VInt3.zero;

        VInt3 pos = m_Offset;
        if (!m_TopOwner.GetFace())
        {
            pos.x = -pos.x;
        }

        return pos;
    }
    
    public override void OnFinish()
    {
        base.OnFinish();
        if (m_Mgr != null) 
            m_Mgr.ReleaseAnchorId(m_AnchorId, this);
    }

    public bool CanControl()
    {
        return !owner.IsInPassiveState() && !owner.IsDeadOrRemoved();
    }
}

