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

    public delegate void OnTeamDuplicationFightSettingDifficultyButtonAction(
        int taskIndex, int teamIndex);

    public delegate void OnTeamDuplicationFightSettingTeamButtonAction(int teamIndex);
    //
    public class TeamDuplicationFightSettingTaskDifficultyItem : MonoBehaviour
    {

        private TeamCopyGrade _teamCopyGrade;
        private int _selectedTeamIndex = 0;
        private OnTeamDuplicationFightSettingDifficultyButtonAction _fightSettingDifficultyButtonAction;

        [Space(10)]
        [HeaderAttribute("Text")]
        [Space(5)]
        [SerializeField] private Text taskDifficultyLabel;

        [SerializeField]
        private List<TeamDuplicationFightSettingTeamItem> fightSettingTeamItemList =
            new List<TeamDuplicationFightSettingTeamItem>();

        private void Awake()
        {

        }

        private void OnDestroy()
        {
            ClearData();
        }

        private void ClearData()
        {
            _teamCopyGrade = TeamCopyGrade.TEAM_COPY_GRADE_TEAM;
            _selectedTeamIndex = 0;
            _fightSettingDifficultyButtonAction = null;
        }


        public void Init(TeamCopyGrade teamCopyGrade,
            int selectedTeamIndex,
            OnTeamDuplicationFightSettingDifficultyButtonAction fightSettingDifficultyButtonAction)
        {

            _teamCopyGrade = teamCopyGrade;
            _selectedTeamIndex = selectedTeamIndex;
            _fightSettingDifficultyButtonAction = fightSettingDifficultyButtonAction;

            InitItem();
        }

        private void InitItem()
        {
            InitTaskInfo();

            InitTeamItemList();
        }

        private void InitTaskInfo()
        {
            if (taskDifficultyLabel == null)
                return;

            var taskStr = "";
            if (_teamCopyGrade == TeamCopyGrade.TEAM_COPY_GRADE_A)
            {
                taskStr = TR.Value("team_duplication_setting_difficulty_A");
            }
            else if (_teamCopyGrade == TeamCopyGrade.TEAM_COPY_GRADE_B)
            {
                taskStr = TR.Value("team_duplication_setting_difficulty_B");
            }
            else if (_teamCopyGrade == TeamCopyGrade.TEAM_COPY_GRADE_C)
            {
                taskStr = TR.Value("team_duplication_setting_difficulty_C");
            }
            else
            {
                taskStr = TR.Value("team_duplication_setting_difficulty_D");
            }

            taskDifficultyLabel.text = taskStr;
        }

        private void InitTeamItemList()
        {
            if (fightSettingTeamItemList == null || fightSettingTeamItemList.Count <= 0)
                return;

            for (var i = 0; i < fightSettingTeamItemList.Count; i++)
            {
                var teamItem = fightSettingTeamItemList[i];
                if(teamItem == null)
                    continue;

                var teamIndex = i + 1;
                bool isSelected = _selectedTeamIndex == teamIndex;

                teamItem.Init(teamIndex, 
                    isSelected,
                    OnTeamDuplicationFightSettingTeamItemAction);
            }
        }

        private void OnTeamDuplicationFightSettingTeamItemAction(int teamIndex)
        {
            _selectedTeamIndex = teamIndex;
            UpdateTeamItemList();

            if (_fightSettingDifficultyButtonAction != null)
            {
                _fightSettingDifficultyButtonAction((int)_teamCopyGrade,
                    _selectedTeamIndex);
            }
        }

        private void UpdateTeamItemList()
        {
            if (fightSettingTeamItemList == null || fightSettingTeamItemList.Count <= 0)
                return;

            for (var i = 0; i < fightSettingTeamItemList.Count; i++)
            {
                var teamItem = fightSettingTeamItemList[i];
                if(teamItem == null)
                    continue;

                if (teamItem.GetTeamIndex() == _selectedTeamIndex)
                {
                    teamItem.SetTeamItem(true);
                }
                else
                {
                    teamItem.SetTeamItem(false);
                }
            }
        }

        public void SetSelectedTeamItem(int selectedTeamIndex)
        {
            _selectedTeamIndex = selectedTeamIndex;

            UpdateTeamItemList();
        }

        public int GetTaskDifficultyIndex()
        {
            return (int) _teamCopyGrade;
        }

    }
}
