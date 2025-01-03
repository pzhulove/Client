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

    //团本邀请Item
    public class TeamDuplicationTeamInviteItem : MonoBehaviour
    {
        private TeamDuplicationTeamInviteDataModel _teamInviteDataModel;

        [Space(15)] [HeaderAttribute("TeamInviteData")] [Space(10)] [SerializeField]
        private Image teamLeaderHeadFrame;
        [SerializeField] private Image teamLeaderHeadImage;
        [SerializeField] private Text teamLeaderLevel;
        [SerializeField] private Text teamLeaderName;

        [Space(15)]
        [HeaderAttribute("TeamInviteTypeLabel")]
        [Space(10)]
        [SerializeField] private Text teamBaseDescription;
        [SerializeField] private Text teamGoldDescription;

        [Space(15)]
        [HeaderAttribute("Button")]
        [Space(10)]
        [SerializeField] private ComButtonWithCd refuseButton;
        [SerializeField] private ComButtonWithCd agreeButton;

        [Space(10)]
        [HeaderAttribute("DetailButton")]
        [Space(5)]
        [SerializeField]
        private Button detailButton;

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

            if (detailButton != null)
            {
                detailButton.onClick.RemoveAllListeners();
                detailButton.onClick.AddListener(OnDetailButtonClick);
            }
        }

        private void UnBindUiEvents()
        {
            if(refuseButton != null)
                refuseButton.ResetButtonListener();

            if(agreeButton != null)
                agreeButton.ResetButtonListener();

            if (detailButton != null)
                detailButton.onClick.RemoveAllListeners();
        }

        public void Init(TeamDuplicationTeamInviteDataModel teamInviteDataModel)
        {
            _teamInviteDataModel = teamInviteDataModel;

            if (_teamInviteDataModel == null)
            {
                Logger.LogErrorFormat("TeamDuplicationTeamInviteItem teamInviteDataModel is null");
                return;
            }
            
            InitItem();
        }

        private void InitItem()
        {
            //团长头像
            if (teamLeaderHeadImage != null)
            {
                string headIconPath = "";
                var jobData = TableManager.GetInstance().GetTableItem<JobTable>(_teamInviteDataModel.TeamLeaderProfessionId);
                if (jobData != null)
                {
                    var resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
                    if (resData != null)
                        headIconPath = resData.IconPath;
                }

                if (string.IsNullOrEmpty(headIconPath) == false)
                    ETCImageLoader.LoadSprite(ref teamLeaderHeadImage, headIconPath);
            }

            if (teamLeaderHeadFrame != null)
            {
                var headFramePath = HeadPortraitFrameDataManager.GetHeadPortraitFramePath(
                    _teamInviteDataModel.HeadFrameId);
                if (teamLeaderHeadFrame != null && string.IsNullOrEmpty(headFramePath) == false)
                {
                    ETCImageLoader.LoadSprite(ref teamLeaderHeadFrame, headFramePath);
                }
            }

            //等级
            if (teamLeaderLevel != null)
            {
                teamLeaderLevel.text = string.Format(TR.Value("team_duplication_team_leader_level_format"),
                    _teamInviteDataModel.TeamLeaderLevel);
            }

            if (teamLeaderName != null)
            {
                teamLeaderName.text = _teamInviteDataModel.TeamLeaderName;
            }

            //邀请团队的难度
            if (teamBaseDescription != null)
            {
                CommonUtility.UpdateTextVisible(teamBaseDescription, true);

                //噩梦难度
                if ((TeamCopyTeamGrade) _teamInviteDataModel.TeamDifficultyLevel ==
                    TeamCopyTeamGrade.TEAM_COPY_TEAM_GRADE_DIFF)
                {
                    teamBaseDescription.text = TR.Value("team_duplication_invite_in_hard_level_team");
                }
                else
                {
                    //普通难度
                    teamBaseDescription.text = TR.Value("team_duplication_invite_in_normal_level_team");
                }
            }

            if (_teamInviteDataModel.TeamType == TeamCopyTeamModel.TEAM_COPY_TEAM_MODEL_GOLD)
            {
                CommonUtility.UpdateTextVisible(teamGoldDescription, true);
            }
            else
            {
                CommonUtility.UpdateTextVisible(teamGoldDescription, false);
            }
        }

        #region ItemButtonClicked
        private void OnRefuseButtonClick()
        {
            OnSendTeamCopyInviteChoiceReq(false);
        }

        private void OnAgreeButtonClick()
        {
            OnSendTeamCopyInviteChoiceReq(true);
        }

        private void OnDetailButtonClick()
        {
            if (_teamInviteDataModel == null)
                return;

            //展示邀请者的团本详情
            TeamDuplicationUtility.OnOpenTeamDuplicationTeamRoomFrame(
                (int)_teamInviteDataModel.TeamId);
        }
        #endregion

        private void OnSendTeamCopyInviteChoiceReq(bool isAgree)
        {
            if (_teamInviteDataModel == null)
                return;

            var teamIdList = GetTeamInviteTeamIdList();
            if (teamIdList == null || teamIdList.Count <= 0)
                return;

            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyInviteChoiceReq(isAgree,
                teamIdList);
        }

        private List<uint> GetTeamInviteTeamIdList()
        {
            if (_teamInviteDataModel == null)
                return null;

            List<uint> teamIdList = new List<uint>();
            teamIdList.Add(_teamInviteDataModel.TeamId);

            return teamIdList;
        }

        public void Reset()
        {
            _teamInviteDataModel = null;
        }


    }
}
