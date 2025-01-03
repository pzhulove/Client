using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace UnityEngine.UI
{
    [AddComponentMenu("GeUISwitchButton")]
    [RequireComponent(typeof(RectTransform))]
    public class GeUISwitchButton : Selectable,IInitializePotentialDragHandler, ICanvasElement
    {
        public enum Direction
        {
            LeftToRight,
            RightToLeft,
            BottomToTop,
            TopToBottom,
        }

        [Serializable]
        public class SwitchEvent : UnityEvent<bool> { }

        [SerializeField]
        private RectTransform m_FillRect;
        public RectTransform fillRect { get { return m_FillRect; } set { if (_SetClass(ref m_FillRect, value)) { UpdateCachedReferences(); UpdateVisuals(); } } }

        [SerializeField]
        private RectTransform m_HandleRect;
        public RectTransform handleRect { get { return m_HandleRect; } set { if (_SetClass(ref m_HandleRect, value)) { UpdateCachedReferences(); UpdateVisuals(); } } }

        [SerializeField]
        private Text m_HandleText = null;
        public Text handleText { get { return m_HandleText; } set { if (_SetClass(ref m_HandleText, value)) { UpdateCachedReferences(); UpdateVisuals(); } } }

        [SerializeField]
        private Image m_HandleImage = null;
        public Image handleImage { get { return m_HandleImage; } set { if (_SetClass(ref m_HandleImage, value)) { UpdateCachedReferences(); UpdateVisuals(); } } }

        [Space]
        [SerializeField]
        private Sprite m_OnImage = null;
        public Sprite onImage { get { return m_OnImage; } set { if (_SetClass(ref m_OnImage, value)) { UpdateCachedReferences(); UpdateVisuals(); } } }

        [SerializeField]
        private Sprite m_OffImage = null;
        public Sprite offImage { get { return m_OffImage; } set { if (_SetClass(ref m_OffImage, value)) { UpdateCachedReferences(); UpdateVisuals(); } } }

        [SerializeField]
        private string m_OnText = null;
        public string onText { get { return m_OnText; } set { if (_SetClass(ref m_OnText, value)) { UpdateCachedReferences(); UpdateVisuals(); } } }

        [SerializeField]
        private string m_OffText = null;
        public string offText { get { return m_OffText; } set { if (_SetClass(ref m_OffText, value)) { UpdateCachedReferences(); UpdateVisuals(); } } }

        [SerializeField]
        private Direction m_Direction = Direction.LeftToRight;
        public Direction direction { get { return m_Direction; } set { if (_SetStruct(ref m_Direction, value)) UpdateVisuals(); } }

        [SerializeField]
        private bool m_WholeNumbers = false;
        public bool wholeNumbers { get { return m_WholeNumbers; } set { if (_SetStruct(ref m_WholeNumbers, value)) { _Switch(m_States,false); UpdateVisuals(); } } }

        [SerializeField]
        protected bool m_States;
        public virtual bool states
        {
            get
            {
                return m_States;
            }
            set
            {
                _Switch(value,true);
            }
        }

        private float m_MinValue = 0;
        private float m_MaxValue = 1;
        [SerializeField]
        private float m_CurValue = 0.0f;

        [Space]

        // Allow for delegate-based subscriptions for faster events than 'eventReceiver', and allowing for multiple receivers.
        [SerializeField]
        private SwitchEvent m_OnValueChanged = new SwitchEvent();
        public SwitchEvent onValueChanged { get { return m_OnValueChanged; } set { m_OnValueChanged = value; } }

        // Private fields
        private Image m_FillImage;
        private Transform m_FillTransform;
        private RectTransform m_FillContainerRect;
        private Transform m_ButtonTransform;
        private RectTransform m_ButtonContainerRect;

        // The offset from button position to mouse down position
        private Vector2 m_Offset = Vector2.zero;

        private DrivenRectTransformTracker m_Tracker;

        protected GeUISwitchButton()
        { }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            //Onvalidate is called before OnEnabled. We need to make sure not to touch any other objects before OnEnable is run.
            if (IsActive())
            {
                UpdateCachedReferences();
                _Switch(m_States, true);
                // Update rects since other things might affect them even if value didn't change.
                UpdateVisuals();
            }

            var prefabType = UnityEditor.PrefabUtility.GetPrefabType(this);
            if (prefabType != UnityEditor.PrefabType.Prefab && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

#endif // if UNITY_EDITOR

        public virtual void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
        if (executing == CanvasUpdate.Prelayout)
            onValueChanged.Invoke(states);
#endif
        }

        public virtual void LayoutComplete()
        { }

        public virtual void GraphicUpdateComplete()
        { }

        protected override void OnEnable()
        {
            base.OnEnable();

            UpdateCachedReferences();
            _Switch(m_States, true);
            // Update rects since they need to be initialized correctly.
            UpdateVisuals();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            base.OnDisable();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            // Has value changed? Various elements of the slider have the old normalisedValue assigned, we can use this to perform a comparison.
            // We also need to ensure the value stays within min/max.
            m_CurValue = ClampValue(m_CurValue);
            float oldNormalizedValue = m_CurValue;
            if (m_FillContainerRect != null)
            {
                if (m_FillImage != null && m_FillImage.type == Image.Type.Filled)
                    oldNormalizedValue = m_FillImage.fillAmount;
                else
                    oldNormalizedValue = (reverseValue ? 1 - m_FillRect.anchorMin[(int)axis] : m_FillRect.anchorMax[(int)axis]);
            }
            else if (m_ButtonContainerRect != null)
                oldNormalizedValue = (reverseValue ? 1 - m_HandleRect.anchorMin[(int)axis] : m_HandleRect.anchorMin[(int)axis]);

            UpdateVisuals();

            if (oldNormalizedValue != m_CurValue)
                onValueChanged.Invoke(m_States);
        }

        void UpdateCachedReferences()
        {
            if (m_FillRect)
            {
                m_FillTransform = m_FillRect.transform;
                m_FillImage = m_FillRect.GetComponent<Image>();
                if (m_FillTransform.parent != null)
                    m_FillContainerRect = m_FillTransform.parent.GetComponent<RectTransform>();
            }
            else
            {
                m_FillContainerRect = null;
                m_FillImage = null;
            }

            if (m_HandleRect)
            {
                m_ButtonTransform = m_HandleRect.transform;
                if (m_ButtonTransform.parent != null)
                    m_ButtonContainerRect = m_ButtonTransform.parent.GetComponent<RectTransform>();
            }
            else
            {
                m_ButtonContainerRect = null;
            }
        }

        float ClampValue(float input)
        {
            float newValue = Mathf.Clamp(input, m_MinValue, m_MaxValue);
            return newValue;
        }

        // Set the valueUpdate the visible Image.
        public void Switch()
        {
            _Switch(!m_States, true);
        }

        public void SetSwitch(bool isOn)
        {
            _Switch(isOn, true);
        }

        public void RefreshUI()
        {
            Vector3 temp = transform.localPosition;
            temp.z += 0.01f;
            transform.localPosition = temp;
            temp.z -= 0.01f;
            transform.localPosition = temp;
            UpdateVisuals();
        }

        protected virtual void _Switch(bool input, bool sendCallback = true)
        {
            bool newValue = input;
            // If the stepped value doesn't match the last one, it's time to update
            if (m_States == newValue)
                return;

            m_States = newValue;

            if(true == m_States)
            {
                if (null != m_HandleImage)
                    m_HandleImage.sprite = m_OnImage;
                if (null != m_HandleText)
                    m_HandleText.text = m_OnText;
            }
            else
            {
                if (null != m_HandleImage)
                    m_HandleImage.sprite = m_OffImage;
                if (null != m_HandleText)
                    m_HandleText.text = m_OffText;
            }

            float values = m_States ? 1.0f : 0.0f;
            _SetValue(values);

            if (sendCallback)
                m_OnValueChanged.Invoke(newValue);
        }

        protected void _SetValue(float value)
        {
            if(value!=m_CurValue)
            {
                m_CurValue = value;
            }
            UpdateVisuals();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            //This can be invoked before OnEnabled is called. So we shouldn't be accessing other objects, before OnEnable is called.
            if (!IsActive())
                return;

            UpdateVisuals();
        }

        enum Axis
        {
            Horizontal = 0,
            Vertical = 1
        }

        Axis axis { get { return (m_Direction == Direction.LeftToRight || m_Direction == Direction.RightToLeft) ? Axis.Horizontal : Axis.Vertical; } }
        bool reverseValue { get { return m_Direction == Direction.RightToLeft || m_Direction == Direction.TopToBottom; } }

        // Force-update the slider. Useful if you've changed the properties and want it to update visually.
        private void UpdateVisuals()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UpdateCachedReferences();
#endif

            m_Tracker.Clear();

            if (m_FillContainerRect != null)
            {
                m_Tracker.Add(this, m_FillRect, DrivenTransformProperties.Anchors);
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;

                if (m_FillImage != null && m_FillImage.type == Image.Type.Filled)
                {
                    m_FillImage.fillAmount = m_CurValue;
                }
                else
                {
                    if (reverseValue)
                        anchorMin[(int)axis] = 1 - m_CurValue;
                    else
                        anchorMax[(int)axis] = m_CurValue;
                }

                m_FillRect.anchorMin = anchorMin;
                m_FillRect.anchorMax = anchorMax;
            }

            if (m_ButtonContainerRect != null)
            {
                m_Tracker.Add(this, m_HandleRect, DrivenTransformProperties.Anchors);
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;
                anchorMin[(int)axis] = anchorMax[(int)axis] = (reverseValue ? (1 - m_CurValue) : m_CurValue);
                m_HandleRect.anchorMin = anchorMin;
                m_HandleRect.anchorMax = anchorMax;
            }
        }

        // Update the slider's position based on the mouse.
        void UpdateDrag(PointerEventData eventData, Camera cam)
        {
            RectTransform clickRect = m_ButtonContainerRect ?? m_FillContainerRect;
            if (clickRect != null && clickRect.rect.size[(int)axis] > 0)
            {
                Vector2 localCursor;
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, eventData.position, cam, out localCursor))
                    return;
                localCursor -= clickRect.rect.position;

                float val = Mathf.Clamp01((localCursor - m_Offset)[(int)axis] / clickRect.rect.size[(int)axis]);
                m_CurValue = (reverseValue ? 1f - val : val);
            }
        }

        private bool MayDrag(PointerEventData eventData)
        {
            return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            base.OnPointerDown(eventData);

            Switch();

            m_Offset = Vector2.zero;
            if (m_ButtonContainerRect != null && RectTransformUtility.RectangleContainsScreenPoint(m_HandleRect, eventData.position, eventData.enterEventCamera))
            {
                Vector2 localMousePos;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_HandleRect, eventData.position, eventData.pressEventCamera, out localMousePos))
                    m_Offset = localMousePos;
            }
        }

        public override Selectable FindSelectableOnLeft()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Horizontal)
                return null;
            return base.FindSelectableOnLeft();
        }

        public override Selectable FindSelectableOnRight()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Horizontal)
                return null;
            return base.FindSelectableOnRight();
        }

        public override Selectable FindSelectableOnUp()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Vertical)
                return null;
            return base.FindSelectableOnUp();
        }

        public override Selectable FindSelectableOnDown()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Vertical)
                return null;
            return base.FindSelectableOnDown();
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        public void SetDirection(Direction direction, bool includeRectLayouts)
        {
            Axis oldAxis = axis;
            bool oldReverse = reverseValue;
            this.direction = direction;

            if (!includeRectLayouts)
                return;

            if (axis != oldAxis)
                RectTransformUtility.FlipLayoutAxes(transform as RectTransform, true, true);

            if (reverseValue != oldReverse)
                RectTransformUtility.FlipLayoutOnAxis(transform as RectTransform, (int)axis, true, true);
        }

        protected static bool _SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (currentValue.Equals(newValue))
                return false;

            currentValue = newValue;
            return true;
        }

        protected static bool _SetClass<T>(ref T currentValue, T newValue) where T : class
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return false;

            currentValue = newValue;
            return true;
        }
    }
}

