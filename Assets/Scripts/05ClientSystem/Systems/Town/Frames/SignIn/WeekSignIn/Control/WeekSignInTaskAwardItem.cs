using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{

    public class WeekSignInTaskAwardItem : MonoBehaviour
    {
        private WeekSignInType _weekSignInType;

        private OpActTaskData _opActTaskData;   //任务的表格数据,  不变的量
        private OpActTask _opActTask;       //任务的相关数据，状态和数量，可能会发生改变

        private uint _opActId;           //运营活动ID
        private uint _opActTaskId;   //运营活动下面的具体任务ID

        private CommonNewItem _commonNewItem;
        private WeekSignBox _weekSignBox;           //奖励数据

        private WeekSignInAwardState _taskAwardState;

        private string _opActTaskDes;

        private BoxOpenType _boxOpenType;

        [Space(10)]
        [HeaderAttribute("item")]
        [Space(10)]
        [SerializeField] private GameObject itemRoot;
        [SerializeField] private Text itemName;

        [Space(10)]
        [HeaderAttribute("Description")]
        [Space(10)]
        [SerializeField] private Text taskAwardDescription;
        [SerializeField] private GameObject taskAwardReceivedFlag;

        [Space(10)]
        [HeaderAttribute("Flag")]
        [Space(10)]
        [SerializeField] private GameObject taskAwardFinishFlag;
        [SerializeField] private Image taskAwardBoxImage;
        [SerializeField] private Button taskAwardButton;
        [SerializeField] private DOTweenAnimation boxTweenAnimation;

        private void Awake()
        {
            BindComEvents();
        }

        private void OnDestroy()
        {
            UnBindComEvents();
            ClearData();
        }

        #region Init
        private void OnEnable()
        {
            BindUiEvents();
        }

        private void OnDisable()
        {
            UnBindUiEvents();
            _boxOpenType = BoxOpenType.None;
        }

        private void BindUiEvents()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityLimitTimeTaskUpdate, OnOpActTaskUpdate);
            UIEventSystem.GetInstance()
                .RegisterEventHandler(EUIEventID.ActivityLimitTimeTaskDataUpdate, OnOpActTaskDataUpdate);
            UIEventSystem.GetInstance()
                .RegisterEventHandler(EUIEventID.OnSyncSceneWeekSignBoxNotify, OnSyncSceneWeekSignBoxNotify);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnBoxOpenFinished, OnBoxOpenFinished);
        }

        private void UnBindUiEvents()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityLimitTimeTaskUpdate, OnOpActTaskUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityLimitTimeTaskDataUpdate, OnOpActTaskDataUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSyncSceneWeekSignBoxNotify, OnSyncSceneWeekSignBoxNotify);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnBoxOpenFinished, OnBoxOpenFinished);
        }

        private void BindComEvents()
        {
            if (taskAwardButton != null)
            {
                taskAwardButton.onClick.RemoveAllListeners();
                taskAwardButton.onClick.AddListener(OnTaskAwardButtonClick);
            }
        }

        private void UnBindComEvents()
        {
            if (taskAwardButton != null)
                taskAwardButton.onClick.RemoveAllListeners();
        }

        private void ClearData()
        {
            _opActTaskData = null;
            _opActTask = null;
            _commonNewItem = null;
            _weekSignBox = null;
            _opActTaskId = 0;
            _opActTaskDes = null;
            _boxOpenType = BoxOpenType.None;
        }

        public void InitItem(WeekSignInType weekSignInType,uint opActId,OpActTaskData opActTaskData)
        {
            _weekSignInType = weekSignInType;
            _opActId = opActId;
            _opActTaskData = opActTaskData;

            if (_opActTaskData == null)
            {
                Logger.LogErrorFormat("OpActTaskData is null ");
                return;
            }
            _opActTaskId = _opActTaskData.dataid;


            _opActTask = ActivityDataManager.GetInstance().GetLimitTimeTaskData(_opActTaskId);
            if (_opActTask == null)
            {
                Logger.LogErrorFormat("OpActTask is null");
                return;
            }

            _opActTaskDes = ActivityDataManager.GetInstance().GetTaskDesByTaskId(_opActTaskId, _opActId);

            InitTaskAwardItemBoxImage();

            UpdateItemView();
        }

        private void InitTaskAwardItemBoxImage()
        {
            if (taskAwardBoxImage != null)
            {
                if (_weekSignInType == WeekSignInType.NewPlayerWeekSignIn)
                {
                    ETCImageLoader.LoadSprite(ref taskAwardBoxImage,
                        WeekSignInDataManager.NewPlayerTaskAwardBoxImagePath);
                }
                else
                {
                    ETCImageLoader.LoadSprite(ref taskAwardBoxImage,
                        WeekSignInDataManager.ActivityTaskAwardBoxImagePath);
                }
            }
        }

        public void OnItemRecycle()
        {
            ClearData();
        }

        //Item再次显示时，调用
        public void OnItemUpdate()
        {
            if (_opActTaskData == null)
            {
                return;
            }

            UpdateItemByChanged();
        }
        #endregion

        #region UpdateItemView
        private void UpdateItemView()
        {
            switch ((OpActTaskState)_opActTask.state)
            {
                case OpActTaskState.OATS_OVER:
                    _taskAwardState = WeekSignInAwardState.Received;
                    UpdateReceivedItemView();
                    break;
                case OpActTaskState.OATS_FINISHED:
                    _taskAwardState = WeekSignInAwardState.Finished;
                    UpdateFinishedItemView();
                    break;
                default:
                    _taskAwardState = WeekSignInAwardState.UnFinished;
                    UpdateUnFinishedItemView();
                    break;
            }
        }

        private void UpdateTaskAwardDescription()
        {

            if (taskAwardDescription != null)
            {
                if (string.IsNullOrEmpty(_opActTaskDes) == false)
                {
                    var taskDescStr = string.Format(_opActTaskDes, _opActTask.curNum, _opActTaskData.completeNum);
                    taskAwardDescription.text = taskDescStr;
                }
            }
        }

        //领取任务奖励的状态
        private void UpdateReceivedItemView()
        {
            UpdateAwardItemFlag(true);

            UpdateBoxFinishTweenAnimation(false);
            UpdateFinishFlag(false);

            UpdateReceivedFlag(true);

            UpdateTaskAwardDescription();

            _weekSignBox = WeekSignInDataManager.GetInstance().GetWeekSignBoxByWeekSignInType(_weekSignInType,
                (int)_opActTaskId);

            UpdateReceivedItemBox();
        }

        //更新奖励相关内容
        private void UpdateAwardItemFlag(bool flag)
        {
            if (itemName != null)
                itemName.gameObject.CustomActive(flag);

            if (itemRoot != null)
                itemRoot.gameObject.CustomActive(flag);
        }

        //更新宝箱的奖励
        private void UpdateReceivedItemBox()
        {
            if (_weekSignBox != null && _weekSignBox.itemVec.Length > 0)
            {
                var curItemReward = _weekSignBox.itemVec[0];


                var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>((int)curItemReward.id);
                if (itemTable != null)
                {
                    var itemNameStr = CommonUtility.GetItemColorName(itemTable);
                    if (itemName != null)
                        itemName.text = itemNameStr;
                }

                if (itemRoot != null)
                {
                    _commonNewItem = itemRoot.GetComponentInChildren<CommonNewItem>();
                    if (_commonNewItem == null)
                        _commonNewItem = CommonUtility.CreateCommonNewItem(itemRoot);

                    if (_commonNewItem != null)
                    {
                        var commonNewItemData = new CommonNewItemDataModel()
                        {
                            ItemId = (int)curItemReward.id,
                            ItemCount = (int)curItemReward.num,
                        };
                        _commonNewItem.InitItem(commonNewItemData);
                    }
                }
            }
        }


        //完成任务奖励的状态
        private void UpdateFinishedItemView()
        {
            UpdateAwardItemFlag(false);

            UpdateReceivedFlag(true);

            UpdateBoxFinishTweenAnimation(true);
            UpdateFinishFlag(true);

            UpdateTaskAwardDescription();
        }

        //未完成任务奖励的状态
        private void UpdateUnFinishedItemView()
        {
            UpdateAwardItemFlag(false);

            UpdateReceivedFlag(false);
            UpdateFinishFlag(true);

            UpdateBoxFinishTweenAnimation(false);
            if (taskAwardFinishFlag != null)
                taskAwardFinishFlag.gameObject.CustomActive(false);
            UpdateTaskAwardDescription();
        }

        //可以领取的对钩
        private void UpdateReceivedFlag(bool flag)
        {
            if (taskAwardReceivedFlag != null)
                taskAwardReceivedFlag.gameObject.CustomActive(flag);
        }

        //完成的状态
        private void UpdateFinishFlag(bool flag)
        {
            if (taskAwardFinishFlag != null)
                taskAwardFinishFlag.CustomActive(flag);

            if (taskAwardBoxImage != null)
                taskAwardBoxImage.CustomActive(flag);

            if (taskAwardButton != null)
                taskAwardButton.gameObject.CustomActive(flag);
        }
        #endregion

        #region UpdateItemByUIEvent

        //开箱完成动画
        private void OnBoxOpenFinished(UIEvent uiEvent)
        {
            if (_boxOpenType != BoxOpenType.Opening)
                return;
            _boxOpenType = BoxOpenType.Finished;

            UpdateItemByChanged();
        }

        //任务奖励接受到
        private void OnSyncSceneWeekSignBoxNotify(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var weekSignType = (WeekSignInType)uiEvent.Param1;

            if (weekSignType != _weekSignInType)
                return;

            var weekSignBox = WeekSignInDataManager.GetInstance().GetWeekSignBoxByWeekSignInType(_weekSignInType,
                (int)_opActTaskId);

            if (weekSignBox != null)
            {
                if (_boxOpenType == BoxOpenType.Preparing)
                {
                    if (weekSignBox.itemVec != null && weekSignBox.itemVec.Length > 0)
                    {
                        _boxOpenType = BoxOpenType.Opening;
                        WeekSignInUtility.OpenBoxOpenFrame(_weekSignInType,
                            (int)weekSignBox.itemVec[0].id,
                            (int)weekSignBox.itemVec[0].num);
                        return;
                    }
                }
                else if (_boxOpenType == BoxOpenType.Opening)
                {
                    return;
                }

                _weekSignBox = weekSignBox;
                UpdateReceivedItemBox();
            }
        }

        //任务状态发生改变
        private void OnOpActTaskUpdate(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var data = (LimitTimeActivityTaskUpdateData)uiEvent.Param1;
            if (data == null)
                return;

            if (data.ActivityId != _opActId || data.TaskId != _opActTaskId)
                return;

            if (_boxOpenType == BoxOpenType.Preparing || _boxOpenType == BoxOpenType.Opening)
                return;

            UpdateItemByChanged();
        }

        //任务数据发生改变
        private void OnOpActTaskDataUpdate(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var data = (LimitTimeActivityTaskUpdateData)uiEvent.Param1;
            if (data == null)
                return;

            if (data.ActivityId != _opActId || data.TaskId != _opActTaskId)
                return;

            if (_boxOpenType == BoxOpenType.Preparing || _boxOpenType == BoxOpenType.Opening)
                return;

            UpdateItemByChanged();
        }

        //判断任务状态或者数据是否发生改变，如果改变，则重置任务，并更新Item
        private void UpdateItemByChanged()
        {
            //获得最新的任务数据
            var currentOpActTask = ActivityDataManager.GetInstance().GetLimitTimeTaskData(_opActTaskId);

            if (currentOpActTask == null)
                return;

            //数据重置并更新相应的View
            _opActTask = currentOpActTask;
            UpdateItemView();
        }
        #endregion

        #region ItemClicked

        private void OnTaskAwardButtonClick()
        {
            if (_opActTaskData == null)
                return;

            if (_opActTask == null)
                return;

            if (_taskAwardState == WeekSignInAwardState.UnFinished)
            {
                //未完成
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("week_sing_in_task_unfinished"));
                return;
            }
            else if (_taskAwardState == WeekSignInAwardState.Finished)
            {
                if (_boxOpenType == BoxOpenType.None)
                    _boxOpenType = BoxOpenType.Preparing;

                //已经领取，播放动画，并领取奖励
                WeekSignInDataManager.GetInstance().OnSendRequestOnTakeActTask(_opActId, _opActTaskId);
            }
        }
        
        #endregion

        //更新罐子可以领取的动画效果
        private void UpdateBoxFinishTweenAnimation(bool flag)
        {
            if (boxTweenAnimation != null)
            {
                if (flag == true)
                {
                    boxTweenAnimation.tween.Restart();
                }
                else
                {
                    boxTweenAnimation.tween.Pause();
                }
            }
        }

    }
}
