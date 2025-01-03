using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieJumpBackGuide : NewbieGuideDataUnit
	{
		public NewbieJumpBackGuide(int tid):base(tid){}

		public override void InitContent()
		{
            AddContent(new ComNewbieData(
                NewbieGuideComType.NEWICON_UNLOCK,
                null,
                "UIFlatten/Prefabs/NewbieGuide/NewbieGuideAddJump",
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
                new int[] { 1 }
            ));

            AddCondition(new NewbieConditionData(
				eNewbieGuideCondition.SceneBattle
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.DungeonID,
                new int[] { 102010 }
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

