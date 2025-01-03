using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CloseMedicinebar : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{

    private bool isOpen = false; 
    public void Update()
    {
        if (isOpen)
        {
            ComDrugTipsBar.instance.SetDrugColumnStat();
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        isOpen = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isOpen = false;
    }

  
}
