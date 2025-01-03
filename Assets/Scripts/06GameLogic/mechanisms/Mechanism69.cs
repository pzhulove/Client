using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 增加/减少特定Buff时间
*/
public class Mechanism69 : BeMechanism
{
    public enum ChangeBuffType
    {
        CHANGE_BUFF = 0,                                        //增加/减少身上已有Buff的时间
        TRIGGER_CHANGE_BUFF = 1,                                //监听Buff添加，增加/减少Buff时间
    }

    protected ChangeBuffType m_ChangeBuffType = ChangeBuffType.CHANGE_BUFF;        
    protected List<int> m_BuffIdList = new List<int>();         //BuffId
    protected int m_AddBuffTime = 0;                            //增加Buff时间
    protected int m_AddBuffTimeRate = 0;                        //增加时间千分比
    protected bool relateByLevel = false;                       //增加固定值随机制等级成长

    public Mechanism69(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        m_ChangeBuffType = ChangeBuffType.CHANGE_BUFF;
        m_BuffIdList.Clear();
        m_AddBuffTime = 0;
        m_AddBuffTimeRate = 0;
        relateByLevel = false;
    }
    public override void OnInit()
    {
        if (data.ValueA.Count > 0)
        {
            m_ChangeBuffType = (ChangeBuffType)TableManager.GetValueFromUnionCell(data.ValueA[0],level);
        }

        if (data.ValueB.Count > 0)
        {
            for(int i = 0; i < data.ValueB.Count; i++)
            {
                int buffId = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
                m_BuffIdList.Add(buffId);
            }
        }

        if (data.ValueC.Count>0)
        {
            m_AddBuffTime = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        }

        if (data.ValueD.Count > 0)
        {
            m_AddBuffTimeRate = TableManager.GetValueFromUnionCell(data.ValueD[0],level);
        }

        if (data.ValueE.Count > 0)
            relateByLevel = TableManager.GetValueFromUnionCell(data.ValueE[0], level) == 1 ? true : false;
    }

    public override void OnStart()
    {
        if(m_ChangeBuffType == ChangeBuffType.TRIGGER_CHANGE_BUFF)                      //监听Buff添加并改变Buff时间
        {
            RemoveHandle();
            handleA = owner.RegisterEventNew(BeEventType.onAddBuff, (args) =>
            {
                BeBuff buff = (BeBuff)args.m_Obj;
                if (m_BuffIdList.Count > 0 && m_BuffIdList.Contains(buff.buffID))
                {
                    ChangeBuffTime(buff);
                }
            });

            handleB = owner.RegisterEventNew(BeEventType.onBuffRefresh, args =>
            {
                int buffId = args.m_Int;
                if (m_BuffIdList.Count > 0 && m_BuffIdList.Contains(buffId))
                {
                    args.m_Int2 = RefreshBuffTime(args.m_Int2);
                }
            });
        }
        else if(m_ChangeBuffType == ChangeBuffType.CHANGE_BUFF)                         //机制添加的时候改变符合条件Buff的时间
        {
            if (m_BuffIdList.Count > 0)
            {
                for(int i = 0; i < m_BuffIdList.Count; i++)
                {
                    BeBuff buff = owner.buffController.HasBuffByID(m_BuffIdList[i]);
                    if (buff != null)
                    {
                        ChangeBuffTime(buff);
                    }
                }
            }
        }
    }

    protected void ChangeBuffTime(BeBuff buff)
    {
        if (m_AddBuffTime != 0)
        {
            buff.duration += relateByLevel? level * m_AddBuffTime : m_AddBuffTime;
        }

        if (m_AddBuffTimeRate != 0)
        {
            buff.duration *= (VFactor.one + new VFactor(m_AddBuffTimeRate, GlobalLogic.VALUE_1000));
        }

        buff.duration = Mathf.Max(buff.duration,1);         //添加一个保护 防止Buff时间改为0
    }

    protected int RefreshBuffTime(int time)
    {
        int ret = time;
        if (m_AddBuffTime != 0)
        {
            ret += relateByLevel ? level * m_AddBuffTime : m_AddBuffTime;
        }

        if (m_AddBuffTimeRate != 0)
        {
            ret *= (VFactor.one + new VFactor(m_AddBuffTimeRate, GlobalLogic.VALUE_1000));
        }

        ret = Mathf.Max(ret, 1);         //添加一个保护 防止Buff时间改为0
        return ret;
    }

    protected void RemoveHandle()
    {
        if (handleA != null)
        {
            handleA.Remove();
            handleA = null;
        }

        if (handleB != null)
        {
            handleB.Remove();
            handleB = null;
        }
    }
}
