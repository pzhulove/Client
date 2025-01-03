using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieMagicPotGuide : NewbieGuideDataUnit
	{
		public NewbieMagicPotGuide(int tid):base(tid){}

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
                MainUIIconPath.jar,
				"魔罐解锁了,神奇的罐子蕴藏着不同的<color=#ff0000ff>宝物</color>",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
                TextTipType.TextTipType_Three,
                new Vector3(-825,-300,0),
                eNewbieGuideAgrsName.SaveBoot
            ));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"JarsSelectFrame",
				"Type/magicJar",
				"",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two
			));


//            AddContent(new ComNewbieData(
//                NewbieGuideComType.INTRODUCTION,
//                null,
//                "MagicJarFrame",
//                "Content/Right/Items/Viewport/Content",
//                "可获得的奖励都在这里显示",
//                ComNewbieGuideBase.eNewbieGuideAnchor.Top,
//                TextTipType.TextTipType_Two
//            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.BUTTON,
                null,
                "MagicJarFrame",
                "Content/Right/Buy/Func(Clone)",
                "赶快来抽一个吧",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
                TextTipType.TextTipType_Two
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.WAIT,
                null,
                3.0f
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.INTRODUCTION2,
                null,
				"充满魔力的罐子，也许下\n一秒好运就会降临在你的\n身上，多多抽取吧"
            ));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ShowItemsFrame",
				"Result/Return",
				"",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(0,0,0)
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"MagicJarFrame",
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
				new int[] { 17 }
			));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.FinishedMissionID,
                new int[] { 2217 }
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

