
using GameClient;

public interface IBeSkillSkipToNextPhase
{
    void DealSwitch();
    void DealSwitchInPhaseFinish();
    void DeInIt();
}

/// <summary>
/// 帧标签中跳转到下个阶段
/// </summary>
public class BeSkillSkipToNextPhase : IBeSkillSkipToNextPhase
{
    private enum AttackHit
    {
        None,
        NoHit,
        Hit,
    }

    private BeActor _actor;
    private AttackHit _attackHitResult = AttackHit.None;
    private IBeEventHandle _skipToNextPhaseHandle;

    public BeSkillSkipToNextPhase(BeActor actor)
    {
        _actor = actor;
        _Init();
    }

    private void _Init()
    {
        _attackHitResult = AttackHit.None;
        _skipToNextPhaseHandle = _actor.RegisterEventNew(BeEventType.onHitOtherAfterHurt, _OnHitOtherAfterHurt);
    }

    private void _OnHitOtherAfterHurt(BeEvent.BeEventParam param)
    {
        _attackHitResult = AttackHit.Hit;
    }

    public void DeInIt()
    {
        _attackHitResult = AttackHit.None;
        if (_skipToNextPhaseHandle != null)
        {
            _skipToNextPhaseHandle.Remove();
            _skipToNextPhaseHandle = null;
        }
    }

    public void DealSwitch()
    {
        //切换到下个阶段
        if (_attackHitResult == AttackHit.Hit)
            _actor.SwitchToNextPahseSkill();
        else
            _attackHitResult = AttackHit.NoHit;
    }

    /// <summary>
    /// 阶段结束时 如果没有命中则跳转到下个阶段
    /// </summary>
    public void DealSwitchInPhaseFinish()
    {
        if (_attackHitResult != AttackHit.NoHit) return;
        _actor.SwitchToNextPahseSkill();
    }
}