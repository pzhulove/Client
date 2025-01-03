using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieDeathTowerGuide : NewbieGuideDataUnit
	{
		public NewbieDeathTowerGuide(int tid):base(tid){}

		public override void InitContent()
		{
            AddContent(new ComNewbieData(
                NewbieGuideComType.INTRODUCTION2,
                null,
                "<color=#ff0000ff>勇者之塔</color>开启咯,与普通关卡不同，需要从<color=#ff0000ff>活动</color>入口进入~",
                eNewbieGuideAgrsName.SaveBoot
            ));

//             AddContent(new ComNewbieData(
//                NewbieGuideComType.WAIT,
//                 null,
//                 0.5f
//             ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.TOGGLE,
                null,
                "ActivityDungeonFrame",
                "R/Single/ContentR/Collect/A/GridRoot/4/toggle",
                "点击查看活动详情",
                ComNewbieGuideBase.eNewbieGuideAnchor.Right,
                TextTipType.TextTipType_Two,
                new Vector3(-200, 0, 0)
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.BUTTON,
                null,
                "ActivityDungeonInfoFrame",
                "C/Middle/button/go",
                "关闭详情",
                ComNewbieGuideBase.eNewbieGuideAnchor.Right,
                TextTipType.TextTipType_Two,
                new Vector3(-200, 0, 0)
            ));

			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION,
				null,
                "ActivityDungeonFrame",
                "R/Single/ContentR/Collect/A/GridRoot/4/right/button/go",
				"马上开始挑战吧！",
				ComNewbieGuideBase.eNewbieGuideAnchor.Right,
				TextTipType.TextTipType_Two
			));
        }

		public override void InitCondition()
		{
			AddCondition(new NewbieConditionData(
				eNewbieGuideCondition.Level,
				new int[] { 16 }
			));

            //             AddCondition(new NewbieConditionData(
            //                 eNewbieGuideCondition.FinishedMissionID,
            //                 new int[] { 1608 }
            //             ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.SceneTown
            ));

//             AddCondition(new NewbieConditionData(
//                 eNewbieGuideCondition.ClientSystemTownFrameOpen
//             ));

//             AddCondition(new NewbieConditionData(
//                 eNewbieGuideCondition.MainFrameMutex
//             ));
        }
	}
}

