
/// <summary>
/// 角色和武器 翅膀 渐隐渐现机制
/// </summary>

public class Mechanism1504 : BeMechanism
{
    public Mechanism1504(int mid, int lv) : base(mid, lv) { }

#if !LOGIC_SERVER

    private uint _showHandle = ~0u;
    private uint _hideHandle = ~0u;

    private int _curTime = 0;
    private int _delayShowTime = 0;
    private bool _showFlag = false;

    public override void OnInit()
    {
        base.OnInit();
        _delayShowTime = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
    }

    public override void OnReset()
    {
        _showHandle = ~0u;
        _hideHandle = ~0u;
        _curTime = 0;
        _delayShowTime = 0;
        _showFlag = false;
}

    public override void OnStart()
    {
        base.OnStart();

        _showFlag = false;
        _HideSurFace();
        //_RegisterEvent();
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        _checkShow(deltaTime);
    }

    private void _checkShow(int deltaTime)
    {
        if (_showFlag) return;
        _curTime += deltaTime;
        if (_curTime < _delayShowTime) return;
        _showFlag = true;
        _ShowSurFace();
    }

    private void _ShowSurFace()
    {
        _SetComponentVisible(true);
        _showHandle = owner.m_pkGeActor.ChangeSurface("渐现", 0);
    }

    private void _HideSurFace()
    {
        _SetComponentVisible();
        _hideHandle = owner.m_pkGeActor.ChangeSurface("渐隐", 0);
    }

    private void _SetComponentVisible(bool isShow = false)
    {
        if (owner.m_pkGeActor == null) return;
        var rWeapon = owner.m_pkGeActor.GetAttachNode("[actor]RWeapon");
        var lWeapon = owner.m_pkGeActor.GetAttachNode("[actor]LWeapon");

        int layer = isShow ? 0 : 19;

        if (rWeapon != null)
            rWeapon.SetLayer(layer);

        if (lWeapon != null)
            lWeapon.SetLayer(layer);

        var wing = owner.m_pkGeActor.GetAttachment(Global.WING_ATTACH_NAME);
        if (wing != null)
            wing.SetLayer(layer);

        if (owner.m_pkGeActor.titleComponent != null)
            owner.m_pkGeActor.titleComponent.CustomActive(isShow);

        //owner.m_pkGeActor.GetEffectManager().SetEffectVisibleByLayer(isShow);

        if (owner.m_pkGeActor.goFootInfo != null)
            owner.m_pkGeActor.goFootInfo.CustomActive(isShow);
        if (owner.m_pkGeActor.goInfoBar != null)
            owner.m_pkGeActor.goInfoBar.CustomActive(isShow);
        owner.m_pkGeActor.SetHeadInfoVisible(isShow);
    }
#endif
}