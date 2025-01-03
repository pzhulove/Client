using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class TeamDuplicationFightPreSettingView : MonoBehaviour
    {

        [Space(15)]
        [HeaderAttribute("freeMode")]
        [Space(10)]
        [SerializeField] private Button freeModeButton;
        [SerializeField] private Text freeModeContentText;

        [Space(15)]
        [HeaderAttribute("guideMode")]
        [Space(10)]
        [SerializeField] private Button guideModeButton;
        [SerializeField] private Text guideModeSpecialText;
        [SerializeField] private Text guideModeContentText;
        
        private void Awake()
        {
            BindUiEvents();
        }

        private void OnDestroy()
        {
            UnBindUiEvents();
        }

        private void BindUiEvents()
        {

            if (freeModeButton != null)
            {
                freeModeButton.onClick.RemoveAllListeners();
                freeModeButton.onClick.AddListener(OnFreeModeButtonClick);
            }

            if (guideModeButton != null)
            {
                guideModeButton.onClick.RemoveAllListeners();
                guideModeButton.onClick.AddListener(OnGuideModeButtonClick);
            }
        }

        private void UnBindUiEvents()
        {

            if (freeModeButton != null)
            {
                freeModeButton.onClick.RemoveAllListeners();
            }

            if (guideModeButton != null)
            {
                guideModeButton.onClick.RemoveAllListeners();
            }
        }

        public void Init()
        {
            InitData();
            InitView();
        }

        private void InitData()
        {
        }


        private void InitView()
        {
            InitFreeMode();
            InitGuideMode();
        }

        private void InitFreeMode()
        {
            if (freeModeContentText != null)
                freeModeContentText.text = TR.Value("team_duplication_fight_pre_setting_free_mode_content");
        }

        private void InitGuideMode()
        {
            if (guideModeContentText != null)
                guideModeContentText.text = TR.Value("team_duplication_fight_pre_setting_guide_mode_content");
        }

        private void OnFreeModeButtonClick()
        {
            var contentStr = string.Format(TR.Value("team_duplication_model_sure_content"),
                TR.Value("team_duplication_fight_pre_setting_free_mode"),
                TR.Value("team_duplication_fight_pre_setting_guide_mode"));

            TeamDuplicationUtility.OnShowCommonMsgBoxFrame(contentStr,
                OnFreeModelSureAction,
                TextAnchor.MiddleLeft);
        }

        private void OnGuideModeButtonClick()
        {
            //设置类型
            TeamDuplicationDataManager.GetInstance().FightSettingConfigPlanModel =
                TeamCopyPlanModel.TEAM_COPY_PLAN_MODEL_GUIDE;
            //如果难度没有设置，则设置为默认难度
            if (TeamDuplicationDataManager.GetInstance().IsTeamDuplicationTeamDifficultyConfigNotSet() == true)
            {
                TeamDuplicationUtility.SetTeamDuplicationDefaultCaptainDifficultySetting();
            }

            TeamDuplicationUtility.OnOpenTeamDuplicationFightSettingDifficultyFrame();
        }

        private void OnFreeModelSureAction()
        {

            TeamDuplicationUtility.OnCloseTeamDuplicationFightPreSettingFrame();

            TeamDuplicationDataManager.GetInstance().FightSettingConfigPlanModel =
                TeamCopyPlanModel.TEAM_COPY_PLAN_MODEL_FREE;

            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_setting_free_model_ok"));
        }
    }
}
