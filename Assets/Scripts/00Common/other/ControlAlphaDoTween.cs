using UnityEngine;
using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class ControlAlphaDoTween : AnimationController
{
	CanvasGroup canvasGroup;

    public override void Initialize()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = from;
        }
	}

    protected override void OnStart()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = from;
        }
    }


    protected override void DoPlay()
    {
        if(canvasGroup != null)
        {
            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, to, alphaTime);
            canvasGroup.DOFade(to, alphaTime);
        }
	}
}