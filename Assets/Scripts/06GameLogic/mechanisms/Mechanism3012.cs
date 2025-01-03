/// <summary>
/// 同步拥有者到自身位置与面向
/// </summary>
public class Mechanism3012 : BeMechanism
{
    public Mechanism3012(int mid, int lv) : base(mid, lv) { }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        var actor = owner.GetOwner();
        if (actor != null)
        {
            actor.SetPosition(owner.GetPosition());
            actor.SetFace(owner.GetFace());
        }
    }
}
