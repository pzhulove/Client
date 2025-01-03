using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ClipScrollrect : ScrollRect
{
    public class ClipScrollrectEvent : UnityEvent<PointerEventData>
    {
        public ClipScrollrectEvent()
        {

        }
    }

    public ClipScrollrectEvent onEndDrag = new ClipScrollrectEvent();

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if(onEndDrag != null)
        {
            onEndDrag.Invoke(eventData);
        }
    }
}