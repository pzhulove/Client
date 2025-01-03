using System.Collections.Generic;
using FlatBuffers;
using GameClient;
using Spine;

/// <summary>
/// 点名上buff。一定时间后传送到背后释放技能
/// </summary>
public class Mechanism1511 : BeMechanism
{
    public Mechanism1511(int mid, int lv) : base(mid, lv)
    {
    }

    public enum CallState
    {
        CD,
        WaitAttack,
    }
    
    private int m_BuffInfoId;
    private int m_SkillId;
    private int m_TriggerCD;
    private int m_DelayTime;
    private int m_OffsetX = 1000;

    private CoolDown m_CD;
    private BeActor m_CallTarget;
    private int m_DelayTimeAcc;
    private CallState m_State;

    public override void OnInit()
    {
        m_BuffInfoId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        m_SkillId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        m_TriggerCD = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        m_DelayTime = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        if (data.ValueE.Count > 0)
            m_OffsetX = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
    }

    public override void OnReset()
    {
        m_OffsetX = 1000;
        m_CD.Clear();
        m_CallTarget = null;
        m_DelayTimeAcc = 0;
        m_State = CallState.CD;
    }

    public override void OnStart()
    {
        base.OnStart();
        m_CD = new CoolDown();
        m_CD.StartCD(m_TriggerCD, true);
        m_State = CallState.CD;
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (m_State == CallState.CD)
        {
            m_CD.UpdateCD(deltaTime);
            if (!m_CD.IsCD())
            {
                m_CallTarget = GetCallActor();
                if (m_CallTarget != null)
                {
                    m_CallTarget.buffController.TryAddBuffInfo(m_BuffInfoId, owner, level);
                    m_DelayTimeAcc = 0;
                    m_State = CallState.WaitAttack;
                }
            }
        }
        else if (m_State == CallState.WaitAttack)
        {
            m_DelayTimeAcc += deltaTime;

            if (m_CallTarget.IsDead())
            {
                StartCD();
            }
            else if (m_DelayTimeAcc >= m_DelayTime)
            {
                if (CanAttack())
                {
                    StartCD();
                    DoAttack();
                }
            }
        }
    }

    public override void OnFinish()
    {
        base.OnFinish();
        if (m_CallTarget != null)
        {
            m_CallTarget.buffController.RemoveBuffByBuffInfoID(m_BuffInfoId);
            m_CallTarget = null;
        }
    }

    private void StartCD()
    {
        m_State = CallState.CD;
        m_CD.StartCD(m_TriggerCD);
    }

    private BeActor GetCallActor()
    {
        if (owner.CurrentBeScene == null)
        {
            return null;
        }

        BeActor ret = null;
        var list = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindTargets(list, owner, VInt.Float2VIntValue(100.0f));
        if (list.Count > 0)
        {
            var index = FrameRandom.Random((uint) list.Count);
            ret = list[index];    
        }
        GamePool.ListPool<BeActor>.Release(list);

        return ret;
    }

    private bool CanAttack()
    {
        return owner.CanUseSkill(m_SkillId) && !owner.IsPassiveState();
    }
    
    private void DoAttack()
    {
        if(m_CallTarget == null)
            return;
        
        owner.SetPosition(GetAttackPosition());
        owner.SetFace(m_CallTarget.GetPosition().x < owner.GetPosition().x);
        owner.UseSkill(m_SkillId);
        m_CallTarget.buffController.RemoveBuffByBuffInfoID(m_BuffInfoId);
    }

    private VInt3 GetAttackPosition()
    {
        if (m_CallTarget == null)
            return owner.GetPosition();

        var pos = m_CallTarget.GetPosition();
        pos.x += m_CallTarget.GetFace() ? m_OffsetX : -m_OffsetX;
        if (owner.CurrentBeScene.IsInBlockPlayer(pos))
        {
            pos = m_CallTarget.GetPosition();
        }
        return pos;
    }
}