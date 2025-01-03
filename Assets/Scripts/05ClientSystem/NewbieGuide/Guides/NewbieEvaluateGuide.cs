using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieEvaluateGuide : NewbieGuideDataUnit
	{
		public NewbieEvaluateGuide(int tid):base(tid){}

		public override void InitContent()
		{
            AddContent(new ComNewbieData(
                NewbieGuideComType.SHOW_IMAGE,
                null,
                "UI/Image/NewbieGuide/NewbieGuide_4.png:NewbieGuide_4",
                eNewbieGuideAgrsName.SaveBoot,
                eNewbieGuideAgrsName.PauseBattle
            ));

			AddContent(new ComNewbieData(
				NewbieGuideComType.STRESS,
				null,
				"ClientSystemBattle",
				"DungeonMap/RScore",
				2.0f,
				eNewbieGuideAgrsName.None,
				eNewbieGuideAgrsName.None,
				eNewbieGuideAgrsName.ResumeBattle
			));

            // 			AddContent(new ComNewbieData(
            // 				NewbieGuideComType.BUTTON,
            // 				null,
            // 				"ClientSystemTownFrame",
            // 				"button/vertical/mission",
            // 				"注意关卡评价也可在关卡中查看",
            // 				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
            // 				TextTipType.TextTipType_Three,
            //                 new Vector3(),
            //                 eNewbieGuideAgrsName.SaveBoot
            //             ));
            // 
            // 			AddContent(new ComNewbieData(
            // 				NewbieGuideComType.INTRODUCTION,
            // 				null,
            // 				"MissionFrameNew",
            // 				"VerticalFilter/FFT_ACHIEVEMENT",
            // 				"三个评价条件分别是时间，被击次数以及复活币使用次数~",
            // 				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
            // 				TextTipType.TextTipType_Two,
            // 				new Vector3(200 , 0, 0)
            // 			));
            // 
            // 			AddContent(new ComNewbieData(
            // 				NewbieGuideComType.BUTTON,
            // 				null,
            // 				"MissionFrameNew",
            // 				"AchievementContent/ScrollView/ViewPort/Content/3201/BtnAward",
            // 				"战斗中可随时查看评价，继续战斗吧！",
            // 				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
            // 				TextTipType.TextTipType_Two,
            // 				new Vector3(50 , 50, 0)
            // 			));
        }

		public override void InitCondition()
		{
            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.Level,
                new int[] { 1 }
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.SceneBattle
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.DungeonID,
                new int[] { 101000 }
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.BattleInitFinished
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.SpecificEvent,
                new int[] { (int)EUIEventID.DungeonOnFight }
            ));
        }
	}
}

