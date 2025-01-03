using FlatBuffers;
using GameClient;
using UnityEngine;

/// <summary>
/// 帧标签控制主角翅膀显隐
/// </summary>
public class Mechanism1118 : BeMechanism
{
    public Mechanism1118(int mid, int lv) : base(mid, lv) { }
    
#if !LOGIC_SERVER
    private string m_StartFrame = null;
    private string m_EndFrame = null;

    public override void OnInit()
    {
        m_StartFrame = TableManager.GetValueFromUnionCell(data.ValueA[0], level).ToString();
        m_EndFrame = TableManager.GetValueFromUnionCell(data.ValueB[0], level).ToString();
    }

    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onSkillCurFrame, OnSkillCurFrame);
        //handleA = owner.RegisterEvent(BeEventType.onSkillCurFrame, OnSkillCurFrame);
    }
    
    public override void OnFinish()
    {
        ResetWing();
    }

    private void OnSkillCurFrame(BeEvent.BeEventParam param)
    {
        string flag = param.m_String;
        if (flag == m_StartFrame)
        {
            ShowWing();
        }
        else if (flag == m_EndFrame)
        {
            ResetWing();
        }
    }

    private void ShowWing()
    {
        if(owner == null || owner.m_pkGeActor == null)
            return;
        
        owner.m_pkGeActor.SetAttachmentVisible("wing", false);
    }
    

    private void ResetWing()
    {
        if(owner == null || owner.m_pkGeActor == null)
            return;
        
        owner.m_pkGeActor.SetAttachmentVisible("wing", true);
    }
#endif
}

