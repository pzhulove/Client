using UnityEngine;

namespace GameClient
{
	public class NewbieRewardBoxGuide : NewbieGuideDataUnit
    {
		public NewbieRewardBoxGuide(int tid):base(tid){}

		public override void InitContent()
		{
//            AddContent(new ComNewbieData(
//                NewbieGuideComType.BUTTON,
//                null,
//                "FunctionFrame",
//                "ScrollView/ViewPort/Content/Prefab(Clone)",
//                "继续主线任务吧",
//                ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
//                TextTipType.TextTipType_One,
//                new Vector3(0, -200, 0),
//                eNewbieGuideAgrsName.SaveBoot
//            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.BUTTON,
                null,
                "ChapterNormalFrame",
                "FKRoot/Top/Title/Close",
                "每通关一个章节后就可以领取该章节的关卡宝箱奖励了",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
                TextTipType.TextTipType_Two,
                new Vector3(0, -250, 0),
                eNewbieGuideAgrsName.SaveBoot
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.BUTTON,
                null,
                "ChapterSelectFrame",
                "Menu/MR/L/Left/B",
                "回退到上一章",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
                TextTipType.TextTipType_Two,
                new Vector3(0, 20, 0)
            ));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ChapterSelectFrame",
				"Menu/Buttom/Reward",
				"打开章节奖励界面",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
				TextTipType.TextTipType_Two,
				new Vector3(-100, 20, 0)
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ChapterRewardFrame",
				"C/Scroll/ViewPort/Content/3500/GR/BtnAward",
				"点击领取奖励",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(0, 20, 0)
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ChapterRewardFrame",
				"C/Scroll/ViewPort/Content/3501/GR/BtnAward",
				"点击领取奖励",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(0, 20, 0)
			));
			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ChapterRewardFrame",
				"C/Middle/SR/Slider/Background/Bx",
				"打开宝箱~",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(0, 20, 0)
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ChapterRewardFrame",
				"Box/Ok/r/AllReward",
				"领取章节奖励吧~",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(0, 20, 0)
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION2,
				null,
				"完成全部相应章节的任务\n就可以领取章节奖励哦~\n继续挑战关卡吧"
			));

        }

		public override void InitCondition()
		{
            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.Level,
                new int[] { 5 }
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.SceneTown
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.
                ChapterChooseFrameOpen
            ));
        }
	}
}

