using System.Collections.Generic;
using Protocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class LevelFightShowItem : ActivityItemBase
    {
        [SerializeField] private ScrollRect mAwardsScrollRect;
        [SerializeField] private GameObject mAwardRoot;
        [SerializeField] private GameObject mRank1;
        [SerializeField] private GameObject mRank2;
        [SerializeField] private GameObject mRank3;
        [SerializeField] private Text mTextDesc;
        [SerializeField] private Text mTextName;
        [SerializeField] private int mScrollCount = 5;//超过多少时才能滑动
        [SerializeField] Vector2 mComItemSize = new Vector2(95, 95);
        private const int DEFAULT_LIST_SIZE = 4;
        readonly List<ComItem> mComItems = new List<ComItem>(DEFAULT_LIST_SIZE);

        protected override void OnInit(ILimitTimeActivityTaskDataModel data)
        {
            _InitAwards(data.AwardDataList);
            _UpdateText(data as LevelFightActivityRankDataModel);
        }

        public override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            _UpdateText(data as LevelFightActivityRankDataModel);
        }

        public override void Dispose()
        {
            base.Dispose();
            if (mComItems != null)
            {
                for (int i = 0; i < mComItems.Count; ++i)
                {
                    ComItemManager.Destroy(mComItems[i]);
                }
                mComItems.Clear();
            }
        }

        void _UpdateText(LevelFightActivityRankDataModel rankData)
        {
            if (rankData == null)
            {
                return;
            }
            //前三名显示图片，后面显示文字
            if (rankData.Rank <= 3)
            {
                mTextDesc.CustomActive(false);
                switch (rankData.Rank)
                {
                    case 1:
                        mRank1.CustomActive(true);
                        mRank2.CustomActive(false);
                        mRank3.CustomActive(false);
                        break;
                    case 2:
                        mRank1.CustomActive(false);
                        mRank2.CustomActive(true);
                        mRank3.CustomActive(false);
                        break;
                    case 3:
                        mRank1.CustomActive(false);
                        mRank2.CustomActive(false);
                        mRank3.CustomActive(true);
                        break;
                }
            }
            else
            {
                mRank1.CustomActive(false);
                mRank2.CustomActive(false);
                mRank3.CustomActive(false);
                mTextDesc.CustomActive(true);
                mTextDesc.SafeSetText(string.Format(TR.Value("activity_level_fight_show_rank"),rankData.Rank));
            }
            mTextName.SafeSetText(rankData.Name);
        }

        void _InitAwards(List<OpTaskReward> awardDataList)
        {
            if (awardDataList == null)
            {
                return;
            }

            for (int i = 0; i < awardDataList.Count; ++i)
            {
                ComItem comItem = ComItemManager.Create(mAwardRoot);
                if (comItem != null)
                {
                    ItemData data = ItemDataManager.CreateItemDataFromTable((int)awardDataList[i].id);
                    data.Count = (int)awardDataList[i].num;
                    comItem.Setup(data, Utility.OnItemClicked);
                    (comItem.transform as RectTransform).sizeDelta = mComItemSize;
                    mComItems.Add(comItem);
                }
            }


            if (awardDataList.Count > mScrollCount)
            {
                mAwardsScrollRect.enabled = true;
            }
            else
            {
                mAwardsScrollRect.enabled = false;
            }
        }
    }
}
