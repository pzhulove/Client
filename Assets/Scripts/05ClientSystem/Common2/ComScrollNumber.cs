using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GameClient
{
    public class ComScrollNumber : MonoBehaviour
    {
        public RectTransform[] m_rects = new RectTransform[10];
        public Image[] m_Imgs = new Image[10];
        public Vector3[] m_positions = new Vector3[10];
        public float m_fSpeed = 124.0f;
        public UnityEvent onActionEnd = null;
        List<int> m_Indexs = new List<int>();
        float m_position = 0.0f;
        bool m_start = false;
        int m_endIndex = 9;

        // Use this for initialization
        void Start()
        {

        }

        public void Trigger(int iEndIndex)
        {
            if(m_start)
            {
                return;
            }
            m_start = true;
            m_endIndex = iEndIndex;
            m_position = 0.0f;
        }

        public void Run(int iStart,int iEnd)
        {
            if (m_start)
            {
                return;
            }
            SetPositionImmediately(iStart);
            Trigger(iEnd);
        }

        public void SetPositionImmediately(int iIndex)
        {
            if(m_Indexs.Count < m_Imgs.Length)
            {
                m_Indexs.Clear();
                for (int i = 0; i < m_Imgs.Length; ++i)
                {
                    m_Indexs.Add(i);
                }
            }
            int iLoopTimes = m_Indexs.Count;
            while (m_Indexs[0] != iIndex && iLoopTimes > 0)
            {
                var tmp = m_Indexs[0];
                m_Indexs.RemoveAt(0);
                m_Indexs.Add(tmp);
                --iLoopTimes;
            }
            m_position = 0.0f;
            _updatePosition();
        }

        void _updatePosition()
        {
            for (int i = 0; i < m_Indexs.Count; ++i)
            {
                m_rects[m_Indexs[i]].anchoredPosition = new Vector2(m_rects[m_Indexs[i]].anchoredPosition.x, m_position + m_positions[i].y);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(m_start)
            {
                m_position += m_fSpeed * Time.deltaTime;
                _updatePosition();

                while (m_Indexs[0] != m_endIndex && m_position >= m_rects[0].sizeDelta.y)
                {
                    m_position -= m_rects[0].sizeDelta.y;
                    var head = m_Indexs[0];
                    m_Indexs.RemoveAt(0);
                    m_Indexs.Add(head);
                }

                if(m_Indexs[0] == m_endIndex)
                {
                    m_position = 0.0f;
                    _updatePosition();
                    if (null != onActionEnd)
                    {
                        onActionEnd.Invoke();
                    }
                    m_start = false;
                }
            }
            else
            {
                _updatePosition();
            }
        }
    }
}