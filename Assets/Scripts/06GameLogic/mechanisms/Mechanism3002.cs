using GameClient;


public class Mechanism3002 : BeMechanism
{
    public Mechanism3002(int mid, int lv) : base(mid, lv) { }

    private BeEntity topOwner = null;

    public override void OnReset()
    {
        topOwner = null;
    }

    public override void OnStart()
    {
        base.OnStart();
        if (owner.aiManager != null)
        {
            owner.aiManager.Stop();
        }

        topOwner = owner.GetTopOwner(owner);
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (topOwner != null)
        {
            var pos = topOwner.GetPosition();
            if (owner.isFloating)
            {
                pos.z = owner.floatingHeight.i;
            }
            else
            {
                pos.z = 0;
            }
            owner.SetPosition(pos);
            
            owner.SetFace(topOwner.GetFace());
        }
    }

    public override void OnFinish()
    {
        base.OnFinish();
        if (owner.aiManager != null)
        {
            owner.aiManager.Start();
        }
    }
}

