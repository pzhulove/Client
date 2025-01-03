using System.Collections.Generic;
using GameClient;
using Spine;

/// <summary>
/// 在上机制时，根据权重添加BuffInfo。支持触发次数。
/// A：BuffInfoId列表
/// B：权重列表
/// C：次数
/// </summary>
public class Mechanism1509 : BeMechanism
{
    public Mechanism1509(int mid, int lv) : base(mid, lv)
    {
    }

    private struct BuffInfoRateData
    {
        public int buffInfoId;
        public int rate;
    }
    
    private List<BuffInfoRateData> list = new List<BuffInfoRateData>();
    private int m_AddBuffInterval = 0;

    public override void OnInit()
    {
        base.OnInit();
        for (int i = 0; i < data.ValueA.Count && i < data.ValueB.Count; i++)
        {
            var item = new BuffInfoRateData();
            item.buffInfoId = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
            item.rate = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
            list.Add(item);
        }

        if (data.ValueC.Count > 0)
        {
            m_AddBuffInterval = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        }
    }

    public override void OnReset()
    {
        list.Clear();
        m_AddBuffInterval = 0;
    }

    public override void OnStart()
    {
        base.OnStart();
        if (m_AddBuffInterval > 0)
        {
            InitTimeAcc(m_AddBuffInterval);
        }
        else
        {
            AddBuffInfo();
        }
    }
    
    public override void OnUpdateTimeAcc()
    {
        if(m_AddBuffInterval > 0)
            AddBuffInfo();
    }

    private void AddBuffInfo()
    {
        int totalRate = 0;
        for (int i = 0; i < list.Count; i++)
        {
            totalRate += list[i].rate;
        }

        int rate = FrameRandom.Random((uint) totalRate);
        int curRate = 0;
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            curRate += item.rate;
            if (rate < curRate)
            {
                owner.buffController.TryAddBuffInfo(item.buffInfoId, owner, level);
                break;
            }
        }
    }


}