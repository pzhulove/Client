using UnityEngine;
using System.Collections;

namespace GameClient
{
    class MissionLinkTest : MonoBehaviour
    {
        public int MissionID = 0;
        public void OnClickToLink()
        {
            var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(MissionID);
            if(missionItem != null)
            {
                Logger.LogErrorFormat("link [{0}][{1}]", missionItem.ID, missionItem.TaskName);
                ActiveManager.GetInstance().OnClickLinkInfo(missionItem.LinkInfo);
            }
        }
    }
}