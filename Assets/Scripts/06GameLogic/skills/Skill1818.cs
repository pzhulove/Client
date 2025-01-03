using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
 * 阵鬼 鬼影鞭技能
*/
public class Skill1818 : BeSkill
{
    protected int m_BuffId = 181101;        //冥炎卡洛BuffID
    protected int m_PhaseTwoId = 18180;     //第二阶段ID
    protected int m_PhaseThreeId = 181801;   //第三阶段ID

    protected IBeEventHandle m_ReplaceSkillHandle = null;      //监听技能替换
    public Skill1818 (int sid, int skillLevel):base(sid, skillLevel)
    {
        
    }

    public override void OnInit()
    { 
    }

    public override void OnStart()
    {
        if (m_ReplaceSkillHandle != null)
        {
            m_ReplaceSkillHandle.Remove();
        }
        m_ReplaceSkillHandle = owner.RegisterEventNew(BeEventType.onPreSetSkillAction, (GameClient.BeEvent.BeEventParam param) =>
        {
            //int[] skillList = (int[])args[0];
            BeBuff buff = owner.buffController.HasBuffByID(m_BuffId);
            if (param.m_Int == m_PhaseTwoId && buff!=null)
            {
                param.m_Int = m_PhaseThreeId;
            }
        });
    }
}
