using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using Protocol;
using System;
using Network;
using ProtoTable;

namespace GameClient
{
    public class SummerVacationWeeklyItem : ActivityItemBase
    {
        /// <summary>
        /// 标题
        /// </summary>
        [SerializeField]
        private Text mTitleTxt;
        /// <summary>
        /// 描述
        /// </summary>
        [SerializeField]
        private Text mDesTxt;
        [SerializeField]
        private Transform mRewardRoot;
        [SerializeField]
        Vector2 mComItemSize = new Vector2(100f, 100f);
        /// <summary>
        /// 等级不够要显示的文字
        /// </summary>
        [SerializeField]
        private Text mLvLockTxt;
        /// <summary>
        /// 本周次数已经用完的文字
        /// </summary>
        [SerializeField]
        private Text mNumHaveUsedTxt;
        /// <summary>
        /// 领取按钮
        /// </summary>
        [SerializeField]
        private Button mTakeTaskBtn;
        /// <summary>
        /// 领取奖励按钮
        /// </summary>
        [SerializeField]
        private Button mRewardBtn;
        /// <summary>
        /// 前往关卡按钮
        /// </summary>
        [SerializeField]
        private Button mGoToBtn;
        /// <summary>
        /// 获取奖励之后
        /// </summary>
        [SerializeField]
        private GameObject mRewardAfterGo;
        /// <summary>
        /// 任务进度条
        /// </summary>
        [SerializeField]
        private Slider mTaskProgressSlider;
        /// <summary>
        /// 任务进度文本
        /// </summary>
        [SerializeField]
        private Text mTaskProgressTxt;
        /// <summary>
        /// 任务奖励的集合
        /// </summary>
        private List<ComItem> mComItems = new List<ComItem>();
        /// <summary>
        /// 遮罩
        /// </summary>
        [SerializeField]
        private GameObject maskGo;

        private ILimitTimeActivityTaskDataModel mData;
        /// <summary>
        /// 总共可以领取的账号次数
        /// </summary>
        private int mTotalReceiveAcountNum = 0;
        /// <summary>
        ///账号可领取的次数
        /// </summary>
        private int mCanReceiveAccountNum =3;


        /// <summary>
        ///角色可领取的次数
        /// </summary>
        private int mCanReceiveRoleNum = 1;
        /// <summary>
        /// 角色总共可领取的次数
        /// </summary>
        private int mTotalReceiveRoleNum = 1;

      

        protected override void OnInit(ILimitTimeActivityTaskDataModel data)
        {
            if (data == null) return;
            mData = data;
            if (mDesTxt != null)
            {
                mDesTxt.text = data.Desc;
            }
            if (mTitleTxt != null)
            {
                mTitleTxt.text = data.taskName;
            }
            if (data.AwardDataList != null)
            {
                for (int i = 0; i < data.AwardDataList.Count; i++)
                {
                    if (mRewardRoot != null)
                    {
                        var comItem = ComItemManager.Create(mRewardRoot.gameObject);
                        if (comItem != null)
                        {
                            ItemData item = ItemDataManager.CreateItemDataFromTable((int)data.AwardDataList[i].id);
                            item.Count = (int)data.AwardDataList[i].num;
                            comItem.Setup(item, Utility.OnItemClicked);
                            mComItems.Add(comItem);
                            (comItem.transform as RectTransform).sizeDelta = mComItemSize;
                        }
                    }
                }

            }

            if (mTakeTaskBtn != null)
            {
                mTakeTaskBtn.SafeAddOnClickListener(OnTakeBtnClick);
            }
            if (mGoToBtn != null)
            {
                mGoToBtn.SafeAddOnClickListener(OnGoToDungeonClikc);
            }
            if(mRewardBtn!=null)
            {
                mRewardBtn.SafeAddOnClickListener(_OnItemClick);
            }
           
        }

     

        public override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            int ret =(int)ActivityDataManager.GetInstance().GetActivityConunter(ActivityLimitTimeFactory.EActivityType.OAT_SUMMER_WEEKLY_VACATION, ActivityLimitTimeFactory.EActivityCounterType.OAT_SUMMER_WEEKLY_VACATION);

            OpActivityData opActivityData = ActivityDataManager.GetInstance().GetActiveDataFromType(ActivityLimitTimeFactory.EActivityType.OAT_SUMMER_WEEKLY_VACATION);
            if (opActivityData != null)
            {
                if (opActivityData.parm2 != null && opActivityData.parm2.Length >= 2)
                {
                    mTotalReceiveAcountNum = (int)opActivityData.parm2[0];
                    mTotalReceiveRoleNum = (int)opActivityData.parm2[1];
                }
            }
            mCanReceiveAccountNum = mTotalReceiveAcountNum - ret;
            if (mCanReceiveAccountNum <= 0)
            {
                mCanReceiveAccountNum = 0;
            }
            List<uint> haveRecivedIdList = ActivityDataManager.GetInstance().GetHaveRecivedTaskID(ActivityLimitTimeFactory.EActivityType.OAT_SUMMER_WEEKLY_VACATION);
            if (haveRecivedIdList != null && haveRecivedIdList.Count > 0)
            {
                if (data != null)
                {
                    if (!haveRecivedIdList.Contains(data.DataId))
                    {
                        if(haveRecivedIdList.Count>= mTotalReceiveRoleNum)
                        {
                            OnlyShowUnTakeTask();
                            return;
                        }
                     
                    }
                }

            }
            switch (data.State)
            {

                case OpActTaskState.OATS_INIT:
                    if(PlayerBaseData.GetInstance().Level<data.PlayerLevelLimit)
                    {
                        OnlyShowLvLockTxt();
                    }
                    else
                    {
                        if(mCanReceiveAccountNum<=0)//可以领取的任务如果次数用完的时候
                        {
                            OnlyShowNumHaveUsedTxt();
                        }
                        else
                        {
                            OnlyShowTakeTaskBtn();
                            ShowTaskProgress();
                        }
                       
                    }
                  
                    break;
                case OpActTaskState.OATS_UNFINISH:
                    OnlyShowGoToBtn();
                    ShowTaskProgress();
                    break;
                case OpActTaskState.OATS_FINISHED:
                    OnlyRewardBtn();
                    ShowTaskProgress();
                    break;
                case OpActTaskState.OATS_OVER:
                    OnlyRewardAfter();
                    ShowTaskProgress();
                    break;
                default:
                    break;
            }
        

        }

        public override void Dispose()
        {
            base.Dispose();
            if (mComItems != null)
            {
                for (int i = this.mComItems.Count - 1; i >= 0; --i)
                {
                    ComItemManager.Destroy(mComItems[i]);
                }
                mComItems.Clear();
            }

            if (mTakeTaskBtn != null)
            {
                mTakeTaskBtn.SafeRemoveOnClickListener(OnTakeBtnClick);
            }
            if (mGoToBtn != null)
            {
                mGoToBtn.SafeRemoveOnClickListener(OnGoToDungeonClikc);
            }
            if(mRewardBtn!=null)
            {
                mRewardBtn.SafeRemoveOnClickListener(_OnItemClick);
            }
            mData = null;
        }

        /// <summary>
        /// 接收任务按钮的点击
        /// </summary>
        private void OnTakeBtnClick()
        {
            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                //TODO
                ContentLabel = string.Format(TR.Value("limitactivity_shuqi_conetnt"),3),
                IsShowNotify = false,
                LeftButtonText = TR.Value("limitactivity_shuqi_cancel"),
                RightButtonText = TR.Value("limitactivity_shuqi_ok"),
                OnRightButtonClickCallBack = OnOKBtnClick
            };
            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);
        }

        private void OnOKBtnClick()
        {
            SceneOpActivityAcceptTaskReq req = new SceneOpActivityAcceptTaskReq();
            //活动ID
            req.opActId =(uint)ActivityDataManager.GetInstance().GetActiveIdFromType(ActivityLimitTimeFactory.EActivityType.OAT_SUMMER_WEEKLY_VACATION);
            //任务ID
            if(mData!=null)
            {
                req.taskId = mData.DataId;
            }
            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, req);

            ActivityDataManager.GetInstance().SendSceneOpActivityGetCounterReq(ActivityLimitTimeFactory.EActivityType.OAT_SUMMER_WEEKLY_VACATION,  ActivityLimitTimeFactory.EActivityCounterType.OAT_SUMMER_WEEKLY_VACATION);
        }
        
        /// <summary>
        /// 前往地下城
        /// </summary>
        private void OnGoToDungeonClikc()
        {
          
            if (mData!=null)
            {
                if(mData.ParamNums2 != null&&mData.ParamNums2.Count > 0)
                {
                    DungeonModelTable.eType type= DungeonUtility.GetDugeonModleTypeById((int)mData.ParamNums2[0]);
                    if(type== DungeonModelTable.eType.Type_None)
                    {
                        ClientSystemTown clientSystem = ClientSystem.GetTargetSystem<ClientSystemTown>();
                        if (clientSystem != null)
                        {
                            clientSystem.MainPlayer.CommandMoveToDungeon((int)mData.ParamNums2[0]);
                        }
                    }
                    else 
                    {
                        ChallengeUtility.OnOpenChallengeMapFrame(type, 0);
                    }
                    ClientSystemManager.GetInstance().CloseFrame<LimitTimeActivityFrame>();
                }
                else
                {
                    //普通试练，打开推荐地下城
                    WeeklyCheckInActivityView.OnRecommendedDungeonsBtnClick();
                }
               
            }
           
        }

        /// <summary>
        /// 显示任务进度
        /// </summary>
        private void ShowTaskProgress()
        {
            if (mData == null) return;
            if (mTaskProgressSlider != null)
            {
                mTaskProgressSlider.value = (mData.DoneNum * 1.0f / mData.TotalNum * 1.0f);
            }
            if (mTaskProgressTxt != null)
            {
                mTaskProgressTxt.text = string.Format("{0}/{1}", mData.DoneNum, mData.TotalNum);
            }
        }
        /// <summary>
        /// 只显示 领取奖励之后
        /// </summary>
        private void OnlyRewardAfter()
        {
            mRewardAfterGo.CustomActive(true);

            mRewardBtn.CustomActive(false);
            mGoToBtn.CustomActive(false);
            maskGo.CustomActive(false);
            mTakeTaskBtn.CustomActive(false);
            mLvLockTxt.CustomActive(false);
            mNumHaveUsedTxt.CustomActive(false);

            mTaskProgressSlider.CustomActive(true);
            mTaskProgressSlider.CustomActive(true);

        }
        /// <summary>
        /// 只显示 按钮领取奖励
        /// </summary>
        private void OnlyRewardBtn()
        {
            mRewardBtn.CustomActive(true);

            mGoToBtn.CustomActive(false);
            maskGo.CustomActive(false);
            mTakeTaskBtn.CustomActive(false);
            mLvLockTxt.CustomActive(false);
            mNumHaveUsedTxt.CustomActive(false);
            mRewardAfterGo.CustomActive(false);

            mTaskProgressSlider.CustomActive(true);
            mTaskProgressSlider.CustomActive(true);
        }
        /// <summary>
        /// 只显示 立即前往按钮
        /// </summary>
        private void OnlyShowGoToBtn()
        {

            mGoToBtn.CustomActive(true);

            maskGo.CustomActive(false);
            mTakeTaskBtn.CustomActive(false);
            mLvLockTxt.CustomActive(false);
            mNumHaveUsedTxt.CustomActive(false);
            mRewardBtn.CustomActive(false);
            mRewardAfterGo.CustomActive(false);

            mTaskProgressSlider.CustomActive(true);
            mTaskProgressSlider.CustomActive(true);
        }
        /// <summary>
        /// 只显示 领取任务按钮
        /// </summary>
        private void OnlyShowTakeTaskBtn()
        {
            mTakeTaskBtn.CustomActive(true);
            mLvLockTxt.CustomActive(false);
            mNumHaveUsedTxt.CustomActive(false);
            mGoToBtn.CustomActive(false);
            mRewardBtn.CustomActive(false);
            mRewardAfterGo.CustomActive(false);

            mTaskProgressSlider.CustomActive(false);
            mTaskProgressSlider.CustomActive(false);
        }

        /// <summary>
        /// 只显示 等级不够的描述
        /// </summary>
        private void OnlyShowLvLockTxt()
        {
            maskGo.CustomActive(true);
            mLvLockTxt.CustomActive(true);
            if(mData!=null)
            {
                mLvLockTxt.text = string.Format(TR.Value("limitactivity_shuqi_limitLevel"), mData.PlayerLevelLimit);
            }
          
            mTakeTaskBtn.CustomActive(false);
            mNumHaveUsedTxt.CustomActive(false);
            mGoToBtn.CustomActive(false);
            mRewardBtn.CustomActive(false);
            mRewardAfterGo.CustomActive(false);

            mTaskProgressSlider.CustomActive(false);
            mTaskProgressSlider.CustomActive(false);
        }

       
        /// <summary>
        /// 只显示 次数用完的描述
        /// </summary>
        private void OnlyShowNumHaveUsedTxt()
        {
            mNumHaveUsedTxt.CustomActive(true);
            mNumHaveUsedTxt.text = TR.Value("limitactivity_shuqi_numhaveused");
            maskGo.CustomActive(true);

            mTakeTaskBtn.CustomActive(false);
            mLvLockTxt.CustomActive(false);
            mGoToBtn.CustomActive(false);
            mRewardBtn.CustomActive(false);
            mRewardAfterGo.CustomActive(false);

            mTaskProgressSlider.CustomActive(false);
            mTaskProgressSlider.CustomActive(false);
        }

        /// <summary>
        /// 只显示 不能接取任务
        /// </summary>
        private void OnlyShowUnTakeTask()
        {
            maskGo.CustomActive(true);

            mNumHaveUsedTxt.CustomActive(false);
            mTakeTaskBtn.CustomActive(false);
            mLvLockTxt.CustomActive(false);
            mGoToBtn.CustomActive(false);
            mRewardBtn.CustomActive(false);
            mRewardAfterGo.CustomActive(false);
            mTaskProgressSlider.CustomActive(false);
            mTaskProgressSlider.CustomActive(false);
        }
    }
}
