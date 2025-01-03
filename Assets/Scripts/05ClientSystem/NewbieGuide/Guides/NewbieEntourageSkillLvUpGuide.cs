using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieEntourageSkillLvUpGuide : NewbieGuideDataUnit
	{
		public NewbieEntourageSkillLvUpGuide(int tid):base(tid){}

		public override void InitContent()
		{
            List<NewbieModifyData> ModifyDataTypeList = new List<NewbieModifyData>();
            NewbieModifyData data = new NewbieModifyData();
            data.iIndex = 1;
            data.ModifyDataType = NewBieModifyDataType.EntourageID;
            ModifyDataTypeList.Add(data);

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ClientSystemTownFrame",
				MainUIIconPath.point2,
				"随从也可以<color=#ff0000ff>升级</color>喔！",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
				TextTipType.TextTipType_Three,
                new Vector3(0,0,0),
                eNewbieGuideAgrsName.SaveBoot
            ));

// 			AddContent(new ComNewbieData(
// 				NewbieGuideComType.BUTTON,
// 				null,
// 				"ClientSystemTownFrame",
// 				MainUIIconPath.retinue,
// 				"",
// 				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
// 				TextTipType.TextTipType_Two
// 			));


			AddContent(new ComNewbieData(
				NewbieGuideComType.TOGGLE,
				null,
				"RetinueFrame",
				"Tabs/Tab1",
				"选择随从列表",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(0, 0, 0 )
			));

            AddContent(new ComNewbieData(
                NewbieGuideComType.TOGGLE,
                null,
                "RetinueFrame",
                "RetinueInfo/RetinueTable/ViewPort/Content/1000/Toggle",
                "选择要升级的随从",
                ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
                TextTipType.TextTipType_Two,
				new Vector3(-300, 0, 0),
				eNewbieGuideAgrsName.None,
				eNewbieGuideAgrsName.None,
				"RetinueInfo/RetinueTable/ViewPort/Content/1000"

            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.WAIT,
                null,
                0.30f
            ));
    
            AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
                "RetinueFrame",
				"RetinueBodyFrame/BodyUnlocked/MainSkill/BtnUpGrade",
				"点击升级",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(-400, 0, 0 )
			));
 
            AddContent(new ComNewbieData(
                NewbieGuideComType.WAIT,
                null,
                0.50f
            ));

            AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
                "RetinueUpFrame",
                "ButtonUpGrade",
				"升级随从技能需要消耗<color=#ff0000ff>勇者之魂</color>哦,点击升级吧",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(-300, 50, 0 )
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION2,
				null,
				"如果不满意随从的加成效\n果，还可以随时进行<color=#ff0000ff>洗练</color>~"
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"RetinueUpFrame",
				"Close",
				"",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(-300, 50, 0 )
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"RetinueFrame",
				"ComWnd/Title/Close",
				"",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two
			));

		}

		public override void InitCondition()
		{
			AddCondition(new NewbieConditionData(
				eNewbieGuideCondition.Level,
				new int[] { 12 }
			));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.FinishedMissionID,
                new int[] { 2211 }
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

