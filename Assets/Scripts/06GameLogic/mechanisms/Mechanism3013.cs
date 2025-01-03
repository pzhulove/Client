using UnityEngine;

/// <summary>
/// 熊合体 同步主角步伐用
/// </summary>
public class Mechanism3013 : BeMechanism
{
    public Mechanism3013(int mid, int lv) : base(mid, lv)
    {
    }

    private bool m_IsInMove;
    private BeActor m_TopOwner;
    private int m_MoveSkillId;
    private int m_IdleSkillId;
    

    public override void OnInit()
    {
        base.OnInit();
        m_IdleSkillId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        m_MoveSkillId = TableManager.GetValueFromUnionCell(data.ValueA[1], level);
    }

    public override void OnReset()
    {
        base.OnReset();
        m_IsInMove = false;
        m_TopOwner = null;
    }

    public override void OnStart()
    {
        base.OnStart();
        m_TopOwner = GetTopOwner();
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (m_TopOwner.IsInMoveDirection())
        {
            if (!m_IsInMove)
            {
                ((BeActorStateGraph)owner.GetStateGraph()).ExecuteSkill(m_MoveSkillId);
                m_IsInMove = true;
            }
        }
        else
        {
            if (m_IsInMove)
            {
                ((BeActorStateGraph)owner.GetStateGraph()).ExecuteSkill(m_IdleSkillId);
                m_IsInMove = false;
            }
        }
    }
}
