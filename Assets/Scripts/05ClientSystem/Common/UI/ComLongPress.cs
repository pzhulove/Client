using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class ComLongPress : MonoBehaviour,IPointerDownHandler,IPointerUpHandler {

    public Action<Transform,object> pointDownCallBack;

    public Action<object> pointUpCallBack;

    private object args;

    public void SetArgs(object args)
    {
        this.args = args;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (pointDownCallBack != null)
            pointDownCallBack(this.transform, args);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (pointUpCallBack != null)
            pointUpCallBack(args);
    }
}
