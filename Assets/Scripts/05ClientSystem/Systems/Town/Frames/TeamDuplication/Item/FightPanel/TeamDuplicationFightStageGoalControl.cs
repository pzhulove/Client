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

    //战斗目标控制器
    public class TeamDuplicationFightStageGoalControl : MonoBehaviour
    {

        [SerializeField]
        private List<TeamDuplicationFightStageGoalItem> fightStageGoalItemList =
            new List<TeamDuplicationFightStageGoalItem>();
        
        public void Init(List<ComControlData> goalItemDataModelList)
        {
            if (goalItemDataModelList == null || goalItemDataModelList.Count <= 0)
                return;

            var i = 0;
            for (; i < goalItemDataModelList.Count && i < fightStageGoalItemList.Count; i++)
            {
                var goalItemDataModel = goalItemDataModelList[i];
                var fightStageGoalItem = fightStageGoalItemList[i];

                if (goalItemDataModel != null && fightStageGoalItem != null)
                {
                    CommonUtility.UpdateGameObjectVisible(fightStageGoalItem.gameObject, true);
                    fightStageGoalItem.Init(goalItemDataModel);
                }
            }

            for (var j = i; j < fightStageGoalItemList.Count; j++)
            {
                var fightStageGoalItem = fightStageGoalItemList[j];
                if (fightStageGoalItem != null)
                {
                    fightStageGoalItem.Init(null);
                    CommonUtility.UpdateGameObjectVisible(fightStageGoalItem.gameObject, false);
                }
            }

        }
        
        //更新小队目标和团队目标内容
        public void UpdateFightStageGoalView()
        {
            for (var i = 0; i < fightStageGoalItemList.Count; i++)
            {
                var fightStageGoalItem = fightStageGoalItemList[i];
                if(fightStageGoalItem == null)
                    continue;

                if (fightStageGoalItem.gameObject.activeSelf == false)
                    continue;

                if(fightStageGoalItem.IsFightStageGoalItem() == false)
                    continue;

                fightStageGoalItem.UpdateGoalContent();
            }
        }

        //更新据点的描述内容
        public void UpdateFightStageFightPointDescriptionView(
            TeamDuplicationFightPointDataModel selectedFightPointDataModel)
        {
            for (var i = 0; i < fightStageGoalItemList.Count; i++)
            {
                var fightStageGoalItem = fightStageGoalItemList[i];
                if (fightStageGoalItem == null)
                    continue;

                if (fightStageGoalItem.gameObject.activeSelf == false)
                    continue;

                if (fightStageGoalItem.IsFightPointItem() == false)
                    continue;

                //据点描述
                fightStageGoalItem.UpdateGoalContent(selectedFightPointDataModel.FightPointId);
            }
        }

    }
}
