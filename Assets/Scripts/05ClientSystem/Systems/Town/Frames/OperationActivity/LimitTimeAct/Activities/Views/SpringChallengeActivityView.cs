using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    public class SpringChallengeActivityView : MonoBehaviour, IActivityView
    {
        [SerializeField]private Text mTimeTxt;
        [SerializeField]private Text mRuleTxt;
        [SerializeField]private Button mIntegrationBtn;
        [SerializeField]private ComUIListScript mUIList;
        [SerializeField] private Text mNum;

        private uint scoreActivityId = 0;
        private ILimitTimeActivityModel mModel;
        public void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            mModel = model;
            scoreActivityId = model.Param;
            mTimeTxt.SafeSetText(string.Format("{0}", Function.GetTimeWithoutYearNoZero((int)model.StartTime, (int)model.EndTime)));
            mRuleTxt.SafeSetText(model.RuleDesc.Replace('|', '\n'));
            mNum.SafeSetText(CountDataManager.GetInstance().GetCount(CounterKeys.TOTAL_SPRING_SCORE).ToString());

            mIntegrationBtn.SafeAddOnClickListener(_OnIntegrationBtnClick);
            _InitItems(model);
        }
        

        public void Close()
        {
            Dispose();

            if (mUIList != null)
            {
                mUIList.onBindItem -= OnBindItemDelegate;
                mUIList.onItemVisiable -= OnItemVisiableDelegate;
            }

            Destroy(gameObject);
        }

        public void Dispose()
        {
            mIntegrationBtn.SafeRemoveOnClickListener(_OnIntegrationBtnClick);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 显示任务数据
        /// </summary>
        /// <param name="model"></param>
        private void _InitItems(ILimitTimeActivityModel data)
        {
            if (mUIList != null)
            {
                mUIList.Initialize();
                mUIList.onBindItem += OnBindItemDelegate;
                mUIList.onItemVisiable += OnItemVisiableDelegate;
            }

            mUIList.SetElementAmount(data.TaskDatas.Count);
        }

        private SpringChallengeItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<SpringChallengeItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var objItem = item.gameObjectBindScript as SpringChallengeItem;
            if (objItem != null && item.m_index >= 0 && item.m_index < mModel.TaskDatas.Count)
            {
                objItem.Init(mModel.TaskDatas[item.m_index].DataId, mModel.Id, mModel.TaskDatas[item.m_index], null);
                objItem.SetBackground(item.m_index);
            }
        }
        
        /// <summary>
        /// 前往积分商城
        /// </summary>
        private void _OnIntegrationBtnClick()
        {
            LimitTimeActivityFrame limitTimeActivityFrame= ClientSystemManager.GetInstance().GetFrame(typeof(LimitTimeActivityFrame)) as LimitTimeActivityFrame;
            if(limitTimeActivityFrame!=null)
            {
                limitTimeActivityFrame.OpenFrameByActivityId(scoreActivityId);
            }
        }
        
        public void Show()
        {

        }

        public void UpdateData(ILimitTimeActivityModel data)
        {

        }

    }
}
