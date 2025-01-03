/// <summary>
/// 在机制期间，自身不会随生命周期死亡，在开始时+10S，在结束时-10S
/// </summary>
public class Mechanism3007 : BeMechanism
{
    public Mechanism3007(int mid, int lv) : base(mid, lv)
    {
    }

    private int m_BuffTime = GlobalLogic.VALUE_10000;

    public override void OnInit()
    {
        base.OnInit();
        if (data.ValueA.Count > 0)
        {
            m_BuffTime = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        }
    }

    public override void OnReset()
    {
        m_BuffTime = GlobalLogic.VALUE_10000;
    }

    public override void OnStart()
    {
        base.OnStart();
        var buff = owner.buffController.HasBuffByID((int) GlobalBuff.LIFE_TIME);
        if (buff != null)
        {
            buff.duration += m_BuffTime;
        }
    }

    public override void OnFinish()
    {
        base.OnFinish();
        var buff = owner.buffController.HasBuffByID((int) GlobalBuff.LIFE_TIME);
        if (buff != null)
        {
            buff.duration -= m_BuffTime;
        }
    }
}