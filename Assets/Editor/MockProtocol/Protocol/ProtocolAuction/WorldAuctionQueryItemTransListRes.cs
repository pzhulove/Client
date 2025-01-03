using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world->client  查询道具拍卖行交易记录返回
	/// </summary>
	[AdvancedInspector.Descriptor("world->client  查询道具拍卖行交易记录返回", "world->client  查询道具拍卖行交易记录返回")]
	public class WorldAuctionQueryItemTransListRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603936;
		public UInt32 Sequence;

		public UInt32 code;

		public UInt32 itemTypeId;

		public UInt32 strengthen;

		public byte enhanceType;

		public UInt32 beadBuffId;

		public AuctionTransaction[] transList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
			BaseDLL.encode_uint32(buffer, ref pos_, strengthen);
			BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
			BaseDLL.encode_uint32(buffer, ref pos_, beadBuffId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)transList.Length);
			for(int i = 0; i < transList.Length; i++)
			{
				transList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref strengthen);
			BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref beadBuffId);
			UInt16 transListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref transListCnt);
			transList = new AuctionTransaction[transListCnt];
			for(int i = 0; i < transList.Length; i++)
			{
				transList[i] = new AuctionTransaction();
				transList[i].decode(buffer, ref pos_);
			}
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
