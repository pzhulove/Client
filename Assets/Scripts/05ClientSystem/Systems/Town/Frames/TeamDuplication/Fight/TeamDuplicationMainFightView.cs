using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class TeamDuplicationMainFightView : MonoBehaviour
    {
        [Space(15)]
        [HeaderAttribute("Control")]
        [Space(10)]
        [SerializeField]
        private Text titleLabel;

        [Space(15)]
        [HeaderAttribute("FightControl")]
        [Space(10)]
        [SerializeField] private TeamDuplicationFightCommonControl fightCommonControl;

        [Space(15)]
        [HeaderAttribute("commonControl")]
        [Space(10)]
        [SerializeField] private TeamDuplicationHeadControl headControl;
        [SerializeField] private TeamDuplicationTeamCaptainPanelControl teamCaptainPanelControl;

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
            InitTitleLabel();

            if (fightCommonControl != null)
                fightCommonControl.Init();

            if (headControl != null)
                headControl.Init();

            if (teamCaptainPanelControl != null)
                teamCaptainPanelControl.Init();
        }

        private void InitTitleLabel()
        {
            if (titleLabel == null)
                return;

            //噩梦难度
            if (TeamDuplicationUtility.IsTeamDuplicationTeamDifficultyHardLevel() == true)
            {
                titleLabel.text = TR.Value("team_duplication_hard_level_format");
            }
            else
            {
                //普通难度
                titleLabel.text = TR.Value("team_duplication_normal_Level_format");
            }
        }
    }
}
