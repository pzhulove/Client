using UnityEngine;
using System.Collections;
using UnityEngine.UI;

class SmartTipContent : MonoBehaviour
{
    public Text m_kObjectLeft;
    public Text m_kObjectRight;

    protected bool m_dirty = false;
    protected LayoutElement m_kElement;
    protected RectTransform m_Rect;

    public void SetText(string left,string right)
    {
        m_kObjectLeft.text = left;
        m_kObjectRight.text = right;
        m_dirty = true;
    }

    public void LateUpdate()
    {
        if(!m_dirty)
        {
            return;
        }
        m_dirty = true;

        if (m_kElement == null)
        {
            m_kElement = transform.GetComponent<LayoutElement>();
            m_Rect = (transform as RectTransform);
        }

        if (m_kElement == null)
        {
            return;
        }

        float fPreferredH = Mathf.Max(LayoutUtility.GetPreferredSize(m_kObjectLeft.rectTransform, (int)RectTransform.Axis.Vertical),
LayoutUtility.GetPreferredSize(m_kObjectRight.rectTransform, (int)RectTransform.Axis.Vertical));

        m_kElement.preferredHeight = fPreferredH;
    }
}