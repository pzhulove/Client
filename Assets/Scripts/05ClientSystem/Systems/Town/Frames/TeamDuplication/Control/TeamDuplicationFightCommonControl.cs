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

    //通用控制器
    public class TeamDuplicationFightCommonControl : MonoBehaviour
    {

        private bool _isLoadingFinish = false;
        [Space(15)] [HeaderAttribute("CommonButton")] [Space(5)] [SerializeField]
        private GameObject backButtonRoot;
        [SerializeField] private Button backButton;
        [SerializeField] private CommonHelpNewAssistant helpNewAssistant;

        [Space(15)]
        [HeaderAttribute("BottomButton")]
        [Space(5)]
        [SerializeField] private Button fightPreSettingButtonWithStart;
        [SerializeField] private ComButtonWithCd fightStartButtonWithCd;
        [SerializeField] private ComButtonWithCd fightEndVoteButtonWithCd;

        [SerializeField] private Button skillButton;
        [SerializeField] private Button packageButton;

        [Space(15)] [HeaderAttribute("fightCountDownTime")] [Space(5)]
        [SerializeField] private CountDownTimeController fightCountDownTimeControl;

        [Space(15)]
        [HeaderAttribute("FightPanelControl")]
        [Space(5)]
        [SerializeField] private TeamDuplicationFightPanelControl fightPanelControl;

        [Space(15)] [HeaderAttribute("GameOver")] [Space(5)]
        [SerializeField] private GameObject gameOverRoot;
        [SerializeField] private Button leaveButton;

        [SerializeField] private GameObject fightingRoot;

        private void Awake()
        {
            _isLoadingFinish = false;
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }

        private void ClearData()
        {
            _isLoadingFinish = false;
        }

        private void BindEvents()
        {
            if (backButton != null)
            {
                backButton.onClick.RemoveAllListeners();
                backButton.onClick.AddListener(OnBackBuildButtonClick);
            }

            if (fightPreSettingButtonWithStart != null)
            {
                fightPreSettingButtonWithStart.onClick.RemoveAllListeners();
                fightPreSettingButtonWithStart.onClick.AddListener(OnFightPreSettingWithStartButtonClick);
            }

            if (fightStartButtonWithCd != null)
            {
                fightStartButtonWithCd.ResetButtonListener();
                fightStartButtonWithCd.SetButtonListener(OnFightStartButtonClick);
            }

            if (fightEndVoteButtonWithCd != null)
            {
                fightEndVoteButtonWithCd.ResetButtonListener();
                fightEndVoteButtonWithCd.SetButtonListener(OnFightEndVoteButtonClick);
            }

            if (leaveButton != null)
            {
                leaveButton.onClick.RemoveAllListeners();
                leaveButton.onClick.AddListener(OnLeaveButtonClick);
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

        }
        
        private void UnBindEvents()
        {
            if (backButton != null)
            {
                backButton.onClick.RemoveAllListeners();
            }

            if (fightPreSettingButtonWithStart != null)
            {
                fightPreSettingButtonWithStart.onClick.RemoveAllListeners();
            }

            if (fightStartButtonWithCd != null)
            {
                fightStartButtonWithCd.ResetButtonListener();
            }

            if (fightEndVoteButtonWithCd != null)
            {
                fightEndVoteButtonWithCd.ResetButtonListener();
            }


            if (leaveButton != null)
            {
                leaveButton.onClick.RemoveAllListeners();
            }

            if (packageButton != null)
            {
                packageButton.onClick.RemoveAllListeners();
            }

            if (skillButton != null)
            {
                skillButton.onClick.RemoveAllListeners();
            }

        }

        private void OnEnable()
        {
            //同意开展完成
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationInBattleMessage,
                OnReceiveTeamDuplicationInBattleMessage);

            //关卡挑战完成
            UIEventSystem.GetInstance().RegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationFightStageEndNotifyMessage,
                OnReceiveTeamDuplicationFightStageEndNotifyMessage);

            //阶段开始通知，自动打开攻坚面板
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTeamDuplicationFightStageBeginMessage,
                OnTeamDuplicationFightStageBeginMessage);

            //团本数据修改
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamDataMessage,
                OnReceiveTeamDuplicationTeamDataMessage);

            //团本阶段数据更新
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationFightStageNotifyMessage,
                OnReceiveTeamDuplicationFightStageNotifyMessage);

            //团本状态更新
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamStatusNotifyMessage,
                OnReceiveTeamDuplicationGameResultMessage);

            //场景加载完成
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SceneChangedLoadingFinish, OnReceiveSceneChangeLoadingFinish);

            //界面展示完成的UI事件
            //阶段完成的描述界面关闭
            UIEventSystem.GetInstance().RegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationStageEndDescriptionCloseMessage,
                OnReceiveTeamDuplicationStageEndDescriptionCloseMessage);
            //阶段奖励界面关闭
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationMiddleStageRewardCloseMessage,
                OnReceiveTeamDuplicationMiddleStageRewardCloseMessage);
            //最终结果描述关闭
            UIEventSystem.GetInstance()
                .RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationFinalResultCloseMessage,
                    OnReceiveTeamDuplicationFinalResultCloseMessage);

            //攻坚场景退出团本
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationQuitTeamMessage,
                OnSwitchToBuildSceneByQuitTeam);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationDismissMessage,
                OnSwitchToBuildSceneByQuitTeam);

            //战斗结束投票标志的通知
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationFightEndVoteFlagMessage,
                OnReceiveTeamDuplicationFightEndVoteFlagMessage);
            //战斗结束投票结果成功的通知
            UIEventSystem.GetInstance().RegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationFightEndVoteResultSucceedMessage,
                OnReceiveTeamDuplicationFightEndVoteResultSucceedMessage);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationInBattleMessage,
                OnReceiveTeamDuplicationInBattleMessage);

            UIEventSystem.GetInstance().UnRegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationFightStageEndNotifyMessage,
                OnReceiveTeamDuplicationFightStageEndNotifyMessage);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTeamDuplicationFightStageBeginMessage,
                OnTeamDuplicationFightStageBeginMessage);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamDataMessage,
                OnReceiveTeamDuplicationTeamDataMessage);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationFightStageNotifyMessage,
                OnReceiveTeamDuplicationFightStageNotifyMessage);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamStatusNotifyMessage,
                OnReceiveTeamDuplicationGameResultMessage);

            UIEventSystem.GetInstance()
                .UnRegisterEventHandler(EUIEventID.SceneChangedLoadingFinish, OnReceiveSceneChangeLoadingFinish);

            UIEventSystem.GetInstance().UnRegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationStageEndDescriptionCloseMessage,
                OnReceiveTeamDuplicationStageEndDescriptionCloseMessage);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationMiddleStageRewardCloseMessage,
                OnReceiveTeamDuplicationMiddleStageRewardCloseMessage);
            UIEventSystem.GetInstance()
                .UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationFinalResultCloseMessage,
                    OnReceiveTeamDuplicationFinalResultCloseMessage);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationQuitTeamMessage,
                OnSwitchToBuildSceneByQuitTeam);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationDismissMessage,
                OnSwitchToBuildSceneByQuitTeam);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationFightEndVoteFlagMessage,
                OnReceiveTeamDuplicationFightEndVoteFlagMessage);
            UIEventSystem.GetInstance().UnRegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationFightEndVoteResultSucceedMessage,
                OnReceiveTeamDuplicationFightEndVoteResultSucceedMessage);
        }

        public void Init()
        {
            InitFightButton();
            InitFightPanelControl();
        }

        //根据不同的状态显示不同的按钮
        private void InitFightButton()
        {
            ResetButton();

            var teamDataModel = TeamDuplicationDataManager.GetInstance().GetTeamDuplicationTeamDataModel();
            if (teamDataModel == null)
                return;

            if (teamDataModel.TeamStatus == TeamCopyTeamStatus.TEAM_COPY_TEAM_STATUS_PARPARE)
            {
                //备战
                //团长，没有开战，显示（战前配置和开战按钮)
                if (TeamDuplicationUtility.IsSelfPlayerIsTeamLeaderInTeamDuplication() == true)
                {
                    //噩梦难度
                    if (TeamDuplicationUtility.IsTeamDuplicationTeamDifficultyHardLevel() == true)
                    {
                        CommonUtility.UpdateButtonVisible(fightPreSettingButtonWithStart, false);
                    }
                    else
                    {
                        CommonUtility.UpdateButtonVisible(fightPreSettingButtonWithStart, true);
                    }
                    CommonUtility.UpdateGameObjectVisible(fightStartButtonWithCd.gameObject, true);
                }
            }
            else if (teamDataModel.TeamStatus == TeamCopyTeamStatus.TEAM_COPY_TEAM_STATUS_BATTLE)
            {
                //战斗
                //已经开战，显示攻坚面板
                UpdateFightRelationView(true);
                CommonUtility.UpdateGameObjectVisible(backButtonRoot, false);

                UpdateFightEndVoteButton();
            }
            else
            {
                //战斗胜利或者失败
                UpdateGameOverView();
            }
            
        }

        private void InitFightPanelControl()
        {
            if(fightPanelControl != null)
                fightPanelControl.InitFightPanelControl();
        }

        #region ButtonClick
        private void OnBackBuildButtonClick()
        {
            TeamDuplicationUtility.BackToTeamDuplicationBuildScene();
        }

        private void OnFightPreSettingWithStartButtonClick()
        {
            TeamDuplicationUtility.OnOpenTeamDuplicationFightPreSettingFrame();
        }

        private void OnFightStartButtonClick()
        {
            //如果存在小队没有成员，无法开战
            if (TeamDuplicationUtility.IsEveryTroopOwnerPlayer() == false)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(
                    TR.Value("team_duplication_start_battle_by_player_not_enough"));
                return;
            }

            //噩梦难度，直接开战，设置为自由模式
            if (TeamDuplicationUtility.IsTeamDuplicationTeamDifficultyHardLevel() == true)
            {
                OnFightStartWithHardLevel();
                return;
            }

            //没有进行战前配置
            if (TeamDuplicationDataManager.GetInstance().FightSettingConfigPlanModel
                == TeamCopyPlanModel.TEAM_COPY_PLAN_MODEL_INVALID)
            {
                var contentStr = TR.Value("team_duplication_start_fighting_without_pre_setting");
                TeamDuplicationUtility.OnShowCommonMsgBoxFrame(contentStr,
                    OnFightStartAction);
                return;
            }

            OnFightStartAction();
        }
        
        //开启战斗
        private void OnFightStartAction()
        {
            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyStartBattleReq(
                TeamDuplicationDataManager.GetInstance().FightSettingConfigPlanModel);
        }

        private void OnFightStartWithHardLevel()
        {
            //噩梦难度，默认为自由模式
            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyStartBattleReq(
                TeamCopyPlanModel.TEAM_COPY_PLAN_MODEL_FREE);
        }

        private void OnLeaveButtonClick()
        {
            //退出团本
            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyTeamQuitReq();
            //清空数据
            TeamDuplicationDataManager.GetInstance().ClearData();

            //切换到组队场景
            TeamDuplicationUtility.BackToTeamDuplicationBuildScene();
        }

        private void OnPackageButtonClick()
        {
            TeamDuplicationUtility.OnOpenTeamDuplicationPackageNewFrame();
        }

        private void OnSkillButtonClick()
        {
            TeamDuplicationUtility.OnOpenTeamDuplicationSkillFrame();
        }


        #endregion

        #region UpdateInfo
        private void UpdateFightRelationView(bool flag)
        {
            //攻坚面板按钮展示，back按钮就要隐藏
            if (fightPanelControl != null)
                fightPanelControl.UpdateFightPanelShowButton(flag);

            UpdateFightCountDownTimeControl(flag);
        }

        private void UpdateFightCountDownTimeControl(bool flag)
        {
            if (fightCountDownTimeControl == null)
                return;

            CommonUtility.UpdateGameObjectVisible(fightCountDownTimeControl.gameObject, flag);

            if (flag == true)
            {
                fightCountDownTimeControl.EndTime =
                    TeamDuplicationDataManager.GetInstance().TeamDuplicationFightFinishTime;
                fightCountDownTimeControl.InitCountDownTimeController();
            }
        }

        private void ResetButton()
        {
            CommonUtility.UpdateButtonVisible(fightPreSettingButtonWithStart, false);
            CommonUtility.UpdateButtonWithCdVisible(fightStartButtonWithCd, false);
            CommonUtility.UpdateButtonWithCdVisible(fightEndVoteButtonWithCd, false);
            

            UpdateFightRelationView(false);
        }

        private void ShowGameOverView()
        {

            TeamDuplicationTeamDataModel teamDataModel =
                TeamDuplicationDataManager.GetInstance().GetTeamDuplicationTeamDataModel();

            if (teamDataModel == null)
                return;

            if (teamDataModel.TeamStatus != TeamCopyTeamStatus.TEAM_COPY_TEAM_STATUS_FAILED
                && teamDataModel.TeamStatus != TeamCopyTeamStatus.TEAM_COPY_TEAM_STATUS_VICTORY)
                return;

            //失败
            if (teamDataModel.TeamStatus == TeamCopyTeamStatus.TEAM_COPY_TEAM_STATUS_FAILED)
            {
                if (TeamDuplicationDataManager.GetInstance().IsNeedShowGameFailResult == true)
                {
                    TeamDuplicationUtility.OnOpenTeamDuplicationGameResultFrame();
                    TeamDuplicationDataManager.GetInstance().IsNeedShowGameFailResult = false;
                }
            }

            //团本结束，成功或者失败
            UpdateGameOverView();
        }

        private void UpdateGameOverView()
        {
            CommonUtility.UpdateGameObjectVisible(gameOverRoot, true);

            //隐藏掉其他
            CommonUtility.UpdateGameObjectVisible(fightingRoot, false);
            if (fightCountDownTimeControl != null)
                fightCountDownTimeControl.ResetCountDownTimeController();
        }

        #endregion

        #region UIEvent
        //阶段开始的消息，自动打开攻坚面板
        private void OnTeamDuplicationFightStageBeginMessage(UIEvent uiEvent)
        {
            if(fightPanelControl != null)
                fightPanelControl.OnFightPanelShow();
        }

        //开战
        private void OnReceiveTeamDuplicationInBattleMessage(UIEvent uiEvent)
        {
            TeamDuplicationUtility.OnOpenTeamDuplicationFightCountDownFrame();

            CommonUtility.UpdateGameObjectVisible(fightStartButtonWithCd.gameObject, false);

            CommonUtility.UpdateButtonVisible(fightPreSettingButtonWithStart, false);


            UpdateFightRelationView(true);

            CommonUtility.UpdateGameObjectVisible(backButtonRoot, false);

        }


        //加载完成是否显示投票页面
        private void ShowTeamDuplicationFightVoteFrameBySceneChangeLoadingFinish()
        {
            //切换场景并同意标志
            if (TeamDuplicationDataManager.GetInstance().FightVoteAgreeBySwitchFightScene == false)
                return;

            //投票已经结束
            if (TeamDuplicationDataManager.GetInstance().IsInStartBattleVotingTime == false)
                return;

            //切换场景完成
            TeamDuplicationDataManager.GetInstance().FightVoteAgreeBySwitchFightScene = false;

            TeamDuplicationUtility.OnOpenTeamDuplicationFightStartVoteFrame();
            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyStartBattleVote(true);
        }

        //可能在loading的时候，收到了游戏结束的消息，loading加载完成之后，显示完成
        private void ShowTeamDuplicationGameOverBySceneChangeLoadingFinish()
        {
            ShowGameOverView();
        }

        //关卡结束
        private void OnReceiveTeamDuplicationFightStageEndNotifyMessage(UIEvent uiEvent)
        {
            TeamDuplicationUtility.OnCloseTeamDuplicationFightStagePanelFrame();
            
            ShowFightStageEndView();
        }

        //loading结束完成之后，可能展示阶段翻牌
        private void ShowTeamDuplicationFightStageEndViewBySceneChangeLoadingFinish()
        {
            ShowFightStageEndView();
        }

        private void ShowFightStageEndView()
        {
            if (TeamDuplicationDataManager.GetInstance().TeamDuplicationEndStageId <= 0)
                return;

            //展示阶段完成描述
            TeamDuplicationUtility.OnOpenTeamDuplicationFightStageEndDescriptionFrame(
                TeamDuplicationDataManager.GetInstance().TeamDuplicationEndStageId);

            //重置关卡
            TeamDuplicationDataManager.GetInstance().TeamDuplicationEndStageId = 0;
        }

        //团本数据修改
        private void OnReceiveTeamDuplicationTeamDataMessage(UIEvent uiEvent)
        {
            if ((TeamCopyStage)TeamDuplicationDataManager.GetInstance().TeamDuplicationFightStageId
                == TeamCopyStage.TEAM_COPY_STAGE_NULL)
            {
                //没有开战
                if (TeamDuplicationUtility.IsSelfPlayerIsTeamLeaderInTeamDuplication() == true)
                {
                    if (TeamDuplicationUtility.IsTeamDuplicationTeamDifficultyHardLevel() == true)
                    {
                        //噩梦难度
                        CommonUtility.UpdateButtonVisible(fightPreSettingButtonWithStart, false);
                        CommonUtility.UpdateGameObjectVisible(fightStartButtonWithCd.gameObject, true);
                    }
                    else
                    {
                        //噩梦难度
                        CommonUtility.UpdateButtonVisible(fightPreSettingButtonWithStart, true);
                        CommonUtility.UpdateGameObjectVisible(fightStartButtonWithCd.gameObject, true);
                    }
                }
                else
                {
                    //不是团长
                    CommonUtility.UpdateButtonVisible(fightPreSettingButtonWithStart, false);
                    CommonUtility.UpdateGameObjectVisible(fightStartButtonWithCd.gameObject, false);
                }
            }
            else
            {
                //开战，存在攻坚面板按钮，不进行操作
            }

            UpdateFightEndVoteByTeamDataMessage();
        }

        //阶段通知
        private void OnReceiveTeamDuplicationFightStageNotifyMessage(UIEvent uiEvent)
        {
            if (fightCountDownTimeControl == null)
                return;

            fightCountDownTimeControl.EndTime =
                TeamDuplicationDataManager.GetInstance().TeamDuplicationFightFinishTime;
            fightCountDownTimeControl.InitCountDownTimeController();
        }

        //游戏结束
        private void OnReceiveTeamDuplicationGameResultMessage(UIEvent uiEvent)
        {
            ShowGameOverView();
        }

        //阶段完成的描述
        private void OnReceiveTeamDuplicationStageEndDescriptionCloseMessage(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var stageId = (int) uiEvent.Param1;

            if(stageId == 1 || stageId == 2)
                TeamDuplicationUtility.OnOpenTeamDuplicationMiddleStageRewardFrame(stageId);
        }

        //阶段奖励的界面
        private void OnReceiveTeamDuplicationMiddleStageRewardCloseMessage(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var stageId = (int) uiEvent.Param1;
            if (stageId == 1)
            {
                //第一阶段小阶段翻牌完成，当前为第二阶段，展示第二阶段开始的描述
                if (TeamDuplicationDataManager.GetInstance().TeamDuplicationFightStageId == 2)
                {
                    TeamDuplicationUtility.OnOpenTeamDuplicationFightStageBeginDescriptionFrame(
                        TeamDuplicationDataManager.GetInstance().TeamDuplicationFightStageId);
                }
            }
            else
            {
                //第二阶段小阶段翻牌完成，展示作战成功的提示
                TeamDuplicationUtility.OnOpenTeamDuplicationGameResultFrame(true);
            }
        }

        //最终结果（战斗胜利）的界面，展示最终翻牌
        private void OnReceiveTeamDuplicationFinalResultCloseMessage(UIEvent uiEvent)
        {
            TeamDuplicationUtility.OnOpenTeamDuplicationFinalStageCardFrame();
        }

        //场景改变的loading加载完成，执行可能存在的相应动作
        private void OnReceiveSceneChangeLoadingFinish(UIEvent uiEvent)
        {
            //进入到组队场景,如果不存在队伍，则直接弹出提示框
            if (TeamDuplicationDataManager.GetInstance().IsTeamDuplicationOwnerTeam == false)
            {
                TeamDuplicationUtility.OnOpenTeamDuplicationFightSceneLeaveTipFrame();
                return;
            }

            _isLoadingFinish = true;

            ShowTeamDuplicationFightVoteFrameBySceneChangeLoadingFinish();
            ShowTeamDuplicationGameOverBySceneChangeLoadingFinish();
            ShowTeamDuplicationFightStageEndViewBySceneChangeLoadingFinish();
        }

        //退出团本，跳转到组队场景
        private void OnSwitchToBuildSceneByQuitTeam(UIEvent uiEvent)
        {
            //loading还没有完成，不进行场景的切换
            if (_isLoadingFinish == false)
                return;

            TeamDuplicationUtility.OnCloseRelationFrameBySwitchBuildSceneInTeamDuplication();
            //切换到团本的组队场景（如果在攻坚场景中）
            TeamDuplicationUtility.SwitchTeamDuplicationToBuildSceneByQuitTeam();
        }
        #endregion

        #region FightEndVote

        private void UpdateFightEndVoteByTeamDataMessage()
        {
            //非团长
            if (TeamDuplicationUtility.IsSelfPlayerIsTeamLeaderInTeamDuplication() == false)
            {
                CommonUtility.UpdateButtonWithCdVisible(fightEndVoteButtonWithCd, false);
                TeamDuplicationDataManager.GetInstance().TeamDuplicationFightEndVoteFlag = false;
                return;
            }

            UpdateFightEndVoteButton();
        }

        //收到标志位改变
        private void OnReceiveTeamDuplicationFightEndVoteFlagMessage(UIEvent uiEvent)
        {
            UpdateFightEndVoteButton();
        }

        private void OnReceiveTeamDuplicationFightEndVoteResultSucceedMessage(UIEvent uiEvent)
        {
            UpdateFightEndVoteButton();
        }


        //战斗结束投票按钮的更新
        private void UpdateFightEndVoteButton()
        {
            CommonUtility.UpdateButtonWithCdVisible(fightEndVoteButtonWithCd, false);
            //不是团长
            if (TeamDuplicationUtility.IsSelfPlayerIsTeamLeaderInTeamDuplication() == false)
                return;
            //标志位false
            if (TeamDuplicationDataManager.GetInstance().TeamDuplicationFightEndVoteFlag == false)
                return;
            //非战斗阶段
            if (TeamDuplicationUtility.IsTeamDuplicationInFightingStatus() == false)
                return;

            CommonUtility.UpdateButtonWithCdVisible(fightEndVoteButtonWithCd, true);
        }

        private void OnFightEndVoteButtonClick()
        {
            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyForceEndReq();
        }


        #endregion

    }
}
