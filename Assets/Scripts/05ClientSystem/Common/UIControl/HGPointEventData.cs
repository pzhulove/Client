using System;
using System.Collections.Generic;

namespace UnityEngine.EventSystems
{
    //
    // ժҪ:
    //     ///
    //     Event payload associated with pointer (mouse / touch) events.
    //     ///
    public class CustomPointerEventData : PointerEventData
    {
        public int customData = 0;
        public CustomPointerEventData(EventSystems.EventSystem ev):base(ev)
        {
            
        }
        public void CopyFrom(PointerEventData src)
        {
            this.eligibleForClick = src.eligibleForClick;

            pointerId = src.pointerId;
            position = src.position; // Current position of the mouse or touch event
            delta = src.delta; // Delta since last update
            pressPosition = src.pressPosition; // Delta since the event started being tracked
            clickTime = src.clickTime; // The last time a click event was sent out (used for double-clicks)
            clickCount = src.clickCount; // Number of clicks in a row. 2 for a double-click for example.

            scrollDelta = src.scrollDelta;
            useDragThreshold = src.useDragThreshold;
            dragging = src.dragging;
            button = src.button;
        }
    }
}