using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieRankListGuide : NewbieGuideDataUnit
	{
		public NewbieRankListGuide(int tid):base(tid){}

		public override void InitContent()
		{
			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ClientSystemTownFrame",
                MainUIIconPath.ranklist,
				"可以查看排行榜喽",
				ComNewbieGuideBase.eNewbieGuideAnchor.Buttom,
				TextTipType.TextTipType_Three,
                new Vector3(),
                eNewbieGuideAgrsName.SaveBoot
            ));

			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION,
				null,
				"RanklistFrame",
                "Details/RankContent/Viewport",
				"这是等级榜，下方是你的排名哦~",
				ComNewbieGuideBase.eNewbieGuideAnchor.Left,
				TextTipType.TextTipType_Two
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION,
				null,
				"RanklistFrame",
                "Ranks/Viewport",
				"其他还有竞技榜和死亡之塔榜哦~",
				ComNewbieGuideBase.eNewbieGuideAnchor.Right,
				TextTipType.TextTipType_Two
			));

            AddContent(new ComNewbieData(
                NewbieGuideComType.BUTTON,
                null,
				"RanklistFrame",
                "Close",
                "继续挑战关卡吧",
                ComNewbieGuideBase.eNewbieGuideAnchor.ButtomLeft,
				TextTipType.TextTipType_Two,
				new Vector3(-50, 50, 0)
            ));
        }

		public override void InitCondition()
		{
			AddCondition(new NewbieConditionData(
				eNewbieGuideCondition.Level,
				new int[] { 8 }
			));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.SceneTown
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.
                ClientSystemTownFrameOpen
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.MainFrameMutex
            ));
        }
	}
}

