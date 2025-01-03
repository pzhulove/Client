using System;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class ArborDayRewardItem : MonoBehaviour
    {

        private ILimitTimeActivityTaskDataModel _activityTaskDataModel;

        private uint _activityId = 0;         //活动Id
        private uint _activityTaskId = 0;     //任务Id

        private int _rewardItemId;
        private ItemTable _rewardItemTable;

        private List<uint> _rewardProgressItemIdList = new List<uint>();
        private List<string> _rewardProgressItemStrList = new List<string>();

        [Space(10)]
        [HeaderAttribute("TreeRoot")]
        [Space(10)]
        [SerializeField] private Text rewardItemNameText;
        [SerializeField] private GameObject rewardItemRoot;

        [Space(10)]
        [HeaderAttribute("Button")]
        [Space(10)]
        [SerializeField] private Text rewardItemButtonText;
        [SerializeField] private Button rewardItemButton;
        [SerializeField] private UIGray rewardItemButtonGray;
        [SerializeField] private Button rewardPreviewButton;

        private void Awake()
        {
            if (rewardItemButton != null)
            {
                rewardItemButton.onClick.RemoveAllListeners();
                rewardItemButton.onClick.AddListener(OnRewardItemButtonClicked);
            }

            if (rewardPreviewButton != null)
            {
                rewardPreviewButton.onClick.RemoveAllListeners();
                rewardPreviewButton.onClick.AddListener(OnRewardPreviewButtonClicked);
            }
        }

        private void OnDestroy()
        {
            if(rewardItemButton != null)
                rewardItemButton.onClick.RemoveAllListeners();

            if(rewardPreviewButton != null)
                rewardPreviewButton.onClick.RemoveAllListeners();
        }

        private void ClearData()
        {
            _rewardItemId = 0;
            _rewardItemTable = null;
            _activityTaskDataModel = null;
            _rewardProgressItemIdList.Clear();
            _rewardProgressItemStrList.Clear();

            _activityId = 0;
            _activityTaskId = 0;
        }

        public void InitItem(ILimitTimeActivityTaskDataModel taskDataModel,
            uint activityId = 0)
        {
            _activityTaskDataModel = taskDataModel;
            _activityId = activityId;

            if (_activityTaskDataModel == null)
            {
                return;
            }

            _activityTaskId = _activityTaskDataModel.DataId;

            _rewardItemId = ArborDayUtility.GetRewardItemId(_activityTaskDataModel);
            _rewardItemTable = TableManager.GetInstance().GetTableItem<ItemTable>(_rewardItemId);
            if (_rewardItemTable == null)
                return;

            //if (_activityTaskDataModel.ParamNums != null)
            //{
            //    for (var i = 0; i < _activityTaskDataModel.ParamNums.Count; i++)
            //    {
            //        var curParamItemId = _activityTaskDataModel.ParamNums[i];
            //        _rewardProgressItemIdList.Add(curParamItemId);
            //    }
            //}

            InitView();
        }

        //只是更新状态
        public void UpdateItem(ILimitTimeActivityTaskDataModel taskDataModel)
        {
            _activityTaskDataModel = taskDataModel;

            UpdateRewardState();
        }

        private void InitView()
        {
            InitItemBaseView();

            UpdateRewardState();
        }

        private void InitItemBaseView()
        {
            if (rewardItemNameText != null)
                rewardItemNameText.text = CommonUtility.GetItemColorName(_rewardItemTable);

            if (rewardItemRoot != null)
            {
                var commonNewItem = rewardItemRoot.GetComponentInChildren<CommonNewItem>();
                if (commonNewItem == null)
                {
                    commonNewItem = CommonUtility.CreateCommonNewItem(rewardItemRoot);
                }

                if (commonNewItem != null)
                    commonNewItem.InitItem(_rewardItemId);
            }
        }

        private void UpdateRewardState()
        {
            if (_activityTaskDataModel == null)
                return;

            var taskState = _activityTaskDataModel.State;

            var itemButtonStr = TR.Value("Arbor_Day_Reward_Can_Receive");

            if (taskState == OpActTaskState.OATS_OVER)
            {
                //结束：已经领取,显示已领取奖励按钮并置灰
                itemButtonStr = TR.Value("Arbor_Day_Reward_Already_Receive");
                CommonUtility.UpdateButtonVisible(rewardItemButton, true);
                CommonUtility.UpdateButtonState(rewardItemButton, rewardItemButtonGray, false);
                
                //预览按钮隐藏
                CommonUtility.UpdateButtonVisible(rewardPreviewButton, false);
            }
            else if (taskState == OpActTaskState.OATS_FINISHED)
            {
                //完成：可以领取,显示领取奖励按钮，并可以点击
                CommonUtility.UpdateButtonVisible(rewardItemButton, true);
                CommonUtility.UpdateButtonState(rewardItemButton, rewardItemButtonGray, true);

                //预览按钮隐藏
                CommonUtility.UpdateButtonVisible(rewardPreviewButton, false);
            }
            else
            {
                //其他状态，只能预览
                //奖励按钮隐藏
                CommonUtility.UpdateButtonVisible(rewardItemButton, false);

                //展示预览按钮
                CommonUtility.UpdateButtonVisible(rewardPreviewButton, true);
            }

            if (rewardItemButtonText != null)
                rewardItemButtonText.text = itemButtonStr;
        }

        #region ButtonClicked
        //领取奖励
        private void OnRewardItemButtonClicked()
        {
            ActivityDataManager.GetInstance().RequestOnTakeActTask(
                _activityId,
                _activityTaskId);
        }

        //奖励预览
        private void OnRewardPreviewButtonClicked()
        {
            if (_rewardItemId <= 0)
                return;

            ArborDayUtility.OnOpenRewardReviewFrame(_rewardItemId);

        }

        #endregion

    }
}
