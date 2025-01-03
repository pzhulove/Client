using System;
using System.Collections.Generic;
using System.Collections;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class WeekSignInView : ActivityChargeBaseView
    {

        private bool _isAlreadyInit = false;

        [SerializeField] private WeekSignInType _weekSignInType;
        [Space(10)]
        [HeaderAttribute("Control")]
        [Space(10)]
        [SerializeField] private WeekSignInCommonControl weekSignInCommonControl;
        [SerializeField] private WeekSignInAwardControl weekSignInAwardControl;

        #region Init
        protected virtual void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }

        private void ClearData()
        {
            _isAlreadyInit = false;
        }

        private void OnEnable()
        {
            if (_isAlreadyInit == false)
            {
                _isAlreadyInit = true;
            }
            else
            {
                OnEnableWeekSignIn();
            }
        }

        private void OnDisable()
        {
        }

        private void BindEvents()
        {

        }

        private void UnBindEvents()
        {

        }

        public override void InitView(int intParam)
        {
            //默认显示活动周签到
            _weekSignInType = WeekSignInType.ActivityWeekSignIn;
            if ((WeekSignInType) intParam == WeekSignInType.NewPlayerWeekSignIn)
                _weekSignInType = WeekSignInType.NewPlayerWeekSignIn;

            WeekSignInUtility.SetWeekSignInShowRedPointTimeByDailyLogin(_weekSignInType);

            InitWeekSignIn();
        }
        
        protected void InitWeekSignIn()
        {
            if (weekSignInCommonControl != null)
                weekSignInCommonControl.InitCommonControl(_weekSignInType);

            if (weekSignInAwardControl != null)
                weekSignInAwardControl.InitAwardControl(_weekSignInType);
        }

        protected void OnEnableWeekSignIn()
        {
            if (_weekSignInType == WeekSignInType.None)
                return;

            if (weekSignInCommonControl != null)
                weekSignInCommonControl.OnEnableCommonControl();

            if (weekSignInAwardControl != null)
                weekSignInAwardControl.OnEnableAwardControl();
        }

        #endregion      

    }
}