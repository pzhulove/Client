using UnityEngine;
using System.Collections;
using Protocol;

namespace GameClient
{
    class BudoActiveBinder : MonoBehaviour
    {
        public GameObject[] RedPoints = null;
        public GameObject[] ShowTargets = null;
        public bool bAlwasyShow = false;
        public bool bReadyShow = false;
        public TimeRefresh comTimer = null;
        public GameObject goGo = null;
        public bool bNeedNotify = false;
        NotifyInfo noticeData = new NotifyInfo { type = (uint)NotifyType.NT_BUDO };
        public void OnClick()
        {
            BudoManager.GetInstance().TryBeginActive();
        }

        void OnLevelChanged(int iPreLv, int iCurLv)
        {
            _UpdateStatus();
        }

        void Start()
        {
            BudoManager.GetInstance().onBudoInfoChanged += OnBudoInfoChanged;
            PlayerBaseData.GetInstance().onLevelChanged += OnLevelChanged;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityUpdate, _OnActivityUpdated);

            _UpdateStatus();
        }

        void _OnActivityUpdated(UIEvent a_event)
        {
            uint nID = (uint)a_event.Param1;
            if(ActiveManager.GetInstance().IsBudoActive((int)nID))
            {
                _UpdateStatus();
            }
        }

        void _UpdateStatus()
        {
            if (comTimer != null)
            {
                comTimer.CustomActive(false);
                if (bReadyShow && ActiveManager.GetInstance().allActivities.ContainsKey(BudoManager.ActiveID))
                {
                    var activity = ActiveManager.GetInstance().allActivities[BudoManager.ActiveID];
                    if (activity != null && activity.state == 2)
                    {
                        comTimer.Initialize();
                        comTimer.CustomActive(true);

                        uint deltaTime = TimeManager.GetInstance().GetServerTime() <= activity.startTime ? activity.startTime - TimeManager.GetInstance().GetServerTime() : 0;
                        comTimer.Time = deltaTime;
                    }
                }
            }

            if(goGo != null)
            {
                goGo.CustomActive(false);
            }
            bool bNeedShow = bAlwasyShow;
            bool bNeedShowRedPoint = false;
            if (BudoManager.GetInstance().CanAcqured && !bReadyShow)
            {
                bNeedShow = true;
                bNeedShowRedPoint = true;
            }
            else if (ActiveManager.GetInstance().allActivities.ContainsKey(BudoManager.ActiveID))
            {
                var activity = ActiveManager.GetInstance().allActivities[BudoManager.ActiveID];
                if (activity != null && (activity.state == 1 || activity.state == 2 && bReadyShow))
                {
                    bNeedShow = true;
                    if (goGo != null)
                    {
                        goGo.CustomActive(activity.state == 2);
                    }
                }
            }

            if (RedPoints != null)
            {
                for (int i = 0; i < RedPoints.Length; ++i)
                {
                    RedPoints[i].CustomActive(bNeedShowRedPoint);
                }
            }

            if(!BudoManager.GetInstance().IsLevelFit)
            {
                bNeedShow = false;
            }

            if (ShowTargets != null)
            {
                for (int i = 0; i < ShowTargets.Length; ++i)
                {
                    ShowTargets[i].CustomActive(bNeedShow);
                }
            }

            if(bNeedNotify)
            {
                if (bNeedShow)
                {
                    ActivityNoticeDataManager.GetInstance().AddActivityNotice(noticeData);
                    DeadLineReminderDataManager.GetInstance().AddActivityNotice(noticeData);
                }
                else
                {
                    ActivityNoticeDataManager.GetInstance().DeleteActivityNotice(noticeData);
                    DeadLineReminderDataManager.GetInstance().DeleteActivityNotice(noticeData);
                }
            }
        }

        void OnBudoInfoChanged()
        {
            _UpdateStatus();
        }

        void OnDestroy()
        {
            BudoManager.GetInstance().onBudoInfoChanged -= OnBudoInfoChanged;
            PlayerBaseData.GetInstance().onLevelChanged -= OnLevelChanged;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityUpdate, _OnActivityUpdated);
            RedPoints = null;
            ShowTargets = null;
        }
    }
}