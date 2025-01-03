using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameClient
{
    public class LongPressOrClickEventTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public float durationThreshold = 1.0f;
        public UnityEvent onLongPressStart = new UnityEvent();
        public UnityEvent onLongPressEnd = new UnityEvent();
        public UnityEvent onClick = new UnityEvent();
        private bool isPointerDown = false;
        private bool longPressTriggered = false;
        private float timePressStarted;

        private void Awake()
        {
            onLongPressStart = new UnityEvent();
            onLongPressEnd = new UnityEvent();
            onClick = new UnityEvent();
        }

        private void OnDestroy()
        {
            onLongPressStart = null;
            onLongPressEnd = null;
            onClick = null;            
        }

        private void Update()
        {
            if (isPointerDown && !longPressTriggered)
            {
                if (Time.time - timePressStarted > durationThreshold)
                {
                    longPressTriggered = true;
                    if(onLongPressStart != null)
                    {
                        onLongPressStart.Invoke();
                    }                    
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            timePressStarted = Time.time;
            isPointerDown = true;
            longPressTriggered = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
            if(longPressTriggered)
            {
                if(onLongPressEnd != null)
                {
                    onLongPressEnd.Invoke();
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
      
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerDown = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!longPressTriggered)
            {
                if(onClick != null)
                {
                    onClick.Invoke();
                }                
            }            
        }
        
        public void AddListener(UnityAction pressStart,UnityAction pressEnd,UnityAction click)
        {
            if(onLongPressStart != null && pressStart != null)
            {
                onLongPressStart.AddListener(pressStart);
            }

            if(onLongPressEnd != null && pressEnd != null)
            {
                onLongPressEnd.AddListener(pressEnd);
            }

            if(onClick != null && click != null)
            {
                onClick.AddListener(click);
            }
        }

        public void SetListener(UnityAction pressStart, UnityAction pressEnd, UnityAction click)
        {
            if (onLongPressStart != null && pressStart != null)
            {
                onLongPressStart.RemoveAllListeners();
                onLongPressStart.AddListener(pressStart);
            }

            if (onLongPressEnd != null && pressEnd != null)
            {
                onLongPressEnd.RemoveAllListeners();
                onLongPressEnd.AddListener(pressEnd);
            }

            if (onClick != null && click != null)
            {
                onClick.RemoveAllListeners();
                onClick.AddListener(click);
            }
        }

        public void RemoveAllListener()
        {
            if (onLongPressStart != null)
            {
                onLongPressStart.RemoveAllListeners();
            }

            if (onLongPressEnd != null)
            {
                onLongPressEnd.RemoveAllListeners();
            }

            if (onClick != null)
            {
                onClick.RemoveAllListeners();
            }
        }
    }
}