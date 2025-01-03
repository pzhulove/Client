using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieAutoFightGuide : NewbieGuideDataUnit
	{
		public NewbieAutoFightGuide(int tid):base(tid){}

		public override void InitContent()
		{
            AddContent(new ComNewbieData(
                NewbieGuideComType.NEWICON_UNLOCK,
                null,
                "UIFlatten/Prefabs/NewbieGuide/NewbieGuideAddAutofight",
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
                new int[] { 12 }
            ));

            AddCondition(new NewbieConditionData(
				eNewbieGuideCondition.SceneBattle
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.BattleInitFinished
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.IsDungeon
            ));
        }
	}
}

