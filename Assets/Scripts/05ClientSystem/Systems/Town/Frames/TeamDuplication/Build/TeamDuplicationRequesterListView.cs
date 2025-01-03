using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class TeamDuplicationRequesterListView : MonoBehaviour
    {

        private List<TeamDuplicationRequesterDataModel> _requesterDataModelList = null;


        [Space(15)]
        [HeaderAttribute("Common")]
        [Space(10)]
        [SerializeField] private Text titleLabel;
        [SerializeField] private Button closeButton;

        [Space(15)]
        [HeaderAttribute("Button")]
        [Space(10)]
        [SerializeField] private Button refuseAllButton;
        [SerializeField] private Button agreeAllButton;
        [SerializeField] private ComButtonWithCd refreshButton;
        [SerializeField] private Text refreshButtonText;


        [Space(15)]
        [HeaderAttribute("ComUIListScript")]
        [Space(10)]
        [SerializeField]
        private ComUIListScriptEx requesterItemList;

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
            _requesterDataModelList = null;
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
                refuseAllButton.onClick.RemoveAllListeners();
                refuseAllButton.onClick.AddListener(OnRefuseAllButtonClick);
            }

            if (agreeAllButton != null)
            {
                agreeAllButton.onClick.RemoveAllListeners();
                agreeAllButton.onClick.AddListener(OnAgreeAllButtonClick);
            }

            if (refreshButton != null)
            {
                refreshButton.ResetListener();
                refreshButton.SetButtonListener(OnRefreshButtonClick);
                refreshButton.SetCountDownTimeDescription(TR.Value("team_duplication_count_down_time_refresh_content"),
                    TR.Value("team_duplication_count_down_time_refresh_format"));
            }

            if (requesterItemList != null)
            {
                requesterItemList.Initialize();
                requesterItemList.onItemVisiable += OnItemVisible;
                requesterItemList.OnItemRecycle += OnItemRecycle;
            }

        }

        private void UnBindUiEvents()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveAllListeners();


            if (refuseAllButton != null)
            {
                refuseAllButton.onClick.RemoveAllListeners();
            }

            if (agreeAllButton != null)
            {
                agreeAllButton.onClick.RemoveAllListeners();
            }

            if (refreshButton != null)
            {
                refreshButton.ResetButtonListener();
            }

            if (requesterItemList != null)
            {
                requesterItemList.onItemVisiable -= OnItemVisible;
                requesterItemList.OnItemRecycle -= OnItemRecycle;
            }
        }

        private void OnEnable()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationRequesterListMessage,
                OnReceiveTeamDuplicationRequesterListMessage);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationRequesterListMessage,
                OnReceiveTeamDuplicationRequesterListMessage);
        }

        public void Init()
        {
            //发送请求列表
            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyTeamApplyListReq();

            InitView();
        }

        private void InitView()
        {
            InitCommonView();
        }

        private void InitCommonView()
        {
            if (titleLabel != null)
                titleLabel.text = TR.Value("team_duplication_troop_request_list_title");

            if (refreshButtonText != null)
                refreshButtonText.text = TR.Value("team_duplication_count_down_time_refresh_content");
        }

        #region friendList

        private void OnReceiveTeamDuplicationRequesterListMessage(UIEvent uiEvent)
        {
            _requesterDataModelList = TeamDuplicationDataManager.GetInstance().GetRequesterDataModelList();

            UpdateFriendList();
        }

        private void UpdateFriendList()
        {
            if (requesterItemList == null)
                return;

            requesterItemList.ResetComUiListScriptEx();
            var requesterNumber = 0;
            if (_requesterDataModelList == null || _requesterDataModelList.Count <= 0)
                requesterNumber = 0;
            else
                requesterNumber = _requesterDataModelList.Count;

            requesterItemList.SetElementAmount(requesterNumber);
        }


        private void OnItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (requesterItemList == null)
                return;

            if (_requesterDataModelList == null || _requesterDataModelList.Count <= 0)
                return;

            if (item.m_index >= _requesterDataModelList.Count)
                return;

            var requesterDataModel = _requesterDataModelList[item.m_index];
            var teamRequesterItem = item.GetComponent<TeamDuplicationTeamRequesterItem>();
            if (requesterDataModel != null
                && teamRequesterItem != null)
                teamRequesterItem.Init(requesterDataModel,
                    true);
        }

        private void OnItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (requesterItemList == null)
                return;

            var requesterItem = item.GetComponent<TeamDuplicationTeamRequesterItem>();
            if (requesterItem != null)
                requesterItem.Reset();
        }


        #endregion

        #region Button

        //刷新列表
        private void OnRefreshButtonClick()
        {
            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyTeamApplyListReq();
        }

        //拒绝所有
        private void OnRefuseAllButtonClick()
        {            
            if (_requesterDataModelList == null || _requesterDataModelList.Count <= 0)
                return;

            var playerGuidList = GetAllRequesterGuid();

            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyTeamApplyReplyReq(false,
                playerGuidList);
        }

        //同意所有
        private void OnAgreeAllButtonClick()
        {
            if (TeamDuplicationUtility.IsTeamTroopIsFull() == true)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_team_troop_is_full"));
                return;
            }

            if (_requesterDataModelList == null || _requesterDataModelList.Count <= 0)
                return;

            var playerGuidList = GetAllRequesterGuid();

            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyTeamApplyReplyReq(true,
                playerGuidList);

        }


        private void OnCloseButtonClick()
        {
            TeamDuplicationUtility.OnCloseTeamDuplicationRequesterListFrame();
        }
        #endregion

        private List<ulong> GetAllRequesterGuid()
        {
            List<ulong> playerGuidList = new List<ulong>();

            for (var i = 0; i < _requesterDataModelList.Count; i++)
            {
                var curDataModel = _requesterDataModelList[i];
                if (curDataModel != null)
                    playerGuidList.Add(curDataModel.PlayerGuid);
            }

            return playerGuidList;
        }

    }
}
