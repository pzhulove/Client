using System;
using GameClient;
//显示头顶蓄力条
public class Mechanism163:BeMechanism
{
    private int m_durTime = 0;
    private string m_text = string.Empty;
    private SpellBar m_spellBar = null;
    public Mechanism163(int mid, int lv):base(mid, lv){ }
    
    public override void OnReset()
    {
        m_spellBar = null;
    }
    public override void OnInit()
    {
        m_durTime = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        m_text = data.StringValueA[0];
    }

    public override void OnStart()
    {
        m_spellBar = owner.StartSpellBar(eDungeonCharactorBar.Progress, m_durTime, true, m_text);
        if (m_spellBar != null)
        {
            m_spellBar.alwaysRefreshUI = true;
        }
    }
    public override void OnFinish()
    {
        base.OnFinish();
        if(m_spellBar != null)
        {
            owner.StopSpellBar(eDungeonCharactorBar.Progress);
        }
    }
}

