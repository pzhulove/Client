using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace GameClient
{
    public class OnDropEvent : UnityEvent<ComDrop, ComDrag> { }
    public class OnEnterEvent : UnityEvent<ComDrop, ComDrag> { }
    public class OnExitEvent : UnityEvent<ComDrop, ComDrag> { }

    public class ComDrop : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public string AcceptDragName = "Default";

        public OnDropEvent OnDropEvent = new OnDropEvent();
        public OnEnterEvent OnEnterEvent = new OnEnterEvent();
        public OnExitEvent OnExitEvent = new OnExitEvent();

        public void OnDrop(PointerEventData data)
        {
            GameObject originalObj = data.pointerDrag;
            if (originalObj == null)
            {
                return;
            }

            ComDrag comDrag = originalObj.GetComponent<ComDrag>();
            if (comDrag == null)
            {
                return;
            }

            if (AcceptDragName != comDrag.Name)
            {
                Logger.Log("drop name is not equal!! cannot drop!");
                return;
            }

            Logger.Log("On drop....");
            OnDropEvent.Invoke(this, comDrag);
        }

        public void OnPointerEnter(PointerEventData data)
        {
            GameObject originalObj = data.pointerDrag;
            if (originalObj == null)
            {
                return;
            }

            ComDrag comDrag = originalObj.GetComponent<ComDrag>();
            if (comDrag == null)
            {
                return;
            }

            if (AcceptDragName != comDrag.Name)
            {
                Logger.Log("drop name is not equal!! cannot drop!");
                return;
            }

            Logger.Log("On drop enter....");
            OnEnterEvent.Invoke(this, comDrag);
        }

        public void OnPointerExit(PointerEventData data)
        {
            GameObject originalObj = data.pointerDrag;
            if (originalObj == null)
            {
                return;
            }

            ComDrag comDrag = originalObj.GetComponent<ComDrag>();
            if (comDrag == null)
            {
                return;
            }

            if (AcceptDragName != comDrag.Name)
            {
                Logger.Log("drop name is not equal!! cannot drop!");
                return;
            }

            Logger.Log("On drop exit....");
            OnExitEvent.Invoke(this, comDrag);
        }
    }
}
