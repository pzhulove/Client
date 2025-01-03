using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class TeamDuplicationTeamBuildView : MonoBehaviour
    {

        //默认是普通难度
        private uint _teamDifficultyLevel;

        private bool _isCanCreateGold = false;          //可以创建金团模式
        private TeamDuplicationTeamDifficultyControl _teamDifficultyControl;

        //特效
        private GameObject _challengeBaseEffectPrefab;
        private GameObject _goldBaseEffectPrefab;
        private GameObject _coverEffectPrefab;

        [Space(25)]
        [HeaderAttribute("GoldType")]
        [Space(20)]
        [SerializeField] private Button goldModelButton;

        [SerializeField] private GameObject goldLockContentRoot;
        [SerializeField] private Text goldLockContentText;
        [SerializeField] private Text goldUnLockContentText;
        [SerializeField] private UIGray goldImageGray;
        

        [Space(25)]
        [HeaderAttribute("ChallengeType")]
        [Space(20)]
        [SerializeField] private Button challengeModelButton;
        [SerializeField] private Text challengeContentText;

        [Space(25)]
        [HeaderAttribute("TeamDifficultyRoot")]
        [Space(20)]
        [SerializeField] private GameObject teamDifficultyRoot;

        [Space(25)] [HeaderAttribute("EffectRoot")] [Space(20)]
        [SerializeField] private GameObject challengeBaseEffectRoot;
        [SerializeField] private GameObject goldBaseEffectRoot;
        [SerializeField] private GameObject coverEffectRoot;

        [Space(25)] [HeaderAttribute("CloseButton")] [Space(20)]
        [SerializeField] private Button closeButton;

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
            _isCanCreateGold = false;
            _teamDifficultyControl = null;
            _teamDifficultyLevel = 0;

            _challengeBaseEffectPrefab = null;
            _goldBaseEffectPrefab = null;
            _coverEffectPrefab = null;
        }

        private void BindUiEvents()
        {

            if (goldModelButton != null)
            {
                goldModelButton.onClick.RemoveAllListeners();
                goldModelButton.onClick.AddListener(OnGoldModelButtonClick);
            }

            if (challengeModelButton != null)
            {
                challengeModelButton.onClick.RemoveAllListeners();
                challengeModelButton.onClick.AddListener(OnChallengeModelButtonClick);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseButtonClick);
            }
        }

        private void UnBindUiEvents()
        {

            if (goldModelButton != null)
                goldModelButton.onClick.RemoveAllListeners();

            if (challengeModelButton != null)
                challengeModelButton.onClick.RemoveAllListeners();

            if(closeButton != null)
                closeButton.onClick.RemoveAllListeners();
        }

        public void Init()
        {
            InitData();
            InitView();
        }

        private void InitData()
        {
            //默认普通难度
            _teamDifficultyLevel = (uint) TeamCopyTeamGrade.TEAM_COPY_TEAM_GRADE_COMMON;

            var playerInformationDataModel = TeamDuplicationDataManager.GetInstance().GetPlayerInformationDataModel();
            if (playerInformationDataModel != null)
                _isCanCreateGold = playerInformationDataModel.IsCanCreateGold;
        }


        private void InitView()
        {
            InitTeamDifficultyControl();

            InitGoldType();
            InitChallengeType();
        }

        #region TeamDifficulty
        //难度
        private void InitTeamDifficultyControl()
        {
            if (teamDifficultyRoot == null)
                return;

            _teamDifficultyControl = LoadTeamDifficultyControl(teamDifficultyRoot);
            if(_teamDifficultyControl != null)
                _teamDifficultyControl.Init(OnTeamDifficultyClickedAction);
        }

        private void OnTeamDifficultyClickedAction(uint teamDifficultyLevel)
        {
            if (_teamDifficultyLevel == teamDifficultyLevel)
                return;

            _teamDifficultyLevel = teamDifficultyLevel;

            UpdateTeamBuildViewByDifficultyLevel();
        }

        //刷新界面特效
        private void UpdateTeamBuildViewByDifficultyLevel()
        {

            //普通模式不显示特效
            if (_teamDifficultyLevel == (uint) TeamCopyTeamGrade.TEAM_COPY_TEAM_GRADE_COMMON)
            {
                CommonUtility.UpdateGameObjectVisible(coverEffectRoot, false);

                CommonUtility.UpdateGameObjectVisible(challengeBaseEffectRoot, false);

                CommonUtility.UpdateGameObjectVisible(goldBaseEffectRoot, false);
            }
            else
            {
                //噩梦难度，显示特效
                //整体特效
                CommonUtility.UpdateGameObjectVisible(coverEffectRoot, true);
                if (_coverEffectPrefab == null)
                    _coverEffectPrefab = CommonUtility.LoadGameObject(coverEffectRoot);

                //挑战特效
                CommonUtility.UpdateGameObjectVisible(challengeBaseEffectRoot, true);
                if (_challengeBaseEffectPrefab == null)
                    _challengeBaseEffectPrefab = CommonUtility.LoadGameObject(challengeBaseEffectRoot);

                //金团不能创建
                if (_isCanCreateGold == false)
                {
                    CommonUtility.UpdateGameObjectVisible(goldBaseEffectRoot, false);
                }
                else
                {
                    CommonUtility.UpdateGameObjectVisible(goldBaseEffectRoot, true);
                    if (_goldBaseEffectPrefab == null)
                        _goldBaseEffectPrefab = CommonUtility.LoadGameObject(goldBaseEffectRoot);
                }
            }
        }

        #endregion

        #region GoldModel
        private void InitGoldType()
        {

            if (_isCanCreateGold == true)
            {
                //可以创建金团模式
                CommonUtility.UpdateGameObjectVisible(goldLockContentRoot, false);
                CommonUtility.UpdateUIGrayVisible(goldImageGray, false);

                CommonUtility.UpdateTextVisible(goldUnLockContentText, true);

                if (goldUnLockContentText != null)
                    goldUnLockContentText.text = TR.Value("team_duplication_team_build_gold_unlock_content");
            }
            else
            {
                //不可以创建金团模式
                CommonUtility.UpdateGameObjectVisible(goldLockContentRoot, true);
                CommonUtility.UpdateUIGrayVisible(goldImageGray, true);

                CommonUtility.UpdateTextVisible(goldUnLockContentText, false);

                if (goldLockContentText != null)
                    goldLockContentText.text = TR.Value("team_duplication_team_build_gold_lock_content");
            }
        }

        private void OnGoldModelButtonClick()
        {
            if (_isCanCreateGold == true)
            {

                //可以创建
                var teamBuildDataModel = new TeamDuplicationTeamBuildDataModel();
                teamBuildDataModel.TeamDifficultyLevel = _teamDifficultyLevel;
                teamBuildDataModel.TeamModelType = (int) TeamCopyTeamModel.TEAM_COPY_TEAM_MODEL_GOLD;
                TeamDuplicationUtility.OnOpenTeamDuplicationTeamSettingFrame(teamBuildDataModel);
            }
            else
            {
                //不可以创建
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_team_build_gold_is_lock"));
            }
        }
        #endregion

        #region ChallengeModel
        private void InitChallengeType()
        {
            if (challengeContentText != null)
                challengeContentText.text = TR.Value("team_duplication_team_build_challenge_content");
        }

        private void OnChallengeModelButtonClick()
        {
            var teamBuildDataModel = new TeamDuplicationTeamBuildDataModel();
            teamBuildDataModel.TeamDifficultyLevel = _teamDifficultyLevel;
            teamBuildDataModel.TeamModelType = (int)TeamCopyTeamModel.TEAM_COPY_TEAM_MODEL_CHALLENGE;

            TeamDuplicationUtility.OnOpenTeamDuplicationTeamSettingFrame(teamBuildDataModel);
        }
        #endregion

        #region Button

        private void OnCloseButtonClick()
        {
            CommonUtility.UpdateButtonState(closeButton, null, false);
            TeamDuplicationUtility.OnCloseTeamDuplicationTeamBuildFrame();
        }


        #endregion 

        #region Helper
        private TeamDuplicationTeamDifficultyControl LoadTeamDifficultyControl(GameObject root)
        {
            if (root == null)
                return null;

            var teamDifficultyViewPrefab = CommonUtility.LoadGameObject(root);
            if (teamDifficultyViewPrefab == null)
                return null;

            var teamDifficultyControl = teamDifficultyViewPrefab.GetComponent<TeamDuplicationTeamDifficultyControl>();
            return teamDifficultyControl;
        }

        #endregion

    }
}
