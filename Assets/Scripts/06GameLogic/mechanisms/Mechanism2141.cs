using System.Collections.Generic;
using System.Linq;
using GameClient;

/// <summary>
/// 如果在机制期间内未受击，在机制结束是会添加buff
/// </summary>
public class Mechanism2141 : BeMechanism
{
    public Mechanism2141(int mid, int lv) : base(mid, lv)
    {
    }

    private List<int> m_BuffInfoId = new List<int>();
    private bool m_HasHurt = false;
    public override void OnInit()
    {
        for(int i = 0; i < data.ValueA.Length; i++)
            m_BuffInfoId.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        m_HasHurt = false;
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onHurt, _OnHurt);
    }

    private void _OnHurt(BeEvent.BeEventParam param)
    {
        m_HasHurt = true;
    }

    public override void OnFinish()
    {
        if (!m_HasHurt)
        {
            for(int i = 0 ; i < m_BuffInfoId.Count; i++)
                owner.buffController.TryAddBuffInfo(m_BuffInfoId[i], owner, level);
        }
    }
}
