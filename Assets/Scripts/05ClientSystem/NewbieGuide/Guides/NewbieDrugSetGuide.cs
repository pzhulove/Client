using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieDrugSetGuide : NewbieGuideDataUnit
    {
		public NewbieDrugSetGuide(int tid):base(tid){}

		public override void InitContent()
		{
			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ClientSystemTownFrame",
				MainUIIconPath.packge,
                "进入战斗前，可以提前配置好药剂哦",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
				TextTipType.TextTipType_Two,
                new Vector3(0, 0, 0),
                eNewbieGuideAgrsName.SaveBoot
            ));

			AddContent(new ComNewbieData(
				NewbieGuideComType.TOGGLE,
				null,
				"PackageNewFrame",
                "Content/ItemListTabs/Title3",
				"点击消耗品页签",
				ComNewbieGuideBase.eNewbieGuideAnchor.ButtomLeft,
				TextTipType.TextTipType_Two,
				new Vector3(-50, 30, 0)
			));

            AddContent(new ComNewbieData(
                NewbieGuideComType.BUTTON,
                null,
                "PackageNewFrame",
                "Content/Bottom/Ctrl/ChapterPotionSet",
                "选择药品配置",
                ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
                TextTipType.TextTipType_Two,
                new Vector3(-50, 30, 0)
            ));           

            AddContent(new ComNewbieData(
                NewbieGuideComType.WAIT,
                null,
                0.30f
            ));

            AddContent(new ComNewbieData(
               NewbieGuideComType.BUTTON,
               null,
               "ChapterBattlePotionSetFrame",
               "set/board/setDrugs/rt/setParent/PackagePotionSet(Clone)/sets/slotMain/BtnSet0",
               "选择药水槽位",
               ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
               TextTipType.TextTipType_Two,
               new Vector3(-50, 30, 0)
           ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.WAIT,
                null,
                0.30f
            ));

            //             AddContent(ComNewbieData.New<ComNewbieGuideDragSkill>(
            //                 null,
            //                 "ChapterBattlePotionSetFrame",
            //                 "board/drugs/drugItems/Content/ChapterBattlePotionUnit(Clone)",
            //                 "请拖动药品进行配置",
            //                 ComNewbieGuideBase.eNewbieGuideAnchor.ButtomLeft,
            //                 TextTipType.TextTipType_Two,
            //                 new Vector3(400, 220, 0),
            //                 "ChapterBattlePotionSetFrame",
            //                 "board/setDrugs/rt/setParent/BattlePotionSet(Clone)/sets/slotMain/drugSlot0",
            //                 "将药品拖拽进快捷药品栏",
            //                 ComNewbieGuideBase.eNewbieGuideAnchor.ButtomRight,
            //                 TextTipType.TextTipType_Two,
            //                 new Vector3(130, 70, 0)
            //             ));

            AddContent(new ComNewbieData(
              NewbieGuideComType.BUTTON,
              null,
              "ChapterBattlePotionSetFrame",
              "set/drugs/drugItems/Content/ChapterBattlePotionUnit(Clone)/bg",
              "选择药水",
              ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
              TextTipType.TextTipType_Two,
              new Vector3(-50, 30, 0)
          ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.WAIT,
                null,
                0.30f
            ));

            AddContent(new ComNewbieData(
              NewbieGuideComType.BUTTON,
              null,
              "ChapterBattlePotionSetFrame",
              "set/drugs/sure",
              "点击确定",
              ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
              TextTipType.TextTipType_Two,
              new Vector3(-50, 30, 0)
          ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.WAIT,
                null,
                0.30f
            ));            

            AddContent(new ComNewbieData(
                NewbieGuideComType.INTRODUCTION2,
                null,
                "在副本内携带多种药品，可长按滑动使用药品哦~"
            ));
        }

		public override void InitCondition()
		{
            //AddCondition(new NewbieConditionData(
            //    eNewbieGuideCondition.Level,
            //    new int[] { 6 }
            //));
            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.FinishedMissionID,
                new int[] { 2204 }
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.SceneTown
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.ClientSystemTownFrameOpen
            ));

//             AddCondition(new NewbieConditionData(
//                  eNewbieGuideCondition.MainFrameMutex
//             ));
        }
    }
}

