using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieDrugGuide : NewbieGuideDataUnit
	{
		public NewbieDrugGuide(int tid):base(tid){}

		public override void InitContent()
		{
            AddContent(new ComNewbieData(
                NewbieGuideComType.WAIT,
                null,
                0.1f,
                false,
                eNewbieGuideAgrsName.SaveBoot,
                eNewbieGuideAgrsName.PauseBattle
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.NEWICON_UNLOCK,
                null,
                "UIFlatten/Prefabs/NewbieGuide/DrugGuideMove",
                "",
                "",
                3f
            ));
            AddContent(new ComNewbieData(
                NewbieGuideComType.INTRODUCTION,
                null,
                "ClientSystemBattle",
                "DungeonDrugTips/V/Bg/rt/child/drugSlot0",
                "这里是主药品，点击该位置进行滑动，可以打开辅助药品界面",
                ComNewbieGuideBase.eNewbieGuideAnchor.ButtomLeft,
                TextTipType.TextTipType_One,
                new Vector3(-100, 0, 0)
                ));
            AddContent(new ComNewbieData(
                NewbieGuideComType.BATTLEDRUGDRAG,
                null,
                "ClientSystemBattle",
                "DungeonDrugTips/V/Bg/rt/child/drugSlot0/Item0",
                "进入战斗前，可以提前配置好药剂哦",
                ComNewbieGuideBase.eNewbieGuideAnchor.ButtomRight,
                TextTipType.TextTipType_Two,
                new Vector3(-600, -100, 0)
            ));
            AddContent(new GameClient.ComNewbieData(
                NewbieGuideComType.INTRODUCTION,
                null,
                "ClientSystemBattle",
                //"DungeonDrugTips/V/Bg/rt/child/drugSlot0",
                "DungeonDrugTips/V/Bg/rt/child/drugSlot0/Item2",
                "点击对应的药品即可使用，主药品可以直接点击使用快试试吧，冒险家~",
                ComNewbieGuideBase.eNewbieGuideAnchor.ButtomLeft,
                TextTipType.TextTipType_One,
                new Vector3(-400, 0, 0)
                ));
            AddContent(new ComNewbieData(
                NewbieGuideComType.WAIT,
                null,
                0.2f,
                false,
                eNewbieGuideAgrsName.ResumeBattle
            ));

            //             AddContent(new ComNewbieData(
            // 				NewbieGuideComType.BUTTON,
            //                 null,
            //                 "ClientSystemBattle",
            //                 "DungeonDrugTips/V/Bg/Drag/Item0",
            //                 "当遇到危机情况时，可以使用药品来回复生命值魔法值,点击即可直接使用",
            // 				ComNewbieGuideBase.eNewbieGuideAnchor.Right,
            // 				TextTipType.TextTipType_Three,
            //                 new Vector3(),
            //                 eNewbieGuideAgrsName.SaveBoot,
            //                 eNewbieGuideAgrsName.PauseBattle
            //             ));

            // 			AddContent(new ComNewbieData(
            // 				NewbieGuideComType.INTRODUCTION,
            // 				null,
            // 				"MissionFrameNew",
            // 				"AchievementContent/ScrollView/ViewPort/Content/3201/BtnAward",
            // 				"另外也可以展开药品栏，选择需要使用的药品！",
            // 				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
            // 				TextTipType.TextTipType_Two,
            // 				new Vector3(50 , 50, 0)
            // 			));
            // 
            // 			AddContent(new ComNewbieData(
            // 				NewbieGuideComType.INTRODUCTION,
            // 				null,
            // 				"MissionFrameNew",
            // 				"AchievementContent/ScrollView/ViewPort/Content/3201/BtnAward",
            // 				"灵活使用药品，使过关变得更加轻松，继续战斗吧！",
            // 				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
            // 				TextTipType.TextTipType_Two,
            // 				new Vector3(50 , 50, 0)
            // 			));
        }

		public override void InitCondition()
		{
            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.Level,
                new int[] { 6 }
            ));

            AddCondition(new NewbieConditionData(
				eNewbieGuideCondition.SceneBattle
            ));

//             AddCondition(new NewbieConditionData(
//                 eNewbieGuideCondition.DungeonID,
//                 new int[] { 102000 }
//             ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.BattleInitFinished
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.IsDungeon
            ));

            //             AddCondition(new NewbieConditionData(
            //                 eNewbieGuideCondition.DungeonStartTime,
            //                 new int[] { 2000 }
            //             ));

//             AddCondition(new NewbieConditionData(
//                 eNewbieGuideCondition.SpecificEvent,
//                 new int[] { (int)EUIEventID.DungeonOnFight }
//             ));
        }
	}
}

