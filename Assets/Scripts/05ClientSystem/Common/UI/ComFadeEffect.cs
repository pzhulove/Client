using UnityEngine;
using UnityEngine.Events;

namespace GameClient
{
    class ComFadeEffect : MonoBehaviour
    {
        public class FadeInEvent : UnityEvent { }
        public FadeInEvent OnFadeIn = new FadeInEvent();
        public AnimationCurve FadeInAlphaCurve;
        public float FadeInTime = 1.0f;

        public class FadeOutEvent : UnityEvent { }
        public FadeOutEvent OnFadeOut = new FadeOutEvent();
        public AnimationCurve FadeOutAlphaCurve;
        public float FadeOutTime = 1.0f;
        

        private CanvasRenderer[] canvasRenders;
        private bool m_fadeIn = false;
        private bool m_fadeOut = false;
        private float m_time = 0.0f;

        public void FadeIn()
        {
            m_fadeIn = true;
            m_fadeOut = false;
            m_time = 0.0f;
            _UpdateFade(0.0f);
        }

        public void FadeOut()
        {
            m_fadeIn = false;
            m_fadeOut = true;
            m_time = 0.0f;
            _UpdateFade(0.0f);
        }

        void Awake()
        {
            canvasRenders = gameObject.GetComponentsInChildren<CanvasRenderer>();
        }

        void Update()
        {
            _UpdateFade(Time.deltaTime);
        }

        void _UpdateFade(float time)
        {
            if (m_fadeIn && FadeInAlphaCurve != null)
            {
                m_time += time;

                float rate = Mathf.Clamp(m_time / FadeInTime, 0.0f, 1.0f);
                float alpha = Mathf.Clamp(FadeInAlphaCurve.Evaluate(rate), 0.0f, 1.0f);
                _SetUpAlpha(alpha);

                if (m_time > FadeInTime)
                {
//                     if (alpha != 1.0f)
//                     {
//                         Logger.LogFormat("fade in finished!!");
//                     }
                    m_fadeIn = false;
                    OnFadeIn.Invoke();
                }

            }
            else if (m_fadeOut && FadeOutAlphaCurve != null)
            {
                m_time += time;

                float rate = Mathf.Clamp(m_time / FadeOutTime, 0.0f, 1.0f);
                float alpha = Mathf.Clamp(FadeOutAlphaCurve.Evaluate(rate), 0.0f, 1.0f);
                _SetUpAlpha(alpha);

                if (m_time > FadeOutTime)
                {
                    m_fadeOut = false;
                    OnFadeOut.Invoke();
                }
                
            }
        }

        void _SetUpAlpha(float alpha)
        {
            for (int i = 0; i < canvasRenders.Length; ++i)
            {
                canvasRenders[i].SetAlpha(alpha);
            }
        }


    }
}
