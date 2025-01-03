using System;
using ProtoTable;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class ArborDayRewardController : MonoBehaviour
    {

        private ILimitTimeActivityModel _model;

        private ILimitTimeActivityTaskDataModel _firstTaskDataModel;

        private int _firstRewardItemId = 0;

        [SerializeField] private Text rewardDescriptionLabel;

        [Space(10)] [HeaderAttribute("Reward")] [Space(10)]
        [SerializeField] private ArborDayRewardItem firstRewardItem;

        //初始化
        public void InitRewardController(ILimitTimeActivityModel model)
        {
            _model = model;
            if (_model == null)
                return;

            var taskDataList = _model.TaskDatas;
            if (taskDataList == null || taskDataList.Count <= 0)
                return;

            _firstTaskDataModel = taskDataList[0];

            if (_firstTaskDataModel == null)
            {
                Logger.LogErrorFormat("RewardTaskDataModel is null");
                return;
            }

            _firstRewardItemId = ArborDayUtility.GetRewardItemId(_firstTaskDataModel);
            
            InitRewardDescription();

            InitRewardItem();
        }

        //更新

        public void OnUpdateRewardController(ILimitTimeActivityModel activityModel)
        {

            if (activityModel == null
                || activityModel.TaskDatas == null
                || activityModel.TaskDatas.Count <= 0)
                return;

            _firstTaskDataModel = activityModel.TaskDatas[0];

            if (firstRewardItem != null)
                firstRewardItem.UpdateItem(_firstTaskDataModel);
        }

        private void InitRewardDescription()
        {
            if (rewardDescriptionLabel == null)
                return;
            
            //第一个奖励
            var firstRewardDesc = _firstTaskDataModel.Desc;
            var firstRewardItemName = ArborDayUtility.GetRewardItemName(_firstRewardItemId);

            var descriptionStr = string.Format(TR.Value("Arbor_Day_Reward_Detail_Label"),
                firstRewardDesc,
                firstRewardItemName);

            rewardDescriptionLabel.text = descriptionStr;
        }

        private void InitRewardItem()
        {
            if (firstRewardItem != null)
                firstRewardItem.InitItem(_firstTaskDataModel,
                    _model.Id);
        }
        
    }
}
