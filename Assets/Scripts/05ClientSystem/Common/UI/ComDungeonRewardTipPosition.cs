using UnityEngine;
using System.Collections;

[ExecuteAlways]
public class ComDungeonRewardTipPosition : MonoBehaviour {

    public RectTransform mTransform;


    public void SetPosition(RectTransform trans)
    {
        if (mTransform != trans)
        {
            mTransform = trans;

            _updatePostion();
        }
    }

    public void _updatePostion()
    {
        var rect = GetComponent<RectTransform>();

        var offsetMin = rect.offsetMin;
        var offsetMax = rect.offsetMax;

        rect.offsetMin = new Vector2(mTransform.offsetMin.x, offsetMin.y);
        rect.offsetMax = new Vector2(mTransform.offsetMax.x, offsetMax.y);
    }


#if UNITY_EDITOR
    void Update()
    {
        _updatePostion();
    }
#endif
}

