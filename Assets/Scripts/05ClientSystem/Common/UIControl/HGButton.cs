using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    // Button that's meant to work with mouse or touch-based devices.
    [AddComponentMenu("UI/HGButton", 30)]
    public class HGButton : Button,IPointerDownHandler,IPointerUpHandler
    {
        [Serializable]
        public class HGButtonDownUpEvent : UnityEvent<bool> { }

        [SerializeField]
        private HGButtonDownUpEvent m_OnUpDown = new HGButtonDownUpEvent();

        public HGButtonDownUpEvent onUpDown
        {
            get { return m_OnUpDown; }
            set { m_OnUpDown = value; }
        }

        private void OnUpDown(bool bUp)
        {
            if (!IsActive() || !IsInteractable())
                return;
            
            m_OnUpDown.Invoke(bUp);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            
            OnUpDown(false);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            
            OnUpDown(true);
        }
    }
}
