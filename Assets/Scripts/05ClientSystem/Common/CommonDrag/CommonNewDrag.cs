using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameClient
{
    public class CommonNewDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {

        private bool _isDragging = false;                  //正在拖动的标志
        private GameObject _dragGameObject;                 //拖动的物体
        private GameObject _dragCanvasRoot;                 //拖动物体所在的Canvas界面

        private IsCanBeginDragAction _isCanBeginDragAction;
        private DragBeginAction _dragBeginAction;
        private DragEndAction _dragEndAction;

        public void SetIsCanBeginDragAction(IsCanBeginDragAction isCanBeginDragAction)
        {
            _isCanBeginDragAction = isCanBeginDragAction;
        }

        public void SetDragBeginAction(DragBeginAction dragBegin)
        {
            _dragBeginAction = dragBegin;
        }

        public void SetDragEndAction(DragEndAction dragEnd)
        {
            _dragEndAction = dragEnd;
        }

        public void ResetDragAction()
        {
            _isCanBeginDragAction = null;
            _dragBeginAction = null;
            _dragEndAction = null;
        }

        //开始拖动
        public void OnBeginDrag(PointerEventData pointerEventData)
        {
            if (pointerEventData == null)
                return;

            if (CommonNewDragUtility.IsAlreadyOwnerFirstDraggingPointerId() == true)
                return;

            _isDragging = false;
            if (_isCanBeginDragAction != null)
            {
                var isCanBeginDrag = _isCanBeginDragAction(pointerEventData);
                if (isCanBeginDrag == false)
                    return;
            }
            
            //创建DragGameObject 和 获得DragCanvasRoot
            if (_dragBeginAction != null)
                _dragBeginAction(pointerEventData);

            SetDraggedPosition(pointerEventData);
            //开始拖拽
            _isDragging = true;
            //设置第一个拖动点
            CommonNewDragUtility.SetFirstDraggingPointerId(pointerEventData.pointerId);
        }

        //拖动中
        public void OnDrag(PointerEventData pointerEventData)
        {
            if (pointerEventData == null)
                return;

            if (CommonNewDragUtility.IsPointerIdIsFirstDraggingPointerId(pointerEventData.pointerId) == false)
                return;

            if (_isDragging == false)
                return;

            SetDraggedPosition(pointerEventData);
        }

        //结束拖动
        public void OnEndDrag(PointerEventData pointerEventData)
        {
            if (pointerEventData == null)
                return;

            if (CommonNewDragUtility.IsPointerIdIsFirstDraggingPointerId(pointerEventData.pointerId) == false)
                return;

            _isDragging = false;
            CommonNewDragUtility.ResetFirstDragPointerId();

            if (_dragEndAction != null)
                _dragEndAction(pointerEventData);
        }

        public void OnForceEndDrag()
        {
            _isDragging = false;
            CommonNewDragUtility.ResetFirstDragPointerId();

            if (_dragEndAction != null)
                _dragEndAction(null);
        }

        //设置位置
        private void SetDraggedPosition(PointerEventData pointerEventData)
        {
            if (_dragCanvasRoot == null)
                return;

            if (_dragGameObject == null)
                return;

            var dragGameObjectRtf = _dragGameObject.GetComponent<RectTransform>();
            Vector2 globalMousePosBack;

            //屏幕坐标转化为rectTransform下的局部坐标
            var rtf = _dragCanvasRoot.transform as RectTransform;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rtf,
                    pointerEventData.position,
                    pointerEventData.pressEventCamera,
                    out globalMousePosBack) == true)
            {
                dragGameObjectRtf.anchoredPosition = globalMousePosBack;
            }
        }

        public void SetDragGameObject(GameObject dragGameObject)
        {
            _dragGameObject = dragGameObject;
        }

        public GameObject GetDragGameObject()
        {
            return _dragGameObject;
        }

        public void SetDragCanvasRoot(GameObject dragCanvasRoot)
        {
            _dragCanvasRoot = dragCanvasRoot;
        }
        
        public bool GetDraggingState()
        {
            return _isDragging;
        }
    }
}
