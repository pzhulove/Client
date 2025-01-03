using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace GameClient
{
    class ComBlueCircle : MonoBehaviour
    {

        // Use this for initialization
        ComMapScene m_scene = null;
        Vector2 m_TargetCenter = Vector2.zero;
        Vector2 m_SrcCenter = Vector2.zero;
        Vector2 m_CurCenter = Vector2.zero;
        float m_SrcRadius = 0.0f;
        float m_TgtRadius = 0.0f;
        float m_CurRadius = 0.0f;
        float m_durTime = 0.0f;
        float m_shrinkTime = 0.0f;
        Image m_Image = null;
        bool startShrink = false;
        RectTransform imageTrans;
        public bool IsShrink { get { return startShrink; } }
        public ComMapScene scene { get { return m_scene; } }
        public void ResetSource(float sourceRadius,Vector2 srcPos)
        {
            m_SrcCenter = srcPos;
            m_SrcRadius = sourceRadius;
            m_CurCenter = m_SrcCenter;
            m_CurRadius = m_SrcRadius;
        }
        void Start()
        {
            m_Image = GetComponent<Image>();
            imageTrans = m_Image.GetComponent<RectTransform>();
        }
        public void Setup(Vector2 pos, float radius, float elapseTime, float durTime,ComMapScene a_comScene)
        {
            m_scene = a_comScene;
          
            if (durTime != 0.0f)
            {
                m_TargetCenter = pos;
                m_SrcCenter = m_CurCenter;
                m_SrcRadius = m_CurRadius;
                m_TgtRadius = radius;
                startShrink = true;
            }
            else
            {
                m_SrcCenter = pos;
                m_CurCenter = pos;
                m_TargetCenter = pos;
                m_CurRadius = radius;
                m_TgtRadius = radius;
                m_SrcRadius = radius;
                
            }
            m_shrinkTime = durTime;
            m_durTime = elapseTime;
            gameObject.SetActive(true);
            gameObject.transform.SetParent(m_scene.gameObject.transform, false);
            Set();
            if(!startShrink)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PoisonNextStage);
            }
        }
        void Set()
        {
          //  gameObject.transform.localPosition = new Vector3(m_CurCenter.x * m_scene.XRate, m_CurCenter.y * m_scene.ZRate, 0.0f);
            if(imageTrans == null)
           {
                m_Image = GetComponent<Image>();
                imageTrans = m_Image.GetComponent<RectTransform>();
            }
            imageTrans.anchoredPosition = new Vector2(m_CurCenter.x * m_scene.XRate, m_CurCenter.y * m_scene.ZRate);
            imageTrans.sizeDelta = new Vector2(m_CurRadius * 12.8f, m_CurRadius * 10);

        }
        // Update is called once per frame
        void Update()
        {
            if (startShrink)
            {
                if (m_durTime < m_shrinkTime)
                {
                    float t = m_durTime / m_shrinkTime;
                    m_CurCenter = Vector2.Lerp(m_SrcCenter, m_TargetCenter, t);
                    m_CurRadius = Mathf.Lerp(m_SrcRadius, m_TgtRadius, t);
                    m_durTime += Time.deltaTime;;
                }
                else
                {
                    startShrink = false;
                    m_CurCenter = m_TargetCenter;
                    m_CurRadius = m_TgtRadius;
                    m_SrcCenter = m_CurCenter;
                    m_SrcRadius = m_CurRadius;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PoisonNextStage);
                }
                Set();
            }
        }
    }
}
