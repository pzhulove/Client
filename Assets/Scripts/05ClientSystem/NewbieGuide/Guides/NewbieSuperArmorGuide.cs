using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieSuperArmorGuide : NewbieGuideDataUnit
	{
		public NewbieSuperArmorGuide(int tid):base(tid){}

		public override void InitContent()
		{
            AddContent(new ComNewbieData(
                NewbieGuideComType.SHOW_IMAGE,
                null,
                "UI/Image/NewbieGuide/NewbieGuide_3.png:NewbieGuide_3",
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
                new int[] { 102000 }
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.BattleInitFinished
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.SpecificEvent,
                new int[] { (int)EUIEventID.DungeonOnFight }
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.DungeonAreaID,
                new int[] { 16 }
            ));
        }
	}
}

