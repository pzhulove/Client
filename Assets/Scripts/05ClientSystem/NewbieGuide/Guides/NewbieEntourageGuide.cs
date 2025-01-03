using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieEntourageGuide : NewbieGuideDataUnit
	{
		public NewbieEntourageGuide(int tid):base(tid){}

		public override void InitContent()
		{
            List<NewbieModifyData> ModifyDataTypeList = new List<NewbieModifyData>();
            NewbieModifyData data = new NewbieModifyData();
            data.iIndex = 1;
            data.ModifyDataType = NewBieModifyDataType.EntourageID;
            ModifyDataTypeList.Add(data);

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ClientSystemTownFrame",
				MainUIIconPath.point2,
				"想拥有一个帅气的伙伴吗？<color=#ff0000ff>随从</color>你值得拥有！",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
				TextTipType.TextTipType_Three,
                new Vector3(0,0,0),
                eNewbieGuideAgrsName.SaveBoot
            ));

// 			AddContent(new ComNewbieData(
// 				NewbieGuideComType.BUTTON,
// 				null,
// 				"ClientSystemTownFrame",
// 				MainUIIconPath.retinue,
// 				"",
// 				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
// 				TextTipType.TextTipType_Two
// 			));


			AddContent(new ComNewbieData(
				NewbieGuideComType.TOGGLE,
				null,
				"RetinueFrame",
				"Tabs/Tab1",
				"选择图鉴",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(0, 0, 0 )
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
                ModifyDataTypeList,
				"RetinueFrame",
				"RetinueInfo/RetinueTable/ViewPort/Content/1000/button",
				"解锁新的随从吧！",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,      
				new Vector3(),
				eNewbieGuideAgrsName.None,
				eNewbieGuideAgrsName.None,
				"RetinueInfo/RetinueTable/ViewPort/Content/1000/Progress"
			));

            AddContent(new ComNewbieData(
                NewbieGuideComType.WAIT,
                null,
                0.60f
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.PASS_THROUGH,
                null,
                "RetinueResultFrame",
                "Close"
            ));

 			AddContent(new ComNewbieData(
				NewbieGuideComType.TOGGLE,
 				null,
				"RetinueFrame",
				"Tabs/Tab0",
 				"选择上阵随从",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
 				TextTipType.TextTipType_Two
 			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.TOGGLE,
				null,
				"RetinueFrame",
                "RetinueInfo/Operate/MainRetinue/bg/back",
				"选择出战槽位",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two
			));

            AddContent(new ComNewbieData(
                NewbieGuideComType.WAIT,
                null,
                0.80f
            ));

            AddContent(new ComNewbieData(
				NewbieGuideComType.TOGGLE,
				null,
				"RetinueFrame",
                "RetinueInfo/Operate/List/ViewPort/Content/1000",
				"选择随从进行出战~",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION,
				null,
				"RetinueFrame",
                "RetinueInfo/Operate/Desc/top",
				"这里可以看到该随从的<color=#ff0000ff>助战技</color>~",
				ComNewbieGuideBase.eNewbieGuideAnchor.Left,
				TextTipType.TextTipType_Two
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION2,
				null,
				"你的冒险并不孤单，有了<color=#ff0000ff>随从</color>\n的陪伴，既可以在危机关头通\n过<color=#ff0000ff>助战技</color>帮你渡过难关，还可\n以提升技能的等级！"
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"RetinueFrame",
				"ComWnd/Title/Close",
				"",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two
			));
		}

		public override void InitCondition()
		{
			AddCondition(new NewbieConditionData(
				eNewbieGuideCondition.Level,
				new int[] { 11 }
			));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.FinishedMissionID,
                new int[] { 2209 }
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

