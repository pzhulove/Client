using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieMagicMaleGuide : NewbieGuideDataUnit
	{
		public NewbieMagicMaleGuide(int tid):base(tid){}

		public override void InitContent()
		{
            // 			AddContent(new ComNewbieData(
            // 				NewbieGuideComType.BUTTON,
            // 				null,
            // 				"ClientSystemTownFrame",
            // 				MainUIIconPath.Activitys,
            // 				"新的地下城开启咯~，我们就来看看去哪获得这些<color=#ff0000ff>能量石</color>吧",
            // 				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
            // 				TextTipType.TextTipType_Three,
            //                 new Vector3(0,0,0),
            //                 eNewbieGuideAgrsName.SaveBoot
            //             ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.INTRODUCTION2,
                null,
                "新的地下城开启咯~，我们就来看看去哪获得这些<color=#ff0000ff>能量石</color>吧",
                eNewbieGuideAgrsName.SaveBoot
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.INTRODUCTION,
                null,
                "ActivityDungeonFrame",
                "R/Single/ContentR/Collect/A/GridRoot/2/toggle",
                "这里是盛产能量石的地方",
                ComNewbieGuideBase.eNewbieGuideAnchor.Left,
                TextTipType.TextTipType_Two,
                new Vector3(-200, 0, 0)
            ));

            AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION2,
				null,
				"可以消耗点券使奖励<color=#ff0000ff>翻倍</color>，甚至三倍哦~"
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION,
				null,
				"ActivityDungeonFrame",
                "R/Single/ContentR/Collect/A/GridRoot/2/right/button/go",
				"每天只有<color=#ff0000ff>3次</color>机会哦，马上开始挑战吧！",
				ComNewbieGuideBase.eNewbieGuideAnchor.Left,
				TextTipType.TextTipType_Two,
				new Vector3(-200,0,0)
			));
        }

		public override void InitCondition()
		{
			AddCondition(new NewbieConditionData(
				eNewbieGuideCondition.Level,
				new int[] { 19 }
			));

//             AddCondition(new NewbieConditionData(
//                 eNewbieGuideCondition.FinishedMissionID,
//                 new int[] { 2220 }
//             ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.SceneTown
            ));

//             AddCondition(new NewbieConditionData(
//                 eNewbieGuideCondition.
//                 ClientSystemTownFrameOpen
//             ));

//             AddCondition(new NewbieConditionData(
//                 eNewbieGuideCondition.MainFrameMutex
//             ));
        }
	}
}

