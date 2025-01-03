using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieDailyTaskGuide : NewbieGuideDataUnit
	{
		public NewbieDailyTaskGuide(int tid):base(tid){}

		public override void InitContent()
		{
            List<NewbieModifyData> ModifyDataTypeList = new List<NewbieModifyData>();
            NewbieModifyData data = new NewbieModifyData();
            data.iIndex = 1;
            data.ModifyDataType = NewBieModifyDataType.DailyMissionPos;
            ModifyDataTypeList.Add(data);

            AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ClientSystemTownFrame",
				MainUIIconPath.Activitys,
				"可以进行<color=#ff0000ff>每日任务</color>了，点击打开活动界面吧",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
				TextTipType.TextTipType_Three,
                new Vector3(0,0,0),
                eNewbieGuideAgrsName.SaveBoot
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.INTRODUCTION2,
                null,
                "这里包含了游戏内所有的活动，\n包括<color=#ff0000ff>特殊地下城</color>，决斗活动等，\n每天记得都来看一看哦"
            ));

            //			AddContent(new ComNewbieData(
            //				NewbieGuideComType.INTRODUCTION2,
            //				null,
            //				"所有任务都会在这里，包括主线，支线，每日，成就等，看到红点记得进来看看哦~"
            //			));

            AddContent(new ComNewbieData(
				NewbieGuideComType.TOGGLE,
				null,
                "ActivityDungeonFrame",
                "L/Reward",
				"选择日常任务切页",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two
			));
//
//            AddContent(new ComNewbieData(
//                NewbieGuideComType.WAIT,
//                null,
//                0.6f
//            ));
//
//            AddContent(new ComNewbieData(
//                NewbieGuideComType.BUTTON,
//                ModifyDataTypeList,
//                "MissionFrameNew",
//                "DailyContent/ScrollView/ViewPort/Content/{0}/BtnAward",
//				"领取每日任务奖励，注意每日任务每天都会刷新哦！",
//				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
//                TextTipType.TextTipType_Two
//            ));

//			AddContent(new ComNewbieData(
//				NewbieGuideComType.INTRODUCTION,
//				null,
//				"MissionFrameNew",
//				"DailyContent/ScoreBar",
//				"完成任务可获得积分，积分到达数额就可领取额外宝箱哦",
//				ComNewbieGuideBase.eNewbieGuideAnchor.Top,
//				TextTipType.TextTipType_Two
//			));


			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION2,
				null,
				"完成任务可获得积分，积分到达一定数\n额就可领取额外宝箱哦\n注意每日任务每天6点刷新哦！\n努力完成每日任务获得任务奖励吧！"
			));
//            AddContent(new ComNewbieData(
//				NewbieGuideComType.INTRODUCTION,
//				null,
//                "MissionFrameNew",
//                "DailyContent/ScrollView/ViewPort",
//				"基本与成就相同，注意每日任务每天6点刷新哦！",
//				ComNewbieGuideBase.eNewbieGuideAnchor.Top,
//				TextTipType.TextTipType_Two
//			));
//
//            AddContent(new ComNewbieData(
//                NewbieGuideComType.BUTTON,
//                null,
//                "MissionFrameNew",
//                "Title/closeicon",
//                "继续挑战关卡吧~",
//				ComNewbieGuideBase.eNewbieGuideAnchor.ButtomLeft,
//                TextTipType.TextTipType_Two,
//				new Vector3(0, 80, 0)
//            ));
        }

		public override void InitCondition()
		{
			AddCondition(new NewbieConditionData(
				eNewbieGuideCondition.Level,
				new int[] { 13 }
			));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.FinishedMissionID,
                new int[] { 2212 }
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

//            AddCondition(new NewbieConditionData(
//                eNewbieGuideCondition.DailyMissionFinished
//            ));
        }
	}
}

