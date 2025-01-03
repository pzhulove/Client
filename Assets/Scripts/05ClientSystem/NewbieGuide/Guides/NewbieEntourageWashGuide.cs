using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieEntourageWashGuide : NewbieGuideDataUnit
	{
		public NewbieEntourageWashGuide(int tid):base(tid){}

		public override void InitContent()
		{
//             AddContent(new ComNewbieData(
//                 NewbieGuideComType.BUTTON,
//                 null,
//                 "ClientSystemTownFrame",
//                 MainUIIconPath.retinue,
//                 "如果不满意随从的加成效果，可以随时进行洗练",
// 				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
// 				TextTipType.TextTipType_Three,
// 				new Vector3(-800,0,0),
//                 eNewbieGuideAgrsName.SaveBoot
//             ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.TOGGLE,
                null,
                "RetinueFrame",
                "Tabs/Tab1",
                "选择随从列表",
                ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
                TextTipType.TextTipType_Two,
                new Vector3(50, 30, 0)
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.TOGGLE,
                null,
                "RetinueFrame",
                "RetinueInfo/RetinueTable/ViewPort/Content/1000/Toggle",
                "选择要洗练的随从",
                ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
                TextTipType.TextTipType_Two,
                new Vector3(-250, 0, 0)
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.WAIT,
                null,
                0.30f
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.BUTTON,
                null,
                "RetinueFrame",
                "RetinueBodyFrame/BodyEx/HelpSkill/BtnChangeSkill",
                "点击洗练",
                ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
                TextTipType.TextTipType_Two,
                new Vector3(-400, 50, 0)
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.WAIT,
                null,
                0.50f
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.BUTTON,
                null,
                "RetinueChangeSkillFrame",
                "BtnChangeSkill",
                "升级随从技能需要消耗勇者之魂哦,点击升级吧",
                ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
                TextTipType.TextTipType_Two,
                new Vector3(-300, 50, 0)
            ));

            AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION2,
				null,
				"多多进行洗练，配出你所需要\n的技能加成吧！"
			));
        }

		public override void InitCondition()
		{
			AddCondition(new NewbieConditionData(
				eNewbieGuideCondition.Level,
				new int[] { 27 }
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

