using UnityEngine;

/// <summary>
/// 自动炫纹机制,随Buff生命周期
/// </summary>
public class Mechanism2078 : BeMechanism
{
    public Mechanism2078(int mid, int lv) : base(mid, lv)
    {
    }

    public override void OnStart()
    {
        base.OnStart();

        SetAutoChaser(owner, true);
    }

    public override void OnFinish()
    {
        base.OnFinish();

        SetAutoChaser(owner, false);
    }

    private void SetAutoChaser(BeActor owner, bool active)
    {
        if (owner == null)
            return;

        var mechaism = owner.GetMechanism(Mechanism2072.ChaserMgrID);
        if (mechaism == null)
            return;

        Mechanism2072 chaserMgr = mechaism as Mechanism2072;
        if (chaserMgr != null)
        {
            chaserMgr.SetAutoChaserFlag(active, level);
        }
    }
}