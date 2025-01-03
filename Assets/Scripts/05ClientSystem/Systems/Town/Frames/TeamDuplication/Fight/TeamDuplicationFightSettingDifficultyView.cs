using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class TeamDuplicationFightSettingDifficultyView : MonoBehaviour
    {

        private List<TeamDuplicationTeamDifficultyConfigDataModel> _teamConfigDataModelList;


        [Space(15)]
        [HeaderAttribute("Common")]
        [Space(10)]
        [SerializeField] private Text titleLabel;
        [SerializeField] private Button closeButton;

        [Space(15)] [HeaderAttribute("Bottom")] [Space(10)]
        [SerializeField] private Text taskSelectedDetailText;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button settingFinishButton;

        [Space(15)] [HeaderAttribute("guideMode")] [Space(10)]
        [SerializeField] private List<TeamDuplicationFightSettingTaskDifficultyItem> taskDifficultyItemList;

        private void Awake()
        {
            BindUiEvents();
        }

        private void OnDestroy()
        {
            UnBindUiEvents();
            ClearData();
        }

        private void BindUiEvents()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseButtonClick);
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveAllListeners();
                cancelButton.onClick.AddListener(OnCancelButtonClick);
            }

            if (settingFinishButton != null)
            {
                settingFinishButton.onClick.RemoveAllListeners();
                settingFinishButton.onClick.AddListener(OnSettingFinishButtonClick);
            }
            
        }

        private void UnBindUiEvents()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveAllListeners();

            if (cancelButton != null)
                cancelButton.onClick.RemoveAllListeners();

            if (settingFinishButton != null)
                settingFinishButton.onClick.RemoveAllListeners();

        }

        private void ClearData()
        {
            _teamConfigDataModelList = null;
        }


        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }

        public void Init()
        {
            InitData();
            InitView();
        }

        private void InitData()
        {

            if (_teamConfigDataModelList == null)
                _teamConfigDataModelList = new List<TeamDuplicationTeamDifficultyConfigDataModel>();
            _teamConfigDataModelList.Clear();

            //缓存的数据
            var curTeamConfigDataModelList = TeamDuplicationDataManager.GetInstance().GetTeamConfigDataModelList();
            for (var i = 0; i < curTeamConfigDataModelList.Count; i++)
            {
                var curTeamConfigDataModel = curTeamConfigDataModelList[i];
                if(curTeamConfigDataModel == null)
                    continue;
                var teamConfigDataModel = new TeamDuplicationTeamDifficultyConfigDataModel();
                teamConfigDataModel.Difficulty = curTeamConfigDataModel.Difficulty;
                teamConfigDataModel.TeamId = curTeamConfigDataModel.TeamId;
                _teamConfigDataModelList.Add(teamConfigDataModel);
            }
        }


        private void InitView()
        {
            InitTitle();
            InitTaskDifficultyItemList();
        }

        private void InitTitle()
        {
            if (titleLabel != null)
                titleLabel.text = TR.Value("team_duplication_setting_title");

            if (taskSelectedDetailText != null)
                taskSelectedDetailText.text = TR.Value("team_duplication_setting_compare_troop_task");
        }

        private void InitTaskDifficultyItemList()
        {
            if (taskDifficultyItemList == null)
                return;

            if (_teamConfigDataModelList == null)
                return;

            for (var i = 0; i < taskDifficultyItemList.Count; i++)
            {
                var curItem = taskDifficultyItemList[i];
                if(curItem == null)
                    continue;

                if(i >= _teamConfigDataModelList.Count)
                    continue;

                var teamConfigDataModel = _teamConfigDataModelList[i];
                if(teamConfigDataModel == null)
                    continue;

                curItem.Init((TeamCopyGrade)(i+1),
                    teamConfigDataModel.TeamId,
                    OnFightSettingDifficultyButtonAction);
            }
        }

        private void OnFightSettingDifficultyButtonAction(int taskDifficultyIndex, int teamIndex)
        {
            if (_teamConfigDataModelList == null || _teamConfigDataModelList.Count <= 0)
                return;

            //保存当前难度选择的队伍
            for (var i = 0; i < _teamConfigDataModelList.Count; i++)
            {
                var teamConfigDataModel = _teamConfigDataModelList[i];
                if(teamConfigDataModel == null)
                    continue;
                if ((int) teamConfigDataModel.Difficulty == taskDifficultyIndex)
                    teamConfigDataModel.TeamId = teamIndex;
            }

            //如果取消，直接返回
            if (teamIndex == 0)
                return;
            
            //如果选择了某个队伍，则需要更其他难度选择队伍
            for (var i = 0; i < _teamConfigDataModelList.Count; i++)
            {
                var teamConfigDataModel = _teamConfigDataModelList[i];
                if(teamConfigDataModel == null)
                    continue;

                if((int)teamConfigDataModel.Difficulty == taskDifficultyIndex)
                    continue;

                //某个难度选择了其他的队伍
                if(teamConfigDataModel.TeamId != teamIndex)
                    continue;

                //某个难度选择了相同的队伍，则将该难度的队伍置为0;
                teamConfigDataModel.TeamId = 0;
                //UI更新
                if (i < taskDifficultyItemList.Count)
                {
                    var taskDifficultyItem = taskDifficultyItemList[i];
                    if (taskDifficultyItem != null)
                        taskDifficultyItem.SetSelectedTeamItem(0);
                }
            }
        }
        
        private void OnSettingFinishButtonClick()
        {
            //存在任务没有配置
            for (var i = 0; i < _teamConfigDataModelList.Count; i++)
            {
                var teamConfigDataModel = _teamConfigDataModelList[i];
                if(teamConfigDataModel == null)
                    continue;

                if (teamConfigDataModel.TeamId == 0)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_setting_not_finished"));
                    return;
                }
            }

            //保存攻坚类型和小队难度
            TeamDuplicationDataManager.GetInstance().UpdateTeamConfigDataModelList(_teamConfigDataModelList);

            //引导模式
            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_setting_guide_model_ok"));

            //关闭预设界面
            TeamDuplicationUtility.OnCloseTeamDuplicationFightPreSettingFrame();
            //关闭界面
            CloseFrame();

            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationSettingFinishMessage);
        }

        private void OnCancelButtonClick()
        {
            CloseFrame();
        }

        private void OnCloseButtonClick()
        {
            CloseFrame();
        }

        private void CloseFrame()
        {
            TeamDuplicationUtility.OnCloseTeamDuplicationFightSettingDifficultyFrame();
        }


    }
}
