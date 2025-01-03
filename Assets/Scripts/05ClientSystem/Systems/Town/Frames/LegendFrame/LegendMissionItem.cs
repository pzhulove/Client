using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace GameClient
{
    class LegendMissionItemData
    {
        public int LegendId = -1;   //传奇之路对应的Id
        public MissionManager.SingleMissionInfo missionValue;
    }

    class LegendMissionItem : CachedNormalObject<LegendMissionItemData>
    {
        public ComLegendLinkMissionItem comLegendLinkMissionItem;

        public override void Initialize()
        {
            comLegendLinkMissionItem = Utility.FindComponent<ComLegendLinkMissionItem>(goLocal, "");
        }

        public override void UnInitialize()
        {
            comLegendLinkMissionItem = null;
        }

        public void SetSiblingIndex(int iIndex)
        {
            if(null != goLocal)
            {
                goLocal.transform.SetSiblingIndex(iIndex);
            }
        }

        public override void OnUpdate()
        {
            if(null != Value && null != Value.missionValue)
            {
                comLegendLinkMissionItem.SetMissionData(Value.missionValue, Value.LegendId);
            }
        }
    }
}