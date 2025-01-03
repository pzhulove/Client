using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class ComDoubleClick : Selectable, IPointerClickHandler
    {
        [SerializeField] private int mClickFrameInterval = 8;
        [SerializeField] private UnityEvent mOnDoubleClick = new UnityEvent();
        [SerializeField] private UnityEvent mOnClick = new UnityEvent();

        private int mClickFrame = 0;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
            {
                return;
            }

            if (mClickFrame != 0)
            {
                if (Time.frameCount - mClickFrame <= mClickFrameInterval)
                {
                    mClickFrame = 0;
                    if (mOnDoubleClick != null)
                    {
                        DoubleClickInvoke();
                    }
                    return;
                }
            }

            mClickFrame = Time.frameCount;
        }

        public UnityEvent onClick
        {
            get { return mOnClick; }
            set { mOnClick = value; }
        }

        public UnityEvent onDoubleClick
        {
            get { return mOnDoubleClick; }
            set { mOnDoubleClick = value; }
        }

        private void DoubleClickInvoke()
        {
            if (!IsActive() || !IsInteractable())
            {
                return;
            }

            UISystemProfilerApi.AddMarker("DoubleClick", this);
            if (mOnDoubleClick != null)
            {
                mOnDoubleClick.Invoke();
            }
        }

        private void ClickInvoke()
        {
            if (!IsActive() || !IsInteractable())
            {
                return;
            }

            UISystemProfilerApi.AddMarker("DoubleClick", this);
            if (mOnClick != null)
            {
                mOnClick.Invoke();
            }
        }

        protected override void Awake()
        {

        }

        // Start is called before the first frame update
        protected override void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!IsActive() || !IsInteractable())
            {
                return;
            }

            if (mOnClick == null)
            {
                return;
            }

            if (mClickFrame != 0)
            {
                if (Time.frameCount - mClickFrame >= mClickFrameInterval)
                {
                    mClickFrame = 0;
                    ClickInvoke();
                }
            }
        }
    }
}