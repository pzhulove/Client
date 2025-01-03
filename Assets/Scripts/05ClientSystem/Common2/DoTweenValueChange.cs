using UnityEngine;
using System.Collections;
using DG.Tweening;

class DoTweenValueChange : MonoBehaviour
{
    float fValue = 0.0f;
    public delegate void OnValueChanged(float value);
    public OnValueChanged onValueChanged;
    public delegate void OnAnimationEnd();
    public OnAnimationEnd onAnimationEnd;
    public float fExeTime = 10.0f;
    public int iLoopTimes = 1;
    bool bStart = false;
    Tween current = null;

    public void Stop()
    {
        bStart = false;
        current.Kill();
    }

    public void StartAnimation()
    {
        if(bStart)
        {
            return;
        }
        bStart = true;
        fValue = 0.0f;
        if(onValueChanged != null)
        {
            onValueChanged.Invoke(fValue);
        }

        DoLoop(iLoopTimes);
    }

    public void DoLoop(int iTimes = 3)
    {
        if(iTimes > 0)
        {
            --iTimes;
        }
        else
        {
            if (onAnimationEnd != null)
            {
                onAnimationEnd.Invoke();
            }
            bStart = false;
            return;
        }
        // 创建一个 Tweener 对象， 另 number的值在 5 秒内变化到 100
        current = DOTween.To((x) => fValue = x, 0.0f, 1.0f, fExeTime * 1.0f / iLoopTimes);
        // 给执行 t 变化时，每帧回调一次 UpdateTween 方法
        current.OnUpdate(() =>
        {
            if (onValueChanged != null)
            {
                onValueChanged.Invoke(fValue);
            }
        });
        current.OnComplete(() =>
        {
            DoLoop(iTimes);
        });
    }
}
