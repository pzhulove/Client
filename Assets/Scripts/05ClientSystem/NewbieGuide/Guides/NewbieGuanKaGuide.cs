using UnityEngine;

namespace GameClient
{
	public class NewbieGuanKaGuide : NewbieGuideDataUnit
    {
		public NewbieGuanKaGuide(int tid):base(tid){}


		public override void InitContent()
		{
            AddContent(new ComNewbieData(
                NewbieGuideComType.WAIT,
                null,
                0.4f
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.INTRODUCTION2,
                null,
                "这里就是<color=#ff0000ff>地下城</color>的入口，包括难\n度选择，地图信息，可能掉落，\n增益药水等信息",
                eNewbieGuideAgrsName.SaveBoot
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.BUTTON,
                null,
                "ChapterNormalFrame",
                "FKRoot/AllRoot/D/R/Start/Start/GroupStart",
                "马上开始挑战地下城",
                ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
                TextTipType.TextTipType_Two,
                new Vector3(-420, 50, 0)
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
                eNewbieGuideCondition.
                ChapterChooseFrameOpen
            ));
        }
	}
}

