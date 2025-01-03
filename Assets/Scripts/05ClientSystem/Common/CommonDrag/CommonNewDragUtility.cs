using System;
using UnityEngine.EventSystems;

namespace GameClient
{

    //拖动
    //是否可以拖拽
    public delegate bool IsCanBeginDragAction(PointerEventData pointerEventData);
    //拖拽开始
    public delegate void DragBeginAction(PointerEventData pointerEventData);
    //拖拽结束
    public delegate void DragEndAction(PointerEventData pointerEventData);

    //放置
    //拖动的物体是否可以放下
    public delegate bool IsPointerItemCanDropDownAction(PointerEventData pointerEventData);
    //在当前区域中放下
    public delegate void OnDropDownAction(PointerEventData pointerEventData);
    //进入当前区域
    public delegate void OnPointerEnterAction(PointerEventData pointerEventData);
    //退出当前区域
    public delegate void OnPointerExitAction(PointerEventData pointerEventData);

    //拖拽助手
    public static class CommonNewDragUtility
    {
        //第一个拖动点（用于单点触控)
        public static int FirstDragPointerId = Int32.MinValue;

        //重置第一个拖动点
        public static void ResetFirstDragPointerId()
        {
            FirstDragPointerId = Int32.MinValue;
        }

        //已经设置第一个拖拽点
        public static bool IsAlreadyOwnerFirstDraggingPointerId()
        {
            if (FirstDragPointerId == Int32.MinValue)
                return false;

            return true;
        }

        //设置第一个拓东店
        public static void SetFirstDraggingPointerId(int pointerId)
        {
            FirstDragPointerId = pointerId;
        }

        //当前拖动点是否为第一个拖动点
        public static bool IsPointerIdIsFirstDraggingPointerId(int pointerId)
        {
            //没有设置，则不是第一个点
            if (FirstDragPointerId == Int32.MinValue)
                return false;

            if (pointerId != FirstDragPointerId)
                return false;

            return true;
        }
    }
}
