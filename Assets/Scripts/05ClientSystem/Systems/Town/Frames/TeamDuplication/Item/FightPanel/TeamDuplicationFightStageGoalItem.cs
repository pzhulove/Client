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

    //战斗阶段目标
    public class TeamDuplicationFightStageGoalItem : MonoBehaviour
    {

        private ComControlData _goalItemDataModel;
        [Space(10)] [HeaderAttribute("Text")] [Space(5)]
        [SerializeField] private Text goalTitleLabel;

        [SerializeField] private Text goalContentLabel;

        private void Awake()
        {

        }

        private void OnDestroy()
        {
            ClearData();
        }

        private void ClearData()
        {
            _goalItemDataModel = null;
        }


        public void Init(ComControlData comControlData)
        {
            _goalItemDataModel = comControlData;
            if (_goalItemDataModel == null)
                return;

            InitGoalItemView();
        }

        private void InitGoalItemView()
        {
            if (goalTitleLabel != null)
                goalTitleLabel.text = _goalItemDataModel.Name;

            UpdateGoalContent(_goalItemDataModel.Index);
        }

        public void UpdateGoalContent(int fightPointId = 0)
        {
            if (goalContentLabel == null)
                return;

            var contentStr = "";
            switch ((TeamDuplicationFightGoalType) _goalItemDataModel.Id)
            {
                case TeamDuplicationFightGoalType.CaptainGoal:
                    //小队目标
                    contentStr = GetFightCaptainGoalContent();
                    break;
                case TeamDuplicationFightGoalType.TeamDuplicationGoal:
                    //团本目标
                    contentStr = GetFightTeamGoalContent();
                    break;
                case TeamDuplicationFightGoalType.FightPointDescription:
                    //据点描述
                    contentStr = GetFightPointDescription(fightPointId);
                    break;
            }

            goalContentLabel.text = contentStr;
        }


        #region Helper
        //小队目标或者团本目标的Item
        public bool IsFightStageGoalItem()
        {
            if (_goalItemDataModel == null)
                return false;

            if ((TeamDuplicationFightGoalType) _goalItemDataModel.Id == TeamDuplicationFightGoalType.CaptainGoal
                || (TeamDuplicationFightGoalType) _goalItemDataModel.Id ==
                TeamDuplicationFightGoalType.TeamDuplicationGoal)
                return true;

            return false;
        }

        //据点描述的Item
        public bool IsFightPointItem()
        {
            if (_goalItemDataModel == null)
                return false;

            if ((TeamDuplicationFightGoalType) _goalItemDataModel.Id ==
                TeamDuplicationFightGoalType.FightPointDescription)
                return true;

            return false;
        }

        //得到小队目标的描述
        public static string GetFightCaptainGoalContent()
        {
            var captainGoalDescription = TeamDuplicationUtility.GetFightGoalDescription(
                TeamDuplicationDataManager.GetInstance().TeamDuplicationCaptainFightGoalDataModel);
            return captainGoalDescription;
        }

        //得到团本目标的描述
        public static string GetFightTeamGoalContent()
        {
            //团本目标的描述
            var teamGoalDescription = TeamDuplicationUtility.GetFightGoalDescription(
                TeamDuplicationDataManager.GetInstance().TeamDuplicationTeamFightGoalDataModel);
            return teamGoalDescription;
        }

        //得到据点的描述
        public static string GetFightPointDescription(int fightPointId)
        {
            var fightPointTable = TableManager.GetInstance()
                .GetTableItem<TeamCopyFieldTable>(fightPointId);
            if (fightPointTable != null)
            {
                return fightPointTable.StrongholdDesc;
            }
            return "";
        }
        #endregion

    }
}
