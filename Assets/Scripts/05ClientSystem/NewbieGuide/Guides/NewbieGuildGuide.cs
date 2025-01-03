using UnityEngine;

namespace GameClient
{
	public class NewbieGuildGuide : NewbieGuideDataUnit
    {
		public NewbieGuildGuide(int tid):base(tid){}

		public override void InitContent()
		{
            AddContent(new ComNewbieData(
                NewbieGuideComType.BUTTON,
                null,
                "ClientSystemTownFrame",
                MainUIIconPath.guild,
                "加入公会，找到组织，可以使你变的更强！",
                ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
                TextTipType.TextTipType_Three,
                new Vector3(0,0,0),
                eNewbieGuideAgrsName.SaveBoot
            ));

			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION2,
				null,
				"公会可以让你找到伙伴，一\n起在游戏中体验关卡，切磋\n技术，分享经验！"
			));

//            AddContent(new ComNewbieData(
//                NewbieGuideComType.BUTTON,
//                null,
//                "GuildListFrame",
//                "JoinGuild/Join",
//                "申请加入一个公会吧，和小伙伴们一起玩耍~",
//                ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
//                TextTipType.TextTipType_Two
//            ));
        }

		public override void InitCondition()
		{
            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.Level,
                new int[] { 17 }
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.FinishedMissionID,
                new int[] { 2293 }
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

