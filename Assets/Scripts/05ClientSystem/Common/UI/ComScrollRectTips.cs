using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ComScrollRectTips : MonoBehaviour 
{
    public Vector2 upDownStep = new Vector2(0.1f, 0.1f);
    public Vector2 leftRightStep = new Vector2(0.1f, 0.1f);

    public Button up;
    public Button down;
    public Button left;
    public Button right;


    public ScrollRect scroll;

    void Awake()
    {
        if (null != up) up.onClick.AddListener(_onMoveUp);
        if (null != down) down.onClick.AddListener(_onMoveDown);
        if (null != left) left.onClick.AddListener(_onMoveLeft);
        if (null != right) right.onClick.AddListener(_onMoveRight);
    }

    void OnDestroy()
    {
        if (null != up) up.onClick.RemoveListener(_onMoveUp);
        if (null != down) down.onClick.RemoveListener(_onMoveDown);
        if (null != left) left.onClick.RemoveListener(_onMoveLeft);
        if (null != right) right.onClick.RemoveListener(_onMoveRight);
    }

    private void _onMoveUp() 
    { 
        var pos = scroll.normalizedPosition;
        pos.y += upDownStep.x; 
        pos.y = Mathf.Clamp01(pos.y);
        scroll.normalizedPosition = pos;
    }

    private void _onMoveDown() 
    { 
        var pos = scroll.normalizedPosition;
        pos.y -= upDownStep.y; 
        pos.y = Mathf.Clamp01(pos.y);
        scroll.normalizedPosition = pos;
    }

    private void _onMoveLeft() 
    { 
        var pos = scroll.normalizedPosition;
        pos.x -= leftRightStep.x; 
        pos.x = Mathf.Clamp01(pos.x);
        scroll.normalizedPosition = pos;
    }

    private void _onMoveRight() 
    { 
        var pos = scroll.normalizedPosition;
        pos.x += leftRightStep.x; 
        pos.x = Mathf.Clamp01(pos.x);
        scroll.normalizedPosition = pos;
    }

    public void onPositionUpdate(Vector2 v)
    {
        //Logger.LogErrorFormat("{0}", scroll.normalizedPosition);
    }

}
