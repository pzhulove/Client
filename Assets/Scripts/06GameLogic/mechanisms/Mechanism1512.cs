using System.Collections.Generic;
using FlatBuffers;
using GameClient;
using Spine;

/// <summary>
/// 当没有携带（学习）指定技能时，无法添加指定buff,当学习指定技能后，buff等级走技能等级
/// </summary>
public class Mechanism1512 : BeMechanism
{
    public Mechanism1512(int mid, int lv) : base(mid, lv)
    {
    }

    private int m_TargetSkill;
    private List<int> m_ControlBuffIdList = new List<int>();
    public override void OnInit()
    {
        m_TargetSkill = TableManager.GetValueFromUnionCell(data.ValueA[0], level);

        m_ControlBuffIdList.Clear();
        for (int i = 0; i < data.ValueB.Count; i++)
        {
            m_ControlBuffIdList.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
        }
    }

    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.BuffCanAdd, CanAddBuff);
    }

    private void CanAddBuff(BeEvent.BeEventParam param)
    {
        if (m_ControlBuffIdList.Contains(param.m_Int2))
        {
            var skill = owner.GetSkill(m_TargetSkill);
            if (skill == null)
            {
                param.m_Int = 1;
            }
            else
            {
                param.m_Int3 = skill.GetLevel();
            }
        }
    }
}