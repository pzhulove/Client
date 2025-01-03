using System;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

/// <summary>
/// 当战法生成或者发射炫纹时（条件可以两个都填），有概率给自己添加BUFFinfo，这个效果可以设置CD
/// </summary>
public class Mechanism2090 : BeMechanism
{
    private CoolDown m_CD = new CoolDown();

    private bool m_IsAddTrigger = false;
    private bool m_IsRemoveTrigger = false;
    private int m_BuffInfoId = 0;
    private int m_TriggerCD = 0;
    private int m_BuffPercent = 1000;
    
    protected List<BeEvent.BeEventHandleNew> m_HandleNewList = new List<BeEvent.BeEventHandleNew>();

    public Mechanism2090(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        m_IsAddTrigger = TableManager.GetValueFromUnionCell(data.ValueA[0], level) > 0;
        m_IsRemoveTrigger = TableManager.GetValueFromUnionCell(data.ValueA[1], level) > 0;
        
        m_BuffInfoId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        
        if (data.ValueC.Count > 0)
        {
            m_BuffPercent = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        }

        if (data.ValueD.Count > 0)
        {
            m_TriggerCD = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        }
    }

    public override void OnReset()
    {
        m_BuffPercent = 1000;
        m_TriggerCD = 0;
        m_HandleNewList.Clear();
        m_CD.Clear();
    }

    public override void OnStart()
    {
        if(owner == null)
            return;
        
        m_HandleNewList.Add(owner.RegisterEventNew(BeEventType.onAddChaser, OnAddChaser));
        m_HandleNewList.Add(owner.RegisterEventNew(BeEventType.onRemoveChaser, OnRemoveChaser));
    }

    private void OnAddChaser(BeEvent.BeEventParam param)
    {
        if(!m_IsAddTrigger)
            return;
        
        AddBuff();
    }

    
    private void OnRemoveChaser(BeEvent.BeEventParam param)
    {
        if(!m_IsRemoveTrigger)
            return;
        
        // 当炫纹强压，炫纹融合时，可能一次消耗多个炫纹，这时也算1次addBuff
        AddBuff();
    }
    
    
    private void AddBuff()
    {
        if(m_TriggerCD > 0 && m_CD.IsCD())
            return;
        
        //给自己添加的Buff信息
        if (m_BuffInfoId != 0)
        {
            if (FrameRandom.Range1000() <= m_BuffPercent)
            {
                if (owner != null && owner.buffController != null)
                {
                    if (owner.buffController.TryAddBuffInfo(m_BuffInfoId, owner, level) != null)
                    {
                        if (m_TriggerCD > 0)
                        {
                            m_CD.StartCD(m_TriggerCD);
                        }
                    }   
                }
            }
        }
    }


    public override void OnUpdate(int deltaTime)
    {
        if (m_TriggerCD > 0)
        {
            m_CD.UpdateCD(deltaTime);
        }
    }

    public override void OnFinish()
    {
        for (int i = 0; i < m_HandleNewList.Count; i++)
        {
            m_HandleNewList[i].Remove();
            m_HandleNewList[i] = null;
        }
        m_HandleNewList.Clear();
    }
    
}

