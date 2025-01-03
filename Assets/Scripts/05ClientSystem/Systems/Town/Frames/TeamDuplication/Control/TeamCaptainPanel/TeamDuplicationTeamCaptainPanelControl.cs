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

    //队伍展示控制器
    public class TeamDuplicationTeamCaptainPanelControl : MonoBehaviour
    {
        private TeamDuplicationTeamCaptainPanelBaseView teamPanelView;
        private TeamDuplicationTeamCaptainPanelBaseView captainPanelView;

        private bool _isOwnerTeam;         //拥有队伍的标志

        [Space(15)]
        [HeaderAttribute("troopTabs")]
        [Space(5)]
        [SerializeField] private Toggle teamToggle;
        [SerializeField] private Toggle captainToggle;

        [SerializeField] private Button noTeamButton;

        [Space(15)]
        [HeaderAttribute("Common")]
        [Space(5)]
        [SerializeField]
        private Button teamBuildButton;
        [SerializeField] private Button teamJoinInButton;
        [SerializeField] private Text noTeamLabel;
        [SerializeField] private GameObject noTeamRoot;

        [Space(15)]
        [HeaderAttribute("Control")]
        [Space(5)]

        [SerializeField] private GameObject teamPanelViewRoot;
        [SerializeField] private GameObject captainPanelViewRoot;

        [Space(25)]
        [HeaderAttribute("FadeOutAnimation")]
        [Space(10)]
        [SerializeField] private Button panelFadeOutButton;
        [SerializeField] private DOTweenAnimation panelFadeOutAnimation;

        [Space(25)] [HeaderAttribute("FadeInAnimation")] [Space(10)]
        [SerializeField] private Button panelFadeInButton;
        [SerializeField] private DOTweenAnimation panelFadeInAnimation;

        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }

        private void BindEvents()
        {
            if (noTeamButton != null)
            {
                noTeamButton.onClick.RemoveAllListeners();
                noTeamButton.onClick.AddListener(OnNoTeamButtonClick);
            }

            if (teamBuildButton != null)
            {
                teamBuildButton.onClick.RemoveAllListeners();
                teamBuildButton.onClick.AddListener(OnTeamBuildButtonClick);
            }

            if (teamJoinInButton != null)
            {
                teamJoinInButton.onClick.RemoveAllListeners();
                teamJoinInButton.onClick.AddListener(OnTeamJoinInButtonClick);
            }

            if (teamToggle != null)
            {
                teamToggle.onValueChanged.RemoveAllListeners();
                teamToggle.onValueChanged.AddListener(OnTeamToggleClick);
            }

            if (captainToggle != null)
            {
                captainToggle.onValueChanged.RemoveAllListeners();
                captainToggle.onValueChanged.AddListener(OnCaptainToggleClick);
            }

            if (panelFadeInButton != null)
            {
                panelFadeInButton.onClick.RemoveAllListeners();
                panelFadeInButton.onClick.AddListener(OnPanelFadeInButtonClick);
            }

            if (panelFadeOutButton != null)
            {
                panelFadeOutButton.onClick.RemoveAllListeners();
                panelFadeOutButton.onClick.AddListener(OnPanelFadeOutButtonClick);
            }

            if (panelFadeInAnimation != null)
            {
                panelFadeInAnimation.onComplete.RemoveAllListeners();
                panelFadeInAnimation.onComplete.AddListener(PanelFadeInAnimationComplete);
            }

            if (panelFadeOutAnimation != null)
            {
                panelFadeOutAnimation.onComplete.RemoveAllListeners();
                panelFadeOutAnimation.onComplete.AddListener(PanelFadeOutAnimationComplete);
            }

        }

        private void UnBindEvents()
        {
            if (noTeamButton != null)
                noTeamButton.onClick.RemoveAllListeners();

            if (teamBuildButton != null)
            {
                teamBuildButton.onClick.RemoveAllListeners();
            }

            if (teamJoinInButton != null)
            {
                teamJoinInButton.onClick.RemoveAllListeners();
            }

            if (teamToggle != null)
            {
                teamToggle.onValueChanged.RemoveAllListeners();
            }

            if (captainToggle != null)
            {
                captainToggle.onValueChanged.RemoveAllListeners();
            }

            if (panelFadeInButton != null)
            {
                panelFadeInButton.onClick.RemoveAllListeners();
            }

            if (panelFadeOutButton != null)
            {
                panelFadeOutButton.onClick.RemoveAllListeners();
            }

            if (panelFadeInAnimation != null)
            {
                panelFadeInAnimation.onComplete.RemoveAllListeners();
            }

            if (panelFadeOutAnimation != null)
            {
                panelFadeOutAnimation.onComplete.RemoveAllListeners();
            }

        }

        private void OnEnable()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamDataMessage,
                OnReceiveTeamDuplicationTeamDataMessage);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationDismissMessage,
                OnReceiveTeamDuplicationTeamQuitMessage);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationQuitTeamMessage,
                OnReceiveTeamDuplicationTeamQuitMessage);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamDataMessage,
                OnReceiveTeamDuplicationTeamDataMessage);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationDismissMessage,
                OnReceiveTeamDuplicationTeamQuitMessage);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationQuitTeamMessage,
                OnReceiveTeamDuplicationTeamQuitMessage);
        }

        private void ClearData()
        {
            _isOwnerTeam = false;
        }

        public void Init()
        {
            _isOwnerTeam = TeamDuplicationDataManager.GetInstance().IsTeamDuplicationOwnerTeam;
            InitCommonInfo();
            InitDoTweenAnimation();
            SetTeamToggle();
        }

        private void SetTeamToggle()
        {
            if (teamToggle != null)
            {
                teamToggle.isOn = false;
                teamToggle.isOn = true;
            }
        }

        private void InitCommonInfo()
        {

            if (noTeamLabel != null)
                noTeamLabel.text = TR.Value("team_duplication_build_or_join_troop");

            UpdateNoTeamButton();
        }

        private void InitDoTweenAnimation()
        {
            if (panelFadeInAnimation != null)
                panelFadeInAnimation.id = DoTweenAnimationFadeType.FadeIn.ToString();

            if (panelFadeOutAnimation != null)
                panelFadeOutAnimation.id = DoTweenAnimationFadeType.FadeOut.ToString();

        }

        private void UpdateNoTeamButton()
        {
            if (noTeamButton != null)
            {
                if (TeamDuplicationDataManager.GetInstance().IsTeamDuplicationOwnerTeam == false)
                {
                    noTeamButton.gameObject.CustomActive(true);
                }
                else
                {
                    noTeamButton.gameObject.CustomActive(false);
                }
            }
        }

        private void OnTeamToggleClick(bool value)
        {
            if (value == false)
                return;

            //不存在队伍
            if (TeamDuplicationDataManager.GetInstance().IsTeamDuplicationOwnerTeam == false)
            {
                CommonUtility.UpdateGameObjectVisible(noTeamRoot, true);
                CommonUtility.UpdateGameObjectVisible(teamPanelViewRoot, false);
                CommonUtility.UpdateGameObjectVisible(captainPanelViewRoot, false);
                return;
            }

            CommonUtility.UpdateGameObjectVisible(noTeamRoot, false);
            CommonUtility.UpdateGameObjectVisible(captainPanelViewRoot, false);

            //再次点击的时候，
            if (teamPanelViewRoot != null && teamPanelViewRoot.activeSelf == true
                                          && teamPanelView != null)
            {
                TeamDuplicationUtility.OnOpenTeamDuplicationTeamRoomFrame();
            }
            else
            {
                UpdateTeamPanelView();
            }
        }

        private void OnCaptainToggleClick(bool value)
        {
            if (value == false)
                return;

            //不存在队伍
            if (TeamDuplicationDataManager.GetInstance().IsTeamDuplicationOwnerTeam == false)
            {
                CommonUtility.UpdateGameObjectVisible(noTeamRoot, true);
                CommonUtility.UpdateGameObjectVisible(teamPanelViewRoot, false);
                CommonUtility.UpdateGameObjectVisible(captainPanelViewRoot, false);
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_troop_not_exist"));
                return;
            }

            CommonUtility.UpdateGameObjectVisible(noTeamRoot, false);
            CommonUtility.UpdateGameObjectVisible(teamPanelViewRoot, false);
            UpdateCaptainPanelView();
        }
        
        //团本的View
        private void UpdateTeamPanelView()
        {
            if (teamPanelViewRoot != null && teamPanelViewRoot.activeSelf == false)
                teamPanelViewRoot.CustomActive(true);

            if (teamPanelView == null)
            {
                teamPanelView = LoadTeamCaptainPanelBaseView(teamPanelViewRoot);
                if (teamPanelView != null)
                    teamPanelView.Init();
            }
            else
            {
                teamPanelView.OnEnableView();
            }
        }

        //更新小队的View
        private void UpdateCaptainPanelView()
        {
            if (captainPanelViewRoot != null && captainPanelViewRoot.activeSelf == false)
                captainPanelViewRoot.CustomActive(true);

            if (captainPanelView == null)
            {
                captainPanelView = LoadTeamCaptainPanelBaseView(captainPanelViewRoot);
                if (captainPanelView != null)
                    captainPanelView.Init();
            }
            else
            {
                captainPanelView.OnEnableView();
            }
        }

        private TeamDuplicationTeamCaptainPanelBaseView LoadTeamCaptainPanelBaseView(GameObject contentRoot)
        {
            if (contentRoot == null)
                return null;

            TeamDuplicationTeamCaptainPanelBaseView panelBaseView = null;

            var uiPrefabWrapper = contentRoot.GetComponent<UIPrefabWrapper>();
            if (uiPrefabWrapper != null)
            {
                var uiPrefab = uiPrefabWrapper.LoadUIPrefab();
                if (uiPrefab != null)
                {
                    uiPrefab.transform.SetParent(contentRoot.transform, false);
                    panelBaseView = uiPrefab.GetComponent<TeamDuplicationTeamCaptainPanelBaseView>();
                }
            }

            return panelBaseView;
        }

        private void OnReceiveTeamDuplicationTeamQuitMessage(UIEvent uiEvent)
        {
            if (_isOwnerTeam == false)
                return;

            //设置标志和展示按钮
            _isOwnerTeam = TeamDuplicationDataManager.GetInstance().IsTeamDuplicationOwnerTeam;
            UpdateNoTeamButton();

            //跳转到teamToggle
            SetTeamToggle();
        }

        //处理按钮和页面的控制器
        private void OnReceiveTeamDuplicationTeamDataMessage(UIEvent uiEvent)
        {
            if (_isOwnerTeam == true)
                return;

            _isOwnerTeam = true;

            UpdateNoTeamButton();

            CommonUtility.UpdateGameObjectVisible(noTeamRoot, false);

            //展示团本所有队伍的数据
            CommonUtility.UpdateGameObjectVisible(captainPanelViewRoot, false);
            UpdateTeamPanelView();
        }

        private void OnNoTeamButtonClick()
        {
            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_troop_not_exist"));
        }

        private void OnTeamJoinInButtonClick()
        {
            TeamDuplicationUtility.OnOpenTeamDuplicationTeamListFrame();
        }

        private void OnTeamBuildButtonClick()
        {
            TeamDuplicationUtility.OnOpenTeamDuplicationTeamBuildFrame();
        }



        //退出
        private void OnPanelFadeOutButtonClick()
        {
            if (panelFadeInAnimation == null
                || panelFadeOutAnimation == null)
                return;

            panelFadeOutAnimation.CreateTween();
            panelFadeOutAnimation.DOPlay();

            //更新按钮
            CommonUtility.UpdateButtonVisible(panelFadeInButton, true);
            CommonUtility.UpdateButtonState(panelFadeOutButton, null, false);
        }


        private void OnPanelFadeInButtonClick()
        {
            if (panelFadeInAnimation == null || panelFadeOutAnimation == null)
                return;

            panelFadeInAnimation.CreateTween();
            panelFadeInAnimation.DOPlay();
            
            //更新按钮
            CommonUtility.UpdateButtonVisible(panelFadeInButton, false);
            CommonUtility.UpdateButtonState(panelFadeOutButton, null, true);
        }

        private void PanelFadeInAnimationComplete()
        {
        }

        private void PanelFadeOutAnimationComplete()
        {
        }

    }
}
