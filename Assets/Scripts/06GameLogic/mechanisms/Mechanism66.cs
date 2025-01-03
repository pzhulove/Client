using UnityEngine;
using System.Collections.Generic;
using GameClient;

/*
 * 替换技能阶段
*/
public class Mechanism66 : BeMechanism
{
    protected int m_OriginalPhaseId = 0;                                //原来的技能阶段ID
    protected int m_ReplacePhaseId = 0;                                 //替换后的技能阶段ID
    protected List<int> m_ReplacePhaseIdList = new List<int>();         //替换后的技能阶段列表                          

    protected IBeEventHandle m_ReplacePhaseHandle = null;                //监听攻击到别人

    public Mechanism66(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        m_OriginalPhaseId = 0;    
        m_ReplacePhaseId = 0;
        m_ReplacePhaseIdList.Clear();
        m_ReplacePhaseHandle = null;
    }
    public override void OnInit()
    {
        m_OriginalPhaseId = TableManager.GetValueFromUnionCell(data.ValueA[0],level);
        if (data.ValueB.Count > 0)
        {
            m_ReplacePhaseId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        }

        if (data.ValueC.Count > 0)
        {
            for(int i = 0; i < data.ValueC.Count; i++)
            {
                int replaceId = TableManager.GetValueFromUnionCell(data.ValueC[i],level);
                m_ReplacePhaseIdList.Add(replaceId);
            }
        }
    }

    public override void OnStart()
    {
        RemoveHandle();
        m_ReplacePhaseHandle = owner.RegisterEventNew(BeEventType.onPreSetSkillAction, (GameClient.BeEvent.BeEventParam param) => 
        {
            //int[] skillArray = (int[])args[0];
            if(param.m_Int == m_OriginalPhaseId)
            {
                if (m_ReplacePhaseId != 0)
                {
                    param.m_Int = m_ReplacePhaseId;
                }

                if (m_ReplacePhaseIdList.Count > 0)
                {
                    int[] newPhaseArray = new int[m_ReplacePhaseIdList.Count];
                    for (int i = 0; i < m_ReplacePhaseIdList.Count; i++)
                    {
                        newPhaseArray[i] = m_ReplacePhaseIdList[i];
                    }
                    owner.skillController.SetCurrentSkillPhases(newPhaseArray);
                }
            }
        });
    }

    public override void OnFinish()
    {
        RemoveHandle();
    }

    protected void RemoveHandle()
    {
        if (m_ReplacePhaseHandle != null)
        {
            m_ReplacePhaseHandle.Remove();
            m_ReplacePhaseHandle = null;
        }
    }
}
