using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameClient
{
    public class ComButtonEx : Button
    {
        public class MouseEvent : UnityEvent<PointerEventData> { }

        public bool penetrable = true;
        public MouseEvent onMouseDown = new MouseEvent();
        public MouseEvent onMouseUp = new MouseEvent();
        public MouseEvent onMouseClick = new MouseEvent();

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            onMouseDown.Invoke(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            onMouseUp.Invoke(eventData);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            onMouseClick.Invoke(eventData);

            if (penetrable)
            {
                List<IPointerClickHandler> handlers = GamePool.ListPool<IPointerClickHandler>.Get();

                List<RaycastResult> results = GamePool.ListPool<RaycastResult>.Get();
                EventSystem.current.RaycastAll(eventData, results);
                for (int i = 0; i < results.Count; ++i)
                {
                    RaycastResult result = results[i];
                    if (result.isValid && result.gameObject == gameObject)
                    {
                        for (int j = i + 1; j < results.Count; ++j)
                        {
                            if (results[j].isValid)
                            {
                                Graphic graphic = results[j].gameObject.GetComponent<Graphic>();
                                if (graphic != null && graphic.raycastTarget == true)
                                {
                                    GameObject objTemp = results[j].gameObject;
                                    do
                                    {
                                        objTemp.GetComponents<IPointerClickHandler>(handlers);
                                        if (objTemp.transform.parent != null)
                                        {
                                            objTemp = objTemp.transform.parent.gameObject;
                                        }
                                        else
                                        {
                                            objTemp = null;
                                        }
                                    } while (handlers.Count <= 0 && objTemp != null);

                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
                GamePool.ListPool<RaycastResult>.Release(results);

                for (int i = 0; i < handlers.Count; ++i)
                {
                    handlers[i].OnPointerClick(eventData);
                }
                GamePool.ListPool<IPointerClickHandler>.Release(handlers);
            }
            
        }

    }
}
