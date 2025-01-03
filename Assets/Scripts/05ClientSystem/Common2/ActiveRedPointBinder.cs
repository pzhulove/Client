using UnityEngine;
using System.Collections;
///////删除linq
using DG.Tweening;

namespace GameClient
{
    class ActiveRedPointBinder : MonoBehaviour
    {
        public GameObject[] redPoints = null;
        public int iActiveConfigID;
        public int iMainID = 0;
        public GameObject PlayAniObj = null;

        // Use this for initialization
        void Start()
        {
            ActiveManager.GetInstance().onAddMainActivity += _OnAddMainActivity;
            ActiveManager.GetInstance().onRemoveMainActivity += _OnRemoveMainActivity;
            ActiveManager.GetInstance().onUpdateMainActivity += _OnUpdateMainActivity;
            ActiveManager.GetInstance().onActivityUpdate += _OnActivityUpdate;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.WelfActivityRedPoint, OnUpdateRedPoint);
            BindEvents();

            _UpdateRedPoint();
        }

        private void BindEvents()
        {
            if (iActiveConfigID == FinancialPlanDataManager.GetInstance().ActivityConfigId)
            {
                //存在理财计划，绑定数据
                if (FinancialPlanDataManager.GetInstance().IsExistFinancialPlanActivity() == true)
                {
                    if (iMainID == 0 || iMainID == FinancialPlanDataManager.GetInstance().ActivityTemplateId)
                    {
                        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FinancialPlanRedPointTips, UpdateSpecialActiveRedPoint);
                    }
                }
            }

            //检测周签到的红点是否存在
            if (iActiveConfigID == WeekSignInDataManager.WeekSignInConfigId)
            {
                //新人周签到
                if (WeekSignInUtility.IsWeekSignInOpenByWeekSignInType(WeekSignInType.NewPlayerWeekSignIn) == true)
                {
                    //包含所有的ActivityConfigID 或者新人周签到的ID
                    if (iMainID == 0 || iMainID == WeekSignInDataManager.NewPlayerWeekSignInOpActTypeId)
                    {
                        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnNewPlayerWeekSignInRedPointChanged, UpdateSpecialActiveRedPoint);
                    }
                }

                //活动周签到
                if (WeekSignInUtility.IsWeekSignInOpenByWeekSignInType(WeekSignInType.ActivityWeekSignIn) == true)
                {
                    //包含所有的ActivityConfigID 或者活动周签到的ID
                    if (iMainID == 0 || iMainID == WeekSignInDataManager.ActivityWeekSignInOpActTypeId)
                    {
                        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnActivityWeekSignInRedPointChanged, UpdateSpecialActiveRedPoint);
                    }
                }
            }
			
			if (iActiveConfigID == PayManager.MONTH_CARD_TYPE_CONFIG_ID)
            {
                if (iMainID == 0 || iMainID == PayManager.MONTH_CARD_TEMPLATE_ID)
                {
                    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMonthCardRewardRedPointReset, _OnMonthCardRewardRedPointReset);
                }
            }
        }

        void OnDestroy()
        {
            ActiveManager.GetInstance().onAddMainActivity -= _OnAddMainActivity;
            ActiveManager.GetInstance().onUpdateMainActivity -= _OnUpdateMainActivity;
            ActiveManager.GetInstance().onRemoveMainActivity -= _OnRemoveMainActivity;
            ActiveManager.GetInstance().onActivityUpdate -= _OnActivityUpdate;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.WelfActivityRedPoint, OnUpdateRedPoint);
            UnBindEvents();
        }

        private void UnBindEvents()
        {
            if(iActiveConfigID == FinancialPlanDataManager.GetInstance().ActivityConfigId)
            {
                //存在理财计划，绑定数据
                if (FinancialPlanDataManager.GetInstance().IsExistFinancialPlanActivity() == true)
                {
                    if (iMainID == 0 || iMainID == FinancialPlanDataManager.GetInstance().ActivityTemplateId)
                    {
                        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FinancialPlanRedPointTips, UpdateSpecialActiveRedPoint);
                    }
                }
            }

            //检测周签到的红点是否存在
            if (iActiveConfigID == WeekSignInDataManager.WeekSignInConfigId)
            {
                //新人周签到
                if (WeekSignInUtility.IsWeekSignInOpenByWeekSignInType(WeekSignInType.NewPlayerWeekSignIn) == true)
                {
                    //包含所有的ActivityConfigID 或者新人周签到的ID
                    if (iMainID == 0 || iMainID == WeekSignInDataManager.NewPlayerWeekSignInOpActTypeId)
                    {
                        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnNewPlayerWeekSignInRedPointChanged, UpdateSpecialActiveRedPoint);
                    }
                }

                //活动周签到
                if (WeekSignInUtility.IsWeekSignInOpenByWeekSignInType(WeekSignInType.ActivityWeekSignIn) == true)
                {
                    //包含所有的ActivityConfigID 或者活动周签到的ID
                    if (iMainID == 0 || iMainID == WeekSignInDataManager.ActivityWeekSignInOpActTypeId)
                    {
                        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnActivityWeekSignInRedPointChanged, UpdateSpecialActiveRedPoint);
                    }
                }
            }
			
			if (iActiveConfigID == PayManager.MONTH_CARD_TYPE_CONFIG_ID)
            {
                if (iMainID == 0 || iMainID == PayManager.MONTH_CARD_TEMPLATE_ID)
                {
                    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMonthCardRewardRedPointReset, _OnMonthCardRewardRedPointReset);
                }
            }
        }

        private void UpdateSpecialActiveRedPoint(UIEvent data = null)
        {
            _UpdateRedPoint();
        }

        private void _OnMonthCardRewardRedPointReset(UIEvent uiEvent)
        {
            _UpdateRedPoint();
        }

        void _UpdateRedPoint()
        {
            if(redPoints == null)
            {
                return;
            }
            bool bNeedShow = false;

            // 先判断走活动模板表的页签中是否有红点，如果有的话直接显示红点
            var activities = ActiveManager.GetInstance().GetType2Templates(iActiveConfigID);    
            if(activities != null)
            {
                int iMainActiveID = 0;
                for (int i = 0; i < activities.Count && !bNeedShow; ++i)
                {
                    iMainActiveID = activities[i].ID;

                    if (iMainID != 0 && iMainID != iMainActiveID)
                    {
                        continue;
                    }

                    if (ActiveManager.GetInstance().ActiveDictionary.ContainsKey(iMainActiveID))
                    {
                        var data = ActiveManager.GetInstance().ActiveDictionary[iMainActiveID];
                        for (int j = 0; j < data.akChildItems.Count; ++j)
                        {
                            if(ActiveManager.GetInstance().CheckChildRedPass(data.akChildItems[j]))
                            {
                                bNeedShow = true;
                                break;
                            }

                        }

                        if (ActiveManager.activityId[0] == data.iActiveID && ActiveManager.GetInstance().WelfareTABEnergyRedPointFlag == true)
                        {
                            bNeedShow = false;
                        }
                        else if (ActiveManager.activityId[1] == data.iActiveID && ActiveManager.GetInstance().WelfareTABRewardRedPointFlag == true)
                        {
                            bNeedShow = false;
                        }
                    }
                }
            }
   
            //如果bNeedShow为false，则检测特殊的活动（不走活动流程）是否存在红点
            if (bNeedShow == false)
            {
                bNeedShow = IsSpecialActiveHaveRedPoint();
            }

            for(int i = 0; i < redPoints.Length; ++i)
            {
                redPoints[i].CustomActive(bNeedShow);
            }

            if(PlayAniObj != null)
            {
                DOTweenAnimation[] anims = PlayAniObj.GetComponents<DOTweenAnimation>(); 
                for(int i = 0; i < anims.Length; i++)
                {
                    if(anims[i] == null)
                    {
                        continue;
                    }

                    if(bNeedShow)
                    {
                        anims[i].DORestart();
                    }
                    else
                    {
                        anims[i].DOPause();
                        anims[i].gameObject.transform.localRotation = Quaternion.identity;
                    }
                }
            }
        }

        private bool IsSpecialActiveHaveRedPoint()
        {
            //检测理财计划是否存在红点，理财计划的逻辑是单独出来的
            if (iActiveConfigID == FinancialPlanDataManager.GetInstance().ActivityConfigId)
            {
                //存在理财计划
                if (FinancialPlanDataManager.GetInstance().IsExistFinancialPlanActivity() == true)
                {
                    //包含所有的ActivityConfigID 或者是理财计划的ID
                    if (iMainID == 0 || iMainID == FinancialPlanDataManager.GetInstance().ActivityTemplateId)
                    {
                        if (FinancialPlanDataManager.GetInstance().IsShowRedPoint() == true)
                            return true;
                    }
                }
            }

            //检测周签到的红点是否存在
            if (iActiveConfigID == WeekSignInDataManager.WeekSignInConfigId)
            {
                //新人周签到
                if (WeekSignInUtility.IsWeekSignInOpenByWeekSignInType(WeekSignInType.NewPlayerWeekSignIn) == true)
                {
                    //包含所有的ActivityConfigID 或者新人周签到的ID
                    if (iMainID == 0 || iMainID == WeekSignInDataManager.NewPlayerWeekSignInOpActTypeId)
                    {
                        if (WeekSignInUtility.IsWeekSignInRedPointVisibleByWeekSignInType(WeekSignInType.NewPlayerWeekSignIn) == true)
                            return true;
                    }
                }

                //活动周签到
                if (WeekSignInUtility.IsWeekSignInOpenByWeekSignInType(WeekSignInType.ActivityWeekSignIn) == true)
                {
                    //包含所有的ActivityConfigID 或者活动周签到的ID
                    if (iMainID == 0 || iMainID == WeekSignInDataManager.ActivityWeekSignInOpActTypeId)
                    {
                        if (WeekSignInUtility.IsWeekSignInRedPointVisibleByWeekSignInType(WeekSignInType.ActivityWeekSignIn) == true)
                            return true;
                    }
                }
            }

            if (iActiveConfigID == ActivityDataManager.MONTH_SIGN_IN_CONFIG_ID)
            {
                if (iMainID == 0 || iMainID == ActivityDataManager.MONTH_SIGN_IN_CONFIG_ID)
                {
                    var isShowRedPoint = ActivityDataManager.GetInstance().IsShowSignInRedPoint();
                    if (isShowRedPoint)
                    {
                        return true;
                    }
                }
            }

            if (iActiveConfigID == PayManager.MONTH_CARD_TYPE_CONFIG_ID)
            {
                if (iMainID == 0 || iMainID == PayManager.MONTH_CARD_TEMPLATE_ID)
                {
                    return MonthCardRewardLockersDataManager.GetInstance().IsRedPointShow();
                }
            }            

            return false;
        }

        void _OnAddMainActivity(ActiveManager.ActiveData data)
        {
            if(data.mainItem.ActiveTypeID == iActiveConfigID)
            {
                _UpdateRedPoint();
            }
        }

        void _OnRemoveMainActivity(ActiveManager.ActiveData data)
        {
            if (data.mainItem.ActiveTypeID == iActiveConfigID)
            {
                _UpdateRedPoint();
            }
        }

        void _OnActivityUpdate(ActiveManager.ActivityData data, ActiveManager.ActivityUpdateType EActivityUpdateType)
        {
            var activeData = ActiveManager.GetInstance().GetActiveData(data.activeItem.TemplateID);
            if(activeData != null && activeData.mainItem.ActiveTypeID == iActiveConfigID)
            {
                _UpdateRedPoint();
            }
        }

        void _OnUpdateMainActivity(ActiveManager.ActiveData data)
        {
            if (data.mainItem.ActiveTypeID == iActiveConfigID)
            {
                _UpdateRedPoint();
            }
        }

        void OnUpdateRedPoint(UIEvent uiEvent)
        {
            _UpdateRedPoint();
        }
    }
}