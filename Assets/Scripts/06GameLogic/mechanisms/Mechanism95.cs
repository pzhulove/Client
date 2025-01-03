public class Mechanism95 : BeMechanism
{
    protected BeActor.NormalAttack m_AttackData = new BeActor.NormalAttack();     //备份替换普攻数据
    protected int m_ReplaceAttackId;             //替换的普攻ID
    public Mechanism95(int mid, int lv) : base(mid, lv)
    {
    }

    public override void OnReset()
    {
        base.OnReset();
        m_ReplaceAttackId = 0;
        m_AttackData = default;
    }

    public override void OnInit()
    {
        base.OnInit();
        m_ReplaceAttackId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        m_AttackData = owner.AddReplaceAttackId(m_ReplaceAttackId, 2);
    }
    
    public override void OnFinish()
    {
        owner.RemoveReplaceAttackId(m_AttackData);
    }
}
