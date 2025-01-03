using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieTitleGuide : NewbieGuideDataUnit
	{
		public NewbieTitleGuide(int tid):base(tid){}

		public override void InitContent()
		{
            AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ClientSystemTownFrame",
                MainUIIconPath.packge,
				"恭喜你获得了一个<color=#ff0000ff>称号</color>，快来穿戴一下吧",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
				TextTipType.TextTipType_Three,
                new Vector3(),
                eNewbieGuideAgrsName.SaveBoot
            ));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
                null,
				"ActorShowFrame",
                "Title/Funcs/TitleBook/Btn",
				"打开称号簿",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(200, -50, 0),
                eNewbieGuideAgrsName.None,
                eNewbieGuideAgrsName.None,
                "Title/Funcs/TitleBook"
            ));

//			AddContent(new ComNewbieData(
//				NewbieGuideComType.INTRODUCTION,
//				null,
//				"TittleBookFrame",
//				"tittles/ScrollView/Viewport/content",
//				"这里显示具体的称号",
//				ComNewbieGuideBase.eNewbieGuideAnchor.Right,
//				TextTipType.TextTipType_Two,
//				new Vector3(0, -80, 0)
//			));
//
//			AddContent(new ComNewbieData(
//				NewbieGuideComType.INTRODUCTION,
//				null,
//				"TittleBookFrame",
//				"tittles/Detail/ScrollView",
//				"右侧显示具体的称号属性和获得条件",
//				ComNewbieGuideBase.eNewbieGuideAnchor.Left,
//				TextTipType.TextTipType_Two,
//				new Vector3(0, -80, 0)
//			));
//
//			AddContent(new ComNewbieData(
//				NewbieGuideComType.INTRODUCTION2,
//				null,
//				"称号不仅能增加属性，而且稀有称号\n还是可以拿来<color=#ff0000ff>“炫耀”</color>!"
//			));
			AddContent(new ComNewbieData(
				NewbieGuideComType.TOGGLE,
				null,
				"TitleBookFrame",
				"tabs/TCT_MISSION",
				"选择任务分页",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(-100, 50, 0)
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
                "TitleBookFrame",
				"tittles/ScrollView/Viewport/content/130193001",
				"选择已解锁的称号",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(-100, 50, 0),
                eNewbieGuideAgrsName.None,
                eNewbieGuideAgrsName.None,
                "tittles/ScrollView/Viewport/content/130193001"
            ));

//			AddContent(new ComNewbieData(
//				NewbieGuideComType.INTRODUCTION,
//				null,
//				"TittleBookFrame",
//				"tittles/Detail",
//				"这里可以查看称号的属性，下方是获得途径",
//				ComNewbieGuideBase.eNewbieGuideAnchor.Left,
//				TextTipType.TextTipType_Two
//			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
                "TitleBookFrame",
				"FuncControls/Equipt",
				"点击穿戴~",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(-200, 0, 0)
			));

//            AddContent(new ComNewbieData(
//     			NewbieGuideComType.BUTTON,
//    			null,
//     			"TittleBookFrame",
//     			MainUIIconPath.mission,
//     			"当然你也可以在这里查看其它还未获得的称号",
//     			ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
//     			TextTipType.TextTipType_Two
// 			));

            AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
                "TitleBookFrame",
                "tittle/close",
				"快回城镇里炫耀一下吧！",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(-550, -50, 0)
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ItemGroupFrame",
				"BG/Title/Close",
				"",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(0, 0, 0)
			));
		}

		public override void InitCondition()
		{
            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.Level,
                new int[] { 7 }
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.FinishedMissionID,
                new int[] { 2205 }
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

