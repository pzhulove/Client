using GameClient;

/// <summary>
/// 接触地面时切换到到底或者倒地弹跳时 清除倒地标记
/// </summary>
public class Mechanism1171 : BeMechanism
{
    public Mechanism1171(int mid, int lv) : base(mid, lv)
    {

    }

    private bool _haveAttack;

    public override void OnStart()
    {
        base.OnStart();
        _haveAttack = false;
        _RegisterEvent();
    }

    private void _RegisterEvent()
    {
        handleNewA = owner.RegisterEventNew(BeEventType.onStateChangeEnd, _OnStateChangeEnd);
    }

    private void _OnStateChangeEnd(BeEvent.BeEventParam param)
    {
        if (_haveAttack) return;
        ActionState state = (ActionState)param.m_Int;
        if (state != ActionState.AS_FALLGROUND && state != ActionState.AS_FALLCLICK) return;
        _haveAttack = true;
        _ClearFallGroundTag();
        _RemoveAttachBuff();
    }

    private void _ClearFallGroundTag()
    {
        owner.SetTag((int)AState.AST_FALLGROUND, false);
    }

    private void _RemoveAttachBuff()
    {
        var attachBuff = GetAttachBuff();
        if (attachBuff == null) return;
        attachBuff.Finish();
    }
}