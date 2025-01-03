using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class TeamDuplicationFightSceneLeaveTipView : MonoBehaviour
    {

        private float _intervalTime = 0;

        [Space(25)] [HeaderAttribute("content")] [Space(10)]
        [SerializeField] private float countDownTime = 10.0f;
        [SerializeField] private Text contentLabel;
        [SerializeField] private Text countDownTimeTipLabel;
        [SerializeField] private Button backButton;

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
            if (backButton != null)
            {
                backButton.onClick.RemoveAllListeners();
                backButton.onClick.AddListener(OnBackButtonClick);
            }
        }

        private void UnBindUiEvents()
        {
            if (backButton != null)
            {
                backButton.onClick.RemoveAllListeners();
            }
        }

        public void Init()
        {
            InitData();
            InitView();
        }

        private void InitData()
        {
            _intervalTime = countDownTime;
            if (_intervalTime < 10.0f)
                _intervalTime = 10.0f;
        }

        private void InitView()
        {
            if (contentLabel != null)
                contentLabel.text = TR.Value("team_duplication_back_build_scene_by_leave_team");

            UpdateCountDownTimeTipLabel((int)_intervalTime);
        }

        private void UpdateCountDownTimeTipLabel(int time)
        {
            if (countDownTimeTipLabel != null)
            {
                countDownTimeTipLabel.text = string.Format(TR.Value("team_duplication_back_build_count_down_time_tip"),
                    time.ToString());
            }
        }

        private void Update()
        {
            _intervalTime -= Time.deltaTime;
            if (_intervalTime < 0)
            {
                BackToBuildScene();
            }
            else
            {
                UpdateCountDownTimeTipLabel((int) (_intervalTime + 1));
            }
        }

        private void OnBackButtonClick()
        {
            BackToBuildScene();
        }

        private void BackToBuildScene()
        {
            TeamDuplicationUtility.OnCloseTeamDuplicationFightSceneLeaveTipFrame();

            if (TeamDuplicationUtility.IsTeamDuplicationInFightScene() == true)
            {
                TeamDuplicationUtility.BackToTeamDuplicationBuildScene();
            }

        }
    }
}
