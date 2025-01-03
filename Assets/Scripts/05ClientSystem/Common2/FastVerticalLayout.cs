using UnityEngine;
using System.Collections.Generic;

namespace GameClient
{
    class SortElement
    {
        public RectTransform rectTransform;
        public int iOrder;
        public static List<SortElement> ms_sortelements = new List<SortElement>();
        public static SortElement Alloc()
        {
            if(ms_sortelements.Count > 0)
            {
                SortElement ret = ms_sortelements[0];
                ms_sortelements.RemoveAt(0);
                return ret;
            }
            return new SortElement
            {
                rectTransform = null,
                iOrder = 0,
            };
        }
        public static void Delloc(SortElement sortElement)
        {
            if(sortElement != null)
            {
                sortElement.rectTransform = null;
                sortElement.iOrder = 0;
                ms_sortelements.Add(sortElement);
            }
        }
    }
    [RequireComponent(typeof(RectTransform))]
    class FastVerticalLayout : MonoBehaviour
    {
        public float fTop = 0.0f;
        public float fBottom = 0.0f;
        public float fInterval = 0.0f;
        public bool bReverse = false;
        bool m_bDirty = false;
        RectTransform rectTransForm;
        Vector2 tempVector = Vector2.zero;
        Vector3 tempVector3 = Vector3.zero;
        List<SortElement> m_akChilds = new List<SortElement>();
        // Use this for initialization
        void Start()
        {
            rectTransForm = GetComponent<RectTransform>();
            _InitRectTransForm(rectTransForm);
        }

        void OnEnable()
        {
            MarkDirty();
        }

        void OnDestroy()
        {
            for (int i = 0; i < m_akChilds.Count; ++i)
            {
                SortElement.Delloc(m_akChilds[i]);
            }
            m_akChilds.Clear();
        }
        int _SortLayout(SortElement left, SortElement right)
        {
            return right.iOrder - left.iOrder;
        }

        public void MarkDirty()
        {
            for (int i = 0; i < m_akChilds.Count; ++i)
            {
                SortElement.Delloc(m_akChilds[i]);
            }
            m_akChilds.Clear();
            for (int i = 0; i < transform.childCount; ++i)
            {
                int iCurIdx = bReverse ? (transform.childCount - 1 - i) : i;
                var current = transform.GetChild(iCurIdx) as RectTransform;
                if (current != null)
                {
                    LayoutSortOrder comOrder = current.gameObject.GetComponent<LayoutSortOrder>();
                    SortElement element = SortElement.Alloc();
                    element.rectTransform = current;
                    element.iOrder = comOrder == null ? 0 : comOrder.SortID;
                    m_akChilds.Add(element);
                }
            }
            if (m_akChilds.Count > 1)
            {
                m_akChilds.Sort(_SortLayout);
            }
            m_bDirty = true;
        }

        void _InitRectTransForm(RectTransform rectTransform)
        {
            if (rectTransform != null)
            {
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.zero;
                rectTransform.pivot = Vector2.zero;
                rectTransform.anchoredPosition = Vector2.zero;
            }
        }

        // Update is called once per frame
        void Update()
        {
            //if(m_bDirty)
            {
                float fPositionY = fTop;

                if(m_akChilds != null)
                {
                    for (int i = 0; i < m_akChilds.Count; ++i)
                    {
                        var current = m_akChilds[i].rectTransform;
                        if (current != null && current.gameObject.activeSelf)
                        {
                            tempVector3.y = fPositionY;
                            current.localPosition = tempVector3;
                            if (i != transform.childCount - 1)
                            {
                                fPositionY += fInterval + current.sizeDelta.y;
                            }
                            else
                            {
                                fPositionY += fBottom + current.sizeDelta.y;
                            }
                        }
                    }
                }

                if(rectTransForm != null)
                {
                    tempVector.x = rectTransForm.sizeDelta.x;
                    tempVector.y = fPositionY;
                    rectTransForm.sizeDelta = tempVector;
                }

                m_bDirty = false;
            }
        }
    }
}