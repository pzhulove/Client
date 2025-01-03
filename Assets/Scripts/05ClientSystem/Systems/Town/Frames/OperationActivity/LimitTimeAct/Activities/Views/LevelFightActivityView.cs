using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class LevelFightActivityView : LimitTimeActivityViewCommon
    {
        [SerializeField] private Text mTextMyRank;
        [SerializeField] private Text mTextTime;
        [SerializeField] private GameObject mPlayerNameTitle;
        [SerializeField] private Button mButtonGoTo;
        private int mEndTime = 0;

        public UnityAction OnButtonClick
        {
            set
            {
                mButtonGoTo.SafeRemoveAllListener();
                mButtonGoTo.SafeAddOnClickListener(value);
            }
        }

        public override void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            base.Init(model, onItemClick);
            mNote.ShowLogoText(false);
        }

        public void SetRank(int rank)
        {
            if (rank != 0)
            {
                mTextMyRank.text = string.Format(TR.Value("activity_level_fight_rank_succeeded"), rank);
            }
            else
            {
                mTextMyRank.text = string.Format(TR.Value("activity_level_fight_rank_failed"));
            }
        }

        public void ShowPlayerName(bool isShow)
        {
            mPlayerNameTitle.CustomActive(isShow);
        }

        public void SetEndTime(int endTime)
        {
            mEndTime = endTime;
            mTextMyRank.CustomActive(true);
            InvokeRepeating("_UpdateTime", 0, 1);
        }

        public void ShowResultText(int num)
        {
            mTextMyRank.CustomActive(true);
            mTextTime.SafeSetText(string.Format(TR.Value("activity_level_fight_show_end_time"), num));
        }

        public override void Dispose()
        {
            base.Dispose();
            mButtonGoTo.SafeRemoveAllListener();
        }

        void _UpdateTime()
        {
            if (mEndTime == 0)
            {
                CancelInvoke("_UpdateTime");
                return;
            }

            mTextTime.SafeSetText(string.Format(TR.Value("activity_level_fight_end_time"), Function.SetShowTime(mEndTime)));
        }

    }
}
