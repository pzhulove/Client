using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieWelfareGuide : NewbieGuideDataUnit
	{
		public NewbieWelfareGuide(int tid):base(tid){}

		public override void InitContent()
		{
            List<NewbieModifyData> ModifyDataTypeList = new List<NewbieModifyData>();

            NewbieModifyData data = new NewbieModifyData();
            data.iIndex = 0;
            data.ModifyDataType = NewBieModifyDataType.WelfareID;

            NewbieModifyData data2 = new NewbieModifyData();
            data2.iIndex = 1;
            data2.ModifyDataType = NewBieModifyDataType.SignInID;
            
            ModifyDataTypeList.Add(data);
            ModifyDataTypeList.Add(data2);

            AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ClientSystemTownFrame",
				MainUIIconPath.activeSevenDays,
				"七日活动解锁啦，一起来看看~",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Three,
				new Vector3(-600, -300,0)
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.WAIT,
                null,
                0.1f
            ));
				
            AddContent(new ComNewbieData(
                NewbieGuideComType.BUTTON,
                null,
				"ActiveChargeFrame9388",
				"Activities/FirstDayActive7100/Activities/DailyFuLi/ViewPort/Content/7101/UnAcquired",
                "点击领取奖励",
                ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
                TextTipType.TextTipType_Two,
                new Vector3(-200,35,0)
//                eNewbieGuideAgrsName.None,
//                eNewbieGuideAgrsName.None,
//				"Activities/FirstDayActive7100/Activities/DailyFuLi/ViewPort/Content/7101"
            ));

			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION2,
				null,
				"精彩活动等你来领，心动不如行动，\n看到小红点记得进来领取哦"
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ActiveChargeFrame9388",
				"Shopbg/tittlebg1/close",
				"",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight
			));
		}

		public override void InitCondition()
		{
			AddCondition(new NewbieConditionData(
				eNewbieGuideCondition.Level,
				new int[] { 8 }
			));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.FinishedMissionID,
                new int[] { 2206 }
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.SceneTown
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.
                ClientSystemTownFrameOpen
            ));

//             AddCondition(new NewbieConditionData(
//                 eNewbieGuideCondition.MainFrameMutex
//             ));

//             AddCondition(new NewbieConditionData(
//                 eNewbieGuideCondition.SignInFinished
//             ));
        }
	}
}

