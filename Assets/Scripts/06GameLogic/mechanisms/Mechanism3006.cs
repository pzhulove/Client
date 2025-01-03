using System.Collections.Generic;
using GameClient;

/// <summary>
/// 警卫员位置绑定机制-位置管理
/// </summary>
public class Mechanism3006 : BeMechanism
{
    public Mechanism3006(int mid, int lv) : base(mid, lv) { }
    /*public static int ID = 6229;
    
    public enum AttackMode
    {
        Far,    // 远距离攻击模式
        Near,   // 近距离攻击模式
        Count
    }
    private AttackMode m_CurAttackMode = AttackMode.Far;

    public AttackMode CurAttackMode => m_CurAttackMode;

    private VInt3[] m_OffsetPos = new VInt3[(int) AttackMode.Count];
    private bool[] m_AnchorMask = new bool[2];
    private List<Mechanism3004> m_EnemyList = new List<Mechanism3004>();
    public override void OnInit()
    {
        base.OnInit();
        m_OffsetPos[(int) AttackMode.Far] = new VInt3(TableManager.GetValueFromUnionCell(data.ValueA[0], level), TableManager.GetValueFromUnionCell(data.ValueA[1], level), 0);
        m_OffsetPos[(int) AttackMode.Near] = new VInt3(TableManager.GetValueFromUnionCell(data.ValueB[0], level), TableManager.GetValueFromUnionCell(data.ValueB[1], level), 0);
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onCastSkill, OnStartSkill);
    }

    private void OnStartSkill(BeEvent.BeEventParam param)
    {
        if (param.m_Int == 1418)
        {
            SwitchEnemy();
        }
    }

    public VInt3 GetAnchorPosition(int id)
    {
        var pos = m_OffsetPos[(int) m_CurAttackMode];
        if (id == 1)
        {
            pos.y = -pos.y;
        }

        return pos;
    }

    public int GetAnchorId(Mechanism3004 enemy)
    {
        for (int i = 0; i < m_AnchorMask.Length; i++)
        {
            if (!m_AnchorMask[i])
            {
                m_EnemyList.Add(enemy);
                m_AnchorMask[i] = true;
                return i;
            }
        }

        Logger.LogError("Mechanism3004 out of enemy limit");
        return 0;
    }

    public void ReleaseAnchorId(int id, Mechanism3004 enemy)
    {
        if(id >= m_AnchorMask.Length)
            return;

        m_EnemyList.Remove(enemy);
        m_AnchorMask[id] = false;
    }

    private void SwitchEnemy()
    {
        m_CurAttackMode = m_CurAttackMode == AttackMode.Far ? AttackMode.Near : AttackMode.Far;
        for (int i = 0; i < m_EnemyList.Count; i++)
        {
            var enemy = m_EnemyList[i];
            enemy.UpdateAttackMode();
        }
    }

    public bool SkillEnable()
    {
        if (m_EnemyList.Count <= 0)
            return false;
        
        for (int i = 0; i < m_EnemyList.Count; i++)
        {
            var enemy = m_EnemyList[i];
            if (!enemy.CanControl())
            {
                return false;
            }
        }
        return true;
    }*/
}

