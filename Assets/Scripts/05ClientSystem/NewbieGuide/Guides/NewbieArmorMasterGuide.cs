using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieArmorMasterGuide : NewbieGuideDataUnit
	{
		public NewbieArmorMasterGuide(int tid):base(tid){}

		public override void InitContent()
		{
			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ClientSystemTownFrame",
				MainUIIconPath.packge,
				"恭喜转职成功，来看看有什么不同吧",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
				TextTipType.TextTipType_Three,
                new Vector3(),
                eNewbieGuideAgrsName.SaveBoot
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.BUTTON,
                null,
                "ActorShowFrame",
                "Title/Funcs/EquipMaster/Btn",
                "进阶职业穿戴对应护甲有加成哦，打开护甲精通界面",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
                TextTipType.TextTipType_Two,
                new Vector3(0, 0, 0),
                eNewbieGuideAgrsName.None,
                eNewbieGuideAgrsName.None,
                "Title/Funcs/EquipMaster"
            ));

			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION,
				null,
				"EquipMasterFrame",
				"Content/State",
				"每穿戴1件装备，就会有相应属性加成",
				ComNewbieGuideBase.eNewbieGuideAnchor.Buttom,
				TextTipType.TextTipType_Two
			));

//			AddContent(new ComNewbieData(
//				NewbieGuideComType.PASS_THROUGH,
//				null,
//				"EquipMasterFrame",
//                "Black",
//                "Content/Attr/BG",
//                "选择装备的时候前往不要忘了查看下护甲呦~",
//				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
//                TextTipType.TextTipType_Two
//            ));

//			AddContent(new ComNewbieData(
//				NewbieGuideComType.BUTTON,
//				null,
//                "ItemGroupFrame",
//                "Title/closeicon",
//                "再来看看其他有什么不同吧",
//				ComNewbieGuideBase.eNewbieGuideAnchor.ButtomLeft,
//				TextTipType.TextTipType_Two
//			));
//				
//			AddContent(new ComNewbieData(
//				NewbieGuideComType.BUTTON,
//				null,
//                "ClientSystemTownFrame",
//                "button/horizen/skill",
//                "转职后技能重置了，赶快学习新技能吧！",
//				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
//				TextTipType.TextTipType_Two
//			));
		}

		public override void InitCondition()
		{
			AddCondition(new NewbieConditionData(
				eNewbieGuideCondition.Level,
				new int[] { 15 }
			));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.SceneTown
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.ClientSystemTownFrameOpen
            ));

//             AddCondition(new NewbieConditionData(
//                 eNewbieGuideCondition.SpecificEvent,
//                 new int[] { (int)EUIEventID.ChangeJobFinished }
//             ));
        }
	}
}

