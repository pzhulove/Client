using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieAchievementGuide : NewbieGuideDataUnit
	{
		public NewbieAchievementGuide(int tid):base(tid){}

		public override void InitContent()
		{
            List<NewbieModifyData> ModifyDataTypeList = new List<NewbieModifyData>();
            NewbieModifyData data = new NewbieModifyData();
            data.iIndex = 1;
            data.ModifyDataType = NewBieModifyDataType.AchievementPos;
            ModifyDataTypeList.Add(data);

            AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ClientSystemTownFrame",
				MainUIIconPath.mission,
				"<color=#ff0000ff>成就</color>达成！赶快来领取成就奖励吧",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
				TextTipType.TextTipType_Three,
				new Vector3(0,0,0),
                eNewbieGuideAgrsName.SaveBoot            
            ));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ClientSystemTownFrame",
				MainUIIconPath.mission,
				"",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two           
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.TOGGLE,
				null,
				"MissionFrameNew",
				"Content/MissionFrame(Clone)/VerticalFilter/FFT_ACHIEVEMENT",
				"选择成就",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two
			));

            AddContent(new ComNewbieData(
                NewbieGuideComType.WAIT,
                null,
                0.2f
            ));

            AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
                ModifyDataTypeList,
				"MissionFrameNew",
				"Content/MissionFrame(Clone)/AchievementContent/ScrollView/ViewPort/Content/{0}/BtnAward",
				"点击领取奖励",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(-150 , 0, 0)
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION2,
				null,
				"多多努力吧！\n更多更好的<color=#ff0000ff>成就奖励</color>还在后面呢！"
			));

//			AddContent(new ComNewbieData(
//				NewbieGuideComType.BUTTON,
//				ModifyDataTypeList,
//				"MissionFrameNew",
//				"Title/Close",
//				"",
//				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
//				TextTipType.TextTipType_Two,
//				new Vector3(0 , 0, 0)
//			));
		}

		public override void InitCondition()
		{
            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.Level,
                new int[] { 6 }
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.FinishedMissionID,
                new int[] { 2204 }
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

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.AchievementFinished
            ));
        }
	}
}

