/// <summary>
/// 当持续A时间处于idle，则移除指定buff，当不处于idle则重新开始计时
/// </summary>
public class Mechanism2142 : BeMechanism
{
    public Mechanism2142(int mid, int lv) : base(mid, lv)
    {
    }

    private int m_IdleTimeAcc;
    private int m_IdleTime;
    private int m_BuffId;
    public override void OnInit()
    {
        base.OnInit();

        m_IdleTime = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        m_BuffId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        m_IdleTimeAcc = 0;
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (owner.sgGetCurrentState() == (int)ActionState.AS_GRABBED 
            || owner.sgGetCurrentState() == (int)ActionState.AS_HURT 
            || owner.sgGetCurrentState() == (int)ActionState.AS_FALL
            || owner.sgGetCurrentState() == (int)ActionState.AS_BUSY)
        {
            m_IdleTimeAcc = 0;
        }
        else
        {
            m_IdleTimeAcc += deltaTime;
            if (m_IdleTimeAcc >= m_IdleTime)
            {
                owner.buffController.RemoveBuff(m_BuffId);
                BeBuff attachBuff = GetAttachBuff();
                if (attachBuff != null)
                    owner.buffController.RemoveBuff(attachBuff);
                else
                    Finish();

            }
        }
    }
}
