using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class NewYearOnDayItem : ActivityItemBase
    {
        [SerializeField]
        private Text mTaskDesTxt;
        [SerializeField]
        private Text mTaskProgressTxt;

        [SerializeField]
        private Button mReceiveBtn;
        [SerializeField]
        private GameObject mUnFinishGo;
        [SerializeField]
        private GameObject mHaveReceiveGo;

        [SerializeField]
        private GameObject mItemRootGo;
        [SerializeField]
        private Vector2 mComItemSize = new Vector2(100, 100);

        protected override void OnInit(ILimitTimeActivityTaskDataModel data)
        {
            mTaskDesTxt.SafeSetText(data.Desc);
            mTaskProgressTxt.SafeSetText(TR.Value("NewYearOnDay_TaskProgress",data.DoneNum,data.TotalNum));
            mReceiveBtn.SafeAddOnClickListener(_OnItemClick);
            if(data.AwardDataList!=null)
            {
                for (int i = 0; i < data.AwardDataList.Count; i++)
                {
                    _CreateItem(data.AwardDataList[i]);
                }
            }
           
        }


        public override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            switch (data.State)
            {
                case Protocol.OpActTaskState.OATS_INIT:
                case Protocol.OpActTaskState.OATS_UNFINISH:
                    mUnFinishGo.CustomActive(true);
                    mReceiveBtn.CustomActive(false);
                    mHaveReceiveGo.CustomActive(false);
                    break;
                case Protocol.OpActTaskState.OATS_FINISHED:
                    mReceiveBtn.CustomActive(true);
                    mUnFinishGo.CustomActive(false);
                    mHaveReceiveGo.CustomActive(false);
                    break;
                case Protocol.OpActTaskState.OATS_OVER:
                    mHaveReceiveGo.CustomActive(true);
                    mUnFinishGo.CustomActive(false);
                    mReceiveBtn.CustomActive(false);
                    break;
              
            }

            mTaskProgressTxt.SafeSetText(TR.Value("NewYearOnDay_TaskProgress", data.DoneNum, data.TotalNum));
        }
        public override void Dispose()
        {
            base.Dispose();
            mReceiveBtn.SafeRemoveOnClickListener(_OnItemClick);
        }

        private void _CreateItem(OpTaskReward opTaskReward)
        {
            if(opTaskReward!=null)
            {
                ComItem comItem = ComItemManager.Create(mItemRootGo);
                ItemData item = ItemDataManager.CreateItemDataFromTable((int)opTaskReward.id);
                if (comItem != null && item != null)
                {
                    comItem.GetComponent<RectTransform>().sizeDelta = mComItemSize;
                    item.Count = (int)opTaskReward.num;
                    comItem.Setup(item, Utility.OnItemClicked);
                }
            }
         
        }



    }
}
