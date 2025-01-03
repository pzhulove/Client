using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieBufferSkillGuide : NewbieGuideDataUnit
	{
		public NewbieBufferSkillGuide(int tid):base(tid){}

		public override void InitContent()
		{
            List<NewbieModifyData> ModifyDataTypeList = new List<NewbieModifyData>();

            NewbieModifyData data = new NewbieModifyData();
            data.iIndex = 1;
            data.ModifyDataType = NewBieModifyDataType.IconPath;

            NewbieModifyData data2 = new NewbieModifyData();
            data2.iIndex = 2;
            data2.ModifyDataType = NewBieModifyDataType.IconName;

            ModifyDataTypeList.Add(data);
            ModifyDataTypeList.Add(data2);

            AddContent(new ComNewbieData(
                NewbieGuideComType.NEWICON_UNLOCK,
                ModifyDataTypeList,
                "UIFlatten/Prefabs/NewbieGuide/NewbieGuideAddBuff",
                "{0}",
                "{0}",
                3.0f,
                eNewbieGuideAgrsName.SaveBoot,
                eNewbieGuideAgrsName.PauseBattle
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.WAIT,
                null,
                0.1f,
                false,
                eNewbieGuideAgrsName.None,
                eNewbieGuideAgrsName.None,
                eNewbieGuideAgrsName.ResumeBattle
            ));
        }

		public override void InitCondition()
		{
            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.Level,
                new int[] { 15 }
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.ChangedJob
            ));

            AddCondition(new NewbieConditionData(
				eNewbieGuideCondition.SceneBattle
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.BattleInitFinished
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.LearnBufferSkill
            ));
        }
	}
}

