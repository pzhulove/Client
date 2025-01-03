/*
 * 替换实体ID
*/
public class Mechanism64 : BeMechanism
{
    protected int m_OriginalEntityId = 0;           //原来的实体ID
    protected int m_ReplaceEntityId = 0;            //替换后的实体ID
    protected int m_AddEffectId = -1;               //附加伤害效果ID

    protected IBeEventHandle m_ReplaceEntity = null;//替换实体

    public Mechanism64(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        m_AddEffectId = -1;
        m_ReplaceEntity = null;
    }
    public override void OnInit()
    {
        m_OriginalEntityId = TableManager.GetValueFromUnionCell(data.ValueA[0],level);
        m_ReplaceEntityId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        if (data.ValueC.Count > 0)
        {
            m_AddEffectId = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        RemoveHandle();
        m_ReplaceEntity = owner.RegisterEventNew(BeEventType.onBeforeGenBullet, (args) => 
        {
            if (args.m_Int == m_OriginalEntityId)
            {
                args.m_Int = m_ReplaceEntityId;

                if (m_AddEffectId > 0)
                {
                    args.m_Int2 = m_AddEffectId;
                }
            }
        });
    }

    public override void OnFinish()
    {
        RemoveHandle();
    }

    protected void RemoveHandle()
    {
        if (m_ReplaceEntity != null)
        {
            m_ReplaceEntity.Remove();
            m_ReplaceEntity = null;
        }
    }
}
