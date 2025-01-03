using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class DragGuideCom : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Action<PointerEventData> eventOnBeginDrag;
    public Action<PointerEventData> eventOnDrag;
    public Action<PointerEventData> eventOnEndDrag;

    public void OnBeginDrag(PointerEventData eventData)
	{
		 if(eventOnBeginDrag != null)
         {
             eventOnBeginDrag.Invoke(eventData);
         }
	}

	public void OnDrag(PointerEventData eventData)
	{
		 if(eventOnDrag != null)
         {
             eventOnDrag.Invoke(eventData);
         }
	}

	public void OnEndDrag(PointerEventData eventData)
	{
         if(eventOnEndDrag != null)
         {
             eventOnEndDrag.Invoke(eventData);
         }
    }
}
