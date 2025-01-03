using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieEnchantGuide : NewbieGuideDataUnit
	{
		public NewbieEnchantGuide(int tid):base(tid){}

		public override void InitContent()
		{
            List<NewbieModifyData> ModifyDataTypeList = new List<NewbieModifyData>();
            NewbieModifyData data = new NewbieModifyData();
            data.iIndex = 1;
            data.ModifyDataType = NewBieModifyDataType.EnchantID;
            ModifyDataTypeList.Add(data);

            List<NewbieModifyData> ModifyDataTypeList2 = new List<NewbieModifyData>();
            NewbieModifyData data2 = new NewbieModifyData();
            data2.iIndex = 1;
            data2.ModifyDataType = NewBieModifyDataType.EnchantMagicCardID;
            ModifyDataTypeList2.Add(data2);

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ClientSystemTownFrame",
				MainUIIconPath.point2,
				"可以对装备进行<color=#ff0000ff>附魔</color>咯~前往锻冶屋看看吧",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
				TextTipType.TextTipType_Three,
                new Vector3(),
                eNewbieGuideAgrsName.SaveBoot
            ));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ClientSystemTownFrame",
				MainUIIconPath.forge,
				"",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two
			));


			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION2,
				null,
				"附魔可以使用材料对装备进行\n<color=#ff0000ff>属性</color>添加，另外同品质的附魔\n卡还可以进行<color=#ff0000ff>合成</color>!"
			));


			AddContent(new ComNewbieData(
				NewbieGuideComType.TOGGLE,
				null,
				"SmithShopFrame",
                "VerticalFilter/FT_ADDMAGIC",
				"选择附魔切页",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two
			));

            AddContent(new ComNewbieData(
                NewbieGuideComType.WAIT,
                null,
                0.3f
            ));

            AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION,
                null,
                "SmithShopFrame",
				"ScrollView/ViewPort/Content",
				"选择装备之后，会显示可以进行附魔的材料",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two
			));

            AddContent(new ComNewbieData(
				NewbieGuideComType.TOGGLE,
                null,
                "SmithShopFrame",
                "Magic/AddMagic/Right/ScrollView/ViewPort/Content/210000901",
                "再选择一个附魔效果",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
                TextTipType.TextTipType_Two,
				new Vector3(-450,0,0)
            ));
//
//            AddContent(new ComNewbieData(
//				NewbieGuideComType.INTRODUCTION,
//                null,
//                "SmithShopFrame",
//                "Magic/AddMagic/Left/BtnAddMagic",
//                "不要忘了点击这里开始附魔哦~",
//                ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
//                TextTipType.TextTipType_Two
//            ));

            AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
                null,
				"SmithShopFrame",
				"Magic/AddMagic/Left/BtnAddMagic",
				"开始进行附魔吧",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
                TextTipType.TextTipType_Two
            ));

//             AddContent(new ComNewbieData(
//				NewbieGuideComType.TOGGLE,
// 				null,
//                "SmithShopFrame",
//				"VerticalFilter/FT_MERGE",
// 				"另外附魔卡可还可以合成，选择合成页签",
// 				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
// 				TextTipType.TextTipType_Two
// 			));
// 
// 			AddContent(new ComNewbieData(
//				NewbieGuideComType.INTRODUCTION,
// 				null,
//                "SmithShopFrame",
//				"Merge/MergeCard/Left/ScrollView/ViewPort/Content",
//				"相同品质的附魔卡才可进行合成，先选择一张附魔卡",
//				ComNewbieGuideBase.eNewbieGuideAnchor.Right,
// 				TextTipType.TextTipType_Two
// 			));
//
//			AddContent(new ComNewbieData(
//				NewbieGuideComType.INTRODUCTION,
//				null,
//				"SmithShopFrame",
//				"Merge/MergeCard/Right/ScrollView/ViewPort/Content",
//				"在右侧再选择可合成的附魔卡，有几率变为更高品质的附魔卡~",
//				ComNewbieGuideBase.eNewbieGuideAnchor.Left,
//				TextTipType.TextTipType_Two
//			));
//
//
//            AddContent(new ComNewbieData(
//                NewbieGuideComType.BUTTON,
//                null,
//                "SmithShopFrame",
//                "Close",
//                "继续挑战关卡吧~",
//                ComNewbieGuideBase.eNewbieGuideAnchor.ButtomLeft,
//                TextTipType.TextTipType_Two
//            ));
			AddContent(new ComNewbieData(
				NewbieGuideComType.WAIT,
				null,
				1.00f
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION2,
				null,
				"装备附魔是提升装备属性\n的重要一环，而且可随时\n更换，非常实用~"
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"EnchantResultFrame",
				"OK",
				"",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(0, 0, 0)
			));


			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"SmithShopFrame",
				"ComWnd/Title/Close",
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
				new int[] { 21 }
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

