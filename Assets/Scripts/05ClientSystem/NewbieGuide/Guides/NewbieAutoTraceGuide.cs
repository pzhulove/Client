using UnityEngine;
using ProtoTable;

namespace GameClient
{
	public class NewbieAutoTraceGuide : NewbieGuideDataUnit
    {
		public NewbieAutoTraceGuide(int tid):base(tid){}

		public override void InitContent()
		{
			AddContent(new ComNewbieData(
				NewbieGuideComType.OPEN_EYES,
				null
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.TALK_DIALOG,
				null,
				13010
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"FunctionFrame",
				"ScrollView/ViewPort/Content/Prefab(Clone)",
				"点击<color=#ff0000ff>任务追踪</color>可以自动前往任务地点",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_One,
				new Vector3(0,-200,0),
				eNewbieGuideAgrsName.SaveBoot
			));

//             AddContent(new ComNewbieData(
//                 NewbieGuideComType.COVER,
//                 null,
//                 EUIEventID.EndNewbieGuideCover
//             ));
        }

		public override void InitCondition()
		{
            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.Level,
                new int[] { 1 }
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.SceneTown
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.ClientSystemTownFrameOpen
            ));
        }
	}

    public class NewbieAutoTraceGuide2 : NewbieGuideDataUnit
    {
        public NewbieAutoTraceGuide2(int tid) : base(tid) { }

        public override void InitContent()
        {
            AddContent(new ComNewbieData(
                NewbieGuideComType.BUTTON,
                null,
                "FunctionFrame",
                "ScrollView/ViewPort/Content/Prefab(Clone)",
				"点击任务按钮，开始冒险历程",
                ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
                TextTipType.TextTipType_One,
                new Vector3(0, -200, 0),
                eNewbieGuideAgrsName.SaveBoot
            ));
        }

        public override void InitCondition()
        {
            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.Level,
                new int[] { 1 }
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

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.NewbieGuideID,
                new int[] { (int)NewbieGuideTable.eNewbieGuideTask.EquipmentGuide }
            ));
        }
    }
}

