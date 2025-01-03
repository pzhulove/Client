using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieMarketGuide : NewbieGuideDataUnit
	{
		public NewbieMarketGuide(int tid):base(tid){}

		public override void InitContent()
		{
			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ClientSystemTownFrame",
                MainUIIconPath.itemmarket,
				"拍卖行开张啦~去看看吧~",
				ComNewbieGuideBase.eNewbieGuideAnchor.Buttom,
				TextTipType.TextTipType_Three,
                new Vector3(),
                eNewbieGuideAgrsName.SaveBoot
            ));

			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION,
				null,
                "AuctionFrame",
                "Type",
				"这里是使用金票购买物品的地方，可以按照类型查看商品进行购买",
				ComNewbieGuideBase.eNewbieGuideAnchor.Top,
				TextTipType.TextTipType_Two
			));

//			AddContent(new ComNewbieData(
//				NewbieGuideComType.INTRODUCTION,
//				null,
//				"AuctionFrame",
//                "Type/ItemType1",
//				"可以按照类型查看商品进行购买",
//				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
//				TextTipType.TextTipType_Two
//			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION,
				null,
                "AuctionFrame",
                "btSearch",
				"也可以直接全局精确搜索",
				ComNewbieGuideBase.eNewbieGuideAnchor.Top,
				TextTipType.TextTipType_Two
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION,
				null,
                "AuctionFrame",
                "MyAuction",
				"上架和下架商品可以点击我的拍卖，注意最多上架5件物品哦",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
				TextTipType.TextTipType_Two,
				new Vector3(300, 20, 0)
			));

//			AddContent(new ComNewbieData(
//				NewbieGuideComType.INTRODUCTION,
//				null,
//                "AuctionFrame",
//                "MyAuction",
//				"注意最多上架5件物品哦",
//				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
//				TextTipType.TextTipType_Two
//			));

            AddContent(new ComNewbieData(
                NewbieGuideComType.BUTTON,
                null,
                "AuctionFrame",
                "btClose",
                "继续挑战关卡吧~",
				ComNewbieGuideBase.eNewbieGuideAnchor.ButtomLeft,
				TextTipType.TextTipType_Two,
				new Vector3(-50, 80, 0)
            ));
        }

		public override void InitCondition()
		{
            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.Level,
                new int[] { 12 }
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

