using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GameClient
{
    public class ComPopPanel : MonoBehaviour
    {
        [SerializeField] private Button mButtonShow;
        [SerializeField] private Button mButtonHide;
        [SerializeField] private float mTotalSeconds;
        [SerializeField] private Transform mPanelRoot;
        [SerializeField] private Vector3 mShowPosition;
        [SerializeField] private Vector3 mHidePosition;
        private Tweener mHideTweener;
        private Tweener mShowTweener;
        void Start()
        {
            mButtonShow.SafeAddOnClickListener(_OnButtonShowClick);
            mButtonHide.SafeAddOnClickListener(_OnButtonHideClick);
        }

        void OnDestroy()
        {
            mButtonShow.SafeRemoveOnClickListener(_OnButtonShowClick);
            mButtonHide.SafeRemoveOnClickListener(_OnButtonHideClick);
        }

        void _OnButtonShowClick()
        {
            if (mShowTweener == null)
            {
                mShowTweener = mPanelRoot.DOLocalMove(mShowPosition, mTotalSeconds);
                mShowTweener.SetAutoKill(false);
                mShowTweener.OnComplete(_OnShowComplete);
            }
            else
            {
                mShowTweener.Restart();
                mButtonShow.enabled = false;
            }
        }

        void _OnShowComplete()
        {
            mButtonShow.enabled = true;
            mButtonShow.CustomActive(false);
            mButtonHide.CustomActive(true);
        }

        void _OnButtonHideClick()
        {
            if (mHideTweener == null)
            {
                mHideTweener = mPanelRoot.DOLocalMove(mHidePosition, mTotalSeconds);
                mHideTweener.OnComplete(_OnHideComplete);
                mHideTweener.SetAutoKill(false);
            }
            else
            {
                mHideTweener.Restart();
                mButtonHide.enabled = false;
            }

        }

        void _OnHideComplete()
        {
            mButtonHide.enabled = true;
            mButtonShow.CustomActive(true);
            mButtonHide.CustomActive(false);
        }
    }
}
