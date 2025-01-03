namespace Scripts.UI
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;
    using UnityEngine.Serialization;
    using UnityEngine.EventSystems;

    public class ComUIListScript : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public delegate System.Object OnBindItemDelegate(GameObject itemObject);
        public delegate void          OnItemSelectedDelegate(ComUIListElementScript item);
        public delegate void          OnItemVisiableDelegate(ComUIListElementScript item);
	    public delegate void          OnItemUpdateDelegate(ComUIListElementScript item);
        public delegate void          OnItemChangeDisplayDelegate(ComUIListElementScript item,bool bSelected);
        public delegate void          OnItemRecycleDelegate(ComUIListElementScript item);
        public delegate void          OnItemEndDragDelegate();
        public delegate void          OnItemDragDelegate();
        public delegate void          OnItemBeginDragDelegate();
        public delegate void          OnScollViewDragDelegate(Vector2 changeValue);

        [HideInInspector]
        public OnBindItemDelegate           onBindItem;
        [HideInInspector]
        public OnItemSelectedDelegate       onItemSelected;
        [HideInInspector]
        public OnItemVisiableDelegate       onItemVisiable;
        [HideInInspector]
        public OnItemChangeDisplayDelegate  onItemChageDisplay;
        [HideInInspector]
        public OnItemRecycleDelegate        OnItemRecycle;
	    [HideInInspector]
		public OnItemUpdateDelegate         OnItemUpdate;
	    [HideInInspector]
		public OnItemEndDragDelegate        OnItemEndDrag;
	    [HideInInspector]
		public OnItemDragDelegate           OnItemDrag;
	    [HideInInspector]
		public OnItemBeginDragDelegate      OnItemBeginDrag;
        [HideInInspector]
        public OnScollViewDragDelegate      OnScrollViewDrag;

        public bool m_alwaysDispatchSelectedChangeEvent;
        [HideInInspector]
        public bool m_autoAdjustScrollAreaSize;
        public bool m_autoCenteredElements;
        protected GameObject m_content;
        protected RectMask2D m_rectMask2D;
        protected Mask m_Mask;
        protected Image m_MaskImage;
        protected RectTransform m_contentRectTransform;
        protected Vector2 m_contentSize;
        public Vector2 contentSize{
            get{return m_contentSize;}
        }

        public int m_elementAmount;
        protected Vector2 m_elementDefaultSize;
        public float m_elementLayoutOffset;

        protected string m_elementName;
        protected List<ComUIListElementScript> m_elementScripts;
        public Vector2 m_elementPadding = new Vector2();
        public Vector2 m_elementSpacing = new Vector2();
        


        protected List<stRect> m_elementsRect;
        protected List<Vector2> m_elementsSize;
        protected GameObject m_elementTemplate;
        public string m_externalElementPrefabPath;
        public GameObject m_extraContent;
        public float m_fSpeed = 0.2f;
        protected Vector2 m_lastContentPosition;
        protected int m_lastSelectedElementIndex = -1;

        protected bool m_isInitialized;

        public enum ParkingDirection
        {
            ParkingLeftOrTop,
            ParkingMiddle,
            ParkingRightOrBottom,
        }

        public enum enUIListType
        {
            Vertical,
            Horizontal,
            VerticalGrid,
            HorizontalGrid
        }

        public enUIListType m_listType;
        [HideInInspector]
        public Vector2 m_scrollAreaSize;
        protected Scrollbar m_scrollBar;
        public bool m_scrollExternal;
        [HideInInspector]
        public ScrollRect m_scrollRect;
        [HideInInspector]
        public Vector2 m_scrollRectAreaMaxSize = new Vector2(100f, 100f);
        protected Vector2 m_scrollValue;
        protected int m_selectedElementIndex = -1;
        protected List<ComUIListElementScript> m_unUsedElementScripts;
        public bool m_useExternalElement;
        public bool m_useOptimized = true;
        public GameObject m_ZeroTipsObj;
        public GameObject m_HaveTipsObj;

        [SerializeField]
        private bool m_IsHideMaskWhenScrollRectDisable = false;

        protected GameObject InstantiateElement(GameObject srcGameObject)
        {
            GameObject obj2 = UnityEngine.Object.Instantiate(srcGameObject) as GameObject;
            obj2.transform.SetParent(srcGameObject.transform.parent);
            RectTransform transform = srcGameObject.transform as RectTransform;
            RectTransform transform2 = obj2.transform as RectTransform;
            if ((transform != null) && (transform2 != null))
            {
                transform2.pivot = transform.pivot;
                transform2.anchorMin = transform.anchorMin;
                transform2.anchorMax = transform.anchorMax;
                transform2.offsetMin = transform.offsetMin;
                transform2.offsetMax = transform.offsetMax;
                transform2.localPosition = transform.localPosition;
                transform2.localRotation = transform.localRotation;
                transform2.localScale = transform.localScale;
            }
            return obj2;
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Start()
        {
            //Initialize();
        }

        protected virtual ComUIListElementScript CreateElement(int index, ref stRect rect)
        {
            ComUIListElementScript item = null;
            if (this.m_unUsedElementScripts.Count > 0)
            {
                item = this.m_unUsedElementScripts[0];
                this.m_unUsedElementScripts.RemoveAt(0);
            }
            else if (this.m_elementTemplate != null)
            {
                GameObject root = InstantiateElement(this.m_elementTemplate);
                root.transform.SetParent(this.m_content.transform);
                root.transform.localScale = Vector3.one;
                item = root.GetComponent<ComUIListElementScript>();

            }
            if (item != null)
            {
                if(onBindItem != null && item.gameObjectBindScript == null)
                {
                    item.gameObjectBindScript = onBindItem(item.gameObject);
                }

                item.Initialize();
                item.Enable(this, index, this.m_elementName, ref rect, this.IsSelectedIndex(index));
                this.m_elementScripts.Add(item);

                if(onItemVisiable != null)
                {
                    onItemVisiable(item);
                }
            }
            return item;
        }

        protected void DetectScroll()
        {
            if (this.m_contentRectTransform.anchoredPosition != this.m_lastContentPosition)
            {
                if ((this.m_listType == enUIListType.Horizontal) || (this.m_listType == enUIListType.HorizontalGrid))
                {
                    float x = (this.m_contentSize.x != this.m_scrollAreaSize.x) ? (this.m_contentRectTransform.anchoredPosition.x / (this.m_contentSize.x - this.m_scrollAreaSize.x)) : 0f;
                    this.OnScrollValueChanged(new Vector2(x, 0f));
                }
                else if ((this.m_listType == enUIListType.VerticalGrid) || (this.m_listType == enUIListType.Vertical))
                {
                    float y = (this.m_contentSize.y != this.m_scrollAreaSize.y) ? (this.m_contentRectTransform.anchoredPosition.y / (this.m_contentSize.y - this.m_scrollAreaSize.y)) : 0f;
                    this.OnScrollValueChanged(new Vector2(0f, y));
                }
                this.m_lastContentPosition = this.m_contentRectTransform.anchoredPosition;
            }
        }

      

        public ComUIListElementScript GetElemenet(int index)
        {
            if ((index >= 0) && (index < this.m_elementAmount))
            {
                if (!this.m_useOptimized)
                {
                    return this.m_elementScripts[index];
                }
                for (int i = 0; i < this.m_elementScripts.Count; i++)
                {
                    if (this.m_elementScripts[i].m_index == index)
                    {
                        return this.m_elementScripts[i];
                    }
                }
            }
            return null;
        }

        public stRect GetElementRect(int index)
        {
            if ((index >= 0) && (index < this.m_elementAmount))
            {
                if (!this.m_useOptimized)
                {
                    return this.m_elementScripts[index].m_rect;
                }
                
                return this.m_elementsRect[index];
            }

            return new stRect();
        }

        public void EnsureElementVisable(int index)
        {
            if ((this.m_listType == enUIListType.Vertical) || (this.m_listType == enUIListType.VerticalGrid))
            {
                stRect rect = GetElementRect(index);
                //float value = rect.m_top / (contentSize.y - m_scrollAreaSize.y);
                //m_scrollBar.value = value;
                m_contentRectTransform.anchoredPosition = new Vector2(m_contentRectTransform.anchoredPosition.x,-rect.m_top);
            }
            else if ((this.m_listType == enUIListType.Horizontal) || (this.m_listType == enUIListType.HorizontalGrid))
            {
                stRect rect = GetElementRect(index);
                //float value = rect.m_top / (contentSize.y - m_scrollAreaSize.y);
                //m_scrollBar.value = value;
                m_contentRectTransform.anchoredPosition = new Vector2(-rect.m_left,m_contentRectTransform.anchoredPosition.y);
            }

        }
        public int GetElementAmount()
        {
            return this.m_elementAmount;
        }

        public ComUIListElementScript GetLastSelectedElement()
        {
            return this.GetElemenet(this.m_lastSelectedElementIndex);
        }

        public int GetLastSelectedIndex()
        {
            return this.m_lastSelectedElementIndex;
        }

        public Vector2 GetScrollValue()
        {
            return this.m_scrollValue;
        }

        public ComUIListElementScript GetSelectedElement()
        {
            return this.GetElemenet(this.m_selectedElementIndex);
        }

        public int GetSelectedIndex()
        {
            return this.m_selectedElementIndex;
        }

        public void HideExtraContent()
        {
            if (this.m_extraContent != null)
            {
                this.m_extraContent.CustomActive(false);
            }
        }

        public bool IsInitialised()
        {
            return m_isInitialized;
        }
        
        public void InitialLizeWithExternalElement(string prefabPath)
        {
            m_externalElementPrefabPath = prefabPath;
            m_useExternalElement = true;
            Initialize();
        }

        public void Initialize()
        {
            if (!m_isInitialized)
            {
                m_isInitialized = true;
                this.m_selectedElementIndex = -1;
                this.m_lastSelectedElementIndex = -1;
                this.m_scrollRect = base.GetComponentInChildren<ScrollRect>(base.gameObject);
                if (this.m_scrollRect != null)
                {
                    if (this.m_scrollRect.viewport != null && m_IsHideMaskWhenScrollRectDisable)
                    {
                        m_rectMask2D = this.m_scrollRect.viewport.GetComponent<RectMask2D>();
                        m_Mask = this.m_scrollRect.viewport.GetComponent<Mask>();
                        m_MaskImage = this.m_scrollRect.viewport.GetComponent<Image>();
                    }

                    if (m_scrollExternal == false)
                    {
                        this.m_scrollRect.enabled = false;

                        if (m_rectMask2D != null)
                        {
                            m_rectMask2D.enabled = false;
                        }

                        _ShowMaskEnable(false);
                    }

                    RectTransform transform = this.m_scrollRect.transform as RectTransform;
                    this.m_scrollAreaSize = new Vector2(transform.rect.width, transform.rect.height);
                    this.m_content = this.m_scrollRect.content.gameObject;
                    m_scrollRect.onValueChanged.RemoveListener(_OnScrollViewDrag);
                    m_scrollRect.onValueChanged.AddListener(_OnScrollViewDrag);
                }

                this.m_scrollBar = base.GetComponentInChildren<Scrollbar>(base.gameObject);
                if ((this.m_listType == enUIListType.Vertical) || (this.m_listType == enUIListType.VerticalGrid))
                {
                    if (this.m_scrollBar != null)
                    {
                        this.m_scrollBar.direction = Scrollbar.Direction.BottomToTop;
                    }
                    //DebugHelper.Assert(this.m_scrollRect != null);
                    if (this.m_scrollRect != null)
                    {
                        this.m_scrollRect.horizontal = false;
                        this.m_scrollRect.vertical = true;
                        this.m_scrollRect.horizontalScrollbar = null;
                        this.m_scrollRect.verticalScrollbar = this.m_scrollBar;
                    }
                }
                else if ((this.m_listType == enUIListType.Horizontal) || (this.m_listType == enUIListType.HorizontalGrid))
                {
                    if (this.m_scrollBar != null)
                    {
                        this.m_scrollBar.direction = Scrollbar.Direction.LeftToRight;
                    }
                    //DebugHelper.Assert(this.m_scrollRect != null);
                    if (this.m_scrollRect != null)
                    {
                        this.m_scrollRect.horizontal = true;
                        this.m_scrollRect.vertical = false;
                        this.m_scrollRect.horizontalScrollbar = this.m_scrollBar;
                        this.m_scrollRect.verticalScrollbar = null;
                    }
                }
                this.m_elementScripts = new List<ComUIListElementScript>();
                this.m_unUsedElementScripts = new List<ComUIListElementScript>();
                if (this.m_useOptimized)
                {
                    this.m_elementsRect = new List<stRect>();
                }
                ComUIListElementScript component = null;
                if (this.m_useExternalElement)
                {

                    GameObject content = (GameObject)AssetLoader.instance.LoadResAsGameObject(this.m_externalElementPrefabPath);
                    component = content.GetComponent<ComUIListElementScript>();
                    if (component != null)
                    {
                        component.Initialize();
                        this.m_elementTemplate = content;
                        this.m_elementName = content.name;
                        this.m_elementDefaultSize = component.m_defaultSize;
                        this.m_elementTemplate.name = this.m_elementName + "_Template";
                        this.m_elementTemplate.transform.SetParent(this.m_content.transform, false);
                    }
                }
                else
                {
                    component = base.GetComponentInChildren<ComUIListElementScript>(base.gameObject);
                    if (component != null)
                    {
                        component.Initialize();
                        this.m_elementTemplate = component.gameObject;
                        this.m_elementName = component.gameObject.name;
                        this.m_elementDefaultSize = component.m_defaultSize;
                        if (this.m_elementTemplate != null)
                        {
                            this.m_elementTemplate.name = this.m_elementName + "_Template";
                        }
                    }
                }
                if (this.m_elementTemplate != null)
                {
                    ComUIListElementScript script2 = this.m_elementTemplate.GetComponent<ComUIListElementScript>();
                    if ((script2 != null) && script2.m_useSetActiveForDisplay)
                    {
                        this.m_elementTemplate.CustomActive(false);
                    }
                    else
                    {
                        if (!this.m_elementTemplate.activeSelf)
                        {
                            this.m_elementTemplate.SetActive(true);
                        }
                        CanvasGroup group = this.m_elementTemplate.GetComponent<CanvasGroup>();
                        if (group == null)
                        {
                            group = this.m_elementTemplate.AddComponent<CanvasGroup>();
                        }
                        group.alpha = 0f;
                        group.blocksRaycasts = false;
                    }
                }

                if (this.m_content != null)
                {
                    this.m_contentRectTransform = this.m_content.transform as RectTransform;
                    this.m_contentRectTransform.pivot = new Vector2(0f, 1f);
                    this.m_contentRectTransform.anchorMin = new Vector2(0f, 1f);
                    this.m_contentRectTransform.anchorMax = new Vector2(0f, 1f);
                    this.m_contentRectTransform.anchoredPosition = Vector2.zero;
                    this.m_contentRectTransform.localRotation = Quaternion.identity;
                    this.m_contentRectTransform.localScale = new Vector3(1f, 1f, 1f);
                    this.m_lastContentPosition = this.m_contentRectTransform.anchoredPosition;
                }
                if (this.m_extraContent != null)
                {
                    RectTransform transform2 = this.m_extraContent.transform as RectTransform;
                    transform2.pivot = new Vector2(0f, 1f);
                    transform2.anchorMin = new Vector2(0f, 1f);
                    transform2.anchorMax = new Vector2(0f, 1f);
                    transform2.anchoredPosition = Vector2.zero;
                    transform2.localRotation = Quaternion.identity;
                    transform2.localScale = Vector3.one;
                    if (this.m_elementTemplate != null)
                    {
                        transform2.sizeDelta = new Vector2((this.m_elementTemplate.transform as RectTransform).rect.width, transform2.sizeDelta.y);
                    }
                    if ((transform2.parent != null) && (this.m_content != null))
                    {
                        transform2.parent.SetParent(this.m_content.transform);
                    }
                    this.m_extraContent.SetActive(false);
                }
                this.SetElementAmount(this.m_elementAmount);
            }
        }

        private void _ShowMaskEnable(bool isShow)
        {
            if (!m_IsHideMaskWhenScrollRectDisable)
            {
                return;
            }

            if (m_Mask == null)
            {
                return;
            }

            m_Mask.enabled = isShow;
            if (m_MaskImage != null)
            {
                m_MaskImage.enabled = isShow;
            }
        }

        protected virtual string GetElementPrefabPath(int index = 0)
        {
            return m_externalElementPrefabPath;
        }
        // 根据成对编码原则，应该要有反初始化的流程
        public void UnInitialize()
        {
            m_isInitialized = false;
            m_lastSelectedElementIndex = -1;
            m_selectedElementIndex = -1;
            onBindItem = null;
            onItemVisiable = null;
            onItemSelected = null;
            onItemChageDisplay = null;
            OnItemRecycle = null;
            OnItemUpdate = null;
            OnItemEndDrag = null;
            OnItemDrag = null;
            OnItemBeginDrag = null;
            return;
        }
        public bool IsElementInScrollArea(int index)
        {
            if ((index < 0) || (index >= this.m_elementAmount))
            {
                return false;
            }
            stRect rect = !this.m_useOptimized ? this.m_elementScripts[index].m_rect : this.m_elementsRect[index];
            return this.IsRectInScrollArea(ref rect);
        }

        protected bool IsRectInScrollArea(ref stRect rect)
        {
            Vector2 zero = Vector2.zero;
            zero.x = this.m_contentRectTransform.anchoredPosition.x + rect.m_left;
            zero.y = this.m_contentRectTransform.anchoredPosition.y + rect.m_top;
            return ((((zero.x + rect.m_width) >= 0f) && (zero.x <= this.m_scrollAreaSize.x)) && (((zero.y - rect.m_height) <= 0f) && (zero.y >= -this.m_scrollAreaSize.y)));        //顶部或者左侧大于显示右侧或者底部边界  底部或者右侧大于左侧或者顶部边界
        }

        public virtual bool IsSelectedIndex(int index)
        {
            return (this.m_selectedElementIndex == index);
        }

        protected virtual stRect LayoutElement(int index, ref Vector2 contentSize, ref Vector2 offset)
        {
            stRect rect;
            rect = new stRect();

            rect.m_width = (((this.m_elementsSize != null) && (this.m_listType != enUIListType.Vertical)) && ((this.m_listType != enUIListType.VerticalGrid) && (this.m_listType != enUIListType.HorizontalGrid))) ? ((int)this.m_elementsSize[index].x) : ((int)this.m_elementDefaultSize.x);
            rect.m_height = (((this.m_elementsSize != null) && (this.m_listType != enUIListType.Horizontal)) && ((this.m_listType != enUIListType.VerticalGrid) && (this.m_listType != enUIListType.HorizontalGrid))) ? ((int)this.m_elementsSize[index].y) : ((int)this.m_elementDefaultSize.y);
            rect.m_left = (int)offset.x;
            rect.m_top = (int)offset.y;
            rect.m_right = rect.m_left + rect.m_width;
            rect.m_bottom = rect.m_top - rect.m_height;
            rect.m_center = new Vector2(rect.m_left + (rect.m_width * 0.5f), rect.m_top - (rect.m_height * 0.5f));

            if (rect.m_right > contentSize.x)
            {
                contentSize.x = rect.m_right;
            }
            if (-rect.m_bottom > contentSize.y)
            {
                contentSize.y = -rect.m_bottom;
            }
            switch (this.m_listType)
            {
                case enUIListType.Vertical:
                    offset.y -= rect.m_height + this.m_elementSpacing.y;
                    return rect;

                case enUIListType.Horizontal:
                    offset.x += rect.m_width + this.m_elementSpacing.x;
                    return rect;

                case enUIListType.VerticalGrid:
                    offset.x += rect.m_width + this.m_elementSpacing.x;
                    if ((offset.x + rect.m_width) > this.m_scrollAreaSize.x)
                    {
                        offset.x = m_elementPadding.x;
                        offset.y -= rect.m_height + this.m_elementSpacing.y;
                    }
                    return rect;

                case enUIListType.HorizontalGrid:
                    offset.y -= rect.m_height + this.m_elementSpacing.y;
                    if ((-offset.y + rect.m_height) > this.m_scrollAreaSize.y)
                    {
                        offset.y = m_elementPadding.y;
                        offset.x += rect.m_width + this.m_elementSpacing.x;
                    }
                    return rect;
            }
            return rect;
        }

        public void MoveElementInScrollArea(int index, bool moveImmediately)
        {
            if ((index >= 0) && (index < this.m_elementAmount))
            {
                Vector2 zero = Vector2.zero;
                Vector2 vector2 = Vector2.zero;
                stRect rect = !this.m_useOptimized ? this.m_elementScripts[index].m_rect : this.m_elementsRect[index];
                vector2.x = this.m_contentRectTransform.anchoredPosition.x + rect.m_left;
                vector2.y = this.m_contentRectTransform.anchoredPosition.y + rect.m_top;
                if (vector2.x < 0f)
                {
                    zero.x = -vector2.x;
                }
                else if ((vector2.x + rect.m_width) > this.m_scrollAreaSize.x)
                {
                    zero.x = this.m_scrollAreaSize.x - (vector2.x + rect.m_width);
                }
                if (vector2.y > 0f)
                {
                    zero.y = -vector2.y;
                }
                else if ((vector2.y - rect.m_height) < -this.m_scrollAreaSize.y)
                {
                    zero.y = -this.m_scrollAreaSize.y - (vector2.y - rect.m_height);
                }
                if (moveImmediately)
                {
                    this.m_contentRectTransform.anchoredPosition += zero;
                }
                else
                {
                    Vector2 to = this.m_contentRectTransform.anchoredPosition + zero;
                    //LeanTween.value(base.gameObject, pos => this.m_contentRectTransform.anchoredPosition = pos, this.m_contentRectTransform.anchoredPosition, to, this.m_fSpeed);
                }
            }
        }

        private void OnDestroy()
        {
            /* 
            if (LeanTween.IsInitialised())
            {
                LeanTween.cancel(base.gameObject);
            }
            */
        }

        protected void OnScrollValueChanged(Vector2 value)
        {
            this.m_scrollValue = value;
            //this.DispatchScrollChangedEvent();
        }

        public void AutoScroll2Bottom()
        {
            this.m_scrollValue = new Vector2 { x = 0.0f, y = 1.0f };
        }

        protected virtual void ProcessElements()
        {
            this.m_contentSize = Vector2.zero;
            Vector2 zero = Vector2.zero;
            if ((this.m_listType == enUIListType.Vertical) || (this.m_listType == enUIListType.VerticalGrid))
            {
                zero.y += this.m_elementLayoutOffset;
            }
            else
            {
                zero.x += this.m_elementLayoutOffset;
            }

            zero.x += this.m_elementPadding.x;
            zero.y -= this.m_elementPadding.y;

            for (int i = 0; i < this.m_elementAmount; i++)
            {
                stRect item = this.LayoutElement(i, ref this.m_contentSize, ref zero);
                if (this.m_useOptimized)
                {
                    if (i < this.m_elementsRect.Count)
                    {
                        this.m_elementsRect[i] = item;
                    }
                    else
                    {
                        this.m_elementsRect.Add(item);
                    }
                }
                if (!this.m_useOptimized || this.IsRectInScrollArea(ref item))
                {
                    this.CreateElement(i, ref item);
                }
            }

            if ((this.m_listType == enUIListType.Vertical) || (this.m_listType == enUIListType.VerticalGrid))
            {
                m_contentSize.y += this.m_elementPadding.y;
            }
            else
            {
                m_contentSize.x += this.m_elementPadding.x;
            }

            if (this.m_extraContent != null)
            {
                if (this.m_elementAmount > 0)
                {
                    this.ProcessExtraContent(ref this.m_contentSize, zero);
                }
                else
                {
                    this.m_extraContent.CustomActive(false);
                }
            }
            this.ResizeContent(ref this.m_contentSize, false);
        }

        private void ProcessExtraContent(ref Vector2 contentSize, Vector2 offset)
        {
            RectTransform transform = this.m_extraContent.transform as RectTransform;
            transform.anchoredPosition = offset;
            this.m_extraContent.CustomActive(true);
            if ((this.m_listType == enUIListType.Horizontal) || (this.m_listType == enUIListType.HorizontalGrid))
            {
                contentSize.x += transform.rect.width + this.m_elementSpacing.x;
            }
            else
            {
                contentSize.y += transform.rect.height + this.m_elementSpacing.y;
            }
        }

        protected void ProcessUnUsedElement()
        {
            if ((this.m_unUsedElementScripts != null) && (this.m_unUsedElementScripts.Count > 0))
            {
                for (int i = 0; i < this.m_unUsedElementScripts.Count; i++)
                {
                    this.m_unUsedElementScripts[i].Disable();
                }
            }
        }

        protected void RecycleElement(bool disableElement)
        {
            if(this.m_elementScripts == null)
            {
                return;
            }

            while (this.m_elementScripts.Count > 0)
            {
                ComUIListElementScript item = this.m_elementScripts[0];
                this.m_elementScripts.RemoveAt(0);
                if (disableElement)
                {
                    item.Disable();
                }

                if(OnItemRecycle != null)
                {
                    OnItemRecycle(item);
                }

                this.m_unUsedElementScripts.Add(item);
            }
        }

        protected void RecycleElement(ComUIListElementScript elementScript, bool disableElement)
        {
            if (disableElement)
            {
                elementScript.Disable();
            }

            if(OnItemRecycle != null)
            {
                OnItemRecycle(elementScript);
            }

            this.m_elementScripts.Remove(elementScript);
            this.m_unUsedElementScripts.Add(elementScript);
        }

        public void ResetContentPosition()
        {
            /* 
            if (LeanTween.IsInitialised())
            {
                LeanTween.cancel(base.gameObject);
            }
            */
            if (this.m_contentRectTransform != null)
            {
                this.m_contentRectTransform.anchoredPosition = Vector2.zero;
            }
        }

        protected virtual void ResizeContent(ref Vector2 size, bool resetPosition)
        {
            float x = 0f;
            float y = 0f;
            if (this.m_autoAdjustScrollAreaSize)
            {
                Vector2 scrollAreaSize = this.m_scrollAreaSize;
                this.m_scrollAreaSize = size;
                if (this.m_scrollAreaSize.x > this.m_scrollRectAreaMaxSize.x)
                {
                    this.m_scrollAreaSize.x = this.m_scrollRectAreaMaxSize.x;
                }
                if (this.m_scrollAreaSize.y > this.m_scrollRectAreaMaxSize.y)
                {
                    this.m_scrollAreaSize.y = this.m_scrollRectAreaMaxSize.y;
                }
                Vector2 vector2 = this.m_scrollAreaSize - scrollAreaSize;
                if (vector2 != Vector2.zero)
                {
                    RectTransform transform = base.gameObject.transform as RectTransform;
                    if (transform.anchorMin == transform.anchorMax)
                    {
                        transform.sizeDelta += vector2;
                    }
                }
            }
            else if (this.m_autoCenteredElements)
            {
                if ((this.m_listType == enUIListType.Vertical) && (size.y < this.m_scrollAreaSize.y))
                {
                    y = (size.y - this.m_scrollAreaSize.y) / 2f;
                }
                else if ((this.m_listType == enUIListType.Horizontal) && (size.x < this.m_scrollAreaSize.x))
                {
                    x = (this.m_scrollAreaSize.x - size.x) / 2f;
                }
                else if ((this.m_listType == enUIListType.VerticalGrid) && (size.x < this.m_scrollAreaSize.x))
                {
                    x = (this.m_scrollAreaSize.x - size.x) / 2f;
                }
            }
            if (size.x < this.m_scrollAreaSize.x)
            {
                size.x = this.m_scrollAreaSize.x;
            }
            if (size.y < this.m_scrollAreaSize.y)
            {
                size.y = this.m_scrollAreaSize.y;
            }
            if (this.m_contentRectTransform != null)
            {
                this.m_contentRectTransform.sizeDelta = size;
                if (resetPosition)
                {
                    this.m_contentRectTransform.anchoredPosition = Vector2.zero;
                }
                if (this.m_autoCenteredElements)
                {
                    if (x != 0f)
                    {
                        this.m_contentRectTransform.anchoredPosition = new Vector2(x, this.m_contentRectTransform.anchoredPosition.y);
                    }
                    if (y != 0f)
                    {
                        this.m_contentRectTransform.anchoredPosition = new Vector2(this.m_contentRectTransform.anchoredPosition.x, y);
                    }
                }
            }
        }

        public virtual void SelectElement(int index, bool isDispatchSelectedChangeEvent = true)
        {
            this.m_lastSelectedElementIndex = this.m_selectedElementIndex;
            this.m_selectedElementIndex = index;
            if (this.m_lastSelectedElementIndex == this.m_selectedElementIndex)
            {
                if (this.m_alwaysDispatchSelectedChangeEvent)
                {
                    ComUIListElementScript script2 = this.GetElemenet(this.m_selectedElementIndex);
                    if (script2 != null && onItemSelected != null)
                    {
                        onItemSelected(script2);
                    }
                }
            }
            else
            {
                if (this.m_lastSelectedElementIndex >= 0)
                {
                    ComUIListElementScript elemenet = this.GetElemenet(this.m_lastSelectedElementIndex);
                    if (elemenet != null)
                    {
                        elemenet.ChangeDisplay(false);
                    }
                }
                if (this.m_selectedElementIndex >= 0)
                {
                    ComUIListElementScript script2 = this.GetElemenet(this.m_selectedElementIndex);
                    if (script2 != null)
                    {
                        script2.ChangeDisplay(true);
                        
                        if(onItemSelected != null)
                        {
                            onItemSelected(script2);
                        }
                    }
                }
                if (isDispatchSelectedChangeEvent)
                {
                    //this.DispatchElementSelectChangedEvent();
                }
            }
        }

        /// <summary>
        /// 重置m_lastSelectedElementIndex m_selectedElementIndex
        /// </summary>
        public void ResetSelectedElementIndex()
        {
            m_selectedElementIndex = -1;
            m_lastSelectedElementIndex = 0;
        }

        public void SetElementAmount(int amount)
        {
            this.SetElementAmount(amount, null);
        }

	    public void UpdateElementAmount(int amount)
	    {
			//如果未初始化 则调用SetElementAmout
		    if (m_useOptimized && m_elementsRect == null)
		    {
				SetElementAmount(amount);
			    return;
		    }
		    Vector2 zero = Vector2.zero;
			//获取每个元素的宽高
		    var cellSize = zero;
		    var width = (((this.m_elementsSize != null) && (this.m_listType != enUIListType.Vertical)) && ((this.m_listType != enUIListType.VerticalGrid) && (this.m_listType != enUIListType.HorizontalGrid))) ? ((int)this.m_elementsSize[m_elementsSize.Count - 1].x) : ((int)this.m_elementDefaultSize.x);
		    var height = (((this.m_elementsSize != null) && (this.m_listType != enUIListType.Horizontal)) && ((this.m_listType != enUIListType.VerticalGrid) && (this.m_listType != enUIListType.HorizontalGrid))) ? ((int)this.m_elementsSize[m_elementsSize.Count - 1].y) : ((int)this.m_elementDefaultSize.y);
		    if ((this.m_listType == enUIListType.Vertical) || (this.m_listType == enUIListType.VerticalGrid))
		    {
			    zero.y += this.m_elementLayoutOffset;
			    cellSize.y += height;
		    }
			else
		    {
			    zero.x += this.m_elementLayoutOffset;
			    cellSize.x += width;
		    }

			zero.x += this.m_elementPadding.x;
		    zero.y -= this.m_elementPadding.y;
		    cellSize += zero;

            //0个元素的提示
            if (amount <= 0)
            {
                this.m_ZeroTipsObj.CustomActive(true);
                this.m_HaveTipsObj.CustomActive(false);
            }
            else
            {
                this.m_ZeroTipsObj.CustomActive(false);
                this.m_HaveTipsObj.CustomActive(true);
            }

            //如果有新增的元素
            if (amount > m_elementAmount)
		    {
				//计算每新增一个元素需要增加的宽和高
			    var deltaY = 0f;
			    var deltaX = 0f;
				switch (this.m_listType)
			    {
				    case enUIListType.Vertical:
					    deltaY -= height + this.m_elementSpacing.y;
					    break;
				    case enUIListType.Horizontal:
					    deltaX += width + this.m_elementSpacing.x;
					    break;
				    case enUIListType.VerticalGrid:
					    deltaX += width + this.m_elementSpacing.x;
					    if ((deltaX + width) > this.m_scrollAreaSize.x)
					    {
						    deltaX = m_elementPadding.x;
						    deltaY -= height + this.m_elementSpacing.y;
					    }
					    break;
				    case enUIListType.HorizontalGrid:
					    deltaY -= height + this.m_elementSpacing.y;
					    if ((-deltaY + height) > this.m_scrollAreaSize.y)
					    {
						    deltaY = m_elementPadding.y;
						    deltaX += width + this.m_elementSpacing.x;
					    }
					    break;
			    }

				//根据新增元素个数*每个需要的宽高 计算出新增元素的起始坐标
			    zero.x += deltaX * m_elementAmount;
			    zero.y += deltaY * m_elementAmount;

				//先刷新当前显示区域的元素
			    for (int i = 0; i < m_elementScripts.Count; ++i)
			    {
					if (OnItemUpdate != null)
						OnItemUpdate(m_elementScripts[i]);
			    }

				//遍历增加新的元素
				for (int i = m_elementAmount; i < amount; ++i)
			    {
					//计算新增元素的Rect
				    stRect item = this.LayoutElement(i, ref this.m_contentSize, ref zero);
				    if (this.m_useOptimized)
				    {
					    if (i < this.m_elementsRect.Count)
					    {
						    this.m_elementsRect[i] = item;
					    }
					    else
					    {
						    this.m_elementsRect.Add(item);
					    }
				    }
				    if (!this.m_useOptimized || this.IsRectInScrollArea(ref item))
				    {
					    this.CreateElement(i, ref item);
				    }
				}
				//重新计算容器的大小
			    ResizeContent(ref m_contentSize, false);
			    m_elementAmount = amount;
			}
			//如果元素有减少或者没有变化
			else
			{
				bool isScrollToEnd = false;
				//判断显示区域是否有元素减少，如果有则回收
				for (int i = m_elementScripts.Count - 1; i >= 0; --i)
				{
					var lastElement = m_elementScripts[i];
					if (lastElement.m_index >= amount)
					{
						RecycleElement(lastElement, true);
						isScrollToEnd = true;
					}
				}

				//重新计算容器大小
				m_contentSize += cellSize * (amount - m_elementAmount);
				ResizeContent(ref m_contentSize, false);

				m_elementAmount = amount;
				//如果显示区域有删除元素则需要重新滚到底部
				if (isScrollToEnd)
				{
					MoveElementInScrollArea(amount - 1, true);
				}

				//刷新显示区域的元素
				for (int i = 0; i < m_elementScripts.Count; ++i)
				{
					if (OnItemUpdate != null)
						OnItemUpdate(m_elementScripts[i]);
				}
			}
		}

        private int mScroll2Index = 0;
        private ParkingDirection mScroll2ParkingDirection = ParkingDirection.ParkingLeftOrTop;
        /// <summary>
        /// 0表示停靠左侧(顶部)   1表示停靠中间   2表示停靠右侧（底部）ture表示慢慢滑动 false表示一步到位
        /// </summary>
        /// <param name="index"></param>
        /// <param name="parkingDirection"></param>
        public void ScrollToItem(int index, ParkingDirection parkingDirection = ParkingDirection.ParkingLeftOrTop, bool needMove = false, float moveTime = 0.5f)
        {
            if ((index >= 0) && (index < this.m_elementAmount))
            {
                if (m_elementScripts == null)
                {
                    mScroll2Index = index;
                    mScroll2ParkingDirection = parkingDirection;
                    return;
                }
                Vector2 zero = Vector2.zero;
                Vector2 vector2 = Vector2.zero;
                stRect rect = !this.m_useOptimized ? this.m_elementScripts[index].m_rect : this.m_elementsRect[index];

                switch (parkingDirection)
                {
                    case ParkingDirection.ParkingLeftOrTop:
                        {
                            vector2.x = -rect.m_left + m_elementPadding.x;       //往左滑动x变小 而rect.m_left是item左边缘相对于content的位置大于0所以要加 -
                            vector2.y = -rect.m_top - m_elementPadding.y;        //往上滑动y变大 而rect.m_top是item上边缘相对于content的位置小于0所以要加 -
                        }
                        break;
                    case ParkingDirection.ParkingMiddle:
                        {
                            vector2.x = -(rect.m_center.x - m_scrollAreaSize.x * 0.5f);
                            vector2.y = -rect.m_center.y - m_scrollAreaSize.y * 0.5f;
                        }
                        break;
                    case ParkingDirection.ParkingRightOrBottom:
                        {
                            vector2.x = -(rect.m_right - m_scrollAreaSize.x);
                            vector2.y = -rect.m_bottom - m_scrollAreaSize.y;
                        }
                        break;
                    default:
                        {
                            vector2.x = -rect.m_left;
                            vector2.y = -rect.m_top;
                        }
                        break;
                }

                if ((this.m_listType == enUIListType.Horizontal) || (this.m_listType == enUIListType.HorizontalGrid))
                {
                    zero.x = vector2.x;
                    float targetPosX = zero.x; //目标位置
                    float limitPosX = this.m_contentRectTransform.sizeDelta.x - this.m_scrollAreaSize.x;        //拖动上限位置
                    if (-targetPosX > limitPosX)
                    {
                        zero.x = -limitPosX;
                    }
                    if (targetPosX > 0)
                    {
                        zero.x = 0;
                    }
                }

                if ((this.m_listType == enUIListType.Vertical) || (this.m_listType == enUIListType.VerticalGrid))
                {
                    zero.y = vector2.y;
                    float targetPosY = zero.y; //目标位置
                    float limitPosY = this.m_contentRectTransform.sizeDelta.y - this.m_scrollAreaSize.y;        //拖动上限位置
                    if (targetPosY > limitPosY)    //如果目标位置超出了可拖动位置上限，则目标位置值改为可拖动上限
                    {
                        zero.y = limitPosY;
                    }

                    if (targetPosY < 0)
                    {
                        zero.y = 0;
                    }
                }
                if (needMove)
                {
                    zero.x -= m_scrollAreaSize.x * 0.5f;
                    zero.y += m_scrollAreaSize.y * 0.5f;
                    this.m_contentRectTransform.DOLocalMove(zero, moveTime, true);
                }
                else
                    this.m_contentRectTransform.anchoredPosition = zero;
                UpdateElementsScroll();
            }
        }

        public void UpdateElement()
        {
            if(m_elementScripts == null || m_elementScripts.Count <= 0)
                return;

            for (var i = 0; i < m_elementScripts.Count; i++)
            {
                if (OnItemUpdate != null)
                    OnItemUpdate(m_elementScripts[i]);
            }
        }

        public virtual void SetElementAmount(int amount, List<Vector2> elementsSize)
        {
            if (amount < 0)
            {
                amount = 0;
            }
            if ((elementsSize == null) || (amount == elementsSize.Count))
            {
                this.RecycleElement(false);
                this.m_elementAmount = amount;
                this.m_elementsSize = elementsSize;
                this.ProcessElements();
                this.ProcessUnUsedElement();

                if (amount <= 0)
                {
                    this.m_ZeroTipsObj.CustomActive(true);
                    this.m_HaveTipsObj.CustomActive(false);
                }
                else
                {
                    this.m_ZeroTipsObj.CustomActive(false);
                    this.m_HaveTipsObj.CustomActive(true);
                }
            }
        }

     
        public void ShowExtraContent()
        {
            if (this.m_extraContent != null)
            {
                this.m_extraContent.CustomActive(true);
            }
        }

        protected virtual void Update()
        {
            if(m_isInitialized)
            //if ((base.m_belongedFormScript == null) || !base.m_belongedFormScript.IsClosed())
            {
                if (this.m_useOptimized)
                {
                    this.UpdateElementsScroll();
                }
                if ((this.m_scrollRect != null) && !this.m_scrollExternal)
                {
                    if ((this.m_contentSize.x > this.m_scrollAreaSize.x) || (this.m_contentSize.y > this.m_scrollAreaSize.y))
                    {
                        if (!this.m_scrollRect.enabled)
                        {
                            this.m_scrollRect.enabled = true;
                        }

                        if (m_IsHideMaskWhenScrollRectDisable)
                        {
                            if (m_rectMask2D != null && !m_rectMask2D.enabled)
                            {
                                m_rectMask2D.enabled = true;
                            }

                            if (m_Mask != null && !m_Mask.enabled)
                            {
                                _ShowMaskEnable(true);
                            }
                        }
                    }
                    else if ((this.m_contentSize.x <= this.m_scrollAreaSize.x) && (this.m_contentSize.y <= this.m_scrollAreaSize.y) && this.m_scrollRect.enabled)
                    {
                        this.m_scrollRect.enabled = false;

                        if (m_IsHideMaskWhenScrollRectDisable)
                        {
                            if (m_rectMask2D != null && m_rectMask2D.enabled)
                            {
                                m_rectMask2D.enabled = false;
                            }

                            if (m_Mask != null && m_Mask.enabled)
                            {
                                _ShowMaskEnable(false);
                            }
                        }
                    }
                    else if (((Mathf.Abs(this.m_contentRectTransform.anchoredPosition.x) < 0.001) && (Mathf.Abs(this.m_contentRectTransform.anchoredPosition.y) < 0.001)) && this.m_scrollRect.enabled)
                    {
                        //this.m_scrollRect.enabled = false;
                    }
                    this.DetectScroll();
                }
            }
        }

        protected void UpdateElementsScroll()
        {
            int num = 0;
            while (num < this.m_elementScripts.Count)
            {
                if (!this.IsRectInScrollArea(ref this.m_elementScripts[num].m_rect))
                {
                    this.RecycleElement(this.m_elementScripts[num], true);
                }
                else
                {
                    num++;
                }
            }
            for (int i = 0; i < this.m_elementAmount; i++)
            {
                if (i < m_elementsRect.Count)
                {
                    stRect rect = this.m_elementsRect[i];
                    if (this.IsRectInScrollArea(ref rect))
                    {
                        bool flag = false;
                        for (int j = 0; j < this.m_elementScripts.Count; j++)
                        {
                            if (this.m_elementScripts[j].m_index == i)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            this.CreateElement(i, ref rect);
                        }
                    }
                }
            }
        }

        protected void SetProperty<T>(ref T currentValue, T newValue)
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return;
            currentValue = newValue;
            SetDirty();
        }

        public virtual bool IsActive()
        {
            return isActiveAndEnabled;
        }

        protected void SetDirty()
        {
            if (!IsActive())
                return;

            SetElementAmount(this.m_elementAmount, m_elementsSize);
        }

        protected virtual void OnDidApplyAnimationProperties()
        {
            if(!IsActive())
                return;
            SetDirty();
        }

        //拖拽中
        public void OnDrag(PointerEventData eventData)
        {
            if (null != OnItemDrag)
                OnItemDrag();
        }

        //开始拖拽
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (null != OnItemBeginDrag)
                OnItemBeginDrag();
        }

        //放开拖拽
        public void OnEndDrag(PointerEventData eventData)
        {
            if (null != OnItemEndDrag)
                OnItemEndDrag();
        }

        private void _OnScrollViewDrag(Vector2 delt)
        {
            if(OnScrollViewDrag != null)
            {
                OnScrollViewDrag(delt);
            }
        }
#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            SetDirty();
        }
#endif
    }
}

