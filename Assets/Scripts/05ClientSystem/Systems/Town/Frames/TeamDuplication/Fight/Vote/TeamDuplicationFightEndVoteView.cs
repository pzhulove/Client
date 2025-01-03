using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class TeamDuplicationFightEndVoteView : MonoBehaviour
    {

        //倒计时
        private float _countDownTotalTime = 0;
        private float _countDownLeftTime = 0;

        private float _refuseIntervalTime = 0;           //临时的倒计时变量
        private int _refuseLeftTime = 0;                    //拒绝按钮上的时间

        private int _closeFrameIntervalTime = 3;            //倒计时结束3s后，强制关闭界面

        //是否投票(同意或者失败)
        private bool _isAlreadyVote = false;

        private List<TeamDuplicationCaptainDataModel> _captainDataModelList;

        [Space(15)]
        [HeaderAttribute("Common")]
        [Space(10)]
        [SerializeField] private Text titleLabel;
        [SerializeField] private Text fightEndVoteDescriptionLabel;

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
        [SerializeField] private Text refuseLeftTimeText;

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
            _refuseIntervalTime = 0;
            _isAlreadyVote = false;
        }

        private void OnEnable()
        {
            //成员投票赞成
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationFightEndVoteAgreeMessage,
                OnReceiveTeamDuplicationFightEndVoteAgreeMessage);

            //成员投票拒绝
            UIEventSystem.GetInstance().RegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationFightEndVoteRefuseMessage,
                OnReceiveTeamDuplicationFightEndVoteRefuseMessage);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationFightEndVoteAgreeMessage,
                OnReceiveTeamDuplicationFightEndVoteAgreeMessage);

            UIEventSystem.GetInstance().UnRegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationFightEndVoteRefuseMessage,
                OnReceiveTeamDuplicationFightEndVoteRefuseMessage);
        }

        public void Init()
        {
            InitData();
            InitView();
        }

        private void InitData()
        {
            //todo
            //战斗结束投票的时间
            _countDownTotalTime = TeamDuplicationDataManager.GetInstance().FightEndVoteIntervalTime;
            _countDownLeftTime = TeamDuplicationDataManager.GetInstance().FightEndVoteEndTime -
                                 TimeManager.GetInstance().GetServerTime();

            if (_countDownTotalTime <= 0)
                _countDownTotalTime = 15;
            if (_countDownLeftTime < 0 || _countDownLeftTime > _countDownTotalTime)
                _countDownLeftTime = _countDownTotalTime;

            _refuseLeftTime = (int) _countDownLeftTime;
            _refuseIntervalTime = 0;

            //是否自己已经投票
            bool isAlreadyAgree =
                TeamDuplicationUtility.IsPlayerAlreadyAgreeFightEndVote(PlayerBaseData.GetInstance().RoleID);
            bool isAlreadyRefuse =
                TeamDuplicationUtility.IsPlayerAlreadyRefuseFightEndVote(PlayerBaseData.GetInstance().RoleID);
            if (isAlreadyAgree == true || isAlreadyRefuse == true)
            {
                _isAlreadyVote = true;
            }
            else
            {
                _isAlreadyVote = false;
            }
        }

        private void InitView()
        {
            InitTitle();

            UpdateCaptainItemList();

            UpdateSliderValue(_countDownLeftTime, _countDownTotalTime);
            UpdateRefuseLeftTimeLabel();
            UpdateVoteViewByPlayerVote(_isAlreadyVote);

            SetCloseFrameCountDownController();
        }

        //用于关闭倒计时
        private void SetCloseFrameCountDownController()
        {
            if (closeCountDownTimeController == null)
                return;

            closeCountDownTimeController.EndTime =
                TimeManager.GetInstance().GetServerTime() + (uint)(_countDownLeftTime + _closeFrameIntervalTime);
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
            if (titleLabel != null)
                titleLabel.text = TR.Value("team_duplication_fight_end_confirm_title");

            if (fightEndVoteDescriptionLabel != null)
                fightEndVoteDescriptionLabel.text = TR.Value("team_duplication_fight_end_vote_description");
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
            TeamDuplicationCaptainDataModel captainDataModel = _captainDataModelList[item.m_index];

            if (fightVoteItem != null && captainDataModel != null)
                fightVoteItem.Init(captainDataModel,
                    TeamDuplicationFightVoteType.FightEndVote);

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

            _refuseIntervalTime += Time.deltaTime;
            if (_refuseIntervalTime >= 1.0f)
            {
                _refuseIntervalTime = 0.0f;

                _refuseLeftTime -= 1;
                if (_refuseLeftTime <= 0)
                    _refuseLeftTime = 0;
                UpdateRefuseLeftTimeLabel();
            }
        }

        private void UpdateRefuseLeftTimeLabel()
        {
            //注释掉，不刷新
            //if (refuseLeftTimeText == null)
            //    return;

            //refuseLeftTimeText.text = string.Format(TR.Value("team_duplication_fight_end_refuse_format"),
            //    _refuseLeftTime);
        }

        //投票同意
        private void OnReceiveTeamDuplicationFightEndVoteAgreeMessage(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var voteAgreePlayerId = (ulong)uiEvent.Param1;

            //同意的角色不是自己,不进行操作
            if (voteAgreePlayerId != PlayerBaseData.GetInstance().RoleID)
                return;

            //自己投票同意
            UpdateVoteViewByPlayerVote(true);
        }

        //投票拒绝
        private void OnReceiveTeamDuplicationFightEndVoteRefuseMessage(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var voteRefusePlayerId = (ulong) uiEvent.Param1;
            if (voteRefusePlayerId != PlayerBaseData.GetInstance().RoleID)
                return;

            UpdateVoteViewByPlayerVote(true);
        }

        private void UpdateVoteViewByPlayerVote(bool flag)
        {
            CommonUtility.UpdateGameObjectVisible(waitingRoot, flag);
            CommonUtility.UpdateGameObjectVisible(buttonRoot, !flag);
        }

        private void OnRefuseButtonClick()
        {
            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyForceEndVoteReq(false);
        }

        private void OnAgreeButtonClick()
        {
            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyForceEndVoteReq(true);
        }
        
        private void OnCloseButtonClick()
        {
            OnCloseFrame();
        }


        //仅仅关闭界面
        private void OnCloseFrame()
        {
            ResetCloseFrameCountDownController();

            TeamDuplicationUtility.OnCloseTeamDuplicationFightEndVoteFrame();
        }
    }
}
