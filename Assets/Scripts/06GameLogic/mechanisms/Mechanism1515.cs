/// <summary>
/// 隐藏功能合集机制
/// 可扩展
/// </summary>
public class Mechanism1515 : BeMechanism
{
    enum HideTag
    {
        None = 0x00,
        HeadInfo = 0x01,
        HPBar = 0x02,
        Foot = 0x04,
        Shadow = 0x08,
        Actor = 0x10,
        Wing = 0x20,
    }
    
    
    public Mechanism1515(int mid, int lv) : base(mid, lv)
    {
    }
    private int m_HideTag = (int) HideTag.None;
    public override void OnInit()
    {
        base.OnInit();
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            if(TableManager.GetValueFromUnionCell(data.ValueA[i], level) > 0)
                m_HideTag |= 1 << i;
        }
    }

    public override void OnReset()
    {
        m_HideTag = (int)HideTag.None;
    }

    public override void OnStart()
    {
        base.OnStart();
        SetVisible(false);
    }

    public override void OnFinish()
    {
        base.OnFinish();
        SetVisible(true);
    }

    private void SetVisible(bool visible)
    {
#if !LOGIC_SERVER
        if (owner.m_pkGeActor == null)
            return;
        
        if (HasFlag(HideTag.HeadInfo))
            owner.m_pkGeActor.SetHeadInfoVisible(visible);
        
        if (HasFlag(HideTag.HPBar))
            owner.m_pkGeActor.SetHPBarVisible(visible);
        
        if (HasFlag(HideTag.Foot))
            owner.m_pkGeActor.SetFootIndicatorVisible(visible);
        
        if (HasFlag(HideTag.Shadow))
            if(owner.CurrentBeScene != null && owner.CurrentBeScene.currentGeScene != null)
                owner.m_pkGeActor.SetShadowVisible(owner.CurrentBeScene.currentGeScene, visible);
                
        if (HasFlag(HideTag.Actor))
            owner.m_pkGeActor.SetActorNodeVisable(visible);
        
        if (HasFlag(HideTag.Wing))
            owner.m_pkGeActor.SetAttachmentVisible("wing", visible);
#endif
    }

    private bool HasFlag(HideTag tag)
    {
        return 0 != (m_HideTag & (uint)tag);
    }
}