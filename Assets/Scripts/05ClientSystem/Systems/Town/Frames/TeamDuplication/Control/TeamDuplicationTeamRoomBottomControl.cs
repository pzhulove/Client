using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;
using Scripts.UI;

namespace GameClient
{

    //团本房间的Bottom的控制器
    public class TeamDuplicationTeamRoomBottomControl : MonoBehaviour
    {
        private bool _isTeamLeader = false;
        private bool _isOtherTeam = false;
        private int _otherTeamId = 0;

        [Space(15)]
        [HeaderAttribute("CommonButton")]
        [Space(5)]
        [SerializeField] private Button quitButton;
        [SerializeField] private Button recruitButton;
        [SerializeField] private Button chatButton;
        [SerializeField] private Button requestJoinInTeamButton;
        [SerializeField] private Button requestListButton;
        [SerializeField] private GameObject requestListButtonRedPoint;
        [SerializeField] private Button teamSettingButton;

        [Space(15)]
        [HeaderAttribute("AutoAgreeGold")]
        [Space(5)]
        [SerializeField] private GameObject autoAgreeGoldJoinInRoot;
        [SerializeField] private ComButtonWithCd autoAgreeGoldJoinInButton;
        [SerializeField] private GameObject autoJoinInFlag;
        [SerializeField] private Text autoJoinInLabel;

        [Space(15)] [HeaderAttribute("AdjustTeamPosition")] [Space(5)] [SerializeField]
        private GameObject adjustTeamPositionRoot;
        [SerializeField] private Text adjustTeamPositionLabel;
        [SerializeField] private CommonGameObjectVisibleControl adjustTeamPositionControl;

        [Space(15)]
        [HeaderAttribute("GameObjectRoot")]
        [Space(5)]
        [SerializeField] private GameObject teamLeaderRoot;
        [SerializeField] private GameObject normalPlayerRoot;
        [SerializeField] private GameObject otherTeamRoot;

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
            _isTeamLeader = false;
        }

        private void BindUiEvents()
        {
            if (quitButton != null)
            {
                quitButton.onClick.RemoveAllListeners();
                quitButton.onClick.AddListener(OnQuitButtonClick);
            }

            if (requestListButton != null)
            {
                requestListButton.onClick.RemoveAllListeners();
                requestListButton.onClick.AddListener(OnRequesterListButtonClick);
            }

            if (recruitButton != null)
            {
                recruitButton.onClick.RemoveAllListeners();
                recruitButton.onClick.AddListener(OnRecruitButtonClick);
            }

            if (chatButton != null)
            {
                chatButton.onClick.RemoveAllListeners();
                chatButton.onClick.AddListener(OnChatButtonClick);
            }

            if (requestJoinInTeamButton != null)
            {
                requestJoinInTeamButton.onClick.RemoveAllListeners();
                requestJoinInTeamButton.onClick.AddListener(OnRequestJoinInTeamButtonClick);
            }

            if (autoAgreeGoldJoinInButton != null)
            {
                autoAgreeGoldJoinInButton.ResetButtonListener();
                autoAgreeGoldJoinInButton.SetButtonListener(OnAutoJoinInButtonClick);
            }

            if (teamSettingButton != null)
            {
                teamSettingButton.onClick.RemoveAllListeners();
                teamSettingButton.onClick.AddListener(OnTeamSettingButtonClicked);
            }

        }

        private void UnBindUiEvents()
        {
            if (quitButton != null)
            {
                quitButton.onClick.RemoveAllListeners();
            }

            if (requestListButton != null)
            {
                requestListButton.onClick.RemoveAllListeners();
            }

            if (recruitButton != null)
            {
                recruitButton.onClick.RemoveAllListeners();
            }

            if (chatButton != null)
            {
                chatButton.onClick.RemoveAllListeners();
            }

            if (requestJoinInTeamButton != null)
                requestJoinInTeamButton.onClick.RemoveAllListeners();

            if (autoAgreeGoldJoinInButton != null)
            {
                autoAgreeGoldJoinInButton.ResetButtonListener();
            }

            if(teamSettingButton != null)
                teamSettingButton.onClick.RemoveAllListeners();
        }

        private void OnEnable()
        {
            BindUiMessages();
        }

        private void OnDisable()
        {
            UnBindUiMessages();
        }

        private void BindUiMessages()
        {
            //自动同意入团
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationAutoAgreeGoldMessage,
                OnReceiveTeamDuplicationAutoAgreeGoldMessage);

            //存在新的邀请者
            UIEventSystem.GetInstance().RegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationOwnerNewRequesterMessage,
                OnReceiveTeamDuplicationOwnerNewRequesterMessage);

        }

        private void UnBindUiMessages()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationAutoAgreeGoldMessage,
                OnReceiveTeamDuplicationAutoAgreeGoldMessage);

            UIEventSystem.GetInstance().UnRegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationOwnerNewRequesterMessage,
                OnReceiveTeamDuplicationOwnerNewRequesterMessage);
        }

        public void Init(bool isTeamLeader = false,
            bool isOtherTeam = false,
            int otherTeamId = 0)
        {
            _isTeamLeader = isTeamLeader;
            _isOtherTeam = isOtherTeam;
            _otherTeamId = otherTeamId;

            UpdateTeamLeaderRoot();
        }

        private void UpdateTeamLeaderRoot()
        {
            if (_isOtherTeam == true)
            {
                CommonUtility.UpdateGameObjectVisible(teamLeaderRoot, false);
                CommonUtility.UpdateGameObjectVisible(normalPlayerRoot, false);
                CommonUtility.UpdateGameObjectVisible(otherTeamRoot, true);
                return;
            }

            CommonUtility.UpdateGameObjectVisible(otherTeamRoot, false);
            CommonUtility.UpdateGameObjectVisible(normalPlayerRoot, true);

            CommonUtility.UpdateGameObjectVisible(teamLeaderRoot, _isTeamLeader);

            if (_isTeamLeader == true)
            {
                UpdateAutoAgreeGoldJoinInRoot();
                UpdateAutoAgreeGoldJoinInFlag();

                if (autoJoinInLabel != null)
                {
                    autoJoinInLabel.text = TR.Value("team_duplication_troop_room_auto_join_in_team");
                }

                UpdateTeamAdjustPositionTip();

                //更新红点
                UpdateRequestListButtonRedPoint();
            }
        }

        private void UpdateTeamAdjustPositionTip()
        {
            if (adjustTeamPositionLabel != null)
            {
                adjustTeamPositionLabel.text = TR.Value("team_duplication_troop_room_adjust_team_position");
            }

            //没有展示过
            if (TeamDuplicationDataManager.GetInstance().IsAlreadyShowPositionAdjustTip == false)
            {
                CommonUtility.UpdateGameObjectVisible(adjustTeamPositionRoot, true);
                if (adjustTeamPositionControl != null)
                    adjustTeamPositionControl.SetVisibleControl();

                TeamDuplicationDataManager.GetInstance().IsAlreadyShowPositionAdjustTip = true;
            }
            else
            {
                //已经展示过了
                CommonUtility.UpdateGameObjectVisible(adjustTeamPositionRoot, false);
            }
        }


        private void OnQuitButtonClick()
        {
            TeamCopyTeamStatus teamStatus = TeamDuplicationUtility.GetTeamDuplicationTeamStatus();
            var quitContentStr = "";
            if (teamStatus == TeamCopyTeamStatus.TEAM_COPY_TEAM_STATUS_VICTORY)
            {
                //已经领取了最终奖励，直接退出
                if (TeamDuplicationDataManager.GetInstance().IsAlreadyReceiveFinalReward == true)
                {
                    QuitTeamAction();
                    return;
                }

                //胜利
                quitContentStr = TR.Value("team_duplication_leave_team_with_pass");
            }
            else if (teamStatus == TeamCopyTeamStatus.TEAM_COPY_TEAM_STATUS_PARPARE)
            {
                //未开战
                quitContentStr = TR.Value("team_duplication_leave_team_normal_tips");
            }
            else
            {
                //开战情况
                //免费退出
                quitContentStr = TR.Value("team_duplication_leave_team_without_cost_tips");
                //不能免费退出
                if (TeamDuplicationUtility.IsPlayerCanFreeQuitTeam() == false)
                {
                    quitContentStr = TR.Value("team_duplication_leave_team_with_cost_tips");
                }
            }

            TeamDuplicationUtility.OnShowLeaveTeamTipFrame(quitContentStr, QuitTeamAction);
        }

        private void QuitTeamAction()
        {
            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyTeamQuitReq();
        }

        private void OnRequesterListButtonClick()
        {
            TeamDuplicationUtility.OnOpenTeamDuplicationRequesterListFrame();

            UpdateRequestListButtonRedPoint();
        }

        private void OnRecruitButtonClick()
        {
            TeamDuplicationUtility.OnOpenTeamDuplicationFindTeamMateFrame();
        }

        private void OnChatButtonClick()
        {
            TeamDuplicationUtility.OnOpenTeamDuplicationChatFrame();
        }

        private void OnRequestJoinInTeamButtonClick()
        {
            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyTeamApplyReq(_otherTeamId);

            TeamDuplicationUtility.OnCloseTeamDuplicationTeamRoomFrame();
        }

        private void OnAutoJoinInButtonClick()
        {
            if (_isTeamLeader == false)
                return;

            var teamDataModel = TeamDuplicationDataManager.GetInstance().GetTeamDuplicationTeamDataModel();
            if (teamDataModel == null)
                return;

            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyAutoAgreeGoldReq(!teamDataModel.AutoAgreeGold);
        }

        //团队设置界面
        private void OnTeamSettingButtonClicked()
        {
            //非团长
            if (_isTeamLeader == false)
                return;

            //团本数据不存在
            var teamDataModel = TeamDuplicationDataManager.GetInstance().GetTeamDuplicationTeamDataModel();
            if (teamDataModel == null)
                return;

            var teamBuildDataModel = new TeamDuplicationTeamBuildDataModel();
            teamBuildDataModel.TeamDifficultyLevel = teamDataModel.TeamDifficultyLevel;
            teamBuildDataModel.TeamModelType = teamDataModel.TeamModel;
            teamBuildDataModel.IsResetEquipmentScore = true;
            teamBuildDataModel.OwnerEquipmentScore = teamDataModel.TeamEquipScore;

            TeamDuplicationUtility.OnOpenTeamDuplicationTeamSettingFrame(teamBuildDataModel);
        }


        private void UpdateAutoAgreeGoldJoinInRoot()
        {
            var teamDataModel = TeamDuplicationDataManager.GetInstance().GetTeamDuplicationTeamDataModel();
            if (teamDataModel == null)
                return;

            if (autoAgreeGoldJoinInRoot == null)
                return;

            if (teamDataModel.TeamModel == (int)TeamCopyTeamModel.TEAM_COPY_TEAM_MODEL_CHALLENGE)
            {
                CommonUtility.UpdateGameObjectVisible(autoAgreeGoldJoinInRoot, false);
            }
            else
            {
                CommonUtility.UpdateGameObjectVisible(autoAgreeGoldJoinInRoot, true);
            }
        }

        private void UpdateAutoAgreeGoldJoinInFlag()
        {
            var teamDataModel = TeamDuplicationDataManager.GetInstance().GetTeamDuplicationTeamDataModel();
            if (teamDataModel == null)
                return;

            var autoAgreeGold = teamDataModel.AutoAgreeGold;

            CommonUtility.UpdateGameObjectVisible(autoJoinInFlag,
                autoAgreeGold);
        }

        private void UpdateRequestListButtonRedPoint()
        {
            CommonUtility.UpdateGameObjectVisible(requestListButtonRedPoint,
                TeamDuplicationDataManager.GetInstance().IsTeamDuplicationOwnerNewRequester);
        }

        private void OnAdjustTeamPositionRootAnimationComplete()
        {
            CommonUtility.UpdateGameObjectVisible(adjustTeamPositionRoot, false);
        }

        #region UIEvent

        //自动同意
        private void OnReceiveTeamDuplicationAutoAgreeGoldMessage(UIEvent uiEvent)
        {
            if (_isTeamLeader == false)
                return;

            UpdateAutoAgreeGoldJoinInFlag();
        }

        //新的申请者
        private void OnReceiveTeamDuplicationOwnerNewRequesterMessage(UIEvent uiEvent)
        {
            //不是团长，不展示
            if (_isTeamLeader == false)
                return;

            UpdateRequestListButtonRedPoint();
        }

        #endregion

    }
}
