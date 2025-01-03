using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

class DelayScrollRect : UIBehaviour
{
    public UnityEngine.UI.ScrollRect m_kScrollRect;
    float m_fValue = 0.0f;
    bool m_bDirty = false;

    public void SetValue(float fValue)
    {
        m_fValue = fValue;
        m_bDirty = true;
    }

    protected override void OnRectTransformDimensionsChange()
    {
        if(m_bDirty == true)
        {
            if (m_kScrollRect.vertical == true)
            {
                m_kScrollRect.verticalNormalizedPosition = m_fValue;
            }
            else if (m_kScrollRect.horizontal == true)
            {
                m_kScrollRect.horizontalNormalizedPosition = m_fValue;
            }
            m_bDirty = false;
        }

        base.OnRectTransformDimensionsChange();
    }
}