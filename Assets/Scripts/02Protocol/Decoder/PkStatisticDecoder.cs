using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using System.Reflection;

namespace Protocol
{
    public enum PkType
    {
        Pk_Normal_1v1 = 0,      // 传统练习
        Pk_Season_1v1 = 1,      // 赛季
        PK_Guild = 2,           // 公会战
        Pk_Wudao = 3,           // 武道大会
        Pk_Friends = 4,         // 好友切磋
        Pk_Premium_League_Preliminay = 5,         // 赏金联赛预选赛
        Pk_Premium_League_Battle = 6,         // 赏金联赛淘汰赛
        Pk_3V3_ROOM = 7,
        Pk_3V3_MATCH = 8,
        Pk_3V3_War_Score = 9,            // 3v3积分赛
        Pk_3V3_TUMBLE = 10,					// 3v3乱斗
        Pk_1V1_CHIJI = 11,					// 1v1吃鸡
        Pk_3V3_CHIJI = 12,					// 3v3吃鸡
        PK_EQUAL_1V1 = 13,           //公平竞技场
        PK_EQUAL_PRACTICE = 14,      // 公平竞技场好友切磋
        PK_2V2_ACTIVITY = 15,        // 2V2乱斗活动
    }

    public enum PkStatisticProperty
    {
        PKIA_INVALID,
        PKIA_TYPE,              // 类型	UInt8
        PKIA_TOTAL_WIN_NUM,     // 总胜场数 UInt32
        PKIA_TOTAL_LOSE_NUM,    // 总负场数 UInt32
        PKIA_TOTAL_NUM,         // 总场数 UInt32
        PKIA_DETAIL_RECORD_INFO,// 对战不同职业的战绩
        PKIA_RECENT_RECORD,     // 最近战绩
        PKIA_MAX_WIN_STEAK,     // 最多连胜
        PKIA_CUR_WIN_STEAK,     // 当前连胜
    }

    public class PkStatistic : StreamObject
    {
        public static byte MAX_RECENT_RECORD_NUM = 10;

        public UInt64 id;

        [SProperty((int)PkStatisticProperty.PKIA_TYPE)]
        public byte type;

        [SProperty((int)PkStatisticProperty.PKIA_TOTAL_WIN_NUM)]
        public UInt32 totalWinNum;

        [SProperty((int)PkStatisticProperty.PKIA_TOTAL_LOSE_NUM)]
        public UInt32 totalLoseNum;

        [SProperty((int)PkStatisticProperty.PKIA_TOTAL_NUM)]
        public UInt32 totalNum;

        [SProperty((int)PkStatisticProperty.PKIA_DETAIL_RECORD_INFO)]
        public Dictionary<byte, PkOccuRecord> detailRecord = new Dictionary<byte, PkOccuRecord>();

        [SProperty((int)PkStatisticProperty.PKIA_RECENT_RECORD)]
        public byte[] recentRecord = new byte[MAX_RECENT_RECORD_NUM];

        [SProperty((int)PkStatisticProperty.PKIA_MAX_WIN_STEAK)]
        public UInt32 maxWinSteak;

        [SProperty((int)PkStatisticProperty.PKIA_CUR_WIN_STEAK)]
        public UInt32 curWinSteak;

        public override bool GetStructProperty(FieldInfo field, byte[] buffer, ref int pos, int length)
        {
            if (field.FieldType == typeof(Dictionary<byte, PkOccuRecord>))
            {
                while(length - pos > 1)
                {
                    byte hasNext = 0;
                    BaseDLL.decode_int8(buffer, ref pos, ref hasNext);
                    if(hasNext == 0)
                    {
                        break;
                    }

                    PkOccuRecord record = new PkOccuRecord();
                    record.decode(buffer, ref pos);

                    if(detailRecord.ContainsKey(record.occu))
                    {
                        detailRecord.Remove(record.occu);
                    }
                    detailRecord.Add(record.occu, record);
                }
                return true;
            }
            else if (field.FieldType == typeof(byte[]))
            {
                for(int i = 0; i < recentRecord.Length; i++)
                {
                    BaseDLL.decode_int8(buffer, ref pos, ref recentRecord[i]);
                }
                return true;
            }

            return false;
        }
    }

	public class PkStatisticDecoder
    {
        public static Dictionary<byte, PkStatistic> DecodeSyncPkStatisticInfoMsg(byte[] buffer, ref int pos, int length)
        {
            Dictionary<byte, PkStatistic> pkStatistics = new Dictionary<byte, PkStatistic>();

            while(length - pos > 1)
            {
                UInt64 id = 0;
                BaseDLL.decode_uint64(buffer, ref pos, ref id);
                if(id == 0)
                {
                    break;
                }

                PkStatistic info = new PkStatistic();
                info.id = id;
                StreamObjectDecoder<PkStatistic>.DecodePropertys(ref info, buffer, ref pos, length);

                pkStatistics.Add(info.type, info);
            }

            return pkStatistics;
        }

        public static void DecodeSyncPkStatisticPropertyMsg(ref PkStatistic info, byte[] buffer, ref int pos, int length)
        {
            StreamObjectDecoder<PkStatistic>.DecodePropertys(ref info, buffer, ref pos, length);
        }
    }
}
