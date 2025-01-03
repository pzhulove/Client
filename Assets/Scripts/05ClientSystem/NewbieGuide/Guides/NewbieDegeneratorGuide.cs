using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieDegeneratorGuide : NewbieGuideDataUnit
	{
		public NewbieDegeneratorGuide(int tid):base(tid){}

		public override void InitContent()
		{
 			//人物说话
// 			AddContent(new ComNewbieData(
//				NewbieGuideComType.INTRODUCTION,
// 				null,
// 				"ClientSystemTownFrame",
//				MainUIIconPath.Degenerator,
// 				"为了对抗黑龙王，必须用能量石激发体内的异次元能量",
// 				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
// 				TextTipType.TextTipType_Three,
//                new Vector3(),
//                eNewbieGuideAgrsName.SaveBoot
//             ));

			//提示打开界面
			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ClientSystemTownFrame",
				MainUIIconPath.point2,
				"为了对抗<color=#ff0000ff>黑龙王</color>，必须用能量石激发体内的<color=#ff0000ff>次元石</color>",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
				TextTipType.TextTipType_Three,
				new Vector3(),
				eNewbieGuideAgrsName.SaveBoot
			));

// 			AddContent(new ComNewbieData(
// 				NewbieGuideComType.BUTTON,
// 				null,
// 				"ClientSystemTownFrame",
// 				MainUIIconPath.Degenerator,
// 				"",
// 				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
// 				TextTipType.TextTipType_Two
// 			));


			AddContent(new ComNewbieData(
				NewbieGuideComType.TOGGLE,
				null,
                "DegeneratorFrame",
                "StoneRoot/stone3",
				"达到对应等级后可开启蕴藏在体内的<color=#ff0000ff>次元石</color>",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(-300,0,0)
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
                "CommonMsgBoxOKCancel",
                "Back/Panel/btOK",
				"确认开启",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two
			));

 			AddContent(new ComNewbieData(
 				NewbieGuideComType.TOGGLE,
 				null,
                 "DegeneratorFrame",
                 "StoneRoot/stone3",
				"接下来我们进行充能吧！选择刚开启的<color=#ff0000ff>次元石</color>",
 				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(-500,0,0)
 			));
  
 			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
 				null,
				"WarpStoneLvUpFrame",
				"midddle_back/rock1/btCharge",
                "可以点击充能，也可以<color=#ff0000ff>长按</color>进行连续充能",
 				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
 				TextTipType.TextTipType_Two
 			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION2,
				null,
				"升级次元石可以提升角色的<color=#ff0000ff>属性</color>，\n快快收集<color=#ff0000ff>能量石</color>提升自己能力吧~"
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"WarpStoneLvUpFrame",
				"midddle_back/btClose",
				"",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"DegeneratorFrame",
				"btClose",
				"",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two
			));


		}

		public override void InitCondition()
		{
			AddCondition(new NewbieConditionData(
				eNewbieGuideCondition.Level,
				new int[] { 19 }
			));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.FinishedMissionID,
                new int[] { 2220 }
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

