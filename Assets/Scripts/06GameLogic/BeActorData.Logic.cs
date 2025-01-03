using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;

/// <summary>
/// 连招数据  逻辑上使用 可以用于验证服务器
/// </summary>
public partial class BeActorData
{
    BeEvent.BeEventHandleNew m_RecordComboHandle;
    protected BeActor m_RecordOwner;
    protected BeActor m_TopOwner;
    protected int m_CurComboCount;
    protected bool m_IsComboRecord = false;

    private readonly int comboDefaultIntervel = 1250;
    protected int m_RecordComboIntervel = 1250;
    protected int m_RecordComboTimeAcc;

    public void InitData(BeActor actor)
    {
        if (actor == null)
            return;

        m_RecordComboIntervel = comboDefaultIntervel;
        m_RecordOwner = actor;
        m_RecordComboHandle = actor.RegisterEventNew(BeEventType.onHitOther, RegisterHitOtherEvent);
        //m_RecordComboHandle = actor.RegisterEvent(BeEventType.onHitOther, RegisterHitOtherEvent);
    }

    protected void RegisterHitOtherEvent(BeEvent.BeEventParam param)
    {
        m_TopOwner = m_RecordOwner.GetTopOwner(m_RecordOwner) as BeActor;
        
        if (m_TopOwner == null)
        {
            RecordComboData();
        }
        else
        {
            m_TopOwner.actorData.RecordComboData();
        }
    }

    /// <summary>
    /// 更新连招记录数据
    /// </summary>
    /// <param name="iDeltaTime"></param>
    public void UpdateRecordCombo(int iDeltaTime)
    {
        if (!m_RecordOwner.isMainActor)
            return;
        
        if (!m_IsComboRecord)
            return;
        if (m_RecordComboTimeAcc > 0)
        {
            m_RecordComboTimeAcc -= iDeltaTime;
        }
        else
        {
            if (m_TopOwner == null)
            {
                StopRecordCombo();
            }
            else
            {
                m_TopOwner.actorData.StopRecordCombo();
            }
        }
    }

    /// <summary>
    /// 变身状态下 需要更新被变身玩家的连招数据
    /// </summary>
    /// <param name="iDeltaTime"></param>
    protected void UpdateTopOwnerCombo(int iDeltaTime)
    {
        if (!m_RecordOwner.isSpecialMonster)
            return;
        
        if (m_TopOwner == null)
            return;

        if(!m_TopOwner.actorData.m_IsComboRecord)
            return;
        
        if (m_TopOwner.actorData.m_RecordComboTimeAcc > 0)
        {
            m_TopOwner.actorData.m_RecordComboTimeAcc -= iDeltaTime;
        }
        else
        {
            m_TopOwner.actorData.StopRecordCombo();
        }
    }

    public void UpdateLogic(int iDeltaTime)
    {
        UpdateRecordCombo(iDeltaTime);
        if (isSpecialMonster)
        {
            UpdateTopOwnerCombo(iDeltaTime);
        }
    }
    
    /// <summary>
    /// 记录连招数据
    /// </summary>
    protected void RecordComboData()
    {
        if (!m_RecordOwner.isMainActor)
            return;
        
        m_RecordComboTimeAcc = m_RecordComboIntervel;
        m_CurComboCount++;
        m_IsComboRecord = true;
        if (m_RecordOwner != null)
        {
            m_RecordOwner.TriggerEventNew(BeEventType.onBattleCombo);
        }

        //Logger.LogError("++ combo:" + m_CurComboCount);
    }

    /// <summary>
    /// 停止记录连招数据
    /// </summary>
    protected void StopRecordCombo()
    {
       // Logger.LogError("stop combo");
        m_IsComboRecord = false;
        m_CurComboCount = 0;
        
        if (m_RecordOwner != null)
        {
            m_RecordOwner.TriggerEventNew(BeEventType.onBattleComboStop);
        }
    }

    /// <summary>
    /// 获取当前连击次数
    /// </summary>
    /// <returns>次数</returns>
    public int GetCurComboCount()
    {
        return m_CurComboCount;
    }

    
    /// <summary>
    /// 重置combo间隔
    /// </summary>
    public void ResetComboIntervel()
    {
        m_RecordComboIntervel = comboDefaultIntervel;
        comboIntervel = comboDefaultIntervel;
    }

    /// <summary>
    /// 设置combo间隔
    /// </summary>
    /// <param name="intervel"></param>
    public void SetComboIntervel(int intervel)
    {
        m_RecordComboIntervel = intervel;
        comboIntervel = intervel;
    }
    
    /// <summary>
    /// 移除逻辑上面监听的事件
    /// </summary>
    protected void RemoveLogicHandle()
    {
        if (m_RecordComboHandle != null)
        {
            m_RecordComboHandle.Remove();
            m_RecordComboHandle = null;
        }
    }
}
