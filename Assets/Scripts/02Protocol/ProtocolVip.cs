using System;
using System.Text;

namespace Protocol
{
	public enum MoneyManageStatus
	{
		MMS_NOT_BUY = 0,
		MMS_ACCOUNT_BUY = 1,
		MMS_PLAYER_BUY = 2,
		MMS_NEW_PLAYER_BUY = 3,
	}

	public class Vip : Protocol.IProtocolStream
	{
		public byte level;
		public UInt32 exp;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, level);
				BaseDLL.encode_uint32(buffer, ref pos_, exp);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
				BaseDLL.decode_uint32(buffer, ref pos_, ref exp);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, level);
				BaseDLL.encode_uint32(buffer, ref pos_, exp);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
				BaseDLL.decode_uint32(buffer, ref pos_, ref exp);
			}

			public int getLen()
			{
				int _len = 0;
				// level
				_len += 1;
				// exp
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneVipBuyItemReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 503301;
		public UInt32 Sequence;
		public byte vipLevel;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, vipLevel);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref vipLevel);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, vipLevel);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref vipLevel);
			}

			public int getLen()
			{
				int _len = 0;
				// vipLevel
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneVipBuyItemRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 503302;
		public UInt32 Sequence;
		public UInt32 result;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneGiveMoneyManageRewardReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 503303;
		public UInt32 Sequence;
		public byte rewardId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, rewardId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref rewardId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, rewardId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref rewardId);
			}

			public int getLen()
			{
				int _len = 0;
				// rewardId
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneGiveMoneyManageRewardRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 503304;
		public UInt32 Sequence;
		public UInt32 result;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

}
