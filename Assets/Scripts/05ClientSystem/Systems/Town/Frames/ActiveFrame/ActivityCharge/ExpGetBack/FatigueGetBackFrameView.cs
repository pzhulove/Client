using DG.Tweening;
using ProtoTable;
using Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class FatigueGetBackFrameView : MonoBehaviour,IDisposable
    {
        public delegate void OnTogglesValueChanged(int id, bool isOn);
        public delegate void OnBtnClicked();
        [SerializeField] private Text mTextTotal;
        [SerializeField] private Text mTextVip;
        [SerializeField] private Text mTextSelect;
        [SerializeField] private Text mTextRewards;
        [SerializeField] private Text mTextCost;
        [SerializeField] private Image mImageIcon;
        [SerializeField] private Image mImageSlider;
        [SerializeField] private Slider mSlider;
        [SerializeField] private UIGray mMinGray;
        [SerializeField] private UIGray mMinusGray;
        [SerializeField] private UIGray mMaxGray;
        [SerializeField] private UIGray mAddGray;
        [SerializeField] private Button mButtonMin;
        [SerializeField] private Button mButtonMax;
        [SerializeField] private Button mButtonMinus;
        [SerializeField] private Button mButtonAdd;
        [SerializeField] private Button mComfirmBtn;
        [SerializeField] private Button mCloseBtn;
        [SerializeField] private int mStep = 25;
        [SerializeField] private float mSliderWidth;

        private Action<int> mOnConfrimClicked;
        private Action mOnCloseClicked;
        private Func<int, int> mGetCost;
        private int mMaxFatigue;
        private int mRewardPerFagitue;
        private int mCurrent;

        public void Init(int total, int vip, int vipCount, int maxFatigue, int rewardPerFagitue, int costId, int step, Action<int> onConfrimClick, Action onCloseClick, Func<int, int> getCost)
        {
            mMaxFatigue = maxFatigue;
            mRewardPerFagitue = rewardPerFagitue;
            mOnConfrimClicked = onConfrimClick;
            mOnCloseClicked = onCloseClick;
            mTextTotal.SafeSetText(total.ToString());
            mTextVip.SafeSetText(TR.Value("activity_fatigue_get_back_vip", vip, vipCount));
            mGetCost = getCost;
            if (step > 0)
            {
                mStep = step;
            }
            mSlider.maxValue = (float)mMaxFatigue / mStep;
            mSlider.minValue = 1f;
            mSlider.value = 1f;
            var costItem = TableManager.GetInstance().GetTableItem<ItemTable>(costId);
            if (costItem != null)
            {
                ETCImageLoader.LoadSprite(ref mImageIcon, costItem.Icon);
            }
        }

        public void Dispose()
		{
			_UnRegisterEvent();
          
        }

        public void OnValueChanged(float value)
        {
            _UpdateValue(value);
        }

        public void OnConfirmClick()
        {
            mOnConfrimClicked?.Invoke(mCurrent / mStep);
        }

        public void OnCloseClick()
        {
            mOnCloseClicked?.Invoke();
        }

        public void OnMinClick()
        {
            mSlider.value = mSlider.minValue;
        }

        public void OnMaxClick()
        {
            mSlider.value = mSlider.maxValue;
        }

        public void OnMinusClick()
        {
            mSlider.value -= 1f;
        }

        public void OnAddClick()
        {
            mSlider.value += 1f;
        }

        private void _UnRegisterEvent()
		{
            mComfirmBtn.SafeRemoveAllListener();
            mCloseBtn.SafeRemoveAllListener();
        }

        private void _UpdateValue(float value)
        {
            mCurrent = Mathf.Min((int)(value * mStep), mMaxFatigue);
            mTextSelect.SafeSetText(mCurrent.ToString());
            mTextRewards.SafeSetText((mCurrent * mRewardPerFagitue).ToString());
            mTextCost.SafeSetText((mGetCost?.Invoke(mCurrent)).ToString());

            mButtonMax.enabled = mCurrent < mMaxFatigue;
            mButtonAdd.enabled = mCurrent < mMaxFatigue;
            mButtonMin.enabled = mCurrent > mStep;
            mButtonMinus.enabled = mCurrent > mStep;

            mMaxGray.SetEnable(!mButtonMax.enabled);
            mAddGray.SetEnable(!mButtonAdd.enabled);
            mMinGray.SetEnable(!mButtonMin.enabled);
            mMinusGray.SetEnable(!mButtonMinus.enabled);

            var size = mImageSlider.rectTransform.sizeDelta;
            size.x = mSliderWidth * (mCurrent - mStep) / (mMaxFatigue - mStep);
            mImageSlider.rectTransform.sizeDelta = size;
        }
	}

}
