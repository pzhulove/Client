using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class TeamDuplicationTeamPermissionView : MonoBehaviour
    {

        private TeamDuplicationPlayerDataModel _ownerPlayerDataModel;
        private TeamDuplicationPlayerDataModel _selectedPlayerDataModel;
        private Vector2 _clickScreenPosition = Vector2.zero;            //屏幕坐标
        private TeamDuplicationPermissionType _permissionType;

        [Space(15)]
        [HeaderAttribute("close")]
        [Space(10)]
        [SerializeField]
        private Button closeButton;

        [Space(15)] [HeaderAttribute("BgRoot")] [Space(15)]
        [SerializeField] private GameObject bgRoot;

        [Space(15)]
        [HeaderAttribute("playerPermission")]
        [Space(10)]
        [SerializeField] private Button playerCheckInformationButton;
        [SerializeField] private GameObject playerPermissionRoot;

        [Space(15)]
        [HeaderAttribute("captainPermission")]
        [Space(10)]
        [SerializeField] private Button captainMakeCaptainButton;
        [SerializeField] private Button captainCheckInformationButton;
        [SerializeField] private GameObject captainPermissionRoot;

        [Space(15)]
        [HeaderAttribute("teamLeaderPermission")]
        [Space(10)]
        [SerializeField] private Button teamLeaderMakeLeaderButton;
        [SerializeField] private UIGray teamLeaderMakeLeaderButtonGray;
        [SerializeField] private Button teamLeaderCheckInformationButton;

        [SerializeField] private Button teamLeaderMakeCaptainButton;
        [SerializeField] private UIGray teamLeaderMakeCaptainButtonGray;

        [SerializeField] private Button teamLeaderTickOutTeamButton;
        [SerializeField] private UIGray teamLeaderTickOutTeamButtonGray;
        [SerializeField] private GameObject teamLeaderPermissionRoot;


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
            _selectedPlayerDataModel = null;
            _clickScreenPosition = Vector2.zero;
            _permissionType = TeamDuplicationPermissionType.None;
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
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamDataMessage,
                OnReceiveTeamDuplicationTeamDataMessage);
        }

        private void UnBindUiMessage()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamDataMessage,
                OnReceiveTeamDuplicationTeamDataMessage);
        }

        private void BindUiEvents()
        {
            if (playerCheckInformationButton != null)
            {
                playerCheckInformationButton.onClick.RemoveAllListeners();
                playerCheckInformationButton.onClick.AddListener(OnPlayerCheckInformationButtonClick);
            }
            
            if (captainMakeCaptainButton != null)
            {
                captainMakeCaptainButton.onClick.RemoveAllListeners();
                captainMakeCaptainButton.onClick.AddListener(OnCaptainMakeCaptainButtonClick);
            }

            if (captainCheckInformationButton != null)
            {
                captainCheckInformationButton.onClick.RemoveAllListeners();
                captainCheckInformationButton.onClick.AddListener(OnCaptainCheckInformationButtonClick);
            }
            
            if (teamLeaderMakeLeaderButton != null)
            {
                teamLeaderMakeLeaderButton.onClick.RemoveAllListeners();
                teamLeaderMakeLeaderButton.onClick.AddListener(OnTeamLeaderMakeLeaderButtonClick);
            }

            if (teamLeaderCheckInformationButton != null)
            {
                teamLeaderCheckInformationButton.onClick.RemoveAllListeners();
                teamLeaderCheckInformationButton.onClick.AddListener(OnTeamLeaderCheckInformationButtonClick);
            }
            
            if (teamLeaderMakeCaptainButton != null)
            {
                teamLeaderMakeCaptainButton.onClick.RemoveAllListeners();
                teamLeaderMakeCaptainButton.onClick.AddListener(OnTeamLeaderMakeCaptainButtonClick);
            }

            if (teamLeaderTickOutTeamButton != null)
            {
                teamLeaderTickOutTeamButton.onClick.RemoveAllListeners();
                teamLeaderTickOutTeamButton.onClick.AddListener(OnTeamLeaderTickOutTeamButtonClick);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseButtonClick);
            }
        }

        private void UnBindUiEvents()
        {
            if (playerCheckInformationButton != null)
            {
                playerCheckInformationButton.onClick.RemoveAllListeners();
            }
            
            if (captainMakeCaptainButton != null)
            {
                captainMakeCaptainButton.onClick.RemoveAllListeners();
            }

            if (captainCheckInformationButton != null)
            {
                captainCheckInformationButton.onClick.RemoveAllListeners();
            }
            
            if (teamLeaderMakeLeaderButton != null)
            {
                teamLeaderMakeLeaderButton.onClick.RemoveAllListeners();
            }

            if (teamLeaderCheckInformationButton != null)
            {
                teamLeaderCheckInformationButton.onClick.RemoveAllListeners();
            }
            
            if (teamLeaderMakeCaptainButton != null)
            {
                teamLeaderMakeCaptainButton.onClick.RemoveAllListeners();
            }

            if (teamLeaderTickOutTeamButton != null)
            {
                teamLeaderTickOutTeamButton.onClick.RemoveAllListeners();
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
            }
        }

        public void Init(TeamDuplicationPermissionDataModel permissionDataModel)
        {
            if (permissionDataModel == null)
                return;

            _ownerPlayerDataModel = permissionDataModel.OwnerPlayerDataModel;
            _selectedPlayerDataModel = permissionDataModel.SelectedPlayerDataModel;
            _clickScreenPosition = permissionDataModel.ClickScreenPosition;

            if (_ownerPlayerDataModel == null
                || _selectedPlayerDataModel == null
                                              || _ownerPlayerDataModel.Guid == _selectedPlayerDataModel.Guid)
                return;

            InitData();

            InitView();
        }

        private void InitData()
        {
            //由owner 和 selected 决定权限
            _permissionType = TeamDuplicationPermissionType.PermissionPlayer;

            if (_ownerPlayerDataModel.IsTeamLeader == true)
            {
                //团长
                _permissionType = TeamDuplicationPermissionType.PermissionTeamLeader;
            }
            else
            {
                if (_ownerPlayerDataModel.IsCaptain == true)
                {
                    //队长
                    _permissionType = TeamDuplicationPermissionType.PermissionPlayer;
                    var isInSameTroop = TeamDuplicationUtility.IsInSameTroopOfTwoPlayer(_ownerPlayerDataModel.Guid,
                        _selectedPlayerDataModel.Guid);

                    //小队长的权限
                    if (isInSameTroop == true)
                        _permissionType = TeamDuplicationPermissionType.PermissionCaptain;
                }
                else
                {
                    //队员
                    _permissionType = TeamDuplicationPermissionType.PermissionPlayer;
                }
            }
        }

        private void InitView()
        {
            ResetPermissionRoot();
            InitPermissionRoot();
        }

        private void ResetPermissionRoot()
        {
            CommonUtility.UpdateGameObjectVisible(playerPermissionRoot, false);
            CommonUtility.UpdateGameObjectVisible(captainPermissionRoot, false);
            CommonUtility.UpdateGameObjectVisible(teamLeaderPermissionRoot, false);
        }

        private void InitPermissionRoot()
        {
            if (_permissionType == TeamDuplicationPermissionType.PermissionTeamLeader)
            {
                //团长
                CommonUtility.UpdateGameObjectVisible(teamLeaderPermissionRoot, true);

                if (_selectedPlayerDataModel != null)
                {
                    //团长选择了队长，则任命队长按钮置为灰色，已经是队长了，不用再次任命为队长
                    if (_selectedPlayerDataModel.IsCaptain == true)
                    {
                        CommonUtility.UpdateButtonState(teamLeaderMakeCaptainButton, teamLeaderMakeCaptainButtonGray,
                            false);
                    }
                    else
                    {
                        CommonUtility.UpdateButtonState(teamLeaderMakeCaptainButton, teamLeaderMakeCaptainButtonGray,
                            true);
                    }

                    //团长选择了金主，任命团长的按钮置灰，金主不可以被任命为团长
                    if (_selectedPlayerDataModel.IsGoldOwner == true)
                    {
                        CommonUtility.UpdateButtonState(teamLeaderMakeLeaderButton, teamLeaderMakeLeaderButtonGray,
                            false);
                    }
                    else
                    {
                        CommonUtility.UpdateButtonState(teamLeaderMakeLeaderButton, teamLeaderMakeLeaderButtonGray,
                            true);
                    }
                }

                if (TeamDuplicationUtility.GetTeamDuplicationTeamStatus() == TeamCopyTeamStatus.TEAM_COPY_TEAM_STATUS_PARPARE)
                {
                    //未开战，团长可以踢人
                    CommonUtility.UpdateButtonState(teamLeaderTickOutTeamButton, teamLeaderTickOutTeamButtonGray,
                        true);
                }
                else
                {
                    //开战之后，团长不可以踢人
                    CommonUtility.UpdateButtonState(teamLeaderTickOutTeamButton, teamLeaderTickOutTeamButtonGray,
                        false);
                }


                //位置调整20，避免出屏幕
                SetPermissionRootPosition(teamLeaderPermissionRoot, 20);
            }
            else if (_permissionType == TeamDuplicationPermissionType.PermissionCaptain)
            {
                //队长
                CommonUtility.UpdateGameObjectVisible(captainPermissionRoot, true);
                SetPermissionRootPosition(captainPermissionRoot);
            }
            else
            {
                //队员
                CommonUtility.UpdateGameObjectVisible(playerPermissionRoot, true);
                SetPermissionRootPosition(playerPermissionRoot);
            }
        }

        private void SetPermissionRootPosition(GameObject permissionRoot, float adjustYPosition = 0.0f)
        {
            if (bgRoot == null)
                return;

            var bgRootRtf = bgRoot.transform as RectTransform;
            if (bgRootRtf == null)
                return;

            if (permissionRoot == null)
                return;

            var permissionRootRtf = permissionRoot.transform as RectTransform;
            if (permissionRootRtf == null)
                return;

            //将屏幕坐标转换为某个坐标系下的局部坐标
            Vector2 localPos;
            var success = RectTransformUtility.ScreenPointToLocalPointInRectangle(bgRootRtf, 
                _clickScreenPosition, 
                ClientSystemManager.GetInstance().UICamera, 
                out localPos);

            if (!success)
            {
                return;
            }

            var permissionRootSize = permissionRootRtf.sizeDelta;

            //设置锚点的位置
            permissionRootRtf.anchoredPosition = new Vector2(
                localPos.x + permissionRootSize.x / 2.0f,
                localPos.y - permissionRootSize.y / 2.0f + adjustYPosition);
        }


        private void OnPlayerCheckInformationButtonClick()
        {
            TeamDuplicationCheckPlayerInformation();
        }

        //任命队长
        private void OnCaptainMakeCaptainButtonClick()
        {           
            TeamCopyMakeAppoint(TeamCopyPost.TEAM_COPY_POST_CAPTAIN);
        }

        private void OnCaptainCheckInformationButtonClick()
        {
            TeamDuplicationCheckPlayerInformation();
        }
        
        //任命团长
        private void OnTeamLeaderMakeLeaderButtonClick()
        {
            TeamCopyMakeAppoint(TeamCopyPost.TEAM_COPY_POST_CHIEF);
        }

        private void OnTeamLeaderCheckInformationButtonClick()
        {
            TeamDuplicationCheckPlayerInformation();
        }
        
        //任命队长
        private void OnTeamLeaderMakeCaptainButtonClick()
        {
            TeamCopyMakeAppoint(TeamCopyPost.TEAM_COPY_POST_CAPTAIN);
        }

        private void OnTeamLeaderTickOutTeamButtonClick()
        {
            //开战之后，不能踢人
            if (TeamDuplicationUtility.GetTeamDuplicationTeamStatus() !=
                TeamCopyTeamStatus.TEAM_COPY_TEAM_STATUS_PARPARE)
                return;

            TeamDuplicationUtility.OnShowLeaveTeamTipFrame(TR.Value("team_duplication_kick_team_normal_tips"),
                TeamCopyKickPlayer);
        }

        private void TeamCopyMakeAppoint(TeamCopyPost teamCopyPost)
        {
            if (_selectedPlayerDataModel == null)
                return;

            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyAppointmentReq(_selectedPlayerDataModel.Guid,
                (int)teamCopyPost);

            CloseFrame();
        }

        private void TeamCopyKickPlayer()
        {
            if (_selectedPlayerDataModel == null)
                return;

            //玩家的角色不存在
            var playerDataModel = TeamDuplicationUtility.GetPlayerDataModelByGuid(_selectedPlayerDataModel.Guid);
            if (playerDataModel == null)
                return;

            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyKickReq(_selectedPlayerDataModel.Guid);
            CloseFrame();
        }

        //显示其他玩家的信息
        private void TeamDuplicationCheckPlayerInformation()
        {
            if (_selectedPlayerDataModel == null)
                return;

            //跨服查找队友数据
            OtherPlayerInfoManager.GetInstance().SendWatchOtherPlayerInfo(_selectedPlayerDataModel.Guid,
                (uint) QueryPlayerType.QPT_TEAM_COPY,
                (uint)_selectedPlayerDataModel.ZoneId);

            CloseFrame();
        }

        //团本队伍的数据更新
        private void OnReceiveTeamDuplicationTeamDataMessage(UIEvent uiEvent)
        {
            //收到玩家退出队伍，关闭界面
            if (_selectedPlayerDataModel == null
                || TeamDuplicationUtility.GetPlayerDataModelByGuid(_selectedPlayerDataModel.Guid) == null)
            {
                CloseFrame();
            }
        }


        private void OnCloseButtonClick()
        {
            CloseFrame();
        }

        private void CloseFrame()
        {
            TeamDuplicationUtility.OnCloseTeamDuplicationTeamPermissionFrame();
        }
    }
}
