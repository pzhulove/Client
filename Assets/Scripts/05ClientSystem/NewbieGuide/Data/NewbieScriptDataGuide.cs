using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieScriptDataGuide : NewbieGuideDataUnit
	{
		public NewbieScriptDataGuide(int tid):base(tid){}
        DNewbieGuideData scriptData;
     
        public void LoadScriptData(DNewbieGuideData data)
        {
            scriptData = data;
        }
		public override void InitContent()
		{
			if(scriptData == null)
			{
				return;
			}

            for(int i = 0; i < scriptData.UnitData.Length; ++i)
			{
				var cur = scriptData.UnitData[i];
				List<NewbieModifyData> mtl = new List<NewbieModifyData>();

				if(cur.modifyData != null && cur.modifyData.Length > 0)
				{
					for(int k = 0; k < cur.modifyData.Length; ++k)
					{
						NewbieModifyData md = new NewbieModifyData();

            			md.iIndex = cur.modifyData[k].iIndex;
            			md.ModifyDataType = cur.modifyData[k].ModifyDataType;

						mtl.Add(md);
					}
				}

				var IData = cur.GetData();
				AddContent(new ComNewbieData(
					cur.Type,
					mtl,
					IData == null ? null : IData.getArgs()
				));
			}
		}

		public override void InitCondition()
		{
			if(scriptData == null)
			{
				return;
			}

			for(int i = 0; i < scriptData.ConditionData.Length; ++i)
			{
				var cur = scriptData.ConditionData[i];

				AddCondition(new NewbieConditionData(cur.condition, cur.LimitArgsList, cur.LimitFramesList));
			}
        }
	}
}

