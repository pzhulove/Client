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

    public class TeamDuplicationMiddleStageRewardItem : MonoBehaviour
    {
        //翻牌动画和特效的路径
        private string rewardItemActionPath = "Effects/Scene_effects/EffectUI/chouka/EffUI_chouka_tuanben";
        private string rewardItemEffectPath = "Effects/Scene_effects/EffectUI/chouka/EffUI_chouka_tuanben_guangxiao";

        private int _stageId = 0;
        private bool _isInitRewardItem = false;

        private TeamDuplicationFightStageRewardDataModel _fightStageRewardDataModel;
        private CommonNewItem _commonNewItem;

        private bool _isBeginRewardAction = false;
        private float _rewardItemActionDuringTime = 0.0f;           //奖励翻牌动画持续时间

        [Space(10)]
        [HeaderAttribute("Index")]
        [Space(10)]
        [SerializeField] private int itemIndex;

        [Space(10)]
        [HeaderAttribute("ItemCover")]
        [Space(10)]
        [SerializeField] private GameObject itemCover;

        [Space(10)]
        [HeaderAttribute("rewardItem")]
        [Space(10)]
        [SerializeField] private GameObject rewardItemGameObject;
        [SerializeField] private Text playerNameLabel;
        [SerializeField] private GameObject rewardItemRoot;

        [Space(25)] [HeaderAttribute("RewardLimit")] [Space(10)]
        [SerializeField] private GameObject rewardLimitRoot;
        [SerializeField] private Text rewardLimitText;

        [Space(25)] [HeaderAttribute("RewardAction")] [Space(10)]
        [SerializeField] private GameObject rewardActionRoot;
        [SerializeField] private GameObject rewardEffectRoot;

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
            _fightStageRewardDataModel = null;
            _commonNewItem = null;
            _stageId = 0;
            _isInitRewardItem = false;
            _isBeginRewardAction = false;
            _rewardItemActionDuringTime = 0.0f;
        }

        public void Init(int stageId)
        {
            ClearData();

            _stageId = stageId;

            CommonUtility.UpdateGameObjectVisible(rewardItemGameObject, false);
        }

        public void UpdateRewardItem(TeamDuplicationFightStageRewardDataModel fightStageRewardDataModel)
        {
            _fightStageRewardDataModel = fightStageRewardDataModel;

            if (_fightStageRewardDataModel == null)
                return;

            _isInitRewardItem = true;

            CommonUtility.UpdateGameObjectVisible(itemCover, false);


            //玩家名字
            ShowRewardItemPlayerName();

            ShowRewardItemView();
            ShowRewardLimit();

            //首先隐藏
            CommonUtility.UpdateGameObjectVisible(rewardItemGameObject, false);
            //开始展示动画
            _isBeginRewardAction = true;
            _rewardItemActionDuringTime = TeamDuplicationDataManager.TeamDuplicationRewardItemActionDuringTime;

            CommonUtility.LoadGameObjectWithPath(rewardActionRoot, rewardItemActionPath);
            CommonUtility.LoadGameObjectWithPath(rewardEffectRoot, rewardItemEffectPath);
        }

        private void ShowRewardItemPlayerName()
        {
            if (playerNameLabel != null)
            {
                playerNameLabel.text = _fightStageRewardDataModel.PlayerName;
                //其他角色，名字的颜色为白色
                if (CommonUtility.IsSelfPlayerByPlayerGuid(_fightStageRewardDataModel.PlayerGuid) == false)
                    playerNameLabel.color = Color.white;
            }
        }

        //展示奖励道具
        private void ShowRewardItemView()
        {
            //奖励道具Id不大于0，则不展示，表示奖励道具不存在
            if (_fightStageRewardDataModel.RewardId <= 0)
                return;

            //奖励道具的图标
            var commonNewItemDataModel = new CommonNewItemDataModel()
            {
                ItemId = _fightStageRewardDataModel.RewardId,
                ItemCount = _fightStageRewardDataModel.RewardNum,
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
                _commonNewItem.SetItemCountFontSize(28);
            }
        }

        //展示限制
        private void ShowRewardLimit()
        {
            //不限制
            if (_fightStageRewardDataModel.IsLimit == 0)
            {
                CommonUtility.UpdateGameObjectVisible(rewardLimitRoot, false);
                return;
            }

            CommonUtility.UpdateGameObjectVisible(rewardLimitRoot, true);
            if (rewardLimitText == null)
                return;

            var limitDescription = TeamDuplicationUtility.GetStageLimitDescriptionByLimitType(
                _fightStageRewardDataModel.IsLimit);
            rewardLimitText.text = limitDescription;
        }

        //RewardItem的索引
        public int GetRewardItemIndex()
        {
            return itemIndex;
        }

        //RewardItem的状态
        public bool GetRewardItemInitState()
        {
            return _isInitRewardItem;
        }

        private void Update()
        {
            if (_isBeginRewardAction == false)
                return;

            _rewardItemActionDuringTime -= Time.deltaTime;
            //动画播放完全，展示奖励
            if (_rewardItemActionDuringTime <= 0.0f)
            {
                CommonUtility.UpdateGameObjectVisible(rewardItemGameObject, true);
                _isBeginRewardAction = false;
            }
        }


    }
}
