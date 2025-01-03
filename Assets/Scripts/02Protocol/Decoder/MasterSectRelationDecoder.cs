using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using System.Reflection;

namespace Protocol
{
    public enum MasterSectRelationAttr
    {
        MSRA_INVALID,
        MSRA_NAME,    //对方名字(std::string)
        MSRA_LEVEL,    //对方等级(UInt16)
        MSRA_OCCU,    //对方职业(UInt8)
        MSRA_TYPE,    //关系(UInt8)
        MSRA_VIPLV,    //vip等级(UInt8)
        MSRA_STATUS,  //状态(UInt8)
		MSRA_ISFINSCH, //是否已经出师
    }
    public class MasterSectRelation : StreamObject
    {
        public UInt64 uid;
        [SProperty((int)MasterSectRelationAttr.MSRA_NAME)]
        public string name;
        [SProperty((int)MasterSectRelationAttr.MSRA_LEVEL)]
        public UInt16 level;
        [SProperty((int)MasterSectRelationAttr.MSRA_OCCU)]
        public byte occu;
        [SProperty((int)MasterSectRelationAttr.MSRA_TYPE)]
        public byte type;
        [SProperty((int)MasterSectRelationAttr.MSRA_VIPLV)]
        public byte vipLv;
        [SProperty((int)MasterSectRelationAttr.MSRA_STATUS)]
        public byte status;
		[SProperty((int)MasterSectRelationAttr.MSRA_ISFINSCH)]
        public byte isFinSch;
        public byte isOnline;
    }

    public class MasterSectRelationDecoder
    {
        public static List<MasterSectRelation> DecodeList(byte[] buffer, ref int pos, int length)
        {
            List<MasterSectRelation> masterSectRelations = new List<MasterSectRelation>();

            while(true)
            {
                UInt64 uid = 0;
                BaseDLL.decode_uint64(buffer, ref pos, ref uid);
                if (0 == uid) break;

                MasterSectRelation masterSectRelation = new MasterSectRelation();
                BaseDLL.decode_int8(buffer, ref pos, ref masterSectRelation.isOnline);
                StreamObjectDecoder<MasterSectRelation>.DecodePropertys(ref masterSectRelation, buffer, ref pos, length);

                masterSectRelation.uid = uid;

                masterSectRelations.Add(masterSectRelation);
            }

            return masterSectRelations;
        }

        public static MasterSectRelation DecodeData(byte[] buffer, ref int pos, int length)
        {
            MasterSectRelation masterSectRelation = new MasterSectRelation();
            //BaseDLL.decode_int8(buffer, ref pos, ref relation.type);
            BaseDLL.decode_uint64(buffer, ref pos, ref masterSectRelation.uid);
            StreamObjectDecoder<MasterSectRelation>.DecodePropertys(ref masterSectRelation, buffer, ref pos, length);

            return masterSectRelation;
        }

        public static MasterSectRelation DecodeNew(byte[] buffer, ref int pos, int length)
        {
            MasterSectRelation masterSectRelation = new MasterSectRelation();
            //BaseDLL.decode_int8(buffer, ref pos, ref relation.type);
            BaseDLL.decode_uint64(buffer, ref pos, ref masterSectRelation.uid);
            BaseDLL.decode_int8(buffer, ref pos, ref masterSectRelation.isOnline);
            StreamObjectDecoder<MasterSectRelation>.DecodePropertys(ref masterSectRelation, buffer, ref pos, length);

            return masterSectRelation;
        }
    }
}
