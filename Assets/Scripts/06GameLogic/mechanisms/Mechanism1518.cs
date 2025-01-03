using System.Collections.Generic;
using GameClient;

/// <summary>
/// </summary>
public class Mechanism1518 : BeMechanism
{
    public Mechanism1518(int mid, int lv) : base(mid, lv)
    {
    }

    private List<int> m_HurtList = new List<int>();
    private int m_SkillId;
    private int m_Time;
    
    public override void OnInit()
    {
        base.OnInit();
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            m_HurtList.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }

        m_SkillId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        m_Time = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
    }

    public override void OnReset()
    {
        m_HurtList.Clear();
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onHitOther, OnHit);
    }

    private void OnHit(BeEvent.BeEventParam param)
    {
        if (m_HurtList.Contains(param.m_Int))
        {
            var skill = owner.GetSkill(m_SkillId);
            if (skill != null)
            {
                if (skill.isCooldown)
                {
                    skill.CDTimeAcc += m_Time;
                }
            }
            Finish();
        }
    }
}