using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    [AddComponentMenu("Layout/Text Layout Element", 140)]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Text))]
    [ExecuteInEditMode]
    public class TextLayoutElement : UIBehaviour, ILayoutElement, ILayoutIgnorer
    {
        [SerializeField]
        private bool m_IgnoreLayout = false;
        [SerializeField]
        private float m_MinWidth = -1;
        [SerializeField]
        private float m_MinHeight = -1;
        [SerializeField]
        private float m_PreferredWidth = -1;
        [SerializeField]
        private float m_PreferredHeight = -1;
        [SerializeField]
        private float m_FlexibleWidth = -1;
        [SerializeField]
        private float m_FlexibleHeight = -1;
        [SerializeField]
        private Text  m_Text;
        
        public enum TextLayoutMode
        {
            LimitMaxSize = 0,
            AutoFitSize,
            Normal
        } 
        
        [SerializeField]
        private  TextLayoutMode m_verticalMode;
        
        [SerializeField]
        private  TextLayoutMode m_horizontalMode;

        public TextLayoutMode verticalMode {get {return m_verticalMode;} set {if (SetStruct(ref m_verticalMode, value)) SetDirty();}}
        public TextLayoutMode horizontalMode {get {return m_horizontalMode;} set {if (SetStruct(ref m_horizontalMode, value)) SetDirty();}}
         
        public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (currentValue.Equals(newValue))
                return false;

            currentValue = newValue;
            return true;
        }

        public virtual bool ignoreLayout { get { return m_IgnoreLayout; } set { if (SetStruct(ref m_IgnoreLayout, value)) SetDirty(); } }

        public virtual void CalculateLayoutInputHorizontal() { }
        public virtual void CalculateLayoutInputVertical() { }
        public virtual float minWidth 
        {
            get
            {
                if (m_Text == null)
                {
                    m_Text = gameObject.GetComponent<Text>();
                }

                if (m_Text && horizontalMode == TextLayoutMode.AutoFitSize)
                {
                    return m_Text.preferredWidth;
                }
                else
                {
                   return m_MinWidth;
                }
            }
            set { if (SetStruct(ref m_MinWidth, value)) SetDirty(); } 
        }
        public virtual float minHeight 
        {
             get 
             { 
                if (m_Text == null)
                {
                    m_Text = gameObject.GetComponent<Text>();
                }

                if (m_Text && verticalMode == TextLayoutMode.AutoFitSize)
                {
                    return m_Text.preferredHeight;
                }
                else
                {
                   return m_MinHeight;
                }
             } 
             set { if (SetStruct(ref m_MinHeight, value)) SetDirty(); } 
        }
        public virtual float preferredWidth
        { 
            get
            {
                if(m_Text == null)
                {
                    m_Text = gameObject.GetComponent<Text>();
                }
                
                if(m_Text && horizontalMode == TextLayoutMode.LimitMaxSize )
                {
                    return Mathf.Min(m_Text.preferredWidth, m_PreferredWidth);
                }
                else
                {
                    return m_PreferredWidth;
                }
                
            }
            set
            {
                if (SetStruct(ref m_PreferredWidth, value)) SetDirty();
            }
        }

        public virtual float preferredHeight
        {
            get
            {
                if (m_Text == null)
                {
                    m_Text = gameObject.GetComponent<Text>();
                }

                if ( m_Text && horizontalMode == TextLayoutMode.LimitMaxSize )
                {
                    return Mathf.Min(m_Text.preferredHeight, m_PreferredHeight);
                }
                else
                {
                    return m_PreferredHeight;
                }
            }
            set
            {
                if (SetStruct(ref m_PreferredHeight, value)) SetDirty();
            }
        }
        public virtual float flexibleWidth { get { return m_FlexibleWidth; } set { if (SetStruct(ref m_FlexibleWidth, value)) SetDirty(); } }
        public virtual float flexibleHeight { get { return m_FlexibleHeight; } set { if (SetStruct(ref m_FlexibleHeight, value)) SetDirty(); } }
        public virtual int layoutPriority { get { return 1; } }


        protected TextLayoutElement()
        { }

        #region Unity Lifetime calls

        protected override void Awake()
        {

        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnTransformParentChanged()
        {
            SetDirty();
        }

        protected override void OnDisable()
        {
            SetDirty();
            base.OnDisable();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            SetDirty();
        }

        protected override void OnBeforeTransformParentChanged()
        {
            SetDirty();
        }

        #endregion

        protected void SetDirty()
        {
            if (!IsActive())
                return;
            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }

#endif
    }
}
