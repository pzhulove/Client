using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class ComSelectScorllRect : ScrollRect,IPointerDownHandler,IPointerUpHandler
{
    public GameObject SelectedGameObject { get; set; }



    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if (SelectedGameObject)
            SelectedGameObject.CustomActive(true);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (SelectedGameObject)
            SelectedGameObject.CustomActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (SelectedGameObject)
            SelectedGameObject.CustomActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (SelectedGameObject)
            SelectedGameObject.CustomActive(true);
    }
}
