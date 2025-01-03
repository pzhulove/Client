using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class TeamDuplicationFindTeamMateView : MonoBehaviour
    {
        private List<TeamDuplicationRequesterDataModel> _teamMateDataModelList = null;

        [Space(15)]
        [HeaderAttribute("Common")]
        [Space(10)]
        [SerializeField] private Text titleLabel;
        [SerializeField] private Button closeButton;

        [Space(15)]
        [HeaderAttribute("Button")]
        [Space(10)]
        [SerializeField] private ComButtonWithCd inviteAllButton;
        [SerializeField] private ComButtonWithCd recruitSendButton;
        [SerializeField] private ComButtonWithCd changeTeamMateButton;

        [Space(15)]
        [HeaderAttribute("TeamMateItemList")]
        [Space(10)]
        [SerializeField]
        private ComUIListScriptEx teamMateItemList;

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
            _teamMateDataModelList = null;
        }

        private void BindUiEvents()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseButtonClick);
            }

            if (inviteAllButton != null)
            {
                inviteAllButton.ResetListener();
                inviteAllButton.SetButtonListener(OnInviteAllButtonClick);
            }

            if (recruitSendButton != null)
            {
                recruitSendButton.ResetListener();
                recruitSendButton.SetButtonListener(OnRecruitSendButtonClick);
            }

            if (changeTeamMateButton != null)
            {
                changeTeamMateButton.ResetListener();
                changeTeamMateButton.SetButtonListener(OnChangeTeamMateButtonClick);
                changeTeamMateButton.SetCountDownTimeDescription(TR.Value("team_duplication_change_teamMate_count_down_format_content"),
                    TR.Value("team_duplication_change_teamMate_count_down_refresh_content"));
            }

            if (teamMateItemList != null)
            {
                teamMateItemList.Initialize();
                teamMateItemList.onItemVisiable += OnItemVisible;
                teamMateItemList.OnItemRecycle += OnItemRecycle;
                teamMateItemList.OnItemUpdate += OnItemUpdate;
            }
        }

        private void UnBindUiEvents()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveAllListeners();

            if (inviteAllButton != null)
            {
                inviteAllButton.ResetListener();
            }

            if (recruitSendButton != null)
            {
                recruitSendButton.ResetListener();
            }

            if (changeTeamMateButton != null)
            {
                changeTeamMateButton.ResetButtonListener();
            }

            if (teamMateItemList != null)
            {
                teamMateItemList.onItemVisiable -= OnItemVisible;
                teamMateItemList.OnItemRecycle -= OnItemRecycle;
                teamMateItemList.OnItemUpdate -= OnItemUpdate;
            }
        }

        private void OnEnable()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamMateListMessage,
                OnReceiveTeamDuplicationTeamMateMessage);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamMateListMessage,
                OnReceiveTeamDuplicationTeamMateMessage);
        }

        public void Init()
        {
            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyFindTeamMateReq();

            InitView();
        }

        private void InitView()
        {
            InitCommonView();
        }

        private void InitCommonView()
        {
            if (titleLabel != null)
                titleLabel.text = TR.Value("team_duplication_troop_find_player_title");
        }

        #region TeamMateList

        private void OnReceiveTeamDuplicationTeamMateMessage(UIEvent uiEvent)
        {
            _teamMateDataModelList = TeamDuplicationDataManager.GetInstance().GetTeamFriendDataModelList();

            UpdateTeamMateList();
        }

        private void UpdateTeamMateList()
        {
            if (teamMateItemList == null)
                return;

            teamMateItemList.ResetComUiListScriptEx();
            var teamMateNumber = 0;
            if (_teamMateDataModelList == null || _teamMateDataModelList.Count <= 0)
                teamMateNumber = 0;
            else
                teamMateNumber = _teamMateDataModelList.Count;

            teamMateItemList.SetElementAmount(teamMateNumber);
        }


        private void OnItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (teamMateItemList == null)
                return;

            if (_teamMateDataModelList == null || _teamMateDataModelList.Count <= 0)
                return;

            if (item.m_index >= _teamMateDataModelList.Count)
                return;

            var teamMateDataModel = _teamMateDataModelList[item.m_index];
            var teamRequesterItem = item.GetComponent<TeamDuplicationTeamRequesterItem>();
            if (teamMateDataModel != null
                && teamRequesterItem != null)
                teamRequesterItem.Init(teamMateDataModel,
                    false);
        }

        private void OnItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (teamMateItemList == null)
                return;

            var teamRequesterItem = item.GetComponent<TeamDuplicationTeamRequesterItem>();
            if (teamRequesterItem != null)
                teamRequesterItem.Reset();
        }

        private void OnItemUpdate(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (teamMateItemList == null)
                return;

            if (_teamMateDataModelList == null || _teamMateDataModelList.Count <= 0)
                return;

            if (item.m_index >= _teamMateDataModelList.Count)
                return;

            var teamRequesterItem = item.GetComponent<TeamDuplicationTeamRequesterItem>();
            if(teamRequesterItem != null)
                teamRequesterItem.OnInviteButtonUpdate();
        }
        
        #endregion

        #region Button

        //全部邀请
        private void OnInviteAllButtonClick()
        {
            if (_teamMateDataModelList == null || _teamMateDataModelList.Count <= 0)
                return;

            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_team_troop_invite_friend"));

            var teamMateGuidList = GetAllTeamMateGuid();
            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyInvitePlayer(teamMateGuidList);

            if (teamMateItemList != null)
                teamMateItemList.UpdateElement();

        }

        private void OnRecruitSendButtonClick()
        {
            TeamDuplicationDataManager.GetInstance().OnSendTeamRecruitReq();
        }

        private void OnChangeTeamMateButtonClick()
        {
            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyFindTeamMateReq();
        }

        private void OnCloseButtonClick()
        {
            TeamDuplicationUtility.OnCloseTeamDuplicationFindTeamMateFrame();
        }
        #endregion

        private List<ulong> GetAllTeamMateGuid()
        {
            List<ulong> playerGuidList = new List<ulong>();

            for (var i = 0; i < _teamMateDataModelList.Count; i++)
            {
                var curDataModel = _teamMateDataModelList[i];
                if (curDataModel != null)
                    playerGuidList.Add(curDataModel.PlayerGuid);
            }

            return playerGuidList;
        }

    }
}
