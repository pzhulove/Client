using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SevenDaysGiftView : MonoBehaviour
    {
        [SerializeField] private ComUIListScript mComUIListPage = null;
        [SerializeField] private ComUIListScript mComUIListAward = null;
        [SerializeField] private UIGray mGrayGetAward = null;
        [SerializeField] private TextEx mTextCost = null;
        [SerializeField] private ImageEx mImageIconCost = null;
        [SerializeField] private TextEx mTextName = null;
        [SerializeField] private TextEx mTextBought = null;
        [SerializeField] private CanvasGroup mCanvasCost = null;
        [SerializeField] private CanvasGroup mCanvasLeft = null;
        [SerializeField] private CanvasGroup mCanvasRight = null;
        [SerializeField] private Vector2 mFirstDayGiftSize = new Vector2(240, 282);
        [SerializeField] private Vector2 mOtherDayGiftSize = new Vector2(220, 282);

        private List<SevenDaysGiftData> mSevenDaysGiftDatas = null;
        private List<SevenDaysGiftData> mSevenDaysGiftFinishedDatas = null;
        private List<ItemData> mAwards = null;
        private List<Vector2> mAwardSizes = null;
        private int mCurSelectDay = -1;
        private int mCurSelectPackIndex = -1;
        private SevenDaysFrame mSevenDaysFrame;
        private bool mInited = false;

        public void Init(SevenDaysFrame sevenDaysFrame)
        {
            if (!mInited)
            {
                if (mComUIListPage != null)
                {
                    mComUIListPage.Initialize();
                    mComUIListPage.onItemVisiable = _OnTabUpdate;
                    mComUIListPage.OnItemUpdate = _OnTabUpdate;
                }

                if (mComUIListAward != null)
                {
                    mComUIListAward.Initialize();
                    mComUIListAward.onItemVisiable = _OnAwardUpdate;
                    mComUIListAward.OnItemUpdate = _OnAwardUpdate;
                    mComUIListAward.onItemChageDisplay = _OnAwardChangeDisplay;
                    mComUIListAward.onItemSelected = _OnAwardSelect;
                }

                mSevenDaysFrame = sevenDaysFrame;

                mInited = true;
            }

            mSevenDaysGiftDatas = SevendaysDataManager.GetInstance().GetSevenDaysGiftDatas();

            if (mCurSelectDay <= 0)
            {
                int unLockDay = SevendaysDataManager.GetInstance().GetMinUnlockGiftDays();
                mCurSelectDay = unLockDay <= 0 ? 1 : unLockDay;
            }

            _UpdatePage();

            if (mComUIListPage != null)
            {
                mSevenDaysGiftFinishedDatas = SevendaysDataManager.GetInstance().GetSevenDaysGiftFinishedDatas();
                if (mSevenDaysGiftFinishedDatas != null)
                {
                    mComUIListPage.SetElementAmount(mSevenDaysGiftFinishedDatas.Count);
                }
            }
        }

        public void UpdateGiftAward(int giftId, List<ItemData> itemDatas)
        {
            if (itemDatas == null)
            {
                return;
            }

            if (mCurSelectDay == 1)     //第一天就展示礼包，不展示礼包内的东西（因为点击礼包出tips也会返回该条协议触发这个函数，过滤一下）
            {
                return;
            }

            SevenDaysGiftData sevenDaysGiftData = mSevenDaysGiftDatas[mCurSelectDay - 1];
            if (sevenDaysGiftData != null && sevenDaysGiftData.itemDatas != null
                && sevenDaysGiftData.itemDatas.Count > 0 && sevenDaysGiftData.itemDatas[0].TableID == giftId)
            {
                if (mComUIListAward != null)
                {
                    mAwards = itemDatas;
                    _UpdateSizeList(false, itemDatas.Count);
                    mComUIListAward.SetElementAmount(itemDatas.Count, mAwardSizes);
                }
            }
        }

        private void _UpdateSizeList(bool isFirstDay, int count)
        {
            if (mAwardSizes == null)
            {
                mAwardSizes = new List<Vector2>();
            }
            else
            {
                mAwardSizes.Clear();
            }

            for (int i = 0; i < count; i++)
            {
                if (isFirstDay)
                {
                    mAwardSizes.Add(mFirstDayGiftSize);
                }
                else
                {
                    mAwardSizes.Add(mOtherDayGiftSize);
                }
            }
        }

        private void _UpdatePage()
        {
            if (mSevenDaysGiftDatas == null)
            {
                return;
            }

            if (mSevenDaysGiftDatas.Count < mCurSelectDay || mCurSelectDay <= 0)
            {
                return;
            }

            mCanvasLeft.CustomActive(mCurSelectDay > 1);
            mCanvasRight.CustomActive(mCurSelectDay < SevendaysDataManager.MaxDay);

            SevenDaysGiftData data = mSevenDaysGiftDatas[mCurSelectDay - 1];
            if (data != null && data.itemDatas != null && data.itemDatas.Count > 0)
            {
                if (mCurSelectDay == 1 )
                {
                    if (mComUIListAward != null)
                    {
                        mAwards = data.itemDatas;
                        _UpdateSizeList(true, data.itemDatas.Count);
                        mComUIListAward.SetElementAmount(data.itemDatas.Count, mAwardSizes);
                        if (mCurSelectPackIndex >= 0)
                        {
                            mComUIListAward.SelectElement(mCurSelectPackIndex);
                        }
                    }
                }
                else if (mCurSelectDay > 1)
                {
                    GiftPackDataManager.GetInstance().GetGiftPackItem(data.itemDatas[0].TableID);
                }

                mTextCost.SafeSetText(data.nowCost);
                mImageIconCost.SafeSetImage(data.nowCostIconPath);
                mTextName.SafeSetText(data.desc);

                bool isFirstDayGray = mCurSelectDay == 1 && mCurSelectPackIndex < 0;
                mTextBought.CustomActive(data.taskStatus == Protocol.TaskStatus.TASK_OVER);
                mCanvasCost.CustomActive(data.taskStatus != Protocol.TaskStatus.TASK_OVER);
                mGrayGetAward.SetEnable(data.taskStatus != Protocol.TaskStatus.TASK_FINISHED || isFirstDayGray);
            }
        }

        private void _OnTabUpdate(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            if (mSevenDaysGiftFinishedDatas == null || mSevenDaysGiftFinishedDatas.Count <= item.m_index)
            {
                return;
            }

            TextEx day = item.GetComponentInChildren<TextEx>();
            if (day != null)
            {
                day.SafeSetText(mSevenDaysGiftFinishedDatas[item.m_index].day.ToString());
            }
        }

        private void _OnAwardChangeDisplay(ComUIListElementScript item, bool isSelect)
        {
            if (item == null)
            {
                return;
            }

            if (mCurSelectDay != 1)
            {
                return;
            }

            if (mSevenDaysGiftDatas == null || mSevenDaysGiftDatas.Count < 1 || mSevenDaysGiftDatas[mCurSelectDay - 1] == null)
            {
                return;
            }

            SevenDaysGiftData data = mSevenDaysGiftDatas[mCurSelectDay - 1];
            bool ispack = data.taskStatus != Protocol.TaskStatus.TASK_OVER;
            if (!ispack)
            {
                return;
            }
           
            SevenDaysGiftAwardItem award = item.GetComponent<SevenDaysGiftAwardItem>();
            if (award != null)
            {
                award.Init(mAwards[item.m_index], true, isSelect);
            }
        }

        private void _OnAwardSelect(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            if (mCurSelectDay != 1)
            {
                return;
            }

            mCurSelectPackIndex = item.m_index;

            if (mCurSelectDay == 1)
            {
                bool isFirstDayGray = mCurSelectDay == 1 && mCurSelectPackIndex < 0;
                SevenDaysGiftData data = mSevenDaysGiftDatas[mCurSelectDay - 1];
                if (data != null)
                {
                    mTextBought.CustomActive(data.taskStatus == Protocol.TaskStatus.TASK_OVER);
                    mCanvasCost.CustomActive(data.taskStatus != Protocol.TaskStatus.TASK_OVER);
                    mGrayGetAward.SetEnable(data.taskStatus != Protocol.TaskStatus.TASK_FINISHED || isFirstDayGray);
                }
            }
        }

        private void _OnAwardUpdate(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            if (mSevenDaysGiftDatas == null || mSevenDaysGiftDatas.Count < mCurSelectDay
                || mCurSelectDay <= 0 || mSevenDaysGiftDatas[mCurSelectDay - 1] == null)
            {
                return;
            }


            if (mAwards == null || mAwards.Count <= item.m_index)
            {
                return;
            }

            SevenDaysGiftAwardItem award = item.GetComponent<SevenDaysGiftAwardItem>();
            if (award != null)
            {
                SevenDaysGiftData data = mSevenDaysGiftDatas[mCurSelectDay - 1];
                bool ispack = mCurSelectDay == 1 && data.taskStatus != Protocol.TaskStatus.TASK_OVER;
                award.Init(mAwards[item.m_index], ispack, mCurSelectPackIndex == item.m_index);
            }
        }

        public void BtnArrowClick(bool isLeft)
        {
            if (isLeft)
            {
                mCurSelectDay = mCurSelectDay <= 1 ? 1 : mCurSelectDay - 1;
            }
            else
            {
                mCurSelectDay = mCurSelectDay >= SevendaysDataManager.MaxDay ? SevendaysDataManager.MaxDay : mCurSelectDay + 1;
            }

            _UpdatePage();
        }

        public void BtnBuyClick()
        {
            if (mSevenDaysGiftDatas == null || mSevenDaysGiftDatas.Count < mCurSelectDay || mCurSelectDay <= 0 || mSevenDaysFrame == null)
            {
                return;
            }

            SevenDaysGiftData data = mSevenDaysGiftDatas[mCurSelectDay - 1];
            if (data == null || data.taskStatus != Protocol.TaskStatus.TASK_FINISHED)
            {
                return;
            }

            if (mCurSelectDay == 1)
            {
                if (mCurSelectPackIndex < 0)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("seven_day_unselect_gift"));
                }
                else
                {
                    mSevenDaysFrame.SubmitActive(data.activeId, mCurSelectPackIndex);
                }
            }
            else
            {
                mSevenDaysFrame.SubmitActive(data.activeId);
            }
        }
    }
}
