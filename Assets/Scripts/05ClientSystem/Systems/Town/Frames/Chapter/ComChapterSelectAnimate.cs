using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ComChapterSelectAnimate : MonoBehaviour 
{
    public RectTransform positionTarget;
    public RectTransform animateTarget;

    public CanvasGroup[]  hiddenRoots;

    public float animateDelayTime = 1.0f;
    public float animateTime = 1.0f;
    public float animateScale = 2.0f;

    public AnimationCurve animateScaleIn;
    public AnimationCurve animateScaleOut;
    public AnimationCurve animateMoveIn;
    public AnimationCurve animateMoveOut;

    public IEnumerator NormalAnimate(Vector3 targetPos)
    {
        yield return Yielders.GetWaitForSeconds(animateDelayTime);

        if (hiddenRoots != null)
        {
            for (int i = 0; i < hiddenRoots.Length; i++)
            {
                if (hiddenRoots[i] != null)
                {
                    hiddenRoots[i].alpha = 0.0f;
                }

                yield return null;
                yield return null;
                yield return null;
            }
        }

        if (null != animateTarget)
        {
            DOTween.To(
                    ()=>{return animateTarget.localScale;},
                    (v)=>{animateTarget.localScale = v;},
                    animateScale * Vector3.one,
                    animateTime)
                .SetEase(animateScaleIn);

                ;

            //DOTween.To(
            //        ()=>{ return animateTarget.localPosition; },
            //        (v)=>{ animateTarget.localPosition = v; },
            //        _convert2Position(targetPos),
            //        animateTime)
            //    .SetEase(animateMoveIn);

            _updatePovit(targetPos);
        }


        yield return Yielders.GetWaitForSeconds(animateTime);
    }

    //直接展示结果，不显示过程的Action
    public void NormalAnimateWithAction(RectTransform targetPos)
    {
        //隐藏
        if (hiddenRoots != null)
        {
            for (int i = 0; i < hiddenRoots.Length; i++)
            {
                if (hiddenRoots[i] != null)
                    hiddenRoots[i].alpha = 0.0f;
            }
        }

        //大小和位置的缩放
        if (null != animateTarget)
        {
            animateTarget.localScale = animateScale * Vector3.one;
            //animateTarget.localPosition = _convert2Position(targetPos);
        }

        //Pivot调整
        _updatePovit(targetPos.localPosition);
    }

    private void _updatePovit(Vector3 targetPos)
    {
        Vector3 pos = targetPos;
        Vector2 np = new Vector2(pos.x / _getWidth() + 0.5f, pos.y / _getHeight() + 0.5f);

        animateTarget.pivot = np;
    }

    private float _getWidth()
    {
        return 1920.0f;
    }

    private float _getHeight()
    {
        return 1080.0f;
    }

    private Vector3 _convert2Position(RectTransform targetPos)
    {
        return positionTarget.localPosition;
    }

    public IEnumerator RevertAnimate()
    {
        if (animateTarget != null)
        {
            animateTarget.pivot = 0.5f * Vector2.one;

            DOTween.To(
                    () => { return animateTarget.localScale; },
                    (v) => { animateTarget.localScale = v; },
                    Vector3.one,
                    animateTime).
                SetEase(animateScaleOut);

            //DOTween.To(
            //        () => { return animateTarget.localPosition; },
            //        (v) => { animateTarget.localPosition = v; },
            //        Vector3.zero,
            //        animateTime).
            //    SetEase(animateMoveOut);

            yield return Yielders.GetWaitForSeconds(animateTime);

            if (hiddenRoots != null)
            {
                for (int i = hiddenRoots.Length - 1; i >= 0; --i)
                {
                    if (hiddenRoots[i] != null)
                        hiddenRoots[i].alpha = 1.0f;

                    yield return null;
                    yield return null;
                    yield return null;
                }
            }
        }
    }
}
