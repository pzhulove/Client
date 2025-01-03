using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieSkillLevelUpGuide : NewbieGuideDataUnit
    {
		public NewbieSkillLevelUpGuide(int tid):base(tid){}

		public override void InitContent()
		{
            List<NewbieModifyData> ModifyDataTypeList = new List<NewbieModifyData>();
            NewbieModifyData data = new NewbieModifyData();
            data.iIndex = 1;
            data.ModifyDataType = NewBieModifyDataType.JobID;
            ModifyDataTypeList.Add(data);

            AddContent(new ComNewbieData(
                NewbieGuideComType.BUTTON,
                null,
                "ClientSystemTownFrame",
				MainUIIconPath.point2,
                "可以进行技能升级啦！",
                ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
                TextTipType.TextTipType_Three,
                new Vector3(0, 0, 0),
                eNewbieGuideAgrsName.SaveBoot
            ));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ClientSystemTownFrame",
				MainUIIconPath.skill,
				"",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two
			));


            AddContent(new ComNewbieData(
                NewbieGuideComType.TOGGLE,
                ModifyDataTypeList,
                "SkillTreeFrame",
				"left/root/JobSkillTree(Clone)/ScrollView/Viewport/Content/ActiveSkillPanel2/Pos10/SkillTreeElement(Clone)/icon",
                "选择技能",
                ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
                TextTipType.TextTipType_Two,
                new Vector3(50, -100, 0)
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.BUTTON,
                null,
                "SkillTreeFrame",
                "right/down/btLvUp",
				"升级技能需要消耗<color=#ff0000ff>技能点</color>，点击这里升级吧",
                ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
                TextTipType.TextTipType_Two,
                new Vector3(-450, 60, 0)
            ));

// 			AddContent(new ComNewbieData(
// 				NewbieGuideComType.INTRODUCTION,
// 				null,
// 				"SkillTreeFrame",
// 				"right/down/btLvDown",
// 				"如果点错技能了，不用着急，点击降级就可以（<color=#ff0000ff>无消耗</color>）",
// 				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
// 				TextTipType.TextTipType_Two,
// 				new Vector3(-600, 60, 0)
// 			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION2,
				null,
				"技能升级是提升<color=#ff0000ff>战斗能力</color>的重要手\n段，看到红点记得进行技能升级，丰\n富多样的技能可以自由搭配，千万\n不要忘记配置哦~!"
			));




//            AddContent(new ComNewbieData(
//                NewbieGuideComType.TOGGLE,
//                null,
//                "SkillTreeFrame",
//                "right/Func/FuncTab2",
//                "新学习的技能会自动配置哦，键位可以在这里调整~",
//                ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
//                TextTipType.TextTipType_Two
//            ));
//
//            AddContent(new ComNewbieData(
//                NewbieGuideComType.INTRODUCTION,
//                null,
//                "SkillPlanFrame",
//                "",
//                "你也可以切换键位方案，调整自己喜欢的技能位置",
//                ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
//                TextTipType.TextTipType_Two
//            ));
        }

		public override void InitCondition()
		{
            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.Level,
                new int[] { 7 }
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

