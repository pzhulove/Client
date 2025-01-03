using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using Protocol;

namespace GameClient
{
    public enum RelationType
    {
        RELATION_NONE = 0,      
        RELATION_FRIEND = 1,        //好友关系
        RELATION_BLACKLIST = 2,     //黑名单
        RELATION_STRANGER = 3,         //陌生人
        RELATION_MASTER = 4,        //师傅
        RELATION_DISCIPLE = 5,		//徒弟

        RELATION_MAX,
    };

    public enum RelationMarkFlag
	{
        RELATION_NONE = 0,
        RELATION_MUTUAL_FRIEND = 1,		//互为好友
	};

public class RelationData
    {
        public byte type;
        public ulong uid;  
        public string name;
        public UInt32 seasonLv;
        public UInt16 level; 
        public byte occu;
        public byte dayGiftNum;
        public byte isOnline;
        public UInt32 createTime;
        public byte vipLv;
        public byte status;
        public string announcement;
        public byte tapDayGiftTimes;
        public UInt32 offlineTime;
        public UInt32 intimacy;
        public string remark;
        public byte isRegress;
        /// <summary>
        /// 双方互为好友标识
        /// </summary>
        public UInt32 mark;
        //新师徒加的
        public PlayerAvatar avatar = new PlayerAvatar();
        public byte activeTimeType;
        public byte masterType;
        public byte regionId;
        public string declaration;
        public byte dailyTaskState;
        public TAPType tapType;
        public PlayerLabelInfo playerLabelInfo = new PlayerLabelInfo();
        /// <summary>
        /// 头像框
        /// </summary>
        public UInt32 headFrame;

        public UInt32 zoneId;       //跨服，服务器Id
        public uint returnYearTitle;//玩家是否穿戴了周年庆称号

        public bool IsFriend()
        {
            return type == (byte)RelationType.RELATION_FRIEND;
		}

        public bool IsMater()
        {
            return type == (byte)RelationType.RELATION_MASTER;
        }

        public bool IsDisciple()
        {
            return type == (byte)RelationType.RELATION_DISCIPLE;
        }
    }

    public class ClassmateRelationData
    {
        public UInt64 uid;
        public string name;
        public UInt16 level;
        public byte occu;
        public byte type;
        public byte vipLv;
        public byte status;
        public byte isFinSch;
    }

    public class InviteFriendData
    {
        //  请求者
        public UInt64 requester;
        //  请求者名字
        public string requesterName;
        //  请求者职业
        public byte requesterOccu;
        //  请求者等级
        public UInt16 requesterLevel;
        //请求者vip等级
        public byte vipLv;
    }

    public class PrivateChatPlayerData
    {
        public RelationData relationData;
        public int chatNum;
        public int iOrder;
    }
}
