using Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace GameClient
{
    public class SummerVacationWeeklyActivityView : LimitTimeActivityViewCommon
    {
        /// <summary>
        /// 角色提示
        /// </summary>
        [SerializeField]
        private Text mRoleTip;

        /// <summary>
        /// 账号提示
        /// </summary>
        [SerializeField]
        private Text mAccountTip;

        /// <summary>
        ///账号可领取的次数
        /// </summary>
        private int mCanReceiveAccountNum = 3;
        /// <summary>
        /// 总共可以领取的账号次数
        /// </summary>
        private int mTotalReceiveAcountNum = 0;
        /// <summary>
        ///角色可领取的次数
        /// </summary>
        private int mCanReceiveRoleNum = 1;
        /// <summary>
        /// 角色总共可领取的次数
        /// </summary>
        private int mTotalReceiveRoleNum = 0;

      
        private ILimitTimeActivityModel mData;

        public override void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            base.Init(model, onItemClick);
            mData = model;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivtiyLimitTimeAccounterNumUpdate, _OnActivityCounterUpdate);
            _UpdateData();
        }
        
        private void _OnActivityCounterUpdate(UIEvent uiEvent)
        {
            if(mData!=null)
            {
                if ((uint)uiEvent.Param1 == mData.Id)
                {
                    _UpdateData();
                }
            }
            
        }

        public override void UpdateData(ILimitTimeActivityModel data)
        {
            base.UpdateData(data);
            _UpdateData();
        }
     

      
        private void _UpdateData()
        {
            int ret = (int)ActivityDataManager.GetInstance().GetActivityConunter(ActivityLimitTimeFactory.EActivityType.OAT_SUMMER_WEEKLY_VACATION,ActivityLimitTimeFactory.EActivityCounterType.OAT_SUMMER_WEEKLY_VACATION);
            if (mData != null)
            {
                if (mData.ParamArray != null && mData.ParamArray.Length >= 2)
                {
                    mTotalReceiveAcountNum = (int)mData.ParamArray[0];
                    mTotalReceiveRoleNum = (int)mData.ParamArray[1];
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
                mCanReceiveRoleNum = mTotalReceiveRoleNum - haveRecivedIdList.Count;
                if (mCanReceiveRoleNum <= 0)
                {
                    mCanReceiveRoleNum = 0;
                }
               
            }
            if (mCanReceiveAccountNum <= 0)//账号次数用完了，角色次数强制为0 
            {
                mCanReceiveRoleNum = 0;
            }
            if (mAccountTip != null)
            {
                mAccountTip.text = string.Format(TR.Value("limitactivity_shuqi_accounttip"), mCanReceiveAccountNum, mTotalReceiveAcountNum);
            }
            if (mRoleTip != null)
            {
                mRoleTip.text = string.Format(TR.Value("limitactivity_shuqi_roletip"), mCanReceiveRoleNum, mTotalReceiveRoleNum);
            }
        }


        public override void Show()
        {
           base.Show();
        }

        public override void Hide()
        {
          base.Hide();
        }

        public override void Dispose()
        {
            base.Dispose();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivtiyLimitTimeAccounterNumUpdate, _OnActivityCounterUpdate);
        }

    }
}
