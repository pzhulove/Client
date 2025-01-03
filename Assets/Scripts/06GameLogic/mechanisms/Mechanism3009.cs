/// <summary>
/// 集火逻辑
/// A: 对应的集火Buff机制ID（用于支持多套集火共存）
/// </summary>
public class Mechanism3009 : LockAttackMechanism
{
    public Mechanism3009(int mid, int lv) : base(mid, lv) { }

    protected override void OnForceTargetChange()
    {
        UpdateAITarget();
    }

    private void UpdateAITarget()
    {
        if (owner.aiManager == null)
            return;
        
        if (ForceTarget == null)
        {
            owner.aiManager.targetUnchange = false;
        }
        else
        {
            owner.aiManager.SetTarget(ForceTarget, true);
            owner.aiManager.ResetAction();
            owner.aiManager.ResetDestinationSelect();
        }
    }
}