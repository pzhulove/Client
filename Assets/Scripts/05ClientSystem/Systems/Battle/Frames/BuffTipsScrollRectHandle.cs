using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuffTipsScrollRectHandle : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IPointerUpHandler
{
    public bool mBeginDragFlag = false;
    public bool mPointerDownFlag = false;
    public bool mPointerUpFlag = false;

    void OnEnable()
    {
        ResetFlag();
    }

    public void ResetFlag()
    {
        mBeginDragFlag = false;
        mPointerDownFlag = false;
        mPointerUpFlag = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        mBeginDragFlag = true;
        //Logger.LogError("开始拖拽");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        mPointerDownFlag = true;
        //Logger.LogError("点击");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mPointerUpFlag = true;
        //Logger.LogError("抬起");
    }
}
