using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GameClient
{
    class ComDoTweenHeight : MonoBehaviour
    {
        RectTransform m_rect;
        float m_startHeight = 0.0f;
        RectTransform m_imageRect;
        float m_startRot = 0.0f;

        public float TargetHeight = 100.0f;
        public AnimationCurve ExpandCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public float ExpandTime = 0.3f;
        public AnimationCurve CollapseCurve = new AnimationCurve(new Keyframe(1, 1), new Keyframe(0, 0));
        public float CollapseTime = 0.3f;
        public Image Arrow;
        public float TargetRot = 0.0f;
        void Start()
        {
            m_rect = gameObject.GetComponent<RectTransform>();
            m_startHeight = m_rect.sizeDelta.y;
            if (Arrow != null)
            {
                m_imageRect = Arrow.GetComponent<RectTransform>();
                m_startRot = m_imageRect.localEulerAngles.z;
            }
        }

        public void Expand()
        {
            Tweener doTweener = DOTween.To(
                () => { return m_rect.sizeDelta.y; }, 
                data => { m_rect.sizeDelta = new Vector2(m_rect.sizeDelta.x, data); },
                TargetHeight,
                ExpandTime
                );
            doTweener.SetEase(ExpandCurve);

            if (m_imageRect != null)
            {
                m_imageRect.DORotate(new Vector3(0.0f, 0.0f, TargetRot), ExpandTime);
            }
        }

        public void Collapse()
        {
            Tweener doTweener = DOTween.To(
                () => { return m_rect.sizeDelta.y; },
                data => { m_rect.sizeDelta = new Vector2(m_rect.sizeDelta.x, data); },
                m_startHeight,
                CollapseTime
                );
            doTweener.SetEase(CollapseCurve);

            if (m_imageRect != null)
            {
                m_imageRect.DORotate(new Vector3(0.0f, 0.0f, m_startRot), CollapseTime);
            }
        }
    }
}
