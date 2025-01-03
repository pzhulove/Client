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

    //队伍列表中，队伍的数据
    public class TeamDuplicationTeamListItem : MonoBehaviour
    {
        private TeamDuplicationTeamListDataModel _teamListDataModel;

        [Space(10)]
        [HeaderAttribute("Type")]
        [Space(5)]
        [SerializeField] private GameObject challengeRoot;

        [SerializeField] private GameObject goldTeamRoot;

        [Space(10)]
        [HeaderAttribute("Text")]
        [Space(5)]
        [SerializeField] private Text goldValue;
        [SerializeField] private Text teamName;
        [SerializeField] private Text teamNumberValue;
        [SerializeField] private Text equipmentScore;
        [SerializeField] private Text teamStatus;

        [Space(10)] [HeaderAttribute("TeamHardLevel")] [Space(5)]
        [SerializeField] private GameObject teamNormalLevelFlag;
        [SerializeField] private GameObject teamHardLevelFlag;

        [Space(10)]
        [HeaderAttribute("SearchButton")]
        [Space(5)]
        [SerializeField]
        private Button detailButton;

        [Space(10)]
        [HeaderAttribute("RequestButton")]
        [Space(5)]
        [SerializeField]
        private GameObject requestButtonRoot;
        [SerializeField] private ComButtonWithCd requestButton;
        [SerializeField] private GameObject lockRequest;

        private void Awake()
        {
            BindUiEvents();
        }

        private void OnDestroy()
        {
            UnBindUiEvents();
        }

        private void BindUiEvents()
        {
            if (detailButton != null)
            {
                detailButton.onClick.RemoveAllListeners();
                detailButton.onClick.AddListener(OnDetailButtonClick);
            }

            if (requestButton != null)
            {
                requestButton.ResetButtonListener();
                requestButton.SetButtonListener(OnRequestButtonClick);
            }
        }

        private void UnBindUiEvents()
        {
            if (detailButton != null)
                detailButton.onClick.RemoveAllListeners();

            if (requestButton != null)
                requestButton.ResetButtonListener();
        }

        private void OnEnable()
        {
            BindUiMessage();
        }

        private void OnDisable()
        {
            UnBindUiMessage();
        }

        private void BindUiMessage()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationJoinTeamInCdTimeMessage,
                OnReceiveTeamDuplicationJoinTeamInCdTimeMessage);
        }

        private void UnBindUiMessage()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationJoinTeamInCdTimeMessage,
                OnReceiveTeamDuplicationJoinTeamInCdTimeMessage);
        }

        public void Init(TeamDuplicationTeamListDataModel teamDataModel)
        {
            _teamListDataModel = teamDataModel;
            if (_teamListDataModel == null)
            {
                Logger.LogErrorFormat("TeamDuplicationTeamItem teamDataModel is null");
                return;
            }

            InitItem();
        }

        private void InitItem()
        {
            //团本类型
            if (_teamListDataModel.TeamType == TeamCopyTeamModel.TEAM_COPY_TEAM_MODEL_CHALLENGE)
            {
                CommonUtility.UpdateGameObjectVisible(challengeRoot, true);
                CommonUtility.UpdateGameObjectVisible(goldTeamRoot, false);
            }
            else
            {
                CommonUtility.UpdateGameObjectVisible(challengeRoot, false);
                CommonUtility.UpdateGameObjectVisible(goldTeamRoot, true);
            }

            //佣金数量
            if (goldValue != null)
            {
                if (_teamListDataModel.GoldValue <= 0)
                {
                    goldValue.text = TR.Value("team_duplication_team_no_gold");
                }
                else
                {
                    goldValue.text = _teamListDataModel.GoldValue.ToString();
                }
            }

            //团队名字
            if (teamName != null)
                teamName.text = _teamListDataModel.TeamName;

            //团队难度
            if ((TeamCopyTeamGrade) _teamListDataModel.TeamHardLevel
                == TeamCopyTeamGrade.TEAM_COPY_TEAM_GRADE_DIFF)
            {
                //噩梦难度
                CommonUtility.UpdateGameObjectVisible(teamHardLevelFlag, true);
                CommonUtility.UpdateGameObjectVisible(teamNormalLevelFlag, false);
            }
            else
            {
                //普通难度
                CommonUtility.UpdateGameObjectVisible(teamHardLevelFlag, false);
                CommonUtility.UpdateGameObjectVisible(teamNormalLevelFlag, true);
            }

            //团队数量
            if (teamNumberValue != null)
            {
                teamNumberValue.text = TR.Value("team_duplication_team_number",
                    _teamListDataModel.TeamNumber,
                    TeamDuplicationDataManager.GetInstance().TeamDuplicationTotalPlayerNumberInTeam);
            }

            //装备评分
            if (equipmentScore != null)
            {
                equipmentScore.text = _teamListDataModel.EquipmentScore.ToString();
            }

            UpdateTeamDuplicationStatus(_teamListDataModel.TroopStatus);
            UpdateRequestButton();
        }

        private void UpdateTeamDuplicationStatus(int troopStatus)
        {
            var statusStr = TR.Value("team_duplication_team_status_prepare");

            TeamCopyTeamStatus teamCopyTeamStatus = (TeamCopyTeamStatus) troopStatus;
            if (teamCopyTeamStatus == TeamCopyTeamStatus.TEAM_COPY_TEAM_STATUS_BATTLE)
                statusStr = TR.Value("team_duplication_team_status_fight");

            if (teamStatus != null)
                teamStatus.text = statusStr;
        }

        private void UpdateRequestButton()
        {
            if (TeamDuplicationDataManager.GetInstance().IsTeamDuplicationGoldOwner == true
                && PlayerBaseData.GetInstance().Gold < (ulong)_teamListDataModel.GoldValue)
            {
                //金主, 金币不足佣金数量
                CommonUtility.UpdateGameObjectVisible(lockRequest, true);
                CommonUtility.UpdateGameObjectVisible(requestButtonRoot, false);
            }
            else
            {
                //非金主, 或者金币数量足够
                CommonUtility.UpdateGameObjectVisible(lockRequest, false);

                CommonUtility.UpdateGameObjectVisible(requestButtonRoot, true);

                if (requestButton != null)
                {
                    requestButton.SetCountDownTimeDescription(TR.Value("team_duplication_request_join_in_normal_format"),
                        TR.Value("team_duplication_request_join_in_count_down_time_format"));
                }

                //设置CD时间
                var requestJoinInEndTime = TeamDuplicationDataManager.GetInstance()
                    .GetTeamRequestJoinInEndTime((int)_teamListDataModel.TeamId);
                UpdateRequestButton(requestJoinInEndTime);

            }
        }

        private void OnDetailButtonClick()
        {
            if (_teamListDataModel == null)
                return;

            //展示
            TeamDuplicationUtility.OnOpenTeamDuplicationTeamRoomFrame((int)_teamListDataModel.TeamId);
        }

        private void OnRequestButtonClick()
        {
            if (_teamListDataModel == null)
            {
                Logger.LogErrorFormat("TeamListDataModel is null");
                return;
            }

            if (TeamDuplicationDataManager.GetInstance().IsTeamDuplicationOwnerTeam == true)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_join_in_troop_failed_with_team"));
                return;
            }

            ////挑战次数达到上限，无法申请
            //if (TeamDuplicationUtility.IsFightNumberAlreadyReachLimit() == true)
            //{
            //    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_join_in_troop_failed_with_number"));
            //    return;
            //}

            //佣金不足，无法加入团队
            if (TeamDuplicationUtility.IsJoinInTeamDuplicationGoldIsNotEnough(_teamListDataModel.GoldValue)
                == true)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_join_in_troop_failed_with_gold"));
                return;
            }

            //发送申请的请求
            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyTeamApplyReq((int)_teamListDataModel.TeamId);

            //设置CD结束的时间
            if(requestButton != null)
                TeamDuplicationDataManager.GetInstance().SetTeamRequestJoinInEndTime((int)_teamListDataModel.TeamId,
                    (int)TimeManager.GetInstance().GetServerTime() + (int)requestButton.GetButtonCdTime());

            //暂时先直接关闭界面
            //TeamDuplicationUtility.OnCloseTeamDuplicationTeamListFrame();

            return;
        }

        //收到无法申请团队的CD消息
        private void OnReceiveTeamDuplicationJoinTeamInCdTimeMessage(UIEvent uiEvent)
        {
            if (_teamListDataModel == null)
                return;

            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var teamId = (int)uiEvent.Param1;
            if (_teamListDataModel.TeamId != (uint)teamId)
                return;

            //设置CD时间
            var requestJoinInEndTime = TeamDuplicationDataManager.GetInstance().GetTeamRequestJoinInEndTime(teamId);
            UpdateRequestButton(requestJoinInEndTime);


        }

        private void UpdateRequestButton(int requestJoinInEndTime)
        {
            if (requestButton == null)
                return;

            if (requestJoinInEndTime <= 0
                || requestJoinInEndTime <= TimeManager.GetInstance().GetServerTime())
            {
                requestButton.Reset();
            }
            else
            {
                requestButton.Reset();
                var cdTime = requestJoinInEndTime - TimeManager.GetInstance().GetServerTime();
                requestButton.SetButtonTimeLimit(cdTime);
            }
        }


        public void Reset()
        {
            _teamListDataModel = null;
        }


    }
}
