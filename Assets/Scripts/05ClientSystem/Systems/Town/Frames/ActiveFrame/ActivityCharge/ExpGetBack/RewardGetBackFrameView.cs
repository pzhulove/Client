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
    public class RewardGetBackFrameView : MonoBehaviour,IDisposable
    {
        public delegate void OnTogglesValueChanged(int id, bool isOn);
        public delegate void OnBtnClicked();
        [SerializeField] private Text mTextTitle;
        [SerializeField] private Text mTextTotalCount;
        [SerializeField] private Text mTextSelect;
        [SerializeField] private GameObject mRewardRoot2;
        [SerializeField] private Text mTextReward1;
        [SerializeField] private Text mTextReward2;
        [SerializeField] private Text mTextCost;
        [SerializeField] private Image mImageRewardIcon1;
        [SerializeField] private Image mImageRewardIcon2;
        [SerializeField] private Image mImageCostIcon;
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
        [SerializeField] private int mStep = 1;
        [SerializeField] private float mSliderWidth;

        private Action<int> mOnConfrimClicked;
        private Action mOnCloseClicked;
        private int mCount;
        private int mRewardPerFagitue;
        private int mCurrent;
        private int mCostNum;
        private IList<int> mRewardNumList;

        public void Init(string title, int count, IList<int> rewardIdList, IList<int> rewardNumList, int costId, int costNum, Action<int> onConfrimClick, Action onCloseClick)
        {
            mRewardNumList = rewardNumList;
            mOnConfrimClicked = onConfrimClick;
            mOnCloseClicked = onCloseClick;
            mCount = count;
            mCostNum = costNum;
            mTextTotalCount.SafeSetText(mCount.ToString());
            mTextTitle.SafeSetText(title);
            mSlider.maxValue = (float)count / mStep;
            mSlider.minValue = 1f;
            mSlider.value = 1f;
            var costItem = TableManager.GetInstance().GetTableItem<ItemTable>(costId);
            if (costItem != null)
            {
                ETCImageLoader.LoadSprite(ref mImageCostIcon, costItem.Icon);
            }

            if (rewardIdList != null && rewardNumList != null && rewardNumList.Count == rewardIdList.Count)
            {
                var talbelData = TableManager.GetInstance().GetTableItem<ItemTable>(rewardIdList[0]);
                if (talbelData != null)
                {
                    ETCImageLoader.LoadSprite(ref mImageRewardIcon1, talbelData.Icon);
                }
                if (rewardIdList.Count > 1)
                {
                    talbelData = TableManager.GetInstance().GetTableItem<ItemTable>(rewardIdList[1]);
                    mRewardRoot2.CustomActive(true);
                    if (talbelData != null)
                    {
                        ETCImageLoader.LoadSprite(ref mImageRewardIcon2, talbelData.Icon);
                    }
                }
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
            mOnConfrimClicked?.Invoke(mCurrent);
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
            mCurrent = Mathf.Min((int)(value * mStep), mCount);
            mTextSelect.SafeSetText(mCurrent.ToString());
            if (mRewardNumList != null)
            {
                if (mRewardNumList.Count > 0)
                {
                    mTextReward1.SafeSetText((mCurrent * mRewardNumList[0]).ToString());
                    if (mRewardNumList.Count > 1)
                    {
                        mTextReward2.SafeSetText((mCurrent * mRewardNumList[1]).ToString());
                    }
                }
            }
            mTextCost.SafeSetText((mCostNum * mCurrent).ToString());

            mButtonMax.enabled = mCurrent < mCount;
            mButtonAdd.enabled = mCurrent < mCount;
            mButtonMin.enabled = mCurrent > mStep;
            mButtonMinus.enabled = mCurrent > mStep;

            mMaxGray.SetEnable(!mButtonMax.enabled);
            mAddGray.SetEnable(!mButtonAdd.enabled);
            mMinGray.SetEnable(!mButtonMin.enabled);
            mMinusGray.SetEnable(!mButtonMinus.enabled);

            var size = mImageSlider.rectTransform.sizeDelta;
            size.x = mSliderWidth * (mCurrent - mStep) / (mCount - mStep);
            mImageSlider.rectTransform.sizeDelta = size;
        }
    }
}
