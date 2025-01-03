using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GameClient
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ComDoFade : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        public float startTime = 0.0f;
        public float duration = 1.0f;
        public AnimationCurve curve = new AnimationCurve();
        float start = -1.0f;
        enum FadeStatus
        {
            FS_START = 0,
            FS_CLOSED,
        }
        FadeStatus eFadeStatus = FadeStatus.FS_CLOSED;

        void Awake()
        {
            eFadeStatus = FadeStatus.FS_CLOSED;
            start = -1.0f;
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void SetAlpha(float value)
        {
            if(null != canvasGroup)
            {
                canvasGroup.alpha = value;
            }
        }

        public void Play()
        {
            if(null != canvasGroup)
            {
                eFadeStatus = FadeStatus.FS_START;
                canvasGroup.alpha = curve.Evaluate(startTime);
                start = Time.time;
            }
        }

        public void Update()
        {
            if(eFadeStatus == FadeStatus.FS_START)
            {
                if (null != canvasGroup)
                {
                    float tt = Time.time - start + startTime;
                    float ev = Mathf.Clamp(tt, startTime, startTime + duration);
                    canvasGroup.alpha = curve.Evaluate(ev);
                    if(tt >= duration)
                    {
                        eFadeStatus = FadeStatus.FS_CLOSED;
                        start = -1.0f;
                    }
                }
            }
        }
    }
}