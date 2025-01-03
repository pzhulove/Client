using UnityEngine;

public class ComBaseCharactorBar : MonoBehaviour
{
    public CanvasGroup mCanvasGroup;

    protected void _SetVisible(bool isShow)
    {
        //if (null != mCanvasGroup)
        //{
        //    mCanvasGroup.alpha = isShow ? 1.0f : 0.0f;
        //}
        //else
        //{
            gameObject.CustomActive(isShow);

            //UnityEngine.Debug.LogErrorFormat("{0} CanvasGroup is missing", gameObject.name);
        //}
    }

#if UNITY_EDITOR
    public void Awake()
    {
        //if (null == mCanvasGroup)
        //{
        //    UnityEngine.Debug.LogErrorFormat("{0} CanvasGroup is missing", gameObject.name);
        //}
    }
#endif

}

