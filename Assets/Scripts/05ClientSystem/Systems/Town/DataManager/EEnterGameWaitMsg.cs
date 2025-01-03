using System;
using System.Collections.Generic;
///////É¾³ýlinq
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Runtime.InteropServices;
using Protocol;
using Network;

namespace GameClient
{
    public static class EEnterGameWaitMsg
    {
        public static readonly UInt32[] msgs = new UInt32[]
        {
            SceneDungeonSyncNewOpenedList.MsgID,
            SceneDungeonInit.MsgID,
            SceneDungeonHardInit.MsgID,
            WorldSyncTeamInfo.MsgID,
            WorldGuildSyncInfo.MsgID,
            WorldSyncRedPacket.MsgID,
            SceneSynItem.MsgID,
            SceneSyncActiveTaskList.MsgID,
            SceneSyncClientActivities.MsgID,
            SceneDailyTaskList.MsgID,
            SceneAchievementTaskList.MsgID,
            SceneTaskListRet.MsgID,
            SceneSyncRetinueList.MsgID,
            SceneSyncSeasonInfo.MsgID,
            SceneInitNotifyList.MsgID,
        };
    }

}