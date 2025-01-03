using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using System.Reflection;

namespace Protocol
{
	public enum RelationAttr
    {
        RA_INVALID,
        RA_NAME,        //对方名字(std::string)
        RA_SEASONLV,         //对方段位(UInt32)
        RA_LEVEL,       //对方等级(UInt16)
        RA_OCCU,        //对方职业(UInt8)
        RA_LASTGIVETIME,        //上一次赠送时间(UInt32)
        RA_DAYGIFTNUM,	//每日赠送次数
        RA_TYPE,
        RA_CREATETIME,   //关系建立时间
        RA_VIPLV,         //vip等级
        RA_STATUS,          // 状态
		RA_MASTER_DAYGIFTNUM,
        RA_OFFLINE_TIME,    //离线时间
        RA_INTIMACY,       //好友度
		RA_DAILY_MASTERTASK_STATE, //师门日常任务状态
		RA_REMARKS,	//备注
		RA_IS_REGRESS, //是否是回归玩家
		RA_MARK,	//标志
		RA_HEAD_FRAME,	//头像框
		RA_GUILD_ID,	//公会ID
		RA_RETURN_YEAR_TITLE,	//回归周年称号
    };

    public class Relation : StreamObject
    {
        public UInt64 uid;
        [SProperty((int)RelationAttr.RA_NAME)]
        public string name;
        [SProperty((int)RelationAttr.RA_SEASONLV)]
        public UInt32 seasonLv;       
        [SProperty((int)RelationAttr.RA_LEVEL)]
        public UInt16 level;       
        [SProperty((int)RelationAttr.RA_OCCU)]
        public byte occu;
        [SProperty((int)RelationAttr.RA_DAYGIFTNUM)]
        public byte dayGiftNum;
        [SProperty((int)RelationAttr.RA_TYPE)]
        public byte type;
        [SProperty((int)RelationAttr.RA_CREATETIME)]
        public UInt32 createTime;
        [SProperty((int)RelationAttr.RA_VIPLV)]
        public byte vipLv;
        [SProperty((int)RelationAttr.RA_STATUS)]
        public byte status;
		[SProperty((int)RelationAttr.RA_MASTER_DAYGIFTNUM)]
        public byte dailyGiftTimes;
        [SProperty((int)RelationAttr.RA_OFFLINE_TIME)]
        public UInt32 offlineTime;
        [SProperty((int)RelationAttr.RA_INTIMACY)]
        public UInt16 intimacy;
		[SProperty((int)RelationAttr.RA_DAILY_MASTERTASK_STATE)]
        public byte dailyMasterTaskState;
		[SProperty((int)RelationAttr.RA_REMARKS)]
        public string remark;
		[SProperty((int)RelationAttr.RA_IS_REGRESS)]
        public byte isRegress;
		[SProperty((int)RelationAttr.RA_MARK)]
        public UInt16 mark;
		[SProperty((int)RelationAttr.RA_HEAD_FRAME)]
        public UInt32 headFrame;  
		[SProperty((int)RelationAttr.RA_GUILD_ID)]
        public UInt64 guildId;
		[SProperty((int)RelationAttr.RA_RETURN_YEAR_TITLE)]
        public UInt32 returnYearTitle;  
        public byte isOnline; 
    }

	public  class RelationDecoder
    {
        public static List<Relation> DecodeList(byte[] buffer, ref int pos, int length)
        {
            List<Relation> relations = new List<Relation>();

            while (true)
            {
                UInt64 uid = 0;
                BaseDLL.decode_uint64(buffer, ref pos, ref uid);
                if (0 == uid) break;

                Relation relation = new Relation();
                BaseDLL.decode_int8(buffer, ref pos, ref relation.isOnline);
                StreamObjectDecoder<Relation>.DecodePropertys(ref relation, buffer, ref pos, length);

                relation.uid = uid;

                relations.Add(relation);
 
            }

            return relations;
        }

        public static Relation DecodeData(byte[] buffer, ref int pos, int length)
        {
            Relation relation = new Relation();
            //BaseDLL.decode_int8(buffer, ref pos, ref relation.type);
            BaseDLL.decode_uint64(buffer, ref pos, ref relation.uid);
            StreamObjectDecoder<Relation>.DecodePropertys(ref relation, buffer, ref pos, length);

            return relation;
        }

        public static Relation DecodeNew(byte[] buffer, ref int pos, int length)
        {
            Relation relation = new Relation();
            //BaseDLL.decode_int8(buffer, ref pos, ref relation.type);
            BaseDLL.decode_uint64(buffer, ref pos, ref relation.uid);
            BaseDLL.decode_int8(buffer, ref pos, ref relation.isOnline);
            StreamObjectDecoder<Relation>.DecodePropertys(ref relation, buffer, ref pos, length);

            return relation;
        }

    }

}
