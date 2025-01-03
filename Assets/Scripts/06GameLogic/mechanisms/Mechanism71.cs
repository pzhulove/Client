using UnityEngine;
using System.Collections.Generic;
using GameClient;

/*
 * 复制实体
*/
public class Mechanism71 : BeMechanism
{
    protected List<int> m_EntityIdList = new List<int>();           //需要复制的实体ID
    protected int m_DelayTime = 0;                                  //每一个延时创建的时间
    protected int m_CopyNum = 1;                                    //实体复制数量

    protected IBeEventHandle m_CopyEntity = null;                   //复制实体

    public Mechanism71(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        m_EntityIdList.Clear();
        m_DelayTime = 0;
        m_CopyNum = 1; 
        m_CopyEntity = null; 
    }    
    public override void OnInit()
    {
        if (data.ValueA.Count > 0)
        {
            for(int i = 0; i < data.ValueA.Count; i++)
            {
                int entityId = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
                m_EntityIdList.Add(entityId);
            }
        }

        if (data.ValueB.Count > 0)
        {

            m_DelayTime = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        }

        if (data.ValueC.Count > 0)
        {
            m_CopyNum = TableManager.GetValueFromUnionCell(data.ValueC[0],level);
        }
    }

    public override void OnStart()
    {
        RemoveHandle();
        m_CopyEntity = owner.RegisterEventNew(BeEventType.onChangeLaunchProNum, (args) => 
        {
            int resId = args.m_Int;
            if(m_EntityIdList.Count > 0 && m_EntityIdList.Contains(resId))
            {
                args.m_Int2 = m_DelayTime;
                args.m_Int3 = m_CopyNum;
            }
        });
    }

    public override void OnFinish()
    {
        RemoveHandle();
    }

    protected void RemoveHandle()
    {
        if (m_CopyEntity != null)
        {
            m_CopyEntity.Remove();
            m_CopyEntity = null;
        }
    }
}
