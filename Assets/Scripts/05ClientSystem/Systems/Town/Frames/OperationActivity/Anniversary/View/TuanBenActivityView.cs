using Protocol;
using ProtoTable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TuanBenActivityView : MonoBehaviour, IActivityView
    {

        [SerializeField]
        private GameObject mHaveTakenGo;

        [SerializeField]
        private GameObject mNotTakenGo;

        [SerializeField]
        private GameObject mCanTakenGo;

        [SerializeField]
        private Button mTakeRewardBtn;

        [SerializeField]
        private Button mGoBtn;

        [SerializeField]
        private Text mTimeTxt;

        [SerializeField]
        private Text mRuleDesTxt;

      

        [SerializeField]
        private Text mTicketDesTxt;

        [SerializeField]
        private Transform mItemParent;

 

        [SerializeField]
        private ComChapterInfoDrop mReviewDrop;

         private List<int> mReviewList = new List<int>();

        private ActivityItemBase.OnActivityItemClick<int> mOnItemClick;
        private ILimitTimeActivityTaskDataModel mTaskData = null;
        public  void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            if (model.Id == 0)
            {
                Logger.LogError("LimitTimeActivityModel data is empty");
                return;
            }

           
            mOnItemClick = onItemClick;
            mTakeRewardBtn.SafeAddOnClickListener(_OnTakeRewardBtnClick);
            mGoBtn.SafeAddOnClickListener(_OnGoBtnClick);
        
            _ShowActivityDes(model);
            UpdateData(model);
            _InitItems();
        }

       

        public void UpdateData(ILimitTimeActivityModel data)
        {
            if (data.Id == 0 || data.TaskDatas == null || data.TaskDatas.Count<1)
            {
                return;
            }
            mHaveTakenGo.CustomActive(false);
            mNotTakenGo.CustomActive(false);
            mCanTakenGo.CustomActive(false);
            mTaskData = data.TaskDatas[0];
            if (mTaskData == null) return;
            switch (mTaskData.State)
            {
             
                case Protocol.OpActTaskState.OATS_UNFINISH:
                    mNotTakenGo.CustomActive(true);
                    break;
                case Protocol.OpActTaskState.OATS_FINISHED:
                    mCanTakenGo.CustomActive(true);
                    break;
                case Protocol.OpActTaskState.OATS_OVER:
                    mHaveTakenGo.CustomActive(true);
                    break;
              
            }

        }

        public  void Close()
        {
            mTaskData = null;
            mOnItemClick = null;
            mTakeRewardBtn.SafeRemoveOnClickListener(_OnTakeRewardBtnClick);
            mGoBtn.SafeRemoveOnClickListener(_OnGoBtnClick);
            Destroy(gameObject);
        }

        public void Show()
        {
            gameObject.CustomActive(true);
        }

        public void Hide()
        {
            gameObject.CustomActive(false);
        }

        public void Dispose()
        {

        }
        private void _OnTakeRewardBtnClick()
        {
            if (mOnItemClick != null&&mTaskData!=null)
            {
                mOnItemClick((int)mTaskData.DataId, 0, 0);
            }
        }


        private void _OnGoBtnClick()
        {
            bool isShowTeamDuplication = TeamDuplicationUtility.IsShowTeamDuplication();
            if(isShowTeamDuplication)
            {
                TeamDuplicationUtility.EnterInTeamDuplicationBuildScene();
                ClientSystemManager.GetInstance().CloseFrame<LimitTimeActivityFrame>();
            }
            else
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("TUANBEN_ACTIVITY_Tip"));
            }
          
        }

        private void _OnReviewBtnClick()
        {
            
        }

        private void _ShowActivityDes(ILimitTimeActivityModel model)
        {
            mTimeTxt.SafeSetText(string.Format("{0}~{1}", _TransTimeStampToStr(model.StartTime), _TransTimeStampToStr(model.EndTime)));
            mRuleDesTxt.SafeSetText(model.RuleDesc);

            if(mReviewDrop!=null)
            {
                mReviewList.Clear();
                mReviewList.Add((int)model.Param);
                mReviewDrop.SetDropList(mReviewList, 0);
            }
            if (model.ParamArray2 != null && model.ParamArray2.Length >= 1)
            {
                mTicketDesTxt.SafeSetText(string.Format(TR.Value("TUANBEN_ACTIVITY_TICKET_DES"), model.ParamArray2[0]));
            }


        }

        private void _InitItems()
        {
            if (mTaskData != null)
            {
                if (mTaskData.AwardDataList != null && mTaskData.AwardDataList.Count >= 1)
                {
                    OpTaskReward taskReward = mTaskData.AwardDataList[0];
                    if (taskReward == null) return;
                    if (mItemParent == null) return;
                     ComItem comItem=   ComItemManager.Create(mItemParent.gameObject);
                    if (comItem == null) return;
                    ItemData itemData= ItemDataManager.CreateItemDataFromTable((int)taskReward.id);
                    if (itemData == null) return;
                    itemData.Count =(int)taskReward.num;
                    comItem.Setup(itemData, Utility.OnItemClicked);

                }
            }
        }

        private string _TransTimeStampToStr(UInt32 timeStamp)
        {
            System.DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime dt = time.AddSeconds(timeStamp);// unix 总秒数
            return string.Format("{0}月{1}日{2:HH:mm}", dt.Month, dt.Day, dt);
        }

        
    }
}
