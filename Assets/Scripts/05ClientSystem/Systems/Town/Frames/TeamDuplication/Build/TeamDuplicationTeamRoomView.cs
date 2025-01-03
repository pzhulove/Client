using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class TeamDuplicationTeamRoomView : MonoBehaviour
    {
        //是否为请求者，请求其他队伍的数据
        private int _otherTeamDetailId = 0;
        //其他队伍详情的标志
        private bool _isOtherTeamDetailFlag = false;

        private TeamDuplicationPlayerDataModel _ownerPlayerDataModel;
        private TeamDuplicationTeamDataModel _teamDataModel;

        [Space(15)]
        [HeaderAttribute("Common")]
        [Space(10)]
        [SerializeField] private Text titleLabel;
        [SerializeField] private Button closeButton;

        [Space(15)] [HeaderAttribute("Gold")] [Space(10)]
        [SerializeField] private GameObject totalGoldRoot;
        [SerializeField] private Text totalGoldValueText;

        [SerializeField] private GameObject ownerGoldRoot;
        [SerializeField] private Text ownerGoldValueText;
        [SerializeField] private GameObject helpRoot;

        [Space(15)]
        [HeaderAttribute("BottomButton")]
        [Space(10)]
        [SerializeField] private TeamDuplicationTeamRoomBottomControl teamRoomBottomControl;

        [Space(15)]
        [HeaderAttribute("TroopRoomList")]
        [Space(10)]
        [SerializeField] private ComUIListScriptEx teamRoomList;

        private void Awake()
        {
            BindUiEvents();
        }

        private void OnDestroy()
        {
            UnBindUiEvents();
            ClearData();
        }

        private void ClearData()
        {
            _ownerPlayerDataModel = null;
            _teamDataModel = null;

            _isOtherTeamDetailFlag = false;
            _otherTeamDetailId = 0;

            CommonNewDragUtility.ResetFirstDragPointerId();
        }

        private void BindUiEvents()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseButtonClick);
            }

            if (teamRoomList != null)
            {
                teamRoomList.Initialize();
                teamRoomList.onItemVisiable += OnItemVisible;
            }
        }

        private void UnBindUiEvents()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveAllListeners();

            if (teamRoomList != null)
            {
                teamRoomList.onItemVisiable -= OnItemVisible;
            }
        }

        private void OnEnable()
        {
            //团本的数据进行更新
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamDataMessage,
                OnReceiveTeamDuplicationTeamDataMessage);

            //其他团本的数据
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamDetailDataMessage,
                OnReceiveTeamDuplicationTeamDetailDataMessage);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamDataMessage,
                OnReceiveTeamDuplicationTeamDataMessage);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamDetailDataMessage,
                OnReceiveTeamDuplicationTeamDetailDataMessage);
        }

        public void Init(int teamId = 0)
        {
            _otherTeamDetailId = teamId;
            if (_otherTeamDetailId == 0)
            {
                //自己所在团本
                _isOtherTeamDetailFlag = false;
            }
            else
            {
                //其他团队的信息
                _isOtherTeamDetailFlag = true;
            }

            if (_isOtherTeamDetailFlag == false)
            {
                //自己队伍
                _teamDataModel = TeamDuplicationDataManager.GetInstance().GetTeamDuplicationTeamDataModel();

                if (_teamDataModel == null)
                    return;

                _ownerPlayerDataModel = TeamDuplicationUtility.GetOwnerPlayerDataModel();
                if (_ownerPlayerDataModel == null || _ownerPlayerDataModel.Guid == 0)
                    return;

                InitView();
            }
            else
            {
                UpdateOwnerGoalRoot(false);
                UpdateBottomControl();
                TeamDuplicationDataManager.GetInstance().OnSendTeamDetailReq(_otherTeamDetailId);
            }

            CommonNewDragUtility.ResetFirstDragPointerId();
        }

        private void InitView()
        {
            UpdateTeamRoomTitle();

            UpdateGoldValue();

            UpdateTroopRoomList();

            UpdateBottomControl();
        }

        private void UpdateTeamRoomTitle()
        {
            if (titleLabel == null)
                return;

            if (_teamDataModel == null)
                return;

            if (string.IsNullOrEmpty(_teamDataModel.TeamName) == false)
            {
                titleLabel.text = string.Format(TR.Value("team_duplication_team_name_format"), _teamDataModel.TeamName);
            }
        }

        private void UpdateBottomControl()
        {
            if (teamRoomBottomControl == null)
                return;

            //队伍详情页面
            if (_isOtherTeamDetailFlag == true)
            {
                teamRoomBottomControl.Init(false,
                    _isOtherTeamDetailFlag,
                    _otherTeamDetailId);
            }
            else
            {
                //团长视角
                var isTeamLeader = _ownerPlayerDataModel.IsTeamLeader == true;
                teamRoomBottomControl.Init(isTeamLeader, _isOtherTeamDetailFlag);
            }
        }


        private void UpdateOwnerGoalRoot(bool flag)
        {
            CommonUtility.UpdateGameObjectVisible(ownerGoldRoot, flag);
        }

        //佣金数量的改变
        private void UpdateGoldValue()
        {
            if (_teamDataModel == null)
                return;

            //挑战模式不显示佣金
            if (_teamDataModel.TeamModel == (int) TeamCopyTeamModel.TEAM_COPY_TEAM_MODEL_CHALLENGE)
                return;

            //总佣金和帮助按钮，都显示
            CommonUtility.UpdateGameObjectVisible(totalGoldRoot, true);
            CommonUtility.UpdateGameObjectVisible(helpRoot, true);
            if (totalGoldValueText != null)
            {
                var totalGoldValue = _teamDataModel.TotalCommission;
                totalGoldValueText.text = totalGoldValue.ToString();
            }


            if (_isOtherTeamDetailFlag == true)
            {
                //其他队伍不显示自己的佣金
                UpdateOwnerGoalRoot(false);
            }
            else
            {
                if (_ownerPlayerDataModel.IsGoldOwner == true)
                {
                    //自己是金主
                    UpdateOwnerGoalRoot(false);
                }
                else
                {
                    if (ownerGoldValueText != null)
                    {
                        var bonusGoldValue = _teamDataModel.BonusCommission;
                        ownerGoldValueText.text = bonusGoldValue.ToString();
                    }

                    UpdateOwnerGoalRoot(true);
                }
            }
        }

        private void UpdateTroopRoomList()
        {
            var troopRoomNumber = 0;
            if (_teamDataModel != null && _teamDataModel.CaptainDataModelList != null
                                            && _teamDataModel.CaptainDataModelList.Count > 0)
                troopRoomNumber = _teamDataModel.CaptainDataModelList.Count;

            if (teamRoomList != null)
                teamRoomList.SetElementAmount(troopRoomNumber);
        }

        private void OnItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (_teamDataModel == null
                || _teamDataModel.CaptainDataModelList == null
                || _teamDataModel.CaptainDataModelList.Count <= 0)
                return;

            if (item.m_index >= _teamDataModel.CaptainDataModelList.Count)
                return;

            TeamDuplicationCaptainDataModel troopDataModel = _teamDataModel.CaptainDataModelList[item.m_index];
            TeamDuplicationTeamRoomItem teamRoomItem = item.GetComponent<TeamDuplicationTeamRoomItem>();

            if (teamRoomItem != null && troopDataModel != null)
            {
                teamRoomItem.Init(troopDataModel,
                    _isOtherTeamDetailFlag);
            }
        }

        #region UIEvent
        //团本更新的消息
        private void OnReceiveTeamDuplicationTeamDataMessage(UIEvent uiEvent)
        {
            if (_isOtherTeamDetailFlag == true)
                return;

            _teamDataModel = TeamDuplicationDataManager.GetInstance().GetTeamDuplicationTeamDataModel();
            if (_teamDataModel == null)
                return;

            _ownerPlayerDataModel = TeamDuplicationUtility.GetOwnerPlayerDataModel();
            if (_ownerPlayerDataModel == null || _ownerPlayerDataModel.Guid == 0)
                return;

            UpdateTeamRoomTitle();
            UpdateGoldValue();
            UpdateTroopRoomList();

            UpdateBottomControl();

        }

        //其他团本的数据
        private void OnReceiveTeamDuplicationTeamDetailDataMessage(UIEvent uiEvent)
        {
            if (_isOtherTeamDetailFlag == false)
                return;

            var otherTeamDetailId = (int)uiEvent.Param1;
            if (otherTeamDetailId != _otherTeamDetailId)
                return;

            _teamDataModel = TeamDuplicationDataManager.GetInstance().OtherTeamDataModel;

            UpdateTeamRoomTitle();
            UpdateGoldValue();
            UpdateTroopRoomList();
        }


        #endregion 

        private void OnCloseButtonClick()
        {
            if(_isOtherTeamDetailFlag)
                TeamDuplicationDataManager.GetInstance().ResetOtherTeamDataModel();

            TeamDuplicationUtility.OnCloseTeamDuplicationTeamRoomFrame();
        }

    }
}
