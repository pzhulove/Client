using System.Collections.Generic;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ReturnTeamBuffItem : ActivityItemBase
    {
        [SerializeField] private ScrollRect mAwardsScrollRect;
        [SerializeField] private GameObject mAwardRoot;
        [SerializeField] private Text mTextDesc;
        [SerializeField] private Text mTextTitle;
        [SerializeField] private Button mButtonChallenge;
        [SerializeField] private Button mButtonChallengeTeam;
        [SerializeField] private int mScrollCount = 5;//超过多少时才能滑动
        [SerializeField] Vector2 mComItemSize = new Vector2(95, 95);
        private const int DEFAULT_LIST_SIZE = 4;
        readonly List<ComItem> mComItems = new List<ComItem>(DEFAULT_LIST_SIZE);
        private int goFrameParam = 0;//等于0去深渊界面 ，1去组队界面

        protected override void OnInit(ILimitTimeActivityTaskDataModel data)
        {
            if (data != null)
            {
                mTextDesc.SafeSetText(data.Desc);
                _InitAwards(data.AwardDataList);
            }
            mButtonChallenge.SafeRemoveAllListener();
            mButtonChallenge.SafeAddOnClickListener(_OnItemClick);
            mButtonChallengeTeam.SafeRemoveAllListener();
            mButtonChallengeTeam.SafeAddOnClickListener(_OnItemClick);
        }
        protected override void _OnItemClick()
        {
            if (mOnItemClick != null)
            {
                mOnItemClick((int)mId, goFrameParam, 0);
            }
        }
        public override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            if (data != null)
            {
                mTextDesc.SafeSetText(data.Desc);
                int id = (int)data.ParamNums[0];
                var mAcquiredMethodTable = TableManager.GetInstance().GetTableItem<AcquiredMethodTable>(id);
                if (mAcquiredMethodTable == null)
                {
                    return;
                }

                mTextTitle.SafeSetText(mAcquiredMethodTable.Name);

                mButtonChallenge.CustomActive(false);
                mButtonChallengeTeam.CustomActive(false);
                if (data.ParamNums.Count > 1)
                {
                    if((int)data.ParamNums[1] == 0)
                    {
                        mButtonChallenge.CustomActive(true);
                        goFrameParam = 0;
                    }
                    if ((int)data.ParamNums[1] == 1)
                    {
                        mButtonChallengeTeam.CustomActive(true);
                        goFrameParam = 1;
                    }
                }
            }
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
            mButtonChallenge.SafeRemoveOnClickListener(_OnItemClick);
            mButtonChallengeTeam.SafeRemoveOnClickListener(_OnItemClick);
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
