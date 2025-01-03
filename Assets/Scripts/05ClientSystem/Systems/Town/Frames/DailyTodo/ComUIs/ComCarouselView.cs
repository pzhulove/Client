/**
 *  源码来源：【CSDN - UGUI 轮播图组件实现】https://blog.csdn.net/yangxun983323204/article/details/52860443  
 * 
 */

/// 主要关注属性、事件及函数：
///     public int CurrentIndex;
///     public Action<int> OnIndexChange;
///     public virtual void MoveToIndex(int ind);
///     public virtual void AddChild(RectTransform t);
/// by yangxun
/// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace GameClient
{
    /// <summary>
    /// 轮播图组件
    /// </summary>
    [RequireComponent(typeof(RectTransform)), ExecuteInEditMode]
    public class ComCarouselView : UIBehaviour, IEventSystemHandler, IBeginDragHandler, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler, ICanvasElement
    {
        public enum Axis
        {
            Horizontal,
            Vertical
        }

        public enum Direction
        {
            RightOrUp = -1,
            None = 0,
            LeftOrDown = 1,
        }

        public delegate System.Object OnBindItemDelegate(GameObject itemObject);
        [HideInInspector]
        public OnBindItemDelegate onBindItem;

        public delegate void OnItemCreateDelegate(ComCarouselCell item);
        [HideInInspector]
        public OnItemCreateDelegate onItemCreate;

        public delegate void OnItemIndexChange(int index);
        /// <summary>
        /// 位于正中的子元素变化的事件,参数为index
        /// </summary>
        [HideInInspector]
        public OnItemIndexChange onIndexChange;

        /// <summary>
        /// 当前处于正中的元素
        /// </summary>
        public int CurrentIndex
        {
            get
            {
                return m_index;
            }
        }

        /// <summary>
        /// 子元素数量
        /// </summary>
        public int CellCount
        {
            get
            {
                if (mScrollViewGo)
                {
                    return mScrollViewGo.transform.childCount;
                }
                return transform.childCount;
            }
        }

        [SerializeField]
        [Header("滚动的子节点的根节点，即滚动窗口")]
        private GameObject mScrollViewGo;

        /// <summary>
        /// 子物体size
        /// </summary>
        [SerializeField]
        [Header("滚动的子节点的尺寸，一般设为和其根节点尺寸一致")]
        private Vector2 mCellSize;
        /// <summary>
        /// 子物体间隔
        /// </summary>
        [SerializeField]
        [Header("滚动的子节点的间隔")]
        private Vector2 mSpacing;
        /// <summary>
        /// 方向
        /// </summary>
        [SerializeField]
        [Header("滚动动画的方向")]
        private Axis mMoveAxis;
        /// <summary>
        /// Tween时的步数
        /// </summary>
        [SerializeField]
        [Header("滚动动画的步数，步数越大，滚动越慢")]
        private int mTweenStepCount = 10;
        /// <summary>
        /// 自动轮播
        /// </summary>
        [SerializeField]
        [Header("滚动动画是否自动轮播")]
        private bool mAutoLoop = false;
        /// <summary>
        /// 轮播间隔
        /// </summary>
        [SerializeField]
        [Header("滚动动画自动轮播的时间间隔，单位秒")]
        private float mLoopSpace = 1;
        /// <summary>
        /// 轮播方向-- 1为向左移动，-1为向右移动
        /// </summary>
        [SerializeField]
        [Header("滚动动画自动轮播的方向，向右或者向上，向左或者向下")]
        private Direction mLoopDir = Direction.None;
        /// <summary>
        /// 可否拖动
        /// </summary>
        [SerializeField]
        [Header("滚动子节点是否可以拖拽")]
        private bool mDrag = true;

        [SerializeField]
        private GameObject mZeroShowGo = null;

        [SerializeField]
        [Header("拖动灵敏度，值越大，灵敏度越大")]
        [Range(1f, 5f)]
        private float mDragSensitivity = 1f;


        private bool m_dragging = false;
        private bool m_isNormalizing = false;
        private Vector2 m_currentPos;
        private int m_currentStep = 0;
        private RectTransform m_viewRectTran;
        private Vector2 m_prePos;
        private int m_index = 0, m_preIndex = 0;
        private RectTransform m_Header;
        private bool m_contentCheckCache = true;

        private List<ComCarouselCell> m_initializeCellList = new List<ComCarouselCell>();
        private bool m_isInitChildCell = false;

        private float m_currTimeDelta = 0;

        private float m_cellSizeXAndSpaceX = 0;
        private float m_cellSizeYAndSpaceY = 0;

        private float viewRectXMin
        {
            get
            {
                Vector3[] v = new Vector3[4];
                m_viewRectTran.GetWorldCorners(v);
                return v[0].x;
            }
        }

        private float viewRectXMax
        {
            get
            {
                Vector3[] v = new Vector3[4];
                m_viewRectTran.GetWorldCorners(v);
                return v[3].x;
            }
        }

        private float viewRectYMin
        {
            get
            {
                Vector3[] v = new Vector3[4];
                m_viewRectTran.GetWorldCorners(v);
                return v[0].y;
            }
        }

        private float viewRectYMax
        {
            get
            {
                Vector3[] v = new Vector3[4];
                m_viewRectTran.GetWorldCorners(v);
                return v[2].y;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (mScrollViewGo != null)
            {
                m_viewRectTran = mScrollViewGo.GetComponent<RectTransform>();
            }
            else
            {
                m_viewRectTran = GetComponent<RectTransform>();
            }
            _Initialize();
        }

        //protected override void OnEnable()
        //{
        //    base.OnEnable();
        //    ResizeChildren();
        //    return;
        //    if (Application.isPlaying)
        //    {
        //        if (ContentIsLongerThanRect())
        //        {
        //            Direction s;
        //            do
        //            {
        //                s = GetBoundaryState();
        //                LoopCell(s);
        //            } while (s != 0);
        //        }
        //    }
        //}

        protected virtual void Update()
        {
            if (!m_isInitChildCell)
            {
                return;
            }
            if (ContentIsLongerThanRect())
            {
                //实现在必要时loop子元素
                if (Application.isPlaying)
                {
                    Direction s = GetBoundaryState();
                    _LoopCell(s);
                }
                //缓动回指定位置
                if (m_isNormalizing && EnsureListCanAdjust())
                {
                    if (m_currentStep == mTweenStepCount)
                    {
                        m_isNormalizing = false;
                        m_currentStep = 0;
                        m_currentPos = Vector2.zero;
                        return;
                    }
                    if (mTweenStepCount != 0)
                    {
                        Vector2 delta = m_currentPos / mTweenStepCount;
                        m_currentStep++;
                        _TweenToCorrect(-delta);
                    }
                }
                //自动loop
                if (mAutoLoop && !m_isNormalizing && EnsureListCanAdjust())
                {
                    m_currTimeDelta += Time.deltaTime;
                    if (m_currTimeDelta > mLoopSpace)
                    {
                        m_currTimeDelta = 0;
                        MoveToIndex(m_index + (int)mLoopDir);
                    }
                }
                //检测index是否变化
                if (mMoveAxis == Axis.Horizontal)
                {
                    m_cellSizeXAndSpaceX = (mCellSize.x + mSpacing.x - 1);
                    if (m_cellSizeXAndSpaceX != 0)
                    {
                        m_index = (int)(m_Header.localPosition.x / m_cellSizeXAndSpaceX);
                    }
                }
                else
                {
                    m_cellSizeYAndSpaceY = (mCellSize.y + mSpacing.y - 1);
                    if (m_cellSizeYAndSpaceY != 0)
                    {
                        m_index = (int)(m_Header.localPosition.y / m_cellSizeYAndSpaceY);
                    }
                }
                if (m_index <= 0)
                {
                    m_index = Mathf.Abs(m_index);
                }
                else
                {
                    m_index = CellCount - m_index;
                }
                if (m_index != m_preIndex)
                {
                    if (onIndexChange != null)
                    {
                        onIndexChange(m_index);
                    }
                }
                m_preIndex = m_index;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _UnInitialize();
        }

        #region UI EventSystem
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (!mDrag || !m_contentCheckCache)
            {
                return;
            }
            Vector2 vector;
            if (((eventData.button == PointerEventData.InputButton.Left) && this.IsActive()) 
                && RectTransformUtility.ScreenPointToLocalPointInRectangle(this.m_viewRectTran, eventData.position, eventData.pressEventCamera, out vector))
            {
                this.m_dragging = true;
                m_prePos = vector;

                //重置计时器
                m_currTimeDelta = 0;
            }
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (!mDrag)
            {
                return;
            }
            return;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!mDrag || !m_contentCheckCache)
            {
                return;
            }
            Vector2 vector;
            if (((eventData.button == PointerEventData.InputButton.Left) && this.IsActive())
                && RectTransformUtility.ScreenPointToLocalPointInRectangle(this.m_viewRectTran, eventData.position, eventData.pressEventCamera, out vector))
            {
                m_isNormalizing = false;
                m_currentPos = Vector2.zero;
                m_currentStep = 0;
                Vector2 vector2 = vector - this.m_prePos;
                Vector2 vec = _CalculateOffset(vector2);
                this._SetContentPosition(vec);
                m_prePos = vector;
            }
        }
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (!mDrag || !m_contentCheckCache)
            {
                return;
            }
            this.m_dragging = false;
            this.m_isNormalizing = true;
            m_currentPos = CalcCorrectDeltaPos();
            m_currentStep = 0;
        }

        public virtual void Rebuild(CanvasUpdate executing)
        {
            return;
        }

        public void LayoutComplete()
        {

        }

        public void GraphicUpdateComplete()
        {

        }

        #endregion

        #region PRIVATE METHODS

        private void _Initialize()
        {
            if (m_viewRectTran)
            {
                m_Header = _GetChild(m_viewRectTran, 0);
            }

            if (mZeroShowGo)
            {
                mZeroShowGo.CustomActive(false);
            }
        }

        private void _UnInitialize()
        {
            onIndexChange = null;
            onBindItem = null;
            onItemCreate = null;

            if (m_initializeCellList != null)
            {
                m_initializeCellList.Clear();
            }
            m_isInitChildCell = false;
        }

        private Vector2 _CalculateOffset(Vector2 delta)
        {
            if (mMoveAxis == Axis.Horizontal)
            {
                delta.y = 0;
            }
            else
            {
                delta.x = 0;
            }
            return delta;
        }

        private void _SetContentPosition(Vector2 position)
        {
            foreach (RectTransform i in m_viewRectTran)
            {
                i.localPosition += (Vector3)position;
            }
            return;
        }

        /// <summary>
        /// Loop列表，分为-1把最右(上)边一个移到最左(下)边，1把最左(下)边一个移到最右(上)边
        /// </summary>
        /// <param name="dir"></param>
        private void _LoopCell(Direction dir)
        {
            if (dir == Direction.None)
            {
                return;
            }
            RectTransform MoveCell;
            RectTransform Tarborder;
            Vector2 TarPos;
            if (dir == Direction.LeftOrDown)
            {
                MoveCell = _GetChild(m_viewRectTran, 0);
                Tarborder = _GetChild(m_viewRectTran, CellCount - 1);
                MoveCell.SetSiblingIndex(CellCount - 1);
            }
            else
            {
                Tarborder = _GetChild(m_viewRectTran, 0);
                MoveCell = _GetChild(m_viewRectTran, CellCount - 1);
                MoveCell.SetSiblingIndex(0);
            }
            if (mMoveAxis == Axis.Horizontal)
            {
                TarPos = Tarborder.localPosition + new Vector3((mCellSize.x + mSpacing.x) * (int)dir, 0, 0);
            }
            else
            {
                TarPos = (Vector2)Tarborder.localPosition + new Vector2(0, (mCellSize.y + mSpacing.y) * (int)dir);
            }
            MoveCell.localPosition = TarPos;
        }

        /// <summary>
        /// 移动指定增量
        /// </summary>
        private void _TweenToCorrect(Vector2 delta)
        {
            foreach (RectTransform i in m_viewRectTran)
            {
                i.localPosition += (Vector3)delta;
            }
        }

        private RectTransform _GetChild(RectTransform parent, int index)
        {
            if (parent == null || index >= parent.childCount)
            {
                return null;
            }
            return parent.GetChild(index) as RectTransform;
        }

        private GameObject _InstantiateCell(GameObject srcGameObject)
        {
            GameObject obj2 = UnityEngine.GameObject.Instantiate(srcGameObject) as GameObject;
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

        private void _ResizeChildren()
        {
            //init child size and pos
            Vector2 delta;
            if (mMoveAxis == Axis.Horizontal)
            {
                delta = new Vector2(mCellSize.x + mSpacing.x, 0);
            }
            else
            {
                delta = new Vector2(0, mCellSize.y + mSpacing.y);
            }
            for (int i = 0; i < CellCount; i++)
            {
                var t = _GetChild(m_viewRectTran, i);
                if (t)
                {
                    t.localPosition = delta * i;
                    t.sizeDelta = mCellSize;
                }
            }
            m_isNormalizing = false;
            m_currentPos = Vector2.zero;
            m_currentStep = 0;
        }


        #endregion

        #region PUBLIC METHODS       

        public void SetCellAmount(int amount)
        {
            if (m_isInitChildCell)
            {
                return;
            }

            if (m_Header == null)
            {
                return;
            }

            if (amount <= 0)
            {
                if (mZeroShowGo)
                {
                    mZeroShowGo.CustomActive(true);
                    //显示在最前
                    mZeroShowGo.transform.SetAsLastSibling();
                }
                m_Header.gameObject.CustomActive(false);
            }
            else
            {
                m_Header.gameObject.CustomActive(true);
            }

            string headerName = m_Header.name;

            for (int i = 0; i < amount; i++)
            {
                GameObject go = null;
                if (i == 0)
                {
                    go = m_Header.gameObject;
                }
                else
                {
                    go = _InstantiateCell(m_Header.gameObject);
                }
                if (go == null)
                    continue;

                ComCarouselCell cellScript = go.GetComponent<ComCarouselCell>();
                if (cellScript == null)
                    continue;
                if (m_initializeCellList != null)
                {
                    m_initializeCellList.Add(cellScript);
                }
            }

            for (int i = 0; i < m_initializeCellList.Count; i++)
            {
                var cellScript = m_initializeCellList[i];
                if (cellScript == null)
                    continue;

                RectTransform rect = cellScript.GetComponent<RectTransform>();

                cellScript.Init(i, rect, headerName);

                if (onBindItem != null)
                {
                    cellScript.BindScript = onBindItem(cellScript.gameObject);
                }

                if (onItemCreate != null)
                {
                    onItemCreate(cellScript);
                }
            }

            _ResizeChildren();

            m_isInitChildCell = true;
        }

        /// <summary>
        /// List是否处于可自由调整状态
        /// </summary>
        /// <returns></returns>
        public bool EnsureListCanAdjust()
        {
            return !m_dragging && ContentIsLongerThanRect();
        }

        /// <summary>
        /// 内容是否比显示范围大
        /// </summary>
        /// <returns></returns>
        public bool ContentIsLongerThanRect()
        {
            float contentLen;
            float rectLen;
            if (mMoveAxis == Axis.Horizontal)
            {
                contentLen = CellCount * (mCellSize.x + mSpacing.x) - mSpacing.x;
                rectLen = m_viewRectTran.rect.xMax - m_viewRectTran.rect.xMin;
            }
            else
            {
                contentLen = CellCount * (mCellSize.y + mSpacing.y) - mSpacing.y;
                rectLen = m_viewRectTran.rect.yMax - m_viewRectTran.rect.yMin;
            }
            m_contentCheckCache = contentLen > rectLen;
            return m_contentCheckCache;
        }

        /// <summary>
        /// 检测边界情况，分为0未触界，-1左(下)触界，1右(上)触界
        /// </summary>
        /// <returns></returns>
        public Direction GetBoundaryState()
        {
            RectTransform left;
            RectTransform right;
            left = _GetChild(m_viewRectTran, 0);
            right = _GetChild(m_viewRectTran, CellCount - 1);
            Vector3[] l = new Vector3[4];
            left.GetWorldCorners(l);
            Vector3[] r = new Vector3[4];
            right.GetWorldCorners(r);
            if (mMoveAxis == Axis.Horizontal)
            {
                if (l[0].x >= viewRectXMin)
                {
                    return Direction.RightOrUp;
                }
                else if (r[3].x < viewRectXMax)
                {
                    return Direction.LeftOrDown;
                }
            }
            else
            {
                if (l[0].y >= viewRectYMin)
                {
                    return Direction.RightOrUp;
                }
                else if (r[1].y < viewRectYMax)
                {
                    return Direction.LeftOrDown;
                }
            }
            return Direction.None;
        }

        /// <summary>
        /// 计算一个最近的正确位置
        /// </summary>
        /// <returns></returns>
        public Vector2 CalcCorrectDeltaPos()
        {
            Vector2 delta = Vector2.zero;
            float distance = float.MaxValue;
            foreach (RectTransform i in m_viewRectTran)
            {
                var td = Mathf.Abs(i.localPosition.x) + Mathf.Abs(i.localPosition.y);
				//灵敏度值越大 需要拖动的距离越短
                if (td <= (distance * mDragSensitivity))
                {
                    distance = td;
                    delta = i.localPosition;
                }
                else
                {
                    break;
                }
            }
            return delta;
        }

        /// <summary>
        /// 加子物体到当前列表的最后面
        /// </summary>
        /// <param name="t"></param>
        public void AddChildInTheEnd(RectTransform t)
        {
            if (t != null)
            {
                t.SetParent(m_viewRectTran, false);
                t.SetAsLastSibling();
                Vector2 delta;
                if (mMoveAxis == Axis.Horizontal)
                {
                    delta = new Vector2(mCellSize.x + mSpacing.x, 0);
                }
                else
                {
                    delta = new Vector2(0, mCellSize.y + mSpacing.y);
                }
                if (CellCount == 0)
                {
                    t.localPosition = Vector3.zero;
                    m_Header = t;
                }
                else
                {
                    t.localPosition = delta + (Vector2)_GetChild(m_viewRectTran, CellCount - 1).localPosition;
                }
            }
        }

        /// <summary>
        /// 移动到指定索引
        /// </summary>
        /// <param name="ind"></param>
        public void MoveToIndex(int ind)
        {
            if (m_isNormalizing)
            {
                return;
            }
            //Debug.LogFormat("{0}->{1}",m_index,ind);
            if (ind == m_index)
            {
                return;
            }
            if (ind >= CellCount)
            {
                return;
            }
            this.m_isNormalizing = true;
            Vector2 offset;
            if (mMoveAxis == Axis.Horizontal)
            {
                offset = new Vector2(mCellSize.x + mSpacing.x, 0);
            }
            else
            {
                offset = new Vector2(0, mCellSize.y + mSpacing.y);
            }
            var delta = CalcCorrectDeltaPos();
            int vindex = m_index;
            m_currentPos = delta + offset * (ind - vindex);
            //m_CurrentPos = -(Vector2)header.localPosition + offset * (ind - m_index);
            m_currentStep = 0;
        }

        #endregion

    }
}