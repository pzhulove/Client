using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;

namespace GameClient
{
    //商店的助手类
    public static class RelationUtility
    {
        //判断是否为好友关系
        public static bool IsRelationFriend(ulong guid)
        {

            var relationData = RelationDataManager.GetInstance().GetRelationByRoleID(guid);
            //关系不存在
            if (relationData == null)
                return false;

            //关系不为好友
            if (relationData.type != (int) RelationType.RELATION_FRIEND)
                return false;

            return true;
        }

        //判断是否为同一工会
        public static bool IsRelationSameGuild(ulong guildId)
        {
            //主角工会不存在
            if (GuildDataManager.GetInstance().myGuild == null)
                return false;
            //两者工会ID不一致
            if (GuildDataManager.GetInstance().myGuild.uGUID != guildId)
                return false;

            return true;
        }


    }
}
