using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieSkillGuide : NewbieGuideDataUnit
    {
		public NewbieSkillGuide(int tid):base(tid){}

		public override void InitContent()
		{
            List<NewbieModifyData> ModifyDataTypeList = new List<NewbieModifyData>();
            NewbieModifyData data = new NewbieModifyData();
            data.iIndex = 1;
            data.ModifyDataType = NewBieModifyDataType.JobID;
            ModifyDataTypeList.Add(data);

            AddContent(new ComNewbieData(
				NewbieGuideComType.TALK_DIALOG,
				null,
                22050
            ));

//             AddContent(new ComNewbieData(
//                 NewbieGuideComType.BUTTON,
//                 null,
//                 "FunctionFrame",
//                 "ScrollView/ViewPort/Content/Prefab(Clone)",
//                 "接取任务",
//                 ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
//                 TextTipType.TextTipType_One,
//                 new Vector3(0, -200, 0)
//             ));

    //        AddContent(new ComNewbieData(
				//NewbieGuideComType.BUTTON,
				//null,
				//"ClientSystemTownFrame",
				//MainUIIconPath.point2,
				//"冒险家大人！铁匠大叔向\n你传授了<color=#fcff21>新技能</color>呢！",
				//ComNewbieGuideBase.eNewbieGuideAnchor.ButtomLeft,
				//TextTipType.TextTipType_Three,
    //            new Vector3(0,0,0),
    //            eNewbieGuideAgrsName.SaveBoot
    //        ));

            //AddContent(new ComNewbieData(
            //    NewbieGuideComType.WAIT,
            //    null,
            //    0.5f
            //));

            AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ClientSystemTownFrame",
				MainUIIconPath.skill,
                "冒险家大人！铁匠大叔向\n你传授了<color=#fcff21>新技能</color>呢！",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
				TextTipType.TextTipType_Three
            ));

//			AddContent(new ComNewbieData(
//				NewbieGuideComType.INTRODUCTION,
//				ModifyDataTypeList,
//				"SkillTreeFrame",
//				"left/root/{0}(Clone)",
//				"左侧是技能列表，可升级的技能会有箭头显示~",
//				ComNewbieGuideBase.eNewbieGuideAnchor.Right,
//				TextTipType.TextTipType_Two
//			));
//
//			AddContent(new ComNewbieData(
//				NewbieGuideComType.INTRODUCTION,
//				ModifyDataTypeList,
//				"SkillTreeFrame",
//				"right",
//				"右侧为技能升级面板",
//				ComNewbieGuideBase.eNewbieGuideAnchor.Left,
//				TextTipType.TextTipType_Two
//			));

//            AddContent(new ComNewbieData(
//                NewbieGuideComType.TOGGLE,
//				null,
//                "SkillTreeFrame",
//                "left/root/JobSkillTree(Clone)/ScrollView/Viewport/Content/ActiveSkillPanel1/Pos8/SkillTreeElement(Clone)/icon",
//				"选择<color=#ff0000ff>技能</color>",
//				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
//				TextTipType.TextTipType_Two,
//				new Vector3(50, -100, 0)
//            ));
//
//            AddContent(new ComNewbieData(
//				NewbieGuideComType.BUTTON,
//                null,
//                "SkillTreeFrame",
//                "right/down/btLvUp",
//				"升级技能需要消耗<color=#ff0000ff>技能点</color>，点击这里升级吧",
//				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
//				TextTipType.TextTipType_Two,
//				new Vector3(-750, 30, 0)
//
//			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"SkillFrame",
                "SkillInfo/BtnSet",
				"学习新技能后一定要记得<color=#fcff21>进行配置</color>~",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
				TextTipType.TextTipType_Two,
				new Vector3(-550, 30, 0)

			));

			AddContent(ComNewbieData.New<ComNewbieGuideDragSkill>(
				null,
				"SkillFrame",
                "SkillTreeRoot/SkillTree_12(Clone)/Viewport/Content/SkillUnit (4)",
				"请拖动技能进行配置",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(0, 30, 0),
                "SkillConfigurationFrame",
                "bg/root/SkillSlotUnit (3)",
				"将学习过的新技能拖拽进快捷技能栏",
				ComNewbieGuideBase.eNewbieGuideAnchor.ButtomLeft,
				TextTipType.TextTipType_Two,
				new Vector3(-200, 30, 0)
			));

			AddContent(ComNewbieData.New<ComNewbieGuideDragSkill>(
				null,
                "SkillFrame",
                "SkillTreeRoot/SkillTree_12(Clone)/Viewport/Content/SkillUnit (7)",
				"请再次尝试配置新技能",
				ComNewbieGuideBase.eNewbieGuideAnchor.ButtomLeft,
				TextTipType.TextTipType_Two,
				new Vector3(0, 30, 0),
                "SkillConfigurationFrame",
                "bg/root/SkillSlotUnit",
				"将第二个新技能配置在快捷栏中",
				ComNewbieGuideBase.eNewbieGuideAnchor.ButtomLeft,
				TextTipType.TextTipType_Two,
				new Vector3(-200, 30, 0)
			));

//             AddContent(new ComNewbieData(
//                 NewbieGuideComType.INTRODUCTION,
//                 null,
//                 "SkillPlanFrame",
//                 "root/BarPos4/SkillBarElement(Clone)/Skill/Icon",
//                 "冒险家，这里是转职后的职业技能,你可以在15级转职后进行自由配置",
//                 ComNewbieGuideBase.eNewbieGuideAnchor.ButtomLeft,
//                 TextTipType.TextTipType_Two,
//                 new Vector3(-200, 0, 0)
//             ));

            /* 
			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"SkillPlanFrame",
				"root/BarPos4",
				"SkillPlaneList",
				"rootlist/Pos5",
				"把新学的技能拖到这里",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(0, 30, 0)

			));
			*/
            AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION2,
				null,
				"学习新技能后，请一定要\n记得进行<color=#fcff21>技能配置</color>喔~"
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
                "SkillFrame",
                "PopWindowTitle/Title/Close",
				"",
				ComNewbieGuideBase.eNewbieGuideAnchor.ButtomLeft,
				TextTipType.TextTipType_Two
			));
				
//            AddContent(new ComNewbieData(
//				NewbieGuideComType.TOGGLE,
//                null,
//                "SkillTreeFrame",
//                "right/Func/FuncTab2",
//                "新学习的技能会自动配置哦，键位可以在这里调整~",
//				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
//				TextTipType.TextTipType_Two
//
//            ));

//            AddContent(new ComNewbieData(
//                NewbieGuideComType.TOGGLE,
//                ModifyDataTypeList,
//                "SkillTreeFrame",
//                "left/root/{0}(Clone)/ScrollView/Viewport/Content/Pos3/SkillTreeElement(Clone)/icon",
//                "再次选中技能",
//                ComNewbieGuideBase.eNewbieGuideAnchor.Right
//            ));

//            AddContent(new ComNewbieData(
//                NewbieGuideComType.TOGGLE,
//                null,
//                "SkillPlanFrame",
//                "root/BarPos3/SkillBarElement(Clone)/Skill",
//                "新解锁的技能会自动配置哦",
//				ComNewbieGuideBase.eNewbieGuideAnchor.ButtomLeft,
//				TextTipType.TextTipType_Two,
//				new Vector3(-30, 100, 0)
//            ));
//
//            AddContent(new ComNewbieData(
//				NewbieGuideComType.INTRODUCTION,
//                null,
//                "SkillTreeFrame",
//                "left/root/10(Clone)/ScrollView/Viewport/Content/Pos5",
//				"新学习的技能会自动配置哦，让我们继续旅程吧",
//                ComNewbieGuideBase.eNewbieGuideAnchor.ButtomLeft,
//				TextTipType.TextTipType_Two,
//				new Vector3(-50, 80, 0)
//            ));

			AddContent(new ComNewbieData(
				NewbieGuideComType.TALK_DIALOG,
				null,
				23020
			));
        }

		public override void InitCondition()
		{
            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.Level,
                new int[] { 5 }
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.FinishedMissionID,
                new int[] { 2203 }
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

