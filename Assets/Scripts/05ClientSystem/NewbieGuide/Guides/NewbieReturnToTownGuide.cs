using UnityEngine;

namespace GameClient
{
	public class NewbieReturnToTownGuide : NewbieGuideDataUnit
    {
		public NewbieReturnToTownGuide(int tid):base(tid){}

		public override void InitContent()
		{
            AddContent(new ComNewbieData(
                NewbieGuideComType.BUTTON,
                null,
                "DungeonMenuFrame",
                "ButtonRoot/Root/MissionRoot",
                "点击回到主城",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
                TextTipType.TextTipType_Two,
                new Vector3(-300,-300,0),
                eNewbieGuideAgrsName.SaveBoot,
                eNewbieGuideAgrsName.None
            ));
        }

		public override void InitCondition()
		{
            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.Level,
                new int[] { 1 }
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.SceneBattle
            ));

//             AddCondition(new NewbieConditionData(
//                 eNewbieGuideCondition.SpecificEvent,
//                 new int[] { (int)EUIEventID.DungeonRewardFrameClose }
//             ));
        }
	}
}

