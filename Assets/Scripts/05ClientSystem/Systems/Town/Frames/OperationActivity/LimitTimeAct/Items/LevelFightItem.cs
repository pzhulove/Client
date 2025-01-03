using System.Collections.Generic;
using Protocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class LevelFightItem : ActivityItemBase
    {
        [SerializeField] private ScrollRect mAwardsScrollRect;
        [SerializeField] private GameObject mAwardRoot;
        [SerializeField] private Text mTextDesc;
        [SerializeField] private int mScrollCount = 5;//超过多少时才能滑动
        [SerializeField] Vector2 mComItemSize = new Vector2(95, 95);
        private const int DEFAULT_LIST_SIZE = 4;
        readonly List<ComItem> mComItems = new List<ComItem>(DEFAULT_LIST_SIZE);

        protected override void OnInit(ILimitTimeActivityTaskDataModel data)
        {
            mTextDesc.SafeSetText(data.Desc);
            _InitAwards(data.AwardDataList);
        }

        public override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            mTextDesc.SafeSetText(data.Desc);
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

            mAwardsScrollRect.enabled = awardDataList.Count > mScrollCount;
        }
    }
}
