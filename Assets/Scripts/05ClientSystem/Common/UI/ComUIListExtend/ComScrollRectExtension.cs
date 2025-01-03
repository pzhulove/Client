using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

namespace GameClient
{
    public class ComScrollRectExtension : ScrollRect
    {
        public ScrollRectDragEndEvent onDragEnd = new ScrollRectDragEndEvent();

        [Serializable]
        public class ScrollRectDragEndEvent : UnityEvent
        {
        }

        public override void OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (onDragEnd != null)
            {
                onDragEnd.Invoke();
            }

            base.OnEndDrag(eventData);
        }
    }
}