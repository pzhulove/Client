using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class TeamDuplicationTeamInviteListView : MonoBehaviour
    {
        private List<TeamDuplicationTeamInviteDataModel> _teamInviteDataModelList;

        [Space(15)]
        [HeaderAttribute("top")]
        [Space(10)]
        [SerializeField] private Text titleText;
        [SerializeField] private Button closeButton;

        [Space(15)] [HeaderAttribute("TeamInviteList")] [Space(15)]
        [SerializeField] private ComUIListScriptEx teamInviteItemList;
        [SerializeField] private Text noTeamInviteTipText;

        [Space(15)] [HeaderAttribute("Bottom")] [Space(15)]
        [SerializeField] private ComButtonWithCd refuseAllButton;

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
            _teamInviteDataModelList = null;
        }

        private void BindUiEvents()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseButtonClick);
            }

            if (refuseAllButton != null)
            {
                refuseAllButton.ResetButtonListener();
                refuseAllButton.SetButtonListener(OnRefuseAllButtonClick);
            }

            if (teamInviteItemList != null)
            {
                teamInviteItemList.Initialize();
                teamInviteItemList.onItemVisiable += OnTeamInviteItemVisible;
                teamInviteItemList.OnItemRecycle += OnTeamInviteItemRecycle;
            }
        }

        private void UnBindUiEvents()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveAllListeners();

            if(refuseAllButton != null)
                refuseAllButton.ResetButtonListener();

            if (teamInviteItemList != null)
            {
                teamInviteItemList.onItemVisiable -= OnTeamInviteItemVisible;
                teamInviteItemList.OnItemRecycle -= OnTeamInviteItemRecycle;
            }
        }

        private void OnEnable()
        {
            //删除某些队伍后，进行刷新
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamInviteChoiceMessage,
                OnReceiveTeamDuplicationTeamInviteListMessage);

            //收到团本邀请消息，进行刷新
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamInviteListMessage,
                OnReceiveTeamDuplicationTeamInviteListMessage);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamInviteChoiceMessage,
                OnReceiveTeamDuplicationTeamInviteListMessage);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamInviteListMessage,
                OnReceiveTeamDuplicationTeamInviteListMessage);
        }

        public void Init()
        {
            InitView();

            //发送消息
            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyInviteListReq();
        }

        private void InitView()
        {
            InitCommonView();

        }

        private void InitCommonView()
        {
            if (titleText != null)
                titleText.text = TR.Value("team_duplication_team_invite_list_title");

            if (noTeamInviteTipText != null)
                noTeamInviteTipText.text = TR.Value("team_duplication_team_invite_list_zero");

            if (teamInviteItemList != null)
                teamInviteItemList.SetElementAmount(0);

        }

        //收到消息
        private void OnReceiveTeamDuplicationTeamInviteListMessage(UIEvent uiEvent)
        {
            _teamInviteDataModelList = TeamDuplicationDataManager.GetInstance().TeamInviteDataModelList;
            var teamInviteNumber = 0;
            if (_teamInviteDataModelList != null)
                teamInviteNumber = _teamInviteDataModelList.Count;

            if (teamInviteItemList != null)
                teamInviteItemList.SetElementAmount(teamInviteNumber);
        }

        #region TeamList

        private void OnTeamInviteItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (teamInviteItemList == null)
                return;

            if (_teamInviteDataModelList == null || _teamInviteDataModelList.Count <= 0)
                return;

            if (item.m_index < 0 || item.m_index >= _teamInviteDataModelList.Count)
                return;

            var teamInviteItem = item.GetComponent<TeamDuplicationTeamInviteItem>();
            var teamInviteDataModel = _teamInviteDataModelList[item.m_index];

            if (teamInviteItem != null && teamInviteDataModel != null)
                teamInviteItem.Init(teamInviteDataModel);
        }

        private void OnTeamInviteItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (teamInviteItemList == null)
                return;

            var teamInviteItem = item.GetComponent<TeamDuplicationTeamInviteItem>();
            if(teamInviteItem != null)
                teamInviteItem.Reset();
        }

        #endregion

        private void OnRefuseAllButtonClick()
        {
            if (_teamInviteDataModelList == null || _teamInviteDataModelList.Count <= 0)
            {
                //没有队伍的时候，点击没有反应，不再飘字
                //SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_team_invite_list_zero"));
                return;
            }

            List<uint> teamIdList = new List<uint>();
            for (var i = 0; i < _teamInviteDataModelList.Count; i++)
            {
                var teamInviteDataModel = _teamInviteDataModelList[i];
                if(teamInviteDataModel == null)
                    continue;

                teamIdList.Add(teamInviteDataModel.TeamId);
            }

            if (teamIdList.Count <= 0)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_team_invite_list_zero"));
                return;
            }

            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyInviteChoiceReq(false,
                teamIdList);
        }

        private void OnCloseButtonClick()
        {
            OnCloseFrame();
        }

        private void OnCloseFrame()
        {
            TeamDuplicationDataManager.GetInstance().ResetTeamDuplicationTeamInviteDataModelList();

            TeamDuplicationUtility.OnCloseTeamDuplicationTeamInviteListFrame();
        }

    }
}
