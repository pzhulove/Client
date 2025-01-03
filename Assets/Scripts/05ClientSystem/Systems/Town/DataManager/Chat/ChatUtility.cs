using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoTable;

namespace GameClient
{
    //挑战助手类
    public static class ChatUtility
    {
        //得到聊天的数据
        public static ChatData GetChatDataBySendGuid(ulong sendGuid)
        {
            var globalChatBlockList = ChatManager.GetInstance().GlobalChatBlock;
            if (globalChatBlockList == null || globalChatBlockList.Count <= 0)
                return null;

            for (var i = 0; i < globalChatBlockList.Count; i++)
            {
                var chatBlock = globalChatBlockList[i];
                if(chatBlock == null || chatBlock.chatData == null)
                    continue;

                //找到聊天的数据
                if (chatBlock.chatData.objid == sendGuid)
                    return chatBlock.chatData;
            }

            return null;
        }

        //判断聊天是否为跨服
        public static bool IsChatDataFromDifferentServer(ChatData chatData)
        {
            if (chatData == null)
                return false;

            //团本相关的聊天为跨服聊天.
            if (chatData.eChatType == ChatType.CT_TEAM_COPY_PREPARE
                || chatData.eChatType == ChatType.CT_TEAM_COPY_TEAM
                || chatData.eChatType == ChatType.CT_TEAM_COPY_SQUAD)
                return true;

            return false;
        }

        //是否禁止私聊
        public static bool IsForbidPrivateChat()
        {
            //在团本场景中，禁止私聊
            if (TeamDuplicationUtility.IsInTeamDuplicationScene() == true)
                return true;

            return false;
        }

    }
}
