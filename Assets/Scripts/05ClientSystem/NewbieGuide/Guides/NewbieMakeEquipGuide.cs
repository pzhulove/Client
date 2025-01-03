using UnityEngine;

namespace GameClient
{
	public class NewbieMakeEquipGuide : NewbieGuideDataUnit
    {
		public NewbieMakeEquipGuide(int tid):base(tid){}

		public override void InitContent()
		{
            AddContent(new ComNewbieData(
                NewbieGuideComType.WAIT,
                null,
                0.5f
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.BUTTON,
                null,
                "ClientSystemTownFrame",
				MainUIIconPath.point2,
                "打造开启啦，赶快来看一看打造吧",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
				TextTipType.TextTipType_Three,
                new Vector3(0,0,0),
                eNewbieGuideAgrsName.SaveBoot
            ));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ClientSystemTownFrame",
				MainUIIconPath.EquipForge,
				"",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two
			));


//            AddContent(new ComNewbieData(
//				NewbieGuideComType.INTRODUCTION,
//                null,
//                "EquipForgeFrame",
//                "TabGroup/Tabs",
//                "这里选择防具或者武器",
//				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
//				TextTipType.TextTipType_Two
//            ));

//            AddContent(new ComNewbieData(
//                NewbieGuideComType.INTRODUCTION,
//                null,
//                "EquipForgeFrame",
//                "TabGroup/Page/TreeList/Viewport",
//                "左侧显示可打造的装备，可打造的武器会有文字显示",
//				ComNewbieGuideBase.eNewbieGuideAnchor.Right,
//				TextTipType.TextTipType_Two
//            ));

//            AddContent(new ComNewbieData(
//                NewbieGuideComType.TOGGLE,
//                null,
//                "EquipForgeFrame",
//                "TabGroup/Page/TreeList/Viewport/Content/Group(Clone)/SubTypes/0",
//                "选择职业对应的武器",
//				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
//                TextTipType.TextTipType_Two
//            ));

//            AddContent(new ComNewbieData(
//                NewbieGuideComType.INTRODUCTION,
//                null,
//                "EquipForgeFrame",
//                "TabGroup/Page/ForgeGroup/Materials/RequireGroup",
//                "下方显示的是打造所需的材料",
//                ComNewbieGuideBase.eNewbieGuideAnchor.Left,
//                TextTipType.TextTipType_Two
//            ));
//
//            AddContent(new ComNewbieData(
//                NewbieGuideComType.INTRODUCTION,
//                null,
//                "EquipForgeFrame",
//                "TabGroup/Page/ForgeGroup/Detail",
//                "右侧显示了装备的属性",
//                ComNewbieGuideBase.eNewbieGuideAnchor.Left,
//                TextTipType.TextTipType_Two
//            ));

//            AddContent(new ComNewbieData(
//                NewbieGuideComType.BUTTON,
//                null,
//                "EquipForgeFrame",
//                "TabGroup/Page/ForgeGroup/Forge",
//                "开始打造！",
//				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
//				TextTipType.TextTipType_Two,
//				new Vector3(-300,0,0)
//            ));
//
//            AddContent(new ComNewbieData(
//                NewbieGuideComType.WAIT,
//                null,
//                3.0f
//            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.INTRODUCTION2,
                null,
                "收集材料，打造装备，你也可以全身神装！"
            ));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"EquipForgeFrame",
				"BG/Title/Close",
				"",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(0,0,0)
			));

        }

		public override void InitCondition()
		{
            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.Level,
                new int[] { 20 }
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.FinishedMissionID,
                new int[] { 2295 }
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.SceneTown
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.ClientSystemTownFrameOpen
            ));

//             AddCondition(new NewbieConditionData(
//                 eNewbieGuideCondition.MainFrameMutex
//             ));
        }
	}
}

