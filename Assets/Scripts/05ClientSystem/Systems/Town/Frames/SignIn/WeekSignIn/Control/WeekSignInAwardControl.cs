using System;
using System.Collections.Generic;
using System.Collections;
using Protocol;
using ProtoTable;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class WeekSignInAwardControl : MonoBehaviour
    {
        private WeekSignInType _weekSignInType = WeekSignInType.None;       //周签到类型

        private WeekSignInAwardDataModel _awardDataModel;
        private List<WeekSignSumTable> _weekSignSumTableList;

        private OpActivityData _opActivityData;
        private List<OpActTaskData> _weekSignInTaskDataList = new List<OpActTaskData>();

        [Space(15)]
        [HeaderAttribute("TaskAward")]
        [Space(15)]
        [SerializeField] private ComUIListScript taskAwardItemList;

        [Space(15)]
        [HeaderAttribute("TotalAward")]
        [Space(15)]
        [SerializeField] private Text alreadySignInWeek;
        [SerializeField] private Text currentSignInWeek;
        [SerializeField] private ComUIListScript totalAwardItemList;
        [SerializeField] private CommonTimeRefreshControl commonTimeRefreshControl;

        #region Init
        private void Awake()
        {
            BindComEvents();
        }

        private void OnDestroy()
        {
            UnBindComEvents();
            ClearData();
        }

        private void OnEnable()
        {
            BindUiEvents();
        }

        private void OnDisable()
        {
            UnBindUiEvents();
        }

        private void ClearData()
        {
            _weekSignInType = WeekSignInType.None;
            _awardDataModel = null;
            _opActivityData = null;
            _weekSignSumTableList = null;
            _weekSignInTaskDataList.Clear();
        }

        private void BindComEvents()
        {
            if (taskAwardItemList != null)
            {
                taskAwardItemList.Initialize();
                taskAwardItemList.onItemVisiable += OnTaskAwardItemVisible;
                taskAwardItemList.OnItemUpdate += OnTaskAwardItemUpdate;
            }

            if (totalAwardItemList != null)
            {
                totalAwardItemList.Initialize();
                totalAwardItemList.onItemVisiable += OnTotalAwardItemVisible;
                totalAwardItemList.OnItemUpdate += OnTotalAwardItemUpdate;
            }

            if (commonTimeRefreshControl != null)
                commonTimeRefreshControl.SetInvokeAction(UpdateCurrentWeekValue);

        }

        private void UnBindComEvents()
        {
            if (taskAwardItemList != null)
            {
                taskAwardItemList.onItemVisiable -= OnTaskAwardItemVisible;
                taskAwardItemList.OnItemUpdate -= OnTaskAwardItemUpdate;
            }

            if (totalAwardItemList != null)
            {
                totalAwardItemList.onItemVisiable -= OnTotalAwardItemVisible;
                totalAwardItemList.OnItemUpdate -= OnTotalAwardItemUpdate;
            }

            if (commonTimeRefreshControl != null)
                commonTimeRefreshControl.ResetInvokeAction();
        }

        private void BindUiEvents()
        {
            //累计周数事件
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSyncSceneWeekSignInNotify,
                OnSyncSceneWeekSignInNotify);
            //活动更新事件
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityLimitTimeTaskUpdate,
                OnReceiveActivityLimitTimeTaskUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityLimitTimeTaskDataUpdate,
                OnReceiveActivityLimitTimeTaskDataUpdate);
        }

        private void UnBindUiEvents()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSyncSceneWeekSignInNotify,
                OnSyncSceneWeekSignInNotify);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityLimitTimeTaskUpdate,
                OnReceiveActivityLimitTimeTaskUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityLimitTimeTaskDataUpdate,
                OnReceiveActivityLimitTimeTaskDataUpdate);
        }

        public void InitAwardControl(WeekSignInType weekSignInType)
        {
            _weekSignInType = weekSignInType;

            if (_weekSignInType == WeekSignInType.None)
            {
                Logger.LogErrorFormat("WeekSingInType is None");
                return;
            }

            _opActivityData = WeekSignInDataManager.GetInstance()
                .GetWeekSignInOpActivityDataByWeekSignInType(_weekSignInType);

            if (_opActivityData == null)
            {
                Logger.LogErrorFormat("OPActivityData is null and WeekSignInType is {0}", _weekSignInType);
                return;
            }

            _awardDataModel = WeekSignInDataManager.GetInstance().GetWeekSignInAwardDataModelByWeekSignInType(
                _weekSignInType);

            if (_awardDataModel == null)
            {
                Logger.LogErrorFormat("WeekSingInAwardDataModel is null and weekSingInType is {0}",
                    _weekSignInType);
                return;
            }

            _weekSignSumTableList = _awardDataModel.WeekSignSumTableList;

            InitAwardControlView();
        }

        //再次显示的时候
        public void OnEnableAwardControl()
        {
            if (_awardDataModel == null)
                return;

            //同步周数
            UpdateWeekSignInWeekNumberValue();

            //任务奖励更新
            if(taskAwardItemList != null)
                taskAwardItemList.UpdateElement();

            //累计奖励更新
            if(totalAwardItemList != null)
                totalAwardItemList.UpdateElement();

        }

        private void InitAwardControlView()
        {

            UpdateWeekSignInTaskAwardItemList();
            
            UpdateWeekSignInWeekNumberValue();
            UpdateWeekSignInTotalAwardItemList();
            //需要对List进行调整
            ResetTotalAwardItemList();
        }

        #endregion

        #region TaskAwardItemList

        //任务奖励的ItemList
        private void UpdateWeekSignInTaskAwardItemList()
        {
            _weekSignInTaskDataList.Clear();
            if (_opActivityData == null || _opActivityData.tasks == null)
                return;

            for (var i = 0; i < _opActivityData.tasks.Length; i++)
                _weekSignInTaskDataList.Add(_opActivityData.tasks[i]);

            
            var taskDataNumber = _weekSignInTaskDataList.Count;
            if (taskAwardItemList != null)
                taskAwardItemList.SetElementAmount(taskDataNumber);

        }

        private void OnTaskAwardItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (taskAwardItemList == null)
                return;

            if (_weekSignInTaskDataList == null
               || _weekSignInTaskDataList.Count <= 0)
                return;
            
            if (item.m_index < 0 || item.m_index >= _weekSignInTaskDataList.Count)
                return;

            var taskDataModel = _weekSignInTaskDataList[item.m_index];
            var taskAwardItem = item.GetComponent<WeekSignInTaskAwardItem>();
            if (taskDataModel != null && taskAwardItem != null)
                taskAwardItem.InitItem(_weekSignInType, _opActivityData.dataId, taskDataModel);
        }

        private void OnTaskAwardItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var taskAwardItem = item.GetComponent<WeekSignInTaskAwardItem>();
            if (taskAwardItem != null)
                taskAwardItem.OnItemRecycle();
        }


        private void OnTaskAwardItemUpdate(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var taskAwardItem = item.GetComponent<WeekSignInTaskAwardItem>();
            if (taskAwardItem != null)
            {
                taskAwardItem.OnItemUpdate();
            }
        }

        #endregion

        #region TotalAwardWeekNumberValue

        //累计的周数
        private void UpdateWeekSignInWeekNumberValue()
        {
            UpdateAlreadySignInWeek();
            UpdateCurrentWeekValue();
        }

        //当前周数
        private void UpdateCurrentWeekValue()
        {
            if (_awardDataModel == null)
                return;

            if (_opActivityData == null)
                return;

            //当前周数/总周数
            var curWeekNumber = TimeUtility.GetWeekNumberBetweenTime(_opActivityData.startTime,
                TimeManager.GetInstance().GetServerTime());
            var totalWeekNumber = TimeUtility.GetWeekNumberBetweenTime(_opActivityData.startTime,
                _opActivityData.endTime);

            if (currentSignInWeek != null)
                currentSignInWeek.text = string.Format(TR.Value("week_sing_in_now_week"), curWeekNumber,
                    totalWeekNumber);
        }

        //已经签到周数
        private void UpdateAlreadySignInWeek()
        {
            if (_awardDataModel == null)
                return;

            if (_opActivityData == null)
                return;

            //当前周数/总周数
            var curWeekNumber = TimeUtility.GetWeekNumberBetweenTime(_opActivityData.startTime,
                TimeManager.GetInstance().GetServerTime());

            //已经签到周数
            if (alreadySignInWeek != null)
            {
                if (_awardDataModel.AlreadySignInWeek < curWeekNumber)
                {
                    alreadySignInWeek.text = string.Format(TR.Value("week_sing_in_get_less_week"), _awardDataModel.AlreadySignInWeek);
                }
                else
                {
                    alreadySignInWeek.text = string.Format(TR.Value("week_sign_in_get_week"), _awardDataModel.AlreadySignInWeek);
                }
            }
        }

        #endregion

        #region TotalAwardItemList
        //累计奖励的ItemList
        private void UpdateWeekSignInTotalAwardItemList()
        {
            var totalAwardNumber = 0;
            if (_weekSignSumTableList != null)
                totalAwardNumber = _weekSignSumTableList.Count;

            if (totalAwardItemList != null)
                totalAwardItemList.SetElementAmount(totalAwardNumber);
        }

        private void OnTotalAwardItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (_awardDataModel == null)
                return;

            if (totalAwardItemList == null)
                return;

            if (_weekSignSumTableList == null
                || _weekSignSumTableList.Count <= 0)
                return;

            if (item.m_index < 0 || item.m_index >= _weekSignSumTableList.Count)
                return;

            var weekSignSumTable = _weekSignSumTableList[item.m_index];
            var totalAwardItem = item.GetComponent<WeekSignInTotalAwardItem>();
            if (weekSignSumTable != null && totalAwardItem != null)
            {
                totalAwardItem.InitItem(_weekSignInType, weekSignSumTable);
            }
        }

        //第一个为领取的奖励处在第一个
        private void ResetTotalAwardItemList()
        {
            if (_awardDataModel == null)
                return;

            if (totalAwardItemList == null)
                return;

            if (_weekSignSumTableList == null)
                return;

            var totalAwardNumber = _weekSignSumTableList.Count;
            
            var firstIndex = WeekSignInUtility.GetFirstTotalAwardItemIndex(_weekSignInType);

            if (firstIndex <= 1)
            {
                totalAwardItemList.MoveElementInScrollArea(0, true);
            }
            else if (firstIndex >= totalAwardNumber - 2)
            {
                totalAwardItemList.MoveElementInScrollArea(totalAwardNumber - 1, true);
            }
            else
            {
                totalAwardItemList.MoveElementInScrollArea(firstIndex + 1, true);
            }
        }

        private void OnTotalAwardItemUpdate(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var totalAwardItem = item.GetComponent<WeekSignInTotalAwardItem>();
            if (totalAwardItem != null)
                totalAwardItem.OnItemUpdate();
        }

        #endregion

        #region UIEvent

        //周数和已完成的周数进行同步
        private void OnSyncSceneWeekSignInNotify(UIEvent uiEvent)
        {
            UpdateWeekSignInWeekNumberValue();
            UpdateWeekSignInTotalAwardItemList();
        }

        //运营活动的任务数据发生改变
        private void OnReceiveActivityLimitTimeTaskDataUpdate(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var data = (LimitTimeActivityTaskUpdateData)uiEvent.Param1;
            ReceiveActivityLimitTimeTaskInfoUpdate(data);
        }

        //运营活动的任务状态发生改变
        private void OnReceiveActivityLimitTimeTaskUpdate(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var data = (LimitTimeActivityTaskUpdateData)uiEvent.Param1;
            ReceiveActivityLimitTimeTaskInfoUpdate(data);
        }

        private void ReceiveActivityLimitTimeTaskInfoUpdate(LimitTimeActivityTaskUpdateData data)
        {
            if (data == null)
                return;

            if (_opActivityData != null && _opActivityData.dataId == data.ActivityId)
            {
                UpdateWeekSignInWeekNumberValue();
            }
        }

        #endregion


    }
}