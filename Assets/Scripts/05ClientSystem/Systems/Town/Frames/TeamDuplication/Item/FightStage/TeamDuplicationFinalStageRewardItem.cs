using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;
using Scripts.UI;

namespace GameClient
{

    public class TeamDuplicationFinalStageRewardItem : MonoBehaviour
    {
        //金牌背景的路径
        private string goldRewardItemBgPath =
            "UI/Image/Background/UI_Tuanben_Fanpai_Jinpai.png:UI_Tuanben_Fanpai_Jinpai";

        private bool _isInitRewardItem = false;

        private TeamDuplicationFightStageRewardDataModel _finalStageRewardDataModel;
        private CommonNewItem _commonNewItem;

        [Space(10)] [HeaderAttribute("RewardItemBg")] [Space(10)]
        [SerializeField] private Image rewardItemBgImage;

        [Space(10)]
        [HeaderAttribute("rewardItem")]
        [Space(10)]
        [SerializeField] private Text playerNameLabel;
        [SerializeField] private GameObject rewardItemRoot;

        [Space(25)]
        [HeaderAttribute("RewardLimit")]
        [Space(10)]
        [SerializeField] private GameObject rewardLimitRoot;
        [SerializeField] private Text rewardLimitText;

        private void Awake()
        {
            BindUiEvents();
        }

        private void OnDestroy()
        {
            UnBindUiEvents();
            ClearData();
        }

        private void BindUiEvents()
        {
        }

        private void UnBindUiEvents()
        {
        }

        private void ClearData()
        {
            _finalStageRewardDataModel = null;
            _commonNewItem = null;
        }

        //初始化
        public void Init(int stageId)
        {
            ClearData();
        }

        public void UpdateRewardItem(TeamDuplicationFightStageRewardDataModel finalStageRewardDataModel)
        {
            _finalStageRewardDataModel = finalStageRewardDataModel;

            if (_finalStageRewardDataModel == null)
                return;

            ShowRewardItemView();
        }

        private void ShowRewardItemView()
        {
            //玩家名字
            ShowRewardItemPlayerName();

            //奖励道具的图标和金牌
            UpdateRewardItem();

            //是否奖励已经领取完全
            ShowRewardLimit();
        }

        private void ShowRewardItemPlayerName()
        {
            if (playerNameLabel == null)
                return;

            playerNameLabel.text = _finalStageRewardDataModel.PlayerName;

            //其他玩家，字体颜色为白色
            if(CommonUtility.IsSelfPlayerByPlayerGuid(_finalStageRewardDataModel.PlayerGuid) == false)
            {
                playerNameLabel.color = Color.white;
            }
        }

        private void UpdateRewardItem()
        {
            //奖励不存在,奖励受到限制
            if (_finalStageRewardDataModel.IsLimit != TeamCopyFlopLimit.TEAM_COPY_FLOP_LIMIT_NULL)
            {
                CommonUtility.UpdateGameObjectVisible(rewardItemRoot, false);
                return;
            }

            //金牌，背景换成特殊的金牌
            if (_finalStageRewardDataModel.IsGoldReward == true)
            {
                if (rewardItemBgImage != null)
                {
                    ETCImageLoader.LoadSprite(ref rewardItemBgImage, goldRewardItemBgPath);
                }
            }

            //是否为金奖的标志
            CommonUtility.UpdateGameObjectVisible(rewardItemRoot, true);

            if (_finalStageRewardDataModel.RewardId <= 0)
                return;

            //奖励道具的图标
            var commonNewItemDataModel = new CommonNewItemDataModel()
            {
                ItemId = _finalStageRewardDataModel.RewardId,
                ItemCount = _finalStageRewardDataModel.RewardNum,
            };

            if (rewardItemRoot != null)
            {
                _commonNewItem = rewardItemRoot.GetComponentInChildren<CommonNewItem>();
                if (_commonNewItem == null)
                    _commonNewItem = CommonUtility.CreateCommonNewItem(rewardItemRoot);
            }

            if (_commonNewItem != null)
            {
                _commonNewItem.InitItem(commonNewItemDataModel);
                _commonNewItem.SetItemCountFontSize(26);
            }
        }

        private void ShowRewardLimit()
        {
            //不限制
            if (_finalStageRewardDataModel.IsLimit == TeamCopyFlopLimit.TEAM_COPY_FLOP_LIMIT_NULL)
            {
                CommonUtility.UpdateGameObjectVisible(rewardLimitRoot, false);
                return;
            }

            CommonUtility.UpdateGameObjectVisible(rewardLimitRoot, true);
            if (rewardLimitText == null)
                return;

            var limitDescription = TeamDuplicationUtility.GetStageLimitDescriptionByLimitType(
                _finalStageRewardDataModel.IsLimit);
            rewardLimitText.text = limitDescription;

        }
    }
}
