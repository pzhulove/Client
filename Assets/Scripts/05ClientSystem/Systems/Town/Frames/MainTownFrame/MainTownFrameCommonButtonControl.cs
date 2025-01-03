using System;
using System.Collections.Generic;
using System.Collections;
using Protocol;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    public class MainTownFrameCommonButtonControl : MonoBehaviour
    {
        private bool _isInit = false;

        [Space(15)]
        [HeaderAttribute("ActivityWeekSignIn")]
        [Space(15)]
        //周签到
        [SerializeField]
        private GameObject activityWeekSignInRoot = null;
        [SerializeField] private Button activityWeekSignInButton = null;
        [SerializeField] private GameObject activityWeekSignInRedPoint = null;

        [Space(15)]
        [HeaderAttribute("NewPlayerWeekSignIn")]
        [Space(15)]
        [SerializeField] private GameObject newPlayerWeekSignInRoot = null;
        [SerializeField] private Button newPlayerWeekSignInButton = null;
        [SerializeField] private GameObject newPlayerWeekSignInRedPoint = null;

        #region Init
        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            _isInit = false;
        }

        private void OnEnable()
        {
            if (_isInit == false)
            {
                _isInit = true;
            }
            else
            {
                UpdateMainTownFrameCommonButtonControl();
            }
        }
        
        private void BindEvents()
        {
            if (activityWeekSignInButton != null)
            {
                activityWeekSignInButton.onClick.RemoveAllListeners();
                activityWeekSignInButton.onClick.AddListener(OnActivityWeekSignInClick);
            }

            if (newPlayerWeekSignInButton != null)
            {
                newPlayerWeekSignInButton.onClick.RemoveAllListeners();
                newPlayerWeekSignInButton.onClick.AddListener(OnNewPlayerWeekSignInClick);
            }

            //if (teamDuplicationButton != null)
            //{
            //    teamDuplicationButton.onClick.RemoveAllListeners();
            //    teamDuplicationButton.onClick.AddListener(OnTeamDuplicationButtonClick);
            //}


            //按钮和红点更新
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnActivityWeekSignInRedPointChanged, 
                OnActivityWeekSignInStatusUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnNewPlayerWeekSignInRedPointChanged,
                OnNewPlayerWeekSignInStatusUpdate);

            //团本功能解锁
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.NewFuncUnlock, OnReceiveNewFuncUnLockMessage);
        }

        private void UnBindEvents()
        {
            if (activityWeekSignInButton != null)
                activityWeekSignInButton.onClick.RemoveAllListeners();

            if (newPlayerWeekSignInButton != null)
                newPlayerWeekSignInButton.onClick.RemoveAllListeners();

            //if (teamDuplicationButton != null)
            //{
            //    teamDuplicationButton.onClick.RemoveAllListeners();
            //}

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnActivityWeekSignInRedPointChanged, OnActivityWeekSignInStatusUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnNewPlayerWeekSignInRedPointChanged,
                OnNewPlayerWeekSignInStatusUpdate);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.NewFuncUnlock, OnReceiveNewFuncUnLockMessage);
        }
        #endregion

        public void UpdateMainTownFrameCommonButtonControl()
        {
            UpdateNewPlayerWeekSignInStatus();
            UpdateActivityWeekSignInStatus();
        }

        #region UIEvent

        private void OnActivityWeekSignInStatusUpdate(UIEvent uiEvent)
        {
            UpdateActivityWeekSignInStatus();
        }

        private void OnNewPlayerWeekSignInStatusUpdate(UIEvent uiEvent)
        {
            UpdateNewPlayerWeekSignInStatus();
        }

        //新的功能解锁
        private void OnReceiveNewFuncUnLockMessage(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null || uiEvent.Param2 == null)
                return;

            FunctionUnLock.eFuncType funcType = (FunctionUnLock.eFuncType)uiEvent.Param2;

            //不是团本
            if (funcType != FunctionUnLock.eFuncType.TeamCopy)
                return;
        }
        #endregion

        #region UpdateWeekSignInStatus
        private void UpdateNewPlayerWeekSignInStatus()
        {
            bool isNewPlayerWeekSignInVisible =
                WeekSignInUtility.IsWeekSignInVisibleByWeekSignInType(WeekSignInType.NewPlayerWeekSignIn);
            if (newPlayerWeekSignInRoot != null)
            {
                if (isNewPlayerWeekSignInVisible == true)
                {
                    newPlayerWeekSignInRoot.CustomActive(true);
                }
                else
                {
                    newPlayerWeekSignInRoot.CustomActive(false);
                }
            }

            if (isNewPlayerWeekSignInVisible == true)
            {
                UpdateNewPlayerWeekSignInRedPoint();
            }
            
        }

        private void UpdateNewPlayerWeekSignInRedPoint()
        {
            if (newPlayerWeekSignInRedPoint != null)
            {
                if (WeekSignInUtility.IsWeekSignInRedPointVisibleByWeekSignInType(WeekSignInType.NewPlayerWeekSignIn) == true)
                {

                    newPlayerWeekSignInRedPoint.CustomActive(true);
                }
                else
                {
                    newPlayerWeekSignInRedPoint.CustomActive(false);
                }
            }
        }

        private void UpdateActivityWeekSignInStatus()
        {

            var isActivityWeekSignInVisible =
                WeekSignInUtility.IsWeekSignInVisibleByWeekSignInType(WeekSignInType.ActivityWeekSignIn);

            if (activityWeekSignInRoot != null)
            {
                if (isActivityWeekSignInVisible == true)
                {
                    activityWeekSignInRoot.CustomActive(true);
                }
                else
                {
                    activityWeekSignInRoot.CustomActive(false);
                }
            }

            if (isActivityWeekSignInVisible == true)
            {
                UpdateActivityWeekSignInRedPoint();
            }
            
        }

        private void UpdateActivityWeekSignInRedPoint()
        {
            if (activityWeekSignInRedPoint != null)
            {
                if (WeekSignInUtility.IsWeekSignInRedPointVisibleByWeekSignInType(WeekSignInType.ActivityWeekSignIn) == true)
                {
                    activityWeekSignInRedPoint.CustomActive(true);
                }
                else
                {
                    activityWeekSignInRedPoint.CustomActive(false);
                }
            }
        }

        #endregion

        #region  ButtonClicked

        private void OnActivityWeekSignInClick()
        {
            WeekSignInUtility.OpenActiveFrameByActivityWeekSignIn();
        }

        private void OnNewPlayerWeekSignInClick()
        {
            WeekSignInUtility.OpenActiveFrameByNewPlayerWeekSignIn();
        }

        private void OnTeamDuplicationButtonClick()
        {
            //进入场景
            TeamDuplicationUtility.EnterToTeamDuplicationSceneFromTown();
        }

        #endregion
    }
}