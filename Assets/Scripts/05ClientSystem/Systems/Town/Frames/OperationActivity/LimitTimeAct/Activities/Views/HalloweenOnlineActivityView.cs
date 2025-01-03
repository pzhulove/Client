using ProtoTable;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class HalloweenOnlineActivityView : LimitTimeActivityViewCommon
    {
        [SerializeField]
        private Text mTextLotteryCount;
        [SerializeField]
        private ActiveUpdate mActiveUpdate;


        [SerializeField]
        private Text mTimeTxt;
        [SerializeField]
        private Text mRuleTxt;

        [SerializeField]
        private Text mTotalDaysTxt;
       
        [SerializeField]
        private GameObject mPetItemRoot;
        [SerializeField]
        private Text mAccountLimitTxt;
        [SerializeField]
        private Button mReviewPetBtn;
        [SerializeField]
        private Button mGetPetRewardBtn;
        [SerializeField]
        private UIGray mGray;
        [SerializeField]
        Vector2 mComItemSize = new Vector2(100f, 100f);
        [SerializeField]
        private GameObject mActivitiyTurnTableRoot;

        private string mActivityTurnTablePath = "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/ActivityTurnTable";
        private ILimitTimeActivityTaskDataModel mPetTaskData;

        private bool mIsLeftMinus0 = false;


    

        public sealed override void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            mOnItemClick = onItemClick;
            mTextLotteryCount.SafeSetText(string.Format(TR.Value("HalloweenRollActivity_Lettety_Count"), CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_LOTTERY_ONLINE_TIME)));
            if (mActiveUpdate != null)
            {
                mActiveUpdate.SetTotlaNum((int)model.Param);
            }
            mTimeTxt.SafeSetText(string.Format("{0}~{1}",_TransTimeStampToStr(model.StartTime), _TransTimeStampToStr(model.EndTime)));
            mRuleTxt.SafeSetText(model.RuleDesc.Replace('|', '\n'));


            FindPetTaskData(model);


            if (mPetTaskData != null && mPetTaskData.AwardDataList.Count > 0)
            {
                //显示宠物蛋
                var comItem = ComItemManager.Create(mPetItemRoot);
                if (comItem != null)
                {
                    ItemData item = ItemDataManager.CreateItemDataFromTable((int)mPetTaskData.AwardDataList[0].id);
                    item.Count = (int)mPetTaskData.AwardDataList[0].num;
                    comItem.Setup(item, Utility.OnItemClicked);
                    (comItem.transform as RectTransform).sizeDelta = mComItemSize;
                }

                mTotalDaysTxt.SafeSetText(string.Format(TR.Value("HalloweenRollActivity_TotalDays"), mPetTaskData.DoneNum, mPetTaskData.TotalNum));
                ShowHaveUsedNumState();
            }

            //加载转盘
            if(mActivitiyTurnTableRoot!=null)
            {
                GameObject go= AssetLoader.GetInstance().LoadResAsGameObject(mActivityTurnTablePath);
                if (go != null)
                {
                    go.transform.SetParent(mActivitiyTurnTableRoot.transform);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localScale = Vector3.one;
                    ActivityTurnTable turnTable= go.GetComponent<ActivityTurnTable>();
                    if(turnTable!=null)
                    {
                        turnTable.Init((int)LotteryType.OnlineTimeLottert);
                    }
                }
                else
                {
                    Logger.LogError("加载活动转盘预制体出错");
                }
            }

            mGetPetRewardBtn.SafeAddOnClickListener(_OnGetRewardBtnClick);
            mReviewPetBtn.SafeAddOnClickListener(_OnReviewPetBtnClick);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivtiyLimitTimeAccounterNumUpdate, _OnActivityCounterUpdate);
            UpdateData(model);
        }

      

        public override void UpdateData(ILimitTimeActivityModel data)
        {
            if(!mIsLeftMinus0)
            {
                FindPetTaskData(data);
                if (mPetTaskData != null)
                {
                    switch (mPetTaskData.State)
                    {
                        case Protocol.OpActTaskState.OATS_INIT:
                        case Protocol.OpActTaskState.OATS_UNFINISH:
                            {
                                if(mGray!=null)
                                {
                                    mGray.enabled = true;
                                }
                                if(mGetPetRewardBtn != null)
                                {
                                    mGetPetRewardBtn.interactable = false;
                                }
                            }
                            break;
                        case Protocol.OpActTaskState.OATS_FINISHED:
                            {
                                if (mGray != null)
                                {
                                    mGray.enabled = false;
                                }
                                if (mGetPetRewardBtn != null)
                                {
                                    mGetPetRewardBtn.interactable = true;
                                }
                            }
                            break;
                        case Protocol.OpActTaskState.OATS_OVER:
                            {
                                if (mGray != null)
                                {
                                    mGray.enabled = true;
                                }
                                if (mGetPetRewardBtn != null)
                                {
                                    mGetPetRewardBtn.interactable = false;
                                }
                            }
                            break;
                    }
                }
            }
            else
            {
                if (mGray != null)
                {
                    mGray.enabled = true;
                }
            }
           

        }

        public override void Dispose()
        {
            base.Dispose();
            mIsLeftMinus0 = false;
            mPetTaskData = null;
            mGetPetRewardBtn.SafeRemoveOnClickListener(_OnGetRewardBtnClick);
            mReviewPetBtn.SafeRemoveOnClickListener(_OnReviewPetBtnClick);
        }


        public void UpdateLotteryCount()
        {
            mTextLotteryCount.SafeSetText(string.Format(TR.Value("HalloweenRollActivity_Lettety_Count"), CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_LOTTERY_ONLINE_TIME)));
        }

        /// <summary>
        /// 预览宠物
        /// </summary>
        private void _OnReviewPetBtnClick()
        {
            if (mPetTaskData!=null&&mPetTaskData.AwardDataList.Count > 0)
            {
                ItemData itemData = ItemDataManager.CreateItemDataFromTable((int)mPetTaskData.AwardDataList[0].id);
                if (itemData != null)
                {
                    PreViewItemData preViewItemData = new PreViewItemData();
                    preViewItemData.itemId = itemData.TableID;

                    PreViewDataModel preViewData = new PreViewDataModel();
                    preViewData.preViewItemList.Add(preViewItemData);

                    ClientSystemManager.GetInstance().OpenFrame<PreviewModelFrame>(FrameLayer.Middle, preViewData);
                }
            }
        }

        /// <summary>
        /// 获取宠物奖励
        /// </summary>
        private void _OnGetRewardBtnClick()
        {
            if(mPetTaskData!=null)
            {
                mOnItemClick((int)mPetTaskData.DataId,0,0);
                
               
                if (mPetTaskData.AccountDailySubmitLimit > 0)
                {
                    ActivityDataManager.GetInstance().SendSceneOpActivityGetCounterReq((int)mPetTaskData.DataId, ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_DAILY_SUBMIT_TASK);
                }
                if (mPetTaskData.AccountTotalSubmitLimit > 0)
                {
                    ActivityDataManager.GetInstance().SendSceneOpActivityGetCounterReq((int)mPetTaskData.DataId, ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_TOTAL_SUBMIT_TASK);
                }
            }
            
        }

        /// <summary>
        /// 找到有宠物蛋的那个任务
        /// </summary>
        private void FindPetTaskData(ILimitTimeActivityModel model)
        {
            int targetTaskId = 0;
            if(model!=null&&model.ParamArray!=null&&model.ParamArray.Length>0)
            {
                targetTaskId =(int)model.ParamArray[0];
            }
            for (int i = 0; i < model.TaskDatas.Count; i++)
            {
                ILimitTimeActivityTaskDataModel taskData = model.TaskDatas[i];
                if (taskData == null) continue;
                if(taskData.DataId==targetTaskId)
                {
                    mPetTaskData = taskData;
                    break;
                }
            }
        }


        private void _OnActivityCounterUpdate(UIEvent uiEvent)
        {
            if (mPetTaskData != null)
            {
                if ((uint)uiEvent.Param1 == mPetTaskData.DataId)
                {
                    ShowHaveUsedNumState();
                }

            }
        }

        /// <summary>
        /// 显示账号次数
        /// </summary>
        private void ShowHaveUsedNumState()
        {
            if (mPetTaskData != null)
            {
                int totalNum = 0;
                int haveNum = 0;

                if (mPetTaskData.AccountDailySubmitLimit > 0)
                {
                    haveNum = (int)ActivityDataManager.GetInstance().GetActivityConunter(mPetTaskData.DataId,
                        ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_DAILY_SUBMIT_TASK);
                    totalNum = mPetTaskData.AccountDailySubmitLimit;
                }
                else if (mPetTaskData.AccountTotalSubmitLimit > 0)
                {
                    haveNum = (int)ActivityDataManager.GetInstance().GetActivityConunter(mPetTaskData.DataId,
                       ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_TOTAL_SUBMIT_TASK);
                    totalNum = mPetTaskData.AccountTotalSubmitLimit;
                }

                int leftNum = totalNum - haveNum;
                if (leftNum <= 0&&totalNum>0)
                {
                    if (mGray != null)
                    {
                        mGray.enabled = true;
                    }
                    if (mGetPetRewardBtn != null)
                    {
                        mGetPetRewardBtn.interactable = false;
                    }
                    mIsLeftMinus0 = true;
                    leftNum = 0;
                }
                mAccountLimitTxt.CustomActive(totalNum>0);
                mAccountLimitTxt.SafeSetText(string.Format(TR.Value("HalloweenRollActivity_AccountTip"), leftNum, totalNum));



            }
        }


        private string _TransTimeStampToStr(UInt32 timeStamp)
        {
            System.DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime dt = time.AddSeconds(timeStamp);// unix 总秒数
            return string.Format("{0}年{1}月{2}日{3:HH:mm}", dt.Year, dt.Month, dt.Day, dt);
        }
    }
}
