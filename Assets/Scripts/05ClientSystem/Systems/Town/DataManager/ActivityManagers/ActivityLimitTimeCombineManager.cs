using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using GameClient;
using Protocol;
using Network;
using System;
using FashionLimitTimeBuy;
using LimitTimeGift;
using ProtoTable;

/*限时活动*/
namespace ActivityLimitTime
{
    public sealed class ActivityLimitTimeCombineManager : DataManager<ActivityLimitTimeCombineManager>
    {
        public BossActivityDataManager BossDataManager { get; private set; }
        public LimitTimeGiftDataManager GiftDataManager { get; private set; }
        public ActivityLimitTimeManager LimitTimeManager { get; private set; }

        public const uint SUMMER_WATERMELON_ID = 1090;
        public const int SUMMER_GIFT_ID = 79000;
        public const int SUMMER_DRINK_ID = 79001;

        public bool IsCheckedLimitGift
        {
            private get { return this.mIsCheckedLimitGift; }
            set
            {
                bool oldValue = mIsCheckedLimitGift;
                this.mIsCheckedLimitGift = value;
                if (oldValue != this.mIsCheckedLimitGift)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnLimitTimeGiftChecked);
                }
            }
        }

        private bool mIsCheckedLimitGift = false;


        public bool IsCheckedLimitTimeMallGift
        {
            get { return this.mIsCheckedLimitTimeMallGift; }
            set
            {
                bool oldValue = mIsCheckedLimitTimeMallGift;
                this.mIsCheckedLimitTimeMallGift = value;
                if (oldValue != this.mIsCheckedLimitTimeMallGift)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeTaskUpdate);
                }
            }
        }

        private bool mIsCheckedLimitTimeMallGift = false;

        public override EEnterGameOrder GetOrder()
        {
            return base.GetOrder();
        }

        public bool IsHaveActivity()
        {
            return IsHaveFestival() || IsHaveLimitGift() || IsHaveLimitTime();
        }

        public bool IsHaveFestival()
        {
            if (!Utility.IsUnLockFunc((int) FunctionUnLock.eFuncType.FestivalActivity))
                return false;


            bool isHave = this.BossDataManager.ActivityDic.Count > 0;
            if (!isHave)
            {
                var limitTimeList = ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.activityLimitTimeDataList;
                if (limitTimeList != null && limitTimeList.Count > 0)
                {
                    for (int i = 0; i < limitTimeList.Count; i++)
                    {
                        var tempData = limitTimeList[i];
                        if (tempData.ActivityState == ActivityState.Start)
                        {
                            if (tempData.DataId == SUMMER_WATERMELON_ID)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            return true;
        }

        public bool IsHaveLimitGift()
        {
            int limitTimeActOpen = 13;
            int enableLevel = 8;
            var clientTable = TableManager.GetInstance().GetTableItem<SwitchClientFunctionTable>(limitTimeActOpen);
            if (clientTable != null)
            {
                enableLevel = clientTable.ValueA;
            }
            if (PlayerBaseData.GetInstance().Level < enableLevel)
            {
                return false;
            }

            var list = GiftDataManager.GetGiftsDataInMall();
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].GiftId != SUMMER_DRINK_ID && list[i].GiftId != SUMMER_GIFT_ID)
                {
                    return true;
                }
            }


            return false;
        }

        public bool IsHaveLimitTime()
        {
            var limitTimeList = LimitTimeManager.activityLimitTimeDataList;

            if (!Utility.IsUnLockFunc((int) FunctionUnLock.eFuncType.ActivityLimitTime))
            {
                return false;
            }

            if (limitTimeList != null && limitTimeList.Count > 0)
            {
                for (int i = 0; i < limitTimeList.Count; i++)
                {
                    var tempData = limitTimeList[i];
                    if (tempData.ActivityState == ActivityState.Start)
                    {
                        if (tempData.ActivityType == Protocol.OpActivityTmpType.OAT_BIND_PHONE)         //屏蔽手机绑定活动！！！
                            continue;

                        if (tempData.ActivityType == Protocol.OpActivityTmpType.OAT_GAMBING)    //暂时在这里屏蔽夺宝活动,应该数据层就屏蔽。从其他接口进入
                            continue;

                        if (tempData.DataId == SUMMER_WATERMELON_ID)    //夏日活动
                            continue;

                        return true;
                    }
                }
            }

            return false;
        }

        public string GetFestivalActivityName()
        {
            return this.BossDataManager.BossActivityBtIconName;
        }

        public string GetGiftActivityName()
        {
            return TR.Value("activity_tab_limit_time_gift");
        }

        public string GetLimitTimeActivityName()
        {
            return TR.Value("activity_tab_limit_time");
        }

        public bool IsNeedRedPoint()
        {
            return IsFestivalNeedRedPoint() || IsLimitTimeNeedRedPoint() || IsGiftNeedRedPoint();
        }

        public bool IsFestivalNeedRedPoint()
        {
            var limitTimeList = ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.activityLimitTimeDataList;
            if (limitTimeList != null && limitTimeList.Count > 0)
            {
                for (int i = 0; i < limitTimeList.Count; i++)
                {
                    var tempData = limitTimeList[i];
                    if (tempData.ActivityState == ActivityState.Start)
                    {
                        if (tempData.DataId == SUMMER_WATERMELON_ID)
                        {

                            for (int j = 0; j < tempData.activityDetailDataList.Count; j++)
                            {
                                if (tempData.activityDetailDataList[j].ActivityDetailState == ActivityTaskState.Finished
                                    || (tempData.ActivityType == OpActivityTmpType.OAT_HELL_TICKET_FOR_DRAW_PRIZE && CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_LOTTERY_TIME) > 0))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return this.BossDataManager.IsHasTaskFinished();
        }

        public bool IsLimitTimeNeedRedPoint()
        {
            return this.LimitTimeManager.CheckHasTaskWaitToReceive() || !IsCheckedLimitTimeMallGift;
        }

        public bool IsGiftNeedRedPoint()
        {
            return LimitTimeBuyActivityManager.GetInstance().mIsHaveOtherGift && !this.IsCheckedLimitGift;
        }

        public override void Initialize()
        {
            Clear();

            this.BossDataManager = new BossActivityDataManager();
            this.BossDataManager.Initialize();
            this.GiftDataManager = new LimitTimeGiftDataManager();
            this.GiftDataManager.Initialize();;
            this.LimitTimeManager = new ActivityLimitTimeManager();
            this.LimitTimeManager.Initialize();
            this.IsCheckedLimitGift = false;
            IsCheckedLimitTimeMallGift = true;
        }

        public override void Clear()
        {
            if (this.BossDataManager != null)
            {
                this.BossDataManager.Clear();
            }

            if (this.GiftDataManager != null)
            {
                this.GiftDataManager.Clear();
            }

            if (this.LimitTimeManager != null)
            {
                this.LimitTimeManager.Clear();
            }
            this.BossDataManager = null;
            this.GiftDataManager = null;
            this.LimitTimeManager = null;
            this.IsCheckedLimitGift = false;
        }

        public void OnUpdate(float timeElapsed)
        {
            if (this.LimitTimeManager != null)
            {
                this.LimitTimeManager.OnUpdate(timeElapsed);
            }
        }

    }
}