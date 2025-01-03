using System;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.UIElements;

public class ComDragPad : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerClickHandler
{
    public RectTransform dragTran;
    RectTransform parentTran;

    Vector3 offset;
    Vector3 dragOffset;
    bool canDrag = false;

    public delegate void Fun(ComDragPad pad);
    Fun callBack;

    void Start()
    {
        parentTran = transform.parent as RectTransform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag)
            return;

        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(parentTran, eventData.position, eventData.pressEventCamera, out worldPos);
        offset = transform.position - worldPos;
        if (dragTran != null)
            dragOffset = dragTran.position - transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag)
            return;

        Vector3 nowPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(parentTran, eventData.position, eventData.pressEventCamera, out nowPos);
        transform.position = nowPos + offset;
        if (dragTran != null)
            dragTran.position = transform.position + dragOffset;

        callBack?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canDrag)
            return;

        callBack?.Invoke(this);
    }

    public void SetDragTransform(RectTransform tran)
    {
        dragTran = tran;
        dragOffset = dragTran.position - transform.position;
    }

    public void SetCanDrag(bool flag)
    {
        canDrag = flag;
    }

    public void SetPosition(Vector3 position)
    {
        var localPos = Vector3.zero;
        if (dragTran != null)
        {
            dragTran.position = position;
            localPos = dragTran.localPosition;
            localPos.z = 0;
            dragTran.localPosition = localPos;
        }

        transform.position = position - dragOffset;
        localPos = transform.localPosition;
        localPos.z = 0;
        transform.localPosition = localPos;
    }
    
    public void SetPosition2(Vector3 position)
    {
        if (dragTran != null)
        {
            dragTran.localPosition = position;
            transform.position = dragTran.position;
            //transform.rectTransform().sizeDelta = dragTran.rectTransform().sizeDelta;
        }
    }

    public void SetLocalScale(Vector3 scale)
    {
        if (dragTran != null)
        {
            dragTran.localScale = scale;
            transform.localScale = scale;
        }
    }
    
    public void SetAlpha(float alpha)
    {
        if (dragTran != null)
        {
            var canvasGroup = dragTran.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = dragTran.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = alpha;
        }
    }
    
    public void SetInputSettingData(InputSettingItem item)
    {
        SetPosition2(item.position);
        SetLocalScale(item.scale);
        SetAlpha(item.alpha);
    }

    public void SetCallBack(Fun callBack)
    {
        this.callBack = callBack;
    }

}
