using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieGoldPotGuide : NewbieGuideDataUnit
	{
		public NewbieGoldPotGuide(int tid):base(tid){}

		public override void InitContent()
		{
            AddContent(new ComNewbieData(
                NewbieGuideComType.BUTTON,
                null,
                "ClientSystemTownFrame",
                MainUIIconPath.goldJar,
				"金罐解锁了,神奇的罐子蕴藏着不同的<color=#ff0000ff>装备</color>",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Three,
				new Vector3(-825,-300,0),
                eNewbieGuideAgrsName.SaveBoot
            ));

			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION,
				null,
				"GoldJarFrame",
				"Content/TreeList/Viewport",
				"左侧可以选择不同的护甲类型，对应的护甲类型会有推荐标识~",
				ComNewbieGuideBase.eNewbieGuideAnchor.Right,
				TextTipType.TextTipType_Two
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION,
				null,
				"GoldJarFrame",
				"Content/TabGroup/Tabs",
				"上方还可选择不同等级~",
				ComNewbieGuideBase.eNewbieGuideAnchor.Buttom,
				TextTipType.TextTipType_Two
			));

            AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
                "GoldJarFrame",
                "Content/TabGroup/Page/Right/Buy/Func(Clone)",
                "点击抽取",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
                new Vector3(-300,0,0)
            ));

			AddContent(new ComNewbieData(
				NewbieGuideComType.WAIT,
				null,
				3.0f
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION2,
				null,
				"根据你自己的等级和护甲类型，抽取对应的装备吧！"
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
				"GoldJarFrame",
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
                eNewbieGuideCondition.SceneTown
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.ClientSystemTownFrameOpen
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.MainFrameMutex
            ));
        }
	}
}

