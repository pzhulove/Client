using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    [RequireComponent(typeof(RectTransform))]
    class ComLayoutMax : MonoBehaviour
    {
        public Vector2 MaxSize;
        public RectTransform BaseRect;

        RectTransform m_rect;

        public void Awake()
        {
            m_rect = GetComponent<RectTransform>();
        }

        public void Update()
        {
            if (m_rect != null && BaseRect != null)
            {
                if (BaseRect.sizeDelta.x <= MaxSize.x)
                {
                    m_rect.sizeDelta = new Vector2(BaseRect.sizeDelta.x, m_rect.sizeDelta.y);
                }
                else
                {
                    m_rect.sizeDelta = new Vector2(MaxSize.x, m_rect.sizeDelta.y);
                }

                if (BaseRect.sizeDelta.y <= MaxSize.y)
                {
                    m_rect.sizeDelta = new Vector2(m_rect.sizeDelta.x, BaseRect.sizeDelta.y);
                }
                else
                {
                    m_rect.sizeDelta = new Vector2(m_rect.sizeDelta.x, MaxSize.y);
                }
            }
        }
    }
}
