using GameClient;

/// <summary>
/// 药剂-树人 盾机制
/// 承受ValueA次伤害，异常伤害不计算在内。
/// 当伤害小于最大值。则飘0
/// 当伤害大于最大值。飘Hurt-Max
/// 当次数用完则移除BUFF.会触发合体技能结束
/// </summary>
public class Mechanism3014 : BeMechanism
{
    public Mechanism3014(int mid, int lv) : base(mid, lv)
    {
    }

    private int m_HurtCount;
    private int m_DefHurtMaxValue;
    private int m_HurtCountAcc;
    public override void OnInit()
    {
        base.OnInit();
        m_HurtCount = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        m_DefHurtMaxValue = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnReset()
    {
        base.OnReset();
        m_HurtCount = 0;
        m_DefHurtMaxValue = 0;
    }

    public override void OnStart()
    {
        base.OnStart();
        m_HurtCountAcc = 0;
        handleA = owner.RegisterEventNew(BeEventType.onChangeHurtValue, onChangeHurtValue);
    }

    private void onChangeHurtValue(BeEvent.BeEventParam param)
    {
        // 异常伤害不计算在内
        if (param.m_Int2 == (int) HitDamageType.AbnormalBuff)
            return;

        if (m_HurtCountAcc >= m_HurtCount)
        {
            RemoveAttachBuff();
            return;
        }
        
        if (param.m_Int > m_DefHurtMaxValue)
        {
            param.m_Int -= m_DefHurtMaxValue;
        }
        else
        {
            param.m_Int = 0;
        }

        m_HurtCountAcc++;

        if (m_HurtCountAcc >= m_HurtCount)
        {
            RemoveAttachBuff();
        }
    }
}
