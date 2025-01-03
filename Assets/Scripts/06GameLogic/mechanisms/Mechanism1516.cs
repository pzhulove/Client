using System.Collections.Generic;
using GameClient;

/// <summary>
/// 指定技能 CD 结束加buffInfo
/// </summary>
public class Mechanism1516 : BeMechanism
{
    public Mechanism1516(int mid, int lv) : base(mid, lv)
    {
    }

    private List<int> m_SkillList = new List<int>();
    private int m_BuffInfoId;
    public override void OnInit()
    {
        base.OnInit();
        m_SkillList.Clear();
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            m_SkillList.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }

        m_BuffInfoId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.OnSkillCoolDown, OnSkillCoolDown);
    }

    private void OnSkillCoolDown(BeEvent.BeEventParam param)
    {
        if (m_SkillList.Contains(param.m_Int))
        {
            owner.buffController.TryAddBuffInfo(m_BuffInfoId, owner, level);
        }
    }
}