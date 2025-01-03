using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieBeatCowGuide : NewbieGuideDataUnit
	{
		public NewbieBeatCowGuide(int tid):base(tid){}

		public override void InitContent()
		{
            // 			AddContent(new ComNewbieData(
            // 				NewbieGuideComType.BUTTON,
            // 				null,
            // 				"ClientSystemTownFrame",
            // 				MainUIIconPath.Activitys,
            // 				"<color=#ff0000ff>国际斗牛节</color>开赛咯~",
            // 				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
            // 				TextTipType.TextTipType_Three,
            //                 new Vector3(0,0,0),
            //                 eNewbieGuideAgrsName.SaveBoot
            //             ));

            // 			AddContent(new ComNewbieData(
            // 				NewbieGuideComType.TOGGLE,
            // 				null,
            // 				"ActivityDungeonFrame",
            //                 "R/Single/ContentR/Collect/A/GridRoot/ActivityDailyUnit(Clone)/toggle",
            //                 "选择相应活动切页",
            // 				ComNewbieGuideBase.eNewbieGuideAnchor.Right,
            // 				TextTipType.TextTipType_Two,
            // 				new Vector3(-300,0,0)
            // 			));

            AddContent(new ComNewbieData(
                NewbieGuideComType.INTRODUCTION2,
                null,
                "<color=#ff0000ff>国际斗牛节</color>开赛咯~",
                eNewbieGuideAgrsName.SaveBoot
            ));

            AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION,
				null,
                "ActivityDungeonFrame",
                "R/Single/ContentR/Collect/A/GridRoot/1/toggle",
				"每天可以挑战<color=#ff0000ff>2次</color>，赶快来试试吧！",
				ComNewbieGuideBase.eNewbieGuideAnchor.Top,
				TextTipType.TextTipType_Two,
				new Vector3(-300,50,0)
			));
		}

		public override void InitCondition()
		{
			AddCondition(new NewbieConditionData(
				eNewbieGuideCondition.Level,
				new int[] { 23 }
			));

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

