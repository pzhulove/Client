using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class DropGuideCom : MonoBehaviour, IDropHandler
{
    public Action<PointerEventData> eventOnDrop;

	public void OnDrop(PointerEventData eventData)
	{
		 if(eventOnDrop != null)
         {
             eventOnDrop.Invoke(eventData);
         }
	}
}
