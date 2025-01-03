using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using GameClient;
using UnityEngine.UI;
using UnityEngine.Serialization;
using System;

public class ActiveVarBinder : MonoBehaviour
{
    public enum DataSource
    {
        DS_TEMPLATE,
        DS_ACTIVEITEM,
    }
    public DataSource m_eDataSource = DataSource.DS_TEMPLATE;
    public int m_iDataID = 0;

    public enum CompareType
    {
        CT_GREAT = 0,
        CT_LESS,
        CT_EQUAL,
        CT_GREAT_EQUAL,
        CT_LESS_EQUAL,
    }

    public CompareType m_eCompareType = CompareType.CT_GREAT;
    public string m_kKey;
    public int m_iCompareValue;

    [SerializeField]
    public ConditionTrigger m_OnSuccessTrigger = new ConditionTrigger();

    [SerializeField]
    public ConditionTrigger m_OnFailedTrigger = new ConditionTrigger();

    void OnDestroy()
    {
        m_OnSuccessTrigger = null;
        m_OnFailedTrigger = null;
    }

    public void RefreshStatus()
    {
        int iSrcValue = 0;
        if (m_eDataSource == DataSource.DS_TEMPLATE)
        {
            iSrcValue = ActiveManager.GetInstance().GetTemplateValue(m_iDataID, m_kKey);
        }
        else if(m_eDataSource == DataSource.DS_ACTIVEITEM)
        {
            iSrcValue = ActiveManager.GetInstance().GetActiveItemValue(m_iDataID, m_kKey);
        }
        else
        {
            return;
        }

        bool bCompareValue = false;
        switch (m_eCompareType)
        {
            case CompareType.CT_EQUAL:
                {
                    bCompareValue = iSrcValue == m_iCompareValue;
                }
                break;
            case CompareType.CT_GREAT:
                {
                    bCompareValue = iSrcValue > m_iCompareValue;
                }
                break;
            case CompareType.CT_GREAT_EQUAL:
                {
                    bCompareValue = iSrcValue >= m_iCompareValue;
                }
                break;
            case CompareType.CT_LESS:
                {
                    bCompareValue = iSrcValue < m_iCompareValue;
                }
                break;
            case CompareType.CT_LESS_EQUAL:
                {
                    bCompareValue = iSrcValue <= m_iCompareValue;
                }
                break;
        }

        if (bCompareValue)
        {
            if (m_OnSuccessTrigger != null && m_OnSuccessTrigger.m_TimeEvent != null)
            {
                m_OnSuccessTrigger.m_TimeEvent.Invoke();
            }
        }
        else
        {
            if (m_OnFailedTrigger != null && m_OnFailedTrigger.m_TimeEvent != null)
            {
                m_OnFailedTrigger.m_TimeEvent.Invoke();
            }
        }
    }
}

[Serializable]
public class ConditionTrigger
{
    [Serializable]
    public class ConditionEvent : UnityEvent { }

    [SerializeField]
    public ConditionEvent m_TimeEvent = new ConditionEvent();
}