using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 增加/减少触发效果表的穿透率和僵直值
*/
public class Mechanism68 : BeMechanism
{
    protected List<int> m_HurtIdList = new List<int>();             //触发效果ID
    protected VRate m_AddHitThroughRateValue = 0;                   //增加穿透率
    protected int m_AddHardValue = 0;                               //增加僵直值（固定值）
    protected int m_AddHardValueRate = 0;                           //增加僵直值（百分比）
    protected int m_ScreenShakeID = -1;                             //震屏效果ID

    protected IBeEventHandle m_ChangeHitThrough = null;
    protected IBeEventHandle m_ChangeHardValue = null;
    protected IBeEventHandle m_ChangeScreenShakeID = null;

    public Mechanism68(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        m_HurtIdList.Clear();
        m_AddHitThroughRateValue = 0;
        m_AddHardValue = 0;
        m_AddHardValueRate = 0;
        m_ScreenShakeID = -1;
        m_ChangeHitThrough = null;
        m_ChangeHardValue = null;
        m_ChangeScreenShakeID = null;
    }
    public override void OnInit()
    {
        if (data.ValueA.Count > 0)
        {
            for(int i = 0; i < data.ValueA.Count; i++)
            {
                int hurtId = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
                m_HurtIdList.Add(hurtId);
            }
        };
        if (data.ValueB.Count > 0)
        {
            m_AddHitThroughRateValue = TableManager.GetValueFromUnionCell(data.ValueB[0],level);
        }

        if (data.ValueC.Count > 0)
        {
            m_AddHardValue = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        }

        if (data.ValueD.Count > 0)
        {
            m_AddHardValueRate = TableManager.GetValueFromUnionCell(data.ValueD[0],level);
        }

        if (data.ValueE.Count > 0)
        {
            m_ScreenShakeID = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        }
    }

    public override void OnStart()
    {
        RemoveHandle();
        m_ChangeHitThrough = owner.RegisterEventNew(BeEventType.onChangeHitThrough, (args) => 
        {
            int hurtId = (int)args.m_Int;
            if(m_HurtIdList.Count > 0 && m_HurtIdList.Contains(hurtId) && m_AddHitThroughRateValue != 0)
            {
                args.m_Rate += m_AddHitThroughRateValue;
            }
        });

        m_ChangeHardValue = owner.RegisterEventNew(BeEventType.onChangeHardValue, args =>
        {
            int hurtId = args.m_Int;
            if(m_HurtIdList.Count>0 && m_HurtIdList.Contains(hurtId))
            {
                if(m_AddHardValue!=0 || m_AddHardValueRate != 0)
                {
                    args.m_Int2 = ChangeHardValue(args.m_Int2);
                }
            }
        });

        if (m_ScreenShakeID != -1)
        {
            m_ChangeScreenShakeID = owner.RegisterEventNew(BeEventType.onChangeScreenShakeID, (args) =>
            {
                int hurtId = args.m_Int;
                if (m_HurtIdList.Count > 0 && m_HurtIdList.Contains(hurtId))
                {
                    args.m_Int2 = m_ScreenShakeID;
                }
            });
        }
    }
    
    //改变僵直值
    protected int ChangeHardValue(int handValue)
    {
        int ret = handValue;
        if (m_AddHardValue != 0)
        {
            ret += m_AddHardValue;
        }

        if (m_AddHardValueRate != 0)
        {
            ret *= (VFactor.one + new VFactor(m_AddHardValueRate, GlobalLogic.VALUE_1000)); 
        }

        return ret;
    }

    public override void OnFinish()
    {
        RemoveHandle();
    }

    protected void RemoveHandle()
    {
        if (m_ChangeHitThrough != null)
        {
            m_ChangeHitThrough.Remove();
            m_ChangeHitThrough = null;
        }

        if (m_ChangeHardValue != null)
        {
            m_ChangeHardValue.Remove();
            m_ChangeHardValue = null;
        }

        if (m_ChangeScreenShakeID != null)
        {
            m_ChangeScreenShakeID.Remove();
            m_ChangeScreenShakeID = null;
        }
    }
}
