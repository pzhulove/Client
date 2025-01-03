using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour
{
    UnityEngine.Events.UnityAction callback;

    public float delayTime = 0.0f;
    public float from = 1.0f;
    public float to = 1.0f;
    public float alphaTime = 1.0f;

    // Use this for initialization
    public virtual void Initialize()
    {
        callback = null;
    }

    protected virtual void OnStart()
    {

    }
    protected virtual void OnDestroy()
    {
        callback = null;
    }

    public virtual float GetTotalRunTime()
    {
        return delayTime + alphaTime;
    }

    public void DoPlay(UnityEngine.Events.UnityAction callback)
    {
        OnStart();
        this.callback = callback;
        Invoke("DoPlay", delayTime);
        Invoke("OnCallback", delayTime + alphaTime);
    }

    protected virtual void DoPlay()
    {
        Invoke("DoPlay", delayTime);
    }

    void OnCallback()
    {
        if(callback != null)
        {
            callback.Invoke();
        }
    }
}