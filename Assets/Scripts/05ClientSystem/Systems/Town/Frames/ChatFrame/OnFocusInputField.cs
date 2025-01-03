using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameClient
{
    class OnFocusInputField : InputField
    {
        public delegate void OnClick();
        public OnClick onClick;
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (onClick != null)
            {
                onClick.Invoke();
            }
        }
    }
}