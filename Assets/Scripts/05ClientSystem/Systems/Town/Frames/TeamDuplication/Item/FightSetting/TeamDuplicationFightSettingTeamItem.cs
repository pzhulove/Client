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

    public class TeamDuplicationFightSettingTeamItem : MonoBehaviour
    {

        private int _teamIndex;
        private bool _isSelected;
        private OnTeamDuplicationFightSettingTeamButtonAction _fightSettingTeamButtonAction;

        [Space(10)] [HeaderAttribute("TeamInfo")] [Space(5)]
        [SerializeField] private GameObject teamSelectedFlag;
        [SerializeField] private Button teamButton;


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
            if (teamButton != null)
            {
                teamButton.onClick.RemoveAllListeners();
                teamButton.onClick.AddListener(OnTeamButtonClick);
            }
        }

        private void UnBindUiEvents()
        {
            if(teamButton != null)
                teamButton.onClick.RemoveAllListeners();
        }

        private void ClearData()
        {
            _teamIndex = 0;
            _isSelected = false;
            _fightSettingTeamButtonAction = null;
        }


        public void Init(int teamIndex,
            bool isSelected,
            OnTeamDuplicationFightSettingTeamButtonAction fightSettingTeamButtonAction)
        {
            _teamIndex = teamIndex;
            _isSelected = isSelected;
            _fightSettingTeamButtonAction = fightSettingTeamButtonAction;

            UpdateTeamItem();
        }

        private void UpdateTeamItem()
        {
            CommonUtility.UpdateGameObjectVisible(teamSelectedFlag, _isSelected);
        }

        private void OnTeamButtonClick()
        {
            _isSelected = !_isSelected;
            UpdateTeamItem();

            if (_fightSettingTeamButtonAction != null)
            {
                if (_isSelected == true)
                {
                    _fightSettingTeamButtonAction(_teamIndex);
                }
                else
                {
                    _fightSettingTeamButtonAction(0);
                }
            }
        }

        public void SetTeamItem(bool isSelected)
        {
            _isSelected = isSelected;
            UpdateTeamItem();
        }

        public int GetTeamIndex()
        {
            return _teamIndex;
        }


    }
}
