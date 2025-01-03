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

    //头像控制器
    public class TeamDuplicationBuildCommonControl : MonoBehaviour
    {

        [Space(15)] [HeaderAttribute("CommonButton")] [Space(5)]
        [SerializeField] private Button backButton;
        [SerializeField] private Button skillButton;
        [SerializeField] private Button packageButton;

        [SerializeField] private CommonHelpNewAssistant helpNewAssistant;

        [Space(15)]
        [HeaderAttribute("TeamDuplicationButton")]
        [Space(5)]
        [SerializeField] private Button teamDuplicationShopButton;
        [SerializeField] private Button teamDuplicationTroopListButton;
        [SerializeField] private Button teamDuplicationTroopForwardButton;

        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
        }

        private void BindEvents()
        {
            if (backButton != null)
            {
                backButton.onClick.RemoveAllListeners();
                backButton.onClick.AddListener(OnBackButtonClick);
            }

            if (packageButton != null)
            {
                packageButton.onClick.RemoveAllListeners();
                packageButton.onClick.AddListener(OnPackageButtonClick);
            }

            if (skillButton != null)
            {
                skillButton.onClick.RemoveAllListeners();
                skillButton.onClick.AddListener(OnSkillButtonClick);
            }

            if (teamDuplicationShopButton != null)
            {
                teamDuplicationShopButton.onClick.RemoveAllListeners();
                teamDuplicationShopButton.onClick.AddListener(OnTeamDuplicationShopButtonClick);
            }

            if (teamDuplicationTroopListButton  != null)
            {
                teamDuplicationTroopListButton.onClick.RemoveAllListeners();
                teamDuplicationTroopListButton.onClick.AddListener(OnTeamDuplicationTroopListButtonClick);
            }

            if (teamDuplicationTroopForwardButton != null)
            {
                teamDuplicationTroopForwardButton.onClick.RemoveAllListeners();
                teamDuplicationTroopForwardButton.onClick.AddListener(OnTeamDuplicationForwardButtonClick);
            }


        }

        private void UnBindEvents()
        {
            if (backButton != null)
            {
                backButton.onClick.RemoveAllListeners();
            }

            if (packageButton != null)
            {
                packageButton.onClick.RemoveAllListeners();
            }

            if (skillButton != null)
            {
                skillButton.onClick.RemoveAllListeners();
            }

            if (teamDuplicationShopButton != null)
            {
                teamDuplicationShopButton.onClick.RemoveAllListeners();
            }

            if (teamDuplicationTroopListButton != null)
            {
                teamDuplicationTroopListButton.onClick.RemoveAllListeners();
            }

            if (teamDuplicationTroopForwardButton != null)
            {
                teamDuplicationTroopForwardButton.onClick.RemoveAllListeners();
            }
        }

        private void OnEnable()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationQuitTeamMessage,
                OnReceiveTeamDuplicationLeaveTeamMessage);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationDismissMessage,
                OnReceiveTeamDuplicationLeaveTeamMessage);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamDataMessage,
                OnReceiveTeamDuplicationTeamDataMessage);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationQuitTeamMessage,
                OnReceiveTeamDuplicationLeaveTeamMessage);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationDismissMessage,
                OnReceiveTeamDuplicationLeaveTeamMessage);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamDataMessage,
                OnReceiveTeamDuplicationTeamDataMessage);
        }

        public void Init()
        {
            UpdateTeamDuplicationButton();
        }

        //根据不同的状态显示不同的按钮
        private void UpdateTeamDuplicationButton()
        {
            //不存在队伍
            if (TeamDuplicationDataManager.GetInstance().IsTeamDuplicationOwnerTeam == false)
            {
                CommonUtility.UpdateButtonVisible(teamDuplicationTroopListButton, true);
                CommonUtility.UpdateButtonVisible(teamDuplicationTroopForwardButton, false);
            }
            else
            {
                CommonUtility.UpdateButtonVisible(teamDuplicationTroopListButton, false);
                CommonUtility.UpdateButtonVisible(teamDuplicationTroopForwardButton, true);
            }
        }

        //收到团本信息
        private void OnReceiveTeamDuplicationTeamDataMessage(UIEvent uiEvent)
        {
            UpdateTeamDuplicationButton();
            if (TeamDuplicationDataManager.GetInstance().IsTeamDuplicationOwnerTeam == true)
            {
                TeamDuplicationUtility.OnCloseRelationFrameByOwnerTeam();
            }
        }

        private void OnReceiveTeamDuplicationLeaveTeamMessage(UIEvent uiEvent)
        {
            UpdateTeamDuplicationButton();
            TeamDuplicationUtility.OnCloseRelationFrameByLeaveTeam();
        }

        
        private void OnBackButtonClick()
        {
            if (TeamDuplicationDataManager.GetInstance().IsTeamDuplicationOwnerTeam == false)
            {
                OnDoBackReturn();
            }
            else
            {
                TeamDuplicationUtility.OnQuitTeamAndReturnToTown(TR.Value("team_duplication_quit_team_and_return_town"),
                    OnDoBackReturn);
            }
        }

        private void OnDoBackReturn()
        {
            TeamDuplicationUtility.SwitchToTeamDuplicationBirthCityScene();

            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyTeamQuitReq();

            TeamDuplicationDataManager.GetInstance().ClearData();

            ChatManager.GetInstance().ClearChannelChatData(ChanelType.CHAT_CHANNEL_TEAM_COPY_PREPARE);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RefreshChatData);
        }

        private void OnPackageButtonClick()
        {
            TeamDuplicationUtility.OnOpenTeamDuplicationPackageNewFrame();
        }

        private void OnSkillButtonClick()
        {
            TeamDuplicationUtility.OnOpenTeamDuplicationSkillFrame();
        }

        private void OnTeamDuplicationShopButtonClick()
        {
            //ShopNewUtility.OnCloseShopNewFrame();
            ShopNewDataManager.GetInstance().OpenShopNewFrame(TeamDuplicationDataManager.TeamDuplicationShopId);
        }

        private void OnTeamDuplicationTroopListButtonClick()
        {
            TeamDuplicationUtility.OnOpenTeamDuplicationTeamListFrame();
        }

        private void OnTeamDuplicationForwardButtonClick()
        {
            if (TeamDuplicationDataManager.GetInstance().IsTeamDuplicationOwnerTeam == false)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_troop_not_exist"));
                return;
            }
            TeamDuplicationUtility.SwitchToTeamDuplicationFightScene();
        }
        

    }
}
