using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SpringFestivalRedEnvelopeRainActivityView : LimitTimeActivityViewCommon
    {
        [SerializeField] private Text mTime;
        [SerializeField] private Text mRule;
        [SerializeField] private Text mActive;
        [SerializeField] private string mActiveDesc = "今日活跃度:{0}";

        public sealed override void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            if (model == null)
            {
                Logger.LogError("LimitTimeActivityModel data is empty");
                return;
            }
            mModel = model;
            mOnItemClick = onItemClick;
            _InitItems(model);

            mTime.SafeSetText(string.Format("{0}~{1}", Function.GetDateTime((int)mModel.StartTime), Function.GetDateTime((int)mModel.EndTime)));
            mRule.SafeSetText(mModel.RuleDesc.Replace('|', '\n'));
            UpdateActiveInfo();

            MissionManager.GetInstance().onDailyScoreChanged += OnDailyScoreChanged;
        }

        private void UpdateActiveInfo()
        {
            mActive.SafeSetText(string.Format(mActiveDesc, MissionManager.GetInstance().Score));
        }

        private void OnDailyScoreChanged(int score)
        {
            UpdateActiveInfo();
        }

        public sealed override void Dispose()
        {
            base.Dispose();

            MissionManager.GetInstance().onDailyScoreChanged -= OnDailyScoreChanged;
        }
    }
}