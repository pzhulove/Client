using System.Collections.Generic;
using GameClient;

public class Skill2408 : BeSkill
{
    public Skill2408(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }
    
    private int m_BuffId;
    private IBeEventHandle m_Handle = null;
    private List<int> m_SummonMonsterIds = new List<int>();
    
    public override void OnInit()
    {
        m_BuffId = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        for (int i = 0; i < skillData.ValueB.Count; i++)
        {
            m_SummonMonsterIds.Add(TableManager.GetValueFromUnionCell(skillData.ValueB[i], level));
        }
    }
    
    public override void OnPostInit()
    {
        RemoveHandle();
        m_Handle = owner.RegisterEventNew(BeEventType.onSummon, OnSummon);
    }

    private void OnSummon(BeEvent.BeEventParam args)
    {
        var monster = args.m_Obj as BeActor;
        if(monster == null)
            return;
        
        if (BeUtility.IsMonsterIDEqualList(m_SummonMonsterIds, monster.GetEntityData().monsterID))
        {
            monster.buffController.TryAddBuff(m_BuffId, -1, level);
        }
    }

    void RemoveHandle()
    {
        if (m_Handle != null)
        {
            m_Handle.Remove();
            m_Handle = null;
        }
    }

}
