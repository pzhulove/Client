using System;
using System.Collections.Generic;
using GameClient;
using UnityEngine;


/// <summary>
/// 当达到指定的连击次数时，后续每次攻击有一定概率给自己添加指定的BUFFinfo
/// </summary>
public class Mechanism1116 : BeMechanism
{
    struct ComboPrecentBuff
    {
        public int combo;
        public int buffInfoId;
        public int precent;
    }

    private List<ComboPrecentBuff> m_comboList = new List<ComboPrecentBuff>();
    private int m_TriggerCD = 0;
    private CoolDown m_CD = new CoolDown();
    public Mechanism1116(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        if (data.ValueA.Count != data.ValueB.Count || data.ValueA.Count != data.ValueC.Count)
        {
            Logger.LogError("1116配置文件，参数配置数量错误");
        }

        for (int i = 0; i < data.ValueA.Count; i++)
        {
            var item = new ComboPrecentBuff();
            item.combo = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
            item.buffInfoId = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
            item.precent = TableManager.GetValueFromUnionCell(data.ValueC[i], level);
            m_comboList.Add(item);
        }
        
        if (data.ValueD.Count > 0)
        {
            m_TriggerCD = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        }
    }

    public override void OnReset()
    {
        m_comboList.Clear();
        m_TriggerCD = 0;
        m_CD.Clear();
    }

    public override void OnStart()
    {
        handleA = OwnerRegisterEventNew(BeEventType.onHitOtherAfterHurt, OnHit);
        //handleA = owner.RegisterEvent(BeEventType.onHitOtherAfterHurt, OnHit);
    }

    private void OnHit(BeEvent.BeEventParam param)
    {
        if(owner == null)
            return;
        
        if(owner.actorData == null)
            return;

        if(m_TriggerCD > 0 && m_CD.IsCD())
            return;
        
        if (m_TriggerCD > 0)
        {
            m_CD.StartCD(m_TriggerCD);
        }
        
        for (int i = m_comboList.Count - 1; i >= 0; i--)
        {
            var item = m_comboList[i];
            if (item.combo <= owner.actorData.GetCurComboCount())
            {
                if (FrameRandom.Range1000() <= item.precent)
                {
                    owner.buffController.TryAddBuffInfo(item.buffInfoId, owner, level);
                }

                return;
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
}

