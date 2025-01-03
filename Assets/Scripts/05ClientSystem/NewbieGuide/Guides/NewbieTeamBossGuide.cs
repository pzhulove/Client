using System;
using UnityEngine;
using System.Collections.Generic;

namespace GameClient
{
	public class NewbieTeamBossGuide : NewbieGuideDataUnit
    {
		public NewbieTeamBossGuide(int tid):base(tid){}

		public override void InitContent()
		{
//             AddContent(new ComNewbieData(
// 				NewbieGuideComType.BUTTON,
//                 null,
//                 "ClientSystemTownFrame",
// 				MainUIIconPath.Activitys,
//                 "那么去那里获得这些材料？Boss挑战等你来战！",
// 				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
// 				TextTipType.TextTipType_Three,
// 				new Vector3(0, 0, 0),
//                 eNewbieGuideAgrsName.SaveBoot
//             ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.INTRODUCTION2,
                null,
                "那么去那里获得这些材料？Boss挑战等你来战！",
                eNewbieGuideAgrsName.SaveBoot
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.TOGGLE,
                null,
                "ActivityDungeonFrame",
                "R/Single/Tabs/Activity1",
                "选择首领切页",
                ComNewbieGuideBase.eNewbieGuideAnchor.ButtomLeft,
                TextTipType.TextTipType_Two,
                new Vector3(0, 0, 0)
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.WAIT,
                null,
                0.3f
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.INTRODUCTION,
                null,
                "ActivityDungeonFrame",
                "R/Single/ContentR/Collect/A/GridRoot/11/toggle",
                "与普通副本基本相同，但这里掉落稀有的打造材料",
				ComNewbieGuideBase.eNewbieGuideAnchor.Right,
                TextTipType.TextTipType_Two,
                new Vector3(0, 0, 0)
            ));
        }

		public override void InitCondition()
		{
            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.Level,
                new int[] { 20 }
            ));

//             AddCondition(new NewbieConditionData(
//                 eNewbieGuideCondition.FinishedMissionID,
//                 new int[] { 2295 }
//             )

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.SceneTown
            ));

//             AddCondition(new NewbieConditionData(
//                 eNewbieGuideCondition.ClientSystemTownFrameOpen
//             )

//             AddCondition(new NewbieConditionData(
//                 eNewbieGuideCondition.MainFrameMutex
//             ));
        }
	}
}

