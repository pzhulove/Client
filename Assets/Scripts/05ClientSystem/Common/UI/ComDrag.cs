using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace GameClient
{
    public class OnBeginDragEvent : UnityEvent<ComDrag> { }
    public class OnEndDragEvent : UnityEvent<ComDrag> { }

    public class ComDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public GameObject DragObject = null;
        public string Name = "Default";

        public OnBeginDragEvent OnBeginDragEvent = new OnBeginDragEvent();
        public OnEndDragEvent OnEndDragEvent = new OnEndDragEvent();

        private RectTransform m_dragRect;
        private RectTransform m_sourceRect;

        public void OnBeginDrag(PointerEventData eventData)
        {
            Logger.Log("OnStart call......");
            OnBeginDragEvent.Invoke(this);

            if (DragObject == null)
            {
                Logger.LogError("ComDrag DragObject is null!");
                return;
            }

            if (string.IsNullOrEmpty(Name))
            {
                Logger.LogError("ComDrag Name is null!");
                return;
            }

            var canvas = FindInParents<Canvas>(gameObject);
            if (canvas == null)
            {
                Logger.LogError("ComDrag canvas is null!");
                return;
            }

            DragObject.transform.SetParent(canvas.transform, false);

            var groupback = DragObject.GetComponent<CanvasGroup>();
            if (groupback == null)
            {
                groupback = DragObject.AddComponent<CanvasGroup>();
            }
            groupback.blocksRaycasts = false;

            m_sourceRect = canvas.transform as RectTransform;
            m_dragRect = DragObject.transform as RectTransform;
            _UpdateDragObjectPos(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Logger.Log("OnDrag call......");
            if (m_sourceRect != null && m_dragRect != null)
            {
                _UpdateDragObjectPos(eventData);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Logger.Log("OnEnd call......");
            OnEndDragEvent.Invoke(this);

            m_dragRect = null;
            m_sourceRect = null;
    }

        static public T FindInParents<T>(GameObject go) where T : Component
        {
            if (go == null)
            {
                return null;
            }

            var comp = go.GetComponent<T>();
            if (comp != null)
            {
                return comp;
            }

            var t = go.transform.parent;
            while (t != null && comp == null)
            {
                comp = t.gameObject.GetComponent<T>();
                t = t.parent;
            }

            return comp;
        }

        void _UpdateDragObjectPos(PointerEventData eventData)
        {
            Vector3 pos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_sourceRect, eventData.position, eventData.pressEventCamera, out pos))
            {
                Logger.Log("draging......");
                m_dragRect.position = pos;
            }
        }
    }
}
