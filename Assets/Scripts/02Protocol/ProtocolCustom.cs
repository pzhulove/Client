using System;
using System.Text;
using System.Collections.Generic;

/*****************************************
 *
 * 一些自定义的协议都放这里
 * 
 *****************************************/

namespace Protocol
{
    //  server->client 阅读邮件返回
    public class WorldReadMailRet : Protocol.IProtocolStream, Protocol.IGetMsgID
    {
        public const UInt32 MsgID = 601505;
        public UInt32 Sequence;
        public UInt64 id;
        public string content;
        // 物品
        public ItemReward[] items = new ItemReward[0];
        // 详细物品信息使用流的方式.
        public List<Item> detailItems = new List<Item>();

        #region METHOD
        public UInt32 GetMsgID()
        {
            return MsgID;
        }

        public void encode(byte[] buffer, ref int pos_)
        {
            BaseDLL.encode_uint64(buffer, ref pos_, id);
            byte[] contentBytes = StringHelper.StringToUTF8Bytes(content);
            BaseDLL.encode_string(buffer, ref pos_, contentBytes, (UInt16)(buffer.Length - pos_));
            BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
            for (int i = 0; i < items.Length; i++)
            {
                items[i].encode(buffer, ref pos_);
            }
        }

        public void decode(byte[] buffer, ref int pos_)
        {
            BaseDLL.decode_uint64(buffer, ref pos_, ref id);
            UInt16 contentLen = 0;
            BaseDLL.decode_uint16(buffer, ref pos_, ref contentLen);
            byte[] contentBytes = new byte[contentLen];
            for (int i = 0; i < contentLen; i++)
            {
                BaseDLL.decode_int8(buffer, ref pos_, ref contentBytes[i]);
            }
            content = StringHelper.BytesToString(contentBytes);
            UInt16 itemsCnt = 0;
            BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
            items = new ItemReward[itemsCnt];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new ItemReward();
                items[i].decode(buffer, ref pos_);
            }
            detailItems = ItemDecoder.Decode(buffer, ref pos_, buffer.Length);
        }

        public UInt32 GetSequence()
        {
            return Sequence;
        }

        public void SetSequence(UInt32 sequence)
        {
            Sequence = sequence;
        }
        #endregion

    }

}