using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieEntourageSkillGuide : NewbieGuideDataUnit
	{
		public NewbieEntourageSkillGuide(int tid):base(tid){}

		public override void InitContent()
		{
            AddContent(new ComNewbieData(
                NewbieGuideComType.NEWICON_UNLOCK,
                null,
                "UIFlatten/Prefabs/NewbieGuide/NewbieGuideAddSkill2",
                "",
                "",
                3.0f,
                eNewbieGuideAgrsName.SaveBoot,
                eNewbieGuideAgrsName.PauseBattle
            ));
        }

		public override void InitCondition()
		{
			AddCondition(new NewbieConditionData(
				eNewbieGuideCondition.Level,
				new int[] { 11 }
			));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.SceneBattle
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.BattleInitFinished
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.SpecificEvent,
                new int[] { (int)EUIEventID.DungeonOnFight }
            ));
        }
	}
}

