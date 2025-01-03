namespace Scripts.UI
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Serialization;
    using GameClient;
    //using UnityEngine.EventSystems;


    [RequireComponent(typeof(ComScrollRectExtension))]
    public class ComUIListScriptExtension : ComUIListScript//, IEndDragHandler
    {
        public delegate void OnItemLocalPosAllInRectViewDelegate(ComUIListElementScript item);
        [HideInInspector]
        public OnItemLocalPosAllInRectViewDelegate OnItemLocalPosAllInRectView;

        protected ComScrollRectExtension m_scrollRectExtend;

        protected virtual void OnEnable()
        {
            if (this.m_scrollRectExtend == null)
            {
                this.m_scrollRectExtend = base.GetComponentInChildren<ComScrollRectExtension>(base.gameObject);
            }
            if (this.m_scrollRectExtend)
            {
                this.m_scrollRectExtend.onDragEnd.AddListener(_OnScrollRectDragEnd);
            }
        }

        protected virtual void OnDisable()
        {
            if (this.m_scrollRectExtend)
            {
                this.m_scrollRectExtend.onDragEnd.RemoveListener(_OnScrollRectDragEnd);
            }
        }

        private void _OnScrollRectDragEnd()
        {
            if (OnItemLocalPosAllInRectView != null)
            {
                if (m_scrollRect != null && m_content != null)
                {
                    RectTransform ScrollRect = m_scrollRect.GetComponent<RectTransform>();
                    RectTransform ContentRect = m_content.GetComponent<RectTransform>();

                    if (ScrollRect != null && ContentRect != null)
                    {
                        switch (this.m_listType)
                        {
                            case enUIListType.Vertical:
                            case enUIListType.VerticalGrid:
                            case enUIListType.Horizontal:
                            case enUIListType.HorizontalGrid:
                                {
                                    InvokeOnItemLocalPosAllInRectView();
                                    break;
                                }
                        }
                    }
                }
            }
        }

        private void InvokeOnItemLocalPosAllInRectView()
        {
            //item 全部位于 可是视区内 
            if (OnItemLocalPosAllInRectView != null)
            {
                for (int i = 0; i < this.m_elementScripts.Count; i++)
                {
                    var element = this.m_elementScripts[i];
                    if (element == null) continue;
                    if (IsRectTotalInScrollArea(ref element.m_rect))
                    {
                        OnItemLocalPosAllInRectView(element);
                    }
                }
            }
        }

        public void UnInitialize()
        {
            base.UnInitialize();

            if (this.m_scrollRectExtend)
            {
                this.m_scrollRectExtend.onDragEnd.RemoveAllListeners();
                m_scrollRectExtend = null;
            }
            OnItemLocalPosAllInRectView = null;
        }

        public bool IsElementTotalInScrollArea(int index = 0)
        {
            if ((index < 0) || (index >= this.m_elementAmount))
            {
                return false;
            }
            stRect rect = !this.m_useOptimized ? this.m_elementScripts[index].m_rect : this.m_elementsRect[index];
            return this.IsRectTotalInScrollArea(ref rect, index);
        }

        protected bool IsRectTotalInScrollArea(ref stRect rect, int index = 0)
        {
            Vector2 zero = Vector2.zero;

            //Debug.LogError("\n ITEM INDEX : " + index.ToString());
            //Debug.LogError("===================================================================================================\n");
            //Debug.LogError("this.m_contentRectTransform.anchoredPosition.x = " + this.m_contentRectTransform.anchoredPosition.x);
            //Debug.LogError("this.m_contentRectTransform.anchoredPosition.y = " + this.m_contentRectTransform.anchoredPosition.y);

            //Debug.LogError("item rect m_left: " + rect.m_left);
            //Debug.LogError("item rect m_top: " + rect.m_top);
            //Debug.LogError("item rect m_right: " + rect.m_right);
            //Debug.LogError("item rect m_bottom: " + rect.m_bottom);

            //Debug.LogError("item rect m_height: " + rect.m_height);
            //Debug.LogError("item rect m_width: " + rect.m_width);

            //Debug.LogError("this.m_scrollAreaSize.x: " + this.m_scrollAreaSize.x);
            //Debug.LogError("this.m_scrollAreaSize.y: " + this.m_scrollAreaSize.y);
            //Debug.LogError("===================================================================================================\n");

            zero.x = this.m_contentRectTransform.anchoredPosition.x + rect.m_right;
            zero.y = this.m_contentRectTransform.anchoredPosition.y + rect.m_bottom;
            return (
                (((zero.x - rect.m_width) >= 0f) && (zero.x - this.m_scrollAreaSize.x <= 0f))
                && (((zero.y + rect.m_height) <= 0f) && (zero.y + this.m_scrollAreaSize.y >= 0f))
                );
        }

        //public virtual void OnEndDrag(PointerEventData eventData)
        //{
        //    //item 全部位于 可是视区内 
        //    if (OnItemLocalPosAllInRectView != null)
        //    {
        //        for (int i = 0; i < this.m_elementScripts.Count; i++)
        //        {
        //            var element = this.m_elementScripts[i];
        //            if (element == null) continue;
        //            if (IsRectTotalInScrollArea(ref element.m_rect))
        //            {
        //                OnItemLocalPosAllInRectView(element);
        //            }
        //        }
        //    }
        //}
    }
}

