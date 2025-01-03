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

    //请求Item
    public class TeamDuplicationTeamRequesterItem : MonoBehaviour
    {
        private TeamDuplicationRequesterDataModel _requesterDataModel;
        private bool _isRequester = false;     //是否为请求者

        [Space(10)]
        [HeaderAttribute("requestPlayerType")]
        [Space(5)]
        [SerializeField]
        private GameObject goldRoot;


        [Space(10)]
        [HeaderAttribute("Text")]
        [Space(5)]
        [SerializeField] private Text nameText;

        [SerializeField] private Text professionNameText;
        [SerializeField] private Text levelText;
        [SerializeField] private Text equipmentScoreText;

        [Space(10)]
        [HeaderAttribute("SearchButton")]
        [Space(5)]
        [SerializeField]
        private Button searchButton;

        [Space(10)]
        [HeaderAttribute("RequestButton")]
        [Space(5)]
        [SerializeField] private GameObject inviteButtonRoot;
        [SerializeField] private ComButtonWithCd inviteButton;

        [Space(10)]
        [HeaderAttribute("AgreeButton")]
        [Space(5)]
        [SerializeField] private GameObject requestButtonRoot;
        [SerializeField] private ComButtonWithCd refuseButton;
        [SerializeField] private ComButtonWithCd agreeButton;

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
            if (searchButton != null)
            {
                searchButton.onClick.RemoveAllListeners();
                searchButton.onClick.AddListener(OnSearchButtonClick);
            }

            if (inviteButton != null)
            {
                inviteButton.ResetListener();
                inviteButton.SetButtonListener(OnInviteButtonClick);
            }

            if (refuseButton != null)
            {
                refuseButton.ResetListener();
                refuseButton.SetButtonListener(OnRefuseButtonClick);
            }

            if (agreeButton != null)
            {
                agreeButton.ResetListener();
                agreeButton.SetButtonListener(OnAgreeButtonClick);
            }
        }

        private void UnBindUiEvents()
        {
            if (searchButton != null)
            {
                searchButton.onClick.RemoveAllListeners();
            }

            if (inviteButton != null)
            {
                inviteButton.ResetListener();
            }

            if (refuseButton != null)
            {
                refuseButton.ResetListener();
            }

            if (agreeButton != null)
            {
                agreeButton.ResetListener();
            }
        }

        private void ClearData()
        {
            _requesterDataModel = null;
            _isRequester = false;
        }


        public void Init(TeamDuplicationRequesterDataModel requesterDataModel,
            bool isRequester = true)
        {
            _requesterDataModel = requesterDataModel;
            _isRequester = isRequester;

            if (_requesterDataModel == null)
                return;

            InitItem();
        }

        private void InitItem()
        {
            SetRequestPlayerType();

            if (nameText != null)
                nameText.text = _requesterDataModel.Name;

            if (professionNameText != null)
                professionNameText.text = TeamDuplicationUtility.GetJobName(_requesterDataModel.ProfessionId);

            if (levelText != null)
                levelText.text = _requesterDataModel.Level.ToString();

            if (equipmentScoreText != null)
                equipmentScoreText.text = _requesterDataModel.EquipmentScore.ToString();

            UpdateButtonRoot();
        }

        //设置类型
        private void SetRequestPlayerType()
        {
            if (goldRoot == null)
                return;

            if (_requesterDataModel.IsGold == true)
            {
                CommonUtility.UpdateGameObjectVisible(goldRoot, true);
            }
            else
            {
                CommonUtility.UpdateGameObjectVisible(goldRoot, false);
            }
        }


        private void UpdateButtonRoot()
        {
            ResetButtonWithCd();

            if (_isRequester == true)
            {
                CommonUtility.UpdateGameObjectVisible(requestButtonRoot, true);
                CommonUtility.UpdateGameObjectVisible(inviteButtonRoot, false);
            }
            else
            {
                CommonUtility.UpdateGameObjectVisible(requestButtonRoot, false);
                CommonUtility.UpdateGameObjectVisible(inviteButtonRoot, true);
            }
        }

        private void ResetButtonWithCd()
        {
            if (inviteButton != null)
                inviteButton.Reset();

            if (agreeButton != null)
                agreeButton.Reset();

            if (refuseButton != null)
                refuseButton.Reset();
        }


        private void OnSearchButtonClick()
        {
            if (_requesterDataModel == null)
                return;

            if (_requesterDataModel.PlayerGuid <= 0)
                return;

            //查看队友详细信息
            OtherPlayerInfoManager.GetInstance().SendWatchOtherPlayerInfo(
                _requesterDataModel.PlayerGuid,
                (uint) QueryPlayerType.QPT_TEAM_COPY,
                (uint) _requesterDataModel.ZoneId);
        }

        private void OnInviteButtonClick()
        {
            if (_requesterDataModel == null)
                return;

            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_team_troop_invite_friend"));

            var playerIdList = GetPlayerIdList();
            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyInvitePlayer(playerIdList);
        }

        private void OnRefuseButtonClick()
        {
            if (_requesterDataModel == null)
                return;

            var playerIdList = GetPlayerIdList();
            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyTeamApplyReplyReq(false,
                playerIdList);
        }

        private void OnAgreeButtonClick()
        {
            if (_requesterDataModel == null)
                return;

            var playerIdList = GetPlayerIdList();
            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyTeamApplyReplyReq(true,
                playerIdList);
        }

        private List<ulong> GetPlayerIdList()
        {
            List<ulong> playerIdList = new List<ulong>();
            if (_requesterDataModel != null)
                playerIdList.Add(_requesterDataModel.PlayerGuid);

            return playerIdList;
        }

        public void OnInviteButtonUpdate()
        {
            if (inviteButton != null)
            {
                inviteButton.Reset();
                var countDownTime = inviteButton.GetButtonCdTime();
                inviteButton.SetButtonTimeLimit(countDownTime);
            }
        }

        public void Reset()
        {
            ClearData();
        }

    }
}
