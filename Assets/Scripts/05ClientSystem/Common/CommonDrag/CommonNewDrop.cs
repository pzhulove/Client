using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameClient
{
    public class CommonNewDrop : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private IsPointerItemCanDropDownAction _isPointerItemCanDropDownAction;
        private OnDropDownAction _onDropDownAction;
        private OnPointerEnterAction _onPointerEnterAction;
        private OnPointerExitAction _onPointerExitAction;

        public void SetIsPointerItemCanDropDownAction(IsPointerItemCanDropDownAction isPointerItemCanDropDownAction)
        {
            _isPointerItemCanDropDownAction = isPointerItemCanDropDownAction;
        }

        public void SetOnDropDownAction(OnDropDownAction onDropDownAction)
        {
            _onDropDownAction = onDropDownAction;
        }

        public void SetOnPointerEnterAction(OnPointerEnterAction onPointerEnterAction)
        {
            _onPointerEnterAction = onPointerEnterAction;
        }

        public void SetOnPointerExitAction(OnPointerExitAction onPointerExitAction)
        {
            _onPointerExitAction = onPointerExitAction;
        }

        public void ResetDropAction()
        {
            _isPointerItemCanDropDownAction = null;
            _onDropDownAction = null;
            _onPointerExitAction = null;
            _onPointerEnterAction = null;
        }

        //放置
        public void OnDrop(PointerEventData pointerEventData)
        {
            if (pointerEventData == null)
                return;

            if (CommonNewDragUtility.IsPointerIdIsFirstDraggingPointerId(pointerEventData.pointerId) == false)
                return;

            var isPointerItemCanDrop = false;
            if (_isPointerItemCanDropDownAction != null)
                isPointerItemCanDrop = _isPointerItemCanDropDownAction(pointerEventData);

            if (isPointerItemCanDrop == false)
                return;

            if (_onDropDownAction != null)
                _onDropDownAction(pointerEventData);
        }


        //进入
        public void OnPointerEnter(PointerEventData pointerEventData)
        {
            if (pointerEventData == null)
                return;

            if (CommonNewDragUtility.IsPointerIdIsFirstDraggingPointerId(pointerEventData.pointerId) == false)
                return;

            var isPointerItemCanDrop = false;
            if (_isPointerItemCanDropDownAction != null)
                isPointerItemCanDrop = _isPointerItemCanDropDownAction(pointerEventData);

            if (isPointerItemCanDrop == false)
                return;

            if (_onPointerEnterAction != null)
                _onPointerEnterAction(pointerEventData);
        }

        //退出
        public void OnPointerExit(PointerEventData pointerEventData)
        {
            if (pointerEventData == null)
                return;

            if (CommonNewDragUtility.IsPointerIdIsFirstDraggingPointerId(pointerEventData.pointerId) == false)
                return;

            if (_onPointerExitAction != null)
                _onPointerExitAction(pointerEventData);
        }
    }
}
