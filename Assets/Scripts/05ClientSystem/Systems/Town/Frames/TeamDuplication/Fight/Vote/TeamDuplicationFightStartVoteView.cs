using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class TeamDuplicationFightStartVoteView : MonoBehaviour
    {

        //倒计时
        private float _countDownTotalTime = 0;
        private float _countDownLeftTime = 0;

        private int _closeFrameIntervalTime = 3;            //倒计时结束3s后，强制关闭界面

        //是否投票同意
        private bool _isAlreadyAgree = false;

        private List<TeamDuplicationCaptainDataModel> _captainDataModelList;

        [Space(15)]
        [HeaderAttribute("Common")]
        [Space(10)]
        [SerializeField] private Text titleLabel;

        [SerializeField] private Button closeButton;

        [Space(15)]
        [HeaderAttribute("ComUIListScript")]
        [Space(10)]
        [SerializeField] private ComUIListScriptEx captainItemList;

        [Space(15)]
        [HeaderAttribute("Slider")]
        [Space(10)]
        [SerializeField]
        private GameObject sliderRoot;
        [SerializeField] private Slider timeSlider;

        [Space(15)]
        [HeaderAttribute("Button")]
        [Space(10)]
        [SerializeField] private GameObject buttonRoot;
        [SerializeField] private ComButtonWithCd refuseButton;
        [SerializeField] private ComButtonWithCd agreeButton;
        [SerializeField] private GameObject waitingRoot;

        [Space(15)]
        [HeaderAttribute("CountDownTimeFrame")]
        [Space(10)]
        [SerializeField] private CountDownTimeController closeCountDownTimeController;

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
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseButtonClick);
            }

            if (refuseButton != null)
            {
                refuseButton.ResetButtonListener();
                refuseButton.SetButtonListener(OnRefuseButtonClick);
            }

            if (agreeButton != null)
            {
                agreeButton.ResetButtonListener();
                agreeButton.SetButtonListener(OnAgreeButtonClick);
            }

            if (captainItemList != null)
            {
                captainItemList.Initialize();
                captainItemList.onItemVisiable += OnItemVisible;
                captainItemList.OnItemRecycle += OnItemRecycle;
            }

            if (closeCountDownTimeController != null)
            {
                closeCountDownTimeController.SetCountDownTimeCallback(OnCloseFrame);
            }
        }

        private void UnBindUiEvents()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
            }

            if (refuseButton != null)
            {
                refuseButton.ResetButtonListener();
            }

            if (agreeButton != null)
            {
                agreeButton.ResetButtonListener();
            }

            if (captainItemList != null)
            {
                captainItemList.onItemVisiable -= OnItemVisible;
                captainItemList.OnItemRecycle -= OnItemRecycle;
            }

            if (closeCountDownTimeController != null)
            {
                closeCountDownTimeController.SetCountDownTimeCallback(null);
            }
        }

        private void ClearData()
        {
            _captainDataModelList = null;
            _countDownTotalTime = 0;
            _countDownLeftTime = 0;
            _isAlreadyAgree = false;
        }

        private void OnEnable()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationFightStartVoteAgreeMessage,
                OnReceiveTeamDuplicationFightStartVoteAgreeMessage);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationFightStartVoteAgreeMessage,
                OnReceiveTeamDuplicationFightStartVoteAgreeMessage);
        }

        public void Init()
        {
            InitData();
            InitView();
        }

        private void InitData()
        {
            _countDownTotalTime = TeamDuplicationDataManager.GetInstance().StartBattleVoteIntervalTime;
            _countDownLeftTime = TeamDuplicationDataManager.GetInstance().StartBattleVoteEndTime -
                                 TimeManager.GetInstance().GetServerTime();

            if (_countDownTotalTime <= 0)
                _countDownTotalTime = 15;
            if (_countDownLeftTime < 0 || _countDownLeftTime > _countDownTotalTime)
                _countDownLeftTime = _countDownTotalTime;

            _isAlreadyAgree = TeamDuplicationUtility.IsPlayerAlreadyAgreeFightStartVote(PlayerBaseData.GetInstance().RoleID);

        }

        private void InitView()
        {
            InitTitle();

            UpdateCaptainItemList();

            UpdateSliderValue(_countDownLeftTime, _countDownTotalTime);
            UpdateVoteViewByPlayerAgree(_isAlreadyAgree);

            SetCloseFrameCountDownController();
        }

        //用于关闭倒计时
        private void SetCloseFrameCountDownController()
        {
            if (closeCountDownTimeController == null)
                return;

            closeCountDownTimeController.EndTime =
                TimeManager.GetInstance().GetServerTime() + (uint)(_countDownTotalTime + _closeFrameIntervalTime);
            closeCountDownTimeController.InitCountDownTimeController();
        }

        private void ResetCloseFrameCountDownController()
        {
            if (closeCountDownTimeController == null)
                return;

            closeCountDownTimeController.ResetCountDownTimeController();
        }

        private void InitTitle()
        {
            if (titleLabel == null)
                return;

            //噩梦难度
            if (TeamDuplicationUtility.IsTeamDuplicationTeamDifficultyHardLevel() == true)
            {
                titleLabel.text = TR.Value("team_duplication_hard_level_format");
            }
            else
            {
                //普通难度
                titleLabel.text = TR.Value("team_duplication_normal_Level_format");
            }
        }

        private void UpdateSliderValue(float leftTime, float totalTime)
        {
            if (leftTime < 0)
                leftTime = 0;
            if (leftTime > totalTime)
                leftTime = totalTime;

            if (timeSlider != null)
                timeSlider.value = leftTime / totalTime;
        }

        #region CaptainItemList
        private void UpdateCaptainItemList()
        {
            _captainDataModelList = TeamDuplicationDataManager.GetInstance().GetTeamDuplicationCaptainDataModelList();
            if (captainItemList != null)
            {
                if (_captainDataModelList == null || _captainDataModelList.Count <= 0)
                    captainItemList.SetElementAmount(0);
                else
                {
                    captainItemList.SetElementAmount(_captainDataModelList.Count);
                }
            }
        }

        private void OnItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (captainItemList == null)
                return;

            if (_captainDataModelList == null
                || _captainDataModelList.Count <= 0)
                return;

            if (item.m_index >= _captainDataModelList.Count)
                return;

            var fightVoteItem =
                item.GetComponent<TeamDuplicationFightVoteItem>();
            TeamDuplicationCaptainDataModel troopDataModel = _captainDataModelList[item.m_index];

            if (fightVoteItem != null && troopDataModel != null)
                fightVoteItem.Init(troopDataModel);

        }

        private void OnItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (captainItemList == null)
                return;

            var fightVoteItem = item.GetComponent<TeamDuplicationFightVoteItem>();
            if (fightVoteItem != null)
                fightVoteItem.Reset();
        }

        #endregion

        private void Update()
        {

            //更新时间进度条
            _countDownLeftTime -= Time.deltaTime;
            if (_countDownLeftTime < 0)
                _countDownLeftTime = 0;
            UpdateSliderValue(_countDownLeftTime, _countDownTotalTime);
        }

        private void OnReceiveTeamDuplicationFightStartVoteAgreeMessage(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var voteAgreePlayerId = (ulong)uiEvent.Param1;

            //同意的角色不是自己,不进行操作
            if (voteAgreePlayerId != PlayerBaseData.GetInstance().RoleID)
                return;

            //自己投票同意
            UpdateVoteViewByPlayerAgree(true);
        }

        private void UpdateVoteViewByPlayerAgree(bool flag)
        {
            CommonUtility.UpdateGameObjectVisible(waitingRoot, flag);
            CommonUtility.UpdateGameObjectVisible(buttonRoot, !flag);
        }

        private void OnRefuseButtonClick()
        {
            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyStartBattleVote(false);
        }

        private void OnAgreeButtonClick()
        {
            if (TeamDuplicationUtility.IsTeamDuplicationInBuildScene() == true)
            {
                ////处在组团场景中
                //TeamDuplicationUtility.OnForwardFightSceneAndAgreeFight(TR.Value("team_duplication_fight_sure_content"),
                //    ForwardFightSceneAndAgreeFight);

                //处在团本组队场景中,不需要二次弹窗，直接到攻坚场景
                ForwardFightSceneAndAgreeFight();

            }
            else
            {
                TeamDuplicationDataManager.GetInstance().OnSendTeamCopyStartBattleVote(true);
            }
        }

        private void ForwardFightSceneAndAgreeFight()
        {
            //关闭界面
            OnCloseFrame();

            //正处在投票阶段
            if (TeamDuplicationDataManager.GetInstance().IsInStartBattleVotingTime == true)
            {
                TeamDuplicationUtility.SwitchTeamDuplicationToFightSceneByAgreeBattle();
            }
        }

        private void OnCloseButtonClick()
        {
            OnCloseFrame();
        }


        //仅仅关闭界面
        private void OnCloseFrame()
        {
            ResetCloseFrameCountDownController();

            TeamDuplicationUtility.OnCloseTeamDuplicationFightStartVoteFrame();
        }


    }
}
