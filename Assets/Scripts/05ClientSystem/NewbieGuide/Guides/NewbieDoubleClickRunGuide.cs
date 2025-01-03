using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieDoubleClickRunGuide : NewbieGuideDataUnit
	{
		public NewbieDoubleClickRunGuide(int tid):base(tid){}

		public override void InitContent()
		{
            AddContent(new ComNewbieData(
                NewbieGuideComType.NEWICON_UNLOCK,
                null,
                "UIFlatten/Prefabs/NewbieGuide/NewbieGuideDoubleMove",
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
                new int[] { 10 }
            ));

            AddCondition(new NewbieConditionData(
				eNewbieGuideCondition.SceneBattle
            ));

            AddCondition(
                NewbieConditionData.NewUserCondition(
                    ()=>{return Global.Settings.hasDoubleRun == true;}
                )
            );

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

