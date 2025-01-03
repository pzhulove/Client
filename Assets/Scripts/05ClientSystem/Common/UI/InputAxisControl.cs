using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace GameClient.UI
{
    [AddComponentMenu("UI/InputAxisControl",40)]
    [RequireComponent(typeof(RectTransform))]
    public class InputAxisControl : Selectable, IBeginDragHandler, IDragHandler, IInitializePotentialDragHandler
    {
        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
            Logger.Log("OnInitializePotentialDrag\n");
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            Logger.Log("OnBeginDrag" + eventData.position.ToString() + "\n");
        }
        public void OnDrag(PointerEventData eventData)
        {
            Logger.Log("OnInitializePotentialDrag" + eventData.position.ToString() + "\n");
        }
    }
}
