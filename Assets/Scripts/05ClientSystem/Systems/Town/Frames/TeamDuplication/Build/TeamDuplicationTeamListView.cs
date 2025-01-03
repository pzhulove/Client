using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class TeamDuplicationTeamListView : MonoBehaviour
    {

        private List<TeamDuplicationTeamListDataModel> _teamListDataModelList;
        private int _curPageNumber;
        private int _totalPageNumber;
        private int _teamDuplicationTeamModelType = 0;

        private List<ComControlData> _teamTypeDataList;

        [Space(15)]
        [HeaderAttribute("Text")]
        [Space(10)]
        [SerializeField] private Text titleLabel;

        [Space(15)]
        [HeaderAttribute("Number")]
        [Space(10)]
        [SerializeField] private Text todayLabel;
        [SerializeField] private Text todayValueText;
        [SerializeField] private Text weekLabel;
        [SerializeField] private Text weekValueText;
        [SerializeField] private Text hardLevelLabel;
        [SerializeField] private Text hardLevelValueText;

        [Space(15)]
        [HeaderAttribute("Button")]
        [Space(10)]
        [SerializeField] private Button closeButton;

        [SerializeField] private Button buildTeamButton;
        [SerializeField] private Button inviteButton;
        [SerializeField] private GameObject inviteButtonRedPoint;

        [Space(15)]
        [HeaderAttribute("Button")]
        [Space(10)]
        [SerializeField] private Button prePageButton;
        [SerializeField] private UIGray prePageGray;
        [SerializeField] private Button nextPageButton;
        [SerializeField] private UIGray nextPageGray;
        [SerializeField] private Text pageValue;

        [Space(15)]
        [HeaderAttribute("Gold")]
        [Space(10)]
        [SerializeField] private ComButtonWithCd goldSelectedButtonWithCd;
        [SerializeField] private Image goldSelectedFlag;

        [Space(20)]
        [HeaderAttribute("TeamTypeDropDown")]
        [Space(20)]
        [SerializeField] private CommonNewDropDownControl teamTypeDropDownControl;
        [SerializeField] private GameObject teamTypeDropDownBeforeCover;

        [Space(15)]
        [HeaderAttribute("Control")]
        [Space(10)]
        [SerializeField] private ComUIListScriptEx teamListItemList;

        #region Init
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
            _teamListDataModelList = null;
            _curPageNumber = 1;
            _totalPageNumber = 1;
            _teamTypeDataList = null;
            _teamDuplicationTeamModelType = 0;
        }

        private void BindUiEvents()
        {
            if (teamListItemList != null)
            {
                teamListItemList.Initialize();
                teamListItemList.onItemVisiable += OnItemVisible;
                teamListItemList.OnItemRecycle += OnItemRecycle;
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseButtonClick);
            }

            if (buildTeamButton != null)
            {
                buildTeamButton.onClick.RemoveAllListeners();
                buildTeamButton.onClick.AddListener(OnBuildTeamButtonClick);
            }

            if (inviteButton != null)
            {
                inviteButton.onClick.RemoveAllListeners();
                inviteButton.onClick.AddListener(OnTeamInviteButtonClick);
            }

            if (goldSelectedButtonWithCd != null)
            {
                goldSelectedButtonWithCd.ResetButtonListener();
                goldSelectedButtonWithCd.SetButtonListener(OnGoldSelectedButtonClick);
            }

            if (prePageButton != null)
            {
                prePageButton.onClick.RemoveAllListeners();
                prePageButton.onClick.AddListener(OnPrePageButtonClick);
            }

            if (nextPageButton != null)
            {
                nextPageButton.onClick.RemoveAllListeners();
                nextPageButton.onClick.AddListener(OnNextPageButtonClick);
            }
        }

        private void UnBindUiEvents()
        {
            if (teamListItemList != null)
            {
                teamListItemList.onItemVisiable -= OnItemVisible;
                teamListItemList.OnItemRecycle -= OnItemRecycle;
            }

            if (closeButton != null)
                closeButton.onClick.RemoveAllListeners();

            if (buildTeamButton != null)
                buildTeamButton.onClick.RemoveAllListeners();

            if (inviteButton != null)
                inviteButton.onClick.RemoveAllListeners();

            if (goldSelectedButtonWithCd != null)
                goldSelectedButtonWithCd.ResetButtonListener();

            if (prePageButton != null)
                prePageButton.onClick.RemoveAllListeners();

        }

        private void OnEnable()
        {
            //刷新队伍列表
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationRefreshTeamListMessage,
                OnReceiveTeamDuplicationRefreshTeamListMessage);

            //收到队伍列表数据，进行展示
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamListRes,
                OnReceiveTeamDuplicationTeamListRes);

            //刷新邀请者的红点
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationOwnerNewTeamInviteMessage,
                OnReceiveTeamDuplicationOwnerNewInviteMessage);

            //团长拒绝的提示
            UIEventSystem.GetInstance().RegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationTeamLeaderRefuseJoinInMessage,
                OnReceiveTeamDuplicationTeamLeaderRefuseJoinInMessage);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationRefreshTeamListMessage,
                OnReceiveTeamDuplicationRefreshTeamListMessage);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamListRes,
                OnReceiveTeamDuplicationTeamListRes);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationOwnerNewTeamInviteMessage,
                OnReceiveTeamDuplicationOwnerNewInviteMessage);

            UIEventSystem.GetInstance().UnRegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationTeamLeaderRefuseJoinInMessage,
                OnReceiveTeamDuplicationTeamLeaderRefuseJoinInMessage);
        }

        #endregion

        public void Init()
        {
            _curPageNumber = 1;
            _totalPageNumber = 1;

            _teamTypeDataList = TeamDuplicationUtility.GetTeamDuplicationTeamTypeDataList();

            _teamDuplicationTeamModelType = (int) TeamCopyTeamModel.TEAM_COPY_TEAM_MODEL_DEFAULT;
            if (TeamDuplicationDataManager.GetInstance().IsTeamDuplicationGoldOwner == true)
                _teamDuplicationTeamModelType = (int) TeamCopyTeamModel.TEAM_COPY_TEAM_MODEL_GOLD;

            InitView();

            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyTeamListReq(_curPageNumber,
                _teamDuplicationTeamModelType);
        }

        #region InitView
        private void InitView()
        {
            InitCommonView();
            UpdateFightNumber();

            UpdateGoldSelectedFlag();

            InitTeamTypeDropDownControl();

            UpdateInviteButtonRedPoint();
        }

        private void InitCommonView()
        {
            if (titleLabel != null)
                titleLabel.text = TR.Value("team_duplication_team_list_title");

            if (todayLabel != null)
                todayLabel.text = TR.Value("team_duplication_team_today_label");

            if (weekLabel != null)
                weekLabel.text = TR.Value("team_duplication_team_week_label");

            if (hardLevelLabel != null)
                hardLevelLabel.text = TR.Value("team_duplication_hard_level_label");
        }

        //更新展示的次数
        private void UpdateFightNumber()
        {
            var playerInformationDataModel = TeamDuplicationDataManager.GetInstance().GetPlayerInformationDataModel();

            //每日次数
            if (todayValueText != null)
            {
                if (playerInformationDataModel.DayAlreadyFightNumber >= playerInformationDataModel.DayTotalFightNumber)
                {
                    todayValueText.text = string.Format(TR.Value("team_duplication_team_fight_number_full_type"),
                        playerInformationDataModel.DayAlreadyFightNumber,
                        playerInformationDataModel.DayTotalFightNumber);
                }
                else
                {
                    todayValueText.text = string.Format(TR.Value("team_duplication_team_fight_number_normal_type"),
                        playerInformationDataModel.DayAlreadyFightNumber,
                        playerInformationDataModel.DayTotalFightNumber);
                }
            }

            //每周次数
            if (weekValueText != null)
            {
                if (playerInformationDataModel.WeekAlreadyFightNumber >=
                    playerInformationDataModel.WeekTotalFightNumber)
                {
                    weekValueText.text = string.Format(TR.Value("team_duplication_team_fight_number_full_type"),
                        playerInformationDataModel.WeekAlreadyFightNumber,
                        playerInformationDataModel.WeekTotalFightNumber);
                }
                else
                {
                    weekValueText.text = string.Format(TR.Value("team_duplication_team_fight_number_normal_type"),
                        playerInformationDataModel.WeekAlreadyFightNumber,
                        playerInformationDataModel.WeekTotalFightNumber);
                }
            }

            //噩梦难度挑战次数
            if (hardLevelValueText != null)
            {
                if (playerInformationDataModel.HardLevelAlreadyFightNumber >=
                    playerInformationDataModel.HardLevelTotalFightNumber)
                {
                    hardLevelValueText.text = string.Format(TR.Value("team_duplication_team_fight_number_full_type"),
                        playerInformationDataModel.HardLevelAlreadyFightNumber,
                        playerInformationDataModel.HardLevelTotalFightNumber);
                }
                else
                {
                    hardLevelValueText.text = string.Format(TR.Value("team_duplication_team_fight_number_normal_type"),
                        playerInformationDataModel.HardLevelAlreadyFightNumber,
                        playerInformationDataModel.HardLevelTotalFightNumber);
                }
            }

        }

        private void InitTeamTypeDropDownControl()
        {
            var defaultTypeData = _teamTypeDataList[0];
            if (TeamDuplicationDataManager.GetInstance().IsTeamDuplicationGoldOwner == true)
                defaultTypeData = GetGoldTeamModel();

            if (teamTypeDropDownControl)
            {
                teamTypeDropDownControl.InitComDropDownControl(defaultTypeData,
                    _teamTypeDataList,
                    OnTeamTypeDropDownItemSelected);
            }

            UpdateTeamTypeDropDownBeforeCover();
        }

        private void UpdateTeamTypeDropDownBeforeCover()
        {
            if (teamTypeDropDownBeforeCover == null)
                return;

            CommonUtility.UpdateGameObjectVisible(teamTypeDropDownBeforeCover,
                TeamDuplicationDataManager.GetInstance().IsTeamDuplicationGoldOwner);
        }

        #endregion

        #region UpdateTeamList

        private void UpdateTeamListData()
        {
            _curPageNumber = TeamDuplicationDataManager.GetInstance().TeamListCurrentPage;
            _totalPageNumber = TeamDuplicationDataManager.GetInstance().TeamListTotalPage;

            if (_totalPageNumber < 1)
                _totalPageNumber = 1;

            if (_curPageNumber < 1)
                _curPageNumber = 1;

            if (_curPageNumber > _totalPageNumber)
                _curPageNumber = _totalPageNumber;


            _teamListDataModelList =
                TeamDuplicationDataManager.GetInstance().GetTeamDuplicationTeamListDataModelList();

        }

        private void UpdatePageView()
        {
            if (pageValue != null)
            {
                pageValue.text = string.Format(TR.Value("team_duplication_team_list_page_number"),
                    _curPageNumber,
                    _totalPageNumber);
            }

            if (_curPageNumber <= 1)
            {
                if (_totalPageNumber <= 1)
                {
                    CommonUtility.UpdateButtonState(prePageButton, prePageGray, false);
                    CommonUtility.UpdateButtonState(nextPageButton, nextPageGray, false);
                }
                else
                {
                    CommonUtility.UpdateButtonState(prePageButton, prePageGray, false);
                    CommonUtility.UpdateButtonState(nextPageButton, nextPageGray, true);
                }
            }
            else
            {
                if (_curPageNumber >= _totalPageNumber)
                {
                    CommonUtility.UpdateButtonState(prePageButton, prePageGray, true);
                    CommonUtility.UpdateButtonState(nextPageButton, nextPageGray, false);
                }
                else
                {
                    CommonUtility.UpdateButtonState(prePageButton, prePageGray, true);
                    CommonUtility.UpdateButtonState(nextPageButton, nextPageGray, true);

                }
            }
        }

        private void UpdateTeamListView()
        {
            //更新List
            if (teamListItemList != null)
            {
                //首先进行重置
                teamListItemList.ResetComUiListScriptEx();

                if (_teamListDataModelList == null || _teamListDataModelList.Count <= 0)
                {
                    teamListItemList.SetElementAmount(0);
                }
                else
                {
                    teamListItemList.SetElementAmount(_teamListDataModelList.Count);
                }
            }
        }

        private void OnItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (_teamListDataModelList == null || _teamListDataModelList.Count <= 0)
                return;

            if (item.m_index >= _teamListDataModelList.Count)
                return;

            TeamDuplicationTeamListDataModel teamDataModel = _teamListDataModelList[item.m_index];
            TeamDuplicationTeamListItem teamListItem = item.GetComponent<TeamDuplicationTeamListItem>();

            if (teamDataModel != null && teamListItem != null)
                teamListItem.Init(teamDataModel);
        }

        private void OnItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
                return;
            if (teamListItemList == null)
                return;

            var teamListItem = item.GetComponent<TeamDuplicationTeamListItem>();
            if(teamListItem != null)
                teamListItem.Reset();
        }

        private void OnPrePageButtonClick()
        {
            if (_curPageNumber <= 1)
                return;

            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyTeamListReq(_curPageNumber - 1,
                _teamDuplicationTeamModelType);
        }

        private void OnNextPageButtonClick()
        {
            if (_curPageNumber >= _totalPageNumber)
                return;

            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyTeamListReq(_curPageNumber + 1,
                _teamDuplicationTeamModelType);
        }

        #endregion

        private void OnBuildTeamButtonClick()
        {
            TeamDuplicationUtility.OnCloseTeamDuplicationTeamListFrame();

            TeamDuplicationUtility.OnOpenTeamDuplicationTeamBuildFrame();

        }

        #region TeamTypeDropDown
        private void OnTeamTypeDropDownItemSelected(ComControlData comControlData)
        {
            //选择模式

            if (comControlData == null)
                return;

            _teamDuplicationTeamModelType = comControlData.Id;

            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyTeamListReq(_curPageNumber,
                _teamDuplicationTeamModelType);
        }

        private void UpdateTeamTypeDropDownItemList()
        {
            //选择我是金主
            if (TeamDuplicationDataManager.GetInstance().IsTeamDuplicationGoldOwner == true)
            {
                if (teamTypeDropDownControl == null)
                    return;

                if (_teamTypeDataList == null
                    || _teamTypeDataList.Count <= 0)
                    return;

                var curTeamTypeData = GetGoldTeamModel();
                _teamDuplicationTeamModelType = curTeamTypeData.Id;

                teamTypeDropDownControl.UpdateCommonNewDropDownSelectedItem(curTeamTypeData);

                TeamDuplicationDataManager.GetInstance().OnSendTeamCopyTeamListReq(_curPageNumber,
                    _teamDuplicationTeamModelType);
            }
            else
            {
                //取消我是金主
                if (teamTypeDropDownControl == null)
                    return;

                if (_teamTypeDataList == null
                    || _teamTypeDataList.Count <= 0)
                    return;

                var curTeamTypeData = GetAllTeamModel();
                _teamDuplicationTeamModelType = curTeamTypeData.Id;

                teamTypeDropDownControl.UpdateCommonNewDropDownSelectedItem(curTeamTypeData);

                TeamDuplicationDataManager.GetInstance().OnSendTeamCopyTeamListReq(_curPageNumber,
                    _teamDuplicationTeamModelType);
            }
            UpdateTeamTypeDropDownBeforeCover();
        }

        private ComControlData GetGoldTeamModel()
        {
            if (_teamTypeDataList == null || _teamTypeDataList.Count <= 0)
                return null;

            var curTeamTypeData = _teamTypeDataList[0];
            for (var i = 1; i < _teamTypeDataList.Count; i++)
            {
                if (_teamTypeDataList[i].Id == (int)TeamCopyTeamModel.TEAM_COPY_TEAM_MODEL_GOLD)
                {
                    curTeamTypeData = _teamTypeDataList[i];
                    break;
                }
            }
            return curTeamTypeData;
        }

        private ComControlData GetAllTeamModel()
        {
            if (_teamTypeDataList == null || _teamTypeDataList.Count <= 0)
                return null;

            var curTeamTypeData = _teamTypeDataList[0];
            for (var i = 1; i < _teamTypeDataList.Count; i++)
            {
                if (_teamTypeDataList[i].Id == (int)TeamCopyTeamModel.TEAM_COPY_TEAM_MODEL_DEFAULT)
                {
                    curTeamTypeData = _teamTypeDataList[i];
                    break;
                }
            }
            return curTeamTypeData;
        }

        #endregion

        private void OnTeamInviteButtonClick()
        {
            TeamDuplicationUtility.OnOpenTeamDuplicationTeamInviteListFrame();
        }

        private void OnGoldSelectedButtonClick()
        {
            TeamDuplicationDataManager.GetInstance().IsTeamDuplicationGoldOwner =
                !TeamDuplicationDataManager.GetInstance().IsTeamDuplicationGoldOwner;
            UpdateGoldSelectedFlag();

            UpdateTeamTypeDropDownItemList();
        }

        private void UpdateGoldSelectedFlag()
        {
            CommonUtility.UpdateGameObjectVisible(goldSelectedFlag.gameObject,
                TeamDuplicationDataManager.GetInstance().IsTeamDuplicationGoldOwner);
        }

        private void OnCloseButtonClick()
        {
            TeamDuplicationUtility.OnCloseTeamDuplicationTeamListFrame();
        }

        //更新红点
        private void UpdateInviteButtonRedPoint()
        {
            CommonUtility.UpdateGameObjectVisible(inviteButtonRedPoint, 
                TeamDuplicationDataManager.GetInstance().IsTeamDuplicationOwnerNewInvite);
        }

        #region UIEvent

        //团本列表刷新
        private void OnReceiveTeamDuplicationTeamListRes(UIEvent uiEvent)
        {
            UpdateTeamListData();

            UpdatePageView();
            UpdateTeamListView();
        }

        //新的邀请团队的刷新
        private void OnReceiveTeamDuplicationOwnerNewInviteMessage(UIEvent uiEvent)
        {
            UpdateInviteButtonRedPoint();
        }

        //刷新队伍列表
        private void OnReceiveTeamDuplicationRefreshTeamListMessage(UIEvent uiEvent)
        {
            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyTeamListReq(_curPageNumber,
                _teamDuplicationTeamModelType);
        }

        //团长拒绝入团的提示
        private void OnReceiveTeamDuplicationTeamLeaderRefuseJoinInMessage(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var teamLeaderNameStr = (string)uiEvent.Param1;
            var showContentStr = string.Format(TR.Value("team_duplication_team_leader_refuse_join_in"),
                teamLeaderNameStr);

            SystemNotifyManager.SysNotifyFloatingEffect(showContentStr);
        }
        #endregion
    }
}
