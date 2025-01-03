using DG.Tweening;
using Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace GameClient
{
    public class PackagePreciousConfirmView : MonoBehaviour,IDisposable
    {
        public delegate void OnTogglesValueChanged(int id, bool isOn);
        public delegate void OnBtnClicked();
        [SerializeField] private Text mTextTitle;
        [SerializeField] private Text mTextTopTip;
        [SerializeField] private Text mTextConfirm;
        [SerializeField] private Text mTextCheck;
        [SerializeField] private Image mImageCheck;
        [SerializeField] private UIGray mGray;

        [SerializeField] private Button mComfirmBtn;
        [SerializeField] private Button mCloseBtn;
        [SerializeField] private UISelectableExtension mExtention;
        [SerializeField] private Button mButtonConfirm;
        [SerializeField] private ComUIListScript mComUIListPrecious;

        private Action mOnConfrimClicked;
        private Action mOnCloseClicked;
        private IList<ItemData> mPreciousItemList;
        private bool mIsCheck = false;

		public void Init(IList<ItemData> list, string title, string tip, string check, string confirm, Action onConfrimClick, Action onCloseClick)
        {
            mPreciousItemList = list;
            mComUIListPrecious.Initialize();
            mComUIListPrecious.onItemVisiable = _OnItemVisible;
            mComUIListPrecious.OnItemUpdate = _OnItemVisible;
            mTextTitle.SafeSetText(title);
            mTextTopTip.SafeSetText(tip);
            mTextConfirm.SafeSetText(confirm);
            mTextCheck.SafeSetText(check);
            mOnConfrimClicked = onConfrimClick;
            mOnCloseClicked = onCloseClick;
            mComUIListPrecious.SetElementAmount(list.Count);
            mGray.SetEnable(true);
            mExtention.enabled = false;
        }

        public void Dispose()
		{
			_UnRegisterEvent();
          
            mPreciousItemList?.Clear();
            mPreciousItemList = null;
        }

        public void OnConfirmClick()
        {
            mOnConfrimClicked?.Invoke();
        }

        public void OnCloseClick()
        {
            mOnCloseClicked?.Invoke();
        }

        public void OnCheckClick()
        {
            mIsCheck = !mIsCheck;
            mImageCheck.enabled = mIsCheck;
            mGray.SetEnable(!mIsCheck);
            mExtention.enabled = mIsCheck;
            mButtonConfirm.enabled = mIsCheck;
        }

        private void _ClosePanel()
        {
            mComUIListPrecious.SetElementAmount(0);
        }

		private void _UnRegisterEvent()
		{
            mComfirmBtn.SafeRemoveAllListener();
            mCloseBtn.SafeRemoveAllListener();
        }

        private void _OnItemVisible(ComUIListElementScript item)
        {
            if (item == null || item.m_index >= mPreciousItemList.Count)
                return;

            var script = item.GetComponentInChildren<ComItemNew>();
            if (script == null)
                return;
            var model = mPreciousItemList[item.m_index];

            var text = item.GetComponentInChildren<Text>();
            text.SafeSetText(model.GetColorName());
            script.Setup(model, null, true);

        }
	}

}
