using UnityEngine;

namespace GameClient
{
	public class NewbieDungeonRewardGuide : NewbieGuideDataUnit
    {
		public NewbieDungeonRewardGuide(int tid):base(tid){}

		public override void InitContent()
		{
            AddContent(new ComNewbieData(
                NewbieGuideComType.WAIT,
                null,
                0.3f
            ));

            AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"DungeonRewardFrame",
                "mid/Root/Card1/CardItem0",
				"点击卡牌翻开奖励~",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
                new Vector3(),
                eNewbieGuideAgrsName.SaveBoot
            ));

//			AddContent(new ComNewbieData(
//				NewbieGuideComType.INTRODUCTION,
//				null,
//				"DungeonRewardFrame",
//                "mid/Root/Card3",
//				"黄金卡牌,还未开放,开启后可以再领一次奖励",
//				ComNewbieGuideBase.eNewbieGuideAnchor.Top,
//				TextTipType.TextTipType_Two,
//				new Vector3(0, 0, 0)       
//			));
        }

		public override void InitCondition()
		{
            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.Level,
                new int[] { 1 }
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.SceneBattle
            ));

//             AddCondition(new NewbieConditionData(
//                 eNewbieGuideCondition.SpecificEvent,
//                 new int[] { (int)EUIEventID.BattleResultFrameClose }
//             ));
        }
	}
}

