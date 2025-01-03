using System.Collections.Generic;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class DungeionDropItem : ActivityItemBase
    {
      
        [SerializeField] private Text mTextDesc;
        [SerializeField] private Text mTextTitle;
        [SerializeField] private Button mButtonChallenge;


        [SerializeField]
        private ComChapterInfoDrop mReviewDrop;

        private List<int> mReviewList = new List<int>();

        protected override void OnInit(ILimitTimeActivityTaskDataModel data)
        {
            if (data != null)
            {
                mTextDesc.SafeSetText(data.Desc);
                _InitAwards(data.AwardDataList);
            }
            mButtonChallenge.SafeRemoveAllListener();
            mButtonChallenge.SafeAddOnClickListener(_OnItemClick);
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
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            mReviewList.Clear();
            mButtonChallenge.SafeRemoveOnClickListener(_OnItemClick);
        }

        void _InitAwards(List<OpTaskReward> awardDataList)
        {
            if (awardDataList == null)
            {
                return;
            }
            mReviewList.Clear();
            for (int i = 0; i < awardDataList.Count; ++i)
            {
                mReviewList.Add((int)awardDataList[i].id);
            }
            if(mReviewDrop!=null)
            {
                mReviewDrop.SetDropList(mReviewList,0);
            }

          
        }
    }
}
