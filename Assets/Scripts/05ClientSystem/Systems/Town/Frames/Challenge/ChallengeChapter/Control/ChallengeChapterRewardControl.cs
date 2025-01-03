using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{

    public class ChallengeChapterRewardControl : MonoBehaviour
    {

        private int _dungeonId;             //地下城ID， 判断是否存在奖励

        private ChallengeDungeonRewardDataModel _rewardDataModel;

        [SerializeField] private Text rewardTitleLabel;
        [SerializeField] private Text rewardValueLabel;

        [SerializeField] private GameObject rewardEffect;
        [SerializeField] private Button rewardButton;
        
        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }

        private void BindEvents()
        {
            if (rewardButton != null)
            {
                rewardButton.onClick.RemoveAllListeners();
                rewardButton.onClick.AddListener(OnRewardButtonClick);
            }
        }

        private void UnBindEvents()
        {
            if (rewardButton != null)
                rewardButton.onClick.RemoveAllListeners();
        }

        private void OnChallengeDungeonRewardUpdate(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            int updateDungeonId = (int) uiEvent.Param1;

            if (_dungeonId != updateDungeonId)
                return;

            _rewardDataModel = ChallengeDataManager.GetInstance().GetChallengeDungeonRewardDataByDungeonId(_dungeonId);
            if (_rewardDataModel == null)
                return;
            UpdateRewardControl();
        }

        private void ClearData()
        {
            _dungeonId = 0;
            _rewardDataModel = null;
        }

        public void InitRewardControl(int dungeonId)
        {
            _dungeonId = dungeonId;

            _rewardDataModel = ChallengeDataManager.GetInstance().GetChallengeDungeonRewardDataByDungeonId(dungeonId);

            if (_rewardDataModel == null)
            {
                gameObject.CustomActive(false);
                return;
            }

            if (rewardTitleLabel != null)
                rewardTitleLabel.text = TR.Value("challenge_pass_number_label");

            gameObject.CustomActive(true);

            UpdateRewardControl();
        }

        private void UpdateRewardControl()
        {
            if (rewardValueLabel != null)
                rewardValueLabel.text = string.Format(TR.Value("challenge_pass_number_value"),
                    _rewardDataModel.ChallengeNumber, _rewardDataModel.TotalNumber);

            bool isCanReceiveAward = _rewardDataModel.ChallengeNumber >= _rewardDataModel.TotalNumber;
            if (rewardEffect != null)
                rewardEffect.CustomActive(isCanReceiveAward);
        }
        
        private void OnRewardButtonClick()
        {
            if (_rewardDataModel == null)
            {
                Logger.LogErrorFormat("RewardDataModel is null");
                return;
            }

            ChallengeUtility.OnOpenChallengeDungeonRewardFrame(_rewardDataModel);
        }

    }
}
